#region Copyright

// Copyright Microsoft Research in collaboration with Moscow State University
// Microsoft Research License, see license file "MSR-LA - Open Solving Library for ODEs.rtf"
// This file originates from project OSLO - Open solving libraries for ODEs - 1.1

#endregion Copyright

using System;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Calc.Ode.Obsolete
{
  /// <summary>Vector implementation. This is thin wrapper over 1D array</summary>
  public struct Vector
  {
    public double[] v { get; private set; }

    /// <summary>Gets number of components in a vector</summary>
    /// <exception cref="NullReferenceException">Thrown when vector constructed by default constructor and not assigned yet</exception>
    public int Length
    {
      get { return v is null ? 0 : v.Length; }
    }

    /// <summary>Constructs vector from array of arguments</summary>
    /// <param name="elts">Elements of array</param>
    /// <example>This creates vector with three elements -1,0,1. New
    /// array is allocated for this vector:
    /// <code>Vector v = new Vector(-1,0,1)</code>
    /// Next example creates vector that wraps specified array. Please note
    /// that no array copying is made thus changing of source array with change vector elements.
    /// <code>
    /// double[] arr = new double[] { -1,0,1 };
    /// Vector v = new Vector(arr);
    /// </code>
    /// </example>
    public Vector(params double[] elts)
    {
      if (elts is null)
        throw new ArgumentNullException("elts");
      v = elts;
    }

    /// <summary>Constructs vector of specified length filled with zeros</summary>
    /// <param name="n">Length of vector</param>
    /// <returns>Constructed vector</returns>
    public static Vector Zeros(int n)
    {
      var v = new Vector
      {
        v = new double[n]
      };
      return v;
    }

    /// <summary>Clones specified vector</summary>
    /// <returns>Copy of vector passes as parameter</returns>
    public Vector Clone()
    {
      return v is null ? new Vector() : new Vector((double[])v.Clone());
    }

    /// <summary>
    /// Copies vector to double[] array
    /// </summary>
    /// <returns></returns>
    public double[] ToArray()
    {
      return (double[])v.Clone();
    }

    /// <summary>Copies content of one vector to another. Vectors must have same length.</summary>
    /// <param name="src">Source vector</param>
    /// <param name="dst">Vector to copy results</param>
    public static void Copy(Vector src, Vector dst)
    {
      if (src.v is null)
        throw new ArgumentNullException("src");
      if (dst.v is null)
        throw new ArgumentNullException("dst");
      int n = src.v.Length;
      if (dst.v.Length != n)
        dst.v = new double[n];
      Array.Copy(src.v, dst.v, n);
    }

    /// <summary>Gets L-infinity norm of the vector</summary>
    public double LInfinityNorm
    {
      get
      {
        double max = 0;

        for (int i = 0; i < v.Length; i++)
        {
          if (Math.Abs(v[i]) > max)
            max = Math.Abs(v[i]);
        }

        return max;
      }
    }

    ///<summary>Gets vector's Euclidean norm</summary>
    public double EuclideanNorm
    {
      get
      {
        double lsq = 0;

        for (int i = 0; i < v.Length; i++)
        {
          lsq += v[i] * v[i];
        }

        return Math.Sqrt(lsq);
      }
    }

    ///<summary>Gets vector's Euclidean norm</summary>
    public double Sum
    {
      get
      {
        double sum = 0;

        for (int i = 0; i < v.Length; i++)
        {
          sum += v[i];
        }

        return sum;
      }
    }

    /// <summary>Returns Euclidean norm of difference between two vectors.
    /// </summary>
    /// <param name="v1">First vector</param>
    /// <param name="v2">Second vector</param>
    /// <returns>Euclidean norm of vector's difference</returns>
    public static double GetEuclideanNorm(Vector v1, Vector v2)
    {
      double[] av1 = v1.v;
      double[] av2 = v2.v;
      if (av1 is null)
        throw new ArgumentNullException("v1");
      if (av2 is null)
        throw new ArgumentNullException("v2");
      if (av1.Length != av2.Length)
        throw new ArgumentException("Vector lenghtes do not match");
      double norm = 0;
      for (int i = 0; i < av1.Length; i++)
        norm += (av1[i] - av2[i]) * (av1[i] - av2[i]);
      return Math.Sqrt(norm);
    }

    /// <summary>Returns L-infinity norm of difference between two vectors.
    /// </summary>
    /// <param name="v1">First vector</param>
    /// <param name="v2">Second vector</param>
    /// <returns>L-infinity norm of vector's difference</returns>
    public static double GetLInfinityNorm(Vector v1, Vector v2)
    {
      double[] av1 = v1.v;
      double[] av2 = v2.v;
      if (av1 is null)
        throw new ArgumentNullException("v1");
      if (av2 is null)
        throw new ArgumentNullException("v2");
      if (av1.Length != av2.Length)
        throw new ArgumentException("Vector lenghtes do not match");
      double norm = 0;
      for (int i = 0; i < av1.Length; i++)
        norm = Math.Max(norm, Math.Abs(av1[i] - av2[i]));
      return norm;
    }

    /// <summary>Performs linear intepolation between two vectors at specified point</summary>
    /// <param name="t">Point of intepolation</param>
    /// <param name="t0">First time point</param>
    /// <param name="v0">Vector at first time point</param>
    /// <param name="t1">Second time point</param>
    /// <param name="v1">Vector at second time point</param>
    /// <returns>Intepolated vector value at point <paramref name="t"/></returns>
    public static Vector Lerp(double t, double t0, Vector v0, double t1, Vector v1)
    {
      return (v0 * (t1 - t) + v1 * (t - t0)) / (t1 - t0);
    }

    /// <summary>Gets or sets vector element at specified index</summary>
    /// <exception cref="NullReferenceException">Thrown when vector is not initialized</exception>
    /// <exception cref="IndexOutOfRangeException">Throws when <paramref name="idx"/> is out of range</exception>
    /// <param name="idx">Index of element</param>
    /// <returns>Value of vector element at specified index</returns>
    public double this[int idx]
    {
      get { return v[idx]; }
      set { v[idx] = value; }
    }

    /// <summary>Performs conversion of vector to array</summary>
    /// <param name="v">Vector to be converted</param>
    /// <returns>Array with contents of vector</returns>
    /// <remarks>This conversions doesn't perform array copy. In fact in returns reference
    /// to the same data</remarks>
    public static implicit operator double[](Vector v)
    {
      return v.v;
    }

    /// <summary>Performs conversion of 1d vector to</summary>
    /// <param name="v">Vector to be converted</param>
    /// <returns>Scalar value with first component of vector</returns>
    /// <exception cref="InvalidOperationException">Thrown when vector length is not one</exception>
    public static implicit operator double(Vector v)
    {
      double[] av = v;
      if (av is null)
        throw new ArgumentNullException("v");
      if (av.Length != 1)
        throw new InvalidOperationException("Cannot convert multi-element vector to scalar");
      return av[0];
    }

    /// <summary>Performs conversion of array to vector</summary>
    /// <param name="v">Array to be represented by vector</param>
    /// <returns>Vector that wraps specified array</returns>
    public static implicit operator Vector(double[] v)
    {
      return new Vector(v);
    }

    /// <summary>Performs conversion of scalar to vector with length 1</summary>
    /// <param name="v">Double precision vector</param>
    /// <returns>Vector that wraps array with 1 element</returns>
    public static implicit operator Vector(double v)
    {
      return new Vector(v);
    }

    /// <summary>Adds vector <paramref name="v1"/> multiplied by <paramref name="factor"/> to this object.</summary>
    /// <param name="v1">Vector to add</param>
    /// <param name="factor">Multiplication factor</param>
    public void MulAdd(Vector v1, double factor)
    {
      double[] av1 = v1.v;
      if (av1 is null)
        throw new ArgumentNullException("v1");
      if (Length != av1.Length)
        throw new InvalidOperationException("Cannot add vectors of different length");

      for (int i = 0; i < v.Length; i++)
        v[i] = v[i] + factor * av1[i];
    }

    /// <summary>Sums two vectors. Vectors must have same length.</summary>
    /// <param name="v1">First vector</param>
    /// <param name="v2">Second vector</param>
    /// <returns>Sum of vectors</returns>
    public static Vector operator +(Vector v1, Vector v2)
    {
      double[] av1 = v1;
      double[] av2 = v2;
      if (av1.Length != av2.Length)
        throw new InvalidOperationException("Cannot add vectors of different length");
      double[] result = new double[av1.Length];
      for (int i = 0; i < av1.Length; i++)
        result[i] = av1[i] + av2[i];
      return new Vector(result);
    }

    /// <summary>Add a scalar to a vector.</summary>
    /// <param name="v">Vector</param>
    /// <param name="c">Scalar to add</param>
    /// <returns>Shifted vector</returns>
    public static Vector operator +(Vector v, double c)
    {
      double[] av = v;
      double[] result = new double[av.Length];
      for (int i = 0; i < av.Length; i++)
        result[i] = av[i] + c;
      return new Vector(result);
    }

    /// <summary>Substracts first vector from second. Vectors must have same length</summary>
    /// <param name="v1">First vector</param>
    /// <param name="v2">Second vector</param>
    /// <returns>Difference of two vectors</returns>
    public static Vector operator -(Vector v1, Vector v2)
    {
      double[] av1 = v1;
      double[] av2 = v2;
      if (av1.Length != av2.Length)
        throw new InvalidOperationException("Cannot subtract vectors of different length");
      double[] result = new double[av1.Length];
      for (int i = 0; i < av1.Length; i++)
        result[i] = av1[i] - av2[i];
      return new Vector(result);
    }

    /// <summary>Multiplies vector <see cref="Vector"/> by matrix</summary>
    /// <param name="v">Vector</param>
    /// <param name="m">Matrix</param>
    /// <returns>Product of <see cref="Vector"/> and matrix</returns>
    public static Vector operator *(Matrix m, Vector v)
    {
      if (m is null)
        throw new ArgumentNullException("m");
      if (v.Length != m.ColumnDimension)
        throw new ArgumentException("Dimensions of vector and matrix do not match");

      double[] av = v;
      int rowDimension = m.RowDimension;
      int columnDimension = m.ColumnDimension;

      double[] result = new double[rowDimension];

      for (int i = 0; i < rowDimension; i++)
      {
        var acc = 0.0;
        var column = m[i];

        for (int j = 0; j < columnDimension; j++)
        {
          acc += column[j] * av[j];
        }
        result[i] = acc;
      }
      return new Vector(result);
    }

    /// <summary>Implements multiplication of matrix by vector</summary>
    /// <param name="m">Matrix</param>
    /// <param name="v">Vector</param>
    /// <returns>Result of multiplication</returns>
    public static Vector operator *(Vector v, Matrix m)
    {
      double[] av = v;
      if (m is null)
        throw new ArgumentNullException("m");
      if (v.Length != m.RowDimension)
        throw new ArgumentException("Dimensions of matrix and vector do not match");
      double[] result = new double[m.ColumnDimension];
      for (int i = 0; i < m.RowDimension; i++)
      {
        for (int j = 0; j < m.ColumnDimension; j++)
        {
          result[j] = result[j] + av[i] * m[i, j];
        }
      }
      return new Vector(result);
    }

    /// <summary>Multiplies a vector by a scalar (per component)</summary>
    /// <param name="v">Vector</param>
    /// <param name="a">Scalar</param>
    /// <returns>Vector with all components multiplied by scalar</returns>
    public static Vector operator *(Vector v, double a)
    {
      double[] av = v;
      double[] result = new double[av.Length];
      for (int i = 0; i < av.Length; i++)
        result[i] = a * av[i];
      return new Vector(result);
    }

    /// <summary>Multiplies a vector by a scalar (per component)</summary>
    /// <param name="v">Vector</param>
    /// <param name="a">Scalar</param>
    /// <returns>Vector with all components multiplied by scalar</returns>
    public static Vector operator *(double a, Vector v)
    {
      double[] av = v;
      double[] result = new double[av.Length];
      for (int i = 0; i < av.Length; i++)
        result[i] = a * av[i];
      return new Vector(result);
    }

    /// <summary>Performs scalar multiplication of two vectors</summary>
    /// <param name="a">First vector</param>
    /// <param name="b">Second vector</param>
    /// <returns>Result of scalar multiplication</returns>
    public static double operator *(Vector a, Vector b)
    {
      double res = 0;
      if (a.Length != a.Length)
        throw new InvalidOperationException("Cannot multiply vectors of different length");

      for (int i = 0; i < a.Length; i++)
      {
        res = res + a[i] * b[i];
      }
      return res;
    }

    /// <summary>Multiplies vector a[i] by vector b[j] and returns matrix with components a[i]*b[j]</summary>
    /// <param name="a">First vector</param>
    /// <param name="b">Second vector</param>
    /// <returns>Matrix with number of rows equals to first vector length and numbers of column equals to second vector length</returns>
    public static Matrix operator &(Vector a, Vector b)
    {
      int m = a.Length, n = b.Length;
      var res = new Matrix(m, n);
      for (int i = 0; i < m; i++)
      {
        for (int j = 0; j < n; j++)
        {
          res[i, j] = a[i] * b[j];
        }
      }
      return res;
    }

    /// <summary>Divides vector by a scalar (per component)</summary>
    /// <param name="v">Vector</param>
    /// <param name="a">Scalar</param>
    /// <returns>Result of division</returns>
    public static Vector operator /(Vector v, double a)
    {
      double[] av = v;

      if (a == 0.0d)
        throw new DivideByZeroException("Cannot divide by zero");

      double[] result = new double[av.Length];
      for (int i = 0; i < av.Length; i++)
        result[i] = av[i] / a;
      return new Vector(result);
    }

    /// <summary>Performs element-wise division of two vectors</summary>
    /// <param name="a">Numerator vector</param>
    /// <param name="b">Denominator vector</param>
    /// <returns>Result of scalar multiplication</returns>
    public static Vector operator /(Vector a, Vector b)
    {
      if (a.Length != b.Length)
        throw new InvalidOperationException("Cannot element-wise divide vectors of different length");
      double[] res = Vector.Zeros(a.Length);

      for (int i = 0; i < a.Length; i++)
      {
        if (b[i] == 0.0d)
          throw new DivideByZeroException("Cannot divide by zero");
        res[i] = a[i] / b[i];
      }

      return res;
    }

    /// <summary>
    /// Returns a vector each of whose elements is the maximal from the corresponding
    /// ones of argument vectors. Note that dimensions of the arguments must match.
    /// </summary>
    /// <param name="v1">First vector</param>
    /// <param name="v2">Second vector</param>
    /// <returns>vector v3 such that for each i = 0...dim(v1) v3[i] = max( v1[i], v2[i] )</returns>
    public static Vector Max(Vector v1, Vector v2)
    {
      double[] av1 = v1.v;
      double[] av2 = v2.v;

      if (av1 is null)
        throw new ArgumentNullException("v1");
      if (av2 is null)
        throw new ArgumentNullException("v2");

      if (av1.Length != av2.Length)
        throw new ArgumentException("Vector lengths do not match");
      var y = Vector.Zeros(av1.Length);
      for (int i = 0; i < av1.Length; i++)
        y[i] = Math.Max(av1[i], av2[i]);

      return y;
    }

    /// <summary>
    /// Returns a vector whose elements are the absolute values of the given vector elements
    /// </summary>
    /// <returns>Vector v1 such that for each i = 0...dim(v) v1[i] = |v[i]|</returns>
    public Vector Abs()
    {
      if (v is null)
        return new Vector();

      int n = v.Length;
      var y = Vector.Zeros(n);
      for (int i = 0; i < n; i++)
        y[i] = Math.Abs(v[i]);
      return y;
    }

    /// <summary>Convers vector to string representation.</summary>
    /// <returns>String consists from vector components separated by comma.</returns>
    public override string ToString()
    {
      var sb = new StringBuilder();
      sb.Append("[");
      if (v is not null)
        for (int i = 0; i < v.Length; i++)
        {
          if (i > 0)
            sb.Append(", ");
          sb.Append(v[i]);
        }
      sb.Append("]");
      return sb.ToString();
    }

    public override bool Equals(object? obj)
    {
      if (obj is Vector v2)
      {
        if (v2.Length != Length)
          return false;
        var av2 = v2.v;
        for (var i = 0; i < v.Length; i++)
          if (v[i] != av2[i])
            return false;
        return true;
      }
      else
        return false;
    }

    public override int GetHashCode()
    {
      return v is null ? base.GetHashCode() : v.GetHashCode();
    }
  }
}
