#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2007 Dr. Dirk Lellinger
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
#endregion

using System;

namespace Altaxo.Calc.Regression.Nonlinear
{
  /// <summary>
  /// Marks a class as containing one or more static functions that create a fit function class.
  /// </summary>
  [AttributeUsage(AttributeTargets.Class)]
  public class FitFunctionClassAttribute : System.Attribute
  {
  }

  /// <summary>
  /// Marks a static function to return a new instance of a <see href="IFitFunction" /> class.
  /// </summary>
  [AttributeUsage(AttributeTargets.Method)]
  public class FitFunctionCreatorAttribute : System.Attribute, IComparable
  {
    string _name;
    string _category;
    string _descriptionResource;
    int    _numberOfIndependentVariables;
    int    _numberOfDependentVariables;
    int    _numberOfParameters;


    public FitFunctionCreatorAttribute(
    string name,
    string category,
    int numIndependentVariables,
    int numDependentVariables,
    int numParameters)
    : this(name,category,numIndependentVariables,numDependentVariables,numParameters,null)
    {
    }

    public FitFunctionCreatorAttribute(
      string name,
      string category,
      int numIndependentVariables,
      int numDependentVariables,
      int numParameters, 
      string descriptionResource)
    {
      _name = name;
      _category = category;
      _numberOfIndependentVariables = numIndependentVariables;
      _numberOfDependentVariables = numDependentVariables;
      _numberOfParameters = numParameters;
      _descriptionResource = descriptionResource;
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

    public string DescriptionResource
    {
      get 
      {
        return _descriptionResource; 
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
      FitFunctionCreatorAttribute other = (FitFunctionCreatorAttribute)obj;
      int result = this._category.CompareTo(other._category);
      if (result == 0)
        result = this._name.CompareTo(other._name);
      return result;
    }
  }
}
