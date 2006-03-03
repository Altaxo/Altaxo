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
using System.Drawing;
using Scaled = System.Single;

namespace MathML.Rendering
{
	/**
	 * A string area would behave like a horizontal array of glyps
	 * This is an optimization of an array of glyphs, as a string is 
	 * always rendered with the same font, color, and other text attributes.
	 * This saves having to switch to font out for each character.
	 * A string area is an imutable terminal node, no change in the
	 * rendering device will afect the size (although a change in the
	 * rendering device can change the final color, but never the
	 * size), this is why a string area keeps a reference to its' own
	 * font. Selecting and de-selecting in the rendering device is a 
	 * fairly cheap operation, it because this node is imutable, it
	 * allows some very good performance optimizations, like caching
	 * the bounding box of the string, since calculating the string 
	 * bounding box is VERY expensive. 
	 * 
	 * TODO handle left and right, but these may be ignored in the future.
	 */
	internal class StringArea : SimpleArea
	{
		/** 
		 * the actual string that will be rendered
		 */
		private readonly string content;

		/**
		 * keep track of the font
		 */
		private readonly IFontHandle font;

		/**
		 * keep track of the bouning box, calculated in the ctor
		 */
		private readonly BoundingBox box;

		/**
		 * the left and right edges
		 */
		private readonly float leftEdge = 0;
		private readonly float rightEdge = 0;

		/**
		 * create a string area
		 */
		public StringArea(IFormattingContext context, string content)
		{
			this.font = context.GetFont();
			this.content = content;
			box = BoundingBox.New();

			float left = 0, right = 0;

			for(int i = 0; i < content.Length; i++)
			{
				BoundingBox tmp = BoundingBox.New();
				context.MeasureGlyph(font, content[i], out tmp, out left, out right);
				box.Append(tmp);

				// left edge of first char
				if(i == 0)
				{
					leftEdge = left;
				}
			}

			// right edge of last char
			rightEdge = right;
		}

		public override void Render(IGraphicDevice device, float x, float y)
		{
			IFontHandle savedFont = device.SetFont(font);
			device.DrawString(x, y, content);
			device.RestoreFont(savedFont);
		}

		public override BoundingBox BoundingBox
		{
			get { return box; }
		}

		public override Scaled LeftEdge 
		{
			get { return leftEdge; }
		}

		public override Scaled RightEdge 
		{
			get { return rightEdge; }
		}

    public override AreaRegion GetEditRegion(IFormattingContext context, float x, float y, int index)
		{
			// measure horizontal position of char:
			BoundingBox horz = BoundingBox.New();
			float left, right;
			for(int i = 0; i < index && i < content.Length; i++)
			{
				BoundingBox tmp = BoundingBox.New();
				context.MeasureGlyph(font, content[i], out tmp, out left, out right);
				horz.Append(tmp);
			}
			return new AreaRegion(this, x + horz.Width, y);
		}


    public override AreaRegion GetRegion(IFormattingContext context, float x, float y, Area area, int index)
		{
			if(area == this && index <= content.Length)
			{
				// measure horizontal position of char:
				BoundingBox horz = BoundingBox.New();
				float left, right;
				for(int i = 0; i < index; i++)
				{
					BoundingBox tmp = BoundingBox.New();
					context.MeasureGlyph(font, content[i], out tmp, out left, out right);
					horz.Append(tmp);
				}
                return new AreaRegion(this, x + horz.Width, y);
			}
			return null;
		}

		/**
		 * String areas ares are read only, so just return this object
		 */
		public override Object Clone()
		{
			return this;
		}
	}
}
