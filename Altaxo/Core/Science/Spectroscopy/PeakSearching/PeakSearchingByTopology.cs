#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2022 Dr. Dirk Lellinger
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

using System;
using System.Collections.Generic;
using System.Linq;

namespace Altaxo.Science.Spectroscopy.PeakSearching
{
  /// <summary>
  /// Executes area normalization : y' = (y-min)/(mean), in which min and mean are the minimal and the mean values of the array.
  /// </summary>
  /// <seealso cref="Altaxo.Science.Spectroscopy.Normalization.INormalization" />
  public record PeakSearchingByTopology : IPeakSearching
  {
    private double? _minimalProminence = 0.01;

    public double? MinimalProminence
    {
      get => _minimalProminence;
      init
      {
        if (value.HasValue && !(value.Value >= 0))
          throw new ArgumentOutOfRangeException("Value must be >=0", nameof(MinimalProminence));

        _minimalProminence = value;
      }
    }

    private int? _maximalNumberOfPeaks = 50;

    /// <summary>
    /// If a value is set, this limits the number of peaks included in the result to this number of peaks with the highest amplitude.
    /// </summary>
    /// <value>
    /// The maximal number of peaks.
    /// </value>
    /// <exception cref="System.ArgumentException">Value must either be null or >0</exception>
    public int? MaximalNumberOfPeaks
    {
      get => _maximalNumberOfPeaks;
      set
      {
        if (value.HasValue && value.Value <= 0)
          throw new ArgumentException("Value must either be null or >0");
        _maximalNumberOfPeaks = value;
      }
    }

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(PeakSearchingByTopology), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (PeakSearchingByTopology)obj;
        info.AddValue("MinimalProminence", s._minimalProminence);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var minimalProminence = info.GetNullableDouble("MinimalProminence");
        return new PeakSearchingByTopology()
        {
          MinimalProminence = minimalProminence,
        };
      }
    }

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(PeakSearchingByTopology), 1)]
    public class SerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (PeakSearchingByTopology)obj;
        info.AddValue("MinimalProminence", s._minimalProminence);
        info.AddValue("MaximalNumberOfPeaks", s.MaximalNumberOfPeaks);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        return new PeakSearchingByTopology()
        {
          MinimalProminence = info.GetNullableDouble("MinimalProminence"),
          MaximalNumberOfPeaks = info.GetNullableInt32("MaximalNumberOfPeaks"),
        };
      }
    }

    #endregion

    public IReadOnlyList<(IReadOnlyList<PeakDescription> PeakDescriptions, int StartOfRegion, int EndOfRegion)> Execute(double[]? x, double[] y, int[]? regions)
    {
      var peakDescriptions = new List<(IReadOnlyList<PeakDescription> PeakDescriptions, int StartOfRegion, int EndOfRegion)>();
      foreach (var (start, end) in RegionHelper.GetRegionRanges(regions, y.Length))
      {
        var subX = x is null ? null : new double[end - start];
        if (subX is not null)
          Array.Copy(x, start, subX, 0, end - start);

        var subY = new double[end - start];
        Array.Copy(y, start, subY, 0, end - start);

        var result = Execute(subX, subY);
        peakDescriptions.Add((result, start, end));
      }

      return peakDescriptions;
    }

    /// <inheritdoc/>
    public List<PeakDescription> Execute(double[]? x, double[] y)
    {
      var pf = new PeakFinder();

      var workingMinimalProminence = (_minimalProminence ?? 0.0) * (y.Max() - y.Min());

      pf.SetProminence(workingMinimalProminence);
      pf.SetRelativeHeight(0.5);
      pf.SetWidth(0.0);
      pf.SetHeight(0.0);
      pf.Execute(y);

      var peakDescriptions = new List<PeakDescription>(pf.PeakPositions.Length);


      for (int i = 0; i < pf.PeakPositions.Length; i++)
      {
        var leftSideIndex = pf.PeakPositions[i] - 0.5 * pf.Widths![i];
        var rightSideIndex = pf.PeakPositions[i] + 0.5 * pf.Widths![i];
        var widthValue = x is null ? pf.Widths![i] : Math.Abs(PeakSearchingNone.GetWidthValue(x, leftSideIndex, pf.PeakPositions[i], rightSideIndex));

        peakDescriptions.Add(
          new PeakDescription()
          {
            PositionIndex = pf.PeakPositions[i],
            PositionValue = x is null ? pf.PeakPositions[i] : x[pf.PeakPositions[i]],
            Prominence = pf.Prominences![i],
            Height = pf.PeakHeights![i],
            WidthPixels = pf.Widths![i],
            WidthValue = widthValue,
            RelativeHeightOfWidthDetermination = 0.5,
            AbsoluteHeightOfWidthDetermination = pf.WidthHeights![i],
          });
      }


      // if there are too many peaks, we prune the peaks with the lowest amplitude
      if (_maximalNumberOfPeaks.HasValue && peakDescriptions.Count > _maximalNumberOfPeaks.Value)
      {
        // Sort so that the hightest peaks are at the beginning of the list
        peakDescriptions.Sort((p1, p2) => Comparer<double>.Default.Compare(p2.Prominence, p1.Prominence));

        // cut the end of the list to the maximal allowed number of peaks
        for (int i = peakDescriptions.Count - 1; i >= _maximalNumberOfPeaks.Value; i--)
        {
          peakDescriptions.RemoveAt(i);
        }

        // now sort again by position
        peakDescriptions.Sort((p1, p2) => Comparer<double>.Default.Compare(p1.PositionIndex, p2.PositionIndex));
      }

      return peakDescriptions;
    }
  }
}
