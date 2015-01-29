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
using System.Windows.Controls;
using System.Windows.Media;

namespace Altaxo.Gui.Common.Drawing
{
	public partial class ShearComboBox : DimensionfulQuantityImageComboBox
	{
		private static Dictionary<double, ImageSource> _cachedImages = new Dictionary<double, ImageSource>();

		private static readonly double[] _initialValues = new double[] { -2.0, -1.0, 0.0, 1.0, 2.0 };

		static ShearComboBox()
		{
			SelectedQuantityProperty.OverrideMetadata(typeof(ShearComboBox), new FrameworkPropertyMetadata(new Altaxo.Units.DimensionfulQuantity(0, Units.Dimensionless.Unity.Instance)));
		}

		public ShearComboBox()
		{
			UnitEnvironment = RelationEnvironment.Instance;
			_converter.ValidationAfterSuccessfulConversion = EhValidateQuantity;

			InitializeComponent();

			foreach (var e in _initialValues)
				Items.Add(new ImageComboBoxItem(this, new Units.DimensionfulQuantity(e, Units.Dimensionless.Unity.Instance).AsQuantityIn(UnitEnvironment.DefaultUnit)));

			_img.Source = GetImage(SelectedQuantityInSIUnits);
		}

		private static ValidationResult EhValidateQuantity(Units.DimensionfulQuantity quantity)
		{
			string error = null;
			double val = quantity.AsValueInSIUnits;
			if (double.IsInfinity(val))
				error = "Value must not be infinity";
			else if (double.IsNaN(val))
				error = "Value must be a valid number";

			return error == null ? ValidationResult.ValidResult : new ValidationResult(false, error);
		}

		protected override void OnSelectedQuantityChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			base.OnSelectedQuantityChanged(obj, args);

			if (null != _img)
			{
				var val = SelectedQuantityInSIUnits;
				_img.Source = GetImage(val);
			}
		}

		public override ImageSource GetItemImage(object item)
		{
			double val = ((Units.DimensionfulQuantity)item).AsValueInSIUnits;
			ImageSource result;
			if (!_cachedImages.TryGetValue(val, out result))
				_cachedImages.Add(val, result = GetImage(val));
			return result;
		}

		public override string GetItemText(object item)
		{
			return (string)_converter.Convert(item, typeof(string), null, System.Globalization.CultureInfo.CurrentUICulture);
		}

		public static ImageSource GetImage(double shear)
		{
			const double height = 1;
			const double width = 1;
			const double lineWidth = 0.08;
			const double lwHalf = lineWidth / 2;

			// draws a transparent outline to fix the borders
			var outlineDrawing = new GeometryDrawing();
			outlineDrawing.Geometry = new RectangleGeometry(new Rect(-lineWidth, -lineWidth, width + lineWidth, height + lineWidth));
			outlineDrawing.Pen = new Pen(Brushes.Transparent, 0);
			//	outlineDrawing.Brush = new SolidColorBrush(Colors.LightBlue);

			//
			// Create the Geometry to draw.
			//
			shear = -shear;
			var geometryGroup = new GeometryGroup();
			var figure1 = new PathFigure();
			if (shear == 0)
			{
				figure1.StartPoint = new Point(0, 0);
				figure1.Segments.Add(new LineSegment(new Point(0, height), true));
				figure1.Segments.Add(new LineSegment(new Point(width, height), true));
				figure1.Segments.Add(new ArcSegment(new Point(0, 0), new Size(width, height), 90, false, SweepDirection.Counterclockwise, true));
				geometryGroup.Children.Add(new PathGeometry(new PathFigure[] { figure1 }));

				geometryGroup.Children.Add(new EllipseGeometry(new Point(width / 3, height - height / 3), lwHalf, lwHalf));
			}
			else if (shear > 0)
			{
				double phi = Math.PI / 2 - Math.Atan(shear);
				Point start = new Point(width * Math.Cos(phi), height - height * Math.Sin(phi));
				figure1.StartPoint = start;
				figure1.Segments.Add(new LineSegment(new Point(0, height), true));
				figure1.Segments.Add(new LineSegment(new Point(width, height), true));
				figure1.Segments.Add(new ArcSegment(start, new Size(width, height), 180 * phi / Math.PI, false, SweepDirection.Counterclockwise, true));

				var figure2 = new PathFigure();
				figure2.StartPoint = new Point(0, 0);
				figure2.Segments.Add(new ArcSegment(start, new Size(width, height), 180 - 180 * phi / Math.PI, false, SweepDirection.Clockwise, true));
				figure2.IsFilled = false;

				geometryGroup.Children.Add(new PathGeometry(new PathFigure[] { figure1, figure2 }));
			}
			else if (shear < 0)
			{
				double phi = Math.PI / 2 - Math.Atan(-shear);
				Point start = new Point(width - width * Math.Cos(phi), height - height * Math.Sin(phi));
				figure1.StartPoint = start;
				figure1.Segments.Add(new LineSegment(new Point(width, height), true));
				figure1.Segments.Add(new LineSegment(new Point(0, height), true));
				figure1.Segments.Add(new ArcSegment(start, new Size(width, height), 180 * phi / Math.PI, false, SweepDirection.Clockwise, true));

				var figure2 = new PathFigure();
				figure2.StartPoint = new Point(width, 0);
				figure2.Segments.Add(new ArcSegment(start, new Size(width, height), 180 - 180 * phi / Math.PI, false, SweepDirection.Counterclockwise, true));
				figure2.IsFilled = false;

				geometryGroup.Children.Add(new PathGeometry(new PathFigure[] { figure1, figure2 }));
			}

			var geometryDrawing = new GeometryDrawing();
			geometryDrawing.Geometry = geometryGroup;

			// Outline the drawing with a solid color.
			geometryDrawing.Pen = new Pen(Brushes.Black, lineWidth) { LineJoin = PenLineJoin.Round };
			geometryDrawing.Brush =
					new LinearGradientBrush(
							Color.FromRgb(100, 100, 255),
							Color.FromRgb(204, 204, 255),
							new Point(0, 0),
							new Point(1, 1));

			var group = new DrawingGroup();
			group.Children.Add(outlineDrawing);
			group.Children.Add(geometryDrawing);
			var geometryImage = new DrawingImage(group);

			// Freeze the DrawingImage for performance benefits.
			geometryImage.Freeze();
			return geometryImage;
		}
	}
}