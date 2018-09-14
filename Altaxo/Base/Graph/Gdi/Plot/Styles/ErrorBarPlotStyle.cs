#region Copyright

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

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using Altaxo.Data;
using Altaxo.Graph.Plot.Data;
using Altaxo.Graph.Plot.Groups;

namespace Altaxo.Graph.Gdi.Plot.Styles
{
  using Altaxo.Graph;
  using Altaxo.Graph.Gdi.Plot.Data;
  using Altaxo.Main;
  using Drawing;
  using Geometry;

  #region Error bar (abstract, for implementations see below)

  public abstract class ErrorBarPlotStyle
    :
    Main.SuspendableDocumentNodeWithEventArgs,
    IG2DPlotStyle,
    IRoutedPropertyReceiver
  {
    /// <summary>
    /// Designates how to interpret the values of the error columns.
    /// </summary>
    public enum ValueInterpretation
    {
      /// <summary>The error columns are absolute errors, i.e. absolute deviations from the nominal value.</summary>
      AbsoluteError = 0,

      /// <summary>The error columns are relative errors, i.e. deviations relativ to the nominal value. If e.g. the nominal value is 20, and the relative error is 0.1, then the absolute error is 2.</summary>
      RelativeError = 1,

      /// <summary>The error columns are interpretet as minimum and maximum. This setting is usefullonly for separate positive and negative error columns.</summary>
      AbsoluteValue = 2
    }

    protected bool _useCommonErrorColumn = true;

    private IReadableColumnProxy _commonErrorColumn;
    private IReadableColumnProxy _positiveErrorColumn;
    private IReadableColumnProxy _negativeErrorColumn;

    private ValueInterpretation _meaningOfValues;

    protected bool _independentSkipFrequency;

    /// <summary>
    /// Skip frequency.
    /// </summary>
    protected int _skipFrequency = 1;

    /// <summary>
    /// If true, treat missing points as if not present (e.g. connect lines over missing points, count skip seamlessly over missing points)
    /// </summary>
    protected bool _ignoreMissingDataPoints;

    /// <summary>If true, group styles that shift the logical position of the items (for instance <see cref="BarSizePosition3DGroupStyle"/>) are not applied. I.e. when true, the position of the item remains unperturbed.</summary>
    private bool _independentOnShiftingGroupStyles;

    /// <summary>
    /// True if the color of the label is not dependent on the color of the parent plot style.
    /// </summary>
    protected bool _independentColor;

    /// <summary>Pen used to draw the error bar.</summary>
    private PenX _pen;

    /// <summary>
    /// true if the symbol size is independent, i.e. is not published nor updated by a group style.
    /// </summary>
    private bool _independentSymbolSize;

    /// <summary>
    /// True if the dash pattern of the error bar line is independent on the dash pattern of the line style.
    /// </summary>
    private bool _independentDashPattern = true;

    /// <summary>Controls the length of the end bar.</summary>
    private double _symbolSize;

    /// <summary>
    /// True when the line is not drawn in the circel of diameter SymbolSize around the symbol center.
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

    private double _endCapSizeFactor = 0.8;

    private double _endCapSizeOffset;

    /// <summary>
    /// If true, the end cap is shown even if the line is not shown, because the line length is zero.
    /// This can happen if the user defined gap is larger than the error.
    /// </summary>
    private bool _forceVisibilityOfEndCap = true;

    private double _lineWidth1Offset;
    private double _lineWidth1Factor;

    /// <summary>Logical x shift between the location of the real data point and the point where the item is finally drawn.</summary>
    private double _cachedLogicalShiftX;

    /// <summary>Logical y shift between the location of the real data point and the point where the item is finally drawn.</summary>
    private double _cachedLogicalShiftY;

    /// <summary>If this function is set, then _symbolSize is ignored and the symbol size is evaluated by this function.</summary>
    [field: NonSerialized]
    protected Func<int, double> _cachedSymbolSizeForIndexFunction;

    /// <summary>If this function is set, the symbol color is determined by calling this function on the index into the data.</summary>
    [field: NonSerialized]
    protected Func<int, Color> _cachedColorForIndexFunction;

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.Gdi.Plot.Styles.ErrorBarPlotStyle", 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        throw new InvalidOperationException("Serialization of old version");

        /*
				ErrorBarPlotStyle s = (ErrorBarPlotStyle)obj;

				info.AddValue("PositiveError", s._positiveErrorColumn);
				info.AddValue("NegativeError", s._negativeErrorColumn);

				info.AddValue("IndependentColor", s._independentColor);
				info.AddValue("Pen", s._pen);

				info.AddValue("Axis", s._isHorizontalStyle ? 0 : 1);
				info.AddValue("IndependentSymbolSize", s._independentSymbolSize);
				info.AddValue("SymbolSize", s._symbolSize);
				info.AddValue("SymbolGap", s._symbolGap);
				info.AddValue("SkipFreq", s._skipFrequency);

				info.AddValue("ShowEndBars", s._showEndBars);
				info.AddValue("NotShiftHorzPos", s._doNotShiftHorizontalPosition);
				*/
      }

      protected virtual ErrorBarPlotStyle SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        ErrorBarPlotStyle s; // = null != o ? (ErrorBarPlotStyle)o : new ErrorBarPlotStyle(info);

        var positiveErrorColumn = (Altaxo.Data.IReadableColumnProxy)info.GetValue("PositiveError", null);

        var negativeErrorColumn = (Altaxo.Data.IReadableColumnProxy)info.GetValue("NegativeError", null);

        var independentColor = info.GetBoolean("IndependentColor");

        var pen = (PenX)info.GetValue("Pen", null);

        var isHorizontalStyle = (0 == info.GetInt32("Axis"));

        if (isHorizontalStyle)
          s = new ErrorBarXPlotStyle(info, positiveErrorColumn, negativeErrorColumn);
        else
          s = new ErrorBarYPlotStyle(info, positiveErrorColumn, negativeErrorColumn);

        s.IndependentColor = independentColor;

        s._independentSymbolSize = info.GetBoolean("IndependentSymbolSize");
        s._symbolSize = info.GetDouble("SymbolSize");
        s._useSymbolGap = info.GetBoolean("SymbolGap");
        s._skipFrequency = info.GetInt32("SkipFreq");
        if (info.GetBoolean("ShowEndBars"))
          pen.EndCap = new LineCaps.SymBarLineCap();
        s._independentOnShiftingGroupStyles = info.GetBoolean("NotShiftHorzPos");

        if (null == pen)
          throw new ArgumentNullException(nameof(pen));
        s.ChildSetMember(ref s._pen, pen);

        s._forceVisibilityOfEndCap = true;

        return s;
      }

      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        ErrorBarPlotStyle s = SDeserialize(o, info, parent);

        return s;
      }
    }

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ErrorBarPlotStyle), 1)]
    private class XmlSerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (ErrorBarPlotStyle)obj;

        info.AddEnum("MeaningOfValues", s._meaningOfValues);
        info.AddValue("UseCommonColumn", s._useCommonErrorColumn);

        if (s._useCommonErrorColumn)
        {
          info.AddValue("CommonError", s._commonErrorColumn);
        }
        else
        {
          info.AddValue("PositiveError", s._positiveErrorColumn);
          info.AddValue("NegativeError", s._negativeErrorColumn);
        }
        info.AddValue("IndependentSkipFreq", s._independentSkipFrequency);
        info.AddValue("SkipFreq", s._skipFrequency);
        info.AddValue("IgnoreMissingDataPoints", s._ignoreMissingDataPoints);
        info.AddValue("IndependentOnShiftingGroupStyles", s._independentOnShiftingGroupStyles);

        info.AddValue("IndependentSymbolSize", s._independentSymbolSize);
        info.AddValue("SymbolSize", s._symbolSize);

        info.AddValue("Pen", s._pen);
        info.AddValue("IndependentColor", s._independentColor);
        info.AddValue("IndependentDashPattern", s._independentDashPattern);

        info.AddValue("LineWidth1Offset", s._lineWidth1Offset);
        info.AddValue("LineWidth1Factor", s._lineWidth1Factor);

        info.AddValue("EndCapSizeOffset", s._endCapSizeOffset);
        info.AddValue("EndCapSizeFactor", s._endCapSizeFactor);

        info.AddValue("UseSymbolGap", s._useSymbolGap);
        info.AddValue("SymbolGapOffset", s._symbolGapOffset);
        info.AddValue("SymbolGapFactor", s._symbolGapFactor);
        info.AddValue("ForceVisibilityOfEndCap", s._forceVisibilityOfEndCap);
      }

      protected virtual ErrorBarPlotStyle SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        var s = (ErrorBarPlotStyle)o;

        s._meaningOfValues = (ValueInterpretation)info.GetEnum("MeaningOfValues", typeof(ValueInterpretation));
        s._useCommonErrorColumn = info.GetBoolean("UseCommonColumn");

        if (s._useCommonErrorColumn)
        {
          s._commonErrorColumn = (IReadableColumnProxy)info.GetValue("CommonError", s);
          if (null != s._commonErrorColumn)
            s._commonErrorColumn.ParentObject = s;
        }
        else
        {
          s._positiveErrorColumn = (IReadableColumnProxy)info.GetValue("PositiveError", s);
          if (null != s._positiveErrorColumn)
            s._positiveErrorColumn.ParentObject = s;

          s._negativeErrorColumn = (IReadableColumnProxy)info.GetValue("NegativeError", s);
          if (null != s._negativeErrorColumn)
            s._negativeErrorColumn.ParentObject = s;
        }

        s._independentSkipFrequency = info.GetBoolean("IndependentSkipFreq");
        s._skipFrequency = info.GetInt32("SkipFreq");
        s._ignoreMissingDataPoints = info.GetBoolean("IgnoreMissingDataPoints");
        s._independentOnShiftingGroupStyles = info.GetBoolean("IndependentOnShiftingGroupStyles");

        s._independentSymbolSize = info.GetBoolean("IndependentSymbolSize");
        s._symbolSize = info.GetDouble("SymbolSize");

        s.ChildSetMember(ref s._pen, (PenX)info.GetValue("Pen", s));
        s._independentColor = info.GetBoolean("IndependentColor");
        s._independentDashPattern = info.GetBoolean("IndependentDashPattern");

        s._lineWidth1Offset = info.GetDouble("LineWidth1Offset");
        s._lineWidth1Factor = info.GetDouble("LineWidth1Factor");

        s._endCapSizeOffset = info.GetDouble("EndCapSizeOffset");
        s._endCapSizeFactor = info.GetDouble("EndCapSizeFactor");

        s._useSymbolGap = info.GetBoolean("UseSymbolGap");
        s._symbolGapOffset = info.GetDouble("SymbolGapOffset");
        s._symbolGapFactor = info.GetDouble("SymbolGapFactor");

        s._forceVisibilityOfEndCap = info.GetBoolean("ForceVisibilityOfEndCap");

        return s;
      }

      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        ErrorBarPlotStyle s = SDeserialize(o, info, parent);

        return s;
      }
    }

    #endregion Serialization

    protected ErrorBarPlotStyle(Altaxo.Serialization.Xml.IXmlDeserializationInfo info)
    {
    }

    protected ErrorBarPlotStyle(Altaxo.Serialization.Xml.IXmlDeserializationInfo info, IReadableColumnProxy posColumnProxy, IReadableColumnProxy negColumnProxy)
    {
      ChildSetMember(ref _positiveErrorColumn, posColumnProxy);
      ChildSetMember(ref _negativeErrorColumn, negColumnProxy);
      _useCommonErrorColumn = null == negColumnProxy;
    }

    public ErrorBarPlotStyle(Altaxo.Main.Properties.IReadOnlyPropertyBag context)
    {
      var penWidth = GraphDocument.GetDefaultPenWidth(context);
      var color = GraphDocument.GetDefaultPlotColor(context);

      _lineWidth1Offset = penWidth;
      _lineWidth1Factor = 0;

      _pen = new PenX(color, penWidth) { EndCap = new Altaxo.Graph.Gdi.LineCaps.SymBarLineCap(), ParentObject = this };
    }

    public ErrorBarPlotStyle(ErrorBarPlotStyle from, bool copyWithDataReferences)
    {
      CopyFrom(from, copyWithDataReferences);
    }

    public bool CopyFrom(object obj, bool copyWithDataReferences)
    {
      if (object.ReferenceEquals(this, obj))
        return true;
      var from = obj as ErrorBarPlotStyle;
      if (null != from)
      {
        _meaningOfValues = from._meaningOfValues;
        _useCommonErrorColumn = from._useCommonErrorColumn;

        if (copyWithDataReferences)
        {
          ChildCloneToMember(ref _commonErrorColumn, from._commonErrorColumn);
          ChildCloneToMember(ref _positiveErrorColumn, from._positiveErrorColumn);
          ChildCloneToMember(ref _negativeErrorColumn, from._negativeErrorColumn);
        }

        _independentSkipFrequency = from._independentSkipFrequency;
        _skipFrequency = from._skipFrequency;
        _ignoreMissingDataPoints = from._ignoreMissingDataPoints;
        _independentOnShiftingGroupStyles = from._independentOnShiftingGroupStyles;

        _independentSymbolSize = from._independentSymbolSize;
        _symbolSize = from._symbolSize;

        ChildCopyToMember(ref _pen, from._pen);
        _independentColor = from._independentColor;
        _independentDashPattern = from._independentDashPattern;

        _lineWidth1Offset = from._lineWidth1Offset;
        _lineWidth1Factor = from._lineWidth1Factor;

        _endCapSizeFactor = from._endCapSizeFactor;
        _endCapSizeOffset = from._endCapSizeOffset;

        _useSymbolGap = from._useSymbolGap;
        _symbolGapFactor = from._symbolGapFactor;
        _symbolGapOffset = from._symbolGapOffset;

        _cachedLogicalShiftX = from._cachedLogicalShiftX;
        _cachedLogicalShiftY = from._cachedLogicalShiftY;

        EhSelfChanged();
        return true;
      }
      return false;
    }

    /// <inheritdoc/>
    public bool CopyFrom(object obj)
    {
      return CopyFrom(obj, true);
    }

    protected override IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
    {
      if (null != _pen)
        yield return new Main.DocumentNodeAndName(_pen, "Pen");

      if (null != _commonErrorColumn)
        yield return new Main.DocumentNodeAndName(_commonErrorColumn, "CommonErrorColumn");

      if (null != _positiveErrorColumn)
        yield return new Main.DocumentNodeAndName(_positiveErrorColumn, "PositiveErrorColumn");

      if (null != _negativeErrorColumn)
        yield return new Main.DocumentNodeAndName(_negativeErrorColumn, "NegativeErrorColumn");
    }

    public abstract object Clone();

    public abstract object Clone(bool copyWithDataReferences);

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

    /// <summary>Controls the length of the end bar.</summary>
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
    /// If true, the end cap is shown even if the line is not shown, because the line length is zero.
    /// This can e.g. happen if the user defined gap is larger than the error.
    /// </summary>
    public bool ForceVisibilityOfEndCap
    {
      get { return _forceVisibilityOfEndCap; }
      set
      {
        if (!(_forceVisibilityOfEndCap == value))
        {
          _forceVisibilityOfEndCap = value;
          EhSelfChanged(EventArgs.Empty);
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
    /// True if the dash pattern of the error bar line is independent on the dash pattern of the line style.
    /// </summary>
    public bool IndependentDashPattern
    {
      get { return _independentDashPattern; }
      set
      {
        if (!(_independentDashPattern == value))
        {
          _independentDashPattern = value;
          EhSelfChanged(EventArgs.Empty);
        }
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
    /// True when no vertical, but horizontal error bars are shown.
    /// </summary>
    public abstract int AxisNumber { get; }

    /// <summary>Pen used to draw the error bar.</summary>
    public PenX Pen
    {
      get { return _pen; }
      set
      {
        if (null == value)
          throw new ArgumentNullException(nameof(value));

        if (!object.Equals(_pen, value))
        {
          ChildCopyToMember(ref _pen, value);
          EhSelfChanged(EventArgs.Empty);
        }
      }
    }

    public bool UseCommonErrorColumn
    {
      get
      {
        return _useCommonErrorColumn;
      }
      set
      {
        if (value == _useCommonErrorColumn)
          return;

        _useCommonErrorColumn = value;
        if (value)
        {
          CommonErrorColumn = _positiveErrorColumn?.Document ?? _negativeErrorColumn?.Document;
          ChildSetMember(ref _positiveErrorColumn, null);
          ChildSetMember(ref _negativeErrorColumn, null);
        }
        else
        {
          PositiveErrorColumn = _commonErrorColumn?.Document;
          NegativeErrorColumn = _commonErrorColumn?.Document;
          ChildSetMember(ref _commonErrorColumn, null);
        }

        EhSelfChanged();
      }
    }

    /// <summary>
    /// Data that define the error in the positive direction.
    /// </summary>
    public IReadableColumn CommonErrorColumn
    {
      get
      {
        if (!_useCommonErrorColumn)
          throw new InvalidOperationException("Style is set to use separate columns for positive and negative error!");

        return _commonErrorColumn?.Document;
      }
      set
      {
        if (_useCommonErrorColumn)
        {
          var oldValue = _commonErrorColumn?.Document;
          if (!object.ReferenceEquals(value, oldValue))
          {
            ChildSetMember(ref _commonErrorColumn, null == value ? null : ReadableColumnProxyBase.FromColumn(value));
            EhSelfChanged(EventArgs.Empty);
          }
        }
        else
        {
          var oldValue1 = _positiveErrorColumn?.Document;
          var oldValue2 = _negativeErrorColumn?.Document;
          if (!object.ReferenceEquals(value, oldValue1) || !object.ReferenceEquals(value, oldValue2))
          {
            ChildSetMember(ref _positiveErrorColumn, null == value ? null : ReadableColumnProxyBase.FromColumn(value));
            ChildSetMember(ref _negativeErrorColumn, null == value ? null : ReadableColumnProxyBase.FromColumn(value));

            EhSelfChanged(EventArgs.Empty);
          }
        }
      }
    }

    /// <summary>
    /// Gets the name of the common error column, if it is a data column. Otherwise, null is returned.
    /// </summary>
    /// <value>
    /// The name of the common error column if it is a data column. Otherwise, null.
    /// </value>
    public string CommonErrorColumnDataColumnName
    {
      get
      {
        return _commonErrorColumn?.DocumentPath?.LastPartOrDefault;
      }
    }

    /// <summary>
    /// Data that define the error in the positive direction.
    /// </summary>
    public IReadableColumn PositiveErrorColumn
    {
      get
      {
        return _useCommonErrorColumn ? _commonErrorColumn?.Document : _positiveErrorColumn?.Document;
      }
      set
      {
        if (_useCommonErrorColumn)
          throw new InvalidOperationException("Style is set to use a common column for positive and negative error!");

        var oldValue = _positiveErrorColumn?.Document;
        if (!object.ReferenceEquals(value, oldValue))
        {
          ChildSetMember(ref _positiveErrorColumn, null == value ? null : ReadableColumnProxyBase.FromColumn(value));
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
    public string PositiveErrorColumnDataColumnName
    {
      get
      {
        return _positiveErrorColumn?.DocumentPath?.LastPartOrDefault;
      }
    }

    /// <summary>
    /// Data that define the error in the negative direction.
    /// </summary>
    public IReadableColumn NegativeErrorColumn
    {
      get
      {
        return _useCommonErrorColumn ? _commonErrorColumn?.Document : _negativeErrorColumn?.Document;
      }
      set
      {
        if (_useCommonErrorColumn)
          throw new InvalidOperationException("Style is set to use a common column for positive and negative error!");

        var oldValue = _negativeErrorColumn?.Document;
        if (!object.ReferenceEquals(value, oldValue))
        {
          ChildSetMember(ref _negativeErrorColumn, null == value ? null : ReadableColumnProxyBase.FromColumn(value));
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
    public string NegativeErrorColumnDataColumnName
    {
      get
      {
        return _negativeErrorColumn?.DocumentPath?.LastPartOrDefault;
      }
    }

    #endregion Properties

    #region IG2DPlotStyle Members

    public void CollectExternalGroupStyles(Altaxo.Graph.Gdi.Plot.Groups.PlotGroupStyleCollection externalGroups)
    {
      if (!_independentColor)
        Graph.Plot.Groups.ColorGroupStyle.AddExternalGroupStyle(externalGroups);
    }

    public void CollectLocalGroupStyles(Altaxo.Graph.Gdi.Plot.Groups.PlotGroupStyleCollection externalGroups, Altaxo.Graph.Gdi.Plot.Groups.PlotGroupStyleCollection localGroups)
    {
      if (!_independentColor)
        Graph.Plot.Groups.ColorGroupStyle.AddLocalGroupStyle(externalGroups, localGroups);

      SkipFrequencyGroupStyle.AddLocalGroupStyle(externalGroups, localGroups); // (local group only)
      IgnoreMissingDataPointsGroupStyle.AddLocalGroupStyle(externalGroups, localGroups);
    }

    public void PrepareGroupStyles(Altaxo.Graph.Gdi.Plot.Groups.PlotGroupStyleCollection externalGroups, Altaxo.Graph.Gdi.Plot.Groups.PlotGroupStyleCollection localGroups, IPlotArea layer, Altaxo.Graph.Gdi.Plot.Data.Processed2DPlotData pdata)
    {
      if (!_independentColor)
        Graph.Plot.Groups.ColorGroupStyle.PrepareStyle(externalGroups, localGroups, delegate ()
        { return _pen.Color; });

      if (!_independentSkipFrequency)
        SkipFrequencyGroupStyle.PrepareStyle(externalGroups, localGroups, delegate ()
        { return SkipFrequency; });

      // IgnoreMissingDataPoints should be the same for all sub plot styles, so there is no "private" property
      IgnoreMissingDataPointsGroupStyle.PrepareStyle(externalGroups, localGroups, () => _ignoreMissingDataPoints);

      // note: symbol size and barposition are only applied, but not prepared
      // this item can not be used as provider of a symbol size
    }

    public void ApplyGroupStyles(Altaxo.Graph.Gdi.Plot.Groups.PlotGroupStyleCollection externalGroups, Altaxo.Graph.Gdi.Plot.Groups.PlotGroupStyleCollection localGroups)
    {
      // IgnoreMissingDataPoints is the same for all sub plot styles
      IgnoreMissingDataPointsGroupStyle.ApplyStyle(externalGroups, localGroups, (ignoreMissingDataPoints) => _ignoreMissingDataPoints = ignoreMissingDataPoints);

      _cachedColorForIndexFunction = null;
      _cachedSymbolSizeForIndexFunction = null;
      // color
      if (!_independentColor)
      {
        ColorGroupStyle.ApplyStyle(externalGroups, localGroups, delegate (NamedColor c)
        { _pen.Color = c; });

        // but if there is a color evaluation function, then use that function with higher priority
        VariableColorGroupStyle.ApplyStyle(externalGroups, localGroups, delegate (Func<int, Color> evalFunc)
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

      // dash style
      if (!_independentDashPattern)
      {
        DashPatternGroupStyle.ApplyStyle(externalGroups, localGroups, delegate (IDashPattern dashPattern)
        { _pen.DashPattern = dashPattern; });
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

    public void Paint(System.Drawing.Graphics g, IPlotArea layer, Altaxo.Graph.Gdi.Plot.Data.Processed2DPlotData pdata, Processed2DPlotData prevItemData, Processed2DPlotData nextItemData)
    {
      // adjust the skip frequency if it was not set appropriate
      if (_skipFrequency <= 0)
        _skipFrequency = 1;

      if (_independentOnShiftingGroupStyles)
      {
        _cachedLogicalShiftX = _cachedLogicalShiftY = 0;
      }

      PlotRangeList rangeList = pdata.RangeList;

      if (_ignoreMissingDataPoints)
      {
        // in case we ignore the missing points, all ranges can be plotted
        // as one range, i.e. continuously
        // for this, we create the totalRange, which contains all ranges
        var totalRange = new PlotRangeCompound(rangeList);
        PaintOneRange(AxisNumber, g, layer, totalRange, pdata);
      }
      else // we not ignore missing points, so plot all ranges separately
      {
        for (int i = 0; i < rangeList.Count; i++)
        {
          PaintOneRange(AxisNumber, g, layer, rangeList[i], pdata);
        }
      }
    }

    protected void PaintOneRange(int axisNumber, Graphics g, IPlotArea layer, IPlotRange range, Processed2DPlotData pdata)
    {
      const double logicalClampMinimum = -10;
      const double logicalClampMaximum = 11;

      _skipFrequency = Math.Max(1, _skipFrequency);

      // Plot error bars for the dependent variable (y)
      var ptArray = pdata.PlotPointsInAbsoluteLayerCoordinates;
      var posErrCol = PositiveErrorColumn;
      var negErrCol = NegativeErrorColumn;

      if (null != posErrCol && !typeof(double).IsAssignableFrom(posErrCol.ItemType))
        posErrCol = null; // TODO make this an runtime paint error to be reported

      if (null != negErrCol && !typeof(double).IsAssignableFrom(negErrCol.ItemType))
        negErrCol = null; // TODO make this an runtime paint error to be reported

      if (posErrCol == null && negErrCol == null)
        return; // nothing to do if both error columns are null

      var strokePen = _pen.Clone();

      var errorBarPath = new System.Drawing.Drawing2D.GraphicsPath();

      Region oldClippingRegion = g.Clip;
      var newClip = oldClippingRegion.Clone();

      int lower = range.LowerBound;
      int upper = range.UpperBound;

      for (int j = lower; j < upper; j += _skipFrequency)
      {
        int originalRowIndex = range.GetOriginalRowIndexFromPlotPointIndex(j);
        double symbolSize = null == _cachedSymbolSizeForIndexFunction ? _symbolSize : _cachedSymbolSizeForIndexFunction(originalRowIndex);
        strokePen.Width = (_lineWidth1Offset + _lineWidth1Factor * symbolSize);

        if (null != _cachedColorForIndexFunction)
          strokePen.Color = GdiColorHelper.ToNamedColor(_cachedColorForIndexFunction(originalRowIndex), "VariableColor");
        if (null != strokePen.EndCap)
          strokePen.EndCap = strokePen.EndCap.WithMinimumAbsoluteAndRelativeSize(symbolSize * _endCapSizeFactor + _endCapSizeOffset, 1 + 1E-6);

        AltaxoVariant vMeanPhysical = pdata.GetPhysical(axisNumber, originalRowIndex);
        Logical3D logicalMean = layer.GetLogical3D(pdata, originalRowIndex);
        logicalMean.RX += _cachedLogicalShiftX;
        logicalMean.RY += _cachedLogicalShiftY;

        if (!Calc.RMath.IsInIntervalCC(logicalMean.RX, logicalClampMinimum, logicalClampMaximum))
          continue;
        if (!Calc.RMath.IsInIntervalCC(logicalMean.RY, logicalClampMinimum, logicalClampMaximum))
          continue;

        var vMeanLogical = logicalMean.GetR(axisNumber);

        Logical3D logicalPos = logicalMean;
        Logical3D logicalNeg = logicalMean;
        bool logicalPosValid = false;
        bool logicalNegValid = false;

        switch (_meaningOfValues)
        {
          case ValueInterpretation.AbsoluteError:
            {
              if (posErrCol != null)
              {
                var vPosLogical = layer.Scales[axisNumber].PhysicalVariantToNormal(vMeanPhysical + Math.Abs(posErrCol[originalRowIndex]));
                vPosLogical = Calc.RMath.ClampToInterval(vPosLogical, logicalClampMinimum, logicalClampMaximum);
                logicalPos.SetR(axisNumber, vPosLogical);
                logicalPosValid = !logicalPos.IsNaN && vPosLogical != vMeanLogical;
              }

              if (negErrCol != null)
              {
                var vNegLogical = layer.Scales[axisNumber].PhysicalVariantToNormal(vMeanPhysical - Math.Abs(negErrCol[originalRowIndex]));
                vNegLogical = Calc.RMath.ClampToInterval(vNegLogical, logicalClampMinimum, logicalClampMaximum);
                logicalNeg.SetR(axisNumber, vNegLogical);
                logicalNegValid = !logicalNeg.IsNaN && vNegLogical != vMeanLogical;
              }
            }
            break;

          case ValueInterpretation.RelativeError:
            {
              if (posErrCol != null)
              {
                var vPosLogical = layer.Scales[axisNumber].PhysicalVariantToNormal(vMeanPhysical * (1 + Math.Abs(posErrCol[originalRowIndex])));
                vPosLogical = Calc.RMath.ClampToInterval(vPosLogical, logicalClampMinimum, logicalClampMaximum);
                logicalPos.SetR(axisNumber, vPosLogical);
                logicalPosValid = !logicalPos.IsNaN && vPosLogical != vMeanLogical;
              }

              if (negErrCol != null)
              {
                var vNegLogical = layer.Scales[axisNumber].PhysicalVariantToNormal(vMeanPhysical * (1 - Math.Abs(negErrCol[originalRowIndex])));
                vNegLogical = Calc.RMath.ClampToInterval(vNegLogical, logicalClampMinimum, logicalClampMaximum);
                logicalNeg.SetR(axisNumber, vNegLogical);
                logicalNegValid = !logicalNeg.IsNaN && vNegLogical != vMeanLogical;
              }
            }
            break;

          case ValueInterpretation.AbsoluteValue:
            {
              if (posErrCol != null)
              {
                var vPosLogical = layer.Scales[axisNumber].PhysicalVariantToNormal(posErrCol[originalRowIndex]);
                vPosLogical = Calc.RMath.ClampToInterval(vPosLogical, logicalClampMinimum, logicalClampMaximum);
                logicalPos.SetR(axisNumber, vPosLogical);
                logicalPosValid = !logicalPos.IsNaN && vPosLogical != vMeanLogical;
              }

              if (negErrCol != null)
              {
                var vNegLogical = layer.Scales[axisNumber].PhysicalVariantToNormal(negErrCol[originalRowIndex]);
                vNegLogical = Calc.RMath.ClampToInterval(vNegLogical, logicalClampMinimum, logicalClampMaximum);
                logicalNeg.SetR(axisNumber, vNegLogical);
                logicalNegValid = !logicalNeg.IsNaN && vNegLogical != vMeanLogical;
              }

              if (object.ReferenceEquals(negErrCol, posErrCol))
              {
                logicalNegValid = false; // then we need only to plot the positive column, since both colums are identical
              }
            }
            break;
        } // end switch

        if (!(logicalPosValid || logicalNegValid))
          continue; // nothing to do for this point if both pos and neg logical point are invalid.

        if (logicalNegValid)
        {
          errorBarPath.Reset();
          layer.CoordinateSystem.GetIsoline(errorBarPath, logicalMean, logicalNeg);
          PointF[] shortenedPathPoints = null;
          bool shortenedPathPointsCalculated = false;
          if (_useSymbolGap)
          {
            double gap = _symbolGapOffset + _symbolGapFactor * symbolSize;
            if (gap > 0)
            {
              errorBarPath.Flatten();
              var pathPoints = errorBarPath.PathPoints;
              shortenedPathPoints = GdiExtensionMethods.ShortenedBy(pathPoints, RADouble.NewAbs(gap / 2), RADouble.NewAbs(0));
              shortenedPathPointsCalculated = true;
              if (null == shortenedPathPoints && _forceVisibilityOfEndCap && !(strokePen.EndCap is Altaxo.Graph.Gdi.LineCaps.FlatCap))
              {
                var totalLineLength = GdiExtensionMethods.TotalLineLength(pathPoints);
                var shortTheLineBy = Math.Max(0, totalLineLength - 0.125 * strokePen.Width);
                shortenedPathPoints = GdiExtensionMethods.ShortenedBy(pathPoints, RADouble.NewAbs(shortTheLineBy), RADouble.NewAbs(0));
              }
            }
          }

          if (shortenedPathPointsCalculated)
          {
            if (null != shortenedPathPoints)
            {
              g.DrawLines(strokePen, shortenedPathPoints);
            }
          }
          else
          {
            g.DrawPath(strokePen, errorBarPath);
          }
        }

        if (logicalPosValid)
        {
          errorBarPath.Reset();
          layer.CoordinateSystem.GetIsoline(errorBarPath, logicalMean, logicalPos);
          PointF[] shortenedPathPoints = null;
          bool shortenedPathPointsCalculated = false;

          if (_useSymbolGap)
          {
            double gap = _symbolGapOffset + _symbolGapFactor * symbolSize;
            if (gap > 0)
            {
              errorBarPath.Flatten();
              var pathPoints = errorBarPath.PathPoints;
              shortenedPathPoints = GdiExtensionMethods.ShortenedBy(pathPoints, RADouble.NewAbs(gap / 2), RADouble.NewAbs(0));
              shortenedPathPointsCalculated = true;
              if (null == shortenedPathPoints && _forceVisibilityOfEndCap && !(strokePen.EndCap is Altaxo.Graph.Gdi.LineCaps.FlatCap))
              {
                var totalLineLength = GdiExtensionMethods.TotalLineLength(pathPoints);
                var shortTheLineBy = Math.Max(0, totalLineLength - 0.125 * strokePen.Width);
                shortenedPathPoints = GdiExtensionMethods.ShortenedBy(pathPoints, RADouble.NewAbs(shortTheLineBy), RADouble.NewAbs(0));
              }
            }
          }

          if (shortenedPathPointsCalculated)
          {
            if (null != shortenedPathPoints)
            {
              g.DrawLines(strokePen, shortenedPathPoints);
            }
          }
          else
          {
            g.DrawPath(strokePen, errorBarPath);
          }
        }
      }

      g.Clip = oldClippingRegion;
    }

    public System.Drawing.RectangleF PaintSymbol(System.Drawing.Graphics g, System.Drawing.RectangleF bounds)
    {
      // Error bars are not painted in the symbol
      return bounds;
    }

    /// <summary>
    /// Prepares the scale of this plot style. Since this style does not utilize a scale, this function does nothing.
    /// </summary>
    /// <param name="layer">The parent layer.</param>
    public void PrepareScales(IPlotArea layer)
    {
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
      if (null != _commonErrorColumn)
        Report(_commonErrorColumn, this, nameof(CommonErrorColumn));
      if (null != _positiveErrorColumn)
        Report(_positiveErrorColumn, this, nameof(PositiveErrorColumn));
      if (null != _negativeErrorColumn)
        Report(_negativeErrorColumn, this, nameof(NegativeErrorColumn));
    }

    /// <summary>
    /// Gets the columns used additionally by this style, e.g. the label column for a label plot style, or the error columns for an error bar plot style.
    /// </summary>
    /// <returns>An enumeration of tuples. Each tuple consist of the column name, as it should be used to identify the column in the data dialog. The second item of this
    /// tuple is a function that returns the column proxy for this column, in order to get the underlying column or to set the underlying column.</returns>
    public IEnumerable<(
      string ColumnLabel, // Column label
      IReadableColumn Column, // the column as it was at the time of this call
      string ColumnName, // the name of the column (last part of the column proxies document path)
      Action<IReadableColumn> ColumnSetAction // action to set the column during Apply of the controller
      )> GetAdditionallyUsedColumns()
    {
      if (_useCommonErrorColumn)
      {
        yield return (nameof(CommonErrorColumn), CommonErrorColumn, _commonErrorColumn?.DocumentPath?.LastPartOrDefault, (col) => CommonErrorColumn = col as INumericColumn);
      }
      else
      {
        yield return (nameof(PositiveErrorColumn), PositiveErrorColumn, _positiveErrorColumn?.DocumentPath?.LastPartOrDefault, (col) => PositiveErrorColumn = col as INumericColumn);

        yield return (nameof(NegativeErrorColumn), NegativeErrorColumn, _negativeErrorColumn?.DocumentPath?.LastPartOrDefault, (col) => NegativeErrorColumn = col as INumericColumn);
      }
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

  #endregion Error bar (abstract, for implementations see below)

  #region Error bar x

  public class ErrorBarXPlotStyle : ErrorBarPlotStyle
  {
    public override int AxisNumber { get { return 0; } }

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ErrorBarXPlotStyle), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        info.AddBaseValueEmbedded(obj, obj.GetType().BaseType);
      }

      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        var s = (ErrorBarXPlotStyle)o ?? new ErrorBarXPlotStyle(info);
        info.GetBaseValueEmbedded(s, s.GetType().BaseType, parent);
        return s;
      }
    }

    #endregion Serialization

    /// <summary>
    /// Deserialization constructor
    /// </summary>
    /// <param name="info">The information.</param>
    protected ErrorBarXPlotStyle(Altaxo.Serialization.Xml.IXmlDeserializationInfo info) : base(info)
    {
    }

    /// <summary>
    /// Deserialization constructor
    /// </summary>
    /// <param name="posErrorColumn">The positive error column.</param>
    /// <param name="negErrorColumn">The negative error column.</param>
    /// <param name="info">The information.</param>
    public ErrorBarXPlotStyle(Altaxo.Serialization.Xml.IXmlDeserializationInfo info, IReadableColumnProxy posErrorColumn, IReadableColumnProxy negErrorColumn) : base(info, posErrorColumn, negErrorColumn)
    {
    }

    public ErrorBarXPlotStyle(Altaxo.Main.Properties.IReadOnlyPropertyBag context) : base(context)
    {
    }

    public ErrorBarXPlotStyle(ErrorBarPlotStyle from, bool copyWithDataReferences) : base(from, copyWithDataReferences)
    {
    }

    /// <inheritdoc/>
    public override object Clone()
    {
      return new ErrorBarXPlotStyle(this, true);
    }

    /// <inheritdoc/>
    public override object Clone(bool copyWithDataReferences)
    {
      return new ErrorBarXPlotStyle(this, copyWithDataReferences);
    }
  }

  #endregion Error bar x

  #region Error bar y

  public class ErrorBarYPlotStyle : ErrorBarPlotStyle
  {
    public override int AxisNumber { get { return 1; } }

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ErrorBarYPlotStyle), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        info.AddBaseValueEmbedded(obj, obj.GetType().BaseType);
      }

      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        var s = (ErrorBarYPlotStyle)o ?? new ErrorBarYPlotStyle(info);
        info.GetBaseValueEmbedded(s, s.GetType().BaseType, parent);
        return s;
      }
    }

    #endregion Serialization

    /// <summary>
    /// Deserialization constructor
    /// </summary>
    /// <param name="info">The information.</param>
    protected ErrorBarYPlotStyle(Altaxo.Serialization.Xml.IXmlDeserializationInfo info) : base(info)
    {
    }

    /// <summary>
    /// Deserialization constructor
    /// </summary>
    /// <param name="posErrorColumn">The positive error column.</param>
    /// <param name="negErrorColumn">The negative error column.</param>
    /// <param name="info">The information.</param>
    public ErrorBarYPlotStyle(Altaxo.Serialization.Xml.IXmlDeserializationInfo info, IReadableColumnProxy posErrorColumn, IReadableColumnProxy negErrorColumn) : base(info, posErrorColumn, negErrorColumn)
    {
    }

    public ErrorBarYPlotStyle(Altaxo.Main.Properties.IReadOnlyPropertyBag context) : base(context)
    {
    }

    public ErrorBarYPlotStyle(ErrorBarPlotStyle from, bool copyWithDataReferences) : base(from, copyWithDataReferences)
    {
    }

    /// <inheritdoc/>
    public override object Clone()
    {
      return new ErrorBarYPlotStyle(this, true);
    }

    /// <inheritdoc/>
    public override object Clone(bool copyWithDataReferences)
    {
      return new ErrorBarYPlotStyle(this, copyWithDataReferences);
    }
  }

  #endregion Error bar y
}
