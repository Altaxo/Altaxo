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
using System.Drawing;
using System.Drawing.Drawing2D;
using Altaxo.Serialization;

namespace Altaxo.Graph.Gdi.Shapes
{
	[Serializable]
	public class LineShape : OpenPathShapeBase
	{
		#region Serialization

		#region Clipboard serialization

		protected LineShape(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
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
			LineShape s = this;
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
			LineShape s = (LineShape)base.SetObjectData(obj, info, context, selector);
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

		private class DeprecatedLineShape : ClosedPathShapeBase
		{
			/// <summary>
			/// Get the object outline for arrangements in object world coordinates.
			/// </summary>
			/// <returns>Object outline for arrangements in object world coordinates</returns>
			public override GraphicsPath GetObjectOutlineForArrangements()
			{
				throw new NotImplementedException();
			}

			public override void Paint(Graphics g, object obj)
			{
				throw new NotImplementedException();
			}

			public override object Clone()
			{
				throw new NotImplementedException();
			}
		}

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.LineGraphic", 0)]
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.Gdi.Shapes.LineShape", 1)]
		class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				throw new NotSupportedException("Can not serialize old versions, maybe this is a programming error");
				/*
				LineShape s = (LineShape)obj;
				info.AddBaseValueEmbedded(s, typeof(LineShape).BaseType);
				*/
			}
			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{

				var s = null != o ? (DeprecatedLineShape)o : new DeprecatedLineShape();
				info.GetBaseValueEmbedded(s, typeof(DeprecatedLineShape).BaseType, parent);

				var l = new LineShape();
				l.CopyFrom(s);
				l.Pen = s.Pen; // we don't need to clone, since it is abandoned anyway

				return l;
			}
		}


		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(LineShape), 2)]
		class XmlSerializationSurrogate2 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				var s = (LineShape)obj;
				info.AddBaseValueEmbedded(s, typeof(LineShape).BaseType);

			}
			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{

				var s = null != o ? (LineShape)o : new LineShape();
				info.GetBaseValueEmbedded(s, typeof(LineShape).BaseType, parent);

				return s;
			}
		}

		#endregion


		#region Constructors
		public LineShape()
		{
		}

		public LineShape(PointD2D startPosition)
		{
			this.Position = startPosition;
		}

		public LineShape(double posX, double posY)
			: this(new PointD2D(posX, posY))
		{
		}

		public LineShape(PointD2D startPosition, PointD2D endPosition)
			:
			this(startPosition)
		{
			this._bounds.Width = endPosition.X - startPosition.X;
			this._bounds.Height = endPosition.Y - startPosition.Y;
		}


		public LineShape(double startX, double startY, PointD2D endPosition)
			:
			this(new PointD2D(startX, startY), endPosition)
		{
		}

		public LineShape(double startX, double startY, double endX, double endY)
			:
			this(new PointD2D(startX, startY), new PointD2D(endX, endY))
		{
		}

		public LineShape(PointD2D startPosition, PointD2D endPosition, double lineWidth, NamedColor lineColor)
			:
			this(startPosition)
		{
			this._bounds.Width = endPosition.X - startPosition.X;
			this._bounds.Height = endPosition.Y - startPosition.Y;
			this.Pen.Width = (float)lineWidth;
			this.Pen.Color = lineColor;
		}

		public LineShape(double startX, double startY, double endX, double endY, double lineWidth, NamedColor lineColor)
			:
			this(new PointD2D(startX, startY), new PointD2D(endX, endY))
		{
			this.Pen.Width = (float)lineWidth;
			this.Pen.Color = lineColor;
		}

		public LineShape(LineShape from)
			: base(from)
		{
		}

		#endregion

		public override bool AllowNegativeSize
		{
			get
			{
				return true;
			}
		}

		public override object Clone()
		{
			return new LineShape(this);
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
			gp.AddLine((float)(_bounds.X), (float)(_bounds.Y), (float)(_bounds.X + Width), (float)(_bounds.Y + Height));
			return gp;
		}

		public override IHitTestObject HitTest(HitTestPointData htd)
		{
			HitTestObjectBase result = null;
			GraphicsPath gp = GetPath();
			if (gp.IsOutlineVisible((PointF)htd.GetHittedPointInWorldCoord(_transformation), _linePen))
			{
				result = new LineShapeHitTestObject(this);
			}
			else
			{
				gp.Transform(htd.GetTransformation(_transformation)); // Transform to page coord
				if (gp.IsOutlineVisible((PointF)htd.HittedPointInPageCoord, new Pen(Color.Black, 6)))
				{
					result = new LineShapeHitTestObject(this);
				}
			}

			if (result != null)
				result.DoubleClick = EhHitDoubleClick;

			return result;
		}

		static bool EhHitDoubleClick(IHitTestObject o)
		{
			object hitted = o.HittedObject;
			Current.Gui.ShowDialog(ref hitted, "Line properties", true);
			((LineShape)hitted).OnChanged();
			return true;
		}





		public override void Paint(Graphics g, object obj)
		{
			GraphicsState gs = g.Save();
			TransformGraphics(g);
			Pen.SetEnvironment((RectangleF)_bounds, BrushX.GetEffectiveMaximumResolution(g, Math.Max(_scaleX, _scaleY)));
			g.DrawLine(Pen, (float)_bounds.X, (float)_bounds.Y, (float)_bounds.Right, (float)_bounds.Bottom);

			if (_outlinePen != null && _outlinePen.IsVisible)
			{
				GraphicsPath p = new GraphicsPath();
				p.AddLine((float)_bounds.X, (float)_bounds.Y, (float)_bounds.Right, (float)_bounds.Bottom);
				p.Widen(Pen);
				OutlinePen.SetEnvironment((RectangleF)_bounds, BrushX.GetEffectiveMaximumResolution(g, Math.Max(_scaleX, _scaleY)));
				g.DrawPath(OutlinePen, p);
			}

			g.Restore(gs);
		}



		protected class LineShapeHitTestObject : GraphicBaseHitTestObject
		{
			public LineShapeHitTestObject(LineShape parent)
				: base(parent)
			{
			}

			public override IGripManipulationHandle[] GetGrips(double pageScale, int gripLevel)
			{
				if (gripLevel <= 1)
				{
					LineShape ls = (LineShape)_hitobject;
					PointF[] pts = new PointF[] { new PointF(0, 0), new PointF((float)ls.Width, (float)ls.Height) };
					for (int i = 0; i < pts.Length; i++)
					{
						var pt = ls._transformation.TransformPoint(pts[i]);
						pt = this.Transformation.TransformPoint(pt);
						pts[i] = pt;
					}

					IGripManipulationHandle[] grips = new IGripManipulationHandle[gripLevel == 0 ? 1 : 3];

					// Translation grips
					GraphicsPath path = new GraphicsPath();
					path.AddLine(pts[0], pts[1]);
					path.Widen(new Pen(Color.Black, (float)(6 / pageScale)));
					grips[0] = new MovementGripHandle(this, path, null);

					// PathNode grips
					if (gripLevel == 1)
					{
						grips[2] = grips[0]; // put the movement grip to the background, the two NodeGrips need more priority
						float gripRadius = (float)(3 / pageScale);
						grips[0] = new PathNodeGripHandle(this, new PointF(0, 0), pts[0], gripRadius);
						grips[1] = new PathNodeGripHandle(this, new PointF(1, 1), pts[1], gripRadius);
					}
					return grips;
				}
				else
				{
					return base.GetGrips(pageScale, gripLevel);
				}
			}
		}
	} // End Class
} // end Namespace
