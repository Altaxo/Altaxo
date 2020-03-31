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
using Altaxo.Drawing.DashPatternManagement;
using Altaxo.Drawing.DashPatterns;
using Altaxo.Drawing.LineCaps;
using Altaxo.Geometry;
using Altaxo.Main;

#nullable enable

namespace Altaxo.Drawing
{
  /// <summary>
  /// PenX is a graphics framework independent pen object. Use <see cref="Altaxo.Graph.Gdi.PenCacheGdi.Instance"/> to convert it into a Gdi pen.
  /// </summary>
  [Serializable]
  public class PenX : IImmutable, IEquatable<PenX>
  {
    /// <summary>The brush of the pen.</summary>
    protected BrushX _brush; // the brush of this pen

    /// <summary>The width of the pen.</summary>
    protected double _width; // Width of this Pen object


    /// <summary>The alignment of the pen.</summary>
    protected PenAlignment _alignment;
    protected LineJoin _lineJoin;
    protected double _miterLimit;
    protected ILineCap _startCap;
    protected ILineCap _endCap;
    protected IDashPattern _dashPattern;
    protected DashCap _dashCap;

    protected double[]? _compoundArray;
    protected Matrix3x2Class? _transformation;

    protected int? _cachedHashCode;


    public const double DefaultWidth = 1;
    public const PenAlignment DefaultAlignment = PenAlignment.Center;
    public const LineJoin DefaultLineJoin = LineJoin.Miter;
    public const double DefaultMiterLimit = 10;

    #region "ConfiguredProperties"

    [Flags]
    private enum Configured // Attention: do not change the values, because it is needed for serialization of old versions
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

    /// <summary>
    /// Gets a flag enum that specifies with of the brush properties should be serialized or deserialized.
    /// </summary>
    /// <returns>A flag enum that specifies with of the brush properties should be serialized or deserialized.</returns>
    private Configured GetConfiguredProperties()
    {
      var c = Configured.Nothing;

      if (!(_brush.IsSolidBrush))
        c |= Configured.Brush;
      else
        c |= Configured.Color;

      if (!(_width == DefaultWidth))
        c |= Configured.Width;

      if (!(_alignment == DefaultAlignment))
        c |= Configured.Alignment;

      if (!(_lineJoin == DefaultLineJoin))
        c |= Configured.LineJoin;

      if (!(_miterLimit == DefaultMiterLimit))
        c |= Configured.MiterLimit;

      // Note: we must even save the solid pattern if it belongs to another list than the BuiltinDefault list,
      // otherwise when deserializing we wouldn't know to which list the solid dash pattern belongs to.
      if (!(_dashPattern is Solid) || !ReferenceEquals(DashPatternListManager.Instance.BuiltinDefault, DashPatternListManager.Instance.GetParentList(_dashPattern)))
        c |= Configured.DashPattern;

      bool hasDashPattern = !(_dashPattern is Solid);

      if (hasDashPattern && !(_dashCap == DashCap.Flat))
        c |= Configured.DashCap;

      if (hasDashPattern && _dashPattern?.DashOffset != 0)
        c |= Configured.DashOffset;

      if (!(_endCap is FlatCap))
        c |= Configured.EndCap;

      if (!(_startCap is FlatCap))
        c |= Configured.StartCap;

      if (_transformation is { } _)
        c |= Configured.Transform;

      if (_compoundArray is { } _)
        c |= Configured.CompoundArray;


      return c;
    }

    #endregion "ConfiguredProperties"

    #region Serialization

    #region Version 0

    [Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.PenHolder", 0)]
    private class XmlSerializationSurrogate0 : Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Serialization.Xml.IXmlSerializationInfo info)
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

      public object Deserialize(object o, Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        var s = null != o ? (PenX)o : new PenX();

        var cp = (Configured)info.GetInt32("Configured");

        if (0 != (cp & Configured.PenType))
          info.GetEnum("Type", typeof(PenType));

        if (0 != (cp & Configured.Alignment))
          s._alignment = (PenAlignment)info.GetEnum("Alignment", typeof(PenAlignment));

        if (0 != (cp & Configured.Brush))
          s._brush = (BrushX)info.GetValue("Brush", s) ?? BrushesX.Black;

        if (0 != (cp & Configured.Color))
        {
          var c = (NamedColor)info.GetValue("Color", s);
          s._brush = NamedColors.Black == c ? BrushesX.Black : new BrushX(c);
        }

        if (0 != (cp & Configured.CompoundArray))
          info.GetArray("CompoundArray", out s._compoundArray);

        DashStyle cachedDashStyle = DashStyle.Solid;
        if (0 != (cp & Configured.DashStyle))
          cachedDashStyle = (DashStyle)info.GetEnum("DashStyle", typeof(DashStyle));

        if (0 != (cp & Configured.DashCap))
          s._dashCap = (DashCap)info.GetEnum("DashCap", typeof(DashCap));


        float cachedDashOffset = 0;
        if (0 != (cp & Configured.DashOffset))
          cachedDashOffset = info.GetSingle("DashOffset");

        float[]? cachedDashPattern = null;
        if (0 != (cp & Configured.DashPattern))
          info.GetArray(out cachedDashPattern);

        s.SetDashPatternFromCachedDashPropertiesAfterOldDeserialization(cp, cachedDashStyle, cachedDashPattern, cachedDashOffset);

        if (0 != (cp & Configured.EndCap))
        {
          var cap = (LineCap)info.GetEnum("EndCap", typeof(LineCap));
          s._endCap = LineCapBase.FromName(Enum.GetName(typeof(LineCap), cap));
        }

        if (0 != (cp & Configured.LineJoin))
          s._lineJoin = (LineJoin)info.GetEnum("LineJoin", typeof(LineJoin));

        if (0 != (cp & Configured.MiterLimit))
          s._miterLimit = info.GetSingle("MiterLimit");

        if (0 != (cp & Configured.StartCap))
        {
          var cap = (LineCap)info.GetEnum("StartCap", typeof(LineCap));
          s._startCap = LineCapBase.FromName(Enum.GetName(typeof(LineCap), cap));
        }

        if (0 != (cp & Configured.Transform))
        {
          info.GetArray(out var el);
          s._transformation = new Matrix3x2Class(el[0], el[1], el[2], el[3], el[4], el[5]);
        }

        if (0 != (cp & Configured.Width))
          s._width = info.GetDouble("Width");

        s._cachedHashCode = null;
        return s;
      }
    }

    #endregion Version 0

    #region Version 1 and 2

    [Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.PenHolder", 1)]
    [Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.Gdi.PenX", 2)]
    private class XmlSerializationSurrogate1 : Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Serialization.Xml.IXmlSerializationInfo info)
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

      public object Deserialize(object o, Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        var s = null != o ? (PenX)o : new PenX();

        var cp = (Configured)info.GetInt32("Configured");

        if (0 != (cp & Configured.PenType))
          info.GetEnum("Type", typeof(PenType));

        if (0 != (cp & Configured.Alignment))
          s._alignment = (PenAlignment)info.GetEnum("Alignment", typeof(PenAlignment));

        if (0 != (cp & Configured.Brush))
          s._brush = (BrushX)info.GetValue("Brush", s) ?? BrushesX.Black;

        if (0 != (cp & Configured.Color))
        {
          var c = (NamedColor)info.GetValue("Color", s);
          s._brush = NamedColors.Black == c ? BrushesX.Black : new BrushX(c);
        }

        if (0 != (cp & Configured.CompoundArray))
          info.GetArray("CompoundArray", out s._compoundArray);

        DashStyle cachedDashStyle = DashStyle.Solid;
        if (0 != (cp & Configured.DashStyle))
          cachedDashStyle = (DashStyle)info.GetEnum("DashStyle", typeof(DashStyle));

        if (0 != (cp & Configured.DashCap))
          s._dashCap = (DashCap)info.GetEnum("DashCap", typeof(DashCap));

        float cachedDashOffset = 0;
        if (0 != (cp & Configured.DashOffset))
          cachedDashOffset = info.GetSingle("DashOffset");

        float[]? cachedDashPattern = null;
        if (0 != (cp & Configured.DashPattern))
          info.GetArray(out cachedDashPattern);

        s.SetDashPatternFromCachedDashPropertiesAfterOldDeserialization(cp, cachedDashStyle, cachedDashPattern, cachedDashOffset);

        if (0 != (cp & Configured.EndCap))
        {
          var name = info.GetString("EndCap");
          var size = info.GetDouble("EndCapSize");
          s._endCap = LineCapBase.FromNameAndAbsAndRelSize(name, size, 2);
        }

        if (0 != (cp & Configured.LineJoin))
          s._lineJoin = (LineJoin)info.GetEnum("LineJoin", typeof(LineJoin));

        if (0 != (cp & Configured.MiterLimit))
          s._miterLimit = info.GetSingle("MiterLimit");

        if (0 != (cp & Configured.StartCap))
        {
          var name = info.GetString("StartCap");
          var size = info.GetDouble("StartCapSize");
          s._startCap = LineCapBase.FromNameAndAbsAndRelSize(name, size, 2);
        }

        if (0 != (cp & Configured.Transform))
        {
          info.GetArray(out var el);
          s._transformation = new Matrix3x2Class(el[0], el[1], el[2], el[3], el[4], el[5]);
        }

        if (0 != (cp & Configured.Width))
          s._width = info.GetDouble("Width");

        s._cachedHashCode = null;
        return s;
      }
    }

    #endregion Version 2

    #region Version 3

    /// <summary>
    /// 2012-03-07: New in version 3: StartCap and EndCap now have a RelativeSize property. The 'StartCapSize' and 'EndCapSize' property was renamed to 'StartCapAbsSize' and 'EndCapAbsSize'.
    /// </summary>
    [Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.Gdi.PenX", 3)]
    private class XmlSerializationSurrogate3 : Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Serialization.Xml.IXmlSerializationInfo info)
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

      public object Deserialize(object o, Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        var s = null != o ? (PenX)o : new PenX();

        var cp = (Configured)info.GetInt32("Configured");

        if (0 != (cp & Configured.PenType))
          info.GetEnum("Type", typeof(PenType));

        if (0 != (cp & Configured.Alignment))
          s._alignment = (PenAlignment)info.GetEnum("Alignment", typeof(PenAlignment));

        if (0 != (cp & Configured.Brush))
          s._brush = (BrushX)info.GetValue("Brush", s) ?? BrushesX.Black;

        if (0 != (cp & Configured.Color))
        {
          var c = (NamedColor)info.GetValue("Color", s);
          s._brush = NamedColors.Black == c ? BrushesX.Black : new BrushX(c);
        }

        if (0 != (cp & Configured.CompoundArray))
          info.GetArray("CompoundArray", out s._compoundArray);

        DashStyle cachedDashStyle = DashStyle.Solid;
        if (0 != (cp & Configured.DashStyle))
          cachedDashStyle = (DashStyle)info.GetEnum("DashStyle", typeof(DashStyle));

        if (0 != (cp & Configured.DashCap))
          s._dashCap = (DashCap)info.GetEnum("DashCap", typeof(DashCap));


        float cachedDashOffset = 0;
        if (0 != (cp & Configured.DashOffset))
          cachedDashOffset = info.GetSingle("DashOffset");

        float[]? cachedDashPattern = null;
        if (0 != (cp & Configured.DashPattern))
          info.GetArray(out cachedDashPattern);

        s.SetDashPatternFromCachedDashPropertiesAfterOldDeserialization(cp, cachedDashStyle, cachedDashPattern, cachedDashOffset);

        if (0 != (cp & Configured.EndCap))
        {
          var name = info.GetString("EndCap");
          var absSize = info.GetDouble("EndCapAbsSize");
          var relSize = info.GetDouble("EndCapRelSize");
          s._endCap = LineCapBase.FromNameAndAbsAndRelSize(name, absSize, relSize);
        }

        if (0 != (cp & Configured.LineJoin))
          s._lineJoin = (LineJoin)info.GetEnum("LineJoin", typeof(LineJoin));

        if (0 != (cp & Configured.MiterLimit))
          s._miterLimit = info.GetSingle("MiterLimit");

        if (0 != (cp & Configured.StartCap))
        {
          var name = info.GetString("StartCap");
          var absSize = info.GetDouble("StartCapAbsSize");
          var relSize = info.GetDouble("StartCapRelSize");
          s._startCap = LineCapBase.FromNameAndAbsAndRelSize(name, absSize, relSize);
        }

        if (0 != (cp & Configured.Transform))
        {
          info.GetArray(out var el);
          s._transformation = new Matrix3x2Class(el[0], el[1], el[2], el[3], el[4], el[5]);
        }

        if (0 != (cp & Configured.Width))
          s._width = info.GetDouble("Width");

        s._cachedHashCode = null;

        if (s._brush is null)
          throw new InvalidOperationException();

        s._cachedHashCode = null;
        return s;
      }
    }

    #endregion Version 3

    #region Version 4

    /// <summary>
    /// 2016-10-10: New in version 4: use Altaxo.Drawing.IDashPattern instead of all the dashpattern properties.
    /// 2020-03-30 Version 5: moved from Altaxo.Graph.Gdi namespace to Altaxo.Drawing namespace (Assembly: AltaxoBase)
    ///
    /// </summary>
    [Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.Gdi.PenX", 4)]
    private class XmlSerializationSurrogate4 : Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Serialization.Xml.IXmlSerializationInfo info)
      {
        throw new InvalidOperationException("Try to serialize old version!");
        /*
        var s = (PenX)obj;
        var cp = _GetConfiguredPropertiesVariable(s);

        info.AddValue("Configured", (int)cp);
        if (0 != (cp & Configured.PenType))
          info.AddEnum("Type", s._penType);
        if (0 != (cp & Configured.Alignment))
          info.AddEnum("Alignment", s.Alignment);
        if (0 != (cp & Configured.Brush))
          info.AddValue("Brush", s.Brush);
        if (0 != (cp & Configured.Color))
          info.AddValue("Color", s.Color);
        if (0 != (cp & Configured.CompoundArray))
          info.AddArray("CompoundArray", s.CompoundArray, s.CompoundArray.Length);

        // Note: we must even save the solid pattern if it belongs to another list than the BuiltinDefault list,
        // otherwise when deserializing we wouldn't know to which list the solid dash pattern belongs to.
        if (null != s._dashPattern && (!Solid.Instance.Equals(s._dashPattern) || !ReferenceEquals(DashPatternListManager.Instance.BuiltinDefault, DashPatternListManager.Instance.GetParentList(s._dashPattern))))
        {
          info.AddValue("DashPattern", s._dashPattern);
          if (0 != (cp & Configured.DashCap))
            info.AddEnum("DashCap", s.DashCap);
        }

        if (0 != (cp & Configured.EndCap))
        {
          info.AddValue("EndCap", s.EndCap.Name);
          info.AddValue("EndCapAbsSize", s._endCap.MinimumAbsoluteSizePt);
          info.AddValue("EndCapRelSize", s._endCap.MinimumRelativeSize);
        }
        if (0 != (cp & Configured.LineJoin))
          info.AddEnum("LineJoin", s.LineJoin);
        if (0 != (cp & Configured.MiterLimit))
          info.AddValue("MiterLimit", s.MiterLimit);
        if (0 != (cp & Configured.StartCap))
        {
          info.AddValue("StartCap", s.StartCap.Name);
          info.AddValue("StartCapAbsSize", s._startCap.MinimumAbsoluteSizePt);
          info.AddValue("StartCapRelSize", s._startCap.MinimumRelativeSize);
        }
        if (0 != (cp & Configured.Transform))
          info.AddArray("Transform", s.Transformation.Elements, s.Transformation.Elements.Length);
        if (0 != (cp & Configured.Width))
          info.AddValue("Width", s.Width);
          */
      }

      public object Deserialize(object o, Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        var s = null != o ? (PenX)o : new PenX();

        var cp = (Configured)info.GetInt32("Configured");

        if (0 != (cp & Configured.PenType))
          info.GetEnum("Type", typeof(PenType));

        if (0 != (cp & Configured.Alignment))
          s._alignment = (PenAlignment)info.GetEnum("Alignment", typeof(PenAlignment));

        if (0 != (cp & Configured.Brush))
        {
          s._brush = (BrushX)info.GetValue("Brush", s) ?? BrushesX.Black;
        }

        if (0 != (cp & Configured.Color))
        {
          var c = (NamedColor)info.GetValue("Color", s);
          s._brush = NamedColors.Black == c ? BrushesX.Black : new BrushX(c);
        }

        if (0 != (cp & Configured.CompoundArray))
          info.GetArray("CompoundArray", out s._compoundArray);

        if (info.CurrentElementName == "DashPattern")
        {
          s._dashPattern = (IDashPattern)info.GetValue("DashPattern", null);
          if (!Solid.Instance.Equals(s._dashPattern))
          {
            if (0 != (cp & Configured.DashCap))
              s._dashCap = (DashCap)info.GetEnum("DashCap", typeof(DashCap));
          }
        }

        if (0 != (cp & Configured.EndCap))
        {
          var name = info.GetString("EndCap");
          var absSize = info.GetDouble("EndCapAbsSize");
          var relSize = info.GetDouble("EndCapRelSize");
          s._endCap = LineCapBase.FromNameAndAbsAndRelSize(name, absSize, relSize);
        }

        if (0 != (cp & Configured.LineJoin))
          s._lineJoin = (LineJoin)info.GetEnum("LineJoin", typeof(LineJoin));

        if (0 != (cp & Configured.MiterLimit))
          s._miterLimit = info.GetSingle("MiterLimit");

        if (0 != (cp & Configured.StartCap))
        {
          var name = info.GetString("StartCap");
          var absSize = info.GetDouble("StartCapAbsSize");
          var relSize = info.GetDouble("StartCapRelSize");
          s._startCap = LineCapBase.FromNameAndAbsAndRelSize(name, absSize, relSize);
        }

        if (0 != (cp & Configured.Transform))
        {
          info.GetArray(out var el);
          s._transformation = new Matrix3x2Class(el[0], el[1], el[2], el[3], el[4], el[5]);
        }

        if (0 != (cp & Configured.Width))
          s._width = info.GetDouble("Width");

        s._cachedHashCode = null;
        return s;
      }
    }

    #endregion Version 4

    #region Version 5

    /// <summary>
    /// 2020-03-30 Version 5: moved from Altaxo.Graph.Gdi namespace to Altaxo.Drawing namespace (Assembly: AltaxoBase)
    /// no more Configured
    /// </summary>
    [Serialization.Xml.XmlSerializationSurrogateFor(typeof(PenX), 5)]
    private class XmlSerializationSurrogate5 : Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (PenX)obj;
        var cp = s.GetConfiguredProperties();
        info.AddValue("Configured", (int)cp);

        if (0 != (cp & Configured.Color))
          info.AddValue("Color", s.Color);
        else if (0 != (cp & Configured.Brush))
          info.AddValue("Brush", s.Brush);
        else
          throw new InvalidProgramException($"Either color or brush must be configured. Developer: Check function {nameof(s.GetConfiguredProperties)} !");

        if (0 != (cp & Configured.Width))
          info.AddValue("Width", s.Width);

        if (0 != (cp & Configured.Alignment))
          info.AddEnum("Alignment", s.Alignment);

        if (0 != (cp & Configured.LineJoin))
          info.AddEnum("LineJoin", s.LineJoin);

        if (0 != (cp & Configured.MiterLimit))
          info.AddValue("MiterLimit", s.MiterLimit);

        // Note: we must even save the solid pattern if it belongs to another list than the BuiltinDefault list,
        // otherwise when deserializing we wouldn't know to which list the solid dash pattern belongs to.
        if (null != s._dashPattern && (!Solid.Instance.Equals(s._dashPattern) || !ReferenceEquals(DashPatternListManager.Instance.BuiltinDefault, DashPatternListManager.Instance.GetParentList(s._dashPattern))))
        {
          info.AddValue("DashPattern", s._dashPattern);

        }

        if (0 != (cp & Configured.DashCap))
          info.AddEnum("DashCap", s.DashCap);

        if (0 != (cp & Configured.StartCap))
        {
          info.AddValue("StartCap", s.StartCap.Name);
          info.AddValue("StartCapAbsSize", s._startCap.MinimumAbsoluteSizePt);
          info.AddValue("StartCapRelSize", s._startCap.MinimumRelativeSize);
        }

        if (0 != (cp & Configured.EndCap))
        {
          info.AddValue("EndCap", s.EndCap.Name);
          info.AddValue("EndCapAbsSize", s._endCap.MinimumAbsoluteSizePt);
          info.AddValue("EndCapRelSize", s._endCap.MinimumRelativeSize);
        }

        if (0 != (cp & Configured.Transform))
          info.AddValue("Transformation", s.Transformation);

        if (0 != (cp & Configured.CompoundArray))
          info.AddArray("CompoundArray", s.CompoundArray, s.CompoundArray?.Length ?? 0);
      }

      public object Deserialize(object o, Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        var s = new PenX();
        var cp = (Configured)info.GetInt32("Configured");

        if (0 != (cp & Configured.Color))
        {
          var c = (NamedColor)info.GetValue("Color", s);
          s._brush = NamedColors.Black == c ? BrushesX.Black : new BrushX(c);
        }
        else if (0 != (cp & Configured.Brush))
        {
          s._brush = (BrushX)info.GetValue("Brush", s) ?? BrushesX.Black;
        }

        if (0 != (cp & Configured.Width))
          s._width = info.GetDouble("Width");

        if (0 != (cp & Configured.Alignment))
          s._alignment = (PenAlignment)info.GetEnum("Alignment", typeof(PenAlignment));

        if (0 != (cp & Configured.LineJoin))
          s._lineJoin = (LineJoin)info.GetEnum("LineJoin", typeof(LineJoin));

        if (0 != (cp & Configured.MiterLimit))
          s._miterLimit = info.GetSingle("MiterLimit");

        if (0 != (cp & Configured.DashPattern))
          s._dashPattern = (IDashPattern)info.GetValue("DashPattern", null);

        if (0 != (cp & Configured.DashCap))
          s._dashCap = (DashCap)info.GetEnum("DashCap", typeof(DashCap));

        if (0 != (cp & Configured.StartCap))
        {
          var name = info.GetString("StartCap");
          var absSize = info.GetDouble("StartCapAbsSize");
          var relSize = info.GetDouble("StartCapRelSize");
          s._startCap = LineCapBase.FromNameAndAbsAndRelSize(name, absSize, relSize);
        }

        if (0 != (cp & Configured.EndCap))
        {
          var name = info.GetString("EndCap");
          var absSize = info.GetDouble("EndCapAbsSize");
          var relSize = info.GetDouble("EndCapRelSize");
          s._endCap = LineCapBase.FromNameAndAbsAndRelSize(name, absSize, relSize);
        }

        if (0 != (cp & Configured.Transform))
        {
          info.GetArray(out var el);
          s._transformation = (Matrix3x2Class)info.GetValue("Transformation", null);
        }

        if (0 != (cp & Configured.CompoundArray))
          info.GetArray("CompoundArray", out s._compoundArray);

        s._cachedHashCode = null; // Invalidate hash code, because we changed a lot
        return s;
      }
    }

    #endregion Version 5

    #endregion Serialization

    #region Construction

    public PenX()
      : this(BrushesX.Black, DefaultWidth)
    {
    }

    public PenX(NamedColor c)
        : this(c, DefaultWidth)
    {
    }

    public PenX(NamedColor c, double width)
      : this(NamedColors.Black == c ? BrushesX.Black : new BrushX(c), width)
    {
    }

    public PenX(BrushX brush, double width)
    {
      _brush = brush ?? throw new ArgumentNullException(nameof(brush));
      _width = width;
      _alignment = DefaultAlignment;
      _lineJoin = DefaultLineJoin;
      _miterLimit = DefaultMiterLimit;
      _dashPattern = DashPatternListManager.Instance.BuiltinDefaultSolid;
      _startCap = FlatCap.Instance;
      _endCap = FlatCap.Instance;
    }

    #endregion

    #region Properties

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
        return _brush.IsVisible;
      }
    }

    /// <summary>
    /// Determines if this pen is invisible. It is invisible if it has an invisible brush or the color is transparent.
    /// </summary>
    public bool IsInvisible
    {
      get
      {
        return _brush.IsInvisible;
      }
    }

    public BrushX Brush
    {
      get
      {
        return _brush;
      }
    }

    public PenX WithBrush(BrushX value)
    {
      if (value is null)
        throw new ArgumentNullException(nameof(value));

      if (!Equals(_brush, value))
      {
        var result = Clone();
        result._brush = value;
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
        return _brush.Color;
      }
    }

    public PenX WithColor(NamedColor value)
    {
      if (!(Color == value))
      {
        var result = Clone();
        result._brush = result._brush.WithColor(value);
        return result;
      }
      else
      {
        return this;
      }
    }

    public double[]? CompoundArray
    {
      get { return _compoundArray; }
    }

    public PenX WithCompoundArray(double[]? value)
    {
      if (!(_compoundArray == value))
      {
        var result = Clone();
        result._compoundArray = value is null ? null : (double[])value.Clone();

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
        return result;
      }
      else
      {
        return this;
      }
    }

    public ILineCap StartCap
    {
      get
      {
        return _startCap;
      }
    }

    public PenX WithStartCap(ILineCap value)
    {
      if (value is null)
        value = FlatCap.Instance;

      if (!Equals(_startCap, value))
      {
        var result = Clone();
        result._startCap = value;
        return result;
      }
      else
      {
        return this;
      }
    }

    public ILineCap EndCap
    {
      get
      {
        return _endCap;
      }
    }

    public PenX WithEndCap(ILineCap value)
    {
      if (value is null)
        value = FlatCap.Instance;

      if (!Equals(_endCap, value))
      {
        var result = Clone();
        result._endCap = value;
        return result;
      }
      else
      {
        return this;
      }
    }

    public IDashPattern DashPattern
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

      if (!ReferenceEquals(_dashPattern, value)) // use ReferenceEquals because the reference determines to which DashPatternList the DashPattern belongs
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
        return result;
      }
      else
      {
        return this;
      }
    }

    public Matrix3x2Class? Transformation
    {
      get { return _transformation; }
    }

    public PenX WithTransformation(Matrix3x2Class? value)
    {
      if (value is null || value.Matrix.IsIdentity)
        value = null; // we do not store the matrix if it is the identity matrix

      if (!Equals(_transformation, value))
      {
        var result = Clone();
        result._transformation = value;
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
        return result;
      }
      else
      {
        return this;
      }
    }

    #endregion

    #region Equality and HashCode

    private bool Equals(PenX? other, bool penWidthMattersHere)
    {
      if (other is null)
        return false;
      if (ReferenceEquals(this, other))
        return true;

      if (!(Equals(_brush, other._brush)))
        return false;

      if (!(_width == other._width) && penWidthMattersHere)
        return false;

      if (!(_alignment == other._alignment))
        return false;

      if (!(_lineJoin == other._lineJoin))
        return false;

      if (!(_miterLimit == other._miterLimit))
        return false;

      if (!(Equals(_startCap, other._startCap)))
        return false;

      if (!(Equals(_endCap, other._endCap)))
        return false;

      if (!(object.ReferenceEquals(_dashPattern, other._dashPattern))) // we need reference equality for dashpattern since this determines the membership in a dash pattern group
        return false;

      if (!(_dashPattern is Solid))
      {
        if (!(_dashCap == other._dashCap))
          return false;
      }

      if (!AreEqual(_compoundArray, other._compoundArray))
        return false;

      if (!Equals(_transformation, other._transformation))
        return false;

      return true;
    }

    public bool Equals(PenX? other)
    {
      return Equals(other, true);
    }

    public override bool Equals(object? obj)
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
      var result = _brush.GetHashCode();

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

        if (!(_dashPattern is Solid))
          result += 29 * _dashCap.GetHashCode();

        if (_compoundArray is { } ca)
          result += 31 * GetHashCode(ca);

        if (_transformation is { } t)
          result += 37 * t.GetHashCode();
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


    private static bool AreEqual(double[]? x1, double[]? x2)
    {
      if (x1 == null && x2 == null)
        return true;
      if (x1 == null || x2 == null)
        return false;
      if (x1.Length != x2.Length)
        return false;
      for (var i = 0; i < x1.Length; i++)
        if (x1[i] != x2[i])
          return false;

      return true;
    }

    private int GetHashCode(double[] x)
    {
      return x is null || x.Length == 0
        ? 0
        :
        x.Length.GetHashCode() +
        5 * x[0].GetHashCode() +
        7 * x[x.Length - 1].GetHashCode();
    }



    private PenX Clone()
    {
      var result = (PenX)MemberwiseClone();
      result._cachedHashCode = null;
      return result;
    }

    #endregion

    #region Helpers (old deserialization)

    /// <summary>
    /// Sets the <see cref="_dashPattern"/> member after deserialization of old versions (before 2016-10-10).
    /// </summary>
    private void SetDashPatternFromCachedDashPropertiesAfterOldDeserialization(Configured _configuredProperties, DashStyle cachedDashStyle, float[]? cachedDashPattern, float cachedDashOffset)
    {
      _dashPattern = !_configuredProperties.HasFlag(Configured.DashStyle)
        ? DashPatternListManager.Instance.BuiltinDefaultSolid
        : (cachedDashStyle switch
        {
          DashStyle.Solid => DashPatternListManager.Instance.BuiltinDefaultSolid,
          DashStyle.Dash => DashPatternListManager.Instance.BuiltinDefaultDash,
          DashStyle.Dot => DashPatternListManager.Instance.BuiltinDefaultDot,
          DashStyle.DashDot => DashPatternListManager.Instance.BuiltinDefaultDashDot,
          DashStyle.DashDotDot => DashPatternListManager.Instance.BuiltinDefaultDashDotDot,
          DashStyle.Custom => new Custom(cachedDashPattern.Select(x => (double)x), cachedDashOffset),
          _ => throw new NotImplementedException(),
        });
    }

    #endregion

  } // end of class PenX
}
