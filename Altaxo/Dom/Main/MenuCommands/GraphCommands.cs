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
//    along with this program; if not, write to the Free Software
//    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
//
/////////////////////////////////////////////////////////////////////////////

#endregion Copyright

#nullable disable warnings
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using Altaxo.Collections;
using Altaxo.Drawing;
using Altaxo.Geometry;
using Altaxo.Graph.Gdi;
using Altaxo.Graph.Gdi.Plot;
using Altaxo.Graph.Gdi.Plot.Styles;
using Altaxo.Graph.Plot.Data;
using Altaxo.Gui;
using Altaxo.Gui.Common;
using Altaxo.Gui.Graph;
using Altaxo.Gui.Graph.Gdi;
using Altaxo.Gui.Graph.Gdi.Viewing;
using Altaxo.Gui.Scripting;
using Altaxo.Gui.Workbench;
using Altaxo.Main;
using Altaxo.Main.Services;
using Altaxo.Scripting;

namespace Altaxo.Graph.Commands
{
  /// <summary>
  /// Provides an abstract base class for commands that apply to graph controllers.
  /// </summary>
  public abstract class AbstractGraphControllerCommand : SimpleCommand
  {
    /// <summary>Determines if the command can be executed.</summary>
    /// <param name="parameter">The parameter (context of the command).</param>
    /// <returns>True if either the <paramref name="parameter"/> or the ActiveViewContent of the workbench is a <see cref="Altaxo.Gui.Graph.Gdi.Viewing.GraphController"/>.
    /// </returns>
    public override bool CanExecute(object parameter)
    {
      if (!(parameter is IViewContent viewContent))
        viewContent = Current.Workbench.ActiveViewContent;
      return viewContent is Altaxo.Gui.Graph.Gdi.Viewing.GraphController;
    }

    /// <summary>
    /// Determines the currently active graph controller and issues the command to that controller by calling
    /// <see cref="Run"/> with the controller as a parameter.
    /// </summary>
    /// <param name="parameter">The command parameter that may contain the active view context.</param>
    public override void Execute(object parameter)
    {
      if (!(parameter is IViewContent viewContent))
        viewContent = Current.Workbench.ActiveViewContent;

      if (viewContent is Altaxo.Gui.Graph.Gdi.Viewing.GraphController ctrl)
        Run(ctrl);
    }

    /// <summary>
    /// Override this function to implement custom graph commands.
    /// </summary>
    /// <param name="ctrl">The graph controller this command is applied to.</param>
    public abstract void Run(GraphController ctrl);
  }

  /// <summary>
  /// Handler for the menu item "File" - "Print".
  /// </summary>
  public class Print : AbstractGraphControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(GraphController ctrl)
    {
      ctrl.Doc.ShowPrintDialogAndPrint();
    }
  }

  /// <summary>
  /// Handler for the menu item "File" - "Print options".
  /// </summary>
  public class PrintOptionsSetup : AbstractGraphControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(GraphController ctrl)
    {
      ctrl.Doc.ShowPrintOptionsDialog();
    }
  }

  /// <summary>
  /// Handler for the menu item "Edit" - "CopyPage".
  /// </summary>
  public class SetCopyPageOptions : AbstractGraphControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(GraphController ctrl)
    {
      ctrl.Doc.ShowCopyPageOptionsDialog();
    }
  }

  /// <summary>
  /// Handler for the menu item "Graph - Resize Graph".
  /// </summary>
  public class ResizeGraph : AbstractGraphControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(GraphController ctrl)
    {
      Altaxo.Gui.Graph.Graph2D.ResizeGraphController.ShowResizeGraphDialog(ctrl.Doc);
    }
  }

  /// <summary>
  /// Handler for the menu item "Edit" - "CopyPage".
  /// </summary>
  public class CopyPage : AbstractGraphControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(GraphController ctrl)
    {
      //ctrl.Doc.CopyToClipboardAsImage();
    }
  }

  /// <summary>
  /// Handler for the menu item "Edit" - "CopyPageAsBitmap".
  /// The resulting bitmap has a resolution of 150 dpi and ARGB format.
  /// </summary>
  public class CopyPageAsBitmap150dpiARGB : AbstractGraphControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(GraphController ctrl)
    {
      ctrl.Doc.CopyToClipboardAsBitmap(150, null, PixelFormat.Format32bppArgb);
    }
  }

  /// <summary>
  /// Handler for the menu item "Edit" - "CopyPageAsBitmap".
  /// The resulting bitmap has a resolution of 150 dpi and RGB format.
  /// </summary>
  public class CopyPageAsBitmap150dpiRGB : AbstractGraphControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(GraphController ctrl)
    {
      var brush = new BrushX(NamedColors.White);
      ctrl.Doc.CopyToClipboardAsBitmap(150, brush, PixelFormat.Format24bppRgb);
    }
  }

  /// <summary>
  /// Handler for the menu item "Edit" - "CopyPageAsBitmap".
  /// The resulting bitmap has a resolution of 300 dpi and ARGB format.
  /// </summary>
  public class CopyPageAsBitmap300dpiARGB : AbstractGraphControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(GraphController ctrl)
    {
      ctrl.Doc.CopyToClipboardAsBitmap(300, null, PixelFormat.Format32bppArgb);
    }
  }

  /// <summary>
  /// Handler for the menu item "Edit" - "CopyPageAsBitmap".
  /// The resulting bitmap has a resolution of 300 dpi and RGB format.
  /// </summary>
  public class CopyPageAsBitmap300dpiRGB : AbstractGraphControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(GraphController ctrl)
    {
      var brush = new BrushX(NamedColors.White);
      ctrl.Doc.CopyToClipboardAsBitmap(300, brush, PixelFormat.Format24bppRgb);
    }
  }

  /// <summary>
  /// Saves the active graph as a reusable graph template.
  /// </summary>
  public class SaveGraphAsTemplate : AbstractGraphControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(GraphController ctrl)
    {
      var saveFileDialog1 = new Altaxo.Gui.SaveFileOptions
      {
        FilterIndex = 1,
        RestoreDirectory = true
      };
      saveFileDialog1.AddFilter("*.axogrp", "Altaxo graph files (*.axogrp)");
      saveFileDialog1.AddFilter("*.*", "All files (*.*)");

      if (true == Current.Gui.ShowSaveFileDialog(saveFileDialog1))
      {
        using var myStream = new System.IO.FileStream(saveFileDialog1.FileName, System.IO.FileMode.Create, System.IO.FileAccess.ReadWrite, System.IO.FileShare.Read);
        var info = new Altaxo.Serialization.Xml.XmlStreamSerializationInfo();
        info.BeginWriting(myStream);
        info.AddValue("Graph", ctrl.Doc);
        info.EndWriting();
        myStream.Close();
      }
    }
  }

  /// <summary>
  /// Handler for the menu item "File" - "Export Metafile".
  /// </summary>
  public class FileExportMetafile : AbstractGraphControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(GraphController ctrl)
    {
      ctrl.Doc.ShowFileExportMetafileDialog();
    }
  }

  /// <summary>
  /// Handler for the menu item "File" - "Export Metafile".
  /// </summary>
  public class FileExportTiff : AbstractGraphControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(GraphController ctrl)
    {
      ctrl.Doc.ShowFileExportTiffDialog();
    }
  }

  /// <summary>
  /// Opens the export dialog for a specific graph export binding.
  /// </summary>
  public class FileExportSpecific : AbstractGraphControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(GraphController ctrl)
    {
      ctrl.Doc.ShowFileExportSpecificDialog();
    }
  }

  /// <summary>
  /// Handler for the menu item "Edit" - "New layer(axes)" - "Normal: Bottom X Left Y".
  /// </summary>
  public class NewLayerNormalBottomXLeftY : AbstractGraphControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(GraphController ctrl)
    {
      ctrl.Doc.CreateNewLayerNormalBottomXLeftY();
    }
  }

  /// <summary>
  /// Handler for the menu item "Edit" - "New layer(axes)" - "Linked: Top X Right Y".
  /// </summary>
  public class NewLayerLinkedTopXRightY : AbstractGraphControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(GraphController ctrl)
    {
      ctrl.Doc.CreateNewLayerLinkedTopXRightY(ctrl.CurrentLayerNumber);
    }
  }

  /// <summary>
  /// Handler for the menu item "Edit" - "New layer(axes)" - "Linked: Top X".
  /// </summary>
  public class NewLayerLinkedTopX : AbstractGraphControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(GraphController ctrl)
    {
      ctrl.Doc.CreateNewLayerLinkedTopX(ctrl.CurrentLayerNumber);
    }
  }

  /// <summary>
  /// Handler for the menu item "Edit" - "New layer(axes)" - "Linked: Right Y".
  /// </summary>
  public class NewLayerLinkedRightY : AbstractGraphControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(GraphController ctrl)
    {
      ctrl.Doc.CreateNewLayerLinkedRightY(ctrl.CurrentLayerNumber);
    }
  }

  /// <summary>
  /// Handler for the menu item "Edit" - "New layer(axes)" - "Linked: Top X Right Y, X axis straight ".
  /// </summary>
  public class NewLayerLinkedTopXRightY_XAxisStraight : AbstractGraphControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(GraphController ctrl)
    {
      ctrl.Doc.CreateNewLayerLinkedTopXRightY_XAxisStraight(ctrl.CurrentLayerNumber);
    }
  }

  /// <summary>
  /// Renames the active graph.
  /// </summary>
  public class GraphRename : AbstractGraphControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(GraphController ctrl)
    {
      ctrl.Doc.ShowRenameDialog();
    }
  }

  /// <summary>
  /// Shows the properties of the active graph.
  /// </summary>
  public class GraphShowProperties : AbstractGraphControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(GraphController ctrl)
    {
      ctrl.Doc.ShowPropertyDialog();
    }
  }

  /// <summary>
  /// Moves the active graph to a different project folder.
  /// </summary>
  public class GraphMoveToFolder : AbstractGraphControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(GraphController ctrl)
    {
      Altaxo.Gui.Pads.ProjectBrowser.ProjectBrowserExtensions.MoveDocuments(new[] { ctrl.Doc });
    }
  }

  /// <summary>
  /// Refreshes the active graph.
  /// </summary>
  public class GraphRefresh : AbstractGraphControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(GraphController ctrl)
    {
      ctrl.RefreshGraph();
    }
  }

  /// <summary>
  /// Arranges the graph layers.
  /// </summary>
  public class ArrangeLayers : AbstractGraphControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(GraphController ctrl)
    {
      ctrl.Doc.ShowLayerArrangementDialog(ctrl.ActiveLayer);
    }
  }

  /// <summary>
  /// Groups the selected graph objects.
  /// </summary>
  public class GroupSelectedObjects : AbstractGraphControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(GraphController ctrl)
    {
      ctrl.GroupSelectedObjects();
    }
  }

  /// <summary>
  /// Ungroups the selected graph objects.
  /// </summary>
  public class UngroupSelectedObjects : AbstractGraphControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(GraphController ctrl)
    {
      ctrl.UngroupSelectedObjects();
    }
  }

  /// <summary>
  /// Aligns the selected graph objects at the top.
  /// </summary>
  public class ArrangeTop : AbstractGraphControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(GraphController ctrl)
    {
      ctrl.ArrangeTopToTop();
    }
  }

  /// <summary>
  /// Aligns the selected graph objects at the bottom.
  /// </summary>
  public class ArrangeBottom : AbstractGraphControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(GraphController ctrl)
    {
      ctrl.ArrangeBottomToBottom();
    }
  }

  /// <summary>
  /// Arranges the selected graph objects from top to bottom.
  /// </summary>
  public class ArrangeTopToBottom : AbstractGraphControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(GraphController ctrl)
    {
      ctrl.ArrangeTopToBottom();
    }
  }

  /// <summary>
  /// Arranges the selected graph objects from bottom to top.
  /// </summary>
  public class ArrangeBottomToTop : AbstractGraphControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(GraphController ctrl)
    {
      ctrl.ArrangeBottomToTop();
    }
  }

  /// <summary>
  /// Aligns the selected graph objects at the left.
  /// </summary>
  public class ArrangeLeft : AbstractGraphControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(GraphController ctrl)
    {
      ctrl.ArrangeLeftToLeft();
    }
  }

  /// <summary>
  /// Aligns the selected graph objects at the right.
  /// </summary>
  public class ArrangeRight : AbstractGraphControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(GraphController ctrl)
    {
      ctrl.ArrangeRightToRight();
    }
  }

  /// <summary>
  /// Arranges the selected graph objects from left to right.
  /// </summary>
  public class ArrangeLeftToRight : AbstractGraphControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(GraphController ctrl)
    {
      ctrl.ArrangeLeftToRight();
    }
  }

  /// <summary>
  /// Arranges the selected graph objects from right to left.
  /// </summary>
  public class ArrangeRightToLeft : AbstractGraphControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(GraphController ctrl)
    {
      ctrl.ArrangeRightToLeft();
    }
  }

  /// <summary>
  /// Arranges the selected graph objects horizontally.
  /// </summary>
  public class ArrangeHorizontal : AbstractGraphControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(GraphController ctrl)
    {
      ctrl.ArrangeHorizontal();
    }
  }

  /// <summary>
  /// Arranges the selected graph objects vertically.
  /// </summary>
  public class ArrangeVertical : AbstractGraphControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(GraphController ctrl)
    {
      ctrl.ArrangeVertical();
    }
  }

  /// <summary>
  /// Arranges the selected graph objects in a horizontal table.
  /// </summary>
  public class ArrangeHorizontalTable : AbstractGraphControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(GraphController ctrl)
    {
      ctrl.ArrangeHorizontalTable();
    }
  }

  /// <summary>
  /// Arranges the selected graph objects in a vertical table.
  /// </summary>
  public class ArrangeVerticalTable : AbstractGraphControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(GraphController ctrl)
    {
      ctrl.ArrangeVerticalTable();
    }
  }

  /// <summary>
  /// Makes the selected graph objects equally wide.
  /// </summary>
  public class ArrangeSameHorizontalSize : AbstractGraphControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(GraphController ctrl)
    {
      ctrl.ArrangeSameHorizontalSize();
    }
  }

  /// <summary>
  /// Makes the selected graph objects equally tall.
  /// </summary>
  public class ArrangeSameVerticalSize : AbstractGraphControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(GraphController ctrl)
    {
      ctrl.ArrangeSameVerticalSize();
    }
  }

  /// <summary>
  /// Moves the selected graph items one position up.
  /// </summary>
  public class MoveGraphItemUp : AbstractGraphControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(GraphController ctrl)
    {
      ctrl.MoveSelectedGraphItemsUp();
    }
  }

  /// <summary>
  /// Moves the selected graph items one position down.
  /// </summary>
  public class MoveGraphItemDown : AbstractGraphControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(GraphController ctrl)
    {
      ctrl.MoveSelectedGraphItemsDown();
    }
  }

  /// <summary>
  /// Moves the selected graph items to the top.
  /// </summary>
  public class MoveGraphItemToTop : AbstractGraphControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(GraphController ctrl)
    {
      ctrl.MoveSelectedGraphItemsToTop();
    }
  }

  /// <summary>
  /// Moves the selected graph items to the bottom.
  /// </summary>
  public class MoveGraphItemToBottom : AbstractGraphControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(GraphController ctrl)
    {
      ctrl.MoveSelectedGraphItemsToBottom();
    }
  }

  /// <summary>
  /// Enables automatic zoom.
  /// </summary>
  public class ZoomAutomatic : AbstractGraphControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(GraphController ctrl)
    {
      ctrl.IsAutoZoomActive = true;
    }
  }

  /// <summary>
  /// Sets the zoom factor to 50 percent.
  /// </summary>
  public class Zoom50Percent : AbstractGraphControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(GraphController ctrl)
    {
      ctrl.ZoomFactor = 0.5;
    }
  }

  /// <summary>
  /// Sets the zoom factor to 100 percent.
  /// </summary>
  public class Zoom100Percent : AbstractGraphControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(GraphController ctrl)
    {
      ctrl.ZoomFactor = 1.0;
    }
  }

  /// <summary>
  /// Sets the zoom factor to 200 percent.
  /// </summary>
  public class Zoom200Percent : AbstractGraphControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(GraphController ctrl)
    {
      ctrl.ZoomFactor = 2.0;
    }
  }

  /// <summary>
  /// Prompts for a custom zoom factor.
  /// </summary>
  public class ZoomUserPercent : AbstractGraphControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(GraphController ctrl)
    {
      double zoom = ctrl.ZoomFactor * 100;
      if (Current.Gui.ShowDialog(ref zoom, "Choose zoom factor (percent)", false))
      {
        ctrl.ZoomFactor = zoom / 100;
      }
    }
  }

  /// <summary>
  /// Prompts for a custom graph margin.
  /// </summary>
  public class MarginUserPercent : AbstractGraphControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(GraphController ctrl)
    {
      double zoom = ctrl.Margin * 100;
      if (Current.Gui.ShowDialog(ref zoom, "Choose margin around graph (percent)", false))
      {
        ctrl.Margin = zoom / 100;
      }
    }
  }

  /// <summary>
  /// Sets the graph margin to zero.
  /// </summary>
  public class Margin0Percent : AbstractGraphControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(GraphController ctrl)
    {
      ctrl.Margin = 0;
    }
  }

  /// <summary>
  /// Sets the graph margin to 10 percent.
  /// </summary>
  public class Margin10Percent : AbstractGraphControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(GraphController ctrl)
    {
      ctrl.Margin = 0.1;
    }
  }

  /// <summary>
  /// Sets the graph margin to 50 percent.
  /// </summary>
  public class Margin50Percent : AbstractGraphControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(GraphController ctrl)
    {
      ctrl.Margin = 0.5;
    }
  }

  /// <summary>
  /// Adds a floating scale to the active layer.
  /// </summary>
  public class AddScale : AbstractGraphControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(GraphController ctrl)
    {
      var layer = ctrl.ActiveLayer;
      var scale = new Gdi.Shapes.FloatingScale(ctrl.Doc.GetPropertyHierarchy());
      if (scale.IsCompatibleWithParent(layer))
      {
        layer.GraphObjects.Add(scale);
      }
      else
      {
        Current.Gui.ErrorMessageBox("Sorry. The new scale is not compatible with the currently selected layer! Please select an XYPlotLayer as current layer.");
      }
    }
  }

  /// <summary>
  /// Adds a legend for the active density image plot.
  /// </summary>
  public class AddDensityImageLegend : AbstractGraphControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(GraphController ctrl)
    {
      var layer = ctrl.ActiveLayer as XYPlotLayer;
      if (layer is null || ctrl.CurrentPlotNumber < 0 || !(layer.PlotItems[ctrl.CurrentPlotNumber] is DensityImagePlotItem))
      {
        Current.Gui.ErrorMessageBox("Current plot item should be a density image plot!");
        return;
      }

      var plotItem = (DensityImagePlotItem)layer.PlotItems[ctrl.CurrentPlotNumber];
      var legend = new Gdi.Shapes.DensityImageLegend(plotItem, layer, 0.5 * layer.Size, new PointD2D(layer.Size.X / 3, layer.Size.Y / 2), ctrl.Doc.GetPropertyHierarchy());
      layer.GraphObjects.Add(legend);
    }
  }

  /// <summary>
  /// Duplicates the Graph and the Graph view to a new one.
  /// </summary>
  public class DuplicateGraph : AbstractGraphControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(GraphController ctrl)
    {
      var newDoc = new GraphDocument(ctrl.Doc);
      var newnamebase = Altaxo.Main.ProjectFolder.CreateFullName(ctrl.Doc.Name, ctrl.Doc.ShortName);
      newDoc.Name = Current.Project.GraphDocumentCollection.FindNewItemName(newnamebase);
      Current.Project.GraphDocumentCollection.Add(newDoc);
      Current.ProjectService.CreateNewGraph(newDoc);
    }
  }

  /// <summary>
  /// Opens the controller for the active layer.
  /// </summary>
  public class LayerControl : AbstractGraphControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(GraphController ctrl)
    {
      ctrl.EnsureValidityOfCurrentLayerNumber();
      var t1 = ctrl.ActiveLayer as XYPlotLayer;
      if (t1 is not null)
      {
        XYPlotLayerController.ShowDialog(t1);
        return;
      }
      var t2 = ctrl.ActiveLayer;
      if (t2 is not null)
      {
        HostLayerController.ShowDialog(t2);
        return;
      }
    }
  }

  /// <summary>
  /// Adds a sample curve plot to the active layer.
  /// </summary>
  public class AddCurvePlot : AbstractGraphControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(GraphController ctrl)
    {
      ctrl.EnsureValidityOfCurrentLayerNumber();
      var xylayer = ctrl.Doc.RootLayer.ElementAt(ctrl.CurrentLayerNumber) as XYPlotLayer;
      if (xylayer is not null)
        xylayer.PlotItems.Add(new XYFunctionPlotItem(new XYFunctionPlotData(new PolynomialFunction(new double[] { 0, 0, 1 })), new G2DPlotStyleCollection(LineScatterPlotStyleKind.Line, xylayer.GetPropertyContext())));
    }
  }

  /// <summary>
  /// Handler for the menu item "Graph" - "New layer legend.
  /// </summary>
  public class NewLayerLegend : AbstractGraphControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(GraphController ctrl)
    {
      ctrl.Doc.RootLayer.IsValidIndex(ctrl.CurrentLayerNumber, out var l);

      if (l is XYPlotLayer)
        ((XYPlotLayer)l).CreateNewLayerLegend();
    }
  }

  /// <summary>
  /// Handler for the toolbar item Rescale axes.
  /// </summary>
  public class RescaleAxes : AbstractGraphControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(GraphController ctrl)
    {
      ctrl.Doc.RootLayer.IsValidIndex(ctrl.CurrentLayerNumber, out var l);

      if (l is XYPlotLayer)
      {
        ((XYPlotLayer)l).OnUserRescaledAxes();
      }
    }
  }

  /// <summary>
  /// Handler for the menu item "Graph" - "New layer legend.
  /// </summary>
  public class FitPolynomial : AbstractGraphControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(GraphController ctrl)
    {
      var dlg = new FitPolynomialDialogController(2, double.NegativeInfinity, double.PositiveInfinity, false);
      if (Current.Gui.ShowDialog(dlg, "Polynomial fit", false))
      {
        var result = (FitPolynomialOptions)dlg.ModelObject;
        Altaxo.Graph.Procedures.PolynomialFitting.Fit(ctrl, result.Order, result.FitCurveXmin ?? double.NegativeInfinity, result.FitCurveXmax ?? double.PositiveInfinity, result.ShowFormulaOnGraph);
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
    /// <inheritdoc/>
    public override void Run(GraphController ctrl)
    {
      string result = Altaxo.Graph.Procedures.NonlinearFitting.ShowFitDialog(ctrl);
      if (result is not null)
        Current.Gui.ErrorMessageBox(result);
    }
  }

  /// <summary>
  /// Opens the master-curve creation dialog for the active graph.
  /// </summary>
  public class CreateMasterCurve : AbstractGraphControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(GraphController ctrl)
    {
      Altaxo.Graph.Procedures.MasterCurveCreation.ShowMasterCurveCreationDialog(ctrl.Doc);
    }
  }

  /// <summary>
  /// Creates a user-defined function plot.
  /// </summary>
  public class NewUserFunction : AbstractGraphControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(GraphController ctrl)
    {
      ctrl.Doc.RootLayer.IsValidIndex(ctrl.CurrentLayerNumber, out var activeLayer);

      if (!(activeLayer is XYPlotLayer))
        return;

      FunctionEvaluationScript script = null; //

      if (script is null)
        script = new FunctionEvaluationScript();

      object[] args = new object[] { script, new ScriptExecutionHandler(EhScriptExecution) };
      if (Current.Gui.ShowDialog(args, "Function script"))
      {
        ctrl.EnsureValidityOfCurrentLayerNumber();

        script = (FunctionEvaluationScript)args[0];
        var functItem = new XYFunctionPlotItem(new XYFunctionPlotData(script), new G2DPlotStyleCollection(LineScatterPlotStyleKind.Line, activeLayer.GetPropertyContext()));
        ((XYPlotLayer)activeLayer).PlotItems.Add(functItem);
      }
    }

    /// <summary>
    /// Executes the function script.
    /// </summary>
    /// <param name="script">The script to execute.</param>
    /// <param name="reporter">The progress reporter.</param>
    /// <returns><see langword="true"/> if execution is allowed; otherwise, <see langword="false"/>.</returns>
    public bool EhScriptExecution(IScriptText script, IProgressReporter reporter)
    {
      return true;
    }
  }

  /// <summary>
  /// Handler for the menu item "Graph" - "New layer legend.
  /// </summary>
  public static class SaveAsMiniProjectBase
  {
    /// <summary>
    /// Creates a mini project from the specified graph document and saves it.
    /// </summary>
    /// <param name="doc">The graph document to save as a mini project.</param>
    public static void Run(Altaxo.Graph.GraphDocumentBase doc)
    {
      var newDocument = Altaxo.Graph.Procedures.MiniProjectBuilder.CreateMiniProject(doc, false);
      SaveProjectAs(newDocument);
    }

    /// <summary>
    /// Asks the user for a file name for the current project, and then saves the project under the given name.
    /// </summary>
    /// <param name="projectToSave">The project to save.</param>
    public static void SaveProjectAs(Altaxo.AltaxoDocument projectToSave)
    {
      var dlg = new Altaxo.Gui.SaveFileOptions();

      string description = StringParser.Parse("${res:Altaxo.FileFilter.ProjectFiles})");

      dlg.AddFilter("*.axoprj", description);
      dlg.OverwritePrompt = true;
      dlg.AddExtension = true;

      if (!Current.Gui.ShowSaveFileDialog(dlg))
        return;

      try
      {
        SaveProject(projectToSave, dlg.FileName);
      }
      catch (Exception ex)
      {
        Current.Gui.ErrorMessageBox(ex.Message, "Error during saving of the mini project");
      }
    }

    /// <summary>
    /// Internal routine to save a project under a given name.
    /// </summary>
    /// <param name="projectToSave">The project to save.</param>
    /// <param name="filename">The file name to save to.</param>
    public static void SaveProject(Altaxo.AltaxoDocument projectToSave, string filename)
    {
      using (var myStream = new System.IO.FileStream(filename, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.Write, System.IO.FileShare.None))
      {
        using (var archive = new Main.Services.Files.ZipArchiveAsProjectArchive(myStream, System.IO.Compression.ZipArchiveMode.Create, false))
        {
          var info = new Altaxo.Serialization.Xml.XmlStreamSerializationInfo();
          projectToSave.SaveToArchive(archive, info);
        }
        myStream.Close();
      }
    }
  }

  /// <summary>
  /// Handler for the menu item "Graph" - "New layer legend.
  /// </summary>
  public class SaveAsMiniProject : AbstractGraphControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(GraphController ctrl)
    {
      SaveAsMiniProjectBase.Run(ctrl.Doc);
    }
  }

  /// <summary>
  /// Command that copies the currently selected graph as mini project to the clipboard
  /// </summary>
  public class CopyAsMiniProjectToClipboard : AbstractGraphControllerCommand
  {
    /// <summary>
    /// Identifier used for putting a list of project items on the clipboard.
    /// </summary>
    public const string ClipboardFormat_MiniProjectItems = "Altaxo.Main.ProjectItems.GraphAsMiniProject";

    /// <inheritdoc/>
    public override void Run(GraphController ctrl)
    {
      var newDocument = Altaxo.Graph.Procedures.MiniProjectBuilder.CreateMiniProject(ctrl.Doc, false);

      var items = new HashSet<IProjectItem>();
      foreach (var coll in newDocument.ProjectItemCollections)
        foreach (var item in coll.ProjectItems)
          items.Add(item);

      Altaxo.Serialization.Clipboard.ClipboardSerialization.PutObjectToClipboard(
        new string[]{
        Main.Commands.ProjectItemCommands.ClipboardFormat_ListOfProjectItems,
        ClipboardFormat_MiniProjectItems
        },
        new Main.Commands.ProjectItemCommands.ProjectItemClipboardList(items, baseFolder: string.Empty));
    }
  }

  /// <summary>
  /// Command that copies the currently selected graph as mini project from the clipboard
  /// to a new user defined project folder.
  /// </summary>
  public class PasteGraphAsMiniProject : SimpleCommand
  {
    /// <inheritdoc/>
    public override bool CanExecute(object parameter)
    {
      return
        Altaxo.Serialization.Clipboard.ClipboardSerialization.IsClipboardFormatAvailable(CopyAsMiniProjectToClipboard.ClipboardFormat_MiniProjectItems) &&
        Altaxo.Serialization.Clipboard.ClipboardSerialization.IsClipboardFormatAvailable(Main.Commands.ProjectItemCommands.ClipboardFormat_ListOfProjectItems);
    }

    /// <inheritdoc/>
    public override void Execute(object parameter)
    {
      if (!CanExecute(parameter))
        return;

      var tvctrl = new TextValueInputController(string.Empty, "Name of new folder for mini project: ")
      {
        Validator = new ProjectFolders.FolderIsNewValidator()
      };

      if (!Current.Gui.ShowDialog(tvctrl, "Enter new folder name", false))
        return;

      var folderName = tvctrl.InputText;
      if (!folderName.EndsWith(ProjectFolder.DirectorySeparatorString))
        folderName += ProjectFolder.DirectorySeparatorString;

      var list = Altaxo.Serialization.Clipboard.ClipboardSerialization.GetObjectFromClipboard<Main.Commands.ProjectItemCommands.ProjectItemClipboardList>(Main.Commands.ProjectItemCommands.ClipboardFormat_ListOfProjectItems);

      list.TryToKeepInternalReferences = true;
      list.RelocateReferences = true;
      Main.Commands.ProjectItemCommands.PasteItems(folderName, list);

    }
  }

  /// <summary>
  /// Provides an abstract base class for checkable commands that apply to graph controllers.
  /// </summary>
  public abstract class AbstractCheckableGraphControllerCommand : SimpleCheckableCommand, System.ComponentModel.INotifyPropertyChanged
  {
    /// <summary>
    /// Gets the currently active graph controller.
    /// </summary>
    public GraphController Controller
    {
      get
      {
        if (Current.Workbench.ActiveViewContent is Altaxo.Gui.Graph.Gdi.Viewing.GraphController ct)
        {
          return ct;
        }
        else
          return null;
      }
    }

    /// <inheritdoc/>
    public override void Execute(object parameter)
    {
      base.Execute(parameter);
    }

    /// <inheritdoc/>
    public override bool IsChecked
    {
      get
      {
        return base.IsChecked;
      }
      set
      {
        var oldValue = base.IsChecked;
        base.IsChecked = value;

        if (value != oldValue)
          OnPropertyChanged("IsChecked");
      }
    }

    /// <inheritdoc/>
    public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    /// Raises the <see cref="PropertyChanged"/> event.
    /// </summary>
    /// <param name="propertyName">The name of the property that changed.</param>
    protected virtual void OnPropertyChanged(string propertyName)
    {
      if (PropertyChanged is not null)
        PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
    }
  }
}
