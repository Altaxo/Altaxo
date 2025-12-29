#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Copyright (C) bsargos, Software Developer, France
//    (see CodeProject article http://www.codeproject.com/Articles/16083/One-dimensional-root-finding-algorithms)
//    This source code file is licenced under the CodeProject open license (CPOL)
//
//    modified for Altaxo:  a data processing and data plotting program
//    Copyright (C) 2012 Dr. Dirk Lellinger
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

namespace Altaxo.Calc.RootFinding
{
  /// <summary>
  /// Root finder using Newton's method.
  /// </summary>
  public class NewtonRootFinder : AbstractRootFinder
  {
    /// <summary>
    /// Derivative of the function used by Newton's method.
    /// </summary>
    protected Func<double, double> _derivativeOfFunction;

    /// <summary>
    /// Initializes a new instance of the <see cref="NewtonRootFinder"/> class.
    /// </summary>
    /// <param name="function">A continuous function.</param>
    /// <param name="derivativeOfFunction">Derivative of <paramref name="function"/>.</param>
    public NewtonRootFinder(Func<double, double> function, Func<double, double> derivativeOfFunction)
      : base(function)
    {
      _derivativeOfFunction = derivativeOfFunction;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NewtonRootFinder"/> class.
    /// </summary>
    /// <param name="function">A continuous function.</param>
    /// <param name="derivativeOfFunction">Derivative of <paramref name="function"/>.</param>
    /// <param name="maxNumberOfIterations">Maximum number of iterations allowed.</param>
    /// <param name="accuracy">Desired accuracy for the computed root.</param>
    public NewtonRootFinder(Func<double, double> function, Func<double, double> derivativeOfFunction, int maxNumberOfIterations, double accuracy)
      : base(function, maxNumberOfIterations, accuracy)
    {
      _derivativeOfFunction = derivativeOfFunction;
    }

    /// <inheritdoc/>
    protected override double Find()
    {
      double dx = 0;
      double x;

      if (_xMin >= _xMax)
        throw new RootFinderException(MessageRangeArgumentInvalid, 0, new Range(_xMin, _xMax), 0.0);

      x = 0.5 * (_xMin + _xMax);
      int iiter = 0;
      for (; iiter < _maximumNumberOfIterations; iiter++)
      {
        dx = _function(x) / _derivativeOfFunction(x);
        x -= dx;

        if (Sign(_xMin - x) != Sign(x - _xMax))
          throw new RootFinderException(MessageInvalidRange, iiter, new Range(x, x + dx), Math.Abs(dx));

        if (Math.Abs(dx) < _accuracy)
          return x;
      }

      // L'algorithme a dépassé le nombre d'itérations autorisé
      throw new RootFinderException(MessageInvalidRange, iiter, new Range(x, x + dx), Math.Abs(dx));
    }
  }
}
