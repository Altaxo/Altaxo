// Copyright Eli Arbel (no explicit copyright notice in original file)

// Originated from: RoslynPad, RoslynPad.Roslyn, Completion/CompletionHelper.cs

#if !NoCompletion
extern alias MCW;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCW::Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Completion;

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
      return _inner.MatchesPattern(item.FilterText, filterText, CultureInfo.InvariantCulture);
    }
  }
}
#endif
