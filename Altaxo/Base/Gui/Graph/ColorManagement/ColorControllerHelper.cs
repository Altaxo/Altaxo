#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2014 Dr. Dirk Lellinger
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Gui.Graph.ColorManagement
{
	internal class ColorControllerHelper
	{
		#region Inner class

		/// <summary>
		/// Special tree node for a color set. This tree node fills its child items only when it gets expanded.
		/// </summary>
		private class NGTreeNodeForColorSet : NGTreeNode
		{
			public NGTreeNodeForColorSet(IColorSet colorSet)
				: base(true)
			{
				_tag = colorSet;
				_text = colorSet.Name;
			}

			protected override void LoadChildren()
			{
				base.LoadChildren();

				foreach (var c in (IColorSet)_tag)
				{
					Nodes.Add(new NGTreeNode() { Text = c.Name, Tag = c });
				}
			}
		}

		#endregion Inner class

		/// <summary>
		/// Updates the TreeView tree nodes of the color tree.
		/// </summary>
		/// <param name="rootNode">The root node of the color tree.</param>
		/// <param name="showPlotColorsOnly">if set to <c>true</c>, the tree will show plot colors only.</param>
		/// <param name="currentSelectedObject">The current selected object. Can either be a <see cref="IColorSet"/> or a <see cref="Altaxo.Graph.NamedColor"/>.</param>
		/// <exception cref="System.InvalidProgramException"></exception>
		public static void UpdateColorTreeViewTreeNodes(NGTreeNode rootNode, bool showPlotColorsOnly, object currentSelectedObject)
		{
			var manager = ColorSetManager.Instance;

			var builtIn = new NGTreeNode() { Text = "Builtin", Tag = ColorSetLevel.Builtin };
			var app = new NGTreeNode() { Text = "Application", Tag = ColorSetLevel.Application };
			var user = new NGTreeNode() { Text = "User", Tag = ColorSetLevel.UserDefined };
			var proj = new NGTreeNode() { Text = "Project", Tag = ColorSetLevel.Project };

			IColorSet parentColorSetOfColor = null;
			NamedColor selectedColor;
			if (currentSelectedObject is NamedColor)
			{
				selectedColor = (NamedColor)currentSelectedObject;
				parentColorSetOfColor = selectedColor.ParentColorSet;
			}
			else
			{
				selectedColor = NamedColors.Black;
			}

			foreach (var set in manager.GetAllColorSets())
			{
				if (showPlotColorsOnly && !set.IsPlotColorSet)
					continue;

				NGTreeNode newNode;

				switch (set.Level)
				{
					case ColorSetLevel.Builtin:
						builtIn.Nodes.Add(newNode = new NGTreeNodeForColorSet(set));
						break;

					case ColorSetLevel.Application:
						app.Nodes.Add(newNode = new NGTreeNodeForColorSet(set));
						break;

					case ColorSetLevel.UserDefined:
						user.Nodes.Add(newNode = new NGTreeNodeForColorSet(set));
						break;

					case ColorSetLevel.Project:
						proj.Nodes.Add(newNode = new NGTreeNodeForColorSet(set));
						break;

					default:
						throw new InvalidProgramException(string.Format("Unknown level {0}. Please report this error to the forum.", set.Level));
				}

				if (currentSelectedObject is IColorSet)
				{
					bool isCurrentlySelected = object.ReferenceEquals(set, currentSelectedObject);
					newNode.IsSelected = isCurrentlySelected;
					if (isCurrentlySelected)
						newNode.IsExpanded = true;
				}
				else if (parentColorSetOfColor != null && object.ReferenceEquals(set, parentColorSetOfColor))
				{
					newNode.IsExpanded = true;

					foreach (var node in newNode.Nodes)
					{
						if ((node.Tag is NamedColor) && ((NamedColor)node.Tag) == selectedColor)
							node.IsSelected = true;
					}
				}
			}

			rootNode.Nodes.Clear();
			rootNode.Nodes.Add(builtIn);
			rootNode.Nodes.Add(app);
			rootNode.Nodes.Add(user);
			rootNode.Nodes.Add(proj);
		}
	}
}