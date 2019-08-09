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

using System;
using System.Collections.Generic;
using System.Text;
using Altaxo.Graph.Gdi.CS;

namespace Altaxo.Gui.Graph.Gdi.CS
{
  #region Interfaces

  public interface IG2DCartesicCSView
  {
    bool ExchangeXY { get; set; }

    bool ReverseX { get; set; }

    bool ReverseY { get; set; }
  }

  #endregion Interfaces

  [UserControllerForObject(typeof(G2DCartesicCoordinateSystem), 101)]
  [ExpectedTypeOfView(typeof(IG2DCartesicCSView))]
  public class G2DCartesicCSController : MVCANControllerEditOriginalDocBase<G2DCartesicCoordinateSystem, IG2DCartesicCSView>
  {
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    public G2DCartesicCSController()
    {
    }

    public G2DCartesicCSController(G2DCartesicCoordinateSystem doc)
    {
      InitializeDocument(doc);
    }

    #region IMVCController Members

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (_view != null)
      {
        _view.ExchangeXY = _doc.IsXYInterchanged;
        _view.ReverseX = _doc.IsXReverse;
        _view.ReverseY = _doc.IsYReverse;
      }
    }

    public override bool Apply(bool disposeController)
    {
      _doc.IsXYInterchanged = _view.ExchangeXY;
      _doc.IsXReverse = _view.ReverseX;
      _doc.IsYReverse = _view.ReverseY;
      return ApplyEnd(true, disposeController);
    }

    #endregion IMVCController Members
  }
}
