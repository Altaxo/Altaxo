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
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Altaxo.Gui.Common.Drawing
{
	/// <summary>
	/// Interaction logic for FontXControl.xaml
	/// </summary>
	public partial class FontXControl : UserControl, IFontXView
	{
		public FontXControl()
		{
			InitializeComponent();
		}

		public System.Drawing.FontFamily SelectedFontFamily
		{
			get
			{
				return _guiFontFamily.SelectedFontFamily;
			}
			set
			{
				_guiFontFamily.SelectedFontFamily = value;
			}
		}

		public double SelectedFontSize
		{
			get
			{
				return _guiFontSize.SelectedQuantityAsValueInPoints;
			}
			set
			{
				_guiFontSize.SelectedQuantityAsValueInPoints = value;
			}
		}
	}
}