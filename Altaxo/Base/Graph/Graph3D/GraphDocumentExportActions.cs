using Altaxo.Drawing;
using Altaxo.Geometry;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;

namespace Altaxo.Graph.Graph3D
{
	public static class GraphDocumentExportActions
	{
		/// <summary>
		/// Saves the graph as an bitmap file and returns the bitmap.
		/// </summary>
		/// <param name="doc">The graph document to export.</param>
		/// <param name="backbrush">Brush used to fill the background of the image. Can be <c>null</c>.</param>
		/// <param name="pixelformat">Specify the pixelformat here.</param>
		/// <param name="sourceDpiResolution">Resolution at which the graph document is rendered into a bitmap.</param>
		/// <param name="destinationDpiResolution">Resolution which is assigned to the bitmap. This determines the physical size of the bitmap.</param>
		/// <returns>The saved bitmap. You should call Dispose when you no longer need the bitmap.</returns>
		public static Bitmap RenderAsBitmap(this GraphDocument doc, Altaxo.Graph.Gdi.BrushX backbrush, PixelFormat pixelformat, double sourceDpiResolution, double destinationDpiResolution)
		{
			return RenderAsBitmap(doc, backbrush, null, pixelformat, sourceDpiResolution, destinationDpiResolution);
		}

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
		public static Bitmap RenderAsBitmap(this GraphDocument doc, Altaxo.Graph.Gdi.BrushX backbrush1, Altaxo.Graph.Gdi.BrushX backbrush2, PixelFormat pixelformat, double sourceDpiResolution, double destinationDpiResolution)
		{
			var imageExporter = Current.ProjectService.GetProjectItemImageExporter(doc);
			if (null == imageExporter)
				return null;

			Bitmap bitmap = null;

			using (var memStream = new System.IO.MemoryStream())
			{
				var exportOptions = new Altaxo.Graph.Gdi.GraphExportOptions();
				exportOptions.TrySetImageAndPixelFormat(ImageFormat.Png, PixelFormat.Format32bppArgb);
				exportOptions.SourceDpiResolution = sourceDpiResolution;
				exportOptions.DestinationDpiResolution = destinationDpiResolution;
				exportOptions.BackgroundBrush = backbrush1;
				imageExporter.ExportAsImageToStream(doc, exportOptions, memStream);

				memStream.Seek(0, System.IO.SeekOrigin.Begin);
				bitmap = (Bitmap)Bitmap.FromStream(memStream);
			}

			int bmpWidth = bitmap.Width;
			int bmpHeight = bitmap.Height;
			/*
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
					backbrush1.SetEnvironment(new RectangleD2D(0, 0, doc.Size.X, doc.Size.Y), sourceDpiResolution);
					grfx.FillRectangle(backbrush1, new RectangleF(0, 0, (float)doc.Size.X, (float)doc.Size.Y));
				}

				if (null != backbrush2)
				{
					backbrush2.SetEnvironment(new RectangleD2D(0, 0, doc.Size.X, doc.Size.Y), sourceDpiResolution);
					grfx.FillRectangle(backbrush2, new RectangleF(0, 0, (float)doc.Size.X, (float)doc.Size.Y));
				}
			}
			*/

			bitmap.SetResolution((float)destinationDpiResolution, (float)destinationDpiResolution);

			return bitmap;
		}
	}
}