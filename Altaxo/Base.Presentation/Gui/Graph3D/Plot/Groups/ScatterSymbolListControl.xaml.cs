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
	/// Interaction logic for ScatterSymbolListControl.xaml
	/// </summary>
	public partial class ScatterSymbolListControl : UserControl, IScatterSymbolListView
	{
		public event Action<NGTreeNode> AvailableLists_SelectionChanged;

		public event Action CurrentScatterSymbolListName_Changed;

		public event CanStartDragDelegate AvailableSymbols_CanStartDrag;

		public event StartDragDelegate AvailableSymbols_StartDrag;

		public event DragEndedDelegate AvailableSymbols_DragEnded;

		public event DragCancelledDelegate AvailableSymbols_DragCancelled;

		public event CanStartDragDelegate CurrentSymbols_CanStartDrag;

		public event StartDragDelegate CurrentSymbols_StartDrag;

		public event DragEndedDelegate CurrentSymbols_DragEnded;

		public event DragCancelledDelegate CurrentSymbols_DragCancelled;

		public event DropCanAcceptDataDelegate CurrentSymbols_DropCanAcceptData;

		public event DropDelegate CurrentSymbols_Drop;

		public ScatterSymbolListControl()
		{
			InitializeComponent();
		}

		public void AvailableLists_Initialize(NGTreeNodeCollection nodes)
		{
			_guiAvailableLists.ItemsSource = nodes;
		}

		public void AvailableScatterSymbols_Initialize(SelectableListNodeList items)
		{
			GuiHelper.Initialize(_guiAvailableSymbols, items);
		}

		public void CurrentScatterSymbolList_Initialize(SelectableListNodeList items)
		{
			GuiHelper.Initialize(_guiCurrentItems, items);
		}

		public void CurrentScatterSymbolListName_Initialize(string name, bool isEnabled, bool isMarked, string toolTipText)
		{
			_guiNewListName.Text = name;
			_guiNewListName.ToolTip = toolTipText;

			_guiNewListName.IsReadOnly = !isEnabled;

			if (isMarked)
				_guiNewListName.Background = Brushes.LightPink;
			else
				_guiNewListName.Background = Brushes.White;
		}

		#region AvailableSymbols_DragHander

		private IDragSource _availableSymbolsDragSource;

		public IDragSource AvailableSymbolsDragSource
		{
			get
			{
				if (null == _availableSymbolsDragSource)
					_availableSymbolsDragSource = new AvailableSymbols_DragSource(this);
				return _availableSymbolsDragSource;
			}
		}

		public class AvailableSymbols_DragSource : IDragSource
		{
			private ScatterSymbolListControl _parentControl;

			public AvailableSymbols_DragSource(ScatterSymbolListControl ctrl)
			{
				_parentControl = ctrl;
			}

			public bool CanStartDrag(IDragInfo dragInfo)
			{
				var result = _parentControl.AvailableSymbols_CanStartDrag?.Invoke(_parentControl._guiAvailableSymbols.SelectedItems);
				return result.HasValue ? result.Value : false;
			}

			public void StartDrag(IDragInfo dragInfo)
			{
				GuiHelper.SynchronizeSelectionFromGui(_parentControl._guiAvailableSymbols);
				var result = _parentControl.AvailableSymbols_StartDrag?.Invoke(dragInfo.SourceItems);
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
				_parentControl.AvailableSymbols_DragEnded?.Invoke(isCopy, isMove);
			}

			public void DragCancelled()
			{
				_parentControl.AvailableSymbols_DragCancelled?.Invoke();
			}
		}

		#endregion AvailableSymbols_DragHander

		#region CurrentSymbols_DragHander

		private IDragSource _currentSymbolsDragSource;

		public IDragSource CurrentSymbolsDragSource
		{
			get
			{
				if (null == _currentSymbolsDragSource)
					_currentSymbolsDragSource = new CurrentSymbols_DragSource(this);
				return _currentSymbolsDragSource;
			}
		}

		public class CurrentSymbols_DragSource : IDragSource
		{
			private ScatterSymbolListControl _parentControl;

			public CurrentSymbols_DragSource(ScatterSymbolListControl ctrl)
			{
				_parentControl = ctrl;
			}

			public bool CanStartDrag(IDragInfo dragInfo)
			{
				var result = _parentControl.CurrentSymbols_CanStartDrag?.Invoke(_parentControl._guiCurrentItems.SelectedItems);
				return result.HasValue ? result.Value : false;
			}

			public void StartDrag(IDragInfo dragInfo)
			{
				GuiHelper.SynchronizeSelectionFromGui(_parentControl._guiCurrentItems);
				var result = _parentControl.CurrentSymbols_StartDrag?.Invoke(dragInfo.SourceItems);
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
				_parentControl.CurrentSymbols_DragEnded?.Invoke(isCopy, isMove);
			}

			public void DragCancelled()
			{
				_parentControl.CurrentSymbols_DragCancelled?.Invoke();
			}
		}

		#endregion CurrentSymbols_DragHander

		#region Current symbols drop handler

		private IDropTarget _currentSymbols_DropTarget;

		public IDropTarget CurrentSymbolsDropTarget
		{
			get
			{
				if (null == _currentSymbols_DropTarget)
					_currentSymbols_DropTarget = new CurrentSymbols_DropTarget(this);
				return _currentSymbols_DropTarget;
			}
		}

		public class CurrentSymbols_DropTarget : IDropTarget
		{
			private ScatterSymbolListControl _parentControl;

			public CurrentSymbols_DropTarget(ScatterSymbolListControl ctrl)
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
				var result = _parentControl.CurrentSymbols_DropCanAcceptData?.Invoke(
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
				var result = _parentControl.CurrentSymbols_Drop?.Invoke(
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

		#endregion Current symbols drop handler

		private void EhCurrentSymbolListName_Changed(object sender, TextChangedEventArgs e)
		{
			CurrentScatterSymbolListName_Changed?.Invoke();
		}

		public string CurrentScatterSymbolListName { get { return _guiNewListName.Text; } }

		private void EhAvailableList_SelectionChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			AvailableLists_SelectionChanged?.Invoke((NGTreeNode)_guiAvailableLists.SelectedItem);
		}
	}
}