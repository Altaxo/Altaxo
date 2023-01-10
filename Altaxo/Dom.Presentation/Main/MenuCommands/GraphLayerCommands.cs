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
using System.Collections.Generic;
using Altaxo.AddInItems;
using Altaxo.Graph.Gdi;
using Altaxo.Graph.Gdi.Plot;

namespace Altaxo.Graph.Commands
{
  /// <summary>
  /// Taken from Commands.MenuItemBuilders. See last line for change.
  /// </summary>
  public class LayerItemsBuilder : IMenuItemBuilder
  {
    public IEnumerable<object> BuildItems(Codon codon, object owner)
    {
      var ctrl = Current.Workbench.ActiveViewContent as Altaxo.Gui.Graph.Gdi.Viewing.GraphController;
      if (ctrl is null)
        return null;
      var activeLayer = ctrl.ActiveLayer as XYPlotLayer;
      if (activeLayer is null)
        return null;

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

    private void EhWpfMenuItem_Clicked(object sender, System.Windows.RoutedEventArgs e)
    {
      var dmi = (System.Windows.Controls.MenuItem)sender;
      int plotItemNumber = (int)dmi.Tag;

      var ctrl = Current.Workbench.ActiveViewContent as Altaxo.Gui.Graph.Gdi.Viewing.GraphController;
      if (ctrl is null)
        return;
      var activeLayer = ctrl.ActiveLayer as XYPlotLayer;
      if (activeLayer is null)
        return;

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
