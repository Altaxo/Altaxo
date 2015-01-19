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
using System.Text;

namespace Altaxo.Graph.Gdi.Plot.Styles
{
	using Altaxo.Main;
	using Graph.Plot.Data;
	using Graph.Plot.Groups;
	using Plot.Data;
	using Plot.Groups;

	/// <summary>
	///
	/// </summary>
	public class BarGraphPlotStyle
		:
		Main.SuspendableDocumentNodeWithEventArgs,
		IG2DPlotStyle
	{
		/// <summary>
		/// Relative gap between the bars belonging to the same x-value (relative to the width of a single bar).
		/// A value of 0.5 means that the gap has half of the width of one bar.
		/// </summary>
		private double _relInnerGapWidth = 0.5;

		/// <summary>
		/// Relative gap between the bars between two consecutive x-values (relative to the width of a single bar).
		/// A value of 1 means that the gap has the same width than one bar.
		/// </summary>
		private double _relOuterGapWidth = 1.0;

		/// <summary>
		/// Indicates whether the fill color is dependent (can be set by the ColorGroupStyle) or not.
		/// </summary>
		private bool _independentFillColor;

		/// <summary>
		/// Brush to fill the bar.
		/// </summary>
		private BrushX _fillBrush = new BrushX(NamedColors.Red);

		/// <summary>
		/// Indicates whether the frame color is dependent (can be set by the ColorGroupStyle) or not.
		/// </summary>
		private bool _independentFrameColor = true; // true because the standard value is transparent

		/// <summary>
		/// Pen used to frame the bar. Can be null!
		/// </summary>
		private PenX _framePen;

		/// <summary>
		/// Indicates whether _baseValue is a physical value or a logical value.
		/// </summary>
		private bool _usePhysicalBaseValue;

		/// <summary>
		/// The y-value where the item normally starts. This is either a logical value (_usePhysicalBaseValue==false) or a physical value.
		/// </summary>
		private Altaxo.Data.AltaxoVariant _baseValue = new Altaxo.Data.AltaxoVariant(0.0);

		/// <summary>
		/// If true, the bar starts at the y value of the previous plot item.
		/// </summary>
		private bool _startAtPreviousItem;

		/// <summary>
		/// Value in logical units, indicating the gap between previous item an this item.
		/// </summary>
		private double _previousItemYGap;

		/// <summary>
		/// Actual width of the item in logical coordinates.
		/// </summary>
		private double _width;

		/// <summary>
		/// Actual position of the item in logical coordinates relative to the logical x coordinate of the item's point.
		/// </summary>
		private double _position;

		/// <summary>If this function is set, the color is determined by calling this function on the index into the data.</summary>
		[field: NonSerialized]
		protected Func<int, Color> _cachedColorForIndexFunction;

		#region Serialization

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(BarGraphPlotStyle), 0)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				BarGraphPlotStyle s = (BarGraphPlotStyle)obj;
				info.AddValue("InnerGapWidth", s._relInnerGapWidth);
				info.AddValue("OuterGapWidth", s._relOuterGapWidth);
				info.AddValue("IndependentColor", s._independentFillColor);
				info.AddValue("FillBrush", s._fillBrush);
				info.AddValue("FramePen", s._framePen);
				info.AddValue("UsePhysicalBaseValue", s._usePhysicalBaseValue);
				info.AddValue("BaseValue", (object)s._baseValue);
				info.AddValue("StartAtPrevious", s._startAtPreviousItem);
				info.AddValue("PreviousItemGap", s._previousItemYGap);
				info.AddValue("ActualWidth", s._width);
				info.AddValue("ActaulPosition", s._position);
			}

			protected virtual BarGraphPlotStyle SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				BarGraphPlotStyle s = null != o ? (BarGraphPlotStyle)o : new BarGraphPlotStyle();

				s._relInnerGapWidth = info.GetDouble("InnerGapWidth");
				s._relOuterGapWidth = info.GetDouble("OuterGapWidth");
				s._independentFillColor = info.GetBoolean("IndependentColor");
				s.FillBrush = (BrushX)info.GetValue("FillBrush", s);
				s.FramePen = (PenX)info.GetValue("FramePen", s);
				s._usePhysicalBaseValue = info.GetBoolean("UsePhysicalBaseValue");
				s._baseValue = (Altaxo.Data.AltaxoVariant)info.GetValue("BaseValue", s);
				s._startAtPreviousItem = info.GetBoolean("StartAtPrevious");
				s._previousItemYGap = info.GetDouble("PreviousItemGap");
				s._width = info.GetDouble("ActualWidth");
				s._position = info.GetDouble("ActualPosition");

				return s;
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				BarGraphPlotStyle s = SDeserialize(o, info, parent);
				return s;
			}
		}

		/// <summary>
		/// <para>Date: 2012-10-06</para>
		/// <para>renamed: IndependentColor in IndependentFillColor</para>
		/// <para>added: IndependentFrameColor</para>
		/// </summary>
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(BarGraphPlotStyle), 1)]
		private class XmlSerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				BarGraphPlotStyle s = (BarGraphPlotStyle)obj;
				info.AddValue("InnerGapWidth", s._relInnerGapWidth);
				info.AddValue("OuterGapWidth", s._relOuterGapWidth);
				info.AddValue("IndependentFillColor", s._independentFillColor);
				info.AddValue("FillBrush", s._fillBrush);
				info.AddValue("IndependentFrameColor", s._independentFrameColor);
				info.AddValue("FramePen", s._framePen);
				info.AddValue("UsePhysicalBaseValue", s._usePhysicalBaseValue);
				info.AddValue("BaseValue", (object)s._baseValue);
				info.AddValue("StartAtPrevious", s._startAtPreviousItem);
				info.AddValue("PreviousItemGap", s._previousItemYGap);
				info.AddValue("ActualWidth", s._width);
				info.AddValue("ActaulPosition", s._position);
			}

			protected virtual BarGraphPlotStyle SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				BarGraphPlotStyle s = null != o ? (BarGraphPlotStyle)o : new BarGraphPlotStyle();

				s._relInnerGapWidth = info.GetDouble("InnerGapWidth");
				s._relOuterGapWidth = info.GetDouble("OuterGapWidth");
				s._independentFillColor = info.GetBoolean("IndependentFillColor");
				s.FillBrush = (BrushX)info.GetValue("FillBrush", s);
				s._independentFrameColor = info.GetBoolean("IndependentFrameColor");
				s.FramePen = (PenX)info.GetValue("FramePen", s);
				s._usePhysicalBaseValue = info.GetBoolean("UsePhysicalBaseValue");
				s._baseValue = (Altaxo.Data.AltaxoVariant)info.GetValue("BaseValue", s);
				s._startAtPreviousItem = info.GetBoolean("StartAtPrevious");
				s._previousItemYGap = info.GetDouble("PreviousItemGap");
				s._width = info.GetDouble("ActualWidth");
				s._position = info.GetDouble("ActualPosition");

				return s;
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				BarGraphPlotStyle s = SDeserialize(o, info, parent);
				return s;
			}
		}

		#endregion Serialization

		public BarGraphPlotStyle()
		{
		}

		public virtual bool CopyFrom(object obj)
		{
			if (object.ReferenceEquals(this, obj))
				return true;

			var from = obj as BarGraphPlotStyle;
			if (null != from)
			{
				this._relInnerGapWidth = from._relInnerGapWidth;
				this._relOuterGapWidth = from._relOuterGapWidth;
				this._width = from._width;
				this._position = from._position;
				this._independentFillColor = from._independentFillColor;
				this._fillBrush = from._fillBrush == null ? null : from._fillBrush.Clone();
				this._independentFrameColor = from._independentFrameColor;
				this._framePen = from._framePen == null ? null : from._framePen.Clone();
				this._startAtPreviousItem = from._startAtPreviousItem;
				this._previousItemYGap = from._previousItemYGap;
				this._usePhysicalBaseValue = from._usePhysicalBaseValue;
				this._baseValue = from._baseValue;
				//this._parent = from._parent;
				return true;
			}
			return false;
		}

		protected override IEnumerable<DocumentNodeAndName> GetDocumentNodeChildrenWithName()
		{
			if (_framePen != null)
				yield return new Main.DocumentNodeAndName(_framePen, "FramePen");
			if (_fillBrush != null)
				yield return new Main.DocumentNodeAndName(_fillBrush, "FillBrush");
		}

		public BarGraphPlotStyle(BarGraphPlotStyle from)
		{
			CopyFrom(from);
		}

		public BarGraphPlotStyle Clone()
		{
			return new BarGraphPlotStyle(this);
		}

		object ICloneable.Clone()
		{
			return new BarGraphPlotStyle(this);
		}

		public bool IsColorReceiver
		{
			get
			{
				return !(this._independentFillColor && this._independentFrameColor);
			}
		}

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
					EhSelfChanged(EventArgs.Empty);
			}
		}

		public BrushX FillBrush
		{
			get { return _fillBrush; }
			set
			{
				var oldValue = _fillBrush;
				_fillBrush = value;
				if (!object.ReferenceEquals(value, oldValue))
					EhSelfChanged(EventArgs.Empty);
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
					EhSelfChanged(EventArgs.Empty);
			}
		}

		public PenX FramePen
		{
			get { return _framePen; }
			set
			{
				var oldValue = _framePen;
				_framePen = value;
				if (!object.ReferenceEquals(value, oldValue))
					EhSelfChanged(EventArgs.Empty);
			}
		}

		public double InnerGap
		{
			get { return _relInnerGapWidth; }
			set { _relInnerGapWidth = value; }
		}

		public double OuterGap
		{
			get { return _relOuterGapWidth; }
			set
			{
				var oldValue = _relOuterGapWidth;
				_relOuterGapWidth = value;
				if (value != oldValue)
					EhSelfChanged(EventArgs.Empty);
			}
		}

		public double PreviousItemYGap
		{
			get { return _previousItemYGap; }
			set
			{
				var oldValue = _previousItemYGap;
				_previousItemYGap = value;
				if (value != oldValue)
					EhSelfChanged(EventArgs.Empty);
			}
		}

		public bool StartAtPreviousItem
		{
			get { return _startAtPreviousItem; }
			set
			{
				var oldValue = _startAtPreviousItem;
				_startAtPreviousItem = value;
				if (value != oldValue)
					EhSelfChanged(EventArgs.Empty);
			}
		}

		public bool UsePhysicalBaseValue
		{
			get { return _usePhysicalBaseValue; }
			set
			{
				var oldValue = _usePhysicalBaseValue;
				_usePhysicalBaseValue = value;
				if (value != oldValue)
					EhSelfChanged(EventArgs.Empty);
			}
		}

		public Altaxo.Data.AltaxoVariant BaseValue
		{
			get { return _baseValue; }
			set
			{
				var oldValue = _baseValue;
				_baseValue = value;
				if (oldValue != value)
					EhSelfChanged(EventArgs.Empty);
			}
		}

		#region IG2DPlotStyle Members

		public void CollectExternalGroupStyles(PlotGroupStyleCollection externalGroups)
		{
			BarWidthPositionGroupStyle.AddExternalGroupStyle(externalGroups);
		}

		public void CollectLocalGroupStyles(PlotGroupStyleCollection externalGroups, PlotGroupStyleCollection localGroups)
		{
			BarWidthPositionGroupStyle.AddLocalGroupStyle(externalGroups, localGroups);
		}

		public void PrepareGroupStyles(PlotGroupStyleCollection externalGroups, PlotGroupStyleCollection localGroups, IPlotArea layer, Processed2DPlotData pdata)
		{
			// first, we have to calculate the span of logical values from the minimum logical value to the maximum logical value
			int numberOfItems = 0;
			double minLogical = double.MaxValue;
			double maxLogical = double.MinValue;

			foreach (int originalRowIndex in pdata.RangeList.OriginalRowIndices())
			{
				double logicalX = layer.XAxis.PhysicalVariantToNormal(pdata.GetXPhysical(originalRowIndex));
				numberOfItems++;
				if (logicalX < minLogical)
					minLogical = logicalX;
				if (logicalX > maxLogical)
					maxLogical = logicalX;
			}

			BarWidthPositionGroupStyle.IntendToApply(externalGroups, localGroups, numberOfItems, minLogical, maxLogical);
			BarWidthPositionGroupStyle bwp = PlotGroupStyle.GetStyleToInitialize<BarWidthPositionGroupStyle>(externalGroups, localGroups);
			if (null != bwp)
				bwp.Initialize(_relInnerGapWidth, _relOuterGapWidth);

			if (!_independentFillColor && null != _fillBrush)
				ColorGroupStyle.PrepareStyle(externalGroups, localGroups, delegate() { return this._fillBrush.Color; });
			else if (!_independentFrameColor && null != _framePen) // else if is used here because fill color has precedence over frame color
				ColorGroupStyle.PrepareStyle(externalGroups, localGroups, delegate() { return this._framePen.Color; });
		}

		public void ApplyGroupStyles(PlotGroupStyleCollection externalGroups, PlotGroupStyleCollection localGroups)
		{
			_cachedColorForIndexFunction = null;

			BarWidthPositionGroupStyle bwp = PlotGroupStyle.GetStyleToApply<BarWidthPositionGroupStyle>(externalGroups, localGroups);
			if (null != bwp)
				bwp.Apply(out _relInnerGapWidth, out _relOuterGapWidth, out _width, out _position);

			if (!_independentFillColor)
			{
				if (null == _fillBrush) _fillBrush = new BrushX(ColorManagement.BuiltinDarkPlotColorSet.Instance[0]);
				ColorGroupStyle.ApplyStyle(externalGroups, localGroups, delegate(NamedColor c) { this._fillBrush.Color = c; });

				// but if there is a color evaluation function, then use that function with higher priority
				VariableColorGroupStyle.ApplyStyle(externalGroups, localGroups, delegate(Func<int, Color> evalFunc) { _cachedColorForIndexFunction = evalFunc; });
			}

			if (!_independentFrameColor)
			{
				if (null == _framePen) _framePen = new PenX(ColorManagement.BuiltinDarkPlotColorSet.Instance[0]);
				ColorGroupStyle.ApplyStyle(externalGroups, localGroups, delegate(NamedColor c) { this._framePen.Color = c; });

				// but if there is a color evaluation function, then use that function with higher priority
				VariableColorGroupStyle.ApplyStyle(externalGroups, localGroups, delegate(Func<int, Color> evalFunc) { _cachedColorForIndexFunction = evalFunc; });
			}
		}

		public void Paint(System.Drawing.Graphics g, IPlotArea layer, Processed2DPlotData pdata, Processed2DPlotData prevItemData, Processed2DPlotData nextItemData)
		{
			PlotRangeList rangeList = pdata.RangeList;
			System.Drawing.PointF[] ptArray = pdata.PlotPointsInAbsoluteLayerCoordinates;

			// paint the style

			double xleft, xright, ytop, ybottom;
			layer.CoordinateSystem.LogicalToLayerCoordinates(new Logical3D(0, 0), out xleft, out ybottom);
			layer.CoordinateSystem.LogicalToLayerCoordinates(new Logical3D(1, 1), out xright, out ytop);
			float xe = (float)xright;
			float ye = (float)ybottom;

			GraphicsPath path = new GraphicsPath();

			double globalBaseValue;
			if (_usePhysicalBaseValue)
			{
				globalBaseValue = layer.YAxis.PhysicalVariantToNormal(_baseValue);
				if (double.IsNaN(globalBaseValue))
					globalBaseValue = 0;
			}
			else
			{
				globalBaseValue = _baseValue.ToDouble();
			}

			bool useVariableFillColor = null != _fillBrush && null != _cachedColorForIndexFunction && !_independentFillColor;
			bool useVariableFrameColor = null != _framePen && null != _cachedColorForIndexFunction && !_independentFrameColor;

			var fillBrush = _fillBrush == null ? null : useVariableFillColor ? _fillBrush.Clone() : _fillBrush;
			var framePen = _framePen == null ? null : useVariableFrameColor ? _framePen.Clone() : _framePen;

			int j = -1;
			foreach (int originalRowIndex in pdata.RangeList.OriginalRowIndices())
			{
				j++;

				double xcn = layer.XAxis.PhysicalVariantToNormal(pdata.GetXPhysical(originalRowIndex));
				double xln = xcn + _position;
				double xrn = xln + _width;

				double ycn = layer.YAxis.PhysicalVariantToNormal(pdata.GetYPhysical(originalRowIndex));
				double ynbase = globalBaseValue;

				if (_startAtPreviousItem && pdata.PreviousItemData != null)
				{
					double prevstart = layer.YAxis.PhysicalVariantToNormal(pdata.PreviousItemData.GetYPhysical(originalRowIndex));
					if (!double.IsNaN(prevstart))
					{
						ynbase = prevstart;
						ynbase += Math.Sign(ynbase - globalBaseValue) * _previousItemYGap;
					}
				}

				path.Reset();
				layer.CoordinateSystem.GetIsoline(path, new Logical3D(xln, ynbase), new Logical3D(xln, ycn));
				layer.CoordinateSystem.GetIsoline(path, new Logical3D(xln, ycn), new Logical3D(xrn, ycn));
				layer.CoordinateSystem.GetIsoline(path, new Logical3D(xrn, ycn), new Logical3D(xrn, ynbase));
				layer.CoordinateSystem.GetIsoline(path, new Logical3D(xrn, ynbase), new Logical3D(xln, ynbase));
				path.CloseFigure();

				if (null != fillBrush)
				{
					if (useVariableFillColor)
						fillBrush.Color = GdiColorHelper.ToNamedColor(_cachedColorForIndexFunction(originalRowIndex), "VariableColor");

					fillBrush.SetEnvironment(path.GetBounds(), BrushX.GetEffectiveMaximumResolution(g, 1));
					g.FillPath(fillBrush, path);
				}

				if (null != framePen)
				{
					if (useVariableFrameColor)
						framePen.Color = GdiColorHelper.ToNamedColor(_cachedColorForIndexFunction(originalRowIndex), "VariableColor");

					framePen.SetEnvironment(path.GetBounds(), BrushX.GetEffectiveMaximumResolution(g, 1));
					g.DrawPath(framePen, path);
				}
			}
		}

		public RectangleF PaintSymbol(Graphics g, RectangleF bounds)
		{
			bounds.Inflate(0, -bounds.Height / 4);
			if (null != _fillBrush)
			{
				_fillBrush.SetEnvironment(bounds, BrushX.GetEffectiveMaximumResolution(g, 1));
				g.FillRectangle(_fillBrush, bounds);
			}
			if (null != _framePen)
			{
				_framePen.SetEnvironment(bounds, BrushX.GetEffectiveMaximumResolution(g, 1));
				g.DrawRectangle(_framePen, bounds.X, bounds.Y, bounds.Width, bounds.Height);
			}
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
		}

		#endregion IDocumentNode Members
	}
}