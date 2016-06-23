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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Altaxo.Gui.Graph3D.Plot.Data
{
	using Altaxo.Collections;
	using Common;
	using GongSolutions.Wpf.DragDrop;
	using System.Collections;
	using System.IO;
	using System.Windows.Data;
	using System.Windows.Input;
	using System.Windows.Markup;
	using System.Windows.Media;
	using System.Xml;

	/// <summary>
	/// Interaction logic for XYPlotDataControl.xaml
	/// </summary>
	public partial class XYZPlotDataControl : UserControl, IXYZColumnPlotDataView
	{
		public event Action TableSelectionChanged;

		public event Action<ColumnTag> Column_AddTo;

		public event Action<ColumnTag> Column_Erase;

		public event Action<int> RangeFromChanged;

		public event Action<int> RangeToChanged;

		public event Action<int> GroupNumberChanged;

		public event CanStartDragDelegate AvailableDataColumns_CanStartDrag;

		public event StartDragDelegate AvailableDataColumns_StartDrag;

		public event DragEndedDelegate AvailableDataColumns_DragEnded;

		public event DragCancelledDelegate AvailableDataColumns_DragCancelled;

		public event CanStartDragDelegate OtherAvailableItems_CanStartDrag;

		public event StartDragDelegate OtherAvailableItems_StartDrag;

		public event DragEndedDelegate OtherAvailableItems_DragEnded;

		public event DragCancelledDelegate OtherAvailableItems_DragCancelled;

		public event DropCanAcceptDataDelegate Column_DropCanAcceptData;

		public event DropDelegate Column_Drop;

		public XYZPlotDataControl()
		{
			InitializeComponent();
		}

		private void EhTables_SelectionChangeCommit(object sender, SelectionChangedEventArgs e)
		{
			if (null != TableSelectionChanged)
			{
				GuiHelper.SynchronizeSelectionFromGui(this._cbTables);
				TableSelectionChanged?.Invoke();
			}
		}

		private List<List<SingleColumnControl>> _columnControls;

		public void TargetColumns_Initialize(
			IEnumerable<Tuple< // list of all groups
			string, // Caption for each group of columns
			IEnumerable<Tuple< // list of column definitions
				ColumnTag, // tag to identify the column and group
				string, // Label of the column
				string, // name of the column,
				string, // tooltip
				ColumnControlState>
			>>> groups)
		{
			_guiTargetColumnsStack.Children.Clear();
			_columnControls = new List<List<SingleColumnControl>>();

			foreach (var group in groups)
			{
				var textBlock = new TextBlock { Text = group.Item1, FontStyle = FontStyles.Italic, FontWeight = FontWeights.Bold };

				_guiTargetColumnsStack.Children.Add(textBlock);
				var groupList = new List<SingleColumnControl>();
				_columnControls.Add(groupList);

				foreach (var col in group.Item2)
				{
					groupList.Add(null);

					var tag = col.Item1;
					var sgc = new SingleColumnControl(tag, col.Item2, col.Item3, col.Item4, (int)col.Item5);
					_guiTargetColumnsStack.Children.Add(sgc);
					_columnControls[tag.GroupNumber][tag.ColumnNumber] = sgc;
				}
			}
		}

		public void Column_Update(ColumnTag tag, string colname, string toolTip, ColumnControlState state)
		{
			var sgc = _columnControls[tag.GroupNumber][tag.ColumnNumber];
			sgc.ColumnText = colname;
			sgc.ToolTipText = toolTip;
			sgc.SetSeverityLevel((int)state);
		}

		private void EhPlotRangeFrom_Validating(object sender, RoutedPropertyChangedEventArgs<int> e)
		{
			RangeFromChanged?.Invoke(this._nudPlotRangeFrom.Value);
		}

		private void EhPlotRangeTo_Validating(object sender, RoutedPropertyChangedEventArgs<int> e)
		{
			RangeToChanged?.Invoke(this.m_nudPlotRangeTo.Value);
		}

		#region IXYColumnPlotDataView

		public void Tables_Initialize(SelectableListNodeList items)
		{
			GuiHelper.Initialize(_cbTables, items);
		}

		public void Columns_Initialize(SelectableListNodeList items)
		{
			GuiHelper.Initialize(_lbColumns, items);
		}

		public void OtherAvailableColumns_Initialize(SelectableListNodeList items)
		{
			GuiHelper.Initialize(_guiOtherAvailableColumns, items);
		}

		public void PlotRangeFrom_Initialize(int from)
		{
			this._nudPlotRangeFrom.Minimum = 0;
			this._nudPlotRangeFrom.Maximum = int.MaxValue;
			this._nudPlotRangeFrom.Value = from;
		}

		public void PlotRangeTo_Initialize(int to)
		{
			this.m_nudPlotRangeTo.Minimum = 0;
			this.m_nudPlotRangeTo.Maximum = int.MaxValue;
			this.m_nudPlotRangeTo.Value = Math.Max(0, to);
		}

		#endregion IXYColumnPlotDataView

		private void EhGroupNumber_Changed(object sender, RoutedPropertyChangedEventArgs<int> e)
		{
			GroupNumberChanged?.Invoke(_guiGroupNumber.Value);
		}

		public void GroupNumber_Initialize(int groupNumber, bool enableControl)
		{
			_guiGroupNumber.Value = groupNumber;
			_guiGroupNumber.IsEnabled = enableControl;
		}

		#region AvailableDataColumns_DragHander

		private IDragSource _availableDataColumnsDragSource;

		public IDragSource AvailableDataColumnsDragSource
		{
			get
			{
				if (null == _otherAvailableColumnsDragSource)
					_availableDataColumnsDragSource = new AvailableDataColumns_DragSource(this);
				return _availableDataColumnsDragSource;
			}
		}

		public class AvailableDataColumns_DragSource : IDragSource
		{
			private XYZPlotDataControl _parentControl;

			public AvailableDataColumns_DragSource(XYZPlotDataControl ctrl)
			{
				_parentControl = ctrl;
			}

			public bool CanStartDrag(IDragInfo dragInfo)
			{
				var result = _parentControl.AvailableDataColumns_CanStartDrag?.Invoke(_parentControl._lbColumns.SelectedItems);
				return result.HasValue ? result.Value : false;
			}

			public void StartDrag(IDragInfo dragInfo)
			{
				var result = _parentControl.AvailableDataColumns_StartDrag?.Invoke(dragInfo.SourceItems);
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
				_parentControl.AvailableDataColumns_DragEnded?.Invoke(isCopy, isMove);
			}

			public void DragCancelled()
			{
				_parentControl.AvailableDataColumns_DragCancelled?.Invoke();
			}
		}

		#endregion AvailableDataColumns_DragHander

		#region OtherAvailableColumns_DragHander

		private IDragSource _otherAvailableColumnsDragSource;

		public IDragSource OtherAvailableColumnsDragSource
		{
			get
			{
				if (null == _otherAvailableColumnsDragSource)
					_otherAvailableColumnsDragSource = new OtherAvailableColumns_DragSource(this);
				return _otherAvailableColumnsDragSource;
			}
		}

		public class OtherAvailableColumns_DragSource : IDragSource
		{
			private XYZPlotDataControl _parentControl;

			public OtherAvailableColumns_DragSource(XYZPlotDataControl ctrl)
			{
				_parentControl = ctrl;
			}

			public bool CanStartDrag(IDragInfo dragInfo)
			{
				var result = _parentControl.OtherAvailableItems_CanStartDrag?.Invoke(_parentControl._guiOtherAvailableColumns.SelectedItems);
				return result.HasValue ? result.Value : false;
			}

			public void StartDrag(IDragInfo dragInfo)
			{
				var result = _parentControl.OtherAvailableItems_StartDrag?.Invoke(dragInfo.SourceItems);
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
				_parentControl.OtherAvailableItems_DragEnded?.Invoke(isCopy, isMove);
			}

			public void DragCancelled()
			{
				_parentControl.OtherAvailableItems_DragCancelled?.Invoke();
			}
		}

		#endregion OtherAvailableColumns_DragHander

		#region Column text boxes drop handler

		private IDropTarget _columTextBoxes_DropTarget;

		public IDropTarget ColumnTextBoxesDropTarget
		{
			get
			{
				if (null == _columTextBoxes_DropTarget)
					_columTextBoxes_DropTarget = new ColumTextBoxes_DropTarget(this);
				return _columTextBoxes_DropTarget;
			}
		}

		public class ColumTextBoxes_DropTarget : IDropTarget
		{
			private XYZPlotDataControl _parentControl;

			public ColumTextBoxes_DropTarget(XYZPlotDataControl ctrl)
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
				var result = _parentControl.Column_DropCanAcceptData?.Invoke(
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
				var result = _parentControl.Column_Drop?.Invoke(
					dropInfo.Data is System.Windows.IDataObject ? GuiHelper.ToAltaxo((System.Windows.IDataObject)dropInfo.Data) : dropInfo.Data,
					(dropInfo.VisualTarget as FrameworkElement)?.Tag,
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

		#endregion Column text boxes drop handler

		#region Column text boxes commands

		#region ColumnAddTo command

		private RelayCommand _columnAddToCommand;

		public ICommand ColumnAddToCommand
		{
			get
			{
				if (this._columnAddToCommand == null)
					this._columnAddToCommand = new RelayCommand(EhColumn_AddToCommand);
				return this._columnAddToCommand;
			}
		}

		private void EhColumn_AddToCommand(object parameter)
		{
			GuiHelper.SynchronizeSelectionFromGui(_lbColumns);
			Column_AddTo?.Invoke(parameter as ColumnTag);
		}

		#endregion ColumnAddTo command

		#region ColumnErase command

		private RelayCommand _columnEraseCommand;

		public ICommand ColumnEraseCommand
		{
			get
			{
				if (this._columnEraseCommand == null)
					this._columnEraseCommand = new RelayCommand(EhColumn_EraseCommand);
				return this._columnEraseCommand;
			}
		}

		private void EhColumn_EraseCommand(object parameter)
		{
			Column_Erase?.Invoke(parameter as ColumnTag);
		}

		#endregion ColumnErase command

		#endregion Column text boxes commands
	}
}