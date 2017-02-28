// Copyright Eli Arbel (no explicit copyright notice in original file)

// Originated from: RoslynPad, RoslynPad.Roslyn, Completion/CompletionHelper.cs

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Completion;
using System.Globalization;

namespace Altaxo.CodeEditing.Completion
{
	public sealed class CompletionHelper
	{
		private readonly Microsoft.CodeAnalysis.Completion.CompletionHelper _inner;

		private CompletionHelper(Microsoft.CodeAnalysis.Completion.CompletionHelper inner)
		{
			_inner = inner;
		}

		public static CompletionHelper GetHelper(Document document, CompletionService service)
		{
			return new CompletionHelper(Microsoft.CodeAnalysis.Completion.CompletionHelper.GetHelper(document));
		}

		public bool MatchesFilterText(CompletionItem item, string filterText)
		{
			return _inner.MatchesFilterText(item, filterText, CultureInfo.InvariantCulture);
		}
	}
}