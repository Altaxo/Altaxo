#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2014 Dr. Dirk Lellinger
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
using Altaxo.Collections;
using Altaxo.Graph.Gdi.Shapes;

namespace Altaxo.Gui.Graph.Gdi.Shapes
{
  public interface IGraphicItemsView
  {
    SelectableListNodeList ItemsList { set; }

    event Action SelectedItemsUp;

    event Action SelectedItemsDown;

    event Action SelectedItemsRemove;
  }

  [UserControllerForObject(typeof(GraphicCollection))]
  [ExpectedTypeOfView(typeof(IGraphicItemsView))]
  public class GraphicItemsController : MVCANControllerEditCopyOfDocBase<GraphicCollection, IGraphicItemsView>
  {
    private SelectableListNodeList _itemsList;

    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    public override void Dispose(bool isDisposing)
    {
      _itemsList = null;
      base.Dispose(isDisposing);
    }

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        _itemsList = new SelectableListNodeList();

        foreach (var item in _doc)
        {
          var node = new SelectableListNode(item.ToString(), item, false);
          _itemsList.Add(node);
        }
      }
      if (null != _view)
      {
        _view.ItemsList = _itemsList;
      }
    }

    public override bool Apply(bool disposeController)
    {
      using (var token = _doc.GetEventDisableToken())
      {
        _doc.Clear();
        foreach (var node in _itemsList)
        {
          _doc.Add((IGraphicBase)node.Tag);
        }
      }
      return ApplyEnd(true, disposeController);
    }

    protected override void AttachView()
    {
      base.AttachView();

      _view.SelectedItemsUp += EhSelectedItemsUp;
      _view.SelectedItemsDown += EhSelectedItemsDown;
      _view.SelectedItemsRemove += EhSelectedItemsRemove;
    }

    protected override void DetachView()
    {
      _view.SelectedItemsUp -= EhSelectedItemsUp;
      _view.SelectedItemsDown -= EhSelectedItemsDown;
      _view.SelectedItemsRemove -= EhSelectedItemsRemove;

      base.DetachView();
    }

    private void EhSelectedItemsRemove()
    {
      _itemsList.RemoveSelectedItems();
    }

    private void EhSelectedItemsDown()
    {
      _itemsList.MoveSelectedItemsDown();
    }

    private void EhSelectedItemsUp()
    {
      _itemsList.MoveSelectedItemsUp();
    }
  }
}
