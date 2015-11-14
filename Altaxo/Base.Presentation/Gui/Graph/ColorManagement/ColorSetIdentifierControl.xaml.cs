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

using Altaxo.Collections;
using Altaxo.Drawing;
using Altaxo.Drawing.ColorManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Altaxo.Gui.Graph.ColorManagement
{
	/// <summary>
	/// Interaction logic for ColorSetIdentifierControl.xaml
	/// </summary>
	public partial class ColorSetIdentifierControl : UserControl, IColorSetsView
	{
		#region Inner classes

		/// <summary>
		/// Selects the data template for the TreeView: either for a <see cref="T:Altaxo.Graph.NamedColor"/>, for a <see cref="Altaxo.Graph.ColorManagement.IColorSet"/> or for another node.
		/// </summary>
		public class TreeViewDataTemplateSelector : DataTemplateSelector
		{
			private FrameworkElement _parent;
			private DataTemplate _namedColorTemplate;
			private DataTemplate _colorSetTemplate;
			private DataTemplate _treeOtherTemplate;

			public TreeViewDataTemplateSelector(FrameworkElement ele)
			{
				_parent = ele;
			}

			public override DataTemplate SelectTemplate(object item, DependencyObject container)
			{
				NGTreeNode node = item as NGTreeNode;
				if (node != null)
				{
					if (node.Tag is NamedColor)
					{
						if (null == _namedColorTemplate)
							_namedColorTemplate = (DataTemplate)_parent.TryFindResource("NamedColorTemplate");
						if (null != _namedColorTemplate)
							return _namedColorTemplate;
					}
					else if (node.Tag is IColorSet)
					{
						if (null == _colorSetTemplate)
							_colorSetTemplate = (DataTemplate)_parent.TryFindResource("ColorSetTemplate");
						if (null != _colorSetTemplate)
							return _colorSetTemplate;
					}
					else
					{
						if (null == _treeOtherTemplate)
							_treeOtherTemplate = (DataTemplate)_parent.TryFindResource("TreeOtherTemplate");
						if (null != _treeOtherTemplate)
							return _treeOtherTemplate;
					}
				}

				return base.SelectTemplate(item, container);
			}
		}

		#endregion Inner classes

		public ColorSetIdentifierControl()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Provides public access to the <see cref="DataTemplateSelector"/> that selected the data template for different nodes of the TreeView.
		/// </summary>
		public DataTemplateSelector TreeViewItemTemplateSelector
		{
			get
			{
				return new TreeViewDataTemplateSelector(this);
			}
		}

		public NGTreeNode ColorSetTree
		{
			set { _treeView.ItemsSource = value.Nodes; }
		}
	}
}