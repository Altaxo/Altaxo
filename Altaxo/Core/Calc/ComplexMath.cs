/*
 * BSD Licence:
 * Copyright (c) 2001, 2002 Ben Houston [ ben@exocortex.org ]
 * Exocortex Technologies [ www.exocortex.org ]
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without 
 * modification, are permitted provided that the following conditions are met:
 *
 * 1. Redistributions of source code must retain the above copyright notice, 
 * this list of conditions and the following disclaimer.
 * 2. Redistributions in binary form must reproduce the above copyright 
 * notice, this list of conditions and the following disclaimer in the 
 * documentation and/or other materials provided with the distribution.
 * 3. Neither the name of the <ORGANIZATION> nor the names of its contributors
 * may be used to endorse or promote products derived from this software
 * without specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
 * AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
 * ARE DISCLAIMED. IN NO EVENT SHALL THE REGENTS OR CONTRIBUTORS BE LIABLE FOR
 * ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
 * DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
 * SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
 * CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT 
 * LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY
 * OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH
 * DAMAGE.
 */

using System;
using System.Diagnostics;


namespace Altaxo.Calc 
{

  // Comments? Questions? Bugs? Tell Ben Houston at ben@exocortex.org
  // Version: May 4, 2002

  /// <summary>
  /// <p>Various mathematical functions for complex numbers.</p>
  /// </summary>
  public class ComplexMath 
  {
    
    //---------------------------------------------------------------------------------------------------

    private ComplexMath() 
    {
    }

    //---------------------------------------------------------------------------------------------------

    /// <summary>
    /// Swap two complex numbers
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    static public void Swap( ref Complex a, ref Complex b ) 
    {
      Complex temp = a;
      a = b;
      b = temp;
    }

    /// <summary>
    /// Swap two complex numbers
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    static public void Swap( ref ComplexF a, ref ComplexF b ) 
    {
      ComplexF temp = a;
      a = b;
      b = temp;
    }
    
    //---------------------------------------------------------------------------------------------------

    static private double _halfOfRoot2  = 0.5 * Math.Sqrt( 2 );

    /// <summary>
    /// Calculate the square root of a complex number
    /// </summary>
    /// <param name="c"></param>
    /// <returns></returns>
    static public ComplexF  Sqrt( ComplexF c ) 
    {
      double  x = c.Re;
      double  y = c.Im;

      double  modulus = Math.Sqrt( x*x + y*y );
      int   sign  = ( y < 0 ) ? -1 : 1;

      c.Re    = (float)( _halfOfRoot2 * Math.Sqrt( modulus + x ) );
      c.Im  = (float)( _halfOfRoot2 * sign * Math.Sqrt( modulus - x ) );

      return  c;
    }

    /// <summary>
    /// Calculate the square root of a complex number
    /// </summary>
    /// <param name="c"></param>
    /// <returns></returns>
    static public Complex Sqrt( Complex c ) 
    {
      double  x = c.Re;
      double  y = c.Im;

      double  modulus = Math.Sqrt( x*x + y*y );
      int   sign  = ( y < 0 ) ? -1 : 1;

      c.Re    = (double)( _halfOfRoot2 * Math.Sqrt( modulus + x ) );
      c.Im  = (double)( _halfOfRoot2 * sign * Math.Sqrt( modulus - x ) );

      return  c;
    }

    //---------------------------------------------------------------------------------------------------

    /// <summary>
    /// Calculate the power of a complex number
    /// </summary>
    /// <param name="c"></param>
    /// <param name="exponent"></param>
    /// <returns></returns>
    static public ComplexF  Pow( ComplexF c, double exponent ) 
    {
      double  x = c.Re;
      double  y = c.Im;
      
      double  modulus   = Math.Pow( x*x + y*y, exponent * 0.5 );
      double  argument  = Math.Atan2( y, x ) * exponent;

      c.Re    = (float)( modulus * System.Math.Cos( argument ) );
      c.Im = (float)( modulus * System.Math.Sin( argument ) );

      return  c;
    }

    /// <summary>
    /// Calculate the power of a complex number
    /// </summary>
    /// <param name="c"></param>
    /// <param name="exponent"></param>
    /// <returns></returns>
    static public Complex Pow( Complex c, double exponent ) 
    {
      double  x = c.Re;
      double  y = c.Im;
      
      double  modulus   = Math.Pow( x*x + y*y, exponent * 0.5 );
      double  argument  = Math.Atan2( y, x ) * exponent;

      c.Re    = (double)( modulus * System.Math.Cos( argument ) );
      c.Im = (double)( modulus * System.Math.Sin( argument ) );

      return  c;
    }
    
    //---------------------------------------------------------------------------------------------------

    #region AltaxoModified

    const double M_LOG10E = 0.43429448190325182765112891891661;

    private static double square(double x) 
    {
      return x*x;
    }


    /// <summary>
    /// The absolute value (modulus) of complex number.
    /// </summary>
    /// <param name="c">The complex argument.</param>
    /// <returns>The absolute value (also called modulus, length) of the complex number.</returns>
    /// <remarks>Only for completeness, you can also use <code>c.GetModulus()</code></remarks>
    public static double Abs(Complex c)
    {
      return c.GetModulus();
    }

    /// <summary>
    /// The argument value (also called phase) of a complex number.
    /// </summary>
    /// <param name="c">The complex number.</param>
    /// <returns>The argument (also called phase) of the complex number.</returns>
    /// <remarks>Only for completeness, you can also use <code>c.GetArgument()</code></remarks>
    public static double Arg(Complex c)
    {
      return c.GetArgument();
    }

    /// <summary>
    /// Create a complex number from a modulus (length) and an argument (radian)
    /// </summary>
    /// <param name="modulus">The modulus (length).</param>
    /// <param name="argument">The argument (angle, radian).</param>
    /// <returns>The complex number created from the modulus and argument.</returns>
    public static Complex Polar(double modulus, double argument)
    {
      return Complex.FromModulusArgument(modulus, argument); 
    } 

    /// <summary>
    /// Returns the complex angle whose cosine is the specified complex number.
    /// </summary>
    /// <param name="z">The function argument.</param>
    /// <returns>The complex number whose cosine is the function argument z.</returns>
    public static Complex Acos(Complex z)
    {
      Complex z1  = Complex.FromRealImaginary(1 - square(z.Re) + square(z.Im), -2*z.Re*z.Im);
      double  phi = Arg(z1)/2;
      double  r   = Math.Sqrt(Abs(z1));
      Complex z2  = Complex.FromRealImaginary(z.Re - r*Math.Sin(phi), z.Im + r*Math.Cos(phi));
      return Complex.FromRealImaginary(Arg(z2), -Math.Log(Abs(z2)));
    }


    /// <summary>
    /// Returns the complex angle whose sine is the specified complex number.
    /// </summary>
    /// <param name="z">The function argument.</param>
    /// <returns>The complex number whose sine is the function argument z.</returns>
    public static Complex Asin(Complex z)
    {
      Complex z1 = Complex.FromRealImaginary(1 - square(z.Re) + square(z.Im), -2*z.Re*z.Im);
      double phi = Arg(z1) / 2;
      double r = Math.Sqrt(Abs(z1));
      Complex z2 = Complex.FromRealImaginary(-z.Im + r*Math.Cos(phi), z.Re + r*Math.Sin(phi));
      return Complex.FromRealImaginary(Arg(z2), -Math.Log(Abs(z2)));
    }

    /// <summary>
    /// Returns the complex angle whose tangent is the specified complex number.
    /// </summary>
    /// <param name="z">The function argument.</param>
    /// <returns>The complex number whose tangent is the function argument z.</returns>
    public static Complex Atan(Complex  z)
    {
      double zip1 = 1 + z.Im;
      double zre2 = square(z.Re);
      double invlen = 1.0/(zip1*zip1 + zre2);
      Complex z1 = Complex.FromRealImaginary(((1-z.Im)*zip1 - zre2)*invlen, 2*z.Re*invlen);
      return Complex.FromRealImaginary(Arg(z1)/2, -Math.Log(Abs(z1))/2);
    }


    /// <summary>
    /// Returns the cosine of the specified complex function argument z.
    /// </summary>
    /// <param name="z">Function argument.</param>
    /// <returns>The cosine of the specified complex function argument z.</returns>
    public static Complex Cos(Complex z)
    {
      double ezi  = Math.Exp(z.Im);
      double inv = 1.0 / ezi;
      return Complex.FromRealImaginary(0.5*Math.Cos(z.Re)*(inv+ezi), 0.5*Math.Sin(z.Re)*(inv-ezi));
    } 


    /// <summary>
    /// Returns the hyperbolic cosine of the specified complex function argument z.
    /// </summary>
    /// <param name="z">Function argument.</param>
    /// <returns>The hyperbolic cosine of the specified complex function argument z.</returns>
    public static Complex Cosh(Complex  z)
    {
      double ezr = Math.Exp(z.Re);
      double inv = 1.0 / ezr;
      return Complex.FromRealImaginary(0.5*Math.Cos(z.Im)*(ezr+inv), 0.5*Math.Sin(z.Im)*(ezr-inv));
    }


    /// <summary>
    /// Returns the exponential function of the complex function argument.
    /// </summary>
    /// <param name="z">The complex function argument.</param>
    /// <returns>The exponential function of the spezified complex function argument.</returns>
    public static Complex Exp(Complex z)
    {
      return Complex.FromModulusArgument(Math.Exp(z.Re),z.Im);
    }
 
    /// <summary>
    /// Returns the natural (base e) logarithm of the complex function argument.
    /// </summary>
    /// <param name="z">The complex function argument.</param>
    /// <returns>The natural (base e) logarithm of the complex function argument.</returns>
    public static Complex Log(Complex z)
    {
      return new Complex(Math.Log(z.GetModulus()), Math.Atan2(z.Im,z.Re));
    }

    /// <summary>
    /// Returns the base 10 logarithm of the complex function argument.
    /// </summary>
    /// <param name="z">The complex function argument.</param>
    /// <returns>The base 10 logarithm of the complex function argument.</returns>
    public static Complex Log10(Complex z)
    {
      return Log(z) * M_LOG10E;
    }

    /// <summary>
    /// Returns a specified (real valued) number raised to the specified (complex valued) power.
    /// </summary>
    /// <param name="z">A number to be raised to a power.</param>
    /// <param name="p">A number that specifies a power.</param>
    /// <returns>The number z raised to the power p.</returns>
    public static Complex Pow(double z, Complex p)
    {
      if (z == 0 && p.Re > 0) 
        return Complex.Zero;

      double logz = Math.Log(Math.Abs(z));
      if (z > 0.0)
        return Exp(p * logz);
      else
        return Exp(p * Complex.FromRealImaginary(logz, Math.PI));
    }

    /// <summary>
    /// Returns a specified (complex valued) number raised to the specified (complex valued) power.
    /// </summary>
    /// <param name="z">A number to be raised to a power.</param>
    /// <param name="p">A number that specifies a power.</param>
    /// <returns>The number z raised to the power p.</returns>
    public static Complex Pow(Complex z, Complex p)
    {
      if (z.Re == 0 && z.Im==0 && p.Re> 0)
        return Complex.Zero;
      else
        return Exp(p * Log(z));
    }

    /// <summary>
    /// Returns the sine of the specified complex function argument z.
    /// </summary>
    /// <param name="z">Function argument.</param>
    /// <returns>The sine of the specified complex function argument z.</returns>
    public static Complex Sin(Complex z)
    {
      double ezi = Math.Exp(z.Im);
      double inv = 1.0/ezi;
      return Complex.FromRealImaginary(0.5*Math.Sin(z.Re)*(inv+ezi), -0.5*Math.Cos(z.Re)*(inv-ezi));
    }

    /// <summary>
    /// Returns the hyperbolic sine of the specified complex function argument z.
    /// </summary>
    /// <param name="z">Function argument.</param>
    /// <returns>The hyperbolic sine of the specified complex function argument z.</returns>
    public static Complex Sinh(Complex z)
    {
      double ezr =Math.Exp(z.Re);
      double inv = 1.0/ezr;
      return Complex.FromRealImaginary(0.5*Math.Cos(z.Im)*(ezr-inv), 0.5*Math.Sin(z.Im)*(ezr+inv));
    }

    /// <summary>
    /// Returns the tangent of the specified complex function argument z.
    /// </summary>
    /// <param name="z">Function argument.</param>
    /// <returns>The tangent of the specified complex function argument z.</returns>
    public static Complex Tan(Complex z)
    {
      double sinzr = Math.Sin(z.Re);
      double coszr = Math.Cos(z.Re);
      double ezi   = Math.Exp(z.Im);
      double inv   = 1.0/ezi;
      double ediff = inv - ezi;
      double esum  = inv + ezi;
      return Complex.FromRealImaginary(4*sinzr*coszr, -ediff*esum)/(square(coszr*esum) + square(sinzr*ediff));
    }

    /// <summary>
    /// Returns the hyperbolic tangent of the specified complex function argument z.
    /// </summary>
    /// <param name="z">Function argument.</param>
    /// <returns>The hyperbolic tangent of the specified complex function argument z.</returns>
    public static Complex Tanh(Complex z)
    {
      double sinzi = Math.Sin(z.Im);
      double coszi = Math.Cos(z.Im);
      double ezr   = Math.Exp(z.Re);
      double inv   = 1.0/ezr;
      double ediff = ezr - inv;
      double esum  = ezr + inv;
      return Complex.FromRealImaginary(ediff*esum, 4*sinzi*coszi)/(square(coszi*esum)+square(sinzi*ediff));
    }



    #endregion
  }
}
