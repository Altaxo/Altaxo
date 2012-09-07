#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
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
//    along with this program; if not, write to the Free Software
//    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
//
/////////////////////////////////////////////////////////////////////////////
#endregion

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Automation;
using System.Globalization;
using System.Diagnostics;

using System.Windows.Data;

namespace Altaxo.Gui.Common
{
	public class DoubleUpDown : NumericUpDownBase
	{
		private const double DefaultMinValue = 0;
		private const double DefaultValue = 0;
		private const double DefaultMaxValue = 100;
		private const double DefaultChange = 1;

		#region Converter

		protected override object GetNewValidationRuleAndConverter()
		{
			return new DoubleUpDownConverter(this);
		}

		protected class DoubleUpDownConverter : ValidationRule, IValueConverter
		{
			DoubleUpDown _parent;

			public DoubleUpDownConverter() { }

			public DoubleUpDownConverter(DoubleUpDown parent)
			{
				_parent = parent;
			}

			public object Convert(object obj, Type targetType, object parameter, CultureInfo culture)
			{
				var val = (double)obj;

				if (null != _parent)
				{
					if (val == _parent.Minimum && null != _parent.MinimumReplacementText)
						return _parent.MinimumReplacementText;
					if (val == _parent.Maximum && null != _parent.MaximumReplacementText)
						return _parent.MaximumReplacementText;
				}

				return val.ToString(System.Globalization.CultureInfo.CurrentUICulture);
			}

			public object ConvertBack(object obj, Type targetType, object parameter, CultureInfo culture)
			{
				ValidationResult validationResult;
				return ConvertBack(obj, targetType, parameter, culture, out validationResult);
			}

			public override ValidationResult Validate(object obj, CultureInfo cultureInfo)
			{
				ValidationResult validationResult;
				ConvertBack(obj, null, null, cultureInfo, out validationResult);
				return validationResult;
			}


			public object ConvertBack(object obj, Type targetType, object parameter, CultureInfo culture, out ValidationResult validationResult)
			{
				validationResult = ValidationResult.ValidResult;

				string s = (string)obj;


				if (null != _parent)
				{
					_parent.SetValue(ValueStringPropertyKey, s); // we set the value string property to have the actual text value as a property
					s = s.Trim();

					if (!string.IsNullOrEmpty(_parent.MinimumReplacementText) && _parent.MinimumReplacementText.Trim() == s)
						return _parent.Minimum;
					else if (!string.IsNullOrEmpty(_parent.MaximumReplacementText) && _parent.MaximumReplacementText.Trim() == s)
						return _parent.Maximum;
					else if (string.IsNullOrEmpty(s) && null != _parent.ValueIfTextIsEmpty)
						return _parent.ValueIfTextIsEmpty;
				}

				double result;
				if (double.TryParse(s, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.CurrentUICulture, out result))
				{
					return result;
				}
				else
				{
					validationResult = new ValidationResult(false, string.Format("The provided string could not be converted to an numeric value!"));
					return System.Windows.Data.Binding.DoNothing;
				}
			}
		}

		#endregion

		#region Properties

		#region Value
		public double Value
		{
			get { return (double)GetValue(ValueProperty); }
			set { SetValue(ValueProperty, value); }
		}

		/// <summary>
		/// Identifies the Value dependency property.
		/// </summary>
		public static readonly DependencyProperty ValueProperty =
				DependencyProperty.Register(
						"Value", typeof(double), typeof(DoubleUpDown),
						new FrameworkPropertyMetadata(DefaultValue,
								new PropertyChangedCallback(OnValueChanged),
								new CoerceValueCallback(CoerceValue)
						)
				);

		private static void OnValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			var control = (DoubleUpDown)obj;

			var oldValue = (double)args.OldValue;
			var newValue = (double)args.NewValue;

			#region Fire Automation events
			var peer = UIElementAutomationPeer.FromElement(control) as DoubleUpDownAutomationPeer;
			if (peer != null)
			{
				peer.RaiseValueChangedEvent(oldValue, newValue);
			}
			#endregion

			var e = new RoutedPropertyChangedEventArgs<double>(oldValue, newValue, ValueChangedEvent);
			control.OnValueChanged(e);
		}

		/// <summary>
		/// Raises the ValueChanged event.
		/// </summary>
		/// <param name="args">Arguments associated with the ValueChanged event.</param>
		protected virtual void OnValueChanged(RoutedPropertyChangedEventArgs<double> args)
		{
			RaiseEvent(args);
		}

		private static object CoerceValue(DependencyObject element, object value)
		{
			var newValue = (double)value;
			var control = (DoubleUpDown)element;

			newValue = Math.Max(control.Minimum, Math.Min(control.Maximum, newValue));

			return newValue;
		}
		#endregion

		#region ValueIfTextIsEmpty
		public double? ValueIfTextIsEmpty
		{
			get { return (double?)GetValue(ValueIfTextIsEmptyProperty); }
			set { SetValue(ValueIfTextIsEmptyProperty, value); }
		}

		/// <summary>
		/// Identifies the Value dependency property.
		/// </summary>
		public static readonly DependencyProperty ValueIfTextIsEmptyProperty =
				DependencyProperty.Register(
						"ValueIfTextIsEmpty", typeof(double?), typeof(DoubleUpDown)
		);

		#endregion

		#region ValueString
		public string ValueString
		{
			get
			{
				return (string)GetValue(ValueStringProperty);
			}
		}

		private static readonly DependencyPropertyKey ValueStringPropertyKey =
				DependencyProperty.RegisterAttachedReadOnly("ValueString", typeof(string), typeof(DoubleUpDown), new PropertyMetadata());

		public static readonly DependencyProperty ValueStringProperty = ValueStringPropertyKey.DependencyProperty;

		private NumberFormatInfo _numberFormatInfo = new NumberFormatInfo();

		#endregion

		#region Minimum
		public double Minimum
		{
			get { return (double)GetValue(MinimumProperty); }
			set { SetValue(MinimumProperty, value); }
		}

		public static readonly DependencyProperty MinimumProperty =
				DependencyProperty.Register(
						"Minimum", typeof(double), typeof(DoubleUpDown),
						new FrameworkPropertyMetadata(DefaultMinValue,
								new PropertyChangedCallback(OnMinimumChanged), new CoerceValueCallback(CoerceMinimum)
						)
				);

		private static void OnMinimumChanged(DependencyObject element, DependencyPropertyChangedEventArgs args)
		{
			element.CoerceValue(MaximumProperty);
			element.CoerceValue(ValueProperty);
		}

		private static object CoerceMinimum(DependencyObject element, object value)
		{
			double minimum = (double)value;
			DoubleUpDown control = (DoubleUpDown)element;
			return minimum;
		}
		#endregion

		#region Maximum
		public double Maximum
		{
			get { return (double)GetValue(MaximumProperty); }
			set { SetValue(MaximumProperty, value); }
		}

		public static readonly DependencyProperty MaximumProperty =
				DependencyProperty.Register(
						"Maximum", typeof(double), typeof(DoubleUpDown),
						new FrameworkPropertyMetadata(DefaultMaxValue,
								new PropertyChangedCallback(OnMaximumChanged),
								new CoerceValueCallback(CoerceMaximum)
						)
				);

		private static void OnMaximumChanged(DependencyObject element, DependencyPropertyChangedEventArgs args)
		{
			element.CoerceValue(ValueProperty);
		}

		private static object CoerceMaximum(DependencyObject element, object value)
		{
			DoubleUpDown control = (DoubleUpDown)element;
			double newMaximum = (double)value;
			return Math.Max(newMaximum, control.Minimum);
		}
		#endregion

		#region Change
		public double Change
		{
			get { return (double)GetValue(ChangeProperty); }
			set { SetValue(ChangeProperty, value); }
		}

		public static readonly DependencyProperty ChangeProperty =
				DependencyProperty.Register(
						"Change", typeof(double), typeof(DoubleUpDown),
						new FrameworkPropertyMetadata(DefaultChange, new PropertyChangedCallback(OnChangeChanged), new CoerceValueCallback(CoerceChange)),
				new ValidateValueCallback(ValidateChange)
				);

		private static bool ValidateChange(object value)
		{
			double change = (double)value;
			return change > 0;
		}

		private static void OnChangeChanged(DependencyObject element, DependencyPropertyChangedEventArgs args)
		{

		}

		private static object CoerceChange(DependencyObject element, object value)
		{
			double newChange = (double)value;
			DoubleUpDown control = (DoubleUpDown)element;

			return newChange;
		}

		#endregion

		#endregion

		#region Events
		/// <summary>
		/// Identifies the ValueChanged routed event.
		/// </summary>
		public static readonly RoutedEvent ValueChangedEvent = EventManager.RegisterRoutedEvent(
				"ValueChanged", RoutingStrategy.Bubble,
				typeof(RoutedPropertyChangedEventHandler<double>), typeof(DoubleUpDown));

		/// <summary>
		/// Occurs when the Value property changes.
		/// </summary>
		public event RoutedPropertyChangedEventHandler<double> ValueChanged
		{
			add { AddHandler(ValueChangedEvent, value); }
			remove { RemoveHandler(ValueChangedEvent, value); }
		}
		#endregion

		#region Commands


		protected override void OnIncrease()
		{
			// avoid an overflow before coerce of the value
			var val = this.Value;
			if (this.Value <= (double.MaxValue - Change))
				this.Value += Change;
			else
				this.Value = double.MaxValue;
		}
		protected override void OnDecrease()
		{
			// avoid an underflow before coerce of the value
			if (this.Value >= (double.MinValue + Change))
				this.Value -= Change;
			else
				this.Value = double.MinValue;
		}
		protected override void OnGotoMinimum()
		{
			this.Value = this.Minimum;
		}
		protected override void OnGotoMaximum()
		{
			this.Value = this.Maximum;
		}


		#endregion

		#region Automation

		protected override AutomationPeer OnCreateAutomationPeer()
		{
			return new DoubleUpDownAutomationPeer(this);
		}
		#endregion
	}

	public class DoubleUpDownAutomationPeer : FrameworkElementAutomationPeer, IRangeValueProvider
	{
		public DoubleUpDownAutomationPeer(DoubleUpDown control)
			: base(control)
		{
		}

		protected override string GetClassNameCore()
		{
			return "DoubleUpDown";
		}

		protected override AutomationControlType GetAutomationControlTypeCore()
		{
			return AutomationControlType.Spinner;
		}

		public override object GetPattern(PatternInterface patternInterface)
		{
			if (patternInterface == PatternInterface.RangeValue)
			{
				return this;
			}
			return base.GetPattern(patternInterface);
		}

		internal void RaiseValueChangedEvent(double oldValue, double newValue)
		{
			base.RaisePropertyChangedEvent(RangeValuePatternIdentifiers.ValueProperty,
					(double)oldValue, (double)newValue);
		}

		#region IRangeValueProvider Members

		bool IRangeValueProvider.IsReadOnly
		{
			get
			{
				return !IsEnabled();
			}
		}

		double IRangeValueProvider.LargeChange
		{
			get { return (double)MyOwner.Change; }
		}

		double IRangeValueProvider.Maximum
		{
			get { return (double)MyOwner.Maximum; }
		}

		double IRangeValueProvider.Minimum
		{
			get { return (double)MyOwner.Minimum; }
		}

		void IRangeValueProvider.SetValue(double value)
		{
			if (!IsEnabled())
			{
				throw new ElementNotEnabledException();
			}

			double val = (double)value;
			if (val < MyOwner.Minimum || val > MyOwner.Maximum)
			{
				throw new ArgumentOutOfRangeException("value");
			}

			MyOwner.Value = val;
		}

		double IRangeValueProvider.SmallChange
		{
			get { return (double)MyOwner.Change; }
		}

		double IRangeValueProvider.Value
		{
			get { return (double)MyOwner.Value; }
		}

		#endregion

		private DoubleUpDown MyOwner
		{
			get
			{
				return (DoubleUpDown)base.Owner;
			}
		}


	}

}