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

namespace Altaxo.Gui.Worksheet
{
	/// <summary>
	/// Interaction logic for TransposeWorksheetControl.xaml
	/// </summary>
	public partial class TransposeWorksheetControl : UserControl, ITransposeWorksheetView
	{
		public TransposeWorksheetControl()
		{
			InitializeComponent();
		}


		/// <summary>
		/// Get/sets the number of data columns that are moved to the property columns before transposing the data columns.
		/// </summary>
		public int DataColumnsMoveToPropertyColumns
		{
			get { return _ctrlNumMovedDataCols.Value; }
			set
			{
				_ctrlNumMovedDataCols.Value = value;
			}
		}

		/// <summary>
		/// Get/sets the number of property columns that are moved after transposing the data columns to the data columns collection.
		/// </summary>
		public int PropertyColumnsMoveToDataColumns
		{
			get { return _ctrlNumMovedPropCols.Value; }
			set
			{
				_ctrlNumMovedPropCols.Value = value;
			}
		}

	}
}
