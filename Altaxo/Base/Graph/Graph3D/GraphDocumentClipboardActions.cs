#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2016 Dr. Dirk Lellinger
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
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;

namespace Altaxo.Graph.Graph3D
{
	using Altaxo.Serialization.Clipboard;
	using ClipboardRenderingOptions = Altaxo.Graph.Gdi.ClipboardRenderingOptions;
	using GraphCopyOptions = Altaxo.Graph.Gdi.GraphCopyOptions;
	using GraphExportOptions = Altaxo.Graph.Gdi.GraphExportOptions;

	/// <summary>
	/// Extension methods for GraphDocument especially for clipboard actions.
	/// </summary>
	public static class GraphDocumentClipboardActions
	{
		public static readonly GraphCopyOptions DefaultGraphDocumentPasteOptions;
		public static readonly GraphCopyOptions DefaultGraphLayerPasteOptions;
		private static GraphCopyOptions _lastChoosenGraphDocumentPasteOptions;
		private static GraphCopyOptions _lastChoosenGraphLayerPasteOptions;
		//	public static readonly PropertyKey<GraphClipboardExportOptions> PropertyKeyCopyPageSettings = new PropertyKey<GraphClipboardExportOptions>("DE1819F6-7E8C-4C43-9984-B5C405236289", "Graph\\CopyPageOptions", PropertyLevel.All, typeof(GraphDocument), () => new GraphClipboardExportOptions());

		static GraphDocumentClipboardActions()
		{
			DefaultGraphDocumentPasteOptions = GraphCopyOptions.All & ~GraphCopyOptions.CopyLayerPlotItems;
			DefaultGraphLayerPasteOptions = GraphCopyOptions.CopyLayerAll & ~GraphCopyOptions.CopyLayerPlotItems;
			_lastChoosenGraphDocumentPasteOptions = DefaultGraphDocumentPasteOptions;
			_lastChoosenGraphLayerPasteOptions = DefaultGraphLayerPasteOptions;
		}

		#region Image formats

		/// <summary>
		/// Shows the copy page options dialog and stores the result as the static field <see cref="P:ClipboardRenderingOptions.CopyPageOptions"/> here in this class
		/// </summary>
		/// <param name="doc">Ignored. Can be set to null.</param>
		/// <returns>True when the dialog was successfully closed, false otherwise.</returns>
		public static bool ShowCopyPageOptionsDialog(this GraphDocument doc)
		{
			object resultobj = ClipboardRenderingOptions.CopyPageOptions;
			if (Current.Gui.ShowDialog(ref resultobj, "Set copy page options"))
			{
				ClipboardRenderingOptions.CopyPageOptions = (ClipboardRenderingOptions)resultobj;
				return true;
			}
			return false;
		}

		/// <summary>
		/// Copies the current graph as an bitmap image to the clipboard in native bmp format.
		/// </summary>
		/// <param name="doc">Graph to copy to the clipboard.</param>
		/// <param name="dpiResolution">Resolution of the bitmap in dpi. Determines the pixel size of the bitmap.</param>
		/// <param name="backbrush">Brush used to fill the background of the image. Can be <c>null</c>.</param>
		/// <param name="pixelformat">Specify the pixelformat here.</param>
		public static void CopyToClipboardAsBitmap(this GraphDocument doc, int dpiResolution, Altaxo.Graph.Gdi.BrushX backbrush, PixelFormat pixelformat)
		{
			var dao = Current.Gui.GetNewClipboardDataObject();
			System.Drawing.Bitmap bitmap = GraphDocumentExportActions.RenderAsBitmap(doc, backbrush, pixelformat, dpiResolution, dpiResolution);
			dao.SetImage(bitmap);
			Current.Gui.SetClipboardDataObject(dao);
		}

		private static string InternalAddClipboardDropDownList(Altaxo.Gui.IClipboardSetDataObject dao, System.Drawing.Bitmap bmp, GraphExportOptions options)
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

		#endregion Image formats

		#region native formats

		/// <summary>
		/// Puts the entire graph to the clipboard in XML format.
		/// </summary>
		/// <param name="doc">Graph to copy to the clipboard.</param>
		public static void CopyToClipboardAsNative(this GraphDocument doc)
		{
			ClipboardSerialization.PutObjectToClipboard("Altaxo.Graph.Graph3D.GraphDocumentAsXml", doc);
		}

		/// <summary>
		/// Try to paste the entire GraphDocument from the clipboard using the specified paste options.
		/// </summary>
		/// <param name="doc">The graph document to paste into.</param>
		/// <param name="options">The options used for paste into that graph.</param>
		public static void PasteFromClipboard(this GraphDocument doc, GraphCopyOptions options)
		{
			object from = ClipboardSerialization.GetObjectFromClipboard("Altaxo.Graph.Graph3D.GraphDocumentAsXml");
			if (from is GraphDocument)
			{
				doc.CopyFrom((GraphDocument)from, options);
			}
		}

		/// <summary>
		/// Try to paste the entire GraphDocument from the clipboard.
		/// </summary>
		/// <param name="doc">The graph document to paste into.</param>
		/// <param name="showOptionsDialog">If <c>true</c>, shows the user an option dialog for choise of specific items to paste.</param>
		public static void PasteFromClipboardAsGraphStyle(this GraphDocument doc, bool showOptionsDialog)
		{
			object from = ClipboardSerialization.GetObjectFromClipboard("Altaxo.Graph.Graph3D.GraphDocumentAsXml");
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
		/// Puts the layer with index <paramref name="layerNumber"/> to the clipboard in XML format.
		/// </summary>
		/// <param name="doc">Graph document to copy.</param>
		/// <param name="layerNumber">Number of the layer to copy.</param>
		public static void CopyToClipboardLayerAsNative(this GraphDocument doc, IEnumerable<int> layerNumber)
		{
			ClipboardSerialization.PutObjectToClipboard("Altaxo.Graph.Graph3D.GraphLayerAsXml", doc.RootLayer.ElementAt(layerNumber));
		}

		public static void PasteFromClipboardAsTemplateForLayer(GraphDocument doc, IEnumerable<int> layerNumber, GraphCopyOptions options)
		{
			object o = ClipboardSerialization.GetObjectFromClipboard("Altaxo.Graph.Graph3D.GraphLayerAsXml");
			if (null == o)
				return;
			XYZPlotLayer layer = o as XYZPlotLayer;
			if (null != layer)
			{
				doc.RootLayer.ElementAt(layerNumber).CopyFrom(layer, options);
			}
		}

		public static void PasteFromClipboardAsTemplateForLayer(this GraphDocument doc, IEnumerable<int> layerNumber)
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
			object o = ClipboardSerialization.GetObjectFromClipboard("Altaxo.Graph.Graph3D.GraphLayerAsXml");
			var layer = o as XYZPlotLayer;
			if (null != layer)
				doc.RootLayer.Layers.Add(layer);
		}

		/// <summary>
		/// Pastes a layer on the clipboard as new layer before the layer specified by index <paramref name="currentActiveLayerNumber"/>.
		/// </summary>
		/// <param name="doc">Graph document in which to paste.</param>
		/// <param name="currentActiveLayerNumber">Index of the layer follows after the pasted layer.</param>
		public static void PasteFromClipboardAsNewLayerBeforeLayerNumber(this GraphDocument doc, IEnumerable<int> currentActiveLayerNumber)
		{
			object o = ClipboardSerialization.GetObjectFromClipboard("Altaxo.Graph.Graph3D.GraphLayerAsXml");
			var layer = o as XYZPlotLayer;
			if (null != layer)
			{
				doc.RootLayer.Insert(currentActiveLayerNumber, layer);
			}
		}

		/// <summary>
		/// Pastes a layer on the clipboard as new layer after the layer at index <paramref name="currentActiveLayerNumber"/>.
		/// </summary>
		/// <param name="doc">Graph document to paste to.</param>
		/// <param name="currentActiveLayerNumber">Index of the layer after which to paste the layer from the clipboard.</param>
		public static void PasteFromClipboardAsNewLayerAfterLayerNumber(this GraphDocument doc, IEnumerable<int> currentActiveLayerNumber)
		{
			object o = ClipboardSerialization.GetObjectFromClipboard("Altaxo.Graph.Graph3D.GraphLayerAsXml");
			var layer = o as XYZPlotLayer;
			if (null != layer)
			{
				doc.RootLayer.InsertAfter(currentActiveLayerNumber, layer);
			}
		}

		/// <summary>
		/// Pastes a layer on the clipboard as new layer after the layer at index <paramref name="currentActiveLayerNumber"/>.
		/// </summary>
		/// <param name="doc">Graph document to paste to.</param>
		/// <param name="currentActiveLayerNumber">Index of the layer after which to paste the layer from the clipboard.</param>
		public static void PasteFromClipboardAsNewChildLayerOfLayerNumber(this GraphDocument doc, IEnumerable<int> currentActiveLayerNumber)
		{
			object o = ClipboardSerialization.GetObjectFromClipboard("Altaxo.Graph.Graph3D.GraphLayerAsXml");
			var layer = o as XYZPlotLayer;
			if (null != layer)
			{
				var parentLayer = doc.RootLayer.ElementAt(currentActiveLayerNumber);
				parentLayer.Layers.Insert(0, layer);
			}
		}

		#endregion native formats
	}
}