// Copyright Eli Arbel (no explicit copyright notice in original file)

// Originated from: RoslynPad, RoslynPad.Roslyn, Snippets/SnippetInfoService.cs

using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;

namespace Altaxo.CodeEditing.SnippetHandling
{
	[Export(typeof(ICSharpEditSnippetInfoService)), Shared]
	internal sealed class AltaxoSnippetInfoService : ICSharpEditSnippetInfoService
	{
		public SnippetManager SnippetManager { get; } = new SnippetManager();

		public IEnumerable<SnippetInfo> GetSnippets()
		{
			return SnippetManager.Snippets.Select(x => new SnippetInfo(x.Name, x.Name, x.Description));
		}
	}
}