#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
//
//    This program is free software; you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation; either version 2 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with this program; if not, write to the Free Software
//    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
//
/////////////////////////////////////////////////////////////////////////////

#endregion Copyright

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using Altaxo.Drawing;

namespace Altaxo.Graph.Gdi.SyntheticBrushes
{
  public class RandomCircles : SyntheticBrushBase
  {
    private static int _staticRandomSeed = 1;

    protected double _circleDiameterPt;

    protected double _fillingFactor;

    protected int _randomSeed;

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(RandomCircles), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (RandomCircles)obj;
        info.AddBaseValueEmbedded(obj, s.GetType().BaseType);

        info.AddValue("RandomSeed", s._randomSeed);
        info.AddValue("CircleDiameter", s._circleDiameterPt);
        info.AddValue("FillingFactor", s._fillingFactor);
      }

      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        var s = (RandomCircles)o ?? new RandomCircles();
        info.GetBaseValueEmbedded(s, s.GetType().BaseType, parent);

        s._randomSeed = info.GetInt32("RandomSeed");
        s._circleDiameterPt = info.GetDouble("CircleDiameter");
        s._fillingFactor = info.GetDouble("FillingFactor");
        return s;
      }
    }

    #endregion Serialization

    public RandomCircles()
    {
      _repeatLengthPt = 144;
      _circleDiameterPt = 1;
      _fillingFactor = 0.2;
      _randomSeed = ++_staticRandomSeed;
    }

    [System.ComponentModel.Editor(typeof(Altaxo.Gui.Common.RelationValueInUnityController), typeof(Altaxo.Gui.IMVCANController))]
    [Altaxo.Main.Services.PropertyReflection.DisplayOrder(3)]
    public double FillingFactor
    {
      get
      {
        return _fillingFactor;
      }
    }

    public RandomCircles WithFillingFactor(double value)
    {
      if (!(_fillingFactor == value))
      {
        var result = (RandomCircles)MemberwiseClone();
        result._fillingFactor = value;
        result._randomSeed = ++_staticRandomSeed;
        return result;
      }
      else
      {
        return this;
      }
    }

    [System.ComponentModel.Editor(typeof(Altaxo.Gui.Common.LengthValueInPointController), typeof(Altaxo.Gui.IMVCANController))]
    [Altaxo.Main.Services.PropertyReflection.DisplayOrder(2)]
    public double CircleDiameter
    {
      get { return _circleDiameterPt; }
    }

    public RandomCircles WithCircleDiameter(double value)
    {
      if (!(_circleDiameterPt == value))
      {
        var result = (RandomCircles)MemberwiseClone();
        result._circleDiameterPt = value;
        result._randomSeed = ++_staticRandomSeed;
        return result;
      }
      else
      {
        return this;
      }
    }





    protected override System.Drawing.Image GetImage(double maxEffectiveResolutionDpi, NamedColor foreColor, NamedColor backColor)
    {
      var randomGenerator = new Random(_randomSeed);
      int pixelDim = GetPixelDimensions(maxEffectiveResolutionDpi, _circleDiameterPt, 2);
      int circleSize = (int)(Math.Abs(_circleDiameterPt / _repeatLengthPt) * pixelDim);
      double bmpArea = ((double)pixelDim) * pixelDim;
      double circleArea = ((Math.PI / 4) * circleSize) * circleSize;

      if (circleSize <= 1)
      {
        circleSize = 1;
        circleArea = 1; // we set single pixels in this case, so every pixel has an area of 1
      }

      double numCircles = Math.Ceiling(_fillingFactor * bmpArea / circleArea);
      int nCircles = (int)Math.Max(1, Math.Min(int.MaxValue, numCircles));

      var bmp = new Bitmap(pixelDim, pixelDim, PixelFormat.Format32bppArgb);
      using (var g = Graphics.FromImage(bmp))
      {
        using (var brush = new SolidBrush(backColor))
        {
          g.FillRectangle(brush, new Rectangle(Point.Empty, bmp.Size));
        }
        g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy; // we want the foreground color to be not influenced by the background color if we have a transparent foreground color

        if (circleSize <= 1)
        {
          var gdiForeColor = (Color)foreColor;
          for (int i = 0; i < nCircles; ++i)
          {
            int x = randomGenerator.Next(pixelDim);
            int y = randomGenerator.Next(pixelDim);
            bmp.SetPixel(x, y, gdiForeColor);
          }
        }
        else
        {
          circleSize = circleSize + 1;
          using (Brush fillBrush = new SolidBrush(foreColor))
          {
            for (int i = 0; i < nCircles; ++i)
            {
              int x = randomGenerator.Next(pixelDim);
              int y = randomGenerator.Next(pixelDim);

              g.FillEllipse(fillBrush, x, y, circleSize, circleSize);

              bool transX = x + circleSize >= pixelDim;
              bool transY = y + circleSize >= pixelDim;

              if (transX)
                g.FillEllipse(fillBrush, x - pixelDim, y, circleSize, circleSize);

              if (transY)
                g.FillEllipse(fillBrush, x, y - pixelDim, circleSize, circleSize);

              if (transX && transY)
                g.FillEllipse(fillBrush, x - pixelDim, y - pixelDim, circleSize, circleSize);
            }
          }
        }
      }

      return bmp;
    }
  }
}
