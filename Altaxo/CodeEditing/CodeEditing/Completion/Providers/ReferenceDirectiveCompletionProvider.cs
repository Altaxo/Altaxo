﻿// Copyright Eli Arbel (no explicit copyright notice in original file), Apache License Version 2.0, January 2004

// Originated from: RoslynPad, RoslynPad.Roslyn, Completion/Providers/ReferenceDirectiveCompletionProvider.cs
using System;
using System.Collections.Generic;
using System.Composition;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Altaxo.CodeEditing.ReferenceHandling;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Completion;
using Microsoft.CodeAnalysis.CSharp;

namespace Altaxo.CodeEditing.Completion.Providers;

[ExportCompletionProvider("ReferenceDirectiveCompletionProvider", LanguageNames.CSharp)]
[method: ImportingConstructor]
internal class ReferenceDirectiveCompletionProvider([Import(AllowDefault = true)] INuGetCompletionProvider nuGetCompletionProvider) : AbstractReferenceDirectiveCompletionProvider
{
  private static readonly CompletionItemRules s_rules = CompletionItemRules.Create(
      filterCharacterRules: [],
      commitCharacterRules: [],
      enterKeyRule: EnterKeyRule.Never,
      selectionBehavior: CompletionItemSelectionBehavior.SoftSelection);

  private readonly INuGetCompletionProvider _nuGetCompletionProvider = nuGetCompletionProvider;

  private CompletionItem CreateNuGetRoot()
      => CommonCompletionItem.Create(
          displayText: ReferenceDirectiveHelper.NuGetPrefix,
          displayTextSuffix: "",
          rules: s_rules,
          glyph: Microsoft.CodeAnalysis.Glyph.NuGet,
          sortText: "");

  protected override Task ProvideCompletionsAsync(CompletionContext context, string pathThroughLastSlash)
  {
    if (_nuGetCompletionProvider != null &&
        pathThroughLastSlash.StartsWith(ReferenceDirectiveHelper.NuGetPrefix, StringComparison.InvariantCultureIgnoreCase))
    {
      return ProvideNuGetCompletionsAsync(context, pathThroughLastSlash);
    }

    if (string.IsNullOrEmpty(pathThroughLastSlash))
    {
      context.AddItem(CreateNuGetRoot());
    }

    return base.ProvideCompletionsAsync(context, pathThroughLastSlash);
  }

  private async Task ProvideNuGetCompletionsAsync(CompletionContext context, string packageIdAndVersion)
  {
    var (id, version) = ReferenceDirectiveHelper.ParseNuGetReference(packageIdAndVersion);
    var packages = await Task.Run(() => _nuGetCompletionProvider.SearchPackagesAsync(id, exactMatch: version != null, context.CancellationToken), context.CancellationToken).ConfigureAwait(false);

    if (version != null)
    {
      if (packages.Count > 0)
      {
        var package = packages[0];
        var versions = package.Versions;
        if (!string.IsNullOrWhiteSpace(version))
        {
          versions = versions.Where(v => v.StartsWith(version, StringComparison.InvariantCultureIgnoreCase));
        }

        context.AddItems(versions.Select((v, i) =>
            CommonCompletionItem.Create(
                v,
                "",
                s_rules,
                Microsoft.CodeAnalysis.Glyph.NuGet,
                sortText: i.ToString("0000", CultureInfo.InvariantCulture))));
      }
    }
    else
    {
      context.AddItems(packages.Select((p, i) =>
          CommonCompletionItem.Create(
              $"{ReferenceDirectiveHelper.NuGetPrefix} {p.Id}, ",
               "",
              s_rules,
              Microsoft.CodeAnalysis.Glyph.NuGet,
              sortText: i.ToString("0000", CultureInfo.InvariantCulture))));
    }
  }

  protected override bool TryGetStringLiteralToken(SyntaxTree tree, int position, out SyntaxToken stringLiteral, CancellationToken cancellationToken) =>
      tree.TryGetStringLiteralToken(position, SyntaxKind.ReferenceDirectiveTrivia, out stringLiteral, cancellationToken);
}

public interface INuGetCompletionProvider
{
  public Task<IReadOnlyList<INuGetPackage>> SearchPackagesAsync(string searchString, bool exactMatch, CancellationToken cancellationToken);
}

public interface INuGetPackage
{
  public string Id { get; }

  public IEnumerable<string> Versions { get; }
}
