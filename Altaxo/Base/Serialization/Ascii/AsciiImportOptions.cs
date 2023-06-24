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
  public class AsciiImportOptions
    :
    Main.SuspendableDocumentLeafNodeWithEventArgs,
    Main.ICopyFrom
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="AsciiImportOptions"/> class with default properties.
    /// </summary>
    public AsciiImportOptions()
    {
      HeaderLinesDestination = AsciiHeaderLinesDestination.ImportToProperties;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the import of multiple streams in one table should be horizontally oriented (in more columns) or vertically oriented (in more rows).
    /// </summary>
    /// <value>
    /// <c>true</c> if multiple streams should be imported vertically oriented, otherwise <c>false</c>.
    /// </value>
    protected bool _importMultipleStreamsVertically;

    /// <summary>If true, rename the columns if 1st line contain  the column names. This option must be set programmatically or by user interaction.</summary>
    protected bool _renameColumns;

    /// <summary>If true, rename the worksheet to the data file name.  This option must be set programmatically or by user interaction.</summary>
    protected bool _renameWorksheet;

    /// <summary>Designates the destination of main header lines. This option must be set programmatically or by user interaction.</summary>
    protected AsciiHeaderLinesDestination _headerLinesDestination;

    /// <summary>Number of lines to skip (the main header).</summary>
    protected int? _numberOfMainHeaderLines;

    /// <summary>Index of the line, where we can extract the column names from.</summary>
    protected int? _indexOfCaptionLine;

    /// <summary>Method to separate the tokens in each line of ascii text.</summary>
    protected IAsciiSeparationStrategy? _separationStrategy;

    /// <summary>Gets or sets the culture that formats numbers.</summary>
    protected System.Globalization.CultureInfo? _numberFormatCulture;

    /// <summary>Gets or sets the culture that formats date/time values.</summary>
    protected System.Globalization.CultureInfo? _dateTimeFormatCulture;

    /// <summary>Structur of the main part of the file (which data type is placed in which column).</summary>
    protected AsciiLineComposition? _recognizedStructure;

    #region Serialization

    #region Version 0

    /// <summary>
    /// 2014-08-03 initial version.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(AsciiImportOptions), 0)]
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
        var s = (o is null ? new AsciiImportOptions() : (AsciiImportOptions)o);

        s.RenameWorksheet = info.GetBoolean("RenameWorksheet");
        s.RenameColumns = info.GetBoolean("RenameColumns");
        s.IndexOfCaptionLine = info.GetNullableInt32("IndexOfCaptionLine");
        s.NumberOfMainHeaderLines = info.GetNullableInt32("NumberOfMainHeaderLines");
        s.HeaderLinesDestination = (AsciiHeaderLinesDestination)info.GetEnum("HeaderLinesDestination", typeof(AsciiHeaderLinesDestination));
        s.SeparationStrategy = (IAsciiSeparationStrategy?)info.GetValueOrNull("SeparationStrategy", s);
        var numberLCID = info.GetInt32("NumberFormatCultureLCID");
        s.NumberFormatCulture = -1 == numberLCID ? null : System.Globalization.CultureInfo.GetCultureInfo(numberLCID);
        var dateLCID = info.GetInt32("DateTimeFormatCultureLCID");
        s.DateTimeFormatCulture = -1 == dateLCID ? null : System.Globalization.CultureInfo.GetCultureInfo(dateLCID);
        s.RecognizedStructure = (AsciiLineComposition?)info.GetValueOrNull("AsciiLineStructure", s);
        s.ImportMultipleStreamsVertically = info.GetBoolean("ImportMultipleStreamsVertically");
        return s;
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = SDeserialize(o, info, parent);
        return s;
      }
    }

    #endregion Version 0

    #endregion Serialization

    #region Properties

    /// <summary>
    /// Gets or sets a value indicating whether the import of multiple streams in one table should be horizontally oriented (in more columns) or vertically oriented (in more rows).
    /// </summary>
    /// <value>
    /// <c>true</c> if multiple streams should be imported vertically oriented, otherwise <c>false</c>.
    /// </value>
    public bool ImportMultipleStreamsVertically { get { return _importMultipleStreamsVertically; } set { SetMemberAndRaiseSelfChanged(ref _importMultipleStreamsVertically, value); } }

    /// <summary>If true, rename the columns if 1st line contain  the column names. This option must be set programmatically or by user interaction.</summary>
    public bool RenameColumns { get { return _renameColumns; } set { SetMemberAndRaiseSelfChanged(ref _renameColumns, value); } }

    /// <summary>If true, rename the worksheet to the data file name.  This option must be set programmatically or by user interaction.</summary>
    public bool RenameWorksheet { get { return _renameWorksheet; } set { SetMemberAndRaiseSelfChanged(ref _renameWorksheet, value); } }

    /// <summary>Designates the destination of main header lines. This option must be set programmatically or by user interaction.</summary>
    public AsciiHeaderLinesDestination HeaderLinesDestination { get { return _headerLinesDestination; } set { SetMemberEnumAndRaiseSelfChanged(ref _headerLinesDestination, value); } }

    /// <summary>Number of lines to skip (the main header).</summary>
    public int? NumberOfMainHeaderLines { get { return _numberOfMainHeaderLines; } set { SetMemberAndRaiseSelfChanged(ref _numberOfMainHeaderLines, value); } }

    /// <summary>Index of the line, where we can extract the column names from.</summary>
    public int? IndexOfCaptionLine { get { return _indexOfCaptionLine; } set { SetMemberAndRaiseSelfChanged(ref _indexOfCaptionLine, value); } }

    /// <summary>Method to separate the tokens in each line of ascii text.</summary>
    public IAsciiSeparationStrategy? SeparationStrategy { get { return _separationStrategy; } set { if (!object.ReferenceEquals(_separationStrategy, value)) { _separationStrategy = value; EhSelfChanged(); } } }

    /// <summary>Gets or sets the culture that formats numbers.</summary>
    public System.Globalization.CultureInfo? NumberFormatCulture
    {
      get
      {
        return _numberFormatCulture;
      }
      set { if (!object.ReferenceEquals(_numberFormatCulture, value)) { _numberFormatCulture = value; EhSelfChanged(); } }
    }

    /// <summary>Gets or sets the culture that formats date/time values.</summary>
    public System.Globalization.CultureInfo? DateTimeFormatCulture { get { return _dateTimeFormatCulture; } set { if (!object.ReferenceEquals(_dateTimeFormatCulture, value)) { _dateTimeFormatCulture = value; EhSelfChanged(); } } }

    /// <summary>Structur of the main part of the file (which data type is placed in which column).</summary>
    public AsciiLineComposition? RecognizedStructure { get { return _recognizedStructure; } set { if (!object.ReferenceEquals(_recognizedStructure, value)) { _recognizedStructure = value; EhSelfChanged(); } } }

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
          _numberOfMainHeaderLines is not null &&
          _indexOfCaptionLine is not null &&
          _separationStrategy is not null &&
          _recognizedStructure is not null &&
          _numberFormatCulture is not null &&
          _dateTimeFormatCulture is not null;
      }
    }

    public bool CopyFrom(object obj)
    {
      if (ReferenceEquals(this, obj))
        return true;

      var from = obj as AsciiImportOptions;
      if (from is not null)
      {
        using (var suspendToken = SuspendGetToken())
        {
          RenameColumns = from.RenameColumns;
          RenameWorksheet = from.RenameWorksheet;
          HeaderLinesDestination = from.HeaderLinesDestination;

          NumberOfMainHeaderLines = from.NumberOfMainHeaderLines;

          IndexOfCaptionLine = from.IndexOfCaptionLine;

          SeparationStrategy = from.SeparationStrategy;

          NumberFormatCulture = from.NumberFormatCulture is null ? null : (System.Globalization.CultureInfo)from.NumberFormatCulture.Clone();

          DateTimeFormatCulture = from.DateTimeFormatCulture is null ? null : (System.Globalization.CultureInfo)from.DateTimeFormatCulture.Clone();

          RecognizedStructure = from.RecognizedStructure is null ? null : from.RecognizedStructure;

          ImportMultipleStreamsVertically = from.ImportMultipleStreamsVertically;

          suspendToken.Resume();
        }

        return true;
      }
      return false;
    }

    object ICloneable.Clone()
    {
      var result = new AsciiImportOptions();
      result.CopyFrom(this);
      return result;
    }

    public AsciiImportOptions Clone()
    {
      var result = new AsciiImportOptions();
      result.CopyFrom(this);
      return result;
    }
  }
}
