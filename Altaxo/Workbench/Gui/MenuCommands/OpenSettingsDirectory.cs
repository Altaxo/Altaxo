using Altaxo.Main.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Gui.MenuCommands
{
  public class OpenSettingsDirectory : SimpleCommand
  {
    public override void Execute(object parameter)
    {
      var appName = StringParser.Parse("${AppName}");

      string dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), appName);

      string args = "/e," + dir;

      var processInfo = new System.Diagnostics.ProcessStartInfo("explorer.exe", args)
      {
        CreateNoWindow = false,
        WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal
      };

      try
      {
        var proc = System.Diagnostics.Process.Start(processInfo);
      }
      catch (Exception)
      {
      }
    }
  }
}
