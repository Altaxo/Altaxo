using System;
using System.Drawing;
using System.Drawing.Drawing2D;


namespace Altaxo.Graph
{
	/// <summary>
	/// Summary description for DataPlotCurve.
	/// </summary>
	public class DataPlotCurve
	{
		protected PlotAssociation m_PlotAssociation;
		protected PlotStyle       m_PlotStyle;


		public DataPlotCurve()
		{
		}


		public void Paint(Graphics g, Graph.Layer layer)
		{
			if(null!=this.m_PlotStyle)
			{
				m_PlotStyle.Paint(g,layer,m_PlotAssociation);
			}
		}

	}
}
