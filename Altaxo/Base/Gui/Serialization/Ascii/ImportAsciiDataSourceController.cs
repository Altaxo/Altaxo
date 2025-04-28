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

#nullable disable
using System;
using System.Collections.Generic;
using Altaxo.Gui.Common;
using Altaxo.Gui.Data;
using Altaxo.Serialization.Ascii;

namespace Altaxo.Gui.Serialization.Ascii
{
  [ExpectedTypeOfView(typeof(IImportDataSourceView))]
  [UserControllerForObject(typeof(AsciiImportDataSource))]
  public class AsciiImportDataSourceController : MVCANControllerEditOriginalDocBase<AsciiImportDataSource, IImportDataSourceView>, IMVCSupportsApplyCallback
  {
    public event Action SuccessfullyApplied;

    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield return new ControllerAndSetNullMethod(_commonImportOptionsController, () => _commonImportOptionsController = null);
      yield return new ControllerAndSetNullMethod(_specificImportOptionsController, () => _specificImportOptionsController = null);
      yield return new ControllerAndSetNullMethod(_specificImportSourceController, () => _specificImportSourceController = null);
    }

    #region Bindings

    private string _commonImportOptionsControlHeader;

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


    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        CommonImportOptionsControlHeader = "Common import options";
        SpecificImportOptionsControlHeader = "Ascii import options";
        SpecificImportSourceControlHeader = "File(s) to import";

        CommonImportOptionsController = (IMVCANController)Current.Gui.GetControllerAndControl(new object[] { _doc.ImportOptions }, typeof(IMVCANController), UseDocument.Directly);
        SpecificImportOptionsController = (IMVCANController)Current.Gui.GetControllerAndControl(new object[] { _doc.ProcessOptions, new AsciiImportOptionsAnalysisDataProvider(this) }, typeof(IMVCANController), UseDocument.Directly);
        var specificImportSourceController = new MultipleFilesController();
        specificImportSourceController.InitializeDocument(_doc.SourceFileNames);
        Current.Gui.FindAndAttachControlTo(specificImportSourceController);

        SpecificImportSourceController = specificImportSourceController;
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
        return result;
      else
        _doc.ProcessOptions = (AsciiImportOptions)_specificImportOptionsController.ModelObject; // AsciiImportOptions is cloned in property set

      result = _specificImportSourceController.Apply(false);
      if (!result)
        return result;
      else
        _doc.SourceFileNames = (IEnumerable<string>)_specificImportSourceController.ModelObject; // AsciiImportOptions is cloned in property set

      SuccessfullyApplied?.Invoke();
      return ApplyEnd(true, disposeController);
    }



    private class AsciiImportOptionsAnalysisDataProvider : IAsciiImportOptionsAnalysisDataProvider
    {
      private AsciiImportDataSourceController _parent;

      internal AsciiImportOptionsAnalysisDataProvider(AsciiImportDataSourceController parent)
      {
        _parent = parent;
      }

      public System.IO.Stream GetStreamForAnalysis()
      {
        try
        {
          var str = AsciiImporter.GetAsciiInputFileStream(_parent._doc.SourceFileName);
          return str;
        }
        catch (Exception)
        {
        }
        return null;
      }
    }

    /// <summary>
    /// Gets a file stream of the first file for analysis purposes. Returns null without throwing an exception if the file is not available or could not be opened.
    /// </summary>
    /// <returns></returns>
    private System.IO.Stream GetFileStreamForAnalysis()
    {
      try
      {
        var str = AsciiImporter.GetAsciiInputFileStream(_doc.SourceFileName);
        return str;
      }
      catch (Exception)
      {
      }
      return null;
    }
  }
}
