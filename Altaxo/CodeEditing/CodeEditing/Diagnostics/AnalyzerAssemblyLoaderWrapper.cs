// Copyright Eli Arbel (no explicit copyright notice in original file)

// Originated from: RoslynPad, RoslynPad.Roslyn, AnalyzerAssemblyLoader.cs


using System;
using System.Composition;
using System.Reflection;
using Microsoft.CodeAnalysis;

namespace Altaxo.CodeEditing.Diagnostics
{
  [Export(typeof(IAnalyzerAssemblyLoader)), Shared]
  internal class AnalyzerAssemblyLoaderWrapper : IAnalyzerAssemblyLoader
  {
    private readonly DefaultAnalyzerAssemblyLoader _inner = new();

    public void AddDependencyLocation(string fullPath)
    {
      _inner.AddDependencyLocation(fullPath);
    }
    public Assembly LoadFromPath(string fullPath)
    {
      Assembly assembly = null;
      try
      {
        assembly = _inner.LoadFromPath(fullPath);
      }
      catch (Exception)
      {

      }

      // Under NetCore, the inner loader is picky about the version
      // of the Dll, that's why, if it failed, we try to load the Dll by normal methods
      if (assembly is null)
      {
        assembly = Assembly.LoadFrom(fullPath);
      }

      return assembly;
    }
  }
}
