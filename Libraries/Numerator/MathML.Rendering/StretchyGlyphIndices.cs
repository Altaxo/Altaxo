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
using System.Xml;

namespace MathML.Rendering.GlyphMapper 
{
	/**
	 * a 'stretchy glyph' is composed of several parts.
	 * a font can have one or more glyphs of varying sizes for
	 * a particular character. These are the 'simple glyphs'. For 
	 * example, the cmex10 font has 4 different sizes of the ')' 
	 * character. a font can aslo have a set of glyphs that make 
	 * a 'compound glyph'. These are glyphs that are pieced
	 * together from serveral smaller glyphs, usually, the center 
	 * glyph is repeated to achive tall skinny stretched glyphs.
	 * 
	 * This struct hold the indeces in the font for the various
	 * glyphs that map to a character.
	 */
	internal struct StretchyGlyphIndices 
	{
		/**
		 * indices of the compound glyphs, and what they mean
		 */
		public const int Bottom = 0;
		public const int Filler = 1;
		public const int Middle = 2;
		public const int Top = 3;

		/**
		 * the character that these indices are for
		 */
		public readonly char Char;

		/**
		 * set of simple glyphs, of varying sizes, stored
		 * from smallest to largest.
		 */
		public readonly short[] SimpleIndices;

		/**
		 * indices for the compound glyphs.
		 * formated as such:
		 * index 0 is the bottom glyph, -1 if none exists.
		 * index 1 is the repeated glyph, this needs to be here.
		 * index 2 is the middle glyph if one exists, -1 otherwise.
		 * index 3 is the top glyph if one exists, -1 otherwise.
		 */
		public readonly short[] CompoundIndices;

		/**
		 * stretch vertially or horizontally
		 */
		public readonly StretchyOrientation Orientation;

		/**
		 * load this set from an xml element.
		 */
		public StretchyGlyphIndices(XmlElement node) 
		{
			XmlElement index = null;

			// make sure we have the right type of xml node
			if(node.Name != "stretchy") throw new Exception(
				String.Format("the node name of \"{0}\" is not valid for a StretchyGlyphIndices", node.Name));

			// get the orientation
			Orientation = node.GetAttribute("orientation") == "vertical" ? StretchyOrientation.Vertical :
				StretchyOrientation.Horizontal;

			// get the char value, required value
			Char = Utility.ParseChar(node.GetAttribute("char"));

			// simple indices are not required
			if((index = node.SelectSingleNode("simple") as XmlElement) != null) 
			{
				SimpleIndices = Utility.ParseShortArray(index.GetAttribute("index"));
			}
			else 
			{
				SimpleIndices = null;
			}

			// compound indices are not required
			if((index = node.SelectSingleNode("compound") as XmlElement) != null) 
			{
				CompoundIndices = Utility.ParseShortArray(index.GetAttribute("index"));
				if(CompoundIndices.Length != 4)
				{
					throw new Exception(String.Format(
						"Error, a compound index array was found with a length of {0}, it must have a length of 4", 
						CompoundIndices.Length));
				}
			}
			else 
			{
				CompoundIndices = null;
			}
		}		
	}
}
