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

using Altaxo.Collections;
using Altaxo.Graph.Graph3D.Plot.Groups;
using Altaxo.Graph.Graph3D.Plot.Styles;
using Altaxo.Gui.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Gui.Graph3D.Plot.Groups
{
	public interface IScatterSymbolListView
	{
		void AvailableLists_Initialize(NGTreeNodeCollection nodes);

		event Action<NGTreeNode> AvailableLists_SelectionChanged;

		event Action CurrentScatterSymbolListName_Changed;

		void AvailableScatterSymbols_Initialize(SelectableListNodeList items);

		void CurrentScatterSymbolList_Initialize(SelectableListNodeList items);

		void CurrentScatterSymbolListName_Initialize(string name, bool isEnabled, bool isMarked, string toolTipText);

		string CurrentScatterSymbolListName { get; }

		event CanStartDragDelegate AvailableSymbols_CanStartDrag;

		event StartDragDelegate AvailableSymbols_StartDrag;

		event DragEndedDelegate AvailableSymbols_DragEnded;

		event DragCancelledDelegate AvailableSymbols_DragCancelled;

		event DropCanAcceptDataDelegate AvailableSymbols_DropCanAcceptData;

		event DropDelegate AvailableSymbols_Drop;

		event CanStartDragDelegate CurrentSymbols_CanStartDrag;

		event StartDragDelegate CurrentSymbols_StartDrag;

		event DragEndedDelegate CurrentSymbols_DragEnded;

		event DragCancelledDelegate CurrentSymbols_DragCancelled;

		event DropCanAcceptDataDelegate CurrentSymbols_DropCanAcceptData;

		event DropDelegate CurrentSymbols_Drop;

		bool StoreInUserSettings { get; }

		event Action AvailableItem_AddToCurrent;

		event Action CurrentItem_MoveUp;

		event Action CurrentItem_MoveDown;

		event Action CurrentItem_Remove;

		event Action CurrentItem_Edit;

		event Action CurrentList_Store;
	}

	[ExpectedTypeOfView(typeof(IScatterSymbolListView))]
	[UserControllerForObject(typeof(ScatterSymbolList))]
	public class ScatterSymbolListController : MVCANControllerEditImmutableDocBase<ScatterSymbolList, IScatterSymbolListView>
	{
		private NGTreeNode _availableListsRootNode;
		private SelectableListNodeList _availableScatterSymbolTypes;
		private SelectableListNodeList _currentScatterSymbols;

		private bool _currentScatterSymbols_IsDirty;
		private bool _isNameOfNewListValid;

		protected override void Initialize(bool initData)
		{
			base.Initialize(initData);

			if (initData)
			{
				Controller_AvailableLists_Initialize();

				// Available scatter symbols
				var scatterSymbolTypes = Altaxo.Main.Services.ReflectionService.GetNonAbstractSubclassesOf(typeof(IScatterSymbol));
				_availableScatterSymbolTypes = new SelectableListNodeList();
				foreach (var type in scatterSymbolTypes)
					_availableScatterSymbolTypes.Add(new SelectableListNode(type.Name, type, false));

				// Current scatter symbols
				_currentScatterSymbols = new SelectableListNodeList();
				foreach (var sym in _doc.Items)
				{
					_currentScatterSymbols.Add(new SelectableListNode(sym.GetType().Name, sym, false));
				}
			}

			if (null != _view)
			{
				View_AvailableLists_Initialize();

				_view.AvailableScatterSymbols_Initialize(_availableScatterSymbolTypes);
				_view.CurrentScatterSymbolList_Initialize(_currentScatterSymbols);
				_view.CurrentScatterSymbolListName_Initialize(_doc.Name, false, false, "Name can not be changed because list is already stored!");
			}
		}

		public override bool Apply(bool disposeController)
		{
			if (false == TryToStoreList())
				return ApplyEnd(false, disposeController);

			return ApplyEnd(true, disposeController);
		}

		public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
		{
			yield break;
		}

		protected override void AttachView()
		{
			base.AttachView();

			_view.AvailableLists_SelectionChanged += EhAvailableLists_SelectionChanged;

			_view.CurrentScatterSymbolListName_Changed += EhCurrentScatterSymbolListName_Changed;

			_view.AvailableSymbols_CanStartDrag += EhAvailableSymbols_CanStartDrag;

			_view.AvailableSymbols_StartDrag += EhAvailableSymbols_StartDrag;

			_view.AvailableSymbols_DragEnded += AvailableSymbols_DragEnded;

			_view.AvailableSymbols_DragCancelled += EhAvailableSymbols_DragCancelled;

			_view.AvailableSymbols_DropCanAcceptData += EhAvailableSymbols_DropCanAcceptData;

			_view.AvailableSymbols_Drop += EhAvailableSymbols_Drop;

			_view.CurrentSymbols_CanStartDrag += EhCurrentSymbols_CanStartDrag;

			_view.CurrentSymbols_StartDrag += EhCurrentSymbols_StartDrag;

			_view.CurrentSymbols_DragEnded += EhCurrentSymbols_DragEnded;

			_view.CurrentSymbols_DragCancelled += EhCurrentSymbols_DragCancelled;

			_view.CurrentSymbols_DropCanAcceptData += EhCurrentSymbols_DropCanAcceptData;

			_view.CurrentSymbols_Drop += EhCurrentSymbols_Drop;

			_view.AvailableItem_AddToCurrent += EhAvailableItem_AddToCurrent;

			_view.CurrentItem_MoveUp += EhCurrentItem_MoveUp;

			_view.CurrentItem_MoveDown += EhCurrentItem_MoveDown;

			_view.CurrentItem_Remove += EhCurrentItem_Remove;

			_view.CurrentItem_Edit += EhCurrentItem_Edit;

			_view.CurrentList_Store += EhCurrentList_Store;
		}

		protected override void DetachView()
		{
			_view.AvailableLists_SelectionChanged -= EhAvailableLists_SelectionChanged;

			_view.CurrentScatterSymbolListName_Changed -= EhCurrentScatterSymbolListName_Changed;

			_view.AvailableSymbols_CanStartDrag -= EhAvailableSymbols_CanStartDrag;

			_view.AvailableSymbols_StartDrag -= EhAvailableSymbols_StartDrag;

			_view.AvailableSymbols_DragEnded -= AvailableSymbols_DragEnded;

			_view.AvailableSymbols_DragCancelled -= EhAvailableSymbols_DragCancelled;

			_view.AvailableSymbols_DropCanAcceptData -= EhAvailableSymbols_DropCanAcceptData;

			_view.AvailableSymbols_Drop -= EhAvailableSymbols_Drop;

			_view.CurrentSymbols_CanStartDrag -= EhCurrentSymbols_CanStartDrag;

			_view.CurrentSymbols_StartDrag -= EhCurrentSymbols_StartDrag;

			_view.CurrentSymbols_DragEnded -= EhCurrentSymbols_DragEnded;

			_view.CurrentSymbols_DragCancelled -= EhCurrentSymbols_DragCancelled;

			_view.CurrentSymbols_DropCanAcceptData -= EhCurrentSymbols_DropCanAcceptData;

			_view.CurrentSymbols_Drop -= EhCurrentSymbols_Drop;

			_view.AvailableItem_AddToCurrent -= EhAvailableItem_AddToCurrent;

			_view.CurrentItem_MoveUp -= EhCurrentItem_MoveUp;

			_view.CurrentItem_MoveDown -= EhCurrentItem_MoveDown;

			_view.CurrentItem_Remove -= EhCurrentItem_Remove;

			_view.CurrentItem_Edit -= EhCurrentItem_Edit;

			_view.CurrentList_Store -= EhCurrentList_Store;

			base.DetachView();
		}

		#region Available lists

		private void Controller_AvailableLists_Initialize()
		{
			_availableListsRootNode = new NGTreeNode();

			var allNames = ScatterSymbolListManager.Instance.GetAllListNames().ToArray();
			Array.Sort(allNames);
			var dict = new Dictionary<string, NGTreeNode>();

			foreach (var name in allNames)
			{
				var list = ScatterSymbolListManager.Instance.GetList(name);
				_availableListsRootNode.Nodes.Add(new NGTreeNode(name) { Tag = list, IsSelected = object.ReferenceEquals(list, _doc) });
			}
		}

		private void View_AvailableLists_Initialize()
		{
			_view.AvailableLists_Initialize(_availableListsRootNode.Nodes);
		}

		#endregion Available lists

		private void ControllerAndView_CurrentItemsAndName_Initialize()
		{
			_currentScatterSymbols.Clear();
			foreach (var sym in _doc.Items)
			{
				_currentScatterSymbols.Add(new SelectableListNode(sym.GetType().Name, sym, false));
			}
			_view?.CurrentScatterSymbolList_Initialize(_currentScatterSymbols);
			_view?.CurrentScatterSymbolListName_Initialize(_doc.Name, false, false, "Name can't be changed because list is already stored!");
			_currentScatterSymbols_IsDirty = false;
		}

		private bool TryToStoreList()
		{
			if (_currentScatterSymbols.Count == 0)
			{
				Current.Gui.ErrorMessageBox("The list does not contains any items, thus it can not be stored");
				return false;
			}

			if (!_isNameOfNewListValid)
			{
				Current.Gui.ErrorMessageBox("Can not store the list because there is some issue with the new name. Please enter a valid and unique name for the list in the text box.");
				return false;
			}

			var newList = new ScatterSymbolList(_view.CurrentScatterSymbolListName, _currentScatterSymbols.Select(node => (IScatterSymbol)node.Tag));

			ScatterSymbolListManager.Instance.TryRegisterInstance(newList, out _doc);
			_isNameOfNewListValid = true;
			_currentScatterSymbols_IsDirty = false;

			Controller_AvailableLists_Initialize();
			View_AvailableLists_Initialize();
			ControllerAndView_CurrentItemsAndName_Initialize();

			return true;
		}

		private void EhCurrentList_Store()
		{
			if (_currentScatterSymbols_IsDirty)
				TryToStoreList();
		}

		private static bool IsItemEditable(Altaxo.Main.IImmutable item)
		{
			if (null == item)
				return false;
			var prop = item.GetType().GetProperty("IsEditable", typeof(bool));
			if (null == prop)
				return false;
			return (bool)prop.GetValue(item, null);
		}

		private void EhCurrentItem_Edit()
		{
			var node = _currentScatterSymbols.FirstSelectedNode;
			var item = node?.Tag as IScatterSymbol;
			if (null == item || !IsItemEditable(item))
				return;

			var controller = (IMVCANController)Current.Gui.GetControllerAndControl(new object[] { item }, typeof(IMVCANController));
			if (null == controller || null == controller.ViewObject)
				return;

			if (true == Current.Gui.ShowDialog(controller, "Edit item"))
			{
				item = (IScatterSymbol)controller.ModelObject;
				node.Text = item.GetType().Name;
				node.Tag = item;
			}
		}

		private void EhCurrentItem_Remove()
		{
			var node = _currentScatterSymbols.FirstSelectedNode;
			if (node != null)
				_currentScatterSymbols.Remove(node);
			SetListDirty();
		}

		private void EhCurrentItem_MoveDown()
		{
			_currentScatterSymbols.MoveSelectedItemsDown();
			SetListDirty();
			_view?.CurrentScatterSymbolList_Initialize(_currentScatterSymbols);
		}

		private void EhCurrentItem_MoveUp()
		{
			_currentScatterSymbols.MoveSelectedItemsUp();
			SetListDirty();
			_view?.CurrentScatterSymbolList_Initialize(_currentScatterSymbols);
		}

		private void EhAvailableItem_AddToCurrent()
		{
			var avNode = _availableScatterSymbolTypes.FirstSelectedNode;
			if (null == avNode)
				return;
			IScatterSymbol newItem = null;
			try
			{
				newItem = (IScatterSymbol)Activator.CreateInstance((Type)avNode.Tag);
			}
			catch (Exception ex)
			{
				Current.Gui.ErrorMessageBox("The new item could not be created, message: " + ex.Message);
			}

			if (null != newItem)
			{
				_currentScatterSymbols.Add(new SelectableListNode(newItem.GetType().Name, newItem, false));
				SetListDirty();
			}
		}

		private void EhAvailableLists_SelectionChanged(NGTreeNode node)
		{
			if (null == node)
				return;

			if (_currentScatterSymbols_IsDirty)
			{
				if (false == AskForDirtyListToSave())
					return;
			}

			// node.Tag contains the list to choose
			_doc = (ScatterSymbolList)node.Tag;

			ControllerAndView_CurrentItemsAndName_Initialize();
		}

		private bool AskForDirtyListToSave()
		{
			if (_currentScatterSymbols_IsDirty)
			{
				for (;;)
				{
					bool? hasToBeStored = Current.Gui.YesNoCancelMessageBox("You selected a new list, but your current list is not stored yet, and all changed would be discarded. Would you like to store your current list now?", "Attention - list not stored!", null);

					if (false == hasToBeStored)
						return true; // true means we can discard the list
					if (null == hasToBeStored)
						return false; // Cancel will end this action unsuccessful
					if (true == TryToStoreList())
						return true;
				}
			}
			return true;
		}

		private void EhCurrentScatterSymbolListName_Changed()
		{
			SetListDirty();
		}

		private void SetListDirty()
		{
			string existingName;
			if (ScatterSymbolListManager.Instance.TryGetGroupByMembers(_currentScatterSymbols.Select(node => (IScatterSymbol)node.Tag), out existingName))
			{
				_currentScatterSymbols_IsDirty = false;
				_isNameOfNewListValid = true;
				_doc = ScatterSymbolListManager.Instance.GetList(existingName);
				_view.CurrentScatterSymbolListName_Initialize(existingName, false, false, "Name can't be changed because list is already stored!");
				_availableListsRootNode.FromHereToLeavesDo(treeNode => treeNode.IsSelected = object.ReferenceEquals(treeNode.Tag, _doc));
			}
			else // this list is not known up to now
			{
				_currentScatterSymbols_IsDirty = true;
				_isNameOfNewListValid = true;
				string name = _view.CurrentScatterSymbolListName;
				_currentScatterSymbols_IsDirty = true;
				if (!ScatterSymbolListManager.Instance.Contains(name) && !string.IsNullOrEmpty(name))
				{
					// is OK, we can use this name
					_view.CurrentScatterSymbolListName_Initialize(name, true, false, "The name is available as new name of the list");
				}
				else if (string.IsNullOrEmpty(name))
				{
					_view.CurrentScatterSymbolListName_Initialize(name, true, true, "Please enter the name of the new list!");
					_isNameOfNewListValid = false;
				}
				else
				{
					_view.CurrentScatterSymbolListName_Initialize(name, true, true, "Please choose another name since this name is already in use!");
					_isNameOfNewListValid = false;
				}
			}
		}

		private static DropReturnData DropFailedReturnData { get { return new DropReturnData { IsCopy = false, IsMove = false }; } }

		#region Drag current symbols

		private void EhCurrentSymbols_DragCancelled()
		{
			_draggedNode = null;
		}

		private void EhCurrentSymbols_DragEnded(bool isCopy, bool isMove)
		{
			if (isMove && _draggedNode != null)
			{
				_currentScatterSymbols.Remove(_draggedNode);
				SetListDirty();
			}

			_draggedNode = null;
		}

		private SelectableListNode _draggedNode;

		private StartDragData EhCurrentSymbols_StartDrag(IEnumerable items)
		{
			_draggedNode = items.OfType<SelectableListNode>().FirstOrDefault();

			return new StartDragData
			{
				Data = _draggedNode.Tag,
				CanCopy = true,
				CanMove = true
			};
		}

		private bool EhCurrentSymbols_CanStartDrag(IEnumerable items)
		{
			var selNode = items.OfType<SelectableListNode>().FirstOrDefault();
			// to start a drag, at least one item must be selected
			return selNode != null;
		}

		#endregion Drag current symbols

		#region Drop onto current symbols

		private DropCanAcceptDataReturnData EhCurrentSymbols_DropCanAcceptData(object data, object nonGuiTargetItem, DragDropRelativeInsertPosition insertPosition, bool isCtrlKeyPressed, bool isShiftKeyPressed)
		{
			// investigate data

			if (data is IScatterSymbol)
			{
				return new DropCanAcceptDataReturnData
				{
					CanCopy = isCtrlKeyPressed,
					CanMove = !isCtrlKeyPressed,
					ItemIsSwallowingData = false
				};
			}
			else if (data is Type)
			{
				return new DropCanAcceptDataReturnData
				{
					CanCopy = true,
					CanMove = false,
					ItemIsSwallowingData = false
				};
			}
			else
			{
				return new DropCanAcceptDataReturnData
				{
					CanCopy = false,
					CanMove = false,
					ItemIsSwallowingData = false
				};
			}
		}

		private DropReturnData EhCurrentSymbols_Drop(object data, object nonGuiTargetItem, DragDropRelativeInsertPosition insertPosition, bool isCtrlKeyPressed, bool isShiftKeyPressed)
		{
			IScatterSymbol droppedSymbol = null;
			if (data is Type)
			{
				object createdObj = null;
				try
				{
					createdObj = System.Activator.CreateInstance((Type)data);
				}
				catch (Exception ex)
				{
					Current.Gui.ErrorMessageBox("This object could not be dropped because it could not be created, message: " + ex.ToString(), "Error");
					return DropFailedReturnData;
				}

				if (!(createdObj is IScatterSymbol))
				{
					return DropFailedReturnData;
				}

				droppedSymbol = (IScatterSymbol)createdObj;
			} // end if data is type
			else if (data is IScatterSymbol)
			{
				droppedSymbol = (IScatterSymbol)data;
			} // end if data is IScatterSymbol
			else
			{
				return DropFailedReturnData;
			}

			int targetIndex = int.MaxValue;
			if (nonGuiTargetItem is SelectableListNode)
			{
				int idx = _currentScatterSymbols.IndexOf((SelectableListNode)nonGuiTargetItem);
				if (idx >= 0 && insertPosition.HasFlag(DragDropRelativeInsertPosition.AfterTargetItem))
					++idx;
				targetIndex = idx;
			}

			var newNode = new SelectableListNode(droppedSymbol.GetType().Name, droppedSymbol, false);
			if (targetIndex >= _currentScatterSymbols.Count)
				_currentScatterSymbols.Add(newNode);
			else
				_currentScatterSymbols.Insert(targetIndex, newNode);

			SetListDirty();

			return new DropReturnData
			{
				IsCopy = isCtrlKeyPressed,
				IsMove = !isCtrlKeyPressed
			};
		}

		#endregion Drop onto current symbols

		#region Drag Available Symbols

		private void EhAvailableSymbols_DragCancelled()
		{
		}

		private void AvailableSymbols_DragEnded(bool isCopy, bool isMove)
		{
		}

		private StartDragData EhAvailableSymbols_StartDrag(IEnumerable items)
		{
			var node = items.OfType<SelectableListNode>().FirstOrDefault();

			return new StartDragData
			{
				Data = node.Tag,
				CanCopy = true,
				CanMove = false
			};
		}

		private bool EhAvailableSymbols_CanStartDrag(IEnumerable items)
		{
			var selNode = items.OfType<SelectableListNode>().FirstOrDefault();
			// to start a drag, at least one item must be selected
			return selNode != null;
		}

		#endregion Drag Available Symbols

		#region Drop onto Available Symbols

		private DropCanAcceptDataReturnData EhAvailableSymbols_DropCanAcceptData(object data, object nonGuiTargetItem, DragDropRelativeInsertPosition insertPosition, bool isCtrlKeyPressed, bool isShiftKeyPressed)
		{
			// when dropping onto available symbols, it's only purpose is to remove some items from the current scatter symbol lists
			// thus the only operation here is move
			return new DropCanAcceptDataReturnData
			{
				CanCopy = false,
				CanMove = true, // we want the item to be removed from the current scatter symbol list
				ItemIsSwallowingData = false
			};
		}

		private DropReturnData EhAvailableSymbols_Drop(object data, object nonGuiTargetItem, DragDropRelativeInsertPosition insertPosition, bool isCtrlKeyPressed, bool isShiftKeyPressed)
		{
			// when dropping onto available symbols, it's only purpose is to remove some items from the current scatter symbol lists
			// thus the only operation here is move

			if (data is IScatterSymbol)
			{
				return new DropReturnData
				{
					IsCopy = false,
					IsMove = true // we want the item to be removed from the current scatter symbol list
				};
			}
			else
			{
				return DropFailedReturnData;
			}
		}

		#endregion Drop onto Available Symbols
	}
}