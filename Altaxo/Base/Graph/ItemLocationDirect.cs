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
		private Altaxo.Calc.RelativeOrAbsoluteValue _positionX;

		private Altaxo.Calc.RelativeOrAbsoluteValue _positionY;

		private Altaxo.Calc.RelativeOrAbsoluteValue _sizeX;

		private Altaxo.Calc.RelativeOrAbsoluteValue _sizeY;

		/// <summary>The rotation angle (in degrees) of the layer.</summary>
		private double _rotation = 0; // Rotation

		/// <summary>The scaling factor of the layer, normally 1.</summary>
		private double _scaleX = 1;  // X-Scale

		private double _scaleY = 1; // Y-Scale

		private double _shear; // Shear

		[field: NonSerialized]
		private event EventHandler _changed;

		[NonSerialized]
		private object _parent;

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

		public ItemLocationDirect()
		{
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

		/// <summary>
		/// Gets the absolute enclosing rectangle without taking into account ScaleX, ScaleY, Rotation and Shear (SSRS).
		/// </summary>
		/// <param name="parentSize">Size of the parent object (needed when relative values for size or position where used).</param>
		/// <returns>The enclosing rectangle in absolute values.</returns>
		public RectangleD GetAbsoluteEnclosingRectangleWithoutSSRS(PointD2D parentSize)
		{
			return new RectangleD(
				_positionX.GetValueRelativeTo(parentSize.X),
				_positionY.GetValueRelativeTo(parentSize.Y),
				_sizeX.GetValueRelativeTo(parentSize.X),
				_sizeY.GetValueRelativeTo(parentSize.Y)
				);
		}

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
	}
}