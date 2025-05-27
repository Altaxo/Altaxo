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
    public double[][] Data { get; private set; }

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


    /// <summary>
    /// Checks if the file format is a valid Q800 file format.
    /// </summary>
    /// <param name="rs">The data stream.</param>
    /// <returns>A boolean indicating if this is a valid Q800 file format. The second boolean indicates if this is a hybrid file, consisting
    /// of an ASCII part, and then a binary part.</returns>
    /// <exception cref="ArgumentNullException">nameof(rs)</exception>
    public static (bool isQ800File, bool isHybridBinary) CheckFileFormat(Stream rs)
    {
      if (rs is null)
      {
        throw new ArgumentNullException(nameof(rs));
      }

      // find the marker that is used to separate the ASCII header from the binary data
      var separatorPosition = FileIOExtensions.PositionOf(rs, [0x0D, 0x00, 0x0A, 0x00, 0x0C, 0x00]);
      rs.Seek(0, SeekOrigin.Begin);
      using (var sr = new StreamReader(rs, true))
      {
        // read the first line and check if it starts with "CLOSED"
        var firstLine = sr.ReadLine();
        if (firstLine is null || firstLine != "CLOSED")
        {
          return (false, false);
        }

        // Check if the second line starts with "Version"
        var secondLine = sr.ReadLine();
        if (secondLine is null || !secondLine.StartsWith("VERSION", StringComparison.OrdinalIgnoreCase))
        {
          return (false, false);
        }

        bool isNSigFound = false;
        while (sr.ReadLine() is { } line)
        {
          // check if the line starts with "Sig" which indicates that this is a valid Q800 file
          if (line.StartsWith("NSig", StringComparison.OrdinalIgnoreCase))
          {
            isNSigFound = true;
            break;
          }
        }
        if (!isNSigFound)
        {
          return (false, false);
        }

        if (separatorPosition > 0)
        {
          return (true, true);
        }

        // If this is not the hybrid binary format, we assume it is a pure text file
        bool isStartOfDataFound = false;
        while (sr.ReadLine() is { } line)
        {
          // check if the line starts with "Sig" which indicates that this is a valid Q800 file
          if (line.StartsWith("StartOfData", StringComparison.OrdinalIgnoreCase))
          {
            isStartOfDataFound = true;
            break;
          }
        }

        return (isStartOfDataFound, false);
      }
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
      bool isHybridBinary = separatorPosition > 0;
      rs.Seek(0, SeekOrigin.Begin);

      // Read the header
      using (var sr = new StreamReader(rs, true))
      {
        if (sr.ReadLine() is not { } firstLine)
        {
          throw new InvalidDataException($"The stream does not contain a valid line");
        }
        if (firstLine != "CLOSED")
        {
          throw new InvalidDataException($"The stream does not contain a valid Q800 file header. The first line is {firstLine}");
        }
        if (sr.ReadLine() is not { } secondLine)
        {
          throw new InvalidDataException($"The stream does not contain a valid 2nd line");
        }
        if (!secondLine.StartsWith("VERSION", StringComparison.OrdinalIgnoreCase))
        {
          throw new InvalidDataException($"The stream does not contain a valid Q800 file header. The second line is {secondLine}, but it should start with 'Version'");
        }

        int numberOfSignals = 0;
        while (sr.ReadLine() is { } line)
        {
          notes.AppendLine(line);
          if (line.StartsWith("NSig", StringComparison.OrdinalIgnoreCase))
          {
            numberOfSignals = int.Parse(line.Split(new char[] { ' ', '\t' }, 2)[1], System.Globalization.CultureInfo.InvariantCulture);
            break;
          }
        }

        if (numberOfSignals <= 0)
        {
          throw new InvalidDataException($"The stream does not contain a valid number of signals. The NSig line is missing or invalid.");
        }

        for (int i = 0; i < numberOfSignals; i++)
        {
          ++numberOfLines;
          var line = sr.ReadLine();
          if (line is null)
          {
            throw new InvalidDataException($"The stream does not contain a valid signal line. Expected {i + 1} lines, but got only {numberOfLines} lines.");
          }
          if (!line.StartsWith(FormattableString.Invariant($"Sig{i + 1}")))
          {
            throw new InvalidDataException($"The stream does not contain a valid signal line. The line {i + 1} is {line}, but it should start with 'Sig'");
          }

          string lastPart = line.Split(new char[] { ' ', '\t' }, 2)[1];
          string[] nameAndUnit = lastPart.Split(new char[] { '(', ')' });
          string name = nameAndUnit[0].Trim();
          string unit = nameAndUnit.Length > 1 ? nameAndUnit[1].Trim() : string.Empty;

          columnNames.Add(name);
          units.Add(unit);
        }

        if (isHybridBinary)
        {
          // if this is a hybrid binary file, we have to read until the separator is found
          while (sr.ReadLine() is { } line && line.Length >= 3 && char.IsLetter(line[0]) && char.IsLetter(line[1]) && char.IsLetter(line[2]))
          {
            notes.AppendLine(line);
          }
          Data = ReadBinaryData(rs, separatorPosition, numberOfSignals);
        }
        else // pure Ascii file
        {
          // if this is supposed to be a pure Ascii file, we have to read until "StartOfData" is found
          while (sr.ReadLine() is { } line)
          {
            if (line.StartsWith("StartOfData"))
            {
              break;
            }
            notes.AppendLine(line);
          }

          Data = ReadAsciiData(sr, numberOfSignals);
        }
      }

      if (convertUnitsToSIUnits)
      {
        // if opted for, we convert the numbers to SI units
        for (int i = 0; i < units.Count; ++i)
        {
          string unit = units[i];
          string? newUnit = unit;
          double unitFactor = 1.0;
          foreach (var unitConversion in _unitConversion)
          {
            if (unit.StartsWith(unitConversion.oldUnit) &&
                (unit.Length == unitConversion.oldUnit.Length ||
                  (unit.Length > unitConversion.oldUnit.Length && char.IsWhiteSpace(unit[unitConversion.oldUnit.Length]))
                )
              )
            {
              newUnit = unitConversion.newUnit + unit.Substring(unitConversion.oldUnit.Length);
              unitFactor = unitConversion.factor;
              break;
            }
          }

          units[i] = newUnit;
          if (unitFactor != 1 || isHybridBinary)
          {
            for (int j = 0; j < Data[i].Length; ++j)
            {
              Data[i][j] = RoundToFloatPrecision(Data[i][j] * unitFactor);
            }
          }
        }
      }

      ColumnNames = columnNames.ToArray();
      Units = units.ToArray();
      NumberOfRows = Data[0].Length;
      Notes = notes.ToString();
    }

    private static double[][] ReadBinaryData(Stream rs, long separatorPosition, int numberOfSignals)
    {
      rs.Seek(separatorPosition + 7, SeekOrigin.Begin);
      var buffer = new byte[numberOfSignals * sizeof(float)];
      var rows = (rs.Length - rs.Position) / (numberOfSignals * sizeof(float));
      var data = Enumerable.Range(0, numberOfSignals).Select(_ => new float[rows]).ToArray();

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

        for (int j = 0; j < numberOfSignals; ++j)
          data[j][idxRow] = BitConverter.ToSingle(buffer, j * sizeof(float));
      }

      return Enumerable.Range(0, numberOfSignals)
        .Select(i => data[i].Take(readrows).Select(x => (double)x).ToArray())
        .ToArray();
    }

    private static double[][] ReadAsciiData(StreamReader sr, int numberOfSignals)
    {
      var dataRows = new List<double[]>();
      while (sr.ReadLine() is { } line)
      {
        var parts = line.Split(new char[] { ' ', '\t' }, StringSplitOptions.None);

        if (parts.Length < numberOfSignals)
        {
          throw new InvalidDataException($"The line '{line}' does not contain enough data for {numberOfSignals} signals.");
        }

        var dataRow = new double[numberOfSignals];
        for (int i = 0; i < numberOfSignals; i++)
        {
          if (!double.TryParse(parts[i], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out double value))
          {
            throw new InvalidDataException($"The value '{parts[i]}' in line '{line}' is not a valid double.");
          }
          dataRow[i] = value;
        }
        dataRows.Add(dataRow);
      }

      return Enumerable.Range(0, numberOfSignals)
        .Select(i =>
          Enumerable.Range(0, dataRows.Count).Select(
            j => dataRows[j][i])
          .ToArray())
        .ToArray();
    }

    private static double RoundToFloatPrecision(double value)
    {
      return double.Parse(value.ToString("G7", System.Globalization.CultureInfo.InvariantCulture), System.Globalization.CultureInfo.InvariantCulture);
    }
  }
}
