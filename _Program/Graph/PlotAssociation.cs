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
using Altaxo.Data;

namespace Altaxo.Graph
{

	/// <summary>
	/// Implemented by objects that hold x bounds, for instance XYPlotAssociations.
	/// </summary>
	public interface IXBoundsHolder
	{
		/// <summary>Fired if the x boundaries of the object changed.</summary>
		event PhysicalBoundaries.BoundaryChangedHandler	XBoundariesChanged;

		/// <summary>
		/// This sets the x boundary object to a object of the same type as val. The inner data of the boundary, if present,
		/// are copied into the new x boundary object.
		/// </summary>
		/// <param name="val">The template boundary object.</param>
		void SetXBoundsFromTemplate(PhysicalBoundaries val);

		/// <summary>
		/// This merges the x boundary of the object with the boundary pb. The boundary pb is updated so that
		/// it now includes the x boundary range of the object.
		/// </summary>
		/// <param name="pb">The boundary object pb which is updated to include the x boundaries of the object.</param>
		void MergeXBoundsInto(PhysicalBoundaries pb);
	}

	/// <summary>
	/// Implemented by objects that hold y bounds, for instance XYPlotAssociations.
	/// </summary>
	public interface IYBoundsHolder
	{
		/// <summary>Fired if the y boundaries of the object changed.</summary>
		event PhysicalBoundaries.BoundaryChangedHandler	YBoundariesChanged;

		/// <summary>
		/// This sets the y boundary object to a object of the same type as val. The inner data of the boundary, if present,
		/// are copied into the new y boundary object.
		/// </summary>
		/// <param name="val">The template boundary object.</param>
		void SetYBoundsFromTemplate(PhysicalBoundaries val);

		/// <summary>
		/// This merges the y boundary of the object with the boundary pb. The boundary pb is updated so that
		/// it now includes the y boundary range of the object.
		/// </summary>
		/// <param name="pb">The boundary object pb which is updated to include the y boundaries of the object.</param>
		void MergeYBoundsInto(PhysicalBoundaries pb);
	}


	/// <summary>
	/// Implemented by objects that hold x bounds and y bounds, for instance XYPlotAssociations.
	/// </summary>
	public interface IXYBoundsHolder : IXBoundsHolder, IYBoundsHolder
	{
	}


	/// <summary>
	/// Summary description for PlotAssociation.
	/// </summary>
	[SerializationSurrogate(0,typeof(PlotAssociation.SerializationSurrogate0))]
	[SerializationVersion(0)]
	public class PlotAssociation : IXYBoundsHolder, System.Runtime.Serialization.IDeserializationCallback, IChangedEventSource, System.ICloneable
	{
		protected Altaxo.Data.IReadableColumn m_xColumn; // the X-Column
		protected Altaxo.Data.IReadableColumn m_yColumn; // the Y-Column

		// cached or temporary data
		protected PhysicalBoundaries m_xBoundaries;
		protected PhysicalBoundaries m_yBoundaries;

		protected int    m_PlottablePoints; // number of plottable points
		protected bool   m_bCachedDataValid=false;

		// events
		public event PhysicalBoundaries.BoundaryChangedHandler	XBoundariesChanged;
		public event PhysicalBoundaries.BoundaryChangedHandler	YBoundariesChanged;


		/// <summary>
		/// Fired if either the data of this PlotAssociation changed or if the bounds changed
		/// </summary>
		public event System.EventHandler Changed;


		#region Serialization
		/// <summary>Used to serialize the PlotAssociation Version 0.</summary>
		public class SerializationSurrogate0 : System.Runtime.Serialization.ISerializationSurrogate
		{
			/// <summary>
			/// Serializes PlotAssociation Version 0.
			/// </summary>
			/// <param name="obj">The PlotAssociation to serialize.</param>
			/// <param name="info">The serialization info.</param>
			/// <param name="context">The streaming context.</param>
			public void GetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context	)
			{
				PlotAssociation s = (PlotAssociation)obj;
				
				info.AddValue("XColumn",s.m_xColumn);
				info.AddValue("YColumn",s.m_yColumn);

				info.AddValue("XBoundaries",s.m_xBoundaries);
				info.AddValue("YBoundaries",s.m_yBoundaries);

			}
			/// <summary>
			/// Deserializes the PlotAssociation Version 0.
			/// </summary>
			/// <param name="obj">The empty PlotAssociation object to deserialize into.</param>
			/// <param name="info">The serialization info.</param>
			/// <param name="context">The streaming context.</param>
			/// <param name="selector">The deserialization surrogate selector.</param>
			/// <returns>The deserialized PlotAssociation.</returns>
			public object SetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context,System.Runtime.Serialization.ISurrogateSelector selector)
			{
				PlotAssociation s = (PlotAssociation)obj;

				s.m_xColumn = (Altaxo.Data.IReadableColumn)info.GetValue("XColumn",typeof(Altaxo.Data.IReadableColumn));
				s.m_yColumn = (Altaxo.Data.IReadableColumn)info.GetValue("YColumn",typeof(Altaxo.Data.IReadableColumn));

				s.m_xBoundaries = (PhysicalBoundaries)info.GetValue("XBoundaries",typeof(PhysicalBoundaries));
				s.m_yBoundaries = (PhysicalBoundaries)info.GetValue("YBoundaries",typeof(PhysicalBoundaries));
	
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
			if(m_xColumn is Altaxo.Data.DataColumn)
				((Altaxo.Data.DataColumn)m_xColumn).DataChanged += new Altaxo.Data.DataColumn.DataChangedHandler(OnColumnDataChangedEventHandler);
			
			if(m_yColumn is Altaxo.Data.DataColumn)
				((Altaxo.Data.DataColumn)m_yColumn).DataChanged += new Altaxo.Data.DataColumn.DataChangedHandler(OnColumnDataChangedEventHandler);
		
			if(null!=m_xBoundaries)
				m_xBoundaries.BoundaryChanged -= new PhysicalBoundaries.BoundaryChangedHandler(this.OnXBoundariesChangedEventHandler);

			if(null!=m_yBoundaries)
				m_yBoundaries.BoundaryChanged -= new PhysicalBoundaries.BoundaryChangedHandler(this.OnYBoundariesChangedEventHandler);
		}
		#endregion



		public PlotAssociation(Altaxo.Data.IReadableColumn xColumn, Altaxo.Data.IReadableColumn yColumn)
		{
			XColumn = xColumn;
			YColumn = yColumn;


			this.SetXBoundsFromTemplate( new FinitePhysicalBoundaries() );
			this.SetYBoundsFromTemplate( new FinitePhysicalBoundaries() );

		}
	

		/// <summary>
		/// Copy constructor.
		/// </summary>
		/// <param name="from">The object to copy from.</param>
		/// <remarks>Only clones the references to the data columns, not the columns itself.</remarks>
		public PlotAssociation(PlotAssociation from)
		{
			XColumn = from.XColumn; // also wires event, do not clone the column data here!!!
			YColumn = from.YColumn; // wires event, do not clone the column data here!!!

			this.SetXBoundsFromTemplate( new FinitePhysicalBoundaries() );
			this.SetYBoundsFromTemplate( new FinitePhysicalBoundaries() );
				
		}

		/// <summary>
		/// Creates a cloned copy of this object.
		/// </summary>
		/// <returns>The cloned copy of this object.</returns>
		/// <remarks>The data columns refered by this object are <b>not</b> cloned, only the reference is cloned here.</remarks>
		public object Clone()
		{
			return new PlotAssociation(this);
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
					m_xBoundaries.BoundaryChanged -= new PhysicalBoundaries.BoundaryChangedHandler(this.OnXBoundariesChangedEventHandler);
				}
				m_xBoundaries = (PhysicalBoundaries)val.Clone();
				m_xBoundaries.BoundaryChanged += new PhysicalBoundaries.BoundaryChangedHandler(this.OnXBoundariesChangedEventHandler);
				m_bCachedDataValid = false;

				OnChanged();
			}
		}


		public void SetYBoundsFromTemplate(PhysicalBoundaries val)
		{
			if(null==m_yBoundaries || val.GetType() != m_yBoundaries.GetType())
			{
				if(null!=m_yBoundaries)
				{
					m_yBoundaries.BoundaryChanged -= new PhysicalBoundaries.BoundaryChangedHandler(this.OnYBoundariesChangedEventHandler);
				}
				m_yBoundaries = (PhysicalBoundaries)val.Clone();
				m_yBoundaries.BoundaryChanged += new PhysicalBoundaries.BoundaryChangedHandler(this.OnYBoundariesChangedEventHandler);
				m_bCachedDataValid = false;

				OnChanged();
			}
		}


		
		protected virtual void OnXBoundariesChangedEventHandler(object sender, BoundariesChangedEventArgs e)
		{
			if(null!=this.XBoundariesChanged)
				XBoundariesChanged(this, e);

			OnChanged();
		}

		protected virtual void OnYBoundariesChangedEventHandler(object sender, BoundariesChangedEventArgs e)
		{
			if(null!=this.YBoundariesChanged)
				YBoundariesChanged(this, e);

			OnChanged();
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

		public Altaxo.Data.IReadableColumn XColumn
		{
			get
			{
				return m_xColumn;
			}
			set
			{

				if(null!=m_xColumn && m_xColumn is Altaxo.Data.DataColumn)
				{
					((Altaxo.Data.DataColumn)m_xColumn).DataChanged -= new Altaxo.Data.DataColumn.DataChangedHandler(OnColumnDataChangedEventHandler);
				}

				this.m_xColumn = value;

				if(null!=m_xColumn && m_xColumn is Altaxo.Data.DataColumn)
				{
					((Altaxo.Data.DataColumn)m_xColumn).DataChanged += new Altaxo.Data.DataColumn.DataChangedHandler(OnColumnDataChangedEventHandler);
				}

				m_bCachedDataValid = false;
				OnChanged();
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
				if(null!=m_yColumn && m_yColumn is Altaxo.Data.DataColumn)
				{
					((Altaxo.Data.DataColumn)m_yColumn).DataChanged -= new Altaxo.Data.DataColumn.DataChangedHandler(OnColumnDataChangedEventHandler);
				}

				this.m_yColumn = value;
				if(null!=m_yColumn && m_yColumn is Altaxo.Data.DataColumn)
				{
					((Altaxo.Data.DataColumn)m_yColumn).DataChanged += new Altaxo.Data.DataColumn.DataChangedHandler(OnColumnDataChangedEventHandler);
				}
				m_bCachedDataValid = false;
				this.OnChanged();
			}
		}

		public override string ToString()
		{
			return String.Format("{0}(X), {1}(Y)",m_xColumn.FullName,m_yColumn.FullName);
		}

		public void CalculateCachedData()
		{
			// we can calulate the bounds only if they are set before
			if(null==m_xBoundaries || null==m_yBoundaries)
				return;


			m_PlottablePoints = 0;

			
			this.m_xBoundaries.BeginUpdate(); // disable events
			this.m_yBoundaries.BeginUpdate(); // disable events
			
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
			this.m_xBoundaries.EndUpdate(); // enable events
			this.m_yBoundaries.EndUpdate(); // enable events
		}

		void OnColumnDataChangedEventHandler(Altaxo.Data.DataColumn dc, int nMinRow, int nMaxRow, bool bRowCountDecreased)
		{
			// !!!todo!!! : special case if only data added to a column should
			// be handeld separately to save computing time
			this.m_bCachedDataValid = false;
		
			OnChanged();
		}

		protected virtual void OnChanged()
		{
			if(null!=Changed)
				Changed(this,new System.EventArgs());
		}
	}
}
