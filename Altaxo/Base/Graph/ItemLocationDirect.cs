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
		private Altaxo.Calc.RelativeOrAbsoluteValue _xPosition;

		private Altaxo.Calc.RelativeOrAbsoluteValue _yPosition;

		private Altaxo.Calc.RelativeOrAbsoluteValue _xSize;

		private Altaxo.Calc.RelativeOrAbsoluteValue _ySize;

		/// <summary>The rotation angle (in degrees) of the layer.</summary>
		private double _rotation = 0; // Rotation

		/// <summary>The scaling factor of the layer, normally 1.</summary>
		private double _scale = 1;  // Scale

		[field: NonSerialized]
		private event EventHandler _changed;

		[NonSerialized]
		private object _parent;

		#region Serialization

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ItemLocationDirect), 0)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				var s = (ItemLocationDirect)obj;

				info.AddValue("XSize", s._xSize);
				info.AddValue("YSize", s._ySize);

				info.AddValue("XPosition", s._xPosition);
				info.AddValue("YPosition", s._yPosition);

				info.AddValue("Rotation", s._rotation);
				info.AddValue("ScaleX", s._scale);
				info.AddValue("ScaleY", s._scale);
				info.AddValue("Shear", 0);
			}

			protected virtual ItemLocationDirect SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				var s = null != o ? (ItemLocationDirect)o : new ItemLocationDirect();

				s._xSize = (Calc.RelativeOrAbsoluteValue)info.GetValue("XSize");
				s._ySize = (Calc.RelativeOrAbsoluteValue)info.GetValue("YSize");
				s._xPosition = (Calc.RelativeOrAbsoluteValue)info.GetValue("XPosition");
				s._yPosition = (Calc.RelativeOrAbsoluteValue)info.GetValue("YPosition");
				s._rotation = info.GetDouble("Rotation");
				s._scale = info.GetDouble("ScaleX");
				var scaleY = info.GetDouble("ScaleY");
				var shear = info.GetDouble("Shear");

				if (shear != 0 || scaleY != s._scale)
					throw new NotImplementedException("Shear or ScaleY not implemented yet.");

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
				this._xPosition = from._xPosition;
				this._yPosition = from._yPosition;
				this._xSize = from._xSize;
				this._ySize = from._ySize;

				this._rotation = from._rotation;
				this._scale = from._scale;
				return true;
			}
			else if (obj is IItemLocation)
			{
				var from = (IItemLocation)obj;
				this._rotation = from.Rotation;
				this._scale = from.Scale;
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
		/// The layers x position value, either absolute or relative, as determined by <see cref="_layerXPositionType"/>.
		/// </summary>
		public Calc.RelativeOrAbsoluteValue XPosition
		{
			get { return _xPosition; }
			set
			{
				var oldvalue = _xPosition;
				_xPosition = value;

				if (value != oldvalue)
					OnChanged();
			}
		}

		/// <summary>
		/// The layers y position value, either absolute or relative, as determined by <see cref="_layerYPositionType"/>.
		/// </summary>
		public Calc.RelativeOrAbsoluteValue YPosition
		{
			get { return _yPosition; }
			set
			{
				var oldvalue = _yPosition;
				_yPosition = value;

				if (value != oldvalue)
					OnChanged();
			}
		}

		/// <summary>
		/// The width of the layer, either as absolute value in point (1/72 inch), or as
		/// relative value as pointed out by <see cref="_layerWidthType"/>.
		/// </summary>
		public Calc.RelativeOrAbsoluteValue XSize
		{
			get { return _xSize; }
			set
			{
				var oldvalue = _xSize;
				_xSize = value;

				if (value != oldvalue)
					OnChanged();
			}
		}

		/// <summary>
		/// The width of the layer, either as absolute value in point (1/72 inch), or as
		/// relative value as pointed out by <see cref="_layerWidthType"/>.
		/// </summary>
		public Calc.RelativeOrAbsoluteValue YSize
		{
			get { return _ySize; }
			set
			{
				var oldvalue = _ySize;
				_ySize = value;

				if (value != oldvalue)
					OnChanged();
			}
		}

		public void SetPositionAndSize(RelativeOrAbsoluteValue x, RelativeOrAbsoluteValue y, RelativeOrAbsoluteValue width, RelativeOrAbsoluteValue height)
		{
			bool isChanged = x != _xPosition || y != _yPosition || width != _xSize || height != _ySize;

			_xPosition = x;
			_yPosition = y;
			_xSize = width;
			_ySize = height;

			if (isChanged)
				OnChanged();
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

		/// <summary>The scaling factor of the layer, normally 1.</summary>
		public double Scale
		{
			get { return _scale; }
			set
			{
				double oldvalue = _scale;
				_scale = value;
				if (value != oldvalue)
					OnChanged();
			}
		}

		public RectangleD GetAbsoluteEnclosingRectangle(PointD2D parentSize)
		{
			return new RectangleD(
				_xPosition.GetValueRelativeTo(parentSize.X),
				_yPosition.GetValueRelativeTo(parentSize.Y),
				_xSize.GetValueRelativeTo(parentSize.X),
				_ySize.GetValueRelativeTo(parentSize.Y)
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
			get { return "Layer Position and Size"; }
		}

		#endregion IDocumentNode Members
	}
}