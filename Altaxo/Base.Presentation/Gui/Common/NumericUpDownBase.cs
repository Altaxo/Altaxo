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
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Globalization;
using System.Diagnostics;

using System.Windows.Data;

namespace Altaxo.Gui.Common
{

	/// <summary>
	/// Base class for numeric up-down controls. As a base class, this class is independent on the type of number. Is is presumed, that derived classes implement a property
	/// named 'Value', which is the numeric value represented by this control.
	/// </summary>
	public abstract class NumericUpDownBase : Control
	{
		/// <summary>
		/// An instance of <see cref=" ValidationRule"/> , that implements additionally <see cref=" IValueConverter"/> to convert and validate from and to text.
		/// </summary>
		protected object _validationRuleAndConverter;

		#region Construction

		/// <summary>
		/// Static initialization.
		/// </summary>
		static NumericUpDownBase()
		{
			InitializeCommands();

			// Listen to MouseLeftButtonDown event to determine if slide should move focus to itself
			EventManager.RegisterClassHandler(typeof(NumericUpDownBase),
					Mouse.MouseDownEvent, new MouseButtonEventHandler(NumericUpDownBase.OnMouseLeftButtonDown), true);

			DefaultStyleKeyProperty.OverrideMetadata(typeof(NumericUpDownBase), new FrameworkPropertyMetadata(typeof(NumericUpDownBase)));
		}

		/// <summary>
		/// Instance constructor.
		/// </summary>
		public NumericUpDownBase()
		{
			_validationRuleAndConverter = GetNewValidationRuleAndConverter();
		}

		/// <summary>
		/// Overrides OnApplyTemplate, to call the function <see cref="SetupBindings"/> after the template was applied.
		/// </summary>
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			SetupBindings();
		}

		/// <summary>
		/// Set up the bindings. As default, a element named '_textBox' is searched. The Text property of this element is bound to the 'Value' property of the control,
		/// using the converter and validation rule stored in <see cref="_validationRuleAndConverter"/>.
		/// </summary>
		protected virtual void SetupBindings()
		{
			var textBox = this.Template.FindName("_textBox", this) as TextBox;
			if (null != textBox)
			{
				var binding = new Binding();
				binding.Source = this;
				binding.Path = new PropertyPath("Value");

				binding.Converter = (IValueConverter)_validationRuleAndConverter;
				binding.ValidationRules.Add((ValidationRule)_validationRuleAndConverter);
				textBox.SetBinding(TextBox.TextProperty, binding);
			}
		}

		/// <summary>
		/// Derived classes must provide a new instance of an object that derives from <see cref="ValidationRule"/> and implements <see cref="IValueConverter"/> here.
		/// </summary>
		/// <returns>A new instance that is able to convert and validate the provided text.</returns>
		protected abstract object GetNewValidationRuleAndConverter();

		#endregion

		#region Properties

		#region IsGotoMinimumAndMaximumVisible

		/// <summary>
		/// If true, two additional buttons are visible, which provide a fast way to go to the minimum and the maximum value.
		/// </summary>
		public bool IsGotoMinimumAndMaximumVisible
		{
			get { return (bool)GetValue(IsGotoMinimumAndMaximumVisibleProperty); }
			set { SetValue(IsGotoMinimumAndMaximumVisibleProperty, value); }
		}

		/// <summary>
		/// The <see cref="DependencyProperty"/> corresponding to <see cref="IsGotoMinimumAndMaximumVisible"/>.
		/// </summary>
		public static readonly DependencyProperty IsGotoMinimumAndMaximumVisibleProperty =
				DependencyProperty.Register(
						"IsGotoMinimumAndMaximumVisible", typeof(bool), typeof(NumericUpDownBase),
						new FrameworkPropertyMetadata(false)
		);

		#endregion

		#region MinimumReplacementText

		/// <summary>
		/// If this property is set (i.e. is not null or empty), the text in the text box is replaced by this text if the Value property is equal to the minimal value.
		/// </summary>
		public string MinimumReplacementText
		{
			get { return (string)GetValue(MinimumReplacementTextProperty); }
			set { SetValue(MinimumReplacementTextProperty, value); }
		}

		/// <summary>
		/// The <see cref="DependencyProperty"/> corresponding to <see cref="MinimumReplacementText"/>.
		/// </summary>
		public static readonly DependencyProperty MinimumReplacementTextProperty =
				DependencyProperty.Register(
						"MinimumReplacementText", typeof(string), typeof(NumericUpDownBase)
		);

		#endregion

		#region MaximumReplacementText

		/// <summary>
		/// If this property is set (i.e. is not null or empty), the text in the text box is replaced by this text if the Value property is equal to the maximum value.
		/// </summary>
		public string MaximumReplacementText
		{
			get { return (string)GetValue(MaximumReplacementTextProperty); }
			set { SetValue(MaximumReplacementTextProperty, value); }
		}

		/// <summary>
		/// The <see cref="DependencyProperty"/> corresponding to <see cref="MaximumReplacementText"/>.
		/// </summary>
		public static readonly DependencyProperty MaximumReplacementTextProperty =
				DependencyProperty.Register(
						"MaximumReplacementText", typeof(string), typeof(NumericUpDownBase)
		);

		#endregion

		#endregion Properties

		#region Commands

		/// <summary>
		/// Command used to increase the value of the control by one change unit.
		/// </summary>
		public static RoutedCommand IncreaseCommand
		{
			get
			{
				return _increaseCommand;
			}
		}

		/// <summary>
		/// Command used to decrease the value of the control by one change unit.
		/// </summary>
		public static RoutedCommand DecreaseCommand
		{
			get
			{
				return _decreaseCommand;
			}
		}

		/// <summary>
		/// Command used to change the value of the control to the minimum value.
		/// </summary>
		public static RoutedCommand GotoMinimumCommand
		{
			get
			{
				return _gotoMinimumCommand;
			}
		}

		/// <summary>
		/// Command used to change the value of the control to the maximum value.
		/// </summary>
		public static RoutedCommand GotoMaximumCommand
		{
			get
			{
				return _gotoMaximumCommand;
			}
		}

		/// <summary>
		/// Defines the commands used by this class.
		/// </summary>
		private static void InitializeCommands()
		{
			_increaseCommand = new RoutedCommand("IncreaseCommand", typeof(NumericUpDownBase));
			CommandManager.RegisterClassCommandBinding(typeof(NumericUpDownBase), new CommandBinding(_increaseCommand, OnIncreaseCommand));
			CommandManager.RegisterClassInputBinding(typeof(NumericUpDownBase), new InputBinding(_increaseCommand, new KeyGesture(Key.Up)));

			_decreaseCommand = new RoutedCommand("DecreaseCommand", typeof(NumericUpDownBase));
			CommandManager.RegisterClassCommandBinding(typeof(NumericUpDownBase), new CommandBinding(_decreaseCommand, OnDecreaseCommand));
			CommandManager.RegisterClassInputBinding(typeof(NumericUpDownBase), new InputBinding(_decreaseCommand, new KeyGesture(Key.Down)));

			_gotoMinimumCommand = new RoutedCommand("GotoMinimumCommand", typeof(NumericUpDownBase));
			CommandManager.RegisterClassCommandBinding(typeof(NumericUpDownBase), new CommandBinding(_gotoMinimumCommand, OnGotoMinimumCommand));
			CommandManager.RegisterClassInputBinding(typeof(NumericUpDownBase), new InputBinding(_gotoMinimumCommand, new KeyGesture(Key.Home)));

			_gotoMaximumCommand = new RoutedCommand("GotoMaximumCommand", typeof(NumericUpDownBase));
			CommandManager.RegisterClassCommandBinding(typeof(NumericUpDownBase), new CommandBinding(_gotoMaximumCommand, OnGotoMaximumCommand));
			CommandManager.RegisterClassInputBinding(typeof(NumericUpDownBase), new InputBinding(_gotoMaximumCommand, new KeyGesture(Key.End)));
		}

		/// <summary>
		/// Static handler that is called on <see cref="IncreaseCommand"/>. This handler calls the instance function <see cref="OnIncrease"/>, which must be overriden in derived classes.
		/// </summary>
		/// <param name="sender">Sender of the command.</param>
		/// <param name="e">Event args.</param>
		private static void OnIncreaseCommand(object sender, ExecutedRoutedEventArgs e)
		{
			NumericUpDownBase control = sender as NumericUpDownBase;
			if (control != null)
			{
				control.OnIncrease();
			}
		}

		/// <summary>
		/// Static handler that is called on <see cref="DecreaseCommand"/>. This handler calls the instance function <see cref="OnDecrease"/>, which must be overriden in derived classes.
		/// </summary>
		/// <param name="sender">Sender of the command.</param>
		/// <param name="e">Event args.</param>
		private static void OnDecreaseCommand(object sender, ExecutedRoutedEventArgs e)
		{
			NumericUpDownBase control = sender as NumericUpDownBase;
			if (control != null)
			{
				control.OnDecrease();
			}
		}

		/// <summary>
		/// Static handler that is called on <see cref="GotoMinimumCommand"/>. This handler calls the instance function <see cref="OnGotoMinimum"/>, which must be overriden in derived classes.
		/// </summary>
		/// <param name="sender">Sender of the command.</param>
		/// <param name="e">Event args.</param>
		private static void OnGotoMinimumCommand(object sender, ExecutedRoutedEventArgs e)
		{
			NumericUpDownBase control = sender as NumericUpDownBase;
			if (control != null)
			{
				control.OnGotoMinimum();
			}
		}

		/// <summary>
		/// Static handler that is called on <see cref="GotoMaximumCommand"/>. This handler calls the instance function <see cref="OnGotoMaximum"/>, which must be overriden in derived classes.
		/// </summary>
		/// <param name="sender">Sender of the command.</param>
		/// <param name="e">Event args.</param>
		private static void OnGotoMaximumCommand(object sender, ExecutedRoutedEventArgs e)
		{
			NumericUpDownBase control = sender as NumericUpDownBase;
			if (control != null)
			{
				control.OnGotoMaximum();
			}
		}

		/// <summary>
		/// Derived classes should increase the property 'Value' by one change unit here.
		/// </summary>
		protected abstract void OnIncrease();

		/// <summary>
		/// Derived classes should decrease the property 'Value' by one change unit here.
		/// </summary>
		protected abstract void OnDecrease();

		/// <summary>
		/// Derived classes should change the property 'Value' to the minimum value.
		/// </summary>
		protected abstract void OnGotoMinimum();

		/// <summary>
		/// Derived classes should change the property 'Value' to the maximum value.
		/// </summary>	
		protected abstract void OnGotoMaximum();

		/// <summary>Stores the <see cref="IncreaseCommand"/>.</summary>
		private static RoutedCommand _increaseCommand;
		/// <summary>Stores the <see cref="DecreaseCommand"/>.</summary>
		private static RoutedCommand _decreaseCommand;
		/// <summary>Stores the <see cref="GotoMinimumCommand"/>.</summary>
		private static RoutedCommand _gotoMinimumCommand;
		/// <summary>Stores the <see cref="GotoMaximumCommand"/>.</summary>
		private static RoutedCommand _gotoMaximumCommand;
		#endregion

		#region Mouse handling

		/// <summary>
		/// Class handler for MouseLeftButtonDown event.
		/// The purpose of this handle is to move input focus to NumericUpDownBase when user pressed
		/// mouse left button on any part of slider that is not focusable.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">Mouse event args.</param>
		private static void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			NumericUpDownBase control = (NumericUpDownBase)sender;

			// When someone click on a part in the NumericUpDownBase and it's not focusable
			// NumericUpDownBase needs to take the focus in order to process keyboard correctly
			if (!control.IsKeyboardFocusWithin)
			{
				e.Handled = control.Focus() || e.Handled;
			}
		}

		#endregion
	}
}
