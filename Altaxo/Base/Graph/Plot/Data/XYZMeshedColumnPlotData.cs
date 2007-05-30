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
using System.Drawing;
using Altaxo.Serialization;
using Altaxo.Data;
using Altaxo.Collections;
using Altaxo.Graph.Scales.Boundaries;

namespace Altaxo.Graph.Plot.Data
{
  /// <summary>
  /// Summary description for XYColumnPlotData.
  /// </summary>
  [Serializable]
  public class XYZMeshedColumnPlotData
    :
    Main.IChangedEventSource,
    System.ICloneable,
    Main.IDocumentNode,
    System.Runtime.Serialization.IDeserializationCallback
  {
    [NonSerialized]
    protected object _parent;

    protected Altaxo.Data.ReadableColumnProxy[] _dataColumns; // the columns that are involved in the picture


    protected Altaxo.Data.ReadableColumnProxy _xColumn;
    protected Altaxo.Data.ReadableColumnProxy _yColumn;

    // cached or temporary data
    protected NumericalBoundaries _xBoundaries;
    protected NumericalBoundaries _yBoundaries;
    protected NumericalBoundaries _vBoundaries;


    /// <summary>
    /// Number of rows, here the maximum of the row counts of all columns.
    /// </summary>
    protected int _numberOfRows;
    [NonSerialized]
    protected bool _isCachedDataValid = false;

    // events
    [field: NonSerialized]
    public event BoundaryChangedHandler XBoundariesChanged;
    [field: NonSerialized]
    public event BoundaryChangedHandler YBoundariesChanged;
    [field: NonSerialized]
    public event BoundaryChangedHandler VBoundariesChanged;


    /// <summary>
    /// Fired if either the data of this XYColumnPlotData changed or if the bounds changed
    /// </summary>
    [field: NonSerialized]
    public event System.EventHandler Changed;


    #region Serialization

    #region Clipboard
  
    public void OnDeserialization(object sender)
    {
      CreateEventChain();
    }

     #endregion
  
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.XYZEquidistantMeshColumnPlotData", 0)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(XYZMeshedColumnPlotData), 1)]
    class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        throw new ApplicationException("Calling a deprecated serialization handler for XYZMeshedColumnPlotData");
        /*
        XYZMeshedColumnPlotData s = (XYZMeshedColumnPlotData)obj;
    
        if(s.m_XColumn is Main.IDocumentNode && !s.Equals(((Main.IDocumentNode)s.m_XColumn).ParentObject))
        {
          info.AddValue("XColumn",Main.DocumentPath.GetAbsolutePath((Main.IDocumentNode)s.m_XColumn));
        }
        else
        {
          info.AddValue("XColumn",s.m_XColumn);
        }

        if(s.m_YColumn is Main.IDocumentNode && !s.Equals(((Main.IDocumentNode)s.m_YColumn).ParentObject))
        {
          info.AddValue("YColumn",Main.DocumentPath.GetAbsolutePath((Main.IDocumentNode)s.m_YColumn));
        }
        else
        {
          info.AddValue("YColumn",s.m_YColumn);
        }

        info.CreateArray("DataColumns",s.m_DataColumns.Length);
        for(int i=0;i<s.m_DataColumns.Length;i++)
        {
          Altaxo.Data.IReadableColumn col = s.m_DataColumns[i];
          if(col is Main.IDocumentNode && !s.Equals(((Main.IDocumentNode)col).ParentObject))
          {
            info.AddValue("e",Main.DocumentPath.GetAbsolutePath((Main.IDocumentNode)col));
          }
          else
          {
            info.AddValue("e",col);
          }
        }
        info.CommitArray();

        info.AddValue("XBoundaries",s.m_xBoundaries);
        info.AddValue("YBoundaries",s.m_yBoundaries);
        info.AddValue("VBoundaries",s.m_vBoundaries);
        */
      }

      Main.DocumentPath _xColumn = null;
      Main.DocumentPath _yColumn = null;
      Main.DocumentPath[] _vColumns = null;
      XYZMeshedColumnPlotData _plotAssociation = null;

      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        bool bSurrogateUsed = false;

        XYZMeshedColumnPlotData s = null != o ? (XYZMeshedColumnPlotData)o : new XYZMeshedColumnPlotData();

        XmlSerializationSurrogate0 surr = new XmlSerializationSurrogate0();

        object deserobj;
        deserobj = info.GetValue("XColumn", s);
        if (deserobj is Main.DocumentPath)
        {
          surr._xColumn = (Main.DocumentPath)deserobj;
          bSurrogateUsed = true;
        }
        else
        {
          s._xColumn = new ReadableColumnProxy((Altaxo.Data.INumericColumn)deserobj);
          s._xColumn.Changed += new EventHandler(s.EhColumnDataChangedEventHandler);
        }


        deserobj = info.GetValue("YColumn", s);
        if (deserobj is Main.DocumentPath)
        {
          surr._yColumn = (Main.DocumentPath)deserobj;
          bSurrogateUsed = true;
        }
        else
        {
          s._yColumn = new ReadableColumnProxy((Altaxo.Data.INumericColumn)deserobj);
          s._yColumn.Changed += new EventHandler(s.EhColumnDataChangedEventHandler);
        }

        int count = info.OpenArray();
        surr._vColumns = new Main.DocumentPath[count];
        s._dataColumns = new ReadableColumnProxy[count];
        for (int i = 0; i < count; i++)
        {
          deserobj = info.GetValue("e", s);
          if (deserobj is Main.DocumentPath)
          {
            surr._vColumns[i] = (Main.DocumentPath)deserobj;
            bSurrogateUsed = true;
          }
          else
          {
            s._dataColumns[i] = new ReadableColumnProxy((Altaxo.Data.IReadableColumn)deserobj);
            s._dataColumns[i].Changed += new EventHandler(s.EhColumnDataChangedEventHandler);
          }
        }
        info.CloseArray(count);


        s._xBoundaries = (NumericalBoundaries)info.GetValue("XBoundaries", typeof(NumericalBoundaries));
        s._yBoundaries = (NumericalBoundaries)info.GetValue("YBoundaries", typeof(NumericalBoundaries));
        s._vBoundaries = (NumericalBoundaries)info.GetValue("VBoundaries", typeof(NumericalBoundaries));

        s._xBoundaries.BoundaryChanged += new BoundaryChangedHandler(s.OnXBoundariesChangedEventHandler);
        s._yBoundaries.BoundaryChanged += new BoundaryChangedHandler(s.OnYBoundariesChangedEventHandler);
        s._vBoundaries.BoundaryChanged += new BoundaryChangedHandler(s.OnVBoundariesChangedEventHandler);


        if (bSurrogateUsed)
        {
          surr._plotAssociation = s;
          info.DeserializationFinished += new Altaxo.Serialization.Xml.XmlDeserializationCallbackEventHandler(surr.EhDeserializationFinished);
        }

        return s;
      }

      public void EhDeserializationFinished(Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object documentRoot)
      {
        bool bAllResolved = true;

        if (this._xColumn != null)
        {
          object xColumn = Main.DocumentPath.GetObject(this._xColumn, this._plotAssociation, documentRoot);
          bAllResolved &= (null != xColumn);
          if (xColumn is Altaxo.Data.INumericColumn)
          {
            this._xColumn = null;
            _plotAssociation._xColumn = new ReadableColumnProxy((Altaxo.Data.INumericColumn)xColumn);
            _plotAssociation._xColumn.Changed += new EventHandler(_plotAssociation.EhColumnDataChangedEventHandler);
          }
        }

        if (this._yColumn != null)
        {
          object yColumn = Main.DocumentPath.GetObject(this._yColumn, this._plotAssociation, documentRoot);
          bAllResolved &= (null != yColumn);
          if (yColumn is Altaxo.Data.INumericColumn)
          {
            this._yColumn = null;
            _plotAssociation._yColumn = new ReadableColumnProxy((Altaxo.Data.INumericColumn)yColumn);
            _plotAssociation._yColumn.Changed += new EventHandler(_plotAssociation.EhColumnDataChangedEventHandler);
          }
        }

        for (int i = 0; i < this._vColumns.Length; i++)
        {
          if (this._vColumns[i] != null)
          {
            object vColumn = Main.DocumentPath.GetObject(this._vColumns[i], this._plotAssociation, documentRoot);
            bAllResolved &= (null != vColumn);
            if (vColumn is Altaxo.Data.IReadableColumn)
            {
              this._vColumns[i] = null;
              _plotAssociation._dataColumns[i] = new ReadableColumnProxy((Altaxo.Data.IReadableColumn)vColumn);
              _plotAssociation._dataColumns[i].Changed += new EventHandler(_plotAssociation.EhColumnDataChangedEventHandler);
            }
          }
        }

        if (bAllResolved)
          info.DeserializationFinished -= new Altaxo.Serialization.Xml.XmlDeserializationCallbackEventHandler(this.EhDeserializationFinished);
      }
    }


    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(XYZMeshedColumnPlotData), 2)]
    class XmlSerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        XYZMeshedColumnPlotData s = (XYZMeshedColumnPlotData)obj;

        info.AddValue("XColumn", s._xColumn);
        info.AddValue("YColumn", s._yColumn);

        info.CreateArray("DataColumns", s._dataColumns.Length);
        for (int i = 0; i < s._dataColumns.Length; i++)
        {
          info.AddValue("e", s._dataColumns[i]);
        }
        info.CommitArray();

        info.AddValue("XBoundaries", s._xBoundaries);
        info.AddValue("YBoundaries", s._yBoundaries);
        info.AddValue("VBoundaries", s._vBoundaries);
      }



      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        bool bSurrogateUsed = false;

        XYZMeshedColumnPlotData s = null != o ? (XYZMeshedColumnPlotData)o : new XYZMeshedColumnPlotData();

        s._xColumn = (ReadableColumnProxy)info.GetValue("XColumn", parent);
        s._yColumn = (ReadableColumnProxy)info.GetValue("YColumn", parent);

        int count = info.OpenArray();
        s._dataColumns = new ReadableColumnProxy[count];
        for (int i = 0; i < count; i++)
        {
          s._dataColumns[i] = (ReadableColumnProxy)info.GetValue("e", parent);
        }
        info.CloseArray(count);


        s._xBoundaries = (NumericalBoundaries)info.GetValue("XBoundaries", typeof(NumericalBoundaries));
        s._yBoundaries = (NumericalBoundaries)info.GetValue("YBoundaries", typeof(NumericalBoundaries));
        s._vBoundaries = (NumericalBoundaries)info.GetValue("VBoundaries", typeof(NumericalBoundaries));

        s._xBoundaries.BoundaryChanged += new BoundaryChangedHandler(s.OnXBoundariesChangedEventHandler);
        s._yBoundaries.BoundaryChanged += new BoundaryChangedHandler(s.OnYBoundariesChangedEventHandler);
        s._vBoundaries.BoundaryChanged += new BoundaryChangedHandler(s.OnVBoundariesChangedEventHandler);

        s.CreateEventChain();

        return s;
      }
    }



    /// <summary>
    /// Finale measures after deserialization.
    /// </summary>
    /// <param name="obj">Not used.</param>
    public void CreateEventChain()
    {
      // restore the event chain

      _xColumn.Changed += new EventHandler(EhColumnDataChangedEventHandler);

      _yColumn.Changed += new EventHandler(EhColumnDataChangedEventHandler);

      for (int i = 0; i < _dataColumns.Length; i++)
      {
        _dataColumns[i].Changed += new EventHandler(EhColumnDataChangedEventHandler);
      }

      _xBoundaries.BoundaryChanged += new BoundaryChangedHandler(this.OnXBoundariesChangedEventHandler);
      _yBoundaries.BoundaryChanged += new BoundaryChangedHandler(this.OnYBoundariesChangedEventHandler);
      _vBoundaries.BoundaryChanged += new BoundaryChangedHandler(this.OnVBoundariesChangedEventHandler);


      // do not calculate cached data here, since it is done the first time this data is really needed
      this._isCachedDataValid = false;
    }
    #endregion

    /// <summary>
    /// Deserialization constructor.
    /// </summary>
    protected XYZMeshedColumnPlotData()
    {
    }

    public XYZMeshedColumnPlotData(Altaxo.Data.INumericColumn xCol, Altaxo.Data.INumericColumn yCol, Altaxo.Data.DataColumnCollection coll, IAscendingIntegerCollection selected)
    {
      _xColumn = new ReadableColumnProxy(xCol);
      _xColumn.Changed += new EventHandler(EhColumnDataChangedEventHandler);

      _yColumn = new ReadableColumnProxy(yCol);
      _yColumn.Changed += new EventHandler(EhColumnDataChangedEventHandler);

      int len = selected == null ? coll.ColumnCount : selected.Count;
      _dataColumns = new ReadableColumnProxy[len];
      for (int i = 0; i < len; i++)
      {
        int idx = null == selected ? i : selected[i];
        _dataColumns[i] = new ReadableColumnProxy(coll[idx]);

        // set the event chain
        _dataColumns[i].Changed += new EventHandler(EhColumnDataChangedEventHandler);
      }


      this.SetXBoundsFromTemplate(new FiniteNumericalBoundaries());
      this.SetYBoundsFromTemplate(new FiniteNumericalBoundaries());
      this.SetVBoundsFromTemplate(new FiniteNumericalBoundaries());

    }


    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="from">The object to copy from.</param>
    /// <remarks>Only clones the references to the data columns, not the columns itself.</remarks>
    public XYZMeshedColumnPlotData(XYZMeshedColumnPlotData from)
    {
      _xColumn = (ReadableColumnProxy)from._xColumn.Clone();
      _xColumn.Changed += new EventHandler(EhColumnDataChangedEventHandler);

      _yColumn = (ReadableColumnProxy)from._yColumn.Clone();
      _yColumn.Changed += new EventHandler(EhColumnDataChangedEventHandler);

      int len = from._dataColumns.Length;
      _dataColumns = new ReadableColumnProxy[len];

      for (int i = 0; i < len; i++)
      {
        _dataColumns[i] = (ReadableColumnProxy)from._dataColumns[i].Clone(); // do not clone the data columns!
        _dataColumns[i].Changed += new EventHandler(EhColumnDataChangedEventHandler);
      }


      this.SetXBoundsFromTemplate(new FiniteNumericalBoundaries());
      this.SetYBoundsFromTemplate(new FiniteNumericalBoundaries());
      this.SetVBoundsFromTemplate(new FiniteNumericalBoundaries());


    }

    /// <summary>
    /// Creates a cloned copy of this object.
    /// </summary>
    /// <returns>The cloned copy of this object.</returns>
    /// <remarks>The data columns refered by this object are <b>not</b> cloned, only the reference is cloned here.</remarks>
    public object Clone()
    {
      return new XYZMeshedColumnPlotData(this);
    }

    public void MergeXBoundsInto(IPhysicalBoundaries pb)
    {
      if (!this._isCachedDataValid)
        this.CalculateCachedData();
      pb.Add(_xBoundaries);
    }

    public void MergeYBoundsInto(IPhysicalBoundaries pb)
    {
      if (!this._isCachedDataValid)
        this.CalculateCachedData();
      pb.Add(_yBoundaries);
    }

    public void MergeVBoundsInto(IPhysicalBoundaries pb)
    {
      if (!this._isCachedDataValid)
        this.CalculateCachedData();
      pb.Add(_vBoundaries);
    }

    public void SetXBoundsFromTemplate(IPhysicalBoundaries val)
    {
      if (null == _xBoundaries || val.GetType() != _xBoundaries.GetType())
      {
        if (null != _xBoundaries)
        {
          _xBoundaries.BoundaryChanged -= new BoundaryChangedHandler(this.OnXBoundariesChangedEventHandler);
        }
        _xBoundaries = (NumericalBoundaries)val.Clone();
        _xBoundaries.BoundaryChanged += new BoundaryChangedHandler(this.OnXBoundariesChangedEventHandler);
        this._isCachedDataValid = false;

        OnChanged();
      }
    }


    public void SetYBoundsFromTemplate(IPhysicalBoundaries val)
    {
      if (null == _yBoundaries || val.GetType() != _yBoundaries.GetType())
      {
        if (null != _yBoundaries)
        {
          _yBoundaries.BoundaryChanged -= new BoundaryChangedHandler(this.OnYBoundariesChangedEventHandler);
        }
        _yBoundaries = (NumericalBoundaries)val.Clone();
        _yBoundaries.BoundaryChanged += new BoundaryChangedHandler(this.OnYBoundariesChangedEventHandler);
        this._isCachedDataValid = false;

        OnChanged();
      }
    }


    public void SetVBoundsFromTemplate(IPhysicalBoundaries val)
    {
      if (null == _vBoundaries || val.GetType() != _vBoundaries.GetType())
      {
        if (null != _vBoundaries)
        {
          _vBoundaries.BoundaryChanged -= new BoundaryChangedHandler(this.OnVBoundariesChangedEventHandler);
        }
        _vBoundaries = (NumericalBoundaries)val.Clone();
        _vBoundaries.BoundaryChanged += new BoundaryChangedHandler(this.OnVBoundariesChangedEventHandler);
        this._isCachedDataValid = false;

        OnChanged();
      }
    }


    protected virtual void OnXBoundariesChangedEventHandler(object sender, BoundariesChangedEventArgs e)
    {
      if (null != this.XBoundariesChanged)
        XBoundariesChanged(this, e);

      OnChanged();
    }

    protected virtual void OnYBoundariesChangedEventHandler(object sender, BoundariesChangedEventArgs e)
    {
      if (null != this.YBoundariesChanged)
        YBoundariesChanged(this, e);

      OnChanged();
    }

    protected virtual void OnVBoundariesChangedEventHandler(object sender, BoundariesChangedEventArgs e)
    {
      if (null != this.VBoundariesChanged)
        VBoundariesChanged(this, e);

      OnChanged();
    }

    public int RowCount
    {
      get
      {
        if (!this._isCachedDataValid)
          this.CalculateCachedData();
        return _numberOfRows;
      }
    }

    public int ColumnCount
    {
      get
      {
        return null == this._dataColumns ? 0 : _dataColumns.Length;
      }
    }



    public ReadableColumnProxy[] DataColumns
    {
      get
      {
        return _dataColumns;
      }
    }

    public Altaxo.Data.IReadableColumn GetDataColumn(int i)
    {
      return _dataColumns[i] == null ? null : _dataColumns[i].Document;
    }

    public Altaxo.Data.IReadableColumn XColumn
    {
      get { return _xColumn == null ? null : _xColumn.Document; }
    }

    public Altaxo.Data.IReadableColumn YColumn
    {
      get { return _yColumn == null ? null : _yColumn.Document; ; }
    }


    public override string ToString()
    {
      if (null != _dataColumns && _dataColumns.Length > 0)
        return String.Format("PictureData {0}-{1}", _dataColumns[0].GetName(2), _dataColumns[_dataColumns.Length - 1].GetName(2));
      else
        return "Empty (no data)";
    }

    public void CalculateCachedData()
    {
      if (null == _dataColumns || _dataColumns.Length == 0)
      {
        _numberOfRows = 0;
        return;
      }

      this._xBoundaries.BeginUpdate(); // disable events
      this._yBoundaries.BeginUpdate(); // disable events
      this._vBoundaries.BeginUpdate();

      this._xBoundaries.Reset();
      this._yBoundaries.Reset();
      this._vBoundaries.Reset();

      // get the length of the largest column as row count
      _numberOfRows = 0;
      for (int i = 0; i < _dataColumns.Length; i++)
      {
        IDefinedCount coli = _dataColumns[i].Document as IDefinedCount;
        if (null != coli)
          _numberOfRows = System.Math.Max(_numberOfRows, coli.Count);
      }

      for (int i = 0; i < _dataColumns.Length; i++)
      {
        Altaxo.Data.IReadableColumn col = _dataColumns[i].Document;
        int collength = (col is Altaxo.Data.IDefinedCount) ? ((Altaxo.Data.IDefinedCount)col).Count : _numberOfRows;
        for (int j = 0; j < collength; j++)
        {
          if (col != null)
            this._vBoundaries.Add(col, j);
        }
      }


      // enter the two bounds for x
      for (int i = 0; i < _dataColumns.Length; i++)
        this._yBoundaries.Add(_yColumn.Document, i);

      // enter the bounds for y
      for (int i = 0; i < _numberOfRows; i++)
        this._xBoundaries.Add(_xColumn.Document, i);

      // now the cached data are valid
      _isCachedDataValid = true;


      // now when the cached data are valid, we can reenable the events
      this._xBoundaries.EndUpdate(); // enable events
      this._yBoundaries.EndUpdate(); // enable events
      this._vBoundaries.EndUpdate(); // enable events

    }

    void EhColumnDataChangedEventHandler(object sender, EventArgs e)
    {

      _isCachedDataValid = false;

      OnChanged();
    }

    protected virtual void OnChanged()
    {
      if (_parent is Main.IChildChangedEventSink)
        ((Main.IChildChangedEventSink)_parent).EhChildChanged(this, EventArgs.Empty);

      if (null != Changed)
        Changed(this, EventArgs.Empty);
    }

    public virtual object ParentObject
    {
      get { return _parent; }
      set { _parent = value; }
    }

    public virtual string Name
    {
      get
      {
        Main.INamedObjectCollection noc = ParentObject as Main.INamedObjectCollection;
        return null == noc ? null : noc.GetNameOfChildObject(this);
      }
    }

   
  }


} // end name space
