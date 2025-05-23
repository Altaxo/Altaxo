﻿#region Copyright

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
using System.Collections.Generic;
using Altaxo.Collections;
using Altaxo.Graph.Gdi;
using Altaxo.Graph.Gdi.Shapes;
using Altaxo.Gui.Common;

namespace Altaxo.Gui.Graph.Gdi.Shapes
{
  public interface IShapeGroupView : IDataContextAwareView
  {
  }

  [UserControllerForObject(typeof(ShapeGroup))]
  [ExpectedTypeOfView(typeof(IShapeGroupView))]
  public class ShapeGroupController : MVCANControllerEditOriginalDocBase<ShapeGroup, IShapeGroupView>
  {
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield return new ControllerAndSetNullMethod(_locationController, () => LocationController = null);
    }

    public ShapeGroupController()
    {
      CmdEditItem = new RelayCommand(EhEditSelectedItem);
    }

    #region Bindings

    public System.Windows.Input.ICommand CmdEditItem { get; }

    private IMVCANController _locationController;

    public IMVCANController LocationController
    {
      get => _locationController;
      set
      {
        if (!(_locationController == value))
        {
          _locationController?.Dispose();
          _locationController = value;
          OnPropertyChanged(nameof(LocationController));
        }
      }
    }

    private ItemsController<GraphicBase> _groupedItems;

    public ItemsController<GraphicBase> GroupedItems
    {
      get => _groupedItems;
      set
      {
        if (!(_groupedItems == value))
        {
          _groupedItems?.Dispose();
          _groupedItems = value;
          OnPropertyChanged(nameof(GroupedItems));
        }
      }
    }

    #endregion

    public override void Dispose(bool isDisposing)
    {
      GroupedItems = null;
      base.Dispose(isDisposing);
    }

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        var itemList = new SelectableListNodeList();
        foreach (var d in _doc.GroupedObjects)
        {
          var node = new SelectableListNode(d.GetType().ToString(), d, false);
          itemList.Add(node);
        }
        GroupedItems = new ItemsController<GraphicBase>(itemList);

        var locationController = (IMVCANController)Current.Gui.GetController(new object[] { _doc.Location }, typeof(IMVCANController), UseDocument.Directly);
        Current.Gui.FindAndAttachControlTo(locationController);
        LocationController = locationController;
      }
    }

    public override bool Apply(bool disposeController)
    {
      if (!_locationController.Apply(disposeController))
        return ApplyEnd(false, disposeController);

      if (!object.ReferenceEquals(_doc.Location, _locationController.ModelObject))
        _doc.Location.CopyFrom((ItemLocationDirect)_locationController.ModelObject);

      return ApplyEnd(true, disposeController);
    }

    private void EhEditSelectedItem()
    {
      _locationController.Apply(false);

      if (GroupedItems.SelectedValue is { } item)
      {
        if (Current.Gui.ShowDialog(ref item, "Edit shape group item " + GroupedItems.SelectedItem.Text, true))
        {
          _doc.AdjustPosition();
          UpdateLocationView();
        }
      }
    }

    private void UpdateLocationView()
    {
      var locationController = (IMVCANController)Current.Gui.GetController(new object[] { _doc.Location }, typeof(IMVCANController), UseDocument.Directly);
      Current.Gui.FindAndAttachControlTo(locationController);
      LocationController = locationController;
    }
  }
}
