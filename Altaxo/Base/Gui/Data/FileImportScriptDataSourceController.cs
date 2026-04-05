#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2019 Dr. Dirk Lellinger
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

#nullable disable
using System;
using System.Collections.Generic;
using Altaxo.Data;
using Altaxo.Gui.Common;
using Altaxo.Scripting;

namespace Altaxo.Gui.Data
{
  /// <summary>
  /// Controller for <see cref="FileImportScriptDataSource"/>.
  /// </summary>
  [ExpectedTypeOfView(typeof(IImportDataSourceView))]
  [UserControllerForObject(typeof(FileImportScriptDataSource))]
  public class FileImportScriptDataSourceController : MVCANControllerEditOriginalDocBase<FileImportScriptDataSource, IImportDataSourceView>, IMVCSupportsApplyCallback
  {
    /// <inheritdoc/>
    public event Action SuccessfullyApplied;

    /// <inheritdoc />
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield return new ControllerAndSetNullMethod(_commonImportOptionsController, () => _commonImportOptionsController = null);
      yield return new ControllerAndSetNullMethod(_specificImportOptionsController, () => _specificImportOptionsController = null);
      yield return new ControllerAndSetNullMethod(_specificImportSourceController, () => _specificImportSourceController = null);
    }

    #region Bindings

    private string _commonImportOptionsControlHeader;

    /// <summary>
    /// Gets or sets the header for the common import options control.
    /// </summary>
    public string CommonImportOptionsControlHeader
    {
      get => _commonImportOptionsControlHeader;
      set
      {
        if (!(_commonImportOptionsControlHeader == value))
        {
          _commonImportOptionsControlHeader = value;
          OnPropertyChanged(nameof(CommonImportOptionsControlHeader));
        }
      }
    }


    private IMVCANController _commonImportOptionsController;

    /// <summary>
    /// Gets or sets the controller for the common import options.
    /// </summary>
    public IMVCANController CommonImportOptionsController
    {
      get => _commonImportOptionsController;
      set
      {
        if (!(_commonImportOptionsController == value))
        {
          _commonImportOptionsController?.Dispose();
          _commonImportOptionsController = value;
          OnPropertyChanged(nameof(CommonImportOptionsController));
        }
      }
    }

    private string _specificImportOptionsControlHeader;

    /// <summary>
    /// Gets or sets the header for the specific import options control.
    /// </summary>
    public string SpecificImportOptionsControlHeader
    {
      get => _specificImportOptionsControlHeader;
      set
      {
        if (!(_specificImportOptionsControlHeader == value))
        {
          _specificImportOptionsControlHeader = value;
          OnPropertyChanged(nameof(SpecificImportOptionsControlHeader));
        }
      }
    }


    private IMVCANController _specificImportOptionsController;

    /// <summary>
    /// Gets or sets the controller for the specific import options.
    /// </summary>
    public IMVCANController SpecificImportOptionsController
    {
      get => _specificImportOptionsController;
      set
      {
        if (!(_specificImportOptionsController == value))
        {
          _specificImportOptionsController?.Dispose();
          _specificImportOptionsController = value;
          OnPropertyChanged(nameof(SpecificImportOptionsController));
        }
      }
    }

    private string _specificImportSourceControlHeader;

    /// <summary>
    /// Gets or sets the header for the specific import source control.
    /// </summary>
    public string SpecificImportSourceControlHeader
    {
      get => _specificImportSourceControlHeader;
      set
      {
        if (!(_specificImportSourceControlHeader == value))
        {
          _specificImportSourceControlHeader = value;
          OnPropertyChanged(nameof(SpecificImportSourceControlHeader));
        }
      }
    }


    private MultipleFilesController _specificImportSourceController;

    /// <summary>
    /// Gets or sets the controller for the import source files.
    /// </summary>
    public MultipleFilesController SpecificImportSourceController
    {
      get => _specificImportSourceController;
      set
      {
        if (!(_specificImportSourceController == value))
        {
          _specificImportSourceController?.Dispose();
          _specificImportSourceController = value;
          OnPropertyChanged(nameof(SpecificImportSourceController));
        }
      }
    }




    #endregion

    /// <inheritdoc />
    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        CommonImportOptionsControlHeader = "Common import options";
        SpecificImportOptionsControlHeader = "Import script";
        SpecificImportSourceControlHeader = "File(s) to import";

        CommonImportOptionsController = (IMVCANController)Current.Gui.GetControllerAndControl(new object[] { _doc.ImportOptions }, typeof(IMVCANController), UseDocument.Directly);
        SpecificImportOptionsController = (IMVCANController)Current.Gui.GetControllerAndControl(new object[] { _doc.ImportScript }, typeof(IMVCANController), UseDocument.Directly);
        var specificImportSourceController = new MultipleFilesController();
        specificImportSourceController.InitializeDocument(_doc.SourceFileNames);
        Current.Gui.FindAndAttachControlTo(specificImportSourceController);
        SpecificImportSourceController = specificImportSourceController;
      }
    }

    /// <inheritdoc />
    public override bool Apply(bool disposeController)
    {
      bool result;

      result = _commonImportOptionsController.Apply(false);
      if (!result)
        return result;
      else
        _doc.ImportOptions = (Altaxo.Data.IDataSourceImportOptions)_commonImportOptionsController.ModelObject;

      result = _specificImportOptionsController.Apply(false);
      if (!result)
      {
        Current.Gui.ErrorMessageBox("Error in script. Please edit the script to remove the error");
        return result;
      }
      else
        _doc.ImportScript = (FileImportScript)_specificImportOptionsController.ModelObject; // AsciiImportOptions is cloned in property set

      result = _specificImportSourceController.Apply(false);
      if (!result)
        return result;
      else
        _doc.SourceFileNames = (IEnumerable<string>)_specificImportSourceController.ModelObject; // AsciiImportOptions is cloned in property set

      SuccessfullyApplied?.Invoke();

      return ApplyEnd(true, disposeController);
    }
  }
}
