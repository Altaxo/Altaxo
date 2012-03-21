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
#endregion

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using Altaxo.Serialization;

namespace Altaxo.Graph.Gdi.Shapes
{
	[Serializable]
	public class ClosedCardinalSpline : ClosedPathShapeBase
	{
		static double _defaultTension = 0.5;

		List<PointD2D> _curvePoints = new List<PointD2D>();
		double _tension = _defaultTension;


		#region Serialization

		#region Clipboard serialization

		protected ClosedCardinalSpline(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			SetObjectData(this, info, context, null);
		}

		/// <summary>
		/// Serializes LineGraphic. 
		/// </summary>
		/// <param name="info">The serialization info.</param>
		/// <param name="context">The streaming context.</param>
		public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			ClosedCardinalSpline s = this;
			base.GetObjectData(info, context);
		}
		/// <summary>
		/// Deserializes the LineGraphic Version 0.
		/// </summary>
		/// <param name="obj">The empty SLineGraphic object to deserialize into.</param>
		/// <param name="info">The serialization info.</param>
		/// <param name="context">The streaming context.</param>
		/// <param name="selector">The deserialization surrogate selector.</param>
		/// <returns>The deserialized LineGraphic.</returns>
		public override object SetObjectData(object obj, System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context, System.Runtime.Serialization.ISurrogateSelector selector)
		{
			ClosedCardinalSpline s = (ClosedCardinalSpline)base.SetObjectData(obj, info, context, selector);
			return s;
		}


		/// <summary>
		/// Finale measures after deserialization.
		/// </summary>
		/// <param name="obj">Not used.</param>
		public override void OnDeserialization(object obj)
		{
			base.OnDeserialization(obj);
		}
		#endregion

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ClosedCardinalSpline), 0)]
		class XmlSerializationSurrogate2 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				var s = (ClosedCardinalSpline)obj;
				info.AddBaseValueEmbedded(s, typeof(ClosedCardinalSpline).BaseType);
				info.AddValue("Tension", s._tension);
				info.CreateArray("Points", s._curvePoints.Count);
				for (int i = 0; i < s._curvePoints.Count; i++)
					info.AddValue("e", s._curvePoints[i]);
				info.CommitArray();
			}
			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{

				var s = null != o ? (ClosedCardinalSpline)o : new ClosedCardinalSpline();
				info.GetBaseValueEmbedded(s, typeof(ClosedCardinalSpline).BaseType, parent);
				s._tension = info.GetDouble("Tension");
				s._curvePoints.Clear();
				int count = info.OpenArray("Points");
				for (int i = 0; i < count; i++)
					s._curvePoints.Add((PointD2D)info.GetValue("e", s));
				info.CloseArray(count);
				return s;
			}
		}

		#endregion


		#region Constructors
		public ClosedCardinalSpline()
		{
		}

		public ClosedCardinalSpline(IEnumerable<PointD2D> points)
			: this(points, DefaultTension)
		{
		}


		public ClosedCardinalSpline(IEnumerable<PointD2D> points, double tension)
		{
			_curvePoints.AddRange(points);
			_tension = Math.Abs(tension);

			if (!(_curvePoints.Count > 2))
				throw new ArgumentException("Number of curve points has to be > 2");

			CalculateAndSetBounds();
		}


		public ClosedCardinalSpline(ClosedCardinalSpline from)
			: base(from)  // all is done here, since CopyFrom is virtual!
		{
		}

		public override bool CopyFrom(object obj)
		{
			var isCopied = base.CopyFrom(obj);
			if (isCopied && !object.ReferenceEquals(this, obj))
			{
				var from = obj as ClosedCardinalSpline;
				if (null != from)
				{
					this._tension = from._tension;
					this._curvePoints.Clear();
					_curvePoints.AddRange(from._curvePoints);
				}
			}
			return isCopied;
		}

		#endregion


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
				// TODO adjust width and size to reflect the new positions of the curve points
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
			return new ClosedCardinalSpline(this);
		}

		/// <summary>
		/// Normally sets the size of the item. For the ClosedCardinalSpline, the size is calculated internally. Thus, the function is overriden in order to ignore both parameters.
		/// </summary>
		/// <param name="width">Unscaled width of the item (ignored here).</param>
		/// <param name="height">Unscaled height of the item (ignored here).</param>
		/// <param name="suppressChangedEvent">Suppressed the change event (ignored here).</param>
		protected override void SetSize(double width, double height, bool suppressChangedEvent)
		{
		}

		void CalculateAndSetBounds()
		{
			var path = GetPath();
			var bounds = path.GetBounds();
			_position += bounds.Location;
			for (int i = 0; i < _curvePoints.Count; i++)
				_curvePoints[i] -= bounds.Location;
			_bounds = new RectangleD(0, 0, bounds.Width, bounds.Height);
			UpdateTransformationMatrix();
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

		/// <summary>
		/// Gets the path of the object in object world coordinates.
		/// </summary>
		/// <returns></returns>
		protected GraphicsPath GetPath()
		{
			GraphicsPath gp = new GraphicsPath();

			PointF[] pt = new PointF[_curvePoints.Count];
			for (int i = 0; i < _curvePoints.Count; i++)
				pt[i] = new PointF((float)_curvePoints[i].X, (float)_curvePoints[i].Y);
			gp.AddClosedCurve(pt, (float)_tension);
			return gp;
		}

		public override IHitTestObject HitTest(HitTestPointData htd)
		{
			HitTestObjectBase result = null;
			GraphicsPath gp = GetPath();

			if (this._fillBrush.IsVisible && gp.IsVisible((PointF)htd.GetHittedPointInWorldCoord(_transformation)))
			{
				result = new ClosedCardinalSplineHitTestObject(this);
			}
			else if (this._linePen.IsVisible && gp.IsOutlineVisible((PointF)htd.GetHittedPointInWorldCoord(_transformation), _linePen))
			{
				result = new ClosedCardinalSplineHitTestObject(this);
			}
			else
			{
				gp.Transform(htd.GetTransformation(_transformation)); // Transform to page coord
				if (gp.IsOutlineVisible((PointF)htd.HittedPointInPageCoord, new Pen(Color.Black, 6)))
				{
					result = new ClosedCardinalSplineHitTestObject(this);
				}
			}

			if (result != null)
				result.DoubleClick = EhHitDoubleClick;

			return result;
		}

		static new bool EhHitDoubleClick(IHitTestObject o)
		{
			object hitted = o.HittedObject;
			Current.Gui.ShowDialog(ref hitted, "Line properties", true);
			((ClosedCardinalSpline)hitted).OnChanged();
			return true;
		}

		public override void Paint(Graphics g, object obj)
		{
			GraphicsState gs = g.Save();
			TransformGraphics(g);

			var path = GetPath();

			if (Brush.IsVisible)
			{
				Brush.SetEnvironment((RectangleF)_bounds, BrushX.GetEffectiveMaximumResolution(g, Math.Max(_scaleX, _scaleY)));
				g.FillPath(Brush, path);
			}

			if (Pen.IsVisible)
			{
				Pen.SetEnvironment((RectangleF)_bounds, BrushX.GetEffectiveMaximumResolution(g, Math.Max(_scaleX, _scaleY)));
				g.DrawPath(Pen, path);
			}
			g.Restore(gs);
		}



		protected class ClosedCardinalSplineHitTestObject : GraphicBaseHitTestObject
		{
			public ClosedCardinalSplineHitTestObject(ClosedCardinalSpline parent)
				: base(parent)
			{
			}

			public override IGripManipulationHandle[] GetGrips(double pageScale, int gripLevel)
			{
				if (gripLevel <= 1)
				{
					ClosedCardinalSpline ls = (ClosedCardinalSpline)_hitobject;
					PointF[] pts = new PointF[ls._curvePoints.Count];
					for (int i = 0; i < pts.Length; i++)
					{
						pts[i] = (PointF)ls._curvePoints[i];
						var pt = ls._transformation.TransformPoint(pts[i]);
						pt = this.Transformation.TransformPoint(pt);
						pts[i] = pt;
					}

					IGripManipulationHandle[] grips = new IGripManipulationHandle[gripLevel == 0 ? 1 : 1 + ls._curvePoints.Count];

					// Translation grips
					GraphicsPath path = new GraphicsPath();
					path.AddClosedCurve(pts, (float)ls._tension);
					path.Widen(new Pen(Color.Black, (float)(6 / pageScale)));
					grips[grips.Length - 1] = new MovementGripHandle(this, path, null);

					// PathNode grips
					if (gripLevel == 1)
					{
						float gripRadius = (float)(3 / pageScale);
						for (int i = 0; i < ls._curvePoints.Count; i++)
						{
							grips[i] = new ClosedCardinalSplinePathNodeGripHandle(this, i, pts[i], gripRadius);
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

		class ClosedCardinalSplinePathNodeGripHandle : PathNodeGripHandle
		{
			int _pointNumber;

			public ClosedCardinalSplinePathNodeGripHandle(IHitTestObject parent, int pointNr, PointD2D gripCenter, double gripRadius)
				: base(parent, new PointD2D(0, 0), gripCenter, gripRadius)
			{
				_pointNumber = pointNr;
			}


			public override void MoveGrip(PointD2D newPosition)
			{
				newPosition = _parent.Transformation.InverseTransformPoint(newPosition);
				var obj = (ClosedCardinalSpline)GraphObject;
				newPosition = obj._transformation.InverseTransformPoint(newPosition);
				obj.SetPoint(_pointNumber, newPosition);
			}

			public override bool Deactivate()
			{
				var obj = (ClosedCardinalSpline)GraphObject;
				obj.CalculateAndSetBounds();
				return false;
			}
		}
	} // End Class
} // end Namespace
