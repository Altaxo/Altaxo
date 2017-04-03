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

using Altaxo.Geometry;
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
	/// Interaction logic for Color2DSurfaceControl.xaml
	/// </summary>
	public partial class Color2DSurfaceControl : UserControl
	{
		/// <summary>The rectangle center position when the left mouse button is going down over the selection rectangle.</summary>
		private Point _initialRectanglePosition;

		/// <summary>The current mouse position when the left mouse button is going down over the selection rectangle.</summary>
		private Point? _initialMousePosition = null;

		/// <summary>Property that describes where the selection rectangle is currently located in x-direction (0: right (!), 1: left)</summary>
		public static readonly DependencyProperty SelectionRectangleRelativePositionProperty;

		/// <summary>
		/// Fired if the relative position of the selection rectangle has changed.
		/// </summary>
		public Action<PointD2D> SelectionRectangleRelativePositionChanged;

		static Color2DSurfaceControl()
		{
			SelectionRectangleRelativePositionProperty = DependencyProperty.Register(nameof(SelectionRectangleRelativePosition), typeof(PointD2D), typeof(Color2DSurfaceControl), new FrameworkPropertyMetadata(PointD2D.Empty, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, EhSelectionRectangleRelativePositionChanged, EhSelectionRectangleRelativePositionCoerce));
		}

		public Color2DSurfaceControl()
		{
			InitializeComponent();

			this.Loaded += EhLoaded;
		}

		private void EhLoaded(object sender, RoutedEventArgs e)
		{
			this.Loaded -= EhLoaded;
			SetRectanglesLeftBottomOnCanvas();
		}

		public void Set2DColorImage(ImageSource imageSource)
		{
			_guiImage.Source = imageSource;
		}

		/// <summary>Gets/sets where the selection rectangle is currently located
		/// in x-direction (0: left, 1: right) and
		/// in y-direction (0: bottom, 1: top).</summary>
		public PointD2D SelectionRectangleRelativePosition
		{
			get { return (PointD2D)GetValue(SelectionRectangleRelativePositionProperty); }
			set { SetValue(SelectionRectangleRelativePositionProperty, value); }
		}

		private static void EhSelectionRectangleRelativePositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((Color2DSurfaceControl)d).SetRectanglesLeftBottomOnCanvas();
			((Color2DSurfaceControl)d).OnSelectionRectangleRelativePositionChanged((PointD2D)e.NewValue);
		}

		private static object EhSelectionRectangleRelativePositionCoerce(DependencyObject d, object baseValue)
		{
			var val = (PointD2D)baseValue;

			if (double.IsNaN(val.X) || double.IsNaN(val.Y))
				throw new ArgumentException("Component x or y of value is NaN, therefore Coerce is impossible");

			return new PointD2D(Altaxo.Calc.RMath.ClampToInterval(val.X, 0, 1), Altaxo.Calc.RMath.ClampToInterval(val.Y, 0, 1));
		}

		protected virtual void OnSelectionRectangleRelativePositionChanged(PointD2D relPosition)
		{
			SelectionRectangleRelativePositionChanged?.Invoke(relPosition);
		}

		private void EhRectangle_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed)
			{
				_initialRectanglePosition = new Point(this.SelectionRectangleRelativePosition.X * _guiCanvas.ActualWidth, this.SelectionRectangleRelativePosition.Y * _guiCanvas.ActualHeight);
				_initialMousePosition = e.GetPosition(_guiCanvas);
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

			// Clip rectangle position
			if (currentRectanglePositionX < 0)
				currentRectanglePositionX = 0;
			else if (currentRectanglePositionX > _guiCanvas.ActualWidth)
				currentRectanglePositionX = _guiCanvas.ActualWidth;

			if (currentRectanglePositionY < 0)
				currentRectanglePositionY = 0;
			else if (currentRectanglePositionY > _guiCanvas.ActualHeight)
				currentRectanglePositionY = _guiCanvas.ActualHeight;

			this.SelectionRectangleRelativePosition = new PointD2D(currentRectanglePositionX / _guiCanvas.ActualWidth, currentRectanglePositionY / _guiCanvas.ActualHeight);
		}

		private void SetRectanglesLeftBottomOnCanvas()
		{
			double currentRectanglePositionX = this.SelectionRectangleRelativePosition.X * _guiCanvas.ActualWidth;
			double currentRectanglePositionY = this.SelectionRectangleRelativePosition.Y * _guiCanvas.ActualHeight;

			Canvas.SetLeft(_guiSelectionRectangle1, currentRectanglePositionX - 0.5 * _guiSelectionRectangle1.ActualWidth);
			Canvas.SetBottom(_guiSelectionRectangle1, currentRectanglePositionY - 0.5 * _guiSelectionRectangle1.ActualHeight);
			Canvas.SetLeft(_guiSelectionRectangle2, currentRectanglePositionX - 0.5 * _guiSelectionRectangle2.ActualWidth);
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