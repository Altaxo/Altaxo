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
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Diagnostics;
using Altaxo.Serialization;
using Altaxo.Main;


namespace Altaxo.Graph
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

    /// <summary>
    /// Overall size of the page (usually the size of the sheet of paper that is selected as printing document) in point (1/72 inch)
    /// </summary>
    /// <remarks>The value is only used by hosting classes, since the reference point (0,0) of the GraphDocument
    /// is the top left corner of the printable area. The hosting class has to translate the graphics origin
    /// to that point before calling the painting routine <see cref="DoPaint"/>.</remarks>
    private RectangleF m_PageBounds = new RectangleF(0, 0, 842, 595);

    
    /// <summary>
    /// The printable area of the document, i.e. the page size minus the margins at each sC:\Users\LelliD\C\CALC\Altaxo\Altaxo\Graph\GraphDocument.cside in points (1/72 inch)
    /// </summary>
    private RectangleF m_PrintableBounds = new RectangleF(14, 14, 814 , 567 );

    XYPlotLayerCollection m_Layers;

    string m_Name;

    object m_Parent;

    /// <summary>
    /// The graph properties, key is a string, value is a property (arbitrary object) you want to store here.
    /// </summary>
    /// <remarks>The properties are saved on disc (with exception of those who starts with "tmp/".
    /// If the property you want to store is only temporary, the properties name should therefore
    /// start with "tmp/".</remarks>
    protected System.Collections.Hashtable _GraphProperties;

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
        info.AddValue("PageBounds",s.m_PageBounds);
        info.AddValue("PrintableBounds",s.m_PrintableBounds);
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
        s.m_PageBounds      = (RectangleF)info.GetValue("PageBounds",typeof(RectangleF));
        s.m_PrintableBounds = (RectangleF)info.GetValue("PrintableBounds",typeof(RectangleF));
        return s;
      }
    }

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(GraphDocument),0)]
      public class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        GraphDocument s = (GraphDocument)obj;

        // info.AddBaseValueEmbedded(s,typeof(GraphDocument).BaseType);
        // now the data of our class
        info.AddValue("Name",s.m_Name);
        info.AddValue("PageBounds",s.m_PageBounds);
        info.AddValue("PrintableBounds",s.m_PrintableBounds);
        info.AddValue("Layers",s.m_Layers);

      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        
        GraphDocument s = null!=o ? (GraphDocument)o : new GraphDocument();

        //  info.GetBaseValueEmbedded(s,typeof(GraphDocument).BaseType,parent);
        s.m_Name            =  info.GetString("Name"); 
        s.m_PageBounds      = (RectangleF)info.GetValue("PageBounds",s);
        s.m_PrintableBounds = (RectangleF)info.GetValue("PrintableBounds",s);

        s.m_Layers          = (XYPlotLayerCollection)info.GetValue("LayerList",s);
        s.m_Layers.ParentObject = s;


        return s;
      }
    }

   
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(GraphDocument),1)]
      public class XmlSerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        GraphDocument s = (GraphDocument)obj;

        // info.AddBaseValueEmbedded(s,typeof(GraphDocument).BaseType);
        // now the data of our class
        info.AddValue("Name",s.m_Name);
        info.AddValue("PageBounds",s.m_PageBounds);
        info.AddValue("PrintableBounds",s.m_PrintableBounds);
        info.AddValue("Layers",s.m_Layers);

        // new in version 1 - Add graph properties
        int numberproperties = s._GraphProperties==null ? 0 : s._GraphProperties.Keys.Count;
        info.CreateArray("TableProperties",numberproperties);
        if(s._GraphProperties!=null)
        {
          foreach(string propkey in s._GraphProperties.Keys)
          {
            if(propkey.StartsWith("tmp/"))
              continue;
            info.CreateElement("e");
            info.AddValue("Key",propkey);
            object val = s._GraphProperties[propkey];
            info.AddValue("Value",info.IsSerializable(val) ? val : null);
            info.CommitElement();
          }
        }
        info.CommitArray();


      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        
        GraphDocument s = null!=o ? (GraphDocument)o : new GraphDocument();

        //  info.GetBaseValueEmbedded(s,typeof(GraphDocument).BaseType,parent);
        s.m_Name            =  info.GetString("Name"); 
        s.m_PageBounds      = (RectangleF)info.GetValue("PageBounds",s);
        s.m_PrintableBounds = (RectangleF)info.GetValue("PrintableBounds",s);

        s.m_Layers          = (XYPlotLayerCollection)info.GetValue("LayerList",s);
        s.m_Layers.ParentObject = s;

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
        return s;
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
      this.m_Layers = new Altaxo.Graph.XYPlotLayerCollection();
      this.m_Layers.ParentObject = this;
      this.m_Layers.SetPrintableGraphBounds(m_PrintableBounds,false);
    }

    public GraphDocument(GraphDocument from)
    {
      this.m_Layers = (XYPlotLayerCollection)from.m_Layers.Clone();
      this.m_Layers.ParentObject = this;
      this.m_PageBounds = from.m_PageBounds;
      this.m_PrintableBounds = from.m_PrintableBounds;


      // Clone also the table properties (deep copy)
      if(from._GraphProperties!=null)
      {
        foreach(string key in from._GraphProperties.Keys)
        {
          ICloneable val = from._GraphProperties[key] as ICloneable;
          if(null!=val)
            this.SetGraphProperty(key,val.Clone());
        }
      }
    }

    public object Clone()
    {
      return new GraphDocument(this);
    }
    
    public string Name
    {
      get { return m_Name; }
      set
      {
        if(m_Name!=value)
        {
          // test if an object with this name is already in the parent
          Altaxo.Main.INamedObjectCollection parentColl = ParentObject as Altaxo.Main.INamedObjectCollection;
          if(null!=parentColl && null!=parentColl.GetChildObjectNamed(value))
            throw new ApplicationException(string.Format("The graph {0} can not be renamed to {1}, since another graph with the same name already exists",this.Name,value));

          string oldValue = m_Name;
          m_Name = value;
          OnNameChanged(this,oldValue,value);
        }
      }
    }
    

    public event NameChangedEventHandler NameChanged;
    public virtual void OnNameChanged(object sender, string oldValue, string newValue)
    {
      if(NameChanged!=null)
        NameChanged(sender, new Altaxo.Main.NameChangedEventArgs(oldValue,newValue));
    }


    public object ParentObject
    {
      get { return m_Parent; }
      set 
      {
        m_Parent = value;
      }
    }


    /// <summary>
    /// Gets an arbitrary object that was stored as graph property by <see cref="SetGraphProperty" />.
    /// </summary>
    /// <param name="key">Name of the property.</param>
    /// <returns>The object, or null if no object under the provided name was stored here.</returns>
    public object GetGraphProperty(string key)
    {
      return _GraphProperties==null ? null : this._GraphProperties[key]; 
    }


    /// <summary>
    /// The table properties, key is a string, val is a object you want to store here.
    /// </summary>
    /// <remarks>The properties are saved on disc (with exception of those who's name starts with "tmp/".
    /// If the property you want to store is only temporary, the property name should therefore
    /// start with "tmp/".</remarks>
    public void   SetGraphProperty(string key, object val)
    {
      if(_GraphProperties ==null)
        _GraphProperties = new System.Collections.Hashtable();

      if(_GraphProperties[key]==null)
        _GraphProperties.Add(key,val);
      else
        _GraphProperties[key]=val;
    }


    /// <summary>
    /// Event fired if either the PageBounds or the PrintableBounds changed
    /// </summary>
    public event EventHandler BoundsChanged;

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
      get { return m_PageBounds; }
      set 
      {
        RectangleF oldValue = m_PageBounds;
        m_PageBounds=value;
        if(value!=oldValue)
          OnBoundsChanged();
      }
    }

    /// <summary>
    /// The boundaries of the printable area of the page in points (1/72 inch).
    /// </summary>
    public RectangleF PrintableBounds
    {
      get { return m_PrintableBounds; }
      set
      {
        RectangleF oldBounds = m_PrintableBounds;
        m_PrintableBounds=value;

        if(m_PrintableBounds!=oldBounds)
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
      get { return new SizeF(m_PrintableBounds.Width,m_PrintableBounds.Height); }
    }


    /// <summary>
    /// The collection of layers of the graph.
    /// </summary>
    public XYPlotLayerCollection Layers
    {
      get { return m_Layers; } 
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

      for(int i=0;i<Layers.Count;i++)
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
      newlayer.TopAxisEnabled=false;
      newlayer.RightAxisEnabled=false;
    
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
      newlayer.SetPosition(0,XYPlotLayer.PositionType.RelativeThisNearToLinkedLayerNear,0,XYPlotLayer.PositionType.RelativeThisNearToLinkedLayerNear);
      newlayer.SetSize(1,XYPlotLayer.SizeType.RelativeToLinkedLayer,1,XYPlotLayer.SizeType.RelativeToLinkedLayer);

      // set enabling of axis
      newlayer.BottomAxisEnabled=false;
      newlayer.LeftAxisEnabled=false;
      newlayer.RightAxisEnabled=false;
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
      newlayer.SetPosition(0,XYPlotLayer.PositionType.RelativeThisNearToLinkedLayerNear,0,XYPlotLayer.PositionType.RelativeThisNearToLinkedLayerNear);
      newlayer.SetSize(1,XYPlotLayer.SizeType.RelativeToLinkedLayer,1,XYPlotLayer.SizeType.RelativeToLinkedLayer);

      // set enabling of axis
      newlayer.BottomAxisEnabled=false;
      newlayer.TopAxisEnabled=false;
      newlayer.LeftAxisEnabled=false;
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
      newlayer.SetPosition(0,XYPlotLayer.PositionType.RelativeThisNearToLinkedLayerNear,0,XYPlotLayer.PositionType.RelativeThisNearToLinkedLayerNear);
      newlayer.SetSize(1,XYPlotLayer.SizeType.RelativeToLinkedLayer,1,XYPlotLayer.SizeType.RelativeToLinkedLayer);

      // set enabling of axis
      newlayer.BottomAxisEnabled=false;
      newlayer.LeftAxisEnabled=false;
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
      newlayer.SetPosition(0,XYPlotLayer.PositionType.RelativeThisNearToLinkedLayerNear,0,XYPlotLayer.PositionType.RelativeThisNearToLinkedLayerNear);
      newlayer.SetSize(1,XYPlotLayer.SizeType.RelativeToLinkedLayer,1,XYPlotLayer.SizeType.RelativeToLinkedLayer);

      // set enabling of axis
      newlayer.BottomAxisEnabled=false;
      newlayer.LeftAxisEnabled=false;

      newlayer.AxisProperties.X.AxisLinkType = XYPlotLayer.AxisLinkType.Straight;
    }



    #endregion

    #region inherited from XYPlotLayer.Collection

    
    #endregion

    #region Change event handling

    protected System.EventArgs m_ChangeData=null;
    protected bool             m_ResumeInProgress=false;
    protected System.Collections.ArrayList m_SuspendedChildCollection=new System.Collections.ArrayList();

    
    public bool IsSuspended
    {
      get 
      {
        return false; // m_SuspendCount>0;
      }
    }

#if false
    public void Suspend()
    {
      System.Diagnostics.Debug.Assert(m_SuspendCount>=0,"SuspendCount must always be greater or equal to zero");    

      ++m_SuspendCount; // suspend one step higher
    }

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
            ((Main.IChildChangedEventSink)m_Parent).OnChildChanged(this, m_ChangeData);
          }
          if(!IsSuspended)
          {
            OnChanged(); // Fire the changed event
          }   
        }
      }
    }

#endif


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
      OnChildChanged(null,EventArgs.Empty);
    }


    /// <summary>
    /// Handle the change notification from the child layers.
    /// </summary>
    /// <param name="sender">The sender of the change notification.</param>
    /// <param name="e">The change details.</param>
    public void OnChildChanged(object sender, System.EventArgs e)
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
        ((Main.IChildChangedEventSink)m_Parent).OnChildChanged(this, m_ChangeData);
        if(IsSuspended) // maybe parent has suspended us now
        {
          this.OnChildChanged(sender, e); // we call the function recursively, but now we are suspended
          return;
        }
      }
      
      OnChanged(); // Fire the changed event
    }

    protected virtual void OnChanged()
    {
      if(null!=Changed)
        Changed(this, m_ChangeData);

      m_ChangeData=null;
    }

    #endregion

    #region IChangedEventSource Members

    public event System.EventHandler Changed;

    #endregion
  } // end of class GraphDocument
} // end of namespace
