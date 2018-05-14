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
	/// This class converts LaTeX formulas to an image (as a framework independent stream).
	/// </summary>
	public class LaTeXFormulaImageStreamProvider : Altaxo.Main.Services.ILaTeXFormulaImageStreamProvider
	{
		private static TexFormulaParser formulaParser = new TexFormulaParser();
		private static Pen pen = new Pen(Brushes.Black, 1);

		public (Stream bitmapStream, string placement) Parse(string text, double fontSize, double dpiResolution)
		{
			TexFormula formula = null;
			try
			{
				formula = formulaParser.Parse(text);
			}
			catch (Exception)
			{
				return (null, null);
			}

			double cyAscent;
			double cyDescent;
			double cyMiddle;
			{
				var bmpTemp = new System.Drawing.Bitmap(4, 4);
				var graphics = System.Drawing.Graphics.FromImage(bmpTemp);
				var gdiFont = new System.Drawing.Font("Segoe UI", (float)fontSize, System.Drawing.FontStyle.Regular);
				var cyLineSpace = gdiFont.GetHeight(graphics); // space between two lines
				int iCellSpace = gdiFont.FontFamily.GetLineSpacing(gdiFont.Style);
				int iEmHeight = gdiFont.FontFamily.GetEmHeight(gdiFont.Style);
				int iCellAscent = gdiFont.FontFamily.GetCellAscent(gdiFont.Style);
				int iCellDescent = gdiFont.FontFamily.GetCellDescent(gdiFont.Style);
				cyAscent = fontSize * iCellAscent / (double)iEmHeight;
				cyDescent = fontSize * iCellDescent / (double)iEmHeight;
				cyMiddle = cyAscent / 2;
				gdiFont.Dispose();
				graphics.Dispose();
				bmpTemp.Dispose();
			}

			var formulaRenderer = formula.GetRenderer(TexStyle.Display, fontSize, "Arial");
			// the placement of the image depends on the depth value
			var absoluteDepth = formulaRenderer.RenderSize.Height * formulaRenderer.RelativeDepth;
			var absoluteAscent = formulaRenderer.RenderSize.Height * (1 - formulaRenderer.RelativeDepth);

			double oversize = 0;
			double yoffset = 0;
			var sort = new SortedDictionary<double, (double, string)>();

			if (formulaRenderer.RelativeDepth < (1 / 16.0))
			{
				sort.Add(0, (0.0, "baseline"));
			}
			if (absoluteAscent <= cyAscent) // then we can use texttop alignment
			{
				oversize = cyAscent - absoluteAscent;
				yoffset = -oversize;
				sort.Add(Math.Abs(yoffset), (yoffset, "texttop"));
			}

			if (absoluteDepth <= cyDescent) // then we can use bottom alignment
			{
				oversize = cyDescent - absoluteDepth;
				yoffset = oversize;
				sort.Add(Math.Abs(yoffset), (yoffset, "bottom"));
			}

			{
				// Alignment: middle
				// Note that this is a moving target: we must change the vertical size of the image, but by that
				// we change the middle of the image, which changes again the offset...

				oversize = 2 * cyMiddle + absoluteDepth - absoluteAscent; // if oversize is negative, then pad at the bottom, else pad at the top

				sort.Add(Math.Abs(oversize), (-oversize, "middle"));
			}

			var firstEntry = sort.First();

			var bmp = formulaRenderer.RenderToBitmap(0, firstEntry.Value.Item1);

			var fileStream = new MemoryStream();
			BitmapEncoder encoder = new PngBitmapEncoder();
			encoder.Frames.Add(BitmapFrame.Create(bmp));
			encoder.Save(fileStream);
			fileStream.Seek(0, SeekOrigin.Begin);

			return (fileStream, firstEntry.Value.Item2);
		}
	}
}
