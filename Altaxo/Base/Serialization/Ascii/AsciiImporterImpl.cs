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
  public record AsciiImporterImpl : IDataFileImporter, Main.IImmutable
  {
    /// <summary>Prepend this string to a file name in order to designate the stream origin as file name origin.</summary>
    public const string FileUrlStart = @"file:///";

    /// <inheritdoc/>
    public (IReadOnlyList<string> FileExtensions, string Explanation) GetFileExtensions()
    {
      return ([".txt", ".csv", ".dat"], "ASCII files (*.txt;*.csv;*.dat)");
    }

    /// <inheritdoc/>
    public double GetProbabilityForBeingThisFileFormat(string fileName)
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
          p += 0.5;
      }
      catch
      {
        p = 0;
      }

      return p;
    }

    /// <inheritdoc/>
    public string? Import(IReadOnlyList<string> filenames, DataTable table, bool attachDataSource)
    {
      if (filenames.Count == 1)
      {
        AsciiImporter.ImportFromAsciiFile(table, filenames[0], out var options);
        if (attachDataSource && !(table.DataSource is AsciiImportDataSource))
        {
          table.DataSource = new AsciiImportDataSource(filenames[0], options);
        }
        return null;
      }
      else
      {
        var options = new AsciiImportOptions();
        AsciiImporter.TryImportFromMultipleAsciiFilesHorizontally(table, filenames, true, options, out var errors);
        if (attachDataSource && !(table.DataSource is AsciiImportDataSource))
        {
          table.DataSource = new AsciiImportDataSource(filenames[0], options);
        }
        return errors;
      }
    }

  } // end class
}
