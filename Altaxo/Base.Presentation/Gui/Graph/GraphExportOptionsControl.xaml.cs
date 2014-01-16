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
				_guiClipboardFormatHost.IsEnabled = value;
				_lblClipboardFormat.IsEnabled = value;
			}
		}

		public void SetClipboardFormatView(object viewObject)
		{
			_guiClipboardFormatHost.Child = viewObject as UIElement;
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

		#endregion IGraphExportView Members
	}
}