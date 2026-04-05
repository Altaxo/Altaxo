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

  /// <summary>
  /// View interface for a controller that combines an integer editor with a combo box.
  /// </summary>
  public interface IIntegerAndComboBoxView
  {
    /// <summary>
    /// Initializes the combo box with the available items.
    /// </summary>
    void ComboBox_Initialize(SelectableListNodeList items, SelectableListNode defaultItem);

    /// <summary>
    /// Initializes the label for the combo box.
    /// </summary>
    void ComboBoxLabel_Initialize(string text);

    /// <summary>
    /// Initializes the integer editor.
    /// </summary>
    void IntegerEdit_Initialize(int min, int max, int val);

    /// <summary>
    /// Initializes the label for the integer editor.
    /// </summary>
    void IntegerLabel_Initialize(string text);

    /// <summary>
    /// Occurs when the combo box selection changes.
    /// </summary>
    event Action<SelectableListNode> ComboBoxSelectionChanged;

    /// <summary>
    /// Occurs when the integer selection changes.
    /// </summary>
    event Action<int> IntegerSelectionChanged;
  }

  #endregion Interfaces

  /// <summary>
  /// Controller for a combined integer editor and combo box view.
  /// </summary>
  [ExpectedTypeOfView(typeof(IIntegerAndComboBoxView))]
  public class IntegerAndComboBoxController : IMVCAController
  {
    /// <summary>
    /// The attached view.
    /// </summary>
    protected IIntegerAndComboBoxView? _view;

    /// <summary>
    /// The label text for the integer input.
    /// </summary>
    protected string _integerLabelText;

    /// <summary>
    /// The label text for the combo box.
    /// </summary>
    protected string _comboBoxLabelText;

    /// <summary>
    /// The minimum integer value.
    /// </summary>
    protected int _integerMinimum;

    /// <summary>
    /// The maximum integer value.
    /// </summary>
    protected int _integerMaximum;

    /// <summary>
    /// The current integer value.
    /// </summary>
    protected int _integerValue;

    /// <summary>
    /// The combo-box items.
    /// </summary>
    protected SelectableListNodeList _comboBoxItems;

    /// <summary>
    /// The selected combo-box item.
    /// </summary>
    protected SelectableListNode _selectedItem;

    /// <summary>
    /// Initializes a new instance of the <see cref="IntegerAndComboBoxController"/> class.
    /// </summary>
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

    /// <summary>
    /// Initializes the controller and, if available, its view.
    /// </summary>
    /// <param name="initData">If set to <c>true</c>, model initialization is requested.</param>
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

    /// <inheritdoc/>
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

    /// <summary>
    /// Gets the selected combo box item.
    /// </summary>
    public SelectableListNode SelectedItem
    {
      get { return _selectedItem; }
    }

    /// <summary>
    /// Gets the selected integer value.
    /// </summary>
    public int IntegerValue
    {
      get { return _integerValue; }
    }

    #region IMVCController Members

    /// <inheritdoc/>
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

    /// <inheritdoc/>
    public object ModelObject
    {
      get { return _integerValue; }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
    }

    #endregion IMVCController Members

    #region IIntegerAndComboBoxController Members

    /// <summary>
    /// Handles changes of the integer value in the view.
    /// </summary>
    public void EhView_IntegerChanged(int val)
    {
      _integerValue = val;
    }

    /// <summary>
    /// Handles changes of the combo box selection in the view.
    /// </summary>
    public void EhView_ComboBoxSelectionChanged(SelectableListNode selectedItem)
    {
      _selectedItem = selectedItem;
    }

    #endregion IIntegerAndComboBoxController Members
  }
}
