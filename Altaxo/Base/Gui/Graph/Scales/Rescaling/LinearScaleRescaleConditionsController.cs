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

#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Altaxo.Collections;
using Altaxo.Graph.Scales;
using Altaxo.Graph.Scales.Rescaling;
using Altaxo.Gui.Common;
using Altaxo.Serialization;

namespace Altaxo.Gui.Graph.Scales.Rescaling
{
  public interface ILinearScaleRescaleConditionsView : IDataContextAwareView
  {
  }

  [ExpectedTypeOfView(typeof(ILinearScaleRescaleConditionsView))]
  [UserControllerForObject(typeof(LinearScaleRescaleConditions))]
  public class LinearScaleRescaleConditionsController : MVCANControllerEditOriginalDocBase<LinearScaleRescaleConditions, ILinearScaleRescaleConditionsView>
  {
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    #region Bindings

    private ItemsController<BoundaryRescaling> _orgRescaling;

    public ItemsController<BoundaryRescaling> OrgRescaling
    {
      get => _orgRescaling;
      set
      {
        if (!(_orgRescaling == value))
        {
          _orgRescaling = value;
          OnPropertyChanged(nameof(OrgRescaling));
        }
      }
    }
    private ItemsController<BoundaryRescaling> _endRescaling;

    public ItemsController<BoundaryRescaling> EndRescaling
    {
      get => _endRescaling;
      set
      {
        if (!(_endRescaling == value))
        {
          _endRescaling = value;
          OnPropertyChanged(nameof(EndRescaling));
        }
      }
    }

    private ItemsController<BoundariesRelativeTo> _orgRelativeTo;

    public ItemsController<BoundariesRelativeTo> OrgRelativeTo
    {
      get => _orgRelativeTo;
      set
      {
        if (!(_orgRelativeTo == value))
        {
          _orgRelativeTo = value;
          OnPropertyChanged(nameof(OrgRelativeTo));
        }
      }
    }

    private ItemsController<BoundariesRelativeTo> _endRelativeTo;

    public ItemsController<BoundariesRelativeTo> EndRelativeTo
    {
      get => _endRelativeTo;
      set
      {
        if (!(_endRelativeTo == value))
        {
          _endRelativeTo = value;
          OnPropertyChanged(nameof(EndRelativeTo));
        }
      }
    }

    private double _orgValue;

    public double OrgValue
    {
      get => _orgValue;
      set
      {
        if (!(_orgValue == value))
        {
          _orgValue = value;
          OnPropertyChanged(nameof(OrgValue));
          EhOrgValueChanged();
        }
      }
    }

    private void EhOrgValueChanged()
    {
      if (OrgRescaling.SelectedValue == BoundaryRescaling.Auto)
      {
        OrgRescaling.SelectedValue = BoundaryRescaling.AutoTempFixed;
      }
    }


    private double _endValue;

    public double EndValue
    {
      get => _endValue;
      set
      {
        if (!(_endValue == value))
        {
          _endValue = value;
          OnPropertyChanged(nameof(EndValue));
          EhEndValueChanged();
        }
      }
    }

    private void EhEndValueChanged()
    {
      if (EndRescaling.SelectedValue == BoundaryRescaling.Auto)
      {
        EndRescaling.SelectedValue = BoundaryRescaling.AutoTempFixed;
      }
    }


    #endregion

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        _orgRescaling = new ItemsController<BoundaryRescaling>(CreateListNodeList(_doc.OrgRescaling));
        _endRescaling = new ItemsController<BoundaryRescaling>(CreateListNodeList(_doc.EndRescaling));
        _orgRelativeTo = new ItemsController<BoundariesRelativeTo>(new SelectableListNodeList(_doc.OrgRelativeTo));
        _endRelativeTo = new ItemsController<BoundariesRelativeTo>(new SelectableListNodeList(_doc.EndRelativeTo));
        _orgValue = GetOrgValueToShow();
        _endValue = GetEndValueToShow();
      }
    }

    public override bool Apply(bool disposeController)
    {
      var orgRescaling = OrgRescaling.SelectedValue;
      var endRescaling = EndRescaling.SelectedValue;
      var orgRelativeTo = OrgRelativeTo.SelectedValue;
      var endRelativeTo = EndRelativeTo.SelectedValue;
      var orgValue = OrgValue;
      var endValue = EndValue;
      _doc.SetUserParameters(orgRescaling, orgRelativeTo, orgValue, endRescaling, endRelativeTo, endValue);

      return ApplyEnd(true, disposeController);
    }

    #region Special Listnode class with description

    private class ListNodeWithDescription : SelectableListNode
    {
      private string _description;

      public ListNodeWithDescription(string text, object tag, bool isSelected, string description)
        : base(text, tag, isSelected)
      {
        _description = description;
      }

      public override string Description
      {
        get
        {
          return _description;
        }
      }
    }

    #endregion Special Listnode class with description

    public static SelectableListNodeList CreateListNodeList(BoundaryRescaling rescalingValue)
    {
      var result = new SelectableListNodeList();
      var values = System.Enum.GetValues(rescalingValue.GetType());
      foreach (var value in values)
      {
        Type t = value.GetType();
        string description = Current.ResourceService.GetString("ClassNames." + t.FullName + "." + Enum.GetName(t, value) + ".Description");

        result.Add(new ListNodeWithDescription(value.ToString(), value, value.ToString() == rescalingValue.ToString(), description));
      }
      return result;
    }

    private double GetOrgValueToShow()
    {
      if (_doc.ParentObject is Scale)
        return _doc.GetOrgValueToShowInDialog(((Scale)_doc.ParentObject).OrgAsVariant);
      else
        return _doc.GetOrgValueToShowInDialog(_doc.ResultingOrg);
    }

    private double GetEndValueToShow()
    {
      if (_doc.ParentObject is Scale)
        return _doc.GetEndValueToShowInDialog(((Scale)_doc.ParentObject).EndAsVariant);
      else
        return _doc.GetOrgValueToShowInDialog(_doc.ResultingEnd);
    }
  }
}
