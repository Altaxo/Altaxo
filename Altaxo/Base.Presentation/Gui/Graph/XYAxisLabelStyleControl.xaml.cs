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

namespace Altaxo.Gui.Graph
{
	using Altaxo.Graph.Gdi;

	/// <summary>
	/// Interaction logic for XYAxisLabelStyleControl.xaml
	/// </summary>
	[UserControlForController(typeof(IXYAxisLabelStyleViewEventSink))]
	public partial class XYAxisLabelStyleControl : UserControl, IXYAxisLabelStyleView
	{
		BackgroundControlsGlue _backgroundGlue;
		GdiFontGlue _fontGlue;

		private IXYAxisLabelStyleViewEventSink _controller;

		public XYAxisLabelStyleControl()
		{
			InitializeComponent();
			_backgroundGlue = new BackgroundControlsGlue();
			_backgroundGlue.CbBrush = _cbBackgroundBrush;
			_backgroundGlue.CbStyle = _cbBackgroundStyle;

			_fontGlue = new GdiFontGlue();
			_fontGlue.GuiFontFamily = m_cbFontFamily;
			_fontGlue.GuiFontStyle = m_cbFontStyle;
			_fontGlue.GuiFontSize = m_cbFontSize;
			_fontGlue.SelectedFontChanged += new EventHandler(EhFontGlue_SelectedFontChanged);
		}

		void EhFontGlue_SelectedFontChanged(object sender, EventArgs e)
		{
			if (null != Controller)
			{
				Controller.EhView_FontChanged(_fontGlue.SelectedFont);
			}
		}

		private void EhColor_SelectionChangeCommitted(object sender, DependencyPropertyChangedEventArgs e)
		{
			if (null != Controller)
			{
				Controller.EhView_ColorChanged(m_cbColor.SelectedBrush);
			}
		}

		private void EhFontSize_SelectionChangeCommitted(object sender, DependencyPropertyChangedEventArgs e)
		{
			if (null != Controller)
			{
				Controller.EhView_FontSizeChanged(m_cbFontSize.SelectedFontSize);
			}
		}

		private void _chkAutomaticAlignment_CheckedChanged(object sender, RoutedEventArgs e)
		{
			if (null != Controller)
				Controller.EhView_AutomaticAlignmentChanged(true == this._chkAutomaticAlignment.IsChecked);

			this.m_cbHorizontalAlignment.IsEnabled = false == _chkAutomaticAlignment.IsChecked;
			this.m_cbVerticalAlignment.IsEnabled = false == _chkAutomaticAlignment.IsChecked;

			e.Handled = true;
		}

		private void EhHorizontalAlignment_SelectionChangeCommitted(object sender, SelectionChangedEventArgs e)
		{
			e.Handled = true;
			if (null != Controller)
			{
				string name = (string)this.m_cbHorizontalAlignment.SelectedItem;
				Controller.EhView_HorizontalAlignmentChanged(name);
			}
		}

		private void EhXOffset_Validating(object sender, ValidationEventArgs<string> e)
		{
			if (null != Controller)
			{
				bool bCancel = e.Cancel;
				Controller.EhView_XOffsetValidating(((TextBox)sender).Text, ref bCancel);
				if (bCancel)
					e.AddError("The provided text could not be recognized as valid number");
			}
		}

		private void EhVerticalAlignment_SelectionChangeCommitted(object sender, SelectionChangedEventArgs e)
		{
			e.Handled = true;
			if (null != Controller)
			{
				string name = (string)this.m_cbVerticalAlignment.SelectedItem;
				Controller.EhView_VerticalAlignmentChanged(name);
			}

			e.Handled = true;
		}

		private void EhYOffset_Validating(object sender, ValidationEventArgs<string> e)
		{
			if (null != Controller)
			{
				bool bCancel = e.Cancel;
				Controller.EhView_YOffsetValidating(((TextBox)sender).Text, ref bCancel);
				if (bCancel)
					e.AddError("The provided text could not be recognized as valid number");
			}
		}

		private void m_cbLabelStyle_SelectionChangeCommitted(object sender, SelectionChangedEventArgs e)
		{
			e.Handled = true;
			if (null != Controller)
			{
				Controller.EhView_LabelStyleChanged(this.m_cbLabelStyle.SelectedIndex);
			}

			e.Handled = true;
		}

		public static void InitComboBox(ComboBox box, string[] names, string name)
		{
			box.ItemsSource = names;
			box.SelectedItem = name;
			box.Text = name;
		}

		#region  IXYAxisLabelStyleView

		public IXYAxisLabelStyleViewEventSink Controller
		{
			get
			{
				return _controller;
			}
			set
			{
				_controller = value;
			}
		}

		public void Font_Initialize(System.Drawing.Font font)
		{
			_fontGlue.SelectedFont = font;
		}

		public void Color_Initialize(BrushX color)
		{
			m_cbColor.SelectedBrush = color;
		}

		public new Altaxo.Graph.Gdi.Background.IBackgroundStyle Background
		{
			get
			{
				return _backgroundGlue.BackgroundStyle;
			}
			set
			{
				_backgroundGlue.BackgroundStyle = value;
			}
		}

		public void FontSize_Initialize(double val)
		{
			_fontGlue.FontSize = val;
		}

		public void HorizontalAlignment_Initialize(string[] names, string name)
		{
			InitComboBox(m_cbHorizontalAlignment, names, name);
		}

		public void VerticalAlignment_Initialize(string[] names, string name)
		{
			InitComboBox(m_cbVerticalAlignment, names, name);
		}

		public void AutomaticAlignment_Initialize(bool value)
		{
			_chkAutomaticAlignment.IsChecked = value;
			this.m_cbHorizontalAlignment.IsEnabled = false == _chkAutomaticAlignment.IsChecked;
			this.m_cbVerticalAlignment.IsEnabled = false == _chkAutomaticAlignment.IsChecked;
		}

		public double Rotation
		{
			get
			{
				return this.m_edRotation.SelectedRotation;
			}
			set
			{
				this.m_edRotation.SelectedRotation = value;
			}
		}

		public void XOffset_Initialize(string text)
		{
			this.m_edXOffset.Text = text;
		}

		public void YOffset_Initialize(string text)
		{
			this.m_edYOffset.Text = text;
		}

		public void LabelStyle_Initialize(string[] names, string name)
		{
			InitComboBox(this.m_cbLabelStyle, names, name);
		}

		public string SuppressedLabelsByValue
		{
			get { return _edSuppressLabelValues.Text; }
			set { _edSuppressLabelValues.Text = value; }

		}

		public string SuppressedLabelsByNumber
		{
			get { return _edSuppressLabelsByNumber.Text; }
			set { _edSuppressLabelsByNumber.Text = value; }
		}


		#endregion  IXYAxisLabelStyleView

		public EventHandler EhSelectedFontChanged { get; set; }
	}
}
