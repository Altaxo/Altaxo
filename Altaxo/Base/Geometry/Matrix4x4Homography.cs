using System.Collections.Generic;
using Altaxo.Calc.LinearAlgebra;

namespace Altaxo.Geometry
{
  /// <summary>
  /// Calculation of a projective matrix from 3D point pairs.
  /// </summary>
  public static class Matrix4x4Homography
  {
    /// <summary>
    /// Evaluates a 4x4 matrix, that best transforms (in the least square sense) 3D points x into 3D points y.
    /// </summary>
    /// <param name="pointPairs">Pairs of points (x and y). The resulting 4x4 matrix should transform x into y.</param>
    /// <returns>A 4x4 matrix, that best transforms (in the least square sense) the points x into y.</returns>
    public static Matrix4x4 EvaluateHomography(IReadOnlyList<(PointD3D x, PointD3D y)> pointPairs, bool setM44ToOne=true)
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
  }
}

