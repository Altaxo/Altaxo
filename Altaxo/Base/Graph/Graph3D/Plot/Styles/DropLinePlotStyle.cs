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
using System.Drawing;
using System.Linq;
using Altaxo.Serialization;

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

  public class DropLinePlotStyle
    :
    Main.SuspendableDocumentNodeWithEventArgs,
    IG3DPlotStyle
  {
    /// <summary>A value indicating whether the skip frequency value is independent from other values.</summary>
    protected bool _independentSkipFreq;

    /// <summary>A value of 2 skips every other data point, a value of 3 skips 2 out of 3 data points, and so on.</summary>
    protected int _skipFreq;

    /// <summary>Target(s) for the drop line.</summary>
    protected CSPlaneIDList _dropTargets;

    protected bool _additionalDropTargetIsEnabled;

    protected int _additionalDropTargetPerpendicularAxis = 2;

    /// <summary>
    /// Indicates whether _baseValue is a physical value or a logical value.
    /// </summary>
    private bool _additionalDropTargetUsePhysicalBaseValue;

    /// <summary>
    /// The y-value where the item normally starts. This is either a logical value (_usePhysicalBaseValue==false) or a physical value.
    /// </summary>
    private Altaxo.Data.AltaxoVariant _additionalDropTargetBaseValue = new Altaxo.Data.AltaxoVariant(0.0);

    /// <summary>Pen for the drop line.</summary>
    protected PenX3D _pen;

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

    /// <summary>Pen width 2 for the drop line, either absolute or relative to the size of the scatter symbol.</summary>
    protected double _lineWidth2Offset;

    protected double _lineWidth2Factor;

    protected double _gapAtStartOffset;
    protected double _gapAtStartFactor = 1.25;

    protected double _gapAtEndOffset;
    protected double _gapAtEndFactor;

    /// <summary>The symbol size that is received from another substyle.</summary>
    protected double _cachedSymbolSize = 10;

    // cached values:
    /// <summary>If this function is set, then _symbolSize is ignored and the symbol size is evaluated by this function.</summary>
    [field: NonSerialized]
    protected Func<int, double>? _cachedSymbolSizeForIndexFunction;

    /// <summary>If this function is set, the symbol color is determined by calling this function on the index into the data.</summary>
    [field: NonSerialized]
    protected Func<int, Color>? _cachedColorForIndexFunction;

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
        var s = (DropLinePlotStyle)obj;

        info.AddValue("IndependentSkipFreq", s._independentSkipFreq);
        info.AddValue("SkipFreq", s._skipFreq);
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
        info.AddValue("LineWidth2Offset", s._lineWidth2Offset);
        info.AddValue("LineWidth2Factor", s._lineWidth2Factor);
        info.AddValue("GapAtStartOffset", s._gapAtStartOffset);
        info.AddValue("GapAtStartFactor", s._gapAtStartFactor);
        info.AddValue("GapAtEndOffset", s._gapAtEndOffset);
        info.AddValue("GapAtEndFactor", s._gapAtEndFactor);
      }

      protected virtual DropLinePlotStyle SDeserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (DropLinePlotStyle?)o ?? new DropLinePlotStyle(info);

        s._independentSkipFreq = info.GetBoolean("IndependentSkipFreq");
        s._skipFreq = info.GetInt32("SkipFreq");
        s._dropTargets = (CSPlaneIDList)info.GetValue("DropLine", s);
        s._additionalDropTargetIsEnabled = info.GetBoolean("HasAdditionalDropTarget");
        if (s._additionalDropTargetIsEnabled)
        {
          s._additionalDropTargetPerpendicularAxis = info.GetInt32("AdditionalDropTargetAxis");
          s._additionalDropTargetUsePhysicalBaseValue = info.GetBoolean("AdditionalDropTargetUsePhysicalValue");
          s._additionalDropTargetBaseValue = (Altaxo.Data.AltaxoVariant)info.GetValue("AdditionalDropTargetBaseValue", s);
        }

        s._pen = (PenX3D)info.GetValue("Pen", s);
        s._independentColor = info.GetBoolean("IndependentColor");

        s._independentSymbolSize = info.GetBoolean("IndependentSymbolSize");
        s._symbolSize = info.GetDouble("SymbolSize");

        s._lineWidth1Offset = info.GetDouble("LineWidth1Offset");
        s._lineWidth1Factor = info.GetDouble("LineWidth1Factor");

        s._lineWidth2Offset = info.GetDouble("LineWidth2Offset");
        s._lineWidth2Factor = info.GetDouble("LineWidth2Factor");

        s._gapAtStartOffset = info.GetDouble("GapAtStartOffset");
        s._gapAtStartFactor = info.GetDouble("GapAtStartFactor");
        s._gapAtEndOffset = info.GetDouble("GapAtEndOffset");
        s._gapAtEndFactor = info.GetDouble("GapAtEndFactor");
        return s;
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
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
#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
    protected DropLinePlotStyle(Altaxo.Serialization.Xml.IXmlDeserializationInfo info)
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
    {
    }

    #endregion Serialization

    public bool CopyFrom(object obj, bool copyWithDataReferences)
    {
      if (ReferenceEquals(this, obj))
        return true;
      if (obj is DropLinePlotStyle from)
      {
        CopyFrom(from, Main.EventFiring.Enabled);
        return true;
      }
      return false;
    }


    [MemberNotNull(nameof(_dropTargets), nameof(_pen))]
    public void CopyFrom(DropLinePlotStyle from, Main.EventFiring eventFiring)
    {
      if (ReferenceEquals(this, from))
#pragma warning disable CS8774 // Member must have a non-null value when exiting.
        return;
#pragma warning restore CS8774 // Member must have a non-null value when exiting.

      using (var suspendToken = SuspendGetToken())
      {
        _independentSkipFreq = from._independentSkipFreq;
        _skipFreq = from._skipFreq;
        _dropTargets = from._dropTargets; // immutable

        _additionalDropTargetIsEnabled = from._additionalDropTargetIsEnabled;
        _additionalDropTargetPerpendicularAxis = from._additionalDropTargetPerpendicularAxis;
        _additionalDropTargetUsePhysicalBaseValue = from._additionalDropTargetUsePhysicalBaseValue;
        _additionalDropTargetBaseValue = from._additionalDropTargetBaseValue;

        _pen = from._pen; // immutable
        _independentColor = from._independentColor;

        _independentSymbolSize = from._independentSymbolSize;
        _symbolSize = from._symbolSize;
        _lineWidth1Offset = from._lineWidth1Offset;
        _lineWidth1Factor = from._lineWidth1Factor;
        _lineWidth2Offset = from._lineWidth2Offset;
        _lineWidth2Factor = from._lineWidth2Factor;

        _gapAtStartOffset = from._gapAtStartOffset;
        _gapAtStartFactor = from._gapAtStartFactor;
        _gapAtEndOffset = from._gapAtEndOffset;
        _gapAtEndFactor = from._gapAtEndFactor;

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
      if (ReferenceEquals(this, obj))
        return true;

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

    public DropLinePlotStyle(CSPlaneID planeID, PenX3D pen)
    {
      if (pen is null)
        throw new ArgumentNullException(nameof(pen));


      _pen = pen;
      _dropTargets = new CSPlaneIDList(new[] { planeID });

      // Cached values
      SetCachedValues();
    }

    public DropLinePlotStyle(Altaxo.Main.Properties.IReadOnlyPropertyBag context)
    {
      _dropTargets = new CSPlaneIDList(new[] { new CSPlaneID(2, 0) });

      var color = GraphDocument.GetDefaultPlotColor(context);
      double penWidth = GraphDocument.GetDefaultPenWidth(context);
      _pen = new PenX3D(color, penWidth);

      _lineWidth1Offset = penWidth;
      _lineWidth1Factor = 0;
      _lineWidth2Offset = penWidth;
      _lineWidth2Factor = 0;
    }

    protected override IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
    {
      yield break;
    }

    public bool IsVisible
    {
      get
      {
        return _pen.Material.IsVisible;
      }
    }

    public PenX3D Pen
    {
      get { return _pen; }
      set
      {
        if (value is null)
          throw new ArgumentNullException(nameof(value));

        if (!object.ReferenceEquals(_pen, value))
        {
          _pen = value;
          SetCachedValues();
          EhSelfChanged(EventArgs.Empty); // Fire Changed event
        }
      }
    }

    public NamedColor Color
    {
      get { return _pen.Material.Color; }
      set
      {
        Pen = _pen.WithColor(value);
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

    public double LineWidth2Offset
    {
      get
      {
        return _lineWidth2Offset;
      }
      set
      {
        if (!(_lineWidth2Offset == value))
        {
          _lineWidth2Offset = value;
          EhSelfChanged();
        }
      }
    }

    public double LineWidth2Factor
    {
      get
      {
        return _lineWidth2Factor;
      }
      set
      {
        if (!(_lineWidth2Factor == value))
        {
          _lineWidth2Factor = value;
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
      get { return _skipFreq; }
      set
      {
        value = Math.Max(1, value);
        if (value != _skipFreq)
        {
          _skipFreq = value;
          EhSelfChanged(EventArgs.Empty); // Fire Changed event
        }
      }
    }

    public bool IndependentSkipFrequency
    {
      get
      {
        return _independentSkipFreq;
      }
      set
      {
        bool oldValue = _independentSkipFreq;
        _independentSkipFreq = value;
        if (value != oldValue)
          EhSelfChanged(EventArgs.Empty);
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
        if (value is null)
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
        return !_independentColor;
      }
    }

    public bool IsColorReceiver
    {
      get { return !_independentColor; }
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

    public void Paint(IGraphicsContext3D g, IPlotArea layer, Processed3DPlotData pdata, Processed3DPlotData? prevItemData, Processed3DPlotData? nextItemData)
    {
      if (pdata is null || !(pdata.RangeList is { } rangeList) || rangeList.Count == 0 || !(pdata.PlotPointsInAbsoluteLayerCoordinates is { } ptArray))
        return;

      // adjust the skip frequency if it was not set appropriate
      if (_skipFreq <= 0)
        _skipFreq = 1;

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

      if (_cachedSymbolSizeForIndexFunction is null && _cachedColorForIndexFunction is null) // using a constant symbol size and constant color
      {
        var pen = _pen;
        // update pen widths
        double w1 = _lineWidth1Offset + _lineWidth1Factor * _cachedSymbolSize;
        double w2 = _lineWidth2Offset + _lineWidth2Factor * _cachedSymbolSize;
        pen = pen.WithThickness1(w1).WithThickness2(w2);

        var gapStart = 0.5 * (_gapAtStartOffset + _gapAtStartFactor * _cachedSymbolSize);
        var gapEnd = 0.5 * (_gapAtEndOffset + _gapAtEndFactor * _cachedSymbolSize);

        for (int r = 0; r < rangeList.Count; r++)
        {
          var range = rangeList[r];
          int lower = range.LowerBound;
          int upper = range.UpperBound;

          for (int j = lower; j < upper; j += _skipFreq)
          {
            Logical3D r3d = layer.GetLogical3D(pdata, j + range.OffsetToOriginal);
            foreach (CSPlaneID id in dropTargets)
            {
              layer.CoordinateSystem.GetIsolineFromPointToPlane(r3d, id, out var isoLine);
              if (gapStart != 0 || gapEnd != 0)
                isoLine = isoLine.ShortenedBy(RADouble.NewAbs(gapStart), RADouble.NewAbs(gapEnd));

              if (isoLine is not null)
                g.DrawLine(pen, isoLine);
            }
          }
        } // for each range
      }
      else // using a variable symbol size or variable symbol color
      {
        for (int r = 0; r < rangeList.Count; r++)
        {
          var range = rangeList[r];
          int lower = range.LowerBound;
          int upper = range.UpperBound;
          int offset = range.OffsetToOriginal;
          for (int j = lower; j < upper; j += _skipFreq)
          {
            var pen = _pen;
            if (_cachedColorForIndexFunction is not null)
            {
              _cachedSymbolSize = _cachedSymbolSizeForIndexFunction is null ? _cachedSymbolSize : _cachedSymbolSizeForIndexFunction(j + offset);
              double w1 = _lineWidth1Offset + _lineWidth1Factor * _cachedSymbolSize;
              double w2 = _lineWidth2Offset + _lineWidth2Factor * _cachedSymbolSize;

              var customSymbolColor = _cachedColorForIndexFunction(j + offset);
              pen = _pen.WithThickness1(w1).WithThickness2(w2).WithColor(NamedColor.FromArgb(customSymbolColor.A, customSymbolColor.R, customSymbolColor.G, customSymbolColor.B));
            }
            else if (_cachedSymbolSizeForIndexFunction is not null)
            {
              _cachedSymbolSize = _cachedSymbolSizeForIndexFunction(j + offset);
              double w1 = _lineWidth1Offset + _lineWidth1Factor * _cachedSymbolSize;
              double w2 = _lineWidth2Offset + _lineWidth2Factor * _cachedSymbolSize;
              pen = _pen.WithThickness1(w1).WithThickness2(w2);
            }


            var gapStart = 0.5 * (_gapAtStartOffset + _gapAtStartFactor * _cachedSymbolSize);
            var gapEnd = 0.5 * (_gapAtEndOffset + _gapAtEndFactor * _cachedSymbolSize);

            Logical3D r3d = layer.GetLogical3D(pdata, j + rangeList[r].OffsetToOriginal);
            foreach (CSPlaneID id in _dropTargets)
            {
              layer.CoordinateSystem.GetIsolineFromPointToPlane(r3d, id, out var isoLine);

              if (gapStart != 0 || gapEnd != 0)
                isoLine = isoLine.ShortenedBy(RADouble.NewAbs(gapStart), RADouble.NewAbs(gapEnd));
              if (isoLine is not null)
                g.DrawLine(pen, isoLine);
            }
          }
        }
      }
    }

    public RectangleD3D PaintSymbol(IGraphicsContext3D g, RectangleD3D bounds)
    {
      return RectangleD3D.Empty;
    }

    /// <summary>
    /// Prepares the scale of this plot style. Since this style does not utilize a scale, this function does nothing.
    /// </summary>
    /// <param name="layer">The parent layer.</param>
    public void PrepareScales(Graph3D.IPlotArea layer)
    {
    }

    #region IPlotStyle Members

    public void CollectExternalGroupStyles(PlotGroupStyleCollection externalGroups)
    {
      if (IsColorProvider)
        ColorGroupStyle.AddExternalGroupStyle(externalGroups);

      ScatterSymbolGroupStyle.AddExternalGroupStyle(externalGroups);
    }

    public void CollectLocalGroupStyles(PlotGroupStyleCollection externalGroups, PlotGroupStyleCollection localGroups)
    {
      ColorGroupStyle.AddLocalGroupStyle(externalGroups, localGroups);
      SkipFrequencyGroupStyle.AddLocalGroupStyle(externalGroups, localGroups); // (local group style only)
    }

    public void PrepareGroupStyles(PlotGroupStyleCollection externalGroups, PlotGroupStyleCollection localGroups, IPlotArea layer, Processed3DPlotData pdata)
    {
      if (IsColorProvider)
        ColorGroupStyle.PrepareStyle(externalGroups, localGroups, delegate ()
        { return Color; });

      // SkipFrequency should be the same for all sub plot styles, so there is no "private" property
      if (!_independentSkipFreq)
        SkipFrequencyGroupStyle.PrepareStyle(externalGroups, localGroups, delegate ()
        { return _skipFreq; });
    }

    public void ApplyGroupStyles(PlotGroupStyleCollection externalGroups, PlotGroupStyleCollection localGroups)
    {
      if (IsColorReceiver)
      {
        // try to get a constant color ...
        ColorGroupStyle.ApplyStyle(externalGroups, localGroups, delegate (NamedColor c)
        { Color = c; });
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
        { _cachedSymbolSize = size; });
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
      if (!_independentSkipFreq)
      {
        _skipFreq = 1;
        SkipFrequencyGroupStyle.ApplyStyle(externalGroups, localGroups, delegate (int c)
        { _skipFreq = c; });
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
      IReadableColumn? Column, // the column as it was at the time of this call
      string? ColumnName, // the name of the column (last part of the column proxies document path)
      Action<IReadableColumn?> ColumnSetAction // action to set the column during Apply of the controller
      )> GetAdditionallyUsedColumns()
    {
      yield break; // no additionally used columns
    }

    #endregion IDocumentNode Members
  }
}
