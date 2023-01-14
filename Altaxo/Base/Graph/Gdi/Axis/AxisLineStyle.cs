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
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Drawing2D;
using Altaxo.Drawing;
using Altaxo.Geometry;
using Altaxo.Graph.Scales;
using Altaxo.Graph.Scales.Ticks;

namespace Altaxo.Graph.Gdi.Axis
{
  /// <summary>
  /// XYAxisStyle is responsible for painting the axes on rectangular two dimensional layers.
  /// </summary>
  public class AxisLineStyle
    :
    Main.SuspendableDocumentNodeWithSetOfEventArgs,
    IRoutedPropertyReceiver,
    Main.ICopyFrom
  {
    /// <summary>Pen used for painting of the axis.</summary>
    protected PenX _axisPen;

    /// <summary>Pen used for painting of the major ticks.</summary>
    protected PenX _majorTickPen;

    /// <summary>Pen used for painting of the minor ticks.</summary>
    protected PenX _minorTickPen;

    /// <summary>Length of the major ticks in points (1/72 inch).</summary>
    protected double _majorTickLength;

    /// <summary>Length of the minor ticks in points (1/72 inch).</summary>
    protected double _minorTickLength;

    /// <summary>True if major ticks should be painted outside of the layer.</summary>
    protected bool _showFirstUpMajorTicks;

    /// <summary>True if major ticks should be painted inside of the layer.</summary>
    protected bool _showFirstDownMajorTicks;

    /// <summary>True if minor ticks should be painted outside of the layer.</summary>
    protected bool _showFirstUpMinorTicks;

    /// <summary>True if major ticks should be painted inside of the layer.</summary>
    protected bool _showFirstDownMinorTicks;

    /// <summary>Axis shift position, either provide as absolute values in point units, or as relative value relative to the layer size.</summary>
    protected RADouble _axisPosition; // if relative, then relative to layer size, if absolute then in points

    protected CSAxisInformation? _cachedAxisStyleInfo;

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.XYAxisStyle", 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        throw new NotSupportedException("Serialization of old versions not supported - probably a programming error");
        /*
                XYAxisStyle s = (XYAxisStyle)obj;
                info.AddValue("Edge",s.m_Edge);
                info.AddValue("AxisPen",s.m_AxisPen);
                info.AddValue("MajorPen",s.m_MajorTickPen);
                info.AddValue("MinorPen",s.m_MinorTickPen);
                info.AddValue("MajorLength",s.m_MajorTickLength);
                info.AddValue("MinorLength",s.m_MinorTickLength);
                info.AddValue("MajorOuter",s.m_bOuterMajorTicks);
                info.AddValue("MajorInner",s.m_bInnerMajorTicks);
                info.AddValue("MinorOuter",s.m_bOuterMinorTicks);
                info.AddValue("MinorInner",s.m_bInnerMinorTicks);
                info.AddValue("AxisPosition",s.m_AxisPosition);
                */
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (AxisLineStyle?)o ?? new AxisLineStyle(info);

        var edge = (Edge)info.GetValue("Edge", s);
        s._axisPen = (PenX)info.GetValue("AxisPen", s);
        s._majorTickPen = (PenX)info.GetValue("MajorPen", s);
        s._minorTickPen = (PenX)info.GetValue("MinorPen", s);

        s._majorTickLength = info.GetDouble("MajorLength");
        s._minorTickLength = info.GetDouble("MinorLength");
        bool bOuterMajorTicks = info.GetBoolean("MajorOuter");
        bool bInnerMajorTicks = info.GetBoolean("MajorInner");
        bool bOuterMinorTicks = info.GetBoolean("MinorOuter");
        bool bInnerMinorTicks = info.GetBoolean("MinorInner");
        s._axisPosition = (RADouble)info.GetValue("AxisPosition", s);

        if (edge.TypeOfEdge == EdgeType.Top || edge.TypeOfEdge == EdgeType.Right)
        {
          s._showFirstUpMajorTicks = bOuterMajorTicks;
          s._showFirstDownMajorTicks = bInnerMajorTicks;
          s._showFirstUpMinorTicks = bOuterMinorTicks;
          s._showFirstDownMinorTicks = bInnerMinorTicks;
        }
        else
        {
          s._showFirstUpMajorTicks = bInnerMajorTicks;
          s._showFirstDownMajorTicks = bOuterMajorTicks;
          s._showFirstUpMinorTicks = bInnerMinorTicks;
          s._showFirstDownMinorTicks = bOuterMinorTicks;
        }

        return s;
      }
    }

    // 2006-09-08 Renaming XYAxisStyle in G2DAxisLineStyle
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(AxisLineStyle), 1)]
    private class XmlSerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (AxisLineStyle)obj;
        info.AddValue("AxisPen", s._axisPen);
        info.AddValue("MajorPen", s._majorTickPen);
        info.AddValue("MinorPen", s._minorTickPen);
        info.AddValue("MajorLength", s._majorTickLength);
        info.AddValue("MinorLength", s._minorTickLength);
        info.AddValue("AxisPosition", s._axisPosition);
        info.AddValue("Major1Up", s._showFirstUpMajorTicks);
        info.AddValue("Major1Dw", s._showFirstDownMajorTicks);
        info.AddValue("Minor1Up", s._showFirstUpMinorTicks);
        info.AddValue("Minor1Dw", s._showFirstDownMinorTicks);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (AxisLineStyle?)o ?? new AxisLineStyle(info);

        s._axisPen = (PenX)info.GetValue("AxisPen", s);
        s._majorTickPen = (PenX)info.GetValue("MajorPen", s);
        s._minorTickPen = (PenX)info.GetValue("MinorPen", s);

        s._majorTickLength = info.GetDouble("MajorLength");
        s._minorTickLength = info.GetDouble("MinorLength");
        s._axisPosition = (RADouble)info.GetValue("AxisPosition", s);
        s._showFirstUpMajorTicks = info.GetBoolean("Major1Up");
        s._showFirstDownMajorTicks = info.GetBoolean("Major1Dw");
        s._showFirstUpMinorTicks = info.GetBoolean("Minor1Up");
        s._showFirstDownMinorTicks = info.GetBoolean("Minor1Dw");

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
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
    {
    }

    /// <summary>
    /// Creates a default axis style.
    /// </summary>
    public AxisLineStyle(Main.Properties.IReadOnlyPropertyBag context)
    {
      double penWidth = GraphDocument.GetDefaultPenWidth(context);
      double majorTickLength = GraphDocument.GetDefaultMajorTickLength(context);
      var color = GraphDocument.GetDefaultForeColor(context);

      _axisPen = new PenX(color, penWidth);
      _majorTickPen = new PenX(color, penWidth);
      _minorTickPen = new PenX(color, penWidth);
      _majorTickLength = majorTickLength;
      _minorTickLength = majorTickLength / 2;
      _showFirstUpMajorTicks = true; // true if right major ticks should be visible
      _showFirstDownMajorTicks = true; // true if left major ticks should be visible
      _showFirstUpMinorTicks = true; // true if right minor ticks should be visible
      _showFirstDownMinorTicks = true; // true if left minor ticks should be visible
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
    private void CopyFrom(AxisLineStyle from)
    {

      using (var suspendToken = SuspendGetToken())
      {
        _axisPen = from._axisPen;
        _axisPosition = from._axisPosition;
        _showFirstDownMajorTicks = from._showFirstDownMajorTicks;
        _showFirstDownMinorTicks = from._showFirstDownMinorTicks;
        _showFirstUpMajorTicks = from._showFirstUpMajorTicks;
        _showFirstUpMinorTicks = from._showFirstUpMinorTicks;
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

    public virtual IHitTestObject? HitTest(IPlotArea layer, PointD2D pt, bool withTicks)
    {
      GraphicsPath selectionPath = GetSelectionPath(layer, withTicks);
      return selectionPath.IsVisible(pt.ToGdi()) ? new HitTestObject(GetObjectPath(layer, withTicks), this) : null;
    }

    public virtual IHitTestObject? HitTest(IPlotArea layer, HitTestRectangularData hitData, bool withTicks)
    {
      GraphicsPath selectionPath = GetSelectionPath(layer, withTicks);
      return hitData.IsCovering(selectionPath.PathPoints) ? new HitTestObject(GetObjectPath(layer, withTicks), this) : null;
    }

    /// <summary>
    /// Returns the used space from the middle line of the axis
    /// to the last outer object (either the outer major thicks or half
    /// of the axis thickness)
    /// </summary>
    /// <param name="side">The side of the axis at which the outer distance is returned.</param>
    public double GetOuterDistance(CSAxisSide side)
    {
      double retVal = _axisPen.Width / 2; // half of the axis thickness
      if (CSAxisSide.FirstUp == side)
      {
        retVal = System.Math.Max(retVal, _showFirstUpMajorTicks ? _majorTickLength : 0);
        retVal = System.Math.Max(retVal, _showFirstUpMinorTicks ? _minorTickLength : 0);
      }
      else if (CSAxisSide.FirstDown == side)
      {
        retVal = System.Math.Max(retVal, _showFirstDownMajorTicks ? _majorTickLength : 0);
        retVal = System.Math.Max(retVal, _showFirstDownMinorTicks ? _minorTickLength : 0);
      }
      else
      {
        retVal = 0;
      }
      return retVal;
    }

    /// <summary>
    /// GetOffset returns the distance of the axis to the layer edge in points
    /// in most cases, the axis position is exactly onto the layer edge and offset is zero,
    /// if the axis is outside the layer, offset is a positive value,
    /// if the axis is shifted inside the layer, offset is negative
    /// </summary>
    public float GetOffset(SizeF layerSize)
    {
      throw new NotImplementedException("Old stuff");
      //return (float)m_AxisPosition.GetValueRelativeTo(m_Edge.GetOppositeEdgeLength(layerSize));
    }

    public PenX AxisPen
    {
      get { return _axisPen; }
      set
      {
        if (value is null)
          throw new ArgumentNullException(nameof(AxisPen));

        if (!(_axisPen == value))
        {
          _axisPen = value;
          EhSelfChanged(EventArgs.Empty);
        }
      }
    }

    public PenX MajorPen
    {
      get { return _majorTickPen; }
      set
      {
        if (value is null)
          throw new ArgumentNullException(nameof(MajorPen));
        if (!(_majorTickPen == value))
        {
          _majorTickPen = value;
          EhSelfChanged(EventArgs.Empty);
        }
      }
    }

    public PenX MinorPen
    {
      get { return _minorTickPen; }
      set
      {
        if (value is null)
          throw new ArgumentNullException(nameof(MinorPen));

        if (!(_minorTickPen == value))
        {
          _minorTickPen = value;
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

    /// <summary>
    /// Gets/sets the axis thickness.
    /// </summary>
    /// <value>Returns the thickness of the axis pen. On setting this value, it sets
    /// the thickness of the axis pen, the tickness of the major ticks pen, and the
    /// thickness of the minor ticks pen together.</value>
    public double Thickness
    {
      get { return _axisPen.Width; }
      set
      {
        if (_axisPen.Width != value || _majorTickPen.Width != value || _minorTickPen.Width != value)
        {
          _axisPen = _axisPen.WithWidth(value);
          _majorTickPen = _majorTickPen.WithWidth(value);
          _minorTickPen = _minorTickPen.WithWidth(value);
          EhSelfChanged(EventArgs.Empty);
        }
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
    public RADouble Position
    {
      get { return _axisPosition; }
      set
      {
        if (value != _axisPosition)
        {
          _axisPosition = value;
          EhSelfChanged(EventArgs.Empty);
        }
      }
    }

    /// <summary>
    /// Gives the path which encloses the axis line only.
    /// </summary>
    /// <param name="layer"></param>
    /// <param name="withTicks">If true, the selection path is not only drawn around the axis, but around the axis and the ticks.</param>
    /// <returns>The graphics path of the axis line.</returns>
    public virtual GraphicsPath GetObjectPath(IPlotArea layer, bool withTicks)
    {
      return GetPath(layer, withTicks, 0);
    }

    /// <summary>
    /// Gives the path where the hit test is successfull.
    /// </summary>
    /// <param name="layer"></param>
    /// <param name="withTicks">If true, the selection path is not only drawn around the axis, but around the axis and the ticks.</param>
    /// <returns>The graphics path of the selection rectangle.</returns>
    public virtual GraphicsPath GetSelectionPath(IPlotArea layer, bool withTicks)
    {
      return GetPath(layer, withTicks, 3);
    }

    /// <summary>
    /// Gives the path where the hit test is successfull.
    /// </summary>
    /// <param name="layer"></param>
    /// <param name="withTicks">If true, the selection path is not only drawn around the axis, but around the axis and the ticks.</param>
    /// <param name="inflateby">Value in points, that the calculated path is inflated.</param>
    /// <returns>The graphics path of the selection rectangle.</returns>
    protected GraphicsPath GetPath(IPlotArea layer, bool withTicks, double inflateby)
    {
      if (_cachedAxisStyleInfo is null)
        throw new InvalidProgramException($"{nameof(_cachedAxisStyleInfo)} is null!");

      Logical3D r0 = _cachedAxisStyleInfo.Identifier.GetLogicalPoint(_cachedAxisStyleInfo.LogicalValueAxisOrg);
      Logical3D r1 = _cachedAxisStyleInfo.Identifier.GetLogicalPoint(_cachedAxisStyleInfo.LogicalValueAxisEnd);
      var gp = new GraphicsPath();
      layer.CoordinateSystem.GetIsoline(gp, r0, r1);

      if (withTicks)
      {
        if (_showFirstDownMajorTicks || _showFirstUpMajorTicks)
          inflateby = Math.Max(inflateby, _majorTickLength);
        if (_showFirstDownMinorTicks || _showFirstUpMinorTicks)
          inflateby = Math.Max(inflateby, _minorTickLength);
      }

      var widenPen = new Pen(System.Drawing.Color.Black, (float)(2 * inflateby));

      gp.Widen(widenPen);

      return gp;
    }

    /// <summary>
    /// Paint the axis in the Graphics context.
    /// </summary>
    /// <param name="g">The graphics context painting to.</param>
    /// <param name="layer">The layer the axis belongs to.</param>
    /// <param name="styleInfo">The axis information of the axis to paint.</param>
    /// <param name="customTickSpacing">If not <c>null</c>, this parameter provides a custom tick spacing that is used instead of the default tick spacing of the scale.</param>
    public void Paint(Graphics g, IPlotArea layer, CSAxisInformation styleInfo, TickSpacing? customTickSpacing)
    {
      CSLineID styleID = styleInfo.Identifier;
      _cachedAxisStyleInfo = styleInfo;
      Scale axis = layer.Scales[styleID.ParallelAxisNumber];

      TickSpacing ticking = customTickSpacing is not null ? customTickSpacing : layer.Scales[styleID.ParallelAxisNumber].TickSpacing;

      Logical3D r0 = styleID.GetLogicalPoint(styleInfo.LogicalValueAxisOrg);
      Logical3D r1 = styleID.GetLogicalPoint(styleInfo.LogicalValueAxisEnd);

      using (var axisPenGdi = PenCacheGdi.Instance.BorrowPen(_axisPen))
      {
        layer.CoordinateSystem.DrawIsoline(g, axisPenGdi, r0, r1);
      }


      using var majorTickPenGdi = PenCacheGdi.Instance.BorrowPen(_majorTickPen);
      using var minorTickPenGdi = PenCacheGdi.Instance.BorrowPen(_minorTickPen);
      Logical3D outer;

      // now the major ticks
      PointD2D outVector;
      double[] majorticks = ticking.GetMajorTicksNormal(axis);
      for (int i = 0; i < majorticks.Length; i++)
      {
        double r = majorticks[i];

        if (_showFirstUpMajorTicks)
        {
          outer = layer.CoordinateSystem.GetLogicalDirection(styleID.ParallelAxisNumber, CSAxisSide.FirstUp);
          var tickorg = layer.CoordinateSystem.GetNormalizedDirection(r0, r1, r, outer, out outVector);
          var tickend = tickorg + outVector * _majorTickLength;
          g.DrawLine(majorTickPenGdi, tickorg.ToGdi(), tickend.ToGdi());
        }
        if (_showFirstDownMajorTicks)
        {
          outer = layer.CoordinateSystem.GetLogicalDirection(styleID.ParallelAxisNumber, CSAxisSide.FirstDown);
          var tickorg = layer.CoordinateSystem.GetNormalizedDirection(r0, r1, r, outer, out outVector);
          var tickend = tickorg + outVector * _majorTickLength;
          g.DrawLine(majorTickPenGdi, tickorg.ToGdi(), tickend.ToGdi());
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
          var tickorg = layer.CoordinateSystem.GetNormalizedDirection(r0, r1, r, outer, out outVector);
          var tickend = tickorg + outVector * _minorTickLength;
          g.DrawLine(minorTickPenGdi, tickorg.ToGdi(), tickend.ToGdi());
        }
        if (_showFirstDownMinorTicks)
        {
          outer = layer.CoordinateSystem.GetLogicalDirection(styleID.ParallelAxisNumber, CSAxisSide.FirstDown);
          var tickorg = layer.CoordinateSystem.GetNormalizedDirection(r0, r1, r, outer, out outVector);
          var tickend = tickorg + outVector * _minorTickLength;
          g.DrawLine(minorTickPenGdi, tickorg.ToGdi(), tickend.ToGdi());
        }
      }
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
          yield return (propertyName, _axisPen.Width, (value) => _axisPen = _axisPen.WithWidth((double)value));
          yield return (propertyName, _majorTickPen.Width, (value) => _majorTickPen = _majorTickPen.WithWidth((double)value));
          yield return (propertyName, _minorTickPen.Width, (value) => _minorTickPen = _minorTickPen.WithWidth((double)value));
          break;

        case "MajorTickLength":
          yield return (propertyName, _majorTickLength, (value) => { MajorTickLength = (double)value; MinorTickLength = 0.5 * (double)value; }
          );
          break;
      }

      yield break;
    }

    #endregion IRoutedPropertyReceiver Members
  }
}
