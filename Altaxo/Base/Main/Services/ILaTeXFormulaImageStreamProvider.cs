#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2018 Dr. Dirk Lellinger
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Main.Services
{
  /// <summary>
  /// Interface to provide a LaTeX formula as an image. The image is provided as a stream, representing a .PNG image.
  /// </summary>
  public interface ILaTeXFormulaImageStreamProvider
  {
    /// <summary>
    /// Parses the specified LaTeX formula text.
    /// </summary>
    /// <param name="formulaText">The formula text.</param>
    /// <param name="fontFamily">The font family of the font in with the formula is embedded.</param>
    /// <param name="fontSize">Size of the font in which the formula text is embedded.</param>
    /// <param name="dpiResolution">The resolution of the required image.</param>
    /// <param name="isIntendedForHelp1File">Set this argument to true if the image is indended to be used in a Help1 file. In such a file, the placement of images with align="middle" differs from HTML rendering (the text baseline is aligned with the middle of the image, whereas in HTML the middle of the text is aligned with the middle of the image).</param>
    /// <returns>A tuple consisting of
    /// 1) the bitmap stream,
    /// 2) the html attribute how to place the image,
    /// 3) the y-offset in pixels (1/96th inch) the bitmap must be shifted relative to the baseline of the text (for Web rendering with html5),
    /// 4) the width of the image in pixels (1/96th inch), and
    /// 5) the height of the bitmap in pixels (1/96th inch).</returns>
    (System.IO.Stream bitmapStream, string placement, int yoffset, int width96thInch, int height96thInch) Parse(string formulaText, string fontFamily, double fontSize, double dpiResolution, bool isIntendedForHelp1File);
  }
}
