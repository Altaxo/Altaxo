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
	public class RidderRootFinder : AbstractRootFinder
	{
		public RidderRootFinder(Func<double, double> function)
			: base(function)
		{
		}

		public RidderRootFinder(Func<double, double> function, int maxNumberOfIterations, double accuracy)
			: base(function, maxNumberOfIterations, accuracy)
		{
		}

		protected override double Find()
		{
			// Vérifications d'usage
			if (_xMin >= _xMax)
				throw new RootFinderException(MessageRangeArgumentInvalid, 0, new Range(_xMin, _xMax), 0.0);						// Mauvaise plage de recherche

			double ans = -1.11e30;
			double fm, fnew, s;
			double xh = _xMax;
			double xl = _xMin;
			double xm, xnew;
			double fl = _function(_xMin);
			double fh = _function(_xMax);

			int iiter = 0;
			if (Sign(fl) != Sign(fh))
			{
				for (; iiter < _maximumNumberOfIterations; iiter++)
				{
					// Compute the mid point
					xm = 0.5 * (xl + xh);
					fm = _function(xm);
					s = Math.Sqrt(fm * fm - fl * fh);
					if (s == 0.0)
						return (ans);

					// Updating formula
					xnew = xm + (xm - xl) * ((fl >= fh ? 1.0 : -1.0) * fm / s);
					// Maybe the new value is the good one
					if (Math.Abs(xnew - ans) <= _accuracy) return (ans);
					// Store this new point
					ans = xnew;
					fnew = _function(ans);
					if (Sign(fm, fnew) != fm)
					{
						xl = xm;
						fl = fm;
						xh = ans;
						fh = fnew;
					}
					else if (Sign(fl, fnew) != fl)
					{
						xh = ans;
						fh = fnew;
					}
					else if (Sign(fh, fnew) != fh)
					{
						xl = ans;
						fl = fnew;
					}
					else
					{
						throw new Exception();
					}

					if (Math.Abs(xh - xl) <= _accuracy)
						return (ans);
				}
				throw new RootFinderException(MessageAccuracyNotReached, iiter, new Range(_xMin, _xMax), Math.Abs(xh - xl));									// nombre d'itérations autorisé dépassé
			}
			else
			{
				if (fl == 0.0) return (_xMin);
				if (fh == 0.0) return (_xMax);
			}
			throw new RootFinderException(MessageAccuracyNotReached, iiter, new Range(_xMin, _xMax), Math.Abs(xh - xl));									// nombre d'itérations autorisé dépassé
		}
	}
}