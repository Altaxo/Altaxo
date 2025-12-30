#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2024 Dr. Dirk Lellinger
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
using System.Collections;
using System.Collections.Generic;

namespace Altaxo.Science.Thermorheology.MasterCurves
{
  /// <summary>
  /// Stores information about an interpolation curve that interpolates the resulting shift curve for one group of columns,
  /// e.g. for the real part of measured values.
  /// </summary>
  /// <typeparam name="T">The type of the y-values stored for interpolation.</typeparam>
  public class InterpolationInformationBase<T>
  {
    /// <summary>
    /// Minimum x value of the points used for interpolation.
    /// </summary>
    public double InterpolationMinimumX { get; protected set; }

    /// <summary>
    /// Maximum x value of the points used for interpolation.
    /// </summary>
    public double InterpolationMaximumX { get; protected set; }

    /// <summary>
    /// List of all x values of the points that are used for the interpolation.
    /// The returned list is a read-only wrapper around the internal storage.
    /// </summary>
    public IReadOnlyList<double> XValues { get { return new WrapperIListToIRoVectorK<double>(ValuesToInterpolate.Keys); } }

    /// <summary>
    /// List of all y values of the points that are used for the interpolation.
    /// The returned list is a read-only wrapper around the internal storage.
    /// </summary>
    public IReadOnlyList<T> YValues { get { return new WrapperIListToIRoVectorV<T>(ValuesToInterpolate.Values); } }

    /// <summary>
    /// List of the index of the curve to which each point belongs.
    /// The returned list is a read-only wrapper around the internal storage.
    /// </summary>
    public IReadOnlyList<int> IndexOfCurve { get { return new WrapperIListToIRoVectorV2<T>(ValuesToInterpolate.Values); } }

    /// <summary>
    /// List of all points used for the interpolation, sorted by the x values.
    /// Keys are the x-Values, values are the y-values and the index of the curve the y-value belongs to.
    /// </summary>
    protected SortedList<double, (T y, int indexOfCurve)> ValuesToInterpolate { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="InterpolationInformationBase{T}"/> class.
    /// </summary>
    public InterpolationInformationBase()
    {
      InterpolationMinimumX = double.MaxValue;
      InterpolationMaximumX = double.MinValue;
      ValuesToInterpolate = [];
    }

    /// <summary>
    /// Clears the stored interpolation information.
    /// After calling this, the instance contains no points and min/max are reset.
    /// </summary>
    public virtual void Clear()
    {
      InterpolationMinimumX = double.MaxValue;
      InterpolationMaximumX = double.MinValue;
      ValuesToInterpolate.Clear();
    }

    /// <summary>
    /// Gets the minimum and maximum of the current x-values used for interpolation. Data points that belong
    /// to the curve with the index given in the argument are not taken into account.
    /// </summary>
    /// <param name="indexOfCurve">The index of curve to exclude from the calculation.</param>
    /// <returns>The minimum and maximum of the x-values, except for those points that belong to the curve with index=<paramref name="indexOfCurve"/>.</returns>
    public (double min, double max) GetMinimumMaximumOfXValuesExceptForCurveIndex(int indexOfCurve)
    {
      double min = double.PositiveInfinity;
      double max = double.NegativeInfinity;
      foreach (var entry in ValuesToInterpolate)
      {
        if (entry.Value.indexOfCurve != indexOfCurve)
        {
          min = Math.Min(min, entry.Key);
          max = Math.Max(max, entry.Key);
        }
      }
      return (min, max);
    }



    #region Wrapper classes

    /// <summary>
    /// Wraps an <see cref="IList{T}"/> of values to an <see cref="IReadOnlyList{T}"/> implementation.
    /// This wrapper reads items directly from the supplied list.
    /// </summary>
    protected class WrapperIListToIRoVectorK<TW>(IList<TW> list) : IReadOnlyList<TW>
    {
      private IList<TW> _list = list;

      #region IROVector Members

      /// <summary>
      /// Gets the number of elements contained in the list.
      /// </summary>
      public int Count
      {
        get { return _list.Count; }
      }

      #endregion IROVector Members

      #region INumericSequence Members

      /// <summary>
      /// Gets the element at the specified index.
      /// </summary>
      public TW this[int i]
      {
        get { return _list[i]; }
      }

      /// <summary>
      /// Returns an enumerator that iterates through the list.
      /// </summary>
      public IEnumerator<TW> GetEnumerator()
      {
        var len = Count;
        for (int i = 0; i < len; ++i)
          yield return this[i];
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
        var len = Count;
        for (int i = 0; i < len; ++i)
          yield return this[i];
      }

      #endregion INumericSequence Members
    }

    /// <summary>
    /// Wraps an <see cref="IList{T}"/> of <c>(TW, int)</c> tuples to an <see cref="IReadOnlyList{TW}"/> that exposes the first component of the tuple.
    /// </summary>
    protected class WrapperIListToIRoVectorV<TW>(IList<(TW, int)> list) : IReadOnlyList<TW>
    {
      private IList<(TW, int)> _list = list;

      #region IROVector Members

      /// <summary>
      /// Gets the number of elements contained in the list.
      /// </summary>
      public int Count
      {
        get { return _list.Count; }
      }

      #endregion IROVector Members

      #region INumericSequence Members

      /// <summary>
      /// Gets the first component of the tuple at the specified index.
      /// </summary>
      public TW this[int i]
      {
        get { return _list[i].Item1; }
      }

      /// <summary>
      /// Returns an enumerator that iterates through the projected values.
      /// </summary>
      public IEnumerator<TW> GetEnumerator()
      {
        var len = Count;
        for (int i = 0; i < len; ++i)
          yield return this[i];
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
        var len = Count;
        for (int i = 0; i < len; ++i)
          yield return this[i];
      }

      #endregion INumericSequence Members
    }

    /// <summary>
    /// Wraps an <see cref="IList{T}"/> of <c>(TW, int)</c> tuples to an <see cref="IReadOnlyList{Int32}"/> that exposes the second component of the tuple (the curve index).
    /// </summary>
    protected class WrapperIListToIRoVectorV2<TW>(IList<(TW, int)> list) : IReadOnlyList<int>
    {
      private IList<(TW, int)> _list = list;

      #region IROVector Members

      /// <summary>
      /// Gets the number of elements contained in the list.
      /// </summary>
      public int Count
      {
        get { return _list.Count; }
      }

      #endregion IROVector Members

      #region INumericSequence Members

      /// <summary>
      /// Gets the second component (the int) of the tuple at the specified index.
      /// </summary>
      public int this[int i]
      {
        get { return _list[i].Item2; }
      }

      /// <summary>
      /// Returns an enumerator that iterates through the projected int values.
      /// </summary>
      public IEnumerator<int> GetEnumerator()
      {
        var len = Count;
        for (int i = 0; i < len; ++i)
          yield return this[i];
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
        var len = Count;
        for (int i = 0; i < len; ++i)
          yield return this[i];
      }

      #endregion INumericSequence Members
    }

    #endregion Wrapper class
  }
}


