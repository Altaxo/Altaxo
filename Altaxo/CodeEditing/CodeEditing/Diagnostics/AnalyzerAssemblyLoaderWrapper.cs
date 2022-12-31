// Copyright Eli Arbel (no explicit copyright notice in original file)

// Originated from: RoslynPad, RoslynPad.Roslyn, AnalyzerAssemblyLoader.cs


using System.Composition;
using System.Reflection;
using Microsoft.CodeAnalysis;

namespace Altaxo.CodeEditing.Diagnostics
{
  [Export(typeof(IAnalyzerAssemblyLoader)), Shared]
  internal class AnalyzerAssemblyLoaderWrapper : IAnalyzerAssemblyLoader
  {
    private readonly DefaultAnalyzerAssemblyLoader _inner = new();

    public void AddDependencyLocation(string fullPath) => _inner.LoadFromPath(fullPath);
    public Assembly LoadFromPath(string fullPath) => _inner.LoadFromPath(fullPath);
  }
}
