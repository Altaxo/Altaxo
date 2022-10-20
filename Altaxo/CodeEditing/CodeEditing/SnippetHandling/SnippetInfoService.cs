// Copyright Eli Arbel (no explicit copyright notice in original file)

// Originated from: RoslynPad, RoslynPad.Roslyn, Snippets/SnippetInfoService.cs

using System.Collections.Generic;
using System.Composition;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Host.Mef;

namespace Altaxo.CodeEditing.SnippetHandling
{
  [ExportLanguageService(typeof(Microsoft.CodeAnalysis.Snippets.ISnippetInfoService), LanguageNames.CSharp)]
  internal sealed class SnippetInfoService : Microsoft.CodeAnalysis.Snippets.ISnippetInfoService
  {
    private readonly ICSharpEditSnippetInfoService _inner;

    [ImportingConstructor]
    public SnippetInfoService([Import(AllowDefault = true)] ICSharpEditSnippetInfoService inner)
    {
      _inner = inner;
    }

    public IEnumerable<Microsoft.CodeAnalysis.Snippets.SnippetInfo> GetSnippetsIfAvailable()
    {
      return _inner?.GetSnippets().Select(x =>
          new Microsoft.CodeAnalysis.Snippets.SnippetInfo(x.Shortcut, x.Title, x.Description, null))
          ?? Enumerable.Empty<Microsoft.CodeAnalysis.Snippets.SnippetInfo>();
    }

    public bool SnippetShortcutExists_NonBlocking(string shortcut)
    {
      return false;
    }

    public bool ShouldFormatSnippet(Microsoft.CodeAnalysis.Snippets.SnippetInfo snippetInfo)
    {
      return false;
    }
  }
}
