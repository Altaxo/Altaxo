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

namespace Altaxo.Gui.Worksheet
{
  #region Interfaces

  public interface IPLSPredictValueView
  {
    void InitializeCalibrationModelTables(string[] tables);

    void InitializeDestinationTables(string[] tables);

    int GetCalibrationTableChoice();

    int GetDestinationTableChoice();
  }

  #endregion Interfaces

  /// <summary>
  /// Summary description for PLSPredictValueController.
  /// </summary>
  [ExpectedTypeOfView(typeof(IPLSPredictValueView))]
  public class PLSPredictValueController : IMVCAController
  {
    private IPLSPredictValueView _view;
    private string[] _calibrationTables;
    private string[] _destinationTables;

    public string SelectedDestinationTableName;
    public string SelectedCalibrationTableName;

    private void SetElements(bool bInit)
    {
      _calibrationTables = Altaxo.Worksheet.Commands.Analysis.ChemometricCommands.GetAvailablePLSCalibrationTables();
      _destinationTables = GetAvailableDestinationTables();

      if (null != _view)
      {
        _view.InitializeCalibrationModelTables(_calibrationTables);
        _view.InitializeDestinationTables(_destinationTables);
      }
    }

    private string[] GetAvailableDestinationTables()
    {
      System.Collections.ArrayList result = new System.Collections.ArrayList();
      result.Add("New table");
      foreach (Altaxo.Data.DataTable table in Current.Project.DataTableCollection)
        result.Add(table.Name);

      return (string[])result.ToArray(typeof(string));
    }

    public IPLSPredictValueView View
    {
      get { return _view; }
      set
      {
        _view = value;

        if (null != _view)
        {
          SetElements(false); // set only the view elements, dont't initialize the variables
        }
      }
    }

    #region IApplyController Members

    public bool Apply(bool disposeController)
    {
      int sel;
      sel = _view.GetCalibrationTableChoice();
      if (sel < 0)
        this.SelectedCalibrationTableName = null;
      else
        this.SelectedCalibrationTableName = this._calibrationTables[sel];

      sel = _view.GetDestinationTableChoice();
      if (sel == 0)
        this.SelectedDestinationTableName = null;
      else
        this.SelectedDestinationTableName = this._destinationTables[sel];

      return true;
    }

    /// <summary>
    /// Try to revert changes to the model, i.e. restores the original state of the model.
    /// </summary>
    /// <param name="disposeController">If set to <c>true</c>, the controller should release all temporary resources, since the controller is not needed anymore.</param>
    /// <returns>
    ///   <c>True</c> if the revert operation was successfull; <c>false</c> if the revert operation was not possible (i.e. because the controller has not stored the original state of the model).
    /// </returns>
    public bool Revert(bool disposeController)
    {
      return false;
    }

    #endregion IApplyController Members

    #region IMVCController Members

    public object ViewObject
    {
      get
      {
        return _view;
      }
      set
      {
        this.View = value as IPLSPredictValueView;
      }
    }

    public object ModelObject
    {
      get { return null; }
    }

    public void Dispose()
    {
    }

    #endregion IMVCController Members
  }
}
