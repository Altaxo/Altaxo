#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
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
//    along with ctrl program; if not, write to the Free Software
//    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
//
/////////////////////////////////////////////////////////////////////////////

#endregion Copyright

using Altaxo.Calc;
using Altaxo.Collections;
using Altaxo.Data;
using Altaxo.Gui.Worksheet.Viewing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Data
{
	/// <summary>
	/// Options to create histograms.
	/// </summary>
	public class HistogramCreationOptions : ICloneable
	{
		/// <summary>
		/// Gets or sets a value indicating whether to ignore NaN (Not-A-Number) values.
		/// </summary>
		/// <value>
		///   <c>true</c> to ignore NaN values ; otherwise, <c>false</c>.
		/// </value>
		public bool IgnoreNaN { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether to ignore infinite values.
		/// </summary>
		/// <value>
		///   <c>true</c> to ignore infinite values; otherwise, <c>false</c>.
		/// </value>
		public bool IgnoreInfinity { get; set; }

		/// <summary>
		/// If not null, all values less than that value are ignored. Use <see cref="IsLowerBoundaryInclusive"/> to determine whether this boundary is inclusive or exclusive.
		/// </summary>
		/// <value>
		/// The lower boundary to ignore.
		/// </value>
		public double? LowerBoundaryToIgnore;

		/// <summary>
		/// Designates whether the lower boundary is inclusive. If <c>true</c>, all values less than or equal to <see cref="LowerBoundaryToIgnore"/> are ignored. If <c>false</c>, only values less than <see cref="LowerBoundaryToIgnore"/> are ignored.
		/// </summary>
		public bool IsLowerBoundaryInclusive;

		/// <summary>
		/// If not null, all values greater than that value are ignored. Use <see cref="IsUpperBoundaryInclusive"/> to determine whether this boundary is inclusive or exclusive.
		/// </summary>
		/// <value>
		/// The upper boundary to ignore.
		/// </value>
		public double? UpperBoundaryToIgnore { get; set; }

		/// <summary>
		/// Designates whether the upper is boundary inclusive. If <c>true</c>, all values greater than or equal to <see cref="UpperBoundaryToIgnore"/> are ignored. If <c>false</c>, only values greater than <see cref="UpperBoundaryToIgnore"/> are ignored.
		/// </summary>
		public bool IsUpperBoundaryInclusive;

		/// <summary>
		/// Gets or sets a value indicating whether the binning type is defined by the user or is determined automatically.
		/// </summary>
		/// <value>
		/// <c>true</c> if the user defined the binning type; otherwise, <c>false</c>.
		/// </value>
		public bool IsUserDefinedBinningType { get; set; }

		/// <summary>
		/// Gets or sets the user defined binning. If this property is defined, the binning defined in this property is used. If this property is null, the binning is done automatically.
		/// </summary>
		/// <value>
		/// The user defined binning.
		/// </value>
		public IBinningDefinition Binning { get; set; }

		/// < inheritdoc />
		public object Clone()
		{
			var result = (HistogramCreationOptions)this.MemberwiseClone();
			result.Binning = this.Binning == null ? null : (IBinningDefinition)this.Binning.Clone();
			return result;
		}
	}

	public class HistogramCreationInformation : ICloneable
	{
		private List<string> _warnings = new List<string>();
		private List<string> _errors = new List<string>();

		public IList<string> Warnings { get { return _warnings; } }

		public IList<string> Errors { get { return _errors; } }

		public int NumberOfValues { get; set; }

		public int NumberOfNaNValues { get; set; }

		public int NumberOfInfiniteValues { get; set; }

		public double MinimumValue { get; set; }

		public double MaximumValue { get; set; }

		public HistogramCreationOptions CreationOptions { get; set; }

		public object Clone()
		{
			var result = (HistogramCreationInformation)this.MemberwiseClone();
			result._warnings = new List<string>(_warnings);
			result._errors = new List<string>(_errors);
			result.CreationOptions = (HistogramCreationOptions)this.CreationOptions.Clone();
			return result;
		}
	}

	public struct Bin
	{
		private double _lowerBound;
		private double _upperBound;
		private double _centerPosition;

		public double LowerBound { get { return _lowerBound; } }

		public double UpperBound { get { return _upperBound; } }

		public double CenterPosition { get { return _centerPosition; } }

		public Bin(double lowerBound, double centerPosition, double upperBound)
		{
			_lowerBound = lowerBound;
			_centerPosition = centerPosition;
			_upperBound = upperBound;
		}
	}

	public interface IBinningDefinition : IEnumerable<Bin>, ICloneable
	{
		double GetBinCenterPosition(int idx);

		double GetBinUpperBound(int idx);

		double GetBinLowerBound(int idx);

		Bin this[int idx] { get; }

		double Count { get; }
	}

	public class LinearBinning : IBinningDefinition
	{
		public bool IsUserDefinedBinOffset { get; set; }

		public double BinOffset { get; set; }

		public bool IsUserDefinedBinWidth { get; set; }

		public double BinWidth { get; set; }

		public int BinLowerFactor { get; set; }

		public double Count { get; set; }

		#region IBinningDefinition

		public double GetBinCenterPosition(int idx)
		{
			return BinOffset + (idx + BinLowerFactor) * BinWidth;
		}

		public double GetBinLowerBound(int idx)
		{
			return BinOffset + (idx + BinLowerFactor - 0.5) * BinWidth;
		}

		public double GetBinUpperBound(int idx)
		{
			return BinOffset + (idx + BinLowerFactor + 0.5) * BinWidth;
		}

		public Bin this[int idx]
		{
			get { return new Bin(GetBinLowerBound(idx), GetBinCenterPosition(idx), GetBinUpperBound(idx)); }
		}

		public IEnumerator<Bin> GetEnumerator()
		{
			for (int i = 0; i < Count; ++i)
				yield return new Bin(GetBinLowerBound(i), GetBinCenterPosition(i), GetBinUpperBound(i));
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			for (int i = 0; i < Count; ++i)
				yield return new Bin(GetBinLowerBound(i), GetBinCenterPosition(i), GetBinUpperBound(i));
		}

		#endregion IBinningDefinition

		public object Clone()
		{
			return this.MemberwiseClone();
		}
	}

	public static class HistogramCreation
	{
		#region Internal classes

		private class NumericTableRegionEnumerator : IEnumerator<double>
		{
			private DataTable _srctable;
			private IAscendingIntegerCollection _selectedColumns;
			private IAscendingIntegerCollection _selectedRows;

			private int _nCol = -1;
			private int _nRow = 0;
			private INumericColumn _col;

			public NumericTableRegionEnumerator(DataTable srctable,
			IAscendingIntegerCollection selectedColumns,
			IAscendingIntegerCollection selectedRows)
			{
				_srctable = srctable;
				_selectedColumns = selectedColumns;
				_selectedRows = selectedRows;
			}

			public double Current
			{
				get
				{
					if (null != _col)
						return _col[_selectedRows[_nRow]];
					else
						throw new InvalidOperationException("Before or behind enumeration");
				}
			}

			public void Dispose()
			{
				_col = null;
				_srctable = null;
				_selectedColumns = null;
				_selectedRows = null;
			}

			object System.Collections.IEnumerator.Current
			{
				get
				{
					if (null != _col)
						return _col[_selectedRows[_nRow]];
					else
						throw new InvalidOperationException("Before or behind enumeration");
				}
			}

			public bool MoveNext()
			{
				if (null != _col)
				{
					if (_nRow < _selectedRows.Count - 1)
					{
						++_nRow;
						return true;
					}
					else
					{
						++_nCol;
						if (_nCol < _selectedColumns.Count)
						{
							_nRow = 0;
							_col = (INumericColumn)_srctable.DataColumns[_selectedColumns[_nCol]];
							return true;
						}
						else
						{
							_col = null;
							return false;
						}
					}
				}
				else // _col is null
				{
					if (_nCol < 0)
					{
						_nCol = 0;
						_nRow = 0;
						_col = (INumericColumn)_srctable.DataColumns[_selectedColumns[_nCol]];
						return true;
					}
				}
				return false;
			}

			public void Reset()
			{
				_col = null;
				_nCol = -1;
				_nRow = 0;
			}
		}

		#endregion Internal classes

		/// <summary>
		/// Calculates statistics of selected columns. Returns a new table where the statistical data will be written to.
		/// </summary>
		/// <param name="srctable">Source table.</param>
		/// <param name="selectedColumns">Selected data columns in the source table.</param>
		/// <param name="selectedRows">Selected rows in the source table.</param>
		public static DataTable CreateHistogramOnColumns(
			this DataTable srctable,
			IAscendingIntegerCollection selectedColumns,
			IAscendingIntegerCollection selectedRows
			)
		{
			var histInfo = new HistogramCreationInformation();

			var enumerator = new NumericTableRegionEnumerator(srctable, selectedColumns, selectedRows);

			var list = new List<double>();

			Func<double, bool> IsExcluded;

			var options = histInfo.CreationOptions;

			if (options.LowerBoundaryToIgnore.HasValue && options.UpperBoundaryToIgnore.HasValue)
			{
				// 4 cases
				if (options.IsLowerBoundaryInclusive && options.IsUpperBoundaryInclusive)
					IsExcluded = (x) => (x <= options.LowerBoundaryToIgnore.Value || x >= options.UpperBoundaryToIgnore.Value);
				else if (options.IsLowerBoundaryInclusive)
					IsExcluded = (x) => (x <= options.LowerBoundaryToIgnore.Value || x > options.UpperBoundaryToIgnore.Value);
				else if (options.IsUpperBoundaryInclusive)
					IsExcluded = (x) => (x < options.LowerBoundaryToIgnore.Value || x >= options.UpperBoundaryToIgnore.Value);
				else
					IsExcluded = (x) => (x < options.LowerBoundaryToIgnore.Value || x > options.UpperBoundaryToIgnore.Value);
			}
			else if (options.LowerBoundaryToIgnore.HasValue)
			{
				// 2 cases
				if (options.IsLowerBoundaryInclusive)
					IsExcluded = (x) => (x <= options.LowerBoundaryToIgnore.Value);
				else
					IsExcluded = (x) => (x < options.LowerBoundaryToIgnore.Value);
			}
			else if (options.UpperBoundaryToIgnore.HasValue)
			{
				if (options.IsUpperBoundaryInclusive)
					IsExcluded = (x) => (x >= options.UpperBoundaryToIgnore.Value);
				else
					IsExcluded = (x) => (x > options.UpperBoundaryToIgnore.Value);
			}
			else
			{
				IsExcluded = (x) => false;
			}

			int containsNaN = 0;
			int containsInfinity = 0;

			while (enumerator.MoveNext())
			{
				double x = enumerator.Current;

				if (double.IsNaN(x))
				{
					++containsNaN;
				}
				else if (!Altaxo.Calc.RMath.IsFinite(x))
				{
					++containsInfinity;
				}
				else if (!IsExcluded(x))
				{
					list.Add(x);
				}
			}

			list.Sort();

			int numberOfValues = list.Count;

			histInfo.NumberOfValues = list.Count;
			histInfo.NumberOfNaNValues = containsNaN;
			histInfo.NumberOfInfiniteValues = containsInfinity;

			double minValue = double.NaN;
			double maxValue = double.NaN;

			if (list.Count == 0)
			{
				histInfo.Errors.Add("No values available (number of selected values is null).");
			}
			else if (list.Count < 10)
			{
				histInfo.Warnings.Add("Number of selected values is less than 10");
			}

			if (list.Count > 0)
			{
				minValue = list[0];
				maxValue = list[numberOfValues - 1];

				histInfo.MinimumValue = minValue;
				histInfo.MaximumValue = maxValue;

				// here we must decide between linear binning and logarithmic binning
				if (!histInfo.CreationOptions.IsUserDefinedBinningType)
				{
					Type binningType;

					if (minValue > 0 && maxValue > 0 && (Math.Log10(maxValue) - Math.Log10(minValue)) > 3)
						binningType = typeof(LogarithmicBinning);
					else
						binningType = typeof(LinearBinning);

					if (binningType != histInfo.CreationOptions.Binning.GetType())
						histInfo.CreationOptions.Binning = (IBinningDefinition)Activator.CreateInstance(binningType);
				}
			}

			double q1 = list[(numberOfValues * 1) / 4]; // 25% Quantile
			double q3 = list[(numberOfValues * 3) / 4]; // 75% Quantile

			// Width of a bin after Freedman and Diaconis
			double widthOfBin = 2 * (q3 - q1) / Math.Pow(list.Count, 1.0 / 3.0);

			bool centerAroundZero = minValue <= 0 && maxValue >= 0;

			double binOffset;
			int binFactorMin;

			if (centerAroundZero)
			{
				binOffset = 0;
			}
			else
			{
				binOffset = 0.5 * minValue + 0.5 * maxValue;
			}

			binFactorMin = (int)StrictCeiling((minValue - binOffset) / widthOfBin - 0.5);
			Func<int, double> GetBinCenterPosition = x => binOffset + widthOfBin * (x + binFactorMin);
			Func<int, double> GetBinLowerBound = x => binOffset + widthOfBin * (x + binFactorMin - 0.5);
			Func<int, double> GetBinUpperBound = x => binOffset + widthOfBin * (x + binFactorMin + 0.5);

			// now put the values into the bins
			int numberOfElementsInBin = 0;
			int nBinIndex = 0;
			double upperBound = GetBinUpperBound(nBinIndex);

			var colBinPosition = new DoubleColumn();
			var colBinCounts = new DoubleColumn();

			for (int i = 0; i < list.Count; ++i)
			{
				if (list[i] < upperBound)
				{
					++numberOfElementsInBin;
				}
				else
				{
					--i;
					colBinPosition[nBinIndex] = GetBinCenterPosition(nBinIndex);
					colBinCounts[nBinIndex] = numberOfElementsInBin;
					numberOfElementsInBin = 0;
					++nBinIndex;
					upperBound = GetBinUpperBound(nBinIndex);
				}
			}
			if (numberOfElementsInBin > 0)
			{
				colBinPosition[nBinIndex] = GetBinCenterPosition(nBinIndex);
				colBinCounts[nBinIndex] = numberOfElementsInBin;
				++nBinIndex;
			}

			int numberOfBins = nBinIndex;

			// calculate probability density
			var colProbabilityDensity = new DoubleColumn();

			for (int i = 0; i < numberOfBins; ++i)
				colProbabilityDensity[i] = colBinCounts[i] / (numberOfValues * widthOfBin);

			var destTable = Current.Project.CreateNewTable(srctable.Name + "-HistogramData", false);

			using (var suspendToken = destTable.SuspendGetToken())
			{
				destTable.DataColumns.Add(colBinPosition, "BinPosition", ColumnKind.X, 0);
				destTable.DataColumns.Add(colBinCounts, "BinCounts", ColumnKind.Y, 0);
				destTable.DataColumns.Add(colProbabilityDensity, "ProbabilityDensity", ColumnKind.Y, 0);
			}

			// Test the probability against the normal distribution

			// First make a fit of the normal distribution to get mu and sigma
			// guess of mu
			double guessedMu = list[list.Count / 2];
			double guessedSigma = q3 - q1;
			Altaxo.Calc.Regression.Nonlinear.SimpleNonlinearFit fit = new Altaxo.Calc.Regression.Nonlinear.SimpleNonlinearFit(
				delegate(double[] indep, double[] p, double[] res)
				{
					// 2 Parameter: mu and sigma
					res[0] = Calc.Probability.NormalDistribution.PDF(indep[0], p[0], p[1]);
				},
				new double[] { guessedMu, guessedSigma },
				colBinPosition,
				colProbabilityDensity,
				0, // Start (first point)
				numberOfBins // point count
				);

			fit.Fit();

			guessedMu = fit.GetParameter(0);
			guessedSigma = fit.GetParameter(1);

			// Test hypothesis that we have a normal distribution

			double chiSquare = 0;

			int degreesOfFreedom = numberOfBins - 1 - 2; // -2 because we lost two degrees of freedom in the previous fitting

			for (nBinIndex = 0; nBinIndex < numberOfBins; ++nBinIndex)
			{
				double lowerBound = GetBinLowerBound(nBinIndex);
				upperBound = GetBinUpperBound(nBinIndex);

				var probability = Calc.Probability.NormalDistribution.CDF(upperBound, guessedMu, guessedSigma) - Calc.Probability.NormalDistribution.CDF(lowerBound, guessedMu, guessedSigma);
				double n0 = probability * list.Count;

				chiSquare += RMath.Pow2(n0 - colBinCounts[nBinIndex]) / n0;
			}

			double chiSquareThreshold = Calc.Probability.ChiSquareDistribution.Quantile(0.95, degreesOfFreedom);

			if (chiSquare <= chiSquareThreshold)
			{
				destTable.Notes.WriteLine("The hypothesis that this is a normal distribution with mu={0} and sigma={1} can not be rejected. ChiSquare ({2}) is less than ChiSquare needed to reject hypothesis ({3})", guessedMu, guessedSigma, chiSquare, chiSquareThreshold);
			}
			else
			{
				destTable.Notes.WriteLine("The hypothesis that this is a normal distribution with mu={0} and sigma={1} must be rejected with a confidence of {2}%. ChiSquare ({3}) is greater than ChiSquare needed to reject hypothesis ({4})", guessedMu, guessedSigma, 100 * Altaxo.Calc.Probability.ChiSquareDistribution.CDF(chiSquare, degreesOfFreedom), chiSquare, chiSquareThreshold);
			}

			return destTable;
		}

		private static double StrictCeiling(double x)
		{
			double result = Math.Ceiling(x);
			if (result == x)
				result += 1;

			return result;
		}
	}
}