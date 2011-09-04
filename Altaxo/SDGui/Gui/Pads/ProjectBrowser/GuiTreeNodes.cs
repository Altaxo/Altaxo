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
using System.Windows.Media;
using System.Windows.Media.Imaging;


using Altaxo.Collections;

namespace Altaxo.Gui.Pads.ProjectBrowser
{
	/// <summary>
	/// Extends the WinForms tree node for special interaction features with their non-Gui equivalents.
	/// </summary>
	public class WpfBrowserTreeNode : ImageTreeViewItem, IGuiBrowserTreeNode
	{
		public static List<ImageSource> Images = new List<ImageSource>();

		public WpfBrowserTreeNode(NGBrowserTreeNode nguinode)
		{

			this.Tag = nguinode;
			this.Text = nguinode.Text;
			nguinode.GuiTag = this;
		}

		public virtual void OnNodeAdded(NGBrowserTreeNode node)
		{
			AddNode(this.Items, node);
		}


		public virtual void OnNodeRemoved(NGBrowserTreeNode node)
		{
			TreeViewItem n = (TreeViewItem)node.GuiTag;
			Items.Remove(n);
		}

		public virtual void OnNodeMultipleChanges()
		{
			var nguinode = (NGTreeNode)this.Tag;
			this.Items.Clear();
			foreach (var n in nguinode.Nodes)
				AddNode(this.Items, (NGBrowserTreeNode)n);
		}

		public static void AddNode(ItemCollection parNodes, NGBrowserTreeNode nguinode)
		{
			var curNode = new WpfBrowserTreeNode(nguinode);
			curNode.ContextMenu = (ContextMenu)nguinode.ContextMenu;


			if (null != nguinode.ImageIndex)
			{
				var src = Images[(int)nguinode.ImageIndex];
				curNode.SelectedImage = src;
				curNode.UnselectedImage = src;
			}
			if (null != nguinode.SelectedImageIndex)
				curNode.SelectedImage = Images[(int)nguinode.ImageIndex];


			foreach (var n in nguinode.Nodes)
				AddNode(curNode.Items, (NGBrowserTreeNode)n);
			parNodes.Add(curNode);
		}
	}


	/// <summary>
	/// Simulate the equivalent of the non-Gui root node. Note that in the TreeList there is no such thing than a root node.
	/// There is only the collection of nodes at the root.
	/// </summary>
	public class WpfRootNode : IGuiBrowserTreeNode
	{
		ItemCollection _rootCollection;
		NGTreeNode _nguiRoot;

		public WpfRootNode(NGTreeNode nguiroot, ItemCollection rootCollection)
		{
			_rootCollection = rootCollection;
			foreach (var n in nguiroot.Nodes)
				WpfBrowserTreeNode.AddNode(_rootCollection, (NGBrowserTreeNode)n);

			_nguiRoot = nguiroot;
			nguiroot.GuiTag = this;
		}

		public void OnNodeAdded(NGBrowserTreeNode node)
		{
			WpfBrowserTreeNode.AddNode(_rootCollection, (NGBrowserTreeNode)node);
		}

		public void OnNodeRemoved(NGBrowserTreeNode node)
		{
			TreeViewItem n = (TreeViewItem)node.GuiTag;
			_rootCollection.Remove(n);
		}

		public virtual void OnNodeMultipleChanges()
		{
			_rootCollection.Clear();
			foreach (var n in _nguiRoot.Nodes)
				WpfBrowserTreeNode.AddNode(_rootCollection, (NGBrowserTreeNode)n);
		}

	}


	public class ImageTreeViewItem : TreeViewItem
	{
		TextBlock _text;
		Image _image;
		ImageSource _srcSelected;
		ImageSource _srcUnselected;

		public ImageTreeViewItem()
		{
			StackPanel stack = new StackPanel();

			stack.Orientation = Orientation.Horizontal;

			Header = stack;

			_image = new Image();

			_image.VerticalAlignment = VerticalAlignment.Center;

			_image.Margin = new Thickness(0, 0, 2, 0);

			_image.Source = _srcSelected;

			stack.Children.Add(_image);

			_text = new TextBlock();

			_text.VerticalAlignment = VerticalAlignment.Center;

			stack.Children.Add(_text);

		}

		public string Text
		{

			set { _text.Text = value; }

			get { return _text.Text; }

		}

		public ImageSource SelectedImage
		{

			set
			{

				_srcSelected = value;

				_image.Source = _srcSelected;

			}

			get { return _srcSelected; }

		}

		public ImageSource UnselectedImage
		{

			set
			{

				_srcUnselected = value;

			}

			get { return _srcUnselected; }

		}



		protected override void OnSelected(RoutedEventArgs args)
		{

			base.OnSelected(args);

			_image.Source = _srcSelected;

		}

		protected override void OnUnselected(RoutedEventArgs args)
		{

			base.OnUnselected(args);

			if (_srcUnselected != null)

				_image.Source = _srcUnselected;

			else

				_image.Source = _srcSelected;

		}
	}

}
