using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Altaxo.Calc.Interpolation;
using Altaxo.Collections;
using Altaxo.Gui.Common;

namespace Altaxo.Gui.Calc.Interpolation
{
  /// <summary>
  /// Defines the view contract for editing interpolation function options.
  /// </summary>
  public interface IInterpolationFunctionOptionsView : IDataContextAwareView { }

  /// <summary>
  /// Controller for <see cref="IInterpolationFunctionOptions"/>.
  /// </summary>
  [ExpectedTypeOfView(typeof(IInterpolationFunctionOptionsView))]
  public class InterpolationFunctionOptionsController : MVCANControllerEditImmutableDocBase<IInterpolationFunctionOptions, IInterpolationFunctionOptionsView>
  {
    /// <inheritdoc/>
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield return new ControllerAndSetNullMethod(_interpolationDetailsController, () => InterpolationDetailsController = null);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InterpolationFunctionOptionsController"/> class.
    /// </summary>
    public InterpolationFunctionOptionsController() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="InterpolationFunctionOptionsController"/> class.
    /// </summary>
    /// <param name="doc">The interpolation options.</param>
    public InterpolationFunctionOptionsController(IInterpolationFunctionOptions doc)
    {
      _doc = _originalDoc = doc;
      Initialize(true);
    }

    #region Bindings

    private ItemsController<Type> _interpolationMethod;

    /// <summary>
    /// Gets or sets the available interpolation methods.
    /// </summary>
    public ItemsController<Type> InterpolationMethod
    {
      get => _interpolationMethod;
      set
      {
        if (!(_interpolationMethod == value))
        {
          _interpolationMethod = value;
          OnPropertyChanged(nameof(InterpolationMethod));
        }
      }
    }

    private IMVCANController _interpolationDetailsController;

    /// <summary>
    /// Gets or sets the controller for interpolation-method details.
    /// </summary>
    public IMVCANController InterpolationDetailsController
    {
      get => _interpolationDetailsController;
      set
      {
        if (!(_interpolationDetailsController == value))
        {
          _interpolationDetailsController?.Dispose();
          _interpolationDetailsController = value;
          OnPropertyChanged(nameof(InterpolationDetailsController));
        }
      }
    }

    #endregion

    /// <inheritdoc/>
    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if(initData)
      {

        InterpolationMethod = new ItemsController<Type>(
          new SelectableListNodeList(
            Altaxo.Main.Services.ReflectionService.GetNonAbstractSubclassesOf(typeof(IInterpolationFunctionOptions))
              .Select(t => new SelectableListNode(TrimOptions(t.Name), t, false))
              ),
          EhInterpolationMethodChanged
            );

        InterpolationMethod.SelectedValue = _doc.GetType();

      }
    }

    private void EhInterpolationMethodChanged(Type newType)
    {
      if(newType is not null)
      {
        if (_doc.GetType() != newType)
        {
          _doc = (IInterpolationFunctionOptions)Activator.CreateInstance(newType);
        }

        InterpolationDetailsController = (IMVCANController)Current.Gui.GetControllerAndControl(new object[] { _doc }, typeof(IMVCANController));
      }
    }

    static string TrimOptions(string name)
    {
      const string o1 = "Options";
      const string o2 = "Option";

      if(name.EndsWith(o1))
        name = name.Substring(0, name.Length - o1.Length);
      else if(name.EndsWith(o2))
        name = name.Substring(0, name.Length - o2.Length);

      return name;
    }

    /// <inheritdoc/>
    public override bool Apply(bool disposeController)
    {
      if(InterpolationDetailsController is not null)
      {
        if (InterpolationDetailsController.Apply(disposeController))
          _doc = (IInterpolationFunctionOptions)InterpolationDetailsController.ModelObject;
        else
          return ApplyEnd(false, disposeController);
      }

      return ApplyEnd(true, disposeController);
    }
  }
}
