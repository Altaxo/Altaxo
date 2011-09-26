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
		public DimensionfulQuantity SelectedQuantity
		{
			get { var result = (DimensionfulQuantity)GetValue(SelectedQuantityProperty); return result; }
			set { SetValue(SelectedQuantityProperty, value); }
		}

		public static readonly DependencyProperty SelectedQuantityProperty =
				DependencyProperty.Register("SelectedQuantity", typeof(DimensionfulQuantity), typeof(QuantityWithUnitTextBox),
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
