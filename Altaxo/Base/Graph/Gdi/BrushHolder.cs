#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2005 Dr. Dirk Lellinger
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
#endregion

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.Serialization;

namespace Altaxo.Graph.Gdi
{
  [Serializable]
  public enum BrushType
  {
    SolidBrush,
    HatchBrush,
    TextureBrush,
    LinearGradientBrush,
    PathGradientBrush
  };

  [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase","Altaxo.Graph.BrushType", 0)]
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
  public class BrushHolder : System.ICloneable, System.IDisposable, Main.IChangedEventSource
  {

    protected BrushType _brushType; // Type of the brush
    protected Color _foreColor; // Color of the brush
    protected Color _backColor; // Backcolor of brush, f.i.f. HatchStyle brushes
    protected HatchStyle _hatchStyle; // Attention: is not serializable!
    protected ImageProxy _textureImage; // für Texturebrush
    protected WrapMode _wrapMode; // für TextureBrush und LinearGradientBrush
    protected RectangleF _brushBoundingRectangle;
    protected float _focus;
    protected float _scale;
    protected bool _bool1;
    protected LinearGradientModeEx _gradientMode;
    protected LinearGradientShape _gradientShape;

    [field:NonSerialized]
    public event System.EventHandler Changed;

    [NonSerialized]
    protected Brush _cachedBrush;      // this is the cached brush object

    #region "Serialization"



    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.BrushHolder", 0)]
    public class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        BrushHolder s = (BrushHolder)obj;
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
      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {

        BrushHolder s = null != o ? (BrushHolder)o : new BrushHolder(Color.Black);

        s._brushType = (BrushType)info.GetValue("Type", s);
        switch (s._brushType)
        {
          case BrushType.SolidBrush:
            s._foreColor = (Color)info.GetValue("ForeColor", s);
            break;
          case BrushType.HatchBrush:
            s._foreColor = (Color)info.GetValue("ForeColor", s);
            s._backColor = (Color)info.GetValue("BackColor", s);
            s._hatchStyle = (HatchStyle)info.GetEnum("HatchStyle", typeof(HatchStyle));
            break;
        }
        return s;
      }
    }


    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase","Altaxo.Graph.BrushHolder", 1)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(BrushHolder), 2)]
    public class XmlSerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        BrushHolder s = (BrushHolder)obj;
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
            info.AddValue("Scale", s._scale);
            info.AddValue("Focus", s._focus);
            break;
          case BrushType.PathGradientBrush:
            info.AddValue("ForeColor", s._foreColor);
            info.AddValue("BackColor", s._backColor);
            info.AddEnum("WrapMode", s._wrapMode);
            break;
          case BrushType.TextureBrush:
            info.AddValue("Texture", s._textureImage);
            info.AddEnum("WrapMode", s._wrapMode);
            info.AddValue("Scale", s._scale);
            break;
        } // end of switch
      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {

        BrushHolder s = null != o ? (BrushHolder)o : new BrushHolder(Color.Black);

        s._brushType = (BrushType)info.GetValue("Type", s);
        switch (s._brushType)
        {
          case BrushType.SolidBrush:
            s._foreColor = (Color)info.GetValue("ForeColor", s);
            break;
          case BrushType.HatchBrush:
            s._foreColor = (Color)info.GetValue("ForeColor", s);
            s._backColor = (Color)info.GetValue("BackColor", s);
            s._hatchStyle = (HatchStyle)info.GetEnum("HatchStyle", typeof(HatchStyle));
            break;
          case BrushType.LinearGradientBrush:
             s._foreColor = (Color)info.GetValue("ForeColor",s);
             s._backColor = (Color)info.GetValue("BackColor", s);
             s._wrapMode = (WrapMode)info.GetEnum("WrapMode", typeof(WrapMode));
             s._gradientMode = (LinearGradientModeEx)info.GetEnum("GradientMode", typeof(LinearGradientModeEx));
            s._gradientShape = (LinearGradientShape)info.GetEnum("GradientShape", typeof(LinearGradientShape));
            s._scale = info.GetSingle("Scale");
            s._focus = info.GetSingle("Focus");
            break;
          case BrushType.PathGradientBrush:
            s._foreColor = (Color)info.GetValue("ForeColor", s);
            s._backColor = (Color)info.GetValue("BackColor", s);
            s._wrapMode = (WrapMode)info.GetEnum("WrapMode", typeof(WrapMode));
            break;
          case BrushType.TextureBrush:
            s.TextureImage = (ImageProxy)info.GetValue("Texture", s);
            s._wrapMode = (WrapMode)info.GetEnum("WrapMode", typeof(WrapMode));
            s._scale = info.GetSingle("Scale");
            break;
        }
        return s;
      }
    }


   
    #endregion

    public BrushHolder(BrushHolder from)
    {
      _brushType = from._brushType; // Type of the brush
      _cachedBrush = null;      // this is the cached brush object
      _foreColor = from._foreColor; // Color of the brush
      _backColor = from._backColor; // Backcolor of brush, f.i.f. HatchStyle brushes
      _hatchStyle = from._hatchStyle; // für HatchBrush
      _textureImage = null == from._textureImage ? null : from._textureImage.Clone(); // für Texturebrush
      _wrapMode = from._wrapMode; // für TextureBrush und LinearGradientBrush
      _brushBoundingRectangle = from._brushBoundingRectangle;
      _focus = from._focus;
      _bool1 = from._bool1;
      this._gradientMode = from._gradientMode;
      this._gradientShape = from._gradientShape;
      this._scale = from._scale;
    }


    public BrushHolder(Color c)
    {
      this._brushType = BrushType.SolidBrush;
      this._foreColor = c;
    }

    public static implicit operator System.Drawing.Brush(BrushHolder bh)
    {
      return bh.Brush;
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
          OnChanged();
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
      switch (newValue)
      {
        case BrushType.LinearGradientBrush:
          _scale = 0;
          break;
        case BrushType.TextureBrush:
          _scale = 1;
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
        return !(_brushType == BrushType.SolidBrush && _foreColor.A == 0);
      }
    }

    /// <summary>
    /// Returns true if the brush is invisible, i.e. is a solid and transparent brush.
    /// </summary>
    public bool IsInvisible
    {
      get
      {
        return _brushType == BrushType.SolidBrush && _foreColor.A == 0;
      }
    }

    public Color Color
    {
      get { return _foreColor; }
      set
      {
        bool bChanged = (_foreColor != value);
        _foreColor = value;
        if (bChanged)
        {
          _SetBrushVariable(null);
          OnChanged();
        }
      }
    }

    public Color BackColor
    {
      get { return _backColor; }
      set
      {
        bool bChanged = (_backColor != value);
        _backColor = value;
        if (bChanged)
        {
          _SetBrushVariable(null);
          OnChanged();
        }
      }
    }

    public HatchStyle HatchStyle
    {
      get
      {
        return _hatchStyle;
      }
      set
      {
        bool bChanged = (_hatchStyle != value);
        _hatchStyle = value;
        if (bChanged)
        {
          _SetBrushVariable(null);
          OnChanged();
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
          OnChanged();
        }
      }
    }

    public LinearGradientModeEx GradientMode
    {
      get
      {
        return _gradientMode;
      }
      set
      {
        bool bChanged = (_gradientMode != value);
        _gradientMode = value;
        if (bChanged)
        {
          _SetBrushVariable(null);
          OnChanged();
        }
      }
    }
    public static void ToLinearGradientMode(LinearGradientModeEx mode, out LinearGradientMode lgm, out bool reverse)
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

    public LinearGradientShape GradientShape
    {
      get
      {
        return _gradientShape;
      }
      set
      {
        bool bChanged = (_gradientShape != value);
        _gradientShape = value;
        if (bChanged)
        {
          _SetBrushVariable(null);
          OnChanged();
        }
      }
    }

    public float GradientFocus
    {
      get
      {
        return _focus;
      }
      set
      {
        bool bChanged = (_focus != value);
        _focus = value;
        if (bChanged)
        {
          _SetBrushVariable(null);
          OnChanged();
        }
      }
    }

    public float GradientScale
    {
      get
      {
        return _scale;
      }
      set
      {
        bool bChanged = (_scale != value);
        _scale = value;
        if (bChanged)
        {
          _SetBrushVariable(null);
          OnChanged();
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
        bool bChanged = _textureImage != value;
        _textureImage = value;
        if (bChanged)
        {
          _SetBrushVariable(null);
          OnChanged();
        }
      }
    }

    public float TextureScale
    {
      get
      {
        return _scale;
      }
      set
      {
        this.GradientScale = value;
      }
    }

    public RectangleF Rectangle
    {
      get
      {
        return _brushBoundingRectangle;
      }
      set
      {
        if (_brushType == BrushType.LinearGradientBrush || _brushType == BrushType.PathGradientBrush)
        {
          bool bChanged = (_brushBoundingRectangle != value);
          _brushBoundingRectangle = value;
          if (bChanged)
          {
            _SetBrushVariable(null);
            OnChanged();
          }
        }
        else
        {
          _brushBoundingRectangle = value; // has no meaning for other brushes, so we set it but dont care
        }
      }
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
              br = new SolidBrush(_foreColor);
              break;
            case BrushType.HatchBrush:
              br = new HatchBrush(_hatchStyle, _foreColor, _backColor);
              break;
            case BrushType.LinearGradientBrush:
              if (_brushBoundingRectangle.IsEmpty)
                _brushBoundingRectangle = new RectangleF(0, 0, 1000, 1000);
              LinearGradientBrush lgb;
              LinearGradientMode lgmode;
              bool reverse;
              BrushHolder.ToLinearGradientMode(_gradientMode, out lgmode, out reverse);
              br = lgb = new LinearGradientBrush(_brushBoundingRectangle, reverse ? _backColor : _foreColor, reverse ? _foreColor : _backColor, lgmode);
              if (_wrapMode != WrapMode.Clamp)
                lgb.WrapMode = _wrapMode;
              if (_gradientShape == LinearGradientShape.Triangular)
                lgb.SetBlendTriangularShape(_focus, _scale);
              else if (_gradientShape == LinearGradientShape.SigmaBell)
                lgb.SetSigmaBellShape(_focus, _scale);
              break;
            case BrushType.PathGradientBrush:
              GraphicsPath p = new GraphicsPath();
              if (_brushBoundingRectangle.IsEmpty)
                _brushBoundingRectangle = new RectangleF(0, 0, 1000, 1000);
              p.AddRectangle(_brushBoundingRectangle);
              PathGradientBrush pgb = new PathGradientBrush(p);
              pgb.SurroundColors = new Color[] { _foreColor };
              pgb.CenterColor = _backColor;
              pgb.WrapMode = _wrapMode;
              br = pgb;
              break;
            case BrushType.TextureBrush:
              Image img = null != _textureImage ? _textureImage.GetImage() : null;
              if (img == null)
              {
                img = Current.MainWindow.Icon.ToBitmap();
              }
              TextureBrush tb = new TextureBrush(img);
              tb.WrapMode = this._wrapMode;
              if (_scale != 1)
                tb.ScaleTransform(_scale, _scale);
              br = tb;
              break;
          } // end of switch
          this._SetBrushVariable(br);
        }
        return _cachedBrush;
      } // end of get
    } // end of prop. Brush



    public void SetSolidBrush(Color c)
    {
      _brushType = BrushType.SolidBrush;
      _foreColor = c;
      _SetBrushVariable(null);
      OnChanged();
    }

    public void SetHatchBrush(HatchStyle hs, Color fc)
    {
      SetHatchBrush(hs, fc, Color.Black);
    }

    public void SetHatchBrush(HatchStyle hs, Color fc, Color bc)
    {
      _brushType = BrushType.HatchBrush;
      _hatchStyle = hs;
      _foreColor = fc;
      _backColor = bc;

      _SetBrushVariable(null);
      OnChanged();
    }

    protected void _SetBrushVariable(Brush br)
    {
      if (null != _cachedBrush)
        _cachedBrush.Dispose();

      _cachedBrush = br;
    }

    object ICloneable.Clone()
    {
      return new BrushHolder(this);
    }

    public BrushHolder Clone()
    {
      return new BrushHolder(this);
    }

    public void Dispose()
    {
      if (null != _cachedBrush)
        _cachedBrush.Dispose();
      _cachedBrush = null;
    }
    #region IChangedEventSource Members

 

    protected virtual void OnChanged()
    {
      if (null != Changed)
        Changed(this, new EventArgs());
    }

    #endregion
  } // end of class BrushHolder
} // end of namespace
