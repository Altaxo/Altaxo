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
using System.Collections;
using Scaled = System.Single;
using System.Diagnostics;

namespace MathML.Rendering 
{
	/**
	 * A factory for all areas.
	 * Althoughg many area types can be constructed with thier
	 * own contstructor, many others are either compound, or are 
	 * shared, so for the sake of consistancy, all areas are created
	 * with this factory.
	 */
	internal class AreaFactory 
	{
		/**
		 * create an area that formats a string.
		 */
		public static Area String(IFormattingContext context, String str) 
		{
			Area result = null;
			float fontSize = context.Size;
			ArrayList list = null;
			int start = 0;
			
			if(str.Length == 1)
			{
				result = str[0] > '\x7f' ? GlyphFactory.GetGlyph(context, fontSize, str[0]) :
					new StringArea(context, str);
			}
			else
			{
				start = 0;
				for(int i = 0; i < str.Length; i++)
				{
					if(str[i] > '\x7f')
					{
						if(list == null) { list = new ArrayList(); }
						list.Add(new StringArea(context, str.Substring(start, i - start)));
						list.Add(GlyphFactory.GetGlyph(context, fontSize, str[i]));
						start = i + 1;
					}
				}
				
				if(list != null)
				{
					result = Horizontal((Area[])list.ToArray(typeof(Area)));
				}
				else
				{
					result = new StringArea(context, str);
				}
			}
			return result;
		}

		public static Area Glyph(IFormattingContext context, String fontName, 
			String altFontName, ushort index) 
		{ 
			IFontHandle font = context.GetFont(fontName, altFontName);
			return new GlyphArea(context, font, index);
		}

		public static Area Background(System.Drawing.Color color, Area area) 
		{ 
			return new BackgroundArea(color, area);
		}

		/**
		 * create a new box area
		 */
		public static Area Box(BoundingBox box, Area area) 
		{
			return new BoxArea(area, box);
		}

		/**
		 * create a color area
		 */
		public static Area Color(System.Drawing.Color color, Area area) 
		{ 
			return new ColorArea(color, area); 
		}

		public static Area Freeze() { return null; }		

		/**
		 * make a new hide area
		 */
		public static Area Hide(Area area) 
		{
			return new HideArea(area);
		}

		/**
		 * create a horizontal area
		 */
		public static Area Horizontal(Area[] areas) 
		{ 
			return new HorizontalArea(areas); 
		}

		/**
		 * create a horizontal area with the given area 
		 * centered between two fillers. The HorizontalArea is created
		 * with a null element so that only the point inside the center area
		 * will return an element.
		 */
		public static Area HorizontalCenter(Area area) 
		{
			Area[] areas = {new HorizontalFillerArea(), area, new HorizontalFillerArea() };
			return new HorizontalArea(areas);
		}

		/**
		 * create a new horizontal filler area
		 */
		public static Area HorizontalFiller() 
		{ 
			return new HorizontalFillerArea(); 
		}

		/**
		 * create a horizontal space area with the given
		 * area pushed to the left
		 */
		public static Area HorizontalLeft(Area area) 
		{ 
			Area[] areas = { area, new HorizontalFillerArea() };
			return new HorizontalArea(areas);
		}

		/**
		 * create a horizontal area with the given area pushed to
		 * the right
		 */
		public static Area HorizontalRight(Area area) 
		{ 
			Area[] areas = { new HorizontalFillerArea(), area };
			return new HorizontalArea(areas);
		}

		/**
		 * create a horizontal space area with the given width
		 */
		public static Area HorizontalSpace(Scaled width) 
		{ 
			return new HorizontalSpaceArea(width); 
		}

		public static Area Id() { return null; }
		public static Area Ignore() { return null; }

		/**
		 * create an InkArea
		 */
		public static Area Ink(Area area) 
		{ 
			return new InkArea(area); 
		}

		/**
		 * create an area that will render a horizontal line. 
		 * this line will the the width of the bounding box that this
		 * area will be Fit in to.
		 */
		public static Area HorizontalLine(Scaled thickness) 
		{ 
			float halfThickness = thickness / 2.0f;
			Area[] areas = { new HorizontalFillerArea(), new VerticalSpaceArea(halfThickness, halfThickness) };
			return new InkArea(new HorizontalArea(areas));
		}

		/**
		 * create an area that will render a vertical line.
		 * this line will fill the height of the bounding box that
		 * this area will be Fit in to.
		 */
		public static Area VerticalLine(System.Drawing.Color color, Scaled thickness) 
		{
			Area[] areas = { new VerticalFillerArea(), 
							   new HorizontalSpaceArea(thickness) };
			return new ColorArea(color, new InkArea(new HorizontalArea(areas)));

		}

		public static Area Mark() { return null; }

		/**
		 * create a shift area
		 */
		public static Area Shift(Scaled shift, Area area) 
		{ 
			return new ShiftArea(shift, area);
		}

		/**
		 * create a vertical aray area
		 */
		public static Area Vertical(Area[] areas, int baseline) 
		{ 
			return new VerticalArea(areas, baseline); 
		}

		/**
		 * create an area that will pushed to the bottom of the
		 * space that it will be fit into
		 */
		public static Area VerticalBottom(Area area) 
		{ 
			Area[] areas = {area, new VerticalFillerArea()};
			return new VerticalArea(areas, 0);
		}

		/**
		 * create an area where the given area will be centered in
		 * the space that it will be fit into
		 */
		public static Area VerticalCenter(Area area) 
		{
			// need to first center the area
			BoundingBox box = area.BoundingBox;
			if(box.Depth != box.Height)
			{
				area = Shift((box.Depth - box.Height) / 2.0f, area);
				Debug.Assert(area.BoundingBox.Height == area.BoundingBox.Depth);
			}
			Area[] areas = {new VerticalFillerArea(), area, new VerticalFillerArea() };
			return new VerticalArea(areas, 0);
		}

		/**
		 * create a vertical filler area
		 */
		public static Area VerticalFiller() 
		{
			return new VerticalFillerArea();
		}

		/**
		 * create a new vertical space area with the given height and depth
		 */
		public static Area VerticalSpace(Scaled height, Scaled depth) 
		{ 
			return new VerticalSpaceArea(height, depth); 
		}

		/**
		 * create an area where the given area will be pushed
		 * to the top of the space where the area will be fit
		 * in to
		 */
		public static Area VerticalTop(Area area) 
		{
			Area[] areas = {new VerticalFillerArea(), area};
			return new VerticalArea(areas, 0);
		}

		/**
		 * create an overlapped area
		 */
		public static Area Overlap(Area[] areas) 
		{
			return new OverlapArea(areas);
		}

		/**
		 * Create a fraction area.
		 * This is an area that consists of 2 areas vertically separated by a line.
		 * this notion natuarally falls into the conecpt of a vertical array area, 
		 * an area, a line, and the second area.
		 */
		public static Area Fraction(IFormattingContext ctx, Area numerator, Area denominator, float lineThickness) 
		{
      
			Area space = new VerticalSpaceArea(ctx.OnePixel, ctx.OnePixel);
			Area line = HorizontalLine(ctx.OnePixel);

			numerator = HorizontalCenter(numerator);

			denominator = HorizontalCenter(denominator);

			Area[] areas = {denominator, space, line, space, numerator};

			return Shift(ctx.Axis, new VerticalArea(areas, 2));
		}

		/**
		 * format a script area. This is an area with a base, and
		 * optional super and subscript areas.
		 */
		public static Area Script(IFormattingContext context, Area baseArea, 
			Area subArea, Length subMinShift, Area superArea, Length superMinShift) 
		{
			// we might have a italic base where we have an overhang at the top, in this
			// case we need to add a space for the script elements 
			float right = baseArea.RightEdge;

			// index where to put script elements
			int scriptIndex;

			// horz area, length = 2, base + scripts or length = 3: base + space + scripts
			Area[] horz;
			float subShift, superShift;

			if(right < 0)
			{
				scriptIndex = 2;
				horz = new Area[3];
				horz[1] = new HorizontalSpaceArea(-right);
			}
			else
			{
				scriptIndex = 1;
				horz = new Area[2];
			}		

			// set the base
			horz[0] = baseArea;

			CalculateScriptShift(context, 
				baseArea.BoundingBox,
				subArea != null ? subArea.BoundingBox : BoundingBox.New(), 
				subMinShift, 
				superArea != null ? superArea.BoundingBox : BoundingBox.New(), 
				superMinShift, 
				out subShift, out superShift);        
		
			// the script areas go in an overlap area
			if(subArea != null && superArea != null) 
			{
				Area[] overlap = new Area[2];
				overlap[0] = Shift(-subShift, subArea);
				overlap[1] = Shift(superShift, superArea);
				horz[scriptIndex] = Overlap(overlap);
			}
			else if(subArea != null) 
			{
				horz[scriptIndex] = Shift(-subShift, subArea);
			}
			else if(superArea != null) 
			{
				horz[scriptIndex] = Shift(superShift, superArea);
			}
			else 
			{
				// no script areas, this is not good, but deal with it anyway
				Debug.WriteLine("Warning, no script areas while create a script area, creating default glyph area");
				horz[scriptIndex] = String(context, "?");
			}

			// make a horizontal area out of these
			return Horizontal(horz);
		}

		/**
		 * format a radical
		 */
		public static Area Radical(IFormattingContext context, Area radicand, Area index)
		{
			const char RadicalGlyphIndex = '\x221a';
			float fontSize = context.Size;
			BoundingBox radicandBox = radicand.BoundingBox;
			BoundingBox radicalBox;
			float minIndexWidth;
			float lineThickness;

			// these space numbers have no real meaning, the were just
			// chosen as a size that looks good.
			float leftSpace = 2*context.OnePixel + radicandBox.Width * 0.01f;
			float rightSpace = 2*context.OnePixel;
			float topMinSpace = context.OnePixel + radicandBox.VerticalExtent * 0.03f;			

			// size to create glyph
			BoundingBox glyphBox = radicandBox;

			// add minimun stretch sizes to the box, sqrt is a vertical stretchy glyph
			glyphBox.Height += topMinSpace;

			// create a glyph for the radical char
			Area radical = GlyphFactory.GetStretchyGlyph(context, fontSize, RadicalGlyphIndex, glyphBox, out lineThickness);			

			// line for the top part of the radical
			Area horizontalLine = HorizontalLine(lineThickness);

			radicalBox = radical.BoundingBox;

			// the glyph is almost never the exact size we request it, 
			// we we need to adjust our vertical padding accoringly
			float topSpace = radicalBox.Height - radicandBox.Height - horizontalLine.BoundingBox.VerticalExtent;

			// pad the radicand with left and right spaces
			radicand = Horizontal(new Area[] {HorizontalSpace(leftSpace), radicand, HorizontalSpace(rightSpace)});

			// make a new vertical array, with a line for the top of the radical spanning
			// the widtch, and the original radicand spaced slightly downward
			radicand = Vertical(new Area[] {radicand, VerticalSpace(topSpace, 0), horizontalLine}, 0);	


			// get the minumum index width, this is needed as radical glyphs
			// are encoded with a negative left edge, this is where the right edge of
			// the index area would normally go. note, we better well have a 
			// negative width, but just in case.....
			minIndexWidth = radical.LeftEdge < 0.0f ? -radical.LeftEdge : 0.0f;

			if(index == null)
			{
				// just make a space to padd the radical
                index = HorizontalSpace(minIndexWidth);				
			}
			else
			{
				BoundingBox indexBox = index.BoundingBox;

				// need to pad the area if less than min width
				if(indexBox.Width < minIndexWidth)
				{
					index = Horizontal(new Area[] {HorizontalSpace(minIndexWidth - indexBox.Width), index});
				}

                // shift the area up just above the radical hook
				index = Shift(GetRadicalShift(radical) + index.BoundingBox.Depth, index);
			}			

			// hide the radical glyph from cursor selection
			radical = new NonSelectionArea(radical);

			// make a new horizontal area out of these three areas
			return Horizontal(new Area[] {index, radical, radicand});
		}

		/**
		 * calculate vertical shift of the radical index.
		 */
		private static float GetRadicalShift(Area radical)
		{
			// first we need the actual glyph area, this may be either a
			// simple glyph, or a compound glyph in a vertical array, 
			// note we only need the radical portion of the compound glyph

			// try the first case, this is a shift area with a 
			// single glyph area child (from a simple stretchy glyph)
			Area glyph = radical.GetArea(new AreaIdentifier(0));

			if(glyph is VerticalArea)
			{
				// the sqrt glyph is allways the bottom, or first element 
				// in a vertical array, if we have a compound glyph
				glyph = radical.GetArea(new AreaIdentifier(new int[] {0,0}));
			}

			Debug.Assert(glyph is GlyphArea);

            // the shift amount is the difference between the total depth,
			// and the glyph depth.
            return glyph.BoundingBox.Depth - radical.BoundingBox.Depth;
		}

		/**
		 * calculate the default amout the script the baseline of the
		 * script rows. this ignores (for now anyway) the min shift values
		 * specified in the DOM
		 */
		private static void CalculateScriptShift(IFormattingContext context, 
			BoundingBox baseBox, BoundingBox subBox, Length subMinSize, 
			BoundingBox superBox, Length superMinSize, 
			out float subShift, out float superShift) 
		{

			CalculateScriptShift(context, baseBox, subBox, superBox, 
				out subShift, out superShift);
		}

		/**
		 * calculate the default amout the script the baseline of the
		 * script rows. this ignores (for now anyway) the min shift values
		 * specified in the DOM.
		 */
		private static void CalculateScriptShift(IFormattingContext context, 
			BoundingBox baseBox, BoundingBox subBox, BoundingBox superBox, 
			out float subShift, out float superShift) 
		{

			float ex = context.Ex;
			float axis = context.Axis;
			float rule = context.DefaultLineThickness;

			superShift = Math.Max(ex, baseBox.Height - axis);
			subShift = Math.Max(axis, baseBox.Depth + axis);

			if(!superBox.Defined) 
			{
				superShift = 0;
				subShift = Math.Max(subShift, subBox.Height - (ex * 4.0f) / 5);
			}
			else 
			{
				superShift = Math.Max(superShift, superBox.Depth + ex / 4.0f);
				if(!subBox.Defined) 
				{
					subShift = 0;
				}
				else 
				{
					if((superShift - superBox.Depth) - (subBox.Height - superShift) < 4.0f * rule) 
					{
						subShift = 4.0f * rule - superShift + superBox.Depth + subBox.Height;
						float psi = (4.0f * ex) / 5.0f - (superShift - superBox.Depth);
						if(psi > 0.0f) 
						{
							subShift -= psi;
							superShift += psi;
						}
					}
				}
			}
		}

		public static Area Table()
		{
			return null;
		}

		/// <summary>
		/// create a space area that occupies the given size.
		/// </summary>
		/// <param name="size">the size to make a space area</param>
		/// <returns></returns>
		public static Area Space(BoundingBox size)
		{
			return new BoxArea(new IgnoreArea(), size);
		}
	}
}
