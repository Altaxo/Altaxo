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
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;

namespace Altaxo.Serialization
{
  /// <summary>
  /// Contains helpers for making names safe for use in the file system.
  /// </summary>
  public static class FileIOHelper
  {
    public static ImmutableDictionary<char, char> InvalidFileNameChars = new Dictionary<char, char>()
    {
      [(char)0x00] = (char)0xF001,
      [(char)0x01] = (char)0xF002,
      [(char)0x02] = (char)0xF003,
      [(char)0x03] = (char)0xF004,
      [(char)0x04] = (char)0xF005,
      [(char)0x05] = (char)0xF006,
      [(char)0x06] = (char)0xF007,
      [(char)0x07] = (char)0xF008,
      [(char)0x08] = (char)0xF009,
      [(char)0x09] = (char)0xF00A,
      [(char)0x0A] = (char)0xF00B,
      [(char)0x0B] = (char)0xF00C,
      [(char)0x0C] = (char)0xF00D,
      [(char)0x0D] = (char)0xF00E,
      [(char)0x0E] = (char)0xF00F,
      [(char)0x0F] = (char)0xF010,
      [(char)0x10] = (char)0xF011,
      [(char)0x11] = (char)0xF012,
      [(char)0x12] = (char)0xF013,
      [(char)0x13] = (char)0xF014,
      [(char)0x14] = (char)0xF015,
      [(char)0x15] = (char)0xF016,
      [(char)0x16] = (char)0xF017,
      [(char)0x17] = (char)0xF018,
      [(char)0x18] = (char)0xF019,
      [(char)0x19] = (char)0xF01A,
      [(char)0x1A] = (char)0xF01B,
      [(char)0x1B] = (char)0xF01C,
      [(char)0x1C] = (char)0xF01D,
      [(char)0x1D] = (char)0xF01E,
      [(char)0x1E] = (char)0xF01F,
      [(char)0x1F] = (char)0xF020,
      [':'] = '፥',
      ['*'] = '✶',
      ['?'] = '¿',
      ['\"'] = '”',
      ['<'] = '⋖',
      ['>'] = '⋗',
      ['|'] = '∣',
      ['/'] = '⁄',
    }.ToImmutableDictionary();

    /// <summary>
    /// Converts the file extensions to file filter.
    /// </summary>
    /// <param name="value">The value, containing the file extensions and the description.</param>
    /// <returns>File filter (which are the file extensions prepended with a joker star), and separated by a semicolon. The description is left unchanged.</returns>
    public static (string Filter, string Description) GetFilterDescriptionForExtensions((IReadOnlyList<string> Extensions, string Description) value)
    {
      return (string.Join(";", value.Extensions.Select(x => "*" + x)), value.Description);
    }

    /// <summary>
    /// Gets the filter description for all files.
    /// </summary>
    /// <returns>The file filter and the description for all files.</returns>
    public static (string Filter, string Description) GetFilterDescriptionForAllFiles()
    {
      return ("*.*", "All files (*.*)");
    }

    /// <summary>
    /// Shows a file import dialog for the specific importer provided as parameter, and imports the files to the table if the user clicked on "OK".
    /// </summary>
    /// <param name="table">The table to import the files to.</param>
    /// <param name="importer">The data file importer.</param>
    public static void ShowDialog(Altaxo.Data.DataTable table, IDataFileImporter importer)
    {
      var options = new Altaxo.Gui.OpenFileOptions();
      var filter = FileIOHelper.GetFilterDescriptionForExtensions(importer.GetFileExtensions());
      options.AddFilter(filter.Filter, filter.Description);
      filter = FileIOHelper.GetFilterDescriptionForAllFiles();
      options.AddFilter(filter.Filter, filter.Description);
      options.FilterIndex = 0;
      options.Multiselect = true; // allow selecting more than one file

      if (Current.Gui.ShowOpenFileDialog(options))
      {
        // if user has clicked ok, import all selected files into Altaxo
        string[] filenames = options.FileNames;
        Array.Sort(filenames); // Windows seems to store the filenames reverse to the clicking order or in arbitrary order

        string? errors = importer.Import(filenames, table, true);

        if (errors is not null)
        {
          Current.Gui.ErrorMessageBox(errors, "Some errors occured during import!");
        }
      }
    }

    /// <summary>
    /// Get a valid full file name out of a raw file name that can contain invalid characters. Those characters are
    /// replaced by unicode chars. Here, special measures are taken to not accidentally replace the question mark and colon in absolute file names,
    /// starting with e.g. C:\... or \\?\C:\....
    /// </summary>
    /// <param name="name">The full path name of a file or folder</param>
    /// <returns>A path name that can be used for storing a file or creating a folder in the file system.</returns>
    public static string GetValidFullPathName(string name)
    {
      StringBuilder? stb = null;
      int start = 0;
      if (name.Length >= 2 && char.IsLetter(name[0]) && name[1] == ':') // if the name start with drive letter and colon, we ignore the colon
      {
        start = 2;
      }
      else if (name.StartsWith(@"\\?\")) // in UNC path names, we ignore the question mark
      {
        start = 4;
        if (name.Length >= 6 && char.IsLetter(name[4]) && name[5] == ':') // and possibly the colon also
        {
          start = 6;
        }
      }

      for (int i = start; i < name.Length; ++i)
      {
        var c = name[i];
        if (InvalidFileNameChars.ContainsKey(c))
        {
          stb ??= new StringBuilder(name);
          stb[i] = InvalidFileNameChars[c]; // we replace this character with a box character
        }
      }
      return stb is null ? name : stb.ToString();
    }

    /// <summary>
    /// Get a valid file name part out of a raw file name that can contain invalid characters. Those characters are
    /// replaced by unicode characters. Attention: the provided name should not be an absolute file name. This is because
    /// here, in a file name like C:\Temp.txt, the colon would be replaced by a unicode character. If an absolute name needs
    /// to be processed, use the function <see cref="GetValidFullPathName(string)"/>. Backslash chars are sustained.
    /// </summary>
    /// <param name="name">The name. Should not be an absolute file name.</param>
    /// <returns>The name without invalid characters, so that it can be used for a name part in the file system.</returns>
    public static string GetValidPathNameFragment(string name)
    {
      StringBuilder? stb = null;
      for (int i = 0; i < name.Length; ++i)
      {
        var c = name[i];
        if (InvalidFileNameChars.ContainsKey(c))
        {
          stb ??= new StringBuilder(name);
          stb[i] = InvalidFileNameChars[c]; // we replace this character with a box character
        }
      }
      return stb is null ? name : stb.ToString();
    }


    /// <summary>
    /// Reads the data into a buffer. Ensures that the provided number of bytes is really read. If not, a <see cref="EndOfStreamException"/> is thrown.
    /// </summary>
    /// <param name="stream">The stream.</param>
    /// <param name="buffer">The buffer.</param>
    /// <param name="offset">The offset where the first read byte should be stored into the buffer.</param>
    /// <param name="length">The number of bytes that must be read.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">length</exception>
    /// <exception cref="System.IO.IOException">Could not read any data from the stream</exception>
    public static void ForcedRead(this Stream stream, byte[] buffer, int offset, int length)
    {
      if (length <= 0)
      {
        throw new System.ArgumentOutOfRangeException(nameof(length));
      }
      while (length > 0)
      {
        var read = stream.Read(buffer, offset, length);
        if (read == 0)
        {
          throw new System.IO.EndOfStreamException();
        }
        length -= read;
        offset += read;
      }
    }
  }
}
