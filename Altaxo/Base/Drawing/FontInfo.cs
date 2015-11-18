#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2015 Dr. Dirk Lellinger
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

namespace Altaxo.Drawing
{
	/// <summary>
	/// Holds Information about the metrics of a font.
	/// </summary>
	public class FontInfo
	{
		public double cyLineSpace { get; private set; } // cached linespace value of the font

		public double cyAscent { get; private set; }    // cached ascent value of the font

		public double cyDescent { get; private set; } /// cached descent value of the font

		public double Size { get; private set; }

		public FontInfo(double cylinespace, double cyascent, double cydescent, double size)
		{
			cyLineSpace = cylinespace;
			cyAscent = cyascent;
			cyDescent = cydescent;
			Size = size;
		}
	}
}