#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2016 Dr. Dirk Lellinger
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
using Altaxo.Drawing;
using Altaxo.Drawing.ColorManagement;
using Altaxo.Drawing.D3D;
using Altaxo.Graph.Graph3D.Plot.Groups;
using Altaxo.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Gui.Drawing.ColorManagement
{
	[ExpectedTypeOfView(typeof(IStyleListView))]
	[UserControllerForObject(typeof(IColorSet))]
	public class ColorSetController : StyleListController<ColorSetManager, IColorSet, NamedColor>
	{
		public ColorSetController()
			: base(ColorSetManager.Instance)
		{
		}

		protected override string ToDisplayName(NamedColor item)
		{
			return item.ToString();
		}

		protected override void Controller_AvailableItems_Initialize()
		{
			if (null == _availableItemsRootNode)
				_availableItemsRootNode = new NGTreeNode();
			else
				_availableItemsRootNode.Nodes.Clear();

			var levelDict = new Dictionary<ItemDefinitionLevel, NGTreeNode>();
			var allLists = ColorSetManager.Instance.GetEntryValues().ToArray(); ;
			Array.Sort(allLists, (x, y) =>
			{
				int result = Comparer<ItemDefinitionLevel>.Default.Compare(x.Level, y.Level);
				return 0 != result ? result : string.Compare(x.List.Name, y.List.Name);
			}
			);

			foreach (var list in allLists)
			{
				NGTreeNode levelNode;
				if (!levelDict.TryGetValue(list.Level, out levelNode))
				{
					levelNode = new NGTreeNode(Enum.GetName(typeof(ItemDefinitionLevel), list.Level));
					levelDict.Add(list.Level, levelNode);
					_availableItemsRootNode.Nodes.Add(levelNode);
				}

				var listNode = new NGTreeNode(list.List.Name);
				foreach (var color in list.List)
					listNode.Nodes.Add(new NGTreeNode(ToDisplayName(color)) { Tag = color });
				levelNode.Nodes.Add(listNode);
			}
		}

		protected override void Controller_CurrentItems_Initialize()
		{
			if (null == _currentItems)
				_currentItems = new SelectableListNodeList();
			else
				_currentItems.Clear();

			foreach (var color in _doc)
			{
				_currentItems.Add(new SelectableListNode(ToDisplayName(color), color, false));
			}
		}

		protected override void EhAvailableItem_AddToCurrent()
		{
			var avNodes = _availableItemsRootNode.TakeFromHereToFirstLeaves(false).Where(node => node.IsSelected && node.Tag is NamedColor).Select(node => (NamedColor)node.Tag).ToArray();
			if (avNodes.Length == 0)
				return;

			foreach (var namedColor in avNodes)
				_currentItems.Add(new SelectableListNode(ToDisplayName(namedColor), namedColor, false));
			SetListDirty();
		}

		protected override bool IsItemEditable(Altaxo.Main.IImmutable item)
		{
			if (null == item)
				return false;

			return true;
		}
	}
}