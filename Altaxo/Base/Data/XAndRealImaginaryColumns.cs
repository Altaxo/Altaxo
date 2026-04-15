#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2023 Dr. Dirk Lellinger
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
  /// Collection of one independent variable called <c>X</c> and two dependent variables called <c>Re</c> and <c>Im</c>.
  /// </summary>
  /// <seealso cref="Altaxo.Data.IndependentAndDependentColumns" />
  public class XAndRealImaginaryColumns : IndependentAndDependentColumns
  {
    string _independentColumnName = "X";

    #region Serialization

    /// <summary>
    /// 2023-05-24 initial version
    /// </summary>
    /// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(XAndRealImaginaryColumns), 0)]
    protected class SerializationSurrogate0 : IndependentAndDependentColumns.XmlSerializationSurrogate0
    {
      /// <inheritdoc/>
      public override void Serialize(object o, IXmlSerializationInfo info)
      {
        var s = (XAndRealImaginaryColumns)o;
        info.AddValue("IndependentColumnName", s._independentColumnName);
        base.Serialize(o, info);
      }

      /// <inheritdoc/>
      public override object? Deserialize(object? o, IXmlDeserializationInfo info, object? parent)
      {
        var independentColumnName = info.GetString("IndependentColumnName");
        if (o is XAndRealImaginaryColumns s)
        {
          s.IndependentColumnName = independentColumnName;
          s.DeserializeSurrogate0(info);
        }
        else
        {
          s = new XAndRealImaginaryColumns(info, 0) { IndependentColumnName = independentColumnName };
        }
        return s;
      }


    }

    #endregion

    /// <summary>
    /// Initializes a new instance by copying the state from another <see cref="XAndRealImaginaryColumns"/>.
    /// </summary>
    /// <param name="from">The source instance to copy.</param>
    public XAndRealImaginaryColumns(XAndRealImaginaryColumns from) : base(from)
    {
    }

    /// <summary>
    /// Initializes a new instance for one independent and two dependent columns.
    /// </summary>
    /// <param name="table">The data table that contains the columns.</param>
    /// <param name="groupNumber">The group number applied to the columns.</param>
    /// <param name="independentColumnName">The display name of the independent column.</param>
    public XAndRealImaginaryColumns(DataTable table, int groupNumber, string independentColumnName) : base(table, groupNumber, 1, 2)
    {
      IndependentColumnName = independentColumnName;
    }

    /// <summary>
    /// Initializes a new instance from XML deserialization data.
    /// </summary>
    /// <param name="info">The deserialization info.</param>
    /// <param name="version">The serialized version.</param>
    protected XAndRealImaginaryColumns(IXmlDeserializationInfo info, int version) : base(info, version)
    {
    }

    /// <inheritdoc/>
    public override object Clone()
    {
      return new XAndRealImaginaryColumns(this);
    }

    /// <summary>
    /// Gets or sets the display name of the independent column.
    /// </summary>
    public string IndependentColumnName
    {
      get => _independentColumnName;
      set
      {
        if (string.IsNullOrEmpty(value))
          throw new System.ArgumentNullException(nameof(IndependentColumnName));
        _independentColumnName = value;
      }
    }

    /// <inheritdoc/>
    protected override string GetIndependentVariableName(int idx)
    {
      return "X";
    }
    /// <inheritdoc/>
    protected override string GetDependentVariableName(int idx)
    {
      return idx switch
      {
        0 => "Re",
        1 => "Im",
        _ => throw new System.IndexOutOfRangeException(nameof(idx)),
      };
    }

    /// <summary>
    /// Gets or sets the independent X column.
    /// </summary>
    public IReadableColumn? XColumn
    {
      get { return GetIndependentVariable(0); }
      set { SetIndependentVariable(0, value); }
    }
    /// <summary>
    /// Gets or sets the real dependent column.
    /// </summary>
    public IReadableColumn? RealColumn
    {
      get { return GetDependentVariable(0); }
      set { SetDependentVariable(0, value); }
    }
    /// <summary>
    /// Gets or sets the imaginary dependent column.
    /// </summary>
    public IReadableColumn? ImaginaryColumn
    {
      get { return GetDependentVariable(1); }
      set { SetDependentVariable(1, value); }
    }

    /// <summary>
    /// Retrieves the resolved X, real, and imaginary data arrays together with the row count.
    /// </summary>
    /// <returns>A tuple containing the resolved X, real, and imaginary data arrays and the row count.</returns>
    public (double[]? X, double[]? Real, double[]? Imaginary, int RowCount) GetResolvedXRealImaginaryData()
    {
      var (Independent, Dependent, RowCount) = GetResolvedData();
      return (Independent[0], Dependent[0], Dependent[1], RowCount);
    }
  }
}
