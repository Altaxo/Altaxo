#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2017 Dr. Dirk Lellinger
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
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;

namespace Altaxo.CodeEditing.ExternalHelp
{
  public class ExternalHelpItem
  {
    /// <summary>
    /// The identity of the assembly where the type/method/etc. is contained.
    /// </summary>
    public AssemblyIdentity AssemblyIdentity { get; private set; }

    /// <summary>
    /// Gets the symbol type character. (E: event, F: field, M: method or constructor, P: property, T: type)
    /// </summary>
    /// <value>
    /// The symbol type character used for documentations.
    /// </value>
    public char SymbolTypeCharacter { get; private set; }

    public bool IsConstructor { get; private set; }

    /// <summary>
    /// The name parts of the name of the type/method/etc.
    /// </summary>
    public IReadOnlyList<string> TypeNameParts { get; private set; }

    public string MemberName { get; }

    public int NumberOfGenericArguments { get; private set; }

    public ExternalHelpItem(AssemblyIdentity assemblyIdentity, IEnumerable<string> typeNameParts, string memberName, char symbolTypeChar, bool isConstructor, int numberOfGenericArguments)
    {
      AssemblyIdentity = assemblyIdentity;
      TypeNameParts = typeNameParts.ToImmutableArray();
      MemberName = memberName;
      SymbolTypeCharacter = symbolTypeChar;
      IsConstructor = isConstructor;
      NumberOfGenericArguments = numberOfGenericArguments;
    }

    /// <summary>
    /// Determines whether this help item is about a type that is contained in one of the provided assemblies.
    /// </summary>
    /// <param name="assemblies">The assemblies to test.</param>
    /// <returns>If the help item concerns an issue that is in one of the provided assemblies, then this assembly is returned; otherwise, the return value is null.</returns>
    public System.Reflection.Assembly GetOneOfTheseAssembliesOrNull(IEnumerable<System.Reflection.Assembly> assemblies)
    {
      foreach (var ass in assemblies)
      {
        if (AssemblyIdentity == AssemblyIdentity.FromAssemblyDefinition(ass))
          return ass;
      }
      return null;
    }

    /// <summary>
    /// Gets the documentation reference identifier. This is the name (without extension) of the HTLM file inside a help file
    /// that is created with Sandcastle help file builder.
    /// </summary>
    /// <value>
    /// The documentation reference identifier.
    /// </value>
    public string DocumentationReferenceIdentifier
    {
      get
      {
        string result = SymbolTypeCharacter + "_" + string.Join("_", TypeNameParts);
        if(!string.IsNullOrEmpty(MemberName))
        {
          result += "_" + MemberName;
        }
        return result;
      }
    }
  }
}
