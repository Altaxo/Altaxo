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
using System.Collections.Generic;
using System.ComponentModel;

using Altaxo.Graph;
using Altaxo.Graph.Gdi;
using Altaxo.Graph.Gdi.Axis;
using Altaxo.Graph.Gdi.Plot;
using Altaxo.Graph.Gdi.Plot.Data;
using Altaxo.Graph.Gdi.Plot.Styles;
using Altaxo.Graph.Gdi.Shapes;
using Altaxo.Gui;
using Altaxo.Gui.Common;
using Altaxo.Collections;

namespace Altaxo.Gui.Graph
{
	/// <summary>
	/// Summary description for LayerController.
	/// </summary>
	[UserControllerForObject(typeof(DensityImageLegend))]
	[ExpectedTypeOfView(typeof(IXYPlotLayerView))]
	public class DensityImageLegendController : MVCANControllerBase<DensityImageLegend, IXYPlotLayerView>
	{
		private string _currentPageName;
		LayerControllerTabType _primaryChoice; // which tab type is currently choosen
		private int _currentScale; // which scale is choosen 0==X-AxisScale, 1==Y-AxisScale

		private CSLineID _currentAxisID; // which style is currently choosen
		private CSPlaneID _currentPlaneID; // which plane is currently chosen for the grid


		IMVCAController _currentController;

		protected IMVCAController _axisScaleController;
		protected IMVCAController _coordinateController;

		Dictionary<CSLineID, AxisStyleControllerConditionalGlue> _axisControl;

		SelectableListNodeList _listOfScales;
		SelectableListNodeList _listOfAxes;
		SelectableListNodeList _listOfPlanes;
		SelectableListNodeList _listOfUniqueItem;

		object _lastControllerApplied;

		public DensityImageLegendController()
		{
			_currentScale = 0;
			_currentPageName = "Scale";
			_currentAxisID = CSLineID.X0;
		}

		public override bool InitializeDocument(params object[] args)
		{
			return base.InitializeDocument(args);
		}


		public DensityImageLegendController(DensityImageLegend layer)
			: this(layer, "Scale", 1, CSLineID.X0)
		{
		}
		public DensityImageLegendController(DensityImageLegend layer, string currentPage, CSLineID id)
			: this(layer, currentPage, id.ParallelAxisNumber, id)
		{
		}


		DensityImageLegendController(DensityImageLegend layer, string currentPage, int axisScaleIdx, CSLineID id)
		{
			_originalDoc = layer;
			_doc = (DensityImageLegend)layer.Clone();

			_currentScale = axisScaleIdx;
			_currentAxisID = id;
			_currentPageName = currentPage;

			Initialize(true);
		}

		protected override void AttachView()
		{
			_view.TabValidating += EhView_TabValidating;
			_view.PageChanged += EhView_PageChanged;
			_view.SecondChoiceChanged += EhView_SecondChoiceChanged;
			_view.CreateOrMoveAxis += EhView_CreateOrMoveAxis;
		}

		protected override void DetachView()
		{
			_view.TabValidating -= EhView_TabValidating;
			_view.PageChanged -= EhView_PageChanged;
			_view.SecondChoiceChanged -= EhView_SecondChoiceChanged;
			_view.CreateOrMoveAxis -= EhView_CreateOrMoveAxis;
		}



		public override bool Apply()
		{
			ApplyCurrentController(false);

			_originalDoc.CopyFrom(_doc); // _doc remains suspended

			return true;
		}


		protected override void Initialize(bool initData)
		{
			if (initData)
			{
				SetCoordinateSystemDependentObjects(_currentAxisID);

				_listOfUniqueItem = new SelectableListNodeList();
				_listOfUniqueItem.Add(new SelectableListNode("Common", null, true));

			}

			if (null != _view)
			{
				// add all necessary Tabs
				_view.AddTab("Scale", "Scale");
				_view.AddTab("CS", "Coord.System");
				// _view.AddTab("Contents", "Contents");
				// _view.AddTab("Position", "Position");
				_view.AddTab("TitleAndFormat", "Title&&Format");
				_view.AddTab("MajorLabels", "Major labels");
				_view.AddTab("MinorLabels", "Minor labels");
				//_view.AddTab("GridStyle", "Grid style");

				// Set the controller of the current visible Tab
				SetCurrentTabController(true);
			}
		}


		void SetCoordinateSystemDependentObjects()
		{
			SetCoordinateSystemDependentObjects(null);
		}
		void SetCoordinateSystemDependentObjects(CSLineID id)
		{
			// Scales
			_listOfScales = new SelectableListNodeList();
			_listOfScales.Add(new SelectableListNode("Z-Scale", 0, false));

			// Axes
			// collect the AxisStyleIdentifier from the actual layer and also all possible AxisStyleIdentifier
			_axisControl = new Dictionary<CSLineID, AxisStyleControllerConditionalGlue>();
			_listOfAxes = new SelectableListNodeList();
			foreach (CSLineID ids in _doc.CoordinateSystem.GetJoinedAxisStyleIdentifier(_doc.AxisStyles.AxisStyleIDs, new CSLineID[] { id }))
			{
				CSAxisInformation info = _doc.CoordinateSystem.GetAxisStyleInformation(ids);
				var axisInfo = new AxisStyleControllerConditionalGlue(info, _doc.AxisStyles);
				_axisControl.Add(info.Identifier, axisInfo);
				_listOfAxes.Add(new SelectableListNode(info.NameOfAxisStyle, info.Identifier, false));
			}

			// Planes
			_listOfPlanes = new SelectableListNodeList();
			_currentPlaneID = CSPlaneID.Front;
			_listOfPlanes.Add(new SelectableListNode("Front", _currentPlaneID, true));
		}






		void SetCurrentTabController(bool pageChanged)
		{
			switch (_currentPageName)
			{
				case "CS":
					if (pageChanged)
					{
						_view.SelectTab(_currentPageName);
						SetSecondaryChoiceToUnique();
					}
					if (null == this._coordinateController)
					{
						this._coordinateController = (IMVCAController)Current.Gui.GetControllerAndControl(new object[] { _doc.CoordinateSystem }, typeof(IMVCAController));
					}
					_currentController = this._coordinateController;
					_view.CurrentContent = this._coordinateController.ViewObject;
					break;


				case "Scale":
					if (pageChanged)
					{
						_view.SelectTab(_currentPageName);
						SetSecondaryChoiceToScales();
					}
					if (_axisScaleController == null)
					{
						_axisScaleController = new AxisScaleController(_doc.ScaleWithTicks);
						Current.Gui.FindAndAttachControlTo(_axisScaleController);
					}
					_currentController = _axisScaleController;
					_view.CurrentContent = _currentController.ViewObject;
					break;

				case "TitleAndFormat":
					if (pageChanged)
					{
						_view.SelectTab(_currentPageName);
						SetSecondaryChoiceToAxes();
					}

					_view.CurrentContent = _axisControl[_currentAxisID].AxisStyleCondView;
					_currentController = _axisControl[_currentAxisID].AxisStyleCondController;

					break;
				case "MajorLabels":
					if (pageChanged)
					{
						_view.SelectTab(_currentPageName);
						SetSecondaryChoiceToAxes();
					}

					_view.CurrentContent = _axisControl[_currentAxisID].MajorLabelCondView;
					_currentController = _axisControl[_currentAxisID].MajorLabelCondController;


					break;
				case "MinorLabels":
					if (pageChanged)
					{
						_view.SelectTab(_currentPageName);
						SetSecondaryChoiceToAxes();
					}

					_view.CurrentContent = _axisControl[_currentAxisID].MinorLabelCondView;
					_currentController = _axisControl[_currentAxisID].MinorLabelCondController;
					break;
			}
		}


		void SetSecondaryChoiceToUnique()
		{
			this._primaryChoice = LayerControllerTabType.Unique;
			_view.InitializeSecondaryChoice(_listOfUniqueItem, this._primaryChoice);
		}

		void SetSecondaryChoiceToScales()
		{
			_listOfScales.ClearSelectionsAll();
			_listOfScales[_currentScale].IsSelected = true;

			this._primaryChoice = LayerControllerTabType.Scales;
			_view.InitializeSecondaryChoice(_listOfScales, this._primaryChoice);
		}


		void SetSecondaryChoiceToAxes()
		{
			foreach (var item in _listOfAxes)
				item.IsSelected = ((CSLineID)item.Tag) == _currentAxisID;

			this._primaryChoice = LayerControllerTabType.Axes;
			_view.InitializeSecondaryChoice(_listOfAxes, this._primaryChoice);
		}


		void SetSecondaryChoiceToPlanes()
		{
			this._primaryChoice = LayerControllerTabType.Planes;
			_view.InitializeSecondaryChoice(_listOfPlanes, this._primaryChoice);
		}

		public void EhView_PageChanged(string firstChoice)
		{
			ApplyCurrentController(false);

			_currentPageName = firstChoice;
			SetCurrentTabController(true);
		}

		public void EhView_SecondChoiceChanged()
		{
			if (!ApplyCurrentController(false))
				return;

			if (_primaryChoice == LayerControllerTabType.Scales)
			{
				_currentScale = (int)_listOfScales.FirstSelectedNode.Tag;
			}
			else if (_primaryChoice == LayerControllerTabType.Axes)
			{
				_currentAxisID = (CSLineID)_listOfAxes.FirstSelectedNode.Tag;
			}
			else if (_primaryChoice == LayerControllerTabType.Planes)
			{
				_currentPlaneID = (CSPlaneID)_listOfPlanes.FirstSelectedNode.Tag;
			}

			SetCurrentTabController(false);
		}

		void EhView_TabValidating(object sender, CancelEventArgs e)
		{
			if (!ApplyCurrentController(true))
				e.Cancel = true;
		}

		public void EhView_CreateOrMoveAxis(bool moveAxis)
		{
			if (!ApplyCurrentController(false))
				return;

			var creationArgs = new AxisCreationArguments();

			creationArgs.InitializeAxisInformationList(_doc.CoordinateSystem, _doc.AxisStyles);
			creationArgs.TemplateStyle = _currentAxisID;
			creationArgs.MoveAxis = moveAxis;

			if (!Current.Gui.ShowDialog(ref creationArgs, "Create/move axis", false))
				return;

			if (_axisControl.ContainsKey(creationArgs.CurrentStyle))
				return; // the axis is already present

			var oldIdentity = creationArgs.TemplateStyle;
			var newIdentity = creationArgs.CurrentStyle;
			var newAxisInfo = _doc.CoordinateSystem.GetAxisStyleInformation(newIdentity);

			AxisCreationArguments.AddAxis(_doc.AxisStyles, creationArgs); // add the new axis to the document
			_axisControl.Add(newIdentity, new AxisStyleControllerConditionalGlue(newAxisInfo, _doc.AxisStyles));

			SetSecondaryChoiceToUnique();

			_listOfAxes.ClearSelectionsAll();
			_listOfAxes.Add(new SelectableListNode(newAxisInfo.NameOfAxisStyle, newIdentity, true));

			if (creationArgs.MoveAxis && _axisControl.ContainsKey(oldIdentity))
			{
				_axisControl.Remove(oldIdentity);
				for (int i = _listOfAxes.Count - 1; i >= 0; --i)
				{
					if (((CSLineID)_listOfAxes[i].Tag) == oldIdentity)
					{
						_listOfAxes.RemoveAt(i);
						break;
					}
				}
			}


			_currentAxisID = newIdentity;
			SetSecondaryChoiceToAxes();
			SetCurrentTabController(false);
		}

		bool ApplyCurrentController(bool force)
		{
			if (_currentController == null)
				return true;

			if (!force && object.ReferenceEquals(_currentController, _lastControllerApplied))
				return true;

			if (!_currentController.Apply())
				return false;
			_lastControllerApplied = _currentController;

			if (object.ReferenceEquals(_currentController, _coordinateController))
			{
				//_doc.CoordinateSystem = (G2DCoordinateSystem)_coordinateController.ModelObject;
				SetCoordinateSystemDependentObjects();
			}



			return true;
		}

		#region Dialog

		public static bool ShowDialog(DensityImageLegend layer)
		{
			return ShowDialog(layer, "Scale", new CSLineID(0, 0));
		}
		public static bool ShowDialog(DensityImageLegend layer, string currentPage)
		{
			return ShowDialog(layer, currentPage, new CSLineID(0, 0));
		}
		public static bool ShowDialog(DensityImageLegend layer, string currentPage, CSLineID currentEdge)
		{
			DensityImageLegendController ctrl = new DensityImageLegendController(layer, currentPage, currentEdge);
			return Current.Gui.ShowDialog(ctrl, layer.Name, true);
		}


		#endregion

		#region Edit Handlers

		public static void RegisterEditHandlers()
		{
			// register here editor methods

			XYPlotLayer.LayerPositionEditorMethod = new DoubleClickHandler(EhLayerPositionEdit);

		}

		public static bool EhLayerPositionEdit(IHitTestObject hit)
		{
			DensityImageLegend layer = hit.HittedObject as DensityImageLegend;
			if (layer == null)
				return false;

			ShowDialog(layer, "Position");

			return false;
		}


		#endregion


	}
}
