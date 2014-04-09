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
using Origin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Addins.OriginConnector
{
	public static class WorksheetActions
	{
		public static readonly string[] LongNamePropertyColumnNames = new string[] { "LongName", "Long Name", "Langname" };

		public static readonly string[] UnitPropertyColumnNames = new string[] { "Unit", "Units", "Einheit", "Einheiten" };

		public static readonly string[] CommentPropertyColumnNames = new string[] { "Comment", "Comments", "Kommentar", "Kommentare" };

		#region Conversion of Altaxo types to/from Origin Types

		public static Altaxo.Data.ColumnKind OriginToAltaxoColumnKind(Origin.COLTYPES originType)
		{
			Altaxo.Data.ColumnKind altaxoType;

			switch (originType)
			{
				case COLTYPES.COLTYPE_ERROR:
					altaxoType = ColumnKind.Err;
					break;

				case COLTYPES.COLTYPE_GROUP:
					altaxoType = ColumnKind.V;
					break;

				case COLTYPES.COLTYPE_LABEL:
					altaxoType = ColumnKind.Label;
					break;

				case COLTYPES.COLTYPE_NONE:
					altaxoType = ColumnKind.V;
					break;

				case COLTYPES.COLTYPE_NO_CHANGE:
					altaxoType = ColumnKind.V;
					break;

				case COLTYPES.COLTYPE_SUBJECT:
					altaxoType = ColumnKind.V;
					break;

				case COLTYPES.COLTYPE_X:
					altaxoType = ColumnKind.X;
					break;

				case COLTYPES.COLTYPE_X_ERROR:
					altaxoType = ColumnKind.Err;
					break;

				case COLTYPES.COLTYPE_Y:
					altaxoType = ColumnKind.V;
					break;

				case COLTYPES.COLTYPE_Z:
					altaxoType = ColumnKind.Y;
					break;

				default:
					altaxoType = ColumnKind.V;
					break;
			}

			return altaxoType;
		}

		public static Origin.COLTYPES AltaxoToOriginColumnType(Altaxo.Data.ColumnKind altaxoType)
		{
			COLTYPES originType;

			switch (altaxoType)
			{
				case ColumnKind.V:
					originType = COLTYPES.COLTYPE_Y;
					break;

				case ColumnKind.X:
					originType = COLTYPES.COLTYPE_X;
					break;

				case ColumnKind.Y:
					originType = COLTYPES.COLTYPE_Z;
					break;

				case ColumnKind.Z:
					originType = COLTYPES.COLTYPE_Z;
					break;

				case ColumnKind.Err:
					originType = COLTYPES.COLTYPE_ERROR;
					break;

				case ColumnKind.pErr:
					originType = COLTYPES.COLTYPE_ERROR;
					break;

				case ColumnKind.mErr:
					originType = COLTYPES.COLTYPE_ERROR;
					break;

				case ColumnKind.Label:
					originType = COLTYPES.COLTYPE_LABEL;
					break;

				case ColumnKind.Condition:
					originType = COLTYPES.COLTYPE_NONE;
					break;

				default:
					originType = COLTYPES.COLTYPE_NO_CHANGE;
					break;
			}

			return originType;
		}

		public static DataColumn GetAltaxoColumnFromOriginDataFormat(COLDATAFORMAT originColDataFormat)
		{
			// create a column
			Altaxo.Data.DataColumn destCol = null;

			switch (originColDataFormat)
			{
				case COLDATAFORMAT.DF_BYTE:
					destCol = new Altaxo.Data.DoubleColumn();
					break;

				case COLDATAFORMAT.DF_CHAR:
					destCol = new Altaxo.Data.DoubleColumn();
					break;

				case COLDATAFORMAT.DF_COMPLEX:
					destCol = new Altaxo.Data.DoubleColumn();
					break;

				case COLDATAFORMAT.DF_DATE:
					destCol = new Altaxo.Data.DateTimeColumn();
					break;

				case COLDATAFORMAT.DF_DOUBLE:
					destCol = new Altaxo.Data.DoubleColumn();
					break;

				case COLDATAFORMAT.DF_FLOAT:
					destCol = new Altaxo.Data.DoubleColumn();
					break;

				case COLDATAFORMAT.DF_LONG:
					destCol = new Altaxo.Data.DoubleColumn();
					break;

				case COLDATAFORMAT.DF_SHORT:
					destCol = new Altaxo.Data.DoubleColumn();
					break;

				case COLDATAFORMAT.DF_TEXT:
					destCol = new Altaxo.Data.TextColumn();
					break;

				case COLDATAFORMAT.DF_TEXT_NUMERIC:
					destCol = new Altaxo.Data.TextColumn();
					break;

				case COLDATAFORMAT.DF_TIME:
					destCol = new Altaxo.Data.DateTimeColumn();
					break;

				case COLDATAFORMAT.DF_ULONG:
					destCol = new Altaxo.Data.DoubleColumn();
					break;

				case COLDATAFORMAT.DF_USHORT:
					destCol = new Altaxo.Data.DoubleColumn();
					break;

				default:
					destCol = new Altaxo.Data.TextColumn();
					break;
			}
			return destCol;
		}

		#endregion Conversion of Altaxo types to/from Origin Types

		#region Helper functions

		/// <summary>
		/// Determines whether the <paramref name="name"/> in contained in any of the <paramref name="names"/> (comparison is case insensitive).
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="names">The names to test against.</param>
		/// <returns><c>true</c> if <paramref name="name"/> in contained in any of the <paramref name="names"/></returns>
		private static bool IsLikeAnyOf(string name, string[] names)
		{
			foreach (var candidate in names)
				if (0 == string.Compare(name, candidate, true))
					return true;

			return false;
		}

		private static void ShowErrorMessage(string strMsg)
		{
			Altaxo.Current.Gui.ErrorMessageBox(strMsg);
		}

		#endregion Helper functions

		#region Send worksheet data to origin

		public static void PutTable(this OriginConnection conn, Altaxo.Data.DataTable srcTable, string originWorksheetName, bool appendRows)
		{
			if (IsColumnReorderingNeccessaryForPuttingTableToOrigin(srcTable))
			{
				srcTable = (DataTable)srcTable.Clone();
				ReorderColumnsInTableForCompatibilityWithOrigin(srcTable);
			}

			var stb = new System.Text.StringBuilder();
			string strWksName = originWorksheetName;

			strWksName.Trim();
			// Validate worksheet name:
			if (0 == strWksName.Length)
			{
				ShowErrorMessage("Please specify a worksheet name first.");
				return;
			}

			int nColumns = srcTable.DataColumnCount;
			int nRows = srcTable.DataRowCount;
			// Validate the number of columns and the number of rows:
			if (nColumns <= 0 || nRows <= 0)
			{
				ShowErrorMessage("Data table is empty, thus nothing needs to be copyied to Origin!");
				return;
			}

			var app = conn.Application;
			//Origin.Application app = new Origin.Application();
			//app.NewProject();

			Origin.WorksheetPage wbk;
			if (null == (wbk = app.WorksheetPages[strWksName]))
			{
				wbk = app.WorksheetPages.Add();
				wbk.Name = strWksName;
				wbk.LongName = strWksName;
			}

			// for every group in our worksheet, make a separate origin worksheet

			Origin.Worksheet wks = wbk.Layers[0] as Origin.Worksheet;
			wks.ClearData();
			wks.Cols = 0;
			wks.set_LabelVisible(Origin.LABELTYPEVALS.LT_LONG_NAME, true);

			// Set the column names

			for (int i = 0; i < srcTable.DataColumnCount; ++i)
			{
				var srcCol = srcTable.DataColumns[i];
				var srcGroup = srcTable.DataColumns.GetColumnGroup(srcCol);

				Origin.Column col = wks.Columns.Add(srcTable.DataColumns.GetColumnName(i));
				col.LongName = srcTable.DataColumns.GetColumnName(i);

				if (srcCol is DoubleColumn)
				{
					col.DataFormat = COLDATAFORMAT.DF_DOUBLE;
					col.SetData((srcCol as DoubleColumn).Array);
				}
				else if (srcCol is DateTimeColumn)
				{
					col.DataFormat = COLDATAFORMAT.DF_DATE;
					col.SetData((srcCol as DateTimeColumn).Array);
				}
				else if (srcCol is TextColumn)
				{
					col.DataFormat = COLDATAFORMAT.DF_TEXT;
					col.SetData((srcCol as TextColumn).Array);
				}
				else
				{
					throw new NotImplementedException("Type of column not implemented");
				}

				col.Type = AltaxoToOriginColumnType(srcTable.DataColumns.GetColumnKind(srcCol));
			} // end of loop for all data columns

			// now put the property columns to ORIGIN
			// note that ORIGIN has only special property columns
			// LongName (but this is set already), Units, Comments
			// and more generic property columns accessible by Parameter(0), Parameter(1) and so on

			var usedPropColIndices = new HashSet<int>();

			// Longname
			for (int i = 0; i < srcTable.PropCols.ColumnCount; ++i)
			{
				if (usedPropColIndices.Contains(i))
					continue;
				if (IsLikeAnyOf(srcTable.PropCols.GetColumnName(i), LongNamePropertyColumnNames))
				{
					usedPropColIndices.Add(i);
					for (int j = 0; j < srcTable.DataColumnCount; ++j)
					{
						wks.Columns[j].LongName = srcTable.PropCols[i][j].ToString();
					}
					wks.set_LabelVisible(Origin.LABELTYPEVALS.LT_LONG_NAME, true);
				}
			}

			// Units
			for (int i = 0; i < srcTable.PropCols.ColumnCount; ++i)
			{
				if (usedPropColIndices.Contains(i))
					continue;
				if (IsLikeAnyOf(srcTable.PropCols.GetColumnName(i), UnitPropertyColumnNames))
				{
					usedPropColIndices.Add(i);
					for (int j = 0; j < srcTable.DataColumnCount; ++j)
					{
						wks.Columns[j].Units = srcTable.PropCols[i][j].ToString();
					}
					wks.set_LabelVisible(Origin.LABELTYPEVALS.LT_UNIT, true);
				}
			}
			// Comments
			for (int i = 0; i < srcTable.PropCols.ColumnCount; ++i)
			{
				if (usedPropColIndices.Contains(i))
					continue;
				if (IsLikeAnyOf(srcTable.PropCols.GetColumnName(i), CommentPropertyColumnNames))
				{
					usedPropColIndices.Add(i);
					for (int j = 0; j < srcTable.DataColumnCount; ++j)
					{
						wks.Columns[j].Comments = srcTable.PropCols[i][j].ToString();
					}
					wks.set_LabelVisible(Origin.LABELTYPEVALS.LT_COMMENT, true);
				}
			}

			// other property columns

			for (int i = 0, k = 0; i < srcTable.PropCols.ColumnCount; ++i)
			{
				if (usedPropColIndices.Contains(i))
					continue;
				usedPropColIndices.Add(i);
				for (int j = 0; j < srcTable.DataColumnCount; ++j)
				{
					wks.Columns[j].Parameter[k] = srcTable.PropCols[i][j].ToString();
				}
				wks.set_LabelVisible(Origin.LABELTYPEVALS.LT_PARAM, true);
				++k;
			}
		}

		/// <summary>
		/// Tests if column reordering is neccessary in order to put a table to origin. Column reordering is neccessary if the table contains more than
		/// one column group, and the x column of every column group is not the first column. Origin only supports table with multiple groups if the first column
		/// of every group is the x-column.
		/// </summary>
		/// <param name="table">The table to test.</param>
		/// <returns>True if column reodering is neccessary, false otherwise.</returns>
		public static bool IsColumnReorderingNeccessaryForPuttingTableToOrigin(Data.DataTable table)
		{
			// Testen, wieviele Gruppen
			var groupColumns = new Dictionary<int, int>(); // counts the number of columns per group
			for (int i = 0; i < table.DataColumnCount; i++)
			{
				int currentGroup = table.DataColumns.GetColumnGroup(i);

				if (groupColumns.ContainsKey(currentGroup))
					groupColumns[currentGroup] += 1;
				else
					groupColumns.Add(currentGroup, 1);

				var currentKind = table.DataColumns.GetColumnKind(i);

				if (groupColumns.Count > 1 && groupColumns[currentGroup] > 1)
				{
					if (currentKind == Altaxo.Data.ColumnKind.X)
						return true;

					if (currentGroup != table.DataColumns.GetColumnGroup(i - 1))
						return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Reorders the columns so that they can be copyied into an Origin table.
		/// </summary>
		/// <param name="table">The table to reorder.</param>
		public static void ReorderColumnsInTableForCompatibilityWithOrigin(Data.DataTable table)
		{
			var groupBegin = new Dictionary<int, Data.DataColumn>();
			var groupEnd = new Dictionary<int, Data.DataColumn>();

			for (int i = 0; i < table.DataColumnCount; i++)
			{
				var currentCol = table[i];
				int currentGroup = table.DataColumns.GetColumnGroup(i);

				if (!groupBegin.ContainsKey(currentGroup))
				{
					groupBegin.Add(currentGroup, currentCol);
					groupEnd.Add(currentGroup, currentCol);
				}

				var currentKind = table.DataColumns.GetColumnKind(i);
				// if the column is a x-column, then move it to the begin of the group
				if (currentKind == Altaxo.Data.ColumnKind.X && !object.ReferenceEquals(currentCol, groupBegin[currentGroup]))
				{
					int destIndex = table.DataColumns.GetColumnNumber(groupBegin[currentGroup]);
					table.ChangeColumnPosition(new Collections.ContiguousIntegerRange(i, 1), destIndex);
					groupBegin[currentGroup] = currentCol;
					continue;
				}

				// if the column is away from the rest of the group, move it to the end of the group
				int lastIndex = table.DataColumns.GetColumnNumber(groupEnd[currentGroup]);
				if (i > 1 + lastIndex)
				{
					int destIndex = 1 + lastIndex;
					table.ChangeColumnPosition(new Collections.ContiguousIntegerRange(i, 1), destIndex);
					groupEnd[currentGroup] = currentCol;
					continue;
				}

				groupEnd[currentGroup] = currentCol;
			}
		}

		#endregion Send worksheet data to origin

		#region Get data from origin

		/// <summary>
		/// Retrieves the data columns from an Origin table.
		/// </summary>
		/// <param name="wks">The Origin worksheet to retrieve the data from.</param>
		/// <returns>The data columns with the data from the Origin worksheet.</returns>
		public static Altaxo.Data.DataColumnCollection GetDataColumns(this Origin.Worksheet wks)
		{
			int nCols = wks.Cols;

			Altaxo.Data.DataColumnCollection result = new Altaxo.Data.DataColumnCollection();

			for (int c = 0; c < nCols; c++)
			{
				var srcCol = wks.Columns[c];

				Altaxo.Data.DataColumn destCol = GetAltaxoColumnFromOriginDataFormat(srcCol.DataFormat);

				int groupNumber = -1;

				var altaxoColumnKind = OriginToAltaxoColumnKind(srcCol.Type);

				if (altaxoColumnKind == Altaxo.Data.ColumnKind.X)
					groupNumber++;

				if (destCol is DoubleColumn)
				{
					var data = srcCol.GetData(Origin.ARRAYDATAFORMAT.ARRAY1D_NUMERIC, 0, -1, 0);
					(destCol as DoubleColumn).Array = (double[])data;
				}
				else if (destCol is DateTimeColumn)
				{
					var data = (double[])srcCol.GetData(Origin.ARRAYDATAFORMAT.ARRAY1D_NUMERIC, 0, -1, 0);
					const double refDateAsDouble = 2451910; // this is the number of days in julian calendar belonging to the date below...
					DateTime refDate = DateTime.Parse("2001-01-01", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AssumeUniversal);
					for (int i = data.Length - 1; i >= 0; --i)
					{
						destCol[i] = refDate.AddDays(data[i] - refDateAsDouble);
					}
				}
				else if (destCol is TextColumn)
				{
					var data = srcCol.GetData(Origin.ARRAYDATAFORMAT.ARRAY1D_TEXT, 0, -1, 0);
					(destCol as TextColumn).Array = (string[])data;
				}

				result.Add(destCol, srcCol.LongName ?? srcCol.Name, altaxoColumnKind, Math.Max(0, groupNumber));
			}

			return result;
		}

		public static DataColumnCollection GetPropertyColumns(this Origin.Worksheet wks)
		{
			if (null == wks)
				throw new ArgumentNullException("wks");

			DataColumnCollection result = new DataColumnCollection();

			// I found no way to ask, if a label column is used or not
			// therefore, we have to try all cells inside the longname, the units and the comments label column

			Dictionary<string, Altaxo.Data.TextColumn> labelCols = new Dictionary<string, Altaxo.Data.TextColumn>();

			DataColumn destLongNameCol = null, destUnitCol = null, destCommentCol = null;
			DataColumn[] paraCol = new DataColumn[20];

			var srcDataCols = wks.Cols;

			for (int i = 0; i < srcDataCols; ++i)
			{
				var srcCol = wks.Columns[i];

				if (!string.IsNullOrEmpty(srcCol.LongName))
				{
					if (null == destLongNameCol)
						destLongNameCol = result.EnsureExistence("LongName", typeof(TextColumn), ColumnKind.V, 0);
					destLongNameCol[i] = srcCol.LongName;
				}

				if (!string.IsNullOrEmpty(srcCol.Units))
				{
					if (null == destUnitCol)
						destUnitCol = result.EnsureExistence("Unit", typeof(TextColumn), ColumnKind.V, 0);
					destUnitCol[i] = srcCol.Units;
				}

				if (!string.IsNullOrEmpty(srcCol.Comments))
				{
					if (null == destCommentCol)
						destCommentCol = result.EnsureExistence("Comments", typeof(TextColumn), ColumnKind.V, 0);
					destCommentCol[i] = srcCol.Comments;
				}

				for (int nPara = 0; nPara <= 11; ++nPara)
				{
					if (!string.IsNullOrEmpty(srcCol.Parameter[nPara]))
					{
						if (null == paraCol[nPara])
							paraCol[nPara] = result.EnsureExistence("Parameter" + nPara.ToString(), typeof(TextColumn), ColumnKind.V, 0);
						paraCol[nPara][i] = srcCol.Parameter[nPara];
					}
				}
			}

			return result;
		}

		public static string GetTable(this OriginConnection conn, string originWorksheetName, Altaxo.Data.DataTable destTable)
		{
			if (!conn.IsConnected())
				return "Not connected to Origin";

			var app = conn.Application;

			Origin.WorksheetPage wbk;
			if (null == (wbk = app.WorksheetPages[originWorksheetName]))
			{
				return string.Format("No origin worksheet named {0} found!", originWorksheetName);
			}

			Origin.Worksheet wks = wbk.Layers[0] as Origin.Worksheet;

			var dataTemplate = GetDataColumns(wks);
			var propCols = GetPropertyColumns(wks);

			var data = destTable.DataColumns;
			data.RemoveColumnsAll();
			data.CopyAllColumnsFrom(dataTemplate);

			var prop = destTable.PropCols;
			prop.RemoveColumnsAll();
			prop.CopyAllColumnsFrom(propCols);

			return null;
		}

		#endregion Get data from origin
	}
}