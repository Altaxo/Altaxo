#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2005 Dr. Dirk Lellinger
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
using System.Diagnostics;

namespace Altaxo.Calc 
{
  /// <summary>
  /// <p>Various mathematical functions for complex numbers.</p>
  /// </summary>
  public class ComplexMath 
  {
    #region private helper constants and functions
    const double M_PI = Math.PI;
    const double M_PI_2 = Math.PI/2;
    const double GSL_DBL_EPSILON = 2.2204460492503131e-16;
    const double M_LOG10E = 0.43429448190325182765112891891661;

    private static double square(double x) 
    {
      return x*x;
    }

    private static double hypot(double x,double y)
    {
      double xabs = Math.Abs(x) ;
      double yabs = Math.Abs(y) ;
      double min, max;

      if (xabs < yabs) 
      {
        min = xabs ;
        max = yabs ;
      } 
      else 
      {
        min = yabs ;
        max = xabs ;
      }

      if (min == 0) 
      {
        return max ;
      }

    {
      double u = min / max ;
      return max * Math.Sqrt (1 + u * u) ;
    }
    }

    #endregion

    #region public helper elementary operations

    /// <summary>
    /// This function returns the product of the complex number a and the
    /// imaginary number iy, z=a*(iy).
    /// </summary>
    /// <param name="a">First multiplicant.</param>
    /// <param name="y">Imaginary part of second multiplicant.</param>
    /// <returns>The product of the complex number a and the
    /// imaginary number iy, z=a*(iy).</returns>
    public static Complex MultiplyImaginaryNumber(Complex a, double y)
    {                            
      return new Complex( -y * a.Im, y * a.Re);
    }

    /// <summary>
    /// This function returns the product of the complex number a and the
    /// real number x, z=ax.
    /// </summary>
    /// <param name="a">First multiplicant.</param>
    /// <param name="x">Real part of second multiplicant.</param>
    /// <returns>The product of the complex number a and the
    /// real number x, z=ax.</returns>
    public static Complex MultiplyRealNumber(Complex a, double x)
    {                      
      return new Complex( x * a.Re, x * a.Im);
    }

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
    static public void Swap( ref ComplexFloat a, ref ComplexFloat b ) 
    {
      ComplexFloat temp = a;
      a = b;
      b = temp;
    }

    #endregion

    #region Functions

    


    /// <summary>
    /// The absolute value (modulus) of complex number.
    /// </summary>
    /// <param name="c">The complex argument.</param>
    /// <returns>The absolute value (also called modulus, length, euclidean norm) of the complex number.</returns>
    /// <remarks>Only for completeness, you can also use <code>c.GetModulus()</code></remarks>
    public static double Abs(Complex c)
    {
      return c.GetModulus();
    }

    /// <summary>
    /// The squared modulus (length^2) of the complex number
    /// </summary>
    /// <param name="c">The complex argument.</param>
    /// <returns>The squared modulus (length^2) of the complex number.</returns>
    /// <remarks>Only for completeness, you can also use <code>c.GetModulusSquared()</code></remarks>
    public static double Abs2(Complex c)
    {
      return c.GetModulusSquared();
    }

    ///<summary>Return the absolute value of a complex type calculated as the euclidean norm</summary>
    ///<remarks>Same as <see cref="Abs" /> and provided here for compatibility with some libraries.</remarks>
    public static double Absolute(Complex c)
    {
      return c.GetModulus();
    }
    ///<summary>Return the absolute value of a complex type calculated as the euclidean norm</summary>
    ///<remarks>Same as <see cref="Abs" /> and provided here for compatibility with some libraries.</remarks>
    public static float Absolute(ComplexFloat c)
    {
      return c.GetModulus();
    }
  
    

    /// <summary>
    /// This function returns the complex arccosine of the complex number a,
    /// arccos(a). The branch cuts are on the real axis, less than -1
    /// and greater than 1.
    /// </summary>
    /// <param name="a">The function argument.</param>
    /// <returns>The complex arccosine of the complex number a.</returns>
    public static Complex Acos(Complex a)
    {
      double R = a.Re, I = a.Im;
      Complex z;

      if (I == 0)
      {
        z = Acos(R);
      }
      else
      {
        double x = Math.Abs (R), y = Math.Abs (I);
        double r = hypot (x + 1, y), s = hypot (x - 1, y);
        double A = 0.5 * (r + s);
        double B = x / A;
        double y2 = y * y;

        double real, imag;

        const double A_crossover = 1.5, B_crossover = 0.6417;

        if (B <= B_crossover)
        {
          real = Math.Acos (B);
        }
        else
        {
          if (x <= 1)
          {
            double D = 0.5 * (A + x) * (y2 / (r + x + 1) + (s + (1 - x)));
            real = Math.Atan (Math.Sqrt (D) / x);
          }
          else
          {
            double Apx = A + x;
            double D = 0.5 * (Apx / (r + x + 1) + Apx / (s + (x - 1)));
            real = Math.Atan ((y * Math.Sqrt (D)) / x);
          }
        }

        if (A <= A_crossover)
        {
          double Am1;

          if (x < 1)
          {
            Am1 = 0.5 * (y2 / (r + (x + 1)) + y2 / (s + (1 - x)));
          }
          else
          {
            Am1 = 0.5 * (y2 / (r + (x + 1)) + (s + (x - 1)));
          }

          imag = RMath.Log1p (Am1 + Math.Sqrt (Am1 * (A + 1)));
        }
        else
        {
          imag = Math.Log (A + Math.Sqrt (A * A - 1));
        }

        z= new Complex( (R >= 0) ? real : Math.PI - real, (I >= 0) ? -imag : imag);
      }
      return z;
    }

    /// <summary>
    /// This function returns the complex arccosine of the complex number a,
    /// arccos(a). The branch cuts are on the real axis, less than -1
    /// and greater than 1.
    /// </summary>
    /// <param name="a">The function argument.</param>
    /// <returns>The complex arccosine of the complex number a.</returns>
    public static ComplexFloat Acos(ComplexFloat a)
    {
      return (ComplexFloat)Acos((Complex)a);
    }

    /// <summary>
    /// This function returns the complex arccosine of the real number a, arccos(a). 
    /// </summary>
    /// <param name="a">The function argument.</param>
    /// <returns>The complex arccosine of the real number a.</returns>
    /// <remarks>
    /// For a between -1 and 1, the
    /// function returns a real value in the range [0,pi]. For a
    /// less than -1 the result has a real part of pi/2 and a
    /// negative imaginary part.  For a greater than 1 the result
    /// is purely imaginary and positive.
    /// </remarks>
    public static Complex Acos(double a)
    {                              
      Complex z;

      if (Math.Abs (a) <= 1.0)
      {
        z = new Complex( Math.Acos(a), 0);
      }
      else
      {
        if (a < 0.0)
        {
          z = new Complex( Math.PI, -RMath.Acosh(-a));
        }
        else
        {
          z = new Complex( 0, RMath.Acosh(a));
        }
      }

      return z;
    }

    /// <summary>
    /// This function returns the complex hyperbolic arccosine of the complex
    /// number a,  arccosh(a).  The branch cut is on the real axis,
    /// less than  1.
    /// </summary>
    /// <param name="a">Function argument.</param>
    /// <returns>The complex hyperbolic arccosine of the complex number a.</returns>
    public static Complex Acosh(Complex a)
    {                  
      Complex z = Acos(a);
      z = MultiplyImaginaryNumber (z, z.Im > 0 ? -1.0 : 1.0);
      return z;
    }

    /// <summary>
    /// This function returns the complex hyperbolic arccosine of the complex
    /// number a,  arccosh(a).  The branch cut is on the real axis,
    /// less than  1.
    /// </summary>
    /// <param name="a">Function argument.</param>
    /// <returns>The complex hyperbolic arccosine of the complex number a.</returns>
    public static ComplexFloat Acosh(ComplexFloat a)
    {
      return (ComplexFloat)Acosh((Complex)a);
    }

    /// <summary>
    /// This function returns the complex hyperbolic arccosine of
    /// the real number a, arccosh(a).
    /// </summary>
    /// <param name="a">Function argument.</param>
    /// <returns>The complex hyperbolic arccosine of
    /// the real number a.</returns>
    public static Complex Acosh(double a)
    {                         
      Complex z;

      if (a >= 1)
      {
        z = new Complex(RMath.Acosh (a), 0);
      }
      else
      {
        if (a >= -1.0)
        {
          z = new Complex( 0, Math.Acos(a));
        }
        else
        {
          z = new Complex(RMath.Acosh (-a), M_PI);
        }
      }

      return z;
    }

    /// <summary>
    /// This function returns the complex arccotangent of the complex number a,
    /// arccot(a) = arctan(1/a). 
    /// </summary>
    /// <param name="a">Function argument.</param>
    /// <returns>The complex arccotangent of the complex number a.</returns>
    public static Complex Acot (Complex a)
    {                            
      Complex z;

      if (a.Re == 0.0 && a.Im == 0.0)
      {
        z = new Complex( M_PI_2, 0);
      }
      else
      {
        z = Inverse(a);
        z = Atan(z);
      }

      return z;
    }

    /// <summary>
    /// This function returns the complex hyperbolic arccotangent of the complex
    /// number a, arccoth(a) = arctanh(1/a).
    /// </summary>
    /// <param name="a">Function argument.</param>
    /// <returns>The complex hyperbolic arccotangent of the complex
    /// number a.</returns>
    public static Complex Acoth(Complex a)
    {                            
      return Atanh(Inverse(a));
    }

    /// <summary>
    /// This function returns the complex arccosecant of the complex number a,
    /// arccsc(a) = arcsin(1/a). 
    /// </summary>
    /// <param name="a">Function argument.</param>
    /// <returns>The complex arccosecant of the complex number a.</returns>
    public static Complex Acsc(Complex a)
    {           
      return Asin(Inverse(a));
    }

    /// <summary>
    /// This function returns the complex hyperbolic arccosecant of the complex
    /// number a,  arccsch(a) = arcsin(1/a).
    /// </summary>
    /// <param name="a">Function argument.</param>
    /// <returns>The complex hyperbolic arccosecant of the complex
    /// number a.</returns>
    public static Complex Acsch(Complex a)
    {                            
      return Asinh(Inverse(a));
    }

    /// <summary>
    /// This function returns the complex arccosecant of the real number a,
    /// arccsc(a) = arcsin(1/a). 
    /// </summary>
    /// <param name="a">Function argument.</param>
    /// <returns>The complex arccosecant of the real number a.</returns>
    public static Complex Acsc(double a)
    {                         
      Complex z;

      if (a <= -1.0 || a >= 1.0)
      {
        z = new Complex( Math.Asin (1 / a), 0.0);
      }
      else
      {
        if (a >= 0.0)
        {
          z = new Complex( M_PI_2, -RMath.Acosh (1 / a));
        }
        else
        {
          z = new Complex( -M_PI_2, RMath.Acosh (-1 / a));
        }
      }

      return z;
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
    /// The argument value (also called phase) of a complex number.
    /// </summary>
    /// <param name="c">The complex number.</param>
    /// <returns>The argument (also called phase) of the complex number.</returns>
    /// <remarks>Only for completeness, you can also use <code>c.GetArgument()</code></remarks>
    public static double Arg(ComplexFloat c)
    {
      return c.GetArgument();
    }

    ///<summary>Calculate the complex argument of a complex type.  Also commonly refered to as the phase.</summary>
    public static double Argument(Complex value)
    {
      return System.Math.Atan(value.Imag / value.Real);
    }
    ///<summary>Calculate the complex argument of a complex type.  Also commonly refered to as the phase.</summary>
    public static float Argument(ComplexFloat value)
    {
      return (float)System.Math.Atan(value.Imag / value.Real);
    }

    ///<summary>Calculate the 2-argument of a complex type.</summary>
    public static double Argument2(Complex value)
    {
      return System.Math.Atan2(value.Imag, value.Real);
    }

    ///<summary>Calculate the 2-argument of a complex type.</summary>
    public static float Argument2(ComplexFloat value)
    {
      return (float)System.Math.Atan2(value.Imag, value.Real);
    }

    /// <summary>
    /// This function returns the complex arcsecant of the complex number a,
    /// arcsec(a) = arccos(1/a). 
    /// </summary>
    /// <param name="a">Function argument.</param>
    /// <returns>The complex arcsecant of the complex number a.</returns>
    public static Complex Asec (Complex a)
    {                            
      return Acos(Inverse(a));
    }

    /// <summary>
    /// This function returns the complex arcsecant of the real number a,
    /// arcsec(a) = arccos(1/a). 
    /// </summary>
    /// <param name="a">Function argument.</param>
    /// <returns>The complex arcsecant of the real number a.</returns>
    public static Complex Asec(double a)
    {           
      Complex z;

      if (a <= -1.0 || a >= 1.0)
      {
        z = new Complex( Math.Acos (1 / a), 0.0);
      }
      else
      {
        if (a >= 0.0)
        {
          z = new Complex( 0, RMath.Acosh (1 / a));
        }
        else
        {
          z = new Complex( M_PI, -RMath.Acosh (-1 / a));
        }
      }

      return z;
    }

    /// <summary>
    /// This function returns the complex hyperbolic arcsecant of the complex
    /// number a, arcsech(a) = arccosh(1/a).
    /// </summary>
    /// <param name="a">Function argument.</param>
    /// <returns>The complex hyperbolic arcsecant of the complex
    /// number a.</returns>
    public static Complex Asech(Complex a)
    {                            
      return Acosh(Inverse(a));
    }

    /// <summary>
    /// This function returns the complex arcsine of the complex number a,
    /// arcsin(a)}. The branch cuts are on the real axis, less than -1
    /// and greater than 1.
    /// </summary>
    /// <param name="a">The function argument.</param>
    /// <returns>the complex arcsine of the complex number a.</returns>
    public static Complex Asin(Complex a)
    {
      double R = a.Re, I = a.Im;
      Complex z;

      if (I == 0)
      {
        z = Asin(R);
      }
      else
      {
        double x = Math.Abs(R), y = Math.Abs(I);
        double r = hypot (x + 1, y), s = hypot (x - 1, y);
        double A = 0.5 * (r + s);
        double B = x / A;
        double y2 = y * y;

        double real, imag;

        const double A_crossover = 1.5, B_crossover = 0.6417;

        if (B <= B_crossover)
        {
          real = Math.Asin (B);
        }
        else
        {
          if (x <= 1)
          {
            double D = 0.5 * (A + x) * (y2 / (r + x + 1) + (s + (1 - x)));
            real = Math.Atan (x / Math.Sqrt (D));
          }
          else
          {
            double Apx = A + x;
            double D = 0.5 * (Apx / (r + x + 1) + Apx / (s + (x - 1)));
            real = Math.Atan (x / (y * Math.Sqrt (D)));
          }
        }

        if (A <= A_crossover)
        {
          double Am1;

          if (x < 1)
          {
            Am1 = 0.5 * (y2 / (r + (x + 1)) + y2 / (s + (1 - x)));
          }
          else
          {
            Am1 = 0.5 * (y2 / (r + (x + 1)) + (s + (x - 1)));
          }

          imag = RMath.Log1p (Am1 + Math.Sqrt (Am1 * (A + 1)));
        }
        else
        {
          imag = Math.Log (A + Math.Sqrt (A * A - 1));
        }

        z = new Complex( (R >= 0) ? real : -real, (I >= 0) ? imag : -imag);
      }

      return z;
    }

    /// <summary>
    /// This function returns the complex arcsine of the complex number a,
    /// arcsin(a)}. The branch cuts are on the real axis, less than -1
    /// and greater than 1.
    /// </summary>
    /// <param name="a">The function argument.</param>
    /// <returns>the complex arcsine of the complex number a.</returns>
    public static ComplexFloat Asin(ComplexFloat a)
    {
      return (ComplexFloat)Asin((Complex)a);
    }

    /// <summary>
    /// This function returns the complex arcsine of the real number a,
    /// arcsin(a). 
    /// </summary>
    /// <param name="a">The function argument.</param>
    /// <returns>The complex arcsine of the real number a.</returns>
    /// <remarks>
    /// For a between -1 and 1, the
    /// function returns a real value in the range -(pi,pi]. For
    /// a less than -1 the result has a real part of -pi/2
    /// and a positive imaginary part.  For a greater than 1 the
    /// result has a real part of pi/2 and a negative imaginary part.
    /// </remarks>
    public static Complex Asin(double a)
    {                               
      Complex z;

      if (Math.Abs(a) <= 1.0)
      {
        z = new Complex(Math.Asin(a), 0.0);
      }
      else
      {
        if (a < 0.0)
        {
          z = new  Complex(-M_PI_2, RMath.Acosh(-a));
        }
        else
        {
          z = new Complex(M_PI_2, -RMath.Acosh (a));
        }
      }

      return z;
    }

                 

    /// <summary>
    /// This function returns the complex hyperbolic arcsine of the
    /// complex number a,  arcsinh(a).  The branch cuts are on the
    /// imaginary axis, below -i and above i.
    /// </summary>
    /// <param name="a">Function argument.</param>
    /// <returns>The complex hyperbolic arcsine of the complex number a.</returns>
    public static Complex Asinh(Complex a)
    {
      Complex z = MultiplyImaginaryNumber(a, 1.0);
      z = Asin (z);
      z = MultiplyImaginaryNumber(z, -1.0);
      return z;
    }

    /// <summary>
    /// This function returns the complex hyperbolic arcsine of the
    /// complex number a,  arcsinh(a).  The branch cuts are on the
    /// imaginary axis, below -i and above i.
    /// </summary>
    /// <param name="a">Function argument.</param>
    /// <returns>The complex hyperbolic arcsine of the complex number a.</returns>
    public static ComplexFloat Asinh(ComplexFloat a)
    {
      return (ComplexFloat)Asinh((Complex)a);
    }

    /// <summary>
    /// This function returns the complex arctangent of the complex number
    /// a,  arctan(a)}. The branch cuts are on the imaginary axis,
    /// below  -i  and above  i .
    /// </summary>
    /// <param name="a">The function argument.</param>
    /// <returns>The complex arctangent of the complex number a.</returns>
    public static Complex Atan(Complex  a)
    {
      
      double R = a.Re, I = a.Im;
      Complex z;

      if (I == 0)
      {
        z = new Complex(Math.Atan(R), 0);
      }
      else
      {
        /* FIXME: This is a naive implementation which does not fully
             take into account cancellation errors, overflow, underflow
             etc.  It would benefit from the Hull et al treatment. */

        double r = hypot (R, I);

        double imag;

        double u = 2 * I / (1 + r * r);

        /* FIXME: the following cross-over should be optimized but 0.1
             seems to work ok */

        if (Math.Abs (u) < 0.1)
        {
          imag = 0.25 * (RMath.Log1p (u) - RMath.Log1p (-u));
        }
        else
        {
          double A = hypot (R, I + 1);
          double B = hypot (R, I - 1);
          imag = 0.5 * Math.Log (A / B);
        }

        if (R == 0)
        {
          if (I > 1)
          {
            z = new Complex( M_PI_2, imag);
          }
          else if (I < -1)
          {
            z = new Complex( -M_PI_2, imag);
          }
          else
          {
            z = new Complex( 0, imag);
          }
        }
        else
        {
          z = new Complex( 0.5 * Math.Atan2 (2 * R, ((1 + r) * (1 - r))), imag);
        }
      }

      return z;
    }

    /// <summary>
    /// This function returns the complex arctangent of the complex number
    /// a,  arctan(a)}. The branch cuts are on the imaginary axis,
    /// below  -i  and above  i .
    /// </summary>
    /// <param name="a">The function argument.</param>
    /// <returns>The complex arctangent of the complex number a.</returns>
    public static ComplexFloat Atan(ComplexFloat a)
    {
      return (ComplexFloat)Atan((Complex)a);
    }

    
    /// <summary>
    /// This function returns the complex hyperbolic arctangent of the complex
    /// number a,  arctanh(a).  The branch cuts are on the real
    /// axis, less than -1 and greater than 1.
    /// </summary>
    /// <param name="a">Function argument.</param>
    /// <returns>The complex hyperbolic arctangent of the complex
    /// number a.</returns>
    public static Complex Atanh(Complex a)
    {                            
      if (a.Im == 0.0)
      {
        return Atanh(a.Re);
      }
      else
      {
        Complex z = MultiplyImaginaryNumber(a, 1.0);
        z = Atan(z);
        z = MultiplyImaginaryNumber(z, -1.0);
        return z;
      }
    }

    /// <summary>
    /// This function returns the complex hyperbolic arctangent of the complex
    /// number a,  arctanh(a).  The branch cuts are on the real
    /// axis, less than -1 and greater than 1.
    /// </summary>
    /// <param name="a">Function argument.</param>
    /// <returns>The complex hyperbolic arctangent of the complex
    /// number a.</returns>
    public static ComplexFloat Atanh(ComplexFloat a)
    {
      return (ComplexFloat)Atanh((Complex)a);
    }

    /// <summary>
    /// This function returns the complex hyperbolic arctangent of the real
    /// number a, arctanh(a).
    /// </summary>
    /// <param name="a">Function argument.</param>
    /// <returns>The complex hyperbolic arctangent of the real
    /// number a.</returns>
    public static Complex Atanh(double a)
    {                          
      Complex z;

      if (a > -1.0 && a < 1.0)
      {
        z = new Complex( RMath.Atanh(a), 0);
      }
      else
      {
        z = new Complex( RMath.Atanh (1 / a), (a < 0) ? M_PI_2 : -M_PI_2);
      }

      return z;
    }

    ///<summary>Return the complex conjugate of a complex type</summary>
    public static Complex Conjugate(Complex a)
    {
      return new Complex(a.Re, -a.Im);
    }

    ///<summary>Return the complex conjugate of a complex type</summary>
    public static ComplexFloat Conjugate(ComplexFloat a)
    {
      return new ComplexFloat(a.Re, -a.Im);
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
    /// Returns the cosine of the specified complex function argument z.
    /// </summary>
    /// <param name="z">Function argument.</param>
    /// <returns>The cosine of the specified complex function argument z.</returns>
    public static ComplexFloat Cos(ComplexFloat z)
    {
      double ezi = Math.Exp(z.Im);
      double inv = 1.0 / ezi;
      return ComplexFloat.FromRealImaginary((float)(0.5 * Math.Cos(z.Re) * (inv + ezi)), (float)( 0.5 * Math.Sin(z.Re) * (inv - ezi)));
    } 

    /// <summary>
    /// This function returns the complex hyperbolic cosine of the complex number
    /// a, cosh(a) = (exp(a) + exp(-z))/2.
    /// </summary>
    /// <param name="a">Function argument.</param>
    /// <returns>The hyperbolic cosine of the specified complex function argument z.</returns>
    public static Complex Cosh(Complex  a)
    {
      double R = a.Re, I = a.Im;
      return new Complex( Math.Cosh (R) * Math.Cos (I), Math.Sinh (R) * Math.Sin (I));
    }

    /// <summary>
    /// This function returns the complex hyperbolic cosine of the complex number
    /// a, cosh(a) = (exp(a) + exp(-z))/2.
    /// </summary>
    /// <param name="a">Function argument.</param>
    /// <returns>The hyperbolic cosine of the specified complex function argument z.</returns>
    public static ComplexFloat Cosh(ComplexFloat a)
    {
      double R = a.Re, I = a.Im;
      return new ComplexFloat((float) (Math.Cosh(R) * Math.Cos(I)), (float)(Math.Sinh(R) * Math.Sin(I)));
    }
  

    /// <summary>
    /// Returns the complex cotangent of the complex number z, i.e. 1/Sin(z).
    /// </summary>
    /// <param name="z">The function argument.</param>
    /// <returns>Complex cotangent of the complex number z, i.e. 1/Tan(z).</returns>
    public static Complex Cot(Complex z)
    {                              
      return Inverse(Tan(z));
    }

    /// <summary>
    /// This function returns the complex hyperbolic cotangent of the complex
    /// number a, coth(a) = 1/tanh(a)}.
    /// </summary>
    /// <param name="a">Function argument.</param>
    /// <returns>The complex hyperbolic cotangent of the complex number a.</returns>
    public static Complex Coth(Complex a)
    {                           
      return Inverse(Tanh(a));
    }

    /// <summary>
    /// Returns the complex cosecant of the complex number z, i.e. 1/Sin(z).
    /// </summary>
    /// <param name="z">The function argument.</param>
    /// <returns>Complex cosecant of the complex number z, i.e. 1/Sin(z).</returns>
    public static Complex Csc(Complex z)
    {                              
      return Inverse(Sin(z));
    }

    /// <summary>
    /// This function returns the complex hyperbolic cosecant of the complex
    /// number a, csch(a) = 1/sinh(a)}.
    /// </summary>
    /// <param name="a">Function argument.</param>
    /// <returns>The complex hyperbolic cosecant of the complex number a.</returns>
    public static Complex Csch(Complex a)
    {                           
      return Inverse(Sinh(a));
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
    /// Returns the exponential function of the complex function argument.
    /// </summary>
    /// <param name="z">The complex function argument.</param>
    /// <returns>The exponential function of the spezified complex function argument.</returns>
    public static ComplexFloat Exp(ComplexFloat z)
    {
      return ComplexFloat.FromModulusArgument((float)Math.Exp(z.Re), (float)z.Im);
    }

    /// <summary>
    /// Returns the inverse of the argument z, i.e. 1/z
    /// </summary>
    /// <param name="z">The argument.</param>
    /// <returns>Inverse of z, i.e. 1/z.</returns>
    public static Complex Inverse(Complex z)
    {                               /* z=1/a */
      double s = 1.0 / Abs(z);
      return new Complex((z.Re * s) * s, -(z.Im * s) * s);
    }
    
    /// <summary>
    /// Returns the natural (base e) logarithm of the complex function argument.
    /// </summary>
    /// <param name="z">The complex function argument.</param>
    /// <returns>The natural (base e) logarithm of the complex function argument.</returns>
    public static Complex Log(Complex z)
    {
      return new Complex(LogAbs(z), Arg(z));
    }

    /// <summary>
    /// Returns the natural (base e) logarithm of the complex function argument.
    /// </summary>
    /// <param name="z">The complex function argument.</param>
    /// <returns>The natural (base e) logarithm of the complex function argument.</returns>
    public static ComplexFloat Log(ComplexFloat z)
    {
      return new ComplexFloat((float)LogAbs(z), (float)Arg(z));
    }



    /// <summary>
    /// Return log |z|.
    /// </summary>
    /// <param name="z">The complex function argument.</param>
    /// <returns>log |z|, i.e. the natural logarithm of the absolute value of z.</returns>
    public static double LogAbs(Complex z)
    {                               
      double xabs = Math.Abs(z.Re);
      double yabs = Math.Abs(z.Im);
      double max, u;

      if (xabs >= yabs)
      {
        max = xabs;
        u = yabs / xabs;
      }
      else
      {
        max = yabs;
        u = xabs / yabs;
      }

      // Handle underflow when u is close to 0 
      return Math.Log(max) + 0.5 * RMath.Log1p(u * u);
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

    ///<summary>Given two complex types return the one with the maximum norm</summary>
    public static Complex Max(Complex v1, Complex v2)
    {
      if (Norm(v1) >= Norm(v2))
        return v1;
      else
        return v2;
    }

    ///<summary>Given two complex types return the one with the maximum norm</summary>
    public static ComplexFloat Max(ComplexFloat v1, ComplexFloat v2)
    {
      if (Norm(v1) >= Norm(v2))
        return v1;
      else
        return v2;
    }

    ///<summary>Return the euclidean norm of a complex type</summary>
    public static double Norm(Complex value)
    {
      return ComplexMath.Absolute(value);
    }
    ///<summary>Return the euclidean norm of a complex type</summary>
    public static float Norm(ComplexFloat value)
    {
      return ComplexMath.Absolute(value);
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

    ///<summary>Return the polar representation of a complex type</summary>
    public static Complex Polar(Complex value)
    {
      return new Complex(ComplexMath.Absolute(value), System.Math.Atan2(value.Imag, value.Real));
    }
    ///<summary>Return the polar representation of a complex type</summary>
    public static ComplexFloat Polar(ComplexFloat value)
    {
      return new ComplexFloat(ComplexMath.Absolute(value), (float)(System.Math.Atan2(value.Imag, value.Real)));
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
      {
        return Complex.Zero;
      }
      else
      {
        double logr = LogAbs(z);
        double theta = Arg(z);
        double br = p.Re, bi = p.Im;

        double rho = Math.Exp(logr * br - bi * theta);
        double beta = theta * br + bi * logr;

        return new Complex( rho * Math.Cos (beta), rho * Math.Sin (beta));
      }
    }

    /// <summary>
    /// Calculate the power of a complex number.
    /// </summary>
    /// <param name="a">The function argument.</param>
    /// <param name="b">The exponent.</param>
    /// <returns>The power of z to the exponent b.</returns>
    static public Complex Pow( Complex a, double b ) 
    {
      Complex z;

      if (a.Re == 0 && a.Im == 0)
      {
        z = Complex.Zero;
      }
      else
      {
        double logr = LogAbs(a);
        double theta = Arg(a);
        double rho = Math.Exp(logr * b);
        double beta = theta * b;
        z = new Complex( rho * Math.Cos (beta), rho * Math.Sin (beta));
      }

      return z;
    }

    /// <summary>
    /// Calculate the power of a complex number
    /// </summary>
    /// <param name="c"></param>
    /// <param name="exponent"></param>
    /// <returns></returns>
    static public ComplexFloat  Pow( ComplexFloat c, double exponent ) 
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
    /// Returns the complex secant of the argument z, i.e. 1/Cos(z)
    /// </summary>
    /// <param name="a">The function argument.</param>
    /// <returns>Complex secant of the argument z, i.e. 1/Cos(z).</returns>
    public static Complex Sec (Complex a)
    {                       
      return Inverse(Cos(a));
    }
    
    /// <summary>
    /// This function returns the complex hyperbolic secant of the complex
    /// number a, sech(a) = 1/cosh(a).
    /// </summary>
    /// <param name="a">Function argument.</param>
    /// <returns>The complex hyperbolic secant of the complex number a.</returns>
    public static Complex Sech(Complex a)
    {                           
      return Inverse (Cosh(a));
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
    /// Returns the sine of the specified complex function argument z.
    /// </summary>
    /// <param name="z">Function argument.</param>
    /// <returns>The sine of the specified complex function argument z.</returns>
    public static ComplexFloat Sin(ComplexFloat z)
    {
      double ezi = Math.Exp(z.Im);
      double inv = 1.0 / ezi;
      return ComplexFloat.FromRealImaginary((float)(0.5 * Math.Sin(z.Re) * (inv + ezi)),(float)( -0.5 * Math.Cos(z.Re) * (inv - ezi)));
    }

    /// <summary>
    /// This function returns the complex hyperbolic sine of the complex number
    /// a, sinh(a) = (exp(a) - exp(-a))/2.
    /// </summary>
    /// <param name="a">Function argument.</param>
    /// <returns>The hyperbolic sine of the specified complex function argument a.</returns>
    public static Complex Sinh(Complex a)
    {
      double R = a.Re, I = a.Im;
      return new Complex( Math.Sinh (R) * Math.Cos (I), Math.Cosh (R) * Math.Sin (I));
    }

    /// <summary>
    /// This function returns the complex hyperbolic sine of the complex number
    /// a, sinh(a) = (exp(a) - exp(-a))/2.
    /// </summary>
    /// <param name="a">Function argument.</param>
    /// <returns>The hyperbolic sine of the specified complex function argument a.</returns>
    public static ComplexFloat Sinh(ComplexFloat a)
    {
      double R = a.Re, I = a.Im;
      return new ComplexFloat((float)(Math.Sinh(R) * Math.Cos(I)), (float)(Math.Cosh(R) * Math.Sin(I)));
    }

    /// <summary>
    /// Calculate the square root of the complex number c.
    /// </summary>
    /// <param name="c">Function argument.</param>
    /// <returns>The square root of the complex number c.</returns>
    public static ComplexFloat  Sqrt( ComplexFloat c ) 
    {
      ComplexFloat z;

      if( c.Re == 0.0 && c.Im == 0.0)
      {
        z = new ComplexFloat(0,0);
      }
      else
      {
        double x = Math.Abs(c.Re);
        double y = Math.Abs(c.Im);
        double w;

        if (x >= y)
        {
          double t = y / x;
          w = Math.Sqrt (x) * Math.Sqrt (0.5 * (1.0 + Math.Sqrt (1.0 + t * t)));
        }
        else
        {
          double t = x / y;
          w = Math.Sqrt (y) * Math.Sqrt (0.5 * (t + Math.Sqrt (1.0 + t * t)));
        }

        if (c.Re >= 0.0)
        {
          double ai = c.Im;
          z = new ComplexFloat((float)w,(float)( ai / (2.0 * w)));
        }
        else
        {
          double ai = c.Im;
          double vi = (ai >= 0) ? w : -w;
          z = new ComplexFloat((float)(ai / (2.0 * vi)), (float)vi);
        }
      }
      return z;
    }

    /// <summary>
    /// Calculate the square root of the complex number c.
    /// </summary>
    /// <param name="c">Function argument.</param>
    /// <returns>The square root of the complex number c.</returns>
    public static Complex Sqrt( Complex c ) 
    {
      Complex z;

      if( c.Re == 0.0 && c.Im == 0.0)
      {
        z = new Complex(0,0);
      }
      else
      {
        double x = Math.Abs(c.Re);
        double y = Math.Abs(c.Im);
        double w;

        if (x >= y)
        {
          double t = y / x;
          w = Math.Sqrt (x) * Math.Sqrt (0.5 * (1.0 + Math.Sqrt (1.0 + t * t)));
        }
        else
        {
          double t = x / y;
          w = Math.Sqrt (y) * Math.Sqrt (0.5 * (t + Math.Sqrt (1.0 + t * t)));
        }

        if (c.Re >= 0.0)
        {
          double ai = c.Im;
          z = new Complex(w, ai / (2.0 * w));
        }
        else
        {
          double ai = c.Im;
          double vi = (ai >= 0) ? w : -w;
          z = new Complex( ai / (2.0 * vi), vi);
        }
      }
      return z;
    }

    /// <summary>
    /// Calculate the square root of the real number x.
    /// </summary>
    /// <param name="x">The function argument.</param>
    /// <returns>The square root of the number x.</returns>
    public static Complex Sqrt( double x ) 
    {
      if (x >= 0)
      {
        return new Complex(Math.Sqrt(x), 0.0);
      }
      else
      {
        return new Complex(0.0, Math.Sqrt(-x));
      }
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
    /// Returns the tangent of the specified complex function argument z.
    /// </summary>
    /// <param name="z">Function argument.</param>
    /// <returns>The tangent of the specified complex function argument z.</returns>
    public static ComplexFloat Tan(ComplexFloat z)
    {
      double sinzr = Math.Sin(z.Re);
      double coszr = Math.Cos(z.Re);
      double ezi = Math.Exp(z.Im);
      double inv = 1.0 / ezi;
      double ediff = inv - ezi;
      double esum = inv + ezi;
      return ComplexFloat.FromRealImaginary((float)(4 * sinzr * coszr),(float)( -ediff * esum)) / (float)(square(coszr * esum) + square(sinzr * ediff));
    }



    /// <summary>
    /// This function returns the complex hyperbolic tangent of the complex number
    /// a, tanh(a) = sinh(a)/cosh(a).
    /// </summary>
    /// <param name="a">Function argument.</param>
    /// <returns>The hyperbolic tangent of the specified complex function argument z.</returns>
    public static Complex Tanh(Complex a)
    {
      double R = a.Re, I = a.Im;

      Complex z;

      if (Math.Abs(R) < 1.0) 
      {
        double D = square (Math.Cos (I)) + square(Math.Sinh (R));
      
        z = new Complex( Math.Sinh (R) * Math.Cosh (R) / D, 0.5 * Math.Sin (2 * I) / D);
      }
      else
      {
        double D = square(Math.Cos(I)) + square(Math.Sinh(R));
        double F = 1 + square(Math.Cos(I) / Math.Sinh(R));

        z = new Complex( 1.0 / (Math.Tanh (R) * F), 0.5 * Math.Sin (2 * I) / D);
      }

      return z;  
    }


    /// <summary>
    /// This function returns the complex hyperbolic tangent of the complex number
    /// a, tanh(a) = sinh(a)/cosh(a).
    /// </summary>
    /// <param name="a">Function argument.</param>
    /// <returns>The hyperbolic tangent of the specified complex function argument z.</returns>
    public static ComplexFloat Tanh(ComplexFloat a)
    {
      double R = a.Re, I = a.Im;

      ComplexFloat z;

      if (Math.Abs(R) < 1.0)
      {
        double D = square(Math.Cos(I)) + square(Math.Sinh(R));

        z = new ComplexFloat((float)(Math.Sinh(R) * Math.Cosh(R) / D),(float)( 0.5 * Math.Sin(2 * I) / D));
      }
      else
      {
        double D = square(Math.Cos(I)) + square(Math.Sinh(R));
        double F = 1 + square(Math.Cos(I) / Math.Sinh(R));

        z = new ComplexFloat((float)(1.0 / (Math.Tanh(R) * F)), (float)(0.5 * Math.Sin(2 * I) / D));
      }

      return z;
    }
    

    #endregion
  }
}
