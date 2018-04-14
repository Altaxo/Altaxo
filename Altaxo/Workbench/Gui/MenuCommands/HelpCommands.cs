using Altaxo.Gui.Workbench;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Gui.MenuCommands
{
  public class DevelopmentTest : SimpleCommand
  {
	public override void Execute(object parameter)
	{
	  Run6();
	}

	public void Run6()
	{
	  GC.Collect();
	  System.Threading.Thread.Sleep(1000);
	  var comMananger = Altaxo.Current.ComManager;
	}
  }

  public class LoadVS2010Theme : SimpleCommand
  {
	public override void Execute(object parameter)
	{
	  var wb = Current.GetRequiredService<AltaxoWorkbench>();
	  wb.DockManagerTheme = "vs2010";
	}
  }

  public class LoadAeroTheme : SimpleCommand
  {
	public override void Execute(object parameter)
	{
	  var wb = Current.GetRequiredService<AltaxoWorkbench>();
	  wb.DockManagerTheme = "aero";
	}
  }

  public class LoadMetroTheme : SimpleCommand
  {
	public override void Execute(object parameter)
	{
	  var wb = Current.GetRequiredService<AltaxoWorkbench>();
	  wb.DockManagerTheme = "metro";
	}
  }

  public class LoadVS2013DarkTheme : SimpleCommand
  {
	public override void Execute(object parameter)
	{
	  var wb = Current.GetRequiredService<AltaxoWorkbench>();
	  wb.DockManagerTheme = "vs2013dark";
	}
  }

  public class LoadVS2013LightTheme : SimpleCommand
  {
	public override void Execute(object parameter)
	{
	  var wb = Current.GetRequiredService<AltaxoWorkbench>();
	  wb.DockManagerTheme = "vs2013light";
	}
  }

  public class LoadVS2013BlueTheme : SimpleCommand
  {
	public override void Execute(object parameter)
	{
	  var wb = Current.GetRequiredService<AltaxoWorkbench>();
	  wb.DockManagerTheme = "vs2013blue";
	}
  }
}
