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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Diagnostics;
using Altaxo.Serialization;
using Altaxo.Main;
using Altaxo.Graph.Scales;

namespace Altaxo.Graph.Gdi
{
  /// <summary>
  /// This is the class that holds all elements of a graph, especially one ore more layers.
  /// </summary>
  /// <remarks>The coordinate system of the graph is in units of points (1/72 inch). The origin (0,0) of the graph
  /// is the top left corner of the printable area (and therefore _not_ the page bounds). The value of the page
  /// bounds is stored inside the class only to know what the original page size of the document was.</remarks>
  [SerializationSurrogate(0,typeof(GraphDocument.SerializationSurrogate0))]
  [SerializationVersion(0)]
  public class GraphDocument 
    :
    System.Runtime.Serialization.IDeserializationCallback,
    System.ICloneable,
    IChangedEventSource,
    Main.IChildChangedEventSink,
    Main.IDocumentNode,
    Main.INameOwner
  {
    // following default unit is point (1/72 inch)
    /// <summary>For the graph elements all the units are in points. One point is 1/72 inch.</summary>
    protected const float UnitPerInch = 72;

    /// <summary>
    /// Overall size of the page (usually the size of the sheet of paper that is selected as printing document) in point (1/72 inch)
    /// </summary>
    /// <remarks>The value is only used by hosting classes, since the reference point (0,0) of the GraphDocument
    /// is the top left corner of the printable area. The hosting class has to translate the graphics origin
    /// to that point before calling the painting routine <see cref="DoPaint"/>.</remarks>
    private RectangleF _pageBounds = new RectangleF(0, 0, 842, 595);

    
    /// <summary>
    /// The printable area of the document, i.e. the page size minus the margins at each sC:\Users\LelliD\C\CALC\Altaxo\Altaxo\Graph\GraphDocument.cside in points (1/72 inch)
    /// </summary>
    private RectangleF _printableBounds = new RectangleF(14, 14, 814 , 567 );

    XYPlotLayerCollection _layers;

    string _name;

    [NonSerialized]
    object _parent;

    /// <summary>
    /// The date/time of creation of this graph.
    /// </summary>
    protected DateTime _creationTime;

    /// <summary>
    /// The date/time when this graph was changed.
    /// </summary>
    protected DateTime _lastChangeTime;

    /// <summary>
    /// Notes concerning this graph.
    /// </summary>
    protected string _notes;

    /// <summary>
    /// An identifier that can be shown on the graph and that is searchable.
    /// </summary>
    protected string _graphIdentifier;

    /// <summary>
    /// The graph properties, key is a string, value is a property (arbitrary object) you want to store here.
    /// </summary>
    /// <remarks>The properties are saved on disc (with exception of those that starts with "tmp/".
    /// If the property you want to store is only temporary, the properties name should therefore
    /// start with "tmp/".</remarks>
    protected Dictionary<string,object> _graphProperties;

    /// <summary>Event fired when anything here changed.</summary>
    [field: NonSerialized]
    public event System.EventHandler Changed;

    Main.EventSuppressor _changedEventSuppressor;

    /// <summary>Event fired when the name changed.</summary>
    [field: NonSerialized]
    public event NameChangedEventHandler NameChanged;

    /// <summary>Event fired if either the PageBounds or the PrintableBounds changed.</summary>
    [field: NonSerialized]
    public event EventHandler BoundsChanged;



    #region "Serialization"

    /// <summary>Used to serialize the GraphDocument Version 0.</summary>
    public class SerializationSurrogate0 : System.Runtime.Serialization.ISerializationSurrogate
    {
      /// <summary>
      /// Serializes GraphDocument Version 0.
      /// </summary>
      /// <param name="obj">The GraphDocument to serialize.</param>
      /// <param name="info">The serialization info.</param>
      /// <param name="context">The streaming context.</param>
      public void GetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context  )
      {
        GraphDocument s = (GraphDocument)obj;
        System.Runtime.Serialization.ISurrogateSelector ss= AltaxoStreamingContext.GetSurrogateSelector(context);
        if(null!=ss)
        {
          // get the serialization surrogate of the base type
          System.Runtime.Serialization.ISerializationSurrogate surr =
            ss.GetSurrogate(obj.GetType().BaseType,context, out ss);
          // stream the data of the base class
          surr.GetObjectData(obj,info,context);
        }
        else 
        {
          throw new NotImplementedException(string.Format("Serializing a {0} without surrogate not implemented yet!",obj.GetType()));
        }
      
      
        // now the data of our class
        info.AddValue("PageBounds",s._pageBounds);
        info.AddValue("PrintableBounds",s._printableBounds);
      }

      /// <summary>
      /// Deserializes the GraphDocument Version 0.
      /// </summary>
      /// <param name="obj">The empty GraphDocument object to deserialize into.</param>
      /// <param name="info">The serialization info.</param>
      /// <param name="context">The streaming context.</param>
      /// <param name="selector">The deserialization surrogate selector.</param>
      /// <returns>The deserialized GraphDocument.</returns>
      public object SetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context,System.Runtime.Serialization.ISurrogateSelector selector)
      {
        GraphDocument s = (GraphDocument)obj;
        System.Runtime.Serialization.ISurrogateSelector ss = AltaxoStreamingContext.GetSurrogateSelector(context);
        if(null!=ss)
        {
          // get the serialization surrogate of the base type
          System.Runtime.Serialization.ISerializationSurrogate surr =
            ss.GetSurrogate(obj.GetType().BaseType, context, out ss);
        
          // deserialize the base type
          surr.SetObjectData(obj,info,context,selector);
        }
        else 
        {
          throw new NotImplementedException(string.Format("Serializing a {0} without surrogate not implemented yet!",obj.GetType()));
        }
        s._pageBounds      = (RectangleF)info.GetValue("PageBounds",typeof(RectangleF));
        s._printableBounds = (RectangleF)info.GetValue("PrintableBounds",typeof(RectangleF));
        return s;
      }
    }

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.GraphDocument", 0)]
    class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        GraphDocument s = (GraphDocument)obj;

        // info.AddBaseValueEmbedded(s,typeof(GraphDocument).BaseType);
        // now the data of our class
        info.AddValue("Name",s._name);
        info.AddValue("PageBounds",s._pageBounds);
        info.AddValue("PrintableBounds",s._printableBounds);
        info.AddValue("Layers",s._layers);

      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        
        GraphDocument s = null!=o ? (GraphDocument)o : new GraphDocument();

        //  info.GetBaseValueEmbedded(s,typeof(GraphDocument).BaseType,parent);
        s._name            =  info.GetString("Name"); 
        s._pageBounds      = (RectangleF)info.GetValue("PageBounds",s);
        s._printableBounds = (RectangleF)info.GetValue("PrintableBounds",s);

        s._layers          = (XYPlotLayerCollection)info.GetValue("LayerList",s);
        s._layers.ParentObject = s;


        return s;
      }
    }


    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.GraphDocument", 1)]
    class XmlSerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        GraphDocument s = (GraphDocument)obj;

        // info.AddBaseValueEmbedded(s,typeof(GraphDocument).BaseType);
        // now the data of our class
        info.AddValue("Name",s._name);
        info.AddValue("PageBounds",s._pageBounds);
        info.AddValue("PrintableBounds",s._printableBounds);
        info.AddValue("Layers",s._layers);

        // new in version 1 - Add graph properties
        int numberproperties = s._graphProperties==null ? 0 : s._graphProperties.Keys.Count;
        info.CreateArray("TableProperties",numberproperties);
        if(s._graphProperties!=null)
        {
          foreach(string propkey in s._graphProperties.Keys)
          {
            if(propkey.StartsWith("tmp/"))
              continue;
            info.CreateElement("e");
            info.AddValue("Key",propkey);
            object val = s._graphProperties[propkey];
            info.AddValue("Value",info.IsSerializable(val) ? val : null);
            info.CommitElement();
          }
        }
        info.CommitArray();


      }

        public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
        {

          GraphDocument s = null != o ? (GraphDocument)o : new GraphDocument();
          Deserialize(s, info, parent);
          return s;
        }

      public virtual void Deserialize(GraphDocument s, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        //  info.GetBaseValueEmbedded(s,typeof(GraphDocument).BaseType,parent);
        s._name            =  info.GetString("Name"); 
        s._pageBounds      = (RectangleF)info.GetValue("PageBounds",s);
        s._printableBounds = (RectangleF)info.GetValue("PrintableBounds",s);

        s._layers          = (XYPlotLayerCollection)info.GetValue("LayerList",s);
        s._layers.ParentObject = s;
        s._layers.SetPrintableGraphBounds(s._printableBounds, false);

        // new in version 1 - Add graph properties
        int numberproperties = info.OpenArray(); // "GraphProperties"
        for(int i=0;i<numberproperties;i++)
        {
          info.OpenElement(); // "e"
          string propkey = info.GetString("Key");
          object propval = info.GetValue("Value",parent);
          info.CloseElement(); // "e"
          s.SetGraphProperty(propkey,propval);
        }
        info.CloseArray(numberproperties);
      }
    }


    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase","Altaxo.Graph.GraphDocument", 2)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(GraphDocument), 3)]
    class XmlSerializationSurrogate2 : XmlSerializationSurrogate1
    {
      public override void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        base.Serialize(obj, info);
        GraphDocument s = (GraphDocument)obj;
        info.AddValue("GraphIdentifier", s._graphIdentifier);
        info.AddValue("Notes", s._notes);
        info.AddValue("CreationTime", s._creationTime.ToLocalTime());
        info.AddValue("LastChangeTime", s._lastChangeTime.ToLocalTime());
        

      }
      public override void Deserialize(GraphDocument s, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        base.Deserialize(s, info, parent);
        s._graphIdentifier = info.GetString("GraphIdentifier");
        s._notes = info.GetString("Notes");
        s._creationTime = info.GetDateTime("CreationTime").ToUniversalTime();
        s._lastChangeTime = info.GetDateTime("LastChangeTime").ToUniversalTime();
      }
    }


    /// <summary>
    /// Finale measures after deserialization.
    /// </summary>
    /// <param name="obj">Not used.</param>
    public  void OnDeserialization(object obj)
    {
    }
    #endregion


    /// <summary>
    /// Creates a empty GraphDocument with no layers and a standard size of A4 landscape.
    /// </summary>
    public GraphDocument()
    {
      this._changedEventSuppressor = new EventSuppressor(this.EhChangedEventResumes);
      this._layers = new XYPlotLayerCollection();
      this._layers.ParentObject = this;
      SetGraphPageBoundsToPrinterSettings();
      this._layers.SetPrintableGraphBounds(_printableBounds, false);
      _creationTime = _lastChangeTime = DateTime.UtcNow;
    }

    public GraphDocument(GraphDocument from)
    {
      this._changedEventSuppressor = new EventSuppressor(this.EhChangedEventResumes);
      this._pageBounds = from._pageBounds;
      this._printableBounds = from._printableBounds;
      _creationTime = _lastChangeTime = DateTime.UtcNow;
      this._notes = from._notes;


      // Clone also the table properties (deep copy)
      if(from._graphProperties!=null)
      {
        foreach(string key in from._graphProperties.Keys)
        {
          ICloneable val = from._graphProperties[key] as ICloneable;
          if(null!=val)
            this.SetGraphProperty(key,val.Clone());
        }
      }

      // the order is important here: clone the layers only before setting the printable graph bounds and other
      // properties, otherwise some errors will happen
      this._layers = (XYPlotLayerCollection)from._layers.Clone();
      this._layers.ParentObject = this;

    }

    /// <summary>
    /// Sets the page bounds of the graph document according to the current printer settings
    /// </summary>
    public void SetGraphPageBoundsToPrinterSettings()
    {
      if (null != Current.PrintingService) // if we are at design time, this is null and we use the default values above
      {
        RectangleF pageBounds = Current.PrintingService.PrintingBounds;
        System.Drawing.Printing.Margins ma = Current.PrintingService.PrintingMargins;

        // since Bounds are in 100th inch, we have to adjust them to points (72th inch)
        pageBounds.X *= UnitPerInch / 100;
        pageBounds.Y *= UnitPerInch / 100;
        pageBounds.Width *= UnitPerInch / 100;
        pageBounds.Height *= UnitPerInch / 100;

        RectangleF printableBounds = new RectangleF();
        printableBounds.X = ma.Left * UnitPerInch / 100;
        printableBounds.Y = ma.Top * UnitPerInch / 100;
        printableBounds.Width = pageBounds.Width - ((ma.Left + ma.Right) * UnitPerInch / 100);
        printableBounds.Height = pageBounds.Height - ((ma.Top + ma.Bottom) * UnitPerInch / 100);


        this.PageBounds = pageBounds;
        this.PrintableBounds = printableBounds;
      }
    }

    public object Clone()
    {
      return new GraphDocument(this);
    }
    
    public string Name
    {
      get { return _name; }
      set
      {
        if(_name!=value)
        {
          // test if an object with this name is already in the parent
          Altaxo.Main.INamedObjectCollection parentColl = ParentObject as Altaxo.Main.INamedObjectCollection;
          if(null!=parentColl && null!=parentColl.GetChildObjectNamed(value))
            throw new ApplicationException(string.Format("The graph {0} can not be renamed to {1}, since another graph with the same name already exists",this.Name,value));

          string oldValue = _name;
          _name = value;
          OnNameChanged(this,oldValue,value);
        }
      }
    }
    

    
    public virtual void OnNameChanged(object sender, string oldValue, string newValue)
    {
      if(NameChanged!=null)
        NameChanged(sender, new Altaxo.Main.NameChangedEventArgs(oldValue,newValue));
    }

    /// <summary>
    /// The date/time of creation of this graph.
    /// </summary>
    public DateTime CreationTimeUtc
    {
      get
      {
        return _creationTime;
      }
    }

    /// <summary>
    /// The date/time when this graph was changed.
    /// </summary>
    public DateTime LastChangeTimeUtc
    {
      get
      {
        return _lastChangeTime;
      }
    }

    /// <summary>
    /// Notes concerning this graph.
    /// </summary>
    public string Notes
    {
      get
      {
        return _notes;
      }
      set
      {
        _notes = value;
        OnChanged();
      }
    }

    public object ParentObject
    {
      get { return _parent; }
      set 
      {
        _parent = value;
      }
    }


    /// <summary>
    /// Gets an arbitrary object that was stored as graph property by <see cref="SetGraphProperty" />.
    /// </summary>
    /// <param name="key">Name of the property.</param>
    /// <returns>The object, or null if no object under the provided name was stored here.</returns>
    public object GetGraphProperty(string key)
    {
      return _graphProperties==null ? null : this._graphProperties[key]; 
    }


    /// <summary>
    /// The table properties, key is a string, val is a object you want to store here.
    /// </summary>
    /// <remarks>The properties are saved on disc (with exception of those who's name starts with "tmp/".
    /// If the property you want to store is only temporary, the property name should therefore
    /// start with "tmp/".</remarks>
    public void   SetGraphProperty(string key, object val)
    {
      if(_graphProperties ==null)
        _graphProperties = new Dictionary<string,object>();

      if(_graphProperties.ContainsKey(key))
        _graphProperties[key]=val;
      else
      _graphProperties.Add(key,val);
    }



    /// <summary>
    /// Fires the <see cref="BoundsChanged" /> event.
    /// </summary>
    protected void OnBoundsChanged()
    {
      if(BoundsChanged!=null)
        BoundsChanged(this,EventArgs.Empty);
    }

    /// <summary>
    /// The boundaries of the page in points (1/72 inch).
    /// </summary>
    /// <remarks>The value is only used by hosting classes, since the reference point (0,0) of the GraphDocument
    /// is the top left corner of the printable area. The hosting class has to translate the graphics origin
    /// to that point before calling the painting routine <see cref="DoPaint"/>.</remarks>
    public RectangleF PageBounds
    {
      get { return _pageBounds; }
      set 
      {
        RectangleF oldValue = _pageBounds;
        _pageBounds=value;
        if(value!=oldValue)
          OnBoundsChanged();
      }
    }

    /// <summary>
    /// The boundaries of the printable area of the page in points (1/72 inch).
    /// </summary>
    public RectangleF PrintableBounds
    {
      get { return _printableBounds; }
      set
      {
        RectangleF oldBounds = _printableBounds;
        _printableBounds=value;

        if(_printableBounds!=oldBounds)
        {
          Layers.SetPrintableGraphBounds( value, true);
          OnBoundsChanged();
        }
      }
    }


    /// <summary>
    /// The size of the printable area in points (1/72 inch).
    /// </summary>
    public virtual SizeF PrintableSize
    {
      get { return new SizeF(_printableBounds.Width,_printableBounds.Height); }
    }


    /// <summary>
    /// The collection of layers of the graph.
    /// </summary>
    public XYPlotLayerCollection Layers
    {
      get { return _layers; } 
    }


    /// <summary>
    /// Paints the graph.
    /// </summary>
    /// <param name="g">The graphic contents to paint to.</param>
    /// <param name="bForPrinting">Indicates if the painting is for printing purposes. Not used for the moment.</param>
    /// <remarks>The reference point (0,0) of the GraphDocument
    /// is the top left corner of the printable area (and not of the page area!). The hosting class has to translate the graphics origin
    /// to the top left corner of the printable area before calling this routine.</remarks>
    public void DoPaint(Graphics g, bool bForPrinting)
    {
      GraphicsState gs = g.Save();

     

      for (int i = 0; i < Layers.Count; i++)
      {
        Layers[i].PreparePainting();
      }

      for(int i=Layers.Count-1;i>=0;i--)
      {
        Layers[i].Paint(g);
      }

      g.Restore(gs);
    } // end of function DoPaint



    /// <summary>
    /// Gets the default layer position in points (1/72 inch).
    /// </summary>
    /// <value>The default position of a (new) layer in points (1/72 inch).</value>
    public PointF DefaultLayerPosition
    {
      get { return new PointF(0.145f*this.PrintableSize.Width, 0.139f*this.PrintableSize.Height); }
    }


    /// <summary>
    /// Gets the default layer size in points (1/72 inch).
    /// </summary>
    /// <value>The default size of a (new) layer in points (1/72 inch).</value>
    public SizeF DefaultLayerSize
    {
      get { return new SizeF(0.763f*this.PrintableSize.Width, 0.708f*this.PrintableSize.Height); }
    }


    #region XYPlotLayer Creation

    /// <summary>
    /// Creates a new layer with bottom x axis and left y axis, which is not linked.
    /// </summary>
    public void CreateNewLayerNormalBottomXLeftY()
    {
      XYPlotLayer newlayer= new XYPlotLayer(DefaultLayerPosition,DefaultLayerSize);
      newlayer.CreateDefaultAxes();
      Layers.Add(newlayer);
    }

    /// <summary>
    /// Creates a new layer with top x axis, which is linked to the same position with top x axis and right y axis.
    /// </summary>
    public void CreateNewLayerLinkedTopX(int linklayernumber)
    {
      XYPlotLayer newlayer= new XYPlotLayer(DefaultLayerPosition,DefaultLayerSize);
      Layers.Add(newlayer); // it is neccessary to add the new layer this early since we must set some properties relative to the linked layer
      // link the new layer to the last old layer
      newlayer.LinkedLayer = (linklayernumber>=0 && linklayernumber<Layers.Count)? Layers[linklayernumber] : null;
      newlayer.SetPosition(0,XYPlotLayerPositionType.RelativeThisNearToLinkedLayerNear,0,XYPlotLayerPositionType.RelativeThisNearToLinkedLayerNear);
      newlayer.SetSize(1,XYPlotLayerSizeType.RelativeToLinkedLayer,1,XYPlotLayerSizeType.RelativeToLinkedLayer);
      newlayer.AxisStyles.CreateDefault(new CSLineID(0, 1));
    }

    /// <summary>
    /// Creates a new layer with right y axis, which is linked to the same position with top x axis and right y axis.
    /// </summary>
    public void CreateNewLayerLinkedRightY(int linklayernumber)
    {
      XYPlotLayer newlayer= new XYPlotLayer(DefaultLayerPosition,DefaultLayerSize);
      Layers.Add(newlayer); // it is neccessary to add the new layer this early since we must set some properties relative to the linked layer
      // link the new layer to the last old layer
      newlayer.LinkedLayer = (linklayernumber>=0 && linklayernumber<Layers.Count)? Layers[linklayernumber] : null;
      newlayer.SetPosition(0,XYPlotLayerPositionType.RelativeThisNearToLinkedLayerNear,0,XYPlotLayerPositionType.RelativeThisNearToLinkedLayerNear);
      newlayer.SetSize(1,XYPlotLayerSizeType.RelativeToLinkedLayer,1,XYPlotLayerSizeType.RelativeToLinkedLayer);

      // set enabling of axis
      newlayer.AxisStyles.CreateDefault(new CSLineID(1, 1));
    }

    /// <summary>
    /// Creates a new layer with bottom x axis and left y axis, which is linked to the same position with top x axis and right y axis.
    /// </summary>
    public void CreateNewLayerLinkedTopXRightY(int linklayernumber)
    {
      XYPlotLayer newlayer= new XYPlotLayer(DefaultLayerPosition,DefaultLayerSize);
      Layers.Add(newlayer); // it is neccessary to add the new layer this early since we must set some properties relative to the linked layer
      // link the new layer to the last old layer
      newlayer.LinkedLayer = (linklayernumber>=0 && linklayernumber<Layers.Count)? Layers[linklayernumber] : null;
      newlayer.SetPosition(0,XYPlotLayerPositionType.RelativeThisNearToLinkedLayerNear,0,XYPlotLayerPositionType.RelativeThisNearToLinkedLayerNear);
      newlayer.SetSize(1,XYPlotLayerSizeType.RelativeToLinkedLayer,1,XYPlotLayerSizeType.RelativeToLinkedLayer);

      // set enabling of axis
      newlayer.AxisStyles.CreateDefault(new CSLineID(0, 1));
      newlayer.AxisStyles.CreateDefault(new CSLineID(1, 1));
     
    }


    /// <summary>
    /// Creates a new layer with bottom x axis and left y axis, which is linked to the same position with top x axis and right y axis. The x axis is linked straight to the x axis of the linked layer.
    /// </summary>
    public void CreateNewLayerLinkedTopXRightY_XAxisStraight(int linklayernumber)
    {
      XYPlotLayer newlayer= new XYPlotLayer(DefaultLayerPosition,DefaultLayerSize);
      Layers.Add(newlayer); // it is neccessary to add the new layer this early since we must set some properties relative to the linked layer
      // link the new layer to the last old layer
      newlayer.LinkedLayer = (linklayernumber>=0 && linklayernumber<Layers.Count)? Layers[linklayernumber] : null;
      newlayer.SetPosition(0,XYPlotLayerPositionType.RelativeThisNearToLinkedLayerNear,0,XYPlotLayerPositionType.RelativeThisNearToLinkedLayerNear);
      newlayer.SetSize(1,XYPlotLayerSizeType.RelativeToLinkedLayer,1,XYPlotLayerSizeType.RelativeToLinkedLayer);

      // set enabling of axis
      newlayer.AxisStyles.CreateDefault(new CSLineID(0, 1));
      newlayer.AxisStyles.CreateDefault(new CSLineID(1, 1));
      

      newlayer.LinkedScales.X.AxisLinkType = ScaleLinkType.Straight;
    }



    #endregion

 

    #region Change event handling

    protected System.EventArgs m_ChangeData=null;
    protected bool             m_ResumeInProgress=false;
    protected System.Collections.ArrayList m_SuspendedChildCollection=new System.Collections.ArrayList();

    public IDisposable BeginUpdate()
    {
      return _changedEventSuppressor.Suspend();
    }
    public void EndUpdate(ref IDisposable locker)
    {
      _changedEventSuppressor.Resume(ref locker);
    }
    
    public bool IsSuspended
    {
      get 
      {
        return _changedEventSuppressor.GetDisabledWithCounting();
      }
    }

  

    /// <summary>
    /// Fires the Invalidate event.
    /// </summary>
    /// <param name="sender">The layer which needs to be repainted.</param>
    protected internal virtual void OnInvalidate(XYPlotLayer sender)
    {
      OnChanged();
    }


    void AccumulateChildChangeData(object sender, EventArgs e)
    {
      if(sender!=null && m_ChangeData==null)
        this.m_ChangeData=new EventArgs();
    }   

    protected bool HandleImmediateChildChangeCases(object sender, EventArgs e)
    {
      return false; // not handled
    }

    protected virtual void OnSelfChanged()
    {
      EhChildChanged(null,EventArgs.Empty);
    }


    /// <summary>
    /// Handle the change notification from the child layers.
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

      if(_parent is Main.IChildChangedEventSink )
      {
        ((Main.IChildChangedEventSink)_parent).EhChildChanged(this, m_ChangeData);
        if(IsSuspended) // maybe parent has suspended us now
        {
          this.EhChildChanged(sender, e); // we call the function recursively, but now we are suspended
          return;
        }
      }
      
      OnChanged(); // Fire the changed event
    }

    void EhChangedEventResumes()
    {
      if (null != Changed)
        Changed(this, m_ChangeData);
      m_ChangeData = null;
    }

    protected virtual void OnChanged()
    {
      if (_changedEventSuppressor.GetEnabledWithCounting())
      {
        if (null != Changed)
          Changed(this, m_ChangeData);

        m_ChangeData = null;
      }
    }

    #endregion

    #region IChangedEventSource Members


    #endregion
  } // end of class GraphDocument
} // end of namespace
