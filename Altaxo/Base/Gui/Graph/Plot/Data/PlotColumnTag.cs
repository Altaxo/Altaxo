#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2022 Dr. Dirk Lellinger
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

#nullable disable
using Altaxo.Main;

namespace Altaxo.Gui.Graph.Plot.Data
{
  /// <summary>
  /// Identifies a plot column by group number and column number.
  /// </summary>
  public record PlotColumnTag : IImmutable
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="PlotColumnTag"/> class.
    /// </summary>
    /// <param name="groupNumber">The group number.</param>
    /// <param name="columnNumber">The column number.</param>
    public PlotColumnTag(int groupNumber, int columnNumber)
    {
      GroupNumber = groupNumber;
      ColumnNumber = columnNumber;
    }

    /// <summary>
    /// Gets the group number.
    /// </summary>
    public int GroupNumber { get; init; }
    /// <summary>
    /// Gets the column number.
    /// </summary>
    public int ColumnNumber { get; init; }
  }
}
