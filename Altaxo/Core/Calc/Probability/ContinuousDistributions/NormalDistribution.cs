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

#region Further copyright(s)

// This file is partly based on Matpack 1.7.3 sources (Author B.Gammel)
// The following Matpack files were used here:
// matpack-1.7.3\source\random\rannormal.cc

// This file is partly based on Troschuetz.Random Class Library (Author S.Troschuetz)

#endregion Further copyright(s)

#region original copyrights

//   -*- C++ -*-
/*****************************************************************************
 *
 *   |_|_|_  |_|_    |_    |_|_|_  |_		     C O M M U N I C A T I O N
 * |_        |_  |_  |_  |_        |_		               N E T W O R K S
 * |_        |_  |_  |_  |_        |_		                     C L A S S
 *   |_|_|_  |_    |_|_    |_|_|_  |_|_|_|_	                 L I B R A R Y
 *
 * $Id: Normal.c,v 1.2 2002/01/14 11:37:33 spee Exp $
 *
 * CNClass: CNNormal --- CNNormal (Gaussian) distributed random numbers
 *
 *****************************************************************************
 * Copyright (C) 1992-1996   Communication Networks
 *                           Aachen University of Technology
 *                           D-52056 Aachen
 *                           Germany
 *                           Email: cncl-adm@comnets.rwth-aachen.de
 *****************************************************************************
 * This file is part of the CN class library. All files marked with
 * this header are free software; you can redistribute it and/or modify
 * it under the terms of the GNU Library General Public License as
 * published by the Free Software Foundation; either version 2 of the
 * License, or (at your option) any later version.  This library is
 * distributed in the hope that it will be useful, but WITHOUT ANY
 * WARRANTY; without even the implied warranty of MERCHANTABILITY or
 * FITNESS FOR A PARTICULAR PURPOSE.  See the GNU Library General Public
 * License for more details.  You should have received a copy of the GNU
 * Library General Public License along with this library; if not, write
 * to the Free Software Foundation, 675 Mass Ave, Cambridge, MA 02139,
 * USA.
 *****************************************************************************
 * original Copyright:
 * -------------------
 * Copyright (C) 1988 Free Software Foundation
 *    written by Dirk Grunwald (grunwald@cs.uiuc.edu)
 *
 * This file is part of the GNU C++ Library.  This library is free
 * software; you can redistribute it and/or modify it under the terms of
 * the GNU Library General Public License as published by the Free
 * Software Foundation; either version 2 of the License, or (at your
 * option) any later version.  This library is distributed in the hope
 * that it will be useful, but WITHOUT ANY WARRANTY; without even the
 * implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR
 * PURPOSE.  See the GNU Library General Public License for more details.
 * You should have received a copy of the GNU Library General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, 675 Mass Ave, Cambridge, MA 02139, USA.
 *****************************************************************************/

#endregion original copyrights

using System;

namespace Altaxo.Calc.Probability
{
    /// <summary>
    /// Provides generation of normal distributed random numbers.
    /// </summary>
    /// <remarks>
    /// The implementation of the <see cref="NormalDistribution"/> type bases upon information presented on
    ///   <a href="http://en.wikipedia.org/wiki/Normal_distribution">Wikipedia - Normal distribution</a>
    ///   and the implementation in the <a href="http://www.lkn.ei.tum.de/lehre/scn/cncl/doc/html/cncl_toc.html">
    ///   Communication Networks Class Library</a>.
    /// <code>
    /// Return normal (Gaussian) distributed random deviates
    /// with mean "m" and standard deviation  "s" according to the density:
    ///
    ///                                           2
    ///                      1               (x-m)
    ///  p   (x) dx =  ------------  exp( - ------- ) dx
    ///   m,s          sqrt(2 pi) s          2 s*s
    ///
    /// </code></remarks>
    public class NormalDistribution : ContinuousDistribution
    {
        #region instance fields

        /// <summary>
        /// Gets or sets the parameter mu which is used for generation of normal distributed random numbers.
        /// </summary>
        /// <remarks>Call <see cref="IsValidMu"/> to determine whether a value is valid and therefor assignable.</remarks>
        public double Mu
        {
            get
            {
                return mu;
            }
            set
            {
                Initialize(value, sigma);
            }
        }

        /// <summary>
        /// Stores the parameter mu which is used for generation of normal distributed random numbers.
        /// </summary>
        private double mu;

        /// <summary>
        /// Gets or sets the parameter sigma which is used for generation of normal distributed random numbers.
        /// </summary>
        /// <remarks>Call <see cref="IsValidSigma"/> to determine whether a value is valid and therefor assignable.</remarks>
        public double Sigma
        {
            get
            {
                return sigma;
            }
            set
            {
                Initialize(mu, value);
            }
        }

        /// <summary>
        /// Stores the parameter sigma which is used for generation of normal distributed random numbers.
        /// </summary>
        private double sigma;

        /// <summary>
        /// Stores a precomputed normal distributed random number that will be returned the next time
        ///   <see cref="NextDouble"/> gets called.
        /// </summary>
        /// <remarks>
        /// Two new normal distributed random numbers are generated every other call to <see cref="NextDouble"/>.
        /// </remarks>
        private double cacheval;

        /// <summary>
        /// Stores a value indicating whether <see cref="NextDouble"/> was called twice since last generation of
        ///   normal distributed random numbers.
        /// </summary>
        /// <remarks>
        /// Two new normal distributed random numbers are generated every other call to <see cref="NextDouble"/>.
        /// </remarks>
        private bool cached;

        private double scale;

        #endregion instance fields

        #region construction

        /// <summary>
        /// Initializes a new instance of the <see cref="T:NormalDistribution"/> class, using a
        ///   <see cref="StandardGenerator"/> as underlying random number generator.
        /// </summary>
        public NormalDistribution()
          : this(DefaultGenerator)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:NormalDistribution"/> class, using the specified
        ///   <see cref="Generator"/> as underlying random number generator.
        /// </summary>
        /// <param name="generator">A <see cref="Generator"/> object.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="generator"/> is NULL (<see langword="Nothing"/> in Visual Basic).
        /// </exception>
        public NormalDistribution(Generator generator)
          : this(0, 1, generator)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:NormalDistribution"/> class, using the <see cref="Distribution.DefaultGenerator"/> as underlying random number generator.
        /// </summary>
        /// <param name="mu">Mean value.</param>
        /// <param name="sigma">Standard deviation.</param>
        public NormalDistribution(double mu, double sigma)
          : this(mu, sigma, DefaultGenerator)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:NormalDistribution"/> class, using the specified
        ///   <see cref="Generator"/> as underlying random number generator.
        /// </summary>
        /// <param name="mu">Mean value.</param>
        /// <param name="sigma">Standard deviation.</param>
        /// <param name="generator">A <see cref="Generator"/> object.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="generator"/> is NULL (<see langword="Nothing"/> in Visual Basic).
        /// </exception>
        public NormalDistribution(double mu, double sigma, Generator generator)
          : base(generator)
        {
            Initialize(mu, sigma);
        }

        #endregion construction

        #region instance methods

        public void Initialize(double mu, double sigma)
        {
            if (!IsValidMu(mu))
                throw new ArgumentOutOfRangeException("Mu is out of range (Infinity and NaN now allowed)");
            if (!IsValidSigma(sigma))
                throw new ArgumentOutOfRangeException("Sigma is out of range (must be positive)");

            this.mu = mu;
            this.sigma = sigma;

            cached = false;
            scale = 2.0 / generator.Maximum;
        }

        /// <summary>
        /// Determines whether the specified value is valid for parameter <see cref="Mu"/>.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <returns><see langword="true"/>.</returns>
        public bool IsValidMu(double value)
        {
            return value >= double.MinValue && value <= double.MaxValue;
        }

        /// <summary>
        /// Determines whether the specified value is valid for parameter <see cref="Sigma"/>.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <returns>
        /// <see langword="true"/> if value is greater than 0.0; otherwise, <see langword="false"/>.
        /// </returns>
        public bool IsValidSigma(double value)
        {
            return value > 0.0;
        }

        #endregion instance methods

        #region overridden Distribution members

        /// <summary>
        /// Gets the minimum possible value of normal distributed random numbers.
        /// </summary>
        public override double Minimum
        {
            get
            {
                return double.MinValue;
            }
        }

        /// <summary>
        /// Gets the maximum possible value of normal distributed random numbers.
        /// </summary>
        public override double Maximum
        {
            get
            {
                return double.MaxValue;
            }
        }

        /// <summary>
        /// Gets the mean value of normal distributed random numbers.
        /// </summary>
        public override double Mean
        {
            get
            {
                return mu;
            }
        }

        /// <summary>
        /// Gets the median of normal distributed random numbers.
        /// </summary>
        public override double Median
        {
            get
            {
                return mu;
            }
        }

        /// <summary>
        /// Gets the variance of normal distributed random numbers.
        /// </summary>
        public override double Variance
        {
            get
            {
                return Math.Pow(sigma, 2.0);
            }
        }

        /// <summary>
        /// Gets the mode of normal distributed random numbers.
        /// </summary>
        public override double[] Mode
        {
            get
            {
                return new double[] { mu };
            }
        }

        /// <summary>
        /// Returns a normal distributed floating point random number.
        /// </summary>
        /// <returns>A normal distributed double-precision floating point number.</returns>
        public override double NextDouble()
        {
            // We don't have an extra deviate
            if (cached == false)
            {
                // Pick two uniform numbers in the square extending from -1 tp +1
                // in each direction and check if they are in the unit circle
                double v1, v2, r;
                do
                {
                    v1 = scale * generator.Next() - 1; // scale maps the random long to [0,2]
                    v2 = scale * generator.Next() - 1;
                    r = v1 * v1 + v2 * v2;
                } while (r >= 1.0);

                double f = Math.Sqrt((-2 * Math.Log(r)) / r);

                // Make Box-Muller transformation to get two normal deviates.
                // Return one and save the other for the next time.
                cacheval = v1 * f;
                cached = true;
                return (v2 * f * sigma + mu);

                // We have an extra deviate, so unset the flag and return it
            }
            else
            {
                cached = false;
                return (cacheval * sigma + mu);
            }
        }

        #endregion overridden Distribution members

        #region CdfPdfQuantile

        private static readonly double _OneBySqrt2Pi = 1 / Math.Sqrt(2 * Math.PI);
        private static readonly double _OneBySqrt2 = 1 / Math.Sqrt(2);

        private static double Sqr(double x)
        {
            return x * x;
        }

        public override double CDF(double z)
        {
            return CDF(z, mu, sigma);
        }

        public static double CDF(double z, double m, double s)
        {
            return 0.5 * (1 + Altaxo.Calc.ErrorFunction.Erf(_OneBySqrt2 * (z - m) / s));
        }

        public override double PDF(double z)
        {
            return PDF(z, mu, sigma);
        }

        public static double PDF(double z, double m, double s)
        {
            return _OneBySqrt2Pi * Math.Exp(-0.5 * Sqr((z - m) / s)) / s;
        }

        public override double Quantile(double p)
        {
            return Quantile(p, mu, sigma);
        }

        public static double Quantile(double p, double m, double s)
        {
            return m + s * ErrorFunction.QuantileOfNormalDistribution01(p);
        }

        #endregion CdfPdfQuantile
    }
}
