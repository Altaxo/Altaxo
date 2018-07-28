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
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Host;
using Microsoft.CodeAnalysis.Text;
using Roslyn.Utilities;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;
using Altaxo.CodeEditing.Diagnostics;

namespace Altaxo.CodeEditing
{
  public class AltaxoWorkspaceForCSharpScripts : AltaxoWorkspaceBase
  {
    protected static readonly ImmutableArray<Type> _defaultReferenceAssemblyTypes = new[] {
            typeof(object), // mscorlib
						typeof(System.Threading.Thread), // mscorlib
						typeof(System.Threading.Tasks.Task), // mscorlib
						typeof(System.Collections.Generic.List<>), // mscorlib
						typeof(System.Text.RegularExpressions.Regex), // system
						typeof(System.Text.StringBuilder), // mscorlib
						typeof(System.Uri), // system
						typeof(System.Linq.Enumerable), // system.core
						typeof(System.Collections.IEnumerable), // mscorlib
						typeof(System.IO.Path), // mscorlib
						typeof(System.Reflection.Assembly), // mscorlib
				}.ToImmutableArray();

    /// <summary>
    /// Gets a proposal for the default assemblies that should be referenced in compilations.
    /// </summary>
    /// <value>
    /// The default reference assemblies.
    /// </value>
    public static ImmutableArray<Assembly> DefaultReferenceAssemblies { get; protected set; } =
        _defaultReferenceAssemblyTypes.Select(x => x.Assembly).Concat(new[]
        {
                Assembly.Load("System.Runtime, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"),
                typeof(Microsoft.CSharp.RuntimeBinder.Binder).Assembly,
        }).Distinct().ToImmutableArray();

    /// <summary>
    /// Dictionary that holds all references directives that comes from the code (by #r statements).
    /// </summary>
    private readonly ConcurrentDictionary<string, DirectiveInfo> _referencesDirectives;

    public static ImmutableArray<string> DefaultImports { get; } = _defaultReferenceAssemblyTypes.Select(x => x.Namespace).Distinct().ToImmutableArray();

    private static readonly ImmutableArray<MetadataReference> DefaultReferences = DefaultReferenceAssemblies.Select(x => x.Location)
                .Concat(RoslynHost.Instance.TryGetFacadeAssemblies())
                .Select(RoslynHost.Instance.CreateMetadataReference)
                .ToImmutableArray();

    public AltaxoWorkspaceForCSharpScripts(RoslynHost roslynHost, string workingDirectory, IEnumerable<MetadataReference> staticReferences)
      :
      base(roslynHost, staticReferences, workingDirectory)
    {
      _referencesDirectives = new ConcurrentDictionary<string, DirectiveInfo>();
    }

    protected override void Dispose(bool finalize)
    {
      _referencesDirectives.Clear();
      base.Dispose(finalize);
    }

    protected override ProjectId CreateInitialProject()
    {
      var compilationOptions = CreateCompilationOptions();
      var parseOptions = CreateParseOptions();

      var name = "Prj" + Guid.NewGuid().ToString();
      var projectId = ProjectId.CreateNewId(name);
      var projectInfo = ProjectInfo.Create(
        ProjectId,
        VersionStamp.Create(),
        name, // project name
        name, // assembly name
        LanguageNames.CSharp, // language
        parseOptions: parseOptions,
        compilationOptions: compilationOptions.WithScriptClassName(name),
        metadataReferences: StaticReferences
        );

      var newSolution = this.CurrentSolution.AddProject(projectInfo);
      base.SetCurrentSolution(newSolution);

      return projectId;
    }

    private CSharpCompilationOptions CreateCompilationOptions()
    {
      var metadataReferenceResolver = CreateMetadataReferenceResolver(this, WorkingDirectory);
      var compilationOptions = new CSharpCompilationOptions(OutputKind.NetModule,
          usings: DefaultImports,
          allowUnsafe: true,
          sourceReferenceResolver: new SourceFileResolver(ImmutableArray<string>.Empty, WorkingDirectory),
          metadataReferenceResolver: metadataReferenceResolver);
      return compilationOptions;
    }

    private CSharpParseOptions CreateParseOptions()
    {
      return new CSharpParseOptions(kind: SourceCodeKind.Script, preprocessorSymbols: PreprocessorSymbols);
    }

    public override Compilation GetCompilation(string assemblyName)
    {
      var project = this.CurrentSolution.GetProject(ProjectId);

      var parseOptions = CreateParseOptions();
      var trees = project.Documents.Select(document => SyntaxFactory.ParseSyntaxTree(document.GetTextAsync().Result, parseOptions, string.Empty));

      var compilationOptions = new CSharpCompilationOptions(
          OutputKind.DynamicallyLinkedLibrary,
          mainTypeName: null,
          scriptClassName: null,
          usings: DefaultImports,
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

    #region Reference directive handling

    /// <summary>
    /// Gets all references currently references by the project, i.e. the <see cref="StaticReferences"/> plus the references
    /// referenced in the code by #r directives.
    /// </summary>
    /// <value>
    /// All references.
    /// </value>
    public override IEnumerable<MetadataReference> AllReferences
    {
      get
      {
        return this.StaticReferences.Concat(
          _referencesDirectives
            .Where(x => x.Value.IsActive)
            .Select(x => x.Value.MetadataReference)
            .WhereNotNull()
            );
      }
    }

    public virtual ImmutableArray<string> ReferencesDirectives => _referencesDirectives.Select(x => x.Key).ToImmutableArray();

    /// <summary>
    /// Processes the reference directives. Searches the provided document for #r directives. Then it
    /// updates the <see cref="_referencesDirectives"/> dictionary. Directives no longer in the document are marked as passive
    /// in the reference dictionary, all that are there are marked active. If changes have occured, the project is updated
    /// to reference all active references.
    /// </summary>
    /// <param name="document">The document.</param>
    /// <returns></returns>
    public override async Task ProcessReferenceDirectives(Document document)
    {
      var project = document.Project;
      var directives = ((CompilationUnitSyntax)await document.GetSyntaxRootAsync().ConfigureAwait(false))
          .GetReferenceDirectives().Select(x => x.File.ValueText).ToImmutableHashSet();

      var changed = false;
      foreach (var referenceDirective in _referencesDirectives)
      {
        if (referenceDirective.Value.IsActive && !directives.Contains(referenceDirective.Key))
        {
          referenceDirective.Value.IsActive = false;
          changed = true;
        }
      }

      foreach (var directive in directives)
      {
        DirectiveInfo referenceDirective;
        if (_referencesDirectives.TryGetValue(directive, out referenceDirective))
        {
          if (!referenceDirective.IsActive)
          {
            referenceDirective.IsActive = true;
            changed = true;
          }
        }
        else
        {
          if (_referencesDirectives.TryAdd(directive, new DirectiveInfo(ResolveReference(directive))))
          {
            changed = true;
          }
        }
      }

      if (!changed)
        return;

      lock (_referencesDirectives)
      {
        var solution = project.Solution;
        var references =
            _referencesDirectives.Where(x => x.Value.IsActive)
                .Select(x => x.Value.MetadataReference)
                .WhereNotNull();
        var newSolution = solution.WithProjectMetadataReferences(project.Id,
            this.StaticReferences.Concat(references));

        SetCurrentSolution(newSolution);
      }
    }

    protected virtual MetadataReference ResolveReference(string name)
    {
      if (File.Exists(name))
      {
        return RoslynHost.CreateMetadataReference(name);
      }
      try
      {
        var assemblyName = GlobalAssemblyCache.Instance.ResolvePartialName(name);
        if (assemblyName == null)
        {
          return null;
        }
        var assembly = Assembly.Load(assemblyName.ToString());
        return RoslynHost.CreateMetadataReference(assembly.Location);
      }
      catch (Exception)
      {
        return null;
      }
    }

    public virtual bool HasReference(string text)
    {
      DirectiveInfo info;
      if (_referencesDirectives.TryGetValue(text, out info))
      {
        return info.IsActive;
      }
      return false;
    }

    public static MetadataReferenceResolver CreateMetadataReferenceResolver(Workspace workspace, string workingDirectory)
    {
      var resolver = Activator.CreateInstance(
          // can't access this type due to a name collision with Scripting assembly
          // can't use extern alias because of project.json
          // ReSharper disable once AssignNullToNotNullAttribute
          Type.GetType("Microsoft.CodeAnalysis.RelativePathResolver, Microsoft.CodeAnalysis.Workspaces"),
          ImmutableArray<string>.Empty,
          workingDirectory);
      return (MetadataReferenceResolver)Activator.CreateInstance(typeof(WorkspaceMetadataFileReferenceResolver),
          workspace.Services.GetService<IMetadataService>(),
          resolver);
    }

    protected class DirectiveInfo
    {
      public MetadataReference MetadataReference { get; }

      public bool IsActive { get; set; }

      public DirectiveInfo(MetadataReference metadataReference)
      {
        MetadataReference = metadataReference;
        IsActive = true;
      }
    }

    #endregion Reference directive handling
  }
}
