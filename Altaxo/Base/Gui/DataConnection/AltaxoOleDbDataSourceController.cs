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
using System.Linq;
using System.Text;
using Altaxo.DataConnection;

namespace Altaxo.Gui.DataConnection
{
  public interface IAltaxoOleDbDataSourceView
  {
    void SetQueryView(object viewObject);

    void SetImportOptionsView(object viewObject);
  }

  [ExpectedTypeOfView(typeof(IAltaxoOleDbDataSourceView))]
  [UserControllerForObject(typeof(AltaxoOleDbDataSource))]
  public class AltaxoOleDbDataSourceController : MVCANControllerEditOriginalDocBase<AltaxoOleDbDataSource, IAltaxoOleDbDataSourceView>
  {
    private OleDbDataQueryController _connectionMainController;
    private Altaxo.Gui.Data.DataSourceImportOptionsController _importOptionsController;

    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield return new ControllerAndSetNullMethod(_connectionMainController, () => _connectionMainController = null);
      yield return new ControllerAndSetNullMethod(_importOptionsController, () => _importOptionsController = null);
    }

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        _importOptionsController = new Data.DataSourceImportOptionsController() { UseDocumentCopy = UseDocument.Directly };
        _importOptionsController.InitializeDocument(_doc.ImportOptions);

        _connectionMainController = new OleDbDataQueryController() { UseDocumentCopy = UseDocument.Directly };
        _connectionMainController.InitializeDocument(_doc.DataQuery);
      }

      if (_view is not null)
      {
        if (_importOptionsController.ViewObject is null)
          Current.Gui.FindAndAttachControlTo(_importOptionsController);

        if (_connectionMainController.ViewObject is null)
          Current.Gui.FindAndAttachControlTo(_connectionMainController);

        _view.SetImportOptionsView(_importOptionsController.ViewObject);
        _view.SetQueryView(_connectionMainController.ViewObject);
      }
    }

    public override bool Apply(bool disposeController)
    {
      if (!_importOptionsController.Apply(disposeController))
        return false;
      if (!_connectionMainController.Apply(disposeController))
        return false;

      _doc.ImportOptions = (Altaxo.Data.DataSourceImportOptions)_importOptionsController.ModelObject;
      _doc.DataQuery = (OleDbDataQuery)_connectionMainController.ModelObject;

      return ApplyEnd(true, disposeController);
    }
  }
}
