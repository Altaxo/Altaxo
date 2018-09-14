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

using System;
using System.Collections.Generic;

namespace Altaxo.Serialization.Ascii
{
  /// <summary>
  /// Immutable key for Ascii line analysis dictionary. Contains the separation strategy, the number format and the date/time format used to analyze a single line of Ascii data.
  /// </summary>
  public class AsciiLineAnalysisOption
  {
    private IAsciiSeparationStrategy _separationStrategy;
    private System.Globalization.CultureInfo _numberFormat;
    private System.Globalization.CultureInfo _dateTimeFormat;
    private int _cachedHashCode;

    public AsciiLineAnalysisOption(IAsciiSeparationStrategy s, System.Globalization.CultureInfo n, System.Globalization.CultureInfo d)
    {
      _separationStrategy = s;
      _numberFormat = n;
      _dateTimeFormat = d;

      _cachedHashCode = _separationStrategy.GetHashCode() ^ _numberFormat.GetHashCode() ^ _dateTimeFormat.GetHashCode();
    }

    public IAsciiSeparationStrategy SeparationStrategy { get { return _separationStrategy; } }

    public System.Globalization.CultureInfo NumberFormat { get { return _numberFormat; } }

    public System.Globalization.CultureInfo DateTimeFormat { get { return _dateTimeFormat; } }

    public override bool Equals(object obj)
    {
      if (obj == null || GetType() != obj.GetType())
      {
        return false;
      }

      var from = (AsciiLineAnalysisOption)obj;
      return _separationStrategy.Equals(from._separationStrategy) && _numberFormat.Equals(from._numberFormat) && _dateTimeFormat.Equals(from._dateTimeFormat);
    }

    public override int GetHashCode()
    {
      return _cachedHashCode;
    }

    public override string ToString()
    {
      return string.Format("{0} {1} {2}", _separationStrategy, _numberFormat, _dateTimeFormat);
    }
  }
}
