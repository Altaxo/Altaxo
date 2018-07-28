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

using Altaxo.Serialization;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Altaxo.Graph.Gdi.Plot.Styles
{
  using Altaxo.Data;
  using Altaxo.Main;
  using Drawing;
  using Geometry;
  using Graph.Plot.Data;
  using Graph.Plot.Groups;
  using Plot.Data;
  using Plot.Groups;
  using System.Drawing.Drawing2D;

  public class DropLinePlotStyle
    :
    Main.SuspendableDocumentNodeWithEventArgs,
    IG2DPlotStyle,
    IRoutedPropertyReceiver
  {
    /// <summary>A value indicating whether the skip frequency value is independent from other values.</summary>
    protected bool _independentSkipFrequency;

    /// <summary>A value of 2 skips every other data point, a value of 3 skips 2 out of 3 data points, and so on.</summary>
    protected int _skipFrequency;

    /// <summary>
    /// If true, treat missing points as if not present (e.g. connect lines over missing points, count skip seamlessly over missing points)
    /// </summary>
    protected bool _ignoreMissingDataPoints;

    /// <summary>If true, group styles that shift the logical position of the items (for instance <see cref="BarSizePosition3DGroupStyle"/>) are not applied. I.e. when true, the position of the item remains unperturbed.</summary>
    private bool _independentOnShiftingGroupStyles = true;

    /// <summary>Target(s) for the drop line.</summary>
    protected CSPlaneIDList _dropTargets;

    protected bool _additionalDropTargetIsEnabled;

    protected int _additionalDropTargetPerpendicularAxis = 0;

    /// <summary>
    /// Indicates whether _baseValue is a physical value or a logical value.
    /// </summary>
    private bool _additionalDropTargetUsePhysicalBaseValue;

    /// <summary>
    /// The y-value where the item normally starts. This is either a logical value (_usePhysicalBaseValue==false) or a physical value.
    /// </summary>
    private Altaxo.Data.AltaxoVariant _additionalDropTargetBaseValue = new Altaxo.Data.AltaxoVariant(0.0);

    /// <summary>Pen for the drop line.</summary>
    protected PenX _pen;

    /// <summary>Is the material color independent, i.e. not influenced by group styles.</summary>
    protected bool _independentColor;

    /// <summary>
    /// true if the symbol size is independent, i.e. is not published nor updated by a group style.
    /// </summary>
    private bool _independentSymbolSize;

    /// <summary>Controls the width of the lines and the gap.</summary>
    private double _symbolSize;

    /// <summary>Pen width 1 for the drop line, either absolute or relative to the size of the scatter symbol.</summary>
    protected double _lineWidth1Offset;

    protected double _lineWidth1Factor;

    protected double _gapAtStartOffset;
    protected double _gapAtStartFactor = 1.25;

    protected double _gapAtEndOffset;
    protected double _gapAtEndFactor;

    /// <summary>The symbol size that is received from another substyle.</summary>
    protected double _cachedSymbolSize = 10;

    // cached values:
    /// <summary>If this function is set, then _symbolSize is ignored and the symbol size is evaluated by this function.</summary>
    [field: NonSerialized]
    protected Func<int, double> _cachedSymbolSizeForIndexFunction;

    /// <summary>If this function is set, the symbol color is determined by calling this function on the index into the data.</summary>
    [field: NonSerialized]
    protected Func<int, Color> _cachedColorForIndexFunction;

    /// <summary>Logical x shift between the location of the real data point and the point where the item is finally drawn.</summary>
    private double _cachedLogicalShiftX;

    /// <summary>Logical y shift between the location of the real data point and the point where the item is finally drawn.</summary>
    private double _cachedLogicalShiftY;

    #region Serialization

    /// <summary>
    /// 2016-08-09 initial version.
    /// </summary>
    /// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(DropLinePlotStyle), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        DropLinePlotStyle s = (DropLinePlotStyle)obj;

        info.AddValue("IndependentSkipFreq", s._independentSkipFrequency);
        info.AddValue("SkipFreq", s._skipFrequency);
        info.AddValue("IgnoreMissingDataPoints", s._ignoreMissingDataPoints);
        info.AddValue("IndependentOnShiftingGroupStyles", s._independentOnShiftingGroupStyles);

        info.AddValue("DropTargets", s._dropTargets);
        info.AddValue("HasAdditionalDropTarget", s._additionalDropTargetIsEnabled);
        if (s._additionalDropTargetIsEnabled)
        {
          info.AddValue("AdditionalDropTargetAxis", s._additionalDropTargetPerpendicularAxis);
          info.AddValue("AdditionalDropTargetUsePhysicalValue", s._additionalDropTargetUsePhysicalBaseValue);
          info.AddValue("AdditionalDropTargetBaseValue", (object)s._additionalDropTargetBaseValue);
        }

        info.AddValue("Pen", s._pen);
        info.AddValue("IndependentColor", s._independentColor);

        info.AddValue("IndependentSymbolSize", s._independentSymbolSize);
        info.AddValue("SymbolSize", s._symbolSize);
        info.AddValue("LineWidth1Offset", s._lineWidth1Offset);
        info.AddValue("LineWidth1Factor", s._lineWidth1Factor);
        info.AddValue("GapAtStartOffset", s._gapAtStartOffset);
        info.AddValue("GapAtStartFactor", s._gapAtStartFactor);
        info.AddValue("GapAtEndOffset", s._gapAtEndOffset);
        info.AddValue("GapAtEndFactor", s._gapAtEndFactor);
      }

      protected virtual DropLinePlotStyle SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        DropLinePlotStyle s = null != o ? (DropLinePlotStyle)o : new DropLinePlotStyle(info);

        s._independentSkipFrequency = info.GetBoolean("IndependentSkipFreq");
        s._skipFrequency = info.GetInt32("SkipFreq");
        s._ignoreMissingDataPoints = info.GetBoolean("IgnoreMissingDataPoints");
        s._independentOnShiftingGroupStyles = info.GetBoolean("IndependentOnShiftingGroupStyles");

        s._dropTargets = (CSPlaneIDList)info.GetValue("DropLine", s);
        s._additionalDropTargetIsEnabled = info.GetBoolean("HasAdditionalDropTarget");
        if (s._additionalDropTargetIsEnabled)
        {
          s._additionalDropTargetPerpendicularAxis = info.GetInt32("AdditionalDropTargetAxis");
          s._additionalDropTargetUsePhysicalBaseValue = info.GetBoolean("AdditionalDropTargetUsePhysicalValue");
          s._additionalDropTargetBaseValue = (Altaxo.Data.AltaxoVariant)info.GetValue("AdditionalDropTargetBaseValue", s);
        }

        s.ChildSetMember(ref s._pen, (PenX)info.GetValue("Pen", s) ?? new PenX(NamedColors.Black, 1));
        s._independentColor = info.GetBoolean("IndependentColor");

        s._independentSymbolSize = info.GetBoolean("IndependentSymbolSize");
        s._symbolSize = info.GetDouble("SymbolSize");

        s._lineWidth1Offset = info.GetDouble("LineWidth1Offset");
        s._lineWidth1Factor = info.GetDouble("LineWidth1Factor");

        s._gapAtStartOffset = info.GetDouble("GapAtStartOffset");
        s._gapAtStartFactor = info.GetDouble("GapAtStartFactor");
        s._gapAtEndOffset = info.GetDouble("GapAtEndOffset");
        s._gapAtEndFactor = info.GetDouble("GapAtEndFactor");
        return s;
      }

      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        DropLinePlotStyle s = SDeserialize(o, info, parent);

        // restore the cached values
        s.SetCachedValues();

        return s;
      }
    }

    /// <summary>
    /// Deserialization constructor.
    /// </summary>
    /// <param name="info">The deserialization information.</param>
    protected DropLinePlotStyle(Altaxo.Serialization.Xml.IXmlDeserializationInfo info)
    {
    }

    #endregion Serialization

    public bool CopyFrom(object obj, bool copyWithDataReferences)
    {
      if (object.ReferenceEquals(this, obj))
        return true;
      var from = obj as DropLinePlotStyle;
      if (null != from)
      {
        CopyFrom(from, Main.EventFiring.Enabled);
        return true;
      }
      return false;
    }

    public void CopyFrom(DropLinePlotStyle from, Main.EventFiring eventFiring)
    {
      if (object.ReferenceEquals(this, from))
        return;

      using (var suspendToken = SuspendGetToken())
      {
        this._independentSkipFrequency = from._independentSkipFrequency;
        this._skipFrequency = from._skipFrequency;
        this._ignoreMissingDataPoints = from._ignoreMissingDataPoints;
        this._independentOnShiftingGroupStyles = from._independentOnShiftingGroupStyles;

        this._dropTargets = from._dropTargets; // immutable

        this._additionalDropTargetIsEnabled = from._additionalDropTargetIsEnabled;
        this._additionalDropTargetPerpendicularAxis = from._additionalDropTargetPerpendicularAxis;
        this._additionalDropTargetUsePhysicalBaseValue = from._additionalDropTargetUsePhysicalBaseValue;
        this._additionalDropTargetBaseValue = from._additionalDropTargetBaseValue;

        ChildCopyToMember(ref _pen, from._pen);

        this._independentColor = from._independentColor;

        _independentSymbolSize = from._independentSymbolSize;
        _symbolSize = from._symbolSize;
        _lineWidth1Offset = from._lineWidth1Offset;
        _lineWidth1Factor = from._lineWidth1Factor;

        this._gapAtStartOffset = from._gapAtStartOffset;
        this._gapAtStartFactor = from._gapAtStartFactor;
        this._gapAtEndOffset = from._gapAtEndOffset;
        this._gapAtEndFactor = from._gapAtEndFactor;

        EhSelfChanged(EventArgs.Empty);

        suspendToken.Resume(eventFiring);
      }
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
      return new DropLinePlotStyle(this);
    }

    /// <inheritdoc/>
    public object Clone()
    {
      return new DropLinePlotStyle(this);
    }

    public DropLinePlotStyle(DropLinePlotStyle from)
    {
      CopyFrom(from, Main.EventFiring.Suppressed);
    }

    public DropLinePlotStyle(CSPlaneID planeID, PenX pen)
    {
      if (null == pen)
        throw new ArgumentNullException(nameof(pen));

      ChildSetMember(ref _pen, pen);
      this._dropTargets = new CSPlaneIDList(new[] { planeID });

      // Cached values
      SetCachedValues();
    }

    public DropLinePlotStyle(IEnumerable<CSPlaneID> planeIDs, PenX pen)
    {
      if (null == pen)
        throw new ArgumentNullException(nameof(pen));

      ChildSetMember(ref _pen, pen);
      this._dropTargets = new CSPlaneIDList(planeIDs);

      // Cached values
      SetCachedValues();
    }

    public DropLinePlotStyle(Altaxo.Main.Properties.IReadOnlyPropertyBag context)
    {
      this._dropTargets = new CSPlaneIDList(new[] { new CSPlaneID(1, 0) });

      var color = GraphDocument.GetDefaultPlotColor(context);
      double penWidth = GraphDocument.GetDefaultPenWidth(context);
      _pen = new PenX(color, penWidth) { ParentObject = this };

      _lineWidth1Offset = penWidth;
      _lineWidth1Factor = 0;
    }

    protected override IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
    {
      if (null != _pen)
        yield return new DocumentNodeAndName(_pen, () => _pen = null, "Pen");
    }

    public bool IsVisible
    {
      get
      {
        return _pen.IsVisible;
      }
    }

    public PenX Pen
    {
      get { return this._pen; }
      set
      {
        if (null == value)
          throw new ArgumentNullException(nameof(value));

        if (!object.ReferenceEquals(this._pen, value))
        {
          ChildCopyToMember(ref _pen, value);
          SetCachedValues();
          EhSelfChanged(EventArgs.Empty); // Fire Changed event
        }
      }
    }

    public NamedColor Color
    {
      get { return this._pen.Color; }
      set
      {
        _pen.Color = value;
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
        bool oldValue = _independentColor;
        _independentColor = value;
        if (value != oldValue)
          EhSelfChanged(EventArgs.Empty);
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
        if (!(_independentSymbolSize == value))
        {
          _independentSymbolSize = value;
          EhSelfChanged(EventArgs.Empty);
        }
      }
    }

    /// <summary>Controls the width of the line and the line gap.</summary>
    public double SymbolSize
    {
      get { return _symbolSize; }
      set
      {
        if (!Calc.RMath.IsFinite(value))
          throw new ArgumentException(nameof(value), "Value must be a finite number");

        if (!(_symbolSize == value))
        {
          _symbolSize = value;
          EhSelfChanged();
        }
      }
    }

    public double LineWidth1Offset
    {
      get
      {
        return _lineWidth1Offset;
      }
      set
      {
        if (!(_lineWidth1Offset == value))
        {
          _lineWidth1Offset = value;
          EhSelfChanged();
        }
      }
    }

    public double LineWidth1Factor
    {
      get
      {
        return _lineWidth1Factor;
      }
      set
      {
        if (!(_lineWidth1Factor == value))
        {
          _lineWidth1Factor = value;
          EhSelfChanged();
        }
      }
    }

    public double GapAtStartOffset
    {
      get
      {
        return _gapAtStartOffset;
      }
      set
      {
        if (!(_gapAtStartOffset == value))
        {
          _gapAtStartOffset = value;
          EhSelfChanged();
        }
      }
    }

    public double GapAtStartFactor
    {
      get
      {
        return _gapAtStartFactor;
      }
      set
      {
        if (!(_gapAtStartFactor == value))
        {
          _gapAtStartFactor = value;
          EhSelfChanged();
        }
      }
    }

    public double GapAtEndOffset
    {
      get
      {
        return _gapAtEndOffset;
      }
      set
      {
        if (!(_gapAtEndOffset == value))
        {
          _gapAtEndOffset = value;
          EhSelfChanged();
        }
      }
    }

    public double GapAtEndFactor
    {
      get
      {
        return _gapAtEndFactor;
      }
      set
      {
        if (!(_gapAtEndFactor == value))
        {
          _gapAtEndFactor = value;
          EhSelfChanged();
        }
      }
    }

    public int SkipFrequency
    {
      get { return _skipFrequency; }
      set
      {
        value = Math.Max(1, value);
        if (value != _skipFrequency)
        {
          _skipFrequency = value;
          EhSelfChanged(EventArgs.Empty); // Fire Changed event
        }
      }
    }

    public bool IndependentSkipFrequency
    {
      get
      {
        return _independentSkipFrequency;
      }
      set
      {
        bool oldValue = _independentSkipFrequency;
        _independentSkipFrequency = value;
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
        if (!(_ignoreMissingDataPoints == value))
        {
          _ignoreMissingDataPoints = value;
          EhSelfChanged(EventArgs.Empty);
        }
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

    /// <summary>
    /// Gets the cached size of the scatter symbol.
    /// </summary>
    /// <value>
    /// The size of the scatter symbol.
    /// </value>
    public double CachedSymbolSize
    {
      get
      {
        return _cachedSymbolSize;
      }
    }

    public CSPlaneIDList DropTargets
    {
      get
      {
        return _dropTargets;
      }
      set
      {
        if (null == value)
          throw new ArgumentNullException(nameof(value));

        if (!object.ReferenceEquals(value, _dropTargets))
        {
          _dropTargets = value;
          EhSelfChanged();
        }
      }
    }

    public bool AdditionalDropTargetIsEnabled
    {
      get { return _additionalDropTargetIsEnabled; }
      set
      {
        if (!(_additionalDropTargetIsEnabled == value))
        {
          _additionalDropTargetIsEnabled = value;
          EhSelfChanged();
        }
      }
    }

    public bool AdditionalDropTargetUsePhysicalBaseValue
    {
      get
      {
        return _additionalDropTargetUsePhysicalBaseValue;
      }
      set
      {
        if (!(_additionalDropTargetUsePhysicalBaseValue == value))
        {
          _additionalDropTargetUsePhysicalBaseValue = value;
          EhSelfChanged();
        }
      }
    }

    public int AdditionalDropTargetPerpendicularAxisNumber
    {
      get
      {
        return _additionalDropTargetPerpendicularAxis;
      }
      set
      {
        if (!(_additionalDropTargetPerpendicularAxis == value))
        {
          _additionalDropTargetPerpendicularAxis = value;
          EhSelfChanged();
        }
      }
    }

    public AltaxoVariant AdditionalDropTargetBaseValue
    {
      get
      {
        return _additionalDropTargetBaseValue;
      }
      set
      {
        if (!(_additionalDropTargetBaseValue == value))
        {
          _additionalDropTargetBaseValue = value;
          EhSelfChanged();
        }
      }
    }

    protected void SetCachedValues()
    {
    }

    #region I3DPlotItem Members

    public bool IsColorProvider
    {
      get
      {
        return !this._independentColor;
      }
    }

    public bool IsColorReceiver
    {
      get { return !this._independentColor; }
    }

    public bool IsSymbolSizeProvider
    {
      get
      {
        return false;
      }
    }

    public bool IsSymbolSizeReceiver
    {
      get
      {
        return true;
      }
    }

    #endregion I3DPlotItem Members

    public void Paint(Graphics g, IPlotArea layer, Processed2DPlotData pdata, Processed2DPlotData prevItemData, Processed2DPlotData nextItemData)
    {
      // adjust the skip frequency if it was not set appropriate
      if (_skipFrequency <= 0)
        _skipFrequency = 1;

      if (_independentOnShiftingGroupStyles)
      {
        _cachedLogicalShiftX = _cachedLogicalShiftY = 0;
      }

      PlotRangeList rangeList = pdata.RangeList;

      if (this._ignoreMissingDataPoints)
      {
        // in case we ignore the missing points, all ranges can be plotted
        // as one range, i.e. continuously
        // for this, we create the totalRange, which contains all ranges
        var totalRange = new PlotRangeCompound(rangeList);
        this.PaintOneRange(g, layer, totalRange, pdata);
      }
      else // we not ignore missing points, so plot all ranges separately
      {
        for (int i = 0; i < rangeList.Count; i++)
        {
          this.PaintOneRange(g, layer, rangeList[i], pdata);
        }
      }
    }

    private void PaintOneRange(Graphics g, IPlotArea layer, IPlotRange range, Processed2DPlotData pdata)
    {
      // adjust the skip frequency if it was not set appropriate
      if (_skipFrequency <= 0)
        _skipFrequency = 1;

      var dropTargets = new List<CSPlaneID>(_dropTargets.Select(id => layer.UpdateCSPlaneID(id)));
      if (_additionalDropTargetIsEnabled)
      {
        CSPlaneID userPlane;
        if (_additionalDropTargetUsePhysicalBaseValue)
        {
          userPlane = new CSPlaneID(_additionalDropTargetPerpendicularAxis, layer.Scales[_additionalDropTargetPerpendicularAxis].PhysicalVariantToNormal(_additionalDropTargetBaseValue));
        }
        else
        {
          userPlane = new CSPlaneID(_additionalDropTargetPerpendicularAxis, _additionalDropTargetBaseValue);
        }
        dropTargets.Add(userPlane);
      }

      // paint the scatter style

      PointD3D pos = PointD3D.Empty;
      var gpath = new GraphicsPath();

      if (null == _cachedSymbolSizeForIndexFunction && null == _cachedColorForIndexFunction) // using a constant symbol size and constant color
      {
        // update pen widths
        var pen = _pen.Clone();
        double w1 = _lineWidth1Offset + _lineWidth1Factor * _cachedSymbolSize;
        pen.Width = w1;

        var gapStart = 0.5 * (_gapAtStartOffset + _gapAtStartFactor * _cachedSymbolSize);
        var gapEnd = 0.5 * (_gapAtEndOffset + _gapAtEndFactor * _cachedSymbolSize);

        int lower = range.LowerBound;
        int upper = range.UpperBound;

        for (int j = lower; j < upper; j += _skipFrequency)
        {
          var originalRowIndex = range.GetOriginalRowIndexFromPlotPointIndex(j);

          Logical3D r3d = layer.GetLogical3D(pdata, originalRowIndex);
          r3d.RX += _cachedLogicalShiftX;
          r3d.RY += _cachedLogicalShiftY;

          foreach (CSPlaneID id in dropTargets)
          {
            gpath.Reset();
            layer.CoordinateSystem.GetIsolineFromPointToPlane(gpath, r3d, id);
            PointF[] shortenedPathPoints = null;
            if (gapStart != 0 || gapEnd != 0)
            {
              gpath.Flatten();
              var pathPoints = gpath.PathPoints;
              shortenedPathPoints = GdiExtensionMethods.ShortenedBy(pathPoints, RADouble.NewAbs(gapStart), RADouble.NewAbs(gapEnd));
              if (null != shortenedPathPoints)
                g.DrawLines(pen, shortenedPathPoints);
            }
            else
            {
              g.DrawPath(pen, gpath);
            }
          }
        }
      }
      else // using a variable symbol size or variable symbol color
      {
        int lower = range.LowerBound;
        int upper = range.UpperBound;
        for (int j = lower; j < upper; j += _skipFrequency)
        {
          var originalRowIndex = range.GetOriginalRowIndexFromPlotPointIndex(j);
          var pen = _pen.Clone();
          if (null == _cachedColorForIndexFunction)
          {
            _cachedSymbolSize = _cachedSymbolSizeForIndexFunction(originalRowIndex);
            double w1 = _lineWidth1Offset + _lineWidth1Factor * _cachedSymbolSize;
            pen.Width = w1;
          }
          else
          {
            _cachedSymbolSize = null == _cachedSymbolSizeForIndexFunction ? _cachedSymbolSize : _cachedSymbolSizeForIndexFunction(originalRowIndex);
            double w1 = _lineWidth1Offset + _lineWidth1Factor * _cachedSymbolSize;

            var customSymbolColor = _cachedColorForIndexFunction(originalRowIndex);
            pen.Width = w1;
            pen.Color = NamedColor.FromArgb(customSymbolColor.A, customSymbolColor.R, customSymbolColor.G, customSymbolColor.B);
          }

          var gapStart = 0.5 * (_gapAtStartOffset + _gapAtStartFactor * _cachedSymbolSize);
          var gapEnd = 0.5 * (_gapAtEndOffset + _gapAtEndFactor * _cachedSymbolSize);

          Logical3D r3d = layer.GetLogical3D(pdata, originalRowIndex);
          r3d.RX += _cachedLogicalShiftX;
          r3d.RY += _cachedLogicalShiftY;

          foreach (CSPlaneID id in _dropTargets)
          {
            gpath.Reset();
            layer.CoordinateSystem.GetIsolineFromPointToPlane(gpath, r3d, id);
            PointF[] shortenedPathPoints = null;
            if (gapStart != 0 || gapEnd != 0)
            {
              gpath.Flatten();
              var pathPoints = gpath.PathPoints;
              shortenedPathPoints = GdiExtensionMethods.ShortenedBy(pathPoints, RADouble.NewAbs(gapStart), RADouble.NewAbs(gapEnd));
              if (null != shortenedPathPoints)
                g.DrawLines(pen, shortenedPathPoints);
            }
            else
            {
              g.DrawPath(pen, gpath);
            }
          }
        }
      }
    }

    public RectangleF PaintSymbol(Graphics g, RectangleF bounds)
    {
      return RectangleF.Empty;
    }

    /// <summary>
    /// Prepares the scale of this plot style. Since this style does not utilize a scale, this function does nothing.
    /// </summary>
    /// <param name="layer">The parent layer.</param>
    public void PrepareScales(IPlotArea layer)
    {
    }

    #region IPlotStyle Members

    public void CollectExternalGroupStyles(PlotGroupStyleCollection externalGroups)
    {
      if (this.IsColorProvider)
        ColorGroupStyle.AddExternalGroupStyle(externalGroups);

      ScatterSymbolGroupStyle.AddExternalGroupStyle(externalGroups);
    }

    public void CollectLocalGroupStyles(PlotGroupStyleCollection externalGroups, PlotGroupStyleCollection localGroups)
    {
      ColorGroupStyle.AddLocalGroupStyle(externalGroups, localGroups);

      if (!_independentSkipFrequency)
        SkipFrequencyGroupStyle.AddLocalGroupStyle(externalGroups, localGroups); // (local group style only)

      IgnoreMissingDataPointsGroupStyle.AddLocalGroupStyle(externalGroups, localGroups);
    }

    public void PrepareGroupStyles(PlotGroupStyleCollection externalGroups, PlotGroupStyleCollection localGroups, IPlotArea layer, Processed2DPlotData pdata)
    {
      if (this.IsColorProvider)
        ColorGroupStyle.PrepareStyle(externalGroups, localGroups, delegate ()
        { return this.Color; });

      // SkipFrequency should be the same for all sub plot styles, so there is no "private" property
      if (!_independentSkipFrequency)
        SkipFrequencyGroupStyle.PrepareStyle(externalGroups, localGroups, delegate ()
        { return _skipFrequency; });

      // IgnoreMissingDataPoints should be the same for all sub plot styles, so there is no "private" property
      IgnoreMissingDataPointsGroupStyle.PrepareStyle(externalGroups, localGroups, () => _ignoreMissingDataPoints);
    }

    public void ApplyGroupStyles(PlotGroupStyleCollection externalGroups, PlotGroupStyleCollection localGroups)
    {
      // IgnoreMissingDataPoints is the same for all sub plot styles
      IgnoreMissingDataPointsGroupStyle.ApplyStyle(externalGroups, localGroups, (ignoreMissingDataPoints) => this._ignoreMissingDataPoints = ignoreMissingDataPoints);

      if (this.IsColorReceiver)
      {
        // try to get a constant color ...
        ColorGroupStyle.ApplyStyle(externalGroups, localGroups, delegate (NamedColor c)
        { this.Color = c; });
        // but if there is a color evaluation function, then use that function with higher priority
        if (!VariableColorGroupStyle.ApplyStyle(externalGroups, localGroups, delegate (Func<int, Color> evalFunc)
        { _cachedColorForIndexFunction = evalFunc; }))
          _cachedColorForIndexFunction = null;
      }

      // try to get the symbol size ...
      if (!_independentSymbolSize)
      {
        _cachedSymbolSize = 0;
        SymbolSizeGroupStyle.ApplyStyle(externalGroups, localGroups, delegate (double size)
        { this._cachedSymbolSize = size; });
        // but if there is an symbol size evaluation function, then use this with higher priority.
        if (!VariableSymbolSizeGroupStyle.ApplyStyle(externalGroups, localGroups, delegate (Func<int, double> evalFunc)
        { _cachedSymbolSizeForIndexFunction = evalFunc; }))
          _cachedSymbolSizeForIndexFunction = null;
      }
      else
      {
        _cachedSymbolSize = _symbolSize;
      }

      // SkipFrequency should be the same for all sub plot styles, so there is no "private" property
      if (!_independentSkipFrequency)
      {
        _skipFrequency = 1;
        SkipFrequencyGroupStyle.ApplyStyle(externalGroups, localGroups, delegate (int c)
        { this._skipFrequency = c; });
      }

      // Shift the items ?
      _cachedLogicalShiftX = 0;
      _cachedLogicalShiftY = 0;
      if (!_independentOnShiftingGroupStyles)
      {
        var shiftStyle = PlotGroupStyle.GetFirstStyleToApplyImplementingInterface<IShiftLogicalXYGroupStyle>(externalGroups, localGroups);
        if (null != shiftStyle)
        {
          shiftStyle.Apply(out _cachedLogicalShiftX, out _cachedLogicalShiftY);
        }
      }
    }

    #endregion IPlotStyle Members

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
      IReadableColumn Column, // the column as it was at the time of this call
      string ColumnName, // the name of the column (last part of the column proxies document path)
      Action<IReadableColumn> ColumnSetAction // action to set the column during Apply of the controller
      )> GetAdditionallyUsedColumns()
    {
      return null; // no additionally used columns
    }

    #endregion IDocumentNode Members

    #region IRoutedPropertyReceiver Members

    public IEnumerable<(string PropertyName, object PropertyValue, Action<object> PropertySetter)> GetRoutedProperties(string propertyName)
    {
      switch (propertyName)
      {
        case "StrokeWidth":
          yield return (propertyName, _pen.Width, (w) => _pen.Width = (double)w);
          break;

        case "SymbolSize":
          if (_independentSymbolSize)
            yield return (propertyName, _symbolSize, (w) => SymbolSize = (double)w);
          break;
      }

      yield break;
    }

    #endregion IRoutedPropertyReceiver Members
  }
}
