namespace Altaxo.Serialization.AutoUpdates;


using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;


public class AdaptiveParallelDownloaderOfSameFile
{
  private const int ChunkSize = 1 * 1024 * 1024; // 1 MB chunks
  private const int MonitoringInterval = 500;              // ms

  private readonly string[] _serverUrls;
  private readonly string _outputPath;

  private long?[] _throttleLimits;
  private long[] _totalBytesPerServer;
  private long _fileSize;
  private int _totalChunks;
  private int[] _chunksPerServer;
  private double[] _throughputPerServer;

  private readonly SemaphoreSlim _throttleLock = new(1, 1);
  private readonly SemaphoreSlim _consoleLock = new(1, 1);

  private int _consoleOriginRow;

  /// <summary>
  /// Initializes a new instance of the <see cref="AdaptiveParallelDownloaderOfSameFile"/> class.
  /// </summary>
  /// <param name="serverUrls">The mirror URLs that provide the same file content.</param>
  /// <param name="outputPath">The path of the output file to create.</param>
  public AdaptiveParallelDownloaderOfSameFile(string[] serverUrls, string outputPath)
  {
    _serverUrls = serverUrls;
    _outputPath = outputPath;
    _throttleLimits = new long?[serverUrls.Length];
    _totalBytesPerServer = new long[serverUrls.Length];
    _chunksPerServer = new int[serverUrls.Length];
    _throughputPerServer = new double[serverUrls.Length];
  }

  /// <summary>
  /// Downloads the file by splitting it into chunks and retrieving them from multiple servers.
  /// </summary>
  /// <param name="fileSize">The expected size of the file in bytes.</param>
  /// <param name="ct">A token that cancels the download.</param>
  /// <returns>A task that represents the asynchronous download operation.</returns>
  public async Task DownloadAsync(long fileSize, CancellationToken ct = default)
  {
    _fileSize = fileSize;
    _totalChunks = (int)Math.Ceiling((double)fileSize / ChunkSize);

    var chunkQueue = new ConcurrentQueue<int>(Enumerable.Range(0, _totalChunks));
    var fileBuffer = new byte[fileSize];

    InitConsoleLayout();

    using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);

    var monitorTask = MonitorAndThrottleAsync(cts.Token);

    var workers = _serverUrls.Select((url, idx) =>
        DownloadWorkerAsync(url, idx, chunkQueue, fileBuffer, cts.Token)
    ).ToArray();

    await Task.WhenAll(workers);
    await cts.CancelAsync();

    try { await monitorTask; } catch (OperationCanceledException) { }

    RenderProgress(final: true);

    await File.WriteAllBytesAsync(_outputPath, fileBuffer, ct);

    Console.SetCursorPosition(0, _consoleOriginRow + _serverUrls.Length + 3);
    Console.CursorVisible = true;
    Console.WriteLine($"\n✓ Saved to {_outputPath}");
  }

  // -------------------------------------------------------------------------
  //  Worker
  // -------------------------------------------------------------------------
  private async Task DownloadWorkerAsync(
      string baseUrl, int serverIndex,
      ConcurrentQueue<int> chunkQueue,
      byte[] fileBuffer, CancellationToken ct)
  {
    using var client = new HttpClient();

    while (chunkQueue.TryDequeue(out int chunkIndex))
    {
      long start = (long)chunkIndex * ChunkSize;
      long end = Math.Min(start + ChunkSize - 1, _fileSize - 1);

      var request = new HttpRequestMessage(HttpMethod.Get, baseUrl);
      request.Headers.Range = new RangeHeaderValue(start, end);

      var response = await client.SendAsync(
          request, HttpCompletionOption.ResponseHeadersRead, ct);

      using var stream = await response.Content.ReadAsStreamAsync(ct);

      await ReadThrottledAsync(stream, fileBuffer, start,
          end - start + 1, serverIndex, ct);

      Interlocked.Increment(ref _chunksPerServer[serverIndex]);
    }
  }

  // -------------------------------------------------------------------------
  //  Throttled read
  // -------------------------------------------------------------------------
  private async Task ReadThrottledAsync(
      Stream stream, byte[] buffer,
      long offset, long length,
      int serverIndex, CancellationToken ct)
  {
    var tempBuf = new byte[64 * 1024];
    long remaining = length;
    long position = offset;
    var windowStart = DateTime.UtcNow;
    long bytesInWindow = 0;

    while (remaining > 0)
    {
      int toRead = (int)Math.Min(tempBuf.Length, remaining);
      int read = await stream.ReadAsync(tempBuf, 0, toRead, ct);
      if (read == 0) break;

      Array.Copy(tempBuf, 0, buffer, position, read);
      position += read;
      remaining -= read;

      Interlocked.Add(ref _totalBytesPerServer[serverIndex], read);

      // --- Token-bucket throttle ---
      var limit = _throttleLimits[serverIndex];
      if (limit.HasValue && limit.Value > 0)
      {
        bytesInWindow += read;
        double elapsed = (DateTime.UtcNow - windowStart).TotalSeconds;
        double allowed = limit.Value * elapsed;

        if (bytesInWindow > allowed)
        {
          double waitSec = (bytesInWindow - allowed) / limit.Value;
          int waitMs = (int)(waitSec * 1000);
          if (waitMs > 0) await Task.Delay(waitMs, ct);
        }

        // Reset window every second
        if ((DateTime.UtcNow - windowStart).TotalSeconds >= 1)
        {
          windowStart = DateTime.UtcNow;
          bytesInWindow = 0;
        }
      }
    }
  }

  // -------------------------------------------------------------------------
  //  Monitor: throughput measurement + adaptive throttle
  // -------------------------------------------------------------------------
  private async Task MonitorAndThrottleAsync(CancellationToken ct)
  {
    const double alpha = 0.4;
    var previousBytes = new long[_serverUrls.Length];

    // Initialise baseline so the first interval isn't artificially large
    for (int i = 0; i < _serverUrls.Length; i++)
      previousBytes[i] = Interlocked.Read(ref _totalBytesPerServer[i]);

    while (!ct.IsCancellationRequested)
    {
      await Task.Delay(MonitoringInterval, ct);

      for (int i = 0; i < _serverUrls.Length; i++)
      {
        // Monotonically increasing — delta is always >= 0
        long current = Interlocked.Read(ref _totalBytesPerServer[i]);
        long delta = current - previousBytes[i];
        previousBytes[i] = current;

        double raw = delta * (1000.0 / MonitoringInterval);
        _throughputPerServer[i] = _throughputPerServer[i] == 0
            ? raw
            : alpha * raw + (1 - alpha) * _throughputPerServer[i];
      }

      // --- Adaptive throttle ---
      double maxTp = _throughputPerServer.Max();
      await _throttleLock.WaitAsync(ct);
      try
      {
        for (int i = 0; i < _serverUrls.Length; i++)
        {
          double ratio = maxTp > 0 ? _throughputPerServer[i] / maxTp : 1;
          _throttleLimits[i] = ratio < 0.5
              ? (long)_throughputPerServer[i]
              : null;
        }
      }
      finally { _throttleLock.Release(); }

      RenderProgress();
    }
  }

  // =========================================================================
  //  Console UI
  // =========================================================================

  private void InitConsoleLayout()
  {
    Console.CursorVisible = false;
    _consoleOriginRow = Console.CursorTop;

    for (int i = 0; i < _serverUrls.Length + 3; i++)
      Console.WriteLine();
  }

  private void RenderProgress(bool final = false)
  {
    _consoleLock.Wait();
    try
    {
      const int barWidth = 30;
      long totalBytes = _totalBytesPerServer.Sum();
      double overall = _fileSize > 0
          ? Math.Min(1.0, (double)totalBytes / _fileSize)
          : 0;

      int row = _consoleOriginRow;

      // --- Header ---
      SetRow(row++);
      WriteColored("  Adaptive Parallel Download\n", ConsoleColor.Cyan);

      // --- Per-server bars ---
      for (int i = 0; i < _serverUrls.Length; i++)
      {
        SetRow(row++);

        double serverShare = _fileSize > 0
            ? Math.Min(1.0, (double)_totalBytesPerServer[i] / _fileSize)
            : 0;

        bool throttled = _throttleLimits[i].HasValue;
        string speed = FormatSpeed(_throughputPerServer[i]);
        string chunks = $"{_chunksPerServer[i]}/{_totalChunks} chunks";
        string flag = throttled ? " [throttled]" : "            ";

        Console.Write($"  Server {i + 1,-3} ");
        DrawBar(serverShare, barWidth,
            throttled ? ConsoleColor.Yellow : ConsoleColor.Green);
        Console.Write($" {serverShare,5:P0}  {speed,12}  {chunks,-20}");
        WriteColored(flag,
            throttled ? ConsoleColor.Yellow : ConsoleColor.DarkGray);
        ClearToEnd();
      }

      // --- Overall bar ---
      SetRow(row++);
      Console.Write("  Overall    ");
      DrawBar(overall, barWidth, ConsoleColor.Cyan);
      Console.Write($" {overall,5:P0}  {FormatBytes(totalBytes)} / {FormatBytes(_fileSize)}");
      ClearToEnd();

      // --- Status line ---
      SetRow(row);
      WriteColored(
          final ? "  ✓ Complete!" : "  Downloading…",
          final ? ConsoleColor.Green : ConsoleColor.DarkGray);
      ClearToEnd();
    }
    finally { _consoleLock.Release(); }
  }

  // -------------------------------------------------------------------------
  //  Console helpers
  // -------------------------------------------------------------------------

  private static void SetRow(int row) =>
      Console.SetCursorPosition(0, row);

  private static void DrawBar(double fraction, int width, ConsoleColor color)
  {
    int filled = Math.Clamp((int)Math.Round(fraction * width), 0, width);
    Console.Write("[");
    Console.ForegroundColor = color;
    Console.Write(new string('█', filled));
    Console.ResetColor();
    Console.Write(new string('░', width - filled));
    Console.Write("]");
  }

  private static void WriteColored(string text, ConsoleColor color)
  {
    Console.ForegroundColor = color;
    Console.Write(text);
    Console.ResetColor();
  }

  private static void ClearToEnd()
  {
    int remaining = Console.WindowWidth - Console.CursorLeft;
    if (remaining > 0) Console.Write(new string(' ', remaining));
  }

  private static string FormatSpeed(double bytesPerSec) =>
      bytesPerSec switch
      {
        >= 1_000_000 => $"{bytesPerSec / 1_000_000:F1} MB/s",
        >= 1_000 => $"{bytesPerSec / 1_000:F1} KB/s",
        _ => $"{bytesPerSec:F0} B/s"
      };

  private static string FormatBytes(long bytes) =>
      bytes switch
      {
        >= 1_073_741_824 => $"{bytes / 1_073_741_824.0:F1} GB",
        >= 1_048_576 => $"{bytes / 1_048_576.0:F1} MB",
        >= 1_024 => $"{bytes / 1_024.0:F1} KB",
        _ => $"{bytes} B"
      };
}
