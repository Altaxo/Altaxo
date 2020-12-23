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
using Altaxo.Data;
using Altaxo.Graph.Plot.Data;
using Altaxo.Graph.Plot.Groups;

namespace Altaxo.Graph.Graph3D.Plot.Styles
{
  using System.ComponentModel;
  using System.Diagnostics.CodeAnalysis;
  using Altaxo.Graph;
  using Altaxo.Main;
  using Data;
  using Drawing;
  using Drawing.D3D;
  using Drawing.D3D.LineCaps;
  using Geometry;
  using GraphicsContext;
  using Groups;

  [DisplayName("${res:ClassNames.Altaxo.Graph.Graph3D.Plot.Styles.VectorCartesicPlotStyle}")]
  public class VectorCartesicPlotStyle
    :
    Main.SuspendableDocumentNodeWithEventArgs, IG3DPlotStyle
  {
    /// <summary>
    /// Designates how to interpret the values of the error columns.
    /// </summary>
    public enum ValueInterpretation
    {
      /// <summary>The columns are absolute differences, i.e. absolute deviations from the nominal value. The target value is the nominal value plus this difference.</summary>
      AbsoluteDifference = 0,

      /// <summary>The columns are interpretet as values that designate the target location of the vector.</summary>
      AbsoluteValue = 1
    }

    private IReadableColumnProxy? _columnX;
    private IReadableColumnProxy? _columnY;
    private IReadableColumnProxy? _columnZ;

    private ValueInterpretation _meaningOfValues;

    /// <summary>If true, the vector length is set manually, and the three columns are used only to determine the vector direction.</summary>
    private bool _useManualVectorLength;

    /// <summary>Constant value of the vector length. Used only if <see cref="_useManualVectorLength"/> is set to true.</summary>
    private double _vectorLengthOffset = 2;

    /// <summary>Factor that is multiplied with the cached symbol size to determine the vector length. Used only if <see cref="_useManualVectorLength"/> is set to true.</summary>
    private double _vectorLengthFactor;

    /// <summary>
    /// True if the color of the label is not dependent on the color of the parent plot style.
    /// </summary>
    protected bool _independentColor;

    /// <summary>Pen used to draw the vector.</summary>
    private PenX3D _strokePen;

    /// <summary>
    /// True if the symbol size is independent, i.e. is not published nor updated by a group style.
    /// </summary>
    private bool _independentSymbolSize;

    /// <summary>Controls the length of the end bar.</summary>
    private double _symbolSize;

    /// <summary>
    /// True when the vector line is not drawn in the circel of diameter SymbolSize around the symbol center.
    /// </summary>
    private bool _useSymbolGap;

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

    private double _endCapSizeFactor = 1;

    private double _endCapSizeOffset;

    private double _lineWidth1Offset;
    private double _lineWidth1Factor;

    private double _lineWidth2Offset;
    private double _lineWidth2Factor;

    /// <summary>If true, group styles that shift the logical position of the items (for instance <see cref="BarSizePosition3DGroupStyle"/>) are not applied. I.e. when true, the position of the item remains unperturbed.</summary>
    private bool _independentOnShiftingGroupStyles = true; // default is true, because we don't want irritated users

    /// <summary>
    /// Skip frequency.
    /// </summary>
    protected int _skipFrequency;

    protected bool _independentSkipFrequency;

    /// <summary>Logical x shift between the location of the real data point and the point where the item is finally drawn.</summary>
    private double _cachedLogicalShiftX;

    /// <summary>Logical y shift between the location of the real data point and the point where the item is finally drawn.</summary>
    private double _cachedLogicalShiftY;

    /// <summary>Logical y shift between the location of the real data point and the point where the item is finally drawn.</summary>
    private double _cachedLogicalShiftZ;

    /// <summary>If this function is set, then _symbolSize is ignored and the symbol size is evaluated by this function.</summary>
    [field: NonSerialized]
    protected Func<int, double>? _cachedSymbolSizeForIndexFunction;

    /// <summary>If this function is set, the symbol color is determined by calling this function on the index into the data.</summary>
    [field: NonSerialized]
    protected Func<int, System.Drawing.Color>? _cachedColorForIndexFunction;

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(VectorCartesicPlotStyle), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (VectorCartesicPlotStyle)obj;

        info.AddEnum("MeaningOfValues", s._meaningOfValues);
        info.AddValueOrNull("ColumnX", s._columnX);
        info.AddValueOrNull("ColumnY", s._columnY);
        info.AddValueOrNull("ColumnZ", s._columnZ);
        info.AddValue("IndependentSkipFreq", s._independentSkipFrequency);
        info.AddValue("SkipFreq", s._skipFrequency);
        info.AddValue("UseManualVectorLength", s._useManualVectorLength);
        info.AddValue("VectorLengthOffset", s._vectorLengthOffset);
        info.AddValue("VectorLengthFactor", s._vectorLengthFactor);

        info.AddValue("IndependentSymbolSize", s._independentSymbolSize);
        info.AddValue("SymbolSize", s._symbolSize);

        info.AddValue("Pen", s._strokePen);
        info.AddValue("IndependentColor", s._independentColor);

        info.AddValue("LineWidth1Offset", s._lineWidth1Offset);
        info.AddValue("LineWidth1Factor", s._lineWidth1Factor);
        info.AddValue("LineWidth2Offset", s._lineWidth2Offset);
        info.AddValue("LineWidth2Factor", s._lineWidth2Factor);

        info.AddValue("EndCapSizeOffset", s._endCapSizeOffset);
        info.AddValue("EndCapSizeFactor", s._endCapSizeFactor);

        info.AddValue("UseSymbolGap", s._useSymbolGap);
        info.AddValue("SymbolGapOffset", s._symbolGapOffset);
        info.AddValue("SymbolGapFactor", s._symbolGapFactor);

        info.AddValue("IndependentOnShiftingGroupStyles", s._independentOnShiftingGroupStyles);
      }

      protected virtual VectorCartesicPlotStyle SDeserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (VectorCartesicPlotStyle?)o ?? new VectorCartesicPlotStyle(info);

        s._meaningOfValues = (ValueInterpretation)info.GetEnum("MeaningOfValues", typeof(ValueInterpretation));

        s.ChildSetMember(ref s._columnX, info.GetValueOrNull<IReadableColumnProxy>("ColumnX", s));
        s.ChildSetMember(ref s._columnY, info.GetValueOrNull<IReadableColumnProxy>("ColumnY", s));
        s.ChildSetMember(ref s._columnZ, info.GetValueOrNull<IReadableColumnProxy>("ColumnZ", s));

        s._independentSkipFrequency = info.GetBoolean("IndependentSkipFreq");
        s._skipFrequency = info.GetInt32("SkipFreq");

        s._useManualVectorLength = info.GetBoolean("UseManualVectorLength");
        s._vectorLengthOffset = info.GetDouble("VectorLengthOffset");
        s._vectorLengthFactor = info.GetDouble("VectorLengthFactor");

        s._independentSymbolSize = info.GetBoolean("IndependentSymbolSize");
        s._symbolSize = info.GetDouble("SymbolSize");

        s.Pen = (PenX3D)info.GetValue("Pen", s);
        s._independentColor = info.GetBoolean("IndependentColor");

        s._lineWidth1Offset = info.GetDouble("LineWidth1Offset");
        s._lineWidth1Factor = info.GetDouble("LineWidth1Factor");

        s._lineWidth2Offset = info.GetDouble("LineWidth2Offset");
        s._lineWidth2Factor = info.GetDouble("LineWidth2Factor");

        s._endCapSizeOffset = info.GetDouble("EndCapSizeOffset");
        s._endCapSizeFactor = info.GetDouble("EndCapSizeFactor");

        s._useSymbolGap = info.GetBoolean("UseSymbolGap");
        s._symbolGapOffset = info.GetDouble("SymbolGapOffset");
        s._symbolGapFactor = info.GetDouble("SymbolGapFactor");

        s._independentOnShiftingGroupStyles = info.GetBoolean("IndependentOnShiftingGroupStyles");

        return s;
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        VectorCartesicPlotStyle s = SDeserialize(o, info, parent);

        return s;
      }
    }

    #endregion Serialization

    /// <summary>
    /// Deserialization constructor
    /// </summary>
    /// <param name="info">The information.</param>
#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
    protected VectorCartesicPlotStyle(Altaxo.Serialization.Xml.IXmlDeserializationInfo info)
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
    {
    }

    public VectorCartesicPlotStyle(Altaxo.Main.Properties.IReadOnlyPropertyBag context)
    {
      var penWidth = GraphDocument.GetDefaultPenWidth(context);
      var color = GraphDocument.GetDefaultPlotColor(context);

      _lineWidth1Offset = penWidth;
      _lineWidth1Factor = 0;
      _lineWidth2Offset = penWidth;
      _lineWidth2Factor = 0;

      _strokePen = new PenX3D(color, penWidth).WithLineEndCap(new ContourArrow05());
    }

    public VectorCartesicPlotStyle(VectorCartesicPlotStyle from, bool copyWithDataReferences)
    {
      CopyFrom(from, copyWithDataReferences);
    }

    [MemberNotNull(nameof(_strokePen))]
    protected void CopyFrom(VectorCartesicPlotStyle from, bool copyWithDataReferences)
    {
      _meaningOfValues = from._meaningOfValues;
      _independentSkipFrequency = from._independentSkipFrequency;
      _skipFrequency = from._skipFrequency;
      _useManualVectorLength = from._useManualVectorLength;
      _vectorLengthOffset = from._vectorLengthOffset;
      _vectorLengthFactor = from._vectorLengthFactor;

      _independentSymbolSize = from._independentSymbolSize;
      _symbolSize = from._symbolSize;

      _strokePen = from._strokePen;
      _independentColor = from._independentColor;

      _lineWidth1Offset = from._lineWidth1Offset;
      _lineWidth1Factor = from._lineWidth1Factor;
      _lineWidth2Offset = from._lineWidth2Offset;
      _lineWidth2Factor = from._lineWidth2Factor;

      _endCapSizeFactor = from._endCapSizeFactor;
      _endCapSizeOffset = from._endCapSizeOffset;

      _useSymbolGap = from._useSymbolGap;
      _symbolGapFactor = from._symbolGapFactor;
      _symbolGapOffset = from._symbolGapOffset;

      _independentSkipFrequency = from._independentSkipFrequency;
      _skipFrequency = from._skipFrequency;
      _independentOnShiftingGroupStyles = from._independentOnShiftingGroupStyles;

      _cachedLogicalShiftX = from._cachedLogicalShiftX;
      _cachedLogicalShiftY = from._cachedLogicalShiftY;

      if (copyWithDataReferences)
      {
        ChildCloneToMember(ref _columnX, from._columnX);
        ChildCloneToMember(ref _columnY, from._columnY);
        ChildCloneToMember(ref _columnZ, from._columnZ);
      }
    }

      public bool CopyFrom(object obj, bool copyWithDataReferences)
    {
      if (ReferenceEquals(this, obj))
        return true;
      if (obj is VectorCartesicPlotStyle from)
      {
        CopyFrom(from, copyWithDataReferences);
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
      if (ReferenceEquals(this, obj))
        return true;

      return CopyFrom(obj, true);
    }

    /// <inheritdoc/>
    public object Clone(bool copyWithDataReferences)
    {
      return new VectorCartesicPlotStyle(this, copyWithDataReferences);
    }

    /// <inheritdoc/>
    public object Clone()
    {
      return new VectorCartesicPlotStyle(this, true);
    }

    protected override IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
    {
      if (_columnX is not null)
        yield return new Main.DocumentNodeAndName(_columnX, nameof(ColumnX));

      if (_columnY is not null)
        yield return new Main.DocumentNodeAndName(_columnY, nameof(ColumnY));

      if (_columnZ is not null)
        yield return new Main.DocumentNodeAndName(_columnZ, nameof(ColumnZ));
    }

    #region Properties

    public ValueInterpretation MeaningOfValues
    {
      get
      {
        return _meaningOfValues;
      }
      set
      {
        if (!(_meaningOfValues == value))
        {
          _meaningOfValues = value;
          EhSelfChanged();
        }
      }
    }

    /// <summary>
    /// Data that define the error in the positive direction.
    /// </summary>
    public IReadableColumn? ColumnX
    {
      get
      {
        return _columnX?.Document();
      }
      set
      {
        var oldValue = _columnX?.Document();
        if (!object.ReferenceEquals(value, oldValue))
        {
          ChildSetMember(ref _columnX, value is null ? null : ReadableColumnProxyBase.FromColumn(value));
          EhSelfChanged(EventArgs.Empty);
        }
      }
    }

    /// <summary>
    /// Gets the name of the common error column, if it is a data column. Otherwise, null is returned.
    /// </summary>
    /// <value>
    /// The name of the common error column if it is a data column. Otherwise, null.
    /// </value>
    public string? ColumnXDataColumnName
    {
      get
      {
        return _columnX?.DocumentPath()?.LastPartOrDefault;
      }
    }

    /// <summary>
    /// Data that define the error in the positive direction.
    /// </summary>
    public IReadableColumn? ColumnY
    {
      get
      {
        return _columnY?.Document();
      }
      set
      {
        var oldValue = _columnY?.Document();
        if (!object.ReferenceEquals(value, oldValue))
        {
          ChildSetMember(ref _columnY, value is null ? null : ReadableColumnProxyBase.FromColumn(value));
          EhSelfChanged(EventArgs.Empty);
        }
      }
    }

    /// <summary>
    /// Gets the name of the positive error column, if it is a data column. Otherwise, null is returned.
    /// </summary>
    /// <value>
    /// The name of the positive error column if it is a data column. Otherwise, null.
    /// </value>
    public string? ColumnYDataColumnName
    {
      get
      {
        return _columnY?.DocumentPath()?.LastPartOrDefault;
      }
    }

    /// <summary>
    /// Data that define the error in the negative direction.
    /// </summary>
    public IReadableColumn? ColumnZ
    {
      get
      {
        return _columnZ?.Document();
      }
      set
      {
        var oldValue = _columnZ?.Document();
        if (!object.ReferenceEquals(value, oldValue))
        {
          ChildSetMember(ref _columnZ, value is null ? null : ReadableColumnProxyBase.FromColumn(value));
          EhSelfChanged(EventArgs.Empty);
        }
      }
    }

    /// <summary>
    /// Gets the name of the negative error column, if it is a data column. Otherwise, null is returned.
    /// </summary>
    /// <value>
    /// The name of the negative error column if it is a data column. Otherwise, null.
    /// </value>
    public string? ColumnZDataColumnName
    {
      get
      {
        return _columnZ?.DocumentPath()?.LastPartOrDefault;
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the skip frequency is independent on other sub group styles using <see cref="SkipFrequency"/>.
    /// </summary>
    /// <value>
    /// <c>true</c> if the skip frequency is independent on other sub group styles using <see cref="SkipFrequency"/>; otherwise, <c>false</c>.
    /// </value>
    public bool IndependentSkipFrequency
    {
      get { return _independentSkipFrequency; }
      set
      {
        if (!(_independentSkipFrequency == value))
        {
          _independentSkipFrequency = value;
          EhSelfChanged();
        }
      }
    }

    /// <summary>Controls how many items are plotted. A value of 1 means every item, a value of 2 every other item, and so on.</summary>
    public int SkipFrequency
    {
      get { return _skipFrequency; }
      set
      {
        if (!(_skipFrequency == value))
        {
          _skipFrequency = Math.Max(1, value);
          EhSelfChanged();
        }
      }
    }

    /// <summary>If true, the vector length is set manually, and the three columns are used only to determine the vector direction.</summary>
    public bool UseManualVectorLength
    {
      get { return _useManualVectorLength; }
      set
      {
        if (!(_useManualVectorLength == value))
        {
          _useManualVectorLength = value;
          EhSelfChanged();
        }
      }
    }

    /// <summary>Constant value of the vector length. Used only if <see cref="_useManualVectorLength"/> is set to true.</summary>
    public double VectorLengthOffset
    {
      get
      {
        return _vectorLengthOffset;
      }
      set
      {
        if (!(_vectorLengthOffset == value))
        {
          _vectorLengthOffset = value;
          EhSelfChanged();
        }
      }
    }

    /// <summary>Factor that is multiplied with the cached symbol size to determine the vector length. Used only if <see cref="_useManualVectorLength"/> is set to true.</summary>
    public double VectorLengthFactor
    {
      get
      {
        return _vectorLengthFactor;
      }
      set
      {
        if (!(_vectorLengthFactor == value))
        {
          _vectorLengthFactor = value;
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

    /// <summary>
    /// True when the line is not drawn in the circel of diameter SymbolSize around the symbol center.
    /// </summary>
    public bool UseSymbolGap
    {
      get { return _useSymbolGap; }
      set
      {
        var oldValue = _useSymbolGap;
        _useSymbolGap = value;
        if (oldValue != value)
          EhSelfChanged(EventArgs.Empty);
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

    public double EndCapSizeOffset
    {
      get
      {
        return _endCapSizeOffset;
      }
      set
      {
        if (!(_endCapSizeOffset == value))
        {
          _endCapSizeOffset = value;
          EhSelfChanged();
        }
      }
    }

    public double EndCapSizeFactor
    {
      get
      {
        return _endCapSizeFactor;
      }
      set
      {
        if (!(_endCapSizeFactor == value))
        {
          _endCapSizeFactor = value;
          EhSelfChanged();
        }
      }
    }

    /// <summary>
    /// True if the color of the label is not dependent on the color of the parent plot style.
    /// </summary>
    public bool IndependentColor
    {
      get { return _independentColor; }
      set
      {
        var oldValue = _independentColor;
        _independentColor = value;
        if (oldValue != value)
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

    /// <summary>Pen used to draw the error bar.</summary>
    public PenX3D Pen
    {
      get { return _strokePen; }
      set
      {
        var oldValue = _strokePen;
        _strokePen = value;
        if (!object.ReferenceEquals(oldValue, value))
        {
          EhSelfChanged(EventArgs.Empty);
        }
      }
    }

    #endregion Properties

    #region IG3DPlotStyle Members

    public void CollectExternalGroupStyles(PlotGroupStyleCollection externalGroups)
    {
      if (!_independentColor)
        Graph.Plot.Groups.ColorGroupStyle.AddExternalGroupStyle(externalGroups);
    }

    public void CollectLocalGroupStyles(PlotGroupStyleCollection externalGroups, PlotGroupStyleCollection localGroups)
    {
      if (!_independentColor)
        Graph.Plot.Groups.ColorGroupStyle.AddLocalGroupStyle(externalGroups, localGroups);

      if (!_independentSkipFrequency)
        SkipFrequencyGroupStyle.AddLocalGroupStyle(externalGroups, localGroups); // (local group only)
    }

    public void PrepareGroupStyles(PlotGroupStyleCollection externalGroups, PlotGroupStyleCollection localGroups, IPlotArea layer, Processed3DPlotData pdata)
    {
      if (!_independentColor)
        Graph.Plot.Groups.ColorGroupStyle.PrepareStyle(externalGroups, localGroups, delegate ()
        { return _strokePen.Color; });

      if (!_independentSkipFrequency)
        SkipFrequencyGroupStyle.PrepareStyle(externalGroups, localGroups, delegate ()
        { return SkipFrequency; });

      // note: symbol size and barposition are only applied, but not prepared
      // this item can not be used as provider of a symbol size
    }

    public void ApplyGroupStyles(PlotGroupStyleCollection externalGroups, PlotGroupStyleCollection localGroups)
    {
      _cachedColorForIndexFunction = null;
      _cachedSymbolSizeForIndexFunction = null;
      // color
      if (!_independentColor)
      {
        ColorGroupStyle.ApplyStyle(externalGroups, localGroups, delegate (NamedColor c)
        { _strokePen = _strokePen.WithColor(c); });

        // but if there is a color evaluation function, then use that function with higher priority
        VariableColorGroupStyle.ApplyStyle(externalGroups, localGroups, delegate (Func<int, System.Drawing.Color> evalFunc)
        { _cachedColorForIndexFunction = evalFunc; });
      }

      if (!_independentSkipFrequency)
        SkipFrequencyGroupStyle.ApplyStyle(externalGroups, localGroups, delegate (int c)
        { SkipFrequency = c; });

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

      // Shift the items ?
      _cachedLogicalShiftX = 0;
      _cachedLogicalShiftY = 0;
      _cachedLogicalShiftZ = 0;
      if (!_independentOnShiftingGroupStyles)
      {
        var shiftStyle = PlotGroupStyle.GetFirstStyleToApplyImplementingInterface<IShiftLogicalXYZGroupStyle>(externalGroups, localGroups);
        if (shiftStyle is not null)
        {
          shiftStyle.Apply(out _cachedLogicalShiftX, out _cachedLogicalShiftY, out _cachedLogicalShiftZ);
        }
      }
    }

    public void Paint(IGraphicsContext3D g, IPlotArea layer, Processed3DPlotData pdata, Processed3DPlotData? prevItemData, Processed3DPlotData? nextItemData)
    {
      const double logicalClampMinimum = -10;
      const double logicalClampMaximum = 11;

      if (pdata is null || !(pdata.RangeList is { } rangeList))
        return;

      // Plot error bars for the dependent variable (y)
      var ptArray = pdata.PlotPointsInAbsoluteLayerCoordinates;
      var columnX = ColumnX;
      var columnY = ColumnY;
      var columnZ = ColumnZ;

      if (columnX is null || columnY is null || columnZ is null)
        return; // nothing to do if both error columns are null

      if (!typeof(double).IsAssignableFrom(columnX.ItemType) || !typeof(double).IsAssignableFrom(columnY.ItemType) || !typeof(double).IsAssignableFrom(columnZ.ItemType))
        return; // TODO make this an runtime paint error to be reported

      var strokePen = _strokePen;

      foreach (PlotRange r in rangeList)
      {
        int lower = r.LowerBound;
        int upper = r.UpperBound;
        int offset = r.OffsetToOriginal;

        for (int j = lower; j < upper; j += _skipFrequency)
        {
          int originalRow = j + offset;
          double symbolSize = _cachedSymbolSizeForIndexFunction is null ? _symbolSize : _cachedSymbolSizeForIndexFunction(originalRow);

          strokePen = strokePen.WithThickness1(_lineWidth1Offset + _lineWidth1Factor * symbolSize);
          strokePen = strokePen.WithThickness2(_lineWidth2Offset + _lineWidth2Factor * symbolSize);

          if (_cachedColorForIndexFunction is not null)
            strokePen = strokePen.WithColor(GdiColorHelper.ToNamedColor(_cachedColorForIndexFunction(originalRow), "VariableColor"));
          if (strokePen.LineEndCap is not null)
            strokePen = strokePen.WithLineEndCap(strokePen.LineEndCap.WithMinimumAbsoluteAndRelativeSize(symbolSize * _endCapSizeFactor + _endCapSizeOffset, 1 + 1E-6));

          // Calculate target
          AltaxoVariant targetX, targetY, targetZ;
          switch (_meaningOfValues)
          {
            case ValueInterpretation.AbsoluteDifference:
              {
                targetX = pdata.GetXPhysical(originalRow) + columnX[originalRow];
                targetY = pdata.GetYPhysical(originalRow) + columnY[originalRow];
                targetZ = pdata.GetZPhysical(originalRow) + columnZ[originalRow];
              }
              break;

            case ValueInterpretation.AbsoluteValue:
              {
                targetX = columnX[originalRow];
                targetY = columnY[originalRow];
                targetZ = columnZ[originalRow];
              }
              break;

            default:
              throw new NotImplementedException(nameof(_meaningOfValues));
          }

          var logicalTarget = layer.GetLogical3D(targetX, targetY, targetZ);
          var logicalOrigin = layer.GetLogical3D(pdata, originalRow);

          if (!_independentOnShiftingGroupStyles && (0 != _cachedLogicalShiftX || 0 != _cachedLogicalShiftY || 0 != _cachedLogicalShiftZ))
          {
            logicalOrigin.RX += _cachedLogicalShiftX;
            logicalOrigin.RY += _cachedLogicalShiftY;
            logicalOrigin.RZ += _cachedLogicalShiftZ;
            logicalTarget.RX += _cachedLogicalShiftX;
            logicalTarget.RY += _cachedLogicalShiftY;
            logicalTarget.RZ += _cachedLogicalShiftZ;
          }

          if (!Calc.RMath.IsInIntervalCC(logicalOrigin.RX, logicalClampMinimum, logicalClampMaximum))
            continue;
          if (!Calc.RMath.IsInIntervalCC(logicalOrigin.RY, logicalClampMinimum, logicalClampMaximum))
            continue;
          if (!Calc.RMath.IsInIntervalCC(logicalOrigin.RZ, logicalClampMinimum, logicalClampMaximum))
            continue;

          if (!Calc.RMath.IsInIntervalCC(logicalTarget.RX, logicalClampMinimum, logicalClampMaximum))
            continue;
          if (!Calc.RMath.IsInIntervalCC(logicalTarget.RY, logicalClampMinimum, logicalClampMaximum))
            continue;
          if (!Calc.RMath.IsInIntervalCC(logicalTarget.RZ, logicalClampMinimum, logicalClampMaximum))
            continue;

          var isoLine = layer.CoordinateSystem.GetIsoline(logicalOrigin, logicalTarget);
          if (isoLine is null)
            continue;

          if (_useManualVectorLength)
          {
            double length = _vectorLengthOffset + _vectorLengthFactor * symbolSize;
            double isoLineLength = isoLine.TotalLineLength;
            isoLine = isoLine.ShortenedBy(RADouble.NewAbs(0), RADouble.NewAbs(isoLineLength - length));
            if (isoLine is null)
              continue;
          }

          if (_useSymbolGap)
          {
            double gap = _symbolGapOffset + _symbolGapFactor * symbolSize;
            if (gap != 0)
            {
              isoLine = isoLine.ShortenedBy(RADouble.NewAbs(gap / 2), RADouble.NewAbs(0));
              if (isoLine is null)
                continue;
            }
          }

          g.DrawLine(strokePen, isoLine);
        }
      }
    }

    /// <summary>
    /// Paints a appropriate symbol in the given rectangle. The width of the rectangle is mandatory, but if the heigth is too small,
    /// you should extend the bounding rectangle and set it as return value of this function.
    /// </summary>
    /// <param name="g">The graphics context.</param>
    /// <param name="bounds">The bounds, in which the symbol should be painted.</param>
    /// <returns>If the height of the bounding rectangle is sufficient for painting, returns the original bounding rectangle. Otherwise, it returns a rectangle that is
    /// inflated in y-Direction. Do not inflate the rectangle in x-direction!</returns>
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

    #endregion IG3DPlotStyle Members

    #region IDocumentNode Members

    /// <summary>
    /// Replaces path of items (intended for data items like tables and columns) by other paths. Thus it is possible
    /// to change a plot so that the plot items refer to another table.
    /// </summary>
    /// <param name="Report">Function that reports the found <see cref="DocNodeProxy"/> instances to the visitor.</param>
    public void VisitDocumentReferences(DocNodeProxyReporter Report)
    {
      if (_columnX is not null)
        Report(_columnX, this, nameof(ColumnX));
      if (_columnY is not null)
        Report(_columnY, this, nameof(ColumnY));
      if (_columnZ is not null)
        Report(_columnZ, this, nameof(ColumnZ));
    }

    /// <summary>
    /// Gets the columns used additionally by this style, e.g. the label column for a label plot style, or the error columns for an error bar plot style.
    /// </summary>
    /// <returns>An enumeration of tuples. Each tuple consist of the column name, as it should be used to identify the column in the data dialog. The second item of this
    /// tuple is a function that returns the column proxy for this column, in order to get the underlying column or to set the underlying column.</returns>
    public IEnumerable<(
      string ColumnLabel, // Column label
      IReadableColumn? Column, // the column as it was at the time of this call
      string? ColumnName, // the name of the column (last part of the column proxies document path)
      Action<IReadableColumn?> ColumnSetAction // action to set the column during Apply of the controller
      )> GetAdditionallyUsedColumns()
    {
      yield return (nameof(ColumnX), ColumnX, _columnX?.DocumentPath()?.LastPartOrDefault, (col) => ColumnX = col as INumericColumn);

      yield return (nameof(ColumnY), ColumnY, _columnY?.DocumentPath()?.LastPartOrDefault, (col) => ColumnY = col as INumericColumn);

      yield return (nameof(ColumnZ), ColumnZ, _columnZ?.DocumentPath()?.LastPartOrDefault, (col) => ColumnZ = col as INumericColumn);
    }

    #endregion IDocumentNode Members
  }
}
