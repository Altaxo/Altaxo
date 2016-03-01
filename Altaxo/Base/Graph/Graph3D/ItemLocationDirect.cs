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

using Altaxo.Calc;
using Altaxo.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Graph.Graph3D
{
	[Serializable]
	public class ItemLocationDirect : Main.SuspendableDocumentLeafNodeWithEventArgs, IItemLocation
	{
		#region Members

		protected RADouble _sizeX;

		protected RADouble _sizeY;

		protected RADouble _sizeZ;

		protected RADouble _positionX;

		protected RADouble _positionY;

		protected RADouble _positionZ;

		protected RADouble _localAnchorX;

		protected RADouble _localAnchorY;

		protected RADouble _localAnchorZ;

		protected RADouble _parentAnchorX;

		protected RADouble _parentAnchorY;

		protected RADouble _parentAnchorZ;

		/// <summary>The rotation angle (in degrees) of the layer.</summary>
		protected double _rotationX; // Rotation around x axis

		/// <summary>The rotation angle (in degrees) of the layer.</summary>
		protected double _rotationY; // Rotation around z axis

		/// <summary>The rotation angle (in degrees) of the layer.</summary>
		protected double _rotationZ; // Rotation around z axis

		protected double _shearX; // Shear in the y-z plane

		protected double _shearY; // Shear in the y-z plane

		protected double _shearZ; // Shear in the x-y plane

		/// <summary>The scaling factor of the layer, normally 1.</summary>
		protected double _scaleX;  // X-Scale

		protected double _scaleY; // Y-Scale

		protected double _scaleZ; // Y-Scale

		// Cached and not-to-serialize members

		protected VectorD3D _parentSize;

		#endregion Members

		#region Serialization

		/// <summary>
		/// 2015-11-14 initial version.
		/// </summary>
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ItemLocationDirect), 0)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				var s = (ItemLocationDirect)obj;

				info.AddValue("ParentSize", s._parentSize);

				info.AddValue("SizeX", s._sizeX);
				info.AddValue("SizeY", s._sizeY);
				info.AddValue("SizeZ", s._sizeZ);

				info.AddValue("PositionX", s._positionX);
				info.AddValue("PositionY", s._positionY);
				info.AddValue("PositionZ", s._positionZ);

				info.AddValue("LocalAnchorX", s._localAnchorX);
				info.AddValue("LocalAnchorY", s._localAnchorY);
				info.AddValue("LocalAnchorZ", s._localAnchorZ);

				info.AddValue("ParentAnchorX", s._parentAnchorX);
				info.AddValue("ParentAnchorY", s._parentAnchorY);
				info.AddValue("ParentAnchorZ", s._parentAnchorZ);

				info.AddValue("RotationX", s._rotationX);
				info.AddValue("RotationY", s._rotationY);
				info.AddValue("RotationZ", s._rotationZ);

				info.AddValue("ShearX", s._shearX);
				info.AddValue("ShearY", s._shearY);
				info.AddValue("ShearZ", s._shearZ);

				info.AddValue("ScaleX", s._scaleX);
				info.AddValue("ScaleY", s._scaleY);
				info.AddValue("ScaleZ", s._scaleZ);
			}

			protected virtual ItemLocationDirect SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				var s = null != o ? (ItemLocationDirect)o : new ItemLocationDirect();

				s._parentSize = (VectorD3D)info.GetValue("ParentSize", s);

				s._sizeX = (RADouble)info.GetValue("SizeX", s);
				s._sizeY = (RADouble)info.GetValue("SizeY", s);
				s._sizeZ = (RADouble)info.GetValue("SizeZ", s);

				s._positionX = (RADouble)info.GetValue("PositionX", s);
				s._positionY = (RADouble)info.GetValue("PositionY", s);
				s._positionZ = (RADouble)info.GetValue("PositionZ", s);

				s._localAnchorX = (RADouble)info.GetValue("LocalAnchorX", s);
				s._localAnchorY = (RADouble)info.GetValue("LocalAnchorY", s);
				s._localAnchorZ = (RADouble)info.GetValue("LocalAnchorZ", s);

				s._parentAnchorX = (RADouble)info.GetValue("ParentAnchorX", s);
				s._parentAnchorY = (RADouble)info.GetValue("ParentAnchorY", s);
				s._parentAnchorZ = (RADouble)info.GetValue("ParentAnchorZ", s);

				s._rotationX = info.GetDouble("RotationX");
				s._rotationY = info.GetDouble("RotationY");
				s._rotationZ = info.GetDouble("RotationZ");

				s._shearX = info.GetDouble("ShearX");
				s._shearY = info.GetDouble("ShearY");
				s._shearZ = info.GetDouble("ShearZ");

				s._scaleX = info.GetDouble("ScaleX");
				s._scaleY = info.GetDouble("ScaleY");
				s._scaleZ = info.GetDouble("ScaleZ");

				return s;
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				ItemLocationDirect s = SDeserialize(o, info, parent);
				return s;
			}
		}

		#endregion Serialization

		#region Construction and copying

		public ItemLocationDirect()
		{
			_localAnchorX = RADouble.NewRel(0);
			_localAnchorY = RADouble.NewRel(0);
			_localAnchorZ = RADouble.NewRel(0);
			_parentAnchorX = RADouble.NewRel(0);
			_parentAnchorY = RADouble.NewRel(0);
			_parentAnchorZ = RADouble.NewRel(0);
			_scaleX = 1;
			_scaleY = 1;
			_scaleZ = 1;
		}

		public ItemLocationDirect(ItemLocationDirect from)
		{
			CopyFrom(from);
		}

		public ItemLocationDirect(IItemLocation from)
		{
			CopyFrom(from);
		}

		public virtual bool CopyFrom(object obj)
		{
			if (object.ReferenceEquals(this, obj))
				return true;

			if (obj is ItemLocationDirect)
			{
				var from = (ItemLocationDirect)obj;
				this._parentSize = from._parentSize;

				this._positionX = from._positionX;
				this._positionY = from._positionY;
				this._positionZ = from._positionZ;

				this._sizeX = from._sizeX;
				this._sizeY = from._sizeY;
				this._sizeZ = from._sizeZ;

				this._localAnchorX = from._localAnchorX;
				this._localAnchorY = from._localAnchorY;
				this._localAnchorZ = from._localAnchorZ;

				this._parentAnchorX = from._parentAnchorX;
				this._parentAnchorY = from._parentAnchorY;
				this._parentAnchorZ = from._parentAnchorZ;

				this._rotationX = from._rotationX;
				this._rotationY = from._rotationY;
				this._rotationZ = from._rotationZ;

				this._scaleX = from._scaleX;
				this._scaleY = from._scaleY;
				this._scaleZ = from._scaleZ;

				this._shearX = from._shearX;
				this._shearY = from._shearY;
				this._shearZ = from._shearZ;

				EhSelfChanged();
				return true;
			}
			else if (obj is IItemLocation)
			{
				var from = (IItemLocation)obj;
				this._rotationX = from.RotationX;
				this._rotationY = from.RotationY;
				this._rotationZ = from.RotationZ;
				this._shearX = from.ShearX;
				this._shearY = from.ShearY;
				this._shearZ = from.ShearZ;
				this._scaleX = from.ScaleX;
				this._scaleY = from.ScaleY;
				this._scaleY = from.ScaleZ;
				EhSelfChanged();
				return true;
			}

			return false;
		}

		object System.ICloneable.Clone()
		{
			return new ItemLocationDirect(this);
		}

		public virtual ItemLocationDirect Clone()
		{
			return new ItemLocationDirect(this);
		}

		#endregion Construction and copying

		#region Properties

		public virtual void SetParentSize(VectorD3D parentSize, bool shouldTriggerChangedEvent)
		{
			var oldValue = _parentSize;
			_parentSize = parentSize;

			if (shouldTriggerChangedEvent && oldValue != _parentSize)
				EhSelfChanged();
		}

		public VectorD3D ParentSize
		{
			get
			{
				return _parentSize;
			}
		}

		/// <summary>
		/// Gets a value indicating whether the this location belongs to a graphical element which is auto sized.
		/// </summary>
		/// <value>
		///   <c>true</c> if [can set size]; otherwise, <c>false</c>.
		/// </value>
		public virtual bool IsAutoSized
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// The width of the item, either as absolute value in point (1/72 inch), or as
		/// value relative to the parent's width.
		/// </summary>
		public virtual RADouble SizeX
		{
			get { return _sizeX; }
			set
			{
				var chg = _sizeX != value;
				InternalSetSizeXSilent(value);

				if (chg)
					EhSelfChanged();
			}
		}

		/// <summary>
		/// The height of the item, either as absolute value in point (1/72 inch), or as
		/// value relative to the parent's height.
		/// </summary>
		public virtual RADouble SizeY
		{
			get { return _sizeY; }
			set
			{
				var chg = _sizeY != value;
				InternalSetSizeYSilent(value);

				if (chg)
					EhSelfChanged();
			}
		}

		/// <summary>
		/// The z size of the item, either as absolute value in point (1/72 inch), or as
		/// value relative to the parent's height.
		/// </summary>
		public virtual RADouble SizeZ
		{
			get { return _sizeZ; }
			set
			{
				var chg = _sizeZ != value;
				InternalSetSizeZSilent(value);

				if (chg)
					EhSelfChanged();
			}
		}

		/// <summary>
		/// The x position of the item, either absolute in points or relative to the parent's width.
		/// </summary>
		public RADouble PositionX
		{
			get { return _positionX; }
			set
			{
				var oldvalue = _positionX;
				_positionX = value;

				if (value != oldvalue)
					EhSelfChanged();
			}
		}

		/// <summary>
		/// The y position of the item, either absolute or relative to the parent's height.
		/// </summary>
		public RADouble PositionY
		{
			get { return _positionY; }
			set
			{
				var oldvalue = _positionY;
				_positionY = value;

				if (value != oldvalue)
					EhSelfChanged();
			}
		}

		/// <summary>
		/// The z position of the item, either absolute or relative to the parent's height.
		/// </summary>
		public RADouble PositionZ
		{
			get { return _positionZ; }
			set
			{
				var oldvalue = _positionZ;
				_positionZ = value;

				if (value != oldvalue)
					EhSelfChanged();
			}
		}

		/// <summary>
		/// Gets or sets the local anchor's x position. This is an absolute value (in points = 1/72 inch) or a value relative to the own item width.
		/// The local anchor point is the point, for which the location can be set in the position/size dialog of the item.
		/// A relative value of 0 designates the left boundary of the item, a relative value of 0.5 designates the horizontal center of the item, and a relative value of 1 designates the right boundary of the item.
		/// </summary>
		/// <value>
		/// The local anchor's x position.
		/// </value>
		public RADouble LocalAnchorX
		{
			get { return _localAnchorX; }
			set
			{
				var oldvalue = _localAnchorX;
				_localAnchorX = value;

				if (value != oldvalue)
					EhSelfChanged();
			}
		}

		/// <summary>
		/// Gets or sets the local anchor's y position. This is an absolute value (in points = 1/72 inch) or a value relative to the own item height.
		/// The local anchor point is the point, for which the location can be set in the position/size dialog of the item.
		/// A relative value of 0 designates the upper boundary of the item, a relative value of 0.5 designates the vertical center of the item, and a relative value of 1 designates the lower boundary of the item.
		/// </summary>
		/// <value>
		/// The local anchor's x position.
		/// </value>
		public RADouble LocalAnchorY
		{
			get { return _localAnchorY; }
			set
			{
				var oldvalue = _localAnchorY;
				_localAnchorY = value;

				if (value != oldvalue)
					EhSelfChanged();
			}
		}

		/// <summary>
		/// Gets or sets the local anchor's z position. This is an absolute value (in points = 1/72 inch) or a value relative to the own item height.
		/// The local anchor point is the point, for which the location can be set in the position/size dialog of the item.
		/// A relative value of 0 designates the upper boundary of the item, a relative value of 0.5 designates the vertical center of the item, and a relative value of 1 designates the lower boundary of the item.
		/// </summary>
		/// <value>
		/// The local anchor's z position.
		/// </value>
		public RADouble LocalAnchorZ
		{
			get { return _localAnchorZ; }
			set
			{
				var oldvalue = _localAnchorZ;
				_localAnchorZ = value;

				if (value != oldvalue)
					EhSelfChanged();
			}
		}

		/// <summary>
		/// Gets or sets the parent anchor's x position. This is an absolute value (in points = 1/72 inch) or a value relative to the parent's width.
		/// The parent anchor point is the point inside the parent item, from which the location of the item is measured. Stricly speaking, the position of the item (as shown in the dialog) is the vector from
		/// the parent's anchor point to the local anchor point).
		/// A relative value of 0 designates the left boundary of the parent, a relative value of 0.5 designates the horizontal center of the parent, and a relative value of 1 designates the right boundary of the parent.
		/// </summary>
		/// <value>
		/// The parent anchor's x position.
		/// </value>
		public RADouble ParentAnchorX
		{
			get { return _parentAnchorX; }
			set
			{
				var oldvalue = _parentAnchorX;
				_parentAnchorX = value;

				if (value != oldvalue)
					EhSelfChanged();
			}
		}

		/// <summary>
		/// Gets or sets the parent anchor's y position. This is an absolute value (in points = 1/72 inch) or a value relative to the parent's height.
		/// The parent anchor point is the point inside the parent item, from which the location of the item is measured. Stricly speaking, the position of the item (as shown in the dialog) is the vector from
		/// the parent's anchor point to the local anchor point).
		/// A relative value of 0 designates the upper boundary of the parent, a relative value of 0.5 designates the vertical center of the parent, and a relative value of 1 designates the lower boundary of the parent.
		/// </summary>
		/// <value>
		/// The parent anchor's y position.
		/// </value>
		public RADouble ParentAnchorY
		{
			get { return _parentAnchorY; }
			set
			{
				var oldvalue = _parentAnchorY;
				_parentAnchorY = value;

				if (value != oldvalue)
					EhSelfChanged();
			}
		}

		/// <summary>
		/// Gets or sets the parent anchor's z position. This is an absolute value (in points = 1/72 inch) or a value relative to the parent's height.
		/// The parent anchor point is the point inside the parent item, from which the location of the item is measured. Stricly speaking, the position of the item (as shown in the dialog) is the vector from
		/// the parent's anchor point to the local anchor point).
		/// A relative value of 0 designates the upper boundary of the parent, a relative value of 0.5 designates the vertical center of the parent, and a relative value of 1 designates the lower boundary of the parent.
		/// </summary>
		/// <value>
		/// The parent anchor's z position.
		/// </value>
		public RADouble ParentAnchorZ
		{
			get { return _parentAnchorZ; }
			set
			{
				var oldvalue = _parentAnchorZ;
				_parentAnchorZ = value;

				if (value != oldvalue)
					EhSelfChanged();
			}
		}

		public virtual void SetPositionAndSize(RADouble x, RADouble y, RADouble z, RADouble size_x, RADouble size_y, RADouble size_z)
		{
			bool isChanged = x != _positionX || y != _positionY || size_x != _sizeX || size_y != _sizeY;

			_positionX = x;
			_positionY = y;
			_positionZ = z;

			InternalSetSizeSilent(size_x, size_y, size_z);

			if (isChanged)
				EhSelfChanged();
		}

		protected virtual bool InternalSetScaleXSilent(double value)
		{
			bool chg = _scaleX != value;
			_scaleX = value;
			return chg;
		}

		protected virtual bool InternalSetScaleYSilent(double value)
		{
			bool chg = _scaleY != value;
			_scaleY = value;
			return chg;
		}

		protected virtual bool InternalSetScaleZSilent(double value)
		{
			bool chg = _scaleZ != value;
			_scaleZ = value;
			return chg;
		}

		protected virtual bool InternalSetScaleSilent(VectorD3D value)
		{
			bool chg = _scaleX != value.X || _scaleY != value.Y || _scaleZ != value.Z;
			_scaleX = value.X;
			_scaleY = value.Y;
			_scaleZ = value.Z;
			return chg;
		}

		/// <summary>The scaling factor of the item, normally 1.</summary>
		public VectorD3D Scale
		{
			get
			{
				return new VectorD3D(_scaleX, _scaleY, _scaleZ);
			}
			set
			{
				bool isChanged = InternalSetScaleSilent(value);
				if (isChanged)
					EhSelfChanged();
			}
		}

		/// <summary>The scaling factor of the item in x-direction, normally 1.</summary>
		public double ScaleX
		{
			get { return _scaleX; }
			set
			{
				if (InternalSetScaleXSilent(value))
					EhSelfChanged();
			}
		}

		/// <summary>The scaling factor of the item in y-direction, normally 1.</summary>
		public double ScaleY
		{
			get { return _scaleY; }
			set
			{
				if (InternalSetScaleYSilent(value))
					EhSelfChanged();
			}
		}

		/// <summary>The scaling factor of the item in z-direction, normally 1.</summary>
		public double ScaleZ
		{
			get { return _scaleZ; }
			set
			{
				if (InternalSetScaleZSilent(value))
					EhSelfChanged();
			}
		}

		/// <summary>The rotation angle (in degrees) of the layer.</summary>
		public double RotationX
		{
			get { return _rotationX; }
			set
			{
				double oldvalue = _rotationX;
				_rotationX = value;

				if (value != oldvalue)
					EhSelfChanged();
			}
		}

		/// <summary>The rotation angle (in degrees) of the layer.</summary>
		public double RotationY
		{
			get { return _rotationY; }
			set
			{
				double oldvalue = _rotationY;
				_rotationY = value;

				if (value != oldvalue)
					EhSelfChanged();
			}
		}

		/// <summary>The rotation angle (in degrees) of the layer.</summary>
		public double RotationZ
		{
			get { return _rotationZ; }
			set
			{
				double oldvalue = _rotationZ;
				_rotationZ = value;

				if (value != oldvalue)
					EhSelfChanged();
			}
		}

		/// <summary>The shear factor of the item.</summary>
		public double ShearX
		{
			get { return _shearX; }
			set
			{
				double oldvalue = _shearX;
				_shearX = value;
				if (value != oldvalue)
					EhSelfChanged();
			}
		}

		/// <summary>The shear factor of the item.</summary>
		public double ShearY
		{
			get { return _shearY; }
			set
			{
				double oldvalue = _shearY;
				_shearY = value;
				if (value != oldvalue)
					EhSelfChanged();
			}
		}

		/// <summary>The shear factor of the item.</summary>
		public double ShearZ
		{
			get { return _shearZ; }
			set
			{
				double oldvalue = _shearZ;
				_shearZ = value;
				if (value != oldvalue)
					EhSelfChanged();
			}
		}

		#endregion Properties

		#region Methods

		/// <summary>
		/// Gets the absolute enclosing rectangle without taking into account ScaleX, ScaleY, Rotation and Shear (SSRS).
		/// </summary>
		/// <returns>The enclosing rectangle in absolute values.</returns>
		public RectangleD3D GetAbsoluteEnclosingRectangleWithoutSSRS()
		{
			var mySizeX = _sizeX.GetValueRelativeTo(_parentSize.X);
			var mySizeY = _sizeY.GetValueRelativeTo(_parentSize.Y);
			var mySizeZ = _sizeZ.GetValueRelativeTo(_parentSize.Z);

			var myPosX = _parentAnchorX.GetValueRelativeTo(_parentSize.X) + _positionX.GetValueRelativeTo(_parentSize.X) - _localAnchorX.GetValueRelativeTo(mySizeX);
			var myPosY = _parentAnchorY.GetValueRelativeTo(_parentSize.Y) + _positionY.GetValueRelativeTo(_parentSize.Y) - _localAnchorY.GetValueRelativeTo(mySizeY);
			var myPosZ = _parentAnchorZ.GetValueRelativeTo(_parentSize.Z) + _positionZ.GetValueRelativeTo(_parentSize.Z) - _localAnchorZ.GetValueRelativeTo(mySizeZ);

			return new RectangleD3D(myPosX, myPosY, myPosZ, mySizeX, mySizeY, mySizeZ);
		}

		/// <summary>
		/// Gets the absolute enclosing rectangle, taking into account ScaleX, ScaleY, Rotation and Shear (SSRS).
		/// </summary>
		/// <returns>The enclosing rectangle in absolute values.</returns>
		public RectangleD3D GetAbsoluteEnclosingRectangle()
		{
			Matrix4x3 m = Matrix4x3.NewScalingShearingRotationDegreesTranslation(
				ScaleX, ScaleY, ScaleZ,
				ShearX, ShearY, ShearZ,
				-RotationX, -RotationY, -RotationZ,
				AbsolutePivotPositionX, AbsolutePivotPositionY, AbsolutePivotPositionZ);

			m.TranslatePrepend(AbsoluteVectorPivotToLeftUpper.X, AbsoluteVectorPivotToLeftUpper.Y, AbsoluteVectorPivotToLeftUpper.Z);

			var r = new RectangleD3D(PointD3D.Empty, this.AbsoluteSize);
			return RectangleD3D.NewRectangleIncludingAllPoints(r.Vertices.Select(p => m.Transform(p)));
		}

		protected virtual void InternalSetSizeXSilent(RADouble value)
		{
			_sizeX = value;
		}

		protected virtual void InternalSetSizeYSilent(RADouble value)
		{
			_sizeY = value;
		}

		protected virtual void InternalSetSizeZSilent(RADouble value)
		{
			_sizeZ = value;
		}

		protected virtual void InternalSetSizeSilent(RADouble valueX, RADouble valueY, RADouble valueZ)
		{
			_sizeX = valueX;
			_sizeY = valueY;
			_sizeZ = valueZ;
		}

		protected virtual void InternalSetAbsoluteSizeXSilent(double value)
		{
			if (_sizeX.IsAbsolute)
				InternalSetSizeXSilent(RADouble.NewAbs(value));
			else if (_parentSize.X != 0 && !double.IsNaN(_parentSize.X))
				InternalSetSizeXSilent(RADouble.NewRel(value / _parentSize.X));
			else
				throw new InvalidOperationException("_parentSize.X is undefined or zero");
		}

		protected virtual void InternalSetAbsoluteSizeYSilent(double value)
		{
			if (_sizeY.IsAbsolute)
				InternalSetSizeYSilent(RADouble.NewAbs(value));
			else if (_parentSize.Y != 0 && !double.IsNaN(_parentSize.Y))
				InternalSetSizeYSilent(RADouble.NewRel(value / _parentSize.Y));
			else
				throw new InvalidOperationException("_parentSize.Y is undefined or zero");
		}

		protected virtual void InternalSetAbsoluteSizeZSilent(double value)
		{
			if (_sizeZ.IsAbsolute)
				InternalSetSizeZSilent(RADouble.NewAbs(value));
			else if (_parentSize.Z != 0 && !double.IsNaN(_parentSize.Z))
				InternalSetSizeYSilent(RADouble.NewRel(value / _parentSize.Z));
			else
				throw new InvalidOperationException("_parentSize.Z is undefined or zero");
		}

		protected virtual void InternalSetAbsoluteSizeSilent(VectorD3D value)
		{
			RADouble sizeX, sizeY, sizeZ;

			if (_sizeX.IsAbsolute)
				sizeX = RADouble.NewAbs(value.X);
			else if (_parentSize.X != 0 && !double.IsNaN(_parentSize.X))
				sizeX = RADouble.NewRel(value.X / _parentSize.X);
			else
				throw new InvalidOperationException("_parentSize.X is undefined or zero");

			if (_sizeY.IsAbsolute)
				sizeY = RADouble.NewAbs(value.Y);
			else if (_parentSize.Y != 0 && !double.IsNaN(_parentSize.Y))
				sizeY = RADouble.NewRel(value.Y / _parentSize.Y);
			else
				throw new InvalidOperationException("_parentSize.Y is undefined or zero");

			if (_sizeZ.IsAbsolute)
				sizeZ = RADouble.NewAbs(value.Z);
			else if (_parentSize.Z != 0 && !double.IsNaN(_parentSize.Z))
				sizeZ = RADouble.NewRel(value.Z / _parentSize.Z);
			else
				throw new InvalidOperationException("_parentSize.Z is undefined or zero");

			InternalSetSizeSilent(sizeX, sizeY, sizeZ);
		}

		public virtual double AbsoluteSizeX
		{
			get
			{
				return _sizeX.GetValueRelativeTo(_parentSize.X);
			}
			set
			{
				var oldValue = _sizeX;
				InternalSetAbsoluteSizeXSilent(value);
				if (oldValue != _sizeX)
					EhSelfChanged();
			}
		}

		public virtual double AbsoluteSizeY
		{
			get
			{
				return _sizeY.GetValueRelativeTo(_parentSize.Y);
			}
			set
			{
				var oldValue = _sizeY;
				InternalSetAbsoluteSizeYSilent(value);
				if (oldValue != _sizeY)
					EhSelfChanged();
			}
		}

		public virtual double AbsoluteSizeZ
		{
			get
			{
				return _sizeZ.GetValueRelativeTo(_parentSize.Z);
			}
			set
			{
				var oldValue = _sizeZ;
				InternalSetAbsoluteSizeZSilent(value);
				if (oldValue != _sizeZ)
					EhSelfChanged();
			}
		}

		public virtual VectorD3D AbsoluteSize
		{
			get
			{
				return new VectorD3D(AbsoluteSizeX, AbsoluteSizeY, AbsoluteSizeZ);
			}
			set
			{
				SetAbsoluteSize(value, Main.EventFiring.Enabled);
			}
		}

		public virtual void SetAbsoluteSize(VectorD3D value, Main.EventFiring eventFiring)
		{
			var oldSizeX = _sizeX;
			var oldSizeY = _sizeY;
			var oldSizeZ = _sizeZ;
			InternalSetAbsoluteSizeSilent(value);

			if (eventFiring == Main.EventFiring.Enabled)
			{
				if (oldSizeX != _sizeX || oldSizeY != _sizeY || oldSizeZ != _sizeZ)
					EhSelfChanged();
			}
		}

		private void InternalSetAbsolutePositionXSilent(double value)
		{
			var mySizeX = _sizeX.GetValueRelativeTo(_parentSize.X);
			if (_positionX.IsAbsolute)
				_positionX = RADouble.NewAbs(value - _parentAnchorX.GetValueRelativeTo(_parentSize.X) + _localAnchorX.GetValueRelativeTo(mySizeX));
			else if (0 != _parentSize.X && _parentSize.X.IsFinite())
				_positionX = RADouble.NewRel((value - _parentAnchorX.GetValueRelativeTo(_parentSize.X) + _localAnchorX.GetValueRelativeTo(mySizeX)) / _parentSize.X);
			else
				throw new InvalidOperationException("_parentSize.X is undefined or zero");
		}

		private void InternalSetAbsolutePositionYSilent(double value)
		{
			var mySizeY = _sizeY.GetValueRelativeTo(_parentSize.Y);
			if (_positionY.IsAbsolute)
				_positionY = RADouble.NewAbs(value - _parentAnchorY.GetValueRelativeTo(_parentSize.Y) + _localAnchorY.GetValueRelativeTo(mySizeY));
			else if (0 != _parentSize.Y && _parentSize.Y.IsFinite())
				_positionY = RADouble.NewRel((value - _parentAnchorY.GetValueRelativeTo(_parentSize.Y) + _localAnchorY.GetValueRelativeTo(mySizeY)) / _parentSize.Y);
			else
				throw new InvalidOperationException("_parentSize.Y is undefined or zero");
		}

		private void InternalSetAbsolutePositionZSilent(double value)
		{
			var mySizeZ = _sizeZ.GetValueRelativeTo(_parentSize.Z);
			if (_positionZ.IsAbsolute)
				_positionZ = RADouble.NewAbs(value - _parentAnchorZ.GetValueRelativeTo(_parentSize.Z) + _localAnchorZ.GetValueRelativeTo(mySizeZ));
			else if (0 != _parentSize.Z && _parentSize.Z.IsFinite())
				_positionZ = RADouble.NewRel((value - _parentAnchorZ.GetValueRelativeTo(_parentSize.Z) + _localAnchorZ.GetValueRelativeTo(mySizeZ)) / _parentSize.Z);
			else
				throw new InvalidOperationException("_parentSize.Z is undefined or zero");
		}

		public double AbsolutePositionX
		{
			get
			{
				var mySizeX = _sizeX.GetValueRelativeTo(_parentSize.X);
				return _parentAnchorX.GetValueRelativeTo(_parentSize.X) + _positionX.GetValueRelativeTo(_parentSize.X) - _localAnchorX.GetValueRelativeTo(mySizeX);
			}
			set
			{
				var oldValue = _positionX;
				InternalSetAbsolutePositionXSilent(value);
				if (oldValue != _positionX)
					EhSelfChanged();
			}
		}

		public double AbsolutePositionY
		{
			get
			{
				var mySizeY = _sizeY.GetValueRelativeTo(_parentSize.Y);
				return _parentAnchorY.GetValueRelativeTo(_parentSize.Y) + _positionY.GetValueRelativeTo(_parentSize.Y) - _localAnchorY.GetValueRelativeTo(mySizeY);
			}
			set
			{
				var oldValue = _positionY;
				InternalSetAbsolutePositionYSilent(value);
				if (oldValue != _positionY)
					EhSelfChanged();
			}
		}

		public double AbsolutePositionZ
		{
			get
			{
				var mySizeZ = _sizeZ.GetValueRelativeTo(_parentSize.Z);
				return _parentAnchorZ.GetValueRelativeTo(_parentSize.Z) + _positionZ.GetValueRelativeTo(_parentSize.Z) - _localAnchorZ.GetValueRelativeTo(mySizeZ);
			}
			set
			{
				var oldValue = _positionZ;
				InternalSetAbsolutePositionZSilent(value);
				if (oldValue != _positionZ)
					EhSelfChanged();
			}
		}

		public PointD3D AbsolutePosition
		{
			get
			{
				return new PointD3D(AbsolutePositionX, AbsolutePositionY, AbsolutePositionZ);
			}
			set
			{
				var oldValueX = _positionX;
				var oldValueY = _positionY;
				var oldValueZ = _positionZ;

				InternalSetAbsolutePositionXSilent(value.X);
				InternalSetAbsolutePositionYSilent(value.Y);
				InternalSetAbsolutePositionZSilent(value.Z);

				if (oldValueX != _positionX || oldValueY != _positionY || oldValueZ != _positionZ)
					EhSelfChanged();
			}
		}

		private void InternalSetAbsolutePivotPositionXSilent(double value)
		{
			var mySizeX = _sizeX.GetValueRelativeTo(_parentSize.X);
			if (_positionX.IsAbsolute)
				_positionX = RADouble.NewAbs(value - _parentAnchorX.GetValueRelativeTo(_parentSize.X));
			else if (0 != _parentSize.X && _parentSize.X.IsFinite())
				_positionX = RADouble.NewRel((value - _parentAnchorX.GetValueRelativeTo(_parentSize.X)) / _parentSize.X);
			else
				throw new InvalidOperationException("_parentSize.X is undefined or zero");
		}

		private void InternalSetAbsolutePivotPositionYSilent(double value)
		{
			var mySizeY = _sizeY.GetValueRelativeTo(_parentSize.Y);
			if (_positionY.IsAbsolute)
				_positionY = RADouble.NewAbs(value - _parentAnchorY.GetValueRelativeTo(_parentSize.Y));
			else if (0 != _parentSize.Y && _parentSize.Y.IsFinite())
				_positionY = RADouble.NewRel((value - _parentAnchorY.GetValueRelativeTo(_parentSize.Y)) / _parentSize.Y);
			else
				throw new InvalidOperationException("_parentSize.Y is undefined or zero");
		}

		private void InternalSetAbsolutePivotPositionZSilent(double value)
		{
			var mySizeZ = _sizeZ.GetValueRelativeTo(_parentSize.Z);
			if (_positionZ.IsAbsolute)
				_positionZ = RADouble.NewAbs(value - _parentAnchorZ.GetValueRelativeTo(_parentSize.Z));
			else if (0 != _parentSize.Z && _parentSize.Z.IsFinite())
				_positionZ = RADouble.NewRel((value - _parentAnchorZ.GetValueRelativeTo(_parentSize.Z)) / _parentSize.Z);
			else
				throw new InvalidOperationException("_parentSize.Z is undefined or zero");
		}

		/// <summary>
		/// Gets or sets the absolute x position of the pivot point of the item.
		/// </summary>
		/// <value>
		/// The absolute pivot x position.
		/// </value>
		/// <exception cref="System.InvalidOperationException">_parentSize.X is undefined or zero</exception>
		public double AbsolutePivotPositionX
		{
			get
			{
				var mySizeX = _sizeX.GetValueRelativeTo(_parentSize.X);
				return _parentAnchorX.GetValueRelativeTo(_parentSize.X) + _positionX.GetValueRelativeTo(_parentSize.X);
			}
			set
			{
				var oldValue = _positionX;
				InternalSetAbsolutePivotPositionXSilent(value);
				if (oldValue != _positionX)
					EhSelfChanged();
			}
		}

		/// <summary>
		/// Gets or sets the absolute y position of the pivot point of the item.
		/// </summary>
		/// <value>
		/// The absolute pivot y position.
		/// </value>
		/// <exception cref="System.InvalidOperationException">_parentSize.Y is undefined or zero</exception>
		public double AbsolutePivotPositionY
		{
			get
			{
				var mySizeY = _sizeY.GetValueRelativeTo(_parentSize.Y);
				return _parentAnchorY.GetValueRelativeTo(_parentSize.Y) + _positionY.GetValueRelativeTo(_parentSize.Y);
			}
			set
			{
				var oldValue = _positionY;
				InternalSetAbsolutePositionYSilent(value);
				if (oldValue != _positionY)
					EhSelfChanged();
			}
		}

		/// <summary>
		/// Gets or sets the absolute z position of the pivot point of the item.
		/// </summary>
		/// <value>
		/// The absolute pivot z position.
		/// </value>
		/// <exception cref="System.InvalidOperationException">_parentSize.Z is undefined or zero</exception>
		public double AbsolutePivotPositionZ
		{
			get
			{
				var mySizeZ = _sizeZ.GetValueRelativeTo(_parentSize.Z);
				return _parentAnchorZ.GetValueRelativeTo(_parentSize.Z) + _positionZ.GetValueRelativeTo(_parentSize.Z);
			}
			set
			{
				var oldValue = _positionZ;
				InternalSetAbsolutePositionZSilent(value);
				if (oldValue != _positionZ)
					EhSelfChanged();
			}
		}

		public PointD3D AbsolutePivotPosition
		{
			get
			{
				return new PointD3D(AbsolutePivotPositionX, AbsolutePivotPositionY, AbsolutePivotPositionZ);
			}
			set
			{
				SetAbsolutePivotPosition(value, Main.EventFiring.Enabled);
			}
		}

		public void SetAbsolutePivotPosition(PointD3D value, Main.EventFiring eventFiring)
		{
			var oldValueX = _positionX;
			var oldValueY = _positionY;
			var oldValueZ = _positionZ;

			InternalSetAbsolutePivotPositionXSilent(value.X);
			InternalSetAbsolutePivotPositionYSilent(value.Y);
			InternalSetAbsolutePivotPositionZSilent(value.Z);

			if (eventFiring == Main.EventFiring.Enabled && (oldValueX != _positionX || oldValueY != _positionY || oldValueZ != _positionZ))
				EhSelfChanged();
		}

		/// <summary>
		/// Gets the absolute vector between the pivot point of the item and its left upper edge.
		/// </summary>
		/// <value>
		/// The absolute vector between the pivot point of the item and its left upper edge.
		/// </value>
		public VectorD3D AbsoluteVectorPivotToLeftUpper
		{
			get
			{
				var mySizeX = _sizeX.GetValueRelativeTo(_parentSize.X);
				var mySizeY = _sizeY.GetValueRelativeTo(_parentSize.Y);
				var mySizeZ = _sizeZ.GetValueRelativeTo(_parentSize.Z);
				return new VectorD3D(-_localAnchorX.GetValueRelativeTo(mySizeX), -_localAnchorY.GetValueRelativeTo(mySizeY), -_localAnchorZ.GetValueRelativeTo(mySizeZ));
			}
		}

		public virtual void SetRelativeSizePositionFromAbsoluteValues(VectorD3D absSize, PointD3D absPos)
		{
			var oldSizeX = SizeX;
			var oldSizeY = SizeY;
			var oldSizeZ = SizeZ;
			var oldPosX = PositionX;
			var oldPosY = PositionY;
			var oldPosZ = PositionZ;

			if (_parentSize.X == 0)
				throw new InvalidOperationException("ParentSize.X is zero. This would lead to an undefined relative value!");
			if (_parentSize.Y == 0)
				throw new InvalidOperationException("ParentSize.Y is zero. This would lead to an undefined relative value!");
			if (_parentSize.Z == 0)
				throw new InvalidOperationException("ParentSize.Z is zero. This would lead to an undefined relative value!");

			InternalSetSizeSilent(
			 RADouble.NewRel(absSize.X / _parentSize.X),
			 RADouble.NewRel(absSize.Y / _parentSize.Y),
			 RADouble.NewRel(absSize.Z / _parentSize.Z)
			);

			_positionX = RADouble.NewRel(absPos.X / _parentSize.X);
			_positionY = RADouble.NewRel(absPos.Y / _parentSize.Y);
			_positionZ = RADouble.NewRel(absPos.Z / _parentSize.Z);

			if (oldSizeX != _sizeX || oldSizeY != _sizeY || oldSizeZ != _sizeZ || oldPosX != _positionX || oldPosY != _positionY || oldPosZ != _positionZ)
				EhSelfChanged();
		}

		public void ChangeRelativeSizeValuesToAbsoluteSizeValues()
		{
			if (_sizeX.IsRelative)
				_sizeX = RADouble.NewAbs(AbsoluteSizeX);
			if (_sizeY.IsRelative)
				_sizeY = RADouble.NewAbs(AbsoluteSizeY);
			if (_sizeZ.IsRelative)
				_sizeZ = RADouble.NewAbs(AbsoluteSizeZ);
		}

		public void ChangeRelativePositionValuesToAbsolutePositionValues()
		{
			if (this._positionX.IsRelative)
				_positionX = RADouble.NewAbs(AbsolutePositionX);
			if (_positionY.IsRelative)
				_positionY = RADouble.NewAbs(AbsolutePositionY);
			if (_positionZ.IsRelative)
				_positionZ = RADouble.NewAbs(AbsolutePositionZ);
		}

		public void ChangeParentAnchorButKeepPosition(RADouble newParentAnchorX, RADouble newParentAnchorY, RADouble newParentAnchorZ)
		{
			var oldRefX = _parentAnchorX.GetValueRelativeTo(_parentSize.X);
			var oldRefY = _parentAnchorY.GetValueRelativeTo(_parentSize.Y);
			var oldRefZ = _parentAnchorZ.GetValueRelativeTo(_parentSize.Z);
			var newRefX = newParentAnchorX.GetValueRelativeTo(_parentSize.X);
			var newRefY = newParentAnchorY.GetValueRelativeTo(_parentSize.Y);
			var newRefZ = newParentAnchorZ.GetValueRelativeTo(_parentSize.Z);

			var oldPos = this.AbsolutePosition;
			this.InternalSetAbsolutePositionXSilent(oldPos.X + (oldRefX - newRefX));
			this.InternalSetAbsolutePositionYSilent(oldPos.Y + (oldRefY - newRefY));
			this.InternalSetAbsolutePositionZSilent(oldPos.Z + (oldRefZ - newRefZ));
			_parentAnchorX = newParentAnchorX;
			_parentAnchorY = newParentAnchorY;
			_parentAnchorZ = newParentAnchorZ;
		}

		public void ChangeParentAnchorToLeftTopButKeepPosition()
		{
			ChangeParentAnchorButKeepPosition(RADouble.NewRel(0), RADouble.NewRel(0), RADouble.NewRel(0));
		}

		#endregion Methods
	}
}