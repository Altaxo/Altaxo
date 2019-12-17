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
using System.Collections.Generic;
using System.Text;

namespace Altaxo.Calc.RootFinding
{
    public class FalsePositionRootFinder : AbstractRootFinder
    {
        public FalsePositionRootFinder(Func<double, double> function)
          : base(function)
        {
        }

        public FalsePositionRootFinder(Func<double, double> function, int maxNumberOfIterations, double accuracy)
          : base(function, maxNumberOfIterations, accuracy)
        {
        }

        protected override double Find()
        {
            double xl, xh, dx, del, f, rtf;
            double x1 = _xMin;
            double x2 = _xMax;
            double fl = _function(x1);
            double fh = _function(x2);

            if (fl * fh > 0.0)
                throw new RootFinderException(MessageRootNotBracketed, 0, new Range(x1, x2), 0.0);

            if (fl < 0.0)
            {
                xl = x1;
                xh = x2;
            }
            else
            {
                xl = x2;
                xh = x1;
                Swap(ref fl, ref fh);
            }

            dx = xh - xl;
            int iiter = 0;
            for (; iiter < _maximumNumberOfIterations; iiter++)
            {
                rtf = xl + dx * fl / (fl - fh);
                f = _function(rtf);
                if (f < 0.0)
                {
                    del = xl - rtf;
                    xl = rtf;
                    fl = f;
                }
                else
                {
                    del = xh - rtf;
                    xh = rtf;
                    fh = f;
                }
                dx = xh - xl;
                if (Math.Abs(del) < _accuracy || f == 0.0)
                    return rtf;
            }
            throw new RootFinderException(MessageRootNotFound, iiter, new Range(xl, xh), _accuracy);
        }
    }
}
