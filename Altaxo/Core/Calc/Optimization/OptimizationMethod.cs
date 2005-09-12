/*
 * OptimizationMethod.cs
 * 
 * Copyright (c) 2004, dnAnalytics Project. All rights reserved.
 * NB: Constraint class inspired by the optimization frame in the QuantLib library
*/

using System;
using Altaxo.Calc.LinearAlgebra;
using System.Text;

namespace Altaxo.Calc.Optimization
{
	///<summary>Base Class for Optimization Methods</summary>
	public abstract class OptimizationMethod : IFormattable {
		
		///<summary> Optimization ending critera </summary>
		public EndCriteria endCriteria_;
		///<summary> Optimization Cost Function </summary>
		public ICostFunction costFunction_;
		
		///<summary> Method Name </summary>
		public virtual string MethodName {
			get { return "Unspecified Optimization Method"; }
		}
		
		///<summary> Perform an evaluation of Cost Function value </summary>
		protected double FunctionEvaluation(DoubleVector x) {
			this.endCriteria_.functionEvaluationCounter++;
			return costFunction_.Value(x);
		}
		
		///<summary> Perform an evaluation of Cost Function gradient </summary>
		protected DoubleVector GradientEvaluation(DoubleVector x) {
			this.endCriteria_.gradientEvaluationCounter++;
			return costFunction_.Gradient(x);
		}
		
		///<summary> Perform an evaluation of Cost Function hessian </summary>
		protected DoubleMatrix HessianEvaluation(DoubleVector x) {
			this.endCriteria_.hessianEvaluationCounter++;
			return costFunction_.Hessian(x);
		}
		
		// --- IFormattable Interface ---
		
		///<summary>A string representation of this <c>OptimizationMethod</c>.</summary>
		public override string ToString() {
			return ToString(null,null);
		}

		///<summary>A string representation of this <c>OptimizationMethod</c>.</summary>
		///<param name="format">A format specification.</param>
		public string ToString(string format) {
			return ToString(format, null);
		}

		///<summary>A string representation of this <c>OptimizationMethod</c>.</summary>
		///<param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
		public string ToString(IFormatProvider formatProvider) {
			return ToString(null,formatProvider);
		}

		///<summary>A string representation of this <c>OptimizationMethod</c>.</summary>
		///<param name="format">A format specification.</param>
		///<param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
		public string ToString(string format, IFormatProvider formatProvider){
			return MethodName;
			/*
			StringBuilder sb = new StringBuilder();
			sb.Append("Current function value: " + this.iterationValues_[endCriteria_.iterationCounter].ToString() + "\n");
			sb.Append("Iterations: " + endCriteria_.iterationCounter.ToString() + "\n");
			sb.Append("Function evaluations: " + endCriteria_.functionEvaluationCounter.ToString() + "\n");
			sb.Append("Optimization criteria satisfied: " + endCriteria_.ToString());
			return sb.ToString(); */
		}
	}

}
