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
  /// <summary>
  /// Loads the Aero docking theme.
  /// </summary>
  public class LoadAeroTheme : SimpleCommand
  {
    /// <inheritdoc/>
    public override void Execute(object? parameter)
    {
      var wb = Current.GetRequiredService<AltaxoWorkbench>();
      wb.DockManagerTheme = "aero";
    }
  }

  /// <summary>
  /// Loads the Expression Dark docking theme.
  /// </summary>
  public class LoadExpressionDarkTheme : SimpleCommand
  {
    /// <inheritdoc/>
    public override void Execute(object? parameter)
    {
      var wb = Current.GetRequiredService<AltaxoWorkbench>();
      wb.DockManagerTheme = "expressiondark";
    }
  }

  /// <summary>
  /// Loads the Expression Light docking theme.
  /// </summary>
  public class LoadExpressionLightTheme : SimpleCommand
  {
    /// <inheritdoc/>
    public override void Execute(object? parameter)
    {
      var wb = Current.GetRequiredService<AltaxoWorkbench>();
      wb.DockManagerTheme = "expressionlight";
    }
  }

  /// <summary>
  /// Loads the Metro docking theme.
  /// </summary>
  public class LoadMetroTheme : SimpleCommand
  {
    /// <inheritdoc/>
    public override void Execute(object? parameter)
    {
      var wb = Current.GetRequiredService<AltaxoWorkbench>();
      wb.DockManagerTheme = "metro";
    }
  }

  /// <summary>
  /// Loads the Visual Studio 2010 docking theme.
  /// </summary>
  public class LoadVS2010Theme : SimpleCommand
  {
    /// <inheritdoc/>
    public override void Execute(object? parameter)
    {
      var wb = Current.GetRequiredService<AltaxoWorkbench>();
      wb.DockManagerTheme = "vs2010";
    }
  }

  /// <summary>
  /// Loads the Visual Studio 2013 Blue docking theme.
  /// </summary>
  public class LoadVS2013BlueTheme : SimpleCommand
  {
    /// <inheritdoc/>
    public override void Execute(object? parameter)
    {
      var wb = Current.GetRequiredService<AltaxoWorkbench>();
      wb.DockManagerTheme = "vs2013blue";
    }
  }

  /// <summary>
  /// Loads the Visual Studio 2013 Dark docking theme.
  /// </summary>
  public class LoadVS2013DarkTheme : SimpleCommand
  {
    /// <inheritdoc/>
    public override void Execute(object? parameter)
    {
      var wb = Current.GetRequiredService<AltaxoWorkbench>();
      wb.DockManagerTheme = "vs2013dark";
    }
  }

  /// <summary>
  /// Loads the Visual Studio 2013 Light docking theme.
  /// </summary>
  public class LoadVS2013LightTheme : SimpleCommand
  {
    /// <inheritdoc/>
    public override void Execute(object? parameter)
    {
      var wb = Current.GetRequiredService<AltaxoWorkbench>();
      wb.DockManagerTheme = "vs2013light";
    }
  }
}
