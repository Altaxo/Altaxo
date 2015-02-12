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

using Altaxo.Graph.Gdi.Background;
using Altaxo.Graph.Scales;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Altaxo.Graph.Gdi
{
	using Axis;
	using Plot;
	using Shapes;

	public partial class XYPlotLayer
		:
		HostLayer,
		IPlotArea
	{
		#region Serialization

		private void SetupOldAxis(int idx, Altaxo.Graph.Scales.Deprecated.Scale axis, bool isLinked, double orgA, double orgB, double endA, double endB)
		{
			Scale transScale = null;
			if (axis is Altaxo.Graph.Scales.Deprecated.TextScale)
				transScale = new TextScale();
			else if (axis is Altaxo.Graph.Scales.Deprecated.DateTimeScale)
				transScale = new DateTimeScale();
			else if (axis is Altaxo.Graph.Scales.Deprecated.Log10Scale)
				transScale = new Log10Scale();
			else if (axis is Altaxo.Graph.Scales.Deprecated.AngularScale)
				transScale = (axis as Altaxo.Graph.Scales.Deprecated.AngularScale).UseDegrees ? (Scale)new AngularDegreeScale() : (Scale)new AngularRadianScale();
			else if (axis is Altaxo.Graph.Scales.Deprecated.LinearScale)
				transScale = new LinearScale();
			else
				throw new ArgumentException("Axis type unknown");

			var ticks = Scale.CreateDefaultTicks(transScale.GetType());

			transScale.SetScaleOrgEnd(axis.OrgAsVariant, axis.EndAsVariant);
			if (transScale.RescalingObject is Altaxo.Graph.Scales.Rescaling.NumericAxisRescaleConditions &&
				axis.RescalingObject is Altaxo.Graph.Scales.Rescaling.NumericAxisRescaleConditions)
			{
				((Altaxo.Graph.Scales.Rescaling.NumericAxisRescaleConditions)transScale.RescalingObject).CopyFrom((Altaxo.Graph.Scales.Rescaling.NumericAxisRescaleConditions)axis.RescalingObject);
			}

			if (isLinked)
			{
				LinkedScale ls = new LinkedScale(transScale, idx);
				ls.SetLinkParameter(orgA, orgB, endA, endB);
				transScale = ls;
			}

			_scales.SetScaleWithTicks(idx, transScale, ticks);
		}

		private void SetupOldAxes(Altaxo.Graph.Scales.Deprecated.LinkedScaleCollection linkedScales)
		{
			SetupOldAxis(0, linkedScales.X.Scale, linkedScales.X.AxisLinkType != ScaleLinkType.None, linkedScales.X.LinkOrgA, linkedScales.X.LinkOrgB, linkedScales.X.LinkEndA, linkedScales.X.LinkEndB);
			SetupOldAxis(1, linkedScales.Y.Scale, linkedScales.Y.AxisLinkType != ScaleLinkType.None, linkedScales.Y.LinkOrgA, linkedScales.Y.LinkOrgB, linkedScales.Y.LinkEndA, linkedScales.Y.LinkEndB);
		}

		#region Version 0 and 1

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.XYPlotLayer", 0)]
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.XYPlotLayer", 1)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				throw new ApplicationException("Calling of an outdated serialization routine");
				/*
				XYPlotLayer s = (XYPlotLayer)obj;
				// XYPlotLayer style
				info.AddValue("FillLayerArea",s._fillLayerArea);
				info.AddValue("LayerAreaFillBrush",s.m_LayerAreaFillBrush);

				// size, position, rotation and scale

				info.AddValue("WidthType",s._location.WidthType);
				info.AddValue("HeightType",s._location.HeightType);
				info.AddValue("Width",s._location.Width);
				info.AddValue("Height",s._location.Height);
				info.AddValue("CachedSize",s._cachedLayerSize);

				info.AddValue("XPositionType",s._location.XPositionType);
				info.AddValue("YPositionType",s._location.YPositionType);
				info.AddValue("XPosition",s._location.XPosition);
				info.AddValue("YPosition",s._location.YPosition);
				info.AddValue("CachedPosition",s._cachedLayerPosition);

				info.AddValue("Rotation",s._location.Angle);
				info.AddValue("Scale",s._location.Scale);

				// axis related

				info.AddValue("XAxis",s._axisProperties.X.Axis);
				info.AddValue("YAxis",s._axisProperties.Y.Axis);
				info.AddValue("LinkXAxis",s._axisProperties.X.IsLinked);
				info.AddValue("LinkYAxis", s._axisProperties.Y.IsLinked);
				info.AddValue("LinkXAxisOrgA", s._axisProperties.X.LinkAxisOrgA);
				info.AddValue("LinkXAxisOrgB", s._axisProperties.X.LinkAxisOrgB);
				info.AddValue("LinkXAxisEndA", s._axisProperties.X.LinkAxisEndA);
				info.AddValue("LinkXAxisEndB", s._axisProperties.X.LinkAxisEndB);
				info.AddValue("LinkYAxisOrgA", s._axisProperties.Y.LinkAxisOrgA);
				info.AddValue("LinkYAxisOrgB", s._axisProperties.Y.LinkAxisOrgB);
				info.AddValue("LinkYAxisEndA", s._axisProperties.Y.LinkAxisEndA);
				info.AddValue("LinkYAxisEndB", s._axisProperties.Y.LinkAxisEndB);

				// Styles
				info.AddValue("ShowLeftAxis", s._axisStyles[EdgeType.Left].ShowAxis);
				info.AddValue("ShowBottomAxis", s._axisStyles[EdgeType.Bottom].ShowAxis);
				info.AddValue("ShowRightAxis", s._axisStyles[EdgeType.Right].ShowAxis);
				info.AddValue("ShowTopAxis", s._axisStyles[EdgeType.Top].ShowAxis);

				info.AddValue("LeftAxisStyle", s._axisStyles[EdgeType.Left].AxisStyle);
				info.AddValue("BottomAxisStyle", s._axisStyles[EdgeType.Bottom].AxisStyle);
				info.AddValue("RightAxisStyle", s._axisStyles[EdgeType.Right].AxisStyle);
				info.AddValue("TopAxisStyle", s._axisStyles[EdgeType.Top].AxisStyle);

				info.AddValue("LeftLabelStyle",s._axisStyles[EdgeType.Left].MajorLabelStyle);
				info.AddValue("BottomLabelStyle", s._axisStyles[EdgeType.Bottom].MajorLabelStyle);
				info.AddValue("RightLabelStyle", s._axisStyles[EdgeType.Right].MajorLabelStyle);
				info.AddValue("TopLabelStyle", s._axisStyles[EdgeType.Top].MajorLabelStyle);

				// Titles and legend
				info.AddValue("LeftAxisTitle", s._axisStyles[EdgeType.Left].Title);
				info.AddValue("BottomAxisTitle", s._axisStyles[EdgeType.Bottom].Title);
				info.AddValue("RightAxisTitle", s._axisStyles[EdgeType.Right].Title);
				info.AddValue("TopAxisTitle", s._axisStyles[EdgeType.Top].Title);
				info.AddValue("Legend",s._legend);

				// XYPlotLayer specific
				info.AddValue("LinkedLayer", null!=s._linkedLayer ? Main.DocumentPath.GetRelativePathFromTo(s,s._linkedLayer) : null);

				info.AddValue("GraphicsObjectCollection",s._graphObjects);
				info.AddValue("Plots",s._plotItems);
				*/
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				XYPlotLayer s = SDeserialize(o, info, parent);

				s.CalculateMatrix();

				return s;
			}

			protected virtual XYPlotLayer SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				XYPlotLayer s = null != o ? (XYPlotLayer)o : new XYPlotLayer(info);

				bool fillLayerArea = info.GetBoolean("FillLayerArea");
				BrushX layerAreaFillBrush = (BrushX)info.GetValue("LayerAreaFillBrush", s);

				if (fillLayerArea)
				{
					if (!s.GridPlanes.Contains(CSPlaneID.Front))
						s.GridPlanes.Add(new GridPlane(CSPlaneID.Front));
					s.GridPlanes[CSPlaneID.Front].Background = layerAreaFillBrush;
				}

				// size, position, rotation and scale

				var widthType = (XYPlotLayerSizeType)info.GetValue("WidthType", s);
				var heightType = (XYPlotLayerSizeType)info.GetValue("HeightType", s);
				var width = info.GetDouble("Width");
				var height = info.GetDouble("Height");
				s._cachedLayerSize = (SizeF)info.GetValue("CachedSize", s);
				s._coordinateSystem.UpdateAreaSize(s._cachedLayerSize);

				var xPositionType = (XYPlotLayerPositionType)info.GetValue("XPositionType", s);
				var yPositionType = (XYPlotLayerPositionType)info.GetValue("YPositionType", s);
				var xPosition = info.GetDouble("XPosition");
				var yPosition = info.GetDouble("YPosition");
				s._cachedLayerPosition = (PointF)info.GetValue("CachedPosition", s);
				var rotation = info.GetSingle("Rotation");
				var scale = info.GetSingle("Scale");
				s.Location = new XYPlotLayerPositionAndSize_V0(widthType, width, heightType, height, xPositionType, xPosition, yPositionType, yPosition, rotation, scale).ConvertToCurrentLocationVersion(s._cachedLayerSize, s._cachedLayerPosition);

				// axis related

				var xAxis = (Altaxo.Graph.Scales.Deprecated.Scale)info.GetValue("XAxis", s);
				var yAxis = (Altaxo.Graph.Scales.Deprecated.Scale)info.GetValue("YAxis", s);
				bool xIsLinked = info.GetBoolean("LinkXAxis");
				bool yIsLinked = info.GetBoolean("LinkYAxis");
				double xOrgA = info.GetDouble("LinkXAxisOrgA");
				double xOrgB = info.GetDouble("LinkXAxisOrgB");
				double xEndA = info.GetDouble("LinkXAxisEndA");
				double xEndB = info.GetDouble("LinkXAxisEndB");
				double yOrgA = info.GetDouble("LinkYAxisOrgA");
				double yOrgB = info.GetDouble("LinkYAxisOrgB");
				double yEndA = info.GetDouble("LinkYAxisEndA");
				double yEndB = info.GetDouble("LinkYAxisEndB");
				s.SetupOldAxis(0, xAxis, xIsLinked, xOrgA, xOrgB, xEndA, xEndB);
				s.SetupOldAxis(1, yAxis, yIsLinked, yOrgA, yOrgB, yEndA, yEndB);

				// Styles
				bool showLeft = info.GetBoolean("ShowLeftAxis");
				bool showBottom = info.GetBoolean("ShowBottomAxis");
				bool showRight = info.GetBoolean("ShowRightAxis");
				bool showTop = info.GetBoolean("ShowTopAxis");

				s._axisStyles.AxisStyleEnsured(CSLineID.Y0).AxisLineStyle = (AxisLineStyle)info.GetValue("LeftAxisStyle", s);
				s._axisStyles.AxisStyleEnsured(CSLineID.X0).AxisLineStyle = (AxisLineStyle)info.GetValue("BottomAxisStyle", s);
				s._axisStyles.AxisStyleEnsured(CSLineID.Y1).AxisLineStyle = (AxisLineStyle)info.GetValue("RightAxisStyle", s);
				s._axisStyles.AxisStyleEnsured(CSLineID.X1).AxisLineStyle = (AxisLineStyle)info.GetValue("TopAxisStyle", s);

				s._axisStyles[CSLineID.Y0].MajorLabelStyle = (AxisLabelStyle)info.GetValue("LeftLabelStyle", s);
				s._axisStyles[CSLineID.X0].MajorLabelStyle = (AxisLabelStyle)info.GetValue("BottomLabelStyle", s);
				s._axisStyles[CSLineID.Y1].MajorLabelStyle = (AxisLabelStyle)info.GetValue("RightLabelStyle", s);
				s._axisStyles[CSLineID.X1].MajorLabelStyle = (AxisLabelStyle)info.GetValue("TopLabelStyle", s);

				// Titles and legend
				s._axisStyles[CSLineID.Y0].Title = (TextGraphic)info.GetValue("LeftAxisTitle", s);
				s._axisStyles[CSLineID.X0].Title = (TextGraphic)info.GetValue("BottomAxisTitle", s);
				s._axisStyles[CSLineID.Y1].Title = (TextGraphic)info.GetValue("RightAxisTitle", s);
				s._axisStyles[CSLineID.X1].Title = (TextGraphic)info.GetValue("TopAxisTitle", s);

				if (!showLeft)
					s._axisStyles.Remove(CSLineID.Y0);
				if (!showRight)
					s._axisStyles.Remove(CSLineID.Y1);
				if (!showBottom)
					s._axisStyles.Remove(CSLineID.X0);
				if (!showTop)
					s._axisStyles.Remove(CSLineID.X1);

				var legend = (TextGraphic)info.GetValue("Legend", s);

				// XYPlotLayer specific
				object linkedLayer = info.GetValue("LinkedLayer", s);
				if (linkedLayer is Main.AbsoluteDocumentPath)
				{
					ProvideLinkedScalesWithLinkedLayerIndex(s, (Main.AbsoluteDocumentPath)linkedLayer, info);
				}
				else if (linkedLayer is Main.RelativeDocumentPath)
				{
					ProvideLinkedScalesWithLinkedLayerIndex(s, (Main.RelativeDocumentPath)linkedLayer, info);
				}
				s.GraphObjects.AddRange((IEnumerable<IGraphicBase>)info.GetValue("GraphObjects", s));

				s._plotItems = (PlotItemCollection)info.GetValue("Plots", s);
				if (null != s._plotItems) s._plotItems.ParentObject = s;

				if (null != legend)
				{
					var legend1 = new LegendText(legend);
					s._graphObjects.Add(legend1);
				}

				return s;
			}
		}

		#endregion Version 0 and 1

		#region Version 2

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.XYPlotLayer", 2)]
		private class XmlSerializationSurrogate2 : XmlSerializationSurrogate0
		{
			public override void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				throw new NotSupportedException("Serialization of old versions not supported, maybe a programming error");
				/*
				base.Serialize(obj, info);

				XYPlotLayer s = (XYPlotLayer)obj;
				// XYPlotLayer style
				info.AddValue("ClipDataToFrame", s._dataClipping);
				*/
			}

			protected override XYPlotLayer SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				XYPlotLayer s = base.SDeserialize(o, info, parent);

				bool clipDataToFrame = info.GetBoolean("ClipDataToFrame");
				s.ClipDataToFrame = clipDataToFrame ? LayerDataClipping.StrictToCS : LayerDataClipping.None;

				return s;
			}
		}

		#endregion Version 2

		#region Version 3

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.XYPlotLayer", 3)]
		private class XmlSerializationSurrogate3 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				throw new NotSupportedException("Serialization of old versions is not supported");
				/*
				XYPlotLayer s = (XYPlotLayer)obj;

				// Background
				info.AddValue("Background",s._layerBackground);

				// size, position, rotation and scale
				info.AddValue("LocationAndSize", s._location);
				info.AddValue("CachedSize", s._cachedLayerSize);
				info.AddValue("CachedPosition", s._cachedLayerPosition);

				// LayerProperties
				info.AddValue("ClipDataToFrame",s._clipDataToFrame);

				// axis related
				info.AddValue("AxisProperties", s._axisProperties);

				// Styles
				info.AddValue("AxisStyles", s._scaleStyles);

				// Legends
				info.CreateArray("Legends",1);
				info.AddValue("e", s._legend);
				info.CommitArray();

				// XYPlotLayer specific
				info.CreateArray("LinkedLayers",1);
				info.AddValue("e", s._linkedLayer);
				info.CommitArray();

				info.AddValue("GraphicGlyphs", s._graphObjects);

				info.AddValue("Plots", s._plotItems);
				*/
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				XYPlotLayer s = SDeserialize(o, info, parent);
				s.CalculateMatrix();
				return s;
			}

			protected virtual XYPlotLayer SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				XYPlotLayer s = (o == null ? new XYPlotLayer(info) : (XYPlotLayer)o);
				int count;

				// Background
				IBackgroundStyle bgs = (IBackgroundStyle)info.GetValue("Background", s);
				if (null != bgs)
				{
					if (!s.GridPlanes.Contains(CSPlaneID.Front))
						s.GridPlanes.Add(new GridPlane(CSPlaneID.Front));
					s.GridPlanes[CSPlaneID.Front].Background = bgs.Brush;
				}

				// size, position, rotation and scale
				var location = (XYPlotLayerPositionAndSize_V0)info.GetValue("LocationAndSize", s);
				s._cachedLayerSize = (SizeF)info.GetValue("CachedSize", s);
				s._cachedLayerPosition = (PointF)info.GetValue("CachedPosition", s);
				s._coordinateSystem.UpdateAreaSize(s._cachedLayerSize);
				s.Location = location.ConvertToCurrentLocationVersion(s._cachedLayerSize, s._cachedLayerPosition);

				// LayerProperties
				bool clipDataToFrame = info.GetBoolean("ClipDataToFrame");
				s._dataClipping = clipDataToFrame ? LayerDataClipping.StrictToCS : LayerDataClipping.None;

				// axis related
				var linkedScales = (Altaxo.Graph.Scales.Deprecated.LinkedScaleCollection)info.GetValue("AxisProperties", s);
				s.SetupOldAxes(linkedScales);

				// Styles
				G2DScaleStyleCollection ssc = (G2DScaleStyleCollection)info.GetValue("AxisStyles", s);
				GridPlane gplane = new GridPlane(CSPlaneID.Front);
				gplane.GridStyle[0] = ssc.ScaleStyle(0).GridStyle;
				gplane.GridStyle[1] = ssc.ScaleStyle(1).GridStyle;
				s.GridPlanes.Add(gplane);
				foreach (AxisStyle ax in ssc.AxisStyles)
					s._axisStyles.Add(ax);

				// Legends
				count = info.OpenArray("Legends");
				var legend = (TextGraphic)info.GetValue("e", s);
				info.CloseArray(count);

				// XYPlotLayer specific
				count = info.OpenArray("LinkedLayers");
				var linkedLayer = (Main.RelDocNodeProxy)info.GetValue("e", s);
				info.CloseArray(count);
				ProvideLinkedScalesWithLinkedLayerIndex(s, linkedLayer, info);

				s.GraphObjects.AddRange((IEnumerable<IGraphicBase>)info.GetValue("GraphicGlyphs", s));
				if (null != legend)
				{
					var legend1 = new LegendText(legend);
					s.GraphObjects.Add(legend1);
				}
				s.PlotItems = (PlotItemCollection)info.GetValue("Plots", s);

				return s;
			}
		}

		#endregion Version 3

		#region Version 4

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(XYPlotLayer), 4)]
		private class XmlSerializationSurrogate4 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				throw new InvalidOperationException("Serialization of old version");
				/*
				XYPlotLayer s = (XYPlotLayer)obj;

				// size, position, rotation and scale
				info.AddValue("LocationAndSize", s._location);
				info.AddValue("CachedSize", s._cachedLayerSize);
				info.AddValue("CachedPosition", s._cachedLayerPosition);

				// CoordinateSystem
				info.AddValue("CoordinateSystem", s._coordinateSystem);

				// Linked layers
				info.CreateArray("LinkedLayers", 1);
				info.AddValue("e", s._linkedLayerProxy);
				info.CommitArray();

				// Scales
				info.AddValue("Scales", s._scales);

				// Grid planes
				info.AddValue("GridPlanes", s._gridPlanes);

				// Axis styles
				info.AddValue("AxisStyles", s._axisStyles);

				// Legends
				info.AddValue("Legends", s._legends);

				// Graphic objects
				info.AddValue("GraphObjects", s._graphObjects);

				// Data clipping
				info.AddValue("DataClipping", s._dataClipping);

				// Plots
				info.AddValue("Plots", s._plotItems);
				*/
			}

			protected virtual XYPlotLayer SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				XYPlotLayer s = (o == null ? new XYPlotLayer(info) : (XYPlotLayer)o);
				int count;

				// size, position, rotation and scale
				var location = (XYPlotLayerPositionAndSize_V0)info.GetValue("LocationAndSize", s);
				s._cachedLayerSize = (SizeF)info.GetValue("CachedSize", s);
				s._cachedLayerPosition = (PointF)info.GetValue("CachedPosition", s);
				s.Location = location.ConvertToCurrentLocationVersion(s._cachedLayerSize, s._cachedLayerPosition);

				// CoordinateSystem
				s.CoordinateSystem = (G2DCoordinateSystem)info.GetValue("CoordinateSystem", s);
				s.CoordinateSystem.UpdateAreaSize(s._cachedLayerSize);

				// linked layers
				count = info.OpenArray("LinkedLayers");
				var linkedLayer = (Main.RelDocNodeProxy)info.GetValue("e", s);
				info.CloseArray(count);

				// Scales
				var linkedScales = (Altaxo.Graph.Scales.Deprecated.LinkedScaleCollection)info.GetValue("Scales", s);
				s.SetupOldAxes(linkedScales);
				ProvideLinkedScalesWithLinkedLayerIndex(s, linkedLayer, info);

				// Grid planes
				s.GridPlanes = (GridPlaneCollection)info.GetValue("GridPlanes", s);

				// Axis Styles
				s.AxisStyles = (AxisStyleCollection)info.GetValue("AxisStyles", s);

				// Legends
				var legends = (IList<IGraphicBase>)info.GetValue("Legends", s);

				// Graphic objects
				s.GraphObjects.AddRange((IEnumerable<IGraphicBase>)info.GetValue("GraphObjects", s));

				foreach (var item in legends)
				{
					if (item is TextGraphic)
					{
						var l = new LegendText((TextGraphic)item);
						s.GraphObjects.Add(l);
					}
				}

				// Data Clipping
				s.ClipDataToFrame = (LayerDataClipping)info.GetValue("DataClipping", s);

				// PlotItemCollection
				s.PlotItems = (PlotItemCollection)info.GetValue("Plots", s);

				return s;
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				XYPlotLayer s = SDeserialize(o, info, parent);
				s.CalculateMatrix();
				return s;
			}
		}

		#endregion Version 4

		#region Version 5

		/// <summary>
		/// In Version 5 we changed the Scales and divided into pure Scale and TickSpacing
		/// </summary>
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(XYPlotLayer), 5)]
		private class XmlSerializationSurrogate5 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				throw new InvalidOperationException("Serialization of old version");
				/*
				XYPlotLayer s = (XYPlotLayer)obj;

				// size, position, rotation and scale
				info.AddValue("LocationAndSize", s._location);
				info.AddValue("CachedSize", s._cachedLayerSize);
				info.AddValue("CachedPosition", s._cachedLayerPosition);

				// CoordinateSystem
				info.AddValue("CoordinateSystem", s._coordinateSystem);

				// Linked layers
				info.CreateArray("LinkedLayers", 1);
				info.AddValue("e", s._linkedLayerProxy);
				info.CommitArray();

				// Scales
				info.AddValue("Scales", s._scales);

				// Grid planes
				info.AddValue("GridPlanes", s._gridPlanes);

				// Axis styles
				info.AddValue("AxisStyles", s._axisStyles);

				// Legends
				info.AddValue("Legends", s._legends);

				// Graphic objects
				info.AddValue("GraphObjects", s._graphObjects);

				// Data clipping
				info.AddValue("DataClipping", s._dataClipping);

				// Plots
				info.AddValue("Plots", s._plotItems);
				*/
			}

			protected virtual XYPlotLayer SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				XYPlotLayer s = (o == null ? new XYPlotLayer(info) : (XYPlotLayer)o);
				int count;

				// size, position, rotation and scale
				var location = (XYPlotLayerPositionAndSize_V0)info.GetValue("LocationAndSize", s);
				s._cachedLayerSize = (SizeF)info.GetValue("CachedSize", s);
				s._cachedLayerPosition = (PointF)info.GetValue("CachedPosition", s);
				s.Location = location.ConvertToCurrentLocationVersion(s._cachedLayerSize, s._cachedLayerPosition);

				// CoordinateSystem
				s.CoordinateSystem = (G2DCoordinateSystem)info.GetValue("CoordinateSystem", s);
				s.CoordinateSystem.UpdateAreaSize(s._cachedLayerSize);

				// linked layers
				count = info.OpenArray("LinkedLayers");
				var linkedLayer = (Main.RelDocNodeProxy)info.GetValue("e", s);
				info.CloseArray(count);

				// Scales
				s.Scales = (ScaleCollection)info.GetValue("Scales", s);

				ProvideLinkedScalesWithLinkedLayerIndex(s, linkedLayer, info);

				// Grid planes
				s.GridPlanes = (GridPlaneCollection)info.GetValue("GridPlanes", s);

				// Axis Styles
				s.AxisStyles = (AxisStyleCollection)info.GetValue("AxisStyles", s);

				// Legends
				var legends = (IList<IGraphicBase>)info.GetValue("Legends", s);

				// Graphic objects
				s.GraphObjects.AddRange((IEnumerable<IGraphicBase>)info.GetValue("GraphObjects", s));

				foreach (var item in legends)
				{
					if (item is TextGraphic)
					{
						var l = new LegendText((TextGraphic)item);
						s.GraphObjects.Add(l);
					}
				}

				// Data Clipping
				s.ClipDataToFrame = (LayerDataClipping)info.GetValue("DataClipping", s);

				// PlotItemCollection
				s.PlotItems = (PlotItemCollection)info.GetValue("Plots", s);

				return s;
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				XYPlotLayer s = SDeserialize(o, info, parent);
				s.CalculateMatrix();
				return s;
			}
		}

		#endregion Version 5

		#region Version 6

		/// <summary>
		/// 2013-11-27 we now have <see cref="HostLayer"/> as base class.
		/// </summary>
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(XYPlotLayer), 6)]
		private class XmlSerializationSurrogate6 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				XYPlotLayer s = (XYPlotLayer)obj;

				info.AddBaseValueEmbedded(obj, typeof(HostLayer));

				// CoordinateSystem
				info.AddValue("CoordinateSystem", s._coordinateSystem);

				// Scales
				info.AddValue("Scales", s._scales);

				// Grid planes
				info.AddValue("GridPlanes", s._gridPlanes);

				// Axis styles
				info.AddValue("AxisStyles", s._axisStyles);

				// Data clipping
				info.AddValue("DataClipping", s._dataClipping);

				// Plots
				info.AddValue("Plots", s._plotItems);
			}

			protected virtual XYPlotLayer SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				XYPlotLayer s = (o == null ? new XYPlotLayer(info) : (XYPlotLayer)o);

				info.GetBaseValueEmbedded(s, typeof(HostLayer), parent);

				// CoordinateSystem
				s.CoordinateSystem = (G2DCoordinateSystem)info.GetValue("CoordinateSystem", s);
				s.CoordinateSystem.UpdateAreaSize(s._cachedLayerSize);

				// Scales
				s.Scales = (ScaleCollection)info.GetValue("Scales", s);

				// Grid planes
				s.GridPlanes = (GridPlaneCollection)info.GetValue("GridPlanes", s);

				// Axis Styles
				s.AxisStyles = (AxisStyleCollection)info.GetValue("AxisStyles", s);

				// Data Clipping
				s.ClipDataToFrame = (LayerDataClipping)info.GetValue("DataClipping", s);

				// PlotItemCollection
				s.PlotItems = (PlotItemCollection)info.GetValue("Plots", s);

				return s;
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				XYPlotLayer s = SDeserialize(o, info, parent);
				return s;
			}
		}

		#endregion Version 6

		private static void ProvideLinkedScalesWithLinkedLayerIndex(XYPlotLayer s, Main.RelDocNodeProxy linkedLayer, Altaxo.Serialization.Xml.IXmlDeserializationInfo info)
		{
			if (null != linkedLayer)
			{
				ProvideLinkedScalesWithLinkedLayerIndex(s, linkedLayer.DocumentPath, info);
			}
		}

		private static void ProvideLinkedScalesWithLinkedLayerIndex(XYPlotLayer s, Main.AbsoluteDocumentPath path, Altaxo.Serialization.Xml.IXmlDeserializationInfo info)
		{
			if (null != path && path.Count > 0)
			{
				var pathend = path[path.Count - 1];
				// extract layer number
				int layerNum = System.Xml.XmlConvert.ToInt32(pathend.Substring(1));
				foreach (var scaleAndTick in s.Scales)
					if (scaleAndTick is LinkedScale)
						((LinkedScale)scaleAndTick).SetLinkedLayerIndex(layerNum, info);
			}
		}

		private static void ProvideLinkedScalesWithLinkedLayerIndex(XYPlotLayer s, Main.RelativeDocumentPath path, Altaxo.Serialization.Xml.IXmlDeserializationInfo info)
		{
			if (null != path && path.Count > 0)
			{
				var pathend = path[path.Count - 1];
				// extract layer number
				int layerNum = System.Xml.XmlConvert.ToInt32(pathend.Substring(1));
				foreach (var scaleAndTick in s.Scales)
					if (scaleAndTick is LinkedScale)
						((LinkedScale)scaleAndTick).SetLinkedLayerIndex(layerNum, info);
			}
		}

		#endregion Serialization
	}
}