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

#nullable disable warnings
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WpfMath.Parsers;
using WpfMath.Rendering;
using XamlMath;
using XamlMath.Boxes;

namespace Altaxo.Gui.Markdown
{
  /// <summary>
  /// This class converts LaTeX formulas to an image (as a framework independent png stream).
  /// </summary>
  public class LaTeXFormulaImageStreamProvider : Altaxo.Main.Services.ILaTeXFormulaImageStreamProvider
  {
    private static TexFormulaParser _formulaParser = WpfTeXFormulaParser.Instance;


    private static TexEnvironment _texEnvironment = WpfTeXEnvironment.Create(TexStyle.Display);


    /// <inheritdoc/>
    public (Stream bitmapStream, string placement, int yoffset, int width96thInch, int height96thInch) Parse(string text, string fontFamily, double fontSize, double dpiResolution, bool isIntendedForHelp1File)
    {
      TexFormula formula = null;
      try
      {
        formula = _formulaParser.Parse(text);
      }
      catch (Exception)
      {
        return (null, null, 0, 0, 0);
      }

      double fontAscent;
      double fontDescent;
      double fontStrikethrough;
      {
        var ff = new System.Windows.Media.FontFamily(fontFamily);
        var tf = new Typeface(ff, System.Windows.FontStyles.Normal, System.Windows.FontWeights.Normal, System.Windows.FontStretches.Normal);
        if (tf.TryGetGlyphTypeface(out var gtf))
        {
          fontAscent = fontSize * gtf.Baseline;
          fontDescent = fontSize * (gtf.Height - gtf.Baseline);
          fontStrikethrough = fontSize * gtf.StrikethroughPosition;
        }
        else
        {
          using var gdiFont = new System.Drawing.Font(fontFamily, (float)fontSize, System.Drawing.FontStyle.Regular);
          int iCellSpace = gdiFont.FontFamily.GetLineSpacing(gdiFont.Style);
          int iEmHeight = gdiFont.FontFamily.GetEmHeight(gdiFont.Style);
          int iCellAscent = gdiFont.FontFamily.GetCellAscent(gdiFont.Style);
          int iCellDescent = gdiFont.FontFamily.GetCellDescent(gdiFont.Style);
          fontAscent = fontSize * iCellAscent / iEmHeight;
          fontDescent = fontSize * iCellDescent / iEmHeight;
          fontStrikethrough = fontSize * 0.258; // is only a first guess, details coming from the Wpf font
        }
      }

      Box box;
      {
        // get the bounding box of the formula
        var visual = new DrawingVisual();
        using (var drawingContext = visual.RenderOpen())
        {
          box = formula.RenderTo(drawingContext, _texEnvironment, fontSize);
        }
      }

      var relativeResolution = dpiResolution / 96.0;
      var imageWidthPixels = (int)Math.Ceiling(fontSize * box.Width * relativeResolution);
      var imageHeightPixels = (int)(Math.Ceiling(fontSize * box.Height * relativeResolution) + Math.Ceiling(fontSize * box.Depth * relativeResolution));
      // the placement of the image depends on the depth value
      var formulaDescentRelative = box.Depth / box.TotalHeight;
      var formulaDescent = box.Depth * fontSize; // TODO: is this in 72th inch because of fontSize or in 96th inch?
      var formulaAscent = box.Height * fontSize;

      double yoffset = 0;

      BitmapSource bmp;
      int width96thInch, height96thInch;
      string alignment;


      if (isIntendedForHelp1File)
      {
        // dictionary that holds the number vertical pixels of the image, and a tuple of the neccessary render offset and the alignment
        var sort = new List<(int imageHeight, double yOffset_96thInch, string alignment)>();

        /* 
        // 2024-08-20 The baseline alignment is not used anymore in Help1 for the following reason:
        // if the formula contains characters with a round shape at the bottom (e.g. '0', 'O' 'C'), there are some
        // pixels required for the character just below the baseline.
        // But if the image is baseline aligned, the first row of pixels
        // starts slightly above the baseline. Thus a correct display of those characters is not possible
        // a alternative would be to slighly shift the formula a little bit to the top, but then it is
        // not anymore baseline aligned

        if (formulaDescentRelative < (1 / 16.0)) // if the formulas baseline is almost at the bottom of the image
        {
          // if the descent is close to zero, then we can use the baseline alignment
          // for a Help1 target this means that the bottom of the formula image is aligned with the baseline of the surrounding font.

          height96thInch = (int)Math.Round(imageHeightPixels / relativeResolution);
          var yoffsetAccurate = formulaAscent - height96thInch; // number of pixels from image bottom to baseline (negative sign)
          if (yoffsetAccurate >= 0) // if the formula fits exactly, then we add one pixel more
          {
            imageHeightPixels += (int)Math.Ceiling(relativeResolution);
            height96thInch = (int)Math.Round(imageHeightPixels / relativeResolution);
            yoffsetAccurate = formulaAscent - height96thInch; // number of pixels from image bottom to baseline (negative sign)
          }
          sort.Add((imageHeightPixels, -yoffsetAccurate, "baseline")); // then we can use baseline as vertical alight
        }
        */


        if (fontAscent >= formulaAscent)
        {
          // Alignment: texttop
          // if our formula is not higher than the top of the text than we can use the texttop alignment
          // texttop alignment means that the top of the formula box is aligned with the top (cyAscent) of the surrounding font
          // we pad our formula box at the top with the difference between the font ascent and the formula ascent
          // and we shift the formula downwards (positive offset)
          yoffset = fontAscent - formulaAscent;
          int additionalPixelsTop = (int)Math.Ceiling(yoffset * relativeResolution);
          var renderOffset = yoffset;
          sort.Add((imageHeightPixels + additionalPixelsTop, renderOffset, "texttop"));
        }

        if (fontDescent >= formulaDescent)
        {
          // Alignment: bottom
          // if our formula is not deeper than the bottom of the text (font) then we can use bottom alignment
          // for a Help1 target bottom alignment means that the bottom of the formula image is aligned with the bottom (cyDescent) of the surrounding font
          // we pad our formula box at the bottom with the difference value
          // note that for the key we use the absolute value in order to later find the sort of padding which requires the
          // least number of pixels to add to the formula box
          yoffset = fontDescent - formulaDescent;

          // calculate the neccessary pixels to add to the bottom
          // by using Math.Ceiling, we shift the image a little bit to high
          int additionalPixelsBottom = (int)Math.Ceiling(yoffset * relativeResolution);
          // in order to compensate for this, we have to shift the formula a little bit down again
          var renderOffset = (imageHeightPixels + additionalPixelsBottom) / relativeResolution - fontDescent - formulaAscent;
          sort.Add((imageHeightPixels + additionalPixelsBottom, renderOffset, "bottom")); // note that we store the value as negative value to mark it as bottom padding
        }

        {
          // Alignment: middle
          // for a Help1 target this means that the vertical middle of the formula box is aligned with the strikethrough position of the surrounding font
          // Note that this is a moving target: we must change the vertical size of the image, but by that
          // we change the middle of the image, which changes again the offset...
          // a positive offset means we have to pad the formula at the top
          // whereas a negative offset means we have to pad the formula box at the bottom
          yoffset = formulaDescent - formulaAscent + 2 * fontStrikethrough;
          int additionalPixels = (int)Math.Ceiling(Math.Abs(yoffset) * relativeResolution);
          var imageHalfHeight = (imageHeightPixels + additionalPixels) / (2.0 * relativeResolution);
          var renderOffset = imageHalfHeight + fontStrikethrough - formulaAscent;
          sort.Add((imageHeightPixels + additionalPixels, renderOffset, "middle"));
        }

        // now find the entry which requires the least padding by sorting by key (which is the image height)
        // and then use the entry with the smallest image size
        sort.Sort((x, y) => Comparer<int>.Default.Compare(x.imageHeight, y.imageHeight));
        var firstEntry = sort[0];
        (bmp, width96thInch, height96thInch) = RenderToBitmap(
          formula,
          _texEnvironment,
          fontSize,
          bitmapWidthPixels: imageWidthPixels,
          bitmapHeightPixels: firstEntry.imageHeight,
          relativeResolution: relativeResolution,
          renderOffsetY: firstEntry.yOffset_96thInch);

        alignment = firstEntry.alignment;
        yoffset = 0; // 0 as return value
      }
      else // MAML is intended for HTML help (so we can use HTML5 alignment with pixel accuracy )
      {
        alignment = "baseline";
        height96thInch = (int)Math.Round(imageHeightPixels / relativeResolution);
        var yoffsetAccurate = formulaAscent - height96thInch; // number of pixels from image bottom to baseline (negative sign)
        yoffset = Math.Ceiling(yoffsetAccurate);

        if (yoffset != yoffsetAccurate)
        {
          imageHeightPixels += (int)Math.Ceiling(relativeResolution);
          height96thInch = (int)Math.Round(imageHeightPixels / relativeResolution);
          yoffsetAccurate = formulaAscent - height96thInch; // number of pixels from image bottom to baseline (negative sign)
          yoffset = Math.Ceiling(yoffsetAccurate);
        }


        (bmp, width96thInch, height96thInch) = RenderToBitmap(
          formula,
          _texEnvironment,
          fontSize,
          bitmapWidthPixels: imageWidthPixels,
          bitmapHeightPixels: imageHeightPixels,
          relativeResolution: relativeResolution,
          renderOffsetY: yoffset - yoffsetAccurate);
      }

      var fileStream = new MemoryStream();
      BitmapEncoder encoder = new PngBitmapEncoder();
      encoder.Frames.Add(BitmapFrame.Create(bmp));
      encoder.Save(fileStream);
      fileStream.Seek(0, SeekOrigin.Begin);

      return (fileStream, alignment, (int)Math.Round(yoffset), width96thInch, height96thInch);
    }

    /// <summary>
    /// Renders a formula to a bitmap.
    /// </summary>
    /// <param name="formula">The formula to render.</param>
    /// <param name="environment">The formula environment needed to render the formula.</param>
    /// <param name="fontSize">The current font size used for rendering of the formula.</param>
    /// <param name="bitmapWidthPixels">The width of the render bitmap in pixels.</param>
    /// <param name="bitmapHeightPixels">The height of the render bitmap in pixels.</param>
    /// <param name="relativeResolution">The relative resolution as defined as resolution in dpi divided by 96 dpi.</param>
    /// <param name="renderOffsetY">The render offset in 1/96th inch. Should be positive (not tested).
    /// Designates the distance between the top of the image and the top of the formula.
    /// <returns>The bitmap souce that represents the formula.</returns>
    public static (BitmapSource bitmapSource, int width96thInch, int heigth96thInch) RenderToBitmap(
      TexFormula formula,
      TexEnvironment environment,
      double fontSize,
      int bitmapWidthPixels,
      int bitmapHeightPixels,
      double relativeResolution,
      double renderOffsetY)
    {
      Box box;
      var visual = new DrawingVisual();
      using (var drawingContext = visual.RenderOpen())
      {
        box = formula.RenderTo(drawingContext, _texEnvironment, fontSize, 0, renderOffsetY / fontSize);
      }

      var bitmap = new RenderTargetBitmap(bitmapWidthPixels, bitmapHeightPixels, (int)(relativeResolution * 96), (int)(relativeResolution * 96), PixelFormats.Default);
      bitmap.Render(visual);
      return (bitmap, (int)(bitmapWidthPixels / relativeResolution), (int)(bitmapHeightPixels / relativeResolution));
    }

  }
}
