#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2016 Dr. Dirk Lellinger
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

using Altaxo.Drawing;
using Altaxo.Drawing.DashPatternManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace Altaxo.Gui.Drawing.DashPatternManagement
{
	public class DashPatternToItemNameConverter : IValueConverter
	{
		private System.Windows.Controls.ComboBox _comboBox;
		private object _originalToolTip;

		/// <summary>
		/// Initializes a new instance of the <see cref="DashPatternToItemNameConverter"/> class.
		/// The thus created instance is appropriate to convert IDashPattern to string, but not vice versa.
		/// </summary>
		public DashPatternToItemNameConverter()
		{
		}

		public DashPatternToItemNameConverter(System.Windows.Controls.ComboBox comboBox)
		{
			_comboBox = comboBox;
		}

		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			string result;
			if (null == value)
			{
				result = string.Empty;
			}
			else if (value is Altaxo.Drawing.DashPatterns.Custom)
			{
				var stb = new StringBuilder();
				var custom = ((Altaxo.Drawing.DashPatterns.Custom)value);
				for (int i = 0; i < custom.Count - 1; ++i)
					stb.AppendFormat("{0}; ", custom[i]);
				stb.AppendFormat("{0}", custom[custom.Count - 1]);
				result = stb.ToString();
			}
			else if (value is IDashPattern)
			{
				result = value.GetType().Name;
			}
			else if (value is string)
			{
				result = "DasistStuss: " + (string)value;
			}
			else
			{
				throw new ArgumentException("Unexpected class to convert: " + value.GetType().ToString(), nameof(value));
			}
			return result;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			string text = (string)value;
			string error;
			var result = ConvertFromText(text, out error);

			if (error == null)
				return result; // Ok conversion to a custom Dash pattern was possible
			else
				return Binding.DoNothing; // For all other cases: do nothing, since we can not deduce from the name only to which dash pattern list the item belongs.
		}

		/// <summary>
		/// Converts text, e.g. '2;2', to a custom dash pattern style. The Gui culture is used for the number format; valid separator chars are space, tab and semicolon.
		/// </summary>
		/// <param name="text">The text.</param>
		/// <param name="error">The error.</param>
		/// <returns></returns>
		private static IDashPattern ConvertFromText(string text, out string error)
		{
			error = null;
			text = text.Trim();
			var parts = text.Split(new char[] { ';', '\t', ' ' }, StringSplitOptions.RemoveEmptyEntries);
			var valList = new List<double>();
			foreach (var part in parts)
			{
				var parttrimmed = part.Trim();
				if (string.IsNullOrEmpty(parttrimmed))
					continue;

				double val;
				if (!Altaxo.Serialization.GUIConversion.IsDouble(parttrimmed, out val))
					error = "Provided string can not be converted to a numeric value";
				else if (!(val > 0 && val < double.MaxValue))
					error = "One of the provided values is not a valid positive number";
				else
					valList.Add(val);
			}

			if (valList.Count < 1 && error == null) // only use this error, if there is no other error;
				error = "At least one number is neccessary";

			return null != error ? null : new Altaxo.Drawing.DashPatterns.Custom(valList);
		}

		public string EhValidateText(object obj, System.Globalization.CultureInfo info)
		{
			string text = (string)obj;
			string error;
			var result = ConvertFromText(text, out error);

			if (null != _comboBox)
			{
				if (null != error)
				{
					_originalToolTip = _comboBox.ToolTip;
					_comboBox.ToolTip = error;
				}
				else
				{
					_comboBox.ToolTip = _originalToolTip;
					_originalToolTip = null;
				}
			}

			return error;
		}
	}

	public class DashPatternToImageSourceConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return null;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	public class DashPatternToListNameConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			var listName = DashPatternListManager.Instance.GetParentList(value as IDashPattern)?.Name;
			if (null != listName)
			{
				var entry = DashPatternListManager.Instance.GetEntryValue(listName);
				string levelName = Enum.GetName(typeof(Altaxo.Main.ItemDefinitionLevel), entry.Level);
				return levelName + "/" + listName;
			}
			else
			{
				return "<<no parent list>>";
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}