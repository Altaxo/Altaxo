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

	public partial class ScatterPlotStyle
		:
		Main.SuspendableDocumentNodeWithEventArgs,
		IG2DPlotStyle
	{
		/// <summary>
		/// Indicates whether <see cref="SkipFrequency"/> is independent of other sub-styles.
		/// </summary>
		protected bool _independentSkipFreq;

		/// <summary>A value of 2 skips every other data point, a value of 3 skips 2 out of 3 data points, and so on.</summary>
		protected int _skipFreq = 1;

		/// <summary>
		/// If true, treat missing points as if not present (e.g. connect lines over missing points, count skip seamlessly over missing points)
		/// </summary>
		protected bool _ignoreMissingDataPoints;

		/// <summary>If true, group styles that shift the logical position of the items (for instance <see cref="BarSizePosition3DGroupStyle"/>) are not applied. I.e. when true, the position of the item remains unperturbed.</summary>
		private bool _independentOnShiftingGroupStyles = true;



		protected bool _independentScatterSymbol;

		/// <summary>
		/// The scatter symbol.
		/// </summary>
		protected IScatterSymbol _scatterSymbol;

		/// <summary>Is the size of the symbols independent, i.e. not influenced by group styles.</summary>
		protected bool _independentSymbolSize;

		/// <summary>Size of the symbols in points.</summary>
		protected double _symbolSize;

		/// <summary>Is the material color independent, i.e. not influenced by group styles.</summary>
		protected bool _independentColor;

		/// <summary>
		/// The color supposed to be the plot color. Depending on the influence flag, this color influences different parts of the symbol.
		/// </summary>
		protected NamedColor _color;

		/// <summary>
		/// If set, this overrides the absolute structure width of the scatter symbol.
		/// </summary>
		protected double? _overrideStructureWidthOffset;

		/// <summary>
		/// If set, this overrides the relative structure width of the scatter symbol.
		/// </summary>
		protected double? _overrideStructureWidthFactor;

		private PlotColorInfluence? _overridePlotColorInfluence;

		protected NamedColor? _overrideFillColor;

		protected NamedColor? _overrideFrameColor;

		protected NamedColor? _overrideInsetColor;

		protected bool _overrideFrame;
		protected IScatterSymbolFrame _overriddenFrame;

		protected bool _overrideInset;
		protected IScatterSymbolInset _overriddenInset;

		// cached values:
		/// <summary>If this function is set, then _symbolSize is ignored and the symbol size is evaluated by this function.</summary>
		[field: NonSerialized]
		protected Func<int, double> _cachedSymbolSizeForIndexFunction;

		/// <summary>If this function is set, the symbol color is determined by calling this function on the index into the data.</summary>
		[field: NonSerialized]
		protected Func<int, Color> _cachedColorForIndexFunction;

		/// <summary>Logical x shift between the location of the real data point and the point where the item is finally drawn.</summary>
		private double _cachedLogicalShiftX;

		/// <summary>Logical y shift between the location of the real data point and the point where the item is finally drawn.</summary>
		private double _cachedLogicalShiftY;


		#region Copying

		/// <inheritdoc/>
		public void CopyFrom(ScatterPlotStyle from, Main.EventFiring eventFiring)
		{
			if (object.ReferenceEquals(this, from))
				return;

			using (var suspendToken = SuspendGetToken())
			{
				this._independentSkipFreq = from._independentSkipFreq;
				this._skipFreq = from._skipFreq;
				this._ignoreMissingDataPoints = from._ignoreMissingDataPoints;
				this._independentOnShiftingGroupStyles = from._independentOnShiftingGroupStyles;

				this._independentScatterSymbol = from._independentScatterSymbol;
				this._scatterSymbol = from._scatterSymbol;

				this._independentSymbolSize = from._independentSymbolSize;
				this._symbolSize = from._symbolSize;

				this._independentColor = from._independentColor;
				this._color = from._color;

				this._overrideFrame = from._overrideFrame;
				this._overriddenFrame = from._overriddenFrame;
				this._overrideInset = from._overrideInset;
				this._overriddenInset = from._overriddenInset;
				this._overrideStructureWidthOffset = from._overrideStructureWidthOffset;
				this._overrideStructureWidthFactor = from._overrideStructureWidthFactor;
				this._overridePlotColorInfluence = from._overridePlotColorInfluence;
				this._overrideFillColor = from._overrideFillColor;
				this._overrideFrameColor = from._overrideFrameColor;
				this._overrideInsetColor = from._overrideInsetColor;

				EhSelfChanged(EventArgs.Empty);

				suspendToken.Resume(eventFiring);
			}
		}

		/// <inheritdoc/>
		public bool CopyFrom(object obj, bool copyWithDataReferences)
		{
			if (object.ReferenceEquals(this, obj))
				return true;
			var from = obj as ScatterPlotStyle;
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
			return new ScatterPlotStyle(this);
		}

		/// <inheritdoc/>
		public object Clone()
		{
			return new ScatterPlotStyle(this);
		}

		#endregion Copying

		// (Altaxo.Main.Properties.IReadOnlyPropertyBag)null
		protected ScatterPlotStyle(Altaxo.Serialization.Xml.IXmlDeserializationInfo info)
		{
		}

		internal ScatterPlotStyle(Altaxo.Serialization.Xml.IXmlDeserializationInfo info, bool oldDeserializationRequiresFullConstruction)
		{
			double symbolSize = 8;
			var color = ColorSetManager.Instance.BuiltinDarkPlotColors[0];

			this._scatterSymbol = ScatterSymbolListManager.Instance.BuiltinDefault[0];
			this._color = NamedColors.Black;
			this._independentColor = false;
			this._symbolSize = symbolSize;
			this._skipFreq = 1;
		}

		public ScatterPlotStyle(ScatterPlotStyle from)
		{
			CopyFrom(from, Main.EventFiring.Suppressed);
		}

		public ScatterPlotStyle(Altaxo.Main.Properties.IReadOnlyPropertyBag context)
		{
			double penWidth = GraphDocument.GetDefaultPenWidth(context);
			double symbolSize = GraphDocument.GetDefaultSymbolSize(context);
			var color = GraphDocument.GetDefaultPlotColor(context);

			this._scatterSymbol = ScatterSymbolListManager.Instance.BuiltinDefault[0];
			_color = color;
			this._independentColor = false;
			this._symbolSize = symbolSize;
			this._skipFreq = 1;
		}

		protected override IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
		{
			yield break;
		}

		public bool IndependentScatterSymbol
		{
			get
			{
				return _independentScatterSymbol;
			}
			set
			{
				if (!(_independentScatterSymbol == value))
				{
					_independentScatterSymbol = value;
					EhSelfChanged(EventArgs.Empty);
				}
			}
		}

		public IScatterSymbol ScatterSymbol
		{
			get { return this._scatterSymbol; }
			set
			{
				if (null == value)
					throw new ArgumentNullException(nameof(value));

				if (!object.ReferenceEquals(_scatterSymbol, value))
				{
					this._scatterSymbol = value;

					EhSelfChanged(EventArgs.Empty); // Fire Changed event
				}
			}
		}

		public bool IsVisible
		{
			get
			{
				return !(_scatterSymbol is NoSymbol);
			}
		}

		public NamedColor Color
		{
			get { return _color; }
			set
			{
				if (!(_color == value))
				{
					_color = value;
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

		public bool OverrideFrame
		{
			get
			{
				return _overrideFrame;
			}
			set
			{
				if (!(_overrideFrame == value))
				{
					_overrideFrame = value;
					EhSelfChanged();
				}
			}
		}

		public IScatterSymbolFrame OverriddenFrame
		{
			get
			{
				return _overriddenFrame;
			}
			set
			{
				if (!object.ReferenceEquals(_overriddenFrame, value))
				{
					_overriddenFrame = value;
					EhSelfChanged();
				}
			}
		}

		public bool OverrideInset
		{
			get
			{
				return _overrideInset;
			}
			set
			{
				if (!(_overrideInset == value))
				{
					_overrideInset = value;
					EhSelfChanged();
				}
			}
		}

		public IScatterSymbolInset OverriddenInset
		{
			get
			{
				return _overriddenInset;
			}
			set
			{
				if (!object.ReferenceEquals(_overriddenInset, value))
				{
					_overriddenInset = value;
					EhSelfChanged();
				}
			}
		}

		public double? OverrideStructureWidthOffset
		{
			get
			{
				return _overrideStructureWidthOffset;
			}
			set
			{
				if (value.HasValue && !(value >= 0))
					throw new ArgumentOutOfRangeException("Value has to be >= 0");

				if (!(_overrideStructureWidthOffset == value))
				{
					_overrideStructureWidthOffset = value;
					EhSelfChanged(EventArgs.Empty);
				}
			}
		}

		public double? OverrideStructureWidthFactor
		{
			get
			{
				return _overrideStructureWidthFactor;
			}
			set
			{
				if (value.HasValue && !(value >= 0 && value <= 0.5))
					throw new ArgumentOutOfRangeException("Value should be >= 0 and <= 0.5");

				if (!(_overrideStructureWidthFactor == value))
				{
					_overrideStructureWidthFactor = value;
					EhSelfChanged(EventArgs.Empty);
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
				if (!(_ignoreMissingDataPoints == value))
				{
					_ignoreMissingDataPoints = value;
					EhSelfChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// True when we don't want to shift the position of the items, for instance due to the bar graph plot group.
		/// </summary>
		public bool IndependentOnShiftingGroupStyles
		{
			get
			{
				return _independentOnShiftingGroupStyles;
			}
			set
			{
				if (!(_independentOnShiftingGroupStyles == value))
				{
					_independentOnShiftingGroupStyles = value;
					EhSelfChanged();
				}
			}
		}

		public PlotColorInfluence? OverridePlotColorInfluence
		{
			get
			{
				return _overridePlotColorInfluence;
			}
			set
			{
				if (!(_overridePlotColorInfluence == value))
				{
					_overridePlotColorInfluence = value;
					EhSelfChanged();
				}
			}
		}

		public NamedColor? OverrideFillColor
		{
			get
			{
				return _overrideFillColor;
			}
			set
			{
				if (!(_overrideFillColor == value))
				{
					_overrideFillColor = value;
					EhSelfChanged();
				}
			}
		}

		public NamedColor? OverrideFrameColor
		{
			get
			{
				return _overrideFrameColor;
			}
			set
			{
				if (!(_overrideFrameColor == value))
				{
					_overrideFrameColor = value;
					EhSelfChanged();
				}
			}
		}

		public NamedColor? OverrideInsetColor
		{
			get
			{
				return _overrideInsetColor;
			}
			set
			{
				if (!(_overrideInsetColor == value))
				{
					_overrideInsetColor = value;
					EhSelfChanged();
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
				return !(this._scatterSymbol is NoSymbol) && !this._independentSymbolSize;
			}
		}

		public bool IsSymbolSizeReceiver
		{
			get
			{
				return !(this._scatterSymbol is NoSymbol) && !this._independentSymbolSize;
			}
		}

		#endregion I2DPlotItem Members

		public static PointF[] ToPointFArray(List<ClipperLib.IntPoint> list, double symbolSize)
		{
			var scale = SymbolBase.InverseClipperScalingToSymbolSize1 * symbolSize;
			return list.Select(
				intPoint =>
				new PointF((float)(scale * intPoint.X), (float)(-scale * intPoint.Y))).ToArray();
		}

		private struct CachedPathData
		{
			public double SymbolSize;
			public GraphicsPath FillPath;
			public GraphicsPath FramePath;
			public GraphicsPath InsetPath;

			public void Clear()
			{
				SymbolSize = 0;
				FillPath?.Dispose(); FillPath = null;
				FramePath?.Dispose(); FramePath = null;
				InsetPath?.Dispose(); InsetPath = null;
			}
		}

		private struct CachedBrushData
		{
			public NamedColor? PlotColor;
			public Brush FillBrush;
			public Brush FrameBrush;
			public Brush InsetBrush;

			public void Clear()
			{
				PlotColor = null;
				FillBrush?.Dispose(); FillBrush = null;
				FrameBrush?.Dispose(); FrameBrush = null;
				InsetBrush?.Dispose(); InsetBrush = null;
			}
		}

		/// <summary>
		/// Calculates the scatter symbol from the original scatter symbol and the overrides of frame and inset.
		/// </summary>
		/// <returns></returns>
		private IScatterSymbol CalculateOverriddenScatterSymbol()
		{
			if (_scatterSymbol is NoSymbol)
			{
				return _scatterSymbol;
			}

			var scatterSymbol = _scatterSymbol;

			if (_overrideFrame)
			{
				var newFrame = _overriddenFrame;
				if (newFrame != null && scatterSymbol.Frame != null && newFrame.Color != scatterSymbol.Frame.Color)
					newFrame = newFrame.WithColor(scatterSymbol.Frame.Color);
				scatterSymbol = scatterSymbol.WithFrame(newFrame);
			}

			if (_overrideInset)
			{
				var newInset = _overriddenInset;
				if (newInset != null && scatterSymbol.Inset != null && newInset.Color != scatterSymbol.Inset.Color)
					newInset = newInset.WithColor(scatterSymbol.Inset.Color);
				scatterSymbol = scatterSymbol.WithInset(newInset);
			}

			return scatterSymbol;
		}

		/// <summary>
		/// Calculates the paths and stores them into a structure given by the argument <paramref name="cachedPathData"/>.
		/// </summary>
		/// <param name="scatterSymbol">ScatterSymbol, already processed via <see cref="CalculateOverriddenScatterSymbol"/></param>
		/// <param name="symbolSize">The size of the symbol for which to calculate the paths.</param>
		/// <param name="cachedPathData">The cached path data.</param>
		/// <returns>True if new paths have been calculated; false if the previously cached data could be used.</returns>
		private bool CalculatePaths(IScatterSymbol scatterSymbol, double symbolSize, ref CachedPathData cachedPathData)
		{
			if (symbolSize == cachedPathData.SymbolSize)
				return false; // we assume that the structure already contains valid data.

			cachedPathData.SymbolSize = symbolSize;
			cachedPathData.FillPath = cachedPathData.FramePath = cachedPathData.InsetPath = null;

			if (scatterSymbol is NoSymbol)
			{
				return true;
			}

			List<List<ClipperLib.IntPoint>> insetPolygon = null;
			List<List<ClipperLib.IntPoint>> framePolygon = null;
			List<List<ClipperLib.IntPoint>> fillPolygon = null;

			double? overrideRelativeStructureWidth = null;
			if (_overrideStructureWidthOffset.HasValue || _overrideStructureWidthFactor.HasValue)
			{
				overrideRelativeStructureWidth = (_overrideStructureWidthFactor ?? 0) + (_overrideStructureWidthOffset ?? 0) / symbolSize;
			}
			scatterSymbol.CalculatePolygons(overrideRelativeStructureWidth, out framePolygon, out insetPolygon, out fillPolygon);

			// calculate the path only once
			if (null != insetPolygon)
			{
				cachedPathData.InsetPath = new GraphicsPath();
				foreach (var list in insetPolygon)
					cachedPathData.InsetPath.AddPolygon(ToPointFArray(list, symbolSize));
			}

			if (null != fillPolygon)
			{
				cachedPathData.FillPath = new GraphicsPath();
				foreach (var list in fillPolygon)
					cachedPathData.FillPath.AddPolygon(ToPointFArray(list, symbolSize));
			}

			if (null != framePolygon)
			{
				cachedPathData.FramePath = new GraphicsPath();
				foreach (var list in framePolygon)
					cachedPathData.FramePath.AddPolygon(ToPointFArray(list, symbolSize));
			}

			return true;
		}

		/// <summary>
		/// Calculates the brushes.
		/// </summary>
		/// <param name="scatterSymbol">ScatterSymbol, already processed via <see cref="CalculateOverriddenScatterSymbol"/></param>
		/// <param name="plotColor">The current plot color.</param>
		/// <param name="cachedPathData">The cached path data.</param>
		/// <param name="cachedBrushData">Cached brush data, which will be filled-in during this call..</param>
		/// <returns>True if new cached brush data were calculated; false if the cached data were up-to-date.</returns>
		private bool CalculateBrushes(IScatterSymbol scatterSymbol, NamedColor plotColor, CachedPathData cachedPathData, ref CachedBrushData cachedBrushData)
		{
			if (plotColor == cachedBrushData.PlotColor)
				return false; // cached data valid and could be reused;

			cachedBrushData.Clear();
			cachedBrushData.PlotColor = plotColor;

			var plotColorInfluence = _overridePlotColorInfluence ?? scatterSymbol.PlotColorInfluence;

			if (null != cachedPathData.InsetPath)
			{
				var insetColor = _overrideInsetColor ?? scatterSymbol.Inset.Color;
				if (plotColorInfluence.HasFlag(PlotColorInfluence.InsetColorFull))
					insetColor = plotColor;
				else if (plotColorInfluence.HasFlag(PlotColorInfluence.InsetColorPreserveAlpha))
					insetColor = plotColor.NewWithAlphaValue(insetColor.Color.A);

				cachedBrushData.InsetBrush = new SolidBrush(insetColor);
			}

			if (null != cachedPathData.FillPath)
			{
				var fillColor = _overrideFillColor ?? scatterSymbol.FillColor;
				if (plotColorInfluence.HasFlag(PlotColorInfluence.FillColorFull))
					fillColor = plotColor;
				else if (plotColorInfluence.HasFlag(PlotColorInfluence.FillColorPreserveAlpha))
					fillColor = plotColor.NewWithAlphaValue(fillColor.Color.A);

				cachedBrushData.FillBrush = new SolidBrush(fillColor);
			}

			if (null != cachedPathData.FramePath)
			{
				var frameColor = _overrideFrameColor ?? scatterSymbol.Frame.Color;
				if (plotColorInfluence.HasFlag(PlotColorInfluence.FrameColorFull))
					frameColor = plotColor;
				else if (plotColorInfluence.HasFlag(PlotColorInfluence.FrameColorPreserveAlpha))
					frameColor = plotColor.NewWithAlphaValue(frameColor.Color.A);

				cachedBrushData.FrameBrush = new SolidBrush(frameColor);
			}

			return true;
		}


		private void PaintOneRange(
			Graphics g,
			IPlotArea layer,
			PointF[] plotPositions,
			IPlotRange range,
			IScatterSymbol scatterSymbol,
			ref CachedPathData cachedPathData,
			ref CachedBrushData cachedBrushData)
		{
			var ptArray = plotPositions;

			float xpos = 0, ypos = 0;
			float xdiff, ydiff;

			int originalIndex;

			// save the graphics stat since we have to translate the origin
			System.Drawing.Drawing2D.GraphicsState gs = g.Save();

			if (null == _cachedSymbolSizeForIndexFunction && null == _cachedColorForIndexFunction) // using a constant symbol size
			{
				// calculate the path only once
				CalculatePaths(scatterSymbol, _symbolSize, ref cachedPathData);
				CalculateBrushes(scatterSymbol, _color, cachedPathData, ref cachedBrushData);



				for (int plotPointIndex = range.LowerBound; plotPointIndex < range.UpperBound; plotPointIndex += _skipFreq)
				{
					xdiff = ptArray[plotPointIndex].X - xpos;
					ydiff = ptArray[plotPointIndex].Y - ypos;
					xpos = ptArray[plotPointIndex].X;
					ypos = ptArray[plotPointIndex].Y;
					g.TranslateTransform(xdiff, ydiff);

					if (null != cachedPathData.InsetPath)
						g.FillPath(cachedBrushData.InsetBrush, cachedPathData.InsetPath);

					if (null != cachedPathData.FillPath)
						g.FillPath(cachedBrushData.FillBrush, cachedPathData.FillPath);

					if (null != cachedPathData.FramePath)
						g.FillPath(cachedBrushData.FrameBrush, cachedPathData.FramePath);
				} // end for


			}
			else // using a variable symbol size or variable symbol color
			{
				CalculatePaths(scatterSymbol, _symbolSize, ref cachedPathData);
				CalculateBrushes(scatterSymbol, _color, cachedPathData, ref cachedBrushData);

				for (int plotPointIndex = range.LowerBound; plotPointIndex < range.UpperBound; plotPointIndex += _skipFreq)
				{
					originalIndex = range.GetOriginalRowIndexFromPlotPointIndex(plotPointIndex);

					if (null == _cachedColorForIndexFunction)
					{
						double customSymbolSize = _cachedSymbolSizeForIndexFunction(originalIndex);
						CalculatePaths(scatterSymbol, customSymbolSize, ref cachedPathData);
					}
					else
					{
						double customSymbolSize = null == _cachedSymbolSizeForIndexFunction ? _symbolSize : _cachedSymbolSizeForIndexFunction(originalIndex);
						var customSymbolColor = _cachedColorForIndexFunction(originalIndex);
						CalculatePaths(scatterSymbol, customSymbolSize, ref cachedPathData);
						CalculateBrushes(scatterSymbol, NamedColor.FromArgb(customSymbolColor.A, customSymbolColor.R, customSymbolColor.G, customSymbolColor.B), cachedPathData, ref cachedBrushData);
					}

					xdiff = ptArray[plotPointIndex].X - xpos;
					ydiff = ptArray[plotPointIndex].Y - ypos;
					xpos = ptArray[plotPointIndex].X;
					ypos = ptArray[plotPointIndex].Y;
					g.TranslateTransform(xdiff, ydiff);

					if (null != cachedPathData.InsetPath)
						g.FillPath(cachedBrushData.InsetBrush, cachedPathData.InsetPath);

					if (null != cachedPathData.FillPath)
						g.FillPath(cachedBrushData.FillBrush, cachedPathData.FillPath);

					if (null != cachedPathData.FramePath)
						g.FillPath(cachedBrushData.FrameBrush, cachedPathData.FramePath);

				}
			}

			g.Restore(gs); // Restore the graphics state
		}


		static PointF[] GetPlotPointsInAbsoluteLayerCoordinatesWithShift(Processed2DPlotData pdata, IPlotArea layer, double cachedLogicalShiftX, double cachedLogicalShiftY)
		{
			var result = new PointF[pdata.PlotPointsInAbsoluteLayerCoordinates.Length];
			foreach (PlotRange r in pdata.RangeList)
			{
				int lower = r.LowerBound;
				int upper = r.UpperBound;
				int offset = r.OffsetToOriginal;
				for (int j = lower; j < upper; ++j)
				{
					int originalRow = j + offset;
					Logical3D logicalMean = layer.GetLogical3D(pdata, originalRow);
					logicalMean.RX += cachedLogicalShiftX;
					logicalMean.RY += cachedLogicalShiftY;

					double x, y;
					layer.CoordinateSystem.LogicalToLayerCoordinates(logicalMean, out x, out y);
					result[j] = new PointF((float)x, (float)y);
				}
			}
			return result;
		}

		public void Paint(Graphics g, IPlotArea layer, Processed2DPlotData pdata, Processed2DPlotData prevItemData, Processed2DPlotData nextItemData)
		{
			// adjust the skip frequency if it was not set appropriate
			if (_skipFreq <= 0)
				_skipFreq = 1;

			if (this._scatterSymbol is NoSymbol)
				return;

			CachedPathData cachedPathData = new CachedPathData();
			CachedBrushData cachedBrushData = new CachedBrushData();

			PlotRangeList rangeList = pdata.RangeList;
			PointF[] plotPositions = pdata.PlotPointsInAbsoluteLayerCoordinates;

			if (!_independentOnShiftingGroupStyles && (0 != _cachedLogicalShiftX || 0 != _cachedLogicalShiftY))
			{
				plotPositions = GetPlotPointsInAbsoluteLayerCoordinatesWithShift(pdata, layer, _cachedLogicalShiftX, _cachedLogicalShiftY);
			}



			// Calculate current scatterSymbol overridden with frame and inset
			var scatterSymbol = CalculateOverriddenScatterSymbol();


			if (this._ignoreMissingDataPoints)
			{
				// in case we ignore the missing points, all ranges can be plotted
				// as one range, i.e. continuously
				// for this, we create the totalRange, which contains all ranges
				var totalRange = new PlotRangeCompound(rangeList);
				this.PaintOneRange(g, layer, plotPositions, totalRange, scatterSymbol, ref cachedPathData, ref cachedBrushData);
			}
			else // we not ignore missing points, so plot all ranges separately
			{
				for (int i = 0; i < rangeList.Count; i++)
				{
					this.PaintOneRange(g, layer, plotPositions, rangeList[i], scatterSymbol, ref cachedPathData, ref cachedBrushData);
				}
			}


			cachedBrushData.Clear();
			cachedPathData.Clear();
		}

		public RectangleF PaintSymbol(System.Drawing.Graphics g, System.Drawing.RectangleF bounds)
		{
			if (_scatterSymbol is NoSymbol)
				return bounds;

			var cachedPathData = new CachedPathData();
			var cachedBrushData = new CachedBrushData();
			var scatterSymbol = CalculateOverriddenScatterSymbol();
			CalculatePaths(scatterSymbol, _symbolSize, ref cachedPathData);
			CalculateBrushes(scatterSymbol, _color, cachedPathData, ref cachedBrushData);

			GraphicsState gs = g.Save();
			g.TranslateTransform(bounds.X + 0.5f * bounds.Width, bounds.Y + 0.5f * bounds.Height);

			if (null != cachedPathData.InsetPath)
				g.FillPath(cachedBrushData.InsetBrush, cachedPathData.InsetPath);

			if (null != cachedPathData.FillPath)
				g.FillPath(cachedBrushData.FillBrush, cachedPathData.FillPath);

			if (null != cachedPathData.FramePath)
				g.FillPath(cachedBrushData.FrameBrush, cachedPathData.FramePath);

			cachedBrushData.Clear();
			cachedPathData.Clear();

			g.Restore(gs);

			if (this.SymbolSize > bounds.Height)
				bounds.Inflate(0, (float)(this.SymbolSize - bounds.Height));

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

			if (!_independentScatterSymbol)
				ScatterSymbolGroupStyle.AddExternalGroupStyle(externalGroups);
		}

		public void CollectLocalGroupStyles(PlotGroupStyleCollection externalGroups, PlotGroupStyleCollection localGroups)
		{
			ColorGroupStyle.AddLocalGroupStyle(externalGroups, localGroups);
			SymbolSizeGroupStyle.AddLocalGroupStyle(externalGroups, localGroups);
			ScatterSymbolGroupStyle.AddLocalGroupStyle(externalGroups, localGroups); // here it is OK to add the local group style, even if _independentScatterSymbol is true
			SkipFrequencyGroupStyle.AddLocalGroupStyle(externalGroups, localGroups); // (local group style only)
			IgnoreMissingDataPointsGroupStyle.AddLocalGroupStyle(externalGroups, localGroups);
		}

		public void PrepareGroupStyles(PlotGroupStyleCollection externalGroups, PlotGroupStyleCollection localGroups, IPlotArea layer, Processed2DPlotData pdata)
		{
			if (this.IsColorProvider)
				ColorGroupStyle.PrepareStyle(externalGroups, localGroups, delegate () { return this.Color; });

			ScatterSymbolGroupStyle.PrepareStyle(externalGroups, localGroups, delegate { return this._scatterSymbol; });

			if (this.IsSymbolSizeProvider)
				SymbolSizeGroupStyle.PrepareStyle(externalGroups, localGroups, delegate () { return SymbolSize; });

			// SkipFrequency should be the same for all sub plot styles, so there is no "private" property
			SkipFrequencyGroupStyle.PrepareStyle(externalGroups, localGroups, delegate () { return SkipFrequency; });

			// IgnoreMissingDataPoints should be the same for all sub plot styles, so there is no "private" property
			IgnoreMissingDataPointsGroupStyle.PrepareStyle(externalGroups, localGroups, () => _ignoreMissingDataPoints);

		}

		public void ApplyGroupStyles(PlotGroupStyleCollection externalGroups, PlotGroupStyleCollection localGroups)
		{
			// IgnoreMissingDataPoints is the same for all sub plot styles
			IgnoreMissingDataPointsGroupStyle.ApplyStyle(externalGroups, localGroups, (ignoreMissingDataPoints) => this._ignoreMissingDataPoints = ignoreMissingDataPoints);


			if (this.IsColorReceiver)
			{
				// try to get a constant color ...
				ColorGroupStyle.ApplyStyle(externalGroups, localGroups, delegate (NamedColor c) { this.Color = c; });
				// but if there is a color evaluation function, then use that function with higher priority
				if (!VariableColorGroupStyle.ApplyStyle(externalGroups, localGroups, delegate (Func<int, Color> evalFunc) { _cachedColorForIndexFunction = evalFunc; }))
					_cachedColorForIndexFunction = null;
			}

			if (!_independentScatterSymbol)
			{
				ScatterSymbolGroupStyle.ApplyStyle(externalGroups, localGroups, delegate (IScatterSymbol c) { this.ScatterSymbol = c; });
			}

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

			// Shift the items ?
			_cachedLogicalShiftX = 0;
			_cachedLogicalShiftY = 0;
			if (!_independentOnShiftingGroupStyles)
			{
				var shiftStyle = PlotGroupStyle.GetFirstStyleToApplyImplementingInterface<IShiftLogicalXYGroupStyle>(externalGroups, localGroups);
				if (null != shiftStyle)
				{
					shiftStyle.Apply(out _cachedLogicalShiftX, out _cachedLogicalShiftY);
				}
			}
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