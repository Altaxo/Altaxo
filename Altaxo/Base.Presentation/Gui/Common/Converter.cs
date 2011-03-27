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
			return ((bool)value) ? Visibility.Visible : Visibility.Collapsed ;
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
