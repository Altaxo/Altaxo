using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Altaxo.Main.Services;

namespace Altaxo.Gui.Graph
{
	public partial class GraphExportOptionsControl : UserControl, IGraphExportOptionsView
	{
		public GraphExportOptionsControl()
		{
			InitializeComponent();
		}

		#region IGraphExportView Members

		public void SetImageFormat(Altaxo.Collections.SelectableListNodeList list)
		{
			GUIFactoryService.InitComboBox(_cbImageFormat, list);
		}

		public void SetPixelFormat(Altaxo.Collections.SelectableListNodeList list)
		{
			GUIFactoryService.InitComboBox(_cbPixelFormat, list);
		}

		public void SetExportArea(Altaxo.Collections.SelectableListNodeList list)
		{
			GUIFactoryService.InitComboBox(_cbExportArea, list);
		}

		public void SetSourceDpi(Altaxo.Collections.SelectableListNodeList list)
		{
			GUIFactoryService.InitComboBox(_cbSourceResolution, list);

		}

		public void SetDestinationDpi(Altaxo.Collections.SelectableListNodeList list)
		{
			GUIFactoryService.InitComboBox(_cbDestinationResolution, list);

		}

		public string SourceDpiResolution
		{
			get { return _cbSourceResolution.Text; }
		}

		public string DestinationDpiResolution
		{
			get { return _cbDestinationResolution.Text; }
		}

		#endregion

		private void EhImageFormatSelected(object sender, EventArgs e)
		{
			GUIFactoryService.SynchronizeSelectableListNodes(_cbImageFormat);
		}

		private void EhPixelFormatSelected(object sender, EventArgs e)
		{
			GUIFactoryService.SynchronizeSelectableListNodes(_cbPixelFormat);
		}

		private void EhExportAreaSelected(object sender, EventArgs e)
		{
			GUIFactoryService.SynchronizeSelectableListNodes(_cbExportArea);
		}
	}
}
