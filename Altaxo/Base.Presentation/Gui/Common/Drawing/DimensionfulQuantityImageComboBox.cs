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

#endregion Copyright

using Altaxo.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Altaxo.Gui.Common.Drawing
{
	/// <summary>
	/// Base class for a combobox in that the user can input a dimensionful quantity. This class will accept dimensionless quantities, since the default value
	/// for <see cref="SelectedQuantity"/> is registered with a dimensionless quantity. To make the box accept quantities in other units (for instance length),
	/// derive from this class and override the metadata of the <see cref="SelectedQuantityProperty"/> to use a default value in the destination unit.
	/// </summary>
	public class DimensionfulQuantityImageComboBox : EditableImageComboBox
	{
		protected QuantityWithUnitConverter _converter;

		public event DependencyPropertyChangedEventHandler SelectedQuantityChanged;

		static DimensionfulQuantityImageComboBox()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(DimensionfulQuantityImageComboBox), new FrameworkPropertyMetadata(typeof(DimensionfulQuantityImageComboBox)));
		}

		public DimensionfulQuantityImageComboBox()
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

			var dpd = System.ComponentModel.DependencyPropertyDescriptor.FromProperty(ComboBox.TextProperty, typeof(DimensionfulQuantityImageComboBox));
			dpd.AddValueChanged(this, QuantityWithUnitTextBox_TextChanged);

			var childs = this.LogicalChildren;
		}

		private void QuantityWithUnitTextBox_TextChanged(object sender, EventArgs e)
		{
			_converter.BindingExpression.ValidateWithoutUpdate();
		}

		protected override void OnIsKeyboardFocusWithinChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnIsKeyboardFocusWithinChanged(e);

			if (true == (bool)e.OldValue && false == (bool)e.NewValue)
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
				DependencyProperty.Register("SelectedQuantity", typeof(DimensionfulQuantity), typeof(DimensionfulQuantityImageComboBox),
				new FrameworkPropertyMetadata(EhSelectedQuantityChanged));

		private static void EhSelectedQuantityChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			((DimensionfulQuantityImageComboBox)obj).OnSelectedQuantityChanged(obj, args);
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

		#endregion Dependency property

		public double SelectedQuantityInSIUnits
		{
			get { return SelectedQuantity.AsValueInSIUnits; }
			set
			{
				if (null == UnitEnvironment)
					throw new InvalidOperationException("The value can not be set because the unit environment is not initialized yet");

				var quant = new Units.DimensionfulQuantity(value, UnitEnvironment.DefaultUnit.Unit.SIUnit);
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