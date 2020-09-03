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
  /// Contains options how to split a table that contains an independent variable with cycling values into
  /// another table, where this independent variable is unique and sorted.
  /// </summary>
  public class ExpandCyclingVariableColumnOptions
    :
    Main.SuspendableDocumentLeafNodeWithEventArgs,
    ICloneable
  {
    #region Enums

    public enum DestinationXColumn
    {
      CyclingVariable,
      FirstAveragedColumn
    }

    public enum OutputFormat
    {
      GroupOneColumn,
      GroupAllColumns,
    }

    public enum OutputSorting
    {
      None,
      Ascending,
      Descending
    }

    #endregion Enums

    #region Members

    /// <summary>Designates whether the destination x column is derived from the cycling variable column or from the (first) averaged column.</summary>
    protected DestinationXColumn _destinationX;

    /// <summary>Designates the order of the newly created columns of the dependent variables.</summary>
    protected OutputFormat _destinationOutput;

    /// <summary>If set, the destination columns will be sorted according to the first averaged column (if there is any).</summary>
    protected OutputSorting _destinationColumnSorting;

    /// <summary>If set, the destination rows will be sorted according to the destination x column.</summary>
    protected OutputSorting _destinationRowSorting;

    #endregion Members

    #region Serialization

    #region Version 0

    /// <summary>
    /// 2014-11-02 initial version.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ExpandCyclingVariableColumnOptions), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (ExpandCyclingVariableColumnOptions)obj;

        info.AddEnum("DestinationX", s._destinationX);
        info.AddEnum("DestinationOutput", s._destinationOutput);
        info.AddEnum("DestinationColumnSorting", s._destinationColumnSorting);
        info.AddEnum("DestinationRowSorting", s._destinationRowSorting);
      }

      protected virtual ExpandCyclingVariableColumnOptions SDeserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (o is null ? new ExpandCyclingVariableColumnOptions() : (ExpandCyclingVariableColumnOptions)o);

        s._destinationX = (DestinationXColumn)info.GetEnum("DestinationX", typeof(DestinationXColumn));
        s._destinationOutput = (OutputFormat)info.GetEnum("DestinationOutput", typeof(OutputFormat));
        s._destinationColumnSorting = (OutputSorting)info.GetEnum("DestinationColumnSorting", typeof(OutputSorting));
        s._destinationRowSorting = (OutputSorting)info.GetEnum("DestinationRowSorting", typeof(OutputSorting));

        return s;
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = SDeserialize(o, info, parent);
        return s;
      }
    }

    #endregion Version 0

    #endregion Serialization

    #region Construction

    public ExpandCyclingVariableColumnOptions()
    {
    }

    public ExpandCyclingVariableColumnOptions(ExpandCyclingVariableColumnOptions from)
    {
      CopyFrom(from);
    }

    public object Clone()
    {
      return new ExpandCyclingVariableColumnOptions(this);
    }

    public virtual bool CopyFrom(object obj)
    {
      if (ReferenceEquals(this, obj))
        return true;

      var from = obj as ExpandCyclingVariableColumnOptions;
      if (from is not null)
      {
        _destinationX = from._destinationX;
        _destinationOutput = from._destinationOutput;
        _destinationRowSorting = from._destinationRowSorting;
        _destinationColumnSorting = from._destinationColumnSorting;

        EhSelfChanged();

        return true;
      }
      return false;
    }

    #endregion Construction

    #region Properties

    /// <summary>Designates whether the destination x column is derived from the cycling variable column or from the (first) averaged column.</summary>
    public DestinationXColumn DestinationX { get { return _destinationX; } set { SetMemberEnumAndRaiseSelfChanged(ref _destinationX, value); } }

    /// <summary>Designates the order of the newly created columns of the dependent variables.</summary>
    public OutputFormat DestinationOutput { get { return _destinationOutput; } set { SetMemberEnumAndRaiseSelfChanged(ref _destinationOutput, value); } }

    /// <summary>If set, the destination columns will be sorted according to the first averaged column (if there is any).</summary>
    public OutputSorting DestinationColumnSorting { get { return _destinationColumnSorting; } set { SetMemberEnumAndRaiseSelfChanged(ref _destinationColumnSorting, value); } }

    /// <summary>If set, the destination rows will be sorted according to the destination x column.</summary>
    public OutputSorting DestinationRowSorting { get { return _destinationRowSorting; } set { SetMemberEnumAndRaiseSelfChanged(ref _destinationRowSorting, value); } }

    #endregion Properties
  }
}
