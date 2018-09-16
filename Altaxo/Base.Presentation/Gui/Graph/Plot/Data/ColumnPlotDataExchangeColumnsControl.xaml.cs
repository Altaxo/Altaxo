#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2018 Dr. Dirk Lellinger
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

namespace Altaxo.Gui.Graph.Plot.Data
{
  using System.Collections;
  using System.IO;
  using System.Windows.Data;
  using System.Windows.Input;
  using System.Windows.Markup;
  using System.Windows.Media;
  using System.Xml;
  using Altaxo.Collections;
  using Common;
  using GongSolutions.Wpf.DragDrop;
  using Graph.Plot.Data;

  /// <summary>
  /// Interaction logic for ColumnPlotDataControl.xaml
  /// </summary>
  public partial class ColumnPlotDataExchangeColumnsControl : UserControl, IColumnPlotDataExchangeColumnsView
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

    private ItemsControl _guiAvailableTableColumnsCurrentlyActive;

    public ColumnPlotDataExchangeColumnsControl()
    {
      InitializeComponent();
    }

    private List<List<SingleColumnControl>> _columnControls;

    public void PlotColumns_Initialize(
      IEnumerable<( // list of all groups
      string GroupName, // Caption for each group of columns
      IEnumerable<( // list of column definitions
        PlotColumnTag PlotColumnTag, // tag to identify the column and group
        string ColumnLabel)
      >)> groups)
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

    #region IXYColumnPlotDataView

    public void AvailableTableColumns_Initialize(NGTreeNodeCollection nodes)
    {
      bool isTreeWithSubnodes = nodes.Count > 0 && nodes[0].HasChilds;

      if (isTreeWithSubnodes)
      {
        _guiAvailableTableColumnsList.ItemsSource = null;
        _guiAvailableTableColumnsList.Visibility = Visibility.Hidden;

        _guiAvailableTableColumnsTree.ItemsSource = nodes;
        _guiAvailableTableColumnsTree.Visibility = Visibility.Visible;

        _guiAvailableTableColumnsCurrentlyActive = _guiAvailableTableColumnsTree;
      }
      else
      {
        _guiAvailableTableColumnsTree.ItemsSource = null;
        _guiAvailableTableColumnsTree.Visibility = Visibility.Hidden;

        _guiAvailableTableColumnsList.ItemsSource = nodes;
        _guiAvailableTableColumnsList.Visibility = Visibility.Visible;

        _guiAvailableTableColumnsCurrentlyActive = _guiAvailableTableColumnsList;
      }
    }

    public object AvailableTableColumns_SelectedItem
    {
      get
      {
        if (object.ReferenceEquals(_guiAvailableTableColumnsCurrentlyActive, _guiAvailableTableColumnsList))
          return _guiAvailableTableColumnsList.SelectedItem;
        else if (object.ReferenceEquals(_guiAvailableTableColumnsCurrentlyActive, _guiAvailableTableColumnsTree))
          return _guiAvailableTableColumnsTree.SelectedItem;
        else
          return null;
      }
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
        if (null == _availableDataColumnsDragSource)
          _availableDataColumnsDragSource = new AvailableDataColumns_DragSource(this);
        return _availableDataColumnsDragSource;
      }
    }

    public class AvailableDataColumns_DragSource : IDragSource
    {
      private ColumnPlotDataExchangeColumnsControl _parentControl;

      public AvailableDataColumns_DragSource(ColumnPlotDataExchangeColumnsControl ctrl)
      {
        _parentControl = ctrl;
      }

      public bool CanStartDrag(IDragInfo dragInfo)
      {
        var result = _parentControl.AvailableTableColumns_CanStartDrag?.Invoke(new[] { _parentControl.AvailableTableColumns_SelectedItem });
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
        GuiHelper.ConvertDragDropEffectToCopyMove(effects, out var isCopy, out var isMove);
        _parentControl.AvailableTableColumns_DragEnded?.Invoke(isCopy, isMove);
      }

      public void DragCancelled()
      {
        _parentControl.AvailableTableColumns_DragCancelled?.Invoke();
      }
    }

    #endregion AvailableDataColumns_DragHander

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
      private ColumnPlotDataExchangeColumnsControl _parentControl;

      public ColumTextBoxes_DropTarget(ColumnPlotDataExchangeColumnsControl ctrl)
      {
        _parentControl = ctrl;
      }

      public void DragOver(IDropInfo dropInfo)
      {
        if (CanAcceptData(dropInfo, out var resultingEffect, out var adornerType))
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
        if (_columnAddToCommand == null)
          _columnAddToCommand = new RelayCommand(EhColumn_AddToCommand);
        return _columnAddToCommand;
      }
    }

    private void EhColumn_AddToCommand(object parameter)
    {
      FrameworkElement itemsControl = _lastItemsControlActivated ?? _guiAvailableTableColumnsCurrentlyActive;

      if (object.ReferenceEquals(itemsControl, _guiAvailableTableColumnsList))
      {
        PlotItemColumn_AddTo?.Invoke(parameter as PlotColumnTag);
      }
      else if (object.ReferenceEquals(itemsControl, _guiAvailableTableColumnsTree))
      {
        PlotItemColumn_AddTo?.Invoke(parameter as PlotColumnTag);
      }
    }

    #endregion ColumnAddTo command

    #region ColumnEdit command

    private RelayCommand _columnEditCommand;

    public ICommand ColumnEditCommand
    {
      get
      {
        if (_columnEditCommand == null)
          _columnEditCommand = new RelayCommand(EhColumn_EditCommand);
        return _columnEditCommand;
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
        if (_columnEraseCommand == null)
          _columnEraseCommand = new RelayCommand(EhColumn_EraseCommand);
        return _columnEraseCommand;
      }
    }

    private void EhColumn_EraseCommand(object parameter)
    {
      PlotItemColumn_Erase?.Invoke(parameter as PlotColumnTag);
    }

    #endregion ColumnErase command

    #endregion Column text boxes commands

    private ItemsControl _lastItemsControlActivated;

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
          object.ReferenceEquals(_guiAvailableTableColumnsCurrentlyActive, sender)

          )
          _lastItemsControlActivated = (ItemsControl)sender;
      }
    }
  }
}
