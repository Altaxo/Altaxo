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

		event CanStartDragDelegate CurrentSymbols_CanStartDrag;

		event StartDragDelegate CurrentSymbols_StartDrag;

		event DragEndedDelegate CurrentSymbols_DragEnded;

		event DragCancelledDelegate CurrentSymbols_DragCancelled;

		event DropCanAcceptDataDelegate CurrentSymbols_DropCanAcceptData;

		event DropDelegate CurrentSymbols_Drop;
	}

	[ExpectedTypeOfView(typeof(IScatterSymbolListView))]
	[UserControllerForObject(typeof(ScatterSymbolList))]
	public class ScatterSymbolListController : MVCANControllerEditImmutableDocBase<ScatterSymbolList, IScatterSymbolListView>
	{
		private NGTreeNode _availableListsRootNode;
		private SelectableListNodeList _availableScatterSymbolTypes;
		private SelectableListNodeList _currentScatterSymbols;

		private bool _currentScatterSymbols_IsDirty;

		protected override void Initialize(bool initData)
		{
			base.Initialize(initData);

			if (initData)
			{
				_availableListsRootNode = new NGTreeNode();

				var allNames = ScatterSymbolList.GetAllListNames().ToArray();
				Array.Sort(allNames);
				var dict = new Dictionary<string, NGTreeNode>();

				foreach (var name in allNames)
				{
					_availableListsRootNode.Nodes.Add(new NGTreeNode(name) { Tag = ScatterSymbolList.GetList(name) });
				}

				// Available scatter symbols
				var scatterSymbolTypes = Altaxo.Main.Services.ReflectionService.GetNonAbstractSubclassesOf(typeof(IScatterSymbol));
				_availableScatterSymbolTypes = new SelectableListNodeList();
				foreach (var type in scatterSymbolTypes)
					_availableScatterSymbolTypes.Add(new SelectableListNode(type.Name, type, false));

				// Current scatter symbols
				_currentScatterSymbols = new SelectableListNodeList();
				foreach (var sym in _doc)
				{
					_currentScatterSymbols.Add(new SelectableListNode(sym.GetType().Name, sym, false));
				}
			}

			if (null != _view)
			{
				_view.AvailableLists_Initialize(_availableListsRootNode.Nodes);
				_view.AvailableScatterSymbols_Initialize(_availableScatterSymbolTypes);
				_view.CurrentScatterSymbolList_Initialize(_currentScatterSymbols);
				_view.CurrentScatterSymbolListName_Initialize(_doc.Name, false, false, "Name can not be changed because list is already stored!");
			}
		}

		public override bool Apply(bool disposeController)
		{
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

			_view.CurrentSymbols_CanStartDrag += EhCurrentSymbols_CanStartDrag;

			_view.CurrentSymbols_StartDrag += EhCurrentSymbols_StartDrag;

			_view.CurrentSymbols_DragEnded += EhCurrentSymbols_DragEnded;

			_view.CurrentSymbols_DragCancelled += EhCurrentSymbols_DragCancelled;

			_view.CurrentSymbols_DropCanAcceptData += EhCurrentSymbols_DropCanAcceptData;

			_view.CurrentSymbols_Drop += EhCurrentSymbols_Drop;
		}

		protected override void DetachView()
		{
			_view.AvailableLists_SelectionChanged -= EhAvailableLists_SelectionChanged;

			_view.CurrentScatterSymbolListName_Changed -= EhCurrentScatterSymbolListName_Changed;

			_view.AvailableSymbols_CanStartDrag -= EhAvailableSymbols_CanStartDrag;

			_view.AvailableSymbols_StartDrag -= EhAvailableSymbols_StartDrag;

			_view.AvailableSymbols_DragEnded -= AvailableSymbols_DragEnded;

			_view.AvailableSymbols_DragCancelled -= EhAvailableSymbols_DragCancelled;

			_view.CurrentSymbols_CanStartDrag -= EhCurrentSymbols_CanStartDrag;

			_view.CurrentSymbols_StartDrag -= EhCurrentSymbols_StartDrag;

			_view.CurrentSymbols_DragEnded -= EhCurrentSymbols_DragEnded;

			_view.CurrentSymbols_DragCancelled -= EhCurrentSymbols_DragCancelled;

			_view.CurrentSymbols_DropCanAcceptData -= EhCurrentSymbols_DropCanAcceptData;

			_view.CurrentSymbols_Drop -= EhCurrentSymbols_Drop;

			base.DetachView();
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

			_currentScatterSymbols.Clear();
			foreach (var sym in _doc)
			{
				_currentScatterSymbols.Add(new SelectableListNode(sym.GetType().Name, sym, false));
			}
			_view?.CurrentScatterSymbolList_Initialize(_currentScatterSymbols);
			_view?.CurrentScatterSymbolListName_Initialize(_doc.Name, false, false, "Name can't be changed because list is already stored!");
			_currentScatterSymbols_IsDirty = false;
		}

		private bool AskForDirtyListToSave()
		{
			return true;
		}

		private void EhCurrentScatterSymbolListName_Changed()
		{
			SetListDirty();
		}

		private void SetListDirty()
		{
			string existingName;
			if (ScatterSymbolList.TryGetGroupByMembers(_currentScatterSymbols.Select(node => (IScatterSymbol)node.Tag), out existingName))
			{
				_currentScatterSymbols_IsDirty = false;
				_doc = ScatterSymbolList.GetList(existingName);
				_view.CurrentScatterSymbolListName_Initialize(existingName, false, false, "Name can't be changed because list is already stored!");
				_availableListsRootNode.FromHereToLeavesDo(treeNode => treeNode.IsSelected = object.ReferenceEquals(treeNode.Tag, _doc));
			}
			else // this list is not known up to now
			{
				_currentScatterSymbols_IsDirty = true;
				string name = _view.CurrentScatterSymbolListName;
				_currentScatterSymbols_IsDirty = true;
				if (!ScatterSymbolList.Contains(name) && !string.IsNullOrEmpty(name))
				{
					// is OK, we can use this name
					_view.CurrentScatterSymbolListName_Initialize(name, true, false, "The name is available as new name of the list");
				}
				else if (string.IsNullOrEmpty(name))
				{
					_view.CurrentScatterSymbolListName_Initialize(name, true, true, "Please enter the name of the new list!");
				}
				else
				{
					_view.CurrentScatterSymbolListName_Initialize(name, true, true, "Please choose another name since this name is already in use!");
				}
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

			var newNode = new SelectableListNode(droppedSymbol.GetType().ToString(), droppedSymbol, false);
			if (targetIndex >= _currentScatterSymbols.Count)
				_currentScatterSymbols.Add(newNode);
			else
				_currentScatterSymbols.Insert(targetIndex, newNode);

			SetListDirty();

			return new DropReturnData
			{
				IsCopy = true,
				IsMove = false
			};
		}

		private static DropReturnData DropFailedReturnData { get { return new DropReturnData { IsCopy = false, IsMove = false }; } }

		private DropCanAcceptDataReturnData EhCurrentSymbols_DropCanAcceptData(object data, object nonGuiTargetItem, DragDropRelativeInsertPosition insertPosition, bool isCtrlKeyPressed, bool isShiftKeyPressed)
		{
			// investigate data

			return new DropCanAcceptDataReturnData
			{
				CanCopy = true,
				CanMove = true,
				ItemIsSwallowingData = false
			};
		}

		private void EhCurrentSymbols_DragCancelled()
		{
		}

		private void EhCurrentSymbols_DragEnded(bool isCopy, bool isMove)
		{
		}

		private StartDragData EhCurrentSymbols_StartDrag(IEnumerable items)
		{
			var node = items.OfType<SelectableListNode>().FirstOrDefault();

			return new StartDragData
			{
				Data = node.Tag,
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
	}
}