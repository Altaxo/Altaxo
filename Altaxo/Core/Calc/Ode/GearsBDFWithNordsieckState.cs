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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Calc.Ode
{
  /// <summary>
  /// Implements Gear's method for integration of stiff ordinary differential equations with step size and order adjustments, using Nordsieck's state.
  /// </summary>
  /// <seealso cref="Altaxo.Calc.Ode.MultiStepMethodBase" />
  /// <remarks>
  /// <para>References:</para>
  /// <para>[1] Byrne and Hindmarsh, A Polyalgorithm for the Numerical Solution of Ordinary Differential Equations, 1975, <see href=" https://doi.org/10.1145/355626.355636"/>.</para>
  /// </remarks>
  public partial class GearsBDFWithNordsieckState : MultiStepMethodBase
  {
    private const double DBL_EPSILON = DoubleConstants.DBL_EPSILON;
    private const double OnePlusDBL_EPSILON = 1 + DoubleConstants.DBL_EPSILON;
    private const double OneMinusDBL_EPSILON = 1 - DoubleConstants.DBL_EPSILON;


    private Core? _core;

    /// <summary>
    /// Initializes this method.
    /// </summary>
    /// <param name="x0">The initial <c>x</c> value.</param>
    /// <param name="y0">The initial <c>y</c> values.</param>
    /// <param name="f">
    /// Evaluates the first derivative <c>dy/dx</c>.
    /// The first argument is the <c>x</c> value, the second argument is the current <c>y</c> vector,
    /// and the third argument is the destination array to store the derivatives.
    /// </param>
    /// <param name="jacobianEvaluation">
    /// Evaluates the Jacobian.
    /// If provided, it is used to evaluate the Jacobian.
    /// If <see langword="null"/>, the behavior depends on the further options:
    /// if <see cref="MultiStepMethodOptions.IterationMethod"/> is set to <see cref="OdeIterationMethod.UseJacobian"/>, 
    /// then the Jacobian is approximated using finite differences; if it is set to
    /// <see cref="OdeIterationMethod.DoNotUseJacobian"/>, then only iteration (and not Newton-Raphson) is used to evaluate
    /// <c>y_next</c>, which can take longer.
    /// </param>
    /// <returns>This instance (for a convenient way to chain this method with sequence creation).</returns>
    public GearsBDFWithNordsieckState Initialize(double x0, double[] y0, Action<double, double[], double[]> f, CalculateJacobian? jacobianEvaluation)
    {
      if (_core is not null || _initialization is not null)
        throw new InvalidOperationException("ODE is already initialized!");

      _initialization = new InitializationData(x0, (double[])y0.Clone(), f) { EvaluateJacobian = jacobianEvaluation };
      return this;
    }


    /// <summary>
    /// The y-values and derivatives (.. kth) at the initialization point.
    /// This array is intended for debugging and testing purposes only.
    /// </summary>
    private double[][]? _initDerivatives;

    /// <summary>
    /// This initialization method is intended for debugging and testing purposes only, because you will need
    /// the derivatives up to order <c>k</c> at the initial point.
    /// </summary>
    /// <param name="x0">The starting value of the independent variable <c>x</c>.</param>
    /// <param name="initDerivatives">
    /// The starting values of the 0th .. <c>k</c>-th derivatives of <c>y</c>, i.e. <c>y</c>, <c>dy/dx</c>, <c>d2y/dx2</c>, etc.
    /// </param>
    /// <param name="f">The function used to evaluate the first derivatives <c>dy/dx</c>.</param>
    /// <param name="jacobianEvaluation">The function used to evaluate the Jacobian.</param>
    /// <returns>This instance.</returns>
    /// <exception cref="InvalidOperationException">ODE is already initialized.</exception>
    public GearsBDFWithNordsieckState Initialize(double x0, double[][] initDerivatives, Action<double, double[], double[]> f, CalculateJacobian jacobianEvaluation)
    {
      if (_core is not null || _initialization is not null)
        throw new InvalidOperationException("ODE is already initialized!");

      _initDerivatives = initDerivatives;
      _initialization = new InitializationData(x0, (double[])initDerivatives[0].Clone(), f) { EvaluateJacobian = jacobianEvaluation };
      return this;
    }


    /// <summary>
    /// Gets the number of steps taken so far.
    /// </summary>
    public int NumberOfStepsTaken
    {
      get
      {
        return _core is null ? 0 : _core.NumberOfStepsTaken;
      }
    }

    /// <summary>
    /// Gets the number of jacobian evaluations so far.
    /// </summary>
    public int NumberOfJacobianEvaluations
    {
      get
      {
        return _core is null ? 0 : _core.NumberOfJacobianEvaluations;
      }
    }

    /// <summary>
    /// Gets volatile solution points with step size control.
    /// </summary>
    /// <param name="options">The evaluation options (preferably of type <see cref="MultiStepMethodOptions"/>).</param>
    /// <returns>
    /// A sequence of solution points, either mandatory, optional, or automatically generated (depending on the settings in <paramref name="options"/>).
    /// </returns>
    public virtual IEnumerable<(double X, double[] Y_volatile)> GetSolutionPointsVolatile(OdeMethodOptions options)
    {
      if (_initialization is null)
        throw new InvalidOperationException("Ode is not initialized. Call Initialize before!");
      if (_core is not null)
        throw new InvalidOperationException("This Ode is alreay in use.");


      if (_initDerivatives is not null)
      {
        _core = new Core(
        _initialization.X0,
        _initDerivatives,
        _initialization.F,
        _initialization.EvaluateJacobian,
        options
        );
      }
      else
      {


        _core = new Core(
          _initialization.X0,
          _initialization.Y0,
          _initialization.F,
          _initialization.EvaluateJacobian,
          options
          );

      }

      // forward mandatory solution point
      var itMandatory = EnumerationInitialize(options.MandatorySolutionPoints);
      var itOptional = EnumerationInitialize(options.OptionalSolutionPoints);

      EnumerationForwardToGreaterThanOrEqual(ref itMandatory, _core.X);
      if (options.IncludeInitialValueInOutput || _core.X == itMandatory?.Current)
      {
        yield return (_core.X, _core.Y_volatile);
      }
      EnumerationForwardToGreaterThan(ref itMandatory, _core.X);

      // before we can start, we need an initial step size (the core was originally initialized with step size 1)
      var initStepSize = options.InitialStepSize ?? _core.GetInitialStepSize(1);
      var (h_init, x_init) = _core.OptimizeStepSizeForNextMandatoryPoint(itMandatory?.Current, initStepSize);
      _core.ChangeStepSize(h_init, x_init);

      // now loop to calculate the sequence of solution points
      for (; options.IncludeAutomaticStepsInOutput || itMandatory is not null || itOptional is not null;)
      {
        bool isMandatorySolutionPoint;

        // Calculate effective step size.
        // If mandatory solution point comes first, then use it instead of the recommended step size.
        // If mandatory solution point is expected in the interval after this interval, then part the interval to the mandatory solution point in two equal parts
        // Otherwise, use the recommended step size.
        if (itMandatory is not null)
        {
          _core.EvaluateNextSolutionPoint(itMandatory.Current);
          isMandatorySolutionPoint = itMandatory.Current == _core.X;
        }
        else
        {
          _core.EvaluateNextSolutionPoint(null);
          isMandatorySolutionPoint = false;
        }

        // output optional solution points
        double lastXOutput = double.NegativeInfinity;
        if (itOptional is not null)
        {
          while (!(itOptional.Current > _core.X))
          {
            yield return (itOptional.Current, _core.GetInterpolatedY_volatile(itOptional.Current));
            lastXOutput = itOptional.Current;
            if (!(itOptional.MoveNext()))
            {
              itOptional.Dispose();
              itOptional = null;
              break;
            }
          }
        }

        if (
            (isMandatorySolutionPoint && options.IncludeMandatorySolutionPointsInOutput) ||
            (!isMandatorySolutionPoint && options.IncludeAutomaticStepsInOutput)
          )
        {
          if (!AreEqual(_core.X, lastXOutput, 1E-15))
          {
            yield return (_core.X, _core.Y_volatile);
          }
        }


        // forward mandatory solution point
        EnumerationForwardToGreaterThan(ref itMandatory, _core.X);


        double? nextMandatorySolutionX = (itMandatory is not null) ? itMandatory.Current : null;
        _core.AdjustStepSizeAndOrder(nextMandatorySolutionX);
      }
    }

    /// <summary>
    /// Gets an interpolated volatile solution point during the enumeration of the solution points.
    /// </summary>
    /// <param name="x">The <c>x</c> value. Must be in the interval <c>[X - StepSize, X]</c>.</param>
    /// <returns>
    /// The interpolated <c>y</c> values.
    /// The returned array must not be modified and has to be consumed immediately, since it is changed in the course of the next ODE evaluation.
    /// </returns>
    public double[] GetInterpolatedSolutionPointVolatile(double x)
    {
      if (_core is null)
        throw new InvalidOperationException("Core is not initialized");

      return _core.GetInterpolatedY_volatile(x);
    }
  }
}
