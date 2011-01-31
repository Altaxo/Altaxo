#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2007 Dr. Dirk Lellinger
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
#endregion

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using Altaxo.Serialization;


namespace Altaxo.Graph.Gdi.Plot.Styles
{
	using Altaxo.Main;
	using Graph.Plot.Groups;
	using Plot.Groups;
	using Plot.Data;
	using Graph.Plot.Data;

	public delegate void Action<T1, T2, T3, T4, T5>(T1 o1, T2 o2, T3 o3, T4 o4, T5 o5);

	public class FillToCurvePlotStyle :
		ICloneable,
		Main.IChangedEventSource,
		Main.IChildChangedEventSink,
		Main.IDocumentNode,
		IG2DPlotStyle
	{
		protected BrushX _fillBrush; // brush to fill the area under the line
		protected PenX _strokePen; // Pen to enclose the path

		bool _fillToPrevPlotItem = true;
		bool _fillToNextPlotItem = true;

		[NonSerialized]
		
		Action<Graphics, Processed2DPlotData, PlotRange, IPlotArea, Processed2DPlotData> _cachedPaintOneRange;

		[NonSerialized]
		protected Main.IDocumentNode _parentObject;

		[NonSerialized]
		protected Main.EventSuppressor _changeEventSuppressor;

		#region Constructor
		public FillToCurvePlotStyle()
		{
			_changeEventSuppressor = new Altaxo.Main.EventSuppressor(EhChangeEventResumed);
			_cachedPaintOneRange = this.StraightConnection_PaintOneRange;
			FillBrush = new BrushX(Color.Aqua);
		}

		public FillToCurvePlotStyle(FillToCurvePlotStyle from)
		{
			_changeEventSuppressor = new Altaxo.Main.EventSuppressor(EhChangeEventResumed);
			_cachedPaintOneRange = this.StraightConnection_PaintOneRange;
			CopyFrom(from,true);
		}

		public void CopyFrom(FillToCurvePlotStyle from, bool suppressChangeEvent)
		{
			var locker = _changeEventSuppressor.Suspend();

			this._fillToPrevPlotItem = from._fillToPrevPlotItem;
			this._fillToNextPlotItem = from._fillToNextPlotItem;

			this.FillBrush = null == from._fillBrush ? null : (BrushX)from._fillBrush.Clone();

			this._parentObject = from._parentObject;

			_changeEventSuppressor.Resume(ref locker, suppressChangeEvent);
		}
		#endregion

		#region Serialization

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(FillToCurvePlotStyle), 0)]
		class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				SSerialize(obj, info);
			}
			public static void SSerialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				var s = (FillToCurvePlotStyle)obj;
				info.AddValue("Brush", s._fillBrush);
				info.AddValue("Pen", s._strokePen);
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
				s._strokePen = (PenX)info.GetValue("Pen", s);
				s._fillToPrevPlotItem = info.GetBoolean("FillToPreviousItem");
				s._fillToNextPlotItem = info.GetBoolean("FillToNextItem");

				return s;
			}
		}
		
		#endregion


		#region Properties

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
				
				_fillBrush = value;

				if (null != _fillBrush)
					_fillBrush.Changed += EhChildChanged;
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
				_fillToPrevPlotItem = value;
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
				_fillToNextPlotItem = value;
			}
		}

		#endregion

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

		#endregion

		#region ICloneable Members

		public object Clone()
		{
			return new FillToCurvePlotStyle(this);
		}

		#endregion

		#region IChangedEventSource Members

		[field:NonSerialized]
		public event EventHandler Changed;

		#endregion

		#region IChildChangedEventSink Members
	
		public void EhChildChanged(object child, EventArgs e)
		{
			if (null != Changed)
				Changed(this, e);
		}

		#endregion

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
		/// <param name="options">Information what to replace.</param>
		public void EnumerateDocumentReferences(IDocNodeProxyVisitor options)
		{
			
		}

		#endregion

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
				// ensure that brush and pen are cached
				if (null != _fillBrush)
				{
          _fillBrush.SetEnvironment(new RectangleF(PointF.Empty, layer.Size), BrushX.GetEffectiveMaximumResolution(g, 1));
				}

				PlotRangeList rangeList = pdata.RangeList;
				int rangelistlen = rangeList.Count;

				// we have to ignore the missing points here, thus all ranges can be plotted
				// as one range, i.e. continuously
				// for this, we create the totalRange, which contains all ranges
				PlotRange totalRange = new PlotRange(rangeList[0].LowerBound, rangeList[rangelistlen - 1].UpperBound);
				_cachedPaintOneRange(g, pdata, totalRange, layer, prevItemData);
			}

			if (_fillToNextPlotItem && null != nextItemData)
			{
				// ensure that brush and pen are cached
				if (null != _fillBrush)
				{
          _fillBrush.SetEnvironment(new RectangleF(PointF.Empty, layer.Size), BrushX.GetEffectiveMaximumResolution(g, 1));
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

		public RectangleF PaintSymbol(Graphics g, RectangleF bounds)
		{
			return Rectangle.Empty;
		}

		object IG2DPlotStyle.ParentObject
		{
			set { _parentObject = (Main.IDocumentNode)value; }
		}

		#endregion

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
			GraphicsPath gp = new GraphicsPath();

			SizeF layerSize = layer.Size;

			PointF[] otherLinePoints = (PointF[])previousData.PlotPointsInAbsoluteLayerCoordinates.Clone();
			Array.Reverse(otherLinePoints);

			gp.StartFigure();
			gp.AddLines(linepts);
			gp.AddLines(otherLinePoints);
			gp.CloseFigure();

			if (_fillBrush.IsVisible)
			{
				g.FillPath(this._fillBrush, gp);
			}

			if (null != _strokePen)
				g.DrawPath(_strokePen, gp);

			gp.Reset();
		} // end function PaintOneRange

		

		#endregion

		#endregion
	}
}
