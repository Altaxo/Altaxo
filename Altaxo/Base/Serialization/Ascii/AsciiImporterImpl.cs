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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Altaxo.Data;

namespace Altaxo.Serialization.Ascii
{
  /// <summary>
  /// Central class for import of ascii data.
  /// </summary>
  public record AsciiImporterImpl : DataFileImporterBase, Main.IImmutable
  {
    /// <summary>Prepend this string to a file name in order to designate the stream origin as file name origin.</summary>
    public const string FileUrlStart = @"file:///";

    /// <inheritdoc/>
    public override (IReadOnlyList<string> FileExtensions, string Explanation) GetFileExtensions()
    {
      return ([".txt", ".csv", ".dat"], "ASCII files (*.txt;*.csv;*.dat)");
    }

    /// <inheritdoc/>
    public override object CheckOrCreateImportOptions(object? importOptions)
    {
      return (importOptions as AsciiImportOptions) ?? new AsciiImportOptions();
    }

    /// <inheritdoc/>
    public override IAltaxoTableDataSource? CreateTableDataSource(IReadOnlyList<string> fileNames, object importOptions)
    {
      return new AsciiImportDataSource(fileNames, (AsciiImportOptions)importOptions);
    }

    /// <inheritdoc/>
    public override double GetProbabilityForBeingThisFileFormat(string fileName)
    {
      double p = 0;
      var fe = GetFileExtensions();
      if (fe.FileExtensions.ToHashSet().Contains(Path.GetExtension(fileName).ToLowerInvariant()))
      {
        p += 0.5;
      }

      try
      {
        using var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
        var reader = new StreamReader(stream, true);
        var buffer = new char[16384];
        var len = reader.ReadBlock(buffer, 0, buffer.Length);
        int sumSpecialChars = 0;
        for (int i = 0; i < len; i++)
        {
          var c = buffer[i];
          if ((c < 0x20 || c >= 0x80) && !(c == 0x0D || c == 0x0A))
            sumSpecialChars++;
        }

        if (len > 10 && sumSpecialChars * 4 < len)
          p += 0.499;  // allow other, more specific text importers to have a higher probability
      }
      catch
      {
        p = 0;
      }

      return p;
    }

    /// <inheritdoc/>
    public override string? Import(IReadOnlyList<string> filenames, DataTable table, object importOptionsObj, bool attachDataSource)
    {
      var importOptions = (AsciiImportOptions)importOptionsObj;

      if (filenames.Count == 1)
      {
        AsciiImporter.ImportFromAsciiFile(table, filenames[0], out var options);
        if (attachDataSource && !(table.DataSource is AsciiImportDataSource))
        {
          table.DataSource = CreateTableDataSource(filenames, options);
        }
        return null;
      }
      else
      {
        var options = importOptions ?? new AsciiImportOptions();
        AsciiImporter.TryImportFromMultipleAsciiFilesHorizontally(table, filenames, true, options, out var errors);
        if (attachDataSource && !(table.DataSource is AsciiImportDataSource))
        {
          table.DataSource = CreateTableDataSource(filenames, options);
        }
        return errors;
      }
    }

  } // end class
}
