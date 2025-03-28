﻿#region Copyright

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

#nullable enable
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using Altaxo.Serialization;

namespace Altaxo.Graph.Gdi.Plot.Styles
{
  using System.ComponentModel;
  using System.Diagnostics.CodeAnalysis;
  using Altaxo.Data;
  using Altaxo.Drawing;
  using Altaxo.Main;
  using Drawing.ColorManagement;
  using Geometry;
  using Graph.Plot.Data;
  using Graph.Plot.Groups;
  using Plot.Data;
  using Plot.Groups;

  /// <summary>
  /// Summary description for XYPlotLineStyle.
  /// </summary>
  [DisplayName("${res:ClassNames.Altaxo.Graph.Gdi.Plot.Styles.DropAreaPlotStyle}")]
  public class DropAreaPlotStyle
    :
    Main.SuspendableDocumentNodeWithEventArgs,
    IG2DPlotStyle,
    IRoutedPropertyReceiver
  {
    protected ILineConnectionStyle _connectionStyle;

    /// <summary>If true, the start and the end point of the line are connected too.</summary>
    protected bool _connectCircular;

    protected bool _ignoreMissingDataPoints; // treat missing points as if not present (connect lines over missing points)

    /// <summary>If true, group styles that shift the logical position of the items (for instance <see cref="BarSizePosition3DGroupStyle"/>) are not applied. I.e. when true, the position of the item remains unperturbed.</summary>
    private bool _independentOnShiftingGroupStyles = true;

    protected CSPlaneID _fillDirection; // the direction to fill

    protected FillAreaRule _fillRule;

    protected BrushX? _fillBrush; // brush to fill the area under the line

    /// <summary>Designates if the fill color is independent or dependent.</summary>
    protected ColorLinkage _fillColorLinkage = ColorLinkage.PreserveAlpha;

    protected PenX _framePen;

    /// <summary>Designates if the fill color is independent or dependent.</summary>
    protected ColorLinkage _frameColorLinkage = ColorLinkage.PreserveAlpha;

    /// <summary>Logical x shift between the location of the real data point and the point where the item is finally drawn.</summary>
    private double _cachedLogicalShiftX;

    /// <summary>Logical y shift between the location of the real data point and the point where the item is finally drawn.</summary>
    private double _cachedLogicalShiftY;

    #region Serialization

    /// <summary>
    /// 2016-10-30 Initial version
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(DropAreaPlotStyle), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (DropAreaPlotStyle)obj;

        info.AddValue("Connection", s._connectionStyle);
        info.AddValue("ConnectCircular", s._connectCircular);
        info.AddValue("IgnoreMissingDataPoints", s._ignoreMissingDataPoints);
        info.AddValue("IndependentOnShiftingGroupStyles", s._independentOnShiftingGroupStyles);

        info.AddValue("FillDirection", s._fillDirection);
        info.AddEnum("FillRule", s._fillRule);
        info.AddValueOrNull("FillBrush", s._fillBrush);
        info.AddEnum("FillColorLinkage", s._fillColorLinkage);

        info.AddValue("Frame", s._framePen);
        info.AddEnum("FrameColorLinkage", s._frameColorLinkage);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (DropAreaPlotStyle?)o ?? new DropAreaPlotStyle(info);

        s._connectionStyle = (ILineConnectionStyle)info.GetValue("Connection", s);
        s._connectCircular = info.GetBoolean("ConnectCircular");
        s._ignoreMissingDataPoints = info.GetBoolean("IgnoreMissingDataPoints");
        s._independentOnShiftingGroupStyles = info.GetBoolean("IndependentOnShiftingGroupStyles");

        s._fillDirection = (CSPlaneID)info.GetValue("FillDirection", s);
        s._fillRule = (FillAreaRule)info.GetEnum("FillRule", typeof(FillAreaRule));
        s._fillBrush = info.GetValueOrNull<BrushX>("FillBrush", s);
        s._fillColorLinkage = (ColorLinkage)info.GetEnum("FillColorLinkage", typeof(ColorLinkage));

        s._framePen = (PenX)info.GetValue("Pen", s);
        s._frameColorLinkage = (ColorLinkage)info.GetEnum("FrameColorLinkage", typeof(ColorLinkage));
        return s;
      }
    }

    #endregion Serialization

    #region Construction and copying

    [MemberNotNull(nameof(_connectionStyle), nameof(_fillDirection), nameof(_framePen))]
    public void CopyFrom(DropAreaPlotStyle from, Main.EventFiring eventFiring)
    {
      if (ReferenceEquals(this, from))
#pragma warning disable CS8774 // Member must have a non-null value when exiting.
        return;
#pragma warning restore CS8774 // Member must have a non-null value when exiting.

      using (var suspendToken = SuspendGetToken())
      {
        _connectionStyle = from._connectionStyle;
        _connectCircular = from._connectCircular;
        _ignoreMissingDataPoints = from._ignoreMissingDataPoints;
        _independentOnShiftingGroupStyles = from._independentOnShiftingGroupStyles;

        _fillDirection = from._fillDirection;
        _fillRule = from._fillRule;
        _fillBrush = from._fillBrush;
        _fillColorLinkage = from._fillColorLinkage;
        _framePen = from._framePen;
        _frameColorLinkage = from._frameColorLinkage;

        EhSelfChanged();

        suspendToken.Resume(eventFiring);
      }
    }

    /// <inheritdoc/>
    public bool CopyFrom(object obj, bool copyWithDataReferences)
    {
      if (ReferenceEquals(this, obj))
        return true;
      var from = obj as DropAreaPlotStyle;
      if (from is not null)
      {
        CopyFrom(from, Main.EventFiring.Enabled);
        return true;
      }
      return false;
    }

    /// <inheritdoc/>
    public bool CopyFrom(object obj)
    {
      if (ReferenceEquals(this, obj))
        return true;

        return CopyFrom(obj, true);
    }

    /// <inheritdoc/>
    public object Clone(bool copyWithDataReferences)
    {
      return new DropAreaPlotStyle(this);
    }

    /// <inheritdoc/>
    public object Clone()
    {
      return new DropAreaPlotStyle(this);
    }

#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
    protected DropAreaPlotStyle(Altaxo.Serialization.Xml.IXmlDeserializationInfo info)
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
    {
      _connectionStyle = LineConnectionStyles.StraightConnection.Instance;
    }

    public DropAreaPlotStyle(ILineConnectionStyle connection, bool ignoreMissingDataPoints, bool connectCircular, CSPlaneID direction, BrushX fillBrush, ColorLinkage fillColorLinkage)
    {
      _connectionStyle = connection;
      _ignoreMissingDataPoints = ignoreMissingDataPoints;
      _connectCircular = connectCircular;
      _fillDirection = direction;
      _fillColorLinkage = fillColorLinkage;
      _fillBrush = fillBrush;
      _framePen = new PenX(NamedColors.Transparent, 1);
    }

    public DropAreaPlotStyle(Altaxo.Main.Properties.IReadOnlyPropertyBag context)
    {
      var penWidth = GraphDocument.GetDefaultPenWidth(context);
      var color = GraphDocument.GetDefaultPlotColor(context);

      _framePen = new PenX(NamedColors.Transparent, penWidth).WithLineJoin(LineJoin.Bevel);
      _ignoreMissingDataPoints = false;
      _fillBrush = new BrushX(color);
      _fillDirection = new CSPlaneID(1, 0);
      _connectionStyle = LineConnectionStyles.StraightConnection.Instance;
    }

    public DropAreaPlotStyle(DropAreaPlotStyle from)
    {
      CopyFrom(from, Main.EventFiring.Suppressed);
    }

    protected override IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
    {
      yield break;
    }

    #endregion Construction and copying

    #region Properties

    public ILineConnectionStyle Connection
    {
      get { return _connectionStyle; }
      set
      {
        if (value is null)
          throw new ArgumentNullException(nameof(value));

        if (!(_connectionStyle.Equals(value)))
        {
          _connectionStyle = value;
          EhSelfChanged();
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

    /// <summary>
    /// True when we don't want to shift the position of the items, for instance due to the bar graph plot group.
    /// </summary>
    public bool IndependentOnShiftingGroupStyles
    {
      get
      {
        return _independentOnShiftingGroupStyles;
      }
      set
      {
        if (!(_independentOnShiftingGroupStyles == value))
        {
          _independentOnShiftingGroupStyles = value;
          EhSelfChanged();
        }
      }
    }

    public CSPlaneID FillDirection
    {
      get { return _fillDirection; }
      set
      {
        CSPlaneID oldvalue = _fillDirection;
        _fillDirection = value;
        if (oldvalue != value)
        {
          EhSelfChanged(EventArgs.Empty); // Fire Changed event
        }
      }
    }

    public FillAreaRule FillRule
    {
      get { return _fillRule; }
      set
      {
        if (!(_fillRule == value))
        {
          _fillRule = value;
          EhSelfChanged(EventArgs.Empty); // Fire Changed event
        }
      }
    }

    public BrushX? FillBrush
    {
      get { return _fillBrush; }
      set
      {
        // copy the brush only if not null
        if (value is null)
          throw new ArgumentNullException("FillBrush", "FillBrush must not be set to null, instead set FillArea to false");

        if (!(_fillBrush == value))
        {
          _fillBrush = value;
          EhSelfChanged(EventArgs.Empty); // Fire Changed event
        }
      }
    }

    public ColorLinkage FillColorLinkage
    {
      get
      {
        return _fillColorLinkage;
      }
      set
      {
        var oldValue = _fillColorLinkage;
        _fillColorLinkage = value;
        if (value != oldValue)
          EhSelfChanged(EventArgs.Empty);
      }
    }

    public PenX FramePen
    {
      get { return _framePen; }
      set
      {
        if (value is null)
          throw new ArgumentNullException(nameof(value));

        if (!(_framePen == value))
        {
          _framePen = value;
          EhSelfChanged();
        }
      }
    }

    public ColorLinkage FrameColorLinkage
    {
      get
      {
        return _frameColorLinkage;
      }
      set
      {
        if (!(_frameColorLinkage == value))
        {
          _frameColorLinkage = value;
          EhSelfChanged();
        }
      }
    }

    public bool IsVisible
    {
      get
      {
        return
          !(LineConnectionStyles.NoConnection.Instance.Equals(_connectionStyle)) &&
          (_fillBrush?.IsVisible ?? false || _framePen.IsVisible);
      }
    }

    #endregion Properties

    #region Painting

    public RectangleF PaintSymbol(System.Drawing.Graphics g, System.Drawing.RectangleF bounds)
    {
      return bounds;
    }

    public void Paint(Graphics g, IPlotArea layer, Processed2DPlotData pdata, Processed2DPlotData? prevItemData, Processed2DPlotData? nextItemData)
    {
      if (_connectionStyle is LineConnectionStyles.NoConnection)
        return;

      if (!(pdata.RangeList is { } rangeList) || !(pdata.PlotPointsInAbsoluteLayerCoordinates is { } plotPositions))
        return;

      if (_independentOnShiftingGroupStyles)
      {
        _cachedLogicalShiftX = _cachedLogicalShiftY = 0;
      }

      if (0 != _cachedLogicalShiftX || 0 != _cachedLogicalShiftY)
      {
        plotPositions = Processed2DPlotData.GetPlotPointsInAbsoluteLayerCoordinatesWithShift(pdata, layer, _cachedLogicalShiftX, _cachedLogicalShiftY);
      }

      if (plotPositions is null)
        return;

      _fillDirection = layer.UpdateCSPlaneID(_fillDirection);

      var gp = new GraphicsPath();

      if (_ignoreMissingDataPoints)
      {
        // in case we ignore the missing points, all ranges can be plotted
        // as one range, i.e. continuously
        // for this, we create the totalRange, which contains all ranges
        IPlotRange totalRange = new PlotRangeCompound(rangeList);
        _connectionStyle.FillOneRange(gp, pdata, totalRange, layer, _fillDirection, _ignoreMissingDataPoints, _connectCircular, plotPositions, _cachedLogicalShiftX, _cachedLogicalShiftY);
      }
      else // we not ignore missing points, so plot all ranges separately
      {
        for (int i = 0; i < rangeList.Count; i++)
        {
          _connectionStyle.FillOneRange(gp, pdata, rangeList[i], layer, _fillDirection, _ignoreMissingDataPoints, _connectCircular, plotPositions, _cachedLogicalShiftX, _cachedLogicalShiftY);
        }
      }

      if (_fillBrush is not null)
      {
        using (var fillBrushGdi = BrushCacheGdi.Instance.BorrowBrush(_fillBrush, new RectangleD2D(PointD2D.Empty, layer.Size), g, 1))
        {
          g.FillPath(fillBrushGdi, gp);
        }
      }

      using (var framePenGdi = PenCacheGdi.Instance.BorrowPen(_framePen))
      {
        g.DrawPath(framePenGdi, gp);
      }
    }

    #endregion Painting

    #region IG2DPlotStyle Members

    /// <summary>
    /// Prepares the scale of this plot style. Since this style does not utilize a scale, this function does nothing.
    /// </summary>
    /// <param name="layer">The parent layer.</param>
    public void PrepareScales(IPlotArea layer)
    {
    }

    public void CollectExternalGroupStyles(PlotGroupStyleCollection externalGroups)
    {
    }

    public void CollectLocalGroupStyles(PlotGroupStyleCollection externalGroups, PlotGroupStyleCollection localGroups)
    {
      ColorGroupStyle.AddLocalGroupStyle(externalGroups, localGroups);
      DashPatternGroupStyle.AddLocalGroupStyle(externalGroups, localGroups);
      IgnoreMissingDataPointsGroupStyle.AddLocalGroupStyle(externalGroups, localGroups);
      LineConnection2DGroupStyle.AddLocalGroupStyle(externalGroups, localGroups);
    }

    public void PrepareGroupStyles(PlotGroupStyleCollection externalGroups, PlotGroupStyleCollection localGroups, IPlotArea layer, Processed2DPlotData pdata)
    {
      if (_fillColorLinkage == ColorLinkage.Dependent && _fillBrush is not null)
        ColorGroupStyle.PrepareStyle(externalGroups, localGroups, delegate ()
        { return _fillBrush.Color; });
      else if (_frameColorLinkage == ColorLinkage.Dependent && _framePen is not null)
        ColorGroupStyle.PrepareStyle(externalGroups, localGroups, delegate ()
        { return _framePen.Color; });

      IgnoreMissingDataPointsGroupStyle.PrepareStyle(externalGroups, localGroups, () => _ignoreMissingDataPoints);
      LineConnection2DGroupStyle.PrepareStyle(externalGroups, localGroups, () => new Tuple<ILineConnectionStyle, bool>(_connectionStyle, _connectCircular));
    }

    public void ApplyGroupStyles(PlotGroupStyleCollection externalGroups, PlotGroupStyleCollection localGroups)
    {
      // IgnoreMissingDataPoints is the same for all sub plot styles
      IgnoreMissingDataPointsGroupStyle.ApplyStyle(externalGroups, localGroups, (ignoreMissingDataPoints) => _ignoreMissingDataPoints = ignoreMissingDataPoints);

      // LineConnectionStyle is the same for all sub plot styles
      LineConnection2DGroupStyle.ApplyStyle(externalGroups, localGroups, (lineConnection, connectCircular) => { _connectionStyle = lineConnection; _connectCircular = connectCircular; });

      if (ColorLinkage.Independent != _fillColorLinkage)
      {
        _fillBrush ??= new BrushX(NamedColors.Black);

        if (_fillColorLinkage == ColorLinkage.Dependent)
          ColorGroupStyle.ApplyStyle(externalGroups, localGroups, delegate (NamedColor c)
          { _fillBrush = _fillBrush.WithColor(c); });
        else if (ColorLinkage.PreserveAlpha == _fillColorLinkage)
          ColorGroupStyle.ApplyStyle(externalGroups, localGroups, delegate (NamedColor c)
          { _fillBrush = _fillBrush.WithColor(c.NewWithAlphaValue(_fillBrush.Color.Color.A)); });
      }
      if (ColorLinkage.Independent != _frameColorLinkage)
      {
        _framePen ??= new PenX(NamedColors.Black);

        if (_frameColorLinkage == ColorLinkage.Dependent)
          ColorGroupStyle.ApplyStyle(externalGroups, localGroups, delegate (NamedColor c)
          { _framePen = _framePen.WithColor(c); });
        else if (ColorLinkage.PreserveAlpha == _fillColorLinkage)
          ColorGroupStyle.ApplyStyle(externalGroups, localGroups, delegate (NamedColor c)
          { _framePen = _framePen.WithColor(c.NewWithAlphaValue(_framePen.Color.Color.A)); });
      }

      // Shift the items ?
      _cachedLogicalShiftX = 0;
      _cachedLogicalShiftY = 0;
      if (!_independentOnShiftingGroupStyles)
      {
        var shiftStyle = PlotGroupStyle.GetFirstStyleToApplyImplementingInterface<IShiftLogicalXYGroupStyle>(externalGroups, localGroups);
        if (shiftStyle is not null)
        {
          shiftStyle.Apply(out _cachedLogicalShiftX, out _cachedLogicalShiftY);
        }
      }
    }

    #endregion IG2DPlotStyle Members

    #region IDocumentNode Members

    /// <summary>
    /// Replaces path of items (intended for data items like tables and columns) by other paths. Thus it is possible
    /// to change a plot so that the plot items refer to another table.
    /// </summary>
    /// <param name="Report">Function that reports the found <see cref="DocNodeProxy"/> instances to the visitor.</param>
    public void VisitDocumentReferences(DocNodeProxyReporter Report)
    {
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

    #endregion IDocumentNode Members

    #region IRoutedPropertyReceiver Members

    public IEnumerable<(string PropertyName, object PropertyValue, Action<object> PropertySetter)> GetRoutedProperties(string propertyName)
    {
      switch (propertyName)
      {
        case "StrokeWidth":
          yield return (propertyName, _framePen.Width, (w) => _framePen = _framePen.WithWidth((double)w));
          break;
      }

      yield break;
    }

    #endregion IRoutedPropertyReceiver Members
  } // end class XYPlotLineStyle
}
