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
	/// Interaction logic for LayerPositionControl.xaml
	/// </summary>
	public partial class LayerPositionControl : UserControl, ILayerPositionView
	{
		public LayerPositionControl()
		{
			InitializeComponent();
		}

		public event Action PositioningTypeChanged;

		private bool _useDirectPositioning;

		public bool UseDirectPositioning
		{
			get
			{
				return _useDirectPositioning;
			}
			set
			{
				_useDirectPositioning = value;

				if (_useDirectPositioning)
					_guiDirectPositioning.IsChecked = true;
				else
					_guiGridPositioning.IsChecked = true;
			}
		}

		public object SubPositionView
		{
			set { _guiSubPositioningHost.Child = (System.Windows.UIElement)value; }
		}

		private void EhPositioningTypeChangedToDirect(object sender, RoutedEventArgs e)
		{
			var oldValue = _useDirectPositioning;
			_useDirectPositioning = true;

			if (_useDirectPositioning != oldValue && PositioningTypeChanged != null)
				PositioningTypeChanged();
		}

		private void EhPositioningTypeChangedToGrid(object sender, RoutedEventArgs e)
		{
			var oldValue = _useDirectPositioning;
			_useDirectPositioning = false;

			if (_useDirectPositioning != oldValue && PositioningTypeChanged != null)
				PositioningTypeChanged();
		}

		public bool IsPositioningTypeChoiceVisible
		{
			set
			{
				var visibility = value ? Visibility.Visible : System.Windows.Visibility.Collapsed;
				_guiDirectPositioning.Visibility = visibility;
				_guiGridPositioning.Visibility = visibility;
			}
		}
	}
}