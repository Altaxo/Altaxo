#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2021 Dr. Dirk Lellinger
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
using System.Collections.Generic;
using Altaxo.Calc.Regression.Nonlinear;
using Altaxo.Main.Services;
using Xunit;

namespace Altaxo.Calc.FitFunctions
{
  /// <summary>
  /// Collects all fit function creator attributes. The tests succeeds, if all creator attributes are unique.
  /// </summary>
  public class FitFunctionCreationUniquenessTest
  {
    /// <summary>
    /// Collects all fit function creator attributes. The tests succeeds if all creator attributes are unique.
    /// </summary>
    [Fact]
    public void Test()
    {
      IEnumerable<Type> classentries = Altaxo.Main.Services.ReflectionService.GetUnsortedClassTypesHavingAttribute(typeof(FitFunctionClassAttribute), true);

      var list = new SortedList<FitFunctionCreatorAttribute, System.Reflection.MethodInfo>();

      foreach (Type definedtype in classentries)
      {
        System.Reflection.MethodInfo[] methods = definedtype.GetMethods(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
        foreach (System.Reflection.MethodInfo method in methods)
        {
          if (method.IsStatic && method.ReturnType != typeof(void) && method.GetParameters().Length == 0)
          {
            object[] attribs = method.GetCustomAttributes(typeof(FitFunctionCreatorAttribute), false);
            foreach (FitFunctionCreatorAttribute creatorattrib in attribs)
            {
              if (list.TryGetValue(creatorattrib, out var alreadyExistingAttribute))
              {
                Assert.False(list.ContainsKey(creatorattrib));
              }
              list.Add(creatorattrib, method);
            }
          }
        }
      }
    }

    /// <summary>
    /// Collects all fit function creator attributes by means of the fit function service
    /// </summary>
    [Fact]
    public void TestCreatorAttributesCollection()
    {
      var list = FitFunctionService.GetFitFunctionCreatorAttributes();
      Assert.NotEmpty(list);
    }

    /// <summary>
    /// Collects all fit function creator attributes. Then creates the fit functions and checks if the number of parameters, of the dependent and independent variables is correct.
    /// </summary>
    [Fact]
    public void TestCreatorNumberOfParameters()
    {
      IEnumerable<Type> classentries = Altaxo.Main.Services.ReflectionService.GetUnsortedClassTypesHavingAttribute(typeof(FitFunctionClassAttribute), true);

      foreach (Type definedtype in classentries)
      {
        System.Reflection.MethodInfo[] methods = definedtype.GetMethods(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
        foreach (System.Reflection.MethodInfo method in methods)
        {
          if (method.IsStatic && method.ReturnType != typeof(void) && method.GetParameters().Length == 0)
          {
            object[] attribs = method.GetCustomAttributes(typeof(FitFunctionCreatorAttribute), false);

            if (attribs.Length == 0)
            {
              continue;
            }

            // now construct the fit function using this method
            var o = method.Invoke(null, null);

            Assert.IsAssignableFrom<IFitFunction>(o);

            foreach (FitFunctionCreatorAttribute creatorattrib in attribs)
            {
              // check if the number of parameters is correct
              var fitfunction = (IFitFunction)o;
              Assert.True(creatorattrib.NumberOfIndependentVariables == fitfunction.NumberOfIndependentVariables, $"{o.GetType()} {method}");
              Assert.True(creatorattrib.NumberOfDependentVariables == fitfunction.NumberOfDependentVariables, $"{o.GetType()} {method}");
              Assert.True(creatorattrib.NumberOfParameters == fitfunction.NumberOfParameters, $"{o.GetType()} {method}");

              // check if the number of parameters is correct
              for (int i = 0; i < fitfunction.NumberOfParameters; i++)
              {
                Assert.False(string.IsNullOrEmpty(fitfunction.ParameterName(i)), $"{o.GetType()} {method} {i}");
                Assert.False(double.IsNaN(fitfunction.DefaultParameterValue(i)), $"{o.GetType()} {method} {i}");
              }

              Exception? exception = null;
              try
              {
                var p = fitfunction.ParameterName(fitfunction.NumberOfParameters);
              }
              catch (Exception ex)
              {
                exception = ex;
              }
              Assert.True(exception is ArgumentOutOfRangeException, $"ParameterName does not throw ArgumentOutOfRangeException {o.GetType()} {method} {exception?.GetType()} {exception?.Message}");

              exception = null;
              try
              {
                var p = fitfunction.DefaultParameterValue(fitfunction.NumberOfParameters);
              }
              catch (ArgumentOutOfRangeException ex)
              {
                exception = ex;
              }
              Assert.True(exception is ArgumentOutOfRangeException, $"DefaultParameterValue does not throw ArgumentOutOfRangeException {o.GetType()} {method} {exception?.GetType()} {exception?.Message}");
            }
          }
        }
      }
    }

    private IEnumerable<IFitFunction> GetAllFitFunctionsCreatedByCreatorAttributes()
    {
      IEnumerable<Type> classentries = Altaxo.Main.Services.ReflectionService.GetUnsortedClassTypesHavingAttribute(typeof(FitFunctionClassAttribute), true);

      foreach (Type definedtype in classentries)
      {
        System.Reflection.MethodInfo[] methods = definedtype.GetMethods(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
        foreach (System.Reflection.MethodInfo method in methods)
        {
          if (method.IsStatic && method.ReturnType != typeof(void) && method.GetParameters().Length == 0)
          {
            object[] attribs = method.GetCustomAttributes(typeof(FitFunctionCreatorAttribute), false);

            if (attribs.Length == 0)
            {
              continue;
            }

            // now construct the fit function using this method
            var o = method.Invoke(null, null);

            Assert.IsAssignableFrom<IFitFunction>(o);
            yield return (IFitFunction)o;
          }
        }
      }
    }

    [Fact]
    public void CheckHardAndSoftLimits()
    {
      foreach (var ff in GetAllFitFunctionsCreatedByCreatorAttributes())
      {
        // Test the hard limits
        var (lowerBounds, upperBounds) = ff.GetParameterBoundariesHardLimit();
        if (lowerBounds is not null)
        {
          Assert.True(lowerBounds.Count == ff.NumberOfParameters, $"Hard limit lowerbounds of {ff.GetType()} has a length of {lowerBounds.Count}, but number of parameters is {ff.NumberOfParameters}");
          for (int i = 0; i < ff.NumberOfParameters; ++i)
          {
            if (lowerBounds[i].HasValue)
            {
              Assert.True(lowerBounds[i].Value <= ff.DefaultParameterValue(i), $"Hard limit lowerbound[{i}] of {ff.GetType()} violated: lowerbound={lowerBounds[i]} but default parameter is {ff.DefaultParameterValue(i)}");
            }
          }
        }
        if (upperBounds is not null)
        {
          Assert.True(lowerBounds.Count == ff.NumberOfParameters, $"Hard limit upperbounds of {ff.GetType()} has a length of {upperBounds.Count}, but number of parameters is {ff.NumberOfParameters}");
          for (int i = 0; i < ff.NumberOfParameters; ++i)
          {
            if (upperBounds[i].HasValue)
            {
              Assert.True(upperBounds[i].Value >= ff.DefaultParameterValue(i), $"Hard limit upperbound[{i}] of {ff.GetType()} violated: upperbound={lowerBounds[i]} but default parameter is {ff.DefaultParameterValue(i)}");
            }
          }
        }

        // Test the soft limits
        (lowerBounds, upperBounds) = ff.GetParameterBoundariesSoftLimit();
        if (lowerBounds is not null)
        {
          Assert.True(lowerBounds.Count == ff.NumberOfParameters, $"Soft limit lowerbounds of {ff.GetType()} has a length of {lowerBounds.Count}, but number of parameters is {ff.NumberOfParameters}");
          for (int i = 0; i < ff.NumberOfParameters; ++i)
          {
            if (lowerBounds[i].HasValue)
            {
              Assert.True(lowerBounds[i].Value <= ff.DefaultParameterValue(i), $"Soft limit lowerbound[{i}] of {ff.GetType()} violated: lowerbound={lowerBounds[i]} but default parameter is {ff.DefaultParameterValue(i)}");
            }
          }
        }
        if (upperBounds is not null)
        {
          Assert.True(lowerBounds.Count == ff.NumberOfParameters, $"Soft limit upperbounds of {ff.GetType()} has a length of {upperBounds.Count}, but number of parameters is {ff.NumberOfParameters}");
          for (int i = 0; i < ff.NumberOfParameters; ++i)
          {
            if (upperBounds[i].HasValue)
            {
              Assert.True(upperBounds[i].Value >= ff.DefaultParameterValue(i), $"Soft limit upperbound[{i}] of {ff.GetType()} violated: upperbound={lowerBounds[i]} but default parameter is {ff.DefaultParameterValue(i)}");
            }
          }
        }
      }
    }

  }
}
