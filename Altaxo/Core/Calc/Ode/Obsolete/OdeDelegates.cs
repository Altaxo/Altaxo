#region Copyright © 2009, De Santiago-Castillo JA. All rights reserved.

//Copyright © 2009 Jose Antonio De Santiago-Castillo
//E-mail:JAntonioDeSantiago@gmail.com
//Web: www.DotNumerics.com
//

#endregion Copyright © 2009, De Santiago-Castillo JA. All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;

namespace Altaxo.Calc.Ode.Obsolete
{
  /// <summary>
  /// Delegate defining the Ordinary Differential Equations (ODEs)  dy(i)/dt = f(i) = f(i,t,y(1),y(2),...,y(N)).
  /// </summary>
  /// <param name="t">The independent variable.</param>
  /// <param name="y">Array of size N containing the dependent variable values(y(1),y(2),...,y(N)).</param>
  /// <param name="dydt">A vector of size N, f(i) = dy(i)/dt that define the ordinary differential equations system,
  /// where N is the number of differential equations.</param>
  public delegate void OdeFunction(double t, double[] y, double[] dydt);

  /// <summary>
  /// Delegate that compute the Jacobian matrix df/dy (size NxN), as a function of the scalar t and the vector y.
  /// </summary>
  /// <param name="t">The independent variable.</param>
  /// <param name="y">Array of size N containing the dependent variable values(y(1),y(2),...,y(N)).</param>
  /// <param name="jacobian">The Jacobian matrix df/dy (size NxN).</param>
  public delegate void OdeJacobian(double t, double[] y, double[,] jacobian);

  /// <summary>
  /// Delegate used for solution output.
  /// </summary>
  /// <param name="t">The value of t where the solution is calculated.</param>
  /// <param name="y">An array containing the solution of the differential equations at the value t.</param>
  public delegate void OdeSolution(double t, double[] y);
}
