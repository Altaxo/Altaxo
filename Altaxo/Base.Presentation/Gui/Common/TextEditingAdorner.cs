#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2014 Dr. Dirk Lellinger
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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace Altaxo.Gui.Common
{
	/// <summary>
	/// An adorner class that contains a TextBox to provide editing capability for an <see cref="TextBlockForEditing"/> instance. The editable TextBox resides in the AdornerLayer of the <see cref="TextBlock"/>.
	/// This adorner is intended for temporary construction in the moment when the <see cref="TextBlockForEditing"/> instance goes to edit mode, and subsequent destruction when edit mode is left.
	/// Thats why, it is visible from the beginning and can not be made invisible, unless you remove it from the adorner layer.
	/// Watch the <see cref="EditingFinished"/> to see when editing is finished.
	/// </summary>
	public class TextEditingAdorner : Adorner
	{
		#region Member variables

		/// <summary>The visual children of the adorner.</summary>
		private VisualCollection _visualChildren;

		//
		/// <summary>The TextBox that is used for editing.</summary>
		private TextBox _textBox;

		/// <summary>Extra padding for the text box at the end.</summary>
		private const double _extraWidth = 16;

		/// <summary>
		/// Occurs when editing is finished.
		/// </summary>
		public event EventHandler EditingFinished;

		#endregion Member variables

		/// <summary>
		/// Initializes a new instance of the <see cref="TextEditingAdorner"/> class.
		/// </summary>
		/// <param name="adornedElement">The adorned text block element.</param>
		/// <param name="initialText">The text that should initially be showed.</param>
		/// <param name="textBoxStyle">The style for the text box that is used for editing.</param>
		/// <param name="textBoxValidationRule">The validation rule for the text box that is used for editing.</param>
		public TextEditingAdorner(UIElement adornedElement, string initialText, Style textBoxStyle, ValidationRule textBoxValidationRule)
			: base(adornedElement)
		{
			_visualChildren = new VisualCollection(this);

			EditedText = initialText; // initialize the EditedText property with the actual text , so when our TextBox bounds to it, it is also initialized with the actual text.

			// Build the text box
			_textBox = new TextBox();

			if (textBoxStyle != null)
			{
				_textBox.Style = textBoxStyle; // Apply a style to the TextBox, if a style was provided
			}
			else
			{
				_textBox.Padding = new Thickness(0, 0, _extraWidth, 0);
			}

			// Trick: we bind the text of our TextBox to our own property 'EditedText'. Even if we not really need this property, the binding provides us with the possibility of validation
			// and to change the style to an ErrorTemplate if validation fails.
			Binding binding = new Binding("EditedText");
			binding.Mode = BindingMode.TwoWay;
			binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
			binding.Source = this;
			if (null != textBoxValidationRule)
			{
				binding.ValidationRules.Add(textBoxValidationRule); // Add a validation rule if it was provided
			}
			_textBox.SetBinding(TextBox.TextProperty, binding);

			_visualChildren.Add(_textBox);

			//Update TextBox's focus status when layout finishs.
			_textBox.LayoutUpdated += new EventHandler(EhTextBox_LayoutUpdated);

			_textBox.KeyDown += new KeyEventHandler(EhTextBoxKeyDown);
			_textBox.LostKeyboardFocus += new KeyboardFocusChangedEventHandler(EhTextBoxLostKeyboardFocus);
			_textBox.LostFocus += EhTextBoxLostFocus;
		}

		/// <summary>
		/// When the layout has finished, update the focus status and the selection of the TextBox
		/// </summary>
		private void EhTextBox_LayoutUpdated(object sender, EventArgs e)
		{
			_textBox.LayoutUpdated -= EhTextBox_LayoutUpdated;
			_textBox.Focus();
			_textBox.SelectAll();
		}

		#region Normal properties

		/// <summary>
		/// Gets a value indicating whether the edited text has a validation error
		/// </summary>
		/// <value>
		///   <c>true</c> if the validation was not successfull; otherwise, <c>false</c>.
		/// </value>
		public bool ValidationHasErrors { get { return Validation.GetHasError(_textBox); } }

		#endregion Normal properties

		#region EditedTextProperty

		/// <summary>
		/// Dependency property used to bind the TextBox.Text property. After sucessfull editing, this property contains the edited value. If the validation fails, it contains the last valid value.
		/// </summary>
		public static readonly DependencyProperty EditedTextProperty =
						DependencyProperty.Register(
										"EditedText",
										typeof(string),
										typeof(TextEditingAdorner),
										new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

		/// <summary>
		/// Gets or sets the (editable) text value of this instance. This value property is bound to the TextBox.Text property during initialization of the TextBox, and then when the user enters text into the text box.
		/// If no validation errors occured, the value is continuosly updated with the current text in the text box.
		/// </summary>
		public string EditedText
		{
			get { return (string)GetValue(EditedTextProperty); }
			set { SetValue(EditedTextProperty, value); }
		}

		#endregion EditedTextProperty

		#region TextBox event handling

		/// <summary>
		/// When in editing mode, pressing the ENTER or F2
		/// keys switches to normal mode.
		/// </summary>
		private void EhTextBoxKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter || e.Key == Key.F2)
			{
				var ev = EditingFinished;
				if (null != ev)
					ev(this, e);
				//IsEditing = false;
				//_earliestTimeItemIsEligibleForEditing = DateTime.MaxValue;
			}
		}

		/// <summary>
		/// If the TextBox looses keyboard focus (i.e. this instance is in editing mode),	this instance switches back to normal mode.
		/// </summary>
		private void EhTextBoxLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
		{
			ContextMenu newFocusedElement = e.NewFocus as ContextMenu;

			if (newFocusedElement != null && newFocusedElement.PlacementTarget == (UIElement)sender)
			{
			}
			else
			{
				var ev = EditingFinished;
				if (null != ev)
					ev(this, e);
			}
		}

		/// <summary>
		/// If an TextBox loses focus (i.e. this instance is in editing mode),
		/// this instance switches to normal mode.
		/// </summary>
		private void EhTextBoxLostFocus(object sender, RoutedEventArgs e)
		{
			ContextMenu newFocusedElement = null;

			if (newFocusedElement != null && newFocusedElement.PlacementTarget == (UIElement)sender)
			{
			}
			else
			{
				var ev = EditingFinished;
				if (null != ev)
					ev(this, e);
			}
		}

		#endregion TextBox event handling

		#region Protected Methods

		/// <summary>
		/// Implements any custom measuring behavior for the adorner.
		/// </summary>
		/// <param name="constraint">A size to constrain the adorner to.</param>
		/// <returns>
		/// A <see cref="T:System.Windows.Size" /> object representing the amount of layout space needed by the adorner.
		/// </returns>
		protected override Size MeasureOverride(Size constraint)
		{
			AdornedElement.Measure(constraint);
			_textBox.Measure(constraint);

			// since the adorner has to cover the TextBlock, it should return
			// the AdornedElement.Width, the extra 15 is to make it more clear.
			return new Size(Math.Max(_textBox.DesiredSize.Width, AdornedElement.DesiredSize.Width), Math.Max(_textBox.DesiredSize.Height, AdornedElement.DesiredSize.Height));
		}

		/// <summary>
		/// When overridden in a derived class, positions child elements and determines a size for a <see cref="T:System.Windows.FrameworkElement" /> derived class.
		/// </summary>
		/// <param name="finalSize">The final area within the parent that this element should use to arrange itself and its children.</param>
		/// <returns>
		/// The actual size used.
		/// </returns>
		protected override Size ArrangeOverride(Size finalSize)
		{
			_textBox.Arrange(new Rect(0, 0, finalSize.Width, finalSize.Height));

			return finalSize;
		}

		/// <summary>
		/// Gets the number of visual child elements within this element.
		/// </summary>
		protected override int VisualChildrenCount
		{
			get { return _visualChildren.Count; }
		}

		/// <summary>
		/// Overrides <see cref="M:System.Windows.Media.Visual.GetVisualChild(System.Int32)" />, and returns a child at the specified index from a collection of child elements.
		/// </summary>
		/// <param name="index">The zero-based index of the requested child element in the collection.</param>
		/// <returns>
		/// The requested child element. This should not return null; if the provided index is out of range, an exception is thrown.
		/// </returns>
		protected override Visual GetVisualChild(int index)
		{
			return _visualChildren[index];
		}

		#endregion Protected Methods
	}
}