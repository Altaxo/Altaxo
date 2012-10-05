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

namespace Altaxo.Gui.Common.Drawing
{
	/// <summary>
	/// Interaction logic for PenSimpleConditionalControl.xaml
	/// </summary>
	public partial class PenSimpleConditionalControl : UserControl
	{
		PenControlsGlue _glue;

		public PenSimpleConditionalControl()
		{
			InitializeComponent();

			_glue = new PenControlsGlue(false);
			_glue.CbBrush = _cbBrush;
			_glue.CbDashStyle = _cbDashStyle;
			_glue.CbLineThickness = _cbThickness;
		}

		public Altaxo.Graph.Gdi.PenX SelectedPen
		{
			get
			{
				if (_chkDoShowThis.IsChecked == true)
				{
					return _glue.Pen;
				}
				else
				{
					var pen = _glue.Pen;
					pen.Color = Altaxo.Graph.NamedColors.Transparent;
					return pen;
				}
			}
			set
			{
				_glue.Pen = value;
				_chkDoShowThis.IsChecked = value.IsVisible;
			}
		}

		private void EhShowOutline_Checked(object sender, RoutedEventArgs e)
		{
			if (!_glue.Pen.IsVisible)
			{
				var pen = _glue.Pen;
				pen.Color = Altaxo.Graph.NamedColors.Black;
				_glue.Pen = pen;
			}
		}

	}
}
