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
#endregion

using System;

namespace Altaxo.Calc.RootFinding
{
	public class SecantRootFinder : AbstractRootFinder
	{
		public SecantRootFinder(Func<double, double> function)
			: base(function)
		{
		}

		public SecantRootFinder(Func<double, double> function, int maxNumberOfIterations, double accuracy)
			: base(function, maxNumberOfIterations, accuracy)
		{
		}

		protected override double Find()
		{
			double x1 = _xMin;
			double x2 = _xMax;
			double fl = _function(x1);
			double f = _function(x2);
			double dx, xl, rts;

			if (Math.Abs(fl) < Math.Abs(f))
			{
				rts = x1;
				xl = x2;
				base.Swap(ref fl, ref f);
			}
			else
			{
				xl = x1;
				rts = x2;
			}
			int iiter = 0;
			for (; iiter < _maximumNumberOfIterations; iiter++)
			{
				if (f == fl)
					throw new RootFinderException(MessageInadequateAlgorithm, iiter, new Range(x1, x2), Math.Abs(x1 - x2));

				dx = (xl - rts) * f / (f - fl);
				xl = rts;
				fl = f;
				rts += dx;
				f = _function(rts);
				if (Math.Abs(dx) < _accuracy || f == 0.0)
					return rts;
			}
			throw new RootFinderException(MessageRootNotFound, iiter, new Range(xl, x2), _accuracy);
		}
	}
}
