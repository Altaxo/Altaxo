#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2016 Dr. Dirk Lellinger
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
using Altaxo.Collections;
using Altaxo.Drawing;
using Altaxo.Graph.Graph2D.Plot.Groups;

namespace Altaxo.Graph.Graph2D.Plot.Styles.ScatterSymbols
{
  public abstract class OpenSymbolBase : SymbolBase, IScatterSymbol
  {
    protected NamedColor _fillColor = NamedColors.Black;
    protected double _relativeStructureWidth = 0.09375;

    #region Serialization

    /// <summary>
    /// 2016-10-27 initial version.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(OpenSymbolBase), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (OpenSymbolBase)obj;
        info.AddValue("StructureScale", s._relativeStructureWidth);
        info.AddValue("Fill", s._fillColor);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (OpenSymbolBase)(o ?? throw new ArgumentNullException(nameof(o)));
        s._relativeStructureWidth = info.GetDouble("StructureScale");
        s._fillColor = (NamedColor)info.GetValue("Fill", null);

        return s;
      }
    }

    #endregion Serialization

    /// <summary>
    /// Gets a copy of the outer symbol shape as polygon(s).
    /// </summary>
    /// <returns>Polygon(s) of the outer symbol shape.</returns>
    public abstract List<List<ClipperLib.IntPoint>> GetCopyOfOuterPolygon(double relativeStructureWidth);

    protected OpenSymbolBase()
    {
    }

    protected OpenSymbolBase(NamedColor fillColor, bool isFillColorInfluencedByPlotColor)
    {
      _fillColor = fillColor;
    }

    public object Clone()
    {
      return MemberwiseClone();
    }

    public double DesignSize { get { return ClipperSymbolSize; } }

    public NamedColor FillColor { get { return _fillColor; } }

    public OpenSymbolBase WithFillColor(NamedColor value)
    {
      if (_fillColor == value)
      {
        return this;
      }
      else
      {
        var result = (OpenSymbolBase)MemberwiseClone();
        result._fillColor = value;
        return result;
      }
    }

    IScatterSymbol IScatterSymbol.WithFillColor(NamedColor fillColor)
    {
      return WithFillColor(fillColor);
    }

    public PlotColorInfluence PlotColorInfluence { get { return PlotColorInfluence.FillColorFull; } }

    IScatterSymbol IScatterSymbol.WithPlotColorInfluence(PlotColorInfluence plotColorInfluence)
    {
      return this;
    }

    public double RelativeStructureWidth { get { return _relativeStructureWidth; } }

    public OpenSymbolBase WithRelativeStructureWidth(double value)
    {
      if (!(value >= 0) || !(value < 0.5))
        throw new ArgumentOutOfRangeException(nameof(value), "Provided value must be >=0 and <0.5");

      if (_relativeStructureWidth == value)
      {
        return this;
      }
      else
      {
        {
          var result = (OpenSymbolBase)MemberwiseClone();
          result._relativeStructureWidth = value;
          return result;
        }
      }
    }

    IScatterSymbol IScatterSymbol.WithRelativeStructureWidth(double relativeStructureWidth)
    {
      return WithRelativeStructureWidth(relativeStructureWidth);
    }

    public IScatterSymbolFrame? Frame
    {
      get { return null; }
    }

    IScatterSymbol IScatterSymbol.WithFrame(IScatterSymbolFrame? frame)
    {
      return this;
    }

    public IScatterSymbolInset? Inset { get { return null; } }

    IScatterSymbol IScatterSymbol.WithInset(IScatterSymbolInset? inset)
    {
      return this;
    }

    public void CalculatePolygons(
      double? relativeStructureWidth,
      out List<List<ClipperLib.IntPoint>>? framePolygon,
      out List<List<ClipperLib.IntPoint>>? insetPolygon,
      out List<List<ClipperLib.IntPoint>>? fillPolygon)

    {
      insetPolygon = null;
      framePolygon = null;
      fillPolygon = GetCopyOfOuterPolygon(relativeStructureWidth ?? _relativeStructureWidth);
    }

    public override bool Equals(object? obj)
    {
      if (!(GetType() == obj?.GetType()))
        return false;

      var from = (OpenSymbolBase)obj;

      return
        _relativeStructureWidth == from._relativeStructureWidth &&
        _fillColor == from._fillColor;
    }

    public override int GetHashCode()
    {
      return
        GetType().GetHashCode() +
        _relativeStructureWidth.GetHashCode() +
        _fillColor.GetHashCode();
    }
  }
}
