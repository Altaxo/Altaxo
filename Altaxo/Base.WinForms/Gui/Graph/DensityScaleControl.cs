using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Altaxo.Gui.Graph
{
	public partial class DensityScaleControl : UserControl, IDensityScaleView
	{
		public DensityScaleControl()
		{
			InitializeComponent();
		}

		
		#region IDensityScaleView Members

		public void InitializeAxisType(Altaxo.Collections.SelectableListNodeList names)
		{
			GuiHelper.UpdateList(_cbScales, names);
		}

		[BrowsableAttribute(false)]
		private UserControl _boundaryControl = null;
		public void SetBoundaryView(object guiobject)
		{
			if (null != _boundaryControl)
			 _tableLayoutPanel.Controls.Remove(_boundaryControl);

			_boundaryControl = guiobject as UserControl;


			if (_boundaryControl != null)
			{
				_tableLayoutPanel.Controls.Add(_boundaryControl);
				_tableLayoutPanel.SetColumnSpan(_boundaryControl, 2);
				_tableLayoutPanel.SetCellPosition(_boundaryControl, new TableLayoutPanelCellPosition(0, 2));
			}
		}

		[BrowsableAttribute(false)]
		private Control _scaleControl = null;
		public void SetScaleView(object guiobject)
		{
			if (null != _scaleControl)
				_tableLayoutPanel.Controls.Remove(_scaleControl);

			_scaleControl = guiobject as Control;

			if (_scaleControl != null)
			{
				_tableLayoutPanel.Controls.Add(_scaleControl);
				_tableLayoutPanel.SetColumnSpan(_scaleControl, 2);
				_tableLayoutPanel.SetCellPosition(_scaleControl, new TableLayoutPanelCellPosition(0, 1));
			}
		}

		public event Action AxisTypeChanged;

		#endregion


		private void EhScaleSelectionChangeCommitted(object sender, EventArgs e)
		{
			GuiHelper.SynchronizeSelectionFromGui(_cbScales);
			if (null != AxisTypeChanged)
				AxisTypeChanged();
		}

	}
}
