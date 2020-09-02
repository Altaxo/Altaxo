#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2019 Dr. Dirk Lellinger
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
using System.Text;
using System.Threading.Tasks;

#nullable enable

namespace Altaxo.Calc.Clustering
{
  /// <summary>
  /// Clusters data using the KMeans algorithm.
  /// </summary>
  /// <typeparam name="TData">Type of the data.</typeparam>
  /// <typeparam name="TDataSum">Type of data that is used to calculate the mean by summing up and dividing by the cluster count.</typeparam>
  /// <remarks>
  /// References:
  /// KMeans algorithm: <see href="https://en.wikipedia.org/wiki/K-means_clustering"/>
  /// KMeans++ initialization: <see href="https://en.wikipedia.org/wiki/K-means%2B%2B"/>
  /// </remarks>
  public class KMeans<TData, TDataSum>
  {
    /// <summary>Contains the data.</summary>
    private TData[] _pointsData;

    /// <summary>
    /// For each of the data in array <see cref="_pointsData"/>, the array designates to which cluster the data currently belongs.
    /// </summary>
    private int[] _pointsClusterIdx;

    /// <summary>Stores the mean value of each cluster.</summary>
    private TDataSum[] _clusterMeans;

    /// <summary>Stores the number of data points in each cluster.</summary>
    private int[] _clusterCounts;

    /// <summary>
    /// The the default data.
    /// </summary>
    private Func<TDataSum> _createDefault;

    /// <summary>
    /// Function that gives the distance between the mean value (1st argument) and the data point (2nd argument).
    /// </summary>
    private Func<TDataSum, TData, double> _distanceFunction;

    /// <summary>
    /// True if the distance function provides the square of the distance, instead of the Euclidean distance.
    /// </summary>
    private bool _isDistanceFunctionSquareOfDistance;

    /// <summary>
    /// Function that gives the sum of the 1st argument and the 2nd argument.
    /// </summary>
    private Func<TDataSum, TData, TDataSum> _sumUpFunction;

    /// <summary>
    /// Function that gives the quotient of the first argument and the second argument.
    /// </summary>
    private Func<TDataSum, int, TDataSum> _divideFunction;

    /// <summary>
    /// If true, the evaluation has reached the maximum number of iterations, without converging.
    /// </summary>
    public bool HasReachedMaximumNumberOfIterations { get; protected set; }

    /// <summary>
    /// If true, during evaluation, empty clusters have appeared, which could not be patched with other data.
    /// </summary>
    public bool HasPatchingEmptyClustersFailed { get; protected set; }

    private SortDirection _sortingOfClusterValues = SortDirection.None;

    /// <summary>
    /// Get/sets the sorting of cluster values after evaluation. It presumes that the generic type TDataSum
    /// implements the <see cref="IComparable"/> interface.
    /// </summary>
    public SortDirection SortingOfClusterValues
    {
      get
      {
        return _sortingOfClusterValues;
      }
      set
      {
        if (value != SortDirection.None && !typeof(IComparable).IsAssignableFrom(typeof(TDataSum)))
          throw new ArgumentOutOfRangeException(nameof(SortingOfClusterValues), $"Sorting is only possible if data type {typeof(TDataSum)} implements interface {nameof(IComparable)}!");

        _sortingOfClusterValues = value;
      }
    }

    #region Properties

    /// <summary>
    /// Gets a list with the same length as the number of data points, in which each element is
    /// the index of the cluster this data point is assigned to.
    /// </summary>
    public IReadOnlyList<int> ClusterIndices => _pointsClusterIdx;

    /// <summary>
    /// Gets a list which contains the mean values of the clusters (length is numberOfClusters).
    /// </summary>
    public IReadOnlyList<TDataSum> ClusterMeans => _clusterMeans;

    /// <summary>
    /// Gets a list which contains the number of values in each of the clusters (length is numberOfClusters).
    /// </summary>
    public IReadOnlyList<int> ClusterCounts => _clusterCounts;

    /// <summary>
    /// Gets a list which contains the data points provided.
    /// </summary>
    public IReadOnlyList<TData> Data => _pointsData;

    #endregion

    /// <summary>
    /// Initalize a new instance of <see cref="KMeans{TData, TDataSum}"/>.
    /// </summary>
    /// <param name="createDefaultFunction">Creates the default data. Sum up the default data and other data should result in the same value of the other data.</param>
    /// <param name="distanceFunction">
    /// A function that evaluates the distance between the mean value of the cluster and a data point.
    /// The return value is either the Euclidean distance, or the square of the distance. In the latter case
    /// the next argument must be set to true.
    ///</param>
    /// <param name="isDistanceFunctionReturningSquareOfDistance">True if the <paramref name="distanceFunction"/> returns the square of the distance.</param>
    /// <param name="sumUpFunction">A function that adds the first and the second argument and returns the result.</param>
    /// <param name="divideFunction">
    /// A function that divides the first argument by the second argument. Calling this function
    /// signals that the sum up is finished. Because of this extra functionality, the divide function is called even if
    /// the divisor is 1.</param>
    /// <remarks>The function <paramref name="sumUpFunction"/> and <paramref name="divideFunction"/>
    /// have to work in such a way that
    /// <code>divideFunction(sumUpFunction(default, x), 1) == x</code>.
    /// </remarks>
    /// <example>
    /// <para>
    /// For taking the linear mean, the <paramref name="sumUpFunction"/> can be defined as
    /// <code>sumUpFunction = (sum, x) => sum + x</code>,
    /// and the divideFunction as
    /// <code>divideFunction = (nom, denom) => nom/denom.</code>
    /// </para>
    /// <para>
    /// For taking the logarithmic mean, the <paramref name="sumUpFunction"/> can be defined as
    /// <code>sumUpFunction = (sum, x) => sum + Math.Log(x)</code>,
    /// and the <paramref name="divideFunction"/> as
    /// <code>divideFunction = (nom, denom) => Math.Exp(nom/denom)</code>.
    /// The latter example works because the divideFunction is called in every case after summing up, even if the denominator is 1.
    /// </para>
    /// </example>
    public KMeans(
      Func<TDataSum> createDefaultFunction,
      Func<TDataSum, TData, double> distanceFunction,
      bool isDistanceFunctionReturningSquareOfDistance,
      Func<TDataSum, TData, TDataSum> sumUpFunction,
      Func<TDataSum, int, TDataSum> divideFunction
      )
    {
      _createDefault = createDefaultFunction ?? throw new ArgumentNullException(nameof(createDefaultFunction));
      _distanceFunction = distanceFunction ?? throw new ArgumentNullException(nameof(distanceFunction));
      _isDistanceFunctionSquareOfDistance = isDistanceFunctionReturningSquareOfDistance;
      _sumUpFunction = sumUpFunction ?? throw new ArgumentNullException(nameof(sumUpFunction));
      _divideFunction = divideFunction ?? throw new ArgumentNullException(nameof(divideFunction));

      _pointsData = new TData[0];
      _pointsClusterIdx = new int[0];
      _clusterMeans = new TDataSum[0];
      _clusterCounts = new int[0];

    }

    /// <summary>
    /// Clusters the provided data. Please note that the data are not normalized. Thus,
    /// for multidimensional data, please normalize the data before!
    /// </summary>
    /// <param name="data">The data points.</param>
    /// <param name="numberOfClusters">The number of clusters to create.</param>
    /// <exception cref="InvalidOperationException">If either the maximum number of iterations has reached without convergence,
    /// or empty clusters were created during evaluation, which could not be filled up.</exception>
    public void Evaluate(IEnumerable<TData> data, int numberOfClusters)
    {
      if (!TryEvaluate(data, numberOfClusters))
      {
        if (HasReachedMaximumNumberOfIterations)
          throw new InvalidOperationException($"{nameof(KMeans<TData, TDataSum>)} has reached maximum number of iterations without converging!");
        if (HasPatchingEmptyClustersFailed)
          throw new InvalidOperationException($"{nameof(KMeans<TData, TDataSum>)} had empty clusters that can not be patched! Try reduce number of clusters.");
      }
    }

    /// <summary>
    /// Clusters the provided data. Please note that the data are not normalized. Thus,
    /// for multidimensional data, please normalize the data before!
    /// </summary>
    /// <param name="data">The data points.</param>
    /// <param name="numberOfClusters">The number of clusters to create.</param>
    /// <returns>True if successful; otherwise, false.</returns>
    public bool TryEvaluate(IEnumerable<TData> data, int numberOfClusters)
    {
      if (data is null)
        throw new ArgumentNullException(nameof(data));
      _pointsData = data.ToArray();

      if (!(numberOfClusters > 1))
        throw new ArgumentOutOfRangeException(nameof(numberOfClusters), "Should be > 1");

      if (!(numberOfClusters <= _pointsData.Length))
        throw new ArgumentOutOfRangeException(nameof(numberOfClusters), $"Should be less than number of data points ({_pointsData.Length})");

      _pointsClusterIdx = new int[_pointsData.Length];
      _clusterMeans = new TDataSum[numberOfClusters];
      _clusterCounts = new int[numberOfClusters];


      InitializeCentroidsUsingKMeansPlusPlus();

      int maxIterations = _pointsData.Length * 10;
      int iteration = 0;
      bool changed = true;
      bool success = true;
      while (success == true && changed == true && iteration < maxIterations)
      {
        ++iteration;
        success = UpdateClusterMeanValues();
        if (!success)
        {
          success = PatchEmptyClusters();
        }

        changed = UpdateEachPointsMembership();
      }

      HasReachedMaximumNumberOfIterations = iteration >= maxIterations;
      HasPatchingEmptyClustersFailed = !success;


      // Sort the clusters by mean, if possible
      if (success && _sortingOfClusterValues != SortDirection.None && typeof(IComparable).IsAssignableFrom(typeof(TDataSum)))
      {
        SortClusters();
      }

      return success;
    }


    /// <summary>
    /// Initialize the centroids with a randomly choosen value from the data set.
    /// Initializing in this way gives poor clustering results, thus we don't use it currently.
    /// Advantage: much faster than <see cref="InitializeCentroidsUsingKMeansPlusPlus"/>.
    /// </summary>
    protected virtual void InitializeCentroidsAtRandom()
    {
      Random random = new Random();
      Array.Clear(_clusterCounts, 0, _clusterCounts.Length);
      for (int i = 0; i < _clusterCounts.Length; ++i)
      {
        _pointsClusterIdx[i] = i;
        ++_clusterCounts[_pointsClusterIdx[i]];
      }
      for (int i = _clusterCounts.Length; i < _pointsClusterIdx.Length; ++i)
      {
        _pointsClusterIdx[i] = random.Next(0, _clusterCounts.Length);
        ++_clusterCounts[_pointsClusterIdx[i]];
      }
    }
    /// <summary>
    /// Initializes the cluster mean values using the KMeans++ procedure (<see href="http://en.wikipedia.org/wiki/K-means%2B%2B"/>).
    /// This is slower than <see cref="InitializeCentroidsAtRandom"/>, but after initializing in this way, significant lesser iterations
    /// are neccessary.
    /// </summary>
    protected virtual void InitializeCentroidsUsingKMeansPlusPlus()
    {
      // Initialize using K-Means++
      // http://en.wikipedia.org/wiki/K-means%2B%2B

      // 1. Choose one center uniformly at random from among the data points.
      Random random = new Random();
      int idx = random.Next(0, _pointsData.Length);
      _clusterMeans[0] = _divideFunction(_sumUpFunction(_createDefault(), _pointsData[idx]), 1); // we use divideFunction here to signal that sumUp is finished

      var D = new double[_pointsData.Length];
      for (int c = 1; c < _clusterMeans.Length; c++)
      {
        // 2. For each data point x, compute D(x), the distance between
        // x and the nearest center that has already been chosen.

        double sumD = 0;
        for (int i = 0; i < D.Length; i++)
        {
          var x = _pointsData[i];
          double min = _distanceFunction(_clusterMeans[0], x);
          for (int j = 1; j < c; j++)
          {
            double d = _distanceFunction(_clusterMeans[j], x);
            if (min > d)
            {
              min = d;
            }
          }
          if (!_isDistanceFunctionSquareOfDistance)
          {
            min *= min;
          }

          D[i] = min;
          sumD += min;
        }
        for (int i = 0; i < D.Length; i++)
        {
          D[i] /= sumD; // normalize the probabilities, so that the sum of D is 1
        }

        // 3. Choose one new data point at random as a new center, using a weighted
        // probability distribution where a point x is chosen with probability
        // proportional to D(x)^2.
        double uniformRandomVariable = random.NextDouble();
        double cumulativeSum = 0;
        for (idx = 0; idx < D.Length - 1; ++idx)
        {
          cumulativeSum += D[idx];
          if (cumulativeSum > uniformRandomVariable)
            break;
        }
        _clusterMeans[c] = _divideFunction(_sumUpFunction(_createDefault(), _pointsData[idx]), 1);
      }


      // now initialize each points membership
      // according to the initialized centroids
      for (int i = 0; i < _pointsData.Length; ++i)
      {
        int minIndex = 0;
        double minDistance = double.PositiveInfinity;
        for (int j = 0; j < _clusterMeans.Length; ++j)
        {
          var distance = _distanceFunction(_clusterMeans[j], _pointsData[i]);
          if (minDistance > distance)
          {
            minDistance = distance;
            minIndex = j;
          }
        }
        ++_clusterCounts[minIndex];
        _pointsClusterIdx[i] = minIndex;
      }
    }

    /// <summary>
    /// Calculates the mean values of the clusters from the data points.
    /// </summary>
    /// <returns>True if successfull. False if unsuccessful, i.e. there are some empty clusters, and those clusters could not
    /// be filled with other data.</returns>
    protected bool UpdateClusterMeanValues()
    {
      for (int i = 0; i < _clusterMeans.Length; ++i)
      {
        _clusterMeans[i] = _createDefault();
      }

      for (int i = 0; i < _pointsClusterIdx.Length; ++i)
      {
        _clusterMeans[_pointsClusterIdx[i]] = _sumUpFunction(_clusterMeans[_pointsClusterIdx[i]], _pointsData[i]);
      }

      var hasEmptyClusters = false;
      for (int i = 0; i < _clusterMeans.Length; ++i)
      {
        var count = _clusterCounts[i];
        hasEmptyClusters |= (0 == count);
        _clusterMeans[i] = _divideFunction(_clusterMeans[i], count);
      }
      return !hasEmptyClusters;
    }

    /// <summary>
    /// Iterates over each data point, and determines to which cluster it belongs.
    /// </summary>
    /// <returns>True if at least one data point has changed its cluster membership; otherwise, false.</returns>
    protected bool UpdateEachPointsMembership()
    {
      bool hasChanged = false;
      for (int i = 0; i < _pointsData.Length; ++i)
      {
        int minIndex = 0;
        double minDistance = double.PositiveInfinity;
        for (int j = 0; j < _clusterMeans.Length; ++j)
        {
          var distance = _distanceFunction(_clusterMeans[j], _pointsData[i]);
          if (minDistance > distance)
          {
            minDistance = distance;
            minIndex = j;
          }
        }

        if (minIndex != _pointsClusterIdx[i])
        {
          --_clusterCounts[_pointsClusterIdx[i]];
          ++_clusterCounts[minIndex];
          _pointsClusterIdx[i] = minIndex;
          hasChanged = true;
        }
      }
      return hasChanged;
    }

    /// <summary>
    /// Tries to fill up empty clusters, by searching the biggest cluster, and then use the farthest point
    /// from that biggest cluster to move to the empty cluster.
    /// </summary>
    /// <returns></returns>
    protected bool PatchEmptyClusters()
    {
      for (int ic = 0; ic < _clusterCounts.Length; ++ic)
      {
        if (_clusterCounts[ic] == 0)
        {
          // Stategy: search the farthest point of the biggest cluster
          // and then move this point into the empty cluster
          int ip = GetFarthestPointOfBiggestCluster();

          if (ip < 0)
            return false;

          var oldClusterIdx = _pointsClusterIdx[ip];

          --_clusterCounts[oldClusterIdx];
          ++_clusterCounts[ic];
          _pointsClusterIdx[ip] = ic;

          // update the mean value of the new cluster
          _clusterMeans[ic] = _divideFunction(_sumUpFunction(_createDefault(), _pointsData[ip]), 1);

          // update the mean value of the old cluster
          _clusterMeans[oldClusterIdx] = _createDefault();
          for (int k = 0; k < _pointsClusterIdx.Length; ++k)
          {
            if (_pointsClusterIdx[k] == oldClusterIdx)
            {
              _clusterMeans[oldClusterIdx] = _sumUpFunction(_clusterMeans[oldClusterIdx], _pointsData[k]);
            }
          }
          _clusterMeans[oldClusterIdx] = _divideFunction(_clusterMeans[oldClusterIdx], _clusterCounts[oldClusterIdx]);
        }
      }
      return true;
    }

    /// <summary>
    /// Returns the index of the point that is farthest outside the mean of its cluster.
    /// </summary>
    /// <returns>Index of the point that is farthest outside the mean of its cluster, or -1 if no such point exists.</returns>
    protected int GetFarthestPoint()
    {
      int max_i = -1;
      double max_d = double.NegativeInfinity;
      for (int i = 0; i < _pointsData.Length; ++i)
      {
        var clusterIdx = _pointsClusterIdx[i];
        if (_clusterCounts[clusterIdx] >= 2)
        {
          var d = _distanceFunction(_clusterMeans[clusterIdx], _pointsData[i]);
          if (max_d < d)
          {
            max_d = d;
            max_i = i;
          }
        }
      }
      return max_i;
    }

    /// <summary>
    /// Returns the index of the point that is farthest outside the mean of its cluster.
    /// </summary>
    /// <returns>Index of the point that is farthest outside the mean of its cluster, or -1 if no such point exists.</returns>
    protected int GetFarthestPointOfBiggestCluster()
    {
      int idxBiggestCluster = -1;
      int biggestClusterCount = 1; // look for clusters with at least 2 points
      for (int i = 0; i < _clusterCounts.Length; ++i)
      {
        if (biggestClusterCount < _clusterCounts[i])
        {
          biggestClusterCount = _clusterCounts[i];
          idxBiggestCluster = i;
        }
      }


      int max_i = -1;
      double max_d = double.NegativeInfinity;
      for (int i = 0; i < _pointsData.Length; ++i)
      {
        var clusterIdx = _pointsClusterIdx[i];
        if (clusterIdx == idxBiggestCluster)
        {
          var d = _distanceFunction(_clusterMeans[clusterIdx], _pointsData[i]);
          if (max_d < d)
          {
            max_d = d;
            max_i = i;
          }
        }
      }
      return max_i;
    }

    #region Helper functions for result output

    /// <summary>
    /// Sorts the cluster values, if cluster data are sortable (if they implement <see cref="IComparable"/>).
    /// </summary>
    private void SortClusters()
    {
      var oldIndices = new int[_clusterMeans.Length];
      for (int i = 0; i < oldIndices.Length; ++i)
        oldIndices[i] = i;

      if (_sortingOfClusterValues == SortDirection.Ascending)
        Array.Sort(_clusterMeans, oldIndices);
      else if (_sortingOfClusterValues == SortDirection.Descending)
        Array.Sort(_clusterMeans, oldIndices, Comparer<TDataSum>.Create((x, y) => Comparer<TDataSum>.Default.Compare(y, x)));

      // invert oldIndices
      var newIndices = new int[_clusterMeans.Length];
      for (int i = 0; i < oldIndices.Length; ++i)
        newIndices[oldIndices[i]] = i;

      // update cluster indices and cluster count
      Array.Clear(_clusterCounts, 0, _clusterCounts.Length);
      for (int i = 0; i < _pointsClusterIdx.Length; ++i)
      {
        _pointsClusterIdx[i] = newIndices[_pointsClusterIdx[i]];
        ++_clusterCounts[_pointsClusterIdx[i]];
      }
    }

    /// <summary>
    /// Evaluates for each cluster the standard deviation, i.e. the square root ( of the sum of squared distances divided by N-1)
    /// </summary>
    /// <returns>The standard deviation of each cluster, i.e. the square root ( of the sum of squared distances divided by N-1)</returns>
    public IReadOnlyList<double> EvaluateClustersStandardDeviation()
    {
      var stdDev = new double[_clusterMeans.Length];

      for (int ic = 0; ic < _clusterMeans.Length; ++ic)
      {
        var mean = _clusterMeans[ic];

        double sum = 0;
        int count = 0;
        for (int jp = 0; jp < _pointsData.Length; ++jp)
        {
          if (_pointsClusterIdx[jp] == ic)
          {
            ++count;
            var x = _distanceFunction(mean, _pointsData[jp]);
            if (_isDistanceFunctionSquareOfDistance)
              sum += x;
            else
              sum += x * x;
          }
        }
        stdDev[ic] = Math.Sqrt(sum / (count - 1));
      }
      return stdDev;
    }


    /// <summary>
    /// Evaluates for each cluster the mean distance, i.e. the square root ( of the sum of squared distances divided by N)
    /// </summary>
    /// <returns>The mean distance in each cluster, i.e. the square root ( of the sum of squared distances divided by N)</returns>
    public IReadOnlyList<double> EvaluateMeanNthMomentOfDistances(int q)
    {
      if (q == 1)
        return EvaluateMeanDistances();
      else if (q == 2)
        return EvaluateMean2ndMomentOfDistances();
      else
        return InternalEvaluateMeanNthMomentOfDistances(q);
    }

    /// <summary>
    /// Evaluates for each cluster the mean distance, i.e. the average of the Euclidean distances (or whatever the distance function is) of the points to their respective centroid.
    /// </summary>
    /// <returns>The mean distance in each cluster, i.e. the mean of the Euclidean distances (or whatever the distance function is) of the points to their respective centroid</returns>
    public IReadOnlyList<double> EvaluateMeanDistances()
    {
      var stdDev = new double[_clusterMeans.Length];

      for (int ic = 0; ic < _clusterMeans.Length; ++ic)
      {
        var mean = _clusterMeans[ic];

        double sum = 0;
        int count = 0;
        for (int jp = 0; jp < _pointsData.Length; ++jp)
        {
          if (_pointsClusterIdx[jp] == ic)
          {
            ++count;
            var x = _distanceFunction(mean, _pointsData[jp]);
            if (_isDistanceFunctionSquareOfDistance)
              sum += Math.Sqrt(x);
            else
              sum += x;
          }
        }
        stdDev[ic] = sum / (count);
      }
      return stdDev;
    }

    /// <summary>
    /// Evaluates for each cluster the 2nd moment of the distances, i.e. the square root of the average of the squared Euclidean distances (or whatever the distance function is) of the points to their respective centroid.
    /// </summary>
    /// <returns>The 2nd moment of the distances in each cluster, i.e. the square root of the average of the squared Euclidean distances (or whatever the distance function is) of the points to their respective centroid</returns>
    public IReadOnlyList<double> EvaluateMean2ndMomentOfDistances()
    {
      var stdDev = new double[_clusterMeans.Length];

      for (int ic = 0; ic < _clusterMeans.Length; ++ic)
      {
        var mean = _clusterMeans[ic];

        double sum = 0;
        int count = 0;
        for (int jp = 0; jp < _pointsData.Length; ++jp)
        {
          if (_pointsClusterIdx[jp] == ic)
          {
            ++count;
            var x = _distanceFunction(mean, _pointsData[jp]);
            if (_isDistanceFunctionSquareOfDistance)
              sum += x;
            else
              sum += x * x;
          }
        }
        stdDev[ic] = Math.Sqrt(sum / (count));
      }
      return stdDev;
    }

    /// <summary>
    /// Evaluates for each cluster the 2nd moment of the distances, i.e. the square root of the average of the squared Euclidean distances (or whatever the distance function is) of the points to their respective centroid.
    /// </summary>
    /// <returns>The 2nd moment of the distances in each cluster, i.e. the square root of the average of the squared Euclidean distances (or whatever the distance function is) of the points to their respective centroid</returns>
    protected IReadOnlyList<double> InternalEvaluateMeanNthMomentOfDistances(int q)
    {
      var stdDev = new double[_clusterMeans.Length];

      for (int ic = 0; ic < _clusterMeans.Length; ++ic)
      {
        var mean = _clusterMeans[ic];

        double sum = 0;
        int count = 0;
        for (int jp = 0; jp < _pointsData.Length; ++jp)
        {
          if (_pointsClusterIdx[jp] == ic)
          {
            ++count;
            var x = _distanceFunction(mean, _pointsData[jp]);
            if (_isDistanceFunctionSquareOfDistance)
              sum += RMath.Pow(Math.Sqrt(x), q);
            else
              sum += RMath.Pow(x, q);
          }
        }
        stdDev[ic] = Math.Pow(sum / (count), 1.0 / q);
      }
      return stdDev;
    }


    /// <summary>
    /// Evaluates the sum of (squared distance of each point to its cluster center).
    /// </summary>
    /// <returns>Sum of (squared distance of each point to its cluster center).</returns>
    public double EvaluateSumOfSquaredDistancesToClusterMean()
    {
      double sum = 0;
      for (int jp = 0; jp < _pointsData.Length; ++jp)
      {
        var mean = _clusterMeans[_pointsClusterIdx[jp]];
        var x = _distanceFunction(mean, _pointsData[jp]);
        if (_isDistanceFunctionSquareOfDistance)
          sum += x;
        else
          sum += x * x;
      }
      return sum;
    }

    /// <summary>
    /// Evaluates the Davies-Bouldin-Index. The exponent q (see <see cref="EvaluateDaviesBouldinIndex(Func{TDataSum, TDataSum, double}, int)"/>) is set to 1,
    /// meaning that the mean Euclidean distance of the points to their respective centroid is used in the nominator.
    /// </summary>
    /// <param name="distanceFunctionOfClusterMeans">Function to calculate the Euclidean distance between two cluster centroids.</param>
    /// <returns>The Davies-Bouldin-Index using q=1 (p=2 if <paramref name="distanceFunctionOfClusterMeans"/> returns the Euclidean distance between the cluster centroids).</returns>
    public double EvaluateDaviesBouldinIndex(Func<TDataSum, TDataSum, double> distanceFunctionOfClusterMeans)
    {
      return EvaluateDaviesBouldinIndex(distanceFunctionOfClusterMeans, 1);
    }

    /// <summary>
    /// Evaluates the Davies-Bouldin-Index. The exponent q (used to calculate the average distance of the cluster points to their centroid) can be set as parameter.
    /// </summary>
    /// <param name="distanceFunctionOfClusterMeans">Function to calculate the Euclidean distance between two cluster centroids.</param>
    /// <param name="q">Order of the moment to calculate the average distance of the cluster points to their centroid. A value of one results in
    /// calculating the average (Euclidean) distance, a value of 2 results in the square root of the mean squared distances, etc.</param>
    /// <returns>The Davies-Bouldin-Index using the exponent q (p is implicitely 2 if <paramref name="distanceFunctionOfClusterMeans"/> returns the Euclidean distance between the cluster centroids).</returns>
    public double EvaluateDaviesBouldinIndex(Func<TDataSum, TDataSum, double> distanceFunctionOfClusterMeans, int q)
    {
      var meanDistances = EvaluateMeanNthMomentOfDistances(q);

      double sumDi = 0;

      for (int i = 0; i < _clusterMeans.Length; ++i)
      {
        double maxRij = double.NegativeInfinity;
        for (int j = 0; j < _clusterMeans.Length; ++j)
        {
          if (i == j)
            continue;

          double rij = meanDistances[i] + meanDistances[j];
          rij /= distanceFunctionOfClusterMeans(_clusterMeans[i], _clusterMeans[j]);
          maxRij = Math.Max(maxRij, rij);
        }
        sumDi += maxRij;
      }
      return sumDi / _clusterMeans.Length;
    }

    #endregion
  }



  /// <summary>
  /// Specialized <see cref="KMeans{TData, TDataSum}"/> for one dimensional double values.
  /// </summary>
  public class KMeans_Double1D : KMeans<double, double>
  {
    public KMeans_Double1D()
      :
      base(
        () => 0.0,
        (x, y) => Math.Abs(x - y),  // distance function
        false,                      // distance is Euclidean distance
        (sum, x) => sum + x,        // sum up function
        (sum, count) => sum / count // divide function
        )
    {
      SortingOfClusterValues = SortDirection.Ascending;
    }
  }


}
