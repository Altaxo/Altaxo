#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2004 Dr. Dirk Lellinger
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

using Altaxo.Graph;
using Altaxo.Graph.GUI;


namespace Altaxo.Main.GUI
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

      GUI.DialogShellController dsc = new GUI.DialogShellController(
        new GUI.DialogShellView(panel),ct,"Add new column(s)",false);

      
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


    public static bool ShowPlotStyleAndDataDialog(System.Windows.Forms.Form parentWindow, Graph.PlotItem pa, PlotGroup plotGroup)
    {
      if(pa is Graph.XYColumnPlotItem || pa is Graph.XYFunctionPlotItem)
        return ShowLineScatterPlotStyleAndDataDialog(parentWindow,pa,plotGroup);
      else if(pa is Graph.DensityImagePlotItem)
        return ShowDensityImagePlotStyleAndDataDialog(parentWindow,pa,plotGroup);
      else
      {
        System.Windows.Forms.MessageBox.Show(parentWindow,"Sorry, a configuration dialog for " + pa.GetType().ToString() + " is not yet implemented!");
        return false;
      }
    }


    public static bool ShowLineScatterPlotStyleAndDataDialog(System.Windows.Forms.Form parentWindow, Graph.PlotItem pa, PlotGroup plotGroup)
    {
      
      // Plot Style
      AbstractXYPlotStyle style = null;

      if(pa is XYColumnPlotItem)
        style = ((XYColumnPlotItem)pa).Style;
      else if(pa is XYFunctionPlotItem)
        style = ((XYFunctionPlotItem)pa).Style;

      LineScatterPlotStyleController  stylectrl = new LineScatterPlotStyleController(style,plotGroup);
      LineScatterPlotStyleControl     styleview = new LineScatterPlotStyleControl();
      stylectrl.View = styleview;

      // Plot Data
      XYColumnPlotData plotData = ((XYColumnPlotItem)pa).Data as XYColumnPlotData;
      LineScatterPlotDataController datactrl=null;
      LineScatterPlotDataControl    dataview=null;
      if(plotData!=null)
      {
        datactrl = new LineScatterPlotDataController(plotData);
        dataview = new LineScatterPlotDataControl();
        datactrl.View = dataview;
      }

      // Label Style
      Graph.GUI.XYPlotLabelStyleController labelctrl = null;
      Graph.GUI.XYPlotLabelStyleControl    labelview = null;
      if(style is XYLineScatterPlotStyle)
      {
        XYLineScatterPlotStyle plotStyle = style as XYLineScatterPlotStyle;
        if(plotStyle.XYPlotLabelStyle!=null)
        {
          labelctrl = new XYPlotLabelStyleController(plotStyle.XYPlotLabelStyle);
          labelview = new XYPlotLabelStyleControl();
          labelctrl.View = labelview;
        }
      }

      GUI.TabbedDialogController tdcctrl = new GUI.TabbedDialogController("Line/Scatter Plot",true);
      tdcctrl.AddTab("Style",stylectrl,styleview);
      
      if(datactrl!=null)
        tdcctrl.AddTab("Data",datactrl,dataview);

      if(labelctrl!=null && labelview!=null)
        tdcctrl.AddTab("Label",labelctrl,labelview);
      GUI.TabbedDialogView  tdcview = new GUI.TabbedDialogView();
      tdcctrl.View = tdcview;

      bool result = tdcctrl.ShowDialog(parentWindow);

      if(plotData.LabelColumn==null && style is XYLineScatterPlotStyle)
      {
        ((XYLineScatterPlotStyle)style).XYPlotLabelStyle = null;
      }


      return result;
    }

    public static bool ShowDensityImagePlotStyleAndDataDialog(System.Windows.Forms.Form parentWindow, Graph.PlotItem pa, PlotGroup plotGroup)
    {
      // Plot Style
      DensityImagePlotStyleController stylectrl = new DensityImagePlotStyleController(((Graph.DensityImagePlotItem)pa).Style);
      DensityImagePlotStyleControl      styleview = new DensityImagePlotStyleControl();
      stylectrl.View = styleview;

      GUI.TabbedDialogController tdcctrl = new GUI.TabbedDialogController("Density Image Plot",true);
      tdcctrl.AddTab("Style",stylectrl,styleview);
      GUI.TabbedDialogView  tdcview = new GUI.TabbedDialogView();
      tdcctrl.View = tdcview;

      return tdcctrl.ShowDialog(parentWindow);
    }


    public static bool ShowColumnScriptDialog(System.Windows.Forms.Form parentWindow, Altaxo.Data.DataTable dataTable, Altaxo.Data.DataColumn dataColumn, Altaxo.Data.ColumnScript columnScript)
    {
      Worksheet.GUI.ColumnScriptController controller = new Worksheet.GUI.ColumnScriptController(dataTable,dataColumn,columnScript);
      Worksheet.GUI.ColumnScriptControl control = new Altaxo.Worksheet.GUI.ColumnScriptControl();

      System.Windows.Forms.Form form = new System.Windows.Forms.Form(); // the parent form used as shell for the control
      form.Controls.Add(control);
      form.ClientSize = control.Size;
      control.Dock = System.Windows.Forms.DockStyle.Fill;
      controller.View = control;


      return System.Windows.Forms.DialogResult.OK == form.ShowDialog(parentWindow);
    }

  
    /// <summary>
    /// Shows the polynomial fit dialog.
    /// </summary>
    /// <param name="parentWindow">The window in whose context the dialog is shown.</param>
    /// <param name="controller">The polynomial fit controller. Contains the user choices at the end of this routine.</param>
    /// <returns>True if the user has pressed OK.</returns>
    public static bool ShowPolynomialFitDialog(System.Windows.Forms.Form parentWindow, Graph.GUI.IFitPolynomialDialogController controller)
    {
      Graph.GUI.FitPolynomialDialogControl panel = new Graph.GUI.FitPolynomialDialogControl();
      controller.ViewObject = panel;

      GUI.DialogShellController dsc = new GUI.DialogShellController(
        new GUI.DialogShellView(panel),controller,"Polynomial fit",false);
 

      return dsc.ShowDialog(parentWindow);
    }
  }
}
