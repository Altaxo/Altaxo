#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2005 Dr. Dirk Lellinger
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
//    along with ctrl program; if not, write to the Free Software
//    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
//
/////////////////////////////////////////////////////////////////////////////
#endregion

using Altaxo.Graph.GUI;

namespace Altaxo.Graph.Procedures
{
  /// <summary>
  /// GraphCommands contain operations concerning the graph document itself, such as rename.
  /// </summary>
  public class GraphCommands
  {
    public static void Rename(GraphController ctrl)
    {
      Main.GUI.TextValueInputController tvctrl = new Main.GUI.TextValueInputController(
        ctrl.Doc.Name,
        new Main.GUI.SingleValueDialog("Rename graph", "Enter a name for the graph:")
        );

      tvctrl.Validator = new GraphRenameValidator(ctrl.Doc, ctrl);
      if (tvctrl.ShowDialog(Current.MainWindow))
        ctrl.Doc.Name = tvctrl.InputText.Trim();
    }

    protected class GraphRenameValidator : Main.GUI.TextValueInputController.NonEmptyStringValidator
    {
      Altaxo.Graph.GraphDocument _doc;
      GraphController _controller;

      public GraphRenameValidator(Altaxo.Graph.GraphDocument graphdoc, GraphController ctrl)
        : base("The graph's name must not be empty! Please enter a valid name.")
      {
        _doc = graphdoc;
        _controller = ctrl;
      }

      public override string Validate(string graphname)
      {
        string err = base.Validate(graphname);
        if (null != err)
          return err;

        if (_doc.Name == graphname)
          return null;
        else if (Graph.GraphDocumentCollection.GetParentGraphDocumentCollectionOf(_controller.Doc) == null)
          return null; // if there is no parent data set we can enter anything
        else if (Graph.GraphDocumentCollection.GetParentGraphDocumentCollectionOf(_controller.Doc).Contains(graphname))
          return "This graph name already exists, please choose another name!";
        else
          return null;
      }
    }

    public static void Refresh(GraphController ctrl)
    {
      ctrl.RefreshGraph();
    }
  }
}
