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
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Altaxo.Graph.Gdi.Plot.Styles
{
  using Graph.Plot.Groups;
  using Graph.Plot.Data;

  using Plot.Groups;
  using Plot.Data;

  public class BarGraphPlotStyle : IG2DPlotStyle
  {
    /// <summary>
    /// Relative gap between the bars belonging to the same x-value (relative to the width of a single bar).
    /// A value of 0.5 means that the gap has half of the width of one bar.
    /// </summary>
    double _relInnerGapWidth = 0.5;
    /// <summary>
    /// Relative gap between the bars between two consecutive x-values (relative to the width of a single bar).
    /// A value of 1 means that the gap has the same width than one bar.
    /// </summary>
    double _relOuterGapWidth = 1.0;

    /// <summary>
    /// Indicates wether the fill color is dependent (can be set by the ColorGroupStyle) or not.
    /// </summary>
    bool _independentColor;


    /// <summary>
    /// Brush to fill the bar.
    /// </summary>
    BrushX _fillBrush = new BrushX(Color.Red);
    
    /// <summary>
    /// Pen used to frame the bar. Can be null.
    /// </summary>
    PenX _framePen;

    /// <summary>
    /// Indicates whether _baseValue is a physical value or a logical value.
    /// </summary>
    bool _usePhysicalBaseValue;

    /// <summary>
    /// The y-value where the item normally starts. This is either a logical value (_usePhysicalBaseValue==false) or a physical value.
    /// </summary>
    Altaxo.Data.AltaxoVariant _baseValue = new Altaxo.Data.AltaxoVariant(0.0);

    /// <summary>
    /// If true, the bar starts at the y value of the previous plot item.
    /// </summary>
    bool _startAtPreviousItem;
    /// <summary>
    /// Value in logical units, indicating the gap between previous item an this item.
    /// </summary>
    double _previousItemYGap;

    

    /// <summary>
    /// Actual width of the item in logical coordinates.
    /// </summary>
    double _width;
    /// <summary>
    /// Actual position of the item in logical coordinates relative to the logical x coordinate of the item's point.
    /// </summary>
    double _position;


    [NonSerialized]
    object _parent;

    [field:NonSerialized]
    public event EventHandler Changed;


    #region Serialization
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(BarGraphPlotStyle), 0)]
    class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        BarGraphPlotStyle s = (BarGraphPlotStyle)obj;
        info.AddValue("InnerGapWidth", s._relInnerGapWidth);
        info.AddValue("OuterGapWidth", s._relOuterGapWidth);
        info.AddValue("IndependentColor", s._independentColor);
        info.AddValue("FillBrush", s._fillBrush);
        info.AddValue("FramePen", s._framePen);
        info.AddValue("UsePhysicalBaseValue", s._usePhysicalBaseValue);
        info.AddValue("BaseValue", (object)s._baseValue);
        info.AddValue("StartAtPrevious", s._startAtPreviousItem);
        info.AddValue("PreviousItemGap", s._previousItemYGap);
        info.AddValue("ActualWidth", s._width);
        info.AddValue("ActaulPosition", s._position);
      }
      protected virtual BarGraphPlotStyle SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        BarGraphPlotStyle s = null != o ? (BarGraphPlotStyle)o : new BarGraphPlotStyle();

        s._relInnerGapWidth = info.GetDouble("InnerGapWidth");
        s._relOuterGapWidth = info.GetDouble("OuterGapWidth");
        s._independentColor = info.GetBoolean("IndependentColor");
        s.FillBrush = (BrushX)info.GetValue("FillBrush", s);
        s.FramePen = (PenX)info.GetValue("FramePen", s);
        s._usePhysicalBaseValue = info.GetBoolean("UsePhysicalBaseValue");
        s._baseValue = (Altaxo.Data.AltaxoVariant)info.GetValue("BaseValue", s);
        s._startAtPreviousItem = info.GetBoolean("StartAtPrevious");
        s._previousItemYGap = info.GetDouble("PreviousItemGap");
        s._width = info.GetDouble("ActualWidth");
        s._position = info.GetDouble("ActualPosition");

        return s;
      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        BarGraphPlotStyle s = SDeserialize(o, info, parent);

        return s;
      }

    }

    #endregion


    public BarGraphPlotStyle()
    {
    }

    public BarGraphPlotStyle(BarGraphPlotStyle from)
    {
      this._relInnerGapWidth = from._relInnerGapWidth;
      this._relOuterGapWidth = from._relOuterGapWidth;
      this._width = from._width;
      this._position = from._position;
      this._independentColor = from._independentColor;
      this._fillBrush = from._fillBrush.Clone();
      this._framePen = from._framePen == null ? null : (PenX)from._framePen.Clone();
      this._startAtPreviousItem = from._startAtPreviousItem;
      this._previousItemYGap = from._previousItemYGap;
      this._usePhysicalBaseValue = from._usePhysicalBaseValue;
      this._baseValue = from._baseValue;

      this._parent = from._parent;
    }

    public bool IsColorReceiver
    {
      get { return !this._independentColor; }
    }

    public bool IndependentColor
    {
      get
      {
        return _independentColor;
      }
      set
      {
        _independentColor = value;
      }
    }

    public BrushX FillBrush
    {
      get { return _fillBrush; }
      set
      {
        if (value == null)
          throw new ArgumentNullException();

        _fillBrush = value;
      }
    }

    public PenX FramePen
    {
      get { return _framePen; }
      set
      {
        _framePen = value;
      }
    }


    public double InnerGap
    {
      get { return _relInnerGapWidth; }
      set { _relInnerGapWidth = value; }
    }

    public double OuterGap
    {
      get { return _relOuterGapWidth; }
      set { _relOuterGapWidth = value; }
    }

    public double PreviousItemYGap
    {
      get { return _previousItemYGap; }
      set { _previousItemYGap = value; }
    }

    public bool StartAtPreviousItem
    {
      get { return _startAtPreviousItem; }
      set { _startAtPreviousItem = value; }
    }

    public bool UsePhysicalBaseValue
    {
      get { return _usePhysicalBaseValue; }
      set { _usePhysicalBaseValue = value; }
    }

    public Altaxo.Data.AltaxoVariant BaseValue
    {
      get { return _baseValue; }
      set { _baseValue = value; }
    }

    #region IG2DPlotStyle Members

    public void CollectExternalGroupStyles(PlotGroupStyleCollection externalGroups)
    {
      BarWidthPositionGroupStyle.AddExternalGroupStyle(externalGroups);
    }
 
    public void CollectLocalGroupStyles(PlotGroupStyleCollection externalGroups, PlotGroupStyleCollection localGroups)
    {
      BarWidthPositionGroupStyle.AddLocalGroupStyle(externalGroups, localGroups);
    }

    public void PrepareGroupStyles(PlotGroupStyleCollection externalGroups, PlotGroupStyleCollection localGroups, IPlotArea layer, Processed2DPlotData pdata)
    {
      // first, we have to calculate the span of logical values from the minimum logical value to the maximum logical value
      int numberOfItems = 0;
      double minLogical = double.MaxValue;
      double maxLogical = double.MinValue;

      foreach (int originalRowIndex in pdata.RangeList.OriginalRowIndices())
      {
        double logicalX = layer.XAxis.PhysicalVariantToNormal(pdata.GetXPhysical(originalRowIndex));
        numberOfItems++;
        if (logicalX < minLogical)
          minLogical = logicalX;
        if (logicalX > maxLogical)
          maxLogical = logicalX;
      }



      BarWidthPositionGroupStyle.IntendToApply(externalGroups, localGroups,numberOfItems,minLogical,maxLogical);
      BarWidthPositionGroupStyle bwp = PlotGroupStyle.GetStyleToInitialize<BarWidthPositionGroupStyle>(externalGroups, localGroups);
      if (null != bwp)
        bwp.Initialize(_relInnerGapWidth, _relOuterGapWidth);

      if (this.IsColorReceiver)
        ColorGroupStyle.PrepareStyle(externalGroups, localGroups, delegate() { return PlotColors.Colors.GetPlotColor(this._fillBrush.Color); });

    }

    public void ApplyGroupStyles(PlotGroupStyleCollection externalGroups, PlotGroupStyleCollection localGroups)
    {

      BarWidthPositionGroupStyle bwp = PlotGroupStyle.GetStyleToApply<BarWidthPositionGroupStyle>(externalGroups, localGroups);
      if (null != bwp)
        bwp.Apply(out _relInnerGapWidth, out _relOuterGapWidth, out _width, out _position);

      if (this.IsColorReceiver)
        ColorGroupStyle.ApplyStyle(externalGroups, localGroups, delegate(PlotColor c) { this._fillBrush.Color = c; });


    }

    public void Paint(System.Drawing.Graphics g, IPlotArea layer, Processed2DPlotData pdata)
    {
      PlotRangeList rangeList = pdata.RangeList;
      System.Drawing.PointF[] ptArray = pdata.PlotPointsInAbsoluteLayerCoordinates;

      // paint the drop style


      double xleft, xright, ytop, ybottom;
      layer.CoordinateSystem.LogicalToLayerCoordinates(new Logical3D(0, 0), out xleft, out ybottom);
      layer.CoordinateSystem.LogicalToLayerCoordinates(new Logical3D(1, 1), out xright, out ytop);
      float xe = (float)xright;
      float ye = (float)ybottom;

      GraphicsPath path = new GraphicsPath();

      double globalBaseValue;
      if (_usePhysicalBaseValue)
      {
        globalBaseValue = layer.YAxis.PhysicalVariantToNormal(_baseValue);
        if (double.IsNaN(globalBaseValue))
          globalBaseValue = 0;
      }
      else
      {
        globalBaseValue = _baseValue.ToDouble();
      }


      int j=-1;
      foreach(int originalRowIndex in pdata.RangeList.OriginalRowIndices())
      {
        j++;

        double xcn = layer.XAxis.PhysicalVariantToNormal(pdata.GetXPhysical(originalRowIndex));
        double xln = xcn + _position;
        double xrn = xln + _width;

        double ycn = layer.YAxis.PhysicalVariantToNormal(pdata.GetYPhysical(originalRowIndex));
        double ynbase = globalBaseValue;

        if (_startAtPreviousItem && pdata.PreviousItemData!=null)
        {
          double prevstart = layer.YAxis.PhysicalVariantToNormal(pdata.PreviousItemData.GetYPhysical(originalRowIndex));
          if (!double.IsNaN(prevstart))
          {
            ynbase = prevstart;
            ynbase += Math.Sign(ynbase - globalBaseValue) * _previousItemYGap;
          }
        }


        path.Reset();
        layer.CoordinateSystem.GetIsoline(path, new Logical3D(xln, ynbase), new Logical3D(xln, ycn));
        layer.CoordinateSystem.GetIsoline(path, new Logical3D(xln, ycn), new Logical3D(xrn, ycn));
        layer.CoordinateSystem.GetIsoline(path, new Logical3D(xrn, ycn), new Logical3D(xrn, ynbase));
        layer.CoordinateSystem.GetIsoline(path, new Logical3D(xrn, ynbase), new Logical3D(xln, ynbase));
        path.CloseFigure();

        _fillBrush.Rectangle = path.GetBounds();
        g.FillPath(_fillBrush, path);

        if (_framePen != null)
        {
          _framePen.BrushRectangle = path.GetBounds();
          g.DrawPath(_framePen, path);
        }
      }




    }

    public RectangleF PaintSymbol(Graphics g, RectangleF bounds)
    {
      bounds.Inflate(0, -bounds.Height / 4);
      _fillBrush.Rectangle = bounds;
      g.FillRectangle(_fillBrush, bounds);
      return bounds;
    }

    #endregion

    protected virtual void OnChanged()
    {
      if (_parent is Main.IChildChangedEventSink)
        ((Main.IChildChangedEventSink)_parent).EhChildChanged(this, EventArgs.Empty);

      if (Changed != null)
        Changed(this, EventArgs.Empty);
    }


    #region ICloneable Members

    public object Clone()
    {
      return new BarGraphPlotStyle(this);
    }

    #endregion

 
 
 
    #region IDocumentNode Members

    public object ParentObject
    {
      get { return _parent; }
      set { _parent = value; }
    }

    public string Name
    {
      get { return "BarGraphStyle"; }
    }

    #endregion
  }
}
