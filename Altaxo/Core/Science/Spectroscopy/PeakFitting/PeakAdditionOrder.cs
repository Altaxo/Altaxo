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

namespace Altaxo.Science.Spectroscopy.PeakFitting
{
  /// <summary>
  /// For peak fitting by incremental peak addition, this enumeration
  /// determines the order in which peaks are added.
  /// </summary>
  public enum PeakAdditionOrder
  {
    /// <summary>
    /// Order by peak height.
    /// </summary>
    Height,

    /// <summary>
    /// Order by peak area (estimated from height × FWHM).
    /// </summary>
    Area,

    /// <summary>
    /// Order by height² × FWHM.
    /// </summary>
    SquaredHeightTimesWidth,
  }
}
