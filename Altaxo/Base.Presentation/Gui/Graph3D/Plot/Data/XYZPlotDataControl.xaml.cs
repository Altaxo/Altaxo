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
	using GongSolutions.Wpf.DragDrop;
	using System.Collections;
	using System.Windows.Media;

	/// <summary>
	/// Interaction logic for XYPlotDataControl.xaml
	/// </summary>
	public partial class XYZPlotDataControl : UserControl, IXYZColumnPlotDataView
	{
		public event Action TableSelectionChanged;

		public event Action Request_ToX;

		public event Action Request_ToY;

		public event Action Request_ToZ;

		public event Action Request_EraseX;

		public event Action Request_EraseY;

		public event Action Request_EraseZ;

		public event Action<int> RangeFromChanged;

		public event Action<int> RangeToChanged;

		public event Action<int> GroupNumberChanged;

		private Brush DefaultTextboxBackground;

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
			DefaultTextboxBackground = _edXColumn.Background;
		}

		private void EhTables_SelectionChangeCommit(object sender, SelectionChangedEventArgs e)
		{
			if (null != TableSelectionChanged)
			{
				GuiHelper.SynchronizeSelectionFromGui(this._cbTables);
				TableSelectionChanged?.Invoke();
			}
		}

		private void EhToX_Click(object sender, RoutedEventArgs e)
		{
			if (null != Request_ToX)
			{
				GuiHelper.SynchronizeSelectionFromGui(_lbColumns);
				Request_ToX?.Invoke();
			}
		}

		private void EhEraseX_Click(object sender, RoutedEventArgs e)
		{
			Request_EraseX?.Invoke();
		}

		private void EhToY_Click(object sender, RoutedEventArgs e)
		{
			GuiHelper.SynchronizeSelectionFromGui(_lbColumns);
			Request_ToY?.Invoke();
		}

		private void EhEraseY_Click(object sender, RoutedEventArgs e)
		{
			Request_EraseY?.Invoke();
		}

		private void EhToZ_Click(object sender, RoutedEventArgs e)
		{
			GuiHelper.SynchronizeSelectionFromGui(_lbColumns);
			Request_ToZ?.Invoke();
		}

		private void EhEraseZ_Click(object sender, RoutedEventArgs e)
		{
			Request_EraseZ?.Invoke();
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

		private void ColumnBox_Initialize(TextBox box, string colName, string toolTip, ColumnControlState state)
		{
			box.Text = colName;
			box.ToolTip = toolTip;

			switch (state)
			{
				case ColumnControlState.Normal:
					box.Background = DefaultTextboxBackground;
					break;

				case ColumnControlState.Warning:
					box.Background = Brushes.Yellow;
					break;

				case ColumnControlState.Error:
					box.Background = Brushes.LightPink;
					break;
			}
		}

		public void XColumn_Initialize(string colname, string toolTip, ColumnControlState state)
		{
			ColumnBox_Initialize(_edXColumn, colname, toolTip, state);
		}

		public void YColumn_Initialize(string colname, string toolTip, ColumnControlState state)
		{
			ColumnBox_Initialize(_edYColumn, colname, toolTip, state);
		}

		public void ZColumn_Initialize(string colname, string toolTip, ColumnControlState state)
		{
			ColumnBox_Initialize(_edZColumn, colname, toolTip, state);
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
	}
}