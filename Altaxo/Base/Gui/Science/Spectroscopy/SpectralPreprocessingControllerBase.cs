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
using System.Windows.Input;
using Altaxo.Collections;
using Altaxo.Gui.Common;
using Altaxo.Science.Spectroscopy;
using Altaxo.Science.Spectroscopy.BaselineEstimation;
using Altaxo.Science.Spectroscopy.Calibration;
using Altaxo.Science.Spectroscopy.Cropping;
using Altaxo.Science.Spectroscopy.DarkSubtraction;
using Altaxo.Science.Spectroscopy.Normalization;
using Altaxo.Science.Spectroscopy.Resampling;
using Altaxo.Science.Spectroscopy.Sanitizing;
using Altaxo.Science.Spectroscopy.Smoothing;
using Altaxo.Science.Spectroscopy.SpikeRemoval;

namespace Altaxo.Gui.Science.Spectroscopy
{


  public interface ISpectralPreprocessingOptionsView : IDataContextAwareView
  {
  }


  public abstract class SpectralPreprocessingControllerBase<TOptions> : MVCANControllerEditImmutableDocBase<TOptions, ISpectralPreprocessingOptionsView> where TOptions : class
  {
    private IMVCANController _selectedController;

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

    public SpectralPreprocessingControllerBase()
    {
      CmdRemoveTab = new RelayCommand(EhCmdRemoveTab);
      CmdMoveTabLeft = new RelayCommand(EhCmdMoveTabLeft);
      CmdMoveTabRight = new RelayCommand(EhCmdMoveTabRight);
    }

    #region Bindings

    public ICommand CmdRemoveTab { get; }

    public ICommand CmdMoveTabRight { get; }

    public ICommand CmdMoveTabLeft { get; }

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


    private bool _isCustomPipeline;

    public bool IsCustomPipeline
    {
      get => _isCustomPipeline;
      set
      {
        if (!(_isCustomPipeline == value))
        {
          ApplyCurrentController();
          if (value == false)
          {
            var newPre = SpectralPreprocessingOptions.TryCreateFrom(InternalPreprocessingOptions);
            if (newPre is null)
              return; // conversion is not possible
            else
              InternalPreprocessingOptions = newPre;
          }
          else
          {
            InternalPreprocessingOptions = SpectralPreprocessingOptionsList.CreateWithoutNoneElements(InternalPreprocessingOptions);
          }

          _isCustomPipeline = value;
          OnPropertyChanged(nameof(IsCustomPipeline));
          UpdateControllers();
        }
      }
    }

    private ItemsController<Type> _contentInsertLeft;

    public ItemsController<Type> ContentInsertLeft
    {
      get => _contentInsertLeft;
      set
      {
        if (!(_contentInsertLeft == value))
        {
          _contentInsertLeft = value;
          OnPropertyChanged(nameof(ContentInsertLeft));
        }
      }
    }

    private ItemsController<Type> _contentInsertRight;

    public ItemsController<Type> ContentInsertRight
    {
      get => _contentInsertRight;
      set
      {
        if (!(_contentInsertRight == value))
        {
          _contentInsertRight = value;
          OnPropertyChanged(nameof(ContentInsertRight));
        }
      }
    }

    #endregion

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        _isCustomPipeline = InternalPreprocessingOptions is not SpectralPreprocessingOptions;
        var controllers = new SelectableListNodeList();
        AddControllers(controllers);
        TabControllers = new ItemsController<IMVCANController>(controllers, EhSelectedTabChanged);
        _selectedController = (IMVCANController)controllers[0].Tag;
        TabControllers.SelectedValue = _selectedController;


        var types = new Type[]
        {
        typeof(ISanitizer),
        typeof(IDarkSubtraction),
        typeof(ISpikeRemoval),
        typeof(IYCalibration),
        typeof(IXCalibration),
        typeof(IResampling),
        typeof(ISmoothing),
        typeof(IBaselineEstimation),
        typeof(ICropping),
        typeof(INormalization),
      };

        var contLeft = new SelectableListNodeList();
        contLeft.Add(new SelectableListNode("↙↙", typeof(object), true));
        contLeft.AddRange(types.Select(t => new SelectableListNode(t.Name, t, false)));
        ContentInsertLeft = new ItemsController<Type>(contLeft, EhInsertContentLeft);
        ContentInsertLeft.SelectedItem = contLeft[0];

        var contRight = new SelectableListNodeList();
        contRight.Add(new SelectableListNode("↘↘", typeof(object), true));
        contRight.AddRange(types.Select(t => new SelectableListNode(t.Name, t, false)));
        ContentInsertRight = new ItemsController<Type>(contRight, EhInsertContentRight);
        ContentInsertRight.SelectedItem = contRight[0];

      }
    }

    protected void UpdateControllers()
    {
      _selectedController = null;
      TabControllers.Items.Clear();
      AddControllers(TabControllers.Items);
    }

    private void EhInsertContentLeft(Type newType)
    {
      ApplyCurrentController();
      EhInsertContent(newType, false);
      Current.Dispatcher.InvokeLaterAndForget(TimeSpan.FromMilliseconds(200), () =>
        ContentInsertLeft.SelectedValue = typeof(object)
        );
    }

    private void EhInsertContentRight(Type newType)
    {
      ApplyCurrentController();
      EhInsertContent(newType, true);
      Current.Dispatcher.InvokeLaterAndForget(TimeSpan.FromMilliseconds(200), () =>
        ContentInsertRight.SelectedValue = typeof(object)
        );
    }

    private void EhInsertContent(Type newType, bool toTheRight)
    {
      if (!typeof(ISingleSpectrumPreprocessor).IsAssignableFrom(newType))
        return;

      var availableTypes = Altaxo.Main.Services.ReflectionService.GetNonAbstractSubclassesOf(newType);

      if (availableTypes.Length == 0)
        return;

      var typeToCreate = availableTypes.FirstOrDefault(e => !e.Name.Contains("None"));
      if (typeToCreate == null) return;

      // create an instance
      var newInstance = (ISingleSpectrumPreprocessor)Activator.CreateInstance(typeToCreate);

      // find out where to insert this
      int idx = -1;
      if (TabControllers.SelectedItem is SelectableListNodeWithController sv)
      {
        idx = (int)sv.ControllerTag;
      }
      if (idx >= 0 && toTheRight)
        ++idx;

      if (idx == -1)
      {
        idx = InternalPreprocessingOptions.Count;
      }

      var list = InternalPreprocessingOptions.ToList();
      list.Insert(idx, newInstance);
      InternalPreprocessingOptions = new SpectralPreprocessingOptionsList(list);
      UpdateControllers();
    }

    private void EhCmdRemoveTab()
    {
      int idx = -1;
      if (TabControllers.SelectedItem is SelectableListNodeWithController sv)
      {
        idx = (int)sv.ControllerTag;
      }

      if (idx >= 0 && idx < InternalPreprocessingOptions.Count && InternalPreprocessingOptions is SpectralPreprocessingOptionsList)
      {
        var ele = InternalPreprocessingOptions.ToList();
        ele.RemoveAt(idx);
        InternalPreprocessingOptions = new SpectralPreprocessingOptionsList(ele);
        UpdateControllers();
      }
    }

    private void EhCmdMoveTabLeft() => EhMoveTab(false);
    private void EhCmdMoveTabRight() => EhMoveTab(true);

    private void EhMoveTab(bool toTheRight)
    {
      int idx = -1;
      if (TabControllers.SelectedItem is SelectableListNodeWithController sv)
      {
        idx = (int)sv.ControllerTag;
      }

      if (idx >= 0 && idx < InternalPreprocessingOptions.Count && InternalPreprocessingOptions is SpectralPreprocessingOptionsList)
      {
        if ((toTheRight && idx < InternalPreprocessingOptions.Count - 1) || (!toTheRight && idx >= 1))
        {
          ApplyCurrentController();
          var ele = InternalPreprocessingOptions.ToList();
          ListExtensions.MoveSelectedItems(ele, i => i == idx, toTheRight ? 1 : -1);
          InternalPreprocessingOptions = new SpectralPreprocessingOptionsList(ele);
          UpdateControllers();
        }
      }
    }

    protected abstract IEnumerable<(string Label, object Doc, Func<IMVCANController> GetController)> GetComponents();

    protected virtual void AddControllers(SelectableListNodeList controllers)
    {
      int idx = 0;
      foreach (var pair in GetComponents())
      {
        var controller = pair.GetController();
        controller.InitializeDocument(new object[] { pair.Doc });
        Current.Gui.FindAndAttachControlTo(controller);
        controllers.Add(new SelectableListNodeWithController(pair.Label, controller, false) { Controller = controller, ControllerTag = idx });
        ++idx;
      }
    }

    private void ApplyCurrentController()
    {
      if (_selectedController is not null)
      {
        _selectedController.Apply(false);
        var model = _selectedController.ModelObject;
        UpdateDoc(model, GetIndexOfController(_selectedController));
      }
    }

    private void EhSelectedTabChanged(IMVCANController controller)
    {
      ApplyCurrentController();

      _selectedController = controller;
    }

    private int GetIndexOfController(IMVCANController controller)
    {
      var element = TabControllers.Items.First(c => object.ReferenceEquals(c.Tag, controller));
      return (int)((SelectableListNodeWithController)element).ControllerTag;
    }


    protected abstract void UpdateDoc(object model, int index);

    protected abstract SpectralPreprocessingOptionsBase InternalPreprocessingOptions { get; set; }

    public override bool Apply(bool disposeController)
    {
      if (_selectedController is not null)
      {
        _selectedController.Apply(false);
        var model = _selectedController.ModelObject;
        UpdateDoc(model, GetIndexOfController(_selectedController));
      }

      return ApplyEnd(true, disposeController);
    }
  }
}
