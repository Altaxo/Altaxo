#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2014 Dr. Dirk Lellinger
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Gui.Settings
{
	using Altaxo.Graph.Gdi;
	using Altaxo.Gui.Graph;
	using Graph.Gdi;

	public class ClipboardGraphExportOptionPanel : OptionPanelBase<ClipboardRenderingOptionsController>
	{
		public override void Initialize(object optionPanelOwner)
		{
			var doc = Current.PropertyService.GetValue(ClipboardRenderingOptions.PropertyKeyClipboardRenderingOptions, Altaxo.Main.Services.RuntimePropertyKind.UserAndApplicationAndBuiltin, () => new ClipboardRenderingOptions());
			_controller = new ClipboardRenderingOptionsController();
			_controller.InitializeDocument(doc);
		}

		protected override void ProcessControllerResult()
		{
			var doc = (ClipboardRenderingOptions)_controller.ModelObject;
			Current.PropertyService.UserSettings.SetValue(ClipboardRenderingOptions.PropertyKeyClipboardRenderingOptions, doc);
		}
	}
}