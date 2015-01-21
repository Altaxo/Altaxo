#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2014 Dr. Dirk Lellinger
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
using Altaxo.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Altaxo.Graph
{
	public class LinearPartitioning
		:
		Main.SuspendableDocumentLeafNodeWithSetOfEventArgs,
		IList<RADouble>,
		ICollection<RADouble>,
		IEnumerable<RADouble>,
		IList,
		ICollection,
		IEnumerable,
		INotifyCollectionChanged, INotifyPropertyChanged,
		ICloneable
	{
		private System.Collections.ObjectModel.ObservableCollection<RADouble> _innerList;

		#region Serialization

		#region Version 0

		/// <summary>
		/// 2013-09-25 initial version.
		/// </summary>
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(LinearPartitioning), 0)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				var s = (LinearPartitioning)obj;

				info.CreateArray("Partitioning", s.Count);
				foreach (var v in s)
					info.AddValue("e", v);
				info.CommitArray();
			}

			protected virtual LinearPartitioning SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				var s = (o == null ? new LinearPartitioning() : (LinearPartitioning)o);

				int count = info.OpenArray("Partitioning");
				for (int i = 0; i < count; ++i)
					s.Add((RADouble)info.GetValue("e", s));
				info.CloseArray(count);

				return s;
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				var s = SDeserialize(o, info, parent);
				return s;
			}
		}

		#endregion Version 0

		#endregion Serialization

		public LinearPartitioning()
		{
			_innerList = new System.Collections.ObjectModel.ObservableCollection<RADouble>();
			_innerList.CollectionChanged += EhInnerList_CollectionChanged;
			((INotifyPropertyChanged)_innerList).PropertyChanged += EhInnerList_PropertyChanged;
		}

		public LinearPartitioning(IEnumerable<RADouble> list)
		{
			_innerList = new System.Collections.ObjectModel.ObservableCollection<RADouble>(list);
			_innerList.CollectionChanged += EhInnerList_CollectionChanged;
			((INotifyPropertyChanged)_innerList).PropertyChanged += EhInnerList_PropertyChanged;
		}

		public object Clone()
		{
			return new LinearPartitioning(this._innerList);
		}

		#region event handling

		private void EhInnerList_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			EhSelfChanged(e);
		}

		private void EhInnerList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			EhSelfChanged(e);
		}

		protected override void OnChanged(EventArgs e)
		{
			var e1 = e as NotifyCollectionChangedEventArgs;
			if (null != e1)
			{
				var ev = CollectionChanged;
				if (null != ev)
					ev(this, e1);
			}

			var e2 = e as PropertyChangedEventArgs;
			if (null != e2)
			{
				var ev = PropertyChanged;
				if (null != ev)
					ev(this, e2);
			}

			base.OnChanged(e);
		}

		#endregion event handling

		public double[] GetPartitionPositions(double totalSize)
		{
			double relSum = this.Sum(x => x.IsRelative ? x.Value : 0);
			double absSum = this.Sum(x => x.IsAbsolute ? x.Value : 0);

			double absValuePerRelativeValue = (totalSize - absSum) / relSum;

			int i = -1;
			double position = 0;
			double[] result = new double[this.Count];
			foreach (var x in this)
			{
				position += x.IsAbsolute ? x.Value : x.Value * absValuePerRelativeValue;
				result[++i] = position;
			}
			return result;
		}

		/// <summary>
		/// Gets the partition position. A relative value of 0 gives the absolute position 0, a value of 1 gives the size of the first partition, a value of two the size of the first plus second partition and so on.
		/// </summary>
		/// <param name="relativeValue">The relative value.</param>
		/// <param name="totalSize">The total size.</param>
		/// <returns></returns>
		public double GetPartitionPosition(double relativeValue, double totalSize)
		{
			var partPositions = GetPartitionPositions(totalSize);

			if (relativeValue < 0)
			{
				return relativeValue * totalSize;
			}
			else if (relativeValue < partPositions.Length)
			{
				int rlower = (int)Math.Floor(relativeValue);
				double r = relativeValue - rlower;
				double pl = rlower == 0 ? 0 : partPositions[rlower - 1];
				double pu = partPositions[rlower];
				return pl * (1 - r) + r * pu;
			}
			else if (relativeValue >= partPositions.Length)
			{
				return (relativeValue - partPositions.Length) * totalSize + totalSize;
			}
			else
			{
				return double.NaN;
			}
		}

		public void GetTilePositionSize(double start, double span, double totalSize, out double absoluteStart, out double absoluteSize)
		{
			absoluteStart = GetPartitionPosition(start, totalSize);
			absoluteSize = GetPartitionPosition(start + span, totalSize) - absoluteStart;
		}

		public double GetSumRelativeValues()
		{
			return this.Sum(x => x.IsRelative ? x.Value : 0);
		}

		#region ICollection<RADouble>

		public void Add(RADouble item)
		{
			_innerList.Add(item);
		}

		public void Clear()
		{
			_innerList.Clear();
		}

		public bool Contains(RADouble item)
		{
			return _innerList.Contains(item);
		}

		public void CopyTo(RADouble[] array, int arrayIndex)
		{
			_innerList.CopyTo(array, arrayIndex);
		}

		public int Count
		{
			get { return _innerList.Count; }
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		public bool Remove(RADouble item)
		{
			return _innerList.Remove(item);
		}

		public IEnumerator<RADouble> GetEnumerator()
		{
			return _innerList.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return _innerList.GetEnumerator();
		}

		#endregion ICollection<RADouble>

		#region INotifyCollectionChanged

		public event NotifyCollectionChangedEventHandler CollectionChanged;

		#endregion INotifyCollectionChanged

		#region INotifyPropertyChanged

		public event PropertyChangedEventHandler PropertyChanged;

		#endregion INotifyPropertyChanged

		#region IList<RADouble>

		public int IndexOf(RADouble item)
		{
			return _innerList.IndexOf(item);
		}

		public void Insert(int index, RADouble item)
		{
			_innerList.Insert(index, item);
		}

		public void RemoveAt(int index)
		{
			RemoveAt(index);
		}

		public RADouble this[int index]
		{
			get
			{
				return _innerList[index];
			}
			set
			{
				_innerList[index] = value;
			}
		}

		#endregion IList<RADouble>

		#region IList

		public int Add(object value)
		{
			return ((IList)_innerList).Add(value);
		}

		public bool Contains(object value)
		{
			return ((IList)_innerList).Contains(value);
		}

		public int IndexOf(object value)
		{
			return ((IList)_innerList).IndexOf(value);
		}

		public void Insert(int index, object value)
		{
			((IList)_innerList).Insert(index, value);
		}

		public bool IsFixedSize
		{
			get { return ((IList)_innerList).IsFixedSize; }
		}

		public void Remove(object value)
		{
			((IList)_innerList).Remove(value);
		}

		object IList.this[int index]
		{
			get
			{
				return _innerList[index];
			}
			set
			{
				((IList)_innerList)[index] = value;
			}
		}

		public void CopyTo(Array array, int index)
		{
			((IList)_innerList).CopyTo(array, index);
		}

		public bool IsSynchronized
		{
			get { return ((IList)_innerList).IsSynchronized; }
		}

		public object SyncRoot
		{
			get { return ((IList)_innerList).SyncRoot; }
		}

		#endregion IList
	}

	public class GridPartitioning
		:
		Main.SuspendableDocumentNodeWithSetOfEventArgs,
		Main.ICopyFrom
	{
		private LinearPartitioning _xPartitioning;
		private LinearPartitioning _yPartitioning;

		#region Serialization

		#region Version 0

		/// <summary>
		/// 2013-09-25 initial version.
		/// </summary>
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(GridPartitioning), 0)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				var s = (GridPartitioning)obj;

				info.AddValue("XPartitioning", s._xPartitioning);
				info.AddValue("YPartitioning", s._yPartitioning);
			}

			protected virtual GridPartitioning SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				var s = (o == null ? new GridPartitioning() : (GridPartitioning)o);

				s._xPartitioning = (LinearPartitioning)info.GetValue("XPartitioning", s);
				if (null != s._xPartitioning) s._xPartitioning.ParentObject = s;

				s._yPartitioning = (LinearPartitioning)info.GetValue("YPartitioning", s);
				if (null != s._yPartitioning) s._yPartitioning.ParentObject = s;

				return s;
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				var s = SDeserialize(o, info, parent);
				return s;
			}
		}

		#endregion Version 0

		#endregion Serialization

		public GridPartitioning()
		{
			_xPartitioning = new LinearPartitioning() { ParentObject = this };
			_yPartitioning = new LinearPartitioning() { ParentObject = this };
		}

		public GridPartitioning(GridPartitioning from)
		{
			_xPartitioning = new LinearPartitioning() { ParentObject = this };
			_yPartitioning = new LinearPartitioning() { ParentObject = this };
			CopyFrom(from);
		}

		public bool CopyFrom(object obj)
		{
			if (object.ReferenceEquals(this, obj))
				return true;

			var from = obj as GridPartitioning;
			if (null != from)
			{
				using (var suspendToken = SuspendGetToken())
				{
					ChildCopyToMember(ref _xPartitioning, from._xPartitioning);
					ChildCopyToMember(ref _yPartitioning, from._yPartitioning);

					suspendToken.Resume();
				}
				return true;
			}
			return false;
		}

		public object Clone()
		{
			return new GridPartitioning(this);
		}

		protected override IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
		{
			if (null != _xPartitioning)
				yield return new Main.DocumentNodeAndName(_xPartitioning, () => _xPartitioning = null, "XPartitioning");
			if (null != _yPartitioning)
				yield return new Main.DocumentNodeAndName(_yPartitioning, () => _yPartitioning = null, "YPartitioning");
		}

		public LinearPartitioning XPartitioning { get { return _xPartitioning; } }

		public LinearPartitioning YPartitioning { get { return _yPartitioning; } }

		public bool IsEmpty { get { return _xPartitioning.Count == 0 && _yPartitioning.Count == 0; } }

		public RectangleD GetTileRectangle(double column, double row, double columnSpan, double rowSpan, PointD2D totalSize)
		{
			double xstart, xsize;
			double ystart, ysize;
			_xPartitioning.GetTilePositionSize(column, columnSpan, totalSize.X, out xstart, out xsize);
			_yPartitioning.GetTilePositionSize(row, rowSpan, totalSize.Y, out ystart, out ysize);
			return new RectangleD(xstart, ystart, xsize, ysize);
		}
	}
}