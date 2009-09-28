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
	
	public interface IAxisScaleView : IMVCView
	{

		IAxisScaleController Controller { get; set; }

		void InitializeAxisType(SelectableListNodeList names);
		void InitializeTickSpacingType(SelectableListNodeList names);
		void InitializeLinkTargets(SelectableListNodeList names);
		bool ScaleIsLinked { get; set; }


		void SetBoundaryView(object guiobject);
		void SetScaleView(object guiobject);
		void SetTickSpacingView(object guiobject);
	}

  public interface IAxisScaleController : IMVCAController
  {
    void EhView_AxisTypeChanged();
		void EhView_TickSpacingTypeChanged();
		void EhView_LinkTargetChanged();
		void EhView_LinkChanged(bool isLinked);
  }

 
  #endregion

  /// <summary>
  /// Summary description for AxisScaleController.
  /// </summary>
	[ExpectedTypeOfView(typeof(IAxisScaleView))]
  public class AxisScaleController : IAxisScaleController
  {
    protected IAxisScaleView _view;
    protected XYPlotLayer _layer;
    protected int _axisNumber;
    
    // Cached values
    protected Scale _originalScale;

    protected Scale _tempScale;
		protected TickSpacing _tempTickSpacing;


    protected IMVCAController m_BoundaryController;

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
				if (_layer.LinkedLayer != null)
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

      if(null!=View)
        View.InitializeAxisType(_scaleTypes);
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

			if (null != View)
				View.InitializeTickSpacingType(_tickSpacingTypes);
		}

    public void InitScaleController(bool bInit)
    {
      if (bInit)
      {
        object scaleObject = _tempScale;
        _scaleController = (IMVCAController)Current.Gui.GetControllerAndControl(new object[] { scaleObject }, typeof(IMVCAController));
      }
      if (null != View)
      {
        View.SetScaleView(null==_scaleController ? null : _scaleController.ViewObject);
      }
    }

    public void InitBoundaryController(bool bInit)
    {
      if(bInit)
      {
        object rescalingObject = _tempScale.RescalingObject;
        if (rescalingObject != null)
          m_BoundaryController = (IMVCAController)Current.Gui.GetControllerAndControl(new object[] { rescalingObject, _tempScale }, typeof(IMVCAController));
        else
          m_BoundaryController = null;
      }
      if(null!=View)
      {
				if (_isScaleLinked)
					View.SetBoundaryView(_linkedScaleParameterController.ViewObject);
				else
					View.SetBoundaryView(null!=m_BoundaryController ? m_BoundaryController.ViewObject : null);
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
			if (null != View)
			{
				View.SetTickSpacingView(null != _tickSpacingController ? _tickSpacingController.ViewObject : null);
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

		public IAxisScaleView View
		{
			get { return _view; }
			set
			{
				if (null != _view)
					_view.Controller = null;

				_view = value;

				if (null != _view)
				{
					_view.Controller = this;
					Initialize(false);
				}
			}
		}

		public object ViewObject
		{
			get { return View; }
			set { View = value as IAxisScaleView; }
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
				if (null != m_BoundaryController)
				{
					if (false == m_BoundaryController.Apply())
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
				_layer.Scales.SetScaleWithTicks(this._axisNumber,ls, _tempTickSpacing);
			}
			else
			{
				_layer.Scales.SetScaleWithTicks(this._axisNumber, _tempScale, _tempTickSpacing);
			}
      
      return true; // all ok
		}

		#endregion

	}

}
