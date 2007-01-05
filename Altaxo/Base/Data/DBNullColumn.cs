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
using Altaxo.Serialization;
using Altaxo;

namespace Altaxo.Data
{
  /// <summary>
  /// AltaxoDBNullColumn serves as a placeholder in case the column
  /// type is not yet known, but some attibutes of the column must
  /// already been set
  /// </summary>
  public class DBNullColumn : DataColumn
  {
    public DBNullColumn()
    {
    }

    public override object Clone()
    {
      return new DBNullColumn();
    }

    public override int Count
    {
      get
      {
        return 0;
      }
    }

    public override System.Type GetColumnStyleType()
    { 
      return null;
    }
    public override void SetValueAt(int i, AltaxoVariant val)
    {
    }
    public override AltaxoVariant GetVariantAt(int i)
    {
      return null;
    }
    public override bool IsElementEmpty(int i)
    {
      return true;
    }

    public override void CopyDataFrom(Altaxo.Data.DataColumn v)
    {
    }
    public override void RemoveRows(int nFirstRow, int nCount) // removes nCount rows starting from nFirstRow 
    {
    }
    public override void InsertRows(int nBeforeRow, int nCount) // inserts additional empty rows
    {
    }
  }
}
