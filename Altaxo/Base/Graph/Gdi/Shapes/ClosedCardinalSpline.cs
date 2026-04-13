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
using Altaxo.Geometry;
using Altaxo.Main;

namespace Altaxo.Graph.Gdi.Shapes
{
  /// <summary>
  /// Represents a closed cardinal spline shape.
  /// </summary>
  [Serializable]
  public class ClosedCardinalSpline : ClosedPathShapeBase
  {
    private static double _defaultTension = 0.5;

    private List<PointD2D> _curvePoints = new List<PointD2D>();
    private double _tension = _defaultTension;

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ClosedCardinalSpline), 0)]
    private class XmlSerializationSurrogate2 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc />
      public void Serialize(object o, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (ClosedCardinalSpline)o;
        info.AddBaseValueEmbedded(s, typeof(ClosedCardinalSpline).BaseType!);
        info.AddValue("Tension", s._tension);
        info.CreateArray("Points", s._curvePoints.Count);
        for (int i = 0; i < s._curvePoints.Count; i++)
          info.AddValue("e", s._curvePoints[i]);
        info.CommitArray();
      }

      /// <inheritdoc />
      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (ClosedCardinalSpline?)o ?? new ClosedCardinalSpline(info);
        info.GetBaseValueEmbedded(s, typeof(ClosedCardinalSpline).BaseType!, parent);
        s._tension = info.GetDouble("Tension");
        s._curvePoints.Clear();
        int count = info.OpenArray("Points");
        for (int i = 0; i < count; i++)
          s._curvePoints.Add((PointD2D)info.GetValue("e", s));
        info.CloseArray(count);
        return s;
      }
    }

    #endregion Serialization

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="ClosedCardinalSpline"/> class during deserialization.
    /// </summary>
    /// <param name="info">The deserialization info.</param>
    protected ClosedCardinalSpline(Altaxo.Serialization.Xml.IXmlDeserializationInfo info)
      : base(new ItemLocationDirectAutoSize(), info)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ClosedCardinalSpline"/> class.
    /// </summary>
    /// <param name="context">The property context.</param>
    public ClosedCardinalSpline(Altaxo.Main.Properties.IReadOnlyPropertyBag context)
      : base(new ItemLocationDirectAutoSize(), context)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ClosedCardinalSpline"/> class.
    /// </summary>
    /// <param name="points">The curve points.</param>
    /// <param name="context">The property context.</param>
    public ClosedCardinalSpline(IEnumerable<PointD2D> points, Altaxo.Main.Properties.IReadOnlyPropertyBag context)
      : this(points, DefaultTension, context)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ClosedCardinalSpline"/> class.
    /// </summary>
    /// <param name="points">The curve points.</param>
    /// <param name="tension">The spline tension.</param>
    /// <param name="context">The property context.</param>
    public ClosedCardinalSpline(IEnumerable<PointD2D> points, double tension, Altaxo.Main.Properties.IReadOnlyPropertyBag context)
      : base(new ItemLocationDirectAutoSize(), context)
    {
      _curvePoints.AddRange(points);
      _tension = Math.Abs(tension);

      if (!(_curvePoints.Count > 2))
        throw new ArgumentException("Number of curve points has to be > 2");

      CalculateAndSetBounds();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ClosedCardinalSpline"/> class by copying another instance.
    /// </summary>
    /// <param name="from">The instance to copy from.</param>
    public ClosedCardinalSpline(ClosedCardinalSpline from)
      : base(from)  // all is done here, since CopyFrom is virtual!
    {
      CopyFrom(from, false);
    }

    /// <summary>
    /// Copies the state from another <see cref="ClosedCardinalSpline"/> instance.
    /// </summary>
    /// <param name="from">The source instance.</param>
    /// <param name="withBaseMembers">If set to <see langword="true"/>, base members are copied as well.</param>
    protected void CopyFrom(ClosedCardinalSpline from, bool withBaseMembers)
    {
      if (withBaseMembers)
        base.CopyFrom(from, withBaseMembers);

      _tension = from._tension;
      _curvePoints.Clear();
      _curvePoints.AddRange(from._curvePoints);
    }

    /// <inheritdoc />
    public override bool CopyFrom(object obj)
    {
      if (ReferenceEquals(this, obj))
        return true;
      if (obj is ClosedCardinalSpline from)
      {
        using (var suspendToken = SuspendGetToken())
        {
          CopyFrom(from, true);
          EhSelfChanged(EventArgs.Empty);
        }
        return true;
      }
      else
      {
        return base.CopyFrom(obj);
      }
    }

    #endregion Constructors

    /// <summary>
    /// Gets the default spline tension.
    /// </summary>
    public static double DefaultTension { get { return _defaultTension; } }

    /// <summary>
    /// Gets or sets the spline tension.
    /// </summary>
    public double Tension
    {
      get
      {
        return _tension;
      }
      set
      {
        var oldValue = _tension;
        value = Math.Max(value, 0);
        value = Math.Min(value, float.MaxValue);
        _tension = value;
        if (value != oldValue)
          EhSelfChanged(EventArgs.Empty);
      }
    }

    /// <summary>Gets a copied list (!) of the curve points. If set, the list is also copied to an internally kept list.</summary>
    public List<PointD2D> CurvePoints
    {
      get
      {
        return new List<PointD2D>(_curvePoints);
      }
      set
      {
        _curvePoints.Clear();
        _curvePoints.AddRange(value);
        CalculateAndSetBounds();
      }
    }

    /// <inheritdoc />
    public override bool AllowNegativeSize
    {
      get
      {
        return true;
      }
    }

    /// <inheritdoc />
    public override object Clone()
    {
      return new ClosedCardinalSpline(this);
    }

    /// <summary>
    /// Normally sets the size of the item. For the ClosedCardinalSpline, the size is calculated internally. Thus, the function is overriden in order to ignore both parameters.
    /// </summary>
    /// <param name="width">Unscaled width of the item (ignored here).</param>
    /// <param name="height">Unscaled height of the item (ignored here).</param>
    /// <param name="eventFiring">Suppressed the change event (ignored here).</param>
    protected override void SetSize(double width, double height, Main.EventFiring eventFiring)
    {
    }

    /// <inheritdoc />
    public override bool AutoSize
    {
      get
      {
        return true;
      }
    }

    private void CalculateAndSetBounds()
    {
      var path = InternalGetPath(PointD2D.Empty);
      var bounds = path.GetBounds();
      for (int i = 0; i < _curvePoints.Count; i++)
        _curvePoints[i] -= bounds.Location.ToPointD2D();

      using (var token = SuspendGetToken())
      {
        ShiftPosition(bounds.Location.ToPointD2D());
        ((ItemLocationDirectAutoSize)_location).SetSizeInAutoSizeMode(bounds.Size.ToPointD2D());

        token.Resume();
      }
    }

    /// <summary>
    /// Sets one control point of the spline.
    /// </summary>
    /// <param name="idx">The point index.</param>
    /// <param name="value">The new point value.</param>
    public void SetPoint(int idx, PointD2D value)
    {
      if (!(_curvePoints[idx] == value))
      {
        _curvePoints[idx] = value;
        EhSelfChanged(EventArgs.Empty);
      }
    }

    /// <summary>
    /// Gets the selection path.
    /// </summary>
    /// <returns>The selection path.</returns>
    public GraphicsPath GetSelectionPath()
    {
      return GetPath();
    }

    /// <summary>
    /// Gets the path of the object in object world coordinates.
    /// </summary>
    /// <returns></returns>
    public override GraphicsPath GetObjectOutlineForArrangements()
    {
      return GetPath();
    }

    private GraphicsPath InternalGetPath(PointD2D offset)
    {
      var gp = new GraphicsPath();

      var pt = new PointF[_curvePoints.Count];
      for (int i = 0; i < _curvePoints.Count; i++)
        pt[i] = new PointF((float)(_curvePoints[i].X + offset.X), (float)(_curvePoints[i].Y + offset.Y));
      gp.AddClosedCurve(pt, (float)_tension);
      return gp;
    }

    /// <summary>
    /// Gets the path of the object in object world coordinates.
    /// </summary>
    /// <returns></returns>
    protected GraphicsPath GetPath()
    {
      return InternalGetPath(_location.AbsoluteVectorPivotToLeftUpper);
    }

    /// <inheritdoc />
    public override IHitTestObject? HitTest(HitTestPointData htd)
    {
      HitTestObjectBase? result = null;
      GraphicsPath gp = GetPath();

      using var linePenGdi = PenCacheGdi.Instance.BorrowPen(_linePen);
      if (_fillBrush.IsVisible && gp.IsVisible(htd.GetHittedPointInWorldCoord(_transformation).ToGdi()))
      {
        result = new ClosedCardinalSplineHitTestObject(this);
      }
      else if (_linePen.IsVisible && gp.IsOutlineVisible(htd.GetHittedPointInWorldCoord(_transformation).ToGdi(), linePenGdi))
      {
        result = new ClosedCardinalSplineHitTestObject(this);
      }
      else
      {
        gp.Transform(htd.GetTransformation(_transformation).ToGdi()); // Transform to page coord
        if (gp.IsOutlineVisible(htd.HittedPointInPageCoord.ToGdi(), new Pen(Color.Black, 6)))
        {
          result = new ClosedCardinalSplineHitTestObject(this);
        }
      }

      if (result is not null)
        result.DoubleClick = EhHitDoubleClick;

      return result;
    }

    private static new bool EhHitDoubleClick(IHitTestObject o)
    {
      object hitted = o.HittedObject;
      Current.Gui.ShowDialog(ref hitted, "Line properties", true);
      ((ClosedCardinalSpline)hitted).EhSelfChanged(EventArgs.Empty);
      return true;
    }

    /// <inheritdoc />
    public override void Paint(Graphics g, IPaintContext context)
    {
      GraphicsState gs = g.Save();
      TransformGraphics(g);

      var bounds = Bounds;

      var path = InternalGetPath(bounds.LeftTop);

      if (Brush.IsVisible)
      {
        using (var brushGdi = BrushCacheGdi.Instance.BorrowBrush(Brush, bounds, g, Math.Max(ScaleX, ScaleY)))
        {
          g.FillPath(brushGdi, path);
        }
      }

      if (Pen.IsVisible)
      {
        using (var penGdi = PenCacheGdi.Instance.BorrowPen(Pen, bounds, g, Math.Max(ScaleX, ScaleY)))
        {
          g.DrawPath(penGdi, path);
        }
      }
      g.Restore(gs);
    }

    /// <summary>
    /// Represents the hit-test object for <see cref="ClosedCardinalSpline"/>.
    /// </summary>
    protected class ClosedCardinalSplineHitTestObject : GraphicBaseHitTestObject
    {
      /// <summary>
      /// Initializes a new instance of the <see cref="ClosedCardinalSplineHitTestObject"/> class.
      /// </summary>
      /// <param name="parent">The parent spline.</param>
      public ClosedCardinalSplineHitTestObject(ClosedCardinalSpline parent)
        : base(parent)
      {
      }

      /// <inheritdoc />
      public override IGripManipulationHandle[]? GetGrips(double pageScale, int gripLevel)
      {
        if (gripLevel <= 1)
        {
          var ls = (ClosedCardinalSpline)_hitobject;
          var pts = new PointF[ls._curvePoints.Count];
          var offset = ls.Location.AbsoluteVectorPivotToLeftUpper;
          for (int i = 0; i < pts.Length; i++)
          {
            pts[i] = (ls._curvePoints[i] + offset).ToGdi();
            var pt = ls._transformation.TransformPoint(pts[i]);
            pt = Transformation.TransformPoint(pt);
            pts[i] = pt;
          }

          var grips = new IGripManipulationHandle[gripLevel == 0 ? 1 : 1 + ls._curvePoints.Count];

          // Translation grips
          var path = new GraphicsPath();
          path.AddClosedCurve(pts, (float)ls._tension);
          path.Widen(new Pen(Color.Black, (float)(6 / pageScale)));
          grips[grips.Length - 1] = new MovementGripHandle(this, path, null);

          // PathNode grips
          if (gripLevel == 1)
          {
            float gripRadius = (float)(3 / pageScale);
            for (int i = 0; i < ls._curvePoints.Count; i++)
            {
              grips[i] = new ClosedCardinalSplinePathNodeGripHandle(this, i, pts[i].ToPointD2D(), gripRadius);
            }
          }
          return grips;
        }
        else
        {
          return base.GetGrips(pageScale, gripLevel);
        }
      }
    }

    private class ClosedCardinalSplinePathNodeGripHandle : PathNodeGripHandle
    {
      private int _pointNumber;
      private PointD2D _offset;
      private ISuspendToken? _suspendToken;

      /// <summary>
      /// Initializes a new instance of the <see cref="ClosedCardinalSplinePathNodeGripHandle"/> class.
      /// </summary>
      /// <param name="parent">The parent hit-test object.</param>
      /// <param name="pointNr">The spline point index.</param>
      /// <param name="gripCenter">The grip center in page coordinates.</param>
      /// <param name="gripRadius">The grip radius.</param>
      public ClosedCardinalSplinePathNodeGripHandle(IHitTestObject parent, int pointNr, PointD2D gripCenter, double gripRadius)
        : base(parent, new PointD2D(0, 0), gripCenter, gripRadius)
      {
        _pointNumber = pointNr;
        _offset = ((ClosedCardinalSpline)GraphObject).Location.AbsoluteVectorPivotToLeftUpper;
      }

      /// <inheritdoc />
      public override void MoveGrip(PointD2D newPosition)
      {
        newPosition = _parent.Transformation.InverseTransformPoint(newPosition);
        var obj = (ClosedCardinalSpline)GraphObject;
        newPosition = obj._transformation.InverseTransformPoint(newPosition);

        if (_suspendToken is null)
          _suspendToken = obj.SuspendGetToken();
        obj.SetPoint(_pointNumber, newPosition - _offset);
      }

      /// <inheritdoc />
      public override bool Deactivate()
      {
        var obj = (ClosedCardinalSpline)GraphObject;
        using (var token = obj.SuspendGetToken())
        {
          int otherPointIndex = _pointNumber == 0 ? 1 : 0;
          PointD2D oldOtherPointCoord = obj._transformation.TransformPoint(obj._curvePoints[otherPointIndex] + _offset); // transformiere in ParentCoordinaten

          // Calculate the new Size
          obj.CalculateAndSetBounds();
          obj.UpdateTransformationMatrix();
          PointD2D newOtherPointCoord = obj._transformation.TransformPoint(obj._curvePoints[otherPointIndex] + obj.Location.AbsoluteVectorPivotToLeftUpper);
          obj.ShiftPosition(oldOtherPointCoord - newOtherPointCoord);
        }

        _suspendToken?.Dispose();
        _suspendToken = null;

        return false;
      }
    }
  } // End Class
} // end Namespace
