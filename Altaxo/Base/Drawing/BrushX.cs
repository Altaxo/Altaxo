#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2020 Dr. Dirk Lellinger
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
using System.Drawing.Drawing2D;
using System.Linq;
using Altaxo.Geometry;
using Altaxo.Graph;
using Altaxo.Main;

#nullable enable

namespace Altaxo.Drawing
{
  /// <summary>
  /// Holds all information necessary to create a brush of any kind without allocating resources, so this class can be serialized.
  /// </summary>
  [Serializable]
  public class BrushX : IEquatable<BrushX>, IImmutable
  {
    /// <summary>
    /// The brush type.
    /// </summary>
    protected BrushType _brushType; // Type of the brush

    /// <summary>
    /// The foreground color of the brush.
    /// </summary>
    protected NamedColor _foreColor; // Color of the brush

    /// <summary>
    /// The background color of the brush.
    /// </summary>
    protected NamedColor _backColor = NamedColors.Transparent; // Backcolor of brush, f.i.f. HatchStyle brushes

    /// <summary>
    /// Indicates whether foreground and background colors are exchanged.
    /// </summary>
    protected bool _exchangeColors;

    /// <summary>
    /// The wrap mode used for texture and linear gradient brushes.
    /// </summary>
    protected WrapMode _wrapMode; // für TextureBrush und LinearGradientBrush

    /// <summary>
    /// The brush angle in degrees.
    /// </summary>
    protected double _angle;

    /// <summary>For a gradient brush, this is the GradientFocus property. For texture brushes, it is the X-Offset property.</summary>
    protected double _offsetX;
    /// <summary>
    /// The Y offset for texture and path-gradient brushes.
    /// </summary>
    protected double _offsetY;

    /// <summary>
    /// The gradient color scale factor.
    /// </summary>
    protected double _gradientColorScale;

    /// <summary>
    /// The texture scaling used for texture-based brushes.
    /// </summary>
    protected TextureScaling _textureScale = TextureScaling.Default;

    /// <summary>
    /// The texture image used for texture-based brushes.
    /// </summary>
    protected ImageProxy? _textureImage; // für Texturebrush

    /// <summary>
    /// Cached hash code. Must be invaliated after cloning the instance.
    /// </summary>
    protected int? _cachedHashCode;

    #region "Serialization"

    #region Version 0

    [Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.BrushHolder", 0)]
    private class XmlSerializationSurrogate0 : Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object o, Serialization.Xml.IXmlSerializationInfo info)
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

      public object Deserialize(object? o, Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = o as BrushX ?? new BrushX(NamedColors.Black);

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
            s._textureImage = HatchStyleToImage(hatchStyle);
            break;
        }

        s._cachedHashCode = null;
        return s;
      }
    }

    #endregion Version 0

    #region Version 1 and 2

    [Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.BrushHolder", 1)]
    [Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.Gdi.BrushX", 2)]
    private class XmlSerializationSurrogate1 : Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object o, Serialization.Xml.IXmlSerializationInfo info)
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

      public object Deserialize(object? o, Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = o as BrushX ?? new BrushX(NamedColors.Black);

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
            s._textureImage = HatchStyleToImage(hatchStyle);
            break;

          case BrushType.LinearGradientBrush:
            s._foreColor = (NamedColor)info.GetValue("ForeColor", s);
            s._backColor = (NamedColor)info.GetValue("BackColor", s);
            s._wrapMode = (WrapMode)info.GetEnum("WrapMode", typeof(WrapMode));
            var gm = info.GetEnum<LinearGradientModeEx>("GradientMode");
            var gmname = Enum.GetName(typeof(LinearGradientModeEx), gm) ?? string.Empty;
            if (gmname.StartsWith("Rev"))
            {
              s._exchangeColors = true;
              s._angle = 180 + LinearGradientModeToAngle((LinearGradientMode)Enum.Parse(typeof(LinearGradientMode), gmname.Substring(3)));
            }
            else
            {
              s._angle = LinearGradientModeToAngle((LinearGradientMode)Enum.Parse(typeof(LinearGradientMode), gmname));
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

        s._cachedHashCode = null;
        return s;
      }
    }

    #endregion Version 1 and 2

    #region Version 3

    [Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.Gdi.BrushX", 3)]
    private class XmlSerializationSurrogate3 : Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object o, Serialization.Xml.IXmlSerializationInfo info)
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

      public object Deserialize(object? o, Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = o as BrushX ?? new BrushX(NamedColors.Black);

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
            s._textureImage = HatchStyleToImage(hatchStyle);
            break;

          case BrushType.LinearGradientBrush:
            s._foreColor = (NamedColor)info.GetValue("ForeColor", s);
            s._backColor = (NamedColor)info.GetValue("BackColor", s);
            s._exchangeColors = info.GetBoolean("ExchangeColors");
            s._wrapMode = (WrapMode)info.GetEnum("WrapMode", typeof(WrapMode));
            s._angle = LinearGradientModeToAngle((LinearGradientMode)info.GetEnum("GradientMode", typeof(LinearGradientMode)));
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

        s._cachedHashCode = null;
        return s;
      }
    }

    #endregion Version 3

    #region Version 4 and 5

    /// <summary>
    /// 2012-02-14 Version 4: new serialization code.
    /// 2020-03-30 Version 5: move from Altaxo.Graph.Gdi namespace to Altaxo.Drawing namespace
    /// </summary>
    /// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
    [Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.Gdi.BrushX", 4)]
    [Serialization.Xml.XmlSerializationSurrogateFor(typeof(BrushX), 5)]
    private class XmlSerializationSurrogate4 : Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object o, Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (BrushX)o;
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
            info.AddValueOrNull("Texture", s._textureImage);
            break;

          case BrushType.TextureBrush:
            info.AddEnum("WrapMode", s._wrapMode);
            info.AddValue("Angle", s._angle);
            info.AddValue("Scale", s._textureScale);
            info.AddValue("OffsetX", s._offsetX);
            info.AddValue("OffsetY", s._offsetY);
            info.AddValueOrNull("Texture", s._textureImage);
            break;
        } // end of switch
      }

      public object Deserialize(object? o, Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = o as BrushX ?? new BrushX(NamedColors.Black);

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

        s._cachedHashCode = null;
        return s;
      }
    }

    #endregion Version 4 and 5

    #endregion "Serialization"

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="BrushX"/> class as a solid brush.
    /// </summary>
    /// <param name="c">The solid brush color.</param>
    public BrushX(NamedColor c)
    {
      _brushType = BrushType.SolidBrush;
      _foreColor = c;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BrushX"/> class for the specified brush type.
    /// </summary>
    /// <param name="brushType">The brush type.</param>
    public BrushX(BrushType brushType)
    {
      _brushType = brushType;

      switch (_brushType)
      {
        case BrushType.SolidBrush:
          break;
        case BrushType.HatchBrush:
          _wrapMode = WrapMode.Tile;
          _offsetX = 0;
          _offsetY = 0;
          _textureImage = DefaultHatchBrush;
          break;
        case BrushType.TextureBrush:
          _wrapMode = WrapMode.Tile;
          _offsetX = 0;
          _offsetY = 0;
          _textureImage = DefaultTextureBrush;
          break;
        case BrushType.LinearGradientBrush:
          _wrapMode = WrapMode.TileFlipXY;
          break;
        case BrushType.PathGradientBrush:
          _wrapMode = WrapMode.TileFlipXY;
          _offsetX = 0.5;
          _offsetY = 0.5;
          break;
        case BrushType.SigmaBellShapeLinearGradientBrush:
          _wrapMode = WrapMode.TileFlipXY;
          _offsetX = 0.5;
          _gradientColorScale = 1;
          break;
        case BrushType.TriangularShapeLinearGradientBrush:
          _wrapMode = WrapMode.TileFlipXY;
          _offsetX = 0.5;
          _gradientColorScale = 1;
          break;
        case BrushType.SigmaBellShapePathGradientBrush:
        case BrushType.TriangularShapePathGradientBrush:
          _wrapMode = WrapMode.TileFlipXY;
          _offsetX = 0.5;
          _offsetY = 0.5;
          _gradientColorScale = 1;
          break;
        case BrushType.SyntheticTextureBrush:
          _wrapMode = WrapMode.Tile;
          _offsetX = 0;
          _offsetY = 0;
          _textureImage = DefaultSyntheticBrush;
          break;
        default:
          break;
      }
    }

    #endregion Constructors

    #region Equality and Hash

    /// <inheritdoc/>
    public bool Equals(BrushX? other)
    {
      if (other is null)
        return false;

      if (ReferenceEquals(this, other))
        return true;

      if (_brushType != other._brushType)
        return false;

      switch (BrushType)
      {
        case BrushType.SolidBrush:
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
          if (!(_angle == other._angle))
            return false;
          if (!(_wrapMode == other._wrapMode))
            return false;
          if (!(_brushType == BrushType.LinearGradientBrush))
          {
            if (!(_offsetX == other._offsetX))
              return false;
            if (!(_gradientColorScale == other._gradientColorScale))
              return false;
          }
          break;

        case BrushType.PathGradientBrush:
        case BrushType.TriangularShapePathGradientBrush:
        case BrushType.SigmaBellShapePathGradientBrush:
          if (!(_exchangeColors == other._exchangeColors))
            return false;
          if (!_foreColor.Equals(other._foreColor))
            return false;
          if (!_backColor.Equals(other._backColor))
            return false;
          if (!(_wrapMode == other._wrapMode))
            return false;
          if (!(_brushType == BrushType.PathGradientBrush))
          {
            if (!(_gradientColorScale == other._gradientColorScale))
              return false;
          }
          if (!(_offsetX == other._offsetX))
            return false;
          if (!(_offsetY == other._offsetY))
            return false;
          break;

        case BrushType.HatchBrush:
        case BrushType.SyntheticTextureBrush:
        case BrushType.TextureBrush:
          if (_brushType == BrushType.HatchBrush || _brushType == BrushType.SyntheticTextureBrush)
          {
            if (!(_exchangeColors == other._exchangeColors))
              return false;
            if (!_foreColor.Equals(other._foreColor))
              return false;
            if (!_backColor.Equals(other._backColor))
              return false;
          }
          if (!Equals(_textureImage, other._textureImage))
            return false;
          if (!(_textureScale == other._textureScale))
            return false;
          if (!(_wrapMode == other._wrapMode))
            return false;
          if (!(_angle == other._angle))
            return false;
          if (!(_offsetX == other._offsetX))
            return false;
          if (!(_offsetY == other._offsetY))
            return false;
          break;

        default:
          throw new NotImplementedException("BrushType not implemented:" + _brushType.ToString());
      }
      return true;
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
      return Equals(obj as BrushX);
    }

    /// <summary>
    /// Determines whether two <see cref="BrushX"/> instances are equal.
    /// </summary>
    /// <param name="x">The first brush.</param>
    /// <param name="y">The second brush.</param>
    /// <returns><see langword="true"/> if both brushes are equal; otherwise, <see langword="false"/>.</returns>
    public static bool operator ==(BrushX? x, BrushX? y)
    {
      return x is { } _ ? x.Equals(y) : y is { } _ ? y.Equals(x) : true;
    }
    /// <summary>
    /// Determines whether two <see cref="BrushX"/> instances are not equal.
    /// </summary>
    /// <param name="x">The first brush.</param>
    /// <param name="y">The second brush.</param>
    /// <returns><see langword="true"/> if the brushes are not equal; otherwise, <see langword="false"/>.</returns>
    public static bool operator !=(BrushX? x, BrushX? y)
    {
      return !(x == y);
    }

    /// <summary>
    /// Calculates the hash code for the current brush state.
    /// </summary>
    /// <returns>The calculated hash code.</returns>
    protected int CalculateHash()
    {
      var result = _brushType.GetHashCode();

      unchecked
      {
        switch (BrushType)
        {
          case BrushType.SolidBrush:
            result += 7 * _foreColor.GetHashCode();
            break;

          case BrushType.LinearGradientBrush:
          case BrushType.TriangularShapeLinearGradientBrush:
          case BrushType.SigmaBellShapeLinearGradientBrush:
            result += _exchangeColors ? 5 : 0;
            result += 7 * _foreColor.GetHashCode();
            result += 11 * _backColor.GetHashCode();
            result += 13 * _angle.GetHashCode();
            result += 17 * _wrapMode.GetHashCode();
            if (!(_brushType == BrushType.LinearGradientBrush))
            {
              result += 19 * _offsetX.GetHashCode();
              result += 23 * _offsetY.GetHashCode();
            }
            break;

          case BrushType.PathGradientBrush:
          case BrushType.TriangularShapePathGradientBrush:
          case BrushType.SigmaBellShapePathGradientBrush:
            result += _exchangeColors ? 5 : 0;
            result += 7 * _foreColor.GetHashCode();
            result += 11 * _backColor.GetHashCode();
            result += 17 * _wrapMode.GetHashCode();

            result += 19 * _offsetX.GetHashCode();
            result += 23 * _offsetY.GetHashCode();
            if (!(_brushType == BrushType.PathGradientBrush))
            {
              result += 29 * _gradientColorScale.GetHashCode();
            }
            break;

          case BrushType.HatchBrush:
          case BrushType.SyntheticTextureBrush:
          case BrushType.TextureBrush:
            if (!(_brushType == BrushType.TextureBrush))
            {
              result += _exchangeColors ? 5 : 0;
              result += 7 * _foreColor.GetHashCode();
              result += 11 * _backColor.GetHashCode();
            }
            result += 17 * _wrapMode.GetHashCode();
            result += 13 * _angle.GetHashCode();
            result += 19 * _offsetX.GetHashCode();
            result += 23 * _offsetY.GetHashCode();
            result += 31 * _textureScale.GetHashCode();
            if (_textureImage is { } texImage)
              result += 37 * texImage.GetHashCode();
            break;

          default:
            throw new NotImplementedException("BrushType not implemented:" + _brushType.ToString());
        }
      }
      return result;
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
      if (!_cachedHashCode.HasValue)
      {
        _cachedHashCode = CalculateHash();
      }
      return _cachedHashCode.Value;
    }

    /// <summary>
    /// Clones this instance, and sets the cached HashCode to null.
    /// </summary>
    /// <returns>Cloned instance.</returns>
    protected BrushX Clone()
    {
      var result = (BrushX)MemberwiseClone();
      result._cachedHashCode = null;
      return result;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the brush type.
    /// </summary>
    public BrushType BrushType
    {
      get
      {
        return _brushType;
      }
    }

    /// <summary>
    /// Returns a copy of this brush with a different brush type.
    /// </summary>
    /// <param name="value">The new brush type.</param>
    /// <returns>A brush with the updated brush type.</returns>
    public BrushX WithBrushType(BrushType value)
    {
      if (!(_brushType == value))
      {
        var result = new BrushX(value);

        if (value != BrushType.TextureBrush)
        {
          if (value == BrushType.SolidBrush)
          {
            result._foreColor = Color;
          }
          else
          {
            result._exchangeColors = _exchangeColors;
            result._backColor = _backColor;
            result._foreColor = _foreColor;
          }
        }
        return result;
      }
      else
      {
        return this;
      }
    }

    /// <summary>
    /// Gets a value indicating whether this brush is a solid brush.
    /// </summary>
    public bool IsSolidBrush
    {
      get
      {
        return _brushType == BrushType.SolidBrush;
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

    /// <summary>
    /// Gets the foreground color of the brush.
    /// </summary>
    public NamedColor Color
    {
      get { return _foreColor; }
    }

    /// <summary>
    /// Returns a copy of this brush with a different foreground color.
    /// </summary>
    /// <param name="color">The new foreground color.</param>
    /// <returns>A brush with the updated foreground color.</returns>
    public BrushX WithColor(NamedColor color)
    {
      if (!(_foreColor == color))
      {
        var result = Clone();
        result._foreColor = color;
        return result;
      }
      else
      {
        return this;
      }
    }

    /// <summary>
    /// Returns a copy of this brush with different foreground and background colors.
    /// </summary>
    /// <param name="foreColor">The new foreground color.</param>
    /// <param name="backColor">The new background color.</param>
    /// <returns>A brush with the updated colors.</returns>
    public BrushX WithColors(NamedColor foreColor, NamedColor backColor)
    {
      if (!(_foreColor == foreColor) || BrushType != BrushType.SolidBrush && !(_backColor == backColor))
      {
        var result = Clone();
        result._foreColor = foreColor;
        result._backColor = BrushType != BrushType.SolidBrush ? backColor : NamedColors.Transparent;
        return result;
      }
      else
      {
        return this;
      }
    }

    /// <summary>
    /// Gets the background color of the brush.
    /// </summary>
    public NamedColor BackColor
    {
      get { return _backColor; }
    }

    /// <summary>
    /// Returns a copy of this brush with a different background color.
    /// </summary>
    /// <param name="color">The new background color.</param>
    /// <returns>A brush with the updated background color.</returns>
    public BrushX WithBackColor(NamedColor color)
    {
      if (BrushType != BrushType.SolidBrush && !(_backColor == color))
      {
        var result = Clone();
        result._backColor = color;
        return result;
      }
      else
      {
        return this;
      }
    }

    /// <summary>
    /// Gets a value indicating whether foreground and background colors are exchanged.
    /// </summary>
    public bool ExchangeColors
    {
      get
      {
        return _exchangeColors;
      }
    }

    /// <summary>
    /// Returns a copy of this brush with the exchanged-colors flag updated.
    /// </summary>
    /// <param name="exchangeColors">The new exchanged-colors flag.</param>
    /// <returns>A brush with the updated exchanged-colors flag.</returns>
    public BrushX WithExchangedColors(bool exchangeColors)
    {
      if (!(_exchangeColors == exchangeColors))
      {
        var result = Clone();
        result._exchangeColors = exchangeColors;
        return result;
      }
      else
      {
        return this;
      }
    }

    /// <summary>
    /// Gets the wrap mode used for texture-based brushes.
    /// </summary>
    public WrapMode WrapMode
    {
      get
      {
        return _wrapMode;
      }
    }

    /// <summary>
    /// Returns a copy of this brush with a different wrap mode.
    /// </summary>
    /// <param name="wrapMode">The new wrap mode.</param>
    /// <returns>A brush with the updated wrap mode.</returns>
    public BrushX WithWrapMode(WrapMode wrapMode)
    {
      if (!(_wrapMode == wrapMode))
      {
        var result = Clone();
        result._wrapMode = _wrapMode;
        return result;
      }
      else
      {
        return this;
      }
    }



    /// <summary>
    /// Gets the gradient angle in degrees.
    /// </summary>
    public double GradientAngle
    {
      get
      {
        return _angle;
      }
    }

    /// <summary>
    /// Returns a copy of this brush with a different gradient angle.
    /// </summary>
    /// <param name="angle">The new gradient angle in degrees.</param>
    /// <returns>A brush with the updated gradient angle.</returns>
    public BrushX WithGradientAngle(double angle)
    {
      if (!(_angle == angle))
      {
        var result = Clone();
        result._angle = angle;
        return result;
      }
      else
      {
        return this;
      }
    }

    /// <summary>
    /// Gets the gradient focus value.
    /// </summary>
    public double GradientFocus
    {
      get
      {
        return _offsetX;
      }
    }

    /// <summary>
    /// Returns a copy of this brush with a different gradient focus.
    /// </summary>
    /// <param name="gradientFocus">The new gradient focus value.</param>
    /// <returns>A brush with the updated gradient focus.</returns>
    public BrushX WithGradientFocus(double gradientFocus)
    {
      if (!(_offsetX == gradientFocus))
      {
        var result = Clone();
        result._offsetX = gradientFocus;
        return result;
      }
      else
      {
        return this;
      }
    }

    /// <summary>
    /// Gets the gradient color scale factor.
    /// </summary>
    public double GradientColorScale
    {
      get
      {
        return _gradientColorScale;
      }
    }

    /// <summary>
    /// Returns a copy of this brush with a different gradient color scale.
    /// </summary>
    /// <param name="gradientColorScale">The new gradient color scale.</param>
    /// <returns>A brush with the updated gradient color scale.</returns>
    public BrushX WithGradientColorScale(double gradientColorScale)
    {
      if (!(_gradientColorScale == gradientColorScale))
      {
        var result = Clone();
        result._gradientColorScale = gradientColorScale;
        return result;
      }
      else
      {
        return this;
      }
    }

    /// <summary>
    /// Gets the horizontal texture offset.
    /// </summary>
    public double TextureOffsetX
    {
      get
      {
        return _offsetX;
      }
    }

    /// <summary>
    /// Returns a copy of this brush with a different horizontal texture offset.
    /// </summary>
    /// <param name="offsetX">The new horizontal texture offset.</param>
    /// <returns>A brush with the updated horizontal texture offset.</returns>
    public BrushX WithTextureOffsetX(double offsetX)
    {
      if (!(_offsetX == offsetX))
      {
        var result = Clone();
        result._offsetX = offsetX;
        return result;
      }
      else
      {
        return this;
      }
    }

    /// <summary>
    /// Gets the vertical texture offset.
    /// </summary>
    public double TextureOffsetY
    {
      get
      {
        return _offsetY;
      }
    }

    /// <summary>
    /// Returns a copy of this brush with a different vertical texture offset.
    /// </summary>
    /// <param name="offsetY">The new vertical texture offset.</param>
    /// <returns>A brush with the updated vertical texture offset.</returns>
    public BrushX WithTextureOffsetY(double offsetY)
    {
      if (!(_offsetY == offsetY))
      {
        var result = Clone();
        result._offsetY = offsetY;
        return result;
      }
      else
      {
        return this;
      }
    }

    /// <summary>
    /// Gets the texture image used by texture-based brushes.
    /// </summary>
    public ImageProxy? TextureImage
    {
      get
      {
        return _textureImage;
      }
    }

    /// <summary>
    /// Returns a copy of this brush with a different texture image.
    /// </summary>
    /// <param name="textureImage">The new texture image.</param>
    /// <returns>A brush with the updated texture image.</returns>
    public BrushX WithTextureImage(ImageProxy textureImage)
    {
      if (!ReferenceEquals(_textureImage, textureImage))
      {
        var result = Clone();
        result._textureImage = textureImage;
        return result;
      }
      else
      {
        return this;
      }
    }

    /// <summary>
    /// Gets the texture scaling mode.
    /// </summary>
    public TextureScaling TextureScale
    {
      get
      {
        return _textureScale;
      }
    }

    /// <summary>
    /// Returns a copy of this brush with a different texture scaling mode.
    /// </summary>
    /// <param name="textureScale">The new texture scaling mode.</param>
    /// <returns>A brush with the updated texture scaling mode.</returns>
    public BrushX WithTextureScale(TextureScaling textureScale)
    {
      if (!(_textureScale == textureScale))
      {
        var result = Clone();
        result._textureScale = textureScale;
        return result;
      }
      else
      {
        return this;
      }
    }

    #endregion Properties

    #region static members

    /// <summary>
    /// Gets an empty transparent brush.
    /// </summary>
    public static BrushX Empty { get; } = new BrushX(NamedColors.Transparent);


    /// <summary>
    /// Gets the default texture brush image.
    /// </summary>
    public static ImageProxy DefaultTextureBrush
    {
      get
      {
        var pair = TextureManager.BuiltinTextures.FirstOrDefault();
        return pair.Value ?? new Graph.Gdi.HatchBrushes.HorizontalHatchBrush();
      }
    }

    /// <summary>
    /// Gets the default hatch brush image.
    /// </summary>
    public static ImageProxy DefaultHatchBrush
    {
      get
      {
        return new Graph.Gdi.HatchBrushes.HorizontalHatchBrush();
      }
    }

    /// <summary>
    /// Gets the default synthetic brush image.
    /// </summary>
    public static ImageProxy DefaultSyntheticBrush
    {
      get
      {
        return new Graph.Gdi.SyntheticBrushes.RandomCircles();
      }
    }

    #endregion static members

    #region Helpers

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
          return new Graph.Gdi.HatchBrushes.HorizontalHatchBrush();

        case HatchStyle.Vertical:
          return new Graph.Gdi.HatchBrushes.VerticalHatchBrush();

        case HatchStyle.ForwardDiagonal:
          return new Graph.Gdi.HatchBrushes.ForwardDiagonalHatchBrush();

        case HatchStyle.BackwardDiagonal:
          return new Graph.Gdi.HatchBrushes.BackwardDiagonalHatchBrush();

        case HatchStyle.Cross:
          return new Graph.Gdi.HatchBrushes.CrossHatchBrush();

        case HatchStyle.DiagonalCross:
          return new Graph.Gdi.HatchBrushes.DiagonalCrossHatchBrush();

        case HatchStyle.SolidDiamond:
          return new Graph.Gdi.HatchBrushes.DiamondHatchBrush();

        case HatchStyle.LargeCheckerBoard:
        case HatchStyle.SmallCheckerBoard:
          return new Graph.Gdi.HatchBrushes.CheckerHatchBrush();

        case HatchStyle.Sphere:
          return new Graph.Gdi.HatchBrushes.CircleHatchBrush();

        case HatchStyle.HorizontalBrick:
          return new Graph.Gdi.HatchBrushes.BrickHatchBrush();
      }
    }

    #endregion Helpers

  } // end of class BrushX


} // end of namespace
