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
using Altaxo.Graph;
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
	public interface IStyleListView
	{
		void AvailableLists_Initialize(NGTreeNodeCollection nodes);

		event Action<NGTreeNode> AvailableLists_SelectionChanged;

		event Action CurrentItemListName_Changed;

		void AvailableItems_Initialize(SelectableListNodeList items);

		void CurrentItemList_Initialize(SelectableListNodeList items);

		void CurrentItemListName_Initialize(string name, bool isEnabled, bool isMarked, string toolTipText);

		string CurrentItemListName { get; }

		event CanStartDragDelegate AvailableItems_CanStartDrag;

		event StartDragDelegate AvailableItems_StartDrag;

		event DragEndedDelegate AvailableItems_DragEnded;

		event DragCancelledDelegate AvailableItems_DragCancelled;

		event DropCanAcceptDataDelegate AvailableItems_DropCanAcceptData;

		event DropDelegate AvailableItems_Drop;

		event CanStartDragDelegate CurrentItems_CanStartDrag;

		event StartDragDelegate CurrentItems_StartDrag;

		event DragEndedDelegate CurrentItems_DragEnded;

		event DragCancelledDelegate CurrentItems_DragCancelled;

		event DropCanAcceptDataDelegate CurrentItems_DropCanAcceptData;

		event DropDelegate CurrentItems_Drop;

		bool StoreInUserSettings { get; }

		event Action AvailableItem_AddToCurrent;

		event Action CurrentItem_MoveUp;

		event Action CurrentItem_MoveDown;

		event Action CurrentItem_Remove;

		event Action CurrentItem_Edit;

		event Action CurrentList_Store;
	}

	[ExpectedTypeOfView(typeof(IStyleListView))]
	public class StyleListController<TManager, TList, TItem>
		:
		MVCANControllerEditImmutableDocBase<IStyleList<TItem>, IStyleListView>
		where TItem : class, Altaxo.Main.IImmutable
		where TList : IStyleList<TItem>
		where TManager : IStyleListManager<TList, TItem>
	{
		private TManager _manager;

		private NGTreeNode _availableListsRootNode;
		private SelectableListNodeList _availableItemTypes;
		private SelectableListNodeList _currentItems;

		private bool _currentItems_IsDirty;
		private bool _isNameOfNewListValid;

		public StyleListController(TManager managerInstance)
		{
			_manager = managerInstance;
		}

		protected override void Initialize(bool initData)
		{
			base.Initialize(initData);

			if (initData)
			{
				Controller_AvailableLists_Initialize();

				// Available items
				var itemTypes = Altaxo.Main.Services.ReflectionService.GetNonAbstractSubclassesOf(typeof(TItem));
				_availableItemTypes = new SelectableListNodeList();
				foreach (var type in itemTypes)
					_availableItemTypes.Add(new SelectableListNode(type.Name, type, false));

				// Current items
				_currentItems = new SelectableListNodeList();
				foreach (var sym in _doc)
				{
					_currentItems.Add(new SelectableListNode(sym.GetType().Name, sym, false));
				}
			}

			if (null != _view)
			{
				View_AvailableLists_Initialize();

				_view.AvailableItems_Initialize(_availableItemTypes);
				_view.CurrentItemList_Initialize(_currentItems);
				_view.CurrentItemListName_Initialize(_doc.Name, false, false, "Name can not be changed because list is already stored!");
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

			_view.CurrentItemListName_Changed += EhCurrentItemListName_Changed;

			_view.AvailableItems_CanStartDrag += EhAvailableItems_CanStartDrag;

			_view.AvailableItems_StartDrag += EhAvailableItems_StartDrag;

			_view.AvailableItems_DragEnded += AvailableItems_DragEnded;

			_view.AvailableItems_DragCancelled += EhAvailableItems_DragCancelled;

			_view.AvailableItems_DropCanAcceptData += EhAvailableItems_DropCanAcceptData;

			_view.AvailableItems_Drop += EhAvailableItems_Drop;

			_view.CurrentItems_CanStartDrag += EhCurrentItems_CanStartDrag;

			_view.CurrentItems_StartDrag += EhCurrentItems_StartDrag;

			_view.CurrentItems_DragEnded += EhCurrentItems_DragEnded;

			_view.CurrentItems_DragCancelled += EhCurrentItems_DragCancelled;

			_view.CurrentItems_DropCanAcceptData += EhCurrentItems_DropCanAcceptData;

			_view.CurrentItems_Drop += EhCurrentItems_Drop;

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

			_view.CurrentItemListName_Changed -= EhCurrentItemListName_Changed;

			_view.AvailableItems_CanStartDrag -= EhAvailableItems_CanStartDrag;

			_view.AvailableItems_StartDrag -= EhAvailableItems_StartDrag;

			_view.AvailableItems_DragEnded -= AvailableItems_DragEnded;

			_view.AvailableItems_DragCancelled -= EhAvailableItems_DragCancelled;

			_view.AvailableItems_DropCanAcceptData -= EhAvailableItems_DropCanAcceptData;

			_view.AvailableItems_Drop -= EhAvailableItems_Drop;

			_view.CurrentItems_CanStartDrag -= EhCurrentItems_CanStartDrag;

			_view.CurrentItems_StartDrag -= EhCurrentItems_StartDrag;

			_view.CurrentItems_DragEnded -= EhCurrentItems_DragEnded;

			_view.CurrentItems_DragCancelled -= EhCurrentItems_DragCancelled;

			_view.CurrentItems_DropCanAcceptData -= EhCurrentItems_DropCanAcceptData;

			_view.CurrentItems_Drop -= EhCurrentItems_Drop;

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

			var allNames = _manager.GetAllListNames().ToArray();
			Array.Sort(allNames);
			var dict = new Dictionary<string, NGTreeNode>();

			foreach (var name in allNames)
			{
				var list = _manager.GetList(name);
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
			_currentItems.Clear();
			foreach (var sym in _doc)
			{
				_currentItems.Add(new SelectableListNode(sym.GetType().Name, sym, false));
			}
			_view?.CurrentItemList_Initialize(_currentItems);
			_view?.CurrentItemListName_Initialize(_doc.Name, false, false, "Name can't be changed because list is already stored!");
			_currentItems_IsDirty = false;
		}

		private bool TryToStoreList()
		{
			if (_currentItems.Count == 0)
			{
				Current.Gui.ErrorMessageBox("The list does not contains any items, thus it can not be stored");
				return false;
			}

			if (!_isNameOfNewListValid)
			{
				Current.Gui.ErrorMessageBox("Can not store the list because there is some issue with the new name. Please enter a valid and unique name for the list in the text box.");
				return false;
			}

			bool isUser = _view.StoreInUserSettings;
			_doc = _manager.CreateNewList(_view.CurrentItemListName, _currentItems.Select(node => (TItem)node.Tag), true, isUser ? Altaxo.Main.ItemDefinitionLevel.UserDefined : Altaxo.Main.ItemDefinitionLevel.Project);

			_isNameOfNewListValid = true;
			_currentItems_IsDirty = false;

			Controller_AvailableLists_Initialize();
			View_AvailableLists_Initialize();
			ControllerAndView_CurrentItemsAndName_Initialize();

			return true;
		}

		private void EhCurrentList_Store()
		{
			if (_currentItems_IsDirty)
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
			var node = _currentItems.FirstSelectedNode;
			var item = node?.Tag as TItem;
			if (null == item || !IsItemEditable(item))
				return;

			var controller = (IMVCANController)Current.Gui.GetControllerAndControl(new object[] { item }, typeof(IMVCANController));
			if (null == controller || null == controller.ViewObject)
				return;

			if (true == Current.Gui.ShowDialog(controller, "Edit item"))
			{
				item = (TItem)controller.ModelObject;
				node.Text = item.GetType().Name;
				node.Tag = item;
			}
		}

		private void EhCurrentItem_Remove()
		{
			var node = _currentItems.FirstSelectedNode;
			if (node != null)
				_currentItems.Remove(node);
			SetListDirty();
		}

		private void EhCurrentItem_MoveDown()
		{
			_currentItems.MoveSelectedItemsDown();
			SetListDirty();
			_view?.CurrentItemList_Initialize(_currentItems);
		}

		private void EhCurrentItem_MoveUp()
		{
			_currentItems.MoveSelectedItemsUp();
			SetListDirty();
			_view?.CurrentItemList_Initialize(_currentItems);
		}

		private void EhAvailableItem_AddToCurrent()
		{
			var avNode = _availableItemTypes.FirstSelectedNode;
			if (null == avNode)
				return;
			TItem newItem = default(TItem);
			try
			{
				newItem = (TItem)Activator.CreateInstance((Type)avNode.Tag);
			}
			catch (Exception ex)
			{
				Current.Gui.ErrorMessageBox("The new item could not be created, message: " + ex.Message);
			}

			if (null != newItem)
			{
				_currentItems.Add(new SelectableListNode(newItem.GetType().Name, newItem, false));
				SetListDirty();
			}
		}

		private void EhAvailableLists_SelectionChanged(NGTreeNode node)
		{
			if (null == node)
				return;

			if (_currentItems_IsDirty)
			{
				if (false == AskForDirtyListToSave())
					return;
			}

			// node.Tag contains the list to choose
			_doc = (TList)node.Tag;

			ControllerAndView_CurrentItemsAndName_Initialize();
		}

		private bool AskForDirtyListToSave()
		{
			if (_currentItems_IsDirty)
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

		private void EhCurrentItemListName_Changed()
		{
			SetListDirty();
		}

		private void SetListDirty()
		{
			string existingName;
			if (_manager.TryGetListByMembers(_currentItems.Select(node => (TItem)node.Tag), out existingName))
			{
				_currentItems_IsDirty = false;
				_isNameOfNewListValid = true;
				_doc = _manager.GetList(existingName);
				_view.CurrentItemListName_Initialize(existingName, false, false, "Name can't be changed because list is already stored!");
				_availableListsRootNode.FromHereToLeavesDo(treeNode => treeNode.IsSelected = object.ReferenceEquals(treeNode.Tag, _doc));
			}
			else // this list is not known up to now
			{
				_currentItems_IsDirty = true;
				_isNameOfNewListValid = true;
				string name = _view.CurrentItemListName;
				_currentItems_IsDirty = true;
				if (!_manager.ContainsList(name) && !string.IsNullOrEmpty(name))
				{
					// is OK, we can use this name
					_view.CurrentItemListName_Initialize(name, true, false, "The name is available as new name of the list");
				}
				else if (string.IsNullOrEmpty(name))
				{
					_view.CurrentItemListName_Initialize(name, true, true, "Please enter the name of the new list!");
					_isNameOfNewListValid = false;
				}
				else
				{
					_view.CurrentItemListName_Initialize(name, true, true, "Please choose another name since this name is already in use!");
					_isNameOfNewListValid = false;
				}
			}
		}

		private static DropReturnData DropFailedReturnData { get { return new DropReturnData { IsCopy = false, IsMove = false }; } }

		#region Drag current items

		private void EhCurrentItems_DragCancelled()
		{
			_draggedNode = null;
		}

		private void EhCurrentItems_DragEnded(bool isCopy, bool isMove)
		{
			if (isMove && _draggedNode != null)
			{
				_currentItems.Remove(_draggedNode);
				SetListDirty();
			}

			_draggedNode = null;
		}

		private SelectableListNode _draggedNode;

		private StartDragData EhCurrentItems_StartDrag(IEnumerable items)
		{
			_draggedNode = items.OfType<SelectableListNode>().FirstOrDefault();

			return new StartDragData
			{
				Data = _draggedNode.Tag,
				CanCopy = true,
				CanMove = true
			};
		}

		private bool EhCurrentItems_CanStartDrag(IEnumerable items)
		{
			var selNode = items.OfType<SelectableListNode>().FirstOrDefault();
			// to start a drag, at least one item must be selected
			return selNode != null;
		}

		#endregion Drag current items

		#region Drop onto current items

		private DropCanAcceptDataReturnData EhCurrentItems_DropCanAcceptData(object data, object nonGuiTargetItem, DragDropRelativeInsertPosition insertPosition, bool isCtrlKeyPressed, bool isShiftKeyPressed)
		{
			// investigate data

			if (data is TItem)
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

		private DropReturnData EhCurrentItems_Drop(object data, object nonGuiTargetItem, DragDropRelativeInsertPosition insertPosition, bool isCtrlKeyPressed, bool isShiftKeyPressed)
		{
			TItem droppedItem = default(TItem);
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

				if (!(createdObj is TItem))
				{
					return DropFailedReturnData;
				}

				droppedItem = (TItem)createdObj;
			} // end if data is type
			else if (data is TItem)
			{
				droppedItem = (TItem)data;
			} // end if data is TItem
			else
			{
				return DropFailedReturnData;
			}

			int targetIndex = int.MaxValue;
			if (nonGuiTargetItem is SelectableListNode)
			{
				int idx = _currentItems.IndexOf((SelectableListNode)nonGuiTargetItem);
				if (idx >= 0 && insertPosition.HasFlag(DragDropRelativeInsertPosition.AfterTargetItem))
					++idx;
				targetIndex = idx;
			}

			var newNode = new SelectableListNode(droppedItem.GetType().Name, droppedItem, false);
			if (targetIndex >= _currentItems.Count)
				_currentItems.Add(newNode);
			else
				_currentItems.Insert(targetIndex, newNode);

			SetListDirty();

			return new DropReturnData
			{
				IsCopy = isCtrlKeyPressed,
				IsMove = !isCtrlKeyPressed
			};
		}

		#endregion Drop onto current items

		#region Drag Available items

		private void EhAvailableItems_DragCancelled()
		{
		}

		private void AvailableItems_DragEnded(bool isCopy, bool isMove)
		{
		}

		private StartDragData EhAvailableItems_StartDrag(IEnumerable items)
		{
			var node = items.OfType<SelectableListNode>().FirstOrDefault();

			return new StartDragData
			{
				Data = node.Tag,
				CanCopy = true,
				CanMove = false
			};
		}

		private bool EhAvailableItems_CanStartDrag(IEnumerable items)
		{
			var selNode = items.OfType<SelectableListNode>().FirstOrDefault();
			// to start a drag, at least one item must be selected
			return selNode != null;
		}

		#endregion Drag Available items

		#region Drop onto available items

		private DropCanAcceptDataReturnData EhAvailableItems_DropCanAcceptData(object data, object nonGuiTargetItem, DragDropRelativeInsertPosition insertPosition, bool isCtrlKeyPressed, bool isShiftKeyPressed)
		{
			// when dropping onto available items, it's only purpose is to remove some items from the current item lists
			// thus the only operation here is move
			return new DropCanAcceptDataReturnData
			{
				CanCopy = false,
				CanMove = true, // we want the item to be removed from the current item list
				ItemIsSwallowingData = false
			};
		}

		private DropReturnData EhAvailableItems_Drop(object data, object nonGuiTargetItem, DragDropRelativeInsertPosition insertPosition, bool isCtrlKeyPressed, bool isShiftKeyPressed)
		{
			// when dropping onto available items, it's only purpose is to remove some items from the item lists
			// thus the only operation here is move

			if (data is TItem)
			{
				return new DropReturnData
				{
					IsCopy = false,
					IsMove = true // we want the item to be removed from the current item list
				};
			}
			else
			{
				return DropFailedReturnData;
			}
		}

		#endregion Drop onto available items
	}
}