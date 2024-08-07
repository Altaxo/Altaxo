﻿#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2018 Dr. Dirk Lellinger
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Altaxo.Drawing;
using Altaxo.Graph.Graph2D.Plot.Groups;
using Altaxo.Graph.Graph2D.Plot.Styles;
using Altaxo.Graph.Graph2D.Plot.Styles.ScatterSymbols;

namespace Altaxo.Graph.Gdi.Plot.Styles
{
  public partial class ScatterPlotStyle
  :
  Main.SuspendableDocumentNodeWithEventArgs,
  IG2DPlotStyle
  {
    #region Code for Legacy deserialion

    private static IEnumerable<CSPlaneID> GetCSPlaneIds(DeprecatedDropLine dropLine)
    {
      if (0 != (dropLine & DeprecatedDropLine.Bottom))
        yield return CSPlaneID.Bottom;
      if (0 != (dropLine & DeprecatedDropLine.Top))
        yield return CSPlaneID.Top;
      if (0 != (dropLine & DeprecatedDropLine.Left))
        yield return CSPlaneID.Left;
      if (0 != (dropLine & DeprecatedDropLine.Right))
        yield return CSPlaneID.Right;
    }

    private static IScatterSymbol GetScatterSymbol(DeprecatedShape shape, DeprecatedStyle style)
    {
      var lm = ScatterSymbolListManager.Instance;
      ScatterSymbolList list;
      switch (style)
      {
        case DeprecatedStyle.Solid:
          list = lm.OldSolid;
          break;

        case DeprecatedStyle.Open:
          list = lm.OldOpen;
          break;

        case DeprecatedStyle.DotCenter:
          list = lm.OldDotCenter;
          break;

        case DeprecatedStyle.Hollow:
          list = lm.OldHollow;
          break;

        case DeprecatedStyle.Plus:
          list = lm.OldPlus;
          break;

        case DeprecatedStyle.Times:
          list = lm.OldTimes;
          break;

        case DeprecatedStyle.BarHorz:
          list = lm.OldBarHorz;
          break;

        case DeprecatedStyle.BarVert:
          list = lm.OldBarVert;
          break;

        default:
          throw new NotImplementedException();
      }

      int iShape = (int)shape;
      if (0 == iShape)
        return new NoSymbol();
      else if (iShape <= 10)
        return list[iShape - 1]; // -1 because in those lists NoSymbol is not included
      else
        throw new NotImplementedException();
    }

    #endregion Code for Legacy deserialion

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.XYPlotScatterStyle", 0)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.XYPlotScatterStyle", 1)] // by accident this was never different from 0
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        throw new NotSupportedException("Serialization of old versions is not supported, probably a programming error");
        /*
                XYPlotScatterStyle s = (XYPlotScatterStyle)obj;
                info.AddValue("Shape", s._shape);
                info.AddValue("Style", s._style);
                info.AddValue("DropLine", s._dropLine);
                info.AddValue("Pen", s._pen);
                info.AddValue("SymbolSize", s._symbolSize);
                info.AddValue("RelativePenWidth", s._relativePenWidth);
                */
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        const double Sqrt2 = 1.4142135623730950488016887242097;
        var s = (ScatterPlotStyle?)o ?? new ScatterPlotStyle(info);

        var shape = (DeprecatedShape)info.GetValue("Shape", typeof(DeprecatedShape));
        var style = (DeprecatedStyle)info.GetValue("Style", typeof(DeprecatedStyle));
        s._scatterSymbol = GetScatterSymbol(shape, style);

        var dropLine = (DeprecatedDropLine)info.GetValue("DropLine", s);
        var pen = (PenX)info.GetValue("Pen", s);
        s._color = pen.Color;
        s._symbolSize = info.GetSingle("SymbolSize");
        s._overrideStructureWidthFactor = info.GetSingle("RelativePenWidth");
        s._symbolSize *= Sqrt2 * (1 + s._overrideStructureWidthFactor.Value);

        if (dropLine != DeprecatedDropLine.NoDrop)
        {
          return new object[] { s, new DropLinePlotStyle(GetCSPlaneIds(dropLine), pen) };
        }
        else
        {
          return s;
        }
      }
    }

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.XYPlotScatterStyle", 2)]
    private class XmlSerializationSurrogate2 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        throw new NotSupportedException("Serialization of old versions is not supported, probably a programming error");
        /*
                base.Serialize(obj, info); // Base was formerly Surrogate0
                XYPlotScatterStyle s = (XYPlotScatterStyle)obj;
                info.AddValue("IndependentColor", s._independentColor);
                info.AddValue("IndependentSymbolSize", s._independentSymbolSize);
                info.AddValue("SkipFreq", s._skipFreq);
                */
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        const double Sqrt2 = 1.4142135623730950488016887242097;

        var s = (ScatterPlotStyle?)o ?? new ScatterPlotStyle(info);

        var shape = (DeprecatedShape)info.GetValue("Shape", typeof(DeprecatedShape));
        var style = (DeprecatedStyle)info.GetValue("Style", typeof(DeprecatedStyle));
        s._scatterSymbol = GetScatterSymbol(shape, style);

        var dropLine = (DeprecatedDropLine)info.GetValue("DropLine", s);
        var pen = (PenX)info.GetValue("Pen", s);
        s._color = pen.Color;
        s._symbolSize = info.GetSingle("SymbolSize");
        s._overrideStructureWidthFactor = info.GetSingle("RelativePenWidth");
        s._symbolSize *= Sqrt2 * (1 + s._overrideStructureWidthFactor.Value);

        s._independentColor = info.GetBoolean("IndependentColor");
        s._independentSymbolSize = info.GetBoolean("IndependentSymbolSize");
        s._skipFreq = info.GetInt32("SkipFreq");

        if (dropLine != DeprecatedDropLine.NoDrop)
        {
          return new object[] { s, new DropLinePlotStyle(GetCSPlaneIds(dropLine), pen) };
        }
        else
        {
          return s;
        }
      }
    }

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.Gdi.Plot.Styles.ScatterPlotStyle", 3)]
    private class XmlSerializationSurrogate3 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        throw new NotSupportedException("Serialization of old versions is not supported, probably a programming error");
        /*
                ScatterPlotStyle s = (ScatterPlotStyle)obj;
                info.AddValue("Shape", s._shape);
                info.AddValue("Style", s._style);
                info.AddValue("DropLine", s._dropLine);
                info.AddValue("Pen", s._pen);
                info.AddValue("SymbolSize", s._symbolSize);
                info.AddValue("RelativePenWidth", s._relativePenWidth);

                info.AddValue("IndependentColor", s._independentColor);
                info.AddValue("IndependentSymbolSize", s._independentSymbolSize);
                info.AddValue("SkipFreq", s._skipFreq);
                */
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        const double Sqrt2 = 1.4142135623730950488016887242097;

        var s = (ScatterPlotStyle?)o ?? new ScatterPlotStyle(info);

        var shape = (DeprecatedShape)info.GetValue("Shape", s);
        var style = (DeprecatedStyle)info.GetValue("Style", s);
        s._scatterSymbol = GetScatterSymbol(shape, style);

        var dropLineTargets = (CSPlaneIDList)info.GetValue("DropLine", s);
        var pen = (PenX)info.GetValue("Pen", s);
        s._color = pen.Color;
        s._symbolSize = info.GetSingle("SymbolSize");
        s._overrideStructureWidthFactor = info.GetSingle("RelativePenWidth");
        s._symbolSize *= Sqrt2 * (1 + s._overrideStructureWidthFactor.Value);
        s._independentColor = info.GetBoolean("IndependentColor");
        s._independentSymbolSize = info.GetBoolean("IndependentSymbolSize");
        s._skipFreq = info.GetInt32("SkipFreq");

        if (dropLineTargets is not null && dropLineTargets.Count != 0)
        {
          return new object[] { s, new DropLinePlotStyle(dropLineTargets, pen) };
        }
        else
        {
          return s;
        }
      }
    }

    /// <summary>
    /// 2016-11-04 Separation of ScatterPlotStyle and DropLinePlotStyle. ScatterSymbol is now a class. Many new properties.
    /// </summary>
    /// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ScatterPlotStyle), 4)]
    private class XmlSerializationSurrogate4 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (ScatterPlotStyle)obj;

        info.AddValue("IndependentSkipFreq", s._independentSkipFreq);
        info.AddValue("SkipFreq", s._skipFreq);
        info.AddValue("IgnoreMissingDataPoints", s._ignoreMissingDataPoints);
        info.AddValue("IndependentOnShiftingGroupStyles", s._independentOnShiftingGroupStyles);

        info.AddValue("IndependentScatterSymbol", s._independentScatterSymbol);
        info.AddValue("ScatterSymbol", s._scatterSymbol);
        info.AddValue("IndependentSymbolSize", s._independentSymbolSize);
        info.AddValue("SymbolSize", s._symbolSize);

        info.AddValue("IndependentColor", s._independentColor);
        info.AddValue("Color", s._color);

        info.AddValue("OverrideFrame", s._overrideFrame);
        info.AddValueOrNull("OverriddenFrame", s._overriddenFrame);
        info.AddValue("OverrideInset", s._overrideInset);
        info.AddValueOrNull("OverriddenInset", s._overriddenInset);
        info.AddValue("OverriddenStructureWidthOffset", s._overrideStructureWidthOffset);
        info.AddValue("OverriddenStructureWidthFactor", s._overrideStructureWidthFactor);
        info.AddNullableEnum("OverriddenPlotColorInfluence", s._overridePlotColorInfluence);
        info.AddValueOrNull("OverriddenFillColor", s._overrideFillColor);
        info.AddValueOrNull("OverriddenFrameColor", s._overrideFrameColor);
        info.AddValueOrNull("OverriddenInsetColor", s._overrideInsetColor);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (ScatterPlotStyle?)o ?? new ScatterPlotStyle(info);

        s._independentSkipFreq = info.GetBoolean("IndependentSkipFreq");
        s._skipFreq = info.GetInt32("SkipFreq");

        s._ignoreMissingDataPoints = info.GetBoolean("IgnoreMissingDataPoints");
        s._independentOnShiftingGroupStyles = info.GetBoolean("IndependentOnShiftingGroupStyles");

        s._independentScatterSymbol = info.GetBoolean("IndependentScatterSymbol");
        s._scatterSymbol = (IScatterSymbol)info.GetValue("ScatterSymbol", s);

        s._independentSymbolSize = info.GetBoolean("IndependentSymbolSize");
        s._symbolSize = info.GetSingle("SymbolSize");
        s._independentColor = info.GetBoolean("IndependentColor");
        s._color = (NamedColor)info.GetValue("Color", s);

        s._overrideFrame = info.GetBoolean("OverrideFrame");
        s._overriddenFrame = info.GetValueOrNull<IScatterSymbolFrame>("OverriddenFrame", s);
        s._overrideInset = info.GetBoolean("OverrideInset");
        s._overriddenInset = info.GetValueOrNull<IScatterSymbolInset>("OverriddenInset", s);
        s._overrideStructureWidthOffset = info.GetNullableDouble("OverriddenStructureWidthOffset");
        s._overrideStructureWidthFactor = info.GetNullableDouble("OverriddenStructureWidthFactor");
        s._overridePlotColorInfluence = info.GetNullableEnum<PlotColorInfluence>("OverriddenPlotColorInfluence");
        s._overrideFillColor = info.GetNullableStruct<NamedColor>("OverriddenFillColor", s);
        s._overrideFrameColor = info.GetNullableStruct<NamedColor>("OverriddenFrameColor", s);
        s._overrideInsetColor = info.GetNullableStruct<NamedColor>("OverriddenInsetColor", s);

        return s;
      }
    }

    #region Legacy classes

    #region Shape

    private enum DeprecatedShape
    {
      NoSymbol,
      Square,
      Circle,
      UpTriangle,
      DownTriangle,
      Diamond,
      CrossPlus,
      CrossTimes,
      Star,
      BarHorz,
      BarVert
    }

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.XYPlotScatterStyles.Shape", 0)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.Gdi.Plot.Styles.XYPlotScatterStyles.Shape", 1)]
    public class ShapeXmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        throw new InvalidOperationException("Serialization of old versions not allowed");
        // info.SetNodeContent(obj.ToString());
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        string val = info.GetNodeContent();
        return System.Enum.Parse(typeof(DeprecatedShape), val, true);
      }
    }

    #endregion Shape

    #region Style

    [Serializable]
    private enum DeprecatedStyle
    {
      Solid,
      Open,
      DotCenter,
      Hollow,
      Plus,
      Times,
      BarHorz,
      BarVert
    }

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.XYPlotScatterStyles.Style", 0)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.Gdi.Plot.Styles.XYPlotScatterStyles.Style", 1)]
    public class StyleXmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        throw new InvalidOperationException("Serialization of old versions not allowed");
        //info.SetNodeContent(obj.ToString());
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        string val = info.GetNodeContent();
        return System.Enum.Parse(typeof(DeprecatedStyle), val, true);
      }
    }

    #endregion Style

    #region Drop

    [Flags]
    [Serializable]
    private enum DeprecatedDropLine
    {
      NoDrop = 0,
      Top = 1,
      Bottom = 2,
      Left = 4,
      Right = 8,
      All = Top | Bottom | Left | Right
    }

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.XYPlotScatterStyles.DropLine", 0)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.Gdi.Plot.Styles.XYPlotScatterStyles.DropLine", 1)]
    public class DropLineXmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        throw new InvalidOperationException("Serialization of old versions not allowed");
        // info.SetNodeContent(obj.ToString());
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        string val = info.GetNodeContent();
        return System.Enum.Parse(typeof(DeprecatedDropLine), val, true);
      }
    }

    #endregion Drop

    #endregion Legacy classes
  }

  #endregion Serialization
}
