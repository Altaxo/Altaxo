#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2022 Dr. Dirk Lellinger
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

#nullable disable warnings
using System.Windows;
using System.Windows.Controls;

namespace Altaxo.Gui.Graph.Plot.Data
{
  /// <summary>
  /// Interaction logic for SingleColumnControl.xaml
  /// </summary>
  public partial class SingleColumnControl : Grid
  {

    public SingleColumnControl()
    {
      InitializeComponent();
    }

    private void EhPopupFocusChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      if (false == (bool)e.NewValue)
        _guiPopup.IsOpen = false;
    }
  }
}
