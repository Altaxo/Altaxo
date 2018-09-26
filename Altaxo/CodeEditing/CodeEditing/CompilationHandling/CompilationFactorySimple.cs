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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Altaxo.Gui.CodeEditing;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Scripting;

namespace Altaxo.CodeEditing.CompilationHandling
{
  public class CompilationFactorySimple
  {
    private static readonly string _globalAssemblyNamePrefix = "\u211B\u2118*" + Guid.NewGuid();

    private readonly InteractiveAssemblyLoader _assemblyLoader;

    public OutputKind OutputKind { get; }

    public Platform Platform { get; }

    public ImmutableArray<MetadataReference> References { get; }

    public MetadataReferenceResolver MetadataResolver { get; }

    public SourceReferenceResolver SourceResolver { get; }

    public ImmutableArray<string> Usings { get; }

    public string FilePath { get; }

    public CSharpParseOptions ParseOptions { get; }

    public CompilationFactorySimple(IAltaxoWorkspace workspace, string code, string workingDirectory, IEnumerable<System.Reflection.Assembly> referencedAssemblies)
    {
      OutputKind = OutputKind.DynamicallyLinkedLibrary;
      Platform = Platform.AnyCpu;
      ParseOptions = new CSharpParseOptions(
                          languageVersion: LanguageVersion.Latest,
                          kind: SourceCodeKind.Script,
                          preprocessorSymbols: workspace.PreprocessorSymbols
                          );

      FilePath = string.Empty;
      Usings = ImmutableArray<string>.Empty;

      _assemblyLoader = new InteractiveAssemblyLoader();

      var builder = ImmutableArray.CreateBuilder<MetadataReference>();
      builder.AddRange(referencedAssemblies.Select(assembly => MetadataReference.CreateFromFile(assembly.Location)));
      References = builder.ToImmutableArray();

      // _assemblyLoader = new InteractiveAssemblyLoader();
      MetadataResolver = ScriptMetadataResolver.Default;
      SourceResolver = (workingDirectory != null
                           ? new SourceFileResolver(ImmutableArray<string>.Empty, workingDirectory)
                           : SourceFileResolver.Default);
    }

    /// <summary>
    /// Compiles the code, the tests if a type with the given <paramref name="classNameOfType"/> exist, test if it has a parameterless constructur and implements the types given in <paramref name="expectedTypesToImplement"/>.
    /// </summary>
    /// <param name="code">The source code(s).</param>
    /// <param name="classNameOfType">Expected name of a type that should exist in the compiled assembly. This type must have a public parameterless constructor.</param>
    /// <param name="expectedTypesToImplement">Types that the compiled type must implement.</param>
    /// <returns>If successfull, returns a class with contains the source code and the compiled type.</returns>
    /// <exception cref="System.NotImplementedException"></exception>
    public AltaxoCompilationResultWithType GetTypeWithParameterlessConstructorExpectedToImplement(string[] code, string classNameOfType, Type[] expectedTypesToImplement)
    {
      throw new NotImplementedException();
    }

    public AltaxoCompilationResultWithAssembly GetCompilation(string code, string scriptClassName)
    {
      var compilation = GetCompilationFromCode(code, scriptClassName);
      var diagnosticsBag = new DiagnosticBag();
      var assembly = Build(compilation, diagnosticsBag, CancellationToken.None);

      if (null != assembly)
      {
        var scriptClassTypeInfo = assembly.DefinedTypes.FirstOrDefault(typeInfo => typeInfo.Name == scriptClassName || typeInfo.FullName == scriptClassName);
        var type = scriptClassTypeInfo?.UnderlyingSystemType;
      }

      return new AltaxoCompilationResultWithAssembly(new string[] { code }, assembly, diagnosticsBag);
    }

    public static AltaxoCompilationResultWithAssembly GetCompilation(CodeEditorView editControl, string scriptClassName)
    {
      var factory = new CompilationFactorySimple(editControl.Adapter.Workspace, null, null, new System.Reflection.Assembly[] { typeof(object).Assembly });
      return factory.GetCompilation(editControl.Document.Text, scriptClassName);
    }

    public System.Reflection.Assembly Build(Compilation compilation, DiagnosticBag diagnostics, CancellationToken cancellationToken)
    {
      using (var peStream = new MemoryStream())
      {
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

          foreach (var referencedAssembly in compilation.References.Select(
              x => new { Key = x, Value = compilation.GetAssemblyOrModuleSymbol(x) }))
          {
            var path = (referencedAssembly.Key as PortableExecutableReference)?.FilePath;
            if (path != null)
            {
              _assemblyLoader.RegisterDependency(((IAssemblySymbol)referencedAssembly.Value).Identity, path);
            }
          }

          peStream.Position = 0;
          pdbStream.Position = 0;

          return _assemblyLoader.LoadAssemblyFromStream(peStream, pdbStream);
        }
      }
    }

    private Compilation GetCompilationFromCode(string code, string scriptClassName)
    {
      var tree = SyntaxFactory.ParseSyntaxTree(code, ParseOptions, FilePath);

      var references = GetReferences();

      var compilationOptions = new CSharpCompilationOptions(
          OutputKind,
          mainTypeName: string.Empty,
          scriptClassName: null,
          usings: Usings,
          optimizationLevel: OptimizationLevel.Debug, // TODO
          checkOverflow: false,                       // TODO
          allowUnsafe: true,                          // TODO
          platform: Platform,
          warningLevel: 4,
          xmlReferenceResolver: null,
          sourceReferenceResolver: SourceResolver,
          metadataReferenceResolver: MetadataResolver,
          assemblyIdentityComparer: DesktopAssemblyIdentityComparer.Default
      );
      //.WithTopLevelBinderFlags(BinderFlags.IgnoreCorLibraryDuplicatedTypes),

      if (OutputKind == OutputKind.ConsoleApplication || OutputKind == OutputKind.WindowsApplication)
      {
        return CSharpCompilation.Create(
         _globalAssemblyNamePrefix,
         new[] { tree },
         references,
         compilationOptions);
      }

      var compilation = CSharpCompilation.Create(
        _globalAssemblyNamePrefix, // Assembly name
        new[] { tree },
        references,
        compilationOptions);

      /*
			var compilation = CSharpCompilation.CreateScriptCompilation(
							_globalAssemblyNamePrefix,
							tree,
							references,
							compilationOptions,
							returnType: null);
							*/

      return compilation;
    }

    private IEnumerable<MetadataReference> GetReferences()
    {
      var references = ImmutableList.CreateBuilder<MetadataReference>();
      foreach (var reference in References)
      {
        var unresolved = reference as UnresolvedMetadataReference;
        if (unresolved != null)
        {
          var resolved = MetadataResolver.ResolveReference(unresolved.Reference, null, unresolved.Properties);
          if (!resolved.IsDefault)
          {
            references.AddRange(resolved);
          }
        }
        else
        {
          references.Add(reference);
        }
      }
      return references.ToImmutable();
    }
  }
}
