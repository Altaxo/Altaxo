#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Copyright (c) 2003-2004, dnAnalytics. All rights reserved.
//
//    modified for Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
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

/*
 * CostFunction.cs
 *
 * Copyright (c) 2004, dnAnalytics Project. All rights reserved.
 * NB: Problem class inspired by the optimization frame in the QuantLib library
*/

using System;
using Altaxo.Calc.LinearAlgebra;

namespace Altaxo.Calc.Optimization
{
  /// <summary>Base class for cost function declaration</summary>
  /// <remarks>
  /// <para>Copyright (c) 2003-2004, dnAnalytics Project. All rights reserved. See <a>http://www.dnAnalytics.net</a> for details.</para>
  /// <para>Adopted to Altaxo (c) 2005 Dr. Dirk Lellinger.</para>
  /// </remarks>
  public abstract class CostFunction : ICostFunction
  {
    /// <inheritdoc/>
    public abstract double Value(Vector<double> x);

    /// <inheritdoc/>
    public virtual Vector<double> Gradient(Vector<double> x)
    {
      double eps = 1e-8;
      double fp, fm;
      var grad = CreateVector.Dense<double>(x.Count);

      var xx = x.Clone();
      for (int i = 0; i < x.Count; i++)
      {
        xx[i] += eps;
        fp = Value(xx);
        xx[i] -= 2.0 * eps;
        fm = Value(xx);
        grad[i] = 0.5 * (fp - fm) / eps;
        xx[i] = x[i];
      }
      return grad;
    }

    /// <inheritdoc/>
    public virtual Matrix<double> Hessian(Vector<double> x)
    {
      throw new OptimizationException("Hessian Evaluation not implemented");
    }

    /// <inheritdoc/>
    /// <remarks>Defaults to no constraints.</remarks>
    public virtual ConstraintDefinition Constraint
    {
      get { return constraint_; }
      set { constraint_ = value; }
    }

    /// <summary>Backs the <see cref="Constraint"/> property.</summary>
    protected ConstraintDefinition constraint_ = new NoConstraint();
  }

  /// <summary>
  /// Simple wrapper around a scalar function <see cref="Func{T, TResult}"/> defined on a single variable,
  /// exposing it as a <see cref="CostFunction"/>.
  /// </summary>
  public class Simple1DCostFunction : CostFunction
  {
    private Func<double, double> _func;

    /// <summary>Initializes a new instance of the <see cref="Simple1DCostFunction"/> class.</summary>
    /// <param name="func">The scalar function to be minimized.</param>
    /// <exception cref="ArgumentNullException"><paramref name="func"/> is <see langword="null"/>.</exception>
    public Simple1DCostFunction(Func<double, double> func)
    {
      _func = func ?? throw new ArgumentNullException(nameof(func));
    }

    /// <inheritdoc/>
    public override double Value(Vector<double> x)
    {
      return _func(x[0]);
    }
  }

  /// <summary>
  /// Simple wrapper around a scalar function <see cref="Func{T1, T2, TResult}"/> defined on two variables,
  /// exposing it as a <see cref="CostFunction"/>.
  /// </summary>
  public class Simple2DCostFunction : CostFunction
  {
    private Func<double, double, double> _func;

    /// <summary>Initializes a new instance of the <see cref="Simple2DCostFunction"/> class.</summary>
    /// <param name="func">The scalar function to be minimized.</param>
    /// <exception cref="ArgumentNullException"><paramref name="func"/> is <see langword="null"/>.</exception>
    public Simple2DCostFunction(Func<double, double, double> func)
    {
      _func = func ?? throw new ArgumentNullException(nameof(func));
    }

    /// <inheritdoc/>
    public override double Value(Vector<double> x)
    {
      return _func(x[0], x[1]);
    }
  }
}
