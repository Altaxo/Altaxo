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
using Altaxo.Drawing;
using Clipper2Lib;

namespace Altaxo.Graph.Graph2D.Plot.Styles.ScatterSymbols
{
  /// <summary>
  /// Provides the base implementation for open scatter symbols.
  /// </summary>
  public abstract class OpenSymbolBase : SymbolBase, IScatterSymbol
  {
    /// <summary>
    /// The fill color.
    /// </summary>
    protected NamedColor _fillColor = NamedColors.Black;

    /// <summary>
    /// The relative structure width.
    /// </summary>
    protected double _relativeStructureWidth = 0.09375;

    #region Serialization

    /// <summary>
    /// Serializes <see cref="OpenSymbolBase"/> state.
    /// 2016-10-27 initial version.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(OpenSymbolBase), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public void Serialize(object o, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (OpenSymbolBase)o;
        info.AddValue("StructureScale", s._relativeStructureWidth);
        info.AddValue("Fill", s._fillColor);
      }

      /// <inheritdoc/>
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
    /// <param name="relativeStructureWidth">The relative structure width.</param>
    /// <returns>Polygon(s) of the outer symbol shape.</returns>
    public abstract Paths64 GetCopyOfOuterPolygon(double relativeStructureWidth);

    /// <summary>
    /// Initializes a new instance of the <see cref="OpenSymbolBase"/> class.
    /// </summary>
    protected OpenSymbolBase()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="OpenSymbolBase"/> class.
    /// </summary>
    /// <param name="fillColor">The fill color.</param>
    /// <param name="isFillColorInfluencedByPlotColor">Indicates whether the fill color is influenced by the plot color.</param>
    protected OpenSymbolBase(NamedColor fillColor, bool isFillColorInfluencedByPlotColor)
    {
      _fillColor = fillColor;
    }

    /// <inheritdoc/>
    public object Clone()
    {
      return MemberwiseClone();
    }

    /// <inheritdoc/>
    public double DesignSize { get { return ClipperSymbolSize; } }

    /// <inheritdoc/>
    public NamedColor FillColor { get { return _fillColor; } }

    /// <summary>
    /// Returns a copy of this symbol with the specified fill color.
    /// </summary>
    /// <param name="value">The fill color.</param>
    /// <returns>The current instance if no change is required; otherwise, a cloned instance with the updated fill color.</returns>
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

    /// <inheritdoc/>
    IScatterSymbol IScatterSymbol.WithFillColor(NamedColor fillColor)
    {
      return WithFillColor(fillColor);
    }

    /// <inheritdoc/>
    public PlotColorInfluence PlotColorInfluence { get { return PlotColorInfluence.FillColorFull; } }

    /// <inheritdoc/>
    IScatterSymbol IScatterSymbol.WithPlotColorInfluence(PlotColorInfluence plotColorInfluence)
    {
      return this;
    }

    /// <inheritdoc/>
    public double RelativeStructureWidth { get { return _relativeStructureWidth; } }

    /// <summary>
    /// Returns a copy of this symbol with the specified relative structure width.
    /// </summary>
    /// <param name="value">The relative width of internal structures.</param>
    /// <returns>The current instance if no change is required; otherwise, a cloned instance with the updated width.</returns>
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

    /// <inheritdoc/>
    IScatterSymbol IScatterSymbol.WithRelativeStructureWidth(double relativeStructureWidth)
    {
      return WithRelativeStructureWidth(relativeStructureWidth);
    }

    /// <inheritdoc/>
    public IScatterSymbolFrame? Frame
    {
      get { return null; }
    }

    /// <inheritdoc/>
    IScatterSymbol IScatterSymbol.WithFrame(IScatterSymbolFrame? frame)
    {
      return this;
    }

    /// <inheritdoc/>
    public IScatterSymbolInset? Inset { get { return null; } }

    /// <inheritdoc/>
    IScatterSymbol IScatterSymbol.WithInset(IScatterSymbolInset? inset)
    {
      return this;
    }

    /// <inheritdoc/>
    public void CalculatePolygons(
      double? relativeStructureWidth,
      out Paths64? framePolygon,
      out Paths64? insetPolygon,
      out Paths64? fillPolygon)

    {
      insetPolygon = null;
      framePolygon = null;
      fillPolygon = GetCopyOfOuterPolygon(relativeStructureWidth ?? _relativeStructureWidth);
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
      if (!(GetType() == obj?.GetType()))
        return false;

      var from = (OpenSymbolBase)obj;

      return
        _relativeStructureWidth == from._relativeStructureWidth &&
        _fillColor == from._fillColor;
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
      return
        GetType().GetHashCode() +
        _relativeStructureWidth.GetHashCode() +
        _fillColor.GetHashCode();
    }
  }
}
