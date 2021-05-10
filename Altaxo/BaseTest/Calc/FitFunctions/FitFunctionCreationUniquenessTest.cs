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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Altaxo.Calc.Regression.Nonlinear;
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
  }
}
