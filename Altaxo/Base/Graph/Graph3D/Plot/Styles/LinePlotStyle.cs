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

using Altaxo.Serialization;
using System;
using System.Collections.Generic;

namespace Altaxo.Graph.Graph3D.Plot.Styles
{
	using Altaxo.Data;
	using Altaxo.Main;
	using Drawing;
	using Drawing.D3D;
	using Geometry;
	using Graph.Plot.Data;
	using Graph.Plot.Groups;
	using GraphicsContext;
	using Plot.Data;
	using Plot.Groups;

	/// <summary>
	/// Summary description for XYPlotLineStyle.
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
		protected PenX3D _penHolder;
		protected ILineConnectionStyle _connectionStyle;
		protected bool _useLineSymbolGap;
		protected double _symbolGap;
		protected bool _ignoreMissingPoints; // treat missing points as if not present (connect lines over missing points)

		/// <summary>If true, the start and the end point of the line are connected too.</summary>
		protected bool _connectCircular;

		private static List<IDashPattern> _standardDashPatterns;

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
				info.AddValue("Pen", s._penHolder);
				info.AddValue("Connection", s._connectionStyle);
				info.AddValue("LineSymbolGap", s._useLineSymbolGap);
				info.AddValue("IgnoreMissingPoints", s._ignoreMissingPoints);
				info.AddValue("IndependentColor", s._independentColor);
				info.AddValue("ConnectCircular", s._connectCircular);
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				LinePlotStyle s = null != o ? (LinePlotStyle)o : new LinePlotStyle((Altaxo.Main.Properties.IReadOnlyPropertyBag)null);

				s._penHolder = (PenX3D)info.GetValue("Pen", s);
				s.Connection = (ILineConnectionStyle)info.GetValue("Connection", s);
				s._useLineSymbolGap = info.GetBoolean("LineSymbolGap");
				s._ignoreMissingPoints = info.GetBoolean("IgnoreMissingPoints");
				s._independentColor = info.GetBoolean("IndependentColor");
				s._connectCircular = info.GetBoolean("ConnectCircular");
				return s;
			}
		}

		#endregion Serialization

		#region Construction and copying

		static LinePlotStyle()
		{
			_standardDashPatterns = new List<IDashPattern>();
			_standardDashPatterns.Add(new Drawing.D3D.DashPatterns.Solid());
			_standardDashPatterns.Add(new Drawing.D3D.DashPatterns.Dot());
			_standardDashPatterns.Add(new Drawing.D3D.DashPatterns.Dash());
			_standardDashPatterns.Add(new Drawing.D3D.DashPatterns.DashDot());
			_standardDashPatterns.Add(new Drawing.D3D.DashPatterns.DashDotDot());
		}

		public LinePlotStyle(Altaxo.Main.Properties.IReadOnlyPropertyBag context)
		{
			var penWidth = GraphDocument.GetDefaultPenWidth(context);
			var color = GraphDocument.GetDefaultPlotColor(context);

			_penHolder = new PenX3D(color, penWidth).WithLineJoin(PenLineJoin.Bevel);
			_useLineSymbolGap = true;
			_ignoreMissingPoints = false;
			_connectionStyle = new LineConnectionStyles.StraightConnection();
			_independentColor = false;

			CreateEventChain();
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
				this._penHolder = from._penHolder; // immutable
				this._useLineSymbolGap = from._useLineSymbolGap;
				this._symbolGap = from._symbolGap;
				this._ignoreMissingPoints = from._ignoreMissingPoints;
				this.Connection = from._connectionStyle; // beachte links nur Connection, damit das Template mit gesetzt wird
				this._independentColor = from._independentColor;
				this._connectCircular = from._connectCircular;

				//this._parent = from._parent;

				suspendToken.Resume(eventFiring);
			}
		}

		public LinePlotStyle(LinePlotStyle from)
		{
			CopyFrom(from, Main.EventFiring.Suppressed);
			CreateEventChain();
		}

		protected override IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
		{
			yield break;
		}

		public object Clone()
		{
			return new LinePlotStyle(this);
		}

		protected virtual void CreateEventChain()
		{
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

		public bool LineSymbolGap
		{
			get
			{
				return _useLineSymbolGap;
			}
			set
			{
				bool oldValue = _useLineSymbolGap;
				_useLineSymbolGap = value;
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

		public void SetToNextLineStyle(IDashPattern template)
		{
			SetToNextLineStyle(template, 1);
		}

		public void SetToNextLineStyle(IDashPattern template, int step)
		{
			// this.CopyFrom(template,true);

			// note a exception: since the last dashstyle is "Custom", not only the next dash
			// style has to be defined, but also the overnect to avoid the selection of "Custom"

			var list = _standardDashPatterns;

			var idx = list.IndexOf(template);
			if (idx < 0)
				idx = 0;

			int len = _standardDashPatterns.Count;
			int next = step + idx;
			next = Calc.BasicFunctions.PMod(next, len);

			_penHolder = _penHolder.WithDashPattern(list[next]);
			EhSelfChanged(EventArgs.Empty); // Fire Changed event
		}

		public PenX3D LinePen
		{
			get { return _penHolder; }
			set
			{
				if (!object.ReferenceEquals(_penHolder, value))
				{
					_penHolder = value;
					EhSelfChanged();
				}
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
			if (null != _penHolder)
			{
				g.DrawLine(_penHolder, beg, end);
			}
		}

		public RectangleD3D PaintSymbol(IGraphicsContext3D g, RectangleD3D bounds)
		{
			if (IsVisible)
			{
				var gs = g.SaveGraphicsState();
				g.TranslateTransform((VectorD3D)bounds.Center);
				var halfwidth = bounds.SizeX / 2;
				var symsize = _symbolGap;

				if (this.LineSymbolGap == true)
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
			var symbolGap = _symbolGap;

			int rangelistlen = rangeList.Count;

			if (this._ignoreMissingPoints)
			{
				// in case we ignore the missing points, all ranges can be plotted
				// as one range, i.e. continuously
				// for this, we create the totalRange, which contains all ranges
				PlotRange totalRange = new PlotRange(rangeList[0].LowerBound, rangeList[rangelistlen - 1].UpperBound);
				_connectionStyle.Paint(g, pdata, totalRange, layer, _penHolder, _useLineSymbolGap ? _symbolGap : 0, _connectCircular);
			}
			else // we not ignore missing points, so plot all ranges separately
			{
				for (int i = 0; i < rangelistlen; i++)
				{
					_connectionStyle.Paint(g, pdata, rangeList[i], layer, _penHolder, _useLineSymbolGap ? _symbolGap : 0, _connectCircular);
				}
			}
		}

		#endregion Painting

		public bool IsColorProvider
		{
			get { return !this._independentColor; }
		}

		public NamedColor Color
		{
			get
			{
				return this._penHolder.Color;
			}
			set
			{
				this._penHolder = this._penHolder.WithColor(value);
			}
		}

		public bool IsColorReceiver
		{
			get { return !this._independentColor; }
		}

		private float SymbolSize
		{
			set
			{
				this._symbolGap = value;
			}
		}

		#region IG3DPlotStyle Members

		public void CollectExternalGroupStyles(PlotGroupStyleCollection externalGroups)
		{
		}

		public void CollectLocalGroupStyles(PlotGroupStyleCollection externalGroups, PlotGroupStyleCollection localGroups)
		{
			ColorGroupStyle.AddLocalGroupStyle(externalGroups, localGroups);
			LineStyleGroupStyle.AddLocalGroupStyle(externalGroups, localGroups);
		}

		public void PrepareGroupStyles(PlotGroupStyleCollection externalGroups, PlotGroupStyleCollection localGroups, IPlotArea layer, Processed3DPlotData pdata)
		{
			if (this.IsColorProvider)
				ColorGroupStyle.PrepareStyle(externalGroups, localGroups, delegate () { return this.Color; });

			LineStyleGroupStyle.PrepareStyle(externalGroups, localGroups, delegate { return this.LinePen.DashPattern ?? Drawing.D3D.DashPatterns.Solid.Instance; });
		}

		public void ApplyGroupStyles(PlotGroupStyleCollection externalGroups, PlotGroupStyleCollection localGroups)
		{
			if (this.IsColorReceiver)
				ColorGroupStyle.ApplyStyle(externalGroups, localGroups, delegate (NamedColor c) { this.Color = c; });

			LineStyleGroupStyle.ApplyStyle(externalGroups, localGroups, delegate (IDashPattern c) { this._penHolder = this.LinePen.WithDashPattern(c); });

			if (!SymbolSizeGroupStyle.ApplyStyle(externalGroups, localGroups, delegate (double size) { this._symbolGap = size; }))
			{
				this._symbolGap = 0;
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
						this._penHolder = this._penHolder.WithUniformThickness(prop.Value);
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
						prop.Merge(Math.Max(this._penHolder.Thickness1, this._penHolder.Thickness2));
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