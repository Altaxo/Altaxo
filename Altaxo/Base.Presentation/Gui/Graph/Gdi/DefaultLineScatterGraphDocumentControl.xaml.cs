#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2014 Dr. Dirk Lellinger
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
	/// Interaction logic for DefaultLineScatterGraphDocumentControl.xaml
	/// </summary>
	public partial class DefaultLineScatterGraphDocumentControl : UserControl, IDefaultLineScatterGraphDocumentView, Altaxo.Gui.Graph.Graph3D.Templates.IDefaultCartesicPlotTemplateView
	{
		public event Action GraphFromProjectSelected;

		public DefaultLineScatterGraphDocumentControl()
		{
			InitializeComponent();
		}

		public Collections.SelectableListNodeList GraphsInProject
		{
			set { GuiHelper.Initialize(_guiGraphsInProject, value); }
		}

		private void EhGraphFromProjectSelected(object sender, RoutedEventArgs e)
		{
			GuiHelper.SynchronizeSelectionFromGui(_guiGraphsInProject);
			var ev = GraphFromProjectSelected;
			if (null != ev)
				ev();
		}

		public void SetPreviewBitmap(string title, System.Drawing.Bitmap bmp)
		{
			_guiPreviewTitle.Content = title;

			if (null != bmp)
				_guiPreview.Source = GuiHelper.ToWpf(bmp);
			else
				_guiPreview.Source = null;
		}
	}
}