#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2014 Dr. Dirk Lellinger
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
using Altaxo.Data;

namespace Altaxo.Serialization.Ascii
{
  public class AsciiImportDataSource : FileImportTableDataSourceBase<AsciiImportOptions>
  {
    #region Serialization

    #region Version 0

    /// <summary>
    /// 2014-07-28 initial version.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(AsciiImportDataSource), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (AsciiImportDataSource)obj;

        info.AddValue("ImportOptions", s._importOptions);
        info.AddValue("AsciiImportOptions", s._processOptions);
        info.AddArray("AsciiFiles", s._files.ToArray(), s._files.Count);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        if (o is AsciiImportDataSource s)
          s.DeserializeSurrogate0(info);
        else
          s = new AsciiImportDataSource(info, 0);
        return s;
      }
    }

    [MemberNotNull(nameof(_importOptions), nameof(_processOptions))]
    private void DeserializeSurrogate0(Altaxo.Serialization.Xml.IXmlDeserializationInfo info)
    {
      _isDeserializationInProgress = true;
      _importOptions = (IDataSourceImportOptions)info.GetValue("ImportOptions", this);
      _processOptions = (AsciiImportOptions)info.GetValue("AsciiImportOptions", this);
      var count = info.OpenArray("AsciiFiles");
      for (int i = 0; i < count; ++i)
        _files.Add((AbsoluteAndRelativeFileName)info.GetValue("e", this));
      info.CloseArray(count);

      info.AfterDeserializationHasCompletelyFinished += EhAfterDeserializationHasCompletelyFinished;
    }

    #endregion Version 0

    /// <summary>
    /// Deserialization constructor
    /// </summary>
    protected AsciiImportDataSource(Altaxo.Serialization.Xml.IXmlDeserializationInfo info, int version)
    {
      switch (version)
      {
        case 0:
          DeserializeSurrogate0(info);
          break;
        default:
          throw new ArgumentOutOfRangeException(nameof(version));
      }
    }

    #endregion Serialization

    #region Construction

    public AsciiImportDataSource(string fileName, AsciiImportOptions options)
      : this(new string[] { fileName }, options)
    {
    }

    public AsciiImportDataSource(IEnumerable<string> fileNames, AsciiImportOptions options)
      : base(fileNames)
    {
      _processOptions = options;
    }

    public AsciiImportDataSource(AsciiImportDataSource from)
      : base(from)
    {
    }

    public override object Clone() => new AsciiImportDataSource(this);

    #endregion Construction

    protected override void ImportFromFiles(string[] validFileNames, DataTable destinationTable, IProgressReporter reporter)
    {

      if (validFileNames.Length > 0)
      {
        if (validFileNames.Length == 1)
        {
          using (var stream = new System.IO.FileStream(validFileNames[0], System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.ReadWrite))
          {
            AsciiImporter.ImportFromAsciiStream(destinationTable, stream, AsciiImporterImpl.FileUrlStart + validFileNames[0], _processOptions);
          }
        }
        else
        {
          bool success;
          string? errors;
          if (_processOptions.ImportMultipleStreamsVertically)
            success = AsciiImporter.TryImportFromMultipleAsciiFilesVertically(destinationTable, validFileNames, false, _processOptions, out errors);
          else
            success = AsciiImporter.TryImportFromMultipleAsciiFilesHorizontally(destinationTable, validFileNames, false, _processOptions, out errors);
        }
      }

      var invalidFileNames = _files.Where(x => string.IsNullOrEmpty(x.GetResolvedFileNameOrNull())).ToArray();
      if (invalidFileNames.Length > 0)
      {
        var stb = new StringBuilder();
        stb.AppendLine("The following file names could not be resolved:");
        foreach (var fn in invalidFileNames)
        {
          stb.AppendLine(fn.AbsoluteFileName);
        }
        stb.AppendLine("(End of file names that could not be resolved)");

        throw new ApplicationException(stb.ToString());
      }
    }

    public override (IReadOnlyList<string> FileExtensions, string Explanation) GetFileExtensions()
    {
      return new AsciiImporterImpl().GetFileExtensions();
    }
  }
}
