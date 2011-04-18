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

using Altaxo.Science;

namespace Altaxo.Gui.Common.Drawing
{
	public class LengthImageComboBox : EditableImageComboBox
	{
		protected QuantityWithUnitConverter _converter;
		public event DependencyPropertyChangedEventHandler SelectedQuantityChanged;

		static LengthImageComboBox()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(LengthImageComboBox), new FrameworkPropertyMetadata(typeof(LengthImageComboBox)));
	}

		public LengthImageComboBox()
		{
			SetBinding("SelectedQuantity");
		}

		protected void SetBinding(string nameOfValueProperty)
		{
			var binding = new Binding();
			binding.Source = this;
			binding.Path = new PropertyPath(nameOfValueProperty);
			binding.Mode = BindingMode.TwoWay;
			_converter = new QuantityWithUnitConverter();
			binding.Converter = _converter;
			binding.ValidationRules.Add(_converter);
			_converter.BindingExpression = this.SetBinding(ComboBox.TextProperty, binding);

			var dpd = System.ComponentModel.DependencyPropertyDescriptor.FromProperty(ComboBox.TextProperty, typeof(LengthImageComboBox));

			dpd.AddValueChanged(this, QuantityWithUnitTextBox_TextChanged);
		}

		void QuantityWithUnitTextBox_TextChanged(object sender, EventArgs e)
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
			get { return (QuantityWithUnit)GetValue(SelectedQuantityProperty); }
			set { SetValue(SelectedQuantityProperty, value); }
		}

		public static readonly DependencyProperty SelectedQuantityProperty =
				DependencyProperty.Register("SelectedQuantity", typeof(QuantityWithUnit), typeof(LengthImageComboBox),
				new FrameworkPropertyMetadata(new QuantityWithUnit(0,LengthUnitPoint.Instance), EhSelectedQuantityChanged));

		private static void EhSelectedQuantityChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			((LengthImageComboBox)obj).OnSelectedQuantityChanged(obj, args);
		}

		/// <summary>
		/// Triggers the <see cref="SelectedQuantityChanged"/> event.
		/// </summary>
		/// <param name="obj">Dependency object (here: the control).</param>
		/// <param name="args">Property changed event arguments.</param>
		protected virtual void OnSelectedQuantityChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			if (null != SelectedQuantityChanged)
				SelectedQuantityChanged(obj, args);
		}

		#endregion


		public double SelectedQuantityInPoints
		{
			get { return SelectedQuantity.AsValueIn(Altaxo.Science.LengthUnitPoint.Instance); }
			set
			{
				var quant = new Science.QuantityWithUnit(value, Science.LengthUnitPoint.Instance);
				if (null != UnitEnvironment)
					quant = quant.AsQuantityIn(UnitEnvironment.DefaultUnit);
				SelectedQuantity = quant;
			}
		}

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
