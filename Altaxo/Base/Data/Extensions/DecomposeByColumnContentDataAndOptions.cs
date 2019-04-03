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
using System.Linq;

namespace Altaxo.Data
{
  /// <summary>
  /// Holds both the data (see <see cref="DataTableMultipleColumnProxy"/>) and the options (see <see cref="DecomposeByColumnContentOptions"/>) to perform
  /// the decomposition of a table containing a column with a cycling variable.
  /// </summary>
  public class DecomposeByColumnContentDataAndOptions : ICloneable
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
    public DecomposeByColumnContentOptions Options { get; private set; }

    /// <summary>Identifies the column with the cycling variable in the <see cref="DataTableMultipleColumnProxy"/> instance.</summary>
    public const string ColumnWithCyclingVariableIdentifier = "ColumnWithContentToSplit";

    /// <summary>Identifies all columns which participate in the <see cref="DataTableMultipleColumnProxy"/> instance.</summary>
    public const string ColumnsParticipatingIdentifier = "ColumnsParticipating";

    public DecomposeByColumnContentDataAndOptions(DataTableMultipleColumnProxy data, DecomposeByColumnContentOptions options)
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

      data.EnsureExistenceOfIdentifier(ColumnsParticipatingIdentifier);
      data.EnsureExistenceOfIdentifier(ColumnWithCyclingVariableIdentifier, 1);

      if (data.GetDataColumns(ColumnsParticipatingIdentifier).Count == 0)
      {
        if (throwIfNonCoherent)
          throw new ArgumentException(!data.ContainsIdentifier(ColumnsParticipatingIdentifier) ? "ColumnsToProcess is not set" : "ColumnsToProcess is empty");
      }

      if (data.GetDataColumnOrNull(ColumnWithCyclingVariableIdentifier) == null)
      {
        if (throwIfNonCoherent)
          throw new ArgumentException("Column with cycling variable was not included in columnsToProcess");
        else
        {
          var col = data.GetDataColumns(ColumnsParticipatingIdentifier).FirstOrDefault();
          if (null != col)
            data.SetDataColumn(ColumnWithCyclingVariableIdentifier, col);
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
      return new DecomposeByColumnContentDataAndOptions((DataTableMultipleColumnProxy)Data.Clone(), (DecomposeByColumnContentOptions)Options.Clone());
    }
  }
}
