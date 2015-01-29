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
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Serialization.Ascii
{
	using Altaxo.Data;

	/// <summary>
	/// Options for export of ASCII files
	/// </summary>
	public class AsciiExportOptions
	{
		/// <summary>
		/// The separator char.
		/// </summary>
		public char SeparatorChar { get; protected set; }

		/// <summary>
		/// Substitute for separator char. Should the separator char be present in header or items, it is replaced by this char.
		/// </summary>
		public char SubstituteForSeparatorChar { get; protected set; }

		/// <summary>
		/// If true, the first line of the exported Ascii file will contain the data column names, separated by the <see cref="SeparatorChar" />.
		/// </summary>
		public bool ExportDataColumnNames { get; set; }

		/// <summary>
		/// If true, the property columns will be exported.
		/// </summary>
		public bool ExportPropertyColumns { get; set; }

		/// <summary>
		/// If true, the property items will be exported with name. In order to do that, each property item will be headed by
		/// "PropColName=". SeparatorChar and Newlines will be removed both from the items text and from the PropertyColumnNames.
		/// </summary>
		public bool ExportPropertiesWithName { get; set; }

		/// <summary>
		/// Holds a dictionary of column types (keys) and functions (values), which convert a AltaxoVariant into a string.
		/// Normally the ToString() function is used on AltaxoVariant to convert to string. By using this dictionary it is possible
		/// to add custom converters.
		/// </summary>
		private Dictionary<System.Type, Func<Altaxo.Data.AltaxoVariant, string>> _typeConverters;

		private IFormatProvider _formatProvider;

		public IFormatProvider FormatProvider
		{
			get { return _formatProvider; }
			set
			{
				if (null == value) throw new ArgumentNullException("value");
				_formatProvider = value;
			}
		}

		/// <summary>
		/// Creates default options: Separator char is Tab, Substitute char is Space, FormatProvider is the CurrentCulture.
		/// </summary>
		public AsciiExportOptions()
		{
			SeparatorChar = '\t';
			SubstituteForSeparatorChar = ' ';
			ExportDataColumnNames = true;
			ExportPropertyColumns = true;
			FormatProvider = System.Globalization.CultureInfo.CurrentCulture;

			_typeConverters = new Dictionary<Type, Func<Altaxo.Data.AltaxoVariant, string>>();
			_typeConverters.Add(typeof(DoubleColumn), GetDefaultConverter(typeof(DoubleColumn)));
			_typeConverters.Add(typeof(DateTimeColumn), GetDefaultConverter(typeof(DateTimeColumn)));
			_typeConverters.Add(typeof(TextColumn), GetDefaultConverter(typeof(TextColumn)));
		}

		/// <summary>
		/// Sets SeparatorChar and SubstituteChar. They must not be the same.
		/// </summary>
		/// <param name="separatorChar">Separator char.</param>
		/// <param name="substituteChar">Substitute char.</param>
		public void SetSeparator(char separatorChar, char substituteChar)
		{
			if (separatorChar == substituteChar)
				throw new ArgumentException("separatorChar == substituteChar");
			SeparatorChar = separatorChar;
			SubstituteForSeparatorChar = substituteChar;
		}

		/// <summary>
		/// Sets the separator char and chooses the substitute char automatically.
		/// </summary>
		/// <param name="separatorChar">The separator char.</param>
		public void SetSeparator(char separatorChar)
		{
			SeparatorChar = separatorChar;
			if ('\t' == SeparatorChar)
				SubstituteForSeparatorChar = ' ';
			else
				SubstituteForSeparatorChar = '_';
		}

		/// <summary>
		/// Sets the converter for the items of a specific column type.
		/// </summary>
		/// <param name="columnType">The column type for which to set the converter.</param>
		/// <param name="stringConverter">The converter function, which converts an AltaxoVariant into a string.</param>
		public void SetConverter(System.Type columnType, Func<Altaxo.Data.AltaxoVariant, string> stringConverter)
		{
			if (columnType == null)
				throw new ArgumentNullException("columnType");

			if (stringConverter != null)
				_typeConverters[columnType] = stringConverter;
			else // stringConverter is null, try to get the default converter
				_typeConverters[columnType] = GetDefaultConverter(columnType);
		}

		public Func<Altaxo.Data.AltaxoVariant, string> GetConverter(System.Type columnType)
		{
			Func<Altaxo.Data.AltaxoVariant, string> result;
			if (_typeConverters.TryGetValue(columnType, out result))
				return result;
			else
				return null;
		}

		#region Helper function

		/// <summary>
		/// Converts a data item to a string.
		/// </summary>
		/// <param name="col">The data column.</param>
		/// <param name="index">Index of the item in the data column, which should be converted.</param>
		/// <returns>The converted item as string.</returns>
		public string DataItemToString(Altaxo.Data.DataColumn col, int index)
		{
			if (col.IsElementEmpty(index))
				return string.Empty;

			string result;
			Func<Altaxo.Data.AltaxoVariant, string> func = null;
			if (_typeConverters.TryGetValue(col.GetType(), out func))
				result = func(col[index]);
			else
				result = DefaultTextConverter(col[index]);

			return result.Replace(SeparatorChar, SubstituteForSeparatorChar);
		}

		/// <summary>
		/// Returns the default converter for a given column type.
		/// </summary>
		/// <param name="columnType">The column type.</param>
		/// <returns>Default converter for the given column type.</returns>
		public Func<AltaxoVariant, string> GetDefaultConverter(System.Type columnType)
		{
			if (columnType == typeof(DoubleColumn))
				return DefaultDoubleConverter;
			else if (columnType == typeof(DateTimeColumn))
				return DefaultDateTimeConverter;
			else return DefaultTextConverter;
		}

		/// <summary>
		/// Converts a given string to a string which will not contain the separator char nor contains newlines.
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		public string ConvertToSaveString(string s)
		{
			s.Replace(SeparatorChar, SubstituteForSeparatorChar);
			s = s.Replace('\r', ' ');
			s = s.Replace('\n', ' ');
			return s;
		}

		private string DefaultTextConverter(AltaxoVariant x)
		{
			string s = x.ToString();

			s = s.Replace('\r', ' ');
			s = s.Replace('\n', ' ');

			return s;
		}

		private string DefaultDoubleConverter(AltaxoVariant x)
		{
			return ((double)x).ToString("r", _formatProvider);
		}

		private string DefaultDateTimeConverter(AltaxoVariant x)
		{
			return ((DateTime)x).ToString("o");
		}

		#endregion Helper function
	}
}