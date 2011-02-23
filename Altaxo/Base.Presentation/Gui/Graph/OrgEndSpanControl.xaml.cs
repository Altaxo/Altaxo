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

using Altaxo.Gui;
using Altaxo.Gui.Graph.Scales.Rescaling;

namespace Altaxo.Gui.Graph
{
	/// <summary>
	/// Interaction logic for OrgEndSpanControl.xaml
	/// </summary>
	[UserControlForController(typeof(IOrgEndSpanViewEventReceiver))]
	public partial class OrgEndSpanControl : UserControl, IOrgEndSpanView
	{
		IOrgEndSpanViewEventReceiver _controller;

		public OrgEndSpanControl()
		{
			InitializeComponent();
		}

		static void SetChoice(ComboBox cb, string[] choices, int selected)
		{
			cb.ItemsSource = choices;
			cb.SelectedIndex = selected;
		}

		private void cbCombo1_SelectionChangeCommitted(object sender, SelectionChangedEventArgs e)
		{
			e.Handled = true;
			if (_controller != null)
				_controller.EhChoice1Changed((string)cbCombo1.SelectedItem, cbCombo1.SelectedIndex);
		}

		private void cbCombo2_SelectionChangeCommitted(object sender, SelectionChangedEventArgs e)
		{
			e.Handled = true;
			if (_controller != null)
				_controller.EhChoice2Changed((string)cbCombo2.SelectedItem, cbCombo2.SelectedIndex);
		}

		private void cbCombo3_SelectionChangeCommitted(object sender, SelectionChangedEventArgs e)
		{
			e.Handled = true;
			if (_controller != null)
				_controller.EhChoice3Changed((string)cbCombo3.SelectedItem, cbCombo3.SelectedIndex);
		}

		private void edText1_Validating(object sender, ValidationEventArgs<string> e)
		{
			if (_controller != null)
				if (_controller.EhValue1Changed(this.edText1.Text))
					e.AddError("Provided text is not valid");
		}

		private void edText2_Validating(object sender, ValidationEventArgs<string> e)
		{
			if (_controller != null)
				if (_controller.EhValue2Changed(this.edText2.Text))
					e.AddError("Provided text is not valid");

		}

		private void edText3_Validating(object sender, ValidationEventArgs<string> e)
		{
			if (_controller != null)
				if (_controller.EhValue3Changed(this.edText3.Text))
					e.AddError("Provided text is not valid");
		}

		#region IOrgEndSpanView

		public IOrgEndSpanViewEventReceiver Controller
		{
			get { return _controller; }
			set { _controller = value; }
		}

		public void SetLabel1(string txt)
		{
			this.lblLabel1.Content = txt;
		}

		public void SetLabel2(string txt)
		{
			this.lblLabel2.Content = txt;
		}

		public void SetLabel3(string txt)
		{
			this.lblLabel3.Content = txt;
		}

		public void SetChoice1(string[] choices, int selected)
		{
			SetChoice(cbCombo1, choices, selected);
		}

		public void SetChoice2(string[] choices, int selected)
		{
			SetChoice(cbCombo2, choices, selected);
		}

		public void SetChoice3(string[] choices, int selected)
		{
			SetChoice(cbCombo3, choices, selected);
		}

		public void SetValue1(string txt)
		{
			this.edText1.Text = txt;
		}

		public void SetValue2(string txt)
		{
			this.edText2.Text = txt;
		}

		public void SetValue3(string txt)
		{
			this.edText3.Text = txt;
		}


		public void EnableChoice1(bool enable)
		{
			this.cbCombo1.IsEnabled = enable;

		}

		public void EnableChoice2(bool enable)
		{
			this.cbCombo2.IsEnabled = enable;

		}

		public void EnableChoice3(bool enable)
		{
			this.cbCombo3.IsEnabled = enable;

		}

		public void EnableValue1(bool enable)
		{

			this.edText1.IsEnabled = enable;
		}

		public void EnableValue2(bool enable)
		{

			this.edText2.IsEnabled = enable;
		}

		public void EnableValue3(bool enable)
		{

			this.edText3.IsEnabled = enable;
		}

		#endregion IOrgEndSpanView
	}
}
