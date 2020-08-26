#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2016 Dr. Dirk Lellinger
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

#nullable enable
using System;
using System.Collections.Generic;
using System.Text;
using Altaxo.Data;
using Altaxo.Graph.Scales.Boundaries;

namespace Altaxo.Graph.Graph3D.Plot.Groups
{
  using Geometry;
  using GraphicsContext;
  using Plot.Data;

  public class RelativeStackTransform
    :
    Main.SuspendableDocumentLeafNodeWithEventArgs,
    ICoordinateTransformingGroupStyle
  {
    #region Serialization

    /// <summary>Initial version 2016-09-06.</summary>
    /// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(RelativeStackTransform), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (RelativeStackTransform)obj;
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
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
      CoordinateTransformingStyleBase.MergeYBoundsInto(pb, coll);
    }

    public void MergeZBoundsInto(IPlotArea layer, IPhysicalBoundaries pb, PlotItemCollection coll)
    {
      var pbclone = (IPhysicalBoundaries)pb.Clone(); // before we can use CanUseStyle, we have to give physical y boundaries template
      CoordinateTransformingStyleBase.MergeZBoundsInto(pbclone, coll);
      if (!CanUseStyle(layer, coll, out var plotDataList))
      {
        pb.Add(pbclone);
        return;
      }

      pb.Add(0);
      pb.Add(100);
    }

    private static bool CanUseStyle(IPlotArea layer, PlotItemCollection coll, out Dictionary<G3DPlotItem, Processed3DPlotData> plotDataList)
    {
      return AbsoluteStackTransform.CanUseStyle(layer, coll, out plotDataList);
    }

    public void PaintPreprocessing(IPaintContext paintContext, IPlotArea layer, PlotItemCollection coll)
    {
      if (!CanUseStyle(layer, coll, out var plotDataDict))
      {
        return;
      }
      else
      {
        paintContext.AddValue(this, plotDataDict);
      }

      AltaxoVariant[]? vSumArray = null;
      foreach (IGPlotItem pi in coll)
      {
        if (pi is G3DPlotItem gpi)
        {
          var pdata = plotDataDict[gpi];
          vSumArray = AbsoluteStackTransform.AddUp(vSumArray, pdata);
        }
      }

      if (vSumArray is null)
        return;

      // now plot the data - the summed up y is in yArray
      AltaxoVariant[]? vArray = null;
      Processed3DPlotData? previousItemData = null;
      foreach (IGPlotItem pi in coll)
      {
        if (pi is G3DPlotItem gpi)
        {
          var pdata = plotDataDict[gpi];
          if (pdata is null || pdata.RangeList is null || pdata.PlotPointsInAbsoluteLayerCoordinates is null)
            continue;

          vArray = AbsoluteStackTransform.AddUp(vArray, pdata);
          var localArray = new AltaxoVariant[vArray.Length];

          int j = -1;
          foreach (int originalIndex in pdata.RangeList.OriginalRowIndices())
          {
            j++;
            AltaxoVariant v = 100 * vArray[j] / vSumArray[j];
            localArray[j] = v;

            var rel = new Logical3D(
            layer.XAxis.PhysicalVariantToNormal(pdata.GetXPhysical(originalIndex)),
            layer.YAxis.PhysicalVariantToNormal(pdata.GetYPhysical(originalIndex)),
            layer.ZAxis.PhysicalVariantToNormal(v));

            layer.CoordinateSystem.LogicalToLayerCoordinates(rel, out var pabs);
            pdata.PlotPointsInAbsoluteLayerCoordinates[j] = pabs;
          }
          // we have also to exchange the accessor for the physical z value and replace it by our own one
          pdata.ZPhysicalAccessor = new IndexedPhysicalValueAccessor(delegate (int i)
          { return localArray[i]; });
          pdata.PreviousItemData = previousItemData;
          previousItemData = pdata;
        }
      }
    }

    public void PaintPostprocessing()
    {
    }

    public void PaintChild(IGraphicsContext3D g, IPaintContext paintContext, IPlotArea layer, PlotItemCollection coll, int indexOfChild)
    {
      var plotDataDict = paintContext.GetValueOrDefault<Dictionary<G3DPlotItem, Processed3DPlotData>>(this);
      if (null == plotDataDict) // if initializing this dict was not successfull, then make a normal plot
      {
        coll[indexOfChild].Paint(g, paintContext, layer, indexOfChild == coll.Count - 1 ? null : coll[indexOfChild + 1], indexOfChild == 0 ? null : coll[indexOfChild - 1]);
        return;
      }

      Processed3DPlotData? prevPlotData = null;
      Processed3DPlotData? nextPlotData = null;

      if ((indexOfChild + 1) < coll.Count && (coll[indexOfChild + 1] is G3DPlotItem keyP1))
        prevPlotData = plotDataDict[keyP1];

      if (indexOfChild > 0 && (coll[indexOfChild - 1] is G3DPlotItem keyM1))
        nextPlotData = plotDataDict[keyM1];

      if (coll[indexOfChild] is G3DPlotItem gpi)
      {
        gpi.Paint(g, layer, plotDataDict[gpi], prevPlotData, nextPlotData);
      }
      else
      {
        coll[indexOfChild].Paint(g, paintContext, layer, null, null);
      }
    }

    #endregion ICoordinateTransformingGroupStyle Members

    #region ICloneable Members

    public object Clone()
    {
      return new RelativeStackTransform(this);
    }

    #endregion ICloneable Members
  }
}
