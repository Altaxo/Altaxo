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
using Altaxo.Graph.Graph3D;
using Altaxo.Graph.Graph3D.Axis;
using Altaxo.Gui.Graph3D.Axis;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Altaxo.Gui.Graph3D
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

		event Action DeleteAxis;

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
	[UserControllerForObject(typeof(XYZPlotLayer))]
	[ExpectedTypeOfView(typeof(IXYPlotLayerView))]
	public class XYPlotLayerController : MVCANControllerEditOriginalDocBase<XYZPlotLayer, IXYPlotLayerView>
	{
		private string _currentPageName;

		private LayerControllerTabType _primaryChoice; // which tab type is currently choosen
		private int _currentScale; // which scale is choosen 0==X-AxisScale, 1==Y-AxisScale

		private CSLineID _currentAxisID; // which style is currently choosen
		private CSPlaneID _currentPlaneID; // which plane is currently chosen for the grid

		private IMVCAController _currentController;

		protected CoordinateSystemController _coordinateController;
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

		public XYPlotLayerController(XYZPlotLayer layer, UseDocument useDocumentCopy)
			: this(layer, "Scale", 1, null, useDocumentCopy)
		{
		}

		public XYPlotLayerController(XYZPlotLayer layer, string currentPage, CSLineID id, UseDocument useDocumentCopy)
			: this(layer, currentPage, id.ParallelAxisNumber, id, useDocumentCopy)
		{
		}

		private XYPlotLayerController(XYZPlotLayer layer, string currentPage, int axisScaleIdx, CSLineID id, UseDocument useDocumentCopy)
		{
			if (!id.Is3DIdentifier)
				throw new ArgumentException(nameof(id) + " has to be a 3D identifier!");

			_useDocumentCopy = useDocumentCopy == UseDocument.Copy;
			_currentAxisID = id;
			_currentScale = axisScaleIdx;
			_currentPageName = currentPage;

			InitializeDocument(layer);
		}

		protected override void Initialize(bool initData)
		{
			base.Initialize(initData);

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

		public override bool Apply(bool disposeController)
		{
			ApplyCurrentController(true, disposeController);

			_doc.GridPlanes.RemoveUnused(); // Remove unused grid planes

			return ApplyEnd(true, disposeController);
		}

		protected override void AttachView()
		{
			_view.TabValidating += EhView_TabValidating;
			_view.PageChanged += EhView_PageChanged;
			_view.SecondChoiceChanged += EhView_SecondChoiceChanged;
			_view.CreateOrMoveAxis += EhView_CreateOrMoveAxis;
			_view.DeleteAxis += EhView_DeleteAxis;
		}

		protected override void DetachView()
		{
			_view.TabValidating -= EhView_TabValidating;
			_view.PageChanged -= EhView_PageChanged;
			_view.SecondChoiceChanged -= EhView_SecondChoiceChanged;
			_view.CreateOrMoveAxis -= EhView_CreateOrMoveAxis;
			_view.DeleteAxis -= EhView_DeleteAxis;
		}

		public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
		{
			yield return new ControllerAndSetNullMethod(_coordinateController, () => _coordinateController = null);
			yield return new ControllerAndSetNullMethod(_layerPositionController, () => _layerPositionController = null);
			yield return new ControllerAndSetNullMethod(_layerContentsController, () => _layerContentsController = null);

			if (null != _axisScaleController)
			{
				for (int i = 0; i < _axisScaleController.Length; ++i)
					yield return new ControllerAndSetNullMethod(_axisScaleController[i], () => _axisScaleController[i] = null);
			}

			yield return new ControllerAndSetNullMethod(_layerGraphItemsController, () => _layerGraphItemsController = null);

			if (null != _GridStyleController)
			{
				foreach (var entry in _GridStyleController)
				{
					yield return new ControllerAndSetNullMethod(entry.Value, null);
				}
			}

			yield return new ControllerAndSetNullMethod(null, () => _GridStyleController = null);

			if (null != _axisControl)
			{
				foreach (var entry in _axisControl)
				{
					yield return new ControllerAndSetNullMethod(entry.Value.AxisStyleCondController, null);
					yield return new ControllerAndSetNullMethod(entry.Value.MajorLabelCondController, null);
					yield return new ControllerAndSetNullMethod(entry.Value.MinorLabelCondController, null);
				}
			}
			yield return new ControllerAndSetNullMethod(null, () => _axisControl = null);
		}

		public override void Dispose(bool isDisposing)
		{
			_lastControllerApplied = null;
			_currentController = null;

			_listOfScales = null;
			_listOfAxes = null;
			_listOfPlanes = null;
			_listOfUniqueItem = null;

			base.Dispose(isDisposing);
		}

		private void SetCoordinateSystemDependentObjects()
		{
			SetCoordinateSystemDependentObjects(null);
		}

		private void SetCoordinateSystemDependentObjects(CSLineID id)
		{
			// Scales
			_axisScaleController = new Graph.ScaleWithTicksController[_doc.Scales.Count];
			_listOfScales = new SelectableListNodeList();
			if (_doc.Scales.Count > 0)
				_listOfScales.Add(new SelectableListNode("X-Scale", 0, false));
			if (_doc.Scales.Count > 1)
				_listOfScales.Add(new SelectableListNode("Y-Scale", 1, false));
			if (_doc.Scales.Count > 2)
				_listOfScales.Add(new SelectableListNode("Z-Scale", 2, false));

			// collect the AxisStyleIdentifier from the actual layer and also all possible AxisStyleIdentifier
			_axisControl = new Dictionary<CSLineID, Axis.AxisStyleControllerConditionalGlue>();
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
						var ctrl = new Graph.ScaleWithTicksController(scale => _doc.Scales[_currentScale] = scale, false);
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
						this._coordinateController = new CoordinateSystemController() { UseDocumentCopy = UseDocument.Directly };
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
						GridPlaneController ctrl = new GridPlaneController() { UseDocumentCopy = UseDocument.Directly };
						ctrl.InitializeDocument(p);
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
			ApplyCurrentController(false, false);

			_currentPageName = firstChoice;
			SetCurrentTabController(true);
		}

		public void EhView_SecondChoiceChanged()
		{
			if (!ApplyCurrentController(false, false))
				return;

			if (_primaryChoice == LayerControllerTabType.Scales)
			{
				_currentScale = (int)_listOfScales.FirstSelectedNode.Tag;
			}
			else if (_primaryChoice == LayerControllerTabType.Axes)
			{
				_currentAxisID = (CSLineID)(_listOfAxes.FirstSelectedNode?.Tag ?? _listOfAxes[0].Tag);
			}
			else if (_primaryChoice == LayerControllerTabType.Planes)
			{
				_currentPlaneID = (CSPlaneID)_listOfPlanes.FirstSelectedNode.Tag;
			}

			SetCurrentTabController(false);
		}

		private void EhView_TabValidating(object sender, CancelEventArgs e)
		{
			if (!ApplyCurrentController(true, false))
				e.Cancel = true;
		}

		public void EhView_CreateOrMoveAxis(bool moveAxis)
		{
			if (!ApplyCurrentController(false, false))
				return;

			var creationArgs = new Axis.AxisCreationArguments();

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

			Axis.AxisCreationArguments.AddAxis(_doc.AxisStyles, creationArgs); // add the new axis to the document
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

		public void EhView_DeleteAxis()
		{
			if (!ApplyCurrentController(false, false))
				return;

			if (_listOfAxes.Count <= 1)
				return;

			var axisID = _currentAxisID;
			if (true == Current.Gui.YesNoMessageBox("Are you sure that you want to delete this axis?", "Confirmation", false))
			{
				if (true == _doc.AxisStyles.Remove(axisID))
				{
					_axisControl.Remove(axisID);
					var axisItem = _listOfAxes.First(x => axisID != (CSLineID)(x.Tag));
					axisItem.IsSelected = true;
					_currentAxisID = (CSLineID)(axisItem.Tag);
					_listOfAxes.RemoveWhere(x => axisID == (CSLineID)(x.Tag));
					SetSecondaryChoiceToAxes();
					SetCurrentTabController(false);
				}
			}
		}

		private bool ApplyCurrentController(bool force, bool disposeCurrentController)
		{
			if (_currentController == null)
				return true;

			if (!force && object.ReferenceEquals(_currentController, _lastControllerApplied))
				return true;

			if (!_currentController.Apply(disposeCurrentController))
				return false;
			_lastControllerApplied = _currentController;

			if (object.ReferenceEquals(_currentController, _coordinateController))
			{
				_doc.CoordinateSystem = (G3DCoordinateSystem)_coordinateController.ModelObject;
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

		public static bool ShowDialog(XYZPlotLayer layer)
		{
			return ShowDialog(layer, "Scale", new CSLineID(0, 0, 0));
		}

		public static bool ShowDialog(XYZPlotLayer layer, string currentPage)
		{
			return ShowDialog(layer, currentPage, new CSLineID(0, 0));
		}

		public static bool ShowDialog(XYZPlotLayer layer, string currentPage, CSLineID currentEdge)
		{
			XYPlotLayerController ctrl = new XYPlotLayerController(layer, currentPage, currentEdge, UseDocument.Copy);
			return Current.Gui.ShowDialog(ctrl, layer.Name, true);
		}

		#endregion Dialog

		#region Edit Handlers

		public static void RegisterEditHandlers()
		{
			// register here editor methods

			XYZPlotLayer.AxisScaleEditorMethod = new DoubleClickHandler(EhAxisScaleEdit);
			XYZPlotLayer.AxisStyleEditorMethod = new DoubleClickHandler(EhAxisStyleEdit);
			XYZPlotLayer.AxisLabelMajorStyleEditorMethod = new DoubleClickHandler(EhAxisLabelMajorStyleEdit);
			XYZPlotLayer.AxisLabelMinorStyleEditorMethod = new DoubleClickHandler(EhAxisLabelMinorStyleEdit);
			XYZPlotLayer.LayerPositionEditorMethod = new DoubleClickHandler(EhLayerPositionEdit);
		}

		public static bool EhLayerPositionEdit(IHitTestObject hit)
		{
			var layer = hit.HittedObject as XYZPlotLayer;
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

			var xylayer = hit.ParentLayer as XYZPlotLayer;
			if (null != xylayer)
				ShowDialog(xylayer, "Scale", style.AxisStyleID);

			return false;
		}

		public static bool EhAxisStyleEdit(IHitTestObject hit)
		{
			AxisLineStyle style = hit.HittedObject as AxisLineStyle;
			if (style == null || hit.ParentLayer == null)
				return false;

			var xylayer = hit.ParentLayer as XYZPlotLayer;
			if (null != xylayer)
				ShowDialog(xylayer, "TitleAndFormat", style.AxisStyleID);

			return false;
		}

		public static bool EhAxisLabelMajorStyleEdit(IHitTestObject hit)
		{
			var style = hit.HittedObject as AxisLabelStyle;
			if (style == null || hit.ParentLayer == null)
				return false;

			var xylayer = hit.ParentLayer as XYZPlotLayer;
			if (null != xylayer)
				ShowDialog(xylayer, "MajorLabels", style.AxisStyleID);

			return false;
		}

		public static bool EhAxisLabelMinorStyleEdit(IHitTestObject hit)
		{
			var style = hit.HittedObject as AxisLabelStyle;
			if (style == null || hit.ParentLayer == null)
				return false;

			var xylayer = hit.ParentLayer as XYZPlotLayer;
			if (null != xylayer)
				ShowDialog(xylayer, "MinorLabels", style.AxisStyleID);

			return false;
		}

		#endregion Edit Handlers
	}
}