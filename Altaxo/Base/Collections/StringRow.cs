#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2018 Dr. Dirk Lellinger
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
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Collections
{
  /// <summary>
  /// Represents a row of string values.
  /// </summary>
  public class StringRow
  {
    /// <summary>
    /// The string values stored in this row.
    /// </summary>
    protected string[] _data;

    /// <summary>
    /// Initializes a new instance of the <see cref="StringRow"/> class.
    /// </summary>
    /// <param name="numCols">The number of columns in the row.</param>
    public StringRow(int numCols)
    {
      _data = new string[numCols];
    }

    /// <summary>
    /// Gets or sets the string at the specified column index.
    /// </summary>
    /// <param name="i">The zero-based column index.</param>
    public string this[int i]
    {
      get
      {
        return _data[i];
      }
      set
      {
        _data[i] = value;
      }
    }

    /// <summary>
    /// Gets or sets the value of column 0.
    /// </summary>
    public string Column0
    {
      get
      {
        return _data[0];
      }
      set
      {
        _data[0] = value;
      }
    }

    /// <summary>
    /// Gets or sets the value of column 1.
    /// </summary>
    public string Column1
    {
      get
      {
        return _data[1];
      }
      set
      {
        _data[1] = value;
      }
    }

    /// <summary>
    /// Gets or sets the value of column 2.
    /// </summary>
    public string Column2
    {
      get
      {
        return _data[2];
      }
      set
      {
        _data[2] = value;
      }
    }

    /// <summary>
    /// Gets or sets the value of column 3.
    /// </summary>
    public string Column3
    {
      get
      {
        return _data[3];
      }
      set
      {
        _data[3] = value;
      }
    }

    /// <summary>
    /// Gets or sets the value of column 4.
    /// </summary>
    public string Column4
    {
      get
      {
        return _data[4];
      }
      set
      {
        _data[4] = value;
      }
    }

    /// <summary>
    /// Gets or sets the value of column 5.
    /// </summary>
    public string Column5
    {
      get
      {
        return _data[5];
      }
      set
      {
        _data[5] = value;
      }
    }
  }

  /// <summary>
  /// Represents a <see cref="StringRow"/> with two columns.
  /// </summary>
  public class TwoStringRow : StringRow
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="TwoStringRow"/> class.
    /// </summary>
    public TwoStringRow()
      : base(2)
    {
    }
  }

  /// <summary>
  /// Represents a <see cref="StringRow"/> with three columns.
  /// </summary>
  public class ThreeStringRow : StringRow
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="ThreeStringRow"/> class.
    /// </summary>
    public ThreeStringRow()
      : base(3)
    {
    }
  }
}
