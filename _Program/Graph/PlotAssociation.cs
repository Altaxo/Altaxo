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

using Altaxo.Data;

namespace Altaxo.Graph
{
	/// <summary>
	/// Summary description for PlotAssociation.
	/// </summary>
	public class PlotAssociation
	{
		protected Altaxo.Data.IReadableColumn m_xColumn; // the X-Column
		protected Altaxo.Data.IReadableColumn m_yColumn; // the Y-Column

		protected PhysicalBoundaries m_xBoundaries;
		protected PhysicalBoundaries m_yBoundaries;

		protected int    m_PlottablePoints; // number of plottable points
		protected bool   m_bCachedDataValid=false;

		protected PlotStyle m_PlotStyle = null;

		public event PhysicalBoundaries.BoundaryChangedHandler	XBoundariesChanged;
		public event PhysicalBoundaries.BoundaryChangedHandler	YBoundariesChanged;


		public PlotAssociation(Altaxo.Data.DataColumn m_xColumn, Altaxo.Data.DataColumn m_yColumn)
		{
			m_PlotStyle = new LineScatterPlotStyle(this);

			this.m_xColumn = m_xColumn;
			this.m_yColumn = m_yColumn;


			m_xBoundaries = new FinitePhysicalBoundaries();
			m_yBoundaries = new FinitePhysicalBoundaries();
			
			// add boundary event handler
			m_xBoundaries.BoundaryChanged += new PhysicalBoundaries.BoundaryChangedHandler(this.OnXBoundariesChanged);
			m_yBoundaries.BoundaryChanged += new PhysicalBoundaries.BoundaryChangedHandler(this.OnYBoundariesChanged);


			// Add Event Handler
			m_xColumn.DataChanged += new Altaxo.Data.DataColumn.DataChangedHandler(OnColumnDataChanged);
			m_yColumn.DataChanged += new Altaxo.Data.DataColumn.DataChangedHandler(OnColumnDataChanged);
		}
	

		public void MergeXBoundsInto(PhysicalBoundaries pb)
		{
			if(!this.m_bCachedDataValid)
				this.CalculateCachedData();
			pb.Add(m_xBoundaries);
		}

		public void MergeYBoundsInto(PhysicalBoundaries pb)
		{
			if(!this.m_bCachedDataValid)
				this.CalculateCachedData();
			pb.Add(m_yBoundaries);
		}

		public void SetXBoundsFromTemplate(PhysicalBoundaries val)
		{
			if(null==m_xBoundaries || val.GetType() != m_xBoundaries.GetType())
			{
				if(null!=m_xBoundaries)
				{
					m_xBoundaries.BoundaryChanged -= new PhysicalBoundaries.BoundaryChangedHandler(this.OnXBoundariesChanged);
				}
				m_xBoundaries = (PhysicalBoundaries)val.Clone();
				m_xBoundaries.BoundaryChanged += new PhysicalBoundaries.BoundaryChangedHandler(this.OnXBoundariesChanged);
				CalculateCachedData();
			}
		}


		public void SetYBoundsFromTemplate(PhysicalBoundaries val)
		{
			if(null==m_yBoundaries || val.GetType() != m_yBoundaries.GetType())
			{
				if(null!=m_yBoundaries)
				{
					m_yBoundaries.BoundaryChanged -= new PhysicalBoundaries.BoundaryChangedHandler(this.OnYBoundariesChanged);
				}
				m_yBoundaries = (PhysicalBoundaries)val.Clone();
				m_yBoundaries.BoundaryChanged += new PhysicalBoundaries.BoundaryChangedHandler(this.OnYBoundariesChanged);
				CalculateCachedData();
			}
		}


		public void Paint(Graphics g, Graph.Layer layer)
		{
			if(null!=this.m_PlotStyle)
			{
				m_PlotStyle.Paint(g,layer,this);
			}
		}



		/*
		public PhysicalBoundaries XBounds
		{
			get
			{
				if(!this.m_bCachedDataValid)
					this.CalculateCachedData();
				return m_xBoundaries;
			}
			set
			{
				if(null==m_xBoundaries || value.GetType() != m_xBoundaries.GetType())
				{
					if(null!=m_xBoundaries)
					{
						m_xBoundaries.BoundaryChanged -= new PhysicalBoundaries.BoundaryChangedHandler(this.OnXBoundariesChanged);
					}
					m_xBoundaries = (PhysicalBoundaries)value.Clone();
					m_xBoundaries.BoundaryChanged += new PhysicalBoundaries.BoundaryChangedHandler(this.OnXBoundariesChanged);
					CalculateCachedData();
				}
			}
		}


		public PhysicalBoundaries YBounds
		{
			get
			{
				if(!this.m_bCachedDataValid)
					this.CalculateCachedData();
				return m_yBoundaries;
			}
			set
			{
				if(null==m_yBoundaries || value.GetType() != m_yBoundaries.GetType())
				{
					if(null!=m_yBoundaries)
					{
						m_yBoundaries.BoundaryChanged -= new PhysicalBoundaries.BoundaryChangedHandler(this.OnYBoundariesChanged);
					}
					m_yBoundaries = (PhysicalBoundaries)value.Clone();
					m_yBoundaries.BoundaryChanged += new PhysicalBoundaries.BoundaryChangedHandler(this.OnYBoundariesChanged);
					CalculateCachedData();
				}
			}
		}

		
		*/


		protected void OnXBoundariesChanged(object sender, BoundariesChangedEventArgs e)
		{
			if(null!=this.XBoundariesChanged)
				XBoundariesChanged(this, e);
		}

		protected void OnYBoundariesChanged(object sender, BoundariesChangedEventArgs e)
		{
			if(null!=this.YBoundariesChanged)
				YBoundariesChanged(this, e);
		}



		public int PlottablePoints
		{
			get
			{
				if(!this.m_bCachedDataValid)
					this.CalculateCachedData();
				return this.m_PlottablePoints;
			}
		}

		public PlotStyle PlotStyle
		{
			get
			{
				return m_PlotStyle;
			}
			set
			{
//				PlotStyle oldPlotStyle = m_PlotStyle;
				m_PlotStyle = value;
/*
				if(null!=oldPlotStyle && !object.ReferenceEquals(oldPlotStyle,value))
					oldPlotStyle.PlotAssociation=null; // good by my old plot association

				if(null!=m_PlotStyle && !object.ReferenceEquals(m_PlotStyle.PlotAssociation,this))
					m_PlotStyle.PlotAssociation = this; // implement my own association
*/	

		}
		}

		public Altaxo.Data.IReadableColumn XColumn
		{
			get
			{
				return m_xColumn;
			}
			set
			{
				this.m_xColumn = value;
				CalculateCachedData();
			}
		}

		public Altaxo.Data.IReadableColumn YColumn
		{
			get
			{
				return m_yColumn;
			}
			set
			{
				this.m_yColumn = value;
				CalculateCachedData();
			}
		}

		public override string ToString()
		{
			return String.Format("{0}(X), {1}(Y)",m_xColumn.FullName,m_yColumn.FullName);
		}

		public void CalculateCachedData()
		{
			m_PlottablePoints = 0;

			
			this.m_xBoundaries.EventsEnabled = false; // disable events
			this.m_yBoundaries.EventsEnabled = false; // disable events
			
			this.m_xBoundaries.Reset();
			this.m_yBoundaries.Reset();

			int len = int.MaxValue;
			if(m_xColumn is IDefinedCount)
				len = System.Math.Min(len,((IDefinedCount)m_xColumn).Count);
			if(m_yColumn is IDefinedCount)
				len = System.Math.Min(len,((IDefinedCount)m_yColumn).Count);

			// if both columns are indefinite long, we set the length to zero
			if(len==int.MaxValue)
				len=0;


			for(int i=0;i<len;i++)
			{
				if(!m_xColumn.IsElementEmpty(i) && !m_yColumn.IsElementEmpty(i)) 
				{
					bool x_added = this.m_xBoundaries.Add(m_xColumn,i);
					bool y_added = this.m_yBoundaries.Add(m_yColumn,i);
					if(y_added && y_added)
						m_PlottablePoints++;
				}
			}

			// now the cached data are valid
			m_bCachedDataValid = true;


			// now when the cached data are valid, we can reenable the events
			this.m_xBoundaries.EventsEnabled = true; // enable events
			this.m_yBoundaries.EventsEnabled = true; // enable events

		}

		void OnColumnDataChanged(Altaxo.Data.DataColumn dc, int nMinRow, int nMaxRow, bool bRowCountDecreased)
		{
			// !!!todo!!! : special case if only data added to a column should
			// be handeld separately to save computing time
			CalculateCachedData();
		}
	}
}
