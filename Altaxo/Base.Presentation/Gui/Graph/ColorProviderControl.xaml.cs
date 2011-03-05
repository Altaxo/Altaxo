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
	/// Interaction logic for ColorProviderControl.xaml
	/// </summary>
	public partial class ColorProviderControl : UserControl, IColorProviderView
	{
		GdiToWpfBitmap _previewBitmap = new GdiToWpfBitmap(4, 4);

		public ColorProviderControl()
		{
			InitializeComponent();
		}

		private void EhColorProviderChanged(object sender, SelectionChangedEventArgs e)
		{
			GuiHelper.SynchronizeSelectionFromGui(_cbColorProvider);
			if (null != ColorProviderChanged)
				ColorProviderChanged();
		}

		#region

		public void InitializeAvailableClasses(Collections.SelectableListNodeList names)
		{
			GuiHelper.Initialize(_cbColorProvider, names);
		}

		public void SetDetailView(object guiobject)
		{
			_detailsPanel.Child = guiobject as UIElement;
		}

		/// <summary>
		/// Gets a bitmap with a certain size.
		/// </summary>
		/// <param name="width">Pixel width of the bitmap.</param>
		/// <param name="height">Pixel height of the bitmap.</param>
		/// <returns>A bitmap that can be used for drawing.</returns>
		public System.Drawing.Bitmap GetPreviewBitmap(int width, int height)
		{
			if (_previewBitmap.GdiBitmap.Width != width || _previewBitmap.GdiBitmap.Height != height)
			{
				_previewBitmap.Resize(width, height);
				_previewPanel.Source = _previewBitmap.WpfBitmap;
			}

			return _previewBitmap.GdiBitmap;
		}


		public void SetPreviewBitmap(System.Drawing.Bitmap bitmap)
		{
			_previewBitmap.WpfBitmap.Invalidate();

		}

		public event Action ColorProviderChanged;

		#endregion
	}
}
