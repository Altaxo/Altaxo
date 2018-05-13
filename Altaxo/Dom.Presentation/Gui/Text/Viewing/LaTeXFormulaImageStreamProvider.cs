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

			var formulaRenderer = formula.GetRenderer(TexStyle.Display, fontSize, "Arial");
			// the placement of the image depends on the depth value
			var absoluteDepth = formulaRenderer.RenderSize.Height * formulaRenderer.RelativeDepth;

			var bmp = formulaRenderer.RenderToBitmap(0, 0);

			var fileStream = new MemoryStream();
			BitmapEncoder encoder = new PngBitmapEncoder();
			encoder.Frames.Add(BitmapFrame.Create(bmp));
			encoder.Save(fileStream);
			fileStream.Seek(0, SeekOrigin.Begin);

			return (fileStream, "baseline");
		}
	}
}
