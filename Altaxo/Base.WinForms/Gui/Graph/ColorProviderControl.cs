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
	public partial class ColorProviderControl : UserControl, IColorProviderView
	{
		public ColorProviderControl()
		{
			InitializeComponent();
		}


		#region IColorProviderView Members

		public void InitializeAvailableClasses(Altaxo.Collections.SelectableListNodeList names)
		{
			GuiHelper.UpdateList(_cbColorProvider, names);
		}

		[BrowsableAttribute(false)]
		private UserControl _detailControl = null;
		public void SetDetailView(object guiobject)
		{
			if (null != _detailControl)
				_tableLayoutPanel.Controls.Remove(_detailControl);

			_detailControl = guiobject as UserControl;


			if (_detailControl != null)
			{
				_tableLayoutPanel.Controls.Add(_detailControl);
				_tableLayoutPanel.SetColumnSpan(_detailControl, 2);
				_tableLayoutPanel.SetCellPosition(_detailControl, new TableLayoutPanelCellPosition(0, 1));
			}
		}


		/// <summary>
		/// Gets a bitmap with a certain size.
		/// </summary>
		/// <param name="width">Pixel width of the bitmap.</param>
		/// <param name="height">Pixel height of the bitmap.</param>
		/// <returns>A bitmap that can be used for drawing.</returns>
		public System.Drawing.Bitmap GetPreviewBitmap(int width, int height)
		{
			return new System.Drawing.Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
		}

		public void SetPreviewBitmap(Bitmap bitmap)
		{
			_previewPanel.Image = bitmap;
		}

		public event Action ColorProviderChanged;

		#endregion

		private void EhColorProviderChanged(object sender, EventArgs e)
		{
			GuiHelper.SynchronizeSelectionFromGui(_cbColorProvider);
			if (null != ColorProviderChanged)
				ColorProviderChanged();
		}
	}
}
