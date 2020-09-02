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

namespace Altaxo.Gui.Main.Services
{
  /// <summary>
  /// Interaction logic for OutputView.xaml
  /// </summary>
  public partial class OutputView : UserControl, IOutputView
  {
    public event Action EnabledChanged;

    public OutputView()
    {
      InitializeComponent();
    }

    #region IOutputView Members

    private void InternalSetText(string text)
    {
      _view.Text = text;
    }

    public void SetText(string text)
    {
      if (Dispatcher.CheckAccess())
        _view.Text = text;
      else
        Dispatcher.BeginInvoke((Action)delegate ()
        { InternalSetText(text); }, System.Windows.Threading.DispatcherPriority.Normal, null);
    }

    bool IOutputView.IsEnabled { get { return _menuTextAppendEnabled.IsChecked; } set { _menuTextAppendEnabled.IsChecked = value; } }

    #endregion IOutputView Members

    private void EhMenuTextAppendEnabled1(object sender, RoutedEventArgs e)
    {
      if (EnabledChanged is not null)
        EnabledChanged();
    }
  }
}
