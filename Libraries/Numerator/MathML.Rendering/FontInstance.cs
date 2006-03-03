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
	 * a collection of glyphs that belong to a realized 
	 * font instance. As fonts can be created at any size, 
	 * this is an instnace of a font created at a particluar
	 * size. It caches a set of glyph areas that were created 
	 * with this font instance, and stores a set of glyph 
	 * sizes that are used to map a stretchy glyph.
	 */
	internal struct FontInstance
	{
		/** 
		 * the native font handle that is the native font
		 * for this instance.
		 */
		public readonly IFontHandle FontHandle;

		public readonly float PointSize;

		private StretchyGlyphAttributes[] stretchyAttributes;

		private SortedList cachedAreas;

    public FontInstance(IFormattingContext context, string fontName, float pointSize, StretchyGlyphIndices[] glyphIndices)
		{
			cachedAreas = null;
			PointSize = pointSize;
			FontHandle = context.CreateFont(pointSize, false, 500, fontName);
			stretchyAttributes = new StretchyGlyphAttributes[glyphIndices.Length];
			for(int i = 0; i < stretchyAttributes.Length; i++)
			{
				stretchyAttributes[i] = new StretchyGlyphAttributes(
					context, FontHandle, ref glyphIndices[i]);
			}
		}

		public Area GetStretchyArea(int stretchyIndex, BoundingBox desiredSize, out float lineThickness)
		{
			return stretchyAttributes[stretchyIndex].GetStretchyArea(FontHandle, desiredSize, out lineThickness);
		}
		
		public void CacheGlyphArea(char c, Area area)
		{
			if(cachedAreas == null) cachedAreas = new SortedList();
			cachedAreas.Add(c, new WeakReference(area));
		}

		/**
		 * locate an area that was previously cached in this 
		 * font instance
		 */
		public Area GetCachedArea(char c)
		{
			Area result = null;
			WeakReference reference = null;

			if(cachedAreas != null)
			{
				if((reference = (WeakReference)cachedAreas[c]) != null)
				{
					if(reference.IsAlive)
					{
						result = (Area)reference.Target;
					}
					else
					{
						cachedAreas.Remove(c);
					}
				}
			}

			return result;
		}
	}
}
