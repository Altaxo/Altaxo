﻿// Copyright Eli Arbel (no explicit copyright notice in original file)

// Originated from: RoslynPad, RoslynPad.Roslyn, QuickInfo/QuickInfoProvider.cs

#if !NoQuickInfo
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.DocumentationComments;
using Microsoft.CodeAnalysis.ExternalAccess.Pythia.Api;
using Microsoft.CodeAnalysis.LanguageService;
using Microsoft.CodeAnalysis.Shared.Extensions;
using Microsoft.CodeAnalysis.Shared.Utilities;
using Roslyn.Utilities;

namespace Altaxo.CodeEditing.QuickInfo;

[Export(typeof(IQuickInfoProvider)), Shared]
[method: ImportingConstructor]
internal sealed class QuickInfoProvider(IDeferredQuickInfoContentProvider contentProvider) : IQuickInfoProvider
{
  private readonly IDeferredQuickInfoContentProvider _contentProvider = contentProvider;

  public async Task<QuickInfoItem?> GetItemAsync(
  Document document,
  int position,
  CancellationToken cancellationToken)
  {
    var tree = await document.GetSyntaxTreeAsync(cancellationToken).ConfigureAwait(false);
    if (tree == null)
    {
      return null;
    }

    var token = await tree.GetTouchingTokenAsync(position, cancellationToken, findInsideTrivia: true).ConfigureAwait(false);

    var state = await GetQuickInfoItemAsync(document, token, position, cancellationToken).ConfigureAwait(false);
    if (state != null)
    {
      return state;
    }

    if (ShouldCheckPreviousToken(token))
    {
      var previousToken = token.GetPreviousToken();

      if ((state = await GetQuickInfoItemAsync(document, previousToken, position, cancellationToken).ConfigureAwait(false)) != null)
      {
        return state;
      }
    }

    return null;
  }

  private static bool ShouldCheckPreviousToken(SyntaxToken token)
  {
    return !token.Parent.IsKind(SyntaxKind.XmlCrefAttribute);
  }

  private async Task<QuickInfoItem?> GetQuickInfoItemAsync(
  Document document,
  SyntaxToken token,
  int position,
  CancellationToken cancellationToken)
  {
    if (token != default &&
      token.Span.IntersectsWith(position))
    {
      var deferredContent = await BuildContentAsync(document, token, cancellationToken).ConfigureAwait(false);
      if (deferredContent != null)
      {
        return new QuickInfoItem(token.Span, deferredContent.Create);
      }
    }

    return null;
  }

  private async Task<IDeferredQuickInfoContent?> BuildContentAsync(
  Document document,
  SyntaxToken token,
  CancellationToken cancellationToken)
  {
    var linkedDocumentIds = document.GetLinkedDocumentIds();

    var modelAndSymbols = await BindTokenAsync(document, token, cancellationToken).ConfigureAwait(false);
    if ((modelAndSymbols.Item2 == null || modelAndSymbols.Item2.Count == 0) && !linkedDocumentIds.Any())
    {
      return null;
    }

    if (!linkedDocumentIds.Any())
    {
      return await CreateContentAsync(document.Project.Solution.Workspace,
          token,
          modelAndSymbols.Item1,
                  modelAndSymbols.Item2!,
          supportedPlatforms: null,
          cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    // Linked files/shared projects: imagine the following when FOO is false
    // #if FOO
    // int x = 3;
    // #endif
    // var y = x$$;
    //
    // 'x' will bind as an error type, so we'll show incorrect information.
    // Instead, we need to find the head in which we get the best binding,
    // which in this case is the one with no errors.

    var candidateProjects = ImmutableArray.CreateBuilder<ProjectId>();
    candidateProjects.Add(document.Project.Id);

    var invalidProjects = ImmutableArray.CreateBuilder<ProjectId>();

    var candidateResults = new List<Tuple<DocumentId, SemanticModel, IList<ISymbol>>>
            {
                Tuple.Create(document.Id, modelAndSymbols.Item1, modelAndSymbols.Item2!)
            };

    foreach (var link in linkedDocumentIds)
    {
      var linkedDocument = document.Project.Solution.GetDocument(link);
      var linkedToken = await FindTokenInLinkedDocument(token, linkedDocument!, cancellationToken).ConfigureAwait(false);

      if (linkedToken != default)
      {
        // Not in an inactive region, so this file is a candidate.
        candidateProjects.Add(link.ProjectId);
        var linkedModelAndSymbols = await BindTokenAsync(linkedDocument!, linkedToken, cancellationToken).ConfigureAwait(false);
        candidateResults.Add(Tuple.Create(link, linkedModelAndSymbols.Item1, linkedModelAndSymbols.Item2));
      }
    }

    // Take the first result with no errors.
    var bestBinding = candidateResults.FirstOrDefault(c => c.Item3.Count > 0 && !ErrorVisitor.ContainsError(c.Item3.First()));

    // Every file binds with errors. Take the first candidate, which is from the current file.
    bestBinding ??= candidateResults.First();

    if (bestBinding.Item3 == null || !bestBinding.Item3.Any())
    {
      return null;
    }

    // We calculate the set of supported projects
    candidateResults.Remove(bestBinding);
    foreach (var candidate in candidateResults)
    {
      // Does the candidate have anything remotely equivalent?
      if (!candidate.Item3.Intersect(bestBinding.Item3, SymbolEqualityComparer.Default).Any())
      {
        invalidProjects.Add(candidate.Item1.ProjectId);
      }
    }

    var supportedPlatforms = new SupportedPlatformData(document.Project.Solution, invalidProjects.ToImmutable(), candidateProjects.ToImmutable());
    return await CreateContentAsync(document.Project.Solution.Workspace, token, bestBinding.Item2, bestBinding.Item3, supportedPlatforms, cancellationToken).ConfigureAwait(false);
  }

  private static async Task<SyntaxToken> FindTokenInLinkedDocument(SyntaxToken token, Document linkedDocument, CancellationToken cancellationToken)
  {
    if (!linkedDocument.SupportsSyntaxTree)
    {
      return default;
    }

    var root = await linkedDocument.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

    // Don't search trivia because we want to ignore inactive regions
    var linkedToken = root!.FindToken(token.SpanStart);

    // The new and old tokens should have the same span?
    if (token.Span == linkedToken.Span)
    {
      return linkedToken;
    }

    return default;
  }

  private async Task<IDeferredQuickInfoContent> CreateContentAsync(
      Workspace workspace,
      SyntaxToken token,
      SemanticModel semanticModel,
      IEnumerable<ISymbol> symbols,
          SupportedPlatformData? supportedPlatforms,
      CancellationToken cancellationToken)
  {
    var descriptionService = workspace.Services.GetLanguageServices(token.Language).GetRequiredService<ISymbolDisplayService>();

    var sections = await descriptionService.ToDescriptionGroupsAsync(semanticModel, token.SpanStart, symbols.AsImmutable(), SymbolDescriptionOptions.Default, cancellationToken).ConfigureAwait(false);

    var mainDescriptionBuilder = new List<TaggedText>();
    if (sections.TryGetValue(SymbolDescriptionGroups.MainDescription, out var value))
    {
      mainDescriptionBuilder.AddRange(value);
    }

    var typeParameterMapBuilder = new List<TaggedText>();
    if (sections.TryGetValue(SymbolDescriptionGroups.TypeParameterMap, out var parts))
    {
      if (!parts.IsDefaultOrEmpty)
      {
        typeParameterMapBuilder.AddLineBreak();
        typeParameterMapBuilder.AddRange(parts);
      }
    }

    var structuralTypesBuilder = new List<TaggedText>();
    if (sections.TryGetValue(SymbolDescriptionGroups.StructuralTypes, out parts))
    {
      if (!parts.IsDefaultOrEmpty)
      {
        structuralTypesBuilder.AddLineBreak();
        structuralTypesBuilder.AddRange(parts);
      }
    }

    var usageTextBuilder = new List<TaggedText>();
    if (sections.TryGetValue(SymbolDescriptionGroups.AwaitableUsageText, out parts))
    {
      if (!parts.IsDefaultOrEmpty)
      {
        usageTextBuilder.AddRange(parts);
      }
    }

    if (supportedPlatforms != null)
    {
      usageTextBuilder.AddRange(supportedPlatforms.ToDisplayParts().ToTaggedText());
    }

    var exceptionsTextBuilder = new List<TaggedText>();
    if (sections.TryGetValue(SymbolDescriptionGroups.Exceptions, out parts))
    {
      if (!parts.IsDefaultOrEmpty)
      {
        exceptionsTextBuilder.AddRange(parts);
      }
    }

    var formatter = workspace.Services.GetLanguageServices(semanticModel.Language).GetRequiredService<IDocumentationCommentFormattingService>();
    var syntaxFactsService = workspace.Services.GetLanguageServices(semanticModel.Language).GetRequiredService<ISyntaxFactsService>();
    var documentationContent = GetDocumentationContent(symbols, sections, semanticModel, token, formatter, syntaxFactsService, cancellationToken);
    var showWarningGlyph = supportedPlatforms != null && supportedPlatforms.HasValidAndInvalidProjects();
    var showSymbolGlyph = true;

    if (workspace.Services.GetLanguageServices(semanticModel.Language).GetRequiredService<ISyntaxFactsService>().IsAwaitKeyword(token) &&
        symbols.First() is INamedTypeSymbol { SpecialType: SpecialType.System_Void })
    {
      documentationContent = _contentProvider.CreateDocumentationCommentDeferredContent(null);
      showSymbolGlyph = false;
    }

    return _contentProvider.CreateQuickInfoDisplayDeferredContent(
        symbol: symbols.First(),
        showWarningGlyph: showWarningGlyph,
        showSymbolGlyph: showSymbolGlyph,
        mainDescription: mainDescriptionBuilder,
        documentation: documentationContent,
        typeParameterMap: typeParameterMapBuilder,
        anonymousTypes: structuralTypesBuilder,
        usageText: usageTextBuilder,
        exceptionText: exceptionsTextBuilder);
  }

  private IDeferredQuickInfoContent GetDocumentationContent(
      IEnumerable<ISymbol> symbols,
      IDictionary<SymbolDescriptionGroups, ImmutableArray<TaggedText>> sections,
      SemanticModel semanticModel,
      SyntaxToken token,
      IDocumentationCommentFormattingService formatter,
      ISyntaxFactsService syntaxFactsService,
      CancellationToken cancellationToken)
  {
    if (sections.TryGetValue(SymbolDescriptionGroups.Documentation, out var value))
    {
      var documentationBuilder = new List<TaggedText>();
      documentationBuilder.AddRange(value);
      return _contentProvider.CreateClassifiableDeferredContent(documentationBuilder);
    }
    if (symbols.Any())
    {
      var symbol = symbols.First().OriginalDefinition;

      // if generating quick info for an attribute, bind to the class instead of the constructor
      if (token.Parent != null &&
          syntaxFactsService.IsNameOfAttribute(token.Parent) &&
          symbol.ContainingType?.IsAttribute() == true)
      {
        symbol = symbol.ContainingType;
      }

      var documentation = symbol.GetDocumentationParts(semanticModel, token.SpanStart, formatter, cancellationToken);

      if (documentation != null)
      {
        return _contentProvider.CreateClassifiableDeferredContent([.. documentation]);
      }
    }

    return _contentProvider.CreateDocumentationCommentDeferredContent(null);
  }

  private async Task<ValueTuple<SemanticModel, IList<ISymbol>>> BindTokenAsync(
      Document document,
      SyntaxToken token,
      CancellationToken cancellationToken)
  {
    var semanticModel = await document.GetSemanticModelForNodeAsync(token.Parent, cancellationToken).ConfigureAwait(false);
    var enclosingType = semanticModel.GetEnclosingNamedType(token.SpanStart, cancellationToken);

    var symbols = semanticModel.GetSemanticInfo(token, document.Project.Solution.Services, cancellationToken).GetSymbols(includeType: true);

    var bindableParent = document.GetLanguageService<ISyntaxFactsService>().TryGetBindableParent(token);
    if (bindableParent != null)
    {
      var overloads = semanticModel.GetMemberGroup(bindableParent, cancellationToken);

      symbols = symbols.Where(IsOk)
          .Where(s => IsAccessible(s, enclosingType!))
          .Concat(overloads)
          .Distinct(SymbolEqualityComparer.Default)
          .ToImmutableArray();

      if (symbols.Any())
      {
        return new ValueTuple<SemanticModel, IList<ISymbol>>(
            semanticModel,
            symbols.First() is ITypeParameterSymbol typeParameter && typeParameter.TypeParameterKind == TypeParameterKind.Cref
                ? SpecializedCollections.EmptyList<ISymbol>()
                : [.. symbols]);
      }

      // Couldn't bind the token to specific symbols.  If it's an operator, see if we can at
      // least bind it to a type.
      var syntaxFacts = document.Project.Services.GetRequiredService<ISyntaxFactsService>();
      if (syntaxFacts.IsOperator(token) && token.Parent != null)
      {
        var typeInfo = semanticModel.GetTypeInfo(token.Parent, cancellationToken);
        if (IsOk(typeInfo.Type!))
        {
          return new ValueTuple<SemanticModel, IList<ISymbol>>(semanticModel, [typeInfo.Type!]);
        }
      }
    }

    return ValueTuple.Create(semanticModel, SpecializedCollections.EmptyList<ISymbol>());
  }

  private static bool IsOk(ISymbol symbol)
  {
    return symbol != null && !symbol.IsErrorType() && !symbol.IsAnonymousFunction();
  }

  private static bool IsAccessible(ISymbol symbol, INamedTypeSymbol within)
  {
    return within == null || symbol.IsAccessibleWithin(within);
  }

  private class ErrorVisitor : SymbolVisitor<bool>
  {
    private static readonly ErrorVisitor s_instance = new();

    public static bool ContainsError(ISymbol symbol)
    {
      return s_instance.Visit(symbol);
    }

    public override bool DefaultVisit(ISymbol symbol)
    {
      return true;
    }

    public override bool VisitAlias(IAliasSymbol symbol)
    {
      return false;
    }

    public override bool VisitArrayType(IArrayTypeSymbol symbol)
    {
      return Visit(symbol.ElementType);
    }

    public override bool VisitEvent(IEventSymbol symbol)
    {
      return Visit(symbol.Type);
    }

    public override bool VisitField(IFieldSymbol symbol)
    {
      return Visit(symbol.Type);
    }

    public override bool VisitLocal(ILocalSymbol symbol)
    {
      return Visit(symbol.Type);
    }

    public override bool VisitMethod(IMethodSymbol symbol)
    {
      foreach (var parameter in symbol.Parameters)
      {
        if (!Visit(parameter))
        {
          return true;
        }
      }

      foreach (var typeParameter in symbol.TypeParameters)
      {
        if (!Visit(typeParameter))
        {
          return true;
        }
      }

      return false;
    }

    public override bool VisitNamedType(INamedTypeSymbol symbol)
    {
      foreach (var typeParameter in symbol.TypeArguments.Concat(symbol.TypeParameters))
      {
        if (Visit(typeParameter))
        {
          return true;
        }
      }

      return symbol.IsErrorType();
    }

    public override bool VisitParameter(IParameterSymbol symbol)
    {
      return Visit(symbol.Type);
    }

    public override bool VisitProperty(IPropertySymbol symbol)
    {
      return Visit(symbol.Type);
    }

    public override bool VisitPointerType(IPointerTypeSymbol symbol)
    {
      return Visit(symbol.PointedAtType);
    }
  }
}

internal interface IDeferredQuickInfoContentProvider
{
  public IDeferredQuickInfoContent CreateQuickInfoDisplayDeferredContent(
        ISymbol symbol,
        bool showWarningGlyph,
        bool showSymbolGlyph,
        IList<TaggedText> mainDescription,
        IDeferredQuickInfoContent documentation,
        IList<TaggedText> typeParameterMap,
        IList<TaggedText> anonymousTypes,
        IList<TaggedText> usageText,
        IList<TaggedText> exceptionText);

  public IDeferredQuickInfoContent CreateDocumentationCommentDeferredContent(string? documentationComment);

  public IDeferredQuickInfoContent CreateClassifiableDeferredContent(IList<TaggedText> content);
}

internal interface IDeferredQuickInfoContent
{
  public object Create();
}
#endif
