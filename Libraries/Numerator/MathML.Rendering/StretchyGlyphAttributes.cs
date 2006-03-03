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
using System.Diagnostics;
using System.Drawing;

namespace MathML.Rendering.GlyphMapper
{
	internal enum StretchyOrientation
	{
		Vertical, Horizontal
	}

	/**
	 * a set of glyph metrics for a collection of glyph indices.
	 * These are specific to a particular font instance.
	 */
	internal struct StretchyGlyphAttributes
	{
		/**
		 * set of attributes for the simple glyphs, this may be null
		 */
		private readonly GlyphAttributes[] simpleGlyphs;

		/**
		 * set of attributes for the compound glyphs, this may be null.
		 */
		private readonly GlyphAttributes[] compoundGlyphs;

		/**
		 * the orientation of this glyph
		 */
		private readonly StretchyOrientation orientation;

		/**
		 * create a set of attributes.
		 * this calculates all of the glyph metrics from the graphic device.
		 */
		public StretchyGlyphAttributes(IFormattingContext context, IFontHandle fontHandle, ref StretchyGlyphIndices indices)
		{
			orientation = indices.Orientation;

			if(indices.SimpleIndices != null)
			{
				simpleGlyphs = new GlyphAttributes[indices.SimpleIndices.Length];
				for(int i = 0; i < simpleGlyphs.Length; i++)
				{
					simpleGlyphs[i] = new GlyphAttributes(context, fontHandle, 
						indices.SimpleIndices[i], GlyphAttributes.FudgeNone);
				}
			} 
			else
			{
				simpleGlyphs = null;
			}

			if(indices.CompoundIndices != null)
			{
				compoundGlyphs = new GlyphAttributes[indices.CompoundIndices.Length];
				for(int i = 0; i < compoundGlyphs.Length; i++)
				{
					// TODO deal with horizontal glyphs
					compoundGlyphs[i] = new GlyphAttributes(context, fontHandle, 
						indices.CompoundIndices[i], 
						i == StretchyGlyphIndices.Filler ? GlyphAttributes.FudgeHeight : GlyphAttributes.FudgeNone);
				}
			}
			else
			{
				compoundGlyphs = null;
			}
		}

        /**
		 * try to get a stretchy glyph 3 different ways, first try to find a 
		 * simple glyph that is just larger than the desired size. If we can
		 * not find a simple glyph, we try to fit together a compound glyph
		 * by repeating the filler area. If that fails, the last resort is 
		 * to just get the largest simple glyph.
		 * 
		 * once a glyph is found, it is centered in the desiredSize region.
		 */
		public Area GetStretchyArea(IFontHandle fontHandle, BoundingBox desiredSize, out float lineThickness)
		{
			Area result = null;
			if((result = GetSimpleArea(fontHandle, desiredSize)) == null)
			{
				if((result = GetCompoundArea(fontHandle, desiredSize)) == null)
				{
					result = GetLargestSimpleArea(fontHandle);
				}
			}
			
			if(result != null)
			{
				lineThickness = GetLineThickness();
				result = ShiftArea(result, desiredSize);
			}
			else
			{
				lineThickness = 0.0f;
			}

			return result;
		}

		/**
		 * calulated the line thickness for an glyph for which we have a repated filler 
		 * section. The line thickness is the width of the filler section.
		 */
		private float GetLineThickness()
		{
			if(orientation == StretchyOrientation.Vertical)
			{
				 return compoundGlyphs[StretchyGlyphIndices.Filler].Box.HorizontalExtent -
					 compoundGlyphs[StretchyGlyphIndices.Filler].Left -
					 compoundGlyphs[StretchyGlyphIndices.Filler].Right;
			}
			else
			{
				return compoundGlyphs[StretchyGlyphIndices.Filler].Box.VerticalExtent -
					compoundGlyphs[StretchyGlyphIndices.Filler].Left -
					compoundGlyphs[StretchyGlyphIndices.Filler].Right;
			}
		}

		/**
		 * search through all of the simple glyphs for this character, and 
		 * return the first one that is just larger that the requested 
		 * size. return null if no mathching glyph is found
		 */
		private Area GetSimpleArea(IFontHandle fontHandle, BoundingBox desiredSize)
		{
			if(orientation == StretchyOrientation.Vertical)
			{
				for(int i = 0; i < simpleGlyphs.Length; i++)
				{
					if(simpleGlyphs[i].Box.VerticalExtent >= desiredSize.VerticalExtent)
					{
						return simpleGlyphs[i].GetGlyph(fontHandle);
					}
				}
			}
			else
			{
				for(int i = 0; i < simpleGlyphs.Length; i++)
				{
					if(simpleGlyphs[i].Box.HorizontalExtent >= desiredSize.HorizontalExtent)
					{
						return simpleGlyphs[i].GetGlyph(fontHandle);
					}
				}
			}
			return null;
		}

		private Area GetCompoundArea(IFontHandle fontHandle, BoundingBox desiredSize)
		{
			Area[] areas = null;
			if(compoundGlyphs[StretchyGlyphIndices.Bottom].Index >= 0)
			{
				if(compoundGlyphs[StretchyGlyphIndices.Top].Index >= 0)
				{
					if(compoundGlyphs[StretchyGlyphIndices.Middle].Index >= 0)
					{
                        areas = StretchBottomMiddleTop(fontHandle, desiredSize);
					}
					else
					{
						areas = StretchBottomTop(fontHandle, desiredSize);
                    }
				}
				else
				{
					areas = StretchBottom(fontHandle, desiredSize);
				}
			}
			else
			{
				if(compoundGlyphs[StretchyGlyphIndices.Top].Index >= 0)
				{
					areas = StretchTop(fontHandle, desiredSize);
				}
				else
				{
					areas = StretchFillerOnly(fontHandle, desiredSize);
				}
			}

            return orientation == StretchyOrientation.Vertical ?
				(Area)(new VerticalCompoundGlyph(null, areas, 0)) : (Area)(new HorizontalCompoundGlyph(areas));
			
		}

		private Area GetLargestSimpleArea(IFontHandle fontHandle)
		{
			Area result = null;
			if(simpleGlyphs.Length > 0)
			{
				result = simpleGlyphs[simpleGlyphs.Length - 1].GetGlyph(fontHandle);
			}
			return result;
		}

		private Area[] StretchBottom(IFontHandle fontHandle, BoundingBox box)
		{
			Area[] result;
			Area filler = compoundGlyphs[StretchyGlyphIndices.Filler].GetGlyph(fontHandle);
			float availSpace = (orientation == StretchyOrientation.Vertical ?
                box.VerticalExtent - compoundGlyphs[StretchyGlyphIndices.Bottom].Box.VerticalExtent :
				box.Width = compoundGlyphs[StretchyGlyphIndices.Bottom].Box.Width);
			int fillerCount = GetFillerCount(availSpace);
			result = new Area[fillerCount + 1];
			result[0] = compoundGlyphs[StretchyGlyphIndices.Bottom].GetGlyph(fontHandle);
			for(int i = 1; i < result.Length; i++)
			{
				result[i] = filler;
			}
		
			return result;
		}

		private Area[] StretchTop(IFontHandle fontHandle, BoundingBox box)
		{
			Area[] result;
			Area filler = compoundGlyphs[StretchyGlyphIndices.Filler].GetGlyph(fontHandle);
			float availSpace = (orientation == StretchyOrientation.Vertical ?
				box.VerticalExtent - compoundGlyphs[StretchyGlyphIndices.Top].Box.VerticalExtent :
				box.Width = compoundGlyphs[StretchyGlyphIndices.Top].Box.Width);
			int fillerCount = GetFillerCount(availSpace);
			result = new Area[fillerCount + 1];
			for(int i = 0; i < result.Length - 1; i++)
			{
				result[i] = filler;
			}	
			result[result.Length - 1] = compoundGlyphs[StretchyGlyphIndices.Top].GetGlyph(fontHandle);	
			return result;
		}

		private Area[] StretchBottomTop(IFontHandle fontHandle, BoundingBox box)
		{
			float availSpace;
			int fillerCount;
			Area[] result;
			Area filler = compoundGlyphs[StretchyGlyphIndices.Filler].GetGlyph(fontHandle);

			if(orientation == StretchyOrientation.Vertical)
			{
				availSpace = box.VerticalExtent - (compoundGlyphs[StretchyGlyphIndices.Bottom].Box.VerticalExtent +
					compoundGlyphs[StretchyGlyphIndices.Top].Box.VerticalExtent);
			}
			else
			{
				availSpace = box.Width - (compoundGlyphs[StretchyGlyphIndices.Bottom].Box.Width +
					compoundGlyphs[StretchyGlyphIndices.Top].Box.Width);
			}

			fillerCount = GetFillerCount(availSpace);
			result = new Area[fillerCount + 2];
			result[0] = compoundGlyphs[StretchyGlyphIndices.Bottom].GetGlyph(fontHandle);
			for(int i = 1; i < result.Length - 1; i++)
			{
				result[i] = filler;
			}			
			result[result.Length - 1] = compoundGlyphs[StretchyGlyphIndices.Top].GetGlyph(fontHandle);

			Debug.WriteLine(String.Format("found bottom-top compound area, {0} filler areas", fillerCount));
			return result;
		}

		private Area[] StretchBottomMiddleTop(IFontHandle fontHandle, BoundingBox box)
		{
			float availSpace;
			Area[] result;
			Area filler = compoundGlyphs[StretchyGlyphIndices.Filler].GetGlyph(fontHandle);

			if(orientation == StretchyOrientation.Vertical)
			{
				availSpace = box.VerticalExtent - (
					compoundGlyphs[StretchyGlyphIndices.Bottom].Box.VerticalExtent +
					compoundGlyphs[StretchyGlyphIndices.Middle].Box.VerticalExtent +
					compoundGlyphs[StretchyGlyphIndices.Top].Box.VerticalExtent);
			}
			else
			{
				availSpace = box.Width - (
					compoundGlyphs[StretchyGlyphIndices.Bottom].Box.Width +
					compoundGlyphs[StretchyGlyphIndices.Middle].Box.Width +
					compoundGlyphs[StretchyGlyphIndices.Top].Box.Width);
			}

			// use halfFillerCount to prevent compiler optimizations 
			// from removing divide multiply by 2
			int halfFillerCount = GetFillerCount(availSpace) / 2;

			// bump the filler count if we are short of the availible space
			if(halfFillerCount * 2 * (orientation == StretchyOrientation.Vertical ?
				filler.BoundingBox.VerticalExtent : filler.BoundingBox.Width) < availSpace)
			{
				halfFillerCount++;
			}

			result = new Area[2 * halfFillerCount + 3];
			result[0] = compoundGlyphs[StretchyGlyphIndices.Bottom].GetGlyph(fontHandle);
			for(int i = 1; i < halfFillerCount + 1; i++)
			{
				result[i] = filler;
			}
			result[halfFillerCount + 1] = compoundGlyphs[StretchyGlyphIndices.Middle].GetGlyph(fontHandle);
			for(int i = halfFillerCount + 2; i < result.Length - 1; i++)
			{
				result[i] = filler;
			}
			result[result.Length - 1] = compoundGlyphs[StretchyGlyphIndices.Top].GetGlyph(fontHandle);

			return result;
		}

		private Area[] StretchFillerOnly(IFontHandle fontHandle, BoundingBox box)
		{
			int fillerCount;
			Area[] result;
			Area filler = compoundGlyphs[StretchyGlyphIndices.Filler].GetGlyph(fontHandle);

			if(orientation == StretchyOrientation.Vertical)
			{
				fillerCount = GetFillerCount(box.VerticalExtent);
			}
			else
			{
				fillerCount = GetFillerCount(box.HorizontalExtent);
			}

			result = new Area[fillerCount];
			for(int i = 0; i < result.Length; i++)
			{
				result[i] = filler;
			}
			return result;
		}

		/**
		 * calculate the number of filler areas needed to fill
		 * a region. There may be cases where the avail space is zero, 
		 * such as if the 2 fixed pieces of a stretchy glyph are larger
		 * than the area to be fitted. If avail space is negative, 0 
		 * filler count is returned.
		 */
		private int GetFillerCount(float availibleSpace)
		{
			float fillerSize = (orientation == StretchyOrientation.Vertical ?
				compoundGlyphs[StretchyGlyphIndices.Filler].Box.VerticalExtent :
				compoundGlyphs[StretchyGlyphIndices.Filler].Box.HorizontalExtent);

			int fillerCount = (int)Math.Ceiling(availibleSpace / fillerSize);

			return fillerCount >= 0 ? fillerCount : 0;
		}

		/**
		 * center the area in the desired size
		 */
		private Area ShiftArea(Area area, BoundingBox desiredSize)
		{
			BoundingBox resultSize = area.BoundingBox;
			float diff = (resultSize.VerticalExtent - desiredSize.VerticalExtent) / 2.0f;
			float shift = desiredSize.Height + diff - resultSize.Height;

			area = AreaFactory.Shift(shift, area);

			Debug.WriteLine("fitting area, desired size: " + desiredSize + ",result size: " + area.BoundingBox); 
			return area;
		}
	}
}
