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

#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Altaxo.Collections;
using Altaxo.Main.Services;

namespace Altaxo.Gui.Units
{
  using System.Collections;
  using System.ComponentModel;
  using System.Windows.Input;
  using Altaxo.Units;

  public interface IUserDefinedUnitEnvironmentView : IDataContextAwareView
  {
  }

  /// <summary>
  /// Creates a custom unit environment
  /// </summary>
  [ExpectedTypeOfView(typeof(IUserDefinedUnitEnvironmentView))]
  public class UserDefinedUnitEnvironmentController : MVCANControllerEditCopyOfDocBase<UserDefinedUnitEnvironment, IUserDefinedUnitEnvironmentView>, System.ComponentModel.INotifyPropertyChanged
  {
    private UnitEnvironmentController? _unitController;

    public event PropertyChangedEventHandler? PropertyChanged;

    private SelectableListNodeList _quantities = new SelectableListNodeList();
    private SelectableListNode? _selectedQuantity;
    private string? _environmentName;

    private UserDefinedUnitEnvironments? _availableEnvironments;

    public override bool InitializeDocument(params object[] args)
    {
      if (args.Length > 1 && args[1] is UserDefinedUnitEnvironments availableEnvironments)
        _availableEnvironments = availableEnvironments;

      return base.InitializeDocument(args);
    }

    protected override void Initialize(bool initData)
    {
      CheckDocumentInitialized(ref _doc);

      base.Initialize(initData);

      if (initData)
      {
        // ------- Set the environment name --------------
        EnvironmentName = _doc.Name;

        // ------- Set available quantities --------------
        _quantities.Clear();
        if (string.IsNullOrEmpty(_doc.Quantity)) // if doc has not quantitie still, then all quantities will be shown
        {
          foreach (var quantity in UnitsExtensions.GetAllDefinedQuantities())
          {
            var node = new SelectableListNode(quantity, quantity, quantity == _doc.Quantity);
            _quantities.Add(node);
            if (node.IsSelected)
              _selectedQuantity = node;
          }
        }
        else // else if doc already has a quantity, then we do not allow to change this quantity
        {
          var node = new SelectableListNode(_doc.Quantity, _doc.Quantity, true);
          _quantities.Add(node);
          _selectedQuantity = node;
        }

        // -------- Set up the unit environment controller -------
        _unitController = new UnitEnvironmentController(quantity: _doc.Quantity);
        _unitController.InitializeDocument(_doc.Environment ?? new QuantityWithUnitGuiEnvironment());
        Current.Gui.FindAndAttachControlTo(_unitController);
      }
    }

    protected override void AttachView()
    {
      base.AttachView();
      _view!.DataContext = this;
    }

    protected override void DetachView()
    {
      _view!.DataContext = null;
      base.DetachView();
    }

    public override bool Apply(bool disposeController)
    {
      if (_unitController is null)
        throw new InvalidOperationException();

      if (!_unitController.Apply(disposeController))
        return ApplyEnd(false, disposeController);

      var quantity = (string?)_selectedQuantity?.Tag;

      if (string.IsNullOrEmpty(quantity))
        return ApplyEnd(false, disposeController);

      if (string.IsNullOrEmpty(_environmentName))
      {
        Current.Gui.ErrorMessageBox("Please enter a name for the unit environment");
        return ApplyEnd(false, disposeController);
      }
      else if (_availableEnvironments is not null &&
                  _availableEnvironments.TryGetValue(_environmentName, out var alreadyPresentEnvironment) &&
                  alreadyPresentEnvironment.Quantity != quantity)
      {
        Current.Gui.ErrorMessageBox("There is already an environment present, but it uses another quantity. Thus overriding this environment is not allowed." +
            " Please enter a new environment name.");
        return ApplyEnd(false, disposeController);
      }

      _doc = new UserDefinedUnitEnvironment(_environmentName, quantity, (QuantityWithUnitGuiEnvironment)_unitController.ModelObject);
      _originalDoc = _doc;

      return ApplyEnd(true, disposeController);
    }

    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      if (_unitController is not null)
        yield return new ControllerAndSetNullMethod(_unitController, () => _unitController = null);
    }

    #region Binding properties

    public object? UnitControllerViewObject
    {
      get
      {
        return _unitController?.ViewObject;
      }
    }

    public SelectableListNodeList Quantities
    {
      get
      {
        return _quantities;
      }
    }

    public SelectableListNode? SelectedQuantity
    {
      get
      {
        return _selectedQuantity;
      }
      set
      {
        if (!object.ReferenceEquals(_selectedQuantity, value))
        {
          _selectedQuantity = value;
          if (_unitController is { } _ && _selectedQuantity is not null && !string.IsNullOrEmpty(_selectedQuantity.Tag as string))
            _unitController.SetQuantity((string)_selectedQuantity.Tag);

          PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedQuantity)));
        }
      }
    }

    public string? EnvironmentName
    {
      get
      {
        return _environmentName;
      }
      set
      {
        if (!(_environmentName == value))
        {
          _environmentName = value;
          PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(EnvironmentName)));
        }
      }
    }

    #endregion Binding properties
  }
}
