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

#nullable enable
using System;

namespace Altaxo.Data
{
  /// <summary>
  /// Placeholder column used when the actual column type is not yet known.
  /// </summary>
  public class DBNullColumn : DataColumn
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="DBNullColumn"/> class.
    /// </summary>
    public DBNullColumn()
    {
    }

    /// <inheritdoc />
    public override object Clone()
    {
      return new DBNullColumn();
    }

    /// <summary>
    /// Gets the type of the colum's items.
    /// </summary>
    /// <value>
    /// The type of the item.
    /// </value>
    public override Type ItemType { get { return typeof(object); } }

    /// <inheritdoc />
    public override int Count
    {
      get
      {
        return 0;
      }
    }

    /// <inheritdoc />
    public override System.Type? GetColumnStyleType()
    {
      return null;
    }

    /// <inheritdoc />
    public override void SetValueAt(int i, AltaxoVariant val)
    {
    }

    /// <inheritdoc />
    public override AltaxoVariant GetVariantAt(int i)
    {
      return new AltaxoVariant((object?)null);
    }

    /// <inheritdoc />
    public override bool IsElementEmpty(int i)
    {
      return true;
    }

    /// <inheritdoc />
    public override void SetElementEmpty(int i)
    {
    }

    /// <inheritdoc />
    public override void CopyDataFrom(object o)
    {
    }

    /// <inheritdoc />
    public override void RemoveRows(int nFirstRow, int nCount) // removes nCount rows starting from nFirstRow
    {
    }

    /// <inheritdoc />
    public override void InsertRows(int nBeforeRow, int nCount) // inserts additional empty rows
    {
    }
  }
}
