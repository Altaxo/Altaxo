#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2007 Dr. Dirk Lellinger
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

using System;
using System.Windows.Forms;
using Altaxo;
using Altaxo.Main;
using Altaxo.Graph;
using Altaxo.Graph.Gdi;
using Altaxo.Graph.Gdi.Plot;
using Altaxo.Graph.Gdi.Plot.Styles;
using Altaxo.Graph.Gdi.Plot.Data;
using Altaxo.Graph.Plot.Data;
using Altaxo.Graph.GUI;
using Altaxo.Gui.Graph;
using Altaxo.Gui;
using Altaxo.Gui.Common;
using Altaxo.Gui.Scripting;
using Altaxo.Scripting;
using ICSharpCode.SharpZipLib.Zip;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;

namespace Altaxo.Graph.Commands
{
	/// <summary>
	/// Handler for the menu item "Edit" - "Copy" - "ActiveLayer".
	/// </summary>
	public class EditActiveLayer : AbstractGraphControllerCommand
	{
		public override void Run(Altaxo.Graph.GUI.WinFormsGraphController ctrl)
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
		public override void Run(Altaxo.Graph.GUI.WinFormsGraphController ctrl)
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
		public override void Run(Altaxo.Graph.GUI.WinFormsGraphController ctrl)
		{
			ctrl.Doc.PasteFromClipboardAsNewLayer();
		}
	}


	/// <summary>
	/// Pastes a layer as new layer before the active layer.
	/// </summary>
	public class PasteAsNewLayerBefore : AbstractGraphControllerCommand
	{
		public override void Run(Altaxo.Graph.GUI.WinFormsGraphController ctrl)
		{
			ctrl.EnsureValidityOfCurrentLayerNumber();
			ctrl.Doc.PasteFromClipboardAsNewLayerBeforeLayerNumber(ctrl.CurrentLayerNumber);
		}
	}

	/// <summary>
	/// Pastes a layer as new layer after the active layer.
	/// </summary>
	public class PasteAsNewLayerAfter : AbstractGraphControllerCommand
	{
		public override void Run(Altaxo.Graph.GUI.WinFormsGraphController ctrl)
		{
			ctrl.EnsureValidityOfCurrentLayerNumber();
			ctrl.Doc.PasteFromClipboardAsNewLayerAfterLayerNumber(ctrl.CurrentLayerNumber);
		}
	}

	/// <summary>
	/// Pastes a layer as template in the active layer. The user can choose which elements should be pasted.
	/// </summary>
	public class PasteInActiveLayer : AbstractGraphControllerCommand
	{
		public override void Run(Altaxo.Graph.GUI.WinFormsGraphController ctrl)
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
		public override void Run(Altaxo.Graph.GUI.WinFormsGraphController ctrl)
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
		public override void Run(Altaxo.Graph.GUI.WinFormsGraphController ctrl)
		{
			ctrl.EnsureValidityOfCurrentLayerNumber();
			ctrl.Doc.ShowMoveLayerToPositionDialog(ctrl.CurrentLayerNumber);
		}
	}

	/// <summary>
	/// Taken from Commands.MenuItemBuilders. See last line for change.
	/// </summary>
	public class LayerItemsBuilder : ISubmenuBuilder
	{
		public ToolStripItem[] BuildSubmenu(Codon codon, object owner)
		{
			var ctrl	 = Current.Workbench.ActiveViewContent as Altaxo.Gui.SharpDevelop.SDGraphViewContent;
			if (null == ctrl)
				return null;
			var activeLayer = ctrl.Controller.ActiveLayer;
			if (null == activeLayer)
				return null;

			int actPA = ctrl.Controller.CurrentPlotNumber;
			int len = activeLayer.PlotItems.Flattened.Length;
			MenuCommand[] items = new MenuCommand[len];
			for (int i = 0; i < len; i++)
			{
				IGPlotItem pa = activeLayer.PlotItems.Flattened[i];
				items[i] = new MenuCommand(pa.ToString(), new EventHandler(EhMenuData_Data));
				items[i].Checked = (i == actPA);
				items[i].Tag = i;
			}

			return items;
			}

		


	  /// <summary>
    /// Handler for all submenu items of the data popup.".
    /// </summary>
    /// <param name="sender">The menuitem, must be of type <see cref="DataMenuItem"/>.</param>
    /// <param name="e">Not used.</param>
    /// <remarks>The handler either checks the menuitem, if it was unchecked. If it was already checked,
    /// it shows the LineScatterPlotStyleControl into a dialog box.
    /// </remarks>
    private void EhMenuData_Data(object sender, System.EventArgs e)
    {
      MenuCommand dmi = (MenuCommand)sender;
			int plotItemNumber = (int)dmi.Tag;

			var ctrl = Current.Workbench.ActiveViewContent as Altaxo.Gui.SharpDevelop.SDGraphViewContent;
			if (null == ctrl)
				return ;
			var activeLayer = ctrl.Controller.ActiveLayer;
			if (null == activeLayer)
				return;

      if(!dmi.Checked)
      {
        // if the menu item was not checked before, check it now
        // by making the plot association shown by the menu item
        // the actual plot association
        if(null!=activeLayer && plotItemNumber<activeLayer.PlotItems.Flattened.Length)
        {
          dmi.Checked=true;
          ctrl.Controller.CurrentPlotNumber = plotItemNumber;
        }
      }
      else
      {
        IGPlotItem pa = activeLayer.PlotItems.Flattened[plotItemNumber];
        Current.Gui.ShowDialog(new object[] { pa }, string.Format("#{0}: {1}", pa.Name, pa.ToString()),true);
      }
    }
	}

}
