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

using Altaxo.Serialization;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

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
				info.AddBaseValueEmbedded(s, typeof(OpenCardinalSpline).BaseType);
				info.AddValue("Tension", s._tension);
				info.CreateArray("Points", s._curvePoints.Count);
				for (int i = 0; i < s._curvePoints.Count; i++)
					info.AddValue("e", s._curvePoints[i]);
				info.CommitArray();
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				var s = null != o ? (OpenCardinalSpline)o : new OpenCardinalSpline();
				info.GetBaseValueEmbedded(s, typeof(OpenCardinalSpline).BaseType, parent);
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

		public OpenCardinalSpline()
			: base(new ItemLocationDirectAutoSize())
		{
		}

		public OpenCardinalSpline(IEnumerable<PointD2D> points)
			: this(points, DefaultTension)
		{
		}

		public OpenCardinalSpline(IEnumerable<PointD2D> points, double tension)
			: base(new ItemLocationDirectAutoSize())
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
		}

		public override bool CopyFrom(object obj)
		{
			var isCopied = base.CopyFrom(obj);
			if (isCopied && !object.ReferenceEquals(this, obj))
			{
				var from = obj as OpenCardinalSpline;
				if (null != from)
				{
					this._tension = from._tension;
					this._curvePoints.Clear();
					_curvePoints.AddRange(from._curvePoints);
				}
			}
			return isCopied;
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
					OnChanged();
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
		/// <param name="suppressChangeEvent">If true, suppresses the change event (ignored here).</param>
		protected override void SetSize(double width, double height, bool suppressChangeEvent)
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
				_curvePoints[i] -= bounds.Location;

			using (var token = _eventSuppressor.Suspend())
			{
				this.ShiftPosition(bounds.Location);
				((ItemLocationDirectAutoSize)_location).SetSizeInAutoSizeMode(bounds.Size);
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
			GraphicsPath gp = new GraphicsPath();

			if (_curvePoints.Count > 1)
			{
				PointF[] pt = new PointF[_curvePoints.Count];
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

		public override IHitTestObject HitTest(HitTestPointData htd)
		{
			HitTestObjectBase result = null;
			GraphicsPath gp = GetPath();
			if (gp.IsOutlineVisible((PointF)htd.GetHittedPointInWorldCoord(_transformation), _linePen))
			{
				result = new OpenBSplineHitTestObject(this);
			}
			else
			{
				gp.Transform(htd.GetTransformation(_transformation)); // Transform to page coord
				if (gp.IsOutlineVisible((PointF)htd.HittedPointInPageCoord, new Pen(Color.Black, 6)))
				{
					result = new OpenBSplineHitTestObject(this);
				}
			}

			if (result != null)
				result.DoubleClick = EhHitDoubleClick;

			return result;
		}

		private static bool EhHitDoubleClick(IHitTestObject o)
		{
			object hitted = o.HittedObject;
			Current.Gui.ShowDialog(ref hitted, "Line properties", true);
			((OpenCardinalSpline)hitted).OnChanged();
			return true;
		}

		public override void Paint(Graphics g, object obj)
		{
			GraphicsState gs = g.Save();
			TransformGraphics(g);
			var bounds = Bounds;
			var path = InternalGetPath(bounds.LeftTop);

			Pen.SetEnvironment((RectangleF)bounds, BrushX.GetEffectiveMaximumResolution(g, Math.Max(ScaleX, ScaleY)));
			g.DrawPath(Pen, path);
			if (_outlinePen != null && _outlinePen.IsVisible)
			{
				path.Widen(Pen);
				OutlinePen.SetEnvironment((RectangleF)bounds, BrushX.GetEffectiveMaximumResolution(g, Math.Max(ScaleX, ScaleY)));
				g.DrawPath(OutlinePen, path);
			}

			g.Restore(gs);
		}

		protected class OpenBSplineHitTestObject : GraphicBaseHitTestObject
		{
			public OpenBSplineHitTestObject(OpenCardinalSpline parent)
				: base(parent)
			{
			}

			public override IGripManipulationHandle[] GetGrips(double pageScale, int gripLevel)
			{
				if (gripLevel <= 1)
				{
					OpenCardinalSpline ls = (OpenCardinalSpline)_hitobject;
					PointF[] pts = new PointF[ls._curvePoints.Count];
					var offset = ls.Location.AbsoluteVectorPivotToLeftUpper;
					for (int i = 0; i < pts.Length; i++)
					{
						pts[i] = (PointF)(ls._curvePoints[i] + offset);
						var pt = ls._transformation.TransformPoint(pts[i]);
						pt = this.Transformation.TransformPoint(pt);
						pts[i] = pt;
					}

					IGripManipulationHandle[] grips = new IGripManipulationHandle[gripLevel == 0 ? 1 : 1 + ls._curvePoints.Count];

					// Translation grips
					GraphicsPath path = new GraphicsPath();
					path.AddCurve(pts, (float)ls._tension);
					path.Widen(new Pen(Color.Black, (float)(6 / pageScale)));
					grips[grips.Length - 1] = new MovementGripHandle(this, path, null);

					// PathNode grips
					if (gripLevel == 1)
					{
						float gripRadius = (float)(3 / pageScale);
						for (int i = 0; i < ls._curvePoints.Count; i++)
						{
							grips[i] = new BSplinePathNodeGripHandle(this, i, pts[i], gripRadius);
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
				using (var token = obj._eventSuppressor.Suspend())
				{
					int otherPointIndex = _pointNumber == 0 ? 1 : 0;
					PointD2D oldOtherPointCoord = obj._transformation.TransformPoint(obj._curvePoints[otherPointIndex] + _offset); // transformiere in ParentCoordinaten

					// Calculate the new Size
					obj.CalculateAndSetBounds();
					obj.UpdateTransformationMatrix();
					PointD2D newOtherPointCoord = obj._transformation.TransformPoint(obj._curvePoints[otherPointIndex] + obj.Location.AbsoluteVectorPivotToLeftUpper);
					obj.ShiftPosition(oldOtherPointCoord - newOtherPointCoord);
				}
				return false;
			}
		}
	} // End Class
} // end Namespace