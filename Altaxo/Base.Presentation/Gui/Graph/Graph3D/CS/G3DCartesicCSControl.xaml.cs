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

namespace Altaxo.Gui.Graph.Graph3D.CS
{
  /// <summary>
  /// Interaction logic for G2DCartesicCSControl.xaml
  /// </summary>
  public partial class G3DCartesicCSControl : UserControl, IG3DCartesicCSView
  {
    public G3DCartesicCSControl()
    {
      InitializeComponent();
    }

    public bool ExchangeXY
    {
      get
      {
        return _chkExchangeXY.IsChecked == true;
      }
      set
      {
        _chkExchangeXY.IsChecked = value;
      }
    }

    public bool ReverseX
    {
      get
      {
        return _chkXReverse.IsChecked == true;
      }
      set
      {
        _chkXReverse.IsChecked = value;
      }
    }

    public bool ReverseY
    {
      get
      {
        return _chkYReverse.IsChecked == true;
      }
      set
      {
        _chkYReverse.IsChecked = value;
      }
    }

    public bool ReverseZ
    {
      get
      {
        return _chkZReverse.IsChecked == true;
      }
      set
      {
        _chkZReverse.IsChecked = value;
      }
    }
  }
}
