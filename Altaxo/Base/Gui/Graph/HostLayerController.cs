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
using Altaxo.Gui;
using Altaxo.Gui.Common;
using Altaxo.Collections;

namespace Altaxo.Gui.Graph
{
	#region Interfaces


	public interface IHostLayerView
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

	#endregion

	/// <summary>
	/// Summary description for LayerController.
	/// </summary>
	[UserControllerForObject(typeof(HostLayer))]
	[ExpectedTypeOfView(typeof(IHostLayerView))]
	public class HostLayerController : MVCANControllerBase<HostLayer, IHostLayerView>
	{
		protected IDisposable _docSuspendLock;

		private string _currentPageName;

		IMVCAController _currentController;

		protected IMVCAController _layerPositionController;
		protected IMVCANController _layerContentsController;

		object _lastControllerApplied;

		SelectableListNodeList _listOfUniqueItem;



		public HostLayerController(HostLayer layer)
			: this(layer, "Position")
		{
		}

		HostLayerController(HostLayer layer, string currentPage)
		{
			_originalDoc = layer;
			_doc = (XYPlotLayer)layer.Clone();

			_currentPageName = currentPage;

			Initialize(true);
		}

		protected override void AttachView()
		{
			_view.TabValidating += EhView_TabValidating;
			_view.PageChanged += EhView_PageChanged;
		}

		protected override void DetachView()
		{
			_view.TabValidating -= EhView_TabValidating;
			_view.PageChanged -= EhView_PageChanged;
		}

		public override bool Apply()
		{
			ApplyCurrentController(true);

			_originalDoc.CopyFrom(_doc, GraphCopyOptions.All); // _doc remains suspended

			return true;
		}

		protected override void Initialize(bool initData)
		{
			if (initData)
			{
				_docSuspendLock = _doc.BeginUpdate();

				_listOfUniqueItem = new SelectableListNodeList();
				_listOfUniqueItem.Add(new SelectableListNode("Common", null, true));
			}

			if (null != _view)
			{
				// add all necessary Tabs
				_view.AddTab("Contents", "Contents");
				_view.AddTab("Position", "Position");

				// Set the controller of the current visible Tab
				SetCurrentTabController(true);
			}
		}


		

		void SetCurrentTabController(bool pageChanged)
		{
			switch (_currentPageName)
			{
				case "Contents":
					if (pageChanged)
					{
						_view.SelectTab(_currentPageName);
					}
					if (null == _layerContentsController)
					{
						// _layerContentsController = (IMVCANController)Current.Gui.GetControllerAndControl(new object[] { _doc.PlotItems }, typeof(IMVCANController), UseDocument.Directly);
					}
					_currentController = _layerContentsController;
					_view.CurrentContent = _currentController.ViewObject;
					break;
				case "Position":
					if (pageChanged)
					{
						_view.SelectTab(_currentPageName);
					}
					if (null == _layerPositionController)
					{
						_layerPositionController = new LayerPositionController(_doc);
						Current.Gui.FindAndAttachControlTo(_layerPositionController);
					}
					_currentController = _layerPositionController;
					_view.CurrentContent = _layerPositionController.ViewObject;
					break;


			

			}
		}


	

		public void EhView_PageChanged(string firstChoice)
		{
			ApplyCurrentController(false);

			_currentPageName = firstChoice;
			SetCurrentTabController(true);
		}

	

		void EhView_TabValidating(object sender, CancelEventArgs e)
		{
			if (!ApplyCurrentController(true))
				e.Cancel = true;
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

		

			return true;
		}

		#region Dialog

		public static bool ShowDialog(HostLayer layer)
		{
			return ShowDialog(layer, "Position");
		}

		public static bool ShowDialog(HostLayer layer, string currentPage)
		{
			HostLayerController ctrl = new HostLayerController(layer, currentPage);
			return Current.Gui.ShowDialog(ctrl, layer.Name, true);
		}


		#endregion

		#region Edit Handlers

		public static void RegisterEditHandlers()
		{
			// register here editor methods

			HostLayer.LayerPositionEditorMethod = new DoubleClickHandler(EhLayerPositionEdit);

		}

		public static bool EhLayerPositionEdit(IHitTestObject hit)
		{
			XYPlotLayer layer = hit.HittedObject as XYPlotLayer;
			if (layer == null)
				return false;

			ShowDialog(layer, "Position");

			return false;
		}


		#endregion


	}
}
