// Copyright Eli Arbel (no explicit copyright notice in original file)

// Originated from: RoslynPad, RoslynPad.Roslyn, Diagnostics/WorkspaceDiagnosticAnalyzerProviderService.cs

#if !NoDiagnostics
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.IO;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Altaxo.CodeEditing.Diagnostics
{
  /// <summary>
  /// Provides diagnostics for CSharp.
  /// </summary>
  /// <seealso cref="Microsoft.CodeAnalysis.Diagnostics.IWorkspaceDiagnosticAnalyzerProviderService" />
  [Export(typeof(IHostDiagnosticAnalyzerPackageProvider))]
  internal sealed class WorkspaceDiagnosticAnalyzerProviderService : IHostDiagnosticAnalyzerPackageProvider
  {
    public ImmutableArray<HostDiagnosticAnalyzerPackage> GetHostDiagnosticAnalyzerPackages()
    {
      var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
      if (path == null)
        throw new ArgumentNullException(nameof(path));
      return new[]
      {
                new HostDiagnosticAnalyzerPackage(LanguageNames.CSharp,
                    ImmutableArray.Create(
                        Path.Combine(path, "Microsoft.CodeAnalysis.dll"),
                        Path.Combine(path, "Microsoft.CodeAnalysis.CSharp.dll")))
            }.AsImmutable();
    }

    public IAnalyzerAssemblyLoader GetAnalyzerAssemblyLoader()
    {
      return new AnalyzerAssemblyLoader();
    }
  }
}
#endif
