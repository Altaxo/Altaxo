#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2022 Dr. Dirk Lellinger
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
using System.ComponentModel;
using System.Linq;
using Altaxo.Collections;

namespace Altaxo.Gui.Common
{
  /// <summary>
  /// Helper class for list boxed and other item controls with single selection mode.
  /// </summary>
  /// <typeparam name="TItem">The type of the item.</typeparam>
  public class ItemsController<TItem> : INotifyPropertyChanged, IDisposable
  {
    /// <summary>
    /// Occurs when a property value changes.
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;
    protected virtual void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    Action<TItem>? _onSelectedValueChanged;
    Action<(TItem OldSelection, int OldIndex, TItem NewSelection, int NewIndex)>? _onSelectedValueChangedEx;

    public ItemsController()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ItemsController{TItem}"/> class.
    /// </summary>
    /// <param name="list">The list of choices. The <see cref="ListNode.Tag"/> property must contain values of type TItem.</param>
    /// <param name="selectedValue">The selected value.</param>
    /// <param name="OnSelectedValueChanged">An optional action, that is executed if the selected value changed</param>
    public ItemsController(SelectableListNodeList list, TItem selectedValue, Action<TItem>? OnSelectedValueChanged = null)
    {
      Initialize(list, selectedValue, OnSelectedValueChanged);
    }


    /// <summary>
    /// Initializes a new instance of the <see cref="ItemsController{TItem}"/> class.
    /// </summary>
    /// <param name="list">The list. Must contain elements with tags of type TItem. On of the list items should have the IsSelected
    /// property set to true, in order to initialize the SelectedItem and SelectedValue property.</param>
    /// <param name="OnSelectedItemChanged">An optional action, that is executed if the selected value changed.</param>
    public ItemsController(SelectableListNodeList list, Action<TItem>? OnSelectedItemChanged = null)
    {
      Initialize(list, OnSelectedItemChanged);
    }

    public ItemsController(SelectableListNodeList list, Action<(TItem OldSelection, int OldIndex, TItem NewSelection, int NewIndex)> OnSelectedItemChanged)
    {
      _onSelectedValueChangedEx = OnSelectedItemChanged;
      Initialize(list, null);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ItemsController{TItem}"/> class.
    /// </summary>
    /// <param name="list">The list of choices. The <see cref="ListNode.Tag"/> property must contain values of type TItem.</param>
    /// <param name="selectedValue">The selected value.</param>
    /// <param name="OnSelectedValueChanged">An optional action, that is executed if the selected value changed</param>
    public void Initialize(SelectableListNodeList list, TItem selectedValue, Action<TItem>? OnSelectedValueChanged = null)
    {
      _items = list;
      _items.SetSelection(n => object.ReferenceEquals(n, selectedValue));
      _selectedItem = _items.FirstOrDefault(element => object.Equals(element.Tag, selectedValue));
      _onSelectedValueChanged = OnSelectedValueChanged;

      OnPropertyChanged(nameof(Items));
      OnPropertyChanged(nameof(SelectedItem));
      OnPropertyChanged(nameof(SelectedValue));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ItemsController{TItem}"/> class.
    /// </summary>
    /// <param name="list">The list. Must contain elements with tags of type TItem. On of the list items should have the IsSelected
    /// property set to true, in order to initialize the SelectedItem and SelectedValue property.</param>
    /// <param name="OnSelectedItemChanged">An optional action, that is executed if the selected value changed.</param>
    public void Initialize(SelectableListNodeList list, Action<TItem>? OnSelectedItemChanged = null)
    {
      _items = list;
      _selectedItem = _items.FirstSelectedNode;
      _onSelectedValueChanged = OnSelectedItemChanged;

      OnPropertyChanged(nameof(Items));
      OnPropertyChanged(nameof(SelectedItem));
      OnPropertyChanged(nameof(SelectedValue));
    }

    public void Dispose()
    {
      // The order of the calls is important here
      _onSelectedValueChanged = null;
      _items = null;
      _selectedItem = null;
      OnPropertyChanged(nameof(Items));
      OnPropertyChanged(nameof(SelectedItem));
      PropertyChanged = null;
    }

    #region Bindings

    private SelectableListNodeList _items;

    /// <summary>
    /// Gets or sets the items. Used to bind the items to the Gui element.
    /// </summary>
    public SelectableListNodeList Items
    {
      get => _items;
      set
      {
        if (!(_items == value))
        {
          _items = value;
          OnPropertyChanged(nameof(Items));
        }
      }
    }

    private SelectableListNode _selectedItem;

    /// <summary>
    /// Gets or sets the selected item. Used to bind the Selecteditem property of the Gui element.
    /// </summary>
    public SelectableListNode SelectedItem
    {
      get => _selectedItem;
      set
      {
        if (!(object.Equals(_selectedItem, value)))
        {
          var oldSelectedItem = _selectedItem;
          _selectedItem = value;
          _items.SetSelection(n => object.ReferenceEquals(n, value));
          OnPropertyChanged(nameof(SelectedItem));
          OnPropertyChanged(nameof(SelectedValue));
          OnPropertyChanged(nameof(SelectedIndex));
          _onSelectedValueChanged?.Invoke((TItem)(value?.Tag ?? default(TItem)));
          if(_onSelectedValueChangedEx is not null)
          {
            var oldIndex = GetIndexOf(oldSelectedItem); // can be -1 if oldSelectedItem is null or is removed
            var newIndex = GetIndexOf(_selectedItem);
            var oldValue = oldIndex < 0 ? default(TItem) : (TItem)(oldSelectedItem?.Tag ?? default(TItem));
            var newValue = newIndex < 0 ? default(TItem) : (TItem)(_selectedItem?.Tag ?? default(TItem));
            _onSelectedValueChangedEx?.Invoke((oldValue, oldIndex, newValue, newIndex));
          }
        }
      }
    }

    /// <summary>
    /// Gets or sets the selected value. It is not recommended to bind the SelectedValue property of the Gui element to this;
    /// better bind the SelectedItem property of the Gui element to <see cref="SelectedItem"/>.
    /// </summary>
    public TItem SelectedValue
    {
      get
      {
        return (TItem)(_selectedItem?.Tag ?? default(TItem));
      }
      set
      {
        SelectedItem = _items.FirstOrDefault(element => object.Equals(element.Tag, value));
      }
    }

    /// <summary>
    /// Gets the index of the selected item, or -1 if nothing is selected.
    /// </summary>
    public int SelectedIndex
    {
      get
      {
        return GetIndexOf(_selectedItem);
      }
    }

    protected int GetIndexOf(SelectableListNode item)
    {
      int result = -1;
      for (int i = 0; i < _items.Count; i++)
      {
        if (object.ReferenceEquals(_items[i], item))
        {
          result = i;
          break;
        }
      }
      return result;
    }

    #endregion

  }
}
