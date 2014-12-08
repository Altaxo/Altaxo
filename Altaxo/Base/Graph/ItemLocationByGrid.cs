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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Graph
{
	public class ItemLocationByGrid : Main.SuspendableDocumentLeafNodeWithEventArgs, IItemLocation
	{
		/// <summary>
		/// The number of the grid column where the cell starts horizontal.
		/// </summary>
		private double _gridColumn = 1;

		/// <summary>The number of the grid row where the cell starts vertically.</summary>
		private double _gridRow = 1;

		/// <summary>
		/// The number of grid columns that the cell spans horizontally.
		/// </summary>
		private double _gridColumnSpan = 1;

		/// <summary>
		/// The number of grid rows that the cell spans vertically.
		/// </summary>
		private double _gridRowSpan = 1;

		/// <summary>
		/// If true, the angle and scale is taken into account, so that the layer fits exactly into the grid cell.
		/// </summary>
		private bool _forceFitIntoCell;

		/// <summary>The rotation angle (in degrees) of the layer.</summary>
		private double _rotation = 0; // Rotation

		/// <summary>The shear factor of the layer, normally 0.</summary>
		private double _shear = 0;

		/// <summary>The scaling factor of the layer, normally 1.</summary>
		private double _scaleX = 1;  // Scale

		/// <summary>The scaling factor of the layer, normally 1.</summary>
		private double _scaleY = 1;  // Scale

		#region Serialization

		/// <summary>
		/// 2013-12-01 initial version.
		/// </summary>
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ItemLocationByGrid), 0)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				var s = (ItemLocationByGrid)obj;

				info.AddValue("Column", s._gridColumn);
				info.AddValue("Row", s._gridRow);
				info.AddValue("ColumnSpan", s._gridColumnSpan);
				info.AddValue("RowSpan", s._gridRowSpan);
				info.AddValue("Rotation", s._rotation);
				info.AddValue("ShearX", s._shear);
				info.AddValue("ScaleX", s._scaleX);
				info.AddValue("ScaleY", s._scaleY);
				info.AddValue("ForceFitIntoCell", s._forceFitIntoCell);
			}

			protected virtual ItemLocationByGrid SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				var s = null != o ? (ItemLocationByGrid)o : new ItemLocationByGrid();

				s._gridColumn = info.GetDouble("Column");
				s._gridRow = info.GetDouble("Row");
				s._gridColumnSpan = info.GetDouble("ColumnSpan");
				s._gridRowSpan = info.GetDouble("RowSpan");
				s._rotation = info.GetDouble("Rotation");
				s._shear = info.GetDouble("ShearX");
				s._scaleX = info.GetDouble("ScaleX");
				s._scaleY = info.GetDouble("ScaleY");
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
				this._gridColumn = from._gridColumn;
				this._gridRow = from._gridRow;
				this._gridColumnSpan = from._gridColumnSpan;
				this._gridRowSpan = from._gridRowSpan;

				this._rotation = from._rotation;
				this._shear = from._shear;
				this._scaleX = from._scaleX;
				this._scaleY = from._scaleY;
				this._forceFitIntoCell = from._forceFitIntoCell;
				EhSelfChanged();
				return true;
			}
			else if (obj is IItemLocation)
			{
				var from = (IItemLocation)obj;
				this._rotation = from.Rotation;
				this._shear = from.ShearX;
				this._scaleX = from.ScaleX;
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

		public RectangleD GetAbsolute(GridPartitioning partition, PointD2D parentSize)
		{
			return partition.GetTileRectangle(_gridColumn, _gridRow, _gridColumnSpan, _gridRowSpan, parentSize);
		}

		/// <summary>
		/// The grid column.
		/// </summary>
		public double GridColumn
		{
			get { return _gridColumn; }
			set
			{
				double oldvalue = _gridColumn;
				_gridColumn = value;

				if (value != oldvalue)
					EhSelfChanged();
			}
		}

		/// <summary>
		/// The grid row.
		/// </summary>
		public double GridRow
		{
			get { return _gridRow; }
			set
			{
				double oldvalue = _gridRow;
				_gridRow = value;

				if (value != oldvalue)
					EhSelfChanged();
			}
		}

		/// <summary>
		/// The column span that determines the horizontal size.
		/// </summary>
		public double GridColumnSpan
		{
			get { return _gridColumnSpan; }
			set
			{
				double oldvalue = _gridColumnSpan;
				_gridColumnSpan = value;

				if (value != oldvalue)
					EhSelfChanged();
			}
		}

		/// <summary>
		/// The row span that determines the vertical size.
		/// </summary>
		public double GridRowSpan
		{
			get { return _gridRowSpan; }
			set
			{
				double oldvalue = _gridRowSpan;
				_gridRowSpan = value;

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
		public double Rotation
		{
			get { return _rotation; }
			set
			{
				double oldvalue = _rotation;
				_rotation = value;

				if (value != oldvalue)
					EhSelfChanged();
			}
		}

		/// <summary>The scaling factor of the layer, normally 1.</summary>
		public double ShearX
		{
			get { return _shear; }
			set
			{
				double oldvalue = _shear;
				_shear = value;
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

		#region IDocumentNode Members

		public override string Name
		{
			get { return "Grid position/size"; }
		}

		#endregion IDocumentNode Members
	}
}