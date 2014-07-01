using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Altaxo.Gui.Data
{
	/// <summary>
	/// Interaction logic for DataSourceImportOptionsControl.xaml
	/// </summary>
	public partial class DataSourceImportOptionsControl : UserControl, IDataSourceImportOptionsView
	{
		public DataSourceImportOptionsControl()
		{
			InitializeComponent();
		}

		public bool DoNotSaveTableData
		{
			get
			{
				return _guiDoNotSaveTableData.IsChecked == true;
			}
			set
			{
				_guiDoNotSaveTableData.IsChecked = value;
			}
		}

		public bool ExecuteScriptAfterImport
		{
			get
			{
				return _guiExecuteWksScriptAfterImport.IsChecked == true;
			}
			set
			{
				_guiExecuteWksScriptAfterImport.IsChecked = value;
			}
		}

		public void InitializeTriggerSource(Altaxo.Collections.SelectableListNodeList list)
		{
			_guiTrigger.Initialize(list);
		}

		public double MinimumTimeIntervalBetweenUpdatesInSeconds
		{
			get
			{
				return _guiMinTimeBetweenUpdates.SelectedValue;
			}
			set
			{
				_guiMinTimeBetweenUpdates.SelectedValue = value;
			}
		}

		public double PollTimeIntervalInSeconds
		{
			get
			{
				return _guiPollingTime.SelectedValue;
			}
			set
			{
				_guiPollingTime.SelectedValue = value;
			}
		}
	}
}