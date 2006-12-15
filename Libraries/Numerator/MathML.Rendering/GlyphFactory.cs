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
    static GlyphFactory _instance;

		/**
		 * a collection of glyph maps. each one is loaded from a config
		 * file nammed font-configuration-?????.xml. All files matching 
		 * this nameing convention in the specified directory are enumerated, 
		 * and a glyph map is loaded for each one.
		 */
		private GlyphMap[] maps;
    
    /// <summary>
    /// We use a instance conctructor instead of a static constructor here. This is because
    /// an exception in a static constructur is embedded in another exception and therefore not easy
    /// to understand.
    /// </summary>
		private GlyphFactory()
		{
      string configDir = System.Configuration.ConfigurationManager.AppSettings.Get("MathMLRenderingConfig");
      string searchDir = null;

      if(configDir==null || configDir.Length==0)
        searchDir = System.IO.Path.GetDirectoryName( System.Reflection.Assembly.GetEntryAssembly().Location);
      else if(Path.IsPathRooted(configDir))
      {
        if(Directory.Exists(configDir))
          searchDir = configDir;
        else
          throw new ApplicationException("Configured font configuration file directory does not exist! Configured path: " + configDir);
      }
      else // path is not rooted
      {
        string basepath = System.IO.Path.GetDirectoryName( System.Reflection.Assembly.GetEntryAssembly().Location) + Path.DirectorySeparatorChar;
        if(Directory.Exists(basepath+configDir))
          searchDir = basepath + configDir;
        else
          throw new ApplicationException("Configured font configuration file directory does not exist! Configured path: " + configDir + ", the base path is: " + basepath);
      }

			string[] files = Directory.GetFiles(searchDir, "font-configuration-*.xml");

      if(files.Length==0)
        throw new ApplicationException("MathMLRendering: No font configuration files found in directory: " + searchDir);

			Array.Sort(files);

			maps = new GlyphMap[files.Length];

			for(int i = 0; i < maps.Length; i++)
			{
				maps[i] = new GlyphMap(files[i]);
			}
		}

    public static GlyphFactory Instance
    {
      get
      {
        if (_instance == null)
          _instance = new GlyphFactory();
        return _instance;
      }
    }


		/**
		 * create a new glyph area.
		 */
    public static Area GetGlyph(IFormattingContext ctx, float pointSize, char c)
		{
      GlyphFactory gf = Instance;

			Area result = null;

			Debug.WriteLine(String.Format("searching for a glyph for character 0x{0:x}", (uint)c));
			for(int i = 0; i < gf.maps.Length; i++)
			{
				if((result = gf.maps[i].GetGlyph(ctx, pointSize, c)) != null)
					return result;
			}

			if(result == null)
			{
				Debug.WriteLine("no glyph found, returning default area");
				result = new StringArea(ctx, "?");             
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
    public static Area GetStretchyGlyph(IFormattingContext context, float pointSize, char c, BoundingBox desiredSize, out float lineThickness)
		{
      GlyphFactory gf = Instance;

			Area result = null;
			lineThickness = 0;
			for(int i = 0; i < gf.maps.Length; i++)
			{
                if((result = gf.maps[i].GetStretchyGlyph(context, pointSize, c, desiredSize, out lineThickness)) != null)
					return result;
			}

			if(result == null)
			{
				Debug.WriteLine("no stretchy glyph found, returning standard glyph area");
				result = GetGlyph(context, pointSize, c);          
			}
			return result;
		}

    public static Area GetStretchyGlyph(IFormattingContext context, float pointSize, char c, BoundingBox desiredSize)
		{
			float lineThickness;
			return GetStretchyGlyph(context, pointSize, c, desiredSize, out lineThickness);
		}
	}
}
