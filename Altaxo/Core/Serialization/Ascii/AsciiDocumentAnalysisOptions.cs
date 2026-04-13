#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2014 Dr. Dirk Lellinger
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
using System.Globalization;
using System.Linq;
using System.Text;

namespace Altaxo.Serialization.Ascii
{
  using System.Collections.Immutable;

  /// <summary>
  /// Stores information about how to analyze an ASCII data file.
  /// </summary>
  public record AsciiDocumentAnalysisOptions : Main.IImmutable
  {
    /// <summary>
    /// Gets the settings storage path for persisting instances in application properties.
    /// </summary>
    public static string SettingsStoragePath = "Altaxo.Options.Serialization.Ascii.DocumentAnalysisOptions";


    /// <summary>Default number of Ascii lines to analyze.</summary>
    public const int DefaultNumberOfLinesToAnalyze = 1000;

    #region Serialization

    /// <summary>
    /// V1: 2026-03-13 Moved from AltaxoBase to AltaxoCore
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Serialization.Ascii.AsciiDocumentAnalysisOptions", 0)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(AsciiDocumentAnalysisOptions), 1)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object o, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (AsciiDocumentAnalysisOptions)o;

        // info.AddBaseValueEmbedded(s,typeof(GraphDocument).BaseType);
        // now the data of our class
        info.AddValue("NumberOfLinesToAnalyze", s.NumberOfLinesToAnalyze);

        info.CreateArray("NumberFormatsToTest", s.NumberFormatsToTest.Count);
        foreach (var cultureInfo in s.NumberFormatsToTest)
          info.AddValue("e", cultureInfo.LCID);
        info.CommitArray();

        info.CreateArray("DateTimeFormatsToTest", s.DateTimeFormatsToTest.Count);
        foreach (var cultureInfo in s.DateTimeFormatsToTest)
          info.AddValue("e", cultureInfo.LCID);
        info.CommitArray();
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (AsciiDocumentAnalysisOptions?)o ?? new AsciiDocumentAnalysisOptions();

        //  info.GetBaseValueEmbedded(s,typeof(GraphDocument).BaseType,parent);
        var numberOfLinesToAnalyze = info.GetInt32("NumberOfLinesToAnalyze");

        int count;

        count = info.OpenArray("NumberFormatsToTest");
        var numberFormatsToTest = new List<CultureInfo>(count);
        for (int i = 0; i < count; ++i)
        {
          var lcid = info.GetInt32("e");
          numberFormatsToTest.Add(System.Globalization.CultureInfo.GetCultureInfo(lcid));
        }
        info.CloseArray(count);

        count = info.OpenArray("DateTimeFormatsToTest");
        var dateTimeFormatsToTest = new List<CultureInfo>(count);
        for (int i = 0; i < count; ++i)
        {
          var lcid = info.GetInt32("e");
          dateTimeFormatsToTest.Add(System.Globalization.CultureInfo.GetCultureInfo(lcid));
        }
        info.CloseArray(count);

        return new AsciiDocumentAnalysisOptions
        {
          NumberOfLinesToAnalyze = numberOfLinesToAnalyze,
          NumberFormatsToTest = numberFormatsToTest.ToImmutableHashSet(),
          DateTimeFormatsToTest = dateTimeFormatsToTest.ToImmutableHashSet(),
        };
      }
    }

    #endregion Serialization

    /// <inheritdoc/>
    public override string ToString()
    {
      var stb = new StringBuilder();
      bool delOneAtEnd = false;
      stb.AppendFormat("#Lines: {0} ", NumberOfLinesToAnalyze);

      if (NumberFormatsToTest is not null)
      {
        stb.Append("| ");
        var cu = new SortedSet<string>(NumberFormatsToTest.Select(x => x.ThreeLetterISOLanguageName));
        foreach (var s in cu)
        {
          stb.Append(s);
          stb.Append(",");
          delOneAtEnd = true;
        }
      }

      if (delOneAtEnd)
        stb.Length -= 1;

      return stb.ToString();
    }

    /// <summary>
    /// Tests all member variables and adjusts them to valid values.
    /// </summary>
    /// <param name="options">The options.</param>
    /// <returns>A corrected instance with valid member values.</returns>
    protected static AsciiDocumentAnalysisOptions TestAndAdjustMembersToValidValues(AsciiDocumentAnalysisOptions options)
    {
      var result = options;
      // Test the deserialized instance for appropriate member values
      if (options.NumberOfLinesToAnalyze <= 0)
        result = options with { NumberOfLinesToAnalyze = DefaultNumberOfLinesToAnalyze };
      if (options.NumberFormatsToTest.Count == 0)
        result = options with { NumberFormatsToTest = [CultureInfo.InvariantCulture] }; ;
      if (options.DateTimeFormatsToTest.Count == 0)
        result = options with { DateTimeFormatsToTest = [CultureInfo.InvariantCulture] };
      return result;
    }

    /// <summary>
    /// Initializes an instance of <see cref="AsciiDocumentAnalysisOptions"/> with the default system values.
    /// </summary>
    /// <returns>An instance initialized with the default system cultures.</returns>
    protected static AsciiDocumentAnalysisOptions InitializeDefaultSystemValues()
    {
      return InitializeWithCultures(
            System.Globalization.CultureInfo.InvariantCulture,
            System.Globalization.CultureInfo.CurrentCulture,
            System.Globalization.CultureInfo.CurrentUICulture,
            System.Globalization.CultureInfo.InstalledUICulture
            );
    }

    /// <summary>
    /// Initializes an instance of <see cref="AsciiDocumentAnalysisOptions"/> with the given cultures.
    /// </summary>
    /// <param name="cultures">The cultures to test.</param>
    /// <returns>An instance initialized with the specified cultures.</returns>
    protected static AsciiDocumentAnalysisOptions InitializeWithCultures(params CultureInfo[] cultures)
    {
      return new AsciiDocumentAnalysisOptions
      {
        NumberOfLinesToAnalyze = DefaultNumberOfLinesToAnalyze,
        NumberFormatsToTest = cultures.ToImmutableHashSet(),
        DateTimeFormatsToTest = cultures.ToImmutableHashSet(),
      };

    }

    /// <summary>
    /// Gets options initialized with a default set of system cultures.
    /// </summary>
    /// <returns>The default system options.</returns>
    public static AsciiDocumentAnalysisOptions GetDefaultSystemOptions()
    {
      return InitializeDefaultSystemValues();
    }

    /// <summary>
    /// Creates options that test the provided cultures.
    /// </summary>
    /// <param name="cultures">The cultures to include for number and date/time parsing tests.</param>
    /// <returns>The initialized options.</returns>
    public static AsciiDocumentAnalysisOptions GetOptionsForCultures(params System.Globalization.CultureInfo[] cultures)
    {
      return InitializeWithCultures(cultures);
    }

    /// <summary>
    /// Gets or sets the number of lines used to analyze the structure of an ASCII data file.
    /// </summary>
    /// <value>
    /// The number of lines to analyze.
    /// </value>
    public int NumberOfLinesToAnalyze
    {
      get { return field; }
      init
      {
        if (value <= 0)
          throw new ArgumentOutOfRangeException(nameof(NumberOfLinesToAnalyze), "Number of lines to analyze must be greater than zero.");
        field = value;
      }
    }


    /// <summary>
    /// Gets a set of cultures used to test whether a substring of text represents a number.
    /// </summary>
    /// <remarks>
    /// You may add additional cultures to test, but note that this will increase the analysis time.
    /// </remarks>
    public ImmutableHashSet<CultureInfo> NumberFormatsToTest
    {
      get { return field; }
      init
      {
        if (value is null || value.Count == 0)
          throw new ArgumentException("Number formats to test must contain at least one culture.", nameof(NumberFormatsToTest));
        field = value;
      }
    } = [CultureInfo.InvariantCulture];

    /// <summary>
    /// Gets a set of cultures used to test whether a substring of text represents a date/time value.
    /// </summary>
    /// <remarks>
    /// You may add additional cultures to test, but note that this will increase the analysis time.
    /// </remarks>
    public ImmutableHashSet<CultureInfo> DateTimeFormatsToTest
    {
      get { return field; }
      init
      {
        if (value is null || value.Count == 0)
          throw new ArgumentException("DateTime formats to test must contain at least one culture.", nameof(DateTimeFormatsToTest));
        field = value;
      }
    } = [CultureInfo.InvariantCulture];
  }
}
