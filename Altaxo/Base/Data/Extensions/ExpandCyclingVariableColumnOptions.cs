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

    /// <summary>
    /// Defines which data becomes the destination x-column.
    /// </summary>
    public enum DestinationXColumn
    {
      /// <summary>
      /// Use the cycling-variable column as destination x-column.
      /// </summary>
      CyclingVariable,
      /// <summary>
      /// Use the first averaged column as destination x-column.
      /// </summary>
      FirstAveragedColumn
    }

    /// <summary>
    /// Defines how generated output columns are grouped.
    /// </summary>
    public enum OutputFormat
    {
      /// <summary>
      /// Place each generated output column into its own group.
      /// </summary>
      GroupOneColumn,
      /// <summary>
      /// Place all generated output columns into one group.
      /// </summary>
      GroupAllColumns,
    }

    /// <summary>
    /// Defines the sorting applied to generated rows or columns.
    /// </summary>
    public enum OutputSorting
    {
      /// <summary>
      /// Do not sort generated rows or columns.
      /// </summary>
      None,
      /// <summary>
      /// Sort generated rows or columns in ascending order.
      /// </summary>
      Ascending,
      /// <summary>
      /// Sort generated rows or columns in descending order.
      /// </summary>
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
      public virtual void Serialize(object o, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (ExpandCyclingVariableColumnOptions)o;

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

    /// <summary>
    /// Initializes a new instance of the <see cref="ExpandCyclingVariableColumnOptions"/> class.
    /// </summary>
    public ExpandCyclingVariableColumnOptions()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ExpandCyclingVariableColumnOptions"/> class by copying another instance.
    /// </summary>
    /// <param name="from">The source options.</param>
    public ExpandCyclingVariableColumnOptions(ExpandCyclingVariableColumnOptions from)
    {
      CopyFrom(from);
    }

    /// <inheritdoc />
    public object Clone()
    {
      return new ExpandCyclingVariableColumnOptions(this);
    }

        /// <summary>
    /// Copies the settings from another instance.
    /// </summary>
    /// <param name="obj">The instance to copy from.</param>
    /// <returns><c>true</c> if the settings were copied; otherwise, <c>false</c>.</returns>
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
