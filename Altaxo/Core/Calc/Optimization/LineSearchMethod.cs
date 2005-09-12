/*
 * LineSearchMethod.cs
 * 
 * Copyright (c) 2004, dnAnalytics Project. All rights reserved.
 * NB: Problem class inspired by the optimization frame in the QuantLib library
*/

using System;
using Altaxo.Calc.LinearAlgebra;

namespace Altaxo.Calc.Optimization
{
	///<summary>Base class for Line Search method declaration</summary>
	public abstract class LineSearchMethod : OptimizationMethod {
		///<summary> Minimize the given cost function </summary>
		public abstract DoubleVector Search(DoubleVector x, DoubleVector direction, double step);
	}
}
