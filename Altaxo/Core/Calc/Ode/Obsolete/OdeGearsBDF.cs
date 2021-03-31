#region Copyright © 2009, De Santiago-Castillo JA. All rights reserved.

//Copyright © 2009 Jose Antonio De Santiago-Castillo
//E-mail:JAntonioDeSantiago@gmail.com
//Web: www.DotNumerics.com
//

#endregion Copyright © 2009, De Santiago-Castillo JA. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Altaxo.Calc.Ode.Obsolete.DVode;

namespace Altaxo.Calc.Ode.Obsolete
{
  /// <summary>
  /// Solves an initial-value problem for stiff ordinary differential equations using
  /// the Gear’s BDF method.
  /// dy(i)/dt = f(i,t,y(1),y(2),...,y(N)).
  /// </summary>
  [Obsolete]
  public sealed class OdeGearsBDF : xBaseOdeGearsAndAdamsMoulton
  {
    #region Constructor

    /// <summary>
    /// Initializes a new instance of the OdeGearsBDF class.
    /// </summary>
    public OdeGearsBDF()
    {
    }

    /// <summary>
    /// Initializes a new instance of the OdeGearsBDF class.
    /// </summary>
    /// <param name="function">A function that evaluates the right side of the differential equations.</param>
    /// <param name="numEquations">The number of differential equations.</param>
    public OdeGearsBDF(OdeFunction function, int numEquations)
    {
      base.InitializationWithoutJacobian(function, ODEType.Stiff, numEquations);
    }

    /// <summary>
    /// Initializes a new instance of the OdeGearsBDF class.
    /// </summary>
    /// <param name="function">A function that evaluates the right side of the differential equations.</param>
    /// <param name="jacobian">A function that evaluates the jacobian matrix.</param>
    /// <param name="numEquations">The number of differential equations.</param>
    public OdeGearsBDF(OdeFunction function, OdeJacobian jacobian, int numEquations)
    {
      base.InitializationWithJacobian(function, jacobian, numEquations);
    }

    #endregion Constructor

    #region Methods

    /// <summary>
    /// Method that initialize the ODE to solve.
    /// </summary>
    /// <param name="function">A function that evaluates the right side of the differential equations.</param>
    /// <param name="numEquations">The number of differential equations.</param>
    public override void InitializeODEs(OdeFunction function, int numEquations)
    {
      base.InitializationWithoutJacobian(function, ODEType.Stiff, numEquations);

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
      base.InitializationWithoutJacobian(function, ODEType.Stiff, numEquations);
      SetInitialValues(t0, y0);
    }

    /// <summary>
    /// Method that initialize the ODE to solve.
    /// </summary>
    /// <param name="function">A function that evaluates the right side of the differential equations.</param>
    /// <param name="jacobian">A function that evaluates the jacobian matrix.</param>
    /// <param name="numEquations">The number of differential equations.</param>
    public void InitializeODEs(OdeFunction function, OdeJacobian jacobian, int numEquations)
    {
      base.InitializationWithJacobian(function, jacobian, numEquations);

      _InvokeSetInitialValues = true;
    }

    /// <summary>
    /// Method that initialize the ODE to solve.
    /// </summary>
    /// <param name="function">A function that evaluates the right side of the differential equations.</param>
    /// <param name="jacobian">A function that evaluates the jacobian matrix.</param>
    /// <param name="numEquations">The number of differential equations.</param>
    /// <param name="t0">The initial value for the independent variable.</param>
    /// <param name="y0">A vector of size N containing the initial conditions. N is the number of differential equations.</param>
    public void InitializeODEs(OdeFunction function, OdeJacobian jacobian, int numEquations, double t0, double[] y0)
    {
      base.InitializationWithJacobian(function, jacobian, numEquations);
      SetInitialValues(t0, y0);
    }

    #endregion Methods
  }
}
