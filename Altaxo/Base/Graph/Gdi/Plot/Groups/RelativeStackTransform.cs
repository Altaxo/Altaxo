using System;
using System.Collections.Generic;
using System.Text;
using Altaxo.Data;
using Altaxo.Graph.Scales.Boundaries;

namespace Altaxo.Graph.Gdi.Plot.Groups
{
  using Plot.Data;

  public class RelativeStackTransform : ICoordinateTransformingGroupStyle
  {
    public RelativeStackTransform()
    {
    }

    public RelativeStackTransform(RelativeStackTransform from)
    {
    }

    #region ICoordinateTransformingGroupStyle Members

    public void MergeXBoundsInto(IPlotArea layer, IPhysicalBoundaries pb, PlotItemCollection coll)
    {
      CoordinateTransformingStyleBase.MergeXBoundsInto(pb, coll);
    }

    public void MergeYBoundsInto(IPlotArea layer, IPhysicalBoundaries pb, PlotItemCollection coll)
    {
      Dictionary<G2DPlotItem, Processed2DPlotData> plotDataList;
      if (!CanUseStyle(layer, coll, out plotDataList))
      {
        CoordinateTransformingStyleBase.MergeXBoundsInto(pb, coll);
        return;
      }

      pb.Add(0);
      pb.Add(100);
    }



    static bool CanUseStyle(IPlotArea layer, PlotItemCollection coll, out Dictionary<G2DPlotItem, Processed2DPlotData> plotDataList)
    {
      return AbsoluteStackTransform.CanUseStyle(layer, coll, out plotDataList);
    }


    public void Paint(System.Drawing.Graphics g, IPlotArea layer, PlotItemCollection coll)
    {
      Dictionary<G2DPlotItem, Processed2DPlotData> plotDataDict;
      if (!CanUseStyle(layer, coll, out plotDataDict))
      {
        CoordinateTransformingStyleBase.Paint(g, layer, coll);
        return;
      }

      AltaxoVariant[] ysumArray = null;
      foreach (IGPlotItem pi in coll)
      {
        if (pi is G2DPlotItem)
        {
          G2DPlotItem gpi = pi as G2DPlotItem;
          Processed2DPlotData pdata = plotDataDict[gpi];
          ysumArray = AbsoluteStackTransform.AddUp(ysumArray, pdata);
        }
      }


     

      // now plot the data - the summed up y is in yArray
      AltaxoVariant[] yArray = null;
      foreach (IGPlotItem pi in coll)
      {
        if (pi is G2DPlotItem)
        {
          G2DPlotItem gpi = pi as G2DPlotItem;
          Processed2DPlotData pdata = plotDataDict[gpi];
          yArray = AbsoluteStackTransform.AddUp(yArray, pdata);

          int j = -1;
          foreach (int originalIndex in pdata.RangeList.OriginalRowIndices())
          {
            j++;
            AltaxoVariant y = 100*yArray[j]/ysumArray[j];

            Logical3D rel = new Logical3D(
            layer.YAxis.PhysicalVariantToNormal(y),
            layer.XAxis.PhysicalVariantToNormal(pdata.GetXPhysical(originalIndex)));

            double xabs, yabs;
            layer.CoordinateSystem.LogicalToLayerCoordinates(rel, out xabs, out yabs);
            pdata.PlotPointsInAbsoluteLayerCoordinates[j] = new System.Drawing.PointF((float)xabs, (float)yabs);

          }
          gpi.Paint(g, layer, pdata);
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
      return new RelativeStackTransform(this);
    }

    #endregion

    #region ICoordinateTransformingGroupStyle Members



    #endregion
  }

}
