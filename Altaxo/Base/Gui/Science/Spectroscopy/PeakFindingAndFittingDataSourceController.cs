#region Copyright

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

using Altaxo.Gui.Data;
using Altaxo.Science.Spectroscopy;

namespace Altaxo.Gui.Science.Spectroscopy
{
  [UserControllerForObject(typeof(PeakSearchingAndFittingDataSource))]
  public class PeakFindingAndFittingDataSourceController : DataSourceControllerBase<PeakSearchingAndFittingDataSource>
  {
    protected override IMVCANController GetProcessDataController()
    {
      var ctrl = new ListOfXAndYColumnController();
      ctrl.InitializeDocument(_doc.ProcessData);
      Current.Gui.FindAndAttachControlTo(ctrl);
      return ctrl;
    }
  }
}
