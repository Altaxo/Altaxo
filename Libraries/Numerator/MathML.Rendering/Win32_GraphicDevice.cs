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


namespace MathML.Rendering
{
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
	internal class GraphicDevice
	{
		public GraphicDevice(IntPtr handle)
		{
			dc = handle;
		}

		/**
		 * wrap a win32 device context, and dispose of it when it is no
		 * longer needed.
		 */
		private class MeasurmentContext
		{
			/**
			 * the HDC
			 */
			public readonly IntPtr Handle;

			/**
			 * this is used only for measuremnt, so just make it like the 
			 * desktop DC
			 */
			public MeasurmentContext()
			{
				IntPtr screenDc = Win32.GetDC(IntPtr.Zero);
				Handle = Win32.CreateCompatibleDC(screenDc);
				Win32.ReleaseDC(IntPtr.Zero, screenDc);
				Debug.WriteLine(String.Format("Created measurment context, handle: {0}", Handle));
			}

			/**
			 * done with the dc, so destroy it
			 */
			~MeasurmentContext()
			{
				Win32.DeleteDC(Handle);
				Debug.WriteLine("Destroyed measurment context");
			}
		}

		/**
		 * hold onto a context, only for measuring glyph metrics.
		 */
		private static MeasurmentContext measurmentContext = new MeasurmentContext();

		/**
		 * The native device context (in the win32 version anyway, in a 
		 * future gtk version, this will be whatever the gtk equivilant
		 * to a device context is. This should be set before
		 * any calls are mede into the formating / layout trees.
		 * 
		 * TODO this needs major cleanup, using this as 
		 * a static var is just asking for trouble.
		 */
		public IntPtr dc = IntPtr.Zero;

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
		private IntPtr currentPen;

		/**
		 * the current solid pen
		 */
		private IntPtr solidPen;

		/**
		 * the current dashed pen
		 */
		private IntPtr dashedPen;

		/**
		 * current dotted pen
		 */
		private IntPtr dottedPen;

		/**
		 * the current line style
		 */
		private LineStyle lineStyle;

		private Color currentBrushColor;
		private IntPtr currentBrush;

		/**
		 * static cach of solid brushes
		 */
		private static SolidBrushCache solidBrushCache = new SolidBrushCache();

		/**
		 * get the dpi resolution of the current device
		 */
		public static Scaled Dpi(FormattingContext context)
		{
			// TODO is this right?
			return 72.0f;
		}

		/**
		 * the height of the character "x" in the current 
		 * font size
		 */
		public static int Ex(FormattingContext context) 
		{

			BoundingBox size;
			float left, right;
			FontHandle font = FontFactory.GetFont(context);

			if(MeasureGlyph(font.Handle, 'x', out size, out left, out right)) 
			{
				return (int)size.Height;
			}
			else 
			{
				throw new Exception("MeasureGlyph failed");
			}				
		}

		/**
		 * the centerline of the current font, this is not 
		 * the baseline, but where the 2 lines cross in an 'x'
		 */
		public static Scaled Axis(FormattingContext context) 
		{
			return Ex(context) / 2.0f;
		}


		public static Scaled DefaultLineThickness(FormattingContext context) 
		{
			// should be at least 1 px thick
			//return Math.Max(context.m
			return 2.0f;
		}

		/**
		* Sets the given font as the curent active font.
		* @param font a native font resource returned from GetFont
		* @return The previous font. This font MUST BE RESTORED when the new
		* font is no longer used
		*/
		public IntPtr SetFont(IntPtr font)
		{
			return Win32.SelectObject(dc, font);
		}

		/**
		* restores a font previously returned from SetFont
		*/
		public void RestoreFont(IntPtr font)
		{
			// note, font resource are not destroyed here. font
			// resources are cached in the fonts list, and are 
			// destroyed when this object is told to clear them.

			Win32.SelectObject(dc, font);
		}

		/**
		 * destroy a native font resource
		 * note, this should only be called by the MathFont finalizer.
		 */
		public static void DestroyFont(IntPtr font)
		{
			Win32.DeleteObject(font);
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
				Win32.SetTextColor(dc, Win32.RGB(value));
			}
		}

		/**
		 * get / set the line style
		 */
		public LineStyle LineStyle
		{
			get { return lineStyle; }
			set { lineStyle = value; }
		}			

		/**
		 * calculate how many pixels per point.
		 */
		public static int PointsToPixels(float points)
		{
			// 72 points per inch
			return (int)((float)Win32.GetDeviceCaps(measurmentContext.Handle, Win32.LOGPIXELSY) * (points  / 72.0f));
		}

		/**
		 * calculate how many pixels per mm.
		 */
		public static int MMsToPixels(float mms)
		{
			// 25.4 mm 
			return (int)((float)Win32.GetDeviceCaps(measurmentContext.Handle, Win32.LOGPIXELSY) * (mms / 25.4));
		}

		/**
		 * calculate how many pixels per cm.
		 */
		public static int CMsToPixels(float cms)
		{
			return (int)((float)Win32.GetDeviceCaps(measurmentContext.Handle, Win32.LOGPIXELSY) * (cms / 2.54));
		}

		/**
		 * calculate how many pixels per pica.
		 */
		public static int PicasToPixels(float picas)
		{
			return (int)((float)Win32.GetDeviceCaps(measurmentContext.Handle, Win32.LOGPIXELSY) * (picas / 12));
		}

		/**
		 * calculate how many pixels per inch.
		 */
		public static int InchesToPixels(float inches)
		{
			return (int)((inches * Win32.GetDeviceCaps(measurmentContext.Handle, Win32.LOGPIXELSY)));
		}

		/**
		 * Create a native font resource. This MUST be explicitly deleted when it is 
		 * no longer needed by calling DestroyFont.
		 * 
		 * @param emHeight the desired character height of the font, this is 
		 * the size above the baseline for a capital M, or the largest
		 * character height in the font		 
		 * @param italic create an italic font 
		 * @param weight the weight of the font
		 * @param fontName the face name of the font
		 */
		public static FontHandle CreateFont(int emHeight, bool italic, int weight, String fontName)
		{			
			Win32.LOGFONT lf = new Win32.LOGFONT();

			lf.lfHeight = -emHeight;
			lf.lfWidth = 0;
			lf.lfEscapement = 0;
			lf.lfOrientation = 0;
			lf.lfWeight = weight;	
			lf.lfItalic = italic ? (byte)1 : (byte)0;
			lf.lfUnderline = 0;
			lf.lfStrikeOut = 0;
			lf.lfCharSet = Win32.DEFAULT_CHARSET;
			lf.lfOutPrecision = Win32.OUT_OUTLINE_PRECIS;
			lf.lfClipPrecision = Win32.CLIP_DEFAULT_PRECIS;
			lf.lfQuality = Win32.PROOF_QUALITY;
			lf.lfPitchAndFamily = Win32.FF_DONTCARE;
			lf.lfFaceName = fontName;

			return new FontHandle(Win32.CreateFontIndirect(ref lf), emHeight, italic, weight, fontName);
		}

		/**
		 * get the glyph metrics of a glyph.
		 * in this case, the returned bounding box is the box that the 
		 * glyph metrics say fully encapsulate the glyph, including the 
		 * space on the left and right sides of the glyph. So, the width 
		 * of the box is the actual amout to advance the x position to
		 * draw the next glyph.
		 * 
		 * note, this method is not extremly efficient, in the future, we
		 * may store glyph metrics with a glyph, or get a whole load of these
		 * in one block, and store them somewhere. The code needs to be 
		 * profiled, and we will see if this method is a bottle neck.
		 * 
		 * @param font A handle to a native font.
		 * @param The char index (the actual character value)
		 * @param box The returned bounding box of the glyph
		 * @param left The distance from the origin the left colored edge of 
		 *             the glyph.
		 * @param right same as left.
		 */
		public static bool MeasureGlyph(IntPtr font, ushort index, out BoundingBox box, 
			out Scaled left, out Scaled right)
		{
			bool retval = false;
			Win32.GLYPHMETRICS gm = new Win32.GLYPHMETRICS();
			Win32.MAT2 mat = new Win32.MAT2();

			// save the old font
			IntPtr savedFont = Win32.SelectObject(measurmentContext.Handle, font);
			
			// need to set the matrix to the identity matrix.
			// note, the sdk documentation does not say anything about 
			// this, but this call WILL NOT WORK without this matrix
			// set to identity, this can be verified on the internet, 
			// and by testing.
			mat.eM11.val = 1;
			mat.eM22.val = 1;

			if(Win32.GetGlyphOutline(measurmentContext.Handle, index, Win32.GGO_METRICS, ref gm, 0, 
				IntPtr.Zero, ref mat) > 0)
			{
				// the total cell advance
				box.Width = gm.gmCellIncX;

				// the height above the baseline
				box.Height = gm.gmptGlyphOrigin.y;

				// the total height - height above baseline
				box.Depth = gm.gmBlackBoxY - gm.gmptGlyphOrigin.y;

				// distance from cell origin to left edge of glyph
				left = gm.gmptGlyphOrigin.x;

				// the space on the right edge of the glyph.
				// get this by taking the total advance, sub bouding box, 
				// and origin.
				right = gm.gmCellIncX - gm.gmBlackBoxX - gm.gmptGlyphOrigin.x;

				// all was ok
				retval = true;
			}
			else
			{
				box = BoundingBox.New();
				left = right = 0;
			}

			// restore the font
			Win32.SelectObject(measurmentContext.Handle, savedFont);

			return retval;
		}

		public void DrawGlyph(ushort index, float x, float y)
		{
			Char[] buffer = {(char)index};
			Win32.TextOut(dc, (int)x, (int)y, buffer, 1);
		}

		public void DrawFilledRectangle(float top, float left, float right, float bottom)
		{
			// play around with int conversion so that we do not get rectangles that are too small
			// when dealing with float values that are close together
			int width = (int)Math.Ceiling(right - left);
			int height = (int)Math.Ceiling(bottom - top);
			int l = (int)left;
			int t = (int)top;
			Win32.RECT rect;

			rect.left = l;
			rect.top = t;
			rect.right = l + width;
			rect.bottom = t + height;

			Win32.FillRect(dc, ref rect, SetupSolidBrush());
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
			Win32.POINT p = new Win32.POINT();

			if (LineStyle.None == lineStyle)
				return;
 
			//mTranMatrix->TransformCoord(&aX0,&aY0);
			//mTranMatrix->TransformCoord(&aX1,&aY1);
 
			SetupPen();

			for(int i = 0; i < points.Length; i++)
			{
				Win32.MoveToEx(dc, (int)points[i].X, (int)points[i].Y, ref p);
				i++;
				Win32.LineTo(dc, (int)points[i].X, (int)points[i].Y);
			}
 
//			if (nsLineStyle_kDotted == mCurrLineStyle)
//			{
//				lineddastruct dda_struct;
// 
//				dda_struct.nDottedPixel = 1;
//				dda_struct.dc = mDC;
//				dda_struct.crColor = mColor;
// 
//				LineDDA((int)(aX0),(int)(aY0),(int)(aX1),(int)(aY1),(LINEDDAPROC) LineDDAFunc,(long)&dda_struct);
//			}
//			else
//			{
//				Win32.MoveToEx(mDC, (int)(aX0), (int)(aY0), NULL);
//				Win32.LineTo(mDC, (int)(aX1), (int)(aY1));
//			}
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
			Win32.POINT p = new Win32.POINT();

			if (LineStyle.None == lineStyle)
				return;
 
			//mTranMatrix->TransformCoord(&aX0,&aY0);
			//mTranMatrix->TransformCoord(&aX1,&aY1);
 
			SetupPen();

			Win32.MoveToEx(dc, (int)from.X, (int)from.Y, ref p);
			Win32.LineTo(dc, (int)to.X, (int)to.Y);

 
			//			if (nsLineStyle_kDotted == mCurrLineStyle)
			//			{
			//				lineddastruct dda_struct;
			// 
			//				dda_struct.nDottedPixel = 1;
			//				dda_struct.dc = mDC;
			//				dda_struct.crColor = mColor;
			// 
			//				LineDDA((int)(aX0),(int)(aY0),(int)(aX1),(int)(aY1),(LINEDDAPROC) LineDDAFunc,(long)&dda_struct);
			//			}
			//			else
			//			{
			//				Win32.MoveToEx(mDC, (int)(aX0), (int)(aY0), NULL);
			//				Win32.LineTo(mDC, (int)(aX1), (int)(aY1));
			//			}
		}

		public void DrawString(float x, float y, String s)
		{
			Win32.TextOut(dc, (int)x, (int)y, s, s.Length);
		}

		/**
		 * setup a solid pen using the current color.
		 * from mozilla
		 */
		private IntPtr SetupSolidPen()
		{
			if ((currentColor != currentPenColor) || (IntPtr.Zero == currentPen) || (currentPen != solidPen))
			{
				IntPtr  tpen;
      
				if (Color.Black == currentColor) 
				{
					tpen = Win32.StockBlackPen;
				} 
				else if (Color.White == currentColor) 
				{
					tpen = Win32.StockWhitePen;
				} 
				else 
				{
					tpen = Win32.CreatePen(Win32.PS_SOLID, 0, Win32.RGB(currentColor));
				}
 
				Win32.SelectObject(dc, tpen);
 
				if (currentPen != IntPtr.Zero && (currentPen != Win32.StockBlackPen) && (currentPen != Win32.StockWhitePen)) 
				{
					Win32.DeleteObject(currentPen);
				}
 
				solidPen = currentPen = tpen;
				currentPenColor = currentColor;
			}
 
			return currentPen;
		}
 
		private IntPtr SetupDashedPen()
		{
			if ((currentColor != currentPenColor) || (IntPtr.Zero == currentPen) || (currentPen != dashedPen))
			{
				IntPtr  tpen = Win32.CreatePen(Win32.PS_DOT, 0, Win32.RGB(currentColor));
 
				Win32.SelectObject(dc, tpen);
 
				if (IntPtr.Zero != currentPen)
				{
					Win32.DeleteObject(currentPen);		
				}

				dashedPen = currentPen = tpen;
				currentPenColor = currentColor;
			} 
			return currentPen;
		}
 
		private IntPtr SetupDottedPen()
		{
			if ((currentColor != currentPenColor) || (IntPtr.Zero == currentPen) || (currentPen != dottedPen))
			{
				IntPtr  tpen = Win32.CreatePen(Win32.PS_DOT, 0, Win32.RGB(currentColor));
 
				Win32.SelectObject(dc, tpen);
 
				if (IntPtr.Zero != currentPen)
				{
					Win32.DeleteObject(currentPen);
				}
 
				dottedPen = currentPen = tpen;
				currentPenColor = currentColor;
			} 
			return currentPen;
		}

		private IntPtr SetupPen()
		{
			IntPtr pen;
 
			switch(lineStyle)
			{
				case LineStyle.Solid:
					pen = SetupSolidPen();
					break;
 
				case LineStyle.Dashed:
					pen = SetupDashedPen();
					break;
 
				//case LineStyle.Dotted:
				//	pen = SetupDottedPen();
				//	break;
 
				case LineStyle.None:
					pen = IntPtr.Zero;
					break;
 
				default:
					pen = SetupSolidPen();
					break;
			} 
			return pen;
		} 

		private IntPtr SetupSolidBrush()
		{
			if ((currentColor != currentBrushColor) || (IntPtr.Zero == currentBrush))
			{
				IntPtr tbrush = solidBrushCache.GetSolidBrush(dc, currentColor);
 
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
				public IntPtr   mBrush;
				public Color mBrushColor;
			};
 
			CacheEntry[]  mCache = new CacheEntry[BRUSH_CACHE_SIZE];
			int         mIndexOldest = 2;  // index of oldest entry in cache

			public SolidBrushCache()
			{
				// First two entries are stock objects
				mCache[0].mBrush = Win32.GetStockObject(Win32.WHITE_BRUSH);
				mCache[0].mBrushColor = Color.White;
				mCache[1].mBrush = Win32.GetStockObject(Win32.BLACK_BRUSH);
				mCache[1].mBrushColor = Color.Black;
			}
 
			~SolidBrushCache()
			{
				Debug.WriteLine("Destroying solid brush cache");
				// No need to delete the stock objects
				for (int i = 2; i < mCache.Length; i++) 
				{
					if (mCache[i].mBrush != IntPtr.Zero) 
					{
						Debug.WriteLine("Destroying native brush...");
						Win32.DeleteObject(mCache[i].mBrush);
					}
				}
			}
 
			public IntPtr GetSolidBrush(IntPtr theHDC, Color aColor)
			{
				int     i;
				IntPtr  result = IntPtr.Zero;
   
				// See if it's already in the cache
				for (i = 0; (i < BRUSH_CACHE_SIZE) && mCache[i].mBrush != IntPtr.Zero; i++) 
				{
					if (mCache[i].mBrush != IntPtr.Zero && (mCache[i].mBrushColor == aColor)) 
					{
						// Found an existing brush
						result = mCache[i].mBrush;
						Win32.SelectObject(theHDC, result);
						break;
					}
				}
 
				if (result == IntPtr.Zero) 
				{
					// We didn't find it in the set of existing brushes, so create a
					// new brush
					result = Win32.CreateSolidBrush(Win32.RGB(aColor));
 
					// Select the brush.  NOTE: we want to select the new brush before
					// deleting the old brush to prevent any win98 GDI leaks (bug 159298)
					Win32.SelectObject(theHDC, result);
 
					// If there's an empty slot in the cache, then just add it there
					if (i >= BRUSH_CACHE_SIZE) 
					{
						// Nope. The cache is full so we need to replace the oldest entry
						// in the cache
						Win32.DeleteObject(mCache[mIndexOldest].mBrush);
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
