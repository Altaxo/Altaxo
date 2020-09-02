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
using System.Windows.Controls;

namespace Altaxo.Gui.Graph.Scales.Ticks
{
  /// <summary>
  /// Interaction logic for AngularTickSpacingControl.xaml
  /// </summary>
  public partial class AngularTickSpacingControl : UserControl, IAngularTickSpacingView
  {
    public AngularTickSpacingControl()
    {
      InitializeComponent();
    }

    private void _cbMajorTicks_SelectedIndexChanged(object sender, SelectionChangedEventArgs e)
    {
      e.Handled = true;
      GuiHelper.SynchronizeSelectionFromGui(_cbMajorTicks);
      if (MajorTicksChanged is not null)
        MajorTicksChanged(sender, e);
    }

    private void _cbMinorTicks_SelectedIndexChanged(object sender, SelectionChangedEventArgs e)
    {
      e.Handled = true;
      GuiHelper.SynchronizeSelectionFromGui(_cbMinorTicks);
    }

    #region IAngularTickSpacingView

    public bool UsePositiveNegativeValues
    {
      get
      {
        return true == _chkPosNegValues.IsChecked;
      }
      set
      {
        _chkPosNegValues.IsChecked = value;
      }
    }

    public Collections.SelectableListNodeList MajorTicks
    {
      set { GuiHelper.Initialize(_cbMajorTicks, value); }
    }

    public Collections.SelectableListNodeList MinorTicks
    {
      set { GuiHelper.Initialize(_cbMinorTicks, value); }
    }

    public event EventHandler MajorTicksChanged;
  }

  #endregion IAngularTickSpacingView
}
