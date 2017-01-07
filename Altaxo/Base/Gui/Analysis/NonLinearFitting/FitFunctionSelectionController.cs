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

#endregion Copyright

using Altaxo.Calc.Regression.Nonlinear;
using Altaxo.Drawing;
using Altaxo.Graph;
using Altaxo.Main.Services;
using Altaxo.Scripting;
using System;
using System.Text;
using Altaxo.Collections;
using Altaxo.Main;

namespace Altaxo.Gui.Analysis.NonLinearFitting
{
	public interface IFitFunctionSelectionView
	{
		void SetFitFunctions(NGTreeNodeCollection list);

		void SetRtfDocumentation(string rtfString);

		NamedColor GetRtfBackgroundColor();

		// events

		event Action<IFitFunctionInformation> SelectionChanged;

		event Action<IFitFunctionInformation> EditItem;

		event Action<IFitFunctionInformation> EditCopyOfItem;

		event Action<IFitFunctionInformation> RemoveItem;

		event Action<IFitFunctionInformation> ItemDoubleClicked;
	}

	public interface IFitFunctionSelectionController : IMVCAController
	{
		void Refresh();
	}

	[ExpectedTypeOfView(typeof(IFitFunctionSelectionView))]
	public class FitFunctionSelectionController : IMVCAController
	{
		#region Other

		private enum FitFunctionContextMenuStyle
		{
			None,
			Edit,
			EditAndDelete
		}

		#endregion Other

		#region Node classes

		private enum RootNodeType { RootNodeBuiltin, RootNodeApplication, RootNodeUser, RootNodeDocument };

		private class MyNGTreeNode : NGTreeNode
		{
			public MyNGTreeNode(string text)
				: base(text)
			{
			}

			public virtual bool IsMenuRemoveEnabled { get { return false; } }

			public virtual bool IsMenuEditEnabled { get { return false; } }

			public virtual bool IsMenuEditCopyEnabled { get { return false; } }

			public object MySelf { get { return this; } }
		}

		private class RootNode : MyNGTreeNode
		{
			public ItemDefinitionLevel RootNodeType;

			public string NodeType { get { return RootNodeType.ToString(); } }

			public RootNode(string text, ItemDefinitionLevel type)
				:
				base(text)
			{
				RootNodeType = type;
				this.Tag = type;
			}
		}

		private class CategoryNode : MyNGTreeNode
		{
			public CategoryNode(string text)
				: base(text)
			{
			}

			public string NodeType { get { return "CategoryNode"; } }
		}

		private class LeafNode : MyNGTreeNode
		{
			public LeafNode(string text)
				: base(text)
			{
			}

			public virtual string NodeType { get { return "LeafNode"; } }

			private bool _canBeRemoved;
			private bool _canBeEdited;
			private bool _canBeACopyEdited;

			public override bool IsMenuEditEnabled { get { return _canBeEdited; } }

			public override bool IsMenuEditCopyEnabled { get { return _canBeACopyEdited; } }

			public override bool IsMenuRemoveEnabled { get { return _canBeRemoved; } }

			public void SetMenuEnabled(bool canBeEdited, bool canBeACopyEdited, bool canBeRemoved)
			{
				_canBeEdited = canBeEdited;
				_canBeACopyEdited = canBeACopyEdited;
				_canBeRemoved = canBeRemoved;
			}
		}

		private class FitFunctionLeafNode : LeafNode
		{
			public object FunctionType;

			public override string NodeType { get { return "BuiltinLeafNode"; } }

			public FitFunctionLeafNode(string text, object functionType)
				: base(text)
			{
				FunctionType = functionType;
				this.Tag = functionType;
			}
		}

		#endregion Node classes

		public event Action<IFitFunctionInformation> FitFunctionSelected;

		private IFitFunction _doc;
		private IFitFunctionInformation _tempdoc;
		private IFitFunctionSelectionView _view;

		private NGTreeNode _fitFunctionsRoot;

		private RootNode _nodeBuiltin;
		private RootNode _nodeApplication;
		private RootNode _nodeUserDefined;
		private RootNode _nodeProject;

		public FitFunctionSelectionController(IFitFunction doc)
		{
			_doc = doc;
			_tempdoc = null;
			Initialize(true);
		}

		private void AttachView()
		{
			_view.SelectionChanged += this.EhView_SelectionChanged;
			_view.EditItem += this.EhView_EditItem;
			_view.EditCopyOfItem += EhView_CreateItemFromHere;
			_view.RemoveItem += EhView_RemoveItem;
			_view.ItemDoubleClicked += EhView_ItemDoubleClicked;
		}

		private void DetachView()
		{
			_view.SelectionChanged -= this.EhView_SelectionChanged;
			_view.EditItem -= this.EhView_EditItem;
			_view.EditCopyOfItem -= EhView_CreateItemFromHere;
			_view.RemoveItem -= EhView_RemoveItem;
			_view.ItemDoubleClicked -= EhView_ItemDoubleClicked;
		}

		public void Refresh()
		{
			Initialize(true);
		}

		public void Initialize(bool initData)
		{
			if (initData)
			{
				_fitFunctionsRoot = new NGTreeNode();

				_fitFunctionsRoot.Nodes.Add(_nodeBuiltin = new RootNode("Builtin", ItemDefinitionLevel.Builtin));
				_fitFunctionsRoot.Nodes.Add(_nodeApplication = new RootNode("Application", ItemDefinitionLevel.Application));
				_fitFunctionsRoot.Nodes.Add(_nodeUserDefined = new RootNode("UserDefined", ItemDefinitionLevel.UserDefined));
				_fitFunctionsRoot.Nodes.Add(_nodeProject = new RootNode("Project", ItemDefinitionLevel.Project));

				AddFitFunctionList(_nodeBuiltin, Current.FitFunctionService.GetBuiltinFitFunctions(), FitFunctionContextMenuStyle.None);
				AddFitFunctionList(_nodeApplication, Current.FitFunctionService.GetApplicationFitFunctions(), FitFunctionContextMenuStyle.Edit);
				AddFitFunctionList(_nodeUserDefined, Current.FitFunctionService.GetUserDefinedFitFunctions(), FitFunctionContextMenuStyle.EditAndDelete);
				AddFitFunctionList(_nodeProject, Current.FitFunctionService.GetDocumentFitFunctions(), FitFunctionContextMenuStyle.EditAndDelete);
			}

			if (_view != null)
			{
				_view.SetFitFunctions(_fitFunctionsRoot.Nodes);
			}
		}

		private void AddFitFunctionList(RootNode rootNode, Altaxo.Main.Services.IFitFunctionInformation[] info, FitFunctionContextMenuStyle menustyle)
		{
			NGTreeNodeCollection root = rootNode.Nodes;

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

				var creationTime = entry.CreationTime;
				var nodeText = entry.Name;

				if (null != creationTime)
				{
					nodeText += " (" + creationTime.Value.ToString("yyyy-MM-dd HH:mm:ss") + ")";
				}

				FitFunctionLeafNode leaf = new FitFunctionLeafNode(nodeText, entry);

				switch (menustyle)
				{
					case FitFunctionContextMenuStyle.None:
						leaf.SetMenuEnabled(false, false, false);
						break;

					case FitFunctionContextMenuStyle.EditAndDelete:
						//	leaf.ContextMenu = _userFileLeafNodeContextMenu;
						leaf.SetMenuEnabled(false, true, true);
						break;

					case FitFunctionContextMenuStyle.Edit:
						//	leaf.ContextMenu = _appFileLeafNodeContextMenu;
						leaf.SetMenuEnabled(true, false, false);
						break;
				}
				where.Add(leaf);
			}
		}

		private NGTreeNode GetPathNode(NGTreeNodeCollection coll, string path)
		{
			foreach (NGTreeNode node in coll)
			{
				if (node.Text == path && node.Tag == null)
					return node;
			}
			return null;
		}

		private FitFunctionLeafNode SelectFitFunction(IFitFunction func)
		{
			var selNode = (FitFunctionLeafNode)TreeNodeExtensions.AnyBetweenHereAndLeaves(_fitFunctionsRoot, (node) => node is FitFunctionLeafNode dln && dln.FunctionType is DocumentFitFunctionInformation dffi && func.Equals(dffi.FitFunction));

			if (null != selNode)
			{
				selNode.IsExpanded = true;
				selNode.IsSelected = true;
			}

			return selNode;
		}

		public void EhView_SelectionChanged(IFitFunctionInformation selectedtag)
		{
			_tempdoc = selectedtag;
		}

		public void EhView_EditItem(IFitFunctionInformation selectedtag)
		{
			EditItemOrItemCopy(selectedtag, false);
		}

		public void EhView_CreateItemFromHere(IFitFunctionInformation selectedtag)
		{
			EditItemOrItemCopy(selectedtag, true);
		}

		public void EditItemOrItemCopy(IFitFunctionInformation selectedtag, bool editItemCopy)
		{
			IFitFunction func = null;
			if (selectedtag is DocumentFitFunctionInformation)
			{
				func = selectedtag.CreateFitFunction();
			}
			else if (selectedtag is FileBasedFitFunctionInformation)
			{
				func = Altaxo.Main.Services.FitFunctionService.ReadUserDefinedFitFunction(selectedtag as Altaxo.Main.Services.FileBasedFitFunctionInformation);
			}

			if (null != func)
			{
				var editedFunc = Edit(func, editItemCopy);

				if (null != editedFunc)
				{
					var selNode = SelectFitFunction(editedFunc);

					if (null != selNode)
						FitFunctionSelected((IFitFunctionInformation)selNode.Tag);
				}
			}
		}

		private FitFunctionScript Edit(IFitFunction func, bool editItemCopy)
		{
			if (null != func)
			{
				if (func is FitFunctionScript)
					editItemCopy = true; // for scripts, we always edit a copy

				object[] args = new object[] { (editItemCopy && func is ICloneable cfunc) ? cfunc.Clone() : func };
				if (Current.Gui.ShowDialog(args, "Edit fit function script"))
				{
					if (args[0] is FitFunctionScript editedScript)
					{
						var ctrl = new Altaxo.Gui.Scripting.FitFunctionNameAndCategoryController();
						ctrl.InitializeDocument(editedScript);
						if (Current.Gui.ShowDialog(ctrl, "Store?"))
						{
							// add the new script to the list
							var editedScript2 = Current.Project.FitFunctionScripts.Add(editedScript);

							if (!object.ReferenceEquals(editedScript, editedScript2))
								Current.Gui.InfoMessageBox("Edited fit function was not added because exactly the same fit function already exists.", "To your information");

							// Note: category and/or name can have changed now, so it is more save to
							// completely reinitialize the fit function tree
							Initialize(true);

							return editedScript2;
						}
					}
				}
			}
			return null;
		}

		public void EhView_RemoveItem(IFitFunctionInformation selectedtag)
		{
			if (selectedtag is DocumentFitFunctionInformation)
			{
				Current.Project.FitFunctionScripts.Remove(selectedtag.CreateFitFunction() as FitFunctionScript);
				Initialize(true);
			}
			else if (selectedtag is FileBasedFitFunctionInformation)
			{
				Current.FitFunctionService.RemoveUserDefinedFitFunction(selectedtag as Altaxo.Main.Services.FileBasedFitFunctionInformation);
				Initialize(true);
			}
		}

		public void EhView_ItemDoubleClicked(IFitFunctionInformation selectedtag)
		{
			_tempdoc = selectedtag;
			FitFunctionSelected?.Invoke(selectedtag);
		}

		#region IMVCController Members

		public object ViewObject
		{
			get
			{
				return _view;
			}
			set
			{
				if (_view != null)
					DetachView();

				_view = value as IFitFunctionSelectionView;

				Initialize(false);

				if (_view != null)
					AttachView();
			}
		}

		public object ModelObject
		{
			get
			{
				return _doc;
			}
		}

		public void Dispose()
		{
		}

		#endregion IMVCController Members

		#region IApplyController Members

		public bool Apply(bool disposeController)
		{
			if (_tempdoc == null) // nothing selected, so return the original doc
				return true;

			try
			{
				_doc = _tempdoc.CreateFitFunction();
				return true;
			}
			catch (Exception ex)
			{
				Current.Gui.ErrorMessageBox("Can not create fit function. An exception was thrown: " + ex.Message);
			}
			return false;
		}

		/// <summary>
		/// Try to revert changes to the model, i.e. restores the original state of the model.
		/// </summary>
		/// <param name="disposeController">If set to <c>true</c>, the controller should release all temporary resources, since the controller is not needed anymore.</param>
		/// <returns>
		///   <c>True</c> if the revert operation was successfull; <c>false</c> if the revert operation was not possible (i.e. because the controller has not stored the original state of the model).
		/// </returns>
		public bool Revert(bool disposeController)
		{
			return false;
		}

		#endregion IApplyController Members
	}
}