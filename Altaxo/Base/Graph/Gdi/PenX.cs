#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2007 Dr. Dirk Lellinger
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
  #region PenX
  /// <summary>
  /// PenHolder is a serializable surrogate for a Pen object.
  /// you can implicitly or explicitly convert it to a pen, but you must
  /// not dispose the pen object you got from this, since the ownership has still the Penholder object
  /// if you convert a Penholder to a Pen, either implicitly or explicitly,
  /// the Cached property of the PenHolder is set to true to indicate that
  /// the PenHolder is holding a Pen object 
  /// </summary>
  [Serializable]
  public class PenX : ICloneable, IDisposable, ISerializable, IDeserializationCallback, Main.IChangedEventSource
  {
    protected PenX.Configured m_ConfiguredProperties; // ORed collection of the configured properties (i.e. non-standard properties) 
    protected PenType m_PenType; // the type of the pen
    protected PenAlignment m_Alignment; // Alignment of the Pen
    protected BrushX m_Brush; // the brush of this pen
    protected Color m_Color; // Color of this Pen object
    protected float[] m_CompoundArray;
    protected DashCap m_DashCap;
    protected float m_DashOffset;
    protected float[] m_DashPattern;
    protected DashStyle m_DashStyle;
    protected LineCapEx m_EndCap;
    protected LineJoin m_LineJoin;
    protected float m_MiterLimit;
    protected LineCapEx m_StartCap;
    protected Matrix m_Transform;
    protected float m_Width; // Width of this Pen object
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
    protected static Configured _GetConfiguredPropertiesVariable(PenX pen)
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
      if (!pen.EndCap.IsDefaultStyle) c |= Configured.EndCap;
      if (!pen.StartCap.IsDefaultStyle) c |= Configured.StartCap;
      if (pen.LineJoin != LineJoin.Miter) c |= Configured.LineJoin;
      if (pen.MiterLimit != 10) c |= Configured.MiterLimit;
      if (null != pen.Transform && !pen.Transform.IsIdentity) c |= Configured.Transform;
      if (pen.Width != 1) c |= Configured.Width;

      return c;
    }



    #endregion

    #region Serialization

    #region Clipboard serialization

    protected PenX(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
    {
      SetObjectData(this, info, context, null);
    }

    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {
      PenX s = this;
      Configured cp = PenX._GetConfiguredPropertiesVariable(s);
      if (s.Cached) cp |= PenX.Configured.InCachedMode;

      info.AddValue("Configured", (int)cp);
      if (0 != (cp & PenX.Configured.PenType))
        info.AddValue("Type", s.PenType);
      if (0 != (cp & PenX.Configured.Alignment))
        info.AddValue("Alignment", s.Alignment);
      if (0 != (cp & PenX.Configured.Brush))
        info.AddValue("Brush", s.BrushHolder);
      if (0 != (cp & PenX.Configured.Color))
        info.AddValue("Color", s.Color);
      if (0 != (cp & PenX.Configured.CompoundArray))
        info.AddValue("CompoundArray", s.CompoundArray);
      if (0 != (cp & PenX.Configured.DashStyle))
        info.AddValue("DashStyle", s.DashStyle);
      if (0 != (cp & PenX.Configured.DashCap))
        info.AddValue("DashCap", s.DashCap);
      if (0 != (cp & PenX.Configured.DashOffset))
        info.AddValue("DashOffset", s.DashOffset);
      if (0 != (cp & PenX.Configured.DashPattern))
        info.AddValue("DashPattern", s.DashPattern);
      if (0 != (cp & PenX.Configured.EndCap))
      {
        info.AddValue("EndCap", s.m_EndCap.Name);
        info.AddValue("EndCapSize", s.m_EndCap.Size);
      }
      if (0 != (cp & PenX.Configured.LineJoin))
        info.AddValue("LineJoin", s.LineJoin);
      if (0 != (cp & PenX.Configured.MiterLimit))
        info.AddValue("MiterLimit", s.MiterLimit);
      if (0 != (cp & PenX.Configured.StartCap))
      {
        info.AddValue("StartCap", s.m_StartCap.Name);
        info.AddValue("StartCapSize", s.m_StartCap.Size);
      }
      if (0 != (cp & PenX.Configured.Transform))
        info.AddValue("Transform", s.Transform.Elements);
      if (0 != (cp & PenX.Configured.Width))
        info.AddValue("Width", s.Width);
    }
    public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
    {
      PenX s = (PenX)obj;
      s.m_ConfiguredProperties = (Configured)info.GetInt32("Configured");
      Configured cp = s.m_ConfiguredProperties;

     
      if (0 != (cp & PenX.Configured.PenType))
        s.m_PenType = (PenType)info.GetValue("Type", typeof(PenType));
      else
        s.m_PenType = PenType.SolidColor;

      if (0 != (cp & PenX.Configured.Alignment))
        s.m_Alignment = (PenAlignment)info.GetValue("Alignment", typeof(PenAlignment));
      else
        s.m_Alignment = PenAlignment.Center;

      if (0 != (cp & PenX.Configured.Brush))
        s.m_Brush = (BrushX)info.GetValue("Brush", typeof(BrushX));
      else
        s.m_Brush = new BrushX(Color.Black);

      if (0 != (cp & PenX.Configured.Color))
        s.m_Color = (Color)info.GetValue("Color", typeof(Color));
      else
        s.m_Color = Color.Black;

      if (0 != (cp & PenX.Configured.CompoundArray))
        s.m_CompoundArray = (float[])info.GetValue("CompoundArray", typeof(float[]));
      else
        s.m_CompoundArray = new float[0];

      if (0 != (cp & PenX.Configured.DashStyle))
        s.m_DashStyle = (DashStyle)info.GetValue("DashStyle", typeof(DashStyle));
      else
        s.m_DashStyle = DashStyle.Solid;

      if (0 != (cp & PenX.Configured.DashCap))
        s.m_DashCap = (DashCap)info.GetValue("DashCap", typeof(DashCap));
      else
        s.m_DashCap = DashCap.Flat;

      if (0 != (cp & PenX.Configured.DashOffset))
        s.m_DashOffset = (float)info.GetSingle("DashOffset");
      else
        s.m_DashOffset = 0;

      if (0 != (cp & PenX.Configured.DashPattern))
        s.m_DashPattern = (float[])info.GetValue("DashPattern", typeof(float[]));
      else
        s.m_DashPattern = null;

      if (0 != (cp & PenX.Configured.EndCap))
      {
        string name = info.GetString("EndCap");
        float size = info.GetSingle("EndCapSize");
        s.m_EndCap = new LineCapEx(name, size);
      }
      else
        s.m_EndCap = LineCapEx.Flat;

      if (0 != (cp & PenX.Configured.LineJoin))
        s.m_LineJoin = (LineJoin)info.GetValue("LineJoin", typeof(LineJoin));
      else
        s.m_LineJoin = LineJoin.Miter;

      if (0 != (cp & PenX.Configured.MiterLimit))
        s.m_MiterLimit = info.GetSingle("MiterLimit");
      else
        s.m_MiterLimit = 10;

      if (0 != (cp & PenX.Configured.StartCap))
      {
        string name = info.GetString("StartCap");
        float size = info.GetSingle("StartCapSize");
        s.m_StartCap = new LineCapEx(name, size);
      }
      else
        s.m_StartCap = LineCapEx.Flat;

      if (0 != (cp & PenX.Configured.Transform))
      {
        float[] el = (float[])info.GetValue("Transform", typeof(float[]));
        s.m_Transform = new Matrix(el[0], el[1], el[2], el[3], el[4], el[5]);
      }
      else
        s.m_Transform = new Matrix();

      if (0 != (cp & PenX.Configured.Width))
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

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.PenHolder", 0)]
    class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        throw new ApplicationException("This point should not be reached");
        /*
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
         */
      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {

        PenX s = null != o ? (PenX)o : new PenX();

        s.m_ConfiguredProperties = (Configured)info.GetInt32("Configured");
        Configured cp = s.m_ConfiguredProperties;

       

        if (0 != (cp & PenX.Configured.PenType))
          s.m_PenType = (PenType)info.GetEnum("Type", typeof(PenType));
        else
          s.m_PenType = PenType.SolidColor;

        if (0 != (cp & PenX.Configured.Alignment))
          s.m_Alignment = (PenAlignment)info.GetEnum("Alignment", typeof(PenAlignment));
        else
          s.m_Alignment = PenAlignment.Center;

        if (0 != (cp & PenX.Configured.Brush))
          s.m_Brush = (BrushX)info.GetValue("Brush", typeof(BrushX));
        else
          s.m_Brush = new BrushX(Color.Black);

        if (0 != (cp & PenX.Configured.Color))
          s.m_Color = (Color)info.GetValue("Color", typeof(Color));
        else
          s.m_Color = Color.Black;

        if (0 != (cp & PenX.Configured.CompoundArray))
          info.GetArray(out s.m_CompoundArray);
        else
          s.m_CompoundArray = new float[0];

        if (0 != (cp & PenX.Configured.DashStyle))
          s.m_DashStyle = (DashStyle)info.GetEnum("DashStyle", typeof(DashStyle));
        else
          s.m_DashStyle = DashStyle.Solid;

        if (0 != (cp & PenX.Configured.DashCap))
          s.m_DashCap = (DashCap)info.GetEnum("DashCap", typeof(DashCap));
        else
          s.m_DashCap = DashCap.Flat;

        if (0 != (cp & PenX.Configured.DashOffset))
          s.m_DashOffset = (float)info.GetSingle("DashOffset");
        else
          s.m_DashOffset = 0;

        if (0 != (cp & PenX.Configured.DashPattern))
          info.GetArray(out s.m_DashPattern);
        else
          s.m_DashPattern = null;

        if (0 != (cp & PenX.Configured.EndCap))
          s.m_EndCap = new LineCapEx((LineCap)info.GetEnum("EndCap", typeof(LineCap)));
        else
          s.m_EndCap = LineCapEx.Flat;

        if (0 != (cp & PenX.Configured.LineJoin))
          s.m_LineJoin = (LineJoin)info.GetEnum("LineJoin", typeof(LineJoin));
        else
          s.m_LineJoin = LineJoin.Miter;

        if (0 != (cp & PenX.Configured.MiterLimit))
          s.m_MiterLimit = info.GetSingle("MiterLimit");
        else
          s.m_MiterLimit = 10;

        if (0 != (cp & PenX.Configured.StartCap))
          s.m_StartCap = new LineCapEx((LineCap)info.GetEnum("StartCap", typeof(LineCap)));
        else
          s.m_StartCap = LineCapEx.Flat;

        if (0 != (cp & PenX.Configured.Transform))
        {
          float[] el;
          info.GetArray(out el);
          s.m_Transform = new Matrix(el[0], el[1], el[2], el[3], el[4], el[5]);
        }
        else
          s.m_Transform = new Matrix();

        if (0 != (cp & PenX.Configured.Width))
          s.m_Width = info.GetSingle("Width");
        else
          s.m_Width = 1;

        return s;
      }
    }


    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase","Altaxo.Graph.PenHolder", 1)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(PenX), 2)]
    class XmlSerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        PenX s = (PenX)obj;
        Configured cp = PenX._GetConfiguredPropertiesVariable(s);
        if (s.Cached) cp |= PenX.Configured.InCachedMode;

        info.AddValue("Configured", (int)cp);
        if (0 != (cp & PenX.Configured.PenType))
          info.AddEnum("Type", s.PenType);
        if (0 != (cp & PenX.Configured.Alignment))
          info.AddEnum("Alignment", s.Alignment);
        if (0 != (cp & PenX.Configured.Brush))
          info.AddValue("Brush", s.BrushHolder);
        if (0 != (cp & PenX.Configured.Color))
          info.AddValue("Color", s.Color);
        if (0 != (cp & PenX.Configured.CompoundArray))
          info.AddArray("CompoundArray", s.CompoundArray, s.CompoundArray.Length);
        if (0 != (cp & PenX.Configured.DashStyle))
          info.AddEnum("DashStyle", s.DashStyle);
        if (0 != (cp & PenX.Configured.DashCap))
          info.AddEnum("DashCap", s.DashCap);
        if (0 != (cp & PenX.Configured.DashOffset))
          info.AddValue("DashOffset", s.DashOffset);
        if (0 != (cp & PenX.Configured.DashPattern))
          info.AddArray("DashPattern", s.DashPattern, s.DashPattern.Length);
        if (0 != (cp & PenX.Configured.EndCap))
        {
          info.AddValue("EndCap", s.EndCap.Name);
          info.AddValue("EndCapSize", s.m_EndCap.Size);
        }
        if (0 != (cp & PenX.Configured.LineJoin))
          info.AddEnum("LineJoin", s.LineJoin);
        if (0 != (cp & PenX.Configured.MiterLimit))
          info.AddValue("MiterLimit", s.MiterLimit);
        if (0 != (cp & PenX.Configured.StartCap))
        {
          info.AddValue("StartCap", s.StartCap.Name);
          info.AddValue("StartCapSize", s.m_StartCap.Size);
        }
        if (0 != (cp & PenX.Configured.Transform))
          info.AddArray("Transform", s.Transform.Elements, s.Transform.Elements.Length);
        if (0 != (cp & PenX.Configured.Width))
          info.AddValue("Width", s.Width);
      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {

        PenX s = null != o ? (PenX)o : new PenX();

        s.m_ConfiguredProperties = (Configured)info.GetInt32("Configured");
        Configured cp = s.m_ConfiguredProperties;

        

        if (0 != (cp & PenX.Configured.PenType))
          s.m_PenType = (PenType)info.GetEnum("Type", typeof(PenType));
        else
          s.m_PenType = PenType.SolidColor;

        if (0 != (cp & PenX.Configured.Alignment))
          s.m_Alignment = (PenAlignment)info.GetEnum("Alignment", typeof(PenAlignment));
        else
          s.m_Alignment = PenAlignment.Center;

        if (0 != (cp & PenX.Configured.Brush))
          s.m_Brush = (BrushX)info.GetValue("Brush", typeof(BrushX));
        else
          s.m_Brush = new BrushX(Color.Black);

        if (0 != (cp & PenX.Configured.Color))
          s.m_Color = (Color)info.GetValue("Color", typeof(Color));
        else
          s.m_Color = Color.Black;

        if (0 != (cp & PenX.Configured.CompoundArray))
          info.GetArray(out s.m_CompoundArray);
        else
          s.m_CompoundArray = new float[0];

        if (0 != (cp & PenX.Configured.DashStyle))
          s.m_DashStyle = (DashStyle)info.GetEnum("DashStyle", typeof(DashStyle));
        else
          s.m_DashStyle = DashStyle.Solid;

        if (0 != (cp & PenX.Configured.DashCap))
          s.m_DashCap = (DashCap)info.GetEnum("DashCap", typeof(DashCap));
        else
          s.m_DashCap = DashCap.Flat;

        if (0 != (cp & PenX.Configured.DashOffset))
          s.m_DashOffset = (float)info.GetSingle("DashOffset");
        else
          s.m_DashOffset = 0;

        if (0 != (cp & PenX.Configured.DashPattern))
          info.GetArray(out s.m_DashPattern);
        else
          s.m_DashPattern = null;

        if (0 != (cp & PenX.Configured.EndCap))
        {
          string name = info.GetString("EndCap");
          float size = info.GetSingle("EndCapSize");
          s.m_EndCap = new LineCapEx(name, size);
        }
        else
          s.m_EndCap = LineCapEx.Flat;

        if (0 != (cp & PenX.Configured.LineJoin))
          s.m_LineJoin = (LineJoin)info.GetEnum("LineJoin", typeof(LineJoin));
        else
          s.m_LineJoin = LineJoin.Miter;

        if (0 != (cp & PenX.Configured.MiterLimit))
          s.m_MiterLimit = info.GetSingle("MiterLimit");
        else
          s.m_MiterLimit = 10;

        if (0 != (cp & PenX.Configured.StartCap))
        {
          string name = info.GetString("StartCap");
          float size = info.GetSingle("StartCapSize");
          s.m_StartCap = new LineCapEx(name, size);
        }
        else
          s.m_StartCap = LineCapEx.Flat;

        if (0 != (cp & PenX.Configured.Transform))
        {
          float[] el;
          info.GetArray(out el);
          s.m_Transform = new Matrix(el[0], el[1], el[2], el[3], el[4], el[5]);
        }
        else
          s.m_Transform = new Matrix();

        if (0 != (cp & PenX.Configured.Width))
          s.m_Width = info.GetSingle("Width");
        else
          s.m_Width = 1;

        return s;
      }
    }




    #endregion

    public PenX()
    {
    }

    public PenX(Color c)
      : this(c, 1, false)
    {
    }

    public PenX(Color c, float width)
      : this(c, width, false)
    {
    }

    public PenX(Color c, float width, bool bCachedMode)
    {
     
      this.m_PenType = PenType.SolidColor;
      this.m_Color = c;
      this.m_Width = width;

      _SetProp(PenX.Configured.IsNotNull, true);
      _SetProp(Configured.Color, Color.Black.ToArgb() != c.ToArgb());
      _SetProp(Configured.Width, 1 != width);

      if (bCachedMode)
        _SetPenVariable(new Pen(c, width));
    }


    /// <summary>
    /// Copy constructor of PenHolder
    /// </summary>
    /// <param name="pen">the PenHolder object to copy</param>
    public PenX(PenX pen)
    {
      CopyFrom(pen);
    }

    /// <summary>
    /// Copies the properties of another instance to this instance.
    /// </summary>
    /// <param name="pen">the PenHolder object to copy</param>
    public void CopyFrom(PenX pen)
    {
      _SetPenVariable(null);

      this.m_ConfiguredProperties = pen.m_ConfiguredProperties;
      this.m_PenType = pen.PenType;
      this.m_Alignment = pen.Alignment;

      if (0 != (this.m_ConfiguredProperties & Configured.Brush))
        this.m_Brush = new BrushX(pen.m_Brush);

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

    /*
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
    */

    public static implicit operator System.Drawing.Pen(PenX ph)
    {
      ph.Cached = true; // if implicit conversion, we maybe are not aware that the pen is never destroyed, so _we_ control the pen by caching it
      return ph.Pen;
    }


    public Pen Pen
    {
      get
      {
        if (m_Pen==null)
          m_Pen = _GetPenFromProperties();

        return m_Pen;
      }
   
    }

    public bool Cached
    {
      get { return false; }
      set
      {
        
      }
    }

    protected void _SetPenVariable(Pen pen)
    {
      if (null != m_Pen)
        m_Pen.Dispose();
      m_Pen = pen;
    }
    /*
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
    */

    protected Pen _GetPenFromProperties()
    {
      Configured cp = this.m_ConfiguredProperties;

      if (0 == (cp & PenX.Configured.IsNotNull))
        return null;

      Pen pen = new Pen(Color.Black);

      // now set the optional Pen properties
      if (0 != (cp & PenX.Configured.Width))
        pen.Width = this.m_Width;

      if (0 != (cp & PenX.Configured.Alignment))
        pen.Alignment = m_Alignment;

      if (0 != (cp & PenX.Configured.Color))
        pen.Color = this.m_Color;
      if (0 != (cp & PenX.Configured.Brush))
        pen.Brush = this.m_Brush;
    
      if (0 != (cp & PenX.Configured.CompoundArray))
        pen.CompoundArray = this.m_CompoundArray;
      if (0 != (cp & PenX.Configured.DashStyle))
        pen.DashStyle = this.m_DashStyle;
      if (0 != (cp & PenX.Configured.DashCap))
        pen.DashCap = this.m_DashCap;
      if (0 != (cp & PenX.Configured.DashOffset))
        pen.DashOffset = this.m_DashOffset;
      if (0 != (cp & PenX.Configured.DashPattern))
        pen.DashPattern = this.m_DashPattern;
      if (0 != (cp & PenX.Configured.EndCap))
        this.m_EndCap.SetPenEndCap(pen);
      if (0 != (cp & PenX.Configured.LineJoin))
        pen.LineJoin = this.m_LineJoin;
      if (0 != (cp & PenX.Configured.MiterLimit))
        pen.MiterLimit = this.m_MiterLimit;
      if (0 != (cp & PenX.Configured.StartCap))
        this.m_StartCap.SetPenStartCap(pen);
      if (0 != (cp & PenX.Configured.Transform))
        pen.Transform = this.m_Transform;
   


      return pen;
    }

    static Configured SetProp(Configured allprop, Configured prop, bool bSet)
    {
      allprop &= (Configured.All ^ prop);
      if (bSet)
        allprop |= prop;

      return allprop;
    }
    static bool AreEqual(float[] x1, float[] x2)
    {
      if (x1 == null && x2 == null)
        return true;
      if (x1 == null || x2 == null)
        return false;
      if (x1.Length != x2.Length)
        return false;
      for (int i = 0; i < x1.Length; i++)
        if (x1[i] != x2[i])
          return false;

      return true;
    }


    /// <summary>
    /// Returns all differences between two pens as a flagged enum.
    /// </summary>
    /// <param name="p1">First pen to compare.</param>
    /// <param name="p2">Second pen to comare.</param>
    /// <returns>A enum where all those bits are set where the two pens are different.</returns>
    public static Configured GetDifferences(PenX p1, PenX p2)
    {
      Configured cp1 = p1.m_ConfiguredProperties;
      Configured cp2 = p2.m_ConfiguredProperties;

      Configured cp = cp1 & cp2;

      // for all properties that are configured both in p1 and p2, test if they are equal
      // now set the optional Pen properties
      if(0!=(cp & PenX.Configured.IsNotNull))
        cp = SetProp(cp,Configured.IsNotNull,false);

      if (0 != (cp & PenX.Configured.Width))
        cp = SetProp(cp, PenX.Configured.Width, p1.m_Width != p2.m_Width);

      if (0 != (cp & PenX.Configured.Alignment))
        cp = SetProp(cp, PenX.Configured.Alignment, p1.m_Alignment != p2.m_Alignment);

      if (0 != (cp & PenX.Configured.Color))
        cp = SetProp(cp, PenX.Configured.Color, p1.m_Color != p2.m_Color);

      if (0 != (cp & PenX.Configured.Brush))
        cp = SetProp(cp, PenX.Configured.Brush, !BrushX.AreEqual(p1.m_Brush, p2.m_Brush));

      if (0 != (cp & PenX.Configured.CompoundArray))
        cp = SetProp(cp, PenX.Configured.CompoundArray, !AreEqual(p1.m_CompoundArray, p2.m_CompoundArray));

      if (0 != (cp & PenX.Configured.DashStyle))
        cp = SetProp(cp, PenX.Configured.DashStyle, p1.m_DashStyle != p2.m_DashStyle);
      
      if (0 != (cp & PenX.Configured.DashCap))
        cp = SetProp(cp, PenX.Configured.DashCap, p1.m_DashCap != p2.m_DashCap);
      
      if (0 != (cp & PenX.Configured.DashOffset))
        cp = SetProp(cp, PenX.Configured.DashOffset, p1.m_DashOffset != p2.m_DashOffset);
      
      if (0 != (cp & PenX.Configured.DashPattern))
        cp = SetProp(cp, PenX.Configured.DashPattern, !AreEqual(p1.m_DashPattern, p2.m_DashPattern));
      
      if (0 != (cp & PenX.Configured.EndCap))
        cp = SetProp(cp, PenX.Configured.EndCap, p1.m_EndCap != p2.m_EndCap);
      
      if (0 != (cp & PenX.Configured.LineJoin))
        cp = SetProp(cp, PenX.Configured.LineJoin, p1.m_LineJoin != p2.m_LineJoin); 
      
      if (0 != (cp & PenX.Configured.MiterLimit))
        cp = SetProp(cp, PenX.Configured.MiterLimit, p1.m_MiterLimit != p2.m_MiterLimit);
      
      if (0 != (cp & PenX.Configured.StartCap))
        cp = SetProp(cp, PenX.Configured.StartCap, p1.m_StartCap != p2.m_StartCap);
      
      if (0 != (cp & PenX.Configured.Transform))
        cp = SetProp(cp, PenX.Configured.Transform, p1.m_Transform != p2.m_Transform);

      return cp | (cp1 ^ cp2);
    }

    public static bool AreEqual(PenX p1, PenX p2)
    {
      if (p1 == null && p2 == null)
        return true;
      if (p1 == null || p2 == null)
        return false;
      if (object.ReferenceEquals(p1, p2))
        return true;

      if (p1.m_ConfiguredProperties != p2.m_ConfiguredProperties)
        return false;

      Configured diff = GetDifferences(p1, p2);
      return diff == 0;
    }

    public static bool AreEqualUnlessWidth(PenX p1, PenX p2)
    {
      if (p1 == null && p2 == null)
        return true;
      if (p1 == null || p2 == null)
        return false;
      if (object.ReferenceEquals(p1, p2))
        return true;

      Configured c1 = p1.m_ConfiguredProperties;
      Configured c2 = p2.m_ConfiguredProperties;
      c1 = SetProp(c1, Configured.Width, false);
      c2 = SetProp(c2, Configured.Width, false);

      if (c1 != c2)
        return false;

      Configured diff = GetDifferences(p1, p2);
      diff = SetProp(diff, Configured.Width, false);
      return diff == 0;
    }


    private void _SetBrushVariable(BrushX bh)
    {
      if (null != m_Brush)
        m_Brush.Dispose();

      m_Brush = bh;
    }


    public PenType PenType
    {
      get { return m_PenType; }
    }

    public PenAlignment Alignment
    {
      get { return  m_Alignment; }
      set
      {
        bool bChanged = (m_Alignment != value);
        m_Alignment = value;
        if (bChanged)
        {
          _SetProp(Configured.Alignment, PenAlignment.Center != value);

          _SetPenVariable(null);

          OnChanged(); // Fire the Changed event
        }
      }
    }


    public BrushX BrushHolder
    {
      get
      {
        if (m_Brush == null)
          return new BrushX(this.m_Color);
        else
           return m_Brush;
      }
      set
      {
        if (null == value)
        {
          _SetProp(Configured.PenType, false);
          _SetProp(Configured.Color, Color.Black != m_Color);
          m_PenType = PenType.SolidColor;
          _SetBrushVariable(null);
        }
        else if (value.BrushType==BrushType.SolidBrush)
        {
          m_PenType = PenType.SolidColor;
          m_Color = value.Color;
          _SetBrushVariable(null);

          _SetProp(Configured.PenType, PenType.SolidColor != m_PenType);
          _SetProp(Configured.Color, Color.Black != m_Color);
          _SetProp(Configured.Brush, false);
        } // if value is SolidBrush
        else if (value.BrushType == BrushType.HatchBrush)
        {
          m_PenType = PenType.HatchFill;
          _SetBrushVariable(new BrushX(value));

          _SetProp(Configured.PenType, true);
          _SetProp(Configured.Color, false);
          _SetProp(Configured.Brush, true);
        }
        else if (value.BrushType == BrushType.TextureBrush)
        {
          m_PenType = PenType.TextureFill;
          _SetBrushVariable(new BrushX(value));

          _SetProp(Configured.PenType, true);
          _SetProp(Configured.Color, false);
          _SetProp(Configured.Brush, true);
        }
        else if (value.BrushType == BrushType.LinearGradientBrush)
        {
          m_PenType = PenType.LinearGradient;
          _SetBrushVariable(new BrushX(value));

          _SetProp(Configured.PenType, true);
          _SetProp(Configured.Color, false);
          _SetProp(Configured.Brush, true);
        }
        else if (value.BrushType == BrushType.PathGradientBrush)
        {
          m_PenType = PenType.PathGradient;
          _SetBrushVariable(new BrushX(value));

          _SetProp(Configured.PenType, true);
          _SetProp(Configured.Color, false);
          _SetProp(Configured.Brush, true);
        }
        _SetPenVariable(null);
        OnChanged(); // Fire the Changed event
      }
    }

    public Color Color
    {
      get { return m_Color; }
      set
      {
        bool bChanged = (m_Color != value);
        m_Color = value;
        if (bChanged)
        {
          _SetProp(Configured.PenType, false);
          _SetProp(Configured.Color, Color.Black != value);
          _SetProp(Configured.Brush, false);

          m_PenType = PenType.SolidColor;
          m_Brush = null;

          _SetBrushVariable(null);
          _SetPenVariable(null);

          OnChanged(); // Fire the Changed event
        }
      }
    }
    public float[] CompoundArray
    {
      get { return  m_CompoundArray; }
      set
      {
        _SetProp(Configured.CompoundArray, null != value && value.Length > 0);
        m_CompoundArray = (float[])value.Clone();
        _SetPenVariable(null);
        OnChanged(); // Fire the Changed event
      }
    }
    public DashCap DashCap
    {
      get { return  m_DashCap; }
      set
      {
        bool bChanged = (m_DashCap != value);
        m_DashCap = value;
        if (bChanged)
        {
          _SetProp(Configured.DashCap, DashCap.Flat != value);
          _SetPenVariable(null);
          OnChanged(); // Fire the Changed event
        }
      }
    }
    public float DashOffset
    {
      get { return  m_DashOffset; }
      set
      {
        bool bChanged = (m_DashOffset != value);
        m_DashOffset = value;
        if (bChanged)
        {
          _SetProp(Configured.DashOffset, 0 != value);
          _SetPenVariable(null);
          OnChanged(); // Fire the Changed event
        }
      }
    }
    public float[] DashPattern
    {
      get { return  m_DashPattern; }
      set
      {
        _SetProp(Configured.DashPattern, null != value && value.Length > 0);
        m_DashPattern = value;
        _SetPenVariable(null);
        OnChanged(); // Fire the Changed event
      }
    }
    public DashStyle DashStyle
    {
      get { return m_DashStyle; }
      set
      {
        bool bChanged = (m_DashStyle != value);
        m_DashStyle = value;
        if (bChanged)
        {
          _SetProp(Configured.DashStyle, DashStyle.Solid != value);
          _SetPenVariable(null);
          OnChanged(); // Fire the Changed event
        }
      }
    }

    public DashStyleEx DashStyleEx
    {
      get
      {
        if (m_DashStyle != DashStyle.Custom)
          return new DashStyleEx(m_DashStyle);
        else
          return new DashStyleEx(m_DashPattern);
      }
      set
      {
        DashStyle = value.KnownStyle;
        DashPattern = value.CustomStyle;
      }
    }

    public LineCapEx EndCap
    {
       get
      {
        return this.m_EndCap;
      }
      set
      {
        bool bChanged = (m_EndCap != value);
        m_EndCap = value;

        if (bChanged)
        {
          _SetProp(Configured.EndCap, !m_EndCap.IsDefaultStyle);
          _SetPenVariable(null);
          OnChanged(); // Fire the Changed event
        }
      }
    }
    public LineJoin LineJoin
    {
      get { return  m_LineJoin; }
      set
      {
        bool bChanged = (m_LineJoin != value);
        m_LineJoin = value;
        if (bChanged)
        {
          _SetProp(Configured.LineJoin, LineJoin.Miter != value);
          _SetPenVariable(null);
          OnChanged(); // Fire the Changed event
        }
      }
    }
    public float MiterLimit
    {
      get { return  m_MiterLimit; }
      set
      {
        bool bChanged = (m_MiterLimit != value);
        m_MiterLimit = value;
        if (bChanged)
        {
          _SetProp(Configured.MiterLimit, 10 != value);
          _SetPenVariable(null);
          OnChanged(); // Fire the Changed event
        }
      }
    }
    public LineCapEx StartCap
    {
      get
      {
        
          return this.m_StartCap;
      }
      set
      {
        bool bChanged = (m_StartCap != value);
        m_StartCap = value;
        if (bChanged)
        {
          _SetProp(Configured.StartCap, !m_StartCap.IsDefaultStyle);
          _SetPenVariable(null);

          OnChanged(); // Fire the Changed event
        }
      }
    }
    public Matrix Transform
    {
      get { return m_Transform; }
      set
      {
        _SetProp(Configured.Transform, null != value && !value.IsIdentity);
        m_Transform = value.Clone();
        _SetPenVariable(null);
        OnChanged(); // Fire the Changed event
      }
    }

    public float Width
    {
      get { return m_Width; }
      set
      {
        bool bChanged = (m_Width != value);
        m_Width = value;
        if (bChanged)
        {
          _SetProp(Configured.Width, 1 != value);
          _SetPenVariable(null);
          OnChanged(); // Fire the Changed event
        }
      }
    }

    void _SetProp(Configured prop, bool bSet)
    {
      this.m_ConfiguredProperties &= (Configured.All ^ prop);
      if (bSet) this.m_ConfiguredProperties |= prop;

    }

    public object Clone()
    {
      return new PenX(this);
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


    public RectangleF BrushRectangle
    {
      get
      {
        return m_Brush == null ? RectangleF.Empty : m_Brush.Rectangle;
      }
      set
      {
        if (m_Brush != null)
        {
          if (value != m_Brush.Rectangle)
          {
            m_Brush.Rectangle = value;
            if (m_Pen != null)
              m_Pen.Brush = m_Brush.Brush;
          }
        }
      }
    }

  } // end of class PenHolder

  #endregion

  #region DashStyleEx

  [Serializable]
  public class DashStyleEx : ICloneable
  {
    DashStyle _knownStyle;
    float[] _customStyle;

    public DashStyleEx(DashStyle style)
    {
      if (style == DashStyle.Custom)
        throw new ArgumentOutOfRangeException("Style must not be a custom style, use the other constructor instead");

      _knownStyle = style;
    }

    public DashStyleEx(float[] customStyle)
    {
      _customStyle = (float[])customStyle.Clone();
      _knownStyle = DashStyle.Custom;
    }

    public DashStyleEx(DashStyleEx from)
    {
      CopyFrom(from);
    }

    public void CopyFrom(DashStyleEx from)
    {
      this._knownStyle = from.KnownStyle;
      this._customStyle = from._customStyle == null ? null : (float[])from._customStyle.Clone();
    }

    public DashStyleEx Clone()
    {
      return new DashStyleEx(this);
    }
    object ICloneable.Clone()
    {
      return new DashStyleEx(this);
    }

    public bool IsKnownStyle
    {
      get
      {
        return _knownStyle != DashStyle.Custom;
      }
    }

    public bool IsCustomStyle
    {
      get
      {
        return _knownStyle == DashStyle.Custom;
      }
    }

    public DashStyle KnownStyle
    {
      get
      {
        return _knownStyle;
      }
    }

    public float[] CustomStyle
    {
      get
      {
        return null == _customStyle ? null : (float[])_customStyle.Clone();
      }
    }


    public void SetPenDash(Pen pen)
    {
      pen.DashStyle = _knownStyle;
      if (IsCustomStyle)
        pen.DashPattern = (float[])this._customStyle.Clone();
    }

    public override bool Equals(object obj)
    {
      if (obj is DashStyleEx)
      {
        DashStyleEx from = (DashStyleEx)obj;

        if (this.IsKnownStyle && this._knownStyle == from._knownStyle)
          return true;
        else if (this.IsCustomStyle && this._customStyle == from._customStyle)
          return true;
      }

      return false;
    }
    public override int GetHashCode()
    {
      if (IsCustomStyle && _customStyle!=null)
        return _customStyle.GetHashCode();
      else
        return _knownStyle.GetHashCode();
    }

    public override string ToString()
    {
      if (_knownStyle != DashStyle.Custom)
        return _knownStyle.ToString();
      else
      {
        System.Text.StringBuilder stb = new System.Text.StringBuilder();
        foreach (float f in _customStyle)
        {
          stb.Append(Altaxo.Serialization.GUIConversion.ToString(f));
          stb.Append(";");
        }
        return stb.ToString(0, stb.Length - 1);
      }
    }


    public static DashStyleEx Solid
    {
      get { return new DashStyleEx(DashStyle.Solid); }
    }
    public static DashStyleEx Dot
    {
      get { return new DashStyleEx(DashStyle.Dot); }
    }

    public static DashStyleEx Dash
    {
      get { return new DashStyleEx(DashStyle.Dash); }
    }

    public static DashStyleEx DashDot
    {
      get { return new DashStyleEx(DashStyle.DashDot); }
    }
    public static DashStyleEx DashDotDot
    {
      get { return new DashStyleEx(DashStyle.DashDotDot); }
    }



  }
  #endregion

  #region LineCapEx

  [Serializable]
  public abstract class LineCapExtension
    {
    public abstract string Name { get; }
    public abstract float DefaultSize { get; }
    public abstract void SetStartCap(Pen pen, float size);
    public abstract void SetEndCap(Pen pen, float size);
    }

  [Serializable]
  public struct LineCapEx : System.Runtime.Serialization.ISerializable
  {
    #region Inner classes
    [Serializable]
    class KnownLineCapWrapper : LineCapExtension
    {
      LineCap _cap;
      string _name;
      public KnownLineCapWrapper(LineCap cap)
      {
        _cap = cap;
        _name = Enum.GetName(typeof(LineCap), _cap);
      }

      #region ILineCapExtension Members

      public override string Name
      {
        get { return _name; }
      }

      public override float DefaultSize { get { return 0; } }
      #endregion

      #region ILineCapExtension Members


      public override void SetStartCap(Pen pen, float size)
      {
        pen.StartCap = _cap;
      }

      public override void SetEndCap(Pen pen, float size)
      {
        pen.EndCap = _cap;
      }

      #endregion
    }
    [Serializable]
    class DefaultLineCapWrapper : LineCapExtension
    {
      public DefaultLineCapWrapper()
      {
      }
      public override string Name
      {
        get
        {
          return "Flat";
        }
      }
      public override float DefaultSize { get { return 0; } }

      public override void SetStartCap(Pen pen, float size)
      {
        pen.StartCap = LineCap.Flat;
      }
      public override void SetEndCap(Pen pen, float size)
      {
        pen.StartCap = LineCap.Flat;
      }
    }
    #endregion


    LineCapExtension _currentStyle;
    float _size;

    static DefaultLineCapWrapper _defaultStyle;
    static System.Collections.Generic.SortedDictionary<string, LineCapExtension> _registeredStyles;

    #region Clipboard Serialization
      public LineCapEx(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
    {
      _currentStyle = null;
      _size = 0;
      SetObjectData(this, info, context, null);
    }

    public void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
    {
     LineCapEx s = this;
     info.AddValue("Name", s.Name);
     info.AddValue("Size", s.Size);

    }
    public object SetObjectData(object obj, System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context, System.Runtime.Serialization.ISurrogateSelector selector)
    {
     string name =  info.GetString("Name");
      float size = info.GetSingle("Size");
      this.CopyFrom(new LineCapEx(name,size));
      return this;
    } // end of SetObjectData
    #endregion

    static LineCapEx()
    {
      _defaultStyle = new DefaultLineCapWrapper();
      _registeredStyles = new System.Collections.Generic.SortedDictionary<string, LineCapExtension>();


      // first register the predefined styles
      foreach (LineCap cap in Enum.GetValues(typeof(LineCap)))
      {
        if (cap == LineCap.Custom)
        {
          continue;
        }
        if(cap==LineCap.Flat)
        {
          _registeredStyles.Add(_defaultStyle.Name,_defaultStyle);
        }
        else
        {
          LineCapExtension ex = new KnownLineCapWrapper(cap);
          _registeredStyles.Add(ex.Name,ex);
        }
      }

      // now the other linecaps
      LineCapExtension more;
      more = new LineCaps.ArrowF10LineCap();
      _registeredStyles.Add(more.Name, more);
      more = new LineCaps.ArrowF20LineCap();
      _registeredStyles.Add(more.Name, more);
      more = new LineCaps.LeftBarLineCap();
      _registeredStyles.Add(more.Name, more);
      more = new LineCaps.RightBarLineCap();
      _registeredStyles.Add(more.Name, more);
      more = new LineCaps.SymBarLineCap();
      _registeredStyles.Add(more.Name, more);
    }

    public LineCapEx(LineCap style)
    {
      if (style == LineCap.Custom)
        throw new ArgumentOutOfRangeException("Style must not be a custom style, use the other constructor instead");
     
      _currentStyle = _registeredStyles[Enum.GetName(typeof(LineCap), style)];
      _size = _currentStyle.DefaultSize;
    }

    public LineCapEx(string name)
    {
     
      _currentStyle = _registeredStyles[name];
      if (_currentStyle == null)
        throw new ArgumentException(string.Format("Unknown LineCapEx style: {0}", name), "name");

      _size = _currentStyle.DefaultSize;
    }

    public LineCapEx(string name, float size)
    {
      _currentStyle = _registeredStyles[name];
      if (_currentStyle == null)
        throw new ArgumentException(string.Format("Unknown LineCapEx style: {0}", name), "name");
      
      _size = size;
    }

    public LineCapEx(LineCapEx from)
    {
      _size = 0;
      _currentStyle = null;
      CopyFrom(from);
    }

    public void CopyFrom(LineCapEx from)
    {
      this._size = from._size;
      this._currentStyle = from._currentStyle;
    }

    public LineCapEx(LineCapEx from, float size)
    {
      _size = 0;
      _currentStyle = null;
      CopyFrom(from);
      _size = size;
    }

    private LineCapEx(LineCapExtension ex)
    {
      _currentStyle = ex;
      _size = _currentStyle.DefaultSize;
    }

   

    public bool IsKnownStyle
    {
      get
      {
        return (_currentStyle==null) || (_currentStyle is KnownLineCapWrapper);
      }
    }

    public bool IsCustomStyle
    {
      get
      {
        return !IsKnownStyle;
      }
    }

    public bool IsDefaultStyle
    {
      get { return _currentStyle == null; }
    }

   

    public float Size
    {
      get {
        return _size;
      }
      set
      {
        _size = value;
      }
    }
    


    public void SetPenStartCap(Pen pen)
    {
      if (_currentStyle != null)
        _currentStyle.SetStartCap(pen, _size);
      else
        _defaultStyle.SetStartCap(pen, _size);
    }

    public void SetPenEndCap(Pen pen)
    {
      if (_currentStyle != null)
        _currentStyle.SetEndCap(pen, _size);
      else
        _defaultStyle.SetEndCap(pen, _size);
    }

    public override bool Equals(object obj)
    {
      if (obj is LineCapEx)
      {
        LineCapEx from = (LineCapEx)obj;
        return (this._size == from._size) && (this._currentStyle == from._currentStyle);
      }
      return false;
    }

    public static bool operator ==(LineCapEx a, LineCapEx b)
    {
      return (a._size == b._size) && (a._currentStyle == b._currentStyle);
    }
    public static bool operator !=(LineCapEx a, LineCapEx b)
    {
      return !(a == b);
    }

    public override int GetHashCode()
    {
      if (_currentStyle != null)
        return _size.GetHashCode() + _currentStyle.GetHashCode();
      else
        return _size.GetHashCode();
    }

    public override string ToString()
    {
      return Name;
    }
    public string Name
    {
      get
      {
        if (_currentStyle != null)
          return _currentStyle.Name;
        else
          return _defaultStyle.Name;
      }
    }


    public static LineCapEx[] GetValues()
    {
      LineCapEx[] arr = new LineCapEx[_registeredStyles.Count];
      int i = 0;
      foreach (LineCapExtension ex in _registeredStyles.Values)
        arr[i++] = new LineCapEx(ex);
      return arr;
    }

    public static LineCapEx FromName(string name)
    {
      return new LineCapEx(name);
    }
    public static LineCapEx AnchorMask
    {
      get { return new LineCapEx("AnchorMask"); }
    }
    public static LineCapEx ArrowAnchor
    {
      get { return new LineCapEx("ArrowAnchor"); }
    }
    public static LineCapEx DiamondAnchor
    {
      get { return new LineCapEx("DiamondAnchor"); }
    }
    public static LineCapEx Flat
    {
      get { return new LineCapEx("Flat"); }
    }
    public static LineCapEx NoAnchor
    {
      get { return new LineCapEx("NoAnchor"); }
    }
    public static LineCapEx Round
    {
      get { return new LineCapEx("Round"); }
    }
    public static LineCapEx RoundAnchor
    {
      get { return new LineCapEx("RoundAnchor"); }
    }
    public static LineCapEx Square
    {
      get { return new LineCapEx("Square"); }
    }
    public static LineCapEx SquareAnchor
    {
      get { return new LineCapEx("SquareAnchor"); }
    }
    public static LineCapEx Triangle
    {
      get { return new LineCapEx("Triangle"); }
    }

    
  

    
  }
  #endregion


 
 
}
