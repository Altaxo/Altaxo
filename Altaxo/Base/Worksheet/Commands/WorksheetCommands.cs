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
//    along with ctrl program; if not, write to the Free Software
//    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
//
/////////////////////////////////////////////////////////////////////////////
#endregion

using Altaxo.Worksheet.GUI;

namespace Altaxo.Worksheet.Commands
{
  /// <summary>
  /// WorksheetCommands contain operations concerning the worksheet itself, such as rename.
  /// </summary>
  public class WorksheetCommands
  {
    public static void Rename(WorksheetController ctrl)
    {
      Main.GUI.TextValueInputController tvctrl = new Main.GUI.TextValueInputController(
        ctrl.Doc.Name,
        new Main.GUI.SingleValueDialog("Rename Worksheet","Enter a name for the worksheet:")
        );

      tvctrl.Validator = new WorksheetRenameValidator(ctrl.Doc,ctrl);
      if(tvctrl.ShowDialog(ctrl.View.TableViewForm))
        ctrl.Doc.Name = tvctrl.InputText.Trim();
    }

    protected class WorksheetRenameValidator : Main.GUI.TextValueInputController.NonEmptyStringValidator
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
      /*
      string msg = ctrl.DataTable.Transpose();

      if(null!=msg)
        System.Windows.Forms.MessageBox.Show(ctrl.View.TableViewForm,msg);
        */


      Worksheet.GUI.TransposeWorksheetControl transposeview = new Worksheet.GUI.TransposeWorksheetControl();
      Worksheet.GUI.TransposeWorksheetController transposectrl = new Worksheet.GUI.TransposeWorksheetController(ctrl.DataTable,transposeview);
      
      Altaxo.Main.GUI.DialogShellController dsc = new Altaxo.Main.GUI.DialogShellController(
        new Altaxo.Main.GUI.DialogShellView(transposeview),transposectrl,"Transpose worksheet",false);

      dsc.ShowDialog(Current.MainWindow);
    }
    

    public static void AddDataColumns(WorksheetController ctrl)
    {
      Altaxo.Main.GUI.DialogFactory.ShowAddColumnsDialog(ctrl.View.TableViewForm,ctrl.DataTable,false);
    }

    public static void AddPropertyColumns(WorksheetController ctrl)
    {
      Altaxo.Main.GUI.DialogFactory.ShowAddColumnsDialog(ctrl.View.TableViewForm,ctrl.DataTable,true);
    }
    


  }
}
