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

using Altaxo.Graph.Gdi;
using Altaxo.Graph.Gdi.Plot;
using Altaxo.Graph.Gdi.Plot.Groups;
using Altaxo.Graph.Gdi.Plot.Styles;
using Altaxo.Main;
using System;
using System.Collections.Generic;

namespace Altaxo.Gui.Graph
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
		private IMVCANController _plotGroupController;

		private IMVCAController _dataController;
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

		protected override void Initialize(bool initData)
		{
			base.Initialize(initData);

			if (initData)
			{
				if (null == _groupStyles && null != _doc.ParentCollection)
					_groupStyles = _doc.ParentCollection.GroupStyles;

				_plotGroupController = new PlotGroupCollectionController();
				_plotGroupController.InitializeDocument(_groupStyles);

				// find the style collection controller
				_styleCollectionController = (IXYPlotStyleCollectionController)Current.Gui.GetControllerAndControl(new object[] { _doc.Style }, typeof(IXYPlotStyleCollectionController), UseDocument.Directly);
				_styleCollectionController.CollectionChangeCommit += new EventHandler(_styleCollectionController_CollectionChangeCommit);
				_styleCollectionController.StyleEditRequested += new Action<int>(_styleCollectionController_StyleEditRequested);

				// Initialize the data controller
				_dataController = (IMVCAController)Current.Gui.GetControllerAndControl(new object[] { _doc.DataObject, _doc }, typeof(IMVCAController), UseDocument.Directly);

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

			if (!_dataController.Apply(disposeController))
				return false;

			if (!_styleCollectionController.Apply(disposeController))
				return false;

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

		private void View_SetAllTabViews()
		{
			_view.ClearTabs();

			// Add the data tab item
			if (_dataController != null)
				_view.AddTab("Data", _dataController.ViewObject);

			// set the plot style tab items
			for (int i = 0; i < _styleControllerList.Count; ++i)
			{
				string title = string.Format("#{0}:{1}", (i + 1), Current.Gui.GetUserFriendlyClassName(_doc.Style[i].GetType()));
				_view.AddTab(title, _styleControllerList[i].ViewObject);
			}
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
		}

		protected void EhView_ActiveChildControlChanged(object sender, InstanceChangedEventArgs e)
		{
			// test if it is the view of the normal styles
			for (int i = 0; i < _styleControllerList.Count; i++)
			{
				if (_styleControllerList[i] != null && object.ReferenceEquals(_styleControllerList[i].ViewObject, e.OldInstance))
				{
					if (!_styleControllerList[i].Apply(false))
						return;

					DistributeStyleChange(i);
				}
			}
		}

		/// <summary>
		/// This distributes changes made to one of the sub plot styles to all other plot styles. Additionally, the controller
		/// for this styles are also updated.
		/// </summary>
		/// <param name="pivotelement"></param>
		private void DistributeStyleChange(int pivotelement)
		{
			IPlotArea layer = AbsoluteDocumentPath.GetRootNodeImplementing<IPlotArea>(_doc);
			_doc.Style.DistributeSubStyleChange(pivotelement, layer, _doc.GetRangesAndPoints(layer));

			// now all style controllers must be updated
			for (int i = 0; i < _styleControllerList.Count; i++)
			{
				if (null != _styleControllerList[i])
					_styleControllerList[i].InitializeDocument(_doc.Style[i]);
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