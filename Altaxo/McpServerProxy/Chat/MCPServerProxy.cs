using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Reflection;
using System.Threading.Tasks;

namespace Altaxo.Chat
{
  public class MCPServerProxy
  {
    public const string DefaultPipeName = "Altaxo.Chat";
    private static readonly string[] AltaxoProcessNames = ["AltaxoStartup", "AltaxoStartup32", "AltaxoStartup64"];

    public string PipeName { get; init; } = DefaultPipeName;

    public async Task RunAsync()
    {
      var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
      using var logFileStream = new StreamWriter(Path.Combine(dir, "MCPServerProxy.log"), true, System.Text.Encoding.UTF8) { AutoFlush = true };

      try
      {
        var processIds = GetAltaxoProcessIds();
        if (processIds.Count == 0)
          throw new InvalidOperationException("No running Altaxo instance was found.");

        var targetPipeName = $"{PipeName}.{processIds[0]}";
        NamedPipeClientStream? pipe = null;

        for (int i = 0; i < 30; ++i)
        {
          var candidate = new NamedPipeClientStream(".", targetPipeName, PipeDirection.InOut, PipeOptions.Asynchronous);
          try
          {
            await candidate.ConnectAsync(1000);
            pipe = candidate;
            break;
          }
          catch (Exception ex)
          {
            candidate.Dispose();
            logFileStream.WriteLine($"[{DateTime.Now}] Error connecting to named pipe '{targetPipeName}': {ex.Message}");
            Console.Error.WriteLine($"Error connecting to named pipe '{targetPipeName}': {ex.Message}");
          }
        }

        if (pipe is null)
          throw new IOException($"Could not connect to named pipe '{targetPipeName}' after 30 attempts.");

        using var connectedPipe = pipe;
        using var pipeReader = new StreamReader(connectedPipe);
        using var pipeWriter = new StreamWriter(connectedPipe) { AutoFlush = true };
        using var conreader = new StreamReader(Console.OpenStandardInput());
        using var conwriter = new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true };

        logFileStream.WriteLine($"[{DateTime.Now}] PROGRAM OPENED; connected to named pipe '{targetPipeName}'");

        var pipeToStdoutTask = Task.Run(async () =>
        {
          try
          {
            while (await pipeReader.ReadLineAsync() is { } line)
            {
              logFileStream.WriteLine($"[{DateTime.Now}] PIPE-CON: {line}");
              conwriter.WriteLine(line);
              conwriter.Flush();
            }
          }
          catch (Exception ex)
          {
            logFileStream.WriteLine($"[{DateTime.Now}] PIPE-CON: exception {ex}");
            Console.Error.WriteLine($"Error reading from named pipe: {ex.Message}");
          }
        });

        var stdinToPipeTask = Task.Run(async () =>
        {
          try
          {
            while (await conreader.ReadLineAsync() is { } line)
            {
              logFileStream.WriteLine($"[{DateTime.Now}] CON-PIPE: {line}");
              pipeWriter.WriteLine(line);
              pipeWriter.Flush();
            }
          }
          catch (Exception ex)
          {
            logFileStream.WriteLine($"[{DateTime.Now}] CON-PIPE: exception {ex}");
            Console.Error.WriteLine($"Error reading from stdin: {ex.Message}");
          }
        });

        await Task.WhenAny(pipeToStdoutTask, stdinToPipeTask);
        logFileStream.WriteLine($"[{DateTime.Now}] PROGRAM CLOSED");
      }
      catch (Exception ex)
      {
        Console.Error.WriteLine($"Error in MCPServerProxy: {ex.Message}");
        await logFileStream.WriteLineAsync($"[{DateTime.Now}] Error: {ex.Message}");
      }
      finally
      {
        await logFileStream.FlushAsync();
      }
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

      processIds.Sort();
      return processIds;
    }
  }
}
