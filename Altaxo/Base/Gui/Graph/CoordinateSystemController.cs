using System;
using System.Collections.Generic;
using System.Text;

using Altaxo.Graph.Gdi;
using Altaxo.Collections;
using Altaxo.Gui.Common;

namespace Altaxo.Gui.Graph
{
  [Main.GUI.UserControllerForObject(typeof(G2DCoordinateSystem))]
  [Main.GUI.ExpectedTypeOfView(typeof(ITypeAndInstanceView))]
  public class CoordinateSystemController : Main.GUI.IMVCAController
  {
    ITypeAndInstanceView _view;
    G2DCoordinateSystem _doc;
    G2DCoordinateSystem _tempdoc;

    Main.GUI.IMVCAController _instanceController;

    public CoordinateSystemController(G2DCoordinateSystem doc)
    {
      _doc = doc;
      _tempdoc = (G2DCoordinateSystem)doc.Clone();
      Initialize(true);
    }


      #region IMVCController Members


    void Initialize(bool bInit)
    {
      if (_view != null)
      {
        // look for coordinate system types
        Type[] subtypes = Main.Services.ReflectionService.GetNonAbstractSubclassesOf(typeof(G2DCoordinateSystem));

        ListNodeList list = new ListNodeList();
        int selection = -1;
        foreach(Type t in subtypes)
          list.Add(new ListNode(Current.Gui.GetUserFriendlyClassName(t), t));

        // look for a controller-control
        _instanceController = (Main.GUI.IMVCAController)Current.Gui.GetControllerAndControl(new object[]{_tempdoc},typeof(Main.GUI.IMVCAController));
        _view.TypeLabel="Type";
        _view.InitializeTypeNames(list, list.IndexOfObject(_tempdoc.GetType()));
        _view.SetInstanceControl(_instanceController.ViewObject);
      }
    }

    void EhTypeChoiceChanged(object sender, EventArgs e)
    {
      ListNode sel = _view.SelectedNode;

      if (sel != null)
      {
        System.Type t = (System.Type)sel.Item;
        if (_tempdoc.GetType() != t)
        {
          _tempdoc = (G2DCoordinateSystem)Activator.CreateInstance((System.Type)sel.Item);
          Initialize(true);
        }
      }
    }

    public object ViewObject
    {
      get
      {
        return _view;
      }
      set
      {
        ITypeAndInstanceView oldvalue = _view;
        _view = value as ITypeAndInstanceView;

        if (oldvalue != null)
          oldvalue.TypeChoiceChanged -= EhTypeChoiceChanged;
        
        Initialize(false);

        if (_view != null)
          _view.TypeChoiceChanged += EhTypeChoiceChanged;
      }
    }

    public object ModelObject
    {
      get { return _doc; }
    }

    #endregion

    #region IApplyController Members

    public bool Apply()
    {
      bool result = _instanceController.Apply();
      if (true == result)
      {
        _doc = _tempdoc;
      }
      return result;

    }

    #endregion


  }
}
