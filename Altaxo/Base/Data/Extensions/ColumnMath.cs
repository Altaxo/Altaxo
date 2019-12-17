#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
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
using System.Text;

namespace Altaxo.Data
{
  public static class ColumnMath
  {
    /// <summary>
    /// Determines if column c has monotonically increasing values.
    /// </summary>
    /// <param name="c">Column to test.</param>
    /// <param name="allowNaN">If true, NaN values will ignored. If false, the function will return false if there are NaN values present.</param>
    /// <returns>True if the values are monotonically increasing.</returns>
    public static bool IsMonotonicallyIncreasing(this DoubleColumn c, bool allowNaN)
    {
      if (c.Count == 0)
        return true;

      int i = 0;
      if (allowNaN)
      {
        for (i = 0; i < c.Count; i++)
          if (!double.IsNaN(c[i]))
            break;
      }
      if (i >= c.Count)
        return false;

      double start = c[i];
      if (double.IsNaN(start))
        return false;

      for (i++; i < c.Count; i++)
      {
        double next = c[i];
        if (double.IsNaN(next))
        {
          if (allowNaN)
            continue;
          else
            return false;
        }
        if (!(start <= next))
          return false;
      }
      return true;
    }

    /// <summary>
    /// Determines if column c has monotonically increasing values.
    /// </summary>
    /// <param name="c">Column to test.</param>
    /// <returns>True if the values are monotonically increasing.</returns>
    public static bool IsMonotonicallyIncreasing(this DateTimeColumn c)
    {
      if (c.Count == 0)
        return true;

      DateTime prev = c[0];
      for (int i = 0; i < c.Count; i++)
      {
        DateTime next = c[i];

        if (next >= prev)
        {
          prev = next;
          continue;
        }
        else
          return false;
      }
      return true;
    }

    #region Convert pairs of columns

    /// <summary>
    /// Converts a pair of data columns to an enumeration of pairs of <see cref="AltaxoVariant"/>s.
    /// </summary>
    /// <param name="columnTuple">The two columns to convert.</param>
    /// <returns>Enumeration of pairs of <see cref="AltaxoVariant"/>s.</returns>
    /// <example>
    /// <code>
    /// var e = (col["A"], col["B"]).ToEnumerable();
    /// </code>
    /// </example>
    public static IEnumerable<(AltaxoVariant X, AltaxoVariant Y)> ToEnumerable(this (DataColumn xCol, DataColumn yCol) columnTuple)
    {
      var (xCol, yCol) = columnTuple;
      var len = Math.Min(xCol.Count, yCol.Count);
      for (int i = 0; i < len; ++i)
      {
        yield return (xCol[i], yCol[i]);
      }
    }

    /// <summary>
    /// Converts a pair of data columns to an enumeration of pairs of <see cref="AltaxoVariant"/>s.
    /// </summary>
    /// <param name="columnTuple">The two columns to convert.</param>
    /// <param name="filterInvalidPairs">If set to <c>true</c>, only pairs will yielded, for which both values are valid.</param>
    /// <returns>Enumeration of pairs of <see cref="AltaxoVariant"/>s.</returns>
    /// <example>
    /// <code>
    /// var e = (col["A"], col["B"]).ToEnumerable(true);
    /// </code>
    /// </example>
    public static IEnumerable<(AltaxoVariant X, AltaxoVariant Y)> ToEnumerable(this (DataColumn xCol, DataColumn yCol) columnTuple, bool filterInvalidPairs)
    {
      var (xCol, yCol) = columnTuple;
      var len = Math.Min(xCol.Count, yCol.Count);
      for (int i = 0; i < len; ++i)
      {
        if (filterInvalidPairs && !(xCol.IsElementEmpty(i) || yCol.IsElementEmpty(i)))
          yield return (xCol[i], yCol[i]);
      }
    }

    /// <summary>
    /// Converts a pair of data columns to an enumeration of pairs of <see cref="Double"/>s.
    /// </summary>
    /// <param name="columnTuple">The two columns to convert.</param>
    /// <returns>Enumeration of pairs of <see cref="double"/>s.</returns>
    /// <example>
    /// <code>
    /// var e = (col["A"], col["B"]).ToEnumerableOfDouble();
    /// </code>
    /// </example>
    public static IEnumerable<(double X, double Y)> ToEnumerableOfDouble(this (DataColumn xCol, DataColumn yCol) columnTuple)
    {
      var (xCol, yCol) = columnTuple;
      var len = Math.Min(xCol.Count, yCol.Count);
      for (int i = 0; i < len; ++i)
      {
        yield return ((double)xCol[i], (double)yCol[i]);
      }
    }

    /// <summary>
    /// Converts a pair of data columns to an enumeration of pairs of <see cref="Double"/>s.
    /// </summary>
    /// <param name="columnTuple">The two columns to convert.</param>
    /// <param name="filterInvalidPairs">If set to <c>true</c>, only pairs will yielded, for which both values are valid.</param>
    /// <returns>Enumeration of pairs of <see cref="double"/>s.</returns>
    /// <example>
    /// <code>
    /// var e = (col["A"], col["B"]).ToEnumerableOfDouble(true);
    /// </code>
    /// </example>
    public static IEnumerable<(double X, double Y)> ToEnumerableOfDouble(this (DataColumn xCol, DataColumn yCol) columnTuple, bool filterInvalidPairs)
    {
      var (xCol, yCol) = columnTuple;
      var len = Math.Min(xCol.Count, yCol.Count);
      for (int i = 0; i < len; ++i)
      {
        var x = (double)xCol[i];
        var y = (double)yCol[i];
        if (filterInvalidPairs && !(double.IsNaN(x) || double.IsNaN(y)))
          yield return (x, y);
      }
    }

    #endregion Convert pairs of columns

    #region Convert triples of columns

    /// <summary>
    /// Converts a triple of data columns to an enumeration of pairs of <see cref="AltaxoVariant"/>s.
    /// </summary>
    /// <param name="columnTriple">The three columns to convert.</param>
    /// <returns>Enumeration of pairs of <see cref="AltaxoVariant"/>s.</returns>
    /// <example>
    /// <code>
    /// var e = (col["A"], col["B"], col["C"]).ToEnumerable();
    /// </code>
    /// </example>
    public static IEnumerable<(AltaxoVariant X, AltaxoVariant Y, AltaxoVariant Z)> ToEnumerable(this (DataColumn xCol, DataColumn yCol, DataColumn zCol) columnTriple)
    {
      var (xCol, yCol, zCol) = columnTriple;
      var len = Math.Min(Math.Min(xCol.Count, yCol.Count), zCol.Count);
      for (int i = 0; i < len; ++i)
      {
        yield return (xCol[i], yCol[i], zCol[i]);
      }
    }

    /// <summary>
    /// Converts a triple of data columns to an enumeration of pairs of <see cref="AltaxoVariant"/>s.
    /// </summary>
    /// <param name="columnTriple">The three columns to convert.</param>
    /// <param name="filterInvalidPairs">If set to <c>true</c>, only pairs will yielded, for which both values are valid.</param>
    /// <returns>Enumeration of pairs of <see cref="AltaxoVariant"/>s.</returns>
    /// <example>
    /// <code>
    /// var e = (col["A"], col["B"], col["C"]).ToEnumerable(true);
    /// </code>
    /// </example>
    public static IEnumerable<(AltaxoVariant X, AltaxoVariant Y, AltaxoVariant Z)> ToEnumerable(this (DataColumn xCol, DataColumn yCol, DataColumn zCol) columnTriple, bool filterInvalidPairs)
    {
      var (xCol, yCol, zCol) = columnTriple;
      var len = Math.Min(Math.Min(xCol.Count, yCol.Count), zCol.Count);
      for (int i = 0; i < len; ++i)
      {
        if (filterInvalidPairs && !(xCol.IsElementEmpty(i) || yCol.IsElementEmpty(i) || zCol.IsElementEmpty(i)))
          yield return (xCol[i], yCol[i], zCol[i]);
      }
    }

    /// <summary>
    /// Converts a triple of data columns to an enumeration of pairs of <see cref="Double"/>s.
    /// </summary>
    /// <param name="columnTriple">The three columns to convert.</param>
    /// <returns>Enumeration of pairs of <see cref="double"/>s.</returns>
    /// <example>
    /// <code>
    /// var e = (col["A"], col["B"], col["C"]).ToEnumerableOfDouble();
    /// </code>
    /// </example>
    public static IEnumerable<(double X, double Y, double Z)> ToEnumerableOfDouble(this (DataColumn xCol, DataColumn yCol, DataColumn zCol) columnTriple)
    {
      var (xCol, yCol, zCol) = columnTriple;
      var len = Math.Min(Math.Min(xCol.Count, yCol.Count), zCol.Count);
      for (int i = 0; i < len; ++i)
      {
        yield return ((double)xCol[i], (double)yCol[i], (double)zCol[i]);
      }
    }

    /// <summary>
    /// Converts a triple of data columns to an enumeration of pairs of <see cref="Double"/>s.
    /// </summary>
    /// <param name="columnTriple">The three columns to convert.</param>
    /// <param name="filterInvalidPairs">If set to <c>true</c>, only pairs will yielded, for which both values are valid.</param>
    /// <returns>Enumeration of pairs of <see cref="double"/>s.</returns>
    /// <example>
    /// <code>
    /// var e = (col["A"], col["B"], col["C"]).ToEnumerableOfDouble(true);
    /// </code>
    /// </example>
    public static IEnumerable<(double X, double Y, double Z)> ToEnumerableOfDouble(this (DataColumn xCol, DataColumn yCol, DataColumn zCol) columnTriple, bool filterInvalidPairs)
    {
      var (xCol, yCol, zCol) = columnTriple;
      var len = Math.Min(Math.Min(xCol.Count, yCol.Count), zCol.Count);
      for (int i = 0; i < len; ++i)
      {
        var x = (double)xCol[i];
        var y = (double)yCol[i];
        var z = (double)zCol[i];
        if (filterInvalidPairs && !(double.IsNaN(x) || double.IsNaN(y) || double.IsNaN(z)))
          yield return (x, y, z);
      }
    }

    #endregion

  }
}
