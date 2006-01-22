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
  /// <summary>
  /// PenHolder is a serializable surrogate for a Pen object.
  /// you can implicitly or explicitly convert it to a pen, but you must
  /// not dispose the pen object you got from this, since the ownership has still the Penholder object
  /// if you convert a Penholder to a Pen, either implicitly or explicitly,
  /// the Cached property of the PenHolder is set to true to indicate that
  /// the PenHolder is holding a Pen object 
  /// </summary>
  [Serializable]
  public class PenHolder : ICloneable, IDisposable, ISerializable, IDeserializationCallback, Main.IChangedEventSource
  {
    protected PenHolder.Configured m_ConfiguredProperties; // ORed collection of the configured properties (i.e. non-standard properties) 
    protected PenType m_PenType; // the type of the pen
    protected bool m_CachedMode; // is a Pen object cached by this object
    protected PenAlignment m_Alignment; // Alignment of the Pen
    protected BrushHolder m_Brush; // the brush of this pen
    protected Color m_Color; // Color of this Pen object
    protected float[] m_CompoundArray;
    protected DashCap m_DashCap;
    protected float m_DashOffset;
    protected float[] m_DashPattern;
    protected DashStyle m_DashStyle;
    protected LineCap m_EndCap;
    protected LineJoin m_LineJoin;
    protected float m_MiterLimit;
    protected LineCap m_StartCap;
    protected Matrix m_Transform;
    protected float m_Width; // Widht of this Pen object
    [NonSerialized()]
    Pen m_Pen; // the cached pen object



    #region "ConfiguredProperties"

    [Serializable]
    [Flags]
    public enum Configured
    {
      IsNull = 0x00000,
      IsNotNull = 0x00001,
      InCachedMode = 0x00002,
      PenType = 0x00004,
      Alignment = 0x00008,
      Brush = 0x00010,
      Color = 0x00020,
      CompoundArray = 0x00040,
      DashStyle = 0x00080,
      DashCap = 0x00100,
      DashOffset = 0x00200,
      DashPattern = 0x00400,
      EndCap = 0x00800,
      StartCap = 0x01000,
      CustomEndCap = 0x02000,
      CustomStartCap = 0x04000,
      LineJoin = 0x08000,
      MiterLimit = 0x10000,
      Transform = 0x20000,
      Width = 0x40000,
      All = -1
    }
    /*
        public const int Configured.NotNull         = 0x00001;
        public const int Configured.InCachedMode    = 0x00002;
        public const int Configured.PenType         = 0x00004;
        public const int Configured.Alignment       = 0x00008;
        public const int Configured.Brush           = 0x00010;
        public const int Configured.Color           = 0x00020;
        public const int Configured.CompoundArray   = 0x00040;
        public const int Configured.DashStyle       = 0x00080;
        public const int Configured.DashCap         = 0x00100;
        public const int Configured.DashOffset      = 0x00200;
        public const int Configured.DashPattern     = 0x00400;
        public const int Configured.EndCap          = 0x00800;
        public const int Configured.StartCap        = 0x01000;
        public const int Configured.CustomEndCap    = 0x02000;
        public const int Configured.CustomStartCap  = 0x04000;
        public const int Configured.LineJoin        = 0x08000;
        public const int Configured.MiterLimit      = 0x10000;
        public const int Configured.Transform       = 0x20000;
        public const int Configured.Width           = 0x40000;
    */

    protected static Configured _GetConfiguredPropertiesVariable(Pen pen)
    {
      Configured c = Configured.IsNull;
      if (null == pen)
        return 0; // Pen is null, so nothing is configured

      c |= Configured.IsNotNull; // Pen is at least not null
      if (pen.PenType != PenType.SolidColor) c |= Configured.PenType;
      if (pen.PenType == PenType.SolidColor && pen.Color != Color.Black) c |= Configured.Color;
      if (pen.PenType != PenType.SolidColor) c |= Configured.Brush;
      if (pen.Alignment != PenAlignment.Center) c |= Configured.Alignment;
      if (pen.CompoundArray != null && pen.CompoundArray.Length > 0) c |= Configured.CompoundArray;
      if (pen.DashStyle != DashStyle.Solid) c |= Configured.DashStyle;
      if (pen.DashStyle != DashStyle.Solid && pen.DashCap != DashCap.Flat) c |= Configured.DashCap;
      if (pen.DashStyle != DashStyle.Solid && pen.DashOffset != 0) c |= Configured.DashOffset;
      if (pen.DashStyle == DashStyle.Custom && pen.DashPattern != null) c |= Configured.DashPattern;
      if (pen.EndCap != LineCap.Flat) c |= Configured.EndCap;
      if (pen.StartCap != LineCap.Flat) c |= Configured.StartCap;
      if (pen.EndCap != LineCap.Custom) c |= Configured.CustomEndCap;
      if (pen.StartCap != LineCap.Custom) c |= Configured.CustomStartCap;
      if (pen.LineJoin != LineJoin.Miter) c |= Configured.LineJoin;
      if (pen.MiterLimit != 10) c |= Configured.MiterLimit;
      if (!pen.Transform.IsIdentity) c |= Configured.Transform;
      if (pen.Width != 1) c |= Configured.Width;

      return c;
    }
    protected static Configured _GetConfiguredPropertiesVariable(PenHolder pen)
    {
      Configured c = Configured.IsNull;
      if (null == pen || pen.m_ConfiguredProperties == 0)
        return c; // Pen is null, so nothing is configured

      c |= Configured.IsNotNull; // Pen is at least not null
      if (pen.PenType != PenType.SolidColor) c |= Configured.PenType;
      if (pen.PenType == PenType.SolidColor && pen.Color != Color.Black) c |= Configured.Color;
      if (pen.PenType != PenType.SolidColor) c |= Configured.Brush;
      if (pen.Alignment != PenAlignment.Center) c |= Configured.Alignment;
      if (pen.CompoundArray != null && pen.CompoundArray.Length > 0) c |= Configured.CompoundArray;
      if (pen.DashStyle != DashStyle.Solid) c |= Configured.DashStyle;
      if (pen.DashStyle != DashStyle.Solid && pen.DashCap != DashCap.Flat) c |= Configured.DashCap;
      if (pen.DashStyle != DashStyle.Solid && pen.DashOffset != 0) c |= Configured.DashOffset;
      if (pen.DashStyle == DashStyle.Custom && pen.DashPattern != null) c |= Configured.DashPattern;
      if (pen.EndCap != LineCap.Flat) c |= Configured.EndCap;
      if (pen.StartCap != LineCap.Flat) c |= Configured.StartCap;
      if (pen.EndCap == LineCap.Custom) c |= Configured.CustomEndCap;
      if (pen.StartCap == LineCap.Custom) c |= Configured.CustomStartCap;
      if (pen.LineJoin != LineJoin.Miter) c |= Configured.LineJoin;
      if (pen.MiterLimit != 10) c |= Configured.MiterLimit;
      if (null != pen.Transform && !pen.Transform.IsIdentity) c |= Configured.Transform;
      if (pen.Width != 1) c |= Configured.Width;

      return c;
    }



    #endregion

    #region Serialization

    #region Clipboard serialization

    protected PenHolder(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
    {
      SetObjectData(this, info, context, null);
    }

    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {
      PenHolder s = this;
      Configured cp = PenHolder._GetConfiguredPropertiesVariable(s);
      if (s.Cached) cp |= PenHolder.Configured.InCachedMode;

      info.AddValue("Configured", (int)cp);
      if (0 != (cp & PenHolder.Configured.PenType))
        info.AddValue("Type", s.PenType);
      if (0 != (cp & PenHolder.Configured.Alignment))
        info.AddValue("Alignment", s.Alignment);
      if (0 != (cp & PenHolder.Configured.Brush))
        info.AddValue("Brush", s.BrushHolder);
      if (0 != (cp & PenHolder.Configured.Color))
        info.AddValue("Color", s.Color);
      if (0 != (cp & PenHolder.Configured.CompoundArray))
        info.AddValue("CompoundArray", s.CompoundArray);
      if (0 != (cp & PenHolder.Configured.DashStyle))
        info.AddValue("DashStyle", s.DashStyle);
      if (0 != (cp & PenHolder.Configured.DashCap))
        info.AddValue("DashCap", s.DashCap);
      if (0 != (cp & PenHolder.Configured.DashOffset))
        info.AddValue("DashOffset", s.DashOffset);
      if (0 != (cp & PenHolder.Configured.DashPattern))
        info.AddValue("DashPattern", s.DashPattern);
      if (0 != (cp & PenHolder.Configured.EndCap))
        info.AddValue("EndCap", s.EndCap);
      if (0 != (cp & PenHolder.Configured.LineJoin))
        info.AddValue("LineJoin", s.LineJoin);
      if (0 != (cp & PenHolder.Configured.MiterLimit))
        info.AddValue("MiterLimit", s.MiterLimit);
      if (0 != (cp & PenHolder.Configured.StartCap))
        info.AddValue("StartCap", s.StartCap);
      if (0 != (cp & PenHolder.Configured.Transform))
        info.AddValue("Transform", s.Transform.Elements);
      if (0 != (cp & PenHolder.Configured.Width))
        info.AddValue("Width", s.Width);
    }
    public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
    {
      PenHolder s = (PenHolder)obj;
      s.m_ConfiguredProperties = (Configured)info.GetInt32("Configured");
      Configured cp = s.m_ConfiguredProperties;

      // cache mode is disabled after serialization, and we
      // can not enable it till all objects are deserialized
      // (for instance sometimes the brushholder deserialization is finished
      // later then the pen itself)
      s.m_CachedMode = false;

      if (0 != (cp & PenHolder.Configured.PenType))
        s.m_PenType = (PenType)info.GetValue("Type", typeof(PenType));
      else
        s.m_PenType = PenType.SolidColor;

      if (0 != (cp & PenHolder.Configured.Alignment))
        s.m_Alignment = (PenAlignment)info.GetValue("Alignment", typeof(PenAlignment));
      else
        s.m_Alignment = PenAlignment.Center;

      if (0 != (cp & PenHolder.Configured.Brush))
        s.m_Brush = (BrushHolder)info.GetValue("Brush", typeof(BrushHolder));
      else
        s.m_Brush = new BrushHolder(Color.Black);

      if (0 != (cp & PenHolder.Configured.Color))
        s.m_Color = (Color)info.GetValue("Color", typeof(Color));
      else
        s.m_Color = Color.Black;

      if (0 != (cp & PenHolder.Configured.CompoundArray))
        s.m_CompoundArray = (float[])info.GetValue("CompoundArray", typeof(float[]));
      else
        s.m_CompoundArray = new float[0];

      if (0 != (cp & PenHolder.Configured.DashStyle))
        s.m_DashStyle = (DashStyle)info.GetValue("DashStyle", typeof(DashStyle));
      else
        s.m_DashStyle = DashStyle.Solid;

      if (0 != (cp & PenHolder.Configured.DashCap))
        s.m_DashCap = (DashCap)info.GetValue("DashCap", typeof(DashCap));
      else
        s.m_DashCap = DashCap.Flat;

      if (0 != (cp & PenHolder.Configured.DashOffset))
        s.m_DashOffset = (float)info.GetSingle("DashOffset");
      else
        s.m_DashOffset = 0;

      if (0 != (cp & PenHolder.Configured.DashPattern))
        s.m_DashPattern = (float[])info.GetValue("DashPattern", typeof(float[]));
      else
        s.m_DashPattern = null;

      if (0 != (cp & PenHolder.Configured.EndCap))
        s.m_EndCap = (LineCap)info.GetValue("EndCap", typeof(LineCap));
      else
        s.m_EndCap = LineCap.Flat;

      if (0 != (cp & PenHolder.Configured.LineJoin))
        s.m_LineJoin = (LineJoin)info.GetValue("LineJoin", typeof(LineJoin));
      else
        s.m_LineJoin = LineJoin.Miter;

      if (0 != (cp & PenHolder.Configured.MiterLimit))
        s.m_MiterLimit = info.GetSingle("MiterLimit");
      else
        s.m_MiterLimit = 10;

      if (0 != (cp & PenHolder.Configured.StartCap))
        s.m_StartCap = (LineCap)info.GetValue("StartCap", typeof(LineCap));
      else
        s.m_StartCap = LineCap.Flat;

      if (0 != (cp & PenHolder.Configured.Transform))
      {
        float[] el = (float[])info.GetValue("Transform", typeof(float[]));
        s.m_Transform = new Matrix(el[0], el[1], el[2], el[3], el[4], el[5]);
      }
      else
        s.m_Transform = new Matrix();

      if (0 != (cp & PenHolder.Configured.Width))
        s.m_Width = info.GetSingle("Width");
      else
        s.m_Width = 1;

      return s;
    } // end of SetObjectData

    public virtual void OnDeserialization(object obj)
    {
      // wire the BrushHolder
      if (null != this.m_Brush)
        m_Brush.Changed += new EventHandler(this.OnBrushChangedEventHandler);
    }

    #endregion

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(PenHolder), 0)]
    public class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        PenHolder s = (PenHolder)obj;
        Configured cp = PenHolder._GetConfiguredPropertiesVariable(s);
        if (s.Cached) cp |= PenHolder.Configured.InCachedMode;

        info.AddValue("Configured", (int)cp);
        if (0 != (cp & PenHolder.Configured.PenType))
          info.AddEnum("Type", s.PenType);
        if (0 != (cp & PenHolder.Configured.Alignment))
          info.AddEnum("Alignment", s.Alignment);
        if (0 != (cp & PenHolder.Configured.Brush))
          info.AddValue("Brush", s.BrushHolder);
        if (0 != (cp & PenHolder.Configured.Color))
          info.AddValue("Color", s.Color);
        if (0 != (cp & PenHolder.Configured.CompoundArray))
          info.AddArray("CompoundArray", s.CompoundArray, s.CompoundArray.Length);
        if (0 != (cp & PenHolder.Configured.DashStyle))
          info.AddEnum("DashStyle", s.DashStyle);
        if (0 != (cp & PenHolder.Configured.DashCap))
          info.AddEnum("DashCap", s.DashCap);
        if (0 != (cp & PenHolder.Configured.DashOffset))
          info.AddValue("DashOffset", s.DashOffset);
        if (0 != (cp & PenHolder.Configured.DashPattern))
          info.AddArray("DashPattern", s.DashPattern, s.DashPattern.Length);
        if (0 != (cp & PenHolder.Configured.EndCap))
          info.AddEnum("EndCap", s.EndCap);
        if (0 != (cp & PenHolder.Configured.LineJoin))
          info.AddEnum("LineJoin", s.LineJoin);
        if (0 != (cp & PenHolder.Configured.MiterLimit))
          info.AddValue("MiterLimit", s.MiterLimit);
        if (0 != (cp & PenHolder.Configured.StartCap))
          info.AddEnum("StartCap", s.StartCap);
        if (0 != (cp & PenHolder.Configured.Transform))
          info.AddArray("Transform", s.Transform.Elements, s.Transform.Elements.Length);
        if (0 != (cp & PenHolder.Configured.Width))
          info.AddValue("Width", s.Width);
      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {

        PenHolder s = null != o ? (PenHolder)o : new PenHolder();

        s.m_ConfiguredProperties = (Configured)info.GetInt32("Configured");
        Configured cp = s.m_ConfiguredProperties;

        // cache mode is disabled after serialization, and we
        // can not enable it till all objects are deserialized
        // (for instance sometimes the brushholder deserialization is finished
        // later then the pen itself)
        s.m_CachedMode = false;

        if (0 != (cp & PenHolder.Configured.PenType))
          s.m_PenType = (PenType)info.GetEnum("Type", typeof(PenType));
        else
          s.m_PenType = PenType.SolidColor;

        if (0 != (cp & PenHolder.Configured.Alignment))
          s.m_Alignment = (PenAlignment)info.GetEnum("Alignment", typeof(PenAlignment));
        else
          s.m_Alignment = PenAlignment.Center;

        if (0 != (cp & PenHolder.Configured.Brush))
          s.m_Brush = (BrushHolder)info.GetValue("Brush", typeof(BrushHolder));
        else
          s.m_Brush = new BrushHolder(Color.Black);

        if (0 != (cp & PenHolder.Configured.Color))
          s.m_Color = (Color)info.GetValue("Color", typeof(Color));
        else
          s.m_Color = Color.Black;

        if (0 != (cp & PenHolder.Configured.CompoundArray))
          info.GetArray(out s.m_CompoundArray);
        else
          s.m_CompoundArray = new float[0];

        if (0 != (cp & PenHolder.Configured.DashStyle))
          s.m_DashStyle = (DashStyle)info.GetEnum("DashStyle", typeof(DashStyle));
        else
          s.m_DashStyle = DashStyle.Solid;

        if (0 != (cp & PenHolder.Configured.DashCap))
          s.m_DashCap = (DashCap)info.GetEnum("DashCap", typeof(DashCap));
        else
          s.m_DashCap = DashCap.Flat;

        if (0 != (cp & PenHolder.Configured.DashOffset))
          s.m_DashOffset = (float)info.GetSingle("DashOffset");
        else
          s.m_DashOffset = 0;

        if (0 != (cp & PenHolder.Configured.DashPattern))
          info.GetArray(out s.m_DashPattern);
        else
          s.m_DashPattern = null;

        if (0 != (cp & PenHolder.Configured.EndCap))
          s.m_EndCap = (LineCap)info.GetEnum("EndCap", typeof(LineCap));
        else
          s.m_EndCap = LineCap.Flat;

        if (0 != (cp & PenHolder.Configured.LineJoin))
          s.m_LineJoin = (LineJoin)info.GetEnum("LineJoin", typeof(LineJoin));
        else
          s.m_LineJoin = LineJoin.Miter;

        if (0 != (cp & PenHolder.Configured.MiterLimit))
          s.m_MiterLimit = info.GetSingle("MiterLimit");
        else
          s.m_MiterLimit = 10;

        if (0 != (cp & PenHolder.Configured.StartCap))
          s.m_StartCap = (LineCap)info.GetEnum("StartCap", typeof(LineCap));
        else
          s.m_StartCap = LineCap.Flat;

        if (0 != (cp & PenHolder.Configured.Transform))
        {
          float[] el;
          info.GetArray(out el);
          s.m_Transform = new Matrix(el[0], el[1], el[2], el[3], el[4], el[5]);
        }
        else
          s.m_Transform = new Matrix();

        if (0 != (cp & PenHolder.Configured.Width))
          s.m_Width = info.GetSingle("Width");
        else
          s.m_Width = 1;

        return s;
      }
    }





    #endregion

    public PenHolder()
    {
    }

    public PenHolder(Color c)
      : this(c, 1, false)
    {
    }

    public PenHolder(Color c, float width)
      : this(c, width, false)
    {
    }

    public PenHolder(Color c, float width, bool bCachedMode)
    {
      this.m_CachedMode = bCachedMode;
      this.m_PenType = PenType.SolidColor;
      this.m_Color = c;
      this.m_Width = width;

      _SetProp(PenHolder.Configured.IsNotNull, true);
      _SetProp(Configured.Color, Color.Black.ToArgb() != c.ToArgb());
      _SetProp(Configured.Width, 1 != width);

      if (bCachedMode)
        _SetPenVariable(new Pen(c, width));
    }


    /// <summary>
    /// Copy constructor of PenHolder
    /// </summary>
    /// <param name="pen">the PenHolder object to copy</param>
    public PenHolder(PenHolder pen)
    {
      CopyFrom(pen);
    }

    /// <summary>
    /// Copies the properties of another instance to this instance.
    /// </summary>
    /// <param name="pen">the PenHolder object to copy</param>
    public void CopyFrom(PenHolder pen)
    {
      // this.m_CachedMode = pen.m_CachedMode;
      this.m_CachedMode = false; _SetPenVariable(null);

      this.m_ConfiguredProperties = pen.m_ConfiguredProperties;
      this.m_PenType = pen.PenType;
      this.m_Alignment = pen.Alignment;

      if (0 != (this.m_ConfiguredProperties & Configured.Brush))
        this.m_Brush = new BrushHolder(pen.Brush, false);

      this.m_Color = pen.Color;

      if (null != pen.m_CompoundArray)
        this.m_CompoundArray = (float[])pen.CompoundArray.Clone();
      else
        this.m_CompoundArray = null;

      this.m_DashCap = pen.DashCap;
      this.m_DashOffset = pen.DashOffset;

      if (null != pen.m_DashPattern)
        this.m_DashPattern = (float[])pen.DashPattern.Clone();
      else
        this.m_DashPattern = null;

      this.m_DashStyle = pen.DashStyle;
      this.m_EndCap = pen.EndCap;
      this.m_LineJoin = pen.LineJoin;
      this.m_MiterLimit = pen.MiterLimit;
      this.m_StartCap = pen.StartCap;

      if (null != pen.m_Transform)
        this.m_Transform = pen.Transform.Clone();
      else
        this.m_Transform = null;

      this.m_Width = pen.Width;

      // note: there is an problem with Pen.Clone() : if the Color of the pen
      // was set to a known color, the color of the cloned pen is the same, but no longer a known color
      // therefore we avoid the cloning of the pen here

      // if(m_CachedMode && null!=pen.m_Pen)
      //   _SetPenVariable( (Pen)pen.m_Pen.Clone() );
      // else
      //   _SetPenVariable(null);
    }

    public PenHolder(Pen pen)
      : this(pen, true)
    {
    }

    public PenHolder(Pen pen, bool bCached)
    {
      this.m_CachedMode = bCached;
      _SetPropertiesFromPen(pen);
      this.m_ConfiguredProperties = _GetConfiguredPropertiesVariable(pen);
      if (bCached)
        _SetPenVariable(_GetPenFromProperties()); // do not clone the pen because there is a problem with pen cloning with known colors (see above)
    }

    public static implicit operator System.Drawing.Pen(PenHolder ph)
    {
      ph.Cached = true; // if implicit conversion, we maybe are not aware that the pen is never destroyed, so _we_ control the pen by caching it
      return ph.Pen;
    }


    public Pen Pen
    {
      get
      {
        if (!m_CachedMode)
        {
          m_Pen = _GetPenFromProperties();
          m_CachedMode = true;
        }
        return m_Pen;
      }
      set
      {
        _SetPropertiesFromPen(value);
        this.m_ConfiguredProperties = _GetConfiguredPropertiesVariable(value);
        if (m_CachedMode)
          _SetPenVariable(_GetPenFromProperties()); // do not clone the pen because there is a problem with pen cloning with known colors (see above)

        OnChanged(); // Fire the Changed event
      }
    }

    public bool Cached
    {
      get { return this.m_CachedMode; }
      set
      {
        if (this.m_CachedMode == value)
          return; // no change in cache mode, so nothing to do here

        // if forced to cache mode, create and set the pen variable
        _SetPenVariable(value ? this._GetPenFromProperties() : null);
        m_CachedMode = value;
      }
    }

    protected void _SetPenVariable(Pen pen)
    {
      if (null != m_Pen)
        m_Pen.Dispose();
      m_Pen = pen;
    }

    protected void _SetPropertiesFromPen(Pen pen)
    {
      this.m_ConfiguredProperties = _GetConfiguredPropertiesVariable(pen);
      this.m_PenType = pen.PenType;
      this.m_Alignment = pen.Alignment;
      this.m_Brush = new BrushHolder(pen.Brush, false);
      this.m_Color = pen.Color;
      this.m_CompoundArray = (float[])pen.CompoundArray.Clone();
      this.m_DashCap = pen.DashCap;
      this.m_DashOffset = pen.DashOffset;
      this.m_DashPattern = (float[])pen.DashPattern.Clone();
      this.m_DashStyle = pen.DashStyle;
      this.m_EndCap = pen.EndCap;
      this.m_LineJoin = pen.LineJoin;
      this.m_MiterLimit = pen.MiterLimit;
      this.m_StartCap = pen.StartCap;
      this.m_Transform = (Matrix)pen.Transform.Clone();
      this.m_Width = pen.Width;
    }

    protected Pen _GetPenFromProperties()
    {
      Configured cp = this.m_ConfiguredProperties;

      if (0 == (cp & PenHolder.Configured.IsNotNull))
        return null;

      Pen pen = new Pen(Color.Black);

      // now set the optional Pen properties
      if (0 != (cp & PenHolder.Configured.Alignment))
        pen.Alignment = m_Alignment;

      if (0 != (cp & PenHolder.Configured.Brush))
        pen.Brush = this.m_Brush;
      if (0 != (cp & PenHolder.Configured.Color))
        pen.Color = this.m_Color;
      if (0 != (cp & PenHolder.Configured.CompoundArray))
        pen.CompoundArray = this.m_CompoundArray;
      if (0 != (cp & PenHolder.Configured.DashStyle))
        pen.DashStyle = this.m_DashStyle;
      if (0 != (cp & PenHolder.Configured.DashCap))
        pen.DashCap = this.m_DashCap;
      if (0 != (cp & PenHolder.Configured.DashOffset))
        pen.DashOffset = this.m_DashOffset;
      if (0 != (cp & PenHolder.Configured.DashPattern))
        pen.DashPattern = this.m_DashPattern;
      if (0 != (cp & PenHolder.Configured.EndCap))
        pen.EndCap = this.m_EndCap;
      if (0 != (cp & PenHolder.Configured.LineJoin))
        pen.LineJoin = this.m_LineJoin;
      if (0 != (cp & PenHolder.Configured.MiterLimit))
        pen.MiterLimit = this.m_MiterLimit;
      if (0 != (cp & PenHolder.Configured.StartCap))
        pen.StartCap = this.m_StartCap;
      if (0 != (cp & PenHolder.Configured.Transform))
        pen.Transform = this.m_Transform;
      if (0 != (cp & PenHolder.Configured.Width))
        pen.Width = this.m_Width;


      return pen;
    }

    private void _SetBrushVariable(BrushHolder bh)
    {
      if (null != m_Brush)
        m_Brush.Dispose();

      m_Brush = bh;
    }


    public PenType PenType
    {
      get { return this.m_CachedMode ? m_Pen.PenType : m_PenType; }
    }

    public PenAlignment Alignment
    {
      get { return this.m_CachedMode ? m_Pen.Alignment : m_Alignment; }
      set
      {
        _SetProp(Configured.Alignment, PenAlignment.Center != value);
        m_Alignment = value;
        if (m_CachedMode)
          m_Pen.Alignment = value;

        OnChanged(); // Fire the Changed event
      }
    }


    public Graph.BrushHolder BrushHolder
    {
      get
      {
        if (m_CachedMode)
          return new BrushHolder(m_Pen.Brush, false);
        else
          return m_Brush;

      }
    }

    public Brush Brush
    {
      get { return m_CachedMode ? m_Pen.Brush : m_Brush; }
      set
      {
        if (null == value)
        {
          _SetProp(Configured.PenType, false);
          _SetProp(Configured.Color, Color.Black != m_Color);
          m_PenType = PenType.SolidColor;
          _SetBrushVariable(null);
        }
        else if (value is SolidBrush)
        {
          m_PenType = PenType.SolidColor;
          m_Color = ((SolidBrush)value).Color;
          _SetBrushVariable(null);

          _SetProp(Configured.PenType, PenType.SolidColor != m_PenType);
          _SetProp(Configured.Color, Color.Black != m_Color);
          _SetProp(Configured.Brush, false);
        } // if value is SolidBrush
        else if (value is HatchBrush)
        {
          m_PenType = PenType.HatchFill;
          _SetBrushVariable(new BrushHolder(value, false));

          _SetProp(Configured.PenType, true);
          _SetProp(Configured.Color, false);
          _SetProp(Configured.Brush, true);
        }
        else if (value is TextureBrush)
        {
          m_PenType = PenType.TextureFill;
          _SetBrushVariable(new BrushHolder(value, false));

          _SetProp(Configured.PenType, true);
          _SetProp(Configured.Color, false);
          _SetProp(Configured.Brush, true);
        }
        else if (value is LinearGradientBrush)
        {
          m_PenType = PenType.LinearGradient;
          _SetBrushVariable(new BrushHolder(value, false));

          _SetProp(Configured.PenType, true);
          _SetProp(Configured.Color, false);
          _SetProp(Configured.Brush, true);
        }
        else if (value is PathGradientBrush)
        {
          m_PenType = PenType.PathGradient;
          _SetBrushVariable(new BrushHolder(value, false));

          _SetProp(Configured.PenType, true);
          _SetProp(Configured.Color, false);
          _SetProp(Configured.Brush, true);
        }

        // now set also the properties of the pen itself
        if (m_CachedMode)
          m_Pen.Brush = value;

        OnChanged(); // Fire the Changed event
      }
    }

    public Color Color
    {
      get { return m_CachedMode ? m_Pen.Color : m_Color; }
      set
      {
        _SetProp(Configured.PenType, false);
        _SetProp(Configured.Color, Color.Black != value);
        _SetProp(Configured.Brush, false);

        m_PenType = PenType.SolidColor;
        m_Color = value;
        m_Brush = null;

        if (m_CachedMode)
          m_Pen.Color = value;

        OnChanged(); // Fire the Changed event
      }
    }
    public float[] CompoundArray
    {
      get { return m_CachedMode ? m_Pen.CompoundArray : m_CompoundArray; }
      set
      {
        _SetProp(Configured.CompoundArray, null != value && value.Length > 0);
        m_CompoundArray = (float[])value.Clone();
        if (m_CachedMode)
          m_Pen.CompoundArray = value;

        OnChanged(); // Fire the Changed event
      }
    }
    public DashCap DashCap
    {
      get { return m_CachedMode ? m_Pen.DashCap : m_DashCap; }
      set
      {
        _SetProp(Configured.DashCap, DashCap.Flat != value);
        m_DashCap = value;
        if (m_CachedMode)
          m_Pen.DashCap = value;

        OnChanged(); // Fire the Changed event
      }
    }
    public float DashOffset
    {
      get { return m_CachedMode ? m_Pen.DashOffset : m_DashOffset; }
      set
      {
        _SetProp(Configured.DashOffset, 0 != value);
        m_DashOffset = value;
        if (m_CachedMode)
          m_Pen.DashOffset = value;

        OnChanged(); // Fire the Changed event
      }
    }
    public float[] DashPattern
    {
      get { return m_CachedMode ? m_Pen.DashPattern : m_DashPattern; }
      set
      {
        _SetProp(Configured.DashPattern, null != value && value.Length > 0);
        m_DashPattern = value;
        if (m_CachedMode)
          m_Pen.DashPattern = value;

        OnChanged(); // Fire the Changed event
      }
    }
    public DashStyle DashStyle
    {
      get { return m_CachedMode ? m_Pen.DashStyle : m_DashStyle; }
      set
      {
        _SetProp(Configured.DashStyle, DashStyle.Solid != value);
        m_DashStyle = value;
        if (m_CachedMode)
          m_Pen.DashStyle = value;

        OnChanged(); // Fire the Changed event
      }
    }
    public LineCap EndCap
    {
      get { return m_CachedMode ? m_Pen.EndCap : m_EndCap; }
      set
      {
        _SetProp(Configured.EndCap, LineCap.Flat != value);
        m_EndCap = value;
        if (m_CachedMode)
          m_Pen.EndCap = value;

        OnChanged(); // Fire the Changed event
      }
    }
    public LineJoin LineJoin
    {
      get { return m_CachedMode ? m_Pen.LineJoin : m_LineJoin; }
      set
      {
        _SetProp(Configured.LineJoin, LineJoin.Miter != value);
        m_LineJoin = value;
        if (m_CachedMode)
          m_Pen.LineJoin = value;

        OnChanged(); // Fire the Changed event
      }
    }
    public float MiterLimit
    {
      get { return m_CachedMode ? m_Pen.MiterLimit : m_MiterLimit; }
      set
      {
        _SetProp(Configured.MiterLimit, 10 != value);
        m_MiterLimit = value;
        if (m_CachedMode)
          m_Pen.MiterLimit = value;

        OnChanged(); // Fire the Changed event
      }
    }
    public LineCap StartCap
    {
      get { return m_CachedMode ? m_Pen.StartCap : m_StartCap; }
      set
      {
        _SetProp(Configured.StartCap, LineCap.Flat != value);
        m_StartCap = value;
        if (m_CachedMode)
          m_Pen.StartCap = value;

        OnChanged(); // Fire the Changed event
      }
    }
    public Matrix Transform
    {
      get { return m_CachedMode ? m_Pen.Transform : m_Transform; }
      set
      {
        _SetProp(Configured.Transform, null != value && !value.IsIdentity);
        m_Transform = value.Clone();
        if (m_CachedMode)
          m_Pen.Transform = value;

        OnChanged(); // Fire the Changed event
      }
    }

    public float Width
    {
      get { return m_CachedMode ? m_Pen.Width : m_Width; }
      set
      {
        _SetProp(Configured.Width, 1 != value);
        m_Width = value;
        if (m_CachedMode)
          m_Pen.Width = value;

        OnChanged(); // Fire the Changed event
      }
    }

    void _SetProp(Configured prop, bool bSet)
    {
      this.m_ConfiguredProperties &= (Configured.All ^ prop);
      if (bSet) this.m_ConfiguredProperties |= prop;

    }

    public object Clone()
    {
      return new PenHolder(this);
    }

    public void Dispose()
    {
      m_ConfiguredProperties = 0;
      if (null != m_Pen) { m_Pen.Dispose(); m_Pen = null; }
      if (null != m_Transform) { m_Transform.Dispose(); m_Transform = null; }
      if (null != m_CompoundArray) { m_CompoundArray = null; }
      if (null != this.m_DashPattern) { m_DashPattern = null; }
    }
    #region IChangedEventSource Members

    public event System.EventHandler Changed;



    protected virtual void OnChanged()
    {
      if (null != Changed)
        Changed(this, new System.EventArgs());
    }

    protected virtual void OnBrushChangedEventHandler(object sender, EventArgs e)
    {
      OnChanged();
    }

    #endregion
  } // end of class PenHolder
}
