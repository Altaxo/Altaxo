/*
 * NewtonRaphsonLineSearch.cs
 * 
 * Copyright (c) 2004, dnAnalytics Project. All rights reserved.
 * NB: Problem class inspired by the optimization frame in the QuantLib library
*/

using System;
using Altaxo.Calc.LinearAlgebra;

namespace Altaxo.Calc.Optimization
{
	///<summary>Newton-Raphson Line Search Method</summary>
  /// <remarks>
  /// <para>Copyright (c) 2003-2004, dnAnalytics Project. All rights reserved. See <a>http://www.dnAnalytics.net</a> for details.</para>
  /// <para>Adopted to Altaxo (c) 2005 Dr. Dirk Lellinger.</para>
  /// </remarks>
  public class NewtonRaphsonLineSearch:LineSearchMethod 
  {
	
		///<summary>Constructor for Newton-Raphson Line Search</summary>
		public NewtonRaphsonLineSearch(CostFunction costfunction)
			: this(costfunction, new EndCriteria()) {}
		public NewtonRaphsonLineSearch(CostFunction costfunction, EndCriteria endcriteria)
			: this(costfunction, new EndCriteria(), 50, 1e-8) {}
		public NewtonRaphsonLineSearch(CostFunction costfunction, EndCriteria endcriteria, 
			int maxiteration, double tolerance) 
		{
			this.costFunction_=costfunction;
			this.endCriteria_=endcriteria;
			this.maxIteration=maxiteration;
			this.tolerance=tolerance;
		}
	
		private int maxIteration;
		private double tolerance;
	
		///<summary> Method Name </summary>
		public override string MethodName {
			get { return "Newton-Raphson Line Search Method"; }
		}
		
		///<summary> Minimize the given cost function </summary>
		public override DoubleVector Search(DoubleVector x, DoubleVector d, double stp) {
			DoubleVector ret = new DoubleVector(x);
			double j=0;
			double delta_d = d.GetDotProduct(d);
			double alpha;
			do {
				alpha = -GradientEvaluation(ret).GetDotProduct(d)/
					d.GetDotProduct(HessianEvaluation(ret)*d);
				ret = ret + alpha*d;
				j++;
			} while ((j<maxIteration) && (alpha*alpha*delta_d > tolerance*tolerance));
			return ret;
		}
	}
}
