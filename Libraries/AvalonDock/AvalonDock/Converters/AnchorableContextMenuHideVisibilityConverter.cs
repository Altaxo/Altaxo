﻿/************************************************************************
   AvalonDock

   Copyright (C) 2007-2013 Xceed Software Inc.

   This program is provided to you under the terms of the Microsoft Public
   License (Ms-PL) as published at https://opensource.org/licenses/MS-PL
 ************************************************************************/

using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace AvalonDock.Converters
{
	public class AnchorableContextMenuHideVisibilityConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if ((values.Count() == 2)
			  && (values[0] != DependencyProperty.UnsetValue)
			  && (values[1] != DependencyProperty.UnsetValue)
			  && (values[1] is bool))
			{
				var canClose = (bool)values[1];

				return canClose ? Visibility.Collapsed : values[0];
			}
			else
			{
				return values[0];
			}
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
