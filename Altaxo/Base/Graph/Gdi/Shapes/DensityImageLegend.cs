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
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Altaxo.Graph.Gdi.Shapes
{
	using Altaxo.Graph.Scales;
	using Altaxo.Graph.Scales.Ticks;
	using Altaxo.Graph.Gdi.Plot;
	using Altaxo.Graph;
	using Altaxo.Graph.Gdi.Axis;

	[Serializable]
	public class DensityImageLegend : GraphicBase, Main.IChildChangedEventSink
	{
		const int _bitmapPixelsAcross = 2;
		const int _bitmapPixelsAlong = 1024;



		/// <summary>
		/// Axis styles for both sides of the density plot item.
		/// </summary>
		protected AxisStyleCollection _axisStyles;

		// Cached members
		Bitmap _bitmap;

		/// <summary>The proxy for the plot item this legend is intended for.</summary>
		Altaxo.Main.RelDocNodeProxy _plotItemProxy;

		DensityLegendArea _cachedArea;


		#region Serialization

		#region Clipboard serialization

		protected DensityImageLegend(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			SetObjectData(this, info, context, null);
		}

		/// <summary>
		/// Serializes EllipseGraphic Version 0.
		/// </summary>
		/// <param name="info">The serialization info.</param>
		/// <param name="context">The streaming context.</param>
		public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			DensityImageLegend s = this;
			base.GetObjectData(info, context);
			info.AddValue("IsOrientationVertical", IsOrientationVertical);
			info.AddValue("IsScaleReversed", IsScaleReversed);
			info.AddValue("Scale", ScaleWithTicks.Scale);
			info.AddValue("TickSpacing", ScaleWithTicks.TickSpacing);
			info.AddValue("AxisStyles", _axisStyles);
		}


		/// <summary>
		/// Deserializes the EllipseGraphic Version 0.
		/// </summary>
		/// <param name="obj">The empty EllipseGraphic object to deserialize into.</param>
		/// <param name="info">The serialization info.</param>
		/// <param name="context">The streaming context.</param>
		/// <param name="selector">The deserialization surrogate selector.</param>
		/// <returns>The deserialized EllipseGraphic.</returns>
		public override object SetObjectData(object obj, System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context, System.Runtime.Serialization.ISurrogateSelector selector)
		{
			DensityImageLegend s = (DensityImageLegend)base.SetObjectData(obj, info, context, selector);

			bool isOrientationVertical = info.GetBoolean("IsOrientationVertical");
			bool isScaleReversed = info.GetBoolean("IsScaleReversed");
			var cachedScale = (NumericalScale)info.GetValue("Scale", typeof(NumericalScale));
			var scaleTickSpacing = (TickSpacing)info.GetValue("TickSpacing", typeof(TickSpacing));
			_axisStyles = (AxisStyleCollection)info.GetValue("AxisStyles", typeof(AxisStyleCollection));
			_axisStyles.ParentObject = this;
			_cachedArea = new DensityLegendArea(Size, isOrientationVertical, isScaleReversed, cachedScale, scaleTickSpacing);
			_axisStyles.UpdateCoordinateSystem(_cachedArea.CoordinateSystem);

			// now we should use the first plotitem that is a densityPlotItem that we can find


			return s;
		}
		/// <summary>
		/// Finale measures after deserialization.
		/// </summary>
		/// <param name="obj">Not used.</param>
		public override void OnDeserialization(object obj)
		{
			base.OnDeserialization(obj);
		}
		#endregion

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(DensityImageLegend), 0)]
		class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				DensityImageLegend s = (DensityImageLegend)obj;
				info.AddBaseValueEmbedded(s, typeof(DensityImageLegend).BaseType);

				info.AddValue("PlotItem", s._plotItemProxy);
				info.AddValue("IsOrientationVertical", s.IsOrientationVertical);
				info.AddValue("IsScaleReversed", s.IsScaleReversed);
				info.AddValue("Scale", s.ScaleWithTicks.Scale);
				info.AddValue("TickSpacing", s.ScaleWithTicks.TickSpacing);
				info.AddValue("AxisStyles", s._axisStyles);
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{

				DensityImageLegend s = null != o ? (DensityImageLegend)o : new DensityImageLegend();
				info.GetBaseValueEmbedded(s, typeof(DensityImageLegend).BaseType, parent);

				s._plotItemProxy = (Main.RelDocNodeProxy)info.GetValue("PlotItem", s);
				s.WirePlotItemProxyEvents();
				bool isOrientationVertical = info.GetBoolean("IsOrientationVertical");
				bool isScaleReversed = info.GetBoolean("IsScaleReversed");
				var cachedScale = (NumericalScale)info.GetValue("Scale", parent);
				var scaleTickSpacing = (TickSpacing)info.GetValue("TickSpacing", parent);
				s._axisStyles = (AxisStyleCollection)info.GetValue("AxisStyles", parent);
				s._axisStyles.ParentObject = s;
				s._cachedArea = new DensityLegendArea(s.Size, isOrientationVertical, isScaleReversed, cachedScale, scaleTickSpacing);
				s._axisStyles.UpdateCoordinateSystem(s._cachedArea.CoordinateSystem);

				return s;
			}
		}

		private DensityImageLegend()
		{
		}

		private void WirePlotItemProxyEvents()
		{
			_plotItemProxy.DocumentInstanceChanged += EhPlotItemProxyDocumentInstanceChanged;
		}

		#endregion



		public DensityImageLegend(DensityImagePlotItem plotItem, PointD2D initialLocation, PointD2D graphicSize)
		{
			this._position = initialLocation;
			this.SetSize(graphicSize.X, graphicSize.Y,true);

			_plotItemProxy = new Main.RelDocNodeProxy();
			WirePlotItemProxyEvents();
			PlotItem = plotItem;


			// _orientationIsVertical = true;
			// _scaleIsReversed = false;

			var cachedScale = (NumericalScale)PlotItem.Style.Scale.Clone();
			var scaleTickSpacing = ScaleWithTicks.CreateDefaultTicks(cachedScale.GetType());
			_cachedArea = new DensityLegendArea(Size, true, false, cachedScale, scaleTickSpacing);

			_axisStyles = new AxisStyleCollection();
			_axisStyles.UpdateCoordinateSystem(_cachedArea.CoordinateSystem);

			var sx0 = new AxisStyle(CSLineID.X0) { ShowAxisLine = true, ShowMajorLabels = true, TitleText = "Z values" };
			sx0.AxisLineStyle.FirstDownMajorTicks = true;
			sx0.AxisLineStyle.FirstUpMajorTicks = false;
			sx0.AxisLineStyle.FirstDownMinorTicks = true;
			sx0.AxisLineStyle.FirstUpMinorTicks = false;

			var sx1 = new AxisStyle(CSLineID.X1) { ShowAxisLine = true };
			sx1.AxisLineStyle.FirstDownMajorTicks = false;
			sx1.AxisLineStyle.FirstUpMajorTicks = false;
			sx1.AxisLineStyle.FirstDownMinorTicks = false;
			sx1.AxisLineStyle.FirstUpMinorTicks = false;

			var sy0 = new AxisStyle(CSLineID.Y0) { ShowAxisLine = true, TitleText = "Color map" };
			var sy1 = new AxisStyle(CSLineID.Y1) { ShowAxisLine = true };
			_axisStyles.Add(sx0);
			_axisStyles.Add(sx1);
			_axisStyles.Add(sy0);
			_axisStyles.Add(sy1);


			sx0.Title.Rotation = 90;
			sx0.Title.XAnchor = XAnchorPositionType.Center;
			sx0.Title.YAnchor = YAnchorPositionType.Bottom;
			sx0.Title.X = -Width / 3;
			sx0.Title.Y = Height / 2;



			sy0.Title.XAnchor = XAnchorPositionType.Center;
			sy0.Title.YAnchor = YAnchorPositionType.Bottom;
			sy0.Title.X = Width / 2;
			sy0.Title.Y = sy0.Title.Height / 2;

			// set the parent objects
			_axisStyles.ParentObject = this;
			this.UpdateTransformationMatrix();
		}

		public DensityImageLegend(DensityImageLegend from)
			: base(from)  // all is done here, since CopyFrom is virtual!
		{
		}

		public override bool CopyFrom(object obj)
		{
			var isCopied = base.CopyFrom(obj);
			if (isCopied && !object.ReferenceEquals(this, obj))
			{
				var from = obj as DensityImageLegend;
				if (null != from)
				{
					_cachedArea = new DensityLegendArea(from._cachedArea);

					this._axisStyles = (AxisStyleCollection)from._axisStyles.Clone();
					this._axisStyles.UpdateCoordinateSystem(_cachedArea.CoordinateSystem);
					this._axisStyles.ParentObject = this;

					this._bitmap = null != from._bitmap ? (Bitmap)from._bitmap.Clone() : null;
					
					if (null == _plotItemProxy)
					{
						_plotItemProxy = new Main.RelDocNodeProxy();
						WirePlotItemProxyEvents();
					}
					this._plotItemProxy.CopyPathOnlyFrom(from._plotItemProxy, this);
				}
			}
				return isCopied;
		}

		public override object Clone()
		{
			return new DensityImageLegend(this);
		}

		private DensityImagePlotItem PlotItem
		{
			set
			{
				_plotItemProxy.SetDocNode(value, this);
			}
			get
			{
				return _plotItemProxy.DocumentObject as DensityImagePlotItem;
			}
		}

		private void EhPlotItemProxyDocumentInstanceChanged(object sender, object oldValue, object newValue)
		{
			var oldPlotItem = oldValue as DensityImagePlotItem;
			var newPlotItem = newValue as DensityImagePlotItem;

			if (null != oldPlotItem)
			{
				oldPlotItem.StyleChanged -= new EventHandler(EhPlotItemStyleChanged);
			}

			if (null != newPlotItem)
			{
				newPlotItem.StyleChanged += new EventHandler(EhPlotItemStyleChanged);
			}
		}


		public AxisStyleCollection AxisStyles
		{
			get
			{
				return _axisStyles;
			}
		}

		public G2DCoordinateSystem CoordinateSystem
		{
			get
			{
				return _cachedArea.CoordinateSystem;
			}
		}

		public ScaleWithTicks ScaleWithTicks
		{
			get
			{
				return _cachedArea.Scales[0];
			}
		}

		void EhPlotItemStyleChanged(object sender, EventArgs e)
		{
			OnChanged();
		}

		public bool IsOrientationVertical
		{
			get
			{
				return ((CS.G2DCartesicCoordinateSystem)_cachedArea.CoordinateSystem).IsXYInterchanged;
			}
		}

		public bool IsScaleReversed
		{
			get
			{
				return ((CS.G2DCartesicCoordinateSystem)_cachedArea.CoordinateSystem).IsXReverse;
			}
		}


		/// <summary>
		/// Get the object outline for arrangements in object world coordinates.
		/// </summary>
		/// <returns>Object outline for arrangements in object world coordinates</returns>
		public override GraphicsPath GetObjectOutlineForArrangements()
		{
			return GetRectangularObjectOutline();
		}

		public override void Paint(System.Drawing.Graphics g, object obj)
		{
			bool orientationIsVertical = IsOrientationVertical;
			bool scaleIsReversed = IsScaleReversed;

			int pixelH = orientationIsVertical ? _bitmapPixelsAcross : _bitmapPixelsAlong;
			int pixelV = orientationIsVertical ? _bitmapPixelsAlong : _bitmapPixelsAcross;

			if (null == _bitmap || _bitmap.Width != pixelH || _bitmap.Height != pixelV)
			{
				if (null != _bitmap)
					_bitmap.Dispose();

				_bitmap = new Bitmap(pixelH, pixelV, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
			}

			// Update the coordinate system size to meet the size of the graph item
			_cachedArea.CoordinateSystem.UpdateAreaSize(Size);

			Data.AltaxoVariant porg;
			Data.AltaxoVariant pend;
			NumericalScale originalZScale;
			Plot.IColorProvider colorProvider;

			if (null == PlotItem)
			{
				// search for the first density plot item in the layer
				var layer = Main.DocumentPath.GetRootNodeImplementing<XYPlotLayer>(this);
				if (null != layer)
				{
					foreach (var item in layer.PlotItems)
					{
						if (item is DensityImagePlotItem)
						{
							PlotItem = item as DensityImagePlotItem;
							break;
						}
					}
				}
			}

			if (null != PlotItem)
			{
				porg = PlotItem.Style.Scale.OrgAsVariant;
				pend = PlotItem.Style.Scale.EndAsVariant;
				originalZScale = PlotItem.Style.Scale;
				colorProvider = PlotItem.Style.ColorProvider;
			}
			else
			{
				porg = 0;
				pend = 1;
				originalZScale = new LinearScale();
				colorProvider = new Plot.ColorProvider.ColorProviderBGRY();
			}

			var legendScale = (NumericalScale)ScaleWithTicks.Scale;
			var legendTickSpacing = ScaleWithTicks.TickSpacing;

			// We set the boundaries of our legend scale to the org and end of the z-scale of the density image item. 
			legendScale.DataBounds.BeginUpdate();
			legendScale.DataBounds.Reset();
			legendScale.DataBounds.Add(originalZScale.OrgAsVariant);
			legendScale.DataBounds.Add(originalZScale.EndAsVariant);
			legendScale.DataBounds.EndUpdate();
			legendScale.Rescale(); // and do a rescale to apply the changes to the boundaries


			// Fill the bitmap

			for (int i = 0; i < _bitmapPixelsAlong; i++)
			{
				double r = (scaleIsReversed ^ orientationIsVertical) ? 1 - i / (double)(_bitmapPixelsAlong - 1) : i / (double)(_bitmapPixelsAlong - 1);
				double l = originalZScale.PhysicalToNormal(legendScale.NormalToPhysical(r));
				var color = colorProvider.GetColor(l);
				if (orientationIsVertical)
				{
					for (int j = 0; j < _bitmapPixelsAcross; j++)
						_bitmap.SetPixel(j, i, color);
				}
				else
				{
					for (int j = 0; j < _bitmapPixelsAcross; j++)
						_bitmap.SetPixel(i, j, color);
				}
			}


			var graphicsState = g.Save();
			TransformGraphics(g);


			g.DrawImage(_bitmap,
				new RectangleF(0, 0, (float)Size.X, (float)Size.Y),
				new RectangleF(0, 0, pixelH - 1, pixelV - 1), GraphicsUnit.Pixel);

			_axisStyles.Paint(g, _cachedArea);

			g.Restore(graphicsState);

		}

		bool EhAxisTitleRemove(IHitTestObject o)
		{
			foreach (var axstyle in _axisStyles)
			{
				if (object.ReferenceEquals(o.HittedObject, axstyle.Title))
				{
					axstyle.Title = null;
					return true;
				}
			}
			return false;
		}

		public override IHitTestObject HitTest(HitTestPointData htd)
		{

			var myHitTestData = htd.NewFromAdditionalTransformation(_transformation);

			IHitTestObject result = null;
			foreach (var axstyle in _axisStyles)
			{
				if (null != axstyle.Title && null != (result = axstyle.Title.HitTest(myHitTestData)))
				{
					result.Remove = this.EhAxisTitleRemove;
					result.Transform(_transformation);
					return result;
				}
			}

			result = base.HitTest(htd);
			if (result != null)
				result.DoubleClick = EhDoubleClick;

			return result;
		}

		private bool EhDoubleClick(IHitTestObject o)
		{
			Current.Gui.ShowDialog(new object[] { o.HittedObject }, "Edit density image legend", true);
			return false;
		}


		#region IChildChangedEventSink Members

		public new void EhChildChanged(object child, EventArgs e)
		{
			OnChanged();
		}

		#endregion



		#region Inner classes

		[Serializable]
		class DensityLegendArea : IPlotArea
		{
			PointD2D _size;
			ScaleCollection _scales;
			CS.G2DCartesicCoordinateSystem _coordinateSystem;

			public DensityLegendArea(PointD2D size, bool isXYInterchanged, bool isXReversed, Scale scale, TickSpacing tickSpacing)
			{
				_size = size;
				_scales = new ScaleCollection();
				_scales[0] = new ScaleWithTicks(scale, tickSpacing);
				_scales[1] = new ScaleWithTicks(new LinearScale(), new NoTickSpacing());
				_coordinateSystem = new Altaxo.Graph.Gdi.CS.G2DCartesicCoordinateSystem();
				_coordinateSystem.IsXYInterchanged = isXYInterchanged;
				_coordinateSystem.IsXReverse = isXReversed;
				_coordinateSystem.UpdateAreaSize(_size);
			}

			public DensityLegendArea(DensityLegendArea from)
			{
				this._size = from._size;
				this._scales = from._scales.Clone();
				this._coordinateSystem = (CS.G2DCartesicCoordinateSystem)from._coordinateSystem.Clone();
			}

			#region IPlotArea Members

			public bool Is3D
			{
				get { return false; }
			}

			public Scale XAxis
			{
				get { return _scales[0].Scale; }
			}

			public Scale YAxis
			{
				get { return _scales[1].Scale; }
			}

			public ScaleCollection Scales
			{
				get { return _scales; }
			}

			public G2DCoordinateSystem CoordinateSystem
			{
				get { return _coordinateSystem; }
			}

			public PointD2D Size
			{
				get { return _size; }
				set { _size = value; }
			}


			public Logical3D GetLogical3D(I3DPhysicalVariantAccessor acc, int idx)
			{
				Logical3D r;
				r.RX = XAxis.PhysicalVariantToNormal(acc.GetXPhysical(idx));
				r.RY = YAxis.PhysicalVariantToNormal(acc.GetYPhysical(idx));
				r.RZ = 0;
				return r;
			}


			public IEnumerable<CSLineID> AxisStyleIDs
			{
				get
				{
					throw new NotImplementedException();
				}
			}

			public void UpdateCSPlaneID(CSPlaneID id)
			{
				throw new NotImplementedException();
			}

			#endregion
		}


		#endregion

	}
}
