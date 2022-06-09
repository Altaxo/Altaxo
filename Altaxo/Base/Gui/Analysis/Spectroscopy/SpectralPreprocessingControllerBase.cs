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

using System.Collections.Generic;
using Altaxo.Collections;
using Altaxo.Gui.Common;
using Altaxo.Science.Spectroscopy;
using Altaxo.Science.Spectroscopy.BaselineEstimation;
using Altaxo.Science.Spectroscopy.Cropping;
using Altaxo.Science.Spectroscopy.Normalization;
using Altaxo.Science.Spectroscopy.Smoothing;
using Altaxo.Science.Spectroscopy.SpikeRemoval;

namespace Altaxo.Gui.Analysis.Spectroscopy
{


  public interface ISpectralPreprocessingOptionsView : IDataContextAwareView
  {
  }


  public class SpectralPreprocessingControllerBase<TOptions> : MVCANControllerEditImmutableDocBase<TOptions, ISpectralPreprocessingOptionsView> where TOptions : SpectralPreprocessingOptions
  {
    IMVCANController _selectedController;

    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      if (TabControllers?.Items is { } list)
      {
        foreach (var item in list)
        {
          yield return new ControllerAndSetNullMethod(((SelectableListNodeWithController)item).Controller, () => { item.Tag = null; ((SelectableListNodeWithController)item).Controller = null; });
        }
      }
    }

    #region Bindings

    private ItemsController<IMVCANController> _tabControllers;

    public ItemsController<IMVCANController> TabControllers
    {
      get => _tabControllers;
      set
      {
        if (!(_tabControllers == value))
        {
          _tabControllers = value;
          OnPropertyChanged(nameof(TabControllers));
        }
      }
    }




    #endregion

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        var controllers = new SelectableListNodeList();
        AddControllers(controllers);




        TabControllers = new ItemsController<IMVCANController>(controllers, EhSelectedTabChanged);
        _selectedController = (IMVCANController)controllers[0].Tag;
        TabControllers.SelectedValue = _selectedController;
      }
    }

    protected virtual void AddControllers(SelectableListNodeList controllers)
    {


      {
        var controller = new SpikeRemoval.SpikeRemovalController();
        controller.InitializeDocument(_doc.SpikeRemoval);
        Current.Gui.FindAndAttachControlTo(controller);
        controllers.Add(new SelectableListNodeWithController("Spike removal", controller, false)
        { Controller = controller });
      }

      {
        var controller = new Smoothing.SmoothingController();
        controller.InitializeDocument(_doc.Smoothing);
        Current.Gui.FindAndAttachControlTo(controller);
        controllers.Add(new SelectableListNodeWithController("Smoothing", controller, false)
        { Controller = controller });
      }

      {
        var controller = new BaselineEstimation.BaselineEstimationController();
        controller.InitializeDocument(_doc.BaselineEstimation);
        Current.Gui.FindAndAttachControlTo(controller);
        controllers.Add(new SelectableListNodeWithController("Baseline", controller, false)
        { Controller = controller });
      }

      {
        var controller = new Cropping.CroppingController();
        controller.InitializeDocument(_doc.Cropping);
        Current.Gui.FindAndAttachControlTo(controller);
        controllers.Add(new SelectableListNodeWithController("Cropping", controller, false)
        { Controller = controller });
      }

      {
        var controller = new Normalization.NormalizationController();
        controller.InitializeDocument(_doc.Normalization);
        Current.Gui.FindAndAttachControlTo(controller);
        controllers.Add(new SelectableListNodeWithController("Normalization", controller, false)
        { Controller = controller });
      }
    }

    private void EhSelectedTabChanged(IMVCANController controller)
    {
      if (_selectedController is not null)
      {
        _selectedController.Apply(false);
        var model = _selectedController.ModelObject;
        UpdateDoc(model);
      }

      _selectedController = controller;
    }

    protected virtual void UpdateDoc(object model)
    {
      switch (model)
      {
        case null:
          break;
        case ISpikeRemoval sr:
          _doc = _doc with { SpikeRemoval = sr };
          break;
        case INormalization no:
          _doc = _doc with { Normalization = no };
          break;
        case ICropping cr:
          _doc = _doc with { Cropping = cr };
          break;
        case IBaselineEstimation be:
          _doc = _doc with { BaselineEstimation = be };
          break;
        case ISmoothing sm:
          _doc = _doc with { Smoothing = sm };
          break;

      }
    }

    public override bool Apply(bool disposeController)
    {
      if (_selectedController is not null)
      {
        _selectedController.Apply(false);
        var model = _selectedController.ModelObject;
        UpdateDoc(model);
      }

      return ApplyEnd(true, disposeController);
    }
  }
}
