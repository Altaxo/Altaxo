/*
 * ICostFunction.cs
 * 
 * Copyright (c) 2004, dnAnalytics Project. All rights reserved.
*/

using System;
using Altaxo.Calc.LinearAlgebra;

namespace Altaxo.Calc.Optimization
{

	///<summary>Base class for cost function declaration</summary>
  /// <remarks>
  /// <para>Copyright (c) 2003-2004, dnAnalytics Project. All rights reserved. See <a>http://www.dnAnalytics.net</a> for details.</para>
  /// <para>Adopted to Altaxo (c) 2005 Dr. Dirk Lellinger.</para>
  /// </remarks>
  public interface ICostFunction 
  {

		///<summary>Method to override to compute the cost function value of x</summary>
		double Value( DoubleVector x);
			
		///<summary>Method to override to calculate the grad_f, the first derivative of 
		/// the cost function with respect to x</summary>
		DoubleVector Gradient(DoubleVector x);

		///<summary>Method to override to calculate the hessian, the second derivative of 
		/// the cost function with respect to x</summary>
		DoubleMatrix Hessian(DoubleVector x);
		
		///<summary>Access the constraints for the given cost function </summary>
		ConstraintDefinition Constraint {
			get;
			set;
		}
	}
}
