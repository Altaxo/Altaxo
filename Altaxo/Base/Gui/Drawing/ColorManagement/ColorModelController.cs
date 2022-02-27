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
using Altaxo.Gui.Common;

namespace Altaxo.Gui.Drawing.ColorManagement
{
  public interface IColorModelView : IDataContextAwareView
  {
    void InitializeColorModel(IColorModel colorModel, bool silentSet);

    void InitializeTextOnlyColorModel(ITextOnlyColorModel colorModel, bool silentSet);

    void InitializeCurrentColor(AxoColor color);

    event Action<AxoColor> CurrentColorChanged;
  }

  [ExpectedTypeOfView(typeof(IColorModelView))]
  public class ColorModelController : MVCANDControllerEditImmutableDocBase<AxoColor, IColorModelView>
  {

    private IColorModel _currentColorModel;
    private ITextOnlyColorModel _currentTextOnlyColorModel;

    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    #region Bindings

    private ItemsController<Type> _availableColorModels;

    public ItemsController<Type> AvailableColorModels
    {
      get => _availableColorModels;
      set
      {
        if (!(_availableColorModels == value))
        {
          _availableColorModels = value;
          OnPropertyChanged(nameof(AvailableColorModels));
        }
      }
    }

    private ItemsController<Type> _availableTextOnlyColorModels;

    public ItemsController<Type> AvailableTextOnlyColorModels
    {
      get => _availableTextOnlyColorModels;
      set
      {
        if (!(_availableTextOnlyColorModels == value))
        {
          _availableTextOnlyColorModels = value;
          OnPropertyChanged(nameof(AvailableTextOnlyColorModels));
        }
      }
    }



    #endregion

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        // Availabe color models
        var availableColorModels = new SelectableListNodeList();
        var models = Altaxo.Main.Services.ReflectionService.GetNonAbstractSubclassesOf(typeof(IColorModel));
        _currentColorModel = new ColorModelRGB();
        foreach (var modelType in models)
        {
          availableColorModels.Add(new SelectableListNode(Current.Gui.GetUserFriendlyClassName(modelType), modelType, modelType == _currentColorModel.GetType()));
        }
        AvailableColorModels = new ItemsController<Type>(availableColorModels, EhColorModelSelectionChanged);

        // Text only color models
       var availableTextOnlyColorModels = new SelectableListNodeList();
        var textOnlyModels = Altaxo.Main.Services.ReflectionService.GetNonAbstractSubclassesOf(typeof(ITextOnlyColorModel));
        _currentTextOnlyColorModel = new TextOnlyColorModelRGB();
        foreach (var modelType in textOnlyModels)
        {
          availableTextOnlyColorModels.Add(new SelectableListNode(Current.Gui.GetUserFriendlyClassName(modelType), modelType, modelType == _currentTextOnlyColorModel.GetType()));
        }
        AvailableTextOnlyColorModels = new ItemsController<Type>(availableTextOnlyColorModels, EhTextOnlyColorModelSelectionChanged);
      }
      if (_view is not null)
      {
        _view.InitializeColorModel(_currentColorModel, false);
        _view.InitializeTextOnlyColorModel(_currentTextOnlyColorModel, false);
        _view.InitializeCurrentColor(_doc);
      }
    }

    public override bool Apply(bool disposeController)
    {
      return ApplyEnd(true, disposeController);
    }

    protected override void AttachView()
    {
      base.AttachView();

      _view.CurrentColorChanged += EhCurrentColorChanged;
    }

    protected override void DetachView()
    {
      _view.CurrentColorChanged -= EhCurrentColorChanged;

      base.DetachView();
    }

    private void EhColorModelSelectionChanged(Type selType)
    {
      if (selType is not null && selType != _currentColorModel.GetType())
      {
        var newColorModel = (IColorModel)Activator.CreateInstance(selType);
        _currentColorModel = newColorModel;
        _view.InitializeColorModel(_currentColorModel, false);
      }
    }

    private void EhTextOnlyColorModelSelectionChanged(Type selType)
    {
      if (selType is not null && selType != _currentTextOnlyColorModel.GetType())
      {
        var newTextOnlyColorModel = (ITextOnlyColorModel)Activator.CreateInstance(selType);
        _currentTextOnlyColorModel = newTextOnlyColorModel;
        _view.InitializeTextOnlyColorModel(_currentTextOnlyColorModel, false);
      }
    }

    private void EhCurrentColorChanged(AxoColor color)
    {
      _doc = color;
      OnMadeDirty();
    }
  }
}
