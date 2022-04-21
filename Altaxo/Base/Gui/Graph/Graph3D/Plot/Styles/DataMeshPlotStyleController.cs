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
using Altaxo.Drawing.D3D;
using Altaxo.Graph.Gdi.Plot;
using Altaxo.Graph.Graph3D.Plot.Styles;
using Altaxo.Graph.Scales;
using Altaxo.Gui.Drawing.D3D;
using Altaxo.Gui.Graph.Gdi.Plot.ColorProvider;
using Altaxo.Gui.Graph.Scales;

namespace Altaxo.Gui.Graph.Graph3D.Plot.Styles
{
  public interface IDataMeshPlotStyleView : IDataContextAwareView
  {
  }

  /// <summary>
  /// Controller for the density image plot style
  /// </summary>
  [UserControllerForObject(typeof(DataMeshPlotStyle))]
  [ExpectedTypeOfView(typeof(IDataMeshPlotStyleView))]
  public class DataMeshPlotStyleController : MVCANControllerEditOriginalDocBase<DataMeshPlotStyle, IDataMeshPlotStyleView>
  {
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield return new ControllerAndSetNullMethod(_scaleController, () => ScaleController = null);
      yield return new ControllerAndSetNullMethod(_colorProviderController, () => ColorProviderController = null);
      yield return new ControllerAndSetNullMethod(_materialController, () => MaterialController = null);
    }

    #region Bindings


    private bool _isCustomColorScaleUsed;

    public bool IsCustomColorScaleUsed
    {
      get => _isCustomColorScaleUsed;
      set
      {
        if (!(_isCustomColorScaleUsed == value))
        {
          _isCustomColorScaleUsed = value;
          OnPropertyChanged(nameof(IsCustomColorScaleUsed));
        }
      }
    }



    private bool _clipToLayer;

    /// <summary>
    /// Initializes the content of the ClipToLayer checkbox
    /// </summary>
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

    private IMVCANController _scaleController;

    public IMVCANController ScaleController
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
          _colorProviderController?.Dispose();
          _colorProviderController = value;
          OnPropertyChanged(nameof(ColorProviderController));
        }
      }
    }

    private IMVCANController _materialController;

    public IMVCANController MaterialController
    {
      get => _materialController;
      set
      {
        if (!(_materialController == value))
        {
          _materialController?.Dispose();
          _materialController = value;
          OnPropertyChanged(nameof(MaterialController));
        }
      }
    }




    #endregion

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        var scaleController = new DensityScaleController(newScale => _doc.ColorScale = (NumericalScale)newScale) { UseDocumentCopy = UseDocument.Directly };
        scaleController.InitializeDocument(_doc.ColorScale ?? new LinearScale());
        ScaleController = scaleController;

        var colorProviderController = new ColorProviderController(newColorProvider => _doc.ColorProvider = newColorProvider) { UseDocumentCopy = UseDocument.Directly };
        colorProviderController.InitializeDocument(_doc.ColorProvider);
        ColorProviderController = colorProviderController;

        var materialController = new MaterialController() { UseDocumentCopy = UseDocument.Directly };
        materialController.InitializeDocument(_doc.Material);
        MaterialController = materialController;

        IsCustomColorScaleUsed = _doc.ColorScale is not null;
        ClipToLayer = _doc.ClipToLayer;
      }
    }

    public override bool Apply(bool disposeController)
    {
      if (!_scaleController.Apply(disposeController))
        return false;

      if (!_colorProviderController.Apply(disposeController))
        return false;

      if (!_materialController.Apply(disposeController))
        return false;
      else
        _doc.Material = (IMaterial)_materialController.ModelObject;

      _doc.ClipToLayer = ClipToLayer;
      _doc.ColorScale = IsCustomColorScaleUsed ? (NumericalScale)_scaleController.ModelObject : null;
      _doc.ColorProvider = (IColorProvider)_colorProviderController.ModelObject;

      return ApplyEnd(true, disposeController);
    }
  }
}
