// <copyright file="Log1p.cs" company="Math.NET">
// Math.NET Numerics, part of the Math.NET Project
// http://numerics.mathdotnet.com
// http://github.com/mathnet/mathnet-numerics
//
// Copyright (c) 2009-2022 Math.NET
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

// ReSharper disable once CheckNamespace
namespace Altaxo.Calc
{
  /// <summary>
  /// This partial implementation of the SpecialFunctions class contains all methods related to the log1p function.
  /// </summary>
  public static partial class SpecialFunctions
  {
    /// <summary>
    /// Computes ln(1+x) with good relative precision when |x| is small
    /// </summary>
    /// <param name="x">The parameter for which to compute the log1p function. Range: x > 0.</param>
    public static double Log1p(double x)
    {
      double y0 = Math.Log(1.0 + x);

      if ((-0.2928 < x) && (x < 0.4142))
      {
        double y = y0;

        if (y == 0.0)
        {
          y = 1.0;
        }
        else if ((y < -0.69) || (y > 0.4))
        {
          y = (Math.Exp(y) - 1.0) / y;
        }
        else
        {
          double t = y / 2.0;
          y = Math.Exp(t) * Math.Sinh(t) / t;
        }

        double s = y0 * y;
        double r = (s - x) / (s + 1.0);
        y0 = y0 - r * (6 - r) / (6 - 4 * r);
      }

      return y0;
    }
  }
}
