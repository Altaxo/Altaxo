// Copyright Eli Arbel (no explicit copyright notice in original file)

// Originated from: RoslynPad, RoslynPad.Roslyn, Snippets/SnippetInfoService.cs

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.CodeEditing.SnippetHandling
{
  public interface ICSharpEditSnippetInfoService
  {
    IEnumerable<SnippetInfo> GetSnippets();
  }
}
