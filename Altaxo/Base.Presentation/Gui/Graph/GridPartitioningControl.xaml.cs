﻿#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2014 Dr. Dirk Lellinger
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

namespace Altaxo.Gui.Graph
{
  /// <summary>
  /// Interaction logic for GridPartitioningControl.xaml
  /// </summary>
  public partial class GridPartitioningControl : UserControl, IGridPartitioningView
  {
    public GridPartitioningControl()
    {
      InitializeComponent();
    }

    public QuantityWithUnitGuiEnvironment XPartitionEnvironment { set { _guiColumnDefinitions.Environment = value; } }

    public QuantityWithUnitGuiEnvironment YPartitionEnvironment { set { _guiRowDefinitions.Environment = value; } }

    public Altaxo.Units.DimensionfulQuantity DefaultXQuantity { set { _guiColumnDefinitions.DefaultQuantity = value; } }

    public Altaxo.Units.DimensionfulQuantity DefaultYQuantity { set { _guiRowDefinitions.DefaultQuantity = value; } }

    public System.Collections.ObjectModel.ObservableCollection<Altaxo.Units.DimensionfulQuantity> ColumnCollection
    {
      set
      {
        _guiColumnDefinitions.ItemsSource = value;
      }
    }

    public System.Collections.ObjectModel.ObservableCollection<Altaxo.Units.DimensionfulQuantity> RowCollection
    {
      set
      {
        _guiRowDefinitions.ItemsSource = value;
      }
    }
  }
}
