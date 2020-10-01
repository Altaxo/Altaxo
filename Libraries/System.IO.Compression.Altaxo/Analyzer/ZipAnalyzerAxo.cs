#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2020 Dr. Dirk Lellinger
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
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.IO.Compression
{
  /// <summary>
  /// Contains methods to test and analyze zip archive files for errors.
  /// </summary>
  public class ZipAnalyzerAxo
  {

    public static void Analyze(string inputFileName)
    {
      using var fs = new FileStream(inputFileName, FileMode.Open, FileAccess.Read, FileShare.None);

      fs.Seek(0, SeekOrigin.Begin);

      // AnalyzeOrderOfFileHeaders(fs);
      Analyze(fs);
    }

    private static int ReadInt32(Stream str)
    {
      var buffer = new byte[4];
      var rd = str.Read(buffer, 0, 4);
      if (rd == 4)
        return BitConverter.ToInt32(buffer, 0);
      else
        throw new InvalidOperationException($"Unexpected end of file at position {str.Position}");
    }

    /// <summary>
    /// Compares the data of the local file header with the data of the central directory entry (except of the file names).
    /// </summary>
    /// <param name="lfh">The local file header.</param>
    /// <param name="entry">The central directory record.</param>
    /// <returns>
    ///   <c>true</c> if data in the local file header and the central directory entry are the same; otherwise, <c>false</c>.
    /// </returns>
    protected static bool IsEqual(LocalFileHeader lfh, CentralDirectoryRecord entry)
    {
      if (lfh.VersionNeeded != entry.VersionNeeded)
        return false;

      if (lfh.CompressionMethod != entry.CompressionMethod)
        return false;

      if (lfh.FileLastModificationTime != entry.FileLastModificationTime)
        return false;

      if (lfh.FileNameLength != entry.FileNameLength)
        return false;

      if (lfh.Crc != entry.Crc)
        return false;


      if (lfh.CompressedSize != entry.CompressedSize)
        return false;

      if (lfh.UncompressedSize != entry.UncompressedSize)
        return false;

      return true;
    }

    /// <summary>
    /// Compares the data of the local file header with the data of the central directory entry (except of the file names).
    /// </summary>
    /// <param name="lfh">The local file header.</param>
    /// <param name="entry">The central directory record.</param>
    /// <returns>
    ///   <c>true</c> if data in the local file header and the central directory entry are the same; otherwise, <c>false</c>.
    /// </returns>
    protected static bool IsEqual(in LocalFileHeaderStruct lfh, in CentralDirectoryRecord entry)
    {
      if (lfh.VersionNeeded != entry.VersionNeeded)
        return false;

      if (lfh.CompressionMethod != entry.CompressionMethod)
        return false;

      if (lfh.FileLastModificationTime != entry.FileLastModificationTime)
        return false;

      if (lfh.FileNameLength != entry.FileNameLength)
        return false;

      if (lfh.Crc != entry.Crc)
        return false;


      if (lfh.CompressedSize != entry.CompressedSize)
        return false;

      if (lfh.UncompressedSize != entry.UncompressedSize)
        return false;

      return true;
    }

    /// <summary>
    /// Try to get (find) the end of central directory record in the zip archive.
    /// </summary>
    /// <param name="str">The zip archive stream.</param>
    /// <param name="endOfCentralDirectory">On successfull return, contains the end of central directory record.</param>
    /// <returns>True if successful; otherwise, false.</returns>
    /// <exception cref="InvalidOperationException"></exception>
    protected static bool TryGetEndOfCentralDirectoryRecord(Stream str, [MaybeNullWhen(false)] out EndOfCentralDirectoryRecord endOfCentralDirectory)
    {
      var length = str.Length;

      // Test if central directory is directly at the end (without comment)
      str.Seek(str.Length - EndOfCentralDirectoryRecord.MinimumSizeOfStructure, SeekOrigin.Begin);
      if (ReadInt32(str) == EndOfCentralDirectoryRecord.EndOfCentralDirectorySignature)
      {
        endOfCentralDirectory = EndOfCentralDirectoryRecord.Create(str);
        return true;
      }
      else
      {
        int maxBufSize = (int)Math.Min(length, 65535 + 22); // Max Distance of stream end from begin of EndOfCentralDirectory
        var buffer = new byte[maxBufSize];
        var offs = length - maxBufSize;
        str.Seek(offs, SeekOrigin.Begin);
        if (!(maxBufSize == str.Read(buffer, 0, buffer.Length)))
          throw new InvalidOperationException();

        // now try to find the EOCD signature backwards
        for (int i = buffer.Length - 4; i >= 0; --i)
        {
          if (buffer[i] == 0x50 && buffer[i + 1] == 0x4b && buffer[i + 2] == 0x05 && buffer[i + 3] == 0x06)
          {
            str.Seek(offs + i + 4, SeekOrigin.Begin);
            endOfCentralDirectory = EndOfCentralDirectoryRecord.Create(str);
            return true;
          }
        }

        endOfCentralDirectory = null;
        return false;
      }
    }

    /// <summary>
    /// Visits the data in the 'End of central directory record' and determines if the data therein are consistent.
    /// </summary>
    /// <param name="str">The zip archive stream.</param>
    /// <param name="endOfCentralDirectory">The end of central directory record.</param>
    /// <param name="ErrorMessage">On return, if the return value is false, contains the error message.</param>
    /// <returns>
    ///   <c>true</c> if the data in the end of central directory record are okay; otherwise, <c>false</c>.
    /// </returns>
    protected static bool IsEndOfCentralDirectoryOkay(Stream str, EndOfCentralDirectoryRecord endOfCentralDirectory, out string ErrorMessage)
    {
      // Analyze endOfCentralDirectory

      if (endOfCentralDirectory.NumberOfThisDisk != 0)
      {
        ErrorMessage = "EndOfCentralDirectory record: NumberOf this disk in EndOfCentralDirectory is != 0. Multi-archives are not supported by this analyzer";
        return false;
      }
      if (endOfCentralDirectory.NumberOfCentralDirectoryRecordsOnThisDisk != endOfCentralDirectory.TotalNumberOfCentralDirectoryRecords)
      {
        ErrorMessage = "EndOfCentralDirectory record: Total number of central directory records not equal to number of central directory records on this disk";
        return false;
      }
      if (endOfCentralDirectory.NumberOfCentralDirectoryRecordsOnThisDisk == 0 && endOfCentralDirectory.SizeOfCentralDirectoryInBytes > 0)
      {
        ErrorMessage = "EndOfCentralDirectory record: Number of central directory records is 0, but size of central directory records is >0";
        return false;
      }
      if (endOfCentralDirectory.NextStreamPosition != str.Length)
      {
        ErrorMessage = "EndOfCentralDirectory record: End of central directory is not located at the end of the stream";
        return false;
      }
      if (endOfCentralDirectory.PositionOfCentralDirectory + endOfCentralDirectory.SizeOfCentralDirectoryInBytes > endOfCentralDirectory.OriginalStreamPosition)
      {
        ErrorMessage = "EndOfCentralDirectory record: Central directory overlaps with EndOfCentralDirectory record";
        return false;
      }

      ErrorMessage = string.Empty;
      return true;
    }

    /// <summary>
    /// Determines whether the zip archive (provided here as file stream) is okay. In contrast to <see cref="Analyze(Stream)"/>,
    /// this method returns as soon as the first error is encountered.
    /// </summary>
    /// <param name="str">The file stream of the zip archive. The stream must be readable and seekable. It is not closed
    /// after this call, and the position of the stream is arbitrary.</param>
    /// <param name="options">The test options.</param>
    /// <param name="ErrorMessage">If the return value is false, this parameter contains the error message.</param>
    /// <returns>True if the provided zip archive is okay; otherwise, <c>false</c>.
    /// </returns>
    /// <exception cref="ArgumentNullException">str</exception>
    /// <exception cref="ArgumentException">Stream is not readable and seekabl</exception>
    public static bool IsZipFileOkay(Stream str, ZipAnalyzerOptions options, out string ErrorMessage)
    {
      if (str is null)
        throw new ArgumentNullException(nameof(str));
      if (!(str.CanRead && str.CanSeek))
        throw new ArgumentException("Stream is not readable and seekable", nameof(str));

      try
      {

        if (!TryGetEndOfCentralDirectoryRecord(str, out var endOfCentralDirectory))
        {
          ErrorMessage = "EndOfCentralDirectory record not found";
          return false;
        }

        if (!IsEndOfCentralDirectoryOkay(str, endOfCentralDirectory, out ErrorMessage))
        {
          return false;
        }

        // Now read the central directory records
        str.Seek(endOfCentralDirectory.PositionOfCentralDirectory, SeekOrigin.Begin);
        var buffer = new byte[endOfCentralDirectory.SizeOfCentralDirectoryInBytes];
        if (buffer.Length != str.Read(buffer, 0, buffer.Length))
        {
          ErrorMessage = $"Unexpected end of file while reading central directory";
          return false;
        }

        int bufferOffset = 0;
        var centralDirectory = new List<CentralDirectoryRecord>(endOfCentralDirectory.TotalNumberOfCentralDirectoryRecords);
        for (int i = 0; i < endOfCentralDirectory.NumberOfCentralDirectoryRecordsOnThisDisk; ++i)
        {

          try
          {
            var cde = CentralDirectoryRecord.Create(buffer, bufferOffset, endOfCentralDirectory.PositionOfCentralDirectory);

            if (cde.FileNameLength == 0)
            {
              ErrorMessage = $"Empty file name of central directory record [{i}] at file position {(endOfCentralDirectory.PositionOfCentralDirectory + bufferOffset):X}";
              return false;
            }


            centralDirectory.Add(cde);
            bufferOffset += cde.TotalSize;


          }
          catch (Exception ex)
          {
            ErrorMessage = $"Invalid central directory entry signature at file position {(endOfCentralDirectory.PositionOfCentralDirectory + bufferOffset):X}, Message: {ex.Message}";
            return false;
          }
        }

        if (options.HasFlag(ZipAnalyzerOptions.TestCentralDirectoryForNameDublettes))
        {
          var names = new HashSet<string>();
          foreach (var cde in centralDirectory)
          {
            if (names.Contains(cde.FileName!))
            {
              ErrorMessage = $"Found more than one central directory record with the name {cde.FileName}";
              return false;
            }
            else
            {
              names.Add(cde.FileName!);
            }
          }
        }

        if (options.HasFlag(ZipAnalyzerOptions.TestStrictOrderOfLocalFileHeaders))
        {
          // Test the strict order of entries - this test is only valid for fresh Zip files (unmanipulated)
          long expectedPosition = 0;
          for (int i = 0; i < centralDirectory.Count; ++i)
          {
            if (!(expectedPosition == centralDirectory[i].RelativeOffsetToLocalFileHeader))
            {
              ErrorMessage = $"Local file header[{i}] expected at position {expectedPosition:X}, but is at {centralDirectory[i].RelativeOffsetToLocalFileHeader:X}";
              return false;
            }
            expectedPosition += centralDirectory[i].FileNameLength + centralDirectory[i].CompressedSize + LocalFileHeader.MinimumSizeOfStructure;
          }
          if (!(expectedPosition == endOfCentralDirectory.PositionOfCentralDirectory))
          {
            ErrorMessage = $"Position of central directory is expected at position {expectedPosition:X}, but is at {endOfCentralDirectory.PositionOfCentralDirectory:X}";
            return false;
          }
        }


        if (options.HasFlag(ZipAnalyzerOptions.TestExistenceOfTheLocalFileHeaders))
        {
          var lfhbuffer = new byte[LocalFileHeaderStruct.MinimumSizeOfStructure];
          // now, get all the local files listed in the central directory
          for (int i = 0; i < centralDirectory.Count; ++i)
          {
            var entry = centralDirectory[i];
            str.Seek(entry.RelativeOffsetToLocalFileHeader, SeekOrigin.Begin);
            if (lfhbuffer.Length != str.Read(lfhbuffer, 0, lfhbuffer.Length))
            {
              ErrorMessage = "Unexpected end of file reading local file header at stream position 0x{entry.RelativeOffsetToLocalFileHeader:X}";
              return false;
            }

            try
            {
              var lfh = LocalFileHeaderStruct.Create(lfhbuffer, entry);
              if (!IsEqual(lfh, entry))
              {
                ErrorMessage = $"Local file header of CDE[{i}], file name: {entry.FileName} is unequal to corresponding central directory entry.";
                return false;
              }
            }
            catch (Exception ex)
            {
              ErrorMessage = $"Error creating local file header of CDE[{i}], file name: {entry.FileName}, Message: {ex.Message}";
              return false;
            }
          }
        }


        ErrorMessage = string.Empty;
        return true;
      }
      catch (Exception ex)
      {
        ErrorMessage = $"Error testing zip archive stream, Message: {ex.Message}";
        return false;
      }
    }

    public static ZipAnalyzerResult Analyze(Stream str)
    {
      var result = new ZipAnalyzerResult();

      var stb = new StringBuilder();

      if (!TryGetEndOfCentralDirectoryRecord(str, out var endOfCentralDirectory))
      {
        result.EndOfCentralDirectoryNotFound = true;
        result.ErrorMessage = "EndOfCentralDirectory record: EndOfCentralDirectory record not found";
        return result;
      }

      if (!IsEndOfCentralDirectoryOkay(str, endOfCentralDirectory, out var msg1))
      {
        result.ErrorMessage = msg1;
        return result;
      }




      str.Seek(endOfCentralDirectory.PositionOfCentralDirectory, SeekOrigin.Begin);

      int signature;
      var listCentralDirectory = new List<CentralDirectoryRecord>();
      for (int i = 0; i < endOfCentralDirectory.NumberOfCentralDirectoryRecordsOnThisDisk; ++i)
      {
        signature = ReadInt32(str);
        if (signature != 0x02014b50)
        {
          result.CentralDirectoryCorrupt = true;
          result.ErrorMessage = $"Invalid central directory entry signature at file position {str.Position - 4:X}";
          return result;
        }
        var centralDirectoryRecord = CentralDirectoryRecord.Create(str, true, false);
        listCentralDirectory.Add(centralDirectoryRecord);
        str.Seek(centralDirectoryRecord.NextStreamPosition, SeekOrigin.Begin);
      }

      if (endOfCentralDirectory.NumberOfCentralDirectoryRecordsOnThisDisk != listCentralDirectory.Count)
      {
        result.ErrorMessage = "EndOfCentralDirectory record: Number of records in central directory is not equal to the number reported in EndOfCentralDirectory record";
        return result;
      }

      {
        // Test the order of entries - this test is only valid for unmanipulated entries
        long expectedPosition = 0;
        for (int i = 0; i < listCentralDirectory.Count; ++i)
        {
          if (!(expectedPosition == listCentralDirectory[i].RelativeOffsetToLocalFileHeader))
          {
            stb.AppendLine($"Local file header[{i}] expected at position {expectedPosition:X}, but is at {listCentralDirectory[i].RelativeOffsetToLocalFileHeader:X}");
          }
          expectedPosition += listCentralDirectory[i].FileNameLength + listCentralDirectory[i].CompressedSize + LocalFileHeader.MinimumSizeOfStructure;
        }
        if (!(expectedPosition == endOfCentralDirectory.PositionOfCentralDirectory))
        {
          stb.AppendLine($"Position of central directory is expected at position {expectedPosition:X}, but is at {endOfCentralDirectory.PositionOfCentralDirectory:X}");
        }
      }


      // Test, whether or not the file names in the central directory are unique
      var dict = new Dictionary<string, List<CentralDirectoryRecord>>();
      foreach (var entry in listCentralDirectory)
      {
        if (dict.TryGetValue(entry.FileName, out var entryList))
          entryList.Add(entry);
        else
          dict.Add(entry.FileName, new List<CentralDirectoryRecord> { entry });
      }

      foreach (var entry in dict)
      {
        if (entry.Value.Count > 1)
          stb.AppendLine($"Dublette found in central directory: Count={entry.Value.Count}, Name: {entry.Key}");
      }


      // now, get all the local files listed in the central directory
      var commonList = new List<(CentralDirectoryRecord CDE, LocalFileHeaderStruct LFH)>();
      var lfhbuffer = new byte[LocalFileHeader.MinimumSizeOfStructure];
      for (int i = 0; i < listCentralDirectory.Count; ++i)
      {
        var entry = listCentralDirectory[i];
        str.Seek(entry.RelativeOffsetToLocalFileHeader, SeekOrigin.Begin);
        if (lfhbuffer.Length != str.Read(lfhbuffer, 0, lfhbuffer.Length))
        {
          stb.AppendLine("Unexpected end of file reading local file header at stream position 0x{entry.RelativeOffsetToLocalFileHeader:X}");
          continue;
        }

        LocalFileHeaderStruct lfh;
        try
        {
          lfh = LocalFileHeaderStruct.Create(lfhbuffer, entry);
          if (!IsEqual(lfh, entry))
          {
            stb.AppendLine($"Local file header of CDE[{i}], file name: {entry.FileName} is unequal to corresponding central directory entry.");
          }
          commonList.Add((entry, lfh));
        }
        catch (Exception ex)
        {
          stb.AppendLine($"Error creating local file header of CDE[{i}], file name: {entry.FileName}, Message: {ex.Message}");
        }
      }

      // now test all checksums
      var buffer = new byte[1024 * 1024];
      foreach (var entry in commonList)
      {
        var lfh = entry.LFH;
        var cde = entry.CDE;

        long streamPositionOfContentStart = cde.RelativeOffsetToLocalFileHeader + lfh.SizeOfLocalFileHeader;
        str.Seek(streamPositionOfContentStart, SeekOrigin.Begin);


        try
        {
          var partialStream = new ReadOnlyPartialStreamView(str, streamPositionOfContentStart, lfh.CompressedSize);
          uint checksum = 0;
          using var decompressedStream = new System.IO.Compression.DeflateStream(partialStream, CompressionMode.Decompress, leaveOpen: true);

          long totalRead = 0;
          int readNow;
          do
          {
            readNow = decompressedStream.Read(buffer, 0, buffer.Length);
            totalRead += readNow;
            if (readNow > 0)
            {
              checksum = Crc32.UpdateChecksum(checksum, buffer, 0, readNow);
            }

          } while (readNow > 0);
          if (!(checksum == lfh.Crc))
          {
            stb.AppendLine($"Checksum mismatch in file {entry.CDE.FileName}");
          }
          if (totalRead != lfh.UncompressedSize)
          {
            stb.AppendLine($"Compressed size mismatch in file {entry.CDE.FileName}");
          }
        }
        catch (Exception ex)
        {
          stb.AppendLine($"Error decoding file {entry.CDE.FileName} : {ex.Message}");
        }
      }

      result.ErrorMessage = stb.ToString();
      return result;
    }
  }

}
