using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Altaxo.Graph.Gdi.Shapes
{
  using Altaxo.Graph.Scales;
  using Altaxo.Graph.Scales.Ticks;
  using Altaxo.Graph.Gdi.Plot;
  

  public class DensityImageLegend : GraphicBase
  {
    const int _bitmapPixelsX = 16;
    const int _bitmapPixelsY = 1024;

    
    TickSpacing _tickSpacing;

    /// <summary>The plot item this legend is intended for.</summary>
    DensityImagePlotItem _plotItem;

    Bitmap _bitmap;

    PenX _axisPen;

    float _majorTickLength = 4;
    float _minorTickLength = 2;


    public DensityImageLegend(DensityImagePlotItem plotItem)
    {
      _axisPen = new PenX();
      _plotItem = plotItem;
      _tickSpacing = ScaleWithTicks.CreateDefaultTicks(_plotItem.Style.Scale.GetType());
    }

    public override void Paint(System.Drawing.Graphics g, object obj)
    {
      if (null == _bitmap)
        _bitmap = new Bitmap(_bitmapPixelsX, _bitmapPixelsY, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

      // Fill the bitmap
      for (int i = 0; i < _bitmapPixelsX; i++)
        for (int j = 0; j < _bitmapPixelsY; j++)
          _bitmap.SetPixel(i, j, _plotItem.Style.ColorProvider.GetColor(j / (double)(_bitmapPixelsY - 1)));

      float bmpYPos = 0;
      float bmpXPos = 0;
      float bmpHeight = this.Height;
      float bmpWidth = this.Height / 3;

      var graphicsState = g.Save();


      g.TranslateTransform(X, Y);

      g.DrawImage(_bitmap, new RectangleF(bmpXPos, bmpYPos, bmpWidth, bmpHeight));

      // now draw the rectangle with the axis on the right side
      g.DrawRectangle(_axisPen, bmpXPos, bmpYPos, bmpWidth, bmpHeight);
      
      var major = _tickSpacing.GetMajorTicksNormal(_plotItem.Style.Scale);
      var minor = _tickSpacing.GetMinorTicksNormal(_plotItem.Style.Scale);

      for (int i = 0; i < major.Length; i++)
        g.DrawLine(_axisPen, bmpXPos + bmpWidth, (float)(bmpYPos + bmpHeight * major[i]), bmpXPos + bmpWidth + _majorTickLength, (float)(bmpYPos + bmpHeight * major[i]));

      for (int i = 0; i < minor.Length; i++)
        g.DrawLine(_axisPen, bmpXPos + bmpWidth, (float)(bmpYPos + bmpHeight * minor[i]), bmpXPos + bmpWidth + _minorTickLength, (float)(bmpYPos + bmpHeight * minor[i]));


      // now the Numbers
      

      g.Restore(graphicsState);

    }

    public override object Clone()
    {
      throw new System.NotImplementedException();
    }
  }
}
