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

using System;
using System.Collections.Generic;
using Altaxo.Collections;
using Altaxo.Science.Spectroscopy;
using Altaxo.Science.Spectroscopy.BaselineEstimation;
using Altaxo.Science.Spectroscopy.Cropping;
using Altaxo.Science.Spectroscopy.Normalization;
using Altaxo.Science.Spectroscopy.Smoothing;
using Altaxo.Science.Spectroscopy.SpikeRemoval;

namespace Altaxo.Gui.Analysis.Spectroscopy
{

  [UserControllerForObject(typeof(SpectralPreprocessingOptions))]
  [ExpectedTypeOfView(typeof(ISpectralPreprocessingOptionsView))]
  public class SpectralPreprocessingController : SpectralPreprocessingControllerBase<SpectralPreprocessingOptions>
  {
    protected override IEnumerable<(string Label, object Doc, Func<IMVCANController> GetController)> GetComponents()
    {
      return GetComponents(_doc);
    }

    public static IEnumerable<(string Label, object Doc, Func<IMVCANController> GetController)> GetComponents(SpectralPreprocessingOptions _doc)
    {
      yield return ("Spike removal", _doc.SpikeRemoval, () => new SpikeRemoval.SpikeRemovalController());
      yield return ("Smoothing", _doc.Smoothing, () => new Smoothing.SmoothingController());
      yield return ("Baseline", _doc.BaselineEstimation, () => new BaselineEstimation.BaselineEstimationController());
      yield return ("Cropping", _doc.Cropping, () => new Cropping.CroppingController());
      yield return ("Normalization", _doc.Normalization, () => new Normalization.NormalizationController());
    }

    protected override void UpdateDoc(object model)
    {
      _doc = UpdateDoc(_doc, model);
    }

    public static SpectralPreprocessingOptions UpdateDoc(SpectralPreprocessingOptions _doc, object model)
    {
      switch (model)
      {
        case null:
          break;
        case ISpikeRemoval sr:
          _doc = _doc with { SpikeRemoval = sr };
          break;
        case INormalization no:
          _doc = _doc with { Normalization = no };
          break;
        case ICropping cr:
          _doc = _doc with { Cropping = cr };
          break;
        case IBaselineEstimation be:
          _doc = _doc with { BaselineEstimation = be };
          break;
        case ISmoothing sm:
          _doc = _doc with { Smoothing = sm };
          break;
      }

      return _doc;
    }

  }
}
