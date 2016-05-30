#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2016 Dr. Dirk Lellinger
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
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Altaxo.Graph.Graph3D.Plot.Styles
{
	using Altaxo.Main;
	using Data;
	using Geometry;
	using Graph.Plot.Groups;
	using GraphicsContext;
	using Plot.Groups;

	public class G3DPlotStyleCollection
		:
		Main.SuspendableDocumentNodeWithSingleAccumulatedData<PlotItemStyleChangedEventArgs>,
		IEnumerable<IG3DPlotStyle>,
		IG3DPlotStyle,
		IRoutedPropertyReceiver
	{
		/// <summary>
		/// Holds the plot styles
		/// </summary>
		private List<IG3DPlotStyle> _innerList;

		#region Serialization

		/// <summary>
		/// 2016-05-30 Initial version
		/// </summary>
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(G3DPlotStyleCollection), 0)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				var s = (G3DPlotStyleCollection)obj;

				info.CreateArray("Styles", s._innerList.Count);
				for (int i = 0; i < s._innerList.Count; i++)
					info.AddValue("e", s._innerList[i]);
				info.CommitArray();
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				int count = info.OpenArray();
				var array = new IG3DPlotStyle[count];
				for (int i = 0; i < count; i++)
					array[i] = (IG3DPlotStyle)info.GetValue("e", null);
				info.CloseArray(count);

				if (o == null)
				{
					return new G3DPlotStyleCollection(array);
				}
				else
				{
					var s = (G3DPlotStyleCollection)o;
					for (int i = 0; i < count; i++)
						s.Add(array[i]);
					return s;
				}
			}
		}

		#endregion Serialization

		/// <summary>
		/// Creates an empty collection, i.e. without any styles (so the item is not visible). You must manually add styles to make the plot item visible.
		/// </summary>
		public G3DPlotStyleCollection()
		{
			_innerList = new List<IG3DPlotStyle>();
		}

		public G3DPlotStyleCollection(IG3DPlotStyle[] styles)
		{
			_innerList = new List<IG3DPlotStyle>();
			for (int i = 0; i < styles.Length; ++i)
				if (styles[i] != null)
					this.Add(styles[i], false);
		}

		public G3DPlotStyleCollection(G3DPlotStyleCollection from)
		{
			CopyFrom(from);
		}

		public void CopyFrom(G3DPlotStyleCollection from)
		{
			if (object.ReferenceEquals(this, from))
				return;

			using (var suspendToken = SuspendGetToken())
			{
				Clear();

				this._innerList = new List<IG3DPlotStyle>();
				for (int i = 0; i < from._innerList.Count; ++i)
					Add((IG3DPlotStyle)from[i].Clone());

				suspendToken.Resume();
			}
		}

		public bool CopyFrom(object obj)
		{
			if (object.ReferenceEquals(this, obj))
				return true;
			var from = obj as G3DPlotStyleCollection;
			if (null != from)
			{
				CopyFrom(from);
				return true;
			}
			return false;
		}

		protected override IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
		{
			if (null != _innerList)
			{
				for (int i = _innerList.Count - 1; i >= 0; --i)
				{
					if (null != _innerList[i])
						yield return new Main.DocumentNodeAndName(_innerList[i], "Style" + i.ToString(System.Globalization.CultureInfo.InvariantCulture));
				}
			}
		}

		public void SetFromTemplate(G3DPlotStyleCollection from, PlotGroupStrictness strictness)
		{
			if (strictness == PlotGroupStrictness.Strict)
			{
				CopyFrom(from);
			}
			else if (strictness == PlotGroupStrictness.Exact)
			{
				// note one sub style in the 'from' collection can update only one item in the 'this' collection
				using (var suspendToken = SuspendGetToken())
				{
					int myidx = 0;
					foreach (IG3DPlotStyle style in from)
					{
						for (int i = myidx; i < this.Count; i++)
						{
							if (this[i].GetType() == style.GetType())
							{
								Replace((IG3DPlotStyle)from[i].Clone(), i, false);
								myidx = i + 1;
								break;
							}
						}
					}
					suspendToken.Resume();
				}
			}
		}

		public IG3DPlotStyle this[int i]
		{
			get
			{
				return _innerList[i];
			}
		}

		public int Count
		{
			get
			{
				return _innerList.Count;
			}
		}

		public void Add(IG3DPlotStyle toadd)
		{
			Add(toadd, true);
		}

		protected void Add(IG3DPlotStyle toadd, bool withReorganizationAndEvents)
		{
			if (toadd != null)
			{
				this._innerList.Add(toadd);
				toadd.ParentObject = this;

				if (withReorganizationAndEvents)
				{
					EhSelfChanged(EventArgs.Empty);
				}
			}
		}

		protected void Replace(IG3DPlotStyle ps, int idx, bool withReorganizationAndEvents)
		{
			if (ps != null)
			{
				this._innerList[idx] = ps;
				ps.ParentObject = this;

				if (withReorganizationAndEvents)
				{
					EhSelfChanged(EventArgs.Empty);
				}
			}
		}

		public void AddRange(IG3DPlotStyle[] toadd)
		{
			if (toadd != null)
			{
				for (int i = 0; i < toadd.Length; i++)
				{
					this._innerList.Add(toadd[i]);
					toadd[i].ParentObject = this;
				}

				EhSelfChanged(EventArgs.Empty);
			}
		}

		public void Insert(int whichposition, IG3DPlotStyle toinsert)
		{
			if (toinsert != null)
			{
				this._innerList.Insert(whichposition, toinsert);
				toinsert.ParentObject = this;

				EhSelfChanged(EventArgs.Empty);
			}
		}

		public void Clear()
		{
			if (_innerList != null)
			{
				this._innerList.Clear();

				EhSelfChanged(EventArgs.Empty);
			}
		}

		public void RemoveAt(int idx)
		{
			var removed = this[idx];
			_innerList.RemoveAt(idx);

			EhSelfChanged(EventArgs.Empty);
		}

		public void ExchangeItemPositions(int pos1, int pos2)
		{
			var item1 = this[pos1];
			_innerList[pos1] = _innerList[pos2];
			_innerList[pos2] = item1;

			EhSelfChanged(EventArgs.Empty);
		}

		protected override void AccumulateChangeData(object sender, EventArgs e)
		{
			_accumulatedEventData = PlotItemStyleChangedEventArgs.Empty;
		}

		object ICloneable.Clone()
		{
			return new G3DPlotStyleCollection(this);
		}

		public G3DPlotStyleCollection Clone()
		{
			return new G3DPlotStyleCollection(this);
		}

		public void Paint(IGraphicsContext3D g, IPlotArea layer, Processed3DPlotData pdata, Processed3DPlotData prevItemData, Processed3DPlotData nextItemData)
		{
			for (int i = _innerList.Count - 1; i >= 0; i--)
			{
				this[i].Paint(g, layer, pdata, prevItemData, nextItemData);
			}
		}

		public RectangleD3D PaintSymbol(IGraphicsContext3D g, RectangleD3D bounds)
		{
			for (int i = _innerList.Count - 1; i >= 0; i--)
			{
				bounds = this[i].PaintSymbol(g, bounds);
			}

			return bounds;
		}

		/// <summary>
		/// Distibute changes made to one group style of the collection (at index <c>pivot</c> to all other members of the collection.
		/// </summary>
		/// <param name="pivot">Index of the group style that was changed. This style keeps it's properties.</param>
		/// <param name="layer"></param>
		/// <param name="pdata"></param>
		public void DistributeSubStyleChange(int pivot, IPlotArea layer, Processed3DPlotData pdata)
		{
			PlotGroupStyleCollection externGroup = new PlotGroupStyleCollection();
			PlotGroupStyleCollection localGroup = new PlotGroupStyleCollection();
			// because we don't step, the order is essential only for PrepareStyles
			for (int i = 0; i < _innerList.Count; i++)
				CollectLocalGroupStyles(externGroup, localGroup);

			// prepare
			this[pivot].PrepareGroupStyles(externGroup, localGroup, layer, pdata);
			for (int i = 0; i < Count; i++)
				if (i != pivot)
					this[i].PrepareGroupStyles(externGroup, localGroup, layer, pdata);

			// apply
			this[pivot].ApplyGroupStyles(externGroup, localGroup);
			for (int i = 0; i < Count; i++)
				if (i != pivot)
					this[i].ApplyGroupStyles(externGroup, localGroup);
		}

		/// <summary>
		/// Prepares a new substyle (one that is not already in the collection) for becoming member of the collection. The substyle will get
		/// all distributes group properties (local only) of this style collection.
		/// </summary>
		/// <param name="newSubStyle">Sub style to prepare.</param>
		/// <param name="layer"></param>
		/// <param name="pdata"></param>
		public void PrepareNewSubStyle(IG3DPlotStyle newSubStyle, IPlotArea layer, Processed3DPlotData pdata)
		{
			PlotGroupStyleCollection externGroup = new PlotGroupStyleCollection();
			PlotGroupStyleCollection localGroup = new PlotGroupStyleCollection();
			// because we don't step, the order is essential only for PrepareStyles
			for (int i = 0; i < _innerList.Count; i++)
				this[i].CollectLocalGroupStyles(externGroup, localGroup);
			newSubStyle.CollectLocalGroupStyles(externGroup, localGroup);

			// prepare
			for (int i = 0; i < Count; i++)
				this[i].PrepareGroupStyles(externGroup, localGroup, layer, pdata);
			newSubStyle.PrepareGroupStyles(externGroup, localGroup, layer, pdata);

			// apply
			for (int i = 0; i < Count; i++)
				this[i].ApplyGroupStyles(externGroup, localGroup);
			newSubStyle.ApplyGroupStyles(externGroup, localGroup);
		}

		#region IEnumerable<IPlotStyle> Members

		public IEnumerator<IG3DPlotStyle> GetEnumerator()
		{
			return _innerList.GetEnumerator();
		}

		#endregion IEnumerable<IPlotStyle> Members

		#region IEnumerable Members

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _innerList.GetEnumerator();
		}

		#endregion IEnumerable Members

		#region IPlotStyle Members

		public void CollectExternalGroupStyles(PlotGroupStyleCollection externalGroups)
		{
			foreach (var ps in this)
				ps.CollectExternalGroupStyles(externalGroups);
		}

		public void CollectLocalGroupStyles(PlotGroupStyleCollection externalGroups, PlotGroupStyleCollection localGroups)
		{
			foreach (var ps in this)
				ps.CollectLocalGroupStyles(externalGroups, localGroups);
		}

		public void PrepareGroupStyles(PlotGroupStyleCollection externalGroups, PlotGroupStyleCollection localGroups, IPlotArea layer, Processed3DPlotData pdata)
		{
			foreach (var ps in this)
				ps.PrepareGroupStyles(externalGroups, localGroups, layer, pdata);
		}

		public void ApplyGroupStyles(PlotGroupStyleCollection externalGroups, PlotGroupStyleCollection localGroups)
		{
			foreach (var ps in this)
				ps.ApplyGroupStyles(externalGroups, localGroups);
		}

		#endregion IPlotStyle Members

		#region IDocumentNode Members

		/// <summary>
		/// Replaces path of items (intended for data items like tables and columns) by other paths. Thus it is possible
		/// to change a plot so that the plot items refer to another table.
		/// </summary>
		/// <param name="options">Information what to replace.</param>
		public void VisitDocumentReferences(DocNodeProxyReporter options)
		{
			foreach (var s in this)
				s.VisitDocumentReferences(options);
		}

		#endregion IDocumentNode Members

		#region IRoutedPropertyReceiver Members

		public void SetRoutedProperty(IRoutedSetterProperty property)
		{
			foreach (object o in _innerList)
			{
				if (o is IRoutedPropertyReceiver)
					(o as IRoutedPropertyReceiver).SetRoutedProperty(property);
			}
		}

		public void GetRoutedProperty(IRoutedGetterProperty property)
		{
			foreach (object o in _innerList)
			{
				if (o is IRoutedPropertyReceiver)
					(o as IRoutedPropertyReceiver).GetRoutedProperty(property);
			}
		}

		#endregion IRoutedPropertyReceiver Members

		string INamedObject.Name
		{
			get { return this.GetType().Name; }
		}
	}
}