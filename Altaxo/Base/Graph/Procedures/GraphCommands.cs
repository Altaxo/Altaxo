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

using Altaxo.Graph.GUI;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

using Altaxo.Graph.Gdi;
using Altaxo.Gui.Common;

using System.Windows.Forms;

namespace Altaxo.Graph.Procedures
{
  /// <summary>
  /// GraphCommands contain operations concerning the graph document itself, such as rename.
  /// </summary>
  public class GraphCommands
  {
    public static void Rename(GraphController ctrl)
    {
      TextValueInputController tvctrl = new TextValueInputController(ctrl.Doc.Name,"Enter a name for the graph:");
      tvctrl.Validator = new GraphRenameValidator(ctrl.Doc, ctrl);

      if(Current.Gui.ShowDialog(tvctrl, "Rename graph", false))
        ctrl.Doc.Name = tvctrl.InputText.Trim();
    }

    protected class GraphRenameValidator : TextValueInputController.NonEmptyStringValidator
    {
      GraphDocument _doc;
      GraphController _controller;

      public GraphRenameValidator(GraphDocument graphdoc, GraphController ctrl)
        : base("The graph's name must not be empty! Please enter a valid name.")
      {
        _doc = graphdoc;
        _controller = ctrl;
      }

      public override string Validate(string graphname)
      {
        string err = base.Validate(graphname);
        if (null != err)
          return err;

        if (_doc.Name == graphname)
          return null;
        else if (GraphDocumentCollection.GetParentGraphDocumentCollectionOf(_controller.Doc) == null)
          return null; // if there is no parent data set we can enter anything
        else if (GraphDocumentCollection.GetParentGraphDocumentCollectionOf(_controller.Doc).Contains(graphname))
          return "This graph name already exists, please choose another name!";
        else
          return null;
      }
    }

    public static void Refresh(GraphController ctrl)
    {
      ctrl.RefreshGraph();
    }


		public static void PrintableSizeSetup(GraphController ctrl)
		{
			var options = new Altaxo.Gui.Graph.PrintableAreaSetupOptions();
			options.Area = ctrl.Doc.PrintableBounds;
			object resultobj = options;
			if (Current.Gui.ShowDialog(ref resultobj, "Setup printable area"))
			{
				var result = (Altaxo.Gui.Graph.PrintableAreaSetupOptions)resultobj;
				ctrl.Doc.SetPrintableBounds(result.Area, result.Rescale);
			}

		}

    /// <summary>
    /// Handler for the menu item "Edit" - "CopyPage".
    /// </summary>
    class CopyPageCommand
    {

      /*
      [DllImport("user32.dll")]
      static extern bool OpenClipboard(IntPtr hWndNewOwner);
      [DllImport("user32.dll")]
      static extern bool EmptyClipboard();
      [DllImport("user32.dll")]
      static extern IntPtr SetClipboardData(uint uFormat, IntPtr hMem);
      [DllImport("user32.dll")]
      static extern bool CloseClipboard();
      [DllImport("gdi32.dll")]
      static extern IntPtr CopyEnhMetaFile(IntPtr hemfSrc, IntPtr hNULL);
      [DllImport("gdi32.dll")]
      static extern bool DeleteEnhMetaFile(IntPtr hemf);

      /// <summary>
      /// Microsoft Knowledge Base Article - 323530 PRB: Metafiles on Clipboard Are Not Visible to All Applications
      /// </summary>
      /// <param name="hWnd"></param>
      /// <param name="mf"></param>
      /// <returns></returns>
      static bool PutEnhMetafileOnClipboard(IntPtr hWnd, Metafile mf)
      {
        bool bResult = false;
        IntPtr hEMF, hEMF2;
        hEMF = mf.GetHenhmetafile(); // invalidates mf
        if (!hEMF.Equals(new IntPtr(0)))
        {
          hEMF2 = CopyEnhMetaFile(hEMF, new IntPtr(0));
          if (!hEMF2.Equals(new IntPtr(0)))
          {
            if (OpenClipboard(hWnd))
            {
              if (EmptyClipboard())
              {
                IntPtr hRes = SetClipboardData(14 , hEMF2); // 14==CF_ENHMETAFILE
                bResult = hRes.Equals(hEMF2);
                CloseClipboard();
             }
            }
          }
          DeleteEnhMetaFile(hEMF);
        }
        return bResult;
      }
      */

			public static GraphExportOptions CopyPageOptions = new GraphExportOptions();


      static public void CopyAsMetafile(GraphController ctrl)
      {
       // System.Drawing.Imaging.Metafile mf = Altaxo.Graph.Procedures.Export.GetMetafile(ctrl.Doc);
       // PutEnhMetafileOnClipboard(ctrl.View.Form.Handle, mf);

        System.Windows.Forms.DataObject dao = new System.Windows.Forms.DataObject();
        string filepath = System.IO.Path.GetTempPath();
        string filename = filepath + "AltaxoClipboardMetafile.emf";
        if (System.IO.File.Exists(filename))
          System.IO.File.Delete(filename);
        Metafile mf = GraphExport.RenderAsMetafile(ctrl.Doc, filename, CopyPageOptions);
        System.Collections.Specialized.StringCollection coll = new System.Collections.Specialized.StringCollection();
        coll.Add(filename);
        dao.SetFileDropList(coll);
        dao.SetData(typeof(Metafile), mf);
        System.Windows.Forms.Clipboard.SetDataObject(dao);
      }


			static public void CopyAsBitmap(GraphController ctrl)
			{
				CopyPageToClipboardAsBitmap(ctrl, Brushes.White, CopyPageOptions);
			}

			static public void Run(GraphController ctrl)
			{
				var opt = CopyPageOptions;
				if (opt.ImageFormat == ImageFormat.Emf || opt.ImageFormat == ImageFormat.Wmf)
				{
					CopyAsMetafile(ctrl);
				}
				else
				{
					CopyAsBitmap(ctrl);
				}
			}

    }

    public static void CopyPageToClipboard(GraphController ctrl)
    {
      CopyPageCommand.Run(ctrl);
    }

		public static void SetCopyPageOptions(GraphController ctrl)
		{
			if (null == CopyPageCommand.CopyPageOptions)
				CopyPageCommand.CopyPageOptions = new GraphExportOptions();

			object resultobj = CopyPageCommand.CopyPageOptions;
			if (Current.Gui.ShowDialog(ref resultobj, "Set copy page options"))
			{
				CopyPageCommand.CopyPageOptions = (GraphExportOptions)resultobj;
			}
		}

		static string GetFileFilterString(ImageFormat fmt)
		{
			string filter;

			if (fmt == ImageFormat.Bmp)
				filter = "Bitmap files (*.bmp)|*.bmp|All files (*.*)|*.*";
			else if (fmt == ImageFormat.Emf)
				filter = "Enhances metafiles (*.emf)|*.emf|All files (*.*)|*.*";
			else if (ImageFormat.Exif == fmt)
				filter = "Exif files (*.exi)|*.exi|All files (*.*)|*.*";
			else if (ImageFormat.Gif == fmt)
				filter = "Gif files (*.gif)|*.gif|All files (*.*)|*.*";
			else if (ImageFormat.Icon == fmt)
				filter = "Icon files (*.ico)|*.ico|All files (*.*)|*.*";
			else if (ImageFormat.Jpeg == fmt)
				filter = "Jpeg files (*.jpg)|*.jpf|All files (*.*)|*.*";
			else if (ImageFormat.Png == fmt)
				filter = "Png files (*.png)|*.png|All files (*.*)|*.*";
			else if (ImageFormat.Tiff == fmt)
				filter = "Tiff files (*.tif)|*.tif|All files (*.*)|*.*";
			else if (ImageFormat.Wmf == fmt)
				filter = "Windows metafiles (*.wmf)|*.wmf|All files (*.*)|*.*";
			else
				filter = "All files (*.*)|*.*";

			return filter;
		}

		static GraphExportOptions _graphExportOptionsToFile = new GraphExportOptions();
		public static void FileExportSpecific(GraphController ctrl)
		{
			object resopt = _graphExportOptionsToFile;
			if (Current.Gui.ShowDialog(ref resopt, "Choose export options"))
			{
				_graphExportOptionsToFile = (GraphExportOptions)resopt;
			}
			else
			{
				return;
			}

			System.IO.Stream myStream;
			SaveFileDialog saveFileDialog1 = new SaveFileDialog();

			saveFileDialog1.Filter = GetFileFilterString(_graphExportOptionsToFile.ImageFormat);
			saveFileDialog1.FilterIndex = 1;
			saveFileDialog1.RestoreDirectory = true;

			if (saveFileDialog1.ShowDialog() == DialogResult.OK)
			{
				if ((myStream = saveFileDialog1.OpenFile()) != null)
				{
					ctrl.Doc.Render(myStream, _graphExportOptionsToFile);
					myStream.Close();
				} // end openfile ok
			} // end dlgresult ok
		}

		/// <summary>
		/// Puts the entire graph to the clipboard in XML format.
		/// </summary>
		/// <param name="ctrl">Graph controller.</param>
    public static void CopyGraphToClipboard(GraphController ctrl)
    {
			Serialization.ClipboardSerialization.PutObjectToClipboard("Altaxo.Graph.GraphDocumentAsXml",ctrl.Doc);
    }



    public static void PasteGraphStyleFromClipboard(GraphController ctrl)
    {
			object o = Serialization.ClipboardSerialization.GetObjectFromClipboard("Altaxo.Graph.GraphDocumentAsXml");
      if (!(o is GraphDocument))
        return;

      GraphDocument from = (GraphDocument)o;
      ctrl.Doc.CopyFrom(from, GraphCopyOptions.CopyFromLayers | GraphCopyOptions.CopyPlotStyles);
      Graph.Procedures.GraphCommands.RescaleAxes(ctrl.Doc);
    }

		public static void EditActiveLayer(GraphController ctrl)
		{
			ctrl.EnsureValidityOfCurrentLayerNumber();
			if (null != ctrl.ActiveLayer)
				Altaxo.Gui.Graph.LayerController.ShowDialog(ctrl.ActiveLayer);
		}

		/// <summary>
		/// Puts the active layer to the clipboard in XML format.
		/// </summary>
		/// <param name="ctrl">Graph controller.</param>
		public static void CopyActiveLayerToClipboard(GraphController ctrl)
    {
      Serialization.ClipboardSerialization.PutObjectToClipboard("Altaxo.Graph.GraphLayerAsXml", ctrl.ActiveLayer);
    }

		public static void PasteLayerAsTemplateForActiveLayerFromClipboard(GraphController ctrl, GraphCopyOptions options)
		{
			object o = Serialization.ClipboardSerialization.GetObjectFromClipboard("Altaxo.Graph.GraphLayerAsXml");
			if (null == o)
				return;
			XYPlotLayer layer = o as XYPlotLayer;
			if (null != layer)
			{
				ctrl.ActiveLayer.CopyFrom(layer,options);
				Graph.Procedures.GraphCommands.RescaleAxes(ctrl.Doc);
			}
		}

		public class PasteLayerOptions
		{
			public bool PastePlotItems { get; set; }
			public bool PastePlotStyles { get; set; }
			public GraphCopyOptions GetCopyOptions()
			{
				GraphCopyOptions co = GraphCopyOptions.None;
				if (PastePlotItems)
					co |= GraphCopyOptions.ClonePlotItems;
				if (PastePlotStyles)
					co |= GraphCopyOptions.CopyPlotStyles;
				return co;
			}
		}


		public static void PasteLayerAsTemplateForActiveLayerFromClipboard(GraphController ctrl)
		{
			object options = new PasteLayerOptions() { PastePlotStyles=true, PastePlotItems=true };
			if (false == Current.Gui.ShowDialog(ref options, "Choose what to paste"))
				return;
			PasteLayerAsTemplateForActiveLayerFromClipboard(ctrl, (options as PasteLayerOptions).GetCopyOptions());
		}

    public static void PasteAsNewLayerFromClipboard(GraphController ctrl)
    {
			object o = Serialization.ClipboardSerialization.GetObjectFromClipboard("Altaxo.Graph.GraphLayerAsXml");
      XYPlotLayer layer = o as XYPlotLayer;
      if(null!=layer)
        ctrl.Doc.Layers.Add(layer);
    }

		/// <summary>
		/// Pastes a layer on the clipboard as new layer before the active layer.
		/// </summary>
		/// <param name="ctrl"></param>
		public static void PasteAsNewLayerBeforeActiveLayerFromClipboard(GraphController ctrl)
		{
			object o = Serialization.ClipboardSerialization.GetObjectFromClipboard("Altaxo.Graph.GraphLayerAsXml");
			XYPlotLayer layer = o as XYPlotLayer;
			if (null != layer)
			{
				int idx = ctrl.CurrentLayerNumber;
				ctrl.Doc.Layers.Insert(idx,layer);
			}
		}


		/// <summary>
		/// Pastes a layer on the clipboard as new layer after the active layer.
		/// </summary>
		/// <param name="ctrl"></param>
		public static void PasteAsNewLayerAfterActiveLayerFromClipboard(GraphController ctrl)
		{
			object o = Serialization.ClipboardSerialization.GetObjectFromClipboard("Altaxo.Graph.GraphLayerAsXml");
			XYPlotLayer layer = o as XYPlotLayer;
			if (null != layer)
			{
				int idx = ctrl.CurrentLayerNumber;
				ctrl.Doc.Layers.Insert(idx+1, layer);
			}
		}

		/// <summary>
		/// Deletes the active layer.
		/// </summary>
		/// <param name="ctrl"></param>
		/// <param name="withGui">If true, a message box will ask the user for approval.</param>
		public static void DeleteActiveLayer(GraphController ctrl, bool withGui)
		{
			if (null == ctrl.ActiveLayer)
				return;

			if (withGui && false==Current.Gui.YesNoMessageBox("This will delete the active layer. Are you sure?", "Attention", false))
				return;

			ctrl.Doc.Layers.RemoveAt(ctrl.CurrentLayerNumber);
		}

		/// <summary>
		/// Deletes the active layer
		/// </summary>
		/// <param name="ctrl"></param>
		/// <param name="destposition"></param>
		public static void MoveActiveLayerToPosition(GraphController ctrl, int destposition)
		{
			XYPlotLayer layer = ctrl.ActiveLayer;
			if (null == layer)
				return;

			int currentpos = ctrl.CurrentLayerNumber;
			ctrl.Doc.Layers.RemoveAt(currentpos);
			
			if (destposition > currentpos)
				destposition--;
			ctrl.Doc.Layers.Insert(destposition, layer);
		}


		/// <summary>
		/// Deletes the active layer
		/// </summary>
		/// <param name="ctrl"></param>
		/// <param name="destposition"></param>
		public static void MoveActiveLayerToPosition(GraphController ctrl)
		{
			IntegerValueInputController ivictrl = new IntegerValueInputController(0, "Please enter the new position (>=0):");
			ivictrl.Validator = new IntegerValueInputController.ZeroOrPositiveIntegerValidator();
			int newposition;
			if (!Current.Gui.ShowDialog(ivictrl, "New position", false))
				return;

			newposition = ivictrl.EnteredContents;
			MoveActiveLayerToPosition(ctrl, newposition);
		}

    /// <summary>
    /// Copies the current graph as an bitmap image to the clipboard.
    /// </summary>
    /// <param name="ctrl">Controller controlling the current graph.</param>
    /// <param name="dpiResolution">Resolution of the bitmap in dpi. Determines the pixel size of the bitmap.</param>
    /// <param name="backbrush">Brush used to fill the background of the image. Can be <c>null</c>.</param>
    /// <param name="pixelformat">Specify the pixelformat here.</param>
    public static void CopyPageToClipboardAsBitmap(GraphController ctrl, int dpiResolution, Brush backbrush, PixelFormat pixelformat, GraphExportArea areaToExport)
    {
      System.Windows.Forms.DataObject dao = new System.Windows.Forms.DataObject();
			System.Drawing.Bitmap bitmap = GraphExport.RenderAsBitmap(ctrl.Doc, backbrush, pixelformat, areaToExport, dpiResolution, dpiResolution);
      dao.SetImage(bitmap);
      System.Windows.Forms.Clipboard.SetDataObject(dao);
    }


		public static void CopyPageToClipboardAsBitmap(GraphController ctrl, Brush backbrush, GraphExportOptions options)
		{
			System.Windows.Forms.DataObject dao = new System.Windows.Forms.DataObject();
			System.Drawing.Bitmap bitmap = GraphExport.RenderAsBitmap(ctrl.Doc, backbrush, options.PixelFormat, options.ExportArea, options.SourceDpiResolution, options.DestinationDpiResolution);
			dao.SetImage(bitmap);
			System.Windows.Forms.Clipboard.SetDataObject(dao);
		}

    /// <summary>
    /// This command will rescale all axes in all layers
    /// </summary>
    public static void RescaleAxes(GraphDocument doc)
    {
      for (int i = 0; i < doc.Layers.Count; i++)
      {
        doc.Layers[i].RescaleXAxis();
        doc.Layers[i].RescaleYAxis();
      }
    }

  }
}
