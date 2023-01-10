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

#nullable disable warnings
using Altaxo.Graph.Gdi;
using Altaxo.Graph.Gdi.Plot;
using Altaxo.Gui.Graph.Gdi.Viewing;
using Altaxo.Serialization.Clipboard;

namespace Altaxo.Graph.Commands
{
  /// <summary>
  /// Handler for the menu item "Edit" - "Copy" - "ActiveLayer".
  /// </summary>
  public class EditActiveLayer : AbstractGraphControllerCommand
  {
    public override void Run(GraphController ctrl)
    {
      ctrl.EnsureValidityOfCurrentLayerNumber();
      ctrl.Doc.ShowLayerDialog(ctrl.CurrentLayerNumber);
    }
  }

  /// <summary>
  /// Handler for the menu item "Edit" - "Copy" - "ActiveLayer".
  /// </summary>
  public class CopyActiveLayer : AbstractGraphControllerCommand
  {
    public override void Run(GraphController ctrl)
    {
      ctrl.EnsureValidityOfCurrentLayerNumber();
      ctrl.Doc.CopyToClipboardLayerAsNative(ctrl.CurrentLayerNumber);
    }
  }

  /// <summary>
  /// Handler for the menu item "Edit" - "Paste" - "AsNewLayer".
  /// </summary>
  public class PasteAsNewLayer : AbstractGraphControllerCommand
  {
    public override void Run(GraphController ctrl)
    {
      ctrl.Doc.PasteFromClipboardAsNewLayer();
    }
  }

  /// <summary>
  /// Pastes a layer as new layer before the active layer.
  /// </summary>
  public class PasteAsNewLayerBefore : AbstractGraphControllerCommand
  {
    public override void Run(GraphController ctrl)
    {
      ctrl.EnsureValidityOfCurrentLayerNumber();
      var currentLayerNumber = ctrl.CurrentLayerNumber;
      if (currentLayerNumber.Count != 0)
      {
        ctrl.Doc.PasteFromClipboardAsNewLayerBeforeLayerNumber(ctrl.CurrentLayerNumber);
      }
      else
      {
        Current.Gui.ErrorMessageBox("'Can't paste before the root layer. Please select another layer.", "Operation not possible");
      }
    }
  }

  /// <summary>
  /// Pastes a layer as new layer after the active layer.
  /// </summary>
  public class PasteAsNewLayerAfter : AbstractGraphControllerCommand
  {
    public override void Run(GraphController ctrl)
    {
      ctrl.EnsureValidityOfCurrentLayerNumber();
      var currentLayerNumber = ctrl.CurrentLayerNumber;
      if (currentLayerNumber.Count != 0)
      {
        ctrl.Doc.PasteFromClipboardAsNewLayerAfterLayerNumber(ctrl.CurrentLayerNumber);
      }
      else
      {
        Current.Gui.ErrorMessageBox("'Can't paste after the root layer. Please select another layer.", "Operation not possible");
      }
    }
  }

  /// <summary>
  /// Pastes a layer as new layer after the active layer.
  /// </summary>
  public class PasteNewLayerAsChild : AbstractGraphControllerCommand
  {
    public override void Run(GraphController ctrl)
    {
      ctrl.EnsureValidityOfCurrentLayerNumber();
      ctrl.Doc.PasteFromClipboardAsNewChildLayerOfLayerNumber(ctrl.CurrentLayerNumber);
    }
  }

  /// <summary>
  /// Pastes a layer as template in the active layer. The user can choose which elements should be pasted.
  /// </summary>
  public class PasteInActiveLayer : AbstractGraphControllerCommand
  {
    public override void Run(GraphController ctrl)
    {
      ctrl.EnsureValidityOfCurrentLayerNumber();
      ctrl.Doc.PasteFromClipboardAsTemplateForLayer(ctrl.CurrentLayerNumber);
    }
  }

  /// <summary>
  /// Deletes the active layer.
  /// </summary>
  public class DeleteActiveLayer : AbstractGraphControllerCommand
  {
    public override void Run(GraphController ctrl)
    {
      ctrl.EnsureValidityOfCurrentLayerNumber();
      ctrl.Doc.DeleteLayer(ctrl.CurrentLayerNumber, true);
    }
  }

  /// <summary>
  /// Moves the active layer to a user choosen position.
  /// </summary>
  public class MoveActiveLayer : AbstractGraphControllerCommand
  {
    public override void Run(GraphController ctrl)
    {
      ctrl.EnsureValidityOfCurrentLayerNumber();
      ctrl.Doc.ShowMoveLayerToPositionDialog(ctrl.CurrentLayerNumber);
    }
  }

  /// <summary>
  /// Copies the plot contents of the active layer to the clipboard.
  /// </summary>
  public class CopyActiveLayerContents : AbstractGraphControllerCommand
  {
    /// <inheritdoc />
    public override void Run(GraphController ctrl)
    {
      ctrl.EnsureValidityOfCurrentLayerNumber();
      var currentLayerNumber = ctrl.CurrentLayerNumber;
      var plotLayer = ctrl.ActiveLayer as XYPlotLayer;

      if (plotLayer is not null)
      {
        var clonedItems = plotLayer.PlotItems.Clone();
        ClipboardSerialization.PutObjectToClipboard("Altaxo.Graph.Gdi.Plot.PlotItemCollection.AsXml", clonedItems);
      }
      else
      {
        Current.Gui.ErrorMessageBox("'Can only copy plot content from an XYPlotLayer. Please select another layer.", "Operation not possible");
      }
    }
  }

  /// <summary>
  /// Copies the plot contents of the active layer to the clipboard. The abstract property <see cref="PasteExclusively"/>
  /// determines whether the old content is deleted before pasting.
  /// </summary>
  public abstract class PasteLayerContentsBase : AbstractGraphControllerCommand
  {
    public abstract bool PasteExclusively { get; }

    /// <inheritdoc />
    public override void Run(GraphController ctrl)
    {
      ctrl.EnsureValidityOfCurrentLayerNumber();
      var currentLayerNumber = ctrl.CurrentLayerNumber;
      var plotLayer = ctrl.ActiveLayer as XYPlotLayer;

      if (plotLayer is not null)
      {
        object o = ClipboardSerialization.GetObjectFromClipboard("Altaxo.Graph.Gdi.Plot.PlotItemCollection.AsXml");
        // if at this point obj is a memory stream, you probably have forgotten the deserialization constructor of the class you expect to deserialize here
        if (o is PlotItemCollection coll)
        {
          if (PasteExclusively)
          {
            plotLayer.PlotItems.ClearPlotItems();
          }

          foreach (IGPlotItem item in coll) // it is neccessary to add the items to the doc first, because otherwise they don't have names
          {
            var clonedItem = (IGPlotItem)item.Clone();
            plotLayer.PlotItems.Add(clonedItem); // cloning neccessary because coll will be disposed afterwards, which would destroy all items
          }
        }
      }
      else
      {
        Current.Gui.ErrorMessageBox("'Can only paste plot content to an XYPlotLayer. Please select another layer.", "Operation not possible");
      }
    }
  }

  /// <summary>
  /// Copies the plot contents of the active layer to the clipboard. The old content is deleted before pasting the new content.
  /// </summary>
  public class PasteLayerContentsExclusively : PasteLayerContentsBase
  {
    public override bool PasteExclusively => true;
  }

  /// <summary>
  /// Copies the plot contents of the active layer to the clipboard. The old content is not deleted, and the new content
  /// is added behind the old content.
  /// </summary>
  public class PasteLayerContentsAdditionally : PasteLayerContentsBase
  {
    public override bool PasteExclusively => false;
  }


}
