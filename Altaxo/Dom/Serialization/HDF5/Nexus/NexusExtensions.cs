#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2024 Dr. Dirk Lellinger
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
using System.Collections.Generic;
using System.Linq;
using PureHDF;

namespace Altaxo.Serialization.HDF5.Nexus
{
  /// <summary>
  /// Extensions that make work with Nexus files easier.
  /// </summary>
  public static class NexusExtensions
  {
    /// <summary>
    /// Gets all objects with a given class name.
    /// </summary>
    /// <typeparam name="T">Type of HDF5 object that is expected.</typeparam>
    /// <param name="group">The HDF5 group whose children should be searched.</param>
    /// <param name="className">The class name.</param>
    /// <returns>All objects of type T with the given class name.</returns>
    public static IEnumerable<T> GetAllObjectsWithClassName<T>(this IH5Group group, string className) where T : class, IH5Object
    {
      foreach (var child in group.Children())
      {
        var classAttribute = child.Attributes().FirstOrDefault(a => a.Name == "NX_class");
        if (classAttribute is not null && classAttribute.Read<string>() == className && child is T)
          yield return (T)child;
      }
    }

    /// <summary>
    /// Gets the first objects with a given class name.
    /// </summary>
    /// <typeparam name="T">Type of HDF5 object that is expected.</typeparam>
    /// <param name="group">The HDF5 group whose children should be searched.</param>
    /// <param name="className">The class name.</param>
    /// <returns>The first object of type T with the given class name.</returns>
    public static T? GetFirstObjectWithClassName<T>(this IH5Group group, string className) where T : class, IH5Object
    {
      return GetAllObjectsWithClassName<T>(group, className).FirstOrDefault();
    }

    /// <summary>
    /// Gets all objects of a given type.
    /// </summary>
    /// <typeparam name="T">Type of HDF5 object(s) that are expected</typeparam>
    /// <param name="group">The HDF5 group whose children should be searched.</param>
    /// <returns>All objects of type T as children of group.</returns>
    public static IEnumerable<T> GetAllObjectsOfType<T>(this IH5Group group) where T : class, IH5Object
    {
      foreach (var child in group.Children())
      {
        if (child is T)
          yield return (T)child;
      }
    }

    /// <summary>
    /// Gets the child object that is named with the given name.
    /// </summary>
    /// <typeparam name="T">Type of HDF5 object that is expected</typeparam>
    /// <param name="group">The group.</param>
    /// <param name="name">The name that is searched.</param>
    /// <returns>The object of type T with the given name, or null if no such object was found.</returns>
    public static T? GetChildObjectNamed<T>(this IH5Group group, string name) where T : class, IH5Object
    {
      return group.Children().FirstOrDefault(a => a.Name == name) as T;
    }

    /// <summary>
    /// Reads data from a <see cref="IH5Dataset"/> independent of the data type in the HDF5 file as double array.
    /// </summary>
    /// <param name="dataSet">The data set to read from.</param>
    /// <returns>The data.</returns>
    public static double[] ReadData(this IH5Dataset dataSet)
    {
      double[]? result = null;
      if (dataSet.Type.Class == H5DataTypeClass.FloatingPoint)
      {
        if (dataSet.Type.Size == 8)
        {
          result = dataSet.Read<double[]>(); // multidimensional arrays only possible when using .NET6 or above
        }
        else if (dataSet.Type.Size == 4)
        {
          var dataArrays = dataSet.Read<float[]>(); // multidimensional arrays only possible when using .NET6 or above
          result = new double[dataArrays.Length];
          Array.Copy(dataArrays, result, dataArrays.Length);
        }
      }
      else if (dataSet.Type.Class == H5DataTypeClass.FixedPoint)
      {
        if (dataSet.Type.Size == 2)
        {
          var dataArrays = dataSet.Read<short[]>(); // multidimensional arrays only possible when using .NET6 or above
          result = new double[dataArrays.Length];
          Array.Copy(dataArrays, result, dataArrays.Length);
        }
        else if (dataSet.Type.Size == 4)
        {
          var dataArrays = dataSet.Read<int[]>(); // multidimensional arrays only possible when using .NET6 or above
          result = new double[dataArrays.Length];
          Array.Copy(dataArrays, result, dataArrays.Length);
        }
        else if (dataSet.Type.Size == 8)
        {
          var dataArrays = dataSet.Read<long[]>(); // multidimensional arrays only possible when using .NET6 or above
          result = new double[dataArrays.Length];
          Array.Copy(dataArrays, result, dataArrays.Length);
        }
      }

      if (result is null)
      {
        throw new NotImplementedException();
      }
      return result ?? throw new NotImplementedException($"Conversion from {dataSet.Type.Class}, SizeOfElement={dataSet.Type.Size} is not yet implemented.");
    }

    /// <summary>
    /// Try to get an attribute value as string. If this fails, null is returned.
    /// </summary>
    /// <param name="obj">The HDF5 object that has the attribute.</param>
    /// <param name="name">The attribute's name.</param>
    /// <returns>Either the attributes value, if it is a string, or null, if the attribute don't exist.
    /// Note that if the attribute exists but its value is not a string, an exception is thrown.</returns>
    public static string? TryGetAttributeValueAsString(this IH5Object obj, string name)
    {
      return obj.AttributeExists(name) ? obj.Attribute(name).Read<string>() : null;
    }
  }
}
