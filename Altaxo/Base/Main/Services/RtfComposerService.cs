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
    static readonly string textheader =
  @"{\rtf1\ansi\ansicpg1252\deff0\deflang1033{\fonttbl{\f0\fswiss\fcharset0 Arial;}{\f1\froman\fprq2\fcharset0 Times New Roman;}{\f2\froman\fprq2\fcharset2 Symbol;}}" +
  @"\viewkind4\uc1\pard\f0 ";

    static readonly string texttrailer = @"}";
    static readonly string imageheader = @"{\pict\wmetafile8 ";
    static readonly string imagetrailer = "}";

    
    public static string GetRtfText(string rawtext, Graphics gr, Color backcolor, int fontsize)
    {
      MathML.Rendering.GraphicsRendering _mmlRendering = new MathML.Rendering.GraphicsRendering();
      _mmlRendering.BackColor = backcolor;
      _mmlRendering.FontSize = fontsize;
      StringBuilder stb = new StringBuilder();
      ComposeText(stb, rawtext, _mmlRendering, gr);
      stb.Append(texttrailer);
      return stb.ToString();
    }

    static void ComposeText(StringBuilder stb, string rawtext, MathML.Rendering.GraphicsRendering _mmlRendering, Graphics gr)
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
        _mmlRendering.MathElement = (MathML.MathMLMathElement)doc.DocumentElement;

        System.Drawing.Image mf = _mmlRendering.GetImage(typeof(Bitmap),gr);
        GraphicsUnit unit = GraphicsUnit.Point;
        RectangleF rect = mf.GetBounds(ref unit);
        string imagetext = _mmlRendering.GetRtfImage(mf);
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


  }
}
