// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

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

namespace Altaxo.CodeEditing
{
	/// <summary>
	/// Workspace for a regular Dll. Contains exactly one solution, and one C# project.
	/// </summary>
	/// <seealso cref="Microsoft.CodeAnalysis.Workspace" />
	public class AltaxoWorkspaceForCSharpRegularDll : AltaxoWorkspaceBase
	{
		public AltaxoWorkspaceForCSharpRegularDll(RoslynHost roslynHost, string workingDirectory, IEnumerable<MetadataReference> staticReferences)
				: base(roslynHost, workingDirectory, staticReferences)
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

			var newSolution = this.CurrentSolution.AddProject(projectInfo);
			base.SetCurrentSolution(newSolution);

			return projectId;
		}

		private CSharpCompilationOptions CreateCompilationOptions()
		{
			var metadataReferenceResolver = CreateMetadataReferenceResolver(this, WorkingDirectory);
			var compilationOptions = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary,
					usings: null,
					allowUnsafe: true,
					sourceReferenceResolver: new SourceFileResolver(ImmutableArray<string>.Empty, WorkingDirectory),
					metadataReferenceResolver: metadataReferenceResolver);
			return compilationOptions;
		}

		private CSharpParseOptions CreateParseOptions()
		{
			return new CSharpParseOptions(kind: SourceCodeKind.Regular, preprocessorSymbols: PreprocessorSymbols);
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