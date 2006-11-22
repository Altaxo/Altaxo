#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2005 Dr. Dirk Lellinger
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
using Altaxo.Graph.Scales;
using Altaxo.Graph.Scales.Boundaries;

namespace Altaxo.Graph.Plot.Data
{
  using Gdi.Plot.Data;

  /// <summary>
  /// Summary description for XYColumnPlotData.
  /// </summary>
  [SerializationSurrogate(0,typeof(XYColumnPlotData.SerializationSurrogate0))]
  [SerializationVersion(0)]
  public class XYColumnPlotData 
    :
    System.Runtime.Serialization.IDeserializationCallback,
    Main.IChangedEventSource, 
    System.ICloneable,
    Main.IDocumentNode
  {
    /// <summary>
    /// The parent object.
    /// </summary>
    protected object m_Parent;
   
    protected Altaxo.Data.ReadableColumnProxy m_xColumn; // the X-Column
    protected Altaxo.Data.ReadableColumnProxy m_yColumn; // the Y-Column

    /// <summary>This is here only for backward deserialization compatibility. Do not use it.</summary>
    private  Altaxo.Data.IReadableColumn m_LabelColumn; // the label column

    protected int m_PlotRangeStart = 0;
    protected int m_PlotRangeLength  = int.MaxValue;

    // cached or temporary data
    protected IPhysicalBoundaries m_xBoundaries;
    protected IPhysicalBoundaries m_yBoundaries;

    /// <summary>
    /// Number of valid pairs of plot data.
    /// </summary>
    protected int    m_PlottablePoints; // number of plottable points
    /// <summary>
    /// One more that the index to the last valid pair of plot data. 
    /// </summary>
    protected int    m_PointCount;
    protected bool   m_bCachedDataValid=false;
    protected int   _SupressBoundaryChangeEvents;

    // events
    public event BoundaryChangedHandler  XBoundariesChanged;
    public event BoundaryChangedHandler  YBoundariesChanged;


    /// <summary>
    /// Fired if either the data of this XYColumnPlotData changed or if the bounds changed
    /// </summary>
    public event System.EventHandler Changed;


    #region Serialization

    #region Binary
    /// <summary>Used to serialize the XYColumnPlotData Version 0.</summary>
    public class SerializationSurrogate0 : System.Runtime.Serialization.ISerializationSurrogate
    {
      /// <summary>
      /// Serializes XYColumnPlotData Version 0.
      /// </summary>
      /// <param name="obj">The XYColumnPlotData to serialize.</param>
      /// <param name="info">The serialization info.</param>
      /// <param name="context">The streaming context.</param>
      public void GetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context  )
      {
        XYColumnPlotData s = (XYColumnPlotData)obj;
        
        info.AddValue("XColumn",s.m_xColumn);
        info.AddValue("YColumn",s.m_yColumn);

        info.AddValue("XBoundaries",s.m_xBoundaries);
        info.AddValue("YBoundaries",s.m_yBoundaries);

      }
      /// <summary>
      /// Deserializes the XYColumnPlotData Version 0.
      /// </summary>
      /// <param name="obj">The empty XYColumnPlotData object to deserialize into.</param>
      /// <param name="info">The serialization info.</param>
      /// <param name="context">The streaming context.</param>
      /// <param name="selector">The deserialization surrogate selector.</param>
      /// <returns>The deserialized XYColumnPlotData.</returns>
      public object SetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context,System.Runtime.Serialization.ISurrogateSelector selector)
      {
        XYColumnPlotData s = (XYColumnPlotData)obj;

        s.m_xColumn = (ReadableColumnProxy)info.GetValue("XColumn",typeof(ReadableColumnProxy));
        s.m_yColumn = (ReadableColumnProxy)info.GetValue("YColumn",typeof(ReadableColumnProxy));

        s.m_xBoundaries = (IPhysicalBoundaries)info.GetValue("XBoundaries",typeof(IPhysicalBoundaries));
        s.m_yBoundaries = (IPhysicalBoundaries)info.GetValue("YBoundaries",typeof(IPhysicalBoundaries));
  
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
      if (m_xColumn != null)
        m_xColumn.Changed += new EventHandler(EhColumnDataChangedEventHandler);

      if (m_yColumn != null)
        m_yColumn.Changed += new EventHandler(EhColumnDataChangedEventHandler);

      if (null != m_xBoundaries)
        m_xBoundaries.BoundaryChanged += new BoundaryChangedHandler(this.EhXBoundariesChanged);

      if (null != m_yBoundaries)
        m_yBoundaries.BoundaryChanged += new BoundaryChangedHandler(this.EhYBoundariesChanged);
    }
    #endregion

    #region Xml 0 und 1
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.XYColumnPlotData", 0)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.XYColumnPlotData", 1)] // by mistake the data of version 0 and 1 are identical
      public class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        throw new ApplicationException("Calling a deprecated serialization handler for XYColumnPlotData");
        /*
        XYColumnPlotData s = (XYColumnPlotData)obj;
        
        if(s.m_xColumn is Main.IDocumentNode && !s.Equals(((Main.IDocumentNode)s.m_xColumn).ParentObject))
        {
          info.AddValue("XColumn",Main.DocumentPath.GetAbsolutePath((Main.IDocumentNode)s.m_xColumn));
        }
        else
        {
          info.AddValue("XColumn",s.m_xColumn);
        }
        
        
        if(s.m_yColumn is Main.IDocumentNode && !s.Equals(((Main.IDocumentNode)s.m_yColumn).ParentObject))
        {
          info.AddValue("YColumn",Main.DocumentPath.GetAbsolutePath((Main.IDocumentNode)s.m_yColumn));
        }
        else
        {
          info.AddValue("YColumn",s.m_yColumn);
        }

        info.AddValue("XBoundaries",s.m_xBoundaries);
        info.AddValue("YBoundaries",s.m_yBoundaries);
        */
      }


      protected Main.DocumentPath _xColumn = null;
      protected Main.DocumentPath _yColumn = null;
    
      protected XYColumnPlotData _plotAssociation = null;
      public virtual object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        bool bNeedsCallback = false;
        XYColumnPlotData s = null!=o ? (XYColumnPlotData)o : new XYColumnPlotData();

        object xColumn = info.GetValue("XColumn",typeof(Altaxo.Data.IReadableColumn));
        object yColumn = info.GetValue("YColumn",typeof(Altaxo.Data.IReadableColumn));

        if(xColumn is Altaxo.Data.IReadableColumn)
          s.XColumn = (Altaxo.Data.IReadableColumn)xColumn;
        else if (xColumn is Main.DocumentPath)
          bNeedsCallback = true;


        if(yColumn is Altaxo.Data.IReadableColumn)
          s.YColumn = (Altaxo.Data.IReadableColumn)yColumn;
        else if (yColumn is Main.DocumentPath)
          bNeedsCallback = true;



        s.m_xBoundaries = (IPhysicalBoundaries)info.GetValue("XBoundaries",typeof(IPhysicalBoundaries));
        if(null!=s.m_xBoundaries)
          s.m_xBoundaries.BoundaryChanged += new BoundaryChangedHandler(s.EhXBoundariesChanged);

        s.m_yBoundaries = (IPhysicalBoundaries)info.GetValue("YBoundaries",typeof(IPhysicalBoundaries));
        if(null!=s.m_yBoundaries)
          s.m_yBoundaries.BoundaryChanged += new BoundaryChangedHandler(s.EhYBoundariesChanged);



        if(bNeedsCallback)
        {
          XmlSerializationSurrogate0 surr = new XmlSerializationSurrogate0();
          surr._xColumn = xColumn as Main.DocumentPath;
          surr._yColumn = yColumn as Main.DocumentPath;
          surr._plotAssociation = s;

          info.DeserializationFinished += new Altaxo.Serialization.Xml.XmlDeserializationCallbackEventHandler(surr.EhDeserializationFinished);

        }
        return s;
      }


      private void EhDeserializationFinished(Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object documentRoot)
      {
        bool bAllResolved = true;

        if(this._xColumn != null)
        {
          object xColumn = Main.DocumentPath.GetObject(this._xColumn, this._plotAssociation, documentRoot);
          bAllResolved &= (null!=xColumn);
          if(xColumn is Altaxo.Data.IReadableColumn)
            _plotAssociation.XColumn = (Altaxo.Data.IReadableColumn)xColumn;
        
        }

        if(this._yColumn != null)
        {
          object yColumn = Main.DocumentPath.GetObject(this._yColumn, this._plotAssociation, documentRoot);
          bAllResolved &= (null!=yColumn);
          if(yColumn is Altaxo.Data.IReadableColumn)
            _plotAssociation.YColumn = (Altaxo.Data.IReadableColumn)yColumn;
        }

        if(bAllResolved)
          info.DeserializationFinished -= new Altaxo.Serialization.Xml.XmlDeserializationCallbackEventHandler(this.EhDeserializationFinished);
      }
    }
    #endregion

    #region Xml2

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.XYColumnPlotData", 2)]
    public class XmlSerializationSurrogate2 : XmlSerializationSurrogate0
    {
      public override void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        throw new ApplicationException("Calling a deprecated serialization handler for XYColumnPlotData");
        /*
        XYColumnPlotData s = (XYColumnPlotData)obj;
        base.Serialize(obj,info);
        
        // -----------------------Added in version 2 ------------------------

        // the rest of the plot data is stored in kind of a array
        // so it should be easy to add more data here, and only data that are valid
        // are been serialized
        int nElements = s.LabelColumn==null ? 0 : 1;
        info.CreateArray("OptionalData",nElements);
        if(null!=s.LabelColumn)
        {
          if(s.m_LabelColumn is Main.IDocumentNode && !s.Equals(((Main.IDocumentNode)s.m_LabelColumn).ParentObject))
            info.AddValue("LabelColumn",Main.DocumentPath.GetAbsolutePath((Main.IDocumentNode)s.m_LabelColumn));
          else
            info.AddValue("LabelColumn",s.m_LabelColumn);
        }
        info.CommitArray(); // end of array OptionalData
      */
      }

      public override object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        XYColumnPlotData s = null!=o ? (XYColumnPlotData)o : new XYColumnPlotData();
        base.Deserialize(s,info,parent);

        bool bNeedsCallback = false;


        object labelColumn = null;

        int nOptionalData = info.OpenArray();
      {
        if(nOptionalData==1)
        {
          string keystring   = info.GetNodeName();
          labelColumn = info.GetValue(parent);

          if(labelColumn is Altaxo.Data.IReadableColumn)
            s.m_LabelColumn = (Altaxo.Data.IReadableColumn)labelColumn;
          else if (labelColumn is Main.DocumentPath)
            bNeedsCallback = true;
        }
      }
        info.CloseArray(nOptionalData);


        if(bNeedsCallback)
        {
          XmlSerializationSurrogate2 surr = new XmlSerializationSurrogate2();
          surr._labelColumn = labelColumn as Main.DocumentPath;
          surr._plotAssociation = s;

          info.DeserializationFinished += new Altaxo.Serialization.Xml.XmlDeserializationCallbackEventHandler(surr.EhDeserializationFinished2);
        }

        return s;
      }

      Main.DocumentPath _labelColumn = null;
      private void EhDeserializationFinished2(Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object documentRoot)
      {
        bool bAllResolved = true;

        if(this._labelColumn != null)
        {
          object labelColumn = Main.DocumentPath.GetObject(this._labelColumn, this._plotAssociation, documentRoot);
          bAllResolved &= (null!=labelColumn);
          if(labelColumn is Altaxo.Data.IReadableColumn)
            _plotAssociation.m_LabelColumn = (Altaxo.Data.IReadableColumn)labelColumn;
        }

        if(bAllResolved)
          info.DeserializationFinished -= new Altaxo.Serialization.Xml.XmlDeserializationCallbackEventHandler(this.EhDeserializationFinished2);
      }
    }
    #endregion

    #region Xml 3
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.XYColumnPlotData", 3)]
      public class XmlSerializationSurrogate3 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        XYColumnPlotData s = (XYColumnPlotData)obj;

        info.AddValue("XColumn", s.m_xColumn);
        info.AddValue("YColumn", s.m_yColumn);

        info.AddValue("XBoundaries", s.m_xBoundaries);
        info.AddValue("YBoundaries", s.m_yBoundaries);
      }

      public virtual XYColumnPlotData SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        XYColumnPlotData s = null != o ? (XYColumnPlotData)o : new XYColumnPlotData();

        s.m_xColumn = (ReadableColumnProxy)info.GetValue("XColumn", parent);
        s.m_yColumn = (ReadableColumnProxy)info.GetValue("YColumn", parent);

        s.m_xBoundaries = (IPhysicalBoundaries)info.GetValue("XBoundaries", parent);
        s.m_yBoundaries = (IPhysicalBoundaries)info.GetValue("YBoundaries", parent);

        return s;
      }

      public virtual object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        XYColumnPlotData s = SDeserialize(o, info, parent);
        CreateEventChain(s);
        return s;
      }

      public virtual void CreateEventChain(XYColumnPlotData s)
      {
        if (null != s.m_xColumn)
          s.m_xColumn.Changed += new EventHandler(s.EhColumnDataChangedEventHandler);
        if (null != s.m_yColumn)
          s.m_yColumn.Changed += new EventHandler(s.EhColumnDataChangedEventHandler);

        if (null != s.m_xBoundaries)
          s.m_xBoundaries.BoundaryChanged += new BoundaryChangedHandler(s.EhXBoundariesChanged);

        if (null != s.m_yBoundaries)
          s.m_yBoundaries.BoundaryChanged += new BoundaryChangedHandler(s.EhYBoundariesChanged);
      }



    }
    #endregion

    #region Xml 4 und 5
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.XYColumnPlotData",4)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(XYColumnPlotData), 5)]
    public class XmlSerializationSurrogate4 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        XYColumnPlotData s = (XYColumnPlotData)obj;

        info.AddValue("XColumn", s.m_xColumn);
        info.AddValue("YColumn", s.m_yColumn);

        info.AddValue("XBoundaries", s.m_xBoundaries);
        info.AddValue("YBoundaries", s.m_yBoundaries);

        info.AddValue("RangeStart", s.m_PlotRangeStart);
        info.AddValue("RangeLength", s.m_PlotRangeLength);
      }

      public virtual XYColumnPlotData SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        XYColumnPlotData s = null != o ? (XYColumnPlotData)o : new XYColumnPlotData();

        s.m_xColumn = (ReadableColumnProxy)info.GetValue("XColumn", parent);
        s.m_yColumn = (ReadableColumnProxy)info.GetValue("YColumn", parent);

        s.m_xBoundaries = (IPhysicalBoundaries)info.GetValue("XBoundaries", parent);
        s.m_yBoundaries = (IPhysicalBoundaries)info.GetValue("YBoundaries", parent);

        s.m_PlotRangeStart = info.GetInt32("RangeStart");
        s.m_PlotRangeLength = info.GetInt32("RangeLength");

        return s;
      }

      public virtual object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        XYColumnPlotData s = SDeserialize(o, info, parent);
        CreateEventChain(s);
        return s;
      }

      public virtual void CreateEventChain(XYColumnPlotData s)
      {
        if (null != s.m_xColumn)
          s.m_xColumn.Changed += new EventHandler(s.EhColumnDataChangedEventHandler);
        if (null != s.m_yColumn)
          s.m_yColumn.Changed += new EventHandler(s.EhColumnDataChangedEventHandler);

        if (null != s.m_xBoundaries)
          s.m_xBoundaries.BoundaryChanged += new BoundaryChangedHandler(s.EhXBoundariesChanged);

        if (null != s.m_yBoundaries)
          s.m_yBoundaries.BoundaryChanged += new BoundaryChangedHandler(s.EhYBoundariesChanged);
      }



    }
    #endregion
    #endregion



    public XYColumnPlotData(Altaxo.Data.IReadableColumn xColumn, Altaxo.Data.IReadableColumn yColumn)
    {
      XColumn = xColumn;
      YColumn = yColumn;


      //this.SetXBoundsFromTemplate( new FiniteNumericalBoundaries() );
      //this.SetYBoundsFromTemplate( new FiniteNumericalBoundaries() );

    }

    protected XYColumnPlotData()
    {
    }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="from">The object to copy from.</param>
    /// <remarks>Only clones the references to the data columns, not the columns itself.</remarks>
    public XYColumnPlotData(XYColumnPlotData from)
    {
      XColumn = from.XColumn; // also wires event, do not clone the column data here!!!
      YColumn = from.YColumn; // wires event, do not clone the column data here!!!

      this.m_PlotRangeStart = from.m_PlotRangeStart;
      this.m_PlotRangeLength  = from.m_PlotRangeLength;
    }

    /// <summary>
    /// Creates a cloned copy of this object.
    /// </summary>
    /// <returns>The cloned copy of this object.</returns>
    /// <remarks>The data columns refered by this object are <b>not</b> cloned, only the reference is cloned here.</remarks>
    public object Clone()
    {
      return new XYColumnPlotData(this);
    }

    public object ParentObject
    {
      get { return m_Parent; }
      set { m_Parent = value; }
    }

    public string Name
    {
      get
      {
        Main.INamedObjectCollection noc = ParentObject as Main.INamedObjectCollection;
        return noc==null ? null : noc.GetNameOfChildObject(this);
      }
    }


    public string GetXName(int level)
    {
      IReadableColumn col = this.m_xColumn.Document;
      if (col is Altaxo.Data.DataColumn)
      {
        Altaxo.Data.DataTable table = Altaxo.Data.DataTable.GetParentDataTableOf((DataColumn)col);
        string tablename = table == null ? string.Empty : table.Name + "\\";
        string collectionname = table == null ? string.Empty : (table.PropertyColumns.ContainsColumn((DataColumn)col) ? "PropCols\\" : "DataCols\\");
        if (level <= 0)
          return ((DataColumn)col).Name;
        else if (level == 1)
          return tablename + ((DataColumn)col).Name;
        else
          return tablename + collectionname + ((DataColumn)col).Name;
      }
      else if (col != null)
      {
        return col.FullName;
      }
      else
      {
        return m_xColumn.GetName(level) + " (broken)";
      }
    }

    public string GetYName(int level)
    {
      IReadableColumn col = this.m_yColumn.Document;
      if (col is Altaxo.Data.DataColumn)
      {
        Altaxo.Data.DataTable table = Altaxo.Data.DataTable.GetParentDataTableOf((DataColumn)col);
        string tablename = table == null ? string.Empty : table.Name + "\\";
        string collectionname = table == null ? string.Empty : (table.PropertyColumns.ContainsColumn((DataColumn)col) ? "PropCols\\" : "DataCols\\");
        if (level <= 0)
          return ((DataColumn)col).Name;
        else if (level == 1)
          return tablename + ((DataColumn)col).Name;
        else
          return tablename + collectionname + ((DataColumn)col).Name;
      }
      else if (col != null)
      {
        return col.FullName;
      }
      else
      {
        return m_yColumn.GetName(level) + " (broken)";
      }
    }

    public void MergeXBoundsInto(IPhysicalBoundaries pb)
    {
       if(null==m_xBoundaries || pb.GetType() != m_xBoundaries.GetType())
         this.SetXBoundsFromTemplate(pb);

      if (!this.m_bCachedDataValid)
      {
        _SupressBoundaryChangeEvents++;
        this.CalculateCachedData();
        _SupressBoundaryChangeEvents--;
      }
      pb.Add(m_xBoundaries);
    }

    public void MergeYBoundsInto(IPhysicalBoundaries pb)
    {
      if (null == m_yBoundaries || pb.GetType() != m_yBoundaries.GetType())
        this.SetYBoundsFromTemplate(pb);

      if(!this.m_bCachedDataValid)
      {
        _SupressBoundaryChangeEvents++;
        this.CalculateCachedData();
        _SupressBoundaryChangeEvents--;
      }
      pb.Add(m_yBoundaries);
    }

    /// <summary>
    /// This sets the x boundary object to a object of the same type as val. The inner data of the boundary, if present,
    /// are copied into the new x boundary object.
    /// </summary>
    /// <param name="val">The template boundary object.</param>
    protected void SetXBoundsFromTemplate(IPhysicalBoundaries val)
    {
      

      if(null==m_xBoundaries || val.GetType() != m_xBoundaries.GetType())
      {
        if(null!=m_xBoundaries)
        {
          m_xBoundaries.BoundaryChanged -= new BoundaryChangedHandler(this.EhXBoundariesChanged);
        }
        m_xBoundaries = (IPhysicalBoundaries)val.Clone();
        m_xBoundaries.BoundaryChanged += new BoundaryChangedHandler(this.EhXBoundariesChanged);
        m_bCachedDataValid = false;

        OnChanged();
      }
    }

    /// <summary>
    /// This sets the y boundary object to a object of the same type as val. The inner data of the boundary, if present,
    /// are copied into the new y boundary object.
    /// </summary>
    /// <param name="val">The template boundary object.</param>
    protected void SetYBoundsFromTemplate(IPhysicalBoundaries val)
    {
      
      if(null==m_yBoundaries || val.GetType() != m_yBoundaries.GetType())
      {
        if(null!=m_yBoundaries)
        {
          m_yBoundaries.BoundaryChanged -= new BoundaryChangedHandler(this.EhYBoundariesChanged);
        }
        m_yBoundaries = (IPhysicalBoundaries)val.Clone();
        m_yBoundaries.BoundaryChanged += new BoundaryChangedHandler(this.EhYBoundariesChanged);
        m_bCachedDataValid = false;

        OnChanged();
      }
    }


    
    protected virtual void EhXBoundariesChanged(object sender, BoundariesChangedEventArgs e)
    {
      if (_SupressBoundaryChangeEvents == 0)
      {
        if (null != this.XBoundariesChanged)
          XBoundariesChanged(this, e);

        OnChanged();
      }
    }

    protected virtual void EhYBoundariesChanged(object sender, BoundariesChangedEventArgs e)
    {
      if (_SupressBoundaryChangeEvents == 0)
      {
        if (null != this.YBoundariesChanged)
          YBoundariesChanged(this, e);

        OnChanged();
      }
    }


    /// <summary>
    /// Number of valid plot data points.
    /// </summary>
    public int PlottablePoints
    {
      get
      {
        if(!this.m_bCachedDataValid)
          this.CalculateCachedData();
        return this.m_PlottablePoints;
      }
    }

    /// <summary>
    /// One more than the index to the last valid plot data point. This is <b>not</b>
    /// the number of plottable points, <seealso cref="PlottablePoints"/>
    /// </summary>
    /// <remarks>This is not neccessarily (PlotRangeStart+PlotRangeLength), but always less or equal than this. This is because
    /// the underlying arrays can be smaller than the proposed plot range.</remarks>
    public int PlotRangeEnd
    {
      get
      {
        if(!this.m_bCachedDataValid)
          this.CalculateCachedData();
        return this.m_PointCount;
      }
    }

    public Altaxo.Data.IReadableColumn XColumn
    {
      get
      {
        return m_xColumn == null ? null : m_xColumn.Document;
      }
      set
      {
        if (null == m_xColumn)
        {
          m_xColumn = new ReadableColumnProxy(value);
          m_xColumn.Changed += new EventHandler(this.EhColumnDataChangedEventHandler);
        }
        else
        {
          m_xColumn.SetDocNode(value);
        }

        m_bCachedDataValid = false;
        OnChanged();
      }
    }

    public Altaxo.Data.IReadableColumn YColumn
    {
      get
      {
        return m_yColumn == null ? null : m_yColumn.Document;
      }
      set
      {
        if (null == m_yColumn)
        {
          m_yColumn = new ReadableColumnProxy(value);
          m_yColumn.Changed += new EventHandler(this.EhColumnDataChangedEventHandler);
        }
        else
        {
          m_yColumn.SetDocNode(value);
        }

        m_bCachedDataValid = false;
        
        OnChanged();
      }
    }

    /// <summary>
    /// For compatibility with older deserialization versions. Do not use it!
    /// </summary>
    public Altaxo.Data.IReadableColumn LabelColumn
    {
      get
      {
        return m_LabelColumn;
      }
    }

    public override string ToString()
    {
      return String.Format("{0}(X), {1}(Y)",m_xColumn.ToString(),m_yColumn.ToString());
    }

    public void CalculateCachedData(IPhysicalBoundaries xBounds, IPhysicalBoundaries yBounds)
    {
      if (m_xBoundaries == null || m_xBoundaries.GetType() != xBounds.GetType())
        this.SetXBoundsFromTemplate(xBounds);

      if (m_yBoundaries == null || m_yBoundaries.GetType() != yBounds.GetType())
        this.SetYBoundsFromTemplate(yBounds);

      CalculateCachedData();
    }

    public void CalculateCachedData()
    {
      // we can calulate the bounds only if they are set before
      if(null==m_xBoundaries || null==m_yBoundaries)
        return;


      m_PlottablePoints = 0;

      
      this.m_xBoundaries.BeginUpdate(); // disable events
      this.m_yBoundaries.BeginUpdate(); // disable events
      
      this.m_xBoundaries.Reset();
      this.m_yBoundaries.Reset();

      System.Diagnostics.Debug.Assert(m_PlotRangeStart>=0);
      System.Diagnostics.Debug.Assert(m_PlotRangeLength>=0);


      m_PointCount = m_PlotRangeLength == int.MaxValue ? int.MaxValue : m_PlotRangeStart + m_PlotRangeLength;

      IReadableColumn xColumn = this.XColumn;
      IReadableColumn yColumn = this.YColumn;

      if (xColumn == null || yColumn == null)
      {
        m_PointCount=0;
        m_PlottablePoints = 0;
      }
      else
      {

        if (xColumn is IDefinedCount)
          m_PointCount = System.Math.Min(m_PointCount, ((IDefinedCount)xColumn).Count);
        if (yColumn is IDefinedCount)
          m_PointCount = System.Math.Min(m_PointCount, ((IDefinedCount)yColumn).Count);

        // if both columns are indefinite long, we set the length to zero
        if (m_PointCount == int.MaxValue || m_PointCount < 0)
          m_PointCount = 0;


        for (int i = m_PlotRangeStart; i < m_PointCount; i++)
        {
          if (!xColumn.IsElementEmpty(i) && !yColumn.IsElementEmpty(i))
          {
            bool x_added = this.m_xBoundaries.Add(xColumn, i);
            bool y_added = this.m_yBoundaries.Add(yColumn, i);
            if (x_added && y_added)
              m_PlottablePoints++;
          }
        }
      }

      // now the cached data are valid
      m_bCachedDataValid = true;


      // now when the cached data are valid, we can reenable the events
      this.m_xBoundaries.EndUpdate(); // enable events
      this.m_yBoundaries.EndUpdate(); // enable events
    }

    void EhColumnDataChangedEventHandler(object sender, EventArgs e)
    {
      // !!!todo!!! : special case if only data added to a column should
      // be handeld separately to save computing time
      this.m_bCachedDataValid = false;
    
      OnChanged();
    }

    protected virtual void OnChanged()
    {
      if(null!=Changed)
        Changed(this,new System.EventArgs());
    }

    /// <summary>
    /// Number of the first point to plot.
    /// </summary>
    public int PlotRangeStart
    {
      get { return this.m_PlotRangeStart; }
      set
      {
        m_PlotRangeStart = value<0 ? 0 : value;
      }
    }

    /// <summary>
    /// Length of the plot range. The last point of the plot range is PlotRangeStart+PlotRangeLength-1.
    /// This is not the number of plottable points!
    /// </summary>
    public int PlotRangeLength
    {
      get { return this.m_PlotRangeLength; }
      set { this.m_PlotRangeLength = value<0 ? 0 : value; }
    }

    class MyPlotData : Processed2DPlotData
    {
      public IReadableColumn _xColumn;
      public IReadableColumn _yColumm;

      public override AltaxoVariant GetXPhysical(int originalRowIndex)
      {
        return _xColumn[originalRowIndex];
      }
      public override AltaxoVariant GetYPhysical(int originalRowIndex)
      {
        return _yColumm[originalRowIndex];
      }
    }
    
    /// <summary>
    /// This will create a point list out of the data, which can be used to plot the data. In order to create this list,
    /// the function must have knowledge how to calculate the points out of the data. This will be done
    /// by a function provided by the calling function.
    /// </summary>
    /// <param name="layer">The plot layer.</param>
    /// <param name="rangeList">On return, this gives the list of plot ranges.</param>
    /// <param name="ptArray">On return, this is an array of plot points in layer coordinates.</param>
    /// <returns>True if the function is successfull, otherwise false.</returns>
    public Processed2DPlotData GetRangesAndPoints(
      Gdi.IPlotArea layer)
    {
      const double MaxRelativeValue = 1E2;
      MyPlotData result = new MyPlotData();

      PlotRangeList rangeList=null;
      PointF[] ptArray=null;
      


      Altaxo.Data.IReadableColumn xColumn = this.XColumn;
      Altaxo.Data.IReadableColumn yColumn = this.YColumn;

      if(null==xColumn || null==yColumn)
        return null; // this plotitem is only for x and y double columns

      if(this.PlottablePoints<=0)
        return null;

      // allocate an array PointF to hold the line points
      ptArray = new PointF[this.PlottablePoints];
      result.PlotPointsInAbsoluteLayerCoordinates = ptArray;

      // allocate  the physical points
      result._xColumn = this.XColumn;
      result._yColumm = this.YColumn;

      // Fill the array with values
      // only the points where x and y are not NaNs are plotted!

      int i,j;

      bool bInPlotSpace = true;
      int  rangeStart=0;
      int  rangeOffset=0;
      rangeList = new PlotRangeList();
      result.RangeList = rangeList;


      Scale xAxis = layer.XAxis;
      Scale yAxis = layer.YAxis;
      Gdi.G2DCoordinateSystem coordsys = layer.CoordinateSystem;


      int len = this.PlotRangeEnd;
      for(i=this.PlotRangeStart,j=0;i<len;i++)
      {
        if(xColumn.IsElementEmpty(i) || yColumn.IsElementEmpty(i))
        {
          if(!bInPlotSpace)
          {
            bInPlotSpace=true;
            rangeList.Add(new PlotRange(rangeStart,j,rangeOffset));
          }
          continue;
        }
          

        double x_rel,y_rel;
        double xcoord, ycoord;

        

        x_rel = xAxis.PhysicalVariantToNormal(xColumn[i]);
        y_rel = yAxis.PhysicalVariantToNormal(yColumn[i]);

        // chop relative values to an range of about -+ 10^6
        if(x_rel>MaxRelativeValue)
          x_rel = MaxRelativeValue;
        if(x_rel<-MaxRelativeValue)
          x_rel=-MaxRelativeValue;
        if(y_rel>MaxRelativeValue)
          y_rel = MaxRelativeValue;
        if(y_rel<-MaxRelativeValue)
          y_rel=-MaxRelativeValue;

          
        // after the conversion to relative coordinates it is possible
        // that with the choosen axis the point is undefined 
        // (for instance negative values on a logarithmic axis)
        // in this case the returned value is NaN
        if(coordsys.LogicalToLayerCoordinates(new Logical3D(x_rel,y_rel), out xcoord, out ycoord))
        {
          if(bInPlotSpace)
          {
            bInPlotSpace=false;
            rangeStart = j;
            rangeOffset = i-j;
          }
          ptArray[j].X = (float)xcoord;
          ptArray[j].Y = (float)ycoord;
          
          j++;
        }
        else
        {
          if(!bInPlotSpace)
          {
            bInPlotSpace=true;
            rangeList.Add(new PlotRange(rangeStart,j,rangeOffset));
          }
        }
      } // end for
      if(!bInPlotSpace)
      {
        bInPlotSpace=true;
        rangeList.Add(new PlotRange(rangeStart,j,rangeOffset)); // add the last range
      }
      return result;
    }
  }
}
