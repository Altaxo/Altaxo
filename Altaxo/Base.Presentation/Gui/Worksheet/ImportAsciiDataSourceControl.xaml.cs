#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2014 Dr. Dirk Lellinger
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
using System.Windows.Input;

namespace Altaxo.Gui.Worksheet
{
	/// <summary>
	/// Interaction logic for RealFourierTransformation2DDataSourceControl.xaml
	/// </summary>
	public partial class ImportAsciiDataSourceControl : UserControl, IImportAsciiDataSourceView
	{
		public event Action BrowseSelectedFileName;

		public event Action DeleteSelectedFileName;

		public event Action MoveUpSelectedFileName;

		public event Action MoveDownSelectedFileName;

		public event Action AddNewFileName;

		public event Action SortFileNamesAscending;

		public ImportAsciiDataSourceControl()
		{
			InitializeComponent();
		}

		public void SetAsciiImportOptionsControl(object p)
		{
			_guiAsciiImportOptionsHost.Child = p as UIElement;
		}

		public void SetImportOptionsControl(object p)
		{
			_guiImportOptionsHost.Child = p as UIElement;
		}

		public Collections.SelectableListNodeList FileNames
		{
			set { GuiHelper.Initialize(_guiFileNames, value); }
		}

		private void EhFileNamesMouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			GuiHelper.SynchronizeSelectionFromGui(_guiFileNames);

			var ev = BrowseSelectedFileName;
			if (null != ev)
				ev();
		}

		private void EhRemoveFileName(object sender, RoutedEventArgs e)
		{
			GuiHelper.SynchronizeSelectionFromGui(_guiFileNames);
			var ev = DeleteSelectedFileName;
			if (null != ev)
				ev();
		}

		private void EhAddNewFileName(object sender, RoutedEventArgs e)
		{
			GuiHelper.SynchronizeSelectionFromGui(_guiFileNames);
			var ev = AddNewFileName;
			if (null != ev)
				ev();
		}

		private void EhMoveDownFileName(object sender, RoutedEventArgs e)
		{
			GuiHelper.SynchronizeSelectionFromGui(_guiFileNames);
			var ev = MoveDownSelectedFileName;
			if (null != ev)
				ev();
		}

		private void EhMoveUpFileName(object sender, RoutedEventArgs e)
		{
			GuiHelper.SynchronizeSelectionFromGui(_guiFileNames);
			var ev = MoveUpSelectedFileName;
			if (null != ev)
				ev();
		}

		private void EhSortFileNamesAscending(object sender, RoutedEventArgs e)
		{
			GuiHelper.SynchronizeSelectionFromGui(_guiFileNames);
			var ev = SortFileNamesAscending;
			if (null != ev)
				ev();
		}
	}
}