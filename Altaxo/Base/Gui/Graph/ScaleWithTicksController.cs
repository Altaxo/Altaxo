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

using Altaxo.Collections;
using Altaxo.Graph.Gdi;
using Altaxo.Graph.Scales;
using Altaxo.Graph.Scales.Ticks;
using Altaxo.Gui;
using System;

namespace Altaxo.Gui.Graph
{
	#region Interfaces

	public interface IScaleWithTicksView
	{
		void InitializeAxisType(SelectableListNodeList names);

		void InitializeTickSpacingType(SelectableListNodeList names);

		void InitializeLinkTargets(SelectableListNodeList names);

		void SetBoundaryView(object guiobject);

		void SetScaleView(object guiobject);

		void SetTickSpacingView(object guiobject);

		event Action AxisTypeChanged;

		event Action TickSpacingTypeChanged;

		event Action LinkTargetChanged;
	}

	#endregion Interfaces

	/// <summary>
	/// Summary description for AxisScaleController.
	/// </summary>
	[ExpectedTypeOfView(typeof(IScaleWithTicksView))]
	public class ScaleWithTicksController : MVCANControllerBase<ScaleWithTicks, IScaleWithTicksView>
	{
		// Cached values
		protected Scale _tempScale;

		protected TickSpacing _tempTickSpacing;

		protected IMVCAController _boundaryController;

		protected SelectableListNodeList _scaleTypes;
		protected IMVCAController _scaleController;

		protected SelectableListNodeList _tickSpacingTypes;
		protected IMVCAController _tickSpacingController;

		protected SelectableListNodeList _linkScaleChoices;
		protected Scale _scaleLinkedTo;

		protected LinkedScaleParameters _linkedScaleParameters;
		protected IMVCAController _linkedScaleParameterController;

		protected override void Initialize(bool initData)
		{
			if (initData)
			{
				if (_originalDoc.Scale is LinkedScale)
				{
					_scaleLinkedTo = (_originalDoc.Scale as LinkedScale).ScaleLinkedTo;
					_tempScale = (Scale)(_originalDoc.Scale as LinkedScale).WrappedScale.Clone();
					_linkedScaleParameters = (LinkedScaleParameters)(_originalDoc.Scale as LinkedScale).LinkParameters.Clone();
				}
				else
				{
					_scaleLinkedTo = null;
					_tempScale = (Scale)_originalDoc.Scale.Clone();
					_linkedScaleParameters = new LinkedScaleParameters();
				}

				_tempTickSpacing = (TickSpacing)_originalDoc.TickSpacing.Clone();
			}

			InitLinkProperties(initData);
			InitScaleTypes(initData);
			InitScaleController(initData);
			InitBoundaryController(initData);
			InitTickSpacingTypes(initData);
			InitTickSpacingController(initData);
		}

		private void InitLinkProperties(bool bInit)
		{
			if (bInit)
			{
				_linkScaleChoices = new SelectableListNodeList();
				_linkScaleChoices.Add(new SelectableListNode("None", null, false));

				// find the parent layer
				var mylayer = Altaxo.Main.AbsoluteDocumentPath.GetRootNodeImplementing<HostLayer>(_originalDoc);
				if (null != mylayer)
				{
					var parentLayerList = mylayer.ParentLayerList;
					if (null != parentLayerList)
					{
						var scaleLinkedTo = _doc.Scale is LinkedScale ? ((LinkedScale)_doc.Scale).ScaleLinkedTo : null;

						for (int i = 0; i < parentLayerList.Count; ++i)
						{
							var l = parentLayerList[i];
							if (l.LayerNumber == mylayer.LayerNumber)
								continue;
							var lxy = l as XYPlotLayer; // must have scales
							if (null == lxy)
								return;

							for (int j = 0; j < lxy.Scales.Count; ++j)
							{
								var scale = lxy.Scales[j].Scale;
								var scaleName = j == 0 ? "x" : (j == 1 ? "y" : (j == 2 ? "z" : string.Format("{0}.", j)));

								string name = string.Format("Layer[{0}] - {1} scale", i, scaleName);
								_linkScaleChoices.Add(new SelectableListNode(name, scale, object.ReferenceEquals(scale, scaleLinkedTo)));
							}
						}
					}
				}

				if (null == _linkScaleChoices.FirstSelectedNode)
					_linkScaleChoices[0].IsSelected = true;

				_linkedScaleParameterController = (IMVCAController)Current.Gui.GetControllerAndControl(new object[] { _linkedScaleParameters }, typeof(IMVCAController), UseDocument.Directly);
			}

			if (_view != null)
			{
				_view.InitializeLinkTargets(_linkScaleChoices);
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

			if (null != _view)
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
				_view.SetScaleView(null == _scaleController ? null : _scaleController.ViewObject);
			}
		}

		public void InitBoundaryController(bool bInit)
		{
			if (bInit)
			{
				object rescalingObject = _tempScale.RescalingObject;
				if (rescalingObject != null)
					_boundaryController = (IMVCAController)Current.Gui.GetControllerAndControl(new object[] { rescalingObject, _tempScale }, typeof(IMVCAController));
				else
					_boundaryController = null;
			}
			if (null != _view)
			{
				if (null != _scaleLinkedTo)
					_view.SetBoundaryView(_linkedScaleParameterController.ViewObject);
				else
					_view.SetBoundaryView(null != _boundaryController ? _boundaryController.ViewObject : null);
			}
		}

		public void InitTickSpacingController(bool bInit)
		{
			if (bInit)
			{
				if (_tempTickSpacing != null)
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
			Type axistype = (Type)_scaleTypes.FirstSelectedNode.Tag;

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

			Type spaceType = (Type)_tickSpacingTypes.FirstSelectedNode.Tag;

			if (spaceType == _tempTickSpacing.GetType())
				return;

			_tempTickSpacing = (TickSpacing)Activator.CreateInstance(spaceType);
			InitTickSpacingController(true);
		}

		public void EhView_LinkTargetChanged()
		{
			_scaleLinkedTo = (Scale)_linkScaleChoices.FirstSelectedNode.Tag;

			InitBoundaryController(false);
		}

		public void EhView_LinkChanged(bool linked)
		{
		}

		#endregion View event handlers

		#region IMVCAController

		protected override void AttachView()
		{
			base.AttachView();

			_view.AxisTypeChanged += this.EhView_AxisTypeChanged;
			_view.TickSpacingTypeChanged += this.EhView_TickSpacingTypeChanged;
			_view.LinkTargetChanged += this.EhView_LinkTargetChanged;
		}

		protected override void DetachView()
		{
			_view.AxisTypeChanged -= this.EhView_AxisTypeChanged;
			_view.TickSpacingTypeChanged -= this.EhView_TickSpacingTypeChanged;
			_view.LinkTargetChanged -= this.EhView_LinkTargetChanged;

			base.DetachView();
		}

		public override bool Apply()
		{
			if (null != _scaleController)
			{
				if (false == _scaleController.Apply())
					return false;
			}

			if (_scaleLinkedTo != null)
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

			if (null != _tickSpacingController && false == _tickSpacingController.Apply())
				return false;

			// wrap the scale if it is linked
			if (_scaleLinkedTo != null)
			{
				var layerLinkedTo = (XYPlotLayer)Altaxo.Main.AbsoluteDocumentPath.GetRootNodeImplementing(_scaleLinkedTo, typeof(XYPlotLayer));
				int scaleNumberLinkedTo = layerLinkedTo.Scales.IndexOfFirst(scaleWithTicks => object.ReferenceEquals(scaleWithTicks.Scale, _scaleLinkedTo));
				LinkedScale ls = new LinkedScale(_tempScale);
				ls.LinkParameters.SetTo(_linkedScaleParameters);
				//_layer.Scales.SetScaleWithTicks(this._axisNumber,ls, _tempTickSpacing);
				_doc.SetTo(ls, _tempTickSpacing);
				ls.ScaleLinkedTo = _scaleLinkedTo;
			}
			else
			{
				//_layer.Scales.SetScaleWithTicks(this._axisNumber, _tempScale, _tempTickSpacing);
				_doc.SetTo(_tempScale, _tempTickSpacing);
			}

			return true; // all ok
		}

		#endregion IMVCAController
	}
}