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
	public class BisectionRootFinder : AbstractRootFinder
	{
		public BisectionRootFinder(Func<double, double> function)
			: base(function)
		{
		}

		public BisectionRootFinder(Func<double, double> function, int maxNumberOfIterations, double accuracy)
			: base(function, maxNumberOfIterations, accuracy)
		{
		}

		protected override double Find()
		{
			double dx, xmid, x;
			double f = _function(_xMin);
			double fmid = _function(_xMax);

			if (_xMin >= _xMax || Sign(f) == Sign(fmid))
				throw new RootFinderException(MessageInvalidRange, 0, new Range(_xMin, _xMax), 0.0);

			if (f < 0.0)
			{
				dx = _xMax - _xMin;
				x = _xMin;
			}
			else
			{
				dx = _xMin - _xMax;
				x = _xMax;
			};

			int iiter = 0;
			do
			{
				fmid = _function(xmid = x + (dx *= 0.5));
				if (fmid <= 0.0)
					x = xmid;
				if (Math.Abs(dx) < _accuracy || fmid == 0.0)
					return (x);
			} while (iiter++ < _maximumNumberOfIterations);

			// L'algorithme a dépassé le nombre d'itérations autorisé
			throw new RootFinderException(MessageAccuracyNotReached, iiter, new Range(Math.Min(xmid, x), Math.Max(xmid, x)), Math.Abs(dx));
		}
	}
}