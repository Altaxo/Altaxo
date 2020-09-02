#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2017 Dr. Dirk Lellinger
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
//    along with ctrl program; if not, write to the Free Software
//    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
//
/////////////////////////////////////////////////////////////////////////////

#endregion Copyright

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Altaxo.Drawing;

namespace Altaxo.Gui.Drawing.ColorManagement
{
  /// <summary>
  /// Interaction logic for ColorCircleControl.xaml
  /// </summary>
  public partial class ColorCircleSurfaceControl : UserControl
  {
    private IColorCircleModel _colorCircleModel;

    private double[] _hueOfButtons;
    private Point[] _posOfButtons;
    private Ellipse _pivotEllipse;
    public Rectangle[] _rectanglesA;
    public Rectangle[] _rectanglesB;
    private const double _rectangleWidthHeight = 12;

    private Line[] _lines;

    private const double radiusOfButtons = 0.75;

    private int? _indexOfDraggedButton;

    /// <summary>
    /// Occurs when at least one of the hue values of the color circle has changed. Argument is the list of hue values of the circle, with the first item
    /// always representing the main hue value.
    /// </summary>
    public event Action<IReadOnlyList<double>> HueValuesChanged;

    public ColorCircleSurfaceControl()
    {
      InitializeComponent();
      _guiImage.Source = GetBitmap();

      _colorCircleModel = new ColorCircleModelRectangle();

      InitializeHueButtons();
    }

    public IColorCircleModel ColorCircleModel
    {
      get
      {
        return _colorCircleModel;
      }
      set
      {
        if (value is null)
          throw new ArgumentNullException(nameof(value));
        if (_colorCircleModel.GetType() != value.GetType())
        {
          _colorCircleModel = value;
          InitializeHueButtons();
          SetButtonPositions();
        }
      }
    }

    /// <summary>
    /// Gets the current hue values. The element at index 0 is always the main hue value.
    /// </summary>
    /// <value>
    /// The hue values.
    /// </value>
    public IReadOnlyList<double> HueValues
    {
      get
      {
        return _hueOfButtons;
      }
    }

    public void SetHueBaseValue(double hueBaseValue, bool silentSet)
    {
      var diff = hueBaseValue - _hueOfButtons[0];
      for (int i = 0; i < _hueOfButtons.Length; ++i)
        _hueOfButtons[i] = BringInbetween0To1(_hueOfButtons[i] + diff);

      SetButtonPositions();

      if (!silentSet)
        HueValuesChanged?.Invoke(_hueOfButtons);
    }

    protected static double BringInbetween0To1(double x)
    {
      if (x < 0)
      {
        while (x < 0)
          x += 1;
      }
      else if (x > 1)
      {
        while (x > 1)
          x -= 1;
      }
      return x;
    }

    private void InitializeHueButtons()
    {
      var numberOfButtons = _colorCircleModel.NumberOfHueValues;

      var hueOfButton0 = _hueOfButtons is null ? 0 : _hueOfButtons[0];

      _hueOfButtons = new double[numberOfButtons];
      _posOfButtons = new Point[numberOfButtons];
      _rectanglesA = new Rectangle[numberOfButtons];
      _rectanglesB = new Rectangle[numberOfButtons];
      _lines = new Line[numberOfButtons];

      // set initial phi
      _colorCircleModel.SetInitialHueValues(_hueOfButtons);

      _guiCanvas.Children.Clear();

      CreateLines(); // lines to create first, because they must lie _under_ the buttons
      CreateButtons();
      SetHueBaseValue(hueOfButton0, false);
    }

    private void CreateButtons()
    {
      var strokeDash = new DoubleCollection(new double[] { 4, 4 });

      _pivotEllipse = new Ellipse
      {
        Stroke = Brushes.Black,
        StrokeThickness = 1,
        Width = _rectangleWidthHeight * 1.5,
        Height = _rectangleWidthHeight * 1.5,
        Fill = Brushes.Transparent
      };
      _pivotEllipse.PreviewMouseDown += EhRectangle_MouseDown;
      _guiCanvas.Children.Add(_pivotEllipse);

      int numberOfButtons = _colorCircleModel.NumberOfHueValues;

      for (int i = 0; i < numberOfButtons; ++i)
      {
        var ra = new Rectangle()
        {
          Stroke = Brushes.Black,
          StrokeDashArray = strokeDash,
          StrokeDashOffset = 0,
          StrokeThickness = 1,
          Width = _rectangleWidthHeight,
          Height = _rectangleWidthHeight,
          Fill = Brushes.Transparent
        };

        var rb = new Rectangle()
        {
          Stroke = Brushes.White,
          StrokeDashArray = strokeDash,
          StrokeDashOffset = 4,
          StrokeThickness = 1,
          Width = _rectangleWidthHeight,
          Height = _rectangleWidthHeight,
          Fill = Brushes.Transparent
        };

        ra.PreviewMouseDown += EhRectangle_MouseDown;
        rb.PreviewMouseDown += EhRectangle_MouseDown;

        _rectanglesA[i] = ra;
        _rectanglesB[i] = rb;
        _guiCanvas.Children.Add(ra);
        _guiCanvas.Children.Add(rb);
      }
    }

    private void CreateLines()
    {
      int numberOfButtons = _colorCircleModel.NumberOfHueValues;

      for (int i = 0; i < numberOfButtons; ++i)
      {
        var line = new Line()
        {
          Stroke = Brushes.Black,
          StrokeThickness = 2
        };
        _lines[i] = line;
        _guiCanvas.Children.Add(line);
      }
    }

    private void EhCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
    {
      if (e.NewSize.Width > 0 && e.NewSize.Height > 0)
      {
        SetButtonPositions();
      }
    }

    private void SetButtonPositions()
    {
      var LX = _guiCanvas.ActualWidth;
      var LY = _guiCanvas.ActualHeight;

      var SX2 = 0.5 * _rectanglesA[0].ActualWidth;
      var SY2 = 0.5 * _rectanglesA[0].ActualHeight;

      int numberOfButtons = _colorCircleModel.NumberOfHueValues;

      for (int i = 0; i < numberOfButtons; ++i)
      {
        var x = 0.5 * LX * (1 + radiusOfButtons * Math.Cos(_hueOfButtons[i] * 2 * Math.PI));
        var y = 0.5 * LY * (1 + radiusOfButtons * Math.Sin(_hueOfButtons[i] * 2 * Math.PI));

        _posOfButtons[i] = new Point(x, y);

        Canvas.SetLeft(_rectanglesA[i], x - 0.5 * _rectanglesA[i].Width);
        Canvas.SetTop(_rectanglesA[i], y - 0.5 * _rectanglesA[i].Height);
        Canvas.SetLeft(_rectanglesB[i], x - 0.5 * _rectanglesB[i].Width);
        Canvas.SetTop(_rectanglesB[i], y - 0.5 * _rectanglesB[i].Height);

        if (i == 0)
        {
          Canvas.SetLeft(_pivotEllipse, x - 0.5 * _pivotEllipse.Width);
          Canvas.SetTop(_pivotEllipse, y - 0.5 * _pivotEllipse.Height);
        }
      }

      // draw lines
      for (int i = 0; i < numberOfButtons; ++i)
      {
        int iprev = i == 0 ? numberOfButtons - 1 : i - 1;

        var l = _lines[i];
        l.X1 = _posOfButtons[iprev].X;
        l.Y1 = _posOfButtons[iprev].Y;
        l.X2 = _posOfButtons[i].X;
        l.Y2 = _posOfButtons[i].Y;
      }
    }

    public static BitmapSource GetBitmap()
    {
      const int widthheight = 256;
      const float innerRadiusRatio2 = 0.25f;

      float mid = (widthheight - 1) / 2f;
      float r2max = mid * mid;
      float r2min = r2max * innerRadiusRatio2;

      var wbitmap = new WriteableBitmap(widthheight, widthheight, 96, 96, PixelFormats.Bgra32, null);
      byte[] pixels = new byte[widthheight * widthheight * 4];

      // The bitmap is already by default transparent
      for (int row = 0; row < widthheight; ++row)
      {
        for (int col = 0; col < widthheight; ++col)
        {
          var dx = col - mid;
          var dy = row - mid;
          var r2 = dx * dx + dy * dy;
          if (r2 < r2min || r2 > r2max)
            continue;

          int idx = (row * widthheight + col) * 4;

          // calculate angle
          var phi = (Math.Atan2(dy, dx) / (2 * Math.PI));
          if (phi < 0)
            phi += 1;
          // calculate color
          AxoColor.FromHue((float)phi, out var red, out var green, out var blue);

          pixels[idx + 0] = blue;  // B
          pixels[idx + 1] = green; // G
          pixels[idx + 2] = red; // R
          pixels[idx + 3] = 255;  // A
        }
      }

      // Update writeable bitmap with the colorArray to the image.
      var rect = new Int32Rect(0, 0, widthheight, widthheight);
      int stride = 4 * widthheight;
      wbitmap.WritePixels(rect, pixels, stride, 0);

      return wbitmap;
    }

    private void EhRectangle_MouseDown(object sender, MouseButtonEventArgs e)
    {
      if (e.LeftButton == MouseButtonState.Pressed)
      {
        _indexOfDraggedButton = Array.IndexOf(_rectanglesA, sender);
        if (-1 == _indexOfDraggedButton.Value)
          _indexOfDraggedButton = Array.IndexOf(_rectanglesB, sender);
        if (-1 == _indexOfDraggedButton.Value)
          _indexOfDraggedButton = null;

        if (_indexOfDraggedButton.HasValue)
          _guiCanvas.CaptureMouse();

        e.Handled = true;
      }
    }

    private void EhRectangle_MouseUp(object sender, MouseButtonEventArgs e)
    {
      if (e.LeftButton == MouseButtonState.Released)
      {
        _indexOfDraggedButton = null;
        _guiCanvas.ReleaseMouseCapture();
      }
    }

    private void EhRectangle_MouseMove(object sender, MouseEventArgs e)
    {
      if (_indexOfDraggedButton.HasValue)
      {
        // calculate a new phi
        var position = e.GetPosition(_guiCanvas);

        position.X -= 0.5 * _guiCanvas.ActualWidth;
        position.Y -= 0.5 * _guiCanvas.ActualHeight;

        var phi = Math.Atan2(position.Y, position.X) / (2 * Math.PI);

        _colorCircleModel.TrySetHueOfButton(_indexOfDraggedButton.Value, phi, _hueOfButtons);

        SetButtonPositions();

        HueValuesChanged?.Invoke(_hueOfButtons);
      }
    }
  }
}
