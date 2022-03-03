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


using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Altaxo.Collections;
using Altaxo.Graph;
using Altaxo.Graph.Gdi;
using Altaxo.Graph.Gdi.Axis;
using Altaxo.Gui.Graph.Gdi.Axis;
using Altaxo.Gui.Graph.Scales;

#nullable disable
namespace Altaxo.Gui.Graph.Gdi
{
  public interface IXYPlotLayerView : IDataContextAwareView
  {
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


  /// <summary>
  /// Summary description for LayerController.
  /// </summary>
  [UserControllerForObject(typeof(XYPlotLayer))]
  [ExpectedTypeOfView(typeof(IXYPlotLayerView))]
  public class XYPlotLayerController : MVCANControllerEditOriginalDocBase<XYPlotLayer, IXYPlotLayerView>
  {
    protected string _initialTag;

    private int _currentScale; // which scale is choosen 0==X-AxisScale, 1==Y-AxisScale

    private CSLineID _currentAxisID; // which style is currently choosen
    private CSPlaneID _currentPlaneID; // which plane is currently chosen for the grid

    private IMVCAController _currentController;

    protected Altaxo.Gui.Graph.Gdi.CoordinateSystemController _coordinateController;
    protected IMVCANController _layerPositionController;
    protected IMVCANController _layerContentsController;
    protected IMVCAController[] _axisScaleController;

    protected IMVCANController _layerGraphItemsController;

    private Dictionary<CSLineID, AxisStyleController> _axisController = new Dictionary<CSLineID, AxisStyleController>();
    private Dictionary<CSPlaneID, IMVCANController> _gridStyleController;
    private object _lastControllerApplied;

    private SelectableListNodeList _listOfScales;
    private SelectableListNodeList _listOfAxes;
    private SelectableListNodeList _listOfPlanes;
    private SelectableListNodeList _listOfUniqueItem;

    public const string PositionTag = "Position";
    public const string GraphItemsTag = "GraphicItems";
    public const string ScaleTag = "Scale";
    public const string CoordSystemTag = "CoordSys";
    public const string ContentsTag = "Content";
    public const string TitleAndFormatTag = "TitleFormat";
    public const string MajorLabelsTag = "MajorLabels";
    public const string MinorLabelsTag = "MinorLables";
    public const string GridStyleTag = "GridStyle";
    public const string SecondaryCommonTag = "2ndCommon";

    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield return new ControllerAndSetNullMethod(_coordinateController, () => _coordinateController = null);
      yield return new ControllerAndSetNullMethod(_layerPositionController, () => _layerPositionController = null);
      yield return new ControllerAndSetNullMethod(_layerContentsController, () => _layerContentsController = null);

      if (_axisScaleController is not null)
      {
        for (int i = 0; i < _axisScaleController.Length; ++i)
          yield return new ControllerAndSetNullMethod(_axisScaleController[i], () => _axisScaleController[i] = null);
      }

      yield return new ControllerAndSetNullMethod(_layerGraphItemsController, () => _layerGraphItemsController = null);

      foreach (var entry in EnumerableExtensions.ThisOrEmpty(_gridStyleController))
      {
        yield return new ControllerAndSetNullMethod(entry.Value, null);
      }

      yield return new ControllerAndSetNullMethod(null, () => _gridStyleController = null);

      foreach (var entry in EnumerableExtensions.ThisOrEmpty(_axisController))
      {
        yield return new ControllerAndSetNullMethod(entry.Value, null);
      }
      yield return new ControllerAndSetNullMethod(null, () => _axisController = null);
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


    public XYPlotLayerController(XYPlotLayer layer, UseDocument useDocumentCopy)
      : this(layer, ScaleTag, 1, null, useDocumentCopy)
    {
    }

    public XYPlotLayerController(XYPlotLayer layer, string currentPage, CSLineID id, UseDocument useDocumentCopy)
      : this(layer, currentPage, id.ParallelAxisNumber, id, useDocumentCopy)
    {
    }

    private XYPlotLayerController(XYPlotLayer layer, string currentPage, int axisScaleIdx, CSLineID id, UseDocument useDocumentCopy)
    {
      _useDocumentCopy = useDocumentCopy == UseDocument.Copy;
      _currentAxisID = id;
      _currentScale = axisScaleIdx;
      _initialTag = currentPage;

      CmdMoveAxis = new RelayCommand(() => EhCmdCreateOrMoveAxis(true));
      CmdCreateAxis = new RelayCommand(() => EhCmdCreateOrMoveAxis(false));
      CmdDeleteAxis = new RelayCommand(EhCmdDeleteAxis);

      InitializeDocument(layer);
    }

    #region Bindings

    public ICommand CmdMoveAxis { get; set; }
    public ICommand CmdCreateAxis { get; set; }
    public ICommand CmdDeleteAxis { get; set; }

    public SelectableListNodeList Tabs { get; } = new();

    private string? _selectedTab;

    public string? SelectedTab
    {
      get => _selectedTab;
      set
      {
        if (!(_selectedTab == value))
        {
          _selectedTab = value;
          EhPrimaryChoiceChanged(value);
          OnPropertyChanged(nameof(SelectedTab));
        }
      }
    }

    protected void EhPrimaryChoiceChanged(string selectedTab)
    {
      switch (selectedTab)
      {
        case GraphItemsTag:
        case ContentsTag:
        case PositionTag:
        case CoordSystemTag:
          SetSecondaryChoiceToUnique();
          break;

        case ScaleTag:
          SetSecondaryChoiceToScales();
          break;

        case GridStyleTag:
          SetSecondaryChoiceToPlanes();
          break;

        case TitleAndFormatTag:
        case MajorLabelsTag:
        case MinorLabelsTag:
          SetSecondaryChoiceToAxes();
          break;
      }
    }

    private SelectableListNodeList _secondaryChoices;

    public SelectableListNodeList SecondaryChoices
    {
      get => _secondaryChoices;
      set
      {

        if (!(_secondaryChoices == value))
        {
          _secondaryChoices = value;
          OnPropertyChanged(nameof(SecondaryChoices));
        }
      }
    }


    private object _selectedSecondaryChoice;

    public object SelectedSecondaryChoice
    {
      get => _selectedSecondaryChoice;
      set
      {
        if (!(_selectedSecondaryChoice == value))
        {
          if (value is not null)
          {
            if (ApplyCurrentController(false, false))
            {
              _selectedSecondaryChoice = value;
              EhSecondaryChoiceChanged(value);
            }
          }
          _selectedSecondaryChoice = value;
          OnPropertyChanged(nameof(SelectedSecondaryChoice));
        }
      }
    }

    public void EhSecondaryChoiceChanged(object value)
    {
      if (SelectedTab == ScaleTag && value is int currentScale)
      {
        _currentScale = currentScale;
      }
      else if (SelectedTab == TitleAndFormatTag || SelectedTab == MajorLabelsTag || SelectedTab == MinorLabelsTag)
      {
        _currentAxisID = value is CSLineID csLineID ? csLineID : (CSLineID)(_listOfAxes[0].Tag!);
      }
      else if (SelectedTab == GridStyleTag && value is CSPlaneID csPlaneID)
      {
        _currentPlaneID = csPlaneID;
      }

      SetCurrentTabController();
    }


    private bool _areAxisButtonsVisible;

    public bool AreAxisButtonsVisible
    {
      get => _areAxisButtonsVisible;
      set
      {
        if (!(_areAxisButtonsVisible == value))
        {
          _areAxisButtonsVisible = value;
          OnPropertyChanged(nameof(AreAxisButtonsVisible));
        }
      }
    }

    #endregion

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        SetCoordinateSystemDependentObjects(_currentAxisID);

        _listOfUniqueItem = new SelectableListNodeList
        {
          new SelectableListNode("Common", SecondaryCommonTag, true)
        };

        Tabs.Clear();
        Tabs.Add(new SelectableListNodeWithController("Scale", ScaleTag, false));
        Tabs.Add(new SelectableListNodeWithController("Coord.system", CoordSystemTag, false));
        Tabs.Add(new SelectableListNodeWithController("Contents", ContentsTag, false));
        Tabs.Add(new SelectableListNodeWithController("Position", PositionTag, false));
        Tabs.Add(new SelectableListNodeWithController("Title/Format", TitleAndFormatTag, false));
        Tabs.Add(new SelectableListNodeWithController("Major labels", MajorLabelsTag, false));
        Tabs.Add(new SelectableListNodeWithController("Minor labels", MinorLabelsTag, false));
        Tabs.Add(new SelectableListNodeWithController("Grid style", GridStyleTag, false));
        Tabs.Add(new SelectableListNodeWithController("Graphic items", GraphItemsTag, false));
        // Set the controller of the current visible Tab
        SelectedTab = _initialTag;
      }
    }

    public override bool Apply(bool disposeController)
    {
      ApplyCurrentController(true, disposeController);

      _doc.GridPlanes.RemoveUnused(); // Remove unused grid planes

      return ApplyEnd(true, disposeController);
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
      _axisController.Values.ForEachDo(controller => controller?.Dispose());
      _axisController.Clear();
      _listOfAxes = new SelectableListNodeList();
      foreach (CSLineID ids in _doc.CoordinateSystem.GetJoinedAxisStyleIdentifier(_doc.AxisStyles.AxisStyleIDs, new CSLineID[] { id }))
      {
        CSAxisInformation info = _doc.CoordinateSystem.GetAxisStyleInformation(ids);
        _listOfAxes.Add(new SelectableListNode(info.NameOfAxisStyle, info.Identifier, false));
      }

      // Planes
      _listOfPlanes = new SelectableListNodeList();
      _currentPlaneID = CSPlaneID.Front;
      _listOfPlanes.Add(new SelectableListNode("Front", _currentPlaneID, true));

      _gridStyleController = new Dictionary<CSPlaneID, IMVCANController>();
    }

    private void SetCurrentTabController()
    {
      ThrowIfNotInitialized();

      if (Tabs.FirstOrDefault(n => (string?)(n.Tag) == SelectedTab) is not SelectableListNodeWithController node)
        return;

      switch (SelectedTab)
      {
        case GraphItemsTag:

          if (_layerGraphItemsController is null)
          {
            _layerGraphItemsController = (IMVCANController)Current.Gui.GetControllerAndControl(new object[] { _doc.GraphObjects }, typeof(IMVCANController), UseDocument.Directly);
          }
          node.Controller = _currentController = _layerGraphItemsController;
          break;

        case ContentsTag:
          if (_layerContentsController is null)
          {
            _layerContentsController = (IMVCANController)Current.Gui.GetControllerAndControl(new object[] { _doc.PlotItems }, typeof(IMVCANController), UseDocument.Directly);
          }
          node.Controller = _currentController = _layerContentsController;
          break;

        case PositionTag:
          if (_layerPositionController is null)
          {
            _layerPositionController = new LayerPositionController() { UseDocumentCopy = UseDocument.Directly };
            _layerPositionController.InitializeDocument(_doc.Location, _doc);
            Current.Gui.FindAndAttachControlTo(_layerPositionController);
          }
          node.Controller = _currentController = _layerPositionController;
          break;

        case ScaleTag:
          if (_axisScaleController[_currentScale] is null)
          {
            var ctrl = new ScaleWithTicksController(scale => _doc.Scales[_currentScale] = scale, false);
            ctrl.InitializeDocument(_doc.Scales[_currentScale]);
            _axisScaleController[_currentScale] = ctrl;
            Current.Gui.FindAndAttachControlTo(_axisScaleController[_currentScale]);
          }
          node.Controller = _currentController = _axisScaleController[_currentScale];
          break;

        case CoordSystemTag:
          if (_coordinateController is null)
          {
            _coordinateController = new Altaxo.Gui.Graph.Gdi.CoordinateSystemController() { UseDocumentCopy = UseDocument.Directly };
            _coordinateController.InitializeDocument(_doc.CoordinateSystem);
            Current.Gui.FindAndAttachControlTo(_coordinateController);
          }
          node.Controller = _currentController = _coordinateController;
          break;

        case GridStyleTag:
          if (!_gridStyleController.ContainsKey(_currentPlaneID))
          {
            GridPlane p = _doc.GridPlanes.Contains(_currentPlaneID) ? _doc.GridPlanes[_currentPlaneID] : new GridPlane(_currentPlaneID);
            var ctrl = new GridPlaneController() { UseDocumentCopy = UseDocument.Directly };
            ctrl.InitializeDocument(p);
            Current.Gui.FindAndAttachControlTo(ctrl);
            _gridStyleController.Add(_currentPlaneID, ctrl);
          }
          node.Controller = _currentController = _gridStyleController[_currentPlaneID];

          break;

        case TitleAndFormatTag:
          {
            var axisStyleController = GetOrCreateAxisStyleController(_currentAxisID);
            node.Controller = _currentController = axisStyleController;
            if (axisStyleController.ViewObject is null)
              Current.Gui.FindAndAttachControlTo(axisStyleController);
            node.ViewObject = axisStyleController.ViewObject;
          }
          break;

        case MajorLabelsTag:
          {
            var axisStyleController = GetOrCreateAxisStyleController(_currentAxisID);
            node.Controller = _currentController = axisStyleController.MajorLabelCondController;
            node.ViewObject = axisStyleController.MajorLabelCondView;
          }
          break;

        case MinorLabelsTag:
          {
            var axisStyleController = GetOrCreateAxisStyleController(_currentAxisID);
            node.Controller = _currentController = axisStyleController.MinorLabelCondController;
            node.ViewObject = axisStyleController.MinorLabelCondView;
          }
          break;
      }
    }

    /// <summary>
    /// Gets or creates the axis style controller for the current line ID. If there is not axis style for the line ID,
    /// an empty axis style is created before.
    /// </summary>
    /// <param name="currentLineID">The current line identifier.</param>
    /// <returns>The axis style controller.</returns>
    private AxisStyleController GetOrCreateAxisStyleController(CSLineID currentLineID)
    {
      if (!_axisController.TryGetValue(_currentAxisID, out var axisStyleController))
      {
        axisStyleController = new AxisStyleController();
        if (!_doc.AxisStyles.TryGetValue(_currentAxisID, out var axisStyle))
        {
          axisStyle = new AxisStyle(_currentAxisID, false, false, false, null, _doc.GetPropertyContext());
          _doc.AxisStyles.Add(axisStyle);
        }
        axisStyleController.InitializeDocument(axisStyle);
        _axisController[_currentAxisID] = axisStyleController;
      }
      return axisStyleController;
    }

    private void SetSecondaryChoiceToUnique()
    {
      AreAxisButtonsVisible = false;
      SelectedSecondaryChoice = null;
      SecondaryChoices = _listOfUniqueItem;
      SelectedSecondaryChoice = SecondaryCommonTag;
    }

    private void SetSecondaryChoiceToScales()
    {
      AreAxisButtonsVisible = false;
      SelectedSecondaryChoice = null;
      SecondaryChoices = _listOfScales;
      SelectedSecondaryChoice = _currentScale;
    }

    private void SetSecondaryChoiceToAxes()
    {
      AreAxisButtonsVisible = true;
      SelectedSecondaryChoice = null;
      SecondaryChoices = _listOfAxes;
      SelectedSecondaryChoice = _currentAxisID;
    }

    private void SetSecondaryChoiceToPlanes()
    {
      AreAxisButtonsVisible = false;
      SelectedSecondaryChoice = null;
      SecondaryChoices = _listOfPlanes;
      SelectedSecondaryChoice = _currentPlaneID;
    }

    private void EhCmdCreateOrMoveAxis(bool moveAxis)
    {
      if (!ApplyCurrentController(false, false))
        return;

      var creationArgs = new AxisCreationArguments();

      creationArgs.InitializeAxisInformationList(_doc.CoordinateSystem, _doc.AxisStyles);
      creationArgs.TemplateStyle = _currentAxisID;
      creationArgs.MoveAxis = moveAxis;

      if (!Current.Gui.ShowDialog(ref creationArgs, "Create/move axis", false))
        return;

      if (_axisController.ContainsKey(creationArgs.CurrentStyle))
        return; // the axis is already present

      var oldIdentity = creationArgs.TemplateStyle;
      var newIdentity = creationArgs.CurrentStyle;
      var newAxisInfo = _doc.CoordinateSystem.GetAxisStyleInformation(newIdentity);

      AxisCreationArguments.AddAxis(_doc.AxisStyles, creationArgs); // add the new axis to the document
      SetSecondaryChoiceToUnique();

      _listOfAxes.ClearSelectionsAll();
      _listOfAxes.Add(new SelectableListNode(newAxisInfo.NameOfAxisStyle, newIdentity, true));

      if (creationArgs.MoveAxis && _axisController.ContainsKey(oldIdentity))
      {
        _axisController.Remove(oldIdentity);
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
      SetCurrentTabController();
    }

    private void EhCmdDeleteAxis()
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
          _axisController.Remove(axisID);
          var axisItem = _listOfAxes.First(x => axisID != (CSLineID)(x.Tag));
          axisItem.IsSelected = true;
          _currentAxisID = (CSLineID)(axisItem.Tag);
          _listOfAxes.RemoveWhere(x => axisID == (CSLineID)(x.Tag));
          SetSecondaryChoiceToAxes();
          SetCurrentTabController();
        }
      }
    }

    private bool ApplyCurrentController(bool force, bool disposeCurrentController)
    {
      if (_currentController is null)
        return true;

      if (!force && object.ReferenceEquals(_currentController, _lastControllerApplied))
        return true;

      if (!_currentController.Apply(disposeCurrentController))
        return false;
      _lastControllerApplied = _currentController;

      if (object.ReferenceEquals(_currentController, _coordinateController))
      {
        _doc.CoordinateSystem = (G2DCoordinateSystem)_coordinateController.ModelObject;
        SetCoordinateSystemDependentObjects(); // ToDo why does this take so long execution time?
      }

      if (object.ReferenceEquals(_currentController, _layerPositionController))
      {
        _doc.Location = (IItemLocation)_currentController.ModelObject;
      }
      else if (_gridStyleController.Values.Contains(_currentController))
      {
        var gp = (GridPlane)_currentController.ModelObject;
        _doc.GridPlanes[_currentPlaneID] = gp.IsUsed ? gp : null;
      }

      return true;
    }

    #region Dialog

    public static bool ShowDialog(XYPlotLayer layer)
    {
      return ShowDialog(layer, ScaleTag, new CSLineID(0, 0));
    }

    public static bool ShowDialog(XYPlotLayer layer, string currentPage)
    {
      return ShowDialog(layer, currentPage, new CSLineID(0, 0));
    }

    public static bool ShowDialog(XYPlotLayer layer, string currentPage, CSLineID currentEdge)
    {
      var ctrl = new XYPlotLayerController(layer, currentPage, currentEdge, UseDocument.Copy);
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
      var layer = hit.HittedObject as XYPlotLayer;
      if (layer is null)
        return false;

      ShowDialog(layer, PositionTag);

      return false;
    }

    public static bool EhAxisScaleEdit(IHitTestObject hit)
    {
      var style = hit.HittedObject as AxisLineStyle;
      if (style is null || hit.ParentLayer is null)
        return false;

      var xylayer = hit.ParentLayer as XYPlotLayer;
      if (xylayer is not null)
        ShowDialog(xylayer, ScaleTag, style.AxisStyleID);

      return false;
    }

    public static bool EhAxisStyleEdit(IHitTestObject hit)
    {
      var style = hit.HittedObject as AxisLineStyle;
      if (style is null || hit.ParentLayer is null)
        return false;

      var xylayer = hit.ParentLayer as XYPlotLayer;
      if (xylayer is not null)
        ShowDialog(xylayer, TitleAndFormatTag, style.AxisStyleID);

      return false;
    }

    public static bool EhAxisLabelMajorStyleEdit(IHitTestObject hit)
    {
      var style = hit.HittedObject as AxisLabelStyle;
      if (style is null || hit.ParentLayer is null)
        return false;

      var xylayer = hit.ParentLayer as XYPlotLayer;
      if (xylayer is not null)
        ShowDialog(xylayer, MajorLabelsTag, style.AxisStyleID);

      return false;
    }

    public static bool EhAxisLabelMinorStyleEdit(IHitTestObject hit)
    {
      var style = hit.HittedObject as AxisLabelStyle;
      if (style is null || hit.ParentLayer is null)
        return false;

      var xylayer = hit.ParentLayer as XYPlotLayer;
      if (xylayer is not null)
        ShowDialog(xylayer, MinorLabelsTag, style.AxisStyleID);

      return false;
    }

    #endregion Edit Handlers
  }
}
