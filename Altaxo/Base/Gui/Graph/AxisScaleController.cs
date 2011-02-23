#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2007 Dr. Dirk Lellinger
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
#endregion

using System;

using Altaxo.Graph.Scales;
using Altaxo.Graph.Scales.Ticks;
using Altaxo.Graph.Gdi;
using Altaxo.Gui;
using Altaxo.Collections;

namespace Altaxo.Gui.Graph
{
  #region Interfaces
	
	public interface IAxisScaleView 
	{
		void InitializeAxisType(SelectableListNodeList names);
		void InitializeTickSpacingType(SelectableListNodeList names);
		void InitializeLinkTargets(SelectableListNodeList names);
		bool ScaleIsLinked { get; set; }


		void SetBoundaryView(object guiobject);
		void SetScaleView(object guiobject);
		void SetTickSpacingView(object guiobject);

		event Action AxisTypeChanged;
		event Action TickSpacingTypeChanged;
		event Action LinkTargetChanged;

		/// <summary>Argument is true if the scale is linked, otherwise false.</summary>
		event Action<bool> LinkChanged;
	}

 

 
  #endregion

  /// <summary>
  /// Summary description for AxisScaleController.
  /// </summary>
	[ExpectedTypeOfView(typeof(IAxisScaleView))]
  public class AxisScaleController : IMVCAController
  {
    protected IAxisScaleView _view;
    protected XYPlotLayer _layer;
    protected int _axisNumber;
    
    // Cached values

		protected ScaleWithTicks _scaleWithTicks;

    protected Scale _originalScale;

    protected Scale _tempScale;
		protected TickSpacing _tempTickSpacing;


    protected IMVCAController _boundaryController;

		protected SelectableListNodeList _scaleTypes;
		protected IMVCAController _scaleController;

		protected SelectableListNodeList _tickSpacingTypes;
		protected IMVCAController _tickSpacingController;

		protected SelectableListNodeList _linkScaleNumbers;
		protected int _linkScaleNumber;

		protected LinkedScaleParameters _linkedScaleParameters;
		protected IMVCAController _linkedScaleParameterController;

		bool _isScaleLinked;

    public AxisScaleController(XYPlotLayer layer, int axisnumber)
    {
      _layer = layer;
      _axisNumber = axisnumber;
      _originalScale = _layer.Scales[axisnumber].Scale;
			_scaleWithTicks = _layer.Scales[axisnumber];
			if (_originalScale is LinkedScale)
			{
				_isScaleLinked = true;
				_linkScaleNumber = (_originalScale as LinkedScale).LinkedScaleIndex;
				_tempScale = (Scale)(_originalScale as LinkedScale).WrappedScale.Clone();
				_linkedScaleParameters = (LinkedScaleParameters)(_originalScale as LinkedScale).LinkParameters.Clone();
			}
			else
			{
				_isScaleLinked = false;
				_linkScaleNumber = _axisNumber;
				_tempScale = (Scale)_originalScale.Clone();
				_linkedScaleParameters = new LinkedScaleParameters();
			}

			_tempTickSpacing = (TickSpacing)_layer.Scales[axisnumber].TickSpacing.Clone();


      Initialize(true);
    }

		public AxisScaleController(ScaleWithTicks scaleWithTicks)
		{
			_layer = null;
			_axisNumber = 0;
			_scaleWithTicks = scaleWithTicks;
			_originalScale = scaleWithTicks.Scale;
			if (_originalScale is LinkedScale)
			{
				_isScaleLinked = true;
				_linkScaleNumber = (_originalScale as LinkedScale).LinkedScaleIndex;
				_tempScale = (Scale)(_originalScale as LinkedScale).WrappedScale.Clone();
				_linkedScaleParameters = (LinkedScaleParameters)(_originalScale as LinkedScale).LinkParameters.Clone();
			}
			else
			{
				_isScaleLinked = false;
				_linkScaleNumber = _axisNumber;
				_tempScale = (Scale)_originalScale.Clone();
				_linkedScaleParameters = new LinkedScaleParameters();
			}

			_tempTickSpacing = (TickSpacing)scaleWithTicks.TickSpacing.Clone();


			Initialize(true);
		}

		public void Initialize(bool initData)
		{
			InitLinkProperties(initData);
			InitScaleTypes(initData);
			InitScaleController(initData);
			InitBoundaryController(initData);
			InitTickSpacingTypes(initData);
			InitTickSpacingController(initData);
		}

	 

		void InitLinkProperties(bool bInit)
		{
			if (bInit)
			{
				_linkScaleNumbers = new SelectableListNodeList();
				if (null!=_layer && null!=_layer.LinkedLayer)
				{
					for (int i = 0; i < _layer.LinkedLayer.Scales.Count; i++)
					{
						string name = i.ToString();
						_linkScaleNumbers.Add(new SelectableListNode(name, i, i == _linkScaleNumber));
					}
				}

				_linkedScaleParameterController = (IMVCAController)Current.Gui.GetControllerAndControl(new object[] { _linkedScaleParameters }, typeof(IMVCAController));
			}

			if (_view != null)
			{
				_view.ScaleIsLinked = _isScaleLinked;
				_view.InitializeLinkTargets(_linkScaleNumbers);
			}
		}

    public void InitScaleTypes(bool bInit)
    {
			if (bInit)
			{
				_scaleTypes = new SelectableListNodeList();
				Type[] classes = Altaxo.Main.Services.ReflectionService.GetNonAbstractSubclassesOf(typeof(Scale));
				for (int i = 0; i < classes.Length; i++)
				{
					if (classes[i] == typeof(LinkedScale))
						continue;
					SelectableListNode node = new SelectableListNode(Current.Gui.GetUserFriendlyClassName(classes[i]), classes[i], _tempScale.GetType() == classes[i]);
					_scaleTypes.Add(node);
				}
			}

      if(null!=_view)
        _view.InitializeAxisType(_scaleTypes);
    }


		public void InitTickSpacingTypes(bool bInit)
		{
			if (bInit)
			{
				_tickSpacingTypes = new SelectableListNodeList();
				Type[] classes = Altaxo.Main.Services.ReflectionService.GetNonAbstractSubclassesOf(typeof(TickSpacing));
				for (int i = 0; i < classes.Length; i++)
				{
					SelectableListNode node = new SelectableListNode(Current.Gui.GetUserFriendlyClassName(classes[i]), classes[i], _tempTickSpacing.GetType() == classes[i]);
					_tickSpacingTypes.Add(node);
				}
			}

			if (null != _view)
				_view.InitializeTickSpacingType(_tickSpacingTypes);
		}

    public void InitScaleController(bool bInit)
    {
      if (bInit)
      {
        object scaleObject = _tempScale;
        _scaleController = (IMVCAController)Current.Gui.GetControllerAndControl(new object[] { scaleObject }, typeof(IMVCAController));
      }
      if (null != _view)
      {
        _view.SetScaleView(null==_scaleController ? null : _scaleController.ViewObject);
      }
    }

    public void InitBoundaryController(bool bInit)
    {
      if(bInit)
      {
        object rescalingObject = _tempScale.RescalingObject;
        if (rescalingObject != null)
          _boundaryController = (IMVCAController)Current.Gui.GetControllerAndControl(new object[] { rescalingObject, _tempScale }, typeof(IMVCAController));
        else
          _boundaryController = null;
      }
      if(null!=_view)
      {
				if (_isScaleLinked)
					_view.SetBoundaryView(_linkedScaleParameterController.ViewObject);
				else
					_view.SetBoundaryView(null!=_boundaryController ? _boundaryController.ViewObject : null);
      }
    }

		public void InitTickSpacingController(bool bInit)
		{
			if (bInit)
			{
			if(_tempTickSpacing!=null)
					_tickSpacingController = (IMVCAController)Current.Gui.GetControllerAndControl(new object[] { _tempTickSpacing }, typeof(IMVCAController));
				else
					_tickSpacingController = null;
			}
			if (null != _view)
			{
				_view.SetTickSpacingView(null != _tickSpacingController ? _tickSpacingController.ViewObject : null);
			}
		}


		#region View event handlers

		public void EhView_AxisTypeChanged()
		{
			Type axistype = (Type)_scaleTypes.FirstSelectedNode.Item;

			try
			{
				if (axistype != _tempScale.GetType())
				{
					// replace the current axis by a new axis of the type axistype
					Scale _oldAxis = _tempScale;
					_tempScale = (Scale)System.Activator.CreateInstance(axistype);

					// Try to set the same org and end as the axis before
					// this will fail for instance if we switch from linear to logarithmic with negative bounds
					try
					{
						_tempScale.SetScaleOrgEnd(_oldAxis.OrgAsVariant, _oldAxis.EndAsVariant);
					}
					catch (Exception)
					{
					}

					InitScaleController(true);
					// now we have also to replace the controller and the control for the axis boundaries
					InitBoundaryController(true);

					_tempTickSpacing = ScaleWithTicks.CreateDefaultTicks(axistype);
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

			Type spaceType = (Type)_tickSpacingTypes.FirstSelectedNode.Item;

			if (spaceType == _tempTickSpacing.GetType())
				return;

			_tempTickSpacing = (TickSpacing)Activator.CreateInstance(spaceType);
			InitTickSpacingController(true);
		}


		public void EhView_LinkTargetChanged()
		{
			_linkScaleNumber = (int)_linkScaleNumbers.FirstSelectedNode.Item;
		}

		public void EhView_LinkChanged(bool linked)
		{
		}

		#endregion


		#region IMVCAController

		public object ViewObject
		{
			get { return _view; }
			set
			{
				if (null != _view)
				{
					_view.AxisTypeChanged -= this.EhView_AxisTypeChanged;
					_view.TickSpacingTypeChanged -= this.EhView_TickSpacingTypeChanged;
					_view.LinkTargetChanged -= this.EhView_LinkTargetChanged;
					_view.LinkChanged -= this.EhView_LinkChanged;

				}

				_view = value as IAxisScaleView;

				if (null != _view)
				{
					_view.AxisTypeChanged += this.EhView_AxisTypeChanged;
					_view.TickSpacingTypeChanged += this.EhView_TickSpacingTypeChanged;
					_view.LinkTargetChanged += this.EhView_LinkTargetChanged;
					_view.LinkChanged += this.EhView_LinkChanged;


					Initialize(false);
				}
			}
		}

		public object ModelObject
		{
			get { return this._originalScale; }
		}

    public bool Apply()
    {


			if (null != _scaleController)
			{
				if (false == _scaleController.Apply())
					return false;
			}

			if (_view.ScaleIsLinked)
			{
				// Apply link conditions
				if (false == _linkedScaleParameterController.Apply())
					return false;
			}
			else // scale is not linked
			{
				if (null != _boundaryController)
				{
					if (false == _boundaryController.Apply())
						return false;
				}
			}

			if (null!=_tickSpacingController && false == _tickSpacingController.Apply())
				return false;

			// wrap the scale if it is linked
			if (_view.ScaleIsLinked)
			{
				LinkedScale ls = new LinkedScale(_tempScale, _layer.LinkedLayer!=null ? _layer.LinkedLayer.Scales[_linkScaleNumber].Scale : null, _linkScaleNumber);
				ls.LinkParameters.SetTo(_linkedScaleParameters);
				//_layer.Scales.SetScaleWithTicks(this._axisNumber,ls, _tempTickSpacing);
				_scaleWithTicks.SetTo(ls, _tempTickSpacing);
			}
			else
			{
				//_layer.Scales.SetScaleWithTicks(this._axisNumber, _tempScale, _tempTickSpacing);
				_scaleWithTicks.SetTo(_tempScale, _tempTickSpacing);
			}
      
      return true; // all ok
		}

		#endregion

	}

}
