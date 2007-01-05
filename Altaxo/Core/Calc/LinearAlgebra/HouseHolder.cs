#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Copyright (c) 2003-2004, dnAnalytics. All rights reserved.
//
//    modified for Altaxo:  a data processing and data plotting program
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

//Port of JAMPACK's householder implementation. 
using System;

namespace Altaxo.Calc.LinearAlgebra
{

  /// <summary>
  /// Householder transformations.
  /// </summary>
  /// <remarks>
  /// <para>Copyright (c) 2003-2004, dnAnalytics Project. All rights reserved. See <a>http://www.dnAnalytics.net</a> for details.</para>
  /// <para>Adopted to Altaxo (c) 2005 Dr. Dirk Lellinger.</para>
  /// </remarks>
  internal class Householder
  {
    private Householder() { }
    public static ComplexDoubleVector GenerateColumn(IComplexDoubleMatrix A, int r1, int r2, int c)
    {
      int ru = r2 - r1 + 1;
      ComplexDoubleVector u = new ComplexDoubleVector(r2 - r1 + 1);

      for (int i = r1; i <= r2; i++)
      {
        u[i - r1] = A[i, c];
        A[i, c] = Complex.Zero;
      }

      double norm = u.GetNorm();

      if (r1 == r2 || norm == 0)
      {
        A[r1, c] = new Complex(-u[0]);
        u[0] = System.Math.Sqrt(2);
        return u;
      }

      Complex scale = new Complex(1 / norm, 0);

      Complex t = Complex.Zero;
      Complex t1 = Complex.Zero;
      if (u[0].Real != 0 || u[0].Imag != 0)
      {
        t = u[0];
        t1 = ComplexMath.Conjugate(u[0]);
        t = ComplexMath.Absolute(t);
        t = t1 / t;
        scale = scale * t;
      }
      A[r1, c] = -Complex.One / scale;

      for (int i = 0; i < ru; i++)
      {
        u[i] = u[i] * scale;
      }

      u[0] = new Complex(u[0].Real + 1, 0);
      double s = System.Math.Sqrt(1 / u[0].Real);

      for (int i = 0; i < ru; i++)
      {
        u[i] = new Complex(s * u[i].Real, s * u[i].Imag);
      }
      return u;
    }

    public static ComplexDoubleVector GenerateRow(IComplexDoubleMatrix A, int r, int c1, int c2)
    {
      int cu = c2 - c1 + 1;
      ComplexDoubleVector u = new ComplexDoubleVector(cu);

      for (int j = c1; j <= c2; j++)
      {
        u[j - c1] = A[r, j];
        A[r, j] = Complex.Zero;
      }

      double norm = u.GetNorm();

      if (c1 == c2 || norm == 0)
      {
        A[r, c1] = new Complex(-u[0].Real, -u[0].Imag);
        u[0] = System.Math.Sqrt(2);
        return u;
      }

      Complex scale = new Complex(1 / norm);

      Complex t = Complex.Zero;
      Complex t1 = Complex.Zero;
      if (u[0].Real != 0 || u[0].Imag != 0)
      {
        t = u[0];
        t1 = ComplexMath.Conjugate(u[0]);
        t = ComplexMath.Absolute(t);
        t = t1 / t;
        scale = scale * t;
      }

      A[r, c1] = -Complex.One / scale;

      for (int j = 0; j < cu; j++)
      {
        u[j] *= scale;
      }

      u[0] = new Complex(u[0].Real + 1);
      double s = System.Math.Sqrt(1 / u[0].Real);

      for (int j = 0; j < cu; j++)
      {
        u[j] = new Complex(s * u[j].Real, -s * u[j].Imag);
      }
      return u;
    }

    public static IComplexDoubleMatrix UA(IROComplexDoubleVector u, IComplexDoubleMatrix A, int r1, int r2, int c1, int c2, IComplexDoubleVector v)
    {
      if (r2 < r1 || c2 < c1)
      {
        return A;
      }

      if (r2 - r1 + 1 > u.Length)
      {
        throw new ArgumentException("Householder vector too short.", "u");
      }

      if (c2 - c1 + 1 > v.Length)
      {
        throw new ArgumentException("Work vector too short.", "v");
      }

      for (int j = c1; j <= c2; j++)
      {
        v[j - c1] = Complex.Zero;
      }

      for (int i = r1; i <= r2; i++)
      {
        for (int j = c1; j <= c2; j++)
        {
          v[j - c1] = new Complex(v[j - c1].Real + u[i - r1].Real * A[i, j].Real + u[i - r1].Imag * A[i, j].Imag,
            v[j - c1].Imag + u[i - r1].Real * A[i, j].Imag - u[i - r1].Imag * A[i, j].Real);
        }
      }

      for (int i = r1; i <= r2; i++)
      {
        for (int j = c1; j <= c2; j++)
        {
          A[i, j] = new Complex(A[i, j].Real - u[i - r1].Real * v[j - c1].Real + u[i - r1].Imag * v[j - c1].Imag,
            A[i, j].Imag - u[i - r1].Real * v[j - c1].Imag - u[i - r1].Imag * v[j - c1].Real);
        }
      }
      return A;
    }

    public static IComplexDoubleMatrix UA(IROComplexDoubleVector u, IComplexDoubleMatrix A, int r1, int r2, int c1, int c2)
    {
      if (c1 > c2)
      {
        return A;
      }
      return UA(u, A, r1, r2, c1, c2, new ComplexDoubleVector(c2 - c1 + 1));
    }


    public static IComplexDoubleMatrix AU(IComplexDoubleMatrix A, IROComplexDoubleVector u, int r1, int r2, int c1, int c2, IComplexDoubleVector v)
    {
      if (r2 < r1 || c2 < c1)
      {
        return A;
      }

      if (c2 - c1 + 1 > u.Length)
      {
        throw new ArgumentException("Householder vector too short.", "u");
      }

      if (r2 - r1 + 1 > v.Length)
      {
        throw new ArgumentException("Work vector too short.", "v");
      }

      for (int i = r1; i <= r2; i++)
      {
        v[i - r1] = Complex.Zero;
        for (int j = c1; j <= c2; j++)
        {
          v[i - r1] = new Complex(v[i - r1].Real + A[i, j].Real * u[j - c1].Real - A[i, j].Imag * u[j - c1].Imag,
            v[i - r1].Imag + A[i, j].Real * u[j - c1].Imag + A[i, j].Imag * u[j - c1].Real);
        }
      }
      for (int i = r1; i <= r2; i++)
      {
        for (int j = c1; j <= c2; j++)
        {
          A[i, j] = new Complex(A[i, j].Real - v[i - r1].Real * u[j - c1].Real - v[i - r1].Imag * u[j - c1].Imag,
            A[i, j].Imag + v[i - r1].Real * u[j - c1].Imag - v[i - r1].Imag * u[j - c1].Real);
        }
      }
      return A;
    }


    public static IComplexDoubleMatrix AU(IComplexDoubleMatrix A, IROComplexDoubleVector u, int r1, int r2, int c1, int c2)
    {
      if (r2 < r1)
      {
        return A;
      }
      return AU(A, u, r1, r2, c1, c2, new ComplexDoubleVector(r2 - r1 + 1));
    }

    public static ComplexFloatVector GenerateColumn(IComplexFloatMatrix A, int r1, int r2, int c)
    {
      int ru = r2 - r1 + 1;
      ComplexFloatVector u = new ComplexFloatVector(r2 - r1 + 1);

      for (int i = r1; i <= r2; i++)
      {
        u[i - r1] = A[i, c];
        A[i, c] = ComplexFloat.Zero;
      }

      float norm = u.GetNorm();

      if (r1 == r2 || norm == 0)
      {
        A[r1, c] = new ComplexFloat(-u[0]);
        u[0] = (float)System.Math.Sqrt(2);
        return u;
      }

      ComplexFloat scale = new ComplexFloat(1 / norm, 0);

      ComplexFloat t = ComplexFloat.Zero;
      ComplexFloat t1 = ComplexFloat.Zero;
      if (u[0].Real != 0 || u[0].Imag != 0)
      {
        t = u[0];
        t1 = ComplexMath.Conjugate(u[0]);
        t = ComplexMath.Absolute(t);
        t = t1 / t;
        scale = scale * t;
      }

      A[r1, c] = -ComplexFloat.One / scale;

      for (int i = 0; i < ru; i++)
      {
        u[i] = u[i] * scale;
      }

      u[0] = new ComplexFloat(u[0].Real + 1, 0);
      float s = (float)System.Math.Sqrt(1 / u[0].Real);

      for (int i = 0; i < ru; i++)
      {
        u[i] = new ComplexFloat(s * u[i].Real, s * u[i].Imag);
      }
      return u;
    }

    public static ComplexFloatVector GenerateRow(IComplexFloatMatrix A, int r, int c1, int c2)
    {
      int cu = c2 - c1 + 1;
      ComplexFloatVector u = new ComplexFloatVector(cu);

      for (int j = c1; j <= c2; j++)
      {
        u[j - c1] = A[r, j];
        A[r, j] = ComplexFloat.Zero;
      }

      float norm = u.GetNorm();

      if (c1 == c2 || norm == 0)
      {
        A[r, c1] = new ComplexFloat(-u[0].Real, -u[0].Imag);
        u[0] = (float)System.Math.Sqrt(2);
        return u;
      }

      ComplexFloat scale = new ComplexFloat(1 / norm);

      ComplexFloat t = ComplexFloat.Zero;
      ComplexFloat t1 = ComplexFloat.Zero;
      if (u[0].Real != 0 || u[0].Imag != 0)
      {
        t = u[0];
        t1 = ComplexMath.Conjugate(u[0]);
        t = ComplexMath.Absolute(t);
        t = t1 / t;
        scale = scale * t;
      }

      A[r, c1] = -ComplexFloat.One / scale;

      for (int j = 0; j < cu; j++)
      {
        u[j] *= scale;
      }

      u[0] = new ComplexFloat(u[0].Real + 1);
      float s = (float)System.Math.Sqrt(1 / u[0].Real);

      for (int j = 0; j < cu; j++)
      {
        u[j] = new ComplexFloat(s * u[j].Real, -s * u[j].Imag);
      }
      return u;
    }

    public static IComplexFloatMatrix UA(IROComplexFloatVector u, IComplexFloatMatrix A, int r1, int r2, int c1, int c2, IComplexFloatVector v)
    {
      if (r2 < r1 || c2 < c1)
      {
        return A;
      }

      if (r2 - r1 + 1 > u.Length)
      {
        throw new ArgumentException("Householder vector too short.", "u");
      }

      if (c2 - c1 + 1 > v.Length)
      {
        throw new ArgumentException("Work vector too short.", "v");
      }

      for (int j = c1; j <= c2; j++)
      {
        v[j - c1] = ComplexFloat.Zero;
      }

      for (int i = r1; i <= r2; i++)
      {
        for (int j = c1; j <= c2; j++)
        {
          v[j - c1] = new ComplexFloat(v[j - c1].Real + u[i - r1].Real * A[i, j].Real + u[i - r1].Imag * A[i, j].Imag,
            v[j - c1].Imag + u[i - r1].Real * A[i, j].Imag - u[i - r1].Imag * A[i, j].Real);
        }
      }

      for (int i = r1; i <= r2; i++)
      {
        for (int j = c1; j <= c2; j++)
        {
          A[i, j] = new ComplexFloat(A[i, j].Real - u[i - r1].Real * v[j - c1].Real + u[i - r1].Imag * v[j - c1].Imag,
            A[i, j].Imag - u[i - r1].Real * v[j - c1].Imag - u[i - r1].Imag * v[j - c1].Real);
        }
      }
      return A;
    }

    public static IComplexFloatMatrix UA(IROComplexFloatVector u, IComplexFloatMatrix A, int r1, int r2, int c1, int c2)
    {
      if (c1 > c2)
      {
        return A;
      }
      return UA(u, A, r1, r2, c1, c2, new ComplexFloatVector(c2 - c1 + 1));
    }


    public static IComplexFloatMatrix AU(IComplexFloatMatrix A, IROComplexFloatVector u, int r1, int r2, int c1, int c2, IComplexFloatVector v)
    {
      if (r2 < r1 || c2 < c1)
      {
        return A;
      }

      if (c2 - c1 + 1 > u.Length)
      {
        throw new ArgumentException("Householder vector too short.", "u");
      }

      if (r2 - r1 + 1 > v.Length)
      {
        throw new ArgumentException("Work vector too short.", "v");
      }

      for (int i = r1; i <= r2; i++)
      {
        v[i - r1] = ComplexFloat.Zero;
        for (int j = c1; j <= c2; j++)
        {
          v[i - r1] = new ComplexFloat(v[i - r1].Real + A[i, j].Real * u[j - c1].Real - A[i, j].Imag * u[j - c1].Imag,
            v[i - r1].Imag + A[i, j].Real * u[j - c1].Imag + A[i, j].Imag * u[j - c1].Real);
        }
      }
      for (int i = r1; i <= r2; i++)
      {
        for (int j = c1; j <= c2; j++)
        {
          A[i, j] = new ComplexFloat(A[i, j].Real - v[i - r1].Real * u[j - c1].Real - v[i - r1].Imag * u[j - c1].Imag,
            A[i, j].Imag + v[i - r1].Real * u[j - c1].Imag - v[i - r1].Imag * u[j - c1].Real);
        }
      }
      return A;
    }


    public static IComplexFloatMatrix AU(IComplexFloatMatrix A, IROComplexFloatVector u, int r1, int r2, int c1, int c2)
    {
      if (r2 < r1)
      {
        return A;
      }
      return AU(A, u, r1, r2, c1, c2, new ComplexFloatVector(r2 - r1 + 1));
    }

    public static FloatVector GenerateColumn(IFloatMatrix A, int r1, int r2, int c)
    {
      int ru = r2 - r1 + 1;
      FloatVector u = new FloatVector(r2 - r1 + 1);

      for (int i = r1; i <= r2; i++)
      {
        u[i - r1] = A[i, c];
        A[i, c] = 0.0f;
      }

      float norm = u.GetNorm();

      if (r1 == r2 || norm == 0)
      {
        A[r1, c] = -u[0];
        u[0] = (float)System.Math.Sqrt(2);
        return u;
      }

      float scale = 1.0f / norm;

      if (u[0] < 0.0f)
        scale *= -1.0f;

      A[r1, c] = -1.0f / scale;

      for (int i = 0; i < ru; i++)
      {
        u[i] = u[i] * scale;
      }

      u[0] = u[0] + 1.0f;
      float s = (float)System.Math.Sqrt(1 / u[0]);

      for (int i = 0; i < ru; i++)
      {
        u[i] = s * u[i];
      }
      return u;
    }

    public static FloatVector GenerateRow(IFloatMatrix A, int r, int c1, int c2)
    {
      int cu = c2 - c1 + 1;
      FloatVector u = new FloatVector(cu);

      for (int j = c1; j <= c2; j++)
      {
        u[j - c1] = A[r, j];
        A[r, j] = 0.0f;
      }

      float norm = u.GetNorm();

      if (c1 == c2 || norm == 0)
      {
        A[r, c1] = -u[0];
        u[0] = (float)System.Math.Sqrt(2);
        return u;
      }

      float scale = 1.0f / norm;
      if (u[0] < 0.0f)
        scale *= -1.0f;

      A[r, c1] = -1.0f / scale;

      for (int j = 0; j < cu; j++)
      {
        u[j] *= scale;
      }

      u[0] += 1.0f;
      float s = (float)System.Math.Sqrt(1 / u[0]);

      for (int j = 0; j < cu; j++)
      {
        u[j] *= s;
      }
      return u;
    }

    public static IFloatMatrix UA(IROFloatVector u, IFloatMatrix A, int r1, int r2, int c1, int c2, IFloatVector v)
    {
      if (r2 < r1 || c2 < c1)
      {
        return A;
      }

      if (r2 - r1 + 1 > u.Length)
      {
        throw new ArgumentException("Householder vector too short.", "u");
      }

      if (c2 - c1 + 1 > v.Length)
      {
        throw new ArgumentException("Work vector too short.", "v");
      }

      for (int j = c1; j <= c2; j++)
      {
        v[j - c1] = 0.0f;
      }

      for (int i = r1; i <= r2; i++)
      {
        for (int j = c1; j <= c2; j++)
        {
          v[j - c1] = v[j - c1] + u[i - r1] * A[i, j];
        }
      }

      for (int i = r1; i <= r2; i++)
      {
        for (int j = c1; j <= c2; j++)
        {
          A[i, j] = A[i, j] - u[i - r1] * v[j - c1];
        }
      }
      return A;
    }

    public static IROFloatMatrix UA(IROFloatVector u, IFloatMatrix A, int r1, int r2, int c1, int c2)
    {
      if (c1 > c2)
      {
        return A;
      }
      return UA(u, A, r1, r2, c1, c2, new FloatVector(c2 - c1 + 1));
    }


    public static IFloatMatrix AU(IFloatMatrix A, IROFloatVector u, int r1, int r2, int c1, int c2, IFloatVector v)
    {
      if (r2 < r1 || c2 < c1)
      {
        return A;
      }

      if (c2 - c1 + 1 > u.Length)
      {
        throw new ArgumentException("Householder vector too short.", "u");
      }

      if (r2 - r1 + 1 > v.Length)
      {
        throw new ArgumentException("Work vector too short.", "v");
      }

      for (int i = r1; i <= r2; i++)
      {
        v[i - r1] = 0.0f;
        for (int j = c1; j <= c2; j++)
        {
          v[i - r1] = v[i - r1] + A[i, j] * u[j - c1];
        }
      }
      for (int i = r1; i <= r2; i++)
      {
        for (int j = c1; j <= c2; j++)
        {
          A[i, j] = A[i, j] - v[i - r1] * u[j - c1];
        }
      }
      return A;
    }


    public static IFloatMatrix AU(IFloatMatrix A, IROFloatVector u, int r1, int r2, int c1, int c2)
    {
      if (r2 < r1)
      {
        return A;
      }
      return AU(A, u, r1, r2, c1, c2, new FloatVector(r2 - r1 + 1));
    }

    public static DoubleVector GenerateColumn(IMatrix A, int r1, int r2, int c)
    {
      int ru = r2 - r1 + 1;
      DoubleVector u = new DoubleVector(r2 - r1 + 1);

      for (int i = r1; i <= r2; i++)
      {
        u[i - r1] = A[i, c];
        A[i, c] = 0.0;
      }

      double norm = u.GetNorm();

      if (r1 == r2 || norm == 0)
      {
        A[r1, c] = -u[0];
        u[0] = (double)System.Math.Sqrt(2);
        return u;
      }

      double scale = 1.0 / norm;
      if (u[0] < 0.0)
        scale *= -1.0;

      A[r1, c] = -1.0 / scale;

      for (int i = 0; i < ru; i++)
      {
        u[i] *= scale;
      }

      u[0] += 1.0;
      double s = (double)System.Math.Sqrt(1 / u[0]);

      for (int i = 0; i < ru; i++)
      {
        u[i] *= s;
      }
      return u;
    }

    public static DoubleVector GenerateRow(IMatrix A, int r, int c1, int c2)
    {
      int cu = c2 - c1 + 1;
      DoubleVector u = new DoubleVector(cu);

      for (int j = c1; j <= c2; j++)
      {
        u[j - c1] = A[r, j];
        A[r, j] = 0.0;
      }

      double norm = u.GetNorm();

      if (c1 == c2 || norm == 0)
      {
        A[r, c1] = -u[0];
        u[0] = (double)System.Math.Sqrt(2);
        return u;
      }

      double scale = 1.0 / norm;
      if (u[0] < 0.0)
        scale *= -1.0;

      A[r, c1] = -1.0 / scale;

      for (int j = 0; j < cu; j++)
      {
        u[j] *= scale;
      }

      u[0] = u[0] + 1.0;
      double s = (double)System.Math.Sqrt(1 / u[0]);

      for (int j = 0; j < cu; j++)
      {
        u[j] *= s;
      }
      return u;
    }

    public static IMatrix UA(IROVector u, IMatrix A, int r1, int r2, int c1, int c2, IVector v)
    {
      if (r2 < r1 || c2 < c1)
      {
        return A;
      }

      if (r2 - r1 + 1 > u.Length)
      {
        throw new ArgumentException("Householder vector too short.", "u");
      }

      if (c2 - c1 + 1 > v.Length)
      {
        throw new ArgumentException("Work vector too short.", "v");
      }

      for (int j = c1; j <= c2; j++)
      {
        v[j - c1] = 0.0;
      }

      for (int i = r1; i <= r2; i++)
      {
        for (int j = c1; j <= c2; j++)
        {
          v[j - c1] = v[j - c1] + u[i - r1] * A[i, j];
        }
      }

      for (int i = r1; i <= r2; i++)
      {
        for (int j = c1; j <= c2; j++)
        {
          A[i, j] = A[i, j] - u[i - r1] * v[j - c1];
        }
      }
      return A;
    }

    public static IMatrix UA(DoubleVector u, IMatrix A, int r1, int r2, int c1, int c2)
    {
      if (c1 > c2)
      {
        return A;
      }
      return UA(u, A, r1, r2, c1, c2, new DoubleVector(c2 - c1 + 1));
    }


    public static IMatrix AU(IMatrix A, DoubleVector u, int r1, int r2, int c1, int c2, IVector v)
    {
      if (r2 < r1 || c2 < c1)
      {
        return A;
      }

      if (c2 - c1 + 1 > u.Length)
      {
        throw new ArgumentException("Householder vector too short.", "u");
      }

      if (r2 - r1 + 1 > v.Length)
      {
        throw new ArgumentException("Work vector too short.", "v");
      }

      for (int i = r1; i <= r2; i++)
      {
        v[i - r1] = 0.0;
        for (int j = c1; j <= c2; j++)
        {
          v[i - r1] = v[i - r1] + A[i, j] * u[j - c1];
        }
      }
      for (int i = r1; i <= r2; i++)
      {
        for (int j = c1; j <= c2; j++)
        {
          A[i, j] = A[i, j] - v[i - r1] * u[j - c1];
        }
      }
      return A;
    }


    public static IMatrix AU(IMatrix A, DoubleVector u, int r1, int r2, int c1, int c2)
    {
      if (r2 < r1)
      {
        return A;
      }
      return AU(A, u, r1, r2, c1, c2, new DoubleVector(r2 - r1 + 1));
    }


  }
}
