#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2015 Dr. Dirk Lellinger
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

using Altaxo.Geometry;
using Altaxo.Graph3D.GraphicsContext;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Altaxo.Graph3D.Shapes
{
	/// <summary>
	/// The abstract base class for general graphical objects on the layer,
	/// for instance text elements, lines, pictures, rectangles and so on.
	/// </summary>
	[Serializable]
	public abstract partial class GraphicBase
		:
		Main.SuspendableDocumentNodeWithSingleAccumulatedData<EventArgs>,
		Altaxo.Graph3D.Shapes.IGraphicBase3D
	{
		/// <summary>
		/// The size of the parent object.
		/// </summary>
		protected PointD3D _cachedParentSize;

		/// <summary>
		/// The item's location (size, position, rotation, shear, scale ..)
		/// </summary>
		/// <remarks>The location is the vector from the reference point of the parent (normally the left upper corner of the parent) to the reference point of this object (normally
		/// also the left upper corner of the object).</remarks>
		protected ItemLocationDirect3D _location;

		/// <summary>Cached matrix which transforms from own coordinates to parent (layer) coordinates.</summary>
		protected MatrixD3D _transformation = MatrixD3D.Identity;

		#region Serialization

		// 2015-09-12 initial version
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(GraphicBase), 0)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				GraphicBase s = (GraphicBase)obj;
				info.AddValue("Location", s._location);
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				GraphicBase s = (GraphicBase)o;

				if (null != s._location) s._location.Dispose(); // because location probably is set already in the derived object
				s._location = (ItemLocationDirect3D)info.GetValue("Location", s);
				if (null != s._location) s._location.ParentObject = s;

				s.UpdateTransformationMatrix();

				return s;
			}
		}

		#endregion Serialization

		/// <summary>
		/// Initializes a fresh instance of this class with default values
		/// </summary>
		protected GraphicBase(ItemLocationDirect3D location)
		{
			if (null == location)
				throw new ArgumentNullException("location");

			_location = location;
			_location.ParentObject = this;
		}

		protected GraphicBase(GraphicBase from)
		{
			if (null == from)
				throw new ArgumentNullException("from");

			_location = from._location.Clone();
			_location.ParentObject = this;

			CopyFrom(from);
		}

		public virtual bool CopyFrom(object obj)
		{
			if (object.ReferenceEquals(this, obj))
				return true;
			var from = obj as GraphicBase;
			if (null == from)
				return false;

			using (var suspendToken = SuspendGetToken())
			{
				this._cachedParentSize = from._cachedParentSize;
				this._location.CopyFrom(from._location);
				bool wasUsed = (null != this._parent);
				//this._parent = from._parent;
				this.UpdateTransformationMatrix();

				if (wasUsed)
				{
					_accumulatedEventData = EventArgs.Empty;
					suspendToken.Resume();
				}
				else
				{
					suspendToken.ResumeSilently();
				}
			}

			return true;
		}

		protected override IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
		{
			if (null != _location)
				yield return new Main.DocumentNodeAndName(_location, () => _location = null, "Location");
		}

		#region Suspend/Resume

		protected override void AccumulateChangeData(object sender, EventArgs e)
		{
			_accumulatedEventData = EventArgs.Empty;
		}

		protected override void OnChanged(EventArgs e)
		{
			UpdateTransformationMatrix();

			base.OnChanged(e);
		}

		#endregion Suspend/Resume

		public void SetParentSize(VectorD3D parentSize, bool shouldTriggerChangeEvent)
		{
			var oldParentSize = _location.ParentSize;
			_location.SetParentSize(parentSize, false); // do not trigger change event here

			if (oldParentSize != parentSize)
			{
				UpdateTransformationMatrix(); // update the matrix in every case

				if (shouldTriggerChangeEvent)
					EhSelfChanged(EventArgs.Empty);
			}
		}

		public VectorD3D ParentSize
		{
			get
			{
				return _location.ParentSize;
			}
		}

		public virtual bool AutoSize
		{
			get
			{
				return false;
			}
		}

		public ItemLocationDirect3D Location
		{
			get
			{
				return _location;
			}
		}

		/// <summary>
		/// Get/sets the x position of the reference point of the object in layer coordinates.
		/// </summary>
		public virtual double X
		{
			get
			{
				return _location.AbsolutePositionX;
			}
			set
			{
				_location.AbsolutePositionX = value;
			}
		}

		/// <summary>
		/// Get/sets the y position of the reference point of the object in layer coordinates.
		/// </summary>
		public virtual double Y
		{
			get
			{
				return _location.AbsolutePositionY;
			}
			set
			{
				_location.AbsolutePositionY = value;
			}
		}

		/// <summary>
		/// Get/sets the z position of the reference point of the object in layer coordinates.
		/// </summary>
		public virtual double Z
		{
			get
			{
				return _location.AbsolutePositionZ;
			}
			set
			{
				_location.AbsolutePositionZ = value;
			}
		}

		/// <summary>
		/// Gets the bound of the object. The X and Y positions depend on the transformation model chosen for this graphic object: if the transformation takes into account the local anchor point,
		/// then the X and Y of the bounds are always 0. If the transformation does not take the local anchor point into account, then (X and Y) is the vector from the local anchor point to the
		/// upper left corner of the graphical object.
		/// </summary>
		/// <value>
		/// The bounds of the graphical object.
		/// </value>
		public virtual RectangleD3D Bounds
		{
			get
			{
				return new RectangleD3D((PointD3D)_location.AbsoluteVectorPivotToLeftUpper, Size);
			}
		}

		/// <summary>
		/// Returns the information if this object allows negative sizes.
		/// </summary>
		public virtual bool AllowNegativeSize
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// Returns the position of the pivot point of the object in parent coordinates (strictly speaking: as vector from the parent's reference point to the pivot point of the object).
		/// </summary>
		/// <returns>The position of the object (the object's pivot point) with reference to the parent's reference point).</returns>
		protected virtual PointD3D GetPosition()
		{
			return _location.AbsolutePivotPosition;
		}

		/// <summary>
		/// Sets the position of the object's pivot point.
		/// </summary>
		/// <param name="value">The position to set (the object's pivot point) with reference to the parent's reference point).</param>
		/// <param name="eventFiring">Designates whether or not the change event should be fired if the value has changed.</param>
		protected virtual void SetPosition(PointD3D value, Main.EventFiring eventFiring)
		{
			_location.SetAbsolutePivotPosition(value, eventFiring);
			if (eventFiring == Main.EventFiring.Suppressed)
				UpdateTransformationMatrix(); // even if events are suppressed - update at least the transformation matrix
		}

		/// <summary>
		/// Sets the position of the object without causing a Changed event.
		/// </summary>
		/// <param name="newPosition"></param>
		public virtual void SilentSetPosition(PointD3D newPosition)
		{
			this.SetPosition(newPosition, Main.EventFiring.Suppressed);
		}

		/// <summary>
		/// Get/set the position of the object. This is defined as the vector from the parent's reference point to the object's pivot point.
		/// </summary>
		public PointD3D Position
		{
			get
			{
				return GetPosition();
			}
			set
			{
				SetPosition(value, Main.EventFiring.Enabled);
			}
		}

		/// <summary>
		/// Scales the position of an item according to the provided xscale and yscale. Can be called with null for the item (in this case nothing happens).
		/// </summary>
		/// <param name="o">The graphics object whose position is scaled.</param>
		/// <param name="xscale">The xscale ratio.</param>
		/// <param name="yscale">The yscale ratio.</param>
		public static void ScalePosition(IGraphicBase3D o, double xscale, double yscale, double zscale)
		{
			if (o != null)
			{
				var oldP = o.Position;
				o.Position = new PointD3D((oldP.X * xscale), (oldP.Y * yscale), (oldP.Z * zscale));
			}
		}

		/// <summary>
		/// Get/sets the width of the item. This is the unscaled width.
		/// </summary>
		public virtual double SizeX
		{
			get
			{
				return _location.AbsoluteSizeX;
			}
			set
			{
				_location.AbsoluteSizeX = value;
			}
		}

		/// <summary>
		/// Gets/sets the height of the item. This is the unscaled height.
		/// </summary>
		public virtual double SizeY
		{
			get
			{
				return _location.AbsoluteSizeY;
			}
			set
			{
				_location.AbsoluteSizeY = value;
			}
		}

		/// <summary>
		/// Gets/sets the height of the item. This is the unscaled height.
		/// </summary>
		public virtual double SizeZ
		{
			get
			{
				return _location.AbsoluteSizeZ;
			}
			set
			{
				_location.AbsoluteSizeZ = value;
			}
		}

		/// <summary>
		/// Sets the size of the item.
		/// </summary>
		/// <param name="sizeX">Unscaled width of the item.</param>
		/// <param name="sizeY">Unscaled height of the item.</param>
		/// <param name="eventFiring">Designates whether the change event should be fired.</param>
		protected virtual void SetSize(double sizeX, double sizeY, double sizeZ, Main.EventFiring eventFiring)
		{
			_location.SetAbsoluteSize(new VectorD3D(sizeX, sizeY, sizeZ), eventFiring);
		}

		/// <summary>
		/// Sets the size and position of this item in relative units, calculated from absolute values. Note that the ParentSize must be set prior to calling this function.
		/// </summary>
		/// <param name="absSize">Absolute size of the item.</param>
		/// <param name="absPos">Absolute position of the item.</param>
		public virtual void SetRelativeSizePositionFromAbsoluteValues(VectorD3D absSize, PointD3D absPos)
		{
			_location.SetRelativeSizePositionFromAbsoluteValues(absSize, absPos);
		}

		/// <summary>
		/// Get/set the unscaled size of the item.
		/// </summary>
		public VectorD3D Size
		{
			get
			{
				return _location.AbsoluteSize;
			}
			set
			{
				_location.SetAbsoluteSize(value, Main.EventFiring.Enabled);
			}
		}

		/// <summary>
		/// Get/sets the rotation x value, measured in degrees in counterclockwise direction.
		/// </summary>
		public virtual double RotationX
		{
			get
			{
				return _location.RotationX;
			}
			set
			{
				_location.RotationX = value;
			}
		}

		/// <summary>
		/// Get/sets the rotation y value, measured in degrees in counterclockwise direction.
		/// </summary>
		public virtual double RotationY
		{
			get
			{
				return _location.RotationY;
			}
			set
			{
				_location.RotationY = value;
			}
		}

		/// <summary>
		/// Get/sets the rotation z value, measured in degrees in counterclockwise direction.
		/// </summary>
		public virtual double RotationZ
		{
			get
			{
				return _location.RotationZ;
			}
			set
			{
				_location.RotationZ = value;
			}
		}

		/// <summary>
		/// Get/sets the scale for the width of the item. Normally this number is one (1).
		/// </summary>
		public double ScaleX
		{
			get
			{
				return _location.ScaleX;
			}
		}

		/// <summary>
		/// Get/sets the scale for the height of the item. Normally this number is one (1).
		/// </summary>
		public virtual double ScaleY
		{
			get
			{
				return _location.ScaleY;
			}
		}

		/// <summary>
		/// Get/sets the scale for the z dimension of the item. Normally this number is one (1).
		/// </summary>
		public virtual double ScaleZ
		{
			get
			{
				return _location.ScaleZ;
			}
		}

		public virtual VectorD3D Scale
		{
			get
			{
				return _location.Scale;
			}
			set
			{
				_location.Scale = value;
			}
		}

		/// <summary>
		/// Get/sets the shear of the item. This is the factor, by which the item points are shifted in x direction, when doing a unit step in y direction.
		/// The shear is the tangents of the shear angle.
		/// </summary>
		public virtual double ShearX
		{
			get
			{
				return _location.ShearX;
			}
			set
			{
				_location.ShearX = value;
			}
		}

		/// <summary>
		/// Get/sets the shear of the item. This is the factor, by which the item points are shifted in x direction, when doing a unit step in y direction.
		/// The shear is the tangents of the shear angle.
		/// </summary>
		public virtual double ShearY
		{
			get
			{
				return _location.ShearY;
			}
			set
			{
				_location.ShearY = value;
			}
		}

		/// <summary>
		/// Get/sets the shear of the item. This is the factor, by which the item points are shifted in x direction, when doing a unit step in y direction.
		/// The shear is the tangents of the shear angle.
		/// </summary>
		public virtual double ShearZ
		{
			get
			{
				return _location.ShearZ;
			}
			set
			{
				_location.ShearZ = value;
			}
		}

		/// <summary>
		/// Transforms the graphics context is such a way, that the object can be drawn in local coordinates.
		/// </summary>
		/// <param name="g">Graphics context (should be saved beforehand).</param>
		protected virtual void TransformGraphics(IGraphicContext3D g)
		{
			if (RotationX != 0 || RotationY != 0 || RotationZ != 0 || ScaleX != 1 || ScaleY != 1 || ScaleZ != 1 || ShearX != 0 || ShearY != 0 || ShearZ != 0)
			{
				g.PrependTransform(MatrixD3D.FromTranslationRotationShearScale(
					_location.AbsolutePivotPosition.X, _location.AbsolutePivotPositionY, _location.AbsolutePivotPositionZ,
					_location.RotationX, _location.RotationY, _location.RotationZ,
					_location.ShearX, _location.ShearY, _location.ShearZ,
					_location.ScaleX, _location.ScaleY, _location.ScaleY
					));
			}
			else
			{
				var p = _location.AbsolutePivotPosition;
				g.TranslateTransform(p.X, p.Y, p.Z);
			}
		}

		/// <summary>
		/// Updates the internal transformation matrix to reflect the settings for position, rotation, scaleX, scaleY and shear. It is designed here by default so that
		/// the local anchor point of the object is located at the world coordinates (0,0). The transformation matrix update can be overridden in derived classes so
		/// that for instance the left upper corner of the object is located at (0,0).
		/// </summary>
		protected virtual void UpdateTransformationMatrix()
		{
			_transformation.SetTranslationRotationShearScale(
				_location.AbsolutePivotPositionX, _location.AbsolutePivotPositionY, _location.AbsolutePivotPositionZ,
				RotationX, RotationY, RotationZ,
				ShearX, ShearY, ShearZ,
				ScaleX, ScaleY, ScaleZ);
		}

		/// <summary>
		/// Determines whether this graphical object is compatible with the parent specified in the argument.
		/// </summary>
		/// <param name="parentObject">The parent object.</param>
		/// <returns>
		///   <c>True</c> if this object is compatible with the parent object; otherwise <c>false</c>.
		/// </returns>
		public virtual bool IsCompatibleWithParent(object parentObject)
		{
			return true;
		}

		public virtual void FixupInternalDataStructures()
		{
		}

		/// <summary>
		/// Is called before the paint procedure is executed.
		/// </summary>
		/// <param name="context">The paint context.</param>
		public virtual void PaintPreprocessing(Altaxo.Graph.IPaintContext context)
		{
		}

		/// <summary>
		/// Paint the object in the graphics context.
		/// </summary>
		/// <param name="g">Graphics context.</param>
		/// <param name="context">Additional information used to draw the object.</param>
		public abstract void Paint(IGraphicContext3D g, Altaxo.Graph.IPaintContext context);

		/// <summary>
		/// Creates a cloned copy of this object.
		/// </summary>
		/// <returns>The cloned copy of this object.</returns>
		public abstract object Clone();

		public abstract IHitTestObject HitTest(HitTestPointData hitData);
	}
}