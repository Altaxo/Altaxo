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
using GongSolutions.Wpf.DragDrop;
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

namespace Altaxo.Gui.Graph3D.Plot.Groups
{
	/// <summary>
	/// Interaction logic for StyleListControl.xaml
	/// </summary>
	public partial class StyleListControl : UserControl, IStyleListView
	{
		public event Action<NGTreeNode> AvailableLists_SelectionChanged;

		public event Action CurrentItemListName_Changed;

		public event CanStartDragDelegate AvailableItems_CanStartDrag;

		public event StartDragDelegate AvailableItems_StartDrag;

		public event DragEndedDelegate AvailableItems_DragEnded;

		public event DragCancelledDelegate AvailableItems_DragCancelled;

		public event DropCanAcceptDataDelegate AvailableItems_DropCanAcceptData;

		public event DropDelegate AvailableItems_Drop;

		public event CanStartDragDelegate CurrentItems_CanStartDrag;

		public event StartDragDelegate CurrentItems_StartDrag;

		public event DragEndedDelegate CurrentItems_DragEnded;

		public event DragCancelledDelegate CurrentItems_DragCancelled;

		public event DropCanAcceptDataDelegate CurrentItems_DropCanAcceptData;

		public event DropDelegate CurrentItems_Drop;

		public bool StoreInUserSettings { get { return true == _guiStoreInUserSettings.IsChecked; } }

		public event Action AvailableItem_AddToCurrent;

		public event Action CurrentItem_MoveUp;

		public event Action CurrentItem_MoveDown;

		public event Action CurrentItem_Remove;

		public event Action CurrentItem_Edit;

		public event Action CurrentList_Store;

		public StyleListControl()
		{
			InitializeComponent();
		}

		public void AvailableLists_Initialize(NGTreeNodeCollection nodes)
		{
			_guiAvailableLists.ItemsSource = nodes;
		}

		public void AvailableItems_Initialize(NGTreeNodeCollection items)
		{
			_guiAvailableSymbols.ItemsSource = items;
		}

		public void CurrentItemList_Initialize(SelectableListNodeList items)
		{
			GuiHelper.Initialize(_guiCurrentItems, items);
		}

		public void CurrentItemListName_Initialize(string name, bool isEnabled, bool isMarked, string toolTipText)
		{
			_guiNewListName.Text = name;
			_guiNewListName.ToolTip = toolTipText;

			_guiNewListName.IsReadOnly = !isEnabled;
			_guiStoreInUserSettings.IsEnabled = isEnabled;
			_guiStoreList.IsEnabled = isEnabled;

			if (isMarked)
				_guiNewListName.Background = Brushes.LightPink;
			else
				_guiNewListName.Background = Brushes.White;
		}

		#region AvailableSymbols_DragHander

		private IDragSource _availableItemsDragSource;

		public IDragSource AvailableItemsDragSource
		{
			get
			{
				if (null == _availableItemsDragSource)
					_availableItemsDragSource = new AvailableItems_DragSource(this);
				return _availableItemsDragSource;
			}
		}

		public class AvailableItems_DragSource : IDragSource
		{
			private StyleListControl _parentControl;

			public AvailableItems_DragSource(StyleListControl ctrl)
			{
				_parentControl = ctrl;
			}

			public bool CanStartDrag(IDragInfo dragInfo)
			{
				var result = _parentControl.AvailableItems_CanStartDrag?.Invoke(dragInfo.SourceItems);
				return result.HasValue ? result.Value : false;
			}

			public void StartDrag(IDragInfo dragInfo)
			{
				var result = _parentControl.AvailableItems_StartDrag?.Invoke(dragInfo.SourceItems);
				if (null != result)
				{
					dragInfo.Effects = GuiHelper.ConvertCopyMoveToDragDropEffect(result.Value.CanCopy, result.Value.CanMove);
					dragInfo.Data = result.Value.Data;
				}
			}

			public void Dropped(IDropInfo dropInfo, DragDropEffects effects)
			{
				bool isCopy, isMove;
				GuiHelper.ConvertDragDropEffectToCopyMove(effects, out isCopy, out isMove);
				_parentControl.AvailableItems_DragEnded?.Invoke(isCopy, isMove);
			}

			public void DragCancelled()
			{
				_parentControl.AvailableItems_DragCancelled?.Invoke();
			}
		}

		#endregion AvailableSymbols_DragHander

		#region Available items drop handler

		// this drop handler's only purpose is to get item dragged from the current list onto the available list being removed from the list

		private IDropTarget _availableItems_DropTarget;

		public IDropTarget AvailableItemsDropTarget
		{
			get
			{
				if (null == _availableItems_DropTarget)
					_availableItems_DropTarget = new AvailableItems_DropTarget(this);
				return _availableItems_DropTarget;
			}
		}

		public class AvailableItems_DropTarget : IDropTarget
		{
			private StyleListControl _parentControl;

			public AvailableItems_DropTarget(StyleListControl ctrl)
			{
				_parentControl = ctrl;
			}

			public void DragOver(IDropInfo dropInfo)
			{
				DragDropEffects resultingEffect;
				Type adornerType;
				if (CanAcceptData(dropInfo, out resultingEffect, out adornerType))
				{
					dropInfo.Effects = resultingEffect;
					dropInfo.DropTargetAdorner = adornerType;
				}
			}

			protected bool CanAcceptData(IDropInfo dropInfo, out DragDropEffects resultingEffect, out Type adornerType)
			{
				var result = _parentControl.AvailableItems_DropCanAcceptData?.Invoke(
					dropInfo.Data is System.Windows.IDataObject ? GuiHelper.ToAltaxo((System.Windows.IDataObject)dropInfo.Data) : dropInfo.Data,
					(dropInfo.VisualTarget as FrameworkElement)?.Tag,
					GuiHelper.ToAltaxo(dropInfo.InsertPosition),
					dropInfo.KeyStates.HasFlag(DragDropKeyStates.ControlKey),
					dropInfo.KeyStates.HasFlag(DragDropKeyStates.ShiftKey));

				if (null != result)
				{
					resultingEffect = GuiHelper.ConvertCopyMoveToDragDropEffect(result.Value.CanCopy, result.Value.CanMove);
					adornerType = result.Value.ItemIsSwallowingData ? DropTargetAdorners.Highlight : DropTargetAdorners.Insert;

					return result.Value.CanCopy | result.Value.CanMove;
				}
				else
				{
					resultingEffect = DragDropEffects.None;
					adornerType = null;
					return false;
				}
			}

			public void Drop(IDropInfo dropInfo)
			{
				var result = _parentControl.AvailableItems_Drop?.Invoke(
					dropInfo.Data is System.Windows.IDataObject ? GuiHelper.ToAltaxo((System.Windows.IDataObject)dropInfo.Data) : dropInfo.Data,
					dropInfo.TargetItem,
					GuiHelper.ToAltaxo(dropInfo.InsertPosition),
					dropInfo.KeyStates.HasFlag(DragDropKeyStates.ControlKey),
					dropInfo.KeyStates.HasFlag(DragDropKeyStates.ShiftKey)
					);

				if (null != result)
				{
					dropInfo.Effects = GuiHelper.ConvertCopyMoveToDragDropEffect(result.Value.IsCopy, result.Value.IsMove); // it is important to get back the resulting effect to dropInfo, because dropInfo informs the drag handler about the resulting effect, which can e.g. delete the items after a move operation
				}
			}
		}

		#endregion Available items drop handler

		#region CurrentItems_DragHander

		private IDragSource _currentItemsDragSource;

		public IDragSource CurrentItemsDragSource
		{
			get
			{
				if (null == _currentItemsDragSource)
					_currentItemsDragSource = new CurrentItems_DragSource(this);
				return _currentItemsDragSource;
			}
		}

		public class CurrentItems_DragSource : IDragSource
		{
			private StyleListControl _parentControl;

			public CurrentItems_DragSource(StyleListControl ctrl)
			{
				_parentControl = ctrl;
			}

			public bool CanStartDrag(IDragInfo dragInfo)
			{
				var result = _parentControl.CurrentItems_CanStartDrag?.Invoke(_parentControl._guiCurrentItems.SelectedItems);
				return result.HasValue ? result.Value : false;
			}

			public void StartDrag(IDragInfo dragInfo)
			{
				GuiHelper.SynchronizeSelectionFromGui(_parentControl._guiCurrentItems);
				var result = _parentControl.CurrentItems_StartDrag?.Invoke(dragInfo.SourceItems);
				if (null != result)
				{
					dragInfo.Effects = GuiHelper.ConvertCopyMoveToDragDropEffect(result.Value.CanCopy, result.Value.CanMove);
					dragInfo.Data = result.Value.Data;
				}
			}

			public void Dropped(IDropInfo dropInfo, DragDropEffects effects)
			{
				bool isCopy, isMove;
				GuiHelper.ConvertDragDropEffectToCopyMove(effects, out isCopy, out isMove);
				_parentControl.CurrentItems_DragEnded?.Invoke(isCopy, isMove);
			}

			public void DragCancelled()
			{
				_parentControl.CurrentItems_DragCancelled?.Invoke();
			}
		}

		#endregion CurrentItems_DragHander

		#region Current items drop handler

		private IDropTarget _currentItems_DropTarget;

		public IDropTarget CurrentItemsDropTarget
		{
			get
			{
				if (null == _currentItems_DropTarget)
					_currentItems_DropTarget = new CurrentItems_DropTarget(this);
				return _currentItems_DropTarget;
			}
		}

		public class CurrentItems_DropTarget : IDropTarget
		{
			private StyleListControl _parentControl;

			public CurrentItems_DropTarget(StyleListControl ctrl)
			{
				_parentControl = ctrl;
			}

			public void DragOver(IDropInfo dropInfo)
			{
				DragDropEffects resultingEffect;
				Type adornerType;
				if (CanAcceptData(dropInfo, out resultingEffect, out adornerType))
				{
					dropInfo.Effects = resultingEffect;
					dropInfo.DropTargetAdorner = adornerType;
				}
			}

			protected bool CanAcceptData(IDropInfo dropInfo, out DragDropEffects resultingEffect, out Type adornerType)
			{
				var result = _parentControl.CurrentItems_DropCanAcceptData?.Invoke(
					dropInfo.Data is System.Windows.IDataObject ? GuiHelper.ToAltaxo((System.Windows.IDataObject)dropInfo.Data) : dropInfo.Data,
					dropInfo.TargetItem,
					GuiHelper.ToAltaxo(dropInfo.InsertPosition),
					dropInfo.KeyStates.HasFlag(DragDropKeyStates.ControlKey),
					dropInfo.KeyStates.HasFlag(DragDropKeyStates.ShiftKey));

				if (null != result)
				{
					resultingEffect = GuiHelper.ConvertCopyMoveToDragDropEffect(result.Value.CanCopy, result.Value.CanMove);
					adornerType = result.Value.ItemIsSwallowingData ? DropTargetAdorners.Highlight : DropTargetAdorners.Insert;

					return result.Value.CanCopy | result.Value.CanMove;
				}
				else
				{
					resultingEffect = DragDropEffects.None;
					adornerType = null;
					return false;
				}
			}

			public void Drop(IDropInfo dropInfo)
			{
				var result = _parentControl.CurrentItems_Drop?.Invoke(
					dropInfo.Data is System.Windows.IDataObject ? GuiHelper.ToAltaxo((System.Windows.IDataObject)dropInfo.Data) : dropInfo.Data,
					dropInfo.TargetItem,
					GuiHelper.ToAltaxo(dropInfo.InsertPosition),
					dropInfo.KeyStates.HasFlag(DragDropKeyStates.ControlKey),
					dropInfo.KeyStates.HasFlag(DragDropKeyStates.ShiftKey)
					);

				if (null != result)
				{
					dropInfo.Effects = GuiHelper.ConvertCopyMoveToDragDropEffect(result.Value.IsCopy, result.Value.IsMove); // it is important to get back the resulting effect to dropInfo, because dropInfo informs the drag handler about the resulting effect, which can e.g. delete the items after a move operation
				}
			}
		}

		#endregion Current items drop handler

		private void EhCurrentItemListName_Changed(object sender, TextChangedEventArgs e)
		{
			CurrentItemListName_Changed?.Invoke();
		}

		public string CurrentItemListName { get { return _guiNewListName.Text; } }

		private void EhAvailableList_SelectionChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			AvailableLists_SelectionChanged?.Invoke((NGTreeNode)_guiAvailableLists.SelectedItem);
		}

		private void EhAvailableItem_AddToCurrent(object sender, RoutedEventArgs e)
		{
			GuiHelper.SynchronizeSelectionFromGui(_guiCurrentItems);
			AvailableItem_AddToCurrent?.Invoke();
		}

		private void EhCurrentItem_Remove(object sender, RoutedEventArgs e)
		{
			GuiHelper.SynchronizeSelectionFromGui(_guiCurrentItems);
			CurrentItem_Remove?.Invoke();
		}

		private void EhCurrentItem_MoveUp(object sender, RoutedEventArgs e)
		{
			GuiHelper.SynchronizeSelectionFromGui(_guiCurrentItems);
			CurrentItem_MoveUp?.Invoke();
		}

		private void EhCurrentItem_MoveDown(object sender, RoutedEventArgs e)
		{
			GuiHelper.SynchronizeSelectionFromGui(_guiCurrentItems);
			CurrentItem_MoveDown?.Invoke();
		}

		private void EhCurrentItem_Edit(object sender, RoutedEventArgs e)
		{
			GuiHelper.SynchronizeSelectionFromGui(_guiCurrentItems);
			CurrentItem_Edit?.Invoke();
		}

		private void EhCurrentList_Store(object sender, RoutedEventArgs e)
		{
			CurrentList_Store?.Invoke();
		}
	}
}