#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2007 Dr. Dirk Lellinger
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
#endregion

using System;
using System.Collections.Generic;
using System.Text;

using Altaxo.Graph.Gdi;
using Altaxo.Collections;
using Altaxo.Gui.Common;

namespace Altaxo.Gui.Graph
{
  [UserControllerForObject(typeof(G2DCoordinateSystem))]
  [ExpectedTypeOfView(typeof(ITypeAndInstanceView))]
  public class CoordinateSystemController : IMVCAController
  {
    ITypeAndInstanceView _view;
    G2DCoordinateSystem _doc;
    G2DCoordinateSystem _tempdoc;

    IMVCAController _instanceController;

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
        foreach(Type t in subtypes)
          list.Add(new ListNode(Current.Gui.GetUserFriendlyClassName(t), t));

        // look for a controller-control
        _view.TypeLabel="Type";
        _view.InitializeTypeNames(list, list.IndexOfObject(_tempdoc.GetType()));

        // To avoid looping when a dedicated controller is unavailable, we first instantiate the controller alone and compare the types
        _instanceController = (IMVCAController)Current.Gui.GetController(new object[] { _tempdoc }, typeof(IMVCAController));
        if (_instanceController != null && (_instanceController.GetType() != this.GetType()))
        {
          Current.Gui.FindAndAttachControlTo(_instanceController);
          if (_instanceController.ViewObject != null)
            _view.SetInstanceControl(_instanceController.ViewObject);
        }
        else
        {
          _instanceController = null;
          _view.SetInstanceControl(null);
        }
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
      
      bool result = _instanceController==null || _instanceController.Apply();
      if (true == result)
      {
        _doc = _tempdoc;
      }
      return result;

    }

    #endregion


  }
}
