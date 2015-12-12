using Altaxo.Drawing;
using Altaxo.Drawing.D3D;
using Altaxo.Drawing.D3D.Material;
using Altaxo.Graph;
using Altaxo.Graph.Graph3D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Altaxo.Gui.Graph3D.Material
{
    public class MaterialToImageSourceConverter : IValueConverter
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
            if (value is IMaterial)
                return GetImageSourceFromBrushX((IMaterial)value, _width, _height);
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

        public static ImageSource GetImageSourceFromBrushX(IMaterial val, int width, int height)
        {
            //
            // Create the Geometry to draw.
            //

            if (val.HasColor)
            {
                return GetImageSourceFromAxoColor(val.Color.Color, width, height);
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }

    /// <summary>
    /// Converts a <see cref="IMaterial"/> to a string, which represents the name of this brush.
    /// </summary>
    public class MaterialToMaterialNameConverter : IValueConverter
    {
        /// <summary>
        /// Converts a <see cref="Material"/> to its name.
        /// </summary>
        /// <param name="value">A <see cref="IMaterial"/> object.</param>
        /// <param name="targetType">Ignored. Return type is always string.</param>
        /// <param name="parameter">Ignored</param>
        /// <param name="culture">The culture to use in the converter. Ignored.</param>
        /// <returns>
        /// The name of the brush.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is IMaterial)
                return GetNameForBrushX((IMaterial)value);
            else if (value is NamedColor)
                return ((NamedColor)value).Name;
            else
                return null;
        }

        public static string GetNameForBrushX(IMaterial brush)
        {
            string name;
            if (brush == null)
            {
                name = "<<null>>";
            }
            else if (brush is SolidColor)
            {
                name = brush.Color.Name;
            }
            else
            {
                name = brush.GetType().Name;
            }

            return name;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}