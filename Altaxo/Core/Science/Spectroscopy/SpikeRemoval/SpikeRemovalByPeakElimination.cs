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
using System.Linq;

namespace Altaxo.Science.Spectroscopy.SpikeRemoval
{
  public record SpikeRemovalByPeakElimination : ISpikeRemoval
  {
    public int MaximalWidth { get; init; } = 1;

    public bool EliminateNegativeSpikes { get; init; } = true;

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoCore", "Altaxo.Science.Spectroscopy.SpikeRemoval.SpikeRemovalByPeakElimination", 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (SpikeRemovalByPeakElimination)obj;
        info.AddValue("MaximalWidth", s.MaximalWidth);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var maximalWidth = info.GetInt32("MaximalWidth");

        return o is null ? new SpikeRemovalByPeakElimination
        {
          MaximalWidth = maximalWidth,
          EliminateNegativeSpikes = false,
        } :
          ((SpikeRemovalByPeakElimination)o) with
          {
            MaximalWidth = maximalWidth,
            EliminateNegativeSpikes = false,
          };
      }
    }

    /// <summary>
    /// 2022-11-14 New property EliminateNegativeSpikes
    /// </summary>
    /// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(SpikeRemovalByPeakElimination), 1)]
    public class SerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (SpikeRemovalByPeakElimination)obj;
        info.AddValue("MaximalWidth", s.MaximalWidth);
        info.AddValue("EliminateNegativeSpikes", s.EliminateNegativeSpikes);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var maximalWidth = info.GetInt32("MaximalWidth");
        var eliminateNegativeSpikes = info.GetBoolean("EliminateNegativeSpikes");

        return o is null ? new SpikeRemovalByPeakElimination
        {
          MaximalWidth = maximalWidth,
          EliminateNegativeSpikes = eliminateNegativeSpikes,
        } :
          ((SpikeRemovalByPeakElimination)o) with
          {
            MaximalWidth = maximalWidth,
            EliminateNegativeSpikes = eliminateNegativeSpikes,
          };
      }
    }
    #endregion

    /// <inheritdoc/>
    public (double[] x, double[] y, int[]? regions) Execute(double[] x, double[] y, int[]? regions)
    {
      if (MaximalWidth >= 1)
      {
        (x, y, regions) = EliminateSpikesOfWidthOneByWavelet(x, y, regions, true, EliminateNegativeSpikes); // always eliminate width one spikes
      }

      (x, y, regions) = EliminateSpikesByTopologicalSearching(x, y, regions, MaximalWidth, false);
      if (EliminateNegativeSpikes)
      {
        (x, y, regions) = EliminateSpikesByTopologicalSearching(x, y, regions, MaximalWidth, true);
      }
      return (x, y, regions);
    }

    /// <summary>
    /// Executes the algorithm either to eliminate positive spikes or negative spikes.
    /// </summary>
    /// <param name="x">The x values.</param>
    /// <param name="y">The y values.</param>
    /// <param name="regions">The regions.</param>
    /// <param name="maximalWidthInPoints">Maximal width of the spike (in points).</param>
    /// <param name="forNegativeSpikes">If set to <c>true</c>, negative spikes will be eliminated; otherwise, positive spikes will be eliminated.</param>
    /// <returns>The x, y and regions array, where y contains the spectrum with eliminated spikes.</returns>
    public static (double[] x, double[] y, int[]? regions) EliminateSpikesByTopologicalSearching(double[] x, double[] y, int[]? regions, int maximalWidthInPoints, bool forNegativeSpikes)
    {
      var yy = (double[])y.Clone();

      foreach (var (start, end) in RegionHelper.GetRegionRanges(regions, x.Length))
      {
        if ((end - start) < 3)
          continue;

        var data = new double[end - start];
        Array.Copy(y, start, data, 0, end - start);
        var dataForPeakSearch = data;
        if (forNegativeSpikes) // if we search negative peaks, we only negate the data for the peak search
        {
          dataForPeakSearch = new double[data.Length];
          for (int i = 0; i < data.Length; ++i)
          {
            dataForPeakSearch[i] = -data[i];
          }
        }

        double noiseLevel = 0;
        for (int i = 2; i < data.Length; ++i)
          noiseLevel += Math.Abs(data[i - 1] - 0.5 * (data[i] + data[i - 2]));
        noiseLevel /= data.Length - 2;

        var peakFinder = new PeakSearching.PeakFinder();
        peakFinder.SetWidth((0, maximalWidthInPoints + 0.25));
        peakFinder.SetRelativeHeight(0.5);
        peakFinder.SetProminence(10 * noiseLevel); // only in order to trigger the calculation of the peak height
        peakFinder.Execute(dataForPeakSearch);

        if (peakFinder.PeakPositions.Length > 0)
        {
          var heights = (double[])peakFinder.Prominences!.Clone();
          var indices = Enumerable.Range(0, heights.Length).ToArray();
          Array.Sort(heights, indices); // Sort heights and the indices

          // now consider the highest peaks that fullfill the peak width criterion
          for (int j = indices.Length - 1; j >= Math.Max(0, indices.Length - 10); --j)
          {
            var idx = indices[j];

            var leftPos = (int)Math.Max(0, Math.Floor(peakFinder.PeakPositions[idx] - peakFinder.Widths![idx] / 2.0));
            var rightPos = (int)Math.Min(data.Length - 1, Math.Ceiling(peakFinder.PeakPositions[idx] + peakFinder.Widths![idx] / 2.0));


            // interpolate data
            for (int k = leftPos + 1; k < rightPos; ++k)
            {
              var r = (k - leftPos) / (double)(rightPos - leftPos);
              var newValue = r * data[rightPos] + (1 - r) * data[leftPos]; // interpolate the base line from left to right
              yy[start + k] = newValue; // replace the values in yArray with the baseline
            }
          }
        }
      }

      return (x, yy, regions);
    }




    /// <summary>
    /// Finds spikes of width = 1 point, using a wavelet transformation (wavelet: -0.5, 1, -0.5), and then eliminates those spikes.
    /// </summary>
    /// <param name="x">The x values.</param>
    /// <param name="y">The y values.</param>
    /// <param name="regions">The regions.</param>
    /// <param name="forPositiveSpikes">If set to <c>true</c>, positive spikes will be eliminated.</param>
    /// <param name="forNegativeSpikes">If set to <c>true</c>, negative spikes will be eliminated.</param>
    /// <returns>The x, y and regions array, where y contains the spectrum with eliminated spikes.</returns>
    public static (double[] x, double[] y, int[]? regions) EliminateSpikesOfWidthOneByWavelet(double[] x, double[] y, int[]? regions, bool forPositiveSpikes, bool forNegativeSpikes)
    {
      double[]? yResult = null;

      foreach (var (start, end) in RegionHelper.GetRegionRanges(regions, x.Length))
      {
        if ((end - start) < 3)
          continue;

        var data = new double[end - start];


        int lenM1 = data.Length - 1, lenM2 = data.Length - 2;
        double noiseLevel = 0;
        for (int i = 0, j = start; i < data.Length; ++i, ++j)
        {
          double v;

          // the wavelet is usually here -0.5, 1, -0.5
          // but at both sides, we have special cases
          if (i == 0)
            v = y[j] - 0.5 * (y[j - 0] + y[j + 1]);
          else if (i == lenM1)
            v = y[j] - 0.5 * (y[j - 1] + y[j + 0]);
          else
            v = y[j] - 0.5 * (y[j - 1] + y[j + 1]);

          data[i] = v;
          noiseLevel += Math.Abs(v);
        }
        noiseLevel /= data.Length;

        double minAmplitude = Math.Abs(5 * noiseLevel);
        for (int i = 1; i < lenM1; ++i)
        {
          bool isPositiveSpike =
                data[i] > minAmplitude &&
                data[i - 1] < -0.35 * data[i] &&
                data[i + 1] < -0.35 * data[i] &&
                (i < 2 || Math.Abs(data[i - 2]) < 0.25 * data[i]) &&
                (i >= lenM2 || Math.Abs(data[i + 2]) < 0.25 * data[i]);

          bool isNegativeSpike =
                data[i] < -minAmplitude &&
                data[i - 1] > -0.35 * data[i] &&
                data[i + 1] > -0.35 * data[i] &&
                (i < 2 || Math.Abs(data[i - 2]) < -0.25 * data[i]) &&
                (i >= lenM2 || Math.Abs(data[i + 2]) < -0.25 * data[i]);

          if ((isPositiveSpike && forPositiveSpikes) || (isNegativeSpike && forNegativeSpikes))
          {
            yResult ??= (double[])y.Clone();

            var leftPos = start + i - 1;
            var rightPos = start + i + 1;

            // if the wavelet at the sides is higher than the noise level,
            // then there is maybe a small influence of the peak to that side, too
            // in this case we use rather one point further away from the peak as base for the interpolation line
            if (i >= 2 && Math.Abs(data[i - 2]) > 1.5 * noiseLevel)
              leftPos = start - i - 2;
            if (i < lenM2 && Math.Abs(data[i + 2]) > 1.5 * noiseLevel)
              rightPos = start + i + 2;


            // now interpolate the data by a straight line from leftPos to rightPos
            for (int k = leftPos + 1; k < rightPos; ++k)
            {
              var r = (k - leftPos) / (double)(rightPos - leftPos);
              var newValue = r * y[rightPos] + (1 - r) * y[leftPos]; // interpolate the base line from left to right
              yResult[k] = newValue; // replace the values in yArray with the baseline
            }
          }
        }

      } // Foreach region

      return (x, yResult ?? y, regions);
    }

    /// <summary>
    /// Finds spikes of width = 2 points, using a wavelet transformation (wavelet: -0.5, 0.5, 0.5, -0.5), and then eliminates those spikes.
    /// </summary>
    /// <param name="x">The x values.</param>
    /// <param name="y">The y values.</param>
    /// <param name="regions">The regions.</param>
    /// <param name="forPositiveSpikes">If set to <c>true</c>, positive spikes will be eliminated.</param>
    /// <param name="forNegativeSpikes">If set to <c>true</c>, negative spikes will be eliminated.</param>
    /// <returns>The x, y and regions array, where y contains the spectrum with eliminated spikes.</returns>
    public static (double[] x, double[] y, int[]? regions) EliminateSpikesOfWidthTwoByWavelet(double[] x, double[] y, int[]? regions, bool forPositiveSpikes, bool forNegativeSpikes)
    {
      double[]? yResult = null;

      foreach (var (start, end) in RegionHelper.GetRegionRanges(regions, x.Length))
      {
        if ((end - start) < 5)
          continue;

        var data = new double[end - start];


        int lenM1 = data.Length - 1, lenM2 = data.Length - 2, lenM3 = data.Length - 3;
        double noiseLevel = 0;
        for (int i = 0, j = start; i < lenM1; ++i, ++j)
        {
          double v;

          // the wavelet used here is -0.5, 0.5, 0.5, -0.5
          // but at both sides, we have special cases
          if (i == 0)
            v = 0.5 * (y[j] + y[j + 1] - 2 * y[j + 2]);
          else if (i == lenM2)
            v = 0.5 * (-2 * y[j - 1] + y[j] + y[j + 1]);
          else
            v = 0.5 * (-y[j - 1] + y[j] + y[j + 1] - y[j + 2]);

          data[i] = v;
          noiseLevel += Math.Abs(v);
        }
        noiseLevel /= lenM1;

        double minAmplitude = Math.Abs(5 * noiseLevel);
        for (int i = 2; i < lenM2; ++i)
        {
          bool isPositiveSpike =
                data[i] > minAmplitude &&
                (data[i - 1] < -0.35 * data[i] || data[i - 2] < -0.35 * data[i]) &&
                (data[i + 1] < -0.35 * data[i] || data[i + 2] < -0.35 * data[i]) &&
                (i < 3 || Math.Abs(data[i - 3]) < 0.25 * data[i]) &&
                (i >= lenM3 || Math.Abs(data[i + 3]) < 0.25 * data[i]);

          bool isNegativeSpike =
                data[i] < -minAmplitude &&
                (data[i - 1] > -0.35 * data[i] || data[i - 2] > -0.35 * data[i]) &&
                (data[i + 1] > -0.35 * data[i] || data[i + 2] > -0.35 * data[i]) &&
                (i < 3 || Math.Abs(data[i - 3]) < -0.25 * data[i]) &&
                (i >= lenM3 || Math.Abs(data[i + 3]) < -0.25 * data[i]);

          if ((isPositiveSpike && forPositiveSpikes) || (isNegativeSpike && forNegativeSpikes))
          {
            yResult ??= (double[])y.Clone();

            // Note that the main spike would be at positions i and i+1, thus the interpolation bases would be i-1 and i+2
            var leftPos = start + i - 1;
            var rightPos = start + i + 2;

            // if the wavelet at the sides is higher than the noise level,
            // then there is maybe a small influence of the peak to that side, too
            // in this case we use rather one point further away from the peak as base for the interpolation line
            if (i >= 3 && Math.Abs(data[i - 3]) > 1.5 * noiseLevel)
              leftPos = start - i - 2;
            if (i < lenM3 && Math.Abs(data[i + 3]) > 1.5 * noiseLevel)
              rightPos = start + i + 3;


            // now interpolate the data by a straight line from leftPos to rightPos
            for (int k = leftPos + 1; k < rightPos; ++k)
            {
              var r = (k - leftPos) / (double)(rightPos - leftPos);
              var newValue = r * y[rightPos] + (1 - r) * y[leftPos]; // interpolate the base line from left to right
              yResult[k] = newValue; // replace the values in yArray with the baseline
            }
          }
        }

      } // Foreach region

      return (x, yResult ?? y, regions);
    }

  }
}
