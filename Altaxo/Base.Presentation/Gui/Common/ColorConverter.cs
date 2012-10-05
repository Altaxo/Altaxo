#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2012 Dr. Dirk Lellinger
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
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Altaxo.Graph;
using Altaxo.Graph.Gdi;

namespace Altaxo.Gui.Common
{
	/// <summary>
	/// Converts an Altaxo <see cref="Altaxo.Graph.NamedColor"/> value into a <see cref="System.Windows.Media.SolidBrush"/> value.
	/// </summary>
	public class NamedColorToWpfBrushConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value is Altaxo.Graph.NamedColor)
			{
				var c = (Altaxo.Graph.NamedColor)value;
				return new System.Windows.Media.SolidColorBrush(GuiHelper.ToWpf(c.Color));
			}
			else if (value is Altaxo.Graph.AxoColor)
			{
				var c = (Altaxo.Graph.AxoColor)value;
				return new System.Windows.Media.SolidColorBrush(GuiHelper.ToWpf(c));
			}
			else
				return System.Windows.Media.Brushes.Black;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}



	public class NamedColorToColorSetNameConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value is Altaxo.Graph.NamedColor)
			{
				var c = (Altaxo.Graph.NamedColor)value;
				if (c.ParentColorSet != null)
					return c.ParentColorSet.Name;
				else
					return "<<no color set>>";
			}
			else
				return string.Empty;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}


	public class BrushXToImageSourceConverter : IValueConverter
	{
		private int _width = 16;
		private int _height = 16;

		public int Width
		{
			get { return _width; }
			set { _width = value; }
		}

		public int Height
		{
			get { return _height; }
			set { _height = value; }
		}
		

		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value is BrushX)
				return GetImageSourceFromBrushX((BrushX)value, _width, _height);
			else if (value is NamedColor)
				return GetImageSourceFromAxoColor(((NamedColor)value).Color, _width, _height);
			else if (value is AxoColor)
				return GetImageSourceFromAxoColor((AxoColor)value, _width, _height);
			else
				return null;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		public static ImageSource GetImageSourceFromAxoColor(AxoColor axoColor, int width, int height)
		{
			var innerRect = new Rect(0, 0, width, height);
			var geometryDrawing = new GeometryDrawing() { Geometry = new RectangleGeometry(innerRect) };
			geometryDrawing.Brush = new SolidColorBrush(GuiHelper.ToWpf(axoColor));
			DrawingImage geometryImage = new DrawingImage(geometryDrawing);
			geometryImage.Freeze(); // Freeze the DrawingImage for performance benefits.
			return geometryImage;
		}

		public static ImageSource GetImageSourceFromBrushX(BrushX val, int width, int height)
		{
			//
			// Create the Geometry to draw.
			//

			if (val.BrushType == BrushType.SolidBrush)
			{
				return GetImageSourceFromAxoColor(val.Color.Color, width, height);
			}
			else
			{
				return GuiHelper.ToWpf(val, width, height);
			}
		}

	}

}
