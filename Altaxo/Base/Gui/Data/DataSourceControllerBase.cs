#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2022 Dr. Dirk Lellinger
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
using Altaxo.Data;

namespace Altaxo.Gui.Data
{
  /// <summary>
  /// Base controller for configuring a table data source, including import options, process options, and process data.
  /// </summary>
  /// <typeparam name="TItem">The type of the data source being edited.</typeparam>
  [ExpectedTypeOfView(typeof(ICommonDataSourceViewN))]
  public class DataSourceControllerBase<TItem> : MVCANControllerEditOriginalDocBase<TItem, IDataContextAwareView>, IMVCSupportsApplyCallback where TItem : IAltaxoTableDataSource
  {
    private IMVCANController _inputOptionsController;
    private IMVCANController _processOptionsController;
    private IMVCANController _processDataController;

    /// <summary>
    /// Occurs when the apply operation has completed successfully.
    /// </summary>
    public event Action SuccessfullyApplied;

    /// <summary>
    /// Name of the tab that shows the input options.
    /// </summary>
    public const string TabInputOptions = "InputOptions";

    /// <summary>
    /// Name of the tab that shows the process options.
    /// </summary>
    public const string TabProcessOptions = "ProcessOptions";

    /// <summary>
    /// Name of the tab that shows the process data.
    /// </summary>
    public const string TabProcessData = "ProcessData";

    /// <inheritdoc/>
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield return new ControllerAndSetNullMethod(_inputOptionsController, () => _inputOptionsController = null);
      yield return new ControllerAndSetNullMethod(_processOptionsController, () => _processOptionsController = null);
      yield return new ControllerAndSetNullMethod(_processDataController, () => _processDataController = null);
    }

    #region Bindings


    /// <summary>
    /// Gets or sets the controller that manages the input options of the data source.
    /// </summary>
    public IMVCANController InputOptionsController
    {
      get { return _inputOptionsController; }
      set
      {
        if (!object.ReferenceEquals(value, _inputOptionsController))
        {
          _inputOptionsController?.Dispose();
          _inputOptionsController = value;
          OnPropertyChanged(nameof(InputOptionsController));
        }
      }
    }

    /// <summary>
    /// Gets or sets the controller that manages the process options of the data source.
    /// </summary>
    public IMVCANController ProcessOptionsController
    {
      get { return _processOptionsController; }
      set
      {
        if (!object.ReferenceEquals(value, _processOptionsController))
        {
          _processOptionsController?.Dispose();
          _processOptionsController = value;
          OnPropertyChanged(nameof(ProcessOptionsController));
          OnPropertyChanged(nameof(ControllerAtPosition2));
          OnPropertyChanged(nameof(ControllerAtPosition3));
        }
      }
    }

    /// <summary>
    /// Gets or sets the controller that manages the process data of the data source.
    /// </summary>
    public IMVCANController ProcessDataController
    {
      get { return _processDataController; }
      set
      {
        if (!object.ReferenceEquals(value, _processDataController))
        {
          _processDataController?.Dispose();
          _processDataController = value;
          OnPropertyChanged(nameof(ProcessDataController));
          OnPropertyChanged(nameof(ControllerAtPosition2));
          OnPropertyChanged(nameof(ControllerAtPosition3));
        }
      }
    }

    /// <summary>
    /// Gets the controller that should be shown in the second position (middle panel).
    /// </summary>
    public IMVCANController ControllerAtPosition2
    {
      get
      {
        return ShowProcessDataBeforeProcessOptions ? ProcessDataController : ProcessOptionsController;
      }
    }

    private bool _isPosition2Expanded;

    /// <summary>
    /// Gets or sets a value indicating whether the second panel (position 2) is expanded.
    /// </summary>
    public bool IsPosition2Expanded
    {
      get => _isPosition2Expanded;
      set
      {
        if (!(_isPosition2Expanded == value))
        {
          _isPosition2Expanded = value;
          OnPropertyChanged(nameof(IsPosition2Expanded));
        }
      }
    }


    /// <summary>
    /// Gets the controller that should be shown in the third position (bottom panel).
    /// </summary>
    public IMVCANController ControllerAtPosition3
    {
      get
      {
        return ShowProcessDataBeforeProcessOptions ?  ProcessOptionsController : ProcessDataController;
      }
    }

    private bool _isPosition3Expanded;

    /// <summary>
    /// Gets or sets a value indicating whether the third panel (position 3) is expanded.
    /// </summary>
    public bool IsPosition3Expanded
    {
      get => _isPosition3Expanded;
      set
      {
        if (!(_isPosition3Expanded == value))
        {
          _isPosition3Expanded = value;
          OnPropertyChanged(nameof(IsPosition3Expanded));
        }
      }
    }

    /// <summary>
    /// If <see langword="true"/>, the process data view is shown before (on top of) the process options view. If <see langword="false"/> (default), the process options view is shown on top of the process data view.
    /// </summary>
    public virtual bool ShowProcessDataBeforeProcessOptions
    {
      get => false;
    }


    #endregion

    /// <inheritdoc/>
    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        InputOptionsController = (IMVCANController)Current.Gui.GetControllerAndControl(new object[] { _doc.ImportOptions }, typeof(IMVCANController), UseDocument.Directly);
        ProcessOptionsController = GetProcessOptionsController();
        ProcessDataController = GetProcessDataController();

        if (ShowProcessDataBeforeProcessOptions)
        {
          IsPosition2Expanded = IsProcessDataInitiallyExpanded();
          IsPosition3Expanded = IsProcessOptionsInitiallyExpanded();
        }
        else
        {
          IsPosition2Expanded = IsProcessOptionsInitiallyExpanded();
          IsPosition3Expanded = IsProcessDataInitiallyExpanded();
        }
      }
    }

    /// <summary>
    /// Creates the controller that manages the process options for the data source.
    /// </summary>
    /// <returns>The process options controller, or <c>null</c> if there is no process options object.</returns>
    protected virtual IMVCANController GetProcessOptionsController()
    {
      if (_doc.ProcessOptionsObject is not null)
      {
        return (IMVCANController)Current.Gui.GetControllerAndControl(new object[] { _doc.ProcessOptionsObject }, typeof(IMVCANController), UseDocument.Directly);
      }
      else
      {
        return null;
      }
    }


    /// <summary>
    /// Creates the controller that manages the process data for the data source.
    /// </summary>
    /// <returns>The process data controller, or <c>null</c> if there is no process data object.</returns>
    protected virtual IMVCANController GetProcessDataController()
    {
      if (_doc.ProcessDataObject is not null)
      {
        return (IMVCANController)Current.Gui.GetControllerAndControl(new object[] { _doc.ProcessDataObject }, typeof(IMVCANController), UseDocument.Directly);
      }
      else
      {
        return null;
      }
    }


    /// <summary>
    /// Determines whether the process options view is initially expanded.
    /// </summary>
    /// <returns>
    ///   <c>true</c> if the process options view is initially expanded; otherwise, <c>false</c>.
    /// </returns>
    protected virtual bool IsProcessOptionsInitiallyExpanded() => true;


    /// <summary>
    /// Determines whether the process data view is initially expanded.
    /// </summary>
    /// <returns>
    ///   <c>true</c> if the process data view is initially expanded; otherwise, <c>false</c>.
    /// </returns>
    protected virtual bool IsProcessDataInitiallyExpanded() => false;


    /// <inheritdoc/>
    public override bool Apply(bool disposeController)
    {

      if (_inputOptionsController.Apply(disposeController))
      {
        _doc.ImportOptions = (IDataSourceImportOptions)_inputOptionsController.ModelObject;
      }
      else
      {
        return ApplyEnd(false, disposeController);
      }


      if (_processOptionsController.Apply(disposeController))
      {
        _doc.ProcessOptionsObject = _processOptionsController.ModelObject;
      }
      else
      {
        return ApplyEnd(false, disposeController);
      }

      if (_processDataController is not null)
      {
        if (_processDataController.Apply(disposeController))
        {
          _doc.ProcessDataObject = _processDataController.ModelObject;
        }
        else
        {
          return ApplyEnd(false, disposeController);
        }
      }


      SuccessfullyApplied?.Invoke();


      return ApplyEnd(true, disposeController);
    }
  }
}
