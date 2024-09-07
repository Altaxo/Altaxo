#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2024 Dr. Dirk Lellinger
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

using Xunit;

namespace Altaxo.Science.Spectroscopy.PeakFitting.MultipleSpectra
{
  public class PeakFittingOfMultipleSpectraByIncrementalPeakAddition_Tests
  {
    /// <summary>
    /// Test with two peaks, and one spectrum.
    /// </summary>
    [Fact]
    public void TestWithOneSpectrum()
    {
      const int numberOfPoints = 100;
      var x = new double[numberOfPoints];
      var y = new double[numberOfPoints];

      for (int i = 0; i < numberOfPoints; ++i)
      {
        x[i] = 1000 + i;
        y[i] = 7000 +
          Altaxo.Calc.FitFunctions.Peaks.GaussAmplitude.GetYOfOneTerm(x[i], 666, 1030, 5) +
          Altaxo.Calc.FitFunctions.Peaks.GaussAmplitude.GetYOfOneTerm(x[i], 444, 1070, 4);
      }



      var fit = new PeakFittingOfMultipleSpectraByIncrementalPeakAddition()
      {
        FitFunction = new Altaxo.Calc.FitFunctions.Peaks.GaussAmplitude(1, 0),
        FitWidthScalingFactor = 2,
        OrderOfBaselinePolynomial = 0,
        MaximumNumberOfPeaks = 2,
      };

      var result = fit.Execute([(x, y)], cancellationToken: System.Threading.CancellationToken.None);

      Assert.Equal(2, result.Count);
      AssertEx.Equal(1030.0, result[0].PeakParameter[0], 1E-7, 1e-7, "Position of first peak");
      AssertEx.Equal(5.0, result[0].PeakParameter[1], 1E-7, 1e-7, "Sigma of first peak");
      AssertEx.Equal(666.0, result[0].PeakAmplitudes[0], 1E-7, 1E-7, "Amplitude of first peak");

      AssertEx.Equal(1070.0, result[1].PeakParameter[0], 1E-7, 1e-7, "Position of second peak");
      AssertEx.Equal(4.0, result[1].PeakParameter[1], 1E-7, 1e-7, "Sigma of second peak");
      AssertEx.Equal(444.0, result[1].PeakAmplitudes[0], 1E-7, 1e-7, "Amplitude of second peak");

    }

    /// <summary>
    /// Test with two peaks, and two absolutely identical spectra.
    /// </summary>
    [Fact]
    public void TestWithTwoEqualSpectra()
    {
      const int numberOfPoints0 = 100;
      var x0 = new double[numberOfPoints0];
      var y0 = new double[numberOfPoints0];

      for (int i = 0; i < numberOfPoints0; ++i)
      {
        x0[i] = 1000 + i;
        y0[i] = 7000 +
          Altaxo.Calc.FitFunctions.Peaks.GaussAmplitude.GetYOfOneTerm(x0[i], 666, 1030, 5) +
          Altaxo.Calc.FitFunctions.Peaks.GaussAmplitude.GetYOfOneTerm(x0[i], 444, 1070, 4);
      }

      var fit = new PeakFittingOfMultipleSpectraByIncrementalPeakAddition()
      {
        FitFunction = new Altaxo.Calc.FitFunctions.Peaks.GaussAmplitude(1, 0),
        FitWidthScalingFactor = 2,
        OrderOfBaselinePolynomial = 0,
        MaximumNumberOfPeaks = 2,
      };

      var result = fit.Execute([(x0, y0), (x0, y0)], cancellationToken: System.Threading.CancellationToken.None);

      Assert.Equal(2, result.Count);
      AssertEx.Equal(1030.0, result[0].PeakParameter[0], 1E-7, 1e-7, "Position of first peak");
      AssertEx.Equal(5.0, result[0].PeakParameter[1], 1E-7, 1e-7, "Sigma of first peak");
      AssertEx.Equal(666.0, result[0].PeakAmplitudes[0], 1E-7, 1E-7, "Amplitude of first peak, first spectrum");
      AssertEx.Equal(666.0, result[0].PeakAmplitudes[1], 1E-7, 1E-7, "Amplitude of first peak, second spectrum");

      AssertEx.Equal(1070.0, result[1].PeakParameter[0], 1E-7, 1e-7, "Position of second peak");
      AssertEx.Equal(4.0, result[1].PeakParameter[1], 1E-7, 1e-7, "Sigma of second peak");
      AssertEx.Equal(444.0, result[1].PeakAmplitudes[0], 1E-7, 1e-7, "Amplitude of second peak, first spectrum");
      AssertEx.Equal(444.0, result[1].PeakAmplitudes[1], 1E-7, 1e-7, "Amplitude of second peak, second spectrum");
    }

    /// <summary>
    /// Test with two peaks, and two spectra with even different length, and different peak amplitudes.
    /// </summary>
    [Fact]
    public void TestWithTwoDifferentSpectra()
    {
      const int numberOfPoints0 = 100;
      var x0 = new double[numberOfPoints0];
      var y0 = new double[numberOfPoints0];

      for (int i = 0; i < numberOfPoints0; ++i)
      {
        x0[i] = 1000 + i;
        y0[i] = 7000 +
          Altaxo.Calc.FitFunctions.Peaks.GaussAmplitude.GetYOfOneTerm(x0[i], 666, 1030, 5) +
          Altaxo.Calc.FitFunctions.Peaks.GaussAmplitude.GetYOfOneTerm(x0[i], 444, 1070, 4);
      }

      const int numberOfPoints1 = 90;
      var x1 = new double[numberOfPoints1];
      var y1 = new double[numberOfPoints1];

      for (int i = 0; i < numberOfPoints1; ++i)
      {
        x1[i] = 1000 + i;
        y1[i] = 5000 +
          Altaxo.Calc.FitFunctions.Peaks.GaussAmplitude.GetYOfOneTerm(x1[i], 222, 1030, 5) +
          Altaxo.Calc.FitFunctions.Peaks.GaussAmplitude.GetYOfOneTerm(x1[i], 555, 1070, 4);
      }



      var fit = new PeakFittingOfMultipleSpectraByIncrementalPeakAddition()
      {
        FitFunction = new Altaxo.Calc.FitFunctions.Peaks.GaussAmplitude(1, 0),
        FitWidthScalingFactor = 2,
        OrderOfBaselinePolynomial = 0,
        MaximumNumberOfPeaks = 2,
      };

      var result = fit.Execute([(x0, y0), (x1, y1)], cancellationToken: System.Threading.CancellationToken.None);

      Assert.Equal(2, result.Count);
      AssertEx.Equal(1030.0, result[0].PeakParameter[0], 1E-7, 1e-7, "Position of first peak");
      AssertEx.Equal(5.0, result[0].PeakParameter[1], 1E-7, 1e-7, "Sigma of first peak");
      AssertEx.Equal(666.0, result[0].PeakAmplitudes[0], 1E-7, 1E-7, "Amplitude of first peak, first spectrum");
      AssertEx.Equal(222.0, result[0].PeakAmplitudes[1], 1E-7, 1E-7, "Amplitude of first peak, second spectrum");

      AssertEx.Equal(1070.0, result[1].PeakParameter[0], 1E-7, 1e-7, "Position of second peak");
      AssertEx.Equal(4.0, result[1].PeakParameter[1], 1E-7, 1e-7, "Sigma of second peak");
      AssertEx.Equal(444.0, result[1].PeakAmplitudes[0], 1E-7, 1e-7, "Amplitude of second peak, first spectrum");
      AssertEx.Equal(555.0, result[1].PeakAmplitudes[1], 1E-7, 1e-7, "Amplitude of second peak, second spectrum");

    }
  }

}
