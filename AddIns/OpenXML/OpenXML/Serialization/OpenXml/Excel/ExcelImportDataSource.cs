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
using System.Linq;
using System.Text;
using Altaxo.Data;

namespace Altaxo.Serialization.OpenXml.Excel
{
  public class ExcelImportDataSource : Altaxo.Serialization.FileImportTableDataSourceBase<ExcelImportOptions>
  {
    // `_processOptions` is provided by the base class

    #region Serialization

    #region Version 0

    /// <summary>
    /// 2024-09-14 initial version.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ExcelImportDataSource), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (ExcelImportDataSource)obj;

        info.AddValue("ImportOptions", s._importOptions);
        info.AddValue("ProcessOptions", s._processOptions);
        info.AddArray("ProcessData", s._files.ToArray(), s._files.Count);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        if (o is ExcelImportDataSource s)
          s.DeserializeSurrogate0(info);
        else
          s = new ExcelImportDataSource(info, 0);
        return s;
      }
    }

#if NET6_0_OR_GREATER
    [MemberNotNull(nameof(_importOptions), nameof(_processOptions))]
#endif
    private void DeserializeSurrogate0(Altaxo.Serialization.Xml.IXmlDeserializationInfo info)
    {
      _isDeserializationInProgress = true;
      _importOptions = (IDataSourceImportOptions)info.GetValue("ImportOptions", this);
      _processOptions = (ExcelImportOptions)info.GetValue("ProcessOptions", this);
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
    protected ExcelImportDataSource(Altaxo.Serialization.Xml.IXmlDeserializationInfo info, int version)
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

#if NET6_0_OR_GREATER
    [MemberNotNull(nameof(_files), nameof(_importOptions), nameof(_processOptions))]
#endif
    private void CopyFrom(ExcelImportDataSource from)
    {
      using (var token = SuspendGetToken())
      {
        _files = new List<AbsoluteAndRelativeFileName>(CopyHelper.GetEnumerationMembersNotNullCloned(from._files));
        _importOptions = from._importOptions;
        _processOptions = from._processOptions;
        EhSelfChanged(EventArgs.Empty);
        token.Resume();
      }
    }

    public bool CopyFrom(object obj)
    {
      if (ReferenceEquals(this, obj))
        return true;

      var from = obj as ExcelImportDataSource;
      if (from is not null)
      {
        CopyFrom(from);
        return true;
      }
      return false;
    }




    public ExcelImportDataSource(string fileName, ExcelImportOptions options)
      : this(new string[] { fileName }, options)
    {
    }

    public ExcelImportDataSource(IEnumerable<string> fileNames, ExcelImportOptions options)
      : base(fileNames)
    {
      _processOptions = options;
    }

    public ExcelImportDataSource(ExcelImportDataSource from)
    {
      CopyFrom(from);
    }

    public override object Clone() => new ExcelImportDataSource(this);

    #endregion Construction

    protected override void ImportFromFiles(string[] validFileNames, DataTable destinationTable, IProgressReporter reporter)
    {
      new ExcelImporter().Import(validFileNames, destinationTable, _processOptions, attachDataSource: false);
    }

    #region Properties

    #endregion Properties

    public override (IReadOnlyList<string> FileExtensions, string Explanation) GetFileExtensions()
      => new ExcelImporter().GetFileExtensions();
  }
}
