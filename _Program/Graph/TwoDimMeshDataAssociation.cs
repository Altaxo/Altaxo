////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2003 Dr. Dirk Lellinger
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
	/// Summary description for PlotAssociation.
	/// </summary>
	[SerializationSurrogate(0,typeof(D2EquidistantMeshDataAssociation.SerializationSurrogate0))]
	[SerializationVersion(0)]
	public class D2EquidistantMeshDataAssociation 
		:
		IXYBoundsHolder, System.Runtime.Serialization.IDeserializationCallback,
		IChangedEventSource,
		System.ICloneable,
		Main.IDocumentNode
	{
		protected object m_Parent;

		protected Altaxo.Data.IReadableColumn[] m_DataColumns; // the columns that are involved in the picture


		protected Altaxo.Data.INumericColumn m_XColumn;
		protected Altaxo.Data.INumericColumn m_YColumn;

		// cached or temporary data
		protected PhysicalBoundaries m_xBoundaries; 
		protected PhysicalBoundaries m_yBoundaries;
		protected PhysicalBoundaries m_vBoundaries;


		/// <summary>
		/// Number of rows, here the maximum of the row counts of all columns.
		/// </summary>
		protected int                m_Rows;
		protected bool   m_bCachedDataValid=false;

		// events
		public event PhysicalBoundaries.BoundaryChangedHandler	XBoundariesChanged;
		public event PhysicalBoundaries.BoundaryChangedHandler	YBoundariesChanged;
		public event PhysicalBoundaries.BoundaryChangedHandler	VBoundariesChanged;


		/// <summary>
		/// Fired if either the data of this PlotAssociation changed or if the bounds changed
		/// </summary>
		public event System.EventHandler Changed;


		#region Serialization
		/// <summary>Used to serialize the PlotAssociation Version 0.</summary>
		public class SerializationSurrogate0 : System.Runtime.Serialization.ISerializationSurrogate
		{
			/// <summary>
			/// Serializes D2EquidistantMeshDataAssociation Version 0.
			/// </summary>
			/// <param name="obj">The D2EquidistantMeshDataAssociation to serialize.</param>
			/// <param name="info">The serialization info.</param>
			/// <param name="context">The streaming context.</param>
			public void GetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context	)
			{
				D2EquidistantMeshDataAssociation s = (D2EquidistantMeshDataAssociation)obj;
				
				info.AddValue("XColumn",s.m_XColumn);
				info.AddValue("YColumn",s.m_YColumn);
				info.AddValue("DataColumns",s.m_DataColumns);

				info.AddValue("XBoundaries",s.m_xBoundaries);
				info.AddValue("YBoundaries",s.m_yBoundaries);
				info.AddValue("VBoundaries",s.m_vBoundaries);

			}
			/// <summary>
			/// Deserializes theD2EquidistantMeshDataAssociation Version 0.
			/// </summary>
			/// <param name="obj">The empty D2EquidistantMeshDataAssociation object to deserialize into.</param>
			/// <param name="info">The serialization info.</param>
			/// <param name="context">The streaming context.</param>
			/// <param name="selector">The deserialization surrogate selector.</param>
			/// <returns>The deserialized D2EquidistantMeshDataAssociation.</returns>
			public object SetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context,System.Runtime.Serialization.ISurrogateSelector selector)
			{
				D2EquidistantMeshDataAssociation s = (D2EquidistantMeshDataAssociation)obj;


				s.m_XColumn = (Altaxo.Data.INumericColumn)info.GetValue("XColumn",typeof(Altaxo.Data.INumericColumn));
				s.m_YColumn = (Altaxo.Data.INumericColumn)info.GetValue("YColumn",typeof(Altaxo.Data.INumericColumn));
				s.m_DataColumns = (Altaxo.Data.IReadableColumn[])info.GetValue("DataColumns",typeof(Altaxo.Data.IReadableColumn[]));
		
				s.m_xBoundaries = (PhysicalBoundaries)info.GetValue("XBoundaries",typeof(PhysicalBoundaries));
				s.m_yBoundaries = (PhysicalBoundaries)info.GetValue("YBoundaries",typeof(PhysicalBoundaries));
				s.m_vBoundaries = (PhysicalBoundaries)info.GetValue("VBoundaries",typeof(PhysicalBoundaries));

				return s;
			}
		}

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(D2EquidistantMeshDataAssociation),0)]
			public new class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				D2EquidistantMeshDataAssociation s = (D2EquidistantMeshDataAssociation)obj;
				info.AddValue("XColumn",s.m_XColumn);
				info.AddValue("YColumn",s.m_YColumn);
				info.AddValue("DataColumns",s.m_DataColumns);

				info.AddValue("XBoundaries",s.m_xBoundaries);
				info.AddValue("YBoundaries",s.m_yBoundaries);
				info.AddValue("VBoundaries",s.m_vBoundaries);
			}
			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlSerializationInfo info, object parent)
			{
				
				D2EquidistantMeshDataAssociation s = null!=o ? (D2EquidistantMeshDataAssociation)o : new D2EquidistantMeshDataAssociation();

				s.m_XColumn = (Altaxo.Data.INumericColumn)info.GetValue("XColumn",typeof(Altaxo.Data.INumericColumn));
				s.m_YColumn = (Altaxo.Data.INumericColumn)info.GetValue("YColumn",typeof(Altaxo.Data.INumericColumn));
				s.m_DataColumns = (Altaxo.Data.IReadableColumn[])info.GetValue("DataColumns",typeof(Altaxo.Data.IReadableColumn[]));
		
				s.m_xBoundaries = (PhysicalBoundaries)info.GetValue("XBoundaries",typeof(PhysicalBoundaries));
				s.m_yBoundaries = (PhysicalBoundaries)info.GetValue("YBoundaries",typeof(PhysicalBoundaries));
				s.m_vBoundaries = (PhysicalBoundaries)info.GetValue("VBoundaries",typeof(PhysicalBoundaries));

				s.OnDeserialization(null);

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

			if(m_XColumn is Altaxo.Data.DataColumn)
				((Altaxo.Data.DataColumn)m_XColumn).DataChanged += new Altaxo.Data.DataColumn.DataChangedHandler(OnColumnDataChangedEventHandler);

			if(m_YColumn is Altaxo.Data.DataColumn)
				((Altaxo.Data.DataColumn)m_YColumn).DataChanged += new Altaxo.Data.DataColumn.DataChangedHandler(OnColumnDataChangedEventHandler);

			for(int i=0;i<m_DataColumns.Length;i++)
			{
				if(m_DataColumns[i] is Altaxo.Data.DataColumn)
					((Altaxo.Data.DataColumn)m_DataColumns[i]).DataChanged += new Altaxo.Data.DataColumn.DataChangedHandler(OnColumnDataChangedEventHandler);
			}			
		
			m_xBoundaries.BoundaryChanged += new PhysicalBoundaries.BoundaryChangedHandler(this.OnXBoundariesChangedEventHandler);
			m_yBoundaries.BoundaryChanged += new PhysicalBoundaries.BoundaryChangedHandler(this.OnYBoundariesChangedEventHandler);
			m_vBoundaries.BoundaryChanged += new PhysicalBoundaries.BoundaryChangedHandler(this.OnVBoundariesChangedEventHandler);


			// do not calculate cached data here, since it is done the first time this data is really needed
			this.m_bCachedDataValid=false;
		}
		#endregion

		/// <summary>
		/// Deserialization constructor.
		/// </summary>
		protected D2EquidistantMeshDataAssociation()
		{
		}

		public D2EquidistantMeshDataAssociation(Altaxo.Data.DataColumnCollection coll, int[] selected)
		{
			m_XColumn = new Altaxo.Data.IndexerColumn();
			m_YColumn = new Altaxo.Data.IndexerColumn();

			int len = selected==null ? coll.ColumnCount : selected.Length;
			m_DataColumns = new Altaxo.Data.IReadableColumn[len];
			for(int i=0;i<len;i++)
			{
				int idx = null==selected ? i : selected[i];
				m_DataColumns[i] = coll[idx];

				// set the event chain
				if(m_DataColumns[i] is Altaxo.Data.DataColumn)
					((Altaxo.Data.DataColumn)m_DataColumns[i]).DataChanged += new Altaxo.Data.DataColumn.DataChangedHandler(OnColumnDataChangedEventHandler);
			}


			this.SetXBoundsFromTemplate( new FinitePhysicalBoundaries() );
			this.SetYBoundsFromTemplate( new FinitePhysicalBoundaries() );
			this.SetVBoundsFromTemplate( new FinitePhysicalBoundaries() );

		}
	

		/// <summary>
		/// Copy constructor.
		/// </summary>
		/// <param name="from">The object to copy from.</param>
		/// <remarks>Only clones the references to the data columns, not the columns itself.</remarks>
		public D2EquidistantMeshDataAssociation(D2EquidistantMeshDataAssociation from)
		{
			if(from.m_XColumn is Altaxo.Data.DataColumn && ((Altaxo.Data.DataColumn)from.m_XColumn).Parent!=null)
				m_XColumn = from.m_XColumn;
			else
				m_XColumn = (Altaxo.Data.INumericColumn)from.m_XColumn.Clone();

			if(from.m_YColumn is Altaxo.Data.DataColumn && ((Altaxo.Data.DataColumn)from.m_YColumn).Parent!=null)
				m_YColumn = from.m_YColumn;
			else
				m_YColumn = (Altaxo.Data.INumericColumn)from.m_YColumn.Clone();

			int len = from.m_DataColumns.Length;
			m_DataColumns = new Altaxo.Data.IReadableColumn[len];
	
			for(int i=0;i<len;i++)
			{
				m_DataColumns[i] = from.m_DataColumns[i]; // do not clone the data columns!

				// set the event chain
				if(m_DataColumns[i] is Altaxo.Data.DataColumn)
					((Altaxo.Data.DataColumn)m_DataColumns[i]).DataChanged += new Altaxo.Data.DataColumn.DataChangedHandler(OnColumnDataChangedEventHandler);
			}


			this.SetXBoundsFromTemplate( new FinitePhysicalBoundaries() );
			this.SetYBoundsFromTemplate( new FinitePhysicalBoundaries() );
			this.SetVBoundsFromTemplate( new FinitePhysicalBoundaries() );

				
		}

		/// <summary>
		/// Creates a cloned copy of this object.
		/// </summary>
		/// <returns>The cloned copy of this object.</returns>
		/// <remarks>The data columns refered by this object are <b>not</b> cloned, only the reference is cloned here.</remarks>
		public object Clone()
		{
			return new D2EquidistantMeshDataAssociation(this);
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

		public void MergeVBoundsInto(PhysicalBoundaries pb)
		{
			if(!this.m_bCachedDataValid)
				this.CalculateCachedData();
			pb.Add(m_vBoundaries);
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
				this.m_bCachedDataValid = false;

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
				this.m_bCachedDataValid = false;

				OnChanged();
			}
		}


		public void SetVBoundsFromTemplate(PhysicalBoundaries val)
		{
			if(null==m_vBoundaries || val.GetType() != m_vBoundaries.GetType())
			{
				if(null!=m_vBoundaries)
				{
					m_vBoundaries.BoundaryChanged -= new PhysicalBoundaries.BoundaryChangedHandler(this.OnVBoundariesChangedEventHandler);
				}
				m_vBoundaries = (PhysicalBoundaries)val.Clone();
				m_vBoundaries.BoundaryChanged += new PhysicalBoundaries.BoundaryChangedHandler(this.OnVBoundariesChangedEventHandler);
				this.m_bCachedDataValid = false;

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

		protected virtual void OnVBoundariesChangedEventHandler(object sender, BoundariesChangedEventArgs e)
		{
			if(null!=this.VBoundariesChanged)
				VBoundariesChanged(this, e);

			OnChanged();
		}

		public int RowCount
		{
			get
			{
				if(!this.m_bCachedDataValid)
					this.CalculateCachedData();
				return m_Rows;
			}
		}

		public int ColumnCount
		{
			get
			{
				return null==this.m_DataColumns ? 0 : m_DataColumns.Length;
			}
		}



		public Altaxo.Data.IReadableColumn[] DataColumns
		{
			get
			{
				return m_DataColumns;
			}
		}

		public Altaxo.Data.IReadableColumn XColumn
		{
			get { return this.m_XColumn; }
		}

		public Altaxo.Data.IReadableColumn YColumn
		{
			get { return this.m_YColumn; }
		}


		public override string ToString()
		{
			if(null!=m_DataColumns && m_DataColumns.Length>0)
			return String.Format("PictureData {0}-{1}",m_DataColumns[0].FullName,m_DataColumns[m_DataColumns.Length-1].FullName);
			else
				return "Empty (no data)";
		}

		public void CalculateCachedData()
		{
			if(null==m_DataColumns || m_DataColumns.Length==0)
			{
				m_Rows = 0;
				return;
			}
			
			this.m_xBoundaries.BeginUpdate(); // disable events
			this.m_yBoundaries.BeginUpdate(); // disable events
			this.m_vBoundaries.BeginUpdate();
			
			this.m_xBoundaries.Reset();
			this.m_yBoundaries.Reset();
			this.m_vBoundaries.Reset();

			// get the length of the largest column as row count
			m_Rows = 0;
			for(int i=0;i<m_DataColumns.Length;i++)
			{
				if(m_DataColumns[i] is IDefinedCount)
					m_Rows = System.Math.Max(m_Rows,((IDefinedCount)m_DataColumns[i]).Count);
			}

			for(int i=0;i<m_DataColumns.Length;i++)
			{
				Altaxo.Data.IReadableColumn col = m_DataColumns[i];
				int collength = (col is Altaxo.Data.IDefinedCount) ? ((Altaxo.Data.IDefinedCount)col).Count : m_Rows;
				for(int j=0;j<collength;j++)
				{
					this.m_vBoundaries.Add(col,j);
				}
			}


			// enter the two bounds for x
			for(int i=0;i<m_DataColumns.Length;i++)
				this.m_yBoundaries.Add(m_YColumn,i);

			// enter the bounds for y
			for(int i=0;i<m_Rows;i++)
				this.m_xBoundaries.Add(m_XColumn,i);

			// now the cached data are valid
			m_bCachedDataValid = true;


			// now when the cached data are valid, we can reenable the events
			this.m_xBoundaries.EndUpdate(); // enable events
			this.m_yBoundaries.EndUpdate(); // enable events
			this.m_vBoundaries.EndUpdate(); // enable events

		}

		void OnColumnDataChangedEventHandler(Altaxo.Data.DataColumn dc, int nMinRow, int nMaxRow, bool bRowCountDecreased)
		{
			
			m_bCachedDataValid = false;
		
			OnChanged();
		}

		protected virtual void OnChanged()
		{
			if(null!=Changed)
				Changed(this,new System.EventArgs());
		}

		public virtual object ParentObject
		{
			get { return m_Parent; }
			set { m_Parent = value; }
		}

		public virtual string Name
		{
			get
			{
				Main.INamedObjectCollection noc = ParentObject as Main.INamedObjectCollection;
				return null==noc ? null : noc.GetNameOfChildObject(this);
			}
		}
	}


} // end name space
