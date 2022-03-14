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
using Altaxo.Collections;
using Altaxo.Graph.Gdi;
using Altaxo.Gui.Common;

namespace Altaxo.Gui.Graph.Gdi
{

  /// <summary>
  /// Summary description for LayerController.
  /// </summary>
  [UserControllerForObject(typeof(HostLayer))]
  [ExpectedTypeOfView(typeof(ITabbedElementViewDC))]
  public class HostLayerController : MVCANControllerEditOriginalDocBase<HostLayer, ITabbedElementViewDC>
  {
    protected IMVCANController? _layerPositionController;
    protected IMVCANController? _layerGraphItemsController;
    protected IMVCANController? _layerGridController;

    private IMVCANController? _currentController;
    private IMVCANController? _lastControllerApplied;

    private string? _initialTab;

    private SelectableListNodeList _listOfUniqueItem = new();

    public const string PositionTag = "Position";
    public const string HostGridTag = "HostGrid";
    public const string GraphItemsTag = "GraphicItems";

    public HostLayerController(HostLayer layer)
      : this(layer, PositionTag)
    {
    }

    private HostLayerController(HostLayer layer, string currentPage)
    {
      _initialTab = currentPage;
      InitializeDocument(layer);
    }
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield return new ControllerAndSetNullMethod(_layerPositionController, () => _layerPositionController = null);
      yield return new ControllerAndSetNullMethod(_layerGraphItemsController, () => _layerGraphItemsController = null);
      yield return new ControllerAndSetNullMethod(_layerGridController, () => _layerGridController = null);
    }


    #region Bindings

    public SelectableListNodeList Tabs { get; } = new();

    private string? _selectedTab;

    public string? SelectedTab
    {
      get => _selectedTab;
      set
      {
        if (!(_selectedTab == value))
        {
          if (ApplyCurrentController(true, false))
          {
            _selectedTab = value;
            SetCurrentTabController();
          }
          OnPropertyChanged(nameof(SelectedTab));
        }
      }
    }

    #endregion

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        _listOfUniqueItem = new SelectableListNodeList
        {
          new SelectableListNode("Common", null, true)
        };

        Tabs.Clear();
        Tabs.Add(new SelectableListNodeWithController("GraphicItems", GraphItemsTag, true));
        Tabs.Add(new SelectableListNodeWithController("Position", PositionTag, false));
        Tabs.Add(new SelectableListNodeWithController("HostGrid", HostGridTag, false));
        SelectedTab = _initialTab;
      }
    }

    public override bool Apply(bool disposeController)
    {
      ApplyCurrentController(true, disposeController);

      return ApplyEnd(true, disposeController);
    }

    public override void Dispose(bool isDisposing)
    {
      _lastControllerApplied = null;
      _currentController = null;
      _listOfUniqueItem = null!;
      Tabs.Clear();

      base.Dispose(isDisposing);
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
            node.Controller = _layerGraphItemsController = (IMVCANController?)Current.Gui.GetControllerAndControl(new object[] { _doc.GraphObjects }, typeof(IMVCANController), UseDocument.Directly);
          }
          _currentController = _layerGraphItemsController;
          break;

        case PositionTag:
          if (_layerPositionController is null)
          {
            node.Controller = _layerPositionController = new LayerPositionController() { UseDocumentCopy = UseDocument.Directly };
            _layerPositionController.InitializeDocument(_doc.Location, _doc);
            Current.Gui.FindAndAttachControlTo(_layerPositionController);
          }
          _currentController = _layerPositionController;
          break;

        case HostGridTag:
          if (_layerGridController is null)
          {
            node.Controller = _layerGridController = new GridPartitioningController() { UseDocumentCopy = UseDocument.Directly };
            _layerGridController.InitializeDocument(_doc.Grid, _doc);
            Current.Gui.FindAndAttachControlTo(_layerGridController);
          }
          _currentController = _layerGridController;
          break;
      }
    }

    private bool ApplyCurrentController(bool force, bool disposeController)
    {
      ThrowIfNotInitialized();

      if (_currentController is null)
        return true;

      if (!force && object.ReferenceEquals(_currentController, _lastControllerApplied))
        return true;

      if (!_currentController.Apply(disposeController))
        return false;

      if (object.ReferenceEquals(_currentController, _layerPositionController))
      {
        _doc.Location = (IItemLocation)_currentController.ModelObject;
      }

      _lastControllerApplied = _currentController;

      return true;
    }

    #region Dialog

    public static bool ShowDialog(HostLayer layer)
    {
      return ShowDialog(layer, PositionTag);
    }

    public static bool ShowDialog(HostLayer layer, string currentPage)
    {
      var ctrl = new HostLayerController(layer, currentPage);
      return Current.Gui.ShowDialog(ctrl, layer.Name, true);
    }

    #endregion Dialog

    #region Edit Handlers

    public static void RegisterEditHandlers()
    {
      // register here editor methods

      HostLayer.LayerPositionEditorMethod = new DoubleClickHandler(EhLayerPositionEdit);
    }

    public static bool EhLayerPositionEdit(IHitTestObject hit)
    {
      if (hit.HittedObject is not XYPlotLayer layer)
        return false;

      ShowDialog(layer, PositionTag);

      return false;
    }

    #endregion Edit Handlers
  }
}
