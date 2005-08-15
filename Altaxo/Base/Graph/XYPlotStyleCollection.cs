using System;
using System.Collections;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Altaxo.Graph
{
  public class XYPlotStyleCollection 
    :
    AbstractXYPlotStyle,
    Main.IChangedEventSource, 
    Main.IChildChangedEventSink,
    I2DPlotItemStyle
  {
    /// <summary>
    /// Index to the color provider. Is set to -1 if no color provider currently exists.
    /// </summary>
    int _colorProvider;

    /// <summary>
    /// Index to the symbol size provider. Is set to -1 if no symbol size provider currently exists.
    /// </summary>
    int _symbolSizeProvider;

    /// <summary>
    /// Holds the plot styles
    /// </summary>
    ArrayList _innerList;

    int _eventSuspendCount;
    bool _changeEventPending;


    #region Serialization
  

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(XYPlotStyleCollection), 0)]
    public class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        XYPlotStyleCollection s = (XYPlotStyleCollection)obj;

        info.CreateArray("Styles",s._innerList.Count);
        for (int i = 0; i < s._innerList.Count; i++)
          info.AddValue("e", s._innerList[i]);
        info.CommitArray();

      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {

        int count = info.OpenArray();
        I2DPlotStyle[] array = new I2DPlotStyle[count];
        for (int i = 0; i < count; i++)
          array[i] = (I2DPlotStyle)info.GetValue("e",this);
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


    public XYPlotStyleCollection(I2DPlotStyle[] styles)
    {
      _innerList = new ArrayList();
      for(int i=0;i<styles.Length;++i)
        if(styles[i]!=null)
          this.Add(styles[i],false);

      this.InternalGetProviders();
    }

    public XYPlotStyleCollection(LineScatterPlotStyleKind kind)
    {
      _innerList = new ArrayList();
      _colorProvider = -1;
      _symbolSizeProvider = -1;

      switch (kind)
      {
        case LineScatterPlotStyleKind.Line:
          Add(new XYPlotLineStyle());
          _colorProvider = 0;
          break;
        case LineScatterPlotStyleKind.Scatter:
          Add(new XYPlotScatterStyle());
          _colorProvider = 0;
          _symbolSizeProvider = 0;
          break;
        case LineScatterPlotStyleKind.LineAndScatter:
          Add(new XYPlotLineStyle());
          Add(new XYPlotScatterStyle());
          _colorProvider = 0;
          _symbolSizeProvider = 1;
          break;
      }
    }

    public XYPlotStyleCollection()
    : this(LineScatterPlotStyleKind.Line)
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
      this._colorProvider = from._colorProvider;
      this._symbolSizeProvider = from._symbolSizeProvider;
      this._innerList = new ArrayList();
      for (int i = 0; i < from._innerList.Count; ++i)
        Add((I2DPlotStyle)from[i].Clone());

      Resume();
    }

    public I2DPlotStyle this[int i]
    {
      get
      {
        return _innerList[i] as I2DPlotStyle;
      }
    }

    public int Count
    {
      get
      {
        return _innerList.Count;
      }
    }

    public void Add(I2DPlotStyle toadd)
    {
      Add(toadd,true);
    }

    protected void Add(I2DPlotStyle toadd, bool withReorganizationAndEvents)
    {
      if (toadd != null)
      {
        this._innerList.Add(toadd);
        toadd.Changed += new EventHandler(this.OnChildChanged);

        if (withReorganizationAndEvents)
        {
          InternalGetProviders();

          OnChanged();
        }
      }
    }

   public void AddRange(I2DPlotStyle[] toadd)
    {
      if (toadd != null)
      {
        for(int i=0;i<toadd.Length;i++)
        {
          this._innerList.Add(toadd[i]);
          toadd[i].Changed += new EventHandler(this.OnChildChanged);
        }

      
          InternalGetProviders();

          OnChanged();
       
      }
    }

    public void Insert(int whichposition, I2DPlotStyle toinsert )
    {
      if (toinsert != null)
      {
        this._innerList.Insert(whichposition, toinsert);
        toinsert.Changed += new EventHandler(this.OnChildChanged);

        
          InternalGetProviders();

          OnChanged();
       
      }
    }

    public void Clear()
    {
      if(_innerList!=null)
      {
        for(int i=0;i<Count;i++)
          this[i].Changed -= new EventHandler(this.OnChildChanged);

        this._innerList.Clear();

        InternalGetProviders();
        OnChanged();
      }
    }

    public void RemoveAt(int idx)
    {
      I2DPlotStyle removed = this[idx];
      _innerList.RemoveAt(idx);
      removed.Changed -= new EventHandler(this.OnChildChanged);

      InternalGetProviders();
      OnChanged();
    }

    public void ExchangeItemPositions(int pos1, int pos2)
    {
      I2DPlotStyle item1 = this[pos1];
      _innerList[pos1] = _innerList[pos2];
      _innerList[pos2] = item1;

      InternalGetProviders();
      OnChanged();

    }

    void InternalGetProviders()
    {
      _colorProvider = -1;
      for (int i = 0; i < _innerList.Count; i++)
      {
        if (this[i].IsColorProvider)
        {
          _colorProvider = i;
          break;
        }
      }

      _symbolSizeProvider = -1;
      for (int i = 0; i < _innerList.Count; i++)
      {
        if (this[i].IsSymbolSizeProvider)
        {
          _symbolSizeProvider = i;
          break;
        }
      }

      Suspend();
      Color color = _colorProvider >= 0 ? this[_colorProvider].Color : Color.Black;
      float symbolSize = _symbolSizeProvider >= 0 ? this[_symbolSizeProvider].SymbolSize : 0;
      for (int i = 0; i < _innerList.Count; i++)
      {
        if (this[i].IsColorReceiver)
          this[i].Color = color;

        if (this[i].IsSymbolSizeReceiver)
          this[i].SymbolSize = symbolSize;
      }
      Resume();
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

    public override object Clone()
    {
      return new XYPlotStyleCollection(this);
    }


    public override void Paint(Graphics g, IPlotArea layer, object plotObject)
    {
      throw new NotImplementedException();
    }
  

    public void Paint(Graphics g, IPlotArea layer, PlotRangeList rangeList, PointF[] ptArray)
    {
      for (int i = 0; i < _innerList.Count; i++)
      {
        this[i].Paint(g, layer, rangeList, ptArray);
      }
    }

    public override System.Drawing.SizeF PaintSymbol(System.Drawing.Graphics g, System.Drawing.PointF pos, float width)
    {
      GraphicsState gs = g.Save();

      float symsize = this.SymbolSize;
      float linelen = width / 2; // distance from start to symbol centre

      g.TranslateTransform(pos.X + linelen, pos.Y);

      for (int i = 0; i < _innerList.Count; ++i)
      {
        if (this[i] is XYPlotLineStyle)
        {
          XYPlotLineStyle lineStyle = this[i] as XYPlotLineStyle;
          if (lineStyle.Connection != XYPlotLineStyles.ConnectionStyle.NoLine)
          {
            if (lineStyle.LineSymbolGap == true)
            {
              // plot a line with the length of symbolsize from 
              lineStyle.PaintLine(g, new PointF(-linelen, 0), new PointF(-symsize, 0));
              lineStyle.PaintLine(g, new PointF(symsize, 0), new PointF(linelen, 0));
            }
            else // no gap
            {
              lineStyle.PaintLine(g, new PointF(-linelen, 0), new PointF(linelen, 0));
            }
          }
        }
      }
      // now Paint the symbols
      for (int i = 0; i < _innerList.Count; ++i)
      {
        if (this[i] is XYPlotScatterStyle)
        {
          XYPlotScatterStyle scatterStyle = this[i] as XYPlotScatterStyle;
          if (scatterStyle.Shape != XYPlotScatterStyles.Shape.NoSymbol)
            scatterStyle.Paint(g);
        }
      }

      g.Restore(gs);

      return new SizeF(2 * linelen, symsize);
    }

    public override void SetToNextStyle(AbstractXYPlotStyle ps, PlotGroupStyle style)
    {
      XYPlotLineStyle lineStyle;
      XYPlotScatterStyle scatterStyle;

      if (0 != (style & PlotGroupStyle.Line))
        if(null!=(lineStyle=this.XYPlotLineStyle))
          lineStyle.SetToNextLineStyle(ps.XYPlotLineStyle);
      
      if (0 != (style & PlotGroupStyle.Symbol))
        if(null!=(scatterStyle=this.XYPlotScatterStyle))
          scatterStyle.SetToNextStyle(ps.XYPlotScatterStyle);
      
      // Color has to be the last, since during the previous operations the styles are cloned, 
      // inclusive the color
      if (0 != (style & PlotGroupStyle.Color))
        this.Color = GetNextPlotColor(ps.Color);
    }

    public void SetIncrementalStyle(I2DPlotItemStyle pstemplate, PlotGroupStyle style, int step)
    {
     

      if ((0 != (style & PlotGroupStyle.Line)) && pstemplate.IsXYLineStyleSupported)
        if(this.XYPlotLineStyle!=null)
          this.XYPlotLineStyle.SetToNextLineStyle(pstemplate.XYLineStyle, step);
      
      if ((0 != (style & PlotGroupStyle.Symbol)) && pstemplate.IsXYScatterStyleSupported)
        if(this.XYPlotScatterStyle!=null)
          this.XYPlotScatterStyle.SetToNextStyle(pstemplate.XYScatterStyle, step);

      // Color has to be the last, since during the previous operations the styles are cloned, 
      // inclusive the color
      if ((0 != (style & PlotGroupStyle.Color)) && pstemplate.IsColorProvider)
        this.Color = GetNextPlotColor(pstemplate.Color, step);
    }

    public override System.Drawing.Color Color
    {
      get
      {
        if (_colorProvider > 0)
          return this[_colorProvider].Color;
        else
          return Color.Black;
      }
      set
      {
        for (int i = 0; i < _innerList.Count; ++i)
        {
          if (this[i].IsColorReceiver)
            this[i].Color = value;
        }
      }
    }

    public override float SymbolSize
    {
      get
      {
        return this._symbolSizeProvider<0 ? 0 : this[_symbolSizeProvider].SymbolSize;
      }
      set
      {
        if(this._symbolSizeProvider>=0)
        {
          if(this[_symbolSizeProvider].IsSymbolSizeReceiver)
            this[_symbolSizeProvider].SymbolSize = value;
        }
      }
    }


      public override XYPlotLineStyle XYPlotLineStyle
    {
      get
      {
        for (int i = 0; i < _innerList.Count; ++i)
          if (this[i] is XYPlotLineStyle)
            return this[i] as XYPlotLineStyle;

        return null;
      }
      set
      {
        throw new Exception("The method or operation is not implemented.");
      }
    }

    public override XYPlotScatterStyle XYPlotScatterStyle
    {
      get
      {
        for (int i = 0; i < _innerList.Count; ++i)
          if (this[i] is XYPlotScatterStyle)
            return this[i] as XYPlotScatterStyle;

        return null;
      }
      set
      {
        throw new Exception("The method or operation is not implemented.");
      }
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

    public void OnChildChanged(object child, EventArgs e)
    {
      if(this._eventSuspendCount==0)
        InternalGetProviders();

      if (null != Changed)
        Changed(this, e);
    }

    #endregion

    #region I2DPlotItemStyle Members

    public bool IsColorProvider
    {
      get { return _colorProvider >= 0; }
    }

    public bool IsXYLineStyleSupported
    {
      get { return this.XYPlotLineStyle != null; }
    }

    public XYPlotLineStyle XYLineStyle
    {
      get { return this.XYPlotLineStyle; }
    }

    public bool IsXYScatterStyleSupported
    {
      get { return this.XYPlotScatterStyle != null; }
    }

    public XYPlotScatterStyle XYScatterStyle
    {
      get { return this.XYPlotScatterStyle; }
    }

    #endregion
}
}
