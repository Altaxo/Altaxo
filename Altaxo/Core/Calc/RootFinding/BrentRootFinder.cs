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
    public class BrentRootFinder : AbstractRootFinder
    {
        public BrentRootFinder(Func<double, double> function)
          : base(function)
        {
        }

        public BrentRootFinder(Func<double, double> function, int maxNumberOfIterations, double accuracy)
          : base(function, maxNumberOfIterations, accuracy)
        {
        }

        protected override double Find()
        {
            double a = _xMin;
            double b = _xMax;
            double c = _xMax;
            double d = 0;
            double e = 0;
            double min1, min2;
            double fa = _function(a);
            double fb = _function(b);
            double fc = fb, p, q, r, s, tol1, xm = double.NaN;

            // Vérifications d'usage
            if (_xMin >= _xMax || Sign(fa) == Sign(fb))
                throw new RootFinderException(MessageInvalidRange, 0, new Range(_xMin, _xMax), 0.0);

            int iiter = 0;
            for (; iiter < _maximumNumberOfIterations; iiter++)
            {
                if (Sign(fb) == Sign(fc))
                { c = a; fc = fa; e = d = b - a; }
                if (Math.Abs(fc) < Math.Abs(fb))
                { a = b; fa = fb; b = c; fb = fc; c = a; fc = fa; }
                tol1 = 2.0 * double_Accuracy * Math.Abs(b) + 0.5 * _accuracy;
                xm = 0.5 * (c - b);
                if (Math.Abs(xm) <= tol1 || fb == 0.0)
                    return (b);
                if (Math.Abs(e) >= tol1 && Math.Abs(fa) >= Math.Abs(fa))
                {
                    s = fb / fa;
                    if (a == c)
                    { p = 2.0 * xm * s; q = 1.0 - s; }
                    else
                    {
                        q = fa / fc;
                        r = fb / fc;
                        p = s * (2.0 * xm * q * (q - r) - (b - a) * (r - 1.0));
                        q = (q - 1.0) * (r - 1.0) * (s - 1.0);
                    }
                    if (p > 0.0)
                        q = -q;
                    p = Math.Abs(p);
                    min1 = 3.0 * xm * q - Math.Abs(tol1 * q);
                    min2 = Math.Abs(e * q);
                    if (2.0 * p < Math.Min(min1, min2))
                    {
                        // On applique l'interpolation
                        e = d;
                        d = p / q;
                    }
                    else
                    {
                        // L'interpolation a échoué; on applique la méthode de bisection
                        d = xm;
                        e = d;
                    }
                }
                else
                {
                    // La décroissance est trop lente, on applique la méthode de bisection
                    d = xm;
                    e = d;
                }
                a = b;
                fa = fb;
                b += (Math.Abs(d) > tol1 ? d : tol1 * Sign(xm));
                /*
                                if(Math.Abs(d)>tol1) b+=d;
                                else b+=sign(tol1,xm);
                        */
                fb = _function(b);
            }
            // L'algorithme a dépassé le nombre d'itérations autorisé
            throw new RootFinderException(MessageAccuracyNotReached, iiter, new Range(a, b), Math.Abs(xm));
        }
    }
}
