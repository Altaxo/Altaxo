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
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections.Generic;



namespace Altaxo.Graph.Gdi.Plot.Styles
{
  using Plot.Groups;
  using Data;
  using Graph.Plot.Groups;

  public class G2DPlotStyleCollection
    :
    IEnumerable<IG2DPlotStyle>,
    IG2DPlotStyle,
    Main.IChangedEventSource,
    Main.IChildChangedEventSink,
    Main.IDocumentNode
  {
    /// <summary>
    /// Holds the plot styles
    /// </summary>
    List<IG2DPlotStyle> _innerList;

    int _eventSuspendCount;
    bool _changeEventPending;

    /// <summary>
    /// The parent object.
    /// </summary>
    protected object _parent;

    #region Serialization


    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase","Altaxo.Graph.XYPlotStyleCollection", 0)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(G2DPlotStyleCollection), 1)]
    public class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        G2DPlotStyleCollection s = (G2DPlotStyleCollection)obj;

        info.CreateArray("Styles", s._innerList.Count);
        for (int i = 0; i < s._innerList.Count; i++)
          info.AddValue("e", s._innerList[i]);
        info.CommitArray();

      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {

        int count = info.OpenArray();
        IG2DPlotStyle[] array = new IG2DPlotStyle[count];
        for (int i = 0; i < count; i++)
          array[i] = (IG2DPlotStyle)info.GetValue("e", this);
        info.CloseArray(count);

        if (o == null)
        {
          return new G2DPlotStyleCollection(array);
        }
        else
        {
          G2DPlotStyleCollection s = (G2DPlotStyleCollection)o;
          for (int i = 0; i < count; i++)
            s.Add(array[i]);
          return s;
        }
      }
    }

    #endregion

    /// <summary>
    /// Creates an empty collection, i.e. without any styles (so the item is not visible). You must manually add styles to make the plot item visible.
    /// </summary>
    public G2DPlotStyleCollection()
    {
      _innerList = new List<IG2DPlotStyle>();
    }


    public G2DPlotStyleCollection(IG2DPlotStyle[] styles)
    {
      _innerList = new List<IG2DPlotStyle>();
      for (int i = 0; i < styles.Length; ++i)
        if (styles[i] != null)
          this.Add(styles[i], false);
    }

    public G2DPlotStyleCollection(LineScatterPlotStyleKind kind)
    {
      _innerList = new List<IG2DPlotStyle>();

      switch (kind)
      {
        case LineScatterPlotStyleKind.Line:
          Add(new LinePlotStyle());
          break;

        case LineScatterPlotStyleKind.Scatter:
          Add(new ScatterPlotStyle());
          break;

        case LineScatterPlotStyleKind.LineAndScatter:
          Add(new LinePlotStyle());
          Add(new ScatterPlotStyle());
          break;

      }
    }


    public G2DPlotStyleCollection(G2DPlotStyleCollection from)
    {
      CopyFrom(from);
    }
    public void CopyFrom(G2DPlotStyleCollection from)
    {
      Suspend();

      Clear();

      this._innerList = new List<IG2DPlotStyle>();
      for (int i = 0; i < from._innerList.Count; ++i)
        Add((IG2DPlotStyle)from[i].Clone());

      this._parent = from._parent;

      Resume();
    }
    public void SetFromTemplate(G2DPlotStyleCollection from, PlotGroupStrictness strictness)
    {
      if (strictness == PlotGroupStrictness.Strict)
      {
        CopyFrom(from);
      }
      else if (strictness == PlotGroupStrictness.Exact)
      {
        // note one sub style in the 'from' collection can update only one item in the 'this' collection
        Suspend();
        int myidx = 0;
        foreach (IG2DPlotStyle style in from)
        {
          for (int i = myidx; i < this.Count; i++)
          {
            if (this[i].GetType() == style.GetType())
            {
              Replace((IG2DPlotStyle)from[i].Clone(), i, false);
              myidx = i+1;
              break;
            }
          }
        }
        Resume();
      }
    }

    public virtual object ParentObject
    {
      get { return _parent; }
      set { _parent = value; }
    }

    public IG2DPlotStyle this[int i]
    {
      get
      {
        return _innerList[i];
      }
    }

    public int Count
    {
      get
      {
        return _innerList.Count;
      }
    }

    public void Add(IG2DPlotStyle toadd)
    {
      Add(toadd, true);
    }

    protected void Add(IG2DPlotStyle toadd, bool withReorganizationAndEvents)
    {
      if (toadd != null)
      {
        this._innerList.Add(toadd);
        toadd.ParentObject = this;
       

        if (withReorganizationAndEvents)
        {
          OnChanged();
        }
      }
    }

    protected void Replace(IG2DPlotStyle ps, int idx, bool withReorganizationAndEvents)
    {
      if (ps != null)
      {
        this._innerList[idx] = ps;
        ps.ParentObject=this;

        if (withReorganizationAndEvents)
        {
          OnChanged();
        }
      }
    }

    public void AddRange(IG2DPlotStyle[] toadd)
    {
      if (toadd != null)
      {
        for (int i = 0; i < toadd.Length; i++)
        {
          this._innerList.Add(toadd[i]);
          toadd[i].ParentObject=this;
        }

      

        OnChanged();

      }
    }

    public void Insert(int whichposition, IG2DPlotStyle toinsert)
    {
      if (toinsert != null)
      {
        this._innerList.Insert(whichposition, toinsert);
        toinsert.ParentObject=this;


       

        OnChanged();

      }
    }

    public void Clear()
    {
      if (_innerList != null)
      {
        this._innerList.Clear();
      
        OnChanged();
      }
    }

    public void RemoveAt(int idx)
    {
      IG2DPlotStyle removed = this[idx];
      _innerList.RemoveAt(idx);
   
      OnChanged();
    }

    public void ExchangeItemPositions(int pos1, int pos2)
    {
      IG2DPlotStyle item1 = this[pos1];
      _innerList[pos1] = _innerList[pos2];
      _innerList[pos2] = item1;

   
      OnChanged();

    }

   

    public void BeginUpdate()
    {
      Suspend();
    }
    void Suspend()
    {
      ++_eventSuspendCount;
    }
    public void EndUpdate()
    {
      Resume();
    }
    void Resume()
    {
      --_eventSuspendCount;
      if (0 == _eventSuspendCount)
      {
        if (_changeEventPending)
          OnChanged();
      }
    }

    object ICloneable.Clone()
    {
      return new G2DPlotStyleCollection(this);
    }
    public G2DPlotStyleCollection Clone()
    {
      return new G2DPlotStyleCollection(this);
    }


    public void Paint(Graphics g, IPlotArea layer, Processed2DPlotData pdata)
    {
      for (int i = 0; i < _innerList.Count; i++)
      {
        this[i].Paint(g, layer, pdata);
      }
    }


    public RectangleF PaintSymbol(System.Drawing.Graphics g, System.Drawing.RectangleF bounds)
    {
      foreach (IG2DPlotStyle ps in this)
        bounds = ps.PaintSymbol(g, bounds);

      return bounds;
    }

  
    

    #region IChangedEventSource Members

    [field:NonSerialized]
    public event EventHandler Changed;

    protected virtual void OnChanged()
    {
      if (_eventSuspendCount > 0)
      {
        _changeEventPending = true;
        return;
      }

      if (_parent is Main.IChildChangedEventSink)
        ((Main.IChildChangedEventSink)_parent).EhChildChanged(this, EventArgs.Empty);

      if (null != Changed)
        Changed(this, EventArgs.Empty);
    }

    #endregion

    #region IChildChangedEventSink Members

    public void EhChildChanged(object child, EventArgs e)
    {
      OnChanged();
    }

    #endregion

  

    #region IEnumerable<IPlotStyle> Members

    public IEnumerator<IG2DPlotStyle> GetEnumerator()
    {
      return _innerList.GetEnumerator();
    }

    #endregion

    #region IEnumerable Members

    IEnumerator IEnumerable.GetEnumerator()
    {
      return _innerList.GetEnumerator();
    }

    #endregion

    #region IPlotStyle Members

    public void CollectExternalGroupStyles(PlotGroupStyleCollection externalGroups)
    {
      foreach (IG2DPlotStyle ps in this)
        ps.CollectExternalGroupStyles(externalGroups);
    }


    public void CollectLocalGroupStyles(PlotGroupStyleCollection externalGroups, PlotGroupStyleCollection localGroups)
    {
      foreach (IG2DPlotStyle ps in this)
        ps.CollectLocalGroupStyles(externalGroups, localGroups);
    }

    public void PrepareGroupStyles(PlotGroupStyleCollection externalGroups, PlotGroupStyleCollection localGroups, IPlotArea layer, Processed2DPlotData pdata)
    {
      foreach (IG2DPlotStyle ps in this)
        ps.PrepareGroupStyles(externalGroups, localGroups,layer,pdata);
    }

    public void ApplyGroupStyles(PlotGroupStyleCollection externalGroups, PlotGroupStyleCollection localGroups)
    {
      foreach (IG2DPlotStyle ps in this)
        ps.ApplyGroupStyles(externalGroups, localGroups);
    }

    #endregion

    #region IDocumentNode Members


    public string Name
    {
      get { return "PlotStyleCollection"; }
    }

    #endregion
  }
}
