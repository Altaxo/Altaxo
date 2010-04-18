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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using Altaxo.Serialization;

namespace Altaxo.Graph.Gdi
{
	using Shapes;
	public class HitTestObject : IHitTestObject
	{
		#region Internal classes

		/// <summary>
		/// Grip that does nothing, but shows a blue polygon
		/// </summary>
		protected class NoopGrip : IGripManipulationHandle
		{
			GraphicsPath _displayPath;

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
			public void Activate(PointF initialPosition, bool isActivatedUponCreation)
			{
			}

			public bool Deactivate()
			{
				return false;
			}

			public void MoveGrip(PointF newPosition)
			{
			}

			public void Show(Graphics g)
			{
				g.DrawPath(Pens.Blue, _displayPath);
			}

			public bool IsGripHitted(PointF point)
			{
				return false;
			}



			#endregion
		}

		#endregion

		protected GraphicsPath _objectPath;
		protected GraphicsPath _selectionPath; // can be null, in this case the object path is used

		protected TransformationMatrix2D _matrix;
		protected object _hitobject;

		#region IHitTestObject Members

		public HitTestObject(GraphicsPath gp, object hitobject)
			: this(gp, null, hitobject)
		{
		}

		public HitTestObject(GraphicsPath gp, GraphicsPath selectionPath, object hitobject)
		{
			_objectPath = gp;
			_selectionPath = selectionPath;
			_hitobject = hitobject;
			_matrix = new TransformationMatrix2D();
		}


		/// <summary>
		/// This will return the transformation matrix. This matrix translates from coordinates of the object to global coordinates.
		/// </summary>
		public TransformationMatrix2D Transformation
		{
			get { return _matrix; }
		}


		public void Transform(TransformationMatrix2D x)
		{
			_matrix.PrependTransform(x);

			_objectPath.Transform(x);
			if (_selectionPath != null) _selectionPath.Transform(x);
		}


		public void Transform(Matrix x)
		{
			_matrix.PrependTransform(x);

			_objectPath.Transform(x);
			if (_selectionPath != null) _selectionPath.Transform(x);
		}


		public GraphicsPath SelectionPath
		{
			get
			{
				return _selectionPath != null ? _selectionPath : _objectPath;
			}
		}

		public GraphicsPath ObjectPath
		{
			get
			{
				return _objectPath;
			}
		}


		public object HittedObject
		{
			get { return _hitobject; }
			set { _hitobject = value; }
		}


		/// <summary>
		/// Shows the grips, i.e. the special areas for manipulation of the object.
		/// </summary>
		/// <param name="gripLevel">The grip level. For 0, only the translation grip is shown.</param>
		/// <returns>Grip manipulation handles that are used to show the grips and to manipulate the object.</returns>
		public virtual IGripManipulationHandle[] GetGrips(double pageScale, int gripLevel)
		{
			return new IGripManipulationHandle[] { new NoopGrip(_objectPath) };
		}

		public virtual int GetNextGripLevel(int currentGripLevel)
		{
			return currentGripLevel;
		}

		public virtual void ShiftPosition(float x, float y)
		{

			if (_hitobject is GraphicBase)
			{
				Matrix mat = new Matrix();
				mat.Translate(x, y);
				_objectPath.Transform(mat);
				if (null != _selectionPath) _selectionPath.Transform(mat);

				PointF pos = new PointF(x, y);
				_matrix.InverseTransformVector(pos);

				((GraphicBase)_hitobject).X += pos.X;
				((GraphicBase)_hitobject).Y += pos.Y;
			}
		}


		DoubleClickHandler _DoubleClick;
		public DoubleClickHandler DoubleClick
		{
			get { return _DoubleClick; }
			set { _DoubleClick = value; }
		}

		/// <summary>
		/// Handler to remove the hitted object. Should return true if the object is removed, otherwise false.
		/// </summary>
		DoubleClickHandler _Remove;
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

		XYPlotLayer _parentLayer;
		public XYPlotLayer ParentLayer
		{
			get { return _parentLayer; }
			set { _parentLayer = value; }
		}

		#endregion
	}
}
