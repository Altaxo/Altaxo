using System.Collections.Generic;

namespace Altaxo.Science.Spectroscopy.Sanitizing
{
  /// <summary>
  /// Remove zeros at the start and the end of the spectrum, and optionally also in the middle of the spectrum. Although normally there is no
  /// problem of having those values at the start or end of the spectrum, they lead to wrong estimates of the noise level.
  /// </summary>
  public record RemoveZeros : ISanitizer
  {
    /// <summary>
    /// Gets the threshold value. Values in the spectrum greater than this value are considered as true measurement values,
    /// the other values are considered as candidates for fake values, that can be removed.
    /// </summary>
    public double ThresholdValue { get; init; }

    /// <summary>
    /// If true, zeros at the start of the spectrum will be removed.
    /// </summary>
    public bool RemoveZerosAtStartOfSpectrum { get; init; }

    /// <summary>
    /// If true, zeros at the end of the spectrum will be removed.
    /// </summary>
    public bool RemoveZerosAtEndOfSpectrum { get; init; }

    /// <summary>
    /// If true, zeros in the middle of the spectrum will be removed.
    /// This can happen at some instruments, if pixels cease working, or are oversaturated.
    /// </summary>
    public bool RemoveZerosInMiddleOfSpectrum { get; init; }

    /// <summary>
    /// If true, and the <see cref="RemoveZerosInMiddleOfSpectrum"/> is set to true,
    /// then the spectrum is split into to regions, at the position where zero pixels are detected.
    /// </summary>
    public bool SplitIntoSeparateRegions { get; init; }



    #region Serialization

    /// <summary>
    /// Version 0 : 2023-06-30
    /// </summary>
    /// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(RemoveZeros), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (RemoveZeros)obj;

        info.AddValue("ThresholdValue", s.ThresholdValue);
        info.AddValue("RemoveZerosAtStart", s.RemoveZerosAtStartOfSpectrum);
        info.AddValue("RemoveZerosAtEnd", s.RemoveZerosAtEndOfSpectrum);
        info.AddValue("RemoveZerosInMiddle", s.RemoveZerosInMiddleOfSpectrum);
        info.AddValue("SplitIntoSeparateRegions", s.SplitIntoSeparateRegions);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        double thresholdValue = info.GetDouble("ThresholdValue");
        bool removeAtStart = info.GetBoolean("RemoveZerosAtStart");
        bool removeAtEnd = info.GetBoolean("RemoveZerosAtEnd");
        bool removeInMiddle = info.GetBoolean("RemoveZerosInMiddle");
        bool splitIntoSeparateRegions = info.GetBoolean("SplitIntoSeparateRegions");

        return new RemoveZeros
        {
          ThresholdValue = thresholdValue,
          RemoveZerosAtStartOfSpectrum = removeAtStart,
          RemoveZerosAtEndOfSpectrum = removeAtEnd,
          RemoveZerosInMiddleOfSpectrum = removeInMiddle,
          SplitIntoSeparateRegions = splitIntoSeparateRegions,
        };
      }
    }
    #endregion



    /// <inheritdoc/>
    public (double[] x, double[] y, int[]? regions) Execute(double[] x, double[] y, int[]? regions)
    {
      var xx = new List<double>(x.Length);
      var yy = new List<double>(y.Length);
      var rr = new List<int>();

      void AddPoints(int start, int end, bool addRegion)
      {
        if (start < end)
        {
          for (int i = start; i < end; i++)
          {
            xx.Add(x[i]);
            yy.Add(y[i]);
          }
          if (addRegion)
          {
            rr.Add(xx.Count);
          }
        }
      }

      foreach (var (start, end) in RegionHelper.GetRegionRanges(regions, x.Length))
      {
        var tstart = start;
        var tend = end;

        if (RemoveZerosAtStartOfSpectrum || RemoveZerosInMiddleOfSpectrum)
        {
          tstart = end;
          for (int i = start; i < end; ++i)
          {
            if (y[i] > ThresholdValue)
            {
              tstart = i;
              break;
            }
          }
        }
        if (RemoveZerosAtEndOfSpectrum || RemoveZerosInMiddleOfSpectrum)
        {
          tend = tstart;
          for (int i = end - 1; i >= start; --i)
          {
            if (y[i] > ThresholdValue)
            {
              tend = i + 1;
              break;
            }
          }
        }

        if (RemoveZerosInMiddleOfSpectrum)
        {
          bool inGap = false;
          int segmentStart = tstart;
          for (int i = tstart; i < tend; ++i)
          {
            if (y[i] > ThresholdValue && inGap)
            {
              inGap = false;
              segmentStart = i;
            }
            else if (!(y[i] > ThresholdValue) && !inGap)
            {
              inGap = true;
              AddPoints(segmentStart, i, SplitIntoSeparateRegions);
            }
          }
          if (!inGap)
          {
            AddPoints(segmentStart, tend, true);
          }
        }
        else
        {
          AddPoints(tstart, tend, true);
        }
      }
      return (xx.ToArray(), yy.ToArray(), RegionHelper.NormalizeRegions(rr, yy.Count));
    }
  }
}
