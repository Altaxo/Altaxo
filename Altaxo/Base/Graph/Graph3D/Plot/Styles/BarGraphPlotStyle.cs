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
using System.Text;

namespace Altaxo.Graph.Graph3D.Plot.Styles
{
  using System.Diagnostics.CodeAnalysis;
  using Altaxo.Data;
  using Altaxo.Main;
  using Drawing;
  using Drawing.D3D;
  using Drawing.D3D.Material;
  using Geometry;
  using Graph.Plot.Data;
  using Graph.Plot.Groups;
  using GraphicsContext;
  using Plot.Data;
  using Plot.Groups;

  /// <summary>
  ///
  /// </summary>
  public class BarGraphPlotStyle
    :
    Main.SuspendableDocumentNodeWithEventArgs,
    IG3DPlotStyle
  {
    /// <summary>
    /// Indicates whether the  color is dependent (can be set by the ColorGroupStyle) or not.
    /// </summary>
    private bool _independentColor;

    /// <summary>
    /// Pen used to draw the bar.
    /// </summary>
    private PenX3D _pen;

    /// <summary>
    /// If true, the thickness 1 should be equal to thickness 2 of the pen. We use the minimum of the calculated thicknesses in this case.
    /// </summary>
    private bool _useUniformCrossSectionThickness;

    /// <summary>
    /// The bar shift strategy, i.e. how to shift the bars that belong to one group.
    /// </summary>
    private BarShiftStrategy3D _barShiftStrategy;

    /// <summary>
    /// The user defined maximum number of items in one direction. Only used if the bar shift strategy is one of the manual values.
    /// </summary>
    private int _barShiftMaxNumberOfItemsInOneDirection = 10;

    /// <summary>
    /// Relative gap between the bars belonging to the same x-value (relative to the width of a single bar).
    /// A value of 0.5 means that the gap has half of the width of one bar.
    /// </summary>
    private double _relInnerGapX = 0.5;

    /// <summary>
    /// Relative gap between the bars between two consecutive x-values (relative to the width of a single bar).
    /// A value of 1 means that the gap has the same width than one bar.
    /// </summary>
    private double _relOuterGapX = 1.0;

    /// <summary>
    /// Relative gap between the bars belonging to the same y-value (relative to the depth of a single bar).
    /// A value of 0.5 means that the gap has half of the depth of one bar.
    /// </summary>
    private double _relInnerGapY = 0.5;

    /// <summary>
    /// Relative gap between the bars between two consecutive y-values (relative to the depth of a single bar).
    /// A value of 1 means that the gap has the same depth than one bar.
    /// </summary>
    private double _relOuterGapY = 1.0;

    /// <summary>
    /// Indicates whether _baseValue is a physical value or a logical value.
    /// </summary>
    private bool _usePhysicalBaseValue;

    /// <summary>
    /// The y-value where the item normally starts. This is either a logical value (_usePhysicalBaseValue==false) or a physical value.
    /// </summary>
    private Altaxo.Data.AltaxoVariant _baseValue = new Altaxo.Data.AltaxoVariant(0.0);

    /// <summary>
    /// If true, the bar starts at the dependent (z) value of the previous plot item.
    /// </summary>
    private bool _startAtPreviousItem;

    /// <summary>
    /// Value in logical units, indicating the gap between previous item an this item.
    /// </summary>
    private double _previousItemZGap;

    /// <summary>
    /// Actual width of the item in logical coordinates.
    /// </summary>
    private double _xSizeLogical;

    /// <summary>
    /// Actual depth of the item in logical coordinates.
    /// </summary>
    private double _ySizeLogical;

    /// <summary>
    /// Actual position of the item in logical coordinates relative to the logical x coordinate of the item's point.
    /// </summary>
    private double _xOffsetLogical;

    /// <summary>
    /// Actual position of the item in logical coordinates relative to the logical y coordinate of the item's point.
    /// </summary>
    private double _yOffsetLogical;

    /// <summary>If this function is set, the color is determined by calling this function on the index into the data.</summary>
    [field: NonSerialized]
    protected Func<int, System.Drawing.Color>? _cachedColorForIndexFunction;

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(BarGraphPlotStyle), 0)]
    private class XmlSerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (BarGraphPlotStyle)obj;
        info.AddValue("UsePhysicalBaseValue", s._usePhysicalBaseValue);
        info.AddValue("BaseValue", (object)s._baseValue);
        info.AddValue("StartAtPrevious", s._startAtPreviousItem);
        info.AddValue("PreviousItemGap", s._previousItemZGap);

        info.AddValue("Pen", s._pen);
        info.AddValue("IndependentColor", s._independentColor);
        info.AddValue("UseUniformCrossSectionThickness", s._useUniformCrossSectionThickness);
        info.AddEnum("BarShift", s._barShiftStrategy);
        info.AddValue("BarShiftMaxItems", s._barShiftMaxNumberOfItemsInOneDirection);
        info.AddValue("InnerGapX", s._relInnerGapX);
        info.AddValue("OuterGapX", s._relOuterGapX);
        info.AddValue("InnerGapY", s._relInnerGapY);
        info.AddValue("OuterGapY", s._relOuterGapY);
      }

      protected virtual BarGraphPlotStyle SDeserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (BarGraphPlotStyle?)o ?? new BarGraphPlotStyle(info);

        s._usePhysicalBaseValue = info.GetBoolean("UsePhysicalBaseValue");
        s._baseValue = (Altaxo.Data.AltaxoVariant)info.GetValue("BaseValue", s);
        s._startAtPreviousItem = info.GetBoolean("StartAtPrevious");
        s._previousItemZGap = info.GetDouble("PreviousItemGap");

        s._pen = (PenX3D)info.GetValue("Pen", s);
        s._independentColor = info.GetBoolean("IndependentColor");
        s._useUniformCrossSectionThickness = info.GetBoolean("UseUniformCrossSectionThickness");
        s._barShiftStrategy = (BarShiftStrategy3D)info.GetEnum("BarShift", typeof(BarShiftStrategy3D));
        s._barShiftMaxNumberOfItemsInOneDirection = info.GetInt32("BarShiftMaxItems");
        s._relInnerGapX = info.GetDouble("InnerGapX");
        s._relOuterGapX = info.GetDouble("OuterGapX");
        s._relInnerGapY = info.GetDouble("InnerGapY");
        s._relOuterGapY = info.GetDouble("OuterGapY");

        return s;
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        BarGraphPlotStyle s = SDeserialize(o, info, parent);
        return s;
      }
    }

    #endregion Serialization

    [MemberNotNull(nameof(_pen))]
    protected void CopyFrom(BarGraphPlotStyle from)
    {
      _usePhysicalBaseValue = from._usePhysicalBaseValue;
      _baseValue = from._baseValue;
      _startAtPreviousItem = from._startAtPreviousItem;
      _previousItemZGap = from._previousItemZGap;

      _independentColor = from._independentColor;
      _pen = from._pen;
      _useUniformCrossSectionThickness = from._useUniformCrossSectionThickness;

      _barShiftStrategy = from._barShiftStrategy;
      _barShiftMaxNumberOfItemsInOneDirection = from._barShiftMaxNumberOfItemsInOneDirection;
      _relInnerGapX = from._relInnerGapX;
      _relOuterGapX = from._relOuterGapX;
      _relInnerGapY = from._relInnerGapY;
      _relOuterGapY = from._relOuterGapY;

      _xSizeLogical = from._xSizeLogical;
      _xOffsetLogical = from._xOffsetLogical;
      _ySizeLogical = from._ySizeLogical;
      _yOffsetLogical = from._yOffsetLogical;
    }


    public virtual bool CopyFrom(object obj, bool copyWithDataReferences)
    {
      if (object.ReferenceEquals(this, obj))
#pragma warning disable CS8774 // Member must have a non-null value when exiting.
        return true;
#pragma warning restore CS8774 // Member must have a non-null value when exiting.

      if (obj is BarGraphPlotStyle from)
      {
        CopyFrom(from);
        EhSelfChanged();
        return true;
      }
      return false;
    }

    /// <summary>
    /// Copies the member variables from another instance.
    /// </summary>
    /// <param name="obj">Another instance to copy the data from.</param>
    /// <returns>True if data was copied, otherwise false.</returns>
    public bool CopyFrom(object obj)
    {
      return CopyFrom(obj, true);
    }

    /// <inheritdoc/>
    public object Clone(bool copyWithDataReferences)
    {
      return new BarGraphPlotStyle(this, copyWithDataReferences);
    }

    /// <inheritdoc/>
    public object Clone()
    {
      return new BarGraphPlotStyle(this, true);
    }

#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
    protected BarGraphPlotStyle(Altaxo.Serialization.Xml.IXmlDeserializationInfo info)
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
    {
    }

    public BarGraphPlotStyle(Altaxo.Main.Properties.IReadOnlyPropertyBag context)
    {
      var color = GraphDocument.GetDefaultPlotColor(context);
      _pen = new PenX3D(color, 1);
    }

    public BarGraphPlotStyle(BarGraphPlotStyle from, bool copyWithDataReferences)
    {
      CopyFrom(from);
    }

    protected override IEnumerable<DocumentNodeAndName> GetDocumentNodeChildrenWithName()
    {
      yield break;
    }

    public BarShiftStrategy3D BarShiftStrategy
    {
      get
      {
        return _barShiftStrategy;
      }
      set
      {
        if (!(_barShiftStrategy == value))
        {
          _barShiftStrategy = value;
          EhSelfChanged();
        }
      }
    }

    public int BarShiftMaxItemsInOneDirection
    {
      get
      {
        return _barShiftMaxNumberOfItemsInOneDirection;
      }
      set
      {
        if (value <= 0)
          throw new ArgumentOutOfRangeException(nameof(value), "Value should be >= 0");

        if (!(_barShiftMaxNumberOfItemsInOneDirection == value))
        {
          _barShiftMaxNumberOfItemsInOneDirection = value;
          EhSelfChanged();
        }
      }
    }

    public bool IsColorReceiver
    {
      get
      {
        return !_independentColor;
      }
    }

    public bool IndependentColor
    {
      get
      {
        return _independentColor;
      }
      set
      {
        var oldValue = _independentColor;
        _independentColor = value;
        if (value != oldValue)
          EhSelfChanged(EventArgs.Empty);
      }
    }

    public PenX3D Pen
    {
      get { return _pen; }
      set
      {
        var oldValue = _pen;
        _pen = value;
        if (!object.ReferenceEquals(value, oldValue))
          EhSelfChanged(EventArgs.Empty);
      }
    }

    public bool UseUniformCrossSectionThickness
    {
      get
      {
        return _useUniformCrossSectionThickness;
      }
      set
      {
        if (!(_useUniformCrossSectionThickness == value))
        {
          _useUniformCrossSectionThickness = value;
          EhSelfChanged();
        }
      }
    }

    public double InnerGapX
    {
      get { return _relInnerGapX; }
      set
      {
        if (!(_relInnerGapX == value))
        {
          _relInnerGapX = value;
          EhSelfChanged();
        }
      }
    }

    public double OuterGapX
    {
      get { return _relOuterGapX; }
      set
      {
        if (!(_relOuterGapX == value))
        {
          _relOuterGapX = value;
          EhSelfChanged();
        }
      }
    }

    public double InnerGapY
    {
      get
      {
        return _relInnerGapY;
      }
      set
      {
        if (!(_relInnerGapY == value))
        {
          _relInnerGapY = value;
          EhSelfChanged();
        }
      }
    }

    public double OuterGapY
    {
      get { return _relOuterGapY; }
      set
      {
        if (!(_relOuterGapY == value))
        {
          _relOuterGapY = value;
          EhSelfChanged();
        }
      }
    }

    public double PreviousItemGapV
    {
      get { return _previousItemZGap; }
      set
      {
        var oldValue = _previousItemZGap;
        _previousItemZGap = value;
        if (value != oldValue)
          EhSelfChanged(EventArgs.Empty);
      }
    }

    public bool StartAtPreviousItem
    {
      get { return _startAtPreviousItem; }
      set
      {
        var oldValue = _startAtPreviousItem;
        _startAtPreviousItem = value;
        if (value != oldValue)
          EhSelfChanged(EventArgs.Empty);
      }
    }

    public bool UsePhysicalBaseValue
    {
      get { return _usePhysicalBaseValue; }
      set
      {
        var oldValue = _usePhysicalBaseValue;
        _usePhysicalBaseValue = value;
        if (value != oldValue)
          EhSelfChanged(EventArgs.Empty);
      }
    }

    public Altaxo.Data.AltaxoVariant BaseValue
    {
      get { return _baseValue; }
      set
      {
        var oldValue = _baseValue;
        _baseValue = value;
        if (oldValue != value)
          EhSelfChanged(EventArgs.Empty);
      }
    }

    #region IG3DPlotStyle Members

    public void CollectExternalGroupStyles(PlotGroupStyleCollection externalGroups)
    {
      BarSizePosition3DGroupStyle.AddExternalGroupStyle(externalGroups);
    }

    public void CollectLocalGroupStyles(PlotGroupStyleCollection externalGroups, PlotGroupStyleCollection localGroups)
    {
      BarSizePosition3DGroupStyle.AddLocalGroupStyle(externalGroups, localGroups);
    }

    public void PrepareGroupStyles(PlotGroupStyleCollection externalGroups, PlotGroupStyleCollection localGroups, IPlotArea layer, Processed3DPlotData pdata)
    {
      // first, we have to calculate the span of logical values from the minimum logical value to the maximum logical value
      int numberOfItems = 0;

      if (pdata?.RangeList is not null)
      {
        double minLogicalX = double.MaxValue;
        double maxLogicalX = double.MinValue;
        double minLogicalY = double.MaxValue;
        double maxLogicalY = double.MinValue;
        foreach (int originalRowIndex in pdata.RangeList.OriginalRowIndices())
        {
          numberOfItems++;

          double logicalX = layer.XAxis.PhysicalVariantToNormal(pdata.GetXPhysical(originalRowIndex));
          if (logicalX < minLogicalX)
            minLogicalX = logicalX;
          if (logicalX > maxLogicalX)
            maxLogicalX = logicalX;

          double logicalY = layer.YAxis.PhysicalVariantToNormal(pdata.GetXPhysical(originalRowIndex));
          if (logicalY < minLogicalY)
            minLogicalY = logicalY;
          if (logicalY > maxLogicalY)
            maxLogicalY = logicalY;
        }

        BarSizePosition3DGroupStyle.IntendToApply(externalGroups, localGroups, numberOfItems, minLogicalX, maxLogicalX, minLogicalY, maxLogicalY);
      }
      var bwp = PlotGroupStyle.GetStyleToInitialize<BarSizePosition3DGroupStyle>(externalGroups, localGroups);
      if (bwp is not null)
        bwp.Initialize(_barShiftStrategy, _barShiftMaxNumberOfItemsInOneDirection, _relInnerGapX, _relOuterGapX, _relInnerGapY, _relOuterGapY);

      if (!_independentColor) // else if is used here because fill color has precedence over frame color
        ColorGroupStyle.PrepareStyle(externalGroups, localGroups, delegate ()
        { return _pen.Color; });
    }

    public void ApplyGroupStyles(PlotGroupStyleCollection externalGroups, PlotGroupStyleCollection localGroups)
    {
      _cachedColorForIndexFunction = null;

      var bwp = PlotGroupStyle.GetStyleToApply<BarSizePosition3DGroupStyle>(externalGroups, localGroups);
      if (bwp is not null)
        bwp.Apply(
          out _barShiftStrategy, out _barShiftMaxNumberOfItemsInOneDirection,
          out _relInnerGapX, out _relOuterGapX, out _xSizeLogical, out _xOffsetLogical,
          out _relInnerGapY, out _relOuterGapY, out _ySizeLogical, out _yOffsetLogical);

      if (!_independentColor)
      {
        ColorGroupStyle.ApplyStyle(externalGroups, localGroups, delegate (NamedColor c)
        { _pen = _pen.WithColor(c); });

        // but if there is a color evaluation function, then use that function with higher priority
        VariableColorGroupStyle.ApplyStyle(externalGroups, localGroups, delegate (Func<int, System.Drawing.Color> evalFunc)
        { _cachedColorForIndexFunction = evalFunc; });
      }
    }

    public void Paint(IGraphicsContext3D g, IPlotArea layer, Processed3DPlotData pdata, Processed3DPlotData? prevItemData, Processed3DPlotData? nextItemData)
    {
      if (pdata is null || !(pdata.RangeList is { } rangeList) || rangeList.Count == 0 || !(pdata.PlotPointsInAbsoluteLayerCoordinates is { } ptArray))
        return;

      layer.CoordinateSystem.LogicalToLayerCoordinates(new Logical3D(0, 0, 0), out var leftFrontBotton);
      layer.CoordinateSystem.LogicalToLayerCoordinates(new Logical3D(1, 1, 1), out var rightBackTop);

      double globalBaseValue;
      if (_usePhysicalBaseValue)
      {
        globalBaseValue = layer.ZAxis.PhysicalVariantToNormal(_baseValue);
        if (double.IsNaN(globalBaseValue))
          globalBaseValue = 0;
      }
      else
      {
        globalBaseValue = _baseValue.ToDouble();
      }

      bool useVariableColor = _cachedColorForIndexFunction is not null && !_independentColor;

      var pen = _pen;

      int j = -1;
      foreach (int originalRowIndex in pdata.RangeList.OriginalRowIndices())
      {
        j++;

        double xCenterLogical = layer.XAxis.PhysicalVariantToNormal(pdata.GetXPhysical(originalRowIndex));
        double xLowerLogical = xCenterLogical + _xOffsetLogical;
        double xUpperLogical = xLowerLogical + _xSizeLogical;

        double yCenterLogical = layer.YAxis.PhysicalVariantToNormal(pdata.GetYPhysical(originalRowIndex));
        double yLowerLogical = yCenterLogical + _yOffsetLogical;
        double yUpperLogical = yLowerLogical + _ySizeLogical;

        double zCenterLogical = layer.ZAxis.PhysicalVariantToNormal(pdata.GetZPhysical(originalRowIndex));
        double zBaseLogical = globalBaseValue;

        if (_startAtPreviousItem && pdata.PreviousItemData is not null)
        {
          double prevstart = layer.ZAxis.PhysicalVariantToNormal(pdata.PreviousItemData.GetZPhysical(originalRowIndex));
          if (!double.IsNaN(prevstart))
          {
            zBaseLogical = prevstart;
            zBaseLogical += Math.Sign(zBaseLogical - globalBaseValue) * _previousItemZGap;
          }
        }

        layer.CoordinateSystem.LogicalToLayerCoordinates(new Logical3D(xLowerLogical, yCenterLogical, zBaseLogical), out var lcp);
        layer.CoordinateSystem.LogicalToLayerCoordinates(new Logical3D(xUpperLogical, yCenterLogical, zBaseLogical), out var ucp);
        layer.CoordinateSystem.LogicalToLayerCoordinates(new Logical3D(xCenterLogical, yLowerLogical, zBaseLogical), out var clp);
        layer.CoordinateSystem.LogicalToLayerCoordinates(new Logical3D(xCenterLogical, yUpperLogical, zBaseLogical), out var cup);

        double penWidth1 = (lcp - ucp).Length;
        double penWidth2 = (clp - cup).Length;

        if (_useUniformCrossSectionThickness)
        {
          pen = pen.WithUniformThickness(Math.Min(penWidth1, penWidth2));
        }
        else
        {
          pen = pen.WithThickness1(penWidth1);
          pen = pen.WithThickness2(penWidth2);
        }

        if (useVariableColor)
          pen = pen.WithColor(GdiColorHelper.ToNamedColor(_cachedColorForIndexFunction!(originalRowIndex), "VariableColor"));

        var isoLine = layer.CoordinateSystem.GetIsoline(new Logical3D(xLowerLogical, yLowerLogical, zBaseLogical), new Logical3D(xLowerLogical, yLowerLogical, zCenterLogical));
        g.DrawLine(pen, isoLine);
      }
    }

    public RectangleD3D PaintSymbol(IGraphicsContext3D g, RectangleD3D bounds)
    {
      bounds = bounds.WithPadding(0, 0, -bounds.SizeZ / 4);
      var heightBy2 = new VectorD3D(0, 0, bounds.SizeZ / 4);

      g.DrawLine(_pen, bounds.Center - heightBy2, bounds.Center + heightBy2);

      return bounds;
    }

    /// <summary>
    /// Prepares the scale of this plot style. Since this style does not utilize a scale, this function does nothing.
    /// </summary>
    /// <param name="layer">The parent layer.</param>
    public void PrepareScales(Graph3D.IPlotArea layer)
    {
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

    /// <inheritdoc/>
    public IEnumerable<(
      string ColumnLabel, // Column label
      IReadableColumn? Column, // the column as it was at the time of this call
      string? ColumnName, // the name of the column (last part of the column proxies document path)
      Action<IReadableColumn?> ColumnSetAction // action to set the column during Apply of the controller
      )> GetAdditionallyUsedColumns()
    {
      yield break;
    }

    #endregion IDocumentNode Members
  }
}
