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

#nullable enable
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using Altaxo.Drawing;
using Altaxo.Geometry;

namespace Altaxo.Graph.Gdi.SyntheticBrushes
{
  public abstract class SyntheticBrushBase : ImageProxy, IHatchBrushTexture
  {
    protected const double DefaultEffectiveResolution = 300;
    protected static readonly NamedColor _defaultBackColor = NamedColors.Transparent;
    protected static readonly NamedColor _defaultForeColor = NamedColors.Black;
    protected const int MinimumBitmapPixelSize = 16;
    protected const int MaximumBitmapPixelSize = 8192;

    /// <summary>
    /// Repeat length in points (1/72 inch) in x-direction.
    /// </summary>
    protected double _repeatLengthPt = 10;

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(SyntheticBrushBase), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (SyntheticBrushBase)obj;
        info.AddValue("RepeatLength", s._repeatLengthPt);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (SyntheticBrushBase)(o ?? throw new ArgumentNullException(nameof(o)));

        s._repeatLengthPt = info.GetDouble("RepeatLength");
        return s;
      }
    }

    #endregion Serialization

    protected abstract Image GetImage(double maxEffectiveResolutionDpi, NamedColor foreColor, NamedColor backColor);

    public virtual System.IO.Stream GetContentStream(double maxEffectiveResolutionDpi, NamedColor foreColor, NamedColor backColor)
    {
      var str = new System.IO.MemoryStream();
      using (var img = GetImage(maxEffectiveResolutionDpi, foreColor, backColor))
      {
        img.Save(str, System.Drawing.Imaging.ImageFormat.Png);
      }
      return str;
    }

    public virtual System.IO.Stream GetContentStream(double maxEffectiveResolutionDpi)
    {
      return GetContentStream(maxEffectiveResolutionDpi, _defaultForeColor, _defaultBackColor);
    }

    public override System.IO.Stream GetContentStream()
    {
      return GetContentStream(DefaultEffectiveResolution);
    }

    public override VectorD2D Size
    {
      get { return new VectorD2D(_repeatLengthPt, _repeatLengthPt); }
    }

    [System.ComponentModel.Editor(typeof(Altaxo.Gui.Common.LengthValueInPointController), typeof(Altaxo.Gui.IMVCANController))]
    [Altaxo.Main.Services.PropertyReflection.DisplayOrder(1)]
    public double RepeatLength
    {
      get { return _repeatLengthPt; }
    }

    public SyntheticBrushBase WithRepeatLength(double repeatLength)
    {
      if (!(_repeatLengthPt == repeatLength))
      {
        var result = (SyntheticBrushBase)MemberwiseClone();
        result._repeatLengthPt = repeatLength;
        return result;
      }
      else
      {
        return this;
      }
    }

    protected int GetPixelDimensions(double maxEffectiveResolutionDpi)
    {
      // use a factor of 2 for safety, thus we have at least two pixels of the bitmap mapping to 1 pixel of the drawing
      int pixels = (int)((2 * InchesPerPoint) * maxEffectiveResolutionDpi * _repeatLengthPt);
      pixels = Math.Max(pixels, MinimumBitmapPixelSize);
      pixels = Math.Min(pixels, MaximumBitmapPixelSize);
      pixels = (pixels + 15) & 0xFFFFFF0; // round to multiples of 16
      return pixels;
    }

    protected int GetPixelDimensions(double maxEffectiveResolutionDpi, double relativeStructureSize, int minimumPixelsForStructureSize)
    {
      // use a factor of 2 for safety, thus we have at least two pixels of the bitmap mapping to 1 pixel of the drawing
      int pixels = (int)((2 * InchesPerPoint) * maxEffectiveResolutionDpi * _repeatLengthPt);
      pixels = Math.Max(pixels, (int)(minimumPixelsForStructureSize / relativeStructureSize));
      pixels = Math.Max(pixels, MinimumBitmapPixelSize);
      pixels = Math.Min(pixels, MaximumBitmapPixelSize);
      pixels = (pixels + 15) & 0xFFFFFF0; // round to multiples of 16
      return pixels;
    }

    public override string ContentHash
    {
      get
      {
        return GetType().ToString();
      }
    }

    public override string Name
    {
      get { return GetType().ToString(); }
    }

    public override bool IsValid
    {
      get { return true; }
    }
  }
}
