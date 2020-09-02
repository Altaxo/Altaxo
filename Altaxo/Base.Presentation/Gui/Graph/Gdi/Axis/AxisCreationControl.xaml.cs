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
using Altaxo.Collections;

namespace Altaxo.Gui.Graph.Gdi.Axis
{
  /// <summary>
  /// Interaction logic for AxisCreationControl.xaml
  /// </summary>
  public partial class AxisCreationControl : UserControl, IAxisCreationView
  {
    public event Action SelectedAxisTemplateChanged;

    public AxisCreationControl()
    {
      InitializeComponent();
    }

    public bool UsePhysicalValue
    {
      get
      {
        return _guiUsePhysicalValue.IsChecked == true;
      }
      set
      {
        _guiUsePhysicalValue.IsChecked = value;
        _guiUseLogicalValue.IsChecked = !value;
      }
    }

    public double AxisPositionLogicalValue
    {
      get
      {
        return _guiLogicalValue.SelectedQuantityAsValueInSIUnits;
      }
      set
      {
        _guiLogicalValue.SelectedQuantityAsValueInSIUnits = value;
      }
    }

    public Altaxo.Data.AltaxoVariant AxisPositionPhysicalValue
    {
      get
      {
        return _guiPhysicalValue.SelectedValue;
      }
      set
      {
        _guiPhysicalValue.SelectedValue = value;
      }
    }

    public bool MoveAxis
    {
      get
      {
        return _guiMoveAxis.IsChecked == true;
      }
      set
      {
        _guiMoveAxis.IsChecked = value;
        _guiCopyAxis.IsChecked = !value;
      }
    }

    public void InitializeAxisTemplates(SelectableListNodeList list)
    {
      GuiHelper.Initialize(_guiTemplateAxis, list);
    }

    private void EhSelectedAxisTemplateChanged(object sender, SelectionChangedEventArgs e)
    {
      GuiHelper.SynchronizeSelectionFromGui(_guiTemplateAxis);
      if (SelectedAxisTemplateChanged is not null)
        SelectedAxisTemplateChanged();
    }
  }
}
