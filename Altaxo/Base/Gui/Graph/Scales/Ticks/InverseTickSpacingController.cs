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
using System.ComponentModel;
using System.Text;
using Altaxo.Calc;
using Altaxo.Collections;
using Altaxo.Data;
using Altaxo.Graph.Scales.Ticks;
using Altaxo.Serialization;

namespace Altaxo.Gui.Graph.Scales.Ticks
{
  [UserControllerForObject(typeof(InverseTickSpacing), 200)]
  [ExpectedTypeOfView(typeof(ILinearTickSpacingView))]
  public class InverseTickSpacingController : MVCANControllerEditOriginalDocBase<InverseTickSpacing, ILinearTickSpacingView>
  {
    private SelectableListNodeList _snapTicksToOrg = new SelectableListNodeList();
    private SelectableListNodeList _snapTicksToEnd = new SelectableListNodeList();

    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    public override void Dispose(bool isDisposing)
    {
      _snapTicksToOrg = null;
      _snapTicksToEnd = null;
      base.Dispose(isDisposing);
    }

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (_view != null)
      {
        _view.MajorTicks = GUIConversion.ToString(_doc.MajorTickSpan);
        _view.MinorTicks = _doc.MinorTicks;
        _view.MinGrace = _doc.OrgGrace;
        _view.MaxGrace = _doc.EndGrace;

        _snapTicksToOrg.Clear();
        _snapTicksToEnd.Clear();
        foreach (BoundaryTickSnapping s in Enum.GetValues(typeof(BoundaryTickSnapping)))
        {
          _snapTicksToOrg.Add(new SelectableListNode(Current.Gui.GetUserFriendlyName(s), s, s == _doc.SnapOrgToTick));
          _snapTicksToEnd.Add(new SelectableListNode(Current.Gui.GetUserFriendlyName(s), s, s == _doc.SnapEndToTick));
        }
        _view.SnapTicksToOrg = _snapTicksToOrg;
        _view.SnapTicksToEnd = _snapTicksToEnd;

        _view.TargetNumberMajorTicks = _doc.TargetNumberOfMajorTicks;
        _view.TargetNumberMinorTicks = _doc.TargetNumberOfMinorTicks;

        _view.TransfoOffset = GUIConversion.ToString(_doc.TransformationOffset);
        _view.DivideBy = GUIConversion.ToString(_doc.TransformationDivider);
        _view.TransfoOperationIsMultiply = _doc.TransformationOperationIsMultiply;

        _view.SuppressMajorTickValues = GUIConversion.ToString(_doc.SuppressedMajorTicks.ByValues);
        _view.SuppressMinorTickValues = GUIConversion.ToString(_doc.SuppressedMinorTicks.ByValues);
        _view.SuppressMajorTicksByNumber = GUIConversion.ToString(_doc.SuppressedMajorTicks.ByNumbers);
        _view.SuppressMinorTicksByNumber = GUIConversion.ToString(_doc.SuppressedMinorTicks.ByNumbers);

        _view.AddMajorTickValues = GUIConversion.ToString(_doc.AdditionalMajorTicks.Values);
        _view.AddMinorTickValues = GUIConversion.ToString(_doc.AdditionalMinorTicks.Values);
      }
    }

    public override bool Apply(bool disposeController)
    {

      if (GUIConversion.TryParseMultipleAltaxoVariant(_view.SuppressMajorTickValues, out var varVals))
      {
        _doc.SuppressedMajorTicks.ByValues.Clear();
        foreach (AltaxoVariant v in varVals)
          _doc.SuppressedMajorTicks.ByValues.Add(v);
      }
      else
      {
        return false;
      }

      if (GUIConversion.TryParseMultipleAltaxoVariant(_view.SuppressMinorTickValues, out varVals))
      {
        _doc.SuppressedMinorTicks.ByValues.Clear();
        foreach (AltaxoVariant v in varVals)
          _doc.SuppressedMinorTicks.ByValues.Add(v);
      }
      else
      {
        return false;
      }

      if (GUIConversion.TryParseMultipleInt32(_view.SuppressMajorTicksByNumber, out var intVals))
      {
        _doc.SuppressedMajorTicks.ByNumbers.Clear();
        foreach (int v in intVals)
          _doc.SuppressedMajorTicks.ByNumbers.Add(v);
      }
      else
      {
        return false;
      }

      if (GUIConversion.TryParseMultipleInt32(_view.SuppressMinorTicksByNumber, out intVals))
      {
        _doc.SuppressedMinorTicks.ByNumbers.Clear();
        foreach (int v in intVals)
          _doc.SuppressedMinorTicks.ByNumbers.Add(v);
      }
      else
      {
        return false;
      }

      if (GUIConversion.TryParseMultipleAltaxoVariant(_view.AddMajorTickValues, out varVals))
      {
        _doc.AdditionalMajorTicks.Clear();
        foreach (AltaxoVariant v in varVals)
          _doc.AdditionalMajorTicks.Add(v);
      }
      else
      {
        return false;
      }

      if (GUIConversion.TryParseMultipleAltaxoVariant(_view.AddMinorTickValues, out varVals))
      {
        _doc.AdditionalMinorTicks.Clear();
        foreach (AltaxoVariant v in varVals)
          _doc.AdditionalMinorTicks.Add(v);
      }
      else
      {
        return false;
      }

      // MajorTicks were validated and set before
      _doc.MinorTicks = _view.MinorTicks;

      _doc.TargetNumberOfMajorTicks = _view.TargetNumberMajorTicks;
      _doc.TargetNumberOfMinorTicks = _view.TargetNumberMinorTicks;

      _doc.OrgGrace = _view.MinGrace;
      _doc.EndGrace = _view.MaxGrace;

      _doc.SnapOrgToTick = (BoundaryTickSnapping)_snapTicksToOrg.FirstSelectedNode.Tag;
      _doc.SnapEndToTick = (BoundaryTickSnapping)_snapTicksToEnd.FirstSelectedNode.Tag;

      return ApplyEnd(true, disposeController);
    }

    protected override void AttachView()
    {
      base.AttachView();

      _view.MajorTicksValidating += EhMajorSpanValidating;
      _view.DivideByValidating += EhDivideByValidating;
      _view.TransfoOffsetValidating += EhTransformationOffsetValidating;
      _view.TransfoOperationChanged += EhTransformationOperationChanged;
    }

    protected override void DetachView()
    {
      _view.MajorTicksValidating -= EhMajorSpanValidating;
      _view.DivideByValidating -= EhDivideByValidating;
      _view.TransfoOffsetValidating -= EhTransformationOffsetValidating;
      _view.TransfoOperationChanged -= EhTransformationOperationChanged;

      base.DetachView();
    }

    private void EhMajorSpanValidating(string txt, CancelEventArgs e)
    {
      if (GUIConversion.IsDoubleOrNull(txt, out var val))
        _doc.MajorTickSpan = val;
      else
        e.Cancel = true;
    }

    private void EhDivideByValidating(string txt, CancelEventArgs e)
    {
      if (GUIConversion.IsDouble(txt, out var val))
      {
        double val1 = val;
        if (val == 0 || !val1.IsFinite())
          e.Cancel = true;
        else
          _doc.TransformationDivider = val1;
      }
      else
      {
        e.Cancel = true;
      }
    }

    private void EhTransformationOffsetValidating(string txt, CancelEventArgs e)
    {
      if (GUIConversion.IsDouble(txt, out var val))
      {
        if (!val.IsFinite())
          e.Cancel = true;
        else
          _doc.TransformationOffset = val;
      }
      else
      {
        e.Cancel = true;
      }
    }

    private void EhTransformationOperationChanged(bool transfoOpIsMultiply)
    {
      _doc.TransformationOperationIsMultiply = transfoOpIsMultiply;
    }
  }
}
