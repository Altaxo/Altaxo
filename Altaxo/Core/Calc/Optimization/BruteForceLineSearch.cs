#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Altaxo.Calc.LinearAlgebra;

namespace Altaxo.Calc.Optimization
{
  /// <summary>
  /// Brute-force line search that evaluates the objective function on a uniform grid and optionally
  /// refines the minimum by recursive subdivision.
  /// </summary>
  public class BruteForceLineSearch : LineSearchMethod
  {
    private int _numberOfInitialDivisions;
    private int _numberOfSubsequentDivisions;
    private int _divisionDepth;

    /// <summary>
    /// Initializes a new instance of the <see cref="BruteForceLineSearch"/> class.
    /// </summary>
    /// <param name="cost">The cost function to be minimized.</param>
    public BruteForceLineSearch(ICostFunction cost)
    {
      costFunction_ = cost;
      endCriteria_ = new EndCriteria();

      _numberOfInitialDivisions = 128;
      _numberOfSubsequentDivisions = 2;
      _divisionDepth = 32;
    }

    /// <summary>
    /// Gets or sets the number of grid divisions used in the initial coarse search.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">The value is less than 2.</exception>
    public int NumberOfInitialDivisions
    {
      get
      {
        return _numberOfInitialDivisions;
      }
      set
      {
        if (value < 2)
          throw new ArgumentOutOfRangeException("Number of initial divisions must not smaller than 2");
        _numberOfInitialDivisions = value;
      }
    }

    /// <summary>
    /// Gets or sets the number of divisions used for each refinement step after the initial search.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">The value is less than 2.</exception>
    public int NumberOfSubsequentDivisions
    {
      get
      {
        return _numberOfInitialDivisions;
      }
      set
      {
        if (value < 2)
          throw new ArgumentOutOfRangeException("Number of subsequent divisions must not smaller than 2");
        _numberOfSubsequentDivisions = value;
      }
    }

    /// <summary>
    /// Gets or sets the maximum recursion depth for refining the minimum.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">The value is less than 0.</exception>
    public int DivisionDepth
    {
      get
      {
        return _divisionDepth;
      }
      set
      {
        if (value < 0)
          throw new ArgumentOutOfRangeException("DivisionDepth must be >=0");
        _divisionDepth = value;
      }
    }

    /// <inheritdoc/>
    public override Vector<double> Search(Vector<double> x, Vector<double> direction, double step)
    {
      return Search(x, x + direction * step, _numberOfInitialDivisions, _numberOfSubsequentDivisions, _divisionDepth);
    }

    /// <summary>
    /// Searches for the minimum of the cost function on the line segment between two bounds.
    /// </summary>
    /// <param name="bound0">Left bound of the search interval.</param>
    /// <param name="bound1">Right bound of the search interval.</param>
    /// <param name="numberOfInitialDivisions">Number of uniform divisions in the initial grid search.</param>
    /// <param name="numberOfSubsequentDivisions">Number of divisions used in each subsequent refinement step.</param>
    /// <param name="divisionDepth">Maximum recursion depth used for refinement.</param>
    /// <returns>The point where the cost function is minimal within the search interval.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="numberOfInitialDivisions"/> is less than 2, <paramref name="numberOfSubsequentDivisions"/> is less than 2,
    /// or a valid minimum cannot be determined because all function evaluations are invalid or infinite.
    /// </exception>
    public Vector<double> Search(Vector<double> bound0, Vector<double> bound1, int numberOfInitialDivisions, int numberOfSubsequentDivisions, int divisionDepth)
    {
      if (numberOfInitialDivisions < 2)
        throw new ArgumentOutOfRangeException("Number of initial divisions must not smaller than 2");
      if (numberOfSubsequentDivisions < 2)
        throw new ArgumentOutOfRangeException("Number of subsequent divisions must be not smaller than 2");

      int actualNumberOfDivisions = numberOfInitialDivisions;
      int imin = SearchMinimumByDivision(bound0, bound1, actualNumberOfDivisions);

      if (imin < 0)
        throw new ArgumentOutOfRangeException("Function evaluation resulted in either all invalid or infinite function values");

      double r = imin / (double)actualNumberOfDivisions;
      double rstep = 1 / (double)actualNumberOfDivisions;

      double rmin = Math.Max(0, r - rstep);
      double rmax = Math.Min(1, r + rstep);

      Vector<double> xLeft = bound0 * (1 - rmin) + bound1 * rmin;
      Vector<double> xRight = bound0 * (1 - rmax) + bound1 * rmax;

      if (divisionDepth <= 0)
      {
        return bound0 * (1 - r) + bound1 * r;
      }
      else if (numberOfSubsequentDivisions == 2)
      {
        Vector<double> xMiddle = bound0 * (1 - r) + bound1 * r;
        return BinaryMinimumSearch(xLeft, FunctionEvaluation(xLeft), xMiddle, FunctionEvaluation(xMiddle), xRight, FunctionEvaluation(xRight), divisionDepth);
      }
      else
      {
        return SearchMinimumByRecursiveDivisions(xLeft, xRight, numberOfSubsequentDivisions, divisionDepth);
      }
    }

    private Vector<double> SearchMinimumByRecursiveDivisions(Vector<double> xLeft, Vector<double> xRight, int numberOfDivisions, int recursionDepth)
    {
      int ifound = SearchMinimumByDivision(xLeft, xRight, numberOfDivisions);

      if (ifound < 0)
        throw new ArgumentOutOfRangeException("Function evaluation resulted in either all invalid or infinite function values");

      double r = ifound / (double)numberOfDivisions;
      double rmin = Math.Max(0, (ifound - 1) / (double)numberOfDivisions);
      double rmax = Math.Min(1, (ifound + 1) / (double)numberOfDivisions);

      if (recursionDepth > 1)
        return SearchMinimumByRecursiveDivisions(xLeft * (1 - rmin) + xRight * rmin, xLeft * (1 - rmax) + xRight * rmax, numberOfDivisions, recursionDepth - 1);
      else
        return xLeft * (1 - r) + xRight * r;
    }

    private int SearchMinimumByDivision(Vector<double> bound0, Vector<double> bound1, int numberOfInitialDivisions)
    {
      double minValue = double.PositiveInfinity;
      int imin = -1;

      for (int i = 0; i <= numberOfInitialDivisions; i++)
      {
        double r = i / (double)numberOfInitialDivisions;
        double actValue = FunctionEvaluation(bound0 * (1 - r) + bound1 * r);
        if (actValue < minValue)
        {
          minValue = actValue;
          imin = i;
        }
      }
      return imin;
    }

    private Vector<double> BinaryMinimumSearch(Vector<double> xLeft, double valLeft, Vector<double> xMiddle, double valMiddle, Vector<double> xRight, double valRight, int recursionDepth)
    {
      if (recursionDepth <= 0)
        return xMiddle;

      var xLeftMiddle = (xLeft + xMiddle) * 0.5;
      double valLeftMiddle = FunctionEvaluation(xLeftMiddle);

      var xRightMiddle = (xRight + xMiddle) * 0.5;
      double valRightMiddle = FunctionEvaluation(xRightMiddle);

      if (valLeftMiddle < valMiddle && valRightMiddle < valMiddle && valRightMiddle < valLeftMiddle) // special case: saddle point and right is smaller than left
      {
        return BinaryMinimumSearch(xMiddle, valMiddle, xRightMiddle, valRightMiddle, xRight, valRight, recursionDepth - 1);
      }
      else if (valLeftMiddle < valMiddle)
      {
        return BinaryMinimumSearch(xLeft, valLeft, xLeftMiddle, valLeftMiddle, xMiddle, valMiddle, recursionDepth - 1);
      }
      else if (valRightMiddle < valMiddle)
      {
        return BinaryMinimumSearch(xMiddle, valMiddle, xRightMiddle, valRightMiddle, xRight, valRight, recursionDepth - 1);
      }
      else
      {
        return BinaryMinimumSearch(xLeftMiddle, valLeftMiddle, xMiddle, valMiddle, xRightMiddle, valRightMiddle, recursionDepth - 1);
      }
    }
  }
}
