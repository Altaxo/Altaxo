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
  public class DecomposeByColumnContentOptions
    :
    Main.SuspendableDocumentLeafNodeWithEventArgs,
    ICloneable
  {
    #region Enums

    /// <summary>
    /// Defines how the decomposed output columns are grouped.
    /// </summary>
    public enum OutputFormat
    {
      /// <summary>
      /// Place each output column into its own group.
      /// </summary>
      GroupOneColumn,
      /// <summary>
      /// Place all output columns into one group.
      /// </summary>
      GroupAllColumns,
    }

    /// <summary>
    /// Defines the sorting applied to generated columns.
    /// </summary>
    public enum OutputSorting
    {
      /// <summary>
      /// Do not sort the generated columns.
      /// </summary>
      None,
      /// <summary>
      /// Sort the generated columns in ascending order.
      /// </summary>
      Ascending,
      /// <summary>
      /// Sort the generated columns in descending order.
      /// </summary>
      Descending
    }

    #endregion Enums

    #region Members

    /// <summary>Designates the order of the newly created columns of the dependent variables.</summary>
    protected OutputFormat _destinationOutput;

    /// <summary>If set, the destination columns will be sorted according to the first averaged column (if there is any).</summary>
    protected OutputSorting _destinationColumnSorting;

    #endregion Members

    #region Serialization

    #region Version 0

    /// <summary>
    /// 2016-05-10 initial version.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(DecomposeByColumnContentOptions), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (DecomposeByColumnContentOptions)obj;

        info.AddEnum("DestinationOutput", s._destinationOutput);
        info.AddEnum("DestinationColumnSorting", s._destinationColumnSorting);
      }

      protected virtual DecomposeByColumnContentOptions SDeserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = o as DecomposeByColumnContentOptions ?? new DecomposeByColumnContentOptions();

        s._destinationOutput = (OutputFormat)info.GetEnum("DestinationOutput", typeof(OutputFormat));
        s._destinationColumnSorting = (OutputSorting)info.GetEnum("DestinationColumnSorting", typeof(OutputSorting));

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
    /// Initializes a new instance of the <see cref="DecomposeByColumnContentOptions"/> class.
    /// </summary>
    public DecomposeByColumnContentOptions()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DecomposeByColumnContentOptions"/> class by copying another instance.
    /// </summary>
    /// <param name="from">The source options.</param>
    public DecomposeByColumnContentOptions(DecomposeByColumnContentOptions from)
    {
      CopyFrom(from);
    }

    /// <inheritdoc />
    public object Clone()
    {
      return new DecomposeByColumnContentOptions(this);
    }

    /// <inheritdoc />
    public virtual bool CopyFrom(object obj)
    {
      if (ReferenceEquals(this, obj))
        return true;

      var from = obj as DecomposeByColumnContentOptions;
      if (from is not null)
      {
        _destinationOutput = from._destinationOutput;
        _destinationColumnSorting = from._destinationColumnSorting;

        EhSelfChanged();

        return true;
      }
      return false;
    }

    #endregion Construction

    #region Properties

    /// <summary>Designates the order of the newly created columns of the dependent variables.</summary>
    public OutputFormat DestinationOutput { get { return _destinationOutput; } set { SetMemberEnumAndRaiseSelfChanged(ref _destinationOutput, value); } }

    /// <summary>If set, the destination columns will be either not sorted or sorted.</summary>
    public OutputSorting DestinationColumnSorting { get { return _destinationColumnSorting; } set { SetMemberEnumAndRaiseSelfChanged(ref _destinationColumnSorting, value); } }

    #endregion Properties
  }
}
