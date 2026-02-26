#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2026 Dr. Dirk Lellinger
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
using System.Collections.Generic;
using Altaxo.Calc.LinearAlgebra;
using Altaxo.Data;

namespace Altaxo.Calc.FitFunctions.Peaks
{
  /// <summary>
  /// Peak function whose shape is defined by a two-dimensional lookup table stored in a <see cref="DataTable" />.
  /// </summary>
  /// <remarks>
  /// The table is expected to contain multiple <see cref="DoubleColumn" /> instances (the participating columns) that
  /// represent peak curves. The x values are taken from the corresponding x column, and the peak position values are
  /// taken from a property column. The resulting matrix is then interpolated using the base implementation.
  /// </remarks>
  public record InterpolatedPeakFunctionFrom2DTable : InterpolatedPeakFunctionFromMatrix
  {
    /// <summary>
    /// Gets the name of the data table that contains the peak curves and any associated metadata.
    /// </summary>
    public string TableName { get; init; }

    /// <summary>
    /// Gets the name of the property column that stores the peak position values for each participating column.
    /// </summary>
    public string NameOfPropertyForPeakPosition { get; init; }

    /// <summary>
    /// Gets the column group number used to select the participating columns within the table.
    /// </summary>
    public int GroupNumberOfParticipatingColumns { get; init; }

    public bool PropertyIsPeakWidth { get; init; }


    #region Serialization

    /// <summary>
    /// Serialization surrogate for <see cref="InterpolatedPeakFunctionFrom2DTable"/> used by the
    /// Altaxo XML serializer.
    /// </summary>
    /// <remarks>
    /// 2026-02-25 Initial version
    /// </remarks>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(InterpolatedPeakFunctionFrom2DTable), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (InterpolatedPeakFunctionFrom2DTable)obj;
        info.AddValue("NumberOfTerms", s.NumberOfTerms);
        info.AddValue("OrderOfBackgroundPolynomial", s.OrderOfBaselinePolynomial);
        info.AddValue("TableName", s.TableName);
        info.AddValue("GroupNumberOfParticipatingColumns", s.GroupNumberOfParticipatingColumns);
        info.AddValue("NameOfProperty", s.NameOfPropertyForPeakPosition);
        info.AddValue("PropertyIsPeakWidth", s.PropertyIsPeakWidth);
      }

      /// <inheritdoc/>
      public virtual object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var numberOfTerms = info.GetInt32("NumberOfTerms");
        var orderOfBackgroundPolynomial = info.GetInt32("OrderOfBackgroundPolynomial");
        var tableName = info.GetString("TableName");
        var groupNumberOfParticipatingColumns = info.GetInt32("GroupNumberOfParticipatingColumns");
        var nameOfProperty = info.GetString("NameOfProperty");
        var propertyIsPeakWidth = info.GetBoolean("PropertyIsPeakWidth");

        return new InterpolatedPeakFunctionFrom2DTable(numberOfTerms, orderOfBackgroundPolynomial, tableName, groupNumberOfParticipatingColumns, nameOfProperty, propertyIsPeakWidth);
      }
    }

    #endregion Serialization

    /// <summary>
    /// Initializes a new default instance of the <see cref="InterpolatedPeakFunctionFrom2DTable"/> class.
    /// </summary>
    public InterpolatedPeakFunctionFrom2DTable()
      : this(1, -1, string.Empty, 0, string.Empty, false)
    {
    }


    /// <summary>
    /// Initializes a new instance of the <see cref="InterpolatedPeakFunctionFrom2DTable"/> class.
    /// </summary>
    /// <param name="numberOfTerms">The number of peak terms.</param>
    /// <param name="orderOfBaselinePolynomial">
    /// The order of the baseline polynomial, or <c>-1</c> to disable the baseline.
    /// </param>
    /// <param name="table">The table that contains the peak curves and metadata.</param>
    /// <param name="groupNumberOfParticipatingColumns">
    /// The column group number used to select the participating columns.
    /// </param>
    /// <param name="nameOfPropertyForPositionOrWidth">
    /// The name of the property column that stores the peak position values or width values for each participating column.
    /// </param>
    /// <param name="propertyIsPeakWidth">
    /// Indicates whether the property column stores peak width values (true) or peak position values (false).
    /// </param>
    /// <exception cref="ArgumentException">
    /// Thrown if no participating columns are found, if no x column can be determined, or if the x column is not a
    /// <see cref="DoubleColumn" />.
    /// </exception>
    public InterpolatedPeakFunctionFrom2DTable(int numberOfTerms,
                                                   int orderOfBaselinePolynomial, DataTable table, int groupNumberOfParticipatingColumns, string nameOfPropertyForPositionOrWidth, bool propertyIsPeakWidth)
      : base(numberOfTerms, orderOfBaselinePolynomial)
    {
      TableName = string.Empty;
      NameOfPropertyForPeakPosition = string.Empty;
      GroupNumberOfParticipatingColumns = 0;
      PropertyIsPeakWidth = propertyIsPeakWidth;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InterpolatedPeakFunctionFrom2DTable"/> class.
    /// </summary>
    /// <param name="numberOfTerms">The number of peak terms.</param>
    /// <param name="orderOfBaselinePolynomial">
    /// The order of the baseline polynomial, or <c>-1</c> to disable the baseline.
    /// </param>
    /// <param name="tableName">The full name of the data table that contains the peak curves and metadata.</param>
    /// <param name="groupNumberOfParticipatingColumns">
    /// The column group number used to select the participating columns.
    /// </param>
    /// <param name="nameOfPropertyForPositionOrWidth">
    /// The name of the property column that stores the peak position values or width values for each participating column.
    /// </param>
    /// <param name="propertyIsPeakWidth">
    /// Indicates whether the property column stores peak width values (true) or peak position values (false).  
    /// </param>
    /// <exception cref="ArgumentException">
    /// Thrown if no participating columns are found, if no x column can be determined, or if the x column is not a
    /// <see cref="DoubleColumn" />.
    /// </exception>
    public InterpolatedPeakFunctionFrom2DTable(
      int numberOfTerms,
      int orderOfBaselinePolynomial,
      string tableName,
      int groupNumberOfParticipatingColumns,
      string nameOfPropertyForPositionOrWidth,
      bool propertyIsPeakWidth)
      : base(numberOfTerms, orderOfBaselinePolynomial)
    {
      TableName = tableName;
      GroupNumberOfParticipatingColumns = groupNumberOfParticipatingColumns;
      NameOfPropertyForPeakPosition = nameOfPropertyForPositionOrWidth;
      PropertyIsPeakWidth = propertyIsPeakWidth;
    }


    /// <inheritdoc/>
    public override void Initialize()
    {
      if (string.IsNullOrEmpty(TableName) || string.IsNullOrEmpty(NameOfPropertyForPeakPosition))
        throw new ArgumentException($"This instance of {this.GetType()} is not yet configured.");

      var project = Current.Project ?? throw new InvalidOperationException("No project is currently loaded.");

      if (!project.DataTableCollection.TryGetValue(TableName, out var table))
        throw new ArgumentException($"The project does not contain a data table with the name '{TableName}'.");

      var pcol = table.PropCols.TryGetColumn(NameOfPropertyForPeakPosition) ?? throw new ArgumentException($"The table '{TableName}' does not contain a property column with the name '{NameOfPropertyForPeakPosition}'.");

      var listParticipatingColumns = new List<int>();
      for (int i = 0; i < table.DataColumnCount; ++i)
      {
        var gn = table.DataColumns.GetColumnGroup(i);
        var t = table.DataColumns.GetColumnKind(i);

        if (table[i] is DoubleColumn &&
            gn == GroupNumberOfParticipatingColumns &&
            t == ColumnKind.V
            && RMath.IsFinite(pcol[i])
            )
        {
          listParticipatingColumns.Add(i);
        }
      }
      if (listParticipatingColumns.Count == 0)
        throw new ArgumentException($"No columns found in the table with group number {GroupNumberOfParticipatingColumns} and column kind {ColumnKind.V}.");

      var columnX = table.DataColumns.FindXColumnOf(table[listParticipatingColumns[0]]);
      if (columnX == null)
        throw new ArgumentException($"No X column found in the table for the first participating column.");
      if (columnX is not DoubleColumn)
        throw new ArgumentException($"The X column found in the table for the first participating column is not a DoubleColumn.");

      var xValues = ((DoubleColumn)columnX).Array;
      var yValues = new double[listParticipatingColumns.Count];

      for (int i = 0; i < listParticipatingColumns.Count; ++i)
      {
        yValues[i] = pcol[listParticipatingColumns[i]];
      }

      var z = CreateMatrix.Dense<double>(listParticipatingColumns.Count, xValues.Length);

      for (int i = 0; i < listParticipatingColumns.Count; ++i)
      {
        var col = (DoubleColumn)table[listParticipatingColumns[i]];
        for (int j = 0; j < xValues.Length; ++j)
        {
          z[i, j] = col[j];
        }
      }

      InitializeSpline(yValues, xValues, z);
    }
  }
}
