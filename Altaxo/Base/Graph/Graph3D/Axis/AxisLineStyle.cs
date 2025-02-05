﻿#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2015 Dr. Dirk Lellinger
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
using System.Linq;
using Altaxo.Drawing;
using Altaxo.Geometry;
using Altaxo.Graph.Scales;
using Altaxo.Graph.Scales.Ticks;

namespace Altaxo.Graph.Graph3D.Axis
{
  using System.Diagnostics.CodeAnalysis;
  using Drawing.D3D;
  using GraphicsContext;

  /// <summary>
  /// Responsible for painting the axes lines on 3D layers.
  /// </summary>
  public class AxisLineStyle
    :
    Main.SuspendableDocumentNodeWithSetOfEventArgs,
    IRoutedPropertyReceiver,
    Main.ICopyFrom
  {
    /// <summary>Pen used for painting of the axis.</summary>
    protected PenX3D _axisPen;

    /// <summary>Pen used for painting of the major ticks.</summary>
    protected PenX3D _majorTickPen;

    /// <summary>Pen used for painting of the minor ticks.</summary>
    protected PenX3D _minorTickPen;

    /// <summary>Length of the major ticks in points (1/72 inch).</summary>
    protected double _majorTickLength;

    /// <summary>Length of the minor ticks in points (1/72 inch).</summary>
    protected double _minorTickLength;

    /// <summary>True if major ticks should be painted outside of the layer.</summary>
    protected bool _showFirstUpMajorTicks;

    /// <summary>True if major ticks should be painted inside of the layer.</summary>
    protected bool _showFirstDownMajorTicks;

    /// <summary>True if major ticks should be painted outside of the layer.</summary>
    protected bool _showSecondUpMajorTicks;

    /// <summary>True if major ticks should be painted inside of the layer.</summary>
    protected bool _showSecondDownMajorTicks;

    /// <summary>True if minor ticks should be painted outside of the layer.</summary>
    protected bool _showFirstUpMinorTicks;

    /// <summary>True if major ticks should be painted inside of the layer.</summary>
    protected bool _showFirstDownMinorTicks;

    /// <summary>True if minor ticks should be painted outside of the layer.</summary>
    protected bool _showSecondUpMinorTicks;

    /// <summary>True if major ticks should be painted inside of the layer.</summary>
    protected bool _showSecondDownMinorTicks;

    /// <summary>Axis shift position, either provide as absolute values in point units, or as relative value relative to the layer size.</summary>
    protected RADouble _axisPosition1; // if relative, then relative to layer size, if absolute then in points

    protected RADouble _axisPosition2; // if relative, then relative to layer size, if absolute then in points

    protected CSAxisInformation? _cachedAxisStyleInfo;

    /// <summary>
    /// The line points that make out the main axis line (in parent layer coordinates). Used for hit testing
    /// </summary>
    [NonSerialized]
    protected PointD3D[]? _cachedMainLinePointsUsedForHitTesting;

    /// <summary>The major tick lines cached for hit testing</summary>
    [NonSerialized]
    private LineD3D[]? _cachedMajorTickLinesUsedForHitTesting;

    /// <summary>The minor tick lines cached for hit testing</summary>
    [NonSerialized]
    private LineD3D[]? _cachedMinorTickLinesUsedForHitTesting;

    #region Serialization

    // 2015-09-10 initial version
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(AxisLineStyle), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (AxisLineStyle)obj;
        info.AddValue("AxisPen", s._axisPen);
        info.AddValue("MajorPen", s._majorTickPen);
        info.AddValue("MinorPen", s._minorTickPen);
        info.AddValue("MajorLength", s._majorTickLength);
        info.AddValue("MinorLength", s._minorTickLength);
        info.AddValue("AxisPosition1", s._axisPosition1);
        info.AddValue("AxisPosition2", s._axisPosition2);
        info.AddValue("Major1Up", s._showFirstUpMajorTicks);
        info.AddValue("Major1Dw", s._showFirstDownMajorTicks);
        info.AddValue("Major2Up", s._showSecondUpMajorTicks);
        info.AddValue("Major2Dw", s._showSecondDownMajorTicks);
        info.AddValue("Minor1Up", s._showFirstUpMinorTicks);
        info.AddValue("Minor1Dw", s._showFirstDownMinorTicks);
        info.AddValue("Minor2Up", s._showSecondUpMinorTicks);
        info.AddValue("Minor2Dw", s._showSecondDownMinorTicks);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (AxisLineStyle?)o ?? new AxisLineStyle(info);

        s._axisPen = (PenX3D)info.GetValue("AxisPen", s);
        s._majorTickPen = (PenX3D)info.GetValue("MajorPen", s);
        s._minorTickPen = (PenX3D)info.GetValue("MinorPen", s);

        s._majorTickLength = info.GetDouble("MajorLength");
        s._minorTickLength = info.GetDouble("MinorLength");
        s._axisPosition1 = (RADouble)info.GetValue("AxisPosition1", s);
        s._axisPosition2 = (RADouble)info.GetValue("AxisPosition2", s);
        s._showFirstUpMajorTicks = info.GetBoolean("Major1Up");
        s._showFirstDownMajorTicks = info.GetBoolean("Major1Dw");
        s._showSecondUpMajorTicks = info.GetBoolean("Major2Up");
        s._showSecondDownMajorTicks = info.GetBoolean("Major2Dw");
        s._showFirstUpMinorTicks = info.GetBoolean("Minor1Up");
        s._showFirstDownMinorTicks = info.GetBoolean("Minor1Dw");
        s._showSecondUpMinorTicks = info.GetBoolean("Minor2Up");
        s._showSecondDownMinorTicks = info.GetBoolean("Minor2Dw");

        return s;
      }
    }

    #endregion Serialization

    /// <summary>
    /// Initializes a new instance of the <see cref="AxisLineStyle"/> class for deserialization purposes only.
    /// </summary>
    /// <param name="info">The deserialization information.</param>
#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
    protected AxisLineStyle(Altaxo.Serialization.Xml.IXmlDeserializationInfo info)
    {
    }
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.

    /// <summary>
    /// Creates a default axis style.
    /// </summary>
    public AxisLineStyle(Main.Properties.IReadOnlyPropertyBag context)
    {
      double penWidth = GraphDocument.GetDefaultPenWidth(context);
      double majorTickLength = GraphDocument.GetDefaultMajorTickLength(context);
      var color = GraphDocument.GetDefaultForeColor(context);

      _axisPen = new PenX3D(color, penWidth);
      _majorTickPen = new PenX3D(color, penWidth);
      _minorTickPen = new PenX3D(color, penWidth);
      _majorTickLength = majorTickLength;
      _minorTickLength = majorTickLength / 2;
      _showFirstUpMajorTicks = true; // true if right major ticks should be visible
      _showFirstDownMajorTicks = true; // true if left major ticks should be visible
      _showFirstUpMinorTicks = true; // true if right minor ticks should be visible
      _showFirstDownMinorTicks = true; // true if left minor ticks should be visible
    }

    /// <summary>
    /// Creates a default axis style.
    /// </summary>
    public AxisLineStyle(bool showTicks, CSAxisSide preferredTickSide, Main.Properties.IReadOnlyPropertyBag context)
    {
      double penWidth = GraphDocument.GetDefaultPenWidth(context);
      double majorTickLength = GraphDocument.GetDefaultMajorTickLength(context);
      var color = GraphDocument.GetDefaultForeColor(context);

      _axisPen = new PenX3D(color, penWidth);
      _majorTickPen = new PenX3D(color, penWidth);
      _minorTickPen = new PenX3D(color, penWidth);
      _majorTickLength = majorTickLength;
      _minorTickLength = majorTickLength / 2;

      if (showTicks)
      {
        _showFirstUpMajorTicks = preferredTickSide == CSAxisSide.FirstUp;
        _showFirstDownMajorTicks = preferredTickSide == CSAxisSide.FirstDown;
        _showSecondUpMajorTicks = preferredTickSide == CSAxisSide.SecondUp;
        _showSecondDownMajorTicks = preferredTickSide == CSAxisSide.SecondDown;

        _showFirstUpMinorTicks = preferredTickSide == CSAxisSide.FirstUp;
        _showFirstDownMinorTicks = preferredTickSide == CSAxisSide.FirstDown;
        _showSecondUpMinorTicks = preferredTickSide == CSAxisSide.SecondUp;
        _showSecondDownMinorTicks = preferredTickSide == CSAxisSide.SecondDown;
      }
    }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="from">The AxisStyle to copy from</param>
    public AxisLineStyle(AxisLineStyle from)
    {
      CopyFrom(from);
    }

    [MemberNotNull(nameof(_axisPen), nameof(_majorTickPen), nameof(_minorTickPen))]
    void CopyFrom(AxisLineStyle from)
      {
      using (var suspendToken = SuspendGetToken())
      {
        _axisPen = from._axisPen;
        _axisPosition1 = from._axisPosition1;
        _axisPosition2 = from._axisPosition2;

        _showFirstDownMajorTicks = from._showFirstDownMajorTicks;
        _showFirstDownMinorTicks = from._showFirstDownMinorTicks;
        _showFirstUpMajorTicks = from._showFirstUpMajorTicks;
        _showFirstUpMinorTicks = from._showFirstUpMinorTicks;

        _showSecondDownMajorTicks = from._showSecondDownMajorTicks;
        _showSecondDownMinorTicks = from._showSecondDownMinorTicks;
        _showSecondUpMajorTicks = from._showSecondUpMajorTicks;
        _showSecondUpMinorTicks = from._showSecondUpMinorTicks;

        _majorTickLength = from._majorTickLength;
        _majorTickPen = from._majorTickPen;
        _minorTickLength = from._minorTickLength;
        _minorTickPen = from._minorTickPen;

        _cachedAxisStyleInfo = from._cachedAxisStyleInfo;

        EhSelfChanged(EventArgs.Empty);

        suspendToken.Resume();
      }
    }

    /// <summary>
    /// Copy operation.
    /// </summary>
    /// <param name="obj">The AxisStyle to copy from</param>
    public bool CopyFrom(object obj)
    {
      if (ReferenceEquals(this, obj))
        return true;

      if (obj is AxisLineStyle from)
      {
        CopyFrom(from);
        return true;
      }

      return false;
    }

    protected override IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
    {
      yield break;
    }

    /// <summary>
    /// Creates a clone of this object.
    /// </summary>
    /// <returns>The cloned object.</returns>
    public object Clone()
    {
      return new AxisLineStyle(this);
    }

    public CSLineID? AxisStyleID
    {
      get
      {
        return _cachedAxisStyleInfo?.Identifier;
      }
    }

  
    public CSAxisInformation? CachedAxisInformation
    {
      get
      {
        return _cachedAxisStyleInfo;
      }
      set
      {
        _cachedAxisStyleInfo = value;
      }
    }

    /// <summary>
    /// Returns the used space from the middle line of the axis
    /// to the last outer object (either the outer major thicks or half
    /// of the axis thickness)
    /// </summary>
    /// <param name="side">The side of the axis at which the outer distance is returned.</param>
    public double GetOuterDistance(CSAxisSide side)
    {
      double retVal = 0;
      if (CSAxisSide.FirstUp == side)
      {
        retVal = _axisPen.Thickness1 / 2; // half of the axis thickness
        retVal = System.Math.Max(retVal, _showFirstUpMajorTicks ? _majorTickLength : 0);
        retVal = System.Math.Max(retVal, _showFirstUpMinorTicks ? _minorTickLength : 0);
      }
      else if (CSAxisSide.FirstDown == side)
      {
        retVal = _axisPen.Thickness1 / 2; // half of the axis thickness
        retVal = System.Math.Max(retVal, _showFirstDownMajorTicks ? _majorTickLength : 0);
        retVal = System.Math.Max(retVal, _showFirstDownMinorTicks ? _minorTickLength : 0);
      }
      else if (CSAxisSide.SecondUp == side)
      {
        retVal = _axisPen.Thickness2 / 2; // half of the axis thickness
        retVal = System.Math.Max(retVal, _showSecondUpMajorTicks ? _majorTickLength : 0);
        retVal = System.Math.Max(retVal, _showSecondUpMinorTicks ? _minorTickLength : 0);
      }
      else if (CSAxisSide.SecondDown == side)
      {
        retVal = _axisPen.Thickness2 / 2; // half of the axis thickness
        retVal = System.Math.Max(retVal, _showSecondDownMajorTicks ? _majorTickLength : 0);
        retVal = System.Math.Max(retVal, _showSecondDownMinorTicks ? _minorTickLength : 0);
      }
      else
      {
        retVal = 0;
      }
      return retVal;
    }

    public PenX3D AxisPen
    {
      get { return _axisPen; }
      set
      {
        if (value is null)
          throw new ArgumentNullException("value");

        var oldValue = _axisPen;
        _axisPen = value;

        if (!object.ReferenceEquals(oldValue, value))
        {
          EhSelfChanged(EventArgs.Empty);
        }
      }
    }

    public PenX3D MajorPen
    {
      get { return _majorTickPen; }
      set
      {
        if (value is null)
          throw new ArgumentNullException("value");

        var oldValue = _majorTickPen;
        _majorTickPen = value;

        if (!object.ReferenceEquals(oldValue, value))
        {
          EhSelfChanged(EventArgs.Empty);
        }
      }
    }

    public PenX3D MinorPen
    {
      get { return _minorTickPen; }
      set
      {
        if (value is null)
          throw new ArgumentNullException("value");

        var oldValue = _minorTickPen;
        _minorTickPen = value;

        if (!object.ReferenceEquals(oldValue, value))
        {
          EhSelfChanged(EventArgs.Empty);
        }
      }
    }

    /// <summary>Get/sets the major tick length.</summary>
    /// <value>The major tick length in point units (1/72 inch).</value>
    public double MajorTickLength
    {
      get { return _majorTickLength; }
      set
      {
        if (value != _majorTickLength)
        {
          _majorTickLength = value;
          EhSelfChanged(EventArgs.Empty);
        }
      }
    }

    /// <summary>Get/sets the minor tick length.</summary>
    /// <value>The minor tick length in point units (1/72 inch).</value>
    public double MinorTickLength
    {
      get { return _minorTickLength; }
      set
      {
        if (value != _minorTickLength)
        {
          _minorTickLength = value;
          EhSelfChanged(EventArgs.Empty);
        }
      }
    }

    /// <summary>Get/sets if outer major ticks are drawn.</summary>
    /// <value>True if outer major ticks are drawn.</value>
    public bool FirstUpMajorTicks
    {
      get { return _showFirstUpMajorTicks; }
      set
      {
        if (value != _showFirstUpMajorTicks)
        {
          _showFirstUpMajorTicks = value;
          EhSelfChanged(EventArgs.Empty);
        }
      }
    }

    /// <summary>Get/sets if inner major ticks are drawn.</summary>
    /// <value>True if inner major ticks are drawn.</value>
    public bool FirstDownMajorTicks
    {
      get { return _showFirstDownMajorTicks; }
      set
      {
        if (value != _showFirstDownMajorTicks)
        {
          _showFirstDownMajorTicks = value;
          EhSelfChanged(EventArgs.Empty);
        }
      }
    }

    /// <summary>Get/sets if outer major ticks are drawn.</summary>
    /// <value>True if outer major ticks are drawn.</value>
    public bool SecondUpMajorTicks
    {
      get { return _showSecondUpMajorTicks; }
      set
      {
        if (value != _showSecondUpMajorTicks)
        {
          _showSecondUpMajorTicks = value;
          EhSelfChanged(EventArgs.Empty);
        }
      }
    }

    /// <summary>Get/sets if inner major ticks are drawn.</summary>
    /// <value>True if inner major ticks are drawn.</value>
    public bool SecondDownMajorTicks
    {
      get { return _showSecondDownMajorTicks; }
      set
      {
        if (value != _showSecondDownMajorTicks)
        {
          _showSecondDownMajorTicks = value;
          EhSelfChanged(EventArgs.Empty);
        }
      }
    }

    /// <summary>Get/sets if outer minor ticks are drawn.</summary>
    /// <value>True if outer minor ticks are drawn.</value>
    public bool FirstUpMinorTicks
    {
      get { return _showFirstUpMinorTicks; }
      set
      {
        if (value != _showFirstUpMinorTicks)
        {
          _showFirstUpMinorTicks = value;
          EhSelfChanged(EventArgs.Empty);
        }
      }
    }

    /// <summary>Get/sets if inner minor ticks are drawn.</summary>
    /// <value>True if inner minor ticks are drawn.</value>
    public bool FirstDownMinorTicks
    {
      get { return _showFirstDownMinorTicks; }
      set
      {
        if (value != _showFirstDownMinorTicks)
        {
          _showFirstDownMinorTicks = value;
          EhSelfChanged(EventArgs.Empty);
        }
      }
    }

    /// <summary>Get/sets if outer minor ticks are drawn.</summary>
    /// <value>True if outer minor ticks are drawn.</value>
    public bool SecondUpMinorTicks
    {
      get { return _showSecondUpMinorTicks; }
      set
      {
        if (value != _showSecondUpMinorTicks)
        {
          _showSecondUpMinorTicks = value;
          EhSelfChanged(EventArgs.Empty);
        }
      }
    }

    /// <summary>Get/sets if inner minor ticks are drawn.</summary>
    /// <value>True if inner minor ticks are drawn.</value>
    public bool SecondDownMinorTicks
    {
      get { return _showSecondDownMinorTicks; }
      set
      {
        if (value != _showSecondDownMinorTicks)
        {
          _showSecondDownMinorTicks = value;
          EhSelfChanged(EventArgs.Empty);
        }
      }
    }

    /// <summary>
    /// Gets/sets the axis thickness.
    /// </summary>
    /// <value>Returns the thickness of the axis pen. On setting this value, it sets
    /// the thickness of the axis pen, the tickness of the major ticks pen, and the
    /// thickness of the minor ticks pen together.</value>
    public double Thickness1
    {
      get { return _axisPen.Thickness1; }
      set
      {
        _axisPen = _axisPen.WithThickness1(value);
        _majorTickPen = _majorTickPen.WithThickness1(value);
        _minorTickPen = _minorTickPen.WithThickness1(value);
        EhSelfChanged(EventArgs.Empty);
      }
    }

    /// <summary>
    /// Gets/sets the axis thickness.
    /// </summary>
    /// <value>Returns the thickness of the axis pen. On setting this value, it sets
    /// the thickness of the axis pen, the tickness of the major ticks pen, and the
    /// thickness of the minor ticks pen together.</value>
    public double Thickness2
    {
      get { return _axisPen.Thickness2; }
      set
      {
        _axisPen = _axisPen.WithThickness2(value);
        _majorTickPen = _majorTickPen.WithThickness2(value);
        _minorTickPen = _minorTickPen.WithThickness2(value);
        EhSelfChanged(EventArgs.Empty);
      }
    }

    /// <summary>
    /// Get/sets the axis color.
    /// </summary>
    /// <value>Returns the color of the axis pen. On setting this value, it sets
    /// the color of the axis pen along with the color of the major ticks pen and the
    /// color of the minor ticks pen together.</value>
    public NamedColor Color
    {
      get { return _axisPen.Color; }
      set
      {
        _axisPen = _axisPen.WithColor(value);
        _majorTickPen = _majorTickPen.WithColor(value);
        _minorTickPen = _minorTickPen.WithColor(value);
        EhSelfChanged(EventArgs.Empty);
      }
    }

    /// <summary>
    /// Get/set the axis shift position value.
    /// </summary>
    /// <value>Zero if the axis is not shifted (normal case). Else the shift value, either as
    /// absolute value in point units (1/72 inch), or relative to the corresponding layer dimension (i.e layer width for bottom axis).</value>
    public RADouble Position1
    {
      get { return _axisPosition1; }
      set
      {
        if (value != _axisPosition1)
        {
          _axisPosition1 = value;
          EhSelfChanged(EventArgs.Empty);
        }
      }
    }

    /// <summary>
    /// Get/set the axis shift position value.
    /// </summary>
    /// <value>Zero if the axis is not shifted (normal case). Else the shift value, either as
    /// absolute value in point units (1/72 inch), or relative to the corresponding layer dimension (i.e layer width for bottom axis).</value>
    public RADouble Position2
    {
      get { return _axisPosition2; }
      set
      {
        if (value != _axisPosition2)
        {
          _axisPosition2 = value;
          EhSelfChanged(EventArgs.Empty);
        }
      }
    }

    /// <summary>
    /// Paint the axis in the Graphics context.
    /// </summary>
    /// <param name="g">The graphics context painting to.</param>
    /// <param name="layer">The layer the axis belongs to.</param>
    /// <param name="styleInfo">The axis information of the axis to paint.</param>
    /// <param name="customTickSpacing">If not <c>null</c>, this parameter provides a custom tick spacing that is used instead of the default tick spacing of the scale.</param>
    public void Paint(IGraphicsContext3D g, IPlotArea layer, CSAxisInformation styleInfo, TickSpacing? customTickSpacing)
    {
      CSLineID styleID = styleInfo.Identifier;
      _cachedAxisStyleInfo = styleInfo;
      Scale axis = layer.Scales[styleID.ParallelAxisNumber];

      TickSpacing ticking = customTickSpacing is not null ? customTickSpacing : layer.Scales[styleID.ParallelAxisNumber].TickSpacing;

      Logical3D r0 = styleID.GetLogicalPoint(styleInfo.LogicalValueAxisOrg);
      Logical3D r1 = styleID.GetLogicalPoint(styleInfo.LogicalValueAxisEnd);

      var sweepPath = layer.CoordinateSystem.GetIsoline(r0, r1);
      g.DrawLine(_axisPen, sweepPath);

      _cachedMainLinePointsUsedForHitTesting = sweepPath.Points.ToArray();
      var majorTickLinesUsedForHitTesting = new List<LineD3D>();
      var minorTickLinesUsedForHitTesting = new List<LineD3D>();

      Logical3D outer;

      // now the major ticks
      VectorD3D outVector;
      double[] majorticks = ticking.GetMajorTicksNormal(axis);
      for (int i = 0; i < majorticks.Length; i++)
      {
        double r = majorticks[i];

        if (_showFirstUpMajorTicks)
        {
          outer = layer.CoordinateSystem.GetLogicalDirection(styleID.ParallelAxisNumber, CSAxisSide.FirstUp);
          var tickorg = layer.CoordinateSystem.GetPositionAndNormalizedDirection(r0, r1, r, outer, out outVector);
          var tickend = tickorg + outVector * _majorTickLength;
          g.DrawLine(_majorTickPen, tickorg, tickend);
          majorTickLinesUsedForHitTesting.Add(new LineD3D(tickorg, tickend));
        }
        if (_showFirstDownMajorTicks)
        {
          outer = layer.CoordinateSystem.GetLogicalDirection(styleID.ParallelAxisNumber, CSAxisSide.FirstDown);
          var tickorg = layer.CoordinateSystem.GetPositionAndNormalizedDirection(r0, r1, r, outer, out outVector);
          var tickend = tickorg + outVector * _majorTickLength;
          g.DrawLine(_majorTickPen, tickorg, tickend);
          majorTickLinesUsedForHitTesting.Add(new LineD3D(tickorg, tickend));
        }
        if (_showSecondUpMajorTicks)
        {
          outer = layer.CoordinateSystem.GetLogicalDirection(styleID.ParallelAxisNumber, CSAxisSide.SecondUp);
          var tickorg = layer.CoordinateSystem.GetPositionAndNormalizedDirection(r0, r1, r, outer, out outVector);
          var tickend = tickorg + outVector * _majorTickLength;
          g.DrawLine(_majorTickPen, tickorg, tickend);
          majorTickLinesUsedForHitTesting.Add(new LineD3D(tickorg, tickend));
        }
        if (_showSecondDownMajorTicks)
        {
          outer = layer.CoordinateSystem.GetLogicalDirection(styleID.ParallelAxisNumber, CSAxisSide.SecondDown);
          var tickorg = layer.CoordinateSystem.GetPositionAndNormalizedDirection(r0, r1, r, outer, out outVector);
          var tickend = tickorg + outVector * _majorTickLength;
          g.DrawLine(_majorTickPen, tickorg, tickend);
          majorTickLinesUsedForHitTesting.Add(new LineD3D(tickorg, tickend));
        }
      }

      // now the major ticks
      double[] minorticks = ticking.GetMinorTicksNormal(axis);
      for (int i = 0; i < minorticks.Length; i++)
      {
        double r = minorticks[i];

        if (_showFirstUpMinorTicks)
        {
          outer = layer.CoordinateSystem.GetLogicalDirection(styleID.ParallelAxisNumber, CSAxisSide.FirstUp);
          var tickorg = layer.CoordinateSystem.GetPositionAndNormalizedDirection(r0, r1, r, outer, out outVector);
          var tickend = tickorg + outVector * _minorTickLength;
          g.DrawLine(_minorTickPen, tickorg, tickend);
          minorTickLinesUsedForHitTesting.Add(new LineD3D(tickorg, tickend));
        }
        if (_showFirstDownMinorTicks)
        {
          outer = layer.CoordinateSystem.GetLogicalDirection(styleID.ParallelAxisNumber, CSAxisSide.FirstDown);
          var tickorg = layer.CoordinateSystem.GetPositionAndNormalizedDirection(r0, r1, r, outer, out outVector);
          var tickend = tickorg + outVector * _minorTickLength;
          g.DrawLine(_minorTickPen, tickorg, tickend);
          minorTickLinesUsedForHitTesting.Add(new LineD3D(tickorg, tickend));
        }

        if (_showSecondUpMinorTicks)
        {
          outer = layer.CoordinateSystem.GetLogicalDirection(styleID.ParallelAxisNumber, CSAxisSide.SecondUp);
          var tickorg = layer.CoordinateSystem.GetPositionAndNormalizedDirection(r0, r1, r, outer, out outVector);
          var tickend = tickorg + outVector * _minorTickLength;
          g.DrawLine(_minorTickPen, tickorg, tickend);
          minorTickLinesUsedForHitTesting.Add(new LineD3D(tickorg, tickend));
        }
        if (_showSecondDownMinorTicks)
        {
          outer = layer.CoordinateSystem.GetLogicalDirection(styleID.ParallelAxisNumber, CSAxisSide.SecondDown);
          var tickorg = layer.CoordinateSystem.GetPositionAndNormalizedDirection(r0, r1, r, outer, out outVector);
          var tickend = tickorg + outVector * _minorTickLength;
          g.DrawLine(_minorTickPen, tickorg, tickend);
          minorTickLinesUsedForHitTesting.Add(new LineD3D(tickorg, tickend));
        }
      }

      _cachedMajorTickLinesUsedForHitTesting = majorTickLinesUsedForHitTesting.ToArray();
      _cachedMinorTickLinesUsedForHitTesting = minorTickLinesUsedForHitTesting.ToArray();
    }

    public IHitTestObject? HitTest(HitTestPointData hitData, bool testTickLines)
    {
      if (!testTickLines)
      {
        var mainAxisPoints = _cachedMainLinePointsUsedForHitTesting;
        if (mainAxisPoints is not null)
        {
          if (hitData.IsHit(mainAxisPoints, _axisPen.Thickness1, _axisPen.Thickness2))
            return new HitTestObject(
              new PolylineObjectOutline(_axisPen.Thickness1, _axisPen.Thickness2, mainAxisPoints, hitData.WorldTransformation),
              this,
              hitData.WorldTransformation);
        }
      }
      else // Test Tick lines
      {
        // test major ticks for hit
        if (_cachedMajorTickLinesUsedForHitTesting is not null)
        {
          foreach (var line in _cachedMajorTickLinesUsedForHitTesting)
          {
            if (hitData.IsHit(line, _majorTickPen.Thickness1, _majorTickPen.Thickness2))
              return new HitTestObject(
                new MultipleSingleLinesObjectOutline(_majorTickPen.Thickness1, _majorTickPen.Thickness2, _cachedMajorTickLinesUsedForHitTesting, hitData.WorldTransformation),
                this,
                hitData.WorldTransformation);
          }
        }
        // test minor ticks for hit
        if (_cachedMinorTickLinesUsedForHitTesting is not null)
        {
          foreach (var line in _cachedMinorTickLinesUsedForHitTesting)
          {
            if (hitData.IsHit(line, _minorTickPen.Thickness1, _majorTickPen.Thickness2))
              return new HitTestObject(
                new MultipleSingleLinesObjectOutline(_minorTickPen.Thickness1, _minorTickPen.Thickness2, _cachedMinorTickLinesUsedForHitTesting, hitData.WorldTransformation),
                this,
                hitData.WorldTransformation);
          }
        }
      }

      return null;
    }

    protected virtual void OnPenChangedEventHandler(object sender, EventArgs e)
    {
      EhSelfChanged(EventArgs.Empty);
    }

    #region IRoutedPropertyReceiver Members

    public IEnumerable<(string PropertyName, object PropertyValue, Action<object> PropertySetter)> GetRoutedProperties(string propertyName)
    {
      switch (propertyName)
      {
        case "StrokeWidth":
          yield return (propertyName, _axisPen.Thickness1, (w) => _axisPen = _axisPen.WithThickness1((double)w));
          yield return (propertyName, _axisPen.Thickness2, (w) => _axisPen = _axisPen.WithThickness2((double)w));
          yield return (propertyName, _axisPen.Thickness1, (w) => _majorTickPen = _majorTickPen.WithThickness1((double)w));
          yield return (propertyName, _axisPen.Thickness2, (w) => _majorTickPen = _majorTickPen.WithThickness2((double)w));
          yield return (propertyName, _axisPen.Thickness1, (w) => _minorTickPen = _minorTickPen.WithThickness1((double)w));
          yield return (propertyName, _axisPen.Thickness2, (w) => _minorTickPen = _minorTickPen.WithThickness2((double)w));
          break;
      }

      yield break;
    }

    #endregion IRoutedPropertyReceiver Members

    /// <summary>
    /// Try to change the showing variables (e.g. <see cref="_showFirstDownMajorTicks"/>) when changing coordinate system, so that
    /// it is tried to keep the positions of the shown major and minor ticks.
    /// </summary>
    /// <param name="GetNewAxisSideFromOldAxisSide">The get new axis side from old axis side.</param>
    public void ChangeTickPositionsWhenChangingCoordinateSystem(Func<CSAxisSide, CSAxisSide?> GetNewAxisSideFromOldAxisSide)
    {
      // store old _show variables into temporary storage
      var oldShown = new bool[]
      {
        _showFirstUpMajorTicks,
      _showFirstDownMajorTicks,
      _showSecondUpMajorTicks,
      _showSecondDownMajorTicks,
      _showFirstUpMinorTicks,
      _showFirstDownMinorTicks,
      _showSecondUpMinorTicks,
      _showSecondDownMinorTicks
      };

      var sides = new CSAxisSide[] {
        CSAxisSide.FirstUp,
        CSAxisSide.FirstDown,
        CSAxisSide.SecondUp,
        CSAxisSide.SecondDown,
      };
      var sideDict = new Dictionary<CSAxisSide, int>();
      for (int i = 0; i < sides.Length; ++i)
        sideDict.Add(sides[i], i);

      var newShown = (bool[])oldShown.Clone();

      // Calculate new show variables

      // Major labels first
      for (int i = 0; i < 4; ++i)
      {
        var newAxisSide = GetNewAxisSideFromOldAxisSide(sides[i]);
        if (newAxisSide.HasValue)
        {
          int newIndex = sideDict[newAxisSide.Value];
          newShown[newIndex] = oldShown[i];
        }
      }
      // then minor labels
      for (int i = 0; i < 4; ++i)
      {
        var newAxisSide = GetNewAxisSideFromOldAxisSide(sides[i]);
        if (newAxisSide.HasValue)
        {
          int newIndex = 4 + sideDict[newAxisSide.Value];
          newShown[newIndex] = oldShown[4 + i];
        }
      }

      // bring _show variables from array back to members
      _showFirstUpMajorTicks = newShown[0];
      _showFirstDownMajorTicks = newShown[1];
      _showSecondUpMajorTicks = newShown[2];
      _showSecondDownMajorTicks = newShown[3];
      _showFirstUpMinorTicks = newShown[4];
      _showFirstDownMinorTicks = newShown[5];
      _showSecondUpMinorTicks = newShown[6];
      _showSecondDownMinorTicks = newShown[7];
    }
  }
}
