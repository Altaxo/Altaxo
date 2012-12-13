using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace Altaxo.Serialization.Ascii
{
	/// <summary>
	/// Stores information about how to analyze an ASCII data file.
	/// </summary>
	public class AsciiDocumentAnalysisOptions : Main.ICopyFrom
	{
		static AsciiDocumentAnalysisOptions _defaultOptions = new AsciiDocumentAnalysisOptions();

		private int _numberOfLinesToAnalyze;

		private HashSet<CultureInfo> _numberFormatsToTest;

		private HashSet<CultureInfo> _dateTimeFormatsToTest;




		/// <summary>
		/// Initializes a new instance of the <see cref="AsciiDocumentAnalysisOptions"/> class with default settings.
		/// The default settings are NumberOfLinesToAnalyze = 30 and NumberFormatsToTest and DateTimeFormatsToTest is filled
		/// with at least the InvariantCulture, the CurrentCulture, CurrentUICultere and InstalledUICulture.
		/// </summary>
		public AsciiDocumentAnalysisOptions()
		{
			_numberOfLinesToAnalyze = 30;
			_numberFormatsToTest = new HashSet<CultureInfo>();
			_dateTimeFormatsToTest = new HashSet<CultureInfo>();

			_numberFormatsToTest.Add(System.Globalization.CultureInfo.InvariantCulture);
			_numberFormatsToTest.Add(System.Globalization.CultureInfo.CurrentCulture);
			_numberFormatsToTest.Add(System.Globalization.CultureInfo.CurrentUICulture);
			_numberFormatsToTest.Add(System.Globalization.CultureInfo.InstalledUICulture);

			_dateTimeFormatsToTest.Add(System.Globalization.CultureInfo.InvariantCulture);
			_dateTimeFormatsToTest.Add(System.Globalization.CultureInfo.CurrentCulture);
			_dateTimeFormatsToTest.Add(System.Globalization.CultureInfo.CurrentUICulture);
			_dateTimeFormatsToTest.Add(System.Globalization.CultureInfo.InstalledUICulture);
		}

		public AsciiDocumentAnalysisOptions(AsciiDocumentAnalysisOptions from)
		{
			CopyFrom(from);
		}


		/// <summary>
		/// Gets the default instance. Note that this instance may change to reflect the current user's settings. To get a fresh new instance
		/// with default settings, please use the parameterless constructor.
		/// </summary>
		public static AsciiDocumentAnalysisOptions DefaultInstance
		{
			get
			{
				return _defaultOptions;
			}

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
				this._numberFormatsToTest = from._numberFormatsToTest;
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
		public object Clone()
		{
			return new AsciiDocumentAnalysisOptions(this);
		}
	}
}
