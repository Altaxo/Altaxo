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
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;

namespace Altaxo.Graph.Gdi.Plot
{
	using Data;
	using Geometry;
	using Graph.Plot.Data;
	using Graph.Plot.Groups;
	using Groups;
	using Styles;

	[Serializable]
	public abstract class G2DPlotItem : PlotItem
	{
		protected G2DPlotStyleCollection _plotStyles;

		[NonSerialized]
		private Processed2DPlotData _cachedPlotDataUsedForPainting;

		[NonSerialized]
		private PlotGroupStyleCollection _localGroups;

		protected override IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
		{
			if (null != _plotStyles)
				yield return new Main.DocumentNodeAndName(_plotStyles, () => _plotStyles = null, "Style");

			if (null != _localGroups)
				yield return new Main.DocumentNodeAndName(_localGroups, () => _localGroups = null, "LocalPlotGroupStyles");
		}

		public override Main.IDocumentLeafNode StyleObject
		{
			get { return _plotStyles; }
			set { this.Style = (G2DPlotStyleCollection)value; }
		}

		public G2DPlotStyleCollection Style
		{
			get
			{
				return _plotStyles;
			}
			set
			{
				if (null == value)
					throw new System.ArgumentNullException();

				if (ChildSetMember(ref _plotStyles, (G2DPlotStyleCollection)value))
				{
					EhSelfChanged(PlotItemStyleChangedEventArgs.Empty);
				}
			}
		}

		public abstract Processed2DPlotData GetRangesAndPoints(IPlotArea layer);

		public void CopyFrom(G2DPlotItem from)
		{
			CopyFrom((PlotItem)from);
		}

		public override bool CopyFrom(object obj)
		{
			if (object.ReferenceEquals(this, obj))
				return true;

			var copied = base.CopyFrom(obj);
			if (copied)
			{
				var from = obj as G2DPlotItem;
				if (from != null)
				{
					this.Style = from.Style.Clone();
				}
			}
			return copied;
		}

		#region IPlotItem Members

		public override void CollectStyles(PlotGroupStyleCollection styles)
		{
			// first add missing local group styles
			foreach (IG2DPlotStyle sps in _plotStyles)
				sps.CollectExternalGroupStyles(styles);
		}

		public override void PrepareGroupStyles(PlotGroupStyleCollection externalGroups, IPlotArea layer)
		{
			Processed2DPlotData pdata = GetRangesAndPoints(layer);
			if (null == _localGroups)
				_localGroups = new PlotGroupStyleCollection() { ParentObject = this };

			using (var suspendToken = _localGroups.SuspendGetToken())
			{
				_localGroups.Clear();

				// first add missing local group styles
				_plotStyles.CollectLocalGroupStyles(externalGroups, _localGroups);

				// for the newly created group styles BeginPrepare must be called
				_localGroups.BeginPrepare();

				// now prepare the groups
				_plotStyles.PrepareGroupStyles(externalGroups, _localGroups, layer, pdata);

				// for the group styles in the local group, PrepareStep and EndPrepare must be called,
				_localGroups.PrepareStep();
				_localGroups.EndPrepare();

				suspendToken.ResumeSilently(); // we are not interested in changes in the _localGroup
			}
		}

		public override void ApplyGroupStyles(PlotGroupStyleCollection externalGroups)
		{
			using (var suspendToken = _localGroups.SuspendGetToken())
			{
				// for externalGroups, BeginApply was called already in the PlotItemCollection, for localGroups it has to be called here
				_localGroups.BeginApply();

				_plotStyles.ApplyGroupStyles(externalGroups, _localGroups);

				// for externalGroups, EndApply is called later in the PlotItemCollection, for localGroups it has to be called here
				_localGroups.EndApply();

				suspendToken.ResumeSilently(); // we are not interested in changes in the _localGroup
			}
		}

		/// <summary>
		/// Sets the plot style (or sub plot styles) in this item according to a template provided by the plot item in the template argument.
		/// </summary>
		/// <param name="template">The template item to copy the plot styles from.</param>
		/// <param name="strictness">Denotes the strictness the styles are copied from the template. See <see cref="PlotGroupStrictness" /> for more information.</param>
		public override void SetPlotStyleFromTemplate(IGPlotItem template, PlotGroupStrictness strictness)
		{
			if (!(template is G2DPlotItem) || object.ReferenceEquals(this, template))
				return;
			G2DPlotItem from = (G2DPlotItem)template;
			this._plotStyles.SetFromTemplate(from._plotStyles, strictness);
		}

		public override void PaintSymbol(Graphics g, RectangleF location)
		{
			_plotStyles.PaintSymbol(g, location);
		}

		#endregion IPlotItem Members

		public Processed2DPlotData GetPlotData(IPlotArea layer)
		{
			if (_cachedPlotDataUsedForPainting == null)
				_cachedPlotDataUsedForPainting = GetRangesAndPoints(layer);

			return _cachedPlotDataUsedForPainting;
		}

		public override void Paint(Graphics g, IPaintContext context, IPlotArea layer, IGPlotItem prevPlotItem, IGPlotItem nextPlotItem)
		{
			Processed2DPlotData pdata = GetRangesAndPoints(layer);
			if (pdata != null)
				Paint(g, layer, pdata,
						(prevPlotItem is G2DPlotItem) ? ((G2DPlotItem)prevPlotItem).GetPlotData(layer) : null,
						(nextPlotItem is G2DPlotItem) ? ((G2DPlotItem)nextPlotItem).GetPlotData(layer) : null
						);
		}

		/// <summary>
		/// Needed for coordinate transforming styles to plot the data.
		/// </summary>
		/// <param name="g">Graphics context.</param>
		/// <param name="layer">The plot layer.</param>
		/// <param name="plotdata">The plot data. Since the data are transformed, you should not rely that the physical values in this item correspond to the area coordinates.</param>
		/// <param name="prevPlotData">Plot data of the previous plot item.</param>
		/// <param name="nextPlotData">Plot data of the next plot item.</param>
		public virtual void Paint(Graphics g, IPlotArea layer, Processed2DPlotData plotdata, Processed2DPlotData prevPlotData, Processed2DPlotData nextPlotData)
		{
			_cachedPlotDataUsedForPainting = plotdata;

			if (null != this._plotStyles)
			{
				_plotStyles.Paint(g, layer, plotdata, prevPlotData, nextPlotData);
			}
		}

		/// <summary>
		/// Test wether the mouse hits a plot item.
		/// </summary>
		/// <param name="layer">The layer in which this plot item is drawn into.</param>
		/// <param name="hitpoint">The point where the mouse is pressed.</param>
		/// <returns>Null if no hit, or a <see cref="IHitTestObject" /> if there was a hit.</returns>
		public override IHitTestObject HitTest(IPlotArea layer, PointD2D hitpoint)
		{
			Processed2DPlotData pdata = _cachedPlotDataUsedForPainting;
			if (null == pdata)
				return null;

			PlotRangeList rangeList = pdata.RangeList;
			PointF[] ptArray = pdata.PlotPointsInAbsoluteLayerCoordinates;

			if (ptArray.Length < 2)
				return null;

			if (ptArray.Length < 2048)
			{
				if (GdiExtensionMethods.IsPointIntoDistance((PointF)hitpoint, 2.5, ptArray))
				{
					GraphicsPath gp = new GraphicsPath();
					gp.AddLines(ptArray);
					gp.Widen(new Pen(Color.Black, 5));
					return new HitTestObject(gp, this);
				}
			}
			else // we have too much points for the graphics path, so make a hit test first
			{
				int hitindex = -1;
				for (int i = 1; i < ptArray.Length; i++)
				{
					if (Math2D.IsPointIntoDistance((PointF)hitpoint, 5, ptArray[i - 1], ptArray[i]))
					{
						hitindex = i;
						break;
					}
				}
				if (hitindex < 0)
					return null;
				GraphicsPath gp = new GraphicsPath();
				int start = Math.Max(0, hitindex - 2);
				gp.AddLine(ptArray[start], ptArray[start + 1]);
				gp.AddLine(ptArray[start + 1], ptArray[start + 2]);
				gp.Widen(new Pen(Color.Black, 5));
				return new HitTestObject(gp, this);
			}

			return null;
		}

		/// <summary>
		/// Returns the index of a scatter point that is nearest to the location <c>hitpoint</c>
		/// </summary>
		/// <param name="layer">The layer in which this plot item is drawn into.</param>
		/// <param name="hitpoint">The point where the mouse is pressed.</param>
		/// <returns>The information about the point that is nearest to the location, or null if it can not be determined.</returns>
		public XYScatterPointInformation GetNearestPlotPoint(IPlotArea layer, PointD2D hitpoint)
		{
			Processed2DPlotData pdata;
			if (null != (pdata = _cachedPlotDataUsedForPainting))
			{
				PlotRangeList rangeList = pdata.RangeList;
				PointF[] ptArray = pdata.PlotPointsInAbsoluteLayerCoordinates;
				double mindistance = double.MaxValue;
				int minindex = -1;
				for (int i = 1; i < ptArray.Length; i++)
				{
					double distance = Math2D.SquareDistanceLineToPoint(hitpoint, ptArray[i - 1], ptArray[i]);
					if (distance < mindistance)
					{
						mindistance = distance;
						minindex = Math2D.Distance(ptArray[i - 1], hitpoint) < Math2D.Distance(ptArray[i], hitpoint) ? i - 1 : i;
					}
				}
				// ok, minindex is the point we are looking for
				// so we have a look in the rangeList, what row it belongs to
				int rowindex = rangeList.GetRowIndexForPlotIndex(minindex);

				return new XYScatterPointInformation(ptArray[minindex], rowindex, minindex);
			}

			return null;
		}

		/// <summary>
		/// For a given plot point of index oldplotindex, finds the index and coordinates of a plot point
		/// of index oldplotindex+increment.
		/// </summary>
		/// <param name="layer">The layer this plot belongs to.</param>
		/// <param name="oldplotindex">Old plot index.</param>
		/// <param name="increment">Increment to the plot index.</param>
		/// <returns>Information about the new plot point find at position (oldplotindex+increment). Returns null if no such point exists.</returns>
		public XYScatterPointInformation GetNextPlotPoint(IPlotArea layer, int oldplotindex, int increment)
		{
			Processed2DPlotData pdata;
			if (null != (pdata = _cachedPlotDataUsedForPainting))
			{
				PlotRangeList rangeList = pdata.RangeList;
				PointF[] ptArray = pdata.PlotPointsInAbsoluteLayerCoordinates;
				if (ptArray.Length == 0)
					return null;

				int minindex = oldplotindex + increment;
				minindex = Math.Max(minindex, 0);
				minindex = Math.Min(minindex, ptArray.Length - 1);
				// ok, minindex is the point we are looking for
				// so we have a look in the rangeList, what row it belongs to
				int rowindex = rangeList.GetRowIndexForPlotIndex(minindex);
				return new XYScatterPointInformation(ptArray[minindex], rowindex, minindex);
			}

			return null;
		}

		/// <summary>Visits the document references.</summary>
		/// <param name="Report">Function that reports the found <see cref="Altaxo.Main.DocNodeProxy"/> instances to the visitor.</param>
		public override void VisitDocumentReferences(Main.DocNodeProxyReporter Report)
		{
			_plotStyles.VisitDocumentReferences(Report);
			base.VisitDocumentReferences(Report);
		}
	} // end of class PlotItem
}