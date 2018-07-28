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

namespace Altaxo.Gui.Drawing.ColorManagement
{
  /// <summary>
  /// Interaction logic for Color1DSurfaceControl.xaml
  /// </summary>
  public partial class Color1DSurfaceControl : UserControl
  {
    /// <summary>The rectangle center position when the left mouse button is going down over the selection rectangle.</summary>
    private Point _initialRectanglePosition;

    /// <summary>The current mouse position when the left mouse button is going down over the selection rectangle.</summary>
    private Point? _initialMousePosition = null;

    /// <summary>Property that describes where the selection rectangle is currently located in y-direction (0: top, 1: bottom)</summary>
    public static readonly DependencyProperty SelectionRectangleRelativePositionProperty;

    /// <summary>
    /// Fired if the relative position of the selection rectangle has changed.
    /// </summary>
    public Action<double> SelectionRectangleRelativePositionChanged;

    static Color1DSurfaceControl()
    {
      SelectionRectangleRelativePositionProperty = DependencyProperty.Register(nameof(SelectionRectangleRelativePosition), typeof(double), typeof(Color1DSurfaceControl), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, EhSelectionRectangleRelativePositionChanged, EhSelectionRectangleRelativePositionCoerce));
    }

    public Color1DSurfaceControl()
    {
      InitializeComponent();

      this.Loaded += EhLoaded;
    }

    private void EhLoaded(object sender, RoutedEventArgs e)
    {
      this.Loaded -= EhLoaded;
      SetRectanglesLeftBottomOnCanvas();
    }

    public void Set1DColorImage(ImageSource imageSource)
    {
      _guiImage.Source = imageSource;
    }

    /// <summary>Gets/sets where the selection rectangle is currently located in y-direction (0: bottom, 1: top)</summary>
    public double SelectionRectangleRelativePosition
    {
      get { return (double)GetValue(SelectionRectangleRelativePositionProperty); }
      set { SetValue(SelectionRectangleRelativePositionProperty, value); }
    }

    protected virtual void OnSelectionRectangleRelativePositionChanged(double relPosition)
    {
      SelectionRectangleRelativePositionChanged?.Invoke(relPosition);
    }

    private static void EhSelectionRectangleRelativePositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((Color1DSurfaceControl)d).SetRectanglesLeftBottomOnCanvas();
      ((Color1DSurfaceControl)d).OnSelectionRectangleRelativePositionChanged((double)e.NewValue);
    }

    private static object EhSelectionRectangleRelativePositionCoerce(DependencyObject d, object baseValue)
    {
      var val = (double)baseValue;
      return Altaxo.Calc.RMath.ClampToInterval(val, 0, 1);
    }

    private void EhRectangle_MouseDown(object sender, MouseButtonEventArgs e)
    {
      if (e.LeftButton == MouseButtonState.Pressed)
      {
        _initialRectanglePosition = new Point(0, this.SelectionRectangleRelativePosition * _guiCanvas.ActualHeight);
        _initialMousePosition = e.GetPosition(_guiCanvas);
        _guiCanvas.CaptureMouse();
        e.Handled = true;
      }
    }

    private void EhCanvas_MouseDown(object sender, MouseButtonEventArgs e)
    {
      // if mouse was pressed on the canvas rather than the rectangle,
      // then move the rectangle under the mouse
      if (e.LeftButton == MouseButtonState.Pressed)
      {
        _initialMousePosition = e.GetPosition(_guiCanvas);
        _initialRectanglePosition = new Point(_initialMousePosition.Value.X, _guiCanvas.ActualHeight - _initialMousePosition.Value.Y);

        SetRectanglePosition(_initialMousePosition.Value);

        _guiCanvas.CaptureMouse();
        e.Handled = true;
      }
    }

    private void EhRectangle_MouseMove(object sender, MouseEventArgs e)
    {
      if (e.LeftButton == MouseButtonState.Pressed && _initialMousePosition.HasValue)
      {
        SetRectanglePosition(e.GetPosition(_guiCanvas));
      }
    }

    private void SetRectanglePosition(Point currentMousePosition)
    {
      var currentRectanglePositionX = (currentMousePosition.X - _initialMousePosition.Value.X) + _initialRectanglePosition.X;
      var currentRectanglePositionY = (-currentMousePosition.Y + _initialMousePosition.Value.Y) + _initialRectanglePosition.Y;

      if (currentRectanglePositionY < 0)
        currentRectanglePositionY = 0;
      else if (currentRectanglePositionY > _guiCanvas.ActualHeight)
        currentRectanglePositionY = _guiCanvas.ActualHeight;

      this.SelectionRectangleRelativePosition = currentRectanglePositionY / _guiCanvas.ActualHeight;
    }

    private void SetRectanglesLeftBottomOnCanvas()
    {
      double currentRectanglePositionY = this.SelectionRectangleRelativePosition * _guiCanvas.ActualHeight;

      Canvas.SetLeft(_guiSelectionRectangle1, 0.5 * _guiCanvas.ActualWidth - 0.5 * _guiSelectionRectangle1.ActualWidth);
      Canvas.SetBottom(_guiSelectionRectangle1, currentRectanglePositionY - 0.5 * _guiSelectionRectangle1.ActualHeight);
      Canvas.SetLeft(_guiSelectionRectangle2, 0.5 * _guiCanvas.ActualWidth - 0.5 * _guiSelectionRectangle2.ActualWidth);
      Canvas.SetBottom(_guiSelectionRectangle2, currentRectanglePositionY - 0.5 * _guiSelectionRectangle2.ActualHeight);
    }

    private void EhRectangle_MouseUp(object sender, MouseButtonEventArgs e)
    {
      if (e.LeftButton == MouseButtonState.Released && _initialMousePosition.HasValue)
      {
        _guiCanvas.ReleaseMouseCapture();
        SetRectanglePosition(e.GetPosition(_guiCanvas));
        _initialMousePosition = null;
        e.Handled = true;
      }
    }
  }
}
