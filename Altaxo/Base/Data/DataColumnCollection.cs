#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2014 Dr. Dirk Lellinger
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

#endregion Copyright

#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Altaxo.Collections;
using Altaxo.Main;
using Altaxo.Main.Services;
using Altaxo.Scripting;

namespace Altaxo.Data
{
  public class DataColumnCollection :
    Main.SuspendableDocumentNodeWithSingleAccumulatedData<DataColumnCollectionChangedEventArgs>,
    IList<DataRow>,
    IDisposable,
    ICloneable
  {
    // Types
    //  public delegate void OnDataChanged(Altaxo.Data.DataColumnCollection sender, int nMinCol, int nMaxCol, int nMinRow, int nMaxRow);   // delegate declaration
    //  public delegate void OnDirtySet(Altaxo.Data.DataColumnCollection sender);

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
        Name = name;
        Number = number;
      }

      /// <summary>
      /// Constructs a DataColumnInfo object.
      /// </summary>
      /// <param name="name">The name of the column.</param>
      public DataColumnInfo(string name)
      {
        Name = name;
      }

      /// <summary>
      /// Constructs a DataColumnInfo object.
      /// </summary>
      /// <param name="name">The name of the column.</param>
      /// <param name="kind">The kind of the column.</param>
      public DataColumnInfo(string name, ColumnKind kind)
      {
        Name = name;
        Kind = kind;
      }

      /// <summary>
      /// Constructs a DataColumnInfo object.
      /// </summary>
      /// <param name="name">The name of the column.</param>
      /// <param name="kind">The kind of the column.</param>
      /// <param name="groupNumber">The group number of the column.</param>
      public DataColumnInfo(string name, ColumnKind kind, int groupNumber)
      {
        Name = name;
        Kind = kind;
        Group = groupNumber;
      }

      /// <summary>
      /// Copy constructor.
      /// </summary>
      /// <param name="from">Another object to copy from.</param>
      public DataColumnInfo(DataColumnInfo from)
      {
        Name = from.Name;
        Number = from.Number;
        Group = from.Group;
        Kind = from.Kind;
      }

      public bool IsIndependentVariable
      {
        get { return Kind == ColumnKind.X || Kind == ColumnKind.Y || Kind == ColumnKind.Z; }
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

      #endregion ICloneable Members
    }

    #endregion ColumnInfo

    #region Member data

    /// <summary>
    /// The collection of the columns (accessible by number).
    /// </summary>
    protected List<DataColumn> _columnsByNumber = new List<DataColumn>();

    /// <summary>
    /// Holds the columns (value) accessible by name (key).
    /// </summary>
    protected Dictionary<string, DataColumn> _columnsByName = new Dictionary<string, DataColumn>();

    /// <summary>
    /// This hashtable has the <see cref="DataColumn" /> as keys and <see cref="DataColumnInfo" /> objects as values.
    /// It stores information like the position of the column, the kind of the column.
    /// </summary>
    protected Dictionary<DataColumn, DataColumnInfo> _columnInfoByColumn = new Dictionary<DataColumn, DataColumnInfo>();

    /// <summary>
    /// Is true if any data in a child DataColumn has changed since the last saving of the project.
    /// This flag is <b>not</b> set to true if other parts of this collection changed, for instance the column scripts.
    /// </summary>
    protected bool _isDataDirty;

    /// <summary>
    /// Cached number of rows. This is the maximum of the Count of all DataColumns contained in this collection.
    /// </summary>
    protected int _numberOfRows = 0; // the max. Number of Rows of the columns of the table

    /// <summary>
    /// Indicates that the cached row count is no longer valid and can be lesser than the actual value in m_NumberOfRows
    /// (but not greater).
    /// </summary>
    protected bool _hasNumberOfRowsDecreased = false;

    /// <summary>
    /// ColumnScripts, key is the corresponding column, value is of type WorksheetColumnScript
    /// </summary>
    protected ColumnScriptCollection _columnScripts = new ColumnScriptCollection();

    /// <summary>
    /// Name of the last column added to this collection.
    /// </summary>
    protected string _nameOfLastColumnAdded = ""; // name of the last column wich was added to the table

    /// <summary>
    /// The last column name autogenerated by this class.
    /// </summary>
    protected string _lastColumnNameGenerated = ""; // name of the last column name that was automatically generated

    /// <summary>
    /// If true, we recently have tested the column names A-ZZ and all column names were in use.
    /// This flag should be resetted if a column deletion or renaming operation took place.
    /// </summary>
    protected bool _triedOutRegularNaming = false;

    /// <summary>
    /// This object can have three states: if it is a <see cref="IProjectArchiveEntryMemento"/>, then this indicates that data
    /// have to be loaded using this memento. If some other object, this indicates that the loading of data is currently in progress.
    /// If null, the data have been loaded. In this case, the archive memento is stored in another field (see <see cref="_archiveMemento"/> ).
    /// </summary>
    protected object? _deferredDataLoader;

    /// <summary>
    /// This field is set to the <see cref="IProjectArchiveEntryMemento"/> that indicates in which entry in
    /// the project archive the table data is stored. This member is set independent of whether
    /// data should be late loaded with the memento or not! (The indicator for that
    /// is stored in <see cref="_deferredDataLoader"/>).
    /// </summary>
    protected IProjectArchiveEntryMemento? _archiveMemento;

    #endregion Member data

    #region Serialization

    /// <summary>If set, only the data should be stored, but e.g. not the scripts etc.</summary>
    public const string SerialiationInfoProperty_StoreDataOnly = "Altaxo.Data.DataColumnCollection_StoreDataOnly";
    public const string DeserialiationInfoProperty_RestoreDataOnly = "Altaxo.Data.DataColumnCollection_RestoreDataOnly";

    /// <summary>Used during deserialization to store the info how to read the data at a later time.</summary>
    public const string DeserialiationInfoProperty_DeferredDataDeserialization = "Altaxo.Data.DataColumnCollection_DefDataDeser";

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Data.DataColumnCollection", 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (Altaxo.Data.DataColumnCollection)obj;

        info.CreateArray("ColumnArray", s._columnsByNumber.Count);
        for (int i = 0; i < s._columnsByNumber.Count; i++)
        {
          info.CreateElement("Column");

          DataColumnInfo colinfo = s.GetColumnInfo(i);
          info.AddValue("Name", colinfo.Name);
          info.AddValue("Kind", (int)colinfo.Kind);
          info.AddValue("Group", colinfo.Group);
          info.AddValue("Data", s._columnsByNumber[i]);

          info.CommitElement();
        }
        info.CommitArray();

        // serialize the column scripts
        info.CreateArray("ColumnScripts", s._columnScripts.Count);
        foreach (KeyValuePair<DataColumn, IColumnScriptText> entry in s._columnScripts)
        {
          info.CreateElement("Script");
          info.AddValue("ColName", s.GetColumnName(entry.Key));
          info.AddValue("Content", entry.Value);
          info.CommitElement();
        }
        info.CommitArray();
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (Altaxo.Data.DataColumnCollection?)o ?? new Altaxo.Data.DataColumnCollection();

        // deserialize the columns
        int count = info.OpenArray();
        for (int i = 0; i < count; i++)
        {
          info.OpenElement(); // Column

          string name = info.GetString("Name");
          var kind = (ColumnKind)info.GetInt32("Kind");
          int group = info.GetInt32("Group");
          var col = (DataColumn)info.GetValue("Data", s);
          if (col != null)
            s.Add(col, new DataColumnInfo(name, kind, group));

          info.CloseElement(); // Column
        }
        info.CloseArray(count);

        // deserialize the scripts
        count = info.OpenArray();
        for (int i = 0; i < count; i++)
        {
          info.OpenElement();
          string name = info.GetString();
          var script = (IColumnScriptText)info.GetValue("e", s);
          info.CloseElement();
          s.ColumnScripts.Add(s[name], script);
        }
        info.CloseArray(count); // end script array
        return s;
      }
    }

    /// <summary>
    /// 2019-08-23 Change order of serialization, so that the columns came last. Add row count in order to have it available before accessing the data.
    /// </summary>
    /// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(Altaxo.Data.DataColumnCollection), 1)]
    private class XmlSerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        // Note: info property 'DataOnly': stores only the data, but not the scripts and so on


        var s = (Altaxo.Data.DataColumnCollection)obj;

        var storeDataOnly = info.GetProperty(SerialiationInfoProperty_StoreDataOnly) == "true";
        var saveAsTemplate = info.GetProperty(DataTable.SerializationInfoProperty_SaveAsTemplate) == "true";
        if (!saveAsTemplate)
        {
          s.EnsureDeferredDataAreLoaded();
        }

        info.AddValue("NumberOfRows", s._numberOfRows);

        {
          info.CreateArray("ColumnArray", s._columnsByNumber.Count);
          for (int i = 0; i < s._columnsByNumber.Count; i++)
          {
            info.CreateElement("Column");

            DataColumnInfo colinfo = s.GetColumnInfo(i);
            info.AddValue("Name", colinfo.Name);
            info.AddValue("Kind", (int)colinfo.Kind);
            info.AddValue("Group", colinfo.Group);
            info.AddValue("Data", s._columnsByNumber[i]);

            info.CommitElement();
          }
          info.CommitArray();
        }

        if (!storeDataOnly)
        {
          // serialize the column scripts
          info.CreateArray("ColumnScripts", s._columnScripts.Count);
          foreach (KeyValuePair<DataColumn, IColumnScriptText> entry in s._columnScripts)
          {
            info.CreateElement("Script");
            info.AddValue("ColName", s.GetColumnName(entry.Key));
            info.AddValue("Content", entry.Value);
            info.CommitElement();
          }
          info.CommitArray();
        }
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (Altaxo.Data.DataColumnCollection?)o ?? new Altaxo.Data.DataColumnCollection();


        int numberOfRows = info.GetInt32("NumberOfRows");

        {
          // deserialize the columns, here the columns are deserialized without data.
          int count = info.OpenArray();
          for (int i = 0; i < count; i++)
          {
            info.OpenElement(); // Column

            string name = info.GetString("Name");
            var kind = (ColumnKind)info.GetInt32("Kind");
            int group = info.GetInt32("Group");
            var col = (DataColumn)info.GetValue("Data", s);
            if (col != null)
              s.Add(col, new DataColumnInfo(name, kind, group));

            info.CloseElement(); // Column
          }
          info.CloseArray(count);
        }

        if ("true" != info.GetPropertyOrDefault<string>(DeserialiationInfoProperty_RestoreDataOnly))
        {
          // deserialize the scripts
          int count = info.OpenArray();
          for (int i = 0; i < count; i++)
          {
            info.OpenElement();
            string name = info.GetString();
            var script = (IColumnScriptText)info.GetValue("e", s);
            info.CloseElement();
            s.ColumnScripts.Add(s[name], script);
          }
          info.CloseArray(count); // end script array
        }

        s._deferredDataLoader = info.GetPropertyOrDefault<object>(DeserialiationInfoProperty_DeferredDataDeserialization);
        if (null != s._deferredDataLoader)
        {
          s._numberOfRows = numberOfRows;
          s._hasNumberOfRowsDecreased = false;
          s._isDataDirty = false;
        }

        return s;
      }
    }

    /// <summary>
    /// This class is responsible for the special purpose to serialize a data table for clipboard. Do not use
    /// it for permanent serialization purposes, since it does not contain version handling.
    /// </summary>
    [Serializable]
    public class ClipboardMemento
    {
      private DataColumnCollection _collection;
      private IAscendingIntegerCollection _selectedColumns;
      private IAscendingIntegerCollection _selectedRows;
      private bool _useOnlySelections;

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
        _collection = collection;
        _selectedColumns = selectedColumns;
        _selectedRows = selectedRows;
        _useOnlySelections = useOnlySelections;
      }

      /// <summary>
      /// Returns the (deserialized) DataColumnCollection.
      /// </summary>
      public DataColumnCollection Collection
      {
        get { return _collection; }
      }

      #region Serialization

      [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(Altaxo.Data.DataColumnCollection.ClipboardMemento), 0)]
      private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
      {
        public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
        {
          var s = (Altaxo.Data.DataColumnCollection.ClipboardMemento)obj;

          int numberOfColumns;
          bool useColumnSelection;

          // special case - no selection
          if (s._selectedColumns.Count == 0 && s._selectedRows.Count == 0 && s._useOnlySelections)
          {
            numberOfColumns = 0;
            useColumnSelection = false;
          }
          else if (s._selectedColumns.Count == 0)
          {
            numberOfColumns = s._collection.ColumnCount;
            useColumnSelection = false;
          }
          else
          {
            numberOfColumns = s._selectedColumns.Count;
            useColumnSelection = true;
          }

          int numberOfRows = 0;
          bool useRowSelection;
          if (s._selectedRows.Count == 0)
          {
            numberOfRows = s._collection.RowCount;
            useRowSelection = false;
          }
          else
          {
            numberOfRows = s._selectedRows.Count;
            useRowSelection = true;
          }

          info.AddValue("ColumnCount", numberOfColumns);

          for (int nCol = 0; nCol < numberOfColumns; nCol++)
          {
            int colidx = useColumnSelection ? s._selectedColumns[nCol] : nCol;
            DataColumnInfo columninfo = s._collection.GetColumnInfo(colidx);
            info.AddValue("ColumnName_" + nCol.ToString(), columninfo.Name);
            info.AddValue("ColumnGroup_" + nCol.ToString(), columninfo.Group);
            info.AddValue("ColumnKind_" + nCol.ToString(), (int)columninfo.Kind);

            // now create an instance of this column and copy the data
            DataColumn column = s._collection[colidx];
            var newcolumn = (DataColumn)Activator.CreateInstance(s._collection[colidx].GetType())!;
            for (int nRow = 0; nRow < numberOfRows; nRow++)
            {
              int rowidx = useRowSelection ? s._selectedRows[nRow] : nRow;
              newcolumn[nRow] = column[rowidx];
            } // for all rows
            info.AddValue("Column_" + nCol.ToString(), newcolumn);
          } // for all columns
        }

        public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
        {
          var s = (Altaxo.Data.DataColumnCollection.ClipboardMemento?)o ?? new Altaxo.Data.DataColumnCollection.ClipboardMemento(info);

          int numberOfColumns = info.GetInt32("ColumnCount");
          for (int nCol = 0; nCol < numberOfColumns; nCol++)
          {
            string name = info.GetString("ColumnName_" + nCol.ToString());
            int group = info.GetInt32("ColumnGroup_" + nCol.ToString());
            var kind = (ColumnKind)info.GetInt32("ColumnKind_" + nCol.ToString());

            var column = (DataColumn)info.GetValue("Column_" + nCol.ToString(), typeof(DataColumn));

            s._collection.Add(column, name, kind, group);
          }

          return s;
        }
      }

      private ClipboardMemento(Altaxo.Serialization.Xml.IXmlDeserializationInfo info)
      {
        _collection = new DataColumnCollection();
        _selectedColumns = new AscendingIntegerCollection();
        _selectedRows = new AscendingIntegerCollection();
      }

      #endregion Serialization
    }

    #endregion Serialization

    #region Deserialization (deferred data loading)

    /// <summary>
    /// Infrastructure - do not use this unless absolutely neccessary.
    /// Gets or sets the deferred data memento that helps to late load the data for this <see cref="DataColumnCollection"/>.
    /// </summary>
    /// <value>
    /// The deferred data memento.
    /// </value>
    public IProjectArchiveEntryMemento? DeferredDataMemento
    {
      get
      {
        lock (_deferredLock)
        {
          return _deferredDataLoader as IProjectArchiveEntryMemento ?? _archiveMemento;
        }
      }
      set
      {
        lock (_deferredLock)
        {
          _archiveMemento = value;
        }

        if (null != value)
          _isDataDirty = false;
      }
    }

    /// <summary>
    /// Ensures that the data for this collection are loaded.
    /// </summary>
    public void EnsureDeferredDataAreLoaded()
    {
      if (null != _deferredDataLoader)
        TryLoadDeferredData();
    }

    private object _deferredLock = new object();
    private void TryLoadDeferredData()
    {
      object? deferredDataLoader = null;
      lock (_deferredLock)
      {
        if (null != _deferredDataLoader)
        {
          deferredDataLoader = _deferredDataLoader;
          _deferredDataLoader = new object();
        }
      }

      if (deferredDataLoader is null)
      {
        return;
      }
      else if (deferredDataLoader is IProjectArchiveEntryMemento tp)
      {
        LoadDeferredData(tp);
        var newMemento = tp.Clone();
        tp?.Dispose();
        lock (_deferredLock)
        {
          _deferredDataLoader = null;
          _archiveMemento = newMemento;
        }
      }
      else
      {
        while (!(_deferredDataLoader is null))
          System.Threading.Thread.Sleep(10);
      }
    }

    private void LoadDeferredData(IProjectArchiveEntryMemento memento)
    {
      var zipEntry = memento.GetArchiveEntry();
      using (var zipinpstream = zipEntry.OpenForReading())
      {
        var info = new Altaxo.Serialization.Xml.XmlStreamDeserializationInfo();
        info.PropertyDictionary[DeserialiationInfoProperty_RestoreDataOnly] = "true";
        info.BeginReading(zipinpstream);
        object readedobject = info.GetValue("TableData", null);
        try
        {
          if (readedobject is Altaxo.Data.DataColumnCollection dataColColl)
          {
            TransferDeferredData(dataColColl);
            _isDataDirty = false;
          }
          else
          {
            throw new InvalidCastException($"Expected type for readedObject is {nameof(Altaxo.Data.DataColumnCollection)}, but it is acually {readedobject?.GetType()}");
          }
        }
        finally
        {
          info.EndReading();
          info.Dispose();

        }
      }
    }

    private void TransferDeferredData(Altaxo.Data.DataColumnCollection dataColColl)
    {
      using (var lc = this.SuspendGetToken())
      {
        int rowCount = 0;
        var len = Math.Min(ColumnCount, dataColColl.ColumnCount);
        for (int i = 0; i < len; ++i)
        {
          _columnsByNumber[i].Data = dataColColl[i];
          rowCount = System.Math.Max(rowCount, _columnsByNumber[i].Count);
        }

        _numberOfRows = rowCount;
        _hasNumberOfRowsDecreased = false;

        lc.ResumeSilently();
      }
    }


    #endregion

    #region Constructors

    /// <summary>
    /// Constructs an empty collection with no parent.
    /// </summary>
    public DataColumnCollection()
    {
      _parent = null;
    }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="from">The column collection to copy this data column collection from.</param>
    public DataColumnCollection(DataColumnCollection from)
    {
      _nameOfLastColumnAdded = from._nameOfLastColumnAdded;
      _lastColumnNameGenerated = from._lastColumnNameGenerated;

      // Copy all Columns
      for (int i = 0; i < from.ColumnCount; i++)
      {
        var newCol = (DataColumn)from[i].Clone();
        var newInfo = (DataColumnInfo)from.GetColumnInfo(from[i]).Clone();
        Add(newCol, newInfo);
      }
      // Copy all Column scripts
      foreach (KeyValuePair<DataColumn, IColumnScriptText> d in from.ColumnScripts)
      {
        DataColumn srccol = d.Key; // the original column this script belongs to
        DataColumn destcol = this[srccol.Name]; // the new (cloned) column the script now belongs to
        var destscript = (IColumnScriptText)d.Value.Clone(); // the cloned script

        // add the cloned script to the own collection
        ColumnScripts.Add(destcol, destscript);
        if (destscript is Main.IDocumentLeafNode)
          destscript.ParentObject = this;
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
    protected override void Dispose(bool isDisposing)
    {
      if (!IsDisposed)
      {
        _deferredDataLoader = null;

        // first relase all column scripts
        _columnScripts?.Dispose();

        // release all owned Data columns
        _columnsByName.Clear();
        _columnInfoByColumn.Clear();
        for (int i = _columnsByNumber.Count - 1; i >= 0; --i) // iterate downwards, because the dispose action can trigger the column to be removed from the collection
          this[i].Dispose();

        _columnsByNumber.Clear();
        _numberOfRows = 0;
      }

      base.Dispose(isDisposing);
    }

    #endregion Constructors

    #region Add / Replace Column

    /// <summary>
    /// Gets the data columns as an enumeration.
    /// </summary>
    /// <value>
    /// The data columns as Enumeration.
    /// </value>
    public IEnumerable<Altaxo.Data.DataColumn> Columns
    {
      get
      {
        if (null != _deferredDataLoader)
          TryLoadDeferredData();

        foreach (var c in _columnsByNumber)
          yield return c;
      }
    }

    /// <summary>
    /// Adds a column by choosing a new unused name for that column automatically.
    /// </summary>
    /// <param name="datac"></param>
    public void Add(Altaxo.Data.DataColumn datac)
    {
      Add(datac, datac.Name);
    }

    /// <summary>
    /// Add a column under the name <code>name</code>.
    /// </summary>
    /// <param name="datac">The column to add.</param>
    /// <param name="name">The name under which the column to add.</param>
    public void Add(Altaxo.Data.DataColumn datac, string name)
    {
      Add(datac, name, ColumnKind.V);
    }

    /// <summary>
    /// Adds a column with a given name and kind.
    /// </summary>
    /// <param name="datac">The column to add.</param>
    /// <param name="name">The name of the column.</param>
    /// <param name="kind">The kind of the column.</param>
    public void Add(Altaxo.Data.DataColumn datac, string name, ColumnKind kind)
    {
      Add(datac, name, kind, 0);
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
      if (name == null)
        name = FindNewColumnName();
      else if (_columnsByName.ContainsKey(name))
        name = FindUniqueColumnName(name);

      Add(datac, new DataColumnInfo(name, kind, groupNumber));
    }

    /// <summary>
    /// Add a column using a DataColumnInfo object. The provided info must not be used elsewhere, since it is used directly.
    /// </summary>
    /// <param name="datac">The column to add.</param>
    /// <param name="info">The DataColumnInfo object for the column to add.</param>
    private void Add(Altaxo.Data.DataColumn datac, DataColumnInfo info)
    {
      if (null != _deferredDataLoader)
        TryLoadDeferredData();


      if (!(ContainsColumn(datac) == false))
        throw new ArgumentException(nameof(datac) + " is already contained in this collection");

      if (!(datac.ParentObject == null))
        throw new ArgumentException(nameof(datac) + " already has a parent. This private function should be only called with fresh DataColumns, if not, alter the behaviour of the calling function");

      if (!(false == _columnsByName.ContainsKey(info.Name)))
        throw new InvalidOperationException("Trying to add a column with a name that is already present (this error must be fixed in the calling function)");

      if (!(datac.IsSomeoneListeningToChanges == false))
        throw new InvalidOperationException("Trying to add a column that was used before because either ParentObject is set or someone is listening to change events");

      info.Number = _columnsByNumber.Count;

      _columnsByNumber.Add(datac);
      _columnsByName[info.Name] = datac;
      _columnInfoByColumn[datac] = info;
      datac.ParentObject = this;

      if (info.IsIndependentVariable)
        EnsureUniqueColumnKindsForIndependentVariables(info.Group, datac);

      EhChildChanged(null, DataColumnCollectionChangedEventArgs.CreateColumnAddArgs(info.Number, datac.Count));
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

      var o = System.Activator.CreateInstance(expectedcolumntype);
      if (!(o is DataColumn))
        throw new ApplicationException("The type you provided is not compatible with DataColumn, provided type: " + expectedcolumntype.GetType().ToString());

      Add((DataColumn)o, columnname, kind, groupNumber);
      return (DataColumn)o;
    }

    /// <summary>
    /// Ensures the existence of a column with exactly the provided properties at the provided position.
    /// </summary>
    /// <param name="columnNumber">The column number. Have to be in the range (0..ColumnCount). If the value is ColumnCount, a new column is added.</param>
    /// <param name="columnName">Name of the column. If another column with the same name exists, the existing column with the same name will be renamed (if the existing column has a higher column number).
    /// If the existing column with the same name has a lower column number, an exception is thrown.</param>
    /// <param name="expectedColumnType">Expected type of the column. If a column with the provided type exists at the provided position, this column is used. If the column at the provided position
    /// is of a different type, a new column with the provided type is created, and is then used to replace the column at the provided position.</param>
    /// <param name="columnKind">Kind of the column.</param>
    /// <param name="groupNumber">The group number of the column.</param>
    /// <returns>A column with exactly the provided properties at exactly the provided position.</returns>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// If columnNumber is either less than 0 or greater than <see cref="ColumnCount"/>
    /// </exception>
    /// <exception cref="System.InvalidOperationException">
    /// If the provided type is not a subclass of <see cref="DataColumn"/> or is an abstract type.
    /// or
    /// A column with the same name already exists to the left of the provided position.
    /// </exception>
    public DataColumn EnsureExistenceAtPositionStrictly(int columnNumber, string columnName, System.Type expectedColumnType, ColumnKind columnKind, int groupNumber)
    {
      return EnsureExistenceAtPositionStrictly(columnNumber, columnName, true, expectedColumnType, columnKind, groupNumber);
    }

    /// <summary>
    /// Ensures the existence of a column with exactly the provided properties at the provided position.
    /// </summary>
    /// <param name="columnNumber">The column number. Have to be in the range (0..ColumnCount). If the value is ColumnCount, a new column is added.</param>
    /// <param name="columnName">Name of the column. If another column with the same name exists, the existing column with the same name will be renamed (if the existing column has a higher column number).
    /// <param name="strictColumnName">If true, and another column with the same name exists to the left of this column, an exception is thrown. Otherwise, a new unique name based on the provided name will be found for this column.</param>
    /// If the existing column with the same name has a lower column number, an exception is thrown.</param>
    /// <param name="expectedColumnType">Expected type of the column. If a column with the provided type exists at the provided position, this column is used. If the column at the provided position
    /// is of a different type, a new column with the provided type is created, and is then used to replace the column at the provided position.</param>
    /// <param name="columnKind">Kind of the column.</param>
    /// <param name="groupNumber">The group number of the column.</param>
    /// <returns>A column with exactly the provided properties at exactly the provided position.</returns>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// If columnNumber is either less than 0 or greater than <see cref="ColumnCount"/>
    /// </exception>
    /// <exception cref="System.InvalidOperationException">
    /// If the provided type is not a subclass of <see cref="DataColumn"/> or is an abstract type.
    /// or
    /// A column with the same name already exists to the left of the provided position.
    /// </exception>
    public DataColumn EnsureExistenceAtPositionStrictly(int columnNumber, string columnName, bool strictColumnName, System.Type expectedColumnType, ColumnKind columnKind, int groupNumber)
    {
      if (columnNumber < 0)
        throw new ArgumentOutOfRangeException("columnNumber must not be < 0");
      if (columnNumber > ColumnCount)
        throw new ArgumentOutOfRangeException("columnNumber must not be > ColumnCount");

      if (columnNumber == ColumnCount) // create a new column
      {
        var o = System.Activator.CreateInstance(expectedColumnType);
        if (!(o is DataColumn))
          throw new InvalidOperationException("The type you provided is not compatible with DataColumn, provided type: " + expectedColumnType.GetType().ToString());

        Add((DataColumn)o, columnName, columnKind, groupNumber);
      }

      // now we can expect that the column always exist at this position
      var col = this[columnNumber];

      // first test if we have the appropriate type
      if (!(col.GetType() == expectedColumnType || col.GetType().IsSubclassOf(expectedColumnType)))
      {
        // then replace the column at this position with the right type
        var o = System.Activator.CreateInstance(expectedColumnType);
        if (!(o is DataColumn))
          throw new InvalidOperationException("The type you provided is not compatible with DataColumn, provided type: " + expectedColumnType.GetType().ToString());

        col = (DataColumn)o;
        Replace(columnNumber, col);
      }

      // now we can be sure that we have the right column type
      // next task is to set the name
      // by convention it if the name already exist to the left of the new column, we throw an exception (because we assume that we build the table from left to right)
      // else if the name already exist to the right of the column, we rename the right column

      if (GetColumnName(columnNumber) != columnName)
      {
        if (!ContainsColumn(columnName)) // Fine, the name doesn't exist, thus we can set it straightforward
        {
          SetColumnName(columnNumber, columnName);
        }
        else // ColumnName exists already
        {
          int otherColumnNumber = GetColumnNumber(this[columnName]);
          if ((otherColumnNumber < columnNumber) && strictColumnName)
            throw new InvalidOperationException("A column with the same name already exists to the left of the current column.");
          if (otherColumnNumber < columnNumber) // the same name exists to the left of the current column
          {
            // Create an arbitrary name for the new column
            string newName = FindUniqueColumnName(columnName);
            SetColumnName(columnNumber, newName);
          }
          else
          {
            // Create an arbitrary name for the other column
            string newName = FindUniqueColumnName(columnName);
            SetColumnName(otherColumnNumber, newName);
            SetColumnName(columnNumber, columnName);
          }
        }
      }

      // Set group number and kind
      SetColumnGroup(columnNumber, groupNumber);
      SetColumnKind(columnNumber, columnKind);

      return col;
    }

    /// <summary>
    /// Ensures the existence of a column with exactly the provided properties at the provided position.
    /// </summary>
    /// <typeparam name="TDataCol">The type of the data column. Has to be a type derived from <see cref="DataColumn"/>.</typeparam>
    /// <param name="columnNumber">The column number. Have to be in the range (0..ColumnCount). If the value is ColumnCount, a new column is added.</param>
    /// <param name="columnName">Name of the column. If another column with the same name exists, the existing column with the same name will be renamed (if the existing column has a higher column number).
    /// If the existing column with the same name has a lower column number, an exception is thrown.</param>
    /// <param name="columnKind">Kind of the column.</param>
    /// <param name="groupNumber">The group number of the column.</param>
    /// <returns>A column with exactly the provided properties at exactly the provided position.</returns>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// If columnNumber is either less than 0 or greater than <see cref="ColumnCount"/>
    /// </exception>
    /// <exception cref="System.InvalidOperationException">
    /// If the provided type is not a subclass of <see cref="DataColumn"/> or is an abstract type.
    /// or
    /// A column with the same name already exists to the left of the provided position.
    /// </exception>
    public TDataCol EnsureExistenceAtPositionStrictly<TDataCol>(int columnNumber, string columnName, ColumnKind columnKind, int groupNumber) where TDataCol : DataColumn
    {
      return (TDataCol)EnsureExistenceAtPositionStrictly(columnNumber, columnName, typeof(TDataCol), columnKind, groupNumber);
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
      if (index < ColumnCount)
      {
        if (this[index].GetType().Equals(datac.GetType()))
        {
          this[index].CopyDataFrom(datac);
        }
        else
        {
          // if the column to add has a parent, we can not add the column directly (we are then not the owner), so we clone it
          Replace(index, datac.ParentObject == null ? datac : (DataColumn)datac.Clone());
        }
      }
      else
      {
        // if the column to add has a parent, we can not add the column directly (we are then not the owner), so we clone it
        Add(datac.ParentObject == null ? datac : (DataColumn)datac.Clone(), name);
      }
    }

    /// <summary>
    /// Replace the column at index <code>index</code> (if index is inside bounds) or add the column.
    /// </summary>
    /// <param name="index">The position of the column which should be replaced.</param>
    /// <param name="datac">The column which replaces the existing column or which is be added to the collection.</param>
    public void ReplaceOrAdd(int index, DataColumn datac)
    {
      if (index < ColumnCount)
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
      if (index >= ColumnCount)
        throw new System.IndexOutOfRangeException(string.Format("Index ({0})for replace operation was outside the bounds, the actual column count is {1}", index, ColumnCount));
      if (null == newCol)
        throw new ArgumentNullException("newCol");
      if (newCol.IsSomeoneListeningToChanges)
        throw new ArgumentException("The column provided in the argument is not a fresh column, because someone is listening already to this column");

      if (null != _deferredDataLoader)
        TryLoadDeferredData();

      DataColumn oldCol = this[index];
      if (object.ReferenceEquals(oldCol, newCol))
        return;

      _columnScripts.TryGetValue(oldCol, out var oldColumnScript);

      int oldRowCount = oldCol.Count;
      DataColumnInfo info = GetColumnInfo(index);
      _columnsByName[info.Name] = newCol;
      _columnsByNumber[index] = newCol;
      _columnInfoByColumn.Remove(oldCol);
      _columnInfoByColumn.Add(newCol, info);

      if (null != oldColumnScript)
      {
        _columnScripts.Remove(oldCol);
        _columnScripts.Add(newCol, oldColumnScript);
      }

      newCol.ParentObject = this;
      oldCol.Dispose();

      EhChildChanged(null, DataColumnCollectionChangedEventArgs.CreateColumnCopyOrReplaceArgs(index, oldRowCount, newCol.Count));
    }

    /// <summary>
    /// Inserts multiple DataColumns into the collection at index <c>nDestinationIndex</c>. The caller must garantuee, that the names are not already be present!
    /// </summary>
    /// <param name="columns">The array of data columns to insert.</param>
    /// <param name="info">The corresponding column information for the columns to insert.</param>
    /// <param name="nDestinationIndex">The index into the collection where the columns are inserted.</param>
    protected void Insert(DataColumn[] columns, DataColumnInfo[] info, int nDestinationIndex)
    {
      Insert(columns, info, nDestinationIndex, false);
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
      using (var suspendToken = SuspendGetToken())
      {
        int indexOfAddedColumns = ColumnCount;
        int numberToAdd = columns.Length;

        // first add the columns to the collection
        for (int i = 0; i < numberToAdd; i++)
        {
          if (renameColumnsIfNecessary && ContainsColumn(info[i].Name))
            info[i].Name = FindUniqueColumnNameWithBase(info[i].Name);

          Add(columns[i], info[i]);
        }

        // then move the columns to the desired position
        ChangeColumnPosition(Altaxo.Collections.ContiguousIntegerRange.FromStartAndCount(indexOfAddedColumns, numberToAdd), nDestinationIndex);

        suspendToken.Dispose();
      }
    }

    /// <summary>
    /// Deletes all columns in the collection, and then copy all columns from the source table.
    /// </summary>
    /// <param name="src">The source collection to copy the columns from.</param>
    public void CopyAllColumnsFrom(DataColumnCollection src)
    {
      using (var suspendToken = SuspendGetToken())
      {
        RemoveColumnsAll();
        for (int i = 0; i < src.ColumnCount; i++)
        {
          Add((DataColumn)src[i].Clone(), src.GetColumnName(i), src.GetColumnKind(i), src.GetColumnGroup(i));
        }
        suspendToken.Dispose();
      }
    }

    /// <summary>
    /// Deletes all columns in the collection, and then copy all columns (but without data) from the source table.
    /// </summary>
    /// <param name="src">The source collection to copy the columns from.</param>
    public void CopyAllColumnsWithoutDataFrom(DataColumnCollection src)
    {
      using (var suspendToken = SuspendGetToken())
      {
        RemoveColumnsAll();
        for (int i = 0; i < src.ColumnCount; i++)
        {
          var newCol = (DataColumn)Activator.CreateInstance(src[i].GetType())!;
          Add(newCol, src.GetColumnName(i), src.GetColumnKind(i), src.GetColumnGroup(i));
        }
        suspendToken.Dispose();
      }
    }

    /// <summary>
    /// Appends data columns from DataTable src to the data in this table.
    /// </summary>
    /// <param name="src">Source table.</param>
    /// <param name="ignoreNames">If true, the data columns in this table and in src table are compared by index. If false,
    /// the data columns in this table and in src table are compared by their name.</param>
    public void AppendAllColumns(DataColumnCollection src, bool ignoreNames)
    {
      AppendAllColumnsToPosition(src, ignoreNames, RowCount);
    }

    /// <summary>
    /// Appends data columns from DataTable src to the data in this table leaving some rows free inbetween.
    /// </summary>
    /// <param name="src">Source table.</param>
    /// <param name="ignoreNames">If true, the data columns in this table and in src table are compared by index. If false,
    /// the data columns in this table and in src table are compared by their name.</param>
    /// <param name="rowSpace">Number of rows to leave free between data in this table and newly appended data.</param>
    public void AppendAllColumnsWithSpace(DataColumnCollection src, bool ignoreNames, int rowSpace)
    {
      AppendAllColumnsToPosition(src, ignoreNames, RowCount + rowSpace);
    }

    /// <summary>
    /// Appends data columns from DataTable src to the data in this table by copying the new data to a specified row.
    /// </summary>
    /// <param name="src">Source table.</param>
    /// <param name="ignoreNames">If true, the data columns in this table and in src table are compared by index. If false,
    /// the data columns in this table and in src table are compared by their name.</param>
    /// <param name="appendPosition">Row number of first row where the new data is copied to.</param>
    public void AppendAllColumnsToPosition(DataColumnCollection src, bool ignoreNames, int appendPosition)
    {
      using (var suspendToken = SuspendGetToken())
      {
        // test structure
        if (!IsColumnStructureCompatible(this, src, ignoreNames))
          throw new ArgumentException(string.Format("DataColumnCollection {0} has another structure than {1}. Append is not possible", src.Name, Name));

        if (ignoreNames)
        {
          for (int i = 0; i < ColumnCount; i++)
          {
            DataColumn destCol = this[i];
            DataColumn srcCol = src[i];
            destCol.AppendToPosition(srcCol, appendPosition);
            _numberOfRows = Math.Max(_numberOfRows, destCol.Count);
          }
        }
        else
        {
          for (int i = 0; i < ColumnCount; i++)
          {
            DataColumn destCol = this[i];
            DataColumn srcCol = src[this[i].Name];
            destCol.AppendToPosition(srcCol, appendPosition);
            _numberOfRows = Math.Max(_numberOfRows, destCol.Count);
          }
        }
        suspendToken.Dispose();
      }
    }

    public static bool IsColumnStructureCompatible(DataColumnCollection a, DataColumnCollection b, bool ignoreColumnNames)
    {
      if (a.ColumnCount != b.ColumnCount)
        return false;

      if (ignoreColumnNames)
      {
        for (int i = 0; i < a.ColumnCount; i++)
        {
          if (a[i].GetType() != b[i].GetType())
            return false;
        }
      }
      else
      {
        for (int i = 0; i < a.ColumnCount; i++)
        {
          string ainame = a[i].Name;
          if (!b.ContainsColumn(ainame))
            return false;
          if (a[i].GetType() != b[ainame].GetType())
            return false;
        }
      }
      return true;
    }

    #endregion Add / Replace Column

    #region Column removal and move to another collection

    /// <summary>
    /// Removes a number of columns of the collection.
    /// </summary>
    /// <param name="nFirstColumn">The first number of column to remove.</param>
    /// <param name="nDelCount">The number of columns to remove.</param>
    public virtual void RemoveColumns(int nFirstColumn, int nDelCount)
    {
      RemoveColumns(ContiguousIntegerRange.FromStartAndCount(nFirstColumn, nDelCount));
    }

    /// <summary>
    /// Removes all columns of the collection.
    /// </summary>
    public virtual void RemoveColumnsAll()
    {
      RemoveColumns(ContiguousIntegerRange.FromStartAndCount(0, ColumnCount));
    }

    /// <summary>
    /// Removes the column at the given index.
    /// </summary>
    /// <param name="nFirstColumn">The index of the column to remove.</param>
    public void RemoveColumn(int nFirstColumn)
    {
      RemoveColumns(nFirstColumn, 1);
    }

    /// <summary>
    /// Remove the column given as the argument.
    /// </summary>
    /// <param name="datac">The column to remove.</param>
    public void RemoveColumn(DataColumn datac)
    {
      RemoveColumns(GetColumnNumber(datac), 1);
    }

    public void RemoveColumns(IAscendingIntegerCollection selectedColumns)
    {
      if (null != _deferredDataLoader)
        TryLoadDeferredData();


      int nOriginalColumnCount = ColumnCount;
      int lastRangeStart = 0;
      var colsToDispose = new List<DataColumn>(selectedColumns.Count);

      foreach (var range in selectedColumns.RangesDescending)
      {
        // first, Dispose the columns and set the places to null
        for (int i = range.LastInclusive; i >= range.Start; i--)
        {
          var colToRemove = _columnsByNumber[i];
          var colName = _columnInfoByColumn[colToRemove].Name;
          _columnScripts.Remove(colToRemove);
          _columnInfoByColumn.Remove(colToRemove);
          _columnsByName.Remove(colName);
          colsToDispose.Add(colToRemove); // dont dispose column here directly, because it may trigger some events, and the handlers want to access this DataColumnCollection, which is now in an undefined state
        }
        _columnsByNumber.RemoveRange(range.Start, range.Count);
        lastRangeStart = range.Start;
      }

      // renumber the remaining columns
      for (int i = _columnsByNumber.Count - 1; i >= lastRangeStart; i--)
        _columnInfoByColumn[_columnsByNumber[i]].Number = i;

      // first, before we dispose the columns, set all columns to DisposeInProgress
      // to prevent access e.g. to parent properties
      foreach (var col in colsToDispose)
        col.SetDisposeInProgress();

      // now dispose all the columns
      foreach (var col in colsToDispose)
        col.Dispose();

      // raise datachange event that some columns have changed
      EhSelfChanged(DataColumnCollectionChangedEventArgs.CreateColumnRemoveArgs(lastRangeStart, nOriginalColumnCount, _numberOfRows));

      // reset the TriedOutRegularNaming flag, maybe one of the regular column names is now free again
      _triedOutRegularNaming = false;
    }

    /// <summary>
    /// Moves some columns of this collection to another collection.
    /// </summary>
    /// <param name="destination">The destination collection where the columns are moved to.</param>
    /// <param name="destindex">The index in the destination collection where the columns are moved to.</param>
    /// <param name="selectedColumns">The indices of the column of the source collection that are moved.</param>
    public void MoveColumnsTo(DataColumnCollection destination, int destindex, IAscendingIntegerCollection selectedColumns)
    {
      if (null != _deferredDataLoader)
        TryLoadDeferredData();

      int nOriginalColumnCount = ColumnCount;
      int numberMoved = selectedColumns.Count;
      var tmpColumn = new DataColumn[numberMoved];
      var tmpInfo = new DataColumnInfo[numberMoved];
      var tmpScript = new IColumnScriptText[numberMoved];

      for (int i = 0; i < numberMoved; i++)
      {
        tmpColumn[i] = (DataColumn)this[selectedColumns[i]].Clone(); // clone the column before moving it! This is essential in order to help DocNodeProxies preserve their target!
        tmpInfo[i] = _columnInfoByColumn[_columnsByNumber[i]];

        if (_columnScripts.TryGetValue(tmpColumn[i], out var script))
        {
          tmpScript[i] = (IColumnScriptText)script.Clone(); // clone the script also
        }
      }

      RemoveColumns(selectedColumns);

      destination.Insert(tmpColumn, tmpInfo, 0, true);

      // Move the column scripts also
      for (int i = 0; i < numberMoved; i++)
      {
        if (tmpScript[i] != null)
          destination._columnScripts.Add(tmpColumn[i], tmpScript[i]);
      }
    }

    /// <summary>
    /// Appends a cloned column from another DataColumnCollection to this collection.
    /// </summary>
    /// <param name="from">The DataColumnCollection to copy from.</param>
    /// <param name="idx">Index of the source column to clone and then append.</param>
    /// <param name="doCopyData">If true, the original source column data a preserved. Otherwise, the column will be cleared after cloning.</param>
    /// <param name="doCopyProperties">If true, the column script will be cloned too.</param>
    /// <returns>The index of the newly appended column.</returns>
    public int AppendCopiedColumnFrom(DataColumnCollection from, int idx, bool doCopyData, bool doCopyProperties)
    {
      string name = from.GetColumnName(idx);
      var newCol = (DataColumn)from[idx].Clone();

      if (!doCopyData)
      {
        newCol.Clear();
      }

      int newIdx = ColumnCount;
      Add(newCol, name, from.GetColumnKind(idx), from.GetColumnGroup(idx));

      if (doCopyProperties)
      {
        from.ColumnScripts.TryGetValue(from[idx], out var script);
        if (null != script)
          ColumnScripts.Add(newCol, (IColumnScriptText)script.Clone());
      }

      return newIdx;
    }

    /// <summary>
    /// Appends a row to this table, which is copied from a source table.
    /// </summary>
    /// <param name="src">Source table to copy the row from.</param>
    /// <param name="srcRow">Index of the row of the source table.</param>
    public void AppendRowFrom(DataColumnCollection src, int srcRow)
    {
      int cols = Math.Min(ColumnCount, src.ColumnCount);
      int destRow = RowCount;
      for (int i = 0; i < cols; i++)
      {
        var destCol = this[i];
        destCol[destRow] = src[i][srcRow];
        if (destCol.Count > _numberOfRows)
          _numberOfRows = destCol.Count;    // we silently update the row count here, otherwise we cannot append more columns
      }
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="from"></param>
    /// <returns></returns>
    public IAscendingIntegerCollection MergeColumnTypesFrom(DataColumnCollection from)
    {
      var coll = new AscendingIntegerCollection();

      for (int i = 0; i < from.ColumnCount; i++)
      {
        string name = from.GetColumnName(i);
        System.Type coltype = from[i].GetType();

        if (ContainsColumn(name) && (this[name].GetType() == from[i].GetType()))
        {
          // then the column is already present with the right name and can be used
          coll.Add(GetColumnNumber(this[name]));
        }
        else // name is there - but type is different, or name is not here at all
        {
          int idx = AppendCopiedColumnFrom(from, i, false, true);
          coll.Add(idx);
        }
      }
      return coll;
    }

    #endregion Column removal and move to another collection

    #region Column Information getting/setting

    /// <summary>
    /// Returns whether the column is contained in this collection.
    /// </summary>
    /// <param name="datac">The column in question.</param>
    /// <returns>True if the column is contained in this collection.</returns>
    public bool ContainsColumn(DataColumn datac)
    {
      return _columnInfoByColumn.ContainsKey(datac);
    }

    /// <summary>
    /// Returns whether the column is contained in this collection.
    /// </summary>
    /// <param name="datac">The column in question.</param>
    /// <returns>True if the column is contained in this collection.</returns>
    public bool Contains(DataColumn datac)
    {
      return _columnInfoByColumn.ContainsKey(datac);
    }

    /// <summary>
    /// Test if a column of a given name is present in this collection.
    /// </summary>
    /// <param name="columnname">The columnname to test for presence.</param>
    /// <returns>True if the column with the name is contained in the collection.</returns>
    public bool ContainsColumn(string columnname)
    {
      return _columnsByName.ContainsKey(columnname);
    }

    /// <summary>
    /// Test if a column of a given name is present in this collection.
    /// </summary>
    /// <param name="columnname">The columnname to test for presence.</param>
    /// <returns>True if the column with the name is contained in the collection.</returns>
    public bool Contains(string columnname)
    {
      return _columnsByName.ContainsKey(columnname);
    }

    /// <summary>
    /// Check if the named column is of the expected type.
    /// </summary>
    /// <param name="columnname">Name of the column.</param>
    /// <param name="expectedtype">Expected type of column.</param>
    /// <returns>True if the column exists and is exactly! of the expeced type. False otherwise.</returns>
    public bool IsColumnOfType(string columnname, System.Type expectedtype)
    {
      if (_columnsByName.ContainsKey(columnname) && _columnsByName[columnname].GetType() == expectedtype)
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
      if (_columnInfoByColumn.TryGetValue(datac, out var info))
        return info.Number;
      else
        return -1;
    }

    /// <summary>
    /// Returns the name of a column.
    /// </summary>
    /// <param name="datac">The column..</param>
    /// <returns>The name of the column.</returns>
    public string? GetColumnName(DataColumn datac)
    {
      if (_columnInfoByColumn.TryGetValue(datac, out var info))
        return info.Name;
      else
        return null;
    }

    /// <summary>
    /// Returns the name of the column at position idx.
    /// </summary>
    /// <param name="idx">The position of the column (column number).</param>
    /// <returns>The name of the column.</returns>
    public string GetColumnName(int idx)
    {
      if (idx >= 0 && idx < _columnsByNumber.Count)
        return GetColumnInfo(idx).Name;
      else
        throw new ArgumentOutOfRangeException(string.Format("The column [{0}] in table \"{1}\" does not exist. The current number of columns is {2}.", idx, Main.AbsoluteDocumentPath.GetPathString(this, 2), ColumnCount));
    }

    /// <summary>
    /// Sets the name of the column <code>datac</code>.
    /// </summary>
    /// <param name="datac">The column which name is to set.</param>
    /// <param name="newName">The new name of the column.</param>
    public void SetColumnName(DataColumn datac, string newName)
    {
      string oldName = GetColumnInfo(datac).Name;
      if (oldName != newName)
      {
        if (ContainsColumn(newName))
        {
          throw new System.ApplicationException("Try to set column name to the name of a already present column: " + newName);
        }
        else
        {
          GetColumnInfo(datac).Name = newName;
          _columnsByName.Remove(oldName);
          _columnsByName.Add(newName, datac);

          EhChildChanged(null, DataColumnCollectionChangedEventArgs.CreateColumnRenameArgs(GetColumnNumber(datac)));


          EhSelfTunnelingEventHappened(new NameChangedEventArgs(datac, oldName, newName));
          // Inform also the data column itself that the name has changed
          datac.EhSelfTunnelingEventHappened(Main.DocumentPathChangedEventArgs.Empty);

          // reset the TriedOutRegularNames flag, maybe one of the regular columns has been renamed
          _triedOutRegularNaming = false;
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
      SetColumnName(this[index], newName);
    }

    /// <summary>
    /// Rename the column with the name <code>oldName</code> to <code>newName</code>.
    /// </summary>
    /// <param name="oldName">The old name of the column.</param>
    /// <param name="newName">The new name of the column.</param>
    public void SetColumnName(string oldName, string newName)
    {
      SetColumnName(this[oldName], newName);
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
      var groupNums = new System.Collections.SortedList();
      for (int i = 0; i < ColumnCount; i++)
      {
        int group = GetColumnGroup(i);
        if (!groupNums.ContainsKey(group))
          groupNums.Add(group, null);
      }

      for (int i = 0; i < int.MaxValue; i++)
      {
        if (!groupNums.Contains(i))
          return i;
      }
      return 0;
    }

    /// <summary>
    /// Determines if this collections has Columns only with one group number, or if it has columns with multiple group numbers.
    /// </summary>
    /// <returns>False if all the columns have the same group number. True if the columns have more than one group number.</returns>
    public bool HaveMultipleGroups()
    {
      if (ColumnCount <= 1)
        return false;
      int firstgroup = GetColumnGroup(0);
      for (int i = 1; i < ColumnCount; i++)
        if (firstgroup != GetColumnGroup(i))
          return true;

      return false;
    }

    /// <summary>
    /// Sets the group number of the column with the given column number <code>idx</code>.
    /// </summary>
    /// <param name="idx">The column number of the column.</param>
    /// <param name="groupNumber">The group number to set for this column.</param>
    public void SetColumnGroup(int idx, int groupNumber)
    {
      GetColumnInfo(idx).Group = groupNumber;
      EnsureUniqueColumnKindsForIndependentVariables(groupNumber, this[idx]);
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
    /// Sets the kind of the column.
    /// </summary>
    /// <param name="datac">The column for which to set the kind (have to be member of this collection).</param>
    /// <param name="columnKind">The new kind of the column.</param>
    public void SetColumnKind(DataColumn datac, ColumnKind columnKind)
    {
      DataColumnInfo info = GetColumnInfo(datac);
      info.Kind = columnKind;
      EnsureUniqueColumnKindsForIndependentVariables(info.Group, datac);
    }

    /// <summary>
    /// Sets the kind of the column with column number <code>idx</code>.
    /// </summary>
    /// <param name="idx">The column number of the column.</param>
    /// <param name="columnKind">The new kind of the column.</param>
    public void SetColumnKind(int idx, ColumnKind columnKind)
    {
      SetColumnKind(this[idx], columnKind);
    }

    /// <summary>
    /// Sets the kind of the column with name <paramref name="columnName"/>.
    /// </summary>
    /// <param name="columnName">The name of the column.</param>
    /// <param name="columnKind">The new kind of the column.</param>
    public void SetColumnKind(string columnName, ColumnKind columnKind)
    {
      SetColumnKind(this[columnName], columnKind);
    }

    /// <summary>
    /// Ensures that for a given group number there is only one column for each independent variable (X,Y,Z).
    /// </summary>
    /// <param name="groupNumber">The group number of the columns which are checked for this rule.</param>
    /// <param name="exceptThisColumn">If not null, this column is treated with priority. If this column is a independent variable column, it can
    /// keep its kind.</param>
    protected void EnsureUniqueColumnKindsForIndependentVariables(int groupNumber, DataColumn exceptThisColumn)
    {
      bool X_present = false;
      bool Y_present = false;
      bool Z_present = false;

      if (exceptThisColumn != null)
      {
        switch (GetColumnInfo(exceptThisColumn).Kind)
        {
          case ColumnKind.X:
            X_present = true;
            break;

          case ColumnKind.Y:
            Y_present = true;
            break;

          case ColumnKind.Z:
            Z_present = true;
            break;
        }
      }

      foreach (KeyValuePair<DataColumn, DataColumnInfo> entry in _columnInfoByColumn)
      {
        if ((entry.Value).Group == groupNumber && !entry.Key.Equals(exceptThisColumn))
        {
          DataColumnInfo info = entry.Value;
          switch (info.Kind)
          {
            case ColumnKind.X:
              if (X_present)
                info.Kind = ColumnKind.V;
              else
                X_present = true;
              break;

            case ColumnKind.Y:
              if (Y_present)
                info.Kind = ColumnKind.V;
              else
                Y_present = true;
              break;

            case ColumnKind.Z:
              if (Z_present)
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
      int len = ColumnCount;
      for (int i = len - 1; i >= 0; i--)
      {
        var dc = _columnsByNumber[i];
        var info = _columnInfoByColumn[dc];
        if (info.Group == nGroup && info.Kind == ColumnKind.X && !dc.Equals(exceptThisColumn))
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
    public Altaxo.Data.DataColumn? FindXColumnOf(DataColumn datac)
    {
      return FindXColumnOfGroup(GetColumnGroup(datac));
    }

    /// <summary>
    /// Returns the X column of the column group <code>nGroup</code>.
    /// </summary>
    /// <param name="nGroup">The column group number.</param>
    /// <returns>The X column of the provided group, or null if it is not found.</returns>
    public Altaxo.Data.DataColumn? FindXColumnOfGroup(int nGroup)
    {
      int len = ColumnCount;
      for (int i = len - 1; i >= 0; i--)
      {
        var dc = _columnsByNumber[i];
        var info = _columnInfoByColumn[dc];
        if (info.Group == nGroup && info.Kind == ColumnKind.X)
        {
          return dc;
        }
      }
      return null;
    }

    /// <summary>
    /// Using a given column, find the related Y column of this.
    /// </summary>
    /// <param name="datac">The column for which to find the related Y column.</param>
    /// <returns>The related Y column, or null if it is not found.</returns>
    public Altaxo.Data.DataColumn? FindYColumnOf(DataColumn datac)
    {
      return FindYColumnOfGroup(GetColumnGroup(datac));
    }

    /// <summary>
    /// Returns the Y column of the column group <code>nGroup</code>.
    /// </summary>
    /// <param name="nGroup">The column group number.</param>
    /// <returns>The Y column of the provided group, or null if it is not found.</returns>
    public Altaxo.Data.DataColumn? FindYColumnOfGroup(int nGroup)
    {
      int len = ColumnCount;
      for (int i = len - 1; i >= 0; i--)
      {
        var dc = _columnsByNumber[i];
        var info = _columnInfoByColumn[dc];
        if (info.Group == nGroup && info.Kind == ColumnKind.Y)
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
      return _columnInfoByColumn[datac];
    }

    /// <summary>
    /// Get the column info for the column with index<code>idx</code>.
    /// </summary>
    /// <param name="idx">The column index of the column for which the column information is returned.</param>
    /// <returns>The column information of the column.</returns>
    private DataColumnInfo GetColumnInfo(int idx)
    {
      return _columnInfoByColumn[_columnsByNumber[idx]];
    }

    /// <summary>
    /// Get the column info for the column with name <code>columnName</code>.
    /// </summary>
    /// <param name="columnName">The column name of the column for which the column information is returned.</param>
    /// <returns>The column information of the column.</returns>
    private DataColumnInfo GetColumnInfo(string columnName)
    {
      return _columnInfoByColumn[_columnsByName[columnName]];
    }

    #endregion Column Information getting/setting

    #region Row insertion/removal

    /// <summary>
    /// Insert an empty row at the provided row number.
    /// </summary>
    /// <param name="atRowNumber">The row number at which to insert.</param>
    public void InsertRow(int atRowNumber)
    {
      InsertRows(atRowNumber, 1);
    }

    /// <summary>
    /// Insert a number of empty rows in all columns.
    /// </summary>
    /// <param name="nBeforeRow">The row number before which the additional rows should be inserted.</param>
    /// <param name="nRowsToInsert">The number of rows to insert.</param>
    public void InsertRows(int nBeforeRow, int nRowsToInsert)
    {
      using (var suspendToken = SuspendGetToken())
      {
        for (int i = 0; i < ColumnCount; i++)
          this[i].InsertRows(nBeforeRow, nRowsToInsert);

        suspendToken.Dispose();
      }
    }

    /// <summary>
    /// Removes a number of rows from all columns.
    /// </summary>
    /// <param name="nFirstRow">Number of first row to remove.</param>
    /// <param name="nCount">Number of rows to remove, starting from nFirstRow.</param>
    public void RemoveRows(int nFirstRow, int nCount)
    {
      RemoveRows(ContiguousIntegerRange.FromStartAndCount(nFirstRow, nCount));
    }

    /// <summary>
    /// Removes the <code>selectedRows</code> from the table.
    /// </summary>
    /// <param name="selectedRows">Collection of indizes to the rows that should be removed.</param>
    public void RemoveRows(IAscendingIntegerCollection selectedRows)
    {
      RemoveRowsInColumns(ContiguousIntegerRange.FromStartAndCount(0, ColumnCount), selectedRows);
    }

    /// <summary>
    /// Removes all rows.
    /// </summary>
    public void RemoveRowsAll()
    {
      RemoveRows(0, RowCount);
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
      using (var suspendToken = SuspendGetToken())
      {
        for (int selcol = 0; selcol < selectedColumns.Count; selcol++)
        {
          int colidx = selectedColumns[selcol];

          foreach (var range in selectedRows.RangesDescending)
          {
            this[colidx].RemoveRows(range.Start, range.Count);
          }
        }

        suspendToken.Dispose();
      }
    }

    /// <summary>
    /// Removes a single row of all columns.
    /// </summary>
    /// <param name="nFirstRow">The row to remove.</param>
    public void RemoveRow(int nFirstRow)
    {
      RemoveRows(nFirstRow, 1);
    }

    #endregion Row insertion/removal

    #region Column and row position manipulation

    public void SwapColumnPositions(int i, int j)
    {
      if (i == j)
        return;
      if (i < 0 || i >= ColumnCount)
        throw new ArgumentOutOfRangeException("Index i is out of rage");
      if (j < 0 || j >= ColumnCount)
        throw new ArgumentOutOfRangeException("Index j is out of rage");

      var coli = _columnsByNumber[i];
      var colj = _columnsByNumber[j];

      _columnsByNumber[i] = colj;
      _columnsByNumber[j] = coli;
      _columnInfoByColumn[coli].Number = j;
      _columnInfoByColumn[colj].Number = i;

      EhChildChanged(null, DataColumnCollectionChangedEventArgs.CreateColumnMoveArgs(Math.Min(i, j), Math.Max(i, j)));
    }

    /// <summary>
    /// Moves one or more columns to a new position.
    /// </summary>
    /// <param name="selectedColumns">The indices of the columns to move to the new position.</param>
    /// <param name="newPosition">The new position where the columns are moved to.</param>
    /// <remarks>An exception is thrown if newPosition is negative or higher than possible.</remarks>
    public void ChangeColumnPosition(Altaxo.Collections.IAscendingIntegerCollection selectedColumns, int newPosition)
    {
      if (null != _deferredDataLoader)
        TryLoadDeferredData();

      int numberSelected = selectedColumns.Count;
      if (numberSelected == 0)
        return;

      int oldPosition = selectedColumns[0];
      if (oldPosition == newPosition)
        return;

      // check that the newPosition is ok
      if (newPosition < 0)
        throw new ArgumentException("New column position is negative!");
      if (newPosition + numberSelected > ColumnCount)
        throw new ArgumentException(string.Format("New column position too high: ColsToMove({0})+NewPosition({1})>ColumnCount({2})", numberSelected, newPosition, ColumnCount));

      // Allocated tempory storage for the datacolumns
      var columnsMoved = new Altaxo.Data.DataColumn[selectedColumns.Count];
      // fill temporary storage
      for (int i = 0; i < selectedColumns.Count; i++)
        columnsMoved[i] = this[selectedColumns[i]];

      int firstAffectedColumn = 0;
      int maxAffectedColumn = ColumnCount;
      if (newPosition < oldPosition) // move down to lower indices
      {
        firstAffectedColumn = newPosition;
        maxAffectedColumn = Math.Max(newPosition + numberSelected, selectedColumns[numberSelected - 1] + 1);

        for (int i = selectedColumns[numberSelected - 1], offset = 0; i >= firstAffectedColumn; i--)
        {
          if (numberSelected > offset && i == selectedColumns[numberSelected - 1 - offset])
            offset++;
          else
            _columnsByNumber[i + offset] = _columnsByNumber[i];
        }
      }
      else // move up to higher
      {
        firstAffectedColumn = selectedColumns[0];
        maxAffectedColumn = newPosition + numberSelected;
        for (int i = selectedColumns[0], offset = 0; i < maxAffectedColumn; i++)
        {
          if (offset < numberSelected && i == selectedColumns[offset])
            offset++;
          else
            _columnsByNumber[i - offset] = _columnsByNumber[i];
        }
      }

      // Fill in temporary stored columns on new position
      for (int i = 0; i < numberSelected; i++)
        _columnsByNumber[newPosition + i] = columnsMoved[i];

      RefreshColumnIndices();
      EhChildChanged(null, DataColumnCollectionChangedEventArgs.CreateColumnMoveArgs(firstAffectedColumn, maxAffectedColumn));
    }

    /// <summary>
    /// Moves on or more rows in to a new position.
    /// </summary>
    /// <param name="selectedIndices">The indices of the rows to move.</param>
    /// <param name="newPosition">The new position of the rows.</param>
    public void ChangeRowPosition(Altaxo.Collections.IAscendingIntegerCollection selectedIndices, int newPosition)
    {
      int numberSelected = selectedIndices.Count;
      if (numberSelected == 0)
        return;

      int oldPosition = selectedIndices[0];
      if (oldPosition == newPosition)
        return;

      // check that the newPosition is ok
      if (newPosition < 0)
        throw new ArgumentException("New row position is negative!");

      // Allocated tempory storage for the datacolumns
      var tempMoved = new Altaxo.Data.AltaxoVariant[numberSelected];

      int firstAffected;
      int maxAffected;

      if (newPosition < oldPosition) // move down to lower indices
      {
        firstAffected = newPosition;
        maxAffected = Math.Max(newPosition + numberSelected, selectedIndices[numberSelected - 1] + 1);
      }
      else
      {
        firstAffected = selectedIndices[0];
        maxAffected = newPosition + numberSelected;
      }

      for (int nColumn = ColumnCount - 1; nColumn >= 0; nColumn--)
      {
        Altaxo.Data.DataColumn thiscolumn = this[nColumn];
        // fill temporary storage
        for (int i = 0; i < numberSelected; i++)
          tempMoved[i] = thiscolumn[selectedIndices[i]];

        if (newPosition < oldPosition) // move down to lower indices
        {
          for (int i = selectedIndices[numberSelected - 1], offset = 0; i >= firstAffected; i--)
          {
            if (numberSelected > offset && i == selectedIndices[numberSelected - 1 - offset])
              offset++;
            else
              thiscolumn[i + offset] = thiscolumn[i];
          }
        }
        else // move up to higher
        {
          for (int i = selectedIndices[0], offset = 0; i < maxAffected; i++)
          {
            if (offset < numberSelected && i == selectedIndices[offset])
              offset++;
            else
              thiscolumn[i - offset] = thiscolumn[i];
          }
        }

        // Fill in temporary stored columns on new position
        for (int i = 0; i < numberSelected; i++)
          thiscolumn[newPosition + i] = tempMoved[i];
      }
      EhChildChanged(null, DataColumnCollectionChangedEventArgs.CreateRowMoveArgs(ColumnCount, firstAffected, maxAffected));
    }

    /// <summary>
    /// This will refresh the column number information in the m_ColumnInfo collection of <see cref="DataColumnInfo" />.
    /// </summary>
    protected void RefreshColumnIndices()
    {
      for (int i = ColumnCount - 1; i >= 0; i--)
      {
        _columnInfoByColumn[_columnsByNumber[i]].Number = i;
      }
    }

    #endregion Column and row position manipulation

    #region Indexer

    /// <summary>
    /// Returns the column with name <code>s</code>. Sets the column with name <code>s</code> by copying data from
    /// the other column (not by replacing). An exception is thrown if the two columns are not of the same type.
    /// </summary>
    public Altaxo.Data.DataColumn this[string s]
    {
      get
      {
        if (!_columnsByName.TryGetValue(s, out var result))
          throw new ArgumentOutOfRangeException(string.Format("The column \"{0}\" in \"{1}\" does not exist.", s, Main.AbsoluteDocumentPath.GetPathString(this, 2)));

        if (null != _deferredDataLoader)
        {
          TryLoadDeferredData();
          result = _columnsByName[s];
        }
        return result;
      }
      set
      {
        // setting a column should not change its name nor its other properties
        // only the data array and the related parameters should be changed
        if (_columnsByName.TryGetValue(s, out var c))
          c.CopyDataFrom(value);
        else
          throw new ArgumentOutOfRangeException(string.Format("The column \"{0}\" in \"{1}\" does not exist.", s, Main.AbsoluteDocumentPath.GetPathString(this, 2)));
      }
    }

    /// <summary>
    /// Returns the column with name <code>s</code>. 
    /// </summary>
    /// <param name="s">The name of the column to retrieve.</param>
    /// <returns>Either the column with the given name, or null if such a column don't exist.</returns>
    public Altaxo.Data.DataColumn? TryGetColumn(string s)
    {
      if (!_columnsByName.TryGetValue(s, out var result))
        return null;

      if (null != _deferredDataLoader)
      {
        TryLoadDeferredData();
        result = _columnsByName[s];
      }

      return result;
    }

    /// <summary>
    /// Returns the column at index <code>idx</code>. Sets the column at index<code>idx</code> by copying data from
    /// the other column (not by replacing). An exception is thrown if the two columns are not of the same type.
    /// </summary>
    public Altaxo.Data.DataColumn this[int idx]
    {
      get
      {
        if (null != _deferredDataLoader)
          TryLoadDeferredData();

        try
        {
          return _columnsByNumber[idx];
        }
        catch (Exception)
        {
          throw new ArgumentOutOfRangeException(string.Format("The column [{0}] in table \"{1}\" does not exist. The current number of columns is {2}.", idx, Main.AbsoluteDocumentPath.GetPathString(this, 2), ColumnCount));
        }
      }
      set
      {
        if (null != _deferredDataLoader)
          TryLoadDeferredData();

        // setting a column should not change its name nor its other properties
        // only the data array and the related parameters should be changed

        if (idx < _columnsByNumber.Count)
        {
          var c = _columnsByNumber[idx];
          c.CopyDataFrom(value);
        }
        else
        {
          throw new ArgumentOutOfRangeException(string.Format("The column [{0}] in table \"{1}\" does not exist. The current number of columns is {2}.", idx, Main.AbsoluteDocumentPath.GetPathString(this, 2), ColumnCount));
        }
      }
    }

    #endregion Indexer

    #region Collection Properties

    /// <summary>
    /// Is true if any data in a child DataColumn has changed since the last saving of the project.
    /// This flag is <b>not</b> set to true if other parts of this collection changed, for instance the column scripts.
    /// </summary>
    public bool IsDataDirty
    {
      get
      {
        return _isDataDirty;
      }
    }

    /// <summary>
    /// Sets the data dirty flag. Setting this flag will force the table to be serialized during the next project saving.
    /// </summary>
    public void SetDataDirty()
    {
      _isDataDirty = true;
    }

    /// <summary>
    /// The row count, i.e. the maximum of the row counts of all columns.
    /// </summary>
    public int RowCount
    {
      get
      {
        if (_hasNumberOfRowsDecreased)
          RefreshRowCount(true);

        return _numberOfRows;
      }
    }

    /// <summary>
    /// The number of columns in the collection.
    /// </summary>
    public int ColumnCount
    {
      get
      {
        return _columnsByNumber.Count;
      }
    }

    /// <summary>
    /// Returns the collection of column scripts.
    /// </summary>
    public ColumnScriptCollection ColumnScripts
    {
      get { return _columnScripts; }
    }

    /// <summary>
    /// Returns an array containing all column names that are contained in this collection.
    /// </summary>
    /// <returns>An array containing all column names that are contained in this collection.</returns>
    public string[] GetColumnNames()
    {
      string[] arr = new string[ColumnCount];
      for (int i = 0; i < arr.Length; i++)
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
      firstdifferentcolumnindex = 0;

      if (0 == ColumnCount)
        return true;

      System.Type t1st = this[0].GetType();

      int len = ColumnCount;
      for (int i = 0; i < len; i++)
      {
        if (this[i].GetType() != t1st)
        {
          firstdifferentcolumnindex = i;
          return false;
        }
      }

      return true;
    }

    /// <summary>
    /// Gets all group numbers in this table as a sorted set of numbers.
    /// </summary>
    /// <returns>All group numbers in this table as a sorted set of numbers.</returns>
    public SortedSet<int> GetGroupNumbersAll()
    {
      return new SortedSet<int>(_columnInfoByColumn.Values.Select(x => x.Group));
    }

    /// <summary>
    /// Gets all columns with the provided group number.
    /// </summary>
    /// <param name="groupNumber">The group number.</param>
    /// <returns>A list with all data columns which have the provided group number. If no such columns exist, an emtpy list will be returned.</returns>
    public List<DataColumn> GetListOfColumnsWithGroupNumber(int groupNumber)
    {
      return new List<DataColumn>(_columnInfoByColumn.Where(entry => entry.Value.Group == groupNumber).Select(entry => entry.Key));
    }

    /// <summary>
    /// Gets all columns with the provided group number as a dictionary where the key is the column name and the value is the column itself.
    /// </summary>
    /// <param name="groupNumber">The group number.</param>
    /// <returns>A dictionary with all data columns which have the provided group number. If no such columns exist, an emtpy dictionary will be returned.</returns>
    public Dictionary<string, DataColumn> GetNameDictionaryOfColumnsWithGroupNumber(int groupNumber)
    {
      var result = new Dictionary<string, DataColumn>();
      foreach (var entry in _columnInfoByColumn.Where(entry => entry.Value.Group == groupNumber))
        result.Add(entry.Value.Name, entry.Key);
      return result;
    }

    /// <summary>
    /// Gets an enumeration of all the names of columns with a given group number.
    /// </summary>
    /// <param name="groupNumber">The group number.</param>
    /// <returns>Enumeration of all the names of columns with a given group number.</returns>
    public IEnumerable<string> GetNamesOfColumnsWithGroupNumber(int groupNumber)
    {
      return _columnInfoByColumn.Values.Where(entry => entry.Group == groupNumber).Select(entry => entry.Name);
    }

    #endregion Collection Properties

    #region Event handling

    /// <summary>
    /// Returns true if this object has outstanding changed not reported yet to the parent.
    /// </summary>
    public virtual bool IsDirty
    {
      get
      {
        return null != _accumulatedEventData;
      }
    }

    /// <summary>
    /// Accumulates the changes reported by the DataColumns.
    /// </summary>
    /// <param name="sender">One of the columns of this collection.</param>
    /// <param name="e">The change details.</param>
    /// <param name="accumulatedEventData">The instance were the event arg e is accumulated. If this parameter is <c>null</c>, a new instance of <see cref="DataColumnCollectionChangedEventArgs"/> is created and returned into this parameter.</param>
    protected void AccumulateChangeData(object? sender, EventArgs e, [NotNull] ref DataColumnCollectionChangedEventArgs? accumulatedEventData)
    {
      if (sender is DataColumn senderAsDataColumn) // ChangeEventArgs from a DataColumn
      {
        _isDataDirty = true;
        if (e is DataColumnChangedEventArgs dataColumnChangeEventArgs)
        {
          int columnNumberOfSender = GetColumnNumber(senderAsDataColumn);
          int rowCountOfSender = senderAsDataColumn.Count;

          if (accumulatedEventData is null)
            accumulatedEventData = new DataColumnCollectionChangedEventArgs(columnNumberOfSender, dataColumnChangeEventArgs.MinRowChanged, dataColumnChangeEventArgs.MaxRowChanged, dataColumnChangeEventArgs.HasRowCountDecreased);
          else
            accumulatedEventData.Accumulate(columnNumberOfSender, dataColumnChangeEventArgs.MinRowChanged, dataColumnChangeEventArgs.MaxRowChanged, dataColumnChangeEventArgs.HasRowCountDecreased);

          // update the row count
          if (_numberOfRows < rowCountOfSender)
            _numberOfRows = rowCountOfSender;
          _hasNumberOfRowsDecreased |= dataColumnChangeEventArgs.HasRowCountDecreased;
        }
      }
      else if (e is DataColumnCollectionChangedEventArgs dataColumnCollectionChangeEventArgs) // ChangeEventArgs from a DataColumnCollection, i.e. from myself
      {
        _isDataDirty = true;
        if (accumulatedEventData is null)
          accumulatedEventData = dataColumnCollectionChangeEventArgs;
        else
          accumulatedEventData.Accumulate(dataColumnCollectionChangeEventArgs);

        // update the row count
        if (_numberOfRows < dataColumnCollectionChangeEventArgs.MaxRowChanged)
          _numberOfRows = dataColumnCollectionChangeEventArgs.MaxRowChanged;
        _hasNumberOfRowsDecreased |= dataColumnCollectionChangeEventArgs.HasRowCountDecreased;
      }

      if (accumulatedEventData is null)
        throw new InvalidProgramException($"Sender: {sender} Args: {e}");

    }

    /// <summary>
    /// Accumulates the changes reported by the DataColumns.
    /// </summary>
    /// <param name="sender">One of the columns of this collection.</param>
    /// <param name="e">The change details.</param>
    protected override void AccumulateChangeData(object? sender, EventArgs e)
    {
      AccumulateChangeData(sender, e, ref _accumulatedEventData);
    }

    protected override bool HandleHighPriorityChildChangeCases(object? sender, ref EventArgs e)
    {
      if (e is Main.ParentChangedEventArgs parentChangedEventArgs && sender is DataColumn column)
      {
        if (object.ReferenceEquals(this, parentChangedEventArgs.OldParent) && ContainsColumn(column))
          RemoveColumn((DataColumn)sender);
        else if (object.ReferenceEquals(this, parentChangedEventArgs.NewParent) && !ContainsColumn(column))
          throw new ApplicationException("Not allowed to set child's parent to this collection before adding it to the collection");

        return true;
      }

      return false;
    }

    /// <summary>
    /// Handles the cases when a child changes, but a reaction is neccessary only if the table is not suspended currently.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
    /// <returns>
    /// True if the event has not changed the state of the table (i.e. it requires no further action).
    /// </returns>
    /// <remarks>
    /// This instance is always sending EventArgs of type <see cref="DataColumnCollectionChangedEventArgs"/>. This means, that event args of any other type should be transformed in <see cref="DataColumnCollectionChangedEventArgs"/>.
    /// </remarks>
    protected override bool HandleLowPriorityChildChangeCases(object? sender, ref EventArgs e)
    {
      DataColumnCollectionChangedEventArgs? result = null;
      AccumulateChangeData(sender, e, ref result); // Get ChangeEventArgs in the result.
      e = result;

      return false;
    }

    protected override void OnTunnelingEvent(IDocumentLeafNode originalSource, TunnelingEventArgs e)
    {
      if (e is DirtyResetEventArgs)
      {
        _isDataDirty = false;
      }

      base.OnTunnelingEvent(originalSource, e);
    }

    #endregion Event handling

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
      if (null != _deferredDataLoader)
        TryLoadDeferredData();

      int rowCount = 0;
      if (bSearchOnlyUntilOldRowCountReached)
      {
        foreach (DataColumn c in _columnsByNumber)
        {
          rowCount = System.Math.Max(rowCount, c.Count);
          if (rowCount >= _numberOfRows)
            break;
        }
      }
      else
      {
        foreach (DataColumn c in _columnsByNumber)
          rowCount = System.Math.Max(rowCount, c.Count);
      }

      // now take over the new row count
      _numberOfRows = rowCount;
      _hasNumberOfRowsDecreased = false; // row count is now actual
    }

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
    public static string? GetNextColumnName(string? lastName)
    {
      if (lastName is null)
        return "A";

      int lastNameLength = lastName.Length;
      if (1 == lastNameLength)
      {
        char _1st = lastName[0];
        _1st++;
        if (_1st >= 'A' && _1st <= 'Z')
          return _1st.ToString();
        else
          return "AA";
      }
      else if (2 == lastNameLength)
      {
        char _1st = lastName[0];
        char _2nd = lastName[1];
        _2nd++;

        if (_2nd < 'A' || _2nd > 'Z')
        {
          _1st++;
          _2nd = 'A';
        }

        if (_1st >= 'A' && _1st <= 'Z')
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
      string? tryName;
      if (_triedOutRegularNaming)
      {
        for (; ; )
        {
          tryName = ((uint)System.Guid.NewGuid().GetHashCode()).ToString("X8");
          if (!_columnsByName.ContainsKey(tryName))
            return tryName;
        }
      }
      else
      {
        // First try it with the next name after the last column
        tryName = GetNextColumnName(_columnsByNumber.Count > 0 ? GetColumnName(ColumnCount - 1) : "");
        if (!string.IsNullOrEmpty(tryName) && !_columnsByName.ContainsKey(tryName))
          return tryName;

        // then try it with all names from A-ZZ
        for (tryName = "A"; tryName != null; tryName = GetNextColumnName(tryName))
        {
          if (!_columnsByName.ContainsKey(tryName))
            return tryName;
        }
        // if no success, set the _TriedOutRegularNaming
        _triedOutRegularNaming = true;
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
      if (!_columnsByName.ContainsKey(sbase))
        return sbase;

      sbase += ".";

      // then try it with all names from A-ZZ

      for (string? tryAppendix = "A"; tryAppendix != null; tryAppendix = GetNextColumnName(tryAppendix))
      {
        if (!_columnsByName.ContainsKey(sbase + tryAppendix))
          return sbase + tryAppendix;
      }

      // if no success, append a hex string
      for (; ; )
      {
        string tryName = sbase + ((uint)System.Guid.NewGuid().GetHashCode()).ToString("X8");
        if (!_columnsByName.ContainsKey(tryName))
          return tryName;
      }
    }

    /// <summary>
    /// Get a unique column name based on a provided string.
    /// </summary>
    /// <param name="sbase">The base name. Can be null.</param>
    /// <returns>An unique column name based on the provided string.</returns>
    public string FindUniqueColumnName(string? sbase)
    {
      return sbase is null ? FindUniqueColumnNameWithoutBase() : FindUniqueColumnNameWithBase(sbase);
    }

    #endregion Automatic column naming

    #region INamedObjectCollection Members

    /// <summary>
    /// Returns the column with the name <code>name</code>.
    /// </summary>
    /// <param name="name">The name of the column to retrieve.</param>
    /// <returns>The column with name <code>name</code>, or null if not found.</returns>
    public override Main.IDocumentLeafNode? GetChildObjectNamed(string name)
    {
      if (_columnsByName.TryGetValue(name, out var col))
      {
        if (null != _deferredDataLoader)
          TryLoadDeferredData();

        return col;
      }
      else
      {
        return null;
      }
    }

    /// <summary>
    /// Retrieves the name of a child column <code>o</code>.
    /// </summary>
    /// <param name="o">The child column.</param>
    /// <returns>The name of the column.</returns>
    public override string? GetNameOfChildObject(Main.IDocumentLeafNode o)
    {
      if ((o is DataColumn) && _columnInfoByColumn.TryGetValue((DataColumn)o, out var info))
        return info.Name;
      else
        return null;
    }

    protected override IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
    {
      for (int i = _columnsByNumber.Count - 1; i >= 0; --i)
      {
        var col = _columnsByNumber[i];
        if (_columnInfoByColumn.TryGetValue(col, out var info))
          yield return new Main.DocumentNodeAndName(col, info.Name);
      }
    }

    #endregion INamedObjectCollection Members

    #region IList<DataRow> support

    #region IEnumerable<DataRow> Members

    public IEnumerator<DataRow> GetEnumerator()
    {
      for (int i = 0; i < RowCount; i++)
        yield return new DataRow(this, i);
    }

    #endregion IEnumerable<DataRow> Members

    #region IEnumerable Members

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      for (int i = 0; i < RowCount; i++)
        yield return new DataRow(this, i);
    }

    #endregion IEnumerable Members

    #region IList<DataRow> Members

    int IList<DataRow>.IndexOf(DataRow item)
    {
      return item.RowIndex;
    }

    void IList<DataRow>.Insert(int index, DataRow item)
    {
      InsertRows(index, 1);

      // if by insertion of a row we changed to row that was meant for insertion, we have to correct the index;
      if (index <= item.RowIndex && object.ReferenceEquals(this, item.ColumnCollection))
        item = new DataRow(item.ColumnCollection, item.RowIndex + 1);

      for (int i = 0; i < ColumnCount; i++)
        this[i][index] = item[i];
    }

    void IList<DataRow>.RemoveAt(int index)
    {
      RemoveRow(index);
    }

    DataRow IList<DataRow>.this[int index]
    {
      get
      {
        return new DataRow(this, index);
      }
      set
      {
        int nCols = Math.Min(ColumnCount, value.ColumnCollection.ColumnCount);
        for (int i = nCols - 1; i >= 0; i--)
          this[i][index] = value[i];
      }
    }

    #endregion IList<DataRow> Members

    #region ICollection<DataRow> Members

    void ICollection<DataRow>.Add(DataRow item)
    {
      int nCols = Math.Min(ColumnCount, item.ColumnCollection.ColumnCount);
      int index = RowCount;
      for (int i = nCols - 1; i >= 0; i--)
        this[i][index] = item[i];
    }

    void ICollection<DataRow>.Clear()
    {
      ClearData();
    }

    bool ICollection<DataRow>.Contains(DataRow item)
    {
      return object.ReferenceEquals(this, item.ColumnCollection);
    }

    void ICollection<DataRow>.CopyTo(DataRow[] array, int arrayIndex)
    {
      for (int i = 0; i < RowCount; i++)
        array[i + arrayIndex] = new DataRow(this, i);
    }

    int ICollection<DataRow>.Count
    {
      get { return RowCount; }
    }

    bool ICollection<DataRow>.IsReadOnly
    {
      get { return false; }
    }

    bool ICollection<DataRow>.Remove(DataRow item)
    {
      RemoveRow(item.RowIndex);
      return true;
    }

    #endregion ICollection<DataRow> Members

    #endregion IList<DataRow> support

    /// <summary>
    /// Gets the parent column collection of a column.
    /// </summary>
    public static Altaxo.Data.DataColumnCollection? GetParentDataColumnCollectionOf(Altaxo.Data.DataColumn column)
    {
      if (column is null)
        return null;
      else if (column.ParentObject is DataColumnCollection parentColl)
        return parentColl;
      else
        return (DataColumnCollection?)Main.AbsoluteDocumentPath.GetRootNodeImplementing(column, typeof(DataColumnCollection));
    }
  } // end class Altaxo.Data.DataColumnCollection
}
