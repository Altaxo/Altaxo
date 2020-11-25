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

#nullable enable
using System;
using Altaxo.Collections;

namespace Altaxo.Gui.Common
{
  #region Interfaces

  public interface IIntegerAndComboBoxView
  {
    void ComboBox_Initialize(SelectableListNodeList items, SelectableListNode defaultItem);

    void ComboBoxLabel_Initialize(string text);

    void IntegerEdit_Initialize(int min, int max, int val);

    void IntegerLabel_Initialize(string text);

    event Action<SelectableListNode> ComboBoxSelectionChanged;

    event Action<int> IntegerSelectionChanged;
  }

  #endregion Interfaces

  /// <summary>
  /// Summary description for IntegerAndComboBoxController.
  /// </summary>
  [ExpectedTypeOfView(typeof(IIntegerAndComboBoxView))]
  public class IntegerAndComboBoxController : IMVCAController
  {
    protected IIntegerAndComboBoxView? _view;
    protected string _integerLabelText;
    protected string _comboBoxLabelText;
    protected int _integerMinimum;
    protected int _integerMaximum;
    protected int _integerValue;
    protected SelectableListNodeList _comboBoxItems;
    protected SelectableListNode _selectedItem;

    public IntegerAndComboBoxController(string integerLabel,
      int intMin, int intMax,
      int intVal,
      string comboBoxLabel,
      SelectableListNodeList items,
      int defaultItem)
    {
      _integerLabelText = integerLabel;
      _integerMinimum = intMin;
      _integerMaximum = intMax;
      _integerValue = intVal;
      _comboBoxLabelText = comboBoxLabel;
      _comboBoxItems = items;
      _selectedItem = items[defaultItem];

      Initialize(true);
    }

    public void Initialize(bool initData)
    {
      if (initData)
      {
      }

      if (_view is not null)
      {
        _view.ComboBoxLabel_Initialize(_comboBoxLabelText);
        _view.ComboBox_Initialize(_comboBoxItems, _selectedItem);
        _view.IntegerLabel_Initialize(_integerLabelText);
        _view.IntegerEdit_Initialize(_integerMinimum, _integerMaximum, _integerValue);
      }
    }

    public bool Apply(bool disposeController)
    {
      return true; // all is done on the fly, we don't need actions here
    }

    /// <summary>
    /// Try to revert changes to the model, i.e. restores the original state of the model.
    /// </summary>
    /// <param name="disposeController">If set to <c>true</c>, the controller should release all temporary resources, since the controller is not needed anymore.</param>
    /// <returns>
    ///   <c>True</c> if the revert operation was successfull; <c>false</c> if the revert operation was not possible (i.e. because the controller has not stored the original state of the model).
    /// </returns>
    public bool Revert(bool disposeController)
    {
      return false;
    }

    public SelectableListNode SelectedItem
    {
      get { return _selectedItem; }
    }

    public int IntegerValue
    {
      get { return _integerValue; }
    }

    #region IMVCController Members

    public object? ViewObject
    {
      get { return _view; }
      set
      {
        if (_view is not null)
        {
          _view.IntegerSelectionChanged -= EhView_IntegerChanged;
          _view.ComboBoxSelectionChanged -= EhView_ComboBoxSelectionChanged;
        }

        _view = value as IIntegerAndComboBoxView;

        if (_view is not null)
        {
          Initialize(false);
          _view.IntegerSelectionChanged += EhView_IntegerChanged;
          _view.ComboBoxSelectionChanged += EhView_ComboBoxSelectionChanged;
        }
      }
    }

    public object ModelObject
    {
      get { return _integerValue; }
    }

    public void Dispose()
    {
    }

    #endregion IMVCController Members

    #region IIntegerAndComboBoxController Members

    public void EhView_IntegerChanged(int val)
    {
      _integerValue = val;
    }

    public void EhView_ComboBoxSelectionChanged(SelectableListNode selectedItem)
    {
      _selectedItem = selectedItem;
    }

    #endregion IIntegerAndComboBoxController Members
  }
}
