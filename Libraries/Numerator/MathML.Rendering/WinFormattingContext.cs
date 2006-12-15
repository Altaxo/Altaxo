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
using System.Configuration;
using System.IO;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Diagnostics;
using Scaled = System.Single;

namespace MathML.Rendering
{
	public class WinFormattingContext : WinContextBase, IFormattingContext
  {
    #region FontFactory
    /// <summary>
    /// Summary description for FontFactory.
    /// </summary>
    private class FontFactory
    {
      /**
       * keep track of all the fonts that this class
       * created. 
       * 
       * This is a list of weak references, in that the font is freed when
       * all references except this one are gone. Font or glyph areas hold
       * strong references to fonts.
       */
      private ArrayList fonts = new ArrayList();

      PrivateFontCollection _fontCollection = new PrivateFontCollection();
      Hashtable _fontCollectionFamilyCache;


      public FontFactory()
      {
        string configDir = System.Configuration.ConfigurationManager.AppSettings.Get("MathMLRenderingConfig");
        string searchDir = null;

        if (configDir == null || configDir.Length == 0)
          searchDir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
        else if (Path.IsPathRooted(configDir))
        {
          if (Directory.Exists(configDir))
            searchDir = configDir;
          else
            throw new ApplicationException("Configured font configuration file directory does not exist! Configured path: " + configDir);
        }
        else // path is not rooted
        {
          string basepath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + Path.DirectorySeparatorChar;
          if (Directory.Exists(basepath + configDir))
            searchDir = basepath + configDir;
          else
            throw new ApplicationException("Configured font configuration file directory does not exist! Configured path: " + configDir + ", the base path is: " + basepath);
        }

        string[] files = Directory.GetFiles(searchDir, "*.ttf");
        foreach (string file in files)
          _fontCollection.AddFontFile(file);

        _fontCollectionFamilyCache = new Hashtable();
        FontFamily[] families = _fontCollection.Families;
        for (int i = 0; i < families.Length; i++)
          _fontCollectionFamilyCache.Add(families[i].Name, i);
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
      public IFontHandle CreateFont(Graphics gr, float emHeight, bool italic, int weight, String fontName)
      {
        Font ft;
        if (_fontCollectionFamilyCache.Contains(fontName))
        {
          FontFamily fam = _fontCollection.Families[(int)_fontCollectionFamilyCache[fontName]];
          ft = new Font(fam, emHeight, (italic ? FontStyle.Italic : FontStyle.Regular) | (weight > 500 ? FontStyle.Bold : FontStyle.Regular), GraphicsUnit.World);
        }
        else
        {
        ft = new Font(fontName, emHeight, (italic ? FontStyle.Italic : FontStyle.Regular) | (weight > 500 ? FontStyle.Bold : FontStyle.Regular), GraphicsUnit.World);
        }
        float ascending = (ft.Size * ft.FontFamily.GetCellAscent(ft.Style)) / ft.FontFamily.GetEmHeight(ft.Style);

        return new FontHandle(ft, emHeight, italic, weight, fontName, ascending);
      }

      /**
       * create a font based only on the MathVariant and MathSize 
       * fields of the FormattingContext
       * Currently, this only a shortcut to GetFont(context, "", ""), but
       * internally may be optimized in the future.
       */
      public  IFontHandle GetFont(IFormattingContext context)
      {
        return GetFont(context, "", "");
      }

     

      /**
       * get a font based on the font type given in the variant, 
       * and the size given in height. If the size can not be 
       * evaluated to a valid size, the default font size is used.
       * the fontName override anything in the context if it is given.
       */
      public  IFontHandle GetFont(IFormattingContext context, string fontName, string altFontName)
      {
        // font to be returned
        IFontHandle font = null;

        // find the attributes from the static array
        FontAttributes attr = fontAttributes[(int)context.MathVariant];

        float height = context.Size;

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

        if (font == null)
        {
          font = CreateFont(((WinFormattingContext)context)._graphics, height, italic, weight, name);
          fonts.Add(new WeakReference(font));
        }
        return font;
      }

      /**
       * try to find a font in the list that matches the contents
       * of the formating context
       */
      private IFontHandle FindFont(float height, bool italic, int weight, string fontName)
      {
        for (int i = 0; i < fonts.Count; i++)
        {
          WeakReference w = (WeakReference)fonts[i];
          if (w.IsAlive)
          {
            IFontHandle f = (IFontHandle)w.Target;
            if (f.Equals(height, italic, weight, fontName)) return f;
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

      private FontAttributes[] fontAttributes = 
		{
			new FontAttributes("Times New Roman", fwNormal,	false), // Normal
			new FontAttributes("Times New Roman", fwBold,	false), // Bold
			new FontAttributes("Times New Roman", fwNormal,	true), // Italic
			new FontAttributes("Times New Roman", fwBold,	false), // BoldItalic
			new FontAttributes("Times New Roman", fwNormal,	false), // DoubleStruck
			new FontAttributes("Times New Roman", fwBold,	false), // BoldFraktur
			new FontAttributes("Times New Roman", fwNormal,	false), // Script
			new FontAttributes("Times New Roman", fwBold,	false), // BoldScript
			new FontAttributes("Times New Roman", fwNormal,	false), // Fracktur
			new FontAttributes("Arial", fwNormal,	false), // SansSerif
			new FontAttributes("Arial", fwBold,	false), // BoldSansSerif
			new FontAttributes("Arial", fwNormal,	true), // SansSerifItalic
			new FontAttributes("Arial", fwBold,	true), // SansSerifBoldItalic
			new FontAttributes("Courier New",	  fwNormal,	false),	// Monospace
			new FontAttributes("Times New Roman", fwNormal,	false)  // Unknown
		};

      /**
      * the default font weight, may change this to look up from
      * some external source in the future.
      */

      public const int fwNormal = 400;
      public const int fwBold = 700;

      private  int DefaultFontWeight
      {
        get
        {
          return fwNormal;
        }
      }
    }
    #endregion

    static FontFactory _fontFactory = new FontFactory();

    public IFontHandle GetFont()
    {
      return _fontFactory.GetFont(this);
    }

    public IFontHandle GetFont(string fontName, string altFontName)
    {
      return _fontFactory.GetFont(this, fontName, altFontName);
    }

    public IFontHandle CreateFont(float emHeightInPixels, bool italic, int weight, String fontName)
    {
      return _fontFactory.CreateFont(_graphics, emHeightInPixels, italic, weight, fontName);
    }

		/// <summary>
		/// default contstuctor
		/// </summary>
		public WinFormattingContext(Graphics g)
      : base(g)
		{
      _graphics = g;
			size = Evaluate(new Length(LengthType.Pt, DefaultFontPointSize));
			minSize  = Evaluate(new Length(LengthType.Pt, 6));
			actualSize = size;
		}

		/// <summary>
		/// default contstuctor
		/// </summary>
		public WinFormattingContext(Graphics g, float fontSize)
      :base(g)
		{
      _graphics = g;
			size = Evaluate(new Length(LengthType.Pt, fontSize));
			minSize  = Evaluate(new Length(LengthType.Pt, 6));
			actualSize = size;
		}

		/// <summary>
		/// make a new copy of an existing context
		/// </summary>
		public WinFormattingContext(WinFormattingContext ctx)
      : base(ctx._graphics)
		{
			this.actualSize = ctx.actualSize;
			this.BackgroundColor = ctx.BackgroundColor;
			this.Color = ctx.Color;
			this.DisplayStyle = ctx.DisplayStyle;
			this.minSize = ctx.minSize;
			this.scriptLevel = ctx.scriptLevel;
			this.size = ctx.size;
			this.SizeMultiplier = ctx.SizeMultiplier;
			this.Stretch = ctx.Stretch;
			this.cacheArea = ctx.cacheArea;
			this.Parens = ctx.Parens;
		}

    public IFormattingContext Clone()
    {
      return new WinFormattingContext(this);
    }

		public const float DefaultFontPointSize = 13;

		/// <summary>
		/// The current font size, this is the current emHeight of the font to used
		/// for creating areas when a node is formatted.
		/// </summary>
		public float Size
		{
			get { return size; }
			set 
			{
				size = Math.Max(value, minSize);
				actualSize = size;
			}
		}

		/// <summary>
		/// evaluate a length using the current font size (in pixels)
		/// as the default size
		/// </summary>
		public float Evaluate(Length length)
		{
			return Evaluate(length, Size);
		}

        /// <summary>
		/// evaluate a length to a true size in pixels. 
		/// this calculation is affected by the current font, and 
		/// by any previous (parent) style node. In gtkmathview, this 
		/// method was part of the graphic device, but I think that it
		/// makes more sense as part of the formatting context. This
		/// calculation is heavily dependent on the current state of
		/// formatting, and the formatting context manages that 
		/// state, so lenght evaluation should go here.
        /// </summary>
        /// <param name="length">a Math Length if the length is not valid, 
        /// the default valud is returned</param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
		public float Evaluate(Length length, float defaultValue)
		{
			float result = 0;
			switch(length.Type)
			{
				case LengthType.Big:
				{
					result = this.PixelsToGU(20);
				} break;
				case LengthType.Cm:
				{
					result = CMsToGU(length.Value);
				} break;
				case LengthType.Em:
				{
					result = (defaultValue * length.Value);
				} break;
				case LengthType.Ex:
				{
					result = (this.Ex * length.Value);
				} break;
				case LengthType.In:
				{
          result = InchesToGU(length.Value);
				} break;
				case LengthType.Infinity:
				{
					Debug.Assert(false);
				} break;
				case LengthType.Medium:
				{
					result = (medium * defaultValue);
				} break;
				case LengthType.Mm:
				{
          result = MMsToGU( length.Value);
				} break;
				case LengthType.NegativeMedium:
				{
					result = (negativeMedium * defaultValue);
				} break;
				case LengthType.NegativeThick:
				{
					result = (negativeThick * defaultValue);
				} break;
				case LengthType.NegativeThin:
				{
					result = (negativeThin * defaultValue);
				} break;
				case LengthType.NegativeVeryThick:
				{
					result = (negativeVeryThick * defaultValue);
				} break;
				case LengthType.NegativeVeryThin:
				{
					result = (negativeVeryThin * defaultValue);
				} break;
				case LengthType.NegativeVeryVeryThick:
				{
					result = (negativeVeryVeryThick * defaultValue);
				} break;
				case LengthType.NegativeVeryVeryThin:
				{
					result = (negativeVeryVeryThin * defaultValue);
				} break;
				case LengthType.Normal:
				{
					result = defaultValue;
				} break;
				case LengthType.Pc:
				{
          result = PicasToGU( length.Value);
				} break;
				case LengthType.Percentage:
				{
					result = (length.Value * defaultValue / 100.0f);
				} break;
				case LengthType.Pt:
				{
          result = PointsToGU(length.Value);
				} break;
				case LengthType.Pure:
				{
					Debug.Assert(false);
				} break;
				case LengthType.Px:
				{
					result = PixelsToGU(length.Value);
				} break;
				case LengthType.Small:
				{
					Debug.Assert(false);
				} break;
				case LengthType.Thick:
				{
					result = (thick * defaultValue);
				} break;
				case LengthType.Thin:
				{
					result = (thin * defaultValue);
				} break;
				case LengthType.Undefined:
				{
					Debug.Assert(false);
				} break;
				case LengthType.VeryThick:
				{
					result = (veryThick * defaultValue);
				} break;
				case LengthType.VeryThin:
				{
					result = (veryThin * defaultValue);
				} break;
				case LengthType.VeryVeryThick:
				{
					result = (veryVeryThick * defaultValue);
				} break;
				case LengthType.VeryVeryThin:
				{
					result = (veryVeryThin * defaultValue);
				} break;
			}
			return result;
		}
	
		/// <summary>
		/// the actual font size. this value is not to be used for creating fonts, 
		/// instead, use the Size value. This value is the true value of the font, 
		/// not taking into acount the min and max font sizes.
		/// </summary>
		public float actualSize;

		/// <summary>
		/// the MathVariant from a presentation token node
		/// </summary>
		public MathVariant _mathVariant = MathVariant.Normal;
    public MathVariant MathVariant { get { return _mathVariant; } set { _mathVariant = value; } }

		/// <summary>
		/// color used for drawing
		/// </summary>
		public Color Color;

		/// <summary>
		/// background color used for drawing
		/// </summary>
		public Color BackgroundColor;

		/// <summary>
		/// number of nested scripts
		/// </summary>
		public int ScriptLevel
		{
			get { return scriptLevel; }
			set
			{
				int d = value - scriptLevel;
				actualSize = (float)(actualSize * Math.Pow(SizeMultiplier, d));
				Size = actualSize;
			}
		}

		/// <summary>
		/// should the current area be cached?
		/// </summary>
		public bool _cacheArea = true;
    public bool cacheArea { get { return _cacheArea; } set { _cacheArea = value; } }

		/// <summary>
		/// currently used for content areas. If true, an apply element should use parens, as
		/// it is inside a function or some other item that does not need parens.
		/// </summary>
		public bool _parens = false;
    public bool Parens { get { return _parens; } set { _parens = value; } }

		/// <summary>
		/// minimum font size that the script can be reduced to. This 
		/// defauts to the value of a 6 point font.
		/// </summary>
		private float minSize;

		/// <summary>
		/// true if formulas must be formated in display mode
		/// </summary>
		public MathML.Display _displayStyle;
    public MathML.Display DisplayStyle { get { return _displayStyle; } set { _displayStyle = value; } }
		
    /// <summary>
		/// amount by which the font size is multiplied when the script level
		/// is incresed or decreased by one
		/// </summary>
		public float SizeMultiplier = 0.71f;

		/// <summary>
		/// the extent at which the node is being asked to stretch to
		/// </summary>
		public BoundingBox _stretch = BoundingBox.New();
    public BoundingBox Stretch { get { return _stretch; } set { _stretch = value; } }
    public float StretchWidth { get { return _stretch.Width; } set { _stretch.Width = value; } }
        
		/// <summary>
		/// the font size
		/// </summary>
		private float size;

		/// <summary>
		/// the script level
		/// </summary>
		private int scriptLevel = 0;

		/// <summary>
		/// scale factors for calculating thickneseses
		/// </summary>
		private float veryVeryThin = 1.0f/18.0f;
		private float veryThin = 2.0f/18.0f;
		private float thin = 3.0f/18.0f;
		private float medium = 4.0f/18.0f;
		private float thick = 5.0f/18.0f;
		private float veryThick = 6.0f/18.0f;
		private float veryVeryThick = 7.0f/18.0f;

		private float negativeVeryVeryThin = 1.0f/18.0f;
		private float negativeVeryThin = 2.0f/18.0f;
		private float negativeThin = 3.0f/18.0f;
		private float negativeMedium = 4.0f/18.0f;
		private float negativeThick = 5.0f/18.0f;
		private float negativeVeryThick = 6.0f/18.0f;
		private float negativeVeryVeryThick = 7.0f/18.0f;

    /// <summary>
    /// the centerline of the current font, this is not 
    /// the baseline, but where the 2 lines cross in an 'x'
    /// </summary>
    public Scaled Axis
    { 
      get
      {
        return Ex / 2.0f;
      }
    }

    /// <summary>
    /// the height of the character "x" in the current 
    /// font size
    /// </summary>
    public float Ex
    {
      get
      {
        BoundingBox size;
        float left, right;
        IFontHandle font = GetFont();

        if (MeasureGlyph( font, 'x', out size, out left, out right))
        {
          return size.Height;
        }
        else
        {
          throw new Exception("MeasureGlyph failed");
        }
      }
    }

    public Scaled DefaultLineThickness
    {
      get
      {
        // should be at least 1 px thick
        //return Math.Max(context.m
        return PixelsToGU(2);
      } 
    }

    


    /**
		 * get the dpi resolution of the current device
		 */
    /*
    public Scaled Dpi
    {
      get
      {// TODO is this right?
      return 72.0f;
      }
    }
     */


    public  bool MeasureGlyph(IFontHandle font, ushort index, out BoundingBox box, out Scaled left, out Scaled right)
    {
      return MeasureGlyph(_graphics, font, index, out box, out left, out right);
    }


   

    

	}
}
