/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002 Dr. Dirk Lellinger
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

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

// this SourceCode was adopted from the source below
// Comments? Questions? Bugs? Tell Ben Houston at ben@exocortex.org
// Version: March 22, 2002



namespace Altaxo.Calc
{


    /// <summary>
    /// <p>A single-precision complex number representation.</p>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Complex : IComparable, ICloneable {

        //-----------------------------------------------------------------------------------
        //-----------------------------------------------------------------------------------

        /// <summary>
        /// The real component of the complex number
        /// </summary>
        public double Re;

        /// <summary>
        /// The imaginary component of the complex number
        /// </summary>
        public double Im;

        //-----------------------------------------------------------------------------------
        //-----------------------------------------------------------------------------------

        /// <summary>
        /// Create a complex number from a real and an imaginary component
        /// </summary>
        /// <param name="real"></param>
        /// <param name="imaginary"></param>
        public Complex( double real, double imaginary ) {
            this.Re        = (double) real;
            this.Im    = (double) imaginary;
        }

        /// <summary>
        /// Create a complex number based on an existing complex number
        /// </summary>
        /// <param name="c"></param>
        public Complex( Complex c ) {
            this.Re        = c.Re;
            this.Im    = c.Im;
        }

        /// <summary>
        /// Create a complex number from a real and an imaginary component
        /// </summary>
        /// <param name="real"></param>
        /// <param name="imaginary"></param>
        /// <returns></returns>
        static public Complex    FromRealImaginary( double real, double imaginary ) {
            Complex c;
            c.Re        = (double) real;
            c.Im = (double) imaginary;
            return c;
        }

        /// <summary>
        /// Create a complex number from a modulus (length) and an argument (radian)
        /// </summary>
        /// <param name="modulus"></param>
        /// <param name="argument"></param>
        /// <returns></returns>
        static public Complex    FromModulusArgument( double modulus, double argument ) {
            Complex c;
            c.Re        = (double)( modulus * System.Math.Cos( argument ) );
            c.Im    = (double)( modulus * System.Math.Sin( argument ) );
            return c;
        }
        
        //-----------------------------------------------------------------------------------
        //-----------------------------------------------------------------------------------

        object    ICloneable.Clone() {
            return    new Complex( this );
        }
        /// <summary>
        /// Clone the complex number
        /// </summary>
        /// <returns></returns>
        public Complex    Clone() {
            return    new Complex( this );
        }
        
        //-----------------------------------------------------------------------------------
        //-----------------------------------------------------------------------------------

        /// <summary>
        /// The modulus (length) of the complex number
        /// </summary>
        /// <returns></returns>
        public double    GetModulus() {
            double    x    = this.Re;
            double    y    = this.Im;
            return    (double) System.Math.Sqrt( x*x + y*y );
        }

        /// <summary>
        /// The squared modulus (length^2) of the complex number
        /// </summary>
        /// <returns></returns>
        public double    GetModulusSquared() {
            double    x    = this.Re;
            double    y    = this.Im;
            return    (double) x*x + y*y;
        }

        /// <summary>
        /// The argument (radians) of the complex number
        /// </summary>
        /// <returns></returns>
        public double    GetArgument() {
            return (double) System.Math.Atan2( this.Im, this.Re );
        }

        //-----------------------------------------------------------------------------------

        /// <summary>
        /// Get the conjugate of the complex number
        /// </summary>
        /// <returns></returns>
        public Complex GetConjugate() {
            return FromRealImaginary( this.Re, -this.Im );
        }

        //-----------------------------------------------------------------------------------

        /// <summary>
        /// Scale the complex number to 1.
        /// </summary>
        public void Normalize() {
            double    modulus = this.GetModulus();
            if( modulus == 0 ) {
                throw new DivideByZeroException( "Can not normalize a complex number that is zero." );
            }
            this.Re    = (double)( this.Re / modulus );
            this.Im    = (double)( this.Im / modulus );
        }

        //-----------------------------------------------------------------------------------
        //-----------------------------------------------------------------------------------

        
        /// <summary>
        /// Convert from a single precision real number to a complex number
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public static explicit operator Complex ( double f ) {
            Complex c;
            c.Re        = (double) f;
            c.Im    = (double) 0;
            return c;
        }

        /// <summary>
        /// Convert from a single precision complex to a real number
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static explicit operator double ( Complex c ) {
            return (double) c.Re;
        }
        
        //-----------------------------------------------------------------------------------
        //-----------------------------------------------------------------------------------

        /// <summary>
        /// Are these two complex numbers equivalent?
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool    operator==( Complex a, Complex b ) {
            return    ( a.Re == b.Re ) && ( a.Im == b.Im );
        }

        /// <summary>
        /// Are these two complex numbers different?
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool    operator!=( Complex a, Complex b ) {
            return    ( a.Re != b.Re ) || ( a.Im != b.Im );
        }

        /// <summary>
        /// Get the hash code of the complex number
        /// </summary>
        /// <returns></returns>
        public override int        GetHashCode() {
            return    ( this.Re.GetHashCode() ^ this.Im.GetHashCode() );
        }

        /// <summary>
        /// Is this complex number equivalent to another object?
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public override bool    Equals( object o ) {
            if( o is Complex ) {
                Complex c = (Complex) o;
                return   ( this == c );
            }
            return    false;
        }

        //-----------------------------------------------------------------------------------
        //-----------------------------------------------------------------------------------

        /// <summary>
        /// Compare to other complex numbers or real numbers
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public int    CompareTo( object o ) {
            if( o is Complex ) {
                return    this.GetModulus().CompareTo( ((Complex)o).GetModulus() );
            }
            if( o is double ) {
                return    this.GetModulus().CompareTo( (double)o );
            }
            if( o is Complex ) {
                return    this.GetModulus().CompareTo( ((Complex)o).GetModulus() );
            }
            if( o is double ) {
                return    this.GetModulus().CompareTo( (double)o );
            }
            return    0;
        }

        //-----------------------------------------------------------------------------------
        //-----------------------------------------------------------------------------------

        /// <summary>
        /// This operator doesn't do much. :-)
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static Complex operator+( Complex a ) {
            return a;
        }

        /// <summary>
        /// Negate the complex number
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static Complex operator-( Complex a ) {
            a.Re    = -a.Re;
            a.Im    = -a.Im;
            return a;
        }

        /// <summary>
        /// Add a complex number to a real
        /// </summary>
        /// <param name="a"></param>
        /// <param name="f"></param>
        /// <returns></returns>
        public static Complex operator+( Complex a, double f ) {
            a.Re    = (double)( a.Re + f );
            return a;
        }

        /// <summary>
        /// Add a real to a complex number
        /// </summary>
        /// <param name="f"></param>
        /// <param name="a"></param>
        /// <returns></returns>
        public static Complex operator+( double f, Complex a ) {
            a.Re    = (double)( a.Re + f );
            return a;
        }

        /// <summary>
        /// Add to complex numbers
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Complex operator+( Complex a, Complex b ) {
            a.Re    = a.Re + b.Re;
            a.Im    = a.Im + b.Im;
            return a;
        }

        /// <summary>
        /// Subtract a real from a complex number
        /// </summary>
        /// <param name="a"></param>
        /// <param name="f"></param>
        /// <returns></returns>
        public static Complex operator-( Complex a, double f ) {
            a.Re    = (double)( a.Re - f );
            return a;
        }

        /// <summary>
        /// Subtract a complex number from a real
        /// </summary>
        /// <param name="f"></param>
        /// <param name="a"></param>
        /// <returns></returns>
        public static Complex operator-( double f, Complex a ) {
            a.Re    = (double)( a.Re - f );
            return a;
        }

        /// <summary>
        /// Subtract two complex numbers
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Complex operator-( Complex a, Complex b ) {
            a.Re    = a.Re - b.Re;
            a.Im    = a.Im - b.Im;
            return a;
        }

        /// <summary>
        /// Multiply a complex number by a real
        /// </summary>
        /// <param name="a"></param>
        /// <param name="f"></param>
        /// <returns></returns>
        public static Complex operator*( Complex a, double f ) {
            a.Re    = (double)( a.Re * f );
            a.Im    = (double)( a.Im * f );
            return a;
        }
        
        /// <summary>
        /// Multiply a real by a complex number
        /// </summary>
        /// <param name="f"></param>
        /// <param name="a"></param>
        /// <returns></returns>
        public static Complex operator*( double f, Complex a ) {
            a.Re    = (double)( a.Re * f );
            a.Im    = (double)( a.Im * f );
            return a;
        }
        
        /// <summary>
        /// Multiply two complex numbers together
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Complex operator*( Complex a, Complex b ) {
            // (x + yi)(u + vi) = (xu – yv) + (xv + yu)i. 
            double    x = a.Re, y = a.Im;
            double    u = b.Re, v = b.Im;
            a.Re    = (double)( x*u - y*v );
            a.Im    = (double)( x*v + y*u );
            return a;
        }

        /// <summary>
        /// Divide a complex number by a real number
        /// </summary>
        /// <param name="a"></param>
        /// <param name="f"></param>
        /// <returns></returns>
        public static Complex operator/( Complex a, double f ) {
            if( f == 0 ) {
                throw new DivideByZeroException();
            }
            a.Re    = (double)( a.Re / f );
            a.Im    = (double)( a.Im / f );
            return a;
        }
        
        /// <summary>
        /// Divide a complex number by a complex number
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Complex operator/( Complex a, Complex b ) {
            double    x = a.Re,    y = a.Im;
            double    u = b.Re,    v = b.Im;
            double    denom = u*u + v*v;

            if( denom == 0 ) {
                throw new DivideByZeroException();
            }
            a.Re    = (double)( ( x*u + y*v ) / denom );
            a.Im    = (double)( ( y*u - x*v ) / denom );
            return a;
        }

        /// <summary>
        /// Parse a complex representation in this fashion: "( %f, %f )"
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        static public Complex Parse( string s ) {
            throw new NotImplementedException( "Complex Complex.Parse( string s ) is not implemented." );
        }
        
        /// <summary>
        /// Get the string representation
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            return    String.Format( "( {0}, {1}i )", this.Re, this.Im );
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
        static public bool IsEqual( Complex a, Complex b, double tolerance ) {
            return
                ( System.Math.Abs( a.Re - b.Re ) < tolerance ) &&
                ( System.Math.Abs( a.Im - b.Im ) < tolerance );

        }
        
        //----------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------

        /// <summary>
        /// Represents zero
        /// </summary>
        static public Complex    Zero {
            get    {    return    Complex.FromRealImaginary( 0, 0 );    }
        }

        /// <summary>
        /// Represents the result of sqrt( -1 )
        /// </summary>
        static public Complex    I {
            get {    return    Complex.FromRealImaginary( 0, 1 );    }
        }

        /// <summary>
        /// Represents the largest possible value of Complex.
        /// </summary>
        static public Complex    MaxValue {
            get {    return    Complex.FromRealImaginary( double.MaxValue, double.MaxValue );    }
        }

        /// <summary>
        /// Represents the smallest possible value of Complex.
        /// </summary>
        static public Complex    MinValue {
            get {    return    Complex.FromRealImaginary( double.MinValue, double.MinValue );    }
        }


        //----------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------
    }

}

