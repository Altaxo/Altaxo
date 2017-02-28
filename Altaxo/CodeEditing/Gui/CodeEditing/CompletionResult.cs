// Copyright Eli Arbel (no explicit copyright notice in original file), Apache License Version 2.0, January 2004

// Originated from: RoslynPad, RoslynPad.Editor.Windows, CompletionResult.cs

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.CodeEditing
{
	public sealed class CompletionResult
	{
		public CompletionResult(IList<ICompletionDataEx> completionData, IOverloadProviderEx overloadProvider)
		{
			CompletionData = completionData;
			OverloadProvider = overloadProvider;
		}

		public IList<ICompletionDataEx> CompletionData { get; private set; }

		public IOverloadProviderEx OverloadProvider { get; private set; }
	}
}