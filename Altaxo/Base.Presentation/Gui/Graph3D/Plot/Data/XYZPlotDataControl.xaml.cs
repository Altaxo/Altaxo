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
		public event Action SelectedTableChanged;

		public event Action<PlotColumnTag> PlotItemColumn_AddTo;

		public event Action<PlotColumnTag> PlotItemColumn_Edit;

		public event Action<PlotColumnTag> PlotItemColumn_Erase;

		public event Action<PlotColumnTag> OtherAvailableColumn_AddTo;

		public event Action<PlotColumnTag> Transformation_AddTo;

		public event Action<PlotColumnTag> Transformation_AddAsSingle;

		public event Action<PlotColumnTag> Transformation_AddAsPrepending;

		public event Action<PlotColumnTag> Transformation_AddAsAppending;

		public event Action<PlotColumnTag> Transformation_Edit;

		public event Action<PlotColumnTag> Transformation_Erase;

		public event Action<int> RangeFromChanged;

		public event Action<int> RangeToChanged;

		public event Action<int> SelectedGroupNumberChanged;

		public event Action SelectedMatchingTableChanged;

		public event CanStartDragDelegate AvailableTableColumns_CanStartDrag;

		public event StartDragDelegate AvailableTableColumns_StartDrag;

		public event DragEndedDelegate AvailableTableColumns_DragEnded;

		public event DragCancelledDelegate AvailableTableColumns_DragCancelled;

		public event CanStartDragDelegate OtherAvailableItems_CanStartDrag;

		public event StartDragDelegate OtherAvailableItems_StartDrag;

		public event DragEndedDelegate OtherAvailableItems_DragEnded;

		public event DragCancelledDelegate OtherAvailableItems_DragCancelled;

		public event CanStartDragDelegate AvailableTransformations_CanStartDrag;

		public event StartDragDelegate AvailableTransformations_StartDrag;

		public event DragEndedDelegate AvailableTransformations_DragEnded;

		public event DragCancelledDelegate AvailableTransformations_DragCancelled;

		public event DropCanAcceptDataDelegate PlotItemColumn_DropCanAcceptData;

		public event DropDelegate PlotItemColumn_Drop;

		public XYZPlotDataControl()
		{
			InitializeComponent();
		}

		private void EhTables_SelectionChangeCommit(object sender, SelectionChangedEventArgs e)
		{
			if (null != SelectedTableChanged)
			{
				GuiHelper.SynchronizeSelectionFromGui(this._cbTables);
				SelectedTableChanged?.Invoke();
			}
		}

		private List<List<SingleColumnControl>> _columnControls;

		public void PlotColumns_Initialize(
			IEnumerable<Tuple< // list of all groups
			string, // Caption for each group of columns
			IEnumerable<Tuple< // list of column definitions
				PlotColumnTag, // tag to identify the column and group
				string>
			>>> groups)
		{
			_guiTargetColumnsStack.Children.Clear();
			_columnControls = new List<List<SingleColumnControl>>();

			foreach (var group in groups)
			{
				var groupBox = new GroupBox();
				var stackPanel = new StackPanel() { Orientation = Orientation.Vertical };
				groupBox.Content = stackPanel;

				var textBlock = new TextBlock { Text = group.Item1, FontStyle = FontStyles.Italic, FontWeight = FontWeights.Bold };
				groupBox.Header = textBlock;

				_guiTargetColumnsStack.Children.Add(groupBox);
				var groupList = new List<SingleColumnControl>();
				_columnControls.Add(groupList);

				foreach (var col in group.Item2)
				{
					groupList.Add(null);

					var tag = col.Item1;
					var sgc = new SingleColumnControl(tag, col.Item2);
					//_guiTargetColumnsStack.Children.Add(sgc);
					stackPanel.Children.Add(sgc);
					_columnControls[tag.GroupNumber][tag.ColumnNumber] = sgc;
				}
			}
		}

		public void PlotColumn_Update(PlotColumnTag tag, string colname, string toolTip, string transformationText, string transformationToolTip, PlotColumnControlState state)
		{
			var sgc = _columnControls[tag.GroupNumber][tag.ColumnNumber];
			sgc.ColumnText = colname;
			sgc.ToolTipText = toolTip;
			sgc.TransformationText = transformationText;
			sgc.TransformationToolTipText = transformationToolTip;
			sgc.SetSeverityLevel((int)state);
		}

		public void ShowTransformationSinglePrependAppendPopup(PlotColumnTag tag)
		{
			var sgc = _columnControls[tag.GroupNumber][tag.ColumnNumber];
			sgc.ShowTransformationSinglePrependAppendPopup(true);
		}

		private void EhPlotRangeFrom_Validating(object sender, RoutedPropertyChangedEventArgs<int> e)
		{
			RangeFromChanged?.Invoke(this._guiPlotRangeFrom.Value);
		}

		private void EhPlotRangeTo_Validating(object sender, RoutedPropertyChangedEventArgs<int> e)
		{
			RangeToChanged?.Invoke(this._guiPlotRangeTo.Value);
		}

		#region IXYColumnPlotDataView

		public void AvailableTables_Initialize(SelectableListNodeList items)
		{
			GuiHelper.Initialize(_cbTables, items);
		}

		public void MatchingTables_Initialize(SelectableListNodeList items)
		{
			GuiHelper.InitializeDeselectable(_guiFittingTables, items);
		}

		public void AvailableTableColumns_Initialize(NGTreeNodeCollection nodes)
		{
			_guiAvailableTableColumns.ItemsSource = nodes;
		}

		public object AvailableTableColumns_SelectedItem
		{
			get
			{
				return _guiAvailableTableColumns.SelectedItem;
			}
		}

		public void OtherAvailableColumns_Initialize(SelectableListNodeList items)
		{
			GuiHelper.Initialize(_guiOtherAvailableColumns, items);
		}

		public void AvailableTransformations_Initialize(SelectableListNodeList items)
		{
			GuiHelper.Initialize(_guiAvailableTransformations, items);
		}

		public void PlotRangeFrom_Initialize(int from)
		{
			this._guiPlotRangeFrom.Minimum = 0;
			this._guiPlotRangeFrom.Maximum = int.MaxValue;
			this._guiPlotRangeFrom.Value = from;
		}

		public void PlotRangeTo_Initialize(int to)
		{
			this._guiPlotRangeTo.Minimum = 0;
			this._guiPlotRangeTo.Maximum = int.MaxValue;
			this._guiPlotRangeTo.Value = Math.Max(0, to);
		}

		#endregion IXYColumnPlotDataView

		private void EhGroupNumber_Changed(object sender, RoutedPropertyChangedEventArgs<int> e)
		{
			SelectedGroupNumberChanged?.Invoke(_guiGroupNumber.Value);
		}

		public void GroupNumber_Initialize(IEnumerable<int> groupNumbers, int groupNumber, bool enableControl)
		{
			_guiGroupNumber.AvailableValues = groupNumbers;
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
				var result = _parentControl.AvailableTableColumns_CanStartDrag?.Invoke(new[] { _parentControl._guiAvailableTableColumns.SelectedItem });
				return result.HasValue ? result.Value : false;
			}

			public void StartDrag(IDragInfo dragInfo)
			{
				//GuiHelper.SynchronizeSelectionFromGui(_parentControl._guiAvailableTableColumns);
				var result = _parentControl.AvailableTableColumns_StartDrag?.Invoke(dragInfo.SourceItems);
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
				_parentControl.AvailableTableColumns_DragEnded?.Invoke(isCopy, isMove);
			}

			public void DragCancelled()
			{
				_parentControl.AvailableTableColumns_DragCancelled?.Invoke();
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
				GuiHelper.SynchronizeSelectionFromGui(_parentControl._guiOtherAvailableColumns);
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

		#region AvailableTransformations_DragHander

		private IDragSource _availableTransformationsDragSource;

		public IDragSource AvailableTransformationsDragSource
		{
			get
			{
				if (null == _availableTransformationsDragSource)
					_availableTransformationsDragSource = new AvailableTransformations_DragSource(this);
				return _availableTransformationsDragSource;
			}
		}

		public class AvailableTransformations_DragSource : IDragSource
		{
			private XYZPlotDataControl _parentControl;

			public AvailableTransformations_DragSource(XYZPlotDataControl ctrl)
			{
				_parentControl = ctrl;
			}

			public bool CanStartDrag(IDragInfo dragInfo)
			{
				var result = _parentControl.AvailableTransformations_CanStartDrag?.Invoke(_parentControl._guiAvailableTransformations.SelectedItems);
				return result.HasValue ? result.Value : false;
			}

			public void StartDrag(IDragInfo dragInfo)
			{
				GuiHelper.SynchronizeSelectionFromGui(_parentControl._guiAvailableTransformations);
				var result = _parentControl.AvailableTransformations_StartDrag?.Invoke(dragInfo.SourceItems);
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
				_parentControl.AvailableTransformations_DragEnded?.Invoke(isCopy, isMove);
			}

			public void DragCancelled()
			{
				_parentControl.AvailableTransformations_DragCancelled?.Invoke();
			}
		}

		#endregion AvailableTransformations_DragHander

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
				var result = _parentControl.PlotItemColumn_DropCanAcceptData?.Invoke(
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
				var result = _parentControl.PlotItemColumn_Drop?.Invoke(
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
			FrameworkElement listBox = _lastListBoxActivated ?? _guiAvailableTableColumns;
			if (listBox is ListBox)
				GuiHelper.SynchronizeSelectionFromGui((ListBox)listBox);

			if (object.ReferenceEquals(listBox, _guiAvailableTableColumns))
				PlotItemColumn_AddTo?.Invoke(parameter as PlotColumnTag);
			else if (object.ReferenceEquals(listBox, _guiOtherAvailableColumns))
				OtherAvailableColumn_AddTo?.Invoke(parameter as PlotColumnTag);
			if (object.ReferenceEquals(listBox, _guiAvailableTransformations))
				Transformation_AddTo?.Invoke(parameter as PlotColumnTag);
		}

		#endregion ColumnAddTo command

		#region ColumnEdit command

		private RelayCommand _columnEditCommand;

		public ICommand ColumnEditCommand
		{
			get
			{
				if (this._columnEditCommand == null)
					this._columnEditCommand = new RelayCommand(EhColumn_EditCommand);
				return this._columnEditCommand;
			}
		}

		private void EhColumn_EditCommand(object parameter)
		{
			PlotItemColumn_Edit?.Invoke(parameter as PlotColumnTag);
		}

		#endregion ColumnEdit command

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
			PlotItemColumn_Erase?.Invoke(parameter as PlotColumnTag);
		}

		#endregion ColumnErase command

		#region TransformationEdit command

		private RelayCommand _transformationEditCommand;

		public ICommand TransformationEditCommand
		{
			get
			{
				if (this._transformationEditCommand == null)
					this._transformationEditCommand = new RelayCommand(EhTransformation_EditCommand);
				return this._transformationEditCommand;
			}
		}

		private void EhTransformation_EditCommand(object parameter)
		{
			Transformation_Edit?.Invoke(parameter as PlotColumnTag);
		}

		#endregion TransformationEdit command

		#region TransformationErase command

		private RelayCommand _transformationEraseCommand;

		public ICommand TransformationEraseCommand
		{
			get
			{
				if (this._transformationEraseCommand == null)
					this._transformationEraseCommand = new RelayCommand(EhTransformation_EraseCommand);
				return this._transformationEraseCommand;
			}
		}

		private void EhTransformation_EraseCommand(object parameter)
		{
			Transformation_Erase?.Invoke(parameter as PlotColumnTag);
		}

		#endregion TransformationErase command

		#region TransformationAddAsSingle command

		private RelayCommand _transformationAddAsSingleCommand;

		public ICommand TransformationAddAsSingleCommand
		{
			get
			{
				if (this._transformationAddAsSingleCommand == null)
					this._transformationAddAsSingleCommand = new RelayCommand(EhTransformationAddAsSingleCommand);
				return _transformationAddAsSingleCommand;
			}
		}

		private void EhTransformationAddAsSingleCommand(object parameter)
		{
			var tag = (PlotColumnTag)parameter;
			var sgc = _columnControls[tag.GroupNumber][tag.ColumnNumber];
			sgc.ShowTransformationSinglePrependAppendPopup(false);

			Transformation_AddAsSingle?.Invoke(tag);
		}

		#endregion TransformationAddAsSingle command

		#region TransformationAddAsPrepending command

		private RelayCommand _transformationAddAsPrependingCommand;

		public ICommand TransformationAddAsPrependingCommand
		{
			get
			{
				if (this._transformationAddAsPrependingCommand == null)
					this._transformationAddAsPrependingCommand = new RelayCommand(EhTransformationAddAsPrependingCommand);
				return _transformationAddAsPrependingCommand;
			}
		}

		private void EhTransformationAddAsPrependingCommand(object parameter)
		{
			var tag = (PlotColumnTag)parameter;
			var sgc = _columnControls[tag.GroupNumber][tag.ColumnNumber];
			sgc.ShowTransformationSinglePrependAppendPopup(false);

			Transformation_AddAsPrepending?.Invoke(tag);
		}

		#endregion TransformationAddAsPrepending command

		#region TransformationAddAsAppending command

		private RelayCommand _transformationAddAsAppendingCommand;

		public ICommand TransformationAddAsAppendingCommand
		{
			get
			{
				if (this._transformationAddAsAppendingCommand == null)
					this._transformationAddAsAppendingCommand = new RelayCommand(EhTransformationAddAsAppendingCommand);
				return _transformationAddAsAppendingCommand;
			}
		}

		private void EhTransformationAddAsAppendingCommand(object parameter)
		{
			var tag = (PlotColumnTag)parameter;
			var sgc = _columnControls[tag.GroupNumber][tag.ColumnNumber];
			sgc.ShowTransformationSinglePrependAppendPopup(false);
			Transformation_AddAsAppending?.Invoke(tag);
		}

		#endregion TransformationAddAsAppending command

		#endregion Column text boxes commands

		private FrameworkElement _lastListBoxActivated;

		/// <summary>
		/// To decide which of the lists was the last one activated.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
		private void EhFocusWithinChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if (true == (bool)e.NewValue)
			{
				if (
					object.ReferenceEquals(_guiAvailableTableColumns, sender) ||
					object.ReferenceEquals(_guiOtherAvailableColumns, sender) ||
					object.ReferenceEquals(_guiAvailableTransformations, sender)
					)
					_lastListBoxActivated = (FrameworkElement)sender;
			}
		}

		private void EhFittingTables_SelectionChangeCommit(object sender, SelectionChangedEventArgs e)
		{
			GuiHelper.SynchronizeSelectionFromGui(_guiFittingTables);
			SelectedMatchingTableChanged?.Invoke();
		}
	}
}