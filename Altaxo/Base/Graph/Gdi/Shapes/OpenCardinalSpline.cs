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
using Altaxo.Geometry;

namespace Altaxo.Graph.Gdi.Shapes
{
  [Serializable]
  public class OpenCardinalSpline : OpenPathShapeBase
  {
    private static double _defaultTension = 0.5;

    private List<PointD2D> _curvePoints = new List<PointD2D>();
    private double _tension = _defaultTension;

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(OpenCardinalSpline), 0)]
    private class XmlSerializationSurrogate2 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (OpenCardinalSpline)obj;
        info.AddBaseValueEmbedded(s, typeof(OpenCardinalSpline).BaseType!);
        info.AddValue("Tension", s._tension);
        info.CreateArray("Points", s._curvePoints.Count);
        for (int i = 0; i < s._curvePoints.Count; i++)
          info.AddValue("e", s._curvePoints[i]);
        info.CommitArray();
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (OpenCardinalSpline?)o ?? new OpenCardinalSpline(info);
        info.GetBaseValueEmbedded(s, typeof(OpenCardinalSpline).BaseType!, parent);
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

    protected OpenCardinalSpline(Altaxo.Serialization.Xml.IXmlDeserializationInfo info)
      : base(new ItemLocationDirectAutoSize(), info)
    {
    }

    public OpenCardinalSpline(Altaxo.Main.Properties.IReadOnlyPropertyBag context)
      : base(new ItemLocationDirectAutoSize(), context)
    {
    }

    public OpenCardinalSpline(IEnumerable<PointD2D> points, Altaxo.Main.Properties.IReadOnlyPropertyBag context)
      : this(points, DefaultTension, context)
    {
    }

    public OpenCardinalSpline(IEnumerable<PointD2D> points, double tension, Altaxo.Main.Properties.IReadOnlyPropertyBag context)
      : base(new ItemLocationDirectAutoSize(), context)
    {
      _curvePoints.AddRange(points);
      _tension = Math.Abs(tension);

      if (!(_curvePoints.Count >= 2))
        throw new ArgumentException("Number of curve points has to be >= 2");

      CalculateAndSetBounds();
    }

    public OpenCardinalSpline(OpenCardinalSpline from)
      : base(from) // all is done here, since CopyFrom is virtual!
    {
      CopyFrom(from, false);
    }

    protected void CopyFrom(OpenCardinalSpline from, bool withBaseMembers)
    {
      if (withBaseMembers)
        base.CopyFrom(from, withBaseMembers);

      _tension = from._tension;
      _curvePoints.Clear();
      _curvePoints.AddRange(from._curvePoints);
    }

    public override bool CopyFrom(object obj)
    {
      if (ReferenceEquals(this, obj))
        return true;
      if (obj is OpenCardinalSpline from)
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

    public static double DefaultTension { get { return _defaultTension; } }

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

    public override bool AllowNegativeSize
    {
      get
      {
        return true;
      }
    }

    public override object Clone()
    {
      return new OpenCardinalSpline(this);
    }

    /// <summary>
    /// Normally sets the size of the item. For the OpenCardinalSpline, the size is calculated internally. Thus, the function is overriden in order to ignore both parameters.
    /// </summary>
    /// <param name="width">Unscaled width of the item (ignored here).</param>
    /// <param name="height">Unscaled height of the item (ignored here).</param>
    /// <param name="eventFiring">If true, suppresses the change event (ignored here).</param>
    protected override void SetSize(double width, double height, Main.EventFiring eventFiring)
    {
    }

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

    public void SetPoint(int idx, PointD2D newPos)
    {
      _curvePoints[idx] = newPos;
    }

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

      if (_curvePoints.Count > 1)
      {
        var pt = new PointF[_curvePoints.Count];
        for (int i = 0; i < _curvePoints.Count; i++)
          pt[i] = new PointF((float)(_curvePoints[i].X + offset.X), (float)(_curvePoints[i].Y + offset.Y));

        gp.AddCurve(pt, (float)_tension);
      }
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

    public override IHitTestObject? HitTest(HitTestPointData htd)
    {
      HitTestObjectBase? result = null;
      GraphicsPath gp = GetPath();
      using var linePenGdi = PenCacheGdi.Instance.BorrowPen(_linePen);
      if (gp.IsOutlineVisible(htd.GetHittedPointInWorldCoord(_transformation).ToGdi(), linePenGdi))
      {
        result = new OpenBSplineHitTestObject(this);
      }
      else
      {
        gp.Transform(htd.GetTransformation(_transformation).ToGdi()); // Transform to page coord
        if (gp.IsOutlineVisible(htd.HittedPointInPageCoord.ToGdi(), new Pen(Color.Black, 6)))
        {
          result = new OpenBSplineHitTestObject(this);
        }
      }

      if (result is not null)
        result.DoubleClick = EhHitDoubleClick;

      return result;
    }

    private static bool EhHitDoubleClick(IHitTestObject o)
    {
      object hitted = o.HittedObject;
      Current.Gui.ShowDialog(ref hitted, "Line properties", true);
      ((OpenCardinalSpline)hitted).EhSelfChanged(EventArgs.Empty);
      return true;
    }

    public override void Paint(Graphics g, IPaintContext paintContext)
    {
      GraphicsState gs = g.Save();
      TransformGraphics(g);
      var bounds = Bounds;
      var path = InternalGetPath(bounds.LeftTop);

      using var penGdi = PenCacheGdi.Instance.BorrowPen(Pen, bounds, g, Math.Max(ScaleX, ScaleY));
      g.DrawPath(penGdi, path);
      if (_outlinePen is not null && _outlinePen.IsVisible)
      {
        path.Widen(penGdi);
        using var outlinePenGdi = PenCacheGdi.Instance.BorrowPen(_outlinePen, bounds, g, Math.Max(ScaleX, ScaleY));
        g.DrawPath(outlinePenGdi, path);
      }

      g.Restore(gs);
    }

    protected class OpenBSplineHitTestObject : GraphicBaseHitTestObject
    {
      public OpenBSplineHitTestObject(OpenCardinalSpline parent)
        : base(parent)
      {
      }

      public override IGripManipulationHandle[]? GetGrips(double pageScale, int gripLevel)
      {
        if (gripLevel <= 1)
        {
          var ls = (OpenCardinalSpline)_hitobject;
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
          path.AddCurve(pts, (float)ls._tension);
          path.Widen(new Pen(Color.Black, (float)(6 / pageScale)));
          grips[grips.Length - 1] = new MovementGripHandle(this, path, null);

          // PathNode grips
          if (gripLevel == 1)
          {
            float gripRadius = (float)(3 / pageScale);
            for (int i = 0; i < ls._curvePoints.Count; i++)
            {
              grips[i] = new BSplinePathNodeGripHandle(this, i, pts[i].ToPointD2D(), gripRadius);
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

    private class BSplinePathNodeGripHandle : PathNodeGripHandle
    {
      private int _pointNumber;
      private PointD2D _offset;

      public BSplinePathNodeGripHandle(IHitTestObject parent, int pointNr, PointD2D gripCenter, double gripRadius)
        : base(parent, new PointD2D(0, 0), gripCenter, gripRadius)
      {
        _pointNumber = pointNr;
        _offset = ((OpenCardinalSpline)GraphObject).Location.AbsoluteVectorPivotToLeftUpper;
      }

      public override void MoveGrip(PointD2D newPosition)
      {
        newPosition = _parent.Transformation.InverseTransformPoint(newPosition);
        var obj = (OpenCardinalSpline)GraphObject;
        newPosition = obj._transformation.InverseTransformPoint(newPosition);
        obj.SetPoint(_pointNumber, newPosition - _offset);
      }

      public override bool Deactivate()
      {
        var obj = (OpenCardinalSpline)GraphObject;
        using (var token = obj.SuspendGetToken())
        {
          int otherPointIndex = _pointNumber == 0 ? 1 : 0;
          PointD2D oldOtherPointCoord = obj._transformation.TransformPoint(obj._curvePoints[otherPointIndex] + _offset); // transformiere in ParentCoordinaten

          // Calculate the new Size
          obj.CalculateAndSetBounds();
          obj.UpdateTransformationMatrix();
          PointD2D newOtherPointCoord = obj._transformation.TransformPoint(obj._curvePoints[otherPointIndex] + obj.Location.AbsoluteVectorPivotToLeftUpper);
          obj.ShiftPosition(oldOtherPointCoord - newOtherPointCoord);

          token.Resume();
        }
        return false;
      }
    }
  } // End Class
} // end Namespace
