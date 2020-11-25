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
using System.Linq;
using System.Text;
using Altaxo.Collections;
using Altaxo.Data;
using Altaxo.Gui.Common;
using Altaxo.Scripting;
using Altaxo.Serialization.Ascii;

namespace Altaxo.Gui.Data
{
  [ExpectedTypeOfView(typeof(IImportDataSourceView))]
  [UserControllerForObject(typeof(FileImportScriptDataSource))]
  public class FileImportScriptDataSourceController : MVCANControllerEditOriginalDocBase<FileImportScriptDataSource, IImportDataSourceView>, IMVCSupportsApplyCallback
  {
    private IMVCANController _commonImportOptionsController;
    private IMVCANController _specificImportOptionsController;
    private MultipleFilesController _specificImportSourceController;

    public event Action SuccessfullyApplied;

    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield return new ControllerAndSetNullMethod(_commonImportOptionsController, () => _commonImportOptionsController = null);
      yield return new ControllerAndSetNullMethod(_specificImportOptionsController, () => _specificImportOptionsController = null);
      yield return new ControllerAndSetNullMethod(_specificImportSourceController, () => _specificImportSourceController = null);
    }

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        //_doc.SourceFileName

        _commonImportOptionsController = (IMVCANController)Current.Gui.GetControllerAndControl(new object[] { _doc.ImportOptions }, typeof(IMVCANController), UseDocument.Directly);
        _specificImportOptionsController = (IMVCANController)Current.Gui.GetControllerAndControl(new object[] { _doc.ImportScript }, typeof(IMVCANController), UseDocument.Directly);
        _specificImportSourceController = new MultipleFilesController();
        _specificImportSourceController.InitializeDocument(_doc.SourceFileNames);
        Current.Gui.FindAndAttachControlTo(_specificImportSourceController);

      }

      if (_view is not null)
      {
      }
    }

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

      if (disposeController)
      {
        _commonImportOptionsController.Dispose();
        _specificImportOptionsController.Dispose();
        _specificImportSourceController.Dispose();
      }

      SuccessfullyApplied?.Invoke();

      return ApplyEnd(true, disposeController);
    }

    protected override void AttachView()
    {
      base.AttachView();
      _view.SetCommonImportOptionsControl("Common import options", _commonImportOptionsController.ViewObject);
      _view.SetSpecificImportOptionsControl("Import script", _specificImportOptionsController.ViewObject);
      _view.SetSpecificImportSourceControl("File(s) to import", _specificImportSourceController.ViewObject);
    }

    protected override void DetachView()
    {
      _view.SetCommonImportOptionsControl(string.Empty, null);
      _view.SetSpecificImportOptionsControl(string.Empty, null);
      _view.SetSpecificImportSourceControl(string.Empty, null);

      base.DetachView();
    }



  }
}
