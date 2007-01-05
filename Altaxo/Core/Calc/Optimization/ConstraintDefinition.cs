#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Copyright (c) 2003-2004, dnAnalytics. All rights reserved.
//
//    modified for Altaxo:  a data processing and data plotting program
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

/*
 * ConstraintDefinition.cs
 * 
 * Copyright (c) 2004, dnAnalytics Project. All rights reserved.
 * NB: Constraint class inspired by the optimization frame in the QuantLib library
*/

using System;
using Altaxo.Calc.LinearAlgebra;

namespace Altaxo.Calc.Optimization
{
  ///<summary>Interface for constraint definitions</summary>
  /// <remarks>
  /// <para>Copyright (c) 2003-2004, dnAnalytics Project. All rights reserved. See <a>http://www.dnAnalytics.net</a> for details.</para>
  /// <para>Adopted to Altaxo (c) 2005 Dr. Dirk Lellinger.</para>
  /// </remarks>
  public interface IConstraintDefinition 
  {
    ///<summary>Test whether constraint is satisfied</summary>
    ///<param name="solution"><c>DoubleVector</c> with solution to test against constraints</param>
    ///<returns>Returns true if solution satisfies constraints</returns>
    bool Check(DoubleVector solution);
    
    ///<summary>
    /// Find a beta so that a new solution = old solution + beta * direction satifies the constraint
    ///</summary>
    ///<param name="solution"><c>DoubleVector</c> with current solution vector</param>
    ///<param name="direction"><c>DoubleVector</c> with direction to add to current solution vector</param>
    ///<param name="beta">Scale factor representing the size of the step in the direction of 'direction' vector</param>
    double Update(DoubleVector solution, DoubleVector direction, double beta);
  }


  ///<summary>Base class for constraint definitions</summary>
  /// <remarks>
  /// <para>Copyright (c) 2003-2004, dnAnalytics Project. All rights reserved. See <a>http://www.dnAnalytics.net</a> for details.</para>
  /// <para>Adopted to Altaxo (c) 2005 Dr. Dirk Lellinger.</para>
  /// </remarks>
  public abstract class ConstraintDefinition : IConstraintDefinition 
  {
    public abstract bool Check(DoubleVector solution);
    
    public double Update(DoubleVector solution, DoubleVector direction, double beta) 
    {
      
      DoubleVector newSolution;
      double newbeta = beta;
      for (int i=0; i<200; i++)
      {
        newSolution = solution + newbeta * direction;
        if (Check(newSolution))
        {
          return newbeta;
        }
        newbeta *= 0.5;
      }
      throw new OptimizationException("Beta couldn't be found to satisfy constraint"); 
    }
  }
  
  ///<summary>Class defining no constraints</summary>
  /// <remarks>
  /// <para>Copyright (c) 2003-2004, dnAnalytics Project. All rights reserved. See <a>http://www.dnAnalytics.net</a> for details.</para>
  /// <para>Adopted to Altaxo (c) 2005 Dr. Dirk Lellinger.</para>
  /// </remarks>
  public class NoConstraint : ConstraintDefinition 
  {
    
    public override bool Check(DoubleVector solution) 
    {
      return true;
    }
  }
  
}
