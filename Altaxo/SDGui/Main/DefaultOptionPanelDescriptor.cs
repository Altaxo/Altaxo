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
#endregion

#region Original Copyright
// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)
#endregion

using System;
using System.Collections.Generic;
using ICSharpCode.Core;

namespace Altaxo.Main
{
	public class DefaultOptionPanelDescriptor : IOptionPanelDescriptor
	{
		string id = String.Empty;
		List<IOptionPanelDescriptor> optionPanelDescriptors = null;
		Altaxo.Gui.IMVCANController optionPanel = null;

		public string ID
		{
			get
			{
				return id;
			}
		}

		public string Label { get; set; }

		public IEnumerable<IOptionPanelDescriptor> ChildOptionPanelDescriptors
		{
			get
			{
				return optionPanelDescriptors;
			}
		}

		AddIn addin;
		object owner;
		string optionPanelPath;

		public Altaxo.Gui.IMVCANController OptionPanel
		{
			get
			{
				if (optionPanelPath != null)
				{
					if (optionPanel == null)
					{
						optionPanel = (Altaxo.Gui.IMVCANController)addin.CreateObject(optionPanelPath);
						if (optionPanel != null)
						{
							optionPanel.InitializeDocument(null,owner);
						}
					}
					optionPanelPath = null;
					addin = null;
				}
				return optionPanel;
			}
		}

		public bool HasOptionPanel
		{
			get
			{
				return optionPanelPath != null;
			}
		}

		public DefaultOptionPanelDescriptor(string id, string label)
		{
			this.id = id;
			this.Label = label;
		}

		public DefaultOptionPanelDescriptor(string id, string label, List<IOptionPanelDescriptor> dialogPanelDescriptors)
			: this(id, label)
		{
			this.optionPanelDescriptors = dialogPanelDescriptors;
		}

		public DefaultOptionPanelDescriptor(string id, string label, AddIn addin, object owner, string optionPanelPath)
			: this(id, label)
		{
			this.addin = addin;
			this.owner = owner;
			this.optionPanelPath = optionPanelPath;
		}
	}
}
