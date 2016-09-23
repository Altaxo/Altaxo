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

#endregion Copyright

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Altaxo.Gui.Graph.Gdi
{
	/// <summary>
	/// Interaction logic for GraphExportOptionsControl.xaml
	/// </summary>
	public partial class ClipboardRenderingOptionsControl : UserControl, IClipboardRenderingOptionsView
	{
		public ClipboardRenderingOptionsControl()
		{
			InitializeComponent();
		}

		public bool RenderDropfile
		{
			get
			{
				return _guiRenderDropfile.IsChecked == true;
			}
			set
			{
				_guiRenderDropfile.IsChecked = value;
			}
		}

		public void SetDropFileImageFormat(Altaxo.Collections.SelectableListNodeList list)
		{
			GuiHelper.Initialize(_cbImageFormat, list);
		}

		public void SetDropFilePixelFormat(Altaxo.Collections.SelectableListNodeList list)
		{
			GuiHelper.Initialize(_cbPixelFormat, list);
		}

		public bool RenderEmbeddedObject
		{
			get
			{
				return _guiRenderEmbeddedObject.IsChecked == true;
			}
			set
			{
				_guiRenderEmbeddedObject.IsChecked = value;
			}
		}

		public bool RenderLinkedObject
		{
			get
			{
				return _guiRenderLinkedObject.IsChecked == true;
			}
			set
			{
				_guiRenderLinkedObject.IsChecked = value;
			}
		}

		private void EhImageFormatSelected(object sender, SelectionChangedEventArgs e)
		{
			GuiHelper.SynchronizeSelectionFromGui(_cbImageFormat);
		}

		private void EhPixelFormatSelected(object sender, SelectionChangedEventArgs e)
		{
			GuiHelper.SynchronizeSelectionFromGui(_cbPixelFormat);
		}

		public object EmbeddedRenderingOptionsView
		{
			set { _guiEmbeddedOptionsViewHost.Child = value as UIElement; }
		}
	}
}