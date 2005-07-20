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

    public XYPlotStyleCollection(I2DPlotStyle[] styles)
    {
      _innerList = new ArrayList();
      for(int i=0;i<styles.Length;++i)
        _innerList.Add(styles[i]);

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


    public I2DPlotStyle this[int i]
    {
      get
      {
        return _innerList[i] as I2DPlotStyle;
      }
    }

    public void Add(I2DPlotStyle toadd)
    {
      this._innerList.Add(toadd);
      InternalGetProviders();
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


    }

    public override object Clone()
    {
      throw new Exception("The method or operation is not implemented.");
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
        if (this[i] is XYPlotLineStyle)
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

    #endregion

    #region IChildChangedEventSink Members

    public void OnChildChanged(object child, EventArgs e)
    {
      throw new Exception("The method or operation is not implemented.");
    }

    #endregion

    #region I2DPlotItemStyle Members

    public bool IsColorProvider
    {
      get { return _colorProvider >= 0; }
    }

    public bool IsXYLineStyleSupported
    {
      get { return this.XYLineStyle != null; }
    }

    public XYPlotLineStyle XYLineStyle
    {
      get { return this.XYLineStyle; }
    }

    public bool IsXYScatterStyleSupported
    {
      get { return this.XYScatterStyle != null; }
    }

    public XYPlotScatterStyle XYScatterStyle
    {
      get { return this.XYScatterStyle; }
    }

    #endregion
}
}
