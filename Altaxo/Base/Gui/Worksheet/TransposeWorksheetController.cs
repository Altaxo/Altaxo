#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2007 Dr. Dirk Lellinger
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
#endregion

using System;
using Altaxo.Gui;

namespace Altaxo.Gui.Worksheet
{
	#region Interfaces

	public interface ITransposeWorksheetView
	{
		
    /// <summary>
    /// Get/sets the number of data columns that are moved to the property columns before transposing the data columns.
    /// </summary>
		int DataColumnsMoveToPropertyColumns { get; set; }

		 /// <summary>
    /// Get/sets the number of property columns that are moved after transposing the data columns to the data columns collection.
    /// </summary>
		int PropertyColumnsMoveToDataColumns { get; set; }
	}

	#endregion

	/// <summary>
  /// Summary description for TransposeWorksheetController.
  /// </summary>
	[ExpectedTypeOfView(typeof(ITransposeWorksheetView))]
  public class TransposeWorksheetController : IMVCAController
  {
    Altaxo.Data.DataTable _table;
    ITransposeWorksheetView _view;

    public TransposeWorksheetController(Altaxo.Data.DataTable table)
    {
      _table = table;
    }
    #region IApplyController Members

   

    public bool Apply()
    {
      Altaxo.Worksheet.Commands.Transpose.DoTranspose(_table, _view.DataColumnsMoveToPropertyColumns, _view.PropertyColumnsMoveToDataColumns, true);
      return true;
    }

    #endregion

    #region IMVCController Members

    public object ViewObject
    {
      get
      {
        return _view;
      }
      set
      {
        _view = value as ITransposeWorksheetView;
      }
    }

    public object ModelObject
    {
      get { return null; }
    }

    #endregion
  }
}
