#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2023 Dr. Dirk Lellinger
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
using Altaxo.Science.Signals;

namespace Altaxo.Gui.Science.Signals
{
  public interface IPronySeriesRelaxationView : IDataContextAwareView
  {
  }

  [UserControllerForObject(typeof(PronySeriesRelaxation))]
  [ExpectedTypeOfView(typeof(IPronySeriesRelaxationView))]
  public class PronySeriesRelaxationController : MVCANControllerEditImmutableDocBase<PronySeriesRelaxation, IPronySeriesRelaxationView>
  {
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    #region Bindings

    private double _timeMinimum;

    public double TimeMinimum
    {
      get => _timeMinimum;
      set
      {
        if (!(_timeMinimum == value))
        {
          _timeMinimum = value;
          OnPropertyChanged(nameof(TimeMinimum));
        }
      }
    }

    private double _timeMaximum;

    public double TimeMaximum
    {
      get => _timeMaximum;
      set
      {
        if (!(_timeMaximum == value))
        {
          _timeMaximum = value;
          OnPropertyChanged(nameof(TimeMaximum));
        }
      }
    }


    private int _numberOfRelaxationTimes;

    public int NumberOfRelaxationTimes
    {
      get => _numberOfRelaxationTimes;
      set
      {
        if (!(_numberOfRelaxationTimes == value))
        {
          _numberOfRelaxationTimes = value;
          OnPropertyChanged(nameof(NumberOfRelaxationTimes));
        }
      }
    }

    private double _regularizationParameter;

    public double RegularizationParameter
    {
      get => _regularizationParameter;
      set
      {
        if (!(_regularizationParameter == value))
        {
          _regularizationParameter = value;
          OnPropertyChanged(nameof(RegularizationParameter));
        }
      }
    }

    private bool _useIntercept;

    public bool UseIntercept
    {
      get => _useIntercept;
      set
      {
        if (!(_useIntercept == value))
        {
          _useIntercept = value;
          OnPropertyChanged(nameof(UseIntercept));
        }
      }
    }



    #endregion

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        TimeMinimum = _doc.MinimalRelaxationTime;
        TimeMaximum = _doc.MaximalRelaxationTime;
        NumberOfRelaxationTimes = _doc.NumberOfRelaxationTimes;
        RegularizationParameter = _doc.RegularizationParameter;
        UseIntercept = _doc.UseIntercept;
      }
    }

    public override bool Apply(bool disposeController)
    {
      try
      {
        _doc = _doc with
        {
          MinimalRelaxationTime = TimeMinimum,
          MaximalRelaxationTime = TimeMaximum,
          NumberOfRelaxationTimes = NumberOfRelaxationTimes,
          RegularizationParameter = RegularizationParameter,
          UseIntercept = UseIntercept,
        };
      }
      catch (Exception ex)
      {
        Current.Gui.ErrorMessageBox($"The changes could not be applied: {ex.Message}", "Apply failed");
        return ApplyEnd(false, disposeController);
      }

      return ApplyEnd(true, disposeController);
    }


  }
}
