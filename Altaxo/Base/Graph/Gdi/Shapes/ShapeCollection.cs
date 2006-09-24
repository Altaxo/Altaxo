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
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;
using Altaxo.Serialization;


namespace Altaxo.Graph.Gdi.Shapes
{
  /// <summary>
  /// Summary description for GraphicsObjectCollection.
  /// </summary>
  [SerializationSurrogate(0,typeof(ShapeCollection.SerializationSurrogate0))]
  [SerializationVersion(0)]
  public class ShapeCollection : Altaxo.Data.CollectionBase, Main.IChangedEventSource, Main.IChildChangedEventSink
  {

    [field:NonSerialized]
    public event System.EventHandler Changed;


    #region "Serialization"

    /// <summary>Used to serialize the GraphicsObjectCollection Version 0.</summary>
    public class SerializationSurrogate0 : System.Runtime.Serialization.ISerializationSurrogate
    {

      /// <summary>
      /// Serializes GraphicsObjectCollection Version 0.
      /// </summary>
      /// <param name="obj">The GraphicsObjectCollection to serialize.</param>
      /// <param name="info">The serialization info.</param>
      /// <param name="context">The streaming context.</param>
      public void GetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context  )
      {
        ShapeCollection s = (ShapeCollection)obj;
        info.AddValue("Data",s.myList);
      }

      /// <summary>
      /// Deserializes the GraphicsObjectCollection Version 0.
      /// </summary>
      /// <param name="obj">The empty GraphicsObjectCollection object to deserialize into.</param>
      /// <param name="info">The serialization info.</param>
      /// <param name="context">The streaming context.</param>
      /// <param name="selector">The deserialization surrogate selector.</param>
      /// <returns>The deserialized GraphicsObjectCollection.</returns>
      public object SetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context,System.Runtime.Serialization.ISurrogateSelector selector)
      {
        ShapeCollection s = (ShapeCollection)obj;

        s.myList =  (System.Collections.ArrayList)info.GetValue("Data",typeof(System.Collections.ArrayList));
        
        return s;
      }
    }

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.GraphicsObjectCollection", 0)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ShapeCollection),1)]
      public class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        ShapeCollection s = (ShapeCollection)obj;
        
        info.CreateArray("GraphObjects",s.myList.Count);
        for(int i=0;i<s.myList.Count;i++)
          info.AddValue("GraphicsObject",s.myList[i]);
        info.CommitArray();
      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        
        ShapeCollection s = null!=o ? (ShapeCollection)o : new ShapeCollection();

        int count = info.OpenArray();
        for(int i=0;i<count;i++)
        {
          ShapeBase go = (ShapeBase)info.GetValue(s);
          s.Add(go);
        }
        info.CloseArray(count);
        
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
      for(int i=0;i<Count;i++)
        this[i].Changed += new EventHandler(this.EhChildChanged);
    }
    #endregion



    public ShapeCollection()
    {
    }


    /// <summary>
    /// Copy constructor. Clones (!) all the graph items from the other collection
    /// </summary>
    /// <param name="from">The collection to clone the items from.</param>
    public ShapeCollection(ShapeCollection from)
    {
      for(int i=0;i<from.Count;i++)
        this.Add((ShapeBase)from[i].Clone());
    }

    public ShapeCollection(ShapeBase[] g)
      : base()
    {
      this.AddRange(g);
    }

    public void DrawObjects(Graphics g, float Scale, object container)
    {
      int len = this.InnerList.Count;
      for(int i=0;i<len;i++)
      {
        ((ShapeBase)this.InnerList[i]).Paint(g, container);
      }
    }

    public ShapeBase FindObjectAtPoint(PointF pt)
    {
      if(null!=this.InnerList)
      {
        int len = this.InnerList.Count;
        foreach(ShapeBase g in this.InnerList)
        {
          if(null!=g.HitTest(pt))
            return g;
        }
      }
      return null;
      
    }

    /// <summary>
    /// Scales the position of all items according to xscale and yscale.
    /// </summary>
    /// <param name="xscale"></param>
    /// <param name="yscale"></param>
    public void ScalePosition(double xscale, double yscale)
    {
      foreach(ShapeBase o in this.InnerList)
      {
        ShapeBase.ScalePosition(o,xscale,yscale);
      }
    }

    public ShapeBase this[int index]
    {
      get
      {
        return (ShapeBase)List[index];
      }
      set
      {
        value.Container = this;
        List[index] = value;

        OnChanged();
      }
    }

    public int Add(ShapeBase go)
    {
      return Add(go, true);
    }

    public int Add(ShapeBase go, bool fireChangedEvent)
    {
      go.Container = this;
      int result =  List.Add(go);

      if (fireChangedEvent)
        OnChanged();

      return result;
    }

    public void AddRange(ShapeBase[] gos)
    {
      int len = gos.Length;
      for(int i=0;i<len;i++)
        this.Add(gos[i],false);

      OnChanged();
    }

    public void AddRange(ShapeCollection goc)
    {
      int len = goc.Count;
      for(int i=0;i<len;i++)
        this.Add(goc[i],false);

      OnChanged();
    }

    public bool Contains(ShapeBase go)
    {
      return List.Contains(go);
    }

    public void CopyTo(ShapeBase[] array, int index)
    {
      List.CopyTo(array, index);
    }

    public int IndexOf(ShapeBase go)
    {
      return List.IndexOf(go);
    }
    public void Insert(int index, ShapeBase go)
    {
      List.Insert(index, go);
      OnChanged();
    }

    // See also 'System.Collections.IEnumerator'
    public new  GraphObjectEnumerator GetEnumerator()
    {
      return new GraphObjectEnumerator(this);
    }

    public void Remove(ShapeBase go)
    {
      List.Remove(go);
      OnChanged();
    }
    #region IChangedEventSource Members


    public virtual void EhChildChanged(object child, EventArgs e)
    {
      if(null!=Changed)
        Changed(this, e);
  
    }

    protected virtual void OnChanged()
    {
      if(null!=Changed)
        Changed(this, new Main.ChangedEventArgs(this,null));
    }
    #endregion
  } // end class GraphicsObjectCollection


  public class GraphObjectEnumerator :  IEnumerator
  {
    private IEnumerator baseEnumerator;

    private IEnumerable temp;


    public GraphObjectEnumerator(ShapeCollection mappings)
      : base()
    {
      this.temp = (IEnumerable)mappings;
      this.baseEnumerator = temp.GetEnumerator();
    }

    public object Current
    {
      get
      {
        return baseEnumerator.Current;
      }
    }

    public bool MoveNext()
    {
      return baseEnumerator.MoveNext();
    }


    public void Reset()
    {
      baseEnumerator.Reset();
    }


  }


}
