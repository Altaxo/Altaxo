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

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Altaxo.CodeEditing.ReferenceHandling;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Scripting.Hosting;

namespace Altaxo.CodeEditing.CompilationHandling
{
  public static class CompilationServiceStatic
  {
    private static readonly InteractiveAssemblyLoader _assemblyLoader = new InteractiveAssemblyLoader();
    private static readonly RoslynHost _roslynHost = new RoslynHost(null);

    public static async Task<AltaxoCompilationResultWithAssembly> GetCompilation(IEnumerable<string> codes, string assemblyName, IEnumerable<Assembly> referenceAssemblies, CancellationToken cancellationToken = default)
    {
      var compilation = await GetCompilationFromCode(codes, assemblyName, referenceAssemblies, _roslynHost, cancellationToken).ConfigureAwait(false);
      var diagnosticsBag = new DiagnosticBag();
      var assembly = Build(compilation, diagnosticsBag, CancellationToken.None);

      if (null != assembly)
      {
        var scriptClassTypeInfo = assembly.DefinedTypes.FirstOrDefault(typeInfo => typeInfo.Name == assemblyName || typeInfo.FullName == assemblyName);
        var type = scriptClassTypeInfo?.UnderlyingSystemType;
      }

      return new AltaxoCompilationResultWithAssembly(codes, assembly, diagnosticsBag);
    }

    public static System.Reflection.Assembly Build(Compilation compilation, DiagnosticBag diagnostics, CancellationToken cancellationToken)
    {
      using (var peStream = new MemoryStream())
      using (var pdbStream = new MemoryStream())
      {
        var emitResult = compilation.Emit(
            peStream: peStream,
            pdbStream: pdbStream,
            cancellationToken: cancellationToken);

        diagnostics.AddRange(emitResult.Diagnostics);

        if (!emitResult.Success)
        {
          return null;
        }

        foreach (var referencedAssembly in compilation.References)
        {
          var path = (referencedAssembly as PortableExecutableReference)?.FilePath;
          if (path != null)
          {
            var assemblySymbol = (IAssemblySymbol)compilation.GetAssemblyOrModuleSymbol(referencedAssembly);
            if (null != assemblySymbol)
            {
              _assemblyLoader.RegisterDependency(assemblySymbol.Identity, path);
            }
            else
            {
              // this can happen if the original reference is to an assembly that is GAC'ed,
              // in this case the new MetaDataReference points to the assembly in the GAC, so its location
              // is different from the original one.
            }
          }
        }

        peStream.Position = 0;
        pdbStream.Position = 0;

        return _assemblyLoader.LoadAssemblyFromStream(peStream, pdbStream);
      }
    }

    /// <summary>
    /// Gets a compilation from code. Here the output type is restricted to DynamicallyLinkedLibrary, Platform to AnyCPU.
    /// </summary>
    /// <param name="codes">The code texts.</param>
    /// <param name="assemblyName">Name of the assembly to generate.</param>
    /// <param name="referenceAssemblies">The assemblies that are referenced by the generated assembly.</param>
    /// <returns></returns>
    public static async Task<Compilation> GetCompilationFromCode(
      IEnumerable<string> codes,
      string assemblyName,
      IEnumerable<Assembly> referenceAssemblies,
      RoslynHost roslynHost,
      CancellationToken cancellationToken = default
      )
    {
      var parseOptions = new CSharpParseOptions(
                              languageVersion: LanguageVersion.Latest,
                              kind: SourceCodeKind.Regular,
                              preprocessorSymbols: new[] { "DEBUG", "TRACE" }
                              );

      var treesStillWithReferenceDirectives = codes.Select(code => SyntaxFactory.ParseSyntaxTree(code, parseOptions, string.Empty));
      var referencesByCode = await ReferenceDirectiveHelper.GetMetadataReferencesAsync(treesStillWithReferenceDirectives, roslynHost, cancellationToken);



      var compilationOptions = new CSharpCompilationOptions(
          OutputKind.DynamicallyLinkedLibrary,
          mainTypeName: null,
          scriptClassName: null,
          usings: ImmutableArray<string>.Empty,
          optimizationLevel: OptimizationLevel.Debug, // TODO
          checkOverflow: false,                       // TODO
          allowUnsafe: true,                          // TODO
          platform: Platform.AnyCpu,
          warningLevel: 4,
          xmlReferenceResolver: null,
          sourceReferenceResolver: null,
          metadataReferenceResolver: null,
          assemblyIdentityComparer: DesktopAssemblyIdentityComparer.Default
      );
      //.WithTopLevelBinderFlags(BinderFlags.IgnoreCorLibraryDuplicatedTypes),

      // var arr = referenceAssemblies.Select(ass => MetadataReference.CreateFromFile(ass.Location)).ToArray();


      // remove the reference directives (#r and #load statements) from the tree, otherwise, the compilation will fail
      var referenceDirectiveRemover = new ReferenceDirectiveFromSyntaxTreeRemover();
      var treesWithoutReferenceDirectives = treesStillWithReferenceDirectives.Select(tree => referenceDirectiveRemover.RemoveReferenceDirectivesFromSyntaxTree(tree));


      // now we should sort out assemblies referenced in the code, but already loaded in the compilation context

      var assemblyIdentitiesAlreadyLoaded = new HashSet<string>();

      foreach(var assName in referenceAssemblies.Where(ass => !string.IsNullOrEmpty(ass.Location)).Select(ass => ass.GetName()))
      {
        assemblyIdentitiesAlreadyLoaded.Add(assName.Name);
        // var version = assName.Version;
      }

      var list2 = new List<MetadataReference>();

      if (referencesByCode.Any())
      {
        // we need to convert the metadataReferences to AssemblyIdentities
        // this is done by creating a temporary compilation, and then extract them from there
        var compilationTemp = CSharpCompilation.Create("Temp")
                                .AddReferences(referencesByCode); // your MetadataReferences

        foreach (var reference in referencesByCode)
        {
          var asmSymbol = compilationTemp.GetAssemblyOrModuleSymbol(reference);
          var asmName = asmSymbol.Name;
          if (!assemblyIdentitiesAlreadyLoaded.Contains(asmName))
          {
            list2.Add(reference);
          }
        }
      }

      // now compile
      var compilation = CSharpCompilation.Create(
        assemblyName, // Assembly name
        treesWithoutReferenceDirectives,
        referenceAssemblies.Where(ass => !string.IsNullOrEmpty(ass.Location)).Select(ass => MetadataReference.CreateFromFile(ass.Location))
        .Concat(list2),
        compilationOptions);

      return compilation;
    }

  }

  internal class AssemblyIdentityByNameComparer : IEqualityComparer<AssemblyIdentity>
  {
    public bool Equals(AssemblyIdentity x, AssemblyIdentity y)
    {
      return x.Name == y.Name;
    }

    public int GetHashCode([DisallowNull] AssemblyIdentity obj)
    {
      return obj.Name.GetHashCode();
    }
  }
}
