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

#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Altaxo.Settings;

namespace Altaxo.Gui.Settings
{
  public class DocumentCultureSettingsOptionPanel : OptionPanelBase<Altaxo.Gui.Common.ConditionalDocumentControllerWithDisabledView<CultureSettings>>
  {
    public override void Initialize(object optionPanelOwner)
    {
      CultureSettings sysCulture = null;
      Current.PropertyService.UserSettings.TryGetValue(CultureSettings.PropertyKeyDocumentCulture, out var docCulture);
      sysCulture = Current.PropertyService.GetValue(CultureSettings.PropertyKeyDocumentCulture, Altaxo.Main.Services.RuntimePropertyKind.ApplicationAndBuiltin);

      _controller = new Altaxo.Gui.Common.ConditionalDocumentControllerWithDisabledView<CultureSettings>(() => sysCulture.Clone(), () => sysCulture)
      {
        EnablingText = "Override system settings"
      };
      _controller.InitializeDocument(new object[] { docCulture, sysCulture });
    }

    protected override void ProcessControllerResult()
    {
      if (_controller.ModelObject is not null)
      {
        var docCulture = (CultureSettings)_controller.ModelObject;
        Current.PropertyService.UserSettings.SetValue(CultureSettings.PropertyKeyDocumentCulture, docCulture);
      }
      else
      {
        Current.PropertyService.UserSettings.RemoveValue(CultureSettings.PropertyKeyDocumentCulture);
      }
    }
  }
}
