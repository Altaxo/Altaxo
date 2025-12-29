#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2022 Dr. Dirk Lellinger
//
//    This program is free software; you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation; either version 2 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with this program; if not, write to the Free Software
//    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
//
/////////////////////////////////////////////////////////////////////////////

#endregion Copyright

using System;

namespace Altaxo.Calc.Regression.Nonlinear
{
  /// <summary>
  /// Marks a static function that returns a new instance of an <see cref="IFitFunction" /> class.
  /// </summary>
  [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
  public class FitFunctionCreatorAttribute : System.Attribute, IComparable
  {
    private string _name;
    private string _category;
    private string? _descriptionResource;
    private int _numberOfIndependentVariables;
    private int _numberOfDependentVariables;
    private int _numberOfParameters;

    /// <summary>
    /// Initializes a new instance of the <see cref="FitFunctionCreatorAttribute"/> class.
    /// </summary>
    /// <param name="name">The display name of the fit function.</param>
    /// <param name="category">The category used to group the fit function.</param>
    /// <param name="numIndependentVariables">The number of independent variables.</param>
    /// <param name="numDependentVariables">The number of dependent variables.</param>
    /// <param name="numParameters">The number of parameters.</param>
    public FitFunctionCreatorAttribute(
    string name,
    string category,
    int numIndependentVariables,
    int numDependentVariables,
    int numParameters)
      : this(name, category, numIndependentVariables, numDependentVariables, numParameters, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FitFunctionCreatorAttribute"/> class.
    /// </summary>
    /// <param name="name">The display name of the fit function.</param>
    /// <param name="category">The category used to group the fit function.</param>
    /// <param name="numIndependentVariables">The number of independent variables.</param>
    /// <param name="numDependentVariables">The number of dependent variables.</param>
    /// <param name="numParameters">The number of parameters.</param>
    /// <param name="descriptionResource">
    /// An optional resource key that provides a localized description of the fit function.
    /// </param>
    public FitFunctionCreatorAttribute(
      string name,
      string category,
      int numIndependentVariables,
      int numDependentVariables,
      int numParameters,
      string? descriptionResource)
    {
      _name = name;
      _category = category;
      _numberOfIndependentVariables = numIndependentVariables;
      _numberOfDependentVariables = numDependentVariables;
      _numberOfParameters = numParameters;
      _descriptionResource = descriptionResource;
    }

    /// <summary>
    /// Gets the display name of the fit function.
    /// </summary>
    public string Name
    {
      get
      {
        return _name;
      }
    }

    /// <summary>
    /// Gets the category used to group the fit function.
    /// </summary>
    public string Category
    {
      get
      {
        return _category;
      }
    }

    /// <summary>
    /// Gets the resource key for a localized description of the fit function, if available.
    /// </summary>
    public string? DescriptionResource
    {
      get
      {
        return _descriptionResource;
      }
    }

    /// <summary>
    /// Gets the number of independent variables.
    /// </summary>
    public int NumberOfIndependentVariables
    {
      get
      {
        return _numberOfIndependentVariables;
      }
    }

    /// <summary>
    /// Gets the number of dependent variables.
    /// </summary>
    public int NumberOfDependentVariables
    {
      get
      {
        return _numberOfDependentVariables;
      }
    }

    /// <summary>
    /// Gets the number of parameters.
    /// </summary>
    public int NumberOfParameters
    {
      get
      {
        return _numberOfParameters;
      }
    }

    /// <inheritdoc/>
    int IComparable.CompareTo(object? obj)
    {
      if (!(obj is FitFunctionCreatorAttribute other))
        throw new ArgumentException();

      int result = _category.CompareTo(other._category);
      if (result == 0)
        result = _name.CompareTo(other._name);
      return result;
    }
  }
}
