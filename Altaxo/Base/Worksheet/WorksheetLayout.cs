using System;

namespace Altaxo.Worksheet
{
	/// <summary>
	/// Stores the layout of a table to be shown in a WorksheetView.
	/// </summary>
	public class WorksheetLayout : Main.IDocumentNode
	{

		#region Member variables


		/// <summary>
		/// The parent node in the document hierarchy.
		/// </summary>
		protected object m_DocumentParent;

		/// <summary>
		/// The view controller that owns this Layout.
		/// </summary>
		protected object m_OwnerWorksheet;

		/// <summary>
		/// The unique identifier of this object.
		/// </summary>
		protected System.Guid m_Guid;

		/// <summary>
		/// The data table this layout is for.
		/// </summary>
		protected Altaxo.Data.DataTable m_DataTable;

		/// <summary>
		/// defaultColumnsStyles stores the default column Styles in a Hashtable
		/// the key for the hash table is the Type of the ColumnStyle
		/// </summary>
		protected System.Collections.Hashtable m_DefaultColumnStyles;

		/// <summary>
		/// m_ColumnStyles stores the column styles for each data column individually,
		/// key is the data column itself.
		/// There is no need to store a column style here if the column is styled as default,
		/// instead the defaultColumnStyle is used in this case
		/// </summary>
		protected internal System.Collections.Hashtable m_ColumnStyles;


		/// <summary>
		/// The style of the row header. This is the leftmost column that shows usually the row number.
		/// </summary>
		protected RowHeaderStyle m_RowHeaderStyle; // holds the style of the row header (leftmost column of data grid)
	
		/// <summary>
		/// The style of the column header. This is the upmost row that shows the name of the columns.
		/// </summary>
		protected ColumnHeaderStyle m_ColumnHeaderStyle; // the style of the column header (uppermost row of datagrid)
	
		/// <summary>
		/// The style of the property column header. This is the leftmost column in the left of the property columns,
		/// that shows the names of the property columns.
		/// </summary>
		protected ColumnHeaderStyle m_PropertyColumnHeaderStyle;
	
		
		/// <summary>
		/// The visibility of the property columns in the view. If true, the property columns are shown in the view.
		/// </summary>
		protected bool m_ShowPropertyColumns; // are the property columns visible?
		
		
		#endregion


		#region Serialization

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(WorksheetLayout),0)]
			public class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			protected WorksheetLayout                  m_TableLayout;
			protected System.Collections.Hashtable m_ColStyles;
			protected Main.DocumentPath  m_PathToTable;

			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				WorksheetLayout s = (WorksheetLayout)obj;

				info.AddValue("Guid",System.Xml.XmlConvert.ToString(s.m_Guid));
				info.AddValue("Table",Main.DocumentPath.GetAbsolutePath(s.m_DataTable));
				info.AddValue("RowHeaderStyle",s.m_RowHeaderStyle);
				info.AddValue("ColumnHeaderStyle",s.m_ColumnHeaderStyle);
				info.AddValue("PropertyColumnHeaderStyle",s.m_PropertyColumnHeaderStyle);

				info.CreateArray("DefaultColumnStyles",s.m_DefaultColumnStyles.Values.Count);
				foreach(object style in s.m_DefaultColumnStyles.Values)
					info.AddValue("DefaultColumnStyle",style);
				info.CommitArray();

				info.CreateArray("ColumnStyles",s.m_ColumnStyles.Count);
				foreach(System.Collections.DictionaryEntry dictentry in s.m_ColumnStyles)
				{
					info.CreateElement("e");
					
					Main.IDocumentNode col = (Main.IDocumentNode)dictentry.Key;
					info.AddValue("Column", Main.DocumentPath.GetAbsolutePath(col));
					info.AddValue("Style",dictentry.Value);					
					
					info.CommitElement(); // "e"
				}
				info.CommitArray();
			
			}
			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				
				WorksheetLayout s = null!=o ? (WorksheetLayout)o : new WorksheetLayout();

				XmlSerializationSurrogate0 surr = new XmlSerializationSurrogate0();
				surr.m_ColStyles = new System.Collections.Hashtable();
				surr.m_TableLayout = s;
				info.DeserializationFinished += new Altaxo.Serialization.Xml.XmlDeserializationCallbackEventHandler(surr.EhDeserializationFinished);


				s.m_Guid = System.Xml.XmlConvert.ToGuid(info.GetString("Guid"));
				surr.m_PathToTable = (Main.DocumentPath)info.GetValue("Table",s);
				s.m_RowHeaderStyle = (RowHeaderStyle)info.GetValue("RowHeaderStyle" , s);  
				s.m_ColumnHeaderStyle = (ColumnHeaderStyle)info.GetValue("ColumnHeaderStyle",s);  
				s.m_PropertyColumnHeaderStyle = (ColumnHeaderStyle)info.GetValue("PropertyColumnHeaderStyle",s);  


				int count;
				count = info.OpenArray(); // DefaultColumnStyles
				
				for(int i=0;i<count;i++)
				{
					object defstyle = info.GetValue("DefaultColumnStyle",s);
					s.DefaultColumnStyles.Add(defstyle.GetType(), defstyle);
				}
				info.CloseArray(count);
			

				// deserialize the columnstyles
				// this must be deserialized in a new instance of this surrogate, since we can not resolve it immediately
				count = info.OpenArray();
				if(count>0)
				{
						for(int i=0;i<count;i++)
					{
						info.OpenElement(); // "e"
						Main.DocumentPath key = (Main.DocumentPath)info.GetValue("Column",s);
						object val = info.GetValue("Style",s);
						surr.m_ColStyles.Add(key,val);
						info.CloseElement();
					}
				}
				info.CloseArray(count);

				return s;
			}

			public void EhDeserializationFinished(Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object documentRoot)
			{
				if(this.m_PathToTable!=null)
				{
					object table = Main.DocumentPath.GetObject(this.m_PathToTable,this.m_TableLayout,documentRoot);
					if(table is Altaxo.Data.DataTable)
					{
						this.m_TableLayout.m_DataTable = (Altaxo.Data.DataTable)table;
						this.m_PathToTable = null;
					}
				}

				System.Collections.ArrayList resolvedStyles = new System.Collections.ArrayList();
				foreach(System.Collections.DictionaryEntry entry in this.m_ColStyles)
				{
					object resolvedobj = Main.DocumentPath.GetObject((Main.DocumentPath)entry.Key,m_TableLayout, documentRoot);
					if(null!=resolvedobj)
					{
						m_TableLayout.ColumnStyles.Add(resolvedobj,entry.Value);
						resolvedStyles.Add(entry.Key);
					}
				}

				foreach(object resstyle in resolvedStyles)
					m_ColStyles.Remove(resstyle);


				// if all columns have resolved, we can close the event link
				if(m_ColStyles.Count==0 && this.m_PathToTable==null)
					info.DeserializationFinished -= new Altaxo.Serialization.Xml.XmlDeserializationCallbackEventHandler(this.EhDeserializationFinished);
			}
		}
 
		
		#endregion

		#region Constructors

		protected WorksheetLayout()
		{
			m_Guid = System.Guid.NewGuid();

			// defaultColumnsStyles stores the default column Styles in a Hashtable
			m_DefaultColumnStyles = new System.Collections.Hashtable();

			// m_ColumnStyles stores the column styles for each data column individually,
			m_ColumnStyles = new System.Collections.Hashtable();


			// The style of the row header. This is the leftmost column that shows usually the row number.
			m_RowHeaderStyle = new RowHeaderStyle(); // holds the style of the row header (leftmost column of data grid)
	
			// The style of the column header. This is the upmost row that shows the name of the columns.
			m_ColumnHeaderStyle = new ColumnHeaderStyle(); // the style of the column header (uppermost row of datagrid)
	
			// The style of the property column header. This is the leftmost column in the left of the property columns,
			m_PropertyColumnHeaderStyle = new ColumnHeaderStyle();


			this.m_ShowPropertyColumns = true;
		}

		public WorksheetLayout(Altaxo.Data.DataTable table)
			: this()
		{
			m_DataTable = table;
		}

		#endregion

		#region Properties

		public System.Guid Guid
		{
			get { return m_Guid; }
		}

		public Altaxo.Data.DataTable DataTable
		{
			get { return m_DataTable; }
		}

		public System.Collections.Hashtable DefaultColumnStyles
		{
			get { return m_DefaultColumnStyles; }
		}

		public System.Collections.Hashtable ColumnStyles
		{
			get { return m_ColumnStyles; }
		}

		public RowHeaderStyle RowHeaderStyle
		{
			get { return m_RowHeaderStyle; }
		}

		public ColumnHeaderStyle ColumnHeaderStyle
		{
			get { return m_ColumnHeaderStyle; }
		}

		public ColumnHeaderStyle PropertyColumnHeaderStyle
		{
			get { return m_PropertyColumnHeaderStyle; }
		}

		public bool ShowPropertyColumns
		{
			get { return m_ShowPropertyColumns; }
			set { m_ShowPropertyColumns = value; }
		}
		

		

		#endregion

		#region IDocumentNode Members

		public object ParentObject
		{
			get
			{
				return m_DocumentParent;
			}
			set
			{
				m_DocumentParent = value;
			}
		}

		public string Name
		{
			get
			{
				return System.Xml.XmlConvert.ToString(m_Guid);
			}
		}

		#endregion
	}
}
