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

			if (null==_img.Parent)
			{
				ImplantImage(sizeInfo.NewSize.Height*_relativeImageWidth, sizeInfo.NewSize.Height);
				SetImageFromContent();
			}
		}


		protected virtual void ImplantImage(double width, double height)
		{
			const double leftRightMargin = 4;
			var grid = VisualTreeHelper.GetChild(this, 0) as Grid;
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

			
			_img.Height = grid.ActualHeight;
			_img.Margin = new Thickness(leftRightMargin, 0, leftRightMargin, 0);
			_img.Stretch = Stretch.Uniform;
			grid.Children.Add(_img);
		}

		protected virtual void SetImageFromContent()
		{
		}
	}
}
