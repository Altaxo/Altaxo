#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2004 Dr. Dirk Lellinger
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


// The following code was translated using Matpack sources (http://www.matpack.de) (Author B.Gammel)
// Original MatPack-1.7.3\Source\hypot3f.cc
//					MatPack-1.7.3\Source\hypot3d.cc
//          MatPack-1.7.3\Source\dlnrel.cc
//          MatPack-1.7.3\Source\clnrel.cc
//          MatPack-1.7.3\Source\dcbrt.cc



using System;



namespace Altaxo.Calc
{
  /// <summary>
  /// Basic functions.
  /// </summary>
  public class BasicFunctions
  {
    #region Common Constants

    /// <summary>
    /// Represents the smallest number where 1+DBL_EPSILON is not equal to 1.
    /// </summary>
    public const double DBL_EPSILON = 2.2204460492503131e-016;
    /// <summary>
    /// The smallest positive double number.
    /// </summary>
    public const double DBL_MIN     = double.Epsilon;
    /// <summary>
    /// The biggest positive double number.
    /// </summary>
    public const double DBL_MAX     = double.MaxValue;

    #endregion

    #region Helper functions

    /// <summary>
    /// Returns -1 if argument negative, 0 if argument zero, or 1 if argument is positive.
    /// </summary>
    /// <param name="x">The number whose sign is returned.</param>
    /// <returns>-1 if the argument is negative, 0 if the argument is zero, or 1 if argument is positive.</returns>
    public static int sign (double x)
    {
      return (x > 0) ? 1 : (x < 0) ? -1 : 0;
    }

    /// <summary>
    /// Return first number with sign of second number
    /// </summary>
    /// <param name="x">The first number.</param>
    /// <param name="y">The second number whose sign is used.</param>
    /// <returns>The first number x with the sign of the second argument y.</returns>
    public static double CopySign (double x, double y)
    {
      return (y < 0) ? ((x < 0) ? x : -x) : ((x > 0) ? x : -x);
    }


    /// <summary>
    /// Round to nearest integer.
    /// </summary>
    /// <param name="d">The argument.</param>
    /// <returns>The nearest integer of the argument d.</returns>
    public static int	Nint (double d)
    {
      return (d>0) ? (int)(d+0.5) : -(int)(-d+0.5);
    }


    #endregion


    #region hypot2d

    public static double hypot (double x, double y)
      //
      // The standard hypot() function for two arguments taking care
      // of overflows and zerodivides. 
      //
      // Version for double arguments.
      //
    {
      double f,h;
      double ax=Math.Abs(x), ay=Math.Abs(y);
      if (ax > ay) 
      {
        f = ay/ax; h = ax;
      }
      else 
      {
        f = ax/ay; h = ay;
      }
      return h*Math.Sqrt(1+f*f);
    }

    #endregion

    #region hypot3f

    //-----------------------------------------------------------------------------//

    public static float hypot (float x, float y, float z)
      //
      // The standard hypot() function for three arguments taking care
      // of overflows and zerodivides. 
      //
      // Version for float arguments.
      //
    {
      float f,g,h;
      float ax = Math.Abs(x), ay = Math.Abs(y), az = Math.Abs(z); 
      if (ax > ay) 
        if (ax > az) 
        {
          f = ay/ax; g = az/ax; h = ax;
        } 
        else 
        {
          f = ax/az; g = ay/az; h = az;
        }
      else
        if (ay > az) 
      {
        f = ax/ay; g = az/ay; h = ay;
      } 
      else if (az != 0)
      {
        f = ax/az; g = ay/az; h = az;
      } 
      else 
        return 0;
      return (float)(h*Math.Sqrt(1+f*f+g*g));
    }

    #endregion

    #region hypot3d

    public static double hypot (double x, double y, double z)
      //
      // The standard hypot() function for three arguments taking care
      // of overflows and zerodivides. 
      //
      // Version for double arguments.
      //
    {
      double f,g,h;
      double ax=Math.Abs(x), ay=Math.Abs(y), az=Math.Abs(z); // use standard fabs()
      if (ax > ay) 
        if (ax > az) 
        {
          f = ay/ax; g = az/ax; h = ax;
        } 
        else 
        {
          f = ax/az; g = ay/az; h = az;
        }
      else
        if (ay > az) 
      {
        f = ax/ay; g = az/ay; h = ay;
      } 
      else if (az != 0)
      {
        f = ax/az; g = ay/az; h = az;
      } 
      else 
        return 0;
      return h*Math.Sqrt(1+f*f+g*g);
    }

    #endregion
		


    #region LogRel

    static readonly double[] alnrcs_LogRel = 
  {
    0.10378693562743769800686267719098e+1,
    -0.13364301504908918098766041553133e+0,
    0.19408249135520563357926199374750e-1,
    -0.30107551127535777690376537776592e-2,
    0.48694614797154850090456366509137e-3,
    -0.81054881893175356066809943008622e-4,
    0.13778847799559524782938251496059e-4,
    -0.23802210894358970251369992914935e-5,
    0.41640416213865183476391859901989e-6,
    -0.73595828378075994984266837031998e-7,
    0.13117611876241674949152294345011e-7,
    -0.23546709317742425136696092330175e-8,
    0.42522773276034997775638052962567e-9,
    -0.77190894134840796826108107493300e-10,
    0.14075746481359069909215356472191e-10,
    -0.25769072058024680627537078627584e-11,
    0.47342406666294421849154395005938e-12,
    -0.87249012674742641745301263292675e-13,
    0.16124614902740551465739833119115e-13,
    -0.29875652015665773006710792416815e-14,
    0.55480701209082887983041321697279e-15,
    -0.10324619158271569595141333961932e-15,
    0.19250239203049851177878503244868e-16,
    -0.35955073465265150011189707844266e-17,
    0.67264542537876857892194574226773e-18,
    -0.12602624168735219252082425637546e-18,
    0.23644884408606210044916158955519e-19,
    -0.44419377050807936898878389179733e-20,
    0.83546594464034259016241293994666e-21,
    -0.15731559416479562574899253521066e-21,
    0.29653128740247422686154369706666e-22,
    -0.55949583481815947292156013226666e-23,
    0.10566354268835681048187284138666e-23,
    -0.19972483680670204548314999466666e-24,
    0.37782977818839361421049855999999e-25,
    -0.71531586889081740345038165333333e-26,
    0.13552488463674213646502024533333e-26,
    -0.25694673048487567430079829333333e-27,
    0.48747756066216949076459519999999e-28,
    -0.92542112530849715321132373333333e-29,
    0.17578597841760239233269760000000e-29,
    -0.33410026677731010351377066666666e-30,
    0.63533936180236187354180266666666e-31
  };

    static int nlnrel_LogRel = 0;
    static double xmin_LogRel = 0.0;

    /// <summary>
    /// LogRel(z) = log(1+z) with relative error accuracy near z = 0.
    /// </summary>
    /// <param name="x"></param>
    /// <returns></returns>
    /// <remarks>
    /// June 1977 edition.   W. Fullerton, c3, Los Alamos Scientific Lab.
    ///
    /// series for alnr       on the interval -3.75000e-01 to  3.75000e-01
    ///                                        with weighted error   6.35e-32
    ///                                         log weighted error  31.20
    ///                               significant figures required  30.93
    ///                                    decimal places required  32.01
    /// </remarks>
    public static double LogRel (double x)
    {

      if (nlnrel_LogRel == 0) 
      {
        nlnrel_LogRel = Series.initds(alnrcs_LogRel, 43, 0.1*0.5 * DBL_EPSILON);
        xmin_LogRel = -1.0 + Math.Sqrt(DBL_EPSILON);
      }
  
      if (x <= -1.0) 
        throw new ArgumentException("x <= -1");

      if (x < xmin_LogRel) 
      {
        System.Diagnostics.Trace.WriteLine("Warning (LogRel function): answer less than half precision because x too near -1");
      }

      if (Math.Abs(x) <= 0.375) 
        return x * (1.0 - x * Series.dcsevl(x/0.375,alnrcs_LogRel,nlnrel_LogRel));
      else 
        return Math.Log(1.0 + x);
    }



    #endregion

    #region LogRel (complex)

    /// <summary>
    /// LogRel(z) = log(1+z) with relative error accuracy near z = 0.
    /// </summary>
    /// <param name="z">The complex argument z.</param>
    /// <returns>Log(1+z) with relative error accuracy near z=0.</returns>
    /// <remarks>
    /// April 1977 version.  W. Fullerton, c3, Los Alamos Scientific Lab.
    ///
    /// let   rho = abs(z)  and
    ///       r**2 = abs(1+z)**2 = (1+x)**2 + y**2 = 1 + 2*x + rho**2 .
    /// now if rho is small we may evaluate LogRel(z) accurately by
    ///       log(1+z) = complex (log(r), arg(1+z))
    ///                = complex (0.5*log(r**2), arg(1+z))
    ///                = complex (0.5*LogRel(2*x+rho**2), arg(1+z))
    /// </remarks>
    public static Complex LogRel(Complex z)
    {
      if (ComplexMath.Abs(1.0 + z) < Math.Sqrt(DBL_EPSILON))
        System.Diagnostics.Trace.WriteLine("Warning (LogRel): answer less than half precision because z too near -1");

      double rho = ComplexMath.Abs(z);
      if (rho > 0.375)
        return ComplexMath.Log(1.0 + z);
		
      return new Complex(0.5*LogRel(2.0*z.Re+rho*rho), ComplexMath.Arg(1.0+z));
    }

    #endregion

  }
}
