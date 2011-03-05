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
	/// Interaction logic for GraphExportOptionsControl.xaml
	/// </summary>
	public partial class GraphExportOptionsControl : UserControl, IGraphExportOptionsView
	{
		public GraphExportOptionsControl()
		{
			InitializeComponent();
		}

		private void EhImageFormatSelected(object sender, SelectionChangedEventArgs e)
		{
			GuiHelper.SynchronizeSelectionFromGui(_cbImageFormat);

		}

		private void EhPixelFormatSelected(object sender, SelectionChangedEventArgs e)
		{
			GuiHelper.SynchronizeSelectionFromGui(_cbPixelFormat);

		}

		private void EhExportAreaSelected(object sender, SelectionChangedEventArgs e)
		{
			GuiHelper.SynchronizeSelectionFromGui(_cbExportArea);

		}

		private void EhClipboardFormatSelected(object sender, SelectionChangedEventArgs e)
		{
			GuiHelper.SynchronizeSelectionFromGui(_cbClipboardFormat);
		}

		#region IGraphExportView Members

		public void SetImageFormat(Altaxo.Collections.SelectableListNodeList list)
		{
			GuiHelper.Initialize(_cbImageFormat, list);
		}

		public void SetPixelFormat(Altaxo.Collections.SelectableListNodeList list)
		{
			GuiHelper.Initialize(_cbPixelFormat, list);
		}

		public void SetExportArea(Altaxo.Collections.SelectableListNodeList list)
		{
			GuiHelper.Initialize(_cbExportArea, list);
		}

		public void SetSourceDpi(Altaxo.Collections.SelectableListNodeList list)
		{
			GuiHelper.Initialize(_cbSourceResolution, list);

		}

		public void SetDestinationDpi(Altaxo.Collections.SelectableListNodeList list)
		{
			GuiHelper.Initialize(_cbDestinationResolution, list);
		}

		public bool EnableClipboardFormat
		{
			set
			{
				_cbClipboardFormat.IsEnabled = value;
				_lblClipboardFormat.IsEnabled = value;
			}
		}


		public void SetClipboardFormat(Altaxo.Collections.SelectableListNodeList list)
		{
			GuiHelper.Initialize(_cbClipboardFormat, list);
		}

		public string SourceDpiResolution
		{
			get { return _cbSourceResolution.Text; }
		}

		public string DestinationDpiResolution
		{
			get { return _cbDestinationResolution.Text; }
		}

		public Altaxo.Graph.Gdi.BrushX BackgroundBrush
		{
			get
			{
				return _cbBackgroundBrush.SelectedBrush;
			}
			set
			{
				_cbBackgroundBrush.SelectedBrush = value;
			}
		}

		#endregion

	}
}
