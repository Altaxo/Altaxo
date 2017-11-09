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
using System;
using System.Collections.Generic;

namespace Altaxo.Gui.Graph.Gdi.Plot
{
    #region Interfaces

    public interface IG2DPlotItemView
    {
        /// <summary>
        /// Removes all Tab pages from the dialog.
        /// </summary>
        void ClearTabs();

        /// <summary>
        /// Adds a Tab page to the dialog
        /// </summary>
        /// <param name="title">The title of the tab page.</param>
        /// <param name="view">The view (must be currently of type Control.</param>
        void AddTab(string title, object view);

        /// <summary>
        /// Activates the tab page with the title <code>title</code>.
        /// </summary>
        /// <param name="index">The index of the tab page to focus.</param>
        void BringTabToFront(int index);

        event EventHandler<InstanceChangedEventArgs> SelectedPage_Changed;

        /// <summary>
        /// Sets the plot style view, i.e. the control where we can add or remove plot styles.
        /// </summary>
        /// <param name="view"></param>
        void SetPlotStyleView(object view);

        void SetPlotGroupCollectionView(object view);
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

        /// <summary>Controller for the <see cref="PlotGroupStyleCollection"/> that is associated with the parent of this plot item.</summary>
        private PlotGroupCollectionController _plotGroupController;

        private IMVCANController _dataController; // IPlotColumnDataController
        private IXYPlotStyleCollectionController _styleCollectionController;
        private List<IMVCANController> _styleControllerList = new List<IMVCANController>();

        private Dictionary<IG2DPlotStyle, IMVCANController> _styleControllerDictionary = new Dictionary<IG2DPlotStyle, IMVCANController>();

        public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
        {
            yield return new ControllerAndSetNullMethod(_plotGroupController, () => _plotGroupController = null);
            yield return new ControllerAndSetNullMethod(_dataController, () => _dataController = null);
            yield return new ControllerAndSetNullMethod(_styleCollectionController, () =>
            {
                if (null != _styleCollectionController)
                {
                    _styleCollectionController.CollectionChangeCommit -= _styleCollectionController_CollectionChangeCommit;
                    _styleCollectionController.StyleEditRequested -= _styleCollectionController_StyleEditRequested;
                    _styleCollectionController = null;
                }
            });

            if (null != _styleControllerList)
            {
                foreach (var ctrl in _styleControllerList)
                    yield return new ControllerAndSetNullMethod(ctrl, null);

                yield return new ControllerAndSetNullMethod(null, () => _styleControllerList = null);
            }

            if (null != _styleControllerDictionary)
            {
                foreach (var ctrl in _styleControllerDictionary.Values)
                    yield return new ControllerAndSetNullMethod(ctrl, null);

                yield return new ControllerAndSetNullMethod(null, () => _styleControllerDictionary = null);
            }
        }

        /// <summary>
        /// We have to override GetSuspendTokenForControllerDocument here because we want to suspend the parent plot item collection instead of the plot item.
        /// This is because we also want to change for instance the plot groups, or distribute style changes to other plot items.
        /// </summary>
        /// <returns>
        /// The suspend token.
        /// </returns>
        protected override ISuspendToken GetSuspendTokenForControllerDocument()
        {
            if (null != _doc.ParentCollection)
                return _doc.ParentCollection.SuspendGetToken();
            else if (null != _doc)
                return _doc.SuspendGetToken();
            else
                return null;
        }

        protected override void Initialize(bool initData)
        {
            base.Initialize(initData);

            if (initData)
            {
                if (null == _groupStyles && null != _doc.ParentCollection)
                    _groupStyles = _doc.ParentCollection.GroupStyles;

                var plotGroupController = new PlotGroupCollectionController();
                plotGroupController.InitializeDocument(_groupStyles);
                plotGroupController.GroupStyleChanged += new WeakActionHandler(EhPlotGroupChanged, (handler) => plotGroupController.GroupStyleChanged -= handler);
                _plotGroupController = plotGroupController;

                // find the style collection controller
                _styleCollectionController = (IXYPlotStyleCollectionController)Current.Gui.GetControllerAndControl(new object[] { _doc.Style }, typeof(IXYPlotStyleCollectionController), UseDocument.Directly);
                _styleCollectionController.CollectionChangeCommit += new EventHandler(_styleCollectionController_CollectionChangeCommit);
                _styleCollectionController.StyleEditRequested += new Action<int>(_styleCollectionController_StyleEditRequested);

                // Initialize the data controller
                _dataController = (IMVCANController)Current.Gui.GetControllerAndControl(new object[] { _doc.DataObject, _doc }, typeof(IMVCANController), UseDocument.Directly);

                // Initialize the style controller list
                InitializeStyleControllerList();
            }

            if (null != _view)
            {
                if (null == _plotGroupController.ViewObject)
                    Current.Gui.FindAndAttachControlTo(_plotGroupController);
                _view.SetPlotGroupCollectionView(_plotGroupController.ViewObject);

                // add the style controller
                _view.SetPlotStyleView(_styleCollectionController.ViewObject);

                View_SetAllTabViews();
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

            if (null != _dataController && !_dataController.Apply(disposeController))
                return false;

            if (!_styleCollectionController.Apply(disposeController))
                return false;

            var activeSubStyleIndex = GetActiveSubStyleControlIndex();
            if (activeSubStyleIndex.HasValue)
            {
                if (false == _styleControllerList[activeSubStyleIndex.Value].Apply(false))
                {
                    _view.BringTabToFront(activeSubStyleIndex.Value);
                    applyResult = false;
                    goto end_of_function;
                }
                DistributeStyleChange(activeSubStyleIndex.Value, true);
            }

            for (int i = 0; i < _styleControllerList.Count; ++i)
            {
                if (false == _styleControllerList[i].Apply(disposeController))
                {
                    _view.BringTabToFront(i);
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
            if (_doc.ParentCollection != null)
            {
                _doc.ParentCollection.GroupStyles.CopyFrom(_groupStyles);
                _doc.ParentCollection.DistributePlotStyleFromTemplate(_doc, _groupStyles.PlotGroupStrictness);
                _doc.ParentCollection.DistributeChanges(_doc);
            }
        }

        protected override void AttachView()
        {
            base.AttachView();
            _view.SelectedPage_Changed += EhView_ActiveChildControlChanged;
        }

        protected override void DetachView()
        {
            _view.SelectedPage_Changed -= EhView_ActiveChildControlChanged;
            base.DetachView();
        }

        private SuspendableObject _disablerOfActiveChildControlChanged = new SuspendableObject();
        private object _activeChildControl;

        /// <summary>
        /// Get the index of the active sub style control, or null if the active control is not a sub style control (e.g. it is the data control).
        /// </summary>
        /// <returns></returns>
        private int? GetActiveSubStyleControlIndex()
        {
            for (int i = 0; i < _styleControllerList.Count; i++)
            {
                if (_styleControllerList[i] != null && object.ReferenceEquals(_styleControllerList[i].ViewObject, _activeChildControl))
                {
                    return i;
                }
            }
            return null;
        }

        private void View_SetAllTabViews()
        {
            using (var suspendToken = _disablerOfActiveChildControlChanged.SuspendGetToken()) // avoid firing a lot of events by adding the tab controls
            {
                _view.ClearTabs();

                // Add the data tab item
                if (_dataController != null)
                    _view.AddTab("Data", _dataController.ViewObject);

                // set the plot style tab items
                for (int i = 0; i < _styleControllerList.Count; ++i)
                {
                    string title = string.Format("#{0}: {1}", (i + 1), Current.Gui.GetUserFriendlyClassName(_doc.Style[i].GetType()));
                    _view.AddTab(title, _styleControllerList[i].ViewObject);
                }
            }
        }

        protected void EhView_ActiveChildControlChanged(object sender, InstanceChangedEventArgs e)
        {
            if (e.NewInstance != null)
                this._activeChildControl = e.NewInstance;

            if (_disablerOfActiveChildControlChanged.IsSuspended)
                return;

            // test if it is the view of the normal styles
            for (int i = 0; i < _styleControllerList.Count; i++)
            {
                if (_styleControllerList[i] != null && object.ReferenceEquals(_styleControllerList[i].ViewObject, e.OldInstance))
                {
                    if (!_styleControllerList[i].Apply(false))
                        return;

                    DistributeStyleChange(i, true);
                }
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

            IMVCANController ct = (IMVCANController)Current.Gui.GetControllerAndControl(new object[] { style }, typeof(IMVCANController), UseDocument.Directly);

            if (ct != null)
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
        }

        private IEnumerable<Tuple<string, IEnumerable<Tuple<string, IReadableColumn, string, Action<IReadableColumn, DataTable, int>>>>> GetAdditionalColumns()
        {
            for (int i = 0; i < _styleControllerList.Count; ++i)
            {
                var styleCtrl = _styleControllerList[i] as IColumnDataExternallyControlled;
                if (null == styleCtrl)
                    continue; // no data columns in this controller

                var additionalColumns = styleCtrl.GetDataColumnsExternallyControlled();

                if (null != additionalColumns)
                {
                    yield return new Tuple<string, IEnumerable<Tuple<string, IReadableColumn, string, Action<IReadableColumn, DataTable, int>>>>(
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
                    if (null != _styleControllerList[i])
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
            if (null != parColl)
            {
                parColl.DistributeChanges(parColl[0]);
            }

            // now all style controllers must be updated
            for (int i = 0; i < _styleControllerList.Count; i++)
            {
                if (null != _styleControllerList[i])
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
            if (_dataController != null)
                ++result;

            return result;
        }

        private void _styleCollectionController_CollectionChangeCommit(object sender, EventArgs e)
        {
            if (true == _styleCollectionController.Apply(false))
            {
                InitializeStyleControllerList();
                View_SetAllTabViews();
            }
        }

        private void _styleCollectionController_StyleEditRequested(int styleIndex)
        {
            if (null != _view)
            {
                _view.BringTabToFront(styleIndex + GetFirstStyleTabIndex());
            }
        }
    }
}