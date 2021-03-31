
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Altaxo.Calc.Ode.Obsolete.DVode;

namespace Altaxo.Calc.Ode.Obsolete
{
  /// <summary>
  /// Solves an initial-value problem for nonstiff ordinary differential equations using
  /// the Adams-Moulton method.
  /// dy(i)/dt = f(i,t,y(1),y(2),...,y(N)).
  /// </summary>
  public sealed class OdeAdamsMoulton : xBaseOdeGearsAndAdamsMoulton
  {
    #region Constructor

    /// <summary>
    /// Initializes a new instance of the OdeAdamsMoulton class.
    /// </summary>
    public OdeAdamsMoulton()
    {
    }

    #endregion Constructor

    #region Methods

    /// <summary>
    /// Initializes a new instance of the OdeAdamsMoulton class.
    /// </summary>
    /// <param name="function">A function that evaluates the right side of the differential equations.</param>
    /// <param name="numEquations">The number of differential equations.</param>
    public OdeAdamsMoulton(OdeFunction function, int numEquations)
    {
      base.InitializationWithoutJacobian(function, ODEType.NonStiff, numEquations);

      _InvokeSetInitialValues = true;
    }

    /// <summary>
    /// Method that initialize the ODE to solve.
    /// </summary>
    /// <param name="function">A function that evaluates the right side of the differential equations.</param>
    /// <param name="numEquations">The number of differential equations.</param>
    public override void InitializeODEs(OdeFunction function, int numEquations)
    {
      base.InitializationWithoutJacobian(function, ODEType.NonStiff, numEquations);

      _InvokeSetInitialValues = true;
    }

    /// <summary>
    /// Method that initialize the ODE to solve.
    /// </summary>
    /// <param name="function">A function that evaluates the right side of the differential equations.</param>
    /// <param name="numEquations">The number of differential equations.</param>
    /// <param name="t0">The initial value for the independent variable.</param>
    /// <param name="y0">A vector of size N containing the initial conditions. N is the number of differential equations.</param>
    public override void InitializeODEs(OdeFunction function, int numEquations, double t0, double[] y0)
    {
      base.InitializationWithoutJacobian(function, ODEType.NonStiff, numEquations);
      SetInitialValues(t0, y0);
    }

    #endregion Methods
  }
}
