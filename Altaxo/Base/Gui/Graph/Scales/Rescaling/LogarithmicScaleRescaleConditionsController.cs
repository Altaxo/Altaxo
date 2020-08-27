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
using Altaxo.Serialization;

namespace Altaxo.Gui.Graph.Scales.Rescaling
{
  public interface ILogarithmicScaleRescaleConditionsView
  {
    SelectableListNodeList OrgRescaling { set; }

    SelectableListNodeList EndRescaling { set; }

    SelectableListNodeList OrgRelativeTo { set; }

    SelectableListNodeList EndRelativeTo { set; }

    double OrgValue { set; get; }

    double EndValue { set; get; }

    event Action OrgValueChanged;

    event Action EndValueChanged;
  }

  [ExpectedTypeOfView(typeof(ILogarithmicScaleRescaleConditionsView))]
  [UserControllerForObject(typeof(LogarithmicScaleRescaleConditions))]
  public class LogarithmicScaleRescaleConditionsController : MVCANControllerEditOriginalDocBase<LogarithmicScaleRescaleConditions, ILogarithmicScaleRescaleConditionsView>
  {
    private SelectableListNodeList _orgRescalingChoices;
    private SelectableListNodeList _endRescalingChoices;

    private SelectableListNodeList _orgRelativeToChoices;
    private SelectableListNodeList _endRelativeToChoices;

    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        _orgRescalingChoices = LinearScaleRescaleConditionsController.CreateListNodeList(_doc.OrgRescaling);
        _endRescalingChoices = LinearScaleRescaleConditionsController.CreateListNodeList(_doc.EndRescaling);

        _orgRelativeToChoices = new SelectableListNodeList(_doc.OrgRelativeTo);
        _endRelativeToChoices = new SelectableListNodeList(_doc.EndRelativeTo);
      }

      if (null != _view)
      {
        _view.OrgRescaling = _orgRescalingChoices;
        _view.EndRescaling = _endRescalingChoices;
        _view.OrgRelativeTo = _orgRelativeToChoices;
        _view.EndRelativeTo = _endRelativeToChoices;
        _view.OrgValue = GetOrgValueToShow();
        _view.EndValue = GetEndValueToShow();
      }
    }

    public override bool Apply(bool disposeController)
    {
      var orgRescaling = (BoundaryRescaling)_orgRescalingChoices.FirstSelectedNode.Tag;
      var endRescaling = (BoundaryRescaling)_endRescalingChoices.FirstSelectedNode.Tag;

      var orgRelativeTo = (BoundariesRelativeTo)_orgRelativeToChoices.FirstSelectedNode.Tag;
      var endRelativeTo = (BoundariesRelativeTo)_endRelativeToChoices.FirstSelectedNode.Tag;

      var orgValue = _view.OrgValue;
      var endValue = _view.EndValue;

      _doc.SetUserParameters(orgRescaling, orgRelativeTo, orgValue, endRescaling, endRelativeTo, endValue);

      return ApplyEnd(true, disposeController);
    }

    protected override void AttachView()
    {
      base.AttachView();

      _view.OrgValueChanged += EhOrgValueChanged;
      _view.EndValueChanged += EhEndValueChanged;
    }

    protected override void DetachView()
    {
      _view.OrgValueChanged -= EhOrgValueChanged;
      _view.EndValueChanged -= EhEndValueChanged;
      base.DetachView();
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

    private void EhOrgValueChanged()
    {
      var orgRescaling = (BoundaryRescaling)_orgRescalingChoices.FirstSelectedNode.Tag;
      if (orgRescaling == BoundaryRescaling.Auto)
      {
        _orgRescalingChoices.ClearSelectionsAll();

        foreach (var node in _orgRescalingChoices)
        {
          if (BoundaryRescaling.AutoTempFixed == (BoundaryRescaling)node.Tag)
          {
            node.IsSelected = true;
            break;
          }
        }

        if (null != _view)
          _view.OrgRescaling = _orgRescalingChoices;
      }
    }

    private void EhEndValueChanged()
    {
      var endRescaling = (BoundaryRescaling)_endRescalingChoices.FirstSelectedNode.Tag;
      if (endRescaling == BoundaryRescaling.Auto)
      {
        _endRescalingChoices.ClearSelectionsAll();

        foreach (var node in _endRescalingChoices)
        {
          if (BoundaryRescaling.AutoTempFixed == (BoundaryRescaling)node.Tag)
          {
            node.IsSelected = true;
            break;
          }
        }

        if (null != _view)
          _view.EndRescaling = _endRescalingChoices;
      }
    }
  }
}
