#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2004 Dr. Dirk Lellinger
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
#endregion

using System.Text.RegularExpressions;
using Altaxo.Worksheet.GUI;

namespace Altaxo.Worksheet.Commands
{
  /// <summary>
  /// Contains static functions for handling column commands.
  /// </summary>
  public class ColumnCommands
  {
    #region Rename column

    /// <summary>
    /// Renames the selected data column or property column.
    /// </summary>
    /// <param name="ctrl">The worksheet controller for the table.</param>
    public static void RenameSelectedColumn(WorksheetController ctrl)
    {
      if(ctrl.SelectedColumns.Count==1 && ctrl.SelectedPropertyColumns.Count==0)
      {
        Altaxo.Data.DataColumn col = ctrl.Doc.DataColumns[ctrl.SelectedColumns[0]];
        Main.GUI.TextValueInputController tvctrl = new Main.GUI.TextValueInputController(
          col.Name,
          new Main.GUI.RenameColumnDialog()
          );

        tvctrl.Validator = new DataColumnRenameValidator(col,ctrl);
        if(tvctrl.ShowDialog(ctrl.View.TableViewForm))
          ctrl.Doc.DataColumns.SetColumnName(col,tvctrl.InputText);
      }
      if(ctrl.SelectedColumns.Count==0 && ctrl.SelectedPropertyColumns.Count==1)
      {
        Altaxo.Data.DataColumn col = ctrl.Doc.PropCols[ctrl.SelectedPropertyColumns[0]];
        Main.GUI.TextValueInputController tvctrl = new Main.GUI.TextValueInputController(
          col.Name,
          new Main.GUI.RenameColumnDialog()
          );

        tvctrl.Validator = new PropertyColumnRenameValidator(col,ctrl);
        if(tvctrl.ShowDialog(ctrl.View.TableViewForm))
          ctrl.Doc.PropCols.SetColumnName(col,tvctrl.InputText);
      }
    }


    /// <summary>
    /// Helper class to make sure that user choosen data column name does not already exists.
    /// </summary>
    public class DataColumnRenameValidator : Main.GUI.TextValueInputController.NonEmptyStringValidator
    {
      Altaxo.Data.DataColumn m_Col;
      WorksheetController m_Ctrl;
      
      public DataColumnRenameValidator(Altaxo.Data.DataColumn col, WorksheetController ctrl)
        : base("The column name must not be empty! Please enter a valid name.")
      {
        m_Col = col;
        m_Ctrl = ctrl;
      }

      public override string Validate(string name)
      {
        string err = base.Validate(name);
        if(null!=err)
          return err;

        if(m_Col.Name==name)
          return null;
        else if(m_Ctrl.Doc.DataColumns.ContainsColumn(name))
          return "This column name already exists, please choose another name!";
        else
          return null;

      }
    }


    /// <summary>
    /// Helper class to make sure that user choosen property column name does not already exists.
    /// </summary>
    public class PropertyColumnRenameValidator : Main.GUI.TextValueInputController.NonEmptyStringValidator
    {
      Altaxo.Data.DataColumn m_Col;
      WorksheetController m_Ctrl;
      
      public PropertyColumnRenameValidator(Altaxo.Data.DataColumn col, WorksheetController ctrl)
        : base("The column name must not be empty! Please enter a valid name.")
      {
        m_Col = col;
        m_Ctrl = ctrl;
      }

      public override string Validate(string name)
      {
        string err = base.Validate(name);
        if(null!=err)
          return err;

        if(m_Col.Name==name)
          return null;
        else if(m_Ctrl.Doc.PropCols.ContainsColumn(name))
          return "This column name already exists, please choose another name!";
        else
          return null;

      }
    }

    

    #endregion

    #region Set group number
    /// <summary>
    /// Sets the group number of the selected column
    /// </summary>
    /// <param name="ctrl">The worksheet controller for the table.</param>
    public static void SetSelectedColumnGroupNumber(WorksheetController ctrl)
    {
      if(ctrl.SelectedColumns.Count>0 || ctrl.SelectedPropertyColumns.Count>0)
      {
        int grpNumber = 0;
        if(ctrl.SelectedColumns.Count>0)
          grpNumber = ctrl.DataTable.DataColumns.GetColumnGroup(ctrl.SelectedColumns[0]);
        else if(ctrl.SelectedPropertyColumns.Count>0)
          grpNumber = ctrl.DataTable.PropertyColumns.GetColumnGroup(ctrl.SelectedPropertyColumns[0]);
        
        Main.GUI.IntegerValueInputController ivictrl = new Main.GUI.IntegerValueInputController(
          grpNumber,
          new Main.GUI.SingleValueDialog("Set group number","Please enter a group number (>=0):")
          );

        ivictrl.Validator = new Altaxo.Main.GUI.IntegerValueInputController.ZeroOrPositiveIntegerValidator();
        if(ivictrl.ShowDialog(ctrl.View.TableViewForm))
        {
          SetSelectedColumnGroupNumber(ctrl,ivictrl.EnteredContents);
        }
      }
    }


    /// <summary>
    /// Sets the group number of the currently selected columns to <code>nGroup</code>.
    /// </summary>
    /// <param name="ctrl">The worksheet controller.</param>
    /// <param name="nGroup">The group number to set for the selected columns.</param>
    public static void SetSelectedColumnGroupNumber(WorksheetController ctrl, int nGroup)
    {
      for(int i=0;i<ctrl.SelectedColumns.Count;i++)
      {
        ctrl.DataTable.DataColumns.SetColumnGroup(ctrl.SelectedColumns[i], nGroup);
      }
      
      for(int i=0;i<ctrl.SelectedPropertyColumns.Count;i++)
      {
        ctrl.DataTable.PropertyColumns.SetColumnGroup(ctrl.SelectedPropertyColumns[i], nGroup);
      }

      ctrl.ClearAllSelections();

      ctrl.UpdateTableView();
    }
    #endregion

    #region Set column to X, Value, Label...
    /// <summary>
    /// Sets the column kind of the first selected column or property column to a X column.
    /// </summary>
    public static void SetSelectedColumnAsX(WorksheetController ctrl)
    {
      bool bChanged = false;
      if(ctrl.SelectedColumns.Count>0)
      {
        ctrl.DataTable.DataColumns.SetColumnKind(ctrl.SelectedColumns[0],Altaxo.Data.ColumnKind.X);
        bChanged = true;
      }
      if(ctrl.SelectedPropertyColumns.Count>0)
      {
        ctrl.DataTable.PropertyColumns.SetColumnKind(ctrl.SelectedPropertyColumns[0],Altaxo.Data.ColumnKind.X);
        bChanged = true;
      }
      if(bChanged)
        ctrl.UpdateTableView(); // draw new because 

    }

    /// <summary>
    /// Sets the column kind of all selected columns or property columns to a label column.
    /// </summary>
    public static void SetSelectedColumnAsLabel(WorksheetController ctrl)
    {
      bool bChanged = false;
      if(ctrl.SelectedColumns.Count>0)
      {
        for(int i=0;i<ctrl.SelectedColumns.Count;i++)
          ctrl.DataTable.DataColumns.SetColumnKind(ctrl.SelectedColumns[i],Altaxo.Data.ColumnKind.Label);
        bChanged = true;
      }
      if(ctrl.SelectedPropertyColumns.Count>0)
      {
        for(int i=0;i<ctrl.SelectedPropertyColumns.Count;i++)
          ctrl.DataTable.PropertyColumns.SetColumnKind(ctrl.SelectedPropertyColumns[i],Altaxo.Data.ColumnKind.Label);
        bChanged = true;
      }

      if(bChanged)
        ctrl.UpdateTableView(); // draw new because 

    }

    /// <summary>
    /// Sets the column kind of the first selected column to a value column.
    /// </summary>
    public static void SetSelectedColumnAsValue(WorksheetController ctrl)
    {
      bool bChanged = false;
      if(ctrl.SelectedColumns.Count>0)
      {
        for(int i=0;i<ctrl.SelectedColumns.Count;i++)
          ctrl.DataTable.DataColumns.SetColumnKind(ctrl.SelectedColumns[i],Altaxo.Data.ColumnKind.V);
        bChanged = true;
      }
      if(ctrl.SelectedPropertyColumns.Count>0)
      {
        for(int i=0;i<ctrl.SelectedPropertyColumns.Count;i++)
          ctrl.DataTable.PropertyColumns.SetColumnKind(ctrl.SelectedPropertyColumns[i],Altaxo.Data.ColumnKind.V);
        bChanged = true;
      }
      if(bChanged)
        ctrl.UpdateTableView(); // draw new because 

    }
    #endregion

    #region Extract property values

    /// <summary>
    /// Extracts the property values of the selected property columns.
    /// </summary>
    /// <param name="ctrl">The controller that controls the table.</param>
    public static void ExtractPropertyValues(WorksheetController ctrl)
    {
      for(int i=0;i<ctrl.SelectedPropertyColumns.Count;i++)
      {
        Altaxo.Data.DataColumn col = ctrl.DataTable.PropCols[ctrl.SelectedPropertyColumns[i]];
        ExtractPropertiesFromColumn(col,ctrl.DataTable.PropCols);
      }
      ctrl.ClearAllSelections();
    }

    /// <summary>
    /// This function searches for patterns like aaa=bbb in the items of the text column. If it finds such a item, it creates a column named aaa
    /// and stores the value bbb at the same position in it as in the text column.
    /// </summary>
    /// <param name="col">The column where to search for the patterns described above.</param>
    /// <param name="store">The column collection where to store the newly created columns of properties.</param>
    public static void ExtractPropertiesFromColumn(Altaxo.Data.DataColumn col, Altaxo.Data.DataColumnCollection store)
    {
      for(int nRow=0;nRow<col.Count;nRow++)
      {
        ExtractPropertiesFromString(col[nRow].ToString(),store, nRow);
      }
    }

    /// <summary>
    /// This function searches for patterns like aaa=bbb in the provided string. If it finds such a item, it creates a column named aaa
    /// and stores the value bbb at the same position in it as in the text column.
    /// </summary>
    /// <param name="strg">The string where to search for the patterns described above.</param>
    /// <param name="store">The column collection where to store the newly created columns of properties.</param>
    /// <param name="index">The index into the column where to store the property value.</param>
    public static void ExtractPropertiesFromString(string strg, Altaxo.Data.DataColumnCollection store, int index)
    {
      string pat;
      pat = @"(\S+)=(\S+)";

      Regex r = new Regex(pat, RegexOptions.Compiled | RegexOptions.IgnoreCase);

      for ( Match m = r.Match(strg); m.Success; m = m.NextMatch()) 
      {
        string propname = m.Groups[1].ToString();
        string propvalue = m.Groups[2].ToString();

        // System.Diagnostics.Trace.WriteLine("Found the pair " + propname + " : " + propvalue);

        if(!store.ContainsColumn(propname))
        {
          Altaxo.Data.DataColumn col;
          if(Altaxo.Serialization.DateTimeParsing.IsDateTime(propvalue))
            col = new Altaxo.Data.DateTimeColumn();
          else if(Altaxo.Serialization.NumberConversion.IsNumeric(propvalue))
            col = new Altaxo.Data.DoubleColumn();
          else
            col = new Altaxo.Data.TextColumn();
        
          store.Add(col,propname); // add the column to the collection
        }

        // now the column is present we can store the value in it.
        store[propname][index] = new Altaxo.Data.AltaxoVariant(propvalue);
      }   
    }

    #endregion

    #region Set column values

    public static void SetColumnValues(WorksheetController ctrl)
    {
      if(ctrl.SelectedColumns.Count<=0)
        return; // no column selected

      Altaxo.Data.DataColumn dataCol = ctrl.DataTable[ctrl.SelectedColumns[0]];
      if(null==dataCol)
        return;

      //Data.ColumnScript colScript = (Data.ColumnScript)altaxoDataGrid1.columnScripts[dataCol];

      Data.ColumnScript colScript = ctrl.DataTable.DataColumns.ColumnScripts[dataCol];

      Altaxo.Main.GUI.DialogFactory.ShowColumnScriptDialog(ctrl.View.TableViewForm,ctrl.DataTable,dataCol,colScript);
    }

    #endregion

  }

  
}
