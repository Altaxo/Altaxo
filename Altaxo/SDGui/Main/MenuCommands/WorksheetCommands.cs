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
  #region Abstract command

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

  #endregion

  #region File commands

  public class SaveAs : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.FileCommands.SaveAs(ctrl,false);
    }
  }

  public class SaveAsTemplate : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.FileCommands.SaveAs(ctrl,true);
    }
  }

  public class ImportAscii : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.FileCommands.ImportAscii(ctrl);
    }
  }

  public class ImportImage : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.FileCommands.ImportImage(ctrl);
    }
  }

  public class ImportGalacticSPC : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.FileCommands.ImportGalacticSPC(ctrl);
    }
  }

  public class ExportAscii : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.FileCommands.ExportAscii(ctrl);
    }
  }

  public class ExportGalacticSPC : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.FileCommands.ExportGalacticSPC(ctrl);
    }
  }

#endregion

  #region Edit commands

  public class EditRemove : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.EditCommands.RemoveSelected(ctrl);
    }
  }
  public class EditCopy : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.EditCommands.CopyToClipboard(ctrl);
    }
  }
  public class EditPaste : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.EditCommands.PasteFromClipboard(ctrl);
    }
  }
  #endregion

  #region Plot commands

  public class PlotLine : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.PlotCommands.PlotLine(ctrl, true, false);
    }
  }

  public class PlotScatter : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.PlotCommands.PlotLine(ctrl, false, true);
    }
  }

  public class PlotLineAndScatter : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.PlotCommands.PlotLine(ctrl, true, true);
    }
  }

  public class PlotDensityImage : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.PlotCommands.PlotDensityImage(ctrl, true, true);
    }
  }


  #endregion

  #region Worksheet

  public class WorksheetRename : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.WorksheetCommands.Rename(ctrl);
    }
  }


  public class WorksheetDuplicate : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.WorksheetCommands.Duplicate(ctrl);
    }
  }

  public class WorksheetTranspose : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.WorksheetCommands.Transpose(ctrl);
    }
  }

  public class AddDataColumns : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.WorksheetCommands.AddDataColumns(ctrl);
    }
  }

  public class AddPropertyColumns : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.WorksheetCommands.AddPropertyColumns(ctrl);
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
      Altaxo.Worksheet.Commands.ColumnCommands.SetColumnValues(ctrl);
    }
  }

  public class SetColumnAsX : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.ColumnCommands.SetSelectedColumnAsX(ctrl);
    }
  }

  public class SetColumnAsLabel : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.ColumnCommands.SetSelectedColumnAsLabel(ctrl);
    }
  }

  public class SetColumnAsValue : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.ColumnCommands.SetSelectedColumnAsValue(ctrl);
    }
  }


  public class RenameColumn : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.ColumnCommands.RenameSelectedColumn(ctrl);
    }
  }

  

  public class SetColumnGroupNumber : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
    {
       Altaxo.Worksheet.Commands.ColumnCommands.SetSelectedColumnGroupNumber(ctrl);
    }
  }


  public class ExtractPropertyValues : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.ColumnCommands.ExtractPropertyValues(ctrl);
    }
  }

  #endregion

  #region Analysis

  public class AnalysisFFT : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.Analysis.FourierCommands.FFT(ctrl);
    }
  }

  public class Analysis2DFFT : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.Analysis.FourierCommands.TwoDimensionalFFT(ctrl);
    }
  }

  public class Analysis2DCenteredFFT : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.Analysis.FourierCommands.TwoDimensionalCenteredFFT(ctrl);
    }
  }

  public class AnalysisStatisticsOnColumns : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.Analysis.StatisticCommands.StatisticsOnColumns(ctrl);
    }
  }


  public class AnalysisStatisticsOnRows : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.Analysis.StatisticCommands.StatisticsOnRows(ctrl);
    }
  }

  public class AnalysisMultiplyColumnsToMatrix : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.Analysis.ChemometricCommands.MultiplyColumnsToMatrix(ctrl);
    }
  }

  public class AnalysisPCAOnRows : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.Analysis.ChemometricCommands.PCAOnRows(ctrl);
    }
  }
  public class AnalysisPCAOnCols : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.Analysis.ChemometricCommands.PLSOnColumns(ctrl);
    }
  }
  public class AnalysisPLSOnRows : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.Analysis.ChemometricCommands.PLSOnRows(ctrl);
    }
  }
  public class AnalysisPLSOnCols : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.Analysis.ChemometricCommands.PLSOnColumns(ctrl);
    }
  }

  #endregion
}
