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
  /// <p>A set of array utilities for complex number arrays</p>
  /// </summary>
  public class ComplexArray 
  {

    //---------------------------------------------------------------------------------------------

    private ComplexArray() 
    {
    }

    //---------------------------------------------------------------------------------------------

    /// <summary>
    /// Clamp length (modulus) of the elements in the complex array
    /// </summary>
    /// <param name="array"></param>
    /// <param name="fMinimum"></param>
    /// <param name="fMaximum"></param>
    static public void ClampLength( Complex[] array, double fMinimum, double fMaximum ) 
    {
      for( int i = 0; i < array.Length; i ++ ) 
      {
        array[i] = Complex.FromModulusArgument( Math.Max( fMinimum, Math.Min( fMaximum, array[i].GetModulus() ) ), array[i].GetArgument() );
      }
    }

    /// <summary>
    /// Clamp elements in the complex array to range [minimum,maximum]
    /// </summary>
    /// <param name="array"></param>
    /// <param name="minimum"></param>
    /// <param name="maximum"></param>
    static public void Clamp( Complex[] array, Complex minimum, Complex maximum ) 
    {
      for( int i = 0; i < array.Length; i ++ ) 
      {
        array[i].Re = Math.Min( Math.Max( array[ i ].Re, minimum.Re ), maximum.Re );
        array[i].Im = Math.Min( Math.Max( array[ i ].Re, minimum.Im ), maximum.Im );
      }
    }

    /// <summary>
    /// Clamp elements in the complex array to real unit range (i.e. [0,1])
    /// </summary>
    /// <param name="array"></param>
    static public void ClampToRealUnit( Complex[] array ) 
    {
      for( int i = 0; i < array.Length; i ++ ) 
      {
        array[i].Re =  Math.Min( Math.Max( array[i].Re, 0 ), 1 );
        array[i].Im = 0;
      }
    }
    
    //---------------------------------------------------------------------------------------------

    static private bool     _workspaceFLocked = false;
    static private ComplexFloat[] _workspaceF     = new ComplexFloat[ 0 ];

    static private void   LockWorkspaceF( int length, ref ComplexFloat[] workspace ) 
    {
      Debug.Assert( _workspaceFLocked == false );
      _workspaceFLocked = true;
      if( length >= _workspaceF.Length ) 
      {
        _workspaceF = new ComplexFloat[ length ];
      }
      workspace = _workspaceF;
    }
    static private void   UnlockWorkspaceF( ref ComplexFloat[] workspace ) 
    {
      Debug.Assert( _workspaceF == workspace );
      Debug.Assert( _workspaceFLocked == true );
      _workspaceFLocked = false;
      workspace = null;
    }

    //---------------------------------------------------------------------------------------------

    /// <summary>
    /// Shift (offset) the elements in the array
    /// </summary>
    /// <param name="array"></param>
    /// <param name="offset"></param>
    static public void Shift( Complex[] array, int offset ) 
    {
      Debug.Assert( array != null );
      Debug.Assert( offset >= 0 );
      Debug.Assert( offset < array.Length );

      if( offset == 0 ) 
      {
        return;
      }

      int     length  = array.Length;
      Complex[] temp  = new Complex[ length ];

      for( int i = 0; i < length; i ++ ) 
      {
        temp[ ( i + offset ) % length ] = array[ i ];
      }
      for( int i = 0; i < length; i ++ ) 
      {
        array[ i ] = temp[ i ];
      }
    }

    /// <summary>
    /// Shift (offset) the elements in the array
    /// </summary>
    /// <param name="array"></param>
    /// <param name="offset"></param>
    static public void Shift( ComplexFloat[] array, int offset ) 
    {
      Debug.Assert( array != null );
      Debug.Assert( offset >= 0 );
      Debug.Assert( offset < array.Length );

      if( offset == 0 ) 
      {
        return;
      }

      int     length    = array.Length;
      ComplexFloat[]  workspace = null;
      ComplexArray.LockWorkspaceF( length, ref workspace );

      for( int i = 0; i < length; i ++ ) 
      {
        workspace[ ( i + offset ) % length ] = array[ i ];
      }
      for( int i = 0; i < length; i ++ ) 
      {
        array[ i ] = workspace[ i ];
      }

      ComplexArray.UnlockWorkspaceF( ref workspace );
    }

    //---------------------------------------------------------------------------------------------

    /// <summary>
    /// Get the range of element lengths
    /// </summary>
    /// <param name="array"></param>
    /// <param name="minimum"></param>
    /// <param name="maximum"></param>
    static public void GetLengthRange( Complex[] array, ref double minimum, ref double maximum ) 
    {
      minimum = +double.MaxValue;
      maximum = -double.MaxValue;
      for( int i = 0; i < array.Length; i ++ ) 
      {
        double temp = array[i].GetModulus();
        minimum = Math.Min( temp, minimum );
        maximum = Math.Max( temp, maximum );
      }
    }
    /// <summary>
    /// Get the range of element lengths
    /// </summary>
    /// <param name="array"></param>
    /// <param name="minimum"></param>
    /// <param name="maximum"></param>
    static public void GetLengthRange( ComplexFloat[] array, ref float minimum, ref float maximum ) 
    {
      minimum = +float.MaxValue;
      maximum = -float.MaxValue;
      for( int i = 0; i < array.Length; i ++ ) 
      {
        float temp = array[i].GetModulus();
        minimum = Math.Min( temp, minimum );
        maximum = Math.Max( temp, maximum );
      }
    }

    // // <summary>
    // // Conver the complex array to a double array
    // // </summary>
    // // <param name="array"></param>
    // // <param name="style"></param>
    // // <returns></returns>
    /* static public double[] ConvertToDoubleArray( Complex[] array, ConversionStyle style ) {
      double[] newArray = new double[ array.Length ];
      switch( style ) {
      case ConversionStyle.Length:
        for( int i = 0; i < array.Length; i ++ ) {
          newArray[i] = (double) array[i].GetModulus();
        }
        break;
      case ConversionStyle.Real:
        for( int i = 0; i < array.Length; i ++ ) {
          newArray[i] = (double) array[i].Re;
        }
        break;
      case ConversionStyle.Imaginary:
        for( int i = 0; i < array.Length; i ++ ) {
          newArray[i] = (double) array[i].Im;
        }
        break;
      default:
        Debug.Assert( false );
        break;
      }
      return  newArray;
    }  */

    //---------------------------------------------------------------------------------------------

    /// <summary>
    /// Determine whether the elements in the two arrays are the same
    /// </summary>
    /// <param name="array1"></param>
    /// <param name="array2"></param>
    /// <param name="tolerance"></param>
    /// <returns></returns>
    static public bool    IsEqual( Complex[] array1, Complex[] array2, double tolerance ) 
    {
      if ( array1.Length != array2.Length ) 
      {
        return false;
      }
      for( int i = 0; i < array1.Length; i ++ ) 
      {
        if( Complex.IsEqual( array1[i], array2[i], tolerance ) == false ) 
        {
          return false;
        }
      }
      return true;
    }

    /// <summary>
    ///  Determine whether the elements in the two arrays are the same
    /// </summary>
    /// <param name="array1"></param>
    /// <param name="array2"></param>
    /// <param name="tolerance"></param>
    /// <returns></returns>
    static public bool    IsEqual( ComplexFloat[] array1, ComplexFloat[] array2, float tolerance ) 
    {
      if ( array1.Length != array2.Length ) 
      {
        return false;
      }
      for( int i = 0; i < array1.Length; i ++ ) 
      {
        if( ComplexFloat.IsEqual( array1[i], array2[i], tolerance ) == false ) 
        {
          return false;
        }
      }
      return true;
    }

    //---------------------------------------------------------------------------------------------
    
    /// <summary>
    /// Add a specific value to each element in the array
    /// </summary>
    /// <param name="array"></param>
    /// <param name="offset"></param>
    static public void Offset( Complex[] array, double offset ) 
    {
      int length = array.Length;
      for( int i = 0; i < length; i ++ ) 
      {
        array[i].Re += offset;
      }
    }

    /// <summary>
    /// Add a specific value to each element in the array
    /// </summary>
    /// <param name="array"></param>
    /// <param name="offset"></param>
    static public void Offset( Complex[] array, Complex offset ) 
    {
      int length = array.Length;
      for( int i = 0; i < length; i ++ ) 
      {
        array[i] += offset;
      }
    }

    /// <summary>
    /// Add a specific value to each element in the array
    /// </summary>
    /// <param name="array"></param>
    /// <param name="offset"></param>
    static public void Offset( ComplexFloat[] array, float offset ) 
    {
      int length = array.Length;
      for( int i = 0; i < length; i ++ ) 
      {
        array[i].Re += offset;
      }
    }

    /// <summary>
    /// Add a specific value to each element in the array
    /// </summary>
    /// <param name="array"></param>
    /// <param name="offset"></param>
    static public void Offset( ComplexFloat[] array, ComplexFloat offset ) 
    {
      int length = array.Length;
      for( int i = 0; i < length; i ++ ) 
      {
        array[i] += offset;
      }
    }

    //---------------------------------------------------------------------------------------------
    
    /// <summary>
    /// Multiply each element in the array by a specific value
    /// </summary>
    /// <param name="array"></param>
    /// <param name="scale"></param>
    static public void Scale( Complex[] array, double scale ) 
    {
      Debug.Assert( array != null );

      int length = array.Length;
      for( int i = 0; i < length; i ++ ) 
      {
        array[i] *= scale;
      }
    }
    /// <summary>
    ///  Multiply each element in the array by a specific value
    /// </summary>
    /// <param name="array"></param>
    /// <param name="scale"></param>
    /// <param name="start"></param>
    /// <param name="length"></param>
    static public void Scale( Complex[] array, double scale, int start, int length ) 
    {
      Debug.Assert( array != null );
      Debug.Assert( start >= 0 );
      Debug.Assert( length >= 0 );
      Debug.Assert( ( start + length ) < array.Length );

      for( int i = 0; i < length; i ++ ) 
      {
        array[i + start] *= scale;
      }
    }

    /// <summary>
    /// Multiply each element in the array by a specific value
    /// </summary>
    /// <param name="array"></param>
    /// <param name="scale"></param>
    static public void Scale( Complex[] array, Complex scale ) 
    {
      Debug.Assert( array != null );

      int length = array.Length;
      for( int i = 0; i < length; i ++ ) 
      {
        array[i] *= scale;
      }
    }
    /// <summary>
    /// Multiply each element in the array by a specific value 
    /// </summary>
    /// <param name="array"></param>
    /// <param name="scale"></param>
    /// <param name="start"></param>
    /// <param name="length"></param>
    static public void Scale( Complex[] array, Complex scale, int start, int length ) 
    {
      Debug.Assert( array != null );
      Debug.Assert( start >= 0 );
      Debug.Assert( length >= 0 );
      Debug.Assert( ( start + length ) < array.Length );

      for( int i = 0; i < length; i ++ ) 
      {
        array[i + start] *= scale;
      }
    }

    /// <summary>
    /// Multiply each element in the array by a specific value
    /// </summary>
    /// <param name="array"></param>
    /// <param name="scale"></param>
    static public void Scale( ComplexFloat[] array, float scale ) 
    {
      Debug.Assert( array != null );

      int length = array.Length;
      for( int i = 0; i < length; i ++ ) 
      {
        array[i] *= scale;
      }
    }
    /// <summary>
    /// Multiply each element in the array by a specific value 
    /// </summary>
    /// <param name="array"></param>
    /// <param name="scale"></param>
    /// <param name="start"></param>
    /// <param name="length"></param>
    static public void Scale( ComplexFloat[] array, float scale, int start, int length ) 
    {
      Debug.Assert( array != null );
      Debug.Assert( start >= 0 );
      Debug.Assert( length >= 0 );
      Debug.Assert( ( start + length ) < array.Length );

      for( int i = 0; i < length; i ++ ) 
      {
        array[i + start] *= scale;
      }
    }

    /// <summary>
    /// Multiply each element in the array by a specific value
    /// </summary>
    /// <param name="array"></param>
    /// <param name="scale"></param>
    static public void Scale( ComplexFloat[] array, ComplexFloat scale ) 
    {
      Debug.Assert( array != null );

      int length = array.Length;
      for( int i = 0; i < length; i ++ ) 
      {
        array[i] *= scale;
      }
    }
    /// <summary>
    /// Multiply each element in the array by a specific value 
    /// </summary>
    /// <param name="array"></param>
    /// <param name="scale"></param>
    /// <param name="start"></param>
    /// <param name="length"></param>
    static public void Scale( ComplexFloat[] array, ComplexFloat scale, int start, int length ) 
    {
      Debug.Assert( array != null );
      Debug.Assert( start >= 0 );
      Debug.Assert( length >= 0 );
      Debug.Assert( ( start + length ) < array.Length );

      for( int i = 0; i < length; i ++ ) 
      {
        array[i + start] *= scale;
      }
    }

    //---------------------------------------------------------------------------------------------

    /// <summary>
    /// Multiply each element in target array with corresponding element in rhs array
    /// </summary>
    /// <param name="target"></param>
    /// <param name="rhs"></param>
    static public void Multiply( Complex[] target, Complex[] rhs ) 
    {
      ComplexArray.Multiply( target, rhs, target );
    }
    /// <summary>
    /// Multiply each element in lhs array with corresponding element in rhs array and
    /// put product in result array
    /// </summary>
    /// <param name="lhs"></param>
    /// <param name="rhs"></param>
    /// <param name="result"></param>
    static public void Multiply( Complex[] lhs, Complex[] rhs, Complex[] result ) 
    {
      Debug.Assert( lhs != null );
      Debug.Assert( rhs != null );
      Debug.Assert( result != null );
      Debug.Assert( lhs.Length == rhs.Length );
      Debug.Assert( lhs.Length == result.Length );

      int length = lhs.Length;
      for( int i = 0; i < length; i ++ ) 
      {
        result[i] = lhs[i] * rhs[i];
      }
    }

    /// <summary>
    /// Multiply each element in target array with corresponding element in rhs array
    /// </summary>
    /// <param name="target"></param>
    /// <param name="rhs"></param>
    static public void Multiply( ComplexFloat[] target, ComplexFloat[] rhs ) 
    {
      ComplexArray.Multiply( target, rhs, target );
    }
    /// <summary>
    /// Multiply each element in lhs array with corresponding element in rhs array and
    /// put product in result array
    /// </summary>
    /// <param name="lhs"></param>
    /// <param name="rhs"></param>
    /// <param name="result"></param>
    static public void Multiply( ComplexFloat[] lhs, ComplexFloat[] rhs, ComplexFloat[] result ) 
    {
      Debug.Assert( lhs != null );
      Debug.Assert( rhs != null );
      Debug.Assert( result != null );
      Debug.Assert( lhs.Length == rhs.Length );
      Debug.Assert( lhs.Length == result.Length );

      int length = lhs.Length;
      for( int i = 0; i < length; i ++ ) 
      {
        result[i] = lhs[i] * rhs[i];
      }
    }

    //---------------------------------------------------------------------------------------------

    /// <summary>
    /// Divide each element in target array with corresponding element in rhs array
    /// </summary>
    /// <param name="target"></param>
    /// <param name="rhs"></param>
    static public void Divide( Complex[] target, Complex[] rhs ) 
    {
      ComplexArray.Divide( target, rhs, target );
    }
    /// <summary>
    /// Divide each element in lhs array with corresponding element in rhs array and
    /// put product in result array
    /// </summary>
    /// <param name="lhs"></param>
    /// <param name="rhs"></param>
    /// <param name="result"></param>
    static public void Divide( Complex[] lhs, Complex[] rhs, Complex[] result ) 
    {
      Debug.Assert( lhs != null );
      Debug.Assert( rhs != null );
      Debug.Assert( result != null );
      Debug.Assert( lhs.Length == rhs.Length );
      Debug.Assert( lhs.Length == result.Length );

      int length = lhs.Length;
      for( int i = 0; i < length; i ++ ) 
      {
        result[i] = lhs[i] / rhs[i];
      }
    }

    /// <summary>
    /// Divide each element in target array with corresponding element in rhs array
    /// </summary>
    /// <param name="target"></param>
    /// <param name="rhs"></param>
    static public void Divide( ComplexFloat[] target, ComplexFloat[] rhs ) 
    {
      ComplexArray.Divide( target, rhs, target );
    }
    /// <summary>
    /// Divide each element in lhs array with corresponding element in rhs array and
    /// put product in result array
    /// </summary>
    /// <param name="lhs"></param>
    /// <param name="rhs"></param>
    /// <param name="result"></param>
    static public void Divide( ComplexFloat[] lhs, ComplexFloat[] rhs, ComplexFloat[] result ) 
    {
      Debug.Assert( lhs != null );
      Debug.Assert( rhs != null );
      Debug.Assert( result != null );
      Debug.Assert( lhs.Length == rhs.Length );
      Debug.Assert( lhs.Length == result.Length );

      ComplexFloat zero = ComplexFloat.Zero;
      int length = lhs.Length;
      for( int i = 0; i < length; i ++ ) 
      {
        if( rhs[i] != zero ) 
        {
          result[i] = lhs[i] / rhs[i];
        }
        else 
        {
          result[i] = zero;
        }
      }
    }

    //---------------------------------------------------------------------------------------------

    /*static public void Flip( ComplexFloat[] array, Size3 size ) {
      Debug.Assert( array != null );

      ComplexFloat[]  workspace = null;
      ComplexArray.LockWorkspaceF( size.GetTotalLength(), ref workspace );
      
      for( int z = 0; z < size.Depth; z ++ ) {
        for( int y = 0; y < size.Height; y ++ ) {
          int xyzOffset = 0 + y * size.Width + z * size.Width * size.Height;
          int abcOffset = size.Width - 1 + ( size.Height - y - 1 ) * size.Width + ( size.Depth - z - 1 ) * size.Width * size.Height;
          for( int x = 0; x < size.Width; x ++ ) {
            workspace[ xyzOffset ++ ] = array[ abcOffset -- ];
          }
        }
      }

      for( int i = 0; i < size.GetTotalLength(); i ++ ) {
        array[ i ] = workspace[ i ];
      }

      ComplexArray.UnlockWorkspaceF( ref workspace );
    }  */
    

    /// <summary>
    /// Copy an array
    /// </summary>
    /// <param name="dest"></param>
    /// <param name="source"></param>
    static public void Copy( Complex[] dest, Complex[] source ) 
    {
      Debug.Assert( dest != null );
      Debug.Assert( source != null );
      Debug.Assert( dest.Length == source.Length );
      for( int i = 0; i < dest.Length; i ++ ) 
      {
        dest[i] = source[i];
      }
    }

    /// <summary>
    /// Copy an array
    /// </summary>
    /// <param name="dest"></param>
    /// <param name="source"></param>
    static public void Copy( ComplexFloat[] dest, ComplexFloat[] source ) 
    {
      Debug.Assert( dest != null );
      Debug.Assert( source != null );
      Debug.Assert( dest.Length == source.Length );
      for( int i = 0; i < dest.Length; i ++ ) 
      {
        dest[i] = source[i];
      }
    }

    /// <summary>
    /// Reverse the elements in the array
    /// </summary>
    /// <param name="array"></param>
    static public void Reverse( Complex[] array ) 
    {
      Complex temp;
      int length = array.Length;
      for( int i = 0; i < length/2; i ++ ) 
      {
        temp = array[i];
        array[i] = array[length-1-i];
        array[length-1-i] = temp;
      }
    }

    /// <summary>
    /// Scale and offset the elements in the array so that the
    /// overall range is [0, 1]
    /// </summary>
    /// <param name="array"></param>
    static public void Normalize( Complex[] array ) 
    {
      double min = 0, max = 0;
      GetLengthRange( array, ref min, ref max );
      Scale( array, ( 1 / ( max - min ) ) );
      Offset( array, ( - min / ( max - min ) ) );
    }

    /// <summary>
    /// Scale and offset the elements in the array so that the
    /// overall range is [0, 1]
    /// </summary>
    /// <param name="array"></param>
    static public void Normalize( ComplexFloat[] array ) 
    {
      float min = 0, max = 0;
      GetLengthRange( array, ref min, ref max );
      Scale( array, ( 1 / ( max - min ) ) );
      Offset( array, ( - min / ( max - min ) ) );
    }

    /// <summary>
    /// Invert each element in the array
    /// </summary>
    /// <param name="array"></param>
    static public void Invert( Complex[] array ) 
    {
      for( int i = 0; i < array.Length; i ++ ) 
      {
        array[i] = ((Complex) 1 ) / array[i];
      }
    }

    /// <summary>
    /// Invert each element in the array
    /// </summary>
    /// <param name="array"></param>
    static public void Invert( ComplexFloat[] array ) 
    {
      for( int i = 0; i < array.Length; i ++ ) 
      {
        array[i] = ((ComplexFloat) 1 ) / array[i];
      }
    }

    //----------------------------------------------------------------------------------------

  }
}
