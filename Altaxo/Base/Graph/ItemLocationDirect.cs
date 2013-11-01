#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2013 Dr. Dirk Lellinger
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Graph
{
	[Serializable]
	public class ItemLocationDirect : IItemLocation
	{
		#region Members

		private RADouble _positionX;

		private RADouble _positionY;

		private RADouble _sizeX;

		private RADouble _sizeY;

		private RADouble _pivotX;

		private RADouble _pivotY;

		private RADouble _referenceX;

		private RADouble _referenceY;

		/// <summary>The rotation angle (in degrees) of the layer.</summary>
		private double _rotation; // Rotation

		/// <summary>The scaling factor of the layer, normally 1.</summary>
		private double _scaleX;  // X-Scale

		private double _scaleY; // Y-Scale

		private double _shear; // Shear

		// Cached and not-to-serialize members

		[field: NonSerialized]
		private event EventHandler _changed;

		[NonSerialized]
		private object _parent;

		[NonSerialized]
		protected PointD2D _parentSize;

		#endregion Members

		#region Serialization

		/// <summary>
		/// 2013-10-01 initial version.
		/// </summary>
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ItemLocationDirect), 0)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				var s = (ItemLocationDirect)obj;

				info.AddValue("SizeX", s._sizeX);
				info.AddValue("SizeY", s._sizeY);

				info.AddValue("PositionX", s._positionX);
				info.AddValue("PositionY", s._positionY);

				info.AddValue("PivotX", s._pivotX);
				info.AddValue("PivotY", s._pivotY);

				info.AddValue("ReferenceX", s._referenceX);
				info.AddValue("ReferenceY", s._referenceY);

				info.AddValue("Rotation", s._rotation);
				info.AddValue("ShearX", s._shear);
				info.AddValue("ScaleX", s._scaleX);
				info.AddValue("ScaleY", s._scaleY);
			}

			protected virtual ItemLocationDirect SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				var s = null != o ? (ItemLocationDirect)o : new ItemLocationDirect();

				s._sizeX = (RADouble)info.GetValue("SizeX");
				s._sizeY = (RADouble)info.GetValue("SizeY");

				s._positionX = (RADouble)info.GetValue("PositionX");
				s._positionY = (RADouble)info.GetValue("PositionY");

				s._pivotX = (RADouble)info.GetValue("PivotX");
				s._pivotY = (RADouble)info.GetValue("PivotY");

				s._referenceX = (RADouble)info.GetValue("ReferenceX");
				s._referenceY = (RADouble)info.GetValue("ReferenceY");

				s._rotation = info.GetDouble("Rotation");
				s._shear = info.GetDouble("ShearX");
				s._scaleX = info.GetDouble("ScaleX");
				s._scaleY = info.GetDouble("ScaleY");

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
			_pivotX = RADouble.NewRel(0);
			_pivotY = RADouble.NewRel(0);
			_referenceX = RADouble.NewRel(0);
			_referenceY = RADouble.NewRel(0);
			_scaleX = 1;
			_scaleY = 1;
		}

		public ItemLocationDirect(ItemLocationDirect from)
		{
			CopyFrom(from);
		}

		public ItemLocationDirect(IItemLocation from)
		{
			CopyFrom(from);
		}

		public bool CopyFrom(object obj)
		{
			if (object.ReferenceEquals(this, obj))
				return true;

			if (obj is ItemLocationDirect)
			{
				var from = (ItemLocationDirect)obj;
				this._parentSize = from._parentSize;
				this._positionX = from._positionX;
				this._positionY = from._positionY;
				this._sizeX = from._sizeX;
				this._sizeY = from._sizeY;
				this._pivotX = from._pivotX;
				this._pivotY = from._pivotY;
				this._referenceX = from._referenceX;
				this._referenceY = from._referenceY;
				this._rotation = from._rotation;
				this._scaleX = from._scaleX;
				this._scaleY = from._scaleY;
				this._shear = from._shear;
				OnChanged();
				return true;
			}
			else if (obj is IItemLocation)
			{
				var from = (IItemLocation)obj;
				this._rotation = from.Rotation;
				this._shear = from.ShearX;
				this._scaleX = from.ScaleX;
				this._scaleY = from.ScaleY;
				OnChanged();
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

		public void SetParentSize(PointD2D parentSize, bool shouldTriggerChangedEvent)
		{
			var oldValue = _parentSize;
			_parentSize = parentSize;

			if (shouldTriggerChangedEvent && oldValue != _parentSize)
				OnChanged();
		}

		public PointD2D ParentSize
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
		/// If the <see cref="IsAutoSize"/> property is <c>true</c> for this instance, the graphical object has to use this function to indicate its size.
		/// </summary>
		/// <param name="autoSize">Size of the graphical object.</param>
		/// <exception cref="System.InvalidOperationException">Using SetAutoSize is not supported because IsAutoSized is false</exception>
		public void SetSizeInAutoSizeMode(PointD2D autoSize)
		{
			if (!IsAutoSized)
				throw new InvalidOperationException("Using SetAutoSize is not supported because IsAutoSized is false");

			if (_sizeX.IsRelative || _sizeY.IsRelative || _sizeX.Value != autoSize.X || _sizeY.Value != autoSize.Y)
			{
				_sizeX = RADouble.NewAbs(autoSize.X);
				_sizeY = RADouble.NewAbs(autoSize.Y);
				OnChanged();
			}
		}

		/// <summary>
		/// The width of the layer, either as absolute value in point (1/72 inch), or as
		/// relative value as pointed out by <see cref="_layerWidthType"/>.
		/// </summary>
		public RADouble SizeX
		{
			get { return _sizeX; }
			set
			{
				if (IsAutoSized)
					throw new InvalidOperationException("Setting the size is not supported because CanSetSize is false");

				var oldvalue = _sizeX;
				_sizeX = value;

				if (value != oldvalue)
					OnChanged();
			}
		}

		/// <summary>
		/// The width of the layer, either as absolute value in point (1/72 inch), or as
		/// relative value as pointed out by <see cref="_layerWidthType"/>.
		/// </summary>
		public RADouble SizeY
		{
			get { return _sizeY; }
			set
			{
				if (IsAutoSized)
					throw new InvalidOperationException("Setting the size is not supported because CanSetSize is false");
				var oldvalue = _sizeY;
				_sizeY = value;

				if (value != oldvalue)
					OnChanged();
			}
		}

		/// <summary>
		/// The layers x position value, either absolute or relative, as determined by <see cref="_layerXPositionType"/>.
		/// </summary>
		public RADouble PositionX
		{
			get { return _positionX; }
			set
			{
				var oldvalue = _positionX;
				_positionX = value;

				if (value != oldvalue)
					OnChanged();
			}
		}

		/// <summary>
		/// The layers y position value, either absolute or relative, as determined by <see cref="_layerYPositionType"/>.
		/// </summary>
		public RADouble PositionY
		{
			get { return _positionY; }
			set
			{
				var oldvalue = _positionY;
				_positionY = value;

				if (value != oldvalue)
					OnChanged();
			}
		}

		/// <summary>
		/// Gets or sets the pivot x coordinate. This is a absolute value or a value relative to the own x-size and designates the point from the left upper corner of the own shape, which is the position of the object.
		/// </summary>
		/// <value>
		/// The pivot y coordinate.
		/// </value>
		public RADouble PivotX
		{
			get { return _pivotX; }
			set
			{
				var oldvalue = _pivotX;
				_pivotX = value;

				if (value != oldvalue)
					OnChanged();
			}
		}

		/// <summary>
		/// Gets or sets the pivot y coordinate. This is a absolute value or a value relative to the own y-size and designates the point from the left upper corner of the own shape, which is the position of the object.
		/// </summary>
		/// <value>
		/// The pivot y coordinate.
		/// </value>
		public RADouble PivotY
		{
			get { return _pivotY; }
			set
			{
				var oldvalue = _pivotY;
				_pivotY = value;

				if (value != oldvalue)
					OnChanged();
			}
		}

		/// <summary>
		/// Gets or sets the reference x coordinate. This is a absolute value or a value relative to the parent SizeX and designates the parent coordinate, where the positioning is referenced to.
		/// </summary>
		/// <value>
		/// The reference x coordinate.
		/// </value>
		public RADouble ReferenceX
		{
			get { return _referenceX; }
			set
			{
				var oldvalue = _referenceX;
				_referenceX = value;

				if (value != oldvalue)
					OnChanged();
			}
		}

		/// <summary>
		/// Gets or sets the reference x coordinate. This is a absolute value or a value relative to the parent SizeX and designates the parent coordinate, where the positioning is referenced to.
		/// </summary>
		/// <value>
		/// The reference x coordinate.
		/// </value>
		public RADouble ReferenceY
		{
			get { return _referenceY; }
			set
			{
				var oldvalue = _referenceY;
				_referenceY = value;

				if (value != oldvalue)
					OnChanged();
			}
		}

		public void SetPositionAndSize(RADouble x, RADouble y, RADouble width, RADouble height)
		{
			bool isChanged = x != _positionX || y != _positionY || width != _sizeX || height != _sizeY;

			_positionX = x;
			_positionY = y;

			if (!IsAutoSized)
			{
				_sizeX = width;
				_sizeY = height;
			}

			if (isChanged)
				OnChanged();
		}

		/// <summary>The scaling factor of the item, normally 1.</summary>
		public PointD2D Scale
		{
			get
			{
				return new PointD2D(_scaleX, _scaleY);
			}
			set
			{
				bool isChanged = _scaleX != value.X || _scaleY != value.Y;
				_scaleX = value.X;
				_scaleY = value.Y;
				if (isChanged)
					OnChanged();
			}
		}

		/// <summary>The scaling factor of the item in x-direction, normally 1.</summary>
		public double ScaleX
		{
			get { return _scaleX; }
			set
			{
				double oldvalue = _scaleX;
				_scaleX = value;
				if (value != oldvalue)
					OnChanged();
			}
		}

		/// <summary>The scaling factor of the item in y-direction, normally 1.</summary>
		public double ScaleY
		{
			get { return _scaleY; }
			set
			{
				double oldvalue = _scaleY;
				_scaleY = value;
				if (value != oldvalue)
					OnChanged();
			}
		}

		/// <summary>The rotation angle (in degrees) of the layer.</summary>
		public double Rotation
		{
			get { return _rotation; }
			set
			{
				double oldvalue = _rotation;
				_rotation = value;

				if (value != oldvalue)
					OnChanged();
			}
		}

		/// <summary>The shear factor of the item.</summary>
		public double ShearX
		{
			get { return _shear; }
			set
			{
				double oldvalue = _shear;
				_shear = value;
				if (value != oldvalue)
					OnChanged();
			}
		}

		#endregion Properties

		#region IChangedEventSource Members

		event EventHandler Altaxo.Main.IChangedEventSource.Changed
		{
			add { _changed += value; }
			remove { _changed -= value; }
		}

		protected virtual void OnChanged()
		{
			if (_parent is Main.IChildChangedEventSink)
				((Main.IChildChangedEventSink)_parent).EhChildChanged(this, EventArgs.Empty);

			if (null != _changed)
				_changed(this, EventArgs.Empty);
		}

		#endregion IChangedEventSource Members

		#region IDocumentNode Members

		public object ParentObject
		{
			get { return _parent; }
			set { _parent = value; }
		}

		public string Name
		{
			get { return "ItemSizeAndPosition"; }
		}

		#endregion IDocumentNode Members

		#region Methods

		/// <summary>
		/// Gets the absolute enclosing rectangle without taking into account ScaleX, ScaleY, Rotation and Shear (SSRS).
		/// </summary>
		/// <returns>The enclosing rectangle in absolute values.</returns>
		public RectangleD GetAbsoluteEnclosingRectangleWithoutSSRS()
		{
			var mySizeX = _sizeX.GetValueRelativeTo(_parentSize.X);
			var mySizeY = _sizeY.GetValueRelativeTo(_parentSize.Y);

			var myPosX = _referenceX.GetValueRelativeTo(_parentSize.X) + _positionX.GetValueRelativeTo(_parentSize.X) - _pivotX.GetValueRelativeTo(mySizeX);
			var myPosY = _referenceY.GetValueRelativeTo(_parentSize.Y) + _positionY.GetValueRelativeTo(_parentSize.Y) - _pivotY.GetValueRelativeTo(mySizeY);

			return new RectangleD(myPosX, myPosY, mySizeX, mySizeY);
		}

		private void InternalSetAbsoluteSizeXSilent(double value)
		{
			if (_sizeX.IsAbsolute)
				_sizeX = RADouble.NewAbs(value);
			else if (_parentSize.X != 0 && !double.IsNaN(_parentSize.X))
				_sizeX = RADouble.NewRel(value / _parentSize.X);
			else
				throw new InvalidOperationException("_parentSize.X is undefined or zero");
		}

		private void InternalSetAbsoluteSizeYSilent(double value)
		{
			if (_sizeY.IsAbsolute)
				_sizeY = RADouble.NewAbs(value);
			else if (_parentSize.Y != 0 && !double.IsNaN(_parentSize.Y))
				_sizeY = RADouble.NewRel(value / _parentSize.Y);
			else
				throw new InvalidOperationException("_parentSize.Y is undefined or zero");
		}

		public double AbsoluteSizeX
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
					OnChanged();
			}
		}

		public double AbsoluteSizeY
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
					OnChanged();
			}
		}

		public PointD2D AbsoluteSize
		{
			get
			{
				return new PointD2D(AbsoluteSizeX, AbsoluteSizeY);
			}
			set
			{
				var oldSizeX = _sizeX;
				var oldSizeY = _sizeY;
				InternalSetAbsoluteSizeXSilent(value.X);
				InternalSetAbsoluteSizeYSilent(value.Y);

				if (oldSizeX != _sizeX || oldSizeY != _sizeY)
					OnChanged();
			}
		}

		private void InternalSetAbsolutePositionXSilent(double value)
		{
			var mySizeX = _sizeX.GetValueRelativeTo(_parentSize.X);
			if (_positionX.IsAbsolute)
				_positionX = RADouble.NewAbs(value - _referenceX.GetValueRelativeTo(_parentSize.X) + _pivotX.GetValueRelativeTo(mySizeX));
			else if (0 != _parentSize.X && _parentSize.X.IsFinite())
				_positionX = RADouble.NewRel((value - _referenceX.GetValueRelativeTo(_parentSize.X) + _pivotX.GetValueRelativeTo(mySizeX)) / _parentSize.X);
			else
				throw new InvalidOperationException("_parentSize.X is undefined or zero");
		}

		private void InternalSetAbsolutePositionYSilent(double value)
		{
			var mySizeY = _sizeY.GetValueRelativeTo(_parentSize.Y);
			if (_positionY.IsAbsolute)
				_positionY = RADouble.NewAbs(value - _referenceY.GetValueRelativeTo(_parentSize.Y) + _pivotY.GetValueRelativeTo(mySizeY));
			else if (0 != _parentSize.Y && _parentSize.Y.IsFinite())
				_positionY = RADouble.NewRel((value - _referenceY.GetValueRelativeTo(_parentSize.Y) + _pivotY.GetValueRelativeTo(mySizeY)) / _parentSize.Y);
			else
				throw new InvalidOperationException("_parentSize.Y is undefined or zero");
		}

		public double AbsolutePositionX
		{
			get
			{
				var mySizeX = _sizeX.GetValueRelativeTo(_parentSize.X);
				return _referenceX.GetValueRelativeTo(_parentSize.X) + _positionX.GetValueRelativeTo(_parentSize.X) - _pivotX.GetValueRelativeTo(mySizeX);
			}
			set
			{
				var oldValue = _positionX;
				InternalSetAbsolutePositionXSilent(value);
				if (oldValue != _positionX)
					OnChanged();
			}
		}

		public double AbsolutePositionY
		{
			get
			{
				var mySizeY = _sizeY.GetValueRelativeTo(_parentSize.Y);
				return _referenceY.GetValueRelativeTo(_parentSize.Y) + _positionY.GetValueRelativeTo(_parentSize.Y) - _pivotY.GetValueRelativeTo(mySizeY);
			}
			set
			{
				var oldValue = _positionY;
				InternalSetAbsolutePositionYSilent(value);
				if (oldValue != _positionY)
					OnChanged();
			}
		}

		public PointD2D AbsolutePosition
		{
			get
			{
				return new PointD2D(AbsolutePositionX, AbsolutePositionY);
			}
			set
			{
				var oldValueX = _positionX;
				var oldValueY = _positionY;

				InternalSetAbsolutePositionXSilent(value.X);
				InternalSetAbsolutePositionYSilent(value.Y);

				if (oldValueX != _positionX || oldValueY != _positionY)
					OnChanged();
			}
		}

		private void InternalSetAbsolutePivotPositionXSilent(double value)
		{
			var mySizeX = _sizeX.GetValueRelativeTo(_parentSize.X);
			if (_positionX.IsAbsolute)
				_positionX = RADouble.NewAbs(value - _referenceX.GetValueRelativeTo(_parentSize.X));
			else if (0 != _parentSize.X && _parentSize.X.IsFinite())
				_positionX = RADouble.NewRel((value - _referenceX.GetValueRelativeTo(_parentSize.X)) / _parentSize.X);
			else
				throw new InvalidOperationException("_parentSize.X is undefined or zero");
		}

		private void InternalSetAbsolutePivotPositionYSilent(double value)
		{
			var mySizeY = _sizeY.GetValueRelativeTo(_parentSize.Y);
			if (_positionY.IsAbsolute)
				_positionY = RADouble.NewAbs(value - _referenceY.GetValueRelativeTo(_parentSize.Y));
			else if (0 != _parentSize.Y && _parentSize.Y.IsFinite())
				_positionY = RADouble.NewRel((value - _referenceY.GetValueRelativeTo(_parentSize.Y)) / _parentSize.Y);
			else
				throw new InvalidOperationException("_parentSize.Y is undefined or zero");
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
				return _referenceX.GetValueRelativeTo(_parentSize.X) + _positionX.GetValueRelativeTo(_parentSize.X);
			}
			set
			{
				var oldValue = _positionX;
				InternalSetAbsolutePivotPositionXSilent(value);
				if (oldValue != _positionX)
					OnChanged();
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
				return _referenceY.GetValueRelativeTo(_parentSize.Y) + _positionY.GetValueRelativeTo(_parentSize.Y);
			}
			set
			{
				var oldValue = _positionY;
				InternalSetAbsolutePositionXSilent(value);
				if (oldValue != _positionY)
					OnChanged();
			}
		}

		public PointD2D AbsolutePivotPosition
		{
			get
			{
				return new PointD2D(AbsolutePivotPositionX, AbsolutePivotPositionY);
			}
			set
			{
				var oldValueX = _positionX;
				var oldValueY = _positionY;

				InternalSetAbsolutePivotPositionXSilent(value.X);
				InternalSetAbsolutePivotPositionYSilent(value.Y);

				if (oldValueX != _positionX || oldValueY != _positionY)
					OnChanged();
			}
		}

		/// <summary>
		/// Gets the absolute vector between the pivot point of the item and its left upper edge.
		/// </summary>
		/// <value>
		/// The absolute vector between the pivot point of the item and its left upper edge.
		/// </value>
		public PointD2D AbsoluteVectorPivotToLeftUpper
		{
			get
			{
				var mySizeX = _sizeX.GetValueRelativeTo(_parentSize.X);
				var mySizeY = _sizeY.GetValueRelativeTo(_parentSize.Y);
				return new PointD2D(-_pivotX.GetValueRelativeTo(mySizeX), -_pivotY.GetValueRelativeTo(mySizeY));
			}
		}

		public virtual void SetRelativeSizePositionFromAbsoluteValues(PointD2D absSize, PointD2D absPos)
		{
			var oldSizeX = SizeX;
			var oldSizeY = SizeY;
			var oldPosX = PositionX;
			var oldPosY = PositionY;

			if (_parentSize.X == 0)
				throw new InvalidOperationException("ParentSize.X is zero. This would lead to an undefined relative value!");
			if (_parentSize.Y == 0)
				throw new InvalidOperationException("ParentSize.Y is zero. This would lead to an undefined relative value!");

			_sizeX = RADouble.NewRel(absSize.X / _parentSize.X);
			_sizeY = RADouble.NewRel(absSize.Y / _parentSize.Y);

			_positionX = RADouble.NewRel(absPos.X / _parentSize.X);
			_positionY = RADouble.NewRel(absPos.Y / _parentSize.Y);

			if (oldSizeX != _sizeX || oldSizeY != _sizeY || oldPosX != _positionX || oldPosY != _positionY)
				OnChanged();
		}

		#endregion Methods
	}
}