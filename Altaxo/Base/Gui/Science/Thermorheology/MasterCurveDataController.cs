#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2024 Dr. Dirk Lellinger
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
using System.Linq;
using Altaxo.Collections;
using Altaxo.Data;
using Altaxo.Gui.Common;
using Altaxo.Science.Thermorheology.MasterCurves;

namespace Altaxo.Gui.Science.Thermorheology
{
  public interface IMasterCurveDataView : IDataContextAwareView
  {
  }

  [ExpectedTypeOfView(typeof(IMasterCurveDataView))]
  [UserControllerForObject(typeof(MasterCurveData))]
  public class MasterCurveDataController : MVCANControllerEditCopyOfDocBase<MasterCurveData, IMasterCurveDataView>
  {
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    #region Bindings

    private ItemsController<int> _dataGroup;

    public ItemsController<int> DataGroup
    {
      get => _dataGroup;
      set
      {
        if (!(_dataGroup == value))
        {
          _dataGroup = value;
          OnPropertyChanged(nameof(DataGroup));
        }
      }
    }


    private ItemsController<XAndYColumn?> _dataItems;

    public ItemsController<XAndYColumn?> DataItems
    {
      get => _dataItems;
      set
      {
        if (!(_dataItems == value))
        {
          _dataItems?.Dispose();
          _dataItems = value;
          OnPropertyChanged(nameof(DataItems));
        }
      }
    }


    #endregion

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      int maxGroup = 1;
      if (initData)
      {
        for (int i = 0; i < _doc.CurveData.Count; i++)
        {
          var curves = _doc.CurveData[i];
          for (int j = 0; j < curves.Length; j++)
          {
            if (curves[j] is not null)
              maxGroup = Math.Max(maxGroup, j + 1);
          }
        }

        DataGroup = new ItemsController<int>(new SelectableListNodeList(
          Enumerable.Range(0, maxGroup).Select(i => new SelectableListNode($"Grp{i}", i, false))
          ), EhSelectedGroupChanged);
        DataGroup.SelectedValue = 0;


      }
    }

    private void EhSelectedGroupChanged(int selectedGroup)
    {

      var itemsCtrl = new ItemsController<XAndYColumn>(new SelectableListNodeList(
        _doc.CurveData.Select(curves =>
        {
          var xy = selectedGroup < curves.Length ? curves[selectedGroup] : null;
          return new SelectableListNode(xy?.ToString() ?? string.Empty, xy, false);
        })));
    }

    public override bool Apply(bool disposeController)
    {
      return ApplyEnd(true, disposeController);
    }
  }
}
