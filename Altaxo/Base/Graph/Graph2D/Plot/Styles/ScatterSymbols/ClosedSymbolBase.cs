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
using Altaxo.Collections;
using Altaxo.Drawing;
using Clipper2Lib;

namespace Altaxo.Graph.Graph2D.Plot.Styles.ScatterSymbols
{
  /// <summary>
  /// Provides the base implementation for closed scatter symbols.
  /// </summary>
  public abstract class ClosedSymbolBase : SymbolBase, IScatterSymbol
  {
    private PlotColorInfluence _plotColorInfluence = PlotColorInfluence.FillColorFull;
    /// <summary>
    /// The fill color of the symbol.
    /// </summary>
    protected NamedColor _fillColor = NamedColors.Black;

    /// <summary>
    /// The relative width of the symbol structure.
    /// </summary>
    protected double _relativeStructureWidth = 0.09375;

    /// <summary>
    /// The optional symbol frame.
    /// </summary>
    protected IScatterSymbolFrame? _frame;

    /// <summary>
    /// The optional symbol inset.
    /// </summary>
    protected IScatterSymbolInset? _inset;

    #region Serialization

    /// <summary>
    /// 2016-10-27 initial version.
    /// </summary>
    /// <summary>
    /// Serializes <see cref="ClosedSymbolBase"/> state.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ClosedSymbolBase), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (ClosedSymbolBase)obj;
        info.AddEnum("PlotColorInfluence", s._plotColorInfluence);
        info.AddValue("StructureScale", s._relativeStructureWidth);
        info.AddValue("Fill", s._fillColor);
        info.AddValueOrNull("Frame", s._frame);
        info.AddValueOrNull("Inset", s._inset);
      }

      /// <inheritdoc/>
      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (ClosedSymbolBase)(o ?? throw new ArgumentNullException(nameof(o)));
        s._plotColorInfluence = (PlotColorInfluence)info.GetEnum("PlotColorInfluence", typeof(PlotColorInfluence));
        s._relativeStructureWidth = info.GetDouble("StructureScale");
        s._fillColor = (NamedColor)info.GetValue("Fill", null);
        s._frame = info.GetValueOrNull<IScatterSymbolFrame>("Frame", null);
        s._inset = info.GetValueOrNull<IScatterSymbolInset>("Inset", null);

        return s;
      }
    }

    #endregion Serialization

    /// <summary>
    /// Gets a copy of the outer symbol shape as polygon(s).
    /// </summary>
    /// <returns>Polygon(s) of the outer symbol shape.</returns>
    public abstract Paths64 GetCopyOfOuterPolygon();

    /// <summary>
    /// Initializes a new instance of the <see cref="ClosedSymbolBase"/> class.
    /// </summary>
    protected ClosedSymbolBase()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ClosedSymbolBase"/> class.
    /// </summary>
    /// <param name="fillColor">The fill color.</param>
    /// <param name="isFillColorInfluencedByPlotColor">If set to <c>true</c>, the fill color is influenced by the plot color.</param>
    protected ClosedSymbolBase(NamedColor fillColor, bool isFillColorInfluencedByPlotColor)
    {
      _fillColor = fillColor;
      _plotColorInfluence = isFillColorInfluencedByPlotColor ? PlotColorInfluence.FillColorFull : PlotColorInfluence.None;
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
    public ClosedSymbolBase WithFillColor(NamedColor value)
    {
      if (_fillColor == value)
      {
        return this;
      }
      else
      {
        var result = (ClosedSymbolBase)MemberwiseClone();
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
    public PlotColorInfluence PlotColorInfluence { get { return _plotColorInfluence; } }

    /// <summary>
    /// Returns a copy of this symbol with the specified plot-color influence.
    /// </summary>
    /// <param name="value">The plot-color influence to apply.</param>
    /// <returns>The current instance if no change is required; otherwise, a cloned instance with the updated influence.</returns>
    public ClosedSymbolBase WithPlotColorInfluence(PlotColorInfluence value)
    {
      if (_plotColorInfluence == value)
      {
        return this;
      }
      else
      {
        var result = (ClosedSymbolBase)MemberwiseClone();
        result._plotColorInfluence = value;
        return result;
      }
    }

    /// <inheritdoc/>
    IScatterSymbol IScatterSymbol.WithPlotColorInfluence(PlotColorInfluence plotColorInfluence)
    {
      return WithPlotColorInfluence(plotColorInfluence);
    }

    /// <inheritdoc/>
    public double RelativeStructureWidth { get { return _relativeStructureWidth; } }

    /// <summary>
    /// Returns a copy of this symbol with the specified relative structure width.
    /// </summary>
    /// <param name="value">The relative width of internal structures.</param>
    /// <returns>The current instance if no change is required; otherwise, a cloned instance with the updated width.</returns>
    public ClosedSymbolBase WithRelativeStructureWidth(double value)
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
          var result = (ClosedSymbolBase)MemberwiseClone();
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
      get { return _frame; }
    }

    /// <summary>
    /// Returns a copy of this symbol with the specified frame.
    /// </summary>
    /// <param name="frame">The frame to apply.</param>
    /// <returns>A symbol with the specified frame.</returns>
    public ClosedSymbolBase WithFrame(IScatterSymbolFrame? frame)
    {
      return WithFrame(frame, null);
    }

    /// <inheritdoc/>
    IScatterSymbol IScatterSymbol.WithFrame(IScatterSymbolFrame? frame)
    {
      return WithFrame(frame, null);
    }

    /// <summary>
    /// Returns a copy of this symbol with the specified frame and optional plot-color influence for the frame.
    /// </summary>
    /// <param name="frame">The frame to apply.</param>
    /// <param name="isInfluencedByPlotColor">A value indicating whether the frame color is influenced by the plot color.</param>
    /// <returns>A symbol with the specified frame settings.</returns>
    public ClosedSymbolBase WithFrame(IScatterSymbolFrame? frame, bool? isInfluencedByPlotColor)
    {
      if (object.ReferenceEquals(_frame, frame) && (!isInfluencedByPlotColor.HasValue || _plotColorInfluence.HasFlag(PlotColorInfluence.FrameColorFull) == isInfluencedByPlotColor.Value))
      {
        return this;
      }
      else
      {
        var result = (ClosedSymbolBase)MemberwiseClone();
        result._frame = frame;
        if (isInfluencedByPlotColor.HasValue)
          result._plotColorInfluence = result._plotColorInfluence.WithFlag(PlotColorInfluence.FrameColorFull, isInfluencedByPlotColor.Value);
        return result;
      }
    }

    /// <inheritdoc/>
    public IScatterSymbolInset? Inset { get { return _inset; } }

    /// <summary>
    /// Returns a copy of this symbol with the specified inset.
    /// </summary>
    /// <param name="inset">The inset to apply.</param>
    /// <returns>A symbol with the specified inset.</returns>
    public ClosedSymbolBase WithInset(IScatterSymbolInset? inset)
    {
      return WithInset(inset, null);
    }

    /// <inheritdoc/>
    IScatterSymbol IScatterSymbol.WithInset(IScatterSymbolInset? inset)
    {
      return WithInset(inset, null);
    }

    /// <summary>
    /// Returns a copy of this symbol with the specified inset and optional plot-color influence for the inset.
    /// </summary>
    /// <param name="inset">The inset to apply.</param>
    /// <param name="isInfluencedByPlotColor">A value indicating whether the inset color is influenced by the plot color.</param>
    /// <returns>A symbol with the specified inset settings.</returns>
    public ClosedSymbolBase WithInset(IScatterSymbolInset? inset, bool? isInfluencedByPlotColor)
    {
      if (object.ReferenceEquals(_inset, inset) && (!isInfluencedByPlotColor.HasValue || _plotColorInfluence.HasFlag(PlotColorInfluence.InsetColorFull) == isInfluencedByPlotColor.Value))
      {
        return this;
      }
      else
      {
        var result = (ClosedSymbolBase)MemberwiseClone();
        result._inset = inset;

        if (isInfluencedByPlotColor.HasValue)
          result._plotColorInfluence = result._plotColorInfluence.WithFlag(PlotColorInfluence.InsetColorFull, isInfluencedByPlotColor.Value);
        return result;
      }
    }

    /// <inheritdoc/>
    public void CalculatePolygons(
      double? overrideRelativeStructureWidth,
      out Paths64? framePolygon,
      out Paths64? insetPolygon,
      out Paths64? fillPolygon)

    {
      insetPolygon = null;
      framePolygon = null;
      fillPolygon = null;

      // get outer polygon
      var outerPolygon = GetCopyOfOuterPolygon();

      Paths64? innerFramePolygon = null;
      double relativeStructureWidth = overrideRelativeStructureWidth ?? _relativeStructureWidth;
      if (_frame is not null && relativeStructureWidth > 0)
      {
        // get frame polygon
        innerFramePolygon = _frame.GetCopyOfClipperPolygon(relativeStructureWidth, outerPolygon);
      }

      if (_inset is not null)
      {
        // get inset polygon
        insetPolygon = _inset.GetCopyOfClipperPolygon(relativeStructureWidth);
      }

      // if null != insetPolygon
      // clip with innerPolygon ?? outerPolygon;
      // store clipped inset polygon / draw it with inset color
      if (insetPolygon is not null)
      {
        var clipper = new Clipper64();
        var solution = new Paths64();
        clipper.AddSubject(insetPolygon);
        clipper.AddClip(innerFramePolygon ?? outerPolygon);
        clipper.Execute(ClipType.Intersection, FillRule.NonZero, solution);
        insetPolygon = solution;
      }

      // if null != framePolygon
      // clip with outer polygon ????
      // draw combined path of outer polygon and frame polygon as a hole with frame color
      if (innerFramePolygon is not null)
      {
        var clipper = new Clipper64();
        clipper.AddSubject(outerPolygon);
        clipper.AddClip(innerFramePolygon);
        framePolygon = new Paths64();
        clipper.Execute(ClipType.Difference, FillRule.NonZero, framePolygon);
      }

      // calculate
      // if null != insetPolygon
      //	(framePolygon ?? outerPolygon ) - insetPolygon
      // or else use (framePolygon ?? outerPolygon ) directly
      // draw result with fillColor

      if (insetPolygon is not null)
      {
        var clipper = new Clipper64();
        clipper.AddSubject(innerFramePolygon ?? outerPolygon);
        clipper.AddClip(insetPolygon);
        fillPolygon = new Paths64();
        clipper.Execute(ClipType.Difference, FillRule.NonZero, fillPolygon);
      }
      else
      {
        fillPolygon = innerFramePolygon ?? outerPolygon;
      }
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
      if (!(GetType() == obj?.GetType()))
        return false;

      var from = (ClosedSymbolBase)obj;

      return
        _plotColorInfluence == from._plotColorInfluence &&
        _relativeStructureWidth == from._relativeStructureWidth &&
        _fillColor == from._fillColor &&
        Equals(_frame, from._frame) &&
        Equals(_inset, from.Inset);
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
      return
        GetType().GetHashCode() +
        (int)_plotColorInfluence +
        _relativeStructureWidth.GetHashCode() +
        _fillColor.GetHashCode() +
        (_frame?.GetHashCode() ?? 0) +
        (_inset?.GetHashCode() ?? 0);
    }
  }
}
