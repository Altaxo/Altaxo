#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2016 Dr. Dirk Lellinger
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
using Altaxo.AddInItems;
using Altaxo.Graph.Graph3D;
using Altaxo.Graph.Graph3D.Plot;
using Altaxo.Gui.Graph.Graph3D.Viewing;

namespace Altaxo.Graph.Graph3D.Commands
{
  /// <summary>
  /// Handler for the menu item "Edit" - "Copy" - "ActiveLayer".
  /// </summary>
  public class EditActiveLayer : AbstractGraph3DControllerCommand
  {
    public override void Run(Graph3DController ctrl)
    {
      ctrl.EnsureValidityOfCurrentLayerNumber();
      ctrl.Doc.ShowLayerDialog(ctrl.CurrentLayerNumber);
    }
  }

  /// <summary>
  /// Handler for the menu item "Edit" - "Copy" - "ActiveLayer".
  /// </summary>
  public class CopyActiveLayer : AbstractGraph3DControllerCommand
  {
    public override void Run(Graph3DController ctrl)
    {
      ctrl.EnsureValidityOfCurrentLayerNumber();
      ctrl.Doc.CopyToClipboardLayerAsNative(ctrl.CurrentLayerNumber);
    }
  }

  /// <summary>
  /// Handler for the menu item "Edit" - "Paste" - "AsNewLayer".
  /// </summary>
  public class PasteAsNewLayer : AbstractGraph3DControllerCommand
  {
    public override void Run(Graph3DController ctrl)
    {
      ctrl.Doc.PasteFromClipboardAsNewLayer();
    }
  }

  /// <summary>
  /// Pastes a layer as new layer before the active layer.
  /// </summary>
  public class PasteAsNewLayerBefore : AbstractGraph3DControllerCommand
  {
    public override void Run(Graph3DController ctrl)
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
  public class PasteAsNewLayerAfter : AbstractGraph3DControllerCommand
  {
    public override void Run(Graph3DController ctrl)
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
  public class PasteNewLayerAsChild : AbstractGraph3DControllerCommand
  {
    public override void Run(Graph3DController ctrl)
    {
      ctrl.EnsureValidityOfCurrentLayerNumber();
      ctrl.Doc.PasteFromClipboardAsNewChildLayerOfLayerNumber(ctrl.CurrentLayerNumber);
    }
  }

  /// <summary>
  /// Pastes a layer as template in the active layer. The user can choose which elements should be pasted.
  /// </summary>
  public class PasteInActiveLayer : AbstractGraph3DControllerCommand
  {
    public override void Run(Graph3DController ctrl)
    {
      ctrl.EnsureValidityOfCurrentLayerNumber();
      ctrl.Doc.PasteFromClipboardAsTemplateForLayer(ctrl.CurrentLayerNumber);
    }
  }

  /// <summary>
  /// Deletes the active layer.
  /// </summary>
  public class DeleteActiveLayer : AbstractGraph3DControllerCommand
  {
    public override void Run(Graph3DController ctrl)
    {
      ctrl.EnsureValidityOfCurrentLayerNumber();
      ctrl.Doc.DeleteLayer(ctrl.CurrentLayerNumber, true);
    }
  }

  /// <summary>
  /// Moves the active layer to a user choosen position.
  /// </summary>
  public class MoveActiveLayer : AbstractGraph3DControllerCommand
  {
    public override void Run(Graph3DController ctrl)
    {
      ctrl.EnsureValidityOfCurrentLayerNumber();
      ctrl.Doc.ShowMoveLayerToPositionDialog(ctrl.CurrentLayerNumber);
    }
  }

  /// <summary>
  /// Taken from Commands.MenuItemBuilders. See last line for change.
  /// </summary>
  public class LayerItemsBuilder : IMenuItemBuilder
  {
    public IEnumerable<object> BuildItems(Codon codon, object owner)
    {
      if (Current.Workbench.ActiveViewContent is Graph3DController ctrl)
      {
        var activeLayer = ctrl.ActiveLayer as XYZPlotLayer;
        if (activeLayer is null)
          return Enumerable.Empty<object>();

        int actPA = ctrl.CurrentPlotNumber;
        int len = activeLayer.PlotItems.Flattened.Length;
        var items = new List<object>();
        for (int i = 0; i < len; i++)
        {
          IGPlotItem pa = activeLayer.PlotItems.Flattened[i];
          var item = new System.Windows.Controls.MenuItem() { Header = pa.ToString() };
          item.Click += EhWpfMenuItem_Clicked;
          item.IsChecked = (i == actPA);
          item.Tag = i;
          items.Add(item);
        }

        return items;
      }
      return Enumerable.Empty<object>();
    }

    private void EhWpfMenuItem_Clicked(object sender, System.Windows.RoutedEventArgs e)
    {
      var dmi = (System.Windows.Controls.MenuItem)sender;
      int plotItemNumber = (int)dmi.Tag;

      if (Current.Workbench.ActiveViewContent is Graph3DController ctrl && ctrl.ActiveLayer is XYZPlotLayer activeLayer)
      {
        if (!dmi.IsChecked)
        {
          // if the menu item was not checked before, check it now
          // by making the plot association shown by the menu item
          // the actual plot association
          if (activeLayer is not null && plotItemNumber < activeLayer.PlotItems.Flattened.Length)
          {
            dmi.IsChecked = true;
            ctrl.CurrentPlotNumber = plotItemNumber;
          }
        }
        else
        {
          IGPlotItem pa = activeLayer.PlotItems.Flattened[plotItemNumber];
          Current.Gui.ShowDialog(new object[] { pa }, string.Format("#{0}: {1}", pa.Name, pa.ToString()), true);
        }
      }
    }
  }
}
