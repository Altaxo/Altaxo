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

namespace Altaxo.Data
{
  /// <summary>
  /// Contains options how to split a table that contains an independent variable with cycling values into
  /// another table, where this independent variable is unique and sorted.
  /// </summary>
  public class ConvertXYVToMatrixOptions
    :
    Main.SuspendableDocumentLeafNodeWithEventArgs,
    ICloneable
  {
    #region Enums



    public enum OutputAveraging
    {
      NoneIgnoreUseLastValue,
      NoneThrowException,
      AverageLinear,
    }

    public enum OutputNaming
    {
      /// <summary>Col and the index number appended.</summary>
      ColAndIndex,

      /// <summary>Use a format string.</summary>
      FormatString,
    }

    #endregion Enums

    #region Members

    protected OutputAveraging _outputAveraging;
    protected OutputNaming _outputNaming;
    protected string _outputColumnNameFormatString = "{0}";



    /// <summary>If set, the destination x-columns will be sorted according to the first averaged column (if there is any).</summary>
    protected SortDirection _destinationXColumnSorting = SortDirection.Ascending;
    private bool _useClusteringForX;
    private int? _numberOfClustersX;
    private bool _createStdDevX;

    /// <summary>If set, the destination y-columns will be sorted according to the first averaged column (if there is any).</summary>
    protected SortDirection _destinationYColumnSorting = SortDirection.Ascending;
    private bool _useClusteringForY;
    private int? _numberOfClustersY;
    private bool _createStdDevY;


    #endregion Members

    #region Serialization

    #region Version 0

    /// <summary>
    /// 2016-05-10 initial version.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ConvertXYVToMatrixOptions), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (ConvertXYVToMatrixOptions)obj;

        info.AddEnum("Averaging", s._outputAveraging);
        info.AddEnum("ColumnNaming", s._outputNaming);
        info.AddValue("ColumnNamingFormatting", s._outputColumnNameFormatString);

        info.AddEnum("DestinationXColumnSorting", s._destinationXColumnSorting);
        info.AddValue("UseClusteringForX", s._useClusteringForX);
        info.AddValue("NumberOfClustersX", s._numberOfClustersX);
        info.AddValue("CreateStdDevX", s._createStdDevX);

        info.AddEnum("DestinationYColumnSorting", s._destinationYColumnSorting);
        info.AddValue("UseClusteringForY", s._useClusteringForY);
        info.AddValue("NumberOfClustersY", s._numberOfClustersY);
        info.AddValue("CreateStdDevY", s._createStdDevY);

      }

      protected virtual ConvertXYVToMatrixOptions SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        var s = o as ConvertXYVToMatrixOptions ?? new ConvertXYVToMatrixOptions();

        s._outputAveraging = (OutputAveraging)info.GetEnum("Averaging", typeof(OutputAveraging));
        s._outputNaming = (OutputNaming)info.GetEnum("ColumnNaming", typeof(OutputNaming));
        s._outputColumnNameFormatString = info.GetString("ColumnNamingFormatting");

        s._destinationXColumnSorting = (SortDirection)info.GetEnum("DestinationXColumnSorting", typeof(SortDirection));
        s._useClusteringForX = info.GetBoolean("UseClusteringForX");
        s._numberOfClustersX = info.GetNullableInt32("NumberOfClustersX");
        s._createStdDevX = info.GetBoolean("CreateStdDevX");

        s._destinationYColumnSorting = (SortDirection)info.GetEnum("DestinationYColumnSorting", typeof(SortDirection));
        s._useClusteringForY = info.GetBoolean("UseClusteringForY");
        s._numberOfClustersY = info.GetNullableInt32("NumberOfClustersY");
        s._createStdDevY = info.GetBoolean("CreateStdDevY");

        return s;
      }

      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        var s = SDeserialize(o, info, parent);
        return s;
      }
    }

    #endregion Version 0

    #endregion Serialization

    #region Construction

    public ConvertXYVToMatrixOptions()
    {
    }

    public ConvertXYVToMatrixOptions(ConvertXYVToMatrixOptions from)
    {
      CopyFrom(from);
    }

    public object Clone()
    {
      return new ConvertXYVToMatrixOptions(this);
    }

    public virtual bool CopyFrom(object obj)
    {
      if (object.ReferenceEquals(this, obj))
        return true;

      var from = obj as ConvertXYVToMatrixOptions;
      if (null != from)
      {
        _outputAveraging = from._outputAveraging;
        _outputNaming = from._outputNaming;
        _outputColumnNameFormatString = from._outputColumnNameFormatString;

        _destinationXColumnSorting = from._destinationXColumnSorting;
        _useClusteringForX = from._useClusteringForX;
        _numberOfClustersX = from._numberOfClustersX;
        _destinationYColumnSorting = from._destinationYColumnSorting;
        _useClusteringForY = from._useClusteringForY;
        _numberOfClustersY = from._numberOfClustersY;

        EhSelfChanged();

        return true;
      }
      return false;
    }

    #endregion Construction

    #region Properties

    public OutputAveraging ValueAveraging { get => _outputAveraging; set => _outputAveraging = value; }
    public OutputNaming ColumnNaming { get => _outputNaming; set => _outputNaming = value; }
    public string ColumnNameFormatString { get => _outputColumnNameFormatString; set => _outputColumnNameFormatString = value ?? throw new ArgumentNullException(nameof(ColumnNameFormatString)); }

    /// <summary>If set, the destination columns will be either not sorted or sorted.</summary>
    public SortDirection DestinationXColumnSorting { get { return _destinationXColumnSorting; } set { SetMemberEnumAndRaiseSelfChanged(ref _destinationXColumnSorting, value); } }

    /// <summary>If set, the destination columns will be either not sorted or sorted.</summary>
    public SortDirection DestinationYColumnSorting { get { return _destinationYColumnSorting; } set { SetMemberEnumAndRaiseSelfChanged(ref _destinationYColumnSorting, value); } }


    public bool UseClusteringForX { get { return _useClusteringForX; } set { _useClusteringForX = value; } }
    public bool UseClusteringForY { get { return _useClusteringForY; } set { _useClusteringForY = value; } }

    public int? NumberOfClustersX { get { return _numberOfClustersX; } set { _numberOfClustersX = value; } }
    public int? NumberOfClustersY { get { return _numberOfClustersY; } set { _numberOfClustersY = value; } }

    public bool CreateStdDevX { get { return _createStdDevX; } set { _createStdDevX = value; } }
    public bool CreateStdDevY { get { return _createStdDevY; } set { _createStdDevY = value; } }


    #endregion Properties
  }
}
