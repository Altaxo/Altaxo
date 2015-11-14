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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Graph.Graph3D
{
	public class ItemLocationByGrid : Main.SuspendableDocumentLeafNodeWithEventArgs, IItemLocation
	{
		/// <summary>
		/// The number of the grid column where the cell starts horizontal.
		/// </summary>
		private double _gridPosX = 1;

		/// <summary>The number of the grid row where the cell starts vertically.</summary>
		private double _gridPosY = 1;

		/// <summary>The number of the grid row where the cell starts vertically.</summary>
		private double _gridPosZ = 1;

		/// <summary>
		/// The number of grid columns that the cell spans horizontally.
		/// </summary>
		private double _gridSpanX = 1;

		/// <summary>
		/// The number of grid rows that the cell spans vertically.
		/// </summary>
		private double _gridSpanY = 1;

		/// <summary>
		/// The number of grid rows that the cell spans vertically.
		/// </summary>
		private double _gridSpanZ = 1;

		/// <summary>
		/// If true, the angle and scale is taken into account, so that the layer fits exactly into the grid cell.
		/// </summary>
		private bool _forceFitIntoCell;

		/// <summary>The rotation angle (in degrees) of the layer.</summary>
		private double _rotationX = 0; // Rotation

		/// <summary>The rotation angle (in degrees) of the layer.</summary>
		private double _rotationY = 0; // Rotation

		/// <summary>The rotation angle (in degrees) of the layer.</summary>
		private double _rotationZ = 0; // Rotation

		/// <summary>The shear factor of the layer, normally 0.</summary>
		private double _shearX = 0;

		/// <summary>The shear factor of the layer, normally 0.</summary>
		private double _shearY = 0;

		/// <summary>The shear factor of the layer, normally 0.</summary>
		private double _shearZ = 0;

		/// <summary>The scaling factor of the layer, normally 1.</summary>
		private double _scaleX = 1;  // Scale

		/// <summary>The scaling factor of the layer, normally 1.</summary>
		private double _scaleY = 1;  // Scale

		/// <summary>The scaling factor of the layer, normally 1.</summary>
		private double _scaleZ = 1;  // Scale

		#region Serialization

		/// <summary>
		/// 2015-11-14 initial version.
		/// </summary>
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ItemLocationByGrid), 0)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				var s = (ItemLocationByGrid)obj;

				info.AddValue("PosX", s._gridPosX);
				info.AddValue("PosY", s._gridPosY);
				info.AddValue("PosZ", s._gridPosZ);
				info.AddValue("SpanX", s._gridSpanX);
				info.AddValue("SpanY", s._gridSpanY);
				info.AddValue("SpanZ", s._gridSpanZ);
				info.AddValue("RotationX", s._rotationX);
				info.AddValue("RotationY", s._rotationY);
				info.AddValue("RotationZ", s._rotationZ);
				info.AddValue("ShearX", s._shearX);
				info.AddValue("ShearY", s._shearY);
				info.AddValue("ShearZ", s._shearZ);
				info.AddValue("ScaleX", s._scaleX);
				info.AddValue("ScaleY", s._scaleY);
				info.AddValue("ScaleZ", s._scaleZ);
				info.AddValue("ForceFitIntoCell", s._forceFitIntoCell);
			}

			protected virtual ItemLocationByGrid SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				var s = null != o ? (ItemLocationByGrid)o : new ItemLocationByGrid();

				s._gridPosX = info.GetDouble("PosX");
				s._gridPosY = info.GetDouble("PosY");
				s._gridPosZ = info.GetDouble("PosZ");
				s._gridSpanX = info.GetDouble("SpanX");
				s._gridSpanY = info.GetDouble("SpanY");
				s._gridSpanZ = info.GetDouble("SpanZ");
				s._rotationX = info.GetDouble("RotationX");
				s._rotationY = info.GetDouble("RotationY");
				s._rotationZ = info.GetDouble("RotationZ");
				s._shearX = info.GetDouble("ShearX");
				s._shearY = info.GetDouble("ShearY");
				s._shearZ = info.GetDouble("ShearZ");
				s._scaleX = info.GetDouble("ScaleX");
				s._scaleY = info.GetDouble("ScaleY");
				s._scaleZ = info.GetDouble("ScaleZ");
				s._forceFitIntoCell = info.GetBoolean("ForceFitIntoCell");

				return s;
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				var s = SDeserialize(o, info, parent);
				return s;
			}
		}

		#endregion Serialization

		public ItemLocationByGrid()
		{
		}

		public ItemLocationByGrid(ItemLocationByGrid from)
		{
			CopyFrom(from);
		}

		public bool CopyFrom(object obj)
		{
			if (object.ReferenceEquals(this, obj))
				return true;

			if (obj is ItemLocationByGrid)
			{
				var from = (ItemLocationByGrid)obj;
				this._gridPosX = from._gridPosX;
				this._gridPosY = from._gridPosY;
				this._gridPosZ = from._gridPosZ;

				this._gridSpanX = from._gridSpanX;
				this._gridSpanY = from._gridSpanY;
				this._gridSpanZ = from._gridSpanZ;

				this._rotationX = from._rotationX;
				this._rotationY = from._rotationY;
				this._rotationZ = from._rotationZ;

				this._shearX = from._shearX;
				this._shearY = from._shearY;
				this._shearZ = from._shearZ;

				this._scaleX = from._scaleX;
				this._scaleY = from._scaleY;
				this._scaleZ = from._scaleZ;

				this._forceFitIntoCell = from._forceFitIntoCell;
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
				this._scaleY = from.ScaleY;

				EhSelfChanged();
				return true;
			}
			return false;
		}

		object System.ICloneable.Clone()
		{
			return new ItemLocationByGrid(this);
		}

		public ItemLocationByGrid Clone()
		{
			return new ItemLocationByGrid(this);
		}

		public RectangleD3D GetAbsolute(GridPartitioning partition, VectorD3D parentSize)
		{
			return partition.GetTileRectangle(_gridPosX, _gridPosY, _gridPosZ, _gridSpanX, _gridSpanY, _gridSpanZ, parentSize);
		}

		/// <summary>
		/// The grid column.
		/// </summary>
		public double GridPosX
		{
			get { return _gridPosX; }
			set
			{
				double oldvalue = _gridPosX;
				_gridPosX = value;

				if (value != oldvalue)
					EhSelfChanged();
			}
		}

		/// <summary>
		/// The grid column.
		/// </summary>
		public double GridPosY
		{
			get { return _gridPosY; }
			set
			{
				double oldvalue = _gridPosY;
				_gridPosY = value;

				if (value != oldvalue)
					EhSelfChanged();
			}
		}

		/// <summary>
		/// The grid row.
		/// </summary>
		public double GridPosZ
		{
			get { return _gridPosZ; }
			set
			{
				double oldvalue = _gridPosZ;
				_gridPosZ = value;

				if (value != oldvalue)
					EhSelfChanged();
			}
		}

		/// <summary>
		/// The column span that determines the horizontal size.
		/// </summary>
		public double GridSpanX
		{
			get { return _gridSpanX; }
			set
			{
				double oldvalue = _gridSpanX;
				_gridSpanX = value;

				if (value != oldvalue)
					EhSelfChanged();
			}
		}

		/// <summary>
		/// The column span that determines the horizontal size.
		/// </summary>
		public double GridSpanY
		{
			get { return _gridSpanY; }
			set
			{
				double oldvalue = _gridSpanY;
				_gridSpanY = value;

				if (value != oldvalue)
					EhSelfChanged();
			}
		}

		/// <summary>
		/// The column span that determines the horizontal size.
		/// </summary>
		public double GridSpanZ
		{
			get { return _gridSpanZ; }
			set
			{
				double oldvalue = _gridSpanZ;
				_gridSpanZ = value;

				if (value != oldvalue)
					EhSelfChanged();
			}
		}

		/// <summary>
		/// If true, the scale and rotation is taken into account, so that the object fits exactly into the grid cell.
		/// </summary>
		/// <value>
		/// <c>true</c> if scale and angle are taken into account to calculate the cell size; otherwise, <c>false</c>.
		/// </value>
		public bool ForceFitIntoCell
		{
			get { return _forceFitIntoCell; }
			set
			{
				var oldvalue = _forceFitIntoCell;
				_forceFitIntoCell = value;

				if (value != oldvalue)
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

		/// <summary>The scaling factor of the layer, normally 1.</summary>
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

		/// <summary>The scaling factor of the layer, normally 1.</summary>
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

		/// <summary>The scaling factor of the layer, normally 1.</summary>
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

		/// <summary>The scaling factor of the layer, normally 1.</summary>
		public double ScaleX
		{
			get { return _scaleX; }
			set
			{
				double oldvalue = _scaleX;
				_scaleX = value;
				if (value != oldvalue)
					EhSelfChanged();
			}
		}

		/// <summary>The scaling factor of the layer, normally 1.</summary>
		public double ScaleY
		{
			get { return _scaleY; }
			set
			{
				double oldvalue = _scaleY;
				_scaleY = value;
				if (value != oldvalue)
					EhSelfChanged();
			}
		}

		/// <summary>The scaling factor of the layer, normally 1.</summary>
		public double ScaleZ
		{
			get { return _scaleZ; }
			set
			{
				double oldvalue = _scaleZ;
				_scaleZ = value;
				if (value != oldvalue)
					EhSelfChanged();
			}
		}
	}
}