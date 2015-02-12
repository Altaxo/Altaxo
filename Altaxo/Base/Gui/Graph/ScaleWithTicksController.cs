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
using System;

namespace Altaxo.Gui.Graph
{
	#region Interfaces

	public interface IScaleWithTicksView
	{
		void InitializeAxisType(SelectableListNodeList names);

		void InitializeTickSpacingType(SelectableListNodeList names);

		void InitializeLinkTargets(SelectableListNodeList names);

		void SetLinkedScalePropertiesView(object guiobject);

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
	public class ScaleWithTicksController : MVCANControllerEditOriginalDocBase<Scale, IScaleWithTicksView>
	{
		protected IMVCAController _scaleController;

		protected IMVCAController _linkedScaleParameterController;

		protected IMVCAController _boundaryController;

		protected IMVCAController _tickSpacingController;

		protected SelectableListNodeList _scaleTypes;

		protected SelectableListNodeList _tickSpacingTypes;

		protected SelectableListNodeList _linkScaleChoices;

		public override System.Collections.Generic.IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
		{
			yield return new ControllerAndSetNullMethod(_scaleController, () => _scaleController = null);
			yield return new ControllerAndSetNullMethod(_boundaryController, () => _boundaryController = null);
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

		protected override void Initialize(bool initData)
		{
			base.Initialize(initData);

			FixScaleIfIsLinkedScaleWithInvalidTarget(initData);
			InitLinkTargetScales(initData);
			InitLinkProperties(initData);
			InitScaleTypes(initData);
			InitScaleController(initData);
			InitBoundaryController(initData);
			InitTickSpacingTypes(initData);
			InitTickSpacingController(initData);
		}

		public override bool Apply(bool disposeController)
		{
			if (null != _scaleController && false == _scaleController.Apply(disposeController))
				return false;

			if (null != _linkedScaleParameterController && false == _linkedScaleParameterController.Apply(disposeController))
				return false;

			if (null != _boundaryController && false == _boundaryController.Apply(disposeController))
				return false;

			if (null != _tickSpacingController && false == _tickSpacingController.Apply(disposeController))
				return false;

			return ApplyEnd(true, disposeController); // all ok
		}

		/// <summary>
		/// Gets or sets the scale to edit. This is the scale that is shown in the dialog. If _doc.Scale is a LinkedScale, the ScaleToEdit is the scale wrapped by the LinkedScale.
		/// Otherwise it is _doc.Scale.
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
						_doc = new LinkedScale((Scale)_doc.Clone()); // Clone necessary to avoid ObjectDisposedProblems

					((LinkedScale)_doc).ScaleLinkedTo = value;
				}
				else // value is null
				{
					if (_doc is LinkedScale)
						_doc = (Scale)((LinkedScale)_doc).WrappedScale.Clone(); // Clone to avoid ObjectDisposedProblems
				}
			}
		}

		private void FixScaleIfIsLinkedScaleWithInvalidTarget(bool initData)
		{
			if (initData)
			{
				if (_doc is LinkedScale && ((LinkedScale)_doc).ScaleLinkedTo == null)
				{
					_doc = (Scale)((LinkedScale)_doc).WrappedScale.Clone(); // this will replace the invalid linked scale with a clone of the wrapped scale
				}
			}
		}

		private void InitLinkTargetScales(bool initData)
		{
			if (initData)
			{
				_linkScaleChoices = new SelectableListNodeList();
				_linkScaleChoices.Add(new SelectableListNode("None", null, false));

				// find the parent layer
				var mylayer = Altaxo.Main.AbsoluteDocumentPath.GetRootNodeImplementing<HostLayer>(_doc);
				if (null != mylayer)
				{
					var siblingLayers = mylayer.SiblingLayers;
					if (null != siblingLayers)
					{
						var scaleLinkedTo = _doc.Scale is LinkedScale ? ((LinkedScale)_doc).ScaleLinkedTo : null;

						for (int i = 0; i < siblingLayers.Count; ++i)
						{
							var lxy = siblingLayers[i] as XYPlotLayer;
							if (null == lxy)
								continue;

							for (int j = 0; j < lxy.Scales.Count; ++j)
							{
								var scale = lxy.Scales[j];

								if (LinkedScale.WouldScaleBeDependentOnMe(_doc, scale))
									continue; // Scale would be dependent on _doc.Scale, thus we can not link to it

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
			}
		}

		public void InitScaleTypes(bool initData)
		{
			if (initData)
			{
				_scaleTypes = new SelectableListNodeList();
				Type[] classes = Altaxo.Main.Services.ReflectionService.GetNonAbstractSubclassesOf(typeof(Scale));
				for (int i = 0; i < classes.Length; i++)
				{
					if (classes[i] == typeof(LinkedScale))
						continue;
					SelectableListNode node = new SelectableListNode(Current.Gui.GetUserFriendlyClassName(classes[i]), classes[i], ScaleToEdit.GetType() == classes[i]);
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
					SelectableListNode node = new SelectableListNode(Current.Gui.GetUserFriendlyClassName(classes[i]), classes[i], _doc.TickSpacing.GetType() == classes[i]);
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

		public void InitBoundaryController(bool bInit)
		{
			if (bInit)
			{
				if (null != ScaleToEdit.RescalingObject && null == ScaleLinkedTo)
					_boundaryController = (IMVCAController)Current.Gui.GetControllerAndControl(new object[] { ScaleToEdit.RescalingObject, ScaleToEdit }, typeof(IMVCAController), UseDocument.Directly);
				else
					DisposeAndSetToNull(ref _boundaryController);
			}

			if (null != _view)
			{
				_view.SetBoundaryView(null != _boundaryController ? _boundaryController.ViewObject : null);
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

		public void EhView_AxisTypeChanged()
		{
			Type axistype = (Type)_scaleTypes.FirstSelectedNode.Tag;

			try
			{
				if (axistype != ScaleToEdit.GetType())
				{
					// replace the current axis by a new axis of the type axistype
					Scale _oldAxis = ScaleToEdit;
					ScaleToEdit = (Scale)System.Activator.CreateInstance(axistype);

					// Try to set the same org and end as the axis before
					// this will fail for instance if we switch from linear to logarithmic with negative bounds
					try
					{
						ScaleToEdit.SetScaleOrgEnd(_oldAxis.OrgAsVariant, _oldAxis.EndAsVariant);
					}
					catch (Exception)
					{
					}

					InitScaleController(true);
					// now we have also to replace the controller and the control for the axis boundaries
					InitBoundaryController(true);

					_doc.TickSpacing = Scale.CreateDefaultTicks(axistype);
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

			if (spaceType == _doc.TickSpacing.GetType())
				return;

			_doc.TickSpacing = (TickSpacing)Activator.CreateInstance(spaceType);
			InitTickSpacingController(true);
		}

		public void EhView_LinkTargetChanged()
		{
			ScaleLinkedTo = (Scale)_linkScaleChoices.FirstSelectedNode.Tag;

			InitLinkProperties(true);
			InitBoundaryController(true);
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

		#endregion IMVCAController
	}
}