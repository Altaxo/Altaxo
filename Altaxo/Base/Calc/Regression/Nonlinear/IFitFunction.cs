using System;

namespace Altaxo.Calc.Regression.Nonlinear
{
	/// <summary>
	/// Represents the interface to a fitting function.
	/// </summary>
	public interface IFitFunction
	{
    /// <summary>
    /// Number of independent variables (i.e. x).
    /// </summary>
    int NumberOfIndependentVariables { get; }

    /// <summary>
    /// Number of dependent variables (i.e. y, in Altaxo this is commonly called v like value).
    /// </summary>
    int NumberOfDependentVariables { get; }

    /// <summary>
    /// Number of parameters of this fit function.
    /// </summary>
    int NumberOfParameters { get; }

    /// <summary>
    /// Returns the ith independent variable name.
    /// </summary>
    /// <param name="i">Index of the independent variable.</param>
    /// <returns>The name of the ith independent variable.</returns>
    string IndependentVariableName(int i);

    /// <summary>
    /// Returns the ith dependent variable name.
    /// </summary>
    /// <param name="i">Index of the dependent variable.</param>
    /// <returns>The name of the ith dependent variable.</returns>
    string DependentVariableName(int i);


    /// <summary>
    /// Returns the ith parameter name.
    /// </summary>
    /// <param name="i">Index of the parameter.</param>
    /// <returns>The name of the ith paramter.</returns>
    string ParameterName(int i);


    /// <summary>
    /// This evaluates a function value. 
    /// </summary>
    /// <param name="independent">The independent variables.</param>
    /// <param name="parameters">Parameters for evaluation.</param>
    /// <param name="result">On return, this array contains the one (or more) evaluated
    /// function values at the point (independent).</param>
    void Evaluate(double[] independent, double[] parameters, double[] result);
	}
}
