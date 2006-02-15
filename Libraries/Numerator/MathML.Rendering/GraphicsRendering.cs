//This file is __not__ part of MathML.Rendering, but was created by 
//Copyright (C) 2006, Dirk Lellinger
//from parts of the MathML.Rendering library
//for creating graphics without using a Control for this purpose.
//The RTF part of this class was copied from the ExRichTextBox project by Khendys Gordon 
//(see CsExRichTextBox on http://www.codeproject.com).

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
  public class GraphicsRendering
  {
    // the current root element
    private MathMLMathElement mathElement = null;

    // the root of the area tree
    private Area area = null;

    // root of the formatting tree
    private Area format = null;        

   	private int fontSize = FormattingContext.DefaultFontPointSize;

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
      set 
      {
        mathElement = value;
        

        if(mathElement != null)
        {
          // build the formatting tree
          MathMLFormatter formatter = new MathMLFormatter();
          FormattingContext ctx = new FormattingContext(fontSize);
          format = formatter.Format(mathElement, ctx);

          // build the are tree
          box = format.BoundingBox;
          area = format.Fit(box);
        }

       
      }
     
    }


    /// <summary>
    /// Draw the current mathml equation to an image object.
    /// This method replaces the Metafile property.
    /// </summary>
    /// <param name="type">The type of image to return, currently this can be
    /// either Bitmap or Metafile</param>
    /// <returns>A new image, null if an invalid type is given or there is no current element</returns>
    public Image GetImage(Type type)
    {
      Image image = null;
      int height = (int)Math.Ceiling(box.VerticalExtent);
      int width = (int)Math.Ceiling(box.HorizontalExtent);

      if(type.Equals(typeof(Bitmap)))
      {
        image = new Bitmap(width, height);
      }
      else if(type.Equals(typeof(Metafile)))
      {
        IntPtr screenDc = Win32.GetDC(IntPtr.Zero);
        image = new Metafile(new MemoryStream(), screenDc, EmfType.EmfOnly);
        Win32.ReleaseDC(IntPtr.Zero, screenDc);
      }

      if(image != null && area != null)
      {
        IntPtr screenDc = Win32.GetDC(IntPtr.Zero);			
        IntPtr mathBmpDc = Win32.CreateCompatibleDC(screenDc);	
        IntPtr mathBmpHandle = Win32.CreateCompatibleBitmap(screenDc, width, height);	
        Win32.ReleaseDC(IntPtr.Zero, screenDc);						

        Win32.SetBkMode(mathBmpDc, Win32.TRANSPARENT);							

        IntPtr stockHandle = Win32.SelectObject(mathBmpDc, mathBmpHandle);	

        Graphics gMF =  Graphics.FromImage(image);
        IntPtr hMF = gMF.GetHdc();
        //Draw(mathBmpDc, width, height, 0, 0);						
        Draw(hMF, width, height, 0, 0);						
        gMF.ReleaseHdc(hMF);
        gMF.Dispose();

        /*
        Graphics g = Graphics.FromImage(image);				

        g.FillRectangle(new SolidBrush(Color.White), 0, 0, width, height);
				
        IntPtr dc = g.GetHdc();

        Win32.BitBlt(dc, 0, 0, width, height, mathBmpDc, 0, 0, Win32.SRCCOPY);				

        g.ReleaseHdc(dc);

        g.Dispose();
        */

        Win32.SelectObject(mathBmpDc, stockHandle);
        Win32.DeleteObject(mathBmpHandle);
        Win32.DeleteDC(mathBmpDc);
        
      }

      return image;
    }

    /// <summary>
    /// draw the current mathml element (if we have one) to the given
    /// device context clipping to the given size
    /// </summary>
    private void Draw(IntPtr dc, int width, int height, 
      int scrollPosX, int scrollPosY)
    {
      Debug.WriteLine("Draw(width: " + width + ", height: " + height + ")");

      // clear the background in all cases
      IntPtr backBrush = IntPtr.Zero;
      IntPtr bmpHandle = IntPtr.Zero;
      if(BackgroundImage != null)
      {
        Bitmap bmp = BackgroundImage as Bitmap;
        bmpHandle = bmp != null ? bmp.GetHbitmap(BackColor) : new Bitmap(BackgroundImage).GetHbitmap(BackColor);
        backBrush = Win32.CreatePatternBrush(bmpHandle);
      }
      else
      {
        
        backBrush = Win32.CreateSolidBrush(Win32.RGB(BackColor));
      }			
      IntPtr oldBrush = Win32.SelectObject(dc, backBrush);
      Win32.RECT rect = new Win32.RECT();
      rect.left = 0; rect.top = 0; rect.right = width; rect.bottom = height;
      Win32.FillRect(dc, ref rect, backBrush);
      Win32.SelectObject(dc, oldBrush);
      Win32.DeleteObject(backBrush);
      if(bmpHandle != IntPtr.Zero)
      {
        Win32.DeleteObject(bmpHandle);
      }

      if(area != null)
      {								
        // save the text align mode, and set it to baseline
        uint textAlign = Win32.SetTextAlign(dc, Win32.TA_BASELINE);	
				
   
			
        Win32.SetBkMode(dc, Win32.TRANSPARENT);	

        // draw the mathml elememt to the backbuffer
        GraphicDevice graphics = new GraphicDevice(dc);				
        area.Render(graphics, scrollPosX, scrollPosY + box.Height);
      }
    }


    #region Rich Text File Export

   
 



    // Specifies the flags/options for the unmanaged call to the GDI+ method
    // Metafile.EmfToWmfBits().
    private enum EmfToWmfBitsFlags
    {

      // Use the default conversion
      EmfToWmfBitsFlagsDefault = 0x00000000,

      // Embedded the source of the EMF metafiel within the resulting WMF
      // metafile
      EmfToWmfBitsFlagsEmbedEmf = 0x00000001,

      // Place a 22-byte header in the resulting WMF file.  The header is
      // required for the metafile to be considered placeable.
      EmfToWmfBitsFlagsIncludePlaceable = 0x00000002,

      // Don't simulate clipping by using the XOR operator.
      EmfToWmfBitsFlagsNoXORClip = 0x00000004
    };

    /// <summary>
    /// Use the EmfToWmfBits function in the GDI+ specification to convert a 
    /// Enhanced Metafile to a Windows Metafile
    /// </summary>
    /// <param name="_hEmf">
    /// A handle to the Enhanced Metafile to be converted
    /// </param>
    /// <param name="_bufferSize">
    /// The size of the buffer used to store the Windows Metafile bits returned
    /// </param>
    /// <param name="_buffer">
    /// An array of bytes used to hold the Windows Metafile bits returned
    /// </param>
    /// <param name="_mappingMode">
    /// The mapping mode of the image.  This control uses MM_ANISOTROPIC.
    /// </param>
    /// <param name="_flags">
    /// Flags used to specify the format of the Windows Metafile returned
    /// </param>
    [System.Runtime.InteropServices.DllImportAttribute("gdiplus.dll")]
    private static extern uint GdipEmfToWmfBits(IntPtr _hEmf, uint _bufferSize,
      byte[] _buffer, int _mappingMode, EmfToWmfBitsFlags _flags);


    // Allows the x-coordinates and y-coordinates of the metafile to be adjusted
    // independently
    private const int MM_ANISOTROPIC = 8;

    /// <summary>
    /// Wraps the image in an Enhanced Metafile by drawing the image onto the
    /// graphics context, then converts the Enhanced Metafile to a Windows
    /// Metafile, and finally appends the bits of the Windows Metafile in HEX
    /// to a string and returns the string.
    /// </summary>
    /// <param name="_image"></param>
    /// <returns>
    /// A string containing the bits of a Windows Metafile in HEX
    /// </returns>
    public string GetRtfImage(Image _image)
    {

      StringBuilder _rtf = null;

      // Used to store the enhanced metafile
      MemoryStream _stream = null;

      // Used to create the metafile and draw the image
      Graphics _graphics = null;

      // The enhanced metafile
      Metafile _metaFile = null;

      // Handle to the device context used to create the metafile
      IntPtr _hdc;

      try
      {
        _rtf = new StringBuilder();
        _stream = new MemoryStream();

        // Get the device context from the graphics context
        _hdc = Win32.GetDC(IntPtr.Zero);



        // Create a new Enhanced Metafile from the device context
        _metaFile = new Metafile(_stream, _hdc);

        // Release the device context
        Win32.ReleaseDC(IntPtr.Zero, _hdc);


        // Get a graphics context from the Enhanced Metafile
        using (_graphics = Graphics.FromImage(_metaFile))
        {

          // Draw the image on the Enhanced Metafile
          _graphics.DrawImage(_image, new Rectangle(0, 0, _image.Width, _image.Height));

        }

        // Get the handle of the Enhanced Metafile
        IntPtr _hEmf = _metaFile.GetHenhmetafile();

        // A call to EmfToWmfBits with a null buffer return the size of the
        // buffer need to store the WMF bits.  Use this to get the buffer
        // size.
        uint _bufferSize = GdipEmfToWmfBits(_hEmf, 0, null, MM_ANISOTROPIC,
          EmfToWmfBitsFlags.EmfToWmfBitsFlagsDefault);

        // Create an array to hold the bits
        byte[] _buffer = new byte[_bufferSize];

        // A call to EmfToWmfBits with a valid buffer copies the bits into the
        // buffer an returns the number of bits in the WMF.  
        uint _convertedSize = GdipEmfToWmfBits(_hEmf, _bufferSize, _buffer, MM_ANISOTROPIC,
          EmfToWmfBitsFlags.EmfToWmfBitsFlagsDefault);

        // Append the bits to the RTF string
        for (int i = 0; i < _buffer.Length; ++i)
        {
          _rtf.Append(String.Format("{0:X2}", _buffer[i]));
        }

        return _rtf.ToString();
      }
      finally
      {
        if (_graphics != null)
          _graphics.Dispose();
        if (_metaFile != null)
          _metaFile.Dispose();
        if (_stream != null)
          _stream.Close();
      }
    }
    #endregion

  }
}
