using System;
using System.Threading.Tasks;
using Altaxo.Chat;

namespace Altaxo
{
  internal class Program
  {
    private static async Task Main(string[] args)
    {
      string? logfileName = null;
      for (int i = 0; i < args.Length; i++)
      {
        var larg = args[i].ToLowerInvariant();

        if (larg == "--attachdebugger")
        {
          System.Diagnostics.Debugger.Launch();
          break;
        }
        else if (larg == "--logfile")
        {
          if (i + 1 < args.Length)
          {
            logfileName = args[i + 1];
            i++;
          }
          else
          {
            Console.Error.WriteLine("Missing logfile name after --logfile argument.");
            // set the return code to 1 to indicate an error
            Environment.ExitCode = 1;
            return;
          }
          break;
        }
      }


      var server = new MCPServerProxy
      {
        PipeName = args.Length > 0 ? args[0] : MCPServerProxy.DefaultPipeName,
        LogfileName = logfileName
      };
      await server.RunAsync();
    }
  }
}
