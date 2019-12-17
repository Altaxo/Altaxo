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
    public abstract class AbstractRootFinder
    {
        #region Constants

        private static readonly StringResourceKey SRK_RangeArgumentInvalid = new StringResourceKey("RootFinding.RangeArgumentInvalid", "The provided range is not valid", "");
        private static readonly StringResourceKey SRK_InvalidRange = new StringResourceKey("RootFinding.InvalidRange", "Invalid range while finding root", "");
        private static readonly StringResourceKey SRK_AccuracyNotReached = new StringResourceKey("RootFinding.AccuracyNotReached", "The accuracy couldn't be reached within the specified number of iterations", "");
        private static readonly StringResourceKey SRK_RootNotFound = new StringResourceKey("RootFinding.RootNotFound", "The algorithm ended without root in the range", "");
        private static readonly StringResourceKey SRK_RootNotBracketed = new StringResourceKey("RootFinding.RootNotBracketed", "The algorithm could not start because the root seemed not to be bracketed", "");
        private static readonly StringResourceKey SRK_InadequateAlgorithm = new StringResourceKey("RootFinding.InadequateAlgorithm", "This algorithm is not able to solve this equation", "");

        protected static string MessageRangeArgumentInvalid { get { return StringResources.AltaxoCore.GetString(SRK_RangeArgumentInvalid); } }

        protected static string MessageInvalidRange { get { return StringResources.AltaxoCore.GetString(SRK_InvalidRange); } }

        protected static string MessageAccuracyNotReached { get { return StringResources.AltaxoCore.GetString(SRK_AccuracyNotReached); } }

        protected static string MessageRootNotFound { get { return StringResources.AltaxoCore.GetString(SRK_RootNotFound); } }

        protected static string MessageRootNotBracketed { get { return StringResources.AltaxoCore.GetString(SRK_RootNotBracketed); } }

        protected static string MessageInadequateAlgorithm { get { return StringResources.AltaxoCore.GetString(SRK_InadequateAlgorithm); } }

        protected static double double_Accuracy = 9.99200722162641E-16;

        #endregion Constants

        #region Variables

        private static int _defaultMaximumNumberOfIterations = 30;
        private static double _defaultAccuracy = 1.0E-04;

        protected int _maximumNumberOfIterations;
        protected double _xMin;
        protected double _xMax;
        protected double _accuracy;
        protected Func<double, double> _function;
        protected Func<double, double> _originalFunction;
        private double _bracketingFactor = 1.6;

        #endregion Variables

        #region Construction

        /// <summary>Constructor.</summary>
        /// <param name="f">A continuous function.</param>
        public AbstractRootFinder(Func<double, double> f)
          : this(f, _defaultMaximumNumberOfIterations, _defaultAccuracy)
        {
        }

        public AbstractRootFinder(Func<double, double> function, int maxNumberOfIterations, double accuracy)
        {
            _function = function;
            _maximumNumberOfIterations = maxNumberOfIterations;
            _accuracy = accuracy;
        }

        #endregion Construction

        #region Properties

        public double BracketingFactor
        {
            get { return _bracketingFactor; }
            set
            {
                if (value <= 0.0)
                    throw new ArgumentOutOfRangeException();
                _bracketingFactor = value;
            }
        }

        public int MaximumNumberOfIterations
        {
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException();
                _maximumNumberOfIterations = value;
            }
        }

        public double Accuracy
        {
            get { return _accuracy; }
            set { _accuracy = value; }
        }

        #endregion Properties

        #region Methods

        /// <summary>Detect a range containing at least one root.</summary>
        /// <param name="xmin">Lower value of the range.</param>
        /// <param name="xmax">Upper value of the range</param>
        /// <param name="factor">The growing factor of research. Usually 1.6.</param>
        /// <returns>True if the bracketing operation succeeded, else otherwise.</returns>
        /// <remarks>This iterative methods stops when two values with opposite signs are found.</remarks>
        public bool SearchBracketsOutward(ref double xmin, ref double xmax, double factor)
        {
            // Check the range
            if (xmin >= xmax)
                throw new RootFinderException(MessageInvalidRange, 0, new Range(xmin, xmax), 0.0);

            var fmin = _function(xmin);
            var fmax = _function(xmax);
            int iiter = 0;
            do
            {
                if (Sign(fmin) != Sign(fmax))
                    return (true);

                if (Math.Abs(fmin) < Math.Abs(fmax))
                    fmin = _function(xmin += factor * (xmin - xmax));
                else
                    fmax = _function(xmax += factor * (xmax - xmin));
            } while (iiter++ < _maximumNumberOfIterations);

            throw new RootFinderException(MessageRootNotFound, iiter, new Range(fmin, fmax), 0.0);
        }

        /// <summary>Prototype algorithm for solving the equation f(x)==0.</summary>
        /// <param name="x1">The low value of the range where the root is supposed to be.</param>
        /// <param name="x2">The high value of the range where the root is supposed to be.</param>
        /// <param name="bracket">Determines whether a bracketing operation is required.</param>
        /// <returns>Returns the root with the specified accuracy.</returns>
        public virtual double Solve(double x1, double x2, bool bracket)
        {
            if (bracket)
                SearchBracketsOutward(ref x1, ref x2, _bracketingFactor);

            _xMin = x1;
            _xMax = x2;
            return Find();
        }

        /// <summary>Prototype algorithm for solving the equation f(x)==y.</summary>
        /// <param name="x1">The lower value of the range where the root is supposed to be.</param>
        /// <param name="x2">The higher value of the range where the root is supposed to be.</param>
        /// <param name="y">The function value to find the function's argument for.</param>
        /// <param name="bracket">Determines whether a bracketing operation is required.</param>
        /// <returns>Returns the root with the specified accuracy.</returns>
        public virtual double Solve(double x1, double x2, double y, bool bracket)
        {
            _originalFunction = _function;
            _function = UnaryFunctions.Subtract(_originalFunction, UnaryFunctions.Constant(y));

            double x = Solve(x1, x2, bracket);

            _function = _originalFunction;
            _originalFunction = null;
            return x;
        }

        protected abstract double Find();

        #endregion Methods

        #region Helper methods

        protected void Swap(ref double x, ref double y)
        {
            var t = x;
            x = y;
            y = t;
        }

        /// <summary>Helper method useful for preventing rounding errors.</summary>
        /// <returns>a*sign(b)</returns>
        protected double Sign(double a, double b)
        {
            return b >= 0 ? (a >= 0 ? a : -a) : (a >= 0 ? -a : a);
        }

        internal static double Sign(double x)
        {
            return x > 0 ? 1.0 : -1.0;
        }

        #endregion Helper methods
    }
}
