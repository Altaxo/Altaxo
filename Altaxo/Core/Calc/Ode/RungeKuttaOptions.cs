#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2020 Dr. Dirk Lellinger
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

namespace Altaxo.Calc.Ode
{
  /// <summary>
  /// Options for explicit Runge-Kutta methods.
  /// </summary>
  /// <remarks>
  /// Essentially there are two modi for explicit Runge-Kutta methods:
  /// <para> i) With automatic step size control (<see cref="AutomaticStepSizeControl"/> = true) and</para>
  /// <para>ii) Without automatic step size control (<see cref="AutomaticStepSizeControl"/> = false)</para>
  /// <para><b>With</b> automatic step size control, the following parameters are relevant:
  /// <list type="">
  /// <item><see cref="IncludeInitialValueInOutput"/> determines whether the inital values for x and y should appear as first item in the output sequence.</item>
  /// <item><see cref="AbsoluteTolerance"/> and <see cref="RelativeTolerance"/> determine the chosen step size.</item>
  /// <item><see cref="MaxStepSize"/> determines the maximum applied step size.</item>
  /// <item><see cref="IncludeAutomaticStepsInOutput"/> if the steps that are chosen automatically should appear in the sequence of solution points.</item>
  /// <item><see cref="InitialStepSize"/> determines the initial step size. If set to null, a reasonable guess of the initial step size will be done.</item>
  /// <item><see cref="MandatorySolutionPoints"/> are points where the method is forced to have a solution point. At those points the evaluation of the derivative is forced.</item>
  /// <item><see cref="IncludeMandatorySolutionPointsInOutput"/> determines if the mandatory solution points should appear in the output sequence (default: true).</item>
  /// <item><see cref="OptionalSolutionPoints"/> are points that are evaluated by interpolation between true solution points. Optional solution points always appear in the output sequence.</item>
  /// </list>
  /// </para>
  /// <para><b>Without</b> automatic step size control, the following parameters are relevant:
  /// <list type="">
  /// <item><see cref="IncludeInitialValueInOutput"/> determines whether the inital values for x and y should appear as first item in the output sequence.</item>
  /// <item><see cref="StepSize"/> determines the fixed step size. Set this parameter to null if you only want to output <see cref="MandatorySolutionPoints"/>.</item>
  /// <item><see cref="MandatorySolutionPoints"/> are points where the method is forced to have a solution point. At those points the evaluation of the derivative is forced. In mode without automatic step size control, mandatory solution points always appear in the output sequence.</item>
  /// <item><see cref="OptionalSolutionPoints"/> are points that are evaluated by interpolation between true solution points. Optional solution points always appear in the output sequence.</item>
  /// </list>
  /// </para>
  /// </remarks>
  public class RungeKuttaOptions
  {
    private double _relativeTolerance;
    private double _absoluteTolerance;

    /// <summary>
    /// Gets or sets the absolute tolerance.
    /// </summary>
    /// <value>
    /// The absolute tolerance.
    /// </value>
    /// <exception cref="ArgumentException">Must be &gt;= 0 - AbsoluteTolerance</exception>
    public double AbsoluteTolerance
    {
      get => _absoluteTolerance;
      set
      {
        if (!(value >= 0))
          throw new ArgumentException("Must be >= 0", nameof(AbsoluteTolerance));
        _absoluteTolerance = value;
      }
    }

    /// <summary>
    /// Gets or sets the relative tolerance.
    /// </summary>
    /// <value>
    /// The relative tolerance.
    /// </value>
    /// <exception cref="ArgumentException">Must be &gt;= 0 - RelativeTolerance</exception>
    public double RelativeTolerance
    {
      get => _relativeTolerance;
      set
      {
        if (!(value >= 0))
          throw new ArgumentException("Must be >= 0", nameof(RelativeTolerance));
        _relativeTolerance = value;
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the inital point should be included in the output.
    /// </summary>
    /// <value>
    ///   <c>true</c> if the initial point should be included in the output; otherwise, <c>false</c>.
    /// </value>
    public bool IncludeInitialValueInOutput { get; set; }


    /// <summary>
    /// Gets or sets a value indicating whether steps generated by the automatic step size control should be included in the output.
    /// This value is true by default. The value is effective only if <see cref="AutomaticStepSizeControl"/> is set to true.
    /// If <see cref="AutomaticStepSizeControl"/> is true, but this value is false, then <see cref="MandatorySolutionPoints"/> have to be set.
    /// </summary>
    /// <value>
    /// <c>true</c> if steps generated by automatic step size control should be included in the output; otherwise, <c>false</c>.
    /// </value>
    public bool IncludeAutomaticStepsInOutput { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether automatic step size control is switched on.
    /// </summary>
    /// <value>
    /// <c>true</c> if the step size is automatically adjusted according to the absolute and relative error; otherwise, <c>false</c>.
    /// </value>
    public bool AutomaticStepSizeControl { get; set; }

    private double? _maxStepSize;

    /// <summary>
    /// Gets or sets the maximum size of the step. This value is effective only if <see cref="AutomaticStepSizeControl"/> is true.
    /// </summary>
    /// <value>
    /// The maximum size of a step, if the step size is evaluated automatically.
    /// </value>
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
    /// Gets or sets the initial step size. This value is effective only if <see cref="AutomaticStepSizeControl"/> is true.
    /// </summary>
    /// <value>
    /// The initial step size. If set to null, the initial step size is calculated automatically.
    /// </value>
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

    private double? _stepSize;

    /// <summary>
    /// Gets or sets the size of the step. This value is effective only if <see cref="AutomaticStepSizeControl"/> is false.
    /// </summary>
    /// <value>
    /// The size of one step. Additionally to this value, you can set further mandatory evaluation points by setting <see cref="MandatorySolutionPoints"/>.
    /// </value>
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

    /// <summary>
    /// Gets or sets optional solution points. Optional solution points will be not evaluated directly, but interpolated between two real solution points.
    /// </summary>
    /// <value>
    /// The sequence of optional solution points. Must be a strongly increasing sequence.
    /// </value>
    public IEnumerable<double>? OptionalSolutionPoints { get; set; }

    /// <summary>
    /// Gets or sets the mandatory solution points. Mandatory solution points will be evaluated directly (i.e. not interpolated).
    /// </summary>
    /// <value>
    /// The sequence of mandatory solution points. If <see cref="StepSize"/> is set too, the sequence of solution points is the
    /// result of zipping the mandatory solution point sequence with the sequence of k*StepSize.
    /// </value>
    public IEnumerable<double>? MandatorySolutionPoints { get; set; }

    /// <summary>
    /// Default: true. Gets or sets a value indicating whether the mandatory solution points (see <see cref="MandatorySolutionPoints"/>)
    /// should appear in the output sequence. This value is only effective if <see cref="AutomaticStepSizeControl"/> is true.
    /// Without automatic step size control, mandatory solution points always appear in the output sequence.
    /// </summary>
    /// <value>
    /// <c>true</c> if the mandatory solution points should appear in the output sequence (default value); otherwise, <c>false</c>.
    /// </value>
    public bool IncludeMandatorySolutionPointsInOutput { get; set; } = true;



    /// <summary>
    /// Checks the consistency of the options. An <see cref="InvalidOperationException"/> is thrown if
    /// some of the parameters exclude each other.
    /// </summary>
    public void CheckConsistency()
    {
      if (AutomaticStepSizeControl)
      {
        if (!((_absoluteTolerance > 0 && _relativeTolerance >= 0) || (_absoluteTolerance >= 0 && _relativeTolerance > 0)))
          throw new InvalidOperationException($"Automatic step size control requires that tolerances have been set.");
        if (_stepSize is not null)
          throw new InvalidOperationException($"{nameof(StepSize)} without effect because {nameof(AutomaticStepSizeControl)} is true.");

        if (IncludeAutomaticStepsInOutput == false && (MandatorySolutionPoints is null || !IncludeMandatorySolutionPointsInOutput) && OptionalSolutionPoints is null)
          throw new InvalidOperationException($"If {nameof(IncludeAutomaticStepsInOutput)} is false, then at least either {nameof(MandatorySolutionPoints)} or {nameof(OptionalSolutionPoints)} must be set!");

        if (InitialStepSize is null)
        {
          if (!((_absoluteTolerance > 0 && _relativeTolerance >= 0) || (_absoluteTolerance >= 0 && _relativeTolerance > 0)))
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
    /// Gets an equidistant sequence, that can be used e.g. for <see cref="OptionalSolutionPoints"/> or <see cref="MandatorySolutionPoints"/>.
    /// </summary>
    /// <param name="start">The first value of the sequence.</param>
    /// <param name="step">The difference between the values.</param>
    /// <param name="count">The number of values to generate. If you leave this parameter out, the sequence is not bounded.</param>
    /// <returns></returns>
    public static IEnumerable<double> GetEquidistantSequence(double start, double step, long count = long.MaxValue)
    {
      for (long i = 0; i < count; ++i)
        yield return start + step * i;
    }
  }
}
