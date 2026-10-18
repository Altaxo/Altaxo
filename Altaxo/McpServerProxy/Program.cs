using System.Threading.Tasks;
using Altaxo.Chat;

namespace Altaxo
{
  internal class Program
  {
    private static async Task Main(string[] args)
    {
      bool debug = false;
      foreach (var arg in args)
      {
        var larg = arg.ToLowerInvariant();

        if (larg == "--attachdebugger")
        {
          System.Diagnostics.Debugger.Launch();
          break;
        }
        else if(larg == "--debug")
        {
          debug = true;
          break;
        }
      }


      var server = new MCPServerProxy
      {
        PipeName = args.Length > 0 ? args[0] : MCPServerProxy.DefaultPipeName,
        Debug = debug
      };
      await server.RunAsync();
    }
  }
}
