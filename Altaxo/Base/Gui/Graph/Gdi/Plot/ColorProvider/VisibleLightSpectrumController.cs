#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2012 Dr. Dirk Lellinger
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

#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Altaxo.Graph.Gdi.Plot.ColorProvider;

namespace Altaxo.Gui.Graph.Gdi.Plot.ColorProvider
{
  public interface IVisibleLightSpectrumView
  {
    IColorProviderBaseView BaseView { get; }

    double Gamma { get; set; }

    double Brightness { get; set; }

    event Action ChoiceChanged;
  }

  [ExpectedTypeOfView(typeof(IVisibleLightSpectrumView))]
  [UserControllerForObject(typeof(VisibleLightSpectrum), 110)]
  public class VisibleLightSpectrumController : MVCANDControllerEditImmutableDocBase<VisibleLightSpectrum, IVisibleLightSpectrumView>
  {
    private ColorProviderBaseController _baseController;

    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield return new ControllerAndSetNullMethod(_baseController, () => _baseController = null);
    }

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        _baseController = new ColorProviderBaseController() { UseDocumentCopy = UseDocument.Directly };
        _baseController.InitializeDocument(_doc);
        _baseController.MadeDirty += EhBaseControllerChanged;
      }
      if (_view is not null)
      {
        _baseController.ViewObject = _view.BaseView;

        _view.Gamma = _doc.Gamma;
        _view.Brightness = _doc.Brightness;
      }
    }

    public override bool Apply(bool disposeController)
    {
      if (!_baseController.Apply(disposeController))
        return false;

      _doc = (VisibleLightSpectrum)_baseController.ModelObject;

      _doc = _doc
              .WithGamma(_view.Gamma)
              .WithBrightness(_view.Brightness);

      return ApplyEnd(true, disposeController);
    }

    protected override void AttachView()
    {
      base.AttachView();
      _view.ChoiceChanged += OnMadeDirty;
    }

    protected override void DetachView()
    {
      _view.ChoiceChanged -= OnMadeDirty;
      base.DetachView();
    }

    private void EhBaseControllerChanged(IMVCANDController ctrl)
    {
      OnMadeDirty();
    }
  }
}
