#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2017 Dr. Dirk Lellinger
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
using System.Threading.Tasks;
using Altaxo.Collections;
using Altaxo.Drawing;

namespace Altaxo.Gui.Drawing.ColorManagement
{
  /// <summary>
  /// View contract for editing colors using circle and variation models.
  /// </summary>
  public interface IColorCircleView
  {
    /// <summary>
    /// Initializes the available color models.
    /// </summary>
    void InitializeAvailableColorModels(SelectableListNodeList listOfColorModels);

    /// <summary>
    /// Initializes the available text-only color models.
    /// </summary>
    void InitializeAvailableTextOnlyColorModels(SelectableListNodeList listOfTextOnlyColorModels);

    /// <summary>
    /// Initializes the current color model.
    /// </summary>
    void InitializeColorModel(IColorModel colorModel, bool silentSet);

    /// <summary>
    /// Initializes the current text-only color model.
    /// </summary>
    void InitializeTextOnlyColorModel(ITextOnlyColorModel colorModel, bool silentSet);

    /// <summary>
    /// Initializes the available color circle models.
    /// </summary>
    void InitializeAvailableColorCircleModels(SelectableListNodeList availableColorCircleModels);

    /// <summary>
    /// Initializes the current color circle model.
    /// </summary>
    IReadOnlyList<double> InitializeCurrentColorCircleModel(IColorCircleModel currentColorCircleModel, bool silentSet);

    /// <summary>
    /// Initializes the available color variation models.
    /// </summary>
    void InitializeAvailableColorVariationModels(SelectableListNodeList availableColorVariationModels);

    /// <summary>
    /// Initializes the current color variation model.
    /// </summary>
    void InitializeCurrentColorVariationModel(IColorVariationModel currentColorVariationModel, bool silentSet);

    /// <summary>
    /// Initializes the currently selected color.
    /// </summary>
    void InitializeCurrentColor(AxoColor color);

    /// <summary>
    /// Initializes the number of generated color shades.
    /// </summary>
    void InitializeNumberOfColorShades(int numberOfShades);

    /// <summary>
    /// Initializes the generated color shades.
    /// </summary>
    void InitializeColorShades(AxoColor[][] colorShades);

    /// <summary>
    /// Occurs when the selected color model changes.
    /// </summary>
    event Action ColorModelSelectionChanged;

    /// <summary>
    /// Occurs when the selected text-only color model changes.
    /// </summary>
    event Action TextOnlyColorModelSelectionChanged;

    /// <summary>
    /// Occurs when the selected color circle model changes.
    /// </summary>
    event Action ColorCircleModelChanged;

    /// <summary>
    /// Occurs when the selected color variation model changes.
    /// </summary>
    event Action ColorVariationModelChanged;

    /// <summary>
    /// Occurs when the current color changes.
    /// </summary>
    event Action<AxoColor> CurrentColorChanged;

    /// <summary>
    /// Occurs when at least one of the hue values of the color circle has changed. Argument is the list of hue values of the circle, with the first item
    /// always representing the main hue value.
    /// </summary>

    event Action<IReadOnlyList<double>> HueValuesChanged;

    /// <summary>
    /// Occurs when the number of generated color shades changes.
    /// </summary>
    event Action<int> NumberOfColorShadesChanged;
  }

  /// <summary>
  /// Controller for editing colors using circle and variation models.
  /// </summary>
  [ExpectedTypeOfView(typeof(IColorCircleView))]
  public class ColorCircleController : MVCANDControllerEditImmutableDocBase<AxoColor, IColorCircleView>
  {
    private SelectableListNodeList _availableColorModels;
    private SelectableListNodeList _availableTextOnlyColorModels;
    private SelectableListNodeList _availableColorCircleModels;
    private SelectableListNodeList _availableColorVariationModels;

    private IColorModel _currentColorModel;
    private ITextOnlyColorModel _currentTextOnlyColorModel;
    private IColorCircleModel _currentColorCircleModel;
    private IColorVariationModel _currentColorVariationModel;

    private int _numberOfColorShades = 20;
    private IReadOnlyList<double> _hueValues;

    private AxoColor[][] _colorShades;

    /// <inheritdoc/>
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    /// <inheritdoc/>
    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        _availableColorModels = new SelectableListNodeList();

        var models = Altaxo.Main.Services.ReflectionService.GetNonAbstractSubclassesOf(typeof(IColorModel));

        _currentColorModel = new ColorModelRGB();
        foreach (var modelType in models)
        {
          _availableColorModels.Add(new SelectableListNode(Current.Gui.GetUserFriendlyClassName(modelType), modelType, modelType == _currentColorModel.GetType()));
        }

        // Text only color models
        _availableTextOnlyColorModels = new SelectableListNodeList();
        var textOnlyModels = Altaxo.Main.Services.ReflectionService.GetNonAbstractSubclassesOf(typeof(ITextOnlyColorModel));

        _currentTextOnlyColorModel = new TextOnlyColorModelRGB();
        foreach (var modelType in textOnlyModels)
        {
          _availableTextOnlyColorModels.Add(new SelectableListNode(Current.Gui.GetUserFriendlyClassName(modelType), modelType, modelType == _currentTextOnlyColorModel.GetType()));
        }

        // Color circle models
        _availableColorCircleModels = new SelectableListNodeList();
        var colorCircleModels = Altaxo.Main.Services.ReflectionService.GetNonAbstractSubclassesOf(typeof(IColorCircleModel));

        _currentColorCircleModel = _currentColorCircleModel ?? new ColorCircleModelComplementary();
        foreach (var type in colorCircleModels)
        {
          _availableColorCircleModels.Add(new SelectableListNode(Current.Gui.GetUserFriendlyClassName(type), type, type == _currentColorCircleModel.GetType()));
        }

        // Color variation models
        _availableColorVariationModels = new SelectableListNodeList();
        var colorVariationModels = Altaxo.Main.Services.ReflectionService.GetNonAbstractSubclassesOf(typeof(IColorVariationModel));

        _currentColorVariationModel = _currentColorVariationModel ?? new ColorVariationModelDesaturate();
        foreach (var type in colorVariationModels)
        {
          _availableColorVariationModels.Add(new SelectableListNode(Current.Gui.GetUserFriendlyClassName(type), type, type == _currentColorVariationModel.GetType()));
        }
      }
      if (_view is not null)
      {
        _view.InitializeAvailableColorModels(_availableColorModels);
        _view.InitializeAvailableTextOnlyColorModels(_availableTextOnlyColorModels);
        _view.InitializeColorModel(_currentColorModel, false);
        _view.InitializeTextOnlyColorModel(_currentTextOnlyColorModel, false);

        _view.InitializeAvailableColorCircleModels(_availableColorCircleModels);
        _hueValues = _view.InitializeCurrentColorCircleModel(_currentColorCircleModel, false);

        _view.InitializeAvailableColorVariationModels(_availableColorVariationModels);
        _view.InitializeCurrentColorVariationModel(_currentColorVariationModel, false);
        _view.InitializeNumberOfColorShades(_numberOfColorShades);
        _view.InitializeCurrentColor(_doc);
        UpdateColorShades();
      }
    }

    /// <inheritdoc/>
    public override bool Apply(bool disposeController)
    {
      return ApplyEnd(true, disposeController);
    }

    /// <inheritdoc/>
    protected override void AttachView()
    {
      base.AttachView();

      _view.HueValuesChanged += EhView_HueValuesChanged;
      _view.NumberOfColorShadesChanged += EhView_NumberOfColorShadesChanged;

      _view.ColorCircleModelChanged += EhColorCircleModelChanged;

      _view.ColorVariationModelChanged += EhColorVariationModelChanged;

      _view.ColorModelSelectionChanged += EhColorModelSelectionChanged;
      _view.TextOnlyColorModelSelectionChanged += EhTextOnlyColorModelSelectionChanged;
      _view.CurrentColorChanged += EhCurrentColorChanged;
    }

    /// <inheritdoc/>
    protected override void DetachView()
    {
      _view.HueValuesChanged -= EhView_HueValuesChanged;
      _view.NumberOfColorShadesChanged -= EhView_NumberOfColorShadesChanged;

      _view.ColorCircleModelChanged -= EhColorCircleModelChanged;

      _view.ColorVariationModelChanged -= EhColorVariationModelChanged;

      _view.ColorModelSelectionChanged -= EhColorModelSelectionChanged;
      _view.TextOnlyColorModelSelectionChanged -= EhTextOnlyColorModelSelectionChanged;
      _view.CurrentColorChanged -= EhCurrentColorChanged;

      base.DetachView();
    }

    private void EhView_HueValuesChanged(IReadOnlyList<double> hueValues)
    {
      _hueValues = hueValues;
      UpdateColorShades();
    }

    private void EhView_NumberOfColorShadesChanged(int numberOfColorShades)
    {
      _numberOfColorShades = numberOfColorShades;
      UpdateColorShades();
    }

    /// <summary>
    /// Updates the generated color shades.
    /// </summary>
    private void UpdateColorShades()
    {
      if (_hueValues is null)
        return; // not initialized yet

      if (_colorShades is null || _colorShades.Length != _hueValues.Count || _colorShades[0].Length != _numberOfColorShades)
      {
        _colorShades = new AxoColor[_hueValues.Count][];
        for (int i = 0; i < _hueValues.Count; ++i)
          _colorShades[i] = new AxoColor[_numberOfColorShades];
      }

      for (int i = 0; i < _hueValues.Count; ++i)
      {
        var baseColor = AxoColor.FromAhsb(1, (float)_hueValues[i], 1, 1);
        _currentColorVariationModel.GetColorVariations(baseColor, _colorShades[i]);
      }

      _view?.InitializeColorShades(_colorShades);
    }

    private void EhColorModelSelectionChanged()
    {
      var node = _availableColorModels.FirstSelectedNode;

      if (node is not null && (Type)node.Tag != _currentColorModel.GetType())
      {
        var newColorModel = (IColorModel)Activator.CreateInstance((Type)node.Tag);
        _currentColorModel = newColorModel;
        _view.InitializeColorModel(_currentColorModel, false);
      }
    }

    private void EhTextOnlyColorModelSelectionChanged()
    {
      var node = _availableTextOnlyColorModels.FirstSelectedNode;

      if (node is not null && (Type)node.Tag != _currentTextOnlyColorModel.GetType())
      {
        var newTextOnlyColorModel = (ITextOnlyColorModel)Activator.CreateInstance((Type)node.Tag);
        _currentTextOnlyColorModel = newTextOnlyColorModel;
        _view.InitializeTextOnlyColorModel(_currentTextOnlyColorModel, false);
      }
    }

    private void EhColorCircleModelChanged()
    {
      var node = _availableColorCircleModels.FirstSelectedNode;

      if (node is not null && (Type)node.Tag != _currentColorCircleModel.GetType())
      {
        var newColorModel = (IColorCircleModel)Activator.CreateInstance((Type)node.Tag);
        _currentColorCircleModel = newColorModel;
        _view.InitializeCurrentColorCircleModel(_currentColorCircleModel, false);
      }
    }

    private void EhColorVariationModelChanged()
    {
      var node = _availableColorVariationModels.FirstSelectedNode;

      if (node is not null && (Type)node.Tag != _currentColorVariationModel.GetType())
      {
        var newColorModel = (IColorVariationModel)Activator.CreateInstance((Type)node.Tag);
        _currentColorVariationModel = newColorModel;
        _view.InitializeCurrentColorVariationModel(_currentColorVariationModel, false);

        UpdateColorShades();
      }
    }

    private void EhCurrentColorChanged(AxoColor color)
    {
      _doc = color;
      OnMadeDirty();
    }
  }
}
