#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2024 Dr. Dirk Lellinger
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

using System.Collections.Generic;


using Altaxo.Serialization;

namespace Altaxo.Gui.Serialization
{
  public interface IImportOptionsInitialView : IDataContextAwareView { }

  [ExpectedTypeOfView(typeof(IImportOptionsInitialView))]
  [UserControllerForObject(typeof(ImportOptionsInitial))]
  public class ImportOptionsInitialController : MVCANControllerEditImmutableDocBase<ImportOptionsInitial, IImportOptionsInitialView>
  {
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield return new ControllerAndSetNullMethod(OptionsController, () => OptionsController = null);
    }

    #region Bindings


    private IMVCANController _optionsController;

    public IMVCANController OptionsController
    {
      get => _optionsController;
      set
      {
        if (!(_optionsController == value))
        {
          _optionsController?.Dispose();
          _optionsController = value;
          OnPropertyChanged(nameof(OptionsController));
        }
      }
    }


    private bool _distributeFilesToSeparateTables;

    public bool DistributeFilesToSeparateTables
    {
      get => _distributeFilesToSeparateTables;
      set
      {
        if (!(_distributeFilesToSeparateTables == value))
        {
          _distributeFilesToSeparateTables = value;
          OnPropertyChanged(nameof(DistributeFilesToSeparateTables));
        }
      }
    }

    private bool _distributeDataPerFileToSeparateTables;

    public bool DistributeDataPerFileToSeparateTables
    {
      get => _distributeDataPerFileToSeparateTables;
      set
      {
        if (!(_distributeDataPerFileToSeparateTables == value))
        {
          _distributeDataPerFileToSeparateTables = value;
          OnPropertyChanged(nameof(DistributeDataPerFileToSeparateTables));
        }
      }
    }

    private bool _useMetaDataNameAsTableName;

    public bool UseMetaDataNameAsTableName
    {
      get => _useMetaDataNameAsTableName;
      set
      {
        if (!(_useMetaDataNameAsTableName == value))
        {
          _useMetaDataNameAsTableName = value;
          OnPropertyChanged(nameof(UseMetaDataNameAsTableName));
        }
      }
    }



    #endregion

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        var ctrl = (IMVCANController)Current.Gui.GetControllerAndControl(new object[] { _doc.ImportOptions }, typeof(IMVCANController));
        OptionsController = ctrl;

        DistributeFilesToSeparateTables = _doc.DistributeFilesToSeparateTables;
        DistributeDataPerFileToSeparateTables = _doc.DistributeDataPerFileToSeparateTables;
        UseMetaDataNameAsTableName = _doc.UseMetaDataNameAsTableName;
      }
    }

    public override bool Apply(bool disposeController)
    {
      if (!OptionsController.Apply(disposeController))
      {
        return ApplyEnd(false, disposeController);
      }

      _doc = new ImportOptionsInitial(OptionsController.ModelObject)
      {
        DistributeFilesToSeparateTables = DistributeFilesToSeparateTables,
        DistributeDataPerFileToSeparateTables = DistributeDataPerFileToSeparateTables,
        UseMetaDataNameAsTableName = UseMetaDataNameAsTableName,
      };

      return ApplyEnd(true, disposeController);
    }

  }
}
