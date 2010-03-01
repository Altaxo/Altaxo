using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Altaxo.Data;
namespace Altaxo.Addins.OriginConnector
{
  public static class WorksheetActions
  {
    #region Conversion of Altaxo types to/from Origin Types
    public static OriginColumnType AltaxoToOriginColumnKind(Altaxo.Data.ColumnKind ckind)
    {
      switch (ckind)
      {
        case Altaxo.Data.ColumnKind.V:
          return OriginColumnType.Y;
        case Altaxo.Data.ColumnKind.X:
          return OriginColumnType.X;
        case Altaxo.Data.ColumnKind.Y:
          return OriginColumnType.Z;
        case Altaxo.Data.ColumnKind.Label:
          return OriginColumnType.Label;
        case Altaxo.Data.ColumnKind.Err:
        case Altaxo.Data.ColumnKind.mErr:
        case Altaxo.Data.ColumnKind.pErr:
          return OriginColumnType.YError;
        case Altaxo.Data.ColumnKind.Condition:
          return OriginColumnType.Disregard;
        default:
          return OriginColumnType.Y;
      }
    }

    public static Altaxo.Data.ColumnKind OriginToAltaxoColumnKind(OriginColumnType ctype)
    {
      Altaxo.Data.ColumnKind ckind;
      switch (ctype)
      {
        case OriginColumnType.Y:
          ckind = Altaxo.Data.ColumnKind.V; // Y in Origin is equivalent to V in Altaxo
          break;
        case OriginColumnType.Disregard:
          ckind = Altaxo.Data.ColumnKind.V; // disregard is not supported by Altaxo
          break;
        case OriginColumnType.YError:
          ckind = Altaxo.Data.ColumnKind.Err;
          break;
        case OriginColumnType.X:
          ckind = Altaxo.Data.ColumnKind.X;
          break;
        case OriginColumnType.Label:
          ckind = Altaxo.Data.ColumnKind.Label;
          break;
        case OriginColumnType.Z:
          ckind = Altaxo.Data.ColumnKind.Y; // Z in Origin is equivalent to Y in Altaxo
          break;
        case OriginColumnType.XError:
          ckind = Altaxo.Data.ColumnKind.Err; // XErr
          break;
        default:
          ckind = Altaxo.Data.ColumnKind.V;
          break;
      }
      return ckind;
    }

    public static Dictionary<string,string> OriginShortcutToAltaxoPropertyColumnName()
    {
      var result = new Dictionary<string, string>();
      result.Add("L", "LongName");
      result.Add("U", "Unit");
      result.Add("C", "Comment");

      return result;
    }

    /// <summary>
    /// Creates from the property colums a dictionary wich contains the origin property column shortcuts as key (e.g. L, C, U, D1, D2 etc.) and the property column index as value.
    /// </summary>
    /// <param name="propCols">Property column collection to create a dictionary from.</param>
    /// <returns>The dictionary with origin shortcuts as keys and the column index as value.</returns>
    public static Dictionary<string, int> PropertyColumnsToOriginShortcuts(Altaxo.Data.DataColumnCollection propCols)
    {
      Dictionary<string, int> dict = new Dictionary<string, int>();
      int nUserParam = 0;
      for (int i = 0; i < propCols.ColumnCount; i++)
      {
        string cname = propCols.GetColumnName(i);
        // special name treatment
        if (!dict.ContainsKey("L") && (cname == "LongName" || cname == "Long Name" || cname == "LangName"))
          dict.Add("L", i);
        else if (!dict.ContainsKey("U") && (cname == "Unit" || cname == "Units" || cname == "Einheit" || cname == "Einheiten"))
          dict.Add("U", i);
        else if (!dict.ContainsKey("C") && (cname == "Comment" || cname == "Comments" || cname == "Kommentar" || cname == "Kommentare"))
          dict.Add("C", i);
        else
        {
          nUserParam++;
          dict.Add("D" + nUserParam.ToString(), i);
        }
      }
      return dict;
    }
    
    #endregion

    #region Actions on origin worksheets


    public static void ClearTable(this OriginConnection conn, string originWorksheetName)
    {
      if (!conn.IsConnected())
        return;
      string str = "clearworksheet " + originWorksheetName;
      conn.ExecuteOriginCMD(str);
    }

    #endregion


    #region Send worksheet data to origin

    /// <summary>
    /// Sends the property columns of an Altaxo table to the current origin worksheet (this worksheet must be made current beforehand!).
    /// </summary>
    /// <param name="table">The table for which to send the property columns.</param>
    public static void PutPropertyColumns(this OriginConnection conn, Altaxo.Data.DataTable srcTable)
    {
      var dict = PropertyColumnsToOriginShortcuts(srcTable.PropCols);
      var stb = new System.Text.StringBuilder();

      string showPredefinedLabels = string.Empty;
      if (dict.ContainsKey("L"))
        showPredefinedLabels += "L";
      if (dict.ContainsKey("U"))
        showPredefinedLabels += "U";
      if (dict.ContainsKey("C"))
        showPredefinedLabels += "C";

      if (!string.IsNullOrEmpty(showPredefinedLabels))
      {
        conn.ExecuteOriginCMD(string.Format("wks.Labels({0});", showPredefinedLabels));
      }


      // create the User property columns and show them
      stb.Length = 0;
      for (int i = 0; i < srcTable.PropCols.ColumnCount; i++)
      {
        string shortCut = "D" + (i + 1).ToString();
        if (dict.ContainsKey(shortCut))
          stb.AppendFormat(" wks.userParam{0}$={1}; wks.userParam{0}=1;", i + 1, srcTable.PropCols.GetColumnName(dict[shortCut]));
      }
      conn.ExecuteOriginCMD(stb.ToString());


      // fill property columns with values
      foreach (var entry in dict)
      {
        stb.Length = 0;
        for (int dc = 0; dc < srcTable.DataColumnCount; dc++)
          stb.AppendFormat("col({0})[{1}]$={2};", dc + 1, entry.Key, srcTable.PropCols[entry.Value][dc].ToString());
        conn.ExecuteOriginCMD(stb.ToString());
      }
    }


    /// <summary>
    /// Sends the Altaxo table to Origin.
    /// </summary>
    /// <param name="conn">The existing connection to Origin.</param>
    /// <param name="srcTable">The table to send.</param>
    /// <param name="originWorksheetName">The worksheet name of the Origin worksheet to create or to use (if it exists).</param>
    /// <param name="appendRows">If false, existing Origin data are cleared before filled with the Altaxo data. If true, the data are appended to the existing data.</param>
    public static void PutTable(this OriginConnection conn, Altaxo.Data.DataTable srcTable, string originWorksheetName, bool appendRows)
    {
      if (!conn.IsConnected())
        return;

      if (IsColumnReorderingNeccessaryForPuttingTableToOrigin(srcTable))
      {
        srcTable = (DataTable)srcTable.Clone();
        ReorderColumnsInTableForCompatibilityWithOrigin(srcTable);
      }


      var stb = new System.Text.StringBuilder();
      string strCMD;
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
        ShowErrorMessage("Failed to access Origin instance!");
        return;
      }


      // If specified worksheet does not exist then create it
      if (!conn.ExistsWorksheet(strWksName))
      {
        strWksName = conn.Application.CreatePage((int)OriginObjectType.Worksheet, strWksName, "", 2);

        strCMD = string.Format("worksheet -a {0}", nColumns); // create nColumn columns
        conn.ExecuteOriginCMD(strCMD);
      }
      else // worksheet exists
      {
        strCMD = "win -a " + strWksName;
        conn.ExecuteOriginCMD(strCMD); // select the worksheet

        // retrieve the number of columns
        strCMD = "d=wks.ncols;";
        if (!(conn.ExecuteOriginCMD(strCMD)))
          throw new ApplicationException("Error retrieving number of columns");
        int nExistingCols = (int)conn.GetDouble("d");

        if (nExistingCols < nColumns)
        {
          string cmd = string.Format("worksheet -a {0}", nColumns - nExistingCols); // create nColumn columns
          conn.ExecuteOriginCMD(cmd);
        }
      }

      // set the column names
      stb.Length = 0;
      for (int i = 0; i < nColumns; i++)
        stb.AppendFormat("worksheet -n {0} {1};", i + 1, srcTable.DataColumns.GetColumnName(i));
      conn.ExecuteOriginCMD(stb.ToString());

      // set the column format (i.e. Text, DateTime, or Numeric; note: numeric columns are created by default)
      stb.Length = 0;
      for (int i = 0; i < nColumns; i++)
      {
        if (srcTable[i] is Altaxo.Data.TextColumn)
          stb.AppendFormat("worksheet -f {0} 2;", i + 1);
        else if (srcTable[i] is Altaxo.Data.DateTimeColumn)
          stb.AppendFormat("worksheet -f {0} 4;", i + 1);
        // else if (table[i] is Altaxo.Data.DoubleColumn)
        // stb.AppendFormat("worksheet - f {0} 1;", i + 1);
      }
      conn.ExecuteOriginCMD(stb.ToString());

      // set the column type (i.e. X, Y, Z, Err etc.)
      if (srcTable.DataColumns.HaveMultipleGroups())
      {
        strCMD = "wks.multix=1;";
        conn.ExecuteOriginCMD(strCMD);
      }
      stb.Length = 0;
      for (int i = 0; i < nColumns; i++)
      {
        var ckind = AltaxoToOriginColumnKind(srcTable.DataColumns.GetColumnKind(i));
        if (ckind != OriginColumnType.Y)
          stb.AppendFormat("worksheet -t {0} {1}; ", i + 1, (int)ckind);
      }
      conn.ExecuteOriginCMD(stb.ToString());


      // create the property columns
      PutPropertyColumns(conn, srcTable);




      // If the data needs to be appended, get the first destination row:
      int nRowStart = 0;
      if (appendRows)
      {
        strCMD = strWksName + "!wks.col1.name$";
        string str = conn.GetString(strCMD);
        string strDataset = strWksName + "_" + str;
        str = "get " + strDataset + " -e i";
        if (conn.ExecuteOriginCMD(str))
        {
          nRowStart = conn.GetInt32("i");
        }
      }


      int nStartColumn = 0;
      int nLastColumn = 0;

      for (nStartColumn = 0; nStartColumn < nColumns; nStartColumn = nLastColumn)
      {
        // bundle columns of the same type: columns in the range nStartColumn ... nLastColumn-1 have the same type
        System.Type coltype = srcTable[nStartColumn].GetType();
        for (nLastColumn = nStartColumn; nLastColumn < nColumns && srcTable[nLastColumn].GetType() == coltype; nLastColumn++) ;
        int nColumnsNow = nLastColumn - nStartColumn; // number of columns to transfer now

        if (coltype == typeof(Altaxo.Data.DoubleColumn))
        {
          // Initialize two-dimensional array for data:
          var arr2D = new double[nRows, nColumnsNow];
          for (int col = 0; col < nColumnsNow; col++)
          {
            var dataCol = srcTable[col + nStartColumn];
            for (int row = 0; row < nRows; row++)
            {
              if (double.IsNaN(dataCol[row]))
                arr2D[row, col] = double.PositiveInfinity; // it seems that PositiveInfinity represents a missing value in Origin
              else
                arr2D[row, col] = dataCol[row];
            }
          }
          conn.Application.PutWorksheet(strWksName, arr2D, nRowStart, nStartColumn); // Finally put the data into the worksheet, beginning  with the row nRowStart and the start column:
        }
        else if (coltype == typeof(Altaxo.Data.TextColumn))
        {
          // Initialize two-dimensional array for data:
          var arr2D = new string[nRows, nColumnsNow];
          for (int col = 0; col < nColumnsNow; col++)
          {
            var dataCol = srcTable[col + nStartColumn];
            for (int row = 0; row < nRows; row++)
              arr2D[row, col] = dataCol[row];
          }
          conn.Application.PutWorksheet(strWksName, arr2D, nRowStart, nStartColumn); // Finally put the data into the worksheet, beginning  with the row nRowStart and the start column:
        }
        else if (coltype == typeof(Altaxo.Data.DateTimeColumn))
        {
          const double secondsPerDay = 24 * 3600;
          const double refDateAsDouble = 2451910; // this is the number of days in julian calendar belonging to the date below...
          DateTime refDate = DateTime.Parse("2001-01-01", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AssumeUniversal);

          // note that we convert the time values to local time always,
          // since Origin8 don't know anything about UTC times
          var arr2D = new double[nRows, nColumnsNow];
          for (int col = 0; col < nColumnsNow; col++)
          {
            var dataCol = srcTable[col + nStartColumn];
            for (int row = 0; row < nRows; row++)
              arr2D[row, col] = refDateAsDouble + (((DateTime)dataCol[row]).ToLocalTime() - refDate).TotalSeconds / secondsPerDay;
          }
          conn.Application.PutWorksheet(strWksName, arr2D, nRowStart, nStartColumn); // Finally put the data into the worksheet, beginning  with the row nRowStart and the start column:
        }
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
          table.ChangeColumnPosition(new Collections.IntegerRangeAsCollection(i, 1), destIndex);
          groupBegin[currentGroup] = currentCol;
          continue;
        }

        // if the column is away from the rest of the group, move it to the end of the group
        int lastIndex = table.DataColumns.GetColumnNumber(groupEnd[currentGroup]);
        if (i > 1 + lastIndex)
        {
          int destIndex = 1 + lastIndex;
          table.ChangeColumnPosition(new Collections.IntegerRangeAsCollection(i, 1), destIndex);
          groupEnd[currentGroup] = currentCol;
          continue;
        }

        groupEnd[currentGroup] = currentCol;
      }
    }

    #endregion // send data to origin

    #region Get data from origin

    /// <summary>
    /// Retrieves the structure of the data section of a origin table.
    /// </summary>
    /// <param name="worksheetName">Name of the origin worksheet to explore.</param>
    /// <returns></returns>
    public static Altaxo.Data.DataColumnCollection GetDataColumnStructure(this OriginConnection conn, string originWorksheetName)
    {
      string cmd = "win -a " + originWorksheetName;
      conn.ExecuteOriginCMD(cmd); // select the worksheet

      // retrieve the number of columns
      cmd = "d=wks.ncols;";
      if (!(conn.ExecuteOriginCMD(cmd)))
        throw new ApplicationException("Error retrieving number of columns");
      int nCols = conn.GetInt32("d");

      Altaxo.Data.DataColumnCollection result = new Altaxo.Data.DataColumnCollection();

      for (int c = 0; c < nCols; c++)
      {
        cmd = string.Format("s$=wks.col{0}.name$; d=wks.col{0}.format; t=wks.col{0}.type;", c + 1);
        if (!(conn.ExecuteOriginCMD(cmd)))
          throw new ApplicationException("Error retrieving properties of column");

        string cname = conn.GetString("s$");
        int cformat = conn.GetInt32("d");
        int ctype = conn.GetInt32("t");

        // create a column
        Altaxo.Data.DataColumn dataCol = null;
        if (cformat == 2)
          dataCol = new Altaxo.Data.TextColumn();
        else if (cformat == 4)
          dataCol = new Altaxo.Data.DateTimeColumn();
        else
          dataCol = new Altaxo.Data.DoubleColumn();

        int groupNumber = -1;

        Altaxo.Data.ColumnKind ckind = OriginToAltaxoColumnKind((OriginColumnType)ctype);

        if (ckind == Altaxo.Data.ColumnKind.X)
          groupNumber++;

        result.Add(dataCol, cname, ckind, Math.Max(0, groupNumber));
      }

      return result;
    }

    public static string GetPropertyColumns(this OriginConnection conn, string originWorksheetName, Altaxo.Data.DataTable destTable)
    {
      if (!conn.IsConnected())
        return "Not connected to Origin";

      if (!conn.ExistsWorksheet(originWorksheetName))
      {
        return string.Format("No origin worksheet named {0} found!", originWorksheetName);
      }

      // I found no way to ask, if a label column is used or not
      // therefore, we have to try all cells inside the longname, the units and the comments label column

      Dictionary<string, Altaxo.Data.TextColumn> labelCols = new Dictionary<string, Altaxo.Data.TextColumn>();

      labelCols.Add("L", new Altaxo.Data.TextColumn());
      labelCols.Add("U", new Altaxo.Data.TextColumn());
      labelCols.Add("C", new Altaxo.Data.TextColumn());

      foreach (var entry in labelCols)
      {
        for (int i = 0; i < destTable.DataColumnCount; i++)
        {
          string v = conn.GetString(string.Format("col({0})[{1}]$", i + 1, entry.Key));
          if (!string.IsNullOrEmpty(v))
            entry.Value[i] = v;
        }
      }

      var dict = OriginShortcutToAltaxoPropertyColumnName();
      foreach (var entry in labelCols)
      {
        if (entry.Value.Count > 0)
          destTable.PropCols.Add(entry.Value, dict[entry.Key]);
      }

      // now test also for user defined label columns, 
      // this is somewhat easier, since the presence can be tested

      for (int u = 1; u < 9; u++)
      {
        string uName = conn.GetString(string.Format("wks.userParam{0}$",u));
       

        if (!string.IsNullOrEmpty(uName))
        {
          var newCol = new Altaxo.Data.TextColumn();
          for (int i = 0; i < destTable.DataColumnCount; i++)
          {
            string v = conn.GetString(string.Format("col({0})[D{1}]$", i + 1, u));
            if (!string.IsNullOrEmpty(v))
              newCol[i] = v;
          }
          destTable.PropCols.Add(newCol, uName);
        }
      }



      return null;
    }

    public static string GetTable(this OriginConnection conn, string originWorksheetName, Altaxo.Data.DataTable destTable)
    {
      if (!conn.IsConnected())
        return "Not connected to Origin";


      if (!conn.ExistsWorksheet(originWorksheetName))
      {
        return string.Format("No origin worksheet named {0} found!", originWorksheetName);
      }


      var dataTemplate = conn.GetDataColumnStructure(originWorksheetName);
      object rawData = conn.Application.GetWorksheet(originWorksheetName, 0, 0, -1, -1, 0);
      Array arr = rawData as Array;
      if (arr == null)
        return string.Format("GetWorksheet didn't return an array, instead it returned {0}", rawData.ToString());

      var data = destTable.DataColumns;

      data.RemoveColumnsAll();
      data.CopyAllColumnsFrom(dataTemplate);

      const double refDateAsDouble = 2451910; // this is the number of days in julian calendar belonging to the date below...
      DateTime refDate = DateTime.Parse("2001-01-01", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AssumeUniversal);

      int rowLow = arr.GetLowerBound(0);
      int colLow = arr.GetLowerBound(1);
      int rows = arr.GetLength(0);
      int cols = arr.GetLength(1);

      for (int c = 0; c < data.ColumnCount; c++)
      {
        var col = data[c];
        if (col is Altaxo.Data.TextColumn)
        {
          for (int r = 0; r < rows; r++)
            col[r] = arr.GetValue(r + rowLow, c + colLow) as string;
        }
        else if (col is Altaxo.Data.DoubleColumn)
        {
          for (int r = 0; r < rows; r++)
          {
            object o = arr.GetValue(r + rowLow, c + colLow);
            if (o is double)
              col[r] = (double)o;
          }
        }
        else if (col is Altaxo.Data.DateTimeColumn)
        {
          for (int r = 0; r < rows; r++)
          {
            object o = arr.GetValue(r + rowLow, c + colLow);
            if (o is double)
            {
              DateTime date = refDate.AddDays((double)o - refDateAsDouble);
              col[r] = date;
            }
          }
        }
      }


      // now get also the table properties (label columns)
      return GetPropertyColumns(conn, originWorksheetName, destTable);
    }


    #endregion // get data from origin


    #region Helper functions

    private static void ShowErrorMessage(string strMsg)
    {
      Altaxo.Current.Gui.ErrorMessageBox(strMsg);
    }

    #endregion
  }
}
