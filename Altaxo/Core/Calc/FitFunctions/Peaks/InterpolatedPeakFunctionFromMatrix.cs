#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2026 Dr. Dirk Lellinger
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
using Altaxo.Calc.Interpolation;
using Altaxo.Calc.LinearAlgebra;
using Altaxo.Calc.Regression.Nonlinear;

namespace Altaxo.Calc.FitFunctions.Peaks
{
  /// <summary>
  /// Base class for peak functions whose shape is defined by a two-dimensional lookup table
  /// and evaluated by interpolation.
  /// </summary>
  /// <remarks>
  /// The lookup table is interpreted as a function <c>z = z(x, y)</c> given on a regular grid.
  /// Interpolation is performed using <see cref="BivariateAkimaSpline" />.
  /// </remarks>
  public abstract record InterpolatedPeakFunctionFromMatrix : IFitFunctionPeak, Main.IImmutable
  {
    private const string ParameterBaseName0 = "a";
    private const string ParameterBaseName1 = "xc";
    private const int NumberOfParametersPerPeak = 2;


    /// <summary>
    /// Initializes a new instance of the <see cref="InterpolatedPeakFunctionFromMatrix"/> class.
    /// </summary>
    /// <param name="numberOfTerms">The number of peak terms.</param>
    /// <param name="orderOfBaselinePolynomial">
    /// The order of the baseline polynomial, or <c>-1</c> to disable the baseline.
    /// </param>
    /// <param name="peakPositionValues">The peak positions of the curves.</param>
    /// <param name="peakCurveXValues">The x values of the peak curves.</param>
    /// <param name="peakCurvesYValues">The y values of the peak curves. Each curve is represented by one row of the matrix.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if <paramref name="numberOfTerms"/> is negative or if <paramref name="orderOfBaselinePolynomial"/> is less than <c>-1</c>.
    /// </exception>
    public InterpolatedPeakFunctionFromMatrix(int numberOfTerms,
                                                   int orderOfBaselinePolynomial,
                                                   IReadOnlyList<double> peakPositionValues,
                                                   IReadOnlyList<double> peakCurveXValues,
                                                   IROMatrix<double> peakCurvesYValues)
     : this(numberOfTerms, orderOfBaselinePolynomial)
    {
      InitializeSpline(peakPositionValues, peakCurveXValues, peakCurvesYValues);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InterpolatedPeakFunctionFromMatrix"/> class.
    /// </summary>
    /// <param name="numberOfTerms">The number of peak terms.</param>
    /// <param name="orderOfBaselinePolynomial">
    /// The order of the baseline polynomial, or <c>-1</c> to disable the baseline.
    /// </param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if <paramref name="numberOfTerms"/> is negative or if <paramref name="orderOfBaselinePolynomial"/> is less than <c>-1</c>.
    /// </exception>
    protected InterpolatedPeakFunctionFromMatrix(int numberOfTerms,
                                                   int orderOfBaselinePolynomial)
    {
      if (numberOfTerms < 0)
        throw new ArgumentOutOfRangeException(nameof(numberOfTerms), $"{nameof(numberOfTerms)} must be greater than or equal to 0");
      if (orderOfBaselinePolynomial < -1)
        throw new ArgumentOutOfRangeException(nameof(orderOfBaselinePolynomial), $"{nameof(orderOfBaselinePolynomial)} must be greater than or equal to 0, or -1 in order to deactivate it.");
      NumberOfTerms = numberOfTerms;
      OrderOfBaselinePolynomial = orderOfBaselinePolynomial;
    }

    /// <summary>
    /// Initializes the spline from a matrix representation of peak curves.
    /// </summary>
    /// <param name="peakPositionValues">The peak positions of the curves.</param>
    /// <param name="peakCurveXValues">The x values of the peak curves.</param>
    /// <param name="peakCurvesYValues">The y values of the peak curves. Each curve is represented by one row of the matrix.</param>
    protected void InitializeSpline(IReadOnlyList<double> peakPositionValues, IReadOnlyList<double> peakCurveXValues, IROMatrix<double> peakCurvesYValues)
    {
      Spline = new BivariateAkimaSpline(peakPositionValues, peakCurveXValues, peakCurvesYValues, copyDataLocally: true);
      MinimalPosition = peakPositionValues[0];
      MaximalPosition = peakPositionValues[^1];
      MinimalX = peakCurveXValues[0];
      MaximalX = peakCurveXValues[^1];

      if (!(MinimalX < 0))
        throw new ArgumentException($"The minimal x value of the table must be less than 0, but {MinimalX} was given.", nameof(peakCurveXValues));
      if (!(MaximalX > 0))
        throw new ArgumentException($"The maximal x value of the table must be greater than 0, but {MaximalX} was given.", nameof(peakCurveXValues));

      // now evaluate the FWHM for each position

      var fwhms = new double[peakPositionValues.Count];
      for (int i = 0; i < peakPositionValues.Count; ++i)
      {
        var peakValue = Spline.Interpolate(peakPositionValues[i], 0);
        var halfValue = peakValue / 2;

        var rootPos = Altaxo.Calc.RootFinding.QuickRootFinding.ByBrentsAlgorithm(x => Spline.Interpolate(peakPositionValues[i], x) - halfValue, 0, MaximalX);
        var rootNeg = Altaxo.Calc.RootFinding.QuickRootFinding.ByBrentsAlgorithm(x => Spline.Interpolate(peakPositionValues[i], x) - halfValue, MinimalX, 0);
        fwhms[i] = rootPos - rootNeg;
      }

      FwhmSpline = new AkimaCubicSplineOptions().Interpolate(peakPositionValues, fwhms);
    }

    /// <summary>
    /// Initializes the component, setting up necessary resources and configurations.
    /// </summary>
    /// <remarks>This method must be implemented by derived classes to define specific initialization logic.
    /// It is called if any of the methods that needs <see cref="Spline"/> is called, and <see cref="Spline"/> is <c>null</c>.
    /// Thus, after calling the method, <see cref="Spline"/> should be valid.</remarks>
    public abstract void Initialize();


    /// <summary>
    /// Gets the spline used to interpolate the lookup table.
    /// </summary>
    protected BivariateAkimaSpline? Spline { get; private set; }

    /// <summary>
    /// Gets the minimal peak position that can be evaluated.
    /// </summary>
    protected double MinimalPosition { get; private set; }

    /// <summary>
    /// Gets the maximal peak position that can be evaluated.
    /// </summary>
    protected double MaximalPosition { get; private set; }

    /// <summary>
    /// Gets the minimal x value that can be evaluated.
    /// </summary>
    protected double MinimalX { get; private set; }

    /// <summary>
    /// Gets the maximal x value that can be evaluated.
    /// </summary>
    protected double MaximalX { get; private set; }

    /// <summary>
    /// Gets the interpolation function used to calculate full width at half maximum (FWHM) values.
    /// </summary>
    /// <remarks>This property provides access to the underlying interpolation function for FWHM calculations.
    /// It is initialized internally and cannot be set from outside the class.</remarks>
    protected IInterpolationFunction FwhmSpline { get; private set; }


    /// <inheritdoc/>
    public string[] ParameterNamesForOnePeak => [ParameterBaseName0, ParameterBaseName1];

    /// <summary>
    /// Gets the order of the baseline polynomial.
    /// </summary>
    public int OrderOfBaselinePolynomial
    {
      get => field;
      init
      {
        if (!(value >= -1))
          throw new ArgumentOutOfRangeException(nameof(OrderOfBaselinePolynomial), $"{nameof(OrderOfBaselinePolynomial)} must be greater than or equal to 0, or -1 in order to deactivate it.");
        field = value;
      }
    }

    /// <inheritdoc/>
    IFitFunctionPeak IFitFunctionPeak.WithOrderOfBaselinePolynomial(int orderOfBaselinePolynomial)
    {
      return this with { OrderOfBaselinePolynomial = orderOfBaselinePolynomial };
    }

    /// <summary>
    /// Gets the number of peak terms.
    /// </summary>
    public int NumberOfTerms
    {
      get => field;
      init
      {
        if (!(value >= 0))
          throw new ArgumentOutOfRangeException(nameof(NumberOfTerms), $"{nameof(NumberOfTerms)} must be greater than or equal to 0");
        field = value;
      }
    }

    /// <inheritdoc/>
    IFitFunctionPeak IFitFunctionPeak.WithNumberOfTerms(int numberOfTerms)
    {
      return this with { NumberOfTerms = numberOfTerms };
    }

    #region IFitFunction members

    /// <inheritdoc/>
    public int NumberOfIndependentVariables => 1;

    /// <inheritdoc/>
    public int NumberOfDependentVariables => 1;

    /// <inheritdoc/>
    public int NumberOfParameters => NumberOfTerms * NumberOfParametersPerPeak + OrderOfBaselinePolynomial + 1;

    /// <inheritdoc/>
    public event EventHandler? Changed;


    /// <inheritdoc/>
    public double DefaultParameterValue(int i)
    {
      return i switch
      {
        0 => 1, // a
        1 => 0, // xc
        _ => throw new ArgumentOutOfRangeException(nameof(i), $"There are only {NumberOfParameters} parameters, but {i} was requested.")
      };
    }

    /// <inheritdoc/>
    public IVarianceScaling? DefaultVarianceScaling(int i)
    {
      return null;
    }

    /// <inheritdoc/>
    public string DependentVariableName(int i)
    {
      return "x";
    }

    /// <inheritdoc/>
    public string IndependentVariableName(int i)
    {
      return "y";
    }

    /// <inheritdoc/>
    public string ParameterName(int i)
    {
      throw new NotImplementedException();
    }

    #endregion


    /// <summary>
    /// Evaluates one peak term from the table interpolation.
    /// </summary>
    /// <param name="x">The independent variable value.</param>
    /// <param name="a">The amplitude factor.</param>
    /// <param name="xc">The table x-coordinate (center/position) used for interpolation.</param>
    /// <returns>The term value.</returns>
    protected double GetYOfOneTerm(double x, double a, double xc)
    {
      var arg = x - xc;

      if (!RMath.IsInIntervalCC(arg, MinimalX, MaximalX))
        return 0; // outside of the x range of the table, so we assume the peak function is zero there
      else
        return a * Spline.Interpolate(xc, arg);
    }


    /// <inheritdoc/>
    public void Evaluate(double[] X, double[] P, double[] FV)
    {
      if (Spline is null)
        Initialize();

      // evaluation of gaussian terms
      double sumTerms = 0, sumPolynomial = 0;
      for (int i = 0, j = 0; i < NumberOfTerms; ++i, j += NumberOfParametersPerPeak)
      {
        sumTerms += GetYOfOneTerm(X[0], P[j], P[j + 1]);
      }

      if (OrderOfBaselinePolynomial >= 0)
      {
        int offset = NumberOfParametersPerPeak * NumberOfTerms;
        // evaluation of terms x^0 .. x^n
        sumPolynomial = P[OrderOfBaselinePolynomial + offset];
        for (int i = OrderOfBaselinePolynomial - 1; i >= 0; i--)
        {
          sumPolynomial *= X[0];
          sumPolynomial += P[i + offset];
        }
      }
      FV[0] = sumTerms + sumPolynomial;
    }

    /// <inheritdoc/>
    public void Evaluate(IROMatrix<double> independent, IReadOnlyList<double> P, IVector<double> FV, IReadOnlyList<bool>? dependentVariableChoice)
    {
      if (Spline is null)
        Initialize();

      var rowCount = independent.RowCount;
      for (int r = 0; r < rowCount; ++r)
      {
        var x = independent[r, 0];
        // evaluation of gaussian terms
        double sumTerms = 0, sumPolynomial = 0;
        for (int i = 0, j = 0; i < NumberOfTerms; ++i, j += NumberOfParametersPerPeak)
        {
          sumTerms += GetYOfOneTerm(x, P[j], P[j + 1]);
        }

        if (OrderOfBaselinePolynomial >= 0)
        {
          int offset = NumberOfParametersPerPeak * NumberOfTerms;
          // evaluation of terms x^0 .. x^n
          sumPolynomial = P[OrderOfBaselinePolynomial + offset];
          for (int i = OrderOfBaselinePolynomial - 1; i >= 0; i--)
          {
            sumPolynomial *= x;
            sumPolynomial += P[i + offset];
          }
        }
        FV[r] = sumTerms + sumPolynomial;
      }
    }

    /// <inheritdoc/>
    public double[] GetInitialParametersFromHeightPositionAndWidthAtRelativeHeight(double height, double position, double width, double relativeHeight)
    {
      return new[] { height, position };
    }

    /// <inheritdoc/>
    public (IReadOnlyList<double?>? LowerBounds, IReadOnlyList<double?>? UpperBounds) GetParameterBoundariesForPositivePeaks(double? minimalPosition = null, double? maximalPosition = null, double? minimalFWHM = null, double? maximalFWHM = null)
    {
      if (Spline is null)
        Initialize();

      var lowerBounds = new double?[NumberOfParameters];
      var upperBounds = new double?[NumberOfParameters];

      for (int i = 0, j = 0; i < NumberOfTerms; ++i, j += NumberOfParametersPerPeak)
      {
        lowerBounds[j + 0] = 0; // minimal amplitude is 0
        upperBounds[j + 0] = null; // maximal amplitude is not limited

        lowerBounds[j + 1] = minimalPosition.HasValue ? Math.Max(minimalPosition.Value, MinimalPosition) : MinimalPosition;
        upperBounds[j + 1] = maximalPosition.HasValue ? Math.Min(maximalPosition.Value, MaximalPosition) : MaximalPosition;
      }

      return (lowerBounds, upperBounds);
    }

    /// <inheritdoc/>
    public (IReadOnlyList<double?>? LowerBounds, IReadOnlyList<double?>? UpperBounds) GetParameterBoundariesHardLimit()
    {
      if (Spline is null)
        Initialize();

      var lowerBounds = new double?[NumberOfParameters];
      var upperBounds = new double?[NumberOfParameters];

      for (int i = 0, j = 0; i < NumberOfTerms; ++i, j += NumberOfParametersPerPeak)
      {
        lowerBounds[j + 1] = MinimalPosition;
        upperBounds[j + 1] = MaximalPosition;
      }

      return (lowerBounds, upperBounds);
    }

    /// <inheritdoc/>
    public (IReadOnlyList<double?>? LowerBounds, IReadOnlyList<double?>? UpperBounds) GetParameterBoundariesSoftLimit()
    {
      return (null, null);
    }

    /// <inheritdoc/>
    public (double Position, double Area, double Height, double FWHM) GetPositionAreaHeightFWHMFromSinglePeakParameters(IReadOnlyList<double> parameters)
    {
      if (Spline is null)
        Initialize();

      return (Position: parameters[1], Area: parameters[0], Height: parameters[0], FWHM: FwhmSpline.GetYOfX(parameters[1]));
    }

    /// <inheritdoc/>
    public (double Position, double PositionStdDev, double Area, double AreaStdDev, double Height, double HeightStdDev, double FWHM, double FWHMStdDev) GetPositionAreaHeightFWHMFromSinglePeakParameters(IReadOnlyList<double> parameters, IROMatrix<double>? cv)
    {
      if (Spline is null)
        Initialize();

      var h = parameters[0];
      var xc = parameters[1];
      var hStdDev = cv is null ? 0 : SafeSqrt(cv[0, 0]);
      var xcStdDev = cv is null ? 0 : SafeSqrt(cv[1, 1]);
      return (h, hStdDev, xc, xcStdDev, h, hStdDev, FwhmSpline.GetYOfX(xc), Math.Abs(FwhmSpline.GetYOfX(xc + xcStdDev) - FwhmSpline.GetYOfX(xc - xcStdDev)));
    }

    private static double SafeSqrt(double x) => Math.Sqrt(Math.Max(0, x));
  }
}
