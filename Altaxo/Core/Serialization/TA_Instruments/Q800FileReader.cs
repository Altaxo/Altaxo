#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2025 Dr. Dirk Lellinger
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
using System.IO;
using System.Linq;
using System.Text;

namespace Altaxo.Serialization.TA_Instruments
{
  /// <summary>
  /// Reads files from TA Instruments Q800 series. The format is a hybrid text and binary format.
  /// The text header is UTF-32 coded, and contains the metadata.
  /// After a sequence 0x0D, 0x00, 0x0A, 0x00, 0x0C, 0x00 follows the binary data, which consist of 32-bit floating point numbers (IEEE 754 format).
  /// </summary>
  public class Q800FileReader : Main.IImmutable
  {
    private static readonly (string oldUnit, string newUnit, double factor)[] _unitConversion =
      [
      ("min", "s", 60),
      ("MPa", "Pa",1E6),
      ("kPa", "Pa",1E3),
      ("µm",  "m", 1E-6),
      ("mm",  "m", 1E-3),
      ("%",   "",  1E-2),
    ];

    /// <summary>
    /// Gets the column names of the data.
    /// </summary>
    public string[] ColumnNames { get; private set; }

    /// <summary>
    /// Gets the units of the data columns.
    /// </summary>
    public string[] Units { get; private set; }

    /// <summary>
    /// Gets the data columns. Each element in the first level corresponds to a column, and each element in the second level corresponds to a row in that column.
    /// </summary>
    public float[][] Data { get; private set; }

    /// <summary>
    /// Gets the number of dat rows.
    /// </summary>
    public int NumberOfRows { get; private set; }

    /// <summary>
    /// Gets the notes (all metadata) of the file.
    /// </summary>
    public string Notes { get; private set; }

    /// <summary>
    /// Gets the number of data columns.
    /// </summary>
    public int NumberOfColumns => ColumnNames.Length;

    /// <summary>
    /// Creates a instance of the reader from a file.
    /// </summary>
    /// <param name="fileName">Name of the file.</param>
    /// <param name="convertUnitsToSIUnits">If set to <c>true</c>, the units are converted to SI units.</param>
    /// <returns>An instance of the reader.</returns>
    public static Q800FileReader FromFileName(string fileName, bool convertUnitsToSIUnits)
    {
      using var rs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
      return FromStream(rs, convertUnitsToSIUnits);
    }

    /// <summary>
    /// Creates a instance of the reader from a stream.
    /// </summary>
    /// <param name="rs">The stream.</param>
    /// <param name="convertUnitsToSIUnits">If set to <c>true</c>, the units are converted to SI units.</param>
    /// <returns>An instance of the reader.</returns>
    public static Q800FileReader FromStream(Stream rs, bool convertUnitsToSIUnits)
    {
      return new Q800FileReader(rs, convertUnitsToSIUnits);
    }



    private Q800FileReader(Stream rs, bool convertUnitsToSIUnits)
    {
      var notes = new StringBuilder();
      var units = new List<string>();
      var columnNames = new List<string>();

      int numberOfLines = 0;

      if (rs is null)
      {
        throw new ArgumentNullException(nameof(rs));
      }

      // find the marker that is used to separate the ASCII header from the binary data
      var separatorPosition = FileIOExtensions.PositionOf(rs, [0x0D, 0x00, 0x0A, 0x00, 0x0C, 0x00]);
      if (separatorPosition <= 0)
      {
        throw new InvalidDataException($"The stream is not a valid binary Q800 file because the binary separator sequence is missing");
      }

      // Read the header
      var buffer = new byte[separatorPosition + 4];
      rs.Seek(0, SeekOrigin.Begin);
      FileIOExtensions.ReadExactly(rs, buffer, 0, buffer.Length);
      var ms = new MemoryStream(buffer);
      using (var sr = new StreamReader(ms, true))
      {
        while (sr.ReadLine() is { } line)
        {
          ++numberOfLines;
          notes.AppendLine(line);
          if (line.StartsWith("Sig"))
          {
            string lastPart = line.Split(new char[] { ' ', '\t' }, 2)[1];
            string[] nameAndUnit = lastPart.Split(new char[] { '(', ')' });
            string name = nameAndUnit[0].Trim();
            string unit = nameAndUnit.Length > 1 ? nameAndUnit[1].Trim() : string.Empty;

            columnNames.Add(name);
            units.Add(unit);
          }
        }

        if (columnNames.Count == 0)
          throw new InvalidDataException($"The stream does not contain a header that starts with Sig1");
      }

      // now read the binary data

      rs.Seek(separatorPosition + 7, SeekOrigin.Begin);
      buffer = new byte[columnNames.Count * sizeof(float)];

      var rows = (rs.Length - rs.Position) / (columnNames.Count * sizeof(float));
      var data = Enumerable.Range(0, columnNames.Count).Select(_ => new float[rows]).ToArray();

      int readrows = 0;
      for (int idxRow = 0; idxRow < rows; ++idxRow)
      {
        FileIOExtensions.ReadExactly(rs, buffer, 0, buffer.Length);
        // break if the first number (time) is negative
        if (BitConverter.ToSingle(buffer, 0) < 0)
        {
          readrows = idxRow;
          break;
        }

        for (int j = 0; j < columnNames.Count; ++j)
          data[j][idxRow] = BitConverter.ToSingle(buffer, j * sizeof(float));
      }

      if (readrows != rows)
      {
        // if we read less rows than expected, we prune the columns to the new length
        PruneColumnsToLength(data, readrows);
        rows = readrows;
      }

      if (convertUnitsToSIUnits)
      {
        // if opted for, we convert the numbers to SI units
        for (int i = 0; i < units.Count; ++i)
        {
          string unit = units[i];
          for (int j = 0; j < _unitConversion.Length; ++j)
          {
            if (unit.StartsWith(_unitConversion[j].oldUnit) &&
                (unit.Length == _unitConversion[j].oldUnit.Length ||
                  (unit.Length > _unitConversion[j].oldUnit.Length && char.IsWhiteSpace(unit[_unitConversion[j].oldUnit.Length]))
                )
              )
            {
              MultiplyColumnWith(data, i, (float)_unitConversion[j].factor);
              units[i] = _unitConversion[j].newUnit + unit.Substring(_unitConversion[i].oldUnit.Length);
              break;
            }
          }
        }
      }
      ColumnNames = columnNames.ToArray();
      Units = units.ToArray();
      Data = data;
      NumberOfRows = readrows > 0 ? readrows : (int)rows;
      Notes = notes.ToString();
    }

    private static void MultiplyColumnWith(float[][] data, int columnIndex, double factor)
    {
      for (int i = 0; i < data[columnIndex].Length; ++i)
      {
        data[columnIndex][i] = (float)(data[columnIndex][i] * factor);
      }
    }

    private static void PruneColumnsToLength(float[][] data, int newLength)
    {
      if (newLength < 0)
      {
        throw new ArgumentOutOfRangeException(nameof(newLength), "New length must be between 0 and the current length of the column.");
      }
      for (int i = 0; i < data.Length; ++i)
      {
        Array.Resize(ref data[i], newLength);
      }
    }
  }
}
