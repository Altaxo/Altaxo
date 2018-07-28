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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WpfMath;

namespace Altaxo.Gui.Markdown
{
  /// <summary>
  /// This class converts LaTeX formulas to an image (as a framework independent png stream).
  /// </summary>
  public class LaTeXFormulaImageStreamProvider : Altaxo.Main.Services.ILaTeXFormulaImageStreamProvider
  {
    private static TexFormulaParser _formulaParser = new TexFormulaParser();

    /// <inheritdoc/>
    public (Stream bitmapStream, string placement, int width96thInch, int height96thInch) Parse(string text, string fontFamily, double fontSize, double dpiResolution, bool isIntendedForHelp1File)
    {
      TexFormula formula = null;
      try
      {
        formula = _formulaParser.Parse(text);
      }
      catch (Exception)
      {
        return (null, null, 0, 0);
      }

      double cyAscent;
      double cyDescent;
      double cyMiddle;
      {
        var gdiFont = new System.Drawing.Font(fontFamily, (float)fontSize, System.Drawing.FontStyle.Regular);
        int iCellSpace = gdiFont.FontFamily.GetLineSpacing(gdiFont.Style);
        int iEmHeight = gdiFont.FontFamily.GetEmHeight(gdiFont.Style);
        int iCellAscent = gdiFont.FontFamily.GetCellAscent(gdiFont.Style);
        int iCellDescent = gdiFont.FontFamily.GetCellDescent(gdiFont.Style);
        cyAscent = fontSize * iCellAscent / (double)iEmHeight;
        cyDescent = fontSize * iCellDescent / (double)iEmHeight;
        cyMiddle = cyAscent / 3; // is only a first guess, details coming from the Wpf font
        gdiFont.Dispose();
        var ff = new FontFamily(fontFamily);
        var tf = new Typeface(ff, System.Windows.FontStyles.Normal, System.Windows.FontWeights.Normal, System.Windows.FontStretches.Normal);
        if (tf.TryGetGlyphTypeface(out var gtf))
        {
          cyMiddle = Math.Floor(fontSize * gtf.Height * gtf.StrikethroughPosition);
        }
      }

      var formulaRenderer = formula.GetRenderer(TexStyle.Display, fontSize, fontFamily);
      // the placement of the image depends on the depth value
      var absoluteDepth = formulaRenderer.RenderSize.Height * formulaRenderer.RelativeDepth;
      var absoluteAscent = formulaRenderer.RenderSize.Height * (1 - formulaRenderer.RelativeDepth);

      double yoffset = 0;
      var sort = new SortedDictionary<double, (double, string)>();

      if (formulaRenderer.RelativeDepth < (1 / 16.0))
      {
        sort.Add(0, (0.0, "baseline"));
      }
      if (absoluteAscent <= cyAscent) // then we can use texttop alignment, and we shift the formula downwards (positive offset)
      {
        yoffset = cyAscent - absoluteAscent;
        sort.Add(Math.Abs(yoffset), (yoffset, "texttop"));
      }

      if (absoluteDepth <= cyDescent) // then we can use bottom alignment, and we shift the formula upwards (negative offset)
      {
        yoffset = absoluteDepth - cyDescent;
        sort.Add(Math.Abs(yoffset), (yoffset, "bottom"));
      }

      {
        // Alignment: middle
        // Note that this is a moving target: we must change the vertical size of the image, but by that
        // we change the middle of the image, which changes again the offset...
        if (isIntendedForHelp1File)
          yoffset = absoluteDepth - absoluteAscent; // in help1 file, the baseline of text is aligned with the middle of the image
        else
          yoffset = 2 * cyMiddle + absoluteDepth - absoluteAscent; // if yoffset is negative, then pad at the bottom, else pad at the top

        sort.Add(Math.Abs(yoffset), (yoffset, "middle"));
      }

      var firstEntry = sort.First();

      var (bmp, width96thInch, height96thInch) = RenderToBitmap(formulaRenderer, 0, firstEntry.Value.Item1, dpiResolution);

      var fileStream = new MemoryStream();
      BitmapEncoder encoder = new PngBitmapEncoder();
      encoder.Frames.Add(BitmapFrame.Create(bmp));
      encoder.Save(fileStream);
      fileStream.Seek(0, SeekOrigin.Begin);

      return (fileStream, firstEntry.Value.Item2, width96thInch, height96thInch);
    }

    /// <summary>
    /// Renders a formula to a bitmap.
    /// </summary>
    /// <param name="formulaRenderer">The formula renderer.</param>
    /// <param name="x">The x offset of the formula.</param>
    /// <param name="y">The y offset.
    /// If y is negative, the absolute value as number of pixels is padded at the bottom of the image,
    /// so that, if measured from the bottom of the image, the formula shifts upwards.
    /// If y is positve, the value as number of pixels is padded at the top of the image,
    /// so that, if measured from the top of the image, the formula shifts downwards.
    /// </param>
    /// <param name="dpiResolution">The resolution of the image in dpi. If not sure, use 96 dpi.</param>
    /// <returns>The bitmap souce that represents the formula.</returns>
    public static (BitmapSource bitmapSource, int width96thInch, int heigth96thInch) RenderToBitmap(TexRenderer formulaRenderer, double x, double y, double dpiResolution)
    {
      var visual = new DrawingVisual();
      using (var drawingContext = visual.RenderOpen())
        formulaRenderer.Render(drawingContext, x, Math.Max(0, y)); // if y negative, then we don't change y, because we measure relative to the upper edge of the image. Only if y is positive, we translate the formula downwards.

      var width = (int)Math.Ceiling(formulaRenderer.RenderSize.Width);
      var height = (int)Math.Ceiling(formulaRenderer.RenderSize.Height);
      height += (int)Math.Abs(y);

      var relativeResolution = dpiResolution / 96.0;

      var bitmap = new RenderTargetBitmap((int)(relativeResolution * width), (int)(relativeResolution * height), (int)(relativeResolution * 96), (int)(relativeResolution * 96), PixelFormats.Default);
      bitmap.Render(visual);

      return (bitmap, width, height);
    }
  }
}
