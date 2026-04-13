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
using Altaxo.Drawing;
using Clipper2Lib;

namespace Altaxo.Graph.Graph2D.Plot.Styles.ScatterSymbols
{
  /// <summary>
  /// Represents the absence of a scatter symbol.
  /// </summary>
  public class NoSymbol : SymbolBase, IScatterSymbol
  {
    #region Serialization

    /// <summary>
    /// 2016-10-27 initial version.
    /// </summary>
    /// <summary>
    /// Serializes <see cref="NoSymbol"/> instances.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(NoSymbol), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public void Serialize(object o, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        SerializeSetV0((IScatterSymbol)o, info);
      }

      /// <inheritdoc/>
      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (NoSymbol?)o ?? new NoSymbol();
        return DeserializeSetV0(s, info, parent);
      }
    }

    #endregion Serialization

    /// <inheritdoc/>
    public double DesignSize { get { return ClipperSymbolSize; } }

    /// <inheritdoc/>
    public double RelativeStructureWidth { get { return 0.09375; } }

    /// <inheritdoc/>
    IScatterSymbol IScatterSymbol.WithRelativeStructureWidth(double relativeStructureWidth)
    {
      return this;
    }

    /// <inheritdoc/>
    public NamedColor FillColor
    {
      get
      {
        return NamedColors.Black;
      }
    }

    /// <inheritdoc/>
    IScatterSymbol IScatterSymbol.WithFillColor(NamedColor fillColor)
    {
      return this;
    }

    /// <inheritdoc/>
    public IScatterSymbolFrame? Frame
    {
      get
      {
        return null;
      }
    }

    /// <inheritdoc/>
    IScatterSymbol IScatterSymbol.WithFrame(IScatterSymbolFrame? frame)
    {
      return this;
    }

    /// <inheritdoc/>
    public IScatterSymbolInset? Inset
    {
      get
      {
        return null;
      }
    }

    /// <inheritdoc/>
    IScatterSymbol IScatterSymbol.WithInset(IScatterSymbolInset? inset)
    {
      return this;
    }

    /// <inheritdoc/>
    public PlotColorInfluence PlotColorInfluence
    {
      get
      {
        return PlotColorInfluence.FillColorFull;
      }
    }

    /// <inheritdoc/>
    IScatterSymbol IScatterSymbol.WithPlotColorInfluence(PlotColorInfluence plotColorInfluence)
    {
      return this;
    }

    /// <inheritdoc/>
    public void CalculatePolygons(double? relativeStructureWidth, out Paths64? framePolygon, out Paths64? insetPolygon, out Paths64? fillPolygon)
    {
      framePolygon = null;
      insetPolygon = null;
      fillPolygon = null;
    }

    /// <inheritdoc/>
    public object Clone()
    {
      return MemberwiseClone();
    }
  }
}
