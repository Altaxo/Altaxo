using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Altaxo.Graph
{
	public enum EdgeType { Bottom, Left, Top, Right } ;

	/// <summary>
	/// LayerEdge provides some common functions that apply to one of the
	/// edges of a rectangular layer (left, right, bottom, top).
	/// </summary>
	public class LayerEdge
	{
		public enum EdgeType { Left, Right, Bottom, Top }
		protected EdgeType m_StyleType = EdgeType.Bottom;

		public LayerEdge(EdgeType st)
		{
			m_StyleType = st;
		}

		public PointF GetOrg(SizeF layerSize)
		{
			switch(m_StyleType)
			{
				case EdgeType.Left:
					return new PointF(0,layerSize.Height);
					
				case EdgeType.Right:
					return new PointF(layerSize.Width,layerSize.Height);
					
				case EdgeType.Bottom:
					return new PointF(0,layerSize.Height);
					
				case EdgeType.Top:
					return new PointF(0,0);
					
			} // end of switch
			return new PointF(0,0);
		}

		public PointF GetEnd(SizeF layerSize)
		{
			switch(m_StyleType)
			{
				case EdgeType.Left:
					return new PointF(0,0);
					
				case EdgeType.Right:
					return new PointF(layerSize.Width,0);
					
				case EdgeType.Bottom:
					return new PointF(layerSize.Width,layerSize.Height);
					
				case EdgeType.Top:
					return new PointF(layerSize.Width,0);
					
			} // end of switch
			return new PointF(0,0);
		}


		public static PointF GetPointBetween(PointF p1, PointF p2, double rel)
		{
			return new PointF((float)(p1.X+rel*(p2.X-p1.X)),(float)(p1.Y+rel*(p2.Y-p1.Y)));
		}

		public PointF GetEdgePoint(SizeF layerSize, double rel)
		{
			switch(m_StyleType)
			{
				case EdgeType.Left:
					return new PointF(0,(float)((1-rel)*layerSize.Height));
					
				case EdgeType.Right:
					return new PointF(layerSize.Width,(float)((1-rel)*layerSize.Height));
					
				case EdgeType.Bottom:
					return new PointF((float)(rel*layerSize.Width),layerSize.Height);
				
				case EdgeType.Top:
					return new PointF((float)(rel*layerSize.Width),0);
			
			} // end of switch
			return new PointF(0,0);
	
		}

		public float GetEdgeLength(SizeF layerSize)
		{
			switch(m_StyleType)
			{
				case EdgeType.Left:
				case EdgeType.Right:
					return layerSize.Height;
					
				case EdgeType.Bottom:
				case EdgeType.Top:
					return layerSize.Width;
			
			} // end of switch
		return 0;
		}

		public float GetOppositeEdgeLength(SizeF layerSize)
		{
			switch(m_StyleType)
			{
				case EdgeType.Left:
				case EdgeType.Right:
					return layerSize.Width;
					
				case EdgeType.Bottom:
				case EdgeType.Top:
					return layerSize.Height;
			} // end of switch
			return 0;
		}

		public PointF OuterVector
		{
			get
			{
				switch(m_StyleType)
				{
					case EdgeType.Left:
						return new PointF(-1,0);
						
					case EdgeType.Right:
						return new PointF(1,0);
					
					case EdgeType.Bottom:
						return new PointF(0,1);
					
					case EdgeType.Top:
						return new PointF(0,-1);
					
				} // end of switch
				return new PointF(0,0);
			}
		}

		public PointF InnerVector
		{
			get
			{
				switch(m_StyleType)
				{
					case EdgeType.Left:
						return new PointF(1,0);
						
					case EdgeType.Right:
						return new PointF(-1,0);
						
					case EdgeType.Bottom:
						return new PointF(0,-1);
						
					case EdgeType.Top:
						return new PointF(0,1);
					
				} // end of switch
				return new PointF(0,0);
			}
		}
	
	}
}
