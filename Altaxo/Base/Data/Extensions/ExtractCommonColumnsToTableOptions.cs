#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2022 Dr. Dirk Lellinger
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
using System.Collections.Immutable;
using System.Linq;
using Altaxo.Calc.Interpolation;

namespace Altaxo.Data
{
  public record ExtractCommonColumnsToTableOptions
  {
    /// <summary>
    /// If this value is not empty, it defines the name of the x-column in the destination table.
    /// </summary>
    public string UserDefinedNameForXColumn { get; init; } = string.Empty;

    /// <summary>
    /// Gets the user defined names for y columns. If there are less user defined names than number of y-columns,
    /// the original column name is used for naming.
    /// </summary>
    public ImmutableArray<string> UserDefinedNamesForYColumns { get; init; } = ImmutableArray<string>.Empty;

    /// <summary>
    /// If true, the x-values of the x-columns of all tables are intersected.
    /// If false, the union of the x-values of the x-columns of all tables is used.
    /// </summary>
    public bool IntersectXValues { get; init; } = true;

    /// <summary>
    /// Gets a value indicating whether to place multiple y columns adjacent to each other in the destination table.
    /// </summary>
    /// <remarks>
    /// If true, and there are more than one y-columns, they are placed in the destination table adjacent to each other.
    /// If false, and there are more than one y-columns, the columns are grouped by their name.
    /// </remarks>
    public bool PlaceMultipleYColumnsAdjacentInDestinationTable { get; init; } = false;

    /// <summary>
    /// If true, a property column 'SourceTableName' is created in the destination table,
    /// and filled with the name of the originating table.
    /// </summary>
    public bool CreatePropertyColumnWithSourceTableName { get; init; } = true;

    /// <summary>
    /// If true, the column properties of the columns of the orignal table are copied to the destination table (only if they are not empty).
    /// </summary>
    public bool CopyColumnProperties { get; init; } = false;

    public bool UseResampling => Interpolation is not null;

    /// <summary>
    /// If resampling of the data is neccessary, this gets the interpolation function (otherwise, it is set to null).
    /// </summary>
    /// <value>
    /// The interpolation function.
    /// </value>
    public IInterpolationFunctionOptions? Interpolation { get; init; }

    /// <summary>
    /// Gets the interpolation interval.
    /// </summary>
    public double InterpolationInterval { get; init; } = 1;

    /// <summary>
    /// Gets the interpolation start specified by the user. If the value is null, the range start of the ranges of x-values common to all x-columns is used.
    /// </summary>
    public double? UserSpecifiedInterpolationStart { get; init; }

    /// <summary>
    /// Gets the interpolation end specified by the user. If the value is null, the range end of the ranges of x-values common to all x-columns is used.
    /// </summary>
    public double? UserSpecifiedInterpolationEnd { get; init; }


    #region Serialization

    /// <summary>
    /// 2022-10-12 Initial version
    /// </summary>
    /// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ExtractCommonColumnsToTableOptions), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (ExtractCommonColumnsToTableOptions)obj;
        info.AddValue("UserDefinedNameForXColumn", s.UserDefinedNameForXColumn);
        info.AddArray("UserDefinedNamesForYColumns", s.UserDefinedNamesForYColumns, s.UserDefinedNamesForYColumns.Length);
        info.AddValue("IntersectXValues", s.IntersectXValues);
        info.AddValue("PlaceMultipleYColumnsAdjacentInDestinationTable", s.PlaceMultipleYColumnsAdjacentInDestinationTable);
        info.AddValue("CreatePropertyColumnWithSourceTableName", s.CreatePropertyColumnWithSourceTableName);
        info.AddValue("CopyColumnProperties", s.CopyColumnProperties);
        info.AddValue("UseResampling", s.UseResampling);

        if(s.UseResampling)
        {
          info.AddValue("InterpolationFunction", s.Interpolation);
          info.AddValue("InterpolationInterval", s.InterpolationInterval);
          info.AddValue("UserInterpolationStart", s.UserSpecifiedInterpolationStart);
          info.AddValue("UserInterpolationEnd", s.UserSpecifiedInterpolationEnd);
        }
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var userDefinedXColumn = info.GetString("UserDefinedNameForXColumn");
        var userDefinedYColumns = info.GetArrayOfStrings("UserDefinedNamesForYColumns");

        var intersectXValues = info.GetBoolean("IntersectXValues");
        var placeMultipleYColumnsAdjacentInDestinationTable = info.GetBoolean("PlaceMultipleYColumnsAdjacentInDestinationTable");
        var createPropertyColumnWithSourceTableName = info.GetBoolean("CreatePropertyColumnWithSourceTableName");
        var copyColumnProperties = info.GetBoolean("CopyColumnProperties");
        var useResampling = info.GetBoolean("UseResampling");

        IInterpolationFunctionOptions? interpolationFunction = null;
        double interpolationInterval = 1;
        double? interpolationStart = null, interpolationEnd = null;
        if (useResampling)
        {
          interpolationFunction = info.GetValue<IInterpolationFunctionOptions>("InterpolationFunction", null);
          interpolationInterval = info.GetDouble("InterpolationInterval");
          interpolationStart = info.GetNullableDouble("UserInterpolationStart");
          interpolationEnd = info.GetNullableDouble("UserInterpolationEnd");
        }

        return o is null ? new ExtractCommonColumnsToTableOptions
        {
          UserDefinedNameForXColumn = userDefinedXColumn,
          UserDefinedNamesForYColumns = userDefinedYColumns.Select(s => s ?? string.Empty).ToImmutableArray(),
          IntersectXValues = intersectXValues,
          PlaceMultipleYColumnsAdjacentInDestinationTable = placeMultipleYColumnsAdjacentInDestinationTable,
          CreatePropertyColumnWithSourceTableName = createPropertyColumnWithSourceTableName,
          CopyColumnProperties = copyColumnProperties,
          Interpolation = interpolationFunction,
          InterpolationInterval = interpolationInterval,
          UserSpecifiedInterpolationStart = interpolationStart,
          UserSpecifiedInterpolationEnd = interpolationEnd,
        } :
          ((ExtractCommonColumnsToTableOptions)o) with
          {
            UserDefinedNameForXColumn = userDefinedXColumn,
            UserDefinedNamesForYColumns = userDefinedYColumns.Select(s => s ?? string.Empty).ToImmutableArray(),
            IntersectXValues = intersectXValues,
            PlaceMultipleYColumnsAdjacentInDestinationTable = placeMultipleYColumnsAdjacentInDestinationTable,
            CreatePropertyColumnWithSourceTableName = createPropertyColumnWithSourceTableName,
            CopyColumnProperties = copyColumnProperties,
            Interpolation = interpolationFunction,
            InterpolationInterval = interpolationInterval,
            UserSpecifiedInterpolationStart = interpolationStart,
            UserSpecifiedInterpolationEnd = interpolationEnd,
          };
      }
    }

    #endregion

  }
}
