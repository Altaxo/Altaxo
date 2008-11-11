using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Altaxo.Calc;
using Altaxo.Calc.LinearAlgebra;

namespace Altaxo.Calc.Probability
{
	public static class Bandwidths
	{
		public static double Nrd0(IROVector x)
{
    if(x.Length < 2) throw new ArgumentException("need at least 2 data points");

    double hi = Statistics.StandardDeviation(x);
		double lo = Math.Min(hi, Statistics.InterQuartileRange(x)/1.34);  // qnorm(.75) - qnorm(.25) = 1.34898
		if (lo.IsNaN())
		{
			lo = hi;
			if (lo.IsNaN())
			{
				lo = Math.Abs(x[0]);
				if (lo.IsNaN())
					lo = 1;
			}
		}
    
			return 0.9 * lo * Math.Pow(x.Length,(-0.2));
} 
	}
}
