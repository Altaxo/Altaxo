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

using Altaxo.Data;
using Altaxo.Graph.Plot.Data;
using Altaxo.Graph.Plot.Groups;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Altaxo.Graph.Gdi.Plot.Styles
{
	using Altaxo.Graph;
	using Altaxo.Graph.Gdi.Plot.Data;
	using Altaxo.Main;
	using Drawing;
	using Geometry;

	public class ErrorBarPlotStyle
		:
		Main.SuspendableDocumentNodeWithEventArgs,
		IG2DPlotStyle
	{
		private INumericColumnProxy _positiveErrorColumn;
		private INumericColumnProxy _negativeErrorColumn;

		/// <summary>
		/// True if the color of the label is not dependent on the color of the parent plot style.
		/// </summary>
		protected bool _independentColor;

		/// <summary>Pen used to draw the error bar.</summary>
		private PenX _strokePen;

		/// <summary>
		/// True when to plot horizontal error bars.
		/// </summary>
		private bool _isHorizontalStyle;

		/// <summary>
		/// true if the symbol size is independent, i.e. is not published nor updated by a group style.
		/// </summary>
		private bool _independentSymbolSize;

		/// <summary>Controls the length of the end bar.</summary>
		private double _symbolSize;

		/// <summary>
		/// True when the line is not drawn in the circel of diameter SymbolSize around the symbol center.
		/// </summary>
		private bool _symbolGap;

		/// <summary>
		/// If true, the bars are capped by an end bar.
		/// </summary>
		private bool _showEndBars = true;

		/// <summary>
		/// When true, bar graph position group styles are not applied, i.e. the item remains where it is.
		/// </summary>
		private bool _doNotShiftHorizontalPosition;

		/// <summary>
		/// Skip frequency.
		/// </summary>
		protected int _skipFreq;

		/// <summary>
		/// When we deal with bar charts, this is the logical shift between real point
		/// and the independent value where the bar is really drawn to.
		/// </summary>
		private double _cachedLogicalShiftOfIndependent;

		/// <summary>If this function is set, then _symbolSize is ignored and the symbol size is evaluated by this function.</summary>
		[field: NonSerialized]
		protected Func<int, double> _cachedSymbolSizeForIndexFunction;

		/// <summary>If this function is set, the symbol color is determined by calling this function on the index into the data.</summary>
		[field: NonSerialized]
		protected Func<int, Color> _cachedColorForIndexFunction;

		#region Serialization

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ErrorBarPlotStyle), 0)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				ErrorBarPlotStyle s = (ErrorBarPlotStyle)obj;

				info.AddValue("PositiveError", s._positiveErrorColumn);
				info.AddValue("NegativeError", s._negativeErrorColumn);

				info.AddValue("IndependentColor", s._isHorizontalStyle);
				info.AddValue("Pen", s._strokePen);

				info.AddValue("Axis", s._isHorizontalStyle ? 0 : 1);
				info.AddValue("IndependentSymbolSize", s._independentSymbolSize);
				info.AddValue("SymbolSize", s._symbolSize);
				info.AddValue("SymbolGap", s._symbolGap);
				info.AddValue("SkipFreq", s._skipFreq);

				info.AddValue("ShowEndBars", s._showEndBars);
				info.AddValue("NotShiftHorzPos", s._doNotShiftHorizontalPosition);
			}

			protected virtual ErrorBarPlotStyle SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				ErrorBarPlotStyle s = null != o ? (ErrorBarPlotStyle)o : new ErrorBarPlotStyle((Altaxo.Main.Properties.IReadOnlyPropertyBag)null);

				s._positiveErrorColumn = (Altaxo.Data.INumericColumnProxy)info.GetValue("PositiveError", s);
				if (null != s._positiveErrorColumn) s._positiveErrorColumn.ParentObject = s;

				s._negativeErrorColumn = (Altaxo.Data.INumericColumnProxy)info.GetValue("NegativeError", s);
				if (null != s._negativeErrorColumn) s._negativeErrorColumn.ParentObject = s;

				s._independentColor = info.GetBoolean("IndependentColor");

				s.Pen = (PenX)info.GetValue("Pen", s);

				s._isHorizontalStyle = (0 == info.GetInt32("Axis"));
				s._independentSymbolSize = info.GetBoolean("IndependentSymbolSize");
				s._symbolSize = info.GetDouble("SymbolSize");
				s._symbolGap = info.GetBoolean("SymbolGap");
				s._skipFreq = info.GetInt32("SkipFreq");
				s._showEndBars = info.GetBoolean("ShowEndBars");
				s._doNotShiftHorizontalPosition = info.GetBoolean("NotShiftHorzPos");

				return s;
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				ErrorBarPlotStyle s = SDeserialize(o, info, parent);

				return s;
			}
		}

		#endregion Serialization

		public ErrorBarPlotStyle(Altaxo.Main.Properties.IReadOnlyPropertyBag context)
		{
			var penWidth = GraphDocument.GetDefaultPenWidth(context);
			var color = GraphDocument.GetDefaultPlotColor(context);

			this._strokePen = new PenX(color, penWidth);
		}

		public ErrorBarPlotStyle(ErrorBarPlotStyle from)
		{
			CopyFrom(from);
		}

		public bool CopyFrom(object obj)
		{
			if (object.ReferenceEquals(this, obj))
				return true;
			var from = obj as ErrorBarPlotStyle;
			if (null != from)
			{
				this._independentSymbolSize = from._independentSymbolSize;
				this._symbolSize = from._symbolSize;
				this._symbolGap = from._symbolGap;
				this._independentColor = from._independentColor;
				this._showEndBars = from._showEndBars;
				this._isHorizontalStyle = from._isHorizontalStyle;
				this._doNotShiftHorizontalPosition = from._doNotShiftHorizontalPosition;
				this._strokePen = (PenX)from._strokePen.Clone();
				this._positiveErrorColumn = (INumericColumnProxy)from._positiveErrorColumn.Clone();
				this._negativeErrorColumn = (INumericColumnProxy)from._negativeErrorColumn.Clone();
				this._cachedLogicalShiftOfIndependent = from._cachedLogicalShiftOfIndependent;
				return true;
			}
			return false;
		}

		protected override IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
		{
			if (null != _strokePen)
				yield return new Main.DocumentNodeAndName(_strokePen, "Pen");

			if (null != _positiveErrorColumn)
				yield return new Main.DocumentNodeAndName(_positiveErrorColumn, "PositiveErrorColumn");

			if (null != _negativeErrorColumn)
				yield return new Main.DocumentNodeAndName(_negativeErrorColumn, "NegativeErrorColumn");
		}

		public ErrorBarPlotStyle Clone()
		{
			return new ErrorBarPlotStyle(this);
		}

		object ICloneable.Clone()
		{
			return new ErrorBarPlotStyle(this);
		}

		#region Properties

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

		/// <summary>Controls the length of the end bar.</summary>
		public int SkipFrequency
		{
			get { return _skipFreq; }
			set
			{
				var oldValue = _skipFreq;
				_skipFreq = value;
				if (oldValue != value)
					EhSelfChanged(EventArgs.Empty);
			}
		}

		/// <summary>
		/// True when the line is not drawn in the circel of diameter SymbolSize around the symbol center.
		/// </summary>
		public bool SymbolGap
		{
			get { return _symbolGap; }
			set
			{
				var oldValue = _symbolGap;
				_symbolGap = value;
				if (oldValue != value)
					EhSelfChanged(EventArgs.Empty);
			}
		}

		/// <summary>
		/// True if the color of the label is not dependent on the color of the parent plot style.
		/// </summary>
		public bool IndependentColor
		{
			get { return _independentColor; }
			set
			{
				var oldValue = _independentColor;
				_independentColor = value;
				if (oldValue != value)
					EhSelfChanged(EventArgs.Empty);
			}
		}

		/// <summary>
		/// If true, the bars are capped by an end bar.
		/// </summary>
		public bool ShowEndBars
		{
			get
			{
				return _showEndBars;
			}
			set
			{
				var oldValue = _showEndBars;
				_showEndBars = value;
				if (oldValue != value)
					EhSelfChanged(EventArgs.Empty);
			}
		}

		/// <summary>
		/// True when we don't want to shift the horizontal position, for instance due to the bar graph plot group.
		/// </summary>
		public bool DoNotShiftIndependentVariable
		{
			get
			{
				return _doNotShiftHorizontalPosition;
			}
			set
			{
				var oldValue = _doNotShiftHorizontalPosition;
				_doNotShiftHorizontalPosition = value;
				if (oldValue != value)
					EhSelfChanged(EventArgs.Empty);
			}
		}

		/// <summary>
		/// True when no vertical, but horizontal error bars are shown.
		/// </summary>
		public bool IsHorizontalStyle
		{
			get
			{
				return _isHorizontalStyle;
			}
			set
			{
				var oldValue = _isHorizontalStyle;
				_isHorizontalStyle = value;
				if (oldValue != value)
					EhSelfChanged(EventArgs.Empty);
			}
		}

		/// <summary>Pen used to draw the error bar.</summary>
		public PenX Pen
		{
			get { return _strokePen; }
			set
			{
				var oldValue = _strokePen;
				_strokePen = value;
				if (!object.ReferenceEquals(oldValue, value))
				{
					_strokePen.ParentObject = this;
					EhSelfChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Data that define the error in the positive direction.
		/// </summary>
		public INumericColumn PositiveErrorColumn
		{
			get { return _positiveErrorColumn == null ? null : _positiveErrorColumn.Document; }
			set
			{
				var oldValue = _positiveErrorColumn == null ? null : _positiveErrorColumn.Document;
				_positiveErrorColumn = NumericColumnProxyBase.FromColumn(value);
				if (!object.ReferenceEquals(oldValue, value))
					EhSelfChanged(EventArgs.Empty);
			}
		}

		/// <summary>
		/// Data that define the error in the negative direction.
		/// </summary>
		public INumericColumn NegativeErrorColumn
		{
			get { return _negativeErrorColumn == null ? null : _negativeErrorColumn.Document; }
			set
			{
				var oldValue = _negativeErrorColumn == null ? null : _negativeErrorColumn.Document;
				_negativeErrorColumn = NumericColumnProxyBase.FromColumn(value);
				if (oldValue != value)
					EhSelfChanged(EventArgs.Empty);
			}
		}

		#endregion Properties

		#region IG2DPlotStyle Members

		public void CollectExternalGroupStyles(Altaxo.Graph.Gdi.Plot.Groups.PlotGroupStyleCollection externalGroups)
		{
			if (!_independentColor)
				Graph.Plot.Groups.ColorGroupStyle.AddExternalGroupStyle(externalGroups);
		}

		public void CollectLocalGroupStyles(Altaxo.Graph.Gdi.Plot.Groups.PlotGroupStyleCollection externalGroups, Altaxo.Graph.Gdi.Plot.Groups.PlotGroupStyleCollection localGroups)
		{
			if (!_independentColor)
				Graph.Plot.Groups.ColorGroupStyle.AddLocalGroupStyle(externalGroups, localGroups);

			SkipFrequencyGroupStyle.AddLocalGroupStyle(externalGroups, localGroups); // (local group only)
		}

		public void PrepareGroupStyles(Altaxo.Graph.Gdi.Plot.Groups.PlotGroupStyleCollection externalGroups, Altaxo.Graph.Gdi.Plot.Groups.PlotGroupStyleCollection localGroups, IPlotArea layer, Altaxo.Graph.Gdi.Plot.Data.Processed2DPlotData pdata)
		{
			if (!_independentColor)
				Graph.Plot.Groups.ColorGroupStyle.PrepareStyle(externalGroups, localGroups, delegate () { return this._strokePen.Color; });

			// SkipFrequency should be the same for all sub plot styles, so there is no "private" property
			SkipFrequencyGroupStyle.PrepareStyle(externalGroups, localGroups, delegate () { return SkipFrequency; });

			// note: symbol size and barposition are only applied, but not prepared
			// this item can not be used as provider of a symbol size
		}

		public void ApplyGroupStyles(Altaxo.Graph.Gdi.Plot.Groups.PlotGroupStyleCollection externalGroups, Altaxo.Graph.Gdi.Plot.Groups.PlotGroupStyleCollection localGroups)
		{
			_cachedColorForIndexFunction = null;
			_cachedSymbolSizeForIndexFunction = null;
			// color
			if (!_independentColor)
			{
				ColorGroupStyle.ApplyStyle(externalGroups, localGroups, delegate (NamedColor c) { this._strokePen.Color = c; });

				// but if there is a color evaluation function, then use that function with higher priority
				VariableColorGroupStyle.ApplyStyle(externalGroups, localGroups, delegate (Func<int, Color> evalFunc) { _cachedColorForIndexFunction = evalFunc; });
			}

			// SkipFrequency should be the same for all sub plot styles, so there is no "private" property
			SkipFrequencyGroupStyle.ApplyStyle(externalGroups, localGroups, delegate (int c) { this.SkipFrequency = c; });

			// symbol size
			if (!_independentSymbolSize)
			{
				if (!SymbolSizeGroupStyle.ApplyStyle(externalGroups, localGroups, delegate (double size) { this._symbolSize = size; }))
				{
					this._symbolSize = 0;
				}

				// but if there is an symbol size evaluation function, then use this with higher priority.
				VariableSymbolSizeGroupStyle.ApplyStyle(externalGroups, localGroups, delegate (Func<int, double> evalFunc) { _cachedSymbolSizeForIndexFunction = evalFunc; });
			}

			// bar position
			BarWidthPositionGroupStyle bwp = PlotGroupStyle.GetStyleToApply<BarWidthPositionGroupStyle>(externalGroups, localGroups);
			if (null != bwp && !_doNotShiftHorizontalPosition)
			{
				double innerGapW, outerGapW, width, lpos;
				bwp.Apply(out innerGapW, out outerGapW, out width, out lpos);
				_cachedLogicalShiftOfIndependent = lpos + width / 2;
			}
			else
			{
				_cachedLogicalShiftOfIndependent = 0;
			}
		}

		public void Paint(System.Drawing.Graphics g, IPlotArea layer, Altaxo.Graph.Gdi.Plot.Data.Processed2DPlotData pdata, Processed2DPlotData prevItemData, Processed2DPlotData nextItemData)
		{
			if (_isHorizontalStyle)
				PaintXErrorBars(g, layer, pdata);
			else
				PaintYErrorBars(g, layer, pdata);
		}

		protected void PaintYErrorBars(System.Drawing.Graphics g, IPlotArea layer, Altaxo.Graph.Gdi.Plot.Data.Processed2DPlotData pdata)
		{
			// Plot error bars for the dependent variable (y)
			PlotRangeList rangeList = pdata.RangeList;
			PointF[] ptArray = pdata.PlotPointsInAbsoluteLayerCoordinates;
			INumericColumn posErrCol = _positiveErrorColumn.Document;
			INumericColumn negErrCol = _negativeErrorColumn.Document;

			if (posErrCol == null && negErrCol == null)
				return; // nothing to do if both error columns are null

			System.Drawing.Drawing2D.GraphicsPath errorBarPath = new System.Drawing.Drawing2D.GraphicsPath();

			Region oldClippingRegion = g.Clip;
			Region newClip = (Region)oldClippingRegion.Clone();
			var strokePen = _cachedColorForIndexFunction == null ? _strokePen : _strokePen.Clone();

			foreach (PlotRange r in rangeList)
			{
				int lower = r.LowerBound;
				int upper = r.UpperBound;
				int offset = r.OffsetToOriginal;

				for (int j = lower; j < upper; j++)
				{
					double symbolSize = null == _cachedSymbolSizeForIndexFunction ? _symbolSize : _cachedSymbolSizeForIndexFunction(j + offset);
					if (null != _cachedColorForIndexFunction)
						strokePen.Color = GdiColorHelper.ToNamedColor(_cachedColorForIndexFunction(j + offset), "VariableColor");

					AltaxoVariant y = pdata.GetYPhysical(j + offset);
					Logical3D lm = layer.GetLogical3D(pdata, j + offset);
					lm.RX += _cachedLogicalShiftOfIndependent;
					if (lm.IsNaN)
						continue;

					Logical3D lh = lm;
					Logical3D ll = lm;
					bool lhvalid = false;
					bool llvalid = false;
					if (posErrCol != null)
					{
						lh.RY = layer.YAxis.PhysicalVariantToNormal(y + Math.Abs(posErrCol[j + offset]));
						lhvalid = !lh.IsNaN;
					}
					if (negErrCol != null)
					{
						ll.RY = layer.YAxis.PhysicalVariantToNormal(y - Math.Abs(negErrCol[j + offset]));
						llvalid = !ll.IsNaN;
					}
					if (!(lhvalid || llvalid))
						continue; // nothing to do for this point if both pos and neg logical point are invalid.

					// now paint the error bar
					if (_symbolGap) // if symbol gap, then clip the painting, exclude a rectangle of size symbolSize x symbolSize
					{
						double xlm, ylm;
						layer.CoordinateSystem.LogicalToLayerCoordinates(lm, out xlm, out ylm);
						newClip.Union(oldClippingRegion);
						newClip.Exclude(new RectangleF((float)(xlm - symbolSize / 2), (float)(ylm - symbolSize / 2), (float)(symbolSize), (float)(symbolSize)));
						g.Clip = newClip;
					}

					if (lhvalid && llvalid)
					{
						errorBarPath.Reset();
						layer.CoordinateSystem.GetIsoline(errorBarPath, ll, lm);
						layer.CoordinateSystem.GetIsoline(errorBarPath, lm, lh);
						g.DrawPath(strokePen, errorBarPath);
					}
					else if (llvalid)
					{
						layer.CoordinateSystem.DrawIsoline(g, strokePen, ll, lm);
					}
					else if (lhvalid)
					{
						layer.CoordinateSystem.DrawIsoline(g, strokePen, lm, lh);
					}

					// now the end bars
					if (_showEndBars)
					{
						if (lhvalid)
						{
							PointD2D outDir;
							layer.CoordinateSystem.GetNormalizedDirection(lm, lh, 1, new Logical3D(1, 0), out outDir);
							outDir = outDir * (symbolSize / 2);
							double xlay, ylay;
							layer.CoordinateSystem.LogicalToLayerCoordinates(lh, out xlay, out ylay);
							// Draw a line from x,y to
							g.DrawLine(strokePen, (float)(xlay - outDir.X), (float)(ylay - outDir.Y), (float)(xlay + outDir.X), (float)(ylay + outDir.Y));
						}

						if (llvalid)
						{
							PointD2D outDir;
							layer.CoordinateSystem.GetNormalizedDirection(lm, ll, 1, new Logical3D(1, 0), out outDir);
							outDir = outDir * (symbolSize / 2);
							double xlay, ylay;
							layer.CoordinateSystem.LogicalToLayerCoordinates(ll, out xlay, out ylay);
							// Draw a line from x,y to
							g.DrawLine(strokePen, (float)(xlay - outDir.X), (float)(ylay - outDir.Y), (float)(xlay + outDir.X), (float)(ylay + outDir.Y));
						}
					}
				}
			}

			g.Clip = oldClippingRegion;
		}

		protected void PaintXErrorBars(System.Drawing.Graphics g, IPlotArea layer, Altaxo.Graph.Gdi.Plot.Data.Processed2DPlotData pdata)
		{
			// Plot error bars for the independent variable (x)
			PlotRangeList rangeList = pdata.RangeList;
			PointF[] ptArray = pdata.PlotPointsInAbsoluteLayerCoordinates;
			INumericColumn posErrCol = _positiveErrorColumn.Document;
			INumericColumn negErrCol = _negativeErrorColumn.Document;

			if (posErrCol == null && negErrCol == null)
				return; // nothing to do if both error columns are null

			System.Drawing.Drawing2D.GraphicsPath errorBarPath = new System.Drawing.Drawing2D.GraphicsPath();

			Region oldClippingRegion = g.Clip;
			Region newClip = (Region)oldClippingRegion.Clone();

			var strokePen = _cachedColorForIndexFunction == null ? _strokePen : _strokePen.Clone();

			foreach (PlotRange r in rangeList)
			{
				int lower = r.LowerBound;
				int upper = r.UpperBound;
				int offset = r.OffsetToOriginal;

				for (int j = lower; j < upper; j++)
				{
					double symbolSize = null == _cachedSymbolSizeForIndexFunction ? _symbolSize : _cachedSymbolSizeForIndexFunction(j + offset);
					if (null != _cachedColorForIndexFunction)
						strokePen.Color = GdiColorHelper.ToNamedColor(_cachedColorForIndexFunction(j + offset), "VariableColor");

					AltaxoVariant x = pdata.GetXPhysical(j + offset);
					Logical3D lm = layer.GetLogical3D(pdata, j + offset);
					lm.RX += _cachedLogicalShiftOfIndependent;
					if (lm.IsNaN)
						continue;

					Logical3D lh = lm;
					Logical3D ll = lm;
					bool lhvalid = false;
					bool llvalid = false;
					if (posErrCol != null)
					{
						lh.RX = layer.XAxis.PhysicalVariantToNormal(x + Math.Abs(posErrCol[j + offset]));
						lhvalid = !lh.IsNaN;
					}
					if (negErrCol != null)
					{
						ll.RX = layer.XAxis.PhysicalVariantToNormal(x - Math.Abs(negErrCol[j + offset]));
						llvalid = !ll.IsNaN;
					}
					if (!(lhvalid || llvalid))
						continue; // nothing to do for this point if both pos and neg logical point are invalid.

					// now paint the error bar
					if (_symbolGap) // if symbol gap, then clip the painting, exclude a rectangle of size symbolSize x symbolSize
					{
						double xlm, ylm;
						layer.CoordinateSystem.LogicalToLayerCoordinates(lm, out xlm, out ylm);
						newClip.Union(oldClippingRegion);
						newClip.Exclude(new RectangleF((float)(xlm - symbolSize / 2), (float)(ylm - symbolSize / 2), (float)(symbolSize), (float)(symbolSize)));
						g.Clip = newClip;
					}

					if (lhvalid && llvalid)
					{
						errorBarPath.Reset();
						layer.CoordinateSystem.GetIsoline(errorBarPath, ll, lm);
						layer.CoordinateSystem.GetIsoline(errorBarPath, lm, lh);
						g.DrawPath(strokePen, errorBarPath);
					}
					else if (llvalid)
					{
						layer.CoordinateSystem.DrawIsoline(g, strokePen, ll, lm);
					}
					else if (lhvalid)
					{
						layer.CoordinateSystem.DrawIsoline(g, strokePen, lm, lh);
					}

					// now the end bars
					if (_showEndBars)
					{
						if (lhvalid)
						{
							PointD2D outDir;
							layer.CoordinateSystem.GetNormalizedDirection(lm, lh, 1, new Logical3D(0, 1), out outDir);
							outDir = outDir * (symbolSize / 2);
							double xlay, ylay;
							layer.CoordinateSystem.LogicalToLayerCoordinates(lh, out xlay, out ylay);
							// Draw a line from x,y to
							g.DrawLine(strokePen, (float)(xlay - outDir.X), (float)(ylay - outDir.Y), (float)(xlay + outDir.X), (float)(ylay + outDir.Y));
						}

						if (llvalid)
						{
							PointD2D outDir;
							layer.CoordinateSystem.GetNormalizedDirection(lm, ll, 1, new Logical3D(0, 1), out outDir);
							outDir = outDir * (symbolSize / 2);
							double xlay, ylay;
							layer.CoordinateSystem.LogicalToLayerCoordinates(ll, out xlay, out ylay);
							// Draw a line from x,y to
							g.DrawLine(strokePen, (float)(xlay - outDir.X), (float)(ylay - outDir.Y), (float)(xlay + outDir.X), (float)(ylay + outDir.Y));
						}
					}
				}
			}

			g.Clip = oldClippingRegion;
		}

		public System.Drawing.RectangleF PaintSymbol(System.Drawing.Graphics g, System.Drawing.RectangleF bounds)
		{
			// Error bars are not painted in the symbol
			return bounds;
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
			Report(_positiveErrorColumn, this, "PositiveErrorColumn");
			Report(_negativeErrorColumn, this, "NegativeErrorColumn");
		}

		#endregion IDocumentNode Members
	}
}