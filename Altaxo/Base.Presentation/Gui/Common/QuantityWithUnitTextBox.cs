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
	/// Supports the entering of length values (units: cm, mm, points and so on), and optionally relative units (percent of something).
	/// </summary>
	public class QuantityWithUnitTextBox : TextBox
	{
		public event DependencyPropertyChangedEventHandler SelectedQuantityChanged;

		QuantityWithUnitGuiEnvironment _unitEnvironment = new QuantityWithUnitGuiEnvironment();
		BindingExpressionBase _binding;
		MyConverter _converter;


		/// <summary>
		/// Static initialization.
		/// </summary>
		static QuantityWithUnitTextBox()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(QuantityWithUnitTextBox), new FrameworkPropertyMetadata(typeof(QuantityWithUnitTextBox)));
		}

		public QuantityWithUnitTextBox()
		{
			var binding = new Binding();
			binding.Source = this;
			binding.Path = new PropertyPath("SelectedQuantity");
			binding.Mode = BindingMode.TwoWay;
			binding.UpdateSourceTrigger = UpdateSourceTrigger.LostFocus;
			_converter = new MyConverter(this);
			binding.Converter = _converter;
			binding.ValidationRules.Add(_converter);
			_binding = this.SetBinding(TextBox.TextProperty, binding);

			this.TextChanged += new TextChangedEventHandler(QuantityWithUnitTextBox_TextChanged);
		}

		void QuantityWithUnitTextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			_binding.ValidateWithoutUpdate();
		}

		protected override void OnContextMenuOpening(ContextMenuEventArgs e)
		{
			_binding.UpdateSource();

			MenuItem convertTo = null;
			MenuItem changeUnitTo = null;
			if (ContextMenu != null)
			{
				foreach (var item in ContextMenu.Items)
				{
					if (!(item is FrameworkElement))
						continue;
					var tag = ((FrameworkElement)item).Tag as string;

					if (tag == "TagConvertTo")
						convertTo = item as MenuItem;
					if (tag == "TagChangeUnitTo")
						changeUnitTo = item as MenuItem;
				}
			}

			// Clear all previous menu items
			if (null != convertTo)
				convertTo.Items.Clear();
			if (null != changeUnitTo)
				changeUnitTo.Items.Clear();

			// make menues only when there is no validation error
			if (!(bool)GetValue(Validation.HasErrorProperty))
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
			}

			base.OnContextMenuOpening(e);
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
						var mi = new MenuItem() { Header = header, Tag = u };
						mi.Click += EhConvertTo_Click;
						rootMenuItem.Items.Add(mi);
					}
					else
					{

						var main = new MenuItem() { Header = u.Name };
						foreach (var p in u.Prefixes)
						{
							string header = string.Format("{0}{1} ({2} {3}{4})", p.Name, u.Name, SelectedQuantity.AsValueIn(p, u), p.ShortCut, u.ShortCut);
							var mi = new MenuItem() { Header = header, Tag = u };
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
						var mi = new MenuItem() { Header = header, Tag = u };
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
						var mi = new MenuItem() { Header = header, Tag = u };
						mi.Click += EhConvertTo_Click;
						rootMenuItem.Items.Add(mi);
					}
					else
					{

						var main = new MenuItem() { Header = u.Name };
						foreach (var p in u.Prefixes)
						{
							string header = string.Format("{0} {1} ({2} {3}{4})", p.Name, u.Name, SelectedQuantity.Value, p.ShortCut, u.ShortCut);
							var mi = new MenuItem() { Header = header, Tag = u };
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
						string header = string.Format("{0}{1} ({2} {3}{4})", p.Name, u.Name, SelectedQuantity.Value, p.ShortCut, u.ShortCut);
						var mi = new MenuItem() { Header = header, Tag = u };
						mi.Click += EhConvertTo_Click;
						rootMenuItem.Items.Add(mi);
					}

				}
			}
		}


		void EhConvertTo_Click(object sender, RoutedEventArgs e)
		{
			var mnu = sender as MenuItem;
			var unit = mnu == null ? null : mnu.Tag as IUnit;
			if (null != unit)
			{
				var newQuantity = SelectedQuantity.AsQuantityIn(unit);
				SelectedQuantity = newQuantity;
			}
		}

		#region Dependency property
		public QuantityWithUnit SelectedQuantity
		{
			get { var result = (QuantityWithUnit)GetValue(SelectedQuantityProperty); return result; }
			set { SetValue(SelectedQuantityProperty, value); }
		}

		public static readonly DependencyProperty SelectedQuantityProperty =
				DependencyProperty.Register("SelectedQuantity", typeof(QuantityWithUnit), typeof(QuantityWithUnitTextBox),
				new FrameworkPropertyMetadata(EhSelectedQuantityChanged));

		private static void EhSelectedQuantityChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			((QuantityWithUnitTextBox)obj).OnSelectedQuantityChanged(obj, args);
		}

		protected void OnSelectedQuantityChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			if (null != SelectedQuantityChanged)
				SelectedQuantityChanged(obj, args);
		}

		#endregion

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

		public Collections.SelectableListNodeList ConvertToItems
		{
			get
			{
				var list = new Collections.SelectableListNodeList();
				list.Add(new Collections.SelectableListNode("Bar", null, false));
				list.Add(new Collections.SelectableListNode("boo", null, false));
				return list;
			}
		}

		#region Converter


		class MyConverter : ValidationRule, IValueConverter
		{
			QuantityWithUnitTextBox _parent;

			QuantityWithUnit _lastConvertedQuantity;
			string _lastConvertedString;

			public MyConverter(QuantityWithUnitTextBox parent)
			{
				_parent = parent;
			}

			public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
			{
				var q = (QuantityWithUnit)value;
				string result = q.Value.ToString(culture);
				string end = q.Prefix.ShortCut + q.Unit.ShortCut;
				if (!string.IsNullOrEmpty(end))
					result += " " + end;

				_lastConvertedQuantity = q;
				_lastConvertedString = result;
				return result;
			}

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
				if (s == _lastConvertedString)
				{
					result = _lastConvertedQuantity;
					return ValidationResult.ValidResult;
				}

				result = new QuantityWithUnit();
				double parsedValue;
				SIPrefix prefix = null;
				s = s.Trim();

				if (string.IsNullOrEmpty(s))
					return new ValidationResult(false, "The text box is empty. You have to enter a valid numeric quantity");


				foreach (IUnit u in _parent._unitEnvironment.UnitsSortedByShortcutLength)
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
							prefix = SIPrefix.TryGetPrefixFromShortcut(s.Substring(s.Length-pl));
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
					result = new QuantityWithUnit(parsedValue, _lastConvertedQuantity.Prefix, _lastConvertedQuantity.Unit);
					return ValidationResult.ValidResult;
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

				foreach (var u in _parent.UnitEnvironment.UnitsSortedByShortcutLength)
				{
					stb.AppendFormat("{0}\t({1}, {2})\n", u.ShortCut, u.Name, u.Prefixes.ContainsNonePrefixOnly ? "without prefix" : "with prefixes possible" );
				}
				return stb.ToString();
			}

		}

		#endregion
	}
}
