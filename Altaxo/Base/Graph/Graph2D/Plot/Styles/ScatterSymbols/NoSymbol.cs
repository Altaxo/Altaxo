﻿#region Copyright

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
using Altaxo.Drawing;
using ClipperLib;

namespace Altaxo.Graph.Graph2D.Plot.Styles.ScatterSymbols
{
  public class NoSymbol : SymbolBase, IScatterSymbol
  {
    #region Serialization

    /// <summary>
    /// 2016-10-27 initial version.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(NoSymbol), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        SerializeSetV0((IScatterSymbol)obj, info);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (NoSymbol?)o ?? new NoSymbol();
        return DeserializeSetV0(s, info, parent);
      }
    }

    #endregion Serialization

    public double DesignSize { get { return ClipperSymbolSize; } }

    public double RelativeStructureWidth { get { return 0.09375; } }

    IScatterSymbol IScatterSymbol.WithRelativeStructureWidth(double relativeStructureWidth)
    {
      return this;
    }

    public NamedColor FillColor
    {
      get
      {
        return NamedColors.Black;
      }
    }

    IScatterSymbol IScatterSymbol.WithFillColor(NamedColor fillColor)
    {
      return this;
    }

    public IScatterSymbolFrame? Frame
    {
      get
      {
        return null;
      }
    }

    IScatterSymbol IScatterSymbol.WithFrame(IScatterSymbolFrame? frame)
    {
      return this;
    }

    public IScatterSymbolInset? Inset
    {
      get
      {
        return null;
      }
    }

    IScatterSymbol IScatterSymbol.WithInset(IScatterSymbolInset? inset)
    {
      return this;
    }

    public PlotColorInfluence PlotColorInfluence
    {
      get
      {
        return PlotColorInfluence.FillColorFull;
      }
    }

    IScatterSymbol IScatterSymbol.WithPlotColorInfluence(PlotColorInfluence plotColorInfluence)
    {
      return this;
    }

    public void CalculatePolygons(double? relativeStructureWidth, out List<List<IntPoint>>? framePolygon, out List<List<IntPoint>>? insetPolygon, out List<List<IntPoint>>? fillPolygon)
    {
      framePolygon = null;
      insetPolygon = null;
      fillPolygon = null;
    }

    public object Clone()
    {
      return MemberwiseClone();
    }
  }
}
