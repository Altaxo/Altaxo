#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2007 Dr. Dirk Lellinger
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


namespace Altaxo.Calc
{
  using LinearAlgebra;
	/// <summary>
	/// The Kohlrausch function in the frequency domain.
	/// </summary>
	public class Kohlrausch
  {

    #region Real part series


    /// <summary>
    /// Real part of the Fourier transformed derivative of the Kohlrausch function for high frequencies.
    /// </summary>
    /// <param name="beta">Beta parameter.</param>
    /// <param name="z">Circular frequency.</param>
    /// <returns>Real part of the Fourier transformed derivative of the Kohlrausch function for high frequencies, or double.NaN if the series not converges.</returns>
    /// <remarks>This is the real part of the Fourier transform (in Mathematica notation): Re[Integrate[-D[Exp[-t^beta],t]*Exp[-I w t],{t, 0, Infinity}]]. For
    /// beta smaller than one, the return value is always positive.</remarks>
    public static double Re1(double beta, double z)
    {
      int k = 1;
      double z_pow_minusBeta = Math.Pow(z, -beta);
      double kfac = 1;
      double z_pow_minusBetaK = z_pow_minusBeta;


      double term1a = GammaRelated.Gamma(1 + k * beta) * z_pow_minusBetaK / kfac;
      double term1 = term1a * Math.Cos(Math.PI * beta * k * 0.5);

      double sum = term1;

      for (; ; )
      {

        ++k;
        z_pow_minusBetaK *= z_pow_minusBeta;
        kfac *= -k;

        double term2a = GammaRelated.Gamma(1 + k * beta) * z_pow_minusBetaK / kfac;
        double term2 = term2a * Math.Cos(Math.PI * beta * k * 0.5);

        if (Math.Abs(term2a) < Math.Abs(sum * 1E-15))
          break;
        if (Math.Abs(term2a) > Math.Abs(term1a) || double.IsInfinity(kfac)) // sum is not converging
        {
          sum = double.NaN;
          break;
        }

        term1a = term2a; // take as previous term
        sum += term2;
      }
      return sum;
    }

    /// <summary>
    /// Real part of the Fourier transformed derivative of the Kohlrausch function for high frequencies.
    /// </summary>
    /// <param name="beta">Beta parameter.</param>
    /// <param name="z">Circular frequency.</param>
    /// <returns>Real part of the Fourier transformed derivative of the Kohlrausch function for high frequencies, or double.NaN if the series not converges.</returns>
    public static double Re1OldStyle(double beta, double z)
    {
      int k = 1;
      double z_pow_minusBeta = Math.Pow(z, -beta);
      double kfac = 1;
      double z_pow_minusBetaK = z_pow_minusBeta;


      double term1 = GammaRelated.Gamma(1 + k * beta) * z_pow_minusBetaK * Math.Cos(Math.PI * beta * k * 0.5) / kfac;

      ++k;
      z_pow_minusBetaK *= z_pow_minusBeta;
      kfac *= k;

      double term2 = GammaRelated.Gamma(1 + k * beta) * z_pow_minusBetaK * Math.Cos(Math.PI * beta * k * 0.5) / kfac;

      double curr = term1 - term2;
      double sum = curr;
      double prev = curr;

      for (; ; )
      {
        ++k;
        z_pow_minusBetaK *= z_pow_minusBeta;
        kfac *= k;

        term1 = GammaRelated.Gamma(1 + k * beta) * z_pow_minusBetaK * Math.Cos(Math.PI * beta * k * 0.5) / kfac;

        ++k;
        z_pow_minusBetaK *= z_pow_minusBeta;
        kfac *= k;

        term2 = GammaRelated.Gamma(1 + k * beta) * z_pow_minusBetaK * Math.Cos(Math.PI * beta * k * 0.5) / kfac;

        curr = term1 - term2;

        if (Math.Abs(curr) < Math.Abs(sum * 1E-15))
          break;
        if (k > 4 && Math.Abs(curr) > Math.Abs(sum)) // sum is not converging
        {
          sum = double.NaN;
          break;
        }
        if (double.IsInfinity(kfac))
        {
          sum = double.NaN;
          break;
        }

        prev = curr;
        sum += curr;
      }
      return sum;
    }

    /// <summary>
    /// Real part of the Fourier transformed derivative of the Kohlrausch function for low frequencies.
    /// </summary>
    /// <param name="beta">Beta parameter.</param>
    /// <param name="z">Circular frequency, must be much lesser than one.</param>
    /// <returns>Real part of the Fourier transformed derivative of the Kohlrausch function for low frequencies, or double.NaN if the series not converges.</returns>
    /// <remarks>This is the real part of the Fourier transform (in Mathematica notation): Re[Integrate[-D[Exp[-t^beta],t]*Exp[-I w t],{t, 0, Infinity}]]. For
    /// beta smaller than one, the return value is always positive.</remarks>
    public static double Re2(double beta, double z)
    {
      if (beta < 0.0625)
        return Re2SmallBeta(beta, z);


      int k = 1;
      double z_pow_2k = z * z;
      double z_square = z * z;
      double k2m1fac = 1;

      double term1 = GammaRelated.Gamma((k + k) / beta) * z_pow_2k / k2m1fac;

      if (z_square == 0)
        return term1; // if z was so small that z_square can not be evaluated, return after term1

      k = 2;
      k2m1fac = 6;
      z_pow_2k *= z_square;
      double term2 = GammaRelated.Gamma((k + k) / beta) * z_pow_2k / k2m1fac;

      double curr = term1 - term2;
      double sum = curr;
      double prev = curr;

      for (; ; )
      {
        k2m1fac *= (k + k) * (k + k + 1);
        ++k;
        z_pow_2k *= z_square;
        term1 = GammaRelated.Gamma((k + k) / beta) * z_pow_2k / k2m1fac;

        k2m1fac *= (k + k) * (k + k + 1);
        ++k;
        z_pow_2k *= z_square;
        term2 = GammaRelated.Gamma((k + k) / beta) * z_pow_2k / k2m1fac;

        curr = term1 - term2;

        if (Math.Abs(curr) < Math.Abs(sum * 1E-15))
          break;
        if (Math.Abs(curr) > Math.Abs(prev)) // sum is not converging
        {
          sum = double.NaN;
          break;
        }
        if (double.IsInfinity(k2m1fac))
        {
          sum = double.NaN;
          break;
        }

        prev = curr;
        sum += curr;
      }
      return sum / beta;
    }


    /// <summary>
    /// Real part of the Fourier transformed derivative of the Kohlrausch function for low frequencies, and beta&lt;=1/20..
    /// </summary>
    /// <param name="beta">Beta parameter.</param>
    /// <param name="z">Circular frequency, must be much lesser than one.</param>
    /// <returns>Real part of the Fourier transformed derivative of the Kohlrausch function for low frequencies, or double.NaN if the series not converges.</returns>
    /// <remarks>This is the real part of the Fourier transform (in Mathematica notation): Re[Integrate[-D[Exp[-t^beta],t]*Exp[-I w t],{t, 0, Infinity}]]. For
    /// beta smaller than one, the return value is always positive.</remarks>
    public static double Re2SmallBeta(double beta, double z)
    {
      int k = 1;
      double ln_z_pow_2k = 2 * k * Math.Log(z);
      double ln_z_square = 2 * Math.Log(z);
      double k2m1fac = 1;

      double term1 = GammaRelated.LnGamma((k + k) / beta) + ln_z_pow_2k - Math.Log(k2m1fac);

      k = 2;
      k2m1fac = 6;
      ln_z_pow_2k += ln_z_square;
      double term2 = GammaRelated.LnGamma((k + k) / beta) + ln_z_pow_2k - Math.Log(k2m1fac);

      double ln_scaling = Math.Max(term1, term2);

      double curr = Math.Exp(term1 - ln_scaling) - Math.Exp(term2 - ln_scaling);
      double sum = curr;
      double prev = curr;

      for (; ; )
      {
        k2m1fac *= (k + k) * (k + k + 1);
        ++k;
        ln_z_pow_2k += ln_z_square;
        term1 = GammaRelated.LnGamma((k + k) / beta) + ln_z_pow_2k - Math.Log(k2m1fac);

        k2m1fac *= (k + k) * (k + k + 1);
        ++k;
        ln_z_pow_2k += ln_z_square;
        term2 = GammaRelated.LnGamma((k + k) / beta) + ln_z_pow_2k - Math.Log(k2m1fac);

        curr = Math.Exp(term1 - ln_scaling) - Math.Exp(term2 - ln_scaling);

        if (Math.Abs(curr) < Math.Abs(sum * 1E-15))
          break;
        if (Math.Abs(curr) > Math.Abs(prev)) // sum is not converging
        {
          sum = double.NaN;
          break;
        }
        if (double.IsInfinity(k2m1fac))
        {
          sum = double.NaN;
          break;
        }

        prev = curr;
        sum += curr;
      }
      return (sum / beta) * Math.Exp(ln_scaling);
    }

    #endregion

    #region Imaginary series

    /// <summary>
    /// Imaginary part of the Fourier transformed derivative of the Kohlrausch function for high frequencies.
    /// </summary>
    /// <param name="beta">Beta parameter.</param>
    /// <param name="z">Circular frequency.</param>
    /// <returns>Imaginary part of the Fourier transformed derivative of the Kohlrausch function for high frequencies, or double.NaN if the series not converges.</returns>
    /// <remarks>This is the imaginary part of the Fourier transform (in Mathematica notation): Im[Integrate[D[Exp[-t^beta],t]*Exp[-I w t],{t, 0, Infinity}]]. The sign of
    /// the return value here is positive!.</remarks>
    public static double Im1OldStyle(double beta, double z)
    {
      int k=1;
      double z_pow_minusBeta = Math.Pow(z,-beta);
      double kfac = 1;
      double z_pow_minusBetaK = z_pow_minusBeta;
      

      double term1 = GammaRelated.Gamma(1+k*beta)*z_pow_minusBetaK*Math.Sin(Math.PI*beta*k*0.5)/kfac;
      
      ++k;
      z_pow_minusBetaK *= z_pow_minusBeta;
      kfac *= k;

      double term2 = GammaRelated.Gamma(1+k*beta)*z_pow_minusBetaK*Math.Sin(Math.PI*beta*k*0.5)/kfac;
 
      double curr = term1-term2;
      double sum = curr;
      double prev = curr;

      for(;;)
      {
        ++k;
        z_pow_minusBetaK *= z_pow_minusBeta;
        kfac *= k;

        term1 = GammaRelated.Gamma(1+k*beta)*z_pow_minusBetaK*Math.Sin(Math.PI*beta*k*0.5)/kfac;

        ++k;
        z_pow_minusBetaK *= z_pow_minusBeta;
        kfac *= k;

        term2 = GammaRelated.Gamma(1+k*beta)*z_pow_minusBetaK*Math.Sin(Math.PI*beta*k*0.5)/kfac;

        curr = term1-term2;

        if(Math.Abs(curr)<Math.Abs(sum*1E-15))
          break;
        if(k>4 && Math.Abs(curr)>Math.Abs(sum)) // sum is not converging
        {
          sum = double.NaN; 
          break;
        }
        if(double.IsInfinity(kfac))
        {
          sum = double.NaN;
          break;
        }

        prev = curr;
        sum += curr;
      }
      return sum;
    }

    /// <summary>
    /// Imaginary part of the Fourier transformed derivative of the Kohlrausch function for high frequencies.
    /// </summary>
    /// <param name="beta">Beta parameter.</param>
    /// <param name="z">Circular frequency.</param>
    /// <returns>Imaginary part of the Fourier transformed derivative of the Kohlrausch function for high frequencies, or double.NaN if the series not converges.</returns>
    /// <remarks>This is the imaginary part of the Fourier transform (in Mathematica notation): Im[Integrate[D[Exp[-t^beta],t]*Exp[-I w t],{t, 0, Infinity}]]. The sign of
    /// the return value here is positive!.</remarks>
    public static double Im1(double beta, double z)
    {
      int k = 1;
      double z_pow_minusBeta = Math.Pow(z, -beta);
      double kfac = 1;
      double z_pow_minusBetaK = z_pow_minusBeta;


      double term1a = GammaRelated.Gamma(1 + k * beta) * z_pow_minusBetaK  / kfac;
      double term1 = term1a * Math.Sin(Math.PI * beta * k * 0.5);

      double sum = term1;

      for(;;)
      {

      ++k;
      z_pow_minusBetaK *= z_pow_minusBeta;
      kfac *= -k;

      double term2a = GammaRelated.Gamma(1 + k * beta) * z_pow_minusBetaK / kfac;
      double term2 = term2a * Math.Sin(Math.PI * beta * k * 0.5);

      if (Math.Abs(term2a) < Math.Abs(sum * 1E-15))
          break;
      if (Math.Abs(term2a)>Math.Abs(term1a) || double.IsInfinity(kfac)) // sum is not converging
        {
          sum = double.NaN;
          break;
        }

      term1a = term2a; // take as previous term
      sum += term2;
      }
      return sum;
    }

    /// <summary>
    /// Imaginary part of the Fourier transformed derivative of the Kohlrausch function for low frequencies.
    /// </summary>
    /// <param name="beta">Beta parameter.</param>
    /// <param name="z">Circular frequency.</param>
    /// <returns>Imaginary part of the Fourier transformed derivative of the Kohlrausch function for high frequencies, or double.NaN if the series not converges.</returns>
    /// <returns>Imaginary part of the Fourier transformed derivative of the Kohlrausch function for high frequencies, or double.NaN if the series not converges.</returns>
    /// <remarks>This is the imaginary part of the Fourier transform (in Mathematica notation): Im[Integrate[D[Exp[-t^beta],t]*Exp[-I w t],{t, 0, Infinity}]]. The sign of
    /// the return value here is positive!.</remarks>
    public static double Im2(double beta, double z)
    {
      if (beta < 0.0625)
        return Im2SmallBeta(beta, z);

      int k = 1;
      double z_pow_2km1 = z;
      double z_square = z * z;
      double k2m2fac = 1;

      double term1 = GammaRelated.Gamma((k+k-1)/beta)*z_pow_2km1/ k2m2fac;

      if (z_square == 0)
        return term1; // if z was so small that z_square can not be evaluated, return after term1

      k=2;
      k2m2fac = 2;
      z_pow_2km1 *= z_square;
      double term2 = GammaRelated.Gamma((k + k - 1) / beta) * z_pow_2km1 / k2m2fac;

      double curr = term1-term2;
      double sum = curr;
      double prev = curr;

      for (; ; )
      {
        k2m2fac *= (k+k)*(k+k-1);
        ++k;
        z_pow_2km1 *= z_square;
        term1 = GammaRelated.Gamma((k + k - 1) / beta) * z_pow_2km1 / k2m2fac;

        k2m2fac *= (k + k) * (k + k - 1);
        ++k;
        z_pow_2km1 *= z_square;
        term2 = GammaRelated.Gamma((k + k - 1) / beta) * z_pow_2km1 / k2m2fac;

        curr = term1 - term2;

        if (Math.Abs(curr) < Math.Abs(sum * 1E-15))
          break;
        if (Math.Abs(curr) > Math.Abs(prev)) // sum is not converging
        {
          sum = double.NaN;
          break;
        }
        if (double.IsInfinity(k2m2fac))
        {
          sum = double.NaN;
          break;
        }

        prev = curr;
        sum += curr;
      }
      return sum/beta;
    }

    /// <summary>
    /// Imaginary part of the Fourier transformed derivative of the Kohlrausch function for low frequencies, and beta&lt;=1/20..
    /// </summary>
    /// <param name="beta">Beta parameter.</param>
    /// <param name="z">Circular frequency.</param>
    /// <returns>Imaginary part of the Fourier transformed derivative of the Kohlrausch function for low frequencies, or double.NaN if the series not converges.</returns>
    /// <returns>Imaginary part of the Fourier transformed derivative of the Kohlrausch function for high frequencies, or double.NaN if the series not converges.</returns>
    /// <remarks>This is the imaginary part of the Fourier transform (in Mathematica notation): Im[Integrate[D[Exp[-t^beta],t]*Exp[-I w t],{t, 0, Infinity}]]. The sign of
    /// the return value here is positive!.</remarks>
    public static double Im2SmallBeta(double beta, double z)
    {
      

      int k = 1;
      double ln_z_pow_2km1 = k * Math.Log(z);
      double ln_z_square = 2 * Math.Log(z);
      double k2m2fac = 1;

      double curr = GammaRelated.LnGamma((k + k -1) / beta) + ln_z_pow_2km1 - Math.Log(k2m2fac);
      double ln_scaling = curr;
      double sum = 1;

      double prev = curr;

      for (; ; )
      {
        k2m2fac *= (k + k) * (k + k - 1);
        ++k;
        ln_z_pow_2km1 += ln_z_square;
        curr = GammaRelated.LnGamma((k + k -1) / beta) + ln_z_pow_2km1 - Math.Log(k2m2fac);

        if (curr < (prev - 34.5))
          break;
        if (curr >= prev)
        {
          sum = double.NaN;
          break;
        }
        if (double.IsInfinity(k2m2fac))
        {
          sum = double.NaN;
          break;
        }

        if ((k & 1) == 0)
          sum -= Math.Exp(curr - ln_scaling);
        else
          sum += Math.Exp(curr - ln_scaling);

        prev = curr;
      }
      return (sum / beta) * Math.Exp(ln_scaling);
    }

    #endregion

    #region Scheduler for real and imaginary part

    /// <summary>
    /// Real part of the Fourier transformed derivative of the Kohlrausch function for all frequencies. In dependence on the parameters,
    /// either a series expansion (accuracy ca. 1E-14) or a bivariate akima spline (accuracy 1E-4) is used.
    /// </summary>
    /// <param name="beta">Beta parameter (0..1).</param>
    /// <param name="w">Circular frequency.</param>
    /// <returns>Real part of the Fourier transformed derivative of the Kohlrausch function, or double.NaN if the function can not be evaluated (can happen for beta smaller than 1/64).</returns>
    /// <remarks>This is the real part of the Fourier transform (in Mathematica notation): Re[Integrate[-D[Exp[-t^beta],t]*Exp[-I w t],{t, 0, Infinity}]]. For
    /// beta smaller than one, the return value is always positive.
    /// <para>Wanted! Who can contribute to a more accurate interpolation between the points? (The points itself are calculated with an
    /// accuracy of 1E-18.)</para>
    /// </remarks>
    public static double Re(double beta, double w)
    {
      double y = beta * Math.Log(w);

      if (y <= -5)
      {
        double re2 = Re2(beta, w);
        if (beta < 0.0625 && double.IsNaN(re2))
          return 1; // Re2() does not converge properly for beta<1/16, but for all practical purposes the return value is 1 then

        return 1-re2;
      }
      else if (y > 0.25)
      {
        return Re1(beta, w);
      }
      else
      {
        if (_respline == null)
          CreateRealPartSpline();

        double log_OneMinusRe = _respline.Interpolate(beta, y);


        return 1 - Math.Exp(log_OneMinusRe);
      }
    }

    /// <summary>
    /// Imaginary part of the Fourier transformed derivative of the Kohlrausch function for all frequencies. In dependence on the parameters,
    /// either a series expansion (accuracy ca. 1E-14) or a bivariate Akima spline (accuracy 1E-4) is used.
    /// </summary>
    /// <param name="beta">Beta parameter (0..1).</param>
    /// <param name="w">Circular frequency.</param>
    /// <returns>Real part of the Fourier transformed derivative of the Kohlrausch function, or double.NaN if the function can not be evaluated (can happen for beta smaller than 1/64).</returns>
    /// <remarks>This is the imaginary part of the Fourier transform (in Mathematica notation): Im[Integrate[-D[Exp[-t^beta],t]*Exp[-I w t],{t, 0, Infinity}]]. For
    /// beta smaller than one, the return value is always negative.
    /// <para>Wanted! Who can contribute to a more accurate interpolation between the points? (The points itself are calculated with an
    /// accuracy of 1E-18.)</para>
    /// </remarks>
    public static double Im(double beta, double w)
    {
      double y = beta * Math.Log(w);

      if (y < -5)
        return -Im2(beta, w);
      else if (y > 0.25)
        return -Im1(beta, w);
      else
      {
        if (_imspline == null)
          CreateImaginaryPartSpline();

        double log_ar = _imspline.Interpolate(beta, y);


        return -beta * Math.Exp(log_ar);
      }
    }

    /// <summary>
    /// Fourier transformed derivative of the Kohlrausch function for all frequencies. In dependence on the parameters,
    /// either a series expansion (accuracy ca. 1E-14) or a bivariate Akima spline (accuracy 1E-4) is used.
    /// </summary>
    /// <param name="beta">Beta parameter (0..1).</param>
    /// <param name="w">Circular frequency.</param>
    /// <returns>Fourier transformed derivative of the Kohlrausch function, or Complex.NaN if the function can not be evaluated (can happen for beta smaller than 1/64).</returns>
    /// <remarks>This is the imaginary part of the Fourier transform (in Mathematica notation): Integrate[-D[Exp[-t^beta],t]*Exp[-I w t],{t, 0, Infinity}]. For
    /// beta smaller than one, this is a retardation function, so the real part is positive and the imaginary part is negative.
    /// <para>Wanted! Who can contribute to a more accurate interpolation between the points? (The points itself are calculated with an
    /// accuracy of 1E-18.)</para>
    /// </remarks>
    public static Complex ReIm(double beta, double w)
    {
      return Complex.FromRealImaginary(Re(beta, w), Im(beta, w));
    }

    #endregion

    #region Interpolation

    static Interpolation.BivariateAkimaSpline _imspline;
    static Interpolation.BivariateAkimaSpline _respline;
    static void CreateImaginaryPartSpline()
    {
      IROVector x = VectorMath.CreateEquidistantSequence(0, 1.0 / 32, 34); // beta ranging from 0 to 1+1/32
      IROVector y = VectorMath.CreateEquidistantSequence(-5, 1.0 / 64, 353); // y ranging from -5 to 0.5
      IROMatrix z = MatrixMath.ToROMatrix(_imdata);
      _imspline = new Interpolation.BivariateAkimaSpline(x, y, z, false);
    }
    static void CreateRealPartSpline()
    {
      IROVector x = VectorMath.CreateEquidistantSequence(0, 1.0 / 32, 34); // beta ranging from 0 to 1+1/32
      IROVector y = VectorMath.CreateEquidistantSequence(-5, 1.0 / 64, 353); // y ranging from -5 to 0.5
      IROMatrix z = MatrixMath.ToROMatrix(_redata);
      _respline = new Interpolation.BivariateAkimaSpline(x, y, z, false);
    }

    #endregion

    #region DataField

    #region RealPart

    /// <summary>
    /// This field is the natural logarithm of (1-re) for beta=0 to 1+1/32 (steps=1/32)
    /// and for y =-5 to 0.5 in steps of 1/64 with  w=Exp(y/beta) -> y = beta*ln(w).
    /// re is the real part of the fourier transformed negative first derivative of the Kohlrausch function.
    /// (in Mathematica notation): Re[Integrate[-D[Exp[-t^beta],t]*Exp[-I w t],{t, 0, Infinity}]].
    /// </summary>
    static double[][] _redata = new double[][]{
new double[]{
-148.4131591025766,-146.11222634120347,-143.84696623584259,-141.61682573322432,-139.42126035437516,-137.25973406168575,-135.13171912803952,-133.03669600797051,-130.9741532108186,-128.94358717585138,-126.94450214932196,-124.97641006343288,
-123.03883041717654,-121.13129015902297,-119.25332357142641,-117.40447215712239,-115.58428452718766,-113.7923162908356,-112.02812994692015,-110.29129477712178,-108.5813867407896,-106.89798837141365,-105.24068867470233,
-103.60908302823987,-102.00277308269969,-100.42136666458902,-98.864477680501579,-97.331726022854525,-95.822737477086875,-94.337143630296652,-92.874581781294509,-91.434694852051862,-90.017131300521811,-88.621545034811746,
-87.247595328686515,-85.894946738381577,-84.563269020706,-83.252237052414884,-81.9615307508321,-80.690834995703412,-79.439839552261333,-78.2082389954826,-76.995732635520056,-75.802024444290538,-74.62682298320091,
-73.469841331994616,-72.330797018701389,-71.209411950672944,-70.105412346687856,-69.018528670109163,-67.9484955630781,-66.895051781728185,-65.8579401324037,-64.836907408866935,-63.83170433047907,-62.842085481339339,
-61.867809250367884,-60.908637772317434,-59.964336869699544,-59.034675995611252,-58.119428177447986,-57.21836996148928,-56.331281358343482,-55.457945789238345,-54.598150033144236,-53.751684174717226,-52.9183415530491,
-52.097918711212095,-51.290215346585704,-50.495034261953741,-49.71218131735948,-48.941465382707243,-48.182698291098816,-47.435694792893365,-46.700272510479522,-45.97625189374871,-45.263456176258764,-44.5617113320772,
-43.870846033293539,-43.190691608190342,-42.521082000062783,-41.861853726676614,-41.2128458403547,-40.573899888682377,-39.944859875821933,-39.325572224426992,-38.715885738147229,-38.115651564714454,-37.524723159600995,
-36.942956250241515,-36.370208800809451,-35.806340977539655,-35.25121511458854,-34.704695680423619,-34.166649244734039,-33.636944445854191,-33.115451958692312,-32.602044463156354,-32.096596613069337,-31.598985005566618,
-31.109088150967661,-30.626786443114831,-30.151962130172098,-29.684499285876434,-29.224283781234941,-28.771203256660744,-28.325147094540903,-27.886006392229614,-27.4536739354601,-27.028044172168716,-26.609013186724894,
-26.196478674560588,-25.790339917193062,-25.390497757634929,-24.9968545761854,-24.609314266596865,-24.227782212610979,-23.852165264858517,-23.482371718117374,-23.118311288923124,-22.759895093526726,-22.407035626193942,
-22.059646737841184,-21.717643615002626,-21.380942759123343,-21.049461966173517,-20.723120306578686,-20.401838105461145,-20.085536923187668,-19.774139536218833,-19.467569918255219,-19.165753221675935,-18.868615759264884,
-18.576084986220376,-18.28808948244362,-18.00455893510183,-17.725424121461643,-17.450616891988709,-17.180070153709277,-16.913717853829741,-16.651494963610144,-16.39333746248769,-16.139182322446413,-15.888967492629165,
-15.642631884188171,-15.400115355370453,-15.16135869683449,-14.926303617194517,-14.694892728788942,-14.467069533669397,-14.242778409807016,-14.021964597512564,-13.804574186067095,-13.59055410055989,-13.379852088930456,
-13.172416709211404,-12.968197316969134,-12.767144052939209,-12.569207830853442,-12.374340325455691,-12.182493960703473,-11.993621898152476,-11.807678025521156,-11.624616945432633,-11.444393964331121,-11.26696508157019,
-11.092286978670202,-10.920317008742302,-10.751013186076355,-10.584334175890335,-10.42023928423861,-10.258688448076699,-10.099642225480054,-9.9430617860144856,-9.7889089012558941,-9.6371459354569549,-9.4877358363585262,
-9.3406421261434964,-9.195828892530896,-9.053260780008058,-8.9129029811987373,-8.7747212283650384,-8.6386817850411024,-8.5047514377964859,-8.3728974881272649,-8.2430877444728257,-8.1152905143564453,-7.9894745966477085,
-7.8656092739448917,-7.743664305075443,-7.6236099177127361,-7.505416801107283,-7.38905609893065,-7.2744994022303073,-7.1617187424937114,-7.0506865848199123,-6.9413758211970356,-6.8337597638839718,-6.7278121388946914,
-6.6235070795835593,-6.5208191203301125,-6.4197231903217373,-6.320194607432744,-6.2222090721983321,-6.1257426618819864,-6.030771824634841,-5.9372733737456072,-5.84522448197963,-5.7546026760057307,-5.66538583090943,
-5.57755216479126,-5.4910802334487974,-5.4059489251411668,-5.3221374554346959,-5.2396253621284892,-5.15839250025867,-5.0784190371800815,-4.999685447724227,-4.9221725094322908,-4.8458612978620605,-4.7707331819676027,
-4.69676981955058,-4.6239531527820805,-4.552265403793883,-4.4816890703380645,-4.4122069215139064,-4.3438019935610424,-4.2764575857178357,-4.21015725614396,-4.1448848179061955,-4.08062433502646,-4.0173601185911147,
-3.9550767229205772,-3.8937589417983349,-3.8333918047584103,-3.7739605734303869,-3.7154507379411039,-3.6578480133721323,-3.6011383362721756,-3.5453078612235411,-3.4903429574618414,-3.4362302055481027,-3.382956394092469,
-3.3305085165287003,-3.2788737679386735,-3.2280395419261225,-3.1779934275388384,-3.1287232062385919,-3.080216848918031,-3.0324625129638281,-2.9854485393653558,-2.9391634498681936,-2.8935959441717611,-2.8487348971703992,
-2.8045693562372267,-2.7610885385501014,-2.7182818284590451,-2.6761387748944769,-2.6346490888156313,-2.5938026406985348,-2.5535894580629268,-2.5139997230375233,-2.4750237699630251,-2.4366520830322917,-2.3988752939670981,
-2.3616841797309096,-2.325069660277121,-2.2890227963322012,-2.2535347872132085,-2.2185969686791451,-2.1842008108156179,-2.1503379159523,-2.1170000166126748,-2.0841789734955687,-2.0518667734879767,-2.0200555277086965,
-1.9887374695822919,-1.957904952942918,-1.9275504501675447,-1.8976665503381187,-1.8682459574322223,-1.8392814885417808,-1.8107660721193872,-1.782692746251815,-1.7550546569602985,-1.7278450565271633,-1.7010573018484008,
-1.6746848528117839,-1.6487212707001282,-1.6231602166193055,-1.5979954499506333,-1.573220826827253,-1.5488302986341331,-1.5248179105313267,-1.5011778000001228,-1.4779041954117385,-1.4549914146182013,-1.4324338635650782,
-1.4102260349257108,-1.3883625067566268,-1.3668379411737963,-1.3456470830494105,-1.3247847587288655,-1.3042458747676378,-1.2840254166877414,-1.2641184477534664,-1.2445201077660952,-1.2252256118773075,-1.2062302494209807,
-1.1875293827631006,-1.1691184461695043,-1.1509929446911764,-1.1331484530668263,-1.1155806146424807,-1.0982851403078258,-1.0812578074490395,-1.0644944589178593,-1.0479910020166328,-1.0317434074991028,-1.0157477085866857,
-1,-0.98449643700540845,-0.96923323447634413,-0.95420666596918835,-0.93941306281347581,-0.92484881321620482,-0.91051036138003416,-0.89639420663515046,-0.88249690258459546,-0.86881505626284317,-0.85534532730742252,
-0.84208442714338239,-0.82902911818040037,-0.81617621302233978,-0.80352257368906077,-0.791065110850296,-0.77880078307140488,-0.76672659607082,-0.75483960198900735,-0.74313689866875832,-0.73161562894664178,-0.72027297995543982,
-0.70910618243739842,-0.6981125100681258,-0.68728927879097224,-0.676633846161729,-0.66614361070348782,-0.65581601127150158,-0.64564852642789206,-0.635638673826052,-0.62578400960459113,-0.61608212779067828,-0.60653065971263342},
new double[]{
-115.52494769791878,-114.52494769791882,-113.52494769791893,-112.52494769791926,-111.5249476979201,-110.52494769792232,-109.52494769792804,-108.52494769794274,-107.52494769797993,-106.52494769807267,-105.52494769830035,-104.52494769884989,
-103.5249477001526,-102.52494770318276,-101.5249477100936,-100.52494772553736,-99.524947759336158,-98.524947831744129,-97.524947983540329,-96.524948294864856,-95.524948919409624,-94.5249501447574,-93.524952495865477,
-92.5249569074653,-91.524965002915565,-90.524979531866435,-89.52500503636216,-88.525048833097188,-87.525122415462661,-86.525243388281126,-85.525438044905286,-84.525744674209733,-83.526217638211676,-82.52693218652982,
-81.52798987335801,-80.529524324463139,-79.531706981586339,-78.534752351178923,-77.538922227580613,-76.544528368128312,-75.551933180258715,-74.561548135048412,-73.573829828743584,-72.589273842088218,-71.608406759668767,
-70.6317768744818,-69.65994419404737,-68.693470376498951,-67.732909166084482,-66.778797786348875,-65.831649609971365,-64.891948280145726,-63.960143328001742,-63.036647225539781,-62.121833738767116,-61.216037400624,
-60.319553903753722,-59.432641213366871,-58.555521214250653,-57.688381727888213,-56.831378761281165,-55.984638875218351,-55.148261584315165,-54.322321722972475,-53.506871729950696,-52.70194381945668,-51.907552018712529,
-51.123694061278293,-50.350353132356837,-49.587499467340749,-48.835091808357454,-48.093078725871528,-47.36139981379803,-46.63998676730322,-45.928764352704277,-45.227651278774623,-44.536560978426216,-43.855402309259091,
-43.184080180902512,-42.522496116465938,-41.870548754803032,-41.228134299688584,-40.5951469214316,-39.971479115905261,-39.357022025471295,-38.751665725814043,-38.155299482278096,-37.567811978921974,-36.989091523155984,
-36.419026228523506,-35.857504177908382,-35.304413569203653,-34.759642845256671,-34.223080809709295,-33.694616730177046,-33.174140430056148,-32.661542370109096,-32.1567137208569,-31.659546426697318,-31.16993326257138,
-30.687767883914567,-30.212944870552509,-29.745359765132878,-29.284909106624649,-28.831490459361852,-28.385002438060823,-27.945344729197089,-27.51241810908958,-27.086124459005784,-26.666366777570659,-26.25304919073487,
-25.846076959533267,-25.445356485842503,-25.050795316327036,-24.662302144744903,-24.279786812768684,-23.903160309462905,-23.532334769545894,-23.1672234705527,-22.807740829005049,-22.453802395684765,-22.105324850098604,
-21.762225994214543,-21.424424745542627,-21.091841129627014,-20.764396272010135,-20.442012389724525,-20.124612782363229,-19.8121218227752,-19.504464947428268,-19.201568646478567,-18.903360453582053,-18.609768935480762,
-18.320723681393641,-18.036155292239403,-17.755995369716455,-17.480176505262897,-17.208632268917675,-16.941297198102237,-16.678106786340329,-16.418997471932236,-16.163906626598251,-15.912772544105101,-15.665534428887677,
-15.422132384677596,-15.182507403148968,-14.946601352590959,-14.714356966615878,-14.485717832910735,-14.260628382039588,-14.039033876303243,-13.820880398662416,-13.60611484172977,-13.394684896835862,-13.18653904317345,
-12.98162653702429,-12.779897401072063,-12.581302413804741,-12.385793099009373,-12.193321715361909,-12.003841246114433,-11.817305388881906,-11.63366854553019,-11.452885812167038,-11.27491296923734,-11.099706471723911,
-10.92722343945475,-10.757421647517626,-10.590259516782671,-10.425696104533516,-10.263691095207348,-10.104204791244166,-9.9471981040453983,-9.7926325450419185,-9.6404702168714156,-9.49067380466499,-9.3432065674427189,
-9.1980323296179343,-9.0551154726098,-8.9144209265637837,-8.77591416217949,-8.6395611826453411,-8.5053285156794676,-8.3731832056761721,-8.24309280595728,-8.11502537112761,-7.98894944953386,-7.86483407582603,
-7.74264876362062,-7.62236349826467,-7.5039487296999026,-7.38737536542583,-7.2726147635611884,-7.1596387260025,-7.0484194916790237,-6.93892972990292,-6.8311425338138854,-6.725031413917,-6.62057029171306,
-6.5177334934201792,-6.41649574378573,-6.3168321599876691,-6.21871824562408,-6.122129884790052,-6.02704333624082,-5.9334352276400777,-5.841282549892556,-5.75056265155975,-5.6612532333578267,-5.57333234273665,
-5.4867783685389542,-5.4015700357386134,-5.317686400257017,-5.2351068438565473,-5.1538110691101462,-5.073779094445996,-4.9949912492663193,-4.9174281691393,-4.8410707910631858,-4.76590034880157,-4.6918983682889106,
-4.61904666310533,-4.5473273300197512,-4.4767227446004227,-4.4072155568919262,-4.3387886871577157,-4.2714253216873042,-4.2051089086671727,-4.139823154114521,-4.07555201787297,-4.012279709669329,-3.9499906852305782,
-3.8886696424601959,-3.8283015176729749,-3.768871481887508,-3.7103649371754912,-3.65276751306703,-3.59606506301116,-3.540243660890698,-3.48528959759079,-3.43118937762015,-3.3779297157844632,-3.32549753391093,
-3.2738799576234352,-3.223064313167455,-3.17303812428396,-3.1237891091316712,-3.0753051772568831,-3.02757442661016,-2.980585140609243,-2.934325785247375,-2.88878500624652,-2.8439516262546332,-2.7998146420864649,
-2.7563632220071268,-2.7135867030578549,-2.67147458842329,-2.6300165448396671,-2.5892024000432738,-2.5490221402585891,-2.5094659077254819,-2.47052399826488,-2.4321868588823232,-2.3944450854088291,-2.357289420178486,
-2.320710749742227,-2.28470010261722,-2.2492486470713331,-2.2143476889421341,-2.1799886694899011,-2.1461631632841081,-2.11286287612288,-2.08007964298491,-2.047805426013332,-2.0160323125310562,-1.9847525130870951,
-1.9539583595333749,-1.923642303131591,-1.8937969126896179,-1.8644148727270311,-1.83548898166928,-1.8070121500700771,-1.778977398861558,-1.751377857631784,-1.724206762929162,-1.697457456593364,-1.671123384112337,
-1.645198093004987,-1.619675231229154,-1.5945485456144659,-1.5698118803197021,-1.5454591753142659,-1.5214844648834109,-1.497881876156826,-1.4746456276602431,-1.451770027889679,-1.429249473907984,-1.407078449963332,
-1.385251526129313,-1.363763356966297,-1.342608680203726,-1.3217823154430211,-1.301279162880765,-1.281094202051859,-1.2612224905923311,-1.241659163021491,-1.222399429543136,-1.203438574865493,-1.184771957039626,
-1.1663950063159949,-1.1483032240189051,-1.1304921814385489,-1.1129575187403771,-1.095694943891516,-1.0787002316039791,-1.06196922229439,-1.0454978210599819,-1.0292819966705911,-1.013317780576422,-0.997601265931313,
-0.982128606631275,-0.966896016368054,-0.951899767697495,-0.937136191122454,-0.922601674190052,-0.908292660603036,-0.894205649345019,-0.880337193819401,-0.86668390100173,-0.853242430605322,-0.840009494259903,
-0.826981854703092,-0.81415632498451,-0.801529767682322,-0.789099094132016,-0.77686126366723,-0.76481328287243089,-0.75295220484726777,-0.74127512848241328,-0.72977919774671007,-0.718461600985449,-0.70731957022960357,
-0.69635038051584786,-0.68555134921719019,-0.6749198353840552,-0.66445323909565057,-0.65414900082145888,-0.64400460079269306,-0.63401755838356144,-0.62418543150218908,-0.61450581599104126,-0.6049763450367035,-0.59559468858886988},
new double[]{
-79.1351877244449,-78.6351877244449,-78.1351877244449,-77.6351877244449,-77.1351877244449,-76.6351877244449,-76.1351877244449,-75.6351877244449,-75.1351877244449,-74.6351877244449,-74.1351877244449,-73.6351877244449,
-73.135187724444918,-72.635187724444918,-72.135187724444918,-71.635187724444933,-71.135187724444947,-70.635187724444975,-70.135187724445018,-69.635187724445089,-69.1351877244452,-68.635187724445387,-68.135187724445686,
-67.6351877244462,-67.135187724447036,-66.635187724448414,-66.1351877244507,-65.635187724454454,-65.13518772446065,-64.635187724470853,-64.1351877244877,-63.63518772451544,-63.13518772456117,-62.635187724636545,
-62.13518772476074,-61.635187724965348,-61.135187725302295,-60.635187725856923,-60.135187726769331,-59.635187728269109,-59.135187730731857,-58.635187734770668,-58.135187741383554,-57.635187752189857,-57.135187769807196,
-56.635187798448364,-56.135187844859843,-55.635187919785409,-55.135188040228741,-54.635188232918068,-54.135188539557788,-53.635189024702939,-53.135189787432139,-52.635190978444143,-52.13519282478444,-51.6351956651401,
-51.135199999535843,-50.635206558322125,-50.135216396548259,-49.635231021113725,-49.135252559409807,-48.635283979379231,-48.135329371868615,-47.635394306618593,-47.135486272987833,-46.635615215281724,-46.135794170110771,
-45.636040009343276,-45.136374286846113,-44.636824180371825,-44.137423511882773,-43.638213820745754,-43.139245455238623,-42.640578639518289,-42.142284466548645,-41.644445763423448,-41.147157774860425,-40.65052861394458,
-40.154679436638396,-39.659744307880075,-39.165869741526159,-38.673213912832331,-38.181945559211478,-37.692242601177405,-37.204290529292585,-36.718280613482889,-36.234407997553987,-35.752869743895182,-35.273862891385413,
-34.797582583994576,-34.324220319332461,-33.853962356395591,-33.386988310961868,-32.923469956348,-32.463570237272243,-32.007442495853077,-31.555229901617665,-31.107065071905474,-30.663069865197333,-30.223355327538336,
-29.788021771145889,-29.3571589642622,-28.930846412080449,-28.509153709915648,-28.092140951504888,-27.679859177237962,-27.272350849106253,-26.869650341114582,-26.471784435757183,-26.0787728188689,-25.690628566700131,
-25.307358620418107,-24.928964244407869,-24.555441465742049,-24.186781493021751,-23.82297111347749,-23.463993067775949,-23.109826402422026,-22.760446799992735,-22.415826887704817,-22.075936525014875,-21.740743071091828,
-21.410211633096509,-21.08430529626164,-20.762985336794706,-20.446211418632725,-20.133941775067051,-19.826133376232324,-19.522742083420336,-19.223722791139295,-18.929029557794607,-18.638615725820173,-18.352434032040964,
-18.070436708999296,-17.792575577929657,-17.51880213402054,-17.249067624557178,-16.983323120496522,-16.721519581985387,-16.46360791829462,-16.20953904260633,-15.959263922057856,-15.712733623414877,-15.469899354717111,
-15.230712503213121,-14.99512466987588,-14.763087700767629,-14.534553715501422,-14.30947513302703,-14.087804694950894,-13.869495486583148,-13.654500955889445,-13.442774930511263,-13.234271633005392,-13.028945694441493,
-12.826752166485612,-12.62764653208756,-12.431584714880843,-12.238523087395318,-12.048418478175025,-11.861228177886448,-11.676909944495925,-11.49542200758885,-11.316723071897764,-11.140772320101352,-10.967529414951617,
-10.796954500782206,-10.629008204446865,-10.46365163573334,-10.300846387294659,-10.140554534136598,-9.98273863269731,-9.8273617195523819,-9.67438730977617,-9.5237793949880523,-9.3755024411100383,-9.2295213858603944,
-9.0858016360060265,-8.9443090643948153,-8.8050100067875317,-8.6678712585075335,-8.5328600709252012,-8.39994414779279,-8.2690916414443052,-8.1402711488739445,-8.0134517077056984,-7.888602792065794,-7.7656943083688548,
-7.6446965910278619,-7.5255803980972917,-7.4083169068581523,-7.2928777093529851,-7.179234807878383,-7.0673606104419733,-6.9572279261903667,-6.8488099608140951,-6.742080311935096,-6.63701296448197,-6.5335822860577828,
-6.4317630223048923,-6.3315302922709211,-6.2328595837797174,-6.1357267488108258,-6.0401079988907744,-5.94597990049918,-5.8533193704925166,-5.7621036715480489,-5.67231040763043,-5.58391751948306,-5.49690328014626,
-5.411246290504117,-5.3269254748617092,-5.24392007655424,-5.1622096535895,-5.0817740743250406,-5.0025935131810426,-4.9246484463901812,-4.84791964778528,-4.77238818462568,-4.6980354134631339,-4.6248429760478356,
-4.55279279527529,-4.48186707117447,-4.4120482769378633,-4.3433191549936261,-4.2756627131203793,-4.2090622206048041,-4.143501204442348,-4.0789634455811772,-4.0154329752095528,-3.9528940710867082,-3.8913312539172851,
-3.83072928376936,-3.7710731565360351,-3.71234810044053,-3.65453957258474,-3.5976332555411,-3.5416150539876692,-3.48647109138623,-3.43218770670324,-3.37875145117351,-3.3261490851061759,-3.27436757473302,
-3.2233940890986,-3.17321599699213,-3.1238208639206708,-3.07519644912344,-3.0273307026268612,-2.9802117623400588,-2.93382795119047,-2.88816777429917,-2.843219916195713,-2.79897323807188,-2.75541677507424,
-2.712539733634955,-2.6703314888405458,-2.6287815818382,-2.5878797172792551,-2.5476157607994518,-2.5079797365355749,-2.46896182467807,-2.4305523590592548,-2.392741824776718,-2.3555208558514882,-2.3188802329205971,
-2.2828108809636141,-2.2473038670627479,-2.2123503981961221,-2.1779418190638071,-2.1440696099462189,-2.1107253845944731,-2.0779008881522811,-2.045587995109023,-2.0137787072835578,-1.9824651518383949,-1.9516395793238339,
-1.92129436175167,-1.891421990698072,-1.862015075435256,-1.833066341091558,-1.804568626839524,-1.7765148841116389,-1.748898174843313,-1.7217116697427539,-1.69494864658735,-1.6686024885461881,-1.6426666825283649,
-1.6171348175567,-1.592000583166489,-1.5672577678290049,-1.54290025739928,-1.5189220335879461,-1.4953171724566741,-1.47207984293697,-1.44920430537192,-1.426684910080569,-1.40451609594463,-1.382692389017131,
-1.3612084011527961,-1.34005882865967,-1.319238450971818,-1.2987421293427619,-1.278564805559242,-1.2587015006751361,-1.2391473137651561,-1.2198974206980391,-1.200947072928948,-1.182291596310791,-1.163926389924161,
-1.1458469249256349,-1.1280487434141351,-1.110527457315081,-1.0932787472820751,-1.076298361615831,-1.059582115200103,-1.043125888454338,-1.026925626302809,-1.0109773371599591,-0.995277091931727,-0.979821023032588,
-0.964605323418079,-0.949626245632565,-0.934880100872009,-0.920363258061519,-0.90607214294743,-0.892003237203715,-0.878153077552473,-0.864518254898307,-0.851095413476352,-0.837881250013747,-0.824872512904343,
-0.812066001396436,-0.79945856479333,-0.787047101666507,-0.774828559081239,-0.76279993183441264,-0.75095826170440161,-0.7393006367127809,-0.72782419039770285,-0.71652610109875114,-0.70540359125309027,-0.69445392670273365,
-0.68367441601275525,-0.67306240980027021,-0.66261530007401659,-0.65233051958436894,-0.6422055411836205,-0.63223787719636981,-0.622425078799854,-0.61276473541406906,-0.60325447410152466,-0.593891958976478,-0.58467489062349953},
new double[]{
-60.954390332512475,-60.621056999179146,-60.28772366584581,-59.954390332512475,-59.621056999179146,-59.28772366584581,-58.954390332512482,-58.621056999179146,-58.287723665845817,-57.954390332512489,-57.621056999179153,-57.287723665845832,
-56.9543903325125,-56.621056999179181,-56.28772366584586,-55.954390332512546,-55.621056999179238,-55.287723665845945,-54.954390332512666,-54.621056999179409,-54.287723665846187,-53.954390332513,-53.621056999179871,
-53.287723665846826,-52.954390332513896,-52.621056999181128,-52.287723665848574,-51.95439033251634,-51.621056999184532,-51.287723665853328,-50.954390332522969,-50.62105699919379,-50.287723665866253,-49.954390332541,
-49.621056999218951,-49.287723665901368,-48.954390332590016,-48.621056999287362,-48.287723665996836,-47.95439033272325,-47.6210569994733,-47.28772366625634,-46.954390333085414,-46.621056999978741,-46.287723666961725,
-45.954390334069849,-45.6210570013526,-45.287723668879046,-44.954390336745583,-44.6210570050867,-44.287723674090039,-43.9543903440174,-43.621057015234,-43.287723688249216,-42.954390363773385,-42.621057042796849,
-42.287723726699831,-41.954390417405214,-41.621057117590482,-41.287723830981555,-40.954390562759272,-40.621057320120791,-40.287724113053024,-39.954390955395382,-39.62105786629558,-39.287724872196932,-38.954392009540477,
-38.62105932842308,-38.287726897525729,-37.954394810717808,-37.621063195856209,-37.287732226435388,-36.9544021369082,-36.6210732426892,-36.287745966071384,-35.954420869531624,-35.6210986981641,-35.2877804332547,
-34.9544673592794,-34.621161146855634,-34.287863954374551,-33.954578551161454,-33.621308465019183,-33.288058156865866,-32.954833224846418,-32.6216406397418,-32.288489012692288,-31.955388895175453,-31.622353109835231,
-31.289397109168661,-30.956539357286783,-30.6238017280506,-30.291209910941213,-29.958793814178186,-29.626587952990608,-29.294631809715,-28.96297015168108,-28.631653292766348,-28.300737285134833,-27.970284029058508,
-27.64036129083004,-27.311042621533506,-26.98240717271009,-26.654539408557639,-26.327528718024823,-26.001468933777559,-25.676457768311515,-25.352596180269643,-25.029987686151181,-24.708737633974831,-24.388952456048397,
-24.070738917821171,-23.754203378924657,-23.439451081051551,-23.126585475417013,-22.815707600337312,-22.506915517094672,-22.200303809867627,-21.895963153208502,-21.59397994843463,-21.294436028432493,-20.997408428794422,
-20.702969221932822,-20.411185409845572,-20.122118870521746,-19.835826352551916,-19.55235951230863,-19.27176498805321,-18.994084505467764,-18.719355009370723,-18.447608816717967,-18.178873786391197,-17.913173501706751,
-17.650527462021596,-17.390951280253571,-17.134456883558013,-16.881052714804671,-16.63074393287112,-16.383532610109064,-16.139417925645919,-15.898396353456093,-15.660461844375382,-15.425606001439462,-15.193818248105819,
-14.965085989070127,-14.73939476351552,-14.516728390739136,-14.297069108187127,-14.080397701999591,-13.866693630222581,-13.655935138887793,-13.448099371193306,-13.243162470042655,-13.041099674215982,-12.841885408457218,
-12.645493367766449,-12.451896596187595,-12.261067560379285,-12.072978218251896,-11.887600082946767,-11.70490428242514,-11.524861614924793,-11.347442600531918,-11.172617529104935,-11.000356504775754,-10.830629487242769,
-10.663406330058608,-10.498656816104703,-10.336350690433902,-10.176457690651942,-10.01894757499848,-9.86379014827873,-9.7109552857875183,-9.5604129553587267,-9.4121332376647651,-9.2660863448827975,-9.1222426378369548,
-8.9805726417187284,-8.8410470604810776,-8.7036367899955778,-8.5683129300560559,-8.4350467953066737,-8.3038099251672772,-8.174574092824006,-8.0473113133486667,-7.9219938510061638,-7.7985942258053242,-7.6770852193448071,
-7.5574398800023541,-7.4396315275124314,-7.3236337569743126,-7.2094204423299226,-7.0969657393480814,-6.9862440881494443,-6.8772302153041309,-6.7698991355319329,-6.6642261530330327,-6.56018686247535,-6.4577571496628732,
-6.3569131919078092,-6.2576314581278281,-6.1598887086883494,-6.0636619950084771,-5.96892865894802,-5.8756663319918738,-5.78385293424701,-5.693466673266335,-5.6044860427127343,-5.5168898208758064,-5.4306570690529448,
-5.3457671298057079,-5.2621996251016956,-5.1799344543515149,-5.0989517923497907,-5.0192320871286054,-4.94075605773123,-4.8635046919134863,-4.7874592437796224,-4.7126012313591472,-4.6389124341306474,-4.56637489049822,
-4.4949708952258414,-4.424682996834524,-4.3554939949670128,-4.2873869377242269,-4.2203451189775611,-4.1543520756608077,-4.08939158504524,-4.0254476620011577,-3.9625045562489918,-3.900546749602845,-3.8395589532091661,
-3.779526104783073,-3.7204333658446629,-3.66226611895751,-3.60500996497135,-3.5486507202709112,-3.493174414032616,-3.4385672854907541,-3.3848157812147481,-3.33190655239885,-3.27982645216559,-3.22856253288426,
-3.1781020435053828,-3.128432426912469,-3.0795413172917372,-3.0314165375208781,-2.98404609657757,-2.93741818696853,-2.8915211821797522,-2.8463436341485369,-2.8018742707579491,-2.7581019933541051,-2.7150158742868218,
-2.67260515447401,-2.6308592409901959,-2.5897677046794558,-2.549320277793115,-2.5095068516523971,-2.4703174743362788,-2.431742348394716,-2.393771828587393,-2.35639641964813,-2.31960677407503,-2.28339368994645,
-2.24774810876282,-2.2126611133143692,-2.17812392557476,-2.14412790462055,-2.11066454457656,-2.07772547258697,-2.0453024468122338,-2.0133873544515168,-1.9819722097908,-1.95104915227635,-1.92061044461352,
-1.8906484708907949,-1.8611557347287651,-1.8321248574541009,-1.8035485762982,-1.77541974262039,-1.7477313201555471,-1.7204763832858521,-1.6936481153366021,-1.66723980689579,-1.64124485415731,-1.61565675728752,
-1.590469118815008,-1.565675642043322,-1.5412701294864151,-1.51724648132662,-1.4935986938949051,-1.4703208581731879,-1.4474071583184689,-1.4248518702085551,-1.4026493600091281,-1.3807940827619349,-1.359280580993846,
-1.3381034833465479,-1.3172575032266281,-1.29673743747581,-1.2765381650610981,-1.2566546457845891,-1.237081919012708,-1.2178151024246291,-1.198849390779634,-1.180180054703176,-1.1618024394914031,-1.143711963933906,
-1.1259041191544421,-1.108374467469416,-1.0911186412638609,-1.0741323418847051,-1.0574113385510691,-1.0409514672813851,-1.02474862983708,-1.00879879268265,-0.993097985961767,-0.977642302489424,-0.962427896759618,
-0.94745098396857,-0.93270783905315,-0.918194795744324,-0.903908245635397,-0.889844637264846,-0.87600047521353,-0.862372319216062,-0.848956783286136,-0.83575053485562,-0.822750293927183,-0.809952832240284,
-0.797354972450306,-0.784953587320648,-0.772745598927571,-0.76072797787762469,-0.74889774253744312,-0.73725195827573886,-0.72578773671730157,-0.71450223500882415,-0.70339265509637161,-0.6924562430143173,-0.6816902881855702,
-0.671092122732919,-0.66065912080132272,-0.65038869789097842,-0.64027831020099868,-0.63032545398353579,-0.62052766490818945,-0.61088251743653688,-0.60138762420663028,-0.592040635427303,-0.58283923828213269,-0.57378115634290938},
new double[]{
-50.021287074479289,-49.7712870744793,-49.521287074479304,-49.271287074479311,-49.021287074479325,-48.771287074479339,-48.521287074479361,-48.271287074479382,-48.02128707447941,-47.771287074479453,-47.5212870744795,-47.271287074479567,
-47.021287074479652,-46.771287074479758,-46.5212870744799,-46.271287074480078,-46.021287074480306,-45.7712870744806,-45.521287074480973,-45.271287074481457,-45.021287074482082,-44.771287074482878,-44.5212870744839,
-44.271287074485215,-44.021287074486906,-43.771287074489074,-43.521287074491852,-43.271287074495426,-43.021287074500016,-42.771287074505906,-42.521287074513474,-42.271287074523187,-42.021287074535657,-41.771287074551672,
-41.521287074572236,-41.271287074598639,-41.021287074632546,-40.771287074676074,-40.521287074731973,-40.271287074803752,-40.021287074895909,-39.771287075014243,-39.521287075166192,-39.271287075361293,-39.0212870756118,
-38.771287075933472,-38.5212870763465,-38.271287076876838,-38.021287077557808,-37.771287078432188,-37.5212870795549,-37.2712870809965,-37.021287082847536,-36.771287085224294,-36.52128708827609,-36.271287092194633,
-36.021287097226072,-35.771287103686461,-35.521287111981586,-35.271287122632437,-35.021287136307912,-34.771287153866759,-34.521287176411462,-34.271287205357275,-34.021287242520913,-33.771287290234227,-33.521287351489981,
-33.271287430128709,-33.021287531078151,-32.771287660659894,-32.521287826981748,-32.271288040439693,-32.021288314359204,-31.771288665813948,-31.521289116669653,-31.271289694913172,-31.021290436342028,-30.771291386708203,
-30.5212926044326,-30.271294164033975,-30.021296160448831,-29.771298714457632,-29.521301979478245,-29.271306150040207,-29.02131147231383,-28.771318257135725,-28.521326896047174,-28.271337880942529,-28.021351828009465,
-27.771369506729489,-27.521391874790865,-27.271420119843079,-27.021455709085561,-26.771500447726382,-26.521556547360696,-26.271626705294896,-26.021714195770677,-25.771822973914244,-25.521957793041345,-25.272124335680889,
-25.022329358335043,-24.77258084957063,-24.522888200539192,-24.273262386460548,-24.023716156992077,-23.774264232764352,-23.524923504720519,-23.275713232283568,-23.026655235828532,-22.777774078492353,-22.52909723204948,
-22.280655221448708,-22.032481742672456,-21.784613748860949,-21.537091500145884,-21.289958573354166,-21.043261828651644,-20.797051331265916,-20.55138022761237,-20.306304576395608,-20.061883136512172,-19.818177114781676,
-19.575249877626906,-19.333166631761237,-19.09199407968509,-18.851800056316744,-18.612653153373003,-18.374622338172887,-18.137776573374882,-17.902184443798333,-17.667913795953151,-17.435031395245041,-17.20360260507459,
-16.973691091245872,-16.745358554280269,-16.518664491425294,-16.293665989384227,-16.070417548090404,-15.848970935226207,-15.629375070649946,-15.411675939448733,-15.195916531981929,-14.982136809014143,-14.77037368985286,
-14.56066106129536,-14.353029805143443,-14.147507842052583,-13.944120189535106,-13.742889032025081,-13.543833801027619,-13.346971263509092,-13.152315616830956,-12.959878588682514,-12.769669540622372,-12.5816955739908,
-12.395961637102644,-12.212470632770836,-12.031223525342105,-11.852219446548507,-11.675455799590148,-11.500928360965775,-11.328631379659015,-11.158557673369129,-10.990698721546726,-10.825044755057627,-10.66158484235249,
-10.500306972066669,-10.341198132014826,-10.184244384578525,-10.029430938513405,-9.8767422172257877,-9.726161923587652,-9.57767310137409,-9.43125819341935,-9.2868990965965974,-9.1445772137332462,-9.0042735025781919,
-8.8659685219401734,-8.7296424751177462,-8.5952752507414942,-8.462846461148164,-8.33233547840469,-8.2037214680976724,-8.0769834210009286,-7.9521001827304705,-7.8290504814925708,-7.707812954026819,-7.5883661698420308,
-7.4706886538388586,-7.3547589074088533,-7.2405554280956492,-7.1280567278999207,-7.0172413503057838,-6.90808788610247,-6.8005749880713031,-6.69468138460442,-6.5903858923181069,-6.48766742772029,-6.386505017988453,
-6.2868778109111867,-6.1887650840435757,-6.0921462531238566,-5.9970008797960723,-5.903308678680899,-5.8110495238344431,-5.72020345463246,-5.6307506811153374,-5.5426715888270941,-5.455946743179739,-5.3705568933724823,
-5.2864829758935761,-5.2037061176309409,-5.1222076386161852,-5.0419690544251816,-4.96297207825701,-4.8851986227117727,-4.8086308012866139,-4.7332509296080891,-4.659041526418001,-4.58598531432879,-4.5140652203636149,
-4.4432643762953861,-4.3735661187981449,-4.3049539894234314,-4.2374117344134818,-4.1709233043624776,-4.1054728537363152,-4.041044740260836,-3.9776235241878091,-3.915193967447443,-3.8537410326956771,-3.793249882264015,
-3.7337058770192222,-3.6750945751397448,-3.6174017308153532,-3.5606132928760639,-3.5047154033561232,-3.449694395998395,-3.3955367947042712,-3.3422293119338571,-3.289758847060944,-3.2381124846869711,-3.18727749291798,
-3.1372413216083062,-3.0879916005744481,-3.0395161377825648,-2.9918029175125649,-2.94484009850179,-2.8986160120710118,-2.853119160235329,-2.8083382138023758,-2.764262010460127,-2.72087955285641,-2.6781800066721591,
-2.636152698690251,-2.594787114861707,-2.5540728983708951,-2.51399984770128,-2.47455791470315,-2.43573720266472,-2.39752796438779,-2.35992060026916,-2.32290565638904,-2.2864738226071881,-2.2506159306680549,
-2.2153229523155531,-2.1805859974184529,-2.1463963121070861,-2.11274527692208,-2.07962440497578,-2.04702534012698,-2.0149398551694828,-1.9833598500350429,-1.9522773500111239,-1.9216845039739341,-1.89157358263711,
-1.8619369768164089,-1.832767195710745,-1.80405686519983,-1.7757987261587409,-1.7479856327895269,-1.720610550970237,-1.6936665566213731,-1.66714683409004,-1.64104467455187,-1.61535347443087,-1.590066733837243,
-1.56517805502329,-1.54068114085745,-1.5165697933165,-1.49283791199591,-1.46947949263843,-1.4464886256809,-1.423859494819113,-1.401586375590973,-1.37966363397763,-1.35808572502273,-1.3368471914696,
-1.31594266241639,-1.2953668519890149,-1.275114558031818,-1.255180660815916,-1.23556012176505,-1.21624798219887,-1.19723936209352,-1.1785294588594,-1.160113546136037,-1.14198697260376,-1.124145160812285,
-1.106583606025856,-1.0892978750849161,-1.07228360528411,-1.05553650326647,-1.039052343933677,-1.0228269693721379,-1.0068562877948291,-0.991136272498673,-0.975662960837308,-0.960432453209083,-0.945440912060104,
-0.930684560902177,-0.916159683345457,-0.901862622145653,-0.887789778265608,-0.873937609951081,-0.860302631820564,-0.84688141396896,-0.833670581084948,-0.820666811581868,-0.807866836741949,-0.795267439873707,
-0.782865455482355,-0.770657768453038,-0.75864131324672845,-0.74681307310862022,-0.73517007928884637,-0.72370941027535118,-0.71242819103875521,-0.70132359228904473,-0.69039282974392246,-0.67963316340865554,-0.66904189686725923,
-0.65861637658485417,-0.64835399122104054,-0.63825217095412656,-0.62830838681605972,-0.61852015003789951,-0.60888501140568341,-0.59940056062652869,-0.59006442570482287,-0.58087427232835,-0.57182780326420746,-0.56292275776436584},
new double[]{
-42.660078779627213,-42.4600787796273,-42.260078779627406,-42.060078779627538,-41.8600787796277,-41.660078779627895,-41.460078779628127,-41.260078779628415,-41.060078779628768,-40.860078779629205,-40.660078779629728,-40.460078779630372,
-40.260078779631158,-40.060078779632121,-39.860078779633291,-39.660078779634723,-39.460078779636468,-39.260078779638611,-39.060078779641216,-38.860078779644404,-38.660078779648295,-38.460078779653053,-38.260078779658862,
-38.06007877966595,-37.860078779674616,-37.6600787796852,-37.460078779698122,-37.260078779713908,-37.060078779733189,-36.860078779756741,-36.660078779785508,-36.460078779820634,-36.260078779863548,-36.060078779915962,
-35.860078779979972,-35.660078780058164,-35.460078780153665,-35.260078780270305,-35.06007878041278,-34.86007878058679,-34.660078780799331,-34.460078781058925,-34.260078781375995,-34.060078781763266,-33.860078782236286,
-33.660078782814026,-33.460078783519684,-33.26007878438157,-33.060078785434278,-32.860078786720067,-32.660078788290527,-32.460078790208691,-32.260078792551532,-32.060078795413091,-31.860078798908191,-31.660078803177111,
-31.46007880839117,-31.260078814759613,-31.060078822538014,-30.860078832038532,-30.66007884364242,-30.46007885781534,-30.26007887512603,-30.060078896269125,-29.860078922093024,-29.660078953633896,-29.460078992157253,
-29.260079039208662,-29.060079096675707,-28.860079166863628,-28.660079252587632,-28.460079357285647,-28.260079485155874,-28.060079641324709,-27.860079832051603,-27.66008006497902,-27.460080349437238,-27.260080696816047,
-27.060081121017774,-26.860081639009248,-26.660082271493952,-26.460083043730151,-26.260083986525967,-26.060085137448795,-25.860086542293939,-25.660088256866228,-25.460090349138838,-25.260092901865676,-25.060096015738051,
-24.860099813192527,-24.660104442995685,-24.460110085752831,-24.260116960511304,-24.060125332655709,-23.860135523321055,-23.660147920581078,-23.460162992701608,-23.260181303782897,-23.060203532148545,-22.860230491871359,
-22.660263157856164,-22.460302694924451,-22.260350491363205,-22.060408197407956,-21.860477769124493,-21.660561518132319,-21.460662167571765,-21.260782914653312,-21.060927500038677,-20.861100284186715,-20.661306330651612,
-20.461551496145809,-20.261842526976714,-20.062187161237322,-19.862594235880589,-19.663073797542474,-19.463637215707095,-19.26429729653951,-19.065068395458766,-18.865966526298486,-18.667009464717438,-18.468216843390664,
-18.269610236444546,-18.071213230606432,-17.873051480627584,-17.675152746711589,-17.477546911937971,-17.280265978008593,-17.083344038053802,-16.886817225703243,-16.690723640137556,-16.495103247372512,-16.299997758567162,
-16.105450486671383,-15.91150618321614,-15.718210857483943,-15.525611580662357,-15.333756277868147,-15.142693511126407,-14.952472256494509,-14.763141678535522,-14.574750905274923,-14.387348806625585,-14.200983779050027,
-14.015703538957906,-13.831554927024362,-13.648583725274541,-13.466834488424926,-13.286350390615059,-13.107173088314816,-12.929342599861755,-12.752897201777415,-12.577873341736249,-12.404305567819689,-12.232226473482527,
-12.061666657489782,-11.892654697948734,-11.725217139460897,-11.55937849235006,-11.395161242881727,-11.232585873373408,-11.071670891100331,-10.912432864924119,-10.754886468609215,-10.599044529840452,-10.444918084012054,
-10.292516431921214,-10.141847200565854,-9.9929164063143912,-9.84572851978382,-9.70028653182967,-9.5565920201167387,-9.4146452158017659,-9.2744450699182472,-9.135989319108603,-8.9992745504000649,-8.8642962647675123,
-8.7310489392693054,-8.5995260875808057,-8.4697203187850647,-8.3416233943112,-8.2152262829383833,-8.0905192138075783,-7.9674917274043295,-7.8461327244941153,-7.7264305130076547,-7.6083728528869132,-7.4919469989139742,
-7.3771397415544175,-7.2639374458546762,-7.152326088439227,-7.04229129265854,-6.9338183619426381,-6.8268923114181,-6.7214978978484066,-6.6176196479589056,-6.5152418852084262,-6.4143487550697165,-6.3149242488806907,
-6.2169522263277877,-6.1204164366218423,-6.0253005384256442,-5.9315881185909953,-5.8392627097614866,-5.7483078068955562,-5.6587068827626075,-5.570443402463134,-5.4835008370219214,-5.3978626761015009,-5.3135124398811469,
-5.2304336901447828,-5.1486100406193414,-5.0680251666032659,-4.988662813923054,-4.9105068072539746,-4.8335410578394322,-4.757749570641761,-4.68311645095567,-4.6096259105140218,-4.5372622731141412,-4.466009979791445,
-4.3958535935658327,-4.3267778037849691,-4.2587674300873317,-4.1918074260067648,-4.12588288223908,-4.0609790295902179,-3.9970812416244481,-3.93417503703008,-3.8722460817192759,-3.8112801906776341,-3.751263329578391,
-3.6921816161753132,-3.634021321487543,-3.576768870789016,-3.5204108444143292,-3.46493397839233,-3.4103251649180839,-3.356571452673287,-3.3036600470046542,-3.251578309969295,-3.2003137602556069,-3.1498540729877171,
-3.1001870794211239,-3.0513007665367158,-3.0031832765399851,-2.9558229062718691,-2.9092081065373159,-2.8633274813572971,-2.8181697871497349,-2.7737239318444562,-2.7299789739370328,-2.6869241214861042,-2.6445487310584919,
-2.602842306626231,-2.56179449841935,-2.52139510173807,-2.4816340557278851,-2.4425014421207392,-2.403987483945405,-2.3660825442099651,-2.3287771245591129,-2.2920618639088759,-2.25592753706119,-2.22036505330062,
-2.1853654549753951,-2.15091991606481,-2.1170197407348961,-2.083656361884199,-2.0508213396813448,-2.0185063600960231,-1.9867032334248811,-1.955403892813764,-1.924600392777625,-1.8942849077193671,-1.864449730448795,
-1.83508727070278,-1.80619005366768,-1.77775071850497,-1.74976201688101,-1.72221681150183,-1.69510807465362,-1.6684288867498,-1.642172434885294,-1.61633201139866,-1.59090101244265,-1.5658729365638311,
-1.541241383291712,-1.5170000517378941,-1.493142739205674,-1.46966333981051,-1.44655584311173,-1.42381433275583,-1.4014329851316589,-1.3794060680378051,-1.3577279393624351,-1.33639304577581,-1.3153959214357129,
-1.2947311867059621,-1.274393546888196,-1.254377790967063,-1.2346787903689631,-1.215291497734446,-1.19621094570437,-1.17743224571987,-1.15895058683629,-1.14076123455102,-1.12285952964531,-1.10524088704016,
-1.08790079466623,-1.070834812347728,-1.0540385707004121,-1.037507770043538,-1.02123817932582,-1.00522563506535,-0.98946604030336,-0.97395536357195,-0.95868963787548,-0.943664959685786,-0.928877487951015,
-0.914323443118032,-0.899999106168341,-0.885900817667399,-0.87202497682725,-0.85836804058239,-0.8449265226787,-0.831696992775442,-0.818676075560127,-0.805860449876176,-0.793246847863255,-0.780832054110167,
-0.768612904820175,-0.756586286988646,-0.7447491375928772,-0.73309844279398972,-0.72163123715075594,-0.71034460284523382,-0.69923566892007816,-0.68830161052739658,-0.67753964818901891,-0.66694704706804653,-0.6565211162515493,
-0.64625920804427417,-0.63615871727323348,-0.62621708060303682,-0.61643177586183273,-0.60680032137772566,-0.59732027532553278,-0.58798923508374756,-0.57880483660157611,-0.569764753775911,-0.56086669783811183,-0.55210841675045808},
new double[]{
-37.333518072665008,-37.166851405999175,-37.000184739333491,-36.833518072667992,-36.6668514060027,-36.500184739337655,-36.3335180726729,-36.166851406008504,-36.000184739344512,-35.833518072681009,-35.666851406018083,-35.500184739355824,
-35.333518072694375,-35.166851406033864,-35.000184739374475,-34.833518072716409,-34.666851406059891,-34.500184739405221,-34.333518072752732,-34.166851406102808,-34.000184739455918,-33.833518072812616,-33.666851406173556,
-33.5001847395395,-33.33351807291136,-33.166851406290206,-33.0001847396773,-32.833518073074153,-32.666851406482522,-32.500184739904505,-32.333518073342553,-32.1668514067996,-32.000184740279089,-31.833518073785079,
-31.666851407322383,-31.500184740896678,-31.333518074514675,-31.166851408184296,-31.000184741914911,-30.833518075717571,-30.666851409605353,-30.500184743593689,-30.333518077700816,-30.166851411948279,-30.00018474636153,
-29.833518080970631,-29.666851415811109,-29.500184750924923,-29.333518086361643,-29.166851422179832,-29.000184758448675,-28.833518095249904,-28.666851432680065,-28.500184770853227,-28.333518109904137,-28.166851449991981,
-28.000184791304815,-27.8335181340648,-27.666851478534383,-27.500184825023613,-27.333518173898764,-27.166851525592524,-27.000184880616061,-26.83351823957323,-26.666851603177392,-26.500184972271271,-26.333518347850394,
-26.166851731090812,-26.000185123381808,-25.833518526364575,-25.666851941977843,-25.500185372511844,-25.333518820671969,-25.166852289654024,-25.000185783233061,-24.83351930586829,-24.666852862826968,-24.5001864603307,
-24.33352010572813,-24.166853807698846,-24.000187576494042,-23.833521424220567,-23.66685536517609,-23.500189416244556,-23.333523597362575,-23.166857932069458,-23.000192448155588,-22.833527178426458,-22.666862161602712,
-22.500197443379818,-22.333533077675035,-22.166869128093936,-22.000205669653806,-21.833542790807428,-21.666880595817407,-21.500219207538848,-21.33355877067704,-21.16689945559617,-21.000241462765985,-20.833585027944977,
-20.66693042821143,-20.500277988967326,-20.333628092054674,-20.166981185138887,-20.000337792529219,-19.833698527621621,-19.667064107164055,-19.500435367557774,-19.333813283419577,-19.167198988638578,-19.000593800165706,
-18.833999244773732,-18.667417089018794,-18.500849372620223,-18.334298445451967,-18.167767008305706,-18.001258157540885,-17.834775433679955,-17.668322873937367,-17.501905068587959,-17.335527220984858,-17.169195210929431,
-17.00291566097809,-16.83669600514483,-16.670544559327325,-16.504470592652186,-16.33848439880542,-16.172597366292724,-16.006822046465768,-15.841172218060667,-15.67566294692868,-15.510310639601814,-15.34513308933137,
-15.180149513269271,-15.015380579531913,-14.850848422995135,-14.686576648815704,-14.522590322856807,-14.358915948408351,-14.195581428831602,-14.032616016015005,-13.870050244795975,-13.707915853773576,-13.54624569320058,
-13.385073620892038,-13.224434387313329,-13.064363511206853,-12.904897147277573,-12.746071947579386,-12.587924918324525,-12.430493273875973,-12.27381428967913,-12.117925155846018,-11.962862833026678,-11.808663912092635,
-11.655364479021769,-11.502999986218105,-11.351605131330107,-11.201213744452389,-11.051858684414137,-10.903571744677626,-10.756383569196441,-10.610323578419035,-10.465419905471723,-10.321699342418505,-10.179187296374435,
-10.037907755145429,-9.89788326198064,-9.7591348989534055,-9.621682278432715,-9.4855435420679246,-9.3507353666840274,-9.2172729764714969,-9.0851701608523783,-8.9544392974110547,-8.825091379292795,-8.6971360464941228,
-8.5705816204951049,-8.4454351417134639,-8.3217024092930139,-8.1993880227732134,-8.0784954252219467,-7.9590269474491,-7.8409838529536566,-7.7243663832913,-7.6091738035826193,-7.49540444791356,-7.3830557644096206,
-7.2721243597932785,-7.1626060432601957,-7.0544958695337936,-6.9477881809799111,-6.8424766486833759,-6.7385543124066016,-6.6360136193667678,-6.5348464617829007,-6.4350442131572736,-6.3365977632672026,-6.2394975518535061,
-6.143733601000859,-6.0492955462130045,-5.9561726661924776,-5.8643539113402081,-5.7738279309951457,-5.6845830994381519,-5.5966075406876428,-5.5098891521172275,-5.4244156269276846,-5.3401744755072578,-5.2571530457154854,
-5.1753385421265889,-5.09471804426896,-5.0152785238975106,-4.9370068613356146,-4.8598898609231664,-4.783914265606855,-4.7090667707082252,-4.6353340369044069,-4.562702702455649,-4.49115939471292,-4.4206907409379586,
-4.3512833784671647,-4.2829239642497718,-4.2155991837896787,-4.1492957595193394,-4.0840004586330361,-4.0197001004058546,-3.9563815630236445,-3.8940317899482353,-3.8326377958411926,-3.7721866720684112,-3.712665591806898,
-3.6540618147741668,-3.5963626915997629,-3.5395556678575639,-3.483628287776658,-3.4285681976477851,-3.3743631489415291,-3.3210010011537121,-3.2684697243926841,-3.216757401722536,-3.165852231275557,-3.1157425281466389,
-3.0664167260817119,-3.0178633789716862,-2.9700711621628382,-2.9230288735940282,-2.8767254347706079,-2.8311498915844209,-2.7862914149887961,-2.74213930153701,-2.698682973792256,-2.6559119806167661,-2.6138159973473312,
-2.5723848258641011,-2.531608394559218,-2.49147675821146,-2.4519800977728021,-2.4131087200724739,-2.374853057443802,-2.3372036672788772,-2.3001512315158079,-2.2636865560630839,-2.227800570165325,-2.1924843257144948,
-2.1577289965104338,-2.123525877474338,-2.0898663838186771,-2.0567420501768132,-2.0241445296954241,-1.9920655930926929,-1.960497127685028,-1.929431136384965,-1.8988597366727511,-1.868775159543975,-1.839169748435469,
-1.8100359581316181,-1.7813663536530651,-1.753153609129712,-1.7253905066598,-1.698069935156771,-1.6711848891854959,-1.6447284677893821,-1.61869387330979,-1.5930744101990939,-1.567863483828662,-1.543054599292939,
-1.518641360210772,-1.494617467525027,-1.470976718301501,-1.44771300452807,-1.42482031191492,-1.4022927186968,-1.38012439443787,-1.35830959884014,-1.3368426805559219,-1.3157180760051179,-1.294930308197876,
-1.274473985563148,-1.2543438007837271,-1.2345345296382,-1.2150410298503029,-1.195858239946076,-1.1769811781192141,-1.15840494110499,-1.14012470306303,-1.12213571446936,-1.104433301017826,-1.0870128625313309,
-1.0698698718830271,-1.0529998739276669,-1.0363984844433669,-1.020061389083905,-1.00398434234174,-0.988163166521891,-0.972593750726778,-0.957272049852165,-0.942194083594268,-0.927355935468132,-0.91275375183733,
-0.89838374095506,-0.88424217201661,-0.87032537422338,-0.85662973585828,-0.84315170337266,-0.82988778048478,-0.81683452728967,-0.80398855938052,-0.791346546981555,-0.77890521409223,-0.766661337642912,
-0.754611746661839,-0.742753321453405,-0.731082992787662,-0.71959774110102326,-0.70829459570806708,-0.697170634024393,-0.68622298080045063,-0.67544880736626467,-0.664845330886975,-0.65440981362910844,-0.644139562237494,
-0.63403192702273148,-0.62408430125911984,-0.61429412049295251,-0.60465886186107742,-0.595176043419627,-0.58584322348281292,-0.576657999971684,-0.56761800977274268,-0.5587209281063138,-0.54996446790455766,-0.5413463791990214},
new double[]{
-33.282858676938496,-33.140001534086643,-32.997144391235594,-32.854287248385482,-32.711430105536458,-32.56857296268867,-32.425715819842324,-32.282858676997641,-32.140001534154862,-31.997144391314293,-31.854287248476268,-31.711430105641181,
-31.56857296280948,-31.425715819981683,-31.282858677158398,-31.140001534340307,-30.997144391528217,-30.854287248723043,-30.711430105925849,-30.568572963137864,-30.425715820360498,-30.282858677595382,-30.1400015348444,
-29.997144392109721,-29.854287249393845,-29.711430106699666,-29.568572964030508,-29.42571582139022,-29.282858678783235,-29.140001536214665,-28.997144393690409,-28.854287251217272,-28.711430108803107,-28.568572966456969,
-28.4257158241893,-28.282858682012154,-28.140001539939433,-27.997144397987171,-27.854287256173865,-27.711430114520859,-27.568572973052763,-27.425715831797977,-27.282858690789258,-27.14000155006439,-26.997144409666966,
-26.854287269647269,-26.711430130063302,-26.568572990981977,-26.425715852480486,-26.282858714647869,-26.14000157758684,-25.997144441415891,-25.8542873062717,-25.711430172311942,-25.5685730397185,-25.4257159087012,
-25.282858779502064,-25.140001652400304,-24.997144527717992,-24.854287405826668,-24.711430287154911,-24.568573172197127,-24.425716061523623,-24.2828589557923,-24.140001855762058,-23.997144762308345,-23.854287676441018,
-23.711430599325002,-23.568573532304082,-23.425716476928365,-23.282859434985951,-23.14000240853947,-22.997145399968215,-22.854288412016729,-22.711431447850888,-22.568574511122527,-22.425717606044007,-22.282860737474206,
-22.14000391101769,-21.997147133139052,-21.8542904112948,-21.711433754085387,-21.568577171430523,-21.425720674771281,-21.282864277303094,-21.140007994244289,-20.997151843145623,-20.854295844246959,-20.711440020888215,
-20.568584399982818,-20.425729012562964,-20.282873894407523,-20.140019086764855,-19.997164637184692,-19.854310600475191,-19.71145703980357,-19.568604027961335,-19.425751648818022,-19.282899998990544,-19.140049189759043,
-18.997199349264044,-18.854350625024264,-18.711503186819392,-18.568657229987533,-18.4258129791929,-18.282970692725684,-18.140130667402794,-17.997293244145336,-17.854458814316132,-17.711627826908192,-17.568800796682872,
-17.425978313363885,-17.283161052000565,-17.140349784620408,-16.997545393296278,-16.85474888475796,-16.71196140667994,-16.569184265777288,-16.426418947838457,-16.283667139817524,-16.140930754097703,-15.998211955022898,
-15.855513187773616,-15.71283720963735,-15.570187123691401,-15.427566414877354,-15.284978988401329,-15.142429210342693,-14.999921950296416,-14.857462625811614,-14.715057248321699,-14.572712470191597,-14.430435632435991,
-14.288234812591677,-14.146118872158942,-14.004097502963779,-13.862181271737352,-13.720381662163856,-13.578711113615395,-13.437183055774911,-13.295811938347647,-13.154613255079505,-13.013603561338227,-12.872800484570732,
-12.732222727026997,-12.591890060236604,-12.451823310836526,-12.31204433747569,-12.172575998660054,-12.033442111547863,-11.894667401854596,-11.756277445176488,-11.618298600186423,-11.48075793429237,-11.343683142472585,
-11.207102460110258,-11.071044570740611,-10.935538509693354,-10.800613564661896,-10.666299174257061,-10.532624825607492,-10.399619952052504,-10.267313831937075,-10.135735489465187,-10.00491359849917,-9.8748763901116234,
-9.7456515646058826,-9.6172662086235654,-9.4897467178563826,-9.36311872577661,-9.2374070386989811,-9.1126355773882022,-8.9888273253326929,-8.8660042837180448,-8.7441874330539591,-8.6233967013371764,-8.5036509385703027,
-8.3849678974029338,-8.2673642196168142,-8.1508554281409324,-8.0354559242547783,-7.9211789896180758,-7.8080367927525423,-7.6960403995947742,-7.5851997877385493,-7.4755238639890118,-7.3670204848594514,-7.2596964796531536,
-7.15355767578734,-7.048608926032955,-6.9448541373624062,-6.8422963011168418,-6.7409375242247105,-6.6407790612238324,-6.5418213468596251,-6.4440640290523019,-6.3475060020454785,-6.2521454395675633,-6.1579798278554323,
-6.0650059984070461,-5.9732201603458526,-5.8826179322949619,-5.7931943736731313,-5.7049440153376052,-5.6178608895108173,-5.5319385589388617,-5.4471701452395829,-5.363548356407132,-5.2810655134478868,-5.1997135761299136,
-5.1194841678345435,-5.0403685995044,-4.9623578926871739,-4.8854428016788756,-4.8096138347741038,-4.73486127463412,-4.66117519778638,-4.588545493271492,-4.5169618804555878,-4.4464139260277067,-4.3768910602031035,
-4.3083825921544365,-4.2408777246935685,-4.17436556822728,-4.108835154010583,-4.044275446721489,-3.9806753563812021,-3.9180237496435715,-3.856309460477517,-3.7955213002658308,-3.7356480673434245,-3.6766785559976731,
-3.6186015649530305,-3.5614059053615827,-3.5050804083206422,-3.4496139319379258,-3.394995367964238,-3.3412136480129888,-3.2882577493852292,-3.236116700518282,-3.1847795860753987,-3.13423555169326,-3.0844738084034975,
-3.0354836367438316,-2.9872543905737832,-2.9397755006093469,-2.8930364776904351,-2.847026915794312,-2.8017364948077339,-2.75715498306992,-2.71327223969801,-2.6700782167061332,-2.627562960928739,-2.58571661575837,
-2.5445294227076052,-2.5039917228044568,-2.4640939578301082,-2.4248266714074518,-2.3861805099485158,-2.3481462234684751,-2.3107146662736229,-2.273876797530292,-2.237623681721423,-2.2019464889971379,-2.1668364954253931,
-2.13228508314849,-2.0982837404509458,-2.0648240617439679,-2.03189774747152,-1.99949660394273,-1.967612543095155,-1.9362375821932121,-1.90536384346585,-1.8749835536873729,-1.8450890437050911,-1.815672747917334,
-1.7867272037051689,-1.758245050820983,-1.730219030736968,-1.702641985956378,-1.6755068592902569,-1.6488066931022609,-1.6225346285239981,-1.5966839046432451,-1.571247857667228,-1.5462199200630871,-1.5215936196775091,
-1.49736257883741,-1.473520513433467,-1.4500612319881969,-1.426978634710171,-1.404266712535921,-1.381919546160943,-1.3599313050611881,-1.3382962465063171,-1.3170087145659459,-1.2960631391100419,-1.275454034804534,
-1.2551760001032111,-1.2352237162368309,-1.215591946200403,-1.1962755337394659,-1.1772694023362109,-1.158568554196197,-1.1401680692363809,-1.122063104075149,-1.1042488910249819,-1.086720737088348,-1.069474022957392,
-1.05250420201794,-1.03580679935833,-1.01937741078346,-1.00321170183461,-0.98730540681527,-0.97165432782356,-0.9562543337913,-0.94110135953035,-0.926191404786345,-0.911520533300077,-0.897084871876902,
-0.882880609464295,-0.868903996237819,-0.855151342695677,-0.841619018762034,-0.828303452899268,-0.815201131229286,-0.802308596664041,-0.789622448045358,-0.77713933929417,-0.764855978569257,-0.752769127435554,
-0.74087560004209863,-0.72917226230966647,-0.71765603112814336,-0.70632387356365656,-0.6951728060754977,-0.68419989374284684,-0.67340224950130567,-0.66277703338923877,-0.652321451803913,-0.64203275676741955,-0.63190824520235778,
-0.621945258217251,-0.61214118040166077,-0.60249343913096043,-0.5929995038807232,-0.58365688555067607,-0.57446313579816488,-0.56541584638107423,-0.55651264851013948,-0.54775121221058631,-0.53912924569302911,-0.530644494733557},
new double[]{
-30.088544277998409,-29.963544278022869,-29.838544278050588,-29.713544278081994,-29.588544278117585,-29.463544278157915,-29.338544278203614,-29.213544278255398,-29.088544278314078,-28.963544278380571,-28.838544278455913,-28.713544278541292,
-28.588544278638036,-28.463544278747666,-28.338544278871886,-28.213544279012652,-28.088544279172158,-27.9635442793529,-27.838544279557709,-27.71354427978979,-27.588544280052769,-27.463544280350767,-27.338544280688442,
-27.213544281071076,-27.088544281504657,-26.963544281995972,-26.8385442825527,-26.713544283183559,-26.588544283898415,-26.463544284708451,-26.338544285626345,-26.213544286666455,-26.088544287845053,-25.963544289180579,
-25.838544290693928,-25.713544292408777,-25.588544294351955,-25.463544296553867,-25.338544299048955,-25.213544301876262,-25.088544305080021,-24.963544308710354,-24.838544312824059,-24.7135443174855,-24.5885443227676,
-24.463544328753002,-24.338544335535349,-24.213544343220757,-24.088544351929457,-23.963544361797705,-23.838544372979889,-23.713544385650955,-23.588544400009148,-23.463544416279102,-23.338544434715356,-23.213544455606357,
-23.088544479278934,-22.963544506103446,-22.838544536499565,-22.71354457094283,-22.5885446099721,-22.463544654197971,-22.338544704312344,-22.213544761099232,-22.088544825447034,-21.963544898362422,-21.838544980986093,
-21.713545074610607,-21.58854518070061,-21.463545300915726,-21.338545437136514,-21.21354559149389,-21.088545766402422,-20.963545964598104,-20.838546189181113,-20.713546443664278,-20.588546732027986,-20.463547058782389,
-20.33854742903787,-20.2135478485849,-20.088548323984465,-19.96354886267056,-19.838549473066223,-19.713550164715034,-19.588550948430029,-19.463551836462369,-19.338552842692366,-19.21355398284582,-19.08855527473899,
-18.96355673855598,-18.838558397162775,-18.713560276462751,-18.588562405799085,-18.463564818410166,-18.338567551944941,-18.213570649045902,-18.088574158008605,-17.96357813352741,-17.838582637538643,-17.713587740173587,
-17.588593520835222,-17.463600069414415,-17.338607487662994,-17.213615890743327,-17.088625408976128,-16.963636189810821,-16.838648400045404,-16.713662228325656,-16.588677887956738,-16.463695620063554,-16.338715697139822,
-16.213738427029632,-16.088764157389125,-15.963793280680033,-15.83882623975097,-15.713863534066453,-15.588905726647615,-15.463953451792456,-15.339007423646871,-15.21406844570066,-15.089137421284935,-14.964215365148705,
-14.839303416192557,-14.714402851436111,-14.589515101293024,-14.46464176622235,-14.339784634817889,-14.214945703387409,-14.090127197060907,-13.965331592451452,-13.840561641873068,-13.715820399097648,-13.59111124660701,
-13.46643792426673,-13.341804559315825,-13.217215697530628,-13.092676335383102,-12.968191952973662,-12.843768547477367,-12.719412666800704,-12.595131443105476,-12.470932625817449,-12.346824613701831,-12.22281648555658,
-12.098918029049358,-11.975139767205913,-11.851492982048042,-11.727989734878987,-11.604642882724219,-11.481466090456321,-11.358473838164684,-11.23568142337381,-11.113104957767574,-10.990761358140437,-10.868668331368822,
-10.746844353275373,-10.625308641343917,-10.50408112133151,-10.383182387913987,-10.262633659590609,-10.142456728159658,-10.022673903157823,-9.9033079517302323,-9.7843820344630377,-9.6659196377652812,-9.5479445034301982,
-9.4304805560373062,-9.31355182887542,-9.197182389072875,-9.08139626261532,-8.9662173599139834,-8.8516694025593612,-8.7377758518579078,-8.6245598397040784,-8.5120441022881828,-8.40025091708375,-8.2892020434977862,
-8.1789186675050978,-8.0694213505249728,-7.9607299827364724,-7.8528637409682034,-7.7458410512410616,-7.6396795559885256,-7.5343960859295036,-7.4300066365238466,-7.32652634890073,-7.2239694951154467,-7.1223494675605821,
-7.0216787723331651,-6.92196902633989,-6.8232309579076613,-6.7254744106562354,-6.6287083503832269,-6.53294087470881,-6.43817922522773,-6.3444298019192784,-6.2516981795713225,-6.1599891259819,-6.0693066217109717,
-5.9796538811652677,-5.8910333748105437,-5.8034468523175917,-5.7168953664609008,-5.6313792976016082,-5.5468983785991917,-5.46345172000908,-5.3810378354357828,-5.2996546669232734,-5.2192996102760114,-5.1399695402151373,
-5.0616608352849655,-4.9843694024348935,-4.9080907012112194,-4.8328197675021061,-4.7585512367870582,-4.6852793668497466,-4.6129980599199589,-4.5417008842166888,-4.4713810948701811,-4.4020316542059108,-4.3336452513781714,
-4.2662143213451715,-4.199731063181285,-4.1341874577254485,-4.0695752845676463,-4.00588613837801,-3.94311144458534,-3.8812424744137832,-3.820270359288092,-3.760186104619327,-3.7009806029840453,-3.6426446467110094,
-3.585168939890266,-3.5285441098200687,-3.4727607179076108,-3.4178092700398941,-3.3636802264412875,-3.3103640110344679,-3.2578510203214845,-3.2061316318016324,-3.1551962119427368,-3.1050351237222644,-3.055638733754475,
-3.0069974190195543,-2.9591015732103818,-2.9119416127122508,-2.86550798223051,-2.8197911600807259,-2.7747816631555722,-2.7304700515822522,-2.6868469330838609,-2.6439029670576719,-2.601628868382917,-2.5600154109702218,
-2.519053431064437,-2.4787338303121831,-2.4390475786050549,-2.3999857167089829,-2.3615393586899009,-2.3236996941454469,-2.28645799025207,-2.2498055936365442,-2.2137339320805132,-2.17823451606637,-2.1432989401724059,
-2.10891888432487,-2.0750861149142139,-2.0417924857825431,-2.0090299390889581,-1.976790506059197,-1.945066307625704,-1.9138495549640111,-1.883132549931013,-1.8529076854105111,-1.82316744557115,-1.7939044060416249,
-1.765111234007847,-1.736780688236522,-1.708905619029389,-1.6814789681122111,-1.6544937684623591,-1.627943144078714,-1.6018203096973991,-1.57611857045671,-1.5508313215144449,-1.525952047620686,-1.50147432264894,
-1.4773918090884239,-1.4536982575001081,-1.43038750593905,-1.407453479345403,-1.384890188906369,-1.3626917313912761,-1.3408522884618119,-1.3193661259594081,-1.298227593171593,-1.2774311220791219,-1.2569712265855351,
-1.23684250173075,-1.2170396228902081,-1.1975573449609971,-1.1783905015363341,-1.159534004069682,-1.140982841029738,-1.122732077047456,-1.104776852056194,-1.0871123804260441,-1.069733950093309,-1.052636921686084,
-1.03581672764681,-1.019268871352623,-1.002988926234315,-0.986972534894618,-0.971215408226531,-0.95571332453234,-0.940462128643958,-0.925457731045162,-0.910696106996288,-0.896173295661887,-0.881885399241833,
-0.867828582106339,-0.853999069935293,-0.84039314886233,-0.82700716462401,-0.81383752171438,-0.80088068254539,-0.78813316661331,-0.77559154967149,-0.76325246290971,-0.75111259214041,-0.73916867699193456,
-0.72741751010897959,-0.71585593636056977,-0.70448085205556155,-0.6932892041659241,-0.68227798955790375,-0.67144425423119891,-0.66078509256625706,-0.65029764657978884,-0.63997910518858614,-0.62982670348171943,-0.61983772200117615,
-0.61000948603099481,-0.60033936489493922,-0.59082477126274668,-0.58146316046497759,-0.57225202981648426,-0.56318891794850978,-0.55427140414941978,-0.54549710771406457,-0.53686368730176159,-0.52836884030288112,-0.52001030221401556},
new double[]{
-27.498763395123252,-27.38765228410097,-27.276541173089122,-27.165430062088941,-27.054318951101795,-26.943207840129219,-26.832096729172925,-26.720985618234824,-26.609874507317052,-26.498763396422003,-26.387652285552349,-26.276541174711067,
-26.165430063901493,-26.05431895312736,-25.943207842392827,-25.832096731702546,-25.720985621061722,-25.609874510476168,-25.498763399952377,-25.387652289497606,-25.276541179119967,-25.165430068828524,-25.054318958633409,
-24.943207848545942,-24.83209673857877,-24.720985628746035,-24.609874519063531,-24.498763409548918,-24.387652300221923,-24.276541191104595,-24.165430082221576,-24.0543189736004,-23.943207865271837,-23.832096757270278,
-23.720985649634148,-23.609874542406395,-23.498763435635013,-23.387652329373626,-23.276541223682177,-23.16543011862764,-23.054319014284864,-22.9432079107375,-22.832096808079012,-22.720985706413867,-22.609874605858803,
-22.498763506544272,-22.387652408616056,-22.276541312237079,-22.1654302175894,-22.054319124876482,-21.943208034325689,-21.832096946191111,-21.720985860756695,-21.609874778339762,-21.498763699294916,-21.387652624018433,
-21.276541552953159,-21.16543048659398,-21.054319425493944,-20.94320837027108,-20.832097321616047,-20.720986280300664,-20.609875247187457,-20.498764223240286,-20.387653209536293,-20.276542207279181,-20.165431217814103,
-20.054320242644266,-19.943209283449519,-19.832098342107081,-19.720987420714756,-19.609876521616822,-19.498765647433011,-19.387654801090832,-19.276543985861753,-19.165433205401584,-19.054322463795621,-18.943211765609085,
-18.832101115943473,-18.720990520499523,-18.609879985647584,-18.498769518506208,-18.387659127029963,-18.276548820107575,-18.165438607671511,-18.054328500820453,-17.943218511956083,-17.832108654935872,-17.720998945243785,
-17.6098894001809,-17.498780039078323,-17.387670883534984,-17.276561957683153,-17.16545328848494,-17.054344906063335,-16.943236844071762,-16.832129140106609,-16.721021836167662,-16.609914979171887,-16.498808621526756,
-16.387702821769746,-16.276597645281683,-16.165493165082118,-16.054389462716085,-15.943286629242365,-15.832184766334622,-15.721083987507837,-15.609984419483823,-15.498886203710963,-15.387789498054827,-15.276694478677932,
-15.165601342128685,-15.054510307661353,-14.943421619810934,-14.832335551248827,-14.721252405947395,-14.610172522683788,-14.499096278915651,-14.38802409506374,-14.276956439238758,-14.165893832452042,-14.054836854351871,
-13.9437861495292,-13.832742434438366,-13.721706504979704,-13.61067924479203,-13.4996616343033,-13.388654760587505,-13.277659828074789,-13.166678170159637,-13.055711261748803,-12.944760732786209,-12.833828382785988,
-12.722916196397433,-12.612026360016253,-12.50116127944548,-12.390323598596286,-12.279516219203904,-12.168742321516866,-12.058005385898706,-11.947309215260528,-11.836657958220357,-11.726056132861338,-11.615508650936016,
-11.505020842338448,-11.394598479640289,-11.284247802461993,-11.173975541426243,-11.063788941418661,-10.953695783861297,-10.8437044076882,-10.733823728700132,-10.62406325696815,-10.514433111953636,-10.40494403501628,
-10.295607398991555,-10.186435214536019,-10.07744013296203,-9.9686354453133781,-9.8600350774693926,-9.7516535811069751,-9.643506120396875,-9.53560845436168,-9.4279769148772825,-9.320628380356,-9.21358024520675,
-9.1068503852245541,-9.0004571191168132,-8.8944191664260313,-8.7887556021569537,-8.6834858084591744,-8.5786294237534211,-8.4742062897202111,-8.37023639659284,-8.2667398272123265,-8.163736700310114,-8.0612471134847539,
-7.9592910863321329,-7.8578885041751469,-7.7570590628189144,-7.6568222147322462,-7.55719711702596,-7.4582025815646009,-7.359857027511147,-7.2621784365651179,-7.165184311114138,-7.068891635478181,-6.973316840385225,
-6.878475770777551,-6.7843836570100065,-6.6910550894657366,-6.5985039965815764,-6.5067436262447371,-6.4157865304949686,-6.3256445534419932,-6.2363288222868869,-6.1478497413181614,-6.0602169887384765,-5.9734395161661107,
-5.8875255506463979,-5.8024825990019906,-5.718317454346983,-5.6350362045882676,-5.552644242737804,-5.4711462788615552,-5.3905463534943792,-5.3108478523550104,-5.2320535222011237,-5.1541654876712206,-5.077185268967491,
-5.0011138002416642,-4.92595144855414,-4.8516980332851105,-4.7783528458849007,-4.7059146698592915,-4.6343818008939648,-4.5637520670304106,-4.49402284881366,-4.42519109933982,-4.3572533641387761,-4.2902058008344239,
-4.2240441985313453,-4.1587639968831391,-4.0943603048033728,-4.0308279187855822,-3.9681613408037628,-3.906354795769452,-3.8454022485257529,-3.7852974203625869,-3.7260338050410038,-3.6676046843176242,-3.6100031429632118,
-3.5532220832720016,-3.4972542390607582,-3.4420921891586471,-3.3877283703908465,-3.3341550900604671,-3.2813645379347816,-3.2293487977430027,-3.1780998581939137,-3.127609623522579,-3.0778699235761149,-3.0288725234491487,
-2.9806091326801067,-2.9330714140198864,-2.88625099178478,-2.8401394598057572,-2.7947283889863579,-2.7500093344815344,-2.70597384250982,-2.6626134568111559,-2.6199197247626524,-2.5778842031644329,-2.5364984637075776,
-2.4957540981359849,-2.4556427231137854,-2.4161559848097069,-2.3772855632095462,-2.3390231761676512,-2.3013605832080435,-2.2642895890855383,-2.227802047116926,-2.1918898622919971,-2.156544994173899,-2.121759459598012,
-2.0875253351782459,-2.0538347596293729,-2.020679935913686,-1.9880531332200497,-1.95594668878305,-1.924353009549739,-1.893264573701154,-1.862673932035541,-1.83257370921994,-1.8029566049165571,-1.773815394790067,
-1.745142931401763,-1.71693214499626,-1.68917604418617,-1.6618677165400171,-1.635000329078381,-1.6085671286831009,-1.58256144242414,-1.556976677808529,-1.531806322955616,-1.5070439467026739,-1.48268319864474,
-1.458717809112388,-1.4351415890909931,-1.4119484300848531,-1.389132303929441,-1.366687262554851,-1.344607437703415,-1.3228870406043161,-1.30152036160788,-1.2805017697821379,-1.2598257124741,-1.2394867148381,
-1.21947937933343,-1.1997983851934111,-1.1804384878679171,-1.1613945184413,-1.1426613830275521,-1.1242340621444651,-1.106107610068455,-1.0882771541716489,-1.070737894242743,-1.0534851017930671,-1.0365141193492391,
-1.019820359733695,-1.003399305334334,-0.987246507364465,-0.971357585114142,-0.955728225193977,-0.940354180772406,-0.925231270807373,-0.91035537927333,-0.895722454384403,-0.881328507814527,-0.867169613915331,
-0.853241908932461,-0.839541590221066,-0.826064915461047,-0.812808201872711,-0.799767825433375,-0.786940220095478,-0.7743218770067,-0.761909343732561,-0.749699223481951,-0.737688174336018,-0.72587290848079,
-0.71425019144391655,-0.70281684133586431,-0.69156972809588757,-0.68050577274307678,-0.66962194663275965,-0.65891527071851541,-0.6483828148200389,-0.63802169689707755,-0.62782908232964363,-0.6178021832046896,-0.60793825760941822,
-0.59823460893138392,-0.58868858516552769,-0.57929757822827543,-0.570059023278815,-0.56097039804765669,-0.55202922217256911,-0.54323305654197251,-0.53457950264585963,-0.52606620193430687,-0.5176908351836248,-0.5094511218701947},
new double[]{
-25.35274220681212,-25.252742207078949,-25.152742207373841,-25.052742207699747,-24.952742208059931,-24.852742208457993,-24.752742208897921,-24.652742209384115,-24.552742209921444,-24.452742210515282,-24.352742211171577,-24.252742211896894,
-24.152742212698492,-24.0527422135844,-23.952742214563475,-23.85274221564552,-23.752742216841366,-23.652742218162981,-23.552742219623589,-23.452742221237813,-23.352742223021806,-23.252742224993423,-23.152742227172396,
-23.052742229580534,-22.952742232241938,-22.852742235183243,-22.752742238433889,-22.652742242026406,-22.552742245996754,-22.452742250384663,-22.352742255234055,-22.252742260593461,-22.152742266516519,-22.052742273062513,
-21.952742280296953,-21.852742288292241,-21.752742297128403,-21.65274230689387,-21.552742317686377,-21.452742329613937,-21.352742342795931,-21.25274235736428,-21.152742373464793,-21.052742391258608,-20.952742410923804,
-20.8527424326572,-20.752742456676305,-20.652742483221513,-20.552742512558485,-20.452742544980836,-20.352742580813054,-20.252742620413752,-20.152742664179254,-20.052742712547573,-19.952742766002782,-19.852742825079861,
-19.752742890370058,-19.652742962526787,-19.552743042272194,-19.452743130404357,-19.352743227805295,-19.252743335449768,-19.152743454415056,-19.052743585891726,-18.952743731195536,-18.852743891780619,-18.752744069254021,
-18.652744265391775,-18.552744482156672,-18.452744721717906,-18.352744986472761,-18.252745279070592,-18.152745602439332,-18.052745959814771,-17.952746354772923,-17.852746791265776,-17.752747273660813,-17.652747806784692,
-17.552748395971481,-17.452749047115997,-17.352749766732707,-17.252750562020847,-17.152751440936342,-17.0527524122713,-16.952753485741827,-16.852754672085045,-16.752755983166317,-16.652757432097651,-16.552759033368556,
-16.452760802990586,-16.352762758656997,-16.25276491991913,-16.152767308381215,-16.052769947915539,-15.952772864900092,-15.852776088480987,-15.752779650862243,-15.652783587625772,-15.552787938084629,-15.452792745673007,
-15.35279805837669,-15.252803929208152,-15.152810416730819,-15.052817585637516,-14.952825507388576,-14.852834260915648,-14.752843933397795,-14.652854621117111,-14.552866430401791,-14.452879478665269,-14.352893895550919,
-14.252909824192578,-14.152927422602131,-14.052946865196361,-13.952968344476314,-13.852992072873523,-13.753018284778628,-13.653047238769162,-13.553079220054496,-13.453114543157364,-13.353153554852652,-13.253196637385564,
-13.153244211992684,-13.053296742750767,-12.953354740779487,-12.853418768825557,-12.753489446256866,-12.653567454496173,-12.55365354292478,-12.45374853528709,-12.353853336627227,-12.253968940788621,-12.154096438507,
-12.054237026125879,-11.954392014961972,-11.854562841345366,-11.754751077355994,-11.654958442273735,-11.555186814754281,-11.455438245736662,-11.355714972080941,-11.256019430926122,-11.156354274748507,-11.056722387089886,
-10.957126898912776,-10.857571205526764,-10.758058984015714,-10.658594211080507,-10.55918118119615,-10.459824524965818,-10.360529227537892,-10.261300646935743,-10.162144532134105,-10.063067040700956,-9.9640747558101115,
-9.86517470241782,-9.7663743623868911,-9.6676816883347634,-9.5691051159777558,-9.4706535747430678,-9.37233649642303,-9.2741638216530138,-9.1761460040054512,-9.0782940115075483,-8.9806193254096112,-8.8831339360541666,
-8.78585033572297,-8.6887815083693916,-8.5919409161767426,-8.4953424829185344,-8.3990005741337761,-8.3029299741683218,-8.2071458601714813,-8.1116637731745982,-8.0164995864145254,-7.9216694710989808,-7.8271898598421048,
-7.7330774080264293,-7.639348953371484,-7.5460214740089571,-7.4531120453793331,-7.360637796275145,-7.2686158643612719,-7.1770633515031763,-7.0859972792296793,-6.9954345446482238,-6.9053918771177338,-6.8158857959677412,
-6.7269325695327336,-6.6385481757483333,-6.5507482645313635,-6.46354812213976,-6.3769626376810526,-6.2910062719104323,-6.2056930284315985,-6.1210364273861879,-6.037049481690965,-5.9537446758564858,-5.8711339473969053,
-5.7892286708182334,-5.7080396441518477,-5.6275770779815133,-5.5478505868957431,-5.4688691832829193,-5.39064127337437,-5.3131746554303305,-5.2364765199555,-5.1605534518245246,-5.0854114341931425,-5.0110558540677328,
-4.937491509404575,-4.86472261760995,-4.792752825313304,-4.7215852192877916,-4.6512223383955567,-4.5816661864388539,-4.5129182458025756,-4.4449794917786178,-4.37785040746791,-4.3115309991614694,-4.2460208121077256,
-4.1813189465792391,-4.1174240741579142,-4.0543344541637838,-3.9920479501582555,-3.9305620464584949,-3.8698738646051796,-3.8099801797312409,-3.7508774367843802,-3.6925617665610697,-3.635029001514416,-3.5782746913026973,
-3.5222941180495084,-3.4670823112903575,-3.4126340625841536,-3.3589439397714056,-3.3060063008640359,-3.2538153075545848,-3.2023649383351995,-3.1516490012191851,-3.1016611460600885,-3.0523948764652449,-3.0038435613025,
-2.9560004458004161,-2.9088586622436967,-2.862411240266832,-2.8166511167500858,-2.7715711453229224,-2.7271641054808415,-2.6834227113223088,-2.640339619913123,-2.59790743928608,-2.5561187360842497,-2.5149660428565395,
-2.4744418650145232,-2.434538687459721,-2.3952489808906923,-2.356565207799425,-2.3184798281665442,-2.2809853048649109,-2.2440741087811471,-2.2077387236645869,-2.1719716507130631,-2.1367654129048432,-2.1021125590859007,
-2.0680056678215579,-2.03443735102138,-2.0014002573460221,-1.9688870754045442,-1.9368905367505167,-1.9054034186850268,-1.8744185468745014,-1.8439287977910279,-1.8139271009826661,-1.78440644118099,-1.7553598602529179,
-1.7267804590036393,-1.6986613988372452,-1.6709959032814503,-1.6437772593825755,-1.6169988189767564,-1.59065399984313,-1.5647362867445469,-1.539239232361165,-1.5141564581220679,-1.4894816549398831,-1.4652085838531581,
-1.441331076581108,-1.417843035995136,-1.3947384365113631,-1.372011324408273,-1.3496558180733449,-1.3276661081824721,-1.306036457815728,-1.2847612025129731,-1.263834750272592,-1.2432515814965419,-1.223006248884754,
-1.2030933772818091,-1.183507663478655,-1.16424387597206,-1.145296854684339,-1.1266615106457949,-1.108332825642218,-1.090305851829672,-1.0725757113186949,-1.055137595729956,-1.0379867657233119,-1.02111855050212,
-1.004528347294573,-0.988211620813765,-0.97216390269807,-0.956380790933398,-0.940857949258772,-0.925591106556637,-0.910576056229213,-0.895808655562167,-0.881284825076802,-0.867000547871909,-0.852951868956371,
-0.839134894573538,-0.82554579151838,-0.81218078644833,-0.7990361651887,-0.78610827203352,-0.773393509042594,-0.760888335335514,-0.74858926638335,-0.736492873298701,-0.72459578212472142,-0.71289467312375387,
-0.70138628006610959,-0.69006738951955071,-0.67893484013997218,-0.66798552196376138,-0.65721637570228308,-0.6466243920389092,-0.636206610928991,-0.6259601209031409,-0.61588205837417354,-0.60596960694802848,-0.59621999673897874,
-0.58663050368940683,-0.5771984488944113,-0.567921197931488,-0.5587961601955127,-0.54982078823923364,-0.540992577119468,-0.53230906374917986,-0.52376782625560192,-0.51536648334455182,-0.50710269367107585,-0.49897415521654626},
new double[]{
-23.542749467572627,-23.45184037735293,-23.360931287198841,-23.270022197116607,-23.179113107113061,-23.088204017195697,-22.997294927372714,-22.906385837653097,-22.815476748046681,-22.72456765856424,-22.63365856921757,-22.542749480019598,
-22.451840390984472,-22.360931302127689,-22.27002221346622,-22.17911312501866,-22.088204036805362,-21.997294948848619,-21.906385861172851,-21.815476773804797,-21.724567686773739,-21.633658600111751,-21.542749513853959,
-21.451840428038828,-21.360931342708486,-21.270022257909069,-21.179113173691103,-21.088204090109933,-20.997295007226153,-20.906385925106136,-20.815476843822569,-20.724567763455056,-20.633658684090779,-20.542749605825211,
-20.451840528762919,-20.360931453018416,-20.270022378717115,-20.179113305996363,-20.088204235006579,-19.9972951659125,-19.906386098894536,-19.81547703415027,-19.724567971896082,-19.633658912368951,-19.542749855828411,
-19.451840802558689,-19.36093175287106,-19.270022707106428,-19.179113665638138,-19.088204628875062,-18.997295597264987,-18.906386571298313,-18.815477551512103,-18.724568538494541,-18.633659532889776,-18.542750535403265,
-18.451841546807593,-18.360932567948865,-18.27002359975371,-18.179114643236939,-18.088205699509928,-17.997296769789813,-17.906387855409555,-17.815478957828965,-17.724570078646753,-17.633661219613771,-17.542752382647468,
-17.45184356984775,-17.360934783514338,-17.270026026165777,-17.179117300560243,-17.088208609718361,-16.997299956948151,-16.906391345872372,-16.815482780458471,-16.724574265051334,-16.63366580440923,-16.542757403743074,
-16.451849068759515,-16.360940805708044,-16.270032621432616,-16.179124523428175,-16.088216519902524,-15.997308619844084,-15.906400833096098,-15.815493170437849,-15.724585643673594,-15.633678265729948,-15.542771050762477,
-15.451864014272411,-15.360957173234409,-15.270050546236419,-15.179144153632784,-15.088238017711829,-14.997332162879296,-14.906426615859109,-14.815521405913103,-14.724616565081485,-14.633712128445962,-14.542808134417676,
-14.451904625052205,-14.361001646394207,-14.270099248854423,-14.179197487622018,-14.088296423115567,-13.997396121476193,-13.906496655106766,-13.815598103261321,-13.724700552689329,-13.633804098339734,-13.542908844130183,
-13.452014903787294,-13.361122401764295,-13.270231474242904,-13.179342270226867,-13.088454952735185,-12.997569700103659,-12.906686707404075,-12.815806187991051,-12.724928375187293,-12.634053524118778,-12.543181913712187,
-12.452313848867721,-12.361449662821283,-12.27058971971085,-12.179734417362724,-12.088884190314174,-11.998039513089829,-11.907200903749956,-11.816368927729457,-11.725544201987088,-11.634727399484953,-11.543919254018691,
-11.453120565419042,-11.362332205145489,-11.27155512229249,-11.180790350028287,-11.090039012485446,-10.999302332121115,-10.908581637563255,-10.817878371957063,-10.727194101823038,-10.636530526434948,-10.545889487722018,
-10.455272980695114,-10.364683164391355,-10.274122373325637,-10.183593129430671,-10.093098154459689,-10.002640382817663,-9.9122229747779365,-9.821849330031565,-9.7315231015065589,-9.64124820938361,-9.55102885522405,
-9.4608695361147888,-9.3707750587240124,-9.2807505531508152,-9.1908014864417336,-9.1009336756378243,-9.0111533002075319,-8.92146691371361,-8.83188145455682,-8.74240425563561,-8.6530430527593225,-8.5638059916533535,
-8.4747016333978014,-8.385738958147126,-8.296927366986754,-8.208276681793965,-8.1197971429842628,-8.0314994050410036,-7.9433945297450155,-7.855493977042026,-7.7678095935087192,-7.6803535984027027,-7.5931385673072791,
-7.5061774134081443,-7.4194833664655651,-7.333069949571704,-7.246950953808124,-7.1611404109425321,-7.07565256432625,-6.9905018381740769,-6.9057028054260554,-6.8212701544055969,-6.7372186545004435,-6.6535631211017456,
-6.5703183800420994,-6.4874992317756481,-6.405120415542414,-6.3231965737549345,-6.2417422168383094,-6.16077168874505,-6.0802991333540124,-6.0003384619484512,-5.9209033219522338,-5.8420070670857767,-5.7636627290847571,
-5.6858829911053839,-5.6086801629203409,-5.5320661579898038,-5.4560524724724111,-5.380650166222039,-5.3058698457979405,-5.2317216494984313,-5.1582152344120047,-5.0853597654647151,-5.0131639064289129,-4.9416358128460667,
-4.8707831268054722,-4.8006129735111713,-4.7311319595613011,-4.6623461728574309,-4.59426118405607,-4.52688204947044,-4.460213315327695,-4.394259023284973,-4.3290227171068407,-4.2645074504068043,-4.2007157953564844,
-4.1376498522676544,-4.0753112599546268,-4.0137012067872,-3.9528204423476359,-3.8926692896086754,-3.8332476575534793,-3.7745550541624322,-3.7165905996959681,-3.6593530402068861,-3.602840761219968,-3.5470518015210573,
-3.4919838670020584,-3.4376343445125497,-3.3840003156728113,-3.3310785706070818,-3.2788656215597016,-3.2273577163604896,-3.1765508517092309,-3.1264407862524775,-3.0770230534290457,-3.0282929740635454,-2.9802456686900691,
-2.9328760695907792,-2.8861789325365264,-2.8401488482189023,-2.7947802533651727,-2.7500674415294495,-2.7060045735552061,-2.6625856877058216,-2.6198047094612846,-2.5776554609805049,-2.536131670229846,-2.495226979779555,
-2.454934955270716,-2.4152490935561715,-2.3761628305196156,-2.337669548577693,-2.2997625838705114,-2.2624352331464497,-2.2256807603475637,-2.1894924029022307,-2.1538633777319682,-2.1187868869795881,-2.0842561234660293,
-2.0502642758833538,-2.0168045337314804,-1.9838700920062979,-1.9514541556468148,-1.919549943749016,-1.8881506935540517,-1.8572496642183445,-1.8268401403731229,-1.7969154354808026,-1.7674688949955284,-1.7384938993350754,
-1.7099838666711769,-1.6819322555452054,-1.6543325673159874,-1.6271783484463789,-1.6004631926350683,-1.5741807427999097,-1.5483246929189247,-1.5228887897349397,-1.4978668343296568,-1.4732526835727886,-1.4490402514517105,
-1.4252235102869189,-1.4017964918384169,-1.3787532883079749,-1.356088053242057,-1.3337950023400329,-1.311868414172146,-1.2903026308115431,-1.2690920583845089,-1.2482311675429341,-1.22771449386284,-1.2075366381727,
-1.187692266815114,-1.1681761118452769,-1.14898297116955,-1.130107708627293,-1.111545254019026,-1.0932906030838381,-1.075338817428849,-1.0576850244134359,-1.0403244169907859,-1.02325225350928,-1.0064638574760609,
-0.989954617285076,-0.973719985911763,-0.957755480576474,-0.942056682378627,-0.926619235903492,-0.911438848803449,-0.896511291355446,-0.881832395996343,-0.867398056837714,-0.853204229161649,-0.839246928898991,
-0.825522232091394,-0.812026274338541,-0.798755250231749,-0.785705412775189,-0.772873072795849,-0.760254598343333,-0.747846414080524,-0.735645000666104,-0.723646894129866,-0.711848685241697,-0.700247018875092,
-0.688838593365989,-0.677620159867692,-0.666588521702598,-0.655740533711416,-0.645073101600524,-0.634583181288074,-0.624267778249421,-0.614123946862434,-0.604148789753194,-0.594339457142568,-0.584693146194125,
-0.575207100363817,-0.565878608751833,-0.556705005457017,-0.547683668934194,-0.538812021354752,-0.530087527970782,-0.521507696483087,-0.51307007641331,-0.504772258480465,-0.496611873982077,-0.488586594180183},
new double[]{
-21.993739267585298,-21.910405935829129,-21.827072604210024,-21.743739272739891,-21.66040594143168,-21.57707261029946,-21.493739279358522,-21.410405948625495,-21.327072618118446,-21.243739287857014,-21.16040595786254,-21.077072628158227,
-20.993739298769288,-20.910405969723136,-20.827072641049554,-20.743739312780924,-20.66040598495244,-20.577072657602347,-20.493739330772222,-20.410406004507252,-20.327072678856549,-20.243739353873497,-20.160406029616119,
-20.077072706147476,-19.993739383536116,-19.910406061856534,-19.82707274118971,-19.743739421623655,-19.660406103254033,-19.577072786184811,-19.493739470529004,-19.410406156409444,-19.327072843959634,-19.243739533324682,
-19.160406224662303,-19.077072918143923,-18.993739613955864,-18.910406312300637,-18.827073013398351,-18.743739717488253,-18.660406424830366,-18.577073135707323,-18.493739850426309,-18.410406569321211,-18.327073292754925,
-18.243740021121887,-18.160406754850808,-18.07707349440766,-17.9937402402989,-17.910406993075007,-17.827073753334282,-17.743740521727034,-17.66040729896007,-17.57707408580163,-17.493740883086705,-17.410407691722849,
-17.327074512696477,-17.243741347079727,-17.160408196037896,-17.077075060837547,-16.993741942855294,-16.91040884358738,-16.827075764660055,-16.74374270784088,-16.66040967505101,-16.577076668378528,-16.493743690092941,
-16.410410742660964,-16.327077828763628,-16.243744951314916,-16.160412113481986,-16.077079318707167,-15.993746570731862,-15.910413873622519,-15.82708123179887,-15.743748650064603,-15.660416133640714,-15.577083688201734,
-15.493751319915123,-15.410419035484058,-15.327086842193943,-15.243754747962957,-15.160422761396978,-15.077090891849265,-14.993759149485324,-14.910427545353388,-14.827096091460996,-14.743764800858219,-14.660433687728082,
-14.57710276748481,-14.493772056880582,-14.410441574121515,-14.327111338993667,-14.243781372999942,-14.160451699508814,-14.077122343915876,-13.993793333819353,-13.910464699210721,-13.827136472681763,-13.743808689649457,
-13.660481388600191,-13.577154611354985,-13.49382840335748,-13.410502813986627,-13.327177896896178,-13.24385371038321,-13.160530317788169,-13.077207787929019,-12.993886195572422,-12.910565621944951,-12.827246155287739,
-12.743927891458105,-12.660610934582044,-12.577295397761745,-12.493981403842634,-12.410669086244754,-12.327358589863669,-12.24405007204647,-12.160743703648823,-12.077439670179482,-11.994138173039071,-11.910839430860435,
-11.827543680958362,-11.744251180896919,-11.660962210183232,-11.577677072096996,-11.494396095665621,-11.411119637795361,-11.327848085569411,-11.244581858724427,-11.161321412317456,-11.078067239595763,-10.994819875082495,
-10.911579897891524,-10.828347935285146,-10.745124666488604,-10.661910826775543,-10.578707211838564,-10.495514682458948,-10.412334169489368,-10.329166679162938,-10.246013298741319,-10.1628752025136,-10.079753658156566,
-9.9966500334653734,-9.9135658034618235,-9.8305025578851577,-9.7474620090677231,-9.6644460001946637,-9.5814565139434027,-9.498495681494532,-9.4155657919013649,-9.33266930180032,-9.2498088454389631,-9.1669872449924519,
-9.084207521132976,-9.0014729038098213,-8.9187868431907873,-8.8361530207082168,-8.7535753601454758,-8.67105803869211,-8.5886054978883948,-8.5062224543727361,-8.4239139103383582,-8.3416851635993545,-8.2595418171603079,
-8.1774897881789244,-8.09553531620713,-8.0136849705934861,-7.9319456569284466,-7.850324622414214,-7.7688294600427357,-7.6874681114690224,-7.6062488684723046,-7.5251803729047753,-7.4442716150367314,-7.3635319302177766,
-7.282970993786285,-7.202598814173518,-7.12242572416427,-7.0424623702927391,-6.9627197003699912,-6.8832089491578108,-6.8039416222225206,-6.724929478021239,-6.6461845082916726,-6.56771891683462,-6.4895450967955695,
-6.4116756065677967,-6.33412314445397,-6.2569005222361458,-6.1800206378150868,-6.1034964470887392,-6.0273409352465119,-5.95156708766051,-5.87618786055711,-5.8012161516522482,-5.72666477093157,-5.6525464117522644,
-5.5788736224371371,-5.5056587785234017,-5.4329140558190137,-5.3606514044083342,-5.2888825237367048,-5.217618838890437,-5.1468714781749139,-5.0766512520792793,-5.0069686337017609,-4.9378337406951873,-4.86925631877803,
-4.8012457268423843,-4.733810923676943,-4.6669604563103,-4.6007024499679954,-4.5350445996256337,-4.4699941631302682,-4.405557955853098,-4.3417423468283411,-4.2785532563260587,-4.2159961548005436,-4.1540760631507734,
-4.0927975542252488,-4.0321647555002924,-3.9721813528584535,-3.9128505953921322,-3.8541753011566269,-3.7961578637967022,-3.7388002599711805,-3.68210405750108,-3.6260704241682782,-3.5707001370935747,-3.5159935926252741,
-3.4619508166719308,-3.4085714754156857,-3.3558548863455808,-3.3038000295533503,-3.2524055592373688,-3.2016698153637262,-3.1515908354366573,-3.1021663663338472,-3.0533938761653774,-3.0052705661182633,-2.9577933822516509,
-2.9109590272107657,-2.8647639718306159,-2.8192044666032694,-2.7742765529851874,-2.7299760745236541,-2.6862986877837662,-2.6432398730597098,-2.6007949448562071,-2.5589590621280323,-2.5177272382673457,-2.4770943508303715,
-2.4370551509965312,-2.3976042727546583,-2.3587362418122773,-2.3204454842252002,-2.2827263347458331,-2.24557304488964,-2.2089797907201456,-2.1729406803537317,-2.1374497611862235,-2.1025010268439712,-2.0680884238627226,
-2.0342058580981313,-2.0008472008722027,-1.9680062948604062,-1.9356769597245125,-1.9038529974965355,-1.8725281977193939,-1.8416963423501247,-1.8113512104316394,-1.7814865825391542,-1.7520962450075128,-1.7231739939456898,
-1.6947136390448057,-1.666709007185992,-1.639153945854446,-1.6120423263659784,-1.5853680469123197,-1.5591250354313877,-1.533307252308646,-1.507908692915596,-1.4829233899913525,-1.4583454158731442,-1.4341688845814706,
-1.4103879537655306,-1.3869968265144042,-1.3639897530393559,-1.3413610322324825,-1.319105013106801,-1.297216096122737,-1.2756887344058325,-1.2545174348603561,-1.2336967591833641,-1.213221324783613,-1.193085805609607,
-1.1732849328909007,-1.1538134957966655,-1.134666342015392,-1.1158383782594572,-1.097324570698172,-1.0791199453227911,-1.0612195882468429,-1.043618645945021,-1.0263123254337569,-1.00929589439648,-0.992564681256453,
-0.976114075199969,-0.959939526152594,-0.944036544711004,-0.928400702032916,-0.913027629687466,-0.897913019468317,-0.883052623171689,-0.868442252341401,-0.854077777982933,-0.839955130248441,-0.826070298094565,
-0.812419328914802,-0.798998328148124,-0.785803458865475,-0.772830941335683,-0.760077052572266,-0.747538125862554,-0.735210550280472,-0.723090770184274,-0.711175284700453,-0.699460647195017,-0.687943464733229,
-0.676620397528896,-0.665488158384209,-0.654543512121115,-0.64378327500513,-0.633204314162472,-0.622803546991355,-0.612577940568223,-0.60252451104968,-0.592640323070833,-0.582922489140716,-0.573368169035444,
-0.56397456918969391,-0.55473894208709984,-0.54565858565009207,-0.5367308426297015,-0.52795309999581164,-0.5193227883283148,-0.51083738120960331,-0.50249439461880108,-0.49429138632811737,-0.48622595530167967,-0.47829574109718281},
new double[]{
-20.651740951350288,-20.574817877693576,-20.497894804298042,-20.420971731184565,-20.3440486583757,-20.267125585895805,-20.190202513771183,-20.113279442030237,-20.03635637070365,-19.959433299824553,-19.882510229428725,-19.805587159554808,
-19.728664090244536,-19.651741021542978,-19.5748179534988,-19.49789488616457,-19.420971819597057,-19.34404875385756,-19.267125689012289,-19.190202625132748,-19.113279562296153,-19.036356500585896,-18.959433440092042,
-18.882510380911846,-18.805587323150352,-18.728664266920998,-18.651741212346284,-18.574818159558522,-18.49789510870059,-18.420972059926793,-18.344049013403779,-18.267125969311515,-18.190202927844361,-18.113279889212212,
-18.036356853641756,-17.959433821377797,-17.88251079268472,-17.805587767848046,-17.728664747176119,-17.651741731001952,-17.574818719685172,-17.497895713614167,-17.420972713208375,-17.344049718920779,-17.267126731240566,
-17.190203750696053,-17.113280777857788,-17.03635781334194,-16.95943485781395,-16.882511911992449,-16.805588976653546,-16.728666052635386,-16.651743140843134,-16.574820242254329,-16.497897357924664,-16.420974488994236,
-16.344051636694296,-16.267128802354531,-16.190205987410948,-16.113283193414357,-16.036360422039547,-15.959437675095218,-15.882514954534663,-15.805592262467334,-15.728669601171328,-15.651746973106864,-15.57482438093084,
-15.497901827512553,-15.420979315950675,-15.344056849591583,-15.267134432049161,-15.190212067226176,-15.113289759337356,-15.036367512934326,-14.959445332932532,-14.882523224640297,-14.805601193790221,-14.728679246573071,
-14.651757389674387,-14.574835630314,-14.497913976288709,-14.420992436018358,-14.344071018595596,-14.267149733839599,-14.190228592354075,-14.113307605589899,-14.036386785912734,-13.959466146676041,-13.882545702299908,
-13.805625468356146,-13.728705461660171,-13.651785700370198,-13.574866204094304,-13.497946994006034,-13.421028092969163,-13.344109525672405,-13.267191318774774,-13.190273501062523,-13.113356103618484,-13.036439160004868,
-12.959522706460517,-12.882606782113761,-12.80569142921213,-12.728776693370174,-12.651862623836873,-12.574949273784107,-12.498036700617849,-12.421124966313826,-12.344214137779547,-12.267304287244725,-12.190395492682267,
-12.113487838262177,-12.036581414840885,-11.959676320488674,-11.882772661058112,-11.805870550796559,-11.728970113006037,-11.652071480754039,-11.575174797638985,-11.498280218614404,-11.421387910876108,-11.344498054816919,
-11.26761084505384,-11.190726491532809,-11.113845220716542,-11.036967276861249,-10.960092923388396,-10.883222444357971,-10.806356146050115,-10.729494358662306,-10.652637438129617,-10.575785768075969,-10.498939761904598,
-10.422099865036305,-10.345266557304374,-10.2684403555153,-10.191621816184778,-10.114811538458545,-10.038010167227894,-9.96121839644975,-9.88443697268119,-9.8076666988382879,-9.7309084381889672,-9.6541631185892243,
-9.5774317369717341,-9.5007153640952211,-9.4240151495622388,-9.3473323271120865,-9.2706682201943984,-9.1940242478276026,-9.1174019307447569,-9.04080289782742,-8.96422889282597,-8.887681781362323,-8.8111635582081878,
-8.7346763548288546,-8.6582224471790532,-8.5818042637336891,-8.505424393732099,-8.4290855956101538,-8.352790805589807,-8.2765431463908641,-8.2003459360245579,-8.1242026966233585,-8.0481171632560446,-7.9720932926718042,
-7.8961352719117883,-7.8202475267215386,-7.7444347296928235,-7.6687018080589864,-7.5930539510639745,-7.5174966168218234,-7.4420355385807779,-7.3666767303043894,-7.2914264914811042,-7.2162914110740033,-7.1412783705236818,
-7.066394545719735,-6.9916474078600794,-6.9170447231223431,-6.84259455107789,-6.7683052417865959,-6.6941854315193,-6.62024403706479,-6.546490248589155,-6.4729335210272438,-6.3995835639986343,-6.3264503302537491,
-6.2535440026694396,-6.1808749798271645,-6.1084538602207639,-6.0362914251543618,-5.9643986204041,-5.8927865367298384,-5.8214663893345486,-5.7504494963796562,-5.6797472566739007,-5.609371126661209,-5.5393325968395946,
-5.469643167747952,-5.4003143256609825,-5.3313575181341211,-5.2627841295403943,-5.1946054567396063,-5.1268326850172024,-5.0594768644256387,-4.9925488866553343,-4.9260594625552327,-4.8600191004150206,-4.7944380851120831,
-4.7293264582166605,-4.6646939991384544,-4.6005502073873252,-4.5369042860098929,-4.4737651262529328,-4.4111412934936078,-4.3490410144658957,-4.287472165802261,-4.2264422638996511,-4.1659584561095153,-4.10602751324269,
-4.04665582337182,-3.9878493869064755,-3.9296138129093818,-3.8719543166160948,-3.8148757181152169,-3.7583824421416665,-3.7024785189317124,-3.6471675860853474,-3.59245289137915,-3.5383372964709552,-3.4848232814364617,
-3.4319129500772227,-3.3796080359393397,-3.3279099089824697,-3.2768195828394862,-3.2263377226082208,-3.1764646531181184,-3.1272003676163167,-3.07854453681958,-3.0304965182806169,-2.9830553660195731,-2.9362198403738584,
-2.8899884180219466,-2.8443593021392828,-2.7993304326470096,-2.754899496516757,-2.7110639380972943,-2.6678209694313595,-2.6251675805334238,-2.5831005496015784,-2.5416164531390306,-2.5007116759629677,-2.460382421080693,
-2.4206247194150077,-2.3814344393627755,-2.3428072961724808,-2.3047388611283406,-2.2672245705302045,-2.23025973446003,-2.1938395453271662,-2.1579590861860578,-2.1226133388212216,-2.0877971915955231,-2.0535054470588596,
-2.0197328293153309,-1.986473991147901,-1.9537235209003703,-1.9214759491172209,-1.8897257549425903,-1.8584673722802323,-1.8276951957168626,-1.7974035862117934,-1.7675868765561735,-1.7382393766055417,-1.7093553782897308,
-1.6809291604044419,-1.65295499318906,-1.6254271426954841,-1.5983398749529183,-1.5716874599337103,-1.5454641753254315,-1.5196643101144776,-1.4942821679865281,-1.4693120705492324,-1.4447483603825138,-1.4205854039218777,
-1.3968175941800871,-1.3734393533125422,-1.3504451350316509,-1.3278294268754187,-1.3055867523354214,-1.2837116728492437,-1.2621987896623885,-1.2410427455645627,-1.2202382265051568,-1.199779963092626,-1.179662731982388,
-1.159881357157726,-1.1404307111080938,-1.1213057159090945,-1.1025013442082927,-1.0840126201209122,-1.0658346200393416,-1.0479624733602744,-1.0303913631331745,-1.0131165266336615,-0.99613325586528445,-0.97943689799304923,
-0.963022855711946,-0.946886587553624,-0.931023608134244,-0.915429488346437,-0.900099855498202,-0.885030393401458,-0.870216842412882,-0.85565499942956,-0.841340717841888,-0.827269907446061,-0.813438534318403,
-0.799842620653713,-0.786478244569692,-0.773341539879463,-0.760428695834091,-0.747735956836956,-0.735259622131728,-0.722996045465651,-0.710941634729745,-0.699092851577493,-0.687446211023479,-0.675998281023419,
-0.664745682036938,-0.653685086574387,-0.642813218728955,-0.632126853695252,-0.621622817275507,-0.611297985374455,-0.601149283483939,-0.591173686158231,-0.581368216480984,-0.571729945524722,-0.562255991803716,
-0.552943520721045,-0.543789744010619,-0.534791919174892,-0.525947348918948,-0.517253380581628,-0.50870740556432,-0.500306858757993,-0.492049217969043,-0.483932003344477,-0.475952776796929,-0.468109141429994},
new double[]{
-19.476891934593198,-19.40546336939391,-19.334034804655847,-19.262606240413159,-19.191177676702523,-19.119749113563334,-19.0483205510379,-18.976891989171669,-18.905463428013444,-18.834034867615653,-18.762606308034595,-18.691177749330741,
-18.619749191569042,-18.548320634819259,-18.476892079156311,-18.405463524660671,-18.334034971418767,-18.262606419523429,-18.191177869074355,-18.119749320178631,-18.048320772951264,-17.976892227515787,-17.905463684004872,
-17.834035142561014,-17.762606603337261,-17.69117806649799,-17.61974953221975,-17.548321000692166,-17.476892472118895,-17.40546394671868,-17.334035424726451,-17.262606906394545,-17.191178391993969,-17.119749881815807,
-17.04832137617268,-16.976892875400374,-16.90546437985952,-16.834035889937461,-16.762607406050211,-16.691178928644586,-16.619750458200492,-16.548321995233358,-16.476893540296778,-16.405465093985327,-16.334036656937617,
-16.262608229839522,-16.191179813427709,-16.119751408493382,-16.048323015886321,-15.976894636519226,-15.905466271372365,-15.834037921498583,-15.762609588028672,-15.691181272177143,-15.619752975248424,-15.54832469864351,
-15.476896443867119,-15.405468212535372,-15.334040006384024,-15.262611827277341,-15.191183677217598,-15.119755558355298,-15.048327473000139,-14.976899423632808,-14.905471412917629,-14.834043443716155,-14.762615519101759,
-14.691187642375327,-14.619759817082075,-14.548332047029644,-14.476904336307515,-14.405476689307857,-14.334049110747937,-14.262621605694164,-14.191194179587921,-14.119766838273311,-14.048339588026938,-13.976912435589915,
-13.905485388202211,-13.834058453639544,-13.762631640253019,-13.691204957011655,-13.619778413548087,-13.548352020207629,-13.476925788100957,-13.405499729160683,-13.334073856202103,-13.262648182988428,-13.191222724300815,
-13.119797496013582,-13.048372515174942,-12.976947800093704,-12.905523370432347,-12.834099247306952,-12.762675453394486,-12.691252013047981,-12.619828952420168,-12.548406299596209,-12.476984084736177,-12.405562340227974,
-12.334141100851488,-12.262720403954768,-12.191300289643104,-12.119880800981942,-12.04846198421464,-11.977043888996123,-11.905626568643593,-11.834210080405512,-11.762794485750177,-11.691379850675258,-11.619966246039844,
-11.548553747920543,-11.477142437993381,-11.405732403943295,-11.334323739903182,-11.26291654692456,-11.191510933482062,-11.120107016014096,-11.048704919502194,-10.977304778091694,-10.905906735756593,-10.83451094701157,
-10.763117577674359,-10.691726805681876,-10.620338821963633,-10.548953831376261,-10.477572053703113,-10.406193724723181,-10.33481909735375,-10.263448442871468,-10.192082052216744,-10.120720237386605,-10.049363332921388,
-9.97801169749088,-9.90666571558576,-9.8353257993203833,-9.7639923903531969,-9.6926659619312723,-9.6213470210655867,-9.5500361108438838,-9.47873381288803,-9.407440749962932,-9.3361575887440456,-9.2648850427505831,
-9.1936238754514434,-9.1223749035507176,-9.0511390004594929,-8.9799170999603284,-8.9087102000703915,-8.83751936710877,-8.7663457399728113,-8.6951905346275868,-8.6240550488116838,-8.55294066696142,-8.4818488653543476,
-8.4107812174714738,-8.3397393995759863,-8.2687251965044446,-8.1977405076643119,-8.1267873532294939,-8.055867880522964,-7.9849843705729544,-7.9141392448261554,-7.843335071998303,-7.7725745750391848,-7.70186063818559,
-7.6311963140721168,-7.5605848308659667,-7.4900295993880492,-7.4195342201788739,-7.3491024904638662,-7.2787384109690079,-7.20844619253411,-7.1382302624676655,-7.0680952705841111,-6.9980460948616647,-6.9280878466565881,
-6.8582258754080385,-6.7884657727664921,-6.7188133760782964,-6.6492747711591864,-6.5798562942906775,-6.5105645333751978,-6.4414063281886591,-6.37238876967289,-6.3035191982150165,-6.2348052008664654,-6.166254607460675,
-6.0978754855959245,-6.029676134457711,-5.96166507746387,-5.893851053724954,-5.8262430083221943,-5.7588500814155017,-5.691681596204349,-5.6247470457747557,-5.5580560788759419,-5.4916184846802656,-5.42544417658975,
-5.3595431751616136,-5.2939255902336742,-5.2286016023380864,-5.163581443498634,-5.09887537751242,-5.0344936798214359,-4.9704466170828958,-4.9067444265494773,-4.8433972953716857,-4.7804153399343621,-4.7178085853380942,
-4.65558694513383,-4.5937602014155052,-4.5323379853710328,-4.4713297583866636,-4.4107447937935582,-4.3505921593386239,-4.2908807004542924,-4.23161902439411,-4.17281548529285,-4.114478170201572,-4.0566148861395543,
-3.9992331481966668,-3.9423401687113913,-3.8859428475416276,-3.8300477634375727,-3.7746611665185026,-3.7197889718482235,-3.6654367540973656,-3.6116097432745908,-3.55831282150324,-3.5055505208149129,-3.4533270219270324,
-3.4016461539675515,-3.3505113951066225,-3.2999258740522621,-3.2498923723647777,-3.2004133275429547,-3.1514908368337333,-3.1031266617162454,-3.0553222330106782,-3.0080786565623763,-2.9613967194519057,-2.9152768966824185,
-2.8697193582965488,-2.824723976876212,-2.780290335380025,-2.7364177352745869,-2.6931052049175479,-2.6503515081521707,-2.6081551530749927,-2.5665144009401542,-2.5254272751659563,-2.4848915704112557,-2.4449048616913438,
-2.4054645135049912,-2.3665676889463581,-2.328211358777454,-2.2903923104387602,-2.2531071569775274,-2.216352345875066,-2.1801241677561305,-2.1444187649651552,-2.1092321399957403,-2.0745601637612974,-2.0403985836962324,
-2.0067430316784152,-1.9735890317649758,-1.9409320077346897,-1.9087672904313413,-1.877090124903527,-1.8458956773373296,-1.8151790417792197,-1.7849352466473738,-1.755159261030373,-1.7258460007729612,-1.6969903343491803,
-1.6685870885237948,-1.6406310538034468,-1.6131169896794653,-1.5860396296646837,-1.5593936861269973,-1.5331738549227425,-1.5073748198332702,-1.4819912568083502,-1.4570178380202679,-1.4324492357326681,-1.4082801259883646,
-1.3845051921204632,-1.3611191280912593,-1.338116641663456,-1.315492457408308,-1.2932413195553436,-1.271357994688342,-1.2498372742922503,-1.2286739771557229,-1.2078629516339403,-1.1873990777763455,-1.1672772693238811,
-1.1474924755802713,-1.1280396831618238,-1.1089139176301726,-1.0901102450122948,-1.0716237732120733,-1.0534496533175783,-1.0355830808081656,-1.0180192966653929,-1.0007535883916665,-0.98378129094043143,-0.96709778756162545,
-0.9506985105660184,-0.934578942011956,-0.91873461431793513,-0.90316111080433148,-0.887854066167506,-0.872809166889419,-0.85802215158577144,-0.84348881129562225,-0.82920498971530154,-0.81516658337937065,-0.80136954179127839,
-0.78780986750626825,-0.77448361616900729,-0.761386896508317,-0.748515870291299,-0.735866752239069,-0.723435809906225,-0.71121936352609483,-0.69921378582374,-0.6874155017986,-0.675820988478608,-0.664426774647509,
-0.65322944054707,-0.642225617555792,-0.631411987845651,-0.620785284018366,-0.610342288722598,-0.600079834253437,-0.589994802135482,-0.580084122690747,-0.57034477459259,-0.560773784406783,-0.551368226120828,
-0.542125220662536,-0.533041935408861,-0.524115583685932,-0.515343424261182,-0.506722760828418,-0.498250941486654,-0.489925358213479,-0.481743446333697,-0.473702683983928,-0.465800591573844,-0.458034731244673},
new double[]{
-18.439076042430976,-18.372409386850958,-18.305742732035242,-18.239076078036522,-18.172409424911116,-18.105742772719228,-18.039076121525223,-17.972409471397885,-17.905742822410751,-17.839076174642429,-17.772409528176937,-17.705742883104094,
-17.639076239519909,-17.572409597527002,-17.505742957235082,-17.439076318761408,-17.372409682231332,-17.305743047778837,-17.239076415547153,-17.172409785689375,-17.105743158369165,-17.039076533761452,-16.972409912053237,
-16.905743293444406,-16.839076678148626,-16.77241006639429,-16.705743458425541,-16.639076854503358,-16.572410254906696,-16.505743659933753,-16.439077069903281,-16.372410485156003,-16.305743906056136,-16.239077332993,
-16.17241076638275,-16.105744206670241,-16.039077654330985,-15.972411109873281,-15.905744573840462,-15.83907804681332,-15.772411529412683,-15.705745022302176,-15.639078526191176,-15.572412041837952,-15.505745570053053,
-15.439079111702895,-15.372412667713629,-15.305746239075242,-15.239079826845968,-15.172413432156985,-15.105747056217451,-15.039080700319872,-14.972414365845847,-14.905748054272214,-14.839081767177612,-14.772415506249491,
-14.705749273291623,-14.639083070232106,-14.572416899131943,-14.5057507621942,-14.439084661773787,-14.372418600387933,-14.30575258072737,-14.239086605668282,-14.172420678285095,-14.105754801864123,-14.039088979918184,
-13.972423216202197,-13.905757514729864,-13.839091879791496,-13.772426315973084,-13.705760828176643,-13.639095421642011,-13.572430101970095,-13.505764875147758,-13.439099747574394,-13.372434726090338,-13.305769818007239,
-13.239105031140511,-13.172440373844021,-13.105775855047158,-13.039111484294466,-12.972447271787976,-12.905783228432469,-12.839119365883841,-12.772455696600792,-12.705792233900063,-12.639128992015465,-12.572465986160969,
-12.505803232598113,-12.439140748708049,-12.372478553068504,-12.305816665536057,-12.239155107334016,-12.172493901146343,-12.105833071217989,-12.039172643462115,-11.972512645574634,-11.90585310715659,-11.8391940598449,
-11.772535537452017,-11.705877576115139,-11.639220214455582,-11.572563493749009,-11.50590745810727,-11.439252154672591,-11.372597633824981,-11.305943949403703,-11.239291158943797,-11.172639323928591,-11.105988510059342,
-11.039338787543075,-10.972690231399852,-10.906042921790771,-10.839396944368016,-10.772752390648428,-10.706109358412146,-10.639467952127902,-10.572828283406748,-10.506190471486013,-10.439554643745433,-10.372920936257536,
-10.306289494374402,-10.239660473353162,-10.173034039022586,-10.106410368493393,-10.039789650914925,-9.9731720882810553,-9.9065578962883318,-9.8399473052494528,-9.773340561065428,-9.7067379262598443,-9.64013968107886,
-9.5735461246607212,-9.5069575762786975,-9.4403743766616035,-9.3737968893960772,-9.3072255024151,-9.24066062957727,-9.1741027123415133,-9.107552221542095,-9.04100965926881,-8.97447556085739,-8.9079504969952481,
-8.8414350759476417,-8.7749299459094683,-8.7084357974877982,-8.64195336632021,-8.57548343583393,-8.5090268401505433,-8.44258446714091,-8.3761572616345337,-8.30974622878737,-8.2433524376115272,-8.1769770246698368,
-8.110621197937613,-8.04428624083317,-7.9779735164177961,-7.9116844717649615,-7.8454206424973121,-7.7791836574888569,-7.7129752437282741,-7.6467972313377608,-7.580651558740116,-7.5145402779649162,-7.44846556008261,
-7.3824297007532209,-7.316435125874011,-7.2504843973080684,-7.184580218673176,-7.1187254411676895,-7.0529230694073872,-6.9871762672444744,-6.921488363537069,-6.8558628578346958,-6.7903034259425281,-6.7248139253244243,
-6.6593984003022539,-6.5940610870066445,-6.5288064180321248,-6.4636390267478134,-6.3985637512132909,-6.3335856376481905,-6.268709943403401,-6.2039421393816161,-6.1392879118553836,-6.0747531636317778,-6.0103440145144607,
-5.946066801016137,-5.8819280752773677,-5.8179346031512669,-5.754093361417925,-5.6904115340972732,-5.6268965078346529,-5.5635558663394482,-5.5003973838637643,-5.4374290177151945,-5.3746588998051577,-5.3120953272419964,
-5.2497467519859367,-5.1876217695909741,-5.1257291070667179,-5.06407760990101,-5.0026762282917252,-4.9415340026433405,-4.8806600483906095,-4.8200635402178662,-4.7597536957480173,-4.6997397587800984,-4.6400309821583008,
-4.580636610358563,-4.5215658618811467,-4.4628279115390193,-4.4044318727323981,-4.34638677979941,-4.28870157053158,-4.2313850689407575,-4.174445968361205,-4.117892814966936,-4.0617339917801338,-4.0059777032415669,
-3.9506319604085549,-3.8957045668401982,-3.841203105223451,-3.7871349247871984,-3.7335071295449196,-3.6803265673998737,-3.6275998201400794,-3.5753331943437745,-3.5235327132095842,-3.4722041093194007,-3.42135281833597,
-3.3709839736315019,-3.3211024018382975,-3.2717126193073947,-3.2228188294566986,-3.1744249209859103,-3.1265344669318487,-3.0791507245344936,-3.0322766358812245,-2.9859148292943152,-2.9400676214247303,-2.894737020013689,
-2.8499247272822155,-2.8056321439080554,-2.7618603735488154,-2.7186102278699713,-2.6758822320364839,-2.6336766306271007,-2.5919933939310149,-2.5508322245873369,-2.51019256452882,-2.470073602192417,-2.4304742799605266,
-2.3913933017981623,-2.3528291410527786,-2.3147800483850189,-2.2772440598002794,-2.240219004752602,-2.2037025142941036,-2.1676920292447854,-2.1321848083592672,-2.0971779364686176,-2.0626683325770876,-2.0286527578951388,
-1.9951278237917138,-1.9620899996501919,-1.9295356206139203,-1.8974608952086129,-1.8658619128302352,-1.8347346510882652,-1.8040749829954352,-1.7738786839961991,-1.7441414388272514,-1.7148588482044445,-1.6860264353314007,
-1.657639652226006,-1.629693885861808,-1.6021844641221064,-1.5751066615652398,-1.5484557050002308,-1.5222267788725554,-1.4964150304603594,-1.4710155748819442,-1.4460234999158077,-1.4214338706349396,-1.3972417338574414,
-1.3734421224158797,-1.3500300592480734,-1.3270005613122839,-1.3043486433299993,-1.282069321359715,-1.2601576162052741,-1.2386085566624836,-1.2174171826078413,-1.1965785479333111,-1.1760877233311562,-1.1559397989329108,
-1.1361298868066008,-1.1166531233163639,-1.0975046713486194,-1.07867972240895,-1.0601734985938354,-1.0419812544413603,-1.0240982786649848,-1.006519895774427,-0.98924146758765341,-0.97225839463792263,-0.955566117479767,
-0.93916011789772558,-0.92303592002157364,-0.90718909135172343,-0.89161524369838618,-0.87631003403800922,-0.86126916529041819,-0.8464883870200115,-0.83196349606426556,-0.817690337092726,-0.80366480309957056,-0.7898828358327441,
-0.7763404261625757,-0.763033614392705,-0.74995849051605268,-0.73711119441849027,-0.72448791603277352,-0.712084895445223,-0.699898422957551,-0.68792483910615565,-0.6761605346411107,-0.66460195046702264,-0.65324557754782431,
-0.642087956777517,-0.63112567881879,-0.620355383911376,-0.609773761651927,-0.599377550747143,-0.589163538741781,-0.579128561723158,-0.569269504003644,-0.559583297782629,-0.55006692278935032,-0.54071740590792,
-0.531531820785851,-0.522507287427305,-0.513640971772229,-0.504930085262531,-0.496371884396348,-0.48796367027144,-0.479702788118706,-0.471586626826732,-0.463612618458288,-0.455778237759609,-0.448081001663282},
new double[]{
-17.515093638773312,-17.452593657383904,-17.390093677194777,-17.327593698283337,-17.26509372073199,-17.202593744628455,-17.140093770066105,-17.077593797144338,-17.015093825968968,-16.952593856652619,-16.890093889315192,-16.827593924084315,
-16.765093961095847,-16.702594000494411,-16.640094042433955,-16.577594087078356,-16.515094134602069,-16.452594185190783,-16.390094239042178,-16.327594296366673,-16.265094357388261,-16.202594422345388,-16.140094491491865,
-16.077594565097886,-16.015094643451057,-15.952594726857544,-15.89009481564325,-15.827594910155103,-15.7650950107624,-15.702595117858259,-15.640095231861148,-15.577595353216527,-15.515095482398579,-15.452595619912071,
-15.390095766294325,-15.327595922117315,-15.265096087989898,-15.202596264560205,-15.140096452518158,-15.077596652598176,-15.015096865582041,-14.952597092301955,-14.890097333643784,-14.827597590550525,-14.765097864025989,
-14.702598155138722,-14.640098465026181,-14.577598794899172,-14.515099146046586,-14.452599519840431,-14.390099917741194,-14.32760034130354,-14.265100792182395,-14.202601272139395,-14.140101783049783,-14.077602326909725,
-14.015102905844106,-13.952603522114833,-13.890104178129663,-13.827604876451611,-13.765105619808962,-13.702606411105913,-13.640107253433932,-13.577608150083808,-13.515109104558519,-13.4526101205869,-13.390111202138199,
-13.327612353437578,-13.265113578982607,-13.202614883560813,-13.140116272268378,-13.077617750530019,-13.015119324120162,-12.952620999185482,-12.890122782268879,-12.827624680335008,-12.765126700797454,-12.702628851547647,
-12.640131140985645,-12.577633578052893,-12.515136172267093,-12.452638933759305,-12.390141873313452,-12.327645002408353,-12.265148333262451,-12.202651878881431,-12.140155653108888,-12.077659670680244,-12.015163947280135,
-11.952668499603483,-11.890173345420475,-11.827678503645725,-11.765183994411855,-11.702689839147807,-11.640196060662163,-11.577702683231808,-11.515209732696254,-11.452717236558028,-11.390225224089447,-11.327733726446239,
-11.265242776788414,-11.202752410408857,-11.140262664870123,-11.077773580149941,-11.015285198796009,-10.952797566090627,-10.890310730225806,-10.827824742489499,-10.765339657463667,-10.702855533234885,-10.640372431618316,
-10.57789041839583,-10.515409563569186,-10.452929941629195,-10.390451631841836,-10.327974718552387,-10.26549929150865,-10.203025446204471,-10.140553284244746,-10.07808291373324,-10.015614449684609,-9.9531480144620179,
-9.8906837382419681,-9.8282217595078745,-9.76576222557414,-9.7033052931424972,-9.6408511288925069,-9.5783999101081942,-9.51595182534289,-9.4535070751244845,-9.3910658727033525,-9.3286284448453518,-9.26619503267242,
-9.203765892553335,-9.1413412970474326,-9.078921535904044,-9.0165069171206635,-8.9540977680628924,-8.8916944366493,-8.8292972926045259,-8.766906728783967,-8.70452316257353,-8.6421470373680211,-8.5797788241318038,
-8.5174190230454272,-8.4550681652419843,-8.3927268146370135,-8.330395569855721,-8.2680750662613676,-8.20576597808857,-8.143469020685238,-8.0811849528667867,-8.01891457938608,-7.9566587535224453,-7.8944183797928664,
-7.832194416788127,-7.76998788013645,-7.7077998455966963,-7.6456314522827631,-7.5834839060202919,-7.5213584828361482,-7.4592565325804623,-7.3971794826802162,-7.3351288420224705,-7.2731062049643418,-7.2111132554657464,
-7.1491517713397226,-7.0872236286138479,-7.0253308059948338,-6.9634753894268675,-6.9016595767326345,-6.8398856823242209,-6.7781561419692675,-6.7164735175958343,-6.6548405021174464,-6.5932599242577474,-6.5317347533520813,
-6.4702681041012324,-6.4088632412504181,-6.3475235841645583,-6.2862527112687978,-6.2250543643213314,-6.1639324524837376,-6.1028910561523748,-6.04193443051291,-5.9810670087788216,-5.9202934050737372,-5.8596184169168133,
-5.7990470272700687,-5.7385844061066358,-5.6782359114593968,-5.6180070899104058,-5.5579036764828924,-5.49793159389953,-5.4380969511730441,-5.3784060414980965,-5.3188653394167869,-5.259481497233935,-5.2002613406626672,
-5.1412118636855482,-5.0823402226216388,-5.0236537293953711,-4.965159844008852,-4.9068661662252326,-4.8487804264768872,-4.7909104760184018,-4.73326427635054,-4.6758498879475354,-4.6186754583259955,-4.5617492094994549,
-4.5050794248680148,-4.4486744355975025,-4.3925426065471651,-4.3366923218089122,-4.2811319699245667,-4.2258699288504245,-4.1709145507405756,-4.1162741466219348,-4.0619569710347321,-4.0079712067122859,-3.954324949373349,
-3.9010261926990082,-3.8480828135642686,-3.7955025575919468,-3.7432930250934464,-3.6914616574574386,-3.6400157240434687,-3.5889623096331094,-3.5383083024865782,-3.488060383047753,-3.4382250133353502,-3.3888084270527465,
-3.3398166204435515,-3.2912553439146826,-3.2431300944433752,-3.1954461087793642,-3.1482083574484059,-3.1014215395584706,-3.0550900784052955,-3.0092181178696435,-2.9638095195945358,-2.9188678609269845,-2.8743964336053129,
-2.8303982431700669,-2.7868760090737696,-2.7438321654623783,-2.7012688625992136,-2.6591879689004334,-2.6175910735496815,-2.5764794896584671,-2.5358542579380154,-2.4957161508478123,-2.4560656771857907,-2.4169030870851005,
-2.3782283773825816,-2.3400412973244693,-2.3023413545754408,-2.2651278214978419,-2.2283997416688193,-2.1921559366040806,-2.1563950126581122,-2.1211153680718637,-2.0863152001401888,-2.0519925124726108,-2.0181451223223723,
-1.9847706679600718,-1.9518666160696103,-1.9194302691455516,-1.8874587728723995,-1.8559491234676677,-1.8248981749719759,-1.7943026464707315,-1.7641591292332453,-1.734464093756386,-1.705213896701087,-1.6764047877111818,
-1.6480329161051639,-1.6200943374325241,-1.5925850198873426,-1.5655008505727619,-1.5388376416108869,-1.5125911360935034,-1.486757013869819,-1.4613308971681747,-1.4363083560493828,-1.4116849136899963,-1.3874560514944228,
-1.3636172140353491,-1.3401638138224661,-1.3170912358999445,-1.2943948422735534,-1.2720699761686969,-1.2501119661210058,-1.228516129901438,-1.2072777782781263,-1.1863922186174749,-1.1658547583272227,-1.1456607081444004,
-1.12580538527127,-1.1062841163624972,-1.0870922403669214,-1.0682251112274006,-1.0496781004422957,-1.0314465994922233,-1.0135260221357605,-0.99591180657782175,-0.97859941751445578,-0.96158434805781257,-0.94486212154503746,
-0.928428293234838,-0.91227845189544265,-0.8964082212876493,-0.88081326154662143,-0.86548927046604429,-0.85043198468821113,-0.83563718080354688,-0.82110067636302575,-0.806818330806867,-0.79278604631283767,-0.778999768567409,
-0.76545548746295355,-0.75214923772408437,-0.73907709946617417,-0.72623519868900255,-0.7136197077084121,-0.70122684552877146,-0.68905287815896188,-0.67709411887453208,-0.66534692842857912,-0.65380771521384307,-0.64247293537841688,
-0.63133909289740442,-0.620402739602775,-0.60966047517359456,-0.59910894708873408,-0.58874485054408676,-0.57856492833625039,-0.56856597071456283,-0.55874481520331021,-0.54909834639585753,-0.53962349572238588,-0.53031724119285917,
-0.52117660711677483,-0.51219866380119583,-0.5033805272285,-0.4947193587152241,-0.48621236455332423,-0.47785679563511885,-0.46964994706312668,-0.46158915774596132,-0.4536718099813925,-0.4458953290276364,-0.43825718266388969},
new double[]{
-16.686768541304513,-16.627945041611021,-16.5691215437181,-16.510298047734839,-16.45147455377694,-16.392651061967118,-16.333827572435521,-16.275004085320173,-16.21618060076748,-16.1573571189327,-16.098533639980509,-16.039710164085555,
-15.980886691433067,-15.9220632222195,-15.863239756653208,-15.804416294955173,-15.745592837359761,-15.686769384115541,-15.627945935486139,-15.569122491751154,-15.510299053207127,-15.451475620168566,-15.392652192969027,
-15.333828771962287,-15.275005357523543,-15.216181950050734,-15.157358549965904,-15.098535157716672,-15.039711773777768,-14.980888398652688,-14.922065032875437,-14.863241677012368,-14.804418331664145,-14.745594997467826,
-14.686771675099068,-14.627948365274456,-14.569125068753992,-14.510301786343725,-14.451478518898531,-14.392655267325084,-14.333832032584985,-14.275008815698092,-14.216185617746046,-14.157362439876023,-14.098539283304691,
-14.039716149322429,-13.980893039297792,-13.922069954682238,-13.863246897015157,-13.8044238679292,-13.745600869155918,-13.686777902531762,-13.62795497000443,-13.56913207363961,-13.510309215628125,-13.451486398293508,
-13.392663624100049,-13.333840895661306,-13.27501821574916,-13.216195587303391,-13.157373013441855,-13.09855049747126,-13.039728042898609,-12.980905653443323,-12.922083333050104,-12.863261085902581,-12.804438916437771,
-12.745616829361419,-12.686794829664274,-12.627972922639337,-12.569151113900158,-12.510329409400244,-12.451507815453635,-12.392686338756722,-12.33386498641139,-12.275043765949562,-12.216222685359215,-12.157401753111982,
-12.098580978192409,-12.039760370128994,-11.980939939027083,-11.922119695603776,-11.863299651224914,-11.804479817944321,-11.745660208545404,-11.686840836585263,-11.628021716441467,-11.56920286336165,-11.510384293516092,
-11.451566024053474,-11.392748073159998,-11.333930460122057,-11.275113205392687,-11.216296330662015,-11.157479858931954,-11.098663814595376,-11.039848223520064,-10.981033113137688,-10.922218512538146,-10.863404452569551,
-10.804590965944229,-10.745778087351068,-10.686965853574597,-10.628154303621201,-10.56934347885289,-10.510533423129054,-10.451724182956708,-10.392915807649692,-10.334108349497363,-10.275301863943358,-10.21649640977498,
-10.157692049323845,-10.098888848678469,-10.040086877909427,-9.9812862113079,-9.9224869276382854,-9.863689110405776,-9.8048928481396853,-9.74609823469349,-9.6873053695624822,-9.6285143582200927,-9.5697253124738726,
-9.51093835084232,-9.4521535989536467,-9.3933711899677625,-9.33459126502274,-9.2758139737071232,-9.2170394745594884,-9.1582679355967489,-9.09949953487277,-9.0407344610688991,-8.9819729141181472,-8.9232151058647613,
-8.8644612607610824,-8.8057116166036042,-8.7469664253102319,-8.6882259537408775,-8.6294904845635134,-8.5707603171679789,-8.5120357686298416,-8.4533171747267382,-8.3946048910096778,-8.3358992939318366,-8.2772007820374984,
-8.2185097772137947,-8.159826726007994,-8.1011521010131169,-8.0424864023247071,-7.983830159071589,-7.9251839310234962,-7.8665483102784073,-7.8079239230324529,-7.74931143143518,-7.6907115355329418,-7.6321249753030491,
-7.5735525327812585,-7.5149950342849863,-7.4564533527344983,-7.3979284100740772,-7.3394211797949467,-7.2809326895614088,-7.22246402394131,-7.16401632724156,-7.1055908064489612,-7.0471887342760775,-6.98881145231133,
-6.9304603742718056,-6.87213698935659,-6.813842865697624,-6.75557965390421,-6.6973490906963606,-6.6391530026211578,-6.5809933098451641,-6.52287203001479,-6.46479128217521,-6.4067532907371421,-6.3487603894793692,
-6.2908150255734405,-6.2329197636154774,-6.1750772896484456,-6.11729041515668,-6.0595620810128343,-6.0018953613558175,-5.9442934673767205,-5.8867597509881433,-5.8292977083508726,-5.7719109832304349,-5.7146033701547534,
-5.6573788173429733,-5.6002414293745089,-5.5431954695665508,-5.4862453620276987,-5.429395693355,-5.372651213941622,-5.316016838862625,-5.259497648306783,-5.2030988875233737,-5.1468259662540126,-5.0906844576212746,
-5.0346800964478104,-4.9788187769820249,-4.92310655000914,-4.8675496193295533,-4.8121543375898579,-4.7569272014556789,-4.70187484611955,-4.6470040391413692,-4.5923216736235792,-4.5378347607278915,-4.48355042154527,
-4.4294758783358,-4.3756184451599784,-4.3219855179278959,-4.268584563897484,-4.2154231106576656,-4.1625087346365692,-4.1098490491790738,-4.05745169224168,-4.0053243137560548,-3.9534745627155057,-3.9019100740411083,
-3.850638455286123,-3.7996672732387879,-3.7490040404844489,-3.6986562019883236,-3.6486311217600114,-3.5989360696601134,-3.5495782084080809,-3.5005645808486721,-3.4519020975321761,-3.4035975246609524,-3.3556574724517771,
-3.308088383960162,-3.26089652440911,-3.2140879710608976,-3.1676686036663368,-3.1216440955217433,-3.0760199051594732,-3.0308012686934998,-2.9859931928371104,-2.941600448605449,-2.8976275657113586,-2.8540788276588454,
-2.8109582675344851,-2.7682696644932991,-2.7260165409320334,-2.6842021603394119,-2.6428295258098249,-2.6019013792040728,-2.5614202009381946,-2.5213882103791119,-2.4818073668238028,-2.442679371036951,-2.4040056673205505,
-2.3657874460877286,-2.3280256469120784,-2.2907209620230846,-2.2538738402177407,-2.2174844911581713,-2.181552890025023,-2.1460787824964886,-2.1110616900231358,-2.0765009153691358,-2.04239554839108,-2.008744472026279,
-1.9755463684632366,-1.9427997254679035,-1.9105028428402975,-1.8786538389771115,-1.8472506575170369,-1.8162910740466565,-1.7857727028459316,-1.7556930036534824,-1.7260492884330549,-1.6968387281237556,-1.6680583593578162,
-1.6397050911308244,-1.6117757114104954,-1.5842668936711903,-1.5571752033424771,-1.5304971041610878,-1.5042289644166522,-1.4783670630825692,-1.4529075958243214,-1.4278466808784422,-1.4031803647961925,-1.3789046280468298,
-1.3550153904761104,-1.331508516616398,-1.3083798208454243,-1.2856250723913956,-1.2632400001827233,-1.2412202975412185,-1.2195616267180991,-1.1982596232726366,-1.1773099002937015,-1.1567080524648739,-1.1364496599741452,
-1.1165302922695775,-1.0969455116625808,-1.0776908767807494,-1.0587619458724347,-1.0401542799654522,-1.0218634458825171,-1.0038850191161592,-0.98621458656602712,-0.96884774914161076,-0.9517801242335171,-0.93500734805652463,
-0.91852507786771442,-0.90232899406302913,-0.88641480215565616,-0.87077823463965942,-0.85541505274230134,-0.84032104806850272,-0.82549204414088484,-0.8109238978388259,-0.79661250073993983,-0.78255378036736289,-0.76874370134619041,
-0.75517826647237385,-0.74185351769733332,-0.72876553703149716,-0.7159104473699196,-0.70328441324307212,-0.69088364149584069,-0.67870438189769877,-0.66674292768695531,-0.65499561605191481,-0.64345882855171077,-0.632128991479507,
-0.62100257617068788,-0.61007609925858652,-0.59934612288022815,-0.58880925483449353,-0.57846214869503165,-0.56830150388018585,-0.55832406568211679,-0.548526625257244,-0.53890601958004924,-0.52945913136222345,-0.52018288893906262,
-0.51107426612495932,-0.50213028203976318,-0.49334800090772341,-0.4847245318306605,-0.47625702853695467,-0.46794268910787429,-0.459778755682711,-0.4517625141441301,-0.44389129378508568,-0.4361624669585985,-0.428573448711638},
new double[]{
-15.939647599496363,-15.884092089398148,-15.828536581896804,-15.772981077140678,-15.7174255752866,-15.661870076500355,-15.6063145809572,-15.550759088842405,-15.495203600351822,-15.439648115692494,-15.384092635083286,-15.328537158755575,
-15.272981686953946,-15.217426219936964,-15.161870757977964,-15.106315301365887,-15.050759850406193,-14.995204405421783,-14.939648966754005,-14.884093534763712,-14.828538109832365,-14.772982692363222,-14.71742728278257,
-14.661871881541053,-14.606316489115061,-14.550761106008192,-14.495205732752813,-14.439650369911709,-14.384095018079805,-14.328539677886015,-14.272984349995182,-14.217429035110124,-14.16187373397381,-14.106318447371647,
-14.050763176133913,-13.995207921138306,-13.939652683312669,-13.884097463637843,-13.828542263150698,-13.772987082947333,-13.717431924186457,-13.66187678809297,-13.606321675961738,-13.55076658916159,-13.495211529139551,
-13.439656497425291,-13.384101495635864,-13.328546525480684,-13.272991588766807,-13.217436687404508,-13.161881823413172,-13.106326998927527,-13.050772216204232,-12.995217477628835,-12.939662785723142,-12.884108143152989,
-12.828553552736473,-12.772999017452642,-12.717444540450689,-12.661890125059658,-12.606335774798728,-12.550781493388053,-12.495227284760247,-12.439673153072508,-12.38411910271944,-12.328565138346606,-12.273011264864854,
-12.217457487465458,-12.161903811636126,-12.10635024317792,-12.050796788223133,-11.995243453254204,-11.939690245123687,-11.884137171075381,-11.828584238766643,-11.773031456291994,-11.717478832208052,-11.661926375559895,
-11.606374095908928,-11.550822003362322,-11.495270108604151,-11.43971842292828,-11.384166958273143,-11.328615727258487,-11.273064743224216,-11.217514020271436,-11.161963573305838,-11.106413418083555,-11.050863571259605,
-10.995314050439108,-10.9397648742314,-10.884216062307228,-10.828667635459174,-10.773119615665534,-10.717572026157795,-10.662024891491955,-10.606478237623874,-10.5509320919889,-10.495386483585996,-10.439841443066639,
-10.384297002828733,-10.328753197115839,-10.27321006212201,-10.217667636102537,-10.162125959490957,-10.10658507502264,-10.051045027865346,-9.9955058657571421,-9.9399676391520426,-9.8844304013738746,-9.82889420877874,
-9.7733591209266226,-9.71782520076258,-9.6622925148080832,-9.6067611333630669,-9.5512311307192181,-9.49570258538519,-9.44017558032434,-9.3846502032056556,-9.32912654666865,-9.2736047086028854,-9.2180847924429834,
-9.1625669074799,-9.1070511691893525,-9.0515376995782955,-8.9960266275504,-8.9405180892915155,-8.885012228676187,-8.8295091976962787,-8.7740091569128644,-8.7185122759325715,-8.6630187339096221,-8.6075287200748374,
-8.552042434293007,-8.4965600876499785,-8.44108190307095,-8.38560811597148,-8.3301389749428036,-8.2746747424730511,-8.2192156957061044,-8.1637621272398,-8.1083143459653115,-8.0528726779495461,-7.99743746736248,
-7.9420090774513854,-7.8865878915639618,-7.8311743142224213,-7.7757687722506148,-7.7203717159563228,-7.6649836203708661,-7.6096049865481934,-7.55423634292563,-7.4988782467484656,-7.4435312855605469,-7.3881960787630083,
-7.3328732792432509,-7.27756357507621,-7.2222676912998782,-7.1669863917669572,-7.1117204810743955,-7.0564708065724222,-7.0012382604545138,-6.9460237819295312,-6.890828359477041,-6.8356530331865395,-6.7804988971810234,
-6.7253671021249781,-6.6702588578164788,-6.6151754358626462,-6.5601181724372388,-6.5050884711185972,-6.4500878058055955,-6.395117723708581,-6.3401798484116112,-6.2852758830015256,-6.2304076132585724,-6.1755769109024374,
-6.12078573688661,-6.0660361447330136,-6.0113302838977907,-5.9566704031580793,-5.9020588540084438,-5.847498094054477,-5.7929906903899013,-5.738539322942243,-5.6841467877709553,-5.629816000300595,-5.5755499984704748,
-5.5213519457809692,-5.4672251342155365,-5.4131729870164014,-5.35919906129083,-5.3053070504239965,-5.2515007862736613,-5.1977842411211626,-5.1441615293527621,-5.0906369088450081,-5.0372147820276743,-4.9838996965978914,
-4.93069634585941,-4.8776095686615077,-4.8246443489128881,-4.7718058146470108,-4.7190992366167066,-4.66653002639762,-4.6141037339819766,-4.5618260448464607,-4.5097027764805127,-4.4577398743641909,-4.4059434073877757,
-4.3543195627086035,-4.3028746400441067,-4.2516150454036961,-4.2005472842659151,-4.1496779542112119,-4.099013737024606,-4.048561390286519,-3.9983277384739524,-3.9483196635980708,-3.8985440954079649,-3.8490080011939445,
-3.7997183752270476,-3.750682227874548,-3.7019065744340209,-3.6533984237310158,-3.605164766527448,-3.5572125637895682,-3.5095487348656471,-3.4621801456244032,-3.41511359660566,-3.3683558112347187,-3.3219134241515382,
-3.2757929697049675,-3.2300008706610459,-3.1845434271727533,-3.1394268060566151,-3.094657030419242,-3.0502399696742648,-3.0061813299872511,-2.9624866451830592,-2.919161268146806,-2.876210362746157,-2.8336388962990986,
-2.7914516326077146,-2.7496531255748318,-2.7082477134167382,-2.667239513481563,-2.6266324176793612,-2.5864300885264981,-2.5466359558036169,-2.5072532138233097,-2.4682848193006204,-2.4297334898167016,-2.3916017028633587,
-2.3538916954538229,-2.3166054642829352,-2.2797447664179846,-2.243311120499734,-2.2073058084316903,-2.1717298775344118,-2.1365841431406016,-2.1018691916059224,-2.0675853837098268,-2.03373285842028,-2.0003115369959881,
-1.9673211273996865,-1.9347611289960973,-1.9026308375084204,-1.8709293502075526,-1.8396555713087286,-1.808808217550848,-1.77838582393444,-1.7483867495949699,-1.7188091837890271,-1.6896511519718171,-1.6609105219453162,
-1.6325850100574222,-1.6046721874334291,-1.5771694862221772,-1.5500742058402577,-1.5233835191986829,-1.4970944788974625,-1.4712040233745534,-1.4457089829966439,-1.4206060860802345,-1.3958919648324304,-1.3715631612018004,
-1.3476161326305707,-1.3240472577002824,-1.3008528416639025,-1.2780291218581639,-1.2555722729906915,-1.2334784122972011,-1.211743604564743,-1.1903638670176284,-1.1693351740632911,-1.1486534618959068,-1.1283146329561522,
-1.1083145602459816,-1.0886490914977678,-1.0693140531976004,-1.0503052544629323,-1.0316184907751314,-1.0132495475678469,-0.9951942036723983,-0.97744823462168318,-0.96000741581435511,-0.94286752554125031,-0.92602434787624677,
-0.909473675433926,-0.89321131199656034,-0.87723307501309811,-0.86153479797293109,-0.84611233265733921,-0.83096155127158977,-0.81607834846074034,-0.80145864321225235,-0.78709838064856386,-0.77299353371280333,-0.75914010475084392,
-0.74553412699290755,-0.73217166593792926,-0.71904882064388154,-0.706161724927243,-0.69350654847477167,-0.68107949787071109,-0.6688768175425257,-0.65689479062821521,-0.64512973976821619,-0.63357802782484751,-0.62223605853220287,
-0.61110027707933745,-0.60016717062953817,-0.58943326877840152,-0.57889514395338626,-0.56854941175743778,-0.55839273125921707,-0.54842180523240358,-0.53863338034646846,-0.52902424731125153,-0.51959124097760445,-0.51033124039629707,
-0.501241168837314,-0.492317993771602,-0.48355872681726508,-0.47496042365212993,-0.46652018389454941,-0.4582351509542395,-0.45010251185488431,-0.44211949703018188,-0.43428338009494188,-0.42659147759278421,-0.4190411487219316},
new double[]{
-15.262086438086728,-15.209454926124387,-15.156823417781984,-15.104191913255146,-15.051560412750067,-14.998928916484086,-14.946297424686287,-14.893665937598135,-14.841034455474141,-14.788402978582569,-14.735771507206186,-14.683140041643036,
-14.630508582207265,-14.577877129230002,-14.525245683060268,-14.472614244065943,-14.419982812634791,-14.367351389175523,-14.314719974118948,-14.262088567919145,-14.209457171054735,-14.156825784030206,-14.104194407377301,
-14.051563041656506,-13.998931687458587,-13.946300345406241,-13.89366901615581,-13.841037700399104,-13.788406398865318,-13.735775112323051,-13.683143841582435,-13.63051258749738,-13.577881350967937,-13.525250132942793,
-13.472618934421902,-13.419987756459252,-13.367356600165778,-13.314725466712451,-13.262094357333517,-13.209463273329913,-13.15683221607287,-13.104201187007714,-13.051570187657873,-12.998939219629087,-12.946308284613858,
-12.893677384396144,-12.841046520856294,-12.788415695976253,-12.735784911845059,-12.683154170664619,-12.63052347475581,-12.577892826564913,-12.525262228670371,-12.472631683789945,-12.420001194788227,-12.36737076468458,
-12.314740396661488,-12.262110094073375,-12.209479860455888,-12.156849699535684,-12.104219615240748,-12.051589611711263,-11.998959693311068,-11.946329864639743,-11.89370013054533,-11.841070496137752,-11.788440966802952,
-11.735811548217804,-11.683182246365805,-11.630553067553636,-11.577924018428615,-11.525295105997067,-11.47266633764372,-11.420037721152115,-11.367409264726138,-11.314780977012697,-11.262152867125632,-11.209524944670909,
-11.156897219773164,-11.104269703103693,-11.051642405909936,-10.999015340046547,-10.946388518008147,-10.893761952963837,-10.841135658793551,-10.788509650126391,-10.735883942381008,-10.68325855180815,-10.630633495535511,
-10.578008791614975,-10.525384459072408,-10.47276051796012,-10.420136989412139,-10.367513895702459,-10.314891260306403,-10.262269107965286,-10.20964746475453,-10.15702635815544,-10.104405817130816,-10.051785872204619,
-9.9991665555458784,-9.9465479010571141,-9.8939299444674518,-9.8413127234307343,-9.7886962776288513,-9.736080648880586,-9.6834658812562537,-9.6308520211984572,-9.578239117649245,-9.5256272221840455,-9.4730163891527024,
-9.420406675828,-9.3677981425620445,-9.315190852950936,-9.26258487400815,-9.2099802763470624,-9.1573771343731138,-9.1047755264860886,-9.0521755352930384,-8.99957724783239,-8.9469807558098076,-8.8943861558463926,
-8.8417935497398723,-8.7892030447393985,-8.73661475383467,-8.6840287960600691,-8.6314452968145634,-8.5788643881981645,-8.52628620936575,-8.473710906899095,-8.4211386351979964,-8.3685695568914316,-8.31600384326969,
-8.2634416747384982,-8.21088324129615,-8.15832874303477,-8.1057783906667815,-8.05323240607778,-8.0006910229070041,-7.9481544871566392,-7.8956230578312789,-7.8430970076088435,-7.79057662354434,-7.7380622078078822,
-7.6855540784584209,-7.633052570254673,-7.580558035504775,-7.528070844956229,-7.475591388727727,-7.423120077284465,-7.3706573424586015,-7.3182036385165086,-7.2657594432744963,-7.2133252592646748,-7.1609016149526523,
-7.1084890660087217,-7.0560881966341862,-7.0036996209444631,-6.9513239844105215,-6.8989619653602041,-6.8466142765408895,-6.7942816667448609,-6.7419649224986751,-6.6896648698176824,-6.637382376026717,-6.5851183516478136,
-6.5328737523556208,-6.4806495810009714,-6.4284468897028137,-6.3762667820084715,-6.32411041512185,-6.2719790021989157,-6.2198738147093753,-6.1677961848630805,-6.1157475080992425,-6.0637292456360417,-6.0117429270776981,
-5.9597901530754944,-5.9078725980386206,-5.8559920128900709,-5.8041502278621016,-5.752349155325037,-5.700590792642414,-5.64887722504464,-5.5972106285124923,-5.54559327266088,-5.4940275236123934,-5.4425158468492123,
-5.3910608100310089,-5.339665085765489,-5.288331454317297,-5.2370628062400018,-5.1858621449150073,-5.1347325889802633,-5.0836773746308568,-5.0326998577727062,-4.981803516009883,-4.9309919504454189,-4.8802688872749238,
-4.829638179151881,-4.7791038063032314,-4.7286698773736395,-4.6783406299769146,-4.6281204309331683,-4.5780137761707262,-4.5280252902723337,-4.4781597256460017,-4.4284219613018267,-4.3788170012173566,-4.3293499722754909,
-4.2800261217606268,-4.2308508144006138,-4.1818295289442355,-4.1329678542662336,-4.08427148499443,-4.0357462166561859,-3.987397940344291,-3.9392326369053734,-3.8912563706570116,-3.843475282642896,-3.7958955834386181,
-3.7485235455239003,-3.7013654952402759,-3.6544278043564051,-3.6077168812662519,-3.5612391618482935,-3.5150011000166947,-3.4690091579979447,-3.4232697963688,-3.3777894638934587,-3.3325745871996828,-3.28763156033509,
-3.2429667342460005,-3.1985864062220579,-3.1544968093503445,-3.1107041020228303,-3.0672143575408053,-3.0240335538593728,-2.9811675635142034,-2.938622143771517,-2.8964029270407594,-2.8545154115876161,-2.8129649525829539,
-2.7717567535209664,-2.7308958580372953,-2.6903871421552208,-2.6502353069851705,-2.6104448718998587,-2.5710201682043485,-2.5319653333172369,-2.49328430547608,-2.4549808189770825,-2.4170583999560331,-2.379520362714465,
-2.34236980659215,-2.3056096133842154,-2.2692424452985311,-2.2332707434464933,-2.1976967268579632,-2.1625223920089405,-2.1277495128485375,-2.0933796413099874,-2.0594141082887938,-2.02585402506967,-1.9927002851826727,
-1.9599535666678465,-1.9276143347268384,-1.895682844739206,-1.864159145620629,-1.8330430834998552,-1.8023343056909957,-1.7720322649377207,-1.7421362239059666,-1.7126452599019613,-1.6835582697926785,-1.6548739751062436,
-1.6265909272903134,-1.5987075131070414,-1.5712219601438937,-1.5441323424203031,-1.5174365860709185,-1.491132475087023,-1.4652176570985425,-1.4396896491799398,-1.4145458436641916,-1.3897835139499377,-1.365399820287817,
-1.3413918155329039,-1.3177564508510682,-1.294490581367975,-1.2715909717503127,-1.249054301709706,-1.2268771714205995,-1.2050561068442127,-1.1835875649514565,-1.1624679388384496,-1.1416935627290061,-1.1212607168591577,
-1.1011656322394323,-1.0814044952912469,-1.0619734523543603,-1.0428686140629,-1.024086059588001,-1.0056218407455979,-0.98747198596836527,-0.969632504141245,-0.95209938830039464,-0.93486861919576036,-0.91793616871782913,
-0.90129800318941877,-0.88495008652366347,-0.86888838324960382,-0.853108861407036,-0.83760749531248513,-0.82238026819835863,-0.80742317472750869,-0.79273222338558269,-0.77830343875366992,-0.76413286366387179,-0.75021656124051506,
-0.73655061682981438,-0.72313113982085453,-0.70995426536082,-0.69701615596743793,-0.68431300304163389,-0.67184102828341652,-0.65959648501402046,-0.6475756594073323,-0.6357748716336229,-0.62419047691858975,-0.61281886652069284,
-0.60165646862974065,-0.59069974918964752,-0.57994521264824628,-0.56938940263699778,-0.55902890258338944,-0.54886033625876984,-0.53888036826430741,-0.5290857044577103,-0.51947309232328376,-0.510039321287844,-0.500781222984943,
-0.49169567146980092,-0.48277958338727522,-0.47402991809513378,-0.4654436777448353,-0.45701790732195441,-0.44874969464832448,-0.44063617034790847,-0.43267450777834088,-0.42486192293002345,-0.41719567429459242,-0.40967306270451226},
new double[]{
-14.644593407088669,-14.594593502631554,-14.544593603073,-14.49459370866416,-14.444593819669061,-14.39459393636527,-14.34459405904458,-14.29459418801375,-14.24459432359526,-14.19459446612813,-14.144594615968757,-14.09459477349181,
-14.044594939091171,-13.99459511318091,-13.944595296196335,-13.89459548859506,-13.844595690858171,-13.794595903491413,-13.74459612702646,-13.694596362022248,-13.644596609066365,-13.594596868776526,-13.544597141802113,
-13.4945974288258,-13.444597730565267,-13.394598047774977,-13.344598381248082,-13.29459873181839,-13.244599100362462,-13.194599487801792,-13.14459989510512,-13.094600323290848,-13.04460077342959,-12.994601246646841,
-12.9446017441258,-12.894602267110324,-12.844602816908031,-12.794603394893583,-12.744604002512103,-12.694604641282805,-12.644605312802781,-12.594606018750996,-12.544606760892483,-12.494607541082759,-12.444608361272454,
-12.394609223512195,-12.344610129957726,-12.294611082875296,-12.244612084647324,-12.194613137778347,-12.144614244901293,-12.094615408784039,-12.044616632336343,-11.994617918617113,-11.94461927084204,-11.894620692391642,
-11.844622186819708,-11.794623757862167,-11.744625409446437,-11.694627145701217,-11.644628970966817,-11.594630889805984,-11.544632907015307,-11.494635027637195,-11.444637256972463,-11.394639600593575,-11.344642064358553,
-11.294644654425607,-11.244647377268505,-11.194650239692736,-11.144653248852494,-11.094656412268536,-11.044659737846953,-10.994663233898889,-10.944666909161288,-10.894670772818692,-10.844674834526138,-10.794679104433254,
-10.744683593209562,-10.69468831207109,-10.644693272808327,-10.594698487815624,-10.544703970122075,-10.49470973342399,-10.444715792119027,-10.394722161342044,-10.344728857002801,-10.294735895825584,-10.244743295390823,
-10.194751074178868,-10.144759251615964,-10.094767848122585,-10.04477688516422,-9.99478638530474,-9.9447963722625037,-9.8948068709692762,-9.8448179076321711,-9.7948295097987419,-9.7448417064253512,-9.6948545279490386,
-9.6448680063630174,-9.5948821752960018,-9.544897070095562,-9.49491272791568,-9.4449291878087678,-9.3949464908222975,-9.3449646801003539,-9.2949838009902876,-9.2450039011547673,-9.1950250306894628,-9.1450472422466742,
-9.09507059116517,-9.045095135606557,-8.9951209366984983,-8.9451480586851133,-8.895176569084926,-8.8452065388567,-8.7952380425735814,-8.7452711586059237,-8.6953059693132371,-8.64534256124568,-8.59538102535559,
-8.5454214572194722,-8.4954639572710331,-8.4455086310457013,-8.3955555894372331,-8.3456049489669581,-8.2956568320662516,-8.2457113673728824,-8.1957686900418327,-8.1458289420713168,-8.0958922726446545,-8.0459588384887617,
-7.9960288042499785,-7.9461023428880511,-7.8961796360890695,-7.8462608746982,-7.7963462591731,-7.74643600005891,-7.6965303184857792,-7.6466294466898619,-7.5967336285588258,-7.5468431202028645,-7.4969581905523031,
-7.4470791219828776,-7.3972062109698129,-7.3473397687718478,-7.2974801221463874,-7.2476276140969853,-7.1977826046543809,-7.1479454716923438,-7.098116611779596,-7.0482964410690805,-6.9984853962259015,-6.9486839353952075,
-6.8988925392113529,-6.8491117118496323,-6.7993419821218852,-6.7495839046172685,-6.699838060889447,-6.6501050606914447,-6.6003855432593532,-6.5506801786460329,-6.5009896691059064,-6.4513147505318473,-6.4016561939451035,
-6.352014807039061,-6.3023914357775928,-6.2527869660485331,-6.2032023253727395,-6.1536384846689751,-6.1040964600746834,-6.0545773148225086,-6.0050821611721643,-5.95561216239701,-5.9061685348243813,-5.8567525499284248,
-5.807365536473827,-5.7580088827084541,-5.7086840386025184,-5.6593925181314386,-5.6101359015991035,-5.5609158379977233,-5.5117340473999494,-5.4625923233783507,-5.4134925354467356,-5.3644366315172078,-5.3154266403661561,
-5.266464674101706,-5.2175529306244748,-5.168693696072701,-5.1198893472421316,-5.0711423539702523,-5.0224552814737073,-4.9738307926269973,-4.9252716501697948,-4.876780718829453,-4.8283609673446222,-4.78001547037515,
-4.7317474102828561,-4.6835600787671394,-4.6354568783389025,-4.5874413236157814,-4.5395170424213553,-4.49168777667071,-4.4439573830246113,-4.39632983329448,-4.3488092145804984,-4.30139972912538,-4.2541056938667845,
-4.2069315396718707,-4.1598818102382218,-4.1129611606463046,-4.0661743555496539,-4.0195262669902778,-3.9730218718281725,-3.9266662487754949,-3.8804645750276769,-3.8344221224857513,-3.7885442535662333,-3.7428364165971577,
-3.6973041408012248,-3.651953030869477,-3.6067887611314959,-3.5618170693307056,-3.5170437500160423,-3.4724746475639008,-3.428115648846938,-3.3839726755689004,-3.340051676287199,-3.2963586181473805,-3.2528994783559493,
-3.2096802354201617,-3.1667068601853741,-3.1239853067023042,-3.0815215029581067,-3.0393213415064686,-2.9973906700329755,-2.9557352818927605,-2.914360906657953,-2.8732732007126467,-2.8324777379330088,-2.7919800004898123,
-2.7517853698099972,-2.7118991177329632,-2.67232639789611,-2.633072237382712,-2.5941415286635707,-2.5555390218620091,-2.5172693173697467,-2.4793368588389675,-2.4417459265735508,-2.4045006313399742,-2.3676049086158555,
-2.3310625132914837,-2.2948770148370561,-2.2590517929456833,-2.2235900336595766,-2.1884947259842455,-2.153768658992973,-2.119414419421374,-2.0854343897494672,-2.051830746766429,-2.0186054606110519,-1.9857602942789225,
-1.9532968035854879,-1.9212163375724431,-1.8895200393433476,-1.8582088473129628,-1.8272834968535863,-1.7967445223205851,-1.7665922594384202,-1.7368268480277045,-1.7074482350532383,-1.678456177972514,-1.6498502483638717,
-1.6216298358133061,-1.5937941520388834,-1.5663422352317831,-1.5392729545931594,-1.5125850150462941,-1.4862769621038725,-1.4603471868706655,-1.4347939311624207,-1.4096152927223473,-1.3848092305172182,-1.3603735700958033,
-1.3363060089930643,-1.3126041221643077,-1.2892653674342625,-1.2662870909468571,-1.2436665326022747,-1.2214008314686775,-1.1994870311568173,-1.1779220851465502,-1.1567028620550819,-1.1358261508375602,-1.1152886659114041,
-1.095087052196519,-1.0752178900642755,-1.0556777001888504,-1.0364629482952052,-1.0175700497986435,-0.99899537433151786,-0.98073525015326135,-0.96278596844049047,-0.94514378745447281,-0.92780493658376739,-0.91076562026032748,
-0.89402202174781609,-0.87757030680130532,-0.86140662719793193,-0.845527124138452,-0.82992793151997479,-0.81460517908048036,-0.79955499541600739,-0.7847735108716718,-0.77025686030791241,-0.75600118574358444,-0.74200263887771722,
-0.72825738349193192,-0.71476159773567172,-0.70151147629653732,-0.6885032324581426,-0.67573310004801268,-0.66319733527813285,-0.650892218480838,-0.63881405574279082,-0.62695918043984777,-0.61532395467565082,-0.60390477062680925,
-0.59269805179755364,-0.581700254186751,-0.57090786737017107,-0.560317415500886,-0.54992545823066574,-0.53972859155521768,-0.52972344858608322,-0.51990670025197772,-0.51027505593231748,-0.5008252640256412,-0.49155411245558189,
-0.48245842911700249,-0.47353508226485275,-0.46478098084825303,-0.45619307479225391,-0.44776835522966385,-0.43950385468527658,-0.431396647214771,-0.42344384850049382,-0.41564261590627527,-0.4079901484933644,-0.40048368699950881},
new double[]{
-14.079350173866688,-14.031731258676867,-13.984112349945713,-13.936493447988216,-13.88887455313473,-13.841255665731719,-13.793636786142544,-13.746017914748288,-13.698399051948618,-13.650780198162693,-13.603161353830114,-13.555542519411921,
-13.507923695391636,-13.460304882276367,-13.412686080597947,-13.365067290914151,-13.317448513809953,-13.269829749898856,-13.222210999824281,-13.174592264261028,-13.126973543916808,-13.079354839533844,-13.031736151890552,
-12.984117481803315,-12.936498830128327,-12.888880197763532,-12.84126158565067,-12.793642994777402,-12.746024426179556,-12.698405880943474,-12.650787360208472,-12.60316886516943,-12.55555039707949,-12.507931957252909,
-12.460313547068033,-12.412695167970423,-12.36507682147613,-12.317458509175136,-12.269840232734952,-12.222221993904409,-12.174603794517608,-12.126985636498096,-12.079367521863205,-12.031749452728644,-11.984131431313282,
-11.936513459944182,-11.888895541061878,-11.841277677225902,-11.793659871120587,-11.74604212556115,-11.698424443500072,-11.650806828033788,-11.60318928240971,-11.555571810033577,-11.507954414477174,-11.460337099486431,
-11.412719868989901,-11.36510272710767,-11.317485678160683,-11.269868726680537,-11.222251877419744,-11.174635135362495,-11.127018505735952,-11.079401994022087,-11.031785605970086,-10.984169347609383,-10.936553225263305,
-10.888937245563385,-10.841321415464382,-10.793705742260018,-10.746090233599498,-10.698474897504807,-10.65085974238888,-10.603244777074625,-10.555630010814896,-10.508015453313421,-10.46040111474675,-10.412787005787276,
-10.36517313762737,-10.317559522004679,-10.269946171228675,-10.222333098208473,-10.174720316482002,-10.127107840246605,-10.079495684391109,-10.031883864529453,-9.9842723970359586,-9.9366612990822958,-9.8890505886762448,
-9.8414402847023457,-9.7938304069644975,-9.7462209762306458,-9.6986120142796022,-9.6510035439501589,-9.60339558919256,-9.5557881751224674,-9.5081813280775354,-9.4605750756767151,-9.4129694468824265,-9.3653644720657248,
-9.3177601830746148,-9.2701566133056552,-9.2225537977790211,-9.174951773217165,-9.1273505781272721,-9.07975025288769,-9.03215083983849,-8.9845523833763981,-8.9369549300542648,-8.8893585286853174,-8.8417632304523934,
-8.7941690890224109,-8.7465761606663133,-8.6989845043847325,-8.6513941820396649,-8.6038052584924074,-8.55621780174808,-8.5086318831069967,-8.4610475773232583,-8.4134649627708367,-8.365884121617551,-8.31830514000726,
-8.27072810825066,-8.2231531210250957,-8.1755802775837516,-8.1280096819747,-8.0804414432702032,-8.032875675806773,-7.9853124994364224,-7.9377520397896459,-7.8901944285506289,-7.8426398037452323,-7.7950883100422956,
-7.7475400990688676,-7.6999953297399335,-7.6524541686032936,-7.604916790200213,-7.5573833774425356,-7.5098541220069306,-7.4623292247470117,-7.4148088961240477,-7.3672933566570364,-7.3197828373929275,-7.2722775803978026,
-7.2247778392698452,-7.1772838796749516,-7.1297959799058708,-7.0823144314657611,-7.0348395396770949,-6.9873716243168387,-6.9399110202788767,-6.8924580782646592,-6.8450131655030457,-6.7975766665003725,-6.7501489838217408,
-6.7027305389045617,-6.6553217729053813,-6.6079231475810163,-6.5605351462050354,-6.5131582745205971,-6.4657930617306683,-6.4184400615266073,-6.3710998531560854,-6.323773042531295,-6.2764602633783353,-6.2291621784286484,
-6.1818794806533033,-6.1346128945408775,-6.0873631774195971,-6.0401311208243236,-5.9929175519088753,-5.94572333490406,-5.8985493726216651,-5.8513966080045225,-5.8042660257226064,-5.7571586538149484,-5.7100755653769548,
-5.66301788029253,-5.6159867670101367,-5.5689834443617148,-5.5220091834230907,-5.4750653094141963,-5.4281532036371436,-5.3812743054498045,-5.3344301142722257,-5.2876221916227939,-5.2408521631806488,-5.19412172087041,
-5.1474326249648312,-5.10078670620047,-5.054185867900995,-5.0076320881021967,-4.9611274216722148,-4.9146740024199422,-4.8682740451839814,-4.8219298478939159,-4.7756437935950879,-4.72941835242744,-4.6832560835483985,
-4.6371596369891623,-4.5911317554331719,-4.5451752759049864,-4.4992931313572324,-4.4534883521427888,-4.4077640673588911,-4.36212350604944,-4.3165699982514028,-4.271106975870925,-4.2257379733745326,-4.180466628280656,
-4.1352966814366887,-4.0902319770668143,-4.0452764625760125,-4.000434188095948,-3.9557093057588317,-3.9111060686858869,-3.8666288296777327,-3.8222820395948029,-3.7780702454168726,-3.7339980879718682,-3.6900702993253733,
-3.6462916998236321,-3.6026671947843623,-3.559201770831343,-3.5159004918705152,-3.4727684947072048,-3.4298109843060667,-3.3870332286974114,-3.3444405535356969,-3.30203833631817,-3.259832000273839,-3.2178270079351887,
-3.1760288544072695,-3.1344430603509537,-3.0930751646993042,-3.0519307171280143,-3.011015270302845,-2.9703343719288009,-2.9298935566274573,-2.8896983376703762,-2.8497541985978727,-2.8100665847535433,-2.77064089476588,
-2.7314824720090094,-2.6925965960750751,-2.653988474291006,-2.6156632333124312,-2.5776259108272548,-2.5398814474009273,-2.5024346784947533,-2.4652903266876431,-2.4284529941305681,-2.391927155261659,-2.3557171498083411,
-2.3198271761012315,-2.2842612847226689,-2.2490233725107878,-2.2141171769379722,-2.1795462708803512,-2.1453140577927892,-2.1114237673015244,-2.0778784512243313,-2.0446809800257784,-2.0118340397128556,-1.9793401291740196,
-1.9472015579624893,-1.9154204445225085,-1.8839987148552471,-1.8529381016190623,-1.8222401436570059,-1.7919061859427374,-1.7619373799344087,-1.7323346843246177,-1.7030988661731981,-1.6742305024084214,-1.6457299816811335,
-1.6175975065554371,-1.5898330960187566,-1.5624365882934861,-1.5354076439319129,-1.5087457491757357,-1.4824502195612368,-1.4565202037510356,-1.4309546875733155,-1.4057524982494964,-1.3809123087914923,-1.3564326425499484,
-1.3323118778951977,-1.308548253013075,-1.285139870798212,-1.2620847038279606,-1.2393805994006726,-1.2170252846226897,-1.1950163715290514,-1.173351362223614,-1.1520276540249861,-1.1310425446054042,-1.1103932371104115,
-1.090076845247943,-1.070090398336158,-1.0504308463001071,-1.0310950646080406,-1.0120798591389013,-0.993381970973235,-0.97499808110045494,-0.9569248150360633,-0.93915874734308225,-0.92169640605257985,-0.90453427697877731,
-0.88766880792480352,-0.87109641277572014,-0.8548134754759642,-0.83881635388885611,-0.82310138353629658,-0.807664881217221,-0.79250314850379855,-0.7776124751147574,-0.76298914216558522,-0.74862942529569165,-0.73452959767294035,
-0.72068593287624572,-0.70709470765720039,-0.69375220458194409,-0.68065471455470727,-0.66779853922466825,-0.65517999327794041,-0.642795406616676,-0.6306411264274111,-0.61871351914091111,-0.60700897228588246,-0.59552389623901691,
-0.58425472587391325,-0.57319792211149267,-0.56234997337457771,-0.55170739694934945,-0.54126674025643084,-0.53102458203436731,-0.52097753343828646,-0.5111222390565282,-0.501455377848028,-0.49197366400322989,-0.48267384773128624,
-0.47355271597627774,-0.46460709306516029,-0.45583384129011068,-0.44722986142790366,-0.43879209319891405,-0.43051751566828894,-0.42240314759178865,-0.41444604770874338,-0.40664331498451867,-0.39899208880482789,-0.39148954912417377},
new double[]{
-13.559855646377359,-13.514401279893809,-13.468946921732947,-13.423492572281804,-13.3780382319454,-13.332583901147595,-13.287129580331959,-13.241675269962682,-13.196220970525548,-13.150766682528927,-13.105312406504826,-13.059858143009997,
-13.014403892627072,-12.968949655965789,-12.923495433664227,-12.878041226390142,-12.832587034842341,-12.787132859752122,-12.741678701884794,-12.696224562041248,-12.650770441059619,-12.605316339817012,-12.559862259231322,
-12.514408200263119,-12.468954163917644,-12.423500151246877,-12.378046163351716,-12.332592201384248,-12.28713826655013,-12.241684360111078,-12.196230483387479,-12.15077663776111,-12.105322824678,-12.059869045651409,
-12.014415302264965,-11.968961596175925,-11.9235079291186,-11.878054302907941,-11.832600719443287,-11.787147180712282,-11.741693688794985,-11.696240245868172,-11.650786854209814,-11.605333516203801,-11.559880234344849,
-11.514427011243665,-11.468973849632326,-11.423520752369928,-11.378067722448488,-11.332614762999123,-11.287161877298507,-11.241709068775647,-11.196256341018954,-11.150803697783656,-11.105351142999545,-11.05989868077909,
-11.014446315425934,-10.96899405144376,-10.9235418935456,-10.878089846663558,-10.83263791595898,-10.787186106833119,-10.741734424938265,-10.696282876189416,-10.650831466776475,-10.605380203177028,-10.559929092169687,
-10.514478140848095,-10.469027356635538,-10.423576747300258,-10.378126320971472,-10.332676086156129,-10.287226051756454,-10.241776227088291,-10.196326621900303,-10.150877246394069,-10.1054281112451,-10.059979227624829,
-10.014530607223634,-9.9690822622749078,-9.9236342055802513,-9.8781864505358374,-9.8327390111599851,-9.7872919021220159,-9.7418451387724367,-9.69639873717453,-9.6509527141373948,-9.60550708725052,-9.5600618749199491,
-9.51461709640612,-9.46917277186345,-9.4237289223817431,-9.378285570029524,-9.3328427378993428,-9.2874004501551983,-9.2419587320821233,-9.1965176101380628,-9.1510771120081458,-9.1056372666614482,-9.0601981044103628,
-9.0147596569727035,-8.9693219575366747,-8.9238850408287949,-8.8784489431849654,-8.83301370262479,-8.78757935892929,-8.742145953722197,-8.6967135305549537,-8.6512821349956024,-8.6058518147217526,-8.5604226196177642,
-8.5149946018763973,-8.4695678161050676,-8.4241423194369638,-8.3787181716471988,-8.3332954352742643,-8.2878741757469747,-8.242454461517184,-8.1970363641985085,-8.1516199587113309,-8.1062053234343452,-8.0607925403629448,
-8.01538169527475,-7.9699728779025767,-7.9245661821151785,-7.8791617061060855,-7.8337595525909052,-7.7883598290134346,-7.7429626477609652,-7.6975681263891689,-7.6521763878569775,-7.6067875607718625,-7.56140177964597,
-7.5160191851635458,-7.4706399244601265,-7.4252641514139865,-7.37989202695033,-7.3345237193587591,-7.2891594046245469,-7.24379926677427,-7.1984434982363794,-7.153092300217283,-7.107745883093564,-7.0624044668209534,
-7.017068281360693,-6.971737567123963,-6.9264125754350356,-6.8810935690138688,-6.8357808224788315,-6.790474622870307,-6.7451752701959009,-6.6998830779980239,-6.6545983739446095,-6.609321500443766,-6.5640528152831346,
-6.5187926922947748,-6.4735415220463857,-6.4282997125596681,-6.3830676900566639,-6.3378458997348748,-6.2926348065719955,-6.2474348961610593,-6.2022466755767978,-6.1570706742740207,-6.1119074450187592,-6.0667575648529422,
-6.0216216360933146,-5.9765002873652779,-5.9313941746723104,-5.8863039825015449,-5.8412304249660618,-5.7961742469843571,-5.7511362254974028,-5.7061171707236138,-5.6611179274519534,-5.6161393763733027,-5.5711824354501065,
-5.5262480613241731,-5.48133725076238,-5.4364510421398693,-5.3915905169601546,-5.3467568014113676,-5.3019510679576891,-5.2571745369647784,-5.2124284783577783,-5.1677142133102389,-5.1230331159620093,-5.0783866151638781,
-5.0337761962464187,-4.9892034028101842,-4.9446698385340326,-4.9001771689980274,-4.8557271235169246,-4.81132149697993,-4.76696215169191,-4.722651019210879,-4.67839010217609,-4.6341814761206086,-4.5900272912617908,
-4.5459297742625955,-4.5018912299561649,-4.457914043025645,-4.4140006796307141,-4.370153688971814,-4.326375704782599,-4.2826694467406723,-4.2390377217862367,-4.1954834253378781,-4.1520095423943078,-4.108619148510571,
-4.065315410636904,-4.0221015878081889,-3.9789810316717671,-3.935957186841232,-3.8930335910637828,-3.8502138751887376,-3.8075017629249164,-3.7649010703748,-3.7224157053336837,-3.6800496663424234,-3.6378070414829051,
-3.5956920069059679,-3.5537088250822593,-3.5118618427673405,-3.470155488673333,-3.428594270840474,-3.3871827737031452,-3.3459256548462433,-3.3048276414491715,-3.2638935264162341,-3.2231281641938145,-3.1825364662763929,
-3.1421233964052049,-3.1018939654651363,-3.0618532260872935,-3.0220062669665544,-2.9823582069052756,-2.9429141885962036,-2.9036793721594734,-2.8646589284503756,-2.8258580321563116,-2.7872818547030018,-2.7489355569915728,
-2.71082428198958,-2.6729531472003405,-2.6353272370360981,-2.5979515951215424,-2.5608312165550422,-2.5239710401555757,-2.4873759407238114,-2.4510507213460344,-2.4150001057696797,-2.379228730879078,-2.3437411392996776,
-2.3085417721584602,-2.27363496202753,-2.2390249260769393,-2.2047157594617106,-2.1707114289667606,-2.1370157669320253,-2.103632465478531,-2.0705650710545016,-2.037816979318813,-2.0053914303772618,-1.9732915043851831,
-1.9415201175279853,-1.9100800183891584,-1.8789737847132997,-1.8482038205696731,-1.81777235391983,-1.7876814345908458,-1.7579329326538189,-1.72852853720542,-1.6994697555485097,-1.6707579127661436,-1.6423941516816936,
-1.6143794331963151,-1.5867145369936166,-1.5594000626001083,-1.5324364307888674,-1.5058238853128272,-1.4795624949531916,-1.4536521558676954,-1.4280925942227747,-1.4028833690931635,-1.378023875612016,-1.353513348354338,
-1.3293508649363097,-1.3055353498129778,-1.282065578256798,-1.2589401804995868,-1.2361576460206203,-1.2137163279638679,-1.19161444766766,-1.1698500992904883,-1.1484212545170669,-1.1273257673292809,-1.1065613788271915,
-1.0861257220858296,-1.0660163270341292,-1.046230625342977,-1.0267659553100079,-1.0076195667294483,-0.98878862573597781,-0.97027021961227011,-0.95206136155055487,-0.93415899535921965,-0.91656000010615135,-0.89926119469117538,
-0.88225934234060654,-0.86555115501756208,-0.84913329774230717,-0.83300239281750377,-0.817155023953817,-0.80158774029188873,-0.78629706031722546,-0.77127947566506128,-0.75653145481274109,-0.74204944665763473,-0.72782988397903114,
-0.71386918678287081,-0.70016376552857007,-0.68671002423754479,-0.67350436348339038,-0.66054318326398065,-0.64782288575604885,-0.63533987795307545,-0.623090574187561,-0.61107139853898518,-0.599278787128959,-0.587709190305264,
-0.5763590747166385,-0.56522492528032231,-0.55430324704450007,-0.54359056694790253,-0.53308343547892512,-0.52277842823670873,-0.51267214739669975,-0.50276122308327031,-0.49304231465202231,-0.48351211188444065,-0.47416733609758438,
-0.46500474117152457,-0.45602111449724353,-0.44721327784771142,-0.43857808817484917,-0.4301124383350719,-0.42181325774608674,-0.41367751297759286,-0.40570220827850134,-0.39788438604325504,-0.39022112721979008,-0.38270955166163745},
new double[]{
-13.08065759463523,-13.037179570262197,-12.993701556398307,-12.950223553510549,-12.906745562086661,-12.863267582636054,-12.819789615690773,-12.776311661806506,-12.732833721563631,-12.689355795568318,-12.64587788445367,-12.60239998888092,
-12.558922109540687,-12.515444247154269,-12.471966402475026,-12.42848857628978,-12.385010769420324,-12.341532982724962,-12.298055217100135,-12.254577473482119,-12.211099752848785,-12.167622056221465,-12.124144384666858,
-12.080666739299069,-12.0371891212817,-11.993711531830058,-11.950233972213445,-11.906756443757562,-11.863278947847011,-11.819801485927915,-11.776324059510648,-11.732846670172691,-11.689369319561614,-11.645892009398185,
-11.602414741479631,-11.55893751768302,-11.515460339968824,-11.471983210384607,-11.428506131068913,-11.385029104255287,-11.341552132276506,-11.298075217568991,-11.254598362677395,-11.211121570259428,-11.16764484309086,
-11.124168184070784,-11.080691596227075,-11.037215082722121,-10.993738646858789,-10.950262292086674,-10.9067860220086,-10.863309840387437,-10.819833751153201,-10.776357758410478,-10.732881866446174,-10.689406079737612,
-10.64593040296098,-10.602454841000171,-10.558979398955989,-10.515504082155795,-10.472028896163547,-10.428553846790315,-10.385078940105245,-10.341604182447009,-10.298129580435774,-10.254655140985687,-10.211180871317932,
-10.16770677897434,-10.124232871831627,-10.080759158116246,-10.037285646419905,-9.9938123457157744,-9.9503392653754,-9.906866415186375,-9.8633938053707979,-9.8199214466045337,-9.77644935003734,-9.7329775273138761,
-9.6895059905956433,-9.6460347525839012,-9.6025638265435873,-9.5590932263283,-9.5156229664063883,-9.4721530618881857,-9.4286835285544548,-9.3852143828860886,-9.3417456420951162,-9.2982773241570769,-9.25480944784484,
-9.21134203276389,-9.1678750993891871,-9.1244086691036479,-9.0809427642383174,-9.037477408114313,-8.9940126250866186,-8.9505484405898041,-8.9070848811857513,-8.863621974613487,-8.8201597498412028,-8.7766982371205575,
-8.7332374680433684,-8.6897774756007848,-8.6463182942450718,-8.602859959954074,-8.5594025102985416,-8.5159459845123671,-8.4724904235659171,-8.4290358702425721,-8.38558236921859,-8.34212996714648,-8.2986787127420012,
-8.2552286568749516,-8.2117798526639181,-8.16833235557514,-8.1248862235256851,-8.081441516991088,-8.0379982991176639,-7.9945566358397029,-7.9511165960017154,-7.9076782514859758,-7.8642416773455706,-7.82080695194318,
-7.7773741570958466,-7.733943378225951,-7.69051470451869,-7.6470882290862914,-7.6036640491392671,-7.560242266164976,-7.5168229861138052,-7.4734063195932814,-7.4299923820704254,-7.3865812940826849,-7.3431731814577983,
-7.2997681755429324,-7.2563664134434784,-7.2129680382718693,-7.1695731994068357,-7.126182052763486,-7.0827947610746458,-7.0394114941838808,-6.9960324293506639,-6.9526577515681272,-6.9092876538939016,-6.865922337794502,
-6.8225620135037843,-6.7792069003959812,-6.7358572273738435,-6.692513233272436,-6.6491751672791422,-6.6058432893704442,-6.5625178707660643,-6.5191991944010592,-6.4758875554164677,-6.4325832616691363,-6.3892866342613406,
-6.3459980080908407,-6.3027177324220087,-6.259446171478678,-6.2161837050593736,-6.1729307291755688,-6.1296876567136342,-6.0864549181211371,-6.0432329621181413,-6.0000222564341605,-5.9568232885714059,-5.9136365665949553,
-5.8704626199504641,-5.8273020003100164,-5.784155282446676,-5.7410230651383127,-5.6979059721011893,-5.654804652953815,-5.6117197842114823,-5.5686520703118862,-5.5256022446721618,-5.4825710707775945,-5.43955934330222,
-5.3965678892614308,-5.3535975691966273,-5.31064927839184,-5.2677239481221854,-5.2248225469338445,-5.1819460819551795,-5.13909560023843,-5.0962721901312973,-5.0534769826775632,-5.0107111530457011,-4.967975921984273,
-4.9252725573026739,-4.8826023753755914,-4.8399667426693123,-4.7973670772877526,-4.7548048505358418,-4.7122815884976026,-4.66979887362599,-4.6273583463412455,-4.5849617066341892,-4.54261071567057,-4.5003071973922157,
-4.4580530401104053,-4.4158501980864671,-4.3737006930942863,-4.3316066159589752,-4.2895701280655976,-4.247593462831424,-4.2056789271348061,-4.1638289026933721,-4.1220458473838457,-4.0803322964954027,-4.0386908639081263,
-3.9971242431877583,-3.9556352085876036,-3.9142266159481438,-3.8729014034846325,-3.8316625924526968,-3.7905132876817649,-3.7494566779659735,-3.7084960363021109,-3.667634719964072,-3.62687617040334,-3.5862239129650519,
-3.5456815564093715,-3.505252792228108,-3.4649413937468174,-3.4247512150030244,-3.3846861893916698,-3.3447503280694608,-3.3049477181104665,-3.265282520406064,-3.2257589673031761,-3.1863813599757136,-3.1471540655251551,
-3.108081513807337,-3.0691681939837383,-3.0304186507968374,-2.9918374805704882,-2.9534293269376919,-2.9151988762996273,-2.877150853021337,-2.839290014371036,-2.8016211452115876,-2.7641490524542971,-2.7268785592867575,
-2.6898144991880568,-2.6529617097461813,-2.6163250262939441,-2.5799092753811781,-2.5437192681022758,-2.5077597932994027,-2.4720356106628376,-2.4365514437509233,-2.4013119729529753,-2.3663218284192493,-2.3315855829826466,
-2.2971077450972608,-2.2628927518191388,-2.2289449618547135,-2.19526864870228,-2.1618679939116552,-2.1287470804867086,-2.095909886454892,-2.0633602786271186,-2.0311020065704595,-1.9991386968150586,-1.967473847315488,
-1.9361108221854524,-1.9050528467233308,-1.8743030027445204,-1.8438642242349488,-1.8137392933384402,-1.7839308366889035,-1.7544413220965383,-1.725273055595471,-1.6964281788584368,-1.6679086669823338,-1.6397163266467056,
-1.6118527946454813,-1.5843195367906007,-1.55711784718454,-1.5302488478571747,-1.5037134887609431,-1.4775125481168729,-1.4516466331027256,-1.4261161808733069,-1.4009214599018895,-1.3760625716306896,-1.3515394524174544,
-1.3273518757644356,-1.3034994548153442,-1.2799816451053312,-1.2567977475485694,-1.233946911647664,-1.2114281389088664,-1.1892402864469054,-1.1673820707631872,-1.1458520716811365,-1.1246487364225544,-1.1037703838090456,
-1.0832152085728191,-1.0629812857614718,-1.0430665752217476,-1.0234689261476684,-1.004186081678917,-0.98521568353584865,-0.96655527667805274,-0.948202313973956,-0.9301541608695495,-0.91240810004493278,-0.89496133604799422,
-0.87781099989517553,-0.86095415362990979,-0.84438779482995785,-0.828108861055502,-0.81211423423048834,-0.79640074495032553,-0.78096517670965682,-0.7658042700445189,-0.75091472658377423,-0.73629321300527029,-0.72193636489271351,
-0.70784079048977333,-0.69400307434842468,-0.68041978086901822,-0.66708745773002087,-0.65400263920579826,-0.64116184937122089,-0.62856160519225679,-0.61619841950207688,-0.6040688038625337,-0.59216927131119024,-0.5804963389943667,
-0.56904653068694389,-0.5578163791999089,-0.54680242867685713,-0.53600123678087419,-0.52540937677340549,-0.51502343948689622,-0.50484003519313181,-0.49485579536934871,-0.48506737436429892,-0.47547145096656224,-0.46606472987748276,
-0.4568439430911872,-0.44780585118420246,-0.43894724451724226,-0.43026494435177132,-0.42175580388398626,-0.41341670919887047,-0.40524458014699116,-0.39723637114671029,-0.38938907191447319,-0.38169970812582893,-0.37416534200981555},
new double[]{
-12.637147669482456,-12.595481309126146,-12.553814961802212,-12.512148628065125,-12.470482308492946,-12.428816003688327,-12.387149714279566,-12.345483440921683,-12.303817184297571,-12.262150945119176,-12.220484724128728,-12.178818522100036,
-12.137152339839833,-12.095486178189171,-12.053820038024881,-12.012153920261099,-11.970487825850849,-11.928821755787704,-11.8871557111075,-11.845489692890142,-11.803823702261479,-11.762157740395253,-11.720491808515138,
-11.678825907896869,-11.637160039870446,-11.595494205822456,-11.553828407198466,-11.512162645505541,-11.470496922314856,-11.428831239264424,-11.387165598061939,-11.345500000487737,-11.30383444839789,-11.262168943727422,
-11.220503488493673,-11.178838084799793,-11.137172734838396,-11.095507440895364,-11.053842205353812,-11.012177030698224,-10.970511919518764,-10.928846874515768,-10.887181898504432,-10.845516994419691,-10.80385216532132,
-10.762187414399232,-10.720522744979013,-10.678858160527696,-10.637193664659774,-10.59552926114346,-10.553864953907237,-10.512200747046659,-10.470536644831457,-10.428872651712949,-10.387208772331748,-10.34554501152582,
-10.303881374338864,-10.262217866029058,-10.220554492078176,-10.178891258201089,-10.137228170355675,-10.095565234753138,-10.053902457868782,-10.01223984645322,-9.9705774075440843,-9.9289151484782128,-9.8872530769043614,
-9.8455912007964557,-9.8039295284674015,-9.7622680685834826,-9.7206068301793742,-9.6789458226737768,-9.6372850558857337,-9.5956245400516273,-9.5539642858428913,-9.5123043043844913,-9.4706446072741688,-9.4289852066025084,
-9.38732611497386,-9.3456673455281347,-9.3040089119635354,-9.2623508285602369,-9.2206931102050778,-9.1790357724172917,-9.1373788313753312,-9.0957223039448163,-9.0540662077076686,-9.0124105609924836,-8.9707553829061784,
-8.9291006933669657,-8.8874465131387446,-8.845792863866917,-8.8041397681157285,-8.762487249407183,-8.7208353322615864,-8.6791840422398181,-8.6375334059873552,-8.59588345128018,-8.5542342070725912,-8.5125857035470425,
-8.4709379721660767,-8.42929104572642,-8.3876449584153736,-8.3459997458695661,-8.3043554452361441,-8.2627120952365622,-8.221069736233023,-8.1794284102977013,-8.1377881612848686,-8.0961490349060252,-8.0545110788081669,
-8.0128743426553335,-7.9712388782135353,-7.9296047394392275,-7.8879719825714707,-7.8463406662279125,-7.80471085150476,-7.7630826020809005,-7.7214559843263357,-7.6798310674151029,-7.6382079234428693,-7.5965866275493745,
-7.5549672580459317,-7.5133498965481715,-7.4717346281142492,-7.4301215413887247,-7.3885107287523457,-7.346902286477957,-7.3052963148927823,-7.2636929185473278,-7.2220922063911637,-7.1804942919558421,-7.1388992935452436,
-7.09730733443362,-7.0557185430716372,-7.0141330533007178,-6.972551004576002,-6.9309725421982424,-6.8893978175549773,-6.8478269883713123,-6.80626021897068,-6.764697680545936,-6.7231395514411627,-6.6815860174445731,
-6.6400372720929148,-6.5984935169877721,-6.556954962124192,-6.5154218262320631,-6.473894337130683,-6.4323727320969626,-6.3908572582477428,-6.3493481729366605,-6.3078457441660829,-6.2663502510145648,-6.22486198408034,
-6.1833812459413489,-6.14190835163232,-6.1004436291393978,-6.05898741991288,-6.017540079398545,-5.9761019775881445,-5.93467349958956,-5.8932550462171687,-5.8518470346029607,-5.8104498988289119,-5.7690640905811623,
-5.7276900798264982,-5.6863283555116544,-5.6449794262859321,-5.6036438212476085,-5.5623220907146029,-5.521014807019843,-5.4797225653317465,-5.4384459845002,-5.3971857079284007,-5.3559424044708628,-5.3147167693578758,
-5.2735095251466335,-5.2323214226992025,-5.1911532421874469,-5.150005794124958,-5.1088799204259487,-5.0677764954910218,-5.0266964273195986,-4.9856406586487356,-4.9446101681179071,-4.9036059714592719,-4.8626291227127663,
-4.82168071546527,-4.7807618841129358,-4.7398738051456153,-4.6990176984521561,-4.6581948286451755,-4.6174065064037091,-4.5766540898319645,-4.5359389858321757,-4.4952626514893321,-4.4546265954653625,-4.4140323794000391,
-4.3734816193156885,-4.33297598702247,-4.2925172115207344,-4.2521070803966854,-4.21174744120726,-4.1714402028498521,-4.1311873369121779,-4.0909908789972853,-4.0508529300183609,-4.0107756574576872,-3.9707612965837642,
-3.9308121516202985,-3.890930596860426,-3.8511190777192419,-3.8113801117173938,-3.7717162893882112,-3.7321302751005652,-3.6926248077894033,-3.6532027015856681,-3.6138668463371046,-3.574620208011289,-3.5354658289720771,
-3.4964068281205667,-3.4574464008916204,-3.4185878190969912,-3.379834430606135,-3.3411896588559071,-3.3026570021805077,-3.2642400329532579,-3.2259423965321123,-3.1877678100011662,-3.1497200607008664,-3.1118030045401666,
-3.0740205640844525,-3.0363767264137547,-2.9988755407465151,-2.9615211158250139,-2.9243176170594714,-2.8872692634288257,-2.8503803241372418,-2.8136551150265325,-2.7770979947458376,-2.7407133606811671,-2.7045056446486626,
-2.668479308356781,-2.6326388386439206,-2.596988742499398,-2.5615335418770417,-2.5262777683120325,-2.49122595735299,-2.456382642822605,-2.4217523509214147,-2.3873395941905398,-2.3531488653503665,-2.3191846310332438,
-2.2854513254292561,-2.2519533438650314,-2.2186950363363294,-2.1856807010158139,-2.1529145777579548,-2.1204008416233986,-2.0881435964454211,-2.0561468684611741,-2.0244146000304166,-1.9929506434642426,-1.9617587549859779,
-1.9308425888459588,-1.9002056916112784,-1.8698514966508328,-1.8397833188351227,-1.8100043494692502,-1.7805176514764329,-1.7513261548481316,-1.7224326523755702,-1.6938397956760289,-1.6655500915258217,-1.6375658985103503,
-1.609889424000061,-1.5825227214595352,-1.5554676880953389,-1.5287260628466386,-1.502299424720996,-1.4761891914761651,-1.450396618647176,-1.4249227989164821,-1.3997686618234986,-1.3749349738084791,-1.3504223385843568,
-1.3262311978289456,-1.3023618321887396,-1.2788143625844923,-1.2555887518077804,-1.2326848063968878,-1.2101021787795676,-1.1878403696695599,-1.1658987307031667,-1.1442764673017025,-1.1229726417452521,-1.1019861764428778,
-1.0813158573842137,-1.0609603377572725,-1.0409181417172577,-1.0211876682912218,-1.00176719540353,-0.98265488400727974,-0.96384878230707594,-0.94534683005887732,-0.92714686293298265,-0.90924661692664388,-0.89164373281323661,
-0.87433576061540841,-0.85732016409014133,-0.84059432521420718,-0.82415554865905782,-0.80800106624477253,-0.79212804136327575,-0.77653357336163475,-0.7612147018768548,-0.746168411114185,-0.7313916340615525,-0.71688125663333413,
-0.70263412173725726,-0.68864703325879828,-0.67491675995800138,-0.66144003927418971,-0.64821358103456217,-0.63523407106318353,-0.62249817468735824,-0.61000254013885169,-0.59774380184786668,-0.58571858362810869,-0.57392350175167717,
-0.56235516791289963,-0.55101019208058521,-0.53988518523850693,-0.52897676201424038,-0.51828154319677266,-0.50779615814357182,-0.49751724707805034,-0.48744146327859078,-0.477565475160507,-0.4678859682525065,-0.45839964706939029,
-0.44910323688288178,-0.43999348539261179,-0.4310671642994095,-0.42232107078315362,-0.41375202888753077,-0.40535689081412346,-0.3971325381283175,-0.389075882879568,-0.38118386863860781,-0.3734534714542086,-0.36588170073213044},
new double[]{
-12.22540294193896,-12.185403331609646,-12.145403737182823,-12.105404159307463,-12.065404598659022,-12.025405055940514,-11.985405531883645,-11.945406027249982,-11.905406542832166,-11.865407079455187,-11.825407637977696,-11.785408219293387,
-11.745408824332422,-11.705409454062917,-11.665410109492497,-11.625410791669902,-11.585411501686666,-11.545412240678871,-11.505413009828947,-11.465413810367584,-11.425414643575683,-11.385415510786416,-11.345416413387355,
-11.305417352822689,-11.265418330595537,-11.225419348270354,-11.185420407475428,-11.145421509905487,-11.105422657324411,-11.065423851568049,-11.025425094547162,-10.985426388250469,-10.945427734747835,-10.905429136193579,
-10.86543059482992,-10.825432112990558,-10.785433693104412,-10.745435337699499,-10.705437049406978,-10.665438830965359,-10.625440685224877,-10.585442615152052,-10.545444623834431,-10.505446714485524,-10.465448890449938,
-10.425451155208734,-10.385453512384975,-10.34545596574953,-10.305458519227097,-10.265461176902475,-10.225463943027092,-10.185466822025802,-10.145469818503949,-10.105472937254735,-10.065476183266874,-10.025479561732558,
-9.9854830780557613,-9.9454867378608682,-9.905490547001655,-9.8654945115706454,-9.8254986379088383,-9.7855029326158345,-9.7455074025603761,-9.7055120548913152,-9.6655168970490219,-9.6255219367772771,-9.5855271821356141,
-9.5455326415122013,-9.505538323637218,-9.46554423759679,-9.42555039284749,-9.3855567992314217,-9.3455634669919228,-9.3055704067899,-9.26557762972084,-9.2255851473324917,-9.1855929716432865,-9.1456011151614973,
-9.10560959090517,-9.0656184124228716,-9.025627593815285,-8.9856371497576681,-8.9456470955232259,-8.90565744700744,-8.8656682207533759,-8.8256794339780136,-8.7856911045996569,-8.7457032512664377,-8.7057158933859888,
-8.6657290511563048,-8.6257427455978615,-8.5857569985870335,-8.545771832890857,-8.5057872722031984,-8.4658033411823936,-8.4258200654903987,-8.3858374718335256,-8.34585558800481,-8.3058744429281113,-8.2658940667039662,
-8.2259144906573081,-8.1859357473870915,-8.1459578708179325,-8.10598089625381,-8.0660048604339352,-8.026029801590866,-7.986055759510946,-7.9460827755971843,-7.90611089293464,-7.8661401563584361,-7.8261706125244945,
-7.7862023099831035,-7.7462352992554182,-7.7062696329130285,-7.6663053656606861,-7.6263425544223491,-7.5863812584306336,-7.5464215393198408,-7.5064634612226691,-7.4665070908707767,-7.4265524976993236,-7.3865997539556565,
-7.3466489348122979,-7.3067001184843914,-7.26675338635179,-7.2268088230859453,-7.1868665167817944,-7.1469265590948252,-7.10698904538352,-7.0670540748573716,-7.0271217507306867,-6.9871921803823911,-6.9472654755220544,
-6.90734175236237,-6.8674211317983191,-6.8275037395932756,-6.7875897065722866,-6.7476791688228088,-6.7077722679031506,-6.6678691510589028,-6.6279699714476479,-6.5880748883722307,-6.5481840675228922,-6.5082976812285818,
-6.46841590871776,-6.4285389363890175,-6.3886669580918438,-6.3488001754178915,-6.3089387980030827,-6.2690830438409186,-6.2292331396073592,-6.1893893209976456,-6.1495518330754457,-6.1097209306347251,-6.0698968785747152,
-6.0300799522884132,-5.9902704380649991,-5.9504686335065884,-5.9106748479597622,-5.8708894029622627,-5.8311126327053229,-5.7913448845120321,-5.751586519332184,-5.7118379122540395,-5.672099453033443,-5.6323715466407167,
-5.59265461382577,-5.5529490917018487,-5.5132554343483351,-5.47357411343303,-5.433905618854296,-5.3942504594034615,-5.3546091634478712,-5.3149822796349131,-5.27537037761739,-5.2357740488005318,-5.1961939071109322,
-5.1566305897876941,-5.1170847581959764,-5.07755709866315,-5.038048323337704,-4.9985591710709949,-4.9590904083218934,-4.9196428300843147,-4.88021726083756,-4.8408145555193363,-4.8014356005212306,-4.7620813147063545,
-4.7227526504487694,-4.683450594694218,-4.6441761700415958,-4.6049304358444489,-4.5657144893317243,-4.52652946674682,-4.4873765445038725,-4.4482569403600882,-4.4091719146027382,-4.3701227712493056,-4.331110859259093,
-4.2921375737544185,-4.2532043572493352,-4.2143127008836183,-4.1754641456595509,-4.1366602836788351,-4.0979027593767059,-4.05919327075012,-4.0205335705766325,-3.9819254676203339,-3.9433708278209689,-3.9048715754620864,
-3.866429694313827,-3.8280472287456639,-3.78972628480417,-3.7514690312505903,-3.7132777005527569,-3.6751545898255924,-3.6371020617142249,-3.5991225452134463,-3.5612185364170479,-3.523392599190303,-3.4856473657586871,
-3.4479855372057053,-3.4104098838725521,-3.3729232456521592,-3.3355285321700912,-3.2982287228446463,-3.2610268668184785,-3.2239260827540468,-3.18692955848522,-3.1500405505174554,-3.1132623833690802,-3.0765984487463984,
-3.0400522045455656,-3.0036271736744791,-2.9673269426882691,-2.9311551602324037,-2.8951155352878919,-2.85921183521361,-2.8234478835814016,-2.78782755780025,-2.7523547865265918,-2.7170335468586209,-2.6818678613133038,
-2.6468617945857598,-2.6120194500916218,-2.5773449662940271,-2.5428425128179715,-2.5085162863558446,-2.474370506369139,-2.4404094105924647,-2.4066372503471984,-2.3730582856732805,-2.3396767802888609,-2.306496996388669,
-2.2735231892931247,-2.2407596019613436,-2.2082104593822427,-2.1758799628589864,-2.1437722842029605,-2.1118915598543437,-2.0802418849471449,-2.0488273073372718,-2.0176518216128,-1.9867193631061122,-1.956033801927938,
-1.9255989370436006,-1.8954184904118949,-1.8654961012070364,-1.8358353201439885,-1.8064396039272275,-1.777312309842616,-1.7484566905115544,-1.7198758888259391,-1.6915729330817115,-1.6635507323279208,-1.6358120719472513,
-1.6083596094829071,-1.5811958707255931,-1.5543232460731022,-1.5277439871737248,-1.5014602038633396,-1.4754738614046532,-1.4497867780356184,-1.4244006228326147,-1.399316913892507,-1.374537016836239,-1.3500621436351685,
-1.3258933517599247,-1.3020315436501739,-1.2784774665023311,-1.2552317123709518,-1.2322947185783026,-1.209666768425427,-1.1873479921969288,-1.1653383684506655,-1.1436377255826011,-1.1222457436562137,-1.1011619564850794,
-1.0803857539565778,-1.0599163845840676,-1.039752958274391,-1.0198944492971414,-1.0003396994418203,-0.98108742134875815,-0.96213620199953065,-0.94348450635251779,-0.92513068110926022,-0.90707295859733572,-0.88930946075561934,
-0.87183820320799488,-0.85465709941184165,-0.83776396486793681,-0.82115652137877093,-0.80483240134268119,-0.78878915207164213,-0.77302424012103366,-0.75753505562020051,-0.74231891659314719,-0.72737307325925149,-0.71269471230443837,
-0.6982809611138231,-0.68412889195740512,-0.67023552612097093,-0.6565978379749402,-0.64321275897445973,-0.63007718158461545,-0.61718796312518975,-0.60454192952993391,-0.59213587901585829,-0.57996658565855863,-0.568030802870094,
-0.55632526677641647,-0.54484669949181164,-0.53359181228825148,-0.522557308657985,-0.511739887268088,-0.5011362448060751,-0.49074307871603368,-0.48055708982507139,-0.47057498486018839,-0.46079347885597127,-0.4512092974537813,
-0.44181917909335738,-0.43261987709798583,-0.4236081616545983,-0.41478082169035174,-0.4061346666474171,-0.39766652815785813,-0.38937326162062114,-0.38125174768277864,-0.3732988936272762,-0.36551163466952391,-0.357886935165254},
new double[]{
-11.842062047323545,-11.803600996632122,-11.765139965066146,-11.72667895337551,-11.688217962339508,-11.649756992767987,-11.611296045502547,-11.572835121417784,-11.534374221422583,-11.495913346461466,-11.457452497515986,-11.418991675606177,
-11.380530881792067,-11.342070117175242,-11.303609382900477,-11.265148680157431,-11.2266880101824,-11.188227374260151,-11.149766773725824,-11.1113062099669,-11.072845684425252,-11.034385198599287,-10.995924754046152,
-10.957464352384044,-10.919003995294593,-10.88054368452536,-10.842083421892413,-10.803623209283018,-10.765163048658421,-10.726702942056756,-10.688242891596058,-10.649782899477387,-10.611322967988091,-10.572863099505181,
-10.534403296498848,-10.495943561536114,-10.457483897284629,-10.419024306516608,-10.380564792112937,-10.342105357067423,-10.303646004491229,-10.265186737617459,-10.226727559805946,-10.188268474548218,-10.149809485472652,
-10.111350596349835,-10.072891811098147,-10.034433133789536,-9.99597456865554,-9.95751612009355,-9.9190577926732821,-9.880599591143552,-9.84214152043928,-9.8036835856887787,-9.7652257922213277,-9.72676814557505,
-9.6883106515050859,-9.6498533159920914,-9.6113961452510743,-9.5729391457405626,-9.5344823241721457,-9.4960256875203815,-9.45756924303309,-9.4191129982420545,-9.380656960974127,-9.3422011393627962,-9.30374554186017,
-9.26529017724946,-9.226835054657931,-9.1883801835703682,-9.1499255738430634,-9.1114712357183549,-9.0730171798397237,-9.034563417267492,-8.9961099594951257,-8.9576568184661731,-8.919204006591869,-8.8807515367694219,
-8.8422994224010143,-8.803847677413545,-8.76539631627914,-8.7269453540364665,-8.688494806312864,-8.6500446893473555,-8.6115950200145477,-8.5731458158494469,-8.53469709507327,-8.4962488766202284,-8.45780118016538,
-8.4193540261535365,-8.3809074358293287,-8.342461431268406,-8.30401603540987,-8.2655712720899484,-8.227127166077004,-8.1886837431078661,-8.1502410299256,-8.1117990543187233,-8.073357845161949,-8.0349174324585029,
-7.9964778473840772,-7.95803912233249,-7.9196012909631053,-7.88116438825008,-7.84272845053353,-7.8042935155726445,-7.7658596226008783,-7.7274268123832419,-7.6889951272758221,-7.65056461128758,-7.6121353101445246,
-7.5737072713563647,-7.5352805442857038,-7.4968551802199022,-7.4584312324456867,-7.4200087563266193,-7.3815878093835332,-7.3431684513780366,-7.30475074439921,-7.2663347529536066,-7.2279205440586889,-7.1895081873398112,
-7.1510977551308939,-7.1126893225789205,-7.0742829677523975,-7.0358787717539126,-6.9974768188369589,-6.9590771965271641,-6.9206799957480909,-6.8822853109517732,-6.843893240254153,-6.805503885575602,-6.7671173527867055,
-6.7287337518594859,-6.6903531970242787,-6.6519758069324384,-6.6136017048250926,-6.57523101870815,-6.5368638815337778,-6.4985004313885755,-6.46014081168867,-6.421785171381976,-6.3834336651578534,-6.3450864536644174,
-6.30674370373376,-6.2684055886153347,-6.2300722882177864,-6.19174398935949,-6.1534208860280923,-6.1151031796493323,-6.0767910793654432,-6.0384848023234454,-6.0001845739736153,-5.96189062837847,-5.9236032085325654,
-5.885322566693457,-5.8470489647241228,-5.8087826744472162,-5.7705239780114672,-5.7322731682705825,-5.6940305491750021,-5.6557964361768436,-5.6175711566484123,-5.5793550503146143,-5.5411484696996336,-5.5029517805882442,
-5.4647653625020896,-5.4265896091913088,-5.3884249291418449,-5.3502717460987892,-5.3121304996061092,-5.2740016455630858,-5.2358856567978,-5.19778302365797,-5.1596942546194668,-5.1216198769127752,-5.0835604371676935,
-5.0455165020765191,-5.00748865907596,-4.9694775170479843,-4.9314837070397806,-4.8935078830030063,-4.8555507225524295,-4.8176129277440465,-4.7796952258727385,-4.7417983702894473,-4.7039231412378326,-4.66607034671031,
-4.6282408233233046,-4.5904354372114993,-4.5526550849407981,-4.51490069443962,-4.4771732259481043,-4.4394736729846818,-4.4018030633294041,-4.3641624600233078,-4.3265529623829977,-4.2889757070295165,-4.2514318689304451,
-4.2139226624540695,-4.1764493424342994,-4.1390132052448942,-4.1016155898813986,-4.0642578790490438,-4.0269415002547042,-3.9896679269008142,-3.9524386793790134,-3.9152553261610512,-3.8781194848843388,-3.8410328234293076,
-3.803997060985552,-3.7670139691035036,-3.7300853727281966,-3.6932131512114434,-3.6563992392985356,-3.6196456280853515,-3.5829543659415348,-3.5463275593951851,-3.5097673739742703,-3.4732760349997669,-3.4368558283253021,
-3.4005091010178781,-3.3642382619740516,-3.3280457824657557,-3.2919341966097728,-3.2559061017547068,-3.2199641587791614,-3.1841110922947031,-3.1483496907470916,-3.1126828064091829,-3.0771133552588643,-3.0416443167353631,
-3.006278733367294,-2.971019710265864,-2.9358704144767453,-2.9008340741842717,-2.86591397776179,-2.831113472662238,-2.7964359641432832,-2.7618849138217114,-2.7274638380521163,-2.6931763061253942,-2.6590259382830341,
-2.6250164035437478,-2.5911514173395824,-2.557434738959325,-2.5238701687977123,-2.4904615454097292,-2.4572127423700914,-2.4241276649388612,-2.39121024653506,-2.35846444502106,-2.3258942388015313,-2.2935036227417047,
-2.2612966039107327,-2.2292771971569727,-2.1974494205230419,-2.16581729050955,-2.1343848171974207,-2.1031559992397515,-2.0721348187351136,-2.041325235995167,-2.0107311842203495,-1.9803565640982526,-1.9502052383400854,
-1.9202810261713301,-1.8905876977933434,-1.8611289688331909,-1.8319084947994693,-1.8029298655622221,-1.7741965998753064,-1.745712139959718,-1.7174798461664109,-1.6895029917370683,-1.6617847576810858,-1.6343282277867266,
-1.6071363837839714,-1.5802121006760688,-1.5535581422561435,-1.5271771568244767,-1.5010716731212381,-1.4752440964885178,-1.4496967052744916,-1.4244316474914656,-1.3994509377383972,-1.374756454397273,-1.3503499371114702,
-1.3262329845529344,-1.3024070524836864,-1.2788734521158318,-1.2556333487729121,-1.2326877608540876,-1.2100375591013315,-1.1876834661685067,-1.1656260564899321,-1.1438657564448223,-1.1224028448128038,-1.1012374535145928,
-1.0803695686308616,-1.0597990316913248,-1.0395255412251647,-1.0195486545630694,-0.999867789880393,-0.98048222847026834,-0.96139111723490445,-0.94259347138278338,-0.92408817731904136,-0.90587399571597127,-0.88794956475031261,
-0.87031340349381137,-0.85296391544341477,-0.83589939217743381,-0.81911801712403187,-0.80261786942850188,-0.78639692790594917,-0.77045307506621974,-0.75478410119818173,-0.73938770850078828,-0.72426151524871563,-0.70940305998077047,
-0.69480980569969863,-0.68047914407249543,-0.66640839962080678,-0.65259483389152828,-0.63903564959823322,-0.62572799472460483,-0.61266896658160053,-0.59985561581062685,-0.58728495032556371,-0.57495393918703064,-0.56285951640283871,
-0.550998584649115,-0.53936801890711938,-0.52796467001129754,-0.51678536810462083,-0.50582692599775569,-0.49508614242908494,-0.48455980522305742,-0.47424469434478389,-0.46413758484921669,-0.45423524972364809,-0.444534462622641,
-0.43503200049486335,-0.42572464610163108,-0.41660919042728112,-0.40768243498178697,-0.39894119399630262,-0.39038229651257189,-0.38200258836737178,-0.37379893407336928,-0.365768218597965,-0.35790734904187005,-0.35021325621931931},
new double[]{
-11.484227393300371,-11.44719095798305,-11.410154545368945,-11.373118156314638,-11.336081791709031,-11.299045452474559,-11.262009139568461,-11.224972853984088,-11.187936596752268,-11.150900368942718,-11.113864171665513,-11.076828006072606,
-11.039791873359414,-11.002755774766445,-10.965719711581011,-10.928683685138989,-10.891647696826649,-10.854611748082563,-10.817575840399572,-10.780539975326834,-10.743504154471948,-10.706468379503159,-10.669432652151643,
-10.632396974213888,-10.595361347554148,-10.558325774107003,-10.521290255880011,-10.484254794956462,-10.447219393498228,-10.410184053748742,-10.373148778036049,-10.336113568776019,-10.299078428475649,-10.262043359736502,
-10.225008365258269,-10.187973447842476,-10.150938610396318,-10.113903855936647,-10.076869187594102,-10.039834608617406,-10.002800122377815,-9.9657657323737361,-9.92873144223552,-9.8916972557304437,-9.854663176767863,
-9.8176292094045721,-9.7805953578503644,-9.7435616264737934,-9.7065280198081663,-9.6694945425577412,-9.6324611996041813,-9.59542799601324,-9.5583949370416939,-9.5213620281445426,-9.48432927498248,-9.44729668342965,
-9.41026425958168,-9.3732320097640347,-9.33619994054067,-9.2991680587230228,-9.262136371379329,-9.2251048858443,-9.1880736097291624,-9.1510425509320719,-9.1140117176489142,-9.0769811183845288,-9.0399507619643344,
-9.0029206575464027,-8.96589081463399,-8.92886124308852,-8.89183195314308,-8.8548029554164085,-8.8177742609274041,-8.7807458811101942,-8.74371782782976,-8.7066901133981442,-8.6696627505912787,-8.63263575266643,
-8.5956091333803091,-8.5585829070078567,-8.5215570883617371,-8.4845316928125474,-8.4475067363098049,-8.41048223540371,-8.3734582072677224,-8.3364346697219833,-8.2994116412576187,-8.2623891410619521,-8.2253671890446558,
-8.1883458058648753,-8.15132501295939,-8.11430483257179,-8.0772852877827841,-8.0402664025416,-8.0032482016985913,-7.9662307110390316,-7.92921395731819,-7.8921979682977028,-7.8551827727833077,-7.8181684006639731,
-7.7811548829524959,-7.7441422518276006,-7.7071305406776078,-7.670119784145724,-7.6331100181770184,-7.596101280067133,-7.5590936085128106,-7.5220870436642882,-7.4850816271796381,-7.4480774022811129,-7.4110744138135818,
-7.3740727083051238,-7.337072334029858,-7.3000733410730962,-7.2630757813988946,-7.2260797089200945,-7.1890851795709452,-7.1520922513823928,-7.1151009845601347,-7.0781114415655439,-7.0411236871995539,-7.0041377886896194,
-6.9671538157798567,-6.9301718408244746,-6.8931919388846188,-6.8562141878287459,-6.8192386684366388,-6.7822654645072209,-6.7452946629702657,-6.7083263540021614,-6.6713606311458644,-6.6343975914351825,-6.5974373355235372,
-6.560479967817364,-6.5235255966143031,-6.4865743342463453,-6.4496262972280984,-6.41268160641035,-6.375740387139099,-6.3388027694202469,-6.3018688880901212,-6.2649388829920394,-6.2280128991591,-6.1910910870034064,
-6.1541736025119373,-6.117260607449265,-6.080352269567352,-6.0434487628226448,-6.00655026760069,-5.9696569709485159,-5.9327690668150126,-5.8958867562995554,-5.85901024790913,-5.822139757824198,-5.7852755101735793,
-5.7484177373186078,-5.7115666801468254,-5.6747225883754977,-5.6378857208652153,-5.6010563459438814,-5.5642347417413411,-5.5274211965349727,-5.4906160091065042,-5.4538194891103622,-5.4170319574538439,-5.3802537466894034,
-5.3434852014193543,-5.3067266787132823,-5.2699785485384529,-5.2332411942035293,-5.1965150128158655,-5.1598004157526738,-5.1230978291463583,-5.0864076943842624,-5.0497304686231326,-5.0130666253185341,-4.9764166547694764,
-4.9397810646784963,-4.9031603807274076,-4.866555147168941,-4.829965927434464,-4.7933933047579487,-4.7568378828163507,-4.7203002863865136,-4.6837811620187217,-4.6472811787269466,-4.6108010286958532,-4.5743414280045585,
-4.5379031173671036,-4.5014868628895837,-4.4650934568437917,-4.4287237184572241,-4.3923784947192139,-4.356058661202912,-4.319765122902778,-4.2834988150871638,-4.24726070416551,-4.2110517885695984,-4.17487309964822,
-4.1387257025745328,-4.1026106972652832,-4.0665292193109872,-4.0304824409160291,-3.994471571847575,-3.9584978603920247,-3.9225625943176556,-3.8866671018419514,-3.8508127526019873,-3.8150009586261064,-3.7792331753049657,
-3.7435109023598883,-3.7078356848062954,-3.6722091139098367,-3.6366328281326536,-3.601108514067064,-3.56563790735375,-3.5302227935813781,-3.4948650091643763,-3.4595664421954191,-3.424329033268982,-3.3891547762721346,
-3.3540457191385569,-3.3190039645615803,-3.28403167066186,-3.2491310516051235,-3.2143043781652416,-3.1795539782277276,-3.1448822372285887,-3.1102915985233173,-3.0757845636806711,-3.0413636926957692,-3.0070316041169249,
-2.9727909750805486,-2.9386445412483879,-2.9045950966413363,-2.870645493364008,-2.8367986412143038,-2.80305750717222,-2.7694251147622366,-2.7359045432837124,-2.7024989269038771,-2.6692114536081792,-2.6360453640029804,
-2.6030039499658435,-2.5700905531389897,-2.5373085632618366,-2.5046614163389389,-2.4721525926401107,-2.4397856145299897,-2.4075640441248529,-2.3754914807750938,-2.3435715583723824,-2.311807942481237,-2.280204327295432,
-2.2487644324204412,-2.2174919994839106,-2.1863907885769782,-2.1554645745301158,-2.1247171430280578,-2.0941522865692623,-2.0637738002762798,-2.0335854775643081,-2.0035911056761342,-1.9737944610925691,-1.9441993048283843,
-1.9148093776246151,-1.885628395048953,-1.8566600425167428,-1.8279079702458665,-1.799375788159493,-1.7710670607513275,-1.7429853019285639,-1.7151339698482553,-1.6875164617632361,-1.660136108894068,-1.6329961713437378,
-1.6060998330719725,-1.5794501969461141,-1.5530502798854293,-1.5269030081156017,-1.5010112125498922,-1.4753776243131123,-1.4500048704240953,-1.4248954696518121,-1.4000518285596229,-1.3754762377514369,-1.3511708683327193,
-1.3271377685984029,-1.3033788609587778,-1.2798959391134102,-1.2566906654820487,-1.2337645689003307,-1.2111190425869352,-1.1887553423876127,-1.1666745853002973,-1.1448777482842665,-1.1233656673550703,-1.1021390369657218,
-1.0811984096734149,-1.0605441960898459,-1.0401766651120539,-1.0200959444295694,-1.0003020213025919,-0.98079474360488939,-0.96157382112415779,-0.94263882711167835,-0.92398920007227925,-0.90562424578485579,-0.88754313954301778,
-0.86974492860482866,-0.85222853484007266,-0.8349927575630367,-0.818036276538423,-0.80135765514771651,-0.78495534370310871,-0.76882768289594317,-0.75297290736657019,-0.73738914938249822,-0.72207444261179132,-0.70702672597878691,
-0.69224384758938884,-0.6777235687134292,-0.66346356781187565,-0.64946144459699284,-0.63571472411393781,-0.62222086083267647,-0.60897724273954612,-0.59598119541825723,-0.58322998611061427,-0.57072082774774391,-0.558450882943141,
-0.54641726793937517,-0.53461705650083946,-0.5230472837454675,-0.51170494990888671,-0.500587024035018,-0.48969044758766395,-0.4790121379781585,-0.4685489920046641,-0.45829788919920816,-0.44825569507904034,-0.4384192642993659,
-0.4287854437049668,-0.41935107527866294,-0.41011299898498216,-0.40106805550781027,-0.39221308888116929,-0.38354494901263086,-0.37506049409920944,-0.36675659293589458,-0.35863012711727665,-0.3506779931329953,-0.34289710435799037},
new double[]{
-11.149387224995376,-11.113673671817571,-11.077960145273828,-11.042246646332496,-11.006533175997131,-10.970819735307778,-10.935106325342284,-10.899392947217688,-10.863679602091635,-10.827966291163859,-10.792253015677707,-10.756539776921729,
-10.720826576231316,-10.68511341499041,-10.649400294633262,-10.613687216646259,-10.577974182569832,-10.542261194000403,-10.506548252592433,-10.470835360060525,-10.435122518181611,-10.39940972879722,-10.36369699381582,
-10.327984315215261,-10.292271695045281,-10.256559135430138,-10.220846638571295,-10.185134206750252,-10.14942184233143,-10.113709547765202,-10.077997325591007,-10.042285178440586,-10.006573109041348,-9.9708611202198263,
-9.9351492149053,-9.8994373961335143,-9.8637256670505522,-9.8280140309168491,-9.7923024911113341,-9.7565910511357519,-9.7208797146191124,-9.6851684853223148,-9.6494573671429471,-9.6137463641202423,-9.5780354804402315,
-9.5423247204410657,-9.5066140886185515,-9.4709035896318774,-9.4351932283095348,-9.3994830096554836,-9.3637729388555133,-9.32806302128385,-9.29235326251,-9.2566436683058377,-9.220934244652959,-9.18522499775029,
-9.1495159340219789,-9.113807060125577,-9.0780983829605084,-9.0423899096768459,-9.006681647684422,-8.9709736046622375,-8.9352657885682536,-8.899558207649493,-8.86385087045255,-8.8281437858344471,-8.7924369629739,
-8.756730411382998,-8.7210241409192868,-8.6853181617983086,-8.6496124846065818,-8.6139071203150586,-8.5782020802930656,-8.5424973763227516,-8.506793020614051,-8.4710890258201985,-8.4353854050537986,-8.3996821719034713,
-8.3639793404511114,-8.3282769252897584,-8.2925749415421226,-8.25687340487977,-8.2211723315430039,-8.185471738361473,-8.1497716427755069,-8.114072062858229,-8.0783730173384729,-8.0426745256245109,-8.0069766078286442,
-7.9712792847926881,-7.93558257811436,-7.8998865101746238,-7.8641911041660162,-7.8284963841219986,-7.7928023749473541,-7.757109102449685,-7.7214165933720365,-7.6857248754266934,-7.6500339773301933,-7.61434392883959,
-7.5786547607900214,-7.542966505133621,-7.5072791949798239,-7.4715928646371177,-7.4359075496562843,-7.4002232868751907,-7.3645401144651732,-7.3288580719790914,-7.2931772004010824,-7.2574975421980987,-7.22181914137328,
-7.186142043521226,-7.1504662958852334,-7.1147919474165748,-7.0791190488358735,-7.0434476526966643,-7.0077778134512076,-6.9721095875186325,-6.9364430333554914,-6.9007782115288165,-6.8651151847917449,-6.8294540181618251,
-6.7937947790020745,-6.7581375371048926,-6.72248236477893,-6.6868293369390006,-6.65117853119915,-6.61553002796899,-6.5798839105533924,-6.5442402652556719,-6.5085991814843656,-6.4729607518637291,-6.4373250723480773,
-6.40169224234009,-6.3660623648132146,-6.330435546438312,-6.2948118977146548,-6.25919153310546,-6.2235745711780615,-6.1879611347489032,-6.1523513510334915,-6.1167453518014661,-6.0811432735369539,-6.0455452576043758,
-6.0099514504198712,-5.9743620036285137,-5.9387770742875032,-5.9031968250555193,-5.8676214243884068,-5.8320510467414124,-5.7964858727781436,-5.7609260895864605,-5.72537189090151,-5.6898234773361009,-5.6542810566186388,
-5.6187448438388321,-5.5832150617013951,-5.54769194078797,-5.5121757198274874,-5.4766666459752109,-5.4411649751006825,-5.40567097208482,-5.3701849111263957,-5.3347070760581383,-5.2992377606727068,-5.2637772690587754,
-5.2283259159474769,-5.1928840270694518,-5.1574519395227476,-5.1220300021518153,-5.0866185759378464,-5.0512180344006987,-5.0158287640126442,-4.9804511646241778,-4.945085649902131,-4.9097326477802969,-4.874392600922814,
-4.8390659672005016,-4.8037532201803668,-4.7684548496284753,-4.733171362026372,-4.6979032811012233,-4.6626511483698456,-4.6274155236967562,-4.5921969858663756,-4.5569961331694948,-4.52181358400408,-4.4866499774904858,
-4.4515059741011109,-4.4163822563045034,-4.3812795292238844,-4.3461985213100549,-4.3111399850285625,-4.2761046975610251,-4.2410934615204132,-4.2061071056800872,-4.1711464857162994,-4.1362124849638695,-4.1013060151846146,
-4.0664280173481249,-4.03157946242437,-3.9967613521875509,-3.9619747200305744,-3.9272206317894023,-3.8925001865764859,-3.8578145176223821,-3.8231647931245751,-3.7885522171024149,-3.7539780302569921,-3.7194435108346662,
-3.684949975492839,-3.650498780166461,-3.6160913209336352,-3.5817290348785513,-3.5474134009498584,-3.5131459408124517,-3.4789282196904949,-3.4447618471993806,-3.4106484781641568,-3.3765898134218211,-3.3425876006047135,
-3.3086436349020927,-3.2747597597968214,-3.24093786777392,-3.2071799009976036,-3.1734878519532441,-3.1398637640505451,-3.1063097321840725,-3.0728279032471137,-3.0394204765947075,-3.0060897044515333,-2.9728378922602214,
-2.9396673989655131,-2.9065806372295939,-2.873580073573804,-2.8406682284418516,-2.8078476761795783,-2.7751210449262564,-2.7424910164123695,-2.7099603256588027,-2.67753176057238,-2.645208161432695,-2.6129924202652677,
-2.5808874800961075,-2.5488963340829085,-2.5170220245182251,-2.485267641700172,-2.4536363226663953,-2.4221312497873231,-2.39075564921499,-2.3595127891840577,-2.3284059781620261,-2.2974385628460361,-2.2666139260041183,
-2.2359354841592265,-2.2054066851149363,-2.175031005322241,-2.1448119470875047,-2.114753035622257,-2.0848578159362043,-2.0551298495755326,-2.0255727112093171,-1.9961899850676137,-1.9669852612355867,-1.9379621318088312,
-1.9091241869158477,-1.8804750106144528,-1.8520181766697115,-1.8237572442218033,-1.7956957533530142,-1.7678372205638384,-1.7401851341689261,-1.7127429496243309,-1.6855140847982031,-1.6585019151977094,-1.6317097691655484,
-1.6051409230599674,-1.5787985964326403,-1.552685947219181,-1.5268060669573706,-1.5011619760484356,-1.4757566190768636,-1.4505928602043245,-1.4256734786532428,-1.4010011642954632,-1.3765785133612549,-1.352408024283605,
-1.3284920936923688,-1.3048330125723788,-1.2814329625990457,-1.2582940126643456,-1.2354181156053654,-1.2128071051467719,-1.1904626930677102,-1.1683864666026975,-1.1465798860850962,-1.1250442828407059,-1.1037808573379437,
-1.0827906775999636,-1.0620746778829309,-1.04163365762352,-1.0214682806575373,-1.0015790747104145,-0.98196643115917148,-0.96263060506430731,-0.94357171546898044,-0.92478974596175689,-0.90628454549817494,-0.888055829475385,
-0.87010318105318485,-0.8524260527138946,-0.835023768052692,-0.81789552378928254,-0.8010403919910899,-0.78445732249754208,-0.76814514553448854,-0.752102574507319,-0.73632820896096052,-0.72082053769461729,-0.70557794201886959,
-0.69059869914258,-0.67588098567695365,-0.66142288124406368,-0.64722237217718881,-0.63327735530039786,-0.61958564177497322,-0.60614496100046811,-0.5929529645584487,-0.58000723018727907,-0.56730526577664664,-0.55484451337091356,
-0.54262235317079166,-0.53063610752328838,-0.51888304489033521,-0.50736038378700776,-0.496065296680747,-0.4849949138435159,-0.47414632714935218,-0.46351659381031124,-0.45310274004433021,-0.44290176466907838,-0.43291064261638906,
-0.42312632836239228,-0.41354575926898191,-0.40416585883275458,-0.39498353983804574,-0.38599570741116579,-0.377199261973395,-0.36859110209073737,-0.36016812721885622,-0.35192724034201384,-0.34386535050522243,-0.33597937523917115},
new double[]{
-10.835352978888331,-10.800871101401654,-10.766389254827841,-10.731907440251373,-10.697425658794773,-10.662943911619944,-10.628462199929544,-10.593980524968421,-10.559498888025091,-10.525017290433269,-10.490535733573456,-10.456054218874574,
-10.421572747815674,-10.387091321927686,-10.35260994279524,-10.318128612058555,-10.283647331415377,-10.249166102623013,-10.214684927500402,-10.180203807930289,-10.145722745861454,-10.111241743311037,-10.076760802366929,
-10.042279925190249,-10.007799114017923,-9.9733183711653339,-9.9388376990290723,-9.9043571000897881,-9.86987657691514,-9.8353961321628312,-9.800915768583792,-9.7664354890254224,-9.7319552964349914,-9.69747519386313,
-9.6629951844674622,-9.6285152715163473,-9.5940354583927636,-9.5595557485983349,-9.5250761457574811,-9.4905966536217221,-9.4561172760741385,-9.4216380171339775,-9.387158880961433,-9.3526798718625752,-9.3182009942944717,
-9.2837222528704739,-9.2492436523657009,-9.2147651977227,-9.1802868940573177,-9.1458087466647733,-9.1113307610259433,-9.0768529428138613,-9.0423752979004632,-9.0078978323635361,-8.9734205524939519,-8.9389434648031134,
-8.9044665760306856,-8.8699898931526,-8.8355134233893082,-8.8010371742143736,-8.766561153363309,-8.7320853688427658,-8.6976098289400259,-8.66313454223282,-8.6286595175995036,-8.5941847642295812,-8.5597102916345964,
-8.5252361096594,-8.4907622284938231,-8.4562886586847466,-8.4218154111485983,-8.3873424971842763,-8.3528699284865446,-8.3183977171598649,-8.2839258757327414,-8.249454417172549,-8.2149833549008768,-8.18051270280942,
-8.14604247527641,-8.111572687183628,-8.077103353934012,-8.0426344914698582,-8.0081661162916848,-7.9736982454777356,-7.9392308967041547,-7.9047640882658845,-7.8702978390982725,-7.8358321687994366,-7.8013670976534089,
-7.766902646654076,-7.7324388375299646,-7.6979756927698686,-7.6635132356493845,-7.6290514902583553,-7.5945904815292691,-7.5601302352666426,-7.5256707781774246,-7.4912121379024494,-7.4567543430489796,-7.422297423224375,
-7.3878414090709281,-7.3533863323019,-7.3189322257388048,-7.2844791233499775,-7.2500270602904786,-7.2155760729433656,-7.1811261989623993,-7.14667747731621,-7.1122299483339919,-7.0777836537527623,-7.0433386367662534,
-7.0088949420754751,-6.9744526159410221,-6.9400117062371685,-6.9055722625078158,-6.8711343360243635,-6.8366979798455532,-6.8022632488793606,-6.7678301999469976,-6.7333988918491068,-6.698969385434201,-6.6645417436694414,
-6.6301160317138157,-6.59569231699381,-6.5612706692816429,-6.5268511607761495,-6.4924338661864107,-6.4580188628182018,-6.4236062306633652,-6.38919605249219,-6.35478841394891,-6.3203834036504034,-6.2859811132882086,
-6.2515816377339588,-6.2171850751483451,-6.18279152709371,-6.148401098650405,-6.1140138985370127,-6.0796300392345612,-6.045249637114857,-6.0108728125730595,-5.9764996901646361,-5.9421303987468139,-5.9077650716246914,
-5.8734038467021277,-5.8390468666375686,-5.8046942790049467,-5.7703462364598215,-5.7360028969108923,-5.7016644236970633,-5.6673309857702092,-5.6330027578838155,-5.5986799207876494,-5.5643626614286479,-5.5300511731581841,
-5.4957456559459041,-5.4614463166003011,-5.4271533689962226,-5.39286703430949,-5.3585875412588333,-5.3243151263553123,-5.2900500341594432,-5.2557925175462108,-5.221542837978177,-5.187301265786882,-5.1530680804627407,
-5.1188435709536435,-5.08462803597246,-5.0504217843136647,-5.0162251351792726,-4.9820384185143087,-4.947861975352005,-4.9136961581689382,-4.8795413312503042,-4.845397871065531,-4.8112661666544358,-4.7771466200241059,
-4.7430396465567055,-4.7089456754283878,-4.6748651500394889,-4.6407985284561777,-4.606746283863723,-4.5727089050315328,-4.53868689679011,-4.50468078052005,-4.4706910946532119,-4.4367183951861549,-4.4027632562059349,
-4.3688262704283352,-4.3349080497485737,-4.3010092258045329,-4.2671304505525027,-4.2332723968554378,-4.1994357590836682,-4.1656212537280082,-4.1318296200251421,-4.0980616205951614,-4.0643180420910685,-4.0305996958600385,
-3.9969074186161766,-3.9632420731244773,-3.9296045488956226,-3.8959957628912294,-3.8624166602390764,-3.8288682149578093,-3.795351430690534,-3.7618673414466639,-3.7284170123513056,-3.6950015404013969,-3.6616220552277396,
-3.6282797198619807,-3.594975731507513,-3.5617113223131831,-3.5284877601485913,-3.4953063493796792,-3.4621684316431947,-3.429075386618524,-3.3960286327952636,-3.3630296282347949,-3.3300798713240192,-3.2971809015192632,
-3.2643343000782763,-3.2315416907780863,-3.1988047406163633,-3.1661251604938148,-3.1335047058749863,-3.1009451774247281,-3.0684484216174304,-3.0360163313160151,-3.0036508463175191,-2.9713539538619815,-2.9391276891012135,
-2.9069741355238872,-2.87489542533328,-2.8428937397738623,-2.8109713094028161,-2.7791304143024691,-2.7473733842295118,-2.7157025986967946,-2.6841204869834079,-2.6526295280686885,-2.621232250485749,-2.5899312320900707,
-2.558729099738708,-2.5276285288756264,-2.4966322430187251,-2.4657430131441473,-2.4349636569635211,-2.4042970380898896,-2.3737460650881879,-2.3433136904062795,-2.31300290918273,-2.2828167579277161,-2.2527583130736861,
-2.2228306893926777,-2.1930370382774917,-2.1633805458842592,-2.13386443113433,-2.10449194357379,-2.0752663610893975,-2.0461909874801689,-2.01726914988439,-1.9885041960623537,-1.9598994915357131,-1.9314584165849376,
-1.9031843631070049,-1.8750807313361084,-1.8471509264308488,-1.81939835493207,-1.7918264210962256,-1.7644385231098665,-1.7372380491915924,-1.7102283735885204,-1.6834128524750678,-1.6567948197625497,-1.6303775828288079,
-1.604164418177757,-1.5781585670394005,-1.5523632309214817,-1.5267815671245304,-1.5014166842325962,-1.4762716375924558,-1.4513494247945158,-1.426652981169005,-1.4021851753113623,-1.3779488046509616,-1.3539465910774851,
-1.3301811766393383,-1.3066551193285192,-1.2833708889662661,-1.2603308632036641,-1.2375373236511444,-1.2149924521504825,-1.1926983272024989,-1.1706569205631756,-1.1488700940203258,-1.1273395963623241,-1.1060670605496721,
-1.0850540010994054,-1.0643018116914924,-1.0438117630054795,-1.0235850007946847,-1.0036225442042477,-0.98392528433831472,-0.96449398308058032,-0.94532927217133012,-0.92643165254303761,-0.90780149391547615,-0.88943903465021379,
-0.87134438186328311,-0.853517511793752,-0.8359582704248939,-0.81866637435364731,-0.801641411903099,-0.78488284447180423,-0.76839000811289337,-0.7521621153351038,-0.73619825711712772,-0.720497405125976,-0.70505841412944292,
-0.68988002459219988,-0.67496086544457079,-0.6602994570126266,-0.645894214097902,-0.631743449194768,-0.61784537583329746,-0.6041981120353368,-0.59079968387142934,-0.57764802910624846,-0.56474100092025759,-0.55207637169544443,
-0.53965183685315421,-0.52746501873227891,-0.51551347049634211,-0.50379468005833827,-0.4923060740125525,-0.48104502156298129,-0.47000883843840757,-0.4591947907846361,-0.44860009902487613,-0.438221941679754,-0.4280574591389491,
-0.41810375737697092,-0.40835791160612062,-0.3988169698602157,-0.38947795650318739,-0.38033787565719052,-0.37139371454539061,-0.36264244674510743,-0.35408103534750113,-0.34570643602047935,-0.33751559997198149,-0.32950547681125986},
new double[]{
-10.540208523571575,-10.506876238554442,-10.473543989068787,-10.440211776318861,-10.406879601549727,-10.373547466048645,-10.3402153711465,-10.306883318219278,-10.273551308689596,-10.240219344028285,-10.206887425756015,-10.173555555444995,
-10.140223734720712,-10.106891965263733,-10.073560248811585,-10.040228587160666,-10.006896982168254,-9.9735654357545638,-9.9402339499048828,-9.9069025266717681,-9.8735711681773353,-9.8402398766156036,-9.8069086542549435,
-9.7735775034405883,-9.7402464265972384,-9.7069154262317543,-9.6735845049359419,-9.6402536653894249,-9.6069229103626217,-9.5735922427198243,-9.5402616654223742,-9.5069311815319448,-9.4736007942139526,-9.4402705067410562,
-9.4069403224968,-9.3736102449793588,-9.3402802778054319,-9.3069504247142447,-9.27362068957171,-9.240291076374703,-9.2069615892555134,-9.1736322324864137,-9.140303010484411,-9.1069739278161421,-9.07364498920294,
-9.040316199526071,-9.0069875638321477,-8.9736590873387332,-8.9403307754401187,-8.9070026337133079,-8.873674667924206,-8.84034688403401,-8.8070192882058187,-8.7736918868114664,-8.74036468643858,-8.7070376938978935,
-8.6737109162307888,-8.6403843607171,-8.6070580348831847,-8.573731946510259,-8.5404061036430239,-8.5070805145985791,-8.4737551879756285,-8.4404301326640141,-8.4071053578545563,-8.3737808730492311,-8.3404566880716988,
-8.3071328130781747,-8.2738092585686687,-8.24048603539862,-8.2071631547908979,-8.1738406283482288,-8.1405184680660323,-8.10719668634569,-8.07387529600826,-8.0405543103086714,-8.0072337429503531,-7.9739136081004132,
-7.94059392040528,-7.9072746950068948,-7.8739559475594456,-7.8406376942466576,-7.80731995179967,-7.7740027375155138,-7.7406860692762081,-7.7073699655685,-7.6740544455042636,-7.6407395288415909,-7.6074252360065842,
-7.5741115881158825,-7.5407986069999424,-7.5074863152270988,-7.4741747361284361,-7.4408638938234892,-7.4075538132468024,-7.3742445201753837,-7.3409360412570752,-7.3076284040398685,-7.2743216370022079,-7.2410157695842967,
-7.20771083222046,-7.1744068563725758,-7.1411038745646334,-7.1078019204184333,-7.0745010286904852,-7.0412012353101305,-7.0079025774189381,-6.9746050934114,-6.9413088229769961,-6.9080138071436448,-6.8747200883225963,
-6.8414277103548242,-6.8081367185589441,-6.7748471597807285,-6.7415590824442537,-6.7082725366047447,-6.6749875740031595,-6.6417042481225854,-6.6084226142464813,-6.5751427295188538,-6.5418646530064066,-6.5085884457627348,
-6.4753141708946291,-6.4420418936305586,-6.4087716813913911,-6.3755036038634412,-6.3422377330738948,-6.3089741434687054,-6.27571291199303,-6.2424541181742814,-6.2091978442078908,-6.1759441750458484,-6.1426931984881179,
-6.1094450052770179,-6.0761996891946461,-6.0429573471634557,-6.00971807935007,-5.976481989272437,-5.9432491839104262,-5.9100197738199656,-5.876793873250838,-5.8435716002682279,-5.8103530768781448,-5.7771384291568335,
-5.7439277873842842,-5.7107212861819665,-5.6775190646549136,-5.6443212665382712,-5.6111280403484551,-5.5779395395390283,-5.5447559226614569,-5.511577353530857,-5.47840400139689,-5.445236041119939,-5.4120736533527145,
-5.3789170247274409,-5.3457663480487643,-5.3126218224925443,-5.2794836538106837,-5.2463520545421485,-5.2132272442303407,-5.1801094496469933,-5.1469989050227412,-5.11389585228454,-5.0808005413001034,-5.0477132301295189,
-5.014634185284228,-4.9815636819935278,-4.9485020044787769,-4.9154494462354759,-4.8824063103233994,-4.8493729096649449,-4.8163495673518861,-4.78333661696069,-4.75033440287658,-4.7173432806265074,-4.6843636172212078,
-4.6513957915064941,-4.6184401945239673,-4.5854972298812831,-4.5525673141321477,-4.5196508771661748,-4.486748362608755,-4.4538602282310737,-4.4209869463704043,-4.3881290043607883,-4.3552869049742284,-4.3224611668724808,
-4.2896523250695351,-4.2568609314048587,-4.22408755502747,-4.1913327828908757,-4.1585972202589,-4.1258814912224224,-4.0931862392270011,-4.0605121276113607,-4.0278598401566681,-3.9952300816465325,-3.9626235784376087,
-3.930041079040659,-3.8974833547119108,-3.8649512000544983,-3.8324454336297422,-3.7999668985779933,-3.767516463248707,-3.7350950218393884,-3.702703495042992,-3.6703428307033064,-3.6380140044778164,-3.6057180205074628,
-3.5734559120926694,-3.5412287423749489,-3.5090376050233245,-3.4768836249247421,-3.4447679588775775,-3.4126917962872585,-3.3806563598629573,-3.3486629063142161,-3.3167127270462853,-3.284807148852869,-3.2529475346048766,
-3.221135283933688,-3.1893718339073356,-3.1576586596979115,-3.1259972752384013,-3.0943892338670387,-3.0628361289571666,-3.0313395945304782,-2.9999013058514019,-2.968522980000273,-2.9372063764228282,-2.9059532974534297,
-2.8747655888093253,-2.8436451400531211,-2.8125938850205348,-2.7816138022103813,-2.7507069151336392,-2.7198752926183212,-2.6891210490667921,-2.6584463446620545,-2.6278533855194479,-2.59734442378011,-2.5669217576424774,
-2.5365877313280238,-2.5063447349773926,-2.4761952044730147,-2.4461416211842768,-2.4161865116312917,-2.3863324470633005,-2.3565820429477666,-2.3269379583662406,-2.2974028953131302,-2.2679795978935751,-2.2386708514167317,
-2.2094794813808729,-2.1804083523468649,-2.1514603666967469,-2.1226384632743294,-2.0939456159049561,-2.0653848317918309,-2.0369591497865787,-2.008671638532042,-1.9805253944756447,-1.952523539752024,-1.9246692199340609,
-1.8969656016518419,-1.8694158700795749,-1.842023226290965,-1.8147908844840772,-1.7877220690772631,-1.7608200116782966,-1.7340879479294644,-1.7075291142319566,-1.6811467443535473,-1.6549440659241872,-1.6289242968247857,
-1.60309064147512,-1.5774462870274697,-1.5519943994732268,-1.5267381196703911,-1.5016805593004909,-1.4768247967640962,-1.4521738730246838,-1.4277307874111946,-1.4034984933901489,-1.3794798943186977,-1.3556778391904383,
-1.3320951183862315,-1.3087344594426131,-1.2855985228506921,-1.262689897898662,-1.24001109857122,-1.2175645595192934,-1.1953526321134953,-1.173377580594696,-1.1516415783349632,-1.1301467042219333,-1.1088949391794005,
-1.0878881628365509,-1.0671281503578445,-1.0466165694450469,-1.0263549775223242,-1.006344819114688,-0.98658742342934869,-0.96708400214877988,-0.94783564744345938,-0.9288433302113811,-0.91010789855051011,-0.89163007646939363,
-0.87341046284015111,-0.85544953059705631,-0.83774762618288945,-0.82030496924420426,-0.80312165257561063,-0.7861976423121414,-0.76953277836774958,-0.75312677511698234,-0.73697922231590263,-0.72108958625739228,-0.70545721115506588,
-0.69008132074917294,-0.67496102012705939,-0.66009529775000908,-0.64548302767759613,-0.63112297198005052,-0.61701378332857271,-0.60315400775303984,-0.589542087556115,-0.57617636437241426,-0.56305508236109458,-0.55017639152000708,
-0.53753835110940784,-0.52513893317313176,-0.51297602614511628,-0.501047438529203,-0.48935090264024644,-0.47788407839471708,-0.46664455713919845,-0.45562986550543605,-0.44483746928090356,-0.43426477728419777,-0.42390914523495921,
-0.413767879608431,-0.40383824146521657,-0.39411745024726585,-0.38460268753161092,-0.37529110073387972,-0.3661798067541352,-0.35726589555811616,-0.34854643368748844,-0.34001846769324984,-0.33167902748696593,-0.32352512960504037},
new double[]{
-10.262268726433671,-10.230011896691117,-10.197755107427666,-10.16549835997027,-10.133241655689377,-10.100984996000356,-10.06872838236497,-10.036471816292893,-10.004215299343281,-9.9719588331264,-9.9397024193052914,-9.9074460595975076,
-9.875189755776896,-9.8429335096754436,-9.8106773231851836,-9.7784211982601619,-9.74616513691847,-9.7139091412443452,-9.6816532133903284,-9.6493973555795165,-9.6171415701078651,-9.5848858593465742,-9.5526302257445632,
-9.5203746718310072,-9.4881192002179713,-9.4558638136031217,-9.423608514772539,-9.391353306603607,-9.3590981920680036,-9.3268431742347939,-9.294588256273622,-9.2623334414579954,-9.2300787331687,-9.1978241348973047,
-9.165569650249795,-9.1333152829503188,-9.1010610368450564,-9.068806915906217,-9.0365529242361671,-9.0042990660716917,-8.9720453457884,-8.9397917679052643,-8.9075383370893242,-8.8752850581605252,-8.8430319360967289,
-8.81077897603889,-8.7785261832963855,-8.746273563352533,-8.71402112187029,-8.6817688646981246,-8.6495167978761,-8.61726492764213,-8.5850132604384743,-8.5527618029184111,-8.5205105619531523,-8.4882595446389661,
-8.4560087583045522,-8.4237582105186437,-8.3915079090978555,-8.3592578621148022,-8.32700807790647,-8.2947585650828639,-8.2625093325359362,-8.2302603894488211,-8.1980117453053367,-8.1657634098998422,-8.1335153933473734,
-8.10126770609414,-8.0690203589283449,-8.036773362991374,-8.0045267297893368,-7.9722804712049848,-7.9400345995100245,-7.9077891273778311,-7.875544067896568,-7.8432994345827414,-7.8110552413951968,-7.7788115027495639,
-7.7465682335331758,-7.7143254491204782,-7.6820831653889314,-7.6498413987354317,-7.6176001660932746,-7.585359484949656,-7.5531193733637512,-7.5208798499853824,-7.4886409340742821,-7.4564026455199945,-7.4241650048624139,
-7.3919280333129906,-7.3596917527766292,-7.3274561858742926,-7.295221355966337,-7.2629872871766095,-7.2307540044173155,-7.198521533414703,-7.16628990073557,-7.1340591338146275,-7.1018292609827558,-7.0696003114961634,
-7.0373723155664942,-7.005145304391907,-6.97291931018915,-6.940694366226678,-6.9084705068588326,-6.8762477675611242,-6.8440261849666459,-6.8118057969036672,-6.7795866424344267,-6.7473687618951761,-6.7151521969375114,
-6.6829369905710223,-6.65072318720732,-6.6185108327054625,-6.5862999744188482,-6.5540906612435936,-6.5218829436684711,-6.4896768738264337,-6.4574725055477815,-6.425269894415031,-6.3930690978195219,-6.3608701750198318,
-6.3286731872020434,-6.2964781975419264,-6.2642852712690891,-6.2320944757331649,-6.1999058804720892,-6.1677195572825383,-6.1355355802925864,-6.1033540260366568,-6.0711749735328286,-6.0389985043625769,-6.0068247027530131,
-5.9746536556617045,-5.9424854528641484,-5.9103201870439772,-5.8781579538859789,-5.8459988521720136,-5.8138429838799146,-5.7816904542854521,-5.74954137206746,-5.7173958494162092,-5.6852540021451263,-5.6531159498059473,
-5.620981815807415,-5.5888517275376035,-5.556725816489994,-5.5246042183933879,-5.4924870733457754,-5.4603745259522629,-5.428266725467183,-5.3961638259404783,-5.3640659863685061,-5.3319733708493517,-5.2998861487427975,
-5.2678044948350529,-5.2357285895083834,-5.2036586189157541,-5.1715947751606288,-5.1395372564820505,-5.1074862674451378,-5.0754420191371352,-5.0434047293691489,-5.0113746228837162,-4.979351931568341,-4.9473368946751419,
-4.91532975904676,-4.8833307793486576,-4.8513402183079721,-4.8193583469590564,-4.7873854448958628,-4.7554218005313107,-4.7234677113637895,-4.6915234842509461,-4.6595894356908989,-4.6276658921110245,-4.59575319016447,
-4.5638516770345268,-4.5319617107470034,-4.5000836604907555,-4.4682179069464789,-4.4363648426239353,-4.4045248722077037,-4.3726984129116095,-4.3408858948419331,-4.3090877613695255,-4.277304469510927,-4.24553649031859,
-4.2137843092803058,-4.1820484267279143,-4.150329358255358,-4.118627635146165,-4.0869438048103914,-4.0552784312310681,-4.0236320954201785,-3.9920053958841653,-3.9603989490989684,-3.9288133899945583,-3.8972493724489228,
-3.86570756979144,-3.8341886753155441,-3.8026934028005752,-3.7712224870426687,-3.7397766843945175,-3.7083567733138132,-3.6769635549201332,-3.6455978535600129,-3.6142605173799049,-3.582952418906689,-3.5516744556353563,
-3.5204275506234493,-3.4892126530917928,-3.458030739031003,-3.4268828118132184,-3.3957699028084289,-3.3646930720047434,-3.3336534086318612,-3.3026520317869585,-3.2716900910621431,-3.2407687671725496,-3.2098892725840931,
-3.1790528521398183,-3.1482607836837078,-3.1175143786807333,-3.0868149828318643,-3.0561639766826416,-3.0255627762238664,-2.9950128334828467,-2.9645156371035553,-2.9340727129139714,-2.90368562447877,-2.8733559736354315,
-2.8430854010117548,-2.8128755865226371,-2.7827282498439097,-2.7526451508608916,-2.7226280900892479,-2.6926789090656182,-2.6627994907053938,-2.6329917596249195,-2.6032576824253009,-2.5735992679349042,-2.5440185674075471,
-2.5145176746732911,-2.4850987262386677,-2.4557639013330914,-2.4265154218981504,-2.3973555525163941,-2.3682866002762033,-2.3393109145692592,-2.3104308868171217,-2.2816489501233894,-2.2529675788479171,-2.2243892880995633,
-2.1959166331439732,-2.1675522087229306,-2.1392986482818817,-2.111158623102285,-2.0831348413355717,-2.0552300469355793,-2.027447018486487,-1.9997885679234269,-1.97225753914314,-1.9448568065022509,-1.9175892732009683,
-1.8904578695502923,-1.8634655511210805,-1.8366152967736622,-1.8099101065670105,-1.7833529995468729,-1.7569470114126424,-1.7306951920631843,-1.7046006030222758,-1.6786663147447944,-1.6528954038052852,-1.6272909499710528,
-1.6018560331624656,-1.5765937303037132,-1.5515071120678312,-1.5265992395203833,-1.5018731606667952,-1.4773319069089195,-1.452978489417023,-1.4288158954239814,-1.4048470844490588,-1.3810749844592412,-1.3575024879766524,
-1.3341324481411441,-1.3109676747376651,-1.288010930198529,-1.2652649255911528,-1.2427323166022766,-1.2204156995300552,-1.1983176072957531,-1.1764405054870621,-1.1547867884452967,-1.1333587754088879,-1.1121587067257168,
-1.0911887401468707,-1.0704509472143833,-1.0499473097554284,-1.02967971649528,-1.0096499598011097,-0.98985973256839144,-0.9703106252613044,-0.95100412311807814,-0.93194160353170286,-0.91312433361584922,-0.894553467965192,
-0.87623004661862725,-0.8581549932331094,-0.840329113475022,-0.82275309363514038,-0.80542749947234282,-0.78835277529030012,-0.77152924325041294,-0.75495710292328888,-0.73863643108006227,-0.72256718172385814,-0.70674918636071038,
-0.69118215450825382,-0.67586567443953538,-0.66079921415834175,-0.64598212260151477,-0.6314136310628391,-0.61709285483223875,-0.603018795043214,-0.58919034072070142,-0.57560627102083961,-0.56226525765348656,-0.5491658674777542,
-0.5363065652603175,-0.5236857165858031,-0.51130159090818872,-0.49915236473182739,-0.48723612491047319,-0.47555087205250657,-0.46409452402044932,-0.45286491951281677,-0.44185982171637456,-0.43107692201694608,-0.42051384375705869,
-0.41016814602890506,-0.40003732749134419,-0.39011883019995564,-0.38041004343949592,-0.37090830754848053,-0.36161091772602311,-0.35251512781150496,-0.34361815402811308,-0.33491717868177379,-0.3264093538075184,-0.31809180475583476},
new double[]{
-10.000045398899216,-9.9687968399812235,-9.9375483268059082,-9.9062998608251576,-9.8750514435369379,-9.8438030764867577,-9.8125547612691761,-9.7813064995293519,-9.7500582929646633,-9.7188101433263512,-9.6875620524212351,-9.6563140221134756,
-9.6250660543263944,-9.5938181510443457,-9.5625703143146623,-9.5313225462496458,-9.5000748490286284,-9.4688272249001066,-9.4375796761839243,-9.4063322052735447,-9.3750848146383827,-9.3438375068262172,-9.31259028446567,
-9.2813431502687767,-9.2500961070336327,-9.2188491576471154,-9.1876023050877134,-9.1563555524284155,-9.1251089028397256,-9.093862359592741,-9.0626159260623531,-9.03136960573053,-9.0001234021897236,-8.96887731914636,
-8.9376313604244686,-8.9063855299693948,-8.8751398318516657,-8.84389427027094,-8.81264884956012,-8.7814035741895626,-8.7501584487714457,-8.7189134780642643,-8.6876686669774639,-8.65642402057623,-8.6251795440864214,
-8.593935242899672,-8.5626911225786344,-8.5314471888624084,-8.50020344767213,-8.46895990511674,-8.4377165674989438,-8.4064734413213351,-8.37523053329275,-8.34398785033479,-8.31274539958856,-8.2815031884216346,
-8.2502612244352278,-8.21901951547159,-8.1877780696216522,-8.15653689523289,-8.1252960009174622,-8.09405539556059,-8.0628150883291951,-8.031575088680837,-8.000335406372896,-7.969096051472083,-7.9378570343642219,
-7.9066183657643538,-7.8753800567271615,-7.8441421186577109,-7.8129045633225482,-7.7816674028611326,-7.7504306497976385,-7.7191943170531259,-7.6879584179580931,-7.6567229662654332,-7.6254879761637868,-7.5942534622913254,
-7.5630194397499659,-7.5317859241200278,-7.5005529314753607,-7.4693204783989415,-7.4380885819989633,-7.4068572599254328,-7.3756265303872892,-7.3443964121700569,-7.3131669246540589,-7.2819380878332005,-7.2507099223343392,
-7.2194824494372707,-7.1882556910953337,-7.1570296699566649,-7.1258044093861246,-7.0945799334879007,-7.0633562671288308,-7.0321334359624457,-7.0009114664537746,-6.9696903859049195,-6.9384702224814347,-6.9072510052395266,
-6.87603276415411,-6.8448155301477316,-6.8135993351204078,-6.782384211980383,-6.7511701946758542,-6.7199573182276851,-6.6887456187631305,-6.6575351335506214,-6.6263259010356279,-6.5951179608776354,-6.5639113539882725,
-6.5327061225706231,-6.5015023101597542,-6.47029996166451,-6.4390991234105854,-6.4078998431849534,-6.3767021702816455,-6.3455061555489634,-6.3143118514381422,-6.2831193120535138,-6.2519285932042195,-6.220739752457515,
-6.1895528491937135,-6.1583679446628219,-6.1271851020429065,-6.0960043865002627,-6.0648258652514144,-6.0336496076270212,-6.00247568513773,-5.9713041715420445,-5.9401351429162581,-5.9089686777265182,-5.877804856903083,
-5.84664376391683,-5.815485484858085,-5.7843301085178371,-5.75317772647141,-5.7220284331646551,-5.6908823260027424,-5.6597395054416211,-5.6286000750822263,-5.5974641417675066,-5.5663318156823536,-5.5352032104565154,
-5.5040784432705712,-5.4729576349650575,-5.4418409101528331,-5.4107283973347586,-5.379620229018804,-5.3485165418426472,-5.3174174766998856,-5.2863231788699387,-5.2552337981517434,-5.2241494890013431,-5.19307041067348,
-5.1619967273672742,-5.1309286083761165,-5.0998662282418676,-5.0688097669134811,-5.0377594099101524,-5.0067153484891183,-4.9756777798182057,-4.9446469071532606,-4.9136229400205584,-4.8826060944043341,-4.8515965929395275,
-4.8205946651098879,-4.7896005474515455,-4.7586144837621758,-4.727636725315886,-4.6966675310839419,-4.6657071679614655,-4.6347559110002212,-4.6038140436476276,-4.5728818579921109,-4.5419596550149288,-4.5110477448485939,
-4.4801464470420092,-4.4492560908324528,-4.4183770154245243,-4.3875095702761735,-4.3566541153919314,-4.3258110216234584,-4.2949806709775213,-4.264163456931505,-4.23335978475657,-4.2025700718485508,-4.1717947480666933,
-4.1410342560803191,-4.1102890517235053,-4.0795596043578515,-4.0488463972434054,-4.0181499279178094,-3.9874707085837198,-3.9568092665045387,-3.926166144408497,-3.895541900901097,-3.8649371108859327,-3.8343523659938747,
-3.8037882750205991,-3.7732454643724251,-3.742724578520404,-3.7122262804625876,-3.6817512521943829,-3.6513001951868751,-3.6208738308729913,-3.59047290114133,-3.5600981688374822,-3.5297504182726205,-3.4994304557391174,
-3.469139110032915,-3.4388772329823438,-3.4086456999830386,-3.3784454105385842,-3.3482772888064631,-3.3181422841488559,-3.288041371687783,-3.2579755528640497,-3.2279458559993959,-3.1979533368612061,-3.1679990792290877,
-3.1380841954625653,-3.1082098270690786,-3.0783771452714306,-3.0485873515737421,-3.0188416783249359,-2.9891413892786813,-2.9594877801486779,-2.9298821791580782,-2.9003259475817815,-2.8708204802802486,-2.8413672062234148,
-2.8119675890031988,-2.7826231273330229,-2.7533353555326778,-2.724105843996778,-2.6949361996449732,-2.6658280663519931,-2.6367831253555125,-2.6078030956397407,-2.5788897342925496,-2.5500448368338655,-2.52127023751297,
-2.4925678095722592,-2.4639394654749389,-2.435387157094044,-2.4069128758600944,-2.3785186528646234,-2.3502065589167471,-2.3219787045498763,-2.2938372399756037,-2.2657843549817591,-2.2378222787715694,-2.2099532797408172,
-2.182179665189877,-2.1545037809674685,-2.1269280110429727,-2.0994547770041505,-2.0720865374771251,-2.0448257874655069,-2.0176750576056017,-1.990636913334682,-1.9637139539694013,-1.9369088116914996,-1.9102241504380872,
-1.8836626646939081,-1.8572270781831508,-1.8309201424585417,-1.8047446353856591,-1.7787033595206216,-1.752799140379556,-1.7270348245985068,-1.7014132779827524,-1.6759373834448001,-1.6506100388306759,-1.625434154634491,
-1.6004126516016479,-1.5755484582214672,-1.5508445081104412,-1.5263037372877781,-1.501929081345373,-1.4777234725148269,-1.4536898366346542,-1.429831090021322,-1.4061501362483149,-1.3826498628379482,-1.3593331378712066,
-1.3362028065214295,-1.3132616875182228,-1.2905125695485153,-1.2679582076022207,-1.2456013192704905,-1.2234445810050558,-1.2014906243476442,-1.1797420321389269,-1.1582013347168862,-1.1368710061148999,-1.1157534602702044,
-1.0948510472537265,-1.0741660495325505,-1.0537006782765186,-1.0334570697206436,-1.0134372815951249,-0.993643289634832,-0.97407698418010669,-0.9547401668806822,-0.93563454751437536,-0.91676174093202034,-0.89812326413984,
-0.87972053353012281,-0.86155486227066824,-0.8436274578630043,-0.82593941987884356,-0.80849173788365736,-0.79128528955559474,-0.77432083900726811,-0.7575990353171691,-0.74112041127667694,-0.72488538235777555,-0.70889424590571493,
-0.69314718055994529,-0.67764424590571493,-0.66238538235777555,-0.64737041127667694,-0.6325990353171691,-0.61807083900726811,-0.60378528955559474,-0.58974173788365736,-0.57593941987884356,-0.5623774578630043,-0.54905486227066824,
-0.53597053353012281,-0.52312326413984,-0.51051174093202034,-0.49813454751437536,-0.4859901668806822,-0.47407698418010669,-0.46239328963483189,-0.450937281595125,-0.43970706972064361,-0.42870067827651864,-0.41791604953255035,
-0.40735104725372651,-0.39700346027020439,-0.38687100611489994,-0.37695133471688635,-0.367242032138927,-0.3577406243476442,-0.34844458100505571,-0.33935131927049045,-0.33045820760222061,-0.32176256954851529,-0.31326168751822281},
new double[]{
-9.7522191254823216,-9.7219177628827254,-9.6916164515903631,-9.6613151931836256,-9.6310139892894551,-9.6007128415848371,-9.5704117517983427,-9.5401107217117112,-9.5098097531614911,-9.4795088480407141,-9.4492080083006442,-9.4189072359525632,
-9.3886065330696127,-9.3583059017887056,-9.3280053443124746,-9.2977048629113064,-9.2674044599254142,-9.2371041377669876,-9.206803898922411,-9.1765037459545376,-9.1462036815050443,-9.1159037082968553,-9.0856038291366339,
-9.05530404691737,-9.025004364621017,-8.9947047853212414,-8.964405312186237,-8.9341059484816245,-8.9038066975734562,-8.8735075629312981,-8.84320854813141,-8.8129096568600289,-8.7826108929167486,-8.752312260218,
-8.722013762800648,-8.6917154048256844,-8.6614171905820516,-8.63111912449057,-8.60082121110799,-8.5705234551311662,-8.5402258614013746,-8.5099284349087352,-8.4796311807967921,-8.4493341043672316,-8.4190372110847278,
-8.388740506581966,-8.35844399666479,-8.3281476873175269,-8.2978515847084733,-8.2675556951955382,-8.23726002533207,-8.2069645818728638,-8.1766693717803385,-8.1463744022309168,-8.1160796806216,-8.0857852145767328,
-8.0554910119549952,-8.02519708085658,-7.9949034296306252,-7.964610066882841,-7.934317001483393,-7.9040242425750176,-7.8737317995813836,-7.8434396822157186,-7.8131479004896907,-7.7828564647225642,-7.7525653855506409,
-7.7222746739369823,-7.691984341181433,-7.6616943989309512,-7.6314048591902548,-7.601115734332792,-7.5708270371120445,-7.5405387806731836,-7.5102509785650735,-7.4799636447526492,-7.4496767936296662,-7.4193904400318411,
-7.3891045992503974,-7.3588192870460167,-7.3285345196632239,-7.2982503138452053,-7.2679666868490775,-7.2376836564616305,-7.20740124101554,-7.1771194594060779,-7.1468383311083326,-7.1165578761949559,-7.08627811535444,
-7.0559990699099613,-7.0257207618387953,-6.9954432137923126,-6.9651664491165963,-6.9348904918736753,-6.9046153668634069,-6.8743410996460206,-6.8440677165653518,-6.8137952447727708,-6.7835237122518475,-6.753253147843755,
-6.7229835812734464,-6.6927150431766229,-6.662447565127521,-6.6321811796675307,-6.6019159203346938,-6.5716518216940756,-6.5413889193690675,-6.5111272500736259,-6.4808668516454873,-6.4506077630803809,-6.4203500245672762,
-6.3900936775246819,-6.3598387646380505,-6.3295853298982907,-6.29933341864145,-6.2690830775895794,-6.2388343548928278,-6.2085873001727911,-6.1783419645671689,-6.1480984007757442,-6.11785666310775,-6.0876168075306349,
-6.0573788917202984,-6.0271429751128105,-5.996909118957678,-5.9666773863726865,-5.9364478424003826,-5.90622055406622,-5.8759955904384347,-5.84577302268969,-5.8155529241605457,-5.7853353704247965,-5.7551204393567419,
-5.7249082112004306,-5.6946987686409454,-5.6644921968777808,-5.6342885837003642,-5.6040880195657969,-5.573890597678858,-5.5436964140743505,-5.5135055677018361,-5.4833181605128409,-5.4531342975505837,-5.422954087042311,
-5.3927776404942938,-5.3626050727895667,-5.3324365022884832,-5.3022720509321513,-5.2721118443488431,-5.2419560119634365,-5.2118046871099848,-5.1816580071474876,-5.1515161135789445,-5.1213791521737777,-5.0912472730937122,
-5.0611206310221979,-5.0309993852974584,-5.0008837000492683,-4.970773744339537,-4.940669692306801,-4.9105717233147237,-4.8804800221046838,-4.8503947789525617,-4.8203161898298283,-4.7902444565690132,-4.7601797870336817,
-4.7301223952930034,-4.7000725018010225,-4.670030333580736,-4.6399961244130825,-4.6099701150309462,-4.579952553318285,-4.5499436945144947,-4.5199438014241045,-4.4899531446319187,-4.4599720027237186,-4.4300006625126178,
-4.4000394192711916,-4.3700885769694748,-4.3401484485189457,-4.3102193560225928,-4.280301631031171,-4.2503956148057433,-4.220501658586624,-4.1906201238687952,-4.16075138268392,-4.1308958178890212,-4.10105382346193,
-4.0712258048035777,-4.0414121790472262,-4.0116133753746945,-3.9818298353396688,-3.9520620131981494,-3.9223103762461027,-3.8925754051643615,-3.8628575943708219,-3.8331574523799756,-3.8034755021697957,-3.7738122815560007,
-3.744168343573703,-3.7145442568664278,-3.6849406060824985,-3.6553579922787471,-3.6257970333315068,-3.5962583643548327,-3.566742638125866,-3.5372505255172495,-3.5077827159364885,-3.4783399177721104,-3.4489228588464846,
-3.4195322868751141,-3.3901689699322026,-3.3608336969222741,-3.331527278057584,-3.30225054534105,-3.2730043530543838,-3.2437895782510808,-3.2146071212538989,-3.1854579061564,-3.1563428813281225,-3.12726301992288,
-3.0982193203896689,-3.06921280698561,-3.0402445302903054,-3.0113155677209495,-2.9824270240474862,-2.9535800319070411,-2.9247757523168234,-2.8960153751846209,-2.8673001198159644,-2.838631235416972,-2.8100100015918348,
-2.7814377288338248,-2.7529157590086584,-2.7244454658289725,-2.6960282553186032,-2.6676655662652919,-2.6393588706603666,-2.6111096741238775,-2.582919516313587,-2.5547899713161506,-2.5267226480187319,-2.4987191904592363,
-2.4707812781532597,-2.4429106263957747,-2.4151089865355093,-2.3873781462198753,-2.359719929608258,-2.3321361975513821,-2.3046288477344095,-2.2771998147813477,-2.2498510703182948,-2.2225846229929673,-2.1954025184479109,
-2.1683068392447336,-2.1412997047366606,-2.1143832708866555,-2.0875597300283228,-2.0608313105667881,-2.0342002766167089,-2.0076689275745818,-1.9812395976225015,-1.9549146551605259,-1.9286965021648503,-1.9025875734689952,
-1.8765903359652876,-1.8507072877239617,-1.8249409570272872,-1.7992939013162272,-1.7737687060472382,-1.7483679834569497,-1.723094371232615,-1.6979505310863805,-1.6729391472316189,-1.6480629247597642,-1.6233245879163287,
-1.5987268782750181,-1.5742725528091397,-1.549964381859785,-1.5258051470005873,-1.5017976387991827,-1.477944654475867,-1.4542489954603135,-1.4307134648476132,-1.4073408647553174,-1.3841339935835948,-1.3610956431810668,
-1.3382285959193467,-1.315535621679792,-1.2930194747564623,-1.2706828906797789,-1.2485285829658843,-1.2265592397972036,-1.2047775206402263,-1.1831860528070253,-1.1617874279675331,-1.1405841986200827,-1.1195788745282003,
-1.0987739191320929,-1.0781717459437148,-1.0577747149347085,-1.0375851289269016,-1.0176052299953895,-0.99783719589455,-0.97828313651761167,-0.95894509040061748,-0.93982502128181777,-0.92092481472764565,-0.90224627483650888,
-0.88379112103164637,-0.86556098495425693,-0.847557407468006,-0.8297818357858473,-0.81223562072986866,-0.79492001413457725,-0.77783616640368181,-0.76098512423000542,-0.74436782848768346,-0.72798511230525387,-0.71183769932764562,
-0.69592620217441536,-0.68025112110087371,-0.66481284286798614,-0.649611639826136,-0.63464766921699911,-0.61992097269691315,-0.6054314760842282,-0.59117898933220869,-0.57716320672812882,-0.56338370731826581,-0.54983995555755583,
-0.53653130218174749,-0.52345698529896556,-0.51061613169669939,-0.49800775835935129,-0.48563077419064038,-0.47348398193434732,-0.46156608028612278,-0.44987566618836511,-0.43841123729950993,-0.42717119462846437,-0.41615384532437161,
-0.40535740561140454,-0.39478000385786666,-0.38441968376852237,-0.37427440768879244,-0.36434206000922942,-0.35462045065853603,-0.34510731867330413,-0.33580033583263136,-0.3266971103458165,-0.31779519058143996,-0.30909206882629792}

    };

    #endregion

    #region ImaginaryPart
    /// <summary>
    /// This field is the logarithm of (im/beta) for beta=0 to 1+1/32 (steps=1/32)
    /// and for y =-5 to 0.5 in steps of 1/64 with w=Exp(y/beta) -&gt; y = beta*ln(w).
    /// im is the imaginary part of the fourier transformed first derivative of the Kohlrausch function.
    /// (in Mathematica notation): Im[Integrate[D[Exp[-t^beta],t]*Exp[-I w t],{t, 0, Infinity}]].
    /// </summary>

    static double[][] _imdata = new double[][]{
new double[]{
-142.96157639728716,-140.676268635914,-138.42663353055315,-136.21211802793485,-134.03217764908573,-131.88627635639631,-129.77388642275008,-127.69448830268105,-125.64757050552915,-123.63262947056194,-121.64916944403251,-119.69670235814343,
-117.77474771188709,-115.88283245373351,-114.02049086613695,-112.18726445183293,-110.38270182189821,-108.60635858554615,-106.85779724163069,-105.13658707183232,-103.44230403550014,-101.77453066612419,-100.13285596941287,
-98.516875322950412,-96.926190377410236,-95.360408959299562,-93.819144975212126,-92.302018317565071,-90.808654771797421,-89.3386859250072,-87.891749076005055,-86.467487146762409,-85.065548595232357,-83.685587329522292,
-82.327262623397061,-80.990239033092124,-79.674186315416534,-78.37877934712543,-77.103698045542643,-75.848627290413958,-74.613256846971879,-73.397281290193149,-72.2003999302306,-71.022316739001084,-69.862740277911456,
-68.721383626705162,-67.597964313411936,-66.49220424538349,-65.4038296413984,-64.332570964819709,-63.278162857788644,-62.240344076438738,-61.218857427114244,-60.213449703577481,-59.223871625189616,-58.249877776049885,
-57.29122654507843,-56.34768006702798,-55.41900416441009,-54.504968290321791,-53.605345472158533,-52.719912256199827,-51.848448653054028,-50.990738083948884,-50.146567327854783,-49.315726469427773,-48.49800884775965,
-47.693211005922635,-46.90113264129625,-46.121576556664287,-45.354348612070027,-44.599257677417789,-43.856115585809363,-43.124737087603911,-42.404939805190068,-41.696544188459256,-40.99937347096931,-40.313253626787748,
-39.638013328004078,-38.973483902900888,-38.31949929477333,-37.67589602138716,-37.042513135065249,-36.419192183392916,-35.80577717053248,-35.202114519137538,-34.608053032857775,-34.023443859425,-33.448140454311542,
-32.881998544952062,-32.32487609552,-31.7766332722502,-31.23713240929909,-30.706237975134165,-30.183816539444585,-29.669736740564737,-29.163869253402858,-28.6660867578669,-28.17626390777988,-27.694277300277164,
-27.220005445678208,-26.753328737825377,-26.294129424882641,-25.842291580586981,-25.397701075945484,-24.960245551371287,-24.529814389251449,-24.106298686940161,-23.689591230170645,-23.279586466879262,-22.876180481435441,
-22.479270969271134,-22.088757211903609,-21.704540052345475,-21.326521870895945,-20.954606561307408,-20.588699507321522,-20.228707559569063,-19.87453901282792,-19.52610358363367,-19.183312388237272,-18.846077920904484,
-18.514314032551731,-18.187935909713172,-17.866860053833889,-17.551004260884064,-17.240287601289232,-16.934630400171688,-16.633954217898214,-16.338181830929379,-16.047237212965765,-15.761045516386478,-15.479533053975429,
-15.202627280930923,-14.930256777154167,-14.662351229812375,-14.398841416172186,-14.139659186699253,-13.884737448419822,-13.634010148540288,-13.387412258320689,-13.144879757198234,-12.906349617156959,-12.671759787339711,
-12.441049178898716,-12.214157650080999,-11.991025991545035,-11.771595911905063,-11.555810023499488,-11.343611828379942,-11.134945704517561,-10.929756892223109,-10.727991480777639,-10.529596395270437,-10.334519383641,
-10.142709003921949,-9.95411461167968,-9.768686347649755,-9.5863751255639862,-9.4071326201662355,-9.2309112554140178,-9.05766419286302,-8.8873453202317,-8.7199092401431777,-8.555311259041666,-8.393507376280736,
-8.2344542733807486,-8.0781093034528464,-7.9244304807869,-7.7733764706008808,-7.6249065789491564,-7.478980742787245,-7.3355595201905981,-7.1946040807250311,-7.0560761959664395,-6.9199382301675,-6.7861531310690708,
-6.6546844208540419,-6.5254961872414405,-6.3985530747186026,-6.2738202759092818,-6.1512635230755839,-6.030849079751647,-5.9125437325070314,-5.7963147828378094,-5.6821300391833711,-5.5699578090669908,-5.459766891358254,
-5.3515265686554372,-5.2452065997859885,-5.1407772124232807,-5.0382090958178285,-4.937473393641195,-4.8385416969408528,-4.741386037204256,-4.6459788795304577,-4.55229311590758,-4.4603020585945172,-4.3699794336052369,
-4.2812993742941048,-4.1942364150406579,-4.1087654850322828,-4.0248619021432885,-3.9425013669088771,-3.8616599565925309,-3.7823141193453864,-3.7044406684561522,-3.6280167766901759,-3.5530199707162757,-3.4794281256199753,
-3.4072194595018042,-3.3363725281593424,-3.2668662198517122,-3.1986797501452413,-3.1317926568390342,-3.0661847949692156,-3.001836331890626,-2.9387277424347715,-2.8768398041428362,-2.8161535925726056,-2.7566504766781481,
-2.6983121142611246,-2.641120447492626,-2.5850576985044285,-2.53010636504861,-2.4762492162244509,-2.4234692882715874,-2.3717498804283812,-2.3210745508545054,-2.2714271126167409,-2.2227916297370056,-2.1751524133016598,
-2.1284940176311222,-2.0828012365088804,-2.0380590994689554,-1.9942528681409322,-1.9513680326516492,-1.9093903080826775,-1.8683056309827208,-1.8281001559340866,-1.7887602521723864,-1.7502725002586477,-1.7126236888030144,
-1.6758008112392453,-1.6397910626492187,-1.6045818366366678,-1.5701607222493834,-1.5365155009491369,-1.5036341436285763,-1.4715048076743733,-1.4401158340759011,-1.4094557445787388,-1.3795132388823062,-1.3502771918809444,
-1.3217366509477717,-1.2938808332606466,-1.2666991231695903,-1.240181069605022,-1.2143163835261763,-1.1890949354090799,-1.164506752773472,-1.1405420177480685,-1.1171910646735703,-1.094444377742837,-1.0722925886776431,
-1.0507264744414546,-1.0297369549876663,-1.0093150910427462,-0.98945208192375367,-0.97013926338969014,-0.951368105526163,-0.93313021066284507,-0.9154173113232198,-0.8982212682061137,-0.881534068198522,-0.86534782241924169,
-0.84965476429283693,-0.83444724765346323,-0.81971774487808979,-0.80545884504866394,-0.79166325214276756,-0.778323783252326,-0.76543336682993235,-0.75298504096236019,-0.74097195167084373,-0.72938735123770837,-0.71822459655894577,
-0.70747714752232915,-0.69713856541067332,-0.68720251132985077,-0.67766274466117837,-0.66851312153779807,-0.65974759334467825,-0.65136020524187188,-0.64334509471066792,-0.63569649012228358,-0.62840870932874648,-0.6214761582756233,
-0.61489332963625587,-0.608654801467172,-0.60275523588434154,-0.59718937775995562,-0.59195205343941071,-0.58703816947818288,-0.58244271139828663,-0.5781607424640115,-0.57418740247664024,-0.57051790658785273,-0.56714754413152579,
-0.56407167747364573,-0.56128574088004957,-0.55878523940172153,-0.55656574777737144,-0.55462290935302594,-0.552952435018371,-0.55155010215958478,-0.55041175362840455,-0.54953329672717788,-0.54891070220964777,-0.54854000329723085,
-0.54841729471054512,-0.54853873171595358,-0.54890052918688925,-0.54949896067973347,-0.55033035752402093,-0.55139110792675,-0.55267765609057928,-0.55418650134569558,-0.55591419729514058,-0.55785735097338829,-0.56001262201796764,
-0.56237672185392751,-0.56494641289094549,-0.5677185077328849,-0.57068986839960589,-0.57385740556084119,-0.57721807778195,-0.58076889078136518,-0.58450689669955247,-0.58842919337930344,-0.59253292365718691,-0.59681527466598494,
-0.60127347714794355,-0.605904804778671,-0.61070657350151736,-0.61567614087227407,-0.62081090541403294,-0.6261083059820467,-0.63156582113843718,-0.63718096853659711,-0.64295130431513625,-0.6488744225012234,-0.65494795442317855},
new double[]{
-74.976304641085235,-74.476304641085235,-73.976304641085235,-73.476304641085235,-72.976304641085235,-72.476304641085235,-71.976304641085235,-71.476304641085235,-70.976304641085235,-70.476304641085235,-69.976304641085235,-69.476304641085235,
-68.976304641085235,-68.476304641085235,-67.976304641085235,-67.476304641085235,-66.976304641085235,-66.476304641085235,-65.976304641085235,-65.476304641085235,-64.976304641085235,-64.476304641085235,-63.976304641085235,
-63.476304641085235,-62.976304641085235,-62.476304641085243,-61.976304641085243,-61.476304641085257,-60.976304641085292,-60.476304641085385,-59.976304641085591,-59.476304641086081,-58.976304641087239,-58.476304641089889,
-57.976304641095894,-57.476304641109273,-56.976304641138611,-56.476304641201921,-55.976304641336277,-55.476304641616814,-54.97630464219295,-54.476304643356841,-53.976304645669636,-53.476304650190571,-52.976304658884381,
-52.476304675332642,-51.976304705952231,-51.476304762043739,-50.976304863170625,-50.476305042629889,-49.976305356143577,-49.476305895409993,-48.976306808837322,-48.476308332673355,-47.976310836866695,-47.476314891356743,
-46.976321360073641,-46.476331531678632,-45.976347297883777,-45.476371391889295,-44.976407700833178,-44.476461666863621,-43.976540791173974,-43.47665525371594,-42.976818657987764,-42.477048905023729,-41.97736919338309,
-41.4778091326682,-40.978405947286774,-40.479205735513169,-39.980264737391849,-39.481650554881014,-38.983443260174973,-38.485736324617896,-37.988637302010353,-37.49226820693886,-36.996765540957824,-36.502279936299935,
-36.008975406954391,-35.517028218648811,-35.026625410492549,-34.5379630198364,-34.051244076640579,-33.56667644320666,-33.08447057903777,-32.604837308997993,-32.127985666544852,-31.654120873695227,-31.183442506841356,
-30.716142883866073,-30.252405694391392,-29.792404882377308,-29.336303779335786,-28.88425447752314,-28.436397425757253,-27.99286122590857,-27.553762605429284,-27.119206540222198,-26.689286502382323,-26.264084808543952,
-25.84367304643364,-25.428112559507007,-25.0174549720233,-24.611742739424155,-24.211009711312766,-23.815281696597662,-23.424577022421637,-23.038907080317916,-22.65827685461554,-22.282685429461296,-21.912126471951638,
-21.546588689794351,-21.186056262669506,-20.83050924705466,-20.479923954743278,-20.134273305637869,-19.793527155660062,-19.457652600804419,-19.126614258486516,-18.80037452741038,-18.478893827216684,-18.162130819179545,
-17.85004260920374,-17.542584934341424,-17.239712334002633,-16.941378306981097,-16.647535455358625,-16.35813561629049,-16.073129982612141,-15.792469213145834,-15.516103533525257,-15.243982828297742,-14.976056725007723,
-14.712274670911823,-14.452586002925896,-14.196940011357194,-13.945285997930878,-13.697573328579203,-13.453751481423662,-13.213770090345268,-12.977578984505717,-12.745128224152253,-12.516368133011506,-12.291249327552333,
-12.069722743374438,-11.851739658958238,-11.637251716991925,-11.426210943473807,-11.218569764771567,-11.014281022805161,-10.813297988506257,-10.615574373694656,-10.421064341500543,-10.229722515450943,-10.041503987329156,
-9.856364323907048,-9.6742595726420859,-9.4951462664235873,-9.31898142744586,-9.1457225702797711,-8.9753277042085262,-8.80775533488831,-8.6429644653895785,-8.4809145966705071,-8.3215657275299719,-8.1648783540838323,
-8.01081346880486,-7.859332559163529,-7.7103976059040242,-7.5639710809872156,-7.4200159452298839,-7.2784956456672667,-7.1393741126639361,-7.0026157567961409,-6.8681854655269605,-6.7360485996940564,-6.60617098982828,
-6.4785189323200516,-6.3530591854491405,-6.2297589652923264,-6.1085859415223176,-5.9895082331103238,-5.8724944039437377,-5.7575134583695453,-5.6445348366732651,-5.533528410502516,-5.4244644782436024,-5.3173137603588945,
-5.2120473946922,-5.10863693174876,-5.0070543299560271,-4.9072719509109008,-4.809262554618666,-4.7129992947284709,-4.6184557137698317,-4.5256057383942716,-4.4344236746259051,-4.344884203124459,-4.2569623744639662,
-4.1706336044300807,-4.0858736693387421,-4.0026587013786923,-3.9209651839801172,-3.8407699472115238,-3.7620501632067516,-3.6847833416238713,-3.6089473251375517,-3.5345202849663377,-3.4614807164361436,-3.3898074345811393,
-3.3194795697830859,-3.2504765634500741,-3.1827781637355113,-3.1163644212981025,-3.0512156851034971,-2.9873125982681734,-2.9246360939460678,-2.8631673912583731,-2.8028879912668812,-2.7437796729911574,-2.6858244894698031,
-2.6290047638659861,-2.5733030856173893,-2.5187023066306624,-2.4651855375204308,-2.4127361438928725,-2.3613377426738378,-2.3109741984814427,-2.2616296200430548,-2.2132883566565384,-2.1659349946956157,-2.119554354159161,
-2.0741314852642407,-2.0296516650826657,-1.9861003942208269,-1.9434633935425454,-1.9017266009346727,-1.8608761681151416,-1.8208984574831675,-1.781780039011281,-1.7435076871788633,-1.7060683779468451,-1.6694492857732168,
-1.6336377806689955,-1.5986214252942799,-1.5643879720940221,-1.5309253604731352,-1.498221714010554,-1.4662653377118582,-1.4350447153000636,-1.4045485065441867,-1.3747655446251799,-1.3456848335388341,-1.3172955455352478,
-1.2895870185944505,-1.2625487539377778,-1.2361704135745864,-1.2104418178839018,-1.1853529432305907,-1.1608939196156456,-1.1370550283601786,-1.1138266998227135,-1.0911995111493709,-1.0691641840565425,-1.0477115826456498,
-1.0268327112495861,-1.0065187123104429,-0.9867608642881216,-0.96755057959943747,-0.94887940258731984,-0.93073900751972194,-0.9131211966178493,-0.89601789811332488,-0.87942116433390694,-0.86332316981738133,-0.84771620945325377,
-0.832592696651867,-0.81794516154057606,-0.80376624918661355,-0.79004871784628428,-0.77678543724012783,-0.7639693868536952,-0.75159365426358682,-0.73965143348840279,-0.72813602336426031,-0.71704082594453789,-0.70635934492350738,
-0.69608518408352016,-0.686212045765417,-0.67673372936183451,-0.6676441298330863,-0.65893723624529665,-0.65060713033047457,-0.64264798506821286,-0.63505406328870673,-0.62781971629678446,-0.62093938251665171,-0.61440758615705016,
-0.60821893589653775,-0.6023681235886007,-0.596849922986309,-0.59165918848623578,-0.58679085389135777,-0.58223993119266348,-0.57800150936919548,-0.57407075320625989,-0.570442902131535,-0.56711326906882054,-0.56407723930916653,
-0.56133026939912867,-0.55886788604589765,-0.556685685039055,-0.55477933018870929,-0.55314455227977322,-0.55177714804214117,-0.55067297913653368,-0.54982797115577586,-0.54923811264128153,-0.54889945411451835,-0.54880810712322947,
-0.54896024330219428,-0.54935209344831115,-0.54997994660978777,-0.5508401491892313,-0.551929104060428,-0.55324326969861026,-0.55477915932400668,-0.55653334005848054,-0.55850243209505557,-0.56068310788014009,-0.56307209130825686,
-0.56566615692909139,-0.56846212916667382,-0.57145688155051166,-0.57464733595849382,-0.57803046187138907,-0.58160327563876291,-0.58536283975614267,-0.58930626215325921,-0.59343069549320016,-0.59773333648230731,-0.60221142519065807,
-0.60686224438297043,-0.61168311885977222,-0.61667141480868182,-0.62182453916564451,-0.62713993898597653,-0.63261510082506489,-0.63824755012857948,-0.64403485063205135,-0.64997460376967531,-0.65606444809219744,-0.66230205869374814},
new double[]{
-46.555551171679546,-46.305551171679546,-46.055551171679546,-45.805551171679546,-45.555551171679546,-45.305551171679546,-45.055551171679546,-44.805551171679546,-44.555551171679546,-44.305551171679546,-44.055551171679546,-43.805551171679546,
-43.555551171679546,-43.305551171679546,-43.055551171679546,-42.805551171679546,-42.555551171679546,-42.305551171679546,-42.055551171679546,-41.805551171679546,-41.555551171679546,-41.305551171679546,-41.055551171679546,
-40.805551171679546,-40.555551171679546,-40.305551171679546,-40.055551171679546,-39.805551171679546,-39.555551171679546,-39.305551171679546,-39.055551171679546,-38.805551171679546,-38.555551171679546,-38.305551171679546,
-38.055551171679546,-37.805551171679546,-37.555551171679546,-37.305551171679546,-37.055551171679554,-36.805551171679554,-36.555551171679561,-36.305551171679575,-36.055551171679589,-35.805551171679618,-35.55555117167966,
-35.305551171679738,-35.055551171679859,-34.805551171680065,-34.5555511716804,-34.305551171680953,-34.055551171681863,-33.805551171683362,-33.555551171685828,-33.305551171689885,-33.055551171696557,-32.805551171707513,
-32.5555511717255,-32.305551171754971,-32.055551171803195,-31.805551171881973,-31.555551172010389,-31.305551172219211,-31.055551172557863,-30.805551173105368,-30.555551173987485,-30.305551175403341,-30.055551177666466,
-29.805551181267649,-29.555551186970359,-29.3055511959545,-29.055551210031098,-28.805551231960095,-28.555551265916595,-28.305551318168558,-28.055551398052536,-27.805551519365004,-27.555551702326881,-27.305551976329731,
-27.0555523837359,-26.8055529850832,-26.5555538661393,-26.305555147362856,-26.055556996457678,-25.805559644852071,-25.555563409095509,-25.305568718334182,-25.055576149199325,-24.805586469607814,-24.555600693121274,
-24.305620145622751,-24.0556465461316,-23.805682103568163,-23.555729631180206,-23.305792680133219,-23.05587569342935,-22.805984180842021,-22.556124914928315,-22.30630614741122,-22.056537844321429,-21.806831937277334,
-21.557202587200106,-21.3076664556563,-21.058242977953796,-20.808954631154677,-20.559827189381881,-20.310889958253981,-20.062175980045335,-19.813722201285145,-19.565569595006661,-19.317763230742905,-19.070352286616377,
-18.823389999441513,-18.576933550579248,-18.331043887263537,-18.085785481157806,-17.841226027887888,-17.59743609313346,-17.35448871245034,-17.112458953266806,-16.871423448397721,-16.63145991092431,-16.392646640395284,
-16.155062030039314,-15.91878408408259,-15.683889953395777,-15.450455496618009,-15.218554872691614,-14.988260169457828,-14.759641071673185,-14.532764570562083,-14.307694715866262,-14.084492410318187,-13.863215245573501,
-13.643917377897887,-13.426649441317991,-13.211458495508678,-12.998388005389506,-12.78747784922712,-12.578764351970982,-12.372280340569974,-12.168055218109545,-11.966115053757051,-11.766482685691647,-11.569177834411454,
-11.37421722404345,-11.181614709521233,-10.991381407734941,-10.803525830990411,-10.618054021336585,-10.434969684528458,-10.254274322585168,-10.075967364078432,-9.9000462914447365,-9.7265067647559924,-9.5553427415079337,
-9.3865465920947191,-9.2201092107325859,-9.0560201216764842,-8.8942675806424969,-8.7348386714067168,-8.5777193975993367,-8.4228947697522134,-8.27034888769003,-8.1200650183804886,-7.9720256693785849,-7.8262126580147866,
-7.6826071764875277,-7.5411898530275794,-7.401940809306045,-7.2648397142595265,-7.1298658345058019,-6.99699808152157,-6.8662150557507005,-6.7374950878073623,-6.610816276933523,-6.4861565268648524,-6.3634935792532472,
-6.242805044788029,-6.1240684321516072,-6.0072611749390683,-5.89236065666477,-5.7793442339728127,-5.6681892581620934,-5.5588730951306564,-5.4513731438382775,-5.3456668533806244,-5.2417317387629492,-5.1395453954561567,
-5.0390855128131582,-4.9403298864187741,-4.843256429442004,-4.7478431830552763,-4.6540683259813269,-4.5619101832245894,-4.4713472340404348,-4.3823581191922649,-4.2949216475433314,-4.20901680202716,-4.1246227450377342,
-4.0417188232779333,-3.9602845721023168,-3.880299719388014,-3.8017441889653552,-3.724598103637843,-3.64884178781919,-3.5744557698133628,-3.5014207837619322,-3.4297177712814646,-3.3593278828122473,-3.2902324786982788,
-3.2224131300171819,-3.1558516191775068,-3.0905299402997906,-3.0264302993966763,-2.963535114366437,-2.9018270148133363,-2.8412888417073918,-2.7819036468953264,-2.7236546924737293,-2.6665254500347531,-2.6104995997940321,
-2.5555610296098639,-2.5016938339021562,-2.4488823124790762,-2.3971109692788506,-2.3463645110336864,-2.2966278458623473,-2.2478860817974908,-2.2001245252535093,-2.1533286794402211,-2.1074842427274509,-2.0625771069651884,
-2.0185933557637372,-1.9755192627379734,-1.933341289719569,-1.8920460849407919,-1.8516204811932628,-1.8120514939648267,-1.7733263195574931,-1.7354323331892134,-1.6983570870820726,-1.6620883085393137,-1.6266138980134468,
-1.5919219271675513,-1.5580006369317323,-1.52483843555657,-1.4924238966652665,-1.4607457573060867,-1.4297929160065774,-1.3995544308309438,-1.3700195174418688,-1.3411775471679719,-1.3130180450780113,-1.2855306880628632,
-1.2587053029262261,-1.2325318644849372,-1.2070004936797132,-1.1821014556970695,-1.1578251581031134,-1.1341621489898455,-1.1111031151345638,-1.0886388801728995,-1.0667604027859823,-1.0454587749021826,-1.0247252199138377,
-1.0045510909093305,-0.98492786892085893,-0.96584716118819258,-0.94730069943868611,-0.92928033818378952,-0.91177805303226511,-0.89478593902029668,-0.87829620895865,-0.86230119179702436,-0.84679333100570731,-0.83176518297463053,
-0.81720941542990277,-0.80311880586787643,-0.78948624000679168,-0.77630471025602377,-0.76356731420294388,-0.75126725311739329,-0.739397830473754,-0.72795245049059087,-0.71692461668782625,-0.70630793046139828,-0.69609608967534664,
-0.68628288727125686,-0.67686220989498924,-0.66782803654060818,-0.65917443721142255,-0.650895571598039,-0.642985687773326,-0.63543912090417876,-0.62825029197997162,-0.62141370655757833,-0.614923953522836,-0.60877570386832747,
-0.602963709487347,-0.59748280198391879,-0.59232789149872633,-0.58749396555081523,-0.58297608789492394,-0.57876939739429878,-0.57486910690884407,-0.57127050219845943,-0.56796894084141281,-0.564959851167597,-0.56223873120651646,
-0.55980114764985056,-0.55764273482843629,-0.55575919370351745,-0.55414629087210088,-0.55279985758626571,-0.55171578878626693,-0.55089004214727688,-0.55031863713960771,-0.54999765410225776,-0.54992323332962478,-0.55009157417123056,
-0.55049893414430029,-0.55114162805904088,-0.55201602715646392,-0.55311855825859979,-0.55444570293094664,-0.55599399665700511,-0.55776002802474345,-0.55974043792484474,-0.56193191876058368,-0.56433121366918615,-0.56693511575452138,
-0.56974046733098083,-0.57274415917839783,-0.57594312980786255,-0.57933436473828925,-0.58291489578359312,-0.58668180035033557,-0.59063220074569811,-0.59476326349564568,-0.599072198673143,-0.603556259236286,-0.60821274037621642,
-0.61303897887468273,-0.61803235247111843,-0.62319027923910408,-0.62851021697208642,-0.63398966257822453,-0.63962615148423685,-0.64541725704812469,-0.65136058998064694,-0.65745379777542456,-0.66369456414755323,-0.67008060848060436},
new double[]{
-34.273247277968849,-34.106580611302185,-33.939913944635514,-33.773247277968849,-33.606580611302185,-33.439913944635514,-33.273247277968849,-33.106580611302185,-32.939913944635514,-32.773247277968849,-32.606580611302185,-32.439913944635514,
-32.273247277968849,-32.106580611302185,-31.939913944635517,-31.773247277968849,-31.606580611302181,-31.439913944635517,-31.273247277968849,-31.106580611302181,-30.939913944635517,-30.773247277968849,-30.606580611302181,
-30.439913944635517,-30.273247277968849,-30.106580611302181,-29.939913944635517,-29.773247277968849,-29.606580611302185,-29.439913944635517,-29.273247277968853,-29.106580611302185,-28.939913944635521,-28.773247277968856,
-28.606580611302192,-28.439913944635531,-28.273247277968867,-28.10658061130221,-27.939913944635553,-27.773247277968903,-27.606580611302256,-27.439913944635617,-27.273247277968991,-27.10658061130238,-26.939913944635791,
-26.773247277969233,-26.606580611302714,-26.43991394463626,-26.273247277969887,-26.106580611303631,-25.939913944637539,-25.77324727797167,-25.606580611306121,-25.439913944641013,-25.273247277976523,-25.106580611312889,
-24.93991394465046,-24.773247277989704,-24.606580611331289,-24.439913944676135,-24.273247278025536,-24.106580611381293,-23.939913944745918,-23.773247278122923,-23.606580611517195,-23.439913944935565,-23.273247278387554,
-23.106580611886443,-22.939913945450758,-22.773247279106315,-22.606580612889108,-22.439913946849266,-22.273247281056626,-22.106580615608365,-21.939913950639671,-21.773247286338428,-21.606580622965545,-21.43991396088294,
-21.273247300592043,-21.106580642786582,-20.939913988424788,-20.773247338827893,-20.606580695813907,-20.439914061878842,-20.273247440441075,-20.10658083616957,-19.939914255422671,-19.773247706831995,-19.606581202075425,
-19.439914756895089,-19.273248392430467,-19.106582136954202,-18.9399160281186,-18.773250115845062,-18.60658446601661,-18.439919165165438,-18.273254326383164,-18.106590096720637,-17.939926666386373,-17.773264280096825,
-17.60660325097686,-17.439943977452703,-17.27328696362046,-17.106632843608327,-16.939982410476571,-16.773336650212773,-16.606696781377167,-16.4400643009298,-16.273441036724307,-16.10682920707837,-15.940231487725932,
-15.773651086318285,-15.607091824470022,-15.440558227141745,-15.274055618916952,-15.107590226470137,-14.941169286243149,-14.77480115605607,-14.608495429087558,-14.442263048379996,-14.276116419769988,-14.110069520928649,
-13.944138004032087,-13.778339289482949,-13.61269264807963,-13.447219269088192,-13.281942311817918,-13.1168869385349,-12.95208032686536,-12.787551660233257,-12.623332095332847,-12.459454706140537,-12.295954404503025,
-12.132867837880131,-11.970233265350361,-11.808090413484667,-11.646480314141018,-11.485445126613314,-11.325027946871037,-11.165272606842251,-11.00622346681871,-10.847925204097757,-10.690422600925734,-10.533760334679304,
-10.377982773024359,-10.223133776539342,-10.069256510993773,-9.9163932711472285,-9.76458531759204,-9.6138727278167089,-9.4642942623275985,-9.3158872463427436,-9.16868746727137,-9.0227290879211584,-8.87804457513635,
-8.7346646433654911,-8.59261821248842,-8.451932379097558,-8.3126324003268852,-8.17474168925075,-8.0382818208311377,-7.9032725473726719,-7.76973182244635,-7.6376758322622562,-7.5071190335050257,-7.3780741966906644,
-7.2505524541565665,-7.1245633518557918,-7.0001149041896689,-6.8772136511776951,-6.755864717328957,-6.6360718716436908,-6.5178375882360058,-6.4011631071285757,-6.2860484948266109,-6.1724927043312912,-6.0604936343018627,
-5.9500481871206317,-5.8411523256561155,-5.7338011285567791,-5.6279888439410755,-5.523708941379267,-5.4209541620888109,-5.3197165672882214,-5.2199875846745361,-5.1217580530069782,-5.02501826479446,-4.9297580070973748,
-4.8359666004648814,-4.7436329360379688,-4.6527455108559,-4.5632924614097146,-4.4752615954911832,-4.3886404223892983,-4.303416181489105,-4.2195758693296055,-4.1371062651786827,-4.0559939551835829,-3.9762253551556519,
-3.8977867320476522,-3.8206642241813653,-3.7448438602821867,-3.6703115773762494,-3.5970532376042077,-3.5250546440042889,-3.4543015553155723,-3.384779699850728,-3.3164747884856651,-3.2493725268117277,-3.1834586264942462,
-3.1187188158794257,-3.0551388498897416,-2.9927045192462209,-2.9314016590542478,-2.871216156787801,-2.8121339597053825,-2.7541410817292715,-2.6972236098181819,-2.6413677098618895,-2.5865596321249509,-2.5327857162652463,
-2.4800323959517376,-2.4282862031045656,-2.3775337717793792,-2.3277618417166481,-2.2789572615755658,-2.231106991871131,-2.1841981076319676,-2.1382178007954891,-2.0931533823561197,-2.0489922842814092,-2.0057220612100726,
-1.9633303919451957,-1.921805080755141,-1.8811340584939587,-1.8413053835524822,-1.8023072426506412,-1.7641279514809498,-1.7267559552125626,-1.6901798288647691,-1.6543882775582959,-1.6193701366523152,-1.5851143717746141,
-1.5516100787519596,-1.5188464834472915,-1.486812941510008,-1.4554989380452454,-1.4248940872077269,-1.3949881317254287,-1.3657709423580262,-1.337232517294785,-1.309362981496309,-1.2821525859842977,-1.2555917070832268,
-1.2296708456176484,-1.2043806260685883,-1.1797117956923171,-1.1556552236045912,-1.1322018998332686,-1.1093429343420471,-1.0870695560279042,-1.065373111694675,-1.0442450650050523,-1.0236769954131733,-1.0036605970798127,
-0.98418767777209737,-0.96525015774953549,-0.94684006863804837,-0.92894955229359388,-0.91157085965687179,-0.89469634960051325,-0.87831848777006982,-0.86242984542003831,-0.8470230982460798,-0.83209102521452183,-0.8176265073901603,
-0.80362252676331858,-0.79007216507705647,-0.77696860265536727,-0.764305117233143,-0.752075082788642,-0.74027196837913978,-0.7288893369804007,-0.717920844330564,-0.707360237778998,-0.69720135514063541,-0.68743812355626865,
-0.67806455835924817,-0.66907476194899407,-0.66046292267170192,-0.65222331370859321,-0.64435029197203386,-0.63683829700981875,-0.629681849917896,-0.622875552261779,-0.616414085006878,-0.6102922074579552,-0.60450475620789434,
-0.59904664409595421,-0.59391285917565817,-0.58909846369245666,-0.58459859307128448,-0.58040845491411774,-0.57652332800762629,-0.57293856134099919,-0.56964957313401254,-0.56665184987539718,-0.56394094537155082,-0.56151247980563168,
-0.55936213880705987,-0.55748567253144443,-0.55587889475094443,-0.554537681955067,-0.55345797246189576,-0.55263576553973626,-0.55206712053916007,-0.5517481560354226,-0.55167504898122255,-0.55184403386976744,-0.55225140190810473,
-0.55289350020067118,-0.55376673094301287,-0.55486755062561932,-0.55619246924781585,-0.55773804954165207,-0.55950090620572313,-0.5614777051488552,-0.56366516274358625,-0.5660600450893698,-0.56865916728542487,-0.57145939271315882,
-0.57445763232807989,-0.577650843961123,-0.58103603162930328,-0.58461024485561564,-0.58837057799809533,-0.59231416958795191,-0.59643820167669126,-0.60073989919213622,-0.6052165293032582,-0.609865400793729,-0.61468386344410331,
-0.61966930742254123,-0.62481916268397975,-0.63013089837766156,-0.63560202226293039,-0.641230080133199,-0.64701265524800178,-0.65294736777303541,-0.6590318742281,-0.66526386694284556,-0.6716410735202355,-0.67816125630763224},
new double[]{
-27.315955555574913,-27.190955555574913,-27.065955555574913,-26.940955555574913,-26.815955555574913,-26.690955555574913,-26.565955555574913,-26.440955555574913,-26.315955555574913,-26.190955555574913,-26.065955555574913,-25.940955555574913,
-25.815955555574917,-25.690955555574917,-25.565955555574917,-25.440955555574917,-25.315955555574917,-25.190955555574917,-25.065955555574917,-24.94095555557492,-24.81595555557492,-24.690955555574924,-24.565955555574924,
-24.440955555574927,-24.315955555574934,-24.190955555574938,-24.065955555574945,-23.940955555574952,-23.815955555574966,-23.69095555557498,-23.565955555574998,-23.44095555557502,-23.315955555575052,-23.190955555575091,
-23.06595555557514,-22.940955555575204,-22.81595555557529,-22.690955555575396,-22.565955555575531,-22.440955555575709,-22.315955555575933,-22.190955555576224,-22.065955555576593,-21.940955555577073,-21.815955555577684,
-21.690955555578473,-21.565955555579482,-21.440955555580782,-21.315955555582448,-21.190955555584587,-21.065955555587333,-20.940955555590861,-20.815955555595394,-20.69095555560121,-20.565955555608678,-20.440955555618267,
-20.31595555563058,-20.19095555564639,-20.065955555666694,-19.94095555569276,-19.81595555572623,-19.690955555769211,-19.565955555824395,-19.440955555895254,-19.315955555986235,-19.190955556103059,-19.065955556253066,
-18.940955556445672,-18.815955556692984,-18.690955557010533,-18.565955557418267,-18.440955557941795,-18.315955558613997,-18.190955559477093,-18.065955560585273,-17.940955562008121,-17.815955563834951,-17.690955566180421,
-17.56595556919169,-17.440955573057639,-17.315955578020652,-17.190955584391734,-17.065955592569878,-16.94095560306685,-16.815955616538918,-16.690955633827386,-16.565955656010416,-16.440955684469166,-16.315955720972088,
-16.190955767782292,-16.065955827794077,-15.940955904706275,-15.815956003241926,-15.690956129426205,-15.565956290937203,-15.440956497547697,-15.315956761680056,-15.190957099101341,-15.065957529791451,-14.940958079023885,
-14.815958778706666,-14.690959669039982,-14.565960800557512,-14.440962236630073,-14.315964056523225,-14.190966359114784,-14.06596926739361,-13.940972933877442,-13.815977547104644,-13.690983339372023,-13.565990595907874,
-13.440999665685578,-13.316010974097306,-13.191025037719102,-13.066042481406448,-12.941064057962457,-12.816090670617687,-12.691123398550015,-12.566163525653918,-12.441212572739547,-12.316272333302383,-12.191344912953108,
-12.066432772534316,-11.941538774875781,-11.81666623505366,-11.691818973922272,-11.567001374581578,-11.442218441331395,-11.317475860547592,-11.192780062799267,-11.068138285413335,-10.943558634587909,-10.819050146063107,
-10.694622843281652,-10.570287791916169,-10.44605714960935,-10.3219442097701,-10.197963438295984,-10.074130502150982,-9.9504622888177554,-9.8269769157645044,-9.7036937292154075,-9.5806332916873,-9.4578173579486684,
-9.3352688392650371,-9.2130117560108342,-9.0910711789453469,-8.96947315966278,-8.84824465092702,-8.72741341778471,-8.6070079405102238,-8.4870573105689981,-8.367591120888326,-8.2486393517950649,-8.1302322540173382,
-8.0124002301525827,-7.8951737159788795,-7.7785830629329578,-7.6626584229999244,-7.5474296371604765,-7.4329261284253665,-7.3191768003585933,-7.2062099418544783,-7.0940531387937042,-6.9827331930632113,-6.8722760492881241,
-6.7627067294933232,-6.65404927579041,-6.5463267010742845,-6.4395609476137814,-6.3337728533334561,-6.2289821255091278,-6.1252073215380225,-6.0224658363949883,-5.9207738963486687,-5.8201465584847956,-5.7205977155670151,
-5.6221401057577767,-5.5247853267216795,-5.4285438536402273,-5.3334250606790485,-5.2394372454653189,-5.1465876561533666,-5.0548825206794561,-4.9643270778316237,-4.8749256097866347,-4.7866814757929239,-4.6995971467053135,
-4.61367424010398,-4.5289135557561426,-4.4453151112040556,-4.3628781772868859,-4.2816013134267639,-4.2014824025306341,-4.1225186853794069,-4.0447067943943473,-3.9680427866875543,-3.8925221763188831,-3.818139965695722,
-3.7448906760647667,-3.6727683770563444,-3.6017667152520567,-3.5318789417555552,-3.4630979387542906,-3.3954162450670591,-3.3288260806783141,-3.2633193702654766,-3.1988877657300261,-3.1355226677469958,-3.0732152463507507,
-3.011956460577605,-2.9517370771880382,-2.8925476884930337,-2.8343787293104246,-2.7772204930781825,-2.7210631471522988,-2.6658967473173938,-2.611711251538428,-2.5584965329819402,-2.50624239233513,-2.4549385694508215,
-2.4045747543459934,-2.3551405975810633,-2.3066257200465539,-2.2590197221831523,-2.212312192660483,-2.1664927165391914,-2.1215508829401886,-2.0774762922441323,-2.0342585628434198,-1.9918873374681876,-1.9503522891070071,
-1.9096431265421674,-1.8697495995186608,-1.8306615035652019,-1.7923686844848559,-1.7548610425321007,-1.718128536292425,-1.6821611862798556,-1.6469490782671172,-1.6124823663624672,-1.5787512758466009,-1.545746105782402,
-1.5134572314097168,-1.48187510633675,-1.4509902645391339,-1.4207933221771811,-1.3912749792413308,-1.3624260210353043,-1.3342373195060195,-1.3066998344288687,-1.279804614456534,-1.2535427980391078,-1.2279056142228964,
-1.2028843833349117,-1.1784705175597035,-1.1546555214148457,-1.1314309921310655,-1.1087886199427044,-1.0867201882939013,-1.0652175739656131,-1.0442727471283249,-1.0238777713250491,-1.004024803388972,-0.984706093299885,
-0.96591398398331263,-0.94764091105605353,-0.9298794025216488,-0.91262207841910981,-0.89586165042806054,-0.87959092143328288,-0.86380278505149422,-0.84849022512303585,-0.83364631517100651,-0.819264217830242,-0.80533718424841039,
-0.7918585534613708,-0.7788217517448266,-0.76622029194419528,-0.75404777278451085,-0.742297878162076,-0.73096437641948586,-0.72004111960555761,-0.70952204272161379,-0.699401162955486,-0.68967257890453149,-0.6803304697888799,
-0.67136909465606065,-0.66278279157809483,-0.65456597684207407,-0.64671314413519021,-0.6392188637251226,-0.63207778163664019,-0.62528461882521968,-0.61883417034844035,-0.61272130453586515,-0.60694096215807869,-0.60148815559550894,
-0.59635796800762486,-0.5915455525030624,-0.58704613131119687,-0.58285499495564985,-0.57896750143018394,-0.57537907537741151,-0.57208520727071477,-0.56908145259974852,-0.5663634310598713,-0.5639268257458262,-0.5617673823499727,
-0.5598809083653461,-0.55826327229380279,-0.55691040285949078,-0.55581828822786594,-0.55498297523045637,-0.55440056859556364,-0.55406723018507109,-0.553979178237518,-0.55413268661758153,-0.55452408407209763,-0.555149753492739,
-0.5560061311854555,-0.55708970614677411,-0.55839701934704111,-0.55992466302068389,-0.56166927996355631,-0.56362756283742543,-0.56579625348164864,-0.56817214223208157,-0.57075206724725025,-0.5735329138418146,-0.576511613827344,
-0.57968514486041611,-0.58305052979805116,-0.58660483606048053,-0.59034517500124972,-0.59426870128464759,-0.59837261227044958,-0.60265414740595979,-0.60711058762533154,-0.61173925475614266,-0.61653751093319775,-0.62150275801952881,
-0.62663243703455773,-0.63192402758938815,-0.637375047329183,-0.64298305138259182,-0.648745631818178,-0.6546604171078062,-0.66072507159693716,-0.66693729498178345,-0.673294821793273,-0.67979542088776912,-0.68643689494449078},
new double[]{
-22.803297033349448,-22.70329703334945,-22.603297033349453,-22.503297033349455,-22.403297033349453,-22.303297033349455,-22.203297033349457,-22.103297033349463,-22.003297033349465,-21.903297033349471,-21.803297033349473,-21.703297033349482,
-21.603297033349488,-21.503297033349497,-21.40329703334951,-21.303297033349523,-21.203297033349539,-21.103297033349559,-21.003297033349586,-20.903297033349617,-20.803297033349654,-20.703297033349703,-20.603297033349758,
-20.503297033349828,-20.403297033349912,-20.303297033350013,-20.20329703335014,-20.103297033350294,-20.003297033350481,-19.903297033350711,-19.80329703335099,-19.70329703335133,-19.603297033351748,-19.503297033352258,
-19.403297033352882,-19.30329703335364,-19.20329703335457,-19.103297033355705,-19.003297033357089,-18.903297033358783,-18.803297033360849,-18.703297033363373,-18.603297033366456,-18.503297033370224,-18.403297033374823,
-18.303297033380442,-18.203297033387305,-18.103297033395688,-18.003297033405925,-17.903297033418429,-17.803297033433704,-17.703297033452358,-17.603297033475144,-17.503297033502971,-17.403297033536962,-17.303297033578481,
-17.203297033629187,-17.103297033691124,-17.003297033766771,-16.903297033859168,-16.803297033972022,-16.703297034109863,-16.603297034278221,-16.503297034483854,-16.403297034735012,-16.30329703504178,-16.203297035416465,
-16.103297035874107,-16.003297036433068,-15.903297037115783,-15.803297037949651,-15.703297038968135,-15.603297040212107,-15.503297041731488,-15.403297043587246,-15.303297045853853,-15.203297048622257,-15.103297052003544,
-15.003297056133379,-14.903297061177456,-14.803297067338137,-14.703297074862558,-14.603297084052528,-14.503297095276626,-14.40329710898494,-14.303297125727084,-14.203297146174164,-14.103297171145584,-14.003297201641759,
-13.90329723888398,-13.803297284363042,-13.703297339898505,-13.603297407710871,-13.503297490509498,-13.403297591599584,-13.303297715012295,-13.203297865662936,-13.103298049543046,-13.00329827395349,-12.903298547787031,
-12.80329888187045,-12.703299289378306,-12.603299786332586,-12.503300392205187,-12.403301130643133,-12.303302030339939,-12.203303126080474,-12.103304459991108,-12.003306083032006,-11.903308056773932,-11.803310455508107,
-11.703313368744322,-11.603316904159689,-11.503321191068023,-11.403326384487777,-11.303332669894585,-11.20334026875255,-11.103349444926302,-11.003360512083196,-10.903373842201425,-10.803389875305088,-10.70340913055071,
-10.603432218791063,-10.503459856740744,-10.403492882863443,-10.303532275092557,-10.203579170484252,-10.103634886885006,-10.003700946673424,-9.9037791026086985,-9.80387136578518,-9.7039800356543235,-9.6041077320318973,
-9.50425742896033,-9.404432490243984,-9.304636706420176,-9.2048743328716114,-9.105150128728507,-9.0054693961521544,-8.9058380195379936,-8.806262504127286,-8.7067500134738864,-8.60730840517861,-8.5079462642795853,
-8.4086729336747776,-8.3094985409535944,-8.2104340210290747,-8.111491133991434,-8.012682477647397,-7.9140214942677654,-7.815522471136843,-7.7172005345805426,-7.61907163724321,-7.5211525384843059,-7.4234607778725632,
-7.3260146418642655,-7.2288331238610892,-7.1319358779486386,-7.0353431667166166,-6.93907580365301,-6.8431550906854763,-6.7476027515114971,-6.65244086141337,-6.5576917742938372,-6.4633780476926868,-6.369522366554,
-6.2761474665083963,-6.183276057415469,-6.0909307478799883,-5.9991339714126282,-5.9079079148539835,-5.8172744496209958,-5.7272550662697093,-5.6378708127992914,-5.5491422370512575,-5.4610893334865755,-5.37373149455321,
-5.2870874667890595,-5.2011753117412152,-5.116012371722972,-5.0316152403756158,-4.9479997379532952,-4.8651808912064451,-4.7831729177024007,-4.7019892143910074,-4.6216423501979778,-4.5421440624092373,-4.4635052565951652,
-4.3857360098140807,-4.3088455768290332,-4.2328423990705879,-4.1577341160802117,-4.0835275791736958,-4.0102288670713238,-3.9378433032507281,-3.8663754747892125,-3.795829252474376,-3.7262078119747937,-3.6575136558760128,
-3.5897486364009685,-3.5229139786478241,-3.4570103041920754,-3.3920376549133229,-3.3279955169202862,-3.2648828444603315,-3.2026980837118875,-3.1414391963696175,-3.081103682943036,-3.021688605699373,-2.9631906111909219,
-2.90560595231582,-2.8489305098692372,-2.7931598135493045,-2.7382890623888083,-2.6843131445897606,-2.6312266567434284,-2.57902392242334,-2.5276990101431722,-2.47724575067534,-2.4276577537295578,-2.3789284239936705,
-2.3310509765417047,-2.2840184516163666,-2.2378237287951972,-2.1924595405512477,-2.1479184852205573,-2.1041930393898793,-2.0612755697190415,-2.0191583442130963,-1.9778335429599954,-1.9372932683499524,-1.8975295547929623,
-1.8585343779511163,-1.8202996635024362,-1.7828172954529256,-1.7460791240134472,-1.7100769730578804,-1.6748026471787789,-1.6402479383564983,-1.6064046322574466,-1.5732645141767709,-1.5408193746404295,-1.5090610146812014,
-1.4779812508027892,-1.4475719196457446,-1.4178248823685204,-1.3887320287565244,-1.3602852810716108,-1.3324765976540143,-1.3052979762882977,-1.2787414573444567,-1.2527991267049035,-1.227463118487635,-1.2027256175754837,
-1.178578861960953,-1.1550151449157484,-1.1320268169937382,-1.1096062878757078,-1.0877460280639202,-1.066438570434143,-1.0456765116524718,-1.0254525134639567,-1.0057593038597272,-0.98658967812900633,-0.96793649980212415,
-0.94979270149035155,-0.93215128562811822,-0.91500532512291377,-0.89834796391792993,-0.88217241747226027,-0.86647197316325264,-0.85123999061538635,-0.83646990195984328,-0.822155212028739,-0.80828949848779175,-0.794866411911025,
-0.781879675800922,-0.769323086557289,-0.75719051339792143,-0.74547589823401572,-0.73417325550312629,-0.72327667196232648,-0.71278030644410173,-0.70267838957737749,-0.69296522347596179,-0.68363518139657087,-0.67468270736849489,
-0.66610231579685686,-0.65788859104132047,-0.65003618697200349,-0.64253982650426888,-0.63539430111397566,-0.62859447033469029,-0.62213526123828422,-0.61601166790026474,-0.61021875085111876,-0.60475163651488151,-0.5996055166360752,
-0.59477564769610625,-0.5902573503201477,-0.5860460086754794,-0.58213706986220748,-0.57852604329723234,-0.575208500092287,-0.57218007242682489,-0.56943645291648959,-0.56697339397785917,-0.5647867071901217,-0.56287226265429524,
-0.56122598835057769,-0.55984386949437071,-0.55872194789149832,-0.55785632129310148,-0.557243142750669,-0.55687861997163324,-0.55675901467593358,-0.55688064195392761,-0.55723986962600447,-0.55783311760423371,-0.558656857256361,
-0.55970761077244291,-0.56098195053439226,-0.56247649848868908,-0.56418792552249375,-0.5661129508433822,-0.56824834136290991,-0.570590911084193,-0.57313752049368383,-0.57588507595730321,-0.57883052912108113,-0.58197087631644207,
-0.58530315797026355,-0.58882445801982408,-0.59253190333274575,-0.59642266313202952,-0.600493948426268,-0.60474301144511788,-0.609167145080099,-0.6137636823307856,-0.61852999575644352,-0.62346349693316272,-0.62856163591652647,
-0.63382190070985378,-0.639241816738044,-0.64481894632704972,-0.6505508881889942,-0.65643527691295134,-0.66246978246139521,-0.6686521096723248,-0.67497999776706685,-0.68145121986375179,-0.68806358249645816,-0.69481492514001564},
new double[]{
-19.626615635305431,-19.543282301972116,-19.459948968638809,-19.376615635305505,-19.293282301972205,-19.209948968638908,-19.126615635305622,-19.043282301972344,-18.959948968639079,-18.876615635305821,-18.793282301972578,-18.709948968639353,
-18.626615635306148,-18.543282301972965,-18.459948968639807,-18.376615635306685,-18.293282301973598,-18.209948968640557,-18.126615635307569,-18.043282301974646,-17.959948968641793,-17.87661563530903,-17.793282301976372,
-17.709948968643833,-17.626615635311438,-17.543282301979215,-17.459948968647193,-17.37661563531541,-17.293282301983908,-17.209948968652736,-17.126615635321958,-17.043282301991642,-16.959948968661873,-16.876615635332751,
-16.793282302004393,-16.709948968676937,-16.626615635350547,-16.543282302025414,-16.459948968701774,-16.376615635379888,-16.293282302060078,-16.209948968742719,-16.126615635428262,-16.043282302117223,-15.959948968810231,
-15.876615635508015,-15.793282302211443,-15.709948968921539,-15.62661563563951,-15.543282302366785,-15.459948969105053,-15.376615635856307,-15.2932823026229,-15.209948969407618,-15.126615636213744,-15.043282303045164,
-14.959948969906462,-14.876615636803059,-14.793282303741357,-14.709948970728917,-14.626615637774675,-14.543282304889184,-14.459948972084913,-14.376615639376594,-14.293282306781627,-14.20994897432057,-14.126615642017708,
-14.043282309901731,-13.959948978006533,-13.876615646372152,-13.793282315045891,-13.709948984083626,-13.626615653551374,-13.543282323527114,-13.459948994102975,-13.376615665387789,-13.293282337510123,-13.209949010621857,
-13.126615684902415,-13.043282360563753,-12.959949037856259,-12.876615717075717,-12.79328239857154,-12.709949082756486,-12.626615770118139,-12.543282461232463,-12.459949156779814,-12.376615857563852,-12.293282564533858,
-12.209949278811106,-12.126616001719983,-12.043282734824727,-11.95994947997281,-11.876616239346108,-11.793283015521311,-11.709949811541181,-11.626616630998605,-11.543283478135706,-11.459950357960702,-11.376617276385604,
-11.293284240388443,-11.209951258204306,-11.126618339550157,-11.043285495889325,-10.959952740742413,-10.876620090052544,-10.793287562614115,-10.70995518057563,-10.626622970028858,-10.543290961698416,-10.459959191747878,
-10.376627702720951,-10.293296544638725,-10.209965776276935,-10.126635466650232,-10.0433056967339,-9.95997656145704,-9.8766481720051882,-9.7933206584743431,-9.7099941729226469,-9.6266688928702386,-9.5433450253021466,
-9.4600228112331681,-9.3767025308977416,-9.2933845096312684,-9.2100691245123123,-9.1267568118373177,-9.0434480755006348,-8.9601434963526216,-8.8768437426070168,-8.7935495813655731,-8.7102618913226291,-8.626981676704828,
-8.5437100824913266,-8.4604484109472,-8.3771981394875183,-8.2939609398714484,-8.2107386987047342,-8.1275335392052881,-8.04434784416039,-7.9611842799755044,-7.8780458216844345,-7.794935778758898,-7.7118578215232292,
-7.6288160079476395,-7.5458148105619713,-7.4628591432021354,-7.3799543872743243,-7.2971064171985986,-7.2143216246744855,-7.1316069413976617,-7.0489698598494313,-6.9664184517802408,-6.883961384015362,-6.8016079312254778,
-6.71936798532736,-6.637252061209991,-6.5552712985190142,-6.4734374592767461,-6.3917629211653466,-6.3102606663560641,-6.2289442658266685,-6.14782785917087,-6.06692612996631,-5.9862542768302136,-5.9058279803524361,
-5.8256633661532113,-5.7457769643659509,-5.666185665892935,-5.5869066758226413,-5.5079574644310316,-5.4293557162148236,-5.3511192774223026,-5.2732661025564731,-5.1958142003264776,-5.1187815795165807,-5.04218619522809,
-4.9660458959291063,-4.8903783717207512,-4.8152011041973255,-4.7405313182427466,-4.6663859360674937,-4.5927815337501023,-4.5197343005059256,-4.447260000864306,-4.3753739398941844,-4.30409093157827,-4.2334252703977988,
-4.1633907061540523,-4.0940004220196595,-4.02526701578251,-3.9572024842180764,-3.8898182105021535,-3.8231249545555706,-3.7571328461952112,-3.6918513809516131,-3.6272894184023818,-3.5634551828624343,-3.5003562662665009,
-3.4379996330760849,-3.3763916270420413,-3.3155379796547391,-3.2554438201162736,-3.1961136866730917,-3.1375515391524651,-3.079760772552302,-3.0227442315406128,-2.9665042257283383,-2.9110425455870867,-2.8563604788914074,
-2.80245882757348,-2.7493379248863632,-2.6969976527801336,-2.6454374594033139,-2.5946563766498092,-2.544653037679133,-2.4954256943449682,-2.4469722344739893,-2.3992901989434348,-2.3523767985120472,-2.3062289303647825,
-2.2608431943370495,-2.21621590878922,-2.1723431261067634,-2.1292206478055693,-2.0868440392258978,-2.04520864380193,-2.0043095968970639,-1.9641418391980181,-1.9247001296633695,-1.8859790580244951,-1.8479730568389357,
-1.810676413098038,-1.7740832793923287,-1.7381876846394861,-1.7029835443809878,-1.6684646706545625,-1.6346247814504678,-1.6014575097603583,-1.5689564122281436,-1.53711497741273,-1.5059266336729484,-1.4753847566852834,
-1.4454826766052338,-1.4162136848833002,-1.3875710407466704,-1.3595479773577071,-1.33213770766032,-1.3053334299252353,-1.27912833300507,-1.2535156013099855,-1.2284884195145187,-1.2040399770060071,-1.1801634720848082,
-1.1568521159262886,-1.1340991363143138,-1.1118977811557269,-1.0902413217850295,-1.069123056068233,-1.0485363113145558,-1.0284744470043943,-1.0089308573416982,-0.989898973638632,-0.97137226654011477,-0.953344248095574,
-0.93580847368498188,-0.91875854380597766,-0.90218810572863228,-0.88609085502415341,-0.87046053697358761,-0.85529094786233861,-0.84057593616608584,-0.82630940363346128,-0.8124853062706251,-0.79909765523266474,-0.7861405176265388,
-0.77360801723008354,-0.76149433513141074,-0.74979371029283681,-0.73850044004330406,-0.72760888050308126,-0.717113446944364,-0.70700861409123483,-0.69728891636228718,-0.68794894805907014,-0.67898336350336685,-0.670386877126182,
-0.66215426351118345,-0.65428035739521573,-0.64676005362838052,-0.63958830709606551,-0.63276013260519048,-0.62627060473683083,-0.62011485766728036,-0.61428808495951359,-0.60878553932691626,-0.60360253237106187,-0.59873443429522732,
-0.59417667359525816,-0.58992473672931489,-0.58597416776795785,-0.58232056802595678,-0.57895959567713973,-0.57588696535353545,-0.57309844772999619,-0.57058986909543119,-0.56835711091172259,-0.56639610936134155,-0.56470285488463112,
-0.56327339170767088,-0.56210381736159376,-0.56119028219417733,-0.56052898887449143,-0.56011619189134176,-0.55994819704620891,-0.56002136094134658,-0.56033209046366739,-0.56087684226500789,-0.56165212223933525,-0.562654484997426,
-0.56388053333951649,-0.56532691772639987,-0.56699033574941349,-0.5688675315997398,-0.57095529553741509,-0.57325046336042174,-0.57574991587421342,-0.57845057836200531,-0.58134942005613965,-0.58444345361081806,-0.58772973457647437,
-0.591205360876046,-0.59486747228338133,-0.598713249904011,-0.60273991565849028,-0.6069447317685116,-0.61132500024596625,-0.61587806238512821,-0.620601298258117,-0.62549212621378536,-0.63054800238016839,-0.63576642017061857,
-0.6411449097937435,-0.64668103776725017,-0.65237240643579553,-0.65821665349293146,-0.66421145150722416,-0.67035450745262293,-0.67664356224314393,-0.68307639027192879,-0.6896507989547308,-0.69636462827787637,-0.7032157503507428},
new double[]{
-17.26391891615803,-17.192490344729709,-17.121061773301424,-17.049633201873181,-16.978204630444992,-16.90677605901686,-16.8353474875888,-16.763918916160812,-16.692490344732917,-16.621061773305122,-16.549633201877452,-16.478204630449916,
-16.40677605902254,-16.335347487595349,-16.263918916168372,-16.192490344741636,-16.121061773315184,-16.049633201889055,-15.978204630463305,-15.906776059037984,-15.835347487613165,-15.763918916188921,-15.692490344765343,
-15.621061773342531,-15.549633201920601,-15.478204630499693,-15.406776059079963,-15.33534748766159,-15.263918916244782,-15.19249034482978,-15.121061773416864,-15.049633202006351,-14.978204630598611,-14.906776059194071,
-14.83534748779322,-14.763918916396627,-14.692490345004943,-14.621061773618925,-14.549633202239441,-14.478204630867497,-14.406776059504248,-14.335347488151029,-14.263918916809383,-14.192490345481085,-14.121061774168185,
-14.049633202873048,-13.978204631598404,-13.906776060347395,-13.835347489123656,-13.76391891793137,-13.692490346775371,-13.621061775661227,-13.54963320459537,-13.478204633585213,-13.406776062639308,-13.335347491767525,
-13.263918920981245,-13.192490350293598,-13.121061779719732,-13.049633209277118,-12.978204638985911,-12.906776068869364,-12.835347498954297,-12.76391892927165,-12.692490359857114,-12.621061790751863,-12.54963322200339,
-12.478204653666483,-12.406776085804342,-12.335347518489876,-12.263918951807185,-12.192490385853285,-12.12106182074009,-12.049633256596698,-11.978204693572032,-11.906776131837882,-11.835347571592413,-11.763919013064225,
-11.69249045651701,-11.621061902254956,-11.54963335062895,-11.478204802043754,-11.406776256966277,-11.335347715935102,-11.263919179571506,-11.192490648592129,-11.121062123823624,-11.049633606219532,-10.978205096879742,
-10.90677659707295,-10.835348108262542,-10.763919632136448,-10.692491170641562,-10.621062726023437,-10.54963430087204,-10.478205898174503,-10.406777521375931,-10.33534917444949,-10.263920861977168,-10.19249258924285,
-10.12106436233954,-10.049636188292865,-9.9782080752033124,-9.9067800324099977,-9.8353520706791553,-9.763924202421066,-9.6924964419395572,-9.6210688057189166,-9.5496413127536659,-9.4782139849274056,-9.4067868474478029,
-9.33535992934576,-9.2639332640478429,-9.1925068900322113,-9.1210808515796771,-9.0496551996328627,-8.9782299927780844,-8.9068052983662618,-8.8353811937910525,-8.763957767944385,-8.69253512287172,-8.6211133756515785,
-8.54969266052632,-8.4782731313134612,-8.4068549641293782,-8.3354383604596975,-8.264023550613,-8.1926107975968652,-8.12120040145727,-8.04979270412417,-7.97838809480757,-7.90698701598915,-7.8355899700549756,
-7.764197526614204,-7.69281033054747,-7.6214291108261873,-7.550054690140545,-7.4786879953691221,-7.4073300689168811,-7.335982080940564,-7.2646453424712369,-7.1933213194328385,-7.1220116475430171,-7.050718148068448,
-6.97944284439114,-6.9081879793252607,-6.836956033105789,-6.765749741951228,-6.6945721170828607,-6.6234264640631171,-6.5523164022958351,-6.481245884512111,-6.41021921604748,-6.3392410736998945,-6.2683165239439207,
-6.1974510402652072,-6.1266505193712018,-6.0559212960295978,-5.9852701562855959,-5.9147043488130144,-5.8442315941627179,-5.7738600916850054,-5.7035985239202835,-5.6334560582745778,-5.5634423458228017,-5.4935675171128686,
-5.4238421748771852,-5.35427738359413,-5.28488465588022,-5.2156759357328477,-5.14666357868317,-5.0778603289578736,-5.0092792937864914,-4.9409339150268874,-4.872837938314647,-4.8050053799719068,-4.7374504919369311,
-4.6701877249972306,-4.6032316906256705,-4.5365971217309005,-4.4702988326402444,-4.4043516786351358,-4.338770515356341,-4.273570158388897,-4.2087653433252115,-4.1443706865896059,-4.0804006472891521,-4.0168694903345648,
-3.953791251051586,-3.8911797014784257,-3.8290483185188182,-3.7674102540937131,-3.7062783074079961,-3.6456648994223664,-3.5855820495949984,-3.5260413549331973,-3.4670539713722319,-3.4086305974771065,-3.3507814604433968,
-3.293516304355538,-3.2368443806452012,-3.1807744406786469,-3.125314730390198,-3.0704729868691563,-3.016256436799599,-2.9626717966463256,-2.9097252744757784,-2.8574225732978293,-2.8057688958128049,-2.7547689504478727,
-2.7044269585677769,-2.6547466627467537,-2.6057313359911665,-2.5573837918057714,-2.5097063950005487,-2.462701073139463,-2.4163693285373613,-2.3707122507162919,-2.3257305292378079,-2.2814244668331809,-2.2377939927588586,
-2.1948386763098795,-2.1525577404292529,-2.1109500753565071,-2.070014252263626,-2.029748536831451,-1.9901509027242694,-1.9512190449247278,-1.9129503928954112,-1.8753421235373773,-1.838391173919641,-1.802094253757087,
-1.7664478576174933,-1.7314482768413566,-1.6970916111609571,-1.6633737800076403,-1.6302905334986098,-1.5978374630966479,-1.5660100119380911,-1.5348034848261347,-1.5042130578880941,-1.4742337878966554,-1.4448606212563975,
-1.4160884026579661,-1.3879118834032636,-1.3603257294058682,-1.3333245288716347,-1.3069027996650768,-1.281054996367663,-1.2557755170346281,-1.2310587096572814,-1.2068988783380923,-1.1832902891860977,-1.1602271759403453,
-1.1377037453292394,-1.1157141821737338,-1.0942526542423725,-1.0733133168661879,-1.0528903173214474,-1.0329777989881888,-1.0135699052924134,-0.994660783439711,-0.97624458794797175,-0.95831548398672017,-0.94086765053045218,
-0.92389528333321,-0.90739259773146141,-0.8913538312821806,-0.87577324624285013,-0.86064513189992042,-0.84596380675208094,-0.83172362055450821,-0.81791895623006861,-0.804544231653264,-0.79159390131252494,-0.7790624578562626,
-0.766944433527914,-0.75523440149502541,-0.74392697707724575,-0.7330168188779197,-0.7224986298238012,-0.71236715811723716,-0.70261719810500667,-0.69324359106783717,-0.68424122593446579,-0.67560503992395793,-0.66733001911984857,
-0.6594111989795256,-0.65184366478213862,-0.64462255201817564,-0.63774304672372462,-0.63120038576230741,-0.62498985705705068,-0.61910679977584315,-0.61354660447201137,-0.60830471318293888,-0.60337661948894616,-0.59875786853464863,
-0.5944440570149091,-0.59043083312741029,-0.5867138964937787,-0.5832889980511069,-0.58015193991563629,-0.57729857522028027,-0.57472480792759406,-0.57242659261972118,-0.57039993426677349,-0.5686408879750392,-0.567145558716341,
-0.56591010103980877,-0.56493071876727041,-0.56420366467340366,-0.56372524015174119,-0.56349179486756484,-0.56349972639867729,-0.563745479864988,-0.56422554754780729,-0.564936468499695,-0.56587482814567092,-0.56703725787655124,
-0.56842043463513747,-0.5700210804959488,-0.57183596223914934,-0.57386189091929418,-0.57609572142947918,-0.57853435206145365,-0.58117472406222381,-0.584013821187646,-0.58704866925348276,-0.59027633568437,-0.59369392906111662,
-0.597298598666738,-0.60108753403159831,-0.60505796447802085,-0.60920715866469877,-0.61353242413122511,-0.61803110684303941,-0.62270059073707185,-0.62753829726834776,-0.63254168495780161,-0.63770824894153266,-0.64303552052172,
-0.64852106671940257,-0.6541624898293148,-0.65995742697695658,-0.66590354967806509,-0.67199856340064412,-0.678240207129695,-0.68462625293478574,-0.69115450554058033,-0.697822801900448,-0.70462901077325446,-0.71157103230343888},
new double[]{
-15.435651808546295,-15.373151808548178,-15.310651808550309,-15.248151808552725,-15.185651808555463,-15.123151808558566,-15.060651808562081,-14.998151808566064,-14.935651808570578,-14.873151808575692,-14.810651808581488,-14.748151808588055,
-14.685651808595498,-14.623151808603931,-14.560651808613486,-14.498151808624314,-14.435651808636584,-14.373151808650487,-14.310651808666242,-14.248151808684094,-14.185651808704323,-14.123151808727247,-14.060651808753221,
-13.998151808782655,-13.935651808816008,-13.8731518088538,-13.810651808896626,-13.748151808945153,-13.685651809000142,-13.623151809062453,-13.560651809133059,-13.498151809213068,-13.43565180930373,-13.373151809406462,
-13.310651809522874,-13.248151809654786,-13.185651809804261,-13.123151809973638,-13.060651810165568,-12.998151810383055,-12.935651810629498,-12.873151810908754,-12.810651811225194,-12.748151811583766,-12.685651811990082,
-12.623151812450498,-12.560651812972219,-12.498151813563405,-12.435651814233307,-12.373151814992404,-12.310651815852575,-12.248151816827276,-12.185651817931756,-12.123151819183295,-12.060651820601477,-11.998151822208486,
-11.935651824029465,-11.873151826092903,-11.810651828431086,-11.748151831080591,-11.685651834082874,-11.623151837484903,-11.560651841339906,-11.498151845708195,-11.435651850658109,-11.373151856267095,-11.3106518626229,
-11.248151869824964,-11.185651877985963,-11.123151887233574,-11.060651897712473,-10.998151909586603,-10.935651923041728,-10.873151938288348,-10.810651955564993,-10.748151975141939,-10.685651997325454,-10.62315202246258,
-10.560652050946562,-10.498152083222992,-10.435652119796787,-10.373152161240082,-10.310652208201173,-10.248152261414658,-10.185652321712915,-10.123152390039127,-10.060652467462015,-9.9981525551925436,-9.9356526546028459,
-9.8731527672476691,-9.8106528948886549,-9.7481530395218634,-9.6856532034089344,-9.6231533891124066,-9.560653599535712,-9.4981538379684718,-9.4356541081378129,-9.3731544142664767,-9.3106547611386219,-9.2481551541743521,
-9.1856555995140852,-9.1231561041140772,-9.060656675854565,-8.9981573236621628,-8.9356580576483839,-8.8731588892663815,-8.810659831488282,-8.7481608990057627,-8.6856621084568726,-8.62316347868248,-8.5606650310161179,
-8.498166789611469,-8.4356687818122911,-8.3731710385700548,-8.31067359491531,-8.2481764904893975,-8.18567977014394,-8.12318348461635,-8.0606876912905054,-7.9981924550527594,-7.9356978492544572,-7.8732039567933176,
-7.8107108713272675,-7.7482186986355588,-7.685727558143415,-7.6232375846278266,-7.5607489301236095,-7.4982617660503124,-7.435776285582044,-7.3732927062838,-7.3108112730392341,-7.2483322612962073,-7.1858559806575659,
-7.1233827788456727,-7.0609130460699125,-6.9984472198268595,-6.9359857901628708,-6.8735293054284563,-6.811078378552879,-6.7486336938659015,-6.6861960144913919,-6.6237661903344982,-6.56134516668032,-6.4989339934172641,
-6.4365338348926056,-6.3741459804011473,-6.3117718553001909,-6.2494130327353785,-6.1870712459523407,-6.12474840115856,-6.0624465908884773,-6.00016810781284,-5.9379154589206822,-5.8756913799894086,-5.8134988502453746,
-5.7513411071044436,-5.689221660869487,-5.62714430924998,-5.5651131515580836,-5.5031326024261746,-5.4412074048830128,-5.3793426426199336,-5.3175437512749264,-5.2558165285613674,-5.1941671430698371,-5.1326021415759282,
-5.0711284546943789,-5.0097534007302427,-4.9484846875911,-4.8873304126404191,-4.8262990603908316,-4.7653994979571115,-4.7046409682116446,-4.6440330806098027,-4.5835857996783949,-4.52330943118688,-4.4632146060476865,
-4.4033122620183622,-4.3436136233038605,-4.2841301781815355,-4.2248736547939734,-4.1658559952751721,-4.107089328393517,-4.04858594091009,-3.9903582478630177,-3.9324187619975044,-3.8747800625670146,-3.8174547637335814,
-3.7604554827946517,-3.7037948084602874,-3.6474852693981039,-3.5915393032543625,-3.5359692263483251,-3.4807872042237005,-3.4260052232260843,-3.3716350632590122,-3.3176882718540157,-3.2641761396721805,-3.2111096775364811,
-3.1584995950759249,-3.1063562810445506,-3.0546897853608028,-3.0035098028960294,-2.9528256590249167,-2.9026462969358304,-2.8529802666852828,-2.8038357159682863,-2.7552203825651387,-2.7071415884153205,-2.6596062352606085,
-2.6126208017922612,-2.5661913422311198,-2.5203234862646777,-2.4750224402615171,-2.4302929896809236,-2.3861395025938874,-2.3425659342309908,-2.2995758324727764,-2.2571723441990184,-2.2153582224147521,-2.1741358340729149,
-2.1335071685158877,-2.09347384646107,-2.0540371294587474,-2.0151979297538873,-1.9769568204870764,-1.9393140461734648,-1.9022695334023638,-1.8658229017039096,-1.8299734745329912,-1.7947202903243595,-1.7600621135765078,
-1.7259974459254641,-1.6925245371730961,-1.6596413962378382,-1.6273458019989326,-1.5956353140082979,-1.5645072830470035,-1.5339588615060322,-1.5039870135735589,-1.4745885252133466,-1.4457600139210776,-1.4174979382474986,
-1.3897986070791646,-1.3626581886693139,-1.3360727194130244,-1.3100381123622755,-1.2845501654778773,-1.2596045696164537,-1.2351969162517631,-1.2113227049306334,-1.1879773504646736,-1.1651561898597187,-1.1428544889856578,
-1.1210674489899215,-1.0997902124584353,-1.0790178693283181,-1.058745462557007,-1.0389679935528291,-1.0196804273723286,-1.0008776976898937,-0.9825547115454184,-0.96470635387588222,-0.94732749183684128,-0.93041297891990526,
-0.91395765887231739,-0.89795636942477752,-0.88240394583364412,-0.867295224243623,-0.85262504487700974,-0.838388255055488,-0.82457971206041458,-0.811194285837429,-0.79822686155113154,-0.7856723419954621,-0.77352564986529815,
-0.76178172989466764,-0.75043555086684577,-0.7394821075014717,-0.72891642222368846,-0.71873354682017254,-0.70892856398677762,-0.69949658877238374,-0.690432769923397,-0.68173229113321143,-0.67339037220080233,-0.66540227010248487,
-0.65776327998073525,-0.65046873605383926,-0.6435140124499984,-0.63689452396939816,-0.63060572677761306,-0.62464311903359981,-0.61900224145541016,-0.61367867782663332,-0.60866805544646674,-0.60396604552619648,-0.599568363534765,
-0.59547076949599254,-0.5916690682399206,-0.58815910961064,-0.5849367886328769,-0.58199804563950952,-0.57933886636210208,-0.57695528198645341,-0.57484336917507461,-0.57299925005842511,-0.57141909219666254,-0.570099108513583,
-0.56903555720435439,-0.568224741618578,-0.567663010120144,-0.56734675592528,-0.56727241692013231,-0.56743647545915554,-0.56783545814553082,-0.56846593559477576,-0.569324522182657,-0.57040787577846175,-0.57171269746464037,
-0.5732357312437788,-0.57497376373381959,-0.57692362385240237,-0.57908218249115562,-0.58144635218073049,-0.5840130867473281,-0.58677938096143734,-0.58974227017946212,-0.59289882997888554,-0.59624617578758432,-0.59978146250787834,
-0.603501884135867,-0.60740467337657889,-0.61148710125543326,-0.61574647672648486,-0.62018014627790019,-0.62478549353508894,-0.62955993886189154,-0.6345009389602031,-0.63960598646839206,-0.64487260955885262,-0.65029837153501224,
-0.65588087042809506,-0.66161773859392758,-0.66750664231005508,-0.67354528137342107,-0.67973138869884875,-0.68606272991854833,-0.6925371029828602,-0.69915233776243091,-0.70590629565200813,-0.71279686917602691,-0.71982198159614907},
new double[]{
-13.977988297843041,-13.922432742297502,-13.86687718675314,-13.811321631210094,-13.755766075668518,-13.700210520128584,-13.644654964590487,-13.589099409054441,-13.533543853520687,-13.477988297989496,-13.422432742461169,-13.366877186936041,
-13.311321631414488,-13.255766075896931,-13.200210520383841,-13.144654964875741,-13.089099409373217,-13.033543853876926,-12.9779882983876,-12.922432742906057,-12.866877187433213,-12.811321631970088,-12.755766076517825,
-12.700210521077702,-12.644654965651142,-12.589099410239744,-12.533543854845286,-12.477988299469761,-12.422432744115394,-12.366877188784669,-12.311321633480365,-12.255766078205589,-12.200210522963809,-12.144654967758905,
-12.089099412595207,-12.033543857477563,-11.977988302411379,-11.922432747402707,-11.866877192458304,-11.811321637585722,-11.755766082793404,-11.70021052809078,-11.644654973488391,-11.589099418998019,-11.533543864632826,
-11.477988310407522,-11.422432756338548,-11.366877202444275,-11.311321648745237,-11.255766095264372,-11.200210542027323,-11.144654989062742,-11.089099436402648,-11.033543884082828,-10.977988332143266,-10.922432780628652,
-10.866877229588923,-10.811321679079889,-10.755766129163913,-10.700210579910694,-10.644655031398115,-10.589099483713216,-10.533543936953263,-10.477988391226955,-10.422432846655761,-10.366877303375429,-10.311321761537654,
-10.255766221311962,-10.2002106828878,-10.144655146476877,-10.089099612315778,-10.033544080668893,-9.9779885518316789,-9.9224330261343123,-9.866877503945771,-9.8113219856783829,-9.7557664717929384,-9.7002109628043627,
-9.6446554592880922,-9.5890999618871771,-9.5335444713202246,-9.4779889883902779,-9.4224335139947311,-9.3668780491364174,-9.31132259493599,-9.2557671526457632,-9.2002117236651735,-9.1446563095580586,-9.0891009120719612,
-9.0335455331597068,-8.9779901750034927,-8.9224348400418254,-8.8668795309996025,-8.8113242509217233,-8.75576900321064,-8.7002137916683076,-8.644658620543046,-8.5891034945818934,-8.5335484190890671,-8.477993399991286,
-8.4224384439107158,-8.3668835582464247,-8.31132875126537,-8.255774032203977,-8.2002194113815658,-8.1446649003269815,-8.0891105119199427,-8.0335562605488224,-7.9780021622867219,-7.9224482350879413,-7.8668944990071692,
-7.8113409764439723,-7.75578769241545,-7.7002346748602264,-7.64468195497729,-7.5891295676035853,-7.5335775516346253,-7.4780259504929054,-7.4224748126493241,-7.3669241922033759,-7.3113741495284463,-7.255824751989163,
-7.200276074738392,-7.144728201602212,-7.089181226061922,-7.03363525234295,-6.9780903966213605,-6.9225467883595391,-6.8670045717835295,-6.811463907515428,-6.7559249743751879,-6.7003879713671148,-6.64485311986727,
-6.5893206660288763,-6.5337908834236789,-6.47826407593793,-6.422740580942353,-6.3672207727559,-6.3117050664234631,-6.2561939218277951,-6.200687848155706,-6.1451874087381784,-6.0896932262831713,-6.0342059885186758,
-5.9787264542619,-5.9232554599282716,-5.8677939264912267,-5.8123428669004475,-5.7569033939622933,-5.7014767286816017,-5.646064209058852,-5.5906672993308062,-5.5352875996362689,-5.4799268560815024,-5.424586971172217,
-5.3692700145708985,-5.3139782341297632,-5.2587140671408132,-5.203480151735552,-5.1482793383579821,-5.0931147012257636,-5.0379895496860314,-4.9829074393645234,-4.9278721829996108,-4.8728878608466983,-4.8179588305335495,
-4.7630897362434492,-4.708285517101146,-4.6535514146360635,-4.5988929791988262,-4.544316075210447,-4.4898268851289211,-4.435431912025301,-4.3811379806706414,-4.3269522370463891,-4.2728821462037914,-4.2189354884124448,
-4.165120353554105,-4.1114451337350184,-4.0579185141080369,-4.00454946191439,-3.9513472137738508,-3.8983212612707985,-3.8454813349021086,-3.7928373864704295,-3.7403995700230785,-3.6881782214520982,-3.6361838368847863,
-3.584427050005996,-3.5329186084635271,-3.481669349515871,-3.4306901750873529,-3.3799920263992616,-3.3295858583469387,-3.2794826137920055,-3.2296931979360677,-3.1802284529374791,-3.1310991329261912,-3.0823158795636147,
-3.0338891982848963,-2.9858294353503503,-2.938146755821172,-2.8908511225622213,-2.8439522763618559,-2.7974597172456712,-2.75138268704785,-2.7057301532907365,-2.6605107944104871,-2.6157329863542924,-2.571404790562887,
-2.5275339433409543,-2.4841278466076946,-2.4411935600102979,-2.3987377943744392,-2.3567669064581689,-2.3152868949687688,-2.2743033977962481,-2.2338216904121428,-2.19384668537818,-2.1543829329060542,-2.1154346224070797,
-2.0770055849687132,-2.0390992966938555,-2.0017188828383934,-1.9648671226825487,-1.9285464550722069,-1.8927589845674684,-1.8575064881370897,-1.8227904223392732,-1.7886119309312993,-1.7549718528527825,-1.7218707305297818,
-1.6893088184495975,-1.657286091958774,-1.6258022562395926,-1.594856755423117,-1.5644487817996546,-1.5345772850902668,-1.505240981745688,-1.4764383642416932,-1.4481677103425394,-1.42042709230663,-1.3932143860109556,
-1.3665272799731811,-1.340363284252452,-1.3147197392120738,-1.2895938241292015,-1.2649825656385261,-1.2408828459986869,-1.2172914111717685,-1.1942048787077493,-1.1716197454271748,-1.1495323948966267,-1.1279391046927514,
-1.1068360534517072,-1.086219327701895,-1.0660849284787435,-1.0464287777211532,-1.0272467244499475,-1.0085345507293526,-0.990287977413129,-0.97250266967751353,-0.955174242343603,-0.9382982649922309,-0.92187026687474583,
-0.90588574162342272,-0.89034015176550108,-0.87522893304507521,-0.86054749855725154,-0.84629124269914158,-0.83245554494238339,-0.81903577343197975,-0.806027288416308,-0.79342544551320227,-0.78122559881703313,-0.76942310385171342,
-0.75801332037454638,-0.74699161503580713,-0.736353363898904,-0.72609395482591765,-0.71620878973324942,-0.70669328672204323,-0.69754288208795878,-0.68875303221479633,-0.6803192153563743,-0.67223693331096857,-0.66450171299252259,
-0.65710910790273036,-0.65005469950799533,-0.64333409852515377,-0.63694294611975011,-0.63087691502053622,-0.6251317105537636,-0.61970307160072313,-0.61458677148188268,-0.60977861877086215,-0.60527445804138313,-0.601070170550219,
-0.597161674859076,-0.59354492739822551,-0.590215922974614,-0.58717069522707754,-0.58440531703118925,-0.58191590085617861,-0.579698599076268,-0.577749604238681,-0.57606514929049613,-0.57464150776642642,-0.57347499393953316,
-0.57256196293679451,-0.571898810821377,-0.57148197464338135,-0.57130793246076261,-0.57137320333205333,-0.571674347282451,-0.572207965244767,-0.57297069897666775,-0.57395923095558021,-0.57517028425257544,-0.57660062238648246,
-0.57824704915943592,-0.58010640847500428,-0.58217558413999337,-0.58445149965097676,-0.58693111796654918,-0.589611441266261,-0.59248951069714373,-0.59556240610869693,-0.59882724577716351,-0.60228118611988679,-0.60592142140049943,
-0.60974518342566331,-0.6137497412340428,-0.61793240077816147,-0.62229050459976121,-0.62682143149925118,-0.63152259619980688,-0.63639144900665023,-0.641425475462015,-0.64662219599627779,-0.65197916557570856,-0.65749397334727278,
-0.66316424228089321,-0.66898762880955953,-0.67496182246765313,-0.681084545527833,-0.68735355263681319,-0.69376663045034093,-0.70032159726766918,-0.70701630266580184,-0.71384862713377162,-0.720816481707197,-0.727917807603352},
new double[]{
-12.788293553622166,-12.738293553663018,-12.688293553708167,-12.638293553758064,-12.588293553813209,-12.538293553874153,-12.488293553941507,-12.438293554015944,-12.38829355409821,-12.338293554189129,-12.288293554289609,-12.238293554400656,
-12.188293554523384,-12.138293554659018,-12.088293554808915,-12.03829355497458,-11.988293555157666,-11.938293555360009,-11.888293555583632,-11.838293555830774,-11.788293556103907,-11.738293556405766,-11.688293556739373,
-11.638293557108064,-11.588293557515531,-11.538293557965853,-11.488293558463534,-11.438293559013557,-11.388293559621427,-11.338293560293227,-11.28829356103568,-11.238293561856219,-11.188293562763054,-11.138293563765261,
-11.08829356487287,-11.03829356609697,-10.988293567449809,-10.938293568944927,-10.888293570597286,-10.838293572423426,-10.788293574441624,-10.738293576672076,-10.688293579137108,-10.638293581861387,-10.588293584872183,
-10.538293588199625,-10.488293591877017,-10.438293595941163,-10.388293600432737,-10.338293605396695,-10.288293610882715,-10.238293616945702,-10.188293623646338,-10.138293631051683,-10.088293639235852,-10.038293648280753,
-9.9882936582769126,-9.93829366932437,-9.8882936815336926,-9.8382936950270725,-9.7882937099395537,-9.7382937264203822,-9.6882937446345,-9.638293764764196,-9.5882937870109277,-9.5382938115973417,-9.4882938387694971,
-9.4382938687993345,-9.3882939019873888,-9.3382939386658013,-9.2882939792016419,-9.2382940240005844,-9.1882940735109635,-9.1382941282282637,-9.0882941887000666,-9.0382942555315466,-8.988294329391513,-8.9382944110191023,
-8.8882945012311776,-8.8382946009304959,-8.7882947111147427,-8.738294832886508,-8.688294967464314,-8.6382951161948061,-8.58829528056622,-8.5382954622232532,-8.48829566298353,-8.4382958848557585,-8.388296130059814,
-8.3382964010489378,-8.2882967005342429,-8.2382970315118165,-8.1882973972926489,-8.1382978015357175,-8.0882982482845183,-8.0382987420074574,-7.9882992876424481,-7.938299890646201,-7.888300557048666,-7.8383012935131795,
-7.7883021074029024,-7.7383030068542107,-7.6883040008577535,-7.6383050993479831,-7.58830631330203,-7.5383076548488885,-7.488309137389991,-7.4383107757323286,-7.3883125862354211,-7.3383145869735609,-7.2883167979148915,
-7.2383192411190418,-7.1883219409552126,-7.1383249243427889,-7.0883282210167629,-7.0383318638204688,-6.9883358890283747,-6.9383403367019278,-6.8883452510817573,-6.838350681019814,-6.788356680455383,-6.7383633089392418,
-6.6883706322106393,-6.6383787228321633,-6.5883876608880039,-6.5383975347516028,-6.4884084419291383,-6.4384204899858446,-6.3884337975626941,-6.3384484954915221,-6.2884647280172929,-6.23848265413677,-6.1885024490634857,
-6.1385243058295087,-6.0885484370351186,-6.0385750767580815,-5.9886044826348019,-5.9386369381261321,-5.88867275498109,-5.8387122759121572,-5.7887558774960794,-5.73880397331432,-5.6888570173473241,-5.6389155076366535,
-5.5889799902286885,-5.5390510634130852,-5.4891293822683442,-5.439215663525772,-5.389310690761679,-5.33941531992594,-5.2895304852128886,-5.2396572052779922,-5.1897965898008165,-5.1399498463914224,-5.0901182878335058,
-5.0403033396533781,-4.9905065479991748,-4.9407295878096491,-4.8909742712464235,-4.8412425563578374,-4.7915365559364851,-4.7418585465263012,-4.6922109775287391,-4.6425964803512327,-4.5930178775348995,-4.5434781917924054,
-4.4939806548812591,-4.444528716232603,-4.395126051250994,-4.3457765691968584,-4.2964844205603754,-4.2472540038336382,-4.1980899715871294,-4.1489972357570331,-4.0999809720515934,-4.0510466233878759,-4.0021999022747492,
-3.9534467920638217,-3.9047935469972841,-3.8562466909902158,-3.8078130150946703,-3.7594995736037724,-3.711313678765916,-3.6632628940918126,-3.6153550262504135,-3.5675981155633956,-3.5200004251217476,-3.4725704285618262,
-3.4253167965517637,-3.3782483820521931,-3.3313742044275876,-3.2847034324959572,-3.2382453666150317,-3.1920094199121678,-3.1460050987729988,-3.1002419827101448,-3.0547297037380576,-3.0094779253832806,-2.9644963214609987,
-2.9197945547488153,-2.8753822556872297,-2.8312690012333968,-2.7874642939905168,-2.7439775417297723,-2.7008180374151758,-2.657994939834246,-2.6155172549291423,-2.5733938179140248,-2.5316332762550164,-2.4902440735794751,
-2.4492344345714132,-2.4086123509000212,-2.3683855682184523,-2.3285615742604575,-2.2891475880532046,-2.2501505502557673,-2.2115771146244332,-2.173433640598168,-2.1357261869903779,-2.0984605067665552,-2.0616420428814743,
-2.0252759251443861,-1.9893669680760717,-1.9539196697177239,-1.9189382113483546,-1.8844264580647765,-1.8503879601761528,-1.8168259553635993,-1.7837433715543372,-1.7511428304593819,-1.7190266517236637,-1.6873968576377987,
-1.6562551783613781,-1.6256030576086122,-1.5954416587483984,-1.5657718712723368,-1.5365943175858732,-1.5079093600795435,-1.4797171084392304,-1.4520174271563593,-1.4248099432010446,-1.398094053823336,-1.3718689344498469,
-1.3461335466452122,-1.3208866461099396,-1.2961267906883238,-1.2718523483621504,-1.2480615052079034,-1.2247522732971403,-1.2019224985215491,-1.1795698683259939,-1.1576919193345632,-1.1362860448562484,-1.1153495022584257,
-1.0948794201977525,-1.0748728056994623,-1.0553265510773082,-1.0362374406876032,-1.0176021575119105,-0.99941728956396569,-0.98167933611736291,-0.96438471375141122,-0.94752976221337071,-0.931110750096011,-0.91512388033010594,
-0.89956529549208208,-0.88443108292758821,-0.86971727969224832,-0.855419877311297,-0.84153482636019517,-0.82805804086866674,-0.814985402550906,-0.80231276486497,-0.79003595690460193,-0.77815078712692742,-0.76665304691963376,
-0.75553851401137739,-0.74480295572927824,-0.73444213210744924,-0.724451798850574,-0.71482771015659341,-0.70556562140259194,-0.696661291697987,-0.68811048630912564,-0.67990897895937563,-0.67205255400877617,-0.66453700851727482,
-0.65735815419553623,-0.6505118192472511,-0.64399385010681887,-0.63780011307620854,-0.631926495864735,-0.6263689090354092,-0.621123287361445,-0.616185591096424,-0.61155180716153335,-0.60721795025320646,-0.603180063874412,
-0.59943422129274371,-0.59597652642837762,-0.592803114674875,-0.589910153655717,-0.58729384391937078,-0.58495041957559935,-0.58287614887563832,-0.58106733473877714,-0.579520315227798,-0.57823146397564229,-0.57719719056558938,
-0.57641394086715636,-0.57587819732984369,-0.575586479236778,-0.57553534292022512,-0.57572138194087452,-0.57614122723272276,-0.57679154721531534,-0.57766904787503637,-0.578770472817071,-0.58009260328959866,-0.58163225818171749,
-0.58338629399653319,-0.58535160480079285,-0.58752512215238417,-0.58990381500696665,-0.59248468960494849,-0.59526478933996962,-0.59824119461000425,-0.60141102265214808,-0.60477142736210521,-0.608319599099352,-0.61205276447890511,
-0.6159681861505848,-0.62006316256662175,-0.62433502773841754,-0.62878115098323417,-0.63339893666154778,-0.63818582390577416,-0.64313928634103279,-0.64825683179859217,-0.65353600202260287,-0.65897437237069945,-0.66456955150902175,
-0.67031918110218114,-0.67622093549866968,-0.68227252141218742,-0.6884716775993367,-0.69481617453411293,-0.70130381407959519,-0.70793242915722365,-0.71469988341402768,-0.72160407088814926,-0.72864291567299166,-0.73581437158029872},
new double[]{
-11.79886398866876,-11.753409443349741,-11.707954898043621,-11.662500352751625,-11.6170458074751,-11.571591262215515,-11.526136716974486,-11.480682171753777,-11.435227626555323,-11.389773081381241,-11.34431853623385,-11.29886399111569,
-11.253409446029544,-11.207954900978459,-11.16250035596577,-11.117045810995133,-11.071591266070548,-11.026136721196401,-10.98068217637749,-10.935227631619071,-10.889773086926903,-10.844318542307288,-10.798863997767136,
-10.753409453314003,-10.707954908956175,-10.662500364702723,-10.617045820563575,-10.571591276549615,-10.526136732672754,-10.480682188946039,-10.435227645383762,-10.38977310200157,-10.344318558816603,-10.298864015847631,
-10.253409473115209,-10.207954930641849,-10.162500388452209,-10.117045846573285,-10.071591305034652,-10.026136763868692,-9.9806822231108736,-9.9352276828000416,-9.889773142978731,-9.844318603693532,-9.798864064995465,
-9.7534095269404073,-9.70795498958955,-9.6625004530099172,-9.6170459172749,-9.5715913824648826,-9.5261368486678961,-9.4806823159803457,-9.43522778450782,-9.3897732543659469,-9.3443187256813633,-9.2988641985927583,
-9.253409673252019,-9.2079551498254837,-9.1625006284953265,-9.117046109461052,-9.0715915929411572,-9.0261370791749265,-8.980682568424422,-8.9352280609766375,-8.8897735571458849,-8.8443190572763868,-8.798864561745118,
-8.7534100709649341,-8.7079555853879747,-8.6625011055094046,-8.6170466318715153,-8.5715921650681981,-8.5261377057498589,-8.4806832546287954,-8.4352288124850876,-8.3897743801730424,-8.34431995862826,-8.2988655488753658,
-8.253411152036481,-8.2079567693404982,-8.1625024021332333,-8.11704805188856,-8.0715937202205712,-8.0261394088969418,-7.980685119853514,-7.9352308552103015,-7.8897766172890025,-7.8443224086321814,-7.798868232024267,
-7.7534140905145481,-7.7079599874423579,-7.662505926464644,-7.6170519115861586,-7.5715979471925126,-7.5261440380863709,-7.4806901895270759,-7.4352364072740329,-7.3897826976342094,-7.3443290675141331,-7.29887552447682,
-7.2534220768040942,-7.2079687335647993,-7.1625155046894777,-7.1170624010520962,-7.0716094345595053,-7.0261566182493462,-6.980703966397189,-6.9352514946337873,-6.8897992200733675,-6.8443471614539986,-6.7988953392911666,
-6.7534437760457573,-6.7079924963078161,-6.6625415269975043,-6.6170908975848652,-6.5716406403301093,-6.5261907905462948,-6.4807413868864554,-6.4352924716573865,-6.3898440911624919,-6.3443962960763178,-6.2989491418535906,
-6.2535026891758436,-6.2080570044389347,-6.1626121602850521,-6.11716823618308,-6.0717253190614846,-6.0262835039982381,-5.9808428949725778,-5.9354036056837991,-5.8899657604426237,-5.844529495141062,-5.7990949583070943,
-5.75366231225088,-5.7082317343096278,-5.6628034181986546,-5.617377575476568,-5.5719544371329155,-5.5265342553070154,-5.4811173051470377,-5.43570388681875,-5.3902943276736028,-5.3448889845860563,-5.2994882464702524,
-5.2540925369861373,-5.208702317445205,-5.1633180899258226,-5.1179404006078766,-5.0725698433360344,-5.0272070634203283,-4.9818527616819832,-4.9365076987513987,-4.8911726996239793,-4.8458486584780163,-4.8005365437570777,
-4.7552374035173228,-4.7099523710378488,-4.6646826706895457,-4.6194296240549946,-4.5741946562887561,-4.5289793027038261,-4.4837852155662965,-4.4386141710761855,-4.39346807650816,-4.34834897748144,-4.30325906532361,
-4.2582006844884468,-4.2131763399832529,-4.1681887047566253,-4.1232406269932076,-4.0783351372578345,-4.0334754554276637,-3.988664997347529,-3.9439073811408889,-3.8992064331065297,-3.8545661931296715,-3.8099909195353843,
-3.7654850933123747,-3.7210534216362583,-3.6767008406234614,-3.6324325172499332,-3.5882538503728822,-3.5441704707987953,-3.5001882403470219,-3.4563132498651363,-3.4125518161601018,-3.3689104778178178,-3.32539598989285,
-3.2820153174599214,-3.2387756280288724,-3.1956842828352228,-3.1527488270289554,-3.1099769787945775,-3.0673766174457047,-3.0249557705472427,-2.9827226001275018,-2.9406853880511856,-2.8988525206319942,-2.857232472570443,
-2.8158337903083837,-2.7746650748964861,-2.7337349644746016,-2.6930521164674053,-2.6526251895990387,-2.6124628258306206,-2.5725736323235222,-2.5329661635292395,-2.4936489035036482,-2.4546302485394036,-2.4159184902054309,
-2.3775217988768707,-2.3394482078326408,-2.3017055979910568,-2.2643016833468379,-2.2272439971653997,-2.19053987898276,-2.1541964624516958,-2.1182206640671652,-2.0826191727964609,-2.0473984406322394,-2.0125646740795022,
-1.9781238265808678,-1.9440815918781385,-1.9104433983022333,-1.8772144039781105,-1.8443994929263228,-1.8120032720383765,-1.7800300688990995,-1.7484839304257527,-1.7173686222906603,-1.6866876290916537,-1.6564441552326028,
-1.626641126474748,-1.5972811921183936,-1.5683667277737639,-1.5398998386794325,-1.5118823635266709,-1.4843158787482971,-1.4572017032311158,-1.4305409034117878,-1.4043342987169114,-1.3785824673092431,-1.3532857521032577,
-1.3284442670146683,-1.3040579034100253,-1.2801263367241134,-1.2566490332145059,-1.2336252568243198,-1.2110540761259208,-1.188934371320046,-1.1672648412665032,-1.1460440105243022,-1.1252702363807212,-1.1049417158504302,
-1.0850564926273607,-1.0656124639735329,-1.0466073875305075,-1.0280388880405338,-1.0099044639657986,-0.99220149399545343,-0.97492724343130122,-0.95807887044415785,-0.9416534321939779,-0.925647890807832,-0.91005911921076366,
-0.8948839068054254,-0.88011896499720421,-0.86576093256229869,-0.851806380856899,-0.83825181886626066,-0.825093698093043,-0.81232841728481608,-0.79995232700112417,-0.78796173402092917,-0.77635290559165382,-0.76512207352139816,
-0.75426543811621638,-0.74377917196462406,-0.73365942357174929,-0.72390232084575945,-0.71450397443937952,-0.70546048094948111,-0.69676792597785242,-0.68842238705637682,-0.68041993643993581,-0.67275664377042665,-0.66542857861533966,
-0.65843181288437924,-0.6517624231276381,-0.64541649271884538,-0.63939011392720813,-0.63367938988135708,-0.62828043642888587,-0.62318938389494227,-0.61840237874329806,-0.613915585143276,-0.60972518644586637,-0.60582738657231117,
-0.60221841131837461,-0.59889450957745682,-0.595851954485642,-0.593087044491705,-0.5905961043550283,-0.58837548607431134,-0.58642156974987936,-0.58473076438232485,-0.58329950861014079,-0.58212427138892875,-0.58120155261468864,
-0.58052788369362462,-0.58009982806082394,-0.57991398165009289,-0.57996697331716007,-0.58025546521838567,-0.58077615314704123,-0.58152576682915813,-0.58250107018086927,-0.58369886152910422,-0.58511597379743008,-0.58674927465876447,
-0.58859566665662433,-0.59065208729651431,-0.59291550910899171,-0.59538293968589462,-0.59805142169115411,-0.60091803284756173,-0.60397988590080653,-0.60723412856204206,-0.61067794343019743,-0.61430854789519018,-0.61812319402315763,
-0.62211916842477188,-0.62629379210766156,-0.6306444203139181,-0.63516844234362491,-0.63986328136530346,-0.6447263942141358,-0.64975527117878007,-0.65494743577756487,-0.66030044452480707,-0.66581188668796965,-0.671479384036339,
-0.67730059058187275,-0.68327319231283679,-0.6893949069208225,-0.69566348352170759,-0.7020767023710951,-0.70863237457474038,-0.71532834179445315,-0.72216247594993332,-0.72913267891698075,-0.7362368822224945,-0.743473046736658},
new double[]{
-10.963164040897928,-10.921497374613065,-10.879830708361382,-10.838164042145763,-10.796497375969341,-10.754830709835526,-10.713164043748016,-10.671497377710837,-10.629830711728364,-10.58816404580535,-10.546497379946962,-10.504830714158818,
-10.463164048447018,-10.421497382818203,-10.37983071727958,-10.338164051838989,-10.296497386504948,-10.254830721286718,-10.213164056194362,-10.171497391238821,-10.129830726431985,-10.088164061786774,-10.046497397317236,
-10.004830733038638,-9.963164068967572,-9.9214974051220732,-9.8798307415217455,-9.8381640781878961,-9.79649741514368,-9.7548307524142714,-9.7131640900270249,-9.6714974280116763,-9.6298307664005449,-9.5881641052287616,
-9.5464974445345039,-9.5048307843592728,-9.4631641247481717,-9.4214974657502264,-9.379830807418724,-9.3381641498115791,-9.29649749299174,-9.25483083702763,-9.21316418199361,-9.1714975279705122,-9.1298308750461885,
-9.0881642233161255,-9.0464975728841086,-9.0048309238629418,-8.9631642763752328,-8.9214976305542439,-8.87983098654482,-8.8381643445043885,-8.7964977046040627,-8.7548310670298228,-8.7131644319838113,-8.67149779968574,
-8.62983117037441,-8.5881645443093788,-8.5464979217727528,-8.50483130307116,-8.4631646885378728,-8.4214980785351212,-8.3798314734566173,-8.3381648737302836,-8.2964982798212272,-8.2548316922349763,-8.2131651115209863,
-8.1714985382764542,-8.1298319731504751,-8.0881654168485468,-8.0464988701374711,-8.004832333850679,-7.96316580889402,-7.9214992962520547,-7.8798327969948945,-7.8381663122856287,-7.7964998433884078,-7.7548333916772192,
-7.713166958645429,-7.671500545916154,-7.6298341552535272,-7.5881677885749514,-7.5465014479644079,-7.5048351356869256,-7.4631688542043015,-7.42150260619219,-7.3798363945586738,-7.3381702224644414,-7.29650409334472,
-7.2548380109331116,-7.213171979287492,-7.1715060028181536,-7.1298400863183957,-7.0881742349977586,-7.04650845451814,-7.0048427510330367,-6.963177131230192,-6.9215116023779242,-6.8798461723754727,-6.8381808498076948,
-6.7965156440044971,-6.7548505651053938,-6.7131856241296539,-6.6715208330524991,-6.6298562048878775,-6.5881917537783785,-6.5465274950928967,-6.5048634455327043,-6.4631996232466618,-6.4215360479563222,-6.3798727410917948,
-6.3382097259392634,-6.2965470278011537,-6.2548846741700173,-6.2132226949172855,-6.1715611224981428,-6.129899992173879,-6.0882393422531615,-6.046579214353824,-6.004919653686855,-5.9632607093644339,-5.9216024347339742,
-5.8799448877403249,-5.8382881313183983,-5.796632233818702,-5.7549772694684123,-5.713323318870839,-5.67167046954631,-5.6300188165177483,-5.5883684629444117,-5.5467195208075353,-5.5050721116518133,-5.4634263673869752,
-5.4217824311539067,-5.3801404582600965,-5.3385006171894132,-5.2968630906915442,-5.2552280769566533,-5.213595790881139,-5.171966465430617,-5.1303403531065115,-5.088717727522897,-5.0470988851004481,-5.0054841468845428,
-4.9638738604947479,-4.9222684022130085,-4.8806681792179569,-4.8390736319727319,-4.7974852367736762,-4.7559035084670827,-4.7143290033409491,-4.6727623221983388,-4.63120411361847,-4.5896550774110381,-4.5481159682685339,
-4.50658759962039,-4.4650708476916954,-4.4235666557679405,-4.3820760386657778,-4.3406000874081,-4.299139974099849,-4.2576969569988652,-4.2162723857737543,-4.1748677069382447,-4.1334844694487485,-4.09212433044895,
-4.0507890611421358,-4.0094805527687729,-3.9682008226634826,-3.9269520203621471,-3.8857364337264273,-3.8445564950495394,-3.8034147871037578,-3.7623140490868727,-3.7212571824217728,-3.6802472563605204,-3.6392875133418126,
-3.5983813740486177,-3.5575324421111567,-3.5167445083992641,-3.4760215548476419,-3.4353677577575974,-3.3947874905196413,-3.3542853257027878,-3.3138660364586334,-3.2735345971912548,-3.2332961834476803,-3.1931561709881509,
-3.1531201340005297,-3.1131938424290522,-3.0733832583940064,-3.0336945316858923,-2.9941339943249958,-2.9547081541850519,-2.9154236876876589,-2.8762874315822287,-2.8373063738343851,-2.7984876436537816,-2.7598385007001083,
-2.7213663235135677,-2.6830785972231346,-2.6449829005924359,-2.607086892468951,-2.5693982977074019,-2.53192489264258,-2.4946744901904219,-2.4576549246588124,-2.4208740363514059,-2.3843396560486503,-2.3480595894502185,
-2.3120416016622007,-2.276293401810749,-2.2408226278614132,-2.2056368317202488,-2.1707434646889574,-2.1361498633419504,-2.1018632358883282,-2.0678906490764928,-2.0342390156934815,-2.0009150827052653,-1.9679254200782255,
-1.935276410315939,-1.9029742387392914,-1.8710248845319204,-1.8394341125670763,-1.8082074660262901,-1.777350259814753,-1.7468675747731359,-1.716764252680693,-1.6870448920399794,-1.6577138446293527,-1.6287752128056494,
-1.6002328475360614,-1.5720903471352312,-1.5443510566810057,-1.5170180680800718,-1.4900942207528587,-1.4635821029056233,-1.4374840533564917,-1.4118021638814227,-1.3865382820455492,-1.361694014485116,-1.3372707306052578,
-1.3132695666591168,-1.2896914301742526,-1.2665370046929523,-1.243806754793847,-1.2215009313631873,-1.1996195770851961,-1.1781625321220757,-1.1571294399554843,-1.1365197533626017,-1.1163327405012511,-1.0965674910799184,
-1.0772229225899095,-1.0582977865782897,-1.0397906749416366,-1.0217000262220337,-1.0040241318880769,-0.98676114258500991,-0.96990907433938489,-0.95346581470491365,-0.93742912883736973,-0.92179666548757411,-0.90656596290260061,
-0.89173445462640366,-0.877299475192069,-0.86325826569884845,-0.84960797926803056,-0.83634568637254969,-0.82346838003602352,-0.8109729808976518,-0.79885634214009593,-0.78711525427809825,-0.77574644980619556,-0.76474660770442049,
-0.75411235780139307,-0.74384028499465926,-0.73392693332855419,-0.72436880993025,-0.71516238880499072,-0.70630411449182817,-0.69779040558145244,-0.68961765809795716,-0.68178224874659876,-0.67428053802980692,-0.66710887323386425,
-0.66026359128882772,-0.65374102150438129,-0.64753748818441992,-0.64164931312324636,-0.63607281798633675,-0.63080432657868191,-0.62584016700375533,-0.62117667371618057,-0.61681018947119526,-0.612737067174005,-0.60895367163212388,
-0.60545638121378065,-0.60224158941545058,-0.59930570634154645,-0.59664516009926627,-0.59425639811155817,-0.59213588835111775,-0.59028012049828493,-0.58868560702565842,-0.58734888421218467,-0.58626651308942979,-0.58543508032267444,
-0.58485119902941618,-0.584511509537799,-0.58441268008742431,-0.58455140747493672,-0.58492441764670855,-0.58552846624088339,-0.5863603390809754,-0.5874168526231508,-0.5886948543592585,-0.59019122317760719,-0.59190286968342576,
-0.59382673648088025,-0.59595979841845459,-0.59829906279944733,-0.60084156955926926,-0.60358439141117426,-0.60652463396199086,-0.60965943579937176,-0.61298596855201759,-0.61650143692428017,-0.62020307870649594,-0.62408816476234985,
-0.62815399899451774,-0.63239791828978853,-0.63681729244481944,-0.64140952407362861,-0.64617204849789012,-0.65110233362104641,-0.65619787978721666,-0.66145621962583512,-0.66687491788291542,-0.6724515712397986,-0.67818380812020551,
-0.6840692884863776,-0.6901057036250573,-0.69629077592402522,-0.70262225863987815,-0.70909793565770185,-0.71571562124326393,-0.72247315978832061,-0.72936842554960535,-0.73639932238204053,-0.74356378346668706,-0.75085977103392232},
new double[]{
-10.248115893392351,-10.20965435587369,-10.171192818430422,-10.132731281068574,-10.094269743794655,-10.055808206615698,-10.017346669539295,-9.9788851325736445,-9.940423595727605,-9.9019620590107387,-9.8635005224333749,-9.8250389860066676,
-9.7865774497426621,-9.74811591365437,-9.7096543777558413,-9.6711928420622471,-9.6327313065899745,-9.5942697713567213,-9.5558082363815977,-9.5173467016852449,-9.47888516728995,-9.44042363321979,-9.40196209950076,
-9.3635005661609352,-9.3250390332306363,-9.2865775007426112,-9.24811596873222,-9.2096544372376563,-9.1711929063001634,-9.1327313759642834,-9.0942698462781237,-9.0558083172936321,-9.0173467890669148,-8.9788852616585615,
-8.94042373513401,-8.9019622095639228,-8.86350068502462,-8.8250391615985233,-8.7865776393746415,-8.7481161184491,-8.7096545989257166,-8.671193080916602,-8.6327315645428353,-8.5942700499351758,-8.5558085372348387,
-8.5173470265943312,-8.4788855181783518,-8.4404240121647689,-8.4019625087456742,-8.3635010081285177,-8.3250395105373336,-8.2865780162140759,-8.2481165254200359,-8.2096550384374023,-8.1711935555709232,-8.1327320771497185,
-8.0942706035292158,-8.0558091350932646,-8.0173476722564043,-7.9788862154663187,-7.9404247652064859,-7.9019633219990384,-7.8635018864078567,-7.8250404590419027,-7.7865790405588271,-7.7481176316688609,-7.7096562331390182,
-7.6711948457976353,-7.6327334705392751,-7.5942721083300171,-7.55581076021318,-7.5173494273154873,-7.4788881108537444,-7.4404268121420332,-7.4019655325994842,-7.3635042737586822,-7.3250430372747237,-7.2865818249350154,
-7.248120638669846,-7.2096594805638032,-7.1711983528681085,-7.1327372580139254,-7.0942761986267442,-7.0558151775419073,-7.0173541978213718,-6.9788932627718179,-6.9404323759641962,-6.9019715412548388,-6.863510762808251,
-6.8250500451217277,-6.7865893930519245,-6.7481288118435545,-6.7096683071603751,-6.6712078851186334,-6.6327475523231918,-6.5942873159065183,-6.5558271835707957,-6.5173671636333719,-6.4789072650758319,-6.4404474975969794,
-6.40198787167003,-6.36352839860434,-6.3250690906120672,-6.2866099608800949,-6.2481510236476918,-6.2096922942903179,-6.1712337894100848,-6.1327755269333784,-6.0943175262162157,-6.0558598081579369,-6.0174023953238844,
-5.9789453120777711,-5.9404885847244993,-5.90203224166423,-5.8635763135585908,-5.8251208335099438,-5.7866658372547466,-5.7482113633720644,-5.7097574535084163,-5.6713041526201993,-5.6328515092350209,-5.5943995757333953,
-5.5559484086523261,-5.5174980690124311,-5.4790486226703674,-5.4406001406984563,-5.4021526997935032,-5.3637063827169769,-5.3252612787688394,-5.2868174842974645,-5.2483751032482475,-5.2099342477536794,-5.1714950387677971,
-5.133057606748145,-5.0946220923885157,-5.0561886474059605,-5.0177574353857377,-4.9793286326880386,-4.9409024294205732,-4.9024790304812331,-4.8640586566752839,-4.8256415459117088,-4.7872279544834884,-4.7488181584368041,
-4.7104124550342661,-4.6720111643174311,-4.6336146307739625,-4.5952232251148937,-4.5568373461674705,-4.51845742288909,-4.480083916507783,-4.4417173227946272,-4.4033581744732757,-4.365007043771608,-4.3266645451201615,
-4.2883313380016359,-4.2500081299552415,-4.211695679739087,-4.1733948006530719,-4.1351063640238941,-4.0968313028528458,-4.0585706156259036,-4.0203253702844064,-3.9820967083531573,-3.9438858492212576,-3.9056940945692284,
-3.8675228329341129,-3.8293735444022348,-3.7912478054171141,-3.7531472936877579,-3.7150737931801365,-3.6770291991721562,-3.6390155233498707,-3.6010348989200658,-3.5630895857117242,-3.5251819752362743,-3.487314595674015,
-3.4494901167516479,-3.41171135447361,-3.3739812756678,-3.3363030023044806,-3.2986798155456154,-3.261115159480704,-3.2236126445044193,-3.18617605029098,-3.1488093283203273,-3.1115166039118267,-3.0743021777223629,
-3.037170526667464,-3.0001263042263582,-2.9631743400947688,-2.9263196391526565,-2.8895673797181121,-2.8529229110630787,-2.8163917501715412,-2.7799795777262033,-2.7436922333154139,-2.7075357098581381,-2.671516147251038,
-2.6356398252481026,-2.5999131555897419,-2.5643426734046519,-2.5289350279140606,-2.4936969724740488,-2.4586353539974155,-2.4237571018019719,-2.3890692159371141,-2.35457875504496,-2.3202928238162137,-2.28621856010417,
-2.2523631217628681,-2.2187336732773129,-2.1853373722549025,-2.1521813558477243,-2.1192727271751943,-2.0866185418156982,-2.0542257944343905,-2.0221014056122355,-1.9902522089387285,-1.9586849384275837,-1.9274062163110881,
-1.8964225412648288,-1.8657402771101999,-1.8353656420375282,-1.8053046983878991,-1.7755633430268705,-1.7461472983383164,-1.7170621038616623,-1.6883131085908658,-1.6599054639486637,-1.6318441174449272,-1.6041338070234665,
-1.5767790560973392,-1.5497841692686831,-1.523153228725326,-1.4968900913029428,-1.4709983861983602,-1.44548151331674,-1.4203426422328223,-1.3955847117441724,-1.3712104299924541,-1.3472222751271228,-1.3236224964846122,
-1.3004131162550343,-1.2775959316076351,-1.2551725172457175,-1.2331442283614515,-1.2115122039609021,-1.1902773705297365,-1.1694404460103531,-1.1490019440616446,-1.1289621785731969,-1.1093212684064517,-1.0900791423361873,
-1.0712355441665957,-1.0527900379972253,-1.034742013615114,-1.017090691990542,-0.99983513085496967,-0.98297423034087961,-0.96650673866442549,-0.95043125783295324,-0.93474624936063866,-0.91945003997663965,-0.904540827311299,
-0.8900166855470506,-0.87587557102176261,-0.86211532777330546,-0.84873369301514512,-0.83572830253373964,-0.8230966959994469,-0.8108363221835494,-0.79894454407484272,-0.7874186438900439,-0.77625582797303039,-0.765453231578639,
-0.75500792353742208,-0.74491691079838973,-0.73517714284734981,-0.72578551599900343,-0.7167388775614616,-0.7080340298723099,-0.69966773420578432,-0.69163671455101072,-0.683937661261625,-0.67656723457741685,-0.66952206801893988,
-0.66279877165629852,-0.65639393525356526,-0.65030413129049658,-0.644525917863409,-0.63905584146724315,-0.63389043966099745,-0.62902624361883352,-0.624459780569273,-0.62018757612499276,-0.61620615650580646,-0.61251205065748071,
-0.609101792269084,-0.60597192169160474,-0.6031189877605988,-0.600539549525643,-0.59823017788937782,-0.59618745715891908,-0.59440798651240878,-0.59288838138345878,-0.59162527476621662,-0.5906153184437557,-0.58985518414245619,
-0.58934156461500831,-0.58907117465462322,-0.58904075204299677,-0.58924705843451941,-0.58968688017917692,-0.59035702908653431,-0.59125434313313963,-0.59237568711563038,-0.59371795325176757,-0.5952780617315665,-0.59705296122063189,
-0.59903962931775157,-0.60123507296873913,-0.60363632883846219,-0.60624046364293183,-0.60904457444327031,-0.61204578890331984,-0.61524126551259573,-0.61862819377623079,-0.62220379437350593,-0.625965319286502,-0.62991005190036065,
-0.63403530707658351,-0.63833843120075284,-0.642816802206001,-0.6474678295735129,-0.652288954311291,-0.65727764891237217,-0.66243141729363353,-0.66774779471628487,-0.67322434768909956,-0.67885867385539345,-0.68464840186472242,
-0.69059119123022661,-0.69668473217251392,-0.702926745450935,-0.70931498218306954,-0.71584722365320363,-0.72252128111054925,-0.72933499555792114,-0.7362862375315552,-0.74337290687272217,-0.75059293249176207,-0.7579442721251336},
new double[]{
-9.6295413522760942,-9.5938270686524287,-9.558112785183555,-9.5223985018809376,-9.4866842187568814,-9.45096993582461,-9.4152556530983222,-9.37954137059327,-9.3438270883258348,-9.3081128063136074,-9.2723985245754879,-9.2366842431317675,
-9.2009699620042458,-9.165255681216335,-9.12954140079318,-9.0938271207617873,-9.0581128411511624,-9.0223985619924623,-8.9866842833191463,-8.9509700051671519,-8.9152557275750812,-8.879541450584389,-8.8438271742396051,
-8.8081128985885488,-8.772398623682589,-8.73668434957689,-8.700970076330707,-8.6652558040076766,-8.6295415326761535,-8.5938272624095475,-8.5581129932867057,-8.5223987253923141,-8.4866844588173311,-8.4509701936594439,
-8.4152559300235747,-8.3795416680224175,-8.3438274077770078,-8.3081131494173412,-8.27239889308304,-8.2366846389240642,-8.2009703871014743,-8.16525613778826,-8.1295418911702164,-8.0938276474468935,-8.05811340683262,
-8.0223991695575876,-7.9866849358690368,-7.9509707060325114,-7.9152564803332135,-7.8795422590774651,-7.8438280425942652,-7.8081138312369704,-7.772399625385102,-7.7366854254462787,-7.700971231858297,-7.6652570450913684,
-7.6295428656505155,-7.5938286940781534,-7.5581145309568516,-7.5224003769123149,-7.4866862326165693,-7.450972098791393,-7.4152579762120041,-7.3795438657110086,-7.3438297681826565,-7.3081156845873991,-7.2724016159567917,
-7.236687563398756,-7.2009735281032343,-7.16525951134826,-7.1295455145064741,-7.0938315390521351,-7.058117586568633,-7.0224036587565681,-6.9866897574424263,-6.9509758845878968,-6.915262042299875,-6.8795482328412056,
-6.8438344586422293,-6.80812072231317,-6.7724070266574481,-6.736693374685971,-6.7009797696324878,-6.6652662149700692,-6.629552714428816,-6.5938392720148693,-6.5581258920308239,-6.5224125790976579,-6.4866993381782745,
-6.4509861746027859,-6.4152730940956619,-6.3795601028048869,-6.3438472073332646,-6.3081344147720335,-6.272421732736964,-6.2367091694071171,-6.20099673356646,-6.1652844346485471,-6.1295722827845012,-6.0938602888545175,
-6.0581484645431685,-6.0224368223987739,-5.9867253758971426,-5.9510141395099962,-5.9153031287784277,-5.8795923603917482,-5.8438818522721254,-5.8081716236654346,-5.7724616952387633,-5.7367520891850621,-5.7010428293354547,
-5.6653339412797656,-5.6296254524958487,-5.5939173924883585,-5.5582097929376477,-5.5225026878594958,-5.4867961137764754,-5.451090109901755,-5.4153847183362451,-5.3796799842800249,-5.3439759562590643,-5.3082726863683085,
-5.27257023053229,-5.2368686487844824,-5.2011680055667027,-5.165468370049954,-5.1297698164781886,-5.094072424536555,-5.0583762797458132,-5.0226814738846723,-4.986988105441946,-4.9512962801005127,-4.9156061112551814,
-4.8799177205667039,-4.8442312385542738,-4.8085468052290006,-4.772864570770972,-4.7371846962526423,-4.7015073544114356,-4.6658327304745821,-4.6301610230393351,-4.5944924450118725,-4.5588272246082884,-4.5231656064212462,
-4.4875078525559573,-4.4518542438392732,-4.4162050811057947,-4.3805606865649693,-4.3449214052532446,-4.3092876065753787,-4.2736596859390623,-4.2380380664869746,-4.2024232009304061,-4.1668155734884831,-4.1312157019369238,
-4.09562413977011,-4.0600414784800369,-4.0244683499554217,-3.9889054290039376,-3.9533534360000937,-3.9178131396608284,-3.8822853599502696,-3.8467709711144749,-3.811270904846185,-3.7757861535787511,-3.7403177739074382,
-3.704866890135202,-3.6694346979388439,-3.6340224681501474,-3.5986315506451549,-3.5632633783332235,-3.527919471235855,-3.492601440643555,-3.4573109933371509,-3.4220499358581047,-3.3868201788103871,-3.3516237411744814,
-3.3164627546120671,-3.2813394677379089,-3.2462562503335022,-3.211215597475086,-3.1762201335468205,-3.1412726161082127,-3.1063759395833346,-3.071533138738042,-3.0367473919102888,-3.00202202395781,-2.9673605088869008,
-2.9327664721258464,-2.8982436924067296,-2.863796103219912,-2.8294277938064725,-2.7951430096553058,-2.7609461524734327,-2.7268417796003663,-2.6928346028401164,-2.6589294866875486,-2.625131445929394,-2.5914456426041026,
-2.5578773823090208,-2.5244321098479232,-2.4911154042167318,-2.45793297293025,-2.4248906456978654,-2.3919943674613569,-2.3592501908131309,-2.3266642678183316,-2.2942428412692411,-2.2619922354051778,-2.2299188461355945,
-2.1980291308082731,-2.1663295975683092,-2.1348267943569543,-2.1035272976022861,-2.072437700656065,-2.0415646020330134,-2.0109145935100616,-1.980494248143875,-1.9503101082651786,-1.9203686735080585,-1.8906763889315561,
-1.8612396332894878,-1.8320647075025793,-1.8031578233847161,-1.7745250926724192,-1.7461725164036241,-1.718105974688491,-1.6903312169113822,-1.6628538523993328,-1.6356793415883963,-1.6088129877151802,-1.5822599290567794,
-1.5560251317381986,-1.5301133831222653,-1.5045292857930339,-1.4792772521397839,-1.454361499544965,-1.4297860461758658,-1.4055547073764059,-1.3816710926522848,-1.3581386032397969,-1.3349604302459301,-1.3121395533449425,
-1.2896787400144263,-1.2675805452919537,-1.2458473120317348,-1.2244811716393043,-1.2034840452610796,-1.1828576454047037,-1.1626034779653649,-1.1427228446327837,-1.1232168456532603,-1.1040863829210419,-1.0853321633733244,
-1.0669547026634016,-1.0489543290868173,-1.0313311877358313,-1.0140852448580993,-0.99721629239612064,-0.98072395268477,-0.96460768328504309,-0.94886678193302842,-0.93350039158403619,-0.91850750553277449,-0.90388697259144413,
-0.8896375023086216,-0.87575767021280526,-0.86224592306550485,-0.84910058410975386,-0.83631985830091127,-0.82390183750758894,-0.81184450567148825,-0.80014574391585314,-0.78880333559313864,-0.77781497126335863,-0.76717825359540348,
-0.75689070218441545,-0.74694975827906418,-0.73735278941328919,-0.72809709393775868,-0.71917990544694121,-0.71059839709829509,-0.7023496858206546,-0.69443083640942571,-0.68683886550670736,-0.67957074546491714,-0.67262340809293475,
-0.665993748284176,-0.65967862752637652,-0.65367487729320528,-0.64797930231813572,-0.64258868375128564,-0.63749978220019221,-0.63270934065571982,-0.62821408730450756,-0.62401073822954789,-0.62009600000065346,-0.6164665721567153,
-0.61311914958178282,-0.61005042477710547,-0.60725709003137318,-0.60473583949146925,-0.6024833711361185,-0.6004963886548641,-0.59877160323485235,-0.59730573525793174,-0.59609551591059584,-0.59513768870931372,-0.59442901094379141,
-0.59396625504070921,-0.59374620985046622,-0.59376568185944878,-0.59402149633031787,-0.59451049837278369,-0.59522955394730392,-0.59617555080410922,-0.59734539935991893,-0.59873603351467142,-0.60034441141054729,-0.60216751613551844,
-0.60420235637361019,-0.6064459670040111,-0.60889540965111577,-0.61154777318753661,-0.614400174192063,-0.61744975736450047,-0.6206936958992606,-0.62412919181952808,-0.62775347627377154,-0.63156380979631332,-0.63555748253362176,
-0.63973181443793714,-0.644084155429786,-0.64861188553089577,-0.65331241496896131,-0.65818318425567313,-0.66322166423936313,-0.66842535613357745,-0.6737917915228393,-0.67931853234681672,-0.68500317086406692,-0.690843329596484,
-0.69683666125553212,-0.70298084865130839,-0.70927360458543343,-0.71571267172873254,-0.722295822484629,-0.72902085883913292,-0.73588561219827642,-0.74288794321380447,-0.75002574159790236,-0.75729692592770392,-0.7646994434402915},
new double[]{
-9.0893447740032549,-9.05601144490927,-9.0226781161075422,-8.9893447876182169,-8.9560114594628342,-8.9226781316644157,-8.8893448042475676,-8.8560114772385958,-8.82267815066562,-8.7893448245586985,-8.756011498949956,-8.72267817387374,
-8.6893448493667638,-8.6560115254682657,-8.6226782022201967,-8.5893448796673955,-8.5560115578577918,-8.5226782368426246,-8.489344916676659,-8.4560115974184367,-8.4226782791305386,-8.3893449618798588,-8.3560116457378975,
-8.322678330781093,-8.2893450170911454,-8.2560117047553909,-8.2226783938671879,-8.1893450845263267,-8.1560117768394775,-8.1226784709206683,-8.0893451668917855,-8.0560118648831143,-8.02267856503393,-7.9893452674930989,
-7.9560119724197591,-7.9226786799840143,-7.889345390367696,-7.85601210376517,-7.8226788203842013,-7.7893455404468792,-7.7560122641906011,-7.722678991869131,-7.6893457237537266,-7.6560124601343444,-7.62267920132093,
-7.5893459476447962,-7.5560126994600942,-7.5226794571453892,-7.4893462211053423,-7.4560129917725124,-7.4226797696092737,-7.389346555109876,-7.3560133488026409,-7.3226801512523068,-7.2893469630625445,-7.2560137848786361,
-7.2226806173903464,-7.189347461334985,-7.1560143175006905,-7.1226811867299249,-7.0893480699232247,-7.0560149680432014,-7.0226818821188184,-6.9893488132499648,-6.9560157626123456,-6.9226827314627046,-6.88934972114441,
-6.8560167330934236,-6.8226837688446826,-6.7893508300389191,-6.7560179184299542,-6.722685035892483,-6.6893521844304118,-6.6560193661857561,-6.622686583448159,-6.589353838665061,-6.5560211344525721,-6.5226884736070927,
-6.4893558591177323,-6.4560232941795848,-6.4226907822079182,-6.3893583268533414,-6.3560259320180066,-6.3226936018729418,-6.28936134087656,-6.2560291537944535,-6.2226970457205413,-6.1893650220996763,-6.1560330887518013,
-6.1227012518977748,-6.0893695181869587,-6.0560378947267113,-6.0227063891139077,-5.9893750094686133,-5.9560437644700821,-5.92271266339522,-5.8893817161596855,-5.8560509333618178,-5.8227203263295673,-5.7893899071706505,
-5.7560596888261379,-5.7227296851277183,-5.6893999108588726,-5.6560703818202462,-5.622741114899485,-5.5894121281458471,-5.556083440849914,-5.5227550736287432,-5.48942704851683,-5.4560993890632714,-5.4227721204355541,
-5.3894452695304036,-5.3561188650921752,-5.3227929378392886,-5.2894675205992492,-5.2561426484528226,-5.2228183588879808,-5.1894946919642546,-5.1561716904882005,-5.1228494002007041,-5.0895278699769033,-5.0562071520395584,
-5.0228873021867555,-4.9895683800348669,-4.9562504492777775,-4.9229335779634074,-4.8896178387886593,-4.8563033094139758,-4.822990072798734,-4.7896782175588282,-4.7563678383478143,-4.72305903626311,-4.6897519192787858,
-4.6564466027066223,-4.62314320968712,-4.5898418717123262,-4.5565427291823486,-4.5232459319975922,-4.4899516401888038,-4.4566600245871317,-4.4233712675364982,-4.39008556365067,-4.3568031206175348,-4.3235241600531671,
-4.2902489184083654,-4.256977647930432,-4.2237106176830626,-4.1904481146272694,-4.1571904447663561,-4.1239379343580032,-4.0906909311965789,-4.0574498059688269,-4.0242149536860889,-3.9909867951962239,-3.9577657787783496,
-3.9245523818234989,-3.8913471126041639,-3.8581505121356257,-3.8249631561317732,-3.7917856570579511,-3.7586186662831071,-3.7254628763332271,-3.6923190232477028,-3.6591878890398468,-3.62607030426232,-3.5929671506776737,
-3.5598793640336077,-3.5268079369418448,-3.493753921858755,-3.4607184341650119,-3.4277026553406174,-3.3947078362306229,-3.361735300395762,-3.3287864475410167,-3.2958627570138881,-3.262965791362789,-3.2300971999445656,
-3.1972587225686988,-3.1644521931642062,-3.1316795434537177,-3.098942806617623,-3.0662441209296061,-3.0335857333433163,-3.0009700030083959,-2.9683994046926125,-2.935876532085449,-2.9034041009572258,-2.8709849521466815,
-2.8386220543489542,-2.8063185066751086,-2.7740775409537721,-2.7419025237451202,-2.7097969580373653,-2.6777644845961377,-2.64580888293767,-2.6139340718975506,-2.58214410976801,-2.5504431939782153,-2.5188356602939632,
-2.4873259815153443,-2.4559187656535375,-2.4246187535707509,-2.3934308160704991,-2.362359950428865,-2.33141127636105,-2.3005900314214371,-2.2699015658394082,-2.2393513367973461,-2.2089449021614604,-2.1786879136803372,
-2.14858610967032,-2.1186453072109357,-2.0888713938775783,-2.0592703190424198,-2.02984808477809,-2.0006107364018906,-1.9715643527012587,-1.9427150358837486,-1.9140689012969734,-1.8856320669656965,-1.8574106429945869,
-1.8294107208860118,-1.8016383628226722,-1.7740995909648469,-1.746800376811545,-1.7197466306739795,-1.6929441913084624,-1.6663988157541587,-1.6401161694190947,-1.614101816455483,-1.5883612104627953,-1.5628996855541488,
-1.5377224478185108,-1.5128345672079886,-1.4882409698761325,-1.4639464309897461,-1.4399555680332346,-1.4162728346210434,-1.3929025148303045,-1.3698487180624288,-1.3471153744390947,-1.324706230734926,-1.302624846846125,
-1.28087459279147,-1.259458646239404,-1.238379990552458,-1.2176414133379621,-1.1972455054919204,-1.1771946607210657,-1.1574910755264567,-1.1381367496305404,-1.1191334868283853,-1.1004828962427529,-1.0821863939618612,
-1.0642452050380526,-1.046660365825115,-1.0294327266317256,-1.0125629546683439,-0.99605153726490125,-0.97989878533677621,-0.9641048370768126,-0.94866966185151591,-0.93359306428003408,-0.91887468847508835,-0.90451402242564882,
-0.89051040250184954,-0.87686301806337641,-0.86357091615335968,-0.85063300626061644,-0.838048065133944,-0.82581474163302415,-0.8139315616013747,-0.80239693274766288,-0.79120914952256871,-0.78036639797925522,-0.76986676060635584,
-0.759708221123229,-0.74988866922804631,-0.74040590529007877,-0.731257644978312,-0.72244152381926785,-0.71395510167761789,-0.70579586715386289,-0.69796124189399988,-0.69044858480671889,-0.683255196184261,-0.67637832172362122,
-0.66981515644530776,-0.66356284850735514,-0.65761850291275326,-0.65197918510888275,-0.64664192447794633,-0.64160371771775737,-0.63686153211259067,-0.63241230869411558,-0.62825296529272157,-0.62438039947981427,-0.620791491401898,
-0.6174831065074845,-0.614452098168061,-0.6116953101945346,-0.60920957925072194,-0.60699173716559951,-0.60503861314615082,-0.60334703589275651,-0.60191383561916523,-0.600735845979165,-0.59980990590213856,-0.59913286133974508,
-0.59870156692600973,-0.5985128875531408,-0.5985636998654158,-0.59885089367349409,-0.59937137329152246,-0.60012205879939973,-0.60109988723256125,-0.60230181370163238,-0.60372481244428178,-0.60536587781158679,-0.60722202519119162,
-0.60929029186951389,-0.61156773783522012,-0.614051446526152,-0.61673852552184993,-0.61962610718377653,-0.62271134924530025,-0.62599143535345436,-0.62946357556444132,-0.63312500679480266,-0.63697299323013,-0.64100482669314007,
-0.64521782697289176,-0.64960934211686849,-0.654176748687605,-0.65891745198548413,-0.66382888623928071,-0.66890851476598312,-0.6741538301013692,-0.67956235410277166,-0.68513163802541277,-0.69085926257365016,-0.69674283792841984,
-0.70278000375212557,-0.70896842917217251,-0.71530581274430471,-0.72178988239685793,-0.728418395357003,-0.7351891380600093,-0.7420999260425194,-0.74914860382079063,-0.75633304475481389,-0.76365115089919133,-0.77110085284161289},
new double[]{
-8.61370576254927,-8.5824557705252413,-8.55120577901562,-8.5199557880535774,-8.4887057976744344,-8.4574558079157818,-8.426205818817639,-8.3949558304226048,-8.3637058427760245,-8.33245585592617,-8.3012058699244271,-8.2699558848254924,
-8.2387059006875916,-8.2074559175727053,-8.1762059355468146,-8.14495595468015,-8.1137059750474769,-8.0824559967283811,-8.05120601980758,-8.0199560443752542,-7.9887060705274022,-7.9574560983662135,-7.9262061280004668,
-7.8949561595459592,-7.8637061931259522,-7.83245622887166,-7.8012062669227573,-7.7699563074279281,-7.7387063505454448,-7.7074563964437868,-7.6762064453023005,-7.644956497311898,-7.6137065526758043,-7.5824566116103513,
-7.5512066743458215,-7.51995674112735,-7.4887068122158809,-7.4574568878891867,-7.4262069684429548,-7.3949570541919423,-7.3637071454712046,-7.3324572426374051,-7.3012073460702114,-7.2699574561737723,-7.2387075733783055,
-7.2074576981417691,-7.1762078309516584,-7.1449579723269068,-7.1137081228199124,-7.0824582830186982,-7.05120845354921,-7.0199586350777583,-6.9887088283136238,-6.957459034011829,-6.9262092529760846,-6.8949594860619312,
-6.8637097341800812,-6.832459998299977,-6.8012102794535751,-6.76996057873938,-6.7387108973267322,-6.7074612364603787,-6.6762115974653309,-6.6449619817520427,-6.6137123908219166,-6.5824628262731713,-6.5512132898070767,
-6.5199637832346031,-6.488714308483492,-6.4574648676057809,-6.4262154627858177,-6.3949660963487887,-6.3637167707697966,-6.3324674886835259,-6.3012182528945218,-6.2699690663881444,-6.2387199323422191,-6.2074708541394363,
-6.1762218353805585,-6.1449728798984689,-6.1137239917731279,-6.0824751753474917,-6.0512264352444562,-6.0199777763848905,-5.9887292040068312,-5.95748072368591,-5.926232341357097,-5.8949840633378416,-5.8637358963527024,
-5.8324878475595572,-5.8012399245774988,-5.7699921355165289,-5.7387444890091492,-5.70749699424399,-5.6762496610015951,-5.6450024996925006,-5.6137555213977635,-5.5825087379120824,-5.551262161789694,-5.5200158063931966,
-5.4887696859455195,-5.4575238155852048,-5.4262782114252373,-5.3950328906156422,-5.3637878714100733,-5.3325431732366706,-5.30129881677344,-5.2700548240284446,-5.238811218425111,-5.2075680248929821,-5.1763252699642424,
-5.1450829818763975,-5.1138411906814794,-5.0825999283621917,-5.0513592289554321,-5.0201191286836515,-4.9888796660945287,-4.9576408822094917,-4.9264028206816235,-4.8951655279635364,-4.8639290534858315,-4.8326934498467873,
-4.8014587730139739,-4.7702250825385129,-4.73899244178276,-4.7077609181622178,-4.6765305834025384,-4.6453015138125284,-4.614073790574106,-4.5828475000502236,-4.55162273411181,-4.5203995904848693,-4.4891781731188969,
-4.457958592577854,-4.4267409664550144,-4.3955254198130316,-4.3643120856506634,-4.3331011053976622,-4.3018926294393838,-4.2706868176727673,-4.23948384009538,-4.20828387742933,-4.1770871217818764,-4.1458937773446785,
-4.1147040611336649,-4.083518203771602,-4.0523364503154715,-4.0211590611308647,-3.9899863128156361,-3.9588184991751207,-3.9276559322512723,-3.8964989434081034,-3.8653478844758542,-3.8342031289563221,-3.803065073291803,
-3.7719341382000717,-3.7408107700778159,-3.7096954424748856,-3.6785886576416544,-3.6474909481516984,-3.6164028786018823,-3.5853250473917884,-3.5542580885842505,-3.5232026738485271,-3.4921595144874029,-3.4611293635491962,
-3.4301130180253123,-3.3991113211335819,-3.3681251646871861,-3.3371554915484474,-3.3062032981662344,-3.27526963719508,-3.2443556201934465,-3.2134624203978315,-3.1825912755685755,-3.1517434909023931,-3.1209204420056906,
-3.0901235779217444,-3.0593544242037782,-3.0286145860248488,-2.9979057513143195,-2.9672296939095033,-2.9365882767098377,-2.9059834548197108,-2.87541727866481,-2.8448918970656289,-2.8144095602505188,-2.783972622789519,
-2.7535835464290255,-2.7232449028063339,-2.6929593760220918,-2.6627297650478594,-2.6325589859452534,-2.6024500738725669,-2.5724061848543949,-2.5424305972895636,-2.5125267131727185,-2.4826980590051408,-2.4529482863708774,
-2.4232811721550163,-2.393700618381958,-2.3642106516528285,-2.3348154221627397,-2.3055192022804558,-2.276326384675122,-2.2472414799770735,-2.2182691139623514,-2.1894140242533719,-2.1606810565312178,-2.1320751602582062,
-2.1036013839127139,-2.0752648697416771,-2.0470708480396578,-2.0190246309669035,-1.9911316059223108,-1.9633972284906571,-1.9358270149867976,-1.9084265346227216,-1.8812014013263865,-1.8541572652440383,-1.8272998039602793,
-1.800634713472399,-1.7741676989574422,-1.7479044653721012,-1.7218507079267869,-1.6960121024761481,-1.6703942958688332,-1.6450028962994654,-1.6198434637055938,-1.5949215002518309,-1.5702424409424702,-1.5458116444026575,
-1.5216343838666371,-1.4977158384097749,-1.4740610844589894,-1.4506750876139065,-1.4275626948085689,-1.4047286268408721,-1.3821774712941106,-1.3599136758721468,-1.3379415421667709,-1.3162652198728504,-1.2948887014639023,
-1.2738158173377789,-1.2530502314392684,-1.2325954373636183,-1.2124547549422711,-1.1926313273095304,-1.1731281184464233,-1.1539479111957292,-1.1350933057400143,-1.1165667185325423,-1.0983703816691517,-1.0805063426875712,
-1.0629764647792332,-1.0457824273973813,-1.0289257272442176,-1.0124076796189281,-0.9962294201077081,-0.98039190659633946,-0.96489592158546778,-0.94974207478845929,-0.93493080599159084,-0.92046238815632586,-0.90633693074354071,
-0.89255438323979142,-0.87911453886602331,-0.86601703844953293,-0.8532613744404659,-0.84084689505468346,-0.82877280852542878,-0.81703818744687573,-0.80564197319333064,-0.79458298039857833,-0.78385990148060825,-0.7734713111977195,
-0.76341567122277332,-0.75369133472314176,-0.74429655093467817,-0.73522946971880609,-0.72648814609258849,-0.718070544722388,-0.70997454437246466,-0.70219794230057242,-0.69473845859330918,-0.68759374043464883,-0.68076136630172712,
-0.674238850082576,-0.66802364511109191,-0.66211314811509214,-0.65650470307384778,-0.651195604981994,-0.64618310351719588,-0.64146440660940474,-0.63703668390996226,-0.63289707015920826,-0.62904266845161816,-0.62547055339784519,
-0.62217777418335563,-0.61916135752364754,-0.61641831051631057,-0.61394562339043757,-0.61174027215412319,-0.60979922114099527,-0.60811942545690989,-0.60669783332811111,-0.60553138835230824,-0.60461703165425729,-0.60395170394755338,
-0.603532347504444,-0.60335590803556471,-0.60341933648157642,-0.6037195907187477,-0.60425363718058167,-0.60501845239763,-0.60601102445767052,-0.60722835438845246,-0.60866745746522644,-0.61032536444529151,-0.61219912273178956,
-0.61428579746897594,-0.61658247257118814,-0.61908625168771392,-0.621794259105749,-0.62470364059360473,-0.62781156418630291,-0.63111522091566263,-0.63461182548695316,-0.63829861690414635,-0.64217285904577059,-0.64623184119332167,
-0.65047287851414892,-0.65489331250068861,-0.65949051136787584,-0.6642618704105171,-0.66920481232236251,-0.67431678747857027,-0.67959527418320675,-0.685037778883383,-0.69064183635157639,-0.69640500983764464,-0.70232489119198838,
-0.70839910096127434,-0.7146252884580867,-0.72100113180582481,-0.72752433796012572,-0.73419264270804152,-0.74100381064616028,-0.74795563513881869,-0.75504593825750632,-0.76227257070252918,-0.7696334117079523,-0.77712636893080711},
new double[]{
-8.1918821537213873,-8.1624704030984852,-8.1330586533288418,-8.1036469044641546,-8.0742351565592525,-8.0448234096722882,-8.0154116638649349,-7.9859999192026052,-7.956588175754673,-7.927176433594715,-7.8977646928007683,-7.8683529534555952,
-7.8389412156469733,-7.8095294794680008,-7.7801177450174119,-7.7507060123999274,-7.7212942817266095,-7.691882553115251,-7.6624708266907806,-7.6330591025856975,-7.6036473809405267,-7.57423566190431,-7.5448239456351143,
-7.515412232300589,-7.4860005220785366,-7.4565888151575344,-7.4271771117375831,-7.397765412030803,-7.3683537162621651,-7.3389420246702688,-7.3095303375081722,-7.2801186550442623,-7.2507069775631905,-7.2212953053668487,
-7.1918836387754252,-7.1624719781285018,-7.1330603237862373,-7.103648676130609,-7.0742370355667381,-7.044825402524288,-7.0154137774589547,-6.986002160854043,-6.956590553222135,-6.9271789551068705,-6.8977673670848247,
-6.8683557897675023,-6.8389442238034555,-6.8095326698805287,-6.7801211287282372,-6.7507096011202918,-6.7212980878772779,-6.69188658986949,-6.66247510801995,-6.6330636433075956,-6.6036521967706685,-6.5742407695103084,
-6.5448293626943608,-6.5154179775614187,-6.4860066154251079,-6.4565952776786286,-6.4271839657995793,-6.3977726813550655,-6.368361426007124,-6.3389502015184673,-6.3095390097585833,-6.280127852710204,-6.2507167324761586,
-6.2213056512866505,-6.1918946115069664,-6.1624836156456615,-6.1330726663632271,-6.1036617664812969,-6.0742509189923988,-6.0448401270703052,-6.0154293940810017,-5.9860187235943272,-5.95660811939631,-5.9271975855022587,
-5.8977871261706358,-5.8683767459177769,-5.8389664495334967,-5.8095562420976385,-5.7801461289976261,-5.7507361159470731,-5.7213262090055172,-5.69191641459935,-5.6625067395440114,-5.6330971910675132,-5.6036877768353994,
-5.574278504977201,-5.5448693841144934,-5.515460423390647,-5.4860516325023641,-5.4566430217331314,-5.427234601988677,-5.3978263848345707,-5.3684183825360909,-5.3390106081004864,-5.309603075321788,-5.280195798828319,
-5.2507887941330544,-5.2213820776870232,-5.1919756669359058,-5.162569580380044,-5.133163837638044,-5.1037584595142063,-5.0743534680699955,-5.0449488866997934,-5.01554474021119,-4.9861410549100915,-4.956737858690901,
-4.9273351811321069,-4.8979330535975727,-4.8685315093438728,-4.8391305836340273,-4.8097303138580116,-4.78033073966044,-4.7509319030758261,-4.7215338486718927,-4.6921366237013613,-4.6627402782627341,-4.6333448654705913,
-4.6039504416359325,-4.5745570664571558,-4.5451648032222725,-4.5157737190230041,-4.4863838849814321,-4.456995376489906,-4.4276082734649727,-4.3982226606160815,-4.3688386277299269,-4.339456269971258,-4.31007568820109,
-4.2806969893132569,-4.2513202865903041,-4.2219457000797753,-4.19257335699197,-4.1632033921203337,-4.1338359482856637,-4.1044711768053688,-4.0751092379890954,-4.0457503016620606,-4.0163945477174892,-3.9870421666996223,
-3.9576933604187996,-3.9283483426001711,-3.8990073395676568,-3.8696705909648066,-3.8403383505142767,-3.8110108868176593,-3.7816884841974705,-3.7523714435831144,-3.7230600834426846,-3.6937547407624836,-3.664455772076161,
-3.6351635545453806,-3.6058784870939169,-3.5766009915970836,-3.547331514128361,-3.5180705262650487,-3.4888185264547271,-3.4595760414442229,-3.4303436277726922,-3.4011218733303079,-3.3719113989839147,-3.3427128602708334,
-3.3135269491618096,-3.2843543958938781,-3.2551959708736491,-3.2260524866512306,-3.1969247999646759,-3.1678138138544552,-3.1387204798470512,-3.1096458002063132,-3.0805908302506944,-3.0515566807339525,-3.02254452028629,
-2.9935555779122542,-2.96459114554104,-2.9356525806240659,-2.90674130877393,-2.8778588264379983,-2.8490067035990112,-2.8201865864941857,-2.7914002003433307,-2.7626493520755475,-2.733935933043079,-2.7052619217098854,
-2.6766293863015287,-2.6480404874019512,-2.6194974804817845,-2.5910027183418878,-2.5625586534549352,-2.5341678401870671,-2.505832936880878,-2.4775567077803768,-2.4493420247780282,-2.4211918689635903,-2.3931093319541978,
-2.3650976169850559,-2.3371600397401768,-2.3093000289028613,-2.2815211264060857,-2.2538269873636292,-2.2262213796636461,-2.1987081832075139,-2.171291388778084,-2.1439750965230342,-2.1167635140407564,-2.0896609540581972,
-2.0626718316922235,-2.0358006612884294,-2.0090520528338081,-1.9824307079423589,-1.9559414154154626,-1.9295890463817158,-1.9033785490238235,-1.8773149429030935,-1.8514033128950174,-1.8256488027523206,-1.8000566083146956,
-1.7746319703871618,-1.7493801673115743,-1.7243065072582378,-1.6994163202667971,-1.6747149500675744,-1.6502077457162845,-1.6259000530765355,-1.6017972061857337,-1.5779045185409071,-1.5542272743415744,-1.53077071972707,
-1.5075400540457238,-1.4845404211929743,-1.4617769010548753,-1.4392545010925546,-1.4169781481020103,-1.394952680182213,-1.3731828389428216,-1.3516732619809626,-1.3304284756544809,-1.3094528881768646,-1.2887507830567246,
-1.2683263129022682,-1.2481834936087075,-1.2283261989439867,-1.2087581555456348,-1.1894829383389813,-1.1705039663844263,-1.1518244991589626,-1.1334476332747188,-1.1153762996349583,-1.09761326102573,-1.0801611101392523,
-1.0630222680231229,-1.0461989829475962,-1.0296933296814685,-1.0135072091655544,-0.99764234857134348,-0.982100301731181,-0.96688244992522621,-0.95199000300951442,-0.93742400086865951,-0.92318531517609825,-0.90927465144427944,
-0.89569255134684111,-0.882439395294578,-0.86951540524688831,-0.85692064774038024,-0.84465503711642,-0.83271833892958658,-0.82111017351928228,-0.80983001972709512,-0.7988772187429326,-0.78825097806342614,-0.77795037554663826,
-0.767974363547678,-0.75832177312044668,-0.74899131827137355,-0.73998160025167,-0.73129111187530937,-0.72291824185063314,-0.71486127911418629,-0.7071184171560787,-0.69968775832687313,-0.69256731811668582,-0.68575502939786837,
-0.67924874662330548,-0.67304624997301465,-0.66714524944236631,-0.66154338886585651,-0.65623824987095447,-0.651227355757119,-0.64650817529561844,-0.6420781264463139,-0.63793457998805769,-0.6340748630598313,-0.63049626261019176,
-0.62719602875301517,-0.62417137802792,-0.62141949656412609,-0.618937543146846,-0.61672265218563371,-0.61477193658441009,-0.6130824905131641,-0.61165139208158414,-0.61047570591510592,-0.60955248563408349,-0.60887877623697906,
-0.60845161638864986,-0.6082680406149692,-0.608325081405161,-0.60861977122335742,-0.609149144431003,-0.60991023912182529,-0.61090009887118357,-0.61211577440167808,-0.613554325166967,-0.6152128208557911,-0.61708834281824831,
-0.61917798541639479,-0.62147885730127306,-0.62398808261848682,-0.62670280214445,-0.62962017435544426,-0.63273737643161276,-0.63605160519801329,-0.6395600780048396,-0.6432600335489026,-0.64714873263843964,-0.65122345890329869,
-0.65548151945251276,-0.6599202454812475,-0.66453699282907641,-0.669329142491495,-0.67429410108655186,-0.67942930127843482,-0.684732202159806,-0.6902002895946403,-0.69583107652327936,-0.70162210323136442,-0.70757093758427492,
-0.71367517522864576,-0.71993243976249921,-0.72634038287547831,-0.7328966844606235,-0.73959905269909121,-0.74644522411916836,-0.75343296363089052,-0.76056006453753155,-0.767824348525185,-0.77522366563161926,-0.78275589419554414},
new double[]{
-7.8153950927505429,-7.787617338524643,-7.7598395856442064,-7.7320618341860952,-7.7042840842315634,-7.6765063358665069,-7.6487285891817276,-7.6209508442732155,-7.5931731012424448,-7.5653953601966863,-7.5376176212493364,-7.5098398845202743,
-7.4820621501362226,-7.4542844182311461,-7.4265066889466622,-7.398728962432477,-7.3709512388468514,-7.3431735183570854,-7.3153958011400357,-7.287618087382663,-7.25984037728261,-7.2320626710488067,-7.2042849689021207,
-7.1765072710760371,-7.1487295778173783,-7.1209518893870669,-7.0931742060609295,-7.0653965281305524,-7.037618855904177,-7.0098411897076529,-6.9820635298854459,-6.9542858768016975,-6.9265082308413515,-6.8987305924113418,
-6.8709529619418479,-6.8431753398876243,-6.8153977267294019,-6.7876201229753734,-6.7598425291627606,-6.7320649458594763,-6.7042873736658715,-6.6765098132165921,-6.6487322651825389,-6.620954730272933,-6.5931772092375125,
-6.565399702868838,-6.537622212004746,-6.50984473753093,-6.4820672803836743,-6.4542898415527494,-6.4265124220844561,-6.398735023084865,-6.3709576457232231,-6.343180291235563,-6.3154029609285205,-6.287625656183363,
-6.2598483784602532,-6.2320711293027529,-6.20429391034259,-6.1765167233046858,-6.1487395700124861,-6.1209624523935791,-6.0931853724856451,-6.0654083324427441,-6.0376313345419579,-6.0098543811904168,-5.9820774749327219,
-5.9543006184587979,-5.9265238146121826,-5.8987470663988049,-5.8709703769962474,-5.8431937497635511,-5.8154171882515691,-5.7876406962139146,-5.7598642776185383,-5.732087936659954,-5.7043116777721767,-5.6765355056423825,
-5.6487594252253563,-5.6209834417587672,-5.5932075607792973,-5.565431788139712,-5.5376561300268836,-5.50988059298085,-5.4821051839149622,-5.4543299101371705,-5.4265547793725295,-5.3987797997869791,-5.3710049800124811,
-5.3432303291735881,-5.31545585691552,-5.2876815734338356,-5.2599074895058004,-5.2321336165235222,-5.2043599665289841,-5.1765865522510595,-5.1488133871446289,-5.1210404854319176,-5.0932678621461855,-5.0654955331778817,
-5.037723515323429,-5.0099518263367644,-4.9821804849838029,-4.9544095110999864,-4.9266389256510825,-4.898868750797428,-4.871099009961803,-4.8433297279011365,-4.8155609307822633,-4.787792646261952,-4.7600249035714439,
-4.7322577336057581,-4.7044911690180191,-4.6767252443190932,-4.6489599959828158,-4.6211954625571376,-4.5934316847814891,-4.56566870571073,-4.537906570846034,-4.5101453282730759,-4.4823850288079505,-4.4546257261512086,
-4.4268674770504761,-4.3991103414721096,-4.371354382782374,-4.3435996679386593,-4.3158462676912741,-4.2880942567963691,-4.260343714240598,-4.2325947234781216,-4.2048473726806108,-4.1771017550009333,-4.1493579688512252,
-4.1216161181961031,-4.09387631286179,-4.066138668861969,-4.0384033087412217,-4.0106703619369206,-3.9829399651605226,-3.9552122627992037,-3.9274874073388486,-3.8997655598094316,-3.8720468902538654,-3.8443315782214413,
-3.8166198132870157,-3.7889117955971425,-3.7612077364443892,-3.733507858871107,-3.7058123983039732,-3.678121603220645,-3.6504357358499098,-3.6227550729067381,-3.5950799063636794,-3.567410544260063,-3.53974731155048,
-3.5120905509940492,-3.4844406240859662,-3.456797912032846,-3.4291628167733559,-3.40153576204563,-3.373917194502928,-3.3463075848789625,-3.3187074292042831,-3.2911172500750321,-3.2635375979753229,-3.2359690526543914,
-3.208412224559567,-3.1808677563259784,-3.1533363243237593,-3.1258186402633452,-3.0983154528592509,-3.0708275495525088,-3.0433557582916682,-3.0159009493720039,-2.9884640373322466,-2.9610459829078088,-2.9336477950391027,
-2.9062705329331182,-2.8789153081759964,-2.8515832868938196,-2.8242756919583338,-2.7969938052337415,-2.7697389698601111,-2.742512592568302,-2.715316146020649,-2.68815117117093,-2.6610192796364256,-2.6339221560741146,
-2.606861560552268,-2.5798393309079235,-2.5528573850799123,-2.5259177234063022,-2.4990224308743314,-2.4721736793101123,-2.4453737294946309,-2.4186249331918357,-2.3919297350739224,-2.3652906745283113,-2.338710387330234,
-2.3121916071643875,-2.2857371669787239,-2.2593500001531708,-2.2330331414659144,-2.2067897278398569,-2.1806229988519723,-2.1545362969885522,-2.1285330676297578,-2.1026168587474987,-2.0767913203014197,-2.051060203318734,
-2.0254273586447709,-1.9998967353524024,-1.9744723788000111,-1.949158428329306,-1.923959114596109,-1.8988787565292047,-1.8739217579144358,-1.8490926036034565,-1.8243958553488704,-1.7998361472698783,-1.7754181809550163,
-1.7511467202110522,-1.7270265854695945,-1.7030626478654394,-1.6792598230030971,-1.6556230644302876,-1.632157356839423,-1.608867709020213,-1.5857591465884777,-1.5628367045180316,-1.5401054195040802,-1.5175703221879362,
-1.495236429273989,-1.4731087355707564,-1.4511922059884754,-1.4294917675260697,-1.4080123012804466,-1.3867586345109317,-1.3657355327912482,-1.3449476922808052,-1.3243997321461698,-1.3040961871624872,-1.2840415005233024,
-1.26424001688572,-1.244695975676162,-1.2254135046801531,-1.206396613937601,-1.1876491899629778,-1.1691749903076565,-1.1509776384794535,-1.1330606192321782,-1.1154272742357356,-1.0980807981350715,-1.0810242350040189,
-1.0642604751979279,-1.0477922526068288,-1.031622142308843,-1.0157525586215885,-1.0001857535474883,-0.98492381560713083,-0.9699686690532312,-0.95532207345622633,-0.940985623651192,-0.92696075003452827,-0.91324871919777517,
-0.89985063488495587,-0.88676743925902712,-0.87399991446232128,-0.86154868445530175,-0.84941421711751475,-0.83759682659429646,-0.82609667587258662,-0.81491377956909483,-0.80404800691406164,-0.79349908491394117,-0.78326660167650364,
-0.77335000988210312,-0.76374863038517116,-0.75446165593037484,-0.74548815496831022,-0.73682707555607962,-0.72847724932862379,-0.72043739552722874,-0.71270612507221376,-0.70528194466740368,-0.69816326092461278,-0.69134838449699487,
-0.6848355342107515,-0.67862284118533056,-0.672708352932882,-0.6670900374283727,-0.66176578714238476,-0.6567334230292321,-0.651990698463634,-0.64753530311976193,-0.64336486678704641,-0.63947696311767455,-0.6358691133012373,
-0.63253878966248855,-0.62948341917866546,-0.62670038691327523,-0.624187039363697,-0.62194068772035882,-0.6199586110356422,-0.61823805930104,-0.61677625643143374,-0.61557040315568623,-0.61461767981304571,-0.61391524905513906,
-0.61346025845359309,-0.61324984301356389,-0.6132811275936747,-0.6135512292330656,-0.6140572593864464,-0.61479632606820622,-0.61576553590679328,-0.616961996110706,-0.61838281634756576,-0.62002511053784592,-0.62188599856492666,
-0.62396260790322888,-0.62625207516625236,-0.62875154757640073,-0.63145818435852863,-0.6343691580591877,-0.63748165579357652,-0.640792880422228,-0.64430005165948123,-0.64800040711579565,-0.65189120327596928,-0.65596971641531709,
-0.66023324345586309,-0.66467910276458175,-0.66930463489571046,-0.674107203279133,-0.67908419485680993,-0.684233020669202,-0.68955111639360678,-0.69503594283628967,-0.70068498638026211,-0.7064957593905159,-0.71246580057849151,
-0.71859267532751,-0.72487397598086589,-0.73130732209422833,-0.73789036065396207,-0.74462076626293139,-0.75149624129530967,-0.75851451602187248,-0.765673348707209,-0.77297052568024083,-0.78040386137939621,-0.78797119837374385},
new double[]{
-7.4774607603248189,-7.4511450084371615,-7.4248292585806928,-7.3985135108651807,-7.372197765406324,-7.3458820223260739,-7.319566281752973,-7.2932505438225093,-7.2669348086774921,-7.24061907646845,-7.2143033473540452,-7.1879876215015139,
-7.1616718990871293,-7.1353561802966921,-7.1090404653260419,-7.0827247543816014,-7.0564090476809511,-7.0300933454534258,-7.0037776479407556,-6.9774619553977333,-6.9511462680929208,-6.9248305863093913,-6.8985149103455186,
-6.8721992405157959,-6.8458835771517146,-6.8195679206026787,-6.7932522712369732,-6.7669366294427844,-6.7406209956292775,-6.7143053702277262,-6.6879897536927118,-6.6616741465033771,-6.6353585491647591,-6.6090429622091866,
-6.5827273861977531,-6.5564118217218708,-6.5300962694049121,-6.5037807299039336,-6.4774652039115,-6.4511496921575935,-6.4248341954116484,-6.3985187144846734,-6.3722032502314994,-6.3458878035531514,-6.3195723753993382,
-6.2932569667710894,-6.2669415787235208,-6.240626212368765,-6.2143108688790427,-6.1879955494899166,-6.1616802555037093,-6.1353649882931114,-6.1090497493049831,-6.0827345400643589,-6.0564193621786737,-6.0301042173422124,
-6.0037891073408014,-5.9774740340567556,-5.9511589994740852,-5.9248440056839948,-5.8985290548906706,-5.8722141494173812,-5.8458992917129136,-5.8195844843583462,-5.7932697300742,-5.7669550317279645,-5.7406403923420388,
-5.71432581510209,-5.6880113033658795,-5.6616968606725449,-5.6353824907524022,-5.6090681975372609,-5.5827539851713084,-5.5564398580225722,-5.5301258206950044,-5.5038118780412155,-5.4774980351759,-5.45118429748998,
-5.42487067066551,-5.3985571606913947,-5.3722437738799416,-5.345930516884315,-5.319617396716926,-5.2933044207688082,-5.266991596830052,-5.2406789331113188,-5.21436643826653,-5.1880541214167684,-5.161741992175469,
-5.135430060674965,-5.1091183375944551,-5.082806834189487,-5.056495562323013,-5.0301845344981233,-5.003873763892531,-4.9775632643949157,-4.9512530506432126,-4.9249431380649593,-4.8986335429198071,-4.8723242823443087,
-4.84601537439911,-4.8197068381186714,-4.7933986935636463,-4.7670909618760655,-4.74078366533748,-4.7144768274302011,-4.6881704729018221,-4.6618646278331832,-4.6355593197099614,-4.60925457749808,-4.5829504317231411,
-4.5566469145540767,-4.5303440598912674,-4.5040419034593331,-4.4777404829048564,-4.4514398378992954,-4.4251400102473486,-4.3988410440010632,-4.372542985579976,-4.3462458838976037,-4.319949790494614,-4.29365475967901,
-4.2673608486736967,-4.2410681177718059,-4.2147766305001664,-4.1884864537913478,-4.1621976581646987,-4.135910317916843,-4.1096245113220968,-4.08334032084332,-4.0570578333536993,-4.0307771403700343,-4.0044983382980632,
-3.9782215286904372,-3.9519468185179565,-3.9256743204547142,-3.8994041531778092,-3.8731364416823348,-3.8468713176123654,-3.820608919608695,-3.7943493936741124,-3.7680928935570259,-3.7418395811542808,-3.7155896269340429,
-3.6893432103796515,-3.6631005204553704,-3.6368617560950112,-3.6106271267144066,-3.5843968527487644,-3.5581711662159488,-3.5319503113067623,-3.5057345450033246,-3.4795241377266843,-3.4533193740147907,-3.4271205532320033,
-3.4009279903113079,-3.3747420165304334,-3.3485629803230674,-3.3223912481263671,-3.2962272052659731,-3.270071256879707,-3.2439238288811354,-3.2177853689641496,-3.1916563476496873,-3.1655372593756743,-3.1394286236312179,
-3.1133309861360283,-3.087244920065952,-3.0611710273254364,-3.0351099398676187,-3.0090623210626366,-2.9830288671146015,-2.957010308527535,-2.9310074116203917,-2.9050209800910864,-2.8790518566292485,-2.8531009245771566,
-2.8271691096380671,-2.8012573816308404,-2.7753667562894582,-2.74949829710568,-2.723653117212709,-2.6978323813073319,-2.6720373076075772,-2.6462691698424603,-2.6205292992699012,-2.5948190867183896,-2.5691399846474137,
-2.5434935092211117,-2.51788124238901,-2.4923048339670992,-2.4667660037118666,-2.44126654337927,-2.4158083187599795,-2.3903932716815568,-2.3650234219676047,-2.3397008693432526,-2.3144277952757348,-2.289206464738204,
-2.26403922788435,-2.2389285216208679,-2.21387687106432,-2.1888868908685386,-2.1639612864083282,-2.1391028548049853,-2.1143144857789387,-2.0895991623147405,-2.0649599611236531,-2.0404000528892161,-2.0159227022814386,
-1.9915312677256516,-1.9672292009125989,-1.9430200460370068,-1.9189074387527039,-1.8948951048333236,-1.8709868585287457,-1.8471866006086855,-1.8234983160862459,-1.799926071615783,-1.7764740125610894,-1.7531463597316852,
-1.7299474057868729,-1.7068815113091829,-1.6839531005508641,-1.6611666568591654,-1.6385267177882628,-1.6160378699078248,-1.593704743320314,-1.5715320059012159,-1.5495243572784117,-1.5276865225688556,-1.5060232458925764,
-1.4845392836857436,-1.4632393978361307,-1.4421283486657344,-1.4212108877865648,-1.4004917508566808,-1.3799756502644116,-1.359667267769356,-1.3395712471291787,-1.3196921867414482,-1.3000346323297334,-1.2806030697029609,
-1.2614019176165769,-1.2424355207634026,-1.2237081429212129,-1.2052239602830239,-1.1869870549948467,-1.1690014089242835,-1.1512708976818182,-1.1337992849149969,-1.1165902168939346,-1.0996472174047394,-1.0829736829655285,
-1.0665728783777442,-1.050447932623485,-1.0346018351175683,-1.0190374323210314,-1.0037574247208172,-0.98876436417844971,-0.97406065164862876,-0.95964853526686444,-0.94553010880353683,-0.931707310480129,-0.9181819221418388,
-0.90495556877933458,-0.89202971839110057,-0.87940568217660253,-0.86708461504941614,-0.85506751645848789,-0.84335523150484692,-0.831948452340352,-0.82084771983444438,-0.81005342549437231,-0.79956581362396184,-0.789384983705724,
-0.77951089299089749,-0.7699433592819408,-0.76068206389197879,-0.75172655476579442,-0.74307624974710818,-0.734730439977116,-0.72668829340954344,-0.71894885842781975,-0.7115110675503693,-0.70437374121045659,-0.69753559159749734,
-0.69099522654725309,-0.68475115346886373,-0.67880178329722252,-0.67314543445977149,-0.66778033684737137,-0.66270463577949434,-0.65791639595457063,-0.65341360537691628,-0.64919417925225154,-0.64525596384439865,-0.64159674028631719,
-0.6382142283391925,-0.63510609009383612,-0.63226993360918338,-0.62970331648318634,-0.62740374935189192,-0.62536869931296513,-0.623595593270376,-0.62208182119739663,-0.62082473931546867,-0.6198216731868903,-0.61906992071964428,
-0.61856675508303161,-0.61830942753310769,-0.61829517014722,-0.61852119846723452,-0.6189847140513044,-0.61968290693427963,-0.62061295799708571,-0.62177204124561014,-0.62315732599982432,-0.62476597899404973,-0.62659516638943025,
-0.62864205569982323,-0.63090381763244618,-0.6333776278447345,-0.63606066861896771,-0.63895013045631022,-0.64204321359199457,-0.64533712943343713,-0.64882910192313847,-0.65251636882826469,-0.65639618295884572,-0.66046581331655529,
-0.66472254617606219,-0.66916368610095289,-0.673786556896239,-0.67858850249946179,-0.6835668878124036,-0.68871909947540766,-0.69404254658629549,-0.69953466136585163,-0.70519289977182686,-0.71101474206338411,-0.71699769331788521,
-0.723139283901887,-0.7294370698981788,-0.73588863349066436,-0.74249158330884879,-0.74924355473365789,-0.75614221016627325,-0.76318523926163129,-0.77037035912818586,-0.77769531449550089,-0.78515787785118785,-0.792755849548667},
new double[]{
-7.1725856305115183,-7.1475856881045337,-7.1225857486503932,-7.0975858123004905,-7.0725858792139826,-7.0475859495581838,-7.02258602350899,-6.9975861012513132,-6.9725861829795459,-6.9475862688980481,-6.9225863592216568,-6.8975864541762233,
-6.8725865539991782,-6.8475866589401253,-6.8225867692614655,-6.7975868852390535,-6.7725870071628851,-6.7475871353378265,-6.7225872700843707,-6.6975874117394465,-6.6725875606572522,-6.6475877172101479,-6.6225878817895847,
-6.5975880548070807,-6.5725882366952533,-6.5475884279088987,-6.5225886289261314,-6.4975888402495761,-6.4725890624076268,-6.4475892959557664,-6.4225895414779561,-6.3975897995880961,-6.372590070931559,-6.3475903561868039,
-6.3225906560670717,-6.2975909713221689,-6.2725913027403424,-6.2475916511502492,-6.2225920174230263,-6.1975924024744735,-6.1725928072673355,-6.1475932328137146,-6.122593680177598,-6.0975941504775149,-6.0725946448893389,
-6.0475951646492181,-6.0225957110566739,-5.9975962854778428,-5.9725968893488917,-5.9475975241796091,-5.9225981915571788,-5.8975988931501435,-5.8725996307125747,-5.8476004060884605,-5.8226012212163081,-5.7976020781339912,
-5.77260297898384,-5.7476039260179954,-5.7226049216040353,-5.69760596823089,-5.6726070685150614,-5.647608225207156,-5.62260944119876,-5.59761071952966,-5.5726120633954377,-5.5476134761554485,-5.5226149613412145,
-5.4976165226652425,-5.472618164030294,-5.4476198895391352,-5.422621703504773,-5.39762361046123,-5.3726256151748606,-5.3476277226562514,-5.3226299381727227,-5.2976322672614806,-5.2726347157434335,-5.2476372897377139,
-5.2226399956769516,-5.1976428403233168,-5.1726458307853918,-5.1476489745358975,-5.1226522794303335,-5.0976557537265608,-5.0726594061053953,-5.0476632456922408,-5.0226672820798388,-4.9976715253521693,-4.9726759861095768,
-4.9476806754951763,-4.9226856052226049,-4.8976907876051907,-4.8726962355866057,-4.8477019627730833,-4.8227079834672759,-4.7977143127038335,-4.7727209662868,-4.7477279608289065,-4.7227353137928638,-4.6977430435347607,
-4.672751169349656,-4.6477597115195,-4.62276869136348,-4.5977781312909221,-4.5727880548568791,-4.5477984868205361,-4.5228094532065724,-4.4978209813696326,-4.4728331000620587,-4.4478458395050477,-4.4228592314634092,
-4.3978733093240843,-4.3728881081786461,-4.3479036649099383,-4.3229200182830931,-4.2979372090411223,-4.2729552800053128,-4.247974276180666,-4.2229942448666291,-4.19801523577337,-4.1730373011438751,-4.148060495882147,
-4.1230848776878091,-4.09811050719741,-4.0731374481327727,-4.048165767456708,-4.02319553553646,-3.9982268263152467,-3.9732597174922843,-3.9482942907116931,-3.9233306317607117,-3.8983688307776534,-3.8734089824700595,
-3.84845118634353,-3.8234955469417211,-3.7985421740980287,-3.7735911831994891,-3.7486426954634586,-3.7236968382276436,-3.698753745254086,-3.6738135570477248,-3.648876421190177,-3.6239424926894048,-3.5990119343459628,
-3.5740849171365343,-3.5491616206154957,-3.5242422333352672,-3.4993269532862326,-3.4744159883570322,-3.4495095568160528,-3.4246078878149659,-3.39971122191518,-3.37481981163809,-3.3499339220400342,-3.3250538313128728,
-3.3001798314111297,-3.2753122287066376,-3.250451344671645,-3.2255975165913506,-3.2007510983068288,-3.1759124609893066,-3.1510819939467658,-3.1262601054638064,-3.101447223675716,-3.0766437974776584,-3.0518502974698691,
-3.0270672169397064,-3.0022950728813735,-2.9775344070540672,-2.9527857870792489,-2.9280498075776635,-2.9033270913466538,-2.87861829057821,-2.8539240881180978,-2.8292451987662881,-2.8045823706187551,-2.7799363864505828,
-2.75530806514013,-2.7306982631338239,-2.7061078759509472,-2.6815378397275489,-2.6569891327983757,-2.6324627773154354,-2.6079598409015343,-2.5834814383367961,-2.5590287332758574,-2.5346029399930585,-2.5102053251525822,
-2.4858372096000791,-2.461499970171908,-2.4371950415176542,-2.4129239179311459,-2.3886881551846866,-2.3644893723607376,-2.3403292536747538,-2.3162095502823683,-2.2921320820635791,-2.2680987393760517,-2.244111484769129,
-2.2201723546495931,-2.1962834608897079,-2.1724469923675751,-2.1486652164293374,-2.1249404802623246,-2.1012752121678115,-2.0776719227216831,-2.054133205810984,-2.0306617395340645,-2.0072602869518334,-1.9839316966775062,
-1.9606789032922114,-1.9375049275738425,-1.9144128765267154,-1.8914059431998267,-1.8684874062818737,-1.8456606294616715,-1.822929060543192,-1.8002962303051675,-1.777765751096031,-1.7553413151559276,-1.7330266926586058,
-1.7108257294671938,-1.6887423445991745,-1.6667805273972811,-1.6449443344045538,-1.6232378859433898,-1.6016653624000938,-1.5802310002181768,-1.5589390876054272,-1.5377939599616068,-1.5167999950354436,-1.4959616078214295,
-1.475283245208741,-1.4547693803963626,-1.4344245070902122,-1.4142531334996911,-1.3942597761526325,-1.3744489535490383,-1.3548251796753001,-1.3353929574017562,-1.3161567717874325,-1.297121083316658,-1.2782903210928933,
-1.2596688760155905,-1.2412610939661868,-1.223071269029427,-1.2051036367761088,-1.1873623676330658,-1.1698515603657169,-1.1525752356978667,-1.1355373300926102,-1.1187416897172127,-1.1021920646136958,-1.0858921030955901,
-1.0698453463899178,-1.0540552235419711,-1.0385250465988529,-1.0232580060860892,-1.008257166789887,-0.99352546385585916,-0.97906569921324083,-0.96488053833182952,-0.950972507317092,-0.93734399034711491,-0.92399722745334667,
-0.91093431264539726,-0.89815719237854208,-0.88566766436102573,-0.87346737669679608,-0.86155782735790942,-0.84994036397956818,-0.83861618396955728,-0.82758633492276357,-0.81685171533047785,-0.80641307557331188,-0.79627101918579057,
-0.78642600438002275,-0.77687834581529969,-0.76762821660001734,-0.75867565051196528,-0.75002054442276811,-0.74166266091209987,-0.73360163105720833,-0.72583695738329068,-0.71836801696033514,-0.711194064632189,-0.70431423636382573,
-0.697727552693046,-0.69143292227317277,-0.68542914549366318,-0.67971491816596585,-0.67428883526239536,-0.66914939469626766,-0.66429500113203244,-0.65972396981465831,-0.65543453040805422,-0.65142483083285541,-0.64769294109444819,
-0.64423685709266176,-0.6410545044051067,-0.63814374203669011,-0.63550236612837607,-0.63312811361880172,-0.6310186658528758,-0.62917165213200321,-0.62758465320107548,-0.62625520466784568,-0.62518080035077561,-0.62435889555188862,
-0.6237869102515875,-0.62346223222281172,-0.62338222006229338,-0.62354420613704464,-0.62394549944455868,-0.62458338838553717,-0.62545514344826691,-0.62655801980406267,-0.62788925981346089,-0.62944609544310681,-0.63122575059350894,
-0.6332254433380553,-0.63544238807388154,-0.63787379758537022,-0.64051688502122106,-0.64336886578619024,-0.646426959348729,-0.64968839096587816,-0.653150393326885,-0.65681020811710222,-0.66066508750382125,-0.664712295545759,
-0.668949109527984,-0.673372821224124,-0.6779807380877334,-0.682770184374744,-0.68773850219894261,-0.692883052522442,-0.69820121608312535,-0.70369039426104785,-0.7093480098857835,-0.715171507986697,-0.72115835648811266,
-0.72730604685133848,-0.73361209466548238,-0.74007404018898026,-0.746689448843725,-0.75345591166366066,-0.76037104569967551,-0.7674324943825912,-0.77463792784601326,-0.78198504321077023,-0.78947156483262981,-0.79709524451494163},
new double[]{
-6.8962727430887938,-6.8724633044469448,-6.8486538699587864,-6.8248444398268973,-6.8010350142637339,-6.7772255934921155,-6.7534161777457262,-6.7296067672696491,-6.7057973623209186,-6.6819879631691039,-6.6581785700969229,-6.6343691834008789,
-6.6105598033919382,-6.586750430396231,-6.5629410647557922,-6.5391317068293393,-6.5153223569930816,-6.4915130156415772,-6.4677036831886268,-6.4438943600682093,-6.42008504673547,-6.3962757436677506,-6.3724664513656712,
-6.3486571703542669,-6.3248479011841763,-6.3010386444328921,-6.2772294007060712,-6.253420170638905,-6.229610954897562,-6.2058017541806985,-6.1819925692210393,-6.1581834007870455,-6.13437424968465,-6.11056511675909,
-6.08675600289682,-6.0629469090275245,-6.0391378361262236,-6.015328785215484,-5.9915197573677377,-5.9677107537077134,-5.9439017754149868,-5.9200928237266535,-5.8962838999401352,-5.87247500541612,-5.848666141581643,
-5.82485730993333,-5.8010485120407838,-5.7772397495501417,-5.7534310241878082,-5.72962233776437,-5.7058136921786966,-5.6820050894222458,-5.6581965315835756,-5.6343880208530805,-5.6105795595279515,-5.5867711500173849,
-5.56296279484804,-5.5391544966697639,-5.5153462582616,-5.4915380825380762,-5.4677299725558166,-5.4439219315204621,-5.4201139627939332,-5.3963060699020451,-5.372498256542495,-5.348690526593237,-5.3248828841212648,
-5.3010753333918226,-5.2772678788780638,-5.2534605252711781,-5.2296532774910185,-5.2058461406972318,-5.1820391203009466,-5.1582322219770163,-5.1344254516768677,-5.110618815641967,-5.0868123204179447,-5.0630059728694077,
-5.0391997801954656,-5.0153937499460222,-4.9915878900388462,-4.967782208777483,-4.9439767148700264,-4.9201714174488105,-4.8963663260910515,-4.872561450840494,-4.8487568022301106,-4.8249523913058985,-4.8011482296518322,
-4.7773443294160263,-4.7535407033381709,-4.7297373647782885,-4.70593432774689,-4.6821316069365944,-4.6583292177552709,-4.6345271763607947,-4.6107254996974767,-4.5869242055342649,-4.5631233125047794,-4.5393228401493,
-4.5155228089587629,-4.4917232404209013,-4.467924157068599,-4.44412558253058,-4.4203275415845491,-4.3965300602128838,-4.3727331656610122,-4.3489368864986062,-4.325141252683709,-4.3013462956299486,-4.2775520482769842,
-4.2537585451643283,-4.22996582250871,-4.20617391828515,-4.18238287231191,-4.1585927263395179,-4.1348035241440275,-4.1110153116247545,-4.0872281369066554,-4.063442050447593,-4.0396571051507006,-4.0158733564820928,
-3.9920908625941558,-3.9683096844546908,-3.9445298859821594,-3.9207515341873322,-3.8969746993216146,-3.873199455032363,-3.849425878525508,-3.8256540507358077,-3.8018840565050835,-3.7781159847687888,-3.7543499287512829,
-3.730585986170202,-3.70682425945032,-3.6830648559473218,-3.6593078881819219,-3.6355534740847726,-3.611801737252633,-3.5880528072162776,-3.5643068197206444,-3.54056391701774,-3.5168242481728389,-3.4930879693845212,
-3.4693552443191287,-3.4456262444602221,-3.4219011494736429,-3.3981801475888087,-3.3744634359968804,-3.3507512212664565,-3.3270437197774783,-3.3033411581740237,-3.2796437738367104,-3.2559518153754188,-3.2322655431430762,
-3.2085852297712467,-3.184911160728285,-3.1612436349008268,-3.1375829651993818,-3.1139294791888212,-3.0902835197445349,-3.0666454457350434,-3.0430156327318532,-3.0193944737473171,-2.9957823800012751,-2.9721797817172182,
-2.9485871289487111,-2.9250048924367724,-2.9014335644988956,-2.8778736599503443,-2.8543257170583187,-2.8307902985295388,-2.80726799253173,-2.7837594137494279,-2.7602652044744431,-2.7367860357312437,-2.7133226084374051,
-2.6898756545991822,-2.6664459385421182,-2.6430342581764878,-2.6196414462972171,-2.5962683719177497,-2.5729159416371732,-2.5495851010396984,-2.5262768361254029,-2.5029921747708954,-2.4797321882183381,-2.4564979925909829,
-2.4332907504331094,-2.4101116722719502,-2.3869620181988718,-2.3638430994667528,-2.3407562801001376,-2.3177029785143914,-2.2946846691396874,-2.2717028840452604,-2.2487592145589588,-2.2258553128766922,-2.2029928936559453,
-2.1801737355870965,-2.1573996829358184,-2.1346726470494177,-2.1119946078195064,-2.0893676150929847,-2.0667937900228694,-2.0442753263501174,-2.021814491607187,-1.9994136282337278,-1.97707515459447,-1.9548015658890716,
-1.9325954349434562,-1.9104594128719683,-1.8883962295995358,-1.8664086942329488,-1.8444996952703667,-1.8226722006382257,-1.8009292575448677,-1.7792739921404575,-1.7577096089730693,-1.7362393902312556,-1.714866694763924,
-1.6935949568689754,-1.6724276848428725,-1.6513684592841385,-1.6304209311447069,-1.609588819524072,-1.5888759092023048,-1.5682860479092129,-1.5478231433282153,-1.5274911598348713,-1.5072941149714405,-1.4872360756603429,
-1.4673211541609259,-1.4475535037755152,-1.4279373143123098,-1.4084768073142813,-1.3891762310648088,-1.3700398553823396,-1.3510719662178743,-1.3322768600705301,-1.3136588382378114,-1.2952222009185106,-1.2769712411873493,
-1.2589102388615376,-1.2410434542803841,-1.223375122019879,-1.2059094445648482,-1.1886505859617593,-1.1716026654756211,-1.1547697512745772,-1.1381558541658141,-1.121764921406246,-1.1056008306111067,-1.0896673837831092,
-1.0739683014841805,-1.0585072171709991,-1.0432876717146344,-1.0283131081235242,-1.0135868664878596,-0.99911217916215855,-0.98489216620144282,-0.97092983106497632,-0.957228056600012,-0.94378960131642875,-0.93061709596154185,
-0.9177130404027487,-0.90507980082405182,-0.89271960724087906,-0.88063455133602553,-0.86882658461797946,-0.85729751690137079,-0.84604901510781727,-0.83508260238404,-0.82439965753278821,-0.81400141475086285,-0.80388896366735685,
-0.79406324967415365,-0.78452507453973985,-0.775275097296492,-0.76631383539080977,-0.75764166608476247,-0.7492588280973228,-0.74116542347274994,-0.73336141966327528,-0.72584665181292207,-0.71862082522905579,-0.71168351802811147,
-0.70503418394187289,-0.69867215527068172,-0.69259664597002768,-0.6868067548571084,-0.681301468924144,-0.67607966674548314,-0.67114012196583661,-0.66648150685731822,-0.66210239593335263,-0.65800126960792782,-0.65417651788910769,
-0.65062644409619341,-0.64734926859040165,-0.64434313250943231,-0.64160610149680841,-0.63913616941739,-0.63693126205098471,-0.63498924075650176,-0.63330790609961563,-0.63188500143742188,-0.63071821645407189,-0.62980519064187679,
-0.62914351672285207,-0.62873074400615125,-0.62856438167729423,-0.62864190201554182,-0.6289607435361908,-0.62951831405497782,-0.63031199367216773,-0.63133913767427907,-0.6325970793517498,-0.63408313273118422,-0.63579459522113946,
-0.63772875017070374,-0.639882869340403,-0.64225421528522808,-0.64484004364982306,-0.64763760537609383,-0.6506441488237118,-0.65385692180417165,-0.657273173529244,-0.66089015647481819,-0.66470512816128213,-0.66871535285170891,
-0.672918103169246,-0.67731066163520148,-0.68189032212941536,-0.68665439127458816,-0.69160018974630266,-0.69672505351054115,-0.70202633499054334,-0.7075014041648986,-0.71314764959879018,-0.71896247941034219,-0.72494332217402924,
-0.7310876277631253,-0.73739286813316929,-0.74385653804842444,-0.75047615575330051,-0.75724926359069888,-0.76417342856922166,-0.77124624288116717,-0.77846532437321248,-0.78582831697165278,-0.793332891064043,-0.80097674383905049},
new double[]{
-6.6448049377902523,-6.6220777871303893,-6.5993506421470345,-6.5766235031041616,-6.5538963702780189,-6.5311692439577,-6.5084421244457422,-6.4857150120587512,-6.4629879071280572,-6.4402608100003951,-6.4175337210386241,-6.3948066406224786,
-6.3720795691493493,-6.349352507035106,-6.3266254547149581,-6.3038984126443509,-6.28117138129991,-6.258444361180425,-6.2357173528078755,-6.2129903567285174,-6.1902633735140054,-6.1675364037625782,-6.1448094481002906,
-6.1220825071823093,-6.0993555816942662,-6.0766286723536753,-6.0539017799114143,-6.0311749051532741,-6.0084480489015863,-5.9857212120169194,-5.9629943953998552,-5.9402675999928523,-5.9175408267821927,-5.8948140768000155,
-5.8720873511264537,-5.8493606508918621,-5.8266339772791547,-5.803907331526247,-5.78118071492861,-5.7584541288419517,-5.7357275746850132,-5.7130010539425005,-5.6902745681681495,-5.6675481189879386,-5.644821708103442,
-5.6220953372953453,-5.5993690084271268,-5.5766427234488987,-5.5539164844014408,-5.5311902934204094,-5.5084641527407525,-5.4857380647013185,-5.4630120317496891,-5.4402860564472322,-5.41756014147439,-5.3948342896362131,
-5.3721085038681462,-5.3493827872420976,-5.32665714297277,-5.3039315744243023,-5.2812060851172111,-5.2584806787356548,-5.235755359135041,-5.2130301303499778,-5.1903049966026007,-5.1675799623112857,-5.1448550320997626,
-5.1221302108066569,-5.0994055034954684,-5.0766809154650163,-5.053956452260369,-5.03123211968428,-5.0085079238091517,-4.9857838709895592,-4.9630599678753482,-4.9403362214253494,-4.9176126389217147,-4.8948892279849332,
-4.8721659965895308,-4.8494429530805041,-4.8267201061905105,-4.8039974650578614,-4.7812750392453385,-4.7585528387598908,-4.735830874073236,-4.713109156143422,-4.6903876964373756,-4.6676665069545056,-4.6449456002513907,
-4.6222249894676057,-4.5995046883527513,-4.57678471129472,-4.5540650733492765,-4.5313457902710006,-4.508626878545658,-4.4859083554240637,-4.463190238957508,-4.4404725480348173,-4.4177553024211189,-4.3950385227983926,
-4.3723222308078924,-4.3496064490945132,-4.3268912013532077,-4.30417651237753,-4.2814624081104071,-4.2587489156972547,-4.2360360635415146,-4.2133238813627489,-4.1906124002573959,-4.167901652762307,-4.1451916729211931,
-4.1224824963541034,-4.099774160330095,-4.0770667038432018,-4.0543601676918826,-4.0316545945620863,-4.0089500291140991,-3.9862465180733442,-3.9635441103253073,-3.940842857014772,-3.9181428116495538,-3.8954440302089366,
-3.8727465712570082,-3.8500504960611228,-3.8273558687157014,-3.804662756271616,-3.7819712288713871,-3.75928135989046,-3.7365932260848109,-3.713906907745161,-3.6912224888580822,-3.6685400572742841,-3.645859704884395,
-3.6231815278025445,-3.600505626558085,-3.5778321062957863,-3.5551610769848643,-3.5324926536371981,-3.5098269565351288,-3.4871641114692196,-3.4645042499863878,-3.4418475096488321,-3.4191940343041765,-3.3965439743672889,
-3.37389748711423,-3.3512547369888042,-3.328615895922205,-3.3059811436662581,-3.2833506681407734,-3.2607246657955389,-3.2381033419875025,-3.2154869113736892,-3.1928755983204327,-3.1702696373294947,-3.1476692734816671,
-3.12507476289846,-3.1024863732224852,-3.0799043841171589,-3.0573290877863539,-3.0347607895146265,-3.0121998082286674,-2.9896464770806128,-2.9671011440538537,-2.9445641725919924,-2.9220359422515703,-2.8995168493792041,
-2.8770073078137437,-2.854507749614057,-2.8320186258130273,-2.8095404071983268,-2.7870735851205071,-2.7646186723289112,-2.7421762038358763,-2.719746737809658,-2.6973308564964484,-2.6749291671718156,-2.6525423031218263,
-2.6301709246540268,-2.6078157201384116,-2.585477407078379,-2.5631567332116139,-2.5408544776407003,-2.5185714519931666,-2.4963085016105291,-2.4740665067657539,-2.4518463839084168,-2.4296490869366574,-2.40747560849486,
-2.3853269812957882,-2.3632042794657009,-2.3411086199107531,-2.3190411637027526,-2.2970031174820829,-2.2749957348753593,-2.2530203179250838,-2.2310782185282849,-2.2091708398808256,-2.1872996379237288,-2.1654661227875613,
-2.1436718602305547,-2.1219184730658047,-2.1002076425725309,-2.078541109886006,-2.0569206773604054,-2.0353482098984532,-2.0138256362413727,-1.9923549502122797,-1.9709382119058161,-1.9495775488164535,-1.9282751568975818,
-1.9070333015431806,-1.8858543184835805,-1.864740614586567,-1.843694668554847,-1.8227190315107127,-1.8018163274585932,-1.7809892536160812,-1.7602405806039845,-1.7395731524859639,-1.7189898866483964,-1.6984937735112518,
-1.6780878760609861,-1.6577753291967519,-1.6375593388816003,-1.6174431810908043,-1.5974302005499774,-1.5775238092562844,-1.5577274847767588,-1.5380447683185339,-1.5184792625666845,-1.4990346292863312,-1.4797145866867065,
-1.4605229065459937,-1.4414634110969284,-1.4225399696743939,-1.4037564951275272,-1.3851169400001966,-1.3666252924850595,-1.3482855721578066,-1.3301018254995864,-1.3120781212169925,-1.2942185453703714,-1.2765271963225497,
-1.2590081795213781,-1.2416656021307335,-1.2245035675257967,-1.2075261696695079,-1.1907374873881074,-1.1741415785645566,-1.1577424742694153,-1.141544172849404,-1.1255506339944006,-1.1097657728040136,-1.0941934538751064,
-1.0788374854317528,-1.0637016135190514,-1.0487895162820291,-1.03410479835053,-1.019650985350492,-1.0054315185614076,-0.99144974973900568,-0.977708936121329,-0.96421223563538694,-0.95096270232048508,-0.93796328198314438,
-0.9252168080972637,-0.912725997961849,-0.90049344912724383,-0.88852163609936607,-0.87681290732999484,-0.8653694824996736,-0.85419345009830927,-0.843286765307075,-0.83265124818375846,-0.82228858215227651,-0.81220031279568139,
-0.80238784695064425,-0.79285245210011979,-0.78359525605967384,-0.77461724695180789,-0.76591927346154487,-0.757502045365545,-0.74936613432611787,-0.74151197494067367,-0.73393986603642636,-0.726649972199516,-0.71964232552716678,
-0.71291682759102637,-0.70647325159945429,-0.70031124474622874,-0.69443033073292737,-0.68882991245209457,-0.68350927481824864,-0.67846758773378368,-0.6737039091768916,-0.66921718839876243,-0.66500626921750472,-0.66106989339647071,
-0.65740670409494872,-0.65401524937951661,-0.65089398578470814,-0.64804128191203969,-0.6454554220568659,-0.64313460985297555,-0.64107697192530289,-0.63928056154160551,-0.63774336225445,-0.636463291525339,-0.63543820432331366,
-0.63466589669086881,-0.63414410927051024,-0.63387053078578193,-0.633842801471079,-0.63405851644503408,-0.63451522902274182,-0.63521045396253206,-0.63614167064345217,-0.63730632617004024,-0.63870183840138617,-0.64032559890186858,
-0.64217497581133831,-0.64424731663287294,-0.646539950936574,-0.64905019297820021,-0.65177534423173489,-0.65471269583527625,-0.657859530949904,-0.661213127031435,-0.66477075801520469,-0.66852969641424009,-0.67248721533138056,
-0.67664059038609337,-0.68098710155689823,-0.68552403494046787,-0.69024868442861265,-0.69515835330448139,-0.70025035575942551,-0.70552201833206851,-0.71097068127121632,-0.716593699824316,-0.72238844545323855,-0.72835230697921538,
-0.73448269165880642,-0.74077702619281316,-0.74723275767007868,-0.7538473544481421,-0.76061830697272181,-0.76754312853801765,-0.77461935598981657,-0.78184455037338774,-0.78921629752813893,-0.79673220863099647,-0.80438992069044923},
new double[]{
-6.4150824559662079,-6.39334349571331,-6.3716045430227561,-6.3498655982305907,-6.3281266616877874,-6.3063877337609178,-6.2846488148328419,-6.2629099053034309,-6.241171005590326,-6.2194321161297248,-6.1976932373772087,-6.1759543698086015,
-6.15421551392087,-6.1324766702330642,-6.1107378392872986,-6.0889990216497729,-6.0672602179118496,-6.0455214286911634,-6.0237826546327975,-6.0020438964104947,-5.9803051547279358,-5.95856643032007,-5.9368277239545,
-5.9150890364329367,-5.8933503685927144,-5.8716117213083692,-5.8498730954932983,-5.82813449210148,-5.8063959121292834,-5.7846573566173438,-5.7629188266525357,-5.7411803233700249,-5.7194418479554132,-5.6977034016469794,
-5.6759649857380188,-5.6542266015792881,-5.6324882505815586,-5.61074993421828,-5.5890116540283659,-5.5672734116191034,-5.5455352086691869,-5.5237970469318913,-5.5020589282383856,-5.4803208545011923,-5.4585828277178008,
-5.43684484997444,-5.4151069234500211,-5.3933690504202536,-5.3716312332619447,-5.3498934744574864,-5.3281557765995489,-5.3064181423959758,-5.2846805746748977,-5.2629430763900737,-5.2412056506264717,-5.2194683006060956,
-5.1977310296940633,-5.17599384140497,-5.1542567394095169,-5.1325197275414469,-5.1107828098047818,-5.08904599038138,-5.0673092736388314,-5.0455726641387022,-5.023836166645145,-5.0020997861338907,-4.9803635278016385,
-4.9586273970758619,-4.9368913996250514,-4.9151555413694075,-4.8934198284920125,-4.8716842674504894,-4.8499488649891891,-4.8282136281519019,-4.80647856429515,-4.7847436811020509,-4.763008986596807,-4.7412744891598315,
-4.7195401975435445,-4.6978061208888651,-4.676072268742443,-4.6543386510746378,-4.6326052782983052,-4.6108721612884072,-4.5891393114024925,-4.567406740502082,-4.5456744609749951,-4.523942485758667,-4.5022108283644924,
-4.480479502903246,-4.4587485241116234,-4.4370179073799516,-4.4152876687811284,-4.3935578251008272,-4.37182839386904,-4.3500993933930134,-4.3283708427916228,-4.3066427620312728,-4.2849151719633687,-4.2631880943634437,
-4.241461551971998,-4.2197355685371374,-4.1980101688590858,-4.1762853788366483,-4.1545612255157147,-4.1328377371398917,-4.1111149432033454,-4.0893928745059744,-4.0676715632109781,-4.045951042904961,-4.0242313486606509,
-4.00251251710236,-3.9807945864743108,-3.9590775967119263,-3.9373615895162395,-3.9156466084315351,-3.89393269892637,-3.8722199084781121,-3.8505082866611504,-3.8287978852389291,-3.8070887582599648,-3.785380962158023,
-3.7636745558566154,-3.7419696008780128,-3.7202661614569519,-3.6985643046592358,-3.6768641005054326,-3.6551656220998781,-3.6334689457652076,-3.61177415118264,-3.5900813215382477,-3.568390543675465,-3.5467019082540778,
-3.5250155099159635,-3.503331447457851,-3.4816498240113836,-3.45997074723077,-3.4382943294883326,-3.4166206880782606,-3.3949499454288872,-3.3732822293238249,-3.3516176731323069,-3.3299564160490771,-3.3082986033442041,
-3.28664438662319,-3.2649939240977561,-3.2433473808677173,-3.2217049292143347,-3.2000667489055812,-3.178433027513746,-3.1568039607458149,-3.1351797527870837,-3.1135606166584657,-3.0919467745879619,-3.0703384583967761,
-3.0487359099005698,-3.0271393813263452,-3.005549135745472,-2.9839654475233655,-2.9623886027863326,-2.9408188999061142,-2.9192566500026453,-2.8977021774655647,-2.8761558204949997,-2.8546179316621587,-2.8330888784902539,
-2.8115690440562693,-2.7900588276140987,-2.7685586452395405,-2.74706893049765,-2.7255901351329181,-2.7041227297827279,-2.6826672047145261,-2.6612240705871062,-2.6397938592363834,-2.6183771244859888,-2.5969744429829866,
-2.57558641505896,-2.554213665616659,-2.5328568450423568,-2.5115166301439826,-2.4901937251150303,-2.4688888625241736,-2.4476028043304128,-2.4263363429234972,-2.405090302189262,-2.3838655385993883,-2.3626629423249974,
-2.3414834383733374,-2.3203279877466838,-2.2991975886224236,-2.2780932775531224,-2.2570161306852046,-2.2359672649946765,-2.2149478395381434,-2.193959056717135,-2.1730021635535559,-2.1520784529738246,-2.13118926509903,
-2.1103359885381803,-2.089520061681339,-2.0687429739891825,-2.0480062672752233,-2.0273115369766437,-2.0066604334093974,-1.9860546630029192,-1.965495989509489,-1.9449862351829688,-1.9245272819213388,-1.9041210723671367,
-1.8837696109596132,-1.8634749649321127,-1.8432392652479095,-1.8230647074674575,-1.8029535525397611,-1.7829081275103387,-1.7629308261380456,-1.7430241094128494,-1.7231905059664947,-1.7034326123678991,-1.6837530932950442,
-1.6641546815751085,-1.644640178084622,-1.625212451501491,-1.6058744379008965,-1.5866291401872568,-1.567479627354716,-1.5484290335689526,-1.5294805570634948,-1.510637458844216,-1.4919030611962119,-1.4732807459878932,
-1.4547739527678107,-1.4363861766504911,-1.4181209659884013,-1.3999819198280528,-1.38197268514922,-1.3640969538872729,-1.3463584597396954,-1.3287609747589819,-1.3113083057352704,-1.2940042903732529,-1.2768527932691296,
-1.2598577016945867,-1.2430229211960189,-1.2263523710184245,-1.2098499793646127,-1.1935196785015199,-1.1773653997265696,-1.1613910682080733,-1.1456005977146815,-1.1299978852498296,-1.1145868056079606,-1.0993712058700711,
-1.0843548998567625,-1.0695416625575265,-1.0549352245554104,-1.0405392664665032,-1.0263574134138589,-1.0123932295555118,-0.99865021268615206,-0.98513178893181041,-0.97184130755655407,-0.95878203589972433,-0.94595715446165218,
-0.93336975215507711,-0.92102282173868111,-0.90891925544822449,-0.897061840839764,-0.88545325685833587,-0.87409607014432289,-0.86299273158849177,-0.85214557314541428,-0.84155680491366025,-0.83122851248981178,-0.82116265460198035,
-0.81136106102714423,-0.8018254307952617,-0.79255733068176892,-0.7835581939887557,-0.7748293196138234,-0.76637187140439511,-0.75818687779405547,-0.75027523171637056,-0.74263769079057174,-0.7352748777724929,-0.728187281263227,
-0.72137525666712676,-0.71483902739000749,-0.70857868626772613,-0.70259419721471283,-0.69688539708151054,-0.69145199770994048,-0.68629358817415642,-0.68140963719557068,-0.67679949571943288,-0.67246239964071264,-0.66839747266687588,
-0.66460372930515288,-0.66108007796196089,-0.657825324142271,-0.65483817373688635,-0.652117236385828,-0.64966102890629462,-0.64746797877397422,-0.64553642764683383,-0.64386463492089,-0.64245078130786792,-0.64129297242508232,
-0.6403892423883204,-0.639737557398965,-0.6393358193170684,-0.63918186921256293,-0.63927349088727914,-0.63960841436092442,-0.64018431931465869,-0.64099883848638,-0.64204956101230715,-0.6433340357099101,-0.64484977429769064,
-0.646594254547762,-0.64856492336760274,-0.65075919980777608,-0.65317447799280648,-0.65580812997278926,-0.6586575084936771,-0.6617199496845354,-0.664992775660396,-0.66847329703964953,-0.67215881537521649,-0.67604662549901806,
-0.68013401777952776,-0.6844182802924319,-0.68889670090465283,-0.69356656927220206,-0.69842517875252419,-0.70346982823217052,-0.708697823870806,-0.71410648076270267,-0.71969312451700362,-0.72545509275816689,-0.73138973654810246,
-0.7374944217316145,-0.74376653020683947,-0.75020346112244918,-0.75680263200344355,-0.76356147980741407,-0.77047746191319921,-0.777548057043886,-0.78477076612613961,-0.79214311308786023,-0.799662645596174,-0.80732693573777459},
new double[]{
-6.2044996045701382,-6.1836665027358979,-6.1628334107511016,-6.1420003290348006,-6.1211672580238767,-6.1003341981737966,-6.0795011499594063,-6.0586681138757523,-6.0378350904389437,-6.017002080187047,-5.9961690836810195,-5.9753361015056825,
-5.954503134270742,-5.9336701826118379,-5.9128372471916553,-5.8920043287010726,-5.8711714278603591,-5.850338545420426,-5.8295056821641325,-5.8086728389076407,-5.7878400165018347,-5.7670072158337975,-5.7461744378283512,
-5.72534168344966,-5.7045089537029048,-5.683676249636032,-5.662843572341564,-5.6420109229585043,-5.6211783026743083,-5.6003457127269458,-5.579513154407052,-5.5586806290601638,-5.537848138089057,-5.5170156829561821,
-5.4961832651861986,-5.4753508863686235,-5.4545185481605891,-5.43368625228972,-5.412854000557128,-5.3920217948405389,-5.3711896370975509,-5.3503575293690337,-5.3295254737826623,-5.3086934725566168,-5.2878615280034262,
-5.2670296425339815,-5.2461978186617158,-5.22536605900697,-5.2045343663015338,-5.1837027433933853,-5.16287119325163,-5.1420397189716534,-5.121208323780488,-5.1003770110424078,-5.0795457842647673,-5.05871464710408,
-5.0378836033723582,-5.0170526570437284,-4.9962218122613153,-4.9753910733444267,-4.9545604447960443,-4.9337299313106264,-4.9128995377822449,-4.89206926931307,-4.8712391312222136,-4.8504091290549418,-4.82957926859229,
-4.808749555861076,-4.7879199971443427,-4.7670905989922412,-4.7462613682333785,-4.7254323119866459,-4.7046034376735415,-4.68377475303103,-4.66294626612493,-4.6421179853638845,-4.621289919513913,-4.6004620777135825,
-4.5796344694898252,-4.55880710477442,-4.5379799939211738,-4.5171531477238345,-4.4963265774347558,-4.4755002947843554,-4.4546743120013934,-4.4338486418341132,-4.4130232975722725,-4.3921982930701029,-4.37137364277025,
-4.3505493617287074,-4.3297254656408271,-4.3089019708684049,-4.2880788944679216,-4.2672562542199781,-4.2464340686599611,-4.225612357110009,-4.2047911397123245,-4.1839704374638913,-4.1631502722526461,-4.1423306668951811,
-4.1215116451760272,-4.1006932318885934,-4.0798754528778218,-4.0590583350846368,-4.0382419065922628,-4.01742619667448,-3.9966112358459078,-3.9757970559143936,-3.9549836900355966,-3.9341711727698536,-3.913359540141427,
-3.8925488297002246,-3.8717390805860994,-3.8509303335958296,-3.8301226312528955,-3.8093160178801568,-3.7885105396755576,-3.7677062447909804,-3.7469031834143705,-3.7261014078552703,-3.7053009726338995,-3.6845019345739205,
-3.6637043528990394,-3.6429082893336,-3.6221138082073203,-3.6013209765643475,-3.580529864276798,-3.559740544162957,-3.5389530921103267,-3.5181675872037137,-3.4973841118585476,-3.4766027519596405,-3.4558235970055984,
-3.4350467402590987,-3.4142722789032658,-3.3935003142043776,-3.3727309516811381,-3.3519643012807792,-3.3312004775622315,-3.3104395998866507,-3.2896817926155553,-3.2689271853168709,-3.2481759129791681,-3.2274281162343925,
-3.2066839415894024,-3.1859435416666235,-3.1652070754541537,-3.1444747085656486,-3.1237466135103391,-3.1030229699735239,-3.0823039651079083,-3.06158979383615,-3.040880659165,-3.0201767725114186,-2.9994783540410581,
-2.9787856330195219,-2.9580988481768018,-2.937418248085311,-2.916744091551934,-2.896076648024517,-2.8754161980132324,-2.8547630335272536,-2.8341174585271678,-2.8134797893935817,-2.7928503554123423,-2.7722294992768273,
-2.7516175776077287,-2.7310149614907733,-2.7104220370327989,-2.6898392059366132,-2.6692668860950417,-2.6487055122045611,-2.6281555363989098,-2.6076174289030347,-2.5870916787077238,-2.5665787942652525,-2.5460793042063288,
-2.5255937580786205,-2.5051227271070773,-2.4846668049762557,-2.464226608634787,-2.4438027791220951,-2.4233959824174045,-2.4030069103110363,-2.3826362812979025,-2.3622848414930657,-2.3419533655691254,-2.3216426577151252,
-2.30135355261658,-2.2810869164561165,-2.2608436479341232,-2.2406246793086773,-2.2204309774539039,-2.2002635449357864,-2.1801234211043004,-2.1600116832005973,-2.1399294474778041,-2.119877870333835,-2.0998581494544277,
-2.0798715249644375,-2.059919280585214,-2.0400027447956872,-2.0201232919945724,-2.0002823436608712,-1.9804813695096257,-1.9607218886396356,-1.9410054706696038,-1.9213337368589265,-1.9017083612090808,-1.882131071541316,
-1.8626036505460746,-1.8431279367993254,-1.8237058257407166,-1.8043392706082047,-1.7850302833235614,-1.7657809353229179,-1.7465933583262643,-1.7274697450396097,-1.7084123497832935,-1.6894234890397559,-1.6705055419139061,
-1.6516609504990887,-1.6328922201415359,-1.6142019195961059,-1.5955926810660772,-1.5770672001197408,-1.5586282354765908,-1.5402786086559746,-1.522021203481208,-1.5038589654323329,-1.4857949008409379,-1.4678320759207477,
-1.4499736156280481,-1.4322227023464149,-1.4145825743907003,-1.3970565243257564,-1.3796478970959862,-1.3623600879624604,-1.3451965402450732,-1.3281607428679776,-1.3112562277073887,-1.2944865667417225,-1.2778553690049781,
-1.26136627734525,-1.2450229649912705,-1.2288291319309328,-1.2127885011068105,-1.1969048144347756,-1.1811818286529088,-1.1656233110089831,-1.1502330347958829,-1.13501477474537,-1.1199723022916428,-1.1051093807171109,
-1.0904297601937447,-1.0759371727342373,-1.0616353270680123,-1.0475279034578464,-1.0336185484735125,-1.0199108697393962,-1.0064084306734888,-0.9931147452355028,-0.980033272702083,-0.9671674124872145,-0.954520499025926,
-0.942095796739274,-0.9298964950983708,-0.91792570380486449,-0.90618644810483218,-0.89468166425247364,-0.88341419513932817,-0.87238678610396592,-0.86160208093624813,-0.8510626180893045,-0.840770827111363,-0.83072902530847614,
-0.82093941464805542,-0.81140407891192978,-0.80212498110642794,-0.79310396113573,-0.78434273374347,-0.77584288672630131,-0.76760587942186786,-0.75963304147237465,-0.75192557186371745,-0.74448453823893945,-0.73731087648361648,
-0.73040539057966924,-0.72376875272303509,-0.71740150369963729,-0.711304053513153,-0.70547668225721338,-0.69991954122387479,-0.69463265423947673,-0.68961591921835819,-0.684869109924334,-0.68039187792934108,-0.67618375475824755,
-0.672244154208482,-0.66857237483287013,-0.66516760257387464,-0.66202891353731,-0.65915527689354514,-0.65654555789421276,-0.65419852099250653,-0.65211283305527112,-0.65028706665525837,-0.648719703432142,-0.64740913751114759,
-0.64635367896845253,-0.6455515573328473,-0.64500092511351637,-0.64469986134418489,-0.64464637513429779,-0.6448384092183238,-0.64527384349472927,-0.64595049854662168,-0.64686613913653079,-0.6480184776682647,-0.64940517760925121,
-0.65102385686724684,-0.65287209111576272,-0.65494741706301884,-0.65724733565969584,-0.65976931524119287,-0.66251079460054052,-0.66546918598853388,-0.66864187803806208,-0.67202623861000166,-0.67561961755841926,-0.67941934941318993,
-0.68342275597848245,-0.68762714884588827,-0.69202983182128375,-0.69662810326480351,-0.70141925834358121,-0.70640059119716958,-0.71156939701579114,-0.71692297403179517,-0.72245862542490347,-0.728173661142016,-0.73406539963252238,
-0.74013116950022706,-0.746368311073135,-0.75277417789248047,-0.759346138122495,-0.76608157588251369,-0.77297789250311311,-0.78003250770804966,-0.787242860723839,-0.79460641131887177,-0.80212064077401124,-0.8097830527866553},
new double[]{
-6.010849919831311,-5.99085022790107,-5.9708505485431935,-5.9508508822707551,-5.9308512296177627,-5.910851591140017,-5.8908519674159994,-5.8708523590477979,-5.85085276666207,-5.8308531909110481,-5.8108536324735764,-5.7908540920562039,
-5.7708545703943086,-5.7508550682532817,-5.730855586429743,-5.710856125752823,-5.6908566870854829,-5.6708572713259011,-5.650857879408905,-5.6308585123074693,-5.6108591710342717,-5.5908598566433119,-5.5708605702315994,
-5.5508613129409072,-5.5308620859595967,-5.51086289052452,-5.490863727923,-5.4708645994948855,-5.4508655066346963,-5.4308664507938547,-5.4108674334830047,-5.3908684562744291,-5.3708695208045629,-5.3508706287766126,
-5.3308717819632783,-5.3108729822095873,-5.2908742314358479,-5.2708755316407183,-5.2508768849043994,-5.2308782933919691,-5.2108797593568346,-5.1908812851443411,-5.1708828731955219,-5.150884526050997,-5.130886246355038,
-5.1108880368597962,-5.0908899004296968,-5.0708918400460252,-5.0508938588116852,-5.0308959599561645,-5.0108981468406935,-4.9909004229636151,-4.9709027919659805,-4.9509052576373644,-4.9309078239219213,-4.9109104949246882,
-4.8909132749181436,-4.8709161683490318,-4.8509191798454694,-4.8309223142243365,-4.8109255764989722,-4.7909289718871815,-4.7709325058195686,-4.7509361839482107,-4.73094001215568,-4.7109439965644437,-4.69094814354663,
-4.670952459734206,-4.6509569520295635,-4.6309616276165366,-4.6109664939718638,-4.5909715588771229,-4.5709768304311424,-4.5509823170629291,-4.5309880275451064,-4.5109939710079123,-4.4910001569537581,-4.4710065952723808,
-4.45101329625661,-4.4310202706187756,-4.4110275295077823,-4.3910350845268775,-4.3710429477521355,-4.3510511317516976,-4.33105964960579,-4.3110685149275518,-4.2910777418847088,-4.271087345222127,-4.251097340285277,
-4.2311077430446487,-4.21111857012116,-4.1911298388125839,-4.1711415671210563,-4.1511537737816839,-4.1311664782923234,-4.1111797009445494,-4.0911934628558839,-4.0712077860033222,-4.0512226932582136,-4.0312382084225495,
-4.0112543562667149,-3.9912711625687614,-3.9712886541552606,-3.9513068589438065,-3.9313258059872243,-3.9113455255195588,-3.8913660490039135,-3.8713874091822071,-3.85140964012693,-3.8314327772949781,-3.8114568575836421,
-3.7914819193888412,-3.7715080026656889,-3.7515351489914766,-3.7315634016311812,-3.7115928056055814,-3.6916234077620964,-3.6716552568484446,-3.6516884035892354,-3.63172290076561,-3.6117588032980432,-3.5917961683324329,
-3.5718350553296014,-3.5518755261583417,-3.5319176451921419,-3.51196147940973,-3.4920070984995832,-3.4720545749685527,-3.4521039842547618,-3.4321554048449321,-3.4122089183963142,-3.3922646098633837,-3.3723225676294915,
-3.3523828836436409,-3.3324456535625937,-3.31251097689849,-3.2925789571721915,-3.2726497020725533,-3.2527233236218418,-3.2327999383475166,-3.2128796674606122,-3.1929626370409463,-3.173048978229402,-3.1531388274275343,
-3.1332323265047535,-3.11332962301335,-3.0934308704116327,-3.073536228295457,-3.0536458626384273,-3.0337599460410649,-3.0138786579892409,-2.9940021851221825,-2.9741307215103556,-2.9542644689435558,-2.93440363722952,
-2.9145484445034016,-2.8946991175484365,-2.87485589212815,-2.8550190133304514,-2.8351887359239618,-2.8153653247269466,-2.7955490549891953,-2.7757402127872308,-2.7559390954331966,-2.7361460118978029,-2.7163612832476915,
-2.6965852430975894,-2.6768182380776189,-2.6570606283161218,-2.6373127879383631,-2.617575105581464,-2.5978479849259082,-2.5781318452439663,-2.5584271219653556,-2.5387342672604531,-2.5190537506413642,-2.499386059581115,
-2.4797317001512478,-2.460091197678036,-2.4404650974175466,-2.4208539652497265,-2.4012583883916578,-2.3816789761301034,-2.362116360573411,-2.3425711974228016,-2.3230441667630286,-2.3035359738723287,-2.2840473500515284,
-2.2645790534721191,-2.2451318700430245,-2.2257066142957265,-2.2063041302873216,-2.1869252925210021,-2.1675710068833594,-2.1482422115978,-2.1289398781932674,-2.1096650124873406,-2.0904186555826612,-2.0712018848755123,
-2.0520158150752268,-2.0328615992329753,-2.01374042977831,-1.9946535395616944,-1.9756022029010771,-1.9565877366303941,-1.9376115011476927,-1.9186749014603939,-1.8997793882250023,-1.8809264587783732,-1.8621176581574446,
-1.8433545801041169,-1.8246388680517571,-1.8059722160895779,-1.7873563699009196,-1.7687931276712441,-1.7502843409614224,-1.7318319155416766,-1.7134378121813252,-1.6951040473892596,-1.6768326940998843,-1.6586258822990505,
-1.6404857995843321,-1.6224146916538242,-1.6044148627174892,-1.5864886758249377,-1.5686385531034282,-1.550866975899772,-1.5331764848197713,-1.5155696796587925,-1.4980492192170749,-1.4806178209934122,-1.46327826075093,
-1.4460333719487881,-1.428886045033815,-1.4118392265862763,-1.3948959183142493,-1.3780591758913747,-1.3613321076331226,-1.3447178730071199,-1.328219680973554,-1.3118407881521899,-1.2955844968131083,-1.2794541526889047,
-1.2634531426067635,-1.2475848919395529,-1.2318528618758602,-1.2162605465097029,-1.2008114697515131,-1.1855091820628834,-1.1703572570184839,-1.1553592876995065,-1.1405188829239583,-1.1258396633200967,-1.1113252572502743,
-1.0969792965934422,-1.0828054123955027,-1.068807230397659,-1.0549883664538011,-1.0413524218488428,-1.0279029785307472,-1.0146435942697418,-1.0015777977589264,-0.9887090836711161,-0.97604090768730545,-0.96357668151261988,
-0.95131976789599393,-0.93927347567010167,-0.92744105482825279,-0.915825691655048,-0.90443050392756807,-0.89325853620374351,-0.88231275521432218,-0.87159604537451385,-0.86111120443095313,-0.85086093925908757,-0.840847861825462,
-0.83107448532865258,-0.82154322053179674,-0.81225637229878378,-0.80321613634522,-0.79442459621427064,-0.785883720486411,-0.77759536023101683,-0.76956124670657577,-0.76178298931513166,-0.754262073815393,-0.74699986079774239,
-0.73999758442319552,-0.73325635142718226,-0.72677714038786267,-0.72056080125756639,-0.71460805515484338,-0.70891949441356938,-0.70349558288454273,-0.69833665648406218,-0.69344292398308693,-0.68881446802975277,-0.68445124639726029,
-0.68035309344846029,-0.6765197218078437,-0.67295072423109537,-0.66964557566190053,-0.66660363546528811,-0.6638241498264712,-0.661306254303883,-0.65904897652492178,-0.6570512390127925,-0.65531186213277892,-0.65382956714627882,
-0.65260297936100109,-0.65163063136583255,-0.65091096633905232,-0.65044234141878166,-0.65022303112481017,-0.650251230821235,-0.650525060209673,-0.65104256684316542,-0.65180172965127692,-0.6528004624672975,-0.65403661754887843,
-0.65550798908387331,-0.65721231667360747,-0.65914728878625561,-0.66131054617347185,-0.66369968524388412,-0.66631226138752708,-0.66914579224575454,-0.67219776092162442,-0.67546561912620406,-0.67894679025668014,-0.68263867240259113,
-0.68653864127691333,-0.69064405306914034,-0.69495224721788162,-0.69946054910088207,-0.70416627264072129,-0.70906672282479322,-0.71415919813849116,-0.71944099291082908,-0.72490939957202016,-0.73056171082280641,-0.73639522171558514,
-0.74240723164761679,-0.74859504626681928,-0.75495597929085523,-0.76148735424040837,-0.76818650608771266,-0.77505078282155793,-0.78207754693013343,-0.78926417680319794,-0.79660806805517914,-0.80410663477090438,-0.81175731067574963},
new double[]{
-5.8322524164948817,-5.8130220492358493,-5.7937916977381283,-5.7745613626197079,-5.7553310445228067,-5.7361007441148235,-5.7168704620893234,-5.6976401991670631,-5.6784099560970613,-5.6591797336577,-5.639949532657881,-5.62071935393822,
-5.601489198372291,-5.5822590668679162,-5.56302896036851,-5.5437988798544762,-5.5245688263446535,-5.5053388008978272,-5.4861088046142914,-5.4668788386374763,-5.4476489041556428,-5.4284190024036354,-5.40918913466471,
-5.3899593022724321,-5.3707295066126495,-5.3514997491255372,-5.3322700313077318,-5.3130403547145448,-5.2938107209622558,-5.2745811317305114,-5.2553515887648,-5.2361220938790378,-5.2168926489582512,-5.1976632559613591,
-5.1784339169240736,-5.159204633961906,-5.1399754092732959,-5.1207462451428611,-5.1015171439447746,-5.0822881081462725,-5.0630591403113048,-5.0438302431043205,-5.0246014192942061,-5.0053726717583817,-4.98614400348705,
-4.9669154175876171,-4.9476869172892828,-4.9284585059478117,-4.909230187050496,-4.8900019642213035,-4.8707738412262342,-4.8515458219788838,-4.8323179105462239,-4.8130901111546116,-4.7938624281960323,-4.7746348662345843,
-4.7554074300132232,-4.7361801244607644,-4.7169529546991624,-4.6977259260510724,-4.6784990440477117,-4.6592723144370245,-4.640045743192168,-4.62081933652033,-4.6015931008718889,-4.5823670429499384,-4.5631411697201729,
-4.5439154884211721,-4.524690006575077,-4.5054647319986918,-4.4862396728150085,-4.4670148374651939,-4.447790234721035,-4.4285658736978766,-4.4093417638680572,-4.3901179150748764,-4.3708943375471039,-4.3516710419140532,
-4.3324480392212434,-4.313225340946679,-4.2940029590177486,-4.274780905828802,-4.2555591942593951,-4.2363378376932559,-4.2171168500379839,-4.1978962457455209,-4.1786760398334142,-4.1594562479069115,-4.1402368861819108,
-4.1210179715088078,-4.1017995213972647,-4.0825815540419477,-4.0633640883492612,-4.0441471439651213,-4.0249307413038045,-4.00571490157792,-3.9864996468295439,-3.967284999962561,-3.9480709847762556,-3.9288576260002093,
-3.9096449493305494,-3.8904329814675962,-3.8712217501549704,-3.8520112842202123,-3.8328016136169683,-3.8135927694688085,-3.7943847841147362,-3.7751776911564492,-3.7559715255074271,-3.736766323443907,-3.7175621226578222,
-3.6983589623117754,-3.6791568830961294,-3.6599559272882849,-3.6407561388142358,-3.6215575633124852,-3.6023602482004087,-3.5831642427431549,-3.5639695981251873,-3.54477636752455,-3.5255846061899736,-3.506394371520916,
-3.487205723150653,-3.4680187230325266,-3.4488334355294681,-3.4296499275069205,-3.410468268429276,-3.3912885304599634,-3.3721107885653172,-3.3529351206223628,-3.3337616075306578,-3.3145903333283435,-3.2954213853125465,
-3.2762548541642937,-3.2570908340780971,-3.2379294228963751,-3.2187707222488822,-3.1996148376973164,-3.1804618788852976,-3.1613119596938866,-3.1421651984028522,-3.1230217178578745,-3.103881645643888,-3.084745114264777,
-3.0656122613296324,-3.0464832297457951,-3.0273581679189059,-3.0082372299601987,-2.9891205759012696,-2.970008371916566,-2.9509007905538485,-2.9317980109728743,-2.9127002191925593,-2.8936076083468976,-2.8745203789498897,
-2.8554387391697729,-2.8363629051128223,-2.81729310111701,-2.7982295600558218,-2.7791725236525053,-2.7601222428050716,-2.74107897792233,-2.7220429992712729,-2.7030145873361104,-2.6839940331892689,-2.6649816388746546,
-2.6459777178035013,-2.6269825951631041,-2.6079966083387522,-2.5890201073491674,-2.5700534552957457,-2.5510970288259105,-2.5321512186108621,-2.513216429838018,-2.4942930827184142,-2.4753816130093456,-2.4564824725524956,
-2.4375961298277993,-2.4187230705232756,-2.39986379812103,-2.3810188344996241,-2.3621887205529823,-2.3433740168259751,-2.3245753041668,-2.3057931843962423,-2.2870282809938685,-2.2682812398011727,-2.2495527297416369,
-2.2308434435576516,-2.2121540985641626,-2.1934854374188819,-2.1748382289088291,-2.1562132687529165,-2.1376113804202173,-2.119033415963488,-2.1004802568674448,-2.0819528149111988,-2.0634520330441815,-2.044978886274786,
-2.0265343825708584,-2.0081195637710616,-1.9897355065060298,-1.9713833231281051,-1.9530641626483269,-1.9347792116792186,-1.9165296953817674,-1.8983168784148627,-1.8801420658852983,-1.8620066042962908,-1.8439118824923113,
-1.8258593325978469,-1.8078504309475505,-1.7898866990050493,-1.7719697042675067,-1.7541010611528463,-1.7362824318663563,-1.7185155272432058,-1.7008021075632116,-1.6831439833340032,-1.6655430160385423,-1.6480011188427659,
-1.6305202572589401,-1.6131024497601256,-1.5957497683409914,-1.5784643390200426,-1.5612483422781815,-1.5441040134283706,-1.5270336429110487,-1.5100395765098382,-1.4931242154819844,-1.4762900165979072,-1.4595394920841953,
-1.4428752094643473,-1.4262997912915822,-1.4098159147680711,-1.3934263112450223,-1.3771337655981519,-1.3609411154732232,-1.3448512503965178,-1.3288671107453367,-1.3129916865738851,-1.2972280162902239,-1.2815791851803213,
-1.2660483237756479,-1.25063860606121,-1.2353532475214211,-1.22019550302175,-1.2051686645246833,-1.1902760586391741,-1.1755210440034221,-1.1609070085015587,-1.1464373663155514,-1.1321155548144426,-1.1179450312838426,
-1.1039292694994447,-1.0900717561491851,-1.0763759871095433,-1.0628454635823585,-1.0494836880994161,-1.0362941604029343,-1.0232803732109348,-1.0104458078773277,-0.99779392995734417,-0.985328184689729,-0.97305199240783546,
-0.96096874389244291,-0.94908179567974216,-0.93739446533848991,-0.9259100267308229,-0.91463170527162774,-0.90356267320169747,-0.89270604489014271,-0.88206487218167973,-0.87164213980447758,-0.86144076085421062,-0.8514635723698295,
-0.84171333101634,-0.83219270888955177,-0.822904289457346,-0.81385056365150132,-0.80503392612352387,-0.796456671677248,-0.78812099189022,-0.78002897193504939,-0.77218258761101877,-0.76458370259529507,-0.75723406592208187,
-0.75013530969701214,-0.743288947053002,-0.7366963703526872,-0.73035884964144293,-0.724277531353864,-0.71845343727545552,-0.71288746376017,-0.70758038120332334,-0.70253283376835274,-0.697745339364832,-0.69321828987415779,
-0.68895195161835976,-0.68494646606657694,-0.68120185077288853,-0.67771800053839237,-0.674494688789687,-0.671531569165251,-0.66882817730060717,-0.66638393280263108,-0.66419814140290057,-0.66226999727958846,-0.66059858553708151,
-0.65918288483225185,-0.65802177013611984,-0.65711401561952465,-0.65645829765136032,-0.656053197897933,-0.65589720651205152,-0.65598872540057551,-0.65632607155930223,-0.65690748046428149,-0.65773110950889435,-0.6587950414763194,
-0.66009728803733048,-0.66163579326372191,-0.66340843714803432,-0.66541303912065652,-0.66764736155579774,-0.67010911325825984,-0.67279595292338756,-0.6757054925630287,-0.67883530089079969,-0.68218290666041537,-0.68574580195130452,
-0.68952144539619675,-0.6935072653458193,-0.69770066296629485,-0.70209901526526952,-0.70669967804323119,-0.71149998876689546,-0.716497269361941,-0.72168882892276509,-0.72707196633730564,-0.732643972825333,-0.73840213438895619,
-0.74434373417441335,-0.75046605474452133,-0.75676638026144938,-0.76324199857975228,-0.769890203249852,-0.77670829543239317,-0.78369358572411485,-0.79084339589608632,-0.79815506054533358,-0.805625928661059,-0.81325336510680413},
new double[]{
-5.6670936376194305,-5.6485756343767015,-5.6300576505756279,-5.6115396869497367,-5.59302174426023,-5.5745038232970279,-5.5559859248798515,-5.5374680498593492,-5.5189501991182617,-5.5004323735726324,-5.4819145741730644,-5.4633968019060246,
-5.4448790577951947,-5.4263613429028768,-5.40784365833145,-5.3893260052248824,-5.3708083847702976,-5.3522907981996042,-5.3337732467911865,-5.3152557318716553,-5.2967382548176669,-5.2782208170578118,-5.2597034200745743,
-5.2411860654063629,-5.2226687546496215,-5.2041514894610179,-5.1856342715597128,-5.1671171027297209,-5.1485999848223534,-5.1300829197587579,-5.1115659095325521,-5.093048956212554,-5.0745320619456225,-5.0560152289595948,
-5.037498459566347,-5.0189817561649557,-5.0004651212449911,-4.9819485573899289,-4.9634320672806878,-4.944915653699308,-4.9263993195327611,-4.9078830677769067,-4.8893669015405985,-4.8708508240499446,-4.8523348386527267,
-4.8338189488229881,-4.8153031581657926,-4.7967874704221645,-4.7782718894742136,-4.7597564193504516,-4.7412410642313132,-4.7227258284548785,-4.7042107165228195,-4.6856957331065594,-4.6671808830536765,-4.6486661713945354,
-4.6301516033491819,-4.6116371843344854,-4.593122919971556,-4.5746088160934413,-4.55609487875311,-4.5375811142317373,-4.519067529047299,-4.5005541299634952,-4.4820409239990058,-4.4635279184370882,-4.4450151208355484,
-4.4265025390370765,-4.4079901811799722,-4.38947805570928,-4.3709661713883365,-4.3524545373107477,-4.3339431629128295,-4.3154320579865022,-4.2969212326926733,-4.2784106975751293,-4.2599004635749349,-4.241390542045389,
-4.2228809447675255,-4.2043716839662109,-4.1858627723268311,-4.1673542230126168,-4.1488460496826063,-4.130338266510293,-4.1118308882029613,-4.0933239300217528,-4.074817407802481,-4.0563113379772284,-4.0378057375967469,
-4.0193006243536971,-4.0007960166067642,-3.9822919334056603,-3.9637883945170787,-3.9452854204516026,-3.9267830324916266,-3.908281252720319,-3.8897801040516633,-3.871279610261622,-3.8527797960204588,-3.8342806869262684,
-3.8157823095397547,-3.7972846914202996,-3.7787878611633796,-3.7602918484393686,-3.7417966840337855,-3.7233023998890364,-3.7048090291477034,-3.6863166061974439,-3.6678251667175505,-3.6493347477272371,-3.6308453876357127,
-3.612357126294107,-3.5938700050493164,-3.5753840667998378,-3.5568993560536621,-3.5384159189883064,-3.5199338035130521,-3.5014530593334792,-3.4829737380183703,-3.4644958930690737,-3.4460195799914106,-3.427544856370222,
-3.4090717819466407,-3.3906004186981922,-3.3721308309218219,-3.3536630853199516,-3.3351972510896695,-3.3167334000151696,-3.2982716065635485,-3.2798119479840753,-3.2613545044110648,-3.2428993589704649,-3.2244465978902985,
-3.2059963106150833,-3.187548589924373,-3.1691035320555532,-3.1506612368310432,-3.1322218077900454,-3.1137853523250034,-3.0953519818229172,-3.0769218118116859,-3.0584949621116384,-3.0400715569924248,-3.0216517253354462,
-3.0032356008019976,-2.9848233220073146,-2.96641503270071,-2.9480108819519977,-2.929611024344394,-2.9112156201741151,-2.8928248356568633,-2.8744388431414256,-2.8560578213305972,-2.8376819555096526,-2.8193114377825927,
-2.800946467316396,-2.7825872505935076,-2.764234001672814,-2.7458869424593293,-2.7275463029828582,-2.7092123216858712,-2.690885245720851,-2.6725653312573634,-2.6542528437991137,-2.6359480585112425,-2.6176512605581279,
-2.5993627454519528,-2.581082819412301,-2.5628117997370468,-2.5445500151847935,-2.5262978063691328,-2.5080555261649673,-2.4898235401271642,-2.4716022269217865,-2.4533919787701426,-2.4351932019058991,-2.4170063170454847,
-2.3988317598720132,-2.3806699815329262,-2.3625214491515716,-2.3443866463528917,-2.3262660738034007,-2.3081602497656037,-2.2900697106669923,-2.2719950116837335,-2.25393672733914,-2.2358954521169929,-2.2178718010897454,
-2.1998664105616186,-2.1818799387265608,-2.163913066341002,-2.1459664974112971,-2.1280409598957131,-2.1101372064207546,-2.0922560150115865,-2.0743981898362494,-2.0565645619633,-2.0387559901324592,-2.020973361537767,
-2.0032175926226863,-1.9854896298865046,-1.9677904507013202,-1.9501210641387929,-1.9324825118057605,-1.9148758686877245,-1.8973022439990996,-1.879762782039019,-1.8622586630513756,-1.8447911040876572,-1.8273613598710152,
-1.8099707236598717,-1.7926205281092433,-1.7753121461278159,-1.7580469917286623,-1.7408265208713529,-1.7236522322930472,-1.7065256683260075,-1.689448415698815,-1.6724221063184053,-1.6554484180298779,-1.6385290753508726,
-1.6216658501771348,-1.6048605624557331,-1.588115080822222,-1.5714313231978903,-1.5548112573430692,-1.5382569013623264,-1.5217703241572265,-1.5053536458221948,-1.4890090379788996,-1.4727387240444387,-1.4565449794285212,
-1.4404301316547372,-1.424396560400935,-1.408446697453668,-1.39258302657164,-1.3768080832530589,-1.3611244544018257,-1.3455347778875142,-1.3300417419941724,-1.3146480847530639,-1.2993565931546018,-1.2841701022348848,
-1.2690914940324516,-1.2541236964110929,-1.2392696817448494,-1.2245324654616236,-1.2099151044421981,-1.1954206952718407,-1.1810523723421187,-1.1668133058010188,-1.1527066993499848,-1.1387357878870472,-1.12490383499581,
-1.1112141302806933,-1.0976699865494934,-1.0842747368450221,-1.0710317313283064,-1.0579443340165817,-1.0450159193800797,-1.0322498688023929,-1.0196495669099974,-1.0072183977773044,-0.9949597410144212,-0.98287696774558442,
-0.97097343648700618,-0.959252488933635,-0.94771744566506211,-0.93637160178150014,-0.92521822248142116,-0.91426053859305,-0.90350174207246914,-0.89294498148158974,-0.88259335745967937,-0.87244991820250151,-0.86251765496341259,
-0.85279949759097584,-0.843298310117779,-0.83401688641518923,-0.82495794592873684,-0.8161241295086884,-0.80751799535015167,-0.7991420150567472,-0.79099856984149086,-0.783089946878049,-0.7754183358149721,-0.76798582546487526,
-0.7607944006798194,-0.75384593942337519,-0.74714221004900394,-0.74068486879350037,-0.73447545749329168,-0.72851540153040528,-0.722806008013896,-0.71734846420147835,-0.71214383616504751,-0.70719306770269874,-0.70249697949878165,
-0.69805626853245684,-0.69387150773416972,-0.68994314588842387,-0.68627150778022838,-0.68285679458162973,-0.67969908447380412,-0.67679833349930807,-0.67415437663825017,-0.67176692910137348,-0.6696355878323188,-0.66775983321068588,
-0.66613903094691884,-0.66477243415951515,-0.66365918562460369,-0.662798320187546,-0.66218876732589,-0.66182935385275543,-0.66171880674953409,-0.66185575611666536,-0.66223873823118284,-0.66286619869972063,-0.66373649569572057,
-0.6648479032696869,-0.66619861472148589,-0.66778674602389443,-0.66961033928683811,-0.6716673662520487,-0.67395573180818258,-0.67647327751679454,-0.6792177851399348,-0.68218698016053714,-0.6853785352871854,-0.688790073935282,
-0.69241917367708639,-0.69626336965355351,-0.70032015794136138,-0.70458699886898646,-0.70906132027615054,-0.7137405207114268,-0.71862197256325633,-0.723703025120074,-0.72898100755569128,-0.73445323183651567,-0.740116995547607,
-0.74596958463498209,-0.7520082760619673,-0.75823034037778381,-0.7646330441969047,-0.7712136525880704,-0.77796943137217833,-0.78489764932856554,-0.79199558030949913,-0.79926050526295944,-0.80668971416405588,-0.81428050785565176},
new double[]{
-5.5139816840501892,-5.4961251912068407,-5.4782687219971544,-5.4604122772803931,-5.4425558579470614,-5.4246994649200388,-5.406843099155755,-5.3889867616454117,-5.3711304534162458,-5.353274175532837,-5.33541792909847,-5.317561715256538,
-5.2997055351920013,-5.2818493901328978,-5.26399328135191,-5.2461372101679853,-5.2282811779480216,-5.2104251861086075,-5.1925692361178264,-5.1747133294971324,-5.1568574678232881,-5.1390016527303741,-5.1211458859118739,
-5.1032901691228307,-5.0854345041820856,-5.0675788929745966,-5.0497233374538428,-5.03186783964431,-5.0140124016440764,-4.9961570256274834,-4.978301713847908,-4.9604464686406367,-4.9425912924258375,-4.9247361877116482,
-4.9068811570973718,-4.8890262032767886,-4.8711713290415881,-4.853316537284929,-4.8354618310051238,-4.8176072133094578,-4.7997526874181524,-4.7818982566684642,-4.7640439245189379,-4.7461896945538129,-4.7283355704875891,
-4.7104815561697571,-4.6926276555897033,-4.6747738728817909,-4.6569202123306246,-4.6390666783765084,-4.6212132756211,-4.6033600088332705,-4.5855068829551744,-4.567653903108547,-4.54980107460122,-4.5319484029338835,
-4.5140958938070828,-4.4962435531284806,-4.47839138702037,-4.46053940182747,-4.4426876041249974,-4.424836000727038,-4.4069845986952068,-4.3891334053476427,-4.37128242826831,-4.3534316753166475,-4.3355811546375662,
-4.3177308746718017,-4.2998808441666529,-4.2820310721870989,-4.2641815681273245,-4.2463323417226606,-4.2284834030619551,-4.2106347626003968,-4.1927864311727934,-4.1749384200073427,-4.1570907407398892,-4.1392434054287017,
-4.1213964265697864,-4.10354981711275,-4.0857035904772374,-4.0678577605699626,-4.0500123418023533,-4.0321673491088355,-4.0143227979657725,-3.9964787044110945,-3.9786350850646293,-3.9607919571491674,-3.9429493385122902,
-3.925107247648981,-3.9072657037250487,-3.8894247266014008,-3.8715843368591845,-3.8537445558258336,-3.8359054056020554,-3.8180669090897812,-3.8002290900211286,-3.7823919729883984,-3.76455558347515,-3.7467199478883955,
-3.7288850935919404,-3.711051048940929,-3.6932178433176208,-3.6753855071684507,-3.6575540720424153,-3.6397235706308337,-3.6218940368085244,-3.6040655056764619,-3.5862380136059482,-3.5684115982843654,-3.5505862987625569,
-3.5327621555038973,-3.5149392104351111,-3.4971175069988973,-3.4792970902084228,-3.4614780067037572,-3.4436603048103027,-3.4258440345993026,-3.4080292479504868,-3.3902159986169371,-3.3724043422922465,-3.3545943366800484,
-3.3367860415659969,-3.318979518892287,-3.3011748328347945,-3.2833720498829284,-3.2655712389222864,-3.2477724713202076,-3.2299758210143237,-3.2121813646042,-3.1943891814461827,-3.176599353751548,-3.1588119666880674,
-3.1410271084851034,-3.12324487054235,-3.1054653475423382,-3.0876886375668282,-3.06991484221722,-3.0521440667391055,-3.034376420151101,-3.0166120153780969,-2.9988509693890624,-2.9810934033395564,-2.9633394427190853,
-2.9455892175034677,-2.9278428623123562,-2.9101005165720815,-2.8923623246839791,-2.87462843619837,-2.8568990059943613,-2.839174194465651,-2.821454167712508,-2.8037390977401171,-2.7860291626634712,-2.7683245469190041,
-2.7506254414831584,-2.7329320440980842,-2.7152445595046721,-2.6975631996831244,-2.6798881841012703,-2.6622197399708369,-2.6445581025118923,-2.6269035152256697,-2.6092562301759976,-2.5916165082795497,-2.5739846196051417,
-2.5563608436822953,-2.5387454698192893,-2.5211387974309321,-2.5035411363762656,-2.4859528073064392,-2.4683741420229635,-2.4508054838465703,-2.4332471879969,-2.415699621983221,-2.3981631660064067,-2.38063821337236,
-2.3631251709170997,-2.3456244594436937,-2.3281365141712258,-2.3106617851959768,-2.2932007379649777,-2.2757538537621,-2.2583216302068134,-2.2409045817657445,-2.2235032402771435,-2.2061181554883516,-2.1887498956063389,
-2.1713990478613683,-2.1540662190838025,-2.1367520362940611,-2.1194571473056945,-2.1021822213415149,-2.0849279496626907,-2.0676950462106736,-2.0504842482617871,-2.0332963170942651,-2.0161320386674797,-1.998992224313054,
-1.9818777114375008,-1.9647893642359662,-1.9477280744166083,-1.9306947619350721,-1.9136903757384491,-1.8967158945180527,-1.879772327470246,-1.8628607150644976,-1.8459821298177441,-1.8291376770740513,-1.81232849578848,
-1.795555759313954,-1.778820676189838,-1.7621244909308134,-1.7454684848145408,-1.7288539766664777,-1.7122823236400995,-1.6957549219906489,-1.6792732078404193,-1.6628386579334322,-1.6464527903772506,-1.6301171653695232,
-1.6138333859067189,-1.5976030984723697,-1.5814279937019971,-1.5653098070217546,-1.5492503192576721,-1.5332513572122519,-1.5173147942050156,-1.5014425505734705,-1.4856365941308232,-1.469898940576637,-1.45423165385651,
-1.4386368464667247,-1.42311667969971,-1.4076733638260661,-1.3923091582087983,-1.3770263713453383,-1.3618273608328659,-1.3467145332523978,-1.3316903439670789,-1.3167572968301047,-1.3019179437977106,-1.2871748844427013,
-1.2725307653640492,-1.2579882794881772,-1.2435501652576531,-1.2292192057031635,-1.214998227394811,-1.2008900992689797,-1.1868977313272577,-1.1730240732041735,-1.1592721126008141,-1.1456448735817393,-1.1321454147329808,
-1.1187768271793417,-1.1055422324596542,-1.0924447802591504,-1.079487645998622,-1.0666740282805975,-1.0540071461933591,-1.0414902364742364,-1.0291265505342557,-1.0169193513468988,-1.0048719102044077,-0.99298750334578034,
-0.98126940846132338,-0.96972090107935038,-0.95834525084134858,-0.94714571767266453,-0.9361255478564815,-0.925287970019568,-0.91463619103897253,-0.90417339187949863,-0.89390272337243692,-0.88382730194662407,-0.87395020532345791,
-0.86427446818800646,-0.854803077848802,-0.84553896989930843,-0.83648502389438029,-0.82764405905529936,-0.81901883001716069,-0.810612022632503,-0.80242624984511,-0.7944640476478726,-0.78672787113847209,-0.77922009068644571,
-0.77194298822490037,-0.76489875367977633,-0.75808948154911215,-0.75151716764423571,-0.74518370600420858,-0.73909088599417971,-0.7332403895975721,-0.72763378891123,-0.72227254385180661,-0.71715800008077513,-0.71229138715450935,
-0.70767381690490649,-0.70330628205502876,-0.69918965507321984,-0.69532468726812524,-0.69171200812601,-0.68835212489073871,-0.68524542238576225,-0.68239216307645523,-0.67979248737017084,-0.67744641415043627,-0.6753538415408,
-0.67351454789298237,-0.6719281929931582,-0.67059431947943848,-0.66951235446290658,-0.668681611343917,-0.66810129181477274,-0.667770488039377,-0.66768818499999294,-0.66785326300085335,-0.668264500318035,-0.66892057598475019,
-0.66982007270101307,-0.67096147985650256,-0.6723431966553769,-0.67396353533177722,-0.67582072444480612,-0.67791291224186478,-0.68023817007937992,-0.68279449589015073,-0.68557981768678211,-0.68859199709095564,-0.69182883287859942,
-0.69528806453137193,-0.69896737578524681,-0.70286439816739033,-0.70697671451294219,-0.71130186245375138,-0.71583733787156956,-0.7205805983086695,-0.72552906632932124,-0.73068013282603717,-0.736031160264967,-0.74157948586529643,
-0.74732242470797472,-0.75325727276955068,-0.75938130987735664,-0.76569180258271541,-0.7721860069492783,-0.77886117125401744,-0.785714538598796,-0.792743349430825,-0.79994484397068333,-0.80731626454692818,-0.81485485783665645},
new double[]{
-5.3717094251340507,-5.3544688539738168,-5.337228311165962,-5.3199877977051431,-5.3027473146209063,-5.2855068629789157,-5.2682664438822178,-5.2510260584725517,-5.2337857079317081,-5.2165453934829333,-5.1993051163923827,-5.1820648779706282,
-5.1648246795742123,-5.1475845226072607,-5.1303444085231567,-5.1131043388262594,-5.095864315073702,-5.0786243388772316,-5.0613844119051343,-5.0441445358842127,-5.0269047126018362,-5.0096649439080716,-4.992425231717875,
-4.975185578013372,-4.9579459848462086,-4.9407064543399946,-4.9234669886928222,-4.9062275901798786,-4.8889882611561521,-4.8717490040592253,-4.8545098214121758,-4.8372707158265706,-4.8200316900055693,-4.8027927467471372,
-4.7855538889473657,-4.7683151196039137,-4.7510764418195706,-4.7338378588059378,-4.7165993738872469,-4.699360990504303,-4.6821227122185771,-4.66488454271643,-4.6476464858134916,-4.6304085454591952,-4.6131707257414618,
-4.5959330308915547,-4.5786954652891056,-4.5614580334673125,-4.5442207401183206,-4.5269835900987925,-4.5097465884356733,-4.4925097403321548,-4.4752730511738541,-4.458036526535202,-4.4408001721860586,-4.4235639940985587,
-4.4063279984541994,-4.3890921916511711,-4.3718565803119507,-4.35462117129115,-4.3373859716836547,-4.3201509888330305,-4.3029162303402364,-4.2856817040726343,-4.2684474181733165,-4.25121338107076,-4.2339796014888158,
-4.2167460884570511,-4.199512851321443,-4.1822798997554607,-4.1650472437715207,-4.147814893732849,-4.1305828603657542,-4.1133511547723325,-4.0961197884436125,-4.07888877327316,-4.0616581215711545,-4.0444278460789613,
-4.0271979599842078,-4.0099684769363853,-3.9927394110629972,-3.9755107769862663,-3.9582825898404241,-3.9410548652896051,-3.9238276195463606,-3.9066008693908181,-3.8893746321905058,-3.8721489259208677,-3.854923769186493,
-3.8376991812430785,-3.8204751820201586,-3.8032517921446236,-3.7860290329650521,-3.7688069265768882,-3.7515854958484964,-3.7343647644481122,-3.7171447568717335,-3.6999254984719734,-3.6827070154879151,-3.6654893350759994,
-3.6482724853419781,-3.6310564953739752,-3.6138413952766895,-3.5966272162067794,-3.5794139904094648,-3.5622017512563988,-3.5449905332848388,-3.52778037223817,-3.5105713051078249,-3.493363370176644,-3.4761566070637264,
-3.4589510567708253,-3.4417467617303319,-3.4245437658549109,-3.4073421145888303,-3.3901418549610591,-3.3729430356401724,-3.3557457069911409,-3.3385499211340584,-3.3213557320048723,-3.3041631954181869,-3.2869723691322039,
-3.2697833129158758,-3.2525960886183372,-3.2354107602406965,-3.2182273940102615,-3.2010460584572784,-3.1838668244942663,-3.1666897654980315,-3.1495149573944503,-3.1323424787461041,-3.1151724108428658,-3.0980048377955263,
-3.0808398466325624,-3.0636775274001407,-3.0465179732654679,-3.0293612806235855,-3.0122075492077216,-2.99505688220331,-2.9779093863657931,-2.96076517214232,-2.9436243537974685,-2.9264870495431072,-2.9093533816725312,
-2.8922234766989918,-2.8750974654987647,-2.8579754834588842,-2.8408576706296871,-2.8237441718823044,-2.8066351370712557,-2.7895307212022824,-2.7724310846055853,-2.7553363931146104,-2.7382468182505519,-2.7211625374127237,
-2.7040837340749726,-2.6870105979882966,-2.6699433253898381,-2.6528821192184306,-2.635827189336867,-2.6187787527610737,-2.6017370338963715,-2.584702264781003,-2.567674685337114,-2.5506545436293773,-2.5336420961314436,
-2.5166376080004138,-2.4996413533595216,-2.4826536155892196,-2.4656746876268585,-2.448704872275155,-2.431744482519639,-2.4147938418552721,-2.3978532846224234,-2.3809231563523983,-2.3640038141226944,-2.3470956269221794,
-2.3301989760263608,-2.3133142553829233,-2.2964418720077058,-2.2795822463912758,-2.262735812916262,-2.2459030202855859,-2.2290843319617388,-2.2122802266172239,-2.1954911985962888,-2.1787177583880419,-2.1619604331110538,
-2.1452197670095079,-2.1284963219609665,-2.1117906779957796,-2.0951034338281684,-2.0784352073989663,-2.0617866364299946,-2.0451583789900218,-2.0285511140722128,-2.011965542182967,-1.9954023859419858,-1.9788623906933984,
-1.9623463251277176,-1.945854981914366,-1.9293891783444721,-1.9129497569835792,-1.8965375863338687,-1.8801535615054461,-1.8637986048961759,-1.8474736668795007,-1.8311797264996108,-1.8149177921732664,-1.7986889023975063,
-1.7824941264624012,-1.7663345651679328,-1.7502113515439992,-1.7341256515724672,-1.7180786649100905,-1.7020716256110375,-1.6861058028476641,-1.6701825016280698,-1.6543030635088785,-1.6384688673015684,-1.6226813297705756,
-1.6069419063212738,-1.591252091675825,-1.5756134205347736,-1.5600274682221302,-1.5444958513115827,-1.5290202282313332,-1.5136022998449434,-1.4982438100054423,-1.4829465460798259,-1.4677123394409513,-1.4525430659237084,
-1.4374406462422287,-1.4224070463647724,-1.4074442778428244,-1.3925543980908193,-1.3777395106128108,-1.3630017651723121,-1.3483433579014379,-1.3337665313454112,-1.3192735744384239,-1.30486682240679,-1.2905486565952946,
-1.2763215042126028,-1.2621878379916047,-1.2481501757605646,-1.2342110799209829,-1.2203731568281264,-1.2066390560702565,-1.1930114696426843,-1.1794931310129018,-1.1660868140731913,-1.1527953319772935,-1.1396215358579178,
-1.1265683134221238,-1.1136385874218628,-1.1008353139972753,-1.0881614808906708,-1.0756201055294787,-1.0632142329768608,-1.0509469337490998,-1.0388213014993462,-1.0268404505677895,-1.0150075133988457,-1.0033256378265,
-0.99179798422951915,-0.98042772255884247,-0.9692180292400876,-0.958172083954735,-0.9472930663042145,-0.93658415236177817,-0.926048511117713,-0.91568930082412126,-0.905509665246168,-0.895512729827358,-0.88570159777705781,
-0.87607934608911631,-0.86664902150104672,-0.85741363640382107,-0.84837616471288024,-0.83953953771147627,-0.83090663987793068,-0.82248030470881883,-0.81426331055045154,-0.80625837645133636,-0.79846815804854721,-0.79089524350110751,
-0.78354214948360534,-0.7764113172532926,-0.76950510880388578,-0.76282580311916959,-0.75637559253931752,-0.75015657925257162,-0.744170771924583,-0.73842008247729418,-0.73290632302874781,-0.72763120300464934,-0.72259632643187355,
-0.7178031894234137,-0.71325317786351849,-0.70894756530095548,-0.7048875110574836,-0.7010740585577232,-0.69750813388567878,-0.69419054457221174,-0.69112197861677716,-0.68830300374574571,-0.68573406690862782,-0.68341549401251733,
-0.68134748989407667,-0.679530138527408,-0.6779634034651929,-0.67664712850955955,-0.67558103860823171,-0.674764740970665,-0.67419772639805686,-0.67387937082035931,-0.6738089370327085,-0.6739855766230386,-0.674408332082047,
-0.67507613908615449,-0.67598782894363152,-0.6771421311936614,-0.6785376763477754,-0.68017299876281956,-0.68204653963441209,-0.68415665009970072,-0.68650159443815228,-0.68907955335908522,-0.69188862736469148,-0.69492684017738515,
-0.69819214222046211,-0.701682414141242,-0.70539547036610584,-0.70932906267711715,-0.71348088380023345,-0.71784857099546373,-0.7224297096397071,-0.72722183679341357,-0.73222244474263809,-0.73742898450849981,-0.74283886931652676,
-0.74844947801883155,-0.75425815846254729,-0.7602622307984358,-0.7664589907240662,-0.77284571265644564,-0.77941965282946413,-0.78617805231199023,-0.79311813994291747,-0.80023713517991557,-0.80753225085908553,-0.81500069586313884},
new double[]{
-5.2392248164260646,-5.2225591413159824,-5.2058934998135848,-5.1892278930579216,-5.1725623222266472,-5.1558967885373246,-5.1392312932487805,-5.1225658376625018,-5.1059004231240817,-5.0892350510247155,-5.0725697228027435,-5.0559044399452491,
-5.0392392039897107,-5.0225740165257085,-5.0059088791966921,-4.9892437937018022,-4.9725787617977586,-4.9559137853008126,-4.9392488660887617,-4.9225840061030359,-4.905919207350852,-4.8892544719074458,-4.8725898019183722,
-4.855925199601888,-4.8392606672514189,-4.8225962072380995,-4.8059318220134095,-4.7892675141118968,-4.772603286153986,-4.7559391408488922,-4.7392750809976247,-4.7226111094960981,-4.7059472293383475,-4.6892834436198489,
-4.6726197555409579,-4.65595616841046,-4.6392926856492389,-4.6226293107940819,-4.6059660475015969,-4.5893028995522709,-4.5726398708546672,-4.555976965449763,-4.5393141875154273,-4.5226515413710615,-4.5059890314823878,
-4.4893266624664045,-4.4726644390965076,-4.456002366307783,-4.4393404492024811,-4.4226786930556781,-4.4060171033211208,-4.3893556856372768,-4.3726944458335861,-4.3560333899369237,-4.339372524178283,-4.3227118549996781,
-4.3060513890612908,-4.2893911332488468,-4.27273109468125,-4.25607128071847,-4.2394116989696986,-4.2227523573017773,-4.2060932638479152,-4.1894344270166961,-4.1727758555013938,-4.1561175582895977,-4.1394595446731666,
-4.1228018242585156,-4.10614440697725,-4.08948730309716,-4.0728305232335877,-4.0561740783611686,-4.0395179798259795,-4.022862239358088,-4.0062068690845285,-3.9895518815427153,-3.972897289694306,-3.9562431069395307,
-3.9395893471320069,-3.9229360245940494,-3.9062831541324985,-3.8896307510550767,-3.8729788311872997,-3.8563274108899517,-3.8396765070771517,-3.8230261372350247,-3.8063763194410005,-3.7897270723837582,-3.7730784153838468,
-3.7564303684149887,-3.7397829521261046,-3.7231361878640747,-3.7064900976972619,-3.6898447044398242,-3.6732000316768412,-3.6565561037902818,-3.6399129459858406,-3.6232705843206721,-3.6066290457320531,-3.5899883580670005,
-3.5733485501128808,-3.5567096516290411,-3.5400716933794949,-3.5234347071666985,-3.5067987258664508,-3.4901637834639616,-3.4735299150911141,-3.4568971570649709,-3.4402655469275589,-3.4236351234869793,-3.4070059268598762,
-3.3903779985153224,-3.3737513813201523,-3.3571261195858058,-3.3405022591167128,-3.3238798472602888,-3.307258932958574,-3.2906395668015831,-3.2740218010824136,-3.2574056898541706,-3.2407912889887664,-3.2241786562376542,
-3.2075678512945585,-3.1909589358602637,-3.1743519737095292,-3.1577470307601923,-3.1411441751445373,-3.1245434772829923,-3.1079450099602322,-3.0913488484037637,-3.0747550703650637,-3.058163756203359,-3.0415749889721222,
-3.0249888545083703,-3.0084054415248533,-2.9918248417052187,-2.9752471498022457,-2.9586724637392381,-2.94210088471468,-2.9255325173102409,-2.9089674696022416,-2.892405853276677,-2.8758477837479077,-2.8592933802811222,
-2.842742766118687,-2.8261960686104945,-2.8096534193484266,-2.793114954305052,-2.7765808139766834,-2.7600511435309105,-2.7435260929587448,-2.7270058172315013,-2.7104904764625495,-2.6939802360740717,-2.6774752669689623,
-2.660975745708015,-2.64448185469253,-2.6279937823524997,-2.6115117233405081,-2.5950358787315042,-2.5785664562285935,-2.562103670375008,-2.5456477427724087,-2.5291989023056756,-2.5127573853743526,-2.4963234361309,
-2.4798973067259276,-2.4634792575605604,-2.4470695575461128,-2.4306684843712292,-2.4142763247766617,-2.39789337483785,-2.3815199402554645,-2.3651563366540875,-2.3488028898891846,-2.3324599363625413,-2.3161278233463163,
-2.2998069093158731,-2.2834975642915492,-2.2672001701895068,-2.2509151211818215,-2.2346428240659466,-2.2183836986436916,-2.2021381781098466,-2.1859067094505789,-2.1696897538517126,-2.1534877871170051,-2.13730130009651,
-2.121130799125122,-2.1049768064713659,-2.0888398607965049,-2.0727205176239987,-2.0566193498193557,-2.0405369480803812,-2.0244739214378211,-2.0084308977663721,-1.9924085243060123,-1.9764074681935719,-1.9604284170044561,
-1.9444720793043839,-1.928539185210991,-1.912630486965109,-1.896746759511496,-1.8808888010887623,-1.8650574338281911,-1.8492535043611165,-1.8334778844344761,-1.8177314715341071,-1.8020151895153087,-1.7863299892401368,
-1.770676849220848,-1.7550567762688454,-1.7394708061484221,-1.7239200042345311,-1.7084054661737442,-1.6929283185474953,-1.677489719536617,-1.662090859586123,-1.6467329620690872,-1.6314172839484016,-1.6161451164351008,
-1.6009177856418573,-1.5857366532301496,-1.5706031170495227,-1.5555186117672479,-1.5404846094866016,-1.5255026203518678,-1.5105741931380725,-1.4957009158233465,-1.480884416141705,-1.466126362113926,-1.4514284625540921,
-1.4367924675492589,-1.422220168909595,-1.4077134005862326,-1.3932740390539613,-1.3789040036557851,-1.3646052569062697,-1.3503798047504967,-1.3362296967753569,-1.3221570263698135,-1.308163930830698,-1.294252591410509,
-1.2804252333036288,-1.2666841255673123,-1.2530315809737507,-1.2394699557894811,-1.2260016494783939,-1.2126291043245754,-1.1993548049712357,-1.1861812778719965,-1.1731110906508564,-1.1601468513672155,-1.1472912076824213,
-1.1345468459244106,-1.1219164900471463,-1.1094029004817063,-1.0970088728760556,-1.0847372367207475,-1.072590853858026,-1.0605726168720666,-1.0486854473583827,-1.0369322940707426,-1.0253161309442953,-1.0138399549939723,
-1.0025067840876454,-0.991319654593957,-0.98028161890518983,-0.96939574283604624,-0.95866510289970819,-0.94809278346309289,-0.9376818737837711,-0.9274354649315979,-0.917356646598691,-0.907448503802005,-0.89771411348335883,
-0.88815654101240182,-0.87877883659862444,-0.86958403161914444,-0.86057513486961668,-0.85175512874621917,-0.84312696536726173,-0.83469356264353289,-0.8264578003070463,-0.81842251590836423,-0.81059050079315664,-0.80296449606909326,
-0.79554718857456252,-0.7883412068610598,-0.78134911720137479,-0.77457341963594817,-0.7680165440699378,-0.76168084643364486,-0.75556860491898992,-0.74968201630470355,-0.74402319238279147,-0.73859415649866633,-0.73339684021708917,
-0.72843308012574648,-0.72370461478789649,-0.71921308185505994,-0.71496001535019671,-0.71094684313121648,-0.707174884544014,-0.70364534827350156,-0.700359330400344,-0.69731781267028237,-0.69452166098207058,-0.69197162409915558,
-0.68966833258929916,-0.6876122979953917,-0.68580391223973747,-0.68424344726311437,-0.68293105489892836,-0.68186676698180837,-0.68105049568901876,-0.68048203411212072,-0.68016105705538932,-0.68008712205660038,-0.68025967062494319,
-0.68067802969000213,-0.6813414132549781,-0.6822489242466081,-0.68339955655357121,-0.68479219724457263,-0.68642562895674519,-0.688298532444532,-0.69040948927879409,-0.69275698468553615,-0.69533941051335391,-0.69815506831849106,
-0.70120217255622974,-0.704478853867251,-0.70798316244756376,-0.71171307149063356,-0.71566648069041872,-0.7198412197941656,-0.72423505219399742,-0.728845678546569,-0.73367074041033564,-0.73870782389030454,-0.74395446328048953,
-0.74940814469467665,-0.75506630967651978,-0.76092635878042469,-0.76698565511513617,-0.77324152784241607,-0.77969127562368623,-0.78633217000800459,-0.79316145875524147,-0.80017636908882828,-0.80737411087294686,-0.81475187970952911},
new double[]{
-5.1156067698088616,-5.0994789395422275,-5.0833511486800163,-5.0672233985139528,-5.0510956903781015,-5.0349680256502536,-5.0188404057533651,-5.0027128321570284,-4.98658530637901,-4.9704578299868212,-4.9543304045993546,-4.9382030318885626,
-4.9220757135812,-4.9059484514606178,-4.8898212473686176,-4.87369410320737,-4.8575670209413886,-4.8414400025995743,-4.8253130502773258,-4.8091861661387183,-4.7930593524187532,-4.7769326114256812,-4.7608059455434049,
-4.7446793572339541,-4.7285528490400477,-4.7124264235877371,-4.6963000835891346,-4.6801738318452344,-4.6640476712488237,-4.6479216047874905,-4.6317956355467276,-4.6156697667131406,-4.5995440015777609,-4.5834183435394644,
-4.5672927961085046,-4.5511673629101619,-4.535042047688508,-4.5189168543103007,-4.5027917867689968,-4.4866668491889055,-4.4705420458294736,-4.4544173810897076,-4.438292859512746,-4.4221684857905776,-4.4060442647689193,
-4.3899202014522425,-4.3737963010089755,-4.3576725687768727,-4.3415490102685528,-4.325425631177227,-4.30930243738261,-4.2931794349570236,-4.2770566301717023,-4.2609340295033027,-4.2448116396406279,-4.2286894674915665,
-4.2125675201902677,-4.1964458051045392,-4.1803243298434971,-4.164203102265458,-4.1480821304860918,-4.1319614228868433,-4.1158409881236233,-4.0997208351357877,-4.0836009731554066,-4.0674814117168365,-4.051362160666609,
-4.0352432301736343,-4.0191246307397446,-4.0030063732105745,-3.9868884687868045,-3.9707709290357629,-3.9546537659034096,-3.93853699172671,-3.9224206192464135,-3.906304661620243,-3.89018913243652,-3.8740740457282281,
-3.8579594159875392,-3.8418452581808076,-3.8257315877640541,-3.8096184206989538,-3.7935057734693425,-3.7773936630982612,-3.7612821071655524,-3.7451711238260303,-3.7290607318282381,-3.7129509505338172,-3.6968417999375025,
-3.6807333006877685,-3.6646254741081425,-3.6485183422192127,-3.6324119277613423,-3.6163062542181286,-3.6002013458406141,-3.5840972276722853,-3.5679939255748789,-3.5518914662550221,-3.5357898772917356,-3.5196891871648228,
-3.5035894252841797,-3.4874906220200454,-3.4713928087342336,-3.4552960178123668,-3.4392002826971537,-3.4231056379227343,-3.4070121191501319,-3.3909197632038479,-3.3748286081096284,-3.3587386931334473,-3.3426500588217407,
-3.3265627470429324,-3.3104768010302874,-3.2943922654261426,-3.2783091863275513,-3.2622276113333868,-3.2461475895929541,-3.2300691718561487,-3.2139924105252233,-3.1979173597081969,-3.1818440752739683,-3.1657726149091836,
-3.1497030381769089,-3.1336354065771679,-3.1175697836093947,-3.1015062348368732,-3.085444827953205,-3.0693856328508873,-3.0533287216920497,-3.0372741689814227,-3.0212220516416015,-3.00517244909068,-2.9891254433223176,
-2.973081118988318,-2.9570395634837907,-2.9410008670349788,-2.9249651227898186,-2.9089324269113277,-2.8929028786738917,-2.8768765805625409,-2.8608536383753003,-2.8448341613287083,-2.8288182621665841,-2.8128060572721507,
-2.796797666783597,-2.780793214713186,-2.7647928290700023,-2.7487966419864467,-2.7328047898485788,-2.7168174134304151,-2.7008346580322935,-2.6848566736234147,-2.6688836149886708,-2.6529156418798809,-2.6369529191715544,
-2.6209956170212916,-2.6050439110349561,-2.5890979824367353,-2.5731580182442184,-2.5572242114486232,-2.5412967612002939,-2.5253758729996125,-2.509461758893452,-2.4935546376773043,-2.4776547351032292,-2.461762284093755,
-2.4458775249618752,-2.4300007056372803,-2.4141320818989684,-2.3982719176143763,-2.3824204849851762,-2.366578064799882,-2.3507449466934069,-2.3349214294137179,-2.3191078210957334,-2.3033044395426008,-2.2875116125145012,
-2.2717296780251188,-2.2559589846459129,-2.2401998918183339,-2.2244527701741048,-2.2087180018637156,-2.1929959808932398,-2.1772871134696068,-2.1615918183544442,-2.1459105272265968,-2.1302436850534354,-2.1145917504710496,
-2.0989551961734123,-2.0833345093106046,-2.0677301918961675,-2.0521427612236423,-2.0365727502923581,-2.02102070824249,-2.0054872007994247,-1.9899728107274386,-1.9744781382926753,-1.9590038017354106,-1.9435504377515482,
-1.9281187019832935,-1.9127092695189136,-1.8973228354014766,-1.8819601151464378,-1.8666218452679095,-1.8513087838134275,-1.8360217109069905,-1.8207614293001257,-1.805528764930687,-1.7903245674890647,-1.7751497109914434,
-1.760005094359705,-1.7448916420075227,-1.7298103044321607,-1.7147620588114279,-1.6997479096051968,-1.6847688891608381,-1.6698260583218658,-1.6549205070390307,-1.6400533549830347,-1.6252257521579787,-1.6104388795145834,
-1.5956939495621572,-1.5809922069782096,-1.5663349292145361,-1.5517234270985134,-1.5371590454282793,-1.5226431635603679,-1.5081771959883026,-1.4937625929105498,-1.4794008407861508,-1.4650934628762571,-1.450842019769697,
-1.4366481098906119,-1.4225133699860975,-1.4084394755916925,-1.3944281414724562,-1.3804811220372819,-1.3666002117239908,-1.3527872453526582,-1.3390440984445255,-1.3253726875037586,-1.3117749702592236,-1.2982529458633616,
-1.2848086550451572,-1.2714441802141252,-1.2581616455121549,-1.2449632168099933,-1.2318511016450837,-1.2188275490974247,-1.2058948496000768,-1.1930553346809034,-1.1803113766321232,-1.1676653881042298,-1.1551198216208543,
-1.1426771690111546,-1.1303399607563576,-1.1181107652471332,-1.1059921879485501,-1.0939868704694513,-1.0820974895332043,-1.0703267558469092,-1.0586774128663035,-1.0471522354537839,-1.0357540284271651,-1.0244856249970236,
-1.013349885090731,-1.0023496935615568,-0.99148795828152481,-0.98076760811704811,-0.97019159078671324,-0.959762870600978,-0.94948442608395378,-0.93935924747787769,-0.92939033413134,-0.91958069177281188,-0.90993332967152307,
-0.90045125768825929,-0.89113748321918884,-0.88199500803638231,-0.873026825029254,-0.86423591485173268,-0.85562524248054506,-0.8471977536905867,-0.8389563714539312,-0.83090399226961165,-0.82304348243187542,-0.81537767424516572,
-0.80790936219462639,-0.80064129908143511,-0.79357619213276054,-0.78671669909659547,-0.78006542433213366,-0.77362491490673913,-0.76739765671088955,-0.7613860706027562,-0.7555925085943167,-0.75001925009106607,-0.74466849819750724,
-0.73954237610064966,-0.7346429235437294,-0.72997209340228031,-0.7255317483745336,-0.72132365779789942,-0.7173494946029928,-0.71361083241630519,-0.71010914282218873,-0.70684579279432858,-0.70382204230630963,-0.70103904213026658,
-0.69849783183191938,-0.69619933796956246,-0.69414437250379013,-0.6923336314239098,-0.69076769359612533,-0.689447019837675,-0.688371952220173,-0.68754271360446484,-0.68695940740833672,-0.68662201760745756,-0.68653040896896156,
-0.68668432751612218,-0.68708340122161915,-0.68772714092598064,-0.68861494147687852,-0.68974608308409469,-0.69111973288414985,-0.69273494670780011,-0.69459067104287886,-0.69668574518427673,-0.6990189035622294,-0.70158877823952015,
-0.70439390156769877,-0.707432708991983,-0.710703541994134,-0.71420465116229181,-0.71793419937651415,-0.72189026509858978,-0.7260708457545838,-0.73047386119853019,-0.73509715724569669,-0.73993850926392368,-0.74499562581166789,
-0.75026615231156335,-0.75574767474854743,-0.76143772338187543,-0.76733377646067136,-0.77343326393301981,-0.7797335711389991,-0.78623204247847966,-0.79292598504496037,-0.79981267221718944,-0.80688934720080585,-0.81415322651273891},
new double[]{
-5.0000453988992168,-4.9844218399812243,-4.9687983268059082,-4.9531748608251576,-4.9375514435369379,-4.9219280764867586,-4.9063047612691753,-4.8906814995293519,-4.8750582929646633,-4.85943514332635,-4.8438120524212342,-4.8281890221134756,
-4.8125660543263935,-4.7969431510443457,-4.7813203143146623,-4.7656975462496458,-4.7500748490286293,-4.7344522249001066,-4.7188296761839235,-4.7032072052735447,-4.6875848146383836,-4.6719625068262172,-4.65634028446567,
-4.6407181502687775,-4.6250961070336327,-4.6094741576471163,-4.5938523050877125,-4.5782305524284155,-4.5626089028397256,-4.546987359592741,-4.5313659260623522,-4.51574460573053,-4.5001234021897236,-4.4845023191463609,
-4.4688813604244677,-4.4532605299693957,-4.4376398318516648,-4.4220192702709396,-4.4063988495601194,-4.3907785741895617,-4.3751584487714466,-4.3595384780642652,-4.3439186669774639,-4.3282990205762291,-4.3126795440864214,
-4.297060242899672,-4.2814411225786344,-4.2658221888624084,-4.2502034476721295,-4.2345849051167406,-4.2189665674989429,-4.203348441321336,-4.1877305332927506,-4.1721128503347886,-4.1564953995885592,-4.1408781884216346,
-4.1252612244352278,-4.109644515471591,-4.0940280696216513,-4.07841189523289,-4.0627960009174622,-4.04718039556059,-4.031565088329196,-4.0159500886808361,-4.000335406372896,-3.9847210514720826,-3.9691070343642214,
-3.9534933657643543,-3.937880056727161,-3.9222671186577109,-3.9066545633225482,-3.891042402861133,-3.875430649797639,-3.8598193170531254,-3.8442084179580931,-3.8285979662654328,-3.8129879761637868,-3.7973784622913258,
-3.7817694397499659,-3.7661609241200278,-3.7505529314753607,-3.7349454783989411,-3.7193385819989633,-3.7037322599254332,-3.6881265303872892,-3.6725214121700569,-3.6569169246540589,-3.6413130878332,-3.6257099223343392,
-3.6101074494372711,-3.5945056910953337,-3.5789046699566653,-3.5633044093861246,-3.5477049334879012,-3.5321062671288308,-3.5165084359624457,-3.5009114664537742,-3.4853153859049191,-3.4697202224814343,-3.4541260052395266,
-3.4385327641541097,-3.4229405301477316,-3.4073493351204078,-3.3917592119803825,-3.3761701946758547,-3.3605823182276851,-3.34499561876313,-3.3294101335506214,-3.3138259010356279,-3.2982429608776354,-3.2826613539882725,
-3.2670811225706227,-3.2515023101597542,-3.2359249616645092,-3.2203491234105854,-3.2047748431849534,-3.1892021702816455,-3.1736311555489638,-3.1580618514381427,-3.1424943120535138,-3.1269285932042195,-3.111364752457515,
-3.095802849193714,-3.0802429446628214,-3.0646851020429065,-3.0491293865002627,-3.0335758652514149,-3.0180246076270216,-3.0024756851377306,-2.986929171542045,-2.9713851429162581,-2.9558436777265182,-2.940304856903083,
-2.92476876391683,-2.9092354848580846,-2.8937051085178367,-2.87817772647141,-2.8626534331646551,-2.8471323260027424,-2.8316145054416211,-2.8161000750822263,-2.8005891417675066,-2.7850818156823536,-2.7695782104565154,
-2.7540784432705707,-2.7385826349650575,-2.7230909101528327,-2.7076033973347591,-2.692120229018804,-2.6766415418426472,-2.6611674766998861,-2.6456981788699392,-2.630233798151743,-2.6147744890013431,-2.59932041067348,
-2.5838717273672742,-2.5684286083761165,-2.5529912282418676,-2.5375597669134806,-2.5221344099101524,-2.5067153484891183,-2.4913027798182057,-2.47589690715326,-2.4604979400205584,-2.4451060944043341,-2.4297215929395275,
-2.4143446651098883,-2.3989755474515455,-2.3836144837621758,-2.3682617253158855,-2.3529175310839423,-2.3375821679614655,-2.3222559110002212,-2.3069390436476276,-2.2916318579921109,-2.2763346550149288,-2.2610477448485939,
-2.2457714470420087,-2.2305060908324528,-2.2152520154245243,-2.2000095702761731,-2.1847791153919309,-2.1695610216234584,-2.1543556709775213,-2.139163456931505,-2.12398478475657,-2.1088200718485508,-2.0936697480666928,
-2.0785342560803186,-2.0634140517235053,-2.0483096043578519,-2.0332213972434059,-2.01814992791781,-2.0030957085837198,-1.9880592665045387,-1.9730411444084968,-1.958041900901097,-1.9430621108859327,-1.9281023659938747,
-1.9131632750205991,-1.8982454643724251,-1.883349578520404,-1.8684762804625879,-1.8536262521943829,-1.8388001951868753,-1.8239988308729913,-1.80922290114133,-1.7944731688374824,-1.7797504182726205,-1.7650554557391172,
-1.7503891100329152,-1.7357522329823438,-1.7211456999830388,-1.7065704105385839,-1.6920272888064631,-1.6775172841488559,-1.6630413716877832,-1.64860055286405,-1.6341958559993959,-1.6198283368612059,-1.6054990792290877,
-1.5912091954625651,-1.5769598270690788,-1.5627521452714304,-1.5485873515737421,-1.5344666783249361,-1.5203913892786816,-1.5063627801486779,-1.4923821791580783,-1.4784509475817818,-1.4645704802802488,-1.4507422062234148,
-1.4369675890031985,-1.4232481273330229,-1.4095853555326778,-1.395980843996778,-1.3824361996449734,-1.3689530663519933,-1.3555331253555125,-1.3421780956397409,-1.3288897342925496,-1.3156698368338657,-1.30252023751297,
-1.289442809572259,-1.2764394654749387,-1.263512157094044,-1.2506628758600944,-1.2378936528646232,-1.2252065589167471,-1.2126037045498765,-1.2000872399756037,-1.1876593549817591,-1.1753222787715694,-1.1630782797408172,
-1.1509296651898773,-1.1388787809674685,-1.1269280110429725,-1.1150797770041505,-1.1033365374771251,-1.0917007874655071,-1.0801750576056015,-1.068761913334682,-1.0574639539694013,-1.0462838116914996,-1.0352241504380872,
-1.0242876646939081,-1.0134770781831508,-1.0027951424585417,-0.992244635385659,-0.98182835952062164,-0.97154914037955586,-0.96140982459850666,-0.95141327798275244,-0.9415623834448,-0.93186003883067592,-0.922309154634491,
-0.91291265160164792,-0.90367345822146727,-0.89459450811044117,-0.88567873728777824,-0.87692908134537284,-0.868348472514827,-0.85993983663465434,-0.851706090021322,-0.84365013624831475,-0.83577486283794822,-0.82808313787120669,
-0.82057780652142953,-0.81326168751822281,-0.80613756954851534,-0.79920820760222067,-0.79247631927049045,-0.78594458100505571,-0.7796156243476442,-0.77349203213892692,-0.76757633471688635,-0.7618710061149,-0.75637846027020439,
-0.75110104725372651,-0.74604104953255035,-0.74120067827651859,-0.73658206972064355,-0.73218728159512492,-0.728018289634832,-0.72407698418010669,-0.7203651668806822,-0.71688454751437536,-0.71363674093202034,-0.71062326413984,
-0.70784553353012281,-0.70530486227066824,-0.7030024578630043,-0.70093941987884356,-0.69911673788365736,-0.69753528955559474,-0.69619583900726811,-0.6950990353171691,-0.69424541127667694,-0.69363538235777555,-0.69326924590571493,
-0.69314718055994529,-0.69326924590571493,-0.69363538235777555,-0.69424541127667694,-0.6950990353171691,-0.69619583900726811,-0.69753528955559474,-0.69911673788365736,-0.70093941987884356,-0.7030024578630043,-0.70530486227066824,
-0.70784553353012281,-0.71062326413984,-0.71363674093202034,-0.71688454751437536,-0.7203651668806822,-0.72407698418010669,-0.728018289634832,-0.73218728159512492,-0.73658206972064355,-0.74120067827651859,-0.74604104953255035,
-0.75110104725372651,-0.75637846027020439,-0.7618710061149,-0.76757633471688635,-0.77349203213892692,-0.7796156243476442,-0.78594458100505571,-0.79247631927049045,-0.79920820760222067,-0.80613756954851534,-0.81326168751822281},
new double[]{
-4.8918257398680378,-4.8766759350279427,-4.8615261828059211,-4.84637648482069,-4.8312268427407572,-4.8160772582859552,-4.8009277332290168,-4.7857782693972037,-4.7706288686739811,-4.75547953300075,-4.7403302643786249,-4.7251810648702719,
-4.7100319366018013,-4.6948828817647179,-4.6797339026179321,-4.6645850014898329,-4.6494361807804223,-4.6342874429635224,-4.6191387905890364,-4.6039902262852976,-4.5888417527614722,-4.57369337281005,-4.5585450893094048,
-4.5433969052264329,-4.5282488236192764,-4.5131008476401275,-4.4979529805381206,-4.482805225662311,-4.4676575864647479,-4.4525100665036366,-4.4373626694466077,-4.4222153990740747,-4.4070682592827044,-4.3919212540889863,
-4.3767743876329188,-4.3616276641818033,-4.3464810881341549,-4.3313346640237382,-4.31618839652372,-4.3010422904509555,-4.2858963507704022,-4.27075058259967,-4.2556049912137128,-4.2404595820496604,-4.2253143607118044,
-4.21016933297673,-4.195024504798611,-4.1798798823146628,-4.1647354718507685,-4.14959127992727,-4.13444731326494,-4.1193035787911425,-4.104160083646172,-4.0890168351897955,-4.0738738410079884,-4.0587311089198845,
-4.0435886469849294,-4.0284464635102637,-4.0133045670583209,-3.998162966454669,-3.9830216707960848,-3.9678806894588781,-3.95274003210747,-3.9375997087032353,-3.9224597295136139,-3.9073201051215012,-3.8921808464349268,
-3.8770419646970278,-3.8619034714963285,-3.8467653787773344,-3.8316276988514488,-3.8164904444082226,-3.8013536285269529,-3.7862172646886276,-3.7710813667882426,-3.7559459491474914,-3.7408110265278429,-3.7256766141440165,
-3.7105427276778733,-3.6954093832927257,-3.680276597648088,-3.6651443879148724,-3.6500127717910522,-3.6348817675178005,-3.619751393896121,-3.6046216703039851,-3.5894926167139909,-3.57436425371156,-3.5592366025136881,
-3.5441096849882667,-3.52898352367399,-3.5138581418008688,-3.4987335633113683,-3.4836098128821864,-3.468486915946694,-3.4533648987180565,-3.438243788213057,-3.4231236122766413,-3.4080043996072056,-3.3928861797826553,
-3.377768983287246,-3.3626528415392429,-3.3475377869194136,-3.3324238528003822,-3.317311073576874,-3.3021994846968692,-3.2870891226937005,-3.2719800252191158,-3.2568722310773413,-3.241765780260168,-3.2266607139830961,
-3.2115570747225668,-3.1964549062543122,-3.1813542536928603,-3.1662551635322234,-3.151157683687809,-3.1360618635395863,-3.1209677539765441,-3.1058754074424835,-3.0907848779831766,-3.0756962212949333,-3.0606094947746203,
-3.0455247575711706,-3.0304420706386264,-3.0153614967907614,-3.0002831007573283,-2.9852069492419733,-2.97013311098187,-2.9550616568091215,-2.9399926597139778,-2.9249261949099212,-2.9098623399006742,-2.894801174549182,
-2.8797427811486247,-2.8646872444955189,-2.8496346519649642,-2.834585093588093,-2.8195386621317913,-2.8044954531807429,-2.7894555652218722,-2.774419099731241,-2.7593861612634747,-2.7443568575437789,-2.7293312995626247,
-2.7143096016731687,-2.699291881691479,-2.6842782609996534,-2.6692688646518907,-2.6542638214836067,-2.6392632642236671,-2.6242673296098213,-2.60927615850742,-2.5942898960314991,-2.5793086916723231,-2.5643326994244635,
-2.549362077919517,-2.5343969905625388,-2.5194376056722994,-2.5044840966254447,-2.4895366420046661,-2.474595425750969,-2.4596606373201486,-2.4447324718435595,-2.4298111302932952,-2.4148968196518674,-2.3999897530864978,
-2.3850901501281214,-2.3701982368552112,-2.3553142460825307,-2.3404384175549198,-2.3255709981462229,-2.3107122420634743,-2.295862411056441,-2.2810217746326389,-2.2661906102779352,-2.2513692036828394,-2.2365578489745985,
-2.2217568489552053,-2.2069665153454303,-2.192187169034979,-2.1774191403388912,-2.1626627692602804,-2.1479184057595218,-2.1331864100299907,-2.1184671527804464,-2.10376101552417,-2.0890683908749343,-2.074389682849914,
-2.0597253071796078,-2.045075691624866,-2.0304412763010946,-2.0158225140097117,-2.0012198705769197,-1.9866338251998574,-1.9720648708001782,-1.9575135143851059,-1.9429802774160003,-1.9284656961844595,-1.9139703221959774,
-1.8994947225611631,-1.8850394803945123,-1.8706051952207172,-1.8561924833884789,-1.8418019784917781,-1.8274343317985404,-1.8130902126866151,-1.7987703090869744,-1.784475327934012,-1.7702059956228056,-1.7559630584731862,
-1.7417472832004286,-1.7275594573923576,-1.71340038999264,-1.6992709117899949,-1.6851718759130412,-1.6711041583304529,-1.6570686583560745,-1.6430662991586078,-1.6290980282754415,-1.6151648181301692,-1.6012676665532859,
-1.5874075973055244,-1.5735856606032403,-1.5598029336452142,-1.5460605211401872,-1.5323595558343985,-1.5187011990383419,-1.5050866411519015,-1.4915171021869731,-1.4779938322866182,-1.4645181122397357,-1.4510912539901784,
-1.4377146011391702,-1.4243895294398201,-1.4111174472824573,-1.3978997961694388,-1.3847380511780194,-1.3716337214097858,-1.3585883504250971,-1.3456035166608855,-1.3326808338301028,-1.3198219513010157,-1.307028554454476,
-1.2943023650172136,-1.2816451413691203,-1.2690586788224159,-1.2565448098705077,-1.24410540440428,-1.2317423698934753,-1.2194576515307511,-1.2072532323359346,-1.1951311332179175,-1.1830934129915822,-1.1711421683470775,
-1.1592795337687174,-1.14750768140072,-1.1358288208569618,-1.1242451989718891,-1.1127590994896917,-1.1013728426888325,-1.0900887849390086,-1.0789093181876239,-1.0678368693728579,-1.0568738997604434,-1.046022904201299,
-1.0352864103072088,-1.02466697754181,-1.014167196224226,-1.0037896864427804,-0.99353709687634062,-0.9834121035209703,-0.97341740831972412,-0.96355573769358716,-0.95382984097175194,-0.94424248871964211,-0.9347964709633213,
-0.92549459530918587,-0.91633968495811557,-0.90733457661355754,-0.89848211828334379,-0.8897851669753829,-0.88124658628773955,-0.872869243893997,-0.86465600892521177,-0.85660974925019417,-0.84873332865629714,-0.84102960393335724,
-0.83350142186391252,-0.82615161612331345,-0.81898300409384561,-0.811998383597499,-0.80520052955253274,-0.79859219055951225,-0.79217608542301465,-0.78595489961572118,-0.779931281692129,-0.77410783965961771,-0.76848713731509877,
-0.76307169055594726,-0.75786396367436637,-0.75286636564476228,-0.74808124641410023,-0.74351089320557573,-0.739157526846257,-0.73502329812963674,-0.73111028422426561,-0.72742048513982638,-0.72395582026213956,-0.72071812496866849,
-0.71770914733610869,-0.71493054495160413,-0.71238388183902535,-0.71007062551157285,-0.707992144161732,-0.70614970399930377,-0.70454446674786453,-0.70317748730957308,-0.70204971160774632,-0.701161974616062,-0.70051499858262689,
-0.70010939145646922,-0.69994564552328742,-0.70002413625650328,-0.70034512138884619,-0.7009087402088322,-0.70171501308560469,-0.70276384122468072,-0.70405500665620158,-0.7055881724563281,-0.7073628832014528,-0.70937856565393487,
-0.71163452967710117,-0.71412996937630757,-0.71686396446192513,-0.71983548182921442,-0.723043377349178,-0.72648639786365043,-0.73016318337709685,-0.734072269436852,-0.73821208969284668,-0.74258097862724071,-0.7471771744438187,
-0.75199882210649982,-0.75704397651588229,-0.7623106058123762,-0.76779659479418194,-0.77349974843814573,-0.7794177955113688,-0.78554839226135653,-0.79188912617247809,-0.79843751977654986,-0.80519103450546792,-0.81214707457398361}

    };

    #endregion

    #endregion
  }
}
