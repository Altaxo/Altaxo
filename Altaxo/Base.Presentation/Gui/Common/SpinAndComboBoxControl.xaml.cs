#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
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

namespace Altaxo.Gui.Common
{
  /// <summary>
  /// Interaction logic for SpinAndComboBoxControl.xaml
  /// </summary>
  public partial class SpinAndComboBoxControl : UserControl, IIntegerAndComboBoxView
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="SpinAndComboBoxControl"/> class.
    /// </summary>
    public SpinAndComboBoxControl()
    {
      InitializeComponent();
    }

    private void EhIntegerUpDown_ValueChanged(object sender, RoutedPropertyChangedEventArgs<int> e)
    {
      if (IntegerSelectionChanged is not null)
        IntegerSelectionChanged(_edIntegerUpDown.Value);
    }

    private void EhComboBox_SelectionChangeCommit(object sender, SelectionChangedEventArgs e)
    {
      if (ComboBoxSelectionChanged is not null)
        ComboBoxSelectionChanged((Collections.SelectableListNode)_cbComboBox.SelectedItem);
    }

    #region IIntegerAndComboBoxView

    /// <inheritdoc/>
    public event Action<Collections.SelectableListNode>? ComboBoxSelectionChanged;

    /// <inheritdoc/>
    public event Action<int>? IntegerSelectionChanged;

    /// <inheritdoc/>
    public void ComboBox_Initialize(Collections.SelectableListNodeList items, Collections.SelectableListNode defaultItem)
    {
      _cbComboBox.ItemsSource = null;
      _cbComboBox.ItemsSource = items;
      _cbComboBox.SelectedItem = defaultItem;
    }

    /// <inheritdoc/>
    public void ComboBoxLabel_Initialize(string text)
    {
      _lblComboBoxLabel.Content = text;
    }

    /// <inheritdoc/>
    public void IntegerEdit_Initialize(int min, int max, int val)
    {
      _edIntegerUpDown.Minimum = min;
      _edIntegerUpDown.Maximum = max;
      _edIntegerUpDown.Value = val;
    }

    /// <inheritdoc/>
    public void IntegerLabel_Initialize(string text)
    {
      _lblIntegerLabel.Content = text;
    }

    #endregion IIntegerAndComboBoxView
  }
}
