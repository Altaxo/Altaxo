// <copyright file="Stability.cs" company="Math.NET">
// Math.NET Numerics, part of the Math.NET Project
// http://numerics.mathdotnet.com
// http://github.com/mathnet/mathnet-numerics
//
// Copyright (c) 2009-2010 Math.NET
//
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
// </copyright>

using System;
using Complex = System.Numerics.Complex;

// ReSharper disable once CheckNamespace
namespace Altaxo.Calc
{
  public partial class SpecialFunctions
  {
    /// <summary>
    /// Numerically stable hypotenuse of a right angle triangle, i.e. <code>(a,b) -> sqrt(a^2 + b^2)</code>
    /// </summary>
    /// <param name="a">The length of side a of the triangle.</param>
    /// <param name="b">The length of side b of the triangle.</param>
    /// <returns>Returns <code>sqrt(a<sup>2</sup> + b<sup>2</sup>)</code> without underflow/overflow.</returns>
    public static Complex Hypotenuse(Complex a, Complex b)
    {
      double x, y, min, max;
      x = a.Magnitude;
      y = b.Magnitude;
      if (y > x)
      {
        min = x;
        max = y;
      }
      else
      {
        min = y;
        max = x;
      }
      if (min == 0)
      {
        return max;
      }
      else if (double.IsInfinity(x + y))
      {
        return double.PositiveInfinity;
      }
      else
      {
        var u = min / max;
        return max * Math.Sqrt(1 + u * u);
      }
    }


    /// <summary>
    /// Numerically stable hypotenuse of a right angle triangle, i.e. <code>(a,b) -> sqrt(a^2 + b^2)</code>
    /// </summary>
    /// <param name="a">The length of side a of the triangle.</param>
    /// <param name="b">The length of side b of the triangle.</param>
    /// <returns>Returns <code>sqrt(a<sup>2</sup> + b<sup>2</sup>)</code> without underflow/overflow.</returns>
    public static Complex32 Hypotenuse(Complex32 a, Complex32 b)
    {
      float x, y, min, max;
      x = a.Magnitude;
      y = b.Magnitude;
      if (y > x)
      {
        min = x;
        max = y;
      }
      else
      {
        min = y;
        max = x;
      }
      if (min == 0)
      {
        return max;
      }
      else if (float.IsInfinity(x + y))
      {
        return float.PositiveInfinity;
      }
      else
      {
        var u = min / max;
        return max * (float)Math.Sqrt(1 + u * u);
      }
    }

    /// <summary>
    /// Numerically stable hypotenuse of a right angle triangle, i.e. <code>(a,b) -> sqrt(a^2 + b^2)</code>
    /// </summary>
    /// <param name="a">The length of side a of the triangle.</param>
    /// <param name="b">The length of side b of the triangle.</param>
    /// <returns>Returns <code>sqrt(a<sup>2</sup> + b<sup>2</sup>)</code> without underflow/overflow.</returns>
    public static double Hypotenuse(double a, double b)
    {
      double x, y, min, max;
      x = Math.Abs(a);
      y = Math.Abs(b);
      if (y > x)
      {
        min = x;
        max = y;
      }
      else
      {
        min = y;
        max = x;
      }
      if (min == 0)
      {
        return max;
      }
      else if (double.IsInfinity(x + y))
      {
        return double.PositiveInfinity;
      }
      else
      {
        var u = min / max;
        return max * Math.Sqrt(1 + u * u);
      }
    }

    /// <summary>
    /// Numerically stable hypotenuse of a right angle triangle, i.e. <code>(a,b) -> sqrt(a^2 + b^2)</code>
    /// </summary>
    /// <param name="a">The length of side a of the triangle.</param>
    /// <param name="b">The length of side b of the triangle.</param>
    /// <returns>Returns <code>sqrt(a<sup>2</sup> + b<sup>2</sup>)</code> without underflow/overflow.</returns>
    public static float Hypotenuse(float a, float b)
    {
      float x, y, min, max;
      x = Math.Abs(a);
      y = Math.Abs(b);
      if (y > x)
      {
        min = x;
        max = y;
      }
      else
      {
        min = y;
        max = x;
      }
      if (min == 0)
      {
        return max;
      }
      else if (float.IsInfinity(x + y))
      {
        return float.PositiveInfinity;
      }
      else
      {
        var u = min / max;
        return max * (float)Math.Sqrt(1 + u * u);
      }
    }
  }
}
