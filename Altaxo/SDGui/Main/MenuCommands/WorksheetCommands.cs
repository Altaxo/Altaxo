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

using System;
using System.Windows.Forms;
using ICSharpCode.Core.AddIns.Codons;
using Altaxo;
using Altaxo.Main;
using Altaxo.Worksheet;
using Altaxo.Worksheet.GUI;
using ICSharpCode.SharpZipLib.Zip;

namespace Altaxo.Worksheet.Commands
{

  /// <summary>
  /// Provides a abstract class for issuing commands that apply to worksheet controllers.
  /// </summary>
  public abstract class AbstractWorksheetControllerCommand : AbstractMenuCommand
  {
    /// <summary>
    /// Determines the currently active worksheet and issues the command to that worksheet by calling
    /// Run with the worksheet as a parameter.
    /// </summary>
    public override void Run()
    {
      Altaxo.Worksheet.GUI.WorksheetController ctrl 
        = Current.Workbench.ActiveViewContent 
        as Altaxo.Worksheet.GUI.WorksheetController;
      
      if(null!=ctrl)
        Run(ctrl);
    }
  
    /// <summary>
    /// Override this function for adding own worksheet commands. You will get
    /// the worksheet controller in the parameter.
    /// </summary>
    /// <param name="ctrl">The worksheet controller this command is applied to.</param>
    public abstract void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl);
  }



  public class SaveAs : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
    {
      ctrl.SaveTableAs(false);
    }
  }

  public class SaveAsTemplate : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
    {
      ctrl.SaveTableAs(true);
    }
  }

  public class ImportAscii : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
    {
      ctrl.ImportAscii();
    }
  }

  public class ImportImage : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
    {
      DataGridOperations.ImportImage(ctrl.DataTable);
    }
  }

  public class ImportGalacticSPC : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
    {
      Altaxo.Serialization.Galactic.Import.ShowDialog(ctrl.View.TableViewForm, ctrl.DataTable);
    }
  }

  public class ExportAscii : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
    {
      ctrl.ExportAscii();
    }
  }

  public class ExportGalacticSPC : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
    {
      Altaxo.Serialization.Galactic.ExportGalacticSpcFileDialog dlg =
        new Altaxo.Serialization.Galactic.ExportGalacticSpcFileDialog();

      dlg.Initialize(ctrl.DataTable,ctrl.SelectedRows,ctrl.SelectedColumns);

      dlg.ShowDialog(ctrl.View.TableViewWindow);
    }
  }


  #region Edit commands

  public class EditRemove : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
    {
      ctrl.RemoveSelected();
    }
  }
  public class EditCopy : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
    {
      DataGridOperations.CopyToClipboard(ctrl);
    }
  }
  public class EditPaste : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
    {
      DataGridOperations.PasteFromClipboard(ctrl);
    }
  }
  #endregion

  #region Plot commands

  public class PlotLine : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
    {
      DataGridOperations.PlotLine(ctrl, true, false);
    }
  }

  public class PlotScatter : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
    {
      DataGridOperations.PlotLine(ctrl, false, true);
    }
  }

  public class PlotLineAndScatter : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
    {
      DataGridOperations.PlotLine(ctrl, true, true);
    }
  }

  public class PlotDensityImage : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
    {
      DataGridOperations.PlotDensityImage(ctrl, true, true);
    }
  }


  #endregion

  #region Worksheet

  public class WorksheetRename : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
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
  }


  public class WorksheetDuplicate : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
    {
      Altaxo.Data.DataTable clonedTable = (Altaxo.Data.DataTable)ctrl.DataTable.Clone();
     

      // find a new name for the cloned table and add it to the DataTableCollection
      clonedTable.Name = Data.DataTableCollection.GetParentDataTableCollectionOf(ctrl.DataTable).FindNewTableName();
      Data.DataTableCollection.GetParentDataTableCollectionOf(ctrl.DataTable).Add(clonedTable);
      Current.ProjectService.CreateNewWorksheet(clonedTable);
    }
  }

  public class WorksheetTranspose : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
    {
      string msg = ctrl.DataTable.Transpose();

      if(null!=msg)
        System.Windows.Forms.MessageBox.Show(ctrl.View.TableViewForm,msg);
    }
  }

  public class AddDataColumns : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
    {
      Altaxo.Main.GUI.DialogFactory.ShowAddColumnsDialog(ctrl.View.TableViewForm,ctrl.DataTable,false);
    }
  }

  public class AddPropertyColumns : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
    {
      Altaxo.Main.GUI.DialogFactory.ShowAddColumnsDialog(ctrl.View.TableViewForm,ctrl.DataTable,true);
    }
  }

  public class OpenTableScriptDialog : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
    {
      Data.DataTable dataTable = ctrl.DataTable;
      Data.TableScript tableScript = ctrl.DataTable.TableScript;

      Worksheet.GUI.TableScriptController controller = new Worksheet.GUI.TableScriptController(dataTable,tableScript);
      Worksheet.GUI.TableScriptControl control = new Altaxo.Worksheet.GUI.TableScriptControl();

      System.Windows.Forms.Form form = new System.Windows.Forms.Form(); // the parent form used as shell for the control
      form.Controls.Add(control);
      form.ClientSize = control.Size;
      control.Dock = System.Windows.Forms.DockStyle.Fill;
      controller.View = control;

      form.Text = "WorksheetScript of " + dataTable.Name;
      ICSharpCode.SharpDevelop.Services.DefaultParserService parserService = (ICSharpCode.SharpDevelop.Services.DefaultParserService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(ICSharpCode.SharpDevelop.Services.DefaultParserService));

      if(parserService!=null)
      {
        parserService.RegisterModalContent(control.EditableContent);
        form.ShowDialog(Altaxo.Current.MainWindow);
        parserService.UnregisterModalContent();
      }
      else
      {
        form.ShowDialog(Altaxo.Current.MainWindow);
      }

      form.Dispose();
    }
  }


  #endregion

  #region Column commands


  public class SetColumnValues : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
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
  }

  public class SetColumnAsX : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
    {
      ctrl.SetSelectedColumnAsX();  
    }
  }

  public class SetColumnAsLabel : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
    {
      ctrl.SetSelectedColumnAsLabel();  
    }
  }

  public class SetColumnAsValue : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
    {
      ctrl.SetSelectedColumnAsValue();  
    }
  }


  public class RenameColumn : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
    {
      if(ctrl.SelectedColumns.Count==1 && ctrl.SelectedPropertyColumns.Count==0)
      {
        Altaxo.Data.DataColumn col = ctrl.Doc.DataColumns[ctrl.SelectedColumns[0]];
        Main.GUI.TextValueInputController tvctrl = new Main.GUI.TextValueInputController(
          col.Name,
          new Main.GUI.RenameColumnDialog()
          );

        tvctrl.Validator = new ColumnRenameValidator(col,ctrl);
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

    protected class ColumnRenameValidator : Main.GUI.TextValueInputController.NonEmptyStringValidator
    {
      Altaxo.Data.DataColumn m_Col;
      WorksheetController m_Ctrl;
      
      public ColumnRenameValidator(Altaxo.Data.DataColumn col, WorksheetController ctrl)
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


    protected class PropertyColumnRenameValidator : Main.GUI.TextValueInputController.NonEmptyStringValidator
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

  }



  public class SetColumnGroupNumber : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
    {
      if(ctrl.SelectedColumns.Count==0)
        return;

      int grpNumber = ctrl.Doc.DataColumns.GetColumnGroup(ctrl.SelectedColumns[0]);
      Main.GUI.IntegerValueInputController ivictrl = new Main.GUI.IntegerValueInputController(
        grpNumber,
        new Main.GUI.SingleValueDialog("Set group number","Please enter a group number (>=0):")
        );

      ivictrl.Validator = new Altaxo.Main.GUI.IntegerValueInputController.ZeroOrPositiveIntegerValidator();
      if(ivictrl.ShowDialog(ctrl.View.TableViewForm))
      {
        // now set the group number for all selected columns
        for(int i=0;i<ctrl.SelectedColumns.Count;i++)
        {
          int idx = ctrl.SelectedColumns[i];
          ctrl.Doc.DataColumns.SetColumnGroup(idx, ivictrl.EnteredContents);
        }
      }
    }
  }


  public class ExtractPropertyValues : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
    {
      // extract the properties from the (first) selected property column
      if(ctrl.SelectedPropertyColumns.Count==0)
        return;

      Altaxo.Data.DataColumn col = ctrl.DataTable.PropCols[ctrl.SelectedPropertyColumns[0]];

      DataGridOperations.ExtractPropertiesFromColumn(col,ctrl.DataTable.PropCols);

    }
  }

  #endregion

  #region Analysis

  public class AnalysisFFT : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
    {
      DataGridOperations.FFT(ctrl);
    }
  }

  public class Analysis2DFFT : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
    {
      string err = DataGridOperations.TwoDimFFT(Current.Project, ctrl);
      if(null!=err)
        System.Windows.Forms.MessageBox.Show(ctrl.View.TableViewForm,err,"An error occured");
    }
  }

  public class Analysis2DCenteredFFT : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
    {
      string err = DataGridOperations.TwoDimCenteredFFT(Current.Project, ctrl);
      if(null!=err)
        System.Windows.Forms.MessageBox.Show(ctrl.View.TableViewForm,err,"An error occured");
    }
  }

  public class AnalysisStatisticsOnColumns : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
    {
      DataGridOperations.StatisticsOnColumns(Current.Project,ctrl.Doc,ctrl.SelectedColumns,ctrl.SelectedRows);
    }
  }


  public class AnalysisStatisticsOnRows : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
    {
      DataGridOperations.StatisticsOnRows(Current.Project,ctrl.Doc,ctrl.SelectedColumns,ctrl.SelectedRows);
    }
  }

  public class AnalysisMultiplyColumnsToMatrix : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
    {
      string err=DataGridOperations.MultiplyColumnsToMatrix(Current.Project,ctrl.Doc,ctrl.SelectedColumns);
      if(null!=err)
        System.Windows.Forms.MessageBox.Show(ctrl.View.TableViewForm,err,"An error occured");
    }
  }

  public class AnalysisPCAOnRows : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
    {
      int maxFactors = 3;
      Main.GUI.IntegerValueInputController ivictrl = new Main.GUI.IntegerValueInputController(
        maxFactors,
        new Main.GUI.SingleValueDialog("Set maximum number of factors","Please enter the maximum number of factors to calculate:")
        );

      ivictrl.Validator = new Altaxo.Main.GUI.IntegerValueInputController.ZeroOrPositiveIntegerValidator();
      if(ivictrl.ShowDialog(ctrl.View.TableViewForm))
      {
        string err=DataGridOperations.PrincipalComponentAnalysis(Current.Project,ctrl.Doc,ctrl.SelectedColumns,ctrl.SelectedRows,true,ivictrl.EnteredContents);
        if(null!=err)
          System.Windows.Forms.MessageBox.Show(ctrl.View.TableViewForm,err,"An error occured");
      }
    }
  }

  public class AnalysisPCAOnCols : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
    {
      int maxFactors = 3;
      Main.GUI.IntegerValueInputController ivictrl = new Main.GUI.IntegerValueInputController(
        maxFactors,
        new Main.GUI.SingleValueDialog("Set maximum number of factors","Please enter the maximum number of factors to calculate:")
        );

      ivictrl.Validator = new Altaxo.Main.GUI.IntegerValueInputController.ZeroOrPositiveIntegerValidator();
      if(ivictrl.ShowDialog(ctrl.View.TableViewForm))
      {
        string err=DataGridOperations.PrincipalComponentAnalysis(Current.Project,ctrl.Doc,ctrl.SelectedColumns,ctrl.SelectedRows,false,ivictrl.EnteredContents);
        if(null!=err)
          System.Windows.Forms.MessageBox.Show(ctrl.View.TableViewForm,err,"An error occured");
      }
    }
  }
  public class AnalysisPLSOnRows : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
    {
      string err=DataGridOperations.PartialLeastSquaresAnalysis(Current.Project,ctrl.Doc,ctrl.SelectedColumns,ctrl.SelectedRows,ctrl.SelectedPropertyColumns,true);
      if(null!=err)
        System.Windows.Forms.MessageBox.Show(ctrl.View.TableViewForm,err,"An error occured");
    }
  }
  public class AnalysisPLSOnCols : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
    {
      string err=DataGridOperations.PartialLeastSquaresAnalysis(Current.Project,ctrl.Doc,ctrl.SelectedColumns,ctrl.SelectedRows,ctrl.SelectedPropertyColumns,false);
      if(null!=err)
        System.Windows.Forms.MessageBox.Show(ctrl.View.TableViewForm,err,"An error occured");
    }
  }

  #endregion
}
