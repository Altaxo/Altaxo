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

		protected GraphObjectCollection coll = new GraphObjectCollection();

		protected PlotAssociationList plotAssociations;
		protected PlotGroup.Collection m_PlotGroups = new PlotGroup.Collection();
		protected int m_ActualPlotAssociation = 0;

		protected LayerCollection m_ParentLayerCollection=null;

		public Layer(SizeF prtSize)
		{
			m_LayerPosition = new PointF(prtSize.Width*0.14f,prtSize.Height*0.14f);
			m_LayerSize = new SizeF(prtSize.Width*0.76f,prtSize.Height*0.7f);

			ExtendedTextGraphObject tgo = new ExtendedTextGraphObject(129,90,@"\L(0)\b(bbbbbb)aaaaaa",new Font(FontFamily.GenericSansSerif,12,GraphicsUnit.World),Color.Black);
			tgo.Rotation = 0;
			coll.Add(tgo);
			CalculateMatrix();

			plotAssociations = new PlotAssociationList(this);

			// create axes and add event handlers to them
			m_xAxis = new LinearAxis(); // the X-Axis
			m_yAxis = new LinearAxis(); // the Y-Axis

			m_xAxis.AxisChanged += new System.EventHandler(this.OnXAxisChanged);
			m_yAxis.AxisChanged += new System.EventHandler(this.OnYAxisChanged);

		}

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


#endregion // Layer Properties


		protected void SetParentLayerCollection(LayerCollection lc)
		{
			m_ParentLayerCollection = lc;
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


		public PointF ToLayerCoordinates(PointF pagecoordinates)
		{
			PointF[] pf = { pagecoordinates };
			matrixi.TransformPoints(pf);
			return pf[0];
		}

		public GraphicsPath HitTest(PointF pageC)
		{
			PointF layerC = ToLayerCoordinates(pageC);
			GraphicsPath gp;

			foreach(GraphObject go in coll)
			{
				gp = go.HitTest(layerC);
				if(null!=gp)
				{
					gp.Transform(matrix);
					return gp;
				}
			}
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

			coll.DrawObjects(g,1,this);

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
			// !!!todo!!! inform the parent to invalidate the plot 
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



		public class LayerCollection : System.Collections.CollectionBase
		{
			public Layer this[int i]
			{
				get { return (Layer)base.InnerList[i]; }
				set
				{
					value.SetParentLayerCollection(this);
					base.InnerList[i] = value;
				}
			}

			public void Add(Layer l)
			{
				l.SetParentLayerCollection(this);
				base.InnerList.Add(l);
			}
		}

	}

}
