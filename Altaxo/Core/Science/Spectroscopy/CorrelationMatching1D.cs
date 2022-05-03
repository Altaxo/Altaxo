#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2022 Dr. Dirk Lellinger
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

#endregion Copyright

using System;
using System.Collections.Generic;
using System.Linq;

namespace Altaxo.Science.Spectroscopy
{
  /// <summary>
  /// Correlation matching of feature points in one dimension. Can be used for instance to
  /// correlate peak positions in two spectra.
  /// </summary>
  public class CorrelationMatching1D
  {
    private double[] _array1;
    private double[] _array2;
    private int _windowSize;
    private double _distanceMax;
    NormalizationMethod _normalization = NormalizationMethod.Variance;

    /// <summary>
    /// Designates the normalization method for the points in the windows, just before the correlation value was evaluated.
    /// </summary>
    [Flags]
    public enum NormalizationMethod
    {
      /// <summary>
      /// From the window values, the mean value of the window values is subtracted.
      /// </summary>
      Mean = 0x01,

      /// <summary>
      /// The window values are divided by the variance of the window values.
      /// </summary>
      Variance = 0x02,
    }

    /// <summary>
    ///   Gets or sets the maximum distance to consider points as correlated.
    ///   Default is 0 (consider all points).
    /// </summary>
    public double DistanceMax
    {
      get { return _distanceMax; }
      set { _distanceMax = value; }
    }

    /// <summary>
    ///   Gets or sets the size of the correlation window.
    /// </summary>
    public int WindowSize
    {
      get { return _windowSize; }
      set { _windowSize = value; }
    }

    /// <summary>
    /// Gets or sets the normalization treatment of the values in the windows, before the correlation value is calculated.
    /// Default is <see cref="NormalizationMethod.Variance"/>
    /// </summary>
    public NormalizationMethod Normalization
    {
      get => _normalization;
      set => _normalization = value;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CorrelationMatching1D"/> class.
    /// </summary>
    /// <param name="windowSize">Size of the correlation window.</param>
    /// <param name="array1">The first array (for instance: a spectrum).</param>
    /// <param name="array2">The second array (for instance: a spectrum). </param>
    /// <exception cref="System.ArgumentException">Window size should be odd - windowSize</exception>
    public CorrelationMatching1D(int windowSize, double[] array1, double[] array2)
        : this(windowSize, 0, array1, array2)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CorrelationMatching1D"/> class.
    /// </summary>
    /// <param name="windowSize">Size of the correlation window.</param>
    /// <param name="maxDistance">The maximum distance to consider points as correlated.</param>
    /// <param name="array1">The first array (for instance, a spectrum).</param>
    /// <param name="array2">The second array (for instance, a spectrum). </param>
    /// <exception cref="System.ArgumentException">Window size should be odd - windowSize</exception>
    public CorrelationMatching1D(int windowSize, double maxDistance, double[] array1, double[] array2)
    {
      if (windowSize <1 || windowSize % 2 == 0)
        throw new ArgumentException("Window size should be positive and odd", nameof(windowSize));

      _array1 = array1;
      _array2 = array2;

      _windowSize = windowSize;
      _distanceMax = maxDistance;
    }

    /// <summary>
    ///  Matches two sets of feature points computed from the given arrays (for instance, peak positions).
    /// </summary>
    /// <param name="points1">Set of feature points for the first array.</param>
    /// <param name="points2">Set of feature points for the second array.</param>
    public int[][] Match(IEnumerable<int> points1, IEnumerable<int> points2)
    {
      return Match(points1.ToArray(), points2.ToArray());
    }

    /// <summary>
    ///   Matches two sets of feature points computed from the given arrays.
    /// </summary>
    /// <param name="points1">Feature points of the first array (that was given in the constructor).</param>
    /// <param name="points2">Feature points of the second array (that was given in the constructor).</param>
    /// <returns>A Nx2 matrix of correlating feature points.</returns>
    public int[][] Match(int[] points1, int[] points2)
    {
      // Generate correlation matrix
      double[,] correlationMatrix =
          EvaluateCorrelationMatrix(_array1, points1, _array2, points2, _windowSize, _distanceMax, _normalization);


      // Select points with maximum correlation measures
      int[] colp2forp1 = Matrix.ArgMax(correlationMatrix, 1);
      int[] rowp1forp2 = Matrix.ArgMax(correlationMatrix, 0);

      // Construct the lists of matched point indices
      int rows = correlationMatrix.GetLength(0);
      List<int> p1ind = new List<int>();
      List<int> p2ind = new List<int>();

      // For each point in the first set of points,
      for (int i = 0; i < rows; i++)
      {
        // Get the point j in the second set of points with which
        // this point i has a maximum correlation measure. (i->j)
        int j = colp2forp1[i];

        // Now, check if this point j in the second set also has
        // a maximum correlation measure with the point i. (j->i)
        if (rowp1forp2[j] == i)
        {
          // The points are consistent. Ensure they are valid.
          if (correlationMatrix[i, j] != double.NegativeInfinity)
          {
            // We have a corresponding pair (i,j)
            p1ind.Add(i);
            p2ind.Add(j);
          }
        }
      }

      // Extract matched points from original arrays
      var m1 = Matrix.Get(points1, p1ind.ToArray());
      var m2 = Matrix.Get(points2, p2ind.ToArray());

      // Create matching point pairs
      return new int[][] { m1, m2 };
    }


    /// <summary>
    ///   Constructs the correlation matrix between selected points from two linear arrays.
    /// </summary>
    /// 
    /// <remarks>
    ///   Rows correspond to points from the first array, columns correspond to points
    ///   in the second array
    /// </remarks>
    /// 
    private static double[,] EvaluateCorrelationMatrix(
        double[] image1, int[] points1,
        double[] image2, int[] points2,
        int windowSize, double maxDistance, NormalizationMethod normalization)
    {

      // Create the initial correlation matrix
      double[,] matrix = new double[points1.Length, points2.Length];
      for (int i = 0; i < points1.Length; i++)
        for (int j = 0; j < points2.Length; j++)
          matrix[i, j] = double.NegativeInfinity;

      // Gather some information
      int width1 = image1.Length;
      int width2 = image2.Length;


      int r = (windowSize - 1) / 2; //  'radius' of correlation window
      double maxDistanceSqr = maxDistance * maxDistance; // maximum considered distance
      double[] w1 = new double[windowSize]; // first window
      double[] w2 = new double[windowSize]; // second window

      // We will ignore points at the edge
      int[] idx1 = Matrix.Find(points1, p => p >= r && p < width1 - r);
      int[] idx2 = Matrix.Find(points2, p => p >= r && p < width2 - r);


      // For each index in the first set of points
      foreach (int n1 in idx1)
      {
        // Get the current point
        int p1 = points1[n1];
        double sumy = 0, sumyy=0;
        for (int i = 0; i < windowSize; i++)
        {
          double w = image1[p1 - r + i];
          w1[i] = w;
          sumy += w;
          sumyy += w * w;
        }
        // Normalize the window
        Normalize(w1, w1.Length, sumy, sumyy, normalization);

        // Identify the indices of points in p2 that we need to consider.
        int[] candidates;
        if (maxDistance == 0)
        {
          // We should consider all points
          candidates = idx2;
        }
        else
        {
          // We should consider points that are within
          //  distance maxDistance apart

          // Compute distances from the current point
          //  to all points in the second image.
          double[] distances = new double[idx2.Length];
          for (int i = 0; i < idx2.Length; i++)
          {
            double dx = p1 - points2[idx2[i]];
            distances[i] = dx * dx;
          }

          candidates = Matrix.Get(idx2, Matrix.Find(distances, d => d < maxDistanceSqr));
        }


        // Calculate normalized correlation measure
        foreach (int n2 in candidates)
        {
          sumy = 0; sumyy = 0;
          int p2 = points2[n2];
          for (int i = 0; i < windowSize; i++)
          {
            double w = image2[p2 - r + i];
            w2[i] = w;
            sumy += w;
            sumyy += w * w;
          }
          Normalize(w2, w2.Length, sumy, sumyy, normalization);

          double sum1 = 0;
          for (int i = 0; i < windowSize; i++)
          {
            sum1 += w1[i] * w2[i];
          }

          matrix[n1, n2] = sum1;
        }
      }

      return matrix;
    }


    #region Helpers

    public static void Normalize(double[] y, int count, double sy, double syy, NormalizationMethod normalization)
    {
      double offset = 0;
      double scale = 1;

      if (normalization.HasFlag(NormalizationMethod.Mean))
      {
        offset = sy / count;
      }
      if(normalization.HasFlag(NormalizationMethod.Variance))
      {
        scale = normalization.HasFlag(NormalizationMethod.Mean) ?
                  1 / Math.Sqrt((syy - sy * sy / count) / count) :
                  1 / Math.Sqrt(syy / count);

      }
      for(int i=0;i<y.Length;++i)
      {
        y[i] = (y[i] - offset) * scale;
      }
    }

    private class Matrix
    {

      public static int[] Find<T>(T[] data, Func<T, bool> func)
      {
        List<int> idx = new List<int>();

        for (int i = 0; i < data.Length; i++)
          if (func(data[i]))
            idx.Add(i);

        return idx.ToArray();
      }


      /// <summary>
      ///   Returns a subvector extracted from the current vector.
      /// </summary>
      /// 
      /// <param name="source">The vector to return the subvector from.</param>
      /// <param name="indexes">Array of indices.</param>
      /// <param name="inPlace">True to return the results in place, changing the
      ///   original <paramref name="source"/> vector; false otherwise.</param>
      /// 
      public static T[] Get<T>(T[] source, int[] indexes, bool inPlace = false)
      {
        if (source is null)
          throw new ArgumentNullException(nameof(source));

        if (indexes is null)
          throw new ArgumentNullException(nameof(indexes));

        if (inPlace && source.Length != indexes.Length)
          throw new ArgumentException("Source and indexes arrays must have the same dimension for in-place operations.");

        var destination = new T[indexes.Length];
        for (int i = 0; i < indexes.Length; i++)
        {
          int j = indexes[i];
          if (j >= 0)
            destination[i] = source[j];
          else
            destination[i] = source[source.Length + j];
        }

        if (inPlace)
        {
          for (int i = 0; i < destination.Length; i++)
            source[i] = destination[i];
        }

        return destination;
      }

      /// <summary>
      ///   Gets the index of the maximum element in a matrix across a given dimension.
      /// </summary>
      /// 
      public static int[] ArgMax<T>(T[,] matrix, int dimension)
          where T : IComparable<T>
      {
        int s = GetLength(matrix, dimension);
        var values = new T[s];
        var indices = new int[s];
        Max(matrix, dimension, indices, values);
        return indices;
      }

      static int GetLength<T>(T[,] values, int dimension)
      {
        return dimension == 1 ? values.GetLength(0) : values.GetLength(1);
      }

      /// <summary>
      ///   Gets the maximum values across one dimension of a matrix.
      /// </summary>
      /// 
      public static T[] Max<T>(T[,] matrix, int dimension, int[] indices, T[] result)
          where T : IComparable<T>
      {
        if (dimension == 1) // Search down columns
        {
          GetColumn(matrix, 0, result: result);
          for (int j = 0; j < matrix.GetLength(0); j++)
          {
            for (int i = 0; i < matrix.GetLength(1); i++)
            {
              if (matrix[j, i].CompareTo(result[j]) > 0)
              {
                result[j] = matrix[j, i];
                indices[j] = i;
              }
            }
          }
        }
        else
        {
          GetRow(matrix, 0, result: result);
          for (int j = 0; j < matrix.GetLength(1); j++)
          {
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
              if (matrix[i, j].CompareTo(result[j]) > 0)
              {
                result[j] = matrix[i, j];
                indices[j] = i;
              }
            }
          }
        }

        return result;
      }

      /// <summary>
      ///   Gets a column vector from a matrix.
      /// </summary>
      /// 
      public static T[] GetColumn<T>(T[,] m, int index, T[] result = null)
      {
        if (result is null)
          result = new T[Rows(m)];

        index = Matrix.Index(index, Columns(m));
        for (int i = 0; i < result.Length; i++)
          result[i] = m[i, index];

        return result;
      }

      /// <summary>
      ///   Gets a row vector from a matrix.
      /// </summary>
      ///
      public static T[] GetRow<T>(T[,] m, int index, T[] result = null)
      {
        if (result is null)
          result = new T[m.GetLength(1)];

        index = Matrix.Index(index, Rows(m));
        for (int i = 0; i < result.Length; i++)
          result[i] = m[index, i];

        return result;
      }

      /// <summary>
      ///   Gets the number of rows in a multidimensional matrix.
      /// </summary>
      /// 
      /// <typeparam name="T">The type of the elements in the matrix.</typeparam>
      /// <param name="matrix">The matrix whose number of rows must be computed.</param>
      /// 
      /// <returns>The number of rows in the matrix.</returns>
      /// 
      public static int Rows<T>(T[,] matrix)
      {
        return matrix.GetLength(0);
      }

      /// <summary>
      ///   Gets the number of columns in a multidimensional matrix.
      /// </summary>
      /// 
      /// <typeparam name="T">The type of the elements in the matrix.</typeparam>
      /// <param name="matrix">The matrix whose number of columns must be computed.</param>
      /// 
      /// <returns>The number of columns in the matrix.</returns>
      /// 
      public static int Columns<T>(T[,] matrix)
      {
        return matrix.GetLength(1);
      }

      private static int Index(int end, int length)
      {
        if (end < 0)
          end = length + end;
        return end;
      }

    }
    #endregion

  }
}
