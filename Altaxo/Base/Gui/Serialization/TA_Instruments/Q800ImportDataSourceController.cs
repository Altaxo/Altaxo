﻿#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2025 Dr. Dirk Lellinger
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

using Altaxo.Gui.Common;
using Altaxo.Gui.Data;
using Altaxo.Serialization;
using Altaxo.Serialization.TA_Instruments;

namespace Altaxo.Gui.Serialization.TA_Instruments
{
  [UserControllerForObject(typeof(Q800ImportDataSource))]
  public class Q800ImportDataSourceController : DataSourceControllerBase<Q800ImportDataSource>
  {
    protected override IMVCANController GetProcessDataController()
    {
      var processDataController = new MultipleFilesController();
      processDataController.FileFilters = [FileIOHelper.GetFilterDescriptionForExtensions(new Q800Importer().GetFileExtensions())];
      processDataController.InitializeDocument(_doc.SourceFileNames);
      Current.Gui.FindAndAttachControlTo(processDataController);
      return processDataController;
    }

    protected override bool IsProcessDataInitiallyExpanded() => true;
    protected override bool IsProcessOptionsInitiallyExpanded() => false;
  }
}
