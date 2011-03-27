using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Altaxo.Graph.Gdi
{
	public static class GraphDocumentExportActions
	{
		static IList<KeyValuePair<string,string>> GetFileFilterString(ImageFormat fmt)
		{
			List<KeyValuePair<string, string>> filter = new List<KeyValuePair<string, string>>();

			if (fmt == ImageFormat.Bmp)
				filter.Add(new KeyValuePair<string,string>("*.bmp", "Bitmap files (*.bmp)"));
			else if (fmt == ImageFormat.Emf)
				filter.Add(new KeyValuePair<string,string>("*.emf", "Enhanced metafiles (*.emf)"));
			else if (ImageFormat.Exif == fmt)
				filter.Add(new KeyValuePair<string,string>("*.exi", "Exif files (*.exi)"));
			else if (ImageFormat.Gif == fmt)
				filter.Add(new KeyValuePair<string,string>("*.gif", "Gif files (*.gif)"));
			else if (ImageFormat.Icon == fmt)
				filter.Add(new KeyValuePair<string,string>("*.ico", "Icon files (*.ico)"));
			else if (ImageFormat.Jpeg == fmt)
				filter.Add(new KeyValuePair<string,string>("*.jpg", "Jpeg files (*.jpg)"));
			else if (ImageFormat.Png == fmt)
				filter.Add(new KeyValuePair<string,string>("*.png", "Png files (*.png)"));
			else if (ImageFormat.Tiff == fmt)
				filter.Add(new KeyValuePair<string,string>("*.tif", "Tiff files (*.tif)"));
			else if (ImageFormat.Wmf == fmt)
				filter.Add(new KeyValuePair<string,string>("*.wmf", "Windows metafiles (*.wmf)"));

			filter.Add(new KeyValuePair<string,string>("*.*", "All files (*.*)"));

			return filter;
		}

		static GraphExportOptions _graphExportOptionsToFile = new GraphExportOptions();

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
			opt.ExportArea = GraphExportArea.Page;
			ShowFileExportDialog(doc, opt);
		}
		public static void ShowFileExportTiffDialog(this GraphDocument doc)
		{
			var opt = new GraphExportOptions();
			opt.IsIntentedForClipboardOperation = false;
			opt.TrySetImageAndPixelFormat(ImageFormat.Tiff, PixelFormat.Format32bppArgb);
			opt.ExportArea = GraphExportArea.Page;
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

		#endregion

		#region Bitmap

		#region main work

		/// <summary>
		/// Saves the graph as an bitmap file and returns the bitmap.
		/// </summary>
		/// <param name="doc">The graph document to export.</param>
		/// <param name="dpiResolution">Resolution of the bitmap in dpi. Determines the pixel size of the bitmap.</param>
		/// <param name="backbrush">Brush used to fill the background of the image. Can be <c>null</c>.</param>
		/// <param name="pixelformat">Specify the pixelformat here.</param>
		/// <returns>The saved bitmap. You should call Dispose when you no longer need the bitmap.</returns>
		public static Bitmap RenderAsBitmap(this GraphDocument doc, Brush backbrush, PixelFormat pixelformat, GraphExportArea areaToExport, double sourceDpiResolution, double destinationDpiResolution)
		{
			double scale = sourceDpiResolution / 72.0;
			// Code to write the stream goes here.
			int width, height;
			switch (areaToExport)
			{
				case GraphExportArea.GraphSize:
					// round the pixels to multiples of 4, many programs rely on this
					width = (int)(4 * Math.Ceiling(0.25 * doc.Layers.GraphSize.Width * scale));
					height = (int)(4 * Math.Ceiling(0.25 * doc.Layers.GraphSize.Height * scale));
					break;
				case GraphExportArea.Page:
					// round the pixels to multiples of 4, many programs rely on this
					width = (int)(4 * Math.Ceiling(0.25 * doc.PageBounds.Width * scale));
					height = (int)(4 * Math.Ceiling(0.25 * doc.PageBounds.Height * scale));
					break;
				case GraphExportArea.PrintableArea:
					// round the pixels to multiples of 4, many programs rely on this
					width = (int)(4 * Math.Ceiling(0.25 * doc.PrintableBounds.Width * scale));
					height = (int)(4 * Math.Ceiling(0.25 * doc.PrintableBounds.Height * scale));
					break;
				case GraphExportArea.BoundingBox:
					// round the pixels to multiples of 4, many programs rely on this
					width = (int)(4 * Math.Ceiling(0.25 * doc.PrintableBounds.Width * scale));
					height = (int)(4 * Math.Ceiling(0.25 * doc.PrintableBounds.Height * scale));
					break;
				default:
					throw new ArgumentException("areaToExport unkown: " + areaToExport.ToString());
					break;
			}
			

			System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(width, height, pixelformat);

			bitmap.SetResolution((float)sourceDpiResolution, (float)sourceDpiResolution);

			Graphics grfx = Graphics.FromImage(bitmap);
			if (null != backbrush)
				grfx.FillRectangle(backbrush, new Rectangle(0, 0, width, height));

			grfx.PageUnit = GraphicsUnit.Point;

			float zoom;
			PointF startLocationOnPage;

			switch (areaToExport)
			{
				default:
				case GraphExportArea.GraphSize:
					break;
				case GraphExportArea.Page:
					doc.PrintOptions.GetZoomAndStartLocation(doc.PageBounds, doc.PrintableBounds, doc.Layers.GraphSize, out zoom, out startLocationOnPage, false);
					grfx.TranslateTransform(-startLocationOnPage.X, -startLocationOnPage.Y);
					grfx.ScaleTransform(zoom, zoom);
					break;
				case GraphExportArea.PrintableArea:
					doc.PrintOptions.GetZoomAndStartLocation(doc.PageBounds, doc.PrintableBounds, doc.Layers.GraphSize, out zoom, out startLocationOnPage, false);
					grfx.TranslateTransform(-startLocationOnPage.X + doc.PrintableBounds.X, -startLocationOnPage.Y + doc.PrintableBounds.Y);
					grfx.ScaleTransform(zoom, zoom); 
					break;
			}
			grfx.PageScale = 1; // (float)scale;


			doc.DoPaint(grfx, true);

			grfx.Dispose();

			bitmap.SetResolution((float)destinationDpiResolution, (float)destinationDpiResolution);

			return bitmap;
		}


		#endregion

		#region stream

		/// <summary>
		/// Saves the graph as an bitmap file into the stream <paramref name="stream"/>.
		/// </summary>
		/// <param name="doc">The graph document to export.</param>
		/// <param name="stream">The stream to save the metafile into.</param>
		/// <param name="dpiResolution">Resolution of the bitmap in dpi. Determines the pixel size of the bitmap.</param>
		/// <param name="imageFormat">The format of the destination image.</param>
		/// <param name="backbrush">Brush used to fill the background of the image. Can be <c>null</c>.</param>
		/// <param name="pixelformat">Specify the pixelformat here.</param>
		public static void RenderAsBitmap(this GraphDocument doc, System.IO.Stream stream, System.Drawing.Imaging.ImageFormat imageFormat, Brush backbrush, PixelFormat pixelformat, GraphExportArea usePageBounds, double sourceDpiResolution, double destinationDpiResolution)
		{
			System.Drawing.Bitmap bitmap = RenderAsBitmap(doc, backbrush, pixelformat, usePageBounds, sourceDpiResolution, destinationDpiResolution);

			bitmap.Save(stream, imageFormat);

			bitmap.Dispose();
		}

		/// <summary>
		/// Saves the graph as an bitmap file into the stream using the default pixelformat 32bppArgb and no background brush.<paramref name="stream"/>.
		/// </summary>
		/// <param name="doc">The graph document to export.</param>
		/// <param name="stream">The stream to save the metafile into.</param>
		/// <param name="dpiResolution">Resolution of the bitmap in dpi. Determines the pixel size of the bitmap.</param>
		/// <param name="imageFormat">The format of the destination image.</param>
		public static void RenderAsBitmap(this GraphDocument doc, System.IO.Stream stream, System.Drawing.Imaging.ImageFormat imageFormat, GraphExportArea usePageBounds, double dpiResolution)
		{
			RenderAsBitmap(doc, stream, imageFormat, null, PixelFormat.Format32bppArgb, usePageBounds, dpiResolution, dpiResolution);
		}


		/// <summary>
		/// Saves the graph as an bitmap file into the stream using the default pixelformat 32bppArgb.<paramref name="stream"/>.
		/// </summary>
		/// <param name="doc">The graph document to export.</param>
		/// <param name="stream">The stream to save the metafile into.</param>
		/// <param name="dpiResolution">Resolution of the bitmap in dpi. Determines the pixel size of the bitmap.</param>
		/// <param name="imageFormat">The format of the destination image.</param>
		/// <param name="backbrush">Brush used to fill the background of the image. Can be <c>null</c>.</param>
		public static void RenderAsBitmap(this GraphDocument doc, System.IO.Stream stream, System.Drawing.Imaging.ImageFormat imageFormat, Brush backbrush, GraphExportArea usePageBounds, double dpiResolution)
		{
			RenderAsBitmap(doc, stream, imageFormat, backbrush, PixelFormat.Format32bppArgb, usePageBounds, dpiResolution, dpiResolution);
		}

		public static void RenderAsBitmap(this GraphDocument doc, System.IO.Stream stream, GraphExportOptions options)
		{
			RenderAsBitmap(doc, stream, options.ImageFormat, options.GetBrushOrDefaultBrush(), options.PixelFormat, options.ExportArea, options.SourceDpiResolution, options.DestinationDpiResolution);
		}


		#endregion


		#region file name

		/// <summary>
		/// Saves the graph as an bitmap file into the file <paramref name="filename"/>.
		/// </summary>
		/// <param name="doc">The graph document to export.</param>
		/// <param name="filename">The filename of the file to save the bitmap into.</param>
		/// <param name="dpiResolution">Resolution of the bitmap in dpi. Determines the pixel size of the bitmap.</param>
		/// <param name="imageFormat">The format of the destination image.</param>
		/// <param name="backbrush">Brush used to fill the background of the image. Can be <c>null</c>.</param>
		/// <param name="pixelformat">Specify the pixelformat here.</param>
		public static void RenderAsBitmap(this GraphDocument doc, string filename, System.Drawing.Imaging.ImageFormat imageFormat, Brush backbrush, PixelFormat pixelformat, GraphExportArea usePageBounds, double sourceDpiResolution, double destinationDpiResolution)
		{
			using (System.IO.Stream str = new System.IO.FileStream(filename, System.IO.FileMode.CreateNew, System.IO.FileAccess.Write, System.IO.FileShare.Read))
			{
				RenderAsBitmap(doc, str, imageFormat, backbrush, pixelformat, usePageBounds, sourceDpiResolution, destinationDpiResolution);
				str.Close();
			}
		}

		/// <summary>
		/// Saves the graph as an bitmap file into the file <paramref name="filename"/> using the default
		/// pixel format 32bppArgb and no background brush.
		/// </summary>
		/// <param name="doc">The graph document to export.</param>
		/// <param name="filename">The filename of the file to save the bitmap into.</param>
		/// <param name="dpiResolution">Resolution of the bitmap in dpi. Determines the pixel size of the bitmap.</param>
		/// <param name="imageFormat">The format of the destination image.</param>
		public static void RenderAsBitmap(this GraphDocument doc, string filename, System.Drawing.Imaging.ImageFormat imageFormat, GraphExportArea usePageBounds, double dpiResolution)
		{
			RenderAsBitmap(doc, filename, imageFormat, null, usePageBounds, dpiResolution);
		}

		/// <summary>
		/// Saves the graph as an bitmap file into the file <paramref name="filename"/> using the default
		/// pixel format 32bppArgb.
		/// </summary>
		/// <param name="doc">The graph document to export.</param>
		/// <param name="filename">The filename of the file to save the bitmap into.</param>
		/// <param name="dpiResolution">Resolution of the bitmap in dpi. Determines the pixel size of the bitmap.</param>
		/// <param name="imageFormat">The format of the destination image.</param>
		/// <param name="backbrush">Brush used to fill the background of the image. Can be <c>null</c>.</param>
		public static void RenderAsBitmap(this GraphDocument doc, string filename, System.Drawing.Imaging.ImageFormat imageFormat, Brush backbrush, GraphExportArea usePageBounds, double dpiResolution)
		{
			RenderAsBitmap(doc, filename, imageFormat, backbrush, PixelFormat.Format32bppArgb, usePageBounds, dpiResolution, dpiResolution);
		}

		public static void RenderAsBitmap(this GraphDocument doc, string filename, GraphExportOptions options)
		{
			RenderAsBitmap(doc, filename, options.ImageFormat, options.BackgroundBrush, options.PixelFormat, options.ExportArea, options.SourceDpiResolution, options.DestinationDpiResolution);
		}


		#endregion

		#region Bitmap

		public static Bitmap RenderAsBitmap(this GraphDocument doc, GraphExportOptions options)
		{
			return RenderAsBitmap(doc, options.BackgroundBrush, options.PixelFormat, options.ExportArea, options.SourceDpiResolution, options.DestinationDpiResolution);
		}
		#endregion


		#endregion

		#region Metafile

		#region Main work

		/// <summary>
		/// Saves the graph as an enhanced windows metafile into the stream <paramref name="stream"/>.
		/// </summary>
		/// <param name="grfx">The graphics context used to create the metafile.</param>
		/// <param name="doc">The graph document used.</param>
		/// <param name="stream">The stream to save the metafile into.</param>
		/// <param name="dpiResolution">Resolution of the bitmap in dpi. Determines the pixel size of the bitmap.</param>
		/// <param name="backbrush">Brush used to fill the background of the image. Can be <c>null</c>.</param>
		/// <param name="pixelformat">The pixel format to use.</param>
		/// <returns>The metafile that was created using the stream.</returns>
		public static Metafile RenderAsMetafile(GraphDocument doc, Graphics grfx, System.IO.Stream stream, Brush backbrush, PixelFormat pixelformat, GraphExportArea area, double scale)
		{
			bool usePageBoundaries = false;

			grfx.PageUnit = GraphicsUnit.Point;
			IntPtr ipHdc = grfx.GetHdc();

			RectangleF metaFileBounds;
			switch (area)
			{
				default:
				case GraphExportArea.GraphSize:
					metaFileBounds = new RectangleF(0, 0, (float)(doc.Layers.GraphSize.Width * scale), (float)(doc.Layers.GraphSize.Height * scale));
					break;
				case GraphExportArea.Page:
					metaFileBounds = new RectangleF(0, 0, (float)(doc.PageBounds.Width * scale), (float)(doc.PageBounds.Height * scale));
					break;
				
				case GraphExportArea.PrintableArea:
					metaFileBounds = new RectangleF(0, 0, (float)(doc.PrintableBounds.Width * scale), (float)(doc.PrintableBounds.Height * scale));
					break;
			}

			System.Drawing.Imaging.Metafile mf = new System.Drawing.Imaging.Metafile(stream, ipHdc, metaFileBounds, MetafileFrameUnit.Point);
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

				float zoom;
				PointF startLocationOnPage;
				switch (area)
				{
					case GraphExportArea.Page:
						doc.PrintOptions.GetZoomAndStartLocation(doc.PageBounds, doc.PrintableBounds, doc.Layers.GraphSize, out zoom, out startLocationOnPage, false);
						grfx2.TranslateTransform(-startLocationOnPage.X, -startLocationOnPage.Y);
						grfx2.ScaleTransform(zoom, zoom);
						break;
					case GraphExportArea.PrintableArea:
						doc.PrintOptions.GetZoomAndStartLocation(doc.PageBounds, doc.PrintableBounds, doc.Layers.GraphSize, out zoom, out startLocationOnPage, false);
						grfx2.TranslateTransform(-startLocationOnPage.X+doc.PrintableBounds.X, -startLocationOnPage.Y+doc.PrintableBounds.Y);
						grfx2.ScaleTransform(zoom, zoom); 
						break;
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
		/// <param name="dpiResolution">Resolution of the bitmap in dpi. Determines the pixel size of the bitmap.</param>
		/// <returns>The metafile that was created using the stream.</returns>
		public static Metafile RenderAsMetafile(this GraphDocument doc, System.IO.Stream stream, Brush backbrush, PixelFormat pixelformat, GraphExportArea area, double sourceDpiResolution, double destinationDpiResolution)
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
				mf = RenderAsMetafile(doc, grfx, stream, backbrush, pixelformat, area, sourceDpiResolution / destinationDpiResolution);
				grfx.Dispose();
			}
			else
			{
				// Create a bitmap just to get a graphics context from it
				System.Drawing.Bitmap helperbitmap = new System.Drawing.Bitmap(4, 4, pixelformat);
				helperbitmap.SetResolution((float)sourceDpiResolution, (float)sourceDpiResolution);
				Graphics grfx = Graphics.FromImage(helperbitmap);
				grfx.PageUnit = GraphicsUnit.Point;
				mf = RenderAsMetafile(doc, grfx, stream, backbrush, pixelformat, area, sourceDpiResolution / destinationDpiResolution);
				grfx.Dispose();
				helperbitmap.Dispose();
			}

			return mf;
		}




		#endregion

		#region with stream

		public static Metafile RenderAsMetafile(this GraphDocument doc, System.IO.Stream stream, GraphExportOptions options)
		{
			return RenderAsMetafile(doc, stream, options.BackgroundBrush, options.PixelFormat, options.ExportArea, options.SourceDpiResolution, options.DestinationDpiResolution);
		}

		/// <summary>
		/// Saves the graph as an bitmap file into the stream using the default pixelformat 32bppArgb.<paramref name="stream"/>.
		/// </summary>
		/// <param name="doc">The graph document to export.</param>
		/// <param name="stream">The stream to save the metafile into.</param>
		/// <param name="backbrush">Brush used to fill the background of the image. Can be <c>null</c>.</param>
		/// <param name="dpiResolution">Resolution of the bitmap in dpi. Determines the pixel size of the bitmap.</param>
		public static Metafile RenderAsMetafile(this GraphDocument doc, System.IO.Stream stream, Brush backbrush, double sourceDpiResolution, double destinationDpiResolution)
		{
			return RenderAsMetafile(doc, stream, backbrush, PixelFormat.Format32bppArgb, GraphExportArea.PrintableArea, sourceDpiResolution, destinationDpiResolution);
		}

		/// <summary>
		/// Saves the graph as an bitmap file into the stream using the default pixelformat 32bppArgb and no background brush.<paramref name="stream"/>.
		/// </summary>
		/// <param name="doc">The graph document to export.</param>
		/// <param name="stream">The stream to save the metafile into.</param>
		/// <param name="dpiResolution">Resolution of the bitmap in dpi. Determines the pixel size of the bitmap.</param>
		public static Metafile RenderAsMetafile(this GraphDocument doc, System.IO.Stream stream, double dpiResolution)
		{
			return RenderAsMetafile(doc, stream, null, PixelFormat.Format32bppArgb, GraphExportArea.PrintableArea, dpiResolution, dpiResolution);
		}



		#endregion

		#region with filename

		public static Metafile RenderAsMetafile(this GraphDocument doc, string filename, GraphExportOptions options)
		{
			Metafile mf;
			using (System.IO.Stream str = new System.IO.FileStream(filename, System.IO.FileMode.CreateNew, System.IO.FileAccess.Write, System.IO.FileShare.Read))
			{
				mf = RenderAsMetafile(doc, str, options.BackgroundBrush, options.PixelFormat, options.ExportArea, options.SourceDpiResolution, options.DestinationDpiResolution);
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
				mf = RenderAsMetafile(doc, str, backbrush, pixelformat, GraphExportArea.PrintableArea, dpiResolution, dpiResolution);
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

		#endregion

		#region default rendering

		public static Metafile RenderAsMetafile(this GraphDocument doc)
		{
			System.IO.MemoryStream stream = new System.IO.MemoryStream();
			Metafile mf = RenderAsMetafile(doc, stream, 300);
			stream.Flush();
			stream.Close();
			return mf;
		}


		#endregion









		#endregion


	}

	/// <summary>
	/// Enumerates the area which is to be exported.
	/// </summary>
	public enum GraphExportArea
	{
		/// <summary>
		/// Area is the graph size.
		/// </summary>
		GraphSize,
		/// <summary>Area is the whole page size. Depending on printing options.</summary>
		Page,
		/// <summary>The printable area of the page.</summary>
		PrintableArea,
		/// <summary>The bounding box of all graph items.</summary>
		BoundingBox
	}

  /// <summary>
  /// Designates how to store the copied page in the clipboard.
  /// </summary>
  [Flags]
  public enum GraphCopyPageClipboardFormat
  {
    /// <summary>Store both as native image and store in temporary file and set the file name in the clipboard as DropDownList.</summary>
    AsNativeAndDropDownList=3,
    /// <summary>Store as native image.</summary>
    AsNative=1,
    /// <summary>Store in a temporary file and set the file name in the clipboard as DropDownList.</summary>
    AsDropDownList=2
  }

	public class GraphExportOptions
	{
		ImageFormat _imageFormat;
		PixelFormat _pixelFormat;
		BrushX _backgroundBrush;
		double _sourceDpiResolution;
		double _destinationDpiResolution;
    public GraphExportArea ExportArea { get; set; }
    public bool IsIntentedForClipboardOperation { get; set; }
    public GraphCopyPageClipboardFormat ClipboardFormat { get; set; }


    public void CopyFrom(object fr)
    {
			if (object.ReferenceEquals(this, fr))
				return;

      var from = fr as GraphExportOptions;
      if (null == from)
        throw new ArgumentException("Argument either null or has wrong type");

      this._imageFormat = from.ImageFormat;
      this._pixelFormat = from.PixelFormat;
      this._backgroundBrush = null == from._backgroundBrush ? null : from._backgroundBrush.Clone();
      this.SourceDpiResolution = from.SourceDpiResolution;
      this.DestinationDpiResolution = from.DestinationDpiResolution;
      this.ExportArea = from.ExportArea;
      this.IsIntentedForClipboardOperation = from.IsIntentedForClipboardOperation;
      this.ClipboardFormat = from.ClipboardFormat;
    }

    public GraphExportOptions()
    {
      this._imageFormat = System.Drawing.Imaging.ImageFormat.Emf;
      this._pixelFormat = System.Drawing.Imaging.PixelFormat.Format32bppArgb;
      this.ExportArea = GraphExportArea.GraphSize;
      this.SourceDpiResolution = 300;
      this.DestinationDpiResolution = 300;
      this.BackgroundBrush = null;
      this.IsIntentedForClipboardOperation = false;
      this.ClipboardFormat = GraphCopyPageClipboardFormat.AsNativeAndDropDownList;
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
    

		static GraphExportOptions _currentSetting = new GraphExportOptions();
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
