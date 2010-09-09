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
	using Altaxo.Collections;
	/// <summary>
	/// Interaction logic for ExpandCyclingVariableControl.xaml
	/// </summary>
	public partial class ExpandCyclingVariableControl : UserControl, IExpandCyclingVariableView
	{
		public ExpandCyclingVariableControl()
		{
			InitializeComponent();
		}

		public void InitializeCyclingVarColumn(SelectableListNodeList list)
		{
			_cbColWithCyclingVar.Initialize(list);
		}

		public void InitializeColumnsToAverage(SelectableListNodeList list)
		{
		 _lbColsToAverage.Initialize(list);
		}

		public void InitializeDestinationOutputFormat(SelectableListNodeList list)
		{
			_chDestinationOutput.Initialize(list);
		}

		public void InitializeDestinationX(SelectableListNodeList list)
		{
			_chDestinationX.Initialize(list);
		}

		public void InitializeDestinationColumnSorting(SelectableListNodeList list)
		{
			_chDestinationColSort.Initialize(list);
		}

		public void InitializeDestinationRowSorting(SelectableListNodeList list)
		{
			_chDestinationRowSort.Initialize(list);
		}


		
	}
}
