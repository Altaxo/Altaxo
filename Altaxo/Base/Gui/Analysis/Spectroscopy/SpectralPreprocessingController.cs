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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Altaxo.Collections;
using Altaxo.Gui.Common;
using Altaxo.Science.Spectroscopy.BaselineEstimation;
using Altaxo.Science.Spectroscopy.Cropping;
using Altaxo.Science.Spectroscopy.Normalization;
using Altaxo.Science.Spectroscopy.PeakFitting;
using Altaxo.Science.Spectroscopy.PeakSearching;
using Altaxo.Science.Spectroscopy.Smoothing;
using Altaxo.Science.Spectroscopy.SpikeRemoval;

namespace Altaxo.Gui.Analysis.Spectroscopy
{
  public record SpectralPreprocessingOptions
  {
    public ISpikeRemoval SpikeRemoval { get; init; } = new SpikeRemovalNone();

    public ISmoothing Smoothing { get; init; } = new SmoothingNone();

    public IBaselineEstimation BaselineEstimation { get; init; } = new BaselineEstimationNone();

    public ICropping Cropping { get; init; } = new CroppingNone();

    public INormalization Normalization { get; init; } = new NormalizationNone();

    public IPeakSearching PeakSearching { get; init; } = new PeakSearchingNone();

    public IPeakFitting PeakFitting { get; init; } = new PeakFittingNone();
  }

  public interface ISpectralPreprocessingOptionsView : IDataContextAwareView
  {
  }

  [UserControllerForObject(typeof(SpectralPreprocessingOptions))]
  [ExpectedTypeOfView(typeof(ISpectralPreprocessingOptionsView))]
  public class SpectralPreprocessingController : MVCANControllerEditImmutableDocBase<SpectralPreprocessingOptions, ISpectralPreprocessingOptionsView>
  {
    IMVCANController _selectedController;

    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      if(TabControllers?.Items is { } list)
      {
        foreach(var item in list)
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

      if(initData)
      {
        var controllers = new SelectableListNodeList();

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


        TabControllers = new ItemsController<IMVCANController>(controllers, EhSelectedTabChanged);
        _selectedController = (IMVCANController)controllers[0].Tag;
        TabControllers.SelectedValue = _selectedController;
      }
    }

    private void EhSelectedTabChanged(IMVCANController controller)
    {
     if(_selectedController is not null)
      {
        _selectedController.Apply(false);
        var model = _selectedController.ModelObject;
        UpdateDoc(model);
      }

      _selectedController = controller;
    }

    private void UpdateDoc(object model)
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
        case IPeakSearching ps:
          _doc = _doc with { PeakSearching = ps };
          break;
        case IPeakFitting pf:
          _doc = _doc with { PeakFitting = pf };
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
