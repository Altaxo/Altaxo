#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2004 Dr. Dirk Lellinger
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
using System.Drawing.Drawing2D;
using Altaxo.Serialization;
using Altaxo.Data;
using Altaxo.Graph.Axes;


namespace Altaxo.Graph
{

  /// <summary>
  /// Implemented by objects that hold x bounds, for instance XYPlotAssociations.
  /// </summary>
  public interface IXBoundsHolder
  {
    /// <summary>Fired if the x boundaries of the object changed.</summary>
    event PhysicalBoundaries.BoundaryChangedHandler XBoundariesChanged;

    /// <summary>
    /// This sets the x boundary object to a object of the same type as val. The inner data of the boundary, if present,
    /// are copied into the new x boundary object.
    /// </summary>
    /// <param name="val">The template boundary object.</param>
    void SetXBoundsFromTemplate(PhysicalBoundaries val);

    /// <summary>
    /// This merges the x boundary of the object with the boundary pb. The boundary pb is updated so that
    /// it now includes the x boundary range of the object.
    /// </summary>
    /// <param name="pb">The boundary object pb which is updated to include the x boundaries of the object.</param>
    void MergeXBoundsInto(PhysicalBoundaries pb);
  }

  /// <summary>
  /// Implemented by objects that hold y bounds, for instance XYPlotAssociations.
  /// </summary>
  public interface IYBoundsHolder
  {
    /// <summary>Fired if the y boundaries of the object changed.</summary>
    event PhysicalBoundaries.BoundaryChangedHandler YBoundariesChanged;

    /// <summary>
    /// This sets the y boundary object to a object of the same type as val. The inner data of the boundary, if present,
    /// are copied into the new y boundary object.
    /// </summary>
    /// <param name="val">The template boundary object.</param>
    void SetYBoundsFromTemplate(PhysicalBoundaries val);

    /// <summary>
    /// This merges the y boundary of the object with the boundary pb. The boundary pb is updated so that
    /// it now includes the y boundary range of the object.
    /// </summary>
    /// <param name="pb">The boundary object pb which is updated to include the y boundaries of the object.</param>
    void MergeYBoundsInto(PhysicalBoundaries pb);
  }


  /// <summary>
  /// Implemented by objects that hold x bounds and y bounds, for instance XYPlotAssociations.
  /// </summary>
  public interface IXYBoundsHolder : IXBoundsHolder, IYBoundsHolder
  {
  }


  /// <summary>
  /// Summary description for XYColumnPlotData.
  /// </summary>
  [SerializationSurrogate(0,typeof(XYColumnPlotData.SerializationSurrogate0))]
  [SerializationVersion(0)]
  public class XYColumnPlotData 
    :
    IXYBoundsHolder, 
    System.Runtime.Serialization.IDeserializationCallback,
    Main.IChangedEventSource, 
    System.ICloneable,
    Main.IDocumentNode
  {
    /// <summary>
    /// The parent object.
    /// </summary>
    protected object m_Parent;

    protected Altaxo.Data.IReadableColumn m_xColumn; // the X-Column
    protected Altaxo.Data.IReadableColumn m_yColumn; // the Y-Column
    protected Altaxo.Data.IReadableColumn m_LabelColumn; // the label column

    protected int m_PlotRangeStart = 0;
    protected int m_PlotRangeLength  = int.MaxValue;

    // cached or temporary data
    protected PhysicalBoundaries m_xBoundaries;
    protected PhysicalBoundaries m_yBoundaries;

    /// <summary>
    /// Number of valid pairs of plot data.
    /// </summary>
    protected int    m_PlottablePoints; // number of plottable points
    /// <summary>
    /// One more that the index to the last valid pair of plot data. 
    /// </summary>
    protected int    m_PointCount;
    protected bool   m_bCachedDataValid=false;

    // events
    public event PhysicalBoundaries.BoundaryChangedHandler  XBoundariesChanged;
    public event PhysicalBoundaries.BoundaryChangedHandler  YBoundariesChanged;


    /// <summary>
    /// Fired if either the data of this XYColumnPlotData changed or if the bounds changed
    /// </summary>
    public event System.EventHandler Changed;


    #region Serialization
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

        s.m_xColumn = (Altaxo.Data.IReadableColumn)info.GetValue("XColumn",typeof(Altaxo.Data.IReadableColumn));
        s.m_yColumn = (Altaxo.Data.IReadableColumn)info.GetValue("YColumn",typeof(Altaxo.Data.IReadableColumn));

        s.m_xBoundaries = (PhysicalBoundaries)info.GetValue("XBoundaries",typeof(PhysicalBoundaries));
        s.m_yBoundaries = (PhysicalBoundaries)info.GetValue("YBoundaries",typeof(PhysicalBoundaries));
  
        return s;
      }
    }

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(XYColumnPlotData),0)]
      [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(XYColumnPlotData),1)] // by mistake the data of version 0 and 1 are identical
      public class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
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



        s.m_xBoundaries = (PhysicalBoundaries)info.GetValue("XBoundaries",typeof(PhysicalBoundaries));
        if(null!=s.m_xBoundaries)
          s.m_xBoundaries.BoundaryChanged += new PhysicalBoundaries.BoundaryChangedHandler(s.OnXBoundariesChangedEventHandler);

        s.m_yBoundaries = (PhysicalBoundaries)info.GetValue("YBoundaries",typeof(PhysicalBoundaries));
        if(null!=s.m_yBoundaries)
          s.m_yBoundaries.BoundaryChanged += new PhysicalBoundaries.BoundaryChangedHandler(s.OnYBoundariesChangedEventHandler);



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

  

    /// <summary>
    /// Finale measures after deserialization.
    /// </summary>
    /// <param name="obj">Not used.</param>
    public virtual void OnDeserialization(object obj)
    {
      // restore the event chain
      if(m_xColumn is Altaxo.Data.DataColumn)
        ((Altaxo.Data.DataColumn)m_xColumn).Changed += new EventHandler(EhColumnDataChangedEventHandler);
      
      if(m_yColumn is Altaxo.Data.DataColumn)
        ((Altaxo.Data.DataColumn)m_yColumn).Changed += new EventHandler(EhColumnDataChangedEventHandler);
    
      if(null!=m_xBoundaries)
        m_xBoundaries.BoundaryChanged += new PhysicalBoundaries.BoundaryChangedHandler(this.OnXBoundariesChangedEventHandler);

      if(null!=m_yBoundaries)
        m_yBoundaries.BoundaryChanged += new PhysicalBoundaries.BoundaryChangedHandler(this.OnYBoundariesChangedEventHandler);
    }


    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(XYColumnPlotData),2)]
      public class XmlSerializationSurrogate2 : XmlSerializationSurrogate0
    {
      public override void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
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
            s.LabelColumn = (Altaxo.Data.IReadableColumn)labelColumn;
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
            _plotAssociation.LabelColumn = (Altaxo.Data.IReadableColumn)labelColumn;
        }

        if(bAllResolved)
          info.DeserializationFinished -= new Altaxo.Serialization.Xml.XmlDeserializationCallbackEventHandler(this.EhDeserializationFinished2);
      }
    }

  


    #endregion



    public XYColumnPlotData(Altaxo.Data.IReadableColumn xColumn, Altaxo.Data.IReadableColumn yColumn)
    {
      XColumn = xColumn;
      YColumn = yColumn;


      this.SetXBoundsFromTemplate( new FinitePhysicalBoundaries() );
      this.SetYBoundsFromTemplate( new FinitePhysicalBoundaries() );

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

      this.SetXBoundsFromTemplate( new FinitePhysicalBoundaries() );
      this.SetYBoundsFromTemplate( new FinitePhysicalBoundaries() );
        
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


    public void MergeXBoundsInto(PhysicalBoundaries pb)
    {
      if(!this.m_bCachedDataValid)
        this.CalculateCachedData();
      pb.Add(m_xBoundaries);
    }

    public void MergeYBoundsInto(PhysicalBoundaries pb)
    {
      if(!this.m_bCachedDataValid)
        this.CalculateCachedData();
      pb.Add(m_yBoundaries);
    }

    public void SetXBoundsFromTemplate(PhysicalBoundaries val)
    {
      if(null==m_xBoundaries || val.GetType() != m_xBoundaries.GetType())
      {
        if(null!=m_xBoundaries)
        {
          m_xBoundaries.BoundaryChanged -= new PhysicalBoundaries.BoundaryChangedHandler(this.OnXBoundariesChangedEventHandler);
        }
        m_xBoundaries = (PhysicalBoundaries)val.Clone();
        m_xBoundaries.BoundaryChanged += new PhysicalBoundaries.BoundaryChangedHandler(this.OnXBoundariesChangedEventHandler);
        m_bCachedDataValid = false;

        OnChanged();
      }
    }


    public void SetYBoundsFromTemplate(PhysicalBoundaries val)
    {
      if(null==m_yBoundaries || val.GetType() != m_yBoundaries.GetType())
      {
        if(null!=m_yBoundaries)
        {
          m_yBoundaries.BoundaryChanged -= new PhysicalBoundaries.BoundaryChangedHandler(this.OnYBoundariesChangedEventHandler);
        }
        m_yBoundaries = (PhysicalBoundaries)val.Clone();
        m_yBoundaries.BoundaryChanged += new PhysicalBoundaries.BoundaryChangedHandler(this.OnYBoundariesChangedEventHandler);
        m_bCachedDataValid = false;

        OnChanged();
      }
    }


    
    protected virtual void OnXBoundariesChangedEventHandler(object sender, BoundariesChangedEventArgs e)
    {
      if(null!=this.XBoundariesChanged)
        XBoundariesChanged(this, e);

      OnChanged();
    }

    protected virtual void OnYBoundariesChangedEventHandler(object sender, BoundariesChangedEventArgs e)
    {
      if(null!=this.YBoundariesChanged)
        YBoundariesChanged(this, e);

      OnChanged();
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
        return m_xColumn;
      }
      set
      {

        if(null!=m_xColumn && m_xColumn is Altaxo.Data.DataColumn)
        {
          ((Altaxo.Data.DataColumn)m_xColumn).Changed -= new EventHandler(EhColumnDataChangedEventHandler);
        }

        this.m_xColumn = value;

        if(null!=m_xColumn && m_xColumn is Altaxo.Data.DataColumn)
        {
          ((Altaxo.Data.DataColumn)m_xColumn).Changed += new EventHandler(EhColumnDataChangedEventHandler);
        }

        m_bCachedDataValid = false;
        OnChanged();
      }
    }

    public Altaxo.Data.IReadableColumn YColumn
    {
      get
      {
        return m_yColumn;
      }
      set
      {
        if(null!=m_yColumn && m_yColumn is Altaxo.Data.DataColumn)
        {
          ((Altaxo.Data.DataColumn)m_yColumn).Changed -= new EventHandler(EhColumnDataChangedEventHandler);
        }

        this.m_yColumn = value;
        if(null!=m_yColumn && m_yColumn is Altaxo.Data.DataColumn)
        {
          ((Altaxo.Data.DataColumn)m_yColumn).Changed += new EventHandler(EhColumnDataChangedEventHandler);
        }
        m_bCachedDataValid = false;
        this.OnChanged();
      }
    }

    public Altaxo.Data.IReadableColumn LabelColumn
    {
      get
      {
        return m_LabelColumn;
      }
      set
      {
        if(null!=m_LabelColumn && m_LabelColumn is Altaxo.Data.DataColumn)
        {
          ((Altaxo.Data.DataColumn)m_LabelColumn).Changed -= new EventHandler(EhColumnDataChangedEventHandler);
        }

        this.m_LabelColumn = value;
        if(null!=m_LabelColumn && m_LabelColumn is Altaxo.Data.DataColumn)
        {
          ((Altaxo.Data.DataColumn)m_LabelColumn).Changed += new EventHandler(EhColumnDataChangedEventHandler);
        }
        m_bCachedDataValid = false;
        this.OnChanged();
      }
    }

    public override string ToString()
    {
      return String.Format("{0}(X), {1}(Y)",m_xColumn.FullName,m_yColumn.FullName);
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

      m_PointCount = m_PlotRangeStart + m_PlotRangeLength;
      
      if(m_xColumn is IDefinedCount)
        m_PointCount = System.Math.Min(m_PointCount,((IDefinedCount)m_xColumn).Count);
      if(m_yColumn is IDefinedCount)
        m_PointCount = System.Math.Min(m_PointCount,((IDefinedCount)m_yColumn).Count);

      // if both columns are indefinite long, we set the length to zero
      if(m_PointCount==int.MaxValue || m_PointCount<0)
        m_PointCount=0;


      for(int i=m_PlotRangeStart;i<m_PointCount;i++)
      {
        if(!m_xColumn.IsElementEmpty(i) && !m_yColumn.IsElementEmpty(i)) 
        {
          bool x_added = this.m_xBoundaries.Add(m_xColumn,i);
          bool y_added = this.m_yBoundaries.Add(m_yColumn,i);
          if(y_added && y_added)
            m_PlottablePoints++;
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

    
    /// <summary>
    /// This will create a point list out of the data, which can be used to plot the data. In order to create this list,
    /// the function must have knowledge how to calculate the points out of the data. This will be done
    /// by a function provided by the calling function.
    /// </summary>
    /// <param name="rangeList">On return, this gives the list of plot ranges.</param>
    /// <param name="ptArray">On return, this is an array of plot points in layer coordinates.</param>
    /// <returns>True if the function is successfull, otherwise false.</returns>
    public bool GetRangesAndPoints(
      IPlotArea layer,
      out PlotRangeList rangeList,
      out PointF[] ptArray)
    {
      const double MaxRelativeValue = 1E2;

      rangeList=null;
      ptArray=null;


      Altaxo.Data.INumericColumn xColumn = this.XColumn as Altaxo.Data.INumericColumn;
      Altaxo.Data.INumericColumn yColumn = this.YColumn as Altaxo.Data.INumericColumn;

      if(null==xColumn || null==yColumn)
        return false; // this plotstyle is only for x and y double columns

      if(this.PlottablePoints<=0)
        return false;

      // allocate an array PointF to hold the line points
      ptArray = new PointF[this.PlottablePoints];

      // Fill the array with values
      // only the points where x and y are not NaNs are plotted!

      int i,j;

      bool bInPlotSpace = true;
      int  rangeStart=0;
      int  rangeOffset=0;
      rangeList = new PlotRangeList();

      Axis xAxis = layer.XAxis;
      Axis yAxis = layer.YAxis;
      I2DTo2DConverter logicalToArea = layer.LogicalToAreaConversion;


      int len = this.PlotRangeEnd;
      for(i=this.PlotRangeStart,j=0;i<len;i++)
      {
        if(Double.IsNaN(xColumn.GetDoubleAt(i)) || Double.IsNaN(yColumn.GetDoubleAt(i)))
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

        x_rel = xAxis.PhysicalToNormal(xColumn.GetDoubleAt(i));
        y_rel = yAxis.PhysicalToNormal(yColumn.GetDoubleAt(i));

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
        if(logicalToArea.Convert(x_rel,y_rel, out xcoord, out ycoord))
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
      return true;
    }
  }
}
