using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Altaxo.Graph
{

	/// <summary>
	/// Summary description for AxisStyle.
	/// </summary>
	public class XYLayerAxisStyle : LayerEdge
	{
		protected PenHolder m_AxisPen = new PenHolder(Color.Black,1);
		protected PenHolder m_MajorTickPen =  new PenHolder(Color.Black,1);
		protected PenHolder m_MinorTickPen =  new PenHolder(Color.Black,1);
		protected float m_MajorTickLength = 12;
		protected float m_MinorTickLength = 8;
		protected bool  m_bOuterMajorTicks=true; // true if outer major ticks should be visible
		protected bool  m_bInnerMajorTicks=true; // true if inner major ticks should be visible
		protected bool  m_bOuterMinorTicks=true; // true if outer minor ticks should be visible
		protected bool  m_bInnerMinorTicks=true; // true if inner minor ticks should be visible
		protected Calc.RelativeOrAbsoluteValue m_AxisPosition; // if relative, then relative to layer size, if absolute then in points


		public XYLayerAxisStyle(EdgeType st)
			: base(st)
		{
		}

		/// <summary>
		/// OuterDistance returns the used space from the middle line of the axis
		/// to the last outer object (either the outer major thicks or half
		/// of the axis thickness)
		/// </summary>
		public float OuterDistance
		{
			get
			{
				float retVal = m_AxisPen.Width/2; // half of the axis thickness
				retVal = System.Math.Max(retVal, m_bOuterMajorTicks ? m_MajorTickLength:0);
				retVal = System.Math.Max(retVal, m_bOuterMinorTicks ? m_MinorTickLength:0);
				return retVal;
			}
		}

		/// <summary>
		/// GetOffset returns the distance of the axis to the layer edge in points
		/// in most cases, the axis position is exactly onto the layer edge and offset is zero,
		/// if the axis is outside the layer, offset is a positive value, 
		/// if the axis is shifted inside the layer, offset is negative 
		/// </summary>
		public float GetOffset(SizeF layerSize)
		{
		return (float)m_AxisPosition.GetValueRelativeTo(this.GetOppositeEdgeLength(layerSize));
		}
		
		
		public float MajorTickLength
		{
			get { return this.m_MajorTickLength; }
			set { m_MajorTickLength = value; }
		}

		public float MinorTickLength
		{
			get { return this.m_MinorTickLength; }
			set { m_MinorTickLength = value; }
		}

		public bool OuterMajorTicks
		{
			get { return this.m_bOuterMajorTicks; }
			set { this.m_bOuterMajorTicks = value; }
		}

		public bool InnerMajorTicks
		{
			get { return this.m_bInnerMajorTicks; }
			set { this.m_bInnerMajorTicks = value; }
		}

		public bool OuterMinorTicks
		{
			get { return this.m_bOuterMinorTicks; }
			set { this.m_bOuterMinorTicks = value; }
		}


		public bool InnerMinorTicks
		{
			get { return this.m_bInnerMinorTicks; }
			set { this.m_bInnerMinorTicks = value; }
		}

		public float Thickness
		{
			get { return this.m_AxisPen.Width; }
			set
			{ 
				this.m_AxisPen.Width = value;
				this.m_MajorTickPen.Width = value;
				this.m_MinorTickPen.Width = value;
			}
		}

		public Color Color
		{
			get { return this.m_AxisPen.Color; }
			set
			{
				this.m_AxisPen.Color = value;
				this.m_MajorTickPen.Color = value;
				this.m_MinorTickPen.Color = value;
			}
		}

		public Calc.RelativeOrAbsoluteValue Position
		{
			get { return this.m_AxisPosition; }
			set	{	m_AxisPosition = value;		}
		}


		public void Paint(Graphics g, Layer layer, Axis axis)
		{
			SizeF layerSize = layer.layerSize;


			PointF orgP = GetOrg(layerSize);
			PointF endP = GetEnd(layerSize);
			PointF outVector = OuterVector;
			PointF offset = OuterVector;
			float foffset = this.GetOffset(layerSize);
			offset.X *= foffset;
			offset.Y *= foffset;
			
			orgP.X += offset.X; orgP.Y += offset.Y;
			endP.X += offset.X; endP.Y += offset.Y;

			g.DrawLine(m_AxisPen,orgP,endP);

			// now the major ticks
			double[] majorticks = axis.GetMajorTicks();
			for(int i=0;i<majorticks.Length;i++)
			{
				double r = axis.PhysicalToNormal(majorticks[i]);
				PointF tickorg = GetPointBetween(orgP,endP,r);
				PointF tickend;
				if(m_bOuterMajorTicks)
				{
					tickend	= tickorg;
					tickend.X += outVector.X * m_MajorTickLength;
					tickend.Y += outVector.Y * m_MajorTickLength;
					g.DrawLine(m_MajorTickPen,tickorg,tickend);
				}
				if(m_bInnerMajorTicks)
				{
					tickend	= tickorg;
					tickend.X -= outVector.X * m_MajorTickLength;
					tickend.Y -= outVector.Y * m_MajorTickLength;
					g.DrawLine(m_MajorTickPen,tickorg,tickend);
				}
			}
			// now the major ticks
			double[] minorticks = axis.GetMinorTicks();
			for(int i=0;i<minorticks.Length;i++)
			{
				double r = axis.PhysicalToNormal(minorticks[i]);
				PointF tickorg =  GetPointBetween(orgP,endP,r);
				PointF tickend;
				if(m_bOuterMinorTicks)
				{
					tickend	= tickorg;
					tickend.X += outVector.X * m_MinorTickLength;
					tickend.Y += outVector.Y * m_MinorTickLength;
					g.DrawLine(m_MinorTickPen,tickorg,tickend);
				}
				if(m_bInnerMinorTicks)
				{
					tickend	= tickorg;
					tickend.X -= outVector.X * m_MinorTickLength;
					tickend.Y -= outVector.Y * m_MinorTickLength;
					g.DrawLine(m_MinorTickPen,tickorg,tickend);
				}
			}
		}
	}
}
