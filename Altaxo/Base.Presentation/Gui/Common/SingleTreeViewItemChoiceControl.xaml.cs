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
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Altaxo.Gui.Common
{
	/// <summary>
	/// Interaction logic for SingleTreeViewItemChoiceControl.xaml
	/// </summary>
	public partial class SingleTreeViewItemChoiceControl : UserControl, Altaxo.Gui.Worksheet.ISingleTreeViewItemChoiceView
	{
		public event Action<Collections.NGTreeNode> SelectionChanged;

		public SingleTreeViewItemChoiceControl()
		{
			InitializeComponent();
		}

		private void EhSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			if (e.NewValue is Collections.NGTreeNode)
			{
				if (null != SelectionChanged)
					SelectionChanged((Collections.NGTreeNode)e.NewValue);
			}
		}

		public void Initialize(Collections.NGTreeNodeCollection nodes)
		{
			_guiTreeView.ItemsSource = null;
			_guiTreeView.ItemsSource = nodes;
		}
	}
}