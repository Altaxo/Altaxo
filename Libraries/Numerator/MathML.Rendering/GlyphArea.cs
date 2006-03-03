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
using System.Collections;
using System.Drawing;
using MathML.Rendering.GlyphMapper;
using Scaled = System.Single;

namespace MathML.Rendering
{
	/**
	* represents a single symbol on the screen. A glyph is
	* different from a standard string, in that each glyph 
	* contains a specific font. A standard glyph area is fixed
	* and read only, it will not re-size to a Fit call.
	* a glyph node is a terminal node, and is imutable. 
	* as this node is imutable, we can pre-calculate the
	* size, and cache it, as it is very expensive to calculate
	* the glyph metrics.
	* 
	* 10-26-2003 
	* Major optimization, calculate glyph metrics only once in ctor, 
	*/
	internal class GlyphArea : Area, IComparable
	{
		/**
		 * the character index or value.
		 */
		private readonly ushort index;

		/**
		* a reference to a font that is used by the graphic device
		*/
		private readonly IFontHandle font;

		/**
		 * keep track of the size so we do not re-calculate them
		 */ 
		private readonly BoundingBox box;
		private readonly float leftEdge = 0;
		private readonly float rightEdge = 0;

		public GlyphArea(IFormattingContext context, IFontHandle font, ushort index)
		{
			this.font = font;
			this.index = index;
			context.MeasureGlyph(font, index, out box, out leftEdge, out rightEdge);
		}

		public GlyphArea(IFontHandle fontHandle, ref GlyphAttributes attributes)
		{
			this.font = fontHandle;
			this.leftEdge = attributes.Left;
			this.rightEdge = attributes.Right;
			this.box = attributes.Box;
			this.index = (ushort)attributes.Index;
		}

		/**
		 * render this area. 
		 */
		public override void Render(IGraphicDevice device, float x, float y) 
		{
			IFontHandle oldFont = device.SetFont(font);
      device.DrawGlyph(index, x, y);		
			device.RestoreFont(oldFont);
		}

		/**
		 * get the bounding box, this was set in the ctor
		 */
		public override BoundingBox BoundingBox 
		{
			get { return box;}
		}

		public override Scaled LeftEdge 
		{
			get { return leftEdge; }
		}

		public override Scaled RightEdge 
		{
			get { return rightEdge; }
		}

		/**
		 * get an area with an identifier that is a relative path
		 * to this node. This is a terminal node, so the identifier better 
		 * be at its' end.
		 */
		public override Area GetArea(AreaIdentifier id) 
		{
			if(id.End)
			{
				return this;
			}
			else
			{
				throw new InvalidIdentifier();
			}
		}

    public override AreaRegion GetEditRegion(IFormattingContext context, float x, float y, int index)
		{
			x = index > 0 ? x + leftEdge + box.HorizontalExtent : x + leftEdge;
			return new AreaRegion(this, x, y);
		}

		public int CompareTo(object obj)
		{
			ushort oIndex = ((GlyphArea)obj).index;
			return (index == oIndex ? 0 :(index < oIndex ? -1 : 1));
		}

		/**
		 * glyph areas are read only terminal nodes, so just return this object
		 */
		public override Object Clone()
		{
			return this;
		}
	}
}
