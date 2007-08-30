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

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

namespace Altaxo.Graph.Procedures
{
  using Gdi;

  /// <summary>
  /// Routines to export graphs as bitmap
  /// </summary>
  public class Export
  {
    #region Bitmap
    /// <summary>
    /// Saves the graph as an bitmap file into the file <paramref name="filename"/> using the default
    /// pixel format 32bppArgb and no background brush.
    /// </summary>
    /// <param name="doc">The graph document to export.</param>
    /// <param name="filename">The filename of the file to save the bitmap into.</param>
    /// <param name="dpiResolution">Resolution of the bitmap in dpi. Determines the pixel size of the bitmap.</param>
    /// <param name="imageFormat">The format of the destination image.</param>
    public static void SaveAsBitmap(GraphDocument doc, string filename, int dpiResolution, System.Drawing.Imaging.ImageFormat imageFormat, bool usePageBounds)
    {
      SaveAsBitmap(doc, filename, dpiResolution, imageFormat, null, usePageBounds);
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
    public static void SaveAsBitmap(GraphDocument doc, string filename, int dpiResolution, System.Drawing.Imaging.ImageFormat imageFormat, Brush backbrush, bool usePageBounds)
    {
      SaveAsBitmap(doc, filename, dpiResolution, imageFormat, backbrush, PixelFormat.Format32bppArgb, usePageBounds);
    }


    /// <summary>
    /// Saves the graph as an bitmap file into the file <paramref name="filename"/>.
    /// </summary>
    /// <param name="doc">The graph document to export.</param>
    /// <param name="filename">The filename of the file to save the bitmap into.</param>
    /// <param name="dpiResolution">Resolution of the bitmap in dpi. Determines the pixel size of the bitmap.</param>
    /// <param name="imageFormat">The format of the destination image.</param>
    /// <param name="backbrush">Brush used to fill the background of the image. Can be <c>null</c>.</param>
    /// <param name="pixelformat">Specify the pixelformat here.</param>
    public static void SaveAsBitmap(GraphDocument doc, string filename, int dpiResolution, System.Drawing.Imaging.ImageFormat imageFormat, Brush backbrush, PixelFormat pixelformat, bool usePageBounds)
    {
      using (System.IO.Stream str = new System.IO.FileStream(filename, System.IO.FileMode.CreateNew, System.IO.FileAccess.Write, System.IO.FileShare.Read))
      {
        SaveAsBitmap(doc, str, dpiResolution, imageFormat, backbrush, pixelformat, usePageBounds);
        str.Close();
      }
    }


    /// <summary>
    /// Saves the graph as an bitmap file into the stream using the default pixelformat 32bppArgb and no background brush.<paramref name="stream"/>.
    /// </summary>
    /// <param name="doc">The graph document to export.</param>
    /// <param name="stream">The stream to save the metafile into.</param>
    /// <param name="dpiResolution">Resolution of the bitmap in dpi. Determines the pixel size of the bitmap.</param>
    /// <param name="imageFormat">The format of the destination image.</param>
    public static void SaveAsBitmap(GraphDocument doc, System.IO.Stream stream, int dpiResolution, System.Drawing.Imaging.ImageFormat imageFormat, bool usePageBounds)
    {
      SaveAsBitmap(doc, stream, dpiResolution, imageFormat, null, PixelFormat.Format32bppArgb, usePageBounds);
    }


    /// <summary>
    /// Saves the graph as an bitmap file into the stream using the default pixelformat 32bppArgb.<paramref name="stream"/>.
    /// </summary>
    /// <param name="doc">The graph document to export.</param>
    /// <param name="stream">The stream to save the metafile into.</param>
    /// <param name="dpiResolution">Resolution of the bitmap in dpi. Determines the pixel size of the bitmap.</param>
    /// <param name="imageFormat">The format of the destination image.</param>
    /// <param name="backbrush">Brush used to fill the background of the image. Can be <c>null</c>.</param>
    public static void SaveAsBitmap(GraphDocument doc, System.IO.Stream stream, int dpiResolution, System.Drawing.Imaging.ImageFormat imageFormat, Brush backbrush, bool usePageBounds)
    {
      SaveAsBitmap(doc, stream, dpiResolution, imageFormat, backbrush, PixelFormat.Format32bppArgb, usePageBounds);
    }

    /// <summary>
    /// Saves the graph as an bitmap file into the stream <paramref name="stream"/>.
    /// </summary>
    /// <param name="doc">The graph document to export.</param>
    /// <param name="stream">The stream to save the metafile into.</param>
    /// <param name="dpiResolution">Resolution of the bitmap in dpi. Determines the pixel size of the bitmap.</param>
    /// <param name="imageFormat">The format of the destination image.</param>
    /// <param name="backbrush">Brush used to fill the background of the image. Can be <c>null</c>.</param>
    /// <param name="pixelformat">Specify the pixelformat here.</param>
    public static void SaveAsBitmap(GraphDocument doc, System.IO.Stream stream, int dpiResolution, System.Drawing.Imaging.ImageFormat imageFormat, Brush backbrush, PixelFormat pixelformat, bool usePageBounds)
    {
      System.Drawing.Bitmap bitmap = SaveAsBitmap(doc, dpiResolution, backbrush, pixelformat, usePageBounds);

      bitmap.Save(stream, imageFormat);

      bitmap.Dispose();
    }


    /// <summary>
    /// Saves the graph as an bitmap file and returns the bitmap.
    /// </summary>
    /// <param name="doc">The graph document to export.</param>
    /// <param name="dpiResolution">Resolution of the bitmap in dpi. Determines the pixel size of the bitmap.</param>
    /// <param name="backbrush">Brush used to fill the background of the image. Can be <c>null</c>.</param>
    /// <param name="pixelformat">Specify the pixelformat here.</param>
    /// <returns>The saved bitmap. You should call Dispose when you no longer need the bitmap.</returns>
    public static System.Drawing.Bitmap SaveAsBitmap(GraphDocument doc, int dpiResolution, Brush backbrush, PixelFormat pixelformat, bool usePageBounds)
    {
      double scale = dpiResolution / 72.0;
      // Code to write the stream goes here.
      int width, height;
      if (usePageBounds)
      {
        // round the pixels to multiples of 4, many programs rely on this
        width = (int)(4 * Math.Ceiling(0.25 * doc.PageBounds.Width * scale));
        height = (int)(4 * Math.Ceiling(0.25 * doc.PageBounds.Height * scale));
      }
      else // usePrintableBounds
      {
        // round the pixels to multiples of 4, many programs rely on this
        width = (int)(4 * Math.Ceiling(0.25 * doc.PrintableBounds.Width * scale));
        height = (int)(4 * Math.Ceiling(0.25 * doc.PrintableBounds.Height * scale));
      }

      System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(width, height, pixelformat);

      bitmap.SetResolution(dpiResolution, dpiResolution);

      Graphics grfx = Graphics.FromImage(bitmap);
      if (null != backbrush)
        grfx.FillRectangle(backbrush, new Rectangle(0, 0, width, height));

      grfx.PageUnit = GraphicsUnit.Point;
      if (usePageBounds)
      {
        grfx.TranslateTransform(doc.PrintableBounds.X, doc.PrintableBounds.Y);
      }
      grfx.PageScale = 1; // (float)scale;


      doc.DoPaint(grfx, true);

      grfx.Dispose();

      return bitmap;
    }


    #endregion

    #region Metafile

    public static Metafile GetMetafile(GraphDocument doc)
    {
      System.IO.MemoryStream stream = new System.IO.MemoryStream();
      Metafile mf = SaveAsMetafile(doc, stream, 300);
      stream.Flush();
      stream.Close();
      return mf;
    }

    /// <summary>
    /// Saves the graph as an bitmap file into the file <paramref name="filename"/> using the default
    /// pixel format 32bppArgb and no background brush.
    /// </summary>
    /// <param name="doc">The graph document to export.</param>
    /// <param name="filename">The filename of the file to save the bitmap into.</param>
    /// <param name="dpiResolution">Resolution of the bitmap in dpi. Determines the pixel size of the bitmap.</param>
    public static Metafile SaveAsMetafile(GraphDocument doc, string filename, int dpiResolution)
    {
      return SaveAsMetafile(doc, filename, dpiResolution, null);
    }

    /// <summary>
    /// Saves the graph as an bitmap file into the file <paramref name="filename"/> using the default
    /// pixel format 32bppArgb.
    /// </summary>
    /// <param name="doc">The graph document to export.</param>
    /// <param name="filename">The filename of the file to save the bitmap into.</param>
    /// <param name="dpiResolution">Resolution of the bitmap in dpi. Determines the pixel size of the bitmap.</param>
    /// <param name="backbrush">Brush used to fill the background of the image. Can be <c>null</c>.</param>
    public static Metafile SaveAsMetafile(GraphDocument doc, string filename, int dpiResolution, Brush backbrush)
    {
      return SaveAsMetafile(doc, filename, dpiResolution, backbrush, PixelFormat.Format32bppArgb);
    }


    /// <summary>
    /// Saves the graph as an bitmap file into the file <paramref name="filename"/>.
    /// </summary>
    /// <param name="doc">The graph document to export.</param>
    /// <param name="filename">The filename of the file to save the bitmap into.</param>
    /// <param name="dpiResolution">Resolution of the bitmap in dpi. Determines the pixel size of the bitmap.</param>
    /// <param name="backbrush">Brush used to fill the background of the image. Can be <c>null</c>.</param>
    /// <param name="pixelformat">Specify the pixelformat here.</param>
    public static Metafile SaveAsMetafile(GraphDocument doc, string filename, int dpiResolution, Brush backbrush, PixelFormat pixelformat)
    {
      Metafile mf;
      using (System.IO.Stream str = new System.IO.FileStream(filename, System.IO.FileMode.CreateNew, System.IO.FileAccess.Write, System.IO.FileShare.Read))
      {
        mf = SaveAsMetafile(doc, str, dpiResolution, backbrush, pixelformat);
        str.Close();
      }
      return mf;
    }


    /// <summary>
    /// Saves the graph as an bitmap file into the stream using the default pixelformat 32bppArgb and no background brush.<paramref name="stream"/>.
    /// </summary>
    /// <param name="doc">The graph document to export.</param>
    /// <param name="stream">The stream to save the metafile into.</param>
    /// <param name="dpiResolution">Resolution of the bitmap in dpi. Determines the pixel size of the bitmap.</param>
    public static Metafile SaveAsMetafile(GraphDocument doc, System.IO.Stream stream, int dpiResolution)
    {
      return SaveAsMetafile(doc, stream, dpiResolution, null, PixelFormat.Format32bppArgb);
    }


    /// <summary>
    /// Saves the graph as an bitmap file into the stream using the default pixelformat 32bppArgb.<paramref name="stream"/>.
    /// </summary>
    /// <param name="doc">The graph document to export.</param>
    /// <param name="stream">The stream to save the metafile into.</param>
    /// <param name="dpiResolution">Resolution of the bitmap in dpi. Determines the pixel size of the bitmap.</param>
    /// <param name="backbrush">Brush used to fill the background of the image. Can be <c>null</c>.</param>
    public static Metafile SaveAsMetafile(GraphDocument doc, System.IO.Stream stream, int dpiResolution, Brush backbrush)
    {
      return SaveAsMetafile(doc, stream, dpiResolution, backbrush, PixelFormat.Format32bppArgb);
    }

    /// <summary>
    /// Saves the graph as an enhanced windows metafile into the stream <paramref name="stream"/>.
    /// </summary>
    /// <param name="doc">The graph document used.</param>
    /// <param name="stream">The stream to save the metafile into.</param>
    /// <param name="dpiResolution">Resolution of the bitmap in dpi. Determines the pixel size of the bitmap.</param>
    /// <param name="backbrush">Brush used to fill the background of the image. Can be <c>null</c>.</param>
    /// <param name="pixelformat">The pixel format to use.</param>
    /// <returns>The metafile that was created using the stream.</returns>
    public static Metafile SaveAsMetafile(GraphDocument doc, System.IO.Stream stream, int dpiResolution, Brush backbrush, PixelFormat pixelformat)
    {
      Metafile mf = null;

      // it is preferable to use a graphics context from a printer to create the metafile, in this case
      // the metafile will become device independent (metaFile.GetMetaFileHeader().IsDisplay() will return false)
      // Only when no printer is installed, we use a graphics context from a bitmap, but this will lead
      // to wrong positioning / wrong boundaries depending on the current screen
      if (Current.PrintingService != null && 
        Current.PrintingService.PrintDocument != null &&
        Current.PrintingService.PrintDocument.PrinterSettings !=null
        )
      {
        Graphics grfx = Current.PrintingService.PrintDocument.PrinterSettings.CreateMeasurementGraphics();
        mf = SaveAsMetafile(grfx, doc, stream, dpiResolution, backbrush, pixelformat);
        grfx.Dispose();
      }
      else
      {
        // Create a bitmap just to get a graphics context from it
        System.Drawing.Bitmap helperbitmap = new System.Drawing.Bitmap(4, 4, pixelformat);
        helperbitmap.SetResolution(dpiResolution, dpiResolution);
        Graphics grfx = Graphics.FromImage(helperbitmap);
        grfx.PageUnit = GraphicsUnit.Point;
        mf = SaveAsMetafile(grfx, doc, stream, dpiResolution, backbrush, pixelformat);
        grfx.Dispose();
        helperbitmap.Dispose();
      }

      return mf;
    }


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
    public static Metafile SaveAsMetafile(Graphics grfx, GraphDocument doc, System.IO.Stream stream, int dpiResolution, Brush backbrush, PixelFormat pixelformat)
    {
      bool usePageBoundaries = false;

      grfx.PageUnit = GraphicsUnit.Point;
      IntPtr ipHdc = grfx.GetHdc();

      RectangleF metaFileBounds;
      if (usePageBoundaries)
        metaFileBounds = doc.PageBounds;
      else
        metaFileBounds = new RectangleF(PointF.Empty, doc.PrintableSize);

      System.Drawing.Imaging.Metafile mf = new System.Drawing.Imaging.Metafile(stream, ipHdc, metaFileBounds, MetafileFrameUnit.Point);
      using (Graphics grfx2 = Graphics.FromImage(mf))
      {

        if (Environment.OSVersion.Version.Major < 6 || !mf.GetMetafileHeader().IsDisplay())
        {
          grfx2.PageUnit = GraphicsUnit.Point;
          grfx2.PageScale = 1; // that would not work properly (a bug?) in Windows Vista, instead we have to use the following:
        }
        else
        {
          grfx2.PageScale = Math.Min(72.0f / grfx2.DpiX, 72.0f / grfx2.DpiY); // this works in Vista with display mode
        }

        if (usePageBoundaries)
          grfx2.TranslateTransform(doc.PrintableBounds.X, doc.PrintableBounds.Y);

        doc.DoPaint(grfx2, true);

        grfx2.Dispose();
      }

      grfx.ReleaseHdc(ipHdc);



      return mf;
    }

    #endregion


  }
}
