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
  /// <summary>
  /// View interface for editing <see cref="MasterCurveTableOutputOptions"/>.
  /// </summary>
  public interface IMasterCurveTableOutputOptionsView : IDataContextAwareView { }

  /// <summary>
  /// Controller for editing <see cref="MasterCurveTableOutputOptions"/>.
  /// </summary>
  [ExpectedTypeOfView(typeof(IMasterCurveTableOutputOptionsView))]
  [UserControllerForObject(typeof(MasterCurveTableOutputOptions))]
  public class MasterCurveTableOutputOptionsController : MVCANControllerEditImmutableDocBase<MasterCurveTableOutputOptions, IMasterCurveTableOutputOptionsView>
  {
    /// <inheritdoc/>
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }


    #region Bindings

    private bool _outputOriginalCurves;

    /// <summary>
    /// Gets or sets a value indicating whether original curves are output.
    /// </summary>
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

    /// <summary>
    /// Gets or sets a value indicating whether shifted curves are output.
    /// </summary>
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

    /// <summary>
    /// Gets or sets a value indicating whether the merged shifted curve is output.
    /// </summary>
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

    /// <summary>
    /// Gets or sets a value indicating whether an interpolated curve is output.
    /// </summary>
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

    /// <summary>
    /// Gets or sets a value indicating whether activation energies are output.
    /// </summary>
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

    /// <summary>
    /// Gets or sets a value indicating whether x-values for activation energies are rates.
    /// </summary>
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

    /// <summary>
    /// Gets or sets the sorting applied to shift-group values.
    /// </summary>
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

    /// <inheritdoc/>
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


    /// <inheritdoc/>
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
