using System;
using System.Collections.Generic;
using System.Text;
using Altaxo.Data;
using Altaxo.Graph.Scales.Boundaries;

namespace Altaxo.Graph.G2D.Plot.Groups
{
  using Plot.Data;

  public class AbsoluteStackTransform : ICoordinateTransformingGroupStyle
  {

    public AbsoluteStackTransform()
    {
    }

    public AbsoluteStackTransform(AbsoluteStackTransform from)
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
        CoordinateTransformingStyleBase.MergeXBoundsInto(layer,pb, coll);
        return;
      }


      AltaxoVariant[] ySumArray = null;

      int idx = -1;
      foreach (IGPlotItem pi in coll)
      {
        if (pi is G2DPlotItem)
        {
          idx++;

          G2DPlotItem gpi = (G2DPlotItem)pi;
          Processed2DPlotData pdata = plotDataList[gpi];

          if (ySumArray == null)
          {
            ySumArray = new AltaxoVariant[pdata.RangeList.PlotPointCount];

            int j = -1;
            foreach (int originalIndex in pdata.RangeList.OriginalRowIndices())
            {
              j++;
              ySumArray[j] = pdata.GetYPhysical(originalIndex);
              pb.Add(ySumArray[j]);
            }
          }
          else // this is not the first item
          {
            int j = -1;
            foreach (int originalIndex in pdata.RangeList.OriginalRowIndices())
            {
              j++;
              ySumArray[j] += pdata.GetYPhysical(originalIndex);
              pb.Add(ySumArray[j]);

            }
          }
        }
      }
    }



    public static bool CanUseStyle(IPlotArea layer, PlotItemCollection coll, out Dictionary<G2DPlotItem, Processed2DPlotData> plotDataList)
    {
      plotDataList = new Dictionary<G2DPlotItem, Processed2DPlotData>();

      AltaxoVariant[] xArray = null;

      int idx = -1;
      foreach (IGPlotItem pi in coll)
      {
        if (pi is G2DPlotItem)
        {
          idx++;
          G2DPlotItem gpi = (G2DPlotItem)pi;
          Processed2DPlotData pdata = gpi.GetRangesAndPoints(layer);
          plotDataList.Add(gpi, pdata);

          if (xArray == null)
          {
            xArray = new AltaxoVariant[pdata.RangeList.PlotPointCount];

            int j = -1;
            foreach (int originalIndex in pdata.RangeList.OriginalRowIndices())
            {
              j++;
              xArray[j] = pdata.GetXPhysical(originalIndex);
            }
          }
          else // this is not the first item
          {
            if (pdata.RangeList.PlotPointCount != xArray.Length)
              return false;


            int j = -1;
            foreach (int originalIndex in pdata.RangeList.OriginalRowIndices())
            {
              j++;


              if (xArray[j] != pdata.GetXPhysical(originalIndex))
                return false;
            }
          }
        }
      }

      return idx >= 1;
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
              double yrel = layer.YAxis.PhysicalVariantToNormal(yArray[j]);
              double xrel = layer.XAxis.PhysicalVariantToNormal(pdata.GetXPhysical(originalIndex));

              double xabs, yabs;
              layer.CoordinateSystem.LogicalToLayerCoordinates(xrel, yrel, out xabs, out yabs);
              pdata.PlotPointsInAbsoluteLayerCoordinates[j] = new System.Drawing.PointF((float)xabs, (float)yabs);


            }
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
      return new AbsoluteStackTransform(this);
    }

    #endregion

    #region ICoordinateTransformingGroupStyle Members



    #endregion
  }

}
