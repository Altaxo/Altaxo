#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2005 Dr. Dirk Lellinger
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
  /// <summary>
  /// Routines to export graphs as bitmap
  /// </summary>
  public class Export
  {
    /// <summary>
    /// Saves the graph as an bitmap file into the file <paramref name="filename"/> using the default
    /// pixel format 32bppArgb and no background brush.
    /// </summary>
    /// <param name="doc">The graph document to export.</param>
    /// <param name="stream">The filename of the file to save the bitmap into.</param>
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
    /// <param name="stream">The filename of the file to save the bitmap into.</param>
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
    /// <param name="stream">The filename of the file to save the bitmap into.</param>
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

      double scale = dpiResolution / 72.0;
      // Code to write the stream goes here.

      // round the pixels to multiples of 4, many programs rely on this
      int width = (int)(4*Math.Ceiling(0.25 * doc.PageBounds.Width * scale));
      int height = (int)(4*Math.Ceiling(0.25 * doc.PageBounds.Height * scale));
      System.Drawing.Bitmap mf = new System.Drawing.Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
      mf.SetResolution(dpiResolution, dpiResolution);

      Graphics grfx = Graphics.FromImage(mf);
      if(null != backbrush)
        grfx.FillRectangle(backbrush,new Rectangle(0,0,width,height));

      grfx.PageUnit = GraphicsUnit.Point;
      grfx.TranslateTransform(doc.PrintableBounds.X, doc.PrintableBounds.Y);
      grfx.PageScale = 1; // (float)scale;


      doc.DoPaint(grfx, true);

      grfx.Dispose();

      mf.Save(stream, imageFormat);

      mf.Dispose();

       
    }


  }
}
