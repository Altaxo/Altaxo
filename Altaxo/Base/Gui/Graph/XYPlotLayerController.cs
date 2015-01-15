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
using Altaxo.Graph;
using Altaxo.Graph.Gdi;
using Altaxo.Graph.Gdi.Axis;
using Altaxo.Graph.Gdi.Plot;
using Altaxo.Graph.Gdi.Plot.Data;
using Altaxo.Graph.Gdi.Plot.Styles;
using Altaxo.Gui;
using Altaxo.Gui.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Altaxo.Gui.Graph
{
	#region Interfaces

	public interface IXYPlotLayerView
	{
		void AddTab(string name, string text);

		object CurrentContent { get; set; }

		void SelectTab(string name);

		void InitializeSecondaryChoice(SelectableListNodeList items, LayerControllerTabType primaryChoice);

		event CancelEventHandler TabValidating;

		event Action<bool> CreateOrMoveAxis;

		event Action SecondChoiceChanged;

		event Action<string> PageChanged;
	}

	/// <summary>Designates, which type of tab is choosen in the layer view. Dependent on this choise a list of secondary choices will be shown.</summary>
	public enum LayerControllerTabType
	{
		/// <summary>No secondary choice available (i.e. for layer position, layer background).</summary>
		Unique,

		/// <summary>List of scales as secondary choice (e.g. x-scale, y-scale).</summary>
		Scales,

		/// <summary>List of axes as secondary choice (e.g. left, top, bottom, right, x=0 etc.)</summary>
		Axes,

		/// <summary>List of planes as secondary choice (e.g. front, back etc.).</summary>
		Planes
	};

	#endregion Interfaces

	/// <summary>
	/// Summary description for LayerController.
	/// </summary>
	[UserControllerForObject(typeof(XYPlotLayer))]
	[ExpectedTypeOfView(typeof(IXYPlotLayerView))]
	public class XYPlotLayerController : MVCANControllerBase<XYPlotLayer, IXYPlotLayerView>
	{
		protected IDisposable _docSuspendLock;

		private string _currentPageName;

		private LayerControllerTabType _primaryChoice; // which tab type is currently choosen
		private int _currentScale; // which scale is choosen 0==X-AxisScale, 1==Y-AxisScale

		private CSLineID _currentAxisID; // which style is currently choosen
		private CSPlaneID _currentPlaneID; // which plane is currently chosen for the grid

		private IMVCAController _currentController;

		protected Altaxo.Gui.Graph.CoordinateSystemController _coordinateController;
		protected IMVCANController _layerPositionController;
		protected IMVCANController _layerContentsController;
		protected IMVCAController[] _axisScaleController;

		protected IMVCANController _layerGraphItemsController;

		private Dictionary<CSLineID, AxisStyleControllerConditionalGlue> _axisControl;
		private Dictionary<CSPlaneID, IMVCANController> _GridStyleController;
		private object _lastControllerApplied;

		private SelectableListNodeList _listOfScales;
		private SelectableListNodeList _listOfAxes;
		private SelectableListNodeList _listOfPlanes;
		private SelectableListNodeList _listOfUniqueItem;

		public XYPlotLayerController(XYPlotLayer layer)
			: this(layer, "Scale", 1, null)
		{
		}

		public XYPlotLayerController(XYPlotLayer layer, string currentPage, CSLineID id)
			: this(layer, currentPage, id.ParallelAxisNumber, id)
		{
		}

		private XYPlotLayerController(XYPlotLayer layer, string currentPage, int axisScaleIdx, CSLineID id)
		{
			_originalDoc = layer;
			_doc = (XYPlotLayer)layer.Clone();

			_currentAxisID = id;
			_currentScale = axisScaleIdx;
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
			ApplyCurrentController(true);

			_doc.GridPlanes.RemoveUnused(); // Remove unused grid planes

			if (null != _docSuspendLock)
				_docSuspendLock.Dispose(); // revoke suspend lock to let cached things fixed

			_originalDoc.CopyFrom(_doc, GraphCopyOptions.All);

			_docSuspendLock = _doc.SuspendGetToken();

			return true;
		}

		protected override void Initialize(bool initData)
		{
			if (initData)
			{
				_docSuspendLock = _doc.SuspendGetToken();
				SetCoordinateSystemDependentObjects(_currentAxisID);

				_listOfUniqueItem = new SelectableListNodeList();
				_listOfUniqueItem.Add(new SelectableListNode("Common", null, true));
			}

			if (null != _view)
			{
				// add all necessary Tabs
				_view.AddTab("Scale", "Scale");
				_view.AddTab("CS", "Coord.System");
				_view.AddTab("Contents", "Contents");
				_view.AddTab("Position", "Position");
				_view.AddTab("TitleAndFormat", "Title/Format");
				_view.AddTab("MajorLabels", "Major labels");
				_view.AddTab("MinorLabels", "Minor labels");
				_view.AddTab("GridStyle", "Grid style");
				_view.AddTab("GraphicItems", "GraphicItems");

				// Set the controller of the current visible Tab
				SetCurrentTabController(true);
			}
		}

		private void SetCoordinateSystemDependentObjects()
		{
			SetCoordinateSystemDependentObjects(null);
		}

		private void SetCoordinateSystemDependentObjects(CSLineID id)
		{
			// Scales
			_axisScaleController = new ScaleWithTicksController[_doc.Scales.Count];
			_listOfScales = new SelectableListNodeList();
			if (_doc.Scales.Count > 0)
				_listOfScales.Add(new SelectableListNode("X-Scale", 0, false));
			if (_doc.Scales.Count > 1)
				_listOfScales.Add(new SelectableListNode("Y-Scale", 1, false));
			if (_doc.Scales.Count > 2)
				_listOfScales.Add(new SelectableListNode("Z-Scale", 2, false));

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

			_GridStyleController = new Dictionary<CSPlaneID, IMVCANController>();
		}

		private void SetCurrentTabController(bool pageChanged)
		{
			switch (_currentPageName)
			{
				case "GraphicItems":
					if (pageChanged)
					{
						_view.SelectTab(_currentPageName);
					}
					if (null == _layerGraphItemsController)
					{
						_layerGraphItemsController = (IMVCANController)Current.Gui.GetControllerAndControl(new object[] { _doc.GraphObjects }, typeof(IMVCANController), UseDocument.Directly);
					}
					_currentController = _layerGraphItemsController;
					_view.CurrentContent = _currentController.ViewObject;
					break;

				case "Contents":
					if (pageChanged)
					{
						_view.SelectTab(_currentPageName);
						SetSecondaryChoiceToUnique();
					}
					if (null == _layerContentsController)
					{
						_layerContentsController = (IMVCANController)Current.Gui.GetControllerAndControl(new object[] { _doc.PlotItems }, typeof(IMVCANController), UseDocument.Directly);
					}
					_currentController = _layerContentsController;
					_view.CurrentContent = _currentController.ViewObject;
					break;

				case "Position":
					if (pageChanged)
					{
						_view.SelectTab(_currentPageName);
						SetSecondaryChoiceToUnique();
					}
					if (null == _layerPositionController)
					{
						_layerPositionController = new LayerPositionController() { UseDocumentCopy = UseDocument.Directly };
						_layerPositionController.InitializeDocument(_doc.Location, _doc);
						Current.Gui.FindAndAttachControlTo(_layerPositionController);
					}
					_currentController = _layerPositionController;
					_view.CurrentContent = _layerPositionController.ViewObject;
					break;

				case "Scale":
					if (pageChanged)
					{
						_view.SelectTab(_currentPageName);
						SetSecondaryChoiceToScales();
					}
					if (_axisScaleController[_currentScale] == null)
					{
						var ctrl = new ScaleWithTicksController();
						ctrl.InitializeDocument(_doc.Scales[_currentScale]);
						_axisScaleController[_currentScale] = ctrl;
						Current.Gui.FindAndAttachControlTo(_axisScaleController[_currentScale]);
					}
					_currentController = _axisScaleController[_currentScale];
					_view.CurrentContent = _currentController.ViewObject;
					break;

				case "CS":
					if (pageChanged)
					{
						_view.SelectTab(_currentPageName);
						SetSecondaryChoiceToUnique();
					}
					if (null == this._coordinateController)
					{
						this._coordinateController = new Altaxo.Gui.Graph.CoordinateSystemController() { UseDocumentCopy = UseDocument.Directly };
						_coordinateController.InitializeDocument(_doc.CoordinateSystem);
						Current.Gui.FindAndAttachControlTo(this._coordinateController);
					}
					_currentController = this._coordinateController;
					_view.CurrentContent = this._coordinateController.ViewObject;
					break;

				case "GridStyle":
					if (pageChanged)
					{
						_view.SelectTab(_currentPageName);
						SetSecondaryChoiceToPlanes();
					}

					if (!_GridStyleController.ContainsKey(_currentPlaneID))
					{
						GridPlane p = _doc.GridPlanes.Contains(_currentPlaneID) ? _doc.GridPlanes[_currentPlaneID] : new GridPlane(_currentPlaneID);
						GridPlaneController ctrl = new GridPlaneController(p);
						//Current.Gui.FindAndAttachControlTo(ctrl);
						Current.Gui.FindAndAttachControlUsingGuiTemplate(ctrl, _view);
						_GridStyleController.Add(_currentPlaneID, ctrl);
					}
					_currentController = _GridStyleController[_currentPlaneID];
					_view.CurrentContent = this._currentController.ViewObject;

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

		private void SetSecondaryChoiceToUnique()
		{
			this._primaryChoice = LayerControllerTabType.Unique;
			_view.InitializeSecondaryChoice(_listOfUniqueItem, this._primaryChoice);
		}

		private void SetSecondaryChoiceToScales()
		{
			_listOfScales.ClearSelectionsAll();
			_listOfScales[_currentScale].IsSelected = true;

			this._primaryChoice = LayerControllerTabType.Scales;
			_view.InitializeSecondaryChoice(_listOfScales, this._primaryChoice);
		}

		private void SetSecondaryChoiceToAxes()
		{
			foreach (var item in _listOfAxes)
				item.IsSelected = ((CSLineID)item.Tag) == _currentAxisID;

			this._primaryChoice = LayerControllerTabType.Axes;
			_view.InitializeSecondaryChoice(_listOfAxes, this._primaryChoice);
		}

		private void SetSecondaryChoiceToPlanes()
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

		private void EhView_TabValidating(object sender, CancelEventArgs e)
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

		private bool ApplyCurrentController(bool force)
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
				_doc.CoordinateSystem = (G2DCoordinateSystem)_coordinateController.ModelObject;
				SetCoordinateSystemDependentObjects();
			}

			if (object.ReferenceEquals(_currentController, _layerPositionController))
			{
				_doc.Location = (IItemLocation)_currentController.ModelObject;
			}
			else if (_currentPageName == "GridStyle")
			{
				GridPlane gp = (GridPlane)_currentController.ModelObject;
				this._doc.GridPlanes[_currentPlaneID] = gp.IsUsed ? gp : null;
			}

			return true;
		}

		#region Dialog

		public static bool ShowDialog(XYPlotLayer layer)
		{
			return ShowDialog(layer, "Scale", new CSLineID(0, 0));
		}

		public static bool ShowDialog(XYPlotLayer layer, string currentPage)
		{
			return ShowDialog(layer, currentPage, new CSLineID(0, 0));
		}

		public static bool ShowDialog(XYPlotLayer layer, string currentPage, CSLineID currentEdge)
		{
			XYPlotLayerController ctrl = new XYPlotLayerController(layer, currentPage, currentEdge);
			return Current.Gui.ShowDialog(ctrl, layer.Name, true);
		}

		#endregion Dialog

		#region Edit Handlers

		public static void RegisterEditHandlers()
		{
			// register here editor methods

			XYPlotLayer.AxisScaleEditorMethod = new DoubleClickHandler(EhAxisScaleEdit);
			XYPlotLayer.AxisStyleEditorMethod = new DoubleClickHandler(EhAxisStyleEdit);
			XYPlotLayer.AxisLabelMajorStyleEditorMethod = new DoubleClickHandler(EhAxisLabelMajorStyleEdit);
			XYPlotLayer.AxisLabelMinorStyleEditorMethod = new DoubleClickHandler(EhAxisLabelMinorStyleEdit);
			XYPlotLayer.LayerPositionEditorMethod = new DoubleClickHandler(EhLayerPositionEdit);
		}

		public static bool EhLayerPositionEdit(IHitTestObject hit)
		{
			XYPlotLayer layer = hit.HittedObject as XYPlotLayer;
			if (layer == null)
				return false;

			ShowDialog(layer, "Position");

			return false;
		}

		public static bool EhAxisScaleEdit(IHitTestObject hit)
		{
			AxisLineStyle style = hit.HittedObject as AxisLineStyle;
			if (style == null || hit.ParentLayer == null)
				return false;

			var xylayer = hit.ParentLayer as XYPlotLayer;
			if (null != xylayer)
				ShowDialog(xylayer, "Scale", style.AxisStyleID);

			return false;
		}

		public static bool EhAxisStyleEdit(IHitTestObject hit)
		{
			AxisLineStyle style = hit.HittedObject as AxisLineStyle;
			if (style == null || hit.ParentLayer == null)
				return false;

			var xylayer = hit.ParentLayer as XYPlotLayer;
			if (null != xylayer)
				ShowDialog(xylayer, "TitleAndFormat", style.AxisStyleID);

			return false;
		}

		public static bool EhAxisLabelMajorStyleEdit(IHitTestObject hit)
		{
			AxisLabelStyle style = hit.HittedObject as AxisLabelStyle;
			if (style == null || hit.ParentLayer == null)
				return false;

			var xylayer = hit.ParentLayer as XYPlotLayer;
			if (null != xylayer)
				ShowDialog(xylayer, "MajorLabels", style.AxisStyleID);

			return false;
		}

		public static bool EhAxisLabelMinorStyleEdit(IHitTestObject hit)
		{
			AxisLabelStyle style = hit.HittedObject as AxisLabelStyle;
			if (style == null || hit.ParentLayer == null)
				return false;

			var xylayer = hit.ParentLayer as XYPlotLayer;
			if (null != xylayer)
				ShowDialog(xylayer, "MinorLabels", style.AxisStyleID);

			return false;
		}

		#endregion Edit Handlers
	}
}