#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2014 Dr. Dirk Lellinger
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

namespace Altaxo.Gui.Data
{
  /// <summary>
  /// Interaction logic for DataSourceImportOptionsControl.xaml
  /// </summary>
  public partial class DataSourceImportOptionsControl : UserControl, IDataSourceImportOptionsView
  {
    public DataSourceImportOptionsControl()
    {
      InitializeComponent();
    }

    public bool DoNotSaveTableData
    {
      get
      {
        return _guiDoNotSaveTableData.IsChecked == true;
      }
      set
      {
        _guiDoNotSaveTableData.IsChecked = value;
      }
    }

    public bool ExecuteScriptAfterImport
    {
      get
      {
        return _guiExecuteWksScriptAfterImport.IsChecked == true;
      }
      set
      {
        _guiExecuteWksScriptAfterImport.IsChecked = value;
      }
    }

    public void InitializeTriggerSource(Altaxo.Collections.SelectableListNodeList list)
    {
      _guiTrigger.Initialize(list);
    }

    public double MinimumWaitingTimeAfterUpdateInSeconds
    {
      get
      {
        return _guiMinTimeAfterUpdate.SelectedQuantityAsValueInSIUnits;
      }
      set
      {
        _guiMinTimeAfterUpdate.SelectedQuantityAsValueInSIUnits = value;
      }
    }

    public double MaximumWaitingTimeAfterUpdateInSeconds
    {
      get
      {
        return _guiMaxTimeAfterUpdate.SelectedQuantityAsValueInSIUnits;
      }
      set
      {
        _guiMaxTimeAfterUpdate.SelectedQuantityAsValueInSIUnits = value;
      }
    }

    public double MinimumWaitingTimeAfterFirstTriggerInSeconds
    {
      get
      {
        return _guiMinimumTimeAfterFirstTrigger.SelectedQuantityAsValueInSIUnits;
      }
      set
      {
        _guiMinimumTimeAfterFirstTrigger.SelectedQuantityAsValueInSIUnits = value;
      }
    }

    public double MaximumWaitingTimeAfterFirstTriggerInSeconds
    {
      get
      {
        return _guiMaximumTimeAfterFirstTrigger.SelectedQuantityAsValueInSIUnits;
      }
      set
      {
        _guiMaximumTimeAfterFirstTrigger.SelectedQuantityAsValueInSIUnits = value;
      }
    }

    public double MinimumWaitingTimeAfterLastTriggerInSeconds
    {
      get
      {
        return _guiMinimumTimeAfterLastTrigger.SelectedQuantityAsValueInSIUnits;
      }
      set
      {
        _guiMinimumTimeAfterLastTrigger.SelectedQuantityAsValueInSIUnits = value;
      }
    }
  }
}
