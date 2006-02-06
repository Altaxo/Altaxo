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
using System.Configuration;
using System.IO;
using System.Diagnostics;
using MathML.Rendering.GlyphMapper;
using System.Windows.Forms;

namespace MathML.Rendering
{
	/**
	 * the source of glyphs.
	 * a glyph can be identified by a character index, and a font, 
	 * these are used to store each glyph in a hash table.
	 * 
	 * glyphs are shared (using the flyweight patern), and therefore
	 * are read only.
	 * 
	 * Most areas are created by the area factory, but glyphs are a 
	 * very speciallized area, and therefore all the logic to manage
	 * them has been islolated into this class.
	 */
	internal class GlyphFactory
	{
		/**
		 * a collection of glyph maps. each one is loaded from a config
		 * file nammed font-configuration-?????.xml. All files matching 
		 * this nameing convention in the specified directory are enumerated, 
		 * and a glyph map is loaded for each one.
		 */
		private static GlyphMap[] maps;

		static GlyphFactory()
		{
			string configDir = ConfigurationSettings.AppSettings.Get("mathml-rendering-config");


			if(configDir == null || configDir.Length == 0 || !Directory.Exists(configDir))
			{
				MessageBox.Show(
@"Warning, no 'mathml-rendering-config' entry found in the application's 
.config file or the entry is an invalid path. 
Looking in current directory for font-configuration files.");

				configDir = Application.StartupPath; 
			}

			string[] files = Directory.GetFiles(configDir, "font-configuration-*.xml");

			Array.Sort(files);

			maps = new GlyphMap[files.Length];

			for(int i = 0; i < maps.Length; i++)
			{
				maps[i] = new GlyphMap(files[i]);
			}
		}

		/**
		 * create a new glyph area.
		 */
		public static Area GetGlyph(int pointSize, char c)
		{
			Area result = null;

			Debug.WriteLine(String.Format("searching for a glyph for character 0x{0:x}", (uint)c));
			for(int i = 0; i < maps.Length; i++)
			{
				if((result = maps[i].GetGlyph(pointSize, c)) != null)
					return result;
			}

			if(result == null)
			{
				Debug.WriteLine("no glyph found, returning default area");
				result = new StringArea(new FormattingContext(), "?");             
			}
			return result;
		}

		/**
		 * find or calculate an area that will fill the requested cell 
		 * height. The returned area may be either a single glyph, or a
		 * compound set of glyphs.
		 * 
		 * @param pointSize the evaluated font size
		 * @param desiredSize the desired stretch size (for either vertical or 
		 * horizontal stretchy glyphs.
		 * @param c the character to find a glyph for.
		 * @param lineThickness a value that get populated with the thickness of the 
		 * repated or stretched sections.
		 */
		public static Area GetStretchyGlyph(int pointSize, char c, BoundingBox desiredSize, out float lineThickness)
		{
			Area result = null;
			lineThickness = 0;
			for(int i = 0; i < maps.Length; i++)
			{
                if((result = maps[i].GetStretchyGlyph(pointSize, c, desiredSize, out lineThickness)) != null)
					return result;
			}

			if(result == null)
			{
				Debug.WriteLine("no stretchy glyph found, returning standard glyph area");
				result = GetGlyph(pointSize, c);          
			}
			return result;
		}

		public static Area GetStretchyGlyph(int pointSize, char c, BoundingBox desiredSize)
		{
			float lineThickness;
			return GetStretchyGlyph(pointSize, c, desiredSize, out lineThickness);
		}
	}
}
