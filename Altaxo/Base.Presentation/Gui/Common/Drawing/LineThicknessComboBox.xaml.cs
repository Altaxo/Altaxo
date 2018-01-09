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
using System.Windows;
using System.Windows.Media;

namespace Altaxo.Gui.Common.Drawing
{
	using Altaxo.Units;
	using AUL = Altaxo.Units.Length;

	/// <summary>
	/// Interaction logic for LineThicknessComboBox.xaml
	/// </summary>
	public partial class LineThicknessComboBox : LengthImageComboBox
	{
		private static Dictionary<double, ImageSource> _cachedImages = new Dictionary<double, ImageSource>();

		private static readonly double[] _initialValues = new double[] { 0.001, 0.125, 0.25, 0.5, 1, 2, 3, 5, 10 };

		public LineThicknessComboBox()
		{
			UnitEnvironment = LineThicknessEnvironment.Instance;
			InitializeComponent();

			foreach (var e in _initialValues)
				Items.Add(new ImageComboBoxItem(this, new DimensionfulQuantity(e, AUL.Point.Instance)));

			_img.Source = GetImage(SelectedQuantityAsValueInPoints);
		}

		protected override void OnSelectedQuantityChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			base.OnSelectedQuantityChanged(obj, args);

			if (null != _img)
			{
				var val = SelectedQuantityAsValueInPoints;
				_img.Source = GetImage(val);
			}
		}

		public override ImageSource GetItemImage(object item)
		{
			double val = ((DimensionfulQuantity)item).AsValueIn(AUL.Point.Instance);
			ImageSource result;
			if (!_cachedImages.TryGetValue(val, out result))
				_cachedImages.Add(val, result = GetImage(val));
			return result;
		}

		public override string GetItemText(object item)
		{
			return (string)_converter.Convert(item, typeof(string), null, System.Globalization.CultureInfo.CurrentUICulture);
		}

		public static DrawingImage GetImage(double thickness)
		{
			const double nominalHeight = 24; // Height as it occurs in the combobox
			const double height = 1;
			const double width = 2;

			var drawingGroup = new DrawingGroup();
			var bounds = new Rect(0, 0, width, height);

			var geometryDrawing = new GeometryDrawing { Geometry = new RectangleGeometry(bounds) };
			geometryDrawing.Pen = new Pen(Brushes.Transparent, 0);
			drawingGroup.Children.Add(geometryDrawing);

			geometryDrawing = new GeometryDrawing { Geometry = new LineGeometry(new Point(0, height / 2), new Point(width, height / 2)) };
			geometryDrawing.Pen = new Pen(Brushes.Black, Math.Min(height, thickness * height / nominalHeight));
			drawingGroup.Children.Add(geometryDrawing);
			drawingGroup.ClipGeometry = new RectangleGeometry(bounds);

			DrawingImage geometryImage = new DrawingImage(drawingGroup);

			// Freeze the DrawingImage for performance benefits.
			geometryImage.Freeze();
			return geometryImage;
		}
	}
}