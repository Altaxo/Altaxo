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

using Altaxo.Units;
using Altaxo.Units.Length;

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
			this.IsTextSearchEnabled = false; // switch text search off since this interferes with the unit system
		}

		protected void SetBinding(string nameOfValueProperty)
		{
			var binding = new Binding();
			binding.Source = this;
			binding.Path = new PropertyPath(nameOfValueProperty);
			binding.Mode = BindingMode.TwoWay;
			_converter = new QuantityWithUnitConverter(this, SelectedQuantityProperty);
			binding.Converter = _converter;
			binding.ValidationRules.Add(_converter);
			_converter.BindingExpression = this.SetBinding(ComboBox.TextProperty, binding);

			var dpd = System.ComponentModel.DependencyPropertyDescriptor.FromProperty(ComboBox.TextProperty, typeof(LengthImageComboBox));
			dpd.AddValueChanged(this, QuantityWithUnitTextBox_TextChanged);

			var childs = this.LogicalChildren;
		}

		void QuantityWithUnitTextBox_TextChanged(object sender, EventArgs e)
		{
			_converter.BindingExpression.ValidateWithoutUpdate();
		}

		protected override void OnIsKeyboardFocusWithinChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnIsKeyboardFocusWithinChanged(e);

			if (true==(bool)e.OldValue && false==(bool)e.NewValue)
			{
				if (!_converter.BindingExpression.HasError) // if text was successfully interpreted
				{
					_converter.ClearIntermediateConversionResults(); // clear the previous conversion, so that a full new conversion from quantity to string is done when UpdateTarget is called
					_converter.BindingExpression.UpdateTarget(); // update the text with the full quanity including the unit
				}
			}

		}


		protected override void OnKeyDown(System.Windows.Input.KeyEventArgs e)
		{
			if (e.Key == System.Windows.Input.Key.F5) // interpret the text and update the quantity
			{
				e.Handled = true;
				_converter.BindingExpression.UpdateSource(); // interpret the text
				if (!_converter.BindingExpression.HasError) // if text was successfully interpreted
				{
					_converter.ClearIntermediateConversionResults(); // clear the previous conversion, so that a full new conversion from quantity to string is done when UpdateTarget is called
					_converter.BindingExpression.UpdateTarget(); // update the text with the full quanity including the unit
				}
			}

			base.OnKeyDown(e);
		}
		

		protected override void OnContextMenuOpening(ContextMenuEventArgs e)
		{
			_converter.OnContextMenuOpening();
			base.OnContextMenuOpening(e);
		}

		#region Dependency property

		/// <summary>
		/// Gets/sets the quantity. The quantity consist of a numeric value together with a unit.
		/// </summary>
		public DimensionfulQuantity SelectedQuantity
		{
			get { return (DimensionfulQuantity)GetValue(SelectedQuantityProperty); }
			set { SetValue(SelectedQuantityProperty, value); }
		}

		public static readonly DependencyProperty SelectedQuantityProperty =
				DependencyProperty.Register("SelectedQuantity", typeof(DimensionfulQuantity), typeof(LengthImageComboBox),
				new FrameworkPropertyMetadata(new DimensionfulQuantity(0, Units.Length.Point.Instance), EhSelectedQuantityChanged));

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
			get { return SelectedQuantity.AsValueIn(Units.Length.Point.Instance); }
			set
			{
				var quant = new Units.DimensionfulQuantity(value, Units.Length.Point.Instance);
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
