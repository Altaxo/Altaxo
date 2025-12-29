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
 * ICostFunction.cs
 *
 * Copyright (c) 2004, dnAnalytics Project. All rights reserved.
*/

using System;
using Altaxo.Calc.LinearAlgebra;

namespace Altaxo.Calc.Optimization
{
  /// <summary>Interface for cost function declarations.</summary>
  /// <remarks>
  /// <para>Copyright (c) 2003-2004, dnAnalytics Project. All rights reserved. See <a>http://www.dnAnalytics.net</a> for details.</para>
  /// <para>Adopted for Altaxo (c) 2005 Dr. Dirk Lellinger.</para>
  /// </remarks>
  public interface ICostFunction
  {
    /// <summary>Computes the cost function value at <paramref name="x"/>.</summary>
    /// <param name="x">Point at which to evaluate the cost function.</param>
    /// <returns>The cost function value at <paramref name="x"/>.</returns>
    double Value(Vector<double> x);

    /// <summary>Computes the gradient (first derivative) of the cost function at <paramref name="x"/>.</summary>
    /// <param name="x">Point at which to evaluate the gradient.</param>
    /// <returns>The gradient vector at <paramref name="x"/>.</returns>
    Vector<double> Gradient(Vector<double> x);

    /// <summary>Computes the Hessian (second derivative) of the cost function at <paramref name="x"/>.</summary>
    /// <param name="x">Point at which to evaluate the Hessian.</param>
    /// <returns>The Hessian matrix at <paramref name="x"/>.</returns>
    Matrix<double> Hessian(Vector<double> x);

    /// <summary>Gets or sets the constraints for the given cost function.</summary>
    ConstraintDefinition Constraint
    {
      get;
      set;
    }
  }
}
