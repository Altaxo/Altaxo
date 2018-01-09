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

using Altaxo.Collections;
using Altaxo.Drawing;
using Altaxo.Geometry;
using Altaxo.Graph.Gdi;
using Altaxo.Graph.Gdi.Plot;
using Altaxo.Graph.Gdi.Plot.Styles;
using Altaxo.Graph.Plot.Data;
using Altaxo.Gui.AddInItems;
using Altaxo.Gui.Graph;
using Altaxo.Gui.Graph.Gdi;
using Altaxo.Gui.Graph.Gdi.Viewing;
using Altaxo.Gui.Scripting;
using Altaxo.Main;
using Altaxo.Main.Services;
using Altaxo.Scripting;
using System;
using System.ComponentModel;
using System.Drawing.Imaging;
using System.Linq;
using System.Windows.Input;

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
			if (Current.Workbench.ActiveViewContent is Altaxo.Gui.Graph.Gdi.Viewing.GraphController ctrl)
				Run(ctrl);
		}

		/// <summary>
		/// Override this function for adding own worksheet commands. You will get
		/// the worksheet controller in the parameter.
		/// </summary>
		/// <param name="ctrl">The worksheet controller this command is applied to.</param>
		public abstract void Run(GraphController ctrl);
	}

	/// <summary>
	/// Handler for the menu item "File" - "Print".
	/// </summary>
	public class Print : AbstractGraphControllerCommand
	{
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
		public override void Run(GraphController ctrl)
		{
			using (var brush = new BrushX(NamedColors.White))
				ctrl.Doc.CopyToClipboardAsBitmap(150, brush, PixelFormat.Format24bppRgb);
		}
	}

	/// <summary>
	/// Handler for the menu item "Edit" - "CopyPageAsBitmap".
	/// The resulting bitmap has a resolution of 300 dpi and ARGB format.
	/// </summary>
	public class CopyPageAsBitmap300dpiARGB : AbstractGraphControllerCommand
	{
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
		public override void Run(GraphController ctrl)
		{
			using (var brush = new BrushX(NamedColors.White))
				ctrl.Doc.CopyToClipboardAsBitmap(300, brush, PixelFormat.Format24bppRgb);
		}
	}

	public class SaveGraphAsTemplate : AbstractGraphControllerCommand
	{
		public override void Run(GraphController ctrl)
		{
			System.IO.Stream myStream;
			var saveFileDialog1 = new Microsoft.Win32.SaveFileDialog();

			saveFileDialog1.Filter = "Altaxo graph files (*.axogrp)|*.axogrp|All files (*.*)|*.*";
			saveFileDialog1.FilterIndex = 1;
			saveFileDialog1.RestoreDirectory = true;

			if (true == saveFileDialog1.ShowDialog((System.Windows.Window)Current.Workbench.ViewObject))
			{
				if ((myStream = saveFileDialog1.OpenFile()) != null)
				{
					Altaxo.Serialization.Xml.XmlStreamSerializationInfo info = new Altaxo.Serialization.Xml.XmlStreamSerializationInfo();
					info.BeginWriting(myStream);
					info.AddValue("Graph", ctrl.Doc);
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
		public override void Run(GraphController ctrl)
		{
			ctrl.Doc.ShowFileExportTiffDialog();
		}
	}

	public class FileExportSpecific : AbstractGraphControllerCommand
	{
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
		public override void Run(GraphController ctrl)
		{
			ctrl.Doc.CreateNewLayerLinkedTopXRightY_XAxisStraight(ctrl.CurrentLayerNumber);
		}
	}

	public class GraphRename : AbstractGraphControllerCommand
	{
		public override void Run(GraphController ctrl)
		{
			ctrl.Doc.ShowRenameDialog();
		}
	}

	public class GraphShowProperties : AbstractGraphControllerCommand
	{
		public override void Run(GraphController ctrl)
		{
			ctrl.Doc.ShowPropertyDialog();
		}
	}

	public class GraphMoveToFolder : AbstractGraphControllerCommand
	{
		public override void Run(GraphController ctrl)
		{
			Altaxo.Gui.Pads.ProjectBrowser.ProjectBrowserExtensions.MoveDocuments(new[] { ctrl.Doc });
		}
	}

	public class GraphRefresh : AbstractGraphControllerCommand
	{
		public override void Run(GraphController ctrl)
		{
			ctrl.RefreshGraph();
		}
	}

	public class ArrangeLayers : AbstractGraphControllerCommand
	{
		public override void Run(GraphController ctrl)
		{
			ctrl.Doc.ShowLayerArrangementDialog(ctrl.ActiveLayer);
		}
	}

	public class GroupSelectedObjects : AbstractGraphControllerCommand
	{
		public override void Run(GraphController ctrl)
		{
			ctrl.GroupSelectedObjects();
		}
	}

	public class UngroupSelectedObjects : AbstractGraphControllerCommand
	{
		public override void Run(GraphController ctrl)
		{
			ctrl.UngroupSelectedObjects();
		}
	}

	public class ArrangeTop : AbstractGraphControllerCommand
	{
		public override void Run(GraphController ctrl)
		{
			ctrl.ArrangeTopToTop();
		}
	}

	public class ArrangeBottom : AbstractGraphControllerCommand
	{
		public override void Run(GraphController ctrl)
		{
			ctrl.ArrangeBottomToBottom();
		}
	}

	public class ArrangeTopToBottom : AbstractGraphControllerCommand
	{
		public override void Run(GraphController ctrl)
		{
			ctrl.ArrangeTopToBottom();
		}
	}

	public class ArrangeBottomToTop : AbstractGraphControllerCommand
	{
		public override void Run(GraphController ctrl)
		{
			ctrl.ArrangeBottomToTop();
		}
	}

	public class ArrangeLeft : AbstractGraphControllerCommand
	{
		public override void Run(GraphController ctrl)
		{
			ctrl.ArrangeLeftToLeft();
		}
	}

	public class ArrangeRight : AbstractGraphControllerCommand
	{
		public override void Run(GraphController ctrl)
		{
			ctrl.ArrangeRightToRight();
		}
	}

	public class ArrangeLeftToRight : AbstractGraphControllerCommand
	{
		public override void Run(GraphController ctrl)
		{
			ctrl.ArrangeLeftToRight();
		}
	}

	public class ArrangeRightToLeft : AbstractGraphControllerCommand
	{
		public override void Run(GraphController ctrl)
		{
			ctrl.ArrangeRightToLeft();
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

	public class ArrangeSameHorizontalSize : AbstractGraphControllerCommand
	{
		public override void Run(GraphController ctrl)
		{
			ctrl.ArrangeSameHorizontalSize();
		}
	}

	public class ArrangeSameVerticalSize : AbstractGraphControllerCommand
	{
		public override void Run(GraphController ctrl)
		{
			ctrl.ArrangeSameVerticalSize();
		}
	}

	public class MoveGraphItemUp : AbstractGraphControllerCommand
	{
		public override void Run(GraphController ctrl)
		{
			ctrl.MoveSelectedGraphItemsUp();
		}
	}

	public class MoveGraphItemDown : AbstractGraphControllerCommand
	{
		public override void Run(GraphController ctrl)
		{
			ctrl.MoveSelectedGraphItemsDown();
		}
	}

	public class MoveGraphItemToTop : AbstractGraphControllerCommand
	{
		public override void Run(GraphController ctrl)
		{
			ctrl.MoveSelectedGraphItemsToTop();
		}
	}

	public class MoveGraphItemToBottom : AbstractGraphControllerCommand
	{
		public override void Run(GraphController ctrl)
		{
			ctrl.MoveSelectedGraphItemsToBottom();
		}
	}

	public class ZoomAutomatic : AbstractGraphControllerCommand
	{
		public override void Run(GraphController ctrl)
		{
			ctrl.IsAutoZoomActive = true;
		}
	}

	public class Zoom50Percent : AbstractGraphControllerCommand
	{
		public override void Run(GraphController ctrl)
		{
			ctrl.ZoomFactor = 0.5;
		}
	}

	public class Zoom100Percent : AbstractGraphControllerCommand
	{
		public override void Run(GraphController ctrl)
		{
			ctrl.ZoomFactor = 1.0;
		}
	}

	public class Zoom200Percent : AbstractGraphControllerCommand
	{
		public override void Run(GraphController ctrl)
		{
			ctrl.ZoomFactor = 2.0;
		}
	}

	public class ZoomUserPercent : AbstractGraphControllerCommand
	{
		public override void Run(GraphController ctrl)
		{
			double zoom = ctrl.ZoomFactor * 100;
			if (Current.Gui.ShowDialog(ref zoom, "Choose zoom factor (percent)", false))
			{
				ctrl.ZoomFactor = zoom / 100;
			}
		}
	}

	public class MarginUserPercent : AbstractGraphControllerCommand
	{
		public override void Run(GraphController ctrl)
		{
			double zoom = ctrl.Margin * 100;
			if (Current.Gui.ShowDialog(ref zoom, "Choose margin around graph (percent)", false))
			{
				ctrl.Margin = zoom / 100;
			}
		}
	}

	public class Margin0Percent : AbstractGraphControllerCommand
	{
		public override void Run(GraphController ctrl)
		{
			ctrl.Margin = 0;
		}
	}

	public class Margin10Percent : AbstractGraphControllerCommand
	{
		public override void Run(GraphController ctrl)
		{
			ctrl.Margin = 0.1;
		}
	}

	public class Margin50Percent : AbstractGraphControllerCommand
	{
		public override void Run(GraphController ctrl)
		{
			ctrl.Margin = 0.5;
		}
	}

	public class AddScale : AbstractGraphControllerCommand
	{
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

	public class AddDensityImageLegend : AbstractGraphControllerCommand
	{
		public override void Run(GraphController ctrl)
		{
			var layer = ctrl.ActiveLayer as XYPlotLayer;
			if (null == layer || ctrl.CurrentPlotNumber < 0 || !(layer.PlotItems[ctrl.CurrentPlotNumber] is DensityImagePlotItem))
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
		public override void Run(GraphController ctrl)
		{
			GraphDocument newDoc = new GraphDocument(ctrl.Doc);
			string newnamebase = Altaxo.Main.ProjectFolder.CreateFullName(ctrl.Doc.Name, "GRAPH");
			newDoc.Name = Current.Project.GraphDocumentCollection.FindNewItemName(newnamebase);
			Current.Project.GraphDocumentCollection.Add(newDoc);
			Current.ProjectService.CreateNewGraph(newDoc);
		}
	}

	public class LayerControl : AbstractGraphControllerCommand
	{
		public override void Run(GraphController ctrl)
		{
			ctrl.EnsureValidityOfCurrentLayerNumber();
			var t1 = ctrl.ActiveLayer as XYPlotLayer;
			if (null != t1)
			{
				XYPlotLayerController.ShowDialog(t1);
				return;
			}
			var t2 = ctrl.ActiveLayer;
			if (null != t2)
			{
				HostLayerController.ShowDialog(t2);
				return;
			}
		}
	}

	public class AddCurvePlot : AbstractGraphControllerCommand
	{
		public override void Run(GraphController ctrl)
		{
			ctrl.EnsureValidityOfCurrentLayerNumber();
			var xylayer = ctrl.Doc.RootLayer.ElementAt(ctrl.CurrentLayerNumber) as XYPlotLayer;
			if (null != xylayer)
				xylayer.PlotItems.Add(new XYFunctionPlotItem(new XYFunctionPlotData(new PolynomialFunction(new double[] { 0, 0, 1 })), new G2DPlotStyleCollection(LineScatterPlotStyleKind.Line, xylayer.GetPropertyContext())));
		}
	}

	/// <summary>
	/// Handler for the menu item "Graph" - "New layer legend.
	/// </summary>
	public class NewLayerLegend : AbstractGraphControllerCommand
	{
		public override void Run(GraphController ctrl)
		{
			HostLayer l;
			ctrl.Doc.RootLayer.IsValidIndex(ctrl.CurrentLayerNumber, out l);

			if (l is XYPlotLayer)
				((XYPlotLayer)l).CreateNewLayerLegend();
		}
	}

	/// <summary>
	/// Handler for the toolbar item Rescale axes.
	/// </summary>
	public class RescaleAxes : AbstractGraphControllerCommand
	{
		public override void Run(GraphController ctrl)
		{
			HostLayer l;
			ctrl.Doc.RootLayer.IsValidIndex(ctrl.CurrentLayerNumber, out l);

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
		public override void Run(GraphController ctrl)
		{
			FitPolynomialDialogController dlg = new FitPolynomialDialogController(2, double.NegativeInfinity, double.PositiveInfinity, false);
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
		public override void Run(GraphController ctrl)
		{
			string result = Altaxo.Graph.Procedures.NonlinearFitting.ShowFitDialog(ctrl);
			if (null != result)
				Current.Gui.ErrorMessageBox(result);
		}
	}

	public class CreateMasterCurve : AbstractGraphControllerCommand
	{
		public override void Run(GraphController ctrl)
		{
			Altaxo.Graph.Procedures.MasterCurveCreation.ShowMasterCurveCreationDialog(ctrl.Doc);
		}
	}

	public class NewUserFunction : AbstractGraphControllerCommand
	{
		public override void Run(GraphController ctrl)
		{
			HostLayer activeLayer;
			ctrl.Doc.RootLayer.IsValidIndex(ctrl.CurrentLayerNumber, out activeLayer);

			if (!(activeLayer is XYPlotLayer))
				return;

			FunctionEvaluationScript script = null; //

			if (script == null)
				script = new FunctionEvaluationScript();

			object[] args = new object[] { script, new ScriptExecutionHandler(this.EhScriptExecution) };
			if (Current.Gui.ShowDialog(args, "Function script"))
			{
				ctrl.EnsureValidityOfCurrentLayerNumber();

				script = (FunctionEvaluationScript)args[0];
				XYFunctionPlotItem functItem = new XYFunctionPlotItem(new XYFunctionPlotData(script), new G2DPlotStyleCollection(LineScatterPlotStyleKind.Line, activeLayer.GetPropertyContext()));
				((XYPlotLayer)activeLayer).PlotItems.Add(functItem);
			}
		}

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
		public static void Run(Altaxo.Graph.GraphDocumentBase doc)
		{
			var miniProjectBuilder = new Altaxo.Graph.Procedures.MiniProjectBuilder();
			var newDocument = miniProjectBuilder.GetMiniProject(doc, false);
			SaveProjectAs(newDocument);
		}

		/// <summary>
		/// Asks the user for a file name for the current project, and then saves the project under the given name.
		/// </summary>
		public static void SaveProjectAs(Altaxo.AltaxoDocument projectToSave)
		{
			var dlg = new Altaxo.Gui.SaveFileOptions();

			string description = StringParser.Parse("${res:Altaxo.FileFilter.ProjectFiles})"); ;
			dlg.AddFilter(".axoprj", description);
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
		/// <param name="filename"></param>
		public static void SaveProject(Altaxo.AltaxoDocument projectToSave, string filename)
		{
			using (var myStream = new System.IO.FileStream(filename, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.Write, System.IO.FileShare.None))
			{
				using (var zippedStream = new System.IO.Compression.ZipArchive(myStream, System.IO.Compression.ZipArchiveMode.Create))
				{
					var info = new Altaxo.Serialization.Xml.XmlStreamSerializationInfo();
					projectToSave.SaveToZippedFile(zippedStream, info);
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
		public override void Run(GraphController ctrl)
		{
			SaveAsMiniProjectBase.Run(ctrl.Doc);
		}
	}

	/// <summary>
	/// Provides a abstract class for issuing commands that apply to worksheet controllers.
	/// </summary>
	public abstract class AbstractCheckableGraphControllerCommand : AbstractCheckableMenuCommand, System.ComponentModel.INotifyPropertyChanged
	{
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

		/// <summary>
		/// This function is never be called, since this is a CheckableMenuCommand.
		/// </summary>
		public override void Run()
		{
			base.Run();
		}

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

		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged(string propertyName)
		{
			if (null != PropertyChanged)
				PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
		}
	}

	/// <summary>
	/// This class is intented to be used for commands into the graph tools toolbar. Commands derived
	/// from it will update the toolbar whenever its state changed.
	/// </summary>
	public abstract class AbstractGraphToolsCommand : AbstractCheckableGraphControllerCommand
	{
		private GraphController myCurrentGraphController;
		private GraphToolType _graphToolType;

		protected AbstractGraphToolsCommand(GraphToolType toolType)
		{
			_graphToolType = toolType;
			if (null != Current.Workbench)
			{
				Current.Workbench.PropertyChanged += this.EhWorkbenchContentChanged;
				this.EhWorkbenchContentChanged(this, new PropertyChangedEventArgs(nameof(Current.Workbench.ActiveViewContent)));
			}
		}

		protected void EhWorkbenchContentChanged(object o, PropertyChangedEventArgs e)
		{
			if (e.PropertyName != nameof(Current.Workbench.ActiveViewContent))
				return;

			if (!object.ReferenceEquals(Controller, myCurrentGraphController))
			{
				if (null != myCurrentGraphController)
				{
					lock (this)
					{
						this.myCurrentGraphController.CurrentGraphToolChanged -= new EventHandler(this.EhGraphToolChanged);
						this.myCurrentGraphController = null;
					}
				}
				if (Controller != null)
				{
					lock (this)
					{
						this.myCurrentGraphController = this.Controller;
						this.myCurrentGraphController.CurrentGraphToolChanged += new EventHandler(this.EhGraphToolChanged);
					}
				}
				OnPropertyChanged("IsChecked");
			}
		}

		protected void EhGraphToolChanged(object o, EventArgs e)
		{
			OnPropertyChanged(nameof(IsChecked));
		}

		public override bool IsChecked
		{
			get
			{
				return null == Controller ? false : _graphToolType == Controller.CurrentGraphTool;
			}
			set
			{
				if (value == true && Controller != null)
				{
					Controller.CurrentGraphTool = _graphToolType;
				}
				OnPropertyChanged("IsChecked");
			}
		}
	}

	/// <summary>
	/// Test class for a selected item
	/// </summary>
	public class SelectPointerTool : AbstractGraphToolsCommand
	{
		public SelectPointerTool()
			: base(GraphToolType.ObjectPointer)
		{
		}
	}

	/// <summary>
	/// Test class for a selected item
	/// </summary>
	public class SelectTextTool : AbstractGraphToolsCommand
	{
		public SelectTextTool()
			: base(GraphToolType.TextDrawing)
		{
		}
	}

	/// <summary>
	/// Tool for reading the x-y scatter values of a data point.
	/// </summary>
	public class ReadPlotItemDataTool : AbstractGraphToolsCommand
	{
		public ReadPlotItemDataTool()
			: base(GraphToolType.ReadPlotItemData)
		{
		}
	}

	/// <summary>Edits the grid of the current layer, or if it has no childs, the grid of the parent layer.</summary>
	public class EditGridTool : AbstractGraphToolsCommand
	{
		public EditGridTool()
			: base(GraphToolType.EditGrid)
		{
		}
	}

	/// <summary>
	/// Tool for reading the x-y coordinate values of a layer.
	/// </summary>
	public class ReadXYCoordinatesTool : AbstractGraphToolsCommand
	{
		public ReadXYCoordinatesTool()
			: base(GraphToolType.ReadXYCoordinates)
		{
		}
	}

	/// <summary>
	/// Drawing a simple line with two points.
	/// </summary>
	public class SingleLineDrawingTool : AbstractGraphToolsCommand
	{
		public SingleLineDrawingTool()
			: base(GraphToolType.SingleLineDrawing)
		{
		}
	}

	/// <summary>
	/// Drawing a simple line with two points.
	/// </summary>
	public class ArrowLineDrawingTool : AbstractGraphToolsCommand
	{
		public ArrowLineDrawingTool()
			: base(GraphToolType.ArrowLineDrawing)
		{
		}
	}

	/// <summary>
	/// Drawing a rectangle on the graph.
	/// </summary>
	public class RectangleDrawingTool : AbstractGraphToolsCommand
	{
		public RectangleDrawingTool()
			: base(GraphToolType.RectangleDrawing)
		{
		}
	}

	/// <summary>
	/// Drawing a rectangle on the graph.
	/// </summary>
	public class CurlyBraceDrawingTool : AbstractGraphToolsCommand
	{
		public CurlyBraceDrawingTool()
			: base(GraphToolType.CurlyBraceDrawing)
		{
		}
	}

	/// <summary>
	/// Drawing an ellipse on the graph.
	/// </summary>
	public class EllipseDrawingTool : AbstractGraphToolsCommand
	{
		public EllipseDrawingTool()
			: base(GraphToolType.EllipseDrawing)
		{
		}
	}

	/// <summary>
	/// Drawing an ellipse on the graph.
	/// </summary>
	public class RegularPolygonDrawingTool : AbstractGraphToolsCommand
	{
		public RegularPolygonDrawingTool()
			: base(GraphToolType.RegularPolygonDrawing)
		{
		}
	}

	/// <summary>
	/// Drawing of an open cardinal spline on a graph.
	/// </summary>
	public class OpenCardinalSplineDrawingTool : AbstractGraphToolsCommand
	{
		public OpenCardinalSplineDrawingTool()
			: base(GraphToolType.OpenCardinalSplineDrawing)
		{
		}
	}

	/// <summary>
	/// Drawing of an closed cardinal spline on a graph.
	/// </summary>
	public class ClosedCardinalSplineDrawingTool : AbstractGraphToolsCommand
	{
		public ClosedCardinalSplineDrawingTool()
			: base(GraphToolType.ClosedCardinalSplineDrawing)
		{
		}
	}

	/// <summary>
	/// Magnifies the axes according to the selected area.
	/// </summary>
	public class ZoomAxesTool : AbstractGraphToolsCommand
	{
		public ZoomAxesTool()
			: base(GraphToolType.ZoomAxes)
		{
		}
	}

	public class FontSizeChooser : System.Windows.Controls.ComboBox, ICommand
	{
		public event EventHandler CanExecuteChanged;

		public FontSizeChooser()
		{
			Initialize();
		}

		protected void Initialize()
		{
			this.IsEditable = true;

			this.Items.Add("8 pt");
			this.Items.Add("10 pt");
			this.Items.Add("12 pt");
			this.Items.Add("24 pt");

			this.KeyDown += comboBox_KeyDown;
		}

		private void comboBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
		{
			if (e.Key == System.Windows.Input.Key.Enter)
				Execute(null);
		}

		public bool CanExecute(object parameter)
		{
			return true;
		}

		public void Execute(object parameter)
		{
			if (string.IsNullOrEmpty(this.Text))
				return;

			var ctrl = Current.Workbench.ActiveViewContent as Altaxo.Gui.Graph.Gdi.Viewing.GraphController;

			if (null == ctrl)
				return;

			Altaxo.Serialization.LengthUnit unit = Altaxo.Serialization.LengthUnit.Point;
			string number;
			double value;
			if (Altaxo.Serialization.GUIConversion.IsDouble(this.Text, out value) ||
				(Altaxo.Serialization.LengthUnit.TryParse(this.Text, out unit, out number) &&
					Altaxo.Serialization.GUIConversion.IsDouble(number, out value)))
			{
				if (unit != null)
					unit = Altaxo.Serialization.LengthUnit.Point;
				string normalizedEntry = Altaxo.Serialization.GUIConversion.ToString(value) + " " + unit.Shortcut;
				value *= (double)(unit.UnitInMeter / Altaxo.Serialization.LengthUnit.Point.UnitInMeter);

				ctrl.SetSelectedObjectsProperty(new RoutedSetterProperty<double>("FontSize", value));

				if (!this.Items.Contains(normalizedEntry))
					this.Items.Add(normalizedEntry);
			}
		}
	}

	public class StrokeWidthChooser : System.Windows.Controls.ComboBox, ICommand
	{
		public event EventHandler CanExecuteChanged;

		public StrokeWidthChooser()
		{
			Initialize();
		}

		protected void Initialize()
		{
			IsEditable = true;

			Items.Add("8 pt");
			Items.Add("10 pt");
			Items.Add("12 pt");
			Items.Add("24 pt");

			KeyDown += EhKeyDown;
		}

		private void EhKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
		{
			if (e.Key == System.Windows.Input.Key.Enter)
				Execute(null);
		}

		public bool CanExecute(object parameter)
		{
			return true;
		}

		public void Execute(object parameter)
		{
			if (string.IsNullOrEmpty(this.Text))
				return;

			var ctrl = Current.Workbench.ActiveViewContent as Altaxo.Gui.Graph.Gdi.Viewing.GraphController;

			if (null == ctrl)
				return;

			Altaxo.Serialization.LengthUnit unit = Altaxo.Serialization.LengthUnit.Point;
			string number;
			double value;
			if (Altaxo.Serialization.GUIConversion.IsDouble(this.Text, out value) ||
				(Altaxo.Serialization.LengthUnit.TryParse(this.Text, out unit, out number) &&
					Altaxo.Serialization.GUIConversion.IsDouble(number, out value)))
			{
				if (unit != null)
					unit = Altaxo.Serialization.LengthUnit.Point;
				string normalizedEntry = Altaxo.Serialization.GUIConversion.ToString(value) + " " + unit.Shortcut;
				value *= (double)(unit.UnitInMeter / Altaxo.Serialization.LengthUnit.Point.UnitInMeter);

				ctrl.SetSelectedObjectsProperty(new RoutedSetterProperty<double>("StrokeWidth", value));

				if (!this.Items.Contains(normalizedEntry))
					this.Items.Add(normalizedEntry);
			}
		}
	}

	public class FontFamilyChooser : System.Windows.Controls.ComboBox, ICommand
	{
		public event EventHandler CanExecuteChanged;

		public FontFamilyChooser()
		{
			Initialize();
		}

		protected void Initialize()
		{
			KeyDown += EhKeyDown;

			// Fill with all available font families
			foreach (var famName in GdiFontManager.EnumerateAvailableGdiFontFamilyNames().OrderBy(x => x))
				Items.Add(famName);
		}

		private void EhKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
		{
			if (e.Key == System.Windows.Input.Key.Enter)
				Execute(null);
		}

		public bool CanExecute(object parameter)
		{
			return true;
		}

		public void Execute(object parameter)
		{
			if (string.IsNullOrEmpty(this.Text))
				return;

			var ctrl = Current.Workbench.ActiveViewContent as Altaxo.Gui.Graph.Gdi.Viewing.GraphController;
			if (null == ctrl)
				return;

			ctrl.SetSelectedObjectsProperty(new RoutedSetterProperty<string>("FontFamily", this.Text));
		}
	}
}