/*
 * SecantLineSearch.cs
 * 
 * Copyright (c) 2004, dnAnalytics Project. All rights reserved.
 * NB: Problem class inspired by the optimization frame in the QuantLib library
*/

using System;
using Altaxo.Calc.LinearAlgebra;

namespace Altaxo.Calc.Optimization
{
	///<summary>Secant Line Search Method</summary>
	public class SecantLineSearch : LineSearchMethod {
	
		///<summary>Constructor for Secant Line Search</summary>
		public SecantLineSearch(CostFunction costfunction)
			: this(costfunction, new EndCriteria()) {}
		public SecantLineSearch(CostFunction costfunction, EndCriteria endcriteria) 
			: this(costfunction, endcriteria, 1) {}
		public SecantLineSearch(CostFunction costfunction, EndCriteria endcriteria, double sigma_0)
			: this(costfunction, endcriteria, sigma_0, 50, 1e-8) {}
		public SecantLineSearch(CostFunction costfunction, EndCriteria endcriteria, double sigma_0, 
			int maxiteration, double tolerance) 
		{
			this.costFunction_=costfunction;
			this.endCriteria_=endcriteria;
			this.sigma_0 = sigma_0;
			this.maxIteration=maxiteration;
			this.tolerance=tolerance;
		}
	
		///<summary>Value of sigma for first step of each Secant method minimization</summary>
		public double sigma_0;
		
		private int maxIteration;
		private double tolerance;
	
		///<summary> Method Name </summary>
		public override string MethodName {
			get { return "Secant Line Search Method"; }
		}
		
		///<summary> Minimize the given cost function </summary>
		public override DoubleVector Search(DoubleVector x, DoubleVector d, double step) {
			DoubleVector ret = new DoubleVector(x);
			double j=0;
			double eta;
		
			double delta_d = d.GetDotProduct(d);
			double alpha = -sigma_0;
			double eta_prev = d.GetDotProduct(GradientEvaluation(ret+sigma_0*d));
			do {
				eta = d.GetDotProduct(GradientEvaluation(ret));
				alpha = alpha*(eta/(eta_prev-eta));
				ret = ret + alpha*d;
				eta_prev = eta;
				j++;
			} while ((j<maxIteration) && (alpha*alpha*delta_d > tolerance*tolerance));
			return ret;
		}
	}
}
