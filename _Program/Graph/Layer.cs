/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002 Dr. Dirk Lellinger
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

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using Altaxo.Serialization;


namespace Altaxo.Graph
{
	/// <summary>
	/// Layer represents a rectangular area on the graph, which holds plot curves, axes and graphical elements.
	/// </summary>
	[SerializationSurrogate(0,typeof(Layer.SerializationSurrogate0))]
	[SerializationVersion(0)]
	public class Layer : System.Runtime.Serialization.IDeserializationCallback, System.ICloneable
	{
		#region Enumerations

		/// <summary>
		/// The type of the size (i.e. width and height values.
		/// </summary>
		[Serializable]
			public enum SizeType 
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


		/// <summary>
		/// The type of the position values  (i.e. x and y position of the layer).
		/// </summary>
		[Serializable]
			public enum PositionType 
		{
			/// <summary>
			/// The value is a absolute value (not relative) in points (1/72 inch).
			/// </summary>
			AbsoluteValue,


			/// <summary>
			/// The value is relative to the graph document. This means that for instance the x position of the layer
			/// is relative to the width of the graph document. A x value of 0 would position the layer at the left edge of the
			/// graph document, a value of 1 on the right edge of the graph.
			/// </summary>
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


		/// <summary>
		/// Provides how the axis is linked to the corresponding axis on the linked layer.
		/// </summary>
		[Serializable]
			public enum AxisLinkType
		{
			/// <summary>
			/// The axis is not linked, i.e. independent.
			/// </summary>
			None,
			/// <summary>
			/// The axis is linked straight, i.e. it has the same origin and end value as the corresponding axis of the linked layer.
			/// </summary>
			Straight,
			/// <summary>
			/// The axis is linked custom, i.e. origin and end of axis are translated linearly using formulas org'=a1+b1*org, end'=a2+b2*end.
			/// </summary>
			Custom
		}


		#endregion

		#region Cached member variables
		/// <summary>
		/// The cached layer position in points (1/72 inch) relative to the upper left corner
		/// of the graph document (upper left corner of the printable area).
		/// </summary>
		protected PointF m_LayerPosition = new PointF(0,0);


		/// <summary>
		/// The size of the layer in points (1/72 inch).
		/// </summary>
		/// <remarks>
		/// In case the size is absolute (see <see cref="SizeType"/>), this is the size of the layer. Otherwise
		/// it is only the cached value for the size, since the size is calculated then.
		/// </remarks>
		protected SizeF  m_LayerSize = new SizeF(0,0);

		protected Matrix m_ForwardMatrix = new Matrix();  // forward transformation m_ForwardMatrix
		protected Matrix m_ReverseMatrix = new Matrix(); // inverse transformation m_ForwardMatrix

		#endregion // Cached member variables

		#region Member variables

		/// <summary>True if the layer area should be filled with a background brush.</summary>
		protected bool m_bFillLayerArea=false;
		/// <summary>The background brush for the layer area.</summary>
		protected BrushHolder m_LayerAreaFillBrush = new BrushHolder(Color.Aqua);


		/// <summary>
		/// The layers x position value, either absolute or relative, as determined by <see cref="m_LayerXPositionType"/>.
		/// </summary>
		protected double m_LayerXPosition = 0;

		/// <summary>
		/// The type of the x position value, see <see cref="PositionType"/>.
		/// </summary>
		protected PositionType m_LayerXPositionType=PositionType.AbsoluteValue;

		/// <summary>
		/// The layers y position value, either absolute or relative, as determined by <see cref="m_LayerYPositionType"/>.
		/// </summary>
		protected double m_LayerYPosition = 0;

		/// <summary>
		/// The type of the y position value, see <see cref="PositionType"/>.
		/// </summary>
		protected PositionType m_LayerYPositionType=PositionType.AbsoluteValue;


		/// <summary>
		/// The width of the layer, either as absolute value in point (1/72 inch), or as 
		/// relative value as pointed out by <see cref="m_LayerWidthType"/>.
		/// </summary>
		protected double m_LayerWidth=0;

		/// <summary>
		/// The type of the value for the layer width, see <see cref="SizeType"/>.
		/// </summary>
		protected SizeType m_LayerWidthType=SizeType.AbsoluteValue;

		/// <summary>
		/// The height of the layer, either as absolute value in point (1/72 inch), or as 
		/// relative value as pointed out by <see cref="m_LayerHeightType"/>.
		/// </summary>
		protected double m_LayerHeight= 0;

		/// <summary>
		/// The type of the value for the layer height, see <see cref="SizeType"/>.
		/// </summary>
		protected SizeType m_LayerHeightType=SizeType.AbsoluteValue;

		/// <summary>The rotation angle (in degrees) of the layer.</summary>
		protected float  m_LayerAngle=0; // Rotation
		
		/// <summary>The scaling factor of the layer, normally 1.</summary>
		protected float  m_LayerScale=1;  // Scale

		/// <summary>The horizontal axis of the layer.</summary>
		protected Axis m_xAxis; // the X-Axis
		
		/// <summary>The vertical axis of the layer.</summary>
		protected Axis m_yAxis; // the Y-Axis

		/// <summary>True if the left axis should be drawn.</summary>
		protected bool m_ShowLeftAxis = true;
		/// <summary>True if the bottom axis should be drawn.</summary>
		protected bool m_ShowBottomAxis = true;
		/// <summary>True if the right axis should be drawn.</summary>
		protected bool m_ShowRightAxis = true;
		/// <summary>True if the top axis should be drawn.</summary>
		protected bool m_ShowTopAxis = true;

		protected XYLayerAxisStyle m_LeftAxisStyle = new XYLayerAxisStyle(EdgeType.Left);
		protected XYLayerAxisStyle m_BottomAxisStyle = new XYLayerAxisStyle(EdgeType.Bottom);
		protected XYLayerAxisStyle m_RightAxisStyle = new XYLayerAxisStyle(EdgeType.Right);
		protected XYLayerAxisStyle m_TopAxisStyle = new XYLayerAxisStyle(EdgeType.Top);

		protected LabelStyle m_LeftLabelStyle = new SimpleLabelStyle(EdgeType.Left);
		protected LabelStyle m_BottomLabelStyle = new SimpleLabelStyle(EdgeType.Bottom);
		protected LabelStyle m_RightLabelStyle = new SimpleLabelStyle(EdgeType.Right);
		protected LabelStyle m_TopLabelStyle = new SimpleLabelStyle(EdgeType.Top);

		protected ExtendedTextGraphObject m_LeftAxisTitle = null;
		protected ExtendedTextGraphObject m_BottomAxisTitle = null;
		protected ExtendedTextGraphObject m_RightAxisTitle = null;
		protected ExtendedTextGraphObject m_TopAxisTitle = null;

		protected ExtendedTextGraphObject m_Legend = null;


		protected GraphObjectCollection m_GraphObjects = new GraphObjectCollection();

		protected PlotList m_PlotItems;

		protected PlotGroup.Collection m_PlotGroups = new PlotGroup.Collection();

		/// <summary>
		/// The parent layer collection which contains this layer (or null if not member of such collection).
		/// </summary>
		protected LayerCollection m_ParentLayerCollection=null;
	
		/// <summary>
		/// The index inside the parent collection of this layer (or 0 if not member of such collection).
		/// </summary>
		protected int             m_LayerNumber=0;

		/// <summary>
		/// The layer to which this layer is linked to, or null if this layer is not linked.
		/// </summary>
		protected Layer						m_LinkedLayer;

		/// <summary>Indicate if x-axis is linked to the linked layer x axis.</summary>
		protected bool						m_LinkXAxis;
	
		/// <summary>The value a of x-axis link for link of origin: org' = a + b*org.</summary>
		protected double					m_LinkXAxisOrgA;
	
		/// <summary>The value b of x-axis link for link of origin: org' = a + b*org.</summary>
		protected double					m_LinkXAxisOrgB;

		/// <summary>The value a of x-axis link for link of end: end' = a + b*end.</summary>
		protected double					m_LinkXAxisEndA;
	
		/// <summary>The value b of x-axis link for link of end: end' = a + b*end.</summary>
		protected double					m_LinkXAxisEndB;

		/// <summary>Indicate if y-axis is linked to the linked layer y axis.</summary>
		protected bool						m_LinkYAxis;

		/// <summary>The value a of y-axis link for link of origin: org' = a + b*org.</summary>
		protected double					m_LinkYAxisOrgA;
	
		/// <summary>The value b of y-axis link for link of origin: org' = a + b*org.</summary>
		protected double					m_LinkYAxisOrgB;
	
		/// <summary>The value a of y-axis link for link of end: end' = a + b*end.</summary>
		protected double					m_LinkYAxisEndA;
	
		/// <summary>The value b of y-axis link for link of end: end' = a + b*end.</summary>
		protected double					m_LinkYAxisEndB;


		/// <summary>Number of times this event is disables, or 0 if it is enabled.</summary>
		int m_PlotAssociationXBoundariesChanged_EventSuspendCount;
		
		/// <summary>Number of times this event is disables, or 0 if it is enabled.</summary>
		int m_PlotAssociationYBoundariesChanged_EventSuspendCount;




		#endregion

		#region Event definitions

		/// <summary>Fired when the size of the layer changed.</summary>
		public event System.EventHandler SizeChanged;
	
		/// <summary>Fired when the position of the layer changed.</summary>
		public event System.EventHandler PositionChanged;
	
		/// <summary>Fired when one of the axis changed its origin or its end.</summary>
		public event System.EventHandler AxesChanged;

		#endregion

		#region Serialization

		/// <summary>Used to serialize the GraphDocument Version 0.</summary>
		public class SerializationSurrogate0 : System.Runtime.Serialization.ISerializationSurrogate
		{
			/// <summary>
			/// Serializes Layer Version 0.
			/// </summary>
			/// <param name="obj">The Layer to serialize.</param>
			/// <param name="info">The serialization info.</param>
			/// <param name="context">The streaming context.</param>
			public void GetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context	)
			{
				Layer s = (Layer)obj;

			
				// Layer style
				info.AddValue("FillLayerArea",s.m_bFillLayerArea);
				info.AddValue("LayerAreaFillBrush",s.m_LayerAreaFillBrush);

				// size, position, rotation and scale
				
				info.AddValue("WidthType",s.m_LayerWidthType);
				info.AddValue("HeightType",s.m_LayerHeightType);
				info.AddValue("Width",s.m_LayerWidth);
				info.AddValue("Height",s.m_LayerHeight);
				info.AddValue("CachedSize",s.m_LayerSize);

				info.AddValue("XPositionType",s.m_LayerXPositionType);
				info.AddValue("YPositionType",s.m_LayerYPositionType);
				info.AddValue("XPosition",s.m_LayerXPosition);
				info.AddValue("YPosition",s.m_LayerYPosition);
				info.AddValue("CachedPosition",s.m_LayerPosition);

				info.AddValue("Rotation",s.m_LayerAngle);
				info.AddValue("Scale",s.m_LayerScale);

				// axis related

				info.AddValue("XAxis",s.m_xAxis);
				info.AddValue("YAxis",s.m_yAxis);
				info.AddValue("LinkXAxis",s.m_LinkXAxis);
				info.AddValue("LinkYAxis",s.m_LinkYAxis);
				info.AddValue("LinkXAxisOrgA",s.m_LinkXAxisOrgA);
				info.AddValue("LinkXAxisOrgB",s.m_LinkXAxisOrgB);
				info.AddValue("LinkXAxisEndA",s.m_LinkXAxisEndA);
				info.AddValue("LinkXAxisEndB",s.m_LinkXAxisEndB);
				info.AddValue("LinkYAxisOrgA",s.m_LinkYAxisOrgA);
				info.AddValue("LinkYAxisOrgB",s.m_LinkYAxisOrgB);
				info.AddValue("LinkYAxisEndA",s.m_LinkYAxisEndA);
				info.AddValue("LinkYAxisEndB",s.m_LinkYAxisEndB);

			
				// Styles
				info.AddValue("ShowLeftAxis",s.m_ShowLeftAxis);
				info.AddValue("ShowBottomAxis",s.m_ShowBottomAxis);
				info.AddValue("ShowRightAxis",s.m_ShowRightAxis);
				info.AddValue("ShowTopAxis",s.m_ShowTopAxis);

				info.AddValue("LeftAxisStyle",s.m_LeftAxisStyle);
				info.AddValue("BottomAxisStyle",s.m_BottomAxisStyle);
				info.AddValue("RightAxisStyle",s.m_RightAxisStyle);
				info.AddValue("TopAxisStyle",s.m_TopAxisStyle);
			
			
				info.AddValue("LeftLabelStyle",s.m_LeftLabelStyle);
				info.AddValue("BottomLabelStyle",s.m_BottomLabelStyle);
				info.AddValue("RightLabelStyle",s.m_RightLabelStyle);
				info.AddValue("TopLabelStyle",s.m_TopLabelStyle);
			
		
				// Titles and legend
				info.AddValue("LeftAxisTitle",s.m_LeftAxisTitle);
				info.AddValue("BottomAxisTitle",s.m_BottomAxisTitle);
				info.AddValue("RightAxisTitle",s.m_RightAxisTitle);
				info.AddValue("TopAxisTitle",s.m_TopAxisTitle);
				info.AddValue("Legend",s.m_Legend);
			
				// Layer specific
				info.AddValue("LinkedLayer",s.m_LinkedLayer);
			
				info.AddValue("GraphObjects",s.m_GraphObjects);
				info.AddValue("PlotGroups",s.m_PlotGroups);
				info.AddValue("Plots",s.m_PlotItems);



			}

			/// <summary>
			/// Deserializes the Layer Version 0.
			/// </summary>
			/// <param name="obj">The empty Layer object to deserialize into.</param>
			/// <param name="info">The serialization info.</param>
			/// <param name="context">The streaming context.</param>
			/// <param name="selector">The deserialization surrogate selector.</param>
			/// <returns>The deserialized Layer.</returns>
			public object SetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context,System.Runtime.Serialization.ISurrogateSelector selector)
			{
				Layer s = (Layer)obj;
			
				// Layer style
				s.m_bFillLayerArea = info.GetBoolean("FillLayerArea");
				s.m_LayerAreaFillBrush = (BrushHolder)info.GetValue("LayerAreaFillBrush",typeof(BrushHolder));



				// size, position, rotation and scale
				
				s.m_LayerWidthType  = (SizeType)info.GetValue("WidthType",typeof(SizeType));
				s.m_LayerHeightType = (SizeType)info.GetValue("HeightType",typeof(SizeType));
				s.m_LayerWidth  = info.GetDouble("Width");
				s.m_LayerHeight = info.GetDouble("Height");
				s.m_LayerSize   = (SizeF)info.GetValue("CachedSize",typeof(SizeF));

				s.m_LayerXPositionType = (PositionType)info.GetValue("XPositionType",typeof(PositionType));
				s.m_LayerYPositionType = (PositionType)info.GetValue("YPositionType",typeof(PositionType));
				s.m_LayerXPosition = info.GetDouble("XPosition");
				s.m_LayerYPosition = info.GetDouble("YPosition");
				s.m_LayerPosition = (PointF)info.GetValue("CachedPosition",typeof(PointF));

				s.m_LayerAngle  = info.GetSingle("Rotation");
				s.m_LayerScale = info.GetSingle("Scale");

				// axis related

				s.m_xAxis = (Axis)info.GetValue("XAxis",typeof(Axis));
				s.m_yAxis = (Axis)info.GetValue("YAxis",typeof(Axis));
				s.m_LinkXAxis = info.GetBoolean("LinkXAxis");
				s.m_LinkYAxis = info.GetBoolean("LinkYAxis");
				s.m_LinkXAxisOrgA = info.GetDouble("LinkXAxisOrgA");
				s.m_LinkXAxisOrgB = info.GetDouble("LinkXAxisOrgB");
				s.m_LinkXAxisEndA = info.GetDouble("LinkXAxisEndA");
				s.m_LinkXAxisEndB = info.GetDouble("LinkXAxisEndB");
				s.m_LinkYAxisOrgA = info.GetDouble("LinkYAxisOrgA");
				s.m_LinkYAxisOrgB = info.GetDouble("LinkYAxisOrgB");
				s.m_LinkYAxisEndA = info.GetDouble("LinkYAxisEndA");
				s.m_LinkYAxisEndB = info.GetDouble("LinkYAxisEndB");


				// Styles
				s.m_ShowLeftAxis = info.GetBoolean("ShowLeftAxis");
				s.m_ShowBottomAxis = info.GetBoolean("ShowBottomAxis");
				s.m_ShowRightAxis = info.GetBoolean("ShowRightAxis");
				s.m_ShowTopAxis = info.GetBoolean("ShowTopAxis");

				s.m_LeftAxisStyle = (Graph.XYLayerAxisStyle)info.GetValue("LeftAxisStyle",typeof(Graph.XYLayerAxisStyle));
				s.m_BottomAxisStyle = (Graph.XYLayerAxisStyle)info.GetValue("BottomAxisStyle",typeof(Graph.XYLayerAxisStyle));
				s.m_RightAxisStyle = (Graph.XYLayerAxisStyle)info.GetValue("RightAxisStyle",typeof(Graph.XYLayerAxisStyle));
				s.m_TopAxisStyle = (Graph.XYLayerAxisStyle)info.GetValue("TopAxisStyle",typeof(Graph.XYLayerAxisStyle));
			
			
				s.m_LeftLabelStyle = (Graph.LabelStyle)info.GetValue("LeftLabelStyle",typeof(Graph.LabelStyle));
				s.m_BottomLabelStyle = (Graph.LabelStyle)info.GetValue("BottomLabelStyle",typeof(Graph.LabelStyle));
				s.m_RightLabelStyle = (Graph.LabelStyle)info.GetValue("RightLabelStyle",typeof(Graph.LabelStyle));
				s.m_TopLabelStyle = (Graph.LabelStyle)info.GetValue("TopLabelStyle",typeof(Graph.LabelStyle));
			
			
				// Titles and legend
				s.m_LeftAxisTitle = (Graph.ExtendedTextGraphObject)info.GetValue("LeftAxisTitle",typeof(Graph.ExtendedTextGraphObject));
				s.m_BottomAxisTitle = (Graph.ExtendedTextGraphObject)info.GetValue("BottomAxisTitle",typeof(Graph.ExtendedTextGraphObject));
				s.m_RightAxisTitle = (Graph.ExtendedTextGraphObject)info.GetValue("RightAxisTitle",typeof(Graph.ExtendedTextGraphObject));
				s.m_TopAxisTitle = (Graph.ExtendedTextGraphObject)info.GetValue("TopAxisTitle",typeof(Graph.ExtendedTextGraphObject));
				s.m_Legend = (Graph.ExtendedTextGraphObject)info.GetValue("Legend",typeof(Graph.ExtendedTextGraphObject));
			
				// Layer specific
				s.m_LinkedLayer = (Layer)info.GetValue("LinkedLayer",typeof(Layer));
			
				s.m_GraphObjects = (Graph.GraphObjectCollection)info.GetValue("GraphObjects",typeof(Graph.GraphObjectCollection));
				s.m_PlotGroups = (Graph.PlotGroup.Collection)info.GetValue("PlotGroups",typeof(Graph.PlotGroup.Collection));

				s.m_PlotItems = (PlotList)info.GetValue("Plots",typeof(PlotList));

				return s;
			}
		}

		/// <summary>
		/// Finale measures after deserialization.
		/// </summary>
		/// <param name="obj">Not used.</param>
		public void OnDeserialization(object obj)
		{
			m_ForwardMatrix = new Matrix();
			m_ReverseMatrix = new Matrix();
			CalculateMatrix();

			CreateEventLinks();
		}
		#endregion

		#region Constructors

		/// <summary>
		/// The copy constructor.
		/// </summary>
		/// <param name="from"></param>
		public Layer(Layer from)
		{
			// Layer style
			this.m_bFillLayerArea = from.m_bFillLayerArea;
			this.m_LayerAreaFillBrush = null==from.m_LayerAreaFillBrush ? null : (BrushHolder)from.m_LayerAreaFillBrush.Clone();

			// size, position, rotation and scale
			this.m_LayerWidthType  = from.m_LayerWidthType;
			this.m_LayerHeightType = from.m_LayerHeightType;
			this.m_LayerWidth  = from.m_LayerWidth;
			this.m_LayerHeight = from.m_LayerHeight ;
			this.m_LayerSize   = from.m_LayerSize;

			this.m_LayerXPositionType = from.m_LayerXPositionType;
			this.m_LayerYPositionType = from.m_LayerYPositionType;
			this.m_LayerXPosition = from.m_LayerXPosition ;
			this.m_LayerYPosition = from.m_LayerYPosition;
			this.m_LayerPosition = from.m_LayerPosition;

			this.m_LayerAngle  =from.m_LayerAngle;
			this.m_LayerScale = from.m_LayerScale;

			// axis related

			this.m_xAxis = null==from.m_xAxis ? null : (Axis)from.m_yAxis.Clone() ;
			this.m_yAxis = null==from.m_yAxis ? null : (Axis)from.m_yAxis.Clone() ;
			this.m_LinkXAxis = from.m_LinkXAxis;
			this.m_LinkYAxis = from.m_LinkYAxis;
			this.m_LinkXAxisOrgA = from.m_LinkXAxisOrgA;
			this.m_LinkXAxisOrgB = 	from.m_LinkXAxisOrgB;
			this.m_LinkXAxisEndA = from.m_LinkXAxisEndA;
			this.m_LinkXAxisEndB =from.m_LinkXAxisEndB;
			this.m_LinkYAxisOrgA = from.m_LinkYAxisOrgA;
			this.m_LinkYAxisOrgB = from.m_LinkYAxisOrgB;
			this.m_LinkYAxisEndA = from.m_LinkYAxisEndA ;
			this.m_LinkYAxisEndB = from.m_LinkYAxisEndB;


			// Styles
			this.m_ShowLeftAxis = from.m_ShowLeftAxis;
			this.m_ShowBottomAxis = from.m_ShowBottomAxis;
			this.m_ShowRightAxis = from.m_ShowRightAxis;
			this.m_ShowTopAxis = from.m_ShowTopAxis;

			this.m_LeftAxisStyle = null==from.m_LeftAxisStyle ? null : (Graph.XYLayerAxisStyle)from.m_LeftAxisStyle.Clone();
			this.m_BottomAxisStyle = null==from.m_BottomAxisStyle ? null : (Graph.XYLayerAxisStyle)from.m_BottomAxisStyle.Clone();
			this.m_RightAxisStyle = null==from.m_RightAxisStyle ? null : (Graph.XYLayerAxisStyle)from.m_RightAxisStyle.Clone();
			this.m_TopAxisStyle = null==from.m_TopAxisStyle ? null : (Graph.XYLayerAxisStyle)from.m_TopAxisStyle.Clone();
			
			
			this.m_LeftLabelStyle = null==from.m_LeftLabelStyle ? null : (Graph.LabelStyle)from.m_LeftLabelStyle.Clone();
			this.m_BottomLabelStyle = null==from.m_BottomLabelStyle ? null : (Graph.LabelStyle)from.m_BottomLabelStyle.Clone();
			this.m_RightLabelStyle = null==from.m_RightLabelStyle ? null : (Graph.LabelStyle)from.m_RightLabelStyle.Clone();
			this.m_TopLabelStyle = null==from.m_TopLabelStyle ? null : (Graph.LabelStyle)from.m_TopLabelStyle.Clone();
			
			
			// Titles and legend
			this.m_LeftAxisTitle = null==from.m_LeftAxisTitle ? null : (Graph.ExtendedTextGraphObject)from.m_LeftAxisTitle.Clone();
			this.m_BottomAxisTitle = null==from.m_BottomAxisTitle ? null : (Graph.ExtendedTextGraphObject)from.m_BottomAxisTitle.Clone();
			this.m_RightAxisTitle = null==from.m_RightAxisTitle ? null : (Graph.ExtendedTextGraphObject)from.m_RightAxisTitle.Clone();
			this.m_TopAxisTitle = null==from.m_TopAxisTitle ? null : (Graph.ExtendedTextGraphObject)from.m_TopAxisTitle.Clone();
			this.m_Legend = null==from.m_Legend ? null : (Graph.ExtendedTextGraphObject)from.m_Legend.Clone();
			
			// Layer specific
			this.m_LinkedLayer = from.m_LinkedLayer; // do not clone here, parent collection's duty to fix this!
			
			this.m_GraphObjects = null==from.m_GraphObjects ? null : new GraphObjectCollection(from.m_GraphObjects);

			this.m_PlotItems = null==from.m_PlotItems ? null : new PlotList(this,from.m_PlotItems);

			// special way neccessary to handle plot groups
			this.m_PlotGroups = null==from.m_PlotGroups ? null : from.m_PlotGroups.Clone(this.m_PlotItems,from.m_PlotItems);

			m_ForwardMatrix = new Matrix();
			m_ReverseMatrix = new Matrix();
			CalculateMatrix();

			CreateEventLinks();
		}


		public virtual object Clone()
		{
			return new Layer(this);
		}

		/// <summary>
		/// Creates a layer with standard position and size using the size of the printable area.
		/// </summary>
		/// <param name="prtSize">Size of the printable area in points (1/72 inch).</param>
		public Layer(SizeF prtSize)
			: this(new PointF(prtSize.Width*0.14f,prtSize.Height*0.14f),new SizeF(prtSize.Width*0.76f,prtSize.Height*0.7f))
		{
		}

	
		/// <summary>
		/// Creates a layer with position <paramref name="position"/> and size <paramref name="size"/>.
		/// </summary>
		/// <param name="position">The position of the layer on the printable area in points (1/72 inch).</param>
		/// <param name="size">The size of the layer in points (1/72 inch).</param>
		public Layer(PointF position, SizeF size)
		{
			this.Size     = size;
			this.Position = position;

			CalculateMatrix();

			m_PlotItems = new PlotList(this);

			// create axes and add event handlers to them
			m_xAxis = new LinearAxis(); // the X-Axis
			m_yAxis = new LinearAxis(); // the Y-Axis

		
		
			LeftAxisTitleString = "Y axis";
			BottomAxisTitleString = "X axis";

			CreateEventLinks();
		
		}

	
		#endregion

		#region Layer properties and methods


		void CreateEventLinks()
		{
			// restore the event chain
			if(null!=m_xAxis) m_xAxis.Changed += new EventHandler(this.OnXAxisChangedEventHandler);
			if(null!=m_yAxis) m_yAxis.Changed += new EventHandler(this.OnYAxisChangedEventHandler);

			if(null!=m_LeftAxisStyle) m_LeftAxisStyle.Changed += new EventHandler(this.OnChildChangedEventHandler);
			if(null!=m_BottomAxisStyle) m_BottomAxisStyle.Changed += new EventHandler(this.OnChildChangedEventHandler);
			if(null!=m_RightAxisStyle) m_RightAxisStyle.Changed += new EventHandler(this.OnChildChangedEventHandler);
			if(null!=m_TopAxisStyle) m_TopAxisStyle.Changed += new EventHandler(this.OnChildChangedEventHandler);

			if(null!=m_LeftLabelStyle) m_LeftLabelStyle.Changed += new EventHandler(this.OnChildChangedEventHandler);
			if(null!=m_BottomLabelStyle) m_BottomLabelStyle.Changed += new EventHandler(this.OnChildChangedEventHandler);
			if(null!=m_RightLabelStyle) m_RightLabelStyle.Changed += new EventHandler(this.OnChildChangedEventHandler);
			if(null!=m_TopLabelStyle) m_TopLabelStyle.Changed += new EventHandler(this.OnChildChangedEventHandler);

		
			if(null!=m_LeftAxisTitle) m_LeftAxisTitle.Changed += new EventHandler(this.OnChildChangedEventHandler);
			if(null!=m_BottomAxisTitle) m_BottomAxisTitle.Changed += new EventHandler(this.OnChildChangedEventHandler);
			if(null!=m_RightAxisTitle) m_RightAxisTitle.Changed += new EventHandler(this.OnChildChangedEventHandler);
			if(null!=m_TopAxisTitle) m_TopAxisTitle.Changed += new EventHandler(this.OnChildChangedEventHandler);

			if(null!=m_Legend) m_Legend.Changed += new EventHandler(this.OnChildChangedEventHandler);

			if(null!=m_LinkedLayer) m_LinkedLayer.AxesChanged += new EventHandler(this.OnLinkedLayerAxesChanged);
		
			if(null!=m_GraphObjects) m_GraphObjects.Changed += new EventHandler(this.OnChildChangedEventHandler);

			if(null!=m_PlotGroups) m_PlotGroups.Changed += new EventHandler(this.OnChildChangedEventHandler);

			if(null!=m_PlotItems)
			{
				m_PlotItems.ParentLayer = this;
				m_PlotItems.Changed += new EventHandler(this.OnChildChangedEventHandler);
			}
		}

		/// <summary>
		/// The layer number.
		/// </summary>
		/// <value>The layer number, i.e. the position of the layer in the layer collection.</value>
		public int Number
		{
			get { return this.m_LayerNumber; } 
		}

		public LayerCollection ParentLayerList
		{
			get { return m_ParentLayerCollection; }
		}

		public GraphObjectCollection GraphObjects
		{
			get { return m_GraphObjects; }
		}

		public void Remove(GraphObject go)
		{
			// test our own objects for removal (only that that _are_ removable)
			if(object.ReferenceEquals(go,this.m_LeftAxisTitle))
				m_LeftAxisTitle=null;
			else if(object.ReferenceEquals(go,this.m_TopAxisTitle))
				m_TopAxisTitle=null;
			else if(object.ReferenceEquals(go,this.m_RightAxisTitle))
				m_RightAxisTitle=null;
			else if(object.ReferenceEquals(go,this.m_BottomAxisTitle))
				m_BottomAxisTitle=null;
			else if(object.ReferenceEquals(go,this.m_Legend))
				m_Legend=null;
			else if(m_GraphObjects.Contains(go))
				m_GraphObjects.Remove(go);

		}

		/// <summary>
		/// Get / sets the layer this layer is linked to.
		/// </summary>
		/// <value>The layer this layer is linked to, or null if not linked.</value>
		public Layer LinkedLayer
		{
			get { return m_LinkedLayer; }
			set
			{

				// ignore the value if it would create a circular dependency
				if(IsLayerDependentOnMe(value))
					return;


				Layer oldValue = this.m_LinkedLayer;
				m_LinkedLayer =  value;

				if(!ReferenceEquals(oldValue,m_LinkedLayer))
				{
					// close the event handlers to the old layer
					if(null!=oldValue)
					{
						oldValue.SizeChanged -= new System.EventHandler(OnLinkedLayerSizeChanged);
						oldValue.PositionChanged -= new System.EventHandler(OnLinkedLayerPositionChanged);
						oldValue.AxesChanged -= new System.EventHandler(OnLinkedLayerAxesChanged);
					}

					// link the events to the new layer
					if(null!=m_LinkedLayer)
					{
						m_LinkedLayer.SizeChanged     += new System.EventHandler(OnLinkedLayerSizeChanged);
						m_LinkedLayer.PositionChanged += new System.EventHandler(OnLinkedLayerPositionChanged);
						m_LinkedLayer.AxesChanged			+= new System.EventHandler(OnLinkedLayerAxesChanged);
					}

				}
			}
		}

		/// <summary>
		/// Is this layer linked to another layer?
		/// </summary>
		/// <value>True if this layer is linked to another layer. See <see cref="LinkedLayer"/> to
		/// find out to which layer this layer is linked to.</value>
		public bool IsLinked
		{
			get { return null!=m_LinkedLayer; }
		}

		/// <summary>
		/// Checks if the provided layer or a linked layer of it is dependent on this layer.
		/// </summary>
		/// <param name="layer">The layer to check.</param>
		/// <returns>True if the provided layer or one of its linked layers is dependend on this layer.</returns>
		public bool IsLayerDependentOnMe(Layer layer)
		{
			while(null!=layer)
			{
				if(Layer.ReferenceEquals(layer,this))
				{
					// this means a circular dependency, so return true
					return true;
				}
				layer = layer.LinkedLayer;
			}
			return false; // no dependency detected
		}

		/// <summary>
		///  Only intended to use by LayerCollection! Sets the parent layer collection for this layer.
		/// </summary>
		/// <param name="lc">The layer collection this layer belongs to.</param>
		private void SetParentAndNumber(LayerCollection lc, int number)
		{
			m_ParentLayerCollection = lc;
			m_LayerNumber = number;
			
			if(m_ParentLayerCollection==null)
				m_LinkedLayer=null;
		}


		public PlotList PlotItems
		{
			get { return m_PlotItems; }
		}

		public PlotGroup.Collection PlotGroups
		{
			get { return m_PlotGroups; }
		}

		public void AddPlotAssociation(PlotAssociation[] pal)
		{
			foreach(PlotAssociation pa in pal)
				this.m_PlotItems.Add(new XYDataPlot(pa,new LineScatterPlotStyle()));
		}
	

		/// <summary>
		/// Creates a new legend, removing the old one.
		/// </summary>
		/// <remarks>The position of the old legend is <b>only</b> used for the new legend if the old legend's position is
		/// inside the layer. This prevents a "stealth" legend in case it is not visible by accident.
		/// </remarks>
		public void CreateNewLayerLegend()
		{
			// remove the legend if there are no plot curves on the layer
			if(PlotItems.Count==0)
			{
				m_Legend=null;
				OnInvalidate();
				return;
			}

			ExtendedTextGraphObject tgo;

			if(m_Legend!=null)
				tgo = new ExtendedTextGraphObject(m_Legend);
			else
				tgo = new ExtendedTextGraphObject();


			string strg="";
			for(int i=0;i<this.PlotItems.Count;i++)
			{
				strg+= String.Format("{0}\\L({1}) \\%({2})",(i==0?"":"\n"), i,i);
			}
			tgo.Text = strg;

			// if the position of the old legend is outside, use a new position
			if(null==m_Legend || m_Legend.Position.X<0 || m_Legend.Position.Y<0 || 
				m_Legend.Position.X>this.Size.Width || m_Legend.Position.Y>this.Size.Height)
				tgo.SetPosition(new PointF(0.1f*this.Size.Width,0.1f*this.Size.Height));
			else
				tgo.SetPosition(m_Legend.Position);

			m_Legend = tgo;

			OnInvalidate();
		}

		#endregion // Layer Properties and Methods

		#region Position and Size
		public PointF Position
		{
			get { return this.m_LayerPosition; }
			set
			{
				SetPosition(value.X,PositionType.AbsoluteValue,value.Y,PositionType.AbsoluteValue);
			}
		}

		public SizeF Size
		{
			get { return this.m_LayerSize; }
			set
			{
				SetSize(value.Width,SizeType.AbsoluteValue, value.Height,SizeType.AbsoluteValue);
			}
		}

		public float Rotation
		{
			get { return this.m_LayerAngle; }
			set
			{
				this.m_LayerAngle = value;
				this.CalculateMatrix();
				this.OnInvalidate();
			}
		}

		public float Scale
		{
			get { return this.m_LayerScale; }
			set
			{
				this.m_LayerScale = value;
				this.CalculateMatrix();
				this.OnInvalidate();
			}
		}

		protected void CalculateMatrix()
		{
			m_ForwardMatrix.Reset();
			m_ForwardMatrix.Translate(m_LayerPosition.X,m_LayerPosition.Y);
			m_ForwardMatrix.Scale(m_LayerScale,m_LayerScale);
			m_ForwardMatrix.Rotate(m_LayerAngle);
			m_ReverseMatrix=m_ForwardMatrix.Clone();
			m_ReverseMatrix.Invert();
		}


		public PointF GraphToLayerCoordinates(PointF pagecoordinates)
		{
			PointF[] pf = { pagecoordinates };
			m_ReverseMatrix.TransformPoints(pf);
			return pf[0];
		}

		/// <summary>
		/// Converts X,Y differences in page units to X,Y differences in layer units
		/// </summary>
		/// <param name="pagediff">X,Y coordinate differences in graph units</param>
		/// <returns>the convertes X,Y coordinate differences in layer units</returns>
		public PointF GraphToLayerDifferences(PointF pagediff)
		{
			// not very intelligent, maybe there is a simpler way to transform without
			// taking the translation into account
			PointF[] pf = { new PointF(pagediff.X + this.Position.X, pagediff.Y + this.Position.Y) };
			m_ReverseMatrix.TransformPoints(pf);
			return pf[0];
		}



		/// <summary>
		/// Transforms a graphics path from layer coordinates to graph (page) coordinates
		/// </summary>
		/// <param name="gp">the graphics path to convert</param>
		/// <returns>graphics path now in graph coordinates</returns>
		public GraphicsPath LayerToGraphCoordinates(GraphicsPath gp)
		{
			gp.Transform(m_ForwardMatrix);
			return gp;
		}



		public void SetPosition(double x, PositionType xpostype, double y, PositionType ypostype)
		{
			this.m_LayerXPosition = x;
			this.m_LayerXPositionType = xpostype;
			this.m_LayerYPosition = y;
			this.m_LayerYPositionType = ypostype;

			CalculateCachedPosition();
		}

		/// <summary>
		/// Calculates from the x position value, which can be absolute or relative, the
		/// x position in points.
		/// </summary>
		/// <param name="x">The horizontal position value of type xpostype.</param>
		/// <param name="xpostype">The type of the horizontal position value, see <see cref="PositionType"/>.</param>
		/// <returns>Calculated absolute position of the layer in units of points (1/72 inch).</returns>
		/// <remarks>The function does not change the member variables of the layer and can therefore used
		/// for position calculations without changing the layer. The function is not static because it has to use either the parent
		/// graph or the linked layer for the calculations.</remarks>
		public double XPositionToPointUnits(double x, PositionType xpostype)
		{
			GraphDocument graph = this.ParentLayerList as GraphDocument;

			switch(xpostype)
			{
				case PositionType.AbsoluteValue:
					break;
				case PositionType.RelativeToGraphDocument:
					if(graph!=null)
						x = x*graph.PrintableSize.Width;
					break;
				case PositionType.RelativeThisNearToLinkedLayerNear:
					if(LinkedLayer!=null)
						x = LinkedLayer.Position.X + x*LinkedLayer.Size.Width;
					break;
				case PositionType.RelativeThisNearToLinkedLayerFar:
					if(LinkedLayer!=null)
						x = LinkedLayer.Position.X + (1+x)*LinkedLayer.Size.Width;
					break;
				case PositionType.RelativeThisFarToLinkedLayerNear:
					if(LinkedLayer!=null)
						x = LinkedLayer.Position.X - this.Size.Width + x*LinkedLayer.Size.Width;
					break;
				case PositionType.RelativeThisFarToLinkedLayerFar:
					if(LinkedLayer!=null)
						x = LinkedLayer.Position.X - this.Size.Width + (1+x)*LinkedLayer.Size.Width;
					break;
			}
			return x;
		}

		/// <summary>
		/// Calculates from the y position value, which can be absolute or relative, the
		///  y position in points.
		/// </summary>
		/// <param name="y">The vertical position value of type xpostype.</param>
		/// <param name="ypostype">The type of the vertical position value, see <see cref="PositionType"/>.</param>
		/// <returns>Calculated absolute position of the layer in units of points (1/72 inch).</returns>
		/// <remarks>The function does not change the member variables of the layer and can therefore used
		/// for position calculations without changing the layer. The function is not static because it has to use either the parent
		/// graph or the linked layer for the calculations.</remarks>
		public double YPositionToPointUnits(double y, PositionType ypostype)
		{
			GraphDocument graph = this.ParentLayerList as GraphDocument;

			switch(ypostype)
			{
				case PositionType.AbsoluteValue:
					break;
				case PositionType.RelativeToGraphDocument:
					if(graph!=null)
						y = y*graph.PrintableSize.Height;
					break;
				case PositionType.RelativeThisNearToLinkedLayerNear:
					if(LinkedLayer!=null)
						y = LinkedLayer.Position.Y + y*LinkedLayer.Size.Height;
					break;
				case PositionType.RelativeThisNearToLinkedLayerFar:
					if(LinkedLayer!=null)
						y = LinkedLayer.Position.Y + (1+y)*LinkedLayer.Size.Height;
					break;
				case PositionType.RelativeThisFarToLinkedLayerNear:
					if(LinkedLayer!=null)
						y = LinkedLayer.Position.Y - this.Size.Height + y*LinkedLayer.Size.Height;
					break;
				case PositionType.RelativeThisFarToLinkedLayerFar:
					if(LinkedLayer!=null)
						y = LinkedLayer.Position.Y - this.Size.Height + (1+y)*LinkedLayer.Size.Height;
					break;
			}

			return y;
		}


		/// <summary>
		/// Calculates from the x position value in points (1/72 inch), the corresponding value in user units.
		/// </summary>
		/// <param name="x">The vertical position value in points.</param>
		/// <param name="xpostype_to_convert_to">The type of the vertical position value to convert to, see <see cref="PositionType"/>.</param>
		/// <returns>Calculated value of x in user units.</returns>
		/// <remarks>The function does not change the member variables of the layer and can therefore used
		/// for position calculations without changing the layer. The function is not static because it has to use either the parent
		/// graph or the linked layer for the calculations.</remarks>
		public double XPositionToUserUnits(double x, PositionType xpostype_to_convert_to)
		{

			GraphDocument graph = this.ParentLayerList as GraphDocument;

			switch(xpostype_to_convert_to)
			{
				case PositionType.AbsoluteValue:
					break;
				case PositionType.RelativeToGraphDocument:
					if(graph!=null)
						x = x/graph.PrintableSize.Width;
					break;
				case PositionType.RelativeThisNearToLinkedLayerNear:
					if(LinkedLayer!=null)
						x = (x-LinkedLayer.Position.X)/LinkedLayer.Size.Height;
					break;
				case PositionType.RelativeThisNearToLinkedLayerFar:
					if(LinkedLayer!=null)
						x = (x-LinkedLayer.Position.X)/LinkedLayer.Size.Width - 1;
					break;
				case PositionType.RelativeThisFarToLinkedLayerNear:
					if(LinkedLayer!=null)
						x = (x-LinkedLayer.Position.X + this.Size.Width)/LinkedLayer.Size.Width;
					break;
				case PositionType.RelativeThisFarToLinkedLayerFar:
					if(LinkedLayer!=null)
						x = (x-LinkedLayer.Position.X + this.Size.Width)/LinkedLayer.Size.Width - 1;
					break;
			}

			return x;
		}


		/// <summary>
		/// Calculates from the y position value in points (1/72 inch), the corresponding value in user units.
		/// </summary>
		/// <param name="y">The vertical position value in points.</param>
		/// <param name="ypostype_to_convert_to">The type of the vertical position value to convert to, see <see cref="PositionType"/>.</param>
		/// <returns>Calculated value of y in user units.</returns>
		/// <remarks>The function does not change the member variables of the layer and can therefore used
		/// for position calculations without changing the layer. The function is not static because it has to use either the parent
		/// graph or the linked layer for the calculations.</remarks>
		public double YPositionToUserUnits(double y, PositionType ypostype_to_convert_to)
		{

			GraphDocument graph = this.ParentLayerList as GraphDocument;

			switch(ypostype_to_convert_to)
			{
				case PositionType.AbsoluteValue:
					break;
				case PositionType.RelativeToGraphDocument:
					if(graph!=null)
						y = y/graph.PrintableSize.Height;
					break;
				case PositionType.RelativeThisNearToLinkedLayerNear:
					if(LinkedLayer!=null)
						y = (y-LinkedLayer.Position.Y)/LinkedLayer.Size.Height;
					break;
				case PositionType.RelativeThisNearToLinkedLayerFar:
					if(LinkedLayer!=null)
						y = (y-LinkedLayer.Position.Y)/LinkedLayer.Size.Height - 1;
					break;
				case PositionType.RelativeThisFarToLinkedLayerNear:
					if(LinkedLayer!=null)
						y = (y-LinkedLayer.Position.Y + this.Size.Height)/LinkedLayer.Size.Height;
					break;
				case PositionType.RelativeThisFarToLinkedLayerFar:
					if(LinkedLayer!=null)
						y = (y-LinkedLayer.Position.Y + this.Size.Height)/LinkedLayer.Size.Height - 1;
					break;
			}

			return y;
		}


		/// <summary>
		/// Sets the cached position value in <see cref="m_LayerPosition"/> by calculating it
		/// from the position values (<see cref="m_LayerXPosition"/> and <see cref="m_LayerYPosition"/>) 
		/// and the position types (<see cref="m_LayerXPositionType"/> and <see cref="m_LayerYPositionType"/>).
		/// </summary>
		protected void CalculateCachedPosition()
		{
			PointF newPos = new PointF(
				(float)XPositionToPointUnits(this.m_LayerXPosition,this.m_LayerXPositionType),
				(float)YPositionToPointUnits(this.m_LayerYPosition, this.m_LayerYPositionType));
			if(newPos != this.m_LayerPosition)
			{
				this.m_LayerPosition=newPos;
				this.CalculateMatrix();
				OnPositionChanged();
			}
		}


		public void SetSize(double width, SizeType widthtype, double height, SizeType heighttype)
		{
			this.m_LayerWidth = width;
			this.m_LayerWidthType = widthtype;
			this.m_LayerHeight = height;
			this.m_LayerHeightType = heighttype;

			CalculateCachedSize();
		}


		protected double WidthToPointUnits(double width, SizeType widthtype)
		{
			GraphDocument graph = this.ParentLayerList as GraphDocument;

			switch(widthtype)
			{
				case SizeType.RelativeToGraphDocument:
					if(null!=graph)
						width *= graph.PrintableSize.Width;
					break;
				case SizeType.RelativeToLinkedLayer:
					if(null!=LinkedLayer)
						width *= LinkedLayer.Size.Width;
					break;
			}
			return width;
		}

		protected double HeightToPointUnits(double height, SizeType heighttype)
		{
			GraphDocument graph = this.ParentLayerList as GraphDocument;
			switch(heighttype)
			{
				case SizeType.RelativeToGraphDocument:
					if(null!=graph)
						height *= graph.PrintableSize.Height;
					break;
				case SizeType.RelativeToLinkedLayer:
					if(null!=LinkedLayer)
						height *= LinkedLayer.Size.Height;
					break;
			}
			return height;
		}


		/// <summary>
		/// Convert the width in points (1/72 inch) to user units of the type <paramref name="widthtype_to_convert_to"/>.
		/// </summary>
		/// <param name="width">The height value to convert (in point units).</param>
		/// <param name="widthtype_to_convert_to">The user unit type to convert to.</param>
		/// <returns>The value of the width in user units.</returns>
		protected double WidthToUserUnits(double width, SizeType widthtype_to_convert_to)
		{
	
			switch(widthtype_to_convert_to)
			{
				case SizeType.RelativeToGraphDocument:
					GraphDocument graph = this.ParentLayerList as GraphDocument;
					if(null!=graph)
						width /= graph.PrintableSize.Width;
					break;
				case SizeType.RelativeToLinkedLayer:
					if(null!=LinkedLayer)
						width /= LinkedLayer.Size.Width;
					break;
			}
			return width;
		}


		/// <summary>
		/// Convert the heigth in points (1/72 inch) to user units of the type <paramref name="heighttype_to_convert_to"/>.
		/// </summary>
		/// <param name="height">The height value to convert (in point units).</param>
		/// <param name="heighttype_to_convert_to">The user unit type to convert to.</param>
		/// <returns>The value of the height in user units.</returns>
		protected double HeightToUserUnits(double height, SizeType heighttype_to_convert_to)
		{

			switch(heighttype_to_convert_to)
			{
				case SizeType.RelativeToGraphDocument:
					GraphDocument graph = this.ParentLayerList as GraphDocument;
					if(null!=graph)
						height /= graph.PrintableSize.Height;
					break;
				case SizeType.RelativeToLinkedLayer:
					if(null!=LinkedLayer)
						height /= LinkedLayer.Size.Height;
					break;
			}
			return height;
		}


		/// <summary>
		/// Sets the cached size value in <see cref="m_LayerSize"/> by calculating it
		/// from the position values (<see cref="m_LayerWidth"/> and <see cref="m_LayerHeight"/>) 
		/// and the size types (<see cref="m_LayerWidthType"/> and <see cref="m_LayerHeightType"/>).
		/// </summary>
		protected void CalculateCachedSize()
		{
			SizeF newSize = new SizeF(
				(float)WidthToPointUnits(this.m_LayerWidth,this.m_LayerWidthType),
				(float)HeightToPointUnits(this.m_LayerHeight, this.m_LayerHeightType));
			if(newSize != this.m_LayerSize)
			{
				this.m_LayerSize=newSize;
				this.CalculateMatrix();
				OnSizeChanged();
			}
		}


		/// <summary>Returns the user x position value of the layer.</summary>
		/// <value>User x position value of the layer.</value>
		public double UserXPosition
		{
			get { return this.m_LayerXPosition; }
		}

		/// <summary>Returns the user y position value of the layer.</summary>
		/// <value>User y position value of the layer.</value>
		public double UserYPosition
		{
			get { return this.m_LayerYPosition; }
		}

		/// <summary>Returns the user width value of the layer.</summary>
		/// <value>User width value of the layer.</value>
		public double UserWidth
		{
			get { return this.m_LayerWidth; }
		}

		/// <summary>Returns the user height value of the layer.</summary>
		/// <value>User height value of the layer.</value>
		public double UserHeight
		{
			get { return this.m_LayerHeight; }
		}

		/// <summary>Returns the type of the user x position value of the layer.</summary>
		/// <value>Type of the user x position value of the layer.</value>
		public PositionType UserXPositionType
		{
			get { return this.m_LayerXPositionType; }
		}

		/// <summary>Returns the type of the user y position value of the layer.</summary>
		/// <value>Type of the User y position value of the layer.</value>
		public PositionType UserYPositionType
		{
			get { return this.m_LayerYPositionType; }
		}

		/// <summary>Returns the type of the the user width value of the layer.</summary>
		/// <value>Type of the User width value of the layer.</value>
		public SizeType UserWidthType
		{
			get { return this.m_LayerWidthType; }
		}

		/// <summary>Returns the the type of the user height value of the layer.</summary>
		/// <value>Type of the User height value of the layer.</value>
		public SizeType UserHeightType
		{
			get { return this.m_LayerHeightType; }
		}



		/// <summary>
		/// Measures to do when the position of the linked layer changed.
		/// </summary>
		/// <param name="sender">The sender of the event.</param>
		/// <param name="e">The event args.</param>
		protected void OnLinkedLayerPositionChanged(object sender, System.EventArgs e)
		{
			CalculateCachedPosition();
		}

		/// <summary>
		/// Measures to do when the size of the linked layer changed.
		/// </summary>
		/// <param name="sender">The sender of the event.</param>
		/// <param name="e">The event args.</param>
		protected void OnLinkedLayerSizeChanged(object sender, System.EventArgs e)
		{
			CalculateCachedSize();
			CalculateCachedPosition();
		}


		#endregion // Position and Size

		#region Axis related

		/// <summary>Gets or sets the x axis of this layer.</summary>
		/// <value>The x axis of the layer.</value>
		public Axis XAxis
		{
			get { return m_xAxis; }
			set
			{
				if(null!=m_xAxis)
					m_xAxis.Changed -= new System.EventHandler(this.OnXAxisChangedEventHandler);
				
				m_xAxis = value;

				if(null!=m_xAxis)
					m_xAxis.Changed += new System.EventHandler(this.OnXAxisChangedEventHandler);


				// now we have to inform all the PlotItems that a new axis was loaded
				
				// we have to disable our own Handler since if we change one DataBound of a association,
				//it generates a OnBoundaryChanged, and then all boundaries are merges into the axis boundary, 
				//but (alas!) not all boundaries are now of the new type!
				m_PlotAssociationXBoundariesChanged_EventSuspendCount++; 
				
				m_xAxis.DataBounds.EventsEnabled=false;
				m_xAxis.DataBounds.Reset();
				foreach(PlotItem pa in this.PlotItems)
				{
					if(pa.Data is Graph.IXBoundsHolder)
					{
						// first ensure the right data bound object is set on the PlotAssociation
						((IXBoundsHolder)pa.Data).SetXBoundsFromTemplate(m_xAxis.DataBounds); // ensure that data bound object is of the right type
						// now merge the bounds with x and yAxis
						((IXBoundsHolder)pa.Data).MergeXBoundsInto(m_xAxis.DataBounds); // merge all x-boundaries in the x-axis boundary object
				
					}
				}
				m_PlotAssociationXBoundariesChanged_EventSuspendCount = Math.Max(0,m_PlotAssociationXBoundariesChanged_EventSuspendCount-1);
				m_xAxis.DataBounds.EventsEnabled=true;
			}
		}

	
		/// <summary>Gets or sets the y axis of this layer.</summary>
		/// <value>The y axis of the layer.</value>
		public Axis YAxis
		{
			get { return m_yAxis; }
			set
			{
				if(null!=m_yAxis)
					m_yAxis.Changed -= new System.EventHandler(this.OnYAxisChangedEventHandler);
				
				m_yAxis = value;

				if(null!=m_yAxis)
					m_yAxis.Changed += new System.EventHandler(this.OnYAxisChangedEventHandler);


				// now we have to inform all the PlotAssociations that a new axis was loaded
				
				// we have to disable our own Handler since if we change one DataBound of a association,
				//it generates a OnBoundaryChanged, and then all boundaries are merges into the axis boundary, 
				//but (alas!) not all boundaries are now of the new type!
				m_PlotAssociationYBoundariesChanged_EventSuspendCount++; 

				m_yAxis.DataBounds.EventsEnabled=false;
				m_yAxis.DataBounds.Reset();
				foreach(PlotItem pa in this.PlotItems)
				{
					if(pa.Data is Graph.IYBoundsHolder)
					{
						// first ensure the right data bound object is set on the PlotAssociation
						((IYBoundsHolder)pa.Data).SetYBoundsFromTemplate(m_yAxis.DataBounds); // ensure that data bound object is of the right type
						// now merge the bounds with x and yAxis
						((IYBoundsHolder)pa.Data).MergeYBoundsInto(m_yAxis.DataBounds); // merge all x-boundaries in the x-axis boundary object
				
					}
				}
				m_PlotAssociationYBoundariesChanged_EventSuspendCount = Math.Max(0,m_PlotAssociationYBoundariesChanged_EventSuspendCount-1);
				m_yAxis.DataBounds.EventsEnabled=true;
			}
		}


		/// <summary>Indicates if x axis is linked to the linked layer x axis.</summary>
		/// <value>True if x axis is linked to the linked layer x axis.</value>
		public bool IsXAxisLinked
		{
			get { return this.m_LinkXAxis; }
			set
			{
				bool oldValue = this.m_LinkXAxis;
				m_LinkXAxis = value;
				if(value!=oldValue && value==true)
				{
					if(null!=LinkedLayer)
						OnLinkedLayerAxesChanged(LinkedLayer, new System.EventArgs());
				}
			}
		}

	
		/// <summary>Indicates if y axis is linked to the linked layer y axis.</summary>
		/// <value>True if y axis is linked to the linked layer y axis.</value>
		public bool IsYAxisLinked
		{
			get { return this.m_LinkYAxis; }
			set
			{
				bool oldValue = this.m_LinkYAxis;
				m_LinkYAxis = value;
				if(value!=oldValue && value==true)
				{
					if(null!=LinkedLayer)
						OnLinkedLayerAxesChanged(LinkedLayer, new System.EventArgs());
				}
			}
		}


		/// <summary>The type of x axis link.</summary>
		/// <value>Can be either None, Straight or Custom link.</value>
		public AxisLinkType XAxisLinkType
		{
			get 
			{
				if(!this.m_LinkXAxis)
					return AxisLinkType.None;
				else if(this.m_LinkXAxisOrgA==0 && this.m_LinkXAxisOrgB==1 && this.m_LinkXAxisEndA==0 && this.m_LinkXAxisEndB==1)
					return AxisLinkType.Straight;
				else return AxisLinkType.Custom;
			}
			set 
			{
				if(value==AxisLinkType.None)
					this.m_LinkXAxis = false;
				else
				{
					this.m_LinkXAxis = true;
					if(value==AxisLinkType.Straight)
					{
						this.m_LinkXAxisOrgA=0;
						this.m_LinkXAxisOrgB=1;
						this.m_LinkXAxisEndA=0;
						this.m_LinkXAxisEndB=1;
					}
					if(null!=LinkedLayer)
						OnLinkedLayerAxesChanged(LinkedLayer, new System.EventArgs());
				}
			}
		}

		
		/// <summary>The type of y axis link.</summary>
		/// <value>Can be either None, Straight or Custom link.</value>
		public AxisLinkType YAxisLinkType
		{
			get 
			{
				if(!this.m_LinkYAxis)
					return AxisLinkType.None;
				else if(this.m_LinkYAxisOrgA==0 && this.m_LinkYAxisOrgB==1 && this.m_LinkYAxisEndA==0 && this.m_LinkYAxisEndB==1)
					return AxisLinkType.Straight;
				else return AxisLinkType.Custom;
			}
			set 
			{
				if(value==AxisLinkType.None)
				{
					this.m_LinkYAxis = false;
				}
				else
				{
					this.m_LinkYAxis = true;
					if(value==AxisLinkType.Straight)
					{
						this.m_LinkYAxisOrgA=0;
						this.m_LinkYAxisOrgB=1;
						this.m_LinkYAxisEndA=0;
						this.m_LinkYAxisEndB=1;
					}
					if(null!=LinkedLayer)
						OnLinkedLayerAxesChanged(LinkedLayer, new System.EventArgs());
				}
			}
		}


		/// <summary>
		/// Set all parameters of the x axis link by once.
		/// </summary>
		/// <param name="orgA">The value a of x-axis link for link of axis origin: org' = a + b*org.</param>
		/// <param name="orgB">The value b of x-axis link for link of axis origin: org' = a + b*org.</param>
		/// <param name="endA">The value a of x-axis link for link of axis end: end' = a + b*end.</param>
		/// <param name="endB">The value b of x-axis link for link of axis end: end' = a + b*end.</param>
		public void SetXAxisLinkParameter(double orgA, double orgB, double endA, double endB)
		{
			if(
				(orgA!=m_LinkXAxisOrgA) ||
				(orgB!=m_LinkXAxisOrgB) ||
				(endA!=m_LinkXAxisEndA) ||
				(endB!=m_LinkXAxisEndB) )
			{
				m_LinkXAxisOrgA = orgA;
				m_LinkXAxisOrgB = orgB;
				m_LinkXAxisEndA = endA;
				m_LinkXAxisEndB = endB;
					
				if(IsLinked)
					OnLinkedLayerAxesChanged(LinkedLayer,new EventArgs());
			}
		}

		/// <summary>The value a of x-axis link for link of origin: org' = a + b*org.</summary>
		/// <value>The value a of x-axis link for link of origin: org' = a + b*org.</value>
		public double LinkXAxisOrgA
		{
			get { return m_LinkXAxisOrgA; }
			set
			{
				if(m_LinkXAxisOrgA!=value)
				{
					m_LinkXAxisOrgA = value;
					
					if(IsLinked)
						OnLinkedLayerAxesChanged(LinkedLayer,new EventArgs());
				}
			}
		}

		/// <summary>The value b of x-axis link for link of origin: org' = a + b*org.</summary>
		/// <value>The value b of x-axis link for link of origin: org' = a + b*org.</value>
		public double LinkXAxisOrgB
		{
			get { return m_LinkXAxisOrgB; }
			set
			{
				if(m_LinkXAxisOrgB!=value)
				{
					m_LinkXAxisOrgB = value;
					
					if(IsLinked)
						OnLinkedLayerAxesChanged(LinkedLayer,new EventArgs());
				}
			}
		}

		/// <summary>The value a of x-axis link for link of axis end: end' = a + b*end.</summary>
		/// <value>The value a of x-axis link for link of axis end: end' = a + b*end.</value>
		public double LinkXAxisEndA
		{
			get { return m_LinkXAxisEndA; }
			set
			{
				if(m_LinkXAxisEndA!=value)
				{
					m_LinkXAxisEndA = value;
					
					if(IsLinked)
						OnLinkedLayerAxesChanged(LinkedLayer,new EventArgs());
				}
			}
		}


		/// <summary>The value b of x-axis link for link of axis end: end' = a + b*end.</summary>
		/// <value>The value b of x-axis link for link of axis end: end' = a + b*end.</value>
		public double LinkXAxisEndB
		{
			get { return m_LinkXAxisEndB; }
			set
			{
				if(m_LinkXAxisEndB!=value)
				{
					m_LinkXAxisEndB = value;
					
					if(IsLinked)
						OnLinkedLayerAxesChanged(LinkedLayer,new EventArgs());
				}
			}
		}


		/// <summary>
		/// Set all parameters of the y axis link by once.
		/// </summary>
		/// <param name="orgA">The value a of y-axis link for link of axis origin: org' = a + b*org.</param>
		/// <param name="orgB">The value b of y-axis link for link of axis origin: org' = a + b*org.</param>
		/// <param name="endA">The value a of y-axis link for link of axis end: end' = a + b*end.</param>
		/// <param name="endB">The value b of y-axis link for link of axis end: end' = a + b*end.</param>
		public void SetYAxisLinkParameter(double orgA, double orgB, double endA, double endB)
		{
			if(
				(orgA!=m_LinkYAxisOrgA) ||
				(orgB!=m_LinkYAxisOrgB) ||
				(endA!=m_LinkYAxisEndA) ||
				(endB!=m_LinkYAxisEndB) )
			{
				m_LinkYAxisOrgA = orgA;
				m_LinkYAxisOrgB = orgB;
				m_LinkYAxisEndA = endA;
				m_LinkYAxisEndB = endB;
					
				if(IsLinked)
					OnLinkedLayerAxesChanged(LinkedLayer,new EventArgs());
			}
		}


		/// <summary>The value a of y-axis link for link of origin: org' = a + b*org.</summary>
		/// <value>The value a of y-axis link for link of origin: org' = a + b*org.</value>
		public double LinkYAxisOrgA
		{
			get { return m_LinkYAxisOrgA; }
			set
			{
				if(m_LinkYAxisOrgA!=value)
				{
					m_LinkYAxisOrgA = value;
					
					if(IsLinked)
						OnLinkedLayerAxesChanged(LinkedLayer,new EventArgs());
				}
			}
		}

		/// <summary>The value b of y-axis link for link of origin: org' = a + b*org.</summary>
		/// <value>The value b of y-axis link for link of origin: org' = a + b*org.</value>
		public double LinkYAxisOrgB
		{
			get { return m_LinkYAxisOrgB; }
			set
			{
				if(m_LinkYAxisOrgB!=value)
				{
					m_LinkYAxisOrgB = value;
					
					if(IsLinked)
						OnLinkedLayerAxesChanged(LinkedLayer,new EventArgs());
				}
			}
		}

		/// <summary>The value a of y-axis link for link of axis end: end' = a + b*end.</summary>
		/// <value>The value a of y-axis link for link of axis end: end' = a + b*end.</value>
		public double LinkYAxisEndA
		{
			get { return m_LinkYAxisEndA; }
			set
			{
				if(m_LinkYAxisEndA!=value)
				{
					m_LinkYAxisEndA = value;
					
					if(IsLinked)
						OnLinkedLayerAxesChanged(LinkedLayer,new EventArgs());
				}
			}
		}


		/// <summary>The value b of y-axis link for link of axis end: end' = a + b*end.</summary>
		/// <value>The value b of y-axis link for link of axis end: end' = a + b*end.</value>
		public double LinkYAxisEndB
		{
			get { return m_LinkYAxisEndB; }
			set
			{
				if(m_LinkYAxisEndB!=value)
				{
					m_LinkYAxisEndB = value;
					
					if(IsLinked)
						OnLinkedLayerAxesChanged(LinkedLayer,new EventArgs());
				}
			}
		}


		/// <summary>
		/// Measures to do when one of the axis of the linked layer changed.
		/// </summary>
		/// <param name="sender">The sender of the event.</param>
		/// <param name="e">The event args.</param>
		protected void OnLinkedLayerAxesChanged(object sender, System.EventArgs e)
		{
			if(null==LinkedLayer)
				return; // this should not happen, since what is sender then?

			if(IsXAxisLinked && null!=LinkedLayer)
			{
				this.m_xAxis.ProcessDataBounds(	
					m_LinkXAxisOrgA+m_LinkXAxisOrgB*LinkedLayer.XAxis.Org,true,
					m_LinkXAxisEndA+m_LinkXAxisEndB*LinkedLayer.XAxis.End,true);
			}

			if(IsYAxisLinked && null!=LinkedLayer)
			{
				this.m_yAxis.ProcessDataBounds(	
					m_LinkYAxisOrgA+m_LinkYAxisOrgB*LinkedLayer.YAxis.Org,true,
					m_LinkYAxisEndA+m_LinkYAxisEndB*LinkedLayer.YAxis.End,true);
			}

		}


		#endregion // Axis related

		#region Style properties
		public XYLayerAxisStyle LeftAxisStyle
		{
			get { return m_LeftAxisStyle; }
		}
		public XYLayerAxisStyle BottomAxisStyle
		{
			get { return m_BottomAxisStyle; }
		}
		public XYLayerAxisStyle RightAxisStyle
		{
			get { return m_RightAxisStyle; }
		}
		public XYLayerAxisStyle TopAxisStyle
		{
			get { return m_TopAxisStyle; }
		}


		public LabelStyle LeftLabelStyle
		{
			get { return m_LeftLabelStyle; }
		}
		public LabelStyle RightLabelStyle
		{
			get { return m_RightLabelStyle; }
		}
		public LabelStyle BottomLabelStyle
		{
			get { return m_BottomLabelStyle; }
		}
		public LabelStyle TopLabelStyle
		{
			get { return m_TopLabelStyle; }
		}
		
		public bool LeftAxisEnabled
		{
			get { return this.m_ShowLeftAxis; }
			set
			{
				if(value!=this.m_ShowLeftAxis)
				{
					m_ShowLeftAxis = value;
					this.OnInvalidate();
				}
			}
		}

		public bool BottomAxisEnabled
		{
			get { return this.m_ShowBottomAxis; }
			set
			{
				if(value!=this.m_ShowBottomAxis)
				{
					m_ShowBottomAxis = value;
					this.OnInvalidate();
				}
			}
		}
		public bool RightAxisEnabled
		{
			get { return this.m_ShowRightAxis; }
			set
			{
				if(value!=this.m_ShowRightAxis)
				{
					m_ShowRightAxis = value;
					this.OnInvalidate();
				}
			}
		}
		public bool TopAxisEnabled
		{
			get { return this.m_ShowTopAxis; }
			set
			{
				if(value!=this.m_ShowTopAxis)
				{
					m_ShowTopAxis = value;
					this.OnInvalidate();
				}
			}
		}

		public ExtendedTextGraphObject LeftAxisTitle
		{
			get { return this.m_LeftAxisTitle; }
			set
			{
				this.m_LeftAxisTitle = value;
				this.OnInvalidate();
			}
		}

		public ExtendedTextGraphObject RightAxisTitle
		{
			get { return this.m_RightAxisTitle; }
			set
			{
				this.m_RightAxisTitle = value;
				this.OnInvalidate();
			}
		}

		public ExtendedTextGraphObject TopAxisTitle
		{
			get { return this.m_TopAxisTitle; }
			set
			{
				this.m_TopAxisTitle = value;
				this.OnInvalidate();
			}
		}
		public ExtendedTextGraphObject BottomAxisTitle
		{
			get { return this.m_BottomAxisTitle; }
			set
			{
				this.m_BottomAxisTitle = value;
				this.OnInvalidate();
			}
		}

		public string LeftAxisTitleString
		{
			get { return m_LeftAxisTitle!=null ? m_LeftAxisTitle.Text : null; }
			set
			{
				if(value==null || value=="")
				{
					m_LeftAxisTitle=null;
				}
				else
				{
					if(m_LeftAxisTitle==null)
					{
						m_LeftAxisTitle = new ExtendedTextGraphObject();
						m_LeftAxisTitle.Rotation=-90;
						m_LeftAxisTitle.XAnchor = ExtendedTextGraphObject.XAnchorPositionType.Center;
						m_LeftAxisTitle.YAnchor = ExtendedTextGraphObject.YAnchorPositionType.Bottom;
						m_LeftAxisTitle.Position = new PointF(-0.125f*Size.Width,0.5f*Size.Height);
					}

					m_LeftAxisTitle.Text = value;
				}
			}
		}

		public string RightAxisTitleString
		{
			get { return m_RightAxisTitle!=null ? m_RightAxisTitle.Text : null; }
			set
			{
				if(value==null || value=="")
				{
					m_RightAxisTitle=null;
				}
				else
				{
					if(m_RightAxisTitle==null)
					{
						m_RightAxisTitle = new ExtendedTextGraphObject();
						m_RightAxisTitle.Rotation=-90;
						m_RightAxisTitle.XAnchor = ExtendedTextGraphObject.XAnchorPositionType.Center;
						m_RightAxisTitle.YAnchor = ExtendedTextGraphObject.YAnchorPositionType.Top;
						m_RightAxisTitle.Position = new PointF(1.125f*Size.Width,0.5f*Size.Height);
					}

					m_RightAxisTitle.Text = value;
				}
			}
		}

		public string TopAxisTitleString
		{
			get { return m_TopAxisTitle!=null ? m_TopAxisTitle.Text : null; }
			set
			{
				if(value==null || value=="")
				{
					m_TopAxisTitle=null;
				}
				else
				{
					if(m_TopAxisTitle==null)
					{
						m_TopAxisTitle = new ExtendedTextGraphObject();
						m_TopAxisTitle = new ExtendedTextGraphObject();
						m_TopAxisTitle.Rotation=0;
						m_TopAxisTitle.XAnchor = ExtendedTextGraphObject.XAnchorPositionType.Center;
						m_TopAxisTitle.YAnchor = ExtendedTextGraphObject.YAnchorPositionType.Bottom;
						m_TopAxisTitle.Position = new PointF(0.5f*Size.Width,-0.125f*Size.Height);
					}

					m_TopAxisTitle.Text = value;
				}
			}
		}

		public string BottomAxisTitleString
		{
			get { return m_BottomAxisTitle!=null ? m_BottomAxisTitle.Text : null; }
			set
			{
				if(value==null || value=="")
				{
					m_BottomAxisTitle=null;
				}
				else
				{
					if(m_BottomAxisTitle==null)
					{
						m_BottomAxisTitle = new ExtendedTextGraphObject();
						m_BottomAxisTitle.Rotation=0;
						m_BottomAxisTitle.XAnchor = ExtendedTextGraphObject.XAnchorPositionType.Center;
						m_BottomAxisTitle.YAnchor = ExtendedTextGraphObject.YAnchorPositionType.Top;
						m_BottomAxisTitle.Position = new PointF(0.5f*Size.Width,1.125f*Size.Height);
					}

					m_BottomAxisTitle.Text = value;
				}
			}
		}


		#endregion // Style properties

		#region Painting and Hit testing
		public virtual void Paint(Graphics g)
		{
			GraphicsState savedgstate = g.Save();
			//g.TranslateTransform(m_LayerPosition.X,m_LayerPosition.Y);
			//g.RotateTransform(m_LayerAngle);
			
			g.MultiplyTransform(m_ForwardMatrix);

			if(m_bFillLayerArea)
				g.FillRectangle(m_LayerAreaFillBrush,0,0,m_LayerSize.Width,m_LayerSize.Height);

			m_GraphObjects.DrawObjects(g,1,this);

			RectangleF layerBounds = new RectangleF(m_LayerPosition,m_LayerSize);

			if(m_ShowLeftAxis)
				m_LeftAxisStyle.Paint(g,this,this.m_yAxis);
			if(m_ShowLeftAxis)
				m_LeftLabelStyle.Paint(g,this,this.m_yAxis,m_LeftAxisStyle);
			if(m_ShowLeftAxis && null!=m_LeftAxisTitle)
				m_LeftAxisTitle.Paint(g,this);
			if(m_ShowBottomAxis)
				m_BottomAxisStyle.Paint(g,this,this.m_xAxis);
			if(m_ShowBottomAxis)
				m_BottomLabelStyle.Paint(g,this,this.m_xAxis,m_BottomAxisStyle);
			if(m_ShowBottomAxis && null!=m_BottomAxisTitle)
				m_BottomAxisTitle.Paint(g,this);
			if(m_ShowRightAxis)
				m_RightAxisStyle.Paint(g,this,this.m_yAxis);
			if(m_ShowRightAxis)
				m_RightLabelStyle.Paint(g,this,this.m_yAxis,m_RightAxisStyle);
			if(m_ShowRightAxis && null!=m_RightAxisTitle)
				m_RightAxisTitle.Paint(g,this);
			if(m_ShowTopAxis)
				m_TopAxisStyle.Paint(g,this,this.m_xAxis);
			if(m_ShowTopAxis)
				m_TopLabelStyle.Paint(g,this,this.m_xAxis,m_TopAxisStyle);
			if(m_ShowTopAxis && null!=m_TopAxisTitle)
				m_TopAxisTitle.Paint(g,this);
			if(m_Legend!=null)
				m_Legend.Paint(g,this);

			foreach(PlotItem pi in m_PlotItems)
			{
				pi.Paint(g,this);
			}


			g.Restore(savedgstate);
		}
		public GraphObject HitTest(PointF pageC, out GraphicsPath gp)
		{
			PointF layerC = GraphToLayerCoordinates(pageC);


			GraphObject[] specObjects = 
					{
						m_LeftAxisTitle,
						m_BottomAxisTitle,
						m_TopAxisTitle,
						m_RightAxisTitle,
						m_Legend
					};


			// do the hit test first for the special objects of the layer
			foreach(GraphObject go in specObjects)
			{
				if(null!=go)
				{
					gp = go.HitTest(layerC);
					if(null!=gp)
					{
						gp.Transform(m_ForwardMatrix);
						return go;
					}
				}
			}



			// now hit testing the other objects in the layer
			foreach(GraphObject go in m_GraphObjects)
			{
				gp = go.HitTest(layerC);
				if(null!=gp)
				{
					gp.Transform(m_ForwardMatrix);
					return go;
				}
			}
			gp=null;
			return null;
		}


		#endregion // Painting and Hit testing

		#region Event firing


		protected void OnSizeChanged()
		{
			if(null!=SizeChanged)
				SizeChanged(this,new System.EventArgs());
		}


		protected void OnPositionChanged()
		{
			if(null!=PositionChanged)
				PositionChanged(this,new System.EventArgs());
		}

		protected virtual void OnAxesChanged()
		{
			if(null!=AxesChanged)
				AxesChanged(this,new System.EventArgs());
		}

		protected void OnInvalidate()
		{
			if(null!=this.m_ParentLayerCollection)
				this.m_ParentLayerCollection.OnInvalidate(this);
		}

		#endregion

		#region Handler of child events

		/// <summary>
		/// This handler is called if a x-boundary from any of the plotassociations of this layer
		/// has changed. We then have to recalculate the boundaries.
		/// </summary>
		/// <param name="sender">The plotassociation that has caused the boundary changed event.</param>
		/// <param name="e">The boundary changed event args.</param>
		/// <remarks>Unfortunately we do not know if the boundary is extended or shrinked, if is is extended
		/// if would be possible to merge only the changed boundary into the x-axis boundary.
		/// But since we don't know about that, we have to completely recalculate the boundary be using the boundaries of
		/// all PlotAssociations of this layer.</remarks>
		protected void OnPlotAssociationXBoundariesChanged(object sender, BoundariesChangedEventArgs e)
		{
			if(0==m_PlotAssociationXBoundariesChanged_EventSuspendCount)
			{
				// now we have to inform all the PlotAssociations that a new axis was loaded
				m_xAxis.DataBounds.EventsEnabled=false;
				m_xAxis.DataBounds.Reset();
				foreach(PlotItem pa in this.PlotItems)
				{
					if(pa.Data is Graph.IXBoundsHolder)
					{
						// merge the bounds with x and yAxis
						((IXBoundsHolder)pa.Data).MergeXBoundsInto(m_xAxis.DataBounds); // merge all x-boundaries in the x-axis boundary object
					}
				}
				m_xAxis.DataBounds.EventsEnabled=true;
			}
		}

		/// <summary>
		/// This handler is called if a y-boundary from any of the plotassociations of this layer
		/// has changed. We then have to recalculate the boundaries.
		/// </summary>
		/// <param name="sender">The plotassociation that has caused the boundary changed event.</param>
		/// <param name="e">The boundary changed event args.</param>
		/// <remarks>Unfortunately we do not know if the boundary is extended or shrinked, if is is extended
		/// if would be possible to merge only the changed boundary into the y-axis boundary.
		/// But since we don't know about that, we have to completely recalculate the boundary be using the boundaries of
		/// all PlotAssociations of this layer.</remarks>
		protected void OnPlotAssociationYBoundariesChanged(object sender, BoundariesChangedEventArgs e)
		{
			if(0==m_PlotAssociationYBoundariesChanged_EventSuspendCount)
			{
				// now we have to inform all the PlotAssociations that a new axis was loaded
				m_yAxis.DataBounds.EventsEnabled=false;
				m_yAxis.DataBounds.Reset();
				foreach(PlotItem pa in this.PlotItems)
				{
					if(pa.Data is Graph.IYBoundsHolder)
					{
						// merge the bounds with x and yAxis
						((IYBoundsHolder)pa.Data).MergeYBoundsInto(m_yAxis.DataBounds); // merge all x-boundaries in the x-axis boundary object
				
					}
				}
				m_yAxis.DataBounds.EventsEnabled=true;
			}
		}
		
		protected virtual void OnXAxisChangedEventHandler(object sender, System.EventArgs e)
		{
			// inform linked layers
			if(null!=AxesChanged)
				AxesChanged(this,new EventArgs());
			
			// renew the picture
			OnInvalidate();
		}

		protected virtual void OnYAxisChangedEventHandler(object sender, System.EventArgs e)
		{
			// inform linked layers 
			if(null!=AxesChanged)
				AxesChanged(this,new EventArgs());

			// renew the picture
			OnInvalidate();
		}

		protected virtual void OnChildChangedEventHandler(object sender, System.EventArgs e)
		{
			OnInvalidate();
		}
		
		#endregion

		#region Inner classes


		[SerializationSurrogate(0,typeof(PlotList.SerializationSurrogate0))]
			[SerializationVersion(0)]
			public class PlotList : Altaxo.Data.CollectionBase, System.Runtime.Serialization.IDeserializationCallback, IChangedEventSource, IChildChangedEventSink, System.ICloneable
		{
			/// <summary>The parent layer of this list.</summary>
			private Layer m_Owner; 


			#region Serialization
			/// <summary>Used to serialize the PlotList Version 0.</summary>
			public class SerializationSurrogate0 : System.Runtime.Serialization.ISerializationSurrogate
			{
				public object[] m_PlotItems = null; 

				/// <summary>
				/// Serializes PlotList Version 0.
				/// </summary>
				/// <param name="obj">The PlotList to serialize.</param>
				/// <param name="info">The serialization info.</param>
				/// <param name="context">The streaming context.</param>
				public void GetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context	)
				{
					PlotList s = (PlotList)obj;
					info.AddValue("Data",s.myList);
				}

				/// <summary>
				/// Deserializes the PlotList Version 0.
				/// </summary>
				/// <param name="obj">The empty PlotList object to deserialize into.</param>
				/// <param name="info">The serialization info.</param>
				/// <param name="context">The streaming context.</param>
				/// <param name="selector">The deserialization surrogate selector.</param>
				/// <returns>The deserialized PlotList.</returns>
				public object SetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context,System.Runtime.Serialization.ISurrogateSelector selector)
				{
					PlotList s = (PlotList)obj;

					s.myList = (System.Collections.ArrayList)info.GetValue("Data",typeof(System.Collections.ArrayList));
					return s;
				}
			}

			/// <summary>
			/// Finale measures after deserialization.
			/// </summary>
			/// <param name="obj">Not used.</param>
			public virtual void OnDeserialization(object obj)
			{
				// restore the event chain
				for(int i=0;i<Count;i++)
					WireItem(this[i]);
			}
				
			#endregion



			public PlotList(Layer owner)
			{
				m_Owner = owner;
			}

			/// <summary>
			/// Copy constructor. Clones (!) all items. The parent owner is set to null and has to be set afterwards.
			/// </summary>
			/// <param name="from">The PlotList to clone this list from.</param>
			public PlotList(PlotList from)
				:
				this(null,from)
			{
			}

			/// <summary>
			/// Copy constructor. Clones (!) all the items in the list.
			/// </summary>
			/// <param name="owner">The new owner of the cloned list.</param>
			/// <param name="from">The list to clone all items from.</param>
			public PlotList(Layer owner, PlotList from)
			{
				m_Owner = owner;

				// Clone all the items in the list.
				for(int i=0;i<from.Count;i++)
					Add((PlotItem)from[i].Clone()); // clone the items
			}

			public object Clone()
			{
				return new PlotList(this);
			}

			public Layer ParentLayer
			{
				get { return m_Owner; }
				set
				{
					if(null==value)
						throw new ArgumentNullException();
					else
					{
						m_Owner = value;
						
						// if the owner changed, it has possibly other x and y axis boundaries, so we have to set the plot items to this new boundaries
						for(int i=0;i<Count;i++)
							SetItemBoundaries(this[i]);
					}
				}
			}


			/// <summary>
			/// Restores the event chain of a item.
			/// </summary>
			/// <param name="plotitem">The plotitem for which the event chain should be restored.</param>
			public void WireItem(Graph.PlotItem plotitem)
			{
				plotitem.Changed += new EventHandler(this.OnChildChanged);
				SetItemBoundaries(plotitem);
			}

			/// <summary>
			/// This sets the type of the item boundaries to the type of the owner layer
			/// </summary>
			/// <param name="plotitem">The plot item for which the boundary type should be set.</param>
			public void SetItemBoundaries(Graph.PlotItem plotitem)
			{
				if(plotitem.Data is Graph.IXBoundsHolder)
				{
					IXBoundsHolder pa = (IXBoundsHolder)plotitem.Data;
					pa.SetXBoundsFromTemplate(m_Owner.XAxis.DataBounds); // ensure that data bound object is of the right type
					pa.XBoundariesChanged += new PhysicalBoundaries.BoundaryChangedHandler(m_Owner.OnPlotAssociationXBoundariesChanged);
					pa.MergeXBoundsInto(m_Owner.XAxis.DataBounds); // merge all x-boundaries in the x-axis boundary object
				}
				if(plotitem.Data is Graph.IYBoundsHolder)
				{
					IYBoundsHolder pa = (IYBoundsHolder)plotitem.Data;
					pa.SetYBoundsFromTemplate(m_Owner.YAxis.DataBounds); // ensure that data bound object is of the right type
					pa.YBoundariesChanged += new PhysicalBoundaries.BoundaryChangedHandler(m_Owner.OnPlotAssociationYBoundariesChanged);
					pa.MergeYBoundsInto(m_Owner.YAxis.DataBounds); // merge the y-boundaries in the y-Axis data boundaries
				}
			}

			public void Add(Graph.PlotItem plotitem)
			{
				if(plotitem==null)
					throw new ArgumentNullException();

				base.InnerList.Add(plotitem);
				WireItem(plotitem);
				OnChanged();
			}

			public PlotItem this[int i]
			{
				get { return (PlotItem)base.InnerList[i]; }
			}
			
			public int IndexOf(PlotItem it)
			{
				return base.InnerList.IndexOf(it,0,Count);
			}
				
			#region IChangedEventSource Members

			public event System.EventHandler Changed;


			public virtual void OnChildChanged(object child, EventArgs e)
			{
				if(null!=Changed)
					Changed(this,e);
			}

			protected virtual void OnChanged()
			{
				if(null!=Changed)
					Changed(this,new ChangedEventArgs(this,null));
			}

			#endregion
		}



		/// <summary>
		/// Holds a bunch of layers by it's index.
		/// </summary>
		/// <remarks>The <see cref="GraphDocument"/> inherits from this class, but implements
		/// its own function for adding the layers and moving them, since it has to track
		/// all changes to the layers.</remarks>
		[SerializationSurrogate(0,typeof(LayerCollection.SerializationSurrogate0))]
			[SerializationVersion(0)]
			public class LayerCollection : Altaxo.Data.CollectionBase, System.Runtime.Serialization.IDeserializationCallback, IChangedEventSource, System.ICloneable
		{
			/// <summary>Fired when something in this collection changed, as for instance
			/// adding or deleting layers, or exchanging layers.</summary>
			public event System.EventHandler LayerCollectionChanged;
			

			/// <summary>
			/// Fired if either the layer collection changed or something in the layers changed
			/// </summary>
			public event System.EventHandler Changed;
		
			#region "Serialization"

			/// <summary>Used to serialize the LayerCollection Version 0.</summary>
			public class SerializationSurrogate0 : System.Runtime.Serialization.ISerializationSurrogate
			{
				/// <summary>
				/// Serializes LayerCollection Version 0.
				/// </summary>
				/// <param name="obj">The LayerCollection to serialize.</param>
				/// <param name="info">The serialization info.</param>
				/// <param name="context">The streaming context.</param>
				public void GetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context	)
				{
					LayerCollection s = (LayerCollection)obj;
					info.AddValue("Data",s.myList);
				}

				/// <summary>
				/// Deserializes the LayerCollection Version 0.
				/// </summary>
				/// <param name="obj">The empty GraphDocument object to deserialize into.</param>
				/// <param name="info">The serialization info.</param>
				/// <param name="context">The streaming context.</param>
				/// <param name="selector">The deserialization surrogate selector.</param>
				/// <returns>The deserialized GraphDocument.</returns>
				public object SetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context,System.Runtime.Serialization.ISurrogateSelector selector)
				{
					LayerCollection s = (LayerCollection)obj;

					s.myList = (System.Collections.ArrayList)info.GetValue("Data",typeof(System.Collections.ArrayList));
					return s;
				}
			}

			/// <summary>
			/// Finale measures after deserialization.
			/// </summary>
			/// <param name="obj">Not used.</param>
			public virtual void OnDeserialization(object obj)
			{
				// set the parent and the number of all items
				for(int i=0;i<base.InnerList.Count;i++)
					this[i].SetParentAndNumber(this,i);
			}
			#endregion



			/// <summary>
			/// Creates an empty LayerCollection without parent.
			/// </summary>
			public LayerCollection()
			{
			}

			/// <summary>
			/// Copy constructor. Clones all objects in this collection.
			/// </summary>
			/// <param name="from">The collection to clone from.</param>
			public LayerCollection(LayerCollection from)
			{
				for(int i=0;i<from.Count;i++)
					this.Add((Layer)from[i].Clone());
	
				// now we have to fix the linked layer list, since the LinkedLayer property of the Layers point to the original layers
				// and not to the cloned layers!
				for(int i=0;i<Count;i++)
				{
					if(null!=from[i].LinkedLayer)
					{
						this[i].LinkedLayer = this[from[i].LinkedLayer.Number];
					}
				}
			}
		
			public virtual object Clone()
			{
				return new LayerCollection(this);
			}

			/// <summary>
			/// References the layer at index i.
			/// </summary>
			/// <value>The layer at index <paramref name="i"/>.</value>
			public virtual Layer this[int i]
			{
				get 
				{
					// for the getter, we can use the innerlist, since no actions are defined for that
					return (Layer)base.InnerList[i];
				}
				set
				{
					// we use List here since we want to have custom actions defined below
					List[i] = value;
				}
			}

			/// <summary>
			/// This will exchange layer i and layer j.
			/// </summary>
			/// <param name="i">Index of the one element to exchange.</param>
			/// <param name="j">Index of the other element to exchange.</param>
			/// <remarks>To avoid the destruction of the linked layer connections, we avoid
			/// firing the custom list actions here by using the InnerList property and
			/// correct the layer numbers of the two exchanged elements directly.</remarks>
			public virtual void ExchangeElements(int i, int j)
			{
				// we use the inner list to do that because we do not want
				// to have custom actions (this is mainly because otherwise we have
				// a remove action that will destoy the linked layer connections

				object o = base.InnerList[i];
				base.InnerList[i] = base.InnerList[j];
				base.InnerList[j] = o;

				// correct the Layer numbers for the two exchanged layers
				this[i].SetParentAndNumber(this,i);
				this[j].SetParentAndNumber(this,j);

				OnLayerCollectionChanged();
			}


			/// <summary>
			/// Adds a layer to this layer collection.
			/// </summary>
			/// <param name="l"></param>
			public void Add(Layer l)
			{
				// we use List for adding since we want to have custom actions below
				List.Add(l);
				// since we use List, we don't need to have OnLayerCollectionChanged here!
			}

			/// <summary>
			/// Perform custom action on clearing: remove the parent attribute and the layer number from all the layers.
			/// </summary>
			protected override void OnClear()
			{
				foreach(Layer l in InnerList)
					l.SetParentAndNumber(null,0);

				OnLayerCollectionChanged();
			}

			/// <summary>
			/// Perform custom action if one element removed: renumber the remaining elements.
			/// </summary>
			/// <param name="idx">The index where the element was removed. </param>
			/// <param name="oldValue">The removed element.</param>
			protected override void OnRemoveComplete(int idx, object oldValue)
			{
				((Layer)oldValue).SetParentAndNumber(null,0);

				// renumber the layers from i to count
				for(int i=idx;i<Count;i++)
				{
					this[i].SetParentAndNumber(this,i);

					// fix linked layer connections if neccessary
					if(Layer.ReferenceEquals(oldValue,this[i]))
						this[i].LinkedLayer=null;
				}
				OnLayerCollectionChanged();
			}

			/// <summary>
			/// Perform custom action if one element is set: set parent and number of the newly
			/// set element.
			/// </summary>
			/// <param name="index">The index where the element is set.</param>
			/// <param name="oldValue">The old value of the list element.</param>
			/// <param name="newValue">The new value this list element is set to.</param>
			protected override void OnSetComplete(int index, object oldValue,	object newValue	)
			{
				((Layer)oldValue).SetParentAndNumber(null,0);
				((Layer)newValue).SetParentAndNumber(this,index);


				for(int i=0;i<Count;i++)
				{
					// fix linked layer connections if neccessary
					if(Layer.ReferenceEquals(oldValue,this[i]))
						this[i].LinkedLayer=null;
				}

				OnLayerCollectionChanged();
			}

			/// <summary>
			/// Perform custom action if an element is inserted: set parent and number
			/// of the inserted element and renumber the other elements.
			/// </summary>
			/// <param name="index"></param>
			/// <param name="newValue"></param>
			protected override void OnInsertComplete(int index,object newValue)
			{
				// renumber the inserted and the following layers
				for(int i=index;i<Count;i++)
					this[i].SetParentAndNumber(this,i);

				OnLayerCollectionChanged();
			}


			protected virtual void OnLayerCollectionChanged()
			{
				if(null!=LayerCollectionChanged)
					LayerCollectionChanged(this,new EventArgs());
			
				OnChanged();
			}

			protected internal virtual void OnInvalidate(Layer sender)
			{
				OnChanged();
			}

			protected virtual void OnChanged()
			{
				if(null!=Changed)
					Changed(this, new EventArgs());
			}
		}
	
	
		#endregion // Inner classes
	}
}
