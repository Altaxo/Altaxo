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

using Altaxo.Graph;

namespace Altaxo.Gui.Graph.Shapes
{
	/// <summary>
	/// Interaction logic for AnchoringControl.xaml
	/// </summary>
	public partial class AnchoringControl : UserControl
	{
		RadioButton[,] _buttons;

		XAnchorPositionType _xAnchor;
		YAnchorPositionType _yAnchor;

		public AnchoringControl()
		{
			InitializeComponent();
			_buttons = new RadioButton[3,3]{{_guiLeftTop, _guiCenterTop, _guiRightTop },{_guiLeftCenter, _guiCenterCenter, _guiRightCenter },{_guiLeftBottom, _guiCenterBottom, _guiRightBottom }};
			SetRadioButton();
		}

		private void EhRadioChecked(object sender, RoutedEventArgs e)
		{
			for (int i = 0; i < 3; ++i)
			{
				for (int j = 0; j < 3; ++j)
				{
					if (object.Equals(sender, _buttons[j, i]))
					{
						_xAnchor = (XAnchorPositionType)i;
						_yAnchor = (YAnchorPositionType)j;
					}
				}
			}
		}

		private void SetRadioButton()
		{
			int i = (int)_xAnchor;
			int j = (int)_yAnchor;
			_buttons[j, i].IsChecked = true;
		}

		public XAnchorPositionType SelectedXAnchor
		{
			get
			{
				return _xAnchor;
			}
			set
			{
				var oldValue = _xAnchor;
				_xAnchor = value;
				if (value != oldValue)
					SetRadioButton();
			}
		}


		public YAnchorPositionType SelectedYAnchor
		{
			get
			{
				return _yAnchor;
			}
			set
			{
				var oldValue = _yAnchor;
				_yAnchor = value;
				if (value != oldValue)
					SetRadioButton();
			}
		}
	}
}
