// Copyright Eli Arbel (no explicit copyright notice in original file)

// Originated from: RoslynPad, RoslynPad.Roslyn, AnalyzerAssemblyLoader.cs

using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.CodeEditing.Diagnostics
{
	public class AnalyzerAssemblyLoader : IAnalyzerAssemblyLoader
	{
		public Assembly LoadFromPath(string fullPath)
		{
			return Assembly.Load(AssemblyName.GetAssemblyName(fullPath));
		}

		public void AddDependencyLocation(string fullPath)
		{
		}
	}
}