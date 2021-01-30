using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Calc.Ode
{
  /// <summary>
  /// Explicit Adams method with constant order and constant step size (no step size control).
  /// </summary>
  /// <remarks>
  /// <para>References:</para>
  /// <para>[1] Hairer et al., Solving Ordinary Differential Equations I (Nonstiff problems), 2nd edition 2008, DOI 10.1007/978-3-540-78862-1</para>
  /// </remarks>
  public partial class AdamsExplicit
  {
    private int _order = 4;
    private Core? _core;
    private InitializationData? _initialization;

    public AdamsExplicit(int numberOfStages)
    {
      if (!(numberOfStages >= 1))
        throw new ArgumentOutOfRangeException(nameof(numberOfStages), "Must be >= 1");
      else if (!(numberOfStages <= 9))
        throw new ArgumentOutOfRangeException(nameof(numberOfStages), "Must be <= 9");
      _order = numberOfStages;
    }

    public AdamsExplicit() : this(4)
    {
    }

    public int NumberOfStages
    {
      get => _order;
    }


    public void Initialize(double stepSize, double x, double[] y, Action<double, double[], double[]> f)
    {
      if (_core is not null || _initialization is not null)
        throw new InvalidOperationException("ODE is already initialized!");

      _initialization = new InitializationData() { StepSize = stepSize, X0 = x, Y0 = (double[])y.Clone(), F = f };
    }

    public void Initialize(double x, double[] y, Action<double, double[], double[]> f)
    {
      if (_core is not null || _initialization is not null)
        throw new InvalidOperationException("ODE is already initialized!");

      _initialization = new InitializationData() { X0 = x, Y0 = (double[])y.Clone(), F = f };
    }

    private static double[] FirstXSequence(double stepSize, int stepDivider, int order)
    {
      var firstXValues = new double[stepDivider * (order - 1)];
      for (int i = 0, destIdx = 0; i < order - 1; ++i)
      {
        for (int j = 1; j < stepDivider; ++j)
          firstXValues[destIdx++] = ((i * stepDivider + j) * stepSize) / stepDivider;
        firstXValues[destIdx++] = (i + 1) * stepSize;
      }
      return firstXValues;
    }


    private IEnumerable<(double X, double[] Y_volatile)> InitializeStages(
      RungeKuttaExplicitBase rk,
      double stepSize,
      double x,
      double[] y,
      Action<double, double[], double[]> f, double[][] kstages,
      bool includeInitialPointInOutput,
      IEnumerable<double> optionalSolutionPoints)
    {
      if (includeInitialPointInOutput)
      {
        yield return (x, y);
      }
      // Initialize the stages
      f(x, y, kstages[kstages.Length - 1]);

      if (_order > 1) // Initialization of other stages neccessary for _order > 1 
      {

        // Initialize the stages
        rk.Initialize(x, y, f);
        int stepDivider = 2;

        var firstXValues = FirstXSequence(stepSize, stepDivider, _order);
        var initialSolution = rk.GetSolutionPointsVolatile(new RungeKuttaOptions { AutomaticStepSizeControl = false, MandatorySolutionPoints = firstXValues });


        int nextXIdx = stepDivider - 1;
        int si = kstages.Length - 2;
        var itOptional = optionalSolutionPoints.GetEnumerator();
        bool itOptionalAvailable = itOptional.MoveNext();
        double x_previous = x;
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
    /// Initializes the method and gets volatile solution points for constant step size.
    /// </summary>
    /// <param name="x0">The initial x value.</param>
    /// <param name="y0">The initial y values.</param>
    /// <param name="f">Calculation of the derivatives. First arg is the x variable. 2nd arg are the current y variables. The 3rd argument provides an array, in which the resulting derivatives dyi/dx should be stored.</param>
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

      var stages = NewJaggedDoubleArray(_order, y0.Length);
      var rk = new RK8713M(); // we do not need interpolation, thus we can use RK8713M

      (double x, double[] y) lastSolutionPoint = (x0, y0);
      foreach (var sp in InitializeStages(rk, stepSize, x0, y0, f, stages, false, System.Linq.Enumerable.Empty<double>()))
      {
        yield return sp;
        lastSolutionPoint = sp;
      }

      _core = new Core(_order, stepSize, lastSolutionPoint.x, lastSolutionPoint.y, f, stages);

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

      var stages = NewJaggedDoubleArray(_order, y0.Length);
      var rk = new DOP853(); // we do need interpolation, therefore we use DOP853

      (double x, double[] y) lastSolutionPoint = (x0, y0);
      foreach (var sp in InitializeStages(rk, stepSize, x0, y0, f, stages, options.IncludeInitialValueInOutput, options.OptionalSolutionPoints ?? Enumerable.Empty<double>()))
      {
        yield return sp;
        lastSolutionPoint = sp;
      }

      _core = new Core(_order, stepSize, lastSolutionPoint.x, lastSolutionPoint.y, f, stages);


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
          yield return (itOptional.Current, _core.EvaluateInterpolatedPoint(p));
          itOptionalAvailable = itOptional.MoveNext();
        }

        _core.EvaluateNextSolutionPoint();
        yield return (_core.X, _core.Y_volatile);
      }
    }

    #region Helper

    private static double[][] NewJaggedDoubleArray(int i, int k)
    {
      var result = new double[i][];
      for (int n = 0; n < result.Length; ++n)
        result[n] = new double[k];

      return result;
    }

    #endregion
  }
}
