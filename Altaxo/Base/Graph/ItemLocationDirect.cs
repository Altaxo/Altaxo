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

		private Altaxo.Calc.RelativeOrAbsoluteValue _positionX;

		private Altaxo.Calc.RelativeOrAbsoluteValue _positionY;

		private Altaxo.Calc.RelativeOrAbsoluteValue _sizeX;

		private Altaxo.Calc.RelativeOrAbsoluteValue _sizeY;

		private Altaxo.Calc.RelativeOrAbsoluteValue _pivotX;

		private Altaxo.Calc.RelativeOrAbsoluteValue _pivotY;

		private Altaxo.Calc.RelativeOrAbsoluteValue _referenceX;

		private Altaxo.Calc.RelativeOrAbsoluteValue _referenceY;

		/// <summary>The rotation angle (in degrees) of the layer.</summary>
		private double _rotation; // Rotation

		/// <summary>The scaling factor of the layer, normally 1.</summary>
		private double _scaleX;  // X-Scale

		private double _scaleY; // Y-Scale

		private double _shear; // Shear

		[field: NonSerialized]
		private event EventHandler _changed;

		[NonSerialized]
		private object _parent;

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
				info.AddValue("ScaleX", s._scaleX);
				info.AddValue("ScaleY", s._scaleY);
				info.AddValue("ShearX", s._shear);
			}

			protected virtual ItemLocationDirect SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				var s = null != o ? (ItemLocationDirect)o : new ItemLocationDirect();

				s._sizeX = (Calc.RelativeOrAbsoluteValue)info.GetValue("SizeX");
				s._sizeY = (Calc.RelativeOrAbsoluteValue)info.GetValue("SizeY");

				s._positionX = (Calc.RelativeOrAbsoluteValue)info.GetValue("PositionX");
				s._positionY = (Calc.RelativeOrAbsoluteValue)info.GetValue("PositionY");

				s._pivotX = (Calc.RelativeOrAbsoluteValue)info.GetValue("PivotX");
				s._pivotY = (Calc.RelativeOrAbsoluteValue)info.GetValue("PivotY");

				s._referenceX = (Calc.RelativeOrAbsoluteValue)info.GetValue("ReferenceX");
				s._referenceY = (Calc.RelativeOrAbsoluteValue)info.GetValue("ReferenceY");

				s._rotation = info.GetDouble("Rotation");
				s._scaleX = info.GetDouble("ScaleX");
				s._scaleY = info.GetDouble("ScaleY");
				s._shear = info.GetDouble("ShearX");

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
			_pivotX = RelativeOrAbsoluteValue.NewRelativeValue(0);
			_pivotY = RelativeOrAbsoluteValue.NewRelativeValue(0);
			_referenceX = RelativeOrAbsoluteValue.NewRelativeValue(0);
			_referenceY = RelativeOrAbsoluteValue.NewRelativeValue(0);
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
				return true;
			}
			else if (obj is IItemLocation)
			{
				var from = (IItemLocation)obj;
				this._rotation = from.Rotation;
				this._scaleX = from.Scale;
				return true;
			}

			return false;
		}

		object System.ICloneable.Clone()
		{
			return new ItemLocationDirect(this);
		}

		public ItemLocationDirect Clone()
		{
			return new ItemLocationDirect(this);
		}

		#endregion Construction and copying

		#region Properties

		/// <summary>
		/// The width of the layer, either as absolute value in point (1/72 inch), or as
		/// relative value as pointed out by <see cref="_layerWidthType"/>.
		/// </summary>
		public Calc.RelativeOrAbsoluteValue SizeX
		{
			get { return _sizeX; }
			set
			{
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
		public Calc.RelativeOrAbsoluteValue SizeY
		{
			get { return _sizeY; }
			set
			{
				var oldvalue = _sizeY;
				_sizeY = value;

				if (value != oldvalue)
					OnChanged();
			}
		}

		/// <summary>
		/// The layers x position value, either absolute or relative, as determined by <see cref="_layerXPositionType"/>.
		/// </summary>
		public Calc.RelativeOrAbsoluteValue PositionX
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
		public Calc.RelativeOrAbsoluteValue PositionY
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
		public Calc.RelativeOrAbsoluteValue PivotX
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
		public Calc.RelativeOrAbsoluteValue PivotY
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
		public Calc.RelativeOrAbsoluteValue ReferenceX
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
		public Calc.RelativeOrAbsoluteValue ReferenceY
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

		public void SetPositionAndSize(RelativeOrAbsoluteValue x, RelativeOrAbsoluteValue y, RelativeOrAbsoluteValue width, RelativeOrAbsoluteValue height)
		{
			bool isChanged = x != _positionX || y != _positionY || width != _sizeX || height != _sizeY;

			_positionX = x;
			_positionY = y;
			_sizeX = width;
			_sizeY = height;

			if (isChanged)
				OnChanged();
		}

		/// <summary>The scaling factor of the item, normally 1.</summary>
		public double Scale
		{
			get
			{
				if (_scaleX == _scaleY)
					return _scaleX;
				else
					throw new InvalidOperationException("The scales in x and y direction are different. Use ScaleX and ScaleY properties instead");
			}
			set
			{
				var oldvalueX = _scaleX;
				var oldValueY = _scaleY;
				_scaleX = _scaleY = value;
				if (value != oldvalueX || value != oldValueY)
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
		/// <param name="parentSize">Size of the parent object (needed when relative values for size or position where used).</param>
		/// <returns>The enclosing rectangle in absolute values.</returns>
		public RectangleD GetAbsoluteEnclosingRectangleWithoutSSRS(PointD2D parentSize)
		{
			var mySizeX = _sizeX.GetValueRelativeTo(parentSize.X);
			var mySizeY = _sizeX.GetValueRelativeTo(parentSize.Y);

			var myPosX = _referenceX.GetValueRelativeTo(parentSize.X) + _positionX.GetValueRelativeTo(parentSize.X) - _pivotX.GetValueRelativeTo(mySizeX);
			var myPosY = _referenceY.GetValueRelativeTo(parentSize.Y) + _positionY.GetValueRelativeTo(parentSize.Y) - _pivotX.GetValueRelativeTo(mySizeY);

			return new RectangleD(myPosX, myPosY, mySizeX, mySizeY);
		}

		public double GetAbsoluteSizeX(double parentSizeX)
		{
			return _sizeX.GetValueRelativeTo(parentSizeX);
		}

		public void SetAbsoluteSizeX(double value, double parentSizeX)
		{
			if (_sizeX.IsAbsolute)
				_sizeX = RelativeOrAbsoluteValue.NewAbsoluteValue(value);
			else if (parentSizeX != 0 && !double.IsNaN(parentSizeX))
				_sizeX = RelativeOrAbsoluteValue.NewRelativeValue(value / parentSizeX);
			else
				throw new InvalidOperationException("parentSizeX is undefined or zero");
		}

		public double GetAbsoluteSizeY(double parentSizeY)
		{
			return _sizeY.GetValueRelativeTo(parentSizeY);
		}

		public void SetAbsoluteSizeY(double value, double parentSizeY)
		{
			if (_sizeY.IsAbsolute)
				_sizeY = RelativeOrAbsoluteValue.NewAbsoluteValue(value);
			else if (parentSizeY != 0 && !double.IsNaN(parentSizeY))
				_sizeY = RelativeOrAbsoluteValue.NewRelativeValue(value / parentSizeY);
			else
				throw new InvalidOperationException("parentSizeY is undefined or zero");
		}

		public double GetAbsolutePositionX(double parentSizeX)
		{
			var mySizeX = _sizeX.GetValueRelativeTo(parentSizeX);
			return _referenceX.GetValueRelativeTo(parentSizeX) + _positionX.GetValueRelativeTo(parentSizeX) - _pivotX.GetValueRelativeTo(mySizeX);
		}

		public void SetAbsolutePositionX(double x, double parentSizeX)
		{
			var mySizeX = _sizeX.GetValueRelativeTo(parentSizeX);
			if (_positionX.IsAbsolute)
				_positionX = RelativeOrAbsoluteValue.NewAbsoluteValue(x - _referenceX.GetValueRelativeTo(parentSizeX) + _pivotX.GetValueRelativeTo(mySizeX));
			else if (0 != parentSizeX && parentSizeX.IsFinite())
				_positionX = RelativeOrAbsoluteValue.NewRelativeValue((x - _referenceX.GetValueRelativeTo(parentSizeX) + _pivotX.GetValueRelativeTo(mySizeX)) / parentSizeX);
			else
				throw new InvalidOperationException("parentSizeX is undefined or zero");
		}

		public double GetAbsolutePositionY(double parentSizeY)
		{
			var mySizeY = _sizeY.GetValueRelativeTo(parentSizeY);
			return _referenceY.GetValueRelativeTo(parentSizeY) + _positionY.GetValueRelativeTo(parentSizeY) - _pivotY.GetValueRelativeTo(mySizeY);
		}

		public void SetAbsolutePositionY(double y, double parentSizeY)
		{
			var mySizeY = _sizeY.GetValueRelativeTo(parentSizeY);
			if (_positionY.IsAbsolute)
				_positionY = RelativeOrAbsoluteValue.NewAbsoluteValue(y - _referenceY.GetValueRelativeTo(parentSizeY) + _pivotY.GetValueRelativeTo(mySizeY));
			else if (0 != parentSizeY && parentSizeY.IsFinite())
				_positionY = RelativeOrAbsoluteValue.NewRelativeValue((y - _referenceY.GetValueRelativeTo(parentSizeY) + _pivotY.GetValueRelativeTo(mySizeY)) / parentSizeY);
			else
				throw new InvalidOperationException("parentSizeY is undefined or zero");
		}

		#endregion Methods
	}
}