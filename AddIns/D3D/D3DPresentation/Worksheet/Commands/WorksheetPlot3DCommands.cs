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

using Altaxo.Data;
using Altaxo.Gui.Scripting;
using Altaxo.Scripting;
using ICSharpCode.Core;
using System;

namespace Altaxo.Worksheet.Commands
{
	#region Plot 3D commands

	public class Plot3DLine : AbstractWorksheetControllerCommand
	{
		public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
		{
			PlotCommands3D.PlotLine(ctrl, true, false);
		}
	}

	public class Plot3DLineStack : AbstractWorksheetControllerCommand
	{
		public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
		{
			PlotCommands3D.PlotLineStack(ctrl);
		}
	}

	public class Plot3DLineRelativeStack : AbstractWorksheetControllerCommand
	{
		public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
		{
			PlotCommands3D.PlotLineRelativeStack(ctrl);
		}
	}

	public class Plot3DScatter : AbstractWorksheetControllerCommand
	{
		public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
		{
			PlotCommands3D.PlotLine(ctrl, false, true);
		}
	}

	public class Plot3DLineAndScatter : AbstractWorksheetControllerCommand
	{
		public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
		{
			PlotCommands3D.PlotLine(ctrl, true, true);
		}
	}

	public class Plot3DBarChartNormal : AbstractWorksheetControllerCommand
	{
		public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
		{
			PlotCommands3D.PlotBarChartNormal(ctrl);
		}
	}

	public class Plot3DBarChartStack : AbstractWorksheetControllerCommand
	{
		public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
		{
			PlotCommands3D.PlotBarChartStack(ctrl);
		}
	}

	public class Plot3DBarChartRelativeStack : AbstractWorksheetControllerCommand
	{
		public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
		{
			PlotCommands3D.PlotBarChartRelativeStack(ctrl);
		}
	}

	public class Plot3DColumnChartNormal : AbstractWorksheetControllerCommand
	{
		public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
		{
			PlotCommands3D.PlotColumnChartNormal(ctrl);
		}
	}

	public class Plot3DColumnChartStack : AbstractWorksheetControllerCommand
	{
		public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
		{
			PlotCommands3D.PlotColumnChartStack(ctrl);
		}
	}

	public class Plot3DColumnChartRelativeStack : AbstractWorksheetControllerCommand
	{
		public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
		{
			PlotCommands3D.PlotColumnChartRelativeStack(ctrl);
		}
	}

	#endregion Plot 3D commands
}