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

using Altaxo.Gui.Common.Drawing;

namespace Altaxo.Gui.Graph
{
	/// <summary>
	/// Interaction logic for XYPlotLabelStyleControl.xaml
	/// </summary>
	[UserControlForController(typeof(IXYPlotLabelStyleViewEventSink))]
	public partial class XYPlotLabelStyleControl : UserControl, IXYPlotLabelStyleView
	{
		private IXYPlotLabelStyleViewEventSink _controller;

		FontControlsGlue _fontControlsGlue;
		BackgroundControlsGlue _backgroundGlue;

		public XYPlotLabelStyleControl()
		{
			InitializeComponent();

			_fontControlsGlue = new FontControlsGlue() { CbFontFamily = m_cbFontFamily, CbFontStyle = m_cbFontStyle, CbFontSize = m_cbFontSize };
			_backgroundGlue = new BackgroundControlsGlue() { CbStyle = _cbBackgroundStyle, CbBrush = _cbBackgroundBrush };

		}

		private void EhSelectLabelColumn_Click(object sender, RoutedEventArgs e)
		{
			if (null != Controller)
				Controller.EhView_SelectLabelColumn();
		}

		private void EhIndependentColor_CheckChanged(object sender, RoutedEventArgs e)
		{
			if (null != Controller)
			{
				Controller.EhView_IndependentColorChanged(true==m_chkIndependentColor.IsChecked);
				this.m_cbColor.IsEnabled = true==m_chkIndependentColor.IsChecked;
			}
		}

		private void EhAttachToAxis_CheckedChanged(object sender, RoutedEventArgs e)
		{
			if (null != Controller)
			{
				Controller.EhView_AttachToAxisChanged(true==m_chkAttachToAxis.IsChecked);
				this.m_cbAttachedAxis.IsEnabled = true==m_chkAttachToAxis.IsChecked;
			}
		}

		private void EhColor_SelectionChangeCommitted(object sender, DependencyPropertyChangedEventArgs e)
		{
			if (null != Controller)
			{
				Controller.EhView_ColorChanged(m_cbColor.SelectedGdiColor);
			}
		}

		private void EhHorizontalAlignment_SelectionChangeCommitted(object sender, SelectionChangedEventArgs e)
		{
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
			if (null != Controller)
			{
				string name = (string)this.m_cbVerticalAlignment.SelectedItem;
				Controller.EhView_VerticalAlignmentChanged(name);
			}
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

		#region IXYPlotLabelStyleView

		public IXYPlotLabelStyleViewEventSink Controller
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

		public void LabelColumn_Initialize(string labelColumnAsText)
		{
			this._edLabelColumn.Text = labelColumnAsText;
		}

		public void Font_Initialize(System.Drawing.Font font)
		{
			_fontControlsGlue.Font = font;
		}

		public void Color_Initialize(System.Drawing.Color color)
		{
			this.m_cbColor.SelectedGdiColor = color;
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

		public void HorizontalAlignment_Initialize(string[] names, string name)
		{
			m_cbHorizontalAlignment.ItemsSource = names;
			m_cbHorizontalAlignment.SelectedItem = name;

		}

		public void VerticalAlignment_Initialize(string[] names, string name)
		{
			m_cbVerticalAlignment.ItemsSource = names;
			m_cbVerticalAlignment.SelectedItem = name;
		}

		public void AttachToAxis_Initialize(bool bAttached)
		{
			this.m_chkAttachToAxis.IsChecked = bAttached;
			this.m_cbAttachedAxis.IsEnabled = !bAttached;     
		}

		public void AttachedAxis_Initialize(List<Collections.ListNode> names, int sel)
		{
			m_cbAttachedAxis.ItemsSource = names;
			m_cbAttachedAxis.SelectedIndex = sel;
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

		public void IndependentColor_Initialize(bool bIndependent)
		{
			this.m_chkIndependentColor.IsChecked = bIndependent;
			this.m_cbColor.IsEnabled = bIndependent;    
		}

		#endregion
	}
}
