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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;

namespace Altaxo.Graph.Gdi
{
  [Serializable]
  public enum BrushType
  {
    SolidBrush,
    HatchBrush,
    TextureBrush,
    LinearGradientBrush,
    PathGradientBrush,
    SigmaBellShapeLinearGradientBrush,
    TriangularShapeLinearGradientBrush,
    SigmaBellShapePathGradientBrush,
    TriangularShapePathGradientBrush,
    SyntheticTextureBrush,
  };

  [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.BrushType", 0)]
  [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(BrushType), 1)]
  public class BrushTypeXmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
  {
    public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
    {
      info.SetNodeContent(obj.ToString());
    }

    public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
    {
      string val = info.GetNodeContent();
      return System.Enum.Parse(typeof(BrushType), val, true);
    }
  }

  [Serializable]
  public enum LinearGradientShape
  {
    Linear,
    Triangular,
    SigmaBell
  }

  [Serializable]
  public enum LinearGradientModeEx
  {
    BackwardDiagonal,
    ForwardDiagonal,
    Horizontal,
    Vertical,
    RevBackwardDiagonal,
    RevForwardDiagonal,
    RevHorizontal,
    RevVertical
  }

  /// <summary>
  /// Holds all information neccessary to create a brush
  /// of any kind without allocating resources, so this class
  /// can be made serializable.
  /// </summary>
  [Serializable]
  public class BrushX
    :
    Main.SuspendableDocumentLeafNodeWithEventArgs,
    System.ICloneable, System.IDisposable, IEquatable<BrushX>
  {
    protected BrushType _brushType; // Type of the brush
    protected NamedColor _foreColor; // Color of the brush
    protected NamedColor _backColor = NamedColors.Transparent; // Backcolor of brush, f.i.f. HatchStyle brushes
    protected bool _exchangeColors;
    protected WrapMode _wrapMode; // für TextureBrush und LinearGradientBrush
    protected double _angle;
    protected double _offsetX;
    protected double _offsetY;
    protected double _gradientColorScale;
    protected ImageProxy _textureImage; // für Texturebrush
    protected TextureScaling _textureScale = TextureScaling.Default;

    protected RectangleD2D _brushBoundingRectangle;

    [NonSerialized]
    protected Brush _cachedBrush;      // this is the cached brush object

    /// <summary>Cached effective maximum resolution in dots per inch. Important for repeateable texture brushes only.</summary>
    [NonSerialized]
    protected double _cachedEffectiveMaximumResolutionDpi = 96;

    #region "Serialization"

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.BrushHolder", 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        throw new NotSupportedException("Serialization of old versions is not supported");
        /*
				BrushX s = (BrushX)obj;
				info.AddValue("Type", s._brushType);
				switch (s._brushType)
				{
					case BrushType.SolidBrush:
						info.AddValue("ForeColor", s._foreColor);
						break;

					case BrushType.HatchBrush:
						info.AddValue("ForeColor", s._foreColor);
						info.AddValue("BackColor", s._backColor);
						info.AddEnum("HatchStyle", s._hatchStyle);
						break;
				} // end of switch
				*/
      }

      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        BrushX s = null != o ? (BrushX)o : new BrushX(NamedColors.Black);

        s._brushType = (BrushType)info.GetValue("Type", s);
        switch (s._brushType)
        {
          case BrushType.SolidBrush:
            s._foreColor = (NamedColor)info.GetValue("ForeColor", s);
            break;

          case BrushType.HatchBrush:
            s._foreColor = (NamedColor)info.GetValue("ForeColor", s);
            s._backColor = (NamedColor)info.GetValue("BackColor", s);
            var hatchStyle = (HatchStyle)info.GetEnum("HatchStyle", typeof(HatchStyle));
            s._textureImage = BrushX.HatchStyleToImage(hatchStyle);
            break;
        }

        if (s.ParentObject == null)
          s.ParentObject = parent as Main.IDocumentNode;

        return s;
      }
    }

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.BrushHolder", 1)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(BrushX), 2)]
    private class XmlSerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        throw new NotSupportedException("Serialization of old versions is not supported");
        /*
				BrushX s = (BrushX)obj;
				info.AddValue("Type", s._brushType);
				switch (s._brushType)
				{
					case BrushType.SolidBrush:
						info.AddValue("ForeColor", s._foreColor);
						break;

					case BrushType.HatchBrush:
						info.AddValue("ForeColor", s._foreColor);
						info.AddValue("BackColor", s._backColor);
						info.AddEnum("HatchStyle", s._hatchStyle);
						break;

					case BrushType.LinearGradientBrush:
						info.AddValue("ForeColor", s._foreColor);
						info.AddValue("BackColor", s._backColor);
						info.AddEnum("WrapMode", s._wrapMode);
						info.AddEnum("GradientMode", s._gradientMode);
						info.AddEnum("GradientShape", s._gradientShape);
						info.AddValue("Scale", s._gradientColorScale);
						info.AddValue("Focus", s._gradientFocus);
						break;

					case BrushType.PathGradientBrush:
						info.AddValue("ForeColor", s._foreColor);
						info.AddValue("BackColor", s._backColor);
						info.AddEnum("WrapMode", s._wrapMode);
						break;

					case BrushType.TextureBrush:
						info.AddValue("Texture", s._textureImage);
						info.AddEnum("WrapMode", s._wrapMode);
						info.AddValue("Scale", s._gradientColorScale);
						break;
				} // end of switch
				*/
      }

      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        BrushX s = null != o ? (BrushX)o : new BrushX(NamedColors.Black);

        s._brushType = (BrushType)info.GetValue("Type", s);
        switch (s._brushType)
        {
          case BrushType.SolidBrush:
            s._foreColor = (NamedColor)info.GetValue("ForeColor", s);
            break;

          case BrushType.HatchBrush:
            s._foreColor = (NamedColor)info.GetValue("ForeColor", s);
            s._backColor = (NamedColor)info.GetValue("BackColor", s);
            var hatchStyle = (HatchStyle)info.GetEnum("HatchStyle", typeof(HatchStyle));
            s._textureImage = BrushX.HatchStyleToImage(hatchStyle);
            break;

          case BrushType.LinearGradientBrush:
            s._foreColor = (NamedColor)info.GetValue("ForeColor", s);
            s._backColor = (NamedColor)info.GetValue("BackColor", s);
            s._wrapMode = (WrapMode)info.GetEnum("WrapMode", typeof(WrapMode));
            LinearGradientModeEx gm = (LinearGradientModeEx)info.GetEnum("GradientMode", typeof(LinearGradientModeEx));
            string gmname = Enum.GetName(typeof(LinearGradientModeEx), gm);
            if (gmname.StartsWith("Rev"))
            {
              s._exchangeColors = true;
              s._angle = 180 + BrushX.LinearGradientModeToAngle((LinearGradientMode)Enum.Parse(typeof(LinearGradientMode), gmname.Substring(3)));
            }
            else
            {
              s._angle = BrushX.LinearGradientModeToAngle((LinearGradientMode)Enum.Parse(typeof(LinearGradientMode), gmname));
            }

            var gradientShape = (LinearGradientShape)info.GetEnum("GradientShape", typeof(LinearGradientShape));
            if (gradientShape == LinearGradientShape.SigmaBell)
              s._brushType = BrushType.SigmaBellShapeLinearGradientBrush;
            else if (gradientShape == LinearGradientShape.Triangular)
              s._brushType = BrushType.TriangularShapeLinearGradientBrush;

            s._gradientColorScale = info.GetSingle("Scale");
            s._offsetX = info.GetSingle("Focus");
            break;

          case BrushType.PathGradientBrush:
            s._foreColor = (NamedColor)info.GetValue("ForeColor", s);
            s._backColor = (NamedColor)info.GetValue("BackColor", s);
            s._wrapMode = (WrapMode)info.GetEnum("WrapMode", typeof(WrapMode));
            s._offsetX = 0.5;
            s._offsetY = 0.5;
            break;

          case BrushType.TextureBrush:
            s.TextureImage = (ImageProxy)info.GetValue("Texture", s);
            s._wrapMode = (WrapMode)info.GetEnum("WrapMode", typeof(WrapMode));
            var scale = info.GetSingle("Scale");
            s._textureScale = new TextureScaling(TextureScalingMode.Source, AspectRatioPreservingMode.PreserveXPriority, scale, scale);
            break;
        }

        if (s.ParentObject == null)
          s.ParentObject = parent as Main.IDocumentNode;

        return s;
      }
    }

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(BrushX), 3)]
    private class XmlSerializationSurrogate3 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        throw new NotSupportedException("Serialization of old versions is not supported");
        /*
				BrushX s = (BrushX)obj;
				info.AddValue("Type", s._brushType);
				switch (s._brushType)
				{
					case BrushType.SolidBrush:
						info.AddValue("ForeColor", s._foreColor);
						break;

					case BrushType.HatchBrush:
						info.AddValue("ForeColor", s._foreColor);
						info.AddValue("BackColor", s._backColor);
						info.AddValue("ExchangeColors", s._exchangeColors);
						info.AddEnum("HatchStyle", s._hatchStyle);
						break;

					case BrushType.LinearGradientBrush:
						info.AddValue("ForeColor", s._foreColor);
						info.AddValue("BackColor", s._backColor);
						info.AddValue("ExchangeColors", s._exchangeColors);
						info.AddEnum("WrapMode", s._wrapMode);
						info.AddEnum("GradientMode", s._gradientMode);
						info.AddEnum("GradientShape", s._gradientShape);
						info.AddValue("Scale", s._gradientColorScale);
						info.AddValue("Focus", s._gradientFocus);
						break;

					case BrushType.PathGradientBrush:
						info.AddValue("ForeColor", s._foreColor);
						info.AddValue("BackColor", s._backColor);
						info.AddValue("ExchangeColors", s._exchangeColors);
						info.AddEnum("WrapMode", s._wrapMode);
						break;

					case BrushType.TextureBrush:
						info.AddValue("Texture", s._textureImage);
						info.AddEnum("WrapMode", s._wrapMode);
						info.AddValue("Scale", s._gradientColorScale);
						break;
				} // end of switch
				*/
      }

      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        BrushX s = null != o ? (BrushX)o : new BrushX(NamedColors.Black);

        s._brushType = (BrushType)info.GetValue("Type", s);
        switch (s._brushType)
        {
          case BrushType.SolidBrush:
            s._foreColor = (NamedColor)info.GetValue("ForeColor", s);
            break;

          case BrushType.HatchBrush:
            s._foreColor = (NamedColor)info.GetValue("ForeColor", s);
            s._backColor = (NamedColor)info.GetValue("BackColor", s);
            s._exchangeColors = info.GetBoolean("ExchangeColors");
            var hatchStyle = (HatchStyle)info.GetEnum("HatchStyle", typeof(HatchStyle));
            s._textureImage = BrushX.HatchStyleToImage(hatchStyle);
            break;

          case BrushType.LinearGradientBrush:
            s._foreColor = (NamedColor)info.GetValue("ForeColor", s);
            s._backColor = (NamedColor)info.GetValue("BackColor", s);
            s._exchangeColors = info.GetBoolean("ExchangeColors");
            s._wrapMode = (WrapMode)info.GetEnum("WrapMode", typeof(WrapMode));
            s._angle = BrushX.LinearGradientModeToAngle((LinearGradientMode)info.GetEnum("GradientMode", typeof(LinearGradientMode)));
            var gradientShape = (LinearGradientShape)info.GetEnum("GradientShape", typeof(LinearGradientShape));
            if (gradientShape == LinearGradientShape.SigmaBell)
              s._brushType = BrushType.SigmaBellShapeLinearGradientBrush;
            else if (gradientShape == LinearGradientShape.Triangular)
              s._brushType = BrushType.TriangularShapeLinearGradientBrush;

            s._gradientColorScale = info.GetSingle("Scale");
            s._offsetX = info.GetSingle("Focus");
            break;

          case BrushType.PathGradientBrush:
            s._foreColor = (NamedColor)info.GetValue("ForeColor", s);
            s._backColor = (NamedColor)info.GetValue("BackColor", s);
            s._exchangeColors = info.GetBoolean("ExchangeColors");
            s._wrapMode = (WrapMode)info.GetEnum("WrapMode", typeof(WrapMode));
            s._offsetX = 0.5;
            s._offsetY = 0.5;
            break;

          case BrushType.TextureBrush:
            s.TextureImage = (ImageProxy)info.GetValue("Texture", s);
            s._wrapMode = (WrapMode)info.GetEnum("WrapMode", typeof(WrapMode));
            var scale = info.GetSingle("Scale");
            s._textureScale = new TextureScaling(TextureScalingMode.Source, AspectRatioPreservingMode.PreserveXPriority, scale, scale);
            break;
        }

        if (s.ParentObject == null)
          s.ParentObject = parent as Main.IDocumentNode;

        return s;
      }
    }

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(BrushX), 4)] // 2012-02-14
    private class XmlSerializationSurrogate4 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        BrushX s = (BrushX)obj;
        info.AddValue("Type", s._brushType);
        switch (s._brushType)
        {
          case BrushType.SolidBrush:
            info.AddValue("ForeColor", s._foreColor);
            break;

          case BrushType.LinearGradientBrush:
          case BrushType.SigmaBellShapeLinearGradientBrush:
          case BrushType.TriangularShapeLinearGradientBrush:
            info.AddValue("ForeColor", s._foreColor);
            info.AddValue("BackColor", s._backColor);
            info.AddValue("ExchangeColors", s._exchangeColors);
            info.AddEnum("WrapMode", s._wrapMode);
            info.AddValue("Angle", s._angle);
            if (s._brushType != BrushType.LinearGradientBrush)
            {
              info.AddValue("Focus", s._offsetX);
              info.AddValue("ColorScale", s._gradientColorScale);
            }
            break;

          case BrushType.PathGradientBrush:
          case BrushType.SigmaBellShapePathGradientBrush:
          case BrushType.TriangularShapePathGradientBrush:
            info.AddValue("ForeColor", s._foreColor);
            info.AddValue("BackColor", s._backColor);
            info.AddValue("ExchangeColors", s._exchangeColors);
            info.AddEnum("WrapMode", s._wrapMode);
            info.AddValue("FocusX", s._offsetX);
            info.AddValue("FocusY", s._offsetY);
            if (s._brushType != BrushType.PathGradientBrush)
              info.AddValue("ColorScale", s._gradientColorScale);
            break;

          case BrushType.HatchBrush: // 2012-02-14 HatchBrush is not a brush that relies on an ImageProxy
          case BrushType.SyntheticTextureBrush:
            info.AddValue("ForeColor", s._foreColor);
            info.AddValue("BackColor", s._backColor);
            info.AddValue("ExchangeColors", s._exchangeColors);
            info.AddEnum("WrapMode", s._wrapMode);
            info.AddValue("Angle", s._angle);
            info.AddValue("Scale", s._textureScale);
            info.AddValue("OffsetX", s._offsetX);
            info.AddValue("OffsetY", s._offsetY);
            info.AddValue("Texture", s._textureImage);
            break;

          case BrushType.TextureBrush:
            info.AddEnum("WrapMode", s._wrapMode);
            info.AddValue("Angle", s._angle);
            info.AddValue("Scale", s._textureScale);
            info.AddValue("OffsetX", s._offsetX);
            info.AddValue("OffsetY", s._offsetY);
            info.AddValue("Texture", s._textureImage);
            break;
        } // end of switch
      }

      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        BrushX s = null != o ? (BrushX)o : new BrushX(NamedColors.Black);

        s._brushType = (BrushType)info.GetValue("Type", s);
        switch (s._brushType)
        {
          case BrushType.SolidBrush:
            s._foreColor = (NamedColor)info.GetValue("ForeColor", s);
            break;

          case BrushType.LinearGradientBrush:
          case BrushType.SigmaBellShapeLinearGradientBrush:
          case BrushType.TriangularShapeLinearGradientBrush:
            s._foreColor = (NamedColor)info.GetValue("ForeColor", s);
            s._backColor = (NamedColor)info.GetValue("BackColor", s);
            s._exchangeColors = info.GetBoolean("ExchangeColors");
            s._wrapMode = (WrapMode)info.GetEnum("WrapMode", typeof(WrapMode));
            s._angle = info.GetDouble("Angle");
            if (s._brushType != BrushType.LinearGradientBrush)
            {
              s._offsetX = info.GetDouble("Focus");
              s._gradientColorScale = info.GetDouble("ColorScale");
            }
            break;

          case BrushType.PathGradientBrush:
          case BrushType.SigmaBellShapePathGradientBrush:
          case BrushType.TriangularShapePathGradientBrush:
            s._foreColor = (NamedColor)info.GetValue("ForeColor", s);
            s._backColor = (NamedColor)info.GetValue("BackColor", s);
            s._exchangeColors = info.GetBoolean("ExchangeColors");
            s._wrapMode = (WrapMode)info.GetEnum("WrapMode", typeof(WrapMode));
            s._offsetX = info.GetDouble("FocusX");
            s._offsetY = info.GetDouble("FocusY");
            if (s._brushType != BrushType.PathGradientBrush)
              s._gradientColorScale = info.GetDouble("ColorScale");
            break;

          case BrushType.SyntheticTextureBrush:
          case BrushType.HatchBrush:
            s._foreColor = (NamedColor)info.GetValue("ForeColor", s);
            s._backColor = (NamedColor)info.GetValue("BackColor", s);
            s._exchangeColors = info.GetBoolean("ExchangeColors");
            s._wrapMode = (WrapMode)info.GetEnum("WrapMode", typeof(WrapMode));
            s._angle = info.GetDouble("Angle");
            s._textureScale = (TextureScaling)info.GetValue("Scale", s);
            s._offsetX = info.GetDouble("OffsetX");
            s._offsetY = info.GetDouble("OffsetY");
            s.TextureImage = (ImageProxy)info.GetValue("Texture", s);
            break;

          case BrushType.TextureBrush:
            s._wrapMode = (WrapMode)info.GetEnum("WrapMode", typeof(WrapMode));
            s._angle = info.GetDouble("Angle");
            s._textureScale = (TextureScaling)info.GetValue("Scale", s);
            s._offsetX = info.GetDouble("OffsetX");
            s._offsetY = info.GetDouble("OffsetY");
            s.TextureImage = (ImageProxy)info.GetValue("Texture", s);
            break;
        }

        if (s.ParentObject == null)
          s.ParentObject = parent as Main.IDocumentNode;

        return s;
      }
    }

    #endregion "Serialization"

    private static bool SafeEquals(object x, object y)
    {
      if (null != x && null != y)
        return x.Equals(y);
      else
        return object.ReferenceEquals(x, y);
    }

    public bool Equals(BrushX other)
    {
      if (object.ReferenceEquals(this, other))
        return true;

      if (this._brushType != other._brushType)
        return false;

      switch (this.BrushType)
      {
        case Gdi.BrushType.SolidBrush:
          if (!this._foreColor.Equals(other._foreColor))
            return false;
          break;

        case BrushType.LinearGradientBrush:
        case BrushType.TriangularShapeLinearGradientBrush:
        case BrushType.SigmaBellShapeLinearGradientBrush:
          if (this._exchangeColors != other._exchangeColors)
            return false;
          if (!this._foreColor.Equals(other._foreColor))
            return false;
          if (!this._backColor.Equals(other._backColor))
            return false;
          if (this._angle != other._angle)
            return false;
          if (this._wrapMode != other._wrapMode)
            return false;
          if (this._brushType != Gdi.BrushType.LinearGradientBrush)
          {
            if (this._offsetX != other._offsetX)
              return false;
            if (this._gradientColorScale != other._gradientColorScale)
              return false;
          }
          break;

        case BrushType.PathGradientBrush:
        case BrushType.TriangularShapePathGradientBrush:
        case BrushType.SigmaBellShapePathGradientBrush:
          if (this._exchangeColors != other._exchangeColors)
            return false;
          if (!this._foreColor.Equals(other._foreColor))
            return false;
          if (!this._backColor.Equals(other._backColor))
            return false;
          if (this._wrapMode != other._wrapMode)
            return false;
          if (this._brushType != BrushType.PathGradientBrush)
          {
            if (this._gradientColorScale != other._gradientColorScale)
              return false;
          }
          if (this._offsetX != other._offsetX)
            return false;
          if (this._offsetY != other._offsetY)
            return false;
          break;

        case BrushType.HatchBrush:
        case BrushType.SyntheticTextureBrush:
        case BrushType.TextureBrush:
          if (_brushType == BrushType.HatchBrush)
          {
            if (this._exchangeColors != other._exchangeColors)
              return false;
            if (!this._foreColor.Equals(other._foreColor))
              return false;
            if (!this._backColor.Equals(other._backColor))
              return false;
          }
          if (!SafeEquals(this._textureImage, other._textureImage))
            return false;
          if (this._textureScale != other._textureScale)
            return false;
          if (this._wrapMode != other._wrapMode)
            return false;
          if (this._angle != other._angle)
            return false;
          if (this._offsetX != other._offsetX)
            return false;
          if (this._offsetY != other._offsetY)
            return false;
          break;

        default:
          throw new NotImplementedException("BrushType not implemented:" + _brushType.ToString());
      }
      return true;
    }

    public override bool Equals(object obj)
    {
      var other = obj as BrushX;
      if (null != other)
        return Equals(other);
      else
        return false;
    }

    public override int GetHashCode()
    {
      return _brushType.GetHashCode() + 13 * _foreColor.GetHashCode();
    }

    public BrushX(BrushX from)
    {
      _brushType = from._brushType; // Type of the brush
      _foreColor = from._foreColor; // Color of the brush
      _backColor = from._backColor; // Backcolor of brush, f.i.f. HatchStyle brushes
      _exchangeColors = from._exchangeColors;
      _wrapMode = from._wrapMode; // für TextureBrush und LinearGradientBrush
      _angle = from._angle;
      _offsetX = from._offsetX;
      _offsetY = from._offsetY;
      _gradientColorScale = from._gradientColorScale;
      _textureImage = null == from._textureImage ? null : (ImageProxy)from._textureImage.Clone(); // für Texturebrush
      _textureScale = from._textureScale;

      _brushBoundingRectangle = from._brushBoundingRectangle;
      _cachedBrush = null;      // this is the cached brush object
    }

    public BrushX(NamedColor c)
    {
      this._brushType = BrushType.SolidBrush;
      this._foreColor = c;
    }

    public static implicit operator System.Drawing.Brush(BrushX bh)
    {
      return bh == null ? null : bh.Brush;
    }

    public static ImageProxy DefaultTextureBrush
    {
      get
      {
        var pair = Altaxo.Graph.TextureManager.BuiltinTextures.FirstOrDefault();
        if (null != pair.Value)
          return pair.Value;
        return new HatchBrushes.HorizontalHatchBrush();
      }
    }

    public static ImageProxy DefaultHatchBrush
    {
      get
      {
        return new HatchBrushes.HorizontalHatchBrush();
      }
    }

    public static ImageProxy DefaultSyntheticBrush
    {
      get
      {
        return new SyntheticBrushes.RandomCircles();
      }
    }

    /// <summary>Invalidates the cached brush.</summary>
    public void InvalidateCachedBrush()
    {
      _SetBrushVariable(null);
    }

    public BrushType BrushType
    {
      get
      {
        return this._brushType;
      }
      set
      {
        BrushType oldValue = this._brushType;
        _brushType = value;
        if (_brushType != oldValue)
        {
          _SetBrushVariable(null);
          OnBrushTypeChanged(oldValue, value);
          EhSelfChanged(EventArgs.Empty);
        }
      }
    }

    /// <summary>
    /// Intended to initialize some brush variables to default values if the brush type changed.
    /// </summary>
    /// <param name="oldValue">Old brush type.</param>
    /// <param name="newValue">New brush type.</param>
    protected virtual void OnBrushTypeChanged(BrushType oldValue, BrushType newValue)
    {
      bool disposeTexture = false;
      switch (newValue)
      {
        case Gdi.BrushType.SolidBrush:
          disposeTexture = true;
          break;

        case Gdi.BrushType.LinearGradientBrush:
          _wrapMode = System.Drawing.Drawing2D.WrapMode.TileFlipXY;
          disposeTexture = true;
          break;

        case Gdi.BrushType.TriangularShapeLinearGradientBrush:
          _wrapMode = System.Drawing.Drawing2D.WrapMode.TileFlipXY;
          _offsetX = 0.5;
          _gradientColorScale = 1;
          disposeTexture = true;
          break;

        case Gdi.BrushType.SigmaBellShapeLinearGradientBrush:
          _wrapMode = System.Drawing.Drawing2D.WrapMode.TileFlipXY;
          _offsetX = 0.5;
          _gradientColorScale = 1;
          disposeTexture = true;
          break;

        case Gdi.BrushType.PathGradientBrush:
          _wrapMode = System.Drawing.Drawing2D.WrapMode.TileFlipXY;
          _offsetX = 0.5;
          _offsetY = 0.5;
          disposeTexture = true;
          break;

        case Gdi.BrushType.TriangularShapePathGradientBrush:
        case Gdi.BrushType.SigmaBellShapePathGradientBrush:
          _wrapMode = System.Drawing.Drawing2D.WrapMode.TileFlipXY;
          _offsetX = 0.5;
          _offsetY = 0.5;
          _gradientColorScale = 1;
          disposeTexture = true;
          break;

        case Gdi.BrushType.HatchBrush:
          _wrapMode = System.Drawing.Drawing2D.WrapMode.Tile;
          _offsetX = 0;
          _offsetY = 0;
          _textureImage = DefaultHatchBrush;
          break;

        case Gdi.BrushType.SyntheticTextureBrush:
          _wrapMode = System.Drawing.Drawing2D.WrapMode.Tile;
          _offsetX = 0;
          _offsetY = 0;
          _textureImage = DefaultSyntheticBrush;
          break;

        case Gdi.BrushType.TextureBrush:
          _wrapMode = System.Drawing.Drawing2D.WrapMode.Tile;
          _offsetX = 0;
          _offsetY = 0;
          _textureImage = DefaultTextureBrush;
          break;
      }

      if (disposeTexture)
      {
        if (_textureImage is IDisposable)
          ((IDisposable)_textureImage).Dispose();
        _textureImage = null;
      }
    }

    /// <summary>
    /// Returns true if the brush is visible, i.e. is not a transparent brush.
    /// </summary>
    public bool IsVisible
    {
      get
      {
        return !(_brushType == BrushType.SolidBrush && _foreColor.Color.A == 0);
      }
    }

    /// <summary>
    /// Returns true if the brush is invisible, i.e. is a solid and transparent brush.
    /// </summary>
    public bool IsInvisible
    {
      get
      {
        return _brushType == BrushType.SolidBrush && _foreColor.Color.A == 0;
      }
    }

    public NamedColor Color
    {
      get { return _foreColor; }
      set
      {
        bool bChanged = !_foreColor.Equals(value);
        _foreColor = value;
        if (bChanged)
        {
          _SetBrushVariable(null);
          EhSelfChanged(EventArgs.Empty);
        }
      }
    }

    public NamedColor BackColor
    {
      get { return _backColor; }
      set
      {
        bool bChanged = !_backColor.Equals(value);
        _backColor = value;
        if (bChanged)
        {
          _SetBrushVariable(null);
          EhSelfChanged(EventArgs.Empty);
        }
      }
    }

    public bool ExchangeColors
    {
      get
      {
        return _exchangeColors;
      }
      set
      {
        bool oldValue = _exchangeColors;
        _exchangeColors = value;
        if (value != oldValue)
        {
          _SetBrushVariable(null);
          EhSelfChanged(EventArgs.Empty);
        }
      }
    }

    public WrapMode WrapMode
    {
      get
      {
        return _wrapMode;
      }
      set
      {
        bool bChanged = (_wrapMode != value);
        _wrapMode = value;
        if (bChanged)
        {
          _SetBrushVariable(null);
          EhSelfChanged(EventArgs.Empty);
        }
      }
    }

    public double GradientAngle
    {
      get
      {
        return _angle;
      }
      set
      {
        bool bChanged = (_angle != value);
        _angle = value;
        if (bChanged)
        {
          _SetBrushVariable(null);
          EhSelfChanged(EventArgs.Empty);
        }
      }
    }

    public double GradientFocus
    {
      get
      {
        return _offsetX;
      }
      set
      {
        bool bChanged = (_offsetX != value);
        _offsetX = value;
        if (bChanged)
        {
          _SetBrushVariable(null);
          EhSelfChanged(EventArgs.Empty);
        }
      }
    }

    public double GradientColorScale
    {
      get
      {
        return _gradientColorScale;
      }
      set
      {
        bool bChanged = (_gradientColorScale != value);
        _gradientColorScale = value;
        if (bChanged)
        {
          _SetBrushVariable(null);
          EhSelfChanged(EventArgs.Empty);
        }
      }
    }

    public double TextureOffsetX
    {
      get
      {
        return _offsetX;
      }
      set
      {
        bool bChanged = (_offsetX != value);
        _offsetX = value;
        if (bChanged)
        {
          _SetBrushVariable(null);
          EhSelfChanged(EventArgs.Empty);
        }
      }
    }

    public double TextureOffsetY
    {
      get
      {
        return _offsetY;
      }
      set
      {
        bool bChanged = (_offsetY != value);
        _offsetY = value;
        if (bChanged)
        {
          _SetBrushVariable(null);
          EhSelfChanged(EventArgs.Empty);
        }
      }
    }

    public ImageProxy TextureImage
    {
      get
      {
        return _textureImage;
      }
      set
      {
        bool bChanged = !object.ReferenceEquals(_textureImage, value) || (null != value && _textureImage.ContentHash != value.ContentHash);
        _textureImage = value;
        if (bChanged)
        {
          _SetBrushVariable(null);
          EhSelfChanged(EventArgs.Empty);
        }
      }
    }

    public TextureScaling TextureScale
    {
      get
      {
        return _textureScale;
      }
      set
      {
        bool bChanged = _textureScale != value;
        _textureScale = value;
        if (bChanged)
        {
          _SetBrushVariable(null);
          EhSelfChanged(EventArgs.Empty);
        }
      }
    }

    /// <summary>
    /// Sets the environment for the creation of native brush.
    /// </summary>
    /// <param name="boundingRectangle">Bounding rectangle used for gradient textures.</param>
    /// <param name="maxEffectiveResolution">Maximum effective resolution in Dpi. This information is neccessary for repeatable texture brushes. You can calculate this using <see cref="M:GetEffectiveMaximumResolution"/></param>
    /// <returns>True if changes to the brush were made. False otherwise.</returns>
    public bool SetEnvironment(RectangleD2D boundingRectangle, double maxEffectiveResolution)
    {
      return SetEnvironment((RectangleF)boundingRectangle, maxEffectiveResolution);
    }

    /// <summary>
    /// Sets the environment for the creation of native brush.
    /// </summary>
    /// <param name="boundingRectangle">Bounding rectangle used for gradient textures.</param>
    /// <param name="maxEffectiveResolution">Maximum effective resolution in Dpi. This information is neccessary for repeatable texture brushes. You can calculate this using <see cref="M:GetEffectiveMaximumResolution"/></param>
    /// <returns>True if changes to the brush were made. False otherwise.</returns>
    public bool SetEnvironment(RectangleF boundingRectangle, double maxEffectiveResolution)
    {
      bool changed = false;

      // fix: in order that a gradient is shown, the bounding rectangle's width and height must always be positive (this is not the case for instance for pens)
      if (boundingRectangle.Width < 0)
      {
        boundingRectangle.X += boundingRectangle.Width;
        boundingRectangle.Width = -boundingRectangle.Width;
      }
      else if (boundingRectangle.Width == 0)
      {
        boundingRectangle.Width = 1;
      }

      if (boundingRectangle.Height < 0)
      {
        boundingRectangle.Y += boundingRectangle.Height;
        boundingRectangle.Height = -boundingRectangle.Height;
      }
      else if (boundingRectangle.Height == 0)
      {
        boundingRectangle.Height = 1;
      }

      if (_brushType == BrushType.SolidBrush)
      {
        _brushBoundingRectangle = boundingRectangle; // has no meaning for solid brushes, so we set it but dont care
      }
      else
      {
        changed = (_brushBoundingRectangle != boundingRectangle); // for all other brushes it has a meaning, thus we will invalidate the brush if the rectangle changed
        _brushBoundingRectangle = boundingRectangle;
      }

      if (_textureImage is ISyntheticRepeatableTexture)
      {
        if (maxEffectiveResolution != _cachedEffectiveMaximumResolutionDpi)
          changed = true;
      }

      _cachedEffectiveMaximumResolutionDpi = maxEffectiveResolution;

      if (changed)
      {
        // we consider this set of the environment not as a change of the brush properties itself
        // thus we invalidate the cached brush, but we don't fire the Changed event
        _SetBrushVariable(null);
      }

      return changed;
    }

    public static double GetEffectiveMaximumResolution(Graphics g)
    {
      return GetEffectiveMaximumResolution(g, 1);
    }

    public static double GetEffectiveMaximumResolution(Graphics g, double objectScale)
    {
      double maxDpi = Math.Max(g.DpiX, g.DpiY) * g.PageScale;
      var e = g.Transform.Elements;
      var scaleX = e[0] * e[0] + e[1] * e[1];
      var scaleY = (e[0] * e[3] - e[1] * e[2]) / Math.Sqrt(scaleX);
      maxDpi *= Math.Max(scaleX, scaleY);
      maxDpi *= objectScale;
      return maxDpi;
    }

    private Bitmap GetDefaultTextureBitmap()
    {
      Bitmap result = new Bitmap(3, 3, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
      result.SetPixel(1, 1, System.Drawing.Color.Black);
      return result;
    }

    private static System.Drawing.Color ToGdi(NamedColor color)
    {
      var c = color.Color;
      return System.Drawing.Color.FromArgb(c.A, c.R, c.G, c.B);
    }

    public Brush Brush
    {
      get
      {
        if (_cachedBrush == null)
        {
          Brush br = null;
          switch (_brushType)
          {
            case BrushType.SolidBrush:
              br = new SolidBrush(ToGdi(_foreColor));
              break;

            case BrushType.LinearGradientBrush:
            case BrushType.TriangularShapeLinearGradientBrush:
            case BrushType.SigmaBellShapeLinearGradientBrush:
              if (_brushBoundingRectangle.IsEmpty)
                _brushBoundingRectangle = new RectangleF(0, 0, 1000, 1000);
              LinearGradientBrush lgb;
              br = lgb = new LinearGradientBrush((RectangleF)_brushBoundingRectangle, GetColor1(), GetColor2(), (float)-_angle);
              if (_wrapMode != WrapMode.Clamp)
                lgb.WrapMode = _wrapMode;
              if (_brushType == Gdi.BrushType.TriangularShapeLinearGradientBrush)
                lgb.SetBlendTriangularShape((float)_offsetX, (float)_gradientColorScale);
              else if (_brushType == Gdi.BrushType.SigmaBellShapeLinearGradientBrush)
                lgb.SetSigmaBellShape((float)_offsetX, (float)_gradientColorScale);
              break;

            case BrushType.PathGradientBrush:
            case BrushType.TriangularShapePathGradientBrush:
            case Gdi.BrushType.SigmaBellShapePathGradientBrush:
              {
                GraphicsPath p = new GraphicsPath();
                if (_brushBoundingRectangle.IsEmpty)
                  _brushBoundingRectangle = new RectangleD2D(0, 0, 1000, 1000);
                var outerRectangle = _brushBoundingRectangle.OuterCircleBoundingBox;
                p.AddEllipse((RectangleF)outerRectangle);
                PathGradientBrush pgb = new PathGradientBrush(p);
                if (_exchangeColors)
                {
                  pgb.SurroundColors = new Color[] { ToGdi(_backColor) };
                  pgb.CenterColor = ToGdi(_foreColor);
                }
                else
                {
                  pgb.SurroundColors = new Color[] { ToGdi(_foreColor) };
                  pgb.CenterColor = ToGdi(_backColor);
                }
                pgb.WrapMode = _wrapMode;
                if (_brushType == Gdi.BrushType.TriangularShapePathGradientBrush)
                  pgb.SetBlendTriangularShape(1, (float)_gradientColorScale);
                if (_brushType == Gdi.BrushType.SigmaBellShapePathGradientBrush)
                  pgb.SetSigmaBellShape(1, (float)_gradientColorScale);
                pgb.CenterPoint = (PointF)(outerRectangle.Location + new PointD2D(outerRectangle.Width * _offsetX, outerRectangle.Height * _offsetY));
                br = pgb;
              }
              break;

            case BrushType.HatchBrush:
            case BrushType.SyntheticTextureBrush:
            case BrushType.TextureBrush:
              if (_brushBoundingRectangle.IsEmpty)
                _brushBoundingRectangle = new RectangleD2D(0, 0, 1000, 1000);

              Image img = null;
              PointD2D finalSize = PointD2D.Empty;
              PointD2D sourceSize = PointD2D.Empty;
              double blowFactor;

              if (_textureImage is IHatchBrushTexture)
              {
                sourceSize = (_textureImage as IHatchBrushTexture).Size;
                finalSize = _textureScale.GetResultingSize(sourceSize, _brushBoundingRectangle.Size);
                blowFactor = Math.Max(Math.Abs(finalSize.X / sourceSize.X), Math.Abs(finalSize.Y / sourceSize.Y));
                img = (_textureImage as IHatchBrushTexture).GetImage(_cachedEffectiveMaximumResolutionDpi * blowFactor, _exchangeColors ? _backColor : _foreColor, _exchangeColors ? _foreColor : _backColor);
              }
              else if (_textureImage is ISyntheticRepeatableTexture)
              {
                sourceSize = (_textureImage as IHatchBrushTexture).Size;
                finalSize = _textureScale.GetResultingSize(sourceSize, _brushBoundingRectangle.Size);
                blowFactor = Math.Max(Math.Abs(finalSize.X / sourceSize.X), Math.Abs(finalSize.Y / sourceSize.Y));
                img = (_textureImage as ISyntheticRepeatableTexture).GetImage(_cachedEffectiveMaximumResolutionDpi * blowFactor);
              }
              else if (_textureImage != null)
              {
                img = _textureImage.GetImage();
                sourceSize = new PointD2D(img.Width * 72.0 / img.HorizontalResolution, img.Height * 72.0 / img.VerticalResolution);
                finalSize = _textureScale.GetResultingSize(sourceSize, _brushBoundingRectangle.Size);
              }

              if (img == null)
              {
                img = GetDefaultTextureBitmap();
                sourceSize = new PointD2D(img.Width * 72.0 / img.HorizontalResolution, img.Height * 72.0 / img.VerticalResolution);
                finalSize = _textureScale.GetResultingSize(sourceSize, _brushBoundingRectangle.Size);
              }
              TextureBrush tb = new TextureBrush(img);
              tb.WrapMode = this._wrapMode;

              double xscale = finalSize.X / img.Width;
              double yscale = finalSize.Y / img.Height;

              if (0 != _offsetX || 0 != _offsetY)
                tb.TranslateTransform((float)(-finalSize.X * _offsetX), (float)(-finalSize.Y * _offsetY));

              if (0 != _angle)
                tb.RotateTransform((float)(-_angle));

              if (xscale != 1 || yscale != 1)
                tb.ScaleTransform((float)xscale, (float)yscale);

              br = tb;
              break;
          } // end of switch
          this._SetBrushVariable(br);
        }
        return _cachedBrush;
      } // end of get
    } // end of prop. Brush

    private Color GetColor1()
    {
      return _exchangeColors ? ToGdi(_backColor) : ToGdi(_foreColor);
    }

    private Color GetColor2()
    {
      return _exchangeColors ? ToGdi(_foreColor) : ToGdi(_backColor);
    }

    public static bool AreEqual(BrushX b1, BrushX b2)
    {
      if (b1._brushType != b2._brushType)
        return false;

      // Brush types are equal - we have to go into details...
      switch (b1._brushType)
      {
        case BrushType.SolidBrush:
          if (b1._foreColor != b2._foreColor)
            return false;
          break;

        case BrushType.LinearGradientBrush:
        case BrushType.SigmaBellShapeLinearGradientBrush:
        case BrushType.TriangularShapeLinearGradientBrush:
          if (b1._foreColor != b2._foreColor)
            return false;
          if (b1._backColor != b2._backColor)
            return false;
          if (b1._exchangeColors != b2._exchangeColors)
            return false;
          if (b1._angle != b2._angle)
            return false;
          if (b1._wrapMode != b2._wrapMode)
            return false;
          if (b1._brushType != BrushType.LinearGradientBrush)
          {
            if (b1._offsetX != b2._offsetX)
              return false;
            if (b1._offsetY != b2._offsetY)
              return false;
            if (b1._gradientColorScale != b2._gradientColorScale)
              return false;
          }
          break;

        case BrushType.PathGradientBrush:
        case BrushType.TriangularShapePathGradientBrush:
        case BrushType.SigmaBellShapePathGradientBrush:
          if (b1._foreColor != b2._foreColor)
            return false;
          if (b1._backColor != b2._backColor)
            return false;
          if (b1._exchangeColors != b2._exchangeColors)
            return false;
          if (b1._wrapMode != b2._wrapMode)
            return false;
          break;

        case BrushType.HatchBrush:
        case BrushType.SyntheticTextureBrush:
        case BrushType.TextureBrush:
          if (b1._wrapMode != b2._wrapMode)
            return false;
          if (b1._textureScale != b2._textureScale)
            return false;
          if (b1._textureImage.ToString() != b2._textureImage.ToString())
            return false;
          break;
      } // end of switch
      return true;
    }

    public void SetSolidBrush(NamedColor c)
    {
      _brushType = BrushType.SolidBrush;
      _foreColor = c;
      _SetBrushVariable(null);
      EhSelfChanged(EventArgs.Empty);
    }

    protected void _SetBrushVariable(Brush br)
    {
      if (null != _cachedBrush)
        _cachedBrush.Dispose();

      _cachedBrush = br;
    }

    object ICloneable.Clone()
    {
      return new BrushX(this);
    }

    public BrushX Clone()
    {
      return new BrushX(this);
    }

    protected override void Dispose(bool isDisposing)
    {
      if (null != _cachedBrush)
      {
        _cachedBrush.Dispose();
        _cachedBrush = null;
      }

      base.Dispose(isDisposing);
    }

    #region static members

    public static BrushX Empty
    {
      get
      {
        return new BrushX(NamedColors.Transparent);
      }
    }

    #endregion static members

    #region Helpers

    private static void ToLinearGradientMode(LinearGradientModeEx mode, out LinearGradientMode lgm, out bool reverse)
    {
      switch (mode)
      {
        case LinearGradientModeEx.BackwardDiagonal:
          lgm = LinearGradientMode.BackwardDiagonal;
          reverse = false;
          break;

        case LinearGradientModeEx.ForwardDiagonal:
          lgm = LinearGradientMode.ForwardDiagonal;
          reverse = false;
          break;

        default:
        case LinearGradientModeEx.Horizontal:
          lgm = LinearGradientMode.Horizontal;
          reverse = false;
          break;

        case LinearGradientModeEx.Vertical:
          lgm = LinearGradientMode.Vertical;
          reverse = false;
          break;

        case LinearGradientModeEx.RevBackwardDiagonal:
          lgm = LinearGradientMode.BackwardDiagonal;
          reverse = true;
          break;

        case LinearGradientModeEx.RevForwardDiagonal:
          lgm = LinearGradientMode.ForwardDiagonal;
          reverse = true;
          break;

        case LinearGradientModeEx.RevHorizontal:
          lgm = LinearGradientMode.Horizontal;
          reverse = true;
          break;

        case LinearGradientModeEx.RevVertical:
          lgm = LinearGradientMode.Vertical;
          reverse = true;
          break;
      }
    }

    private static double LinearGradientModeToAngle(LinearGradientMode mode)
    {
      switch (mode)
      {
        default:
        case LinearGradientMode.Horizontal:
          return 0;

        case LinearGradientMode.Vertical:
          return -90;

        case LinearGradientMode.ForwardDiagonal:
          return -45;

        case LinearGradientMode.BackwardDiagonal:
          return 45;
      }
    }

    private static ImageProxy HatchStyleToImage(HatchStyle s)
    {
      switch (s)
      {
        default:
        case HatchStyle.Horizontal:
          return new Gdi.HatchBrushes.HorizontalHatchBrush();

        case HatchStyle.Vertical:
          return new Gdi.HatchBrushes.VerticalHatchBrush();

        case HatchStyle.ForwardDiagonal:
          return new Gdi.HatchBrushes.ForwardDiagonalHatchBrush();

        case HatchStyle.BackwardDiagonal:
          return new Gdi.HatchBrushes.BackwardDiagonalHatchBrush();

        case HatchStyle.Cross:
          return new Gdi.HatchBrushes.CrossHatchBrush();

        case HatchStyle.DiagonalCross:
          return new Gdi.HatchBrushes.DiagonalCrossHatchBrush();

        case HatchStyle.SolidDiamond:
          return new Gdi.HatchBrushes.DiamondHatchBrush();

        case HatchStyle.LargeCheckerBoard:
        case HatchStyle.SmallCheckerBoard:
          return new Gdi.HatchBrushes.CheckerHatchBrush();

        case HatchStyle.Sphere:
          return new Gdi.HatchBrushes.CircleHatchBrush();

        case HatchStyle.HorizontalBrick:
          return new Gdi.HatchBrushes.BrickHatchBrush();
      }
    }

    #endregion Helpers
  } // end of class BrushHolder
} // end of namespace
