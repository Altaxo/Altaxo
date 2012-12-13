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
		/// This is the edit box inside the editable ComboBox that is used to enter text.
		/// </summary>
		protected TextBox _editBox;

		/// <summary>
		/// Get around the bug that the context menu of the editable part is not bound to the combobox 
		/// (<see href="http://www.wpfmentor.com/2008/12/setting-context-menu-on-editable.html"/>)
		/// </summary>
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			// Use Snoop to find the name of the TextBox part  
			// http://wpfmentor.blogspot.com/2008/11/understand-bubbling-and-tunnelling-in-5.html  
			_editBox = (TextBox)Template.FindName("PART_EditableTextBox", this);
			_editBox.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Right;

			// Create a template-binding in code  
			Binding binding = new Binding("ContextMenu");
			binding.RelativeSource = new RelativeSource(RelativeSourceMode.TemplatedParent);
			BindingOperations.SetBinding(_editBox, FrameworkElement.ContextMenuProperty, binding);
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
			_img.Height = _editBox.ActualHeight;
			_img.Margin = _editBox.Margin;
			_img.Stretch = Stretch.Uniform;


			var parent = _editBox.Parent;


			var stackPanel = new StackPanel();
			stackPanel.Orientation = Orientation.Horizontal;
			stackPanel.Children.Add(_img);
			// stackPanel.Children.Add(_editBox); // this must be postponed here, since _editBox is still into the hierarchy

			if (parent is ContentControl)
			{
				((ContentControl)parent).Content = stackPanel;
			}
			else if (parent is Decorator)
			{
				((Decorator)parent).Child = stackPanel;
			}
			else if (parent is Panel)
			{
				var panel = (Panel)parent;
				var idx = panel.Children.IndexOf(_editBox);
				if (idx < 0)
					throw new InvalidOperationException(string.Format("The parent of the EditBox is a {0}, but the parent's children collection does not contain the EditBox", panel.GetType()));
				panel.Children.RemoveAt(idx);
				panel.Children.Insert(idx, stackPanel);
			}
			else
			{
				var stb = new StringBuilder();
				stb.AppendFormat("Unexpected location of the EditBox within {0}", this.ToString());
				stb.AppendLine();
				stb.AppendFormat("The parent of the editbox is {0}", _editBox.Parent.ToString());
				stb.AppendLine();
				stb.AppendLine("The hierarchy of childs is as follows:");
				PrintVisualChilds(this, 0, stb);
				throw new ApplicationException(stb.ToString());
			}

			stackPanel.Children.Add(_editBox);

			// now some special properties
			if (parent is Grid)
			{
				foreach(DependencyProperty dp in new DependencyProperty[]{ Grid.RowProperty, Grid.ColumnProperty, Grid.RowSpanProperty, Grid.ColumnSpanProperty})
				{
				stackPanel.SetValue(dp, _editBox.GetValue(dp));
				}
			}
			if (parent is DockPanel)
			{
				stackPanel.SetValue(DockPanel.DockProperty, _editBox.GetValue(DockPanel.DockProperty));
			}
			
			/*

			if (_editBox.Parent is Grid) // most Windows version have the TextBox located inside a Grid
			{
				var grid = _editBox.Parent as Grid;
				_imgColumnDefinition = new ColumnDefinition();
				_imgColumnDefinition.Width = new GridLength(1, GridUnitType.Auto);
				grid.ColumnDefinitions.Insert(0, _imgColumnDefinition);
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
				}
				grid.Children.Add(_img);
			}
			else if (_editBox.Parent is DockPanel) // Some Windows XP versions have the TextBox sitting in a DockPanel instead of a Grid
			{
				var dockp = _editBox.Parent as DockPanel;
				var list = new List<UIElement>();
				foreach (UIElement child in dockp.Children) // collect the original children temporary in a list
					list.Add(child);

				dockp.Children.Clear(); // clear the children, because we need to dock them again

				_img.SetValue(DockPanel.DockProperty, Dock.Left);
				dockp.Children.Add(_img); // add the image to the left side
				foreach (UIElement child in list) // now dock the original children again
					dockp.Children.Add(child);
			}
			else
			{
				var stb = new StringBuilder();
				stb.AppendFormat("Unexpected location of grid within {0}", this.ToString());
				stb.AppendLine();
				stb.AppendFormat("The parent of the editbox is {0}", _editBox.Parent.ToString());
				stb.AppendLine();
				stb.AppendLine("The hierarchy of childs is as follows:");
				PrintVisualChilds(this, 0, stb);
				throw new ApplicationException(stb.ToString());
			}

			*/
		
		}


		/// <summary>Prints the visual childs recursively (intended only for debugging).</summary>
		/// <param name="start">The parent.</param>
		/// <param name="level">The level number.</param>
		/// <param name="stb">The StringBuilder where the information should be printed to.</param>
		private void PrintVisualChilds(DependencyObject start, int level, StringBuilder stb)
		{
			int count = VisualTreeHelper.GetChildrenCount(start);
			for(int i=0;i<count;++i)
			{
				var child = VisualTreeHelper.GetChild(start, i);
				for(int j=0;j<level;++j) 
					stb.Append("  ");
				if(child is Grid)
					stb.AppendFormat("child[{0}]: {1} (#cols:{2})", i, child.ToString(), ((Grid)child).ColumnDefinitions.Count);
				else
					stb.AppendFormat("child[{0}]: {1}",i,child.ToString());
				stb.AppendLine();
				PrintVisualChilds(child, level + 1, stb);
			}
		}

		protected virtual void SetImageFromContent()
		{
		}
	}
}
