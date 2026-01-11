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
using Altaxo.Science.Spectroscopy.EnsembleProcessing;

namespace Altaxo.Gui.Science.Spectroscopy.EnsembleProcessing
{
  // MultiplicativeScatterCorrection

  public interface IEnsembleMeanAndScaleView : IDataContextAwareView
  {
  }

  [UserControllerForObject(typeof(EnsembleMeanAndScaleCorrection))]
  [ExpectedTypeOfView(typeof(IEnsembleMeanAndScaleView))]
  public class EnsembleMeanAndScaleController : MVCANControllerEditImmutableDocBase<EnsembleMeanAndScaleCorrection, IEnsembleMeanAndScaleView>
  {
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    #region Bindings

    public bool EnsembleMean
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field = value;
          OnPropertyChanged(nameof(EnsembleMean));
        }
      }
    }

    public bool EnsembleScale
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field = value;
          OnPropertyChanged(nameof(EnsembleScale));
        }
      }
    }
    #endregion

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        EnsembleScale = _doc.EnsembleScale;
      }
    }

    public override bool Apply(bool disposeController)
    {
      _doc = _doc with { EnsembleScale = EnsembleScale };
      return ApplyEnd(true, disposeController);
    }
  }
}
