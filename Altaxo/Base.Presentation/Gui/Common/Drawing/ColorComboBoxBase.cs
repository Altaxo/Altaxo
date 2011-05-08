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

namespace Altaxo.Gui.Common.Drawing
{
	public class ColorComboBoxBase : ImageComboBox
	{
		#region Inner classes

		protected class ColorComboBoxItem : ImageComboBoxItem
		{
			DrawingImage _cachedDrawing;
			
			public override string Text
			{
				get
				{
					if (Value is NamedColor)
						return ((NamedColor)Value).Name;
					else
						return NamedColor.GetColorName((AxoColor)Value);
				}
			}

			public override ImageSource Image
			{
				get
				{
					if (null == _cachedDrawing)
						_cachedDrawing = GetImage((NamedColor)Value);

					return _cachedDrawing;
				}
			}
		}

		#endregion

		protected static List<NamedColor> _knownColors = new List<NamedColor>();

		/// <summary>Cached items of all known colors.</summary>
		protected static Dictionary<NamedColor, ColorComboBoxItem> _knownColorItems = new Dictionary<NamedColor, ColorComboBoxItem>();

		/// <summary>Helper brush.</summary>
		protected static Brush _checkerBrush;

		protected Dictionary<NamedColor, ImageComboBoxItem> _cachedItems = new Dictionary<NamedColor, ImageComboBoxItem>();



		static ColorComboBoxBase()
		{
			foreach (var e in KnownAxoColors.Items)
			{
				var item = new ColorComboBoxItem() { Value = e };
				_knownColorItems.Add(e, item);
				_knownColors.Add(e);
			}
		}

	

		public static Brush CreateCheckerBrush(double checkerRepeatLength)
		{
			DrawingBrush checkerBrush = new DrawingBrush();

			GeometryDrawing backgroundSquare =
					new GeometryDrawing(
							Brushes.White,
							null,
							new RectangleGeometry(new Rect(0, 0, checkerRepeatLength, checkerRepeatLength)));

			double c2 = checkerRepeatLength / 2;

			GeometryGroup aGeometryGroup = new GeometryGroup();
			aGeometryGroup.Children.Add(new RectangleGeometry(new Rect(0, 0, c2, c2)));
			aGeometryGroup.Children.Add(new RectangleGeometry(new Rect(c2, c2, c2, c2)));

			GeometryDrawing checkers = new GeometryDrawing(Brushes.Black, null, aGeometryGroup);

			DrawingGroup checkersDrawingGroup = new DrawingGroup();
			checkersDrawingGroup.Children.Add(backgroundSquare);
			checkersDrawingGroup.Children.Add(checkers);

			checkerBrush.Drawing = checkersDrawingGroup;
			checkerBrush.Viewport = new Rect(0, 0, 0.5, 0.5);
			checkerBrush.TileMode = TileMode.Tile;

			return checkerBrush;
		}

		public static DrawingImage GetImage(NamedColor namedColor)
		{
			Color val = GuiHelper.ToWpf(namedColor.Color);

			const double border = 0.1;
			const double height = 1;
			const double width = 2;
			//
			// Create the Geometry to draw.
			//

			if (null == _checkerBrush)
				_checkerBrush = CreateCheckerBrush(Math.Min(width, height) / 2);

			var drawingGroup = new DrawingGroup();

			var geometryDrawing = new GeometryDrawing() { Geometry = new RectangleGeometry(new Rect(0, 0, width, height)) };
			geometryDrawing.Brush = Brushes.Black;
			drawingGroup.Children.Add(geometryDrawing);

			var innerRect = new Rect(border, border, width - 2 * border, height - 2 * border);

			geometryDrawing = new GeometryDrawing() { Geometry = new RectangleGeometry(innerRect) };
			geometryDrawing.Brush = _checkerBrush;
			drawingGroup.Children.Add(geometryDrawing);

			geometryDrawing = new GeometryDrawing() { Geometry = new RectangleGeometry(innerRect) };
			geometryDrawing.Brush = new SolidColorBrush(val);
			drawingGroup.Children.Add(geometryDrawing);

			DrawingImage geometryImage = new DrawingImage(drawingGroup);

			// Freeze the DrawingImage for performance benefits.
			geometryImage.Freeze();
			return geometryImage;
		}

		protected static List<NamedColor> GetFilteredList(List<NamedColor> originalList, string filterString)
		{
			var result = new List<NamedColor>();
			filterString = filterString.ToLowerInvariant();
			foreach (var item in originalList)
			{
				if (item.Name.ToLowerInvariant().StartsWith(filterString))
					result.Add(item);
			}
			return result;
		}
	}
}
