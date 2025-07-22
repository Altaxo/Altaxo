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

//extern alias MCW;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
//using MCW::Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Altaxo.CodeEditing
{
  /// <summary>
  /// Workspace for a regular Dll. Contains exactly one solution, and one C# project.
  /// </summary>
  /// <seealso cref="Microsoft.CodeAnalysis.Workspace" />
  public class AltaxoWorkspaceForCSharpRegularDll : AltaxoWorkspaceBase
  {
    public AltaxoWorkspaceForCSharpRegularDll(
      RoslynHost roslynHost,
      string workingDirectory,
      IEnumerable<MetadataReference> staticReferences)
        : base(roslynHost, staticReferences, workingDirectory)
    {
    }

    public AltaxoWorkspaceForCSharpRegularDll(
    RoslynHost roslynHost,
    string workingDirectory,
    IEnumerable<System.Reflection.Assembly> staticReferences)
      : base(roslynHost, staticReferences, workingDirectory)
    {
    }

    protected override ProjectId CreateInitialProject()
    {
      var compilationOptions = CreateCompilationOptions();
      var parseOptions = CreateParseOptions();

      var name = "Prj" + Guid.NewGuid().ToString();
      var projectId = ProjectId.CreateNewId(name);
      var projectInfo = ProjectInfo.Create(
        projectId,
        VersionStamp.Create(),
        name, // project name
        name, // assembly name
        LanguageNames.CSharp, // language
        parseOptions: parseOptions,
        compilationOptions: compilationOptions,
        metadataReferences: StaticReferences
        );

      var newSolution = CurrentSolution.AddProject(projectInfo);
      base.SetCurrentSolution(newSolution);

      return projectId;
    }

    private CSharpCompilationOptions CreateCompilationOptions()
    {
      var compilationOptions = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary,
          usings: null,
          allowUnsafe: true,
          sourceReferenceResolver: new SourceFileResolver(ImmutableArray<string>.Empty, WorkingDirectory)
        );
      return compilationOptions;
    }

    private CSharpParseOptions CreateParseOptions()
    {
      return new CSharpParseOptions(
                  languageVersion: LanguageVersion.Preview,
                  kind: SourceCodeKind.Regular,
                  preprocessorSymbols: PreprocessorSymbols
                  );
    }

    public override Compilation GetCompilation(string assemblyName)
    {
      var project = CurrentSolution.GetProject(ProjectId);

      var parseOptions = CreateParseOptions();
      var trees = project.Documents.Select(document => SyntaxFactory.ParseSyntaxTree(document.GetTextAsync().Result, parseOptions, string.Empty));

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

      var compilation = CSharpCompilation.Create(
        assemblyName, // Assembly name
        trees,
        AllReferences,
        compilationOptions);

      return compilation;
    }
  }
}
