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

using Altaxo.Graph.Gdi;
using Altaxo.Graph.GUI;
using Altaxo.Gui.Graph;
using Altaxo.Graph.Gdi.Plot;
using Altaxo.Graph.Gdi.Plot.Groups;

namespace Altaxo.Gui.Common
{
  /// <summary>
  /// DialogFactory contains static methods for building and showing dialogs.
  /// </summary>
  public class DialogFactory
  {

    /// <summary>
    /// Shows a dialog to add columns to a table.
    /// </summary>
    /// <param name="owner">The parent form of the shown dialog.</param>
    /// <param name="table">The table where to add the columns.</param>
    /// <param name="bAddToPropertyColumns">If true, the columns are added to the property columns instead of the data columns collection.</param>
    public static void ShowAddColumnsDialog(System.Windows.Forms.Form owner, Altaxo.Data.DataTable table, bool bAddToPropertyColumns)
    {
      ListBoxEntry[] lbitems = new ListBoxEntry[]
        {
          new ListBoxEntry("Numeric",typeof(Altaxo.Data.DoubleColumn)),
          new ListBoxEntry("Date/Time",typeof(Altaxo.Data.DateTimeColumn)),
          new ListBoxEntry("Text",typeof(Altaxo.Data.TextColumn))
        };


      IntegerAndComboBoxController ct = new IntegerAndComboBoxController(
        "Number of colums to add:",1,int.MaxValue,1,
        "Type of columns to add:",lbitems,0);

      SpinAndComboBoxControl panel = new SpinAndComboBoxControl();
      ct.View = panel;

      DialogShellController dsc = new DialogShellController(
        new DialogShellView(panel),ct,"Add new column(s)",false);

      
      if(true==dsc.ShowDialog(owner))
      {
        System.Type columntype = (System.Type)ct.SelectedItem.Tag;

        table.Suspend();

        if(bAddToPropertyColumns)
        {
          for(int i=0;i<ct.IntegerValue;i++)
          {
            table.PropCols.Add((Altaxo.Data.DataColumn)Activator.CreateInstance(columntype));
          }
        }
        else
        {
          for(int i=0;i<ct.IntegerValue;i++)
          {
            table.DataColumns.Add((Altaxo.Data.DataColumn)Activator.CreateInstance(columntype));
          }
        }

        table.Resume();
      }
    }

  
    /// <summary>
    /// Shows the polynomial fit dialog.
    /// </summary>
    /// <param name="parentWindow">The window in whose context the dialog is shown.</param>
    /// <param name="controller">The polynomial fit controller. Contains the user choices at the end of this routine.</param>
    /// <returns>True if the user has pressed OK.</returns>
    public static bool ShowPolynomialFitDialog(System.Windows.Forms.Form parentWindow, Altaxo.Gui.Graph.IFitPolynomialDialogController controller)
    {
      FitPolynomialDialogControl panel = new FitPolynomialDialogControl();
      controller.ViewObject = panel;

      DialogShellController dsc = new DialogShellController(
        new DialogShellView(panel),controller,"Polynomial fit",false);
 

      return dsc.ShowDialog(parentWindow);
    }
  }
}
