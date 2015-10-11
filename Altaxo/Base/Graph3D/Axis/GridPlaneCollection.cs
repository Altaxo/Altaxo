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

using Altaxo.Graph;
using Altaxo.Graph3D.GraphicsContext;
using System;
using System.Collections.Generic;
using System.Text;

namespace Altaxo.Graph3D.Axis
{
	[Serializable]
	public class GridPlaneCollection
		:
		Main.SuspendableDocumentNodeWithSetOfEventArgs,
		IEnumerable<GridPlane>,
		ICloneable
	{
		private List<GridPlane> _innerList = new List<GridPlane>();

		private void CopyFrom(GridPlaneCollection from)
		{
			if (object.ReferenceEquals(this, from))
				return;

			this.Clear();

			foreach (GridPlane plane in from)
				this.Add((GridPlane)plane.Clone());
		}

		#region Serialization

		#region Version 0

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(GridPlaneCollection), 0)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				GridPlaneCollection s = (GridPlaneCollection)obj;

				info.CreateArray("GridPlanes", s.Count);
				foreach (GridPlane plane in s)
					info.AddValue("e", plane);
				info.CommitArray();
			}

			protected virtual GridPlaneCollection SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				GridPlaneCollection s = (o == null ? new GridPlaneCollection() : (GridPlaneCollection)o);

				int count = info.OpenArray("GridPlanes");
				for (int i = 0; i < count; i++)
				{
					GridPlane plane = (GridPlane)info.GetValue("e", s);
					s.Add(plane);
				}
				info.CloseArray(count);

				return s;
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				GridPlaneCollection s = SDeserialize(o, info, parent);
				return s;
			}
		}

		#endregion Version 0

		#endregion Serialization

		public GridPlaneCollection()
		{
		}

		public GridPlaneCollection(GridPlaneCollection from)
		{
			CopyFrom(from);
		}

		protected override IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
		{
			int i = -1;
			foreach (var plane in _innerList)
			{
				++i;
				if (null != plane)
					yield return new Main.DocumentNodeAndName(plane, i.ToString(System.Globalization.CultureInfo.InvariantCulture));
			}
		}

		public GridPlaneCollection Clone()
		{
			return new GridPlaneCollection(this);
		}

		object ICloneable.Clone()
		{
			return new GridPlaneCollection(this);
		}

		public int Count { get { return _innerList.Count; } }

		public GridPlane this[int idx]
		{
			get
			{
				return _innerList[idx];
			}
		}

		public GridPlane this[CSPlaneID planeid]
		{
			get
			{
				foreach (GridPlane plane in _innerList)
				{
					if (plane.PlaneID == planeid)
						return plane;
				}
				return null;
			}
			set
			{
				for (int i = 0; i < Count; i++)
				{
					if (_innerList[i].PlaneID == planeid)
					{
						if (value == null)
							_innerList.RemoveAt(i);
						else
							_innerList[i] = value;
						return;
					}
				}
				// if not found, we add the value to the collection
				if (null != value)
					Add(value);
			}
		}

		public void Add(GridPlane plane)
		{
			if (null == plane)
				throw new ArgumentNullException("plane");

			plane.ParentObject = this;
			_innerList.Add(plane);
		}

		public void Clear()
		{
			var list = _innerList;
			_innerList = new List<GridPlane>();
			foreach (GridPlane plane in list)
				plane.Dispose();
		}

		public void RemoveUnused()
		{
			for (int i = _innerList.Count - 1; i >= 0; i--)
			{
				var item = _innerList[i];
				if (!item.IsUsed)
				{
					_innerList.RemoveAt(i);
					item.Dispose();
				}
			}
		}

		public bool Contains(CSPlaneID planeid)
		{
			foreach (GridPlane plane in _innerList)
			{
				if (plane.PlaneID == planeid)
					return true;
			}
			return false;
		}

		public void Paint(IGraphicContext3D g, IPlotArea3D layer)
		{
			for (int i = 0; i < _innerList.Count; ++i)
				_innerList[i].Paint(g, layer);
		}

		/// <summary>
		/// Paints only the background of all planes (but not the grid).
		/// </summary>
		/// <param name="g">The graphics context.</param>
		/// <param name="layer">The layer.</param>
		public void PaintBackground(IGraphicContext3D g, IPlotArea3D layer)
		{
			for (int i = 0; i < _innerList.Count; ++i)
				_innerList[i].PaintBackground(g, layer);
		}

		/// <summary>
		/// Paints the grid of all planes, but not the background.
		/// </summary>
		/// <param name="g">The g.</param>
		/// <param name="layer">The layer.</param>
		public void PaintGrid(IGraphicContext3D g, IPlotArea3D layer)
		{
			for (int i = 0; i < _innerList.Count; ++i)
				_innerList[i].PaintGrid(g, layer);
		}

		#region IEnumerable<GridPlane> Members

		public IEnumerator<GridPlane> GetEnumerator()
		{
			return _innerList.GetEnumerator();
		}

		#endregion IEnumerable<GridPlane> Members

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return _innerList.GetEnumerator();
		}

		#endregion IEnumerable Members
	}
}