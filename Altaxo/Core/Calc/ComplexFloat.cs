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
using System.Runtime.InteropServices;


namespace Altaxo.Calc 
{

  // Comments? Questions? Bugs? Tell Ben Houston at ben@exocortex.org
  // Version: May 4, 2002

  /// <summary>
  /// <p>A single-precision complex number representation.</p>
  /// </summary>
  [StructLayout(LayoutKind.Sequential)]
  public struct ComplexFloat : IComparable, ICloneable 
  {

    //-----------------------------------------------------------------------------------
    //-----------------------------------------------------------------------------------

    /// <summary>
    /// The real component of the complex number
    /// </summary>
    public float Re;

    /// <summary>
    /// The imaginary component of the complex number
    /// </summary>
    public float Im;

    //-----------------------------------------------------------------------------------
    //-----------------------------------------------------------------------------------


    /// <summary>
    /// The real component of the complex number
    /// </summary>
    public float Real
    {
      get
      {
        return Re;
      }
      set
      {
        Re = value;
      }
    }

    /// <summary>
    /// The imaginary component of the complex number
    /// </summary>
    public float Imag
    {
      get
      {
        return Im;
      }
      set
      {
        Im = value;
      }
    }

    //-----------------------------------------------------------------------------------
    //-----------------------------------------------------------------------------------

    /// <summary>
    /// Create a complex number from a real and an imaginary component
    /// </summary>
    /// <param name="real"></param>
    /// <param name="imaginary"></param>
    public ComplexFloat( float real, float imaginary ) 
    {
      this.Re   = (float) real;
      this.Im = (float) imaginary;
    }

    /// <summary>
    /// Create a complex number based on an existing complex number
    /// </summary>
    /// <param name="c"></param>
    public ComplexFloat( ComplexFloat c ) 
    {
      this.Re   = c.Re;
      this.Im = c.Im;
    }

    ///<summary>Created a <c>ComplexFloat</c> from the given string. The string can be in the
    ///following formats: <c>n</c>, <c>ni</c>, <c>n +/- ni</c>, <c>n,n</c>, <c>n,ni</c>,
    ///<c>(n,n)</c>, or <c>(n,ni)</c>, where n is a real number.</summary>
    ///<param name="s">The string to create the <c>ComplexFloat</c> from.</param>
    ///<exception cref="FormatException">if the n, is not a number.</exception>
    ///<exception cref="ArgumentNullException">if s, is <c>null</c>.</exception>
    public ComplexFloat(string s)
    {
      this = ComplexFloat.Parse(s);
    }

    /// <summary>
    /// Create a complex number from a real and an imaginary component
    /// </summary>
    /// <param name="real"></param>
    /// <param name="imaginary"></param>
    /// <returns></returns>
    static public ComplexFloat  FromRealImaginary( float real, float imaginary ) 
    {
      ComplexFloat c;
      c.Re    = (float) real;
      c.Im = (float) imaginary;
      return c;
    }

    /// <summary>
    /// Create a complex number from a modulus (length) and an argument (radian)
    /// </summary>
    /// <param name="modulus"></param>
    /// <param name="argument"></param>
    /// <returns></returns>
    static public ComplexFloat  FromModulusArgument( float modulus, float argument ) 
    {
      ComplexFloat c;
      c.Re    = (float)( modulus * System.Math.Cos( argument ) );
      c.Im  = (float)( modulus * System.Math.Sin( argument ) );
      return c;
    }
    
    //-----------------------------------------------------------------------------------
    //-----------------------------------------------------------------------------------

    object  ICloneable.Clone() 
    {
      return  new ComplexFloat( this );
    }
    /// <summary>
    /// Clone the complex number
    /// </summary>
    /// <returns></returns>
    public ComplexFloat Clone() 
    {
      return  new ComplexFloat( this );
    }
    
    //-----------------------------------------------------------------------------------
    //-----------------------------------------------------------------------------------

    /// <summary>
    /// The modulus (length) of the complex number
    /// </summary>
    /// <returns></returns>
    public float  GetModulus() 
    {
      float x = this.Re;
      float y = this.Im;
      return  (float) Math.Sqrt( x*x + y*y );
    }

    /// <summary>
    /// The squared modulus (length^2) of the complex number
    /// </summary>
    /// <returns></returns>
    public float  GetModulusSquared() 
    {
      float x = this.Re;
      float y = this.Im;
      return  (float) x*x + y*y;
    }

    /// <summary>
    /// The argument (radians) of the complex number
    /// </summary>
    /// <returns></returns>
    public float  GetArgument() 
    {
      return (float) Math.Atan2( this.Im, this.Re );
    }

    //-----------------------------------------------------------------------------------

    /// <summary>
    /// Get the conjugate of the complex number
    /// </summary>
    /// <returns></returns>
    public ComplexFloat GetConjugate() 
    {
      return FromRealImaginary( this.Re, -this.Im );
    }

    //-----------------------------------------------------------------------------------

    /// <summary>
    /// Scale the complex number to 1.
    /// </summary>
    public void Normalize() 
    {
      double  modulus = this.GetModulus();
      if( modulus == 0 ) 
      {
        throw new DivideByZeroException( "Can not normalize a complex number that is zero." );
      }
      this.Re = (float)( this.Re / modulus );
      this.Im = (float)( this.Im / modulus );
    }

    //-----------------------------------------------------------------------------------
    //-----------------------------------------------------------------------------------

    /// <summary>
    /// Convert to a from double precision complex number to a single precison complex number
    /// </summary>
    /// <param name="c"></param>
    /// <returns></returns>
    public static explicit operator ComplexFloat ( Complex c ) 
    {
      ComplexFloat cF;
      cF.Re = (float) c.Re;
      cF.Im = (float) c.Im;
      return cF;
    }
    
    /// <summary>
    /// Convert from a single precision real number to a complex number
    /// </summary>
    /// <param name="f"></param>
    /// <returns></returns>
    public static implicit operator ComplexFloat ( float f ) 
    {
      ComplexFloat c;
      c.Re    = (float) f;
      c.Im  = (float) 0;
      return c;
    }

    /// <summary>
    /// Convert from a single precision complex to a real number
    /// </summary>
    /// <param name="c"></param>
    /// <returns></returns>
    public static explicit operator float ( ComplexFloat c ) 
    {
      return (float) c.Re;
    }
    
    //-----------------------------------------------------------------------------------
    //-----------------------------------------------------------------------------------

    /// <summary>
    /// Are these two complex numbers equivalent?
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static bool  operator==( ComplexFloat a, ComplexFloat b ) 
    {
      return  ( a.Re == b.Re ) && ( a.Im == b.Im );
    }

    /// <summary>
    /// Are these two complex numbers different?
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static bool  operator!=( ComplexFloat a, ComplexFloat b ) 
    {
      return  ( a.Re != b.Re ) || ( a.Im != b.Im );
    }

    /// <summary>
    /// Get the hash code of the complex number
    /// </summary>
    /// <returns></returns>
    public override int   GetHashCode() 
    {
      return  ( this.Re.GetHashCode() + this.Im.GetHashCode() );
    }

    ///<summary>Check if <c>ComplexFloat</c> variable is the same as another object</summary>
    ///<param name="obj"><c>obj</c> to compare present <c>ComplexFloat</c> to.</param>
    ///<returns>Returns true if the variable is the same as the <c>ComplexFloat</c> variable</returns>
    ///<remarks>The <c>obj</c> parameter is converted into a <c>ComplexFloat</c> variable before comparing with the current <c>ComplexFloat</c>.</remarks>
    public bool Equals(Complex obj) 
    {
      return this.Re == obj.Re && this.Im == obj.Im;
    }
    
    ///<summary>Check if <c>ComplexFloat</c> variable is the same as another object</summary>
    ///<param name="obj"><c>obj</c> to compare present <c>ComplexFloat</c> to.</param>
    ///<returns>Returns true if the variable is the same as the <c>ComplexFloat</c> variable</returns>
    ///<remarks>The <c>obj</c> parameter is converted into a <c>ComplexFloat</c> variable before comparing with the current <c>ComplexFloat</c>.</remarks>
    public bool Equals(ComplexFloat obj) 
    {
      return this.Re == obj.Re && this.Im == obj.Im;
    }
    
    ///<summary>Check if <c>ComplexFloat</c> variable is the same as another object</summary>
    ///<param name="obj"><c>obj</c> to compare present <c>ComplexFloat</c> to.</param>
    ///<returns>Returns true if the variable is the same as the <c>ComplexFloat</c> variable</returns>
    ///<remarks>The <c>obj</c> parameter is converted into a <c>ComplexFloat</c> variable before comparing with the current <c>ComplexFloat</c>.</remarks>
    public override bool Equals(Object obj) 
    {
      if( obj == null )
      {
        return false;
      }
      if( obj is ComplexFloat )
      {
        ComplexFloat rhs = (ComplexFloat)obj;
        return this.Equals(rhs);
      } 
      else if(obj is Complex )
      {
        Complex rhs = (Complex)obj;
        return this.Equals(rhs);
      }
      else
      {
        return false;
      }

    }

    //-----------------------------------------------------------------------------------
    //-----------------------------------------------------------------------------------

    /// <summary>
    /// Compare to other complex numbers or real numbers
    /// </summary>
    /// <param name="o"></param>
    /// <returns></returns>
    public int  CompareTo( object o ) 
    {
      if( o == null ) 
      {
        return 1;  // null sorts before current
      }
      if( o is ComplexFloat ) 
      {
        return  this.GetModulus().CompareTo( ((ComplexFloat)o).GetModulus() );
      }
      if( o is float ) 
      {
        return  this.GetModulus().CompareTo( (float)o );
      }
      if( o is Complex ) 
      {
        return  this.GetModulus().CompareTo( ((Complex)o).GetModulus() );
      }
      if( o is double ) 
      {
        return  this.GetModulus().CompareTo( (double)o );
      }
      throw new ArgumentException();
    }

    //-----------------------------------------------------------------------------------
    //-----------------------------------------------------------------------------------

    /// <summary>
    /// This operator doesn't do much. :-)
    /// </summary>
    /// <param name="a"></param>
    /// <returns></returns>
    public static ComplexFloat operator+( ComplexFloat a ) 
    {
      return a;
    }

    /// <summary>
    /// Negate the complex number
    /// </summary>
    /// <param name="a"></param>
    /// <returns></returns>
    public static ComplexFloat operator-( ComplexFloat a ) 
    {
      a.Re  = -a.Re;
      a.Im  = -a.Im;
      return a;
    }

    /// <summary>
    /// Add a complex number to a real
    /// </summary>
    /// <param name="a"></param>
    /// <param name="f"></param>
    /// <returns></returns>
    public static ComplexFloat operator+( ComplexFloat a, float f ) 
    {
      a.Re  = (float)( a.Re + f );
      return a;
    }

    /// <summary>
    /// Add a real to a complex number
    /// </summary>
    /// <param name="f"></param>
    /// <param name="a"></param>
    /// <returns></returns>
    public static ComplexFloat operator+( float f, ComplexFloat a ) 
    {
      a.Re  = (float)( a.Re + f );
      return a;
    }

    /// <summary>
    /// Add to complex numbers
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static ComplexFloat operator+( ComplexFloat a, ComplexFloat b ) 
    {
      a.Re  = a.Re + b.Re;
      a.Im  = a.Im + b.Im;
      return a;
    }

    /// <summary>
    /// Subtract a real from a complex number
    /// </summary>
    /// <param name="a"></param>
    /// <param name="f"></param>
    /// <returns></returns>
    public static ComplexFloat operator-( ComplexFloat a, float f ) 
    {
      a.Re  = (float)( a.Re - f );
      return a;
    }

    /// <summary>
    /// Subtract a complex number from a real
    /// </summary>
    /// <param name="f"></param>
    /// <param name="a"></param>
    /// <returns></returns>
    public static ComplexFloat operator-( float f, ComplexFloat a ) 
    {
      a.Re  = (float)( f - a.Re );
      a.Im  = (float)( 0 - a.Im );
      return a;
    }

    /// <summary>
    /// Subtract two complex numbers
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static ComplexFloat operator-( ComplexFloat a, ComplexFloat b ) 
    {
      a.Re  = a.Re - b.Re;
      a.Im  = a.Im - b.Im;
      return a;
    }

    /// <summary>
    /// Multiply a complex number by a real
    /// </summary>
    /// <param name="a"></param>
    /// <param name="f"></param>
    /// <returns></returns>
    public static ComplexFloat operator*( ComplexFloat a, float f ) 
    {
      a.Re  = (float)( a.Re * f );
      a.Im  = (float)( a.Im * f );
      return a;
    }
    
    /// <summary>
    /// Multiply a real by a complex number
    /// </summary>
    /// <param name="f"></param>
    /// <param name="a"></param>
    /// <returns></returns>
    public static ComplexFloat operator*( float f, ComplexFloat a ) 
    {
      a.Re  = (float)( a.Re * f );
      a.Im  = (float)( a.Im * f );
      return a;
    }
    
    /// <summary>
    /// Multiply two complex numbers together
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static ComplexFloat operator*( ComplexFloat a, ComplexFloat b ) 
    {
      // (x + yi)(u + vi) = (xu – yv) + (xv + yu)i. 
      double  x = a.Re, y = a.Im;
      double  u = b.Re, v = b.Im;
      a.Re  = (float)( x*u - y*v );
      a.Im  = (float)( x*v + y*u );
      return a;
    }

    /// <summary>
    /// Divide a complex number by a real number
    /// </summary>
    /// <param name="a"></param>
    /// <param name="f"></param>
    /// <returns></returns>
    public static ComplexFloat operator/( ComplexFloat a, float f ) 
    {
      if( f == 0 ) 
      {
        throw new DivideByZeroException();
      }
      a.Re  = (float)( a.Re / f );
      a.Im  = (float)( a.Im / f );
      return a;
    }
    
    /// <summary>
    /// Divide a complex number by a complex number
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static ComplexFloat operator/( ComplexFloat a, ComplexFloat b ) 
    {
      double  x = a.Re, y = a.Im;
      double  u = b.Re, v = b.Im;
      double  denom = u*u + v*v;

      if( denom == 0 ) 
      {
        throw new DivideByZeroException();
      }
      a.Re  = (float)( ( x*u + y*v ) / denom );
      a.Im  = (float)( ( y*u - x*v ) / denom );
      return a;
    }

    /// <summary>Creates a <c>ComplexFloat</c> based on a string. The string can be in the
    ///following formats: <c>n</c>, <c>ni</c>, <c>n +/- ni</c>, <c>n,n</c>, <c>n,ni</c>,
    ///<c>(n,n)</c>, or <c>(n,ni)</c>, where n is a real number.</summary>
    /// <param name="s">the string to parse.</param>
    /// <returns></returns>
    public static ComplexFloat Parse(string s)
    {
      if (s == null)
      {
        throw new ArgumentNullException(s, "s cannot be null.");
      }
      s = s.Trim();
      if (s.Length == 0)
      {
        throw new FormatException();
      }

      //check if one character strings are valid
      if (s.Length == 1)
      {
        if (String.Compare(s, "i") == 0)
        {
          return new ComplexFloat(0, 1);
        }
        else
        {
          return new ComplexFloat(float.Parse(s));
        }
      }

      //strip out parens
      if (s.StartsWith("("))
      {
        if (!s.EndsWith(")"))
        {
          throw new FormatException();
        }
        else
        {
          s = s.Substring(1, s.Length - 2);
        }
      }

      string real = s;
      string imag = "0";

      //comma separated
      int index = s.IndexOf(',');
      if (index > -1)
      {
        real = s.Substring(0, index);
        imag = s.Substring(index + 1, s.Length - index - 1);
      }
      else
      {
        index = s.IndexOf('+', 1);
        if (index > -1)
        {
          real = s.Substring(0, index);
          imag = s.Substring(index + 1, s.Length - index - 1);
        }
        else
        {
          index = s.IndexOf('-', 1);
          if (index > -1)
          {
            real = s.Substring(0, index);
            imag = s.Substring(index, s.Length - index);
          }
        }
      }

      //see if we have numbers in the format xxxi
      if (real.EndsWith("i"))
      {
        if (!imag.Equals("0"))
        {
          throw new FormatException();
        }
        else
        {
          imag = real.Substring(0, real.Length - 1);
          real = "0";
        }
      }
      if (imag.EndsWith("i"))
      {
        imag = imag.Substring(0, imag.Length - 1);
      }
      //handle cases of - n, + n
      if (real.StartsWith("-"))
      {
        real = "-" + real.Substring(1, real.Length - 1).Trim();
      }
      if (imag.StartsWith("-"))
      {
        imag = "-" + imag.Substring(1, imag.Length - 1).Trim();
      }

      ComplexFloat ret;
      try
      {
        ret = new ComplexFloat(float.Parse(real.Trim()), float.Parse(imag.Trim()));
      }
      catch (Exception)
      {
        throw new FormatException();
      }
      return ret;
    }

    ///<summary>Tests whether the the complex number is not a number.</summary>
    ///<returns>True if either the real or imaginary components are NaN, false otherwise.</returns>
    public bool IsNaN()
    {
      return (float.IsNaN(this.Re) || float.IsNaN(this.Im));
    }

    ///<summary>Tests whether the the complex number is infinite.</summary>
    ///<returns>True if either the real or imaginary components are infinite, false otherwise.</returns>
    public bool IsInfinity()
    {
      return (float.IsInfinity(this.Re) || float.IsInfinity(this.Im));
    }


    ///<summary>A string representation of this <c>ComplexFloat</c>.</summary>
    ///<returns>The string representation of the value of <c>this</c> instance.</returns>
    public override string ToString()
    {
      return ToString(null, null);
    }

    ///<summary>A string representation of this <c>ComplexFloat</c>.</summary>
    ///<param name="format">A format specification.</param>
    ///<returns>The string representation of the value of <c>this</c> instance as specified by format.</returns>
    public string ToString(string format)
    {
      return ToString(format, null);
    }

    ///<summary>A string representation of this <c>ComplexFloat</c>.</summary>
    ///<param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
    ///<returns>The string representation of the value of <c>this</c> instance as specified by provider.</returns>
    public string ToString(IFormatProvider formatProvider)
    {
      return ToString(null, formatProvider);
    }

    ///<summary>A string representation of this <c>ComplexFloat</c>.</summary>
    ///<param name="format">A format specification.</param>
    ///<param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
    ///<returns>The string representation of the value of <c>this</c> instance as specified by format and provider.</returns>
    ///<exception cref="FormatException">if the n, is not a number.</exception>
    ///<exception cref="ArgumentNullException">if s, is <c>null</c>.</exception>    
    public string ToString(string format, IFormatProvider formatProvider)
    {
      if (IsNaN())
      {
        return "NaN";
      }
      if (IsInfinity())
      {
        return "IsInfinity";
      }

      System.Text.StringBuilder ret = new System.Text.StringBuilder();

      ret.Append(Re.ToString(format, formatProvider));
      if (Im < 0)
      {
        ret.Append(" ");
      }
      else
      {
        ret.Append(" + ");
      }
      ret.Append(Im.ToString(format, formatProvider)).Append("i");

      return ret.ToString();
    }

    //-----------------------------------------------------------------------------------
    //-----------------------------------------------------------------------------------

    /// <summary>
    /// Determine whether two complex numbers are almost (i.e. within the tolerance) equivalent.
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="tolerance"></param>
    /// <returns></returns>
    static public bool IsEqual( ComplexFloat a, ComplexFloat b, float tolerance ) 
    {
      return
        ( Math.Abs( a.Re - b.Re ) < tolerance ) &&
        ( Math.Abs( a.Im - b.Im ) < tolerance );

    }
    
    //----------------------------------------------------------------------------------
    //----------------------------------------------------------------------------------

    

    /// <summary>
    /// Represents zero
    /// </summary>
    static public ComplexFloat Zero
    {
      get { return new ComplexFloat(0, 0); }
    }

    /// <summary>
    /// Represents one
    /// </summary>
    static public ComplexFloat One
    {
      get { return new ComplexFloat(1, 0); }
    }

    /// <summary>
    /// Represents the result of sqrt( -1 )
    /// </summary>
    static public ComplexFloat  I 
    {
      get { return  new ComplexFloat( 0, 1 ); }
    }

    /// <summary>
    /// Represents the largest possible value of ComplexFloat.
    /// </summary>
    static public ComplexFloat  MaxValue 
    {
      get { return  new ComplexFloat( float.MaxValue, float.MaxValue ); }
    }

    /// <summary>
    /// Represents the smallest possible value of ComplexFloat.
    /// </summary>
    static public ComplexFloat  MinValue 
    {
      get { return  new ComplexFloat( float.MinValue, float.MinValue ); }
    }


    //----------------------------------------------------------------------------------
    //----------------------------------------------------------------------------------
  }

}
