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
using Altaxo.Graph.Scales;
using Altaxo.Graph.Scales.Boundaries;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;

namespace Altaxo.Graph.Gdi.Plot.Groups
{
	using Plot.Data;

	/// <summary>
	/// Transforming plot style used for waterfall plots.
	/// </summary>
	public class WaterfallTransform
		:
		Main.SuspendableDocumentLeafNodeWithEventArgs,
		ICoordinateTransformingGroupStyle
	{
		/// <summary>User defined scale. The multiplication with _xinc results in the xinc that is used for the waterfall.</summary>
		private double _scaleXInc = 1;

		/// <summary>User defined scale. The multiplication with _yinc results in the yinc that is used for the waterfall.</summary>
		private double _scaleYInc = 1;

		/// <summary>If true, the actual plot item is clipped by the previous plot items.</summary>
		private bool _useClipping;

		// Cached values
		private double _xinc = 0;

		private double _yinc = 0;

		#region Serialization

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(WaterfallTransform), 0)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				WaterfallTransform s = (WaterfallTransform)obj;
				info.AddValue("XScale", s._scaleXInc);
				info.AddValue("YScale", s._scaleYInc);
				info.AddValue("UseClipping", s._useClipping);
				info.AddValue("XInc", s._xinc);
				info.AddValue("YInc", s._yinc);
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				WaterfallTransform s = null != o ? (WaterfallTransform)o : new WaterfallTransform();
				s._scaleXInc = info.GetDouble("XScale");
				s._scaleYInc = info.GetDouble("YScale");
				s._useClipping = info.GetBoolean("UseClipping");
				s._xinc = info.GetDouble("XInc");
				s._yinc = info.GetDouble("YInc");
				return s;
			}
		}

		#endregion Serialization

		public WaterfallTransform()
		{
		}

		/// <summary>
		/// Copy constructor of a waterfall plot style.
		/// </summary>
		/// <param name="from">The waterfall plot style to copy from.</param>
		public WaterfallTransform(WaterfallTransform from)
		{
			this._scaleXInc = from._scaleXInc;
			this._scaleYInc = from._scaleYInc;
			this._useClipping = from._useClipping;

			this._xinc = from._xinc;
			this._yinc = from._yinc;
		}

		public double XScale
		{
			get
			{
				return _scaleXInc;
			}
			set
			{
				var oldValue = _scaleXInc;
				_scaleXInc = value;

				if (oldValue != value)
					EhSelfChanged(EventArgs.Empty);
			}
		}

		public double YScale
		{
			get
			{
				return _scaleYInc;
			}
			set
			{
				var oldValue = _scaleYInc;
				_scaleYInc = value;
				if (oldValue != value)
					EhSelfChanged(EventArgs.Empty);
			}
		}

		public bool UseClipping
		{
			get
			{
				return _useClipping;
			}
			set
			{
				var oldValue = _useClipping;
				_useClipping = value;
				if (oldValue != value)
					EhSelfChanged(EventArgs.Empty);
			}
		}

		#region ICoordinateTransformingGroupStyle Members

		public void MergeXBoundsInto(IPlotArea layer, IPhysicalBoundaries pb, PlotItemCollection coll)
		{
			if (!(pb is NumericalBoundaries))
			{
				CoordinateTransformingStyleBase.MergeXBoundsInto(pb, coll);
				return;
			}

			NumericalBoundaries xbounds = (NumericalBoundaries)pb.Clone();
			xbounds.Reset();

			int nItems = 0;
			foreach (IGPlotItem pi in coll)
			{
				if (pi is IXBoundsHolder)
				{
					IXBoundsHolder xbpi = (IXBoundsHolder)pi;
					xbpi.MergeXBoundsInto(xbounds);
				}
				if (pi is G2DPlotItem)
					nItems++;
			}

			if (nItems == 0)
				_xinc = 0;
			else
				_xinc = (xbounds.UpperBound - xbounds.LowerBound) / nItems;

			int idx = 0;
			foreach (IGPlotItem pi in coll)
			{
				if (pi is IXBoundsHolder)
				{
					IXBoundsHolder xbpi = (IXBoundsHolder)pi;
					xbounds.Reset();
					xbpi.MergeXBoundsInto(xbounds);
					xbounds.Shift(_xinc * _scaleXInc * idx);
					pb.Add(xbounds);
				}
				if (pi is G2DPlotItem)
					idx++;
			}
		}

		public void MergeYBoundsInto(IPlotArea layer, IPhysicalBoundaries pb, PlotItemCollection coll)
		{
			if (!(pb is NumericalBoundaries))
			{
				CoordinateTransformingStyleBase.MergeYBoundsInto(pb, coll);
				return;
			}

			NumericalBoundaries ybounds = (NumericalBoundaries)pb.Clone();
			ybounds.Reset();

			int nItems = 0;
			foreach (IGPlotItem pi in coll)
			{
				if (pi is IYBoundsHolder)
				{
					IYBoundsHolder ybpi = (IYBoundsHolder)pi;
					ybpi.MergeYBoundsInto(ybounds);
				}
				if (pi is G2DPlotItem)
					nItems++;
			}

			if (nItems == 0)
				_yinc = 0;
			else
				_yinc = (ybounds.UpperBound - ybounds.LowerBound);

			int idx = 0;
			foreach (IGPlotItem pi in coll)
			{
				if (pi is IYBoundsHolder)
				{
					IYBoundsHolder ybpi = (IYBoundsHolder)pi;
					ybounds.Reset();
					ybpi.MergeYBoundsInto(ybounds);
					ybounds.Shift(_yinc * _scaleYInc * idx);
					pb.Add(ybounds);
				}
				if (pi is G2DPlotItem)
					idx++;
			}
		}

		// members that are created at PaintParent and must be released at latest at FinishPainting
		private System.Drawing.Region[] _clippingColl;

		private Processed2DPlotData[] _plotDataColl;
		private double[] _xincColl;
		private double[] _yincColl;

		public void PaintBegin(System.Drawing.Graphics g, IPlotArea layer, PlotItemCollection coll)
		{
			_clippingColl = new System.Drawing.Region[coll.Count];
			_plotDataColl = new Processed2DPlotData[coll.Count];
			_xincColl = new double[coll.Count];
			_yincColl = new double[coll.Count];

			// First prepare
			int idx = -1;
			Processed2DPlotData previousPlotData = null;
			for (int i = 0; i < coll.Count; i++)
			{
				if (coll[i] is G2DPlotItem)
				{
					idx++;
					double currxinc = _xincColl[i] = idx * _xinc * _scaleXInc;
					double curryinc = _yincColl[i] = idx * _yinc * _scaleYInc;

					G2DPlotItem gpi = coll[i] as G2DPlotItem;
					Processed2DPlotData plotdata = _plotDataColl[i] = gpi.GetRangesAndPoints(layer);
					plotdata.PreviousItemData = previousPlotData;
					previousPlotData = plotdata;

					int j = -1;
					foreach (int rowIndex in plotdata.RangeList.OriginalRowIndices())
					{
						j++;

						AltaxoVariant xx = plotdata.GetXPhysical(rowIndex) + currxinc;
						AltaxoVariant yy = plotdata.GetYPhysical(rowIndex) + curryinc;

						Logical3D rel = new Logical3D(layer.XAxis.PhysicalVariantToNormal(xx), layer.YAxis.PhysicalVariantToNormal(yy));
						double xabs, yabs;
						layer.CoordinateSystem.LogicalToLayerCoordinates(rel, out xabs, out yabs);
						plotdata.PlotPointsInAbsoluteLayerCoordinates[j] = new System.Drawing.PointF((float)xabs, (float)yabs);
					}

					// if clipping is used, we must get a clipping region for every plot item
					// and combine the regions from
					if (_useClipping)
					{
						if (i == 0)
							_clippingColl[i] = g.Clip;

						Plot.Styles.LinePlotStyle linestyle = null;
						foreach (Plot.Styles.IG2DPlotStyle st in gpi.Style)
						{
							if (st is Plot.Styles.LinePlotStyle)
							{
								linestyle = st as Plot.Styles.LinePlotStyle;
								break;
							}
						}

						if (null != linestyle)
						{
							GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
							linestyle.GetFillPath(path, layer, plotdata, CSPlaneID.Bottom);
							if ((i + 1) < _clippingColl.Length)
							{
								_clippingColl[i + 1] = (Region)_clippingColl[i].Clone();
								_clippingColl[i + 1].Exclude(path);
							}
						}
						else
						{
							if ((i + 1) < _clippingColl.Length)
								_clippingColl[i + 1] = _clippingColl[i];
						}
					}
				}
			}
		}

		public void PaintEnd()
		{
			_clippingColl = null;
			_plotDataColl = null;
			_xincColl = null;
			_yincColl = null;
		}

		public void PaintChild(System.Drawing.Graphics g, IPlotArea layer, PlotItemCollection coll, int i)
		{
			if (_useClipping)
			{
				//g.SetClip(clippingColl[i], CombineMode.Replace);
				g.Clip = _clippingColl[i];
			}

			if (null == _plotDataColl[i])
			{
				coll[i].Paint(g, layer, i == coll.Count - 1 ? null : coll[i - 1], i == 0 ? null : coll[i - 1]);
			}
			else
			{
				TransformedLayerWrapper layerwrapper = new TransformedLayerWrapper(layer, _xincColl[i], _yincColl[i]);
				((G2DPlotItem)coll[i]).Paint(g, layerwrapper, _plotDataColl[i], i == coll.Count - 1 ? null : _plotDataColl[i + 1], i == 0 ? null : _plotDataColl[i - 1]);
			}

			// The clipping region is no longer needed, so we can dispose it
			if (_useClipping)
			{
				if (i == 0)
					g.Clip = _clippingColl[0]; // restore the original clipping region
				else
					_clippingColl[i].Dispose(); // for i!=0 dispose the clipping region
			}
		}

		/*
		public void Paint(System.Drawing.Graphics g, IPlotArea layer, PlotItemCollection coll)
		{
			System.Drawing.Region[] clippingColl = new System.Drawing.Region[coll.Count];
			Processed2DPlotData[] plotDataColl = new Processed2DPlotData[coll.Count];
			double[] xincColl = new double[coll.Count];
			double[] yincColl = new double[coll.Count];

			// First prepare
			int idx = -1;
			Processed2DPlotData previousPlotData = null;
			for (int i = 0; i < coll.Count; i++)
			{
				if (coll[i] is G2DPlotItem)
				{
					idx++;
					double currxinc = xincColl[i] = idx * _xinc * _scaleXInc;
					double curryinc = yincColl[i] = idx * _yinc * _scaleYInc;

					G2DPlotItem gpi = coll[i] as G2DPlotItem;
					Processed2DPlotData plotdata = plotDataColl[i] = gpi.GetRangesAndPoints(layer);
					plotdata.PreviousItemData = previousPlotData;
					previousPlotData = plotdata;

					int j = -1;
					foreach (int rowIndex in plotdata.RangeList.OriginalRowIndices())
					{
						j++;

						AltaxoVariant xx = plotdata.GetXPhysical(rowIndex) + currxinc;
						AltaxoVariant yy = plotdata.GetYPhysical(rowIndex) + curryinc;

						Logical3D rel = new Logical3D(layer.XAxis.PhysicalVariantToNormal(xx), layer.YAxis.PhysicalVariantToNormal(yy));
						double xabs, yabs;
						layer.CoordinateSystem.LogicalToLayerCoordinates(rel, out xabs, out yabs);
						plotdata.PlotPointsInAbsoluteLayerCoordinates[j] = new System.Drawing.PointF((float)xabs, (float)yabs);
					}

					// if clipping is used, we must get a clipping region for every plot item
					// and combine the regions from
					if (_useClipping)
					{
						if (i == 0)
							clippingColl[i] = g.Clip;

						Plot.Styles.LinePlotStyle linestyle = null;
						foreach (Plot.Styles.IG2DPlotStyle st in gpi.Style)
						{
							if (st is Plot.Styles.LinePlotStyle)
							{
								linestyle = st as Plot.Styles.LinePlotStyle;
								break;
							}
						}

						if (null != linestyle)
						{
							GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
							linestyle.GetFillPath(path, layer, plotdata, CSPlaneID.Bottom);
							if ((i + 1) < clippingColl.Length)
							{
								clippingColl[i + 1] = (Region)clippingColl[i].Clone();
								clippingColl[i + 1].Exclude(path);
							}
						}
						else
						{
							if ((i + 1) < clippingColl.Length)
								clippingColl[i + 1] = clippingColl[i];
						}
					}
				}
			}

			// now paint
			for (int i = coll.Count - 1; i >= 0; i--)
			{
				if (_useClipping)
				{
					//g.SetClip(clippingColl[i], CombineMode.Replace);
					g.Clip = clippingColl[i];
				}

				if (null == plotDataColl[i])
				{
					coll[i].Paint(g, layer, i == coll.Count - 1 ? null : coll[i - 1], i == 0 ? null : coll[i - 1]);
				}
				else
				{
					TransformedLayerWrapper layerwrapper = new TransformedLayerWrapper(layer, xincColl[i], yincColl[i]);
					((G2DPlotItem)coll[i]).Paint(g, layerwrapper, plotDataColl[i], i == coll.Count - 1 ? null : plotDataColl[i + 1], i == 0 ? null : plotDataColl[i - 1]);
				}

				// The clipping region is no longer needed, so we can dispose it
				if (_useClipping)
				{
					if (i == 0)
						g.Clip = clippingColl[0]; // restore the original clipping region
					else
						clippingColl[i].Dispose(); // for i!=0 dispose the clipping region
				}
			}
		}
		*/

		#endregion ICoordinateTransformingGroupStyle Members

		#region ICloneable Members

		public object Clone()
		{
			return new WaterfallTransform(this);
		}

		#endregion ICloneable Members

		#region Inner Classes - TransformedLayerWrapper

		private class TransformedLayerWrapper : IPlotArea
		{
			private IPlotArea _layer;
			private ScaleCollection _scales;

			/// <summary>
			/// Only a shortcut to _scales[0].Scale
			/// </summary>
			private TransformedScale _xScale;

			/// <summary>
			/// Only a shortcut to _scales[1].Scale
			/// </summary>
			private TransformedScale _yScale;

			public TransformedLayerWrapper(IPlotArea layer, double xinc, double yinc)
			{
				_layer = layer;
				_xScale = new TransformedScale(layer.XAxis, xinc);
				_yScale = new TransformedScale(layer.YAxis, yinc);
				_scales = new ScaleCollection();
				_scales[0] = _xScale;
				_scales[1] = _yScale;
			}

			#region IPlotArea Members

			public bool Is3D
			{
				get { return _layer.Is3D; }
			}

			public Scale XAxis
			{
				get { return _xScale; }
			}

			public Scale YAxis
			{
				get { return _yScale; }
			}

			public ScaleCollection Scales
			{
				get
				{
					return _scales;
				}
			}

			public G2DCoordinateSystem CoordinateSystem
			{
				get { return _layer.CoordinateSystem; }
			}

			public PointD2D Size
			{
				get { return _layer.Size; }
			}

			public Logical3D GetLogical3D(I3DPhysicalVariantAccessor acc, int idx)
			{
				Logical3D shifted = new Logical3D(
					_xScale.PhysicalVariantToNormal(acc.GetXPhysical(idx)),
					_yScale.PhysicalVariantToNormal(acc.GetYPhysical(idx)));
				return shifted;
			}

			public IEnumerable<CSLineID> AxisStyleIDs
			{
				get { return _layer.AxisStyleIDs; }
			}

			public void UpdateCSPlaneID(CSPlaneID id)
			{
				if (id.UsePhysicalValue)
				{
					int scaleidx = id.PerpendicularAxisNumber;
					switch (scaleidx)
					{
						case 0:
							id.LogicalValue = _xScale.PhysicalVariantToNormal(id.PhysicalValue);
							break;

						case 1:
							id.LogicalValue = _yScale.PhysicalVariantToNormal(id.PhysicalValue);
							break;

						default:
							_layer.UpdateCSPlaneID(id);
							break;
					}
				}
			}

			#endregion IPlotArea Members

			#region Helper Scale Wrapper

			private class TransformedScale : Scale
			{
				private Scale _originalScale;
				private double _offset;

				public TransformedScale(Scale scale, double offset)
				{
					_originalScale = scale;
					_offset = offset;
				}

				public override double PhysicalVariantToNormal(AltaxoVariant x)
				{
					return _originalScale.PhysicalVariantToNormal(x + _offset);
				}

				public override AltaxoVariant NormalToPhysicalVariant(double x)
				{
					AltaxoVariant result = _originalScale.NormalToPhysicalVariant(x);
					return result - _offset;
				}

				public override object Clone()
				{
					throw new NotImplementedException();
				}

				protected override IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
				{
					yield break; // we do not own the _originalScale (is this OK?)
				}

				public override object RescalingObject
				{
					get { throw new NotImplementedException(); }
				}

				public override IPhysicalBoundaries DataBoundsObject
				{
					get { throw new NotImplementedException(); }
				}

				public override AltaxoVariant OrgAsVariant
				{
					get { throw new NotImplementedException(); }
				}

				public override AltaxoVariant EndAsVariant
				{
					get { throw new NotImplementedException(); }
				}

				public override string SetScaleOrgEnd(AltaxoVariant org, AltaxoVariant end)
				{
					throw new NotImplementedException();
				}

				public override void OnUserRescaled()
				{
					throw new NotImplementedException();
				}

				public override bool IsOrgExtendable
				{
					get { throw new NotImplementedException(); }
				}

				public override bool IsEndExtendable
				{
					get { throw new NotImplementedException(); }
				}

				public override void OnUserZoomed(AltaxoVariant newZoomOrg, AltaxoVariant newZoomEnd)
				{
					throw new NotImplementedException();
				}

				public override Scales.Ticks.TickSpacing TickSpacing
				{
					get
					{
						return null;
					}
					set
					{
						throw new NotImplementedException();
					}
				}
			}

			#endregion Helper Scale Wrapper
		}

		#endregion Inner Classes - TransformedLayerWrapper
	}
}