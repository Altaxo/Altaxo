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
  /// Rounding of numbers
  /// </summary>
  public class Rounding
  {
    /// <summary>
    /// This returns the next number k with k greater or equal i, and k mod n == 0. 
    /// </summary>
    /// <param name="i">The number to round up.</param>
    /// <param name="n">The rounding step.</param>
    /// <returns></returns>
    public static int RoundUp(int i , int n)
    {
      n = Math.Abs(n);
      int r = i % n;
      return r==0 ? i : i + n - r;
    }
    /// <summary>
    /// This returns the next number k with k lesser or equal i, and k mod n == 0. 
    /// </summary>
    /// <param name="i">The number to round down.</param>
    /// <param name="n">The rounding step.</param>
    /// <returns></returns>
    public static int RoundDown(int i , int n)
    {
      n = Math.Abs(n);
      int r = i % n;
      return r==0 ? i : i - r;
    }
    /// <summary>
    /// This returns the next number k with k greater or equal i, and k mod n == 0. 
    /// </summary>
    /// <param name="i">The number to round up.</param>
    /// <param name="n">The rounding step.</param>
    /// <returns></returns>
    public static long RoundUp(long i , long n)
    {
      n = Math.Abs(n);
      long r = i % n;
      return r==0 ? i : i + n - r;
    }
    /// <summary>
    /// This returns the next number k with k lesser or equal i, and k mod n == 0. 
    /// </summary>
    /// <param name="i">The number to round down.</param>
    /// <param name="n">The rounding step.</param>
    /// <returns></returns>
    public static long RoundDown(long i , long n)
    {
      n = Math.Abs(n);
      long r = i % n;
      return r==0 ? i : i - r;
    }

  }
}
