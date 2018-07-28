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

using Altaxo.Drawing;
using Altaxo.Geometry;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Altaxo.Graph.Gdi.HatchBrushes
{
  public abstract class HatchBrushBase : ImageProxy, IHatchBrushTexture
  {
    protected const double DefaultEffectiveResolution = 300;
    protected static readonly NamedColor _defaultBackColor = NamedColors.Transparent;
    protected static readonly NamedColor _defaultForeColor = NamedColors.Black;

    /// <summary>
    /// Repeat length in points (1/72 inch) in x-direction.
    /// </summary>
    protected double _repeatLengthPt = 10;

    /// <summary>
    /// Structure factor. For hatch brushes based on lines, the line width is this value times the repeat length. For shape based brushes, the size
    /// of the shape is this value times the repeat length.
    /// </summary>
    protected double _structureFactor = 0.2;

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(HatchBrushBase), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (HatchBrushBase)obj;
        info.AddValue("RepeatLength", s._repeatLengthPt);
        info.AddValue("StructureFactor", s._structureFactor);
      }

      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        var s = (HatchBrushBase)o;

        s._repeatLengthPt = info.GetDouble("RepeatLength");
        s._structureFactor = info.GetDouble("StructureFactor");

        return s;
      }
    }

    #endregion Serialization

    public abstract Image GetImage(double maxEffectiveResolutionDpi, NamedColor foreColor, NamedColor backColor);

    public override PointD2D Size
    {
      get { return new PointD2D(_repeatLengthPt, _repeatLengthPt); }
    }

    [System.ComponentModel.Editor(typeof(Altaxo.Gui.Common.LengthValueInPointController), typeof(Altaxo.Gui.IMVCANController))]
    [Altaxo.Main.Services.PropertyReflection.DisplayOrder(1)]
    public double RepeatLength
    {
      get { return _repeatLengthPt; }
      set { _repeatLengthPt = value; }
    }

    [System.ComponentModel.Editor(typeof(Altaxo.Gui.Common.RelationValueInUnityController), typeof(Altaxo.Gui.IMVCANController))]
    [Altaxo.Main.Services.PropertyReflection.DisplayOrder(2)]
    public double StructureFactor
    {
      get { return _structureFactor; }
      set { _structureFactor = value; }
    }

    protected int GetPixelDimensions(double maxEffectiveResolutionDpi)
    {
      // use a factor of 2 for safety, thus we have at least two pixels of the bitmap mapping to 1 pixel of the drawing
      int pixels = (int)((2 * InchesPerPoint) * maxEffectiveResolutionDpi * _repeatLengthPt);
      pixels = Math.Max(pixels, 16);
      pixels = Math.Min(pixels, 8192);
      pixels = (pixels + 15) & 0xFFFFFF0; // round to multiples of 16
      return pixels;
    }

    public override string ContentHash
    {
      get
      {
        return this.GetType().ToString();
      }
    }

    public override string Name
    {
      get { return GetType().ToString(); }
    }

    public override bool CopyFrom(object obj)
    {
      bool isCopied = base.CopyFrom(obj);
      if (isCopied && !object.ReferenceEquals(this, obj))
      {
        var from = obj as HatchBrushBase;
        if (null != from)
        {
          _repeatLengthPt = from._repeatLengthPt;
          _structureFactor = from._structureFactor;
        }
      }
      return isCopied;
    }

    public virtual Image GetImage(double maxEffectiveResolutionDpi)
    {
      return GetImage(maxEffectiveResolutionDpi, _defaultForeColor, _defaultBackColor);
    }

    public override Image GetImage()
    {
      return GetImage(DefaultEffectiveResolution, _defaultForeColor, _defaultBackColor);
    }

    public override bool IsValid
    {
      get { return true; }
    }
  }
}
