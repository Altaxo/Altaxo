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
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Reflection;
using System.Diagnostics;

namespace Altaxo.Gui.Common.Drawing
{
	public partial class ScaleComboBox : DimensionfulQuantityImageComboBox
	{
		/// <summary>The items here were stored for the x scale only.</summary>
		static Dictionary<double, ImageSource> _cachedXImages = new Dictionary<double, ImageSource>();

		/// <summary>The items here were stored for the x scale only.</summary>
		static Dictionary<double, ImageSource> _cachedYImages = new Dictionary<double, ImageSource>();

		bool _isForYScale;

		static readonly double[] _initialValues = new double[] { 0.1, 0.2, 0.5, 1.0, 2.0, 5.0, 10.0 };

		public ScaleComboBox()
		{
			UnitEnvironment = RelationEnvironment.Instance;
			_converter.ValidationAfterSuccessfulConversion = EhValidateQuantity;

			InitializeComponent();

			foreach (var e in _initialValues)
				Items.Add(new ImageComboBoxItem(this, new Units.DimensionfulQuantity(e, Units.Dimensionless.Unity.Instance).AsQuantityIn(UnitEnvironment.DefaultUnit)));

			_img.Source = GetImage(SelectedQuantityInSIUnits, _isForYScale);
		}

	


		private static ValidationResult EhValidateQuantity(Units.DimensionfulQuantity quantity)
		{
			string error = null;
			double val = quantity.Value;
			if (val == 0)
				error = "Value must be non-zero";
			else if (double.IsInfinity(val))
				error = "Value must not be infinity";
			if (double.IsNaN(val))
				error = "Value must be a valid number";

			return error == null ? ValidationResult.ValidResult : new ValidationResult(false, error);
		}


		public bool IsForYScale
		{
			set
			{
				_isForYScale = value;
			}
		}

		protected override void OnSelectedQuantityChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			base.OnSelectedQuantityChanged(obj, args);

			if (null != _img)
			{
				var val = SelectedQuantityInSIUnits;
				_img.Source = GetImage(val, _isForYScale);
			}
		}


		public override ImageSource GetItemImage(object item)
		{
			double value = ((Units.DimensionfulQuantity)item).AsValueInSIUnits;
			ImageSource result;
			if (_isForYScale)
			{
				if (!_cachedYImages.TryGetValue(value, out result))
					_cachedYImages.Add(value, result = GetImage(value, _isForYScale));
			}
			else
			{
				if (!_cachedXImages.TryGetValue(value, out result))
					_cachedXImages.Add(value, result = GetImage(value, _isForYScale));
			}

			return result;
		}

		public override string GetItemText(object item)
		{
			return (string)_converter.Convert(item, typeof(string), null, System.Globalization.CultureInfo.CurrentUICulture);
		}

		public static ImageSource GetImage(double scale, bool isForY)
		{
			double height = 1;
			double width = 1;
			double lineWidth = 0.05;

			var group = new DrawingGroup();

			// draws a transparent outline to fix the borders
			var outlineDrawing = new GeometryDrawing();
			outlineDrawing.Geometry = new RectangleGeometry(new Rect(-lineWidth, -lineWidth, width + lineWidth, height + lineWidth));
			outlineDrawing.Pen = new Pen(Brushes.Transparent, 0);
			group.Children.Add(outlineDrawing);

			var absscale = Math.Abs(scale);
			if (absscale == 1)
			{
				var drawing1 = new GeometryDrawing();
				drawing1.Geometry = new EllipseGeometry(new Point(width / 2, height / 2), width / 4, height / 4);
				if (scale > 0)
					drawing1.Brush = new RadialGradientBrush(Color.FromRgb(204, 204, 255), Color.FromRgb(100, 100, 255));
				else
					drawing1.Brush = new RadialGradientBrush(Color.FromRgb(100, 100, 255), Color.FromRgb(204, 204, 255));

				group.Children.Add(drawing1);

				drawing1 = new GeometryDrawing();
				drawing1.Geometry = new LineGeometry(new Point(0, height / 2), new Point(width, height / 2));
				drawing1.Pen = new Pen(Brushes.Black, lineWidth);
				group.Children.Add(drawing1);

				drawing1 = new GeometryDrawing();
				drawing1.Geometry = new LineGeometry(new Point(width / 2, 0), new Point(width / 2, height));
				drawing1.Pen = new Pen(Brushes.Black, lineWidth);
				group.Children.Add(drawing1);
			}
			else
			{

				var drawing1 = new GeometryDrawing();
				if (absscale > 2)
					drawing1.Geometry = new EllipseGeometry(new Point(width / 2, height / 2), (width / 2), (height / 2) / absscale);
				else if (absscale > 1)
					drawing1.Geometry = new EllipseGeometry(new Point(width / 2, height / 2), (width / 4) * absscale, (height / 4));
				else if (absscale > 0.5)
					drawing1.Geometry = new EllipseGeometry(new Point(width / 2, height / 2), (width / 4), (height / 4) / absscale);
				else if (absscale > 0)
					drawing1.Geometry = new EllipseGeometry(new Point(width / 2, height / 2), (width / 2) * absscale, (height / 2));

				//	drawing1.Brush = new RadialGradientBrush(Color.FromRgb(204, 204, 255), Color.FromRgb(100, 100, 255));
				if (scale > 0)
					drawing1.Brush = new LinearGradientBrush(Color.FromRgb(204, 204, 255), Color.FromRgb(100, 100, 255), 0);
				else
					drawing1.Brush = new LinearGradientBrush(Color.FromRgb(100, 100, 255), Color.FromRgb(204, 204, 255), 0);

				group.Children.Add(drawing1);

				drawing1 = new GeometryDrawing();
				drawing1.Geometry = new LineGeometry(new Point(0, height / 2), new Point(width, height / 2));
				drawing1.Pen = new Pen(Brushes.Black, lineWidth);
				group.Children.Add(drawing1);

				Point d11, d12, d13;
				Point d21, d22, d23;

				if (absscale > 1)
				{
					// triangles pointing outside;
					d11 = new Point(0, height / 2);
					d12 = new Point(width / 4, height / 2 + height / 8);
					d13 = new Point(width / 4, height / 2 - height / 8);

					d21 = new Point(width, height / 2);
					d22 = new Point(width - width / 4, height / 2 + height / 8);
					d23 = new Point(width - width / 4, height / 2 - height / 8);
				}
				else
				{
					// triangles pointing inside
					d11 = new Point(width / 4, height / 2);
					d12 = new Point(0, height / 2 + height / 8);
					d13 = new Point(0, height / 2 - height / 8);

					d21 = new Point(width - width / 4, height / 2);
					d22 = new Point(width, height / 2 + height / 8);
					d23 = new Point(width, height / 2 - height / 8);
				}

				// now adjust the triangles a little
				if (absscale > 2)
				{
					// nothing to do here, the triangles are already max outside
				}
				else if (absscale > 1)
				{
					double offs = width / 2 - absscale * (width / 4);
					d11.X += offs; d12.X += offs; d13.X += offs;
					d21.X -= offs; d22.X -= offs; d23.X -= offs;
				}
				else if (absscale > 0.5)
				{
					// nothing to do here, the triangles are already max outside
				}
				else if (absscale > 0)
				{
					double offs = width / 4 - absscale * width / 2;
					d11.X += offs; d12.X += offs; d13.X += offs;
					d21.X -= offs; d22.X -= offs; d23.X -= offs;
				}

				var fig1 = new PathFigure(d11, new PathSegment[] { new LineSegment(d12, false), new LineSegment(d13, false) }, true);
				var fig2 = new PathFigure(d21, new PathSegment[] { new LineSegment(d22, false), new LineSegment(d23, false) }, true);

				drawing1 = new GeometryDrawing();
				drawing1.Geometry = new PathGeometry(new PathFigure[] { fig1, fig2 });
				drawing1.Brush = Brushes.Black;
				group.Children.Add(drawing1);
			}



			if (isForY)
				group.Transform = new RotateTransform(90, width / 2, height / 2);


			var geometryImage = new DrawingImage(group);

			// Freeze the DrawingImage for performance benefits.
			geometryImage.Freeze();
			return geometryImage;
		}
	}
}
