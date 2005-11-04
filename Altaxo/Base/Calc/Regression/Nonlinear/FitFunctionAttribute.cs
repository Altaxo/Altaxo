using System;

namespace Altaxo.Calc.Regression.Nonlinear
{
	/// <summary>
	/// Marks a class as a fit function class.
	/// </summary>
	public class FitFunctionAttribute : System.Attribute, IComparable
	{
    string _name;
    string _category;
    int    _numberOfIndependentVariables;
    int    _numberOfDependentVariables;
    int    _numberOfParameters;

		public FitFunctionAttribute(
      string name,
      string category,
      int numIndependentVariables,
      int numDependentVariables,
      int numParameters)
		{
      _name = name;
      _category = category;
      _numberOfIndependentVariables = numIndependentVariables;
      _numberOfDependentVariables = numDependentVariables;
      _numberOfParameters = numParameters;
		}

    public string Name
    {
      get
      {
        return _name;
      }
    }
    public string Category
    {
      get
      {
        return _category;
      }
    }
    public int NumberOfIndependentVariables
    {
      get
      {
        return _numberOfIndependentVariables;
      }
    }
    public int NumberOfDependentVariables
    {
      get
      {
        return _numberOfDependentVariables;
      }
    }
    public int NumberOfParameters
    {
      get
      {
        return _numberOfParameters;
      }
    }

    int IComparable.CompareTo(object obj)
    {
      FitFunctionAttribute other = (FitFunctionAttribute)obj;
      int result = this._category.CompareTo(other._category);
      if (result == 0)
        result = this._name.CompareTo(other._name);
      return result;
    }
  }
}
