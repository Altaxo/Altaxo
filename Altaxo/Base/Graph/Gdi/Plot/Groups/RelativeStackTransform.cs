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
      IPhysicalBoundaries pbclone = (IPhysicalBoundaries)pb.Clone(); // before we can use CanUseStyle, we have to give physical y boundaries template
      CoordinateTransformingStyleBase.MergeYBoundsInto(pbclone, coll);
      if (!CanUseStyle(layer, coll, out plotDataList))
      {
        pb.Add(pbclone);
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
      Processed2DPlotData previousItemData = null;
      foreach (IGPlotItem pi in coll)
      {
        if (pi is G2DPlotItem)
        {
          G2DPlotItem gpi = pi as G2DPlotItem;
          Processed2DPlotData pdata = plotDataDict[gpi];
          yArray = AbsoluteStackTransform.AddUp(yArray, pdata);
          AltaxoVariant[] localArray = new AltaxoVariant[yArray.Length];

          int j = -1;
          foreach (int originalIndex in pdata.RangeList.OriginalRowIndices())
          {
            j++;
            AltaxoVariant y = 100*yArray[j]/ysumArray[j];
            localArray[j] = y;

            Logical3D rel = new Logical3D(
            layer.XAxis.PhysicalVariantToNormal(pdata.GetXPhysical(originalIndex)),
            layer.YAxis.PhysicalVariantToNormal(y));

            double xabs, yabs;
            layer.CoordinateSystem.LogicalToLayerCoordinates(rel, out xabs, out yabs);
            pdata.PlotPointsInAbsoluteLayerCoordinates[j] = new System.Drawing.PointF((float)xabs, (float)yabs);

          }
          // we have also to exchange the accessor for the physical y value and replace it by our own one
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
      return new RelativeStackTransform(this);
    }

    #endregion

    #region ICoordinateTransformingGroupStyle Members



    #endregion
  }

}
