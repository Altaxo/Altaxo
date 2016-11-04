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
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Altaxo.Graph.Gdi.Plot.Styles
{
	using Altaxo.Data;
	using Altaxo.Main;
	using Drawing;
	using Drawing.ColorManagement;
	using Geometry;
	using Graph.Plot.Data;
	using Graph.Plot.Groups;
	using Plot.Data;
	using Plot.Groups;

	/// <summary>
	/// Summary description for XYPlotLineStyle.
	/// </summary>
	public class DropAreaPlotStyle
		:
		Main.SuspendableDocumentNodeWithEventArgs,
		IG2DPlotStyle,
		IRoutedPropertyReceiver
	{
		protected ILineConnectionStyle _connectionStyle;

		protected bool _ignoreMissingDataPoints; // treat missing points as if not present (connect lines over missing points)

		/// <summary>If true, the start and the end point of the line are connected too.</summary>
		protected bool _connectCircular;

		protected bool _fillArea;

		protected BrushX _fillBrush; // brush to fill the area under the line

		protected CSPlaneID _fillDirection; // the direction to fill

		/// <summary>Designates if the fill color is independent or dependent.</summary>
		protected ColorLinkage _fillColorLinkage = ColorLinkage.PreserveAlpha;

		protected PenX _framePen;

		/// <summary>Designates if the fill color is independent or dependent.</summary>
		protected ColorLinkage _frameColorLinkage = ColorLinkage.PreserveAlpha;

		#region Serialization

		/// <summary>
		/// 2016-10-30 Initial version
		/// </summary>
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(DropAreaPlotStyle), 0)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				var s = (DropAreaPlotStyle)obj;

				info.AddValue("IgnoreMissingPoints", s._ignoreMissingDataPoints);
				info.AddValue("ConnectCircular", s._connectCircular);
				info.AddValue("Connection", s._connectionStyle);

				info.AddValue("FillDirection", s._fillDirection);
				info.AddValue("FillBrush", s._fillBrush);
				info.AddEnum("FillColorLinkage", s._fillColorLinkage);
				info.AddValue("Frame", s._framePen);
				info.AddEnum("FrameColorLinkage", s._frameColorLinkage);
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				DropAreaPlotStyle s = (DropAreaPlotStyle)o ?? new DropAreaPlotStyle(info);

				s._ignoreMissingDataPoints = info.GetBoolean("IgnoreMissingPoints");
				s._connectCircular = info.GetBoolean("ConnectCircular");
				s._connectionStyle = (ILineConnectionStyle)info.GetValue("Connection", s);

				s._fillDirection = (CSPlaneID)info.GetValue("FillDirection", s);
				s._fillBrush = (BrushX)info.GetValue("FillBrush", s);
				if (null != s._fillBrush) s._fillBrush.ParentObject = s;
				s._fillColorLinkage = (ColorLinkage)info.GetEnum("FillColorLinkage", typeof(ColorLinkage));
				s._framePen = (PenX)info.GetValue("Pen", s);
				if (null != s._framePen) s._framePen.ParentObject = s;
				s._frameColorLinkage = (ColorLinkage)info.GetEnum("FrameColorLinkage", typeof(ColorLinkage));
				return s;
			}
		}

		#endregion Serialization

		#region Construction and copying

		public void CopyFrom(DropAreaPlotStyle from, Main.EventFiring eventFiring)
		{
			if (object.ReferenceEquals(this, from))
				return;

			using (var suspendToken = SuspendGetToken())
			{
				this._ignoreMissingDataPoints = from._ignoreMissingDataPoints;
				this._connectCircular = from._connectCircular;
				this._connectionStyle = from._connectionStyle;

				this._fillDirection = from._fillDirection;
				ChildCopyToMember(ref _fillBrush, from._fillBrush);
				this._fillColorLinkage = from._fillColorLinkage;
				ChildCopyToMember(ref _framePen, from._framePen);
				this._frameColorLinkage = from._frameColorLinkage;

				EhSelfChanged();

				suspendToken.Resume(eventFiring);
			}
		}

		/// <inheritdoc/>
		public bool CopyFrom(object obj, bool copyWithDataReferences)
		{
			if (object.ReferenceEquals(this, obj))
				return true;
			var from = obj as DropAreaPlotStyle;
			if (null != from)
			{
				CopyFrom(from, Main.EventFiring.Enabled);
				return true;
			}
			return false;
		}

		/// <inheritdoc/>
		public bool CopyFrom(object obj)
		{
			return CopyFrom(obj, true);
		}

		/// <inheritdoc/>
		public object Clone(bool copyWithDataReferences)
		{
			return new DropAreaPlotStyle(this);
		}

		/// <inheritdoc/>
		public object Clone()
		{
			return new DropAreaPlotStyle(this);
		}

		protected DropAreaPlotStyle(Altaxo.Serialization.Xml.IXmlDeserializationInfo info)
		{
			_connectionStyle = LineConnectionStyles.StraightConnection.Instance;
		}

		public DropAreaPlotStyle(ILineConnectionStyle connection, bool ignoreMissingDataPoints, bool connectCircular, CSPlaneID direction, BrushX fillBrush)
		{
			_connectionStyle = connection;
			_ignoreMissingDataPoints = ignoreMissingDataPoints;
			_connectCircular = connectCircular;
			_fillDirection = direction;
			ChildCopyToMember(ref _fillBrush, fillBrush);
			_framePen = new PenX(NamedColors.Transparent, 1);
		}

		public DropAreaPlotStyle(Altaxo.Main.Properties.IReadOnlyPropertyBag context)
		{
			var penWidth = GraphDocument.GetDefaultPenWidth(context);
			var color = GraphDocument.GetDefaultPlotColor(context);

			_framePen = new PenX(NamedColors.Transparent, penWidth) { LineJoin = LineJoin.Bevel, ParentObject = this };
			_ignoreMissingDataPoints = false;
			_fillBrush = new BrushX(color) { ParentObject = this };
			_fillDirection = new CSPlaneID(1, 0);
			_connectionStyle = LineConnectionStyles.StraightConnection.Instance;
		}

		public DropAreaPlotStyle(DropAreaPlotStyle from)
		{
			CopyFrom(from, Main.EventFiring.Suppressed);
		}

		protected override IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
		{
			if (null != _framePen)
				yield return new Main.DocumentNodeAndName(_framePen, "Pen");

			if (null != _fillBrush)
				yield return new Main.DocumentNodeAndName(_fillBrush, "FillBrush");
		}

		#endregion Construction and copying

		#region Properties

		public ILineConnectionStyle Connection
		{
			get { return _connectionStyle; }
			set
			{
				if (null == value)
					throw new ArgumentNullException(nameof(value));

				if (!(_connectionStyle.Equals(value)))
				{
					_connectionStyle = value;
					EhSelfChanged();
				}
			}
		}

		public bool ConnectCircular
		{
			get
			{
				return _connectCircular;
			}
			set
			{
				bool oldValue = _connectCircular;
				_connectCircular = value;
				if (value != oldValue)
					EhSelfChanged(EventArgs.Empty);
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether to ignore missing data points. If the value is set to true,
		/// the line is plotted even if there is a gap in the data points.
		/// </summary>
		/// <value>
		/// <c>true</c> if missing data points should be ignored; otherwise, if <c>false</c>, no line is plotted between a gap in the data.
		/// </value>
		public bool IgnoreMissingDataPoints
		{
			get
			{
				return _ignoreMissingDataPoints;
			}
			set
			{
				bool oldValue = _ignoreMissingDataPoints;
				_ignoreMissingDataPoints = value;
				if (value != oldValue)
					EhSelfChanged(EventArgs.Empty);
			}
		}

		public CSPlaneID FillDirection
		{
			get { return this._fillDirection; }
			set
			{
				CSPlaneID oldvalue = _fillDirection;
				_fillDirection = value;
				if (oldvalue != value)
				{
					EhSelfChanged(EventArgs.Empty); // Fire Changed event
				}
			}
		}

		public BrushX FillBrush
		{
			get { return this._fillBrush; }
			set
			{
				// copy the brush only if not null
				if (null != value)
				{
					this._fillBrush = (BrushX)value.Clone();
					this._fillBrush.ParentObject = this;
					EhSelfChanged(EventArgs.Empty); // Fire Changed event
				}
				else
					throw new ArgumentNullException("FillBrush", "FillBrush must not be set to null, instead set FillArea to false");
			}
		}

		public ColorLinkage FillColorLinkage
		{
			get
			{
				return _fillColorLinkage;
			}
			set
			{
				var oldValue = _fillColorLinkage;
				_fillColorLinkage = value;
				if (value != oldValue)
					EhSelfChanged(EventArgs.Empty);
			}
		}

		public PenX FramePen
		{
			get { return _framePen; }
			set
			{
				if (null == value)
					throw new ArgumentNullException(nameof(value));

				if (ChildCopyToMember(ref _framePen, value))
					EhSelfChanged();
			}
		}

		public ColorLinkage FrameColorLinkage
		{
			get
			{
				return _frameColorLinkage;
			}
			set
			{
				if (!(_frameColorLinkage == value))
				{
					_frameColorLinkage = value;
					EhSelfChanged();
				}
			}
		}

		public bool IsVisible
		{
			get
			{
				return
					!(LineConnectionStyles.NoConnection.Instance.Equals(_connectionStyle)) &&
					(_fillBrush.IsVisible || _framePen.IsVisible);
			}
		}

		#endregion Properties

		#region Painting

		public RectangleF PaintSymbol(System.Drawing.Graphics g, System.Drawing.RectangleF bounds)
		{
			return bounds;
		}

		public void Paint(Graphics g, IPlotArea layer, Processed2DPlotData pdata, Processed2DPlotData prevItemData, Processed2DPlotData nextItemData)
		{
			PointF[] linePoints = pdata.PlotPointsInAbsoluteLayerCoordinates;
			PlotRangeList rangeList = pdata.RangeList;
			int rangelistlen = rangeList.Count;

			// ensure that brush and pen are cached
			if (null != _framePen) _framePen.Cached = true;

			if (null != _fillBrush)
				_fillBrush.SetEnvironment(new RectangleD2D(PointD2D.Empty, layer.Size), BrushX.GetEffectiveMaximumResolution(g, 1));

			_fillDirection = layer.UpdateCSPlaneID(_fillDirection);

			var gp = new GraphicsPath();

			if (this._ignoreMissingDataPoints)
			{
				// in case we ignore the missing points, all ranges can be plotted
				// as one range, i.e. continuously
				// for this, we create the totalRange, which contains all ranges
				PlotRange totalRange = new PlotRange(rangeList[0].LowerBound, rangeList[rangelistlen - 1].UpperBound);
				_connectionStyle.FillOneRange(gp, pdata, totalRange, layer, _fillDirection, _ignoreMissingDataPoints, _connectCircular);
			}
			else // we not ignore missing points, so plot all ranges separately
			{
				for (int i = 0; i < rangelistlen; i++)
				{
					_connectionStyle.FillOneRange(gp, pdata, rangeList[i], layer, _fillDirection, _ignoreMissingDataPoints, _connectCircular);
				}
			}

			g.FillPath(_fillBrush, gp);

			g.DrawPath(_framePen, gp);
		}

		#endregion Painting

		#region IG2DPlotStyle Members

		/// <summary>
		/// Prepares the scale of this plot style. Since this style does not utilize a scale, this function does nothing.
		/// </summary>
		/// <param name="layer">The parent layer.</param>
		public void PrepareScales(IPlotArea layer)
		{
		}

		public void CollectExternalGroupStyles(PlotGroupStyleCollection externalGroups)
		{
		}

		public void CollectLocalGroupStyles(PlotGroupStyleCollection externalGroups, PlotGroupStyleCollection localGroups)
		{
			ColorGroupStyle.AddLocalGroupStyle(externalGroups, localGroups);
			DashPatternGroupStyle.AddLocalGroupStyle(externalGroups, localGroups);
		}

		public void PrepareGroupStyles(PlotGroupStyleCollection externalGroups, PlotGroupStyleCollection localGroups, IPlotArea layer, Processed2DPlotData pdata)
		{
			if (this._fillColorLinkage == ColorLinkage.Dependent && this._fillBrush != null)
				ColorGroupStyle.PrepareStyle(externalGroups, localGroups, delegate () { return this._fillBrush.Color; });
			else if (this._frameColorLinkage == ColorLinkage.Dependent && this._framePen != null)
				ColorGroupStyle.PrepareStyle(externalGroups, localGroups, delegate () { return this._framePen.Color; });
		}

		public void ApplyGroupStyles(PlotGroupStyleCollection externalGroups, PlotGroupStyleCollection localGroups)
		{
			if (ColorLinkage.Independent != _fillColorLinkage)
			{
				if (null == _fillBrush)
					_fillBrush = new BrushX(NamedColors.Black);

				if (_fillColorLinkage == ColorLinkage.Dependent)
					ColorGroupStyle.ApplyStyle(externalGroups, localGroups, delegate (NamedColor c) { _fillBrush.Color = c; });
				else if (ColorLinkage.PreserveAlpha == _fillColorLinkage)
					ColorGroupStyle.ApplyStyle(externalGroups, localGroups, delegate (NamedColor c) { _fillBrush.Color = c.NewWithAlphaValue(_fillBrush.Color.Color.A); });
			}
			if (ColorLinkage.Independent != _frameColorLinkage)
			{
				if (null == _framePen)
					_framePen = new PenX(NamedColors.Black);

				if (_frameColorLinkage == ColorLinkage.Dependent)
					ColorGroupStyle.ApplyStyle(externalGroups, localGroups, delegate (NamedColor c) { _framePen.Color = c; });
				else if (ColorLinkage.PreserveAlpha == _fillColorLinkage)
					ColorGroupStyle.ApplyStyle(externalGroups, localGroups, delegate (NamedColor c) { _framePen.Color = c.NewWithAlphaValue(_framePen.Color.Color.A); });
			}
		}

		#endregion IG2DPlotStyle Members

		#region IDocumentNode Members

		/// <summary>
		/// Replaces path of items (intended for data items like tables and columns) by other paths. Thus it is possible
		/// to change a plot so that the plot items refer to another table.
		/// </summary>
		/// <param name="Report">Function that reports the found <see cref="DocNodeProxy"/> instances to the visitor.</param>
		public void VisitDocumentReferences(DocNodeProxyReporter Report)
		{
		}

		/// <inheritdoc/>
		public IEnumerable<Tuple<string, IReadableColumn, string, Action<IReadableColumn>>> GetAdditionallyUsedColumns()
		{
			return null; // no additionally used columns
		}

		#endregion IDocumentNode Members

		#region IRoutedPropertyReceiver Members

		public void SetRoutedProperty(IRoutedSetterProperty property)
		{
			switch (property.Name)
			{
				case "StrokeWidth":
					{
						var prop = (RoutedSetterProperty<double>)property;
						this._framePen.Width = (float)prop.Value;
						EhSelfChanged(EventArgs.Empty);
					}
					break;
			}
		}

		public void GetRoutedProperty(IRoutedGetterProperty property)
		{
			switch (property.Name)
			{
				case "StrokeWidth":
					{
						var prop = (RoutedGetterProperty<double>)property;
						prop.Merge(this._framePen.Width);
					}
					break;
			}
		}

		#endregion IRoutedPropertyReceiver Members
	} // end class XYPlotLineStyle
}