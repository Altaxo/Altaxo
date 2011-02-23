using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Ink;
using System.Windows.Shapes;

namespace Altaxo.Gui.Common.Drawing
{
	public partial class ColorPickerControl : UserControl, IWpfColorView
	{
		private Color _oldColor, _newColor;

		//
		// Initialization

		public ColorPickerControl()
			: this(Colors.Red)
		{
		}

		public ColorPickerControl(Color oldColor)
		{
			_oldColor = oldColor;
			_newColor = _oldColor;
			InitializeComponent();
		}




		public Color SelectedColor
		{
			get { return _newColor; }
			set { _newColor = value; }
		}

		// Completes initialization after all XAML member vars have been initialized.
		protected override void OnInitialized(EventArgs e)
		{
			base.OnInitialized(e);

			UpdateControlValues();
			UpdateControlVisuals();

			colorComb.ColorSelected += new EventHandler<ColorEventArgs>(EhColorCombControl_ColorSelected);
			brightnessSlider.ValueChanged += new RoutedPropertyChangedEventHandler<double>(EhBrightnessSlider_ValueChanged);
			opacitySlider.ValueChanged += new RoutedPropertyChangedEventHandler<double>(EhOpacitySlider_ValueChanged);

			rectangle2.Fill = new System.Windows.Media.SolidColorBrush(_oldColor);
		}

		//
		// Implementation

		bool _notUserInitiated;

		// Updates values of controls when new DA is set (or upon initialization).
		void UpdateControlValues()
		{
			_notUserInitiated = true;
			try
			{
				// Set nominal color on comb.
				//Color nc = m_selectedDA.Color;
				Color nc = _newColor;
				float f = Math.Max(Math.Max(nc.ScR, nc.ScG), nc.ScB);
				if (f < 0.001f) // black
					nc = Color.FromScRgb(1f, 1f, 1f, 1f);
				else
					nc = Color.FromScRgb(1f, nc.ScR / f, nc.ScG / f, nc.ScB / f);
				colorComb.SelectedColor = nc;

				// Set brightness and opacity.
				brightnessSlider.Value = f;
				opacitySlider.Value = _newColor.ScA;

			}
			finally
			{
				_notUserInitiated = false;
			}
		}

		// Updates visual properties of all controls, in response to any change.
		void UpdateControlVisuals()
		{
			Color c = _newColor;

			// Update LGB for brightnessSlider
			Border sb1 = brightnessSlider.Parent as Border;
			LinearGradientBrush lgb1 = sb1.Background as LinearGradientBrush;
			lgb1.GradientStops[1].Color = colorComb.SelectedColor;

			// Update LGB for opacitySlider
			Color c2a = Color.FromScRgb(0f, c.ScR, c.ScG, c.ScB);
			Color c2b = Color.FromScRgb(1f, c.ScR, c.ScG, c.ScB);
			Border sb2 = opacitySlider.Parent as Border;
			LinearGradientBrush lgb2 = sb2.Background as LinearGradientBrush;
			lgb2.GradientStops[0].Color = c2a;
			lgb2.GradientStops[1].Color = c2b;

			rectangle1.Fill = new System.Windows.Media.SolidColorBrush(c);
		}

		//
		// Event Handlers

		void EhColorCombControl_ColorSelected(object sender, ColorEventArgs e)
		{
			if (_notUserInitiated) return;

			float a, f, r, g, b;
			a = (float)opacitySlider.Value;
			f = (float)brightnessSlider.Value;

			Color nc = e.Color;
			r = f * nc.ScR;
			g = f * nc.ScG;
			b = f * nc.ScB;

			_newColor = Color.FromScRgb(a, r, g, b);
			UpdateControlVisuals();
		}

		void EhBrightnessSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (_notUserInitiated) return;

			Color nc = colorComb.SelectedColor;
			float f = (float)e.NewValue;

			float a, r, g, b;
			a = (float)opacitySlider.Value;
			r = f * nc.ScR;
			g = f * nc.ScG;
			b = f * nc.ScB;

			_newColor = Color.FromScRgb(a, r, g, b);
			UpdateControlVisuals();
		}

		void EhOpacitySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (_notUserInitiated) return;

			Color c = _newColor;
			float a = (float)e.NewValue;

			_newColor = Color.FromScRgb(a, c.ScR, c.ScG, c.ScB);
			UpdateControlVisuals();
		}

	}
}