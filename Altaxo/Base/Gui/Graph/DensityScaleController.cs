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

	public interface IDensityScaleView
	{
		void InitializeAxisType(SelectableListNodeList names);
		void SetBoundaryView(object guiobject);
		void SetScaleView(object guiobject);

		event Action AxisTypeChanged;
	}

	


	#endregion

	/// <summary>
	/// Lets the user choose a numerical scale.
	/// </summary>
	[ExpectedTypeOfView(typeof(IDensityScaleView))]
	// [UserControllerForObject(typeof(NumericalScale),101)] // outcommented since this causes an infinite loop when searching for detailed scale controllers
	public class DensityScaleController : IMVCANController
	{
		protected IDensityScaleView _view;

		// Cached values
		protected Scale _originalScale;

		protected Scale _tempScale;


		protected IMVCAController _boundaryController;

		protected SelectableListNodeList _scaleTypes;
		protected IMVCAController _scaleController;

		public bool InitializeDocument(params object[] args)
		{
			if (args.Length == 0 || !(args[0] is Scale))
				return false;

			_originalScale = (Scale)args[0];
			_tempScale = (Scale)_originalScale.Clone();
			Initialize(true);
			return true;
		}

		public UseDocument UseDocumentCopy
		{
			set { }
		}


		public void Initialize(bool initData)
		{
			InitScaleTypes(initData);
			InitScaleController(initData);
			InitBoundaryController(initData);
		}

		public void InitScaleTypes(bool bInit)
		{
			if (bInit)
			{
				_scaleTypes = new SelectableListNodeList();
				Type[] classes = Altaxo.Main.Services.ReflectionService.GetNonAbstractSubclassesOf(typeof(NumericalScale));
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
				_view.SetBoundaryView(null != _boundaryController ? _boundaryController.ViewObject : null);
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
				}
			}
			catch (Exception)
			{
			}
		}

	


		#endregion


		#region IMVCAController

		public object ViewObject
		{
			get { return _view; }
			set
			{
				if (null != _view)
					_view.AxisTypeChanged -= EhView_AxisTypeChanged;

				_view = value as IDensityScaleView;

				if (null != _view)
				{
					Initialize(false);
					_view.AxisTypeChanged += EhView_AxisTypeChanged;
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

		
				if (null != _boundaryController)
				{
					if (false == _boundaryController.Apply())
						return false;
				}

				_originalScale = _tempScale;

			return true; // all ok
		}

		#endregion

	

}

}
