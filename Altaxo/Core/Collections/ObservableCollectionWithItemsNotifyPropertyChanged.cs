#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2026 Dr. Dirk Lellinger
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

using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Altaxo.Collections
{
  /// <summary>
  /// An ObservableCollection that listens to the PropertyChanged event of its items and raises an ItemPropertyChanged event when an item's property changes.
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public class ObservableCollectionWithItemsNotifyPropertyChanged<T> : ObservableCollection<T> where T : INotifyPropertyChanged
  {
    /// <inheritdoc />
    protected override void InsertItem(int index, T item)
    {
      base.InsertItem(index, item);
      item.PropertyChanged += EhItem_PropertyChanged;
    }

    /// <inheritdoc />
    protected override void SetItem(int index, T item)
    {
      var oldItem = this[index];
      base.SetItem(index, item);
      oldItem.PropertyChanged -= EhItem_PropertyChanged;
      item.PropertyChanged += EhItem_PropertyChanged;
    }

    /// <inheritdoc />
    protected override void RemoveItem(int index)
    {
      var oldItem = this[index];
      base.RemoveItem(index);
      oldItem.PropertyChanged -= EhItem_PropertyChanged;
    }

    /// <inheritdoc />
    override protected void ClearItems()
    {
      foreach (var item in this)
      {
        item.PropertyChanged -= EhItem_PropertyChanged;
      }
      base.ClearItems();
    }

    /// <summary>
    /// Occurs when a property of an item in the collection changes.
    /// </summary>
    public event PropertyChangedEventHandler? ItemPropertyChanged;


    private void EhItem_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
      ItemPropertyChanged?.Invoke(sender, e);
    }
  }
}
