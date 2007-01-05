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
using Altaxo.Serialization;
using Altaxo.Collections;
using Altaxo.Scripting;


namespace Altaxo.Data
{
  
  [SerializationSurrogate(0,typeof(Altaxo.Data.DataColumnCollection.SerializationSurrogate0))]
  [SerializationVersion(0)]
  public class DataColumnCollection :
    System.Runtime.Serialization.IDeserializationCallback, 
    Altaxo.Main.IDocumentNode,
    IDisposable,
    ICloneable,
    Main.INamedObjectCollection,
    Main.ISuspendable,
    Main.IChildChangedEventSink
  {
    // Types
    //  public delegate void OnDataChanged(Altaxo.Data.DataColumnCollection sender, int nMinCol, int nMaxCol, int nMinRow, int nMaxRow);   // delegate declaration
    //  public delegate void OnDirtySet(Altaxo.Data.DataColumnCollection sender);
    
    #region ChangeEventArgs
    /// <summary>
    /// Used for notifying receivers about what columns in this collection have changed.
    /// </summary>
    public class ChangeEventArgs : System.EventArgs
    {
      protected int m_MinColChanged;
      protected int m_MaxColChanged;
      protected int m_MinRowChanged;
      protected int m_MaxRowChanged;
      protected bool m_RowCountDecreased;

      /// <summary>
      /// Constructor.
      /// </summary>
      /// <param name="columnNumber">The number of the column that has changed.</param>
      /// <param name="minRow">The first number of row that has changed.</param>
      /// <param name="maxRow">The last number of row (plus one) that has changed.</param>
      /// <param name="rowCountDecreased">If true, in one of the columns the row count has decreased, so a complete recalculation of the row count of the collection is neccessary.</param>
      public ChangeEventArgs(int columnNumber, int minRow, int maxRow, bool rowCountDecreased)
      {
        m_MinColChanged = columnNumber;
        m_MaxColChanged = columnNumber;
        m_MinRowChanged = minRow;
        m_MaxRowChanged = maxRow;
        m_RowCountDecreased = rowCountDecreased;
      }

      /// <summary>
      /// Accumulates the change state by adding a change info from a column.
      /// </summary>
      /// <param name="columnNumber">The number of column that has changed.</param>
      /// <param name="minRow">The lowest row number that has changed.</param>
      /// <param name="maxRow">The highest row number that has changed (plus one).</param>
      /// <param name="rowCountDecreased">True if the row count of the column has decreased.</param>
      public void Accumulate(int columnNumber, int minRow, int maxRow, bool rowCountDecreased)
      {
        if(columnNumber<m_MinColChanged)
          m_MinColChanged=columnNumber;
        if((columnNumber+1)>m_MaxColChanged)
          m_MaxColChanged=columnNumber+1;
        if(minRow < m_MinRowChanged)
          m_MinRowChanged=minRow;
        if(maxRow > m_MaxRowChanged)
          m_MaxRowChanged=maxRow;
        m_RowCountDecreased |= rowCountDecreased;
      }

      /// <summary>
      /// Accumulate the change state by adding another change state.
      /// </summary>
      /// <param name="args">The other change state to be added.</param>
      public void Accumulate(ChangeEventArgs args)
      {
        if(args.m_MinColChanged < this.m_MinColChanged)
          this.m_MinColChanged  = args.m_MinColChanged;
        
        if(args.m_MaxColChanged > this.m_MaxColChanged)
          this.m_MaxColChanged  = args.m_MaxColChanged;
        
        if(args.m_MinRowChanged < this.m_MinRowChanged)
          this.m_MinRowChanged = args.m_MinRowChanged;
        
        if(args.MaxRowChanged  > this.m_MaxRowChanged)
          this.m_MaxRowChanged = args.m_MaxRowChanged;
        
        m_RowCountDecreased |= args.m_RowCountDecreased;
      }

      /// <summary>
      /// Creates a change state that reflects the removal of some columns.
      /// </summary>
      /// <param name="firstColumnNumber">The first column number that was removed.</param>
      /// <param name="originalNumberOfColumns">The number of columns in the collection before the removal.</param>
      /// <param name="maxRowCountOfRemovedColumns">The maximum row count of the removed columns.</param>
      /// <returns>The change state that reflects the removal.</returns>
      public static ChangeEventArgs CreateColumnRemoveArgs(int firstColumnNumber, int originalNumberOfColumns, int maxRowCountOfRemovedColumns)
      {
        ChangeEventArgs args = new ChangeEventArgs(firstColumnNumber,0,maxRowCountOfRemovedColumns,true);
        if(originalNumberOfColumns > args.m_MaxColChanged)
          args.m_MaxColChanged = originalNumberOfColumns;
        return args;
      }

      /// <summary>
      /// Creates a change state that reflects the move of some columns.
      /// </summary>
      /// <param name="firstColumnNumber">The first column number that was removed.</param>
      /// <param name="maxColumnNumber">One more than the last affected column.</param>
      /// <returns>The change state that reflects the move.</returns>
      public static ChangeEventArgs CreateColumnMoveArgs(int firstColumnNumber, int maxColumnNumber)
      {
        ChangeEventArgs args = new ChangeEventArgs(firstColumnNumber,0,0,false);
        args.m_MaxColChanged = maxColumnNumber;
        return args;
      }

      /// <summary>
      /// Creates a change state that reflects the move of some rows (in all columns).
      /// </summary>
      /// <param name="numberOfColumns">The number of columns in the table.</param>
      /// <param name="firstRowNumber">The first row number that was affected.</param>
      /// <param name="maxRowNumber">One more than the last affected row number.</param>
      /// <returns>The change state that reflects the move.</returns>
      public static ChangeEventArgs CreateRowMoveArgs(int numberOfColumns, int firstRowNumber, int maxRowNumber)
      {
        ChangeEventArgs args = new ChangeEventArgs(0,firstRowNumber,maxRowNumber,false);
        args.m_MaxColChanged = numberOfColumns;
        return args;
      }

      /// <summary>
      /// Create the change state that reflects the addition of one column.
      /// </summary>
      /// <param name="columnIndex">The index of the added column.</param>
      /// <param name="rowCountOfAddedColumn">The row count of the added column.</param>
      /// <returns>The newly created ChangeEventArgs for this case.</returns>
      public static ChangeEventArgs CreateColumnAddArgs(int columnIndex, int rowCountOfAddedColumn)
      {
        ChangeEventArgs args = new ChangeEventArgs(columnIndex,0,rowCountOfAddedColumn,false);
        return args;
      }

      /// <summary>
      /// Create the change state that reflects the renaming of one column.
      /// </summary>
      /// <param name="columnIndex">The index of the renamed column.</param>
      /// <returns>The newly created ChangeEventArgs for this case.</returns>
      public static ChangeEventArgs CreateColumnRenameArgs(int columnIndex)
      {
        ChangeEventArgs args = new ChangeEventArgs(columnIndex,0,0,false);
        return args;
      }


      /// <summary>
      /// Create the change state that reflects the replace of one column by another (or copying data).
      /// </summary>
      /// <param name="columnIndex">The index of the column to replace.</param>
      /// <param name="oldRowCount">The row count of the old (replaced) column.</param>
      /// <param name="newRowCount">The row count of the new column.</param>
      /// <returns>The newly created ChangeEventArgs for this case.</returns>
      public static ChangeEventArgs CreateColumnCopyOrReplaceArgs(int columnIndex, int oldRowCount, int newRowCount)
      {
        ChangeEventArgs args = new ChangeEventArgs(columnIndex,0,Math.Max(oldRowCount,newRowCount),newRowCount<oldRowCount);
        return args;
      }

      /// <summary>
      /// Returns the lowest column number that has changed.
      /// </summary>
      public int MinColChanged
      {
        get { return m_MinColChanged; }
      }
      /// <summary>
      /// Returns the highest column number that has changed (plus one).
      /// </summary>
      public int MaxColChanged
      {
        get { return m_MaxColChanged; }
      }
      /// <summary>
      /// Returns the lowest row number that has changed.
      /// </summary>
      public int MinRowChanged
      {
        get { return m_MinRowChanged; }
      }
      /// <summary>
      /// Returns the highest row number that has changed (plus one).
      /// </summary>
      public int MaxRowChanged
      {
        get { return m_MaxRowChanged; }
      }
      /// <summary>
      /// Returns whether the row count may have decreased.
      /// </summary>
      public bool RowCountDecreased
      {
        get { return m_RowCountDecreased; }
      }
    }

    #endregion

    #region ColumnInfo
    [Serializable]
      protected class DataColumnInfo : ICloneable
    {
      /// <summary>
      /// The column number, i.e. it's position in the array
      /// </summary>
      public int Number;

      /// <summary>
      /// The name of the column.
      /// </summary>
      public string Name;

      /// <summary>
      /// The kind of the column.
      /// </summary>
      public ColumnKind Kind;

      /// <summary>
      /// The group this column belongs to.
      /// </summary>
      public int Group;

      /// <summary>
      /// Constructs a DataColumnInfo object.
      /// </summary>
      /// <param name="name">The name of the column.</param>
      /// <param name="number">The position of the column in the collection.</param>
      public DataColumnInfo(string name, int number)
      {
        this.Name   = name;
        this.Number = number;
      }

      /// <summary>
      /// Constructs a DataColumnInfo object.
      /// </summary>
      /// <param name="name">The name of the column.</param>
      public DataColumnInfo(string name)
      {
        this.Name   = name;     
      }

      /// <summary>
      /// Constructs a DataColumnInfo object.
      /// </summary>
      /// <param name="name">The name of the column.</param>
      /// <param name="kind">The kind of the column.</param>
      public DataColumnInfo(string name, ColumnKind kind)
      {
        this.Name = name;
        this.Kind = kind;
      }

      /// <summary>
      /// Constructs a DataColumnInfo object.
      /// </summary>
      /// <param name="name">The name of the column.</param>
      /// <param name="kind">The kind of the column.</param>
      /// <param name="groupNumber">The group number of the column.</param>
      public DataColumnInfo(string name, ColumnKind kind, int groupNumber)
      {
        this.Name = name;
        this.Kind = kind;
        this.Group = groupNumber;
      }
      /// <summary>
      /// Copy constructor.
      /// </summary>
      /// <param name="from">Another object to copy from.</param>
      public DataColumnInfo(DataColumnInfo from)
      {
        this.Name   = from.Name;
        this.Number = from.Number;
        this.Group  = from.Group;
        this.Kind   = from.Kind;
      }

      public bool IsIndependentVariable
      {
        get { return Kind==ColumnKind.X || Kind==ColumnKind.Y || Kind==ColumnKind.Z; }
      }

      #region ICloneable Members

      /// <summary>
      /// Clones the object.
      /// </summary>
      /// <returns>The cloned object.</returns>
      public object Clone()
      {
        return new DataColumnInfo(this);
      }

      #endregion
    }

  
    #endregion

    #region Member data
    // Data

    /// <summary>
    /// The parent of this DataColumnCollection, normally a DataTable.
    /// </summary>
    protected object m_Parent=null; // the DataTable this set is belonging to
    
    /// <summary>
    /// The collection of the columns (accessible by number).
    /// </summary>
    protected System.Collections.ArrayList m_ColumnsByNumber = new System.Collections.ArrayList();
    
    /// <summary>
    /// Holds the columns (value) accessible by name (key).
    /// </summary>
    protected System.Collections.Hashtable m_ColumnsByName=new System.Collections.Hashtable();

    /// <summary>
    /// This hashtable has the <see cref="DataColumn" /> as keys and <see cref="DataColumnInfo" /> objects as values. 
    /// It stores information like the position of the column, the kind of the column.
    /// </summary>
    protected System.Collections.Hashtable m_ColumnInfo = new System.Collections.Hashtable();

    /// <summary>
    /// Cached number of rows. This is the maximum of the Count of all DataColumns contained in this collection.
    /// </summary>
    protected int m_NumberOfRows=0; // the max. Number of Rows of the columns of the table

    /// <summary>
    /// Indicates that the cached row count is no longer valid and can be lesser than the actual value in m_NumberOfRows
    /// (but not greater).
    /// </summary>
    protected bool m_NumberOfRowsDecreased=false;


    /// <summary>
    /// ColumnScripts, key is the corresponding column, value is of type WorksheetColumnScript
    /// </summary>
    protected ColumnScriptCollection m_ColumnScripts = new ColumnScriptCollection();

    
    /// <summary>
    /// Name of the last column added to this collection.
    /// </summary>
    protected string m_LastColumnNameAdded=""; // name of the last column wich was added to the table
    
    /// <summary>
    /// The last column name autogenerated by this class.
    /// </summary>
    protected string m_LastColumnNameGenerated=""; // name of the last column name that was automatically generated

    /// <summary>
    /// If true, we recently have tested the column names A-ZZ and all column names were in use.
    /// This flag should be resetted if a column deletion or renaming operation took place.
    /// </summary>
    protected bool _TriedOutRegularNaming=false; 
    
    /// <summary>
    /// Number of suspends to this object.
    /// </summary>
    protected int m_SuspendCount=0;

    /// <summary>
    /// If true, the resume is in progress, but not finished.
    /// </summary>
    private  bool m_ResumeInProgress=false;

    /// <summary>
    /// Collection of suspended childs, i.e. of supended data columns (only of that that are suspended by this collection).
    /// </summary>
    protected System.Collections.ArrayList m_SuspendedChildCollection = new System.Collections.ArrayList(); // the collection of dirty columns

    /// <summary>
    /// Holds the accumulated change data.
    /// </summary>
    protected ChangeEventArgs m_ChangeData;

    /// <summary>
    /// Event to signal a change of this collection.
    /// </summary>
    public event EventHandler Changed;

    /// <summary>
    /// Signals the change of the parent of the collection.
    /// </summary>
    public event Main.ParentChangedEventHandler ParentChanged;
  
    /// <summary>
    /// Flag to signal if deserialization is finished.
    /// </summary>
    private bool m_DeserializationFinished=false;

    #endregion

    #region Serialization
    public class SerializationSurrogate0 : System.Runtime.Serialization.ISerializationSurrogate
    {
      public void GetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context  )
      {
        Altaxo.Data.DataColumnCollection s = (Altaxo.Data.DataColumnCollection)obj;

        info.AddValue("Columns",s.m_ColumnsByNumber);
        // serialize the column scripts
        info.AddValue("ColumnScripts",s.m_ColumnScripts);
      }

      public object SetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context,System.Runtime.Serialization.ISurrogateSelector selector)
      {
        Altaxo.Data.DataColumnCollection s = (Altaxo.Data.DataColumnCollection)obj;
        
        s.m_ColumnsByNumber = (System.Collections.ArrayList)(info.GetValue("Columns",typeof(System.Collections.ArrayList)));
        s.m_ColumnScripts = (ColumnScriptCollection)(info.GetValue("ColumnScripts",typeof(ColumnScriptCollection)));

        // set up helper objects
        s.m_ColumnsByName=new System.Collections.Hashtable();
        s.m_SuspendedChildCollection = new System.Collections.ArrayList();
        s.m_LastColumnNameAdded="";
        s.m_LastColumnNameGenerated="";
        return s;
      }
    }

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(Altaxo.Data.DataColumnCollection),0)]
      class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info )
      {
        Altaxo.Data.DataColumnCollection s = (Altaxo.Data.DataColumnCollection)obj;

        info.CreateArray("ColumnArray",s.m_ColumnsByNumber.Count);
        for(int i=0;i<s.m_ColumnsByNumber.Count;i++)
        {
          info.CreateElement("Column");

          DataColumnInfo colinfo = s.GetColumnInfo(i);
          info.AddValue("Name",colinfo.Name);
          info.AddValue("Kind",(int)colinfo.Kind);
          info.AddValue("Group",colinfo.Group);
          info.AddValue("Data",s.m_ColumnsByNumber[i]);

          info.CommitElement();

        }
        info.CommitArray();

        // serialize the column scripts
        info.CreateArray("ColumnScripts",s.m_ColumnScripts.Count);
        foreach(System.Collections.DictionaryEntry entry in s.m_ColumnScripts)
        {
          info.CreateElement("Script");
          info.AddValue("ColName", s.GetColumnName((Altaxo.Data.DataColumn)entry.Key));
          info.AddValue("Content",entry.Value);
          info.CommitElement();
        }
        info.CommitArray();
      }

      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        Altaxo.Data.DataColumnCollection s = null!=o ? (Altaxo.Data.DataColumnCollection)o : new Altaxo.Data.DataColumnCollection();

        // deserialize the columns
        int count = info.OpenArray();
        for(int i=0;i<count;i++)
        {
          info.OpenElement(); // Column

          string name = info.GetString("Name");
          ColumnKind  kind = (ColumnKind)info.GetInt32("Kind");
          int    group = info.GetInt32("Group");
          object col = info.GetValue(s);
          if(col!=null)
            s.Add((DataColumn)col,new DataColumnInfo(name,kind,group));

          info.CloseElement(); // Column
        }
        info.CloseArray(count);

        // deserialize the scripts
        count = info.OpenArray();
        for(int i=0;i<count;i++)
        {
          info.OpenElement();
          string name =   info.GetString();
          IColumnScriptText script = (IColumnScriptText)info.GetValue(s);
          info.CloseElement();
          s.ColumnScripts.Add(s[name],script);
        }
        info.CloseArray(count); // end script array
        return s;
      }
    }

    /// <summary>
    /// Final measures for the object after deserialization.
    /// </summary>
    /// <param name="obj">The object for which the deserialization has finished.</param>
    public virtual void OnDeserialization(object obj)
    {
      if(!m_DeserializationFinished && obj is DeserializationFinisher)
      {
        m_DeserializationFinished = true;
        DeserializationFinisher finisher = new DeserializationFinisher(this);

        // 1. Set the parent Data table of the columns,
        // because they may be feel lonesome
        int nCols = m_ColumnsByNumber.Count;
        m_NumberOfRows = 0;
        DataColumn dc;
        for(int i=0;i<nCols;i++)
        {
          dc = (DataColumn)m_ColumnsByNumber[i];
          dc.ParentObject = this;
          dc.OnDeserialization(finisher);


          // add it also to the column name cache
          m_ColumnsByName.Add(dc.Name,dc);

          // count the maximumn number of rows
          if(dc.Count>m_NumberOfRows)
            m_NumberOfRows = dc.Count;
        }

      }
    }


    /// <summary>
    /// This class is responsible for the special purpose to serialize a data table for clipboard. Do not use
    /// it for permanent serialization purposes, since it does not contain version handling.
    /// </summary>
    [Serializable]
      public class ClipboardMemento : System.Runtime.Serialization.ISerializable
    {
      DataColumnCollection _collection;
      IAscendingIntegerCollection _selectedColumns;
      IAscendingIntegerCollection _selectedRows;
      bool                        _useOnlySelections;

      /// <summary>
      /// Constructor. Besides the table, the current selections must be provided. Only the areas that corresponds to the selections are
      /// serialized. The serialization process has to occur immediately after this constructor, because only a reference
      /// to the table is hold by this object.
      /// </summary>
      /// <param name="collection">The collection to serialize.</param>
      /// <param name="selectedColumns">The selected data columns.</param>
      /// <param name="selectedRows">The selected data rows.</param>
      /// <param name="useOnlySelections">If true, only the selections are serialized. If false and there is no selection, the whole collection is serialized.</param>
      public ClipboardMemento(
        DataColumnCollection collection,
        IAscendingIntegerCollection selectedColumns, 
        IAscendingIntegerCollection selectedRows,
        bool useOnlySelections)
      {
        this._collection          = collection;
        this._selectedColumns     = selectedColumns;
        this._selectedRows        = selectedRows;
        this._useOnlySelections   = useOnlySelections;
      }

      /// <summary>
      /// Returns the (deserialized) DataColumnCollection.
      /// </summary>
      public DataColumnCollection Collection
      {
        get { return _collection; }
      }

      #region ISerializable Members

      public void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
      {
        int numberOfColumns;
        bool useColumnSelection;

        // special case - no selection
        if(_selectedColumns.Count==0 && _selectedRows.Count==0 && _useOnlySelections)
        {
          numberOfColumns = 0;
          useColumnSelection = false;
        }
        else if(_selectedColumns.Count==0)
        {
          numberOfColumns = _collection.ColumnCount;
          useColumnSelection = false;
        }
        else
        {
          numberOfColumns = _selectedColumns.Count;
          useColumnSelection = true;
        }

        int numberOfRows=0;
        bool useRowSelection;
        if(_selectedRows.Count==0)
        {
          numberOfRows = _collection.RowCount;
          useRowSelection = false;
        }
        else
        {
          numberOfRows = _selectedRows.Count;
          useRowSelection = true;
        }


        info.AddValue("ColumnCount",numberOfColumns);


        for(int nCol=0;nCol<numberOfColumns;nCol++)
        {
          int colidx = useColumnSelection ? _selectedColumns[nCol] : nCol;
          DataColumnInfo columninfo = _collection.GetColumnInfo(colidx);
          info.AddValue("ColumnName_"+nCol.ToString(),columninfo.Name);
          info.AddValue("ColumnGroup_"+nCol.ToString(),columninfo.Group);
          info.AddValue("ColumnKind_"+nCol.ToString(),columninfo.Kind);

          // now create an instance of this column and copy the data
          DataColumn column = _collection[colidx];
          DataColumn newcolumn = (DataColumn)Activator.CreateInstance(_collection[colidx].GetType());
          for(int nRow=0;nRow<numberOfRows;nRow++)
          {
            int rowidx = useRowSelection ? _selectedRows[nRow] : nRow;
            newcolumn[nRow] = column[rowidx];
          } // for all rows
          info.AddValue("Column_"+nCol.ToString(),newcolumn);
        } // for all columns
      } // end serialization

      public ClipboardMemento(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
      {
        _collection = new DataColumnCollection();

        int numberOfColumns = info.GetInt32("ColumnCount");
        for(int nCol=0;nCol<numberOfColumns;nCol++)
        {
          string name = info.GetString("ColumnName_"+nCol.ToString());
          int    group = info.GetInt32("ColumnGroup_"+nCol.ToString());
          ColumnKind kind = (ColumnKind)info.GetValue("ColumnKind_"+nCol.ToString(),typeof(ColumnKind));

          DataColumn column = (DataColumn)info.GetValue("Column_"+nCol.ToString(),typeof(DataColumn));
        
        
          _collection.Add(column,name,kind,group);
        }
      }


      #endregion

    }

    #endregion

    #region Constructors

    /// <summary>
    /// Constructs an empty collection with no parent.
    /// </summary>
    public DataColumnCollection()
    {
      this.m_Parent = null;
    }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="from">The column collection to copy this data column collection from.</param>
    public DataColumnCollection(DataColumnCollection from)
    {
      this.m_LastColumnNameAdded = from.m_LastColumnNameAdded;
      this.m_LastColumnNameGenerated = from.m_LastColumnNameGenerated;

      // Copy all Columns
      for(int i=0;i<from.ColumnCount;i++)
      {
        DataColumn newCol = (DataColumn)from[i].Clone();
        DataColumnInfo newInfo = (DataColumnInfo)from.GetColumnInfo(from[i]).Clone();
        this.Add( newCol, newInfo);
      }
      // Copy all Column scripts
      foreach(System.Collections.DictionaryEntry d in from.ColumnScripts)
      {
        DataColumn srccol = (DataColumn)d.Key; // the original column this script belongs to
        DataColumn destcol = this[srccol.Name]; // the new (cloned) column the script now belongs to
        object destscript = ((ICloneable)d.Value).Clone(); // the cloned script

        // add the cloned script to the own collection
        this.ColumnScripts.Add(destcol,destscript);
      }
    }

    /// <summary>
    /// Clones the collection and all columns in it (deep copy).
    /// </summary>
    /// <returns></returns>
    public virtual object Clone()
    {
      return new DataColumnCollection(this);
    }

    /// <summary>
    /// Disposes the collection and all columns in it.
    /// </summary>
    public virtual void Dispose()
    {
      // first relase all column scripts
      foreach(System.Collections.DictionaryEntry d in this.m_ColumnScripts)
      {
        if(d.Value is IDisposable)
          ((IDisposable)d.Value).Dispose();
      }
      m_ColumnScripts.Clear();


      // release all owned Data columns
      this.m_ColumnsByName.Clear();

      for(int i=0;i<m_ColumnsByNumber.Count;i++)
        this[i].Dispose();

      m_ColumnsByNumber.Clear();
      this.m_NumberOfRows = 0;
    }

    #endregion
    
    #region Add / Replace Column



    /// <summary>
    /// Adds a column by choosing a new unused name for that column automatically.
    /// </summary>
    /// <param name="datac"></param>
    public void Add(Altaxo.Data.DataColumn datac)
    {
    

      Add(datac,datac.Name);
    }

    /// <summary>
    /// Add a column under the name <code>name</code>.
    /// </summary>
    /// <param name="datac">The column to add.</param>
    /// <param name="name">The name under which the column to add.</param>
    public void Add(Altaxo.Data.DataColumn datac, string name)
    {
      Add(datac,name,ColumnKind.V);
    }

    /// <summary>
    /// Adds a column with a given name and kind.
    /// </summary>
    /// <param name="datac">The column to add.</param>
    /// <param name="name">The name of the column.</param>
    /// <param name="kind">The kind of the column.</param>
    public void Add(Altaxo.Data.DataColumn datac, string name, ColumnKind kind)
    {
      Add(datac,name,kind,0);
    }

    /// <summary>
    /// Add a column with a given name, kind, and group number.
    /// </summary>
    /// <param name="datac">The column to add.</param>
    /// <param name="name">The name of the column.</param>
    /// <param name="kind">The kind of the column.</param>
    /// <param name="groupNumber">The group number of the column.</param>
    public void Add(Altaxo.Data.DataColumn datac, string name, ColumnKind kind, int groupNumber)
    {
      if(name==null)
        name = this.FindNewColumnName();
      else if( m_ColumnsByName.ContainsKey(name))
        name = this.FindUniqueColumnName(name);

      Add(datac,new DataColumnInfo(name,kind,groupNumber));
    }

    /// <summary>
    /// Either returns an existing column with name <c>columnname</c> and type <c>expectedtype</c> or creates a new column of the provided type.
    /// </summary>
    /// <param name="columnname">The column name.</param>
    /// <param name="expectedcolumntype">Expected type of the column.</param>
    /// <param name="kind">Only used when a new column is created.</param>
    /// <param name="groupNumber">Only used when a new column is created.</param>
    /// <returns>Either an existing column of name <c>columnname</c> and type <c>expectedtype</c>. If this is
    /// not the case, a new column with a similar name and the <c>expectedtype</c> is created and added to the collection. The new column is returned then.</returns>
    public DataColumn EnsureExistence(string columnname, System.Type expectedcolumntype, ColumnKind kind, int groupNumber)
    {
      if (IsColumnOfType(columnname, expectedcolumntype))
        return this[columnname];

      object o = System.Activator.CreateInstance(expectedcolumntype);
      if(!(o is DataColumn))
        throw new ApplicationException("The type you provided is not compatible with DataColumn, provided type: " + expectedcolumntype.GetType().ToString());

      Add((DataColumn)o, columnname, kind, groupNumber);
      return (DataColumn)o;
    }

    /// <summary>
    /// Add a column using a DataColumnInfo object. The provided info must not be used elsewhere, since it is used directly.
    /// </summary>
    /// <param name="datac">The column to add.</param>
    /// <param name="info">The DataColumnInfo object for the column to add.</param>
    private void Add(Altaxo.Data.DataColumn datac, DataColumnInfo info)
    {
      System.Diagnostics.Debug.Assert(this.ContainsColumn(datac)==false);
      System.Diagnostics.Debug.Assert(datac.ParentObject==null,"This private function should be only called with fresh DataColumns, if not, alter the behaviour of the calling function"); 
      System.Diagnostics.Debug.Assert(false==this.m_ColumnsByName.ContainsKey(info.Name),"Trying to add a column with a name that is already present (this error must be fixed in the calling function)");

      info.Number = this.m_ColumnsByNumber.Count;

      this.m_ColumnsByNumber.Add(datac);
      this.m_ColumnsByName[info.Name]=datac;
      this.m_ColumnInfo[datac] = info;
      datac.ParentObject = this;

      if(info.IsIndependentVariable)
        this.EnsureUniqueColumnKindsForIndependentVariables(info.Group,datac);

      this.EhChildChanged(null,ChangeEventArgs.CreateColumnAddArgs(info.Number,datac.Count));
    }


    /// <summary>
    /// Copies the data of the column (columns have same type, index is inside bounds), or replaces 
    /// the column (columns of different types, index inside bounds), or adds the column (index outside bounds).
    /// </summary>
    /// <param name="index">The column position where to replace or add.</param>
    /// <param name="datac">The column from which the data should be copied or which should replace the existing column or which should be added.</param>
    /// <param name="name">The name under which the column should be stored.</param>
    public void CopyOrReplaceOrAdd(int index, DataColumn datac, string name)
    {
      if(index<ColumnCount)
      {
        if(this[index].GetType().Equals(datac.GetType()))
        {
          this[index].CopyDataFrom(datac);
        }
        else
        {
          // if the column to add has a parent, we can not add the column directly (we are then not the owner), so we clone it
          Replace(index,datac.ParentObject==null ? datac : (DataColumn)datac.Clone());
        }
      }
      else
      {
        // if the column to add has a parent, we can not add the column directly (we are then not the owner), so we clone it
        Add(datac.ParentObject==null ? datac : (DataColumn)datac.Clone(), name);
      }
    }

    /// <summary>
    /// Replace the column at index <code>index</code> (if index is inside bounds) or add the column.
    /// </summary>
    /// <param name="index">The position of the column which should be replaced.</param>
    /// <param name="datac">The column which replaces the existing column or which is be added to the collection.</param>
    public void ReplaceOrAdd(int index, DataColumn datac)
    {
      if(index<ColumnCount)
        Replace(index, datac);
      else
        Add(datac);
    }

    /// <summary>
    /// Replace an existing column at index <code>index</code> by a new column <code>newCol.</code>
    /// </summary>
    /// <param name="index">The position of the column which should be replaced.</param>
    /// <param name="newCol">The new column that replaces the old one.</param>
    public void Replace(int index, DataColumn newCol)
    {
      if(index>=ColumnCount)
        throw new System.IndexOutOfRangeException(string.Format("Index ({0})for replace operation was outside the bounds, the actual column count is {1}",index,ColumnCount));

    
      DataColumn oldCol = this[index];
      if(!oldCol.Equals(newCol))
      {
        int oldRowCount = oldCol.Count;
        DataColumnInfo info = GetColumnInfo(index);
        m_ColumnsByName[info.Name] = newCol;
        m_ColumnsByNumber[index] = newCol;
        m_ColumnInfo.Remove(oldCol);
        m_ColumnInfo.Add(newCol,info);
        oldCol.ParentObject = null;
        newCol.ParentObject = this;
        
        object script = m_ColumnScripts[oldCol];
        if(null!=script)
        {
          m_ColumnScripts.Remove(oldCol);
          m_ColumnScripts.Add(newCol,script);
        }

        this.EhChildChanged(null,ChangeEventArgs.CreateColumnCopyOrReplaceArgs(index,oldRowCount,newCol.Count));

      }
    }

    /// <summary>
    /// Inserts multiple DataColumns into the collection at index <c>nDestinationIndex</c>. The caller must garantuee, that the names are not already be present!
    /// </summary>
    /// <param name="columns">The array of data columns to insert.</param>
    /// <param name="info">The corresponding column information for the columns to insert.</param>
    /// <param name="nDestinationIndex">The index into the collection where the columns are inserted.</param>
    protected void Insert(DataColumn[] columns, DataColumnInfo[] info, int nDestinationIndex)
    {
      Insert(columns,info,nDestinationIndex,false);
    }


    /// <summary>
    /// Inserts multiple DataColumns into the collection at index <c>nDestinationIndex</c>. The caller must garantuee, that the names are not already be present,
    /// or the argument <c>renameColumnsIfNecessary</c> must be set to true!
    /// </summary>
    /// <param name="columns">The array of data columns to insert.</param>
    /// <param name="info">The corresponding column information for the columns to insert.</param>
    /// <param name="nDestinationIndex">The index into the collection where the columns are inserted.</param>
    /// <param name="renameColumnsIfNecessary">If set to true, the columns to insert are automatically renamed if already be present in the destination collection. If false,
    /// an exception is thrown if a column with the same name is already be present in the destination table.</param>
    protected void Insert(DataColumn[] columns, DataColumnInfo[] info, int nDestinationIndex, bool renameColumnsIfNecessary)
    {
      this.Suspend();
      
      int indexOfAddedColumns = this.ColumnCount;
      int numberToAdd = columns.Length;
     
      // first add the columns to the collection
      for(int i=0;i<numberToAdd;i++)
      {
        if(renameColumnsIfNecessary && this.ContainsColumn(info[i].Name))
          info[i].Name = this.FindUniqueColumnNameWithBase(info[i].Name);

        this.Add(columns[i],info[i]);
      }

      // then move the columns to the desired position
      this.ChangeColumnPosition(new Altaxo.Collections.IntegerRangeAsCollection(indexOfAddedColumns,numberToAdd),nDestinationIndex);
      
      this.Resume();
    }

    #endregion

    #region Column removal and move to another collection

    /// <summary>
    /// Removes a number of columns of the collection.
    /// </summary>
    /// <param name="nFirstColumn">The first number of column to remove.</param>
    /// <param name="nDelCount">The number of columns to remove.</param>
    public virtual void RemoveColumns(int nFirstColumn, int nDelCount)
    {
      RemoveColumns(new IntegerRangeAsCollection(nFirstColumn,nDelCount));
    }

    /// <summary>
    /// Removes the column at the given index.
    /// </summary>
    /// <param name="nFirstColumn">The index of the column to remove.</param>
    public void RemoveColumn(int nFirstColumn)
    {
      RemoveColumns(nFirstColumn,1);
    }

    /// <summary>
    /// Remove the column given as the argument.
    /// </summary>
    /// <param name="datac">The column to remove.</param>
    public void RemoveColumn(DataColumn datac)
    {
      RemoveColumns(this.GetColumnNumber(datac),1);
    }

    public void RemoveColumns(IAscendingIntegerCollection selectedColumns)
    {
      RemoveColumns(selectedColumns,true);
    }

    public void RemoveColumns(IAscendingIntegerCollection selectedColumns, bool disposeColumns)
    {
      int nOriginalColumnCount = ColumnCount;

      int currentPosition = selectedColumns.Count-1;
      int nFirstColumn=0;
      int nDelCount=0;
      while(selectedColumns.GetNextRangeDescending(ref currentPosition, ref nFirstColumn, ref nDelCount))
      {
        // first, Dispose the columns and set the places to null
        for(int i=nFirstColumn+nDelCount-1;i>=nFirstColumn;i--)
        {
          string columnName = GetColumnName(this[i]);
          if(this.m_ColumnScripts.Contains(this[i]))
            this.m_ColumnScripts.Remove(this[i]);
          this.m_ColumnInfo.Remove(m_ColumnsByNumber[i]);
          this.m_ColumnsByName.Remove(columnName);
          this[i].ParentObject=null;
          if(disposeColumns)
            this[i].Dispose();
        }
        this.m_ColumnsByNumber.RemoveRange(nFirstColumn, nDelCount);
      }

      // renumber the remaining columns
      for(int i=m_ColumnsByNumber.Count-1;i>=nFirstColumn;i--)
        ((DataColumnInfo)m_ColumnInfo[m_ColumnsByNumber[i]]).Number = i; 

      // raise datachange event that some columns have changed
      this.EhChildChanged(null, ChangeEventArgs.CreateColumnRemoveArgs(nFirstColumn, nOriginalColumnCount, this.m_NumberOfRows));
   
      // reset the TriedOutRegularNaming flag, maybe one of the regular column names is now free again
      this._TriedOutRegularNaming = false;
    }

 
    /// <summary>
    /// Moves some columns of this collection to another collection-
    /// </summary>
    /// <param name="destination">The destination collection where the columns are moved to.</param>
    /// <param name="destindex">The index in the destination collection where the columns are moved to.</param>
    /// <param name="selectedColumns">The indices of the column of the source collection that are moved.</param>
    public void MoveColumnsTo(DataColumnCollection destination, int destindex, IAscendingIntegerCollection selectedColumns)
    {
      int nOriginalColumnCount = ColumnCount;
      
      int numberMoved = selectedColumns.Count;

      DataColumn[] tmpColumn = new DataColumn[numberMoved];
      DataColumnInfo[] tmpInfo = new DataColumnInfo[numberMoved];
      object[] tmpScript = new object[numberMoved];

      for(int i=0;i<numberMoved;i++)
      {
        tmpColumn[i] = this[selectedColumns[i]];
        tmpInfo[i] = (DataColumnInfo)this.m_ColumnInfo[m_ColumnsByNumber[i]];
        tmpScript[i] = this.m_ColumnScripts[tmpColumn[i]];
      }

      this.RemoveColumns(selectedColumns,false);

      destination.Insert(tmpColumn,tmpInfo,0,true);

      // Move the column scripts also
      for(int i=0; i<numberMoved; i++)
      {
        if(tmpScript[i]!=null)
          destination.m_ColumnScripts.Add(tmpColumn[i],tmpScript[i]);
      }
    }

    #endregion

    #region Column Information getting/setting
  
    /// <summary>
    /// Returns whether the column is contained in this collection.
    /// </summary>
    /// <param name="datac">The column in question.</param>
    /// <returns>True if the column is contained in this collection.</returns>
    public bool ContainsColumn(DataColumn datac)
    {
      return m_ColumnInfo.ContainsKey(datac);
    }

    /// <summary>
    /// Test if a column of a given name is present in this collection.
    /// </summary>
    /// <param name="columnname">The columnname to test for presence.</param>
    /// <returns>True if the column with the name is contained in the collection.</returns>
    public bool ContainsColumn(string columnname)
    {
      return m_ColumnsByName.ContainsKey(columnname);
    }

    /// <summary>
    /// Check if the named column is of the expected type.
    /// </summary>
    /// <param name="columnname">Name of the column.</param>
    /// <param name="expectedtype">Expected type of column.</param>
    /// <returns>True if the column exists and is exactly! of the expeced type. False otherwise.</returns>
    public bool IsColumnOfType(string columnname, System.Type expectedtype)
    {
      if (m_ColumnsByName.ContainsKey(columnname) && m_ColumnsByName[columnname].GetType() == expectedtype)
        return true;
      else
        return false;
    }

    /// <summary>
    /// Returns the position of the column in the Collection.
    /// </summary>
    /// <param name="datac">The column.</param>
    /// <returns>The position of the column, or -1 if the column is not contained.</returns>
    public int GetColumnNumber(Altaxo.Data.DataColumn datac)
    {
      DataColumnInfo info = GetColumnInfo(datac);
      return info==null ? -1 : info.Number;
    }
  

    /// <summary>
    /// Returns the name of a column.
    /// </summary>
    /// <param name="datac">The column..</param>
    /// <returns>The name of the column.</returns>
    public string GetColumnName(DataColumn datac)
    {
      return GetColumnInfo(datac).Name;
    }

    /// <summary>
    /// Returns the name of the column at position idx.
    /// </summary>
    /// <param name="idx">The position of the column (column number).</param>
    /// <returns>The name of the column.</returns>
    public string GetColumnName(int idx)
    {
      return GetColumnInfo(idx).Name;
    }

    /// <summary>
    /// Sets the name of the column <code>datac</code>.
    /// </summary>
    /// <param name="datac">The column which name is to set.</param>
    /// <param name="newName">The new name of the column.</param>
    public void SetColumnName(DataColumn datac, string newName)
    {
      string oldName = GetColumnInfo(datac).Name;
      if(oldName != newName)
      {
        if(this.ContainsColumn(newName))
        {
          throw new System.ApplicationException("Try to set column name to the name of a already present column: " + newName);
        }
        else
        {
          GetColumnInfo(datac).Name = newName;
          m_ColumnsByName.Remove(oldName);
          m_ColumnsByName.Add(newName,datac);

          this.EhChildChanged(null,ChangeEventArgs.CreateColumnRenameArgs(this.GetColumnNumber(datac)));

          // reset the TriedOutRegularNames flag, maybe one of the regular columns has been renamed
          this._TriedOutRegularNaming = false;
        }
      }
    }

    /// <summary>
    /// Sets the name of the column at position<code>index</code>.
    /// </summary>
    /// <param name="index">The column number.</param>
    /// <param name="newName">The new name of the column.</param>
    public void SetColumnName(int index, string newName)
    {
      SetColumnName(this[index],newName);
    }

    /// <summary>
    /// Rename the column with the name <code>oldName</code> to <code>newName</code>.
    /// </summary>
    /// <param name="oldName">The old name of the column.</param>
    /// <param name="newName">The new name of the column.</param>
    public void SetColumnName(string oldName, string newName)
    {
      SetColumnName(this[oldName],newName);
    }
  
    /// <summary>
    /// Returns the goup number of the column <code>datac</code>.
    /// </summary>
    /// <param name="datac">The column.</param>
    /// <returns>The group number of this column.</returns>
    public int GetColumnGroup(DataColumn datac)
    {
      return GetColumnInfo(datac).Group;
    }
    /// <summary>
    /// Returns the goup number of the column at index <code>idx</code>
    /// </summary>
    /// <param name="idx">The column number of the column.</param>
    /// <returns>The group number of this column.</returns>
    public int GetColumnGroup(int idx)
    {
      return GetColumnInfo(idx).Group;
    }

    /// <summary>
    /// This function will return the smallest possible group number, which is currently not in use.
    /// </summary>
    /// <returns>The smallest unused group number (starting at 0).</returns>
    public int GetUnusedColumnGroupNumber()
    {
      System.Collections.SortedList groupNums = new System.Collections.SortedList();
      for(int i=0;i<ColumnCount;i++)
      {
        int group = this.GetColumnGroup(i);
        if(!groupNums.ContainsKey(group))
          groupNums.Add(group,null);
      }

      for(int i=0;i<int.MaxValue;i++)
      {
        if(!groupNums.Contains(i))
          return i;
      }
      return 0;
    }

    /// <summary>
    /// Sets the group number of the column with the given column number <code>idx</code>.
    /// </summary>
    /// <param name="idx">The column number of the column.</param>
    /// <param name="groupNumber">The group number to set for this column.</param>
    public void SetColumnGroup(int idx, int groupNumber)
    {
      GetColumnInfo(idx).Group = groupNumber;
      EnsureUniqueColumnKindsForIndependentVariables(groupNumber,this[idx]);
    }
    
    /// <summary>
    /// Returns the kind of the column <code>datac</code>.
    /// </summary>
    /// <param name="datac">The column for which the kind is returned.</param>
    /// <returns>The kind of the provided column.</returns>
    public ColumnKind GetColumnKind(DataColumn datac)
    {
      return GetColumnInfo(datac).Kind;
    }

    /// <summary>
    /// Returns the column kind of the column at index <code>idx</code>
    /// </summary>
    /// <param name="idx">The column number of the column.</param>
    /// <returns>The kind of this column.</returns>
    public ColumnKind GetColumnKind(int idx)
    {
      return GetColumnInfo(idx).Kind;
    }

    /// <summary>
    /// Sets the kind of the column with column number <code>idx</code>.
    /// </summary>
    /// <param name="idx">The column number of the column.</param>
    /// <param name="columnKind">The new kind of the column.</param>
    public void SetColumnKind(int idx, ColumnKind columnKind)
    {
      DataColumnInfo info = GetColumnInfo(idx);
      info.Kind = columnKind;
      EnsureUniqueColumnKindsForIndependentVariables(info.Group,this[idx]);
    }

    /// <summary>
    /// Ensures that for a given group number there is only one column for each independent variable (X,Y,Z).
    /// </summary>
    /// <param name="groupNumber">The group number of the columns which are checked for this rule.</param>
    /// <param name="exceptThisColumn">If not null, this column is treated with priority. If this column is a independent variable column, it can
    /// keep its kind.</param>
    protected void EnsureUniqueColumnKindsForIndependentVariables(int groupNumber, DataColumn exceptThisColumn)
    {
      bool X_present=false;
      bool Y_present=false;
      bool Z_present=false;

      if(exceptThisColumn!=null)
      {
        switch(GetColumnInfo(exceptThisColumn).Kind)
        {
          case ColumnKind.X:
            X_present=true;
            break;
          case ColumnKind.Y:
            Y_present=true;
            break;
          case ColumnKind.Z:
            Z_present=true;
            break;
        }
      }


      foreach(System.Collections.DictionaryEntry entry in this.m_ColumnInfo)
      {
        if(((DataColumnInfo)entry.Value).Group==groupNumber && !entry.Key.Equals(exceptThisColumn))
        {
          DataColumnInfo info = (DataColumnInfo)entry.Value;
          switch(info.Kind)
          {
            case ColumnKind.X:
              if(X_present)
                info.Kind = ColumnKind.V;
              else
                X_present = true;
              break;
            case ColumnKind.Y:
              if(Y_present)
                info.Kind = ColumnKind.V;
              else
                Y_present = true;
              break;
            case ColumnKind.Z:
              if(Z_present)
                info.Kind = ColumnKind.V;
              else
                Z_present = true;
              break;
          }
        }
      }
    }


    /// <summary>
    /// Removes the x-property from all columns in the group nGroup.
    /// </summary>
    /// <param name="nGroup">the group number for the columns from which to remove the x-property</param>
    /// <param name="exceptThisColumn">If not null, this column is treated with priority, it can keep its kind.</param>
    public void DeleteXProperty(int nGroup, DataColumn exceptThisColumn)
    {
      int len = this.ColumnCount;
      for(int i=len-1;i>=0;i--)
      {
        DataColumn dc = (DataColumn)m_ColumnsByNumber[i];
        DataColumnInfo info = (DataColumnInfo)this.m_ColumnInfo[dc];
        if(info.Group==nGroup && info.Kind==ColumnKind.X && !dc.Equals(exceptThisColumn))
        {
          info.Kind = ColumnKind.V;
        }
      }
    }

    /// <summary>
    /// Using a given column, find the related X column of this.
    /// </summary>
    /// <param name="datac">The column for which to find the related X column.</param>
    /// <returns>The related X column, or null if it is not found.</returns>
    public Altaxo.Data.DataColumn FindXColumnOf(DataColumn datac)
    {
      return FindXColumnOfGroup(GetColumnGroup(datac));
    }

    /// <summary>
    /// Returns the X column of the column group <code>nGroup</code>.
    /// </summary>
    /// <param name="nGroup">The column group number.</param>
    /// <returns>The X column of the provided group, or null if it is not found.</returns>
    public Altaxo.Data.DataColumn FindXColumnOfGroup(int nGroup)
    {
      int len = this.ColumnCount;
      for(int i=len-1;i>=0;i--)
      {
        DataColumn dc = (DataColumn)m_ColumnsByNumber[i];
        DataColumnInfo info = (DataColumnInfo)this.m_ColumnInfo[dc];
        if(info.Group==nGroup && info.Kind==ColumnKind.X)
        {
          return dc;
        }
      }
      return null;
    }

    /// <summary>
    /// Get the column info for the column <code>datac</code>.
    /// </summary>
    /// <param name="datac">The column for which the column information is returned.</param>
    /// <returns>The column information of the provided column.</returns>
    private DataColumnInfo GetColumnInfo(DataColumn datac)
    {
      return (DataColumnInfo)m_ColumnInfo[datac];
    }

    /// <summary>
    /// Get the column info for the column with index<code>idx</code>.
    /// </summary>
    /// <param name="idx">The column index of the column for which the column information is returned.</param>
    /// <returns>The column information of the column.</returns>
    private DataColumnInfo GetColumnInfo(int idx)
    {
      return (DataColumnInfo)m_ColumnInfo[m_ColumnsByNumber[idx]];
    }

    /// <summary>
    /// Get the column info for the column with name <code>columnName</code>.
    /// </summary>
    /// <param name="columnName">The column name of the column for which the column information is returned.</param>
    /// <returns>The column information of the column.</returns>
    private DataColumnInfo GetColumnInfo(string columnName)
    {
      return (DataColumnInfo)m_ColumnInfo[m_ColumnsByName[columnName]];
    }

    #endregion

    #region Row insertion/removal

    /// <summary>
    /// Insert a number of empty rows in all columns.
    /// </summary>
    /// <param name="nBeforeRow">The row number before which the additional rows should be inserted.</param>
    /// <param name="nRowsToInsert">The number of rows to insert.</param>
    public void InsertRows(int nBeforeRow, int nRowsToInsert)
    {
      Suspend();

      for(int i=0;i<ColumnCount;i++)
        this[i].InsertRows(nBeforeRow,nRowsToInsert);
    
      Resume();
    }

    /// <summary>
    /// Removes a number of rows from all columns.
    /// </summary>
    /// <param name="nFirstRow">Number of first row to remove.</param>
    /// <param name="nCount">Number of rows to remove, starting from nFirstRow.</param>
    public void RemoveRows(int nFirstRow, int nCount)
    {
      RemoveRows(new IntegerRangeAsCollection(nFirstRow,nCount));
    }

    /// <summary>
    /// Removes the <code>selectedRows</code> from the table.
    /// </summary>
    /// <param name="selectedRows">Collection of indizes to the rows that should be removed.</param>
    public void RemoveRows(IAscendingIntegerCollection selectedRows)
    {
      RemoveRowsInColumns(new IntegerRangeAsCollection(0,ColumnCount),selectedRows);
    }

    /// <summary>
    /// Removes all rows.
    /// </summary>
    public void RemoveRowsAll()
    {
      RemoveRows(0,this.RowCount);
    }

    /// <summary>
    /// Removes all rows. Same function as <see cref="RemoveRowsAll" />.
    /// </summary>
    public void ClearData()
    {
      RemoveRowsAll();
    }

    /// <summary>
    /// Removes the <code>selectedRows</code> from the table.
    /// </summary>
    /// <param name="selectedColumns">Collection of indices to the columns that should be removed.</param>
    /// <param name="selectedRows">Collection of indizes to the rows that should be removed.</param>
    public void RemoveRowsInColumns(IAscendingIntegerCollection selectedColumns, IAscendingIntegerCollection selectedRows)
    {
      // if we remove rows, we have to do that in reverse order
      Suspend();

      for(int selcol=0;selcol<selectedColumns.Count;selcol++)
      {
        int colidx = selectedColumns[selcol];

        int rangestart=0, rangecount=0;
        int i = selectedRows.Count-1;

        while(selectedRows.GetNextRangeDescending(ref i, ref rangestart, ref rangecount))
        {
          this[colidx].RemoveRows(rangestart,rangecount);
        }
      }

      Resume();
    }
    
  

    /// <summary>
    /// Removes a single row of all columns.
    /// </summary>
    /// <param name="nFirstRow">The row to remove.</param>
    public void RemoveRow(int nFirstRow)
    {
      RemoveRows(nFirstRow,1);
    }


    #endregion

    #region Column and row position manipulation

    /// <summary>
    /// Moves one or more columns to a new position.
    /// </summary>
    /// <param name="selectedColumns">The indices of the columns to move to the new position.</param>
    /// <param name="newPosition">The new position where the columns are moved to.</param>
    /// <remarks>An exception is thrown if newPosition is negative or higher than possible.</remarks>
    public void ChangeColumnPosition(Altaxo.Collections.IAscendingIntegerCollection selectedColumns, int newPosition)
    {
      int numberSelected = selectedColumns.Count;
      if(numberSelected==0)
        return;

      int oldPosition = selectedColumns[0];
      if(oldPosition==newPosition)
        return;

      // check that the newPosition is ok
      if(newPosition<0)
        throw new ArgumentException("New column position is negative!");
      if(newPosition+numberSelected>ColumnCount)
        throw new ArgumentException(string.Format("New column position too high: ColsToMove({0})+NewPosition({1})>ColumnCount({2})",numberSelected,newPosition,ColumnCount));

      // Allocated tempory storage for the datacolumns
      Altaxo.Data.DataColumn[] columnsMoved = new Altaxo.Data.DataColumn[selectedColumns.Count];
      // fill temporary storage
      for(int i=0;i<selectedColumns.Count;i++)
        columnsMoved[i]=this[selectedColumns[i]];

      int firstAffectedColumn=0;
      int maxAffectedColumn = ColumnCount;
      if(newPosition<oldPosition) // move down to lower indices
      {
        firstAffectedColumn = newPosition;
        maxAffectedColumn  = Math.Max(newPosition+numberSelected,selectedColumns[numberSelected-1]+1);

        for(int i=selectedColumns[numberSelected-1],offset=0;i>=firstAffectedColumn;i--)
        {
          if(numberSelected>offset && i==selectedColumns[numberSelected-1-offset])
            offset++;
          else
            m_ColumnsByNumber[i+offset] = m_ColumnsByNumber[i];

        }
      }
      else // move up to higher
      {
        firstAffectedColumn = selectedColumns[0];
        maxAffectedColumn = newPosition+numberSelected;
        for(int i=selectedColumns[0],offset=0;i<maxAffectedColumn;i++)
        {
          if(offset<numberSelected && i==selectedColumns[offset])
            offset++;
          else
            m_ColumnsByNumber[i-offset] = m_ColumnsByNumber[i];

        }
      }

      // Fill in temporary stored columns on new position
      for(int i=0;i<numberSelected;i++)
        m_ColumnsByNumber[newPosition+i] = columnsMoved[i];
      
      RefreshColumnIndices();
      this.EhChildChanged(null,ChangeEventArgs.CreateColumnMoveArgs(firstAffectedColumn,maxAffectedColumn));
    }

    /// <summary>
    /// Moves on or more rows in to a new position.
    /// </summary>
    /// <param name="selectedIndices">The indices of the rows to move.</param>
    /// <param name="newPosition">The new position of the rows.</param>
    public void ChangeRowPosition(Altaxo.Collections.IAscendingIntegerCollection selectedIndices, int newPosition)
    {
      int numberSelected = selectedIndices.Count;
      if(numberSelected==0)
        return;

      int oldPosition = selectedIndices[0];
      if(oldPosition==newPosition)
        return;

      // check that the newPosition is ok
      if(newPosition<0)
        throw new ArgumentException("New row position is negative!");

      

      // Allocated tempory storage for the datacolumns
      Altaxo.Data.AltaxoVariant[] tempMoved = new Altaxo.Data.AltaxoVariant[numberSelected];
     
      int firstAffected;
      int maxAffected;

      if(newPosition<oldPosition) // move down to lower indices
      {
        firstAffected = newPosition;
        maxAffected  = Math.Max(newPosition+numberSelected,selectedIndices[numberSelected-1]+1);
      }
      else
      {
        firstAffected = selectedIndices[0];
        maxAffected = newPosition+numberSelected;
      }

      for(int nColumn=ColumnCount-1;nColumn>=0;nColumn--)
      {
        Altaxo.Data.DataColumn thiscolumn = this[nColumn];
        // fill temporary storage
        for(int i=0;i<numberSelected;i++)
          tempMoved[i]=thiscolumn[selectedIndices[i]];
  
        if(newPosition<oldPosition) // move down to lower indices
        {
          for(int i=selectedIndices[numberSelected-1],offset=0;i>=firstAffected;i--)
          {
            if(numberSelected>offset && i==selectedIndices[numberSelected-1-offset])
              offset++;
            else
              thiscolumn[i+offset] = thiscolumn[i];

          }
        }
        else // move up to higher
        {
          for(int i=selectedIndices[0],offset=0;i<maxAffected;i++)
          {
            if(offset<numberSelected && i==selectedIndices[offset])
              offset++;
            else
              thiscolumn[i-offset] = thiscolumn[i];

          }
        }

        // Fill in temporary stored columns on new position
        for(int i=0;i<numberSelected;i++)
          thiscolumn[newPosition+i] = tempMoved[i];

      }
      this.EhChildChanged(null,ChangeEventArgs.CreateRowMoveArgs(ColumnCount,firstAffected,maxAffected));
    }

    /// <summary>
    /// This will refresh the column number information in the m_ColumnInfo collection of <see cref="DataColumnInfo" />.
    /// </summary>
    protected void RefreshColumnIndices()
    {
      for(int i=ColumnCount-1;i>=0;i--)
      {
        ((DataColumnInfo)m_ColumnInfo[m_ColumnsByNumber[i]]).Number = i;
      }
    }
    

    #endregion

    #region Indexer

    /// <summary>
    /// Returns the column with name <code>s</code>. Sets the column with name <code>s</code> by copying data from
    /// the other column (not by replacing). An exception is thrown if the two columns are not of the same type.
    /// </summary>
    public Altaxo.Data.DataColumn this[string s]
    {
      get
      {
        return (Altaxo.Data.DataColumn)m_ColumnsByName[s];
      }
      set
      {
        // setting a column should not change its name nor its other properties
        // only the data array and the related parameters should be changed
        Altaxo.Data.DataColumn c = (Altaxo.Data.DataColumn)m_ColumnsByName[s];

        if(null!=c)
        {
          c.CopyDataFrom(value);
        }
        else
        {
          throw new Exception("The column \"" + s + "\" in node \"" + Main.DocumentPath.GetAbsolutePath(this).ToString() + "\" does not exist.");
        }
      }
    }

    
    /// <summary>
    /// Returns the column at index <code>idx</code>. Sets the column at index<code>idx</code> by copying data from
    /// the other column (not by replacing). An exception is thrown if the two columns are not of the same type.
    /// </summary>
    public Altaxo.Data.DataColumn this[int idx]
    {
      get
      {
        return (Altaxo.Data.DataColumn)m_ColumnsByNumber[idx];
      }
      set
      {
        // setting a column should not change its name nor its other properties
        // only the data array and the related parameters should be changed
        
        if(idx<m_ColumnsByNumber.Count)
        {
          Altaxo.Data.DataColumn c = (Altaxo.Data.DataColumn)m_ColumnsByNumber[idx];
          c.CopyDataFrom(value);
        }
        else
        {
          throw new Exception("The column[" + idx + "] in table \"" + Main.DocumentPath.GetAbsolutePath(this).ToString() + "\" does not exist.");
        }
      }
    }


  

    #endregion Indexer

    #region Collection Properties

    /// <summary>
    /// The row count, i.e. the maximum of the row counts of all columns.
    /// </summary>
    public int RowCount
    {
      get
      {
        if(this.m_NumberOfRowsDecreased)
          this.RefreshRowCount(true);

        return m_NumberOfRows;
      }
    }
  
    /// <summary>
    /// The number of columns in the collection.
    /// </summary>
    public int ColumnCount
    {
      get
      {
        return this.m_ColumnsByNumber.Count;
      }
    }
  

    /// <summary>
    /// The parent of this collection.
    /// </summary>
    public virtual object ParentObject
    {
      get { return m_Parent; }
      set 
      {
        object oldParent = m_Parent;
        m_Parent=value;

        if(!object.ReferenceEquals(oldParent,m_Parent))
          OnParentChanged(oldParent,m_Parent);
      }
    }

    protected virtual void OnParentChanged(object oldParent, object newParent)
    {
      if(null!=ParentChanged)
        ParentChanged(this,new Altaxo.Main.ParentChangedEventArgs(oldParent,newParent));
    }

    /// <summary>
    /// The name of this collection.
    /// </summary>
    public virtual string Name
    {
      get 
      {
        Main.INamedObjectCollection noc = ParentObject as Main.INamedObjectCollection;
        return noc==null ? null : noc.GetNameOfChildObject(this);
      }
    }

  

    /// <summary>
    /// Returns the collection of column scripts.
    /// </summary>
    public ColumnScriptCollection ColumnScripts
    {
      get { return this.m_ColumnScripts; }
    }

    /// <summary>
    /// Returns an array containing all column names that are contained in this collection.
    /// </summary>
    /// <returns>An array containing all column names that are contained in this collection.</returns>
    public string[] GetColumnNames()
    {
      string[] arr = new string[this.ColumnCount];
      for(int i=0;i<arr.Length;i++)
        arr[i] = GetColumnName(i);

      return arr;
    }

    /// <summary>
    /// Tests whether or not all columns in this collection have the same type.
    /// </summary>
    /// <param name="firstdifferentcolumnindex">Out: returns the first column that has a different type from the first column</param>.
    /// <returns>True if all columns are of the same type.</returns>
    public bool AreAllColumnsOfTheSameType(out int firstdifferentcolumnindex)
    {
      firstdifferentcolumnindex=0;

      if(0==this.ColumnCount)
        return true;

      System.Type t1st = this[0].GetType();

      int len = this.ColumnCount;
      for(int i=0;i<len;i++)
      {
        if(this[i].GetType()!=t1st)
        {
          firstdifferentcolumnindex = i;
          return false;
        }
      }

      return true;
    }

    #endregion

    #region Event handling

    /// <summary>
    /// Returns true if this object has outstanding changed not reported yet to the parent.
    /// </summary>
    public virtual bool IsDirty
    {
      get
      {
        return null!=m_ChangeData;
      }
    }

    /// <summary>
    /// True if the notification of changes is currently suspended.
    /// </summary>
    public bool IsSuspended
    {
      get { return m_SuspendCount>0; }
    }

    /// <summary>
    /// Suspend the notification of changes.
    /// </summary>
    public virtual void Suspend()
    {
      System.Diagnostics.Debug.Assert(m_SuspendCount>=0,"SuspendCount must always be greater or equal to zero");    
      m_SuspendCount++;
    }

    /// <summary>
    /// Resume the notification of changed.
    /// </summary>
    public void Resume()
    {
      System.Diagnostics.Debug.Assert(m_SuspendCount>=0,"SuspendCount must always be greater or equal to zero");    
      if(m_SuspendCount>0 && (--m_SuspendCount)==0)
      {
        this.m_ResumeInProgress = true;
        foreach(Main.ISuspendable obj in m_SuspendedChildCollection)
          obj.Resume();
        m_SuspendedChildCollection.Clear();
        this.m_ResumeInProgress = false;

        // send accumulated data if available and release it thereafter
        if(null!=m_ChangeData)
        {
          if(m_Parent is Main.IChildChangedEventSink)
          {
            ((Main.IChildChangedEventSink)m_Parent).EhChildChanged(this, m_ChangeData);
          }
          if(!IsSuspended)
          {
            OnDataChanged(); // Fire the changed event
          }   
        }
      }
    }


    /// <summary>
    /// Accumulates the changes reported by the DataColumns.
    /// </summary>
    /// <param name="sender">One of the columns of this collection.</param>
    /// <param name="e">The change details.</param>
    void AccumulateChildChangeData(object sender, EventArgs e)
    {
      DataColumn.ChangeEventArgs changed = e as DataColumn.ChangeEventArgs;
      if(changed!=null && sender is DataColumn)
      {
        int columnNumberOfSender = GetColumnNumber((DataColumn)sender);
        int rowCountOfSender = ((DataColumn)sender).Count;

        if(m_ChangeData==null)
          m_ChangeData = new ChangeEventArgs(columnNumberOfSender,changed.MinRowChanged,changed.MaxRowChanged,changed.RowCountDecreased);
        else 
          m_ChangeData.Accumulate(columnNumberOfSender,changed.MinRowChanged,changed.MaxRowChanged,changed.RowCountDecreased);

        // update the row count
        if(this.m_NumberOfRows < rowCountOfSender)
          m_NumberOfRows = rowCountOfSender;
        this.m_NumberOfRowsDecreased |= changed.RowCountDecreased;
      }
      else if(e is DataColumnCollection.ChangeEventArgs)
      {
        DataColumnCollection.ChangeEventArgs changeargs = (DataColumnCollection.ChangeEventArgs)e;
        if(null==m_ChangeData)
          m_ChangeData = changeargs;
        else
          m_ChangeData.Accumulate(changeargs);

        // update the row count
        if(this.m_NumberOfRows < changeargs.MaxRowChanged)
          this.m_NumberOfRows = changeargs.MaxRowChanged;
        this.m_NumberOfRowsDecreased |= changeargs.RowCountDecreased;
      }
    }

    protected bool HandleImmediateChildChangeCases(object sender, EventArgs e)
    {
      if(e is Main.ParentChangedEventArgs)
      {
        Main.ParentChangedEventArgs pce = (Main.ParentChangedEventArgs)e;
        if(object.ReferenceEquals(this,pce.OldParent) && this.ContainsColumn((DataColumn)sender))
          this.RemoveColumn((DataColumn)sender);
        else if(object.ReferenceEquals(this,pce.NewParent) && !this.ContainsColumn((DataColumn)sender))
          throw new ApplicationException("Not allowed to set child's parent to this collection before adding it to the collection");
      
        return true;
      }

      return false;
    }


    /// <summary>
    /// Handle the change notification from the child data columns.
    /// </summary>
    /// <param name="sender">The sender of the change notification.</param>
    /// <param name="e">The change details.</param>
    public void EhChildChanged(object sender, System.EventArgs e)
    {
      if(HandleImmediateChildChangeCases(sender, e))
        return;

      if(this.IsSuspended &&  sender is Main.ISuspendable)
      {
        m_SuspendedChildCollection.Add(sender); // add sender to suspended child
        ((Main.ISuspendable)sender).Suspend();
        return;
      }

      AccumulateChildChangeData(sender,e);  // AccumulateNotificationData
      
      if(m_ResumeInProgress || IsSuspended)
        return;

      if(m_Parent is Main.IChildChangedEventSink )
      {
        ((Main.IChildChangedEventSink)m_Parent).EhChildChanged(this, m_ChangeData);
        if(IsSuspended) // maybe parent has suspended us now
        {
          this.EhChildChanged(sender, e); // we call the function recursively, but now we are suspended
          return;
        }
      }
      
      OnDataChanged(); // Fire the changed event
    }

    /// <summary>
    /// Fires the change event.
    /// </summary>
    /// <param name="e">The change details.</param>
    protected virtual void OnChanged(ChangeEventArgs e)
    {
      if(null!=Changed)
        Changed(this,e);
    }


    /// <summary>
    /// Fires the change event.
    /// </summary>
    protected virtual void OnDataChanged()
    {
      if(null!=Changed)
        Changed(this,m_ChangeData);

      m_ChangeData=null;
    }



    /// <summary>
    /// Refreshes the row count by observing all columns.
    /// </summary>
    public void RefreshRowCount()
    {
      RefreshRowCount(true);
    }


    /// <summary>
    /// Refreshes the row count.
    /// </summary>
    /// <param name="bSearchOnlyUntilOldRowCountReached">If false, all columns are observed. If true, the columns are
    /// observed only until one column is reached, which has the same value of the row count as this collection.</param>
    public void RefreshRowCount(bool bSearchOnlyUntilOldRowCountReached)
    {
      int rowCount=0;

      if(bSearchOnlyUntilOldRowCountReached)
      {
        foreach(DataColumn c in this.m_ColumnsByNumber)
        {
          rowCount = System.Math.Max(rowCount,c.Count);
          if(rowCount>=m_NumberOfRows) 
            break;
        }
      }
      else
      {
        foreach(DataColumn c in this.m_ColumnsByNumber)
          rowCount = System.Math.Max(rowCount,c.Count);
      }

      // now take over the new row count
      this.m_NumberOfRows =  rowCount;
      this.m_NumberOfRowsDecreased = false; // row count is now actual

    }


    #endregion
  
    #region Automatic column naming

    /// <summary>
    /// Finds a new unique column name.
    /// </summary>
    /// <returns>The new unique column name.</returns>
    public string FindNewColumnName()
    {
      return FindUniqueColumnName(null);
    }


    /// <summary>
    /// Calculates a new column name dependend on the last name. You have to check whether the returned name is already in use by yourself.
    /// </summary>
    /// <param name="lastName">The last name that was used to name a column.</param>
    /// <returns>The logical next name of a column calculated from the previous name. This name is in the range "A" to "ZZ". If
    /// no further name can be found, this function returns null.</returns>
    public static string GetNextColumnName(string lastName)
    {
      int lastNameLength = null==lastName ? 0 : lastName.Length;
      
      if(0==lastNameLength)
        return "A";
      else if(1==lastNameLength)
      {
        char _1st = lastName[0];
        _1st++;
        if(_1st>='A' && _1st<='Z')
          return _1st.ToString();
        else
          return "AA";
      }
      else if(2==lastNameLength)
      {
        char _1st = lastName[0];
        char _2nd = lastName[1];
        _2nd++;

        if(_2nd<'A' || _2nd>'Z')
        {
          _1st++;
          _2nd='A';
        }

        if(_1st>='A' && _1st<='Z')
          return _1st.ToString() + _2nd;
        else
          return null;
      }

      else
      {
        return null;
      }

    }



    /// <summary>
    /// Get a unique column name based on regular naming from A to ZZ.
    /// </summary>
    /// <returns>An unique column name based on regular naming.</returns>
    protected string FindUniqueColumnNameWithoutBase()
    {
      string tryName;
      if(_TriedOutRegularNaming)
      {
        for(;;)
        {
          tryName = ((uint)System.Guid.NewGuid().GetHashCode()).ToString("X8");
          if(null==this[tryName])
            return tryName;
        }
      }
      else
      {
        // First try it with the next name after the last column
        tryName = GetNextColumnName(m_ColumnsByNumber.Count>0 ? GetColumnName(ColumnCount-1) : "");
        if(null!=tryName && null==this[tryName])
          return tryName;

        // then try it with all names from A-ZZ
        for(tryName="A"; tryName!=null; tryName = GetNextColumnName(tryName))
        {
          if(null==this[tryName])
            return tryName;
        }
        // if no success, set the _TriedOutRegularNaming
        _TriedOutRegularNaming = true;
        return FindUniqueColumnNameWithoutBase();
      }
    }


    /// <summary>
    /// Get a unique column name based on a provided string. If a column with the name of the provided string
    /// already exists, a new name is created by appending a dot and then A-ZZ.
    /// </summary>
    /// <param name="sbase">A string which is the base of the new name. Must not be null!</param>
    /// <returns>An unique column name based on the provided string.</returns>
    protected string FindUniqueColumnNameWithBase(string sbase)
    {
      // try the name directly
      if(null==this[sbase])
        return sbase;

      sbase = sbase+".";
 

      // then try it with all names from A-ZZ
        
      for(string tryAppendix="A";tryAppendix!=null;tryAppendix=GetNextColumnName(tryAppendix))
      {
        if(null==this[sbase+tryAppendix])
          return sbase+tryAppendix;
      }

      // if no success, append a hex string
      for(;;)
      {
        string tryName = sbase+((uint)System.Guid.NewGuid().GetHashCode()).ToString("X8");
        if(null==this[tryName])
          return tryName;
      }
    }

    /// <summary>
    /// Get a unique column name based on a provided string.
    /// </summary>
    /// <param name="sbase">The base name.</param>
    /// <returns>An unique column name based on the provided string.</returns>
    public string FindUniqueColumnName(string sbase)
    {
      return sbase==null ? FindUniqueColumnNameWithoutBase() : FindUniqueColumnNameWithBase(sbase);
    }


    #endregion

    #region Special Collection methods

    /// <summary>
    /// Transpose transpose the table, i.e. exchange columns and rows
    /// this can only work if all columns in the table are of the same type
    /// </summary>
    /// <returns>null if succeeded, error string otherwise</returns>
    public virtual string Transpose()
    {
      int firstdifferent;

      if(!AreAllColumnsOfTheSameType(out firstdifferent))
      {
        return String.Format("Column[{0}] ({1}) has a different type than the first column transpose is not possible!",firstdifferent,GetColumnName(firstdifferent));
      }

      // now we can start by adding additional columns of the row count is greater
      // than the column count
      this.Suspend();
      int originalrowcount = this.RowCount;
      int originalcolcount = this.ColumnCount;
      if(this.RowCount>this.ColumnCount)
      {
        int addcols = this.RowCount - this.ColumnCount;
        // this is a little tricky - we have to add the same type of
        // column like the first one
        System.Type coltype = this[0].GetType();
        for(int ii=0;ii<addcols;ii++)
        {
          Altaxo.Data.DataColumn dt = (Altaxo.Data.DataColumn)Activator.CreateInstance(coltype);
          this.Add(dt);
        }
      } // if RowCount>ColumnCount

      // now we can exchange the data

      int tocol;
      int i,j;
      if(originalcolcount>=originalrowcount)
      {
        for(i=0;i<originalrowcount;i++)
        {
          
          for(j=i+1;j<originalcolcount;j++)
          {
            Altaxo.Data.AltaxoVariant hlp = this[j][i];
            this[j][i] = this[i][j];
            this[i][j] = hlp;
          }
        }

      }
      else // originalrowcount>originalcolcount
      {
        tocol = originalcolcount;
        for(i=0;i<originalcolcount;i++)
        {
          for(j=i+1;j<originalrowcount;j++)
          {
            Altaxo.Data.AltaxoVariant hlp = this[i][j];
            this[i][j] = this[j][i];
            this[j][i] = hlp;
          }
        }

      }

      // now we should delete the superfluous columns when originalcolcount>originalrowcount
      if(originalcolcount>originalrowcount)
      {
        this.RemoveColumns(originalrowcount,originalcolcount-originalrowcount);
      }

      this.Resume();


      return null; // no error message
    }

    #endregion

    #region INamedObjectCollection Members

    /// <summary>
    /// Returns the column with the name <code>name</code>.
    /// </summary>
    /// <param name="name">The name of the column to retrieve.</param>
    /// <returns>The column with name <code>name</code>, or null if not found.</returns>
    public object GetChildObjectNamed(string name)
    {
      return this.m_ColumnsByName[name];
    }

    /// <summary>
    /// Retrieves the name of a child column <code>o</code>.
    /// </summary>
    /// <param name="o">The child column.</param>
    /// <returns>The name of the column.</returns>
    public string GetNameOfChildObject(object o)
    {
      DataColumnInfo info = (DataColumnInfo)this.m_ColumnInfo[o];
      if(info!=null)
        return info.Name;
      else 
        return null;
    }

    #endregion

    /// <summary>
    /// Gets the parent column collection of a column.
    /// </summary>
    public static Altaxo.Data.DataColumnCollection GetParentDataColumnCollectionOf(Altaxo.Data.DataColumn column)
    {
      if(column.ParentObject is DataColumnCollection)
        return (DataColumnCollection)column.ParentObject;
      else
        return (DataColumnCollection)Main.DocumentPath.GetRootNodeImplementing(column,typeof(DataColumnCollection));
    }

  } // end class Altaxo.Data.DataColumnCollection
}
