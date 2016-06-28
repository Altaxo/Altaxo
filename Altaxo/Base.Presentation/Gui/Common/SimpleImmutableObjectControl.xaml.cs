#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2016 Dr. Dirk Lellinger
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

namespace Altaxo.Gui.Common
{
	/// <summary>
	/// Interaction logic for VariantToVariantTransformationControl.xaml
	/// </summary>
	public partial class SimpleImmutableObjectControl : UserControl, ISimpleImmutableObjectView
	{
		private List<Tuple<Type, FrameworkElement>> _elements = new List<Tuple<Type, FrameworkElement>>();

		public SimpleImmutableObjectControl()
		{
			InitializeComponent();
		}

		public void Values_Initialize(IEnumerable<Tuple<string, Type, object>> values)
		{
			_elements.Clear();
			_guiGrid.Children.Clear();

			_guiGrid.RowDefinitions.Clear();

			int idx = 0;
			foreach (var tuple in values)
			{
				_guiGrid.RowDefinitions.Add(new RowDefinition());
				_guiGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(4) });

				var label = new Label { Content = tuple.Item1 + ":" };
				label.SetValue(Grid.RowProperty, idx * 2);
				_guiGrid.Children.Add(label);

				FrameworkElement fe;
				if (tuple.Item2 == typeof(double))
				{
					var item = new Altaxo.Gui.Common.NumericDoubleTextBox();
					fe = item;
					if (tuple.Item3 is double)
						item.SelectedValue = (double)tuple.Item3;
				}
				else
				{
					throw new NotImplementedException();
				}

				fe.SetValue(Grid.ColumnProperty, 2);
				fe.SetValue(Grid.RowProperty, idx * 2);
				_guiGrid.Children.Add(fe);

				_elements.Add(new Tuple<Type, FrameworkElement>(tuple.Item2, fe));
				++idx;
			}

			// append another RowDefinition to ensure that the textboxes are not expanded vertically
			_guiGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(4, GridUnitType.Star) });
		}

		public object Value_Get(int idx)
		{
			if (_elements[idx].Item2 is Altaxo.Gui.Common.NumericDoubleTextBox)
			{
				return (_elements[idx].Item2 as Altaxo.Gui.Common.NumericDoubleTextBox).SelectedValue;
			}
			else
			{
				throw new NotImplementedException();
			}
		}
	}
}