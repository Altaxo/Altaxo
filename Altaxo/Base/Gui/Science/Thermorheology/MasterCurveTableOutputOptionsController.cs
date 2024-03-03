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

using System.Collections.Generic;
using Altaxo.Gui.Common;
using Altaxo.Science.Thermorheology.MasterCurves;

namespace Altaxo.Gui.Science.Thermorheology
{
  public interface IMasterCurveTableOutputOptionsView : IDataContextAwareView { }

  [ExpectedTypeOfView(typeof(IMasterCurveTableOutputOptionsView))]
  [UserControllerForObject(typeof(MasterCurveTableOutputOptions))]
  public class MasterCurveTableOutputOptionsController : MVCANControllerEditImmutableDocBase<MasterCurveTableOutputOptions, IMasterCurveTableOutputOptionsView>
  {
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }


    #region Bindings

    private bool _outputOriginalCurves;

    public bool OutputOriginalCurves
    {
      get => _outputOriginalCurves;
      set
      {
        if (!(_outputOriginalCurves == value))
        {
          _outputOriginalCurves = value;
          OnPropertyChanged(nameof(OutputOriginalCurves));
        }
      }
    }

    private bool _outputShiftedCurves;

    public bool OutputShiftedCurves
    {
      get => _outputShiftedCurves;
      set
      {
        if (!(_outputShiftedCurves == value))
        {
          _outputShiftedCurves = value;
          OnPropertyChanged(nameof(OutputShiftedCurves));
        }
      }
    }

    private bool _outputMergedShiftedCurve;

    public bool OutputMergedShiftedCurve
    {
      get => _outputMergedShiftedCurve;
      set
      {
        if (!(_outputMergedShiftedCurve == value))
        {
          _outputMergedShiftedCurve = value;
          OnPropertyChanged(nameof(OutputMergedShiftedCurve));
        }
      }
    }

    private bool _outputInterpolatedCurve;

    public bool OutputInterpolatedCurve
    {
      get => _outputInterpolatedCurve;
      set
      {
        if (!(_outputInterpolatedCurve == value))
        {
          _outputInterpolatedCurve = value;
          OnPropertyChanged(nameof(OutputInterpolatedCurve));
        }
      }
    }

    private bool _outputActivationEnergies;

    public bool OutputActivationEnergies
    {
      get => _outputActivationEnergies;
      set
      {
        if (!(_outputActivationEnergies == value))
        {
          _outputActivationEnergies = value;
          OnPropertyChanged(nameof(OutputActivationEnergies));
        }
      }
    }

    private bool _xValuesForActivationEnergiesAreRates;

    public bool XValuesForActivationEnergiesAreRates
    {
      get => _xValuesForActivationEnergiesAreRates;
      set
      {
        if (!(_xValuesForActivationEnergiesAreRates == value))
        {
          _xValuesForActivationEnergiesAreRates = value;
          OnPropertyChanged(nameof(XValuesForActivationEnergiesAreRates));
        }
      }
    }

    private ItemsController<ShiftGroupSorting> _sortShiftGroupValuesBy;

    public ItemsController<ShiftGroupSorting> SortShiftGroupValuesBy
    {
      get => _sortShiftGroupValuesBy;
      set
      {
        if (!(_sortShiftGroupValuesBy == value))
        {
          _sortShiftGroupValuesBy = value;
          OnPropertyChanged(nameof(SortShiftGroupValuesBy));
        }
      }
    }


    #endregion

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        OutputOriginalCurves = _doc.OutputOriginalCurves;
        OutputShiftedCurves = _doc.OutputShiftedCurves;
        OutputMergedShiftedCurve = _doc.OutputMergedShiftedCurve;
        OutputInterpolatedCurve = _doc.OutputInterpolatedCurve;
        OutputActivationEnergies = _doc.OutputActivationEnergies;
        XValuesForActivationEnergiesAreRates = _doc.XValuesForActivationEnergiesAreRates;
        SortShiftGroupValuesBy = new ItemsController<ShiftGroupSorting>(new Collections.SelectableListNodeList(_doc.SortShiftGroupValuesBy));
      }
    }


    public override bool Apply(bool disposeController)
    {
      _doc = _doc with
      {
        OutputOriginalCurves = OutputOriginalCurves,
        OutputShiftedCurves = OutputShiftedCurves,
        OutputMergedShiftedCurve = OutputMergedShiftedCurve,
        OutputInterpolatedCurve = OutputInterpolatedCurve,
        OutputActivationEnergies = OutputActivationEnergies,
        XValuesForActivationEnergiesAreRates = XValuesForActivationEnergiesAreRates,
        SortShiftGroupValuesBy = SortShiftGroupValuesBy.SelectedValue,
      };

      return ApplyEnd(true, disposeController);
    }

  }
}
