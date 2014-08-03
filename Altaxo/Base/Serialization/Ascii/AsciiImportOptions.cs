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
	public class AsciiImportOptions : Main.ICopyFrom
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="AsciiImportOptions"/> class with default properties.
		/// </summary>
		public AsciiImportOptions()
		{
			HeaderLinesDestination = AsciiHeaderLinesDestination.ImportToProperties;
		}

		/// <summary>If true, rename the columns if 1st line contain  the column names. This option must be set programmatically or by user interaction.</summary>
		public bool RenameColumns { get; set; }

		/// <summary>If true, rename the worksheet to the data file name.  This option must be set programmatically or by user interaction.</summary>
		public bool RenameWorksheet { get; set; }

		/// <summary>Designates the destination of main header lines. This option must be set programmatically or by user interaction.</summary>
		public AsciiHeaderLinesDestination HeaderLinesDestination { get; set; }

		/// <summary>Number of lines to skip (the main header).</summary>
		public int? NumberOfMainHeaderLines { get; set; }

		/// <summary>Index of the line, where we can extract the column names from.</summary>
		public int? IndexOfCaptionLine { get; set; }

		/// <summary>Method to separate the tokens in each line of ascii text.</summary>
		public IAsciiSeparationStrategy SeparationStrategy { get; set; }

		/// <summary>Gets or sets the culture that formats numbers.</summary>
		public System.Globalization.CultureInfo NumberFormatCulture { get; set; }

		/// <summary>Gets or sets the culture that formats date/time values.</summary>
		public System.Globalization.CultureInfo DateTimeFormatCulture { get; set; }

		/// <summary>Structur of the main part of the file (which data type is placed in which column).</summary>
		public AsciiLineStructure RecognizedStructure { get; set; }

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
				info.AddValue("SeparationStrategy", s.SeparationStrategy);
				info.AddValue("NumberFormatCultureLCID", s.NumberFormatCulture.LCID);
				info.AddValue("DateTimeFormatCultureLCID", s.DateTimeFormatCulture.LCID);
				info.AddValue("RecognizedStructure", s.RecognizedStructure);
			}

			protected virtual AsciiImportOptions SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				var s = (o == null ? new AsciiImportOptions() : (AsciiImportOptions)o);

				s.RenameWorksheet = info.GetBoolean("RenameWorksheet");
				s.RenameColumns = info.GetBoolean("RenameColumns");
				s.IndexOfCaptionLine = info.GetNullableInt32("IndexOfCaptionLine");
				s.NumberOfMainHeaderLines = info.GetNullableInt32("NumberOfMainHeaderLines");
				s.HeaderLinesDestination = (AsciiHeaderLinesDestination)info.GetEnum("HeaderLinesDestination", typeof(AsciiHeaderLinesDestination));
				s.SeparationStrategy = (IAsciiSeparationStrategy)info.GetValue("SeparationStrategy", s);
				s.NumberFormatCulture = System.Globalization.CultureInfo.GetCultureInfo(info.GetInt32("NumberFormatCultureLCID"));
				s.DateTimeFormatCulture = System.Globalization.CultureInfo.GetCultureInfo(info.GetInt32("DateTimeFormatCultureLCID"));
				s.RecognizedStructure = (AsciiLineStructure)info.GetValue("AsciiLineStructure", s);

				return s;
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				var s = SDeserialize(o, info, parent);
				return s;
			}
		}

		#endregion Version 0

		#endregion Serialization

		/// <summary>
		/// Gets a value indicating whether everything is fully specified now, so that the instance can be used to import Ascii data.
		/// If this value is false, the Ascii data have to be analyzed in order to find the missing values.
		/// </summary>
		public bool IsFullySpecified
		{
			get
			{
				return
					null != NumberOfMainHeaderLines &&
					null != IndexOfCaptionLine &&
					null != SeparationStrategy &&
					null != RecognizedStructure &&
					null != NumberFormatCulture &&
					null != DateTimeFormatCulture;
			}
		}

		public bool CopyFrom(object obj)
		{
			if (object.ReferenceEquals(this, obj))
				return true;

			var from = obj as AsciiImportOptions;
			if (null != from)
			{
				this.RenameColumns = from.RenameColumns;
				this.RenameWorksheet = from.RenameWorksheet;
				this.HeaderLinesDestination = from.HeaderLinesDestination;

				this.NumberOfMainHeaderLines = from.NumberOfMainHeaderLines;

				this.IndexOfCaptionLine = from.IndexOfCaptionLine;

				this.SeparationStrategy = null == from.SeparationStrategy ? null : (IAsciiSeparationStrategy)from.SeparationStrategy.Clone();

				this.NumberFormatCulture = null == from.NumberFormatCulture ? null : (System.Globalization.CultureInfo)from.NumberFormatCulture.Clone();

				this.DateTimeFormatCulture = null == from.DateTimeFormatCulture ? null : (System.Globalization.CultureInfo)from.DateTimeFormatCulture.Clone();

				this.RecognizedStructure = from.RecognizedStructure == null ? null : from.RecognizedStructure;

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