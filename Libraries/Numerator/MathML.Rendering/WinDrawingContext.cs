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
using System.Drawing;
using Scaled = System.Single;
using System.Diagnostics;
using System.Drawing.Drawing2D;

namespace MathML.Rendering
{
  #region History
  /**
	 * all native rendering and font related functions
	 * go here. The windows version will use this file called
	 * Win32_RenderingDevice.cs. In the future, we may create
	 * a Gtk_RenderingDevice. The .net Graphics object is far 
	 * to limited to handle all of the font handling and layout
	 * we need to do here, so we have to import the native api
	 * functions anyway.
	 * 
	 * All methods of this class should be static, as they
	 * just call into one of the native wrappers.
	 * 
	 * notes:
	 * 9-25-2003 verified that Win32.CreateFontIndirect returns 
	 * different hfonts for the same logfont. This means that the
	 * os can not be used to manage fonts, that is, we want to 
	 * be able to choose the SAME font many times without creating
	 * a new font each time, and it would appear that CreateFontIndirect
	 * allways creates a new font. This would be a tremendous waste of
	 * space if we allocated a new font for EACH glyph.
	 * 
	 * 9-28-2003
	 * Came up with a change / improvement to Luca's area model. This
	 * is the addition of the new FontArea. As the mathml tree structure
	 * is essentially the recursive setting and unsetting of rendering 
	 * states (point size, color, font, etc...), a font is just one of
	 * those states. Previously, the font was stored with each glyph /
	 * string, this wastes a good bit of space, and is inefficient in
	 * terms of rendering performance, as the font needs to be selectet /
	 * unseleged every time a glyph / character is drawn. The only time 
	 * the font really needs to be changed is when a mstyle, or a token
	 * node has a "mathvariant" attribute that is different than the
	 * previous, or inherited "mathvariant" attribute.
	 * 
	 * 10-25-2003
	 * re-wrote (yet again) font finding. Previously, native font handles
	 * were stored in a list, and each area that changed the font had a 
	 * reference to one of these. This posed a big problem, when do we clean
	 * up fonts? I could have let just the areas hold on to fonts, but then 
	 * how do we find them again, to be able to re-use them? I could have held
	 * onto a refernece to each font, but then the garbage collector will 
	 * never free them. This problem was solved by the use of WeakReferences.
	 * These allow the garbage collector to do it's job properly, without any
	 * homegrown reference counting system. Fonts are being created and found 
	 * correctly, this has been verified by the debug statements in the 
	 * new MathFont class, so when a series of them are destroyed, if they
	 * are needed again, they are re-created.
	 * 
	 * 10-26-2003
	 * Experimented with making the GraphicDevice statefull, that is passing
	 * allong a pointer to it in the formatting and rendering context. Turns
	 * out that this just complicates code, and causes problems, such as areas
	 * that replace themselves with a another area that needs a font. This
	 * class works best as a static class, we just NEED TO MAKE SURE ALL 
	 * OPERATIONS ARE DONE WHILE THE DC IS VALID, such a durring a paint.
	 * 
	 * 11-2-2003
	 * Now allow the MeasureGlyph method to be fully static and stateless.
	 * This is really needed for the new glyph factories. To allow this 
	 * method to be stateless, I created a new class to manage a device
	 * context that is only used for measuring glyphs. As this is an 
	 * object that wraps a true device context, we can allow the garbage
	 * collector the dispose of it when it is no longer needed.
	 * 
	 * 11-5-2003
	 * removed width measurment, all fonts are now created with a zero widht, 
	 * so all fonts will have the default aspect ratio. The stretchy glyph factory
	 * will take care of finding the right stretched version of a glyph character, 
	 * and we no longer use the native font mapper to apply any un-natural stretching.
	 * 
	 * 12-10-2003
	 * Definitions of Terms Used When Describing Fonts
	 * 
	 * 1-12-2004
	 * Made graphic device statefull for output methods, but kept
	 * stateless (static) for measurment methods
	 * 
	 * a-space, b-space, c-space
	 * The a-space is the distance from the left of the character frame
	 * to the left edge of the character. The b-space is the width of the
	 * character. The c-space is the distance from the right edge of the
	 * character to the right of the character frame. Negative values of
	 * a and c allow adjacent character frames to overlap. See also
	 * character increment, and space default values.
	 * 
	 * average char width
	 * The average horizontal distance from the left edge of one character
	 * to the left edge of the next. Contrast with max char increment.
	 * 
	 * baseline
	 * The line on which the bottom of a character rests, and below which
	 * a descender extends.
	 * 
	 * break char cade point
	 * The code point of the space or break character. Contrast with
	 * default char code point, first char code point, and last char code
	 * point.
	 * 
	 * character increment
	 * A set.of three values (a-space, b-space, and c-space) that define
	 * the proportions of a character. The sum of the three values (a+b+c)
	 * specifies only one value for the entire character increment.
	 * See also font width and space default values.
	 * 
	 * character rotation
	 * The angle by which each character is rotated around its own center,
	 * increasing clockwise from vertical. Contrast with character slope
	 * and inline direction.
	 * 
	 * character slope
	 * The angle by which a character is slanted, increasing clockwise
	 * from vertical. Contrast with character rotation and inline
	 * direction.
	 * 
	 * default char code point
	 * The code point of the character to be used if a code point outside
	 * the range of a font is passed to an application using that font.
	 * Contrast with break char code point, first char code point, and
	 * last char code point.
	 * 
	 * em height
	 * The maximum distance above the baseline reached by an uppercase
	 * symbol. Contrast with x height.
	 * 
	 * external leading
	 * The vertical distance from the bottom of one character to the top
	 * of the character below it. Contrast with internal leading and max
	 * baseline extent.
	 * 
	 * first char code point
	 * The code point of the first character. All numbers between the
	 * first char code point and the last char code point must represent
	 * a character in the font. Contrast with break char code point,
	 * default char code point, and last char code point.
	 * 
	 * fixed spacing
	 * The same amount of space separates each character. Contrast with
	 * proportional spacing.
	 * 
	 * font weight
	 * The line-thickness of a character relative to its size. Contrast
	 * with font width.
	 * 
	 * font width
	 * The relative width of a character to its height; condensed fonts
	 * are very narrow while expanded fonts are very wide. See also
	 * character increment. Contrast with font weight.
	 * 
	 * Inline direction
	 * The angle of a line of type, increasing clockwise from horizontal.
	 * Contrast with character rotation and character slope.
	 * 
	 * Internal leading
	 * The vertical distance from the top or bottom of a character to any
	 * accent marks that may appear with it. Contrast with external
	 * leading.
	 * 
	 * last char code point
	 * The code point of the last character. All numbers between the first
	 * char code point and the last char code point must represent a
	 * character in the font. Contrast with break char code point, default
	 * char code point, and first char code point.
	 * 
	 * lowercase ascent
	 * The maximum distance above the baseline reached by any part of any
	 * lowercase character. Contrast with maximum ascender and x height.
	 * 
	 * lowercase descent
	 * The maximum distance below the baseline reached by any part of any
	 * lowercase character. Contrast with maximum descender.
	 * 
	 * max baseline extent
	 * The maximum space occupied by the font (typically, the sum of the
	 * maximum ascender and maximum descender). Contrast with external
	 * leading and max char increment.
	 * 
	 * max char increment
	 * The maximum horizontal distance from the left edge of one character
	 * to the left edge of the next character to the right. Contrast with
	 * average char width and max baseline extent.
	 * 
	 * maximum ascender
	 * The maximum distance that any part of any character may extend
	 * above the x height of a font. Contrast with lowercase ascent and
	 * maximum descender.
	 * 
	 * maximum descender
	 * The maximum distance that any part of any character may extend
	 * below the x height of a font. Contrast with lowercase descent and
	 * maximum ascender.
	 * 
	 * maximum vert point size
	 * The maximum vertical dimensions to which a font can be resized.
	 * Contrast with minimum vert point size and nominal vert point size.
	 * 
	 * minimum vert point size
	 * The minimum vertical dimensions to which a font can be resized.
	 * Contrast with maximum vert point size and nominal vert point size.
	 * 
	 * nominal vert point size
	 * The normal display size of a font. Contrast with maximum vert point
	 * size and minimum vert point size.
	 * 
	 * pel
	 * The smallest element of a display surface that can be independently
	 * assigned color and density.
	 * 
	 * point
	 * Printer's unit of measurement. There are 72 points to an inch
	 * (approximately 3.5 points to a millimeter).
	 * 
	 * proportional spacing
	 * The space that each character occupies is in proportion to its
	 * width. See also font width, Contrast with fixed spacing.
	 * 
	 * Registry ID
	 * A code number that Presentation Manager uses to register a font
	 * file as a resource.
	 * 
	 * space default values
	 * Values that specify the space to be left between characters. Once
	 * defined, they are used for the entire font, and do not have to be
	 * specified for each character. However, they can be changed for
	 * characters that require more or less spacing than the defaults
	 * provide, by giving values for the a Space and the c Space. See also
	 * character increment.
	 * 
	 * strikeout position
	 * The distance of the strikeout character above the baseline (in
	 * pels). See also strikeout size and underscore position.
	 * 
	 * strikeout size
	 * The size of the strikeout character (in points). See also strikeout
	 * position and underscore size.
	 * 
	 * subscript position
	 * The distance of a subscript character of a font below the baseline
	 * (in pels). See also subscript size and superscript position.
	 * 
	 * subscript size
	 * The size of a subscript character (in points). See also subscript
	 * position and superscript size.
	 * 
	 * superscript position
	 * The distance of a superscript character above the baseline (in
	 * pels). See also subscript position and superscript size.
	 * 
	 * superscript size
	 * The size of a superscript character (in points). See also subscript
	 * size and superscript position.
	 * 
	 * target dev resolution X
	 * The number of pels per inch in the horizontal axis of a display
	 * device on which a font is to be displayed. Contrast with target dev
	 * resolution Y.
	 * 
	 * target dev resolution Y
	 * The number of pels per inch in the vertical axis of a display
	 * device on which a font is to be displayed. Contrast with target dev
	 * resolution X.
	 * 
	 * underscore position
	 * The distance in pels of the first underscore stroke from the
	 * baseline of a font. Successive strokes below this create a heavier
	 * underscore. See also strikeout position and underscore size.
	 * 
	 * underscore size
	 * The size of the underscore character measured in single strikeout
	 * strokes. See also strikeout size and underscore position.
	 * 
	 * x height
	 * The maximum distance above the baseline reached by a lowercase
	 * character. Contrast with em height and lowercase ascent.
	 * 
	 * cell height
	 * ----------  <------------------------------
	 * |        |           |- Internal Leading  |
	 * | |   |  |  <---------                    |
	 * | |   |  |        |                       |- Cell Height
	 * | |---|  |        |- Character Height     |
	 * | |   |  |        |  or em height         |
	 * | |   |  |        |                       |
	 * ----------  <------------------------------
	 */

  #endregion
  public class WinDrawingContext : WinContextBase, IGraphicDevice
	{


    public WinDrawingContext(Graphics handle)
      : base(handle)
		{
			

     _stringFormat = (StringFormat)StringFormat.GenericTypographic.Clone();
     _stringFormat.Alignment = StringAlignment.Near;
     _stringFormat.LineAlignment = StringAlignment.Near;

    
		}

    
    StringFormat _stringFormat;
   


	

		/**
		 * the current color (pen, text, brush)
		 */
		private Color currentColor;

		/**
		 * color of only current pen
		 */
		private Color currentPenColor;

		/**
		 * the current pen
		 */
		private Pen currentPen;

		/**
		 * the current solid pen
		 */
		private Pen solidPen;

	
	

		/**
		 * the current line style
		 */
    private LineStyle lineStyle = LineStyle.Solid;

    private Color currentBrushColor = Color.Black;
		private Brush currentBrush = Brushes.Black;
    private IFontHandle currentFont;
    

		/**
		 * static cach of solid brushes
		 */
		private static SolidBrushCache solidBrushCache = new SolidBrushCache();

		

		/**
		* Sets the given font as the curent active font.
		* @param font a native font resource returned from GetFont
		* @return The previous font. This font MUST BE RESTORED when the new
		* font is no longer used
		*/


		public IFontHandle SetFont(IFontHandle font)
		{
      IFontHandle result = currentFont;
      currentFont = font;
      
      return result;
		}

		/**
		* restores a font previously returned from SetFont
		*/
		public void RestoreFont(IFontHandle font)
		{
			// note, font resource are not destroyed here. font
			// resources are cached in the fonts list, and are 
			// destroyed when this object is told to clear them.

      currentFont = font;
     
     
    }

		/**
		 * destroy a native font resource
		 * note, this should only be called by the MathFont finalizer.
		 */
		public static void DestroyFont(Font font)
		{
      font.Dispose();
		}		

		/**
		 * get / set the foreground of all output of this device. This will set 
		 * the text / line / fill color
		 */
		public Color Color
		{
			get { return currentColor; }
			set 
			{ 
				currentColor = value; 
			}
		}

		/**
		 * get / set the line style
		 */
		public LineStyle LineStyle
		{
			get { return lineStyle; }
			set 
      {
        lineStyle = value; 
      }
		}			

	
   
   

		public void DrawGlyph(ushort index, float x, float y)
		{
      this.DrawString(x, y, string.Empty+(char)index);
		}


    
    public void DrawString(float x, float y, String s)
    {
      FontHandle fhandle = (FontHandle)currentFont;
      _graphics.DrawString(s, fhandle.Handle, currentBrush, x, (y-fhandle.Baseline), _stringFormat);
    }


		public void DrawFilledRectangle(float top, float left, float right, float bottom)
		{
      _graphics.FillRectangle(currentBrush, left, top, right - left, bottom - top);
		}

		/**
		 * Draw a set of lines between each pair of points in the given array.
		 * The array must be a multiple of 2, as every 2 points starting at 0 
		 * form the end points of a line. 
		 * 
		 * All lines will be single pixel wide
		 */
		public void DrawLines(PointF[] points)
		{
		
			if (LineStyle.None == lineStyle)
				return;


			//mTranMatrix->TransformCoord(&aX0,&aY0);
			//mTranMatrix->TransformCoord(&aX1,&aY1);
 
			SetupPen();

      _graphics.DrawLines(currentPen, points);
		}

		/**
				 * Draw a set of lines between each pair of points in the given array.
				 * The array must be a multiple of 2, as every 2 points starting at 0 
				 * form the end points of a line. 
				 * 
				 * All lines will be single pixel wide
				 */
		public void DrawLine(PointF from, PointF to)
		{

			if (LineStyle.None == lineStyle)
				return;
 
			SetupPen();

      _graphics.DrawLine(currentPen, from, to);

 
	
		}

	

		/**
		 * setup a solid pen using the current color.
		 * from mozilla
		 */
		private Pen SetupSolidPen()
		{
      if ((currentColor != currentPenColor) || (null == currentPen) || (currentPen != solidPen))
      {
        solidPen = currentPen = new Pen(currentColor);
        currentPenColor = currentColor;
      }
			return currentPen;
		}
 
		
		private Pen SetupPen()
		{
      
      if(lineStyle == LineStyle.Solid)
      {
        SetupSolidPen();
        currentPen.DashStyle = DashStyle.Solid;
        return currentPen;
      }
      else if (lineStyle == LineStyle.Dashed)
      {
        SetupSolidPen();
        currentPen.DashStyle = DashStyle.Dash;
        return currentPen;
      }
      else
      {
        return null;
      }
		} 
    

		private Brush SetupSolidBrush()
		{
			if ((currentColor != currentBrushColor) || (null == currentBrush))
			{
				Brush tbrush = solidBrushCache.GetSolidBrush(this, currentColor);
 
				currentBrush = tbrush;
				currentBrushColor = currentColor;
			}
 
			return currentBrush;
		}

		/**
		 * Small cache of HBRUSH objects
		 * Note: the current assumption is that there is only one UI thread so
		 * we do not lock, and we do not use TLS
		 */ 
		private class SolidBrushCache 
		{
			const int  BRUSH_CACHE_SIZE = 17;  // 2 stock plus 15
 
			private struct CacheEntry 
			{
				public Brush   mBrush;
				public Color mBrushColor;
			};
 
			CacheEntry[]  mCache = new CacheEntry[BRUSH_CACHE_SIZE];
			int         mIndexOldest = 2;  // index of oldest entry in cache

			public SolidBrushCache()
			{
				// First two entries are stock objects
        mCache[0].mBrush = Brushes.White;
				mCache[0].mBrushColor = Color.White;
        mCache[1].mBrush = Brushes.Black;
				mCache[1].mBrushColor = Color.Black;
			}
 
		
			 
			public Brush GetSolidBrush(WinDrawingContext theHDC, Color aColor)
			{
				int     i;
        Brush result = null;
   
				// See if it's already in the cache
				for (i = 0; (i < BRUSH_CACHE_SIZE) && mCache[i].mBrush != null; i++) 
				{
					if (mCache[i].mBrush != null && (mCache[i].mBrushColor == aColor)) 
					{
						// Found an existing brush
						result = mCache[i].mBrush;
						theHDC.currentBrush = result;
						break;
					}
				}
 
				if (result == null) 
				{
					// We didn't find it in the set of existing brushes, so create a
					// new brush
          result = new SolidBrush(aColor);
          theHDC.currentBrush = result;
 
					
 
					// If there's an empty slot in the cache, then just add it there
					if (i >= BRUSH_CACHE_SIZE) 
					{
						// Nope. The cache is full so we need to replace the oldest entry
						// in the cache
						
						i = mIndexOldest;
						if (++mIndexOldest >= BRUSH_CACHE_SIZE) 
						{
							mIndexOldest = 2;
						}
					}
 
					// Add the new entry
					mCache[i].mBrush = result;
					mCache[i].mBrushColor = aColor;
				}
				return result;
			}
		}
	}
}
