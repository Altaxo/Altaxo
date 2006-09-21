using System;
using System.Collections.Generic;
using System.Text;
using Altaxo.Data;
using Altaxo.Graph.Scales.Boundaries;

namespace Altaxo.Graph.PlotGroups
{
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
      CoordinateTransformingStyleBase.MergeXBoundsInto(layer, pb, coll);
    }

    public void MergeYBoundsInto(IPlotArea layer, IPhysicalBoundaries pb, PlotItemCollection coll)
    {
      Dictionary<G2DPlotItem, Processed2DPlotData> plotDataList;
      if (!CanUseStyle(layer, coll, out plotDataList))
      {
        CoordinateTransformingStyleBase.MergeXBoundsInto(layer, pb, coll);
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

      AltaxoVariant[] yArray = null;
      int idx = -1;
      foreach (IGPlotItem pi in coll)
      {
        if (pi is G2DPlotItem)
        {
          idx++;

          G2DPlotItem gpi = pi as G2DPlotItem;
          Processed2DPlotData pdata = plotDataDict[gpi];


          if (yArray == null)
          {
            yArray = new AltaxoVariant[pdata.RangeList.PlotPointCount];

            int j = -1;
            foreach (int originalIndex in pdata.RangeList.OriginalRowIndices())
            {
              j++;
              yArray[j] = pdata.GetYPhysical(originalIndex);
            }
          }
          else // this is not the first item
          {
            int j = -1;
            foreach (int originalIndex in pdata.RangeList.OriginalRowIndices())
            {
              j++;
              yArray[j] += pdata.GetYPhysical(originalIndex);
            }
          }
        }
      }


      // now plot the data - the summed up y is in yArray
      foreach (IGPlotItem pi in coll)
      {
        if (pi is G2DPlotItem)
        {
          idx++;

          G2DPlotItem gpi = pi as G2DPlotItem;
          Processed2DPlotData pdata = plotDataDict[gpi];



          int j = -1;
          foreach (int originalIndex in pdata.RangeList.OriginalRowIndices())
          {
            j++;


            AltaxoVariant y = 100*(pdata.GetYPhysical(originalIndex) / yArray[j]);
            double yrel = layer.YAxis.PhysicalVariantToNormal(y);
            double xrel = layer.XAxis.PhysicalVariantToNormal(pdata.GetXPhysical(originalIndex));

            double xabs, yabs;
            layer.CoordinateSystem.LogicalToLayerCoordinates(xrel, yrel, out xabs, out yabs);
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
