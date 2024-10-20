﻿#region Copyright

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
using Altaxo.Calc.LinearAlgebra;

namespace Altaxo.Calc.Regression.Multivariate
{
  /// <summary>
  /// Provides a strategie for grouping the data (spectra etc.) according to their corresponding
  /// calibration values (concentration etc).
  /// </summary>
  public interface ICrossValidationGroupingStrategy
  {
    /// <summary>
    /// Divides observations into groups according to the y-values (calibration values) in argument <c>matrixY</c>.
    /// </summary>
    /// <param name="matrixY">Contains the y-values. Each observation corresponds to one or more y-values,
    /// for instance concentrations, size etc.). The matrix consists of many observations (each row is one observation). Each observation
    /// corresponds to one or more y-values, which are the columns of the matrix.
    /// </param>
    /// <returns>An array of integer arrays. Each element of the main array is one group. The elements of each
    /// subarray are the indices of the observations (==row numbers in matrixY), that are grouped together.</returns>
    int[][] Group(IROMatrix<double> matrixY);
  }

  /// <summary>
  /// Represents the no-grouping strategy. Thus a call to <see cref="ICrossValidationGroupingStrategy.Group(IROMatrix{double})"/> will result in
  /// a <see cref="NotImplementedException"/>.
  /// </summary>
  public record CrossValidationGroupingStrategyNone : ICrossValidationGroupingStrategy
  {
    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(CrossValidationGroupingStrategyNone), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        return new CrossValidationGroupingStrategyNone();
      }
    }
    #endregion


    public int[][] Group(IROMatrix<double> matrixY)
    {
      throw new NotImplementedException();
    }
  }

  /// <summary>
  /// This strategy groups together similar observations, i.e. observations that have exactly the same target values.
  /// </summary>
  public record CrossValidationGroupingStrategyExcludeGroupsOfSimilarMeasurements : ICrossValidationGroupingStrategy, Main.IImmutable
  {
    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(CrossValidationGroupingStrategyExcludeGroupsOfSimilarMeasurements), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        return new CrossValidationGroupingStrategyExcludeGroupsOfSimilarMeasurements();
      }
    }
    #endregion


    /// <summary>
    /// <see cref="ICrossValidationGroupingStrategy.Group" />
    /// </summary>
    /// <param name="Y"></param>
    /// <returns></returns>
    public int[][] Group(IROMatrix<double> Y)
    {
      var groups = new List<List<int>>();

      // add the first y-row to the first group
      var newcoll = new List<int>
      {
        0
      };
      groups.Add(newcoll);
      // now test all other rows of the y-matrix against the existing groups
      for (int i = 1; i < Y.RowCount; i++)
      {
        bool bNewGroup = true;
        for (int gr = 0; gr < groups.Count; gr++)
        {
          int refrow = groups[gr][0];
          bool match = true;
          for (int j = 0; j < Y.ColumnCount; j++)
          {
            if (Y[i, j] != Y[refrow, j])
            {
              match = false;
              break;
            }
          }

          if (match)
          {
            bNewGroup = false;
            groups[gr].Add(i);
            break;
          }
        }
        if (bNewGroup)
        {
          newcoll = new List<int>
          {
            i
          };
          groups.Add(newcoll);
        }
      }

      int[][] result = new int[groups.Count][];
      for (int i = 0; i < result.Length; i++)
        result[i] = groups[i].ToArray();
      return result;
    }
  }

  /// <summary>
  /// This strategy groups the observations into two groups. It try to part observations with the same target values
  /// equally into the one group and the other group.
  /// </summary>
  public record CrossValidationGroupingStrategyExcludeHalfObservations : ICrossValidationGroupingStrategy, Main.IImmutable
  {
    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(CrossValidationGroupingStrategyExcludeHalfObservations), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        return new CrossValidationGroupingStrategyExcludeHalfObservations();
      }
    }
    #endregion

    public int[][] Group(IROMatrix<double> Y)
    {
      var groups = new List<int>[2];
      for (int i = 0; i < 2; i++)
        groups[i] = new List<int>();

      int[][] similarGroups = new CrossValidationGroupingStrategyExcludeGroupsOfSimilarMeasurements().Group(Y);

      int destinationGroupNumber = 0;
      for (int g = 0; g < similarGroups.Length; g++)
      {
        for (int i = 0; i < similarGroups[g].Length; i++)
        {
          groups[destinationGroupNumber % 2].Add(similarGroups[g][i]);
          destinationGroupNumber++;
        }
      }

      int[][] result = new int[2][];
      for (int i = 0; i < result.Length; i++)
        result[i] = groups[i].ToArray();
      return result;
    }
  }

  /// <summary>
  /// Stragegy that groups each observation in an own group.
  /// </summary>
  public record CrossValidationGroupingStrategyExcludeSingleMeasurements : ICrossValidationGroupingStrategy, Main.IImmutable
  {
    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(CrossValidationGroupingStrategyExcludeSingleMeasurements), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        return new CrossValidationGroupingStrategyExcludeSingleMeasurements();
      }
    }
    #endregion


    public int[][] Group(IROMatrix<double> Y)
    {
      int[][] groups = new int[Y.RowCount][];

      for (int i = 0; i < Y.RowCount; i++)
      {
        int[] newcoll = new int[1];
        newcoll[0] = i;
        groups[i] = newcoll;
      }

      return groups;
    }
  }
}
