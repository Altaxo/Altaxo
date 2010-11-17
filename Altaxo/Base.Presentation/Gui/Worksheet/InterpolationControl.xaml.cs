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
	/// Interaction logic for InterpolationControl.xaml
	/// </summary>
	public partial class InterpolationControl : UserControl, IInterpolationParameterView
	{
		public InterpolationControl()
		{
			InitializeComponent();
		}

		public IInterpolationParameterViewEventSink Controller
		{
			set { throw new NotImplementedException(); }
		}

		public void InitializeClassList(string[] classes, int preselection)
		{
			throw new NotImplementedException();
		}

		public void InitializeNumberOfPoints(int val)
		{
			throw new NotImplementedException();
		}

		public void InitializeXOrg(double val)
		{
			throw new NotImplementedException();
		}

		public void InitializeXEnd(double val)
		{
			throw new NotImplementedException();
		}

		public void SetDetailControl(object detailControl)
		{
			throw new NotImplementedException();
		}
	}
}
