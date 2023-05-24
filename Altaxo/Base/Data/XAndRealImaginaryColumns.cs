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
  /// Collection of one independent variable called 'X' and one dependent variable called 'Y'.
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
      public override void Serialize(object obj, IXmlSerializationInfo info)
      {
        var s = (XAndRealImaginaryColumns)obj;
        info.AddValue("IndependentColumnName", s._independentColumnName);
        base.Serialize(obj, info);
      }

      public override object? Deserialize(object? o, IXmlDeserializationInfo info, object? parentobject)
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

    public XAndRealImaginaryColumns(XAndRealImaginaryColumns from) : base(from)
    {
    }

    public XAndRealImaginaryColumns(DataTable table, int groupNumber, string independentColumnName) : base(table, groupNumber, 1, 2)
    {
      IndependentColumnName = independentColumnName;
    }

    protected XAndRealImaginaryColumns(IXmlDeserializationInfo info, int version) : base(info, version)
    {
    }

    public override object Clone()
    {
      return new XAndRealImaginaryColumns(this);
    }

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

    protected override string GetIndependentVariableName(int idx)
    {
      return "X";
    }
    protected override string GetDependentVariableName(int idx)
    {
      return idx switch
      {
        0 => "Re",
        1 => "Im",
        _ => throw new System.IndexOutOfRangeException(nameof(idx)),
      };
    }

    public IReadableColumn? XColumn
    {
      get { return GetIndependentVariable(0); }
      set { SetIndependentVariable(0, value); }
    }
    public IReadableColumn? RealColumn
    {
      get { return GetDependentVariable(0); }
      set { SetDependentVariable(0, value); }
    }
    public IReadableColumn? ImaginaryColumn
    {
      get { return GetDependentVariable(1); }
      set { SetDependentVariable(1, value); }
    }

    public (double[]? X, double[]? Real, double[]? Imaginary, int RowCount) GetResolvedXRealImaginaryData()
    {
      var (Independent, Dependent, RowCount) = GetResolvedData();
      return (Independent[0], Dependent[0], Dependent[1], RowCount);
    }
  }
}
