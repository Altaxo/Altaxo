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

using Altaxo.Graph.Gdi.Plot;
using Altaxo.Gui.Common;

namespace Altaxo.Gui.Graph
{
  [UserControllerForObject(typeof(DensityImagePlotItem))]
  class DensityImagePlotItemController : TabbedElementController, IMVCANController
  {

    UseDocument _useDocument;
    DensityImagePlotItem _doc;
    DensityImagePlotItem _tempdoc;

    IMVCANController _styleController;

    public DensityImagePlotItemController()
    {
    }

    public bool InitializeDocument(params object[] args)
    {
      if (args == null || args.Length == 0)
        return false;

      if (!(args[0] is DensityImagePlotItem))
        return false;
      else
        _doc = _tempdoc = (DensityImagePlotItem)args[0];

     

      if (_useDocument == UseDocument.Copy)
        _tempdoc = (DensityImagePlotItem)_doc.Clone();

      InitializeStyle();
      BringTabToFront(0);

      return true;
    }

    public UseDocument UseDocumentCopy
    {
      set { _useDocument = value; }
    }

    void InitializeStyle()
    {
      _styleController = (IMVCANController)Current.Gui.GetControllerAndControl(new object[] { _tempdoc.Style }, typeof(IMVCANController), UseDocument.Directly);
      this.AddTab("Style", _styleController, _styleController.ViewObject);
    }


    #region IMVCController Members


    public override object ModelObject
    {
      get { return _doc; }
    }

    #endregion

    #region IApplyController Members

    public override bool Apply()
    {
      bool result = true;

      if (_styleController != null)
        result &= _styleController.Apply();

      return result;
    }

    #endregion
  }
}
