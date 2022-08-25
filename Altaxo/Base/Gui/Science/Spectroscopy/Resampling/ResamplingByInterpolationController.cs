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
using Altaxo.Calc.Interpolation;
using Altaxo.Calc.LinearAlgebra;
using Altaxo.Gui.Calc.Interpolation;
using Altaxo.Science.Spectroscopy.Resampling;

namespace Altaxo.Gui.Science.Spectroscopy.Resampling
{
  public interface IResamplingByInterpolationView : IDataContextAwareView { }

  [ExpectedTypeOfView(typeof(IResamplingByInterpolationView))]
  [UserControllerForObject(typeof(ResamplingByInterpolation))]
  public class ResamplingByInterpolationController : MVCANControllerEditImmutableDocBase<ResamplingByInterpolation, IResamplingByInterpolationView>
  {
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield return new ControllerAndSetNullMethod(_interpolation, () => Interpolation = null);
      yield return new ControllerAndSetNullMethod(_samplingPoints, () => SamplingPoints = null);
    }

    #region Bindings

    private InterpolationFunctionOptionsController _interpolation;

    public InterpolationFunctionOptionsController Interpolation
    {
      get => _interpolation;
      set
      {
        if (!(_interpolation == value))
        {
          _interpolation?.Dispose();
          _interpolation = value;
          OnPropertyChanged(nameof(Interpolation));
        }
      }
    }

    private Altaxo.Gui.Common.EquallySpacedIntervalController _samplingPoints;

    public Altaxo.Gui.Common.EquallySpacedIntervalController SamplingPoints
    {
      get => _samplingPoints;
      set
      {
        if (!(_samplingPoints == value))
        {
          _samplingPoints?.Dispose();
          _samplingPoints = value;
          OnPropertyChanged(nameof(SamplingPoints));
        }
      }
    }


    #endregion

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        {
          var ctrl = new InterpolationFunctionOptionsController(_doc.Interpolation);
          Current.Gui.FindAndAttachControlTo(ctrl);
          Interpolation = ctrl;
        }


        {
          var ctrl = new Altaxo.Gui.Common.EquallySpacedIntervalController(_doc.SamplingPoints);
          Current.Gui.FindAndAttachControlTo(ctrl);
          SamplingPoints = ctrl;
        }
      }
    }

    public override bool Apply(bool disposeController)
    {
      if (Interpolation.Apply(disposeController))
      {
        _doc = _doc with { Interpolation = (IInterpolationFunctionOptions)Interpolation.ModelObject };
      }
      else
      {
        return ApplyEnd(false, disposeController);
      }

      if (SamplingPoints.Apply(disposeController))
      {
        _doc = _doc with { SamplingPoints = (ISpacedInterval)SamplingPoints.ModelObject };
      }
      else
      {
        return ApplyEnd(false, disposeController);
      }

      return ApplyEnd(true, disposeController);
    }
  }
}
