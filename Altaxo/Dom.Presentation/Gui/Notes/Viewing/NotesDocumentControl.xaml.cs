using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;

#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2018 Dr. Dirk Lellinger
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

using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Altaxo.Gui.Notes.Viewing
{
	/// <summary>
	/// Interaction logic for NotesDocumentControl.xaml
	/// </summary>
	public partial class NotesDocumentControl : UserControl, INotesDocumentView
	{
		public NotesDocumentControl()
		{
			InitializeComponent();
		}

		public string SourceText { get => _guiEditor.SourceText; set => _guiEditor.SourceText = value; }
		public string StyleName { set => _guiEditor.StyleName = value; }

		public event EventHandler SourceTextChanged
		{
			add
			{
				_guiEditor.SourceTextChanged += value;
			}
			remove
			{
				_guiEditor.SourceTextChanged -= value;
			}
		}
	}
}
