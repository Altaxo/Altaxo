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
			GuiHelper.UpdateList(_cbImageFormat, list);
		}

		public void SetPixelFormat(Altaxo.Collections.SelectableListNodeList list)
		{
			GuiHelper.UpdateList(_cbPixelFormat, list);
		}

		public void SetExportArea(Altaxo.Collections.SelectableListNodeList list)
		{
			GuiHelper.UpdateList(_cbExportArea, list);
		}

		public void SetSourceDpi(Altaxo.Collections.SelectableListNodeList list)
		{
			GuiHelper.UpdateList(_cbSourceResolution, list);

		}

		public void SetDestinationDpi(Altaxo.Collections.SelectableListNodeList list)
		{
			GuiHelper.UpdateList(_cbDestinationResolution, list);
		}

    public bool EnableClipboardFormat
    {
      set
      {
        _cbClipboardFormat.Enabled = value;
        _lblClipboardFormat.Enabled = value;
      }
    }


    public void SetClipboardFormat(Altaxo.Collections.SelectableListNodeList list)
    {
			GuiHelper.UpdateList(_cbClipboardFormat, list);
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
        return _cbBackgroundBrush.Brush;
      }
      set
      {
        _cbBackgroundBrush.Brush = value;
      }
    }

		#endregion

		private void EhImageFormatSelected(object sender, EventArgs e)
		{
			GuiHelper.SynchronizeSelectionFromGui(_cbImageFormat);
		}

		private void EhPixelFormatSelected(object sender, EventArgs e)
		{
			GuiHelper.SynchronizeSelectionFromGui(_cbPixelFormat);
		}

		private void EhExportAreaSelected(object sender, EventArgs e)
		{
			GuiHelper.SynchronizeSelectionFromGui(_cbExportArea);
		}

    private void EhClipboardFormatSelected(object sender, EventArgs e)
    {
			GuiHelper.SynchronizeSelectionFromGui(_cbClipboardFormat);
    }
	}
}
