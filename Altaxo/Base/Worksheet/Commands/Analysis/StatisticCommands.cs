#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
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
//    along with ctrl program; if not, write to the Free Software
//    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
//
/////////////////////////////////////////////////////////////////////////////

#endregion Copyright

using Altaxo.Data;
using Altaxo.Gui.Worksheet.Viewing;
using System;

namespace Altaxo.Worksheet.Commands.Analysis
{
	/// <summary>
	/// Contain statistic commands.
	/// </summary>
	public class StatisticCommands
	{
		#region Statistical commands

		public static void StatisticsOnColumns(IWorksheetController ctrl)
		{
			var table = ctrl.DataTable.DoStatisticsOnColumns(ctrl.SelectedDataColumns, ctrl.SelectedDataRows);

			Current.Project.DataTableCollection.Add(table);
			// create a new worksheet without any columns
			Current.ProjectService.CreateNewWorksheet(table);
		}

		public static void StatisticsOnRows(IWorksheetController ctrl)
		{
			var table = ctrl.DataTable.DoStatisticsOnRows(ctrl.SelectedDataColumns, ctrl.SelectedDataRows);

			Current.Project.DataTableCollection.Add(table);
			// create a new worksheet without any columns
			Current.ProjectService.CreateNewWorksheet(table);
		}

		#endregion Statistical commands

		public static void CreateHistogram(WorksheetController ctrl)
		{
			var table = Altaxo.Analysis.Statistics.Histograms.HistogramCreation.CreateHistogramOnColumns(ctrl.DataTable, ctrl.SelectedDataColumns, ctrl.SelectedDataRows, Gui.UserInteractionLevel.InteractAlways);

			if (null != table && !Current.Project.DataTableCollection.Contains(table))
			{
				Current.Project.DataTableCollection.Add(table);

				// create a new worksheet without any columns
				Current.ProjectService.CreateNewWorksheet(table);
			}
		}
	}
}