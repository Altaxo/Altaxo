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
		protected PointF m_LayerPosition = new PointF(119,80);
		protected SizeF  m_LayerSize = new SizeF(626,407);
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

		protected GraphObjectCollection m_GraphObjects = new GraphObjectCollection();

		protected PlotAssociationList plotAssociations;
		protected PlotGroup.Collection m_PlotGroups = new PlotGroup.Collection();
		protected int m_ActualPlotAssociation = 0;

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
			m_LayerPosition = position;
			m_LayerSize = size;

			CalculateMatrix();

			plotAssociations = new PlotAssociationList(this);

			// create axes and add event handlers to them
			m_xAxis = new LinearAxis(); // the X-Axis
			m_yAxis = new LinearAxis(); // the Y-Axis

			m_xAxis.AxisChanged += new System.EventHandler(this.OnXAxisChanged);
			m_yAxis.AxisChanged += new System.EventHandler(this.OnYAxisChanged);
		}

		#endregion

		#region "Layer Properties"

		public PointF Position
		{
			get { return this.m_LayerPosition; }
			set
			{
				this.m_LayerPosition = value;
				this.CalculateMatrix();
				this.OnInvalidate();
			}
		}

		public SizeF Size
		{
			get { return this.m_LayerSize; }
			set
			{
				this.m_LayerSize = value;
				this.CalculateMatrix();
				this.OnInvalidate();
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

				Layer oldValue = value;
				m_LinkedLayer = this;

				// make sure there is no circular dependency, if there is any, set
				// the LinkedLayer to null
				// note we trust the program is this way that we believe there is no
				// circular dependency already created
				Layer searchLayer = m_LinkedLayer;
				while(null!=searchLayer)
				{
					if(Layer.ReferenceEquals(searchLayer,this))
					{
						// this means a circular dependency, so set the linked layer to null
						m_LinkedLayer=null;
						break;
					}
					searchLayer = searchLayer.LinkedLayer;
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


#endregion // Layer Properties


		/// <summary>
		///  Only indended to use by LayerCollection! Sets the parent layer collection for this layer.
		/// </summary>
		/// <param name="lc">The layer collection this layer belongs to.</param>
		protected void SetParentAndNumber(LayerCollection lc, int number)
		{
			m_ParentLayerCollection = lc;
			m_LayerNumber = number;
			
			if(m_ParentLayerCollection==null)
				m_LinkedLayer=null;
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

		public PlotAssociationList PlotAssociations
		{
			get { return plotAssociations; }
		}

		public PlotGroup.Collection PlotGroups
		{
			get { return m_PlotGroups; }
		}

		public int ActualPlotAssociation 
		{
			get 
			{
				if(m_ActualPlotAssociation>=plotAssociations.Count)
					m_ActualPlotAssociation = 0;
					
				return m_ActualPlotAssociation;
			}
			set
			{
				if(value<0)
					throw new ArgumentOutOfRangeException("ActualPlotAssociation",value,"Must be greater or equal than zero");
				if(value>=plotAssociations.Count)
					throw new ArgumentOutOfRangeException("ActualPlotAssociation",value,"Must be lesser than actual count: " + plotAssociations.Count.ToString());

				m_ActualPlotAssociation = value;
			}
		}


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
			if(m_ShowBottomAxis) m_BottomAxisStyle.Paint(g,this,this.m_xAxis);
			if(m_ShowBottomAxis) m_BottomLabelStyle.Paint(g,this,this.m_xAxis,m_BottomAxisStyle);
			if(m_ShowRightAxis) m_RightAxisStyle.Paint(g,this,this.m_yAxis);
			if(m_ShowRightAxis) m_RightLabelStyle.Paint(g,this,this.m_yAxis,m_RightAxisStyle);
			if(m_ShowTopAxis) m_TopAxisStyle.Paint(g,this,this.m_xAxis);
			if(m_ShowTopAxis) m_TopLabelStyle.Paint(g,this,this.m_xAxis,m_TopAxisStyle);


			foreach(PlotAssociation pa in plotAssociations)
			{
				pa.Paint(g,this);
			}


			g.Restore(savedgstate);
		}

	
		public void AddPlotAssociation(PlotAssociation[] pal)
		{
			foreach(PlotAssociation pa in pal)
				this.plotAssociations.Add(pa);
		}
	






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


		protected void OnInvalidate()
		{
			if(null!=this.m_ParentLayerCollection)
				this.m_ParentLayerCollection.OnInvalidate(this);
		}


		#region "Layer Event Handlers"

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



		/// <summary>
		/// Holds a bunch of layers by it's index.
		/// </summary>
		/// <remarks>The <see cref="GraphDocument"/> inherits from this class, but implements
		/// its own function for adding the layers and moving them, since it has to track
		/// all changes to the layers.</remarks>
		public class LayerCollection : System.Collections.CollectionBase
		{

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
			public Layer this[int i]
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
			public void ExchangeElements(int i, int j)
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
			}


			/// <summary>
			/// Adds a layer to this layer collection.
			/// </summary>
			/// <param name="l"></param>
			public void Add(Layer l)
			{
				// we use List for adding since we want to have custom actions below
				List.Add(l);
			}

			/// <summary>
			/// Perform custom action on clearing: remove the parent attribute and the layer number from all the layers.
			/// </summary>
			protected override void OnClear()
			{
				foreach(Layer l in InnerList)
					l.SetParentAndNumber(null,0);
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
			}


			protected internal virtual void OnInvalidate(Layer sender)
			{
			}
		}
	}
}
