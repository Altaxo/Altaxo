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

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Altaxo.Collections;
using Altaxo.Science.Spectroscopy;

namespace Altaxo.Gui.Science.Spectroscopy
{
  public interface IPeakSearchingAndFittingOutputOptionsView : IDataContextAwareView
  {
  }

  [ExpectedTypeOfView(typeof(IPeakSearchingAndFittingOutputOptionsView))]
  [UserControllerForObject(typeof(PeakSearchingAndFittingOutputOptions))]
  public class PeakSearchingAndFittingOutputOptionsController : MVCANControllerEditImmutableDocBase<PeakSearchingAndFittingOutputOptions, IPeakSearchingAndFittingOutputOptionsView>
  {
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    #region Bindings

    private bool _outputPreprocessedCurve;

    public bool OutputPreprocessedCurve
    {
      get => _outputPreprocessedCurve;
      set
      {
        if (!(_outputPreprocessedCurve == value))
        {
          _outputPreprocessedCurve = value;
          OnPropertyChanged(nameof(OutputPreprocessedCurve));
        }
      }
    }

    private bool _outputFitCurve;

    public bool OutputFitCurve
    {
      get => _outputFitCurve;
      set
      {
        if (!(_outputFitCurve == value))
        {
          _outputFitCurve = value;
          OnPropertyChanged(nameof(OutputFitCurve));
        }
      }
    }

    private bool _outputFitCurveAsSeparatePeaks;

    public bool OutputFitCurveAsSeparatePeaks
    {
      get => _outputFitCurveAsSeparatePeaks;
      set
      {
        if (!(_outputFitCurveAsSeparatePeaks == value))
        {
          _outputFitCurveAsSeparatePeaks = value;
          OnPropertyChanged(nameof(OutputFitCurveAsSeparatePeaks));
        }
      }
    }

    private int _outputFitCurveSamplingFactor;

    public int OutputFitCurveSamplingFactor
    {
      get => _outputFitCurveSamplingFactor;
      set
      {
        if (!(_outputFitCurveSamplingFactor == value))
        {
          _outputFitCurveSamplingFactor = value;
          OnPropertyChanged(nameof(OutputFitCurveSamplingFactor));
        }
      }
    }

    private int _outputFitCurveAsSeparatePeaksSamplingFactor;

    public int OutputFitCurveAsSeparatePeaksSamplingFactor
    {
      get => _outputFitCurveAsSeparatePeaksSamplingFactor;
      set
      {
        if (!(_outputFitCurveAsSeparatePeaksSamplingFactor == value))
        {
          _outputFitCurveAsSeparatePeaksSamplingFactor = value;
          OnPropertyChanged(nameof(OutputFitCurveAsSeparatePeaksSamplingFactor));
        }
      }
    }

    public class Property : IEditableObject, INotifyPropertyChanged
    {
      public event PropertyChangedEventHandler? PropertyChanged;

      public Property() { }
      public Property(string name)
      {
        _propertyName = name ?? string.Empty;
      }


      public void BeginEdit()
      {
      }

      public void CancelEdit()
      {
      }

      public void EndEdit()
      {
      }

      private string _propertyName = string.Empty;
      public string PropertyName
      {
        get => _propertyName;
        set
        {
          if (_propertyName != value)
          {
            _propertyName = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PropertyName)));
          }
        }
      }
    }

    public ObservableCollection<Property> PropertyNames { get; } = new();


    #endregion

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        OutputPreprocessedCurve = _doc.OutputPreprocessedCurve;
        OutputFitCurve = _doc.OutputFitCurve;
        OutputFitCurveSamplingFactor = _doc.OutputFitCurveSamplingFactor;
        OutputFitCurveAsSeparatePeaks = _doc.OutputFitCurveAsSeparatePeaks;
        OutputFitCurveAsSeparatePeaksSamplingFactor = _doc.OutputFitCurveAsSeparatePeaksSamplingFactor;
        PropertyNames.AddRange(_doc.PropertyNames.Select(x => new Property(x)));
      }
    }

    public override bool Apply(bool disposeController)
    {
      _doc = _doc with
      {
        OutputPreprocessedCurve = OutputPreprocessedCurve,
        OutputFitCurve = OutputFitCurve,
        OutputFitCurveSamplingFactor = _doc.OutputFitCurveSamplingFactor,
        OutputFitCurveAsSeparatePeaks = OutputFitCurveAsSeparatePeaks,
        OutputFitCurveAsSeparatePeaksSamplingFactor = OutputFitCurveAsSeparatePeaksSamplingFactor,
        PropertyNames = PropertyNames.Select(x => x.PropertyName).Distinct().ToArray(),
      };

      return ApplyEnd(true, disposeController);
    }

  }
}
