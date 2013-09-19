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
	public class ItemLocationByGrid : IItemLocation
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
		private bool _takeScaleAngleIntoAccountToFitInCell;

		/// <summary>The rotation angle (in degrees) of the layer.</summary>
		private double _rotation = 0; // Rotation

		/// <summary>The scaling factor of the layer, normally 1.</summary>
		private double _scale = 1;  // Scale

		[field: NonSerialized]
		private event EventHandler _changed;

		[NonSerialized]
		private object _parent;

		#region Serialization

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
				info.AddValue("ScaleX", s._scale);
				info.AddValue("ScaleY", s._scale);
				info.AddValue("Shear", 0);
				info.AddValue("ForceFitInCell", s._takeScaleAngleIntoAccountToFitInCell);
			}

			protected virtual ItemLocationByGrid SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				var s = null != o ? (ItemLocationByGrid)o : new ItemLocationByGrid();

				s._gridColumn = info.GetDouble("Column");
				s._gridRow = info.GetDouble("Row");
				s._gridColumnSpan = info.GetDouble("ColumnSpan");
				s._gridRowSpan = info.GetDouble("RowSpan");
				s._rotation = info.GetDouble("Rotation");
				s._scale = info.GetDouble("ScaleX");
				var scaleY = info.GetDouble("ScaleY");
				var shear = info.GetDouble("Shear");

				s._takeScaleAngleIntoAccountToFitInCell = info.GetBoolean("ForceFitInCell");

				if (shear != 0 || scaleY != s._scale)
					throw new NotImplementedException("Shear or ScaleY not implemented yet.");

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
					OnChanged();
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
					OnChanged();
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
					OnChanged();
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
					OnChanged();
			}
		}

		/// <summary>
		/// If true, the scale and rotation is taken into account, so that the object fits exactly into the grid cell.
		/// </summary>
		/// <value>
		/// <c>true</c> if scale and angle are taken into account to calculate the cell size; otherwise, <c>false</c>.
		/// </value>
		public bool TakeScaleAngleIntoAccountToFitInCell
		{
			get { return _takeScaleAngleIntoAccountToFitInCell; }
			set
			{
				var oldvalue = _takeScaleAngleIntoAccountToFitInCell;
				_takeScaleAngleIntoAccountToFitInCell = value;

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
			get { return "Grid position/size"; }
		}

		#endregion IDocumentNode Members
	}
}