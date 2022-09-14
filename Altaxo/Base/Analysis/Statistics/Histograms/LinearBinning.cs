#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2018 Dr. Dirk Lellinger
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

#nullable enable
using System;
using System.Collections.Generic;
using Altaxo.Calc;

namespace Altaxo.Analysis.Statistics.Histograms
{
  /// <summary>
  /// Represents linear binning, i.e. bins with the same width at equally spaced positions.
  /// </summary>
  public class LinearBinning : IBinning
  {
    #region Inner classes

    private class BinList : Altaxo.Calc.LinearAlgebra.ROVectorBase<Bin>
    {
      private LinearBinning _parent;

      internal BinList(LinearBinning b)
      {
        _parent = b;
      }

      public override Bin this[int index]
      {
        get
        {
          return new Bin(_parent.GetBinLowerBound(index), _parent.GetBinCenterPosition(index), _parent.GetBinUpperBound(index), _parent._binCounts[index]);
        }
        set
        {
          throw new InvalidOperationException("This list is read-only.");
        }
      }

      public override int Count
      {
        get { return _parent._binCounts.Length; }
      }
    }

    #endregion Inner classes

    protected double _binOffset = double.NaN;
    protected double _binWidth = double.NaN;
    protected int _numberOfBins = 0;
    protected int _binLowerIndex;
    protected int[] _binCounts = new int[0];
    private BinList _binListProxy;
    protected int _ensembleDataCount = 0;

    /// <summary>
    /// Gets or sets a value indicating whether the bin offset is user defined.
    /// </summary>
    /// <value>
    /// <c>true</c> if the bin offset is user defined; otherwise, <c>false</c>.
    /// </value>
    public bool IsUserDefinedBinOffset { get; set; }

    /// <summary>
    /// Gets or sets the bin offset. This is the position of one bin. The positions of all other bins are calculated relative to this reference position by using integer multiples of the bin width.
    /// </summary>
    /// <value>
    /// The bin offset.
    /// </value>
    /// <exception cref="System.ArgumentException">Value has to be a finite number</exception>
    public double BinOffset
    {
      get
      {
        return _binOffset;
      }
      set
      {
        if (!(value.IsFinite()))
          throw new ArgumentException("Value has to be a finite number");

        var oldValue = _binOffset;
        _binOffset = value;

        if (oldValue != value)
          Invalidate();
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the bin width was defined by the user.
    /// </summary>
    /// <value>
    /// <c>true</c> if the bin width is user defined; otherwise, <c>false</c>.
    /// </value>
    public bool IsUserDefinedBinWidth { get; set; }

    /// <summary>
    /// Gets or sets the width of the bins.
    /// </summary>
    /// <value>
    /// The width of the bins.
    /// </value>
    /// <exception cref="System.ArgumentException">BinWidth has to be a finite number > 0</exception>
    public double BinWidth
    {
      get
      {
        return _binWidth;
      }
      set
      {
        if (!(value.IsFinite()) || !(value > 0))
          throw new ArgumentException("BinWidth has to be a finite number > 0");

        var oldValue = _binWidth;
        _binWidth = value;

        if (oldValue != value)
          Invalidate();
      }
    }

    /// <summary>
    /// Gets the number of bins.
    /// </summary>
    /// <value>
    /// The number of bins.
    /// </value>
    public int NumberOfBins { get { return _numberOfBins; } }

    /// <summary>
    /// Gets the list of bins.
    /// </summary>
    /// <value>
    /// The bins.
    /// </value>
    public IReadOnlyList<Bin> Bins { get { return _binListProxy; } }

    private void Invalidate()
    {
      _numberOfBins = 0;
      _ensembleDataCount = 0;
      _binCounts = new int[0];
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LinearBinning"/> class.
    /// </summary>
    public LinearBinning()
    {
      _binListProxy = new BinList(this);
    }

    #region IBinningDefinition

    /// <inheritdoc />
    public void CalculateBinPositionsFromSortedList(IReadOnlyList<double> list)
    {
      if (list is null)
        throw new ArgumentNullException();
      if (list.Count == 0)
        throw new ArgumentException("list is empty");

      var numberOfValues = list.Count;
      var minValue = list[0];
      var maxValue = list[numberOfValues - 1];

      if (!IsUserDefinedBinWidth)
      {
        double q1 = list[(numberOfValues * 1) / 4]; // 25% Quantile
        double q3 = list[(numberOfValues * 3) / 4]; // 75% Quantile

        // Width of a bin after Freedman and Diaconis
        double widthOfBin = 2 * (q3 - q1) / Math.Pow(list.Count, 1.0 / 3.0);

        BinWidth = widthOfBin; // use property in order to invalidate the bins if the value has changed
      }

      if (!IsUserDefinedBinOffset)
      {
        bool centerAroundZero = minValue <= 0 && maxValue >= 0;

        double binOffset;

        if (centerAroundZero)
        {
          binOffset = 0;
        }
        else
        {
          binOffset = 0.5 * minValue + 0.5 * maxValue;
        }

        BinOffset = binOffset; // use property in order to invalidate the bins if the value has changed
      }

      // calculate number of resulting bins
      SetCountFromValueMinimumMaximum(minValue, maxValue);
    }

    /// <inheritdoc />
    public void CalculateBinsFromSortedList(IReadOnlyList<double> sortedListOfValues)
    {
      _binCounts = new int[_numberOfBins];
      int listIndex = 0;
      for (int binIndex = 0; binIndex < NumberOfBins; ++binIndex)
      {
        double binUpperBound = GetBinUpperBound(binIndex);
        double binLowerBound = GetBinLowerBound(binIndex);
        double binCenter = GetBinCenterPosition(binIndex);

        int orgListIndex = listIndex;
        while (listIndex < sortedListOfValues.Count && (sortedListOfValues[listIndex] < binUpperBound || binIndex == (NumberOfBins - 1)))  // with binIndex == (Count - 1) make sure that rounding errors will not swallow the last value
        {
          ++listIndex;
        }

        _binCounts[binIndex] = listIndex - orgListIndex;
      }
      _ensembleDataCount = listIndex;
    }

    private double GetBinCenterPosition(int idx)
    {
      return BinOffset + (idx + _binLowerIndex) * BinWidth;
    }

    private double GetBinLowerBound(int idx)
    {
      return BinOffset + (idx + _binLowerIndex - 0.5) * BinWidth;
    }

    private double GetBinUpperBound(int idx)
    {
      return BinOffset + (idx + _binLowerIndex + 0.5) * BinWidth;
    }

    #endregion IBinningDefinition

    public object Clone()
    {
      return MemberwiseClone();
    }

    private static double StrictCeiling(double x)
    {
      double result = Math.Ceiling(x);
      if (result == x)
        result += 1;

      return result;
    }

    /// <summary>
    /// Gets the binning count (and <see cref="_binLowerIndex"/> from the minimum and maximum of the values. Presumes that <see cref="BinOffset"/> and <see cref="BinWidth"/> are already set.
    /// </summary>
    /// <param name="minValue">The minimum value.</param>
    /// <param name="maxValue">The maximum value.</param>
    /// <exception cref="InvalidOperationException">
    /// </exception>
    private void SetCountFromValueMinimumMaximum(double minValue, double maxValue)
    {
      double lowerIndex = StrictCeiling((minValue - BinOffset) / BinWidth - 0.5);
      double upperIndex = StrictCeiling((maxValue - BinOffset) / BinWidth - 0.5);
      double binCount = upperIndex + 1 - lowerIndex;

      if (!(binCount <= int.MaxValue))
        throw new InvalidOperationException(string.Format("Binning would result in far too many bins. The bin count would be {0}. Please try to increase the bin width.", binCount));

      if (!(lowerIndex >= int.MinValue))
        throw new InvalidOperationException(string.Format("Values of binning offset and minimum value are too far away. Please try to adjust binning offset so that it is between minimum value and maximum value."));

      _binLowerIndex = (int)lowerIndex;
      _numberOfBins = (int)binCount;
    }
  }
}
