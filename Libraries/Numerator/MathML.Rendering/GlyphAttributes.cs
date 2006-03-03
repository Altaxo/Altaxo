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

/**
 * store a set of glyph attributes, these are calculated
 * by the GetGlyphMetrics method of the GraphicDevice.
 * attributes are typically stored in the GlyphMap, and used
 * for finding appropriate stretchy glyphs.
 * 
 * A GlyphArea needs all the data in a GlyphAttribute, 
 * except the glyph reference, so the GlyphArea just has
 * a copy of the relative fields.
 * 
 * note, a realized GlyphArea is much larger than a simple
 * GlyphAttributes struct. A GlyphArea has 12 bytes for the class, plus
 * 13 virtuals * 4 bytes = 52 bytes + 22 bytes for attribute data =
 * 74 bytes, A GlyphAttributes is only 
 * Box = 12 bytes + 
 * 2 * float = 8 bytes +
 * Index = 2 bytes +
 * 4 bytes = WeakReference =
 * 26 bytes. 
 * 
 * So we do not create a GlyphArea right away, just keep a weak 
 * reference to it, and create it on a need be basis. 
 */
namespace MathML.Rendering.GlyphMapper 
{
	internal struct GlyphAttributes 
	{
		/**
		 * fudge the reported glyph metrics so compound stretchy glyphs
		 * fit together nicely.
		 */
		public const int FudgeNone = 0;
		public const int FudgeWidth = 1;
		public const int FudgeHeight = 2;

		/**
		 * standard set of attributes (metrics) for a glyph
		 */
		public readonly BoundingBox Box;
		public readonly float Left;
		public readonly float Right;
		public readonly short Index;

		private const float fudgeFactor = 1.0f;

		/**
		 * set the attributes, but do not actually create a glyph 
		 * area.
		 * @param fontHandle a handle to a native font resource
		 * @param charIndex the index in the font of the desired glyph
		 * @param stretchyType if this glyph is a repeated section in a
		 *		  stretchy glyph, shrink the reported metrics by one, so
		 *		  we get a nice overlap.
		 */
    public GlyphAttributes(IFormattingContext context, IFontHandle fontHandle, short charIndex, int fudge)
		{
			Index = charIndex;
			glyphArea = null;
			float tmp;

			if(charIndex >= 0)
			{
				context.MeasureGlyph(fontHandle, (ushort)charIndex, out Box, 
					out Left, out Right);

				if(fudge == FudgeWidth)
				{
					Box.Width -= 2 * fudgeFactor;
				}
				else if(fudge == FudgeHeight)
				{
					if((tmp = Box.Height - fudgeFactor) >= 0.0f) Box.Height = tmp;
					if((tmp = Box.Depth - fudgeFactor) >= 0.0f) Box.Depth = tmp;
				}
			}
			else
			{
				Box = new BoundingBox();
				Left = 0;
				Right = 0;
			}
		}

		/**
		 * a weak refernece to a realized glyph area. This allows
		 * realized glyph areas to be shared, and allows the garbage
		 * collector to properly manage glyph area lifetimes.
		 * 
		 * this is set by the container of this class.
		 */
		private WeakReference glyphArea;

		public Area GetGlyph(IFontHandle fontHandle)
		{
			Area result = null;
			if(glyphArea != null && glyphArea.IsAlive)
			{
				result = (Area)glyphArea.Target;
			}
			else
			{
				result = new GlyphArea(fontHandle, ref this);
				glyphArea = new WeakReference(result);
			}
			return result;
		}
	}
}
