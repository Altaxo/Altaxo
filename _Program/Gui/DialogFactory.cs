using System;

using Altaxo.Graph;

namespace Altaxo.Gui
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

			Gui.DialogShellController dsc = new Gui.DialogShellController(
				new Gui.DialogShellView(panel),ct,"Add new column(s)",false);

			
			if(true==dsc.ShowDialog(owner))
			{
				System.Type columntype = (System.Type)ct.SelectedItem.Tag;

				table.SuspendDataChangedNotifications();

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
						table.Add((Altaxo.Data.DataColumn)Activator.CreateInstance(columntype));
					}
				}

				table.ResumeDataChangedNotifications();
			}

		}




		public static bool ShowLineScatterPlotStyleAndDataDialog(System.Windows.Forms.Form parentWindow, Graph.PlotItem pa, PlotGroup plotGroup)
		{
			// Plot Style
			Graph.LineScatterPlotStyleController	stylectrl = new Graph.LineScatterPlotStyleController((Graph.PlotStyle)pa.Style,plotGroup);
			Graph.LineScatterPlotStyleControl			styleview = new Graph.LineScatterPlotStyleControl();
			stylectrl.View = styleview;

			// Plot Data
			Graph.LineScatterPlotDataController datactrl = new Graph.LineScatterPlotDataController((PlotAssociation)pa.Data);
			Graph.LineScatterPlotDataControl    dataview = new Graph.LineScatterPlotDataControl();
			datactrl.View = dataview;

			Gui.TabbedDialogController tdcctrl = new Gui.TabbedDialogController("Line/Scatter Plot",true);
			tdcctrl.AddTab("Style",stylectrl,styleview);
			tdcctrl.AddTab("Data",datactrl,dataview);
			Gui.TabbedDialogView  tdcview = new Gui.TabbedDialogView();
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


	}
}
