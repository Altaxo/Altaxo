using System.Collections.Immutable;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Completion;
using Microsoft.CodeAnalysis.Options;
using Microsoft.CodeAnalysis.Shared.Extensions;
using Microsoft.CodeAnalysis.Shared.Extensions.ContextQuery;
using Microsoft.CodeAnalysis.Text;

namespace Altaxo.CodeEditing.Completion.Providers;

/// <summary>
/// Ensures that in source code of kind Regular,i.e. not of kind Script,
/// the #r directive is recognized as a directive in the completion context.
/// </summary>
/// <seealso cref="Microsoft.CodeAnalysis.Completion.CompletionProvider" />
[ExportCompletionProvider("DirectivesCompletionProvider", LanguageNames.CSharp)]
internal class DirectivesCompletionProvider : CompletionProvider
{
  private static readonly ImmutableArray<string> s_directivesName = ["r"];

  public override bool ShouldTriggerCompletion(SourceText text, int caretPosition, CompletionTrigger trigger, OptionSet options)
  {
    return trigger.Kind == CompletionTriggerKind.Insertion && trigger.Character == '#';
  }

  public override async Task ProvideCompletionsAsync(CompletionContext context)
  {
    // We trigger only on '#'
    if (context.Trigger.Character != '#' || context.Trigger.Kind != CompletionTriggerKind.Insertion)
    {
      return;
    }

    // We provide completions only in regular source code, not in script code.
    var originatingDocument = context.Document;
    if (originatingDocument.SourceCodeKind != SourceCodeKind.Regular)
    {
      return;
    }


    var cancellationToken = context.CancellationToken;
    var position = context.Position;
    var semanticModel = await originatingDocument.ReuseExistingSpeculativeModelAsync(position, cancellationToken).ConfigureAwait(false);
    var service = originatingDocument.GetRequiredLanguageService<ISyntaxContextService>();
    var syntaxContext = service.CreateContext(originatingDocument, semanticModel, position, cancellationToken);
    if (!syntaxContext.IsPreProcessorDirectiveContext) // Changed by Lellid, originally: if (!syntaxContext.IsPreProcessorExpressionContext) 
    {
      return;
    }

    foreach (var name in s_directivesName)
    {
      context.AddItem(CommonCompletionItem.Create(
      name,
      displayTextSuffix: "",
      CompletionItemRules.Default,
      glyph: Microsoft.CodeAnalysis.Glyph.Keyword,
      sortText: "_0_" + name));
    }
  }
}
