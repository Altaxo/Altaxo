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

#nullable disable
using System.Collections.Generic;
using Altaxo.Graph.Gdi.Plot;
using Altaxo.Graph.Gdi.Plot.Styles;

namespace Altaxo.Gui.Graph.Gdi.Plot.Styles
{
  using Altaxo.Graph.Scales;
  using ColorProvider;
  using Scales;


  public interface IDensityImagePlotStyleView : IDataContextAwareView
  {
  }


  /// <summary>
  /// Controller for the density image plot style
  /// </summary>
  [UserControllerForObject(typeof(DensityImagePlotStyle))]
  [ExpectedTypeOfView(typeof(IDensityImagePlotStyleView))]
  public class DensityImagePlotStyleController : MVCANControllerEditOriginalDocBase<DensityImagePlotStyle, IDensityImagePlotStyleView>
  {

    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield return new ControllerAndSetNullMethod(_scaleController, () => _scaleController = null);
      yield return new ControllerAndSetNullMethod(_colorProviderController, () => _colorProviderController = null);
    }

    #region Bindings

    private DensityScaleController _scaleController;

    public DensityScaleController ScaleController
    {
      get => _scaleController;
      set
      {
        if (!(_scaleController == value))
        {
          _scaleController?.Dispose();
          _scaleController = value;
          OnPropertyChanged(nameof(ScaleController));
        }
      }
    }

    private IMVCANController _colorProviderController;

    public IMVCANController ColorProviderController
    {
      get => _colorProviderController;
      set
      {
        if (!(_colorProviderController == value))
        {
          _colorProviderController = value;
          OnPropertyChanged(nameof(ColorProviderController));
        }
      }
    }

    private bool _clipToLayer;

    public bool ClipToLayer
    {
      get => _clipToLayer;
      set
      {
        if (!(_clipToLayer == value))
        {
          _clipToLayer = value;
          OnPropertyChanged(nameof(ClipToLayer));
        }
      }
    }



    #endregion
    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        ScaleController = new DensityScaleController(newScale => _doc.Scale = (NumericalScale)newScale) { UseDocumentCopy = UseDocument.Directly };
        ScaleController.InitializeDocument(_doc.Scale);

        ColorProviderController = new ColorProviderController(newColorProvider => _doc.ColorProvider = newColorProvider) { UseDocumentCopy = UseDocument.Directly };
        ColorProviderController.InitializeDocument(_doc.ColorProvider);
        ClipToLayer = _doc.ClipToLayer;
      }
    }

    public override bool Apply(bool disposeController)
    {
      if (!_scaleController.Apply(disposeController))
        return false;

      if (!_colorProviderController.Apply(disposeController))
        return false;

      _doc.ClipToLayer = ClipToLayer;
      _doc.Scale = (NumericalScale)_scaleController.ModelObject;
      _doc.ColorProvider = (IColorProvider)_colorProviderController.ModelObject;

      return ApplyEnd(true, disposeController);
    }
  }
}
