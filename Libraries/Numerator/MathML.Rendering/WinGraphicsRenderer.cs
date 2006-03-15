//This file is __not__ part of MathML.Rendering, but was created by 
//Copyright (C) 2006, Dirk Lellinger
//from parts of the MathML.Rendering library
//for creating graphics without using a Control for this purpose.


using System;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.Diagnostics;
using System.Text;

namespace MathML.Rendering
{
  /// <summary>
  /// Converting of MathML to an image.
  /// </summary>
  public class WinGraphicsRenderer
  {
    // the current root element
    private MathMLMathElement mathElement = null;

    // the root of the area tree
    private Area area = null;

    // root of the formatting tree
    private Area format = null;

    private float fontSize = WinFormattingContext.DefaultFontPointSize;

    public float FontSize
    {
      get { return fontSize; }
      set { fontSize = value; }
    }

    Image BackgroundImage=null;
    public Color BackColor=Color.Transparent;

    BoundingBox box ;

    /// <summary>
    /// get or set the current mathml document
    /// Note, this documeht is live, all external updates to the document will be reflected in the 
    /// control.
    /// </summary>
    public MathMLMathElement MathElement
    {
      get { return mathElement; }
    }
    public void SetMathElement(MathMLMathElement value)
      {
        mathElement = value;
      }
     
   


    /// <summary>
    /// Draw the current mathml equation to an image object.
    /// </summary>
    /// <param name="type">The type of image to return, currently this can be
    /// either Bitmap or Metafile</param>
    /// <param name="gr">The graphics context in which this bitmap should be created.</param>
    /// <returns>A new image, null if an invalid type is given or there is no current element</returns>
    public Image GetImage(Type type, Graphics gr)
    {

      if (mathElement == null)
        return null;

        gr.PageUnit = GraphicsUnit.Pixel;

      // build the formatting tree
        MathMLFormatter formatter = new MathMLFormatter();
        WinFormattingContext ctx = new WinFormattingContext(gr, fontSize);
        format = formatter.Format(mathElement, ctx);

        // build the are tree
        box = format.BoundingBox;
        area = format.Fit(box);
      

      Image image = null;
      int height = (int)Math.Ceiling(2+box.VerticalExtent);
      int width = (int)Math.Ceiling(2+box.HorizontalExtent);

      if(type.Equals(typeof(Bitmap)))
      {
        image = new Bitmap(width, height);
      }
      else if(type.Equals(typeof(Metafile)))
      {
        IntPtr dc = gr.GetHdc();
        image = new Metafile(new MemoryStream(), dc, EmfType.EmfOnly);
        gr.ReleaseHdc(dc);
      }

      if(image != null && area != null)
      {

        using (Graphics gi = Graphics.FromImage(image))
        {
          gi.PageUnit = GraphicsUnit.Pixel;
          gi.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
          DrawWithoutFormatting(gi, width, height, 1, 1);
        }
      }

      return image;
    }

    /// <summary>
    /// draw the current mathml element (if we have one) to the given
    /// device context clipping to the given size
    /// </summary>
    private void DrawWithoutFormatting(Graphics gr, int width, int height, 
      int scrollPosX, int scrollPosY)
    {
      // clear the background in all cases
      Brush backBrush = null;
      
      if(BackgroundImage != null)
      {
        backBrush = new TextureBrush(BackgroundImage);
      }
      else if(BackColor != Color.Transparent)
      {
        backBrush = new SolidBrush(BackColor);
      }			

    
      Rectangle rect = new Rectangle(0,0,width,height);

      if(backBrush != null)
        gr.FillRectangle(backBrush, rect);

      if (area != null)
      {
        area.Render(new WinDrawingContext(gr), scrollPosX, scrollPosY + box.Height);
      }
      
      
    }


 

  }
}
