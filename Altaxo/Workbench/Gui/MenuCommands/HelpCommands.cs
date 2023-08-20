#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2018 Dr. Dirk Lellinger
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

using Altaxo.Gui.Workbench;

namespace Altaxo.Gui.MenuCommands
{


  public class LoadAeroTheme : SimpleCommand
  {
    public override void Execute(object? parameter)
    {
      var wb = Current.GetRequiredService<AltaxoWorkbench>();
      wb.DockManagerTheme = "aero";
    }
  }

  public class LoadExpressionDarkTheme : SimpleCommand
  {
    public override void Execute(object? parameter)
    {
      var wb = Current.GetRequiredService<AltaxoWorkbench>();
      wb.DockManagerTheme = "expressiondark";
    }
  }

  public class LoadExpressionLightTheme : SimpleCommand
  {
    public override void Execute(object? parameter)
    {
      var wb = Current.GetRequiredService<AltaxoWorkbench>();
      wb.DockManagerTheme = "expressionlight";
    }
  }

  public class LoadMetroTheme : SimpleCommand
  {
    public override void Execute(object? parameter)
    {
      var wb = Current.GetRequiredService<AltaxoWorkbench>();
      wb.DockManagerTheme = "metro";
    }
  }

  public class LoadVS2010Theme : SimpleCommand
  {
    public override void Execute(object? parameter)
    {
      var wb = Current.GetRequiredService<AltaxoWorkbench>();
      wb.DockManagerTheme = "vs2010";
    }
  }

  public class LoadVS2013BlueTheme : SimpleCommand
  {
    public override void Execute(object? parameter)
    {
      var wb = Current.GetRequiredService<AltaxoWorkbench>();
      wb.DockManagerTheme = "vs2013blue";
    }
  }

  public class LoadVS2013DarkTheme : SimpleCommand
  {
    public override void Execute(object? parameter)
    {
      var wb = Current.GetRequiredService<AltaxoWorkbench>();
      wb.DockManagerTheme = "vs2013dark";
    }
  }

  public class LoadVS2013LightTheme : SimpleCommand
  {
    public override void Execute(object? parameter)
    {
      var wb = Current.GetRequiredService<AltaxoWorkbench>();
      wb.DockManagerTheme = "vs2013light";
    }
  }
}
