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

using System.Collections.Generic;
using Altaxo.Science.Spectroscopy.PeakEnhancement;

namespace Altaxo.Gui.Science.Spectroscopy.PeakEnhancement
{
  public interface IPeakEnhancement2ndDerivativeOfSmoothingSplineView : IDataContextAwareView
  {
  }

  [UserControllerForObject(typeof(PeakEnhancement2ndDerivativeOfSmoothingSpline), 100)]
  [ExpectedTypeOfView(typeof(IPeakEnhancement2ndDerivativeOfSmoothingSplineView))]
  public class PeakEnhancement2ndDerivativeOfSmoothingSplineController : MVCANControllerEditImmutableDocBase<PeakEnhancement2ndDerivativeOfSmoothingSpline, IPeakEnhancement2ndDerivativeOfSmoothingSplineView>
  {
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    #region Bindings

    private bool _isSmoothnessManual;

    public bool IsSmoothnessManual
    {
      get => _isSmoothnessManual;
      set
      {
        if (!(_isSmoothnessManual == value))
        {
          _isSmoothnessManual = value;
          OnPropertyChanged(nameof(IsSmoothnessManual));
        }
      }
    }


    private double _smoothness;

    public double Smoothness
    {
      get => _smoothness;
      set
      {
        if (!(_smoothness == value))
        {
          _smoothness = value;
          OnPropertyChanged(nameof(Smoothness));
        }
      }
    }


    #endregion

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        IsSmoothnessManual = _doc.Smoothness is not null;
        Smoothness = _doc.Smoothness ?? PeakEnhancement2ndDerivativeOfSmoothingSpline.SmoothnessDefaultValue;
      }
    }

    public override bool Apply(bool disposeController)
    {
      _doc = _doc with
      {
        Smoothness = IsSmoothnessManual ? Smoothness : null,
      };

      return ApplyEnd(true, disposeController);
    }
  }
}

