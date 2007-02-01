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
//    along with this program; if not, write to the Free Software
//    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
//
/////////////////////////////////////////////////////////////////////////////
#endregion

using System;
using System.Windows.Forms;
using Altaxo;
using Altaxo.Main;
using Altaxo.Graph;
using Altaxo.Graph.Gdi;
using Altaxo.Graph.Gdi.Plot;
using Altaxo.Graph.Gdi.Plot.Styles;
using Altaxo.Graph.Gdi.Plot.Data;
using Altaxo.Graph.Plot.Data;
using Altaxo.Graph.GUI;
using Altaxo.Gui.Graph;
using Altaxo.Gui;
using Altaxo.Gui.Common;
using Altaxo.Gui.Scripting;
using Altaxo.Scripting;
using ICSharpCode.SharpZipLib.Zip;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using ICSharpCode.Core;

namespace Altaxo.Graph.Commands
{

  /// <summary>
  /// Provides a abstract class for issuing commands that apply to worksheet controllers.
  /// </summary>
  public abstract class AbstractGraphControllerCommand : AbstractMenuCommand
  {
    /// <summary>
    /// Determines the currently active worksheet and issues the command to that worksheet by calling
    /// Run with the worksheet as a parameter.
    /// </summary>
    public override void Run()
    {
      Altaxo.Gui.SharpDevelop.SDGraphViewContent ctrl 
        = Current.Workbench.ActiveViewContent
        as Altaxo.Gui.SharpDevelop.SDGraphViewContent;
      
      if(null!=ctrl)
        Run(ctrl.Controller);
    }
  
    /// <summary>
    /// Override this function for adding own worksheet commands. You will get
    /// the worksheet controller in the parameter.
    /// </summary>
    /// <param name="ctrl">The worksheet controller this command is applied to.</param>
    public abstract void Run(Altaxo.Graph.GUI.GraphController ctrl);
  }


  /// <summary>
  /// Handler for the menu item "File" - "Setup Page".
  /// </summary>
  public class PageSetup : AbstractGraphControllerCommand
  {
    public override void Run(Altaxo.Graph.GUI.GraphController ctrl)
    {
      try
      {
        if(Current.Gui.ShowPageSetupDialog())
        {
          ctrl.Doc.SetGraphPageBoundsToPrinterSettings();
        }
      }
      catch(Exception exc)
      {
        MessageBox.Show(ctrl.View.Form, exc.ToString(),"Exception occured!");
      }   
    }
  }


  /// <summary>
  /// Handler for the menu item "File" - "Print".
  /// </summary>
  public class Print : AbstractGraphControllerCommand
  {
    public override void Run(Altaxo.Graph.GUI.GraphController ctrl)
    {
      try
      {
        if(Current.Gui.ShowPrintDialog())
        {
          Current.PrintingService.PrintDocument.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(ctrl.EhPrintPage);
          Current.PrintingService.PrintDocument.Print();
        }
      }
      catch(Exception ex)
      {
        System.Windows.Forms.MessageBox.Show(ctrl.View.Form,ex.ToString());
      }
      finally
      {
        Current.PrintingService.PrintDocument.PrintPage -= new System.Drawing.Printing.PrintPageEventHandler(ctrl.EhPrintPage);
      }
    }
  }

  /// <summary>
  /// Handler for the menu item "File" - "Print Preview".
  /// </summary>
  public class PrintPreview : AbstractGraphControllerCommand
  {
    public override void Run(Altaxo.Graph.GUI.GraphController ctrl)
    {
      try
      {
        System.Windows.Forms.PrintPreviewDialog dlg = new System.Windows.Forms.PrintPreviewDialog();
        Current.PrintingService.PrintDocument.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(ctrl.EhPrintPage);
        dlg.Document = Current.PrintingService.PrintDocument;
        dlg.ShowDialog(ctrl.View.Form);
        dlg.Dispose();
      }
      catch(Exception ex)
      {
        System.Windows.Forms.MessageBox.Show(ctrl.View.Form,ex.ToString());
      }
      finally
      {
        Current.PrintingService.PrintDocument.PrintPage -= new System.Drawing.Printing.PrintPageEventHandler(ctrl.EhPrintPage);
      }
    }
  }


  /// <summary>
  /// Handler for the menu item "Edit" - "CopyPage".
  /// </summary>
  public class CopyPage : AbstractGraphControllerCommand
  {
    public override void Run(Altaxo.Graph.GUI.GraphController ctrl)
    {
      Altaxo.Graph.Procedures.GraphCommands.CopyPageToClipboard(ctrl);
    }
  }

  /// <summary>
  /// Handler for the menu item "Edit" - "CopyPageAsBitmap".
  /// The resulting bitmap has a resolution of 150 dpi and ARGB format.
  /// </summary>
  public class CopyPageAsBitmap150dpiARGB : AbstractGraphControllerCommand
  {
    public override void Run(Altaxo.Graph.GUI.GraphController ctrl)
    {
      Altaxo.Graph.Procedures.GraphCommands.CopyPageToClipboardAsBitmap(ctrl, 150, null, PixelFormat.Format32bppArgb);
    }
  }

  /// <summary>
  /// Handler for the menu item "Edit" - "CopyPageAsBitmap".
  /// The resulting bitmap has a resolution of 150 dpi and RGB format.
  /// </summary>
  public class CopyPageAsBitmap150dpiRGB : AbstractGraphControllerCommand
  {
    public override void Run(Altaxo.Graph.GUI.GraphController ctrl)
    {
      Altaxo.Graph.Procedures.GraphCommands.CopyPageToClipboardAsBitmap(ctrl, 150, Brushes.White, PixelFormat.Format24bppRgb);
    }
  }


  /// <summary>
  /// Handler for the menu item "Edit" - "CopyPageAsBitmap".
  /// The resulting bitmap has a resolution of 300 dpi and ARGB format.
  /// </summary>
  public class CopyPageAsBitmap300dpiARGB : AbstractGraphControllerCommand
  {
    public override void Run(Altaxo.Graph.GUI.GraphController ctrl)
    {
      Altaxo.Graph.Procedures.GraphCommands.CopyPageToClipboardAsBitmap(ctrl,300,null,PixelFormat.Format32bppArgb);
    }
  }

  /// <summary>
  /// Handler for the menu item "Edit" - "CopyPageAsBitmap".
  /// The resulting bitmap has a resolution of 300 dpi and RGB format.
  /// </summary>
  public class CopyPageAsBitmap300dpiRGB : AbstractGraphControllerCommand
  {
    public override void Run(Altaxo.Graph.GUI.GraphController ctrl)
    {
      Altaxo.Graph.Procedures.GraphCommands.CopyPageToClipboardAsBitmap(ctrl, 300, Brushes.White, PixelFormat.Format24bppRgb);
    }
  }


  public class SaveGraphAsTemplate : AbstractGraphControllerCommand
  {
    public override void Run(Altaxo.Graph.GUI.GraphController ctrl)
    {
      System.IO.Stream myStream ;
      SaveFileDialog saveFileDialog1 = new SaveFileDialog();
 
      saveFileDialog1.Filter = "Altaxo graph files (*.axogrp)|*.axogrp|All files (*.*)|*.*"  ;
      saveFileDialog1.FilterIndex = 1 ;
      saveFileDialog1.RestoreDirectory = true ;
 
      if(saveFileDialog1.ShowDialog() == DialogResult.OK)
      {
        if((myStream = saveFileDialog1.OpenFile()) != null)
        {
          Altaxo.Serialization.Xml.XmlStreamSerializationInfo info = new Altaxo.Serialization.Xml.XmlStreamSerializationInfo();
          info.BeginWriting(myStream);
          info.AddValue("Graph",ctrl.Doc);
          info.EndWriting();
          myStream.Close();
        }
      }
    }
  }

  /// <summary>
  /// Handler for the menu item "File" - "Export Metafile".
  /// </summary>
  public class FileExportMetafile : AbstractGraphControllerCommand
  {
    public override void Run(Altaxo.Graph.GUI.GraphController ctrl)
    {
      System.IO.Stream myStream ;
      SaveFileDialog saveFileDialog1 = new SaveFileDialog();
 
      saveFileDialog1.Filter = "Windows Metafiles (*.emf)|*.emf|All files (*.*)|*.*"  ;
      saveFileDialog1.FilterIndex = 1 ;
      saveFileDialog1.RestoreDirectory = true ;
 
      if(saveFileDialog1.ShowDialog() == DialogResult.OK)
      {
        if((myStream = saveFileDialog1.OpenFile()) != null)
        {
          ctrl.SaveAsMetafile(myStream);
          myStream.Close();
        } // end openfile ok
      } // end dlgresult ok
    }
  }


  /// <summary>
  /// Handler for the menu item "File" - "Export Metafile".
  /// </summary>
  public class FileExportTiff : AbstractGraphControllerCommand
  {
    public override void Run(Altaxo.Graph.GUI.GraphController ctrl)
    {
      System.IO.Stream myStream;
      SaveFileDialog saveFileDialog1 = new SaveFileDialog();

      saveFileDialog1.Filter = "Tiff files (*.tif)|*.tif|All files (*.*)|*.*";
      saveFileDialog1.FilterIndex = 1;
      saveFileDialog1.RestoreDirectory = true;

      if (saveFileDialog1.ShowDialog() == DialogResult.OK)
      {
        if ((myStream = saveFileDialog1.OpenFile()) != null)
        {
          ctrl.SaveAsBitmap(myStream,300,System.Drawing.Imaging.ImageFormat.Tiff);
          myStream.Close();
        } // end openfile ok
      } // end dlgresult ok
    }
  }
  /// <summary>
  /// Handler for the menu item "Edit" - "New layer(axes)" - "Normal: Bottom X Left Y".
  /// </summary>
  public class NewLayerNormalBottomXLeftY : AbstractGraphControllerCommand
  {
    public override void Run(Altaxo.Graph.GUI.GraphController ctrl)
    {
      ctrl.Doc.CreateNewLayerNormalBottomXLeftY();
  
    }
  }

  /// <summary>
  /// Handler for the menu item "Edit" - "New layer(axes)" - "Linked: Top X Right Y".
  /// </summary>
  public class NewLayerLinkedTopXRightY : AbstractGraphControllerCommand
  {
    public override void Run(Altaxo.Graph.GUI.GraphController ctrl)
    {
      ctrl.Doc.CreateNewLayerLinkedTopXRightY(ctrl.CurrentLayerNumber);

    }
  }
  
  /// <summary>
  /// Handler for the menu item "Edit" - "New layer(axes)" - "Linked: Top X".
  /// </summary>
  public class NewLayerLinkedTopX : AbstractGraphControllerCommand
  {
    public override void Run(Altaxo.Graph.GUI.GraphController ctrl)
    {
      ctrl.Doc.CreateNewLayerLinkedTopX(ctrl.CurrentLayerNumber);
    }
  }

  /// <summary>
  /// Handler for the menu item "Edit" - "New layer(axes)" - "Linked: Right Y".
  /// </summary>
  public class NewLayerLinkedRightY : AbstractGraphControllerCommand
  {
    public override void Run(Altaxo.Graph.GUI.GraphController ctrl)
    {
      ctrl.Doc.CreateNewLayerLinkedRightY(ctrl.CurrentLayerNumber);
    }
  }

  /// <summary>
  /// Handler for the menu item "Edit" - "New layer(axes)" - "Linked: Top X Right Y, X axis straight ".
  /// </summary>
  public class NewLayerLinkedTopXRightY_XAxisStraight : AbstractGraphControllerCommand
  {
    public override void Run(Altaxo.Graph.GUI.GraphController ctrl)
    {
      ctrl.Doc.CreateNewLayerLinkedTopXRightY_XAxisStraight(ctrl.CurrentLayerNumber);
    }
  }

  public class GraphRename : AbstractGraphControllerCommand
  {
    public override void Run(Altaxo.Graph.GUI.GraphController ctrl)
    {
      Altaxo.Graph.Procedures.GraphCommands.Rename(ctrl);
    }
  }

  public class GraphRefresh : AbstractGraphControllerCommand
  {
    public override void Run(Altaxo.Graph.GUI.GraphController ctrl)
    {
      Altaxo.Graph.Procedures.GraphCommands.Refresh(ctrl);
    }
  }



  public class ArrangeLayers : AbstractGraphControllerCommand
  {
    public override void Run(GraphController ctrl)
    {
      Altaxo.Graph.Procedures.ArrangeLayersDocument.ArrangeLayers(ctrl.Doc);
    }
  }

  public class ArrangeTop : AbstractGraphControllerCommand
  {
    public override void Run(GraphController ctrl)
    {
      ctrl.ArrangeTop();
    }
  }
  public class ArrangeBottom : AbstractGraphControllerCommand
  {
    public override void Run(GraphController ctrl)
    {
      ctrl.ArrangeBottom();
    }
  }
  public class ArrangeLeft : AbstractGraphControllerCommand
  {
    public override void Run(GraphController ctrl)
    {
      ctrl.ArrangeLeft();
    }
  }
  public class ArrangeRight : AbstractGraphControllerCommand
  {
    public override void Run(GraphController ctrl)
    {
      ctrl.ArrangeRight();
    }
  }
  public class ArrangeHorizontal : AbstractGraphControllerCommand
  {
    public override void Run(GraphController ctrl)
    {
      ctrl.ArrangeHorizontal();
    }
  }
  public class ArrangeVertical : AbstractGraphControllerCommand
  {
    public override void Run(GraphController ctrl)
    {
      ctrl.ArrangeVertical();
    }
  }
  public class ArrangeHorizontalTable : AbstractGraphControllerCommand
  {
    public override void Run(GraphController ctrl)
    {
      ctrl.ArrangeHorizontalTable();
    }
  }
  public class ArrangeVerticalTable : AbstractGraphControllerCommand
  {
    public override void Run(GraphController ctrl)
    {
      ctrl.ArrangeVerticalTable();
    }
  }


  /// <summary>
  /// Duplicates the Graph and the Graph view to a new one.
  /// </summary>
  public class DuplicateGraph : AbstractGraphControllerCommand
  {
    public override void Run(Altaxo.Graph.GUI.GraphController ctrl)
    {
      GraphDocument newDoc = new GraphDocument(ctrl.Doc);
      Current.Project.GraphDocumentCollection.Add(newDoc);
      Current.ProjectService.CreateNewGraph(newDoc);
    }
  }

  public class LayerControl : AbstractGraphControllerCommand
  {
    public override void Run(Altaxo.Graph.GUI.GraphController ctrl)
    {
      ctrl.EnsureValidityOfCurrentLayerNumber();
      if(null!=ctrl.ActiveLayer)
        LayerController.ShowDialog(ctrl.ActiveLayer);
    }
  }

  public class AddCurvePlot : AbstractGraphControllerCommand
  {
    public override void Run(Altaxo.Graph.GUI.GraphController ctrl)
    {
      ctrl.EnsureValidityOfCurrentLayerNumber();
      ctrl.Doc.Layers[ctrl.CurrentLayerNumber].PlotItems.Add(new XYFunctionPlotItem(new XYFunctionPlotData(new PolynomialFunction(new double[]{0,0,1})),new G2DPlotStyleCollection(LineScatterPlotStyleKind.Line)));
    }
  }

  /// <summary>
  /// Handler for the menu item "Graph" - "New layer legend.
  /// </summary>
  public class NewLayerLegend : AbstractGraphControllerCommand
  {
    public override void Run(Altaxo.Graph.GUI.GraphController ctrl)
    {
      ctrl.Doc.Layers[ctrl.CurrentLayerNumber].CreateNewLayerLegend();
    }
  }

  /// <summary>
  /// Handler for the toolbar item Rescale axes.
  /// </summary>
  public class RescaleAxes : AbstractGraphControllerCommand
  {
    public override void Run(Altaxo.Graph.GUI.GraphController ctrl)
    {
      ctrl.Doc.Layers[ctrl.CurrentLayerNumber].RescaleXAxis();
      ctrl.Doc.Layers[ctrl.CurrentLayerNumber].RescaleYAxis();
    }
  }

  /// <summary>
  /// Handler for the menu item "Graph" - "New layer legend.
  /// </summary>
  public class FitPolynomial : AbstractGraphControllerCommand
  {
    public override void Run(Altaxo.Graph.GUI.GraphController ctrl)
    {
      FitPolynomialDialogController dlg = new FitPolynomialDialogController(2,double.NegativeInfinity,double.PositiveInfinity,false);
      if (Current.Gui.ShowDialog(dlg, "Polynomial fit", false))
      {
        Altaxo.Graph.Procedures.PolynomialFitting.Fit(ctrl, dlg.Order, dlg.FitCurveXmin, dlg.FitCurveXmax, dlg.ShowFormulaOnGraph);
      }

      /*
      if(DialogFactory.ShowPolynomialFitDialog(Current.MainWindow,dlg))
      {
        Altaxo.Graph.Procedures.PolynomialFitting.Fit(ctrl,dlg.Order,dlg.FitCurveXmin,dlg.FitCurveXmax,dlg.ShowFormulaOnGraph);
      }
      */
    }
  }

  /// <summary>
  /// Handler for the menu item "Graph" - "New layer legend.
  /// </summary>
  public class FitNonlinear : AbstractGraphControllerCommand
  {
    public override void Run(Altaxo.Graph.GUI.GraphController ctrl)
    {
      string result = Altaxo.Graph.Procedures.NonlinearFitting.Fit(ctrl);
      if(null!=result)
        Current.Gui.ErrorMessageBox(result);
    }
  }

  public class NewUserFunction : AbstractGraphControllerCommand
  {
    public override void Run(Altaxo.Graph.GUI.GraphController ctrl)
    {
      FunctionEvaluationScript script = null; // 

      if(script==null)
        script = new FunctionEvaluationScript();

      object[] args = new object[]{script,new ScriptExecutionHandler(this.EhScriptExecution)};
      if(Current.Gui.ShowDialog(args, "Function script"))
      {
        ctrl.EnsureValidityOfCurrentLayerNumber();

        script = (FunctionEvaluationScript)args[0];
        XYFunctionPlotItem functItem = new XYFunctionPlotItem(new XYFunctionPlotData(script),new G2DPlotStyleCollection(LineScatterPlotStyleKind.Line));
        ctrl.Doc.Layers[ctrl.CurrentLayerNumber].PlotItems.Add(functItem);
      }
    }

    public bool EhScriptExecution(IScriptText script)
    {
      return true;
    }
  }




  /// <summary>
  /// Provides a abstract class for issuing commands that apply to worksheet controllers.
  /// </summary>
  public abstract class AbstractCheckableGraphControllerCommand : AbstractCheckableMenuCommand
  {
    public Altaxo.Graph.GUI.GraphController Controller
    {
      get 
      {
        if (null != Current.Workbench && null != Current.Workbench.ActiveViewContent)
        {
          Altaxo.Gui.SharpDevelop.SDGraphViewContent ct = Current.Workbench.ActiveViewContent as Altaxo.Gui.SharpDevelop.SDGraphViewContent;
          return ct==null ? null : ct.Controller;
        }
        else
          return null;
      }
    }

    /// <summary>
    /// This function is never be called, since this is a CheckableMenuCommand.
    /// </summary>
    public override void Run()
    {
      base.Run();
    }
  }

  /// <summary>
  /// This class is intented to be used for commands into the graph tools toolbar. Commands derived
  /// from it will update the toolbar whenever its state changed.
  /// </summary>
  public abstract class AbstractGraphToolsCommand : AbstractCheckableGraphControllerCommand
  {
    Altaxo.Graph.GUI.GraphController myCurrentGraphController;

    protected AbstractGraphToolsCommand()
    {
      if(null!=Current.Workbench)
      {
        Current.Workbench.ActiveWorkbenchWindowChanged += new EventHandler(this.EhWorkbenchContentChanged);
        this.EhWorkbenchContentChanged(this,EventArgs.Empty);
      }
    }

    protected void EhWorkbenchContentChanged(object o, System.EventArgs e)
    {
      if(!object.ReferenceEquals(Controller,myCurrentGraphController))
      {
        if(null!=myCurrentGraphController)
        {
          lock(this)
          {
            this.myCurrentGraphController.CurrentGraphToolChanged -= new EventHandler(this.EhGraphToolChanged);
            this.myCurrentGraphController = null;
          }
        }
        if(Controller!=null)
        {
          lock(this)
          {
            this.myCurrentGraphController = this.Controller;
            this.myCurrentGraphController.CurrentGraphToolChanged += new EventHandler(this.EhGraphToolChanged);
          }
        }
      }
    }

    protected void EhGraphToolChanged(object o, EventArgs e)
    {
      bool bBaseChecked = base.IsChecked;
      bool bThisChecked = this.IsChecked; // only to retrieve the checked state
      if(bBaseChecked!=bThisChecked) // to prevent that every button "updates" the toolbar
      {
        //ICSharpCode.SharpDevelop.Gui.Workbench1 wb = Current.Workbench as ICSharpCode.SharpDevelop.Gui.Workbench1;
        //wb.UpdateToolbars();      
      }
    }

  }

  /// <summary>
  /// Test class for a selected item
  /// </summary>
  public class SelectPointerTool : AbstractGraphToolsCommand
  {
    public override bool IsChecked 
    {
      get 
      {
        if(null!=Controller)
        {
          base.IsChecked = (Controller.CurrentGraphToolType==typeof(GUI.GraphControllerMouseHandlers.ObjectPointerMouseHandler));
        }

        return base.IsChecked;
      }
      set 
      {
        base.IsChecked = value;
        if(true==value && null!=Controller)
        {
          Controller.CurrentGraphToolType=typeof(GUI.GraphControllerMouseHandlers.ObjectPointerMouseHandler);
        }

       // ((ICSharpCode.SharpDevelop.Gui.DefaultWorkbench)ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.Workbench).UpdateToolbars();

      }
    }
  }

  /// <summary>
  /// Test class for a selected item
  /// </summary>
  public class SelectTextTool : AbstractGraphToolsCommand
  {
    public override bool IsChecked 
    {
      get 
      {
        if(null!=Controller)
        {
          base.IsChecked = (Controller.CurrentGraphToolType==typeof(GUI.GraphControllerMouseHandlers.TextToolMouseHandler));
        }

        return base.IsChecked;
      }
      set 
      {
        base.IsChecked = value;
        if(true==value && null!=Controller)
        {
          Controller.CurrentGraphToolType=typeof(GUI.GraphControllerMouseHandlers.TextToolMouseHandler);
        }

        //((ICSharpCode.SharpDevelop.Gui.DefaultWorkbench)ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.Workbench).UpdateToolbars();

      }
    }
  }


  /// <summary>
  /// Tool for reading the x-y scatter values of a data point.
  /// </summary>
  public class ReadPlotItemDataTool : AbstractGraphToolsCommand
  {
    public override bool IsChecked 
    {
      get 
      {
        if(null!=Controller)
        {
          base.IsChecked = (Controller.CurrentGraphToolType==typeof(GUI.GraphControllerMouseHandlers.ReadPlotItemDataMouseHandler));
        }

        return base.IsChecked;
      }
      set 
      {
        base.IsChecked = value;
        if(true==value && null!=Controller)
        {
          Controller.CurrentGraphToolType=typeof(GUI.GraphControllerMouseHandlers.ReadPlotItemDataMouseHandler);
        }

        //((ICSharpCode.SharpDevelop.Gui.DefaultWorkbench)ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.Workbench).UpdateToolbars();

      }
    }
  }

   

  /// <summary>
  /// Tool for reading the x-y coordinate values of a layer.
  /// </summary>
  public class ReadXYCoordinatesTool : AbstractGraphToolsCommand
  {
    public override bool IsChecked 
    {
      get 
      {
        if(null!=Controller)
        {
          base.IsChecked = (Controller.CurrentGraphToolType==typeof(GUI.GraphControllerMouseHandlers.ReadXYCoordinatesMouseHandler));
        }

        return base.IsChecked;
      }
      set 
      {
        base.IsChecked = value;
        if(true==value && null!=Controller)
        {
          Controller.CurrentGraphToolType=typeof(GUI.GraphControllerMouseHandlers.ReadXYCoordinatesMouseHandler);
        }

        //((ICSharpCode.SharpDevelop.Gui.DefaultWorkbench)ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.Workbench).UpdateToolbars();

      }
    }
  }
  /// <summary>
  /// Drawing a simple line with two points.
  /// </summary>
  public class SingleLineDrawingTool : AbstractGraphToolsCommand
  {
    public override bool IsChecked 
    {
      get 
      {
        if(null!=Controller)
        {
          base.IsChecked = (Controller.CurrentGraphToolType==typeof(GUI.GraphControllerMouseHandlers.SingleLineDrawingMouseHandler));
        }

        return base.IsChecked;
      }
      set 
      {
        base.IsChecked = value;
        if(true==value && null!=Controller)
        {
          Controller.CurrentGraphToolType=typeof(GUI.GraphControllerMouseHandlers.SingleLineDrawingMouseHandler);
        }

       // ((ICSharpCode.SharpDevelop.Gui.DefaultWorkbench)ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.Workbench).UpdateToolbars();

      }
    }
  }


  /// <summary>
  /// Drawing a simple line with two points.
  /// </summary>
  public class ArrowLineDrawingTool : AbstractGraphToolsCommand
  {
    public override bool IsChecked 
    {
      get 
      {
        if(null!=Controller)
        {
          base.IsChecked = (Controller.CurrentGraphToolType==typeof(GUI.GraphControllerMouseHandlers.ArrowLineDrawingMouseHandler));
        }

        return base.IsChecked;
      }
      set 
      {
        base.IsChecked = value;
        if(true==value && null!=Controller)
        {
          Controller.CurrentGraphToolType=typeof(GUI.GraphControllerMouseHandlers.ArrowLineDrawingMouseHandler);
        }

        //((ICSharpCode.SharpDevelop.Gui.DefaultWorkbench)ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.Workbench).UpdateToolbars();

      }
    }
  }



  /// <summary>
  /// Drawing a rectangle on the graph.
  /// </summary>
  public class RectangleDrawingTool : AbstractGraphToolsCommand
  {
    public override bool IsChecked 
    {
      get 
      {
        if(null!=Controller)
        {
          base.IsChecked = (Controller.CurrentGraphToolType==typeof(GUI.GraphControllerMouseHandlers.RectangleDrawingMouseHandler));
        }

        return base.IsChecked;
      }
      set 
      {
        base.IsChecked = value;
        if(true==value && null!=Controller)
        {
          Controller.CurrentGraphToolType=typeof(GUI.GraphControllerMouseHandlers.RectangleDrawingMouseHandler);
        }

       // ((ICSharpCode.SharpDevelop.Gui.DefaultWorkbench)ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.Workbench).UpdateToolbars();

      }
    }
  }
  

  /// <summary>
  /// Drawing an ellipse on the graph.
  /// </summary>
  public class EllipseDrawingTool : AbstractGraphToolsCommand
  {
    public override bool IsChecked 
    {
      get 
      {
        if(null!=Controller)
        {
          base.IsChecked = (Controller.CurrentGraphToolType==typeof(GUI.GraphControllerMouseHandlers.EllipseDrawingMouseHandler));
        }

        return base.IsChecked;
      }
      set 
      {
        base.IsChecked = value;
        if(true==value && null!=Controller)
        {
          Controller.CurrentGraphToolType=typeof(GUI.GraphControllerMouseHandlers.EllipseDrawingMouseHandler);
        }

       // ((ICSharpCode.SharpDevelop.Gui.DefaultWorkbench)ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.Workbench).UpdateToolbars();

      }
    }
  }
  /// <summary>
  /// Magnifies the axes according to the selected area.
  /// </summary>
  public class ZoomAxesTool : AbstractGraphToolsCommand
  {
    public override bool IsChecked 
    {
      get 
      {
        if(null!=Controller)
        {
          base.IsChecked = (Controller.CurrentGraphToolType==typeof(Altaxo.Graph.GUI.GraphControllerMouseHandlers.ZoomAxesMouseHandler));
        }

        return base.IsChecked;
      }
      set 
      {
        base.IsChecked = value;
        if(true==value && null!=Controller)
        {
          Controller.CurrentGraphToolType=typeof(Altaxo.Graph.GUI.GraphControllerMouseHandlers.ZoomAxesMouseHandler);
          
          Cursor cursor = ResourceService.GetCursor("Cursors.32x32.ZoomAxes");
          this.Controller.View.SetPanelCursor(cursor);
        }

        //((ICSharpCode.SharpDevelop.Gui.DefaultWorkbench)ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.Workbench).UpdateToolbars();

      }
    }
  }



}
