#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2016 Dr. Dirk Lellinger
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
using Altaxo.Data;
using Altaxo.Gui.Scripting;
using Altaxo.Scripting;

namespace Altaxo.Worksheet.Commands
{
  #region Plot 3D commands

  /// <summary>
  /// Plots selected worksheet data as 3D line plot.
  /// </summary>
  public class Plot3DLine : AbstractWorksheetControllerCommand
  {
    /// <summary>
    /// Executes the command.
    /// </summary>
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      PlotCommands3D.PlotLine(ctrl, true, false);
    }
  }

  /// <summary>
  /// Plots selected worksheet data as stacked 3D lines.
  /// </summary>
  public class Plot3DLineStack : AbstractWorksheetControllerCommand
  {
    /// <summary>
    /// Executes the command.
    /// </summary>
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      PlotCommands3D.PlotLineStack(ctrl);
    }
  }

  /// <summary>
  /// Plots selected worksheet data as relative stacked 3D lines.
  /// </summary>
  public class Plot3DLineRelativeStack : AbstractWorksheetControllerCommand
  {
    /// <summary>
    /// Executes the command.
    /// </summary>
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      PlotCommands3D.PlotLineRelativeStack(ctrl);
    }
  }

  /// <summary>
  /// Plots selected worksheet data as 3D scatter plot.
  /// </summary>
  public class Plot3DScatter : AbstractWorksheetControllerCommand
  {
    /// <summary>
    /// Executes the command.
    /// </summary>
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      PlotCommands3D.PlotLine(ctrl, false, true);
    }
  }

  /// <summary>
  /// Plots selected worksheet data as combined 3D line and scatter plot.
  /// </summary>
  public class Plot3DLineAndScatter : AbstractWorksheetControllerCommand
  {
    /// <summary>
    /// Executes the command.
    /// </summary>
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      PlotCommands3D.PlotLine(ctrl, true, true);
    }
  }

  /// <summary>
  /// Plots selected worksheet data as normal 3D bar chart.
  /// </summary>
  public class Plot3DBarChartNormal : AbstractWorksheetControllerCommand
  {
    /// <summary>
    /// Executes the command.
    /// </summary>
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      PlotCommands3D.PlotBarChartNormal(ctrl);
    }
  }

  /// <summary>
  /// Plots selected worksheet data as stacked 3D bar chart.
  /// </summary>
  public class Plot3DBarChartStack : AbstractWorksheetControllerCommand
  {
    /// <summary>
    /// Executes the command.
    /// </summary>
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      PlotCommands3D.PlotBarChartStack(ctrl);
    }
  }

  /// <summary>
  /// Plots selected worksheet data as relative stacked 3D bar chart.
  /// </summary>
  public class Plot3DBarChartRelativeStack : AbstractWorksheetControllerCommand
  {
    /// <summary>
    /// Executes the command.
    /// </summary>
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      PlotCommands3D.PlotBarChartRelativeStack(ctrl);
    }
  }

  /// <summary>
  /// Plots selected worksheet data as normal 3D column chart.
  /// </summary>
  public class Plot3DColumnChartNormal : AbstractWorksheetControllerCommand
  {
    /// <summary>
    /// Executes the command.
    /// </summary>
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      PlotCommands3D.PlotColumnChartNormal(ctrl);
    }
  }

  /// <summary>
  /// Plots selected worksheet data as stacked 3D column chart.
  /// </summary>
  public class Plot3DColumnChartStack : AbstractWorksheetControllerCommand
  {
    /// <summary>
    /// Executes the command.
    /// </summary>
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      PlotCommands3D.PlotColumnChartStack(ctrl);
    }
  }

  /// <summary>
  /// Plots selected worksheet data as relative stacked 3D column chart.
  /// </summary>
  public class Plot3DColumnChartRelativeStack : AbstractWorksheetControllerCommand
  {
    /// <summary>
    /// Executes the command.
    /// </summary>
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      PlotCommands3D.PlotColumnChartRelativeStack(ctrl);
    }
  }

  #endregion Plot 3D commands
}
