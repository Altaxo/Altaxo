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

namespace Altaxo.Gui.Graph.Graph3D.Axis
{
  /// <summary>
  /// Interaction logic for XYGridStyleControl.xaml
  /// </summary>
  public partial class XYGridStyleControl : UserControl, IXYGridStyleView
  {
    private NotifyChangedValue<bool> _isGridStyleEnabled = new NotifyChangedValue<bool>();
    private NotifyChangedValue<string> _headerTitle = new NotifyChangedValue<string>();

    public XYGridStyleControl()
    {
      InitializeComponent();
    }

    private void EhEnableCheckChanged(object sender, RoutedEventArgs e)
    {
      if (ShowGridChanged is not null)
        ShowGridChanged(true == _chkEnable.IsChecked);
    }

    private void EhShowZeroOnlyCheckChanged(object sender, RoutedEventArgs e)
    {
      if (ShowZeroOnlyChanged is not null)
        ShowZeroOnlyChanged(true == _chkShowZeroOnly.IsChecked);
    }

    private void EhShowMinorCheckChanged(object sender, RoutedEventArgs e)
    {
      if (ShowMinorGridChanged is not null)
        ShowMinorGridChanged(true == _chkShowMinor.IsChecked);
    }

    #region IXYGridStyleView

    public event Action<bool>? ShowGridChanged;

    public event Action<bool>? ShowMinorGridChanged;

    public event Action<bool>? ShowZeroOnlyChanged;

    public void InitializeBegin()
    {
    }

    public void InitializeEnd()
    {
    }

    public void InitializeMajorGridStyle(IMVCANDController controller)
    {
      controller.ViewObject = _majorStyle;
    }

    public void InitializeMinorGridStyle(IMVCANDController controller)
    {
      controller.ViewObject = _minorStyle;
    }

    public void InitializeShowGrid(bool value)
    {
      _chkEnable.IsChecked = value;
    }

    public void InitializeShowMinorGrid(bool value)
    {
      _chkShowMinor.IsChecked = value;
    }

    public void InitializeShowZeroOnly(bool value)
    {
      _chkShowZeroOnly.IsChecked = value;
    }

    public void InitializeElementEnabling(bool majorstyle, bool minorstyle, bool showminor, bool showzeroonly)
    {
      _majorStyle.IsEnabled = majorstyle;
      _minorStyle.IsEnabled = minorstyle;
      _chkShowMinor.IsEnabled = showminor;
      _chkShowZeroOnly.IsEnabled = showzeroonly;
    }

    #endregion IXYGridStyleView
  }
}
