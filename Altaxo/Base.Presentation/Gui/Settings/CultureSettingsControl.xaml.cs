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

namespace Altaxo.Gui.Settings
{
  /// <summary>
  /// Interaction logic for CultureSettingsControl.xaml
  /// </summary>
  public partial class CultureSettingsControl : UserControl, ICultureSettingsView
  {
    public event Action CultureChanged;

    public CultureSettingsControl()
    {
      InitializeComponent();
    }

    public void InitializeCultureFormatList(Collections.SelectableListNodeList list)
    {
      GuiHelper.Initialize(_guiCultures, list);
    }

    public string NumberDecimalSeparator
    {
      get
      {
        return _guiNumberDecimalSeparator.Text;
      }
      set
      {
        _guiNumberDecimalSeparator.Text = value;
      }
    }

    public string NumberGroupSeparator
    {
      get
      {
        return _guiNumberGroupSeparator.Text;
      }
      set
      {
        _guiNumberGroupSeparator.Text = value;
      }
    }

    private void EhCultureChanged(object sender, SelectionChangedEventArgs e)
    {
      GuiHelper.SynchronizeSelectionFromGui(_guiCultures);
      if (CultureChanged is not null)
      {
        CultureChanged();
      }
    }
  }
}
