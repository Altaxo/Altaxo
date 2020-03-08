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
using System.Drawing.Drawing2D;
using System.Linq;
using Altaxo.Drawing;
using Altaxo.Geometry;
using Altaxo.Main;

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
  public class BrushX : IEquatable<BrushX>, IImmutable
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
            var gm = (LinearGradientModeEx)info.GetEnum("GradientMode", typeof(LinearGradientModeEx));
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
            s._textureImage = (ImageProxy)info.GetValue("Texture", s);
            s._wrapMode = (WrapMode)info.GetEnum("WrapMode", typeof(WrapMode));
            var scale = info.GetSingle("Scale");
            s._textureScale = new TextureScaling(TextureScalingMode.Source, AspectRatioPreservingMode.PreserveXPriority, scale, scale);
            break;
        }

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
            s._textureImage = (ImageProxy)info.GetValue("Texture", s);
            s._wrapMode = (WrapMode)info.GetEnum("WrapMode", typeof(WrapMode));
            var scale = info.GetSingle("Scale");
            s._textureScale = new TextureScaling(TextureScalingMode.Source, AspectRatioPreservingMode.PreserveXPriority, scale, scale);
            break;
        }


        return s;
      }
    }

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(BrushX), 4)] // 2012-02-14
    private class XmlSerializationSurrogate4 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (BrushX)obj;
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
            s._textureImage = (ImageProxy)info.GetValue("Texture", s);
            break;

          case BrushType.TextureBrush:
            s._wrapMode = (WrapMode)info.GetEnum("WrapMode", typeof(WrapMode));
            s._angle = info.GetDouble("Angle");
            s._textureScale = (TextureScaling)info.GetValue("Scale", s);
            s._offsetX = info.GetDouble("OffsetX");
            s._offsetY = info.GetDouble("OffsetY");
            s._textureImage = (ImageProxy)info.GetValue("Texture", s);
            break;
        }

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

      if (_brushType != other._brushType)
        return false;

      switch (BrushType)
      {
        case Gdi.BrushType.SolidBrush:
          if (!_foreColor.Equals(other._foreColor))
            return false;
          break;

        case BrushType.LinearGradientBrush:
        case BrushType.TriangularShapeLinearGradientBrush:
        case BrushType.SigmaBellShapeLinearGradientBrush:
          if (_exchangeColors != other._exchangeColors)
            return false;
          if (!_foreColor.Equals(other._foreColor))
            return false;
          if (!_backColor.Equals(other._backColor))
            return false;
          if (_angle != other._angle)
            return false;
          if (_wrapMode != other._wrapMode)
            return false;
          if (_brushType != Gdi.BrushType.LinearGradientBrush)
          {
            if (_offsetX != other._offsetX)
              return false;
            if (_gradientColorScale != other._gradientColorScale)
              return false;
          }
          break;

        case BrushType.PathGradientBrush:
        case BrushType.TriangularShapePathGradientBrush:
        case BrushType.SigmaBellShapePathGradientBrush:
          if (_exchangeColors != other._exchangeColors)
            return false;
          if (!_foreColor.Equals(other._foreColor))
            return false;
          if (!_backColor.Equals(other._backColor))
            return false;
          if (_wrapMode != other._wrapMode)
            return false;
          if (_brushType != BrushType.PathGradientBrush)
          {
            if (_gradientColorScale != other._gradientColorScale)
              return false;
          }
          if (_offsetX != other._offsetX)
            return false;
          if (_offsetY != other._offsetY)
            return false;
          break;

        case BrushType.HatchBrush:
        case BrushType.SyntheticTextureBrush:
        case BrushType.TextureBrush:
          if (_brushType == BrushType.HatchBrush)
          {
            if (_exchangeColors != other._exchangeColors)
              return false;
            if (!_foreColor.Equals(other._foreColor))
              return false;
            if (!_backColor.Equals(other._backColor))
              return false;
          }
          if (!SafeEquals(_textureImage, other._textureImage))
            return false;
          if (_textureScale != other._textureScale)
            return false;
          if (_wrapMode != other._wrapMode)
            return false;
          if (_angle != other._angle)
            return false;
          if (_offsetX != other._offsetX)
            return false;
          if (_offsetY != other._offsetY)
            return false;
          break;

        default:
          throw new NotImplementedException("BrushType not implemented:" + _brushType.ToString());
      }
      return true;
    }

    public override bool Equals(object obj)
    {
      if (obj is BrushX other)
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
    }

    public BrushX(NamedColor c)
    {
      _brushType = BrushType.SolidBrush;
      _foreColor = c;
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

    public BrushType BrushType
    {
      get
      {
        return _brushType;
      }
    }

    public BrushX WithBrushType(BrushType brushType)
    {
      if (_brushType == brushType)
      {
        return this;
      }
      else
      {
        var result = new BrushX(brushType);
        return result;
      }
    }

    public BrushX(BrushType brushType)
    {
      _brushType = brushType;

      switch (_brushType)
      {
        case BrushType.SolidBrush:
          break;
        case BrushType.HatchBrush:
          _wrapMode = System.Drawing.Drawing2D.WrapMode.Tile;
          _offsetX = 0;
          _offsetY = 0;
          _textureImage = DefaultHatchBrush;
          break;
        case BrushType.TextureBrush:
          _wrapMode = System.Drawing.Drawing2D.WrapMode.Tile;
          _offsetX = 0;
          _offsetY = 0;
          _textureImage = DefaultTextureBrush;
          break;
        case BrushType.LinearGradientBrush:
          _wrapMode = System.Drawing.Drawing2D.WrapMode.TileFlipXY;
          break;
        case BrushType.PathGradientBrush:
          _wrapMode = System.Drawing.Drawing2D.WrapMode.TileFlipXY;
          _offsetX = 0.5;
          _offsetY = 0.5;
          break;
        case BrushType.SigmaBellShapeLinearGradientBrush:
          _wrapMode = System.Drawing.Drawing2D.WrapMode.TileFlipXY;
          _offsetX = 0.5;
          _gradientColorScale = 1;
          break;
        case BrushType.TriangularShapeLinearGradientBrush:
          _wrapMode = System.Drawing.Drawing2D.WrapMode.TileFlipXY;
          _offsetX = 0.5;
          _gradientColorScale = 1;
          break;
        case BrushType.SigmaBellShapePathGradientBrush:
        case BrushType.TriangularShapePathGradientBrush:
          _wrapMode = System.Drawing.Drawing2D.WrapMode.TileFlipXY;
          _offsetX = 0.5;
          _offsetY = 0.5;
          _gradientColorScale = 1;
          break;
        case BrushType.SyntheticTextureBrush:
          _wrapMode = System.Drawing.Drawing2D.WrapMode.Tile;
          _offsetX = 0;
          _offsetY = 0;
          _textureImage = DefaultSyntheticBrush;
          break;
        default:
          break;
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
    }

    public BrushX WithColor(NamedColor color)
    {
      if (!(_foreColor == color))
      {
        var result = (BrushX)this.MemberwiseClone();
        result._foreColor = color;
        return result;
      }
      else
      {
        return this;
      }
    }

    public BrushX WithColors(NamedColor foreColor, NamedColor backColor)
    {
      if (!(_foreColor == foreColor) || (BrushType != BrushType.SolidBrush) && !(_backColor == backColor))
      {
        var result = (BrushX)this.MemberwiseClone();
        result._foreColor = foreColor;
        result._backColor = backColor;
        return result;
      }
      else
      {
        return this;
      }
    }

    public NamedColor BackColor
    {
      get { return _backColor; }
    }

    public BrushX WithBackColor(NamedColor color)
    {
      if (!(_backColor == color))
      {
        var result = (BrushX)this.MemberwiseClone();
        result._backColor = color;
        return result;
      }
      else
      {
        return this;
      }
    }

    public bool ExchangeColors
    {
      get
      {
        return _exchangeColors;
      }
    }

    public BrushX WithExchangedColors(bool exchangeColors)
    {
      if (!(_exchangeColors == exchangeColors))
      {
        var result = (BrushX)this.MemberwiseClone();
        result._exchangeColors = exchangeColors;
        return result;
      }
      else
      {
        return this;
      }
    }

    public WrapMode WrapMode
    {
      get
      {
        return _wrapMode;
      }
    }

    public BrushX WithWrapMode(WrapMode wrapMode)
    {
      if (!(_wrapMode == wrapMode))
      {
        var result = (BrushX)this.MemberwiseClone();
        result._wrapMode = _wrapMode;
        return result;
      }
      else
      {
        return this;
      }
    }



    public double GradientAngle
    {
      get
      {
        return _angle;
      }
    }

    public BrushX WithGradientAngle(double angle)
    {
      if (!(_angle == angle))
      {
        var result = (BrushX)this.MemberwiseClone();
        result._angle = angle;
        return result;
      }
      else
      {
        return this;
      }
    }

    public double GradientFocus
    {
      get
      {
        return _offsetX;
      }
    }

    public BrushX WithGradientFocus(double gradientFocus)
    {
      if (!(_offsetX == gradientFocus))
      {
        var result = (BrushX)this.MemberwiseClone();
        result._offsetX = gradientFocus;
        return result;
      }
      else
      {
        return this;
      }
    }

    public double GradientColorScale
    {
      get
      {
        return _gradientColorScale;
      }

    }

    public BrushX WithGradientColorScale(double gradientColorScale)
    {
      if (!(_gradientColorScale == gradientColorScale))
      {
        var result = (BrushX)this.MemberwiseClone();
        result._gradientColorScale = gradientColorScale;
        return result;
      }
      else
      {
        return this;
      }
    }

    public double TextureOffsetX
    {
      get
      {
        return _offsetX;
      }
    }

    public BrushX WithTextureOffsetX(double offsetX)
    {
      if (!(_offsetX == offsetX))
      {
        var result = (BrushX)this.MemberwiseClone();
        result._offsetX = offsetX;
        return result;
      }
      else
      {
        return this;
      }
    }

    public double TextureOffsetY
    {
      get
      {
        return _offsetY;
      }
    }

    public BrushX WithTextureOffsetY(double offsetY)
    {
      if (!(_offsetY == offsetY))
      {
        var result = (BrushX)this.MemberwiseClone();
        result._offsetY = offsetY;
        return result;
      }
      else
      {
        return this;
      }
    }

    public ImageProxy TextureImage
    {
      get
      {
        return _textureImage;
      }
    }

    public BrushX WithTextureImage(ImageProxy textureImage)
    {
      if (!object.ReferenceEquals(_textureImage, textureImage))
      {
        var result = (BrushX)this.MemberwiseClone();
        result._textureImage = textureImage;
        return result;
      }
      else
      {
        return this;
      }
    }

    public TextureScaling TextureScale
    {
      get
      {
        return _textureScale;
      }
    }

    public BrushX WithTextureScaling(TextureScaling textureScale)
    {
      if (!(_textureScale == textureScale))
      {
        var result = (BrushX)this.MemberwiseClone();
        result._textureScale = textureScale;
        return result;
      }
      else
      {
        return this;
      }
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

  } // end of class BrushX


} // end of namespace
