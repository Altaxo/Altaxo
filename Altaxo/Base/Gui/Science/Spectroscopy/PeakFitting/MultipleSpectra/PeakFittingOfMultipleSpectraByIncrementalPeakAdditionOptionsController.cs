#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2024 Dr. Dirk Lellinger
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
using Altaxo.Gui.Science.Spectroscopy.PeakFitting.MultipleSpectra;
using Altaxo.Science.Spectroscopy;
using Altaxo.Science.Spectroscopy.PeakFitting.MultipleSpectra;

namespace Altaxo.Gui.Science.Spectroscopy.MultipleSpectra
{
  [UserControllerForObject(typeof(PeakFittingOfMultipleSpectraByIncrementalPeakAdditionOptions))]
  [ExpectedTypeOfView(typeof(ISpectralPreprocessingOptionsView))]
  public class PeakFittingOfMultipleSpectraByIncrementalPeakAdditionOptionsController : SpectralPreprocessingControllerBase<PeakFittingOfMultipleSpectraByIncrementalPeakAdditionOptions>
  {


    protected override IEnumerable<(string Label, object Doc, Func<IMVCANController> GetController)> GetComponents()
    {
      foreach (var pair in SpectralPreprocessingController.GetComponents(_doc.Preprocessing))
        yield return pair;

      yield return ("PeakFitting", _doc.PeakFitting, () => new PeakFittingOfMultipleSpectraByIncrementalPeakAdditionController());
      yield return ("Output", _doc.OutputOptions, () => new PeakSearchingAndFittingOutputOptionsController());
    }

    protected override void UpdateDoc(object model, int index)
    {
      var pre = SpectralPreprocessingController.UpdateDoc(_doc.Preprocessing, model, index);
      _doc = _doc with { Preprocessing = pre };

      switch (model)
      {
        case PeakFittingOfMultipleSpectraByIncrementalPeakAddition pf:
          _doc = _doc with { PeakFitting = pf };
          break;
        case PeakSearchingAndFittingOutputOptions oo:
          _doc = _doc with { OutputOptions = oo };
          break;
      }
    }

    protected override bool ApplyEnd(bool applyResult, bool disposeController)
    {
      // clean the SpectralPreprocessingOptionsList by dropping non-elements
      if (applyResult == true)
      {
        if (_doc.Preprocessing is SpectralPreprocessingOptionsList doclist)
        {
          _doc = _doc with { Preprocessing = SpectralPreprocessingOptionsList.CreateWithoutNoneElements(doclist) };
        }
      }

      return base.ApplyEnd(applyResult, disposeController);
    }

    protected override SpectralPreprocessingOptionsBase InternalPreprocessingOptions
    {
      get => _doc.Preprocessing;
      set => _doc = _doc with { Preprocessing = value };
    }
  }
}
