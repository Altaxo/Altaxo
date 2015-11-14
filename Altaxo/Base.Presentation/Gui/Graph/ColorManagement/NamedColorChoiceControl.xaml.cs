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

using Altaxo.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Altaxo.Gui.Graph.ColorManagement
{
	/// <summary>
	/// Interaction logic for NamedColorChoiceControl.xaml
	/// </summary>
	public partial class NamedColorChoiceControl : UserControl, INamedColorChoiceView
	{
		public NamedColorChoiceControl()
		{
			InitializeComponent();
		}

		private void EhSelectedColorChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			_guiColorPreview.Fill = new SolidColorBrush(_guiColor.SelectedWpfColor);
		}

		public bool ShowPlotColorsOnly
		{
			set
			{
				_guiColor.ShowPlotColorsOnly = value;
			}
		}

		public NamedColor SelectedColor
		{
			get
			{
				return _guiColor.SelectedColor;
			}
			set
			{
				_guiColor.SelectedColor = value;
			}
		}
	}
}