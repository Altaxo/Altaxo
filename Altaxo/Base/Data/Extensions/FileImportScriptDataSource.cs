#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2019 Dr. Dirk Lellinger
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
using Altaxo.Scripting;
using Altaxo.Serialization;

namespace Altaxo.Data
{
  public class FileImportScriptDataSource : FileImportTableDataSourceBase
  {
    private FileImportScript _importScript;

    #region Serialization

    #region Version 0

    /// <summary>
    /// 2014-07-28 initial version.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(FileImportScriptDataSource), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (FileImportScriptDataSource)obj;

        info.AddValue("ImportOptions", s._importOptions);
        info.AddValue("ImportScript", s._importScript);
        info.AddArray("Files", s._files.ToArray(), s._files.Count);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        if (o is FileImportScriptDataSource s)
          s.DeserializeSurrogate0(info);
        else
          s = new FileImportScriptDataSource(info, 0);
        return s;
      }
    }

    [MemberNotNull(nameof(_importOptions), nameof(_importScript))]
    void DeserializeSurrogate0(Altaxo.Serialization.Xml.IXmlDeserializationInfo info)
    {
      _isDeserializationInProgress = true;
      _importOptions = info.GetValue<DataSourceImportOptions>("ImportOptions", this);
      ChildSetMember(ref _importScript, (FileImportScript)info.GetValue("ImportScript", this));
      var count = info.OpenArray("Files");
      for (int i = 0; i < count; ++i)
        _files.Add((AbsoluteAndRelativeFileName)info.GetValue("e", this));
      info.CloseArray(count);

      info.AfterDeserializationHasCompletelyFinished += EhAfterDeserializationHasCompletelyFinished;
    }

    #endregion Version 0

    protected FileImportScriptDataSource(Altaxo.Serialization.Xml.IXmlDeserializationInfo info, int version)
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

    [MemberNotNull(nameof(_files), nameof(_importOptions), nameof(_importScript))]
    void CopyFrom(FileImportScriptDataSource from)
    {
      using (var token = SuspendGetToken())
      {
        _files = new List<AbsoluteAndRelativeFileName>(CopyHelper.GetEnumerationMembersNotNullCloned(from._files));
        _importOptions = from._importOptions;
        ChildCopyToMember(ref _importScript, from._importScript);


        EhSelfChanged(EventArgs.Empty);
        token.Resume();
      }
    }

    public override bool CopyFrom(object obj)
    {
      if (ReferenceEquals(this, obj))
        return true;

      if (obj is FileImportScriptDataSource from)
      {
        CopyFrom(from);
        return true;
      }
      return false;
    }

    public FileImportScriptDataSource(string fileName, FileImportScript script)
      : this(new string[] { fileName }, script)
    {
    }

    public FileImportScriptDataSource(IEnumerable<string> fileNames, FileImportScript script)
      : base(fileNames)
    {
      ChildCopyToMember(ref _importScript, script);
    }

    public FileImportScriptDataSource(FileImportScriptDataSource from)
      : base(from)
    {
      ChildCopyToMember(ref _importScript, from._importScript);
    }

    public override object Clone() => new FileImportScriptDataSource(this);
    protected override IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
    {
      if (_importScript is not null)
        yield return new Main.DocumentNodeAndName(_importScript, () => _importScript = null!, "ImportScript");
    }


    #endregion Construction

    protected override void ImportFromFiles(string[] validFileNames, DataTable destinationTable, IProgressReporter reporter)
    {
      _importScript.ExecuteWithoutExceptionCatching(destinationTable, validFileNames, reporter);
    }

    #region Properties

    public string SourceFileName
    {
      get
      {
        if (_files.Count != 1)
          throw new InvalidOperationException("In order to get the source file name, the number of files has to be one");

        return _files[0].GetResolvedFileNameOrNull() ?? _files[0].AbsoluteFileName;
      }
      set
      {
        string? oldName = null;
        if (_files.Count == 1)
          oldName = SourceFileName;

        _files.Clear();
        _files.Add(new AbsoluteAndRelativeFileName(value));

        if (oldName != SourceFileName)
        {
          UpdateWatching();
        }
      }
    }

    public FileImportScript ImportScript
    {
      get
      {
        return (FileImportScript)_importScript.Clone();
      }
      set
      {
        ChildCloneToMember(ref _importScript, value);
      }
    }

    #endregion Properties

    public override void VisitDocumentReferences(Main.DocNodeProxyReporter ReportProxies)
    {
    }

    public override object ProcessOptionsObject
    {
      get => _importScript;
      set => _importScript = (FileImportScript)value;
    }

    public override (IReadOnlyList<string> FileExtensions, string Explanation) GetFileExtensions()
      => (Array.Empty<string>(), "");
  }
}
