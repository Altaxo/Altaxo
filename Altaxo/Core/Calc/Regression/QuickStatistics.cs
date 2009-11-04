using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Calc.Regression
{
	/// <summary>
	/// Uses the method of running sums to calculate mean, sample standard deviation, and standard deviation of a data set.
	/// </summary>
	/// <remarks>See wikipedia article "Standard deviation".</remarks>
  public class QuickStatistics
  {
    long _n;
		double _ai;
		double _qi;

    /// <summary>
    /// Initializes the statistics.
    /// </summary>
    public void Clear()
    {
      _n = 0;
			_ai = 0;
			_qi = 0;
    }

    /// <summary>
    /// Adds a data point to the statistics.
    /// </summary>
    /// <param name="x">The value of the data point.</param>
    public void Add(double x)
    {
      _n++;
			double aim1 = _ai;
			double qim1 = _qi;
			_ai = aim1 + (x - aim1) / _n;
			_qi = qim1 + (x - aim1) * (x - _ai);
    }

		/// <summary>
		/// Returns the mean (average) of the data values.
		/// </summary>
		public double Mean
		{
			get
			{
				return _ai;
			}
		}


    /// <summary>
    /// Returns the number of data values that were added.
    /// </summary>
    public double N
    {
      get
      {
        return _n;
      }
    }

		/// <summary>
		/// Returns the sample standard deviation: square root of the error sum of squares divided by (N-1).
		/// </summary>
		public double SampleStandardDeviation
		{
			get
			{
				return _n > 1 ? Math.Sqrt(_qi / (_n - 1)) : double.NaN;
			}
		}



		/// <summary>
		/// Returns the standard deviation: square root of the error sum of squares divided by N.
		/// </summary>
		public double StandardDeviation
		{
			get
			{
				return _n > 0 ? Math.Sqrt(_qi / (_n)) : double.NaN;
			}
		}


  }
}
