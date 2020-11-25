#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2015 Dr. Dirk Lellinger
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

namespace Altaxo.Gui.Graph.Scales.Rescaling
{
  /// <summary>
  /// Interaction logic for LinearScaleRescaleConditionsControl.xaml
  /// </summary>
  public partial class LogarithmicScaleRescaleConditionsControl : UserControl, ILogarithmicScaleRescaleConditionsView
  {
    public event Action? OrgValueChanged;

    public event Action? EndValueChanged;

    public LogarithmicScaleRescaleConditionsControl()
    {
      InitializeComponent();
    }

    private void _guiOrgValue_SelectedValueChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      var ev = OrgValueChanged;
      if (ev is not null)
        ev();
    }

    private void _guiEndValue_SelectedValueChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      var ev = EndValueChanged;
      if (ev is not null)
        ev();
    }

    public Collections.SelectableListNodeList OrgRescaling
    {
      set { GuiHelper.Initialize(_guiOrgRescaling, value); }
    }

    public Collections.SelectableListNodeList EndRescaling
    {
      set { GuiHelper.Initialize(_guiEndRescaling, value); }
    }

    public Collections.SelectableListNodeList OrgRelativeTo
    {
      set { GuiHelper.Initialize(_guiOrgRelativeTo, value); }
    }

    public Collections.SelectableListNodeList EndRelativeTo
    {
      set { GuiHelper.Initialize(_guiEndRelativeTo, value); }
    }

    public double OrgValue
    {
      get
      {
        return _guiOrgValue.SelectedValue;
      }
      set
      {
        _guiOrgValue.SelectedValue = value;
      }
    }

    public double EndValue
    {
      get
      {
        return _guiEndValue.SelectedValue;
      }
      set
      {
        _guiEndValue.SelectedValue = value;
      }
    }

    private void EhComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      GuiHelper.SynchronizeSelectionFromGui((ComboBox)sender);
    }
  }
}
