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
  public record PeakSearchingByCWT : IPeakSearching
  {
    int _pointsPerOctave = 8;

    public int PointsPerOctave
    {
      get
      {
        return _pointsPerOctave;
      }
      set
      {
        if (!(value >= 4))
          throw new ArgumentOutOfRangeException("Points per octave must be >= 4", nameof(PointsPerOctave));
        _pointsPerOctave = value; 
      }
    }

    IWaveletForPeakSearching _wavelet = new WaveletRicker();
    public IWaveletForPeakSearching Wavelet
    {
      get => _wavelet;
      set => _wavelet = value ?? throw new ArgumentNullException(nameof(Wavelet));
    }

    private double _requiredSignalToNoiseRatio=3;

    public double RequiredSignalToNoiseRatio
    {
      get => _requiredSignalToNoiseRatio;
      set
      {
        _requiredSignalToNoiseRatio = value;
      }
    }

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(PeakSearchingByCWT), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (PeakSearchingByCWT)obj;
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        return new PeakSearchingByCWT()
        {
        };
      }
    }
    #endregion

    public IPeakSearchingResult Execute(double[] input)
    {
      int numberOfStages = (int)(PointsPerOctave * Math.Log(input.Length) / Math.Log(2));

      var widths = Enumerable.Range(0, numberOfStages).Select(stage => Math.Pow(2, stage / (double)PointsPerOctave)).ToArray();
      var max_distances = widths.Select(x => (int)Math.Ceiling(x / 4.0)).ToArray();

      var gap_thresh = 1;
      var (ridgeLines, cwtMatrix) = PeakFinderCWT.Execute(input, widths, _wavelet.WaveletFunction, (i) => max_distances[i], gap_thresh);
      var noise = PeakFinderCWT.GetNoiseLevel(cwtMatrix.ToROMatrix(), null, 0.5);

      // filter the ridge lines

      double maximalGaussAmplitude = double.NegativeInfinity;
      var filteredRidgeLines1 = new List<RidgeLine>();
      foreach (var ridgeLine in ridgeLines)
      {
        var maxPoint = ridgeLine.GetPointAtMaximalCwtCoefficient(PointsPerOctave);

        if (ridgeLine.PointAtLowestWidth.Row == 0 &&                 // ridge line should proceed to stage 0
           ridgeLine.Count >= (2 * PointsPerOctave) &&                // ridge line should have at least 2 octaves
           (maxPoint.CwtCoefficient / noise[maxPoint.Column]) >= _requiredSignalToNoiseRatio && // required signal-to-noise ration at maximum Cwt coefficient
           maxPoint.Row > 0
          )
        {
          filteredRidgeLines1.Add(ridgeLine);
        }

        var (gaussAmplitude, gaussSigma) = _wavelet.GetParametersForGaussianPeak(maxPoint.CwtCoefficient, maxPoint.Row);

        maximalGaussAmplitude = Math.Max(maximalGaussAmplitude, gaussAmplitude);
      }

      // filter level 2 => discard peaks with Gaussian amplitudes below a value relative to the maximal Gaussian amplitude
      var peakDescriptions = new List<PeakDescription>();
      foreach (var ridgeLine in ridgeLines)
      {
        var maxPoint = ridgeLine.GetPointAtMaximalCwtCoefficient(PointsPerOctave);
        var (gaussAmplitude, gaussSigma) = _wavelet.GetParametersForGaussianPeak(maxPoint.CwtCoefficient, maxPoint.Row);
        if (gaussAmplitude > (maximalGaussAmplitude * 0.005))
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
