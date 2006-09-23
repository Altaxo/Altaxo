using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Altaxo.Graph.G2D.Plot.Styles
{
  using PlotGroups;
  using Plot.Groups;
  using Plot.Data;

  public class BarGraphPlotStyle : IG2DPlotStyle
  {
    double _relGapWidth = 0.5;
    double _relBoundsWidth = 1.0;
    double _width;
    double _position;

    BrushHolder _fillBrush = new BrushHolder(Color.Red);
    object _parent;



    public BarGraphPlotStyle()
    {
    }

    public BarGraphPlotStyle(BarGraphPlotStyle from)
    {
      this._relGapWidth = from._relGapWidth;
      this._relBoundsWidth = from._relBoundsWidth;
      this._width = from._width;
      this._position = from._position;
    }


    #region IG2DPlotStyle Members

 
    public void AddLocalGroupStyles(PlotGroupStyleCollection externalGroups, PlotGroupStyleCollection localGroups)
    {
      BarWidthPositionGroupStyle.AddLocalGroupStyle(externalGroups, localGroups);
    }

    public void PrepareGroupStyles(PlotGroupStyleCollection externalGroups, PlotGroupStyleCollection localGroups)
    {
      BarWidthPositionGroupStyle.IntendToApply(externalGroups, localGroups);
      BarWidthPositionGroupStyle bwp = PlotGroupStyle.GetStyleToInitialize<BarWidthPositionGroupStyle>(externalGroups, localGroups);
      if (null != bwp)
        bwp.Initialize(_relGapWidth, _relBoundsWidth);
    }

    public void ApplyGroupStyles(PlotGroupStyleCollection externalGroups, PlotGroupStyleCollection localGroups)
    {

      BarWidthPositionGroupStyle bwp = PlotGroupStyle.GetStyleToApply<BarWidthPositionGroupStyle>(externalGroups, localGroups);
      if (null != bwp)
        bwp.Apply(out _relGapWidth, out _relBoundsWidth, out _width, out _position);

    }

    public void Paint(System.Drawing.Graphics g, IPlotArea layer, Processed2DPlotData pdata)
    {
      PlotRangeList rangeList = pdata.RangeList;
      System.Drawing.PointF[] ptArray = pdata.PlotPointsInAbsoluteLayerCoordinates;

      // paint the drop style


      double xleft, xright, ytop, ybottom;
      layer.CoordinateSystem.LogicalToLayerCoordinates(0, 0, out xleft, out ybottom);
      layer.CoordinateSystem.LogicalToLayerCoordinates(1, 1, out xright, out ytop);
      float xe = (float)xright;
      float ye = (float)ybottom;


      int j=-1;
      foreach(int originalRowIndex in pdata.RangeList.OriginalRowIndices())
      {
        j++;

        double x = (double)pdata.GetXPhysical(originalRowIndex);

        double xlp = x + _position;
        double xrp = xlp + _width;
        double xln = layer.XAxis.PhysicalVariantToNormal(xlp);
        double xrn = layer.XAxis.PhysicalVariantToNormal(xrp);

        double xla, xra;
        layer.CoordinateSystem.LogicalToLayerCoordinates(xln, 1, out xla, out ybottom);
        layer.CoordinateSystem.LogicalToLayerCoordinates(xrn, 1, out xra, out ybottom);


        float y = ptArray[j].Y;
        g.FillRectangle(Brushes.Red, (float)xla, (float)y, (float)(xra - xla), (float)(ye - y));
      }




    }

    public RectangleF PaintSymbol(Graphics g, RectangleF bounds)
    {
      return bounds;
    }

    #endregion

    #region ICloneable Members

    public object Clone()
    {
      return new BarGraphPlotStyle(this);
    }

    #endregion

    #region IChangedEventSource Members

    public event EventHandler Changed;

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
