#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2018 Dr. Dirk Lellinger
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

#nullable disable warnings
using Altaxo.Gui;

namespace Altaxo.Main.Commands
{

  public class RegisterApplicationForCom : SimpleCommand
  {
    public override void Execute(object parameter)
    {
      Current.ComManager.RegisterApplicationForCom();
    }
  }

  public class UnregisterApplicationForCom : SimpleCommand
  {
    public override void Execute(object parameter)
    {
      Current.ComManager.UnregisterApplicationForCom();
    }
  }

  public class CopyDocumentAsComObjectToClipboard : SimpleCommand
  {
    public override void Execute(object parameter)
    {
      {
        if (Current.Workbench.ActiveViewContent is Altaxo.Gui.Graph.Gdi.Viewing.GraphController ctrl)
        {
          var doc = ctrl.Doc;

          var comManager = (Com.ComManager)Current.ComManager;
          //var dataObject = comManager.GetDocumentsComObjectForDocument(doc);

          var dataObject = Current.ComManager.GetDocumentsDataObjectForDocument(doc);

          if (dataObject is not null)
            System.Windows.Clipboard.SetDataObject(dataObject);
        }
      }
    }
  }
}
