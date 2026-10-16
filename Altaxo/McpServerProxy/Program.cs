using System;
using System.Threading.Tasks;
using Altaxo.Chat;

namespace Altaxo
{
  internal class Program
  {
    private static async Task Main(string[] args)
    {
      // Debugger.Launch();

      var server = new MCPServerProxy
      {
        PipeName = args.Length > 0 ? args[0] : MCPServerProxy.DefaultPipeName
      };
      await server.RunAsync();
    }
  }
}
