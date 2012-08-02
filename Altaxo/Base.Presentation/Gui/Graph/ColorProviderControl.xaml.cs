#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
//
//    This program is free software; you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation; either version 2 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with this program; if not, write to the Free Software
//    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
//
/////////////////////////////////////////////////////////////////////////////
#endregion

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
			_previewBitmap.EndGdiPainting();
		}

		public event Action ColorProviderChanged;

		#endregion
	}
}
