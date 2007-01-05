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
//    along with this program; if not, write to the Free Software
//    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
//
/////////////////////////////////////////////////////////////////////////////
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace Altaxo.Main.Services
{
  public class RtfComposerService
  {
    /*
    static readonly string textheader =
  @"{\rtf1\ansi\ansicpg1252\deff0\deflang1033{\fonttbl{\f0\fswiss\fcharset0 Arial;}{\f1\froman\fprq2\fcharset0 Times New Roman;}{\f2\froman\fprq2\fcharset2 Symbol;}}" +
  @"\viewkind4\uc1\pard\f0 ";
    */

    static readonly string textheader =
  @"{\rtf1\ansi\ansicpg1252\deff0\deflang1033{\fonttbl{\f0\froman\fprq2\fcharset0 Times New Roman;}{\f1\fswiss\fcharset0 Arial;}{\f2\froman\fprq2\fcharset2 Symbol;}}" +
  @"\viewkind4\uc1\pard\f0 ";

    static readonly string texttrailer = @"}";
    static readonly string imageheader = @"{\pict\wmetafile8 ";
    static readonly string imagetrailer = "}";

    
    public static string GetRtfText(string rawtext, Graphics gr, Color backcolor, int fontsize)
    {
      MathML.Rendering.WinGraphicsRenderer _mmlRendering = new MathML.Rendering.WinGraphicsRenderer();
      _mmlRendering.BackColor = backcolor;
      _mmlRendering.FontSize = fontsize;
      StringBuilder stb = new StringBuilder();
      ComposeText(stb, rawtext, _mmlRendering, gr);
      stb.Append(texttrailer);
      return stb.ToString();
    }

    static void ComposeText(StringBuilder stb, string rawtext, MathML.Rendering.WinGraphicsRenderer _mmlRendering, Graphics gr)
    {
     

      if (stb.Length == 0)
        stb.Append(textheader);

      int currpos = 0;
      for (; ; )
      {
        int startidx = rawtext.IndexOf("<math>", currpos);
        if (startidx < 0)
          break;
        int endidx = rawtext.IndexOf("</math>", startidx);
        if (endidx < 0)
          break;
        endidx += "</math>".Length;

        // all text from currpos to startidx-1 can be copyied to the stringbuilder
        stb.Append(rawtext, currpos, startidx - currpos);

        // all text from startidx to endidx-1 must be loaded into the control and rendered
        System.IO.StringReader rd = new StringReader(rawtext.Substring(startidx, endidx - startidx));
        MathML.MathMLDocument doc = new MathML.MathMLDocument();
        doc.Load(rd);
        rd.Close();
        _mmlRendering.SetMathElement((MathML.MathMLMathElement)doc.DocumentElement);

        System.Drawing.Image mf = _mmlRendering.GetImage(typeof(Bitmap),gr);
        GraphicsUnit unit = GraphicsUnit.Point;
        RectangleF rect = mf.GetBounds(ref unit);
        string imagetext = GetRtfImage(mf);
        stb.Append(imageheader);
        stb.Append(@"\picwgoal" + Math.Ceiling(15 * rect.Width).ToString());
        stb.Append(@"\pichgoal" + Math.Ceiling(15 * rect.Height).ToString());
        stb.Append(" ");
        stb.Append(imagetext);
        stb.Append(imagetrailer);

        currpos = endidx;
      }

      stb.Append(rawtext, currpos, rawtext.Length - currpos);
    }


    #region Rich Text File Export






    // Specifies the flags/options for the unmanaged call to the GDI+ method
    // Metafile.EmfToWmfBits().
    private enum EmfToWmfBitsFlags
    {

      // Use the default conversion
      EmfToWmfBitsFlagsDefault = 0x00000000,

      // Embedded the source of the EMF metafiel within the resulting WMF
      // metafile
      EmfToWmfBitsFlagsEmbedEmf = 0x00000001,

      // Place a 22-byte header in the resulting WMF file.  The header is
      // required for the metafile to be considered placeable.
      EmfToWmfBitsFlagsIncludePlaceable = 0x00000002,

      // Don't simulate clipping by using the XOR operator.
      EmfToWmfBitsFlagsNoXORClip = 0x00000004
    };

    /// <summary>
    /// Use the EmfToWmfBits function in the GDI+ specification to convert a 
    /// Enhanced Metafile to a Windows Metafile
    /// </summary>
    /// <param name="hEmf">
    /// A handle to the Enhanced Metafile to be converted
    /// </param>
    /// <param name="bufferSize">
    /// The size of the buffer used to store the Windows Metafile bits returned
    /// </param>
    /// <param name="buffer">
    /// An array of bytes used to hold the Windows Metafile bits returned
    /// </param>
    /// <param name="mappingMode">
    /// The mapping mode of the image.  This control uses MM_ANISOTROPIC.
    /// </param>
    /// <param name="flags">
    /// Flags used to specify the format of the Windows Metafile returned
    /// </param>
    [System.Runtime.InteropServices.DllImportAttribute("gdiplus.dll")]
    private static extern uint GdipEmfToWmfBits(IntPtr hEmf, uint bufferSize,
      byte[] buffer, int mappingMode, EmfToWmfBitsFlags flags);


    // Allows the x-coordinates and y-coordinates of the metafile to be adjusted
    // independently
    private const int MM_ANISOTROPIC = 8;

    /// <summary>
    /// Wraps the image in an Enhanced Metafile by drawing the image onto the
    /// graphics context, then converts the Enhanced Metafile to a Windows
    /// Metafile, and finally appends the bits of the Windows Metafile in HEX
    /// to a string and returns the string.
    /// </summary>
    /// <param name="image"></param>
    /// <returns>
    /// A string containing the bits of a Windows Metafile in HEX
    /// </returns>
    public static string GetRtfImage(Image image)
    {

      StringBuilder rtf = null;

      // Used to store the enhanced metafile
      MemoryStream stream = null;

      // The enhanced metafile
      Metafile metaFile = null;

      // Handle to the device context used to create the metafile
      IntPtr hdc;

      try
      {
        rtf = new StringBuilder();
        stream = new MemoryStream();

        using (Graphics gr = Graphics.FromImage(image))
        {
          // Get the device context from the graphics context
          hdc = gr.GetHdc();
          // Create a new Enhanced Metafile from the device context
          metaFile = new Metafile(stream, hdc);
          // Release the device context
          gr.ReleaseHdc(hdc);
        }

        // Get a graphics context from the Enhanced Metafile
        using (Graphics gr = Graphics.FromImage(metaFile))
        {
          // Draw the image on the Enhanced Metafile
          gr.DrawImage(image, new Rectangle(0, 0, image.Width, image.Height));

        }

        // Get the handle of the Enhanced Metafile
        IntPtr hEmf = metaFile.GetHenhmetafile();

        // A call to EmfToWmfBits with a null buffer return the size of the
        // buffer need to store the WMF bits.  Use this to get the buffer
        // size.
        uint bufferSize = GdipEmfToWmfBits(hEmf, 0, null, MM_ANISOTROPIC,
          EmfToWmfBitsFlags.EmfToWmfBitsFlagsDefault);

        // Create an array to hold the bits
        byte[] buffer = new byte[bufferSize];

        // A call to EmfToWmfBits with a valid buffer copies the bits into the
        // buffer an returns the number of bits in the WMF.  
        uint convertedSize = GdipEmfToWmfBits(hEmf, bufferSize, buffer, MM_ANISOTROPIC,
          EmfToWmfBitsFlags.EmfToWmfBitsFlagsDefault);

        // Append the bits to the RTF string
        for (int i = 0; i < buffer.Length; ++i)
        {
          rtf.Append(String.Format("{0:X2}", buffer[i]));
        }

        return rtf.ToString();
      }
      finally
      {
        if (metaFile != null)
          metaFile.Dispose();
        if (stream != null)
          stream.Close();
      }
    }
    #endregion

  }
}
