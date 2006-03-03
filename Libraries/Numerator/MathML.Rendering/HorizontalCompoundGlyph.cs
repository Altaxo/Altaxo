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
	/// specialization of a horizontal area. This class is treated as a single
	/// glyph
	/// </summary>
	internal class HorizontalCompoundGlyph : HorizontalArea
	{
		public HorizontalCompoundGlyph(Area[] content) : base(content)
		{
		}

		/**
		 * a compound glyph is just a fixed collection of glyphs, it can not
		 * be resized, so just return this.
		 */
		public override Area Fit(BoundingBox box)
		{
			return this;
		}

    public override AreaRegion GetEditRegion(IFormattingContext context, float x, float y, int index)
		{
			x = index > 0 ? x + LeftEdge + box.HorizontalExtent : x + LeftEdge;
			return new AreaRegion(this, x, y);
		}

		public override AreaRegion GetRegion(float x, float y, float pointX, float pointY)
		{
			return BoundingBox.Contains(x, y, pointX, pointY) ? new AreaRegion(this, x, y) : null;
		}
	}
}
