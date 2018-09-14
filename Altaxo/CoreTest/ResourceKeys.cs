#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
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
using Altaxo;
using NUnit.Framework;

namespace AltaxoTest
{
  [TestFixture]
  internal class ResourceKeys
  {
    [Test]
    public void TestAllKeysPresent()
    {
      var assemblyToTest = System.Reflection.Assembly.GetAssembly(typeof(Altaxo.Calc.Complex));

      foreach (System.Type typeInAssembly in assemblyToTest.GetTypes())
      {
        // we deliberately use BindingFlags.Instance here, because afterwards we want to check that no fields declared as instance variables exist
        var members = typeInAssembly.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);

        foreach (var memberInfo in members)
        {
          if (memberInfo.FieldType == typeof(StringResourceKey))
          {
            if (!memberInfo.IsStatic)
            {
              throw new InvalidOperationException("The field with the StringResourceKeyAttribute must be declared as constant (const string). Field: " + memberInfo.ToString());
            }

            var resourceKey = (StringResourceKey)typeInAssembly.InvokeMember(memberInfo.Name,
              System.Reflection.BindingFlags.GetField |
              System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public |
              System.Reflection.BindingFlags.Static,
              null,
              null,
              new object[] { });

            string resourceString = StringResources.AltaxoCore.GetString(resourceKey, false);

            Assert.That(!string.IsNullOrEmpty(resourceString), string.Format("The resource string for the resource key '{0}' is null or empty", resourceKey));
          }
        }
      }
    }
  }
}
