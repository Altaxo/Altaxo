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
  public abstract class ClosedSymbolBase : SymbolBase, IScatterSymbol
  {
    private PlotColorInfluence _plotColorInfluence = PlotColorInfluence.FillColorFull;
    protected NamedColor _fillColor = NamedColors.Black;

    protected double _relativeStructureWidth = 0.09375;
    protected IScatterSymbolFrame? _frame;
    protected IScatterSymbolInset? _inset;

    #region Serialization

    /// <summary>
    /// 2016-10-27 initial version.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ClosedSymbolBase), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (ClosedSymbolBase)obj;
        info.AddEnum("PlotColorInfluence", s._plotColorInfluence);
        info.AddValue("StructureScale", s._relativeStructureWidth);
        info.AddValue("Fill", s._fillColor);
        info.AddValueOrNull("Frame", s._frame);
        info.AddValueOrNull("Inset", s._inset);
      }

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

    protected ClosedSymbolBase()
    {
    }

    protected ClosedSymbolBase(NamedColor fillColor, bool isFillColorInfluencedByPlotColor)
    {
      _fillColor = fillColor;
      _plotColorInfluence = isFillColorInfluencedByPlotColor ? PlotColorInfluence.FillColorFull : PlotColorInfluence.None;
    }

    public object Clone()
    {
      return MemberwiseClone();
    }

    public double DesignSize { get { return ClipperSymbolSize; } }

    public NamedColor FillColor { get { return _fillColor; } }

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

    IScatterSymbol IScatterSymbol.WithFillColor(NamedColor fillColor)
    {
      return WithFillColor(fillColor);
    }

    public PlotColorInfluence PlotColorInfluence { get { return _plotColorInfluence; } }

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

    IScatterSymbol IScatterSymbol.WithPlotColorInfluence(PlotColorInfluence plotColorInfluence)
    {
      return WithPlotColorInfluence(plotColorInfluence);
    }

    public double RelativeStructureWidth { get { return _relativeStructureWidth; } }

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

    IScatterSymbol IScatterSymbol.WithRelativeStructureWidth(double relativeStructureWidth)
    {
      return WithRelativeStructureWidth(relativeStructureWidth);
    }

    public IScatterSymbolFrame? Frame
    {
      get { return _frame; }
    }

    public ClosedSymbolBase WithFrame(IScatterSymbolFrame? frame)
    {
      return WithFrame(frame, null);
    }

    IScatterSymbol IScatterSymbol.WithFrame(IScatterSymbolFrame? frame)
    {
      return WithFrame(frame, null);
    }

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

    public IScatterSymbolInset? Inset { get { return _inset; } }

    public ClosedSymbolBase WithInset(IScatterSymbolInset? inset)
    {
      return WithInset(inset, null);
    }

    IScatterSymbol IScatterSymbol.WithInset(IScatterSymbolInset? inset)
    {
      return WithInset(inset, null);
    }

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
