#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2019 Dr. Dirk Lellinger
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

using System;

namespace Altaxo.MachineLearning.ML_Net
{
  public partial class TableDataView
  {
    public class GetterClass
    {
      RowCursor _cursor;
      Altaxo.Data.DataColumn _column;

      public GetterClass(Altaxo.Data.DataColumn column, RowCursor cursor)
      {
        _column = column;
        _cursor = cursor;
      }

      public void TextGetterImplementation(ref ReadOnlyMemory<char> value)
      {
        var s = (string)_column[_cursor.CurrentRowIndex];
        value = s.AsMemory();
      }

      public void DoubleGetterImplementation(ref double d)
      {
        d = _column[_cursor.CurrentRowIndex];
      }

      public void FloatGetterImplementation(ref float d)
      {
        d = (float)_column[_cursor.CurrentRowIndex];
      }

      public void DateTimeGetterImplementation(ref DateTime d)
      {
        d = (DateTime)_column[_cursor.CurrentRowIndex];
      }

      public void BooleanGetterImplementation(ref bool d)
      {
        d = _column[_cursor.CurrentRowIndex].ToNullableBoolean() ?? false;
      }
    }
  }
}

