#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2012 Dr. Dirk Lellinger
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

#region Original Copyright

// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

#endregion Original Copyright

using Altaxo.Gui;
using ICSharpCode.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Main
{
	public interface IOptionPanelDescriptor
	{
		/// <value>
		/// Returns the ID of the dialog panel codon
		/// </value>
		string ID
		{
			get;
		}

		/// <value>
		/// Returns the label of the dialog panel
		/// </value>
		string Label
		{
			get;
			set;
		}

		/// <summary>
		/// The child dialog panels (e.g. for treeviews)
		/// </summary>
		IEnumerable<IOptionPanelDescriptor> ChildOptionPanelDescriptors
		{
			get;
		}

		/// <value>
		/// Returns the dialog panel object
		/// </value>
		IOptionPanel OptionPanel
		{
			get;
		}

		/// <summary>
		/// Gets whether the descriptor has an option panel (as opposed to having only child option panels)
		/// </summary>
		bool HasOptionPanel
		{
			get;
		}
	}

	/// <summary>
	/// Creates DefaultOptionPanelDescriptor objects that are used in option dialogs.
	/// </summary>
	/// <attribute name="class">
	/// Name of the IOptionPanel class. Optional if the page has subpages.
	/// </attribute>
	/// <attribute name="label" use="required">
	/// Caption of the dialog panel.
	/// </attribute>
	/// <children childTypes="OptionPanel">
	/// In the SharpDevelop options, option pages can have subpages by specifying them
	/// as children in the AddInTree.
	/// </children>
	/// <usage>In /SharpDevelop/BackendBindings/ProjectOptions/ and /SharpDevelop/Dialogs/OptionsDialog</usage>
	/// <returns>
	/// A DefaultOptionPanelDescriptor object.
	/// </returns>
	public class OptionPanelDoozer : IDoozer
	{
		/// <summary>
		/// Gets if the doozer handles codon conditions on its own.
		/// If this property return false, the item is excluded when the condition is not met.
		/// </summary>
		public bool HandleConditions
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// Creates an item with the specified sub items. And the current
		/// Condition status for this item.
		/// </summary>
		public object BuildItem(BuildItemArgs args)
		{
			string label = args.Codon["label"];
			string id = args.Codon.Id;

			var subItems = args.BuildSubItems<IOptionPanelDescriptor>();
			if (subItems.Count == 0)
			{
				if (args.Codon.Properties.Contains("class"))
				{
					return new DefaultOptionPanelDescriptor(id, StringParser.Parse(label), args.AddIn, null, args.Codon["class"]);
				}
				else
				{
					return new DefaultOptionPanelDescriptor(id, StringParser.Parse(label));
				}
			}

			return new DefaultOptionPanelDescriptor(id, StringParser.Parse(label), subItems);
		}
	}
}