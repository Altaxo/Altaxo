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

namespace Altaxo.Calc
{
  /// <summary>
  /// BinaryMath provides static methods for "Bit" mathematics.
  /// </summary>
  public class BinaryMath
  {

    /// <summary>
    /// Returns k so that 2^k &lt;= x &lt; 2^(k+1). If x==0 then 0 is returned.
    /// </summary>
    /// <param name="x">The argument.</param>
    /// <returns>A number k so that 2^k &lt;= x &lt; 2^(k+1). If x==0 then 0 is returned.</returns>
    public static int Ld(uint x)
    {
      if ( 0==x )  return  0;

      int r = 0;
      if ( (x & 0xffff0000)!=0 )  { x >>= 16;  r += 16; }
      if ( (x & 0x0000ff00)!=0 )  { x >>=  8;  r +=  8; }
      if ( (x & 0x000000f0)!=0 )  { x >>=  4;  r +=  4; }
      if ( (x & 0x0000000c)!=0 )  { x >>=  2;  r +=  2; }
      if ( (x & 0x00000002)!=0 )  {            r +=  1; }
      return r;
    }


    /// <summary>
    /// Returns k so that 2^k &lt;= x &lt; 2^(k+1).
    /// If x==0 then 0 is returned.
    /// </summary>
    /// <param name="x">The argument.</param>
    /// <returns>A number k so that 2^k &lt;= x &lt; 2^(k+1). If x==0 then 0 is returned.</returns>
    public static int Ld(ulong x)
    {
      if ( 0==x )  return  0;

      int r = 0;
      if ( (x & (~0UL<<32))!=0 )  { x >>= 32;  r += 32; }
      if ( (x & 0xffff0000)!=0 )  { x >>= 16;  r += 16; }
      if ( (x & 0x0000ff00)!=0 )  { x >>=  8;  r +=  8; }
      if ( (x & 0x000000f0)!=0 )  { x >>=  4;  r +=  4; }
      if ( (x & 0x0000000c)!=0 )  { x >>=  2;  r +=  2; }
      if ( (x & 0x00000002)!=0 )  {            r +=  1; }
      return r;
    }


    // return true if number is 0 (!) or a power of two
    /// <summary>
    /// Return true if number is 0 (!) or a power of two
    /// </summary>
    /// <param name="x">Argument to test.</param>
    /// <returns>Return true if number is 0 (!) or a power of two.</returns>
    public static bool IsPowerOfTwo(int x)
    {
      return  ((x & -x) == x);
    }


    /// <summary>
    /// Return true if x &gt; 0 and x is a power of two.
    /// </summary>
    /// <param name="x">The argument to test.</param>
    /// <returns>True if x &gt; 0 and x is a power of two.</returns>
    public static bool IsNonzeroPowerOfTwo(uint x)
    {
      ulong m = x-1;
      return  (((x^m)>>1) == m);
    }

    /// <summary>
    /// Return true if x &gt; 0 and x is a power of two.
    /// </summary>
    /// <param name="x">The argument to test.</param>
    /// <returns>True if x &gt; 0 and x is a power of two.</returns>
    public static bool IsNonzeroPowerOfTwo(ulong x)
    {
      ulong m = x-1;
      return  (((x^m)>>1) == m);
    }

   
    /// <summary>
    /// Return x if x is is a power of two, else return the smallest number &gt;x, which is a power of two.
    /// </summary>
    /// <param name="x">The argument to test.</param>
    /// <returns>The argument, if it is a power of two. Else the next greater number which is a power of two.</returns>
    public static int NextPowerOfTwo(int x)
    {
      int i;
      for(i=1; i<x ;i<<=1);
      return i;
    }
  }
}
