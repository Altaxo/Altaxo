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
using System.IO;
using System.IO.Pipelines;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.Extensions.Logging.Abstractions;
using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;
using Xunit;

namespace Altaxo.Chat
{
  public class McpToolRegistryTests
  {
    [Fact]
    public void RegistryAcceptsNamespacedTools()
    {
      var provider = new TestToolProvider("tests", "tests.echo");
      var registry = new McpToolRegistry(new[] { provider }, EmptyServiceProvider.Instance);

      var tool = Assert.Single(registry.Tools);
      Assert.Equal("tests.echo", tool.ProtocolTool.Name);
    }

    [Fact]
    public void RegistryRejectsDuplicateProviderIds()
    {
      var providers = new[]
      {
        new TestToolProvider("tests", "tests.one"),
        new TestToolProvider("tests", "tests.two")
      };

      Assert.Throws<ArgumentException>(() => new McpToolRegistry(providers, EmptyServiceProvider.Instance));
    }

    [Fact]
    public void RegistryRejectsDuplicateToolNames()
    {
      var provider = new TestToolProvider("tests", "tests.echo", "tests.echo");

      Assert.Throws<ArgumentException>(() => new McpToolRegistry(new[] { provider }, EmptyServiceProvider.Instance));
    }

    [Fact]
    public void RegistryRequiresProviderNamespacedToolNames()
    {
      var provider = new TestToolProvider("tests", "other.echo");

      Assert.Throws<ArgumentException>(() => new McpToolRegistry(new[] { provider }, EmptyServiceProvider.Instance));
    }

    [Fact]
    public void AddinManifestDefinesMcpPipeName()
    {
      var solutionDirectory = FindSolutionDirectory();
      var manifestPath = Path.Combine(solutionDirectory, "Altaxo", "Dom.Presentation", "AltaxoCore.addin");
      var manifest = XDocument.Load(manifestPath);
      var pipeNamePath = manifest.Root!
        .Elements("Path")
        .Single(element => (string?)element.Attribute("name") == "/Altaxo/Chat/Configuration/PipeName");

      Assert.Equal("Altaxo.Chat", (string?)pipeNamePath.Element("String")?.Attribute("text"));
    }

    [Fact]
    public async Task StreamClientCanInitializeListAndInvokeTool()
    {
      var clientToServer = new Pipe();
      var serverToClient = new Pipe();
      var provider = new TestToolProvider("tests", "tests.echo");
      var registry = new McpToolRegistry(new[] { provider }, EmptyServiceProvider.Instance);
      var serverOptions = new McpServerOptions
      {
        ServerInfo = new Implementation { Name = "Altaxo test", Version = "1.0" },
        ToolCollection = new McpServerPrimitiveCollection<McpServerTool>()
      };
      foreach (var tool in registry.Tools)
        serverOptions.ToolCollection.Add(tool);

      var serverTransport = new StreamServerTransport(
        clientToServer.Reader.AsStream(),
        serverToClient.Writer.AsStream(),
        "Altaxo test",
        NullLoggerFactory.Instance);
      await using var server = McpServer.Create(serverTransport, serverOptions, NullLoggerFactory.Instance, EmptyServiceProvider.Instance);
      using var cancellation = new CancellationTokenSource();
      var serverTask = server.RunAsync(cancellation.Token);

      var clientTransport = new StreamClientTransport(
        clientToServer.Writer.AsStream(),
        serverToClient.Reader.AsStream(),
        NullLoggerFactory.Instance);
      try
      {
        await using var client = await McpClient.CreateAsync(clientTransport, null, NullLoggerFactory.Instance, cancellation.Token);
        var tools = await client.ListToolsAsync(cancellationToken: cancellation.Token);
        Assert.Contains(tools, tool => tool.Name == "tests.echo");

        var result = await client.CallToolAsync(
          "tests.echo",
          new Dictionary<string, object?> { ["text"] = "hello from Mcp" },
          cancellationToken: cancellation.Token);

        Assert.Contains("hello from Mcp", JsonSerializer.Serialize(result));
      }
      finally
      {
        cancellation.Cancel();
        clientToServer.Writer.Complete();
        clientToServer.Reader.Complete();
        serverToClient.Writer.Complete();
        serverToClient.Reader.Complete();
        try
        {
          await serverTask;
        }
        catch (OperationCanceledException)
        {
        }
      }
    }

    private static string FindSolutionDirectory()
    {
      var directory = new DirectoryInfo(AppContext.BaseDirectory);
      while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "Altaxo.slnx")))
        directory = directory.Parent;

      return directory?.FullName ?? throw new DirectoryNotFoundException("Could not locate Altaxo.slnx.");
    }

    private sealed class TestToolProvider : IMcpToolProvider
    {
      private readonly IReadOnlyList<McpServerTool> _tools;

      public TestToolProvider(string id, params string[] toolNames)
      {
        Id = id;
        _tools = toolNames.Select(name => McpServerTool.Create(
          typeof(TestToolProvider).GetMethod(nameof(Echo), BindingFlags.Instance | BindingFlags.NonPublic)!,
          _ => this,
          new McpServerToolCreateOptions { Name = name, Description = "Echoes the supplied text." })).ToArray();
      }

      public string Id { get; }

      public IReadOnlyList<McpServerTool> CreateTools(IServiceProvider services) => _tools;

      private string Echo(string text) => text;
    }

    private sealed class EmptyServiceProvider : IServiceProvider
    {
      public static EmptyServiceProvider Instance { get; } = new EmptyServiceProvider();

      public object? GetService(Type serviceType) => null;
    }
  }
}
