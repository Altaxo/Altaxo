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
using System.Text;

namespace Altaxo.Graph.Graph3D.Plot.Styles
{
	using Altaxo.Data;
	using Altaxo.Main;
	using Drawing;
	using Drawing.D3D;
	using Drawing.D3D.Material;
	using Geometry;
	using Graph.Plot.Data;
	using Graph.Plot.Groups;
	using GraphicsContext;
	using Plot.Data;
	using Plot.Groups;

	/// <summary>
	///
	/// </summary>
	public class BarGraphPlotStyle
		:
		Main.SuspendableDocumentNodeWithEventArgs,
		IG3DPlotStyle
	{
		/// <summary>
		/// Relative gap between the bars belonging to the same x-value (relative to the width of a single bar).
		/// A value of 0.5 means that the gap has half of the width of one bar.
		/// </summary>
		private double _relInnerGapX = 0.5;

		/// <summary>
		/// Relative gap between the bars between two consecutive x-values (relative to the width of a single bar).
		/// A value of 1 means that the gap has the same width than one bar.
		/// </summary>
		private double _relOuterGapX = 1.0;

		/// <summary>
		/// Relative gap between the bars belonging to the same y-value (relative to the depth of a single bar).
		/// A value of 0.5 means that the gap has half of the depth of one bar.
		/// </summary>
		private double _relInnerGapY = 0.5;

		/// <summary>
		/// Relative gap between the bars between two consecutive y-values (relative to the depth of a single bar).
		/// A value of 1 means that the gap has the same depth than one bar.
		/// </summary>
		private double _relOuterGapY = 1.0;

		/// <summary>
		/// Indicates whether the  color is dependent (can be set by the ColorGroupStyle) or not.
		/// </summary>
		private bool _independentColor = true; // true because the standard value is transparent

		/// <summary>
		/// Pen used to draw the bar.
		/// </summary>
		private PenX3D _pen;

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
		private double _previousItemZGap;

		/// <summary>
		/// Actual width of the item in logical coordinates.
		/// </summary>
		private double _xSizeLogical;

		/// <summary>
		/// Actual depth of the item in logical coordinates.
		/// </summary>
		private double _ySizeLogical;

		/// <summary>
		/// Actual position of the item in logical coordinates relative to the logical x coordinate of the item's point.
		/// </summary>
		private double _xOffsetLogical;

		/// <summary>
		/// Actual position of the item in logical coordinates relative to the logical y coordinate of the item's point.
		/// </summary>
		private double _yOffsetLogical;

		/// <summary>If this function is set, the color is determined by calling this function on the index into the data.</summary>
		[field: NonSerialized]
		protected Func<int, System.Drawing.Color> _cachedColorForIndexFunction;

		#region Serialization

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(BarGraphPlotStyle), 0)]
		private class XmlSerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				BarGraphPlotStyle s = (BarGraphPlotStyle)obj;
				info.AddValue("InnerGapX", s._relInnerGapX);
				info.AddValue("OuterGapX", s._relOuterGapX);
				info.AddValue("IndependentColor", s._independentColor);
				info.AddValue("Pen", s._pen);
				info.AddValue("UsePhysicalBaseValue", s._usePhysicalBaseValue);
				info.AddValue("BaseValue", (object)s._baseValue);
				info.AddValue("StartAtPrevious", s._startAtPreviousItem);
				info.AddValue("PreviousItemGap", s._previousItemZGap);
				info.AddValue("ActualWidth", s._xSizeLogical);
				info.AddValue("ActualPosition", s._xOffsetLogical);
			}

			protected virtual BarGraphPlotStyle SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				BarGraphPlotStyle s = null != o ? (BarGraphPlotStyle)o : new BarGraphPlotStyle();

				s._relInnerGapX = info.GetDouble("InnerGapWidth");
				s._relOuterGapX = info.GetDouble("OuterGapWidth");
				s._independentColor = info.GetBoolean("IndependentColor");
				s._pen = (PenX3D)info.GetValue("Pen", s);
				s._usePhysicalBaseValue = info.GetBoolean("UsePhysicalBaseValue");
				s._baseValue = (Altaxo.Data.AltaxoVariant)info.GetValue("BaseValue", s);
				s._startAtPreviousItem = info.GetBoolean("StartAtPrevious");
				s._previousItemZGap = info.GetDouble("PreviousItemGap");
				s._xSizeLogical = info.GetDouble("ActualWidth");
				s._xOffsetLogical = info.GetDouble("ActualPosition");

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
				this._relInnerGapX = from._relInnerGapX;
				this._relOuterGapX = from._relOuterGapX;
				this._relInnerGapY = from._relInnerGapY;
				this._relOuterGapY = from._relOuterGapY;
				this._xSizeLogical = from._xSizeLogical;
				this._xOffsetLogical = from._xOffsetLogical;
				this._ySizeLogical = from._ySizeLogical;
				this._yOffsetLogical = from._yOffsetLogical;

				this._independentColor = from._independentColor;
				this._pen = from._pen;
				this._startAtPreviousItem = from._startAtPreviousItem;
				this._previousItemZGap = from._previousItemZGap;
				this._usePhysicalBaseValue = from._usePhysicalBaseValue;
				this._baseValue = from._baseValue;
				EhSelfChanged();
				return true;
			}
			return false;
		}

		protected override IEnumerable<DocumentNodeAndName> GetDocumentNodeChildrenWithName()
		{
			yield break;
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
				return !this._independentColor;
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
				var oldValue = _independentColor;
				_independentColor = value;
				if (value != oldValue)
					EhSelfChanged(EventArgs.Empty);
			}
		}

		public PenX3D Pen
		{
			get { return _pen; }
			set
			{
				var oldValue = _pen;
				_pen = value;
				if (!object.ReferenceEquals(value, oldValue))
					EhSelfChanged(EventArgs.Empty);
			}
		}

		public double InnerGapX
		{
			get { return _relInnerGapX; }
			set { _relInnerGapX = value; }
		}

		public double OuterGapX
		{
			get { return _relOuterGapX; }
			set
			{
				var oldValue = _relOuterGapX;
				_relOuterGapX = value;
				if (value != oldValue)
					EhSelfChanged(EventArgs.Empty);
			}
		}

		public double PreviousItemZGap
		{
			get { return _previousItemZGap; }
			set
			{
				var oldValue = _previousItemZGap;
				_previousItemZGap = value;
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

		#region IG3DPlotStyle Members

		public void CollectExternalGroupStyles(PlotGroupStyleCollection externalGroups)
		{
			BarSizePosition3DGroupStyle.AddExternalGroupStyle(externalGroups);
		}

		public void CollectLocalGroupStyles(PlotGroupStyleCollection externalGroups, PlotGroupStyleCollection localGroups)
		{
			BarSizePosition3DGroupStyle.AddLocalGroupStyle(externalGroups, localGroups);
		}

		public void PrepareGroupStyles(PlotGroupStyleCollection externalGroups, PlotGroupStyleCollection localGroups, IPlotArea layer, Processed3DPlotData pdata)
		{
			// first, we have to calculate the span of logical values from the minimum logical value to the maximum logical value
			int numberOfItems = 0;

			if (null != pdata)
			{
				double minLogicalX = double.MaxValue;
				double maxLogicalX = double.MinValue;
				double minLogicalY = double.MaxValue;
				double maxLogicalY = double.MinValue;
				foreach (int originalRowIndex in pdata.RangeList.OriginalRowIndices())
				{
					numberOfItems++;

					double logicalX = layer.XAxis.PhysicalVariantToNormal(pdata.GetXPhysical(originalRowIndex));
					if (logicalX < minLogicalX)
						minLogicalX = logicalX;
					if (logicalX > maxLogicalX)
						maxLogicalX = logicalX;

					double logicalY = layer.YAxis.PhysicalVariantToNormal(pdata.GetXPhysical(originalRowIndex));
					if (logicalY < minLogicalY)
						minLogicalY = logicalY;
					if (logicalY > maxLogicalY)
						maxLogicalY = logicalY;
				}

				BarSizePosition3DGroupStyle.IntendToApply(externalGroups, localGroups, numberOfItems, minLogicalX, maxLogicalX, minLogicalY, maxLogicalY);
			}
			BarSizePosition3DGroupStyle bwp = PlotGroupStyle.GetStyleToInitialize<BarSizePosition3DGroupStyle>(externalGroups, localGroups);
			if (null != bwp)
				bwp.Initialize(_relInnerGapX, _relOuterGapX, _relInnerGapY, _relOuterGapY);

			if (!_independentColor) // else if is used here because fill color has precedence over frame color
				ColorGroupStyle.PrepareStyle(externalGroups, localGroups, delegate () { return this._pen.Color; });
		}

		public void ApplyGroupStyles(PlotGroupStyleCollection externalGroups, PlotGroupStyleCollection localGroups)
		{
			_cachedColorForIndexFunction = null;

			BarSizePosition3DGroupStyle bwp = PlotGroupStyle.GetStyleToApply<BarSizePosition3DGroupStyle>(externalGroups, localGroups);
			if (null != bwp)
				bwp.Apply(
					out _relInnerGapX, out _relOuterGapX, out _xSizeLogical, out _xOffsetLogical,
					out _relInnerGapY, out _relOuterGapY, out _ySizeLogical, out _yOffsetLogical);

			if (!_independentColor)
			{
				ColorGroupStyle.ApplyStyle(externalGroups, localGroups, delegate (NamedColor c) { _pen = _pen.WithColor(c); });

				// but if there is a color evaluation function, then use that function with higher priority
				VariableColorGroupStyle.ApplyStyle(externalGroups, localGroups, delegate (Func<int, System.Drawing.Color> evalFunc) { _cachedColorForIndexFunction = evalFunc; });
			}
		}

		public void Paint(IGraphicsContext3D g, IPlotArea layer, Processed3DPlotData pdata, Processed3DPlotData prevItemData, Processed3DPlotData nextItemData)
		{
			PlotRangeList rangeList = pdata.RangeList;
			var ptArray = pdata.PlotPointsInAbsoluteLayerCoordinates;

			// paint the style

			PointD3D leftFrontBotton, rightBackTop;
			layer.CoordinateSystem.LogicalToLayerCoordinates(new Logical3D(0, 0, 0), out leftFrontBotton);
			layer.CoordinateSystem.LogicalToLayerCoordinates(new Logical3D(1, 1, 1), out rightBackTop);

			double globalBaseValue;
			if (_usePhysicalBaseValue)
			{
				globalBaseValue = layer.ZAxis.PhysicalVariantToNormal(_baseValue);
				if (double.IsNaN(globalBaseValue))
					globalBaseValue = 0;
			}
			else
			{
				globalBaseValue = _baseValue.ToDouble();
			}

			bool useVariableColor = null != _cachedColorForIndexFunction && !_independentColor;

			var pen = _pen;

			int j = -1;
			foreach (int originalRowIndex in pdata.RangeList.OriginalRowIndices())
			{
				j++;

				double xCenterLogical = layer.XAxis.PhysicalVariantToNormal(pdata.GetXPhysical(originalRowIndex));
				double xShiftedLogical = xCenterLogical + _xOffsetLogical;

				double yCenterLogical = layer.YAxis.PhysicalVariantToNormal(pdata.GetYPhysical(originalRowIndex));
				double yShiftedLogical = yCenterLogical + _yOffsetLogical;

				double zCenterLogical = layer.ZAxis.PhysicalVariantToNormal(pdata.GetZPhysical(originalRowIndex));
				double zBaseLogical = globalBaseValue;

				if (_startAtPreviousItem && pdata.PreviousItemData != null)
				{
					double prevstart = layer.ZAxis.PhysicalVariantToNormal(pdata.PreviousItemData.GetZPhysical(originalRowIndex));
					if (!double.IsNaN(prevstart))
					{
						zBaseLogical = prevstart;
						zBaseLogical += Math.Sign(zBaseLogical - globalBaseValue) * _previousItemZGap;
					}
				}

				if (useVariableColor)
					pen = pen.WithColor(GdiColorHelper.ToNamedColor(_cachedColorForIndexFunction(originalRowIndex), "VariableColor"));

				var isoLine = layer.CoordinateSystem.GetIsoline(new Logical3D(xShiftedLogical, yShiftedLogical, zBaseLogical), new Logical3D(xShiftedLogical, yShiftedLogical, zCenterLogical));
				g.DrawLine(pen, isoLine);
			}
		}

		public RectangleD3D PaintSymbol(IGraphicsContext3D g, RectangleD3D bounds)
		{
			bounds = bounds.WithPadding(0, 0, -bounds.SizeZ / 4);
			var heightBy2 = new VectorD3D(0, 0, bounds.SizeZ / 4);

			g.DrawLine(_pen, bounds.Center - heightBy2, bounds.Center + heightBy2);

			return bounds;
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

		public IEnumerable<Tuple<string, IReadableColumn, string, Action<IReadableColumn>>> GetAdditionallyUsedColumns()
		{
			return null;
		}

		#endregion IDocumentNode Members
	}
}