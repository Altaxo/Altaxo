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
	/// <summary>
	/// Helper functions for graph document export.
	/// </summary>
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
				doc.RenderToStream(myStream, graphExportOptions);
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
					doc.RenderToStream(myStream, graphExportOptions);
					myStream.Close();
				} // end openfile ok
			} // end dlgresult ok
		}

		public static void ShowFileExportMetafileDialog(this GraphDocument doc)
		{
			var opt = new GraphExportOptions();
			opt.TrySetImageAndPixelFormat(ImageFormat.Emf, PixelFormat.Format32bppArgb);
			ShowFileExportDialog(doc, opt);
		}

		public static void ShowFileExportTiffDialog(this GraphDocument doc)
		{
			var opt = new GraphExportOptions();
			opt.TrySetImageAndPixelFormat(ImageFormat.Tiff, PixelFormat.Format32bppArgb);
			opt.SourceDpiResolution = 300;
			opt.DestinationDpiResolution = 300;
			ShowFileExportDialog(doc, opt);
		}

		#region Stream und file

		public static void RenderToStream(this GraphDocument doc, System.IO.Stream stream, GraphExportOptions options)
		{
			if (options.ImageFormat == ImageFormat.Wmf || options.ImageFormat == ImageFormat.Emf)
				RenderAsEnhancedMetafileVectorFormatToStream(doc, stream, options);
			else
				doc.RenderAsBitmapToStream(stream, options);
		}

		public static void RenderToFile(this GraphDocument doc, string filename, GraphExportOptions options)
		{
			using (System.IO.Stream str = new System.IO.FileStream(filename, System.IO.FileMode.CreateNew, System.IO.FileAccess.Write, System.IO.FileShare.Read))
			{
				RenderToStream(doc, str, options);
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
		/// <param name="backbrush1">First brush used to fill the background of the image. Can be <c>null</c>.</param>
		/// <param name="backbrush2">Second brush used to fill the background of the image. Can be <c>null</c>.</param>
		/// <param name="pixelformat">Specify the pixelformat here.</param>
		/// <param name="sourceDpiResolution">Resolution at which the graph document is rendered into a bitmap.</param>
		/// <param name="destinationDpiResolution">Resolution which is assigned to the bitmap. This determines the physical size of the bitmap.</param>
		/// <returns>The saved bitmap. You should call Dispose when you no longer need the bitmap.</returns>
		public static Bitmap RenderAsBitmap(this GraphDocument doc, BrushX backbrush1, BrushX backbrush2, PixelFormat pixelformat, double sourceDpiResolution, double destinationDpiResolution)
		{
			// round the pixels to multiples of 4, many programs rely on this
			int bmpWidth = (int)(4 * Math.Ceiling(0.25 * doc.Size.X * sourceDpiResolution / 72));
			int bmpHeight = (int)(4 * Math.Ceiling(0.25 * doc.Size.Y * sourceDpiResolution / 72));
			System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(bmpWidth, bmpHeight, pixelformat);

			double outputScaling = sourceDpiResolution / destinationDpiResolution;
			bitmap.SetResolution((float)(bmpWidth / (outputScaling * doc.Size.X / 72)), (float)(bmpHeight / (outputScaling * doc.Size.Y / 72)));

			using (Graphics grfx = Graphics.FromImage(bitmap))
			{
				// Set everything to high quality
				grfx.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
				grfx.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;

				// 2014-10-10 Setting InterpolationMode to HighQualityBicubic and PixelOffsetMode to HighQuality
				// causes problems when rendering small bitmaps (at a large magnification, for instance the density image legend):
				// the resulting image seems a litte soft, the colors somehow distorted, so I decided not to use them here any more

				//		grfx.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
				//		grfx.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

				grfx.PageUnit = GraphicsUnit.Point;
				grfx.ScaleTransform((float)outputScaling, (float)outputScaling);
				grfx.SetClip(new RectangleF(0, 0, (float)doc.Size.X, (float)doc.Size.Y));

				if (null != backbrush1)
				{
					backbrush1.SetEnvironment(new Graph.RectangleD(0, 0, doc.Size.X, doc.Size.Y), sourceDpiResolution);
					grfx.FillRectangle(backbrush1, new RectangleF(0, 0, (float)doc.Size.X, (float)doc.Size.Y));
				}

				if (null != backbrush2)
				{
					backbrush2.SetEnvironment(new Graph.RectangleD(0, 0, doc.Size.X, doc.Size.Y), sourceDpiResolution);
					grfx.FillRectangle(backbrush2, new RectangleF(0, 0, (float)doc.Size.X, (float)doc.Size.Y));
				}

#if DIAGNOSTICLINERENDERING
				{
					var fDocSizeX = (float)doc.Size.X;
					var fDocSizeY = (float)doc.Size.Y;
					grfx.DrawLine(Pens.Black, 0, 0, fDocSizeX * 2, fDocSizeY * 1.3f);

					grfx.DrawLine(Pens.Black, 0, 0, fDocSizeX, fDocSizeY);
					grfx.DrawLine(Pens.Black, 0, fDocSizeY, fDocSizeX, 0);
					grfx.DrawLine(Pens.Black, 0, 0, fDocSizeX / 4, fDocSizeY / 2);
					grfx.DrawLine(Pens.Black, 0, fDocSizeY, fDocSizeX / 4, fDocSizeY / 2);
					grfx.DrawLine(Pens.Black, fDocSizeX * 0.75f, fDocSizeY / 2, fDocSizeX, 0);
					grfx.DrawLine(Pens.Black, fDocSizeX * 0.75f, fDocSizeY / 2, fDocSizeX, fDocSizeY);
				}
#endif

				doc.DoPaint(grfx, true);
			}

			bitmap.SetResolution((float)destinationDpiResolution, (float)destinationDpiResolution);

			return bitmap;
		}

		#endregion main work

		#region Convenience functions with export / embedded rendering options

		/// <summary>
		/// Saves the graph as an bitmap file and returns the bitmap.
		/// </summary>
		/// <param name="doc">The graph document to export.</param>
		/// <param name="backbrush">Brush used to fill the background of the image. Can be <c>null</c>.</param>
		/// <param name="pixelformat">Specify the pixelformat here.</param>
		/// <param name="sourceDpiResolution">Resolution at which the graph document is rendered into a bitmap.</param>
		/// <param name="destinationDpiResolution">Resolution which is assigned to the bitmap. This determines the physical size of the bitmap.</param>
		/// <returns>The saved bitmap. You should call Dispose when you no longer need the bitmap.</returns>
		public static Bitmap RenderAsBitmap(this GraphDocument doc, BrushX backbrush, PixelFormat pixelformat, double sourceDpiResolution, double destinationDpiResolution)
		{
			return RenderAsBitmap(doc, backbrush, null, pixelformat, sourceDpiResolution, destinationDpiResolution);
		}

		/// <summary>
		/// Renders the graph document as bitmap with default PixelFormat.Format32bppArgb.
		/// </summary>
		/// <param name="doc">The graph document used.</param>
		/// <param name="exportOptions">The clipboard export options.</param>
		/// <param name="pixelFormat">The pixel format for the bitmap. Default is PixelFormat.Format32bppArgb.</param>
		/// <returns>The rendered enhanced metafile.</returns>
		public static Bitmap RenderAsBitmap(GraphDocument doc, EmbeddedObjectRenderingOptions exportOptions, PixelFormat pixelFormat = PixelFormat.Format32bppArgb)
		{
			BrushX opaqueGround = null;
			if (!GraphExportOptions.HasPixelFormatAlphaChannel(pixelFormat))
				opaqueGround = new BrushX(exportOptions.BackgroundColorForFormatsWithoutAlphaChannel);

			var result = RenderAsBitmap(doc, opaqueGround, exportOptions.BackgroundBrush, pixelFormat, exportOptions.SourceDpiResolution, exportOptions.SourceDpiResolution / exportOptions.OutputScalingFactor);

			if (null != opaqueGround)
				opaqueGround.Dispose();

			return result;
		}

		public static Bitmap RenderAsBitmap(this GraphDocument doc, GraphExportOptions options)
		{
			return RenderAsBitmap(doc, options.BackgroundBrush, options.PixelFormat, options.SourceDpiResolution, options.DestinationDpiResolution);
		}

		#endregion Convenience functions with export / embedded rendering options

		#region stream

		/// <summary>
		/// Saves the graph as an bitmap file into the stream <paramref name="stream"/>.
		/// </summary>
		/// <param name="doc">The graph document to export.</param>
		/// <param name="stream">The stream to save the metafile into.</param>
		/// <param name="imageFormat">The format of the destination image.</param>
		/// <param name="backbrush">Brush used to fill the background of the image. Can be <c>null</c>.</param>
		/// <param name="pixelformat">Specify the pixelformat here.</param>
		/// <param name="sourceDpiResolution">Resolution at which the graph is rendered to a bitmap.</param>
		/// <param name="destinationDpiResolution">Resolution of the resulting bitmap. This determines the physical size of the bitmap.</param>
		public static void RenderAsBitmapToStream(this GraphDocument doc, System.IO.Stream stream, System.Drawing.Imaging.ImageFormat imageFormat, BrushX backbrush, PixelFormat pixelformat, double sourceDpiResolution, double destinationDpiResolution)
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
		/// <param name="dpiResolution">Resolution of the bitmap in dpi. Determines the pixel size of the bitmap.</param>
		public static void RenderAsBitmapToStream(this GraphDocument doc, System.IO.Stream stream, System.Drawing.Imaging.ImageFormat imageFormat, double dpiResolution)
		{
			RenderAsBitmapToStream(doc, stream, imageFormat, null, PixelFormat.Format32bppArgb, dpiResolution, dpiResolution);
		}

		/// <summary>
		/// Saves the graph as an bitmap file into the stream using the default pixelformat 32bppArgb.<paramref name="stream"/>.
		/// </summary>
		/// <param name="doc">The graph document to export.</param>
		/// <param name="stream">The stream to save the metafile into.</param>
		/// <param name="imageFormat">The format of the destination image.</param>
		/// <param name="backbrush">Brush used to fill the background of the image. Can be <c>null</c>.</param>
		/// <param name="dpiResolution">Resolution of the bitmap in dpi. Determines the pixel size of the bitmap.</param>
		public static void RenderAsBitmapToStream(this GraphDocument doc, System.IO.Stream stream, System.Drawing.Imaging.ImageFormat imageFormat, BrushX backbrush, double dpiResolution)
		{
			RenderAsBitmapToStream(doc, stream, imageFormat, backbrush, PixelFormat.Format32bppArgb, dpiResolution, dpiResolution);
		}

		public static void RenderAsBitmapToStream(this GraphDocument doc, System.IO.Stream stream, GraphExportOptions options)
		{
			RenderAsBitmapToStream(doc, stream, options.ImageFormat, options.GetBrushOrDefaultBrush(), options.PixelFormat, options.SourceDpiResolution, options.DestinationDpiResolution);
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
		/// <param name="sourceDpiResolution">Resolution in dpi used to render the graph into the bitmap.</param>
		/// <param name="destinationDpiResolution">Resolution that is set in the parameters of the bitmap. This determines the physical size of the bitmap.</param>
		public static void RenderAsBitmapToFile(this GraphDocument doc, string filename, System.Drawing.Imaging.ImageFormat imageFormat, BrushX backbrush, PixelFormat pixelformat, double sourceDpiResolution, double destinationDpiResolution)
		{
			using (System.IO.Stream str = new System.IO.FileStream(filename, System.IO.FileMode.CreateNew, System.IO.FileAccess.Write, System.IO.FileShare.Read))
			{
				RenderAsBitmapToStream(doc, str, imageFormat, backbrush, pixelformat, sourceDpiResolution, destinationDpiResolution);
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
		/// <param name="dpiResolution">Resolution of the bitmap in dpi. Determines the pixel size of the bitmap.</param>
		public static void RenderAsBitmapToFile(this GraphDocument doc, string filename, System.Drawing.Imaging.ImageFormat imageFormat, double dpiResolution)
		{
			RenderAsBitmapToFile(doc, filename, imageFormat, null, dpiResolution);
		}

		/// <summary>
		/// Saves the graph as an bitmap file into the file <paramref name="filename"/> using the default
		/// pixel format 32bppArgb.
		/// </summary>
		/// <param name="doc">The graph document to export.</param>
		/// <param name="filename">The filename of the file to save the bitmap into.</param>
		/// <param name="imageFormat">The format of the destination image.</param>
		/// <param name="backbrush">Brush used to fill the background of the image. Can be <c>null</c>.</param>
		/// <param name="dpiResolution">Resolution of the bitmap in dpi. Determines the pixel size of the bitmap.</param>
		public static void RenderAsBitmapToFile(this GraphDocument doc, string filename, System.Drawing.Imaging.ImageFormat imageFormat, BrushX backbrush, double dpiResolution)
		{
			RenderAsBitmapToFile(doc, filename, imageFormat, backbrush, PixelFormat.Format32bppArgb, dpiResolution, dpiResolution);
		}

		public static void RenderAsBitmapToFile(this GraphDocument doc, string filename, GraphExportOptions options)
		{
			RenderAsBitmapToFile(doc, filename, options.ImageFormat, options.BackgroundBrush, options.PixelFormat, options.SourceDpiResolution, options.DestinationDpiResolution);
		}

		#endregion file name

		#region Bitmap conversion

		/// <summary>
		/// Converts the given bitmap to another pixel format.
		/// </summary>
		/// <param name="bitmapToConvert">The bitmap to convert.</param>
		/// <param name="pixelFormat">The pixel format of the converted bitmap.</param>
		/// <param name="backgroundColorForFormatsWithoutAlphaChannel">The background color for pixel formats without alpha channel. This color is used to paint the background of the new bitmap if the new bitmap's pixel format has no alpha channel.</param>
		/// <returns>Converted bitmap.</returns>
		public static Bitmap ConvertBitmapToPixelFormat(this Bitmap bitmapToConvert, PixelFormat pixelFormat, NamedColor backgroundColorForFormatsWithoutAlphaChannel)
		{
			var convertedBitmap = new System.Drawing.Bitmap(bitmapToConvert.Width, bitmapToConvert.Height, pixelFormat);
			convertedBitmap.SetResolution(bitmapToConvert.HorizontalResolution, bitmapToConvert.VerticalResolution);
			using (var grfx = System.Drawing.Graphics.FromImage(convertedBitmap))
			{
				grfx.PageUnit = GraphicsUnit.Pixel;
				if (!Altaxo.Graph.Gdi.GraphExportOptions.HasPixelFormatAlphaChannel(pixelFormat))
				{
					using (var brush = new System.Drawing.SolidBrush(backgroundColorForFormatsWithoutAlphaChannel))
					{
						grfx.FillRectangle(brush, 0, 0, convertedBitmap.Width, convertedBitmap.Height);
					}
				}
				grfx.DrawImageUnscaled(bitmapToConvert, 0, 0);
			}

			return convertedBitmap;
		}

		#endregion Bitmap conversion

		#endregion Bitmap

		#region Enhanced Metafile (vector format)

		#region Main work

		/// <summary>
		/// Renders a document as enhanced metafile. The metafile is rendered into a stream. You can create a metafile object afterwards from that stream.
		/// </summary>
		/// <param name="renderingProc">Procedure for rendering the document.
		/// The argument is a graphics context, which is set to GraphicsUnits equal to Points.
		/// The drawing must be inside of the boundaries of docSize.X and docSize.Y.
		/// </param>
		/// <param name="stream">Destination stream. The metafile is rendered into this stream. The stream has to be writeable and seekable. At return, the position of the stream is set to 0, thus the stream is ready to be used to create a metafile object from it.</param>
		/// <param name="docSize">Size of the document in points (1/72 inch)</param>
		/// <param name="sourceDpiResolution">The resolution in dpi of the source. This parameter is used only if creating the reference graphics context from the current printer fails. In this case, a context from a bitmap with the provided resolution is created.</param>
		/// <param name="outputScalingFactor">Output scaling factor. If less than 1, the image will appear smaller than originally, if greater than 1, the image will appear larger than originally.</param>
		/// <param name="pixelFormat">Optional: Only used if the graphics context can not be created from a printer document. Pixel format of the bitmap that is used in this case to construct the graphics context.</param>
		/// <returns>The rendered enhanced metafile (vector format).</returns>
		/// <remarks>
		/// <para>
		/// I found no other way to realize different dpi resolutions, independently of screen or printer device contexts, as to patch the resulting metafile stream with
		/// informations about an 'artifical' device, which has exactly the resolution that is neccessary. By careful choice of the size of this artifical device one can
		/// avoid rounding errors concerning resolution and size.
		/// It happens that some programs (for instance MS Word 2010 when saving as PDF document) mess up the size of the metafile graphics, if the graphics was created with a PageUnit
		/// (of the graphics context) other than PIXELS.
		/// Thus I now always use PIXEL as PageUnit and scale the graphics context accordingly.
		/// </para>
		/// <para>
		/// Another problem, which is actually without solution, is that e.g. MS Office will not show polylines with more than 8125 points. These polylines are included in the metafile,
		/// but MS Office seems to ignore them. On the other hand, CorelDraw X5 can show these polylines correctly.
		/// This problem might be related to the EmfPlus format, because MS Office will show these polylines if the EmfOnly format is used. But EmfOnly can not handle transparencies, thus
		/// it is not really a choice.
		/// </para>
		/// </remarks>
		public static void RenderAsEnhancedMetafileToStream(Action<Graphics> renderingProc, System.IO.Stream stream, PointD2D docSize, double sourceDpiResolution, double outputScalingFactor, PixelFormat pixelFormat = PixelFormat.Format32bppArgb)
		{
			if (stream == null)
				throw new ArgumentNullException("stream");
			if (!stream.CanWrite)
				throw new ArgumentException("stream is not writeable");
			if (!stream.CanSeek)
				throw new ArgumentException("stream is not seekable");
			stream.SetLength(0);

			var scaledDocSize = docSize * outputScalingFactor;

			// our artifical device has a square size, and the size is an integer multiple of 5 inch (5 inch because this is the smallest size which converts to an integer number of millimeters: 127 mm)
			int deviceSizeInch = (int)(5 * Math.Ceiling(Math.Max(scaledDocSize.X, scaledDocSize.Y) / (72 * 5)));

			// we have to design our artifical device so that it has a resolution of sourceDpiResolution/outputScalingFactor
			// this accounts for the fact, that if
			double deviceResolution = sourceDpiResolution / outputScalingFactor;

			// then the number of pixels of the device is simple the device size in inch times the device resolution
			int devicePixelsX = (int)Math.Round(deviceSizeInch * deviceResolution);
			int devicePixelsY = (int)Math.Round(deviceSizeInch * deviceResolution);

			// device size in millimeter. Because of the choice of the device size (see above) there should be no rounding errors here
			int deviceSizeXMillimeter = (deviceSizeInch * 254) / 10;
			int deviceSizeYMillimeter = (deviceSizeInch * 254) / 10;

			// device size in micrometer
			int deviceSizeXMicrometer = deviceSizeInch * 25400;
			int deviceSizeYMicrometer = deviceSizeInch * 25400;

			// bounds of the graphic in pixels. Because it is in pixels, it is calculated with the unscaled size of the document and the sourceDpiResolution
			int graphicBoundsLeft_Pixels = 0;
			int graphicBoundsTop_Pixels = 0;
			int graphicBoundsWidth_Pixels = (int)Math.Ceiling(sourceDpiResolution * docSize.X / 72);
			int graphicBoundsHeight_Pixels = (int)Math.Ceiling(sourceDpiResolution * docSize.Y / 72);

			// position and size of the bounding box. Please not that the bounds are scaled with the outputScalingFactor
			int boundingBoxLeft_HIMETRIC = 0;
			int boundingBoxTop_HIMETRIC = 0;
			int boundingBoxWidth_HIMETRIC = (int)Math.Ceiling(scaledDocSize.X * 2540.0 / 72);
			int boundingBoxHeight_HIMETRIC = (int)Math.Ceiling(scaledDocSize.Y * 2540.0 / 72);

			Metafile metafile;
			using (var helperbitmap = new System.Drawing.Bitmap(4, 4, PixelFormat.Format32bppArgb))
			{
				using (Graphics grfxReference = Graphics.FromImage(helperbitmap))
				{
					IntPtr deviceContextHandle = grfxReference.GetHdc();

					metafile = new Metafile(
					stream,
					deviceContextHandle,
					new RectangleF(boundingBoxLeft_HIMETRIC, boundingBoxTop_HIMETRIC, boundingBoxWidth_HIMETRIC, boundingBoxHeight_HIMETRIC),
					MetafileFrameUnit.GdiCompatible,
					EmfType.EmfPlusDual); // EmfOnly is working with PolyLines with more than 8125 Points, but can not handle transparencies  // EmfPlusDual and EmfPlusOnly: there is no display of polylines with more than 8125 points, although the polyline seems embedded in the EMF. // EmfPlusOnly can not be converted to WMF

					grfxReference.ReleaseHdc();
				}
			}

			using (Graphics grfxMetafile = Graphics.FromImage(metafile))
			{
				// Set everything to high quality
				grfxMetafile.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
				grfxMetafile.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;

				// 2014-10-10 Setting InterpolationMode to HighQualityBicubic and PixelOffsetMode to HighQuality
				// causes problems when rendering small bitmaps (at a large magnification, for instance the density image legend):
				// the resulting image seems a litte soft, the colors somehow distorted, so I decided not to use them here any more

				//grfxMetafile.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
				//grfxMetafile.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

				grfxMetafile.PageUnit = GraphicsUnit.Pixel; // Attention: always use pixels here. Any other choice will cause problems in some programs (see remarks above).
				grfxMetafile.PageScale = (float)(sourceDpiResolution / 72.0); // because our choice of GraphicsUnit is pixels, at the resolution of 72 dpi one point is one pixel. At a higher resolution, one point is more than one pixel.

				grfxMetafile.SetClip(new RectangleF(0, 0, (float)docSize.X, (float)docSize.Y));
				renderingProc(grfxMetafile);
			}
			stream.Flush();

			// we have to patch the resulting metafile stream with the parameters of the graphics and the device

			stream.Position = 0x04;
			var buf4 = new byte[4];
			stream.Read(buf4, 0, 4);
			int headerSize = BitConverter.ToInt32(buf4, 0); // Read the header size to make sure that Metafile header extension 2 is present

			// At position 0x08 there are the bounds of the graphic (not the bounding box, but the box for all the graphical elements)
			stream.Position = 0x08;
			stream.Write(BitConverter.GetBytes(graphicBoundsLeft_Pixels), 0, 4);
			stream.Write(BitConverter.GetBytes(graphicBoundsTop_Pixels), 0, 4);
			stream.Write(BitConverter.GetBytes(graphicBoundsWidth_Pixels), 0, 4);
			stream.Write(BitConverter.GetBytes(graphicBoundsHeight_Pixels), 0, 4);

			// At position 0x48 the device parameters are located: here the number of pixels of the device
			stream.Position = 0x48;
			stream.Write(BitConverter.GetBytes(devicePixelsX), 0, 4); //  the number of pixels of the device X
			stream.Write(BitConverter.GetBytes(devicePixelsY), 0, 4);  // the number of pixels of the device Y
			stream.Write(BitConverter.GetBytes(deviceSizeXMillimeter), 0, 4); // size X of the device in millimeter
			stream.Write(BitConverter.GetBytes(deviceSizeYMillimeter), 0, 4); // size Y of the device in millimeter

			if (headerSize >= (0x64 + 0x08))
			{
				stream.Position = 0x64;
				stream.Write(BitConverter.GetBytes(deviceSizeXMicrometer), 0, 4); // size X of the device in micrometer
				stream.Write(BitConverter.GetBytes(deviceSizeYMicrometer), 0, 4); // size Y of the device in micrometer
			}

			stream.Flush();

			stream.Position = 0;

			metafile.Dispose(); // we can safely dispose this metafile, because stream and metafile are independent of each other, and only the stream is patched
		}

		/// <summary>
		/// Renders a document as enhanced metafile. The metafile is rendered into a stream. You can create a metafile object afterwards from that stream.
		/// </summary>
		/// <param name="renderingProc">Procedure for rendering the document.
		/// The argument is a graphics context, which is set to GraphicsUnits equal to Points.
		/// The drawing must be inside of the boundaries of docSize.X and docSize.Y.
		/// </param>
		/// <param name="docSize">Size of the document in points (1/72 inch)</param>
		/// <param name="sourceDpiResolution">The resolution in dpi of the source. This parameter is used only if creating the reference graphics context from the current printer fails. In this case, a context from a bitmap with the provided resolution is created.</param>
		/// <param name="outputScalingFactor">Output scaling factor. If less than 1, the image will appear smaller than originally, if greater than 1, the image will appear larger than originally.</param>
		/// <param name="pixelFormat">Optional: Only used if the graphics context can not be created from a printer document. Pixel format of the bitmap that is used in this case to construct the graphics context.</param>
		/// <param name="stream">If not null, the metafile is rendered into this stream.</param>
		/// <returns>The rendered enhanced metafile (vector format).</returns>
		/// <remarks>
		/// I found no other way to realize different dpi resolutions, independently of screen or printer device contexts, as to patch the resulting metafile stream with
		/// informations about an 'artifical' device, which has exactly the resolution that is neccessary. By careful choice of the size of this artifical device one can
		/// avoid rounding errors concerning resolution and size.
		/// It happens that some programs (for instance MS Word 2010 when saving as PDF document) mess up the size of the metafile graphics, if the graphics was created with a PageUnit
		/// (of the graphics context) other than PIXELS.
		/// Thus I now always use PIXEL as PageUnit and scale the graphics context accordingly.
		/// </remarks>
		public static Metafile RenderAsEnhancedMetafile(Action<Graphics> renderingProc, PointD2D docSize, double sourceDpiResolution, double outputScalingFactor, PixelFormat pixelFormat = PixelFormat.Format32bppArgb, System.IO.Stream stream = null)
		{
			var mystream = null != stream ? stream : new MemoryStream();
			try
			{
				RenderAsEnhancedMetafileToStream(renderingProc, mystream, docSize, sourceDpiResolution, outputScalingFactor, pixelFormat);
				return new Metafile(mystream);
			}
			finally
			{
				if (null == stream) // only if stream is null, i.e. we had created a new Memorystream
					mystream.Dispose(); // we should dispose this MemoryStream
			}
		}

		/// <summary>
		/// Renders the graph document as enhanced metafile in vector format.
		/// </summary>
		/// <param name="doc">graph document.</param>
		/// <param name="stream">The stream the metafile is rendered to.</param>
		/// <param name="sourceDpiResolution">The resolution in dpi of the source.</param>
		/// <param name="outputScalingFactor">Output scaling factor. If less than 1, the image will appear smaller than originally, if greater than 1, the image will appear larger than originally.</param>
		/// <param name="backgroundBrush">The background brush. This argument can be null, or the brush can be transparent.</param>
		/// <param name="pixelFormat">Optional: Only used if the graphics context can not be created from a printer document. Pixel format of the bitmap that is used in this case to construct the graphics context.</param>
		public static void RenderAsEnhancedMetafileVectorFormatToStream(GraphDocument doc, System.IO.Stream stream, double sourceDpiResolution, double outputScalingFactor, BrushX backgroundBrush = null, PixelFormat pixelFormat = PixelFormat.Format32bppArgb)
		{
			var renderingProc = new Action<Graphics>(
					(grfxMetafile) =>
					{
						if (backgroundBrush != null)
						{
							backgroundBrush.SetEnvironment(new Graph.RectangleD(0, 0, doc.Size.X, doc.Size.Y), sourceDpiResolution);
							grfxMetafile.FillRectangle(backgroundBrush, new RectangleF(0, 0, (float)doc.Size.X, (float)doc.Size.Y));
						}

#if DIAGNOSTICLINERENDERING
				{
					grfxMetafile.DrawLine(Pens.Black, 0, 0, fDocSizeX * 2, fDocSizeY * 1.3f);

					grfxMetafile.DrawLine(Pens.Black, 0, 0, fDocSizeX, fDocSizeY);
					grfxMetafile.DrawLine(Pens.Black, 0, fDocSizeY, fDocSizeX, 0);
					grfxMetafile.DrawLine(Pens.Black, 0, 0, fDocSizeX / 4, fDocSizeY / 2);
					grfxMetafile.DrawLine(Pens.Black, 0, fDocSizeY, fDocSizeX / 4, fDocSizeY / 2);
					grfxMetafile.DrawLine(Pens.Black, fDocSizeX * 0.75f, fDocSizeY / 2, fDocSizeX, 0);
					grfxMetafile.DrawLine(Pens.Black, fDocSizeX * 0.75f, fDocSizeY / 2, fDocSizeX, fDocSizeY);
				}
#endif

						doc.DoPaint(grfxMetafile, true);
					});

			RenderAsEnhancedMetafileToStream(renderingProc, stream, doc.Size, sourceDpiResolution, outputScalingFactor, pixelFormat);
		}

		/// <summary>
		/// Renders the graph document as enhanced metafile in vector format.
		/// </summary>
		/// <param name="doc">graph document.</param>
		/// <param name="sourceDpiResolution">The resolution in dpi of the source.</param>
		/// <param name="outputScalingFactor">Output scaling factor. If less than 1, the image will appear smaller than originally, if greater than 1, the image will appear larger than originally.</param>
		/// <param name="backgroundBrush">The background brush. This argument can be null, or the brush can be transparent.</param>
		/// <param name="pixelFormat">Optional: Only used if the graphics context can not be created from a printer document. Pixel format of the bitmap that is used in this case to construct the graphics context.</param>
		/// <param name="stream">Optional: stream. If given, the metafile is rendered into the given stream.</param>
		/// <returns>The rendered enhanced metafile (vector format).</returns>
		public static Metafile RenderAsEnhancedMetafileVectorFormat(GraphDocument doc, double sourceDpiResolution, double outputScalingFactor, BrushX backgroundBrush = null, PixelFormat pixelFormat = PixelFormat.Format32bppArgb, System.IO.Stream stream = null)
		{
			var mystream = null != stream ? stream : new MemoryStream();
			try
			{
				RenderAsEnhancedMetafileVectorFormatToStream(doc, mystream, sourceDpiResolution, outputScalingFactor, backgroundBrush, pixelFormat);
				return new Metafile(mystream);
			}
			finally
			{
				if (null == stream) // only if stream is null, i.e. we had created a new Memorystream
					mystream.Dispose(); // we should dispose this MemoryStream
			}
		}

		#endregion Main work

		#region Convenience functions with export or embedded rendering options

		/// <summary>
		/// Renders the graph document as enhanced metafile image in vector format with the options given in <paramref name="exportOptions"/>
		/// </summary>
		/// <param name="doc">The graph document used.</param>
		/// <param name="exportOptions">The clipboard export options.</param>
		/// <param name="stream">Optional: if given, the metafile is additionally rendered into the stream.</param>
		/// <returns>The rendered enhanced metafile.</returns>
		public static Metafile RenderAsEnhancedMetafileVectorFormat(GraphDocument doc, EmbeddedObjectRenderingOptions exportOptions, System.IO.Stream stream = null)
		{
			return RenderAsEnhancedMetafileVectorFormat(doc, exportOptions.SourceDpiResolution, exportOptions.OutputScalingFactor, exportOptions.BackgroundBrush, PixelFormat.Format32bppArgb, stream);
		}

		/// <summary>
		/// Renders the graph document as enhanced metafile image in vector format with the options given in <paramref name="exportOptions"/>
		/// </summary>
		/// <param name="doc">The graph document used.</param>
		/// <param name="exportOptions">The clipboard export options.</param>
		/// <param name="stream">Optional: if given, the metafile is additionally rendered into the stream.</param>
		/// <returns>The rendered enhanced metafile.</returns>
		public static Metafile RenderAsEnhancedMetafileVectorFormat(this GraphDocument doc, GraphExportOptions exportOptions, System.IO.Stream stream = null)
		{
			return RenderAsEnhancedMetafileVectorFormat(doc, exportOptions.SourceDpiResolution, exportOptions.SourceDpiResolution / exportOptions.DestinationDpiResolution, exportOptions.BackgroundBrush, exportOptions.PixelFormat, stream);
		}

		/// <summary>
		/// Renders the graph document as enhanced metafile image in vector format with the options given in <paramref name="exportOptions"/>
		/// </summary>
		/// <param name="doc">The graph document used.</param>
		/// <param name="stream">The stream to which to render the metafile.</param>
		/// <param name="exportOptions">The clipboard export options.</param>
		public static void RenderAsEnhancedMetafileVectorFormatToStream(this GraphDocument doc, System.IO.Stream stream, GraphExportOptions exportOptions)
		{
			RenderAsEnhancedMetafileVectorFormatToStream(doc, stream, exportOptions.SourceDpiResolution, exportOptions.SourceDpiResolution / exportOptions.DestinationDpiResolution, exportOptions.BackgroundBrush, exportOptions.PixelFormat);
		}

		#endregion Convenience functions with export or embedded rendering options

		#region Convenience functions with filename

		public static Metafile RenderAsEnhancedMetafileVectorFormat(this GraphDocument doc, GraphExportOptions options, string filename)
		{
			Metafile mf;
			using (System.IO.Stream stream = new System.IO.FileStream(filename, System.IO.FileMode.CreateNew, System.IO.FileAccess.Write, System.IO.FileShare.Read))
			{
				mf = RenderAsEnhancedMetafileVectorFormat(doc, options, stream);
				stream.Close();
			}
			return mf;
		}

		/// <summary>
		/// Saves the graph as an bitmap file into the file <paramref name="filename"/>.
		/// </summary>
		/// <param name="doc">The graph document to export.</param>
		/// <param name="dpiResolution">Resolution of the bitmap in dpi. Determines the pixel size of the bitmap.</param>
		/// <param name="backbrush">Brush used to fill the background of the image. Can be <c>null</c>.</param>
		/// <param name="pixelformat">Specify the pixelformat here.</param>
		/// <param name="filename">The filename of the file to save the bitmap into.</param>
		public static Metafile RenderAsEnhancedMetafileVectorFormat(this GraphDocument doc, double dpiResolution, BrushX backbrush, PixelFormat pixelformat, string filename)
		{
			Metafile mf;
			using (System.IO.Stream str = new System.IO.FileStream(filename, System.IO.FileMode.CreateNew, System.IO.FileAccess.Write, System.IO.FileShare.Read))
			{
				mf = RenderAsEnhancedMetafileVectorFormat(doc, dpiResolution, 1, backbrush, pixelformat, str);
				str.Close();
			}
			return mf;
		}

		/// <summary>
		/// Saves the graph as an bitmap file into the file <paramref name="filename"/> using the default
		/// pixel format 32bppArgb and no background brush.
		/// </summary>
		/// <param name="doc">The graph document to export.</param>
		/// <param name="dpiResolution">Resolution of the bitmap in dpi. Determines the pixel size of the bitmap.</param>
		/// <param name="filename">The filename of the file to save the bitmap into.</param>
		public static Metafile RenderAsEnhancedMetafileVectorFormat(this GraphDocument doc, double dpiResolution, string filename)
		{
			return RenderAsEnhancedMetafileVectorFormat(doc, dpiResolution, null, filename);
		}

		/// <summary>
		/// Saves the graph as an bitmap file into the file <paramref name="filename"/> using the default
		/// pixel format 32bppArgb.
		/// </summary>
		/// <param name="doc">The graph document to export.</param>
		/// <param name="dpiResolution">Resolution of the bitmap in dpi. Determines the pixel size of the bitmap.</param>
		/// <param name="backbrush">Brush used to fill the background of the image. Can be <c>null</c>.</param>
		/// <param name="filename">The filename of the file to save the bitmap into.</param>
		public static Metafile RenderAsEnhancedMetafileVectorFormat(this GraphDocument doc, double dpiResolution, BrushX backbrush, string filename)
		{
			return RenderAsEnhancedMetafileVectorFormat(doc, dpiResolution, backbrush, PixelFormat.Format32bppArgb, filename);
		}

		#endregion Convenience functions with filename

		#endregion Enhanced Metafile (vector format)

		#region Enhanced Metafile (bitmap format)

		/// <summary>
		/// Creates a new metafile and renders a bitmap into it.
		/// </summary>
		/// <param name="bmp">The image to render.</param>
		/// <param name="docSize">The document size  (in points = 1/72 inch).</param>
		/// <param name="stream">Optional: if given, the metafile is additionally rendered into the stream.</param>
		/// <returns>The newly created metafile. It contains only the provided bitmap.</returns>
		public static Metafile RenderAsEnhancedMetafileBitmapFormat(System.Drawing.Bitmap bmp, PointD2D docSize, System.IO.Stream stream = null)
		{
			var renderingProc = new Action<Graphics>(
			(grfxMetafile) =>
			{
				grfxMetafile.DrawImage(bmp, 0, 0, (float)docSize.X, (float)docSize.Y);
			});

			return RenderAsEnhancedMetafile(renderingProc, docSize, Math.Max(bmp.HorizontalResolution, bmp.VerticalResolution), 1, bmp.PixelFormat, stream);
		}

		#endregion Enhanced Metafile (bitmap format)
	}
}