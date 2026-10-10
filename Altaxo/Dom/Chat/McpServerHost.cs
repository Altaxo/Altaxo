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
using System.Collections.Concurrent;
using System.ComponentModel.Design;
using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;
using Altaxo.AddInItems;
using Altaxo.Main.Services;
using Microsoft.Extensions.Logging.Abstractions;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;

namespace Altaxo.Chat
{
  /// <summary>
  /// Accepts Mcp clients over a current-user-only named pipe.
  /// </summary>
  public sealed class McpServerHost : IMcpServerHost
  {
    /// <summary>
    /// Gets the named pipe name configured in the Altaxo add-in tree.
    /// </summary>
    public static string ConfiguredPipeName
    {
      get
      {
        var configuredNames = AddInTree.BuildItems<string>("/Altaxo/Chat/Configuration/PipeName", null, false);
        if (configuredNames.Count != 1 || string.IsNullOrWhiteSpace(configuredNames[0]))
          throw new InvalidOperationException("AltaxoCore.addin must define exactly one Mcp pipe name at /Altaxo/Chat/Configuration/PipeName.");

        var configuredName = configuredNames[0].Trim();
        if (configuredName.Contains('\\') || configuredName.Contains('/'))
          throw new InvalidOperationException("The Mcp pipe name in AltaxoCore.addin must not contain path separators.");

        return configuredName;
      }
    }

    private readonly object _gate = new();
    private readonly ConcurrentDictionary<long, Task> _connections = new();
    private CancellationTokenSource? _cancellationTokenSource;
    private Task? _acceptTask;
    private McpToolRegistry? _toolRegistry;
    private long _nextConnectionId;
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="McpServerHost"/> class.
    /// </summary>
    public McpServerHost()
    {
    }

    /// <inheritdoc/>
    public string PipeName => ConfiguredPipeName;

    /// <inheritdoc/>
    public void Start()
    {
      lock (_gate)
      {
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (_acceptTask is not null)
          return;

        var services = Altaxo.Current.GetRequiredService<IServiceContainer>();
        var providers = AddInTree.BuildItems<IMcpToolProvider>("/Altaxo/Chat/Tools", services, false);
        _toolRegistry = new McpToolRegistry(providers, services);

        var shutdownToken = Altaxo.Current.GetRequiredService<IShutdownService>().ShutdownToken;
        _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(shutdownToken);
        _acceptTask = Task.Run(() => AcceptConnectionsAsync(_cancellationTokenSource.Token));
      }
    }

    private async Task AcceptConnectionsAsync(CancellationToken cancellationToken)
    {
      while (!cancellationToken.IsCancellationRequested)
      {
        var pipe = new NamedPipeServerStream(
          PipeName,
          PipeDirection.InOut,
          NamedPipeServerStream.MaxAllowedServerInstances,
          PipeTransmissionMode.Byte,
          PipeOptions.Asynchronous | PipeOptions.CurrentUserOnly);

        try
        {
          await pipe.WaitForConnectionAsync(cancellationToken).ConfigureAwait(false);
        }
        catch
        {
          pipe.Dispose();
          throw;
        }

        var id = Interlocked.Increment(ref _nextConnectionId);
        var task = HandleConnectionAsync(pipe, cancellationToken);
        _connections[id] = task;
        _ = task.ContinueWith(
          _ => _connections.TryRemove(id, out _),
          CancellationToken.None,
          TaskContinuationOptions.ExecuteSynchronously,
          TaskScheduler.Default);
      }
    }

    private async Task HandleConnectionAsync(NamedPipeServerStream pipe, CancellationToken cancellationToken)
    {
      using (pipe)
      {
        try
        {
          var serverOptions = new McpServerOptions
          {
            ServerInfo = new Implementation
            {
              Name = "Altaxo",
              Version = typeof(McpServerHost).Assembly.GetName().Version?.ToString() ?? "0.0.0"
            },
            ToolCollection = new McpServerPrimitiveCollection<McpServerTool>()
          };

          foreach (var tool in _toolRegistry!.Tools)
            serverOptions.ToolCollection.Add(tool);

          await using var transport = new StreamServerTransport(pipe, pipe, "Altaxo", NullLoggerFactory.Instance);
          var services = Altaxo.Current.GetRequiredService<IServiceContainer>();
          await using var server = McpServer.Create(transport, serverOptions, NullLoggerFactory.Instance, services);
          await server.RunAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
        }
        catch (Exception ex)
        {
          Altaxo.Current.Log.Error(ex);
        }
      }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
      Task? acceptTask;
      CancellationTokenSource? cancellationTokenSource;
      lock (_gate)
      {
        if (_disposed)
          return;

        _disposed = true;
        cancellationTokenSource = _cancellationTokenSource;
        acceptTask = _acceptTask;
      }

      cancellationTokenSource?.Cancel();
      try
      {
        acceptTask?.GetAwaiter().GetResult();
      }
      catch (OperationCanceledException)
      {
      }
      catch (ObjectDisposedException)
      {
      }

      try
      {
        Task.WhenAll(_connections.Values).GetAwaiter().GetResult();
      }
      catch (OperationCanceledException)
      {
      }

      cancellationTokenSource?.Dispose();
    }
  }
}
