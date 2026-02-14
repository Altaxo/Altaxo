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
using System.Linq;
using System.Text;

namespace Altaxo.Serialization.Ascii
{
  /// <summary>
  /// Designates what to do with the main header lines of an ASCII file.
  /// </summary>
  public enum AsciiHeaderLinesDestination
  {
    /// <summary>Ignore the main header lines (throw them away).</summary>
    Ignore,

    /// <summary>Try to import the items in the header lines as property columns.</summary>
    ImportToProperties,

    /// <summary>Try to import the items in the header line(s) as properties. If the number of items doesn't match with that of the table, those header line is imported into the notes of the worksheet.</summary>
    ImportToPropertiesOrNotes,

    /// <summary>Store the main header lines as notes in the worksheet.</summary>
    ImportToNotes,

    /// <summary>Try to import the items in the header lines as property columns. Additionally, those lines are added to the notes of the table.</summary>
    ImportToPropertiesAndNotes
  }

  /// <summary>
  /// Denotes options about how to import data from an ascii text file.
  /// </summary>
  public record AsciiImportOptions : Main.IImmutable
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="AsciiImportOptions"/> class with default properties.
    /// </summary>
    public AsciiImportOptions()
    {
      HeaderLinesDestination = AsciiHeaderLinesDestination.ImportToProperties;
    }

    /// <summary>
    /// If true, the encoding is detected from the byte order marks (BOM). If no BOM is present, the encoding according to the <see cref="CodePage"/> property is used,
    /// or, if also not available, the standard encoding.
    /// </summary>
    public bool DetectEncodingFromByteOrderMarks { get; init; } = true;

    /// <summary>
    /// Gets or sets the code page that is used to recognize the Ascii data. To use the system default code page, set this property to 0.
    /// </summary>
    public int CodePage { get; init; }

    /// <summary>
    /// Gets the encoding. You can set the Encoding setting the CodePage (see <see cref="CodePage"/>).
    /// </summary>
    public Encoding Encoding
    {
      get
      {
        var result = System.Text.Encoding.Default;
        if (CodePage != 0)
        {
          var cp = System.Text.Encoding.GetEncodings().Where(ei => ei.CodePage == CodePage).FirstOrDefault();
          if (cp is not null)
          {
            result = cp.GetEncoding();
          }
        }
        return result;
      }
    }



    #region Serialization

    #region Version 0

    /// <summary>
    /// 2014-08-03 initial version.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Serialization.Ascii.AsciiImportOptions", 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (AsciiImportOptions)obj;

        info.AddValue("RenameWorksheet", s.RenameWorksheet);
        info.AddValue("RenameColumns", s.RenameColumns);
        info.AddValue("IndexOfCaptionLine", s.IndexOfCaptionLine);
        info.AddValue("NumberOfMainHeaderLines", s.NumberOfMainHeaderLines);
        info.AddEnum("HeaderLinesDestination", s.HeaderLinesDestination);
        info.AddValueOrNull("SeparationStrategy", s.SeparationStrategy);
        info.AddValue("NumberFormatCultureLCID", s.NumberFormatCulture?.LCID ?? -1);
        info.AddValue("DateTimeFormatCultureLCID", s.DateTimeFormatCulture?.LCID ?? -1);
        info.AddValueOrNull("RecognizedStructure", s.RecognizedStructure);
        info.AddValue("ImportMultipleStreamsVertically", s.ImportMultipleStreamsVertically);
      }

      protected virtual AsciiImportOptions SDeserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var renameWorksheet = info.GetBoolean("RenameWorksheet");
        var renameColumns = info.GetBoolean("RenameColumns");
        var indexOfCaptionLine = info.GetNullableInt32("IndexOfCaptionLine");
        var numberOfMainHeaderLines = info.GetNullableInt32("NumberOfMainHeaderLines");
        var headerLinesDestination = (AsciiHeaderLinesDestination)info.GetEnum("HeaderLinesDestination", typeof(AsciiHeaderLinesDestination));
        var separationStrategy = (IAsciiSeparationStrategy?)info.GetValueOrNull("SeparationStrategy", null);
        var numberLCID = info.GetInt32("NumberFormatCultureLCID");
        var numberFormatCulture = -1 == numberLCID ? null : System.Globalization.CultureInfo.GetCultureInfo(numberLCID);
        var dateLCID = info.GetInt32("DateTimeFormatCultureLCID");
        var dateTimeFormatCulture = -1 == dateLCID ? null : System.Globalization.CultureInfo.GetCultureInfo(dateLCID);
        var recognizedStructure = (AsciiLineComposition?)info.GetValueOrNull("AsciiLineStructure", null);
        var importMultipleStreamsVertically = info.GetBoolean("ImportMultipleStreamsVertically");

        return (o as AsciiImportOptions ?? new AsciiImportOptions()) with
        {
          RenameWorksheet = renameWorksheet,
          RenameColumns = renameColumns,
          IndexOfCaptionLine = indexOfCaptionLine,
          NumberOfMainHeaderLines = numberOfMainHeaderLines,
          HeaderLinesDestination = headerLinesDestination,
          SeparationStrategy = separationStrategy,
          NumberFormatCulture = numberFormatCulture,
          DateTimeFormatCulture = dateTimeFormatCulture,
          RecognizedStructure = recognizedStructure,
          ImportMultipleStreamsVertically = importMultipleStreamsVertically
        };
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = SDeserialize(o, info, parent);
        return s;
      }
    }

    #endregion Version 0

    #region Version 1

    /// <summary>
    /// 2025-09-25: add CodePage property and DetectEncodingFromByteOrderMarks
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(AsciiImportOptions), 1)]
    private class XmlSerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (AsciiImportOptions)obj;

        info.AddValue("RenameWorksheet", s.RenameWorksheet);
        info.AddValue("RenameColumns", s.RenameColumns);
        info.AddValue("IndexOfCaptionLine", s.IndexOfCaptionLine);
        info.AddValue("NumberOfMainHeaderLines", s.NumberOfMainHeaderLines);
        info.AddEnum("HeaderLinesDestination", s.HeaderLinesDestination);
        info.AddValueOrNull("SeparationStrategy", s.SeparationStrategy);
        info.AddValue("NumberFormatCultureLCID", s.NumberFormatCulture?.LCID ?? -1);
        info.AddValue("DateTimeFormatCultureLCID", s.DateTimeFormatCulture?.LCID ?? -1);
        info.AddValueOrNull("RecognizedStructure", s.RecognizedStructure);
        info.AddValue("ImportMultipleStreamsVertically", s.ImportMultipleStreamsVertically);
        info.AddValue("DetectEncodingFromByteOrderMarks", s.DetectEncodingFromByteOrderMarks);
        info.AddValue("CodePage", s.CodePage);
      }

      protected virtual AsciiImportOptions SDeserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (o is null ? new AsciiImportOptions() : (AsciiImportOptions)o);

        var renameWorksheet = info.GetBoolean("RenameWorksheet");
        var renameColumns = info.GetBoolean("RenameColumns");
        var indexOfCaptionLine = info.GetNullableInt32("IndexOfCaptionLine");
        var numberOfMainHeaderLines = info.GetNullableInt32("NumberOfMainHeaderLines");
        var headerLinesDestination = (AsciiHeaderLinesDestination)info.GetEnum("HeaderLinesDestination", typeof(AsciiHeaderLinesDestination));
        var separationStrategy = (IAsciiSeparationStrategy?)info.GetValueOrNull("SeparationStrategy", s);
        var numberLCID = info.GetInt32("NumberFormatCultureLCID");
        var numberFormatCulture = -1 == numberLCID ? null : System.Globalization.CultureInfo.GetCultureInfo(numberLCID);
        var dateLCID = info.GetInt32("DateTimeFormatCultureLCID");
        var dateTimeFormatCulture = -1 == dateLCID ? null : System.Globalization.CultureInfo.GetCultureInfo(dateLCID);
        var recognizedStructure = (AsciiLineComposition?)info.GetValueOrNull("AsciiLineStructure", s);
        var importMultipleStreamsVertically = info.GetBoolean("ImportMultipleStreamsVertically");
        var detectEncodingFromByteOrderMarks = info.GetBoolean("DetectEncodingFromByteOrderMarks");
        var codePage = info.GetInt32("CodePage");

        return (o as AsciiImportOptions ?? new AsciiImportOptions()) with
        {
          RenameWorksheet = renameWorksheet,
          RenameColumns = renameColumns,
          IndexOfCaptionLine = indexOfCaptionLine,
          NumberOfMainHeaderLines = numberOfMainHeaderLines,
          HeaderLinesDestination = headerLinesDestination,
          SeparationStrategy = separationStrategy,
          NumberFormatCulture = numberFormatCulture,
          DateTimeFormatCulture = dateTimeFormatCulture,
          RecognizedStructure = recognizedStructure,
          ImportMultipleStreamsVertically = importMultipleStreamsVertically,
          DetectEncodingFromByteOrderMarks = detectEncodingFromByteOrderMarks,
          CodePage = codePage
        };

        return s;
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = SDeserialize(o, info, parent);
        return s;
      }
    }

    #endregion Version 1


    #endregion Serialization

    #region Properties

    /// <summary>
    /// Gets or sets a value indicating whether the import of multiple streams in one table should be horizontally oriented (in more columns) or vertically oriented (in more rows).
    /// </summary>
    /// <value>
    /// <c>true</c> if multiple streams should be imported vertically oriented, otherwise <c>false</c>.
    /// </value>
    public bool ImportMultipleStreamsVertically { get; init; }

    /// <summary>If true, rename the columns if 1st line contain  the column names. This option must be set programmatically or by user interaction.</summary>
    public bool RenameColumns { get; init; }

    /// <summary>If true, rename the worksheet to the data file name.  This option must be set programmatically or by user interaction.</summary>
    public bool RenameWorksheet { get; init; }

    /// <summary>Designates the destination of main header lines. This option must be set programmatically or by user interaction.</summary>
    public AsciiHeaderLinesDestination HeaderLinesDestination { get; init; }

    /// <summary>Number of lines to skip (the main header).</summary>
    public int? NumberOfMainHeaderLines { get; init; }

    /// <summary>Index of the line, where we can extract the column names from.</summary>
    public int? IndexOfCaptionLine { get; init; }

    /// <summary>Method to separate the tokens in each line of ascii text.</summary>
    public IAsciiSeparationStrategy? SeparationStrategy { get; init; }

    /// <summary>Gets or sets the culture that formats numbers.</summary>
    public System.Globalization.CultureInfo? NumberFormatCulture { get; init; }

    /// <summary>Gets or sets the culture that formats date/time values.</summary>
    public System.Globalization.CultureInfo? DateTimeFormatCulture { get; init; }

    /// <summary>Structur of the main part of the file (which data type is placed in which column).</summary>
    public AsciiLineComposition? RecognizedStructure { get; init; }

    #endregion Properties

    /// <summary>
    /// Gets a value indicating whether everything is fully specified now, so that the instance can be used to import Ascii data.
    /// If this value is false, the Ascii data have to be analyzed in order to find the missing values.
    /// </summary>
    public bool IsFullySpecified
    {
      get
      {
        return
          NumberOfMainHeaderLines is not null &&
          IndexOfCaptionLine is not null &&
          SeparationStrategy is not null &&
          RecognizedStructure is not null &&
          NumberFormatCulture is not null &&
          DateTimeFormatCulture is not null;
      }
    }


  }
}
