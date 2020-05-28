﻿// Copyright Eli Arbel (no explicit copyright notice in original file)

// Originated from: RoslynPad, RoslynPad.Roslyn, SignatureHelp/AggregateSignatureHelpProvider.cs

#if !NoCompletion && !NoSignatureHelp
extern alias MCW;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MCW::Microsoft.CodeAnalysis;
using MCW::Microsoft.CodeAnalysis.Host.Mef;
using MCW::Microsoft.CodeAnalysis.Shared.Utilities;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.SignatureHelp;
using Microsoft.CodeAnalysis.Text;

namespace Altaxo.CodeEditing.SignatureHelp
{
  [Export(typeof(IAxoSignatureHelpProvider)), Shared]
  internal sealed class AggregateSignatureHelpProvider : IAxoSignatureHelpProvider
  {
    private ImmutableArray<Microsoft.CodeAnalysis.SignatureHelp.ISignatureHelpProvider> _providers;

    [ImportingConstructor]
    public AggregateSignatureHelpProvider([ImportMany] IEnumerable<Lazy<Microsoft.CodeAnalysis.SignatureHelp.ISignatureHelpProvider, OrderableLanguageMetadata>> providers)
    {
      _providers = ExtensionOrderer.Order(providers) // maybe not neccessary to order them?
        .Where(x => x.Metadata.Language == LanguageNames.CSharp)
        .Select(x => x.Value).ToImmutableArray();
    }

    public bool IsTriggerCharacter(char ch)
    {
      return _providers.Any(p => p.IsTriggerCharacter(ch));
    }

    public bool IsRetriggerCharacter(char ch)
    {
      return _providers.Any(p => p.IsRetriggerCharacter(ch));
    }

    async Task<SignatureHelpItems> IAxoSignatureHelpProvider.GetItemsAsync(Document document, int position, SignatureHelpTriggerInfo trigger, CancellationToken cancellationToken)
    {
      Microsoft.CodeAnalysis.SignatureHelp.SignatureHelpItems bestItems = null;

      // TODO(cyrusn): We're calling into extensions, we need to make ourselves resilient
      // to the extension crashing.
      foreach (var provider in _providers)
      {
        cancellationToken.ThrowIfCancellationRequested();

        var currentItems = await provider.GetItemsAsync(document, position, trigger, cancellationToken).ConfigureAwait(false);
        if (currentItems != null && currentItems.ApplicableSpan.IntersectsWith(position))
        {
          // If another provider provides sig help items, then only take them if they
          // start after the last batch of items.  i.e. we want the set of items that
          // conceptually are closer to where the caret position is.  This way if you have:
          //
          //  Foo(new Bar($$
          //
          // Then invoking sig help will only show the items for "new Bar(" and not also
          // the items for "Foo(..."
          if (IsBetter(bestItems, currentItems.ApplicableSpan))
          {
            bestItems = currentItems;
          }
        }
      }

      if (bestItems != null)
      {
        // var items = new SignatureHelpItems(bestItems);
        if (bestItems.SelectedItemIndex == null)
        {
          var bestItem = GetBestItem(null, bestItems.Items, bestItems.ArgumentCount, bestItems.ArgumentName, isCaseSensitive: true);
          if (bestItem != null)
          {
            int selectedItemIndex = bestItems.Items.IndexOf(bestItem);
            bestItems = new SignatureHelpItems(bestItems.Items, bestItems.ApplicableSpan, bestItems.ArgumentIndex, bestItems.ArgumentCount, bestItems.ArgumentName, selectedItemIndex);
          }
        }
        return bestItems;
      }
      return null;
    }

    private static bool IsBetter(Microsoft.CodeAnalysis.SignatureHelp.SignatureHelpItems bestItems, TextSpan? currentTextSpan)
    {
      return bestItems == null || currentTextSpan?.Start > bestItems.ApplicableSpan.Start;
    }

    private static SignatureHelpItem GetBestItem(SignatureHelpItem currentItem, ICollection<SignatureHelpItem> filteredItems, int argumentCount, string name, bool isCaseSensitive)
    {
      while (true)
      {
        // If the current item is still applicable, then just keep it.
        if (filteredItems.Contains(currentItem) && IsApplicable(currentItem, argumentCount, name, isCaseSensitive))
        {
          return currentItem;
        }

        // Try to find the first applicable item.  If there is none, then that means the
        // selected parameter was outside the bounds of all methods.  i.e. all methods only
        // went up to 3 parameters, and selected parameter is 3 or higher.  In that case,
        // just pick the very last item as it is closest in parameter count.
        var result = filteredItems.FirstOrDefault(i => IsApplicable(i, argumentCount, name, isCaseSensitive));
        if (result != null)
        {
          return result;
        }

        // if we couldn't find a best item, and they provided a name, then try again without
        // a name.
        if (name != null)
        {
          name = null;
          continue;
        }

        // If we don't have an item that can take that number of parameters, then just pick
        // the last item.  Or stick with the current item if the last item isn't any better.
        var lastItem = filteredItems.Last();
        if (currentItem.IsVariadic || currentItem.Parameters.Length == lastItem.Parameters.Length)
        {
          return currentItem;
        }

        return lastItem;
      }
    }

    private static bool IsApplicable(SignatureHelpItem item, int argumentCount, string name, bool isCaseSensitive)
    {
      // If they provided a name, then the item is only valid if it has a parameter that
      // matches that name.
      if (name != null)
      {
        var comparer = isCaseSensitive ? StringComparer.Ordinal : StringComparer.OrdinalIgnoreCase;
        return item.Parameters.Any(p => comparer.Equals(p.Name, name));
      }

      // An item is applicable if it has at least as many parameters as the selected
      // parameter index.  i.e. if it has 2 parameters and we're at index 0 or 1 then it's
      // applicable.  However, if it has 2 parameters and we're at index 2, then it's not
      // applicable.
      if (item.Parameters.Length >= argumentCount)
      {
        return true;
      }

      // However, if it is variadic then it is applicable as it can take any number of
      // items.
      if (item.IsVariadic)
      {
        return true;
      }

      // Also, we special case 0.  that's because if the user has "Foo(" and foo takes no
      // arguments, then we'll see that it's arg count is 0.  We still want to consider
      // any item applicable here though.
      return argumentCount == 0;
    }
  }
}
#endif
