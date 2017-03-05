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

using Altaxo.CodeEditing.CompilationHandling;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Altaxo.CodeEditing.CompilationHandling
{
	public static class CompilationServiceStatic
	{
		private static readonly InteractiveAssemblyLoader _assemblyLoader = new InteractiveAssemblyLoader();

		public static AltaxoCompilationResultWithAssembly GetCompilation(IEnumerable<string> codes, string assemblyName, IEnumerable<Assembly> referenceAssemblies)
		{
			var compilation = GetCompilationFromCode(codes, assemblyName, referenceAssemblies);
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
		/// <param name="codes">The code textes.</param>
		/// <param name="assemblyName">Name of the assembly to generate.</param>
		/// <param name="referenceAssemblies">The assemblies that are referenced by the generated assembly.</param>
		/// <returns></returns>
		public static Compilation GetCompilationFromCode(
			IEnumerable<string> codes,
			string assemblyName,
			IEnumerable<Assembly> referenceAssemblies
			)
		{
			var parseOptions = new CSharpParseOptions().WithKind(SourceCodeKind.Regular).WithPreprocessorSymbols(new[] { "DEBUG", "TRACE" });
			var trees = codes.Select(code => SyntaxFactory.ParseSyntaxTree(code, parseOptions, string.Empty));

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
				referenceAssemblies.Select(ass => MetadataReference.CreateFromFile(ass.Location)),
				compilationOptions);

			return compilation;
		}
	}
}