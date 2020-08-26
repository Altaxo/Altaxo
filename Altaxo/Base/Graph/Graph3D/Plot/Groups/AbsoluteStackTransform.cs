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
  using Altaxo.Graph.Plot.Data;
  using Geometry;
  using GraphicsContext;
  using Plot.Data;

  /// <summary>
  ///
  /// </summary>
  public class AbsoluteStackTransform
    :
    Main.SuspendableDocumentLeafNodeWithEventArgs,
    ICoordinateTransformingGroupStyle
  {
    #region Serialization

    /// <summary>Initial version 2016-09-06.</summary>
    /// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(AbsoluteStackTransform), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (AbsoluteStackTransform)obj;
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (AbsoluteStackTransform?)o ?? new AbsoluteStackTransform();
        return s;
      }
    }

    #endregion Serialization

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

      // we put zero into the y-Boundaries, since the addition starts with that value
      pb.Add(new AltaxoVariant(0.0));

      AltaxoVariant[]? vSumArray = null;

      int idx = -1;
      foreach (IGPlotItem pi in coll)
      {
        if (pi is G3DPlotItem gpi)
        {
          idx++;
          Processed3DPlotData pdata = plotDataList[gpi];

          if (pdata is null || pdata.RangeList is null)
            continue;

          // Note: we can not use AddUp function here, since
          // when we have positive/negative items, the intermediate bounds
          // might be wider than the bounds of the end result

          if (vSumArray == null)
          {
            vSumArray = new AltaxoVariant[pdata.RangeList.PlotPointCount];

            int j = -1;
            foreach (int originalIndex in pdata.RangeList.OriginalRowIndices())
            {
              j++;
              vSumArray[j] = pdata.GetZPhysical(originalIndex);
              pb.Add(vSumArray[j]);
            }
          }
          else // this is not the first item
          {
            int j = -1;
            foreach (int originalIndex in pdata.RangeList.OriginalRowIndices())
            {
              j++;
              vSumArray[j] += pdata.GetZPhysical(originalIndex);
              pb.Add(vSumArray[j]);
            }
          }
        }
      }
    }

    /// <summary>
    /// Determines whether the plot items in <paramref name="coll"/> can be plotted as stack. Presumption is that all plot items
    /// have the same number of plot points, and that all items have the same order of x values associated with the plot points.
    /// </summary>
    /// <param name="layer">Plot layer.</param>
    /// <param name="coll">Collection of plot items.</param>
    /// <param name="plotDataList">Output: dictionary with associates each plot item with a list of processed plot data.</param>
    /// <returns>Returns <c>true</c> if the plot items in <paramref name="coll"/> can be plotted as stack; otherwise, <c>false</c>.</returns>
    public static bool CanUseStyle(IPlotArea layer, PlotItemCollection coll, out Dictionary<G3DPlotItem, Processed3DPlotData> plotDataList)
    {
      plotDataList = new Dictionary<G3DPlotItem, Processed3DPlotData>();

      AltaxoVariant[]? xArray = null;
      AltaxoVariant[]? yArray = null;

      int idx = -1;
      foreach (IGPlotItem pi in coll)
      {
        if (pi is G3DPlotItem gpi)
        {
          idx++;
          var pdata = gpi.GetRangesAndPoints(layer);

          if (pdata is null || pdata.RangeList is null)
            continue;

          plotDataList.Add(gpi, pdata);

          if (xArray is null || yArray is null)
          {
            xArray = new AltaxoVariant[pdata.RangeList.PlotPointCount];
            yArray = new AltaxoVariant[pdata.RangeList.PlotPointCount];

            int j = -1;
            foreach (int originalIndex in pdata.RangeList.OriginalRowIndices())
            {
              j++;
              xArray[j] = pdata.GetXPhysical(originalIndex);
              yArray[j] = pdata.GetYPhysical(originalIndex);
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

              if (yArray[j] != pdata.GetYPhysical(originalIndex))
                return false;
            }
          }
        }
      }

      return idx >= 1;
    }

    /// <summary>Adds the y-values of a plot item to an array of y-values..</summary>
    /// <param name="vArray">The y array to be added to. If null, a new array will be allocated (and filled with the y-values of the plot item).</param>
    /// <param name="pdata">The pdata.</param>
    /// <returns>If the parameter <paramref name="vArray"/> was not null, then that <paramref name="vArray"/> is returned. Otherwise the newly allocated array is returned.</returns>
    public static AltaxoVariant[] AddUp(AltaxoVariant[]? vArray, Processed3DPlotData pdata)
    {
      if (pdata is null)
        throw new ArgumentNullException(nameof(pdata));
      if (!(pdata.RangeList is { } rangeList))
        throw new ArgumentNullException(nameof(pdata.RangeList));

      if (vArray is null)
      {
        vArray = new AltaxoVariant[rangeList.PlotPointCount];

        int j = -1;
        foreach (int originalIndex in rangeList.OriginalRowIndices())
        {
          j++;
          vArray[j] = pdata.GetZPhysical(originalIndex);
        }
      }
      else // this is not the first item
      {
        int j = -1;
        foreach (int originalIndex in rangeList.OriginalRowIndices())
        {
          j++;
          vArray[j] += pdata.GetZPhysical(originalIndex);
        }
      }
      return vArray;
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

      AltaxoVariant[]? vArray = null;
      // First, add up all items since we start always with the last item
      int idx = -1;
      Processed3DPlotData? previousItemData = null;
      foreach (IGPlotItem pi in coll)
      {
        if (pi is G3DPlotItem gpi)
        {
          idx++;

          Processed3DPlotData pdata = plotDataDict[gpi];
          if (pdata is null || pdata.RangeList is null || pdata.PlotPointsInAbsoluteLayerCoordinates is null)
            continue;

          vArray = AddUp(vArray, pdata);

          if (idx > 0) // this is not the first item
          {
            int j = -1;
            foreach (int originalIndex in pdata.RangeList.OriginalRowIndices())
            {
              j++;
              var rel = new Logical3D(
              layer.XAxis.PhysicalVariantToNormal(pdata.GetXPhysical(originalIndex)),
              layer.YAxis.PhysicalVariantToNormal(pdata.GetYPhysical(originalIndex)),
              layer.ZAxis.PhysicalVariantToNormal(vArray[j]));

              layer.CoordinateSystem.LogicalToLayerCoordinates(rel, out var pabs);
              pdata.PlotPointsInAbsoluteLayerCoordinates[j] = pabs;
            }
          }

          // we have also to exchange the accessor for the physical y value and replace it by our own one
          var localArray = (AltaxoVariant[])vArray.Clone();
          var localArrayHolder = new LocalArrayHolder(localArray, pdata.RangeList);
          pdata.ZPhysicalAccessor = localArrayHolder.GetPhysical;
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

      if ((indexOfChild + 1) < coll.Count && (coll[indexOfChild + 1] is G3DPlotItem key1))
        prevPlotData = plotDataDict[key1];

      if (indexOfChild > 0 && (coll[indexOfChild - 1] is G3DPlotItem keyM1))
        nextPlotData = plotDataDict[keyM1];

      if (coll[indexOfChild] is G3DPlotItem gpi)
      {
        var pdata = plotDataDict[gpi];
        if (pdata is not null)
        {
          gpi.Paint(g, layer, pdata, prevPlotData, nextPlotData);
        }
      }
      else
      {
        coll[indexOfChild].Paint(g, paintContext, layer, null, null);
      }
    }

    /// <summary>
    /// Private class to hold the transformed physical y values. The problem is that <see cref="GetPhysical"/> is called with the original row index (and not the plot index),
    /// thus a dictionary with translates the original row index to the index in the array is neccessary.
    /// </summary>
    private class LocalArrayHolder
    {
      private AltaxoVariant[] _localArray;
      private Dictionary<int, int> _originalRowIndexToPlotIndex;

      /// <summary>Initializes a new instance of the <see cref="LocalArrayHolder"/> class.</summary>
      /// <param name="localArray">The local array of transformed y values. The array length is equal to the number of plot points.</param>
      /// <param name="rangeList">The range list of the processed plot data. This instance is used to build a dictionary which associates the original row indices to the indices of the <paramref name="localArray"/>.</param>
      public LocalArrayHolder(AltaxoVariant[] localArray, PlotRangeList rangeList)
      {
        _localArray = localArray;
        _originalRowIndexToPlotIndex = rangeList.GetDictionaryOfOriginalRowIndicesToPlotIndices();
      }

      /// <summary>Gets the physical y value for a given original row index.</summary>
      /// <param name="originalRowIndex">Index of the original row.</param>
      /// <returns>The (now transformed) y value of the given original row index.
      /// If it is not found in the dictionary which associates the original row indices to the indices of the <see cref="_localArray"/>, a empty <see cref="Altaxo.Data.AltaxoVariant"/> instance is returned.</returns>
      public AltaxoVariant GetPhysical(int originalRowIndex)
      {
        if (_originalRowIndexToPlotIndex.TryGetValue(originalRowIndex, out var idx))
          return _localArray[idx];
        else
          return new AltaxoVariant();
      }
    }

    #endregion ICoordinateTransformingGroupStyle Members

    #region ICloneable Members

    public object Clone()
    {
      return new AbsoluteStackTransform(this);
    }

    #endregion ICloneable Members
  }
}
