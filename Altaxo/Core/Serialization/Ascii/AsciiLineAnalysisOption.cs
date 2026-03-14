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
//    along with this program; if not, write to the Free Software
//    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
//
/////////////////////////////////////////////////////////////////////////////

#endregion Copyright

#nullable enable

namespace Altaxo.Serialization.Ascii
{
  /// <summary>
  /// Represents an immutable key for the Ascii line analysis dictionary.
  /// Contains the separation strategy, the number format and the date/time format used to analyze a single line of Ascii data.
  /// </summary>
  public class AsciiLineAnalysisOption : Main.IImmutable
  {
    /// <summary>
    /// Gets the separation strategy used to split a line into tokens.
    /// </summary>
    public IAsciiSeparationStrategy SeparationStrategy { get; }

    /// <summary>
    /// Gets the culture used to parse numeric values.
    /// </summary>
    public System.Globalization.CultureInfo NumberFormat { get; }

    /// <summary>
    /// Gets the culture used to parse date/time values.
    /// </summary>
    public System.Globalization.CultureInfo DateTimeFormat { get; }

    /// <summary>
    /// Gets a cached hash code for this instance.
    /// </summary>
    private int CachedHashCode { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AsciiLineAnalysisOption"/> class.
    /// </summary>
    /// <param name="separationStrategy">Separation strategy used to split a line into tokens.</param>
    /// <param name="numberFormat">Culture used to parse numeric values.</param>
    /// <param name="dateTimeFormat">Culture used to parse date/time values.</param>
    public AsciiLineAnalysisOption(IAsciiSeparationStrategy separationStrategy, System.Globalization.CultureInfo numberFormat, System.Globalization.CultureInfo dateTimeFormat)
    {
      SeparationStrategy = separationStrategy;
      NumberFormat = numberFormat;
      DateTimeFormat = dateTimeFormat;

      CachedHashCode = SeparationStrategy.GetHashCode() ^ NumberFormat.GetHashCode() ^ DateTimeFormat.GetHashCode();
    }


    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
      if (obj is null || GetType() != obj.GetType())
      {
        return false;
      }

      var from = (AsciiLineAnalysisOption)obj;
      return SeparationStrategy.Equals(from.SeparationStrategy) && NumberFormat.Equals(from.NumberFormat) && DateTimeFormat.Equals(from.DateTimeFormat);
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
      return CachedHashCode;
    }

    /// <inheritdoc/>
    public override string ToString()
    {
      return $"{SeparationStrategy} [{NumberFormat}] [{DateTimeFormat}]";
    }
  }
}
