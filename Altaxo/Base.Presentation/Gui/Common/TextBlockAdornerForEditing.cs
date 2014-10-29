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
	/// This adorner is intended for temporary construction in the moment when the <see cref="TextBlockForEditing"/> instance goes to edit mode, and subsequent destruction when edit mode is left. Thats why,
	/// it is visible from the beginning and can not be made invisible, unless you remove it from the adorner layer.
	/// </summary>
	internal sealed class TextBlockAdornerForEditing : Adorner
	{
		#region Member variables

		/// <summary>The visual children of the adorner.</summary>
		private VisualCollection _visualChildren;

		//
		/// <summary>The TextBox that is used for editing.</summary>
		private TextBox _textBox;

		/// <summary>Extra padding for the text box at the end.</summary>
		private const double _extraWidth = 15;

		#endregion Member variables

		/// <summary>
		/// Initializes a new instance of the <see cref="TextBlockAdornerForEditing"/> class.
		/// </summary>
		/// <param name="adornedElement">The adorned text block element.</param>
		/// <param name="textBoxStyle">The style for the text box that is used for editing.</param>
		/// <param name="textBoxValidationRule">The validation rule for the text box that is used for editing.</param>
		public TextBlockAdornerForEditing(TextBlock adornedElement, Style textBoxStyle, ValidationRule textBoxValidationRule)
			: base(adornedElement)
		{
			_visualChildren = new VisualCollection(this);

			// Build the text box
			_textBox = new TextBox();

			if (textBoxStyle != null)
				_textBox.Style = textBoxStyle;

			//Bind the text of the TextBlock from/to the text of the TextBox
			Binding binding = new Binding("Text");
			binding.Mode = BindingMode.TwoWay;
			binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
			binding.Source = this.AdornedElement;
			if (null != textBoxValidationRule)
			{
				binding.ValidationRules.Add(textBoxValidationRule);
			}

			_textBox.SetBinding(TextBox.TextProperty, binding);

			_visualChildren.Add(_textBox);

			//Update TextBox's focus status when layout finishs.
			_textBox.LayoutUpdated += new EventHandler(EhTextBox_LayoutUpdated);
		}

		/// <summary>
		/// Gets the text box that is used for editing.
		/// </summary>
		/// <value>
		/// The text box.
		/// </value>
		public TextBox TextBox { get { return _textBox; } }

		/// <summary>
		/// When the layout has finished, update the focus status and the selection of the TextBox
		/// </summary>
		private void EhTextBox_LayoutUpdated(object sender, EventArgs e)
		{
			_textBox.LayoutUpdated -= EhTextBox_LayoutUpdated;
			_textBox.Focus();
			_textBox.SelectAll();
		}

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
			return new Size(AdornedElement.DesiredSize.Width + _extraWidth, _textBox.DesiredSize.Height);
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