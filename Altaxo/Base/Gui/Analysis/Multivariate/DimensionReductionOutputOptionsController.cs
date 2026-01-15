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
using Altaxo.Calc.Regression.Multivariate;

namespace Altaxo.Gui.Analysis.Multivariate
{

  public interface IDimensionReductionOutputOptionsView : IDataContextAwareView
  {
  }

  /// <summary>
  /// Controller for <see cref="DimensionReductionAndRegressionOptions"/>
  /// </summary>
  [ExpectedTypeOfView(typeof(IDimensionReductionOutputOptionsView))]
  [UserControllerForObject(typeof(DimensionReductionOutputOptions))]
  public class DimensionReductionOutputOptionsController : MVCANControllerEditImmutableDocBase<DimensionReductionOutputOptions, IDimensionReductionOutputOptionsView>
  {

    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    #region Bindings


    public bool IncludeEnsemblePreprocessingAuxiliaryData
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field = value;
          OnPropertyChanged(nameof(IncludeEnsemblePreprocessingAuxiliaryData));
        }
      }
    }


    public bool IncludePreprocessedSpectra
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field = value;
          OnPropertyChanged(nameof(IncludePreprocessedSpectra));
        }
      }
    }



    #endregion

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        IncludeEnsemblePreprocessingAuxiliaryData = _doc.IncludeEnsemblePreprocessingAuxiliaryData;
        IncludePreprocessedSpectra = _doc.IncludePreprocessedSpectra;
      }
    }



    public override bool Apply(bool disposeController)
    {



      _doc = _doc with
      {
        IncludeEnsemblePreprocessingAuxiliaryData = IncludeEnsemblePreprocessingAuxiliaryData,
        IncludePreprocessedSpectra = IncludePreprocessedSpectra,
      };


      return ApplyEnd(true, disposeController);
    }
  }
}

