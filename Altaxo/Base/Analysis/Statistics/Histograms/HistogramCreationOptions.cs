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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Analysis.Statistics.Histograms
{
  /// <summary>
  /// Options to create histograms.
  /// </summary>
  public class HistogramCreationOptions : ICloneable
  {
    private IBinning _binning = new LinearBinning();

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
    public double? LowerBoundaryToIgnore { get; set; }

    /// <summary>
    /// Designates whether the lower boundary is inclusive. If <c>true</c>, all values less than or equal to <see cref="LowerBoundaryToIgnore"/> are ignored. If <c>false</c>, only values less than <see cref="LowerBoundaryToIgnore"/> are ignored.
    /// </summary>
    public bool IsLowerBoundaryInclusive { get; set; }

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
    public bool IsUpperBoundaryInclusive { get; set; }

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
    public IBinning Binning
    {
      get { return _binning; }
      set
      {
        if (null == value)
          throw new ArgumentNullException();
        _binning = value;
      }
    }

    /// <inheritdoc />
    public object Clone()
    {
      var result = (HistogramCreationOptions)this.MemberwiseClone();
      result._binning = this._binning == null ? null : (IBinning)this._binning.Clone();
      return result;
    }

    /// <summary>
    /// Gets a function that can be used to determine whether a value of a data ensemble should be excluded.
    /// </summary>
    /// <returns>Function that determines whether a value of a data set should be excluded. The argument of the returned function is the value from the data set. The return value of this function is
    /// true if the value given in the argument should be excluded from further processing.</returns>
    public Func<double, bool> GetValueExcludingFunction()
    {
      Func<double, bool> IsExcluded;

      if (LowerBoundaryToIgnore.HasValue && UpperBoundaryToIgnore.HasValue)
      {
        // 4 cases
        if (IsLowerBoundaryInclusive && IsUpperBoundaryInclusive)
          IsExcluded = (x) => (x <= LowerBoundaryToIgnore.Value || x >= UpperBoundaryToIgnore.Value);
        else if (IsLowerBoundaryInclusive)
          IsExcluded = (x) => (x <= LowerBoundaryToIgnore.Value || x > UpperBoundaryToIgnore.Value);
        else if (IsUpperBoundaryInclusive)
          IsExcluded = (x) => (x < LowerBoundaryToIgnore.Value || x >= UpperBoundaryToIgnore.Value);
        else
          IsExcluded = (x) => (x < LowerBoundaryToIgnore.Value || x > UpperBoundaryToIgnore.Value);
      }
      else if (LowerBoundaryToIgnore.HasValue)
      {
        // 2 cases
        if (IsLowerBoundaryInclusive)
          IsExcluded = (x) => (x <= LowerBoundaryToIgnore.Value);
        else
          IsExcluded = (x) => (x < LowerBoundaryToIgnore.Value);
      }
      else if (UpperBoundaryToIgnore.HasValue)
      {
        if (IsUpperBoundaryInclusive)
          IsExcluded = (x) => (x >= UpperBoundaryToIgnore.Value);
        else
          IsExcluded = (x) => (x > UpperBoundaryToIgnore.Value);
      }
      else
      {
        IsExcluded = (x) => false;
      }

      return IsExcluded;
    }
  }
}
