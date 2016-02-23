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

using Altaxo.Collections;
using Altaxo.Geometry;
using Altaxo.Graph;
using Altaxo.Graph.Gdi.Plot.Groups;
using Altaxo.Graph.Plot;
using Altaxo.Graph.Plot.Data;
using Altaxo.Graph.Plot.Groups;
using Altaxo.Graph.Scales;
using Altaxo.Graph.Scales.Boundaries;
using Altaxo.Main;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Altaxo.Graph.Graph3D.Plot
{
	using Altaxo.Graph.Plot.Groups;
	using Graph.Gdi;
	using GraphicsContext;
	using Groups;
	using System.Collections;

	[Serializable]
	public class PlotItemCollection
		:
		Main.SuspendableDocumentNodeWithSetOfEventArgs,
		IGPlotItem,
		IEnumerable<IGPlotItem>,
		IXBoundsHolder,
		IYBoundsHolder,
		IZBoundsHolder
	{
		/// <summary>Local collection of plot group styles of this plot item collection.</summary>
		private PlotGroupStyleCollection _plotGroupStyles;

		/// <summary>Collection of plot items.</summary>
		private List<IGPlotItem> _plotItems;

		[NonSerialized]
		private IGPlotItem[] _cachedPlotItemsFlattened;

		#region Serialization

		/// <summary>
		/// 2015-11-14 initial version.
		/// </summary>
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(PlotItemCollection), 0)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				PlotItemCollection s = (PlotItemCollection)obj;

				info.CreateArray("PlotItems", s.Count);
				for (int i = 0; i < s.Count; i++)
					info.AddValue("PlotItem", s[i]);
				info.CommitArray();

				info.AddValue("GroupStyles", s._plotGroupStyles);
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				PlotItemCollection s = null != o ? (PlotItemCollection)o : new PlotItemCollection();

				int count = info.OpenArray();
				IGPlotItem[] plotItems = new IGPlotItem[count];
				for (int i = 0; i < count; i++)
				{
					s.Add((IGPlotItem)info.GetValue("PlotItem", s));
				}
				info.CloseArray(count);

				s._plotGroupStyles = (PlotGroupStyleCollection)info.GetValue("GroupStyles", s);
				if (null != s._plotGroupStyles) s._plotGroupStyles.ParentObject = s;

				return s;
			}
		}

		#endregion Serialization

		public PlotItemCollection()
		{
			_plotItems = new List<IGPlotItem>();
			_plotGroupStyles = new PlotGroupStyleCollection() { ParentObject = this };
		}

		/// <summary>
		/// Copy constructor. Clones (!) all items. The parent owner is set to null and has to be set afterwards.
		/// </summary>
		/// <param name="from">The PlotItemCollection to clone this list from.</param>
		public PlotItemCollection(PlotItemCollection from)
			:
			this(null, from, true)
		{
		}

		public PlotItemCollection(XYZPlotLayer owner)
		{
			_parent = owner;
			_plotItems = new List<IGPlotItem>();
			_plotGroupStyles = new PlotGroupStyleCollection() { ParentObject = this };
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PlotItemCollection"/> class.
		/// </summary>
		/// <param name="owner">The owner of this collection.</param>
		/// <param name="plotItems">The plot items that should initially belong to this collection.</param>
		/// <param name="clonePlotItems">If set to <c>true</c> the plot items are cloned before added to this collection. If false, the plot items are added directly to this collection.</param>
		public PlotItemCollection(XYZPlotLayer owner, IEnumerable<IGPlotItem> plotItems, bool clonePlotItems)
		{
			_parent = owner;

			if (clonePlotItems)
				_plotItems = new List<IGPlotItem>(plotItems.Select(pi => (IGPlotItem)pi.Clone()));
			else
				_plotItems = new List<IGPlotItem>(plotItems);
		}

		public IGPlotItem[] Flattened
		{
			get
			{
				if (_cachedPlotItemsFlattened == null)
				{
					List<IGPlotItem> list = new List<IGPlotItem>();
					FillPlotItemList(list);
					_cachedPlotItemsFlattened = list.ToArray();
				}
				return _cachedPlotItemsFlattened;
			}
		}

		/// <summary>
		/// This method must be called, if plot item members are added or removed to this collection.
		/// </summary>
		protected void FillPlotItemList(IList<IGPlotItem> list)
		{
			foreach (IGPlotItem pi in _plotItems)
				if (pi is PlotItemCollection)
					((PlotItemCollection)pi).FillPlotItemList(list);
				else
					list.Add(pi);
		}

		public int Count
		{
			get { return _plotItems.Count; }
		}

		public IGPlotItem this[int i]
		{
			get { return _plotItems[i]; }
		}

		public IList<IGPlotItem> ChildNodes
		{
			get
			{
				return _plotItems;
			}
		}

		IEnumerable<IGPlotItem> ITreeNode<IGPlotItem>.ChildNodes
		{
			get
			{
				return _plotItems;
			}
		}

		IGPlotItem INodeWithParentNode<IGPlotItem>.ParentNode
		{
			get
			{
				return _parent as IGPlotItem;
			}
		}

		public PlotItemCollection ParentCollection
		{
			get
			{
				return _parent as PlotItemCollection;
			}
		}

		public XYZPlotLayer ParentLayer
		{
			get
			{
				return Main.AbsoluteDocumentPath.GetRootNodeImplementing<XYZPlotLayer>(this);
			}
		}

		public IEnumerator<IGPlotItem> GetEnumerator()
		{
			return _plotItems.GetEnumerator();
		}

		public void MergeXBoundsInto(IPhysicalBoundaries pb)
		{
			ICoordinateTransformingGroupStyle coordTransStyle;
			if (null != (coordTransStyle = _plotGroupStyles.CoordinateTransformingStyle))
				coordTransStyle.MergeXBoundsInto(this.ParentLayer, pb, this);
			else
				CoordinateTransformingStyleBase.MergeXBoundsInto(pb, this);
		}

		public void MergeYBoundsInto(IPhysicalBoundaries pb)
		{
			ICoordinateTransformingGroupStyle coordTransStyle;
			if (null != (coordTransStyle = _plotGroupStyles.CoordinateTransformingStyle))
				coordTransStyle.MergeYBoundsInto(this.ParentLayer, pb, this);
			else
				CoordinateTransformingStyleBase.MergeYBoundsInto(pb, this);
		}

		public void MergeZBoundsInto(IPhysicalBoundaries pb)
		{
			ICoordinateTransformingGroupStyle coordTransStyle;
			if (null != (coordTransStyle = _plotGroupStyles.CoordinateTransformingStyle))
				coordTransStyle.MergeZBoundsInto(this.ParentLayer, pb, this);
			else
				CoordinateTransformingStyleBase.MergeZBoundsInto(pb, this);
		}

		public void PaintSymbol(IGraphicContext3D g, RectangleD3D symbolRect)
		{
			throw new NotImplementedException();
		}

		protected override IEnumerable<DocumentNodeAndName> GetDocumentNodeChildrenWithName()
		{
			var index = 0;
			foreach (var item in _plotItems.ToArray())
			{
				yield return new Main.DocumentNodeAndName(item, index.ToString(System.Globalization.CultureInfo.CurrentCulture));
				++index;
			}

			if (null != _plotGroupStyles)
			{
				yield return new Main.DocumentNodeAndName(_plotGroupStyles, () => _plotGroupStyles = null, "PlotGroupStyles");
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _plotItems.GetEnumerator();
		}

		public void PrepareScales(Graph3D.IPlotArea layer)
		{
			foreach (IGPlotItem pi in _plotItems)
				pi.PrepareScales(layer);
		}

		public void PrepareGroupStyles(PlotGroupStyleCollection parentPlotGroupStyles, Graph3D.IPlotArea layer)
		{
			PrepareGroupStylesForward_HierarchyUpOnly(parentPlotGroupStyles, layer);
		}

		public void ApplyGroupStyles(PlotGroupStyleCollection parentPlotGroupStyles)
		{
			ApplyGroupStylesForward_HierarchyUpOnly(parentPlotGroupStyles);
		}

		public void PaintPostprocessing()
		{
			foreach (var pi in _plotItems)
				pi.PaintPostprocessing();
		}

		/// <summary>
		/// Replaces path of items (intended for data items like tables and columns) by other paths. Thus it is possible
		/// to change a plot so that the plot items refer to another table.
		/// </summary>
		/// <param name="Report">Function that reports the found <see cref="DocNodeProxy"/> instances to the visitor.</param>
		public virtual void VisitDocumentReferences(DocNodeProxyReporter Report)
		{
			foreach (var item in this)
				item.VisitDocumentReferences(Report);
		}

		public void CopyFrom(PlotItemCollection from, GraphCopyOptions options)
		{
			if (object.ReferenceEquals(this, from))
				return;

			if (GraphCopyOptions.CopyLayerPlotStyles == (GraphCopyOptions.CopyLayerPlotStyles & options))
			{
				var thisFlat = this.Flattened;
				var fromFlat = from.Flattened;
				int len = Math.Min(thisFlat.Length, fromFlat.Length);
				for (int i = 0; i < len; i++)
				{
					thisFlat[i].SetPlotStyleFromTemplate(fromFlat[i], PlotGroupStrictness.Strict);
				}
			}
		}

		public string GetName(int level)
		{
			return string.Format("<Collection of {0} plot items>", this._plotItems.Count);
		}

		public string GetName(string style)
		{
			return string.Format("<Collection of {0} plot items>", this._plotItems.Count);
		}

		public bool CopyFrom(object obj)
		{
			if (object.ReferenceEquals(this, obj))
				return true;

			var from = obj as PlotItemCollection;
			if (null != from)
			{
				CopyFrom(from, GraphCopyOptions.All);
			}
			return false;
		}

		object ICloneable.Clone()
		{
			return new PlotItemCollection(this);
		}

		public PlotItemCollection Clone()
		{
			return new PlotItemCollection(this);
		}

		/// <summary>
		/// Collects all possible group styles that can be applied to this plot item in
		/// styles.
		/// </summary>
		/// <param name="styles">The collection of group styles.</param>
		public void CollectStyles(PlotGroupStyleCollection styles)
		{
			foreach (IGPlotItem pi in _plotItems)
				pi.CollectStyles(styles);
		}

		public void SetPlotStyleFromTemplate(IGPlotItem template, PlotGroupStrictness strictness)
		{
		}

		public void PaintPreprocessing(IPaintContext context)
		{
			foreach (var pi in _plotItems)
				pi.PaintPreprocessing(context);
		}

		public void Paint(IGraphicContext3D g, IPaintContext context, Graph3D.IPlotArea layer, IGPlotItem previousPlotItem, IGPlotItem nextPlotItem)
		{
			for (int i = 0; i < _plotItems.Count; ++i)
			{
				_plotItems[i].Paint(g, context, layer, i == 0 ? null : _plotItems[i - 1], i == _plotItems.Count - 1 ? null : _plotItems[i + 1]);
			}
		}

		void IGPlotItem.PaintPostprocessing()
		{
			foreach (var pi in _plotItems)
				pi.PaintPostprocessing();
		}

		public Graph3D.IHitTestObject HitTest(Graph3D.IPlotArea layer, Ray3D hitpoint)
		{
			throw new NotImplementedException();
		}

		#region Collection methods

		public void Add(IGPlotItem item)
		{
			if (item == null)
				throw new ArgumentNullException();

			item.ParentObject = this;
			_plotItems.Add(item);

			OnCollectionChanged();
		}

		#endregion Collection methods

		#region Change handling

		protected virtual void OnCollectionChanged()
		{
			_cachedPlotItemsFlattened = null; // invalidate _cachedPlotItemsFlattened
			EhSelfChanged(new SimpleCollectionChangedEventArgs(this)); // notify items down in the hierarchy to invalidate _cachedPlotItemsFlattened
			EhSelfChanged(new BoundariesChangedEventArgs(BoundariesChangedData.ComplexChange)); // notify that possibly the boundaries have been changed
		}

		#endregion Change handling

		/// <summary>
		/// Prepare group styles forward, from the first to the last item.
		/// The function is called recursively for child PlotItemCollections, but only up in the hierarchy.
		/// This function therefore has no influence on items down in hierarchie, i.e. parental PlotItemCollections.
		/// </summary>
		/// <param name="parentGroupStyles">The parent plot group style collection.</param>
		/// <param name="layer">The plot layer.</param>
		/// <remarks>The preparation is used for:
		/// <para>BarGraph: to count items, calculating the width and position of each item afterwards.</para>
		/// <para>It is <b>not</b> used to enumerate colors, line styles etc., since this is done during the Apply stage.</para>
		/// </remarks>
		protected void PrepareGroupStylesForward_HierarchyUpOnly(PlotGroupStyleCollection parentGroupStyles, Graph3D.IPlotArea layer)
		{
			bool transferFromParentStyles =
			 parentGroupStyles != null &&
			 parentGroupStyles.Count != 0 &&
			 parentGroupStyles.DistributeToChildGroups &&
			 this._plotGroupStyles.InheritFromParentGroups;

			// Announce the local plot group styles, that we start preparing
			_plotGroupStyles.BeginPrepare();

			//string thisname = Main.DocumentPath.GetPathString(this, int.MaxValue);
			//System.Diagnostics.Debug.WriteLine(string.Format("{0}:Begin:PrepareFWHUO", thisname));

			// if TransferFromParentStyles was choosen, transfer some of the plot group settings of the parental plot group styles to the local styles
			if (transferFromParentStyles)
			{
				PlotGroupStyleCollection.TransferFromTo(parentGroupStyles, _plotGroupStyles);
				//System.Diagnostics.Debug.WriteLine(string.Format("{0}:Begin:PrepareFWHUO (transfer from parent style", thisname));
			}

			// for each PlotItem in this collection, announce the preparation, using the local plot group style collection
			// after each item, announce a step to the plot group styles, so that the properties (like color etc.) can be stepped forward
			int last = _plotItems.Count - 1;
			for (int i = 0; i <= last; i++)
			{
				IGPlotItem pi = _plotItems[i];
				if (pi is PlotItemCollection)
				{
					PlotItemCollection pic = (PlotItemCollection)pi;
					pic.PrepareGroupStylesForward_HierarchyUpOnly(_plotGroupStyles, layer);
					_plotGroupStyles.PrepareStepIfForeignSteppingFalse(((PlotItemCollection)pi)._plotGroupStyles);
				}
				else
				{
					pi.PrepareGroupStyles(_plotGroupStyles, layer);
					_plotGroupStyles.PrepareStep();
				}
			}

			// after all our own PlotItems are prepared now,
			// if TransferFromParentStyles was choosen, transfer our own plot group settings back to the parental plot group styles
			// so that the parental plot group can continue i.e. with the color etc.
			if (transferFromParentStyles)
			{
				PlotGroupStyleCollection.TransferFromTo(_plotGroupStyles, parentGroupStyles);
				//System.Diagnostics.Debug.WriteLine(string.Format("{0}:End:PrepareFWHUO (transfer back to parent style", thisname));
			}

			// after preparation of all plot items is done, announce the end of preparation,
			// some of the calculations can be done only now.
			_plotGroupStyles.EndPrepare();
			//System.Diagnostics.Debug.WriteLine(string.Format("{0}:End:PrepareFWHUO", thisname));
		}

		/// <summary>
		/// Apply plot group styles forward, from the first to the last item.
		/// The function is called recursively for child PlotItemCollections, but only up in the hierarchy.
		/// This function therefore has no influence on items down in hierarchie, i.e. parental PlotItemCollections.
		/// </summary>
		/// <param name="parentGroupStyles">The parent plot group style collection.</param>
		/// <remarks>The application is used for example:
		/// <para>BarGraph: to calculate the exact position of each plot item.</para>
		/// <para>Color: To step forward through the available colors and apply each color to another PlotItem.</para>
		/// </remarks>

		protected void ApplyGroupStylesForward_HierarchyUpOnly(PlotGroupStyleCollection parentGroupStyles)
		{
			bool transferFromParentStyles =
			 parentGroupStyles != null &&
			 parentGroupStyles.Count != 0 &&
			 parentGroupStyles.DistributeToChildGroups &&
			 this._plotGroupStyles.InheritFromParentGroups;

			// if TransferFromParentStyles was choosen, transfer some of the plot group settings of the parental plot group styles to the local styles
			if (transferFromParentStyles)
			{
				PlotGroupStyleCollection.TransferFromTo(parentGroupStyles, _plotGroupStyles);
			}

			// Announce the local plot group styles the begin of the application stage
			_plotGroupStyles.BeginApply();

			// for each PlotItem in this collection, announce the application, using the local plot group style collection
			// after each item, announce an application step (of stepwidth 1) to the plot group styles, so that the properties (like color etc.) can be stepped forward
			int last = _plotItems.Count - 1;
			for (int i = 0; i <= last; i++)
			{
				IGPlotItem pi = _plotItems[i];
				if (pi is PlotItemCollection)
				{
					PlotItemCollection pic = (PlotItemCollection)pi;
					pic.ApplyGroupStylesForward_HierarchyUpOnly(_plotGroupStyles);
					_plotGroupStyles.StepIfForeignSteppingFalse(1, ((PlotItemCollection)pi)._plotGroupStyles);
				}
				else
				{
					pi.ApplyGroupStyles(_plotGroupStyles);
					_plotGroupStyles.Step(1);
				}
			}
			// after application of PlotGroupStyles to all the plot items is done, announce the end of application,
			_plotGroupStyles.EndApply();

			if (transferFromParentStyles)
			{
				PlotGroupStyleCollection.TransferFromToIfBothSteppingEnabled(_plotGroupStyles, parentGroupStyles);
				parentGroupStyles.SetAllToApplied(); // to indicate that we have applied this styles and so to enable stepping
			}
		}
	}
}