using System;
using System.Collections.Generic;
using System.Text;

using Altaxo.Data;
using Altaxo.Graph.Scales.Boundaries;

namespace Altaxo.Graph.PlotGroups
{
  public class WaterfallTransform : ICoordinateTransformingGroupStyle
  {
    double _xinc=0;
    double _yinc=0;

    public WaterfallTransform()
    {
    }

    public WaterfallTransform(WaterfallTransform from)
    {
      this._xinc = from._xinc;
      this._yinc = from._yinc;
    }

    #region ICoordinateTransformingGroupStyle Members

    public void MergeXBoundsInto(IPlotArea layer, IPhysicalBoundaries pb, PlotItemCollection coll)
    {
      if(!(pb is NumericalBoundaries))
      {
        CoordinateTransformingStyleBase.MergeXBoundsInto(layer, pb,coll);
        return;
      }

      NumericalBoundaries xbounds = (NumericalBoundaries)pb.Clone();
      xbounds.Reset();

      int nItems = 0;
      foreach (IGPlotItem pi in coll)
      {
        if (pi is IXBoundsHolder)
        {
          IXBoundsHolder xbpi = (IXBoundsHolder)pi;
          xbpi.MergeXBoundsInto(layer, xbounds);
        }
        if(pi is G2DPlotItem)
          nItems++;

      }

     
      if (nItems == 0)
        _xinc = 0;
      else
        _xinc = (xbounds.UpperBound - xbounds.LowerBound) / nItems;

      int idx = 0;
      foreach (IGPlotItem pi in coll)
      {
        if (pi is IXBoundsHolder)
        {
          IXBoundsHolder xbpi = (IXBoundsHolder)pi;
          xbounds.Reset();
          xbpi.MergeXBoundsInto(layer, xbounds);
          xbounds.Shift(_xinc * idx);
          pb.Add(xbounds);
        }
        if (pi is G2DPlotItem)
          idx++;
      }

    }

    public void MergeYBoundsInto(IPlotArea layer, IPhysicalBoundaries pb, PlotItemCollection coll)
    {
      throw new Exception("The method or operation is not implemented.");
    }


    public void Paint(System.Drawing.Graphics g, IPlotArea layer, PlotItemCollection coll)
    {
     

      int idx = -1;
      foreach (IGPlotItem pi in coll)
      {
        if (pi is G2DPlotItem)
        {
          idx++;
          double currxinc = idx * _xinc;
          double curryinc = idx * _yinc;

          G2DPlotItem gpi = pi as G2DPlotItem;
          Processed2DPlotData plotdata = gpi.GetRangesAndPoints(layer);

          int j = -1;
          foreach (int rowIndex in plotdata.RangeList.OriginalRowIndices())
          {
            j++;

            AltaxoVariant xx = plotdata.GetXPhysical(rowIndex) + currxinc;
            AltaxoVariant yy = plotdata.GetYPhysical(rowIndex) + curryinc;

            double xrel = layer.XAxis.PhysicalVariantToNormal(xx);
            double yrel = layer.YAxis.PhysicalVariantToNormal(yy);

            double xabs, yabs;
            layer.CoordinateSystem.LogicalToLayerCoordinates(xrel, yrel, out xabs, out yabs);
            plotdata.PlotPointsInAbsoluteLayerCoordinates[j] = new System.Drawing.PointF((float)xabs, (float)yabs);

            gpi.Paint(g, layer, plotdata);
          }
        }
        else
        {
          pi.Paint(g, layer);
        }
      }
    }

    #endregion

    #region ICloneable Members

    public object Clone()
    {
      return new WaterfallTransform(this);
    }

    #endregion

    #region ICoordinateTransformingGroupStyle Members


   
    #endregion
  }
}
