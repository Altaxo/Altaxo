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
using System.Runtime.Serialization;
using Altaxo.Serialization;
using ICSharpCode.SharpZipLib.Zip;

namespace Altaxo
{
	/// <summary>
	/// Summary description for AltaxoDocument.
	/// </summary>
	[SerializationSurrogate(0,typeof(AltaxoDocument.SerializationSurrogate0))]
	[SerializationVersion(0,"Initial version of the main document only contains m_DataSet")]
	public class AltaxoDocument 
		: 
		IDeserializationCallback,
		Main.INamedObjectCollection	,
		Main.IChildChangedEventSink	
	{
		protected Altaxo.Data.DataTableCollection m_DataSet = null; // The root of all the data

		protected Altaxo.Graph.GraphDocumentCollection m_GraphSet = null; // all graphs are stored here

		protected Altaxo.Worksheet.WorksheetLayoutCollection m_TableLayoutList = null;

	//	protected System.Collections.ArrayList m_Worksheets;
		/// <summary>The list of GraphForms for the document.</summary>
	//	protected System.Collections.ArrayList m_GraphForms;

		[NonSerialized]
		protected bool m_IsDirty=false;
		public event EventHandler DirtyChanged;
		[NonSerialized]
		private bool m_DeserializationFinished=false;

		public AltaxoDocument()
		{
			m_DataSet = new Altaxo.Data.DataTableCollection(this);
			m_GraphSet = new Altaxo.Graph.GraphDocumentCollection(this);
			m_TableLayoutList = new Altaxo.Worksheet.WorksheetLayoutCollection(this);
		//	m_Worksheets = new System.Collections.ArrayList();
		//	m_GraphForms = new System.Collections.ArrayList();
		}

		#region Serialization
		public class SerializationSurrogate0 : System.Runtime.Serialization.ISerializationSurrogate
		{
			public void GetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context	)
			{
				AltaxoDocument s = (AltaxoDocument)obj;
				info.AddValue("DataTableCollection",s.m_DataSet);
				//info.AddValue("Worksheets",s.m_Worksheets);
			//	info.AddValue("GraphForms",s.m_GraphForms);
			}
			public object SetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context,System.Runtime.Serialization.ISurrogateSelector selector)
			{
				AltaxoDocument s = (AltaxoDocument)obj;
				s.m_DataSet = (Altaxo.Data.DataTableCollection)info.GetValue("DataTableCollection",typeof(Altaxo.Data.DataTableCollection));
				// s.tstObj    = (AltaxoTestObject02)info.GetValue("TstObj",typeof(AltaxoTestObject02));
				//s.m_Worksheets = (System.Collections.ArrayList)info.GetValue("Worksheets",typeof(System.Collections.ArrayList));
			//	s.m_GraphForms = (System.Collections.ArrayList)info.GetValue("GraphForms",typeof(System.Collections.ArrayList));
				s.m_IsDirty = false;
				return s;
			}
		}

		public void OnDeserialization(object obj)
		{
			if(!m_DeserializationFinished && obj is DeserializationFinisher)
			{
				m_DeserializationFinished=true;
				DeserializationFinisher finisher = new DeserializationFinisher(this);
			
				m_DataSet.ParentDocument = this;
				m_DataSet.OnDeserialization(finisher);

			/*
				for(int i=0;i<m_Worksheets.Count;i++)
				{
					m_Worksheets[i] = ((IDeserializationSubstitute)m_Worksheets[i]).GetRealObject(App.Current.View.Form);
					((System.Windows.Forms.Form)m_Worksheets[i]).Show();
				}
				for(int i=0;i<m_GraphForms.Count;i++)
				{
					m_GraphForms[i] = ((IDeserializationSubstitute)m_GraphForms[i]).GetRealObject(App.Current.View.Form);
					((System.Windows.Forms.Form)m_GraphForms[i]).Show();
				}
				*/
			}
		}

		public void RestoreWindowsAfterDeserialization()
		{
		}


		public void SaveToZippedFile(ZipOutputStream zippedStream, Altaxo.Serialization.Xml.XmlStreamSerializationInfo info)
		{
			// first, we save all tables into the tables subdirectory
			foreach(Altaxo.Data.DataTable table in this.m_DataSet)
			{
				ZipEntry ZipEntry = new ZipEntry("Tables/"+table.Name+".xml");
				zippedStream.PutNextEntry(ZipEntry);
				zippedStream.SetLevel(0);
				info.BeginWriting(zippedStream);
				info.AddValue("Table",table);
				info.EndWriting();
			}

			// second, we save all graphs into the Graphs subdirectory
			foreach(Altaxo.Graph.GraphDocument graph in this.m_GraphSet)
			{
				ZipEntry ZipEntry = new ZipEntry("Graphs/"+graph.Name+".xml");
				zippedStream.PutNextEntry(ZipEntry);
				zippedStream.SetLevel(0);
				info.BeginWriting(zippedStream);
				info.AddValue("Graph",graph);
				info.EndWriting();
			}

			// third, we save all TableLayouts into the TableLayouts subdirectory
			foreach(Altaxo.Worksheet.WorksheetLayout layout in this.m_TableLayoutList)
			{
				ZipEntry ZipEntry = new ZipEntry("TableLayouts/"+layout.Name+".xml");
				zippedStream.PutNextEntry(ZipEntry);
				zippedStream.SetLevel(0);
				info.BeginWriting(zippedStream);
				info.AddValue("WorksheetLayout",layout);
				info.EndWriting();
			}
			
		}


		public void RestoreFromZippedFile(ZipFile zipFile, Altaxo.Serialization.Xml.XmlStreamDeserializationInfo info)
		{
			foreach(ZipEntry zipEntry in zipFile)
			{
				if(!zipEntry.IsDirectory && zipEntry.Name.StartsWith("Tables/"))
				{
					System.IO.Stream zipinpstream =zipFile.GetInputStream(zipEntry);
					info.BeginReading(zipinpstream);
					object readedobject = info.GetValue("Table",this);
					if(readedobject is Altaxo.Data.DataTable)
						this.m_DataSet.Add((Altaxo.Data.DataTable)readedobject);
					info.EndReading();
				
				}
				else if(!zipEntry.IsDirectory && zipEntry.Name.StartsWith("Graphs/"))
				{
					System.IO.Stream zipinpstream =zipFile.GetInputStream(zipEntry);
					info.BeginReading(zipinpstream);
					object readedobject = info.GetValue("Graph",this);
					if(readedobject is Altaxo.Graph.GraphDocument)
						this.m_GraphSet.Add((Altaxo.Graph.GraphDocument)readedobject);
					info.EndReading();
					
				}
				else if(!zipEntry.IsDirectory && zipEntry.Name.StartsWith("TableLayouts/"))
				{
					System.IO.Stream zipinpstream =zipFile.GetInputStream(zipEntry);
					info.BeginReading(zipinpstream);
					object readedobject = info.GetValue("WorksheetLayout",this);
					if(readedobject is Altaxo.Worksheet.WorksheetLayout)
						this.m_TableLayoutList.Add((Altaxo.Worksheet.WorksheetLayout)readedobject);
					info.EndReading();
					
				}
			}

			info.AnnounceDeserializationEnd(this);
		}


		#endregion

		
		public Altaxo.Data.DataTableCollection DataTableCollection
		{
			get { return m_DataSet; }
		}
		public Altaxo.Graph.GraphDocumentCollection GraphDocumentCollection
		{
			get { return m_GraphSet; }
		}

		public Altaxo.Worksheet.WorksheetLayoutCollection TableLayouts
		{
			get { return this.m_TableLayoutList; }
		}

		protected virtual void OnDirtyChanged()
		{
			if(null!=DirtyChanged)
				DirtyChanged(this,EventArgs.Empty);
		}

		public bool IsDirty
		{
			get { return m_IsDirty; }
			set 
			{
				bool oldValue = m_IsDirty;
				m_IsDirty = value;
				if(oldValue!=m_IsDirty)
				{
					OnDirtyChanged();
				}

			}
		}

		public void OnChildChanged(object sender, EventArgs e)
		{
			this.IsDirty = true;
		}

		public Altaxo.Data.DataTable CreateNewTable(string worksheetName, bool bCreateDefaultColumns)
		{
			Altaxo.Data.DataTable dt1 = new Altaxo.Data.DataTable(worksheetName);


			if(bCreateDefaultColumns)
			{
				dt1.DataColumns.Add(new Altaxo.Data.DoubleColumn(),"A",Altaxo.Data.ColumnKind.X);
				dt1.DataColumns.Add(new Altaxo.Data.DoubleColumn(),"B");
			}

			DataTableCollection.Add(dt1);

			return dt1;
		}

		public Altaxo.Graph.GraphDocument CreateNewGraphDocument()
		{
			Altaxo.Graph.GraphDocument doc = new Altaxo.Graph.GraphDocument();
			GraphDocumentCollection.Add(doc);

			return doc;
		}

	


		public Altaxo.Worksheet.WorksheetLayout CreateNewTableLayout(Altaxo.Data.DataTable table)
		{
			Altaxo.Worksheet.WorksheetLayout layout = new Altaxo.Worksheet.WorksheetLayout(table);
			this.m_TableLayoutList.Add(layout);
			return layout;
		}

		public object GetChildObjectNamed(string name)
		{
			switch(name)
			{
				case "Tables":
					return this.m_DataSet;
				case "Graphs":
					return this.m_GraphSet;
				case "TableLayouts":
					return this.m_TableLayoutList;
			}
			return null;
		}

		public string GetNameOfChildObject(object o)
		{
			if(null==o)
				return null;
			else if(o.Equals(this.m_DataSet))
				return "Tables";
			else if(o.Equals(this.m_GraphSet))
				return "Graphs";
			else
				return null;
		}
	
	}
}
