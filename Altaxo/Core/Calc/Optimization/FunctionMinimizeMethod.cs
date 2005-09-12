/*
 * MinimizationMethod.cs
 * 
 * Copyright (c) 2004, dnAnalytics Project. All rights reserved.
 * NB: Constraint class inspired by the optimization frame in the QuantLib library
*/

using System;
using Altaxo.Calc.LinearAlgebra;
using System.Text;

namespace Altaxo.Calc.Optimization
{
	///<summary>Base Class for Function Minimization Optimization Methods</summary>
	public abstract class FunctionMinimizeMethod : OptimizationMethod {
		
		///<summary> Iteration Solutions </summary>
		protected DoubleVector[] iterationVectors_;
		///<summary> Iteration Values </summary>
		protected double[] iterationValues_;
		///<summary> Iteration Gradient Norms </summary>
		protected double[] iterationGradientNorms_;		
		
		///<summary> Get solution vectors for all iterations </summary>
		public DoubleVector[] IterationVectors {
			get { return this.iterationVectors_; }
		}
		///<summary> Get initial vector </summary>
		public DoubleVector InitialVector {
			get { return this.iterationVectors_[0]; }
		}
		///<summary> Get minimum vector </summary>
		public DoubleVector SolutionVector {
			get { return this.iterationVectors_[endCriteria_.iterationCounter]; }
		}
		
		///<summary> Get solution function values for all iterations </summary>
		public double[] IterationValues {
			get { return this.iterationValues_; }
		}
		///<summary> Get initial value </summary>
		public double InitialValue {
			get { return this.iterationValues_[0]; }
		}
		///<summary> Get value of minimum vector </summary>
		public double SolutionValue {
			get { return this.iterationValues_[endCriteria_.iterationCounter]; }
		}
		
		///<summary> Get gradient norms  for all iterations </summary>
		public double[] IterationGradientNorms {
			get { return this.iterationGradientNorms_; }
		}
		
		///<summary> Initialize the optimization method </summary>
		///<remarks> The use of this function is intended for testing/debugging purposes only </remarks>
		public virtual void InitializeMethod(DoubleVector initialvector){
			// Initialize optimization method
			this.iterationVectors_ = new DoubleVector[endCriteria_.maxIteration+1];
			this.iterationVectors_[0] = initialvector;
			this.iterationValues_ = new double[endCriteria_.maxIteration+1];
			this.iterationValues_[0] = costFunction_.Value(this.iterationVectors_[0]);
		}
		
		///<summary> Perform a single iteration of the optimization method </summary>
		///<remarks> The use of this function is intended for testing/debugging purposes only </remarks>
		public abstract void IterateMethod();
		
		///<summary> Minimize the given cost function </summary>
		public virtual void Minimize(DoubleVector initialvector) {
			endCriteria_.Reset();
			// Method specific implementation
			InitializeMethod(initialvector);
			
			// Iterate the optimization method
			do {
				endCriteria_.iterationCounter++;
				IterateMethod();
			} while (endCriteria_.CheckCriteria(iterationValues_[endCriteria_.iterationCounter-1], iterationValues_[endCriteria_.iterationCounter]));
		}
		
		///<summary> Function to check that criteria is still satisfied </summary>
		///<remarks> Returns true if criteria is still met and optimization continues </remarks>
		protected virtual bool CheckCriteria() {
			return !(
                endCriteria_.CheckIterations() ||
				endCriteria_.CheckFunctionEvaluations() ||
                endCriteria_.CheckStationaryPoint(iterationValues_[endCriteria_.iterationCounter-1], iterationValues_[endCriteria_.iterationCounter]) ||
                endCriteria_.CheckFunctionEpsilon(iterationValues_[endCriteria_.iterationCounter]) ||
                endCriteria_.CheckFunctionEpsilon(iterationValues_[endCriteria_.iterationCounter-1]) 
				);
		}
	}
}
