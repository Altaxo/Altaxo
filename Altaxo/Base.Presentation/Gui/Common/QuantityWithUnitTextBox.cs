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
		QuantityWithUnitConverter _converter;


		/// <summary>
		/// Static initialization.
		/// </summary>
		static QuantityWithUnitTextBox()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(QuantityWithUnitTextBox), new FrameworkPropertyMetadata(typeof(QuantityWithUnitTextBox)));
		}

		/// <summary>
		/// Default constructor.
		/// </summary>
		public QuantityWithUnitTextBox()
		{
			var binding = new Binding();
			binding.Source = this;
			binding.Path = new PropertyPath("SelectedQuantity");
			binding.Mode = BindingMode.TwoWay;
			binding.UpdateSourceTrigger = UpdateSourceTrigger.LostFocus;
			_converter = new QuantityWithUnitConverter();
			binding.Converter = _converter;
			binding.ValidationRules.Add(_converter);
			_converter.BindingExpression = this.SetBinding(TextBox.TextProperty, binding);

			this.TextChanged += new TextChangedEventHandler(QuantityWithUnitTextBox_TextChanged);

			this.Loaded += new RoutedEventHandler(EhLoaded);
			this.Unloaded += new RoutedEventHandler(EhUnloaded);
		}

		

		void EhLoaded(object sender, RoutedEventArgs e)
		{
			
		}

		void EhUnloaded(object sender, RoutedEventArgs e)
		{

		}
	

		void QuantityWithUnitTextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
		  _converter.BindingExpression.ValidateWithoutUpdate();
		}

		protected override void OnContextMenuOpening(ContextMenuEventArgs e)
		{
			_converter.OnContextMenuOpening(this, SelectedQuantityProperty);
			base.OnContextMenuOpening(e);
		}

		#region Dependency property

		/// <summary>
		/// Gets/sets the quantity. The quantity consist of a numeric value together with a unit.
		/// </summary>
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

		/// <summary>
		/// Triggers the <see cref="SelectedQuantityChanged"/> event.
		/// </summary>
		/// <param name="obj">Dependency object (here: the control).</param>
		/// <param name="args">Property changed event arguments.</param>
		protected void OnSelectedQuantityChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			if (null != SelectedQuantityChanged)
				SelectedQuantityChanged(obj, args);
		}

		#endregion

		/// <summary>
		/// Sets the unit environment. The unit environment determines the units the user is able to enter.
		/// </summary>
		public QuantityWithUnitGuiEnvironment UnitEnvironment
		{
			get
			{
				return _converter.UnitEnvironment;
			}
			set
			{
				_converter.UnitEnvironment = value;
			}
		}
	}
}
