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
using Altaxo.Calc.LinearAlgebra;

namespace Altaxo.Science.Spectroscopy.PeakSearching
{
  /// <summary>
  /// Executes peak searching by continuous wavelet transformation (CWT).
  /// </summary>
  /// <remarks>
  /// <para>References:</para>
  /// <para>[1] Bioinformatics (2006) 22 (17): 2059-2065. doi: 10.1093/bioinformatics/btl355</para>
  /// </remarks>
  public record PeakSearchingByCwt : IPeakSearching
  {
    private IWaveletForPeakSearching _wavelet = new WaveletRicker();

    /// <summary>
    /// Gets the wavelet function used for transformation. Default is <see cref="WaveletRicker"/>.
    /// </summary>
    /// <value>
    /// The wavelet.
    /// </value>
    /// <exception cref="System.ArgumentNullException">Wavelet</exception>
    public IWaveletForPeakSearching Wavelet
    {
      get => _wavelet;
      init => _wavelet = value ?? throw new ArgumentNullException(nameof(Wavelet));
    }

    private int _pointsPerOctave = 8;

    /// <summary>
    /// The width of the wavelets is varied logarithmically. The value gives the number of points per octave of width variation (octave = factor of two).
    /// The default value is 8.
    /// </summary>
    /// <value>
    /// The number of points per octave.
    /// </value>
    /// <exception cref="System.ArgumentOutOfRangeException">Points per octave must be >= 4 - NumberOfPointsPerOctave</exception>
    public int NumberOfPointsPerOctave
    {
      get
      {
        return _pointsPerOctave;
      }
      init
      {
        if (!(value >= 4))
          throw new ArgumentOutOfRangeException("Points per octave must be >= 4", nameof(NumberOfPointsPerOctave));
        _pointsPerOctave = value;
      }
    }

    private double _minimalRidgeLengthInOctaves = 2;

    /// <summary>
    /// Gets the minimal ridge length in octaves a ridge must have, in order to be considered as an indication of a peak.
    /// </summary>
    /// <value>
    /// The minimal ridge length in octaves.
    /// </value>
    public double MinimalRidgeLengthInOctaves
    {
      get => _minimalRidgeLengthInOctaves;
      init
      {
        _minimalRidgeLengthInOctaves = value;
      }
    }

    private double _minimalWidthOfRidgeMaximumInOctaves = 2;

    /// <summary>
    /// Going along a ridge, the maximum of the Cwt coefficient indicates the best fit of the peak with the wavelet. The width of the peak can be derived from this location.
    /// The value designates the minimal width of the ridge maximum. Default value is 2 octaves (1 to the left, 1 to the right).
    /// </summary>
    /// <value>
    /// The minimal width of the ridge maximum in octaves.
    /// </value>
    public double MinimalWidthOfRidgeMaximumInOctaves
    {
      get => _minimalWidthOfRidgeMaximumInOctaves;
      init
      {
        _minimalWidthOfRidgeMaximumInOctaves = value;
      }
    }

    private double _minimalSignalToNoiseRatio = 3;

    /// <summary>
    /// Gets the minimal signal to noise ratio a peak must have in order to be included in the result list.
    /// </summary>
    /// <value>
    /// The minimal signal to noise ratio. Default value is 3.
    /// </value>
    public double MinimalSignalToNoiseRatio
    {
      get => _minimalSignalToNoiseRatio;
      init
      {
        _minimalSignalToNoiseRatio = value;
      }
    }

    private double _minimalRelativeGaussianAmplitude = 0.005;

    /// <summary>
    /// Gets the minimal relative gaussian amplitude (relative to the maximum Gaussian amplitude) of the signal, that a peak must have in order to be included in the result.
    /// The default value is 0.005 (0.5%).
    /// </summary>
    /// <value>
    /// The minimal relative gaussian amplitude.
    /// </value>
    public double MinimalRelativeGaussianAmplitude
    {
      get => _minimalRelativeGaussianAmplitude;
      init
      {
        _minimalRelativeGaussianAmplitude = value;
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

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(PeakSearchingByCwt), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (PeakSearchingByCwt)obj;

        info.AddValue("Wavelet", s.Wavelet);
        info.AddValue("NumberOfPointsPerOctave", s.NumberOfPointsPerOctave);
        info.AddValue("MinimalRidgeLengthInOctaves", s.MinimalRidgeLengthInOctaves);
        info.AddValue("MinimalWidthOfRidgeMaximumInOctaves", s.MinimalWidthOfRidgeMaximumInOctaves);
        info.AddValue("MinimalSignalToNoiseRatio", s.MinimalSignalToNoiseRatio);
        info.AddValue("MinimalRelativeGaussianAmplitude", s.MinimalRelativeGaussianAmplitude);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        return new PeakSearchingByCwt()
        {
          Wavelet = info.GetValue<IWaveletForPeakSearching>("Wavelet", null),
          NumberOfPointsPerOctave = info.GetInt32("NumberOfPointsPerOctave"),
          MinimalRidgeLengthInOctaves = info.GetDouble("MinimalRidgeLengthInOctaves"),
          MinimalWidthOfRidgeMaximumInOctaves = info.GetDouble("MinimalWidthOfRidgeMaximumInOctaves"),
          MinimalSignalToNoiseRatio = info.GetDouble("MinimalSignalToNoiseRatio"),
          MinimalRelativeGaussianAmplitude = info.GetDouble("MinimalRelativeGaussianAmplitude"),
          MaximalNumberOfPeaks = null,
        };
      }
    }

    /// <summary>
    /// 2022-10-19 Add property 'MaximalNumberOfPeaks'
    /// </summary>
    /// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(PeakSearchingByCwt), 1)]
    public class SerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (PeakSearchingByCwt)obj;

        info.AddValue("Wavelet", s.Wavelet);
        info.AddValue("NumberOfPointsPerOctave", s.NumberOfPointsPerOctave);
        info.AddValue("MinimalRidgeLengthInOctaves", s.MinimalRidgeLengthInOctaves);
        info.AddValue("MinimalWidthOfRidgeMaximumInOctaves", s.MinimalWidthOfRidgeMaximumInOctaves);
        info.AddValue("MinimalSignalToNoiseRatio", s.MinimalSignalToNoiseRatio);
        info.AddValue("MinimalRelativeGaussianAmplitude", s.MinimalRelativeGaussianAmplitude);
        info.AddValue("MaximalNumberOfPeaks", s.MaximalNumberOfPeaks);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        return new PeakSearchingByCwt()
        {
          Wavelet = info.GetValue<IWaveletForPeakSearching>("Wavelet", null),
          NumberOfPointsPerOctave = info.GetInt32("NumberOfPointsPerOctave"),
          MinimalRidgeLengthInOctaves = info.GetDouble("MinimalRidgeLengthInOctaves"),
          MinimalWidthOfRidgeMaximumInOctaves = info.GetDouble("MinimalWidthOfRidgeMaximumInOctaves"),
          MinimalSignalToNoiseRatio = info.GetDouble("MinimalSignalToNoiseRatio"),
          MinimalRelativeGaussianAmplitude = info.GetDouble("MinimalRelativeGaussianAmplitude"),
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

    public List<PeakDescription> Execute(double[]? x, double[] y)
    {
      int numberOfStages = (int)(NumberOfPointsPerOctave * Math.Log(y.Length) / Math.Log(2));

      var widths = Enumerable.Range(0, numberOfStages).Select(stage => Math.Pow(2, stage / (double)NumberOfPointsPerOctave)).ToArray();
      var max_distances = widths.Select(x => (int)Math.Ceiling(x / 4.0)).ToArray();

      var gap_thresh = 1;
      var (ridgeLines, cwtMatrix) = PeakFinderCWT.Execute(y, widths, _wavelet.WaveletFunction, (i) => max_distances[i], gap_thresh);
      var noise = PeakFinderCWT.GetNoiseLevel(cwtMatrix.ToROMatrix(), null, 0.5);

      // filter the ridge lines

      double maximalGaussAmplitude = double.NegativeInfinity;
      int minimalRequiredRidgeLength = Math.Max(1, (int)Math.Ceiling(_pointsPerOctave * MinimalRidgeLengthInOctaves));
      int minimalOrderOfRidgeMaximum = Math.Max(1, (int)Math.Ceiling(_pointsPerOctave * MinimalWidthOfRidgeMaximumInOctaves / 2.0));

      var filteredRidgeLines1 = new List<RidgeLine>();
      foreach (var ridgeLine in ridgeLines)
      {
        var maxPoint = ridgeLine.GetPointAtMaximalCwtCoefficient(minimalOrderOfRidgeMaximum);

        if (ridgeLine.PointAtLowestWidth.Row == 0 &&                 // ridge line should proceed to stage 0
           ridgeLine.Count >= minimalRequiredRidgeLength &&                // ridge line should have at least 2 octaves
           (maxPoint.CwtCoefficient / noise[maxPoint.Column]) >= _minimalSignalToNoiseRatio && // required signal-to-noise ration at maximum Cwt coefficient
           maxPoint.Row > 0
          )
        {
          filteredRidgeLines1.Add(ridgeLine);
        }

        var (gaussAmplitude, gaussSigma) = _wavelet.GetParametersForGaussianPeak(maxPoint.CwtCoefficient, widths[maxPoint.Row]);

        maximalGaussAmplitude = Math.Max(maximalGaussAmplitude, gaussAmplitude);
      }

      // filter level 2 => discard peaks with Gaussian amplitudes below a value relative to the maximal Gaussian amplitude
      var peakDescriptions = new List<PeakDescription>();
      foreach (var ridgeLine in filteredRidgeLines1)
      {
        var maxPoint = ridgeLine.GetPointAtMaximalCwtCoefficient(minimalOrderOfRidgeMaximum);
        var (gaussAmplitude, gaussSigma) = _wavelet.GetParametersForGaussianPeak(maxPoint.CwtCoefficient, widths[maxPoint.Row]);
        if (gaussAmplitude > (maximalGaussAmplitude * _minimalRelativeGaussianAmplitude))
        {
          var leftSideIndex = maxPoint.Column - gaussSigma;
          var rightSideIndex = maxPoint.Column + gaussSigma;
          var widthValue = x is null ? 2 * gaussSigma : Math.Abs(PeakSearchingNone.GetWidthValue(x, leftSideIndex, maxPoint.Column, rightSideIndex));

          var peakDescription = new PeakDescription()
          {
            Prominence = gaussAmplitude,
            WidthPixels = gaussSigma * 2,
            WidthValue = widthValue,
            RelativeHeightOfWidthDetermination = Math.Exp(-0.5),
            PositionIndex = maxPoint.Column,
            PositionValue = x is null ? maxPoint.Column : x[maxPoint.Column],
            Height = y[maxPoint.Column],
            AbsoluteHeightOfWidthDetermination = y[maxPoint.Column] - gaussAmplitude * (1 - Math.Exp(-0.5))
          };

          peakDescriptions.Add(peakDescription);
        }
      }

      // if there are too many peaks, we prune the peaks with the lowest amplitude
      if (_maximalNumberOfPeaks.HasValue && peakDescriptions.Count > _maximalNumberOfPeaks.Value)
      {
        // Sort so that the hightest peaks are at the beginning of the list
        peakDescriptions.Sort((p1, p2) => Comparer<double>.Default.Compare(p2.Prominence, p1.Prominence));
        // cut the end of the list to the maximal allowed number of peaks
        for (int i = peakDescriptions.Count - 1; i >= _maximalNumberOfPeaks.Value; i--)
          peakDescriptions.RemoveAt(i);
      }

      peakDescriptions.Sort((p1, p2) => Comparer<double>.Default.Compare(p1.PositionIndex, p2.PositionIndex));
      return peakDescriptions;
    }
  }
}
