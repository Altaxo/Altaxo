using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using Scaled = System.Single;

namespace MathML.Rendering
{
  public class WinContextBase
  {
    #region internal classes

    /**
	 * A way to store and locate native font resources.
	 * This class holds onto a native font resource, and the
	 * font attributes with which the native font was created.
	 * This is created by the RenderingDevice, and is held 
	 * currently by the glyph and font areas.
	 * When this class is disposed of, it will free the native 
	 * font resource
	 * 
	 * The RenderingDevice will maintain a list of weak references
	 * to all fonts that it creates. When a request for a new font
	 * is made, the RenderingDevice will look through this list list
	 * for a matching font. If one is found, than a reference to the
	 * found font is returned.
	 * 
	 * As the RenderingDevice contains only weak references to fonts, 
	 * the standard garbage collector will take care of freeing them
	 * when there are no more strong references.
	 */
    protected class FontHandle : IFontHandle
    {
      /**
       * create a math font.
       */
      public FontHandle(Font font, float height, bool italic, int weight, string name, float baseline)
      {
        Handle = font;
        Height = height;
        Italic = italic;
        Weight = weight;
        Name = name;
        _baseline = baseline;
        
      }

      /**
       * A reference to a native font resource.
       */
      internal readonly Font Handle;

      /**
       * the height in pixels that the native font was created with
       */
      private readonly float Height;

      /// <summary>
      /// Distance from the top of the cell to the baseline
      /// </summary>
      private readonly float _baseline;
      /**
       * the italic state the native font was created with
       */
      private readonly bool Italic;

      /**
       * the weight the native font resource was created with
       */
      private readonly int Weight;

      /**
       * the name that the native font resource was created with.
       */
      private readonly string Name;

      /**
       * compare a set of font attributes to this font resouce.
       */
      bool IFontHandle.Equals(float height, bool italic, int weight, string name)
      {
        return (Height == height && Italic == italic && Weight == weight &&
          String.Compare(Name, name, false) == 0);
      }

      /// <summary>
      /// Distance of top of the font cell to the baseline = ascending.
      /// </summary>
      public float Baseline { get { return _baseline; } }


    }
    #endregion

    protected Graphics _graphics;
    /// <summary>
    /// This is the factor that must be multiplied to "point" units to get the units in the graphic context.
    /// </summary>
    protected double _pointsToGU;

    public WinContextBase(Graphics g)
    {
      _graphics = g;
      switch (_graphics.PageUnit)
      {
        case GraphicsUnit.Display:
          throw new ApplicationException("GraphicsUnit.Display is not supported here, because no relation to pixel units can be deduced from it.");
        case GraphicsUnit.Document:
          _pointsToGU = 300.0 / 72.0;
          break;
        case GraphicsUnit.Inch:
          _pointsToGU = 1.0 / 72.0;
          break;
        case GraphicsUnit.Millimeter:
          _pointsToGU = 254.0 / 720.0;
          break;
        case GraphicsUnit.Pixel:
          _pointsToGU = _graphics.DpiY / 72.0;
          break;
        case GraphicsUnit.Point:
          _pointsToGU = 1;
          break;
        case GraphicsUnit.World:
          throw new ApplicationException("GraphicsUnit.World is not supported here, because no relation to pixel units can be deduced from it.");
      }
    }


    /**
   * calculate how many pixels per point.
   */
    public float PointsToGU(float points)
    {
      // 72 points per inch

      return (float)(points*_pointsToGU);
    }

    /**
     * calculate how many pixels per mm.
     */
    
    public float MMsToGU(float mms)
    {
      // 25.4 mm 
      return (float)(mms*_pointsToGU*(720.0 / 254.0));
    }

    /**
     * calculate how many pixels per cm.
     */
    public float CMsToGU(float cms)
    {
      return (float)(cms * _pointsToGU * (7200.0 / 254.0));
    }

    /**
     * calculate how many pixels per pica.
     */
    public float PicasToGU(float picas)
    {
      return (float)(picas *_pointsToGU * (72.0/ 12.0));
    }

    /**
     * calculate how many pixels per inch.
     */
    public float InchesToGU( float inches)
    {
      return (float)(inches * _pointsToGU * 72.0);
    }


    /**
     * calculate how many pixels per graphics unit
     */
    public float PixelsToGU(float pixels)
    {
      return (float)(pixels * _pointsToGU * 72.0 /_graphics.DpiY);
    }

    public float OnePixel { get { return PixelsToGU(1); } }
    
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
    /*
    public static IFontHandle CreateFontStatic(Graphics gr, float emHeight, bool italic, int weight, String fontName)
    {
      

      
      Font ft = new Font(fontName, emHeight, (italic ? FontStyle.Italic : FontStyle.Regular) | (weight > 500 ? FontStyle.Bold : FontStyle.Regular), GraphicsUnit.World);

      float ascending = (ft.Size * ft.FontFamily.GetCellAscent(ft.Style)) / ft.FontFamily.GetEmHeight(ft.Style);
      
      return new FontHandle(ft, emHeight, italic, weight, fontName, ascending);
    }
    */


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
    public static bool MeasureGlyph(Graphics gr, IFontHandle fonthandle, ushort index, out BoundingBox box,
      out Scaled left, out Scaled right)
    {
      bool retval = false;
      string s = string.Empty + (char)index;
      Font font = ((FontHandle)fonthandle).Handle;

      int em = font.FontFamily.GetEmHeight(font.Style);
      int asc = font.FontFamily.GetCellAscent(font.Style);
      int des = font.FontFamily.GetCellDescent(font.Style);
      float ascending = font.Size * asc / (float)em;
      float totalheight = font.Size * (asc + des) / (float)em;


      GraphicsPath path = new GraphicsPath();
      path.AddString(s, font.FontFamily, (int)font.Style, font.Size, new PointF(0, -ascending), StringFormat.GenericTypographic);
      RectangleF rect = path.GetBounds();
      
      SizeF mbox = gr.MeasureString(s, font, new PointF(0,0), StringFormat.GenericTypographic);
   
   
      //System.Diagnostics.Debug.WriteLine("Bounds=" + rect.ToString());

      box.Width = mbox.Width;
      box.Height =  - rect.Y;
      box.Depth = rect.Bottom;
      left = Math.Max(0,rect.X);
      right = Math.Max(0, mbox.Width - rect.Right);
      retval = true;




      return retval;
    }


  }
}
