using System;

namespace Altaxo.Calc
{
	/// <summary>
	/// Provides the interface to a function with one double argument, and one resulting double value.
	/// </summary>
	public interface IScalarFunctionDD
	{
		/// <summary>
		/// The function evaluation.
		/// </summary>
		/// <param name="x">The argument of the function.</param>
		/// <returns>The resulting value that the function evaluates.</returns>
		double Function(double x);
	}
}
