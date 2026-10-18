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

    public string TimeTag => $"{DateTime.Now:yyyy-MM-dd-HH-mm-ss}";

    public bool Debug { get; init; }

    public async Task RunAsync()
    {
      StreamWriter? logFileStream = null;
      if (Debug)
      {
        var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        logFileStream = new StreamWriter(Path.Combine(dir, $"MCPServerProxy_{TimeTag}.log"), true, System.Text.Encoding.UTF8) { AutoFlush = true };
        logFileStream.WriteLine($"{TimeTag}\tMCP proxy started.");
      }

      try
      {
        var processIds = GetAltaxoProcessIds();
        if (processIds.Count == 0)
        {
          if (Debug)
            logFileStream?.WriteLine($"{TimeTag}\tNo running Altaxo instance was found!");
          throw new InvalidOperationException("No running Altaxo instance was found.");
        }
        else
        {
          if (Debug)
            logFileStream?.WriteLine($"{TimeTag}\t{processIds.Count} running Altaxo instance(s) found!");
        }

        var targetPipeName = $"{PipeName}.{processIds[0]}";
        NamedPipeClientStream? pipe = null;

        for (int i = 0; i < 30; ++i)
        {
          var candidate = new NamedPipeClientStream(".", targetPipeName, PipeDirection.InOut, PipeOptions.Asynchronous);
          try
          {
            if (Debug)
              logFileStream?.WriteLine($"{TimeTag}\tTry to connect to pipe {targetPipeName}!");
            await candidate.ConnectAsync(1000);
            pipe = candidate;
            break;
          }
          catch (Exception ex)
          {
            candidate.Dispose();
            if (Debug)
              logFileStream?.WriteLine($"{TimeTag}\tError connecting to named pipe '{targetPipeName}': {ex.Message}");
            Console.Error.WriteLine($"Error connecting to named pipe '{targetPipeName}': {ex.Message}");
          }
        }

        if (pipe is null)
        {
          if (Debug)
            logFileStream?.WriteLine($"{TimeTag}]\tCould not connect to named pipe '{targetPipeName}' after 30 attempts.");
          throw new IOException($"Could not connect to named pipe '{targetPipeName}' after 30 attempts.");
        }

        using var connectedPipe = pipe;
        using var pipeReader = new StreamReader(connectedPipe);
        using var pipeWriter = new StreamWriter(connectedPipe) { AutoFlush = true };
        using var conreader = new StreamReader(Console.OpenStandardInput());
        using var conwriter = new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true };

        if (Debug)
          logFileStream?.WriteLine($"{TimeTag}\tPROGRAM OPENED; connected to named pipe '{targetPipeName}'");

        var pipeToStdoutTask = Task.Run(async () =>
        {
          try
          {
            while (await pipeReader.ReadLineAsync() is { } line)
            {
              if (Debug)
                logFileStream?.WriteLine($"{TimeTag}\tPIPE-CON: {line}");
              conwriter.WriteLine(line);
              conwriter.Flush();
            }
          }
          catch (Exception ex)
          {
            if (Debug)
              logFileStream?.WriteLine($"{TimeTag}\tPIPE-CON: exception {ex}");
            Console.Error.WriteLine($"Error reading from named pipe: {ex.Message}");
          }
        });

        var stdinToPipeTask = Task.Run(async () =>
        {
          try
          {
            while (await conreader.ReadLineAsync() is { } line)
            {
              if (Debug)
                logFileStream?.WriteLine($"[{DateTime.Now}] CON-PIPE: {line}");
              pipeWriter.WriteLine(line);
              pipeWriter.Flush();
            }
          }
          catch (Exception ex)
          {
            if (Debug)
              logFileStream?.WriteLine($"{TimeTag}\tCON-PIPE: exception {ex}");
            Console.Error.WriteLine($"Error reading from stdin: {ex.Message}");
          }
        });

        await Task.WhenAny(pipeToStdoutTask, stdinToPipeTask);
        if (Debug)
          logFileStream?.WriteLine($"{TimeTag}\tPROGRAM CLOSED");
      }
      catch (Exception ex)
      {
        Console.Error.WriteLine($"Error in MCPServerProxy: {ex.Message}");
        if (logFileStream is not null)
          await logFileStream.WriteLineAsync($"{TimeTag}\tError: {ex.Message}");
      }
      finally
      {
        if (logFileStream is not null)
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
