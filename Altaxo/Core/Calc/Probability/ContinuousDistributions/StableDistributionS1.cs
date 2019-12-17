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
using System.Text;

namespace Altaxo.Calc.Probability
{
    using Altaxo.Calc.RootFinding;

    /// <summary>
    ///  Represents a stable distribution in Nolan's S1 parametrization.
    /// </summary>
    /// <remarks>
    /// The characteristic function in Nolan's S1 parametrization is:
    /// <code>
    /// log(phi(t))= -scale^alpha |t|^alpha (1-i beta Sign(t) Tan(pi alpha/2)) + i location t    (for alpha not equal to 1)
    /// </code>
    /// and
    /// <code>
    /// log(phi(t)) = -scale |t| (1+i beta Sign(t) (2/pi) Log(|t|)) + i location t  (for alpha equal to 1)
    /// </code>
    /// <para>Reference: J.P.Nolan, Numerical calculation of stable densities and distribution functions. Communication is statistics - Stochastic models, 13, 759-774, 1999</para>
    /// <para>Reference: S.Borak, W.Härdle, R.Weron, Stable distributions. SFB 649 Discussion paper 2005-2008, http://sfb649.wiwi.hu-berlin.de, ISSN 1860-5664</para>
    /// <para/>
    /// <para>If you are interested in accurate calculations when beta is close to 1 or -1, you should use those functions which allow you to provide the parameter <c>abe</c>. This helps
    /// specifying beta with higher accuracy close to +1 or -1. For instance, by using abe=1E-30 and beta=1, it is possible to specify beta=1-1E-30, which is impossible otherwise since with the 64-bit representation of numbers.</para>
    /// </remarks>
    public class StableDistributionS1 : StableDistributionBase
    {
        /// <summary>Characteristic exponent in the range (0,2].</summary>
        private double _alpha;

        /// <summary>Skewness parameter in the range [-1,1].</summary>
        private double _beta;

        /// <summary>Helps to represent <see cref="_beta"/> with higher accuracy close to |beta|=1. If beta&gt;0, <c>abe</c> is defined as 1-beta. If beta&lt;0, <c>abe</c> is defined as 1+beta. </summary>
        private double _abe;

        /// <summary>Location parameter. Default value is 0.</summary>
        private double _location;

        /// <summary>Scale parameter (&gt;0). Default value is 1.</summary>
        private double _scale = 1;

        /// <summary>Stores helper objects to calculate the <see cref="M:PDF"/>.</summary>
        private object _tempStorePDF;

        #region Construction

        /// <summary>
        /// Creates a new instance of the stable distribution in Nolan's parametrization with default parameters (alpha=1, beta=0) and the default generator.
        /// </summary>
        public StableDistributionS1()
          : this(DefaultGenerator)
        {
        }

        /// <summary>
        /// Creates a new instance of he stable distribution in Nolan's parametrization with default parameters (alpha=1, beta=0).
        /// </summary>
        /// <param name="generator">Random number generator to be used with this distribution.</param>
        public StableDistributionS1(Generator generator)
          : this(1, 0, 1, 1, 0, generator)
        {
        }

        /// <summary>
        /// Creates a new instance of he stable distribution in Nolan's parametrization with given parameters (alpha, beta) and the default random number generator.
        /// </summary>
        /// <param name="alpha">Characteristic exponent of the distribution.</param>
        /// <param name="beta">Skewness parameter of the distribution, in the range [-1,1].</param>
        public StableDistributionS1(double alpha, double beta)
          : this(alpha, beta, GetAbeFromBeta(beta), 1, 0, DefaultGenerator)
        {
        }

        /// <summary>
        /// Creates a new instance of he stable distribution in Nolan's parametrization with given parameters (alpha, beta, abe) and the default random number generator.
        /// </summary>
        /// <param name="alpha">Characteristic exponent of the distribution, in the range (0,2].</param>
        /// <param name="beta">Skewness parameter of the distribution, in the range [-1,1].</param>
        /// <param name="abe">Parameter to specify beta with higher accuracy around -1 and 1. Is defined as 1-beta for beta&gt;=0 or as 1+beta for beta&lt;0.</param>
        public StableDistributionS1(double alpha, double beta, double abe)
          : this(alpha, beta, abe, 1, 0, DefaultGenerator)
        {
        }

        /// <summary>
        /// Creates a new instance of he stable distribution in Nolan's parametrization with given parameters (alpha, beta, scale, location) and the default random number generator.
        /// </summary>
        /// <param name="alpha">Distribution parameter alpha (broadness exponent).</param>
        /// <param name="beta">Distribution parameter beta (skew).</param>
        /// <param name="scale">Scaling parameter (broadness of the distribution).</param>
        /// <param name="location">Location parameter of the distribution.</param>
        public StableDistributionS1(double alpha, double beta, double scale, double location)
          : this(alpha, beta, GetAbeFromBeta(beta), scale, location, DefaultGenerator)
        {
        }

        /// <summary>
        /// Creates a new instance of he stable distribution in Nolan's parametrization with given parameters (alpha, beta, scale, location) and the provided random number generator.
        /// </summary>
        /// <param name="alpha">Characteristic exponent of the distribution.</param>
        /// <param name="beta">Skewness parameter of the distribution, in the range [-1,1].</param>
        /// <param name="scale">Scaling parameter (broadness) of the distribution, &gt;0.</param>
        /// <param name="location">Location parameter of the distribution.</param>
        /// <param name="generator">Random number generator to be used with this distribution.</param>
        public StableDistributionS1(double alpha, double beta, double scale, double location, Generator generator)
          : this(alpha, beta, GetAbeFromBeta(beta), scale, location, generator)
        {
        }

        /// <summary>
        /// Creates a new instance of the stable distribution in Nolan's parametrization with given parameters (alpha, beta, abe, scale, location) and the default random number generator.
        /// </summary>
        /// <param name="alpha">Characteristic exponent of the distribution.</param>
        /// <param name="beta">Skewness parameter of the distribution, in the range [-1,1].</param>
        /// <param name="abe">Parameter to specify beta with higher accuracy around -1 and 1. Is defined as 1-beta for beta&gt;=0 or as 1+beta for beta&lt;0.</param>
        /// <param name="scale">Scaling parameter (broadness) of the distribution, &gt;0.</param>
        /// <param name="location">Location parameter of the distribution.</param>
        public StableDistributionS1(double alpha, double beta, double abe, double scale, double location)
          : this(alpha, beta, abe, scale, location, DefaultGenerator)
        {
        }

        /// <summary>
        /// Creates a new instance of he stable distribution in Nolan's parametrization with given parameters (alpha, beta, abe, scale, location) and the provided random number generator.
        /// </summary>
        /// <param name="alpha">Distribution parameter alpha (broadness exponent).</param>
        /// <param name="beta">Distribution parameter beta (skew).</param>
        /// <param name="abe">Parameter to specify beta with higher accuracy around -1 and 1. Is defined as 1-beta for beta&gt;=0 or as 1+beta for beta&lt;0.</param>
        /// <param name="scale">Scaling parameter (broadness) of the distribution, &gt;0.</param>
        /// <param name="location">Location parameter of the distribution.</param>
        /// <param name="generator">Random number generator to be used with this distribution.</param>
        public StableDistributionS1(double alpha, double beta, double abe, double scale, double location, Generator generator)
          : base(generator)
        {
            Initialize(alpha, beta, abe, scale, location);
        }

        #endregion Construction

        #region Distribution properties

        /// <summary>
        /// Initializes this instance of the distribution with the distribution parameters.
        /// </summary>
        /// <param name="alpha">Distribution parameter alpha (broadness exponent).</param>
        /// <param name="beta">Distribution parameter beta (skew).</param>
        /// <param name="abe">Parameter to specify beta with higher accuracy around -1 and 1. Is defined as 1-beta for beta&gt;=0 or as 1+beta for beta&lt;0.</param>
        /// <param name="scale">Scaling parameter (broadness) of the distribution, &gt;0.</param>
        /// <param name="location">Location parameter of the distribution.</param>
        public void Initialize(double alpha, double beta, double abe, double scale, double location)
        {
            if (!IsValidAlpha(alpha))
                throw new ArgumentOutOfRangeException("Alpha out of range (must be greater than 0 and smaller than or equal to 2)");
            if (!IsValidBeta(beta))
                throw new ArgumentOutOfRangeException("Beta out of range (must be in the range [-1,1])");
            if (!IsValidScale(scale))
                throw new ArgumentOutOfRangeException("Sigma out of range (must be >0)");
            if (!IsValidLocation(location))
                throw new ArgumentOutOfRangeException("Mu out of range (must be finite)");

            _alpha = alpha;
            _beta = beta;
            _abe = abe;
            _scale = scale;
            _location = location;

            // Generator variables
            if (_alpha != 1 && _alpha != 2)
            {
                double tanpialpha2 = TanXPiBy2(alpha);
                _gen_t = beta * tanpialpha2;
                _gen_B = Math.Atan(_gen_t) / alpha;
                _gen_S = PowerOfOnePlusXSquared(_gen_t, 0.5 / _alpha);
                _gen_Scale = 1;
            }
        }

        public static bool IsValidAlpha(double alpha)
        {
            return alpha > 0 && alpha <= 2;
        }

        public static bool IsValidBeta(double beta)
        {
            return beta >= -1 && beta <= 1;
        }

        public static bool IsValidScale(double scale)
        {
            return scale > 0;
        }

        public static bool IsValidLocation(double location)
        {
            return location >= double.MinValue && location <= double.MaxValue;
        }

        /// <summary>Gets the minimum possible value of distributed random numbers.</summary>
        public override double Minimum
        {
            get
            {
                if (_alpha < 1 && _beta == 1)
                    return _location;
                else
                    return double.NegativeInfinity;
            }
        }

        /// <summary>Gets the maximum possible value of distributed random numbers.</summary>
        public override double Maximum
        {
            get
            {
                if (_alpha < 1 && _beta == -1)
                    return _location;
                else
                    return double.PositiveInfinity;
            }
        }

        /// <summary>Gets the mean of distributed random numbers. For alpha&lt;=1, it is not defined. For alpha&gt;1, it is the <see cref="_location"/> parameter.</summary>
        public override double Mean
        {
            get { return _alpha <= 1 ? double.NaN : _location; }
        }

        /// <summary>Gets the median of distributed random numbers. If beta=0, it is <see cref="_location"/>. For beta!=0, it is also defined, but not analytically expressable, and is not calcuated here (TODO, please help!).</summary>
        public override double Median
        {
            get
            {
                if (0 == _beta)
                    return _location;
                else
                    return double.NaN; // TODO : this is not analytical expressable, but is defined!
            }
        }

        /// <summary>Gets the variance of distributed random numbers. Is finite only for alpha=2.</summary>
        public override double Variance
        {
            get
            {
                if (_alpha == 2)
                    return 2 * _scale * _scale;
                else
                    return double.PositiveInfinity;
            }
        }

        /// <summary>Gets the mode of distributed random numbers.</summary>
        public override double[] Mode
        {
            get
            {
                if (0 == _beta)
                    return new double[] { _location };
                else
                    return new double[] { double.NaN }; // TODO : this is not analytical expressable, but is defined!
            }
        }

        /// <summary>Returns a distributed floating point random number.</summary>
        /// <returns>A distributed double-precision floating point number.</returns>
        public override double NextDouble()
        {
            if (_beta == 0)
                return _location + _scale * GenerateSymmetricCase(_alpha);
            else
            {
                if (_alpha == 1) // c * (X + beta * Math.Log(c) / M_PI_2);
                    return (GenerateAsymmetricCaseS1_AEq1(_alpha, _beta) + _beta * Math.Log(_scale) * 2 / Math.PI) * _scale + _location;
                else
                    return GenerateAsymmetricCaseS1_ANe1(_alpha, _gen_t, _gen_B, _gen_S, _scale) + _location;
            }
        }

        /// <summary>Calculates the probability density function.</summary>
        /// <param name="x">Argument.</param>
        /// <returns>The relative likelihood for the random variable to occur at the point <paramref name="x"/>.</returns>
        public override double PDF(double x)
        {
            return PDF(x, _alpha, _beta, _abe, _scale, _location, ref _tempStorePDF, DefaultPrecision);
        }

        /// <summary>Calculates the cumulative distribution function.</summary>
        /// <param name="x">Argument.</param>
        /// <returns>
        /// The probability that the random variable of this probability distribution will be found at a value less than or equal to <paramref name="x"/>.
        /// </returns>
        public override double CDF(double x)
        {
            return CDF(x, _alpha, _beta, _abe, _scale, _location, ref _tempStorePDF, DefaultPrecision);
        }

        /// <summary>Calculates the quantile of the distribution function.</summary>
        /// <param name="p">The probability p.</param>
        /// <returns>The point x at which the cumulative distribution function <see cref="M:CDF"/>() of argument x is equal to <paramref name="p"/>.</returns>
        public override double Quantile(double p)
        {
            return _location + _scale * Quantile(p, _alpha, _beta, _abe);
        }

        #endregion Distribution properties

        #region PDF dispatcher

        /// <summary>Calculates the probability density function of the stable distribution in S1 parametrization.</summary>
        /// <param name="x">Argument.</param>
        /// <param name="alpha">Characteristic exponent of the distribution, in the range (0,2].</param>
        /// <param name="beta">Skewness parameter of the distribution, in the range [-1,1].</param>
        /// <returns>Probability density at <paramref name="x"/>.</returns>
        public static double PDF(double x, double alpha, double beta)
        {
            object tempStore = null;
            return PDF(x, alpha, beta, 1, 0, ref tempStore, Math.Sqrt(DoubleConstants.DBL_EPSILON));
        }

        /// <summary>Calculates the probability density function of the stable distribution in S1 parametrization.</summary>
        /// <param name="x">Argument.</param>
        /// <param name="alpha">Characteristic exponent of the distribution, in the range (0,2].</param>
        /// <param name="beta">Skewness parameter of the distribution, in the range [-1,1].</param>
        /// <param name="scale">Scaling parameter (broadness) of the distribution, &gt;0.</param>
        /// <param name="location">Location parameter of the distribution.</param>
        /// <returns>Probability density at <paramref name="x"/>.</returns>
        public static double PDF(double x, double alpha, double beta, double scale, double location)
        {
            object tempStore = null;
            return PDF(x, alpha, beta, scale, location, ref tempStore, Math.Sqrt(DoubleConstants.DBL_EPSILON));
        }

        /// <summary>Calculates the probability density function of the stable distribution in S1 parametrization.</summary>
        /// <param name="x">Argument.</param>
        /// <param name="alpha">Characteristic exponent of the distribution, in the range (0,2].</param>
        /// <param name="beta">Skewness parameter of the distribution, in the range [-1,1].</param>
        /// <param name="scale">Scaling parameter (broadness) of the distribution, &gt;0.</param>
        /// <param name="location">Location parameter of the distribution.</param>
        /// <param name="tempStorage">Temporary storage. For the first call, provide null as parameter. For subsequent calls, you can provide the object returned in the first call to speed up calculation.</param>
        /// <param name="precision">The required relative precision of calculation.</param>
        /// <returns>Probability density at <paramref name="x"/>.</returns>
        public static double PDF(double x, double alpha, double beta, double scale, double location, ref object tempStorage, double precision)
        {
            double abe;
            if (beta >= 0)
                abe = 1 - beta;
            else
                abe = 1 + beta;

            return PDF(x, alpha, beta, abe, scale, location, ref tempStorage, precision);
        }

        /// <summary>Calculates the probability density function of the stable distribution in S1 parametrization.</summary>
        /// <param name="x">Argument.</param>
        /// <param name="alpha">Characteristic exponent of the distribution, in the range (0,2].</param>
        /// <param name="beta">Skewness parameter of the distribution, in the range [-1,1].</param>
        /// <param name="abe">Parameter to specify beta with higher accuracy around -1 and 1. Is defined as 1-beta for beta&gt;=0 or as 1+beta for beta&lt;0.</param>
        /// <param name="scale">Scaling parameter (broadness) of the distribution, &gt;0.</param>
        /// <param name="location">Location parameter of the distribution.</param>
        /// <param name="tempStorage">Temporary storage. For the first call, provide null as parameter. For subsequent calls, you can provide the object returned in the first call to speed up calculation.</param>
        /// <param name="precision">The required relative precision of calculation.</param>
        /// <returns>Probability density at <paramref name="x"/>.</returns>
        public static double PDF(double x, double alpha, double beta, double abe, double scale, double location, ref object tempStorage, double precision)
        {
            // Test for special case of symmetric destribution, this can be handled much better
            if (beta == 0)
                return StableDistributionSymmetric.PDF((x - location) / scale, alpha, ref tempStorage, precision) / scale;

            // test input parameter
            if (!(alpha > 0 && alpha <= 2))
                throw new ArgumentOutOfRangeException(string.Format("Alpha must be in the range (0,2], but was: {0}", alpha));
            if (!(beta >= -1 && beta <= 1))
                throw new ArgumentOutOfRangeException(string.Format("Beta must be in the range [-1,1], but was: {0}", beta));

            if (alpha != 1)
            {
                ParameterConversionS1ToFeller(alpha, beta, abe, scale, location, out var gamma, out var aga, out var sigmaf, out var muf);
                return StableDistributionFeller.PDF((x - muf) / sigmaf, alpha, gamma, aga) / sigmaf;
            }
            else
            {
                double mu0 = location + scale * beta * 2 * Math.Log(scale) / Math.PI;
                return StableDistributionS0.PDFMethodAlphaOne((x - mu0) / scale, beta, abe, ref tempStorage, precision) / scale;
            }
        }

        #endregion PDF dispatcher

        #region CDF dispatcher

        /// <summary>Calculates the cumulative distribution function of the stable distribution in S1 parametrization.</summary>
        /// <param name="x">Argument.</param>
        /// <param name="alpha">Characteristic exponent of the distribution, in the range (0,2].</param>
        /// <param name="beta">Skewness parameter of the distribution, in the range [-1,1].</param>
        /// <returns>Cumulative distribution value at <paramref name="x"/>.</returns>
        public static double CDF(double x, double alpha, double beta)
        {
            object tempStorage = null;
            double abe = GetAbeFromBeta(beta);
            return CDF(x, alpha, beta, abe, 1, 0, ref tempStorage, DefaultPrecision);
        }

        /// <summary>Calculates the cumulative distribution function of the stable distribution in S1 parametrization.</summary>
        /// <param name="x">Argument.</param>
        /// <param name="alpha">Characteristic exponent of the distribution, in the range (0,2].</param>
        /// <param name="beta">Skewness parameter of the distribution, in the range [-1,1].</param>
        /// <param name="scale">Scaling parameter (broadness) of the distribution, &gt;0.</param>
        /// <param name="location">Location parameter of the distribution.</param>
        /// <returns>Cumulative distribution value at <paramref name="x"/>.</returns>
        public static double CDF(double x, double alpha, double beta, double scale, double location)
        {
            object tempStorage = null;
            double abe = GetAbeFromBeta(beta);
            return CDF(x, alpha, beta, abe, scale, location, ref tempStorage, DefaultPrecision);
        }

        /// <summary>Calculates the cumulative distribution function of the stable distribution in S1 parametrization.</summary>
        /// <param name="x">Argument.</param>
        /// <param name="alpha">Characteristic exponent of the distribution, in the range (0,2].</param>
        /// <param name="beta">Skewness parameter of the distribution, in the range [-1,1].</param>
        /// <param name="tempStorage">Temporary storage. For the first call, provide null as parameter. For subsequent calls, you can provide the object returned in the first call to speed up calculation.</param>
        /// <param name="precision">The required relative precision of calculation.</param>
        /// <returns>Cumulative distribution value at <paramref name="x"/>.</returns>
        public static double CDF(double x, double alpha, double beta, ref object tempStorage, double precision)
        {
            double abe = GetAbeFromBeta(beta);
            return CDF(x, alpha, beta, abe, 1, 0, ref tempStorage, DefaultPrecision);
        }

        /// <summary>Calculates the cumulative distribution function of the stable distribution in S1 parametrization.</summary>
        /// <param name="x">Argument.</param>
        /// <param name="alpha">Characteristic exponent of the distribution, in the range (0,2].</param>
        /// <param name="beta">Skewness parameter of the distribution, in the range [-1,1].</param>
        /// <param name="scale">Scaling parameter (broadness) of the distribution, &gt;0.</param>
        /// <param name="location">Location parameter of the distribution.</param>
        /// <param name="tempStorage">Temporary storage. For the first call, provide null as parameter. For subsequent calls, you can provide the object returned in the first call to speed up calculation.</param>
        /// <param name="precision">The required relative precision of calculation.</param>
        /// <returns>Cumulative distribution value at <paramref name="x"/>.</returns>
        public static double CDF(double x, double alpha, double beta, double scale, double location, ref object tempStorage, double precision)
        {
            double abe = GetAbeFromBeta(beta);
            return CDF(x, alpha, beta, abe, scale, location, ref tempStorage, DefaultPrecision);
        }

        /// <summary>Calculates the cumulative distribution function of the stable distribution in S1 parametrization.</summary>
        /// <param name="x">Argument.</param>
        /// <param name="alpha">Characteristic exponent of the distribution, in the range (0,2].</param>
        /// <param name="beta">Skewness parameter of the distribution, in the range [-1,1].</param>
        /// <param name="abe">Parameter to specify beta with higher accuracy around -1 and 1. Is defined as 1-beta for beta&gt;=0 or as 1+beta for beta&lt;0.</param>
        /// <returns>Cumulative distribution value at <paramref name="x"/>.</returns>
        public static double CDF(double x, double alpha, double beta, double abe)
        {
            object temp = null;
            return CDF(x, alpha, beta, abe, 1, 0, ref temp, DefaultPrecision);
        }

        /// <summary>Calculates the cumulative distribution function of the stable distribution in S1 parametrization.</summary>
        /// <param name="x">Argument.</param>
        /// <param name="alpha">Characteristic exponent of the distribution, in the range (0,2].</param>
        /// <param name="beta">Skewness parameter of the distribution, in the range [-1,1].</param>
        /// <param name="abe">Parameter to specify beta with higher accuracy around -1 and 1. Is defined as 1-beta for beta&gt;=0 or as 1+beta for beta&lt;0.</param>
        /// <param name="scale">Scaling parameter (broadness) of the distribution, &gt;0.</param>
        /// <param name="location">Location parameter of the distribution.</param>
        /// <returns>Cumulative distribution value at <paramref name="x"/>.</returns>
        public static double CDF(double x, double alpha, double beta, double abe, double scale, double location)
        {
            object temp = null;
            return CDF(x, alpha, beta, abe, scale, location, ref temp, DefaultPrecision);
        }

        /// <summary>Calculates the cumulative distribution function of the stable distribution in S1 parametrization.</summary>
        /// <param name="x">Argument.</param>
        /// <param name="alpha">Characteristic exponent of the distribution, in the range (0,2].</param>
        /// <param name="beta">Skewness parameter of the distribution, in the range [-1,1].</param>
        /// <param name="abe">Parameter to specify beta with higher accuracy around -1 and 1. Is defined as 1-beta for beta&gt;=0 or as 1+beta for beta&lt;0.</param>
        /// <param name="scale">Scaling parameter (broadness) of the distribution, &gt;0.</param>
        /// <param name="location">Location parameter of the distribution.</param>
        /// <param name="tempStorage">Temporary storage. For the first call, provide null as parameter. For subsequent calls, you can provide the object returned in the first call to speed up calculation.</param>
        /// <param name="precision">The required relative precision of calculation.</param>
        /// <returns>Cumulative distribution value at <paramref name="x"/>.</returns>
        public static double CDF(double x, double alpha, double beta, double abe, double scale, double location, ref object tempStorage, double precision)
        {
            // test input parameter
            if (!(alpha > 0 && alpha <= 2))
                throw new ArgumentOutOfRangeException(string.Format("Alpha must be in the range (0,2], but was: {0}", alpha));
            if (!(beta >= -1 && beta <= 1))
                throw new ArgumentOutOfRangeException(string.Format("Beta must be in the range [-1,1], but was: {0}", beta));

            if (alpha != 1)
            {
                ParameterConversionS1ToFeller(alpha, beta, abe, scale, location, out var gamma, out var aga, out var sigmaf, out var muf);
                return StableDistributionFeller.CDF((x - muf) / sigmaf, alpha, gamma, aga, ref tempStorage, precision);
            }
            else
            {
                double mu0 = location + scale * beta * 2 * Math.Log(scale) / Math.PI;
                return StableDistributionS0.CDFMethodAlphaOne((x - mu0) / scale, beta, abe, ref tempStorage, precision);
            }
        }

        #endregion CDF dispatcher

        #region Quantile

        /// <summary>Calculates the quantile of the stable distribution in S1 parametrization.</summary>
        /// <param name="p">Probability p.</param>
        /// <param name="alpha">Characteristic exponent of the distribution, in the range (0,2].</param>
        /// <param name="beta">Skewness parameter of the distribution, in the range [-1,1].</param>
        /// <returns>The value x, so that with a probability of <paramref name="p"/> the random variable is &lt;x.</returns>
        public static double Quantile(double p, double alpha, double beta)
        {
            object tempStorage = null;
            double abe = GetAbeFromBeta(beta);
            return Quantile(p, alpha, beta, abe, ref tempStorage, DefaultPrecision);
        }

        /// <summary>Calculates the quantile of the stable distribution in S1 parametrization.</summary>
        /// <param name="p">Probability p.</param>
        /// <param name="alpha">Characteristic exponent of the distribution, in the range (0,2].</param>
        /// <param name="beta">Skewness parameter of the distribution, in the range [-1,1].</param>
        /// <param name="abe">Parameter to specify beta with higher accuracy around -1 and 1. Is defined as 1-beta for beta&gt;=0 or as 1+beta for beta&lt;0.</param>
        /// <returns>The value x, so that with a probability of <paramref name="p"/> the random variable is &lt;x.</returns>
        public static double Quantile(double p, double alpha, double beta, double abe)
        {
            object tempStorage = null;
            return Quantile(p, alpha, beta, abe, ref tempStorage, DefaultPrecision);
        }

        /// <summary>Calculates the quantile of the stable distribution in S1 parametrization.</summary>
        /// <param name="p">Probability p.</param>
        /// <param name="alpha">Characteristic exponent of the distribution, in the range (0,2].</param>
        /// <param name="beta">Skewness parameter of the distribution, in the range [-1,1].</param>
        /// <param name="abe">Parameter to specify beta with higher accuracy around -1 and 1. Is defined as 1-beta for beta&gt;=0 or as 1+beta for beta&lt;0.</param>
        /// <param name="tempStorage">Temporary storage. For the first call, provide null as parameter. For subsequent calls, you can provide the object returned in the first call to speed up calculation.</param>
        /// <param name="precision">The required relative precision of calculation.</param>
        /// <returns>The value x, so that with a probability of <paramref name="p"/> the random variable is &lt;x.</returns>
        public static double Quantile(double p, double alpha, double beta, double abe, ref object tempStorage, double precision)
        {
            double xguess = Math.Exp(2 / alpha); // guess value for a nearly constant p value in dependence of alpha
            double x0 = -xguess;
            double x1 = xguess;

            object temp = tempStorage;
            double root = double.NaN;
            if (QuickRootFinding.BracketRootByExtensionOnly(delegate (double x)
            { return CDF(x, alpha, beta, abe, 1, 0, ref temp, DefaultPrecision) - p; }, 0, ref x0, ref x1))
            {
                if (null != QuickRootFinding.ByBrentsAlgorithm(delegate (double x)
                { return CDF(x, alpha, beta, abe, 1, 0, ref temp, DefaultPrecision) - p; }, x0, x1, 0, DoubleConstants.DBL_EPSILON, out root))
                    root = double.NaN;
            }
            tempStorage = temp;

            return root;
        }

        #endregion Quantile

        #region Calculation of integration parameters

        public static void GetAlt1GnParameter(double x, double alpha, double beta, double abe,
                                              out double factorp, out double facdiv, out double dev, out double logPdfPrefactor)
        {
            double tan_pi_alpha_2 = TanXPiBy2(alpha);

            double gamma = GammaFromAlphaBetaTanPiA2(alpha, beta, abe, tan_pi_alpha_2, out var aga);
            dev = Math.PI * (gamma < 0 ? 0.5 * aga : 1 - 0.5 * aga);
            // double factor = Math.Pow(xx, alpha / (alpha - 1)) * Math.Pow(Math.Cos(alpha * xi), 1 / (alpha - 1));
            facdiv = CosGammaPiBy2(alpha, gamma, aga); // Inverse part of the original factor without power
            factorp = x * facdiv; // part of the factor with power alpha/(alpha-1);
            logPdfPrefactor = Math.Log(alpha / (Math.PI * Math.Abs(alpha - 1) * (x)));
        }

        public static void GetAlt1GpParameter(double x, double alpha, double beta, double abe,
                                              out double factorp, out double facdiv, out double dev, out double logPdfPrefactor)
        {
            double tan_pi_alpha_2 = TanXPiBy2(alpha);

            double gamma = GammaFromAlphaBetaTanPiA2(alpha, beta, abe, tan_pi_alpha_2, out var aga);
            dev = Math.PI * (gamma >= 0 ? 0.5 * aga : 1 - 0.5 * aga);
            // double factor = Math.Pow(xx, alpha / (alpha - 1)) * Math.Pow(Math.Cos(alpha * xi), 1 / (alpha - 1));
            facdiv = CosGammaPiBy2(alpha, gamma, aga); // Inverse part of the original factor without power
            factorp = x * facdiv; // part of the factor with power alpha/(alpha-1);
            logPdfPrefactor = Math.Log(alpha / (Math.PI * Math.Abs(alpha - 1) * x));
        }

        public static void GetAgt1GnParameter(double x, double alpha, double beta, double abe,
                                                     out double factorp, out double factorw, out double dev, out double logPrefactor)
        {
            double tan_pi_alpha_2 = TanXPiBy2(alpha);

            double gamma = GammaFromAlphaBetaTanPiA2(alpha, beta, abe, tan_pi_alpha_2, out var aga);

            dev = Math.PI * (gamma < 0 ? 0.5 * aga : (0.5 * ((2 - alpha) + gamma)));
            if (dev < 0)
                dev = 0;

            //double factor = Math.Pow(xx, alpha / (alpha - 1)) * Math.Pow(Math.Cos(alpha * xi), 1 / (alpha - 1));
            // we separate the factor in a power of 1/alpha-1 and the rest
            factorp = x * CosGammaPiBy2(alpha, gamma, aga); // Math.Cos(-gamma * 0.5 * Math.PI);
            factorw = x;

            logPrefactor = Math.Log(alpha / (Math.PI * Math.Abs(alpha - 1) * x));
        }

        #endregion Calculation of integration parameters
    }
}
