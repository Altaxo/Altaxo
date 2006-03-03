//This file is part of MathML.Rendering, a library for displaying mathml
//Copyright (C) 2003, Andy Somogyi
//
//This library is free software; you can redistribute it and/or
//modify it under the terms of the GNU Lesser General Public
//License as published by the Free Software Foundation; either
//version 2.1 of the License, or (at your option) any later version.
//
//This library is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//Lesser General Public License for more details.
//
//You should have received a copy of the GNU Lesser General Public
//License along with this library; if not, write to the Free Software
//Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//
//For details, see http://numerator.sourceforge.net, or send mail to
//(slightly obfuscated for spam mail harvesters)
//andy[at]epsilon3[dot]net

using System;

namespace MathML.Rendering
{
	/// <summary>
	/// information describing the 'region' surrounding an area.
	/// This included the offset of an area to the starting postion, 
	/// and the inner most source mathml element
	/// </summary>
	public class AreaRegion
	{
		// area this region is describing
		public readonly Area Area;

		// offset to source origin
		public readonly float X;

		// offset to source origin
		public readonly float Y;

		// containing MathML Element
		public MathMLElement Element = null;

		// current character index input location
		public int CharIndex = 0;

		public AreaRegion(Area area, float x, float y)
		{
			this.Area = area;
			this.X = x;
			this.Y = y;
		}

		public AreaRegion(Area area, MathMLElement element, float x, float y)
		{
			this.Area = area;
			this.Element = element;
			this.X = x;
			this.Y = y;
		}
	}
}
