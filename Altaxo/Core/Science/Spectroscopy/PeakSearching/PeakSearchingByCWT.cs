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
    IWaveletForPeakSearching _wavelet = new WaveletRicker();

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

    int _pointsPerOctave = 8;

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

    private double _minimalRidgeLengthInOctaves=2;

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
      get => _minimalRelativeGaussianAmplitude ;
      init
      {
          _minimalRelativeGaussianAmplitude  = value;
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
          MinimalRelativeGaussianAmplitude = info.GetDouble("MinimalRelativeGaussianAmplitude")
        };
      }
    }
    #endregion

    public IPeakSearchingResult Execute(double[] input)
    {
      int numberOfStages = (int)(NumberOfPointsPerOctave * Math.Log(input.Length) / Math.Log(2));

      var widths = Enumerable.Range(0, numberOfStages).Select(stage => Math.Pow(2, stage / (double)NumberOfPointsPerOctave)).ToArray();
      var max_distances = widths.Select(x => (int)Math.Ceiling(x / 4.0)).ToArray();

      var gap_thresh = 1;
      var (ridgeLines, cwtMatrix) = PeakFinderCWT.Execute(input, widths, _wavelet.WaveletFunction, (i) => max_distances[i], gap_thresh);
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
          var peakDescription = new PeakDescription()
          {
            Prominence = gaussAmplitude,
            Width = gaussSigma * 2,
            RelativeHeightOfWidthDetermination = Math.Exp(-0.5),
            PositionIndex = maxPoint.Column,
            Height = input[maxPoint.Column],
            AbsoluteHeightOfWidthDetermination = input[maxPoint.Column] - gaussAmplitude * (1 - Math.Exp(-0.5))
          };

          peakDescriptions.Add(peakDescription);
        }
      }

      peakDescriptions.Sort((p1, p2) => Comparer<double>.Default.Compare(p1.PositionIndex, p2.PositionIndex));
      return new Result() { PeakDescriptions = peakDescriptions };
    }


    #region Result

    class Result : IPeakSearchingResult
    {
      IReadOnlyList<PeakDescription> _description = new PeakDescription[0];

      public IReadOnlyList<PeakDescription> PeakDescriptions
      {
        get => _description;
        init => _description = value ?? throw new ArgumentNullException();
      }
    }
    #endregion
  }
}
