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
  public class CoordinateSystemController : IMVCANController
  {
    ITypeAndInstanceView _view;
    G2DCoordinateSystem _originalDoc;
    G2DCoordinateSystem _doc;

    IMVCAController _instanceController;

		SelectableListNodeList _choiceList;
		UseDocument _useDocumentCopy;

		/// <summary>Holds all instantiable subtypes of G2DCoordinateSystem</summary>
		Type[] _cosSubTypes;


      #region IMVCController Members

		public bool InitializeDocument(params object[] args)
		{
			if (null == args || args.Length == 0 || !(args[0] is G2DCoordinateSystem))
				return false;

			_originalDoc = (G2DCoordinateSystem)args[0];
			if (_useDocumentCopy == UseDocument.Copy)
				_doc = (G2DCoordinateSystem)_originalDoc.Clone();
			else
				_doc = _originalDoc;

			Initialize(true);

			return true;
		}

		public UseDocument UseDocumentCopy
		{
			set { _useDocumentCopy = value; }
		}

    void Initialize(bool initData)
    {
			if (initData)
			{
				// look for coordinate system types
				if(null==_cosSubTypes)
				_cosSubTypes = Main.Services.ReflectionService.GetNonAbstractSubclassesOf(typeof(G2DCoordinateSystem));

				if(null==_choiceList)
					_choiceList = new SelectableListNodeList();
				_choiceList.Clear();
				foreach (Type t in _cosSubTypes)
					_choiceList.Add(new SelectableListNode(Current.Gui.GetUserFriendlyClassName(t), t, t == _doc.GetType()));
			}

      if (_view != null)
      {
        // look for a controller-control
        _view.TypeLabel="Type:";
				_view.InitializeTypeNames(_choiceList);

        // To avoid looping when a dedicated controller is unavailable, we first instantiate the controller alone and compare the types
        _instanceController = (IMVCAController)Current.Gui.GetController(new object[] { _doc }, typeof(IMVCAController));
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
			var sel = _choiceList.FirstSelectedNode;

      if (sel != null)
      {
        System.Type t = (System.Type)sel.Tag;
        if (_doc.GetType() != t)
        {
          _doc = (G2DCoordinateSystem)Activator.CreateInstance((System.Type)sel.Tag);
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
				if (null != _view)
				{
					_view.TypeChoiceChanged -= EhTypeChoiceChanged;
				}

				_view = value as ITypeAndInstanceView;

				if (null != _view)
				{
					Initialize(false);
					_view.TypeChoiceChanged += EhTypeChoiceChanged;
				}
      }
    }

    public object ModelObject
    {
      get { return _originalDoc; }
    }

    #endregion

    #region IApplyController Members

    public bool Apply()
    {
      
      bool result = _instanceController==null || _instanceController.Apply();
      if (true == result)
      {
				_originalDoc = _doc;
      }
      return result;

    }

    #endregion




	}
}
