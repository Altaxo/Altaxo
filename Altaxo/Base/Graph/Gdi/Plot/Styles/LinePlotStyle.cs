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

#nullable enable
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using Altaxo.Serialization;

namespace Altaxo.Graph.Gdi.Plot.Styles
{
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

  namespace XYPlotLineStyles
  {
    [Serializable]
    public enum FillDirection
    {
      Left = 0,
      Bottom = 1,
      Right = 2,
      Top = 3
    }

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.XYPlotLineStyles.FillDirection", 0)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(FillDirection), 1)]
    public class FillDirectionXmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (FillDirection)obj;
        info.SetNodeContent(s.ToString());
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        string val = info.GetNodeContent();
        return System.Enum.Parse(typeof(FillDirection), val, true);
      }
    }

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.XYPlotLineStyles.ConnectionStyle", 0)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.Gdi.Plot.Styles.XYPlotLineStyles.ConnectionStyle", 1)]
    public class ConnectionStyleXmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        throw new InvalidOperationException("Serialization of old version");
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        string val = info.GetNodeContent();
        switch (val)
        {
          case "NoLine":
            return LineConnectionStyles.NoConnection.Instance;

          case "Straight":
            return LineConnectionStyles.StraightConnection.Instance;

          case "Segment2":
            return LineConnectionStyles.Segment2Connection.Instance;

          case "Segment3":
            return LineConnectionStyles.Segment3Connection.Instance;

          case "Spline":
            return LineConnectionStyles.SplineConnection.Instance;

          case "Bezier":
            return LineConnectionStyles.BezierConnection.Instance;

          case "StepHorz":
            return LineConnectionStyles.StepHorizontalConnection.Instance;

          case "StepVert":
            return LineConnectionStyles.StepVerticalConnection.Instance;

          case "StepHorzCenter":
            return LineConnectionStyles.StepHorizontalCenteredConnection.Instance;

          case "StepVertCenter":
            return LineConnectionStyles.StepVerticalCenteredConnection.Instance;

          default:
            throw new NotImplementedException();
        }
      }
    }
  }

  /// <summary>
  /// Summary description for XYPlotLineStyle.
  /// </summary>
  public class LinePlotStyle
    :
    Main.SuspendableDocumentNodeWithEventArgs,
    IG2DPlotStyle,
    IRoutedPropertyReceiver
  {
    /// <summary>A value indicating whether the skip frequency value is independent from other values.</summary>
    protected bool _independentSkipFrequency;

    /// <summary>A value of 2 skips every other data point, a value of 3 skips 2 out of 3 data points, and so on.</summary>
    protected int _skipFrequency = 1;

    /// <summary>
    /// If true, treat missing points as if not present (e.g. connect lines over missing points, continue with skip over missing points).
    /// </summary>
    protected bool _ignoreMissingDataPoints; // treat missing points as if not present (connect lines over missing points)

    /// <summary>If true, group styles that shift the logical position of the items (for instance <see cref="BarSizePosition3DGroupStyle"/>) are not applied. I.e. when true, the position of the item remains unperturbed.</summary>
    private bool _independentOnShiftingGroupStyles = true;

    protected bool _independentColor;

    protected bool _independentDashStyle;

    protected PenX _linePen;

    protected ILineConnectionStyle _connectionStyle;

    /// <summary>
    /// true if the symbol size is independent, i.e. is not published nor updated by a group style.
    /// </summary>
    protected bool _independentSymbolSize;

    /// <summary>Controls the length of the end bar.</summary>
    protected double _symbolSize;

    /// <summary>If true, the start and the end point of the line are connected too.</summary>
    protected bool _connectCircular;

    protected bool _useSymbolGap;

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

    /// <summary>Logical x shift between the location of the real data point and the point where the item is finally drawn.</summary>
    private double _cachedLogicalShiftX;

    /// <summary>Logical y shift between the location of the real data point and the point where the item is finally drawn.</summary>
    private double _cachedLogicalShiftY;

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.XYPlotLineStyle", 0)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.XYPlotLineStyle", 1)] // by accident, it was never different from 0
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        throw new InvalidOperationException("Serialization of old version not allowed");
        /*
                LinePlotStyle s = (LinePlotStyle)obj;
                info.AddValue("Pen", s._linePen);
                info.AddValue("Connection", s._connectionStyle);
                info.AddValue("LineSymbolGap", s._useSymbolGap);
                info.AddValue("IgnoreMissingPoints", s._ignoreMissingDataPoints);
                info.AddValue("FillArea", s._fillArea);
                info.AddValue("FillBrush", s._fillBrush);
                info.AddValue("FillDirection", s._fillDirection);
                */
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = SDeserialize(o, info, parent);
        return s;
      }

      public virtual object SDeserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (LinePlotStyle?)o ?? new LinePlotStyle(info);

        s._linePen = (PenX)info.GetValue("Pen", s);
        s.Connection = (ILineConnectionStyle)info.GetValue("Connection", s);
        s._useSymbolGap = info.GetBoolean("LineSymbolGap");
        s._ignoreMissingDataPoints = info.GetBoolean("IgnoreMissingPoints");
        bool fillArea = info.GetBoolean("FillArea");
        var fillBrush = (BrushX)info.GetValue("FillBrush", s);
        var fillDir = (XYPlotLineStyles.FillDirection)info.GetValue("FillDirection", s);

        if (!fillArea)
        {
          return s;
        }
        else
        {
          var drop = new DropAreaPlotStyle(s.Connection, s.IgnoreMissingDataPoints, false, GetFillDirection(fillDir), fillBrush, ColorLinkage.PreserveAlpha);
          return new object[] { s, drop };
        }
      }

      public static CSPlaneID GetFillDirection(XYPlotLineStyles.FillDirection fillDir)
      {
        switch (fillDir)
        {
          case XYPlotLineStyles.FillDirection.Bottom:
            return CSPlaneID.Bottom;

          case XYPlotLineStyles.FillDirection.Top:
            return CSPlaneID.Top;

          case XYPlotLineStyles.FillDirection.Left:
            return CSPlaneID.Left;

          case XYPlotLineStyles.FillDirection.Right:
            return CSPlaneID.Right;
          default:
            throw new NotImplementedException();
        }

      }
    }

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.XYPlotLineStyle", 2)]
    private class XmlSerializationSurrogate2 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        throw new InvalidOperationException("Try to serialize old version");
        /*
                base.Serialize(obj, info);
                LinePlotStyle s = (LinePlotStyle)obj;
                info.AddValue("IndependentColor", s._independentColor);
                */
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (LinePlotStyle?)o ?? new LinePlotStyle(info);

        s._linePen = (PenX)info.GetValue("Pen", s);
        s.Connection = (ILineConnectionStyle)info.GetValue("Connection", s);
        s._useSymbolGap = info.GetBoolean("LineSymbolGap");
        s._ignoreMissingDataPoints = info.GetBoolean("IgnoreMissingPoints");
        bool fillArea = info.GetBoolean("FillArea");
        var fillBrush = (BrushX)info.GetValue("FillBrush", s);
        var fillDir = (XYPlotLineStyles.FillDirection)info.GetValue("FillDirection", s);
        s._independentColor = info.GetBoolean("IndependentColor");

        if (!fillArea)
        {
          return s;
        }
        else
        {
          var drop = new DropAreaPlotStyle(s.Connection, s.IgnoreMissingDataPoints, false, XmlSerializationSurrogate0.GetFillDirection(fillDir), fillBrush, ColorLinkage.PreserveAlpha);
          return new object[] { s, drop };
        }
      }
    }

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.Gdi.Plot.Styles.LinePlotStyle", 3)]
    private class XmlSerializationSurrogate3 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        throw new InvalidOperationException("Try to serialize old version");
        /*
                base.Serialize(obj, info);
                LinePlotStyle s = (LinePlotStyle)obj;
                info.AddValue("IndependentColor", s._independentColor);
                info.AddValue("IndependentFillColor", s._independentFillColor);
                info.AddValue("ConnectCircular", s._connectCircular);
                */
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (LinePlotStyle?)o ?? new LinePlotStyle(info);

        s._linePen = (PenX)info.GetValue("Pen", s);
        s.Connection = (ILineConnectionStyle)info.GetValue("Connection", s);
        s._useSymbolGap = info.GetBoolean("LineSymbolGap");
        s._ignoreMissingDataPoints = info.GetBoolean("IgnoreMissingPoints");
        bool fillArea = info.GetBoolean("FillArea");
        var fillBrush = (BrushX)info.GetValue("FillBrush", s);
        var fillDir = info.GetValueOrNull<CSPlaneID>("FillDirection", s) ?? CSPlaneID.Bottom;
        s._independentColor = info.GetBoolean("IndependentColor");
        var fillColorLinkage = info.GetBoolean("IndependentFillColor") ? ColorLinkage.Independent : ColorLinkage.PreserveAlpha;
        s._connectCircular = info.GetBoolean("ConnectCircular");

        if (!fillArea)
        {
          return s;
        }
        else
        {
          var drop = new DropAreaPlotStyle(s.Connection, s.IgnoreMissingDataPoints, false, fillDir, fillBrush, fillColorLinkage);
          return new object[] { s, drop };
        }
      }
    }

    /// <summary>
    /// <para>Date: 2012-10-10</para>
    /// Change: instead _independentFillColor being a boolean value, it is now a ColorLinkage enumeration value
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.Gdi.Plot.Styles.LinePlotStyle", 4)]
    private class XmlSerializationSurrogate4 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        throw new InvalidOperationException("Serialization of old version not allowed");
        /*
                base.Serialize(obj, info);
                LinePlotStyle s = (LinePlotStyle)obj;
                info.AddValue("IndependentColor", s._independentColor);
                info.AddEnum("FillColorLinkage", s._fillColorLinkage);
                info.AddValue("ConnectCircular", s._connectCircular);
                */
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (LinePlotStyle?)o ?? new LinePlotStyle(info);

        s._linePen = (PenX)info.GetValue("Pen", s);
        s.Connection = (ILineConnectionStyle)info.GetValue("Connection", s);
        s._useSymbolGap = info.GetBoolean("LineSymbolGap");
        s._ignoreMissingDataPoints = info.GetBoolean("IgnoreMissingPoints");
        bool fillArea = info.GetBoolean("FillArea");
        var fillBrush = (BrushX)info.GetValue("FillBrush", s);
        var fillDir = info.GetValueOrNull<CSPlaneID>("FillDirection", s) ?? CSPlaneID.Bottom;
        s._independentColor = info.GetBoolean("IndependentColor");
        var fillColorLinkage = (ColorLinkage)info.GetEnum("FillColorLinkage", typeof(ColorLinkage));
        s._connectCircular = info.GetBoolean("ConnectCircular");

        if (!fillArea)
        {
          return s;
        }
        else
        {
          var drop = new DropAreaPlotStyle(s.Connection, s.IgnoreMissingDataPoints, false, fillDir, fillBrush, fillColorLinkage);
          return new object[] { s, drop };
        }
      }
    }

    /// <summary>
    /// 2016-11-04 Major changes: Fill style is now in a separate style. Many new properties.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(LinePlotStyle), 5)]
    private class XmlSerializationSurrogate5 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (LinePlotStyle)obj;

        info.AddValue("IndependentSkipFreq", s._independentSkipFrequency);
        info.AddValue("SkipFreq", s._skipFrequency);

        info.AddValue("IgnoreMissingDataPoints", s._ignoreMissingDataPoints);
        info.AddValue("IndependentOnShiftingGroupStyles", s._independentOnShiftingGroupStyles);

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
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (LinePlotStyle?)o ?? new LinePlotStyle(info);

        s._independentSkipFrequency = info.GetBoolean("IndependentSkipFreq");
        s._skipFrequency = info.GetInt32("SkipFreq");

        s._ignoreMissingDataPoints = info.GetBoolean("IgnoreMissingDataPoints");
        s._independentOnShiftingGroupStyles = info.GetBoolean("IndependentOnShiftingGroupStyles");

        s._connectCircular = info.GetBoolean("ConnectCircular");
        s._connectionStyle = (ILineConnectionStyle)info.GetValue("Connection", s);

        s._linePen = (PenX)info.GetValue("Pen", s);
        s._independentDashStyle = info.GetBoolean("IndependentDashStyle");
        s._independentColor = info.GetBoolean("IndependentColor");

        s._independentSymbolSize = info.GetBoolean("IndependentSymbolSize");
        s._symbolSize = info.GetDouble("SymbolSize");

        s._useSymbolGap = info.GetBoolean("UseSymbolGap");
        s._symbolGapOffset = info.GetDouble("SymbolGapOffset");
        s._symbolGapFactor = info.GetDouble("SymbolGapFactor");

        return s;
      }
    }

    #endregion Serialization

    #region Construction and copying

    [MemberNotNull(nameof(_linePen), nameof(_connectionStyle))]
    public void CopyFrom(LinePlotStyle from, Main.EventFiring eventFiring)
    {
      if (ReferenceEquals(this, from))
#pragma warning disable CS8774 // Member must have a non-null value when exiting.
        return;
#pragma warning restore CS8774 // Member must have a non-null value when exiting.

      using (var suspendToken = SuspendGetToken())
      {
        _independentSkipFrequency = from._independentSkipFrequency;
        _skipFrequency = from._skipFrequency;

        _ignoreMissingDataPoints = from._ignoreMissingDataPoints;
        _independentOnShiftingGroupStyles = from._independentOnShiftingGroupStyles;

        _connectCircular = from._connectCircular;
        _connectionStyle = from._connectionStyle;

        _linePen = from._linePen;
        _independentDashStyle = from._independentDashStyle;
        _independentColor = from._independentColor;

        _independentSymbolSize = from._independentSymbolSize;
        _symbolSize = from._symbolSize;

        _useSymbolGap = from._useSymbolGap;
        _symbolGapOffset = from._symbolGapOffset;
        _symbolGapFactor = from._symbolGapFactor;

        EhSelfChanged();

        suspendToken.Resume(eventFiring);
      }
    }

    /// <inheritdoc/>
    public bool CopyFrom(object obj, bool copyWithDataReferences)
    {
      if (ReferenceEquals(this, obj))
        return true;
      var from = obj as LinePlotStyle;
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
      return new LinePlotStyle(this);
    }

    /// <inheritdoc/>
    public object Clone()
    {
      return new LinePlotStyle(this);
    }

#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
    protected LinePlotStyle(Altaxo.Serialization.Xml.IXmlDeserializationInfo info)
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
    {
      _connectionStyle = LineConnectionStyles.StraightConnection.Instance;
    }

#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
    internal LinePlotStyle(Altaxo.Serialization.Xml.IXmlDeserializationInfo info, bool oldDeserializationRequiresFullConstruction)
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
    {
      var penWidth = 1;
      var color = ColorSetManager.Instance.BuiltinDarkPlotColors[0];

      _linePen = new PenX(color, penWidth).WithLineJoin(LineJoin.Bevel);
      _useSymbolGap = true;
      _ignoreMissingDataPoints = false;
      _connectionStyle = LineConnectionStyles.StraightConnection.Instance;
      _independentColor = false;
    }

    public LinePlotStyle(Altaxo.Main.Properties.IReadOnlyPropertyBag context)
    {
      var penWidth = GraphDocument.GetDefaultPenWidth(context);
      var color = GraphDocument.GetDefaultPlotColor(context);

      _linePen = new PenX(color, penWidth).WithLineJoin(LineJoin.Bevel);
      _ignoreMissingDataPoints = false;
      _connectionStyle = LineConnectionStyles.StraightConnection.Instance;
      _independentColor = false;
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
      get { return _skipFrequency; }
      set
      {
        value = Math.Max(1, value);
        if (!(_skipFrequency == value))
        {
          _skipFrequency = value;
          EhSelfChanged(EventArgs.Empty); // Fire Changed event
        }
      }
    }

    /// <summary>A value indicating whether the skip frequency value is independent from values in other sub plot styles.</summary>
    public bool IndependentSkipFrequency
    {
      get
      {
        return _independentSkipFrequency;
      }
      set
      {
        if (!(_independentSkipFrequency == value))
        {
          _independentSkipFrequency = value;
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

          if (true == value)
          {
            _cachedLogicalShiftX = _cachedLogicalShiftY = 0;
          }

          EhSelfChanged();
        }
      }
    }

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

    /// <summary>
    /// True when the line is not drawn in the circle of diameter SymbolSize around the symbol center.
    /// </summary>
    public bool UseSymbolGap
    {
      get
      {
        return _useSymbolGap;
      }
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
        if (!(_ignoreMissingDataPoints == value))
        {
          _ignoreMissingDataPoints = value;
          EhSelfChanged(EventArgs.Empty);
        }
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

    public PenX LinePen
    {
      get { return _linePen; }
      set
      {
        if (value is null)
          throw new ArgumentNullException(nameof(value));

        if (!(_linePen == value))
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
        if (!LineConnectionStyles.NoConnection.Instance.Equals(_connectionStyle))
          return true;

        return false;
      }
    }

    #endregion Properties

    #region Painting

    /// <summary>
    /// Prepares the scale of this plot style. Since this style does not utilize a scale, this function does nothing.
    /// </summary>
    /// <param name="layer">The parent layer.</param>
    public void PrepareScales(IPlotArea layer)
    {
    }

    public RectangleF PaintSymbol(System.Drawing.Graphics g, System.Drawing.RectangleF bounds)
    {
      if (!LineConnectionStyles.NoConnection.Instance.Equals(_connectionStyle))
      {
        GraphicsState gs = g.Save();
        g.TranslateTransform(bounds.X + 0.5f * bounds.Width, bounds.Y + 0.5f * bounds.Height);
        float halfwidth = bounds.Width / 2;

        if (_linePen is { } _)
        {
          using (var linePenGdi = PenCacheGdi.Instance.BorrowPen(_linePen))
          {
            if (UseSymbolGap == true)
            {
              // plot a line with the length of symbolsize from
              var symGap = (float)(_symbolGapOffset + _symbolGapFactor * _symbolSize);
              g.DrawLine(linePenGdi, new PointF(-halfwidth, 0), new PointF(-symGap / 2, 0));
              g.DrawLine(linePenGdi, new PointF(symGap / 2, 0), new PointF(halfwidth, 0));
            }
            else // no gap
            {
              g.DrawLine(linePenGdi, new PointF(-halfwidth, 0), new PointF(halfwidth, 0));
            }
          }
        }
        g.Restore(gs);
      }

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


      Func<int, double>? symbolGapFunction = null;

      if (_useSymbolGap)
      {
        if (_cachedSymbolSizeForIndexFunction is not null && !_independentSymbolSize)
        {
          symbolGapFunction = (idx) => _symbolGapOffset + _symbolGapFactor * _cachedSymbolSizeForIndexFunction(idx);
        }
        else if (0 != _symbolGapOffset || 0 != _symbolGapFactor * _symbolSize)
        {
          symbolGapFunction = (idx) => _symbolGapOffset + _symbolGapFactor * _symbolSize;
        }
        else
        {
          symbolGapFunction = null;
        }
      }

      using var linePenGdi = PenCacheGdi.Instance.BorrowPen(_linePen);

      if (_ignoreMissingDataPoints)
      {
        // in case we ignore the missing points, all ranges can be plotted
        // as one range, i.e. continuously
        // for this, we create the totalRange, which contains all ranges
        var totalRange = new PlotRangeCompound(rangeList);
        _connectionStyle.PaintOneRange(g, plotPositions, totalRange, layer, linePenGdi, symbolGapFunction, _skipFrequency, _connectCircular, this);
      }
      else // we not ignore missing points, so plot all ranges separately
      {
        for (int i = 0; i < rangeList.Count; i++)
        {
          _connectionStyle.PaintOneRange(g, plotPositions, rangeList[i], layer, linePenGdi, symbolGapFunction, _skipFrequency, _connectCircular, this);
        }
      }
    }

    public void GetFillPath(GraphicsPath gp, IPlotArea layer, Processed2DPlotData pdata, CSPlaneID fillDirection)
    {


      PointF[]? plotPositions = pdata.PlotPointsInAbsoluteLayerCoordinates;
      if (0 != _cachedLogicalShiftX || 0 != _cachedLogicalShiftY)
      {
        plotPositions = Processed2DPlotData.GetPlotPointsInAbsoluteLayerCoordinatesWithShift(pdata, layer, _cachedLogicalShiftX, _cachedLogicalShiftY);
      }
      var rangeList = pdata.RangeList;

      if (rangeList is null || plotPositions is null)
        return;

      fillDirection = layer.UpdateCSPlaneID(fillDirection);

      int rangelistlen = rangeList.Count;

      if (_ignoreMissingDataPoints)
      {
        // in case we ignore the missing points, all ranges can be plotted
        // as one range, i.e. continuously
        // for this, we create the totalRange, which contains all ranges
        var totalRange = new PlotRange(rangeList[0].LowerBound, rangeList[rangelistlen - 1].UpperBound);
        _connectionStyle.FillOneRange(gp, pdata, totalRange, layer, fillDirection, _ignoreMissingDataPoints, _connectCircular, plotPositions, _cachedLogicalShiftX, _cachedLogicalShiftY);
      }
      else // we not ignore missing points, so plot all ranges separately
      {
        for (int i = 0; i < rangelistlen; i++)
        {
          _connectionStyle.FillOneRange(gp, pdata, rangeList[i], layer, fillDirection, _ignoreMissingDataPoints, _connectCircular, plotPositions, _cachedLogicalShiftX, _cachedLogicalShiftY);
        }
      }
    }

    #endregion Painting

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
        if (!(_linePen.Color == value))
        {
          _linePen = _linePen.WithColor(value);
          EhSelfChanged();
        }
      }
    }

    public bool IsColorReceiver
    {
      get { return !_independentColor; }
    }

    #region IG2DPlotStyle Members

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
      if (IsColorProvider)
        ColorGroupStyle.PrepareStyle(externalGroups, localGroups, delegate ()
        { return Color; });

      if (!_independentDashStyle)
        DashPatternGroupStyle.PrepareStyle(externalGroups, localGroups, delegate
        { return LinePen.DashPattern; });

      IgnoreMissingDataPointsGroupStyle.PrepareStyle(externalGroups, localGroups, () => _ignoreMissingDataPoints);
      LineConnection2DGroupStyle.PrepareStyle(externalGroups, localGroups, () => new Tuple<ILineConnectionStyle, bool>(_connectionStyle, _connectCircular));
    }

    public void ApplyGroupStyles(PlotGroupStyleCollection externalGroups, PlotGroupStyleCollection localGroups)
    {
      // IgnoreMissingDataPoints is the same for all sub plot styles
      IgnoreMissingDataPointsGroupStyle.ApplyStyle(externalGroups, localGroups, (ignoreMissingDataPoints) => _ignoreMissingDataPoints = ignoreMissingDataPoints);

      // LineConnectionStyle is the same for all sub plot styles
      LineConnection2DGroupStyle.ApplyStyle(externalGroups, localGroups, (lineConnection, connectCircular) => { _connectionStyle = lineConnection; _connectCircular = connectCircular; });

      // SkipFrequency should be the same for all sub plot styles
      if (!_independentSkipFrequency)
      {
        _skipFrequency = 1;
        SkipFrequencyGroupStyle.ApplyStyle(externalGroups, localGroups, delegate (int c)
        { _skipFrequency = c; });
      }

      if (IsColorReceiver)
        ColorGroupStyle.ApplyStyle(externalGroups, localGroups, delegate (NamedColor c)
        { Color = c; });

      if (!_independentDashStyle)
        DashPatternGroupStyle.ApplyStyle(externalGroups, localGroups, delegate (IDashPattern c)
        { _linePen = _linePen.WithDashPattern(c); });

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
          yield return (propertyName, _linePen.Width, (w) => _linePen = _linePen.WithWidth((double)w));
          break;

        case "SymbolSize":
          if (_independentSymbolSize)
            yield return (propertyName, _symbolSize, (w) => SymbolSize = (double)w);
          break;
      }

      yield break;
    }

    #endregion IRoutedPropertyReceiver Members
  } // end class XYPlotLineStyle
}
