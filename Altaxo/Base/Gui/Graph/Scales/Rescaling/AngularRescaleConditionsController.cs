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

using Altaxo.Collections;
using Altaxo.Graph.Scales.Rescaling;
using System;
using System.Collections.Generic;
using System.Text;

namespace Altaxo.Gui.Graph.Scales.Rescaling
{
  #region Interfaces

  public interface IAngularRescaleConditionsView
  {
    SelectableListNodeList Origin { set; }
  }

  #endregion Interfaces

  [UserControllerForObject(typeof(AngularRescaleConditions))]
  [ExpectedTypeOfView(typeof(IAngularRescaleConditionsView))]
  public class AngularRescaleConditionsController : MVCANControllerEditOriginalDocBase<AngularRescaleConditions, IAngularRescaleConditionsView>
  {
    private SelectableListNodeList _originList;

    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    public override void Dispose(bool isDisposing)
    {
      _originList = null;
      base.Dispose(isDisposing);
    }

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        BuildOriginList();
      }

      if (_view != null)
      {
        _view.Origin = _originList;
      }
    }

    public override bool Apply(bool disposeController)
    {
      _doc.ScaleOrigin = (int)_originList.FirstSelectedNode.Tag;

      return ApplyEnd(true, disposeController);
    }

    private void BuildOriginList()
    {
      _originList = new SelectableListNodeList();
      _originList.Add(new SelectableListNode("-90°", -1, -1 == _doc.ScaleOrigin));
      _originList.Add(new SelectableListNode("0°", 0, 0 == _doc.ScaleOrigin));
      _originList.Add(new SelectableListNode("90°", 1, 1 == _doc.ScaleOrigin));
      _originList.Add(new SelectableListNode("180°", 2, 2 == _doc.ScaleOrigin));
      _originList.Add(new SelectableListNode("270°", 3, 3 == _doc.ScaleOrigin));
    }
  }
}
