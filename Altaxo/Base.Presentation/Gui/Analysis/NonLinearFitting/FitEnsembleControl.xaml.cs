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

namespace Altaxo.Gui.Analysis.NonLinearFitting
{
	/// <summary>
	/// Interaction logic for FitEnsembleControl.xaml
	/// </summary>
	[UserControlForController(typeof(IFitEnsembleViewEventSink))]
	public partial class FitEnsembleControl : UserControl, IFitEnsembleView
	{
		public FitEnsembleControl()
		{
			InitializeComponent();
		}

		#region  IFitEnsembleView

		IFitEnsembleViewEventSink _controller;
		public IFitEnsembleViewEventSink Controller
		{
			get
			{
				return _controller;
			}
			set
			{
				_controller = value;
			}
		}

		public void Initialize(Calc.Regression.Nonlinear.FitEnsemble ensemble, object[] fitEleControls)
		{
			_itemsHost.Children.Clear();

			foreach (UIElement ele in fitEleControls)
				_itemsHost.Children.Add(ele);
		}

		#endregion
	}
}
