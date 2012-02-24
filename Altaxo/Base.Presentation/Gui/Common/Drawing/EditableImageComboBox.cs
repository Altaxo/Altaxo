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
using System.Reflection;
using System.Diagnostics;

namespace Altaxo.Gui.Common.Drawing
{
	public class EditableImageComboBox : ImageComboBox
	{
		protected Image _img;
		protected ColumnDefinition _imgColumnDefinition;

		/// <summary>
		/// Get around the bug that the context menu of the editable part is not bound to the combobox 
		/// (<see href="http://www.wpfmentor.com/2008/12/setting-context-menu-on-editable.html"/>)
		/// </summary>
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			// Use Snoop to find the name of the TextBox part  
			// http://wpfmentor.blogspot.com/2008/11/understand-bubbling-and-tunnelling-in-5.html  
			TextBox textBox = (TextBox)Template.FindName("PART_EditableTextBox", this);

			// Create a template-binding in code  
			Binding binding = new Binding("ContextMenu");
			binding.RelativeSource = new RelativeSource(RelativeSourceMode.TemplatedParent);
			BindingOperations.SetBinding(textBox, FrameworkElement.ContextMenuProperty, binding);
		}  


		public EditableImageComboBox()
		{
			_img = new Image();
			this.IsEditable = true;
			var dpd = System.ComponentModel.DependencyPropertyDescriptor.FromProperty(ComboBox.TextProperty, this.GetType());
			dpd.AddValueChanged(this, EhTextChanged);
		}

		protected virtual void EhTextChanged(object sender, EventArgs e)
		{
			SetImageFromContent();
		}


		protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
		{
			base.OnRenderSizeChanged(sizeInfo);

			if (null == _img.Parent)
			{
				ImplantImage(sizeInfo.NewSize.Height * _relativeImageWidth, sizeInfo.NewSize.Height);
				SetImageFromContent();
			}
		}


		protected virtual void ImplantImage(double width, double height)
		{
			const double leftRightMargin = 4;
			const double topDownMargin = 3;
			var grid = VisualTreeHelper.GetChild(this, 0) as Grid;
			_imgColumnDefinition = new ColumnDefinition();
			_imgColumnDefinition.Width = new GridLength(1, GridUnitType.Auto);
			grid.ColumnDefinitions.Insert(0, _imgColumnDefinition);
			TextBox textBox = null;
			foreach (UIElement ele in grid.Children)
			{
				if (ele is TextBox || ele is System.Windows.Controls.Primitives.ToggleButton)
				{
					ele.SetValue(Grid.ColumnProperty, 1 + (int)ele.GetValue(Grid.ColumnProperty));
				}
				else
				{
					ele.SetValue(Grid.ColumnSpanProperty, 1 + (int)ele.GetValue(Grid.ColumnSpanProperty));
				}

				if (ele is TextBox)
					textBox = ele as TextBox;
			}

			if (textBox != null)
			{
				_img.Height = textBox.ActualHeight;
				_img.Margin = textBox.Margin;
			}
			else
			{
				_img.Margin = new Thickness(leftRightMargin, topDownMargin, leftRightMargin, topDownMargin);
				_img.Height = grid.ActualHeight;
			}

			_img.Stretch = Stretch.Uniform;
			grid.Children.Add(_img);
		}


		protected virtual void SetImageFromContent()
		{
		}
	}
}
