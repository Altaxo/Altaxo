using System;
using System.Collections.Generic;
using System.Text;
using Altaxo.Data;
using Altaxo.Graph.Scales.Boundaries;

namespace Altaxo.Graph.Gdi.Plot.Groups
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
      CoordinateTransformingStyleBase.MergeXBoundsInto(pb, coll);
    }

    public void MergeYBoundsInto(IPlotArea layer, IPhysicalBoundaries pb, PlotItemCollection coll)
    {
      Dictionary<G2DPlotItem, Processed2DPlotData> plotDataList;
      IPhysicalBoundaries pbclone = (IPhysicalBoundaries)pb.Clone(); // before we can use CanUseStyle, we have to give physical y boundaries template
      CoordinateTransformingStyleBase.MergeYBoundsInto(pbclone, coll);
      if (!CanUseStyle(layer, coll, out plotDataList))
      {
        pb.Add(pbclone);
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

          // Note: we can not use AddUp function here, since
          // when we have positive/negative items, the intermediate bounds
          // might be wider than the bounds of the end result

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


    public static AltaxoVariant[] AddUp(AltaxoVariant[] yArray, Processed2DPlotData pdata)
    {
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
      return yArray;
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
      // First, add up all items since we start always with the last item
      int idx = -1;
      Processed2DPlotData previousItemData = null;
      foreach (IGPlotItem pi in coll)
       {
         
        if (pi is G2DPlotItem)
        {
          idx++;

          G2DPlotItem gpi = pi as G2DPlotItem;
          Processed2DPlotData pdata = plotDataDict[gpi];
          yArray = AddUp(yArray, pdata);
        
          if(idx>0) // this is not the first item
          {
            int j = -1;
            foreach (int originalIndex in pdata.RangeList.OriginalRowIndices())
            {
              j++;
              Logical3D rel = new Logical3D(
              layer.XAxis.PhysicalVariantToNormal(pdata.GetXPhysical(originalIndex)),
              layer.YAxis.PhysicalVariantToNormal(yArray[j]));

              double xabs, yabs;
              layer.CoordinateSystem.LogicalToLayerCoordinates(rel, out xabs, out yabs);
              pdata.PlotPointsInAbsoluteLayerCoordinates[j] = new System.Drawing.PointF((float)xabs, (float)yabs);
            }
          }

          // we have also to exchange the accessor for the physical y value and replace it by our own one
          AltaxoVariant[] localArray = (AltaxoVariant[])yArray.Clone();
          pdata.YPhysicalAccessor = new IndexedPhysicalValueAccessor(delegate(int i) { return localArray[i]; });
          pdata.PreviousItemData = previousItemData;
          previousItemData = pdata;
        }
      }

      for (int i = coll.Count - 1; i >= 0; --i)
      {
        IGPlotItem pi = coll[i];
        if (pi is G2DPlotItem)
        {
          G2DPlotItem gpi = pi as G2DPlotItem;
          Processed2DPlotData pdata = plotDataDict[gpi];
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
