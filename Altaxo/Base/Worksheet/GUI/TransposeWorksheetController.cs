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

namespace Altaxo.Worksheet.GUI
{
  /// <summary>
  /// Summary description for TransposeWorksheetController.
  /// </summary>
  public class TransposeWorksheetController : IMVCAController
  {
    Altaxo.Data.DataTable _table;
    TransposeWorksheetControl _view;

    public TransposeWorksheetController(Altaxo.Data.DataTable table)
    {
      _table = table;
    }
    #region IApplyController Members

    bool IsTransposePossible(int numMovedDataColumns, ref int indexDifferentColumn)
    {
      if(numMovedDataColumns>=_table.DataColumnCount)
        return true;

      Altaxo.Data.DataColumn masterColumn = _table[numMovedDataColumns];

      for(int i=0;i<_table.DataColumnCount;i++)
      {
        if(_table[i].GetType()!=masterColumn.GetType())
        {
          indexDifferentColumn = i;
          return false;
        }
      }
      return true;
    }

    public bool Apply()
    {
      
      int datacols = Math.Min(_table.DataColumnCount,_view.DataColumnsMoveToPropertyColumns);
      int propcols = Math.Min(_table.PropertyColumnCount, _view.PropertyColumnsMoveToDataColumns);

      // test if the transpose is possible
      int indexDifferentColumn = 0;
      if(!IsTransposePossible(datacols,ref indexDifferentColumn))
      {
        string message = string.Format("The columns to transpose have not all the same type. The type of column[{0}] ({1}) differs from the type of column[{2}] ({3}). Continue anyway?",
          indexDifferentColumn,
          _table[indexDifferentColumn].GetType(),
          datacols,
          _table[datacols].GetType());

        System.Windows.Forms.DialogResult result = System.Windows.Forms.MessageBox.Show(Current.MainWindow,message,"Attention",
          System.Windows.Forms.MessageBoxButtons.YesNo,System.Windows.Forms.MessageBoxIcon.Exclamation);
      
        if(result==System.Windows.Forms.DialogResult.No)
          return false;
      }

      
      string error = _table.Transpose(datacols,propcols);
      if(error!=null)
      {
        System.Windows.Forms.MessageBox.Show(Current.MainWindow,error,"An error has occured",
          System.Windows.Forms.MessageBoxButtons.OK,System.Windows.Forms.MessageBoxIcon.Error);
      }


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
        _view = value as TransposeWorksheetControl;
      }
    }

    public object ModelObject
    {
      get { return null; }
    }

    #endregion
  }
}
