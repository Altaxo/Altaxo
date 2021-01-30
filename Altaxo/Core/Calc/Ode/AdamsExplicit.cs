#region Copyright

/////////////////////////////////////////////////////////////////////////////
//
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2021 Dr. Dirk Lellinger
//    This source file is licensed under the MIT license.
//
/////////////////////////////////////////////////////////////////////////////

#endregion Copyright

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Calc.Ode
{
  /// <summary>
  /// Explicit Adams method with constant number of stages (order) and constant step size (no step size control).
  /// </summary>
  /// <remarks>
  /// <para>References:</para>
  /// <para>[1] Hairer et al., Solving Ordinary Differential Equations I (Nonstiff problems), 2nd edition 2008, DOI 10.1007/978-3-540-78862-1</para>
  /// </remarks>
  public partial class AdamsExplicit
  {
    /// <summary>
    /// The number of stages = number of points used to extrapolate the slope.
    /// </summary>
    private int _numberOfStages;

    /// <summary>
    /// Temporary storage for the initialization data. This is neccessary because in order to initialize
    /// the core, we have to make the <see cref="NumberOfStages"/>-1 steps in advance.
    /// </summary>
    private InitializationData? _initialization;

    /// <summary>The core of the method.</summary>
    private Core? _core;

    /// <summary>
    /// Initializes a new instance of the <see cref="AdamsExplicit"/> class.
    /// </summary>
    /// <param name="numberOfStages">The number of stages = number of points used to extrapolate the slope [1..9].</param>
    public AdamsExplicit(int numberOfStages)
    {
      if (!(numberOfStages >= 1))
        throw new ArgumentOutOfRangeException(nameof(numberOfStages), "Must be >= 1");
      else if (!(numberOfStages <= 9))
        throw new ArgumentOutOfRangeException(nameof(numberOfStages), "Must be <= 9");
      _numberOfStages = numberOfStages;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AdamsExplicit"/> class with <see cref="NumberOfStages"/> of 4.
    /// </summary>
    public AdamsExplicit() : this(4)
    {
    }

    /// <summary>
    /// Gets the number of stages = number of points used for extrapolation of the slope to the next point.
    /// The value may range from 1 .. 9.
    /// </summary>
    public int NumberOfStages
    {
      get => _numberOfStages;
    }

    #region Initialization

    /// <summary>
    /// Initializes this explicit Adams method.
    /// </summary>
    /// <param name="x0">The initial x value.</param>
    /// <param name="y0">The initial y values.</param>
    /// <param name="f">Calculation of the derivatives. First argument is x value, 2nd argument are the current y values. The 3rd argument is an array that store the derivatives.</param>
    /// <param name="stepSize">The (fixed) step size used in this method.</param>
    /// <returns>This instance (for a convenient way to chain this method with sequence creation).</returns>
    public AdamsExplicit Initialize(double x0, double[] y0, Action<double, double[], double[]> f, double stepSize)
    {
      if (_core is not null || _initialization is not null)
        throw new InvalidOperationException("ODE is already initialized!");

      _initialization = new InitializationData(x0, (double[])y0.Clone(), f) { StepSize = stepSize };
      return this;
    }

    /// <summary>
    /// Initializes this explicit Adams method.
    /// </summary>
    /// <param name="x0">The initial x value.</param>
    /// <param name="y0">The initial y values.</param>
    /// <param name="f">Calculation of the derivatives. First argument is x value, 2nd argument are the current y values. The 3rd argument is an array that store the derivatives.</param>
    /// <returns>This instance (for a convenient way to chain this method with sequence creation).</returns>
    public AdamsExplicit Initialize(double x0, double[] y0, Action<double, double[], double[]> f)
    {
      if (_core is not null || _initialization is not null)
        throw new InvalidOperationException("ODE is already initialized!");

      _initialization = new InitializationData(x0, (double[])y0.Clone(), f );
      return this;
    }

    #endregion Initialization

    #region Volatile methods

    /// <summary>
    /// Initializes the method and gets volatile solution points for constant step size.
    /// </summary>
    /// <param name="x0">The initial x value.</param>
    /// <param name="y0">The initial y values.</param>
    /// <param name="f">Calculation of the derivatives. First arg is the x variable. 2nd arg are the current y variables. The 3rd argument provides an array, in which the resulting derivatives dyi/dx should be stored.</param>
    /// <param name="stepSize">Size of a step.</param>
    /// <returns>Endless sequence of solution points. You have to consume the values immediately because the content of the y array is changed in the further course of the evaluation.</returns>
    public virtual IEnumerable<(double X, double[] Y_volatile)> GetSolutionPointsVolatileForStepSize(double x0, double[] y0, Action<double, double[], double[]> f, double stepSize)
    {
      if (_core is not null || _initialization is not null)
        throw new InvalidOperationException("This ODE is already initialized!");

      Initialize(x0, y0, f);
      return GetSolutionPointsVolatileForStepSize(stepSize);
    }


    /// <summary>
    /// Initializes the method and gets volatile solution points for constant step size.
    /// </summary>
    /// <param name="stepSize">Size of a step.</param>
    /// <returns>Endless sequence of solution points. You have to consume the values immediately because the content of the y array is changed in the further course of the evaluation.</returns>
    public virtual IEnumerable<(double X, double[] Y_volatile)> GetSolutionPointsVolatileForStepSize(double stepSize)
    {
      if (_core is not null)
        throw new InvalidOperationException($"Core already in use - allocate a new instance!");
      if (_initialization is null)
        throw new InvalidOperationException($"Please call {nameof(Initialize)} before!");

      var x0 = _initialization.X0;
      var y0 = _initialization.Y0;
      var f = _initialization.F;

      var stages = NewJaggedDoubleArray(_numberOfStages, y0.Length);
      var rk = new RK8713M(); // we do not need interpolation, thus we can use RK8713M

      (double x, double[] y) lastSolutionPoint = (x0, y0);
      foreach (var sp in InitializeStages(rk, stepSize, x0, y0, f, stages, false, System.Linq.Enumerable.Empty<double>()))
      {
        yield return sp;
        lastSolutionPoint = sp;
      }

      _core = new Core(_numberOfStages, stepSize, lastSolutionPoint.x, lastSolutionPoint.y, f, stages);
      _initialization = null;

      for (; ; )
      {
        _core.EvaluateNextSolutionPoint();
        yield return (_core.X, _core.Y_volatile);
      }
    }

    /// <summary>
    /// Gets a sequence of solution points, using the settings in the argument. The y-values in the returned tuple
    /// are intended for immediate consumption, because the content of the array will change in the further course of the
    /// evaluation.
    /// </summary>
    /// <param name="options">The evaluation options, see <see cref="RungeKuttaOptions"/>.</param>
    /// <returns>Tuple of the current x, and y values. The y-values are intended for immediate consumption,
    /// because the content of the array will change in the further course of the
    /// evaluation.</returns>
    public virtual IEnumerable<(double X, double[] Y_volatile)> GetSolutionPointsVolatile(RungeKuttaOptions options)
    {
      if (_core is not null)
        throw new InvalidOperationException($"Core already in use - allocate a new instance!");
      if (_initialization is null)
        throw new InvalidOperationException($"Please call {nameof(Initialize)} before!");

      var x0 = _initialization.X0;
      var y0 = _initialization.Y0;
      var f = _initialization.F;

      var stepSize = options.StepSize ?? _initialization.StepSize ?? throw new InvalidOperationException($"Neither options nor initialization contains a specification of the StepSize");

      var stages = NewJaggedDoubleArray(_numberOfStages, y0.Length);
      var rk = new DOP853(); // we do need interpolation, therefore we use DOP853

      (double x, double[] y) lastSolutionPoint = (x0, y0);
      foreach (var sp in InitializeStages(rk, stepSize, x0, y0, f, stages, options.IncludeInitialValueInOutput, options.OptionalSolutionPoints ?? Enumerable.Empty<double>()))
      {
        yield return sp;
        lastSolutionPoint = sp;
      }

      _core = new Core(_numberOfStages, stepSize, lastSolutionPoint.x, lastSolutionPoint.y, f, stages);
      _initialization = null;


      var itOptional = (options.OptionalSolutionPoints ?? Enumerable.Empty<double>()).GetEnumerator();
      var itOptionalAvailable = itOptional.MoveNext();

      for (; ; )
      {
        var x_current = _core.X;
        while (itOptionalAvailable && itOptional.Current < x_current)
        {
          itOptionalAvailable = itOptional.MoveNext();
        }
        var x_next = x_current + stepSize;
        while (itOptionalAvailable && itOptional.Current < x_next)
        {
          double p = (itOptional.Current - x_current) / stepSize;
          yield return (itOptional.Current, _core.GetInterpolatedPoint(p));
          itOptionalAvailable = itOptional.MoveNext();
        }

        _core.EvaluateNextSolutionPoint();
        yield return (_core.X, _core.Y_volatile);
      }
    }

    #endregion Volatile methods

    #region Non Volatile methods

    /// <summary>
    /// Initializes the method and gets volatile solution points for constant step size.
    /// </summary>
    /// <param name="x0">The initial x value.</param>
    /// <param name="y0">The initial y values.</param>
    /// <param name="f">Calculation of the derivatives. First arg is the x variable. 2nd arg are the current y variables. The 3rd argument provides an array, in which the resulting derivatives dyi/dx should be stored.</param>
    /// <param name="stepSize">Size of a step.</param>
    /// <returns>Endless sequence of solution points: tuple of the current x, and y values. The array of y-values is a copy of the solution vector, and is therefore save be to stored permanently.</returns>
    public virtual IEnumerable<(double X, double[] Y_volatile)> GetSolutionPointsForStepSize(double x0, double[] y0, Action<double, double[], double[]> f, double stepSize)
    {
      return GetSolutionPointsVolatileForStepSize(x0, y0, f, stepSize).Select(sp => (sp.X, (double[])sp.Y_volatile.Clone()));
    }

    /// <summary>
    /// Initializes the method and gets solution points for constant step size.
    /// </summary>
    /// <param name="stepSize">Size of a step.</param>
    /// <returns>Endless sequence of solution points: tuple of the current x, and y values. The array of y-values is a copy of the solution vector, and is therefore save be to stored permanently.</returns>
    public virtual IEnumerable<(double X, double[] Y_volatile)> GetSolutionPointsForStepSize(double stepSize)
    {
      return GetSolutionPointsVolatileForStepSize(stepSize).Select(sp => (sp.X, (double[])sp.Y_volatile.Clone()));
    }

    /// <summary>
    /// Gets a sequence of solution points, using the settings in the argument. 
    /// </summary>
    /// <param name="options">The evaluation options, see <see cref="RungeKuttaOptions"/>.</param>
    /// <returns>Endless sequence of solution points: tuple of the current x, and y values. The array of y-values is a copy of the solution vector, and is therefore save be to stored permanently.</returns>
    public virtual IEnumerable<(double X, double[] Y_volatile)> GetSolutionPoints(RungeKuttaOptions options)
    {
      return GetSolutionPointsVolatile(options).Select(sp => (sp.X, (double[])sp.Y_volatile.Clone()));
    }

    #endregion

    #region Helper

    /// <summary>
    /// Evaluates the very first points by using a Runge-Kutta method
    /// </summary>
    /// <param name="rk">The Runga-Kutta method to use.</param>
    /// <param name="stepSize">The step size of the Adams method.</param>
    /// <param name="x0">The initial x value.</param>
    /// <param name="y0">The initial y values.</param>
    /// <param name="f">The function used to calculate the derivatives.</param>
    /// <param name="kstages">Array of stages of the Adams method. In index 0, the <b>last</b> point (slope) that is evaluated by the Runga-Kutta method is stored.</param>
    /// <param name="includeInitialPointInOutput">If set to <c>true</c>, the very first point (x0, y0) is included in the output.</param>
    /// <param name="optionalSolutionPoints">The optional solution points (interpolated points).</param>
    /// <returns>Sequence of solution points of the first <see cref="NumberOfStages"/> points, and optional points.</returns>
    private IEnumerable<(double X, double[] Y_volatile)> InitializeStages(
      RungeKuttaExplicitBase rk,
      double stepSize,
      double x0,
      double[] y0,
      Action<double, double[], double[]> f, double[][] kstages,
      bool includeInitialPointInOutput,
      IEnumerable<double> optionalSolutionPoints)
    {
      if (includeInitialPointInOutput)
      {
        yield return (x0, y0);
      }
      // Initialize the stages
      f(x0, y0, kstages[kstages.Length - 1]);

      if (_numberOfStages > 1) // Initialization of other stages neccessary for _order > 1 
      {

        // Initialize the stages
        rk.Initialize(x0, y0, f);
        int stepDivider = 2;

        var firstXValues = FirstXSequence(stepSize, stepDivider, _numberOfStages);
        var initialSolution = rk.GetSolutionPointsVolatile(new RungeKuttaOptions { AutomaticStepSizeControl = false, MandatorySolutionPoints = firstXValues });


        int nextXIdx = stepDivider - 1;
        int si = kstages.Length - 2;
        var itOptional = optionalSolutionPoints.GetEnumerator();
        bool itOptionalAvailable = itOptional.MoveNext();
        double x_previous = x0;
        foreach (var s in initialSolution)
        {
          while (itOptionalAvailable && itOptional.Current < x_previous)
          {
            itOptionalAvailable = itOptional.MoveNext();
          }

          while (itOptionalAvailable && itOptional.Current <= s.X)
          {
            yield return (itOptional.Current, rk.GetInterpolatedSolutionPointVolatile(itOptional.Current));
            itOptionalAvailable = itOptional.MoveNext();
          }

          if (s.X == firstXValues[nextXIdx])
          {
            f(s.X, s.Y_volatile, kstages[si--]);
            nextXIdx += stepDivider;
            yield return (s);
            if (si < 0)
              break;
          }

          x_previous = s.X;
        }
      }
    }


    /// <summary>
    /// Creates a new jagged double array.
    /// </summary>
    /// <param name="i">First dimension of the array (spine dimension).</param>
    /// <param name="k">Second dimension of the array.</param>
    /// <returns></returns>
    private static double[][] NewJaggedDoubleArray(int i, int k)
    {
      var result = new double[i][];
      for (int n = 0; n < result.Length; ++n)
        result[n] = new double[k];

      return result;
    }

    /// <summary>
    /// Creates an array of mandatory points used by the Runge-Kutta method to evaluate the first p
    /// </summary>
    /// <param name="stepSize">Size of the Adams metho step.</param>
    /// <param name="stepDivider">The step divider. Used to divide one Adams step in sub steps in order to
    /// improve the accuracy of the Runge-Kutta solution points.</param>
    /// <param name="numberOfStages">Number of stages of the Adams method.</param>
    /// <returns>The x values of the mandatory solution points for the Runge-Kutta method.</returns>
    private static double[] FirstXSequence(double stepSize, int stepDivider, int numberOfStages)
    {
      var firstXValues = new double[stepDivider * (numberOfStages - 1)];
      for (int i = 0, destIdx = 0; i < numberOfStages - 1; ++i)
      {
        for (int j = 1; j < stepDivider; ++j)
          firstXValues[destIdx++] = ((i * stepDivider + j) * stepSize) / stepDivider;
        firstXValues[destIdx++] = (i + 1) * stepSize;
      }
      return firstXValues;
    }


    #endregion
  }
}
