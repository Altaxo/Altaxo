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

namespace Altaxo.Calc.Ode
{

  /// <summary>
  /// Options for explicit Runge-Kutta methods.
  /// </summary>
  /// <remarks>
  /// Essentially there are two modes for explicit Runge-Kutta methods:
  /// <para> i) With automatic step size control (<see cref="AutomaticStepSizeControl"/> = true) and</para>
  /// <para>ii) Without automatic step size control (<see cref="AutomaticStepSizeControl"/> = false)</para>
  /// <para><b>With</b> automatic step size control, the following parameters are relevant:
  /// <list type="">
  /// <item><see cref="IncludeInitialValueInOutput"/> determines whether the initial values for x and y should appear as the first item in the output sequence.</item>
  /// <item><see cref="AbsoluteTolerance"/> and <see cref="RelativeTolerance"/> and/or <see cref="AbsoluteTolerances"/> and <see cref="RelativeTolerances"/> determine the chosen step size.</item>
  /// <item><see cref="MaxStepSize"/> determines the maximum applied step size.</item>
  /// <item><see cref="IncludeAutomaticStepsInOutput"/> determines whether the steps that are chosen automatically should appear in the sequence of solution points.</item>
  /// <item><see cref="InitialStepSize"/> determines the initial step size. If set to <see langword="null"/>, a reasonable guess for the initial step size will be made.</item>
  /// <item><see cref="StepSizeFilter"/> determines the variations of the step size.</item>
  /// <item><see cref="MandatorySolutionPoints"/> are points where the method is forced to have a solution point. At those points the evaluation of the derivative is forced.</item>
  /// <item><see cref="IncludeMandatorySolutionPointsInOutput"/> determines whether the mandatory solution points should appear in the output sequence (default: true).</item>
  /// <item><see cref="OptionalSolutionPoints"/> are points that are evaluated by interpolation between true solution points. Optional solution points always appear in the output sequence.</item>
  /// </list>
  /// </para>
  /// <para><b>Without</b> automatic step size control, the following parameters are relevant:
  /// <list type="">
  /// <item><see cref="IncludeInitialValueInOutput"/> determines whether the initial values for x and y should appear as the first item in the output sequence.</item>
  /// <item><see cref="StepSize"/> determines the fixed step size. Set this parameter to <see langword="null"/> if you only want to output <see cref="MandatorySolutionPoints"/>.</item>
  /// <item><see cref="MandatorySolutionPoints"/> are points where the method is forced to have a solution point. At those points the evaluation of the derivative is forced. In mode without automatic step size control, mandatory solution points always appear in the output sequence.</item>
  /// <item><see cref="OptionalSolutionPoints"/> are points that are evaluated by interpolation between true solution points. Optional solution points always appear in the output sequence.</item>
  /// </list>
  /// </para>
  /// </remarks>
  public class OdeMethodOptions
  {
    /// <summary>
    /// Gets or sets a value indicating whether the initial point should be included in the output.
    /// </summary>
    /// <value>
    /// <see langword="true"/> if the initial point should be included in the output; otherwise, <see langword="false"/>.
    /// </value>
    public bool IncludeInitialValueInOutput { get; set; }

    /// <summary>
    /// Gets or sets optional solution points.
    /// Optional solution points are not evaluated directly, but are interpolated between two real solution points.
    /// </summary>
    /// <value>
    /// The sequence of optional solution points. Must be a strictly increasing sequence.
    /// </value>
    public IEnumerable<double>? OptionalSolutionPoints { get; set; }



    /// <summary>
    /// Gets or sets the step size.
    /// This value is effective only if automatic step size control is not active.
    /// </summary>
    /// <value>
    /// The size of one step. In addition to this value, you can set further mandatory evaluation points by setting <see cref="MandatorySolutionPoints"/>.
    /// </value>
    /// <exception cref="ArgumentException">Thrown if the assigned value is negative.</exception>
    public double? StepSize
    {
      get => _stepSize;
      set
      {
        if (value.HasValue && !(value.Value >= 0))
          throw new ArgumentException($"Either {nameof(StepSize)} must be null, or a value > 0", nameof(StepSize));

        if (value.HasValue && value == 0)
          value = null;
        _stepSize = value;
      }
    }
    protected double? _stepSize;




    private double[] _relativeTolerances = new double[] { 0 };
    private double[] _absoluteTolerances = new double[] { 0 };

    /// <summary>
    /// Gets or sets the absolute tolerance for all <c>y</c> values.
    /// Use <see cref="AbsoluteTolerances"/> if you want to set the absolute tolerance for each individual <c>y</c> value.
    /// </summary>
    /// <value>
    /// The absolute tolerance for all <c>y</c> values.
    /// </value>
    /// <exception cref="ArgumentException">Thrown if the assigned value is negative.</exception>
    /// <exception cref="InvalidOperationException">Thrown if <see cref="AbsoluteTolerances"/> is not a scalar (length is not 1).</exception>
    public double AbsoluteTolerance
    {
      get
      {
        if (_absoluteTolerances.Length == 1)
          return _absoluteTolerances[0];
        else
          throw new InvalidOperationException($"{nameof(AbsoluteTolerance)} is an array and not a scalar.");
      }
      set
      {
        if (!(value >= 0))
          throw new ArgumentException("Must be >= 0", nameof(AbsoluteTolerance));
        _absoluteTolerances = new double[1] { value };
      }
    }

    /// <summary>
    /// Gets or sets the relative tolerance for all <c>y</c> values.
    /// Use <see cref="RelativeTolerances"/> if you want to set the relative tolerance for each individual <c>y</c> value.
    /// </summary>
    /// <value>
    /// The relative tolerance.
    /// </value>
    /// <exception cref="ArgumentException">Thrown if the assigned value is negative.</exception>
    /// <exception cref="InvalidOperationException">Thrown if <see cref="RelativeTolerances"/> is not a scalar (length is not 1).</exception>
    public double RelativeTolerance
    {
      get
      {
        if (_relativeTolerances.Length == 1)
          return _relativeTolerances[0];
        else
          throw new InvalidOperationException($"{nameof(RelativeTolerance)} is an array and not a scalar.");
      }
      set
      {
        if (!(value >= 0))
          throw new ArgumentException("Must be >= 0", nameof(RelativeTolerance));
        _relativeTolerances = new double[1] { value };
      }
    }

    /// <summary>
    /// Gets or sets the absolute tolerances.
    /// The length of the array must either be 1 (equal tolerances for all <c>y</c> values) or <c>N</c>.
    /// </summary>
    /// <value>
    /// The absolute tolerances.
    /// </value>
    /// <exception cref="ArgumentNullException">Thrown if the assigned value is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown if any element of the assigned array is negative.</exception>
    public double[] AbsoluteTolerances
    {
      get => _absoluteTolerances;
      set
      {
        if (value is null)
          throw new ArgumentNullException(nameof(AbsoluteTolerances));
        _absoluteTolerances = (double[])value.Clone();

        for (int i = 0; i < _absoluteTolerances.Length; ++i)
        {
          if (!(_absoluteTolerances[i] >= 0))
            throw new ArgumentException($"Element {i} must be >= 0", nameof(AbsoluteTolerances));
        }
      }
    }

    /// <summary>
    /// Gets or sets the relative tolerances.
    /// The length of the array must either be 1 (equal tolerances for all <c>y</c> values) or <c>N</c>.
    /// </summary>
    /// <value>
    /// The relative tolerances.
    /// </value>
    /// <exception cref="ArgumentNullException">Thrown if the assigned value is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown if any element of the assigned array is negative.</exception>
    public double[] RelativeTolerances
    {
      get => _relativeTolerances;
      set
      {
        if (value is null)
          throw new ArgumentNullException(nameof(RelativeTolerances));
        _relativeTolerances = (double[])value.Clone();

        for (int i = 0; i < _relativeTolerances.Length; ++i)
        {
          if (!(_relativeTolerances[i] >= 0))
            throw new ArgumentException($"Element {i} must be >= 0", nameof(RelativeTolerances));
        }

      }
    }




    /// <summary>
    /// Gets or sets a value indicating whether steps generated by the automatic step size control should be included in the output.
    /// This value is <see langword="true"/> by default.
    /// The value is effective only if <see cref="AutomaticStepSizeControl"/> is set to <see langword="true"/>.
    /// If <see cref="AutomaticStepSizeControl"/> is <see langword="true"/> but this value is <see langword="false"/>, then <see cref="MandatorySolutionPoints"/> must be set.
    /// </summary>
    /// <value>
    /// <see langword="true"/> if steps generated by automatic step size control should be included in the output; otherwise, <see langword="false"/>.
    /// </value>
    public bool IncludeAutomaticStepsInOutput { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether automatic step size control is enabled.
    /// </summary>
    /// <value>
    /// <see langword="true"/> if the step size is adjusted automatically according to the absolute and relative error; otherwise, <see langword="false"/>.
    /// </value>
    public bool AutomaticStepSizeControl { get; set; }

    private double? _maxStepSize;

    /// <summary>
    /// Gets or sets the maximum step size.
    /// This value is effective only if <see cref="AutomaticStepSizeControl"/> is <see langword="true"/>.
    /// </summary>
    /// <value>
    /// The maximum step size when the step size is evaluated automatically.
    /// </value>
    /// <exception cref="ArgumentException">Thrown if the assigned value is negative.</exception>
    public double? MaxStepSize
    {
      get => _maxStepSize;
      set
      {
        if (value.HasValue && !(value.Value >= 0))
          throw new ArgumentException($"Either {nameof(MaxStepSize)} must be null, or a value > 0", nameof(MaxStepSize));

        if (value.HasValue && value == 0)
          value = null;
        _maxStepSize = value;
      }
    }


    private double? _initialStepSize;

    /// <summary>
    /// Gets or sets the initial step size.
    /// This value is effective only if <see cref="AutomaticStepSizeControl"/> is <see langword="true"/>.
    /// </summary>
    /// <value>
    /// The initial step size. If set to <see langword="null"/>, the initial step size is calculated automatically.
    /// </value>
    /// <exception cref="ArgumentException">Thrown if the assigned value is negative.</exception>
    public double? InitialStepSize
    {
      get => _initialStepSize;
      set
      {
        if (value.HasValue && !(value.Value >= 0))
          throw new ArgumentException($"Either {nameof(InitialStepSize)} must be null, or a value > 0", nameof(InitialStepSize));

        if (value.HasValue && value == 0)
          value = null;
        _initialStepSize = value;
      }
    }




    private StepSizeFilter _stepSizeFilter;
    /// <summary>
    /// Gets or sets the step size filter (determines the variation of step sizes), see <see cref="StepSizeFilter"/>.
    /// </summary>
    public StepSizeFilter StepSizeFilter
    {
      get => _stepSizeFilter;
      set
      {
        _stepSizeFilter = value;
      }
    }

    /// <summary>
    /// Stores the error norm that is used for the evaluation of the relative error in order to control the step size.
    /// </summary>
    private ErrorNorm _errorNorm;

    /// <summary>
    /// Gets or sets the error norm that is used for the evaluation of the relative error in order to control the step size.
    /// </summary>
    public ErrorNorm ErrorNorm
    {
      get => _errorNorm;
      set
      {
        _errorNorm = value;
      }
    }



    /// <summary>
    /// Gets or sets the mandatory solution points.
    /// Mandatory solution points are evaluated directly (i.e. not interpolated).
    /// </summary>
    /// <value>
    /// The sequence of mandatory solution points.
    /// If <see cref="StepSize"/> is set too, the sequence of solution points is the
    /// result of zipping the mandatory solution point sequence with the sequence of <c>k * StepSize</c>.
    /// </value>
    public IEnumerable<double>? MandatorySolutionPoints { get; set; }

    /// <summary>
    /// Default: <see langword="true"/>.
    /// Gets or sets a value indicating whether the mandatory solution points (see <see cref="MandatorySolutionPoints"/>)
    /// should appear in the output sequence.
    /// This value is only effective if <see cref="AutomaticStepSizeControl"/> is <see langword="true"/>.
    /// Without automatic step size control, mandatory solution points always appear in the output sequence.
    /// </summary>
    /// <value>
    /// <see langword="true"/> if the mandatory solution points should appear in the output sequence (default value); otherwise, <see langword="false"/>.
    /// </value>
    public bool IncludeMandatorySolutionPointsInOutput { get; set; } = true;

    private int _stiffnessDetectionEveryNumberOfSteps;

    /// <summary>
    /// Gets or sets the number of successful steps between tests for stiffness.
    /// Setting this value to 0 disables stiffness detection. The default value is 0.
    /// </summary>
    /// <value>
    /// The number of successful steps between tests for stiffness.
    /// </value>
    public int StiffnessDetectionEveryNumberOfSteps
    {
      get
      {
        return _stiffnessDetectionEveryNumberOfSteps;
      }
      set
      {
        if (!(value >= 0))
          throw new ArgumentOutOfRangeException(nameof(StiffnessDetectionEveryNumberOfSteps), "Must be >=0");
        _stiffnessDetectionEveryNumberOfSteps = value;
      }
    }



    /// <summary>
    /// Checks the consistency of the options.
    /// An <see cref="InvalidOperationException"/> is thrown if some of the parameters exclude each other.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if the options are inconsistent.</exception>
    public virtual void CheckConsistency()
    {
      if (AutomaticStepSizeControl)
      {
        double maxtol = 0;
        for (int i = 0; i < _absoluteTolerances.Length; ++i)
          maxtol = Math.Max(maxtol, _absoluteTolerances[i]);
        for (int i = 0; i < _relativeTolerances.Length; ++i)
          maxtol = Math.Max(maxtol, _relativeTolerances[i]);
        if (!(maxtol > 0))
          throw new InvalidOperationException($"Automatic step size control requires that tolerances have been set.");

        if (_stepSize is not null)
          throw new InvalidOperationException($"{nameof(StepSize)} without effect because {nameof(AutomaticStepSizeControl)} is true.");

        if (IncludeAutomaticStepsInOutput == false && (MandatorySolutionPoints is null || !IncludeMandatorySolutionPointsInOutput) && OptionalSolutionPoints is null)
          throw new InvalidOperationException($"If {nameof(IncludeAutomaticStepsInOutput)} is false, then at least either {nameof(MandatorySolutionPoints)} or {nameof(OptionalSolutionPoints)} must be set!");

        if (InitialStepSize is null)
        {
          if (!(maxtol > 0))
            throw new InvalidOperationException($"Evaluation of initial step size requires that tolerances have been set.");
        }
      }
      else // no automatic step size control
      {
        if (StepSize is null && MandatorySolutionPoints is null)
          throw new InvalidOperationException($"If {nameof(AutomaticStepSizeControl)} is {AutomaticStepSizeControl}, then at least either {nameof(StepSize)} or {nameof(MandatorySolutionPoints)} must be set!");
      }
    }


    /// <summary>
    /// Gets an equidistant sequence that can be used, e.g. for <see cref="OptionalSolutionPoints"/> or <see cref="MandatorySolutionPoints"/>.
    /// </summary>
    /// <param name="start">The first value of the sequence.</param>
    /// <param name="step">The difference between consecutive values.</param>
    /// <param name="count">The number of values to generate. If you leave this parameter out, the sequence is unbounded.</param>
    /// <returns>An enumerable that yields the generated sequence.</returns>
    public static IEnumerable<double> GetEquidistantSequence(double start, double step, long count = long.MaxValue)
    {
      for (long i = 0; i < count; ++i)
        yield return start + step * i;
    }
  }

  /// <summary>
  /// Designates the filter method for calculation of the recommended step size of the next step.
  /// </summary>
  /// <remarks>
  /// See Table 1 in [Söderlind, Adaptive Time-Stepping and Computational Stability, 2003].
  /// </remarks>
  public enum StepSizeFilter
  {
    /// <summary>
    /// The H211b digital filter (b=4).
    /// Takes the current relative error, the previous relative error, and the current and previous step sizes into account.
    /// </summary>
    H211b,

    /// <summary>
    /// The PI4.2 digital filter.
    /// Takes the current relative error and the previous relative error into account.
    /// </summary>
    PI_4_2,

    /// <summary>
    /// Elementary controller (not recommended).
    /// Takes only the current relative error into account.
    /// </summary>
    Elementary
  }

  /// <summary>
  /// Designates the norm used to calculate the local error.
  /// </summary>
  public enum ErrorNorm
  {
    /// <summary>
    /// Use the L2 norm of the relative errors.
    /// </summary>
    L2Norm,

    /// <summary>
    /// Use the infinity norm of the relative errors.
    /// </summary>
    InfinityNorm
  }
}
