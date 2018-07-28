using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Altaxo.Collections;
using Altaxo.Main.Services;

namespace Altaxo.Gui.Units
{
  using System.ComponentModel;
  using Altaxo.Units;
  using System.Windows.Input;
  using System.Collections;

  public interface IUserDefinedUnitEnvironmentView : IDataContextAwareView
  {
  }

  /// <summary>
  /// Creates a custom unit environment
  /// </summary>
  [ExpectedTypeOfView(typeof(IUserDefinedUnitEnvironmentView))]
  public class UserDefinedUnitEnvironmentController : MVCANControllerEditCopyOfDocBase<UserDefinedUnitEnvironment, IUserDefinedUnitEnvironmentView>, System.ComponentModel.INotifyPropertyChanged
  {
    private UnitEnvironmentController _unitController;

    public event PropertyChangedEventHandler PropertyChanged;

    private SelectableListNodeList _quantities = new SelectableListNodeList();
    private SelectableListNode _selectedQuantity;
    private string _environmentName;

    private UserDefinedUnitEnvironments _availableEnvironments;

    public override bool InitializeDocument(params object[] args)
    {
      if (args.Length > 1 && args[1] is UserDefinedUnitEnvironments availableEnvironments)
        _availableEnvironments = availableEnvironments;

      return base.InitializeDocument(args);
    }

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        _unitController = new UnitEnvironmentController();
        _unitController.InitializeDocument(_doc.Environment ?? new QuantityWithUnitGuiEnvironment());
        Current.Gui.FindAndAttachControlTo(_unitController);

        _unitController.SetQuantity(_doc.Quantity);

        _quantities.Clear();
        if (string.IsNullOrEmpty(_doc.Quantity))
        {
          foreach (var quantity in UnitsExtensions.GetAllDefinedQuantities())
          {
            var node = new SelectableListNode(quantity, quantity, quantity == _doc.Quantity);
            _quantities.Add(node);
            if (node.IsSelected)
              _selectedQuantity = node;
          }
        }
        else
        {
          var node = new SelectableListNode(_doc.Quantity, _doc.Quantity, true);
          _quantities.Add(node);
          _selectedQuantity = node;
        }
      }
    }

    protected override void AttachView()
    {
      base.AttachView();
      _view.DataContext = this;
    }

    protected override void DetachView()
    {
      _view.DataContext = null;
      base.DetachView();
    }

    public override bool Apply(bool disposeController)
    {
      if (!_unitController.Apply(disposeController))
        return ApplyEnd(false, disposeController);

      string quantity = (string)_selectedQuantity.Tag;

      if (string.IsNullOrEmpty(_environmentName))
      {
        Current.Gui.ErrorMessageBox("Please enter a name for the unit environment");
        return ApplyEnd(false, disposeController);
      }
      else if (null != _availableEnvironments &&
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
      yield return new ControllerAndSetNullMethod(_unitController, () => _unitController = null);
    }

    #region Binding properties

    public object UnitControllerViewObject
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

    public SelectableListNode SelectedQuantity
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
          _unitController.SetQuantity((string)_selectedQuantity.Tag);
          PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedQuantity)));
        }
      }
    }

    public string EnvironmentName
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
