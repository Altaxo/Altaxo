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


namespace Altaxo.Graph
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

  [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(BrushType),0)]
  public class BrushTypeXmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
  {
    public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
    {
      info.SetNodeContent(obj.ToString()); 
    }
    public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
    {
      
      string val = info.GetNodeContent();
      return System.Enum.Parse(typeof(BrushType),val,true);
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
  public class BrushHolder : System.ICloneable, System.IDisposable, ISerializable, IDeserializationCallback, Main.IChangedEventSource
  {

    protected BrushType m_BrushType; // Type of the brush
    protected Brush     m_Brush;      // this is the cached brush object

    protected Color     m_ForeColor; // Color of the brush
    protected Color     m_BackColor; // Backcolor of brush, f.i.f. HatchStyle brushes
    protected HatchStyle  m_HatchStyle; // für HatchBrush
    protected Image     m_Image; // für Texturebrush
    protected Matrix    m_Matrix; // für TextureBrush
    protected WrapMode  m_WrapMode; // für TextureBrush und LinearGradientBrush
    protected RectangleF m_Rectangle;
    protected float     m_Float1;
    protected float m_Scale;
    protected bool      m_Bool1;
    protected LinearGradientModeEx m_GradientMode;
    protected LinearGradientShape m_GradientShape;

    #region "Serialization"

    protected BrushHolder(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
    {
      SetObjectData(this, info, context, null);
    }
    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {

      info.AddValue("Type", m_BrushType);
      switch (m_BrushType)
      {
        case BrushType.SolidBrush:
          info.AddValue("ForeColor", m_ForeColor);
          break;
        case BrushType.HatchBrush:
          info.AddValue("ForeColor", m_ForeColor);
          info.AddValue("BackColor", m_BackColor);
          info.AddValue("HatchStyle", m_HatchStyle);
          break;
      } // end of switch
    }
    public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
    {
      BrushHolder s = (BrushHolder)obj;
      s.m_BrushType = (BrushType)info.GetValue("Type", typeof(BrushType));
      switch (s.m_BrushType)
      {
        case BrushType.SolidBrush:
          s.m_ForeColor = (Color)info.GetValue("ForeColor", typeof(Color));
          break;
        case BrushType.HatchBrush:
          s.m_ForeColor = (Color)info.GetValue("ForeColor", typeof(Color));
          s.m_BackColor = (Color)info.GetValue("BackColor", typeof(Color));
          break;
      }
      return s;
    } // end of SetObjectData
   


    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(BrushHolder),0)]
      public class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        BrushHolder s = (BrushHolder)obj;
        info.AddValue("Type",s.m_BrushType);
        switch(s.m_BrushType)
        {
          case BrushType.SolidBrush:
            info.AddValue("ForeColor",s.m_ForeColor);
            break;
          case BrushType.HatchBrush:
            info.AddValue("ForeColor",s.m_ForeColor);
            info.AddValue("BackColor",s.m_BackColor);
            info.AddEnum("HatchStyle",s.m_HatchStyle);
            break;
        } // end of switch
      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        
        BrushHolder s = null!=o ? (BrushHolder)o : new BrushHolder(Color.Black);

        s.m_BrushType  = (BrushType)info.GetValue("Type",s);
        switch(s.m_BrushType)
        {
          case BrushType.SolidBrush:
            s.m_ForeColor = (Color)info.GetValue("ForeColor",s);
            break;
          case BrushType.HatchBrush:
            s.m_ForeColor = (Color)info.GetValue("ForeColor",s);
            s.m_BackColor = (Color)info.GetValue("BackColor",s);
            s.m_HatchStyle = (HatchStyle)info.GetEnum("HatchStyle",typeof(HatchStyle));
            break;
        }
        return s;
      }
    }


    /// <summary>
    /// Finale measures after deserialization of the linear axis.
    /// </summary>
    /// <param name="obj">Not used.</param>
    public virtual void OnDeserialization(object obj)
    {
    }
    #endregion

    public BrushHolder(BrushHolder from)
    {
      m_BrushType   = from.m_BrushType; // Type of the brush
      m_Brush = null;      // this is the cached brush object
      m_ForeColor   = from.m_ForeColor; // Color of the brush
      m_BackColor   = from.m_BackColor; // Backcolor of brush, f.i.f. HatchStyle brushes
      m_HatchStyle  = from.m_HatchStyle; // für HatchBrush
      m_Image       = null==from.m_Image ? null : (Image)from.m_Image.Clone(); // für Texturebrush
      m_Matrix      = null==from.m_Matrix ? null : (Matrix)from.m_Matrix.Clone(); // für TextureBrush
      m_WrapMode    = from.m_WrapMode; // für TextureBrush und LinearGradientBrush
      m_Rectangle   = from.m_Rectangle;
      m_Float1      = from.m_Float1;
      m_Bool1       = from.m_Bool1;
      this.m_GradientMode = from.m_GradientMode;
      this.m_GradientShape = from.m_GradientShape;
      this.m_Scale = from.m_Scale;
    }

  
    public BrushHolder(Color c)
    {
      this.m_BrushType = BrushType.SolidBrush;
      this.m_ForeColor = c;
    }

    public static implicit operator System.Drawing.Brush(BrushHolder bh)
    {
      return bh.Brush;
    }
 

    public BrushType BrushType
    {
      get 
      {
        return this.m_BrushType; 
      }
      set
      {
        BrushType oldValue = this.m_BrushType;
        m_BrushType = value;
        if (m_BrushType != oldValue)
        {
          _SetBrushVariable(null);
          OnChanged();
        }
      }
    }

    /// <summary>
    /// Returns true if the brush is visible, i.e. is not a transparent brush.
    /// </summary>
    public bool IsVisible
    {
      get
      {
        return !(m_BrushType == BrushType.SolidBrush && m_ForeColor.A == 0);
      }
    }

    /// <summary>
    /// Returns true if the brush is invisible, i.e. is a solid and transparent brush.
    /// </summary>
    public bool IsInvisible
    {
      get
      {
        return m_BrushType == BrushType.SolidBrush && m_ForeColor.A == 0;
      }
    }

    public Color Color
    {
      get { return m_ForeColor; }
      set
      {
        bool bChanged = (m_ForeColor!=value);
        m_ForeColor = value;
        if (bChanged)
        {
          _SetBrushVariable(null);
          OnChanged();
        }
      }
    }

    public Color BackColor
    {
      get { return m_BackColor; }
      set
      {
        bool bChanged = (m_BackColor!=value);
        m_BackColor = value;
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
        return m_HatchStyle;
      }
      set
      {
        bool bChanged = (m_HatchStyle != value);
        m_HatchStyle = value;
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
        return m_WrapMode;
      }
      set
      {
        bool bChanged = (m_WrapMode != value);
        m_WrapMode = value;
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
        return m_GradientMode;
      }
      set
      {
        bool bChanged = (m_GradientMode != value);
        m_GradientMode = value;
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
        return m_GradientShape;
      }
      set
      {
        bool bChanged = (m_GradientShape != value);
        m_GradientShape = value;
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
        return m_Float1;
      }
      set
      {
        bool bChanged = (m_Float1 != value);
        m_Float1 = value;
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
        return m_Scale;
      }
      set
      {
        bool bChanged = (m_Scale != value);
        m_Scale = value;
        if (bChanged)
        {
          _SetBrushVariable(null);
          OnChanged();
        }
      }
    }

    public RectangleF Rectangle
    {
      get
      {
        return m_Rectangle;
      }
      set
      {
        if (m_BrushType == BrushType.LinearGradientBrush || m_BrushType == BrushType.PathGradientBrush)
        {
          bool bChanged = (m_Rectangle != value);
          m_Rectangle = value;
          if (bChanged)
          {
            _SetBrushVariable(null);
            OnChanged();
          }
        }
        else
        {
          m_Rectangle = value; // has no meaning for other brushes, so we set it but dont care
        }
      }
    }


    public Brush Brush
    {
      get
      {
        if (m_Brush == null)
        {
          Brush br = null;
          switch (m_BrushType)
          {
            case BrushType.SolidBrush:
              br = new SolidBrush(m_ForeColor);
              break;
            case BrushType.HatchBrush:
              br = new HatchBrush(m_HatchStyle, m_ForeColor, m_BackColor);
              break;
            case BrushType.LinearGradientBrush:
              if (m_Rectangle.IsEmpty)
                m_Rectangle = new RectangleF(0, 0, 1000, 1000);
              LinearGradientBrush lgb;
              LinearGradientMode lgmode;
              bool reverse;
              BrushHolder.ToLinearGradientMode(m_GradientMode, out lgmode, out reverse);
              br = lgb = new LinearGradientBrush(m_Rectangle, reverse?m_BackColor:m_ForeColor, reverse?m_ForeColor:m_BackColor, lgmode);
              if(m_WrapMode!= WrapMode.Clamp)
                lgb.WrapMode = m_WrapMode;
              if (m_GradientShape == LinearGradientShape.Triangular)
                lgb.SetBlendTriangularShape(m_Float1, m_Scale);
              else if (m_GradientShape == LinearGradientShape.SigmaBell)
                lgb.SetSigmaBellShape(m_Float1, m_Scale);
              break;
            case BrushType.PathGradientBrush:
              GraphicsPath p = new GraphicsPath();
              if (m_Rectangle.IsEmpty)
                m_Rectangle = new RectangleF(0, 0, 1000, 1000);
              p.AddRectangle(m_Rectangle);
              PathGradientBrush pgb =  new PathGradientBrush(p);
              pgb.SurroundColors = new Color[] { m_ForeColor };
              pgb.CenterColor = m_BackColor;
              pgb.WrapMode = m_WrapMode;
              br = pgb;
              break;
            case BrushType.TextureBrush:
              TextureBrush tb = new TextureBrush(System.Windows.Forms.Form.ActiveForm.Icon.ToBitmap());
              br = tb;
              break;
          } // end of switch
          this._SetBrushVariable(br);
        }
        return m_Brush;
      } // end of get
     } // end of prop. Brush

  

    public void SetSolidBrush(Color c)
    {
      m_BrushType = BrushType.SolidBrush;
      m_ForeColor     = c;
      _SetBrushVariable(null);
      OnChanged();
    }

    public void SetHatchBrush(HatchStyle hs, Color fc)
    {
      SetHatchBrush(hs,fc,Color.Black);
    }

    public void SetHatchBrush(HatchStyle hs, Color fc, Color bc)
    {
      m_BrushType = BrushType.HatchBrush;
      m_HatchStyle = hs;
      m_ForeColor = fc;
      m_BackColor = bc;

      _SetBrushVariable(null);
      OnChanged();
    }

    protected void _SetBrushVariable(Brush br)
    {
      if(null!=m_Brush)
        m_Brush.Dispose();

      m_Brush = br;
    }
    
    public object Clone()
    {
      return new BrushHolder(this);
    }

    public void Dispose()
    {
      if(null!=m_Brush)
        m_Brush.Dispose();
      m_Brush = null;
    }
    #region IChangedEventSource Members

    public event System.EventHandler Changed;

    protected virtual void OnChanged()
    {
      if(null!=Changed)
        Changed(this, new EventArgs());
    }

    #endregion
  } // end of class BrushHolder
} // end of namespace
