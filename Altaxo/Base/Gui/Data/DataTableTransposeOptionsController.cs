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

#nullable disable
using System;
using System.Collections.Generic;

namespace Altaxo.Gui.Data
{
  #region Interfaces

  public interface IDataTableTransposeOptionsView
  {
    string ColumnNamingPreString { get; set; }

    /// <summary>
    /// Get/sets the number of data columns that are moved to the property columns before transposing the data columns.
    /// </summary>
    int DataColumnsMoveToPropertyColumns { get; set; }

    /// <summary>
    /// Get/sets the number of property columns that are moved after transposing the data columns to the data columns collection.
    /// </summary>
    int PropertyColumnsMoveToDataColumns { get; set; }

    bool StoreDataColumnNamesInFirstDataColumn { get; set; }
    bool UseFirstDataColumnForColumnNaming { get; set; }
  }

  #endregion Interfaces

  /// <summary>
  /// Summary description for TransposeWorksheetController.
  /// </summary>
  [ExpectedTypeOfView(typeof(IDataTableTransposeOptionsView))]
  [UserControllerForObject(typeof(Altaxo.Data.DataTableTransposeOptions))]
  public class DataTableTransposeOptionsController : MVCANControllerEditOriginalDocBase<Altaxo.Data.DataTableTransposeOptions, IDataTableTransposeOptionsView>
  {
    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (_view is not null)
      {
        _view.DataColumnsMoveToPropertyColumns = _doc.DataColumnsMoveToPropertyColumns;
        _view.PropertyColumnsMoveToDataColumns = _doc.PropertyColumnsMoveToDataColumns;
        _view.StoreDataColumnNamesInFirstDataColumn = _doc.StoreDataColumnNamesInFirstDataColumn;
        _view.UseFirstDataColumnForColumnNaming = _doc.UseFirstDataColumnForColumnNaming;
        _view.ColumnNamingPreString = _doc.ColumnNamingPreString;
      }
    }

    public override bool Apply(bool disposeController)
    {
      _doc.DataColumnsMoveToPropertyColumns = _view.DataColumnsMoveToPropertyColumns;
      _doc.PropertyColumnsMoveToDataColumns = _view.PropertyColumnsMoveToDataColumns;
      _doc.StoreDataColumnNamesInFirstDataColumn = _view.StoreDataColumnNamesInFirstDataColumn;
      _doc.UseFirstDataColumnForColumnNaming = _view.UseFirstDataColumnForColumnNaming;
      _doc.ColumnNamingPreString = _view.ColumnNamingPreString;

      return ApplyEnd(true, disposeController);
    }

    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }
  }
}
