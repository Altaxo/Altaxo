using System;

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

			Main.DialogShellController dsc = new Main.DialogShellController(
				new Main.DialogShellView(panel),ct,"Add new column(s)",false);

			
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


	}
}
