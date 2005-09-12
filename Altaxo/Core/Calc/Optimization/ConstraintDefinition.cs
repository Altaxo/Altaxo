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
	public interface IConstraintDefinition {
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
	public abstract class ConstraintDefinition : IConstraintDefinition {
		public abstract bool Check(DoubleVector solution);
		
		public double Update(DoubleVector solution, DoubleVector direction, double beta) {
			
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
	public class NoConstraint : ConstraintDefinition {
		
		public override bool Check(DoubleVector solution) {
			return true;
		}
	}
	
}
