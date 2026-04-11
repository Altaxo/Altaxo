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
  /// Controller for editing a <see cref="HostLayer"/>.
  /// </summary>
  [UserControllerForObject(typeof(HostLayer))]
  [ExpectedTypeOfView(typeof(ITabbedElementViewDC))]
  public class HostLayerController : MVCANControllerEditOriginalDocBase<HostLayer, ITabbedElementViewDC>
  {
    /// <summary>
    /// The controller for the position page.
    /// </summary>
    protected IMVCANController? _layerPositionController;

    /// <summary>
    /// The controller for the graphic-items page.
    /// </summary>
    protected IMVCANController? _layerGraphItemsController;

    /// <summary>
    /// The controller for the host-grid page.
    /// </summary>
    protected IMVCANController? _layerGridController;

    private IMVCANController? _currentController;
    private IMVCANController? _lastControllerApplied;

    private string? _initialTab;

    private SelectableListNodeList _listOfUniqueItem = new();

    /// <summary>
    /// Tag for the position page.
    /// </summary>
    public const string PositionTag = "Position";

    /// <summary>
    /// Tag for the host grid page.
    /// </summary>
    public const string HostGridTag = "HostGrid";

    /// <summary>
    /// Tag for the graphic items page.
    /// </summary>
    public const string GraphItemsTag = "GraphicItems";

    /// <summary>
    /// Initializes a new instance of the <see cref="HostLayerController"/> class.
    /// </summary>
    /// <param name="layer">The host layer.</param>
    public HostLayerController(HostLayer layer)
      : this(layer, PositionTag)
    {
    }

    private HostLayerController(HostLayer layer, string currentPage)
    {
      _initialTab = currentPage;
      InitializeDocument(layer);
    }
    /// <inheritdoc />
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield return new ControllerAndSetNullMethod(_layerPositionController, () => _layerPositionController = null);
      yield return new ControllerAndSetNullMethod(_layerGraphItemsController, () => _layerGraphItemsController = null);
      yield return new ControllerAndSetNullMethod(_layerGridController, () => _layerGridController = null);
    }


    #region Bindings

    /// <summary>
    /// Gets the available tabs.
    /// </summary>
    public SelectableListNodeList Tabs { get; } = new();

    private string? _selectedTab;

    /// <summary>
    /// Gets or sets the selected tab.
    /// </summary>
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

    /// <inheritdoc />
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

    /// <inheritdoc />
    public override bool Apply(bool disposeController)
    {
      ApplyCurrentController(true, disposeController);

      return ApplyEnd(true, disposeController);
    }

    /// <inheritdoc />
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

    /// <summary>
    /// Shows the dialog for editing the specified host layer.
    /// </summary>
    /// <param name="layer">The host layer to edit.</param>
    /// <returns><c>true</c> if the dialog was accepted; otherwise, <c>false</c>.</returns>
    public static bool ShowDialog(HostLayer layer)
    {
      return ShowDialog(layer, PositionTag);
    }

    /// <summary>
    /// Shows the dialog for editing the specified host layer and initial page.
    /// </summary>
    /// <param name="layer">The host layer to edit.</param>
    /// <param name="currentPage">The page that should be shown initially.</param>
    /// <returns><c>true</c> if the dialog was accepted; otherwise, <c>false</c>.</returns>
    public static bool ShowDialog(HostLayer layer, string currentPage)
    {
      var ctrl = new HostLayerController(layer, currentPage);
      return Current.Gui.ShowDialog(ctrl, layer.Name, true);
    }

    #endregion Dialog

    #region Edit Handlers

    /// <summary>
    /// Registers edit handlers for host layers.
    /// </summary>
    public static void RegisterEditHandlers()
    {
      // register here editor methods

      HostLayer.LayerPositionEditorMethod = new DoubleClickHandler(EhLayerPositionEdit);
    }

    /// <summary>
    /// Edits the position of the host layer associated with the hit-test object.
    /// </summary>
    /// <param name="hit">The hit-test object that identifies the edited layer.</param>
    /// <returns><c>false</c> so the caller keeps the edited layer.</returns>
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
