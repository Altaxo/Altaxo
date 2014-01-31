using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Altaxo.Serialization.Ascii
{
	using Altaxo.Main.Properties;

	/// <summary>
	/// Stores information about how to analyze an ASCII data file.
	/// </summary>
	public class AsciiDocumentAnalysisOptions : Main.ICopyFrom
	{
		/// <summary>
		/// Storage path for storing this instance in the application properties.
		/// </summary>
		public static string SettingsStoragePath = "Altaxo.Options.Serialization.Ascii.DocumentAnalysisOptions";

		public static readonly PropertyKey<AsciiDocumentAnalysisOptions> PropertyKeyAsciiDocumentAnalysisOptions = new PropertyKey<AsciiDocumentAnalysisOptions>("2AD2EBB9-F4C6-4BD2-A71B-23974776F5DF", "Table\\AsciiAnalysisOptions", PropertyLevel.All, typeof(Altaxo.Data.DataTable), GetDefaultSystemOptions);

		/// <summary>Default number of Ascii lines to analyze.</summary>
		public const int DefaultNumberOfLinesToAnalyze = 30;

		/// <summary>Number of lines of the Ascii document to analyze.</summary>
		private int _numberOfLinesToAnalyze;

		/// <summary>Number formats to test. Here, the number formats are specified by cultures.</summary>
		private HashSet<CultureInfo> _numberFormatsToTest;

		/// <summary>DateTime formats to test. Here, the DateTime formats are specified by cultures.</summary>
		private HashSet<CultureInfo> _dateTimeFormatsToTest;

		#region Serialization

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(AsciiDocumentAnalysisOptions), 0)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				var s = (AsciiDocumentAnalysisOptions)obj;

				// info.AddBaseValueEmbedded(s,typeof(GraphDocument).BaseType);
				// now the data of our class
				info.AddValue("NumberOfLinesToAnalyze", s._numberOfLinesToAnalyze);

				info.CreateArray("NumberFormatsToTest", s._numberFormatsToTest.Count);
				foreach (var cultureInfo in s._numberFormatsToTest)
					info.AddValue("e", cultureInfo.LCID);
				info.CommitArray();

				info.CreateArray("DateTimeFormatsToTest", s._dateTimeFormatsToTest.Count);
				foreach (var cultureInfo in s._dateTimeFormatsToTest)
					info.AddValue("e", cultureInfo.LCID);
				info.CommitArray();
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				var s = null != o ? (AsciiDocumentAnalysisOptions)o : new AsciiDocumentAnalysisOptions();

				//  info.GetBaseValueEmbedded(s,typeof(GraphDocument).BaseType,parent);
				s._numberOfLinesToAnalyze = info.GetInt32("NumberOfLinesToAnalyze");

				int count;

				count = info.OpenArray("NumberFormatsToTest");
				for (int i = 0; i < count; ++i)
				{
					var lcid = info.GetInt32("e");
					s._numberFormatsToTest.Add(System.Globalization.CultureInfo.GetCultureInfo(lcid));
				}
				info.CloseArray(count);

				count = info.OpenArray("DateTimeFormatsToTest");
				for (int i = 0; i < count; ++i)
				{
					var lcid = info.GetInt32("e");
					s._dateTimeFormatsToTest.Add(System.Globalization.CultureInfo.GetCultureInfo(lcid));
				}
				info.CloseArray(count);

				return s;
			}
		}

		#endregion Serialization

		public override string ToString()
		{
			var stb = new StringBuilder();
			bool delOneAtEnd = false;
			stb.AppendFormat("#Lines: {0} ", _numberOfLinesToAnalyze);

			if (null != _numberFormatsToTest)
			{
				stb.Append("| ");
				SortedSet<string> cu = new SortedSet<string>(_numberFormatsToTest.Select(x => x.ThreeLetterISOLanguageName));
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
		/// Initializes a new instance of the <see cref="AsciiDocumentAnalysisOptions"/> class with empty content. You must set default values for the members afterwards.
		/// </summary>
		protected AsciiDocumentAnalysisOptions()
		{
			_numberFormatsToTest = new HashSet<CultureInfo>();
			_dateTimeFormatsToTest = new HashSet<CultureInfo>();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AsciiDocumentAnalysisOptions"/> class with values from another instance (copy constructor).
		/// </summary>
		/// <param name="from">Instance to copy the values from.</param>
		public AsciiDocumentAnalysisOptions(AsciiDocumentAnalysisOptions from)
		{
			CopyFrom(from);
		}

		/// <summary>
		/// Copies from another object.
		/// </summary>
		/// <param name="obj">The object to copy from.</param>
		/// <returns>True if anything has been copied. Otherwise, the return value is false.</returns>
		public bool CopyFrom(object obj)
		{
			if (object.ReferenceEquals(this, obj))
				return true;
			var from = obj as AsciiDocumentAnalysisOptions;
			if (null != from)
			{
				this._numberOfLinesToAnalyze = from._numberOfLinesToAnalyze;
				this._numberFormatsToTest = new HashSet<CultureInfo>(from._numberFormatsToTest);
				this._dateTimeFormatsToTest = new HashSet<CultureInfo>(from._dateTimeFormatsToTest);
				return true;
			}
			return false;
		}

		/// <summary>
		/// Creates a new object that is a copy of the current instance.
		/// </summary>
		/// <returns>
		/// A new object that is a copy of this instance.
		/// </returns>
		public AsciiDocumentAnalysisOptions Clone()
		{
			return new AsciiDocumentAnalysisOptions(this);
		}

		/// <summary>
		/// Creates a new object that is a copy of the current instance.
		/// </summary>
		/// <returns>
		/// A new object that is a copy of this instance.
		/// </returns>
		object ICloneable.Clone()
		{
			return new AsciiDocumentAnalysisOptions(this);
		}

		/// <summary>
		/// Tests all member variables and adjusts them to valid values.
		/// </summary>
		/// <param name="options">The options.</param>
		protected static void TestAndAdjustMembersToValidValues(AsciiDocumentAnalysisOptions options)
		{
			// Test the deserialized instance for appropriate member values
			if (options.NumberOfLinesToAnalyze <= 0)
				options.NumberOfLinesToAnalyze = DefaultNumberOfLinesToAnalyze;
			if (options.NumberFormatsToTest.Count == 0)
				options.NumberFormatsToTest.Add(CultureInfo.InvariantCulture);
			if (options.DateTimeFormatsToTest.Count == 0)
				options.DateTimeFormatsToTest.Add(CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// Initializes  an instance of <see cref="AsciiDocumentAnalysisOptions"/> with the default system values.
		/// </summary>
		/// <param name="options">The options.</param>
		protected static void InitializeDefaultSystemValues(AsciiDocumentAnalysisOptions options)
		{
			options._numberOfLinesToAnalyze = 30;
			options._numberFormatsToTest.Clear();
			options._dateTimeFormatsToTest.Clear();

			options._numberFormatsToTest.Add(System.Globalization.CultureInfo.InvariantCulture);
			options._numberFormatsToTest.Add(System.Globalization.CultureInfo.CurrentCulture);
			options._numberFormatsToTest.Add(System.Globalization.CultureInfo.CurrentUICulture);
			options._numberFormatsToTest.Add(System.Globalization.CultureInfo.InstalledUICulture);

			options._dateTimeFormatsToTest.Add(System.Globalization.CultureInfo.InvariantCulture);
			options._dateTimeFormatsToTest.Add(System.Globalization.CultureInfo.CurrentCulture);
			options._dateTimeFormatsToTest.Add(System.Globalization.CultureInfo.CurrentUICulture);
			options._dateTimeFormatsToTest.Add(System.Globalization.CultureInfo.InstalledUICulture);
		}

		protected static AsciiDocumentAnalysisOptions GetDefaultSystemOptions()
		{
			var options = new AsciiDocumentAnalysisOptions();
			InitializeDefaultSystemValues(options);
			return options;
		}

		/// <summary>
		/// Gets or sets the number of lines used to analyze the structure of an ASCII data file.
		/// </summary>
		/// <value>
		/// The number of lines to analyze.
		/// </value>
		public int NumberOfLinesToAnalyze
		{
			get { return _numberOfLinesToAnalyze; }
			set { _numberOfLinesToAnalyze = value; }
		}

		/// <summary>
		/// Gets a set of cultures used to test whether an substring of text represents a number. You may add additional cultures to test, but note that this will increase the analyzing time.
		/// </summary>
		public HashSet<CultureInfo> NumberFormatsToTest
		{
			get { return _numberFormatsToTest; }
		}

		/// <summary>
		/// Gets a set of cultures used to test whether an substring of text represents a number. You may add additional cultures to test, but note that this will increase the analyzing time.
		/// </summary>
		public HashSet<CultureInfo> DateTimeFormatsToTest
		{
			get { return _dateTimeFormatsToTest; }
		}
	}
}