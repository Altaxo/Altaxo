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

namespace Altaxo.Data
{
  /// <summary>
  /// Options for transposing a worksheet.
  /// </summary>
  public class DataTableTransposeOptions : ICloneable
  {
    /// <summary>
    /// Gets or sets the number of data columns to transpose.
    /// </summary>
    /// <value>
    /// The number of data columns to transpose.
    /// </value>
    public int DataColumnsMoveToPropertyColumns { get; set; }

    /// <summary>
    /// Gets or sets the number of property columns to transpose.
    /// </summary>
    /// <value>
    /// The number of property columns to transpose.
    /// </value>
    public int PropertyColumnsMoveToDataColumns { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the existing column names of the source table should be stored in the first data column of the transposed table.
    /// </summary>
    /// <value>
    /// <c>true</c> if the existing column names of the source table should be stored in the first data column of the transposed table; otherwise, <c>false</c>.
    /// </value>
    public bool StoreDataColumnNamesInFirstDataColumn { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the first data column of the source table should be used to set the column names in the transposed table.
    /// </summary>
    /// <value>
    /// <c>true</c> if the first data column of the source table should be used to set the column names in the transposed table; otherwise, <c>false</c>.
    /// </value>
    public bool UseFirstDataColumnForColumnNaming { get; set; }

    private string _columnNamingPreString = "Row";

    public string ColumnNamingPreString
    {
      get { return _columnNamingPreString; }
      set { _columnNamingPreString = value ?? string.Empty; }
    }

    #region Serialization

    #region Version 0

    /// <summary>
    /// 2015-08-26 initial version.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(DataTableTransposeOptions), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (DataTableTransposeOptions)obj;

        info.AddValue("NumberOfDataColumnsMovingToPropertyColumns", s.DataColumnsMoveToPropertyColumns);
        info.AddValue("NumberOfPropertyColumnsMovingToDataColumns", s.PropertyColumnsMoveToDataColumns);
        info.AddValue("StoreDataColumnNamesInFirstDataColumn", s.StoreDataColumnNamesInFirstDataColumn);
        info.AddValue("UseFirstDataColumnForColumnNaming", s.UseFirstDataColumnForColumnNaming);
        info.AddValue("ColumnNamingPreString", s.ColumnNamingPreString);
      }



      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (DataTableTransposeOptions?)o ?? new DataTableTransposeOptions();
        s.DataColumnsMoveToPropertyColumns = info.GetInt32("NumberOfDataColumnsMovingToPropertyColumns");
        s.PropertyColumnsMoveToDataColumns = info.GetInt32("NumberOfPropertyColumnsMovingToDataColumns");
        s.StoreDataColumnNamesInFirstDataColumn = info.GetBoolean("StoreDataColumnNamesInFirstDataColumn");
        s.UseFirstDataColumnForColumnNaming = info.GetBoolean("UseFirstDataColumnForColumnNaming");
        s.ColumnNamingPreString = info.GetString("ColumnNamingPreString");
        return s;
      }
    }

    #endregion Version 0

    #endregion Serialization

    public object Clone()
    {
      return MemberwiseClone();
    }
  }
}
