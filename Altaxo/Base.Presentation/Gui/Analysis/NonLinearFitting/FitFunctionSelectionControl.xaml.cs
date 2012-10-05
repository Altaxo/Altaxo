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

using Altaxo.Collections;
using Altaxo.Main.Services;
using Altaxo.Graph;

namespace Altaxo.Gui.Analysis.NonLinearFitting
{
	/// <summary>
	/// Interaction logic for FitFunctionSelectionControl.xaml
	/// </summary>
	[UserControlForController(typeof(IFitFunctionSelectionViewEventSink))]
	public partial class FitFunctionSelectionControl : UserControl, IFitFunctionSelectionView
	{
		IFitFunctionSelectionViewEventSink _controller;


		#region Node classes

		enum RootNodeType { RootNodeBuiltin, RootNodeDocument, RootNodeUser };

		class MyNGTreeNode : NGTreeNode
		{
			public MyNGTreeNode(string text) : base(text) { }

			public virtual bool IsMenuRemoveEnabled { get { return false; } }
			public virtual bool IsMenuEditEnabled { get { return false; } }
			public object MySelf { get { return this; } }
		}

		class RootNode : MyNGTreeNode
		{
			public RootNodeType RootNodeType;

			public string NodeType { get { return RootNodeType.ToString(); } }

			public RootNode(string text, RootNodeType type)
				:
				base(text)
			{
				RootNodeType = type;
				this.Tag = type;
			}
		}

		class CategoryNode : MyNGTreeNode
		{
			public CategoryNode(string text) : base(text) { }

			public string NodeType { get { return "CategoryNode"; } }
		}

		class LeafNode : MyNGTreeNode
		{
			public LeafNode(string text) : base(text) { }
			public virtual string NodeType { get { return "LeafNode"; } }

			bool _canBeRemoved;
			bool _canBeEdited;

			public override bool IsMenuEditEnabled { get { return _canBeEdited; } }
			public override bool IsMenuRemoveEnabled { get { return _canBeRemoved; } }

			public void SetMenuEnabled(bool canBeEdited, bool canBeRemoved)
			{
				_canBeEdited = canBeEdited;
				_canBeRemoved = canBeRemoved;
			}

		}
		class BuiltinLeafNode : LeafNode
		{
			public object FunctionType;
			public override string NodeType { get { return "BuiltinLeafNode"; } }

			public BuiltinLeafNode(string text, object functionType)
				: base(text)
			{
				FunctionType = functionType;
				this.Tag = functionType;
			}
		}

		class DocumentLeafNode : LeafNode
		{
			public Altaxo.Main.Services.IFitFunctionInformation FunctionInstance;
			public override string NodeType { get { return "DocumentLeafNode"; } }


			public DocumentLeafNode(string text, Altaxo.Main.Services.IFitFunctionInformation func)
				: base(text)
			{
				FunctionInstance = func;
				this.Tag = func;
			}
		}

		class UserFileLeafNode : LeafNode
		{
			public Altaxo.Main.Services.FileBasedFitFunctionInformation FunctionInfo;
			public override string NodeType { get { return "UserFileLeafNode"; } }


			public UserFileLeafNode(string text, Altaxo.Main.Services.FileBasedFitFunctionInformation func)
				: base(text)
			{
				FunctionInfo = func;
				this.Tag = func;
			}
		}

		#endregion


		public FitFunctionSelectionControl()
		{
			InitializeComponent();
		}

		private System.Drawing.Graphics _rtfGraphics;
		private void _twFitFunctions_AfterSelect(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			var node = e.NewValue as NGTreeNode;
			if (node == null)
				return;
			var fitInfo = node.Tag as IFitFunctionInformation;

			if (_controller != null)
				_controller.EhView_SelectionChanged(fitInfo);


			if (fitInfo != null)
			{
				if (_rtfGraphics == null)
				{
					var bmp = new System.Drawing.Bitmap(4, 4);
					_rtfGraphics = System.Drawing.Graphics.FromImage(bmp);
				}
				string rtf = Altaxo.Main.Services.RtfComposerService.GetRtfText(fitInfo.Description, _rtfGraphics, GetRtfBackgroundColor(), 12);
				var stream = new System.IO.MemoryStream(ASCIIEncoding.Default.GetBytes(rtf));
				this._rtfDescription.SelectAll();
				this._rtfDescription.Selection.Load(stream, DataFormats.Rtf);
			}

		}

		#region IFitFunctionSelectionView

		public IFitFunctionSelectionViewEventSink Controller
		{
			get
			{
				return _controller;
			}
			set
			{
				_controller = value;
			}
		}

		public void ClearFitFunctionList()
		{
			this._twFitFunctions.ItemsSource = null;
		}


		public void AddFitFunctionList(string rootname, Altaxo.Main.Services.IFitFunctionInformation[] info, FitFunctionContextMenuStyle menustyle)
		{
			if (_twFitFunctions.ItemsSource == null)
			{
				_twFitFunctions.ItemsSource = new NGTreeNode().Nodes;
			}
			var mainRoot = (NGTreeNodeCollection)_twFitFunctions.ItemsSource;



			// The key of the entries is the FitFunctionAttribute, the value is the type of the fitting function
			RootNode rnode = new RootNode(rootname, RootNodeType.RootNodeBuiltin);
			mainRoot.Add(rnode);
			NGTreeNodeCollection root = rnode.Nodes;

			foreach (Altaxo.Main.Services.IFitFunctionInformation entry in info)
			{

				string[] path = entry.Category.Split(new char[] { '\\', '/' });

				var where = root;
				for (int j = 0; j < path.Length; j++)
				{
					var node = GetPathNode(where, path[j]);
					if (node == null)
					{
						node = new CategoryNode(path[j]);
						where.Add(node);
					}
					where = node.Nodes;
				}

				BuiltinLeafNode leaf = new BuiltinLeafNode(entry.Name, entry);

				switch (menustyle)
				{
					case FitFunctionContextMenuStyle.None:
						leaf.SetMenuEnabled(false, false);
						break;
					case FitFunctionContextMenuStyle.EditAndDelete:
						//	leaf.ContextMenu = _userFileLeafNodeContextMenu;
						leaf.SetMenuEnabled(true, true);
						break;
					case FitFunctionContextMenuStyle.Edit:
						//	leaf.ContextMenu = _appFileLeafNodeContextMenu;
						leaf.SetMenuEnabled(true, false);
						break;
				}
				where.Add(leaf);
			}



		}

		NGTreeNode GetPathNode(NGTreeNodeCollection coll, string path)
		{
			foreach (NGTreeNode node in coll)
			{
				if (node.Text == path && node.Tag == null)
					return node;
			}
			return null;
		}

		public void SetRtfDocumentation(string rtfString)
		{
			this._rtfDescription.AppendText(rtfString);
		}

		public NamedColor GetRtfBackgroundColor()
		{
			var brush = _rtfDescription.Background;
			if (brush is SolidColorBrush)
				return new NamedColor(GuiHelper.ToAxo(((SolidColorBrush)brush).Color));
			else
				return NamedColors.Transparent;
		}

		#endregion

		private void EhRemoveItem(object sender, RoutedEventArgs e)
		{
			var node = ((FrameworkElement)sender).Tag as NGTreeNode;
			if (_controller != null && node != null)
				_controller.EhView_RemoveItem(node.Tag as IFitFunctionInformation);
		}

		private void EhEditItem(object sender, RoutedEventArgs e)
		{
			var node = ((FrameworkElement)sender).Tag as NGTreeNode;
			if (_controller != null && node != null)
				_controller.EhView_EditItem(node.Tag as IFitFunctionInformation);
		}

		private void EhEditCopyOfThisItem(object sender, RoutedEventArgs e)
		{
			var node = ((FrameworkElement)sender).Tag as NGTreeNode;
			if (_controller != null && node != null)
				_controller.EhView_CreateItemFromHere(node.Tag as IFitFunctionInformation);
		}
	}
}
