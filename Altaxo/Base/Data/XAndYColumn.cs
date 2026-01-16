#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2025 Dr. Dirk Lellinger
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

using Altaxo.Serialization.Xml;

namespace Altaxo.Data
{
  /// <summary>
  /// Collection of one independent variable called 'X' and one dependent variable called 'Y'.
  /// </summary>
  /// <seealso cref="Altaxo.Data.IndependentAndDependentColumns" />
  public class XAndYColumn : IndependentAndDependentColumns
  {
    #region Serialization

    protected XAndYColumn(IXmlDeserializationInfo info, int version) : base(info, version)
    {
    }

    /// <summary>
    /// 2023-05-15 initial version
    /// </summary>
    /// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(XAndYColumn), 0)]
    protected class SerializationSurrogate0 : IndependentAndDependentColumns.XmlSerializationSurrogate0
    {
      public override object? Deserialize(object? o, IXmlDeserializationInfo info, object? parentobject)
      {
        if (o is XAndYColumn s)
          s.DeserializeSurrogate0(info);
        else
          s = new XAndYColumn(info, 0);
        return s;
      }
    }

    #endregion

    /// <summary>
    /// Initializes a new instance by copying the state from another <see cref="XAndYColumn"/>.
    /// </summary>
    /// <param name="from">The source instance to copy.</param>
    public XAndYColumn(XAndYColumn from) : base(from)
    {
    }

    /// <summary>
    /// Initializes a new instance for a single X and Y column bound to the specified data table and group.
    /// </summary>
    /// <param name="table">The data table that contains the columns.</param>
    /// <param name="groupNumber">The group number applied to both X and Y columns.</param>
    public XAndYColumn(DataTable table, int groupNumber) : base(table, groupNumber, 1, 1)
    {
    }



    public XAndYColumn(DataTable table, int groupNumber, Altaxo.Data.IReadableColumn xColumn, Altaxo.Data.IReadableColumn yColumn)
     : base(table, groupNumber, 1, 1)
    {
      XColumn = xColumn;
      YColumn = yColumn;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="IndependentAndDependentColumns"/> class from given proxies of DataTable and columns.
    /// This constructor is intended for internal use only, e.g. for creating copies of existing instances.
    /// ATTENTION: The proxies are not cloned! 
    /// </summary>
    /// <param name="table">The table proxy.</param>
    /// <param name="groupNumber">The group number of the x- and y-column.</param>
    /// <param name="xCol">The proxy of the x-column.</param>
    /// <param name="yCol">The proxy of the y-column.</param>
    protected XAndYColumn(DataTableProxy table, int groupNumber, IReadableColumnProxy xCol, IReadableColumnProxy yCol)
      : base(table, groupNumber, xCol, yCol)
    {
    }


    /// <inheritdoc/>
    public override object Clone()
    {
      return new XAndYColumn(this);
    }

    protected override string GetIndependentVariableName(int idx)
    {
      return "X";
    }
    protected override string GetDependentVariableName(int idx)
    {
      return "Y";
    }

    /// <summary>
    /// Gets or sets the X (independent) column.
    /// </summary>
    public virtual IReadableColumn? XColumn
    {
      get { return GetIndependentVariable(0); }
      set { SetIndependentVariable(0, value); }
    }
    /// <summary>
    /// Gets or sets the Y (dependent) column.
    /// </summary>
    public virtual IReadableColumn? YColumn
    {
      get { return GetDependentVariable(0); }
      set { SetDependentVariable(0, value); }
    }

    /// <inheritdoc/>
    public override string ToString()
    {
      return string.Format("{0}(X), {1}(Y)", _independentVariables[0].ToString(), _dependentVariables[0].ToString());
    }

    /// <summary>
    /// Retrieves the resolved X and Y data arrays along with the number of rows in the dataset.
    /// </summary>
    /// <returns>A tuple containing the X data array, the Y data array, and the row count. The X and Y arrays may be null if no
    /// data is available. The row count indicates the number of data points in the arrays.</returns>
    public (double[]? X, double[]? Y, int RowCount) GetResolvedXYData()
    {
      var result = GetResolvedData();
      return (result.Independent[0], result.Dependent[0], result.RowCount);
    }

    /// <summary>
    /// Retrieves a combined display name for the X and Y columns based on the provided style.
    /// </summary>
    /// <param name="style">A bit-packed style value; low nibble selects X name detail, high nibble selects Y name detail.</param>
    /// <returns>A formatted name string for X and/or Y depending on style.</returns>
    public virtual string GetName(int style)
    {
      int st = (int)style;
      int sx = st & 0x0F;
      int sy = (st & 0xF0) >> 4;

      var stb = new System.Text.StringBuilder();
      if (sx > 0)
      {
        stb.Append(GetXName(sx - 1));
        if (sx > 0 && sy > 0)
          stb.Append("(X)");
        if (sy > 0)
          stb.Append(",");
      }
      if (sy > 0)
      {
        stb.Append(GetYName(sy - 1));
        if (sx > 0 && sy > 0)
          stb.Append("(Y)");
      }

      return stb.ToString();
    }

    /// <summary>
    /// Gets the name of the x column, depending on the provided level.
    /// </summary>
    /// <param name="level">The level (0..2).</param>
    /// <returns>The name of the x-column, depending on the provided level: 0: only name of the data column. 1: table name and column name. 2: table name, collection, and column name.</returns>
    public string GetXName(int level)
    {
      IReadableColumn? col = XColumn;
      if (col is Altaxo.Data.DataColumn dataCol)
      {
        var table = Altaxo.Data.DataTable.GetParentDataTableOf(dataCol);
        string tablename = table is null ? string.Empty : table.Name + "\\";
        string collectionname = table is null ? string.Empty : (table.PropertyColumns.ContainsColumn(dataCol) ? "PropCols\\" : "DataCols\\");
        if (level <= 0)
          return dataCol.Name;
        else if (level == 1)
          return tablename + dataCol.Name;
        else
          return tablename + collectionname + dataCol.Name;
      }
      else if (col is not null)
      {
        return col.FullName;
      }
      else if (_independentVariables.Length > 0 && _independentVariables[0] is not null)
      {
        return _independentVariables[0].GetName(level) + " (broken)";
      }
      else
      {
        return " (broken)";
      }
    }

    /// <summary>
    /// Gets the name of the y column, depending on the provided level.
    /// </summary>
    /// <param name="level">The level (0..2).</param>
    /// <returns>The name of the y-column, depending on the provided level: 0: only name of the data column. 1: table name and column name. 2: table name, collection, and column name.</returns>
    public string GetYName(int level)
    {
      IReadableColumn? col = YColumn;
      if (col is Altaxo.Data.DataColumn dataCol)
      {
        var table = Altaxo.Data.DataTable.GetParentDataTableOf(dataCol);
        string tablename = table is null ? string.Empty : table.Name + "\\";
        string collectionname = table is null ? string.Empty : (table.PropertyColumns.ContainsColumn(dataCol) ? "PropCols\\" : "DataCols\\");
        if (level <= 0)
          return dataCol.Name;
        else if (level == 1)
          return tablename + dataCol.Name;
        else
          return tablename + collectionname + dataCol.Name;
      }
      else if (col is not null)
      {
        return col.FullName;
      }
      else if (_dependentVariables.Length > 0 && _dependentVariables[0] is not null)
      {
        return _dependentVariables[0].GetName(level) + " (broken)";
      }
      else
      {
        return " (broken)";
      }
    }

    /// <summary>
    /// Creates a new <see cref="XAndYColumn"/> using existing proxies without cloning them.
    /// </summary>
    /// <param name="table">The data table proxy.</param>
    /// <param name="groupNumber">The group number for X and Y.</param>
    /// <param name="xCol">The X column proxy.</param>
    /// <param name="yCol">The Y column proxy.</param>
    /// <returns>A new <see cref="XAndYColumn"/> instance bound to the provided proxies.</returns>
    public static XAndYColumn CreateFromProxies(DataTableProxy table, int groupNumber, IReadableColumnProxy xCol, IReadableColumnProxy yCol)
    {
      return new XAndYColumn(table, groupNumber, xCol, yCol);
    }

    /// <summary>
    /// Gets the kind of the X column as defined by the parent data table.
    /// </summary>
    /// <returns>The column kind of X, or <see cref="ColumnKind.V"/> if unavailable.</returns>
    public ColumnKind GetXKind()
    {
      if (XColumn is null)
        return ColumnKind.V;
      var rootCol = IReadableColumn.GetRootDataColumn(XColumn);
      if (rootCol is null)
        return ColumnKind.V;
      else
        return DataTable.DataColumns.GetColumnKind(rootCol);
    }

    /// <summary>
    /// Gets the kind of the Y column as defined by the parent data table.
    /// </summary>
    /// <returns>The column kind of Y, or <see cref="ColumnKind.V"/> if unavailable.</returns>
    public ColumnKind GetYKind()
    {
      if (YColumn is null)
        return ColumnKind.V;
      var rootCol = IReadableColumn.GetRootDataColumn(YColumn);
      if (rootCol is null)
        return ColumnKind.V;
      else
        return DataTable.DataColumns.GetColumnKind(rootCol);
    }

    /// <summary>
    /// Gets the property value of the x-column.
    /// </summary>
    /// <param name="propertyName">Name of the property.</param>
    /// <returns>The property value of the x-column. If not found, returns an empty <see cref="AltaxoVariant"/>.</returns>
    public AltaxoVariant GetXPropertyValue(string propertyName)
    {
      return GetPropertyValueOfIndependentOrDependentVariable(isIndependent: true, 0, propertyName);
    }

    /// <summary>
    /// Gets the property value of the y-column.
    /// </summary>
    /// <param name="propertyName">Name of the property.</param>
    /// <returns>The property value of the y-column. If not found, returns an empty <see cref="AltaxoVariant"/>.</returns>
    public AltaxoVariant GetYPropertyValue(string propertyName)
    {
      return GetPropertyValueOfIndependentOrDependentVariable(isIndependent: false, 0, propertyName);
    }
  }
}
