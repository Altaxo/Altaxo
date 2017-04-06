#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2017 Dr. Dirk Lellinger
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
using Altaxo.Drawing;
using Altaxo.Graph.Graph2D.Plot.Styles.ScatterSymbols;
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

namespace Altaxo.Gui.Drawing.ColorManagement
{
  /// <summary>
  /// Interaction logic for ScatterSymbolListControl.xaml
  /// </summary>
  public partial class ColorListControl : UserControl, IColorListView
  {
    public ColorListControl()
    {
      InitializeComponent();

      // set the item template of current items to a more appropriate template
      _guiSL.CurrentItemsTemplate = this.FindResource("CurrentItemsTemplateResource") as DataTemplate;
      _guiSL.AvailableItemsTemplate = this.FindResource("AvailableItemsTemplateResource") as HierarchicalDataTemplate;
    }

    #region Interface forwarding

    public string CurrentItemListName
    {
      get
      {
        return _guiSL.CurrentItemListName;
      }
    }

    public bool StoreInUserSettings
    {
      get
      {
        return _guiSL.StoreInUserSettings;
      }

      set
      {
        _guiSL.StoreInUserSettings = value;
      }
    }

    public event CanStartDragDelegate AvailableItems_CanStartDrag
    {
      add
      {
        _guiSL.AvailableItems_CanStartDrag += value;
      }
      remove
      {
        _guiSL.AvailableItems_CanStartDrag -= value;
      }
    }

    public event DragCancelledDelegate AvailableItems_DragCancelled
    {
      add
      {
        _guiSL.AvailableItems_DragCancelled += value;
      }
      remove
      {
        _guiSL.AvailableItems_DragCancelled -= value;
      }
    }

    public event DragEndedDelegate AvailableItems_DragEnded
    {
      add
      {
        _guiSL.AvailableItems_DragEnded += value;
      }
      remove
      {
        _guiSL.AvailableItems_DragEnded -= value;
      }
    }

    public event DropDelegate AvailableItems_Drop
    {
      add
      {
        _guiSL.AvailableItems_Drop += value;
      }
      remove
      {
        _guiSL.AvailableItems_Drop -= value;
      }
    }

    public event DropCanAcceptDataDelegate AvailableItems_DropCanAcceptData
    {
      add
      {
        _guiSL.AvailableItems_DropCanAcceptData += value;
      }
      remove
      {
        _guiSL.AvailableItems_DropCanAcceptData -= value;
      }
    }

    public event StartDragDelegate AvailableItems_StartDrag
    {
      add
      {
        _guiSL.AvailableItems_StartDrag += value;
      }
      remove
      {
        _guiSL.AvailableItems_StartDrag -= value;
      }
    }

    public event Action AvailableItem_AddToCurrent
    {
      add
      {
        _guiSL.AvailableItem_AddToCurrent += value;
      }
      remove
      {
        _guiSL.AvailableItem_AddToCurrent -= value;
      }
    }

    public event Action<NGTreeNode> AvailableLists_SelectionChanged
    {
      add
      {
        _guiSL.AvailableLists_SelectionChanged += value;
      }
      remove
      {
        _guiSL.AvailableLists_SelectionChanged -= value;
      }
    }

    public event Action CurrentItemListName_Changed
    {
      add
      {
        _guiSL.CurrentItemListName_Changed += value;
      }
      remove
      {
        _guiSL.CurrentItemListName_Changed -= value;
      }
    }

    public event CanStartDragDelegate CurrentItems_CanStartDrag
    {
      add
      {
        _guiSL.CurrentItems_CanStartDrag += value;
      }
      remove
      {
        _guiSL.CurrentItems_CanStartDrag -= value;
      }
    }

    public event DragCancelledDelegate CurrentItems_DragCancelled
    {
      add
      {
        _guiSL.CurrentItems_DragCancelled += value;
      }
      remove
      {
        _guiSL.CurrentItems_DragCancelled -= value;
      }
    }

    public event DragEndedDelegate CurrentItems_DragEnded
    {
      add
      {
        _guiSL.CurrentItems_DragEnded += value;
      }
      remove
      {
        _guiSL.CurrentItems_DragEnded -= value;
      }
    }

    public event DropDelegate CurrentItems_Drop
    {
      add
      {
        _guiSL.CurrentItems_Drop += value;
      }
      remove
      {
        _guiSL.CurrentItems_Drop -= value;
      }
    }

    public event DropCanAcceptDataDelegate CurrentItems_DropCanAcceptData
    {
      add
      {
        _guiSL.CurrentItems_DropCanAcceptData += value;
      }
      remove
      {
        _guiSL.CurrentItems_DropCanAcceptData -= value;
      }
    }

    public event StartDragDelegate CurrentItems_StartDrag
    {
      add
      {
        _guiSL.CurrentItems_StartDrag += value;
      }
      remove
      {
        _guiSL.CurrentItems_StartDrag -= value;
      }
    }

    public event Action CurrentItem_Edit
    {
      add
      {
        _guiSL.CurrentItem_Edit += value;
      }
      remove
      {
        _guiSL.CurrentItem_Edit -= value;
      }
    }

    public event Action CurrentItem_MoveDown
    {
      add
      {
        _guiSL.CurrentItem_MoveDown += value;
      }
      remove
      {
        _guiSL.CurrentItem_MoveDown -= value;
      }
    }

    public event Action CurrentItem_MoveUp
    {
      add
      {
        _guiSL.CurrentItem_MoveUp += value;
      }
      remove
      {
        _guiSL.CurrentItem_MoveUp -= value;
      }
    }

    public event Action CurrentItem_Remove
    {
      add
      {
        _guiSL.CurrentItem_Remove += value;
      }
      remove
      {
        _guiSL.CurrentItem_Remove -= value;
      }
    }

    public event Action CurrentList_Store
    {
      add
      {
        _guiSL.CurrentList_Store += value;
      }
      remove
      {
        _guiSL.CurrentList_Store -= value;
      }
    }

    public void AvailableItems_Initialize(NGTreeNodeCollection items)
    {
      _guiSL.AvailableItems_Initialize(items);
    }

    public void AvailableLists_Initialize(NGTreeNodeCollection nodes)
    {
      _guiSL.AvailableLists_Initialize(nodes);
    }

    public void CurrentItemListName_Initialize(string name, bool isEnabled, bool isMarked, string toolTipText)
    {
      _guiSL.CurrentItemListName_Initialize(name, isEnabled, isMarked, toolTipText);
    }

    public void CurrentItemList_Initialize(SelectableListNodeList items)
    {
      _guiSL.CurrentItemList_Initialize(items);
    }

    public void SetCustomColorView(object guiCustomColorViewObject)
    {
      _guiCustomColorControlHost.Child = (UIElement)guiCustomColorViewObject;
    }

    #endregion Interface forwarding

    #region New Interface

    public event Action UserRequest_AddCustomColorToList;

    public event Action<double> UserRequest_ForAllSelectedItemsSetOpacity;

    public event Action<double> UserRequest_ForAllSelectedItemsShiftHue;

    public event Action<double> UserRequest_ForAllSelectedItemsSetSaturation;

    public event Action<double> UserRequest_ForAllSelectedItemsSetBrightness;

    public event Action<string> UserRequest_ForAllSelectedItemsSetColorName;

    #endregion New Interface

    public virtual DataTemplate CurrentItemsTemplate
    {
      get
      {
        return this.FindResource("CurrentItemsTemplateResource") as DataTemplate;
      }
    }

    private void EhAddCustomColorToList(object sender, RoutedEventArgs e)
    {
      UserRequest_AddCustomColorToList?.Invoke();
    }

    private void EhSetAlphaValueForAllSelectedItems(object sender, RoutedEventArgs e)
    {
      UserRequest_ForAllSelectedItemsSetOpacity?.Invoke(_guiAlphaValueForAllSelectedItems.Value / 100);
    }

    private void EhForAllSelectedItemsShiftHue(object sender, RoutedEventArgs e)
    {
      UserRequest_ForAllSelectedItemsShiftHue?.Invoke(_guiForAllSelectedItemsHueShift.Value / 100);
    }

    private void EhForAllSelectedItemsSetSaturation(object sender, RoutedEventArgs e)
    {
      UserRequest_ForAllSelectedItemsSetSaturation?.Invoke(_guiForAllSelectedItemsSaturationValue.Value / 100);
    }

    private void EhForAllSelectedItemsSetBrightness(object sender, RoutedEventArgs e)
    {
      UserRequest_ForAllSelectedItemsSetBrightness?.Invoke(_guiForAllSelectedItemsBrightnessValue.Value / 100);
    }

    private void EhForAllSelectedItemsSetColorName(object sender, RoutedEventArgs e)
    {
      UserRequest_ForAllSelectedItemsSetColorName?.Invoke(_guiForAllSelectedItemsColorName.Text);
    }
  }
}