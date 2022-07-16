using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Altaxo.Calc;

namespace Altaxo.Science.Spectroscopy.Raman
{
  /// <summary>
  /// Options for x-axis calibration of a Raman spectrum by a spectrum measured from a Neon lamp.
  /// </summary>
  public record NeonCalibrationOptions
  {
    /// <summary>
    /// Gets the x axis unit of the Neon spectra.
    /// </summary>
    public XAxisUnit XAxisUnit { get; init; }

    /// <summary>
    /// Gets the approximate laser wavelength in nanometer.
    /// This is neccessary to convert Raman shift values to absolute wavelength.
    /// It is enough to give the value approximately here, because a calibration of the laser wavelength itself is done elsewhere.
    /// </summary>
    public double LaserWavelength_Nanometer { get; init; }

    /// <summary>
    /// Wavelength tolerance in nm (this is the value how much the spectrometer can be differ from calibration)
    /// </summary>
    public double Wavelength_Tolerance_nm = 30;


    /// <summary>
    /// Finds a coarse match bewtween the peaks in the measured Neon spectrum and the Nist table.
    /// </summary>
    /// <param name="x">The x values of the measured Neon spectrum.</param>
    /// <param name="y">The y values of the measured Neon spectrum.</param>
    /// <returns>A tuple of wavelength (in nm): Nist wavelength and Meas wavelength at the left of the range, Nist wavelength and meas wavelength at the right of the range.</returns>
    /// The returned value is null if no peaks could be matched.
    public (double NistWL_Left, double MeasWL_Left, double NistWL_Right, double MeasWL_Right)? FindCoarseMatch(double[] x, double[] y)
    {
      var tolWL = Wavelength_Tolerance_nm;
      var x_nm = new double[x.Length];
      ConvertXAxisToNanometer(x, x_nm);

      Array.Sort(x_nm, y); // Sort x-axis ascending

      var peakOptions = new PeakSearchingAndFittingOptions()
      {
        Preprocessing = new SpectralPreprocessingOptions
        {
          BaselineEstimation = new BaselineEstimation.SNIP_Linear { IsHalfWidthInXUnits = false, HalfWidth = 15 },
        },
        PeakSearching = new PeakSearching.PeakSearchingByTopology { MinimalProminence = 0.01 },
      };

      
      (x_nm, y, _) = peakOptions.Preprocessing.Execute(x_nm, y, null);

      var peakSearchingResults = peakOptions.PeakSearching.Execute(y, null);
      var peakDescriptions = peakSearchingResults[0].PeakDescriptions.ToList();

      peakDescriptions.Sort((a,b) => Comparer<double>.Default.Compare(a.PositionIndex, b.PositionIndex));

      // The boundaries for the search in the NIST table
      double boundaryWLSearchLeft = RMath.InterpolateLinear(peakDescriptions[0].PositionIndex, x_nm) - Wavelength_Tolerance_nm;
      double boundaryWLSearchRight = RMath.InterpolateLinear(peakDescriptions[^1].PositionIndex, x_nm) + Wavelength_Tolerance_nm;

      // The inner boundaries are chosen so that the left search is done for 1/4 at the left of the full range,
      // and the right search is done for 1/4 at the right of the full range
      double innerBoundaryWLLeft = boundaryWLSearchLeft + (boundaryWLSearchRight - boundaryWLSearchLeft) / 4.0;
      double innerBoundaryWLRight = boundaryWLSearchRight - (boundaryWLSearchRight - boundaryWLSearchLeft) / 4.0;


      var peaks = NeonCalibration.NistNeonPeaks
                    .Where(pair => pair.Wavelength_Nanometer >= boundaryWLSearchLeft && pair.Wavelength_Nanometer <= boundaryWLSearchRight);

      // Maximum intensity of selected Nist peaks
      var maxIts = peaks.Select(pair => pair.Intensity).Max();

      // Normalize selected Nist peaks to max. intensity of 1
      var nistArr = peaks.Select(pair => (WL: pair.Wavelength_Nanometer, Its: pair.Intensity / maxIts)).ToArray();
      var measArr = peakDescriptions.Select(r => (WL: RMath.InterpolateLinear(r.PositionIndex, x_nm), r.Prominence)).ToArray();

      var left = GetNextPeakToGreaterWavelength(nistArr, innerBoundaryWLLeft);
      var right = GetNextPeakToLessWavelength(nistArr, innerBoundaryWLRight);

      var listOfCandidates = new List<(double Sum, double NistLeft, double MeasLeft, double NistRight, double MeasRight)>();
      for (int lr = 0; left.WL < right.WL - tolWL; ++lr)
      {
        // pick up peaks in measured table that are within the tolerance
        var candidatesLeft = measArr.Where(pair => pair.WL >= left.WL - tolWL && pair.WL <= left.WL + tolWL).ToArray();
        var candidatesRight = measArr.Where(pair => pair.WL >= right.WL - tolWL && pair.WL <= right.WL + tolWL).ToArray();

        foreach (var candidateLeft in candidatesLeft)
        {
          foreach (var candidateRight in candidatesRight)
          {
            // calculate the x-axis stretching that would occur when choosing so
            // the streching should be inside the range from 90% to 110%, otherwise it is not reasonable
            var r = (candidateRight.WL - candidateLeft.WL) / (right.WL - left.WL);
            if (!(r >= 0.9 && r <= 1 / 0.9))
              continue;

            // calculate correlation function

            // need a function that translates real wavelength to wavelength of the measurement system, i.e.
            // left => candLeft.WL and right => candRight.WL
            double ConvertWavelengthNistToMeas(double x)
            {
              var r = (x - left.WL) / (right.WL - left.WL);
              return (1 - r) * candidateLeft.WL + r * candidateRight.WL;
            }

            double sum = 0;
            foreach (var nistPeak in nistArr)
            {
              sum += GetIntensityAtWL(x_nm, y, ConvertWavelengthNistToMeas(nistPeak.WL));
            }

            listOfCandidates.Add((sum, left.WL, candidateLeft.WL, right.WL, candidateRight.WL));
          }
        }

        if (lr % 2 == 0)
        {
          left = GetNextPeakToGreaterWavelength(nistArr, left.WL);
        }
        else
        {
          right = GetNextPeakToLessWavelength(nistArr, right.WL);
        }
      }

      // Sort list, so that the largest sum value (the hottest candidate) is at the top of the list
      listOfCandidates.Sort((a, b) => Comparer<double>.Default.Compare(b.Sum, a.Sum));



      return listOfCandidates.Count == 0 ? null : (listOfCandidates[0].NistLeft, listOfCandidates[0].MeasLeft, listOfCandidates[0].NistRight, listOfCandidates[0].MeasRight);
    }

    static (double WL, double Its) GetNextPeakToGreaterWavelength((double WL, double Its)[] peaks, double actualWavelength)
    {
      for (int i = 0; i < peaks.Length; ++i)
      {
        if (peaks[i].WL > actualWavelength)
          return peaks[i];
      }
      return (double.NaN, double.NaN);
    }

    static (double WL, double Its) GetNextPeakToLessWavelength((double WL, double Its)[] peaks, double actualWavelength)
    {
      for (int i = peaks.Length - 1; i >= 0; --i)
      {
        if (peaks[i].WL < actualWavelength)
          return peaks[i];
      }
      return (double.NaN, double.NaN);
    }

    static double GetIntensityAtWL(double[] WL, double[] Its, double actualWavelength)
    {

      for (int i = 1; i < WL.Length; ++i)
      {
        if (WL[i - 1] <= actualWavelength && WL[i] > actualWavelength)
          return Math.Max(Its[i - 1], Its[i]);
      }
      return 0;
    }

    private void ConvertXAxisToNanometer(double[] x, double[] x_nm)
    {
      // convert x to nanometer
      switch (XAxisUnit)
      {
        case XAxisUnit.RelativeShiftInverseCentimeter:
          for (int i = 0; i < x.Length; i++)
            x_nm[i] = 1 / (1 / LaserWavelength_Nanometer - x[i] / 1E7);
          break;
        case XAxisUnit.AbsoluteWavelengthNanometer:
          for (int i = 0; i < x.Length; i++)
            x_nm[i] = x[i];
          break;
        case XAxisUnit.AbsoluteWavenumberInverseCentimeter:
          for (int i = 0; i < x.Length; i++)
            x_nm[i] = 1E7 / x[i];
          break;
        default:
          break;
      }
    }
  }
}
