using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

using Altaxo.Science;


namespace Altaxo.Gui.Common
{
	/// <summary>
	/// Converts a <see cref="QuantityWithUnit"/> to a string and vice versa. Additionaly, this class can act as validation rule for the strings entered.
	/// </summary>
	public class QuantityWithUnitConverter : ValidationRule, IValueConverter
	{
		QuantityWithUnitGuiEnvironment _unitEnvironment;
		BindingExpressionBase _binding;
	

		QuantityWithUnit? _lastConvertedQuantity;
		string _lastConvertedString;

		/// <summary>Used for the context menu helpers only.</summary>
		FrameworkElement _parent;
		/// <summary>Used for the context menu helpers only.</summary>
		DependencyProperty _quantityGetSetProperty;

		/// <summary>
		/// Empty constructor. You should set as soon as possible the <see cref="BindingExpression"/> to the binding expression between your QuantityWithUnit property and the
		/// Text property of your TextBox, ComboBox, or other Gui element. Furthermore, the <see cref="UnitEnvironment"/> property should be set
		/// to a valid unit environment.
		/// </summary>
		public QuantityWithUnitConverter()
		{
		}


	

		/// <summary>
		/// Get/sets the unit environment used for this converter. It determines, which units are allowed to be entered, which is the default unit, and how many
		/// decimal digits are shown in the box by default.
		/// </summary>
		public QuantityWithUnitGuiEnvironment UnitEnvironment
		{
			get
			{
				return _unitEnvironment;
			}
			set
			{
				if (null == value)
					throw new ArgumentNullException();
				_unitEnvironment = value;
			}
		}

		/// <summary>
		/// Sets the binding expression between the property with gets/sets a QuantityWithUnit and the
		/// Text property of your TextBox, ComboBox, or other Gui element.
		/// </summary>
		public BindingExpressionBase BindingExpression
		{
			get
			{
				return _binding;
			}
			set
			{
				_binding = value;
			}
		}

	

		/// <summary>
		/// Converts from a <see cref="QuantityWithUnit"/> to a string.
		/// </summary>
		/// <param name="value">The value to convert. Has to be a <see cref="QuantityWithUnit"/>.</param>
		/// <param name="targetType">Ignored.</param>
		/// <param name="parameter">Ignored.</param>
		/// <param name="culture">The culture used to convert the quantity.</param>
		/// <returns></returns>
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			var q = (QuantityWithUnit)value;

			if (null != _lastConvertedQuantity && q.IsEqualInValuePrefixUnit(((QuantityWithUnit)_lastConvertedQuantity)))
			{
				return _lastConvertedString;
			}
			else
			{

				string result;

				if (null != _unitEnvironment)
					result = q.Value.ToString("G" + _unitEnvironment.NumberOfDisplayedDigits.ToString(), culture);
				else
					result = q.Value.ToString(culture);

				string end = q.Prefix.ShortCut + q.Unit.ShortCut;
				if (!string.IsNullOrEmpty(end))
					result += " " + end;

				_lastConvertedQuantity = q;
				_lastConvertedString = result;
				return result;
			}
		}

		/// <summary>
		/// Converts a string to a <see cref="QuantityWithUnit"/>.
		/// </summary>
		/// <param name="value">String to convert.</param>
		/// <param name="targetType">Ignored.</param>
		/// <param name="parameter">Ignored.</param>
		/// <param name="culture">The culture used to convert the string..</param>
		/// <returns>The converted quantity.</returns>
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			QuantityWithUnit q;
			string s = (string)value;
			var result = ConvertValidate(s, culture, out q);
			if (result.IsValid)
			{
				_lastConvertedString = s;
				_lastConvertedQuantity = q;
			}
			return q;
		}

		/// <summary>
		/// Validates a string, whether or not it can be converted to a <see cref="QuantityWithUnit"/>.
		/// </summary>
		/// <param name="value">String value to validate.</param>
		/// <param name="cultureInfo">The culture used to convert the string.</param>
		/// <returns>A validation result depending on the result of the validation.</returns>
		public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
		{
			QuantityWithUnit q;
			string s = (string)value;
			var result = ConvertValidate(s, cultureInfo, out q);
			if (result.IsValid)
			{
				_lastConvertedString = s;
				_lastConvertedQuantity = q;
			}
			return result;
		}

		private ValidationResult ConvertValidate(string s, System.Globalization.CultureInfo cultureInfo, out QuantityWithUnit result)
		{
			if (null != _lastConvertedQuantity && s == _lastConvertedString)
			{
				result = (QuantityWithUnit)_lastConvertedQuantity;
				return ValidationResult.ValidResult;
			}

			result = new QuantityWithUnit();
			double parsedValue;
			SIPrefix prefix = null;
			s = s.Trim();

			if (string.IsNullOrEmpty(s))
				return new ValidationResult(false, "The text box is empty. You have to enter a valid numeric quantity");


			foreach (IUnit u in _unitEnvironment.UnitsSortedByShortcutLength)
			{
				if (!s.EndsWith(u.ShortCut))
					continue;

				s = s.Substring(0, s.Length - u.ShortCut.Length);

				if (!u.Prefixes.ContainsNonePrefixOnly && s.Length > 0)
				{
					// try first prefixes of bigger length, then of smaller length
					for (int pl = SIPrefix.MaxShortCutLength; pl > 0; --pl)
					{
						if (s.Length < pl)
							continue;
						prefix = SIPrefix.TryGetPrefixFromShortcut(s.Substring(s.Length - pl));
						if (null != prefix)
						{
							s = s.Substring(0, s.Length - prefix.ShortCut.Length);
							break;
						}
					}
				}

				if (double.TryParse(s, System.Globalization.NumberStyles.Float, cultureInfo, out parsedValue))
				{
					result = new QuantityWithUnit(parsedValue, prefix, u);
					return ValidationResult.ValidResult;
				}
				else
				{
					string firstPart;
					if (null != prefix)
						firstPart = string.Format("The last part \"{0}\" of your entered text is recognized as prefixed unit, ", prefix.ShortCut + u.ShortCut);
					else
						firstPart = string.Format("The last part \"{0}\" of your entered text is recognized as unit, ", u.ShortCut);

					string lastPart;
					if (string.IsNullOrEmpty(s.Trim()))
						lastPart = string.Format("but the first part is empty. You have to prepend a numeric value!");
					else
						lastPart = string.Format("but the first part \"{0}\" is not recognized as a numeric value!", s);

					return new ValidationResult(false, firstPart + lastPart);
				}
			}

			// if nothing is found in this way, we try to split the text
			var parts = s.Split(new char[] { ' ', '\t' }, 2, StringSplitOptions.RemoveEmptyEntries);


			// try to parse the first part as a number
			if (!double.TryParse(parts[0], System.Globalization.NumberStyles.Float, cultureInfo, out parsedValue))
				return new ValidationResult(false, string.Format("The part \"{0}\" of your entered text was not recognized as a numeric value.", parts[0]));

			string unitString = parts.Length >= 2 ? parts[1] : string.Empty;

			if (string.IsNullOrEmpty(unitString))
			{
				if (null != _lastConvertedQuantity)
				{
					result = new QuantityWithUnit(parsedValue, ((QuantityWithUnit)_lastConvertedQuantity).Prefix, ((QuantityWithUnit)_lastConvertedQuantity).Unit);
					return ValidationResult.ValidResult;
				}
				else if (null != _unitEnvironment && null != _unitEnvironment.DefaultUnit)
				{
					result = new QuantityWithUnit(parsedValue, _unitEnvironment.DefaultUnit.Prefix, _unitEnvironment.DefaultUnit.Unit);
					return ValidationResult.ValidResult;
				}
				else
				{
					return new ValidationResult(false, "No unit was given by you and no default unit could be deduced from the environment!");
				}
			}
			else
			{
				return new ValidationResult(false, GetErrorStringForUnrecognizedUnit(unitString));
			}
		}

		string GetErrorStringForUnrecognizedUnit(string unrecognizedPart)
		{
			StringBuilder stb = new StringBuilder();
			stb.AppendFormat("The part \"{0}\" of your entered text is not recognized as a valid unit!\n", unrecognizedPart);
			stb.AppendFormat("Valid units are: \n");

			foreach (var u in _unitEnvironment.UnitsSortedByShortcutLength)
			{
				stb.AppendFormat("{0}\t({1}, {2})\n", u.ShortCut, u.Name, u.Prefixes.ContainsNonePrefixOnly ? "without prefix" : "with prefixes possible");
			}
			return stb.ToString();
		}

		#region Helper functions for supporting context menu



		private QuantityWithUnit SelectedQuantity
		{
			get { var result = (QuantityWithUnit)_parent.GetValue(_quantityGetSetProperty); return result; }
			set { _parent.SetValue(_quantityGetSetProperty, value); }
		}

		/// <summary>
		/// Can be called if the context menu of the gui element is about to be opened. Extends the context menu by additional menu items
		/// for unit conversion, and for the setting of the number of decimal places.
		/// </summary>
		/// <param name="gui"></param>
		/// <param name="quantityGetSetProperty"></param>
		public void OnContextMenuOpening(FrameworkElement gui, DependencyProperty quantityGetSetProperty)
		{
			_parent = gui;
			_quantityGetSetProperty = quantityGetSetProperty;

			BindingExpression.UpdateSource();
			MenuItem convertTo = null;
			MenuItem changeUnitTo = null;
			MenuItem setNoOfDigits = null;
			if (gui.ContextMenu != null)
			{
				foreach (var item in gui.ContextMenu.Items)
				{
					if (!(item is FrameworkElement))
						continue;
					var tag = ((FrameworkElement)item).Tag as string;

					if (tag == "TagConvertTo")
						convertTo = item as MenuItem;
					if (tag == "TagChangeUnitTo")
						changeUnitTo = item as MenuItem;
					if (tag == "TagSetDigits")
						setNoOfDigits = item as MenuItem;
				}
			}

			// Clear all previous menu items
			if (null != convertTo)
				convertTo.Items.Clear();
			if (null != changeUnitTo)
				changeUnitTo.Items.Clear();
			if (null != setNoOfDigits)
				setNoOfDigits.Items.Clear();

			// make menues only when there is no validation error
			if (!(bool)gui.GetValue(Validation.HasErrorProperty))
			{
				// count the units multiplied with the possible prefixes to decided whether to show submenues or not
				int count = 0;
				foreach (var u in _unitEnvironment.UnitsSortedByShortcutLength)
					count += u.Prefixes.Count;

				bool makeSubMenusForEachUnit = count > 10;

				if (null != convertTo)
					MakeConvertToSubMenus(convertTo, makeSubMenusForEachUnit);

				if (null != changeUnitTo)
					MakeChangeUnitToSubMenus(changeUnitTo, makeSubMenusForEachUnit);

				if (null != setNoOfDigits)
					MakeSetDigitsSubMenus(setNoOfDigits);
			}
		}

		private void MakeConvertToSubMenus(MenuItem rootMenuItem, bool makeSubMenusForEachUnit)
		{
			if (makeSubMenusForEachUnit)
			{
				foreach (var u in _unitEnvironment.UnitsSortedByShortcutLength)
				{
					if (u.Prefixes.Count == 1)
					{
						var p = u.Prefixes.First();
						string header = string.Format("{0}{1} ({2} {3}{4})", p.Name, u.Name, SelectedQuantity.AsValueIn(p, u), p.ShortCut, u.ShortCut);
						var mi = new MenuItem() { Header = header, Tag = new PrefixedUnit(p, u) };
						mi.Click += EhConvertTo_Click;
						rootMenuItem.Items.Add(mi);
					}
					else
					{

						var main = new MenuItem() { Header = u.Name };
						foreach (var p in u.Prefixes)
						{
							string header = string.Format("{0}{1} ({2} {3}{4})", p.Name, u.Name, SelectedQuantity.AsValueIn(p, u), p.ShortCut, u.ShortCut);
							var mi = new MenuItem() { Header = header, Tag = new PrefixedUnit(p, u) };
							mi.Click += EhConvertTo_Click;
							rootMenuItem.Items.Add(mi);
						}
						rootMenuItem.Items.Add(main);
					}
				}
			}
			else // do not make submenues for each unit
			{
				foreach (var u in _unitEnvironment.UnitsSortedByShortcutLength)
				{
					foreach (var p in u.Prefixes)
					{
						string header = string.Format("{0}{1} ({2} {3}{4})", p.Name, u.Name, SelectedQuantity.AsValueIn(p, u), p.ShortCut, u.ShortCut);
						var mi = new MenuItem() { Header = header, Tag = new PrefixedUnit(p, u) };
						mi.Click += EhConvertTo_Click;
						rootMenuItem.Items.Add(mi);
					}

				}
			}
		}

		private void MakeChangeUnitToSubMenus(MenuItem rootMenuItem, bool makeSubMenusForEachUnit)
		{
			if (makeSubMenusForEachUnit)
			{
				foreach (var u in _unitEnvironment.UnitsSortedByShortcutLength)
				{
					if (u.Prefixes.Count == 1)
					{
						var p = u.Prefixes.First();
						string header = string.Format("{0} {1} ({2} {3}{4})", p.Name, u.Name, SelectedQuantity.Value, p.ShortCut, u.ShortCut);
						var mi = new MenuItem() { Header = header, Tag = new PrefixedUnit(p, u) };
						mi.Click += EhChangeUnitTo_Click;
						rootMenuItem.Items.Add(mi);
					}
					else
					{

						var main = new MenuItem() { Header = u.Name };
						foreach (var p in u.Prefixes)
						{
							string header = string.Format("{0} {1} ({2} {3}{4})", p.Name, u.Name, SelectedQuantity.Value, p.ShortCut, u.ShortCut);
							var mi = new MenuItem() { Header = header, Tag = new PrefixedUnit(p, u) };
							mi.Click += EhChangeUnitTo_Click;
							rootMenuItem.Items.Add(mi);
						}
						rootMenuItem.Items.Add(main);
					}
				}
			}
			else // do not make submenues for each unit
			{
				foreach (var u in _unitEnvironment.UnitsSortedByShortcutLength)
				{
					foreach (var p in u.Prefixes)
					{
						string header = string.Format("{0}{1} ({2} {3}{4})", p.Name, u.Name, SelectedQuantity.Value, p.ShortCut, u.ShortCut);
						var mi = new MenuItem() { Header = header, Tag = new PrefixedUnit(p, u) };
						mi.Click += EhChangeUnitTo_Click;
						rootMenuItem.Items.Add(mi);
					}

				}
			}
		}

		private void MakeSetDigitsSubMenus(MenuItem rootMenuItem)
		{
			for (int i = 3; i <= 15; i++)
			{
				string header = string.Format("{0}", i);
				var mi = new MenuItem() { Header = header, Tag = i, IsCheckable = true, IsChecked = (null != _unitEnvironment && i == _unitEnvironment.NumberOfDisplayedDigits) };
				mi.Click += EhSetDigitsTo_Click;
				rootMenuItem.Items.Add(mi);
			}
		}


		void EhConvertTo_Click(object sender, RoutedEventArgs e)
		{
			var mnu = sender as MenuItem;
			if (null != mnu && mnu.Tag is PrefixedUnit)
			{
				var prefixedUnit = (PrefixedUnit)mnu.Tag;
				var newQuantity = SelectedQuantity.AsQuantityIn(prefixedUnit);
				SelectedQuantity = newQuantity;
				if (null != _unitEnvironment)
					_unitEnvironment.DefaultUnit = prefixedUnit;
			}
		}


		void EhChangeUnitTo_Click(object sender, RoutedEventArgs e)
		{
			var mnu = sender as MenuItem;
			if (null != mnu && mnu.Tag is PrefixedUnit)
			{
				var prefixedUnit = (PrefixedUnit)mnu.Tag;
				var newQuantity = new QuantityWithUnit(SelectedQuantity.Value, prefixedUnit.Prefix, prefixedUnit.Unit);
				SelectedQuantity = newQuantity;
				if (null != _unitEnvironment)
					_unitEnvironment.DefaultUnit = prefixedUnit;
			}
		}

		void EhSetDigitsTo_Click(object sender, RoutedEventArgs e)
		{
			var mnu = sender as MenuItem;
			if (null != mnu && mnu.Tag is int)
			{
				int digits = (int)mnu.Tag;
				if (null != _unitEnvironment)
					_unitEnvironment.NumberOfDisplayedDigits = digits;

				this.EhNumberOfDisplayedDigitsChanged();
			}
		}

		void EhNumberOfDisplayedDigitsChanged()
		{
			if (null != _lastConvertedQuantity && null != _binding && !_binding.HasError)
			{
				var quant = (QuantityWithUnit)_lastConvertedQuantity; // save the current quantity
				_lastConvertedQuantity = null; // set the temporarily stored values to null in order to force a full conversion
				_lastConvertedString = null;
				_binding.UpdateTarget();
			}
		}

		#endregion

	}

}
