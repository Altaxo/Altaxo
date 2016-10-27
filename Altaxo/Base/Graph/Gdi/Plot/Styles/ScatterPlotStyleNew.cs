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

namespace Altaxo.Graph.Gdi.Plot.Styles
{
	using Altaxo.Data;
	using Altaxo.Main;
	using Drawing;
	using Drawing.ColorManagement;
	using Graph.Plot.Data;
	using Graph.Plot.Groups;
	using Graph2D.Plot.Groups;
	using Graph2D.Plot.Styles;
	using Graph2D.Plot.Styles.ScatterSymbols;
	using Plot.Data;
	using Plot.Groups;

	public class ScatterPlotStyleNew
		:
		Main.SuspendableDocumentNodeWithEventArgs,
		IG2DPlotStyle
	{
		/// <summary>A value of 2 skips every other data point, a value of 3 skips 2 out of 3 data points, and so on.</summary>
		protected int _skipFreq;

		/// <summary>
		/// Indicates whether <see cref="SkipFrequency"/> is independent of other sub-styles.
		/// </summary>
		protected bool _independentSkipFreq;

		/// <summary>
		/// The scatter symbol.
		/// </summary>
		protected IScatterSymbol _symbolShape;

		/// <summary>Is the size of the symbols independent, i.e. not influenced by group styles.</summary>
		protected bool _independentSymbolSize;

		/// <summary>Size of the symbols in points.</summary>
		protected double _symbolSize;

		protected NamedColor _color1;

		/// <summary>Is the material color independent, i.e. not influenced by group styles.</summary>
		protected bool _independentColor;

		protected double _relativePenWidth;

		// cached values:
		/// <summary>If this function is set, then _symbolSize is ignored and the symbol size is evaluated by this function.</summary>
		[field: NonSerialized]
		protected Func<int, double> _cachedSymbolSizeForIndexFunction;

		/// <summary>If this function is set, the symbol color is determined by calling this function on the index into the data.</summary>
		[field: NonSerialized]
		protected Func<int, Color> _cachedColorForIndexFunction;

		#region Copying

		/// <inheritdoc/>
		public void CopyFrom(ScatterPlotStyleNew from, Main.EventFiring eventFiring)
		{
			if (object.ReferenceEquals(this, from))
				return;

			using (var suspendToken = SuspendGetToken())
			{
				this._symbolShape = from._symbolShape;
				this._color1 = from._color1;
				this._independentColor = from._independentColor;
				this._independentSymbolSize = from._independentSymbolSize;

				this._symbolSize = from._symbolSize;
				this._relativePenWidth = from._relativePenWidth;
				this._skipFreq = from._skipFreq;

				EhSelfChanged(EventArgs.Empty);

				suspendToken.Resume(eventFiring);
			}
		}

		/// <inheritdoc/>
		public bool CopyFrom(object obj, bool copyWithDataReferences)
		{
			if (object.ReferenceEquals(this, obj))
				return true;
			var from = obj as ScatterPlotStyleNew;
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
			return new ScatterPlotStyleNew(this);
		}

		/// <inheritdoc/>
		public object Clone()
		{
			return new ScatterPlotStyleNew(this);
		}

		#endregion Copying

		// (Altaxo.Main.Properties.IReadOnlyPropertyBag)null
		protected ScatterPlotStyleNew(Altaxo.Serialization.Xml.IXmlDeserializationInfo info)
		{
		}

		internal ScatterPlotStyleNew(Altaxo.Serialization.Xml.IXmlDeserializationInfo info, bool oldDeserializationRequiresFullConstruction)
		{
			double penWidth = 1;
			double symbolSize = 8;
			var color = ColorSetManager.Instance.BuiltinDarkPlotColors[0];

			this._symbolShape = ScatterSymbolListManager.Instance.BuiltinDefault[0];
			this._color1 = NamedColors.Black;
			this._independentColor = false;

			this._symbolSize = symbolSize;

			this._relativePenWidth = 0.1f;
			this._skipFreq = 1;
			CreateEventChain();
		}

		public ScatterPlotStyleNew(ScatterPlotStyleNew from)
		{
			CopyFrom(from, Main.EventFiring.Suppressed);
			CreateEventChain();
		}

		public ScatterPlotStyleNew(IScatterSymbol shape, double size, double penWidth, NamedColor penColor)
		{
			_symbolShape = shape;
			_color1 = penColor;
			_symbolSize = size;

			_relativePenWidth = penWidth / size;
			_skipFreq = 1;

			CreateEventChain();
		}

		public ScatterPlotStyleNew(Altaxo.Main.Properties.IReadOnlyPropertyBag context)
		{
			double penWidth = GraphDocument.GetDefaultPenWidth(context);
			double symbolSize = GraphDocument.GetDefaultSymbolSize(context);
			var color = GraphDocument.GetDefaultPlotColor(context);

			this._symbolShape = ScatterSymbolListManager.Instance.BuiltinDefault[0];
			_color1 = color;
			this._independentColor = false;
			this._symbolSize = symbolSize;

			this._relativePenWidth = penWidth / symbolSize;
			this._skipFreq = 1;
			CreateEventChain();
		}

		protected void CreateEventChain()
		{
		}

		protected override IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
		{
			yield break;
		}

		public IScatterSymbol Shape
		{
			get { return this._symbolShape; }
			set
			{
				if (null == value)
					throw new ArgumentNullException(nameof(value));

				if (!object.ReferenceEquals(_symbolShape, value))
				{
					this._symbolShape = value;

					EhSelfChanged(EventArgs.Empty); // Fire Changed event
				}
			}
		}

		public bool IsVisible
		{
			get
			{
				return !(_symbolShape is NoSymbol);
			}
		}

		public NamedColor Color
		{
			get { return _color1; }
			set
			{
				if (!(_color1 == value))
				{
					_color1 = value;
					EhSelfChanged(EventArgs.Empty); // Fire Changed event
				}
			}
		}

		public bool IndependentColor
		{
			get
			{
				return _independentColor;
			}
			set
			{
				if (!(_independentColor == value))
				{
					_independentColor = value;
					EhSelfChanged(EventArgs.Empty);
				}
			}
		}

		public double SymbolSize
		{
			get { return _symbolSize; }
			set
			{
				if (!(value >= 0))
					throw new ArgumentOutOfRangeException(nameof(value), "Must be >= 0");

				if (!(_symbolSize == value))
				{
					_symbolSize = value;
					EhSelfChanged(EventArgs.Empty); // Fire Changed event
				}
			}
		}

		public double RelativePenWidth
		{
			get
			{
				return _relativePenWidth;
			}
			set
			{
				if (!(value >= 0 && value <= float.MaxValue))
					throw new ArgumentOutOfRangeException("Out of range: RelativePenWidth = " + value.ToString());

				if (!(_relativePenWidth == value))
				{
					_relativePenWidth = value;
					EhSelfChanged(EventArgs.Empty);
				}
			}
		}

		public bool IndependentSymbolSize
		{
			get
			{
				return _independentSymbolSize;
			}
			set
			{
				if (!(_independentSymbolSize == value))
				{
					_independentSymbolSize = value;
					EhSelfChanged(EventArgs.Empty);
				}
			}
		}

		public int SkipFrequency
		{
			get { return _skipFreq; }
			set
			{
				if (!(_skipFreq == value))
				{
					_skipFreq = value;
					EhSelfChanged(EventArgs.Empty); // Fire Changed event
				}
			}
		}

		public bool IndependentSkipFrequency
		{
			get
			{
				return _independentSkipFreq;
			}
			set
			{
				if (!(_independentSkipFreq == value))
				{
					_independentSkipFreq = value;
					EhSelfChanged(EventArgs.Empty);
				}
			}
		}

		#region I2DPlotItem Members

		public bool IsColorProvider
		{
			get
			{
				return !this._independentColor;
			}
		}

		public bool IsColorReceiver
		{
			get { return !this._independentColor; }
		}

		public bool IsSymbolSizeProvider
		{
			get
			{
				return !(this._symbolShape is NoSymbol) && !this._independentSymbolSize;
			}
		}

		public bool IsSymbolSizeReceiver
		{
			get
			{
				return !(this._symbolShape is NoSymbol) && !this._independentSymbolSize;
			}
		}

		#endregion I2DPlotItem Members

		public static PointF[] ToPointFArray(List<ClipperLib.IntPoint> list, double symbolSize)
		{
			var scale = ScatterSymbolBase.InverseClipperScalingToSymbolSize1 * symbolSize;
			return list.Select(
				intPoint =>
				new PointF((float)(scale * intPoint.X), (float)(-scale * intPoint.Y))).ToArray();
		}

		public void Paint(Graphics g, IPlotArea layer, Processed2DPlotData pdata, Processed2DPlotData prevItemData, Processed2DPlotData nextItemData)
		{
			if (this._symbolShape is NoSymbol)
				return;

			List<List<ClipperLib.IntPoint>> insetPolygon = null;
			List<List<ClipperLib.IntPoint>> framePolygon = null;
			List<List<ClipperLib.IntPoint>> fillPolygon = null;
			_symbolShape.CalculatePolygons(out framePolygon, out insetPolygon, out fillPolygon);

			var path = new GraphicsPath();

			PlotRangeList rangeList = pdata.RangeList;
			PointF[] ptArray = pdata.PlotPointsInAbsoluteLayerCoordinates;

			// adjust the skip frequency if it was not set appropriate
			if (_skipFreq <= 0)
				_skipFreq = 1;

			float xpos = 0, ypos = 0;
			float xdiff, ydiff;

			if (null == _cachedSymbolSizeForIndexFunction && null == _cachedColorForIndexFunction) // using a constant symbol size
			{
				// calculate the path only once
				GraphicsPath insetPath = null;
				SolidBrush insetBrush = null;
				if (null != insetPolygon)
				{
					insetPath = new GraphicsPath();
					insetBrush = new SolidBrush(_symbolShape.PlotColorInfluence.HasFlag(PlotColorInfluence.InsetColor) ? _color1 : _symbolShape.Inset.Color);
					foreach (var list in insetPolygon)
						insetPath.AddPolygon(ToPointFArray(list, _symbolSize));
				}

				GraphicsPath fillPath = null;
				SolidBrush fillBrush = null;
				if (null != fillPolygon)
				{
					fillPath = new GraphicsPath();
					fillBrush = new SolidBrush(_symbolShape.PlotColorInfluence.HasFlag(PlotColorInfluence.FillColor) ? _color1 : _symbolShape.FillColor);

					foreach (var list in fillPolygon)
						fillPath.AddPolygon(ToPointFArray(list, _symbolSize));
				}

				GraphicsPath framePath = null;
				SolidBrush frameBrush = null;
				if (null != framePolygon)
				{
					framePath = new GraphicsPath();
					frameBrush = new SolidBrush(_symbolShape.PlotColorInfluence.HasFlag(PlotColorInfluence.FrameColor) ? _color1 : _symbolShape.Frame.Color);
					foreach (var list in framePolygon)
						framePath.AddPolygon(ToPointFArray(list, _symbolSize));
				}

				// end of path calculations

				// save the graphics stat since we have to translate the origin
				System.Drawing.Drawing2D.GraphicsState gs = g.Save();

				for (int j = 0; j < ptArray.Length; j += _skipFreq)
				{
					xdiff = ptArray[j].X - xpos;
					ydiff = ptArray[j].Y - ypos;
					xpos = ptArray[j].X;
					ypos = ptArray[j].Y;
					g.TranslateTransform(xdiff, ydiff);

					if (null != insetPath)
						g.FillPath(insetBrush, insetPath);

					if (null != fillPath)
						g.FillPath(fillBrush, fillPath);

					if (null != framePath)
						g.FillPath(frameBrush, framePath);
				} // end for

				g.Restore(gs); // Restore the graphics state
			}
			else // using a variable symbol size or variable symbol color
			{
				throw new NotImplementedException();
			}
		}

		public RectangleF PaintSymbol(System.Drawing.Graphics g, System.Drawing.RectangleF bounds)
		{
			if (_symbolShape is NoSymbol)
				return bounds;
			/*
			GraphicsState gs = g.Save();
			g.TranslateTransform(bounds.X + 0.5f * bounds.Width, bounds.Y + 0.5f * bounds.Height);
			Paint(g);
			g.Restore(gs);

			if (this.SymbolSize > bounds.Height)
				bounds.Inflate(0, (float)(this.SymbolSize - bounds.Height));
			*/
			return bounds;
		}

		/// <summary>
		/// Prepares the scale of this plot style. Since this style does not utilize a scale, this function does nothing.
		/// </summary>
		/// <param name="layer">The parent layer.</param>
		public void PrepareScales(IPlotArea layer)
		{
		}

		#region IPlotStyle Members

		public void CollectExternalGroupStyles(PlotGroupStyleCollection externalGroups)
		{
			if (this.IsColorProvider)
				ColorGroupStyle.AddExternalGroupStyle(externalGroups);
			if (this.IsSymbolSizeProvider)
				SymbolSizeGroupStyle.AddExternalGroupStyle(externalGroups);

			ScatterSymbolGroupStyle.AddExternalGroupStyle(externalGroups);
		}

		public void CollectLocalGroupStyles(PlotGroupStyleCollection externalGroups, PlotGroupStyleCollection localGroups)
		{
			ColorGroupStyle.AddLocalGroupStyle(externalGroups, localGroups);
			SymbolSizeGroupStyle.AddLocalGroupStyle(externalGroups, localGroups);
			ScatterSymbolGroupStyle.AddLocalGroupStyle(externalGroups, localGroups);
			SkipFrequencyGroupStyle.AddLocalGroupStyle(externalGroups, localGroups); // (local group style only)
		}

		public void PrepareGroupStyles(PlotGroupStyleCollection externalGroups, PlotGroupStyleCollection localGroups, IPlotArea layer, Processed2DPlotData pdata)
		{
			if (this.IsColorProvider)
				ColorGroupStyle.PrepareStyle(externalGroups, localGroups, delegate () { return this.Color; });

			ScatterSymbolGroupStyle.PrepareStyle(externalGroups, localGroups, delegate { return this._symbolShape; });

			if (this.IsSymbolSizeProvider)
				SymbolSizeGroupStyle.PrepareStyle(externalGroups, localGroups, delegate () { return SymbolSize; });

			// SkipFrequency should be the same for all sub plot styles, so there is no "private" property
			SkipFrequencyGroupStyle.PrepareStyle(externalGroups, localGroups, delegate () { return SkipFrequency; });
		}

		public void ApplyGroupStyles(PlotGroupStyleCollection externalGroups, PlotGroupStyleCollection localGroups)
		{
			if (this.IsColorReceiver)
			{
				// try to get a constant color ...
				ColorGroupStyle.ApplyStyle(externalGroups, localGroups, delegate (NamedColor c) { this.Color = c; });
				// but if there is a color evaluation function, then use that function with higher priority
				if (!VariableColorGroupStyle.ApplyStyle(externalGroups, localGroups, delegate (Func<int, Color> evalFunc) { _cachedColorForIndexFunction = evalFunc; }))
					_cachedColorForIndexFunction = null;
			}

			ScatterSymbolGroupStyle.ApplyStyle(externalGroups, localGroups, delegate (IScatterSymbol c) { this.Shape = c; });

			// per Default, set the symbol size evaluation function to null
			_cachedSymbolSizeForIndexFunction = null;
			if (!_independentSymbolSize)
			{
				// try to get a constant symbol size ...
				SymbolSizeGroupStyle.ApplyStyle(externalGroups, localGroups, delegate (double size) { this.SymbolSize = size; });
				// but if there is an symbol size evaluation function, then use this with higher priority.
				if (!VariableSymbolSizeGroupStyle.ApplyStyle(externalGroups, localGroups, delegate (Func<int, double> evalFunc) { _cachedSymbolSizeForIndexFunction = evalFunc; }))
					_cachedSymbolSizeForIndexFunction = null;
			}

			// SkipFrequency should be the same for all sub plot styles, so there is no "private" property
			if (!this._independentSkipFreq)
				SkipFrequencyGroupStyle.ApplyStyle(externalGroups, localGroups, delegate (int c) { this.SkipFrequency = c; });
		}

		#endregion IPlotStyle Members

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
	}
}