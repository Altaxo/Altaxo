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
using System;
using System.Collections.Generic;
using System.Windows.Input;
using Altaxo.Graph.Graph3D.Plot.Groups;

namespace Altaxo.Gui.Graph.Graph3D.Plot.Groups
{
  public interface IPlotGroupCollectionView : IDataContextAwareView
  {
  }

  /// <summary>
  /// This is the controller for a <see cref="PlotGroupStyleCollection"/> that choose between the simple and the advanced presentation mode.
  /// </summary>
  [ExpectedTypeOfView(typeof(IPlotGroupCollectionView))]
  public class PlotGroupCollectionController : MVCANControllerEditOriginalDocBase<PlotGroupStyleCollection, IPlotGroupCollectionView>
  {
    public event Action GroupStyleChanged;

    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield return new ControllerAndSetNullMethod(_controllerAdvanced, () => { _controllerAdvanced = null; });
      yield return new ControllerAndSetNullMethod(_controllerSimple, () => _controllerSimple = null);
    }

    #region Bindings

    private PlotGroupCollectionControllerAdvanced _controllerAdvanced;

    public PlotGroupCollectionControllerAdvanced ControllerAdvanced
    {
      get => _controllerAdvanced;
      set
      {
        if (!(_controllerAdvanced == value))
        {
          _controllerAdvanced?.Dispose();
          _controllerAdvanced = value;
          if (_controllerAdvanced is not null)
          {
            _controllerAdvanced.GroupStyleChanged += new WeakActionHandler(EhGroupStyleChanged, _controllerAdvanced, nameof(_controllerAdvanced.GroupStyleChanged));
          }
          OnPropertyChanged(nameof(ControllerAdvanced));
          OnPropertyChanged(nameof(IsSimpleViewActive));
          OnPropertyChanged(nameof(CurrentView));
        }
      }
    }


    private PlotGroupCollectionControllerSimple _controllerSimple;

    public PlotGroupCollectionControllerSimple ControllerSimple
    {
      get => _controllerSimple;
      set
      {
        if (!(_controllerSimple == value))
        {
          _controllerSimple?.Dispose();
          _controllerSimple = value;
          OnPropertyChanged(nameof(ControllerSimple));
          OnPropertyChanged(nameof(IsSimpleViewActive));
          OnPropertyChanged(nameof(CurrentView));
        }
      }
    }

    public object CurrentView
    {
      get
      {
        if (_controllerAdvanced is { } controllerAdvanced)
        {
          if (controllerAdvanced.ViewObject is null)
            Current.Gui.FindAndAttachControlTo(controllerAdvanced);
          return controllerAdvanced.ViewObject;
        }
        else if (_controllerSimple is { } controllerSimple)
        {
          if (controllerSimple.ViewObject is null)
            Current.Gui.FindAndAttachControlTo(controllerSimple);
          return controllerSimple.ViewObject;
        }
        else
        {
          return null;
        }
      }
    }

    public bool IsSimpleViewActive
    {
      get => _controllerSimple is not null;
    }
    public bool IsAdvancedViewActive
    {
      get => _controllerAdvanced is not null;
    }

    public ICommand CmdGotoSimple { get; }
    public ICommand CmdGotoAdvanced { get; }
    #endregion

    public PlotGroupCollectionController()
    {
      CmdGotoSimple = new RelayCommand(EhView_GotoSimple);
      CmdGotoAdvanced = new RelayCommand(EhView_GotoAdvanced);
    }

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {

        if (PlotGroupCollectionControllerSimple.IsSimplePlotGrouping(_doc, out var isSerialStepping, out var isColor, out var isLineStyle, out var isSymbolStyle))
        {
          var controllerSimple = new PlotGroupCollectionControllerSimple() { UseDocumentCopy = UseDocument.Directly };
          controllerSimple.InitializeDocument(_doc);
          ControllerSimple = controllerSimple;
        }
        else
        {
          var controllerAdvanced = new PlotGroupCollectionControllerAdvanced() { UseDocumentCopy = UseDocument.Directly };
          controllerAdvanced.InitializeDocument(_doc);
          ControllerAdvanced = controllerAdvanced;
        }
      }
    }

    private void EhGroupStyleChanged()
    {
      GroupStyleChanged?.Invoke();
    }

    public override bool Apply(bool disposeController)
    {
      bool result;
      if (_controllerSimple is not null)
        result = _controllerSimple.Apply(disposeController);
      else
        result = _controllerAdvanced.Apply(disposeController);

      if (true == result)
      {
        if (_controllerSimple is not null)
        {
          _doc = (PlotGroupStyleCollection)_controllerSimple.ModelObject;
        }
        else
        {
          _doc = (PlotGroupStyleCollection)_controllerAdvanced.ModelObject;
        }
      }

      return ApplyEnd(result, disposeController);
    }

    private void EhView_GotoSimple()
    {
      _controllerAdvanced.Apply(false);
      _doc = (PlotGroupStyleCollection)_controllerAdvanced.ModelObject;

      if (PlotGroupCollectionControllerSimple.IsSimplePlotGrouping(_doc))
      {
        ControllerAdvanced = null;
        var controllerSimple = new PlotGroupCollectionControllerSimple
        {
          UseDocumentCopy = UseDocument.Directly
        };
        controllerSimple.InitializeDocument(_doc);
        ControllerSimple = controllerSimple;
      }
      else
      {
        Current.Gui.ErrorMessageBox("Sorry, this collection is too complicate to be represented by a simple view.");
      }
    }

    private void EhView_GotoAdvanced()
    {
      _controllerSimple.Apply(false);
      _doc = (PlotGroupStyleCollection)_controllerSimple.ModelObject;
      ControllerSimple = null;

      var controllerAdvanced = new PlotGroupCollectionControllerAdvanced
      {
        UseDocumentCopy = UseDocument.Directly
      };
      controllerAdvanced.InitializeDocument(_doc);
      controllerAdvanced.GroupStyleChanged += new WeakActionHandler(EhGroupStyleChanged, controllerAdvanced, nameof(controllerAdvanced.GroupStyleChanged));
      ControllerAdvanced = controllerAdvanced;
      Initialize(false);
    }
  }
}
