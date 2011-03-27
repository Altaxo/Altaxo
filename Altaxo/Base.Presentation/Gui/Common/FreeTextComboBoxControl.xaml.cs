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

namespace Altaxo.Gui.Common
{
	/// <summary>
	/// Interaction logic for FreeTextComboBoxControl.xaml
	/// </summary>
	public partial class FreeTextComboBoxControl : UserControl, IFreeTextChoiceView
	{
		BindingExpressionBase _bindingExpression;

		public FreeTextComboBoxControl()
		{
			InitializeComponent();

			var binding = new Binding();
			binding.Source = this;
			binding.Path = new PropertyPath("ValidatedText");
			var validator = new MyValidationRule(this);
			binding.ValidationRules.Add(validator);
			binding.Mode = BindingMode.TwoWay;
			binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
			_bindingExpression = _cbChoice.SetBinding(ComboBox.TextProperty, binding);
		}

		private void EhSelectionChangeCommitted(object sender, SelectionChangedEventArgs e)
		{
			if (null != SelectionChangeCommitted)
				SelectionChangeCommitted(_cbChoice.SelectedIndex);
		}

		#region Dependency property
		public string ValidatedText
		{
			get { return (string)GetValue(ValidatedTextProperty); }
			set { SetValue(ValidatedTextProperty, value); }
		}

		public static readonly DependencyProperty ValidatedTextProperty =
				DependencyProperty.Register("ValidatedText", typeof(string), typeof(FreeTextComboBoxControl),
				new FrameworkPropertyMetadata(EhValidatedTextChanged));

		private static void EhValidatedTextChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{

		}
		#endregion

		private class MyValidationRule : ValidationRule, IValueConverter
		{
			FreeTextComboBoxControl _parent;
			public MyValidationRule(FreeTextComboBoxControl parent)
			{
				_parent = parent;
			}

			public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
			{
				if(null!=_parent.TextValidating)
				{
					System.ComponentModel.CancelEventArgs cea = new System.ComponentModel.CancelEventArgs();
					_parent.TextValidating((string)value, cea);
					if (cea.Cancel)
						return new ValidationResult(false, "The entered text is not valid");
				}
				return ValidationResult.ValidResult;
			}

			public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
			{
				return value;
			}

			public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
			{
				return value;
			}
		}


		#region IFreeTextChoiceView

		public event Action<int> SelectionChangeCommitted;

		public event Action<string, System.ComponentModel.CancelEventArgs> TextValidating;

		public void SetDescription(string value)
		{
			_lblDescription.Content = value;
		}

		public void SetChoices(string[] values, int initialselection, bool allowFreeText)
		{
			_cbChoice.ItemsSource = null;
			_cbChoice.IsEditable = allowFreeText;
			_cbChoice.ItemsSource = values;
			if (initialselection >= 0 && initialselection < values.Length)
				_cbChoice.SelectedIndex = initialselection;
		}

		#endregion

	
	}
}
