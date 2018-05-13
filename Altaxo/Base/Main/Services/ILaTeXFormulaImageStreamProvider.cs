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
		/// <param name="fontSize">Size of the font in which the formula text is embedded.</param>
		/// <param name="dpiResolution">The resolution of the required image.</param>
		/// <returns>A tuple consisting of the bitmap stream, and the html attribute how to place the image.</returns>
		(System.IO.Stream bitmapStream, string placement) Parse(string formulaText, double fontSize, double dpiResolution);
	}
}
