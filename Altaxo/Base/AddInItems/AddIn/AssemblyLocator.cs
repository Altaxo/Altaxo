﻿// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

#nullable enable
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Altaxo.AddInItems
{
  // Based on http://ayende.com/Blog/archive/2006/05/22/SolvingTheAssemblyLoadContextProblem.aspx
  // This class ensures that assemblies loaded into the LoadFrom context are also available
  // in the Load context.
  internal static class AssemblyLocator
  {
    private static Dictionary<string, Assembly> _assemblies = new Dictionary<string, Assembly>();
    private static bool _isInitialized;

    public static void Init()
    {
      lock (_assemblies)
      {
        if (_isInitialized)
          return;
        _isInitialized = true;
        AppDomain.CurrentDomain.AssemblyLoad += CurrentDomain_AssemblyLoad;
        AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
      }
    }

    private static Assembly? CurrentDomain_AssemblyResolve(object? sender, ResolveEventArgs args)
    {
      if (string.IsNullOrEmpty(args.Name))
        return null;

      lock (_assemblies)
      {
        if (_assemblies.TryGetValue(args.Name, out var assembly))
          return assembly;
      }

      var fileNameParts = args.Name.Split(new char[] { ',' });
      Assembly? result = null;


      // try to load the assembly by the name, from the same directory as the calling assembly
      string path1 = args.RequestingAssembly is { } requestingAssembly ? System.IO.Path.GetDirectoryName(requestingAssembly.Location) ?? string.Empty : string.Empty;

      if (!string.IsNullOrEmpty(path1))
      {
        result = TryGetAssembly(path1, fileNameParts[0]);
      }

      if (result is null)
      {
        var path2 = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location) ?? string.Empty;
        if (!string.IsNullOrEmpty(path2) && path2 != path1)
        {
          result = TryGetAssembly(path2, fileNameParts[0]);
        }
      }

      return result;
    }

    private static Assembly? TryGetAssembly(string path, string fileNameWithoutExtension)
    {
      var fileName = System.IO.Path.Combine(path, fileNameWithoutExtension + ".dll");
      try
      {
        if (System.IO.File.Exists(fileName))
        {
          var assembly = Assembly.LoadFile(fileName);
          if (assembly is not null)
            return assembly;
        }
      }
      catch (Exception)
      {
      }
      return null;
    }

    private static void CurrentDomain_AssemblyLoad(object? sender, AssemblyLoadEventArgs args)
    {
      Assembly assembly = args.LoadedAssembly;

      if (!(assembly.FullName is null))
      {
        lock (_assemblies)
        {
          _assemblies[assembly.FullName] = assembly;
        }
      }
    }
  }
}
