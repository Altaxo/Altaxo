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
using Altaxo.Graph.Gdi.Plot;
using Altaxo.Graph.Gdi.Plot.Groups;
using Altaxo.Graph.Plot.Groups;
using Altaxo.Main.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Altaxo.Gui.Graph.Gdi.Plot.Groups
{
	#region Interfaces

	public interface IPlotGroupCollectionViewAdvanced
	{
		void InitializeAvailableCoordinateTransformingGroupStyles(SelectableListNodeList list);

		void InitializeAvailableNormalGroupStyles(SelectableListNodeList list);

		void InitializeUpdateMode(SelectableListNodeList list, bool inheritFromParent, bool distributeToChilds);

		void InitializeCurrentNormalGroupStyles(CheckableSelectableListNodeList list);

		void SynchronizeCurrentNormalGroupStyles();

		void QueryUpdateMode(out bool inheritFromParent, out bool distributeToChilds);

		event Action CoordinateTransformingGroupStyleChanged;

		event Action RequestCoordinateTransformingGroupStyleEdit;

		event Action RequestAddNormalGroupStyle;

		event Action RequestRemoveNormalGroupStyle;

		event Action RequestIndentGroupStyle;

		event Action RequestUnindentGroupStyle;

		event Action RequestMoveUpGroupStyle;

		event Action RequestMoveDownGroupStyle;
	}

	#endregion Interfaces

	[UserControllerForObject(typeof(PlotGroupStyleCollection))]
	[ExpectedTypeOfView(typeof(IPlotGroupCollectionViewAdvanced))]
	public class PlotGroupCollectionControllerAdvanced
		:
		MVCANControllerEditOriginalDocBase<PlotGroupStyleCollection, IPlotGroupCollectionViewAdvanced>

	{
		#region Inner classes

		private class MyListNode : CheckableSelectableListNode
		{
			public MyListNode(string name, object item, bool isSelected, bool isChecked, bool isCheckBoxVisible)
				: base(name, item, isSelected, isChecked)
			{
				this.IsCheckBoxVisible = isCheckBoxVisible;
			}

			public bool IsCheckBoxVisible { get; set; }
		}

		#endregion Inner classes

		private IGPlotItem _parent; // usually the parent is the PlotItemCollection

		private SelectableListNodeList _availableTransfoStyles;
		private SelectableListNodeList _availableNormalStyles;
		private CheckableSelectableListNodeList _currentNormalStyles;
		private SelectableListNodeList _availableUpdateModes;
		private ICoordinateTransformingGroupStyle _currentTransfoStyle;

		/// <summary>
		/// Number of items where the property <see cref="IPlotGroupStyle.CanCarryOver"/> is true. The list of items is maintained in the way, that those items appear first in the list.
		/// </summary>
		private int _currentNoOfItemsThatCanHaveChilds;

		public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
		{
			yield break;
		}

		public override void Dispose(bool isDisposing)
		{
			_parent = null;
			_availableTransfoStyles = null;
			_availableNormalStyles = null;
			_currentNormalStyles = null;
			_availableUpdateModes = null;
			_currentTransfoStyle = null;

			base.Dispose(isDisposing);
		}

		public override bool InitializeDocument(params object[] args)
		{
			if (args != null && args.Length > 1 && args[1] is IGPlotItem)
				_parent = (IGPlotItem)args[1];

			return base.InitializeDocument(args);
		}

		protected override void Initialize(bool initData)
		{
			base.Initialize(initData);

			if (initData)
			{
				// available Update modes
				_availableUpdateModes = new SelectableListNodeList();
				foreach (object obj in Enum.GetValues(typeof(PlotGroupStrictness)))
					_availableUpdateModes.Add(new SelectableListNode(obj.ToString(), obj, ((PlotGroupStrictness)obj) == PlotGroupStrictness.Normal));

				Type[] types;
				// Transfo-Styles
				_currentTransfoStyle = _doc.CoordinateTransformingStyle == null ? null : (ICoordinateTransformingGroupStyle)_doc.CoordinateTransformingStyle.Clone();
				_availableTransfoStyles = new SelectableListNodeList();
				_availableTransfoStyles.Add(new SelectableListNode("None", null, null == _currentTransfoStyle));
				types = ReflectionService.GetNonAbstractSubclassesOf(typeof(ICoordinateTransformingGroupStyle));
				foreach (Type t in types)
				{
					Type currentStyleType = _currentTransfoStyle == null ? null : _currentTransfoStyle.GetType();
					ICoordinateTransformingGroupStyle style;
					if (t == currentStyleType)
						style = _currentTransfoStyle;
					else
						style = (ICoordinateTransformingGroupStyle)Activator.CreateInstance(t);

					_availableTransfoStyles.Add(new SelectableListNode(Current.Gui.GetUserFriendlyClassName(t), style, t == currentStyleType));
				}

				// Normal Styles
				_availableNormalStyles = new SelectableListNodeList();
				if (_parent != null) // if possible, collect only those styles that are applicable
				{
					PlotGroupStyleCollection avstyles = new PlotGroupStyleCollection();
					_parent.CollectStyles(avstyles);
					foreach (IPlotGroupStyle style in avstyles)
					{
						if (!_doc.ContainsType(style.GetType()))
							_availableNormalStyles.Add(new SelectableListNode(Current.Gui.GetUserFriendlyClassName(style.GetType()), style.GetType(), false));
					}
				}
				else // or else, find all available styles
				{
					types = ReflectionService.GetNonAbstractSubclassesOf(typeof(IPlotGroupStyle));
					foreach (Type t in types)
					{
						if (!_doc.ContainsType(t))
							_availableNormalStyles.Add(new SelectableListNode(Current.Gui.GetUserFriendlyClassName(t), t, false));
					}
				}

				var list = _doc.GetOrderedListOfItems(ComparePlotGroupStyles);
				_currentNormalStyles = new CheckableSelectableListNodeList();
				_currentNoOfItemsThatCanHaveChilds = 0;
				foreach (var item in list)
				{
					if (item.CanCarryOver)
						++_currentNoOfItemsThatCanHaveChilds;

					var node = new MyListNode(Current.Gui.GetUserFriendlyClassName(item.GetType()), item.GetType(), false, item.IsStepEnabled, item.CanStep);

					_currentNormalStyles.Add(node);
				}
				UpdateCurrentNormalIndentation();
			}

			if (_view != null)
			{
				_view.InitializeAvailableCoordinateTransformingGroupStyles(_availableTransfoStyles);
				_view.InitializeAvailableNormalGroupStyles(_availableNormalStyles);
				_view.InitializeCurrentNormalGroupStyles(_currentNormalStyles);
				_view.InitializeUpdateMode(_availableUpdateModes, _doc.InheritFromParentGroups, _doc.DistributeToChildGroups);
			}
		}

		public override bool Apply(bool disposeController)
		{
			foreach (SelectableListNode node in _availableTransfoStyles)
			{
				if (node.IsSelected)
				{
					_currentTransfoStyle = (ICoordinateTransformingGroupStyle)node.Tag;
					_doc.CoordinateTransformingStyle = _currentTransfoStyle;
					break;
				}
			}

			_view.SynchronizeCurrentNormalGroupStyles(); // synchronize the checked state of the items
			foreach (CheckableSelectableListNode node in _currentNormalStyles)
			{
				IPlotGroupStyle style = _doc.GetPlotGroupStyle((Type)node.Tag);
				style.IsStepEnabled = node.IsChecked;
			}

			bool inherit, distribute;
			_view.QueryUpdateMode(out inherit, out distribute);
			_doc.InheritFromParentGroups = inherit;
			_doc.DistributeToChildGroups = distribute;
			foreach (SelectableListNode node in _availableUpdateModes)
			{
				if (node.IsSelected)
				{
					_doc.PlotGroupStrictness = (PlotGroupStrictness)node.Tag;
					break;
				}
			}

			return ApplyEnd(true, disposeController);
		}

		protected override void AttachView()
		{
			base.AttachView();

			_view.CoordinateTransformingGroupStyleChanged += EhView_CoordinateTransformingGroupStyleChanged;

			_view.RequestCoordinateTransformingGroupStyleEdit += EhView_CoordinateTransformingGroupStyleEdit;

			_view.RequestAddNormalGroupStyle += EhView_AddNormalGroupStyle;

			_view.RequestRemoveNormalGroupStyle += EhView_RemoveNormalGroupStyle;

			_view.RequestIndentGroupStyle += EhView_IndentGroupStyle;

			_view.RequestUnindentGroupStyle += EhView_UnindentGroupStyle;

			_view.RequestMoveUpGroupStyle += EhView_MoveUpGroupStyle;

			_view.RequestMoveDownGroupStyle += EhView_MoveDownGroupStyle;
		}

		protected override void DetachView()
		{
			_view.CoordinateTransformingGroupStyleChanged -= EhView_CoordinateTransformingGroupStyleChanged;

			_view.RequestCoordinateTransformingGroupStyleEdit -= EhView_CoordinateTransformingGroupStyleEdit;

			_view.RequestAddNormalGroupStyle -= EhView_AddNormalGroupStyle;

			_view.RequestRemoveNormalGroupStyle -= EhView_RemoveNormalGroupStyle;

			_view.RequestIndentGroupStyle -= EhView_IndentGroupStyle;

			_view.RequestUnindentGroupStyle -= EhView_UnindentGroupStyle;

			_view.RequestMoveUpGroupStyle -= EhView_MoveUpGroupStyle;

			_view.RequestMoveDownGroupStyle -= EhView_MoveDownGroupStyle;

			base.DetachView();
		}

		/// <summary>
		/// Comparison of plot group styles. Primarily they are sorted by the flag <see cref="IPlotGroupStyle.CanCarryOver"/>, so that items that can not have childs appear
		/// later in the list. Secondly, the items that can step appear earlier in the list.  Thirdly, the items are sorted by their parent-child relationship, and finally, by their name.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		private int ComparePlotGroupStyles(IPlotGroupStyle x, IPlotGroupStyle y)
		{
			if (x.CanCarryOver != y.CanCarryOver)
				return x.CanCarryOver ? -1 : 1;
			if (x.CanStep != y.CanStep)
				return x.CanStep ? -1 : 1;
			else
				return string.Compare(Current.Gui.GetUserFriendlyClassName(x.GetType()), Current.Gui.GetUserFriendlyClassName(y.GetType()));
		}

		/// <summary>
		/// Prepends the names in the item list with spaces according to their tree level.
		/// </summary>
		private void UpdateCurrentNormalIndentation()
		{
			foreach (var item in _currentNormalStyles)
			{
				int level = _doc.GetTreeLevelOf((Type)item.Tag);
				StringBuilder stb = new StringBuilder();
				stb.Append(' ', level * 3);
				stb.Append(item.Text.Trim());
				item.Text = stb.ToString();
			}
		}

		/// <summary>
		/// This updates the list, presuming that the number of items has not changed.
		/// </summary>
		private void UpdateCurrentNormalOrder()
		{
			// if possible, we try to maintain the order in the list in which the items
			// appear

			if (0 == _currentNoOfItemsThatCanHaveChilds)
				return; // then there is nothing to do now

			IPlotGroupStyle previousStyle = null;
			IPlotGroupStyle style = null;
			for (int i = 0; i < _currentNoOfItemsThatCanHaveChilds; i++, previousStyle = style)
			{
				CheckableSelectableListNode node = _currentNormalStyles[i];
				style = _doc.GetPlotGroupStyle((Type)node.Tag);

				if (previousStyle != null)
				{
					Type prevchildtype = _doc.GetTypeOfChild(previousStyle.GetType());
					if (prevchildtype != null)
					{
						if (prevchildtype != style.GetType())
						{
							int pi = _currentNormalStyles.IndexOfObject(prevchildtype);
							_currentNormalStyles.Exchange(i, pi);
						}
						continue;
					}
				}

				Type parenttype = _doc.GetParentTypeOf(style.GetType());
				if (parenttype != null &&
					(previousStyle == null || previousStyle.GetType() != parenttype))
				{
					int pi = _currentNormalStyles.IndexOfObject(parenttype);
					_currentNormalStyles.Exchange(i, pi);
				}
			}
			UpdateCurrentNormalIndentation();
		}

		#region IPlotGroupCollectionViewEventSink Members

		public void EhView_CoordinateTransformingGroupStyleChanged()
		{
			foreach (SelectableListNode node in _availableTransfoStyles)
			{
				if (node.IsSelected)
				{
					_currentTransfoStyle = (ICoordinateTransformingGroupStyle)node.Tag;
					break;
				}
			}
		}

		public void EhView_CoordinateTransformingGroupStyleEdit()
		{
			// look wheter there is a appropriate edit dialog available for the group style
			foreach (SelectableListNode node in _availableTransfoStyles)
			{
				if (node.IsSelected)
				{
					_currentTransfoStyle = (ICoordinateTransformingGroupStyle)node.Tag;
					break;
				}
			}

			if (null != _currentTransfoStyle)
				Current.Gui.ShowDialog(new object[] { _currentTransfoStyle }, "Edit transformation style");
		}

		public void EhView_AddNormalGroupStyle()
		{
			SelectableListNode selected = null;
			foreach (SelectableListNode node in _availableNormalStyles)
			{
				if (node.IsSelected)
				{
					selected = node;
					break;
				}
			}
			if (null != selected)
			{
				_availableNormalStyles.Remove(selected);

				IPlotGroupStyle s = (IPlotGroupStyle)Activator.CreateInstance((Type)selected.Tag);
				_doc.Add(s);
				var node = new MyListNode(
					Current.Gui.GetUserFriendlyClassName(s.GetType()),
					s.GetType(), true, s.IsStepEnabled, s.CanStep);
				if (s.CanCarryOver)
				{
					_currentNormalStyles.Insert(_currentNoOfItemsThatCanHaveChilds, node);
					_currentNoOfItemsThatCanHaveChilds++;
				}
				else
				{
					_currentNormalStyles.Add(node);
				}

				_view.InitializeAvailableNormalGroupStyles(_availableNormalStyles);
				_view.InitializeCurrentNormalGroupStyles(_currentNormalStyles);
			}
		}

		public void EhView_RemoveNormalGroupStyle()
		{
			for (int i = _currentNormalStyles.Count - 1; i >= 0; i--)
			{
				CheckableSelectableListNode selected = _currentNormalStyles[i];
				if (!selected.IsSelected)
					continue;

				_doc.RemoveType((Type)selected.Tag);

				_currentNormalStyles.RemoveAt(i);
				if (i < _currentNoOfItemsThatCanHaveChilds)
					_currentNoOfItemsThatCanHaveChilds--;

				_availableNormalStyles.Add(new SelectableListNode(
					Current.Gui.GetUserFriendlyClassName((Type)selected.Tag),
					selected.Tag,
					true));
			}

			UpdateCurrentNormalIndentation();
			_view.InitializeAvailableNormalGroupStyles(_availableNormalStyles);
			_view.InitializeCurrentNormalGroupStyles(_currentNormalStyles);
		}

		public void EhView_IndentGroupStyle()
		{
			// for all selected items: append it as child to the item upward
			int count = Math.Min(_currentNoOfItemsThatCanHaveChilds + 1, _currentNormalStyles.Count); // note: the first item that can step, but can not have childs, could also be indented, thats why 1+
			for (int i = 1; i < count; ++i)
			{
				CheckableSelectableListNode selected = _currentNormalStyles[i];
				if (!selected.IsSelected)
					continue;

				if (null != _doc.GetParentTypeOf((Type)selected.Tag))
					continue; // only ident those items who dont have a parent

				IPlotGroupStyle style = _doc.GetPlotGroupStyle((Type)selected.Tag);
				_doc.RemoveType(style.GetType()); // Removing the type so removing also the parent-child-relationship
				_doc.Add(style, (Type)_currentNormalStyles[i - 1].Tag); // Add the type again, but this time without parents or childs
			}
			// this requires the whole currentNormalStyle list to be updated
			UpdateCurrentNormalOrder();
			UpdateCurrentNormalIndentation();
			_view.InitializeCurrentNormalGroupStyles(_currentNormalStyles);
		}

		public void EhView_UnindentGroupStyle()
		{
			// make sure that all the selected items are not child of another item
			for (int i = _currentNormalStyles.Count - 1; i >= 0; i--)
			{
				CheckableSelectableListNode selected = _currentNormalStyles[i];
				if (!selected.IsSelected)
					continue;

				if (null != _doc.GetParentTypeOf((Type)selected.Tag))
				{
					IPlotGroupStyle style = _doc.GetPlotGroupStyle((Type)selected.Tag);
					_doc.RemoveType(style.GetType()); // Removing the type so removing also the parent-child-relationship
					_doc.Add(style); // Add the type again, but this time without parents or childs
				}
			}

			// this requires the whole currentNormalStyle list to be updated
			UpdateCurrentNormalOrder();
			UpdateCurrentNormalIndentation();
			_view.InitializeCurrentNormalGroupStyles(_currentNormalStyles);
		}

		public void EhView_MoveUpGroupStyle()
		{
			if (0 == _currentNoOfItemsThatCanHaveChilds || _currentNormalStyles[0].IsSelected)
				return; // can not move up any more

			for (int i = 1; i < _currentNoOfItemsThatCanHaveChilds; i++)
			{
				CheckableSelectableListNode selected = _currentNormalStyles[i];
				if (!selected.IsSelected)
					continue;

				IPlotGroupStyle style = _doc.GetPlotGroupStyle((Type)selected.Tag);
				Type parenttype = _doc.GetParentTypeOf(style.GetType());
				_doc.RemoveType(style.GetType()); // Removing the type so removing also the parent-child-relationship
				if (parenttype == null)
				{
					_doc.Add(style);
				}
				else
				{
					_doc.Insert(style, parenttype); // Add the type, but parent type is this time the child type
				}
				_currentNormalStyles.Exchange(i, i - 1);
			}
			// this requires the whole currentNormalStyle list to be updated
			UpdateCurrentNormalOrder();
			_view.InitializeCurrentNormalGroupStyles(_currentNormalStyles);
		}

		public void EhView_MoveDownGroupStyle()
		{
			if (0 == _currentNoOfItemsThatCanHaveChilds || _currentNormalStyles[_currentNoOfItemsThatCanHaveChilds - 1].IsSelected)
				return; // can not move down any more

			for (int i = _currentNoOfItemsThatCanHaveChilds - 2; i >= 0; i--)
			{
				CheckableSelectableListNode selected = _currentNormalStyles[i];
				if (!selected.IsSelected)
					continue;

				IPlotGroupStyle style = _doc.GetPlotGroupStyle((Type)selected.Tag);
				Type childtype = _doc.GetTypeOfChild(style.GetType());
				_doc.RemoveType(style.GetType()); // Removing the type so removing also the parent-child-relationship
				if (childtype == null)
				{
					_doc.Add(style);
				}
				else
				{
					_doc.Add(style, childtype); // Add the type, but the child type this time is the parent type
				}
				_currentNormalStyles.Exchange(i, i + 1);
			}
			// this requires the whole currentNormalStyle list to be updated
			UpdateCurrentNormalOrder();
			_view.InitializeCurrentNormalGroupStyles(_currentNormalStyles);
		}

		#endregion IPlotGroupCollectionViewEventSink Members
	}
}