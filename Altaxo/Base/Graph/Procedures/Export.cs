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
    public static void SaveAsBitmap(GraphDocument doc, string filename, int dpiResolution, System.Drawing.Imaging.ImageFormat imageFormat)
    {
      SaveAsBitmap(doc, filename, dpiResolution, imageFormat, null);
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
    public static void SaveAsBitmap(GraphDocument doc, string filename, int dpiResolution, System.Drawing.Imaging.ImageFormat imageFormat, Brush backbrush)
    {
      SaveAsBitmap(doc, filename, dpiResolution, imageFormat, backbrush, PixelFormat.Format32bppArgb);
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
    public static void SaveAsBitmap(GraphDocument doc, string filename, int dpiResolution, System.Drawing.Imaging.ImageFormat imageFormat, Brush backbrush, PixelFormat pixelformat)
    {
      using (System.IO.Stream str = new System.IO.FileStream(filename, System.IO.FileMode.CreateNew, System.IO.FileAccess.Write, System.IO.FileShare.Read))
      {
        SaveAsBitmap(doc, str, dpiResolution, imageFormat, backbrush, pixelformat);
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
    public static void SaveAsBitmap(GraphDocument doc, System.IO.Stream stream, int dpiResolution, System.Drawing.Imaging.ImageFormat imageFormat)
    {
      SaveAsBitmap(doc, stream, dpiResolution, imageFormat, null, PixelFormat.Format32bppArgb);
    }


    /// <summary>
    /// Saves the graph as an bitmap file into the stream using the default pixelformat 32bppArgb.<paramref name="stream"/>.
    /// </summary>
    /// <param name="doc">The graph document to export.</param>
    /// <param name="stream">The stream to save the metafile into.</param>
    /// <param name="dpiResolution">Resolution of the bitmap in dpi. Determines the pixel size of the bitmap.</param>
    /// <param name="imageFormat">The format of the destination image.</param>
    /// <param name="backbrush">Brush used to fill the background of the image. Can be <c>null</c>.</param>
    public static void SaveAsBitmap(GraphDocument doc, System.IO.Stream stream, int dpiResolution, System.Drawing.Imaging.ImageFormat imageFormat, Brush backbrush)
    {
      SaveAsBitmap(doc, stream, dpiResolution, imageFormat, backbrush, PixelFormat.Format32bppArgb);
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
    public static void SaveAsBitmap(GraphDocument doc, System.IO.Stream stream, int dpiResolution, System.Drawing.Imaging.ImageFormat imageFormat, Brush backbrush, PixelFormat pixelformat)
    {
      System.Drawing.Bitmap bitmap = SaveAsBitmap(doc, dpiResolution, backbrush, pixelformat);

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
    public static System.Drawing.Bitmap SaveAsBitmap(GraphDocument doc, int dpiResolution, Brush backbrush, PixelFormat pixelformat)
    {
      double scale = dpiResolution / 72.0;
      // Code to write the stream goes here.

      // round the pixels to multiples of 4, many programs rely on this
      int width = (int)(4 * Math.Ceiling(0.25 * doc.PageBounds.Width * scale));
      int height = (int)(4 * Math.Ceiling(0.25 * doc.PageBounds.Height * scale));
      System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(width, height, pixelformat);
      bitmap.SetResolution(dpiResolution, dpiResolution);

      Graphics grfx = Graphics.FromImage(bitmap);
      if (null != backbrush)
        grfx.FillRectangle(backbrush, new Rectangle(0, 0, width, height));

      grfx.PageUnit = GraphicsUnit.Point;
      grfx.TranslateTransform(doc.PrintableBounds.X, doc.PrintableBounds.Y);
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
      Metafile mf = SaveAsMetafile(doc, stream,300);
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
    public static Metafile SaveAsMetafile(GraphDocument doc, string filename, int dpiResolution,  Brush backbrush, PixelFormat pixelformat)
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
    public static Metafile SaveAsMetafile(GraphDocument doc, System.IO.Stream stream, int dpiResolution,  Brush backbrush)
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
      // Create a bitmap just to have a graphics context
      System.Drawing.Bitmap helperbitmap = new System.Drawing.Bitmap(4, 4, pixelformat);
      helperbitmap.SetResolution(dpiResolution, dpiResolution);
      Graphics grfx = Graphics.FromImage(helperbitmap);
      // Code to write the stream goes here.
      IntPtr ipHdc = grfx.GetHdc();
      System.Drawing.Imaging.Metafile mf = new System.Drawing.Imaging.Metafile(stream, ipHdc, doc.PageBounds, MetafileFrameUnit.Point);
      grfx.ReleaseHdc(ipHdc);
      grfx.Dispose();
      grfx = Graphics.FromImage(mf);
      grfx.PageUnit = GraphicsUnit.Point;
      grfx.PageScale = 1;
      grfx.TranslateTransform(doc.PrintableBounds.X, doc.PrintableBounds.Y);

      doc.DoPaint(grfx, true);

      grfx.Dispose();
      helperbitmap.Dispose();
      return mf;
    }

    #endregion


  }
}
