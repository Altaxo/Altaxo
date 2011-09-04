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
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Data;

using Altaxo;
using Altaxo.Collections;

namespace Altaxo.Gui.Common
{
	/// <summary>
	/// Converts a boolean value of true to <see cref="Visibility.Collapsed"/> and a value of false to <see cref="Visibility.Visible"/>
	/// </summary>
	[ValueConversion(typeof(bool), typeof(Visibility))]
	public class VisibilityCollapsedForTrueConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return ((bool)value) ? Visibility.Collapsed : Visibility.Visible;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return ((Visibility)value) == Visibility.Collapsed;
		}
	}

	/// <summary>
	/// Converts a boolean value of false to <see cref="Visibility.Collapsed"/> and a value of true to <see cref="Visibility.Visible"/>
	/// </summary>
	[ValueConversion(typeof(bool), typeof(Visibility))]
	public class VisibilityCollapsedForFalseConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return ((bool)value) ? Visibility.Visible : Visibility.Collapsed;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return ((Visibility)value) == Visibility.Visible;
		}
	}


	/// <summary>
	/// Converts a boolean value of false to <see cref="Visibility.Collapsed"/> and a value of true to <see cref="Visibility.Visible"/>
	/// </summary>
	[ValueConversion(typeof(bool), typeof(Visibility))]
	public class VisibilityHiddenForFalseConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return ((bool)value) ? Visibility.Visible : Visibility.Hidden;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return ((Visibility)value) == Visibility.Visible;
		}
	}
}
