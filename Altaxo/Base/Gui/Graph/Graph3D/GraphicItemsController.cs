#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2022 Dr. Dirk Lellinger
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
using System.Windows.Input;
using Altaxo.Collections;
using Altaxo.Graph.Graph3D;
using Altaxo.Graph.Graph3D.Shapes;
using Altaxo.Gui.Common;

namespace Altaxo.Gui.Graph.Graph3D
{
  public interface IGraphicItemsView : IDataContextAwareView
  {
  }

  [UserControllerForObject(typeof(GraphicCollection))]
  [ExpectedTypeOfView(typeof(IGraphicItemsView))]
  public class GraphicItemsController : MVCANControllerEditCopyOfDocBase<GraphicCollection, IGraphicItemsView>
  {
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    public GraphicItemsController()
    {
      CmdItemsUp = new RelayCommand(EhSelectedItemsUp);
      CmdItemsDown = new RelayCommand(EhSelectedItemsDown);
      CmdItemsRemove = new RelayCommand(EhSelectedItemsRemove);
      CmdItemEdit = new RelayCommand(EhSelectedItemEdit);
    }

    #region Bindings

    public ICommand CmdItemsUp { get; }
    public ICommand CmdItemsDown { get; }
    public ICommand CmdItemsRemove { get; }
    public ICommand CmdItemEdit { get; }

    private SelectableListNodeList _items;

    public SelectableListNodeList Items
    {
      get => _items;
      set
      {
        if (!(_items == value))
        {
          _items = value;
          OnPropertyChanged(nameof(Items));
        }
      }
    }


    #endregion

    public override void Dispose(bool isDisposing)
    {
      Items = null;
      base.Dispose(isDisposing);
    }

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        var itemsList = new SelectableListNodeList();

        foreach (var item in _doc)
        {
          var node = new SelectableListNode(item.ToString(), item, false);
          itemsList.Add(node);
        }
        Items =itemsList;
      }
    }

    public override bool Apply(bool disposeController)
    {
      using (var token = _doc.GetEventDisableToken())
      {
        _doc.Clear();
        foreach (var node in _items)
        {
          _doc.Add((IGraphicBase)node.Tag);
        }
      }
      return ApplyEnd(true, disposeController);
    }

    private void EhSelectedItemsRemove()
    {
      _items.RemoveSelectedItems();
    }

    private void EhSelectedItemsDown()
    {
      _items.MoveSelectedItemsDown();
    }

    private void EhSelectedItemsUp()
    {
      _items.MoveSelectedItemsUp();
    }

    private void EhSelectedItemEdit()
    {
      if (_items.FirstSelectedNode?.Tag is IGraphicBase item)
      {
        var ctrl = (IMVCAController)Current.Gui.GetControllerAndControl(new object[] { item }, typeof(IMVCAController));
        if (ctrl is not null)
        {
          Current.Gui.ShowDialog(ctrl, "Edit graphic item");
        }
      }
    }
  }
}
