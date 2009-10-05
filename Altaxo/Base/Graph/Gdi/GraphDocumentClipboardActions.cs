using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

using System.Runtime.InteropServices;
using Altaxo.Graph.Gdi;
using Altaxo.Gui.Common;

namespace Altaxo.Graph.Gdi
{
	/// <summary>
	/// Extension methods for GraphDocument especially for clipboard actions.
	/// </summary>
	public static class GraphDocumentClipboardActions
	{
		public static readonly GraphCopyOptions DefaultGraphDocumentPasteOptions;
		public static readonly GraphCopyOptions DefaultGraphLayerPasteOptions;
		static GraphCopyOptions _lastChoosenGraphDocumentPasteOptions;
		static GraphCopyOptions _lastChoosenGraphLayerPasteOptions;

		static GraphDocumentClipboardActions()
		{
			DefaultGraphDocumentPasteOptions = GraphCopyOptions.All & ~GraphCopyOptions.CopyLayerPlotItems;
			DefaultGraphLayerPasteOptions = GraphCopyOptions.CopyLayerAll & ~GraphCopyOptions.CopyLayerPlotItems;
			_lastChoosenGraphDocumentPasteOptions = DefaultGraphDocumentPasteOptions;
			_lastChoosenGraphLayerPasteOptions = DefaultGraphLayerPasteOptions;
		}

		#region Image formats

		public static GraphExportOptions CopyPageOptions = new GraphExportOptions();


	
    /// <summary>
    /// Shows the copy page options dialog and stores the result as the static field <see cref="CopyPageOptions"/> here in this class
    /// </summary>
    /// <param name="doc">Ignored. Can be set to null.</param>
    /// <returns>True when the dialog was successfully closed, false otherwise.</returns>
		public static bool ShowCopyPageOptionsDialog(this GraphDocument doc)
		{
			if (null == CopyPageOptions)
				CopyPageOptions = new GraphExportOptions();

      CopyPageOptions.IsIntentedForClipboardOperation = true;

			object resultobj = CopyPageOptions;
			if (Current.Gui.ShowDialog(ref resultobj, "Set copy page options"))
			{
				CopyPageOptions = (GraphExportOptions)resultobj;
				return true;
			}
			return false;
		}

    /// <summary>
    /// Copies the current graph as image to the clipboard using the <see cref="CopyPageOptions"/> stored here in this class.
    /// </summary>
    /// <param name="doc"></param>
		static public void CopyToClipboardAsImage(this GraphDocument doc)
		{
			var opt = CopyPageOptions;
			if (opt.ImageFormat == ImageFormat.Emf || opt.ImageFormat == ImageFormat.Wmf)
			{
				CopyToClipboardAsMetafile(doc);
			}
			else
			{
				CopyToClipboardAsBitmap(doc);
			}
		}

		/// <summary>
		/// Copies the current graph as an bitmap image to the clipboard in native bmp format.
		/// </summary>
		/// <param name="ctrl">Controller controlling the current graph.</param>
		/// <param name="dpiResolution">Resolution of the bitmap in dpi. Determines the pixel size of the bitmap.</param>
		/// <param name="backbrush">Brush used to fill the background of the image. Can be <c>null</c>.</param>
		/// <param name="pixelformat">Specify the pixelformat here.</param>
		public static void CopyToClipboardAsBitmap(this GraphDocument doc, int dpiResolution, Brush backbrush, PixelFormat pixelformat, GraphExportArea areaToExport)
		{
			var dao = Current.Gui.GetNewClipboardDataObject();
			System.Drawing.Bitmap bitmap = GraphDocumentExportActions.RenderAsBitmap(doc, backbrush, pixelformat, areaToExport, dpiResolution, dpiResolution);
			dao.SetImage(bitmap);
			Current.Gui.SetClipboardDataObject(dao);
		}

    /// <summary>
    /// Copies the current graph as an bitmap either in native format or as DropDownList or both to the clipboard.
    /// </summary>
    /// <param name="doc">The graph document to copy.</param>
    /// <param name="options">Graph copy options.</param>
		public static void CopyToClipboardAsBitmap(this GraphDocument doc, GraphExportOptions options)
		{
			var dao = Current.Gui.GetNewClipboardDataObject();
			System.Drawing.Bitmap bitmap = GraphDocumentExportActions.RenderAsBitmap(doc, options.BackgroundBrush, options.PixelFormat, options.ExportArea, options.SourceDpiResolution, options.DestinationDpiResolution);

      if(GraphCopyPageClipboardFormat.AsNative == (options.ClipboardFormat & GraphCopyPageClipboardFormat.AsNative))
  			dao.SetImage(bitmap);
      if (GraphCopyPageClipboardFormat.AsDropDownList == (options.ClipboardFormat & GraphCopyPageClipboardFormat.AsDropDownList))
        InternalAddClipboardDropDownList(dao, bitmap, options);

			Current.Gui.SetClipboardDataObject(dao);
		}


    static string InternalAddClipboardDropDownList(Altaxo.Gui.IClipboardSetDataObject dao, System.Drawing.Bitmap bmp, GraphExportOptions options)
    {

      string filepath = System.IO.Path.GetTempPath();
      string filename = filepath + "AltaxoGraphCopyPage" + options.GetDefaultFileNameExtension(); ;
      if (System.IO.File.Exists(filename))
        System.IO.File.Delete(filename);

      bmp.Save(filename, options.ImageFormat);

      System.Collections.Specialized.StringCollection coll = new System.Collections.Specialized.StringCollection();
      coll.Add(filename);
      dao.SetFileDropList(coll);

      return filename;
    }

		static public void CopyToClipboardAsBitmap(this GraphDocument doc)
		{
			CopyToClipboardAsBitmap(doc, CopyPageOptions);
		}


    static public void CopyToClipboardAsMetafile(this GraphDocument doc)
    {
      CopyToClipboardAsMetafile(doc, CopyPageOptions);
    }

		static public void CopyToClipboardAsMetafile(this GraphDocument doc, GraphExportOptions options)
		{
			var dao = Current.Gui.GetNewClipboardDataObject();
			string filepath = System.IO.Path.GetTempPath();
			string filename = filepath + "AltaxoClipboardMetafile.emf";
			if (System.IO.File.Exists(filename))
				System.IO.File.Delete(filename);
			Metafile mf = GraphDocumentExportActions.RenderAsMetafile(doc, filename, options);

      if (GraphCopyPageClipboardFormat.AsNative == (options.ClipboardFormat & GraphCopyPageClipboardFormat.AsNative))
      {
        dao.SetData(typeof(Metafile), mf);
      }

      if (GraphCopyPageClipboardFormat.AsDropDownList == (options.ClipboardFormat & GraphCopyPageClipboardFormat.AsDropDownList))
      {
        System.Collections.Specialized.StringCollection coll = new System.Collections.Specialized.StringCollection();
        coll.Add(filename);
        dao.SetFileDropList(coll);
      }
      Current.Gui.SetClipboardDataObject(dao);
		}
   


	

		#endregion

		#region native formats

		/// <summary>
		/// Puts the entire graph to the clipboard in XML format.
		/// </summary>
		/// <param name="ctrl">Graph controller.</param>
		public static void CopyToClipboardAsNative(this GraphDocument doc)
		{
			Serialization.ClipboardSerialization.PutObjectToClipboard("Altaxo.Graph.GraphDocumentAsXml", doc);
		}

		/// <summary>
		/// Try to paste the entire GraphDocument from the clipboard using the specified paste options.
		/// </summary>
		/// <param name="doc">The graph document to paste into.</param>
		/// <param name="options">The options used for paste into that graph.</param>
		public static void PasteFromClipboard(this GraphDocument doc, GraphCopyOptions options)
		{
			object from = Serialization.ClipboardSerialization.GetObjectFromClipboard("Altaxo.Graph.GraphDocumentAsXml");
			if (from is GraphDocument)
			{
				doc.CopyFrom((GraphDocument)from, options);
				doc.RescaleAxes();
			}
		}


		/// <summary>
		/// Try to paste the entire GraphDocument from the clipboard.
		/// </summary>
		/// <param name="doc">The graph document to paste into.</param>
		public static void PasteFromClipboardAsGraphStyle(this GraphDocument doc, bool showOptionsDialog)
		{
			object from = Serialization.ClipboardSerialization.GetObjectFromClipboard("Altaxo.Graph.GraphDocumentAsXml");
      if (from is GraphDocument)
      {
				GraphCopyOptions options = DefaultGraphDocumentPasteOptions;
				if (showOptionsDialog)
				{
					System.Enum o = _lastChoosenGraphDocumentPasteOptions;
					if (!Current.Gui.ShowDialogForEnumFlag(ref o, "Choose options for pasting"))
						return;
					_lastChoosenGraphDocumentPasteOptions = (GraphCopyOptions)o;
					options = _lastChoosenGraphDocumentPasteOptions;
				}
				PasteFromClipboard(doc, options);
      }
		}


		/// <summary>
		/// Puts the active layer to the clipboard in XML format.
		/// </summary>
		/// <param name="ctrl">Graph controller.</param>
		public static void CopyToClipboardLayerAsNative(this GraphDocument doc, int layerNumber)
		{
			Serialization.ClipboardSerialization.PutObjectToClipboard("Altaxo.Graph.GraphLayerAsXml", doc.Layers[layerNumber]);
		}

		public static void PasteFromClipboardAsTemplateForLayer(GraphDocument doc, int layerNumber, GraphCopyOptions options)
		{
			object o = Serialization.ClipboardSerialization.GetObjectFromClipboard("Altaxo.Graph.GraphLayerAsXml");
			if (null == o)
				return;
			XYPlotLayer layer = o as XYPlotLayer;
			if (null != layer)
			{
				doc.Layers[layerNumber].CopyFrom(layer, options);
				doc.RescaleAxes();
			}
		}


		

		public static void PasteFromClipboardAsTemplateForLayer(this GraphDocument doc, int layerNumber)
		{
      /*
			object options = new PasteLayerOptions() { PastePlotStyles = true, PastePlotItems = true };
			if (false == Current.Gui.ShowDialog(ref options, "Choose what to paste"))
				return;
  			PasteFromClipboardAsTemplateForLayer(doc, layerNumber, (options as PasteLayerOptions).GetCopyOptions());
       * 
      */

			GraphCopyOptions options = _lastChoosenGraphLayerPasteOptions;
      System.Enum options1 = options;
      if (Current.Gui.ShowDialogForEnumFlag(ref options1, "Choose paste options"))
      {
        options = (GraphCopyOptions)options1;
				_lastChoosenGraphLayerPasteOptions = options;
        PasteFromClipboardAsTemplateForLayer(doc, layerNumber, options);
      }
    }

		public static void PasteFromClipboardAsNewLayer(this GraphDocument doc)
		{
			object o = Serialization.ClipboardSerialization.GetObjectFromClipboard("Altaxo.Graph.GraphLayerAsXml");
			XYPlotLayer layer = o as XYPlotLayer;
			if (null != layer)
				doc.Layers.Add(layer);
		}

		/// <summary>
		/// Pastes a layer on the clipboard as new layer before the active layer.
		/// </summary>
		/// <param name="ctrl"></param>
		public static void PasteFromClipboardAsNewLayerBeforeLayerNumber(this GraphDocument doc, int currentActiveLayerNumber)
		{
			object o = Serialization.ClipboardSerialization.GetObjectFromClipboard("Altaxo.Graph.GraphLayerAsXml");
			XYPlotLayer layer = o as XYPlotLayer;
			if (null != layer)
			{
				doc.Layers.Insert(currentActiveLayerNumber, layer);
			}
		}


		/// <summary>
		/// Pastes a layer on the clipboard as new layer after the active layer.
		/// </summary>
		/// <param name="ctrl"></param>
		public static void PasteFromClipboardAsNewLayerAfterLayerNumber(this GraphDocument doc, int currentActiveLayerNumber)
		{
			object o = Serialization.ClipboardSerialization.GetObjectFromClipboard("Altaxo.Graph.GraphLayerAsXml");
			XYPlotLayer layer = o as XYPlotLayer;
			if (null != layer)
			{
				doc.Layers.Insert(currentActiveLayerNumber + 1, layer);
			}
		}


#endregion



	}
}
