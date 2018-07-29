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

using Altaxo.Calc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Analysis.Statistics.Histograms
{
  public class LogarithmicBinning : IBinning
  {
    #region Inner classes

    private class BinList : Altaxo.Calc.LinearAlgebra.ROVectorBase<Bin>
    {
      private LogarithmicBinning _parent;

      internal BinList(LogarithmicBinning b)
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

    protected double _lgBinOffset = double.NaN;
    protected double _lgBinWidth = double.NaN;
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
    /// Gets or sets a value indicating whether the bin width was defined by the user.
    /// </summary>
    /// <value>
    /// <c>true</c> if the bin width is user defined; otherwise, <c>false</c>.
    /// </value>
    public bool IsUserDefinedBinWidth { get; set; }

    /// <summary>
    /// Gets the number of bins.
    /// </summary>
    /// <value>
    /// The number of bins.
    /// </value>
    public int NumberOfBins { get { return _numberOfBins; } }

    /// <summary>
    /// Gets or sets the bin offset. This is the position of one bin. The positions of all other bins are calculated relative to this reference position by using logarithmically equally spaced distances.
    /// </summary>
    /// <value>
    /// The bin offset.
    /// </value>
    /// <exception cref="System.ArgumentException">BinOffset has to be a positive finite number</exception>
    public double BinOffset
    {
      get
      {
        return Math.Pow(10, _lgBinOffset);
      }
      set
      {
        if (!RMath.IsFinite(value) || !(value > 0))
          throw new ArgumentException("BinOffset has to be a positive finite number");
        var oldValue = _lgBinOffset;
        _lgBinOffset = Math.Log10(value);
        if (oldValue != value)
          Invalidate();
      }
    }

    /// <summary>
    /// Gets or sets the number of decades that every bin spans.
    /// </summary>
    /// <value>
    /// The number of decades that every bin spans.
    /// </value>
    /// <exception cref="System.ArgumentException">LgBinWidth has to be a finite number > 0</exception>
    /// <example>A value of 1 means that every bin spans one decade.</example>
    public double BinWidthInDecades
    {
      get
      {
        return _lgBinWidth;
      }
      set
      {
        if (!RMath.IsFinite(value) || !(value > 0))
          throw new ArgumentException("LgBinWidth has to be a finite number > 0");
        var oldValue = _lgBinWidth;
        _lgBinWidth = value;
        if (oldValue != value)
          Invalidate();
      }
    }

    private void Invalidate()
    {
      _numberOfBins = 0;
      _ensembleDataCount = 0;
      _binCounts = new int[0];
    }

    private static double StrictCeiling(double x)
    {
      double result = Math.Ceiling(x);
      if (result == x)
        result += 1;

      return result;
    }

    #region IBinningDefinition

    private double GetBinCenterPosition(int idx)
    {
      return Math.Pow(10, _lgBinOffset + (idx + _binLowerIndex) * _lgBinWidth);
    }

    private double GetBinLowerBound(int idx)
    {
      return Math.Pow(10, _lgBinOffset + (idx + _binLowerIndex - 0.5) * _lgBinWidth);
    }

    private double GetBinUpperBound(int idx)
    {
      return Math.Pow(10, _lgBinOffset + (idx + _binLowerIndex + 0.5) * _lgBinWidth);
    }

    #endregion IBinningDefinition

    /// <summary>
    /// Initializes a new instance of the <see cref="LogarithmicBinning"/> class.
    /// </summary>
    public LogarithmicBinning()
    {
      _binListProxy = new BinList(this);
    }

    /// <summary>
    /// Gets the list of bins.
    /// </summary>
    /// <value>
    /// The bins.
    /// </value>
    public IReadOnlyList<Bin> Bins { get { return _binListProxy; } }

    /// <inheritdoc />
    public object Clone()
    {
      return this.MemberwiseClone();
    }

    /// <inheritdoc />
    public void CalculateBinPositionsFromSortedList(IReadOnlyList<double> list)
    {
      if (null == list)
        throw new ArgumentNullException();
      if (list.Count == 0)
        throw new ArgumentException("list is empty");

      var numberOfValues = list.Count;
      var minValue = list[0];
      var maxValue = list[numberOfValues - 1];

      if (!(minValue > 0))
        throw new InvalidOperationException(string.Format("For logarithmic binning the minimum ensemble value should be > 0, but it is {0}. Try to exclude values <= 0 or use linear binning.", minValue));

      if (!IsUserDefinedBinWidth)
      {
        double q1 = list[(numberOfValues * 1) / 4]; // 25% Quantile
        double q3 = list[(numberOfValues * 3) / 4]; // 75% Quantile

        // Width of a bin after Freedman and Diaconis
        _lgBinWidth = 2 * (Math.Log10(q3) - Math.Log10(q1)) / Math.Pow(list.Count, 1.0 / 3.0);
      }

      if (!IsUserDefinedBinOffset)
      {
        bool centerAroundOne = minValue <= 1 && maxValue >= 1;

        if (centerAroundOne)
        {
          _lgBinOffset = 0;
        }
        else
        {
          _lgBinOffset = 0.5 * Math.Log10(minValue) + 0.5 * Math.Log10(maxValue);
        }
      }

      // calculate number of resulting bins
      SetCountFromValueMinimumMaximum(minValue, maxValue);
    }

    /// <summary>
    /// Gets the binning count (and <see cref="_binLowerIndex"/> from the minimum and maximum of the values. Presumes that <see cref="BinOffset"/> and <see cref="BinWidthInDecades"/> are already set.
    /// </summary>
    /// <param name="minValue">The minimum value.</param>
    /// <param name="maxValue">The maximum value.</param>
    /// <exception cref="InvalidOperationException">
    /// </exception>
    private void SetCountFromValueMinimumMaximum(double minValue, double maxValue)
    {
      double lgMinValue = Math.Log10(minValue);
      double lgMaxValue = Math.Log10(maxValue);

      double lowerIndex = StrictCeiling((lgMinValue - _lgBinOffset) / _lgBinWidth - 0.5);
      double upperIndex = StrictCeiling((lgMaxValue - _lgBinOffset) / _lgBinWidth - 0.5);
      double binCount = upperIndex + 1 - lowerIndex;

      if (!(binCount <= int.MaxValue))
        throw new InvalidOperationException(string.Format("Binning would result in far too many bins. The bin count would be {0}. Please try to increase the bin width.", binCount));

      if (!(lowerIndex >= int.MinValue))
        throw new InvalidOperationException(string.Format("Values of binning offset and minimum value are too far away. Please try to adjust binning offset so that it is between minimum value and maximum value."));

      _binLowerIndex = (int)lowerIndex;
      _numberOfBins = (int)binCount;
    }

    /// <inheritdoc />
    public void CalculateBinsFromSortedList(IReadOnlyList<double> sortedListOfValues)
    {
      _binCounts = new int[_numberOfBins];
      int listIndex = 0;
      for (int binIndex = 0; binIndex < _numberOfBins; ++binIndex)
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
  }
}
