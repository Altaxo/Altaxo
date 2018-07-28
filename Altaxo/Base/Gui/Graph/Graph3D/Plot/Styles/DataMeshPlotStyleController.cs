#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2016 Dr. Dirk Lellinger
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

using Altaxo.Graph.Gdi.Plot;
using Altaxo.Graph.Gdi.Plot.Styles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Gui.Graph.Graph3D.Plot.Styles
{
  using Altaxo.Drawing.D3D;
  using Altaxo.Graph.Graph3D.Plot.Styles;
  using Altaxo.Graph.Scales;
  using Drawing.D3D;
  using Gdi.Plot.ColorProvider;
  using Graph;
  using Scales;

  #region Interfaces

  public interface IDataMeshPlotStyleView
  {
    IDensityScaleView ColorScaleView { get; }

    bool IsCustomColorScaleUsed { get; set; }

    IColorProviderView ColorProviderView { get; }

    /// <summary>
    /// Initializes the content of the ClipToLayer checkbox
    /// </summary>
    bool ClipToLayer { get; set; }

    object MaterialViewObject { get; }
  }

  #endregion Interfaces

  /// <summary>
  /// Controller for the density image plot style
  /// </summary>
  [UserControllerForObject(typeof(DataMeshPlotStyle))]
  [ExpectedTypeOfView(typeof(IDataMeshPlotStyleView))]
  public class DataMeshPlotStyleController : MVCANControllerEditOriginalDocBase<DataMeshPlotStyle, IDataMeshPlotStyleView>
  {
    private IMVCANController _scaleController;
    private IMVCANController _colorProviderController;
    private IMVCANController _materialController;

    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield return new ControllerAndSetNullMethod(_scaleController, () => _scaleController = null);
      yield return new ControllerAndSetNullMethod(_colorProviderController, () => _colorProviderController = null);
      yield return new ControllerAndSetNullMethod(_materialController, () => _materialController = null);
    }

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        _scaleController = new DensityScaleController(newScale => _doc.ColorScale = (NumericalScale)newScale) { UseDocumentCopy = UseDocument.Directly };
        _scaleController.InitializeDocument(_doc.ColorScale ?? new LinearScale());

        _colorProviderController = new ColorProviderController(newColorProvider => _doc.ColorProvider = newColorProvider) { UseDocumentCopy = UseDocument.Directly };
        _colorProviderController.InitializeDocument(_doc.ColorProvider);

        _materialController = new MaterialController() { UseDocumentCopy = UseDocument.Directly };
        _materialController.InitializeDocument(_doc.Material);
      }

      if (_view != null)
      {
        _scaleController.ViewObject = _view.ColorScaleView;
        _view.IsCustomColorScaleUsed = null != _doc.ColorScale;
        _colorProviderController.ViewObject = _view.ColorProviderView;

        if (null == _materialController.ViewObject)
          _materialController.ViewObject = _view.MaterialViewObject;

        _view.ClipToLayer = _doc.ClipToLayer;
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

      _doc.ClipToLayer = _view.ClipToLayer;
      _doc.ColorScale = _view.IsCustomColorScaleUsed ? (NumericalScale)_scaleController.ModelObject : null;
      _doc.ColorProvider = (IColorProvider)_colorProviderController.ModelObject;

      return ApplyEnd(true, disposeController);
    }
  }
}
