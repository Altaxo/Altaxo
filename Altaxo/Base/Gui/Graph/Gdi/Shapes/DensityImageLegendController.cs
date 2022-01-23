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
using Altaxo.Graph.Gdi.Shapes;
using Altaxo.Gui.Graph.Gdi.Axis;
using Altaxo.Gui.Graph.Scales;

namespace Altaxo.Gui.Graph.Gdi.Shapes
{
  /// <summary>
  /// Summary description for LayerController.
  /// </summary>
  [UserControllerForObject(typeof(DensityImageLegend))]
  [ExpectedTypeOfView(typeof(IXYPlotLayerView))]
  public class DensityImageLegendController : MVCANControllerEditOriginalDocBase<DensityImageLegend, IXYPlotLayerView>
  {
    protected string _initialTag;

    private int _currentScale; // which scale is choosen 0==X-AxisScale, 1==Y-AxisScale
    private CSLineID _currentAxisID; // which style is currently choosen
    private CSPlaneID _currentPlaneID; // which plane is currently chosen for the grid

    private IMVCAController _currentController;

    protected IMVCANController _axisScaleController;
    protected IMVCAController _coordinateController;
    protected IMVCANController _layerPositionController;

    private Dictionary<CSLineID, AxisStyleControllerConditionalGlue> _axisControl;

    private SelectableListNodeList _listOfScales;
    private SelectableListNodeList _listOfAxes;
    private SelectableListNodeList _listOfPlanes;
    private SelectableListNodeList _listOfUniqueItem;

    private object _lastControllerApplied;

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
      yield return new ControllerAndSetNullMethod(_axisScaleController, () => _axisScaleController = null);
      yield return new ControllerAndSetNullMethod(_coordinateController, () => _coordinateController = null);
      yield return new ControllerAndSetNullMethod(_layerPositionController, () => _layerPositionController = null);

      if (_axisControl is not null)
      {
        foreach (var item in EnumerableExtensions.ThisOrEmpty(_axisControl.Values))
        {
          yield return new ControllerAndSetNullMethod(item.AxisStyleCondController, null);
          yield return new ControllerAndSetNullMethod(item.MajorLabelCondController, null);
          yield return new ControllerAndSetNullMethod(item.MinorLabelCondController, null);
          yield return new ControllerAndSetNullMethod(null, () => _axisControl = null);
        }
      }
    }

    public override void Dispose(bool isDisposing)
    {
      _currentController = null;
      _lastControllerApplied = null;

      _listOfScales = null;
      _listOfAxes = null;
      _listOfPlanes = null;
      _listOfUniqueItem = null;

      base.Dispose(isDisposing);
    }

    public DensityImageLegendController()
    {
      _currentScale = 0;
      _initialTag = ScaleTag;
      _currentAxisID = CSLineID.X0;

      CmdMoveAxis = new RelayCommand(() => EhCmdCreateOrMoveAxis(true));
      CmdCreateAxis = new RelayCommand(() => EhCmdCreateOrMoveAxis(false));
      CmdDeleteAxis = new RelayCommand(EhCmdDeleteAxis);

    }

    public DensityImageLegendController(DensityImageLegend layer)
      : this(layer, ScaleTag, 1, CSLineID.X0)
    {
    }

    public DensityImageLegendController(DensityImageLegend layer, string currentPage, CSLineID id)
      : this(layer, currentPage, id.ParallelAxisNumber, id)
    {
    }

    private DensityImageLegendController(DensityImageLegend layer, string currentPage, int axisScaleIdx, CSLineID id)
    {
      CmdMoveAxis = new RelayCommand(() => EhCmdCreateOrMoveAxis(true));
      CmdCreateAxis = new RelayCommand(() => EhCmdCreateOrMoveAxis(false));
      CmdDeleteAxis = new RelayCommand(EhCmdDeleteAxis);

      InitializeDocument(layer, currentPage, axisScaleIdx, id);
    }

    public override bool InitializeDocument(params object[] args)
    {
      if (args is not null)
      {
        if (args.Length > 1 && args[1] is string) // [1] is currentPage
          _initialTag = (string)args[1];

        if (args.Length > 2 && args[2] is int)
          _currentScale = (int)args[2];

        if (args.Length > 3 && args[3] is CSLineID)
          _currentAxisID = (CSLineID)args[3];
      }

      return base.InitializeDocument(args);
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
        Tabs.Add(new SelectableListNodeWithController("Coord.System", CoordSystemTag, false));
        Tabs.Add(new SelectableListNodeWithController("Position", PositionTag, false));
        Tabs.Add(new SelectableListNodeWithController("Title/Format", TitleAndFormatTag, false));
        Tabs.Add(new SelectableListNodeWithController("Major labels", MajorLabelsTag, false));
        Tabs.Add(new SelectableListNodeWithController("Minor labels", MinorLabelsTag, false));
        // Set the controller of the current visible Tab
        SelectedTab = _initialTag;
      }
    }

    public override bool Apply(bool disposeController)
    {
      ApplyCurrentController(true, disposeController);

      return ApplyEnd(true, disposeController);
    }

    private void SetCoordinateSystemDependentObjects()
    {
      SetCoordinateSystemDependentObjects(null);
    }

    private void SetCoordinateSystemDependentObjects(CSLineID id)
    {
      // Scales
      _listOfScales = new SelectableListNodeList
      {
        new SelectableListNode("Z-Scale", 0, false)
      };

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

    private void SetCurrentTabController()
    {
      ThrowIfNotInitialized();

      if (Tabs.FirstOrDefault(n => (string?)(n.Tag) == SelectedTab) is not SelectableListNodeWithController node)
        return;

      switch (SelectedTab)
      {
        case CoordSystemTag:
          if (_coordinateController is null)
          {
            _coordinateController = (IMVCAController)Current.Gui.GetControllerAndControl(new object[] { _doc.CoordinateSystem }, typeof(IMVCAController), UseDocument.Directly);
          }
          node.Controller = _currentController = _coordinateController;
          break;

        case PositionTag:
          if (_layerPositionController is null)
          {
            _layerPositionController = new ItemLocationDirectController() { UseDocumentCopy = UseDocument.Directly };
            _layerPositionController.InitializeDocument(_doc.Location);
            Current.Gui.FindAndAttachControlTo(_layerPositionController);
          }
          node.Controller = _currentController = _layerPositionController;
          break;

        case ScaleTag:
          if (_axisScaleController is null)
          {
            _axisScaleController = new ScaleWithTicksController(null, true) { UseDocumentCopy = UseDocument.Directly };
            _axisScaleController.InitializeDocument(_doc.ScaleWithTicks);
            Current.Gui.FindAndAttachControlTo(_axisScaleController);
          }
          node.Controller = _currentController = _axisScaleController;
          break;

        case TitleAndFormatTag:
          node.Controller = _currentController = _axisControl[_currentAxisID].AxisStyleCondController;
          node.ViewObject = _axisControl[_currentAxisID].AxisStyleCondView;
          break;

        case MajorLabelsTag:
          node.Controller = _currentController = _axisControl[_currentAxisID].MajorLabelCondController;
          node.ViewObject = _axisControl[_currentAxisID].MajorLabelCondView;

          break;

        case MinorLabelsTag:
          node.Controller = _currentController = _axisControl[_currentAxisID].MinorLabelCondController;
          node.ViewObject = _axisControl[_currentAxisID].MinorLabelCondView;
          break;
      }
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

    public void EhCmdCreateOrMoveAxis(bool moveAxis)
    {
      if (!ApplyCurrentController(false, false))
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
          _axisControl.Remove(axisID);
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
        //_doc.CoordinateSystem = (G2DCoordinateSystem)_coordinateController.ModelObject;
        SetCoordinateSystemDependentObjects();
      }
      else if (object.ReferenceEquals(_currentController, _layerPositionController))
      {
        _doc.Location.CopyFrom((IItemLocation)_currentController.ModelObject);
      }

      return true;
    }

    #region Dialog

    public static bool ShowDialog(DensityImageLegend layer)
    {
      return ShowDialog(layer, ScaleTag, new CSLineID(0, 0));
    }

    public static bool ShowDialog(DensityImageLegend layer, string currentPage)
    {
      return ShowDialog(layer, currentPage, new CSLineID(0, 0));
    }

    public static bool ShowDialog(DensityImageLegend layer, string currentPage, CSLineID currentEdge)
    {
      var ctrl = new DensityImageLegendController(layer, currentPage, currentEdge);
      return Current.Gui.ShowDialog(ctrl, layer.Name, true);
    }

    #endregion Dialog

    #region Edit Handlers

    public static void RegisterEditHandlers()
    {
      // register here editor methods

      XYPlotLayer.LayerPositionEditorMethod = new DoubleClickHandler(EhLayerPositionEdit);
    }

    public static bool EhLayerPositionEdit(IHitTestObject hit)
    {
      var layer = hit.HittedObject as DensityImageLegend;
      if (layer is null)
        return false;

      ShowDialog(layer, PositionTag);

      return false;
    }

    #endregion Edit Handlers
  }
}
