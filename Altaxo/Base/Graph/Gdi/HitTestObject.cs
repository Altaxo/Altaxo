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

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;

namespace Altaxo.Graph.Gdi
{
	public abstract class HitTestObjectBase : IHitTestObject
	{
		#region Internal classes

		/// <summary>
		/// Grip that does nothing, but shows a blue polygon
		/// </summary>
		protected class NoopGrip : IGripManipulationHandle
		{
			private GraphicsPath _displayPath;

			public NoopGrip(GraphicsPath displayPath)
			{
				_displayPath = displayPath;
			}

			#region IGripManipulationHandle Members

			/// <summary>
			/// Activates this grip, providing the initial position of the mouse.
			/// </summary>
			/// <param name="initialPosition">Initial position of the mouse.</param>
			/// <param name="isActivatedUponCreation">If true the activation is called right after creation of this handle. If false,
			/// thie activation is due to a regular mouse click in this grip.</param>
			public void Activate(PointD2D initialPosition, bool isActivatedUponCreation)
			{
			}

			public bool Deactivate()
			{
				return false;
			}

			public void MoveGrip(PointD2D newPosition)
			{
			}

			/// <summary>Draws the grip in the graphics context.</summary>
			/// <param name="g">Graphics context.</param>
			/// <param name="pageScale">Current zoom factor that can be used to calculate pen width etc. for displaying the handle. Attention: this factor must not be used to transform the path of the handle.</param>
			public void Show(Graphics g, double pageScale)
			{
				using (var pen = new Pen(Color.Blue, (float)(1 / pageScale)))
				{
					g.DrawPath(pen, _displayPath);
				}
			}

			public bool IsGripHitted(PointD2D point)
			{
				return false;
			}

			#endregion IGripManipulationHandle Members
		}

		#endregion Internal classes

		/// <summary>
		/// Transformation matrix which transforms the coordinates of the parent of the hitted object (i.e. the parent layer)
		/// into page coordinates.
		/// </summary>
		protected TransformationMatrix2D _matrix;

		/// <summary>The hitted object.</summary>
		protected object _hitobject;

		#region IHitTestObject Members

		/// <summary>
		/// Creates a new HitTestObject.
		/// </summary>
		/// <param name="hitobject">The hitted object.</param>
		public HitTestObjectBase(object hitobject)
		{
			_hitobject = hitobject;
			_matrix = new TransformationMatrix2D();
		}

		/// <summary>
		/// This will return the transformation matrix.
		/// This matrix translates from coordinates of the parent of the object (i.e. mostly the parent layer) to global coordinates.
		/// </summary>
		public TransformationMatrix2D Transformation
		{
			get { return _matrix; }
		}

		/// <summary>
		/// Appends a transformation to the transformation matrix of the hit test object. Call this while walking down the hierarchie of objects.
		/// </summary>
		/// <param name="x">The transformation to append.</param>
		public virtual void Transform(TransformationMatrix2D x)
		{
			_matrix.AppendTransform(x);
		}

		public object HittedObject
		{
			get { return _hitobject; }
			set { _hitobject = value; }
		}

		public abstract GraphicsPath ObjectOutlineForArrangements { get; }

		/// <summary>
		/// Shows the grips, i.e. the special areas for manipulation of the object.
		/// </summary>
		/// <param name="pageScale"></param>
		/// <param name="gripLevel">The grip level. For 0, only the translation grip is shown.</param>
		/// <returns>Grip manipulation handles that are used to show the grips and to manipulate the object.</returns>
		public abstract IGripManipulationHandle[] GetGrips(double pageScale, int gripLevel);

		public virtual int GetNextGripLevel(int currentGripLevel)
		{
			return currentGripLevel;
		}

		/// <summary>
		/// Shifts the position of the object by x and y. Used to arrange objects.
		/// </summary>
		/// <param name="dx">Shift value of x in page coordinates.</param>
		/// <param name="dy">Shift value of y in page coordinates.</param>
		public abstract void ShiftPosition(double dx, double dy);

		/// <summary>
		/// Changes the size of the hitted item either in x or in y direction.
		/// </summary>
		/// <param name="x">If not null, this is the new x size of the hitted object.</param>
		/// <param name="y">If not null, this is the new y size of the hitted object.</param>
		public abstract void ChangeSize(double? x, double? y);

		private DoubleClickHandler _DoubleClick;

		public DoubleClickHandler DoubleClick
		{
			get { return _DoubleClick; }
			set { _DoubleClick = value; }
		}

		/// <summary>
		/// Handler to remove the hitted object. Should return true if the object is removed, otherwise false.
		/// </summary>
		private DoubleClickHandler _Remove;

		/// <summary>
		/// Handler to remove the hitted object. Should return true if the object is removed, otherwise false.
		/// </summary>
		public DoubleClickHandler Remove
		{
			get { return _Remove; }
			set { _Remove = value; }
		}

		/// <summary>
		/// This handles the double-click event
		/// </summary>
		/// <returns>False normally, true if the HitTestObject should be removed from the list of selected objects (i.e. because the object was deleted).</returns>
		public virtual bool OnDoubleClick()
		{
			if (DoubleClick != null)
				return DoubleClick(this);
			else
				return false;
		}

		private HostLayer _parentLayer;

		public HostLayer ParentLayer
		{
			get { return _parentLayer; }
			set { _parentLayer = value; }
		}

		#endregion IHitTestObject Members
	}

	/// <summary>
	/// This class holds the arrangement path by itself.
	/// </summary>
	public class HitTestObject : HitTestObjectBase
	{
		/// <summary>
		/// Path of the outline of the object. Is used to arrange objects. The path is in page coordinates.
		/// </summary>
		protected GraphicsPath _objectPath;

		#region IHitTestObject Members

		/// <summary>
		/// Creates a new HitTestObject.
		/// </summary>
		/// <param name="objectPath">Path of the object outline used for arrangement of multiple objects.
		/// You have to provide it in coordinates of the parent layer.</param>
		/// <param name="hitobject">The hitted object.</param>
		public HitTestObject(GraphicsPath objectPath, object hitobject)
			: base(hitobject)
		{
			_objectPath = objectPath;
		}

		/// <summary>
		/// Creates a new HitTestObject.
		/// </summary>
		/// <param name="objectPath">Path of the object outline used for arrangement of multiple objects.
		/// This path is in object world coordinates.</param>
		/// <param name="transformation">Transformation matrix of the object.</param>
		/// <param name="hitobject">The hitted object.</param>
		public HitTestObject(GraphicsPath objectPath, TransformationMatrix2D transformation, object hitobject)
			: base(hitobject)
		{
			_objectPath = objectPath;
			_objectPath.Transform(transformation);
		}

		/// <summary>
		/// Appends a transformation to the transformation matrix of the hit test object. Call this while walking down the hierarchie of objects.
		/// </summary>
		/// <param name="x">The transformation to append.</param>
		public override void Transform(TransformationMatrix2D x)
		{
			base.Transform(x);
			_objectPath.Transform(x);
		}

		/// <summary>
		/// Returns the object path in page coordinates. This path is used for the arrangement of multiple selected objects.
		/// </summary>
		public override GraphicsPath ObjectOutlineForArrangements
		{
			get
			{
				return _objectPath;
			}
		}

		/// <summary>
		/// Shows the grips, i.e. the special areas for manipulation of the object.
		/// </summary>
		/// <param name="pageScale"></param>
		/// <param name="gripLevel">The grip level. For 0, only the translation grip is shown.</param>
		/// <returns>Grip manipulation handles that are used to show the grips and to manipulate the object.</returns>
		public override IGripManipulationHandle[] GetGrips(double pageScale, int gripLevel)
		{
			return new IGripManipulationHandle[] { new NoopGrip(_objectPath) };
		}

		/// <summary>
		/// Shifts the position of the object by x and y. Used to arrange objects.
		/// </summary>
		/// <param name="dx">Shift value of x in page coordinates.</param>
		/// <param name="dy">Shift value of y in page coordinates.</param>
		public override void ShiftPosition(double dx, double dy)
		{
			// per default: do nothing
		}

		public override void ChangeSize(double? x, double? y)
		{
			// per default: do nothing
		}

		#endregion IHitTestObject Members
	}
}