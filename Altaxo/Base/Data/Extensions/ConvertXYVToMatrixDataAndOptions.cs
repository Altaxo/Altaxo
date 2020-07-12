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

#nullable enable
using System;
using System.Linq;

namespace Altaxo.Data
{
  /// <summary>
  /// Holds both the data (see <see cref="DataTableMultipleColumnProxy"/>) and the options (see <see cref="ConvertXYVToMatrixOptions"/>) to perform
  /// the decomposition of a table containing a column with a cycling variable.
  /// </summary>
  public class ConvertXYVToMatrixDataAndOptions : ICloneable
  {
    /// <summary>
    /// Holds the data nessessary for decomposing of a table containing a column with a cycling variable.
    /// </summary>
    /// <value>
    /// The data.
    /// </value>
    public DataTableMultipleColumnProxy Data { get; private set; }

    /// <summary>
    /// Holds the options nessessary for decomposing of a table containing a column with a cycling variable.
    /// </summary>
    /// <value>
    /// The options.
    /// </value>
    public ConvertXYVToMatrixOptions Options { get; private set; }

    /// <summary>Identifies the column acting as X-column in the <see cref="DataTableMultipleColumnProxy"/> instance.</summary>
    public const string ColumnX = "X-Column";

    /// <summary>Identifies the column acting as Y-column in the <see cref="DataTableMultipleColumnProxy"/> instance.</summary>
    public const string ColumnY = "Y-Column";


    /// <summary>Identifies all columns which participate in the <see cref="DataTableMultipleColumnProxy"/> instance.</summary>
    public const string ColumnV = "V-Column";

    public ConvertXYVToMatrixDataAndOptions(DataTableMultipleColumnProxy data, ConvertXYVToMatrixOptions options)
    {
      Data = data;
      Options = options;
    }

    /// <summary>
    /// Tests if the data in <paramref name="data"/> can be used for the DecomposeByColumnContent action.
    /// </summary>
    /// <param name="data">The data to test.</param>
    /// <param name="throwIfNonCoherent">If true, an exception is thrown if any problems are detected. If false, it is tried to rectify the problem by making some assumtions.</param>
    public static void EnsureCoherence(DataTableMultipleColumnProxy data, bool throwIfNonCoherent)
    {
      if (null == data.DataTable) // this is mandatory, thus an exception is always thrown
      {
        throw new ArgumentNullException("SourceTable is null, it must be set before");
      }

      data.EnsureExistenceOfIdentifier(ColumnX, 1);
      data.EnsureExistenceOfIdentifier(ColumnY, 1);
      data.EnsureExistenceOfIdentifier(ColumnV, 1);

      if (data.GetDataColumns(ColumnV).Count == 0)
      {
        if (throwIfNonCoherent)
          throw new ArgumentException(!data.ContainsIdentifier(ColumnV) ? "ColumnsToProcess is not set" : "ColumnsToProcess is empty");
      }

      if (data.GetDataColumnOrNull(ColumnX) == null)
      {
        if (throwIfNonCoherent)
          throw new ArgumentException("X column  not included in columnsToProcess");
        else
        {
          var col = data.GetDataColumns(ColumnV).FirstOrDefault();
          if (null != col)
            data.SetDataColumn(ColumnX, col);
        }
      }

      if (data.GetDataColumnOrNull(ColumnY) == null)
      {
        if (throwIfNonCoherent)
          throw new ArgumentException("Y column  not included in columnsToProcess");
        else
        {
          var col = data.GetDataColumns(ColumnV).FirstOrDefault();
          if (null != col)
            data.SetDataColumn(ColumnY, col);
        }
      }

    }

    /// <summary>
    /// Creates a new object that is a copy of the current instance.
    /// </summary>
    /// <returns>
    /// A new object that is a copy of this instance.
    /// </returns>
    public object Clone()
    {
      return new ConvertXYVToMatrixDataAndOptions((DataTableMultipleColumnProxy)Data.Clone(), (ConvertXYVToMatrixOptions)Options.Clone());
    }
  }
}
