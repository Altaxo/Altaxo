using System;

namespace Altaxo.Worksheet
{
	/// <summary>
	/// Summary description for TableLayoutList.
	/// </summary>
	public class TableLayoutList : Main.IDocumentNode
	{
		protected object m_DocumentParent;
		protected System.Collections.Hashtable m_TableLayouts;



		#region Serialization

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(TableLayoutList),0)]
			public class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				TableLayoutList s = (TableLayoutList)obj;

				info.CreateArray("TableLayoutArray",s.m_TableLayouts.Count);
				foreach(object style in s.m_TableLayouts.Values)
					info.AddValue("TableLayout",style);
				info.CommitArray();
			}
			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				
				TableLayoutList s = null!=o ? (TableLayoutList)o : new TableLayoutList();

				int count;
				count = info.OpenArray(); // TableLayouts
				
				for(int i=0;i<count;i++)
				{
					TableLayout style = (TableLayout)info.GetValue("TableLayout",s);
					s.m_TableLayouts.Add(style.Guid,style);
				}
				info.CloseArray(count);

				return s;
			}
		}
 
		
		#endregion
		
		
		public TableLayoutList()
		{
			m_TableLayouts = new System.Collections.Hashtable();
		}

		public TableLayoutList(object documentParent)
			: this()
		{
			m_DocumentParent = documentParent;
		}

		public TableLayout this[System.Guid guid]
		{
			get { return (TableLayout)m_TableLayouts[guid]; }		
		}

		public void Add(TableLayout layout)
		{
			layout.ParentObject = this;
			m_TableLayouts[layout.Guid] = layout;
		}

		#region "ICollection support"

		public void CopyTo(Array array, int index)
		{
			this.m_TableLayouts.Values.CopyTo(array,index);
		}

		public int Count 
		{
			get { return this.m_TableLayouts.Count; }
		}

		public bool IsSynchronized
		{
			get { return this.m_TableLayouts.IsSynchronized; }
		}

		public object SyncRoot
		{
			get { return this.m_TableLayouts.SyncRoot; }
		}

		public System.Collections.IEnumerator GetEnumerator()
		{
			return this.m_TableLayouts.Values.GetEnumerator();
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
				return "TableLayouts";
			}
		}

		#endregion
	}
}
