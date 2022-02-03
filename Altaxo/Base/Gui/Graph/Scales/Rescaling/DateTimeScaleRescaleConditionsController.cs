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
  public interface IDateTimeScaleRescaleConditionsView : IDataContextAwareView
  {
  }

  [ExpectedTypeOfView(typeof(IDateTimeScaleRescaleConditionsView))]
  [UserControllerForObject(typeof(DateTimeScaleRescaleConditions))]
  public class DateTimeScaleRescaleConditionsController : MVCANControllerEditOriginalDocBase<DateTimeScaleRescaleConditions, IDateTimeScaleRescaleConditionsView>
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

    private void EhOrgRelativeToChanged(BoundariesRelativeTo value)
    {
        ShowOrgTS = value != BoundariesRelativeTo.Absolute;
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

    private void EhEndRelativeToChanged(BoundariesRelativeTo value)
    {
        ShowEndTS = value != BoundariesRelativeTo.Absolute;
    }

    private DateTime _orgValueDT;

    public DateTime OrgValueDT
    {
      get => _orgValueDT;
      set
      {
        if (!(_orgValueDT == value))
        {
          _orgValueDT = value;
          OnPropertyChanged(nameof(OrgValueDT));
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


    private DateTime _endValueDT;

    public DateTime EndValueDT
    {
      get => _endValueDT;
      set
      {
        if (!(_endValueDT == value))
        {
          _endValueDT = value;
          OnPropertyChanged(nameof(EndValueDT));
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

    private TimeSpan _orgValueTS;

    public TimeSpan OrgValueTS
    {
      get => _orgValueTS;
      set
      {
        if (!(_orgValueTS == value))
        {
          _orgValueTS = value;
          OnPropertyChanged(nameof(OrgValueTS));
        }
      }
    }
    private TimeSpan _endValueTS;

    public TimeSpan EndValueTS
    {
      get => _endValueTS;
      set
      {
        if (!(_endValueTS == value))
        {
          _endValueTS = value;
          OnPropertyChanged(nameof(EndValueTS));
        }
      }
    }

    private bool _showOrgTS;

    public bool ShowOrgTS
    {
      get => _showOrgTS;
      set
      {
        if (!(_showOrgTS == value))
        {
          _showOrgTS = value;
          OnPropertyChanged(nameof(ShowOrgTS));
        }
      }
    }
    private bool _showEndTS;

    public bool ShowEndTS
    {
      get => _showEndTS;
      set
      {
        if (!(_showEndTS == value))
        {
          _showEndTS = value;
          OnPropertyChanged(nameof(ShowEndTS));
        }
      }
    }


    #endregion


    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        _orgRescaling = new ItemsController<BoundaryRescaling>(LinearScaleRescaleConditionsController.CreateListNodeList(_doc.OrgRescaling));
        _endRescaling = new ItemsController<BoundaryRescaling>(LinearScaleRescaleConditionsController.CreateListNodeList(_doc.EndRescaling));

        _orgRelativeTo = new ItemsController<BoundariesRelativeTo>(new SelectableListNodeList(_doc.OrgRelativeTo), EhOrgRelativeToChanged);
        _endRelativeTo = new ItemsController<BoundariesRelativeTo>(new SelectableListNodeList(_doc.EndRelativeTo), EhEndRelativeToChanged);

        var org = GetOrgValueToShow();
        var end = GetEndValueToShow();

        if (org is DateTime)
        {
          OrgValueDT = (DateTime)org;
          ShowOrgTS = false;
        }
        else
        {
         OrgValueTS = (TimeSpan)org;
         ShowOrgTS = true;
        }

        if (end is DateTime)
        {
         EndValueDT = (DateTime)end;
          ShowEndTS = false;
        }
        else
        {
          EndValueTS = (TimeSpan)end;
         ShowEndTS = true;
        }
      }
    }

    public override bool Apply(bool disposeController)
    {
      var orgRescaling = _orgRescaling.SelectedValue;
      var endRescaling = _endRescaling.SelectedValue;

      var orgRelativeTo = _orgRelativeTo.SelectedValue;
      var endRelativeTo = _endRelativeTo.SelectedValue;

      long orgValue, endValue;
      DateTimeKind orgKind, endKind;
      if (orgRelativeTo == BoundariesRelativeTo.Absolute)
      {
        orgValue = OrgValueDT.Ticks;
        orgKind = OrgValueDT.Kind;
      }
      else
      {
        orgValue = OrgValueTS.Ticks;
        orgKind = DateTimeKind.Utc;
      }
      if (endRelativeTo == BoundariesRelativeTo.Absolute)
      {
        endValue = EndValueDT.Ticks;
        endKind = EndValueDT.Kind;
      }
      else
      {
        endValue = EndValueTS.Ticks;
        endKind = DateTimeKind.Utc;
      }

      _doc.SetUserParameters(orgRescaling, orgRelativeTo, orgValue, orgKind, endRescaling, endRelativeTo, endValue, endKind);

      return ApplyEnd(true, disposeController);
    }

   

    private object GetOrgValueToShow()
    {
      if (_doc.OrgRescaling == BoundaryRescaling.Auto)
      {
        DateTime orgValue = _doc.ResultingOrg;
        if (_doc.ParentObject is DateTimeScale)
          orgValue = ((DateTimeScale)_doc.ParentObject).Org;

        switch (_doc.EndRelativeTo)
        {
          case BoundariesRelativeTo.Absolute:
            return orgValue;

          case BoundariesRelativeTo.RelativeToDataBoundsOrg:
            return orgValue - _doc.DataBoundsOrg;

          case BoundariesRelativeTo.RelativeToDataBoundsEnd:
            return orgValue - _doc.DataBoundsEnd;

          case BoundariesRelativeTo.RelativeToDataBoundsMean:
            return orgValue - (_doc.DataBoundsOrg + TimeSpan.FromTicks((_doc.DataBoundsEnd - _doc.DataBoundsOrg).Ticks / 2));

          default:
            throw new NotImplementedException();
        }
      }
      else
      {
        if (_doc.OrgRelativeTo == BoundariesRelativeTo.Absolute)
          return new DateTime(_doc.UserProvidedOrgValue, _doc.UserProvidedOrgKind);
        else
          return TimeSpan.FromTicks(_doc.UserProvidedOrgValue);
      }
    }

    private object GetEndValueToShow()
    {
      if (_doc.EndRescaling == BoundaryRescaling.Auto)
      {
        DateTime endValue = _doc.ResultingEnd;
        if (_doc.ParentObject is DateTimeScale)
          endValue = ((DateTimeScale)_doc.ParentObject).End;

        switch (_doc.EndRelativeTo)
        {
          case BoundariesRelativeTo.Absolute:
            return endValue;

          case BoundariesRelativeTo.RelativeToDataBoundsEnd:
            return TimeSpan.FromTicks(0);

          case BoundariesRelativeTo.RelativeToDataBoundsOrg:
            return (endValue - _doc.DataBoundsOrg);

          case BoundariesRelativeTo.RelativeToDataBoundsMean:
            return endValue - (_doc.DataBoundsOrg + TimeSpan.FromTicks((_doc.DataBoundsEnd - _doc.DataBoundsOrg).Ticks / 2));

          default:
            throw new NotImplementedException();
        }
      }
      else
      {
        if (_doc.EndRelativeTo == BoundariesRelativeTo.Absolute)
          return new DateTime(_doc.UserProvidedEndValue, _doc.UserProvidedEndKind);
        else
          return TimeSpan.FromTicks(_doc.UserProvidedEndValue);
      }
    }
  }
}
