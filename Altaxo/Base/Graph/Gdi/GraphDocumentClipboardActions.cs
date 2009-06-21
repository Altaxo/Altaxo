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
		#region Image formats

		public static GraphExportOptions CopyPageOptions = new GraphExportOptions();


	

		public static bool ShowCopyPageOptionsDialog(this GraphDocument doc)
		{
			if (null == CopyPageOptions)
				CopyPageOptions = new GraphExportOptions();

			object resultobj = CopyPageOptions;
			if (Current.Gui.ShowDialog(ref resultobj, "Set copy page options"))
			{
				CopyPageOptions = (GraphExportOptions)resultobj;
				return true;
			}
			return false;
		}

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
		/// Copies the current graph as an bitmap image to the clipboard.
		/// </summary>
		/// <param name="ctrl">Controller controlling the current graph.</param>
		/// <param name="dpiResolution">Resolution of the bitmap in dpi. Determines the pixel size of the bitmap.</param>
		/// <param name="backbrush">Brush used to fill the background of the image. Can be <c>null</c>.</param>
		/// <param name="pixelformat">Specify the pixelformat here.</param>
		public static void CopyToClipboardAsBitmap(this GraphDocument doc, int dpiResolution, Brush backbrush, PixelFormat pixelformat, GraphExportArea areaToExport)
		{
			System.Windows.Forms.DataObject dao = new System.Windows.Forms.DataObject();
			System.Drawing.Bitmap bitmap = GraphDocumentExportActions.RenderAsBitmap(doc, backbrush, pixelformat, areaToExport, dpiResolution, dpiResolution);
			dao.SetImage(bitmap);
			System.Windows.Forms.Clipboard.SetDataObject(dao);
		}


		public static void CopyToClipboardAsBitmap(this GraphDocument doc, Brush backbrush, GraphExportOptions options)
		{
			System.Windows.Forms.DataObject dao = new System.Windows.Forms.DataObject();
			System.Drawing.Bitmap bitmap = GraphDocumentExportActions.RenderAsBitmap(doc, backbrush, options.PixelFormat, options.ExportArea, options.SourceDpiResolution, options.DestinationDpiResolution);
			dao.SetImage(bitmap);
			System.Windows.Forms.Clipboard.SetDataObject(dao);
		}

		static public void CopyToClipboardAsBitmap(this GraphDocument doc)
		{
			CopyToClipboardAsBitmap(doc, Brushes.White, CopyPageOptions);
		}



		static public void CopyToClipboardAsMetafile(this GraphDocument doc)
		{
			// System.Drawing.Imaging.Metafile mf = Altaxo.Graph.Procedures.Export.GetMetafile(ctrl.Doc);
			// PutEnhMetafileOnClipboard(ctrl.View.Form.Handle, mf);

			System.Windows.Forms.DataObject dao = new System.Windows.Forms.DataObject();
			string filepath = System.IO.Path.GetTempPath();
			string filename = filepath + "AltaxoClipboardMetafile.emf";
			if (System.IO.File.Exists(filename))
				System.IO.File.Delete(filename);
			Metafile mf = GraphDocumentExportActions.RenderAsMetafile(doc, filename, CopyPageOptions);
			System.Collections.Specialized.StringCollection coll = new System.Collections.Specialized.StringCollection();
			coll.Add(filename);
			dao.SetFileDropList(coll);
			dao.SetData(typeof(Metafile), mf);
			System.Windows.Forms.Clipboard.SetDataObject(dao);
		}


	

		#endregion

		#region native formats


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

		/// <summary>
		/// Puts the entire graph to the clipboard in XML format.
		/// </summary>
		/// <param name="ctrl">Graph controller.</param>
		public static void CopyToClipboardAsNative(this GraphDocument doc)
		{
			Serialization.ClipboardSerialization.PutObjectToClipboard("Altaxo.Graph.GraphDocumentAsXml", doc);
		}



		public static void PasteFromClipboardAsGraphStyle(this GraphDocument doc)
		{
			object o = Serialization.ClipboardSerialization.GetObjectFromClipboard("Altaxo.Graph.GraphDocumentAsXml");
			if (!(o is GraphDocument))
				return;

			GraphDocument from = (GraphDocument)o;
			doc.CopyFrom(from, GraphCopyOptions.CopyFromLayers | GraphCopyOptions.CopyPlotStyles);
			doc.RescaleAxes();
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
			object options = new PasteLayerOptions() { PastePlotStyles = true, PastePlotItems = true };
			if (false == Current.Gui.ShowDialog(ref options, "Choose what to paste"))
				return;
			PasteFromClipboardAsTemplateForLayer(doc, layerNumber, (options as PasteLayerOptions).GetCopyOptions());
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
