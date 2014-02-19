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
using Altaxo.Graph.Gdi;
using Altaxo.Gui.Common;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Altaxo.Graph.Gdi
{
	using Altaxo.Main.Properties;

	/// <summary>
	/// Extension methods for GraphDocument especially for clipboard actions.
	/// </summary>
	public static class GraphDocumentClipboardActions
	{
		public static readonly GraphCopyOptions DefaultGraphDocumentPasteOptions;
		public static readonly GraphCopyOptions DefaultGraphLayerPasteOptions;
		private static GraphCopyOptions _lastChoosenGraphDocumentPasteOptions;
		private static GraphCopyOptions _lastChoosenGraphLayerPasteOptions;
		public static readonly PropertyKey<GraphClipboardExportOptions> PropertyKeyCopyPageSettings = new PropertyKey<GraphClipboardExportOptions>("DE1819F6-7E8C-4C43-9984-B5C405236289", "Graph\\CopyPageOptions", PropertyLevel.All, typeof(GraphDocument), () => new GraphClipboardExportOptions());

		public static GraphClipboardExportOptions CopyPageOptions
		{
			get
			{
				var doc = Current.PropertyService.GetValue(GraphDocumentClipboardActions.PropertyKeyCopyPageSettings, Altaxo.Main.Services.RuntimePropertyKind.UserAndApplicationAndBuiltin, () => new GraphClipboardExportOptions());
				return doc;
			}
			set
			{
				if (null == value)
					throw new ArgumentNullException();

				Current.PropertyService.UserSettings.SetValue(GraphDocumentClipboardActions.PropertyKeyCopyPageSettings, value);
			}
		}

		static GraphDocumentClipboardActions()
		{
			DefaultGraphDocumentPasteOptions = GraphCopyOptions.All & ~GraphCopyOptions.CopyLayerPlotItems;
			DefaultGraphLayerPasteOptions = GraphCopyOptions.CopyLayerAll & ~GraphCopyOptions.CopyLayerPlotItems;
			_lastChoosenGraphDocumentPasteOptions = DefaultGraphDocumentPasteOptions;
			_lastChoosenGraphLayerPasteOptions = DefaultGraphLayerPasteOptions;
		}

		#region Image formats

		/// <summary>
		/// Shows the copy page options dialog and stores the result as the static field <see cref="CopyPageOptions"/> here in this class
		/// </summary>
		/// <param name="doc">Ignored. Can be set to null.</param>
		/// <returns>True when the dialog was successfully closed, false otherwise.</returns>
		public static bool ShowCopyPageOptionsDialog(this GraphDocument doc)
		{
			object resultobj = CopyPageOptions;
			if (Current.Gui.ShowDialog(ref resultobj, "Set copy page options"))
			{
				CopyPageOptions = (GraphClipboardExportOptions)resultobj;
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
		/// Get the image that should be placed on the clipboard.
		/// </summary>
		/// <param name="doc">The graph document to render.</param>
		public static Image GetImageForClipboard(this GraphDocument doc)
		{
			var options = CopyPageOptions;
			if (options.ImageFormat == ImageFormat.Emf || options.ImageFormat == ImageFormat.Wmf)
			{
				return GraphDocumentExportActions.RenderAsMetafile(doc, null, options.BackgroundBrush, options.PixelFormat, options.SourceDpiResolution, options.DestinationDpiResolution);
			}
			else
			{
				return GraphDocumentExportActions.RenderAsBitmap(doc, options.BackgroundBrush, options.PixelFormat, options.SourceDpiResolution, options.DestinationDpiResolution);
			}
		}

		/// <summary>
		/// Renders the graph document to an image intended for the clipboard. If the image should also be copied as dropdown file, the image is saved into a temporary file,
		/// and the fileName of the temporary file is returned as output parameter.
		/// </summary>
		/// <param name="doc">The graph document to export.</param>
		/// <param name="image">Output: the rendered image of the graph document. Is either a metafile or a bitmap dependend on the export options.</param>
		/// <param name="fileName">Output: name of the file for the dropdown list, or null if the image should not be copyied as dropdown file.</param>
		public static void GetImageForClipboard(this GraphDocument doc, out Image image, out string fileName)
		{
			GetImageForClipboard(doc, CopyPageOptions, out image, out fileName);
		}

		/// <summary>
		/// Renders the graph document to an image intended for the clipboard. If the image should also be copied as dropdown file, the image is saved into a temporary file,
		/// and the fileName of the temporary file is returned as output parameter.
		/// </summary>
		/// <param name="doc">The graph document to export.</param>
		/// <param name="image">Output: the rendered image of the graph document. Is either a metafile or a bitmap dependend on the export options.</param>
		/// <param name="fileName">Output: name of the file for the dropdown list, or null if the image should not be copyied as dropdown file.</param>
		/// <param name="options">The export and render options for the graph document.</param>
		public static void GetImageForClipboard(this GraphDocument doc, GraphClipboardExportOptions options, out Image image, out string fileName)
		{
			bool isMetafile = options.ImageFormat == ImageFormat.Emf || options.ImageFormat == ImageFormat.Wmf;

			fileName = null;
			if (options.ClipboardFormat.HasFlag(GraphCopyPageClipboardFormat.AsDropDownList))
			{
				string filepath = System.IO.Path.GetTempPath();
				if (isMetafile)
					fileName = filepath + "AltaxoClipboardMetafile.emf";
				else
					fileName = filepath + "AltaxoGraphCopyPage" + options.GetDefaultFileNameExtension();

				if (System.IO.File.Exists(fileName))
					System.IO.File.Delete(fileName);
			}

			if (isMetafile)
			{
				Metafile mf;
				if (!string.IsNullOrEmpty(fileName))
				{
					using (System.IO.Stream str = new System.IO.FileStream(fileName, System.IO.FileMode.CreateNew, System.IO.FileAccess.Write, System.IO.FileShare.Read))
					{
						mf = GraphDocumentExportActions.RenderAsMetafile(doc, str, options.BackgroundBrush, options.PixelFormat, options.SourceDpiResolution, options.DestinationDpiResolution);
						str.Close();
					}
				}
				else
				{
					mf = GraphDocumentExportActions.RenderAsMetafile(doc, null, options.BackgroundBrush, options.PixelFormat, options.SourceDpiResolution, options.DestinationDpiResolution);
				}
				image = mf;
			}
			else
			{
				System.Drawing.Bitmap bitmap = GraphDocumentExportActions.RenderAsBitmap(doc, options.BackgroundBrush, options.PixelFormat, options.SourceDpiResolution, options.DestinationDpiResolution);

				if (!string.IsNullOrEmpty(fileName))
					bitmap.Save(fileName, options.ImageFormat);

				image = bitmap;
			}
		}

		/// <summary>
		/// Copies the current graph as an bitmap image to the clipboard in native bmp format.
		/// </summary>
		/// <param name="doc">Graph to copy to the clipboard.</param>
		/// <param name="dpiResolution">Resolution of the bitmap in dpi. Determines the pixel size of the bitmap.</param>
		/// <param name="backbrush">Brush used to fill the background of the image. Can be <c>null</c>.</param>
		/// <param name="pixelformat">Specify the pixelformat here.</param>
		public static void CopyToClipboardAsBitmap(this GraphDocument doc, int dpiResolution, Brush backbrush, PixelFormat pixelformat)
		{
			var dao = Current.Gui.GetNewClipboardDataObject();
			System.Drawing.Bitmap bitmap = GraphDocumentExportActions.RenderAsBitmap(doc, backbrush, pixelformat, dpiResolution, dpiResolution);
			dao.SetImage(bitmap);
			Current.Gui.SetClipboardDataObject(dao);
		}

		/// <summary>
		/// Copies the current graph as an bitmap either in native format or as DropDownList or both to the clipboard.
		/// </summary>
		/// <param name="doc">The graph document to copy.</param>
		/// <param name="options">Graph copy options.</param>
		public static void CopyToClipboardAsBitmap(this GraphDocument doc, GraphClipboardExportOptions options)
		{
			var dao = Current.Gui.GetNewClipboardDataObject();
			System.Drawing.Bitmap bitmap = GraphDocumentExportActions.RenderAsBitmap(doc, options.BackgroundBrush, options.PixelFormat, options.SourceDpiResolution, options.DestinationDpiResolution);

			if (GraphCopyPageClipboardFormat.AsNative == (options.ClipboardFormat & GraphCopyPageClipboardFormat.AsNative))
				dao.SetImage(bitmap);
			if (GraphCopyPageClipboardFormat.AsDropDownList == (options.ClipboardFormat & GraphCopyPageClipboardFormat.AsDropDownList))
				InternalAddClipboardDropDownList(dao, bitmap, options);

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

		static public void CopyToClipboardAsBitmap(this GraphDocument doc)
		{
			CopyToClipboardAsBitmap(doc, CopyPageOptions);
		}

		static public void CopyToClipboardAsMetafile(this GraphDocument doc)
		{
			CopyToClipboardAsMetafile(doc, CopyPageOptions);
		}

		static public void CopyToClipboardAsMetafile(this GraphDocument doc, GraphClipboardExportOptions options)
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

		#endregion Image formats

		#region native formats

		/// <summary>
		/// Puts the entire graph to the clipboard in XML format.
		/// </summary>
		/// <param name="doc">Graph to copy to the clipboard.</param>
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
		/// <param name="showOptionsDialog">If <c>true</c>, shows the user an option dialog for choise of specific items to paste.</param>
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
		/// Puts the layer with index <paramref name="layerNumber"/> to the clipboard in XML format.
		/// </summary>
		/// <param name="doc">Graph document to copy.</param>
		/// <param name="layerNumber">Number of the layer to copy.</param>
		public static void CopyToClipboardLayerAsNative(this GraphDocument doc, IEnumerable<int> layerNumber)
		{
			Serialization.ClipboardSerialization.PutObjectToClipboard("Altaxo.Graph.GraphLayerAsXml", doc.RootLayer.ElementAt(layerNumber));
		}

		public static void PasteFromClipboardAsTemplateForLayer(GraphDocument doc, IEnumerable<int> layerNumber, GraphCopyOptions options)
		{
			object o = Serialization.ClipboardSerialization.GetObjectFromClipboard("Altaxo.Graph.GraphLayerAsXml");
			if (null == o)
				return;
			XYPlotLayer layer = o as XYPlotLayer;
			if (null != layer)
			{
				doc.RootLayer.ElementAt(layerNumber).CopyFrom(layer, options);
				doc.RescaleAxes();
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
			object o = Serialization.ClipboardSerialization.GetObjectFromClipboard("Altaxo.Graph.GraphLayerAsXml");
			XYPlotLayer layer = o as XYPlotLayer;
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
			object o = Serialization.ClipboardSerialization.GetObjectFromClipboard("Altaxo.Graph.GraphLayerAsXml");
			XYPlotLayer layer = o as XYPlotLayer;
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
			object o = Serialization.ClipboardSerialization.GetObjectFromClipboard("Altaxo.Graph.GraphLayerAsXml");
			XYPlotLayer layer = o as XYPlotLayer;
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
			object o = Serialization.ClipboardSerialization.GetObjectFromClipboard("Altaxo.Graph.GraphLayerAsXml");
			XYPlotLayer layer = o as XYPlotLayer;
			if (null != layer)
			{
				var parentLayer = doc.RootLayer.ElementAt(currentActiveLayerNumber);
				parentLayer.Layers.Insert(0, layer);
			}
		}

		#endregion native formats
	}
}