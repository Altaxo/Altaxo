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
		protected Altaxo.Data.TableSet m_DataSet = null; // The root of all the data
		protected System.Collections.ArrayList m_Worksheets;
		/// <summary>The list of GraphForms for the document.</summary>
		protected System.Collections.ArrayList m_GraphForms;

		[NonSerialized]
		protected bool m_IsDirty=false;
		[NonSerialized]
		private bool m_DeserializationFinished=false;

		public AltaxoDocument()
		{
			m_DataSet = new Altaxo.Data.TableSet(this);
			m_Worksheets = new System.Collections.ArrayList();
			m_GraphForms = new System.Collections.ArrayList();
		}

		public class SerializationSurrogate0 : System.Runtime.Serialization.ISerializationSurrogate
		{
			public void GetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context	)
			{
				AltaxoDocument s = (AltaxoDocument)obj;
				info.AddValue("TableSet",s.m_DataSet);
				info.AddValue("Worksheets",s.m_Worksheets);
				info.AddValue("GraphForms",s.m_GraphForms);
			}
			public object SetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context,System.Runtime.Serialization.ISurrogateSelector selector)
			{
				AltaxoDocument s = (AltaxoDocument)obj;
				s.m_DataSet = (Altaxo.Data.TableSet)info.GetValue("TableSet",typeof(Altaxo.Data.TableSet));
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
					m_Worksheets[i] = ((IDeserializationSubstitute)m_Worksheets[i]).GetRealObject(App.Current.View.Form);
					((System.Windows.Forms.Form)m_Worksheets[i]).Show();
				}
				for(int i=0;i<m_GraphForms.Count;i++)
				{
					m_GraphForms[i] = ((IDeserializationSubstitute)m_GraphForms[i]).GetRealObject(App.Current.View.Form);
					((System.Windows.Forms.Form)m_GraphForms[i]).Show();
				}
			}
		}

		public void RestoreWindowsAfterDeserialization()
		{
		}

		public AltaxoDocument(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			m_DataSet = (Altaxo.Data.TableSet)(info.GetValue("TableSet",typeof(Altaxo.Data.TableSet)));
		}

		public void GetObjectData(
			System.Runtime.Serialization.SerializationInfo info,
			System.Runtime.Serialization.StreamingContext context
			)
		{
			info.AddValue("TableSet",m_DataSet);
		}
		
		public Altaxo.Data.TableSet TableSet
		{
			get { return m_DataSet; }
		}

		public void OnDirtySet(object sender)
		{
			m_IsDirty=true;
		}

		public bool IsDirty
		{
			get { return m_IsDirty; }
			set { m_IsDirty = value; }
		}

		public Altaxo.Data.DataTable CreateNewTable(string worksheetName, bool bCreateDefaultColumns)
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

			TableSet.Add(dt1);

			return dt1;
		}

		public Altaxo.Worksheet.ITableView CreateNewWorksheet(System.Windows.Forms.Form parentForm, string worksheetName, bool bCreateDefaultColumns)
		{
			
			Altaxo.Data.DataTable dt1 = CreateNewTable(worksheetName, bCreateDefaultColumns);
			return CreateNewWorksheet(parentForm,dt1);
		}
	
		public Altaxo.Worksheet.ITableView CreateNewWorksheet(System.Windows.Forms.Form parent, bool bCreateDefaultColumns)
		{
			return CreateNewWorksheet(parent, this.TableSet.FindNewTableName(),bCreateDefaultColumns);
		}

		public Altaxo.Worksheet.ITableView CreateNewWorksheet(System.Windows.Forms.Form parent)
		{
			return CreateNewWorksheet(parent, this.TableSet.FindNewTableName(),false);
		}


		public Altaxo.Worksheet.ITableView CreateNewWorksheet(System.Windows.Forms.Form parentForm, Altaxo.Data.DataTable table)
		{
			Altaxo.Gui.WorkbenchForm form = new Altaxo.Gui.WorkbenchForm(parentForm);
			Altaxo.Worksheet.TableView view = new Altaxo.Worksheet.TableView(form,null);
			form.Controls.Add(view);
			view.Dock = System.Windows.Forms.DockStyle.Fill;
			Altaxo.Worksheet.TableController ctrl = new Altaxo.Worksheet.TableController(view,table);
			ctrl.View.TableViewForm.Text = table.TableName;
			m_Worksheets.Add(ctrl.View.TableViewForm);
			form.Show();
			return ctrl.View;
		}


		public Altaxo.Graph.IGraphView CreateNewGraph(System.Windows.Forms.Form parentForm)
		{
			Altaxo.Gui.WorkbenchForm form = new Altaxo.Gui.WorkbenchForm(parentForm);
			Altaxo.Graph.GraphView view = new Altaxo.Graph.GraphView(form,null);
			form.Controls.Add(view);
			view.Dock = System.Windows.Forms.DockStyle.Fill;
			Altaxo.Graph.GraphController ctrl = new Altaxo.Graph.GraphController(view);
			m_GraphForms.Add(ctrl.View.Form);
			form.Show();
			return ctrl.View;
		}


		public void AddGraph(Altaxo.Graph.IGraphView view)
		{
			m_GraphForms.Add(view.Form);
		}


		/// <summary>This will remove the GraphForm <paramref>frm</paramref> from the graph forms collection.</summary>
		/// <param name="frm">The GraphForm to remove.</param>
		/// <remarks>No exception is thrown if the Form frm is not a member of the graph forms collection.</remarks>
		public void RemoveGraph(System.Windows.Forms.Form frm)
		{
			if(m_GraphForms.Contains(frm))
				m_GraphForms.Remove(frm);
		}

		/// <summary>This will remove the Worksheet <paramref>frm</paramref> from the corresponding forms collection.</summary>
		/// <param name="frm">The Worksheet to remove.</param>
		/// <remarks>No exception is thrown if the Form frm is not a member of the worksheet forms collection.</remarks>
		public void RemoveWorksheet(System.Windows.Forms.Form frm)
		{
			if(m_Worksheets.Contains(frm))
				m_Worksheets.Remove(frm);
		}

	
	}
}
