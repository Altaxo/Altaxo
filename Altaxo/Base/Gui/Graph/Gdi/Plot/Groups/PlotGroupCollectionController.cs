﻿#region Copyright

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

#nullable disable
using System;
using System.Collections.Generic;
using System.Text;
using Altaxo.Graph.Gdi.Plot.Groups;

namespace Altaxo.Gui.Graph.Gdi.Plot.Groups
{
  #region Interface

  public interface IPlotGroupCollectionView
  {
    /// <summary>
    /// Sets the simple view object and makes it visible.
    /// </summary>
    /// <param name="viewObject">The object to visualize.</param>
    void SetSimpleView(object viewObject);

    /// <summary>
    /// Sets the advanced view object  and makes it visible.
    /// </summary>
    /// <param name="viewObject">The object to visualize.</param>
    void SetAdvancedView(object viewObject);

    event Action GotoAdvanced;

    event Action GotoSimple;
  }

  #endregion Interface

  /// <summary>
  /// This is the controller for a <see cref="PlotGroupStyleCollection"/> that choose between the simple and the advanced presentation mode.
  /// </summary>
  [ExpectedTypeOfView(typeof(IPlotGroupCollectionView))]
  public class PlotGroupCollectionController : MVCANControllerEditOriginalDocBase<PlotGroupStyleCollection, IPlotGroupCollectionView>
  {
    private PlotGroupCollectionControllerAdvanced _controllerAdvanced;
    private PlotGroupCollectionControllerSimple _controllerSimple;

    public event Action GroupStyleChanged;

    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield return new ControllerAndSetNullMethod(_controllerAdvanced, () => { _controllerAdvanced = null; });
      yield return new ControllerAndSetNullMethod(_controllerSimple, () => _controllerSimple = null);
    }

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        if (_controllerSimple is not null)
        {
          _controllerSimple.Dispose();
          _controllerSimple = null;
        }
        if (_controllerAdvanced is not null)
        {
          _controllerAdvanced.Dispose();
          _controllerAdvanced = null;
        }

        if (PlotGroupCollectionControllerSimple.IsSimplePlotGrouping(_doc, out var isSerialStepping, out var isColor, out var isLineStyle, out var isSymbolStyle))
        {
          _controllerSimple = new PlotGroupCollectionControllerSimple() { UseDocumentCopy = UseDocument.Directly };
          _controllerSimple.InitializeDocument(_doc);
        }
        else
        {
          var controllerAdvanced = new PlotGroupCollectionControllerAdvanced() { UseDocumentCopy = UseDocument.Directly };
          controllerAdvanced.InitializeDocument(_doc);
          controllerAdvanced.GroupStyleChanged += new WeakActionHandler(EhGroupStyleChanged, controllerAdvanced, nameof(controllerAdvanced.GroupStyleChanged));
          _controllerAdvanced = controllerAdvanced;
        }
      }

      if (_view is not null)
      {
        if (_controllerSimple is not null)
        {
          if (_controllerSimple.ViewObject is null)
            Current.Gui.FindAndAttachControlTo(_controllerSimple);
          _view.SetSimpleView(_controllerSimple.ViewObject);
        }
        else if (_controllerAdvanced is not null)
        {
          if (_controllerAdvanced.ViewObject is null)
            Current.Gui.FindAndAttachControlTo(_controllerAdvanced);
          _view.SetAdvancedView(_controllerAdvanced.ViewObject);
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

    protected override void AttachView()
    {
      base.AttachView();

      _view.GotoAdvanced += new Action(EhView_GotoAdvanced);
      _view.GotoSimple += new Action(EhView_GotoSimple);
    }

    protected override void DetachView()
    {
      _view.GotoAdvanced -= new Action(EhView_GotoAdvanced);
      _view.GotoSimple -= new Action(EhView_GotoSimple);

      base.DetachView();
    }

    private void EhView_GotoSimple()
    {
      _controllerAdvanced.Apply(false);
      _doc = (PlotGroupStyleCollection)_controllerAdvanced.ModelObject;

      if (PlotGroupCollectionControllerSimple.IsSimplePlotGrouping(_doc))
      {
        _controllerAdvanced = null;
        _controllerSimple = new PlotGroupCollectionControllerSimple
        {
          UseDocumentCopy = UseDocument.Directly
        };
        _controllerSimple.InitializeDocument(_doc);
        Initialize(false);
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
      _controllerSimple = null;

      var controllerAdvanced = new PlotGroupCollectionControllerAdvanced
      {
        UseDocumentCopy = UseDocument.Directly
      };
      controllerAdvanced.InitializeDocument(_doc);
      controllerAdvanced.GroupStyleChanged += new WeakActionHandler(EhGroupStyleChanged, controllerAdvanced, nameof(controllerAdvanced.GroupStyleChanged));
      _controllerAdvanced = controllerAdvanced;
      Initialize(false);
    }
  }
}
