#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2007 Dr. Dirk Lellinger
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
	public class LineShape : ShapeGraphic
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

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.LineGraphic", 0)]
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(LineShape), 1)]
		class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				LineShape s = (LineShape)obj;
				info.AddBaseValueEmbedded(s, typeof(LineShape).BaseType);

			}
			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{

				LineShape s = null != o ? (LineShape)o : new LineShape();
				info.GetBaseValueEmbedded(s, typeof(LineShape).BaseType, parent);

				return s;
			}
		}


		#endregion


		#region Constructors
		public LineShape()
		{
		}

		public LineShape(PointF startPosition)
		{
			this.SetStartPosition(startPosition);
		}

		public LineShape(float posX, float posY)
			: this(new PointF(posX, posY))
		{
		}

		public LineShape(PointF startPosition, PointF endPosition)
			:
			this(startPosition)
		{
			this.SetEndPosition(endPosition);
			this.AutoSize = false;
		}


		public LineShape(float startX, float startY, PointF endPosition)
			:
			this(new PointF(startX, startY), endPosition)
		{
		}

		public LineShape(float startX, float startY, float endX, float endY)
			:
			this(new PointF(startX, startY), new PointF(endX, endY))
		{
		}

		public LineShape(PointF startPosition, PointF endPosition, float lineWidth, Color lineColor)
			:
			this(startPosition)
		{
			this.SetEndPosition(endPosition);
			this.Pen.Width = lineWidth;
			this.Pen.Color = lineColor;
			this.AutoSize = false;
		}

		public LineShape(float startX, float startY, float endX, float endY, float lineWidth, Color lineColor)
			:
			this(new PointF(startX, startY), new PointF(endX, endY))
		{
			this.Pen.Width = lineWidth;
			this.Pen.Color = lineColor;
			this.AutoSize = false;
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


		public override GraphicsPath GetSelectionPath()
		{
			if (Pen.Width <= 5)
				return GetPath(5);
			else
				return GetPath(Pen.Width);
		}

		public override GraphicsPath GetObjectPath()
		{
			return GetPath(0);
		}

		protected GraphicsPath GetPath(float minWidth)
		{
			GraphicsPath gp = new GraphicsPath();
			Matrix myMatrix = new Matrix();

			gp.AddLine(X + _bounds.X, Y + _bounds.Y, X + _bounds.X + Width, Y + _bounds.Y + Height);
			if (Pen.Width != minWidth)
				gp.Widen(new Pen(Color.Black, minWidth));
			else
				gp.Widen(Pen);

			if (this.Rotation != 0)
			{
				myMatrix.RotateAt(-this._rotation, new PointF(X, Y), MatrixOrder.Append);
			}

			gp.Transform(myMatrix);
			return gp;
		}

		public override IHitTestObject HitTest(HitTestData htd)
		{
			HitTestObject result = null;
			PointF pt = htd.GetHittedPointInWorldCoord();
			GraphicsPath gp = GetSelectionPath();
			if (gp.IsVisible(pt))
			{
				result = new LineShapeHitTestObject(this);
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
			Pen.BrushRectangle = this._bounds;
			g.DrawLine(Pen, _bounds.X, _bounds.Y, _bounds.Right, _bounds.Bottom);
			g.Restore(gs);
		}
	


		protected class LineShapeHitTestObject : GraphicBaseHitTestObject
		{
			public LineShapeHitTestObject(LineShape parent)
				: base(parent.GetObjectPath(), parent.GetSelectionPath(), parent)
			{
			}

			public override IGripManipulationHandle[] GetGrips(double pageScale, int gripLevel)
			{
				if (gripLevel <= 1)
				{
					LineShape ls = (LineShape)_hitobject;
					PointF[] pts = new PointF[] { new PointF(0, 0), new PointF(ls.Width, ls.Height) };
					for (int i = 0; i < pts.Length; i++)
					{
						var pt = ls._transfoToLayerCoord.TransformPoint(pts[i]);
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
