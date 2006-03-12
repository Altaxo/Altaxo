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
using System.Diagnostics;


namespace MathML.Rendering.GlyphMapper
{
	internal struct GlyphMap
	{
		// the font name
		private readonly string fontName;

		// factor to scale font by relative to current font
		private readonly float scaleFactor;

		// list of font instances
		private FontInstance[] fontInstances;

		private StretchyGlyphIndices[] stretchyGlyphIndices;
		private SimpleGlyphAttributes[] simpleGlyphs;
		private SimpleGlyphRange[] simpleGlyphRanges;

		public GlyphMap(string configFile)
		{
			fontInstances = new FontInstance[0];
			stretchyGlyphIndices = null;
			simpleGlyphs = null;
			simpleGlyphRanges = null;
			fontName = null;
			scaleFactor = 1.0f;

			try
			{
				Debug.WriteLine("loading glyph map from file:" + configFile);
				XmlDocument doc = new XmlDocument();

				doc.Load(new System.Xml.XmlTextReader(configFile));
				
				// get the root, should be "font-configuration", root should
				// have 2 child nodes, "stretchy-glyphs" and "simple-glyphs"
				XmlElement root = (XmlElement)doc.DocumentElement;

				fontName = root.GetAttribute("font-name");

				string scalingfactor = root.GetAttribute("scaling-factor");
				if(scalingfactor.Length > 0) scaleFactor = Utility.ParseSingle(scalingfactor);
				
				for(int i = 0; i < root.ChildNodes.Count; i++)
				{
					LoadXmlNode(root.ChildNodes[i]);
				}

				// check the values of the member varibles, if they have not been
				// created from the config file, init them to thier default 
				// values
				if(stretchyGlyphIndices == null) stretchyGlyphIndices = new StretchyGlyphIndices[0];
				if(simpleGlyphRanges == null) simpleGlyphRanges = new SimpleGlyphRange[0];
				
			}
			catch(System.Exception e)
			{
				stretchyGlyphIndices = new StretchyGlyphIndices[0];
				simpleGlyphs = new SimpleGlyphAttributes[0];
				simpleGlyphRanges = new SimpleGlyphRange[0];
				fontName = "";
				throw new Exception("could not load mathfont.config.xml, " + e.ToString());
			}	
		}

		/**
		 * create a new glyph area.
		 */
    public Area GetGlyph(IFormattingContext context, float pointSize, char c)
		{
			Area result = null;
			int simpleGlyphIndex;
			int fontIndex = GetFontIndex(context, pointSize);

			if(fontIndex >= 0)
			{
				// first try to find a cached glyph
				if((result = this.fontInstances[fontIndex].GetCachedArea(c)) != null)
				{
					return result;
				}

				// look for a glyph in a range
				for(int i = 0; i < simpleGlyphRanges.Length; i++)
				{
					if((result = simpleGlyphRanges[i].GetArea(context, fontInstances[fontIndex].FontHandle, c)) != null)
					{
						fontInstances[fontIndex].CacheGlyphArea(c, result);
						return result;                        
					}
				}

				// look for the glyph in the maps
				if((simpleGlyphIndex = this.GetSimpleIndex(c)) >= 0 &&
					(result = 
					simpleGlyphs[simpleGlyphIndex].GetArea(context, fontInstances[fontIndex].FontHandle)) != null)
				{
					fontInstances[fontIndex].CacheGlyphArea(c, result);
					return result;
				}				
			}

			Debug.WriteLine(String.Format("failed to find glyph for 0x{0:x}, null", (ushort)c));
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
		 */
    public Area GetStretchyGlyph(IFormattingContext context, float pointSize, char c, BoundingBox desiredSize, out float lineThickness)
		{
			Area result = null;

			Debug.WriteLine(String.Format("requesting stretchy glyph, pointSize: {0}, char: {1}, desiredSize: {2}", 
				pointSize, c, desiredSize));

			lineThickness = 0.0f;
			int stretchyIndex = GetStretchyIndex(c);
			if(stretchyIndex >= 0)
			{
				int fontIndex = GetFontIndex(context, pointSize);
				if(fontIndex >= 0)
				{
					result = fontInstances[fontIndex].GetStretchyArea(stretchyIndex, desiredSize, out lineThickness);
				}
			}

			Debug.WriteLineIf(result == null, "stretchy glyph request failed in font " + fontName);
			Debug.WriteLineIf(result != null, "stretchy glyph request succeeded in " + fontName + " for " + c + ", lineThickness: " + lineThickness);
			return result;
		}

        /**
		 * locate a index for a simple glyph item.
		 * return -1 if none exists.
		 */
		private int GetSimpleIndex(char c)
		{
			for(int i = 0; i < simpleGlyphs.Length; i++)
			{
				if(simpleGlyphs[i].Char == c) 
					return i;
			}
			return -1;
		}

		/** 
		 * find an index to a font that mathces the given font cell height, 
		 * if no font is availible, one is created with this height.
		 */
    private int GetFontIndex(IFormattingContext context, float fontSize)
		{
			fontSize = (fontSize * scaleFactor);
			for(int i = 0; i < fontInstances.Length; i++)
			{
				if(fontInstances[i].PointSize == fontSize)
				{
					return i;
				}
			}

			// no font found, so try to create one.
			return CreateFont(context, fontSize);
		}

		/** 
		 * create a font with the given font size. return the index of the new
		 * font, or -1 if we failed to create the font
		 */
    private int CreateFont(IFormattingContext context, float fontSize)
		{
			fontSize = (fontSize * scaleFactor);
			FontInstance[] tmp = fontInstances;
			fontInstances = new FontInstance[fontInstances.Length + 1];	
			tmp.CopyTo(fontInstances, 0);
			fontInstances[fontInstances.Length - 1] = new FontInstance(context, fontName, fontSize, stretchyGlyphIndices);
			return fontInstances.Length - 1;
		}	

		private int GetStretchyIndex(char c)
		{
			for(int i = 0; i < stretchyGlyphIndices.Length; i++)
			{
				if(stretchyGlyphIndices[i].Char == c)
					return i;
			}
			return -1;
		}

		/**
		 * load one of the root xml nodes.
		 */
		private void LoadXmlNode(XmlNode node)
		{
			// need child nodes in any case
			XmlNodeList nodes = node.ChildNodes;
			int i = 0;

			if(node.Name == "stretchy-glyphs")
			{
				stretchyGlyphIndices = new StretchyGlyphIndices[nodes.Count];
				foreach(XmlElement n in nodes)
				{
					stretchyGlyphIndices[i++] = new StretchyGlyphIndices(n);
				}
			}
			else if(node.Name == "simple-glyphs")
			{
				simpleGlyphs = new SimpleGlyphAttributes[nodes.Count];
				foreach(XmlElement n in nodes)
				{
					simpleGlyphs[i++] = new SimpleGlyphAttributes(
						Char.Parse(n.GetAttribute("char")), UInt16.Parse(n.GetAttribute("index")));
				}
			}
			else if(node.Name == "simple-glyph-ranges")
			{
				simpleGlyphRanges = new SimpleGlyphRange[nodes.Count];
				foreach(XmlElement n in nodes)
				{
					simpleGlyphRanges[i++] = new SimpleGlyphRange(n);
				}
			}
			else
			{
				throw new Exception("Error, \"" + node.Name + "\" is not a valid item in a font config file");
			}
		}
	}
}
