using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Gui.Common
{
  public interface IPropertyView
  {
    object[] SelectedObjectsToView { get; set; }
  }

  [ExpectedTypeOfView(typeof(IPropertyView))]
  public class PropertyController : IMVCAController
  {
    IPropertyView _view;
    object _doc;

    public PropertyController(object doc)
    {
      _doc = doc;
      Initialize();
    }

    void Initialize()
    {
      if (_view != null)
      {
        _view.SelectedObjectsToView = new object[] { _doc };
      }
    }


    #region IMVCController Members

    public object ViewObject
    {
      get
      {
        return _view;
      }
      set
      {
        _view = (IPropertyView)value;
        Initialize();
      }
    }

    public object ModelObject
    {
      get { return _doc; }
    }

    public bool Apply()
    {
      return true;
    }

    #endregion
  }
}
