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

using Altaxo.Gui.Common.MultiRename;
using Altaxo.Main.Commands;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;

namespace Altaxo.Graph.Gdi
{
	public static class GraphDocumentExportActions
	{
		private static IList<KeyValuePair<string, string>> GetFileFilterString(ImageFormat fmt)
		{
			List<KeyValuePair<string, string>> filter = new List<KeyValuePair<string, string>>();

			if (fmt == ImageFormat.Bmp)
				filter.Add(new KeyValuePair<string, string>("*.bmp", "Bitmap files (*.bmp)"));
			else if (fmt == ImageFormat.Emf)
				filter.Add(new KeyValuePair<string, string>("*.emf", "Enhanced metafiles (*.emf)"));
			else if (ImageFormat.Exif == fmt)
				filter.Add(new KeyValuePair<string, string>("*.exi", "Exif files (*.exi)"));
			else if (ImageFormat.Gif == fmt)
				filter.Add(new KeyValuePair<string, string>("*.gif", "Gif files (*.gif)"));
			else if (ImageFormat.Icon == fmt)
				filter.Add(new KeyValuePair<string, string>("*.ico", "Icon files (*.ico)"));
			else if (ImageFormat.Jpeg == fmt)
				filter.Add(new KeyValuePair<string, string>("*.jpg", "Jpeg files (*.jpg)"));
			else if (ImageFormat.Png == fmt)
				filter.Add(new KeyValuePair<string, string>("*.png", "Png files (*.png)"));
			else if (ImageFormat.Tiff == fmt)
				filter.Add(new KeyValuePair<string, string>("*.tif", "Tiff files (*.tif)"));
			else if (ImageFormat.Wmf == fmt)
				filter.Add(new KeyValuePair<string, string>("*.wmf", "Windows metafiles (*.wmf)"));

			filter.Add(new KeyValuePair<string, string>("*.*", "All files (*.*)"));

			return filter;
		}

		private static GraphExportOptions _graphExportOptionsToFile = new GraphExportOptions();

		/// <summary>Shows the dialog to choose the graph export options, and then the multi file export dialog.</summary>
		/// <param name="documents">List with graph documents to export.</param>
		public static void ShowExportMultipleGraphsDialogAndExportOptions(IEnumerable<Graph.Gdi.GraphDocument> documents)
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

			ShowExportMultipleGraphsDialog(documents);
		}

		/// <summary>Shows the multi file export dialog and exports the graphs, using the <see cref="GraphExportOptions"/> that are stored in this class.</summary>
		/// <param name="documents">List with graph documents to export.</param>
		public static void ShowExportMultipleGraphsDialog(IEnumerable<Graph.Gdi.GraphDocument> documents)
		{
			MultiRenameData mrData = new MultiRenameData();
			MultiRenameDocuments.RegisterCommonDocumentShortcuts(mrData);
			mrData.RegisterStringShortcut("E", (o, i) => _graphExportOptionsToFile.GetDefaultFileNameExtension(), "File extension (depends on the image type that was chosen before");

			mrData.RegisterRenameActionHandler(DoExportGraphs);

			mrData.AddObjectsToRename(documents);

			mrData.RegisterListColumn("FullName", MultiRenameDocuments.GetFullName);
			mrData.RegisterListColumn("File name", null);
			mrData.RegisterListColumn("Creation date", MultiRenameDocuments.GetCreationDateString);

			mrData.DefaultPatternString = "[SN][E]";

			MultiRenameController mrController = new MultiRenameController();
			mrController.InitializeDocument(mrData);
			Current.Gui.ShowDialog(mrController, "Export multiple graphs");
		}

		private static List<object> DoExportGraphs(MultiRenameData mrData)
		{
			var failedItems = new List<object>();
			var errors = new StringBuilder();

			bool allPathsRooted = true;
			for (int i = 0; i < mrData.ObjectsToRenameCount; ++i)
			{
				var fileName = mrData.GetNewNameForObject(i);
				if (!System.IO.Path.IsPathRooted(fileName))
				{
					allPathsRooted = false;
					break;
				}
			}

			if (!allPathsRooted)
			{
				//Current.Gui.ShowFolderDialog();
				// http://wpfdialogs.codeplex.com/
			}

			for (int i = 0; i < mrData.ObjectsToRenameCount; ++i)
			{
				var graph = (GraphDocument)mrData.GetObjectToRename(i);
				var fileName = mrData.GetNewNameForObject(i);
				try
				{
					DoExportGraph(graph, fileName, _graphExportOptionsToFile);
				}
				catch (Exception ex)
				{
					failedItems.Add(graph);
					errors.AppendFormat("Graph {0} -> file name {1}: export failed, {2}\n", graph.Name, fileName, ex.Message);
				}
			}

			if (errors.Length != 0)
				Current.Gui.ErrorMessageBox(errors.ToString(), "Export failed for some items");
			else
				Current.Gui.InfoMessageBox(string.Format("{0} graphs successfully exported.", mrData.ObjectsToRenameCount));

			return failedItems;
		}

		public static void DoExportGraph(Graph.Gdi.GraphDocument doc, string fileName, Graph.Gdi.GraphExportOptions graphExportOptions)
		{
			if (!System.IO.Path.IsPathRooted(fileName))
				throw new ArgumentException("Path is not rooted!");

			var fileNamePart = System.IO.Path.GetFileName(fileName);
			var pathPart = System.IO.Path.GetDirectoryName(fileName);
			if (true && !System.IO.Directory.Exists(pathPart))
			{
				System.IO.Directory.CreateDirectory(pathPart);
			}

			using (Stream myStream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.Read))
			{
				doc.Render(myStream, graphExportOptions);
				myStream.Close();
			}
		}

		public static void ShowFileExportSpecificDialog(this GraphDocument doc)
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
			ShowFileExportDialog(doc, _graphExportOptionsToFile);
		}

		public static void ShowFileExportDialog(this GraphDocument doc, GraphExportOptions graphExportOptions)
		{
			var saveOptions = new Altaxo.Gui.SaveFileOptions();
			var list = GetFileFilterString(graphExportOptions.ImageFormat);
			foreach (var entry in list)
				saveOptions.AddFilter(entry.Key, entry.Value);
			saveOptions.FilterIndex = 0;
			saveOptions.RestoreDirectory = true;

			if (Current.Gui.ShowSaveFileDialog(saveOptions))
			{
				using (Stream myStream = new FileStream(saveOptions.FileName, FileMode.Create, FileAccess.Write, FileShare.Read))
				{
					doc.Render(myStream, graphExportOptions);
					myStream.Close();
				} // end openfile ok
			} // end dlgresult ok
		}

		public static void ShowFileExportMetafileDialog(this GraphDocument doc)
		{
			var opt = new GraphExportOptions();
			opt.IsIntentedForClipboardOperation = false;
			opt.TrySetImageAndPixelFormat(ImageFormat.Emf, PixelFormat.Format32bppArgb);
			ShowFileExportDialog(doc, opt);
		}

		public static void ShowFileExportTiffDialog(this GraphDocument doc)
		{
			var opt = new GraphExportOptions();
			opt.IsIntentedForClipboardOperation = false;
			opt.TrySetImageAndPixelFormat(ImageFormat.Tiff, PixelFormat.Format32bppArgb);
			opt.SourceDpiResolution = 300;
			opt.DestinationDpiResolution = 300;
			ShowFileExportDialog(doc, opt);
		}

		#region Stream und file

		public static void Render(this GraphDocument doc, System.IO.Stream stream, GraphExportOptions options)
		{
			if (options.ImageFormat == ImageFormat.Wmf || options.ImageFormat == ImageFormat.Emf)
				doc.RenderAsMetafile(stream, options);
			else
				doc.RenderAsBitmap(stream, options);
		}

		public static void Render(this GraphDocument doc, string filename, GraphExportOptions options)
		{
			using (System.IO.Stream str = new System.IO.FileStream(filename, System.IO.FileMode.CreateNew, System.IO.FileAccess.Write, System.IO.FileShare.Read))
			{
				Render(doc, str, options);
				str.Close();
			}
		}

		#endregion Stream und file

		#region Bitmap

		#region main work

		/// <summary>
		/// Saves the graph as an bitmap file and returns the bitmap.
		/// </summary>
		/// <param name="doc">The graph document to export.</param>
		/// <param name="backbrush">Brush used to fill the background of the image. Can be <c>null</c>.</param>
		/// <param name="pixelformat">Specify the pixelformat here.</param>
		/// <param name="areaToExport">The area of the graph to render.</param>
		/// <param name="sourceDpiResolution">Resolution at which the graph document is rendered into a bitmap.</param>
		/// <param name="destinationDpiResolution">Resolution which is assigned to the bitmap. This determines the physical size of the bitmap.</param>
		/// <returns>The saved bitmap. You should call Dispose when you no longer need the bitmap.</returns>
		public static Bitmap RenderAsBitmap(this GraphDocument doc, Brush backbrush, PixelFormat pixelformat, double sourceDpiResolution, double destinationDpiResolution)
		{
			double scale = sourceDpiResolution / 72.0;
			// Code to write the stream goes here.
			int width, height;

			// round the pixels to multiples of 4, many programs rely on this
			width = (int)(4 * Math.Ceiling(0.25 * doc.Size.X * scale));
			height = (int)(4 * Math.Ceiling(0.25 * doc.Size.Y * scale));

			System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(width, height, pixelformat);

			bitmap.SetResolution((float)sourceDpiResolution, (float)sourceDpiResolution);

			Graphics grfx = Graphics.FromImage(bitmap);
			if (null != backbrush)
				grfx.FillRectangle(backbrush, new Rectangle(0, 0, width, height));

			grfx.PageUnit = GraphicsUnit.Point;

			float zoom;
			PointF startLocationOnPage;

			grfx.PageScale = 1; // (float)scale;

			doc.DoPaint(grfx, true);

			grfx.Dispose();

			bitmap.SetResolution((float)destinationDpiResolution, (float)destinationDpiResolution);

			return bitmap;
		}

		#endregion main work

		#region stream

		/// <summary>
		/// Saves the graph as an bitmap file into the stream <paramref name="stream"/>.
		/// </summary>
		/// <param name="doc">The graph document to export.</param>
		/// <param name="stream">The stream to save the metafile into.</param>
		/// <param name="imageFormat">The format of the destination image.</param>
		/// <param name="backbrush">Brush used to fill the background of the image. Can be <c>null</c>.</param>
		/// <param name="pixelformat">Specify the pixelformat here.</param>
		/// <param name="usePageBounds"></param>
		/// <param name="sourceDpiResolution">Resolution at which the graph is rendered to a bitmap.</param>
		/// <param name="destinationDpiResolution">Resolution of the resulting bitmap. This determines the physical size of the bitmap.</param>
		public static void RenderAsBitmap(this GraphDocument doc, System.IO.Stream stream, System.Drawing.Imaging.ImageFormat imageFormat, Brush backbrush, PixelFormat pixelformat, double sourceDpiResolution, double destinationDpiResolution)
		{
			System.Drawing.Bitmap bitmap = RenderAsBitmap(doc, backbrush, pixelformat, sourceDpiResolution, destinationDpiResolution);

			bitmap.Save(stream, imageFormat);

			bitmap.Dispose();
		}

		/// <summary>
		/// Saves the graph as an bitmap file into the stream using the default pixelformat 32bppArgb and no background brush.<paramref name="stream"/>.
		/// </summary>
		/// <param name="doc">The graph document to export.</param>
		/// <param name="stream">The stream to save the metafile into.</param>
		/// <param name="imageFormat">The format of the destination image.</param>
		/// <param name="usePageBounds">If <c>true</c>, the page bounds where used.</param>
		/// <param name="dpiResolution">Resolution of the bitmap in dpi. Determines the pixel size of the bitmap.</param>
		public static void RenderAsBitmap(this GraphDocument doc, System.IO.Stream stream, System.Drawing.Imaging.ImageFormat imageFormat, double dpiResolution)
		{
			RenderAsBitmap(doc, stream, imageFormat, null, PixelFormat.Format32bppArgb, dpiResolution, dpiResolution);
		}

		/// <summary>
		/// Saves the graph as an bitmap file into the stream using the default pixelformat 32bppArgb.<paramref name="stream"/>.
		/// </summary>
		/// <param name="doc">The graph document to export.</param>
		/// <param name="stream">The stream to save the metafile into.</param>
		/// <param name="imageFormat">The format of the destination image.</param>
		/// <param name="backbrush">Brush used to fill the background of the image. Can be <c>null</c>.</param>
		/// <param name="usePageBounds">If <c>true</c>, the page bounds where used as bounding box.</param>
		/// <param name="dpiResolution">Resolution of the bitmap in dpi. Determines the pixel size of the bitmap.</param>
		public static void RenderAsBitmap(this GraphDocument doc, System.IO.Stream stream, System.Drawing.Imaging.ImageFormat imageFormat, Brush backbrush, double dpiResolution)
		{
			RenderAsBitmap(doc, stream, imageFormat, backbrush, PixelFormat.Format32bppArgb, dpiResolution, dpiResolution);
		}

		public static void RenderAsBitmap(this GraphDocument doc, System.IO.Stream stream, GraphExportOptions options)
		{
			RenderAsBitmap(doc, stream, options.ImageFormat, options.GetBrushOrDefaultBrush(), options.PixelFormat, options.SourceDpiResolution, options.DestinationDpiResolution);
		}

		#endregion stream

		#region file name

		/// <summary>
		/// Saves the graph as an bitmap file into the file <paramref name="filename"/>.
		/// </summary>
		/// <param name="doc">The graph document to export.</param>
		/// <param name="filename">The filename of the file to save the bitmap into.</param>
		/// <param name="imageFormat">The format of the destination image.</param>
		/// <param name="backbrush">Brush used to fill the background of the image. Can be <c>null</c>.</param>
		/// <param name="pixelformat">Specify the pixelformat here.</param>
		/// <param name="usePageBounds">If <c>true</c>, the page bounds were used for rendering.</param>
		/// <param name="sourceDpiResolution">Resolution in dpi used to render the graph into the bitmap.</param>
		/// <param name="destinationDpiResolution">Resolution that is set in the parameters of the bitmap. This determines the physical size of the bitmap.</param>
		public static void RenderAsBitmap(this GraphDocument doc, string filename, System.Drawing.Imaging.ImageFormat imageFormat, Brush backbrush, PixelFormat pixelformat, double sourceDpiResolution, double destinationDpiResolution)
		{
			using (System.IO.Stream str = new System.IO.FileStream(filename, System.IO.FileMode.CreateNew, System.IO.FileAccess.Write, System.IO.FileShare.Read))
			{
				RenderAsBitmap(doc, str, imageFormat, backbrush, pixelformat, sourceDpiResolution, destinationDpiResolution);
				str.Close();
			}
		}

		/// <summary>
		/// Saves the graph as an bitmap file into the file <paramref name="filename"/> using the default
		/// pixel format 32bppArgb and no background brush.
		/// </summary>
		/// <param name="doc">The graph document to export.</param>
		/// <param name="filename">The filename of the file to save the bitmap into.</param>
		/// <param name="imageFormat">The format of the destination image.</param>
		/// <param name="usePageBounds">If <c>true</c>, the page bounds are used for rendering.</param>
		/// <param name="dpiResolution">Resolution of the bitmap in dpi. Determines the pixel size of the bitmap.</param>
		public static void RenderAsBitmap(this GraphDocument doc, string filename, System.Drawing.Imaging.ImageFormat imageFormat, double dpiResolution)
		{
			RenderAsBitmap(doc, filename, imageFormat, null, dpiResolution);
		}

		/// <summary>
		/// Saves the graph as an bitmap file into the file <paramref name="filename"/> using the default
		/// pixel format 32bppArgb.
		/// </summary>
		/// <param name="doc">The graph document to export.</param>
		/// <param name="filename">The filename of the file to save the bitmap into.</param>
		/// <param name="imageFormat">The format of the destination image.</param>
		/// <param name="backbrush">Brush used to fill the background of the image. Can be <c>null</c>.</param>
		/// <param name="usePageBounds">If <c>true</c>, page bounds are used.</param>
		/// <param name="dpiResolution">Resolution of the bitmap in dpi. Determines the pixel size of the bitmap.</param>
		public static void RenderAsBitmap(this GraphDocument doc, string filename, System.Drawing.Imaging.ImageFormat imageFormat, Brush backbrush, double dpiResolution)
		{
			RenderAsBitmap(doc, filename, imageFormat, backbrush, PixelFormat.Format32bppArgb, dpiResolution, dpiResolution);
		}

		public static void RenderAsBitmap(this GraphDocument doc, string filename, GraphExportOptions options)
		{
			RenderAsBitmap(doc, filename, options.ImageFormat, options.BackgroundBrush, options.PixelFormat, options.SourceDpiResolution, options.DestinationDpiResolution);
		}

		#endregion file name

		#region Bitmap

		public static Bitmap RenderAsBitmap(this GraphDocument doc, GraphExportOptions options)
		{
			return RenderAsBitmap(doc, options.BackgroundBrush, options.PixelFormat, options.SourceDpiResolution, options.DestinationDpiResolution);
		}

		#endregion Bitmap

		#endregion Bitmap

		#region Metafile

		#region Main work

		/// <summary>
		/// Saves the graph as an enhanced windows metafile into the stream <paramref name="stream"/>.
		/// </summary>
		/// <param name="doc">The graph document used.</param>
		/// <param name="grfx">The graphics context used to create the metafile.</param>
		/// <param name="stream">The stream to save the metafile into.</param>
		/// <param name="backbrush">Brush used to fill the background of the image. Can be null.</param>
		/// <param name="pixelformat">The pixel format to use.</param>
		/// <param name="area">Area to render.</param>
		/// <param name="scale">Factor that is multiplied with the size of the exported area to determine the size of the bounding box of the meta file.</param>
		/// <returns>The metafile that was created using the stream.</returns>
		public static Metafile RenderAsMetafile(GraphDocument doc, Graphics grfx, System.IO.Stream stream, Brush backbrush, PixelFormat pixelformat, double scale)
		{
			grfx.PageUnit = GraphicsUnit.Point;
			IntPtr ipHdc = grfx.GetHdc();

			var metaFileBounds = new RectangleF(0, 0, (float)(doc.Size.X * scale), (float)(doc.Size.Y * scale));

			System.Drawing.Imaging.Metafile mf;

			if (null != stream)
				mf = new System.Drawing.Imaging.Metafile(stream, ipHdc, metaFileBounds, MetafileFrameUnit.Point);
			else
				mf = new System.Drawing.Imaging.Metafile(ipHdc, metaFileBounds, MetafileFrameUnit.Point);

			using (Graphics grfx2 = Graphics.FromImage(mf))
			{
				if (Environment.OSVersion.Version.Major < 6 || !mf.GetMetafileHeader().IsDisplay())
				{
					grfx2.PageUnit = GraphicsUnit.Point;
					grfx2.PageScale = (float)scale; // that would not work properly (a bug?) in Windows Vista, instead we have to use the following:
				}
				else
				{
					grfx2.PageScale = (float)(scale * Math.Min(72.0f / grfx2.DpiX, 72.0f / grfx2.DpiY)); // this works in Vista with display mode
				}

				doc.DoPaint(grfx2, true);

				grfx2.Dispose();
			}

			grfx.ReleaseHdc(ipHdc);

			return mf;
		}

		/// <summary>
		/// Saves the graph as an enhanced windows metafile into the stream <paramref name="stream"/>.
		/// </summary>
		/// <param name="doc">The graph document used.</param>
		/// <param name="stream">The stream to save the metafile into.</param>
		/// <param name="backbrush">Brush used to fill the background of the image. Can be <c>null</c>.</param>
		/// <param name="pixelformat">The pixel format to use.</param>
		/// <param name="area">Area to export.</param>
		/// <param name="sourceDpiResolution">Resolution whith witch the plot is sampled.</param>
		/// <param name="destinationDpiResolution">Resolution of the bitmap in dpi. Determines the apparent size (width, height) of the bitmap.</param>
		/// <returns>The metafile that was created using the stream.</returns>
		public static Metafile RenderAsMetafile(this GraphDocument doc, System.IO.Stream stream, Brush backbrush, PixelFormat pixelformat, double sourceDpiResolution, double destinationDpiResolution)
		{
			Metafile mf = null;

			// it is preferable to use a graphics context from a printer to create the metafile, in this case
			// the metafile will become device independent (metaFile.GetMetaFileHeader().IsDisplay() will return false)
			// Only when no printer is installed, we use a graphics context from a bitmap, but this will lead
			// to wrong positioning / wrong boundaries depending on the current screen
			if (Current.PrintingService != null &&
				Current.PrintingService.PrintDocument != null &&
				Current.PrintingService.PrintDocument.PrinterSettings != null
				)
			{
				Graphics grfx = Current.PrintingService.PrintDocument.PrinterSettings.CreateMeasurementGraphics();
				mf = RenderAsMetafile(doc, grfx, stream, backbrush, pixelformat, sourceDpiResolution / destinationDpiResolution);
				grfx.Dispose();
			}
			else
			{
				// Create a bitmap just to get a graphics context from it
				System.Drawing.Bitmap helperbitmap = new System.Drawing.Bitmap(4, 4, pixelformat);
				helperbitmap.SetResolution((float)sourceDpiResolution, (float)sourceDpiResolution);
				Graphics grfx = Graphics.FromImage(helperbitmap);
				grfx.PageUnit = GraphicsUnit.Point;
				mf = RenderAsMetafile(doc, grfx, stream, backbrush, pixelformat, sourceDpiResolution / destinationDpiResolution);
				grfx.Dispose();
				helperbitmap.Dispose();
			}

			return mf;
		}

		#endregion Main work

		#region with stream

		public static Metafile RenderAsMetafile(this GraphDocument doc, System.IO.Stream stream, GraphExportOptions options)
		{
			return RenderAsMetafile(doc, stream, options.BackgroundBrush, options.PixelFormat, options.SourceDpiResolution, options.DestinationDpiResolution);
		}

		/// <summary>
		/// Saves the graph as an bitmap file into the stream using the default pixelformat 32bppArgb.<paramref name="stream"/>.
		/// </summary>
		/// <param name="doc">The graph document to export.</param>
		/// <param name="stream">The stream to save the metafile into.</param>
		/// <param name="backbrush">Brush used to fill the background of the image. Can be <c>null</c>.</param>
		/// <param name="sourceDpiResolution">Resolution whith witch the plot is sampled.</param>
		/// <param name="destinationDpiResolution">Resolution of the bitmap in dpi. Determines the apparent size (width, height) of the bitmap.</param>
		public static Metafile RenderAsMetafile(this GraphDocument doc, System.IO.Stream stream, Brush backbrush, double sourceDpiResolution, double destinationDpiResolution)
		{
			return RenderAsMetafile(doc, stream, backbrush, PixelFormat.Format32bppArgb, sourceDpiResolution, destinationDpiResolution);
		}

		/// <summary>
		/// Saves the graph as an bitmap file into the stream using the default pixelformat 32bppArgb and no background brush.<paramref name="stream"/>.
		/// </summary>
		/// <param name="doc">The graph document to export.</param>
		/// <param name="stream">The stream to save the metafile into.</param>
		/// <param name="dpiResolution">Resolution of the bitmap in dpi. Determines the pixel size of the bitmap.</param>
		public static Metafile RenderAsMetafile(this GraphDocument doc, System.IO.Stream stream, double dpiResolution)
		{
			return RenderAsMetafile(doc, stream, null, PixelFormat.Format32bppArgb, dpiResolution, dpiResolution);
		}

		#endregion with stream

		#region with filename

		public static Metafile RenderAsMetafile(this GraphDocument doc, string filename, GraphExportOptions options)
		{
			Metafile mf;
			using (System.IO.Stream str = new System.IO.FileStream(filename, System.IO.FileMode.CreateNew, System.IO.FileAccess.Write, System.IO.FileShare.Read))
			{
				mf = RenderAsMetafile(doc, str, options.BackgroundBrush, options.PixelFormat, options.SourceDpiResolution, options.DestinationDpiResolution);
				str.Close();
			}
			return mf;
		}

		/// <summary>
		/// Saves the graph as an bitmap file into the file <paramref name="filename"/>.
		/// </summary>
		/// <param name="doc">The graph document to export.</param>
		/// <param name="filename">The filename of the file to save the bitmap into.</param>
		/// <param name="backbrush">Brush used to fill the background of the image. Can be <c>null</c>.</param>
		/// <param name="pixelformat">Specify the pixelformat here.</param>
		/// <param name="dpiResolution">Resolution of the bitmap in dpi. Determines the pixel size of the bitmap.</param>
		public static Metafile RenderAsMetafile(this GraphDocument doc, string filename, Brush backbrush, PixelFormat pixelformat, double dpiResolution)
		{
			Metafile mf;
			using (System.IO.Stream str = new System.IO.FileStream(filename, System.IO.FileMode.CreateNew, System.IO.FileAccess.Write, System.IO.FileShare.Read))
			{
				mf = RenderAsMetafile(doc, str, backbrush, pixelformat, dpiResolution, dpiResolution);
				str.Close();
			}
			return mf;
		}

		/// <summary>
		/// Saves the graph as an bitmap file into the file <paramref name="filename"/> using the default
		/// pixel format 32bppArgb and no background brush.
		/// </summary>
		/// <param name="doc">The graph document to export.</param>
		/// <param name="filename">The filename of the file to save the bitmap into.</param>
		/// <param name="dpiResolution">Resolution of the bitmap in dpi. Determines the pixel size of the bitmap.</param>
		public static Metafile RenderAsMetafile(this GraphDocument doc, string filename, double dpiResolution)
		{
			return RenderAsMetafile(doc, filename, null, dpiResolution);
		}

		/// <summary>
		/// Saves the graph as an bitmap file into the file <paramref name="filename"/> using the default
		/// pixel format 32bppArgb.
		/// </summary>
		/// <param name="doc">The graph document to export.</param>
		/// <param name="filename">The filename of the file to save the bitmap into.</param>
		/// <param name="backbrush">Brush used to fill the background of the image. Can be <c>null</c>.</param>
		/// <param name="dpiResolution">Resolution of the bitmap in dpi. Determines the pixel size of the bitmap.</param>
		public static Metafile RenderAsMetafile(this GraphDocument doc, string filename, Brush backbrush, double dpiResolution)
		{
			return RenderAsMetafile(doc, filename, backbrush, PixelFormat.Format32bppArgb, dpiResolution);
		}

		#endregion with filename

		#region default rendering

		public static Metafile RenderAsMetafile(this GraphDocument doc)
		{
			System.IO.MemoryStream stream = new System.IO.MemoryStream();
			Metafile mf = RenderAsMetafile(doc, stream, 300);
			stream.Flush();
			stream.Close();
			return mf;
		}

		#endregion default rendering

		#endregion Metafile
	}

	/// <summary>
	/// Designates how to store the copied page in the clipboard.
	/// </summary>
	[Flags]
	public enum GraphCopyPageClipboardFormat
	{
		/// <summary>Store as native image.</summary>
		AsNative = 1,

		/// <summary>Store in a temporary file and set the file name in the clipboard as DropDownList.</summary>
		AsDropDownList = 2,

		/// <summary>
		/// As bitmap wrapped in an enhanced metafile (not applicable if native image is a metafile or enhanced metafile).
		/// </summary>
		AsNativeWrappedInEnhancedMetafile = 4,

		/// <summary>Copy the graph as Com object that can be embedded in another application</summary>
		AsEmbeddedObject = 8,

		/// <summary>
		/// Copy the graph as Com object that can be linked to in another application (is only available if the project has a valid file name).
		/// </summary>
		AsLinkedObject = 16,
	}

	public class GraphExportOptions : Main.ICopyFrom
	{
		private ImageFormat _imageFormat;
		private PixelFormat _pixelFormat;
		private BrushX _backgroundBrush;
		private double _sourceDpiResolution;
		private double _destinationDpiResolution;
		private GraphCopyPageClipboardFormat _clipboardFormat;

		public bool IsIntentedForClipboardOperation { get; set; }

		public GraphCopyPageClipboardFormat ClipboardFormat
		{
			get { return _clipboardFormat; }
			set
			{
				_clipboardFormat = value;

				if (0 == (int)_clipboardFormat)
					_clipboardFormat = GraphCopyPageClipboardFormat.AsNative;
			}
		}

		public bool CopyFrom(object obj)
		{
			if (object.ReferenceEquals(this, obj))
				return true;

			var from = obj as GraphExportOptions;

			if (null != from)
			{
				this._imageFormat = from.ImageFormat;
				this._pixelFormat = from.PixelFormat;
				this._backgroundBrush = null == from._backgroundBrush ? null : from._backgroundBrush.Clone();
				this.SourceDpiResolution = from.SourceDpiResolution;
				this.DestinationDpiResolution = from.DestinationDpiResolution;
				this.IsIntentedForClipboardOperation = from.IsIntentedForClipboardOperation;
				this.ClipboardFormat = from.ClipboardFormat;
				return true;
			}

			return false;
		}

		public GraphExportOptions()
		{
			this._imageFormat = System.Drawing.Imaging.ImageFormat.Png;
			this._pixelFormat = System.Drawing.Imaging.PixelFormat.Format32bppArgb;
			this.SourceDpiResolution = 300;
			this.DestinationDpiResolution = 300;
			this.BackgroundBrush = null;
			this.IsIntentedForClipboardOperation = false;
			this.ClipboardFormat = GraphCopyPageClipboardFormat.AsNative | GraphCopyPageClipboardFormat.AsDropDownList | GraphCopyPageClipboardFormat.AsNativeWrappedInEnhancedMetafile | GraphCopyPageClipboardFormat.AsEmbeddedObject | GraphCopyPageClipboardFormat.AsLinkedObject;
		}

		public GraphExportOptions(GraphExportOptions from)
		{
			CopyFrom(from);
		}

		object ICloneable.Clone()
		{
			return new GraphExportOptions(this);
		}

		public GraphExportOptions Clone()
		{
			return new GraphExportOptions(this);
		}

		public ImageFormat ImageFormat { get { return _imageFormat; } }

		public PixelFormat PixelFormat { get { return _pixelFormat; } }

		public BrushX BackgroundBrush
		{
			get
			{
				return _backgroundBrush;
			}
			set
			{
				_backgroundBrush = value;
			}
		}

		public double SourceDpiResolution
		{
			get
			{
				return _sourceDpiResolution;
			}
			set
			{
				if (!(value > 0))
					throw new ArgumentException("SourceDpiResolution has to be >0");

				_sourceDpiResolution = value;
			}
		}

		public double DestinationDpiResolution
		{
			get
			{
				return _destinationDpiResolution;
			}
			set
			{
				if (!(value > 0))
					throw new ArgumentException("DestinationDpiResolution has to be >0");

				_destinationDpiResolution = value;
			}
		}

		public bool TrySetImageAndPixelFormat(ImageFormat imgfmt, PixelFormat pixfmt)
		{
			if (!IsVectorFormat(imgfmt) && !CanCreateAndSaveBitmap(imgfmt, pixfmt))
				return false;

			_imageFormat = imgfmt;
			_pixelFormat = pixfmt;

			return true;
		}

		public Brush GetDefaultBrush()
		{
			if (IsVectorFormat(_imageFormat) || HasPixelFormatAlphaChannel(_pixelFormat))
				return null;
			else
				return new SolidBrush(Color.White);
		}

		public Brush GetBrushOrDefaultBrush()
		{
			if (null != _backgroundBrush)
				return _backgroundBrush;
			else
				return GetDefaultBrush();
		}

		/// <summary>
		/// Returns the default file name extension (including leading dot) for the current image format.
		/// </summary>
		/// <returns>Default file name extension (including leading dot) for the current image format</returns>
		public string GetDefaultFileNameExtension()
		{
			if (_imageFormat == ImageFormat.Bmp)
				return ".bmp";
			else if (_imageFormat == ImageFormat.Emf)
				return ".emf";
			else if (_imageFormat == ImageFormat.Exif)
				return ".exif";
			else if (_imageFormat == ImageFormat.Gif)
				return ".gif";
			else if (_imageFormat == ImageFormat.Icon)
				return ".ico";
			else if (_imageFormat == ImageFormat.Jpeg)
				return ".jpg";
			else if (_imageFormat == ImageFormat.Png)
				return ".png";
			else if (_imageFormat == ImageFormat.Tiff)
				return ".tif";
			else if (_imageFormat == ImageFormat.Wmf)
				return ".wmf";
			else return ".img";
		}

		private static GraphExportOptions _currentSetting = new GraphExportOptions();

		public static GraphExportOptions CurrentSetting
		{
			get
			{
				return _currentSetting;
			}
		}

		public static bool IsVectorFormat(ImageFormat fmt)
		{
			return ImageFormat.Emf == fmt || ImageFormat.Wmf == fmt;
		}

		public static bool CanCreateBitmap(PixelFormat fmt)
		{
			try
			{
				var bmp = new Bitmap(4, 4, fmt);
				bmp.Dispose();
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		public static bool CanCreateAndSaveBitmap(ImageFormat imgfmt, PixelFormat pixfmt)
		{
			try
			{
				using (var bmp = new Bitmap(8, 8, pixfmt))
				{
					using (var str = new System.IO.MemoryStream())
					{
						bmp.Save(str, imgfmt);
						str.Close();
					}
				}

				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		public static bool HasPixelFormatAlphaChannel(PixelFormat fmt)
		{
			return
				PixelFormat.Alpha == fmt ||
				PixelFormat.Canonical == fmt ||
				PixelFormat.Format16bppArgb1555 == fmt ||
				PixelFormat.Format32bppArgb == fmt ||
				PixelFormat.Format32bppPArgb == fmt ||
				PixelFormat.Format64bppArgb == fmt ||
				PixelFormat.Format64bppPArgb == fmt ||
				PixelFormat.PAlpha == fmt;
		}
	}
}