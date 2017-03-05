using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Immutable;

namespace Altaxo.CodeEditing
{
	public class AltaxoWorkspaceForCSharpScripts : AltaxoWorkspaceBase
	{
		public static ImmutableArray<string> DefaultImports { get; } = _defaultReferenceAssemblyTypes.Select(x => x.Namespace).Distinct().ToImmutableArray();

		public AltaxoWorkspaceForCSharpScripts(RoslynHost roslynHost, string workingDirectory, IEnumerable<MetadataReference> staticReferences)
			:
			base(roslynHost, workingDirectory, staticReferences)
		{
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
	}
}