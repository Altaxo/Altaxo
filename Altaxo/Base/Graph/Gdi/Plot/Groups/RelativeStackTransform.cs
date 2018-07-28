#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
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

#endregion Copyright

using Altaxo.Data;
using Altaxo.Graph.Scales.Boundaries;
using System;
using System.Collections.Generic;
using System.Text;

namespace Altaxo.Graph.Gdi.Plot.Groups
{
  using Plot.Data;

  public class RelativeStackTransform
    :
    Main.SuspendableDocumentLeafNodeWithEventArgs,
    ICoordinateTransformingGroupStyle
  {
    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(RelativeStackTransform), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        RelativeStackTransform s = (RelativeStackTransform)obj;
      }

      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        RelativeStackTransform s = null != o ? (RelativeStackTransform)o : new RelativeStackTransform();
        return s;
      }
    }

    #endregion Serialization

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

    private static bool CanUseStyle(IPlotArea layer, PlotItemCollection coll, out Dictionary<G2DPlotItem, Processed2DPlotData> plotDataList)
    {
      return AbsoluteStackTransform.CanUseStyle(layer, coll, out plotDataList);
    }

    public void PaintPreprocessing(System.Drawing.Graphics g, IPaintContext paintContext, IPlotArea layer, PlotItemCollection coll)
    {
      Dictionary<G2DPlotItem, Processed2DPlotData> plotDataDict = null;
      if (!CanUseStyle(layer, coll, out plotDataDict))
      {
        return;
      }
      else
      {
        paintContext.AddValue(this, plotDataDict);
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
            AltaxoVariant y = 100 * yArray[j] / ysumArray[j];
            localArray[j] = y;

            Logical3D rel = new Logical3D(
            layer.XAxis.PhysicalVariantToNormal(pdata.GetXPhysical(originalIndex)),
            layer.YAxis.PhysicalVariantToNormal(y));

            double xabs, yabs;
            layer.CoordinateSystem.LogicalToLayerCoordinates(rel, out xabs, out yabs);
            pdata.PlotPointsInAbsoluteLayerCoordinates[j] = new System.Drawing.PointF((float)xabs, (float)yabs);
          }
          // we have also to exchange the accessor for the physical y value and replace it by our own one
          pdata.YPhysicalAccessor = new IndexedPhysicalValueAccessor(delegate (int i)
          { return localArray[i]; });
          pdata.PreviousItemData = previousItemData;
          previousItemData = pdata;
        }
      }
    }

    public void PaintPostprocessing()
    {
    }

    public void PaintChild(System.Drawing.Graphics g, IPaintContext paintContext, IPlotArea layer, PlotItemCollection coll, int indexOfChild)
    {
      var plotDataDict = paintContext.GetValueOrDefault<Dictionary<G2DPlotItem, Processed2DPlotData>>(this);
      if (null == plotDataDict) // if initializing this dict was not successfull, then make a normal plot
      {
        coll[indexOfChild].Paint(g, paintContext, layer, indexOfChild == coll.Count - 1 ? null : coll[indexOfChild + 1], indexOfChild == 0 ? null : coll[indexOfChild - 1]);
        return;
      }

      Processed2DPlotData prevPlotData = null;
      Processed2DPlotData nextPlotData = null;

      if ((indexOfChild + 1) < coll.Count && (coll[indexOfChild + 1] is G2DPlotItem))
        prevPlotData = plotDataDict[coll[indexOfChild + 1] as G2DPlotItem];

      if (indexOfChild > 0 && (coll[indexOfChild - 1] is G2DPlotItem))
        nextPlotData = plotDataDict[coll[indexOfChild - 1] as G2DPlotItem];

      if (coll[indexOfChild] is G2DPlotItem)
      {
        var gpi = coll[indexOfChild] as G2DPlotItem;
        gpi.Paint(g, layer, plotDataDict[gpi], prevPlotData, nextPlotData);
      }
      else
      {
        coll[indexOfChild].Paint(g, paintContext, layer, null, null);
      }
    }

    /*
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
						AltaxoVariant y = 100 * yArray[j] / ysumArray[j];
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
			Processed2DPlotData prevPlotData = null;
			Processed2DPlotData nextPlotData = null;
			Processed2DPlotData currPlotData = null;
			for (int i = coll.Count - 1; i >= 0; --i)
			{
				if (i > 0 && (coll[i - 1] is G2DPlotItem))
					nextPlotData = plotDataDict[coll[i - 1] as G2DPlotItem];
				else
					nextPlotData = null;

				if (coll[i] is G2DPlotItem)
				{
					var gpi = coll[i] as G2DPlotItem;
					currPlotData = plotDataDict[gpi];
					gpi.Paint(g, layer, currPlotData, prevPlotData, nextPlotData);
				}
				else
				{
					currPlotData = null;
					coll[i].Paint(g, layer, null, null);
				}

				prevPlotData = currPlotData;
				currPlotData = nextPlotData;
				nextPlotData = null;
			}
		}
		*/

    #endregion ICoordinateTransformingGroupStyle Members

    #region ICloneable Members

    public object Clone()
    {
      return new RelativeStackTransform(this);
    }

    #endregion ICloneable Members
  }
}
