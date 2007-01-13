#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2007 Dr. Dirk Lellinger
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
#endregion

using Altaxo.Worksheet.GUI;
using Altaxo.Gui.Common;

namespace Altaxo.Worksheet.Commands
{
  /// <summary>
  /// WorksheetCommands contain operations concerning the worksheet itself, such as rename.
  /// </summary>
  public class WorksheetCommands
  {
    public static void Rename(WorksheetController ctrl)
    {
      TextValueInputController tvctrl = new TextValueInputController(ctrl.Doc.Name,"Enter a name for the worksheet:");
      tvctrl.Validator = new WorksheetRenameValidator(ctrl.Doc,ctrl);
      if(Current.Gui.ShowDialog(tvctrl,"Rename worksheet",false))
        ctrl.Doc.Name = tvctrl.InputText.Trim();
    }

    protected class WorksheetRenameValidator : TextValueInputController.NonEmptyStringValidator
    {
      Altaxo.Data.DataTable m_Table;
      WorksheetController m_Ctrl;
      
      public WorksheetRenameValidator(Altaxo.Data.DataTable tab, WorksheetController ctrl)
        : base("The worksheet name must not be empty! Please enter a valid name.")
      {
        m_Table = tab;
        m_Ctrl = ctrl;
      }

      public override string Validate(string wksname)
      {
        string err = base.Validate(wksname);
        if(null!=err)
          return err;

        if(m_Table.Name==wksname)
          return null;
        else if(Data.DataTableCollection.GetParentDataTableCollectionOf(m_Ctrl.Doc)==null)
          return null; // if there is no parent data set we can enter anything
        else if(Data.DataTableCollection.GetParentDataTableCollectionOf(m_Ctrl.Doc).ContainsTable(wksname))
          return "This worksheet name already exists, please choose another name!";
        else
          return null;
      }
    }
    


    public static void Duplicate(WorksheetController ctrl)
    {
      Altaxo.Data.DataTable clonedTable = (Altaxo.Data.DataTable)ctrl.DataTable.Clone();
     

      // find a new name for the cloned table and add it to the DataTableCollection
      clonedTable.Name = Data.DataTableCollection.GetParentDataTableCollectionOf(ctrl.DataTable).FindNewTableName();
      Data.DataTableCollection.GetParentDataTableCollectionOf(ctrl.DataTable).Add(clonedTable);
      Current.ProjectService.CreateNewWorksheet(clonedTable);
    }
  
    /// <summary>
    /// This will create a property column as text column with the names of the data columns.
    /// </summary>
    /// <param name="ctrl">The worksheet controller.</param>
    public static void CreatePropertyColumnOfColumnNames(WorksheetController ctrl)
    {
      CreatePropertyColumnOfColumnNames(ctrl.Doc);
    }

    /// <summary>
    /// This will create a property column as text column with the names of the data columns.
    /// </summary>
    /// <param name="table">The data table.</param>
    public static void CreatePropertyColumnOfColumnNames(Altaxo.Data.DataTable table)
    {
      const string NameColumnName="ColumnName";
      
      Altaxo.Data.TextColumn col = table.PropertyColumns[NameColumnName] as Altaxo.Data.TextColumn;

      if(col==null)
      {
        col = new Altaxo.Data.TextColumn();
        table.PropertyColumns.Add(col,NameColumnName,Altaxo.Data.ColumnKind.Label,0);
      }

      col.Suspend();
      for(int i=table.DataColumnCount-1;i>=0;i--)
      {
        col[i] = table.DataColumns.GetColumnName(i);
      }
      col.Resume();

    }


    public static void Transpose(WorksheetController ctrl)
    {
      Worksheet.GUI.TransposeWorksheetControl transposeview = new Worksheet.GUI.TransposeWorksheetControl();
      Worksheet.GUI.TransposeWorksheetController transposectrl = new Worksheet.GUI.TransposeWorksheetController(ctrl.DataTable);
      transposectrl.ViewObject = transposeview;
      Current.Gui.ShowDialog(transposectrl, "Transpose worksheet", false);
    }
    

    public static void AddDataColumns(WorksheetController ctrl)
    {
      ShowAddColumnsDialog(ctrl.DataTable,false);
    }

    public static void AddPropertyColumns(WorksheetController ctrl)
    {
      ShowAddColumnsDialog(ctrl.DataTable,true);
    }

    /// <summary>
    /// Shows a dialog to add columns to a table.
    /// </summary>
    /// <param name="table">The table where to add the columns.</param>
    /// <param name="bAddToPropertyColumns">If true, the columns are added to the property columns instead of the data columns collection.</param>
    public static void ShowAddColumnsDialog(Altaxo.Data.DataTable table, bool bAddToPropertyColumns)
    {
      Altaxo.Collections.SelectableListNodeList lbitems = new Altaxo.Collections.SelectableListNodeList();
      lbitems.Add(new Altaxo.Collections.SelectableListNode("Numeric", typeof(Altaxo.Data.DoubleColumn), true));
      lbitems.Add(new Altaxo.Collections.SelectableListNode("Date/Time", typeof(Altaxo.Data.DateTimeColumn), false));
      lbitems.Add(new Altaxo.Collections.SelectableListNode("Text", typeof(Altaxo.Data.TextColumn), false));

      IntegerAndComboBoxController ct = new IntegerAndComboBoxController(
        "Number of colums to add:", 1, int.MaxValue, 1,
        "Type of columns to add:", lbitems, 0);

      SpinAndComboBoxControl panel = new SpinAndComboBoxControl();
      ct.View = panel;

      if (true == Current.Gui.ShowDialog(ct,"Add new column(s)",false))
      {
        System.Type columntype = (System.Type)ct.SelectedItem.Item;

        table.Suspend();

        if (bAddToPropertyColumns)
        {
          for (int i = 0; i < ct.IntegerValue; i++)
          {
            table.PropCols.Add((Altaxo.Data.DataColumn)System.Activator.CreateInstance(columntype));
          }
        }
        else
        {
          for (int i = 0; i < ct.IntegerValue; i++)
          {
            table.DataColumns.Add((Altaxo.Data.DataColumn)System.Activator.CreateInstance(columntype));
          }
        }

        table.Resume();
      }
    }


  }
}
