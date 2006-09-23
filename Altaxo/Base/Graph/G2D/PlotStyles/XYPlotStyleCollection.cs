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



namespace Altaxo.Graph.G2D.Plot.Styles
{
  using Plot.Groups;
  using Data;

  public class XYPlotStyleCollection
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


    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(XYPlotStyleCollection), 0)]
    public class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        XYPlotStyleCollection s = (XYPlotStyleCollection)obj;

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
          return new XYPlotStyleCollection(array);
        }
        else
        {
          XYPlotStyleCollection s = (XYPlotStyleCollection)o;
          for (int i = 0; i < count; i++)
            s.Add(array[i]);
          return s;
        }
      }
    }

    #endregion


    public XYPlotStyleCollection(IG2DPlotStyle[] styles)
    {
      _innerList = new List<IG2DPlotStyle>();
      for (int i = 0; i < styles.Length; ++i)
        if (styles[i] != null)
          this.Add(styles[i], false);

      this.InternalGetProviders();
    }

    public XYPlotStyleCollection(LineScatterPlotStyleKind kind)
    {
      _innerList = new List<IG2DPlotStyle>();
      

      switch (kind)
      {
        case LineScatterPlotStyleKind.Line:
          Add(new XYPlotLineStyle());
          break;

        case LineScatterPlotStyleKind.Scatter:
          Add(new XYPlotScatterStyle());
          break;

        case LineScatterPlotStyleKind.LineAndScatter:
          Add(new XYPlotLineStyle());
          Add(new XYPlotScatterStyle());
          break;

      }
    }

    public XYPlotStyleCollection()
      : this(LineScatterPlotStyleKind.Scatter)
    {
    }

    public XYPlotStyleCollection(XYPlotStyleCollection from)
    {
      CopyFrom(from);
    }
    public void CopyFrom(XYPlotStyleCollection from)
    {
      Suspend();

      Clear();

      this._changeEventPending = false;
      this._eventSuspendCount = 0;
      this._innerList = new List<IG2DPlotStyle>();
      for (int i = 0; i < from._innerList.Count; ++i)
        Add((IG2DPlotStyle)from[i].Clone());

      Resume();
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
        toadd.Changed += new EventHandler(this.EhChildChanged);
        toadd.ParentObject = this;
       

        if (withReorganizationAndEvents)
        {
          InternalGetProviders();

          OnChanged();
        }
      }
    }

    protected void Replace(IG2DPlotStyle ps, int idx, bool withReorganizationAndEvents)
    {
      if (ps != null)
      {
        IG2DPlotStyle oldStyle = this[idx];
        oldStyle.Changed -= new EventHandler(this.EhChildChanged);
        oldStyle.ParentObject =null;

        ps.Changed += new EventHandler(this.EhChildChanged);
        this._innerList[idx] = ps;
        ps.ParentObject=this;

        if (withReorganizationAndEvents)
        {
          InternalGetProviders();

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
          toadd[i].Changed += new EventHandler(this.EhChildChanged);
          toadd[i].ParentObject=this;
        }

        InternalGetProviders();

        OnChanged();

      }
    }

    public void Insert(int whichposition, IG2DPlotStyle toinsert)
    {
      if (toinsert != null)
      {
        this._innerList.Insert(whichposition, toinsert);
        toinsert.Changed += new EventHandler(this.EhChildChanged);
        toinsert.ParentObject=this;


        InternalGetProviders();

        OnChanged();

      }
    }

    public void Clear()
    {
      if (_innerList != null)
      {
        for (int i = 0; i < Count; i++)
        {
          this[i].Changed -= new EventHandler(this.EhChildChanged);
          this[i].ParentObject= null;
        }

        this._innerList.Clear();

        InternalGetProviders();
        OnChanged();
      }
    }

    public void RemoveAt(int idx)
    {
      IG2DPlotStyle removed = this[idx];
      _innerList.RemoveAt(idx);
      removed.Changed -= new EventHandler(this.EhChildChanged);
      removed.ParentObject=null;

      InternalGetProviders();
      OnChanged();
    }

    public void ExchangeItemPositions(int pos1, int pos2)
    {
      IG2DPlotStyle item1 = this[pos1];
      _innerList[pos1] = _innerList[pos2];
      _innerList[pos2] = item1;

      InternalGetProviders();
      OnChanged();

    }

    void InternalGetProviders()
    {
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
      return new XYPlotStyleCollection(this);
    }
    public XYPlotStyleCollection Clone()
    {
      return new XYPlotStyleCollection(this);
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

    public event EventHandler Changed;

    protected virtual void OnChanged()
    {
      if (_eventSuspendCount == 0 && null != Changed)
        Changed(this, new EventArgs());
      else
        _changeEventPending = true;
    }

    #endregion

    #region IChildChangedEventSink Members

    public void EhChildChanged(object child, EventArgs e)
    {
      if (this._eventSuspendCount == 0)
        InternalGetProviders();

      if (null != Changed)
        Changed(this, e);
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

    public void AddLocalGroupStyles(PlotGroupStyleCollection externalGroups, PlotGroupStyleCollection localGroups)
    {
      foreach (IG2DPlotStyle ps in this)
        ps.AddLocalGroupStyles(externalGroups, localGroups);
    }

    public void PrepareGroupStyles(PlotGroupStyleCollection externalGroups, PlotGroupStyleCollection localGroups)
    {
      foreach (IG2DPlotStyle ps in this)
        ps.PrepareGroupStyles(externalGroups, localGroups);
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
