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
using System.Collections.Generic;

namespace Altaxo.Graph.Graph3D.Plot.Styles
{
	using Altaxo.Data;
	using Altaxo.Main;
	using Drawing;
	using Drawing.D3D;
	using Drawing.DashPatternManagement;
	using Geometry;
	using Graph.Plot.Data;
	using Graph.Plot.Groups;
	using GraphicsContext;
	using Plot.Data;
	using Plot.Groups;

	/// <summary>
	/// Style to show 3D data as lines.
	/// </summary>
	public class LinePlotStyle
		:
		Main.SuspendableDocumentNodeWithEventArgs,
		IG3DPlotStyle,
		IRoutedPropertyReceiver
	{
		/// <summary>
		/// Template to make a line draw.
		/// </summary>
		/// <param name="g">Graphics context.</param>
		/// <param name="pdata">The plot data. Don't use the Range property of the pdata, since it is overriden by the next argument.</param>
		/// <param name="overriderange">The plot range to use.</param>
		/// <param name="layer">Graphics layer.</param>
		/// <param name="symbolGap">The size of the symbol gap.</param>
		public delegate void PaintOneRangeTemplate(
			IGraphicsContext3D g,
			Processed3DPlotData pdata,
			PlotRange overriderange,
			IPlotArea layer,
			double symbolGap);

		protected bool _independentColor;
		protected bool _independentDashStyle;
		protected PenX3D _linePen;
		protected ILineConnectionStyle _connectionStyle;

		/// <summary>
		/// true if the symbol size is independent, i.e. is not published nor updated by a group style.
		/// </summary>
		protected bool _independentSymbolSize;

		/// <summary>Controls the length of the end bar.</summary>
		protected double _symbolSize;

		protected bool _ignoreMissingDataPoints; // treat missing points as if not present (connect lines over missing points)

		/// <summary>If true, the start and the end point of the line are connected too.</summary>
		protected bool _connectCircular;

		protected bool _useSymbolGap;

		/// <summary>
		/// Offset used to calculate the real gap between symbol center and beginning of the bar, according to the formula:
		/// realGap = _symbolGap * _symbolGapFactor + _symbolGapOffset;
		/// </summary>
		private double _symbolGapOffset;

		/// <summary>
		/// Factor used to calculate the real gap between symbol center and beginning of the bar, according to the formula:
		/// realGap = _symbolGap * _symbolGapFactor + _symbolGapOffset;
		/// </summary>
		private double _symbolGapFactor = 1.25;

		/// <summary>If this function is set, then _symbolSize is ignored and the symbol size is evaluated by this function.</summary>
		[field: NonSerialized]
		protected Func<int, double> _cachedSymbolSizeForIndexFunction;

		#region Serialization

		/// <summary>
		/// 2016-05-06 initial version.
		/// </summary>
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(LinePlotStyle), 0)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				LinePlotStyle s = (LinePlotStyle)obj;
				info.AddValue("IgnoreMissingPoints", s._ignoreMissingDataPoints);
				info.AddValue("ConnectCircular", s._connectCircular);
				info.AddValue("Connection", s._connectionStyle);
				info.AddValue("Pen", s._linePen);
				info.AddValue("IndependentDashStyle", s._independentDashStyle);
				info.AddValue("IndependentColor", s._independentColor);

				info.AddValue("IndependentSymbolSize", s._independentSymbolSize);
				info.AddValue("SymbolSize", s._symbolSize);

				info.AddValue("UseSymbolGap", s._useSymbolGap);
				info.AddValue("SymbolGapOffset", s._symbolGapOffset);
				info.AddValue("SymbolGapFactor", s._symbolGapFactor);
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				LinePlotStyle s = null != o ? (LinePlotStyle)o : new LinePlotStyle(info);

				s._ignoreMissingDataPoints = info.GetBoolean("IgnoreMissingPoints");
				s._connectCircular = info.GetBoolean("ConnectCircular");
				s.Connection = (ILineConnectionStyle)info.GetValue("Connection", s);
				s._linePen = (PenX3D)info.GetValue("Pen", s);
				s._independentDashStyle = info.GetBoolean("IndependentDashStyle");
				s._independentColor = info.GetBoolean("IndependentColor");

				s._independentSymbolSize = info.GetBoolean("IndependentSymbolSize");
				s._symbolSize = info.GetDouble("SymbolSize");

				s._useSymbolGap = info.GetBoolean("UseSymbolGap");
				s._symbolGapOffset = info.GetDouble("SymbolGapOffset");
				s._symbolGapFactor = info.GetDouble("SymbolGapFactor");
				return s;
			}
		}

		/// <summary>
		/// Deserialization constructor.
		/// </summary>
		/// <param name="info">The deserialization information.</param>
		protected LinePlotStyle(Altaxo.Serialization.Xml.IXmlDeserializationInfo info)
		{
		}

		#endregion Serialization

		#region Construction and copying

		public LinePlotStyle(Altaxo.Main.Properties.IReadOnlyPropertyBag context)
		{
			var penWidth = GraphDocument.GetDefaultPenWidth(context);
			var color = GraphDocument.GetDefaultPlotColor(context);

			_linePen = new PenX3D(color, penWidth).WithLineJoin(PenLineJoin.Bevel);
			_connectionStyle = new LineConnectionStyles.StraightConnection();
		}

		public bool CopyFrom(object obj)
		{
			if (object.ReferenceEquals(this, obj))
				return true;
			var from = obj as LinePlotStyle;
			if (null != from)
			{
				CopyFrom(from, Main.EventFiring.Enabled);
				return true;
			}
			return false;
		}

		public void CopyFrom(LinePlotStyle from, Main.EventFiring eventFiring)
		{
			if (object.ReferenceEquals(this, from))
				return;

			using (var suspendToken = SuspendGetToken())
			{
				this._ignoreMissingDataPoints = from._ignoreMissingDataPoints;
				this._connectCircular = from._connectCircular;
				this.Connection = from._connectionStyle; // beachte links nur Connection, damit das Template mit gesetzt wird

				this._linePen = from._linePen; // immutable
				this._independentDashStyle = from._independentDashStyle;
				this._independentColor = from._independentColor;

				this._independentSymbolSize = from._independentSymbolSize;
				this._symbolSize = from._symbolSize;

				this._useSymbolGap = from._useSymbolGap;
				this._symbolGapOffset = from._symbolGapOffset;
				this._symbolGapFactor = from._symbolGapFactor;

				EhSelfChanged();
				suspendToken.Resume(eventFiring);
			}
		}

		public LinePlotStyle(LinePlotStyle from)
		{
			CopyFrom(from, Main.EventFiring.Suppressed);
		}

		protected override IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
		{
			yield break;
		}

		public object Clone()
		{
			return new LinePlotStyle(this);
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

				if (!object.ReferenceEquals(value, _connectionStyle))
				{
					_connectionStyle = value;
					EhSelfChanged(EventArgs.Empty); // Fire Changed event
				}
			}
		}

		/// <summary>
		/// True when the line is not drawn in the circel of diameter SymbolSize around the symbol center.
		/// </summary>
		public bool UseSymbolGap
		{
			get { return _useSymbolGap; }
			set
			{
				var oldValue = _useSymbolGap;
				_useSymbolGap = value;
				if (oldValue != value)
					EhSelfChanged(EventArgs.Empty);
			}
		}

		public double SymbolGapOffset
		{
			get
			{
				return _symbolGapOffset;
			}
			set
			{
				if (!(_symbolGapOffset == value))
				{
					_symbolGapOffset = value;
					EhSelfChanged();
				}
			}
		}

		public double SymbolGapFactor
		{
			get
			{
				return _symbolGapFactor;
			}
			set
			{
				if (!(_symbolGapFactor == value))
				{
					_symbolGapFactor = value;
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

		public bool IndependentLineColor
		{
			get
			{
				return _independentColor;
			}
			set
			{
				bool oldValue = _independentColor;
				_independentColor = value;
				if (value != oldValue)
					EhSelfChanged(EventArgs.Empty);
			}
		}

		public bool IndependentDashStyle
		{
			get
			{
				return _independentDashStyle;
			}
			set
			{
				bool oldValue = _independentDashStyle;
				_independentDashStyle = value;
				if (value != oldValue)
					EhSelfChanged(EventArgs.Empty);
			}
		}

		public PenX3D LinePen
		{
			get { return _linePen; }
			set
			{
				if (!object.ReferenceEquals(_linePen, value))
				{
					_linePen = value;
					EhSelfChanged();
				}
			}
		}

		/// <summary>
		/// true if the symbol size is independent, i.e. is not published nor updated by a group style.
		/// </summary>
		public bool IndependentSymbolSize
		{
			get { return _independentSymbolSize; }
			set
			{
				var oldValue = _independentSymbolSize;
				_independentSymbolSize = value;
				if (oldValue != value)
					EhSelfChanged(EventArgs.Empty);
			}
		}

		/// <summary>Controls the length of the end bar.</summary>
		public double SymbolSize
		{
			get { return _symbolSize; }
			set
			{
				var oldValue = _symbolSize;
				_symbolSize = value;
				if (oldValue != value)
					EhSelfChanged(EventArgs.Empty);
			}
		}

		public bool IsVisible
		{
			get
			{
				return !object.ReferenceEquals(_connectionStyle, LineConnectionStyles.NoConnection.Instance);
			}
		}

		#endregion Properties

		#region Painting

		public virtual void PaintLine(IGraphicsContext3D g, PointD3D beg, PointD3D end)
		{
			if (null != _linePen)
			{
				g.DrawLine(_linePen, beg, end);
			}
		}

		public RectangleD3D PaintSymbol(IGraphicsContext3D g, RectangleD3D bounds)
		{
			if (IsVisible)
			{
				var gs = g.SaveGraphicsState();
				g.TranslateTransform((VectorD3D)bounds.Center);
				var halfwidth = bounds.SizeX / 2;
				var symsize = _symbolSize;

				if (_useSymbolGap)
				{
					// plot a line with the length of symbolsize from
					PaintLine(g, new PointD3D(-halfwidth, 0, 0), new PointD3D(-symsize, 0, 0));
					PaintLine(g, new PointD3D(symsize, 0, 0), new PointD3D(halfwidth, 0, 0));
				}
				else // no gap
				{
					PaintLine(g, new PointD3D(-halfwidth, 0, 0), new PointD3D(halfwidth, 0, 0));
				}
				g.RestoreGraphicsState(gs);
			}

			return bounds;
		}

		public void Paint(IGraphicsContext3D g, IPlotArea layer, Processed3DPlotData pdata, Processed3DPlotData prevItemData, Processed3DPlotData nextItemData)
		{
			var linePoints = pdata.PlotPointsInAbsoluteLayerCoordinates;
			var rangeList = pdata.RangeList;
			var symbolGap = _symbolSize;
			int rangelistlen = rangeList.Count;

			Func<int, double> symbolGapFunction = null;

			if (_useSymbolGap)
			{
				if (null != _cachedSymbolSizeForIndexFunction && !_independentSymbolSize)
				{
					symbolGapFunction = (idx) => _symbolGapOffset + _symbolGapFactor * _cachedSymbolSizeForIndexFunction(idx);
				}
				else
				{
					symbolGapFunction = (idx) => _symbolGapOffset + _symbolGapFactor * _symbolSize;
				}
			}

			if (this._ignoreMissingDataPoints)
			{
				// in case we ignore the missing points, all ranges can be plotted
				// as one range, i.e. continuously
				// for this, we create the totalRange, which contains all ranges
				PlotRange totalRange = new PlotRange(rangeList[0].LowerBound, rangeList[rangelistlen - 1].UpperBound);
				_connectionStyle.Paint(g, pdata, totalRange, layer, _linePen, symbolGapFunction, _connectCircular);
			}
			else // we not ignore missing points, so plot all ranges separately
			{
				for (int i = 0; i < rangelistlen; i++)
				{
					_connectionStyle.Paint(g, pdata, rangeList[i], layer, _linePen, symbolGapFunction, _connectCircular);
				}
			}
		}

		#endregion Painting

		/// <summary>
		/// Prepares the scale of this plot style. Since this style does not utilize a scale, this function does nothing.
		/// </summary>
		/// <param name="layer">The parent layer.</param>
		public void PrepareScales(Graph3D.IPlotArea layer)
		{
		}

		public bool IsColorProvider
		{
			get { return !this._independentColor; }
		}

		public NamedColor Color
		{
			get
			{
				return this._linePen.Color;
			}
			set
			{
				this._linePen = this._linePen.WithColor(value);
			}
		}

		public bool IsColorReceiver
		{
			get { return !this._independentColor; }
		}

		#region IG3DPlotStyle Members

		public void CollectExternalGroupStyles(PlotGroupStyleCollection externalGroups)
		{
		}

		public void CollectLocalGroupStyles(PlotGroupStyleCollection externalGroups, PlotGroupStyleCollection localGroups)
		{
			ColorGroupStyle.AddLocalGroupStyle(externalGroups, localGroups);
			DashPatternGroupStyle.AddLocalGroupStyle(externalGroups, localGroups);
		}

		public void PrepareGroupStyles(PlotGroupStyleCollection externalGroups, PlotGroupStyleCollection localGroups, IPlotArea layer, Processed3DPlotData pdata)
		{
			if (this.IsColorProvider)
				ColorGroupStyle.PrepareStyle(externalGroups, localGroups, delegate () { return this.Color; });

			if (!_independentDashStyle)
				DashPatternGroupStyle.PrepareStyle(externalGroups, localGroups, delegate { return this.LinePen.DashPattern ?? DashPatternListManager.Instance.BuiltinDefaultSolid; });
		}

		public void ApplyGroupStyles(PlotGroupStyleCollection externalGroups, PlotGroupStyleCollection localGroups)
		{
			if (this.IsColorReceiver)
				ColorGroupStyle.ApplyStyle(externalGroups, localGroups, delegate (NamedColor c) { this.Color = c; });

			if (!_independentDashStyle)
				DashPatternGroupStyle.ApplyStyle(externalGroups, localGroups, delegate (IDashPattern c) { this._linePen = this.LinePen.WithDashPattern(c); });

			if (!_independentSymbolSize)
			{
				_symbolSize = 0;
				SymbolSizeGroupStyle.ApplyStyle(externalGroups, localGroups, delegate (double size) { this._symbolSize = size; });
			}

			// symbol size
			if (!_independentSymbolSize)
			{
				this._symbolSize = 0;
				SymbolSizeGroupStyle.ApplyStyle(externalGroups, localGroups, delegate (double size) { this._symbolSize = size; });

				// but if there is an symbol size evaluation function, then use this with higher priority.
				_cachedSymbolSizeForIndexFunction = null;
				VariableSymbolSizeGroupStyle.ApplyStyle(externalGroups, localGroups, delegate (Func<int, double> evalFunc) { _cachedSymbolSizeForIndexFunction = evalFunc; });
			}
			else
			{
				_cachedSymbolSizeForIndexFunction = null;
			}
		}

		#endregion IG3DPlotStyle Members

		#region IDocumentNode Members

		/// <summary>
		/// Replaces path of items (intended for data items like tables and columns) by other paths. Thus it is possible
		/// to change a plot so that the plot items refer to another table.
		/// </summary>
		/// <param name="Report">Function that reports the found <see cref="DocNodeProxy"/> instances to the visitor.</param>
		public void VisitDocumentReferences(DocNodeProxyReporter Report)
		{
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
						this._linePen = this._linePen.WithUniformThickness(prop.Value);
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
						prop.Merge(Math.Max(this._linePen.Thickness1, this._linePen.Thickness2));
					}
					break;
			}
		}

		public IEnumerable<Tuple<string, IReadableColumn, string, Action<IReadableColumn>>> GetAdditionallyUsedColumns()
		{
			return null; // no additionally used columns
		}

		#endregion IRoutedPropertyReceiver Members
	} // end class XYPlotLineStyle
}