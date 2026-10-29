#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2026 Dr. Dirk Lellinger
//
//    This program is free software; you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation; either version 2 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with this program; if not, write to the Free Software
//    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
//
/////////////////////////////////////////////////////////////////////////////

#endregion Copyright

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Altaxo.Chat
{
  /// <summary>
  /// MCP stdio proxy between an MCP client (e.g. Claude) and a running Altaxo instance.
  /// <para>
  /// Normal operation: every line received on stdin is forwarded to the named pipe of Altaxo,
  /// and every line received from the pipe is forwarded to stdout.
  /// </para>
  /// <para>
  /// Problem solved by this version: the MCP client usually starts the proxy once, and calls
  /// 'initialize' / 'tools/list' only at that time. If Altaxo is not running yet, the old proxy
  /// simply exited and the client never got the Altaxo tools. Now the proxy stays alive and
  /// answers the MCP handshake ITSELF ("local mode", with an empty tool list). In the background
  /// it waits for Altaxo. As soon as Altaxo is available, it
  /// (1) connects to the pipe,
  /// (2) replays the client's handshake to Altaxo (the answers are swallowed, because the client
  ///     already got its answers from the proxy),
  /// (3) sends 'notifications/tools/list_changed' to the client, which then asks for the tool list again
  ///     (this request is forwarded to Altaxo) and thus gets the Altaxo tools.
  /// If Altaxo is closed later, the proxy returns to local mode, tells the client that the tools changed
  /// (now: empty) and waits for the next Altaxo instance.
  /// </para>
  /// </summary>
  public class MCPServerProxy
  {
    #region Public configuration

    public const string DefaultPipeName = "Altaxo.Chat";

    /// <summary>Base name of the pipe. The Altaxo process id is appended: "Altaxo.Chat.{pid}".</summary>
    public string PipeName { get; init; } = DefaultPipeName;

    /// <summary>Optional name of a log file. If null, nothing is logged.</summary>
    public string? LogfileName { get; init; }

    /// <summary>
    /// Maximum time to wait for an Altaxo instance. Default: infinite
    /// (the proxy ends anyway as soon as the MCP client closes stdin).
    /// </summary>
    public TimeSpan StartupTimeout { get; init; } = Timeout.InfiniteTimeSpan;

    /// <summary>Interval used to poll for Altaxo processes and for their pipe.</summary>
    public TimeSpan PollInterval { get; init; } = TimeSpan.FromMilliseconds(500);

    /// <summary>Maximum time Altaxo may take to answer the replayed 'initialize' request.</summary>
    public TimeSpan HandshakeTimeout { get; init; } = TimeSpan.FromSeconds(30);

    public string TimeTag => $"{DateTime.Now:yyyy-MM-dd-HH-mm-ss}";

    #endregion

    #region Constants

    private static readonly string[] AltaxoProcessNames = ["AltaxoStartup", "AltaxoStartup32", "AltaxoStartup64"];

    // Protocol version we announce if the client's 'initialize' request does not contain one.
    private const string FallbackProtocolVersion = "2025-03-26";

    // Fixed JSON-RPC notifications (one line each, as required by the MCP stdio transport).
    private const string ToolsListChangedNotification = """{"jsonrpc":"2.0","method":"notifications/tools/list_changed"}""";
    private const string InitializedNotification = """{"jsonrpc":"2.0","method":"notifications/initialized"}""";

    #endregion

    #region Event model

    // All things that can happen are funneled into ONE channel and processed sequentially by ONE consumer
    // (the main loop in RunAsync). This avoids races between the stdin reader, the connect task and the
    // pipe reader, and means that the state fields below need no locking.
    private enum EventKind
    {
      /// <summary>A line was received from the MCP client (stdin).</summary>
      ClientLine,
      /// <summary>stdin was closed: the MCP client has gone away, the proxy must end.</summary>
      StdinClosed,
      /// <summary>The connect task has connected to the pipe of an Altaxo instance.</summary>
      Connected,
      /// <summary>The connect task gave up because StartupTimeout elapsed.</summary>
      ConnectTimeout,
      /// <summary>The pipe to Altaxo was closed (Altaxo ended).</summary>
      PipeClosed,
    }

    private sealed record ProxyEvent(EventKind Kind, string? Line = null, NamedPipeClientStream? Pipe = null, int Generation = 0);

    #endregion

    #region State (only touched by the main loop, except where noted)

    private TextWriter? _log;                 // synchronized, may be written from several tasks

    private TextWriter? _stdout;              // may be written from the main loop AND the pipe reader task ...
    private readonly object _stdoutLock = new(); // ... therefore every write is done under this lock

    private StreamWriter? _pipeWriter;        // non-null <=> we are connected to Altaxo ("transparent mode")
    private StreamReader? _pipeReader;
    private int _generation;                  // identifies the current pipe connection; filters stale PipeClosed events

    private string? _initializeLine;          // the client's original 'initialize' request (for replay to Altaxo)
    private bool _initializedReceived;        // the client has sent 'notifications/initialized'
    private bool _pendingListChanged;         // 'list_changed' must be sent as soon as the client is initialized

    #endregion

    private void Log(string message)
    {
      try { _log?.WriteLine($"{TimeTag}\t{message}"); }
      catch { /* logging must never break the proxy */ }
    }

    /// <summary>Writes one line to the MCP client (stdout). Thread-safe.</summary>
    /// <param name="line">The line to send.</param>
    /// <param name="logLine">
    /// If false, the line itself is not logged. Used for lines which were forwarded from Altaxo and
    /// are therefore already in the log as 'PIPE-READ' (avoids logging large messages twice).
    /// </param>
    private void SendToClient(string line, bool logLine = true)
    {
      lock (_stdoutLock)
      {
        try
        {
          _stdout?.WriteLine(line); // AutoFlush is on
          if (logLine)
            Log($"STDOUT: {line}");
        }
        catch (Exception ex)
        {
          Log($"STDOUT: exception {ex.Message}");
        }
      }
    }

    public async Task RunAsync()
    {
      if (LogfileName is not null)
      {
        // Synchronized, because the log is written from several tasks concurrently.
        _log = TextWriter.Synchronized(new StreamWriter(LogfileName, true, System.Text.Encoding.UTF8) { AutoFlush = true });
        Log("MCP proxy started");
      }

      // Cancelled when the proxy ends, so that a still-running connect task stops.
      using var shutdown = new CancellationTokenSource();

      try
      {
        using var conreader = new StreamReader(Console.OpenStandardInput());
        // NewLine = "\n": MCP stdio messages are newline-delimited; avoid "\r\n" on Windows.
        _stdout = new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true, NewLine = "\n" };

        var events = Channel.CreateUnbounded<ProxyEvent>(new UnboundedChannelOptions { SingleReader = true, SingleWriter = false });

        // (1) Read stdin from the very first moment, so that no client message is lost.
        _ = Task.Run(async () =>
        {
          try
          {
            while (await conreader.ReadLineAsync() is { } line)
              await events.Writer.WriteAsync(new ProxyEvent(EventKind.ClientLine, Line: line));
          }
          catch (Exception ex)
          {
            Log($"STDIN: exception {ex}");
            Console.Error.WriteLine($"Error reading from stdin: {ex.Message}");
          }
          finally
          {
            events.Writer.TryWrite(new ProxyEvent(EventKind.StdinClosed));
          }
        });

        // (2) Look for Altaxo in the background. Meanwhile the main loop below answers the client locally.
        StartConnectTask(events, TimeSpan.Zero, shutdown.Token);

        // (3) Main loop: process all events one after the other.
        bool exit = false;
        await foreach (var ev in events.Reader.ReadAllAsync())
        {
          switch (ev.Kind)
          {
            case EventKind.ClientLine:
              await HandleClientLineAsync(ev.Line!);
              break;

            case EventKind.Connected:
              if (!await TryEstablishConnectionAsync(ev.Pipe!, events))
              {
                // Process found, but the handshake failed (e.g. Altaxo is just shutting down): try again later.
                StartConnectTask(events, PollInterval, shutdown.Token);
              }
              break;

            case EventKind.PipeClosed:
              if (ev.Generation == _generation && _pipeWriter is not null)
                HandleAltaxoGone(events, shutdown.Token);
              break; // otherwise: stale event of an older connection, ignore

            case EventKind.ConnectTimeout:
              Log("Timeout while waiting for Altaxo.");
              Console.Error.WriteLine("Timeout while waiting for a running Altaxo instance.");
              Environment.ExitCode = 1;
              exit = true;
              break;

            case EventKind.StdinClosed:
              Log("stdin closed by client");
              exit = true;
              break;
          }

          if (exit)
            break;
        }
      }
      catch (Exception ex)
      {
        Console.Error.WriteLine($"Error in MCPServerProxy: {ex.Message}");
        Log($"Error: {ex.Message}");
      }
      finally
      {
        shutdown.Cancel();
        _pipeWriter?.Dispose();
        _pipeReader?.Dispose();
        Log("PROGRAM CLOSED");
      }
    }

    #region Handling of client messages

    /// <summary>
    /// Handles one line from the MCP client. Depending on the state, the line is
    /// forwarded to Altaxo (connected) or answered locally (not connected).
    /// </summary>
    private async Task HandleClientLineAsync(string line)
    {
      Log($"STDIN: {line}");

      // Parse the JSON-RPC envelope: 'method' (absent for responses) and 'id' (absent for notifications).
      string? method = null;
      JsonNode? id = null;
      JsonObject? message = null;
      try
      {
        message = JsonNode.Parse(line) as JsonObject;
        method = message?["method"]?.GetValue<string>();
        id = message?["id"];
      }
      catch (Exception ex)
      {
        Log($"Could not parse client line: {ex.Message}");
      }

      // Remember the handshake messages in ANY state. We need them to replay the handshake to
      // Altaxo, even if Altaxo was already connected when the client initialized (transparent mode)
      // and appears again later after a restart.
      if (method == "initialize")
        _initializeLine = line;
      else if (method == "notifications/initialized")
        _initializedReceived = true;

      // --- Connected: transparent mode, simply forward everything -----------------------------------------
      if (_pipeWriter is not null)
      {
        try
        {
          await _pipeWriter.WriteLineAsync(line);
        }
        catch (Exception ex)
        {
          // Do not react here: the pipe reader task will detect the closed pipe and post PipeClosed.
          Log($"PIPE-WRITE: exception {ex.Message}");
        }

        // The line itself is already in the log as 'STDIN' (logged at the top of this method).
        Log("PIPE-WRITE: (line above forwarded to Altaxo)");

        // Altaxo connected before the client had finished its handshake: the notification
        // was deferred (see TryEstablishConnectionAsync) and can be sent now.
        if (method == "notifications/initialized" && _pendingListChanged)
        {
          _pendingListChanged = false;
          SendToClient(ToolsListChangedNotification);
        }

        return;
      }

      // --- Not connected: answer locally ---------------------------------------------------------------------
      if (message is null)
        return; // unparseable, nothing sensible to do

      if (method is null)
        return; // a response from the client to a request of a server: there is no such request, ignore

      if (id is null)
      {
        // Notification (e.g. 'notifications/initialized', 'notifications/cancelled'): needs no answer.
        // 'notifications/initialized' was already remembered above and is replayed to Altaxo later.
        return;
      }

      HandleLocalRequest(method, id, message);
    }

    /// <summary>Answers a request of the client while Altaxo is not available.</summary>
    private void HandleLocalRequest(string method, JsonNode id, JsonObject request)
    {
      switch (method)
      {
        case "initialize":
          // Echo the protocol version requested by the client; announce that our tool list can change.
          // This 'listChanged' capability is what allows us to push the Altaxo tools later.
          var protocolVersion = request["params"]?["protocolVersion"]?.ToString() ?? FallbackProtocolVersion;
          SendToClient(MakeResult(id, new JsonObject
          {
            ["protocolVersion"] = protocolVersion,
            ["capabilities"] = new JsonObject { ["tools"] = new JsonObject { ["listChanged"] = true } },
            ["serverInfo"] = new JsonObject { ["name"] = "Altaxo.Chat.Proxy", ["version"] = "1.0.0" },
          }));
          break;

        case "ping":
          SendToClient(MakeResult(id, new JsonObject()));
          break;

        case "tools/list":
          // No tools as long as Altaxo is not running.
          SendToClient(MakeResult(id, new JsonObject { ["tools"] = new JsonArray() }));
          break;

        case "tools/call":
          // Should not happen (the tool list is empty), but if the client calls a (stale) tool anyway,
          // give a result the language model can understand and report to the user.
          SendToClient(MakeResult(id, new JsonObject
          {
            ["content"] = new JsonArray(new JsonObject
            {
              ["type"] = "text",
              ["text"] = "Altaxo is currently not running or not yet reachable. Please start Altaxo; the Altaxo tools become available automatically.",
            }),
            ["isError"] = true,
          }));
          break;

        default:
          // Everything else (resources/*, prompts/*, ...) is not offered by this proxy.
          SendToClient(MakeError(id, -32601, $"Method not found: {method}"));
          break;
      }
    }

    private static string MakeResult(JsonNode id, JsonObject result) =>
      new JsonObject { ["jsonrpc"] = "2.0", ["id"] = id.DeepClone(), ["result"] = result }.ToJsonString();

    private static string MakeError(JsonNode id, int code, string message) =>
      new JsonObject
      {
        ["jsonrpc"] = "2.0",
        ["id"] = id.DeepClone(), // DeepClone: a node can only have one parent
        ["error"] = new JsonObject { ["code"] = code, ["message"] = message },
      }.ToJsonString();

    #endregion

    #region Connecting to / losing Altaxo

    /// <summary>
    /// Takes a freshly connected pipe into operation: replays the client's handshake to Altaxo,
    /// notifies the client that the tools have changed, and starts forwarding Altaxo's output.
    /// </summary>
    /// <returns>True on success; false if the handshake failed (the pipe is then disposed).</returns>
    private async Task<bool> TryEstablishConnectionAsync(NamedPipeClientStream pipe, Channel<ProxyEvent> events)
    {
      var reader = new StreamReader(pipe);
      var writer = new StreamWriter(pipe) { AutoFlush = true, NewLine = "\n" };

      try
      {
        if (_initializeLine is not null)
        {
          // The client has already been initialized by us. Now Altaxo must go through the same
          // handshake, with exactly the parameters the client sent.
          Log("Replaying initialize to Altaxo");
          await writer.WriteLineAsync(_initializeLine);

          // Altaxo's answer is swallowed: the client already got an answer (from us) to this request id.
          using var timeout = new CancellationTokenSource(HandshakeTimeout);
          var reply = await reader.ReadLineAsync(timeout.Token);
          if (reply is null)
            throw new IOException("Altaxo closed the pipe during the handshake.");
          Log($"PIPE-READ (swallowed handshake answer): {reply}");

          // If the client has already sent 'notifications/initialized' (we swallowed it while not connected),
          // Altaxo needs it too. Otherwise the client sends it later, and it is forwarded in transparent mode.
          if (_initializedReceived)
          {
            Log("Replaying notifications/initialized to Altaxo");
            await writer.WriteLineAsync(InitializedNotification);
          }
        }
        // else: the client has not sent anything yet. Nothing to replay; everything it sends
        // later (including 'initialize') is simply forwarded.
      }
      catch (Exception ex)
      {
        Log($"Handshake with Altaxo failed: {ex.Message}");
        reader.Dispose(); // also closes the pipe
        writer.Dispose();
        return false;
      }

      // From now on: transparent mode. The pipe reader forwards everything Altaxo says to the client.
      _pipeReader = reader;
      _pipeWriter = writer;
      int generation = ++_generation;
      _ = Task.Run(() => PumpPipeToClientAsync(reader, generation, events));

      Log("PROGRAM OPENED; connected to Altaxo, transparent mode");

      // Tell the client that the tool list has changed. The MCP specification says a server must
      // not send such notifications before it got 'notifications/initialized', so defer if needed.
      if (_initializedReceived)
        SendToClient(ToolsListChangedNotification);
      else if (_initializeLine is not null)
        _pendingListChanged = true;

      return true;
    }

    /// <summary>
    /// Runs as a task: forwards every line of Altaxo to the client, and posts a PipeClosed event when the pipe ends.
    /// </summary>
    private async Task PumpPipeToClientAsync(StreamReader reader, int generation, Channel<ProxyEvent> events)
    {
      try
      {
        while (await reader.ReadLineAsync() is { } line)
        {
          Log($"PIPE-READ: {line}");
          SendToClient(line, logLine: false); // already logged above
        }
      }
      catch (Exception ex)
      {
        Log($"PIPE-READ: exception {ex.Message}");
      }

      events.Writer.TryWrite(new ProxyEvent(EventKind.PipeClosed, Generation: generation));
    }

    /// <summary>
    /// Altaxo has ended: go back to local mode, tell the client that the tools are gone,
    /// and wait for the next Altaxo instance.
    /// </summary>
    private void HandleAltaxoGone(Channel<ProxyEvent> events, CancellationToken shutdownToken)
    {
      Log("Pipe to Altaxo closed; back to local mode");

      _pipeWriter?.Dispose();
      _pipeReader?.Dispose();
      _pipeWriter = null;
      _pipeReader = null;
      _pendingListChanged = false;

      // The client asks again for the tool list and now gets the empty local list.
      if (_initializedReceived)
        SendToClient(ToolsListChangedNotification);

      // Short delay: the old process may still show up in the process list for a moment.
      StartConnectTask(events, PollInterval * 2, shutdownToken);
    }

    /// <summary>
    /// Starts a background task which waits for an Altaxo instance and posts a Connected (or ConnectTimeout) event.
    /// </summary>
    private void StartConnectTask(Channel<ProxyEvent> events, TimeSpan initialDelay, CancellationToken cancellationToken)
    {
      _ = Task.Run(async () =>
      {
        try
        {
          if (initialDelay > TimeSpan.Zero)
            await Task.Delay(initialDelay, cancellationToken);

          var pipe = await WaitForAltaxoAsync(cancellationToken);
          if (pipe is not null)
            await events.Writer.WriteAsync(new ProxyEvent(EventKind.Connected, Pipe: pipe), cancellationToken);
          else if (!cancellationToken.IsCancellationRequested)
            events.Writer.TryWrite(new ProxyEvent(EventKind.ConnectTimeout));
        }
        catch (OperationCanceledException)
        {
          // proxy is ending
        }
      });
    }

    /// <summary>
    /// Polls until an Altaxo process exists AND its pipe accepts a connection.
    /// Returns null on timeout or cancellation.
    /// </summary>
    private async Task<NamedPipeClientStream?> WaitForAltaxoAsync(CancellationToken cancellationToken)
    {
      var stopwatch = Stopwatch.StartNew();
      int lastProcessCount = -1;
      string? lastError = null;
      bool announcedWaiting = false;

      while (!cancellationToken.IsCancellationRequested)
      {
        var processIds = GetAltaxoProcessIds();

        if (processIds.Count != lastProcessCount) // log only changes, not every poll
        {
          Log($"{processIds.Count} running Altaxo instance(s) found");
          lastProcessCount = processIds.Count;
        }

        if (processIds.Count == 0 && !announcedWaiting)
        {
          Console.Error.WriteLine("No running Altaxo instance found; waiting for Altaxo to start...");
          announcedWaiting = true;
        }

        // The process may exist before its pipe server is up (Altaxo is still starting),
        // so the pipe connection is retried on every poll, for every instance.
        foreach (var processId in processIds)
        {
          var targetPipeName = $"{PipeName}.{processId}";
          var candidate = new NamedPipeClientStream(".", targetPipeName, PipeDirection.InOut, PipeOptions.Asynchronous);
          try
          {
            await candidate.ConnectAsync(1000, cancellationToken);
            Log($"Connected to pipe '{targetPipeName}'");
            return candidate;
          }
          catch (OperationCanceledException)
          {
            candidate.Dispose();
            return null;
          }
          catch (Exception ex)
          {
            candidate.Dispose();
            var error = $"Error connecting to named pipe '{targetPipeName}': {ex.Message}";
            if (error != lastError) // avoid flooding the log while polling
            {
              Log(error);
              lastError = error;
            }
          }
        }

        if (StartupTimeout != Timeout.InfiniteTimeSpan && stopwatch.Elapsed > StartupTimeout)
          return null;

        try
        {
          await Task.Delay(PollInterval, cancellationToken);
        }
        catch (OperationCanceledException)
        {
          return null;
        }
      }

      return null;
    }

    private static List<int> GetAltaxoProcessIds()
    {
      var processIds = new List<int>();
      foreach (var processName in AltaxoProcessNames)
      {
        foreach (var process in Process.GetProcessesByName(processName))
        {
          using (process)
            processIds.Add(process.Id);
        }
      }

      processIds.Sort(); // lowest process id first, as in the original version
      return processIds;
    }

    #endregion
  }
}
