#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2022 Dr. Dirk Lellinger
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

#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using Altaxo.Collections;
using Altaxo.Data;
using Altaxo.Graph.Gdi;
using Altaxo.Graph.Gdi.Plot;
using Altaxo.Graph.Gdi.Plot.Groups;
using Altaxo.Graph.Gdi.Plot.Styles;
using Altaxo.Gui.Data;
using Altaxo.Gui.Graph.Gdi.Plot.Groups;
using Altaxo.Gui.Graph.Gdi.Plot.Styles;
using Altaxo.Gui.Graph.Plot.Data;
using Altaxo.Main;

namespace Altaxo.Gui.Graph.Gdi.Plot
{
  #region Interfaces

  public interface IG2DPlotItemView : IDataContextAwareView
  {
  }

  #endregion Interfaces

  /// <summary>
  /// Summary description for XYColumnPlotItemController.
  /// </summary>
  [UserControllerForObject(typeof(G2DPlotItem))]
  [ExpectedTypeOfView(typeof(IG2DPlotItemView))]
  public class G2DPlotItemController : MVCANControllerEditOriginalDocBase<G2DPlotItem, IG2DPlotItemView>
  {
    private int _applySuspend; // to avoid multiple invoking here because some of the child controls

    private PlotGroupStyleCollection _groupStyles;


    private IMVCANController _dataController; // IPlotColumnDataController
    private List<IMVCANController> _styleControllerList = new List<IMVCANController>();

    private Dictionary<IG2DPlotStyle, IMVCANController> _styleControllerDictionary = new Dictionary<IG2DPlotStyle, IMVCANController>();

    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield return new ControllerAndSetNullMethod(_plotGroupController, () => _plotGroupController = null);
      yield return new ControllerAndSetNullMethod(_dataController, () => _dataController = null);
      yield return new ControllerAndSetNullMethod(_styleCollectionController, () => StyleCollectionController = null);

      if (_styleControllerList is not null)
      {
        foreach (var ctrl in _styleControllerList)
          yield return new ControllerAndSetNullMethod(ctrl, null);

        yield return new ControllerAndSetNullMethod(null, () => _styleControllerList = null);
      }

      if (_styleControllerDictionary is not null)
      {
        foreach (var ctrl in _styleControllerDictionary.Values)
          yield return new ControllerAndSetNullMethod(ctrl, null);

        yield return new ControllerAndSetNullMethod(null, () => _styleControllerDictionary = null);
      }
    }

    #region Bindings


    private IXYPlotStyleCollectionController _styleCollectionController;

    public IXYPlotStyleCollectionController StyleCollectionController
    {
      get => _styleCollectionController;
      set
      {
        if (!(_styleCollectionController == value))
        {
          if (_styleCollectionController is not null)
          {
            _styleCollectionController.CollectionChangeCommit -= EhStyleCollectionController_CollectionChangeCommit;
            _styleCollectionController.StyleEditRequested -= EhStyleCollectionController_StyleEditRequested;
          }

          _styleCollectionController?.Dispose();
          _styleCollectionController = value;

          if (_styleCollectionController is not null)
          {
            _styleCollectionController.CollectionChangeCommit += EhStyleCollectionController_CollectionChangeCommit;
            _styleCollectionController.StyleEditRequested += EhStyleCollectionController_StyleEditRequested;
          }

          OnPropertyChanged(nameof(StyleCollectionController));
        }
      }
    }


    /// <summary>Controller for the <see cref="PlotGroupStyleCollection"/> that is associated with the parent of this plot item.</summary>
    private PlotGroupCollectionController _plotGroupController;

    public PlotGroupCollectionController PlotGroupController
    {
      get => _plotGroupController;
      set
      {
        if (!(_plotGroupController == value))
        {
          _plotGroupController?.Dispose();
          _plotGroupController = value;
          OnPropertyChanged(nameof(PlotGroupController));
        }
      }
    }

    public SelectableListNodeList Tabs { get; } = new();

    private int? _selectedTab;

    /// <summary>
    /// Gets or sets the selected tab. The value of -1 selectes the data tab, values &gt;= 0 select one of the style tabs.
    /// </summary>
    /// <value>
    /// The selected tab.
    /// </value>
    public int? SelectedTab
    {
      get => _selectedTab;
      set
      {
        if (!(_selectedTab == value))
        {
          var oldValue = _selectedTab;
          _selectedTab = value;
          OnPropertyChanged(nameof(SelectedTab));
          EhView_ActiveChildControlChanged(value, oldValue);
        }
      }
    }

    public IMVCAController? GetControllerFromTag(int? tag)
    {
        if (tag is { } selIndex && Tabs is { } tabs)
        {
          var node = (SelectableListNodeWithController)Tabs.FirstOrDefault(tab => selIndex.Equals(tab.Tag));
          return node?.Controller;
        }
        else
        {
          return null;
        }
      }



    #endregion

    /// <summary>
    /// We have to override GetSuspendTokenForControllerDocument here because we want to suspend the parent plot item collection instead of the plot item.
    /// This is because we also want to change for instance the plot groups, or distribute style changes to other plot items.
    /// </summary>
    /// <returns>
    /// The suspend token.
    /// </returns>
    protected override ISuspendToken GetSuspendTokenForControllerDocument()
    {
      if (_doc.ParentCollection is not null)
        return _doc.ParentCollection.SuspendGetToken();
      else if (_doc is not null)
        return _doc.SuspendGetToken();
      else
        return null;
    }

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        if (_groupStyles is null && _doc.ParentCollection is not null)
          _groupStyles = _doc.ParentCollection.GroupStyles;

        var plotGroupController = new PlotGroupCollectionController();
        plotGroupController.InitializeDocument(_groupStyles);
        plotGroupController.GroupStyleChanged += new WeakActionHandler(EhPlotGroupChanged, plotGroupController, nameof(plotGroupController.GroupStyleChanged));
        PlotGroupController = plotGroupController;
        if (_plotGroupController.ViewObject is null)
          Current.Gui.FindAndAttachControlTo(_plotGroupController);

        // find the style collection controller
        StyleCollectionController = (IXYPlotStyleCollectionController)Current.Gui.GetControllerAndControl(new object[] { _doc.Style }, typeof(IXYPlotStyleCollectionController), UseDocument.Directly);

        // Initialize the data controller
        _dataController = (IMVCANController)Current.Gui.GetControllerAndControl(new object[] { _doc.DataObject, _doc }, typeof(IMVCANController), UseDocument.Directly);

        // Initialize the style controller list
        InitializeStyleControllerList();
        SelectedTab = -1; // select the data tab by default
      }
    }

    // have this here as controller too
    public override bool Apply(bool disposeController)
    {
      if (_applySuspend++ > 0)
      {
        _applySuspend--;
        return true;
      }

      bool applyResult = false;

      if (_dataController is not null && !_dataController.Apply(disposeController))
        return false;

      if (!_styleCollectionController.Apply(disposeController))
        return false;

      var activeSubStyleIndex = SelectedTab;
      if (activeSubStyleIndex.HasValue && activeSubStyleIndex.Value >=0)
      {
        if (false == _styleControllerList[activeSubStyleIndex.Value].Apply(false))
        {
          BringTabToFront(activeSubStyleIndex.Value);
          applyResult = false;
          goto end_of_function;
        }
        DistributeStyleChange(activeSubStyleIndex.Value, true);
      }

      for (int i = 0; i < _styleControllerList.Count; ++i)
      {
        if (false == _styleControllerList[i].Apply(disposeController))
        {
          BringTabToFront(i);
          applyResult = false;
          goto end_of_function;
        }
      }

      ApplyPlotGroupView(disposeController);

      applyResult = true;

end_of_function:
      _applySuspend--;

      return ApplyEnd(applyResult, disposeController);
    }

    private void ApplyPlotGroupView(bool disposeController)
    {
      _plotGroupController.Apply(disposeController);
      _groupStyles.CopyFrom((PlotGroupStyleCollection)_plotGroupController.ModelObject);

      // now distribute the new style to the other plot items
      if (_doc.ParentCollection is not null)
      {
        _doc.ParentCollection.GroupStyles.CopyFrom(_groupStyles);
        _doc.ParentCollection.DistributePlotStyleFromTemplate(_doc, _groupStyles.PlotGroupStrictness);
        _doc.ParentCollection.DistributeChanges(_doc);
      }
    }

    private SuspendableObject _disablerOfActiveChildControlChanged = new SuspendableObject();
   

    protected void EhView_ActiveChildControlChanged(int? selectedTab, int? oldSelectedTab)
    {
      if (_disablerOfActiveChildControlChanged.IsSuspended)
        return;


      // test if it is the view of the normal styles
      if (oldSelectedTab.HasValue && oldSelectedTab >= 0 && GetControllerFromTag(oldSelectedTab) is { } oldStyleController)
      {
            if (!oldStyleController.Apply(false))
              return;

            DistributeStyleChange(oldSelectedTab.Value, true);
        }

      if (_dataController is IPlotColumnDataController)
        ((IPlotColumnDataController)_dataController).SetAdditionalPlotColumns(GetAdditionalColumns()); // update list in case it has changed
    }

    /// <summary>
    /// Gets the controller for a certain style instance from either the dictionary, or if not found, by creating a new instance.
    /// </summary>
    /// <param name="style"></param>
    /// <returns></returns>
    private IMVCANController GetStyleController(IG2DPlotStyle style)
    {
      if (_styleControllerDictionary.ContainsKey(style))
        return _styleControllerDictionary[style];

      var ct = (IMVCANController)Current.Gui.GetControllerAndControl(new object[] { style }, typeof(IMVCANController), UseDocument.Directly);

      if (ct is not null)
        _styleControllerDictionary.Add(style, ct);

      return ct;
    }

    private void InitializeStyleControllerList()
    {
      // Clear the previous controller cache
      _styleControllerList.Clear();

      // start to create new controllers
      for (int i = 0; i < _doc.Style.Count; i++)
      {
        IMVCANController ctrl = GetStyleController(_doc.Style[i]);
        _styleControllerList.Add(ctrl);
      }

      if (_dataController is IPlotColumnDataController)
        ((IPlotColumnDataController)_dataController).SetAdditionalPlotColumns(GetAdditionalColumns());


      using (var suspendToken = _disablerOfActiveChildControlChanged.SuspendGetToken()) // avoid firing a lot of events by adding the tab controls
      {
        Tabs.Clear();

        // Add the data tab item
        if (_dataController is not null)
        {
          Tabs.Add(new SelectableListNodeWithController("Scale", -1, false) { Controller = _dataController });
        }

        // set the plot style tab items
        for (int i = 0; i < _styleControllerList.Count; ++i)
        {
          string title = $"#{(i + 1)}: {Current.Gui.GetUserFriendlyClassName(_doc.Style[i].GetType())}";
          Tabs.Add(new SelectableListNodeWithController(title, i, false) { Controller = _styleControllerList[i] });
        }
      }
    }


    private IEnumerable<(string ColumnGroupNumberAndName, IEnumerable<(string ColumnLabel, IReadableColumn Column, string ColumnName, Action<IReadableColumn, DataTable, int> ColumnSetAction)> ColumnInfos)> GetAdditionalColumns()
    {
      for (int i = 0; i < _styleControllerList.Count; ++i)
      {
        var styleCtrl = _styleControllerList[i] as IColumnDataExternallyControlled;
        if (styleCtrl is null)
          continue; // no data columns in this controller

        var additionalColumns = styleCtrl.GetDataColumnsExternallyControlled();

        if (additionalColumns is not null)
        {
          yield return (
              string.Format("#{0}: {1}", i + 1, Current.Gui.GetUserFriendlyClassName(_doc.Style[i].GetType())),
              additionalColumns);
        }
      }
    }

    /// <summary>
    /// This distributes changes made to one of the sub plot styles to all other plot styles. Additionally, the controller
    /// for this styles are also updated.
    /// </summary>
    /// <param name="pivotelement"></param>
    /// <param name="updateAllStyleControllers">If true, the style controllers are newly initialized. Set this parameter to false if this is unneccessary, e.g.
    /// when applying the controller and closing it afterwards.</param>
    private void DistributeStyleChange(int pivotelement, bool updateAllStyleControllers)
    {
      IPlotArea layer = AbsoluteDocumentPath.GetRootNodeImplementing<IPlotArea>(_doc);
      _doc.Style.DistributeSubStyleChange(pivotelement, layer, _doc.GetRangesAndPoints(layer));

      if (updateAllStyleControllers)
      {
        // now all style controllers must be updated
        for (int i = 0; i < _styleControllerList.Count; i++)
        {
          if (_styleControllerList[i] is not null)
            _styleControllerList[i].InitializeDocument(_doc.Style[i], (_doc.DataObject as Altaxo.Graph.Plot.Data.XYZColumnPlotData)?.DataTable, (_doc.DataObject as Altaxo.Graph.Plot.Data.XYZColumnPlotData)?.GroupNumber ?? 0);
        }
      }
    }

    /// <summary>
    /// Is called when the user has made a major change to the plot groups.
    /// </summary>
    /// <remarks>In this case probably one of the lists in the plot group has changed. Thus the first item of the plot item collection is used as
    /// pivot element to distribute the style changes.</remarks>
    private void EhPlotGroupChanged()
    {
      var parColl = _doc.ParentCollection;
      if (parColl is not null)
      {
        parColl.DistributeChanges(parColl[0]);
      }

      // now all style controllers must be updated
      for (int i = 0; i < _styleControllerList.Count; i++)
      {
        if (_styleControllerList[i] is not null)
          _styleControllerList[i].InitializeDocument(_doc.Style[i], (_doc.DataObject as Altaxo.Graph.Plot.Data.XYZColumnPlotData)?.DataTable);
      }
    }

    /// <summary>
    /// Returns the tab index of the first style that is shown.
    /// </summary>
    /// <returns>Tab index of the first shown style.</returns>
    private int GetFirstStyleTabIndex()
    {
      int result = 0;
      if (_dataController is not null)
        ++result;

      return result;
    }

    private void EhStyleCollectionController_CollectionChangeCommit(object sender, EventArgs e)
    {
      if (true == _styleCollectionController.Apply(false))
      {
        InitializeStyleControllerList();
      }
    }

    private void EhStyleCollectionController_StyleEditRequested(int styleIndex)
    {
      BringTabToFront(styleIndex);
    }

    protected void BringTabToFront(int styleIndex)
    {
      SelectedTab = styleIndex;
    }
  }
}
