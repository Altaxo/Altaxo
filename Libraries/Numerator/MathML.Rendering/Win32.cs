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
using System.Runtime.InteropServices;
using System.Drawing;

namespace MathML.Rendering
{
	/**
	 * direct access to win32 api methods
	 * 
	 * This is a copy of the Win32.cs file from the font tester. 
	 * This class has way more methods than what is used or needed
	 * by the mathml renderer, and will be trimmed down to the bare
	 * essentials sometime in the future.
	 */
	internal class Win32
	{
		public const uint GDI_ERROR = 0xffffffff;

		[StructLayout(LayoutKind.Sequential, CharSet=CharSet.Auto)]
		public struct ABC
		{
			public int     abcA; 
			public uint    abcB; 
			public int     abcC; 
		}

		/**
		 * The ABCFLOAT structure contains the A, B, and C widths 
		 * of a font character. 
		 */
		[StructLayout(LayoutKind.Sequential, CharSet=CharSet.Auto)]
		public struct ABCFLOAT 
		{
			public float   abcfA; 
			public float   abcfB; 
			public float   abcfC; 
		} 


		/**
		 * The RECT structure defines the coordinates of the upper-left 
		 * and lower-right corners of a rectangle. 
		 */
		[StructLayout(LayoutKind.Sequential, CharSet=CharSet.Auto)]
			public struct RECT 
		{
			public int left; 
			public int top; 
			public int right; 
			public int bottom; 
		}

		[StructLayout(LayoutKind.Sequential, CharSet=CharSet.Auto)]
			public struct LOGFONT 
		{ 
			public const int LF_FACESIZE = 32;
			public int lfHeight; 
			public int lfWidth; 
			public int lfEscapement; 
			public int lfOrientation; 
			public int lfWeight; 
			public byte lfItalic; 
			public byte lfUnderline; 
			public byte lfStrikeOut; 
			public byte lfCharSet; 
			public byte lfOutPrecision; 
			public byte lfClipPrecision; 
			public byte lfQuality; 
			public byte lfPitchAndFamily;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst=LF_FACESIZE)]
			public string lfFaceName; 
		}

		public const byte  OUT_DEFAULT_PRECIS =         0;
		public const byte  OUT_STRING_PRECIS  =         1;
		public const byte  OUT_CHARACTER_PRECIS =       2;
		public const byte  OUT_STROKE_PRECIS =          3;
		public const byte  OUT_TT_PRECIS  =             4;
		public const byte  OUT_DEVICE_PRECIS   =        5;
		public const byte  OUT_RASTER_PRECIS  =         6;
		public const byte  OUT_TT_ONLY_PRECIS  =        7;
		public const byte  OUT_OUTLINE_PRECIS =         8;
		public const byte  OUT_SCREEN_OUTLINE_PRECIS =  9;
		public const byte  OUT_PS_ONLY_PRECIS  =        10;

		public const byte  CLIP_DEFAULT_PRECIS =    0;
		public const byte  CLIP_CHARACTER_PRECIS  = 1;
		public const byte  CLIP_STROKE_PRECIS    =  2;
		public const byte  CLIP_MASK    =           0xf;
		public const byte  CLIP_LH_ANGLES       =   (1<<4);
		public const byte  CLIP_TT_ALWAYS        =  (2<<4);
		public const byte  CLIP_EMBEDDED        =   (8<<4);

		public const byte  DEFAULT_QUALITY    =     0;
		public const byte  DRAFT_QUALITY        =   1;
		public const byte  PROOF_QUALITY         =  2;

		public const byte  NONANTIALIASED_QUALITY = 3;
		public const byte  ANTIALIASED_QUALITY  =   4;
		public const byte  CLEARTYPE_QUALITY   =    5;
		public const byte  DEFAULT_PITCH =          0;
		public const byte  FIXED_PITCH   =          1;
		public const byte  VARIABLE_PITCH   =       2;
		public const byte  MONO_FONT     =          8;
		public const byte  ANSI_CHARSET    =        0;
		public const byte  DEFAULT_CHARSET   =      1;
		public const byte  SYMBOL_CHARSET =         2;
		public const byte  SHIFTJIS_CHARSET  =      128;
		public const byte  HANGEUL_CHARSET =        129;
		public const byte  HANGUL_CHARSET   =       129;
		public const byte  GB2312_CHARSET =         134;
		public const byte  CHINESEBIG5_CHARSET  =   136;
		public const byte  OEM_CHARSET     =        255;
		public const byte  JOHAB_CHARSET   =        130;
		public const byte  HEBREW_CHARSET    =      177;
		public const byte  ARABIC_CHARSET =         178;
		public const byte  GREEK_CHARSET =          161;
		public const byte  TURKISH_CHARSET    =     162;
		public const byte  VIETNAMESE_CHARSET =     163;
		public const byte  THAI_CHARSET   =         222;
		public const byte  EASTEUROPE_CHARSET  =    238;
		public const byte  RUSSIAN_CHARSET  =       204;
		public const byte  MAC_CHARSET  =           77;
		public const byte  BALTIC_CHARSET    =      186;

		public const uint  FS_LATIN1  =             0x00000001;
		public const uint  FS_LATIN2  =             0x00000002;
		public const uint  FS_CYRILLIC  =           0x00000004;
		public const uint  FS_GREEK  =              0x00000008;
		public const uint  FS_TURKISH   =           0x00000010;
		public const uint  FS_HEBREW =              0x00000020;
		public const uint  FS_ARABIC  =             0x00000040;
		public const uint  FS_BALTIC   =            0x00000080;
		public const uint  FS_VIETNAMESE  =         0x00000100;
		public const uint  FS_THAI       =          0x00010000;
		public const uint  FS_JISJAPAN    =         0x00020000;
		public const uint  FS_CHINESESIMP   =       0x00040000;
		public const uint  FS_WANSUNG      =        0x00080000;
		public const uint  FS_CHINESETRAD   =       0x00100000;
		public const uint  FS_JOHAB     =           0x00200000;
		public const uint  FS_SYMBOL     =          0x80000000;

		/* Font Families */
		public const byte  FF_DONTCARE  =       (0<<4);  /* Don't care or don't know. */
		public const byte  FF_ROMAN        =    (1<<4);  /* Variable stroke width, serifed. */
		/* Times Roman, Century Schoolbook, etc. */
		public const byte  FF_SWISS   =         (2<<4);  /* Variable stroke width, sans-serifed. */
		/* Helvetica, Swiss, etc. */
		public const byte  FF_MODERN       =    (3<<4);  /* Constant stroke width, serifed or sans-serifed. */
		/* Pica, Elite, Courier, etc. */
		public const byte  FF_SCRIPT     =      (4<<4);  /* Cursive, etc. */
		public const byte  FF_DECORATIVE   =    (5<<4);  /* Old English, etc. */

		/* Font Weights */
		public const int  FW_DONTCARE    =     0;
		public const int  FW_THIN        =     100;
		public const int  FW_EXTRALIGHT =      200;
		public const int  FW_LIGHT   =         300;
		public const int  FW_NORMAL =          400;
		public const int  FW_MEDIUM =          500;
		public const int  FW_SEMIBOLD  =       600;
		public const int  FW_BOLD  =           700;
		public const int  FW_EXTRABOLD  =      800;
		public const int  FW_HEAVY =           900;

		public const int  FW_ULTRALIGHT  =     FW_EXTRALIGHT;
		public const int  FW_REGULAR=          FW_NORMAL;
		public const int  FW_DEMIBOLD   =      FW_SEMIBOLD;
		public const int  FW_ULTRABOLD  =      FW_EXTRABOLD;
		public const int  FW_BLACK  =          FW_HEAVY;



		/* Pen Styles */
		public const int PS_SOLID =            0;
		public const int PS_DASH =             1;       /* -------  */
		public const int PS_DOT =              2;       /* .......  */
		public const int PS_DASHDOT =          3;       /* _._._._  */
		public const int PS_DASHDOTDOT =       4;       /* _.._.._  */
		public const int PS_NULL =             5;
		public const int PS_INSIDEFRAME =      6;
		public const int PS_USERSTYLE =        7;
		public const int PS_ALTERNATE =        8;
		public const int PS_STYLE_MASK =       0x0000000F;

		public const int PS_ENDCAP_ROUND =     0x00000000;
		public const int PS_ENDCAP_SQUARE =    0x00000100;
		public const int PS_ENDCAP_FLAT =      0x00000200;
		public const int PS_ENDCAP_MASK =      0x00000F00;

		public const int PS_JOIN_ROUND =       0x00000000;
		public const int PS_JOIN_BEVEL =       0x00001000;
		public const int PS_JOIN_MITER =       0x00002000;
		public const int PS_JOIN_MASK =        0x0000F000;

		public const int PS_COSMETIC =         0x00000000;
		public const int PS_GEOMETRIC =        0x00010000;
		public const int PS_TYPE_MASK =        0x000F0000;

		/* Stock Logical Objects */
		public const int WHITE_BRUSH =         0;
		public const int LTGRAY_BRUSH =        1;
		public const int GRAY_BRUSH =          2;
		public const int DKGRAY_BRUSH =        3;
		public const int BLACK_BRUSH =         4;
		public const int NULL_BRUSH =          5;
		public const int HOLLOW_BRUSH =        NULL_BRUSH;
		public const int WHITE_PEN =           6;
		public const int BLACK_PEN =           7;
		public const int NULL_PEN =            8;
		public const int OEM_FIXED_FONT =      10;
		public const int ANSI_FIXED_FONT =     11;
		public const int ANSI_VAR_FONT =       12;
		public const int SYSTEM_FONT =         13;
		public const int DEVICE_DEFAULT_FONT = 14;
		public const int DEFAULT_PALETTE =     15;
		public const int SYSTEM_FIXED_FONT =   16;

		/**
		 * The POINT structure defines the x- and y- coordinates of a point. 
		 */
		[StructLayout(LayoutKind.Sequential, CharSet=CharSet.Auto)]
			public struct POINT 
		{
			public int x; 
			public int y;
		}

		/**
		 * The SIZE structure specifies the width and height of a rectangle. 
		 */
		[StructLayout(LayoutKind.Sequential, CharSet=CharSet.Auto)]
			public struct SIZE
		{
			public int cx; 
			public int cy; 
		}

		/**
		 * The GLYPHMETRICS structure contains information about the placement 
		 * and orientation of a glyph in a character cell. 
		 */
		[StructLayout(LayoutKind.Sequential, CharSet=CharSet.Auto)]
			public struct GLYPHMETRICS 
		{
			public uint  gmBlackBoxX; 
			public uint  gmBlackBoxY; 
			public POINT gmptGlyphOrigin; 
			public short gmCellIncX; 
			public short gmCellIncY; 
		};

		/**
		 * layed out correctly, fract is before val, even though
		 * the header file has them the other way. tested sep 22 2003, 
		 * and verified the order of the fields. If the fields are 
		 * reversed, GetGlyphOutline fails
		 */
		[StructLayout(LayoutKind.Sequential, CharSet=CharSet.Auto)]
		public struct FIXED
		{
			public ushort fract;
			public short  val;	
		}

		/**
		 * The MAT2 structure contains the values for a transformation matrix 
		 * used by the GetGlyphOutline function. 
		 */
		[StructLayout(LayoutKind.Sequential)]
		public struct MAT2 
		{
			public FIXED eM11; 
			public FIXED eM12; 
			public FIXED eM21; 
			public FIXED eM22; 
		} 

		/**
		 * The TEXTMETRIC structure contains basic information about a physical font. 
		 * All sizes are specified in logical units; that is, they depend on the 
		 * current mapping mode of the display context. 
		 */
		[StructLayout(LayoutKind.Sequential, CharSet=CharSet.Auto)]
			public struct TEXTMETRIC 
		{ 
			public int tmHeight; 
			public int tmAscent; 
			public int tmDescent; 
			public int tmInternalLeading; 
			public int tmExternalLeading; 
			public int tmAveCharWidth; 
			public int tmMaxCharWidth; 
			public int tmWeight; 
			public int tmOverhang; 
			public int tmDigitizedAspectX; 
			public int tmDigitizedAspectY; 
			public char tmFirstChar; 
			public char tmLastChar; 
			public char tmDefaultChar; 
			public char tmBreakChar; 
			public byte tmItalic; 
			public byte tmUnderlined; 
			public byte tmStruckOut; 
			public byte tmPitchAndFamily; 
			public byte tmCharSet; 
		}

		[StructLayout(LayoutKind.Sequential, CharSet=CharSet.Auto)]
		public struct CHOOSEFONT
		{
			public uint		lStructSize;
			public IntPtr	hwndOwner;          // caller's window handle
			public IntPtr	hDC;                // printer DC/IC or NULL
			public IntPtr	lpLogFont;          // ptr. to a LOGFONT struct
			public int		iPointSize;         // 10 * size in points of selected font
			public uint		Flags;              // enum. type flags
			public uint		rgbColors;          // returned text color
			public uint		lCustData;          // data passed to hook fn.
			public IntPtr	lpfnHook;           // ptr. to hook function
			public string	lpTemplateName;     // custom template name
			public uint		hInstance;          // instance handle of.EXE that
			//   contains cust. dlg. template
			public string	lpszStyle;          // return the style field here
			// must be LF_FACESIZE or bigger
			public ushort	nFontType;          // same value reported to the EnumFonts
			//   call back with the extra FONTTYPE_
			//   bits added
			public ushort	___MISSING_ALIGNMENT__;
			public int		nSizeMin;           // minimum pt size allowed &
			public int		nSizeMax;           // max pt size allowed if
			//   CF_LIMITSIZE is used
		} 


		/**
		 * The WCRANGE structure specifies a range of Unicode characters.
		 */
		[StructLayout(LayoutKind.Sequential, CharSet=CharSet.Auto)]
		public struct WCRANGE 
		{
			public char  wcLow;
			public short cGlyphs;
		}

		/**
		 * The GLYPHSET structure contains information about a range of 
		 * Unicode code points.
		 */
//		[StructLayout(LayoutKind.Sequential, CharSet=CharSet.Auto)]
//		public struct GLYPHSET 
//		{
//			public uint		cbThis;
//			public uint		flAccel;
//			public uint		cGlyphsSupported;
//			public uint		cRanges;
//			//actually should be WCRANGE[1], but since this is inline
//			//this will work too, and is simpler
//			public WCRANGE*  ranges; 
//		}




		[DllImport("Gdi32.dll", CharSet=CharSet.Auto)]
		public static extern int GetCharABCWidths(
			IntPtr hdc,      // handle to DC
			uint uFirstChar, // first character in range
			uint uLastChar,  // last character in range
			[In,Out] ABC[] lpabc      // array of character widths
			);

		/**
		 * The CreateFontIndirect function creates a logical font that has the 
		 * specified characteristics. The font can subsequently be selected as 
		 * the current font for any device context. 
		 */
		[DllImport("Gdi32.dll", CharSet=CharSet.Auto)]
		public static extern IntPtr CreateFontIndirect(ref LOGFONT lplf);

		/**
		 * The CreateSolidBrush function creates a logical brush that has 
		 * the specified solid color. 
		 */
		[DllImport("Gdi32.dll", CharSet=CharSet.Auto)]
		public static extern IntPtr CreateSolidBrush(uint color);

		
		/**
		 * implementation of the standard win32 RGB macro
		 */
		public static uint RGB(byte r, byte g, byte b)
		{
			//#define RGB(r,g,b)((COLORREF)(((BYTE)(r)|((WORD)((BYTE)(g))<<8))|(((DWORD)(BYTE)(b))<<16)))
			//return ((uint)(((byte)(r)|((ushort)((byte)(g))<<8))|(((uint)(byte)(b))<<16)));
			return (uint)(r + (g << 8) + (b << 16));
		}

		/**
		 * shortcut for color obj
		 */
		public static uint RGB(Color c)
		{
      return (uint)(c.R + (c.G << 8) + (c.B << 16));
		}


		/**
		 * The CreatePen function creates a logical pen that has the specified style, 
		 * width, and color. The pen can subsequently be selected into a device context 
		 * and used to draw lines and curves. 
		 */
		[DllImport("Gdi32.dll", CharSet=CharSet.Auto)]
		public static extern IntPtr CreatePen(
			int fnPenStyle,    // pen style
			int nWidth,        // pen width
			uint crColor       // pen color
			);


		/**
		 * The GetGlyphOutline function retrieves the outline or bitmap for a 
		 * character in the TrueType font that is selected into the specified 
		 * device context. 
		 * 
		 * Understanding what the documentation has to offer is only half the problem. 
		 * When the good doctor reviewed the documentation for GetGlyphOutline, his heart was 
		 * broken to find that the docs had, well Â… degenerated into an outline. So, here we 
		 * go with "Everything You Always Wanted to Know About GetGlyphOutline but Were 
		 * Afraid—Very Afraid—to Ask."
		 * To remedy this, the good doctor prescribes the following general overview of how 
		 * to use the function to get TrueType glyph outlines, and suggests a number of Microsoft 
		 * Knowledge Base articles that directly discuss the native buffer format.
		 * 
		 * To retrieve the native TrueType outlines from GetGlyphOutline, you should call the 
		 * function twice using GGO_NATIVE for the uFormat parameter. For the first call to the 
		 * function, pass zero (0) and NULL for the buffer size and the pointer to the buffer, 
		 * respectively. The function then returns the required buffer size in bytes. Allocate the 
		 * buffer and call the function again with the appropriate parameters to retrieve the buffer 
		 * of outlines.
		 * 
		 * Please note that, contrary to the current documentation, the MAT2 parameter modifies 
		 * the result of the function call. So, if you want an unmodified outline, make sure to 
		 * pass a true identity matrix to this parameter. An identity matrix is a MAT2 structure 
		 * with a value of one (1) in the eM11 and eM22 members and a value set to zero (0) for 
		 * all other members.
		 * 
		 * Getting the outline is easy; decoding it is hard (and sometimes not unlike trying to 
		 * add some topical humor to a technical column). Here are some references and help that 
		 * can assist you in drawing an outline from the native data.
		 * 
		 * You need to find and understand the TTPOLYGONHEADER, TTPOLYCURVE, and POINTFX structure 
		 * documentation to understand the native buffer format. The documentation for these 
		 * structures is accurate, but is not referenced by the GetGlyphOutline documentation. 
		 * It also describes the native outline data structures in rather terse terms. To get a 
		 * better description of these structures and the GGO_NATIVE buffer format, review the 
		 * following Knowledge Base article, which contains sample code to parse the native buffer 
		 * and to draw the Quadratic B-Spline curves that it contains: Q243285: HOWTO: Draw TrueType 
		 * Glyph Outlines.
		 * 
		 * There is also a much older Knowledge Base article from the Windows 3.1 era, which also 
		 * describes the native buffer format: Q87115: HOWTO: GetGlyphOutline() Native Buffer Format.
		 * 
		 * If, for some reason, your code runs on 3.x versions of Windows, or on platforms other 
		 * than Windows where Bezier drawing functions may not be available, you might also be 
		 * interested in Knowledge Base article: Q135058: How to Draw Cubic Bezier Curves in Windows 
		 * and Win32s.
		 * 
		 * For a discussion of how Quadratic B-Spline curves are used in the contours of TrueType 
		 * outlines, see the TrueType Specification, revision 1.66. It's available on the MSDN CD, 
		 * or online at Microsoft Typography.
		 * 
		 * If you are working with Unicode on Microsoft Windows 95 or Microsoft Windows 98, you should 
		 * review the following Knowledge Base articles, which discuss ways of using Unicode with the 
		 * GetGlyphOutline function: Q241020: HOWTO: Translate Unicode Character Codes to TrueType Glyph 
		 * Indices in Windows 95, and Q241358: PRB: The GetGlyphOutlineW Function Fails on Windows 95 
		 * and Windows 98.
		 * 
		 * Also, the August 1999 issue of C/C++ Users Journal contains an introductory article on 
		 * TrueType Outlines titled "TrueType Font Secrets." It can give you a different and fresh 
		 * perspective on TrueType outlines.
		 * 
		 * This information should get you started drawing the outlines, so the good doctor will now do 
		 * a dramatic finish by correcting the rest of the outline documentation—or would that be 
		 * documentation outline?
		 * 
		 * Anyway, the GGO_GLYPH_INDEX flag, contrary to the current documentation, has nothing to do 
		 * with ABC character spacing. If this flag is passed, it means that the UINT uChar parameter 
		 * is a 16-bit TrueType glyph index rather than a character code. Glyph indices can be obtained 
		 * with the GetCharacterPlacement function or through the techniques described in the 
		 * Knowledge Base article Q241020 just mentioned. The GGO_NATIVE format flag does not return 
		 * data in design units unless, as the documentation goes on to describe, the font is realized 
		 * with a size equal to the otmEMSquare of the OUTLINETEXTMETRIC structure. Also, the rasterizer 
		 * is capable of applying any transformation expressed in the MAT2 structure, not just rotation. 
		 * 
		 * Finally, the GGO_GRAY8_BITMAP flag returns a bitmap whose byte values range from 0 to 64, 
		 * not 255 as the remarks section describes.
		 */
		[DllImport("Gdi32.dll", CharSet=CharSet.Auto)]
		public static extern uint GetGlyphOutline(
			IntPtr hdc,				// handle to DC
			uint uChar,				// character to query
			uint uFormat,			// data format
			ref GLYPHMETRICS lpgm,	// glyph metrics
			uint cbBuffer,			// size of data buffer
			IntPtr lpvBuffer,		// data buffer
			ref MAT2 lpmat2			// transformation matrix
			);

		
		/**
		 * Windows 2000/XP: The function retrieves the curve data as a cubic 
		 * Bézier spline (not in quadratic spline format). 
		 */
		public const uint GGO_BEZIER = 3; 
		 
		/**
		 * The function retrieves the glyph bitmap. For information about memory 
		 * allocation, see the following Remarks section. 
		 */
		public const uint GGO_BITMAP = 1;	
		
		/**
		 * Windows 95/98/Me, Windows NT 4.0 and later: Indicates that the uChar 
		 * parameter is a TrueType Glyph Index rather than a character code. 
		 * See the ExtTextOut function for additional remarks on Glyph Indexing. 		
		 */																												   
		public const uint GGO_GLYPH_INDEX = 0x0080;
		
		/**
		 * Windows 95/98/Me, Windows NT 4.0 and later: The function 
		 * retrieves a glyph bitmap that contains five levels of gray. 	
		 */																																																																																		  
		public const uint GGO_GRAY2_BITMAP = 4;
		
		/**
		 * Windows 95/98/Me, Windows NT 4.0 and later: The function retrieves a 
		 * glyph bitmap that contains 17 levels of gray. 
		 */
		public const uint GGO_GRAY4_BITMAP = 5;
		
		/**
		 * Windows 95/98/Me, Windows NT 4.0 and later: The function retrieves a glyph 
		 * bitmap that contains 65 levels of gray. 	
		 */																															  
		public const uint GGO_GRAY8_BITMAP = 6;
		
		/**
		 * The function only retrieves the GLYPHMETRICS structure specified by lpgm. 
		 * The other buffers are ignored. This value affects the meaning of the function's 
		 * return value upon failure; see the Return Values section. 
		 */																																																																
		public const uint GGO_METRICS = 0;
		
		/**
		 * The function retrieves the curve data points in the rasterizer's native 
		 * format and uses the font's design units.  
		 */
		public const uint GGO_NATIVE = 2;
		
		/** 
		 * Windows 2000/XP: The function only returns unhinted outlines. This flag 
		 * only works in conjunction with GGO_BEZIER and GGO_NATIVE. 
		 */
		public const uint GGO_UNHINTED = 0x0100;

		[DllImport("Gdi32.dll", CharSet=CharSet.Auto)]
		public static extern int GetCharWidthFloat(
			IntPtr hdc,					// handle to DC
			uint iFirstChar,			// first-character code point
			uint iLastChar,				// last-character code point
			[In,Out] float[] pxBuffer	// buffer for widths
			);

		/**
		 * The GetTextExtentPoint32 function computes the width and height of 
		 * the specified string of text. 
		 */
		[DllImport("Gdi32.dll", CharSet=CharSet.Auto)]
		public static extern int GetTextExtentPoint32(
			IntPtr hdc,				// handle to DC
			String lpString,		// text string
			int cbString,			// characters in string
			[In,Out] SIZE lpSize    // string size
			);

		/**
		 * The GetTextMetrics function fills the specified buffer with the metrics 
		 * for the currently selected font. 
		 */
		[DllImport("Gdi32.dll", CharSet=CharSet.Auto)]
		public static extern int GetTextMetrics(
			IntPtr hdc,				// handle to DC
			ref TEXTMETRIC lptm		// text metrics
			);

		/**
		 * The GetDC function retrieves a handle to a display device context (DC) 
		 * for the client area of a specified window or for the entire screen. 
		 * You can use the returned handle in subsequent GDI functions to draw 
		 * in the DC.
		 */
		[DllImport("User32.dll")]
		public static extern IntPtr GetDC(
			IntPtr hWnd				// handle to window
			);

		/**
		 * The ReleaseDC function releases a device context (DC), freeing it 
		 * for use by other applications. The effect of the ReleaseDC function 
		 * depends on the type of DC. It frees only common and window DCs. 
		 * It has no effect on class or private DCs. 
		 */
		[DllImport("User32.dll")]
		public static extern int ReleaseDC(
			IntPtr hWnd,	// handle to window
			IntPtr hDC		// handle to DC
			);

		/**
		 * The GetGlyphIndices function translates a string into an array of 
		 * glyph indices. The function can be used to determine whether a 
		 * glyph exists in a font.
		 */
		[DllImport("Gdi32.dll", CharSet=CharSet.Auto)]
		public static extern uint GetGlyphIndices(
			IntPtr hdc,				// handle to DC
			string lpstr,			// string to convert
			int c,					// number of characters in string
			[In,Out] ushort[] pgi,	// array of glyph indices
			uint fl					// glyph options
			);

		public const uint GGI_MARK_NONEXISTING_GLYPHS = 0xffff;

		/**
		 * The GetLastError function retrieves the calling thread's last-error code 
		 * value. The last-error code is maintained on a per-thread basis. Multiple 
		 * threads do not overwrite each other's last-error code.
		 */
		[DllImport("Kernel32.dll")]
		public static extern uint GetLastError();

		/**
		 * The SelectObject function selects an object into the specified 
		 * device context (DC). The new object replaces the previous object 
		 * of the same type. 
		 */
		[DllImport("Gdi32.dll", CharSet=CharSet.Auto)]
		public static extern IntPtr SelectObject(
			IntPtr hdc,      // handle to DC
			IntPtr hgdiobj   // handle to object
			);

		/**
		 * The DeleteObject function deletes a logical pen, brush, font, 
		 * bitmap, region, or palette, freeing all system resources associated 
		 * with the object. After the object is deleted, the specified handle 
		 * is no longer valid. 
		 */
		[DllImport("Gdi32.dll", CharSet=CharSet.Auto)]
		public static extern bool DeleteObject(IntPtr hObject);

		/**
		 * The GetCharABCWidthsI function retrieves the widths, in logical units, 
		 * of consecutive glyph indices in a specified range from the current 
		 * TrueType font. This function succeeds only with TrueType fonts. 
		 */
		[DllImport("Gdi32.dll", CharSet=CharSet.Auto)]
		public static extern bool GetCharABCWidthsI(
			IntPtr hdc,				// handle to DC
			uint giFirst,			// first glyph index in range
			uint cgi,				// count of glyph indices in range
			[In,Out] ushort[] pgi,  // array of glyph indices
			[In,Out] ABC[] lpabc    // array of character widths
			);

		/**
		 * The DrawText function draws formatted text in the specified rectangle. 
		 * It formats the text according to the specified method (expanding tabs, 
		 * justifying characters, breaking lines, and so forth). 
		 * To specify additional formatting options, use the DrawTextEx function.
		 */
		[DllImport("User32.dll", CharSet=CharSet.Auto)]
		public static extern int DrawText(
            IntPtr hDC,				// handle to DC
			string lpString,		// text to draw
			int nCount,				// text length
			ref RECT lpRect,		// formatting dimensions
			uint uFormat			// text-drawing options
			);

		/**
		 * The ChooseFont function creates a Font dialog box that enables 
		 * the user to choose attributes for a logical font. These 
		 * attributes include a typeface name, style (bold, italic, or regular), 
		 * point size, effects (underline, strikeout, and text color), and a 
		 * script (or character set). 
		 */
		[DllImport("Comdlg32.dll", CharSet=CharSet.Auto)]
		public static extern bool ChooseFont(
            ref CHOOSEFONT lpcf   // initialization data
			);


		/**
		 * The GetTextFace function retrieves the typeface name of the font 
		 * that is selected into the specified device context. 
		 */
		[DllImport("Gdi32.dll", CharSet=CharSet.Auto)]
		public static extern int GetTextFace(
            IntPtr hdc,					// handle to DC
			int nCount,					// length of typeface name buffer
			[In,Out] char[] lpFaceName  // typeface name buffer
		);

		/**
		 * The GetCharABCWidthsFloat function retrieves the widths, in logical units, 
		 * of consecutive characters in a specified range from the current font. 
		 */
		[DllImport("Gdi32.dll", CharSet=CharSet.Auto)]
		public static extern bool GetCharABCWidthsFloat(
			IntPtr hdc,				 // handle to DC
			uint iFirstChar,		 // first character in range
			uint iLastChar,			 // last character in range
			[In,Out] ABCFLOAT[] lpABCF // array of character widths
			);

		/**
		 * The TextOut function writes a character string at the specified 
		 * location, using the currently selected font, background color, 
		 * and text color. 
		 */
		[DllImport("Gdi32.dll", CharSet=CharSet.Auto)]
		public static extern bool  TextOut(
			IntPtr hdc,        // handle to DC
			int nXStart,       // x-coordinate of starting position
			int nYStart,       // y-coordinate of starting position
			String lpString,   // character string
			int cbString       // number of characters
			);

		/**
		 * Same as the previous TextOut, just with a minor change
		 * to help marshaling in certain cases
		 */
		[DllImport("Gdi32.dll", CharSet=CharSet.Auto)]
		public static extern bool  TextOut(
			IntPtr hdc,        // handle to DC
			int nXStart,       // x-coordinate of starting position
			int nYStart,       // y-coordinate of starting position
			[In] Char[] lpString,   // character string
			int cbString       // number of characters
			);

		/* Text Alignment Options */
		public const uint TA_NOUPDATECP = 0;
		public const uint TA_UPDATECP=    1;
		public const uint TA_LEFT =       0;
		public const uint TA_RIGHT =      2;
		public const uint TA_CENTER =     6;
		public const uint TA_TOP =        0;
		public const uint TA_BOTTOM =     8;
		public const uint TA_BASELINE =   24;
		public const uint TA_RTLREADING = 256;
		public const uint TA_MASK =       (TA_BASELINE+TA_CENTER+TA_UPDATECP+TA_RTLREADING);

		/* Ternary raster operations */
		public const uint  SRCCOPY =             (uint)0x00CC0020; /* dest = source                   */
		public const uint  SRCPAINT =            (uint)0x00EE0086; /* dest = source OR dest           */
		public const uint  SRCAND =              (uint)0x008800C6; /* dest = source AND dest          */
		public const uint  SRCINVERT =           (uint)0x00660046; /* dest = source XOR dest          */
		public const uint  SRCERASE =            (uint)0x00440328; /* dest = source AND (NOT dest )   */
		public const uint  NOTSRCCOPY =          (uint)0x00330008; /* dest = (NOT source)             */
		public const uint  NOTSRCERASE =         (uint)0x001100A6; /* dest = (NOT src) AND (NOT dest) */
		public const uint  MERGECOPY =           (uint)0x00C000CA; /* dest = (source AND pattern)     */
		public const uint  MERGEPAINT =          (uint)0x00BB0226; /* dest = (NOT source) OR dest     */
		public const uint  PATCOPY =             (uint)0x00F00021; /* dest = pattern                  */
		public const uint  PATPAINT =            (uint)0x00FB0A09; /* dest = DPSnoo                   */
		public const uint  PATINVERT =           (uint)0x005A0049; /* dest = pattern XOR dest         */
		public const uint  DSTINVERT =           (uint)0x00550009; /* dest = (NOT dest)               */
		public const uint  BLACKNESS =           (uint)0x00000042; /* dest = BLACK                    */
		public const uint  WHITENESS =           (uint)0x00FF0062; /* dest = WHITE                    */

		/**
		 * The SetTextAlign function sets the text-alignment flags for the 
		 * specified device context. 
		 */
		[DllImport("Gdi32.dll", CharSet=CharSet.Auto)]
		public static extern uint SetTextAlign(
			IntPtr hdc,     // handle to DC
			uint fMode		// text-alignment option
			);

		/**
		 * The GetTextAlign function retrieves the text-alignment setting 
		 * for the specified device context. 
		 */
		[DllImport("Gdi32.dll", CharSet=CharSet.Auto)]
		public static extern uint GetTextAlign(
			IntPtr hdc   // handle to DC
			);


		/**
		 * The LineTo function draws a line from the current position up to, but 
		 * not including, the specified point. 
		 */
		[DllImport("Gdi32.dll", CharSet=CharSet.Auto)]
		public static extern bool LineTo(
			IntPtr hdc, // device context handle
			int nXEnd,  // x-coordinate of ending point
			int nYEnd   // y-coordinate of ending point
			);

		/**
		 * The MoveToEx function updates the current position to the specified 
		 * point and optionally returns the previous position. 
		 */
		[DllImport("Gdi32.dll", CharSet=CharSet.Auto)]
		public static extern bool MoveToEx(
			IntPtr hdc,          // handle to device context
			int X,               // x-coordinate of new current position
			int Y,               // y-coordinate of new current position
			ref POINT lpPoint    // old current position
			);

		/**
		 * The Rectangle function draws a rectangle. The rectangle is outlined by 
		 * using the current pen and filled by using the current brush. 
		 */
		[DllImport("Gdi32.dll", CharSet=CharSet.Auto)]
		public static extern bool Rectangle(
			IntPtr hdc,      // handle to DC
			int nLeftRect,   // x-coord of upper-left corner of rectangle
			int nTopRect,    // y-coord of upper-left corner of rectangle
			int nRightRect,  // x-coord of lower-right corner of rectangle
			int nBottomRect  // y-coord of lower-right corner of rectangle
			);

		/**
		 * The GetDeviceCaps function retrieves device-specific information 
		 * for the specified device.
		 */
		[DllImport("Gdi32.dll", CharSet=CharSet.Auto)]
		public static extern int GetDeviceCaps(
			IntPtr hdc,     // handle to DC
			int nIndex      // index of capability
			);

		public const int LOGPIXELSX =   88;    /* Logical pixels/inch in X                 */
		public const int LOGPIXELSY =   90;    /* Logical pixels/inch in Y                 */

		/**
		* The GetTextExtentPoint32 function computes the width and height of 
		* the specified string of text. 
		*/
		[DllImport("Gdi32.dll", CharSet=CharSet.Auto)]
		public static extern bool GetTextExtentPoint32(
			IntPtr hdc,           // handle to DC
			String lpString,  // text string
			int cbString,      // characters in string
			ref SIZE lpSize      // string size
			);


		/**
		* he SetBkMode function sets the background mix mode of the specified 
		* device context. The background mix mode is used with text, hatched brushes, 
		* and pen styles that are not solid lines. 
		*/
		[DllImport("Gdi32.dll", CharSet=CharSet.Auto)]
		public static extern int SetBkMode(
			IntPtr hdc,      // handle to DC
			int iBkMode   // background mode
			);

		/* Background Modes */
		public static readonly int TRANSPARENT = 1;
		public static readonly int OPAQUE =      2;
		public static readonly int BKMODE_LAST = 2;

		/**
		 * The CreateCompatibleDC function creates a memory device context (DC) compatible 
		 * with the specified device. 
		 */
		[DllImport("Gdi32.dll", CharSet=CharSet.Auto)]
		public static extern IntPtr CreateCompatibleDC(IntPtr hdc);

		/**
		 * The DeleteDC function deletes the specified device context (DC).
		 */
		[DllImport("Gdi32.dll", CharSet=CharSet.Auto)]
		public static extern bool DeleteDC(IntPtr hdc);


		/**
		 * The GetTextColor function retrieves the current text color for the specified device context. 
		 */
		[DllImport("Gdi32.dll", CharSet=CharSet.Auto)]
		public static extern uint GetTextColor(IntPtr hdc);

		/**
		 * The SetTextColor function sets the text color for the specified 
		 * device context to the specified color. 
		 */
		[DllImport("Gdi32.dll", CharSet=CharSet.Auto)]
		public static extern uint SetTextColor(
			IntPtr hdc,    // handle to DC
			uint crColor   // text color
            );

		/**
		 * The BitBlt function performs a bit-block transfer of the color data corresponding 
		 * to a rectangle of pixels from the specified source device context into a destination 
		 * device context. 
		 */
		[DllImport("Gdi32.dll", CharSet=CharSet.Auto)]
		public static extern bool BitBlt(
            IntPtr hdcDest, // handle to destination DC
            int nXDest,  // x-coord of destination upper-left corner
            int nYDest,  // y-coord of destination upper-left corner
            int nWidth,  // width of destination rectangle
            int nHeight, // height of destination rectangle
            IntPtr hdcSrc,  // handle to source DC
            int nXSrc,   // x-coordinate of source upper-left corner
            int nYSrc,   // y-coordinate of source upper-left corner
            uint dwRop  // raster operation code
            );

		/**
		 * The CreateCompatibleBitmap function creates a bitmap compatible with the device that 
		 * is associated with the specified device context. 
		 */
		[DllImport("Gdi32.dll", CharSet=CharSet.Auto)]
		public static extern IntPtr CreateCompatibleBitmap(
            IntPtr hdc,        // handle to DC
            int nWidth,     // width of bitmap, in pixels
            int nHeight     // height of bitmap, in pixels
			);

		/**
		 * The CreateCaret function creates a new shape for the system caret and assigns 
		 * ownership of the caret to the specified window. The caret shape can be a line, 
		 * a block, or a bitmap. 
		 */
		[DllImport("User32.dll", CharSet=CharSet.Auto)]
		public static extern bool CreateCaret(IntPtr hWnd, IntPtr hBitmap, int nWidth, int nHeight);

		/**
		 * The DestroyCaret function destroys the caret's current shape, frees the caret 
		 * from the window, and removes the caret from the screen. 
		 */
		[DllImport("User32.dll", CharSet=CharSet.Auto)]
		public static extern bool DestroyCaret();

		/**
		 * The GetCaretBlinkTime function returns the time required to invert the caret's pixels. 
		 * The user can set this value. 
		 */
		[DllImport("User32.dll", CharSet=CharSet.Auto)]
		public static extern uint GetCaretBlinkTime();

		/**
		 * The GetSelection function copies the caret's position to the specified POINT structure. 
		 */
		[DllImport("User32.dll", CharSet=CharSet.Auto)]
		public static extern bool GetSelection(ref POINT lpPoint);

		/**
		 * The HideCaret function removes the caret from the screen. Hiding a caret does not 
		 * destroy its current shape or invalidate the insertion point. 
		 */
		[DllImport("User32.dll", CharSet=CharSet.Auto)]
		public static extern bool HideCaret(IntPtr hWnd);

		/**
		 * The SetCaretBlinkTime function sets the caret blink time to the specified 
		 * number of milliseconds. The blink time is the elapsed time, in milliseconds, 
		 * required to invert the caret's pixels. 
		 */
		[DllImport("User32.dll", CharSet=CharSet.Auto)]
		public static extern bool SetCaretBlinkTime(uint uMSeconds);

		/**
		 * The SetSelection function moves the caret to the specified coordinates. 
		 * If the window that owns the caret was created with the CS_OWNDC class style, 
		 * then the specified coordinates are subject to the mapping mode of the device 
		 * context associated with that window. 
		 */
		[DllImport("User32.dll", CharSet=CharSet.Auto)]
		public static extern bool SetCaretPos(int X, int Y);

		/**
		 * The ShowCaret function makes the caret visible on the screen at the 
		 * caret's current position. When the caret becomes visible, it begins flashing automatically.
		 */
		[DllImport("User32.dll", CharSet=CharSet.Auto)]
		public static extern bool ShowCaret(IntPtr hWnd);

		[DllImport("User32.dll", CharSet=CharSet.Auto)]
		public static extern bool IsWindowEnabled(IntPtr hWnd);

		[DllImport("User32.dll", CharSet=CharSet.Auto)]
		public static extern bool IsWindowVisible(IntPtr hWnd);

		/**
		 * The GetStockObject function retrieves a handle to one of the stock pens, 
		 * brushes, fonts, or palettes. 
		 */
		[DllImport("Gdi32.dll", CharSet=CharSet.Auto)]
		public static extern IntPtr  GetStockObject(
			int fnObject   // stock object type
			);

		/**
		 * The FillRect function fills a rectangle by using the specified brush. 
		 * This function includes the left and top borders, but excludes the right 
		 * and bottom borders of the rectangle. 
		 */
		[DllImport("User32.dll", CharSet=CharSet.Auto)]
		public static extern int FillRect(
			IntPtr hDC,     // handle to DC
			ref RECT lprc,  // rectangle
			IntPtr hbr      // handle to brush
			);

		/**
		 * The RoundRect function draws a rectangle with rounded corners. The rectangle 
		 * is outlined by using the current pen and filled by using the current brush. 
		 */
		[DllImport("Gdi32.dll", CharSet=CharSet.Auto)]
		public static extern bool RoundRect(
			IntPtr hdc,      // handle to DC
			int nLeftRect,   // x-coord of upper-left corner of rectangle
			int nTopRect,    // y-coord of upper-left corner of rectangle
			int nRightRect,  // x-coord of lower-right corner of rectangle
			int nBottomRect, // y-coord of lower-right corner of rectangle
			int nWidth,      // width of ellipse
			int nHeight      // height of ellipse
			);

        /**
		 * float version of the RoundRect function. This draws a rectangle that 
		 * fully encloses the given values.
		 * this probably should not go here, but does not really fit anywhere else.
		 */
		public static void RoundRect(IntPtr hdc, float left, float top, float right, float bottom, float eWidth, float eHeight)
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

			Win32.RoundRect(hdc, rect.left, rect.top, rect.right, rect.bottom, (int)eWidth, (int)eHeight);
		}


		/**
		 * stock gid objects, read in static ctor
		 */
		public static readonly IntPtr StockBlackPen = Win32.GetStockObject(Win32.BLACK_PEN);
		public static readonly IntPtr StockWhitePen = Win32.GetStockObject(Win32.WHITE_PEN);


		/**
		 * misc clipboard functions. These are needed because there is a bug in the 
		 * .net Clipboard object where the copying of a metafile does not
		 * work correctly.
		 */
		[DllImport("user32.dll")]
		public static extern bool OpenClipboard(IntPtr hWndNewOwner);
		[DllImport("user32.dll")]
		public static extern bool EmptyClipboard();
		[DllImport("user32.dll")]
		public static extern IntPtr SetClipboardData(uint uFormat, IntPtr hMem);
		[DllImport("user32.dll")]
		public static extern bool CloseClipboard();
		[DllImport("gdi32.dll")]
		public static extern IntPtr CopyEnhMetaFile(IntPtr hemfSrc, IntPtr hNULL);
		[DllImport("gdi32.dll")]
		public static extern bool DeleteEnhMetaFile(IntPtr hemf);


		/**
		 * The PatBlt function paints the specified rectangle using the brush that is currently 
		 * selected into the specified device context. The brush color and the surface color or 
		 * colors are combined by using the specified raster operation. 
		 **/
		[DllImport("Gdi32.dll", CharSet=CharSet.Auto)]
		public static extern bool PatBlt(
			IntPtr hdc,      // handle to DC
			int nXLeft,   // x-coord of upper-left rectangle corner
			int nYLeft,   // y-coord of upper-left rectangle corner
			int nWidth,   // width of rectangle
			int nHeight,  // height of rectangle
			uint dwRop   // raster operation code
			);

		/**
		 * The CreatePatternBrush function creates a logical brush with the specified bitmap pattern. 
		 * The bitmap can be a DIB section bitmap, which is created by the CreateDIBSection function, 
		 * or it can be a device-dependent bitmap.
		 */
		[DllImport("Gdi32.dll", CharSet=CharSet.Auto)]
		public static extern IntPtr CreatePatternBrush(
            IntPtr hbmp   // handle to bitmap
            );
	}
}


/*
 * more info on GetGlyphOutline:
 * C# Signature:
 * [DllImport("gdi32.dll")]
 * static extern uint GetGlyphOutline(IntPtr hdc, uint uChar, uint uFormat,
 *  out GLYPHMETRICS lpgm, uint cbBuffer, IntPtr lpvBuffer, ref MAT2 lpmat2);
 * User-Defined Types:
 * [StructLayout(LayoutKind.Sequential)]
 * public struct TTPOLYGONHEADER
 * {
 *		public int cb;
 *
 *        public int dwType;
 *        [MarshalAs(UnmanagedType.Struct)] public POINTFX pfxStart;
 *		
 *
 *}
 *
 *[StructLayout(LayoutKind.Sequential)]
 *
 *        public struct TTPOLYCURVEHEADER
 *
 *{
 *
 *        public short wType;
 *        public short cpfx;
 *
 *}
 *
 *[StructLayout(LayoutKind.Sequential)]
 *
 *public struct FIXED
 *
 *{
 *
 *        public short fract;
 *        public short value;
 *
 *}
 *
 *[StructLayout(LayoutKind.Sequential)]
 *
 *public struct TODO - a short description
 *5/7/2004 6:30:16 PM - -207.46.238.143
 *MAT2
 *
 *{
 *
 *        [MarshalAs(UnmanagedType.Struct)] public FIXED eM11;
 *        [MarshalAs(UnmanagedType.Struct)] public FIXED eM12;
 *        [MarshalAs(UnmanagedType.Struct)] public FIXED eM21;
 *        [MarshalAs(UnmanagedType.Struct)] public FIXED eM22;
 *
 *}
 *
 *[StructLayout(LayoutKind.Sequential)]
 *
 *        public struct POINTFX
 *
 *{
 *
 *        [MarshalAs(UnmanagedType.Struct)] public FIXED x;
 *        [MarshalAs(UnmanagedType.Struct)] public FIXED y;
 *
 *}
 *
 *[StructLayout(LayoutKind.Sequential)]
 *
 *        public struct GLYPHMETRICS
 *
 *{
 *
 *        public int gmBlackBoxX;
 *        public int gmBlackBoxY;
 *        [MarshalAs(UnmanagedType.Struct)] public POINTFX gmptGlyphOrigin;
 *        public short gmCellIncX;
 *        public short gmCellIncY;
 *
 *}
 *Notes:
 *
 *None.
 *Tips & Tricks:
 *
 *Please add some!
 *Sample Code:
 *
 * // Parse a glyph outline in native format
 * public static void GetGlyphShape(Font font, Char c)
 * {                        
 *		GLYPHMETRICS metrics = new GLYPHMETRICS();                        
 *      MAT2 matrix = new MAT2();
 *      matrix.eM11.value = 1;
 *      matrix.eM12.value = 0;
 *      matrix.eM21.value = 0;
 *      matrix.eM22.value = 1;
 *      using(Bitmap b = new Bitmap(1,1))
 *      {
 *			using(Graphics g = Graphics.FromImage(b))
 *          {
 *				IntPtr hdc = g.GetHdc();                                                                                
 *              IntPtr prev = SelectObject(hdc, font.ToHfont());                                        
 *              int bufferSize = (int)GetGlyphOutline(hdc, (uint)c, (uint)2, out metrics, 0, IntPtr.Zero, ref matrix);
 *              IntPtr buffer = Marshal.AllocHGlobal(bufferSize);
 *              try
 *              {					                        
 *                  int ret;
 *                  if((ret = GetGlyphOutline(hdc, (uint)c, 2, out metrics, (uint)bufferSize, buffer, ref matrix)) > 0)
 *                  {						                                                        
 *                      int polygonHeaderSize = Marshal.SizeOf(typeof(TTPOLYGONHEADER));
 *                      int curveHeaderSize = Marshal.SizeOf(typeof(TTPOLYCURVEHEADER));
 *                      int pointFxSize = Marshal.SizeOf(typeof(POINTFX));
 *                      int index = 0;
 *                      while(index < bufferSize)
 *						{                                                                
 *							TTPOLYGONHEADER header = (TTPOLYGONHEADER)Marshal.PtrToStructure(
 *								new IntPtr(buffer.ToInt32()+index), typeof(TTPOLYGONHEADER));
 *							
 *                          int startX = header.pfxStart.x.value;
 *                          int startY = -header.pfxStart.y.value;
 *                          // ...do something with start coords...
 *                          int endCurvesIndex = index+header.cb;
 *                          index+=polygonHeaderSize;
 *                          while(index < endCurvesIndex)
 *                          {
 *								TTPOLYCURVEHEADER curveHeader = (TTPOLYCURVEHEADER)Marshal.PtrToStructure(
 *									new IntPtr(buffer.ToInt32()+index), typeof(TTPOLYCURVEHEADER));
 *								
 *                              index+=curveHeaderSize;
 *                              POINTFX[] curvePoints = new POINTFX[curveHeader.cpfx];
 *                              for(int i = 0; i < curveHeader.cpfx; i++)
 *                              {
 *									curvePoints[i] = (POINTFX)Marshal.PtrToStructure(new IntPtr(buffer.ToInt32()+index), typeof(POINTFX));
 *                                  index+=pointFxSize;
 *                              }
 *                              if(curveHeader.wType == (int)1)
 *                              {
 *									// POLYLINE
 *                                  for(int i=0; i < curveHeader.cpfx; i++)
 *                                  {
 *										short x = curvePoints[i].x.value;
 *                                      short y = (short)-curvePoints[i].y.value;
 *										// ...do something with line points...
 *									}
 *                              }
 *                              else
 *                              {
 *									// CURVE
 *                                  for(int i=0; i < curveHeader.cpfx - 1; i++)
 *                                  {
 *										POINTFX pfxB = curvePoints[i];
 *										POINTFX pfxC = curvePoints[i+1];
 *                                      short cx = pfxB.x.value;
 *                                      short cy = (short)-pfxB.y.value;
 *                                      short ax;
 *                                      short ay;
 *                                      if(i < curveHeader.cpfx - 2)
 *                                      {
 *											ax = (short)((pfxB.x.value+pfxC.x.value) / 2);
 *                                          ay = (short)-((pfxB.y.value+pfxC.y.value) / 2);
 *                                      }
 *                                      else
 *                                      {
 *											ax = pfxC.x.value;
 *                                          ay = (short)-pfxC.y.value;
 *                                      } // ...do something with curve points...
 *									}
 *                              }
 *                          }                                                                
 *                      }
 *                  }
 *                  else
 *                  {
 *						throw new Exception("Could not retrieve glyph (GDI Error: 0x"+ret.ToString("X")+")");
 *                  }
 *                  g.ReleaseHdc(hdc);
 *              }
 *              finally
 *              {                                                
 *					Marshal.FreeHGlobal(buffer);                                        
 *              }
 *          }
 *     }
 * return shape;
 * }
 *
 */