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
	public class Int32UpDown : NumericUpDownBase
	{
		private const int DefaultMinValue = int.MinValue;
		private const int DefaultValue = 0;
		private const int DefaultMaxValue = int.MaxValue;
		private const int DefaultChange = 1;

		#region Converter

		protected override object GetNewValidationRuleAndConverter()
		{
			return new IntegerUpDownConverter(this);
		}

		protected class IntegerUpDownConverter : ValidationRule, IValueConverter
		{
			Int32UpDown _parent;

			public IntegerUpDownConverter() { }

			public IntegerUpDownConverter(Int32UpDown parent)
			{
				_parent = parent;
			}

			public object Convert(object obj, Type targetType, object parameter, CultureInfo culture)
			{
				int val = (int)obj;

				if (null != _parent)
				{
					if (val == _parent.Minimum && null != _parent.MinimumReplacementText)
						return _parent.MinimumReplacementText;
					if (val == _parent.Maximum && null != _parent.MaximumReplacementText)
						return _parent.MaximumReplacementText;
				}

				return val.ToString();
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

				int result;
				if (int.TryParse(s, out result))
				{
					return result;
				}
				else
				{
					validationResult = new ValidationResult(false, string.Format("The provided string could not be converted to an integer value!"));
					return System.Windows.Data.Binding.DoNothing;
				}
			}
		}

		#endregion

		#region Properties

		#region Value
		public int Value
		{
			get { return (int)GetValue(ValueProperty); }
			set { SetValue(ValueProperty, value); }
		}

		/// <summary>
		/// Identifies the Value dependency property.
		/// </summary>
		public static readonly DependencyProperty ValueProperty =
				DependencyProperty.Register(
						"Value", typeof(int), typeof(Int32UpDown),
						new FrameworkPropertyMetadata(DefaultValue,
								new PropertyChangedCallback(OnValueChanged),
								new CoerceValueCallback(CoerceValue)
						)
				);

		private static void OnValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			Int32UpDown control = (Int32UpDown)obj;

			int oldValue = (int)args.OldValue;
			int newValue = (int)args.NewValue;

			#region Fire Automation events
			IntegerUpDownAutomationPeer peer = UIElementAutomationPeer.FromElement(control) as IntegerUpDownAutomationPeer;
			if (peer != null)
			{
				peer.RaiseValueChangedEvent(oldValue, newValue);
			}
			#endregion

			RoutedPropertyChangedEventArgs<int> e = new RoutedPropertyChangedEventArgs<int>(
					oldValue, newValue, ValueChangedEvent);

			control.OnValueChanged(e);
		}

		/// <summary>
		/// Raises the ValueChanged event.
		/// </summary>
		/// <param name="args">Arguments associated with the ValueChanged event.</param>
		protected virtual void OnValueChanged(RoutedPropertyChangedEventArgs<int> args)
		{
			RaiseEvent(args);
		}

		private static object CoerceValue(DependencyObject element, object value)
		{
			int newValue = (int)value;
			Int32UpDown control = (Int32UpDown)element;

			newValue = Math.Max(control.Minimum, Math.Min(control.Maximum, newValue));

			return newValue;
		}
		#endregion

		#region ValueIfTextIsEmpty
		public int? ValueIfTextIsEmpty
		{
			get { return (int?)GetValue(ValueIfTextIsEmptyProperty); }
			set { SetValue(ValueIfTextIsEmptyProperty, value); }
		}

		/// <summary>
		/// Identifies the Value dependency property.
		/// </summary>
		public static readonly DependencyProperty ValueIfTextIsEmptyProperty =
				DependencyProperty.Register(
						"ValueIfTextIsEmpty", typeof(int?), typeof(Int32UpDown)
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
				DependencyProperty.RegisterAttachedReadOnly("ValueString", typeof(string), typeof(Int32UpDown), new PropertyMetadata());

		public static readonly DependencyProperty ValueStringProperty = ValueStringPropertyKey.DependencyProperty;

		private NumberFormatInfo _numberFormatInfo = new NumberFormatInfo();

		#endregion

		#region Minimum
		public int Minimum
		{
			get { return (int)GetValue(MinimumProperty); }
			set { SetValue(MinimumProperty, value); }
		}

		public static readonly DependencyProperty MinimumProperty =
				DependencyProperty.Register(
						"Minimum", typeof(int), typeof(Int32UpDown),
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
			int minimum = (int)value;
			Int32UpDown control = (Int32UpDown)element;
			return minimum;
		}
		#endregion

		#region Maximum
		public int Maximum
		{
			get { return (int)GetValue(MaximumProperty); }
			set { SetValue(MaximumProperty, value); }
		}

		public static readonly DependencyProperty MaximumProperty =
				DependencyProperty.Register(
						"Maximum", typeof(int), typeof(Int32UpDown),
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
			Int32UpDown control = (Int32UpDown)element;
			int newMaximum = (int)value;
			return Math.Max(newMaximum, control.Minimum);
		}
		#endregion

		#region Change
		public int Change
		{
			get { return (int)GetValue(ChangeProperty); }
			set { SetValue(ChangeProperty, value); }
		}

		public static readonly DependencyProperty ChangeProperty =
				DependencyProperty.Register(
						"Change", typeof(int), typeof(Int32UpDown),
						new FrameworkPropertyMetadata(DefaultChange, new PropertyChangedCallback(OnChangeChanged), new CoerceValueCallback(CoerceChange)),
				new ValidateValueCallback(ValidateChange)
				);

		private static bool ValidateChange(object value)
		{
			int change = (int)value;
			return change > 0;
		}

		private static void OnChangeChanged(DependencyObject element, DependencyPropertyChangedEventArgs args)
		{

		}

		private static object CoerceChange(DependencyObject element, object value)
		{
			int newChange = (int)value;
			Int32UpDown control = (Int32UpDown)element;

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
				typeof(RoutedPropertyChangedEventHandler<int>), typeof(Int32UpDown));

		/// <summary>
		/// Occurs when the Value property changes.
		/// </summary>
		public event RoutedPropertyChangedEventHandler<int> ValueChanged
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
			if (this.Value <= (int.MaxValue - Change))
				this.Value += Change;
			else
				this.Value = int.MaxValue;
		}
		protected override void OnDecrease()
		{
			// avoid an underflow before coerce of the value
			if (this.Value >= (int.MinValue + Change))
				this.Value -= Change;
			else
				this.Value = int.MinValue;
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
			return new IntegerUpDownAutomationPeer(this);
		}
		#endregion
	}

	public class IntegerUpDownAutomationPeer : FrameworkElementAutomationPeer, IRangeValueProvider
	{
		public IntegerUpDownAutomationPeer(Int32UpDown control)
			: base(control)
		{
		}

		protected override string GetClassNameCore()
		{
			return "Int32UpDown";
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

		internal void RaiseValueChangedEvent(int oldValue, int newValue)
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

			int val = (int)value;
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

		private Int32UpDown MyOwner
		{
			get
			{
				return (Int32UpDown)base.Owner;
			}
		}


	}

}