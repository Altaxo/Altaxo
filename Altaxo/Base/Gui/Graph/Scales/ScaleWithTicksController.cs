#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
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
using Altaxo.Collections;
using Altaxo.Graph.Gdi;
using Altaxo.Graph.Scales;
using Altaxo.Graph.Scales.Ticks;
using Altaxo.Gui.Common;

namespace Altaxo.Gui.Graph.Scales
{
  /// <summary>
  /// Provides the view contract for <see cref="ScaleWithTicksController"/>.
  /// </summary>
  public interface IScaleWithTicksView : IDataContextAwareView
  {
  }

  /// <summary>
  /// Controller for editing a scale together with its tick spacing and rescaling.
  /// </summary>
  [ExpectedTypeOfView(typeof(IScaleWithTicksView))]
  public class ScaleWithTicksController : MVCANControllerEditOriginalDocInstanceCanChangeBase<Scale, IScaleWithTicksView>
  {


    /// <summary>
    /// If true, the scale type can not be changed.
    /// </summary>
    protected bool _lockScaleType;

    /// <inheritdoc />
    public override System.Collections.Generic.IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield return new ControllerAndSetNullMethod(_scaleController, () => _scaleController = null);
      yield return new ControllerAndSetNullMethod(_rescalingController, () => _rescalingController = null);
      yield return new ControllerAndSetNullMethod(_tickSpacingController, () => _tickSpacingController = null);
      yield return new ControllerAndSetNullMethod(_linkedScaleParameterController, () => _linkedScaleParameterController = null);
    }

    #region Bindings

    private ItemsController<Type> _scaleTypes;

    /// <summary>
    /// Gets or sets the selectable scale types.
    /// </summary>
    public ItemsController<Type> ScaleTypes
    {
      get => _scaleTypes;
      set
      {
        if (!(_scaleTypes == value))
        {
          _scaleTypes = value;
          OnPropertyChanged(nameof(ScaleTypes));
        }
      }
    }



    private ItemsController<Type> _tickSpacingTypes;

    /// <summary>
    /// Gets or sets the selectable tick-spacing types.
    /// </summary>
    public ItemsController<Type> TickSpacingTypes
    {
      get => _tickSpacingTypes;
      set
      {
        if (!(_tickSpacingTypes == value))
        {
          _tickSpacingTypes = value;
          OnPropertyChanged(nameof(TickSpacingTypes));
        }
      }
    }


    private ItemsController<Scale?> _linkScaleChoices;

    /// <summary>
    /// Gets or sets the selectable target scales for linking.
    /// </summary>
    public ItemsController<Scale?> LinkScaleChoices
    {
      get => _linkScaleChoices;
      set
      {
        if (!(_linkScaleChoices == value))
        {
          _linkScaleChoices = value;
          OnPropertyChanged(nameof(LinkScaleChoices));
        }
      }
    }

    private bool _LinkScaleType;

    /// <summary>
    /// Gets or sets a value indicating whether the scale type is linked to the target scale.
    /// </summary>
    public bool LinkScaleType
    {
      get => _LinkScaleType;
      set
      {
        if (!(_LinkScaleType == value))
        {
          _LinkScaleType = value;
          OnPropertyChanged(nameof(LinkScaleType));
        }
      }
    }

    private bool _LinkTicksStraight;

    /// <summary>
    /// Gets or sets a value indicating whether the tick spacing is linked to the target scale.
    /// </summary>
    public bool LinkTicksStraight
    {
      get => _LinkTicksStraight;
      set
      {
        if (!(_LinkTicksStraight == value))
        {
          _LinkTicksStraight = value;
          OnPropertyChanged(nameof(LinkTicksStraight));
        }
      }
    }

    private IMVCAController _scaleController;

    /// <summary>
    /// Gets or sets the controller that edits the scale-specific parameters.
    /// </summary>
    public IMVCAController ScaleController
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


    private IMVCAController _linkedScaleParameterController;

    /// <summary>
    /// Gets or sets the controller for parameters of a linked scale.
    /// </summary>
    public IMVCAController LinkedScaleParameterController
    {
      get => _linkedScaleParameterController;
      set
      {
        if (!(_linkedScaleParameterController == value))
        {
          _linkedScaleParameterController?.Dispose();
          _linkedScaleParameterController = value;
          OnPropertyChanged(nameof(LinkedScaleParameterController));
        }
      }
    }


    private IMVCAController _rescalingController;

    /// <summary>
    /// Gets or sets the controller that edits the rescaling settings.
    /// </summary>
    public IMVCAController RescalingController
    {
      get => _rescalingController;
      set
      {
        if (!(_rescalingController == value))
        {
          _rescalingController?.Dispose();
          _rescalingController = value;
          OnPropertyChanged(nameof(RescalingController));
        }
      }
    }


    private IMVCAController _tickSpacingController;

    /// <summary>
    /// Gets or sets the controller that edits the tick-spacing settings.
    /// </summary>
    public IMVCAController TickSpacingController
    {
      get => _tickSpacingController;
      set
      {
        if (!(_tickSpacingController == value))
        {
          _tickSpacingController?.Dispose();
          _tickSpacingController = value;
          OnPropertyChanged(nameof(TickSpacingController));
        }
      }
    }

    private bool _showLinkTargets;

    /// <summary>
    /// Gets or sets a value indicating whether the link targets are shown.
    /// </summary>
    public bool ShowLinkTargets
    {
      get => _showLinkTargets;
      set
      {
        if (!(_showLinkTargets == value))
        {
          _showLinkTargets = value;
          OnPropertyChanged(nameof(ShowLinkTargets));
        }
      }
    }

    private bool _showOtherLinkProperties;

    /// <summary>
    /// Gets or sets a value indicating whether additional link properties are shown.
    /// </summary>
    public bool ShowOtherLinkProperties
    {
      get => _showOtherLinkProperties;
      set
      {
        if (!(_showOtherLinkProperties == value))
        {
          _showOtherLinkProperties = value;
          OnPropertyChanged(nameof(ShowOtherLinkProperties));
        }
      }
    }

    #endregion

    /// <inheritdoc />
    public override void Dispose(bool isDisposing)
    {
      _scaleTypes = null;
      _tickSpacingTypes = null;
      _linkScaleChoices = null;

      base.Dispose(isDisposing);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ScaleWithTicksController"/> class.
    /// </summary>
    /// <param name="SetNewScaleInstance">Procedure that sets the scale in the parent instance (in case the scale type is changed by the user so that a new scale instance must be instantiated).</param>
    /// <param name="hideLinkTargets">If set to <c>true</c>, the link targets will be hidden, so that the user can not setup a linked scale.</param>
    public ScaleWithTicksController(Action<Scale> SetNewScaleInstance, bool hideLinkTargets)
      : base(SetNewScaleInstance is not null ? SetNewScaleInstance : (scale => { }))
    {
      // if providing a null value for the SetNewScaleInstance action, we lock the possibility to choose a new scale type
      if (SetNewScaleInstance is null)
        _lockScaleType = true;

      _showLinkTargets = !hideLinkTargets;
    }

    /// <inheritdoc />
    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        if (_doc is LinkedScale)
          ShowLinkTargets = true;

        FixScaleIfIsLinkedScaleWithInvalidTarget(initData);
        InitLinkTargetScales();
        InitLinkProperties(initData);
        InitScaleTypes();
        InitScaleController();
        InitRescalingController();
        InitTickSpacingTypes();
        InitTickSpacingController();
      }
    }

    /// <inheritdoc />
    public override bool Apply(bool disposeController)
    {
      if (_scaleController is not null && false == _scaleController.Apply(disposeController))
        return ApplyEnd(false, disposeController);

      if (_linkedScaleParameterController is not null && false == _linkedScaleParameterController.Apply(disposeController))
        return ApplyEnd(false, disposeController);

      if (_rescalingController is not null && false == _rescalingController.Apply(disposeController))
        return ApplyEnd(false, disposeController);

      if (_tickSpacingController is not null && false == _tickSpacingController.Apply(disposeController))
        return ApplyEnd(false, disposeController);

      var lscale = _doc as LinkedScale;
      if (lscale is not null)
      {
        lscale.LinkScaleType = LinkScaleType;
        lscale.LinkTickSpacing = LinkTicksStraight;
      }

      return ApplyEnd(true, disposeController); // all ok
    }

    /// <summary>
    /// Gets or sets the scale to edit. This is the scale that is shown in the dialog. If _doc is a LinkedScale, the ScaleToEdit is the scale wrapped by the LinkedScale.
    /// Otherwise it is _doc.
    /// </summary>
    /// <value>
    /// The scale to edit.
    /// </value>
    /// <exception cref="System.ArgumentNullException">ScaleToEdit</exception>
    protected Scale ScaleToEdit
    {
      get
      {
        if (_doc is LinkedScale)
          return ((LinkedScale)_doc).WrappedScale;
        else
          return _doc;
      }
      set
      {
        if (value is null)
          throw new ArgumentNullException("ScaleToEdit");

        if (_doc is LinkedScale)
          ((LinkedScale)_doc).WrappedScale = value;
        else
          _doc = value;
      }
    }

    /// <summary>
    /// Gets or sets the scale to which the edited scale is linked.
    /// </summary>
    protected Scale? ScaleLinkedTo
    {
      get
      {
        if (_doc is LinkedScale)
          return ((LinkedScale)_doc).ScaleLinkedTo;
        else
          return null;
      }
      set
      {
        var oldValue = ScaleLinkedTo;
        if (object.ReferenceEquals(oldValue, value))
          return;

        if (value is not null)
        {
          if (!(_doc is LinkedScale))
          {
            var oldDoc = _doc;
            _doc = new LinkedScale((Scale)_doc.Clone()); // Clone necessary to avoid ObjectDisposedProblems
            OnDocumentInstanceChanged(oldDoc, _doc);
          }

          ((LinkedScale)_doc).ScaleLinkedTo = value;
        }
        else // value is null
        {
          if (_doc is LinkedScale)
          {
            var oldDoc = _doc;
            _doc = (Scale)((LinkedScale)_doc).WrappedScale.Clone(); // Clone to avoid ObjectDisposedProblems
            OnDocumentInstanceChanged(oldDoc, _doc);
          }
        }
      }
    }

    private void FixScaleIfIsLinkedScaleWithInvalidTarget(bool initData)
    {
      if (initData)
      {
        if (_doc is LinkedScale && ((LinkedScale)_doc).ScaleLinkedTo is null)
        {
          var oldScale = _doc;
          _doc = (Scale)((LinkedScale)_doc).WrappedScale.Clone(); // this will replace the invalid linked scale with a clone of the wrapped scale
          OnDocumentInstanceChanged(oldScale, _doc);
        }
      }
    }

    private void InitLinkTargetScales()
    {
        var linkScaleChoices = new SelectableListNodeList
        {
          new SelectableListNode("None", null, false)
        };

        // find the parent layer
        var mylayer = Altaxo.Main.AbsoluteDocumentPath.GetRootNodeImplementing<HostLayer>(_doc);
        if (mylayer is not null)
        {
          var siblingLayers = mylayer.SiblingLayers;
          if (siblingLayers is not null)
          {
            var scaleLinkedTo = _doc is LinkedScale ? ((LinkedScale)_doc).ScaleLinkedTo : null;

            for (int i = 0; i < siblingLayers.Count; ++i)
            {
              var lxy = siblingLayers[i] as XYPlotLayer;
              if (lxy is null)
                continue;

              for (int j = 0; j < lxy.Scales.Count; ++j)
              {
                var scale = lxy.Scales[j];

                if (LinkedScale.WouldScaleBeDependentOnMe(_doc, scale))
                  continue; // Scale would be dependent on _doc, thus we can not link to it

                var scaleName = j == 0 ? "x" : (j == 1 ? "y" : (j == 2 ? "z" : string.Format("{0}.", j)));
                string name = string.Format("Layer[{0}] - {1} scale", i, scaleName);
                linkScaleChoices.Add(new SelectableListNode(name, scale, object.ReferenceEquals(scale, scaleLinkedTo)));
              }
            }
          }
        }
      LinkScaleChoices = new ItemsController<Scale?>(linkScaleChoices, EhView_LinkTargetChanged);
    }

    /// <summary>
    /// Initializes the properties that are only relevant for linked scales.
    /// </summary>
    /// <param name="initData">If set to <c>true</c>, initialization is performed for freshly loaded data.</param>
    public void InitLinkProperties(bool initData)
    {
        if (_doc is LinkedScale)
        {
          var linkedScaleParameters = ((LinkedScale)_doc).LinkParameters;
          LinkedScaleParameterController = (IMVCAController)Current.Gui.GetControllerAndControl(new object[] { linkedScaleParameters }, typeof(IMVCAController), UseDocument.Directly);
        }
        else
        {
          LinkedScaleParameterController = null;
        }

        // other link properties
        var lscale = _doc as LinkedScale;
        if (lscale is not null)
        {
          LinkScaleType = lscale.LinkScaleType;
          LinkTicksStraight = lscale.LinkTickSpacing;
        }
        ShowOtherLinkProperties = _doc is LinkedScale;
    }

    /// <summary>
    /// Initializes the list of selectable scale types.
    /// </summary>
    public void InitScaleTypes()
    {
      var scaleTypes = new SelectableListNodeList();
      Type[] classes;

      if (_lockScaleType) // if the scale type is locked (i.e.) can not be chosen by the user, we simply offer only the one scale type we have now
        classes = new Type[] { _doc.GetType() };
      else
        classes = Altaxo.Main.Services.ReflectionService.GetNonAbstractSubclassesOf(typeof(Scale));

      for (int i = 0; i < classes.Length; i++)
      {
        if (classes[i] == typeof(LinkedScale))
          continue;
        var node = new SelectableListNode(Current.Gui.GetUserFriendlyClassName(classes[i]), classes[i], ScaleToEdit.GetType() == classes[i]);
        scaleTypes.Add(node);
      }

      ScaleTypes = new ItemsController<Type>(scaleTypes, EhView_ScaleTypeChanged);
    }

    /// <summary>
    /// Initializes the list of selectable tick-spacing types.
    /// </summary>
    public void InitTickSpacingTypes()
    {
        var tickSpacingTypes = new SelectableListNodeList();
        Type[] classes = Altaxo.Main.Services.ReflectionService.GetNonAbstractSubclassesOf(typeof(TickSpacing));
        for (int i = 0; i < classes.Length; i++)
        {
          var node = new SelectableListNode(Current.Gui.GetUserFriendlyClassName(classes[i]), classes[i], _doc.TickSpacing.GetType() == classes[i]);
          tickSpacingTypes.Add(node);
        }
      TickSpacingTypes = new ItemsController<Type>(tickSpacingTypes, EhView_TickSpacingTypeChanged);
    }

    /// <summary>
    /// Initializes the controller that edits the scale-specific settings.
    /// </summary>
    public void InitScaleController()
    {
        ScaleController = (IMVCAController)Current.Gui.GetControllerAndControl(new object[] { ScaleToEdit }, typeof(IMVCAController), UseDocument.Directly);
    }

    /// <summary>
    /// Initializes the controller that edits the rescaling settings.
    /// </summary>
    public void InitRescalingController()
    {
      if (ScaleToEdit.RescalingObject is not null && ScaleLinkedTo is null)
        RescalingController = (IMVCAController)Current.Gui.GetControllerAndControl(new object[] { ScaleToEdit.RescalingObject, ScaleToEdit }, typeof(IMVCAController), UseDocument.Directly);
      else
        RescalingController = null;
    }

    /// <summary>
    /// Initializes the controller that edits the tick-spacing settings.
    /// </summary>
    public void InitTickSpacingController()
    {
      if (_doc.TickSpacing is not null)
        TickSpacingController = (IMVCAController)Current.Gui.GetControllerAndControl(new object[] { _doc.TickSpacing }, typeof(IMVCAController), UseDocument.Directly);
      else
        TickSpacingController = null;
    }

    #region View event handlers

    /// <summary>
    /// Handles changes to the selected scale type.
    /// </summary>
    /// <param name="scaleType">The newly selected scale type.</param>
    public void EhView_ScaleTypeChanged(Type scaleType)
    {
      try
      {
        if (scaleType != ScaleToEdit.GetType())
        {
          // replace the current scale by a new scale of the type axistype
          Scale oldScale = ScaleToEdit;
          var newScale = (Scale)System.Activator.CreateInstance(scaleType);

          OnDocumentInstanceChanged(oldScale, newScale);

          ScaleToEdit = newScale;

          // Try to set the same org and end as the axis before
          // this will fail for instance if we switch from linear to logarithmic with negative bounds
          try
          {
            if (ScaleToEdit.RescalingObject is Altaxo.Main.ICopyFrom)
              ScaleToEdit.RescalingObject.CopyFrom(oldScale.RescalingObject);
          }
          catch (Exception)
          {
          }

          InitScaleController();
          // now we have also to replace the controller and the control for the axis boundaries
          InitRescalingController();
          InitTickSpacingTypes();
          InitTickSpacingController();
        }
      }
      catch (Exception)
      {
      }
    }

    /// <summary>
    /// Handles changes to the selected tick-spacing type.
    /// </summary>
    /// <param name="spaceType">The newly selected tick-spacing type.</param>
    public void EhView_TickSpacingTypeChanged(Type spaceType)
    {
      if (spaceType is null)
        return;

      if (spaceType == _doc.TickSpacing.GetType())
        return;

      _doc.TickSpacing = (TickSpacing)Activator.CreateInstance(spaceType);
      InitTickSpacingController();
    }

    /// <summary>
    /// Handles changes to the selected link target.
    /// </summary>
    /// <param name="selectedScale">The selected target scale, or <see langword="null"/> to remove the link.</param>
    public void EhView_LinkTargetChanged(Scale? selectedScale)
    {
      ScaleLinkedTo = selectedScale;
      InitLinkProperties(true);
      InitRescalingController();
    }

    
    #endregion View event handlers
  }
}
