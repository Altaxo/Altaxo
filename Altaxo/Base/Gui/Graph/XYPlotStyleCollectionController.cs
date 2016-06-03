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
using Altaxo.Graph.Gdi;
using Altaxo.Graph.Gdi.Plot;
using Altaxo.Graph.Gdi.Plot.Styles;
using Altaxo.Main;
using Altaxo.Main.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Altaxo.Gui.Graph
{
	#region interfaces

	/// <summary>
	/// Summary description for XYPlotStyleCollectionController.
	/// </summary>
	public interface IXYPlotStyleCollectionView
	{
		void InitializePredefinedStyles(SelectableListNodeList list);

		void InitializeStyleList(SelectableListNodeList list);

		void InitializeAvailableStyleList(SelectableListNodeList list);

		event Action RequestAddStyle;

		event Action RequestStyleUp;

		event Action RequestStyleDown;

		event Action RequestStyleEdit;

		event Action RequestStyleRemove;

		event Action PredefinedStyleSelected;
	}

	/// <summary>
	/// Summary description for XYPlotStyleCollectionController.
	/// </summary>
	public interface IXYPlotStyleCollectionController : IMVCANController
	{
		event EventHandler CollectionChangeCommit;

		/// <summary>Is fired when user selected a style for editing. The argument is the index of the style to edit.</summary>
		event Action<int> StyleEditRequested;
	}

	#endregion interfaces

	[UserControllerForObject(typeof(G2DPlotStyleCollection))]
	[ExpectedTypeOfView(typeof(IXYPlotStyleCollectionView))]
	public class XYPlotStyleCollectionController
		:
		MVCANControllerEditOriginalDocBase<G2DPlotStyleCollection, IXYPlotStyleCollectionView>,
		IXYPlotStyleCollectionController
	{
		private SelectableListNodeList _predefinedStyleSetsAvailable;
		private SelectableListNodeList _singleStylesAvailable;
		private SelectableListNodeList _currentItems;

		/// <summary>Is fired when user selected a style for editing. The argument is the index of the style to edit.</summary>
		public event Action<int> StyleEditRequested;

		public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
		{
			yield break;
		}

		public override void Dispose(bool isDisposing)
		{
			_predefinedStyleSetsAvailable = null;
			_singleStylesAvailable = null;
			_currentItems = null;
			StyleEditRequested = null;

			base.Dispose(isDisposing);
		}

		protected override void Initialize(bool initData)
		{
			base.Initialize(initData);

			if (initData)
			{
				// predefined styles
				string[] names = G2DPlotStyleCollectionTemplates.GetAvailableNames();
				_predefinedStyleSetsAvailable = new SelectableListNodeList();
				for (int i = 0; i < names.Length; ++i)
					_predefinedStyleSetsAvailable.Add(new SelectableListNode(names[i], i, false));

				// single styles
				_singleStylesAvailable = new SelectableListNodeList();
				Type[] avtypes = ReflectionService.GetNonAbstractSubclassesOf(typeof(IG2DPlotStyle));
				for (int i = 0; i < avtypes.Length; i++)
				{
					if (avtypes[i] != typeof(G2DPlotStyleCollection))
					{
						_singleStylesAvailable.Add(new SelectableListNode(Current.Gui.GetUserFriendlyClassName(avtypes[i]), avtypes[i], false));
					}
				}

				BuildCurrentStyleListNodeList();
			}

			if (null != _view)
			{
				_view.InitializePredefinedStyles(_predefinedStyleSetsAvailable);
				_view.InitializeAvailableStyleList(_singleStylesAvailable);
				_view.InitializeStyleList(_currentItems);
			}
		}

		public override bool Apply(bool disposeController)
		{
			return ApplyEnd(true, disposeController);
		}

		protected override void AttachView()
		{
			base.AttachView();

			_view.RequestAddStyle += EhView_AddStyle;
			_view.RequestStyleUp += EhView_StyleUp;
			_view.RequestStyleDown += EhView_StyleDown;
			_view.RequestStyleEdit += EhView_StyleEdit;
			_view.RequestStyleRemove += EhView_StyleRemove;
			_view.PredefinedStyleSelected += EhView_PredefinedStyleSelected;
		}

		protected override void DetachView()
		{
			_view.RequestAddStyle -= EhView_AddStyle;
			_view.RequestStyleUp -= EhView_StyleUp;
			_view.RequestStyleDown -= EhView_StyleDown;
			_view.RequestStyleEdit -= EhView_StyleEdit;
			_view.RequestStyleRemove -= EhView_StyleRemove;
			_view.PredefinedStyleSelected -= EhView_PredefinedStyleSelected;

			base.DetachView();
		}

		#region IXYPlotStyleCollectionViewEventSink Members

		private void BuildCurrentStyleListNodeList()
		{
			// current styles
			_currentItems = new SelectableListNodeList();
			for (int i = 0; i < _doc.Count; ++i)
				_currentItems.Add(new SelectableListNode(Current.Gui.GetUserFriendlyClassName(_doc[i].GetType()), _doc[i], false));
		}

		public virtual void EhView_AddStyle()
		{
			var sel = _singleStylesAvailable.FirstSelectedNode;
			if (null == sel)
				return;

			var propertyContext = Altaxo.PropertyExtensions.GetPropertyContext(_doc);
			IG2DPlotStyle style = null;
			try
			{
				style = (IG2DPlotStyle)Activator.CreateInstance((Type)sel.Tag, propertyContext); // first try with a constructor which uses a property context
			}
			catch (System.MissingMethodException)
			{
			}

			if (null == style) // if style was not constructed
				style = (IG2DPlotStyle)Activator.CreateInstance((Type)sel.Tag); // try with parameterless constructor

			IPlotArea layer = AbsoluteDocumentPath.GetRootNodeImplementing<IPlotArea>(_doc);
			G2DPlotItem plotitem = AbsoluteDocumentPath.GetRootNodeImplementing<G2DPlotItem>(_doc);
			if (layer != null && plotitem != null)
				_doc.PrepareNewSubStyle(style, layer, plotitem.GetRangesAndPoints(layer));

			_currentItems.Add<IG2DPlotStyle>(new SelectableListNode(Current.Gui.GetUserFriendlyClassName(style.GetType()), style, true), (docNodeToAdd) => _doc.Add(docNodeToAdd));

			OnCollectionChangeCommit();
		}

		public virtual void EhView_StyleUp()
		{
			_currentItems.MoveSelectedItemsUp((i, j) => _doc.ExchangeItemPositions(i, j));
			OnCollectionChangeCommit();
		}

		public virtual void EhView_StyleDown()
		{
			_currentItems.MoveSelectedItemsDown((i, j) => _doc.ExchangeItemPositions(i, j));
			OnCollectionChangeCommit();
		}

		public virtual void EhView_StyleEdit()
		{
			var idx = _currentItems.FirstSelectedNodeIndex;
			if (idx >= 0 && null != StyleEditRequested)
				StyleEditRequested(idx);
		}

		public virtual void EhView_StyleRemove()
		{
			_currentItems.RemoveSelectedItems((i, tag) => _doc.RemoveAt(i));
			OnCollectionChangeCommit();
		}

		public void EhView_PredefinedStyleSelected()
		{
			var sel = _predefinedStyleSetsAvailable.FirstSelectedNode;
			if (null == sel)
				return;

			G2DPlotStyleCollection template = G2DPlotStyleCollectionTemplates.GetTemplate((int)sel.Tag, _doc.GetPropertyContext());
			_currentItems.Clear(() => _doc.Clear());
			for (int i = 0; i < template.Count; i++)
			{
				var listNode = new SelectableListNode(Current.Gui.GetUserFriendlyClassName(template[i].GetType()), template[i], false);
				_currentItems.Add<IG2DPlotStyle>(listNode, (docNode) => _doc.Add(docNode));
			}

			OnCollectionChangeCommit();
		}

		#endregion IXYPlotStyleCollectionViewEventSink Members

		#region CollectionController Members

		public event EventHandler CollectionChangeCommit;

		public virtual void OnCollectionChangeCommit()
		{
			if (CollectionChangeCommit != null)
				CollectionChangeCommit(this, EventArgs.Empty);
		}

		#endregion CollectionController Members
	}
}