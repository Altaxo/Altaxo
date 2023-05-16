#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2023 Dr. Dirk Lellinger
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
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Altaxo.Data;
using Altaxo.Main.Properties;

namespace Altaxo.Serialization.Ascii
{
  /// <summary>
  /// Options for export of ASCII files
  /// </summary>
  public record AsciiExportOptions
  {
    public static readonly PropertyKey<AsciiExportOptions> PropertyKeyAsciiExportOptions = new(
      "12CFB92C-8D90-4A34-A481-7C30B15654AB",
      "Table\\AsciiExportOptions",
      PropertyLevel.All,
      typeof(DataTable),
      () => null // we use null here, to be able to detect if the options will be set for the first time
      );


    (char Separator, char Substitute) _separatorAndSubstituteChar;

    /// <summary>
    /// The separator char.
    /// </summary>
    public char SeparatorChar => _separatorAndSubstituteChar.Separator;

    /// <summary>
    /// Substitute for separator char. Should the separator char be present in header or items, it is replaced by this char.
    /// </summary>
    public char SubstituteForSeparatorChar => _separatorAndSubstituteChar.Substitute;

    /// <summary>
    /// Gets the separator and substitute character.
    /// </summary>
    /// <value>
    /// The separator and substitute character.
    /// </value>
    /// <exception cref="Markdig.Helpers.ThrowHelper.InvalidOperationException(System.String)">Separator char and substitute char must be different</exception>
    public (char Separator, char Substitute) SeparatorAndSubstituteChar
    {
      get => _separatorAndSubstituteChar;
      init
      {
        if (value.Substitute == value.Separator)
          throw new InvalidOperationException("Separator char and substitute char must be different");
        _separatorAndSubstituteChar = value;
      }
    }

    /// <summary>
    /// If true, the first line of the exported Ascii file will contain the data column names, separated by the <see cref="SeparatorChar" />.
    /// </summary>
    public bool ExportDataColumnNames { get; init; }

    /// <summary>
    /// If true, the property columns will be exported.
    /// </summary>
    public bool ExportPropertyColumns { get; init; }

    /// <summary>
    /// If true, the property items will be exported with name. In order to do that, each property item will be headed by
    /// "PropColName=". SeparatorChar and Newlines will be removed both from the items text and from the PropertyColumnNames.
    /// </summary>
    public bool ExportPropertiesWithName { get; init; }

    /// <summary>
    /// If this string is not empty, it is a C# format string to convert date/times.
    /// </summary>
    public string DateTimeFormat { get; init; }


    private CultureInfo _culture;

    public CultureInfo Culture
    {
      get
      {
        return _culture;
      }
      [MemberNotNull(nameof(_culture))]
      init
      {
        if (value is null)
          throw new ArgumentNullException(nameof(Culture));
        _culture = value;
      }
    }

    /// <summary>
    /// Creates default options: Separator char is Tab, Substitute char is Space, FormatProvider is the CurrentCulture.
    /// </summary>
    public AsciiExportOptions()
    {
      _separatorAndSubstituteChar = ('\t', ' ');
      ExportDataColumnNames = true;
      ExportPropertyColumns = true;
      DateTimeFormat = string.Empty;
      Culture = Altaxo.Settings.GuiCulture.Instance;
    }



    /// <summary>
    /// Sets the separator char and chooses the substitute char automatically.
    /// </summary>
    /// <param name="separatorChar">The separator char.</param>
    public AsciiExportOptions WithSeparator(char separatorChar)
    {
      return this with
      {
        SeparatorAndSubstituteChar = (separatorChar, separatorChar == '\t' ? ' ' : '_'),
      };
    }

    #region Serialization

    #region Version 0

    /// <summary>
    /// 2023-05-15 initial version.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(AsciiExportOptions), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (AsciiExportOptions)obj;

        info.AddValue("CultureLCID", s.Culture?.LCID ?? -1);
        info.AddValue("SeparatorChar", s.SeparatorChar);
        info.AddValue("SubstituteForSeparatorChar", s.SubstituteForSeparatorChar);
        info.AddValue("ExportDataColumnNames", s.ExportDataColumnNames);
        info.AddValue("ExportPropertyColumns", s.ExportPropertyColumns);
        info.AddValue("ExportPropertiesWithName", s.ExportPropertiesWithName);
        info.AddValue("DateTimeFormat", s.DateTimeFormat);
      }

      protected virtual AsciiExportOptions SDeserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (o is null ? new AsciiExportOptions() : (AsciiExportOptions)o);

        var cultureLCID = info.GetInt32("CultureLCID");
        var separatorChar = info.GetChar("SeparatorChar");
        var substituteForSeparatorChar = info.GetChar("SubstituteForSeparatorChar");
        var exportDataColumnNames = info.GetBoolean("ExportDataColumnNames");
        var exportPropertyColumns = info.GetBoolean("ExportPropertyColumns");
        var exportPropertiesWithName = info.GetBoolean("ExportPropertiesWithName");
        var dateTimeFormat = info.GetString("DateTimeFormat");

        return s with
        {
          Culture = CultureInfo.GetCultureInfo(cultureLCID),
          SeparatorAndSubstituteChar = (separatorChar, substituteForSeparatorChar),
          ExportDataColumnNames = exportDataColumnNames,
          ExportPropertyColumns = exportPropertyColumns,
          ExportPropertiesWithName = exportPropertiesWithName,
          DateTimeFormat = dateTimeFormat,
        };
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = SDeserialize(o, info, parent);
        return s;
      }
    }

    #endregion Version 0

    #endregion Serialization
  }
}
