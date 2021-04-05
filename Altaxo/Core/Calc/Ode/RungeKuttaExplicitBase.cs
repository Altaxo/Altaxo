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
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Altaxo.Calc.LinearAlgebra;
using Altaxo.Science.Thermodynamics.Fluids;

namespace Altaxo.Calc.Ode
{
  /// <summary>
  /// Base class for explicit Runge-Kutta methods.
  /// </summary>
  /// <remarks>
  /// <para>References:</para>
  /// <para>[1] Hairer, Ordinary differential equations I, 2nd edition, 1993.</para>
  /// <para>[2] Jimenez et al., Locally Linearized Runge Kutta method of Dormand and Prince, arXiv:1209.1415v2, 22 Dec 2013</para>
  /// </remarks>
  public abstract partial class RungeKuttaExplicitBase
  {
    /// <summary>An empty double array intended for initialization of arrays.</summary>
    protected static readonly double[] _emptyDoubleArray = new double[0];

    /// <summary>An empty jagged double array intended for initialization of arrays.</summary>
    protected static readonly double[][] _emptyJaggedDoubleArray = new double[0][];

    /// <summary>
    /// Gets the order of the method (the highest of the pair).
    /// </summary>
    public abstract int Order { get; }

    /// <summary>
    /// Gets the number of stages for the main process (stages needed for dense output not included).
    /// </summary>
    public abstract int NumberOfStages { get; }

    /// <summary>
    /// Gets the number of stages of additional stages needed for dense output.
    /// </summary>
    public virtual int NumberOfAdditionalStagesForDenseOutput => 0;

    /// <summary>Central coefficients of the Runge-Kutta scheme. See [1], page 135.</summary>
    protected abstract double[][] A { get; }

    /// <summary>High order bottom side coefficients of the Runge-Kutta scheme.</summary>
    protected abstract double[] BH { get; }

    /// <summary>Differences between high order and low order bottom side coefficients of the Runge-Kutta scheme.</summary>
    protected abstract double[]? BHML { get; }

    /// <summary>Left side coefficients of the Runge-Kutta scheme.</summary>
    protected abstract double[] C { get; }

    /// <summary>
    /// The interpolation coefficients aij. Note that zero to third order interpolation is using y and slope of x_previous and x_current.
    /// Thus in this array we only need the coefficients for 4th order (and higher) interpolation.
    /// </summary>
    protected virtual double[][] InterpolationCoefficients => _emptyJaggedDoubleArray;

    /// <summary>
    /// Gets additional central coefficients of the Runge-Kutta scheme that are used for dense output (interpolation).
    /// </summary>
    protected virtual double[][] A_Interpolation => _emptyJaggedDoubleArray;

    /// <summary>
    /// Gets additional left side coefficients of the Runge-Kutta scheme (x-partitions) that are used for dense output (interpolation).
    /// </summary>
    protected virtual double[] C_Interpolation => _emptyDoubleArray;

    /// <summary>
    /// Sets the stiffness detection threshold value.
    /// </summary>
    /// <value>
    /// The stiffness detection threshold value.
    /// </value>
    protected abstract double StiffnessDetectionThresholdValue { get; }

    /// <summary>
    /// The solver core.
    /// </summary>
    protected Core? _core;

    /// <summary>
    /// Initializes the Runge-Kutta method.
    /// </summary>
    /// <param name="x">The initial x value.</param>
    /// <param name="y">The initial y values.</param>
    /// <param name="f">Calculation of the derivatives. First argument is x value, 2nd argument are the current y values. The 3rd argument is an array that store the derivatives.</param>
    /// <returns>This instance (for a convenient way to chain this method with sequence creation).</returns>
    [MemberNotNull(nameof(_core))]
    public virtual RungeKuttaExplicitBase Initialize(double x, double[] y, Action<double, double[], double[]> f)
    {
      _core = new Core(Order, NumberOfStages, NumberOfAdditionalStagesForDenseOutput, A, BH, BHML, C, x, y, f);
      if (InterpolationCoefficients is not null)
      {
        _core.InterpolationCoefficients = InterpolationCoefficients;
      }
      if (A_Interpolation.Length > 0)
      {
        _core.SetCoefficientsForAdditionalStages(A_Interpolation, C_Interpolation);
      }

      _core.StiffnessDetectionThresholdValue = StiffnessDetectionThresholdValue;

      return this;
    }

    /// <summary>
    /// Initializes the method and gets volatile solution points for constant step size.
    /// </summary>
    /// <param name="x0">The initial x value.</param>
    /// <param name="y0">The initial y values.</param>
    /// <param name="f">Calculation of the derivatives. First arg is the x variable. 2nd arg are the current y variables. The 3rd argument provides an array, in which the resulting derivatives dyi/dx should be stored.</param>
    /// <param name="stepSize">Size of a step.</param>
    /// <returns>Endless sequence of solution points. You have to consume the values immediately because the content of the y array is changed in the further course of the evaluation.</returns>
    public virtual IEnumerable<(double X, double[] Y_volatile)> GetSolutionPointsVolatileForStepSize(
         double x0, double[] y0, Action<double, double[], double[]> f, double stepSize)
    {
      Initialize(x0, y0, f);
      for (long i = 1; ; ++i)
      {
        _core.EvaluateNextSolutionPoint(x0 + i * stepSize);
        _core.ThrowIfStiffnessDetected();
        yield return (_core.X, _core.Y_volatile);
      }
    }

    /// <summary>
    /// Gets volatile solution points for constant step size. The method has to be initialized (see <see cref="Initialize(double, double[], Action{double, double[], double[]})"/>) before.
    /// </summary>
    /// <param name="stepSize">Size of a step.</param>
    /// <returns>Endless sequence of solution points. You have to consume the values immediately because the content of the y array is changed in the further course of the evaluation.</returns>
    public virtual IEnumerable<(double X, double[] Y_volatile)> GetSolutionPointsVolatileForStepSize(
          double stepSize)
    {
      if (_core is null)
        throw NewCoreNotInitializedException;

      double x0 = _core.X;
      for (long i = 1; ; ++i)
      {
        _core.EvaluateNextSolutionPoint(x0 + i * stepSize);
        _core.ThrowIfStiffnessDetected();
        yield return (_core.X, _core.Y_volatile);
      }
    }

    /// <summary>
    /// Gets solution points for constant step size. Returns the same results as <see cref="GetSolutionPointsVolatileForStepSize(double, double[], Action{double, double[], double[]}, double)"/>,
    /// but the returned solution point already contains a copy of the y array.
    /// </summary>
    /// <param name="x0">The initial x value.</param>
    /// <param name="y0">The initial y values.</param>
    /// <param name="f">Calculation of the derivatives. First arg is the x variable. 2nd arg are the current y variables. The 3rd argument provides an array, in which the resulting derivatives dyi/dx should be stored.</param>
    /// <param name="stepSize">Size of a step.</param>
    /// <returns>Endless sequence of solution points. It is safe to store the returned y array permanently.</returns>
    public virtual IEnumerable<(double x, double[] y)> GetSolutionPointsForStepSize(
      double x0, double[] y0, Action<double, double[], double[]> f, double stepSize)
    {
      return GetSolutionPointsVolatileForStepSize(x0, y0, f, stepSize).Select(sp => (sp.X, Clone(sp.Y_volatile)));
    }

    /// <summary>
    /// Gets solution points for constant step size. Returns the same results as <see cref="GetSolutionPointsVolatileForStepSize(double, double[], Action{double, double[], double[]}, double)"/>,
    /// but the returned solution point already contains a copy of the y array.
    /// </summary>
    /// <param name="stepSize">Size of a step.</param>
    /// <returns>Endless sequence of solution points. It is safe to store the returned y array permanently.</returns>
    public virtual IEnumerable<(double x, double[] y)> GetSolutionPointsForStepSize(double stepSize)
    {
      return GetSolutionPointsVolatileForStepSize(stepSize).Select(sp => (sp.X, Clone(sp.Y_volatile)));
    }

    /// <summary>
    /// Provides the core with the parameters found in the <paramref name="options"/>.
    /// </summary>
    /// <param name="options">The options to set.</param>
    protected virtual void SetOptionsToCore(OdeMethodOptions options)
    {
      if (_core is null)
        throw NewCoreNotInitializedException;

      options.CheckConsistency();
      _core.AbsoluteTolerances = Clone(options.AbsoluteTolerances);
      _core.RelativeTolerances = Clone(options.RelativeTolerances);
      _core.StepSizeFilter = options.StepSizeFilter;
      _core.StiffnessDetectionEveryNumberOfSteps = options.StiffnessDetectionEveryNumberOfSteps;
      _core.ErrorNorm = options.ErrorNorm;

    }

    /// <summary>
    /// Gets a sequence of solution points, using the settings in the argument. The y-values in the returned tuple
    /// are intended for immediate consumption, because the content of the array will change in the further course of the
    /// evaluation.
    /// </summary>
    /// <param name="options">The evaluation options, see <see cref="OdeMethodOptions"/>.</param>
    /// <returns>Tuple of the current x, and y values. The y-values are intended for immediate consumption,
    /// because the content of the array will change in the further course of the
    /// evaluation.</returns>
    public virtual IEnumerable<(double X, double[] Y_volatile)> GetSolutionPointsVolatile(OdeMethodOptions options)
    {
      if (_core is null)
        throw NewCoreNotInitializedException;

      SetOptionsToCore(options);

      if (options.IncludeInitialValueInOutput)
        yield return (_core.X, _core.Y_volatile);


      if (options.AutomaticStepSizeControl)
      {
        foreach (var sp in GetSolutionPointsVolatile_WithStepSizeControl(options))
          yield return sp;
      }
      else
      {
        foreach (var sp in GetSolutionPointsVolatile_WithoutStepSizeControl(options))
          yield return sp;
      }
    }

    /// <summary>
    /// Gets a sequence of solution points, using the settings in the argument. 
    /// </summary>
    /// <param name="options">The evaluation options, see <see cref="OdeMethodOptions"/>.</param>
    /// <returns>Tuple of the current x, and y values. The array of y-values is a copy of the solution vector, and is therefore save be to stored permanently.</returns>
    public virtual IEnumerable<(double X, double[] Y)> GetSolutionPoints(OdeMethodOptions options)
    {
      return GetSolutionPointsVolatile(options).Select(sp => (sp.X, Clone(sp.Y_volatile)));
    }

    /// <summary>
    /// Gets you an interpolated volative solution point during the enumeration of the solution points.
    /// The returned array must not be modified and has to be immediately consumed, since it is changed in the course of the next ODE evaluation.
    /// </summary>
    /// <param name="x">The x value. Must be in the interval [X-StepSize, X].</param>
    /// <returns>The interpolated y values. The elements of the array must not be altered, and are intended for immediate use only.</returns>
    public virtual double[] GetInterpolatedSolutionPointVolatile(double x)
    {
      if (_core is null)
        throw NewCoreNotInitializedException;

      if (!(_core.X_previous <= x && x <= _core.X))
        throw new Exception($"Can only get interpolated point in the interval between x_previous={_core.X_previous} and x_current={_core.X}, but the argument was {x}");

      double theta = (x - _core.X_previous) / (_core.X - _core.X_previous);
      return _core.GetInterpolatedY_volatile(theta);
    }

    /// <summary>
    /// Gets the initial step size. The absolute and relative tolerances must be set before the call to this function.
    /// </summary>
    /// <returns>The initial step size in the context of the absolute and relative tolerances.</returns>
    /// <exception cref="InvalidOperationException">Either absolute tolerance or relative tolerance is required to be &gt; 0</exception>
    public double GetInitialStepSize()
    {
      if (_core is null)
        throw NewCoreNotInitializedException;

      return _core.GetInitialStepSize();
    }

    /// <summary>
    /// Gets or sets the absolute tolerance.
    /// </summary>
    /// <value>
    /// The absolute tolerance.
    /// </value>
    /// <exception cref="ArgumentException">Must be >= 0 - AbsoluteTolerance</exception>
    public double AbsoluteTolerance
    {
      get
      {
        if (_core is null)
          throw NewCoreNotInitializedException;

        return _core.AbsoluteTolerance;
      }
      set
      {
        if (_core is null)
          throw NewCoreNotInitializedException;
        if (!(value >= 0))
          throw new ArgumentException("Must be >= 0", nameof(AbsoluteTolerance));

        _core.AbsoluteTolerance = value;
      }
    }

    /// <summary>
    /// Gets or sets the relative tolerance.
    /// </summary>
    /// <value>
    /// The relative tolerance.
    /// </value>
    /// <exception cref="ArgumentException">Must be >= 0 - RelativeTolerance</exception>
    public double RelativeTolerance
    {
      get
      {
        if (_core is null)
          throw NewCoreNotInitializedException;

        return _core.RelativeTolerance;
      }
      set
      {
        if (_core is null)
          throw NewCoreNotInitializedException;
        if (!(value >= 0))
          throw new ArgumentException("Must be >= 0", nameof(RelativeTolerance));

        _core.RelativeTolerance = value;
      }
    }

    /// <summary>
    /// Gets or sets the relative tolerances. The length of the array must either be 1 (tolerances for all y equal), or of length N.
    /// </summary>
    /// <value>
    /// The relative tolerances.
    /// </value>
    public double[] RelativeTolerances
    {
      get
      {
        if (_core is null)
          throw NewCoreNotInitializedException;

        return Clone(_core.RelativeTolerances);
      }
      set
      {
        if (_core is null)
          throw NewCoreNotInitializedException;

        _core.RelativeTolerances = Clone(value);
      }
    }

    /// <summary>
    /// Gets or sets the absolute tolerances. The length of the array must either be 1 (tolerances for all y equal), or of length N.
    /// </summary>
    /// <value>
    /// The absolute tolerances.
    /// </value>
    public double[] AbsoluteTolerances
    {
      get
      {
        if (_core is null)
          throw NewCoreNotInitializedException;

        return Clone(_core.AbsoluteTolerances);
      }
      set
      {
        if (_core is null)
          throw NewCoreNotInitializedException;

        _core.AbsoluteTolerances = Clone(value);
      }
    }

    /// <summary>
    /// Gets or sets the number of successful steps between test for stiffness.
    /// Setting this value to 0 disables stiffness detection. The default value is 0.
    /// </summary>
    /// <value>
    /// The number of successful steps between test for stiffness.
    /// </value>
    public int StiffnessDetectionEveryNumberOfSteps
    {
      get
      {
        if (_core is null)
          throw NewCoreNotInitializedException;
        return _core.StiffnessDetectionEveryNumberOfSteps;
      }
      set
      {
        if (_core is null)
          throw NewCoreNotInitializedException;
        _core.StiffnessDetectionEveryNumberOfSteps = value;
      }
    }

    /// <summary>
    /// Gets volatile solution points with step size control.
    /// </summary>
    /// <param name="options">The evaluation options.</param>
    /// <returns></returns>
    protected virtual IEnumerable<(double X, double[] Y_volatile)> GetSolutionPointsVolatile_WithStepSizeControl(OdeMethodOptions options)
    {
      if (_core is null)
        throw NewCoreNotInitializedException;
      SetOptionsToCore(options);

      double stepSize;
      if (options.InitialStepSize.HasValue)
        stepSize = options.InitialStepSize.Value;
      else
        stepSize = _core.GetInitialStepSize();

      var itMandatory = options.MandatorySolutionPoints?.GetEnumerator();
      if (itMandatory is not null && !itMandatory.MoveNext())
      {
        itMandatory.Dispose();
        itMandatory = null;
      }

      var itOptional = options.OptionalSolutionPoints?.GetEnumerator();
      if (itOptional is not null && !itOptional.MoveNext())
      {
        itOptional.Dispose();
        itOptional = null;
      }


      double error_previous = 1;
      double error_current;

      for (; options.IncludeAutomaticStepsInOutput || itMandatory is not null || itOptional is not null;)
      {
        var x_current = _core.X;

        bool isStepToMandatorySolutionPoint;
        do // do one step. If automatic step size control is on, repeat until target accuracy is reached.
        {
          double effectiveStepSize;
          double x_nextStepEffective;

          double x_next = x_current + stepSize;

          // forward mandatory solution point
          if (itMandatory is not null)
          {
            while (!(itMandatory.Current > x_current))
            {
              if (!(itMandatory.MoveNext()))
              {
                itMandatory.Dispose();
                itMandatory = null;
                break;
              }
            }
          }

          // Calculate effective step size.
          // If mandatory solution point comes first, then use it instead of the recommended step size.
          // If mandatory solution point is expected in the interval after this interval, then part the interval to the mandatory solution point in two equal parts
          // Otherwise, use the recommended step size.
          if (itMandatory is not null)
          {
            if (itMandatory.Current <= x_next)
            {
              // Mandatory point is in this interval => use a step to the mandatory point
              x_nextStepEffective = itMandatory.Current;
              effectiveStepSize = itMandatory.Current - x_current;
              isStepToMandatorySolutionPoint = true;
            }
            else if (itMandatory.Current < (x_next + stepSize))
            {
              // Mandatory point is in the interval after this interval => use two equal steps to the mandatory point
              x_nextStepEffective = 0.5 * (itMandatory.Current + x_current);
              effectiveStepSize = 0.5 * (itMandatory.Current - x_current);
              isStepToMandatorySolutionPoint = false;
            }
            else
            {
              // In all other cases: use recommend step size
              x_nextStepEffective = x_current + stepSize;
              effectiveStepSize = stepSize;
              isStepToMandatorySolutionPoint = false;
            }
          }
          else
          {
            // if there are no mandatory points: use recommended step size
            x_nextStepEffective = x_current + stepSize;
            effectiveStepSize = stepSize;
            isStepToMandatorySolutionPoint = false;
          }

          // Make the step with the effective step size.
          _core.EvaluateNextSolutionPoint(x_nextStepEffective);

          error_current = _core.ErrorNorm switch
          {
            ErrorNorm.L2Norm => _core.GetRelativeError_L2Norm(),
            ErrorNorm.InfinityNorm => _core.GetRelativeError_InfinityNorm(),
            _ => throw new NotImplementedException()
          };

          if (double.IsNaN(error_current) || double.IsInfinity(error_current))
            error_current = 10; // if error is NaN then probably the step size is too big, we force to reduce step size by setting error to 10
          stepSize = _core.GetRecommendedStepSize(error_current, error_previous);
          if (options.MaxStepSize.HasValue && options.MaxStepSize.Value < stepSize)
            stepSize = options.MaxStepSize.Value;

          if (error_current > 1)
          {
            error_previous = error_current;
            _core.Revert();
          }

          if ((_core.X + stepSize * 0.125) == _core.X)
            throw new InvalidOperationException($"Minimum step size reached. Step size now is {stepSize}");

        } while (error_current > 1);
        error_previous = error_current;

        _core.ThrowIfStiffnessDetected();

        bool includeTrueSolutionPointInOutput = (isStepToMandatorySolutionPoint && options.IncludeMandatorySolutionPointsInOutput) || options.IncludeAutomaticStepsInOutput;

        // if needed, output the optional interpolated points
        while (itOptional is not null && itOptional.Current <= _core.X) // move it3 until its greater than this step
        {
          var rel = (itOptional.Current - _core.X_previous) / (_core.X - _core.X_previous);
          if (itOptional.Current < _core.X || !includeTrueSolutionPointInOutput) // output only if x it is truely smaller than currently evaluated step, except in the case that no true solution points are output
            yield return (itOptional.Current, _core.GetInterpolatedY_volatile(rel));
          if (!itOptional.MoveNext())
          {
            itOptional.Dispose();
            itOptional = null;
          }
        }

        // output the true solution point if required
        if (includeTrueSolutionPointInOutput)
        {
          yield return (_core.X, _core.Y_volatile); // yield the true (not interpolated) solution point
        }
      }
    }


    /// <summary>
    /// Gets volatile solution points without step size control.
    /// </summary>
    /// <param name="options">The evaluation options.</param>
    /// <returns></returns>
    protected virtual IEnumerable<(double X, double[] Y_volatile)> GetSolutionPointsVolatile_WithoutStepSizeControl(OdeMethodOptions options)
    {
      if (_core is null)
        throw NewCoreNotInitializedException;
      SetOptionsToCore(options);

      var itFixedStep = options.StepSize.HasValue ? EnumerateXForFixedStepSize(_core.X, options.StepSize.Value).GetEnumerator() : null;
      var itMandatory = options.MandatorySolutionPoints?.GetEnumerator();

      var itOptional = options.OptionalSolutionPoints?.GetEnumerator();

      if (itFixedStep is not null && !itFixedStep.MoveNext())
      {
        itFixedStep.Dispose();
        itFixedStep = null;
      }
      if (itMandatory is not null && !itMandatory.MoveNext())
      {
        itMandatory.Dispose();
        itMandatory = null;
      }
      if (itOptional is not null && !itOptional.MoveNext())
      {
        itOptional.Dispose();
        itOptional = null;
      }

      double x_next;

      while (TryGetNextValue(ref itFixedStep, ref itMandatory, out x_next))
      {
        _core.EvaluateNextSolutionPoint(x_next);
        _core.ThrowIfStiffnessDetected();

        // if needed, output the optional interpolated points
        while (itOptional is not null && itOptional.Current <= _core.X) // move it3 until its greater than this step
        {
          var rel = (itOptional.Current - _core.X_previous) / (_core.X - _core.X_previous);
          if (itOptional.Current < _core.X) // output only if x it is truely smaller than currently evaluated step
            yield return (itOptional.Current, _core.GetInterpolatedY_volatile(rel));
          if (!itOptional.MoveNext())
          {
            itOptional.Dispose();
            itOptional = null;
          }
        }

        // now yield the true solution point
        yield return (_core.X, _core.Y_volatile);
      }

      itFixedStep?.Dispose();
      itMandatory?.Dispose();
    }

    /// <summary>
    /// Try to get the smaller value of the two enumerations. After that, the enumeration with the smaller value is advanced by one step.
    /// </summary>
    /// <param name="it1">The first enumeration.</param>
    /// <param name="it2">The second enumeration.</param>
    /// <param name="value">If successfull, the smaller value of the two enumerations; otherwise <see cref="double.NaN"/>.</param>
    /// <returns>True if a value could be retrieved at least of one of the enumerations; otherwise, false.</returns>
    protected bool TryGetNextValue(ref IEnumerator<double>? it1, ref IEnumerator<double>? it2, out double value)
    {
      if (it1 is not null && it2 is not null)
      {
        value = Math.Min(it1.Current, it2.Current);
        if (it1.Current < it2.Current)
        {
          if (!it1.MoveNext())
          {
            it1.Dispose();
            it1 = null;
          }
        }
        else if (it2.Current < it1.Current)
        {
          if (!it2.MoveNext())
          {
            it2.Dispose();
            it2 = null;
          }
        }
        else if (it1.Current == it2.Current)
        {
          if (!it1.MoveNext())
          {
            it1.Dispose();
            it1 = null;
          }
          if (!it2.MoveNext())
          {
            it2.Dispose();
            it2 = null;
          }
        }
        else
        {
          throw new InvalidOperationException($"The case that it1.Current is {it1.Current} and it2.Current is {it2.Current } is not supported.");
        }
        return true;
      }
      else if (it1 is not null)
      {
        value = it1.Current;
        if (!it1.MoveNext())
        {
          it1.Dispose();
          it1 = null;
        }
        return true;
      }
      else if (it2 is not null)
      {
        value = it2.Current;
        if (!it2.MoveNext())
        {
          it2.Dispose();
          it2 = null;
        }
        return true;
      }
      else
      {
        value = double.NaN;
        return false;
      }
    }

    #region Helpers

    /// <summary>
    /// Creates a new exception that indicates that the core is not initialized.
    /// </summary>
    protected InvalidOperationException NewCoreNotInitializedException => new InvalidOperationException($"Core is not initialized. Please call {nameof(Initialize)} first!");

    /// <summary>
    /// Enumerates the endless sequence <paramref name="x_current"/> + k * <paramref name="stepSize"/>, for k=1..Infinity.
    /// </summary>
    /// <param name="x_current">The base value.</param>
    /// <param name="stepSize">The step size.</param>
    /// <returns>The endless sequence <paramref name="x_current"/> + k * <paramref name="stepSize"/>, for k=1..Infinity.</returns>
    protected IEnumerable<double> EnumerateXForFixedStepSize(double x_current, double stepSize)
    {
      for (long i = 1; ; ++i)
      {
        yield return x_current + stepSize * i;
      }
    }

    #endregion

    #region Static helper methods

    /// <summary>
    /// Exchanges the two instances in the argument.
    /// </summary>
    /// <typeparam name="T">The type of objects to exchange.</typeparam>
    /// <param name="instance1">The instance1.</param>
    /// <param name="instance2">The instance2.</param>
    public static void Exchange<T>(ref T instance1, ref T instance2)
    {
      T temp = instance1;
      instance1 = instance2;
      instance2 = temp;
    }

    /// <summary>
    /// Clones an array of <see cref="double"/>.
    /// </summary>
    /// <param name="toClone">The array to clone.</param>
    /// <returns>Cloned array.</returns>
    public static double[] Clone(double[] toClone)
    {
      var result = new double[toClone.Length];
      Array.Copy(toClone, result, toClone.Length);
      return result;
    }

    #endregion

  }
}
