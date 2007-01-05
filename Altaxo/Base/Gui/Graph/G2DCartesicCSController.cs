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
using Altaxo.Graph.Gdi.CS;

namespace Altaxo.Gui.Graph
{
  [UserControllerForObject(typeof(G2DCartesicCoordinateSystem),101)]
  [ExpectedTypeOfView(typeof(IG2DCartesicCSView))]
  public class G2DCartesicCSController : IMVCAController
  {
    IG2DCartesicCSView _view;
    G2DCartesicCoordinateSystem _doc;

    public G2DCartesicCSController(G2DCartesicCoordinateSystem doc)
    {
      _doc = doc;
      Initialize(true);
    }

#region IMVCController Members


    void Initialize(bool bInit)
    {
      if (_view != null)
      {
        _view.ExchangeXY = _doc.IsXYInterchanged;
        _view.ReverseX = _doc.IsXReverse;
        _view.ReverseY = _doc.IsYReverse;
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
        _view = value as IG2DCartesicCSView;
        Initialize(false);
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
      _doc.IsXYInterchanged = _view.ExchangeXY;
      _doc.IsXReverse = _view.ReverseX;
      _doc.IsYReverse = _view.ReverseY;
      return true;

    }

    #endregion


  }
}
