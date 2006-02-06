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

namespace MathML.Rendering
{
	/// <summary>
	/// Summary description for FontFactory.
	/// </summary>
	internal class FontFactory
	{
		/**
		 * keep track of all the fonts that this class
		 * created. 
		 * 
		 * This is a list of weak references, in that the font is freed when
		 * all references except this one are gone. Font or glyph areas hold
		 * strong references to fonts.
		 */
		private static ArrayList fonts = new ArrayList();

		public FontFactory()
		{
		}

		/**
		 * create a font based only on the MathVariant and MathSize 
		 * fields of the FormattingContext
		 * Currently, this only a shortcut to GetFont(context, "", ""), but
		 * internally may be optimized in the future.
		 */
		public static FontHandle GetFont(FormattingContext context)
		{
			return GetFont(context, "", "");
		}

		/**
		 * get a font based on the font type given in the variant, 
		 * and the size given in height. If the size can not be 
		 * evaluated to a valid size, the default font size is used.
		 * the fontName override anything in the context if it is given.
		 */
		public static FontHandle GetFont(FormattingContext context, string fontName, string altFontName)
		{
			// font to be returned
			FontHandle font = null;

			// find the attributes from the static array
			FontAttributes attr = fontAttributes[(int)context.MathVariant];

			int height = context.Size;

			// no italic if we are using the font name, otherwise use the 
			// value from the mathvariant attribute
			bool italic = fontName.Length > 0 ? false : attr.Italic;

			// use default value if we are using the font specified by a name, 
			// otherwise use the one from the attributes from mathvariant
			int weight = fontName.Length > 0 ? DefaultFontWeight : attr.Weight;

			// use either the fontName stirng if we have one, or the one
			// from the mathvariant attribute
			string name = fontName.Length > 0 ? fontName : attr.Name;
			
			font = FindFont(height, italic, weight, name);

			if(font == null)
			{
				font = GraphicDevice.CreateFont(height, italic, weight, name);
				fonts.Add(new WeakReference(font));
			}
			return font;
		}

		/**
		 * try to find a font in the list that matches the contents
		 * of the formating context
		 */
		private static FontHandle FindFont(int height, bool italic, int weight, string fontName)
		{
			for(int i = 0; i < fonts.Count; i++)
			{
				WeakReference w = (WeakReference)fonts[i];
				if(w.IsAlive)
				{
					FontHandle f = (FontHandle)w.Target;
					if(f.Equals(height, italic, weight, fontName)) return f;					
				}
				else
				{
					// reference is dead, so get rid of it
					fonts.RemoveAt(i--);
				}
			}
			// no font was found
			return null;
		}

		private struct FontAttributes
		{
			public FontAttributes(String name, int weight, bool italic)
			{
				Name = name; Weight = weight; Italic = italic; 
			}
			public String Name;
			public int Weight;
			public bool Italic;
		}

		private static FontAttributes[] fontAttributes = 
		{
			new FontAttributes("Times New Roman", Win32.FW_NORMAL,	false), // Normal
			new FontAttributes("Times New Roman", Win32.FW_BOLD,	false), // Bold
			new FontAttributes("Times New Roman", Win32.FW_NORMAL,	true), // Italic
			new FontAttributes("Times New Roman", Win32.FW_BOLD,	false), // BoldItalic
			new FontAttributes("Times New Roman", Win32.FW_NORMAL,	false), // DoubleStruck
			new FontAttributes("Times New Roman", Win32.FW_BOLD,	false), // BoldFraktur
			new FontAttributes("Times New Roman", Win32.FW_NORMAL,	false), // Script
			new FontAttributes("Times New Roman", Win32.FW_BOLD,	false), // BoldScript
			new FontAttributes("Times New Roman", Win32.FW_NORMAL,	false), // Fracktur
			new FontAttributes("Arial", Win32.FW_NORMAL,	false), // SansSerif
			new FontAttributes("Arial", Win32.FW_BOLD,	false), // BoldSansSerif
			new FontAttributes("Arial", Win32.FW_NORMAL,	true), // SansSerifItalic
			new FontAttributes("Arial", Win32.FW_BOLD,	true), // SansSerifBoldItalic
			new FontAttributes("Courier New",	  Win32.FW_NORMAL,	false),	// Monospace
			new FontAttributes("Times New Roman", Win32.FW_NORMAL,	false)  // Unknown
		};

		/**
		* the default font weight, may change this to look up from
		* some external source in the future.
		*/
		private static int DefaultFontWeight
		{
			get
			{
				return Win32.FW_NORMAL;
			}
		}
	}
}
