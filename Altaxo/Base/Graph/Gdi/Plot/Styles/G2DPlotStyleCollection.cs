#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
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
using System.Drawing;
using System.Text;

namespace Altaxo.Graph.Gdi.Plot.Styles
{
	using Altaxo.Data;
	using Altaxo.Main;
	using Collections;
	using Data;
	using Graph.Plot.Groups;
	using Plot.Groups;

	public class G2DPlotStyleCollection
			:
			Main.SuspendableDocumentNodeWithSingleAccumulatedData<PlotItemStyleChangedEventArgs>,
			IEnumerable<IG2DPlotStyle>,
			IG2DPlotStyle
	{
		/// <summary>
		/// Holds the plot styles
		/// </summary>
		private List<IG2DPlotStyle> _innerList;

		#region Serialization

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.XYPlotStyleCollection", 0)]
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(G2DPlotStyleCollection), 1)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				throw new NotSupportedException("Serialization of old versions is not supported");
				/*
G2DPlotStyleCollection s = (G2DPlotStyleCollection)obj;

info.CreateArray("Styles", s._innerList.Count);
for (int i = 0; i < s._innerList.Count; i++)
	info.AddValue("e", s._innerList[i]);
info.CommitArray();
*/
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				int count = info.OpenArray();
				IG2DPlotStyle[] array = new IG2DPlotStyle[count];
				for (int i = 0; i < count; i++)
					array[i] = (IG2DPlotStyle)info.GetValue("e", null);
				info.CloseArray(count);

				if (o == null)
				{
					return new G2DPlotStyleCollection(array);
				}
				else
				{
					G2DPlotStyleCollection s = (G2DPlotStyleCollection)o;
					for (int i = count - 1; i >= 0; i--)
						s.Add(array[i]);
					return s;
				}
			}
		}

		/// <summary>
		/// 2006-12-06 We changed the order in which the substyles are plotted.
		/// </summary>
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(G2DPlotStyleCollection), 2)]
		private class XmlSerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				G2DPlotStyleCollection s = (G2DPlotStyleCollection)obj;

				info.CreateArray("Styles", s._innerList.Count);
				for (int i = 0; i < s._innerList.Count; i++)
					info.AddValue("e", s._innerList[i]);
				info.CommitArray();
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				var s = (G2DPlotStyleCollection)o ?? new G2DPlotStyleCollection();

				int count = info.OpenArray();
				for (int i = 0; i < count; i++)
				{
					var item = info.GetValue("e", null);
					if (item is object[])
					{
						foreach (var itemInner in (object[])item)
							s.Add((IG2DPlotStyle)itemInner, false);
					}
					else
					{
						s.Add((IG2DPlotStyle)item, false);
					}
				}

				info.CloseArray(count);
				return s;
			}
		}

		#endregion Serialization

		#region Copying

		public void CopyFrom(G2DPlotStyleCollection from)
		{
			if (object.ReferenceEquals(this, from))
				return;

			using (var suspendToken = SuspendGetToken())
			{
				Clear();

				this._innerList = new List<IG2DPlotStyle>();
				for (int i = 0; i < from._innerList.Count; ++i)
					Add((IG2DPlotStyle)from[i].Clone());

				suspendToken.Resume();
			}
		}

		/// <summary>
		/// Copies all styles 1:1 from a template collection, but try to reuse the data columns from
		/// the old styles collection. This function is used if the user has selected the <see cref="PlotGroupStrictness.Strict"/>.
		/// </summary>
		/// <param name="from">The template style collection to copy from.</param>
		/// <returns>On return, this collection has exactly the same styles as the template collection, in
		/// exactly the same order and with the same properties, except for the data of the styles. The style data
		/// are tried to reuse from the old styles. If this is not possible, the data references will be left empty.</returns>
		public bool CopyFromTemplateCollection(G2DPlotStyleCollection from)
		{
			if (object.ReferenceEquals(this, from))
				return true;

			using (var suspendToken = SuspendGetToken())
			{
				var oldInnerList = this._innerList;

				this._innerList = new List<IG2DPlotStyle>();

				for (int i = 0; i < from._innerList.Count; ++i)
				{
					var fromStyleType = from[i].GetType();

					// try to find the same style in the old list, and use the data from this style
					int foundIdx = oldInnerList.IndexOfFirst(item => item.GetType() == fromStyleType);

					IG2DPlotStyle clonedStyle;

					if (foundIdx >= 0) // if old style list has such an item, we clone that item, and then CopyFrom (but without data)
					{
						clonedStyle = (IG2DPlotStyle)oldInnerList[foundIdx].Clone(true); // First, clone _with_ the old data because we want to reuse them
						clonedStyle.CopyFrom(from[i], false); // now copy the properties from the template style, but _without_ the data
						oldInnerList.RemoveAt(foundIdx); // remove the used style now
					}
					else // an old style of the same type was not found
					{
						clonedStyle = (IG2DPlotStyle)from[i].Clone(false); // clone the style without data
					}

					Add(clonedStyle);
				}
				suspendToken.Resume();
			}

			return true;
		}

		/// <inheritdoc/>
		public bool CopyFrom(object obj, bool copyWithDataReferences)
		{
			if (object.ReferenceEquals(this, obj))
				return true;
			var from = obj as G2DPlotStyleCollection;
			if (null != from)
			{
				CopyFrom(from);
				return true;
			}
			return false;
		}

		/// <inheritdoc/>
		public bool CopyFrom(object obj)
		{
			if (object.ReferenceEquals(this, obj))
				return true;
			var from = obj as G2DPlotStyleCollection;
			if (null != from)
			{
				CopyFrom(from);
				return true;
			}
			return false;
		}

		/// <inheritdoc/>
		public object Clone(bool copyWithDataReferences)
		{
			return new G2DPlotStyleCollection(this);
		}

		/// <inheritdoc/>
		object ICloneable.Clone()
		{
			return new G2DPlotStyleCollection(this);
		}

		public G2DPlotStyleCollection Clone()
		{
			return new G2DPlotStyleCollection(this);
		}

		#endregion Copying

		/// <summary>
		/// Creates an empty collection, i.e. without any styles (so the item is not visible). You must manually add styles to make the plot item visible.
		/// </summary>
		public G2DPlotStyleCollection()
		{
			_innerList = new List<IG2DPlotStyle>();
		}

		public G2DPlotStyleCollection(IG2DPlotStyle[] styles)
		{
			_innerList = new List<IG2DPlotStyle>();
			for (int i = 0; i < styles.Length; ++i)
				if (styles[i] != null)
					this.Add(styles[i], false);
		}

		public G2DPlotStyleCollection(G2DPlotStyleCollection from)
		{
			CopyFrom(from);
		}

		public G2DPlotStyleCollection(LineScatterPlotStyleKind kind, Altaxo.Main.Properties.IReadOnlyPropertyBag context)
		{
			_innerList = new List<IG2DPlotStyle>();

			switch (kind)
			{
				case LineScatterPlotStyleKind.Line:
					Add(new LinePlotStyle(context));
					break;

				case LineScatterPlotStyleKind.Scatter:
					Add(new ScatterPlotStyle(context));
					break;

				case LineScatterPlotStyleKind.LineAndScatter:
					Add(new ScatterPlotStyle(context));
					Add(new LinePlotStyle(context));
					break;
			}
		}

		public void SetFromTemplate(G2DPlotStyleCollection from, PlotGroupStrictness strictness)
		{
			if (strictness == PlotGroupStrictness.Strict)
			{
				CopyFromTemplateCollection(from); // take the whole style collection as is from the template, but try to reuse the additionally needed data columns from the old style
			}
			else if (strictness == PlotGroupStrictness.Exact)
			{
				// note one sub style in the 'from' collection can update only one item in the 'this' collection
				using (var suspendToken = SuspendGetToken())
				{
					var indicesFrom = new SortedSet<int>(System.Linq.Enumerable.Range(0, from.Count));

					for (int i = 0; i < this.Count; ++i)
					{
						var thisStyleType = this[i].GetType();

						// search in from for a style with the same name
						foreach (var fromIndex in indicesFrom)
						{
							if (thisStyleType == from[fromIndex].GetType())
							{
								this[i].CopyFrom(from[fromIndex], false);
								indicesFrom.Remove(fromIndex); // this from style was used, thus remove it
								break;
							}
						}
					}
					suspendToken.Resume();
				}
			}
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

		public IEnumerable<Tuple<string, IReadableColumn, string, Action<IReadableColumn>>> GetAdditionallyUsedColumns()
		{
			return null; // no additionally used columns
		}

		public IG2DPlotStyle this[int i]
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

		public IReadOnlyList<IG2DPlotStyle> Styles
		{
			get
			{
				return _innerList;
			}
		}

		public void Add(IG2DPlotStyle toadd)
		{
			Add(toadd, true);
		}

		protected void Add(IG2DPlotStyle toadd, bool withReorganizationAndEvents)
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

		protected void Replace(IG2DPlotStyle ps, int idx, bool withReorganizationAndEvents)
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

		public void AddRange(IG2DPlotStyle[] toadd)
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

		public void Insert(int whichposition, IG2DPlotStyle toinsert)
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
			IG2DPlotStyle removed = this[idx];
			_innerList.RemoveAt(idx);

			EhSelfChanged(EventArgs.Empty);
		}

		public void ExchangeItemPositions(int pos1, int pos2)
		{
			IG2DPlotStyle item1 = this[pos1];
			_innerList[pos1] = _innerList[pos2];
			_innerList[pos2] = item1;

			EhSelfChanged(EventArgs.Empty);
		}

		protected override void AccumulateChangeData(object sender, EventArgs e)
		{
			_accumulatedEventData = PlotItemStyleChangedEventArgs.Empty;
		}

		public void Paint(Graphics g, IPlotArea layer, Processed2DPlotData pdata, Processed2DPlotData prevItemData, Processed2DPlotData nextItemData)
		{
			if (null == pdata)
				throw new ArgumentNullException(nameof(pdata));

			for (int i = _innerList.Count - 1; i >= 0; i--)
			{
				this[i].Paint(g, layer, pdata, prevItemData, nextItemData);
			}
		}

		public RectangleF PaintSymbol(System.Drawing.Graphics g, System.Drawing.RectangleF bounds)
		{
			for (int i = _innerList.Count - 1; i >= 0; i--)
			{
				bounds = this[i].PaintSymbol(g, bounds);
			}

			return bounds;
		}

		public void PrepareScales(IPlotArea layer)
		{
			for (int i = 0; i < _innerList.Count; ++i)
			{
				this[i].PrepareScales(layer);
			}
		}

		/// <summary>
		/// Distibute changes made to one group style of the collection (at index <c>pivot</c> to all other members of the collection.
		/// </summary>
		/// <param name="pivot">Index of the group style that was changed. This style keeps it's properties.</param>
		/// <param name="layer"></param>
		/// <param name="pdata"></param>
		public void DistributeSubStyleChange(int pivot, IPlotArea layer, Processed2DPlotData pdata)
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
		public void PrepareNewSubStyle(IG2DPlotStyle newSubStyle, IPlotArea layer, Processed2DPlotData pdata)
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

		public IEnumerator<IG2DPlotStyle> GetEnumerator()
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
			foreach (IG2DPlotStyle ps in this)
				ps.CollectExternalGroupStyles(externalGroups);
		}

		public void CollectLocalGroupStyles(PlotGroupStyleCollection externalGroups, PlotGroupStyleCollection localGroups)
		{
			foreach (IG2DPlotStyle ps in this)
				ps.CollectLocalGroupStyles(externalGroups, localGroups);
		}

		public void PrepareGroupStyles(PlotGroupStyleCollection externalGroups, PlotGroupStyleCollection localGroups, IPlotArea layer, Processed2DPlotData pdata)
		{
			foreach (IG2DPlotStyle ps in this)
				ps.PrepareGroupStyles(externalGroups, localGroups, layer, pdata);
		}

		public void ApplyGroupStyles(PlotGroupStyleCollection externalGroups, PlotGroupStyleCollection localGroups)
		{
			foreach (IG2DPlotStyle ps in this)
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

		string INamedObject.Name
		{
			get { return this.GetType().Name; }
		}
	}
}