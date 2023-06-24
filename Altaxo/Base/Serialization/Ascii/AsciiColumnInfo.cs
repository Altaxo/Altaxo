#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2023 Dr. Dirk Lellinger
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
using System.Collections.Immutable;

namespace Altaxo.Serialization.Ascii
{
  /// <summary>
  /// For the different possible types of columns, gets the score value and the shortcut.
  /// </summary>
  public record AsciiColumnInfo : IEquatable<AsciiColumnInfo>
  {
    private AsciiColumnInfo(AsciiColumnType t, int scoreValue, char shortCut)
    {
      ColumnType = t;
      ScoreValue = scoreValue;
      ShortCut = shortCut;
    }

    /// <summary>
    /// Gets the type of the column.
    /// </summary>
    public AsciiColumnType ColumnType { get; }

    /// <summary>
    /// Gets the score value of this type of column.
    /// </summary>
    public int ScoreValue { get; }

    /// <summary>
    /// Gets the short cut for this type of column.
    /// </summary>
    public char ShortCut { get; }

    /// <summary>Column that can contain an element or not.</summary>
    public static AsciiColumnInfo DBNull { get; } = new AsciiColumnInfo(AsciiColumnType.DBNull, 1, 'O');

    /// <summary>Column that contains text (that can not be parsed to a number or a date/time).</summary>
    public static AsciiColumnInfo Text { get; } = new AsciiColumnInfo(AsciiColumnType.Text, 2, 'T');

    /// <summary>Column that contains a floating-point number with a decimal separator.</summary>
    public static AsciiColumnInfo FloatWithDecimalSeparator { get; } = new AsciiColumnInfo(AsciiColumnType.Double, 8, 'F');

    /// <summary>Column that contains a floating-point number without a decimal separator.</summary>
    public static AsciiColumnInfo FloatWithoutDecimalSeparator { get; } = new AsciiColumnInfo(AsciiColumnType.Double, 4, 'E');

    /// <summary>Column that contains an integer number.</summary>
    public static AsciiColumnInfo Integer { get; } = new AsciiColumnInfo(AsciiColumnType.Int64, 3, 'I');

    /// <summary>Column that contains a general number.</summary>
    public static AsciiColumnInfo GeneralNumber { get; } = new AsciiColumnInfo(AsciiColumnType.Double, 3, 'N');

    /// <summary>Column that contains a date/time value.</summary>
    public static AsciiColumnInfo DateTime { get; } = new AsciiColumnInfo(AsciiColumnType.DateTime, 17, 'D');

    /// <summary>
    /// Gets a dictionary that associates each value of <see cref="AsciiColumnType"/> with the appropriate instance of <see cref="AsciiColumnInfo"/>.
    /// </summary>
    public static ImmutableDictionary<AsciiColumnType, AsciiColumnInfo> ColumnTypeToColumnInfo { get; }

    static AsciiColumnInfo()
    {
      var builder = ImmutableDictionary.CreateBuilder<AsciiColumnType, AsciiColumnInfo>();

      foreach (AsciiColumnType key in Enum.GetValues(typeof(AsciiColumnType)))
      {
        var val = key switch
        {
          AsciiColumnType.DBNull => DBNull,
          AsciiColumnType.Int64 => Integer,
          AsciiColumnType.Double => FloatWithDecimalSeparator,
          AsciiColumnType.DateTime => DateTime,
          AsciiColumnType.Text => Text,
          _ => throw new NotImplementedException(),
        };
        builder.Add(key, val);
      }
      ColumnTypeToColumnInfo = builder.ToImmutable();
    }
  }
}
