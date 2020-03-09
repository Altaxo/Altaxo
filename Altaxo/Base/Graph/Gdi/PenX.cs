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
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.Serialization;
using Altaxo.Drawing;
using Altaxo.Drawing.DashPatternManagement;
using Altaxo.Drawing.DashPatterns;
using Altaxo.Geometry;
using Altaxo.Graph.Gdi.LineCaps;
using Altaxo.Main;

namespace Altaxo.Graph.Gdi
{
  /// <summary>
  /// PenX is a graphics framework independent pen object. Use <see cref="PenCacheGdi.Instance"/> to convert it into a Gdi pen.
  /// </summary>
  [Serializable]
  public class PenX : IImmutable, IEquatable<PenX>
  {
    protected Configured _configuredProperties; // ORed collection of the configured properties (i.e. non-standard properties)
    protected PenType _penType; // the type of the pen    

    /// <summary>The color of the pen (only in effect if <see cref="_brush"/> is null.</summary>
    protected NamedColor _color; // Color of this Pen object

    /// <summary>The brush of the pen (has always precendence over <see cref="_color"/>.</summary>
    protected BrushX _brush; // the brush of this pen

    /// <summary>The width of the pen.</summary>
    protected double _width; // Width of this Pen object

    /// <summary>The alignment of the pen.</summary>
    protected PenAlignment _alignment;
    protected LineJoin _lineJoin;
    protected double _miterLimit;
    protected LineCapExtension _startCap;
    protected LineCapExtension _endCap;
    protected IDashPattern _dashPattern;
    protected DashCap _dashCap;

    protected float[] _compoundArray;
    protected Matrix _transformation;

    protected int? _cachedHashCode;


    public const double DefaultWidth = 1;
    public const PenAlignment DefaultAlignment = PenAlignment.Center;
    public const LineJoin DefaultLineJoin = LineJoin.Miter;
    public const double DefaultMiterLimit = 10;


    #region "ConfiguredProperties"

    [Serializable]
    [Flags]
    public enum Configured
    {
      Nothing = 0x00000,
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

    protected static Configured _GetConfiguredPropertiesVariable(PenX pen)
    {
      if (null == pen)
        throw new ArgumentNullException(nameof(pen));

      Configured c = Configured.Nothing;
      if (pen.PenType != PenType.SolidColor)
        c |= Configured.PenType;
      if (pen.PenType == PenType.SolidColor && pen.Color != NamedColors.Black)
        c |= Configured.Color;
      if (pen.PenType != PenType.SolidColor)
        c |= Configured.Brush;
      if (pen.Alignment != PenAlignment.Center)
        c |= Configured.Alignment;
      if (pen.CompoundArray != null && pen.CompoundArray.Length > 0)
        c |= Configured.CompoundArray;
      if (pen.DashPattern != Solid.Instance)
        c |= Configured.DashStyle;
      if (pen.DashPattern != Solid.Instance && pen.DashCap != DashCap.Flat)
        c |= Configured.DashCap;
      if (pen.DashPattern != Solid.Instance && pen.DashPattern != null && pen.DashPattern?.DashOffset != 0)
        c |= Configured.DashOffset;
      if (pen.DashPattern != Solid.Instance && pen.DashPattern != null)
        c |= Configured.DashPattern;
      if (!pen.EndCap.IsDefaultStyle)
        c |= Configured.EndCap;
      if (!pen.StartCap.IsDefaultStyle)
        c |= Configured.StartCap;
      if (pen.LineJoin != LineJoin.Miter)
        c |= Configured.LineJoin;
      if (pen.MiterLimit != 10)
        c |= Configured.MiterLimit;
      if (null != pen.Transform && !pen.Transform.IsIdentity)
        c |= Configured.Transform;
      if (pen.Width != 1)
        c |= Configured.Width;

      return c;
    }

    #endregion "ConfiguredProperties"

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.PenHolder", 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
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

        s._configuredProperties = (Configured)info.GetInt32("Configured");
        Configured cp = s._configuredProperties;

        if (0 != (cp & PenX.Configured.PenType))
          s._penType = (PenType)info.GetEnum("Type", typeof(PenType));
        else
          s._penType = PenType.SolidColor;

        if (0 != (cp & PenX.Configured.Alignment))
          s._alignment = (PenAlignment)info.GetEnum("Alignment", typeof(PenAlignment));
        else
          s._alignment = PenAlignment.Center;

        if (0 != (cp & PenX.Configured.Brush))
          s._brush = (BrushX)info.GetValue("Brush", s);
        else
          s._brush = new BrushX(NamedColors.Black);

        if (0 != (cp & PenX.Configured.Color))
          s._color = (NamedColor)info.GetValue("Color", s);
        else
          s._color = NamedColors.Black;

        if (0 != (cp & PenX.Configured.CompoundArray))
          info.GetArray(out s._compoundArray);
        else
          s._compoundArray = new float[0];

        DashStyle cachedDashStyle;
        if (0 != (cp & PenX.Configured.DashStyle))
          cachedDashStyle = (DashStyle)info.GetEnum("DashStyle", typeof(DashStyle));
        else
          cachedDashStyle = DashStyle.Solid;

        if (0 != (cp & PenX.Configured.DashCap))
          s._dashCap = (DashCap)info.GetEnum("DashCap", typeof(DashCap));
        else
          s._dashCap = DashCap.Flat;

        float cachedDashOffset;
        if (0 != (cp & PenX.Configured.DashOffset))
          cachedDashOffset = info.GetSingle("DashOffset");
        else
          cachedDashOffset = 0;

        float[] cachedDashPattern;
        if (0 != (cp & PenX.Configured.DashPattern))
          info.GetArray(out cachedDashPattern);
        else
          cachedDashPattern = null;

        s.SetDashPatternFromCachedDashPropertiesAfterOldDeserialization(cachedDashStyle, cachedDashPattern, cachedDashOffset);

        if (0 != (cp & PenX.Configured.EndCap))
        {
          var cap = (LineCap)info.GetEnum("EndCap", typeof(LineCap));
          s._endCap = LineCapExtension.FromName(Enum.GetName(typeof(LineCap), cap));
        }
        else
          s._endCap = LineCapExtension.Flat;

        if (0 != (cp & PenX.Configured.LineJoin))
          s._lineJoin = (LineJoin)info.GetEnum("LineJoin", typeof(LineJoin));
        else
          s._lineJoin = LineJoin.Miter;

        if (0 != (cp & PenX.Configured.MiterLimit))
          s._miterLimit = info.GetSingle("MiterLimit");
        else
          s._miterLimit = 10;

        if (0 != (cp & PenX.Configured.StartCap))
        {
          var cap = (LineCap)info.GetEnum("StartCap", typeof(LineCap));
          s._startCap = LineCapExtension.FromName(Enum.GetName(typeof(LineCap), cap));
        }
        else
          s._startCap = LineCapExtension.Flat;

        if (0 != (cp & PenX.Configured.Transform))
        {
          info.GetArray(out var el);
          s._transformation = new Matrix(el[0], el[1], el[2], el[3], el[4], el[5]);
        }
        else
          s._transformation = new Matrix();

        if (0 != (cp & PenX.Configured.Width))
          s._width = info.GetDouble("Width");
        else
          s._width = 1;

        return s;
      }
    }

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.PenHolder", 1)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.Gdi.PenX", 2)]
    private class XmlSerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        throw new ApplicationException("Serialization into old versions is not supported");
        /*
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
    info.AddValue("EndCapSize", s.m_EndCap.MinimumAbsoluteSizePt);
}
if (0 != (cp & PenX.Configured.LineJoin))
    info.AddEnum("LineJoin", s.LineJoin);
if (0 != (cp & PenX.Configured.MiterLimit))
    info.AddValue("MiterLimit", s.MiterLimit);
if (0 != (cp & PenX.Configured.StartCap))
{
    info.AddValue("StartCap", s.StartCap.Name);
    info.AddValue("StartCapSize", s.m_StartCap.MinimumAbsoluteSizePt);
}
if (0 != (cp & PenX.Configured.Transform))
    info.AddArray("Transform", s.Transform.Elements, s.Transform.Elements.Length);
if (0 != (cp & PenX.Configured.Width))
    info.AddValue("Width", s.Width);
*/
      }

      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        PenX s = null != o ? (PenX)o : new PenX();

        s._configuredProperties = (Configured)info.GetInt32("Configured");
        Configured cp = s._configuredProperties;

        if (0 != (cp & PenX.Configured.PenType))
          s._penType = (PenType)info.GetEnum("Type", typeof(PenType));
        else
          s._penType = PenType.SolidColor;

        if (0 != (cp & PenX.Configured.Alignment))
          s._alignment = (PenAlignment)info.GetEnum("Alignment", typeof(PenAlignment));
        else
          s._alignment = PenAlignment.Center;

        if (0 != (cp & PenX.Configured.Brush))
          s._brush = (BrushX)info.GetValue("Brush", s);
        else
          s._brush = new BrushX(NamedColors.Black);



        if (0 != (cp & PenX.Configured.Color))
          s._color = (NamedColor)info.GetValue("Color", s);
        else
          s._color = NamedColors.Black;

        if (0 != (cp & PenX.Configured.CompoundArray))
          info.GetArray(out s._compoundArray);
        else
          s._compoundArray = new float[0];
        DashStyle cachedDashStyle;
        if (0 != (cp & PenX.Configured.DashStyle))
          cachedDashStyle = (DashStyle)info.GetEnum("DashStyle", typeof(DashStyle));
        else
          cachedDashStyle = DashStyle.Solid;

        if (0 != (cp & PenX.Configured.DashCap))
          s._dashCap = (DashCap)info.GetEnum("DashCap", typeof(DashCap));
        else
          s._dashCap = DashCap.Flat;

        float cachedDashOffset;
        if (0 != (cp & PenX.Configured.DashOffset))
          cachedDashOffset = info.GetSingle("DashOffset");
        else
          cachedDashOffset = 0;

        float[] cachedDashPattern;
        if (0 != (cp & PenX.Configured.DashPattern))
          info.GetArray(out cachedDashPattern);
        else
          cachedDashPattern = null;

        s.SetDashPatternFromCachedDashPropertiesAfterOldDeserialization(cachedDashStyle, cachedDashPattern, cachedDashOffset);

        if (0 != (cp & PenX.Configured.EndCap))
        {
          string name = info.GetString("EndCap");
          var size = info.GetDouble("EndCapSize");
          s._endCap = LineCapExtension.FromNameAndAbsAndRelSize(name, size, 2);
        }
        else
          s._endCap = LineCapExtension.Flat;

        if (0 != (cp & PenX.Configured.LineJoin))
          s._lineJoin = (LineJoin)info.GetEnum("LineJoin", typeof(LineJoin));
        else
          s._lineJoin = LineJoin.Miter;

        if (0 != (cp & PenX.Configured.MiterLimit))
          s._miterLimit = info.GetSingle("MiterLimit");
        else
          s._miterLimit = 10;

        if (0 != (cp & PenX.Configured.StartCap))
        {
          string name = info.GetString("StartCap");
          var size = info.GetDouble("StartCapSize");
          s._startCap = LineCapExtension.FromNameAndAbsAndRelSize(name, size, 2);
        }
        else
          s._startCap = LineCapExtension.Flat;

        if (0 != (cp & PenX.Configured.Transform))
        {
          info.GetArray(out var el);
          s._transformation = new Matrix(el[0], el[1], el[2], el[3], el[4], el[5]);
        }
        else
          s._transformation = new Matrix();

        if (0 != (cp & PenX.Configured.Width))
          s._width = info.GetDouble("Width");
        else
          s._width = 1;

        return s;
      }
    }

    /// <summary>
    /// 2012-03-07: New in version 3: StartCap and EndCap now have a RelativeSize property. The 'StartCapSize' and 'EndCapSize' property was renamed to 'StartCapAbsSize' and 'EndCapAbsSize'.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.Gdi.PenX", 3)]
    private class XmlSerializationSurrogate3 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        throw new InvalidOperationException("Serialization of old version");
        /*
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
    info.AddValue("EndCapAbsSize", s._endCap.MinimumAbsoluteSizePt);
    info.AddValue("EndCapRelSize", s._endCap.MinimumRelativeSize);
}
if (0 != (cp & PenX.Configured.LineJoin))
    info.AddEnum("LineJoin", s.LineJoin);
if (0 != (cp & PenX.Configured.MiterLimit))
    info.AddValue("MiterLimit", s.MiterLimit);
if (0 != (cp & PenX.Configured.StartCap))
{
    info.AddValue("StartCap", s.StartCap.Name);
    info.AddValue("StartCapAbsSize", s._startCap.MinimumAbsoluteSizePt);
    info.AddValue("StartCapRelSize", s._startCap.MinimumRelativeSize);
}
if (0 != (cp & PenX.Configured.Transform))
    info.AddArray("Transform", s.Transform.Elements, s.Transform.Elements.Length);
if (0 != (cp & PenX.Configured.Width))
    info.AddValue("Width", s.Width);
*/
      }

      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        PenX s = null != o ? (PenX)o : new PenX();

        s._configuredProperties = (Configured)info.GetInt32("Configured");
        Configured cp = s._configuredProperties;

        if (0 != (cp & PenX.Configured.PenType))
          s._penType = (PenType)info.GetEnum("Type", typeof(PenType));
        else
          s._penType = PenType.SolidColor;

        if (0 != (cp & PenX.Configured.Alignment))
          s._alignment = (PenAlignment)info.GetEnum("Alignment", typeof(PenAlignment));
        else
          s._alignment = PenAlignment.Center;

        if (0 != (cp & PenX.Configured.Brush))
          s._brush = (BrushX)info.GetValue("Brush", s);
        else
          s._brush = new BrushX(NamedColors.Black);



        if (0 != (cp & PenX.Configured.Color))
          s._color = (NamedColor)info.GetValue("Color", s);
        else
          s._color = NamedColors.Black;

        if (0 != (cp & PenX.Configured.CompoundArray))
          info.GetArray(out s._compoundArray);
        else
          s._compoundArray = new float[0];

        DashStyle cachedDashStyle;
        if (0 != (cp & PenX.Configured.DashStyle))
          cachedDashStyle = (DashStyle)info.GetEnum("DashStyle", typeof(DashStyle));
        else
          cachedDashStyle = DashStyle.Solid;

        if (0 != (cp & PenX.Configured.DashCap))
          s._dashCap = (DashCap)info.GetEnum("DashCap", typeof(DashCap));
        else
          s._dashCap = DashCap.Flat;

        float cachedDashOffset;
        if (0 != (cp & PenX.Configured.DashOffset))
          cachedDashOffset = info.GetSingle("DashOffset");
        else
          cachedDashOffset = 0;

        float[] cachedDashPattern;
        if (0 != (cp & PenX.Configured.DashPattern))
          info.GetArray(out cachedDashPattern);
        else
          cachedDashPattern = null;

        s.SetDashPatternFromCachedDashPropertiesAfterOldDeserialization(cachedDashStyle, cachedDashPattern, cachedDashOffset);

        if (0 != (cp & PenX.Configured.EndCap))
        {
          string name = info.GetString("EndCap");
          var absSize = info.GetDouble("EndCapAbsSize");
          var relSize = info.GetDouble("EndCapRelSize");
          s._endCap = LineCapExtension.FromNameAndAbsAndRelSize(name, absSize, relSize);
        }
        else
          s._endCap = LineCapExtension.Flat;

        if (0 != (cp & PenX.Configured.LineJoin))
          s._lineJoin = (LineJoin)info.GetEnum("LineJoin", typeof(LineJoin));
        else
          s._lineJoin = LineJoin.Miter;

        if (0 != (cp & PenX.Configured.MiterLimit))
          s._miterLimit = info.GetSingle("MiterLimit");
        else
          s._miterLimit = 10;

        if (0 != (cp & PenX.Configured.StartCap))
        {
          string name = info.GetString("StartCap");
          var absSize = info.GetDouble("StartCapAbsSize");
          var relSize = info.GetDouble("StartCapRelSize");
          s._startCap = LineCapExtension.FromNameAndAbsAndRelSize(name, absSize, relSize);
        }
        else
          s._startCap = LineCapExtension.Flat;

        if (0 != (cp & PenX.Configured.Transform))
        {
          info.GetArray(out var el);
          s._transformation = new Matrix(el[0], el[1], el[2], el[3], el[4], el[5]);
        }
        else
          s._transformation = new Matrix();

        if (0 != (cp & PenX.Configured.Width))
          s._width = info.GetDouble("Width");
        else
          s._width = 1;

        return s;
      }
    }

    /// <summary>
    /// 2016-10-10: New in version 4: use Altaxo.Drawing.IDashPattern instead of all the dashpattern properties.
    ///
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(PenX), 4)]
    private class XmlSerializationSurrogate4 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (PenX)obj;
        Configured cp = PenX._GetConfiguredPropertiesVariable(s);

        info.AddValue("Configured", (int)cp);
        if (0 != (cp & PenX.Configured.PenType))
          info.AddEnum("Type", s.PenType);
        if (0 != (cp & PenX.Configured.Alignment))
          info.AddEnum("Alignment", s.Alignment);
        if (0 != (cp & PenX.Configured.Brush))
          info.AddValue("Brush", s.Brush);
        if (0 != (cp & PenX.Configured.Color))
          info.AddValue("Color", s.Color);
        if (0 != (cp & PenX.Configured.CompoundArray))
          info.AddArray("CompoundArray", s.CompoundArray, s.CompoundArray.Length);

        // Note: we must even save the solid pattern if it belongs to another list than the BuiltinDefault list,
        // otherwise when deserializing we wouldn't know to which list the solid dash pattern belongs to.
        if (null != s._dashPattern && (!Drawing.DashPatterns.Solid.Instance.Equals(s._dashPattern) || !object.ReferenceEquals(DashPatternListManager.Instance.BuiltinDefault, DashPatternListManager.Instance.GetParentList(s._dashPattern))))
        {
          info.AddValue("DashPattern", s._dashPattern);
          if (0 != (cp & PenX.Configured.DashCap))
            info.AddEnum("DashCap", s.DashCap);
        }

        if (0 != (cp & PenX.Configured.EndCap))
        {
          info.AddValue("EndCap", s.EndCap.Name);
          info.AddValue("EndCapAbsSize", s._endCap.MinimumAbsoluteSizePt);
          info.AddValue("EndCapRelSize", s._endCap.MinimumRelativeSize);
        }
        if (0 != (cp & PenX.Configured.LineJoin))
          info.AddEnum("LineJoin", s.LineJoin);
        if (0 != (cp & PenX.Configured.MiterLimit))
          info.AddValue("MiterLimit", s.MiterLimit);
        if (0 != (cp & PenX.Configured.StartCap))
        {
          info.AddValue("StartCap", s.StartCap.Name);
          info.AddValue("StartCapAbsSize", s._startCap.MinimumAbsoluteSizePt);
          info.AddValue("StartCapRelSize", s._startCap.MinimumRelativeSize);
        }
        if (0 != (cp & PenX.Configured.Transform))
          info.AddArray("Transform", s.Transform.Elements, s.Transform.Elements.Length);
        if (0 != (cp & PenX.Configured.Width))
          info.AddValue("Width", s.Width);
      }

      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        PenX s = null != o ? (PenX)o : new PenX();

        s._configuredProperties = (Configured)info.GetInt32("Configured");
        Configured cp = s._configuredProperties;

        if (0 != (cp & PenX.Configured.PenType))
          s._penType = (PenType)info.GetEnum("Type", typeof(PenType));
        else
          s._penType = PenType.SolidColor;

        if (0 != (cp & PenX.Configured.Alignment))
          s._alignment = (PenAlignment)info.GetEnum("Alignment", typeof(PenAlignment));
        else
          s._alignment = PenAlignment.Center;

        if (0 != (cp & PenX.Configured.Brush))
          s._brush = (BrushX)info.GetValue("Brush", s);
        else
          s._brush = new BrushX(NamedColors.Black);



        if (0 != (cp & PenX.Configured.Color))
          s._color = (NamedColor)info.GetValue("Color", s);
        else
          s._color = NamedColors.Black;

        if (0 != (cp & PenX.Configured.CompoundArray))
          info.GetArray(out s._compoundArray);
        else
          s._compoundArray = new float[0];

        if (info.CurrentElementName == "DashPattern")
        {
          s._dashPattern = (IDashPattern)info.GetValue("DashPattern", null);
          if (!Drawing.DashPatterns.Solid.Instance.Equals(s._dashPattern))
          {
            if (0 != (cp & PenX.Configured.DashCap))
              s._dashCap = (DashCap)info.GetEnum("DashCap", typeof(DashCap));
            else
              s._dashCap = DashCap.Flat;
          }
        }

        if (0 != (cp & PenX.Configured.EndCap))
        {
          string name = info.GetString("EndCap");
          var absSize = info.GetDouble("EndCapAbsSize");
          var relSize = info.GetDouble("EndCapRelSize");
          s._endCap = LineCapExtension.FromNameAndAbsAndRelSize(name, absSize, relSize);
        }
        else
          s._endCap = LineCapExtension.Flat;

        if (0 != (cp & PenX.Configured.LineJoin))
          s._lineJoin = (LineJoin)info.GetEnum("LineJoin", typeof(LineJoin));
        else
          s._lineJoin = LineJoin.Miter;

        if (0 != (cp & PenX.Configured.MiterLimit))
          s._miterLimit = info.GetSingle("MiterLimit");
        else
          s._miterLimit = 10;

        if (0 != (cp & PenX.Configured.StartCap))
        {
          string name = info.GetString("StartCap");
          var absSize = info.GetDouble("StartCapAbsSize");
          var relSize = info.GetDouble("StartCapRelSize");
          s._startCap = LineCapExtension.FromNameAndAbsAndRelSize(name, absSize, relSize);
        }
        else
          s._startCap = LineCapExtension.Flat;

        if (0 != (cp & PenX.Configured.Transform))
        {
          info.GetArray(out var el);
          s._transformation = new Matrix(el[0], el[1], el[2], el[3], el[4], el[5]);
        }
        else
          s._transformation = new Matrix();

        if (0 != (cp & PenX.Configured.Width))
          s._width = info.GetDouble("Width");
        else
          s._width = 1;

        return s;
      }
    }

    #endregion Serialization

    public PenX()
      : this(NamedColors.Black, 1)
    {
    }

    public PenX(NamedColor c)
        : this(c, 1)
    {
    }

    public PenX(NamedColor c, double width)
    {
      _penType = PenType.SolidColor;
      _color = c;
      _width = width;
      _alignment = DefaultAlignment;
      _lineJoin = DefaultLineJoin;
      _miterLimit = DefaultMiterLimit;
      _dashPattern = DashPatternListManager.Instance.BuiltinDefaultSolid;
      _startCap = LineCapExtension.Flat;
      _endCap = LineCapExtension.Flat;


      _SetProp(Configured.Color, NamedColors.Black != c);
      _SetProp(Configured.Width, 1 != width);
    }

    private bool Equals(PenX other, bool penWidthMattersHere)
    {
      if (other is null)
        return false;
      if (object.ReferenceEquals(this, other))
        return true;

      if (this._brush is null && other._brush is null)
      {
        if (!(this._color == other._color))
          return false;
      }
      else
      {
        if (!(this._brush == other._brush))
          return false;
      }

      if (!(this._width == other._width) && penWidthMattersHere)
        return false;

      if (!(this._alignment == other._alignment))
        return false;

      if (!(this._lineJoin == other._lineJoin))
        return false;

      if (!(this._miterLimit == other._miterLimit))
        return false;

      if (!(this._startCap == other._startCap))
        return false;

      if (!(this._endCap == other._endCap))
        return false;


      if (!(this._dashPattern == other._dashPattern))
        return false;

      if (!(this._dashPattern is Drawing.DashPatterns.Solid))
      {
        if (!(this._dashCap != other._dashCap))
          return false;
      }

      if (!(AreEqual(this._compoundArray, other._compoundArray)))
        return false;

      if (!object.Equals(this._transformation, other._transformation))
        return false;

      return true;
    }

    public bool Equals(PenX other)
    {
      return Equals(other, true);
    }

    public override bool Equals(object obj)
    {
      return Equals(obj as PenX, true);
    }

    public static bool operator ==(PenX x, PenX y)
    {
      return x is { } _ ? x.Equals(y, true) : y is { } _ ? y.Equals(x, true) : true;
    }
    public static bool operator !=(PenX x, PenX y)
    {
      return !(x == y);
    }

    public static bool AreEqualUnlessWidth(PenX x, PenX y)
    {
      return x is { } _ ? x.Equals(y, false) : y is { } _ ? y.Equals(x, false) : true;
    }

    protected int CalculateHash()
    {
      int result = _brush is null ? _color.GetHashCode() : _brush.GetHashCode();

      unchecked
      {
        if (!(_width == DefaultWidth))
          result += 5 * _width.GetHashCode();

        if (!(_alignment == DefaultAlignment))
          result += 7 * _alignment.GetHashCode();

        if (!(_lineJoin == DefaultLineJoin))
          result += 11 * _lineJoin.GetHashCode();

        if (!(_miterLimit == DefaultMiterLimit))
          result += 13 * _miterLimit.GetHashCode();

        result += 17 * _startCap.GetHashCode();
        result += 19 * _endCap.GetHashCode();
        result += 23 * _dashPattern.GetHashCode();

        if (!(this._dashPattern is Drawing.DashPatterns.Solid))
          result += 29 * _dashCap.GetHashCode();

        if (_compoundArray is { } _)
          result += 31 * _compoundArray.GetHashCode();

        if (_transformation is { } _)
          result += 37 * _transformation.GetHashCode();
      }
      return result;
    }

    public override int GetHashCode()
    {
      if (!_cachedHashCode.HasValue)
      {
        _cachedHashCode = CalculateHash();
      }
      return _cachedHashCode.Value;
    }


    private static bool AreEqual(float[] x1, float[] x2)
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

    protected PenX Clone()
    {
      var result = (PenX)MemberwiseClone();
      result._cachedHashCode = null;
      return result;
    }

    public PenType PenType
    {
      get { return _penType; }
    }

    public PenAlignment Alignment
    {
      get { return _alignment; }
    }

    public PenX WithAlignment(PenAlignment value)
    {
      if (!(_alignment == value))
      {
        var result = Clone();
        result._alignment = value;
        result._SetProp(Configured.Alignment, PenAlignment.Center != value);
        return result;
      }
      else
      {
        return this;
      }
    }

    /// <summary>
    /// Determines if this pen is visible. It is visible if it has a visible brush.
    /// </summary>
    public bool IsVisible
    {
      get
      {
        return (_brush != null && _brush.IsVisible) || _color.Color.A != 0;
      }
    }

    /// <summary>
    /// Determines if this pen is invisible. It is invisible if it has an invisible brush or the color is transparent.
    /// </summary>
    public bool IsInvisible
    {
      get
      {
        return (_brush != null && _brush.IsInvisible) || _color.Color.A == 0;
      }
    }

    public BrushX Brush
    {
      get
      {
        return _brush ?? new BrushX(_color);
      }
    }

    public PenX WithBrush(BrushX value)
    {
      if (!(Brush == value))
      {
        var result = Clone();
        result._brush = value;

        if (value is null)
        {
          result._SetProp(Configured.PenType, false);
          result._SetProp(Configured.Color, NamedColors.Black != _color);
          result._penType = PenType.SolidColor;
        }
        else if (value.BrushType == BrushType.SolidBrush)
        {
          result._penType = PenType.SolidColor;
          result._color = value.Color;
          result._SetProp(Configured.PenType, PenType.SolidColor != _penType);
          result._SetProp(Configured.Color, NamedColors.Black != _color);
          result._SetProp(Configured.Brush, false);
        } // if value is SolidBrush
        else if (value.BrushType == BrushType.HatchBrush)
        {
          result._penType = PenType.HatchFill;
          result._SetProp(Configured.PenType, true);
          result._SetProp(Configured.Color, false);
          result._SetProp(Configured.Brush, true);
        }
        else if (value.BrushType == BrushType.TextureBrush)
        {
          result._penType = PenType.TextureFill;
          result._SetProp(Configured.PenType, true);
          result._SetProp(Configured.Color, false);
          result._SetProp(Configured.Brush, true);
        }
        else if (value.BrushType == BrushType.LinearGradientBrush)
        {
          result._penType = PenType.LinearGradient;
          result._SetProp(Configured.PenType, true);
          result._SetProp(Configured.Color, false);
          result._SetProp(Configured.Brush, true);
        }
        else if (value.BrushType == BrushType.PathGradientBrush)
        {
          result._penType = PenType.PathGradient;
          result._SetProp(Configured.PenType, true);
          result._SetProp(Configured.Color, false);
          result._SetProp(Configured.Brush, true);
        }

        return result;
      }
      else
      {
        return this;
      }
    }

    public NamedColor Color
    {
      get
      {
        return _brush?.Color ?? _color;
      }
    }

    public PenX WithColor(NamedColor value)
    {
      if (!(Color == value))
      {
        var result = Clone();

        if (result._brush is { } brush)
          result._brush = brush.WithColor(value);
        else
          result._SetProp(Configured.Color, NamedColors.Black != value);

        return result;
      }
      else
      {
        return this;
      }
    }

    public float[] CompoundArray
    {
      get { return _compoundArray; }
    }

    public PenX WithCompoundArray(float[] value)
    {
      if (!(_compoundArray == value))
      {
        var result = Clone();
        result._compoundArray = (float[])value?.Clone();
        result._SetProp(Configured.CompoundArray, null != value && value.Length > 0);

        return result;
      }
      else
      {
        return this;
      }
    }

    public DashCap DashCap
    {
      get { return _dashCap; }
    }

    public PenX WithDashCap(DashCap value)
    {
      if (!(_dashCap == value))
      {
        var result = Clone();
        result._dashCap = value;
        result._SetProp(Configured.DashCap, DashCap.Flat != value);
        return result;
      }
      else
      {
        return this;
      }
    }

    public LineCapExtension StartCap
    {
      get
      {
        return _startCap;
      }
    }

    public PenX WithStartCap(LineCapExtension value)
    {
      if (value is null)
        throw new ArgumentNullException(nameof(value));

      if (!(object.Equals(_startCap, value)))
      {
        var result = Clone();
        result._startCap = value;
        result._SetProp(Configured.StartCap, value != null && !value.IsDefaultStyle);

        return result;
      }
      else
      {
        return this;
      }
    }

    public LineCapExtension EndCap
    {
      get
      {
        return _endCap;
      }
    }

    public PenX WithEndCap(LineCapExtension value)
    {
      if (value is null)
        throw new ArgumentNullException(nameof(value));

      if (!(object.Equals(_endCap, value)))
      {
        var result = Clone();
        result._endCap = value;
        result._SetProp(Configured.EndCap, result._endCap != null && !result._endCap.IsDefaultStyle);

        return result;
      }
      else
      {
        return this;
      }
    }

    public Altaxo.Drawing.IDashPattern DashPattern
    {
      get
      {
        return _dashPattern;
      }
    }

    public PenX WithDashPattern(IDashPattern value)
    {
      if (null == value)
        throw new ArgumentNullException();

      if (!(object.Equals(_dashPattern, value)))
      {
        var result = Clone();
        result._dashPattern = value;
        return result;
      }
      else
      {
        return this;
      }
    }


    /// <summary>
    /// Sets the <see cref="_dashPattern"/> member after deserialization of old versions (before 2016-10-10).
    /// </summary>
    private void SetDashPatternFromCachedDashPropertiesAfterOldDeserialization(DashStyle cachedDashStyle, float[] cachedDashPattern, float cachedDashOffset)
    {
      if (!_configuredProperties.HasFlag(Configured.DashStyle))
      {
        _dashPattern = DashPatternListManager.Instance.BuiltinDefaultSolid;
      }
      else // DashStyle is configured
      {
        switch (cachedDashStyle)
        {
          case DashStyle.Solid:
            _dashPattern = DashPatternListManager.Instance.BuiltinDefaultSolid;
            break;

          case DashStyle.Dash:
            _dashPattern = DashPatternListManager.Instance.BuiltinDefaultDash;
            break;

          case DashStyle.Dot:
            _dashPattern = DashPatternListManager.Instance.BuiltinDefaultDot;
            break;

          case DashStyle.DashDot:
            _dashPattern = DashPatternListManager.Instance.BuiltinDefaultDashDot;
            break;

          case DashStyle.DashDotDot:
            _dashPattern = DashPatternListManager.Instance.BuiltinDefaultDashDotDot;
            break;

          case DashStyle.Custom:
            _dashPattern = new Drawing.DashPatterns.Custom(cachedDashPattern.Select(x => (double)x), cachedDashOffset);
            break;

          default:
            throw new NotImplementedException();
        }
      }
    }

    private static bool IsEqual(float[] a, float[] b)
    {
      if (a == null || b == null)
        return false;
      if (a.Length != b.Length)
        return false;
      for (int i = a.Length - 1; i >= 0; i--)
        if (a[i] != b[i])
          return false;

      return true;
    }



    public LineJoin LineJoin
    {
      get { return _lineJoin; }
    }

    public PenX WithLineJoin(LineJoin value)
    {
      if (!(_lineJoin == value))
      {
        var result = Clone();
        result._lineJoin = value;
        result._SetProp(Configured.LineJoin, LineJoin.Miter != value);
        return result;
      }
      else
      {
        return this;
      }
    }

    public double MiterLimit
    {
      get { return _miterLimit; }
    }

    public PenX WithMiterLimit(double value)
    {
      if (!(_miterLimit == value))
      {
        var result = Clone();
        result._miterLimit = value;
        result._SetProp(Configured.MiterLimit, 10 != value);
        return result;
      }
      else
      {
        return this;
      }
    }

    public Matrix Transform
    {
      get { return _transformation; }
    }

    public PenX WithTransform(Matrix value)
    {
      if (value is null)
        throw new ArgumentNullException(nameof(value));

      if (!(object.Equals(_transformation, value)))
      {
        var result = Clone();
        result._transformation = value.Clone();
        result._SetProp(Configured.Transform, null != value && !value.IsIdentity);
        return result;
      }
      else
      {
        return this;
      }
    }

    public double Width
    {
      get { return _width; }
    }

    public PenX WithWidth(double value)
    {
      if (!(_width == value))
      {
        var result = Clone();
        result._width = value;
        result._SetProp(Configured.Width, 1 != value);
        return result;
      }
      else
      {
        return this;
      }
    }

    private void _SetProp(Configured prop, bool bSet)
    {
      _configuredProperties &= (Configured.All ^ prop);
      if (bSet)
        _configuredProperties |= prop;
    }
  } // end of class PenX
}
