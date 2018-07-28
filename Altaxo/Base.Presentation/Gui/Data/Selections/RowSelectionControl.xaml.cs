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
using Altaxo.Data.Selections;
using Altaxo.Gui.Common;
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

namespace Altaxo.Gui.Data.Selections
{
  /// <summary>
  /// Interaction logic for RowSelectionControl.xaml
  /// </summary>
  public partial class RowSelectionControl : UserControl, IRowSelectionView
  {
    public event Action<int, Type> SelectionTypeChanged;

    public event Action<int> CmdAddNewSelection;

    public event Action<int> CmdRemoveSelection;

    public event Action<int> CmdIndentSelection;

    public event Action<int> CmdUnindentSelection;

    private List<RSEntry> _rowSelections;

    public RowSelectionControl()
    {
      InitializeComponent();
    }

    public void InitRowSelections(List<RSEntry> rowSelections, SelectableListNodeList rowSelectionSimpleTypes, SelectableListNodeList rowSelectionCollectionTypes)
    {
      _rowSelections = rowSelections;

      _guiStackPanel.Children.Clear();

      for (int i = 0; i < _rowSelections.Count; ++i)
      {
        var rsItem = _rowSelections[i];

        var rsGuiItem = rsItem.GuiItem as RowSelectionItemControl;

        if (null == rsGuiItem)
        {
          var selTypes = new SelectableListNodeList();

          if (rsItem.RowSelection is IRowSelectionCollection)
          {
            foreach (var item in rowSelectionCollectionTypes)
            {
              selTypes.Add(new SelectableListNode(item.Text, item.Tag, rsItem.RowSelection.GetType() == (Type)item.Tag));
            }
          }
          else // simple type
          {
            foreach (var item in rowSelectionSimpleTypes)
            {
              selTypes.Add(new SelectableListNode(item.Text, item.Tag, rsItem.RowSelection.GetType() == (Type)item.Tag));
            }
          }

          rsGuiItem = new RowSelectionItemControl(selTypes, rsItem.DetailsController?.ViewObject);
        }

        rsGuiItem.Tag = i;
        rsGuiItem.IndentationLevel = rsItem.IndentationLevel;
        rsGuiItem.RowSelectionDetailControl = rsItem.DetailsController?.ViewObject;
        rsItem.GuiItem = rsGuiItem;
        _guiStackPanel.Children.Add(rsGuiItem);
      }
    }

    public void ChangeRowSelection(int idx, SelectableListNodeList rowSelectionTypes)
    {
      var rsItem = _rowSelections[idx];
      var rsGuiItem = rsItem.GuiItem as RowSelectionItemControl;

      if (null == rsGuiItem)
      {
        var selTypes = new SelectableListNodeList();
        foreach (var item in rowSelectionTypes)
        {
          selTypes.Add(new SelectableListNode(item.Text, item.Tag, rsItem.RowSelection.GetType() == (Type)item.Tag));
        }

        rsGuiItem = new RowSelectionItemControl(selTypes, rsItem.DetailsController?.ViewObject);
      }

      rsGuiItem.Tag = idx;
      rsGuiItem.IndentationLevel = rsItem.IndentationLevel;
      rsGuiItem.RowSelectionDetailControl = rsItem.DetailsController?.ViewObject;
      rsItem.GuiItem = rsGuiItem;

      _guiStackPanel.Children.RemoveAt(idx);
      _guiStackPanel.Children.Insert(idx, rsGuiItem);
    }

    #region AddNewSelection command

    private ICommand _addNewSelectionCommand;

    public ICommand AddNewSelectionCommand
    {
      get
      {
        if (this._addNewSelectionCommand == null)
          this._addNewSelectionCommand = new RelayCommand<int>(EhAddNewSelectionCommand);
        return this._addNewSelectionCommand;
      }
    }

    private void EhAddNewSelectionCommand(int parameter)
    {
      CmdAddNewSelection?.Invoke(parameter);
    }

    #endregion AddNewSelection command

    #region RemoveSelection command

    private ICommand _removeSelectionCommand;

    public ICommand RemoveSelectionCommand
    {
      get
      {
        if (this._removeSelectionCommand == null)
          this._removeSelectionCommand = new RelayCommand<int>(EhRemoveSelectionCommand);
        return this._removeSelectionCommand;
      }
    }

    private void EhRemoveSelectionCommand(int parameter)
    {
      CmdRemoveSelection?.Invoke(parameter);
    }

    #endregion RemoveSelection command

    #region IndentSelection command

    private ICommand _indentSelectionCommand;

    public ICommand IndentSelectionCommand
    {
      get
      {
        if (this._indentSelectionCommand == null)
          this._indentSelectionCommand = new RelayCommand<int>(EhIndentSelectionCommand);
        return this._indentSelectionCommand;
      }
    }

    private void EhIndentSelectionCommand(int parameter)
    {
      CmdIndentSelection?.Invoke(parameter);
    }

    #endregion IndentSelection command

    #region UnindentSelection command

    private ICommand _unindentSelectionCommand;

    public ICommand UnindentSelectionCommand
    {
      get
      {
        if (this._unindentSelectionCommand == null)
          this._unindentSelectionCommand = new RelayCommand<int>(EhUnindentSelectionCommand);
        return this._unindentSelectionCommand;
      }
    }

    private void EhUnindentSelectionCommand(int parameter)
    {
      CmdUnindentSelection?.Invoke(parameter);
    }

    #endregion UnindentSelection command

    #region SelectionChanged command

    private ICommand _selectionChangedCommand;

    public ICommand SelectionChangedCommand
    {
      get
      {
        if (this._selectionChangedCommand == null)
          this._selectionChangedCommand = new RelayCommand<int>(EhSelectionChanged);
        return this._selectionChangedCommand;
      }
    }

    private void EhSelectionChanged(int parameter)
    {
      int idx = parameter;

      var control = (RowSelectionItemControl)_rowSelections[idx].GuiItem;

      var node = (SelectableListNode)control.SelectedSelection;

      if (node != null)
      {
        SelectionTypeChanged?.Invoke(idx, (Type)node.Tag);
      }
    }

    #endregion SelectionChanged command
  }
}
