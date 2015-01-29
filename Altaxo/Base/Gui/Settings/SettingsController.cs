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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Gui.Settings
{
	/// <summary>
	/// Implemented by the Gui element that displays the settings (topics as a tree, and the selected topic in a control).
	/// </summary>
	public interface ISettingsView
	{
		/// <summary>Occurs when the user selected another topic.</summary>
		event Action<NGTreeNode> TopicSelectionChanged;

		/// <summary>Occurs when the current topic view was entered.</summary>
		event Action CurrentTopicViewMadeDirty;

		/// <summary>Initializes the topic tree.</summary>
		/// <param name="topics">The topics tree structure.</param>
		void InitializeTopics(NGTreeNodeCollection topics);

		/// <summary>Sets the currently selected topic view.</summary>
		/// <param name="title">Titel (shown above the topic view).</param>
		/// <param name="guiTopicObject">The Gui topic view object.</param>
		void InitializeTopicView(string title, object guiTopicObject);

		/// <summary>Initializes an indicator whether the topic view is dirty or not.</summary>
		/// <param name="dirtyIndicator">The dirty indicator. Zero means: is not dirty, 1: is dirty, and 2 means: the topic view contains errors.</param>
		void InitializeTopicViewDirtyIndicator(int dirtyIndicator);

		/// <summary>Sets the selected node in the topic view.</summary>
		/// <param name="node">The node to select.</param>
		void SetSelectedNode(NGTreeNode node);
	}
}