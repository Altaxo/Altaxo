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

namespace Altaxo
{
	/// <summary>
	/// Summary description for AltaxoDocument.
	/// </summary>
	[SerializationSurrogate(0,typeof(AltaxoDocument.SerializationSurrogate0))]
	[SerializationVersion(0,"Initial version of the main document only contains m_DataSet")]
	public class AltaxoDocument : IDeserializationCallback
	{
		protected Altaxo.Data.DataSet m_DataSet = null; // The root of all the data
		protected System.Collections.ArrayList m_Worksheets;
		/// <summary>The list of GraphForms for the document.</summary>
		protected System.Collections.ArrayList m_GraphForms;
	
		[NonSerialized]
		protected bool m_IsDirty=false;
		[NonSerialized]
		private bool m_DeserializationFinished=false;

		public AltaxoDocument()
		{
			m_DataSet = new Altaxo.Data.DataSet(this);
			m_Worksheets = new System.Collections.ArrayList();
			m_GraphForms = new System.Collections.ArrayList();
		}

		public class SerializationSurrogate0 : System.Runtime.Serialization.ISerializationSurrogate
		{
			public void GetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context	)
			{
				AltaxoDocument s = (AltaxoDocument)obj;
				info.AddValue("DataSet",s.m_DataSet);
				info.AddValue("Worksheets",s.m_Worksheets);
				info.AddValue("GraphForms",s.m_GraphForms);
			}
			public object SetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context,System.Runtime.Serialization.ISurrogateSelector selector)
			{
				AltaxoDocument s = (AltaxoDocument)obj;
				s.m_DataSet = (Altaxo.Data.DataSet)info.GetValue("DataSet",typeof(Altaxo.Data.DataSet));
				// s.tstObj    = (AltaxoTestObject02)info.GetValue("TstObj",typeof(AltaxoTestObject02));
				s.m_Worksheets = (System.Collections.ArrayList)info.GetValue("Worksheets",typeof(System.Collections.ArrayList));
				s.m_GraphForms = (System.Collections.ArrayList)info.GetValue("GraphForms",typeof(System.Collections.ArrayList));
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

			
				for(int i=0;i<m_Worksheets.Count;i++)
				{
					m_Worksheets[i] = ((IDeserializationSubstitute)m_Worksheets[i]).GetRealObject(App.CurrentApplication);
					((System.Windows.Forms.Form)m_Worksheets[i]).Show();
				}
				for(int i=0;i<m_GraphForms.Count;i++)
				{
					m_GraphForms[i] = ((IDeserializationSubstitute)m_GraphForms[i]).GetRealObject(App.CurrentApplication);
					((System.Windows.Forms.Form)m_GraphForms[i]).Show();
				}
			}
		}

		public void RestoreWindowsAfterDeserialization()
		{
		}

		public AltaxoDocument(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			m_DataSet = (Altaxo.Data.DataSet)(info.GetValue("DataSet",typeof(Altaxo.Data.DataSet)));
		}

		public void GetObjectData(
			System.Runtime.Serialization.SerializationInfo info,
			System.Runtime.Serialization.StreamingContext context
			)
		{
			info.AddValue("DataSet",m_DataSet);
		}
		
		public Altaxo.Data.DataSet DataSet
		{
			get { return m_DataSet; }
		}

		public void OnDirtySet(object sender)
		{
			m_IsDirty=true;
		}

		public Altaxo.Worksheet.Worksheet CreateNewWorksheet(string worksheetName, System.Windows.Forms.Form parent, bool bCreateDefaultColumns)
		{
			Altaxo.Data.DataTable dt1 = new Altaxo.Data.DataTable(worksheetName);


			if(bCreateDefaultColumns)
			{
				Altaxo.Data.DoubleColumn colA = new Altaxo.Data.DoubleColumn("A");
				colA.Kind = Data.ColumnKind.X;

				Altaxo.Data.DoubleColumn colB = new Altaxo.Data.DoubleColumn("B");

				dt1.Add(colA);
				dt1.Add(colB);
			}

			DataSet.Add(dt1);

			Altaxo.Worksheet.Worksheet form1= new Altaxo.Worksheet.Worksheet(parent,this,dt1);
			form1.Text = worksheetName;
			m_Worksheets.Add(form1);
			return form1;
		}

		public Altaxo.Graph.IGraphView CreateNewGraph(System.Windows.Forms.Form parent)
		{
		//	Altaxo.Graph.GraphForm frm = new Altaxo.Graph.GraphForm(parent,this);
		//	m_GraphForms.Add(frm);
			
			Altaxo.Graph.GraphController ctrl = new Altaxo.Graph.GraphController(new Altaxo.Graph.GraphView(parent,null));
			m_GraphForms.Add(ctrl.View.Form);
			return ctrl.View;
		}

		/// <summary>This will remove the GraphForm <paramref>frm</paramref> from the graph forms collection.</summary>
		/// <param name="frm">The GraphForm to remove.</param>
		/// <remarks>No exception is thrown if the Form frm is not a member of the graph forms collection.</remarks>
		public void RemoveGraph(System.Windows.Forms.Form frm)
		{
			if(m_GraphForms.Contains(frm))
				m_GraphForms.Remove(frm);
		}

		public Altaxo.Worksheet.Worksheet CreateNewWorksheet(System.Windows.Forms.Form parent, bool bCreateDefaultColumns)
		{
			return CreateNewWorksheet(this.DataSet.FindNewTableName(),parent,bCreateDefaultColumns);
		}

		public Altaxo.Worksheet.Worksheet CreateNewWorksheet(System.Windows.Forms.Form parent)
		{
			return CreateNewWorksheet(this.DataSet.FindNewTableName(),parent,true);
		}

	}
}
