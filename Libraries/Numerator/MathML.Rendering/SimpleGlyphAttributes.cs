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

namespace MathML.Rendering.GlyphMapper
{
	/**
	 * a mapping of a character to a glyph index in a font.
	 * 
	 * A fast was is needed to retrieve cached fonts. The
	 * 'correct' object oriented way would be to store a list
	 * of created glyph areas in the FontInstance. However, there
	 * will be far more glyphs than font instances created, so it
	 * is more effcient to store a list of glyphs of varying sizes 
	 * with this struct rather than a list of glyphs in each font 
	 * instance. The reason being, is that once this structure 
	 * is found (there will be a lot of them), we know the mapped
	 * glyph index, so we just search through usually no more than
	 * 5 glyph instances, each with the same glyph index, just created
	 * with different font sizes, rather than searching through 
	 * many glyphs if they would be stored in the FontInstance.
	 */
	internal struct SimpleGlyphAttributes
	{
		/** 
		 * the character that this is a match for.
		 */
		public readonly char Char;

		/**
		 * the index in the font of this character's glyph
		 */
		public readonly ushort GlyphIndex;

		/**
		 * initially created with a null glyphs list because
		 * the majority of glyphs will never be used.
		 */
		public SimpleGlyphAttributes(char c, ushort i)
		{
			Char = c;
			GlyphIndex = i;
		}

		/**
		 * get a glyph area for this glyph.
		 * this searches through the glyphs list to see
		 * if an area has allready been created for the mathing
		 * set of glyph index and font, if none exists, a new 
		 * glyph area is created and cached via a weak reference.
		 */
    public Area GetArea(IFormattingContext context, IFontHandle fontHandle)
		{
			return new GlyphArea(context, fontHandle, GlyphIndex);
		}
	}
}
