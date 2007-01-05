#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2007 Dr. Dirk Lellinger
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
#endregion

using System;
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
    int[][] Group(IROMatrix matrixY);
  }


  /// <summary>
  /// This strategy groups together similar observations, i.e. observations that have exactly the same y-values.
  /// </summary>
  public class ExcludeGroupsGroupingStrategy : ICrossValidationGroupingStrategy
  {
    /// <summary>
    /// <see cref="ICrossValidationGroupingStrategy.Group" />
    /// </summary>
    /// <param name="Y"></param>
    /// <returns></returns>
    public int[][] Group(IROMatrix Y)
    {
      System.Collections.ArrayList groups = new System.Collections.ArrayList();

      // add the first y-row to the first group
      System.Collections.ArrayList newcoll = new System.Collections.ArrayList();
      newcoll.Add(0);
      groups.Add(newcoll);
      // now test all other rows of the y-matrix against the existing groups
      for(int i=1;i<Y.Rows;i++)
      {
        bool bNewGroup=true;
        for(int gr=0;gr<groups.Count;gr++)
        {
          int refrow = (int)(((System.Collections.ArrayList)groups[gr])[0]);
          bool match = true;
          for(int j=0;j<Y.Columns;j++)
          {
            if(Y[i,j]!= Y[refrow,j])
            {
              match=false;
              break;
            }
          }
            
          if(match)
          {
            bNewGroup=false;
            ((System.Collections.ArrayList)groups[gr]).Add(i);
            break;
          }
        }
        if(bNewGroup)
        {
          newcoll = new System.Collections.ArrayList();
          newcoll.Add(i);
          groups.Add(newcoll);
        }
      }

      int[][] result = new int[groups.Count][];
      for(int i=0;i<result.Length;i++)
        result[i] = (int[])((System.Collections.ArrayList)groups[i]).ToArray(typeof(int));
      return result;
    }
  }



  /// <summary>
  /// This strategy groups the observations into two groups. It try to part observations with the same y-values
  /// equally into the one group and the other group.
  /// </summary>
  public class ExcludeHalfObservationsGroupingStrategy : ICrossValidationGroupingStrategy
  {
    public int[][] Group(IROMatrix Y)
    {
      System.Collections.ArrayList[] groups = new System.Collections.ArrayList[2];
      for(int i=0;i<2;i++)
        groups[i] = new System.Collections.ArrayList();

      int[][] similarGroups = new ExcludeGroupsGroupingStrategy().Group(Y);

      int destinationGroupNumber=0;
      for(int g=0;g<similarGroups.Length;g++)
      {
        for(int i=0;i<similarGroups[g].Length;i++)
        {
          groups[destinationGroupNumber%2].Add(similarGroups[g][i]);
          destinationGroupNumber++;
        }
      }

      int[][] result = new int[2][];
      for(int i=0;i<result.Length;i++)
        result[i] = (int[])groups[i].ToArray(typeof(int));
      return result;
    }
  }



  /// <summary>
  /// Stragegy that groups not at all, so each observation appears in an own group.
  /// </summary>
  public class ExcludeSingleMeasurementsGroupingStrategy : ICrossValidationGroupingStrategy
  {
    public int[][] Group(IROMatrix Y)
    {
      int[][] groups = new int[Y.Rows][];

      for(int i=0;i<Y.Rows;i++)
      {
        int[] newcoll = new int[1];
        newcoll[0]=i;
        groups[i]=newcoll;
      }

      return groups;
    }
  }


}
