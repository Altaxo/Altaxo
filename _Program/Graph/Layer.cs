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


namespace Altaxo.Graph
{
	/// <summary>
	/// Summary description for GraphLayer.
	/// </summary>
	public class Layer
	{
		#region Enumerations

		/// <summary>
		/// The type of the size (i.e. width and height values.
		/// </summary>
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

		#region Member variables

		/// <summary>
		/// The cached layer position in points (1/72 inch) relative to the upper left corner
		/// of the graph document (upper left corner of the printable area).
		/// </summary>
		protected PointF m_LayerPosition = new PointF(119,80);

		/// <summary>
		/// The layers x position value, either absolute or relative, as determined by <see cref="m_LayerXPositionType"/>.
		/// </summary>
		protected double m_LayerXPosition = 119;

		/// <summary>
		/// The type of the x position value, see <see cref="PositionType"/>.
		/// </summary>
		protected PositionType m_LayerXPositionType=PositionType.AbsoluteValue;

		/// <summary>
		/// The layers y position value, either absolute or relative, as determined by <see cref="m_LayerYPositionType"/>.
		/// </summary>
		protected double m_LayerYPosition = 80;

		/// <summary>
		/// The type of the y position value, see <see cref="PositionType"/>.
		/// </summary>
		protected PositionType m_LayerYPositionType=PositionType.AbsoluteValue;

		/// <summary>
		/// The size of the layer in points (1/72 inch).
		/// </summary>
		/// <remarks>
		/// In case the size is absolute (see <see cref="SizeType"/>), this is the size of the layer. Otherwise
		/// it is only the cached value for the size, since the size is calculated then.
		/// </remarks>
		protected SizeF  m_LayerSize = new SizeF(626,407);

		/// <summary>
		/// The width of the layer, either as absolute value in point (1/72 inch), or as 
		/// relative value as pointed out by <see cref="m_LayerWidthType"/>.
		/// </summary>
		protected double m_LayerWidth=626;

		/// <summary>
		/// The type of the value for the layer width, see <see cref="SizeType"/>.
		/// </summary>
		protected SizeType m_LayerWidthType=SizeType.AbsoluteValue;

		/// <summary>
		/// The height of the layer, either as absolute value in point (1/72 inch), or as 
		/// relative value as pointed out by <see cref="m_LayerHeightType"/>.
		/// </summary>
		protected double m_LayerHeight= 407;

		/// <summary>
		/// The type of the value for the layer height, see <see cref="SizeType"/>.
		/// </summary>
		protected SizeType m_LayerHeightType=SizeType.AbsoluteValue;

		/// <summary>
		/// The rotation angle (in degrees) of the layer.
		/// </summary>
		protected float  m_LayerAngle=0; // Rotation
		protected float  m_LayerScale=1;  // Scale
		protected Matrix matrix = new Matrix();  // forward transformation matrix
		protected Matrix matrixi = new Matrix(); // inverse transformation matrix

		protected Axis m_xAxis; // the X-Axis
		protected Axis m_yAxis; // the Y-Axis


		protected bool m_ShowLeftAxis = true;
		protected bool m_ShowBottomAxis = true;
		protected bool m_ShowRightAxis = true;
		protected bool m_ShowTopAxis = true;

		protected bool m_bFillLayerArea=false;
		protected BrushHolder m_LayerAreaFillBrush = new BrushHolder(Color.Aqua);

		protected XYLayerAxisStyle m_LeftAxisStyle = new XYLayerAxisStyle(XYLayerAxisStyle.EdgeType.Left);
		protected XYLayerAxisStyle m_BottomAxisStyle = new XYLayerAxisStyle(XYLayerAxisStyle.EdgeType.Bottom);
		protected XYLayerAxisStyle m_RightAxisStyle = new XYLayerAxisStyle(XYLayerAxisStyle.EdgeType.Right);
		protected XYLayerAxisStyle m_TopAxisStyle = new XYLayerAxisStyle(XYLayerAxisStyle.EdgeType.Top);

		protected SimpleLabelStyle m_LeftLabelStyle = new SimpleLabelStyle(LayerEdge.EdgeType.Left);
		protected SimpleLabelStyle m_BottomLabelStyle = new SimpleLabelStyle(LayerEdge.EdgeType.Bottom);
		protected SimpleLabelStyle m_RightLabelStyle = new SimpleLabelStyle(LayerEdge.EdgeType.Right);
		protected SimpleLabelStyle m_TopLabelStyle = new SimpleLabelStyle(LayerEdge.EdgeType.Top);

		protected ExtendedTextGraphObject m_LeftAxisTitle = null;
		protected ExtendedTextGraphObject m_BottomAxisTitle = null;
		protected ExtendedTextGraphObject m_RightAxisTitle = null;
		protected ExtendedTextGraphObject m_TopAxisTitle = null;

		protected GraphObjectCollection m_GraphObjects = new GraphObjectCollection();

		protected PlotAssociationList m_PlotAssociations;
		protected PlotGroup.Collection m_PlotGroups = new PlotGroup.Collection();

		/// <summary>
		/// The parent layer collection wich contains this layer (or null if not member of such collection).
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

		#endregion

		#region Event definitions

		/// <summary>Fired when the size of the layer changed.</summary>
		public event System.EventHandler SizeChanged;
	
		/// <summary>Fired when the position of the layer changed.</summary>
		public event System.EventHandler PositionChanged;
	
		/// <summary>Fired when one of the axis changed its origin or its end.</summary>
		public event System.EventHandler AxesChanged;

		#endregion

		#region Constructors

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

			m_PlotAssociations = new PlotAssociationList(this);

			// create axes and add event handlers to them
			m_xAxis = new LinearAxis(); // the X-Axis
			m_yAxis = new LinearAxis(); // the Y-Axis

			m_xAxis.AxisChanged += new System.EventHandler(this.OnXAxisChanged);
			m_yAxis.AxisChanged += new System.EventHandler(this.OnYAxisChanged);
		
		
			m_LeftAxisTitle = new ExtendedTextGraphObject();
			m_LeftAxisTitle.Rotation=-90;
			m_LeftAxisTitle.XAnchor = ExtendedTextGraphObject.XAnchorPositionType.Center;
			m_LeftAxisTitle.YAnchor = ExtendedTextGraphObject.YAnchorPositionType.Bottom;
			m_LeftAxisTitle.Text = "Y axis";
			m_LeftAxisTitle.Position = new PointF(-0.125f*size.Width,0.5f*size.Height);

			m_BottomAxisTitle = new ExtendedTextGraphObject();
			m_BottomAxisTitle.Rotation=0;
			m_BottomAxisTitle.XAnchor = ExtendedTextGraphObject.XAnchorPositionType.Center;
			m_BottomAxisTitle.YAnchor = ExtendedTextGraphObject.YAnchorPositionType.Top;
			m_BottomAxisTitle.Text = "X axis";
			m_BottomAxisTitle.Position = new PointF(0.5f*size.Width,1.125f*size.Height);



		}

	
		#endregion

		#region Layer properties and methods

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


		public PlotAssociationList PlotAssociations
		{
			get { return m_PlotAssociations; }
		}

		public PlotGroup.Collection PlotGroups
		{
			get { return m_PlotGroups; }
		}

		public void AddPlotAssociation(PlotAssociation[] pal)
		{
			foreach(PlotAssociation pa in pal)
				this.m_PlotAssociations.Add(pa);
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
			matrix.Reset();
			matrix.Translate(m_LayerPosition.X,m_LayerPosition.Y);
			matrix.Scale(m_LayerScale,m_LayerScale);
			matrix.Rotate(m_LayerAngle);
			matrixi=matrix.Clone();
			matrixi.Invert();
		}


		public PointF GraphToLayerCoordinates(PointF pagecoordinates)
		{
			PointF[] pf = { pagecoordinates };
			matrixi.TransformPoints(pf);
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
			matrixi.TransformPoints(pf);
			return pf[0];
		}



		/// <summary>
		/// Transforms a graphics path from layer coordinates to graph (page) coordinates
		/// </summary>
		/// <param name="gp">the graphics path to convert</param>
		/// <returns>graphics path now in graph coordinates</returns>
		public GraphicsPath LayerToGraphCoordinates(GraphicsPath gp)
		{
			gp.Transform(matrix);
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
					m_xAxis.AxisChanged -= new System.EventHandler(this.OnXAxisChanged);
				
				m_xAxis = value;

				if(null!=m_xAxis)
					m_xAxis.AxisChanged += new System.EventHandler(this.OnXAxisChanged);


				// now we have to inform all the PlotAssociations that a new axis was loaded
				foreach(PlotAssociation pa in this.PlotAssociations)
				{
					// first ensure the right data bound object is set on the PlotAssociation
					pa.SetXBoundsFromTemplate(m_xAxis.DataBounds); // ensure that data bound object is of the right type
					// now merge the bounds with x and yAxis
					pa.MergeXBoundsInto(m_xAxis.DataBounds); // merge all x-boundaries in the x-axis boundary object
				}
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
					m_yAxis.AxisChanged -= new System.EventHandler(this.OnYAxisChanged);
				
				m_yAxis = value;

				if(null!=m_yAxis)
					m_yAxis.AxisChanged += new System.EventHandler(this.OnYAxisChanged);


				// now we have to inform all the PlotAssociations that a new axis was loaded
				foreach(PlotAssociation pa in this.PlotAssociations)
				{
					// first ensure the right data bound object is set on the PlotAssociation
					pa.SetYBoundsFromTemplate(m_yAxis.DataBounds); // ensure that data bound object is of the right type
					// now merge the bounds with x and yAxis
					pa.MergeYBoundsInto(m_yAxis.DataBounds); // merge all x-boundaries in the x-axis boundary object
				}
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
			get { return this.m_LinkXAxis; }
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

		#endregion // Style properties

		#region Painting and Hit testing
		public virtual void Paint(Graphics g)
		{
			GraphicsState savedgstate = g.Save();
			//g.TranslateTransform(m_LayerPosition.X,m_LayerPosition.Y);
			//g.RotateTransform(m_LayerAngle);
			
			g.MultiplyTransform(matrix);

			if(m_bFillLayerArea)
				g.FillRectangle(m_LayerAreaFillBrush,0,0,m_LayerSize.Width,m_LayerSize.Height);

			m_GraphObjects.DrawObjects(g,1,this);

			RectangleF layerBounds = new RectangleF(m_LayerPosition,m_LayerSize);

			if(m_ShowLeftAxis) m_LeftAxisStyle.Paint(g,this,this.m_yAxis);
			if(m_ShowLeftAxis) m_LeftLabelStyle.Paint(g,this,this.m_yAxis,m_LeftAxisStyle);
			if(m_ShowLeftAxis && null!=m_LeftAxisTitle) m_LeftAxisTitle.Paint(g,this);
			if(m_ShowBottomAxis) m_BottomAxisStyle.Paint(g,this,this.m_xAxis);
			if(m_ShowBottomAxis) m_BottomLabelStyle.Paint(g,this,this.m_xAxis,m_BottomAxisStyle);
			if(m_ShowBottomAxis && null!=m_BottomAxisTitle) m_BottomAxisTitle.Paint(g,this);
			if(m_ShowRightAxis) m_RightAxisStyle.Paint(g,this,this.m_yAxis);
			if(m_ShowRightAxis) m_RightLabelStyle.Paint(g,this,this.m_yAxis,m_RightAxisStyle);
			if(m_ShowTopAxis) m_TopAxisStyle.Paint(g,this,this.m_xAxis);
			if(m_ShowTopAxis) m_TopLabelStyle.Paint(g,this,this.m_xAxis,m_TopAxisStyle);


			foreach(PlotAssociation pa in m_PlotAssociations)
			{
				pa.Paint(g,this);
			}


			g.Restore(savedgstate);
		}
		public GraphObject HitTest(PointF pageC, out GraphicsPath gp)
		{
			PointF layerC = GraphToLayerCoordinates(pageC);

			foreach(GraphObject go in m_GraphObjects)
			{
				gp = go.HitTest(layerC);
				if(null!=gp)
				{
					gp.Transform(matrix);
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

		protected void OnPlotAssociationXBoundariesChanged(object sender, BoundariesChangedEventArgs e)
		{
			((PlotAssociation)sender).MergeXBoundsInto(m_xAxis.DataBounds);
		}

		protected void OnPlotAssociationYBoundariesChanged(object sender, BoundariesChangedEventArgs e)
		{
			((PlotAssociation)sender).MergeYBoundsInto(m_yAxis.DataBounds);
		}
		
		protected void OnXAxisChanged(object sender, System.EventArgs e)
		{
			OnInvalidate();
		}

		protected void OnYAxisChanged(object sender, System.EventArgs e)
		{
			OnInvalidate();
		}

		
		#endregion

		#region Inner classes

		public class PlotAssociationList : System.Collections.ArrayList
		{
			private Layer m_Owner; // the parent of this list

			public PlotAssociationList(Layer owner)
			{
				m_Owner = owner;
			}

			public new void Add(object o)
			{
				if(o is PlotAssociation)
				{
					PlotAssociation pa = (PlotAssociation)o;
					pa.SetXBoundsFromTemplate(m_Owner.XAxis.DataBounds); // ensure that data bound object is of the right type
					pa.SetYBoundsFromTemplate(m_Owner.YAxis.DataBounds); // ensure that data bound object is of the right type
					pa.XBoundariesChanged += new PhysicalBoundaries.BoundaryChangedHandler(m_Owner.OnPlotAssociationXBoundariesChanged);
					pa.YBoundariesChanged += new PhysicalBoundaries.BoundaryChangedHandler(m_Owner.OnPlotAssociationYBoundariesChanged);
					base.Add(pa);
					// now merge the bounds with x and yAxis
					pa.MergeXBoundsInto(m_Owner.XAxis.DataBounds); // merge all x-boundaries in the x-axis boundary object
					pa.MergeYBoundsInto(m_Owner.YAxis.DataBounds); // merge the y-boundaries in the y-Axis data boundaries
				}
				else
					throw new ArgumentException("Only PlotAssociations can be added to the list, but you try to add a " + o.GetType());
			}

			public new PlotAssociation this[int i]
			{
				get { return (PlotAssociation)base[i]; }
				set 
				{
					if(null!=base[i])
					{
						// remove the old event handlers
						((PlotAssociation)base[i]).XBoundariesChanged -= new PhysicalBoundaries.BoundaryChangedHandler(m_Owner.OnPlotAssociationXBoundariesChanged);
						((PlotAssociation)base[i]).YBoundariesChanged -= new PhysicalBoundaries.BoundaryChangedHandler(m_Owner.OnPlotAssociationYBoundariesChanged);
					}
					base[i] = value;
					// add event handlers to the new value
					((PlotAssociation)base[i]).XBoundariesChanged += new PhysicalBoundaries.BoundaryChangedHandler(m_Owner.OnPlotAssociationXBoundariesChanged);
					((PlotAssociation)base[i]).YBoundariesChanged += new PhysicalBoundaries.BoundaryChangedHandler(m_Owner.OnPlotAssociationYBoundariesChanged);
				}
			}
		}



		/// <summary>
		/// Holds a bunch of layers by it's index.
		/// </summary>
		/// <remarks>The <see cref="GraphDocument"/> inherits from this class, but implements
		/// its own function for adding the layers and moving them, since it has to track
		/// all changes to the layers.</remarks>
		public class LayerCollection : System.Collections.CollectionBase
		{
			/// <summary>Fired when something in this collection changed, as for instance
			/// adding or deleting layers, or exchanging layers.</summary>
			public event System.EventHandler LayerCollectionChanged;


			/// <summary>
			/// Fired if some of the layer signals that a redraw is neccessary.
			/// </summary>
			public event System.EventHandler Invalidate;


			/// <summary>
			/// Creates an empty LayerCollection without parent.
			/// </summary>
			public LayerCollection()
			{
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
			}

			protected internal virtual void OnInvalidate(Layer sender)
			{
				if(null!=Invalidate)
					Invalidate(this, new EventArgs());
			}
		}
	
	
		#endregion // Inner classes
	}
}
