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
    protected override void AddControllers(SelectableListNodeList controllers)
    {
      base.AddControllers(controllers);

      {
        var controller = new PeakSearching.PeakSearchingController();
        controller.InitializeDocument(_doc.PeakSearching);
        Current.Gui.FindAndAttachControlTo(controller);
        controllers.Add(new SelectableListNodeWithController("PeakSearching", controller, false)
        { Controller = controller });
      }

      {
        var controller = new PeakFitting.PeakFittingController();
        controller.InitializeDocument(_doc.PeakFitting);
        Current.Gui.FindAndAttachControlTo(controller);
        controllers.Add(new SelectableListNodeWithController("PeakFitting", controller, false)
        { Controller = controller });
      }
    }

    protected override void UpdateDoc(object model)
    {
      base.UpdateDoc(model);

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
