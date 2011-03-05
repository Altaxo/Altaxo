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

namespace Altaxo.Gui.Graph
{
	/// <summary>
	/// Interaction logic for DensityScaleControl.xaml
	/// </summary>
	public partial class DensityScaleControl : UserControl, IDensityScaleView
	{
		public DensityScaleControl()
		{
			InitializeComponent();
		}

		private void EhScaleSelectionChangeCommitted(object sender, SelectionChangedEventArgs e)
		{
			GuiHelper.SynchronizeSelectionFromGui(_cbScales);
			if (null != AxisTypeChanged)
				AxisTypeChanged();
		}

		#region IDensityScaleView Members

		public void InitializeAxisType(Altaxo.Collections.SelectableListNodeList names)
		{
			GuiHelper.Initialize(_cbScales, names);
		}

		private UserControl _boundaryControl = null;
		public void SetBoundaryView(object guiobject)
		{
			_boundaryHost.Child = guiobject as UIElement;
		}

		public void SetScaleView(object guiobject)
		{
			_scaleViewHost.Child = guiobject as UIElement;
		}

		public event Action AxisTypeChanged;

		#endregion

	}
}
