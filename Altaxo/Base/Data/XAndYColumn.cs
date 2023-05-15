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
  public class XAndYColumn : IndependentAndDependentColumns
  {
    #region Serialization

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

    public XAndYColumn(XAndYColumn from) : base(from)
    {
    }

    public XAndYColumn(DataTable table, int groupNumber) : base(table, groupNumber, 1, 1)
    {
    }

    protected XAndYColumn(IXmlDeserializationInfo info, int version) : base(info, version)
    {
    }

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

    public IReadableColumn? XColumn
    {
      get { return GetIndependentVariable(0); }
      set { SetIndependentVariable(0, value); }
    }
    public IReadableColumn? YColumn
    {
      get { return GetDependentVariable(0); }
      set { SetDependentVariable(0, value); }
    }

    public (double[]? X, double[]? Y, int RowCount) GetResolvedXYData()
    {
      var result = GetResolvedData();
      return (result.Independent[0], result.Dependent[0], result.RowCount);
    }
  }
}
