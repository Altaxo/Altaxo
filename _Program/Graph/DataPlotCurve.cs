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
