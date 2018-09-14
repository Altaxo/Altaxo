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

using System;
using Altaxo.Collections;
using Altaxo.Graph.Gdi;
using Altaxo.Graph.Scales;
using Altaxo.Graph.Scales.Ticks;

namespace Altaxo.Gui.Graph.Scales
{
  #region Interfaces

  public interface IScaleWithTicksView
  {
    void InitializeAxisType(SelectableListNodeList names);

    void InitializeTickSpacingType(SelectableListNodeList names);

    void InitializeLinkTargets(SelectableListNodeList names);

    void SetLinkedScalePropertiesView(object guiobject);

    bool LinkScaleType { get; set; }

    bool LinkTickSpacing { get; set; }

    void SetVisibilityOfLinkElements(bool showLinkTargets, bool showOtherLinkProperties);

    void SetRescalingView(object guiobject);

    void SetScaleView(object guiobject);

    void SetTickSpacingView(object guiobject);

    event Action ScaleTypeChanged;

    event Action TickSpacingTypeChanged;

    event Action LinkTargetChanged;
  }

  #endregion Interfaces

  /// <summary>
  /// Summary description for AxisScaleController.
  /// </summary>
  [ExpectedTypeOfView(typeof(IScaleWithTicksView))]
  public class ScaleWithTicksController : MVCANControllerEditOriginalDocInstanceCanChangeBase<Scale, IScaleWithTicksView>
  {
    protected IMVCAController _scaleController;

    protected IMVCAController _linkedScaleParameterController;

    protected IMVCAController _rescalingController;

    protected IMVCAController _tickSpacingController;

    protected SelectableListNodeList _scaleTypes;

    protected SelectableListNodeList _tickSpacingTypes;

    protected SelectableListNodeList _linkScaleChoices;

    /// <summary>
    /// If true, the scale type can not be changed.
    /// </summary>
    protected bool _lockScaleType;

    protected bool _hideLinkTargets;

    public override System.Collections.Generic.IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield return new ControllerAndSetNullMethod(_scaleController, () => _scaleController = null);
      yield return new ControllerAndSetNullMethod(_rescalingController, () => _rescalingController = null);
      yield return new ControllerAndSetNullMethod(_tickSpacingController, () => _tickSpacingController = null);
      yield return new ControllerAndSetNullMethod(_linkedScaleParameterController, () => _linkedScaleParameterController = null);
    }

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
      : base(SetNewScaleInstance != null ? SetNewScaleInstance : (scale => { }))
    {
      // if providing a null value for the SetNewScaleInstance action, we lock the possibility to choose a new scale type
      if (null == SetNewScaleInstance)
        _lockScaleType = true;

      _hideLinkTargets = hideLinkTargets;
    }

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (_doc is LinkedScale)
        _hideLinkTargets = false;

      FixScaleIfIsLinkedScaleWithInvalidTarget(initData);
      InitLinkTargetScales(initData);
      InitLinkProperties(initData);
      InitScaleTypes(initData);
      InitScaleController(initData);
      InitRescalingController(initData);
      InitTickSpacingTypes(initData);
      InitTickSpacingController(initData);
    }

    public override bool Apply(bool disposeController)
    {
      if (null != _scaleController && false == _scaleController.Apply(disposeController))
        return false;

      if (null != _linkedScaleParameterController && false == _linkedScaleParameterController.Apply(disposeController))
        return false;

      if (null != _rescalingController && false == _rescalingController.Apply(disposeController))
        return false;

      if (null != _tickSpacingController && false == _tickSpacingController.Apply(disposeController))
        return false;

      var lscale = _doc as LinkedScale;
      if (null != lscale)
      {
        lscale.LinkScaleType = _view.LinkScaleType;
        lscale.LinkTickSpacing = _view.LinkTickSpacing;
      }

      return ApplyEnd(true, disposeController); // all ok
    }

    protected override void AttachView()
    {
      base.AttachView();

      _view.ScaleTypeChanged += EhView_ScaleTypeChanged;
      _view.TickSpacingTypeChanged += EhView_TickSpacingTypeChanged;
      _view.LinkTargetChanged += EhView_LinkTargetChanged;
    }

    protected override void DetachView()
    {
      _view.ScaleTypeChanged -= EhView_ScaleTypeChanged;
      _view.TickSpacingTypeChanged -= EhView_TickSpacingTypeChanged;
      _view.LinkTargetChanged -= EhView_LinkTargetChanged;

      base.DetachView();
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
        if (null == value)
          throw new ArgumentNullException("ScaleToEdit");

        if (_doc is LinkedScale)
          ((LinkedScale)_doc).WrappedScale = value;
        else
          _doc = value;
      }
    }

    protected Scale ScaleLinkedTo
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

        if (null != value)
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
        if (_doc is LinkedScale && ((LinkedScale)_doc).ScaleLinkedTo == null)
        {
          var oldScale = _doc;
          _doc = (Scale)((LinkedScale)_doc).WrappedScale.Clone(); // this will replace the invalid linked scale with a clone of the wrapped scale
          OnDocumentInstanceChanged(oldScale, _doc);
        }
      }
    }

    private void InitLinkTargetScales(bool initData)
    {
      if (initData)
      {
        _linkScaleChoices = new SelectableListNodeList
        {
          new SelectableListNode("None", null, false)
        };

        // find the parent layer
        var mylayer = Altaxo.Main.AbsoluteDocumentPath.GetRootNodeImplementing<HostLayer>(_doc);
        if (null != mylayer)
        {
          var siblingLayers = mylayer.SiblingLayers;
          if (null != siblingLayers)
          {
            var scaleLinkedTo = _doc is LinkedScale ? ((LinkedScale)_doc).ScaleLinkedTo : null;

            for (int i = 0; i < siblingLayers.Count; ++i)
            {
              var lxy = siblingLayers[i] as XYPlotLayer;
              if (null == lxy)
                continue;

              for (int j = 0; j < lxy.Scales.Count; ++j)
              {
                var scale = lxy.Scales[j];

                if (LinkedScale.WouldScaleBeDependentOnMe(_doc, scale))
                  continue; // Scale would be dependent on _doc, thus we can not link to it

                var scaleName = j == 0 ? "x" : (j == 1 ? "y" : (j == 2 ? "z" : string.Format("{0}.", j)));
                string name = string.Format("Layer[{0}] - {1} scale", i, scaleName);
                _linkScaleChoices.Add(new SelectableListNode(name, scale, object.ReferenceEquals(scale, scaleLinkedTo)));
              }
            }
          }
        }

        if (null == _linkScaleChoices.FirstSelectedNode)
          _linkScaleChoices[0].IsSelected = true;
      }

      if (_view != null)
      {
        _view.InitializeLinkTargets(_linkScaleChoices);
      }
    }

    public void InitLinkProperties(bool initData)
    {
      if (initData)
      {
        if (_doc is LinkedScale)
        {
          var linkedScaleParameters = ((LinkedScale)_doc).LinkParameters;
          _linkedScaleParameterController = (IMVCAController)Current.Gui.GetControllerAndControl(new object[] { linkedScaleParameters }, typeof(IMVCAController), UseDocument.Directly);
        }
        else
        {
          DisposeAndSetToNull(ref _linkedScaleParameterController);
        }
      }

      if (null != _view)
      {
        if (null != _linkedScaleParameterController)
          _view.SetLinkedScalePropertiesView(_linkedScaleParameterController.ViewObject);
        else
          _view.SetLinkedScalePropertiesView(null);

        // other link properties
        var lscale = _doc as LinkedScale;
        if (null != lscale)
        {
          _view.LinkScaleType = lscale.LinkScaleType;
          _view.LinkTickSpacing = lscale.LinkTickSpacing;
        }
        _view.SetVisibilityOfLinkElements(!_hideLinkTargets, _doc is LinkedScale);
      }
    }

    public void InitScaleTypes(bool initData)
    {
      if (initData)
      {
        _scaleTypes = new SelectableListNodeList();
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
          _scaleTypes.Add(node);
        }
      }

      if (null != _view)
        _view.InitializeAxisType(_scaleTypes);
    }

    public void InitTickSpacingTypes(bool initData)
    {
      if (initData)
      {
        _tickSpacingTypes = new SelectableListNodeList();
        Type[] classes = Altaxo.Main.Services.ReflectionService.GetNonAbstractSubclassesOf(typeof(TickSpacing));
        for (int i = 0; i < classes.Length; i++)
        {
          var node = new SelectableListNode(Current.Gui.GetUserFriendlyClassName(classes[i]), classes[i], _doc.TickSpacing.GetType() == classes[i]);
          _tickSpacingTypes.Add(node);
        }
      }

      if (null != _view)
        _view.InitializeTickSpacingType(_tickSpacingTypes);
    }

    public void InitScaleController(bool initData)
    {
      if (initData)
      {
        _scaleController = (IMVCAController)Current.Gui.GetControllerAndControl(new object[] { ScaleToEdit }, typeof(IMVCAController), UseDocument.Directly);
      }
      if (null != _view)
      {
        _view.SetScaleView(null == _scaleController ? null : _scaleController.ViewObject);
      }
    }

    public void InitRescalingController(bool bInit)
    {
      if (bInit)
      {
        if (null != ScaleToEdit.RescalingObject && null == ScaleLinkedTo)
          _rescalingController = (IMVCAController)Current.Gui.GetControllerAndControl(new object[] { ScaleToEdit.RescalingObject, ScaleToEdit }, typeof(IMVCAController), UseDocument.Directly);
        else
          DisposeAndSetToNull(ref _rescalingController);
      }

      if (null != _view)
      {
        _view.SetRescalingView(null != _rescalingController ? _rescalingController.ViewObject : null);
      }
    }

    public void InitTickSpacingController(bool bInit)
    {
      if (bInit)
      {
        if (_doc.TickSpacing != null)
          _tickSpacingController = (IMVCAController)Current.Gui.GetControllerAndControl(new object[] { _doc.TickSpacing }, typeof(IMVCAController), UseDocument.Directly);
        else
          DisposeAndSetToNull(ref _tickSpacingController);
      }
      if (null != _view)
      {
        _view.SetTickSpacingView(null != _tickSpacingController ? _tickSpacingController.ViewObject : null);
      }
    }

    #region View event handlers

    public void EhView_ScaleTypeChanged()
    {
      var scaleType = (Type)_scaleTypes.FirstSelectedNode.Tag;

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

          InitScaleController(true);
          // now we have also to replace the controller and the control for the axis boundaries
          InitRescalingController(true);
          InitTickSpacingTypes(true);
          InitTickSpacingController(true);
        }
      }
      catch (Exception)
      {
      }
    }

    public void EhView_TickSpacingTypeChanged()
    {
      var selNode = _tickSpacingTypes.FirstSelectedNode; // FirstSelectedNode can be null when the content of the box changes
      if (null == selNode)
        return;

      var spaceType = (Type)_tickSpacingTypes.FirstSelectedNode.Tag;

      if (spaceType == _doc.TickSpacing.GetType())
        return;

      _doc.TickSpacing = (TickSpacing)Activator.CreateInstance(spaceType);
      InitTickSpacingController(true);
    }

    public void EhView_LinkTargetChanged()
    {
      ScaleLinkedTo = (Scale)_linkScaleChoices.FirstSelectedNode.Tag;

      InitLinkProperties(true);
      InitRescalingController(true);
    }

    public void EhView_LinkChanged(bool linked)
    {
    }

    #endregion View event handlers
  }
}
