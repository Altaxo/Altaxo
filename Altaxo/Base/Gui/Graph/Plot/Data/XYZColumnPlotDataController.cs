#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2016 Dr. Dirk Lellinger
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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Altaxo.Collections;
using Altaxo.Data;
using Altaxo.Graph.Plot.Data;
using Altaxo.Gui.Graph.Plot.Data;
using Altaxo.Main;

namespace Altaxo.Gui.Graph.Graph3D.Plot.Data
{
  /// <summary>
  /// Summary description for LineScatterPlotDataController.
  /// </summary>
  [UserControllerForObject(typeof(XYZColumnPlotData))]
  [ExpectedTypeOfView(typeof(IColumnPlotDataView))]
  public class XYZColumnPlotDataController
    :
    ColumnPlotDataControllerBase<XYZColumnPlotData>
  {
  }
}
