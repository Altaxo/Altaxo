using System;

namespace Altaxo.Calc.Regression.Nonlinear
{
	/// <summary>
	/// Enumeration how to scale the differences between real values (dependent variables) and fitted values.
	/// </summary>
	public enum ErrorScaling
	{
    /// <summary>Mean sum of squares of the differences. This is the default.</summary>
    Norm2,
    /// <summary>Mean sum of absolute values of the differences.</summary>
    Norm1,
    /// <summary>Mean sum of relative differences evaluated as follows.
    /// Abs(y-ym)/Min(Abs(y),Abs(ym)). If both y and ym are zero, the relative difference is set to zero.</summary>
    ScaledNorm1,
	}


  public delegate double ErrorEvaluation(double yo, double yf);
}
