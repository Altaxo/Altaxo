using System.Collections.Generic;
using Altaxo.Calc.LinearAlgebra;

namespace Altaxo.Geometry
{
  /// <summary>
  /// Calculation of a projective matrix from 2D or 3D point pairs.
  /// </summary>
  public static class MatrixHomography
  {
    /// <summary>
    /// Evaluates a 2x2 matrix, that best transforms (in the least square sense) 1D points x into 1D points y.
    /// </summary>
    /// <param name="pointPairs">Pairs of points (x and y). The resulting 3x3 matrix should transform x into y.</param>
    /// <param name="setM22ToOne">If true, the resulting matrix is scaled so that the element M22 is 1.</param>
    /// <returns>A 2x2 matrix, that best transforms (in the least square sense) the points x into y.</returns>
    public static Matrix2x2 EvaluateHomography(IReadOnlyList<(double x, double y)> pointPairs, bool setM22ToOne = true)
    {
      var mx = new DoubleMatrix(pointPairs.Count * 2, 9);

      for (int i = 0, j = 0; i < pointPairs.Count; i++)
      {
        var x = pointPairs[i].x;
        var y = pointPairs[i].y;

        // X-Component
        mx[j, 0] = -x;
        mx[j, 1] = x * y;
        mx[j, 2] = -1;
        mx[j, 3] = y;
        ++j;
      }

      var decomposition = MatrixMath.GetSingularValueDecomposition(mx);

      // the solution of the homogeneous equation is located in the column of V that
      // corresponds to the smallest singular value
      // here the singular values are ordered, thus the solution is located in column 15 of V

      var v = decomposition.V;
      if (setM22ToOne)
      {
        var d = v[3][3];
        return new Matrix2x2(v[0][3] / d, v[1][3] / d, v[2][3] / d, 1);
      }
      else
      {
        return new Matrix2x2(v[0][3], v[1][3], v[2][3], v[3][3]);
      }
    }

    /// <summary>
    /// Transforms a 2D point with a 3D homography matrix.
    /// </summary>
    /// <param name="m">The matrix.</param>
    /// <param name="p">The point to transform.</param>
    /// <returns>The transformed point.</returns>
    public static double Transform(Matrix2x2 m, double p)
    {
      var pt = m.Transform(new PointD2D(p, 1));
      return pt.X / pt.Y;
    }

    /// <summary>
    /// Evaluates a 3x3 matrix, that best transforms (in the least square sense) 2D points x into 2D points y.
    /// </summary>
    /// <param name="pointPairs">Pairs of points (x and y). The resulting 3x3 matrix should transform x into y.</param>
    /// <param name="setM33ToOne">If true, the resulting matrix is scaled so that the element M33 is 1.</param>
    /// <returns>A 3x3 matrix, that best transforms (in the least square sense) the points x into y.</returns>
    public static Matrix3x3 EvaluateHomography(IReadOnlyList<(PointD2D x, PointD2D y)> pointPairs, bool setM33ToOne = true)
    {
      var mx = new DoubleMatrix(pointPairs.Count * 2, 9);

      for (int i = 0, j = 0; i < pointPairs.Count; i++)
      {
        var x = pointPairs[i].x;
        var y = pointPairs[i].y;

        // X-Component
        mx[j, 0] = -x.X;
        mx[j, 2] = x.X * y.X;
        mx[j, 3] = -x.Y;
        mx[j, 5] = x.Y * y.X;
        mx[j, 6] = -1;
        mx[j, 8] = y.X;
        ++j;

        // Y-Component
        mx[j, 1] = -x.X;
        mx[j, 2] = x.X * y.Y;
        mx[j, 4] = -x.Y;
        mx[j, 5] = x.Y * y.Y;
        mx[j, 7] = -1;
        mx[j, 8] = y.Y;
        ++j;
      }

      var decomposition = MatrixMath.GetSingularValueDecomposition(mx);

      // the solution of the homogeneous equation is located in the column of V that
      // corresponds to the smallest singular value
      // here the singular values are ordered, thus the solution is located in column 15 of V

      var v = decomposition.V;
      if (setM33ToOne)
      {
        var d = v[8][8];
        return new Matrix3x3(v[0][8] / d, v[1][8] / d, v[2][8] / d, v[3][8] / d, v[4][8] / d, v[5][8] / d, v[6][8] / d, v[7][8] / d, 1);
      }
      else
      {
        return new Matrix3x3(v[0][8], v[1][8], v[2][8], v[3][8], v[4][8], v[5][8], v[6][8], v[7][8], v[8][8]);
      }
    }


    /// <summary>
    /// Transforms a 2D point with a 3D homography matrix.
    /// </summary>
    /// <param name="m">The matrix.</param>
    /// <param name="p">The point to transform.</param>
    /// <returns>The transformed point.</returns>
    public static PointD2D Transform(Matrix3x3 m, PointD2D p)
    {
      var pt = m.Transform(new PointD3D(p.X, p.Y, 1));
      return new PointD2D(pt.X / pt.Z, pt.Y / pt.Z);
    }


    /// <summary>
    /// Evaluates a 4x4 matrix, that best transforms (in the least square sense) 3D points x into 3D points y.
    /// </summary>
    /// <param name="pointPairs">Pairs of points (x and y). The resulting 4x4 matrix should transform x into y.</param>
    /// <param name="setM44ToOne">If true, the resulting matrix is scaled so that the element M44 is 1.</param>
    /// <returns>A 4x4 matrix, that best transforms (in the least square sense) the points x into y.</returns>
    public static Matrix4x4 EvaluateHomography(IReadOnlyList<(PointD3D x, PointD3D y)> pointPairs, bool setM44ToOne = true)
    {
      var mx = new DoubleMatrix(pointPairs.Count * 3, 16);

      for (int i = 0, j = 0; i < pointPairs.Count; i++)
      {
        var x = pointPairs[i].x;
        var y = pointPairs[i].y;

        // X-Component
        mx[j, 0] = -x.X;
        mx[j, 3] = x.X * y.X;
        mx[j, 4] = -x.Y;
        mx[j, 7] = x.Y * y.X;
        mx[j, 8] = -x.Z;
        mx[j, 11] = x.Z * y.X;
        mx[j, 12] = -1;
        mx[j, 15] = y.X;
        ++j;

        // Y-Component
        mx[j, 1] = -x.X;
        mx[j, 3] = x.X * y.Y;
        mx[j, 5] = -x.Y;
        mx[j, 7] = x.Y * y.Y;
        mx[j, 9] = -x.Z;
        mx[j, 11] = x.Z * y.Y;
        mx[j, 13] = -1;
        mx[j, 15] = y.Y;
        ++j;

        // Z-Component
        mx[j, 2] = -x.X;
        mx[j, 3] = x.X * y.Z;
        mx[j, 6] = -x.Y;
        mx[j, 7] = x.Y * y.Z;
        mx[j, 10] = -x.Z;
        mx[j, 11] = x.Z * y.Z;
        mx[j, 14] = -1;
        mx[j, 15] = y.Z;
        ++j;
      }

      var decomposition = MatrixMath.GetSingularValueDecomposition(mx);

      // the solution of the homogeneous equation is located in the column of V that
      // corresponds to the smallest singular value
      // here the singular values are ordered, thus the solution is located in column 15 of V

      var v = decomposition.V;
      if (setM44ToOne)
      {
        var d = v[15][15];
        return new Matrix4x4(v[0][15] / d, v[1][15] / d, v[2][15] / d, v[3][15] / d, v[4][15] / d, v[5][15] / d, v[6][15] / d, v[7][15] / d, v[8][15] / d, v[9][15] / d, v[10][15] / d, v[11][15] / d, v[12][15] / d, v[13][15] / d, v[14][15] / d, 1);
      }
      else
      {
        return new Matrix4x4(v[0][15], v[1][15], v[2][15], v[3][15], v[4][15], v[5][15], v[6][15], v[7][15], v[8][15], v[9][15], v[10][15], v[11][15], v[12][15], v[13][15], v[14][15], v[15][15]);
      }
    }

    /// <summary>
    /// Transforms a 2D point with a 3D homography matrix.
    /// </summary>
    /// <param name="m">The matrix.</param>
    /// <param name="p">The point to transform.</param>
    /// <returns>The transformed point.</returns>
    public static PointD3D Transform(Matrix4x4 m, PointD3D p)
    {
      var pt = m.Transform(new VectorD4D(p.X, p.Y, p.Z, 1));
      return new PointD3D(pt.X / pt.W, pt.Y / pt.W, pt.Z/pt.W);
    }

  }
}

