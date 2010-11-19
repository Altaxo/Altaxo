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

using Altaxo.Collections;

namespace Altaxo.Gui.Worksheet
{
	/// <summary>
	/// Interaction logic for InterpolationControl.xaml
	/// </summary>
	public partial class InterpolationControl : UserControl, IInterpolationParameterView
	{
		public event Action<ValidationEventArgs<string>> ValidatingFrom;
		public event Action<ValidationEventArgs<string>> ValidatingTo;
		public event Action<ValidationEventArgs<string>> ValidatingNumberOfPoints;
		public event Action ChangedInterpolationMethod;

		public InterpolationControl()
		{
			InitializeComponent();
		}

		public void InitializeClassList(SelectableListNodeList list)
		{
			GuiHelper.Initialize(_cbInterpolationClass, list);
		}

		public void InitializeNumberOfPoints(string val)
		{
			this._edNumberOfPoints.Text = val;
		}

		public void InitializeXOrg(string val)
		{
			this._edFrom.Text = val;
		}

		public void InitializeXEnd(string val)
		{
			this._edTo.Text = val;
		}

		UIElement _detailControl;
		public void SetDetailControl(object detailControl)
		{
			if (_detailControl != null)
				_mainGrid.Children.Remove(_detailControl);

			if (detailControl is System.Windows.Forms.Control)
			{
				var host = new System.Windows.Forms.Integration.WindowsFormsHost();
				host.Child = (System.Windows.Forms.Control)detailControl;
				_detailControl = host;
			}
			else
			{
				_detailControl = (UIElement)detailControl;
			}

			if (null != _detailControl)
			{
				_detailControl.SetValue(Grid.RowProperty, 4);
				_detailControl.SetValue(Grid.ColumnProperty, 2);
				_mainGrid.Children.Add(_detailControl);
			}
			
		}

		private void EhValueTo_Validating(object sender, ValidationEventArgs<string> e)
		{
			if (null != ValidatingTo)
				ValidatingTo(e);
		}

		private void EhValueNumberOfPoints_Validating(object sender, Altaxo.Gui.ValidationEventArgs<string> e)
		{
			if (null != ValidatingNumberOfPoints)
				ValidatingNumberOfPoints(e);
		}

		private void EhValueFrom_Validating(object sender, Altaxo.Gui.ValidationEventArgs<string> e)
		{
			if (null != ValidatingFrom)
				ValidatingFrom(e);
		}

		private void EhInterpolationClassChanged(object sender, SelectionChangedEventArgs e)
		{
			GuiHelper.SynchronizeSelectionFromGui(_cbInterpolationClass);
			if (null != ChangedInterpolationMethod)
				ChangedInterpolationMethod();
		}
	}
}
