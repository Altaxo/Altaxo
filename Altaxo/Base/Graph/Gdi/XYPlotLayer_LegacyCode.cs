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

using Altaxo.Collections;
using Altaxo.Graph.Gdi.Background;
using Altaxo.Graph.Scales;
using Altaxo.Graph.Scales.Boundaries;
using Altaxo.Graph.Scales.Ticks;
using Altaxo.Serialization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Reflection;

namespace Altaxo.Graph.Gdi
{
	public partial class XYPlotLayer
		:
		HostLayer,
		IPlotArea
	{
		#region class XYPlotLayerSizeType

		/// <summary>
		/// The type of the size (i.e. width and height values.
		/// </summary>
		[Serializable]
		private enum XYPlotLayerSizeType
		{
			/// <summary>
			///  the value is a absolute value (not relative) in points (1/72 inch).
			/// </summary>
			AbsoluteValue,

			/// <summary>
			/// The value is relative to the graph document. This means that for instance the width of the layer
			/// is relative to the width of the graph document.
			/// </summary>
			RelativeToGraphDocument,

			/// <summary>
			/// The value is relative to the linked layer. This means that for instance the width of the layer
			/// is relative to the width of the linked layer.
			/// </summary>
			RelativeToLinkedLayer
		}

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.XYPlotLayer+SizeType", 0)]
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.XYPlotLayerSizeType", 1)]
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.Gdi.XYPlotLayerSizeType", 2)]
		public class SizeTypeXmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				info.SetNodeContent(obj.ToString());
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				string val = info.GetNodeContent();
				return System.Enum.Parse(typeof(XYPlotLayerSizeType), val, true);
			}
		}

		#endregion class XYPlotLayerSizeType

		#region XYPlotLayerPositionType

		/// <summary>
		/// The type of the position values  (i.e. x and y position of the layer).
		/// </summary>
		[Serializable]
		private enum XYPlotLayerPositionType
		{
			/// <summary>
			/// The value is a absolute value (not relative) in points (1/72 inch).
			/// </summary>
			[Description("Absolute value (points)")]
			AbsoluteValue,

			/// <summary>
			/// The value is relative to the graph document. This means that for instance the x position of the layer
			/// is relative to the width of the graph document. A x value of 0 would position the layer at the left edge of the
			/// graph document, a value of 1 on the right edge of the graph.
			/// </summary>
			[Description("Relative to graph size (0..1)")]
			RelativeToGraphDocument,

			/// <summary>
			/// The value relates the near edge (either upper or left) of this layer to the near edge of the linked layer.
			/// </summary>
			/// <remarks> The values are relative to the size of the linked layer.
			/// This means that for instance for a x position value of 0 the left edges of both layers are on the same position, for a value of 1
			/// this means that the left edge of this layer is on the right edge of the linked layer.
			/// </remarks>
			RelativeThisNearToLinkedLayerNear,

			/// <summary>
			/// The value relates the near edge (either upper or left) of this layer to the far edge (either right or bottom) of the linked layer.
			/// </summary>
			/// <remarks> The values are relative to the size of the linked layer.
			/// This means that for instance for a x position value of 0 the left edges of this layer is on the right edge of the linked layer,
			/// for a value of 1
			/// this means that the left edge of this layer is one width away from the right edge of the linked layer.
			/// </remarks>
			RelativeThisNearToLinkedLayerFar,

			/// <summary>
			/// The value relates the far edge (either right or bottom) of this layer to the near edge (either left or top) of the linked layer.
			/// </summary>
			/// <remarks> The values are relative to the size of the linked layer.
			/// This means that for instance for a x position value of 0 the right edge of this layer is on the left edge of the linked layer,
			/// for a value of 1
			/// this means that the right edge of this layer is one width away (to the right) from the leftt edge of the linked layer.
			/// </remarks>
			RelativeThisFarToLinkedLayerNear,

			/// <summary>
			/// The value relates the far edge (either right or bottom) of this layer to the far edge (either right or bottom) of the linked layer.
			/// </summary>
			/// <remarks> The values are relative to the size of the linked layer.
			/// This means that for instance for a x position value of 0 the right edge of this layer is on the right edge of the linked layer,
			/// for a value of 1
			/// this means that the right edge of this layer is one width away from the right edge of the linked layer, for a x value of -1 this
			/// means that the right edge of this layer is one width away to the left from the right edge of the linked layer and this falls together
			/// with the left edge of the linked layer.
			/// </remarks>
			RelativeThisFarToLinkedLayerFar
		}

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.XYPlotLayer+PositionType", 0)]
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.Gdi.XYPlotLayerPositionType", 1)]
		private class PositionTypeXmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				info.SetNodeContent(obj.ToString());
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				string val = info.GetNodeContent();
				return System.Enum.Parse(typeof(XYPlotLayerPositionType), val, true);
			}
		}

		#endregion XYPlotLayerPositionType

		#region XYPlotLayerPositionAndSize

		[Serializable]
		private class XYPlotLayerPositionAndSize_V0
		{
			/// <summary>
			/// The layers x position value, either absolute or relative, as determined by <see cref="_layerXPositionType"/>.
			/// </summary>
			private double _layerXPosition = 0;

			/// <summary>
			/// The type of the x position value, see <see cref="XYPlotLayerPositionType"/>.
			/// </summary>
			private XYPlotLayerPositionType _layerXPositionType = XYPlotLayerPositionType.AbsoluteValue;

			/// <summary>
			/// The layers y position value, either absolute or relative, as determined by <see cref="_layerYPositionType"/>.
			/// </summary>
			private double _layerYPosition = 0;

			/// <summary>
			/// The type of the y position value, see <see cref="XYPlotLayerPositionType"/>.
			/// </summary>
			private XYPlotLayerPositionType _layerYPositionType = XYPlotLayerPositionType.AbsoluteValue;

			/// <summary>
			/// The width of the layer, either as absolute value in point (1/72 inch), or as
			/// relative value as pointed out by <see cref="_layerWidthType"/>.
			/// </summary>
			private double _layerWidth = 0;

			/// <summary>
			/// The type of the value for the layer width, see <see cref="XYPlotLayerSizeType"/>.
			/// </summary>
			private XYPlotLayerSizeType _layerWidthType = XYPlotLayerSizeType.AbsoluteValue;

			/// <summary>
			/// The height of the layer, either as absolute value in point (1/72 inch), or as
			/// relative value as pointed out by <see cref="_layerHeightType"/>.
			/// </summary>
			private double _layerHeight = 0;

			/// <summary>
			/// The type of the value for the layer height, see <see cref="XYPlotLayerSizeType"/>.
			/// </summary>
			private XYPlotLayerSizeType _layerHeightType = XYPlotLayerSizeType.AbsoluteValue;

			/// <summary>The rotation angle (in degrees) of the layer.</summary>
			private double _layerAngle = 0; // Rotation

			/// <summary>The scaling factor of the layer, normally 1.</summary>
			private double _layerScale = 1;  // Scale

			[field: NonSerialized]
			private event EventHandler _changed;

			[NonSerialized]
			private object _parent;

			public XYPlotLayerPositionAndSize_V0()
			{
			}

			public XYPlotLayerPositionAndSize_V0(XYPlotLayerSizeType xzt, double xs, XYPlotLayerSizeType yzt, double ys, XYPlotLayerPositionType xpt, double xp, XYPlotLayerPositionType ypt, double yp, double rotation, double scale)
			{
				_layerWidthType = xzt;
				_layerWidth = xs;
				_layerHeightType = yzt;
				_layerHeight = ys;

				_layerXPositionType = xpt;
				_layerXPosition = xp;
				_layerYPositionType = ypt;
				_layerYPosition = yp;

				_layerAngle = rotation;
				_layerScale = scale;
			}

			public IItemLocation ConvertToCurrentLocationVersion(PointD2D cachedLayerSize, PointD2D cachedLayerPosition)
			{
				var newLoc = new ItemLocationDirect();
				switch (_layerWidthType)
				{
					case XYPlotLayerSizeType.AbsoluteValue:
						newLoc.XSize = Calc.RelativeOrAbsoluteValue.NewAbsoluteValue(_layerWidth);
						break;

					case XYPlotLayerSizeType.RelativeToGraphDocument:
						newLoc.XSize = Calc.RelativeOrAbsoluteValue.NewRelativeValue(_layerWidth);
						break;

					default:
						newLoc.XSize = Calc.RelativeOrAbsoluteValue.NewAbsoluteValue(cachedLayerSize.X);
						break;
				}

				switch (_layerHeightType)
				{
					case XYPlotLayerSizeType.AbsoluteValue:
						newLoc.YSize = Calc.RelativeOrAbsoluteValue.NewAbsoluteValue(_layerHeight);
						break;

					case XYPlotLayerSizeType.RelativeToGraphDocument:
						newLoc.YSize = Calc.RelativeOrAbsoluteValue.NewRelativeValue(_layerHeight);
						break;

					default:
						newLoc.YSize = Calc.RelativeOrAbsoluteValue.NewAbsoluteValue(cachedLayerSize.Y);
						break;
				}

				switch (_layerXPositionType)
				{
					case XYPlotLayerPositionType.AbsoluteValue:
						newLoc.XPosition = Calc.RelativeOrAbsoluteValue.NewAbsoluteValue(_layerXPosition);
						break;

					case XYPlotLayerPositionType.RelativeToGraphDocument:
						newLoc.XPosition = Calc.RelativeOrAbsoluteValue.NewRelativeValue(_layerXPosition);
						break;

					default:
						newLoc.XPosition = Calc.RelativeOrAbsoluteValue.NewAbsoluteValue(cachedLayerPosition.X);
						break;
				}

				switch (_layerYPositionType)
				{
					case XYPlotLayerPositionType.AbsoluteValue:
						newLoc.YPosition = Calc.RelativeOrAbsoluteValue.NewAbsoluteValue(_layerYPosition);
						break;

					case XYPlotLayerPositionType.RelativeToGraphDocument:
						newLoc.YPosition = Calc.RelativeOrAbsoluteValue.NewRelativeValue(_layerYPosition);
						break;

					default:
						newLoc.YPosition = Calc.RelativeOrAbsoluteValue.NewAbsoluteValue(cachedLayerPosition.Y);
						break;
				}

				newLoc.Rotation = _layerAngle;
				newLoc.Scale = _layerScale;

				return newLoc;
			}

			#region Serialization

			[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.XYPlotLayerPositionAndSize", 0)]
			[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.Gdi.XYPlotLayerPositionAndSize", 1)]
			private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
			{
				public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
				{
					var s = (XYPlotLayerPositionAndSize_V0)obj;

					info.AddValue("Width", s._layerWidth);
					info.AddEnum("WidthType", s._layerWidthType);
					info.AddValue("Height", s._layerHeight);
					info.AddEnum("HeightType", s._layerHeightType);
					info.AddValue("Angle", s._layerAngle);
					info.AddValue("Scale", s._layerScale);

					info.AddValue("XPos", s._layerXPosition);
					info.AddEnum("XPosType", s._layerXPositionType);
					info.AddValue("YPos", s._layerYPosition);
					info.AddEnum("YPosType", s._layerYPositionType);
				}

				protected virtual XYPlotLayerPositionAndSize_V0 SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
				{
					var s = null != o ? (XYPlotLayerPositionAndSize_V0)o : new XYPlotLayerPositionAndSize_V0();

					s._layerWidth = info.GetDouble("Width");
					s._layerWidthType = (XYPlotLayerSizeType)info.GetEnum("WidthType", typeof(XYPlotLayerSizeType));
					s._layerHeight = info.GetDouble("Height");
					s._layerHeightType = (XYPlotLayerSizeType)info.GetEnum("HeightType", typeof(XYPlotLayerSizeType));
					s._layerAngle = info.GetDouble("Angle");
					s._layerScale = info.GetDouble("Scale");

					s._layerXPosition = info.GetDouble("XPos");
					s._layerXPositionType = (XYPlotLayerPositionType)info.GetEnum("XPosType", typeof(XYPlotLayerPositionType));
					s._layerYPosition = info.GetDouble("YPos");
					s._layerYPositionType = (XYPlotLayerPositionType)info.GetEnum("YPosType", typeof(XYPlotLayerPositionType));

					return s;
				}

				public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
				{
					var s = SDeserialize(o, info, parent);
					return s;
				}
			}

			#endregion Serialization
		}

		#endregion XYPlotLayerPositionAndSize

		#region XYPlotLayerCollection

		public class XYPlotLayerCollection : List<XYPlotLayer>
		{
			public SizeF _graphSize;

			#region "Serialization"

			[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.XYPlotLayerCollection", 0)]
			[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.Gdi.XYPlotLayerCollection", 1)]
			private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
			{
				public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
				{
					throw new InvalidOperationException();
					/*
					XYPlotLayerCollection s = (XYPlotLayerCollection)obj;

					info.CreateArray("LayerArray",s.Count);
					for(int i=0;i<s.Count;i++)
						info.AddValue("XYPlotLayer",s[i]);
					info.CommitArray();
					*/
				}

				public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
				{
					XYPlotLayerCollection s = null != o ? (XYPlotLayerCollection)o : new XYPlotLayerCollection();

					int count = info.OpenArray();
					for (int i = 0; i < count; i++)
					{
						XYPlotLayer l = (XYPlotLayer)info.GetValue("XYPlotLayer", s);
						s.Add(l);
					}
					info.CloseArray(count);

					return s;
				}
			}

			[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.Gdi.XYPlotLayerCollection", 2)]
			private class XmlSerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
			{
				public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
				{
					throw new InvalidOperationException();
					/*
					XYPlotLayerCollection s = (XYPlotLayerCollection)obj;

					info.AddValue("Size", s._graphSize);

					info.CreateArray("LayerArray", s.Count);
					for (int i = 0; i < s.Count; i++)
						info.AddValue("XYPlotLayer", s[i]);
					info.CommitArray();
					*/
				}

				public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
				{
					XYPlotLayerCollection s = null != o ? (XYPlotLayerCollection)o : new XYPlotLayerCollection();

					s._graphSize = (SizeF)info.GetValue("Size", parent);

					int count = info.OpenArray();
					for (int i = 0; i < count; i++)
					{
						XYPlotLayer l = (XYPlotLayer)info.GetValue("XYPlotLayer", s);
						s.Add(l);
					}
					info.CloseArray(count);

					return s;
				}
			}

			#endregion "Serialization"

		#endregion XYPlotLayerCollection
		}
	}
}