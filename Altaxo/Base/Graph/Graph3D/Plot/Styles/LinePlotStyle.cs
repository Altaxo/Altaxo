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

namespace Altaxo.Graph.Graph3D.Plot.Styles
{
  using System.Diagnostics.CodeAnalysis;
  using Altaxo.Data;
  using Altaxo.Main;
  using Drawing;
  using Drawing.D3D;
  using Drawing.DashPatternManagement;
  using Geometry;
  using Graph.Plot.Data;
  using Graph.Plot.Groups;
  using GraphicsContext;
  using Plot.Data;
  using Plot.Groups;

  /// <summary>
  /// Style to show 3D data as lines.
  /// </summary>
  public class LinePlotStyle
    :
    Main.SuspendableDocumentNodeWithEventArgs,
    IG3DPlotStyle,
    IRoutedPropertyReceiver
  {
    /// <summary>A value indicating whether the skip frequency value is independent from other values.</summary>
    protected bool _independentSkipFreq;

    /// <summary>A value of 2 skips every other data point, a value of 3 skips 2 out of 3 data points, and so on.</summary>
    protected int _skipFreq = 1;

    protected bool _independentColor;

    protected bool _independentDashStyle;

    protected PenX3D _linePen;

    protected ILineConnectionStyle _connectionStyle;

    /// <summary>
    /// true if the symbol size is independent, i.e. is not published nor updated by a group style.
    /// </summary>
    protected bool _independentSymbolSize;

    /// <summary>Controls the length of the end bar.</summary>
    protected double _symbolSize;

    protected bool _ignoreMissingDataPoints; // treat missing points as if not present (connect lines over missing points)

    /// <summary>If true, the start and the end point of the line are connected too.</summary>
    protected bool _connectCircular;

    protected bool _useSymbolGap;

    /// <summary>
    /// If true, the line's west and north vector are kept as if no symbol gap exist. If false, the west and north vector
    /// of the line after the symbol gap is calculated anew.
    /// </summary>
    protected bool _keepWestNorthThroughSymbolGap;

    /// <summary>
    /// Offset used to calculate the real gap between symbol center and beginning of the bar, according to the formula:
    /// realGap = _symbolGap * _symbolGapFactor + _symbolGapOffset;
    /// </summary>
    private double _symbolGapOffset;

    /// <summary>
    /// Factor used to calculate the real gap between symbol center and beginning of the bar, according to the formula:
    /// realGap = _symbolGap * _symbolGapFactor + _symbolGapOffset;
    /// </summary>
    private double _symbolGapFactor = 1.25;

    /// <summary>If this function is set, then _symbolSize is ignored and the symbol size is evaluated by this function.</summary>
    [field: NonSerialized]
    protected Func<int, double>? _cachedSymbolSizeForIndexFunction;

    #region Serialization

    /// <summary>
    /// 2016-05-06 initial version.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(LinePlotStyle), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (LinePlotStyle)obj;

        info.AddValue("IndependentSkipFreq", s._independentSkipFreq);
        info.AddValue("SkipFreq", s._skipFreq);

        info.AddValue("IgnoreMissingPoints", s._ignoreMissingDataPoints);
        info.AddValue("ConnectCircular", s._connectCircular);
        info.AddValue("Connection", s._connectionStyle);
        info.AddValue("Pen", s._linePen);
        info.AddValue("IndependentDashStyle", s._independentDashStyle);
        info.AddValue("IndependentColor", s._independentColor);

        info.AddValue("IndependentSymbolSize", s._independentSymbolSize);
        info.AddValue("SymbolSize", s._symbolSize);

        info.AddValue("UseSymbolGap", s._useSymbolGap);
        info.AddValue("SymbolGapOffset", s._symbolGapOffset);
        info.AddValue("SymbolGapFactor", s._symbolGapFactor);
        info.AddValue("KeepWestNorth", s._keepWestNorthThroughSymbolGap);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (LinePlotStyle?)o ?? new LinePlotStyle(info);

        s._independentSkipFreq = info.GetBoolean("IndependentSkipFreq");
        s._skipFreq = info.GetInt32("SkipFreq");

        s._ignoreMissingDataPoints = info.GetBoolean("IgnoreMissingPoints");
        s._connectCircular = info.GetBoolean("ConnectCircular");
        s.Connection = (ILineConnectionStyle)info.GetValue("Connection", s);
        s._linePen = (PenX3D)info.GetValue("Pen", s);
        s._independentDashStyle = info.GetBoolean("IndependentDashStyle");
        s._independentColor = info.GetBoolean("IndependentColor");

        s._independentSymbolSize = info.GetBoolean("IndependentSymbolSize");
        s._symbolSize = info.GetDouble("SymbolSize");

        s._useSymbolGap = info.GetBoolean("UseSymbolGap");
        s._symbolGapOffset = info.GetDouble("SymbolGapOffset");
        s._symbolGapFactor = info.GetDouble("SymbolGapFactor");
        s._keepWestNorthThroughSymbolGap = info.GetBoolean("KeepWestNorth");
        return s;
      }
    }

    /// <summary>
    /// Deserialization constructor.
    /// </summary>
    /// <param name="info">The deserialization information.</param>
#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
    protected LinePlotStyle(Altaxo.Serialization.Xml.IXmlDeserializationInfo info)
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
    {
    }

    #endregion Serialization

    #region Construction and copying

    [MemberNotNull(nameof(_linePen), nameof(_connectionStyle))]
    public void CopyFrom(LinePlotStyle from, Main.EventFiring eventFiring)
    {
      if (object.ReferenceEquals(this, from))
#pragma warning disable CS8774 // Member must have a non-null value when exiting.
        return;
#pragma warning restore CS8774 // Member must have a non-null value when exiting.

      using (var suspendToken = SuspendGetToken())
      {
        _independentSkipFreq = from._independentSkipFreq;
        _skipFreq = from._skipFreq;

        _ignoreMissingDataPoints = from._ignoreMissingDataPoints;
        _connectCircular = from._connectCircular;
        _connectionStyle = from._connectionStyle; 

        _linePen = from._linePen; // immutable
        _independentDashStyle = from._independentDashStyle;
        _independentColor = from._independentColor;

        _independentSymbolSize = from._independentSymbolSize;
        _symbolSize = from._symbolSize;

        _useSymbolGap = from._useSymbolGap;
        _symbolGapOffset = from._symbolGapOffset;
        _symbolGapFactor = from._symbolGapFactor;
        _keepWestNorthThroughSymbolGap = from._keepWestNorthThroughSymbolGap;

        EhSelfChanged();
        suspendToken.Resume(eventFiring);
      }
    }

    /// <inheritdoc/>
    public bool CopyFrom(object obj, bool copyWithDataReferences)
    {
      if (object.ReferenceEquals(this, obj))
        return true;
      var from = obj as LinePlotStyle;
      if (null != from)
      {
        CopyFrom(from, Main.EventFiring.Enabled);
        return true;
      }
      return false;
    }

    /// <inheritdoc/>
    public bool CopyFrom(object obj)
    {
      return CopyFrom(obj, true);
    }

    /// <inheritdoc/>
    public object Clone(bool copyWithDataReferences)
    {
      return new LinePlotStyle(this);
    }

    /// <inheritdoc/>
    public object Clone()
    {
      return new LinePlotStyle(this);
    }

    public LinePlotStyle(Altaxo.Main.Properties.IReadOnlyPropertyBag context)
    {
      var penWidth = GraphDocument.GetDefaultPenWidth(context);
      var color = GraphDocument.GetDefaultPlotColor(context);

      _linePen = new PenX3D(color, penWidth).WithLineJoin(PenLineJoin.Bevel);
      _connectionStyle = new LineConnectionStyles.StraightConnection();
    }

    public LinePlotStyle(LinePlotStyle from)
    {
      CopyFrom(from, Main.EventFiring.Suppressed);
    }

    protected override IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
    {
      yield break;
    }

    #endregion Construction and copying

    #region Properties

    /// <summary>Skip frequency. A value of 2 skips every other data point, a value of 3 skips 2 out of 3 data points, and so on.</summary>
    public int SkipFrequency
    {
      get { return _skipFreq; }
      set
      {
        value = Math.Max(1, value);
        if (!(_skipFreq == value))
        {
          _skipFreq = value;
          EhSelfChanged(EventArgs.Empty); // Fire Changed event
        }
      }
    }

    /// <summary>A value indicating whether the skip frequency value is independent from values in other sub plot styles.</summary>
    public bool IndependentSkipFrequency
    {
      get
      {
        return _independentSkipFreq;
      }
      set
      {
        if (!(_independentSkipFreq == value))
        {
          _independentSkipFreq = value;
          EhSelfChanged(EventArgs.Empty);
        }
      }
    }

    public ILineConnectionStyle Connection
    {
      get { return _connectionStyle; }
      set
      {
        if (null == value)
          throw new ArgumentNullException(nameof(value));

        if (!object.ReferenceEquals(value, _connectionStyle))
        {
          _connectionStyle = value;
          EhSelfChanged(EventArgs.Empty); // Fire Changed event
        }
      }
    }

    /// <summary>
    /// True when the line is not drawn in the circle of diameter SymbolSize around the symbol center.
    /// </summary>
    public bool UseSymbolGap
    {
      get { return _useSymbolGap; }
      set
      {
        if (!(_useSymbolGap == value))
        {
          _useSymbolGap = value;
          EhSelfChanged(EventArgs.Empty);
        }
      }
    }

    public double SymbolGapOffset
    {
      get
      {
        return _symbolGapOffset;
      }
      set
      {
        if (!(_symbolGapOffset == value))
        {
          _symbolGapOffset = value;
          EhSelfChanged();
        }
      }
    }

    public double SymbolGapFactor
    {
      get
      {
        return _symbolGapFactor;
      }
      set
      {
        if (!(_symbolGapFactor == value))
        {
          _symbolGapFactor = value;
          EhSelfChanged();
        }
      }
    }

    /// <summary>
    /// If true, the line's west and north vector are kept as if no symbol gap exist. If false, the west and north vector
    /// of the line after the symbol gap is calculated anew.
    /// </summary>
    public bool KeepWestNorthThroughSymbolGap
    {
      get
      {
        return _keepWestNorthThroughSymbolGap;
      }
      set
      {
        if (!(_keepWestNorthThroughSymbolGap == value))
        {
          _keepWestNorthThroughSymbolGap = value;
          EhSelfChanged(EventArgs.Empty);
        }
      }
    }

    public bool ConnectCircular
    {
      get
      {
        return _connectCircular;
      }
      set
      {
        bool oldValue = _connectCircular;
        _connectCircular = value;
        if (value != oldValue)
          EhSelfChanged(EventArgs.Empty);
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether to ignore missing data points. If the value is set to true,
    /// the line is plotted even if there is a gap in the data points.
    /// </summary>
    /// <value>
    /// <c>true</c> if missing data points should be ignored; otherwise, if <c>false</c>, no line is plotted between a gap in the data.
    /// </value>
    public bool IgnoreMissingDataPoints
    {
      get
      {
        return _ignoreMissingDataPoints;
      }
      set
      {
        bool oldValue = _ignoreMissingDataPoints;
        _ignoreMissingDataPoints = value;
        if (value != oldValue)
          EhSelfChanged(EventArgs.Empty);
      }
    }

    public bool IndependentLineColor
    {
      get
      {
        return _independentColor;
      }
      set
      {
        bool oldValue = _independentColor;
        _independentColor = value;
        if (value != oldValue)
          EhSelfChanged(EventArgs.Empty);
      }
    }

    public bool IndependentDashStyle
    {
      get
      {
        return _independentDashStyle;
      }
      set
      {
        bool oldValue = _independentDashStyle;
        _independentDashStyle = value;
        if (value != oldValue)
          EhSelfChanged(EventArgs.Empty);
      }
    }

    public PenX3D LinePen
    {
      get { return _linePen; }
      set
      {
        if (!object.ReferenceEquals(_linePen, value))
        {
          _linePen = value;
          EhSelfChanged();
        }
      }
    }

    /// <summary>
    /// true if the symbol size is independent, i.e. is not published nor updated by a group style.
    /// </summary>
    public bool IndependentSymbolSize
    {
      get { return _independentSymbolSize; }
      set
      {
        var oldValue = _independentSymbolSize;
        _independentSymbolSize = value;
        if (oldValue != value)
          EhSelfChanged(EventArgs.Empty);
      }
    }

    /// <summary>Controls the length of the end bar.</summary>
    public double SymbolSize
    {
      get { return _symbolSize; }
      set
      {
        var oldValue = _symbolSize;
        _symbolSize = value;
        if (oldValue != value)
          EhSelfChanged(EventArgs.Empty);
      }
    }

    public bool IsVisible
    {
      get
      {
        return !object.ReferenceEquals(_connectionStyle, LineConnectionStyles.NoConnection.Instance);
      }
    }

    #endregion Properties

    #region Painting

    public virtual void PaintLine(IGraphicsContext3D g, PointD3D beg, PointD3D end)
    {
      if (null != _linePen)
      {
        g.DrawLine(_linePen, beg, end);
      }
    }

    public RectangleD3D PaintSymbol(IGraphicsContext3D g, RectangleD3D bounds)
    {
      if (IsVisible)
      {
        var gs = g.SaveGraphicsState();
        g.TranslateTransform((VectorD3D)bounds.Center);
        var halfwidth = bounds.SizeX / 2;
        var symsize = _symbolSize;

        if (_useSymbolGap)
        {
          // plot a line with the length of symbolsize from
          PaintLine(g, new PointD3D(-halfwidth, 0, 0), new PointD3D(-symsize, 0, 0));
          PaintLine(g, new PointD3D(symsize, 0, 0), new PointD3D(halfwidth, 0, 0));
        }
        else // no gap
        {
          PaintLine(g, new PointD3D(-halfwidth, 0, 0), new PointD3D(halfwidth, 0, 0));
        }
        g.RestoreGraphicsState(gs);
      }

      return bounds;
    }

    public void Paint(IGraphicsContext3D g, IPlotArea layer, Processed3DPlotData pdata, Processed3DPlotData? prevItemData, Processed3DPlotData? nextItemData)
    {
      var linePoints = pdata.PlotPointsInAbsoluteLayerCoordinates;
      var rangeList = pdata.RangeList;
      var symbolGap = _symbolSize;

      if (rangeList is null || rangeList.Count == 0)
        return;

      int rangelistlen = rangeList.Count;

      Func<int, double>? symbolGapFunction = null;

      if (_useSymbolGap)
      {
        if (null != _cachedSymbolSizeForIndexFunction && !_independentSymbolSize)
        {
          symbolGapFunction = (idx) => _symbolGapOffset + _symbolGapFactor * _cachedSymbolSizeForIndexFunction(idx);
        }
        else
        {
          symbolGapFunction = (idx) => _symbolGapOffset + _symbolGapFactor * _symbolSize;
        }
      }

      if (_ignoreMissingDataPoints)
      {
        // in case we ignore the missing points, all ranges can be plotted
        // as one range, i.e. continuously
        // for this, we create the totalRange, which contains all ranges
        var totalRange = new PlotRange(rangeList[0].LowerBound, rangeList[rangelistlen - 1].UpperBound);
        _connectionStyle.Paint(g, pdata, totalRange, layer, _linePen, symbolGapFunction, _skipFreq, _connectCircular);
      }
      else // we not ignore missing points, so plot all ranges separately
      {
        for (int i = 0; i < rangelistlen; i++)
        {
          _connectionStyle.Paint(g, pdata, rangeList[i], layer, _linePen, symbolGapFunction, _skipFreq, _connectCircular);
        }
      }
    }

    #endregion Painting

    /// <summary>
    /// Prepares the scale of this plot style. Since this style does not utilize a scale, this function does nothing.
    /// </summary>
    /// <param name="layer">The parent layer.</param>
    public void PrepareScales(Graph3D.IPlotArea layer)
    {
    }

    public bool IsColorProvider
    {
      get { return !_independentColor; }
    }

    public NamedColor Color
    {
      get
      {
        return _linePen.Color;
      }
      set
      {
        _linePen = _linePen.WithColor(value);
      }
    }

    public bool IsColorReceiver
    {
      get { return !_independentColor; }
    }

    #region IG3DPlotStyle Members

    public void CollectExternalGroupStyles(PlotGroupStyleCollection externalGroups)
    {
    }

    public void CollectLocalGroupStyles(PlotGroupStyleCollection externalGroups, PlotGroupStyleCollection localGroups)
    {
      ColorGroupStyle.AddLocalGroupStyle(externalGroups, localGroups);
      DashPatternGroupStyle.AddLocalGroupStyle(externalGroups, localGroups);
    }

    public void PrepareGroupStyles(PlotGroupStyleCollection externalGroups, PlotGroupStyleCollection localGroups, IPlotArea layer, Processed3DPlotData pdata)
    {
      if (IsColorProvider)
        ColorGroupStyle.PrepareStyle(externalGroups, localGroups, delegate ()
        { return Color; });

      if (!_independentDashStyle)
        DashPatternGroupStyle.PrepareStyle(externalGroups, localGroups, delegate
        { return LinePen.DashPattern ?? DashPatternListManager.Instance.BuiltinDefaultSolid; });
    }

    public void ApplyGroupStyles(PlotGroupStyleCollection externalGroups, PlotGroupStyleCollection localGroups)
    {
      // SkipFrequency should be the same for all sub plot styles
      if (!_independentSkipFreq)
      {
        _skipFreq = 1;
        SkipFrequencyGroupStyle.ApplyStyle(externalGroups, localGroups, delegate (int c)
        { _skipFreq = c; });
      }

      if (IsColorReceiver)
        ColorGroupStyle.ApplyStyle(externalGroups, localGroups, delegate (NamedColor c)
        { Color = c; });

      if (!_independentDashStyle)
        DashPatternGroupStyle.ApplyStyle(externalGroups, localGroups, delegate (IDashPattern c)
        { _linePen = LinePen.WithDashPattern(c); });

      if (!_independentSymbolSize)
      {
        _symbolSize = 0;
        SymbolSizeGroupStyle.ApplyStyle(externalGroups, localGroups, delegate (double size)
        { _symbolSize = size; });
      }

      // symbol size
      if (!_independentSymbolSize)
      {
        _symbolSize = 0;
        SymbolSizeGroupStyle.ApplyStyle(externalGroups, localGroups, delegate (double size)
        { _symbolSize = size; });

        // but if there is an symbol size evaluation function, then use this with higher priority.
        _cachedSymbolSizeForIndexFunction = null;
        VariableSymbolSizeGroupStyle.ApplyStyle(externalGroups, localGroups, delegate (Func<int, double> evalFunc)
        { _cachedSymbolSizeForIndexFunction = evalFunc; });
      }
      else
      {
        _cachedSymbolSizeForIndexFunction = null;
      }
    }

    #endregion IG3DPlotStyle Members

    #region IDocumentNode Members

    /// <summary>
    /// Replaces path of items (intended for data items like tables and columns) by other paths. Thus it is possible
    /// to change a plot so that the plot items refer to another table.
    /// </summary>
    /// <param name="Report">Function that reports the found <see cref="DocNodeProxy"/> instances to the visitor.</param>
    public void VisitDocumentReferences(DocNodeProxyReporter Report)
    {
    }

    #endregion IDocumentNode Members

    #region IRoutedPropertyReceiver Members

    public IEnumerable<(string PropertyName, object PropertyValue, Action<object> PropertySetter)> GetRoutedProperties(string propertyName)
    {
      switch (propertyName)
      {
        case "StrokeWidth":
          yield return (propertyName, _linePen.Thickness1, (w) => LinePen = _linePen.WithThickness1((double)w));
          yield return (propertyName, _linePen.Thickness2, (w) => LinePen = _linePen.WithThickness2((double)w));
          break;
      }

      yield break;
    }

    /// <inheritdoc/>
    public IEnumerable<(
      string ColumnLabel, // Column label
      IReadableColumn? Column, // the column as it was at the time of this call
      string? ColumnName, // the name of the column (last part of the column proxies document path)
      Action<IReadableColumn?> ColumnSetAction // action to set the column during Apply of the controller
      )> GetAdditionallyUsedColumns()
    {
      yield break; // no additionally used columns
    }

    #endregion IRoutedPropertyReceiver Members
  } // end class XYPlotLineStyle
}
