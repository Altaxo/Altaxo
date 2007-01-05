#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2007 Dr. Dirk Lellinger
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
#endregion

using System;
using Altaxo.Gui;

namespace Altaxo.Worksheet.GUI
{
  /// <summary>
  /// Summary description for PLSPredictValueController.
  /// </summary>
  public class PLSPredictValueController : IApplyController

  {
    PLSPredictValueControl _view;
    string[] _calibrationTables;
    string[] _destinationTables;

    public string SelectedDestinationTableName;
    public string SelectedCalibrationTableName;

    void SetElements(bool bInit)
    {
      _calibrationTables = Altaxo.Worksheet.Commands.Analysis.ChemometricCommands.GetAvailablePLSCalibrationTables();
      _destinationTables = GetAvailableDestinationTables();

      if(null!=_view)
      {
        _view.InitializeCalibrationModelTables(_calibrationTables);
        _view.InitializeDestinationTables(_destinationTables);
      }
    }


    string[] GetAvailableDestinationTables()
    {
      System.Collections.ArrayList result = new System.Collections.ArrayList();
      result.Add("New table");
      foreach(Altaxo.Data.DataTable table in Current.Project.DataTableCollection)
        result.Add(table.Name);

      return (string[])result.ToArray(typeof(string));
    }
   
    public PLSPredictValueControl View
    {
      get { return _view; }
      set
      {
        if(null!=_view)
          _view.Controller = null;
        
        _view = value;

        if(null!=_view)
        {
          _view.Controller = this;
          SetElements(false); // set only the view elements, dont't initialize the variables
        }
      }
    }

    #region IApplyController Members

    public bool Apply()
    {
      int sel;
      sel = _view.GetCalibrationTableChoice();
      if(sel<0)
        this.SelectedCalibrationTableName = null;
      else
        this.SelectedCalibrationTableName = this._calibrationTables[sel];

      sel = _view.GetDestinationTableChoice();
      if(sel==0)
        this.SelectedDestinationTableName = null;
      else
        this.SelectedDestinationTableName = this._destinationTables[sel];


      return true;
    }

    #endregion


  }
}
