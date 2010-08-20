#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2007 Dr. Dirk Lellinger
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
#endregion

using System;
using System.Collections.Generic;

namespace Altaxo.Worksheet
{
	using Altaxo.Data;
  /// <summary>
  /// Stores the layout of a table to be shown in a WorksheetView.
  /// </summary>
  public class WorksheetLayout : Main.IDocumentNode
  {

    #region Member variables


    /// <summary>
    /// The parent node in the document hierarchy.
    /// </summary>
    protected object _documentParent;

    /// <summary>
    /// The unique identifier of this object.
    /// </summary>
    protected System.Guid _guid;

    /// <summary>
    /// The data table this layout is for.
    /// </summary>
    protected Altaxo.Data.DataTable _dataTable;

    /// <summary>
    /// defaultColumnsStyles stores the default column Styles in a dictionary
    /// the key for the hash table is the Type of the ColumnStyle
    /// </summary>
    protected Dictionary<System.Type, ColumnStyle> _defaultDataColumnStyles;

    /// <summary>
    /// Stores the default property column Styles in a Hashtable
    /// the key for the hash table is the Type of the ColumnStyle
    /// </summary>
		protected Dictionary<System.Type, ColumnStyle> _defaultPropertyColumnStyles;

    /// <summary>
    /// m_ColumnStyles stores the column styles for each data column individually,
    /// key is the data column itself.
    /// There is no need to store a column style here if the column is styled as default,
    /// instead the defaultColumnStyle is used in this case
    /// </summary>
    protected ColumnDictionary _dataColumnStyles;


		protected ColumnDictionary _propertyColumnStyles;


    /// <summary>
    /// The style of the row header. This is the leftmost column that shows usually the row number.
    /// </summary>
    protected RowHeaderStyle _rowHeaderStyle; // holds the style of the row header (leftmost column of data grid)
  
    /// <summary>
    /// The style of the column header. This is the upmost row that shows the name of the columns.
    /// </summary>
    protected ColumnHeaderStyle _columnHeaderStyle; // the style of the column header (uppermost row of datagrid)
  
    /// <summary>
    /// The style of the property column header. This is the leftmost column in the left of the property columns,
    /// that shows the names of the property columns.
    /// </summary>
    protected ColumnHeaderStyle _propertyColumnHeaderStyle;
    
    /// <summary>
    /// The visibility of the property columns in the view. If true, the property columns are shown in the view.
    /// </summary>
    protected bool _doShowPropertyColumns;
    
    
    #endregion

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(WorksheetLayout),0)]
      class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      protected WorksheetLayout                  _worksheetLayout;
      protected System.Collections.Hashtable _colStyles;
      protected Main.DocumentPath  _pathToTable;

      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        WorksheetLayout s = (WorksheetLayout)obj;

        info.AddValue("Guid",System.Xml.XmlConvert.ToString(s._guid));
        info.AddValue("Table",Main.DocumentPath.GetAbsolutePath(s._dataTable));
        info.AddValue("RowHeaderStyle",s._rowHeaderStyle);
        info.AddValue("ColumnHeaderStyle",s._columnHeaderStyle);
        info.AddValue("PropertyColumnHeaderStyle",s._propertyColumnHeaderStyle);

        info.CreateArray("DefaultColumnStyles",s._defaultDataColumnStyles.Values.Count);
        foreach(object style in s._defaultDataColumnStyles.Values)
          info.AddValue("DefaultColumnStyle",style);
        info.CommitArray();

        info.CreateArray("ColumnStyles",s._dataColumnStyles.Count);
        foreach(KeyValuePair<DataColumn, ColumnStyle> dictentry in s._dataColumnStyles)
        {
          info.CreateElement("e");
					info.AddValue("Column", Main.DocumentPath.GetAbsolutePath(dictentry.Key));
          info.AddValue("Style",dictentry.Value);         
          info.CommitElement(); // "e"
        }
        info.CommitArray();
      
      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        WorksheetLayout s = null != o ? (WorksheetLayout)o : new WorksheetLayout();
        Deserialize(s, info, parent);
        return s;
      }

        protected virtual void Deserialize(WorksheetLayout s, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
        {
        

        XmlSerializationSurrogate0 surr = new XmlSerializationSurrogate0();
        surr._colStyles = new System.Collections.Hashtable();
        surr._worksheetLayout = s;
        info.DeserializationFinished += new Altaxo.Serialization.Xml.XmlDeserializationCallbackEventHandler(surr.EhDeserializationFinished);


        s._guid = System.Xml.XmlConvert.ToGuid(info.GetString("Guid"));
        surr._pathToTable = (Main.DocumentPath)info.GetValue("Table",s);
        s._rowHeaderStyle = (RowHeaderStyle)info.GetValue("RowHeaderStyle" , s);  
        s._columnHeaderStyle = (ColumnHeaderStyle)info.GetValue("ColumnHeaderStyle",s);  
        s._propertyColumnHeaderStyle = (ColumnHeaderStyle)info.GetValue("PropertyColumnHeaderStyle",s);  


        int count;
        count = info.OpenArray(); // DefaultColumnStyles
        
        for(int i=0;i<count;i++)
        {
          var defstyle = (ColumnStyle)info.GetValue("DefaultColumnStyle",s);
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
            surr._colStyles.Add(key,val);
            info.CloseElement();
          }
        }
        info.CloseArray(count);
      }

      public void EhDeserializationFinished(Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object documentRoot)
      {
        if(this._pathToTable!=null)
        {
          object table = Main.DocumentPath.GetObject(this._pathToTable,this._worksheetLayout,documentRoot);
          if(table is Altaxo.Data.DataTable)
          {
            this._worksheetLayout._dataTable = (Altaxo.Data.DataTable)table;
            this._pathToTable = null;
          }
        }

        System.Collections.ArrayList resolvedStyles = new System.Collections.ArrayList();
        foreach(System.Collections.DictionaryEntry entry in this._colStyles)
        {
          object resolvedobj = Main.DocumentPath.GetObject((Main.DocumentPath)entry.Key,_worksheetLayout, documentRoot);
          if(null!=resolvedobj)
          {
            _worksheetLayout.DataColumnStyles.Add((DataColumn)resolvedobj,(ColumnStyle)entry.Value);
            resolvedStyles.Add(entry.Key);
          }
        }

        foreach(object resstyle in resolvedStyles)
          _colStyles.Remove(resstyle);


        // if all columns have resolved, we can close the event link
        if(_colStyles.Count==0 && this._pathToTable==null)
          info.DeserializationFinished -= new Altaxo.Serialization.Xml.XmlDeserializationCallbackEventHandler(this.EhDeserializationFinished);
      }
    }


    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(WorksheetLayout), 1)]
    class XmlSerializationSurrogate1 : XmlSerializationSurrogate0
    {
      public override void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        base.Serialize(obj, info);

        WorksheetLayout s = (WorksheetLayout)obj;
        info.CreateArray("DefaultPropertyColumnStyles", s._defaultPropertyColumnStyles.Values.Count);
        foreach (object style in s._defaultPropertyColumnStyles.Values)
          info.AddValue("DefaultPropertyColumnStyle", style);
        info.CommitArray();
      }


      protected override void Deserialize(WorksheetLayout s, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        base.Deserialize(s, info, parent);

        int count = info.OpenArray(); // DefaultPropertyColumnStyles
        for (int i = 0; i < count; i++)
        {
          var defstyle = (ColumnStyle)info.GetValue("DefaultPropertyColumnStyle", s);
          s.DefaultPropertyColumnStyles.Add(defstyle.GetType(), defstyle);
        }
        info.CloseArray(count);
      }
    }

    #endregion

    #region Constructors

    protected WorksheetLayout()
    {
      _guid = System.Guid.NewGuid();

      // defaultColumnsStyles stores the default column Styles in a Hashtable
			_defaultDataColumnStyles = new Dictionary<System.Type, ColumnStyle>();

      // defaultPropertyColumnsStyles stores the default property column Styles in a Hashtable
			_defaultPropertyColumnStyles = new Dictionary<System.Type, ColumnStyle>();

      // m_ColumnStyles stores the column styles for each data column individually,
      _dataColumnStyles = new ColumnDictionary(_defaultDataColumnStyles);

			_propertyColumnStyles = new ColumnDictionary(_defaultPropertyColumnStyles);

      // The style of the row header. This is the leftmost column that shows usually the row number.
      _rowHeaderStyle = new RowHeaderStyle(); // holds the style of the row header (leftmost column of data grid)
  
      // The style of the column header. This is the upmost row that shows the name of the columns.
      _columnHeaderStyle = new ColumnHeaderStyle(); // the style of the column header (uppermost row of datagrid)
  
      // The style of the property column header. This is the leftmost column in the left of the property columns,
      _propertyColumnHeaderStyle = new ColumnHeaderStyle();


      this._doShowPropertyColumns = true;
    }

    


    public WorksheetLayout(Altaxo.Data.DataTable table)
      : this()
    {
      _dataTable = table;
    }

    #endregion

    #region Properties

    public System.Guid Guid
    {
      get { return _guid; }
    }

    /// <summary>
    /// Don't use this! Only intended for use by WorksheetLayoutCollection.
    /// </summary>
    protected internal void NewGuid()
    {
      this._guid = System.Guid.NewGuid();
    }
    
    public Altaxo.Data.DataTable DataTable
    {
      get { return _dataTable; }
      set { _dataTable = value; }
    }

    public Dictionary<System.Type, ColumnStyle> DefaultColumnStyles
    {
      get { return _defaultDataColumnStyles; }
    }

		public Dictionary<System.Type, ColumnStyle> DefaultPropertyColumnStyles
    {
      get { return _defaultPropertyColumnStyles; }
    }

    public IDictionary<Data.DataColumn, ColumnStyle> DataColumnStyles
    {
      get { return _dataColumnStyles; }
    }

		public ColumnStyle GetDataColumnStyle(int i)
		{
			return _dataColumnStyles[_dataTable.DataColumns[i]];
		}

		public IDictionary<Data.DataColumn, ColumnStyle> PropertyColumnStyles
		{
			get { return _propertyColumnStyles; }
		}

		public ColumnStyle GetPropertyColumnStyle(int i)
		{
			return _propertyColumnStyles[_dataTable.PropertyColumns[i]];
		}

    public RowHeaderStyle RowHeaderStyle
    {
      get { return _rowHeaderStyle; }
    }

    public ColumnHeaderStyle ColumnHeaderStyle
    {
      get { return _columnHeaderStyle; }
    }

    public ColumnHeaderStyle PropertyColumnHeaderStyle
    {
      get { return _propertyColumnHeaderStyle; }
    }

    public bool ShowPropertyColumns
    {
      get { return _doShowPropertyColumns; }
      set { _doShowPropertyColumns = value; }
    }
    

    

    #endregion

    #region IDocumentNode Members

    public object ParentObject
    {
      get
      {
        return _documentParent;
      }
      set
      {
        _documentParent = value;
      }
    }

    public string Name
    {
      get
      {
        return System.Xml.XmlConvert.ToString(_guid);
      }
    }

    #endregion

    #region Dictionaries


    protected class ColumnDictionary : IDictionary<Data.DataColumn, ColumnStyle>
    {
			Dictionary<Data.DataColumn, ColumnStyle> _hash;
			Dictionary<System.Type, ColumnStyle> _defaultColumnStyles;


			public ColumnDictionary(Dictionary<System.Type, ColumnStyle> defaultColumnStyles)
			{
				_defaultColumnStyles = defaultColumnStyles;
				_hash = new Dictionary<Altaxo.Data.DataColumn, ColumnStyle>();
			}

			void AttachKey(DataColumn key)
			{
				key.TunneledEvent += EhKey_TunneledEvent;
			}

			void DetachKey(DataColumn key)
		{
			key.TunneledEvent -= EhKey_TunneledEvent;
		}

      void EhKey_TunneledEvent(object sender, object source, Main.TunnelingEventArgs e)
      {
				if (e is Main.DisposeEventArgs)
				{
					var c = source as DataColumn;
					if (c != null)
						this.Remove(c); // do not use direct remove, as the event handler has to be detached also
				}
      }

			#region IDictionary<DataColumn,ColumnStyle> Members

			public void Add(DataColumn key, ColumnStyle value)
			{
        if (null == value)
          throw new ArgumentNullException("value");

				_hash.Add(key, value);
				AttachKey(key);
			}

			public bool ContainsKey(DataColumn key)
			{
				return _hash.ContainsKey(key);
			}

			public ICollection<DataColumn> Keys
			{
				get { return _hash.Keys; }
			}

			public bool Remove(DataColumn key)
			{
				bool result = _hash.Remove(key);
				if (result)
					DetachKey(key);
				return result;
			}

			public bool TryGetValue(DataColumn key, out ColumnStyle value)
			{
				return _hash.TryGetValue(key, out value);
			}

			public ICollection<ColumnStyle> Values
			{
				get { return _hash.Values; }
			}

			public ColumnStyle this[DataColumn key]
			{
				get
				{
					ColumnStyle colstyle;
					// first look at the column styles hash table, column itself is the key
					if (_hash.TryGetValue(key, out colstyle))
						return colstyle;

					if(_defaultColumnStyles.TryGetValue(key.GetType(), out colstyle))
						return colstyle;

					// second look to the defaultcolumnstyles hash table, key is the type of the column style

					System.Type searchstyletype = key.GetColumnStyleType();
					if (null == searchstyletype)
					{
						throw new ApplicationException("Error: Column of type +" + key.GetType() + " returns no associated ColumnStyleType, you have to overload the method GetColumnStyleType.");
					}
					else
					{
						// if not successfull yet, we will create a new defaultColumnStyle
						colstyle = (ColumnStyle)Activator.CreateInstance(searchstyletype);
						_defaultColumnStyles.Add(key.GetType(), colstyle);
						return colstyle;
					}
				}
				set
				{
          if (null == value)
            throw new ArgumentNullException("value");

					bool hadOldValue = _hash.ContainsKey(key);
					_hash[key] = value;
					if (!hadOldValue)
						AttachKey(key);
				}
			}



		

			#endregion

			#region ICollection<KeyValuePair<DataColumn,ColumnStyle>> Members

			public void Add(KeyValuePair<DataColumn, ColumnStyle> item)
			{
				((ICollection<KeyValuePair<DataColumn,ColumnStyle>>)_hash).Add(item);
				AttachKey(item.Key);
			}

			public void Clear()
			{
				foreach (DataColumn c in _hash.Keys)
					DetachKey(c);

				_hash.Clear();
			}

			public bool Contains(KeyValuePair<DataColumn, ColumnStyle> item)
			{
				return ((ICollection<KeyValuePair<DataColumn, ColumnStyle>>)_hash).Contains(item);
			}

			public void CopyTo(KeyValuePair<DataColumn, ColumnStyle>[] array, int arrayIndex)
			{
				((ICollection<KeyValuePair<DataColumn, ColumnStyle>>)_hash).CopyTo(array, arrayIndex);
			}

			public int Count
			{
				get { return _hash.Count; }
			}

			public bool IsReadOnly
			{
				get { return ((ICollection<KeyValuePair<DataColumn, ColumnStyle>>)_hash).IsReadOnly; }
			}

			public bool Remove(KeyValuePair<DataColumn, ColumnStyle> item)
			{
				bool result = ((ICollection<KeyValuePair<DataColumn, ColumnStyle>>)_hash).Remove(item);
				if (result)
					DetachKey(item.Key);
				return result;
			}

			#endregion

			#region IEnumerable<KeyValuePair<DataColumn,ColumnStyle>> Members

			public IEnumerator<KeyValuePair<DataColumn, ColumnStyle>> GetEnumerator()
			{
				return ((ICollection<KeyValuePair<DataColumn, ColumnStyle>>)_hash).GetEnumerator();
			}

			#endregion

			#region IEnumerable Members

			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
			{
				return ((ICollection<KeyValuePair<DataColumn, ColumnStyle>>)_hash).GetEnumerator();
			}

			#endregion
		}

    #endregion
  }
}
