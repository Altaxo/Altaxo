#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2021 Dr. Dirk Lellinger
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
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Altaxo.Collections;
using GongSolutions.Wpf.DragDrop;
using Altaxo.Gui.Common;

namespace Altaxo.Gui.Analysis.NonLinearFitting
{
  /// <summary>
  /// Interaction logic for DependentVariableTransformationControl.xaml
  /// </summary>
  public partial class DependentVariableTransformationControl : UserControl, IDependentVariableTransformationView
  {
    public DependentVariableTransformationControl()
    {
      InitializeComponent();
    }

    public event CanStartDragDelegate? AvailableTransformations_CanStartDrag;
    public event StartDragDelegate? AvailableTransformations_StartDrag;
    public event DragEndedDelegate? AvailableTransformations_DragEnded;
    public event DragCancelledDelegate? AvailableTransformations_DragCancelled;
    public event DropCanAcceptDataDelegate? PlotItemColumn_DropCanAcceptData;
    public event DropDelegate? PlotItemColumn_Drop;


    private void EhPopup_Cancel(object sender, RoutedEventArgs e)
    {
      _guiPopup.IsOpen = false;
    }

    private void EhPopupFocusChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      if (false == (bool)e.NewValue)
        _guiPopup.IsOpen = false;
    }

    private void EhFocusWithinChanged(object sender, DependencyPropertyChangedEventArgs e)
    {

    }

    public void ShowTransformationSinglePrependAppendPopup(bool isOpen)
    {
      _guiPopup.IsOpen = isOpen;
    }

    #region AvailableTransformations_DragHander

    private IDragSource _availableTransformationsDragSource;

    public IDragSource AvailableTransformationsDragSource
    {
      get
      {
        return _availableTransformationsDragSource ??= new AvailableTransformations_DragSource(this); 
      }
    }

    public class AvailableTransformations_DragSource : IDragSource
    {
      private DependentVariableTransformationControl _parentControl;

      public AvailableTransformations_DragSource(DependentVariableTransformationControl ctrl)
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
        if (result is not null)
        {
          dragInfo.Effects = GuiHelper.ConvertCopyMoveToDragDropEffect(result.Value.CanCopy, result.Value.CanMove);
          dragInfo.Data = result.Value.Data;
        }
      }

      public void Dropped(IDropInfo dropInfo, DragDropEffects effects)
      {
        GuiHelper.ConvertDragDropEffectToCopyMove(effects, out var isCopy, out var isMove);
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
        if (_columTextBoxes_DropTarget is null)
          _columTextBoxes_DropTarget = new ColumTextBoxes_DropTarget(this);
        return _columTextBoxes_DropTarget;
      }
    }

    public class ColumTextBoxes_DropTarget : IDropTarget
    {
      private DependentVariableTransformationControl _parentControl;

      public ColumTextBoxes_DropTarget(DependentVariableTransformationControl ctrl)
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

        if (result is not null)
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

        if (result is not null)
        {
          dropInfo.Effects = GuiHelper.ConvertCopyMoveToDragDropEffect(result.Value.IsCopy, result.Value.IsMove); // it is important to get back the resulting effect to dropInfo, because dropInfo informs the drag handler about the resulting effect, which can e.g. delete the items after a move operation
        }
      }
    }

    #endregion Column text boxes drop handler


  }
}
