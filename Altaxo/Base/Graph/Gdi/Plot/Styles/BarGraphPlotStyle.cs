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
    /// Relative gap between the bars belonging to the same x-value.
    /// A value of 0.5 means that the gap has half of the width of one bar.
    /// </summary>
    double _relInnerGapWidth = 0.5;
    /// <summary>
    /// Relative gap between the bars between two consecutive x-values.
    /// A value of 1 means that the gap has the same width than one bar.
    /// </summary>
    double _relOuterGapWidth = 1.0;


    double _width;
    double _position;

    BrushX _fillBrush = new BrushX(Color.Red);
    
    [NonSerialized]
    object _parent;

    [field:NonSerialized]
    public event EventHandler Changed;



    public BarGraphPlotStyle()
    {
    }

    public BarGraphPlotStyle(BarGraphPlotStyle from)
    {
      this._relInnerGapWidth = from._relInnerGapWidth;
      this._relOuterGapWidth = from._relOuterGapWidth;
      this._width = from._width;
      this._position = from._position;

      this._parent = from._parent;
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
    }

    public void ApplyGroupStyles(PlotGroupStyleCollection externalGroups, PlotGroupStyleCollection localGroups)
    {

      BarWidthPositionGroupStyle bwp = PlotGroupStyle.GetStyleToApply<BarWidthPositionGroupStyle>(externalGroups, localGroups);
      if (null != bwp)
        bwp.Apply(out _relInnerGapWidth, out _relOuterGapWidth, out _width, out _position);

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

      int j=-1;
      foreach(int originalRowIndex in pdata.RangeList.OriginalRowIndices())
      {
        j++;

        double xcn = layer.XAxis.PhysicalVariantToNormal(pdata.GetXPhysical(originalRowIndex));
        double xln = xcn + _position;
        double xrn = xln + _width;

        double ycn = layer.YAxis.PhysicalVariantToNormal(pdata.GetYPhysical(originalRowIndex));
        double ynbase = 0;


        path.Reset();
        layer.CoordinateSystem.GetIsoline(path, new Logical3D(xln, ynbase), new Logical3D(xln, ycn));
        layer.CoordinateSystem.GetIsoline(path, new Logical3D(xln, ycn), new Logical3D(xrn, ycn));
        layer.CoordinateSystem.GetIsoline(path, new Logical3D(xrn, ycn), new Logical3D(xrn, ynbase));
        layer.CoordinateSystem.GetIsoline(path, new Logical3D(xrn, ynbase), new Logical3D(xln, ynbase));

        g.FillPath(Brushes.Red, path);
      }




    }

    public RectangleF PaintSymbol(Graphics g, RectangleF bounds)
    {
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
