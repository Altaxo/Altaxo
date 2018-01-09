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

using Altaxo.Gui.AddInItems;
using Altaxo.Gui.Graph.Gdi.Viewing;
using System;
using System.Collections.Generic;
using System.Text;

namespace Altaxo.Main.Commands
{
	public class AddTemporaryUserAssembly : AbstractMenuCommand
	{
		public override void Run()
		{
			Settings.Scripting.ReferencedAssembliesCommands.ShowAddTemporaryAssemblyDialog();
		}
	}

	public class TestProjectLoading : AbstractMenuCommand
	{
		public override void Run()
		{
			Altaxo.Main.Commands.TestAllProjectsInFolder.ShowDialogToVerifyOpeningOfDocumentsWithoutException();
		}
	}

	public class ShowOptions : AbstractMenuCommand
	{
		public override void Run()
		{
			var ctrl = new Altaxo.Gui.Settings.SettingsController();
			Current.Gui.ShowDialog(ctrl, "Altaxo settings", false);
		}
	}

	public class ShowUserSettings : AbstractMenuCommand
	{
		public override void Run()
		{
			var ph = new Altaxo.Main.Properties.PropertyHierarchy(PropertyExtensions.GetPropertyBagsStartingFromUserSettings());
			Current.Gui.ShowDialog(new object[] { ph }, "Edit user settings", false);
		}
	}

	public class RegisterApplicationForCom : AbstractMenuCommand
	{
		public override void Run()
		{
			Current.ComManager.RegisterApplicationForCom();
		}
	}

	public class UnregisterApplicationForCom : AbstractMenuCommand
	{
		public override void Run()
		{
			Current.ComManager.UnregisterApplicationForCom();
		}
	}

	public class CopyDocumentAsComObjectToClipboard : AbstractMenuCommand
	{
		public override void Run()
		{
			{
				if (Current.Workbench.ActiveViewContent is Altaxo.Gui.Graph.Gdi.Viewing.GraphController ctrl)
				{
					var doc = ctrl.Doc;

					var comManager = (Com.ComManager)Current.ComManager;
					//var dataObject = comManager.GetDocumentsComObjectForDocument(doc);

					var dataObject = Current.ComManager.GetDocumentsDataObjectForDocument(doc);

					if (null != dataObject)
						System.Windows.Clipboard.SetDataObject(dataObject);
				}
			}
		}
	}
}