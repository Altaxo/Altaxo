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

using Altaxo.Serialization;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Altaxo.Graph.Gdi.Plot.Styles
{
	using Altaxo.Main;
	using Graph.Plot.Data;
	using Graph.Plot.Groups;
	using Plot.Data;
	using Plot.Groups;

	public class FillToCurvePlotStyle : IG2DPlotStyle
	{
		/// <summary>
		/// Indicates whether the fill color is dependent (can be set by the ColorGroupStyle) or not.
		/// </summary>
		private bool _independentFillColor = true;

		/// <summary>
		/// Brush to fill the area under the line. Can be null.
		/// </summary>
		protected BrushX _fillBrush;

		/// <summary>
		/// Indicates whether the frame color is dependent (can be set by the ColorGroupStyle) or not.
		/// </summary>
		private bool _independentFrameColor = true; // true because the standard value is transparent

		/// <summary>
		/// Pen to enclose the path. Can be null.
		/// </summary>
		protected PenX _framePen;

		private bool _fillToPrevPlotItem = true;
		private bool _fillToNextPlotItem = true;

		[NonSerialized]
		private Action<Graphics, Processed2DPlotData, PlotRange, IPlotArea, Processed2DPlotData> _cachedPaintOneRange;

		[NonSerialized]
		protected Main.IDocumentNode _parentObject;

		[NonSerialized]
		protected Main.EventSuppressor _changeEventSuppressor;

		#region Constructor

		public FillToCurvePlotStyle()
		{
			_changeEventSuppressor = new Altaxo.Main.EventSuppressor(EhChangeEventResumed);
			_cachedPaintOneRange = this.StraightConnection_PaintOneRange;
			FillBrush = new BrushX(NamedColors.Aqua);
		}

		public FillToCurvePlotStyle(FillToCurvePlotStyle from)
		{
			_changeEventSuppressor = new Altaxo.Main.EventSuppressor(EhChangeEventResumed);
			_cachedPaintOneRange = this.StraightConnection_PaintOneRange;
			CopyFrom(from, Main.EventFiring.Suppressed);
		}

		public void CopyFrom(FillToCurvePlotStyle from, Main.EventFiring eventFiring)
		{
			if (object.ReferenceEquals(this, from))
				return;

			var locker = _changeEventSuppressor.Suspend();
			try
			{
				this._independentFillColor = from._independentFillColor;
				this.FillBrush = null == from._fillBrush ? null : from._fillBrush.Clone();

				this._independentFrameColor = from._independentFrameColor;
				this._framePen = null == from._framePen ? null : from._framePen.Clone();

				this._fillToPrevPlotItem = from._fillToPrevPlotItem;
				this._fillToNextPlotItem = from._fillToNextPlotItem;

				this._parentObject = from._parentObject;
			}
			finally
			{
				_changeEventSuppressor.Resume(ref locker, eventFiring);
			}
		}

		public bool CopyFrom(object obj)
		{
			if (object.ReferenceEquals(this, obj))
				return true;
			var from = obj as FillToCurvePlotStyle;
			if (null != from)
			{
				CopyFrom(from, Main.EventFiring.Enabled);
				return true;
			}
			return false;
		}

		#endregion Constructor

		#region Serialization

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(FillToCurvePlotStyle), 0)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				SSerialize(obj, info);
			}

			public static void SSerialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				var s = (FillToCurvePlotStyle)obj;
				info.AddValue("Brush", s._fillBrush);
				info.AddValue("Pen", s._framePen);
				info.AddValue("FillToPreviousItem", s._fillToPrevPlotItem);
				info.AddValue("FillToNextItem", s._fillToNextPlotItem);
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				return SDeserialize(o, info, parent);
			}

			public static object SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				var s = null != o ? (FillToCurvePlotStyle)o : new FillToCurvePlotStyle();

				s._fillBrush = (BrushX)info.GetValue("Brush", s);
				s._framePen = (PenX)info.GetValue("Pen", s);
				s._fillToPrevPlotItem = info.GetBoolean("FillToPreviousItem");
				s._fillToNextPlotItem = info.GetBoolean("FillToNextItem");

				return s;
			}
		}

		/// <summary>
		/// <para>Date: 2012-10-07</para>
		/// <para>Added: IndependentFillColor, IndependentFrameColor</para>
		/// <para>Renamed: Brush in FillBrush</para>
		/// <para>Renamed: Pen in FramePen</para>
		/// </summary>
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(FillToCurvePlotStyle), 1)]
		private class XmlSerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				SSerialize(obj, info);
			}

			public static void SSerialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				var s = (FillToCurvePlotStyle)obj;
				info.AddValue("IndependentFillColor", s._independentFillColor);
				info.AddValue("FillBrush", s._fillBrush);
				info.AddValue("IndependentFrameColor", s._independentFrameColor);
				info.AddValue("FramePen", s._framePen);
				info.AddValue("FillToPreviousItem", s._fillToPrevPlotItem);
				info.AddValue("FillToNextItem", s._fillToNextPlotItem);
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				return SDeserialize(o, info, parent);
			}

			public static object SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				var s = null != o ? (FillToCurvePlotStyle)o : new FillToCurvePlotStyle();

				s._independentFillColor = info.GetBoolean("IndependentFillColor");
				s._fillBrush = (BrushX)info.GetValue("FillBrush", s);
				s._independentFrameColor = info.GetBoolean("IndependentFrameColor");
				s._framePen = (PenX)info.GetValue("FramePen", s);
				s._fillToPrevPlotItem = info.GetBoolean("FillToPreviousItem");
				s._fillToNextPlotItem = info.GetBoolean("FillToNextItem");

				return s;
			}
		}

		#endregion Serialization

		#region Properties

		public bool IndependentFillColor
		{
			get
			{
				return _independentFillColor;
			}
			set
			{
				var oldValue = _independentFillColor;
				_independentFillColor = value;
				if (value != oldValue)
					OnChanged();
			}
		}

		public BrushX FillBrush
		{
			get
			{
				return _fillBrush;
			}
			set
			{
				if (null != _fillBrush)
					_fillBrush.Changed -= EhChildChanged;

				var oldValue = _fillBrush;
				_fillBrush = value;

				if (null != _fillBrush)
					_fillBrush.Changed += EhChildChanged;

				if (!object.ReferenceEquals(oldValue, value))
					OnChanged();
			}
		}

		public bool IndependentFrameColor
		{
			get
			{
				return _independentFrameColor;
			}
			set
			{
				var oldValue = _independentFrameColor;
				_independentFrameColor = value;
				if (value != oldValue)
					OnChanged();
			}
		}

		public PenX FramePen
		{
			get { return _framePen; }
			set
			{
				if (null != _framePen)
					_framePen.Changed -= EhChildChanged;

				var oldValue = _framePen;
				_framePen = value;

				if (null != _framePen)
					_framePen.Changed += EhChildChanged;

				if (!object.ReferenceEquals(oldValue, value))
					OnChanged();
			}
		}

		public bool FillToPreviousItem
		{
			get
			{
				return _fillToPrevPlotItem;
			}
			set
			{
				var oldValue = _fillToPrevPlotItem;
				_fillToPrevPlotItem = value;
				if (oldValue != value)
					OnChanged();
			}
		}

		public bool FillToNextItem
		{
			get
			{
				return _fillToNextPlotItem;
			}
			set
			{
				var oldValue = _fillToNextPlotItem;
				_fillToNextPlotItem = value;
				if (oldValue != value)
					OnChanged();
			}
		}

		#endregion Properties

		#region Change event handling

		protected void EhChangeEventResumed()
		{
			if (_parentObject is Main.IChildChangedEventSink)
				((Main.IChildChangedEventSink)this._parentObject).EhChildChanged(this, EventArgs.Empty);

			if (null != Changed)
				Changed(this, EventArgs.Empty);
		}

		protected void OnChanged()
		{
			if (_changeEventSuppressor.GetEnabledWithCounting())
			{
				EhChangeEventResumed();
			}
		}

		#endregion Change event handling

		#region ICloneable Members

		public FillToCurvePlotStyle Clone()
		{
			return new FillToCurvePlotStyle(this);
		}

		object ICloneable.Clone()
		{
			return new FillToCurvePlotStyle(this);
		}

		#endregion ICloneable Members

		#region IChangedEventSource Members

		[field: NonSerialized]
		public event EventHandler Changed;

		#endregion IChangedEventSource Members

		#region IChildChangedEventSink Members

		public void EhChildChanged(object child, EventArgs e)
		{
			if (null != Changed)
				Changed(this, e);
		}

		#endregion IChildChangedEventSink Members

		#region IDocumentNode Members

		public object ParentObject
		{
			get { return _parentObject; }
			set { _parentObject = (Main.IDocumentNode)value; }
		}

		public string Name
		{
			get { return this.GetType().Name; }
		}

		/// <summary>
		/// Replaces path of items (intended for data items like tables and columns) by other paths. Thus it is possible
		/// to change a plot so that the plot items refer to another table.
		/// </summary>
		/// <param name="Report">Function that reports the found <see cref="DocNodeProxy"/> instances to the visitor.</param>
		public void VisitDocumentReferences(DocNodeProxyReporter Report)
		{
		}

		#endregion IDocumentNode Members

		#region IG2DPlotStyle Members

		public void CollectExternalGroupStyles(Altaxo.Graph.Gdi.Plot.Groups.PlotGroupStyleCollection externalGroups)
		{
			// nothing to collect here
		}

		public void CollectLocalGroupStyles(Altaxo.Graph.Gdi.Plot.Groups.PlotGroupStyleCollection externalGroups, Altaxo.Graph.Gdi.Plot.Groups.PlotGroupStyleCollection localGroups)
		{
			// nothing to collect here
		}

		public void PrepareGroupStyles(Altaxo.Graph.Gdi.Plot.Groups.PlotGroupStyleCollection externalGroups, Altaxo.Graph.Gdi.Plot.Groups.PlotGroupStyleCollection localGroups, IPlotArea layer, Altaxo.Graph.Gdi.Plot.Data.Processed2DPlotData pdata)
		{
			// nothing to collect here
		}

		public void ApplyGroupStyles(Altaxo.Graph.Gdi.Plot.Groups.PlotGroupStyleCollection externalGroups, Altaxo.Graph.Gdi.Plot.Groups.PlotGroupStyleCollection localGroups)
		{
			// nothing to collect here
		}

		public void Paint(Graphics g, IPlotArea layer, Altaxo.Graph.Gdi.Plot.Data.Processed2DPlotData pdata, Processed2DPlotData prevItemData, Processed2DPlotData nextItemData)
		{
			if (_fillToPrevPlotItem && null != prevItemData)
			{
				PaintFillToPrevPlotItem(g, layer, pdata, prevItemData);
			}

			if (_fillToNextPlotItem && null != nextItemData)
			{
				// ensure that brush and pen are cached
				if (null != _fillBrush)
				{
					_fillBrush.SetEnvironment(new RectangleD(PointD2D.Empty, layer.Size), BrushX.GetEffectiveMaximumResolution(g, 1));
				}

				PlotRangeList rangeList = pdata.RangeList;
				int rangelistlen = rangeList.Count;

				// we have to ignore the missing points here, thus all ranges can be plotted
				// as one range, i.e. continuously
				// for this, we create the totalRange, which contains all ranges
				PlotRange totalRange = new PlotRange(rangeList[0].LowerBound, rangeList[rangelistlen - 1].UpperBound);
				_cachedPaintOneRange(g, pdata, totalRange, layer, nextItemData);
			}
		}

		private void PaintFillToPrevPlotItem(Graphics g, IPlotArea layer, Altaxo.Graph.Gdi.Plot.Data.Processed2DPlotData pdata, Processed2DPlotData prevItemData)
		{
			// ensure that brush and pen are cached
			if (null != _fillBrush)
			{
				_fillBrush.SetEnvironment(new RectangleD(PointD2D.Empty, layer.Size), BrushX.GetEffectiveMaximumResolution(g, 1));
			}

			PlotRangeList rangeList = pdata.RangeList;
			int rangelistlen = rangeList.Count;

			// we have to ignore the missing points here, thus all ranges can be plotted
			// as one range, i.e. continuously
			// for this, we create the totalRange, which contains all ranges
			PlotRange totalRange = new PlotRange(rangeList[0].LowerBound, rangeList[rangelistlen - 1].UpperBound, rangeList[0].OffsetToOriginal);
			_cachedPaintOneRange(g, pdata, totalRange, layer, prevItemData);
		}

		public RectangleF PaintSymbol(Graphics g, RectangleF bounds)
		{
			return Rectangle.Empty;
		}

		object IG2DPlotStyle.ParentObject
		{
			set { _parentObject = (Main.IDocumentNode)value; }
		}

		#endregion IG2DPlotStyle Members

		#region Work

		#region StraightConnection

		public void StraightConnection_PaintOneRange(
			Graphics g,
			Processed2DPlotData pdata,
			PlotRange range,
			IPlotArea layer,
			Processed2DPlotData previousData)
		{
			PointF[] linePoints = pdata.PlotPointsInAbsoluteLayerCoordinates;
			PointF[] linepts = new PointF[range.Length];
			Array.Copy(linePoints, range.LowerBound, linepts, 0, range.Length); // Extract
			int lastIdx = range.Length - 1;

			// Try to find points with a similar x value on otherlinepoints
			double firstLogicalX = layer.XAxis.PhysicalVariantToNormal(pdata.GetXPhysical(range.OriginalFirstPoint));
			double lastLogicalX = layer.XAxis.PhysicalVariantToNormal(pdata.GetXPhysical(range.OriginalLastPoint));
			double minDistanceToFirst = double.MaxValue;
			double minDistanceToLast = double.MaxValue;
			int minIdxFirst = -1;
			int minIdxLast = -1;
			foreach (var rangeP in previousData.RangeList)
			{
				for (int i = rangeP.LowerBound; i < rangeP.UpperBound; ++i)
				{
					double logicalX = layer.XAxis.PhysicalVariantToNormal(previousData.GetXPhysical(i + rangeP.OffsetToOriginal));
					if (Math.Abs(logicalX - firstLogicalX) < minDistanceToFirst)
					{
						minDistanceToFirst = Math.Abs(logicalX - firstLogicalX);
						minIdxFirst = i;
					}
					if (Math.Abs(logicalX - lastLogicalX) < minDistanceToLast)
					{
						minDistanceToLast = Math.Abs(logicalX - lastLogicalX);
						minIdxLast = i;
					}
				}
			}

			// if nothing found, use the outmost boundaries of the plot points of the other data item
			if (minIdxFirst < 0)
				minIdxFirst = 0;
			if (minIdxLast < 0)
				minIdxLast = previousData.PlotPointsInAbsoluteLayerCoordinates.Length - 1;

			PointF[] otherLinePoints = new PointF[minIdxLast + 1 - minIdxFirst];
			Array.Copy(previousData.PlotPointsInAbsoluteLayerCoordinates, minIdxFirst, otherLinePoints, 0, otherLinePoints.Length);
			Array.Reverse(otherLinePoints);

			// now paint this

			GraphicsPath gp = new GraphicsPath();
			var layerSize = layer.Size;

			gp.StartFigure();
			gp.AddLines(linepts);
			gp.AddLines(otherLinePoints);
			gp.CloseFigure();

			if (_fillBrush.IsVisible)
			{
				g.FillPath(this._fillBrush, gp);
			}

			if (null != _framePen)
				g.DrawPath(_framePen, gp);

			gp.Reset();
		} // end function PaintOneRange

		#endregion StraightConnection

		#endregion Work
	}
}