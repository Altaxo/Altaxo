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
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Altaxo.Data;

namespace Altaxo.Serialization.HDF5.Nexus
{
  public class NexusImportDataSource : Altaxo.Serialization.FileImportTableDataSourceBase<NexusImportOptions>
  {
    private NexusImportOptions _processOptions;

    #region Serialization

    #region Version 0

    /// <summary>
    /// 2024-10-08 initial version.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(NexusImportDataSource), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (NexusImportDataSource)obj;

        info.AddValue("ImportOptions", s._importOptions);
        info.AddValue("ProcessOptions", s._processOptions);
        info.AddArray("ProcessData", s._files.ToArray(), s._files.Count);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        if (o is NexusImportDataSource s)
          s.DeserializeSurrogate0(info);
        else
          s = new NexusImportDataSource(info, 0);
        return s;
      }
    }

    [MemberNotNull(nameof(_importOptions), nameof(_processOptions))]
    private void DeserializeSurrogate0(Altaxo.Serialization.Xml.IXmlDeserializationInfo info)
    {
      _isDeserializationInProgress = true;
      _importOptions = (IDataSourceImportOptions)info.GetValue("ImportOptions", this);
      _processOptions = (NexusImportOptions)info.GetValue("ProcessOptions", this);
      var count = info.OpenArray("ProcessData");
      for (int i = 0; i < count; ++i)
        _files.Add((AbsoluteAndRelativeFileName)info.GetValue("e", this));
      info.CloseArray(count);

      info.AfterDeserializationHasCompletelyFinished += EhAfterDeserializationHasCompletelyFinished;
    }

    #endregion Version 0

    /// <summary>
    /// Deserialization constructor
    /// </summary>
    protected NexusImportDataSource(Altaxo.Serialization.Xml.IXmlDeserializationInfo info, int version)
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

    public NexusImportDataSource(string fileName, NexusImportOptions options)
      : this(new string[] { fileName }, options)
    {
    }

    public NexusImportDataSource(IEnumerable<string> fileNames, NexusImportOptions options)
      : base(fileNames)
    {
      _processOptions = options;
    }

    public NexusImportDataSource(NexusImportDataSource from)
    {
      CopyFrom(from);
    }

    public override object Clone() => new NexusImportDataSource(this);

    #endregion Construction

    protected override void ImportFromFiles(string[] validFileNames, DataTable destinationTable, IProgressReporter reporter)
    {
      new NexusImporter().Import(validFileNames, destinationTable, _processOptions, attachDataSource: false);
    }

    #region Properties

    public NexusImportOptions ProcessOptions
    {
      get
      {
        return _processOptions;
      }
      set
      {
        _processOptions = value;
      }
    }

    #endregion Properties

    public override (IReadOnlyList<string> FileExtensions, string Explanation) GetFileExtensions()
      => new NexusImporter().GetFileExtensions();
  }
}
