#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2016 Dr. Dirk Lellinger
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

#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Altaxo.Graph.Graph3D.Plot.Groups;
using Altaxo.Gui.Drawing;

namespace Altaxo.Gui.Graph.Graph3D.Plot.Groups
{
  [ExpectedTypeOfView(typeof(IStyleListView))]
  [UserControllerForObject(typeof(ScatterSymbolGroupStyle))]
  public class ScatterSymbolGroupStyleController : MVCANControllerEditOriginalDocBase<ScatterSymbolGroupStyle, IStyleListView>
  {
    private ScatterSymbolListController _listController;

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        _listController = new ScatterSymbolListController();
        _listController.InitializeDocument(_doc.ListOfValues);
      }

      if (null != _view)
      {
        _listController.ViewObject = _view;
      }
    }

    public override bool Apply(bool disposeController)
    {
      if (!_listController.Apply(disposeController))
        return ApplyEnd(false, disposeController);

      _doc.ListOfValues = (ScatterSymbolList)_listController.ModelObject;

      return ApplyEnd(true, disposeController);
    }

    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield return new ControllerAndSetNullMethod(_listController, () => _listController = null);
    }
  }
}
