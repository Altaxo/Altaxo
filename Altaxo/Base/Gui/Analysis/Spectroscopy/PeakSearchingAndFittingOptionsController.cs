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
using Altaxo.Science.Spectroscopy.PeakFitting;
using Altaxo.Science.Spectroscopy.PeakSearching;

namespace Altaxo.Gui.Analysis.Spectroscopy
{
  [UserControllerForObject(typeof(PeakSearchingAndFittingOptions))]
  [ExpectedTypeOfView(typeof(ISpectralPreprocessingOptionsView))]
  public class PeakSearchingAndFittingOptionsController : SpectralPreprocessingControllerBase<PeakSearchingAndFittingOptions>
  {
    

    protected override IEnumerable<(string Label, object Doc, Func<IMVCANController> GetController)> GetComponents()
    {
      foreach (var pair in SpectralPreprocessingController.GetComponents(_doc.Preprocessing))
        yield return pair;

      yield return ("PeakSearching", _doc.PeakSearching, () => new PeakSearching.PeakSearchingController());
      yield return ("PeakFitting", _doc.PeakFitting, () => new PeakFitting.PeakFittingController());
    }

    protected override void UpdateDoc(object model)
    {
      var pre = SpectralPreprocessingController.UpdateDoc(_doc.Preprocessing, model);
      _doc = _doc with {  Preprocessing = pre };

      switch (model)
      {

        case IPeakSearching ps:
          _doc = _doc with { PeakSearching = ps };
          break;
        case IPeakFitting pf:
          _doc = _doc with { PeakFitting = pf };
          break;
      }
    }
  }
}
