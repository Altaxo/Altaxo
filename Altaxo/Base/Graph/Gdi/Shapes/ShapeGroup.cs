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
using System.Collections.Generic;


namespace Altaxo.Graph.Gdi.Shapes
{
	/// <summary>
	/// Groups two or more graphics objects together. This is an autosize shape.
	/// </summary>
	public class ShapeGroup : GraphicBase
	{
		/// <summary>List of grouped objects</summary>
		List<GraphicBase> _groupedObjects;


		#region Serialization


		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ShapeGroup), 0)]
		class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				var s = (ShapeGroup)obj;
				info.AddBaseValueEmbedded(s, typeof(ShapeGroup).BaseType);

				info.CreateArray("Elements", s._groupedObjects.Count);
				foreach (var e in s._groupedObjects)
					info.AddValue("e", e);
				info.CommitArray();

			}
			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{

				var s = null != o ? (ShapeGroup)o : new ShapeGroup();
				info.GetBaseValueEmbedded(s, typeof(ShapeGroup).BaseType, parent);

				int count = info.OpenArray("Elements");
				var list = new GraphicBase[count];
				for (int i = 0; i < count; i++)
					list[i] = (GraphicBase)info.GetValue("e", parent);
				info.CloseArray(count);
				s.AddRange(list);

				return s;
			}
		}

		#endregion

		private ShapeGroup()
		{
		}

		/// <summary>
		/// Constructs a shape group from at least two objects.
		/// </summary>
		/// <param name="objectsToGroup">Objects to group together. An exception is thrown if the list contains less than 2 items.</param>
		public ShapeGroup(ICollection<GraphicBase> objectsToGroup)
		{
			if (objectsToGroup.Count < 2)
				throw new ArgumentException("objectsToGroup must contain at least two elements");

			_groupedObjects = new List<GraphicBase>();

			AddRange(objectsToGroup);
		}

		/// <summary>
		/// Copy constructor.
		/// </summary>
		/// <param name="from">Another shape group to copy from. The objects of this shape group are cloned before added to the new group.</param>
		public ShapeGroup(ShapeGroup from)
			: base(from) // all is done here, since CopyFrom is virtual!
		{
		}


		public override bool CopyFrom(object obj)
		{
			bool isCopied = base.CopyFrom(obj);
			if (isCopied && !object.ReferenceEquals(this, obj))
			{
				var from = obj as ShapeGroup;
				if (null != from)
				{
					// deep copy of the objects
					_groupedObjects = new List<GraphicBase>(from._groupedObjects.Count);
					foreach (GraphicBase go in from._groupedObjects)
						_groupedObjects.Add((GraphicBase)go.Clone());
				}
			}
			return isCopied;
		}

		/// <summary>
		/// Clones the shape group.
		/// </summary>
		/// <returns>A clone of this shape group.</returns>
		public override object Clone()
		{
			return new ShapeGroup(this);
		}

		public override bool AutoSize
		{
			get
			{
				return true;
			}
		}

		/// <summary>
		/// Get the object outline for arrangements in object world coordinates.
		/// </summary>
		/// <returns>Object outline for arrangements in object world coordinates</returns>
		public override GraphicsPath GetObjectOutlineForArrangements()
		{
			GraphicsPath result = new GraphicsPath();
			foreach (var ele in _groupedObjects)
				result.AddPath(ele.GetObjectOutlineForArrangements(), false);
			return result;
		}

		/// <summary>
		/// Paint the shape group in the graphic context.
		/// </summary>
		/// <param name="g">Graphic context.</param>
		/// <param name="obj"></param>
		public override void Paint(Graphics g, object obj)
		{
			GraphicsState gs = g.Save();
			this.TransformGraphics(g);

			foreach (GraphicBase graphics in _groupedObjects)
				graphics.Paint(g, this);

			g.Restore(gs);
		}


		public GraphicsPath GetSelectionPath()
		{
			GraphicsPath gp = new GraphicsPath();
			Matrix myMatrix = new Matrix();

			gp.AddRectangle(new RectangleF((float)(X + _bounds.X), (float)(Y + _bounds.Y), (float)Width, (float)Height));
			if (this.Rotation != 0)
			{
				myMatrix.RotateAt((float)(-this._rotation), (PointF)Position, MatrixOrder.Append);
			}

			gp.Transform(myMatrix);
			return gp;
		}

		public override IHitTestObject HitTest(HitTestPointData htd)
		{
			IHitTestObject result = base.HitTest(htd);
			if (result != null)
				result.DoubleClick = EhHitDoubleClick;
			return result;
		}

		protected static bool EhHitDoubleClick(IHitTestObject o)
		{
			object hitted = o.HittedObject;
			Current.Gui.ShowDialog(ref hitted, "Shape group properties", true);
			((ShapeGroup)hitted).OnChanged();
			return true;
		}

		#region Addition of objects

		/// <summary>
		/// Adds an item to this shape group.
		/// </summary>
		/// <param name="obj">Item to add.</param>
		public void Add(GraphicBase obj)
		{
			obj.SetCoordinatesByAppendInverseTransformation(this._transformation, true);
			_groupedObjects.Add(obj);
			obj.ParentObject = this;
			AdjustPosition();
			OnChanged();
		}

		/// <summary>
		/// Adds a number of items to this shape group.
		/// </summary>
		/// <param name="list">List of items to add.</param>
		public void AddRange(IEnumerable<GraphicBase> list)
		{
			foreach (var obj in list)
			{
				obj.SetCoordinatesByAppendInverseTransformation(this._transformation, true);
				_groupedObjects.Add(obj);
				obj.ParentObject = this;
			}
			AdjustPosition();
			OnChanged();
		}


		/// <summary>Gets access to the grouped objects. This function has to be used with care. No size/position update of the ShapeGroup is done if the position/size/rotation/share values of one of the grouped objects is changed.
		/// One the other hand, you can change other properties, like colors and brushes, of the individual grouped objects.</summary>
		public IEnumerable<GraphicBase> GroupedObjects
		{
			get
			{
				return _groupedObjects.AsReadOnly();
			}
		}

		/// <summary>
		/// Adjusts the position and auto size of this group shape according to the contained elements. Must be called after changing any of the contained elements.
		/// </summary>
		public void AdjustPosition()
		{
			RectangleD bounds = RectangleD.Empty;
			bool boundsInitialized = false;
			foreach (var e in _groupedObjects)
			{
				var p1 = new PointD2D(0, 0);
				var p2 = new PointD2D(1, 0);
				var p3 = new PointD2D(0, 1);
				var p4 = new PointD2D(1, 1);

				p1 = e.RelativeLocalToAbsoluteParentCoordinates(p1);
				p2 = e.RelativeLocalToAbsoluteParentCoordinates(p2);
				p3 = e.RelativeLocalToAbsoluteParentCoordinates(p3);
				p4 = e.RelativeLocalToAbsoluteParentCoordinates(p4);

				if (boundsInitialized)
				{
					bounds.ExpandToInclude(p1);
				}
				else
				{
					bounds = new RectangleD(p1.X, p1.Y, 0, 0);
					boundsInitialized = true;
				}
				bounds.ExpandToInclude(p2);
				bounds.ExpandToInclude(p3);
				bounds.ExpandToInclude(p4);
			}

			if (bounds != _bounds)
			{
				// adjust position in this way that bounds.X and bounds.Y get zero
				var dx = bounds.X;
				var dy = bounds.Y;


				foreach (var e in _groupedObjects)
				{
					e.ShiftPosition(-dx, -dy);
				}
				this._position.X += dx;
				this._position.Y += dy;
				bounds.Location = new PointD2D(0, 0);
				this._bounds = bounds;
				UpdateTransformationMatrix();
			}
		}

		/// <summary>
		/// Ungroup the items. The items are transformed according to the transformations of this shape group.
		/// After returning from this function, the shape group contains no items and can be deleted.
		/// </summary>
		/// <returns>The ungrouped and transformed items of this shape group.</returns>
		public GraphicBase[] Ungroup()
		{
			foreach (GraphicBase e in _groupedObjects)
			{
				e.SetCoordinatesByAppendTransformation(this._transformation, true);
			}

			var result = _groupedObjects.ToArray();
			_groupedObjects.Clear();
			return result;
		}

		#endregion
	}
}
