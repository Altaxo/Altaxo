// Copyright Eli Arbel (no explicit copyright notice in original file)

// Originated from: RoslynPad, RoslynPad.Roslyn, SignatureHelp/SignatureHelpProviderExtensions.cs

#if !NoCompletion && !NoSignatureHelp
using System;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.SignatureHelp;
using Microsoft.CodeAnalysis.Text;

namespace Altaxo.CodeEditing.SignatureHelp
{
  public static class SignatureHelpProviderExtensions
  {
    internal static async Task<bool> IsTriggerCharacter(this ISignatureHelpProvider provider, Document document, int position)
    {
      if (provider == null)
        throw new ArgumentNullException(nameof(provider));

      var text = await document.GetTextAsync().ConfigureAwait(false);
      var character = text.GetSubText(new TextSpan(position, 1))[0];
      return provider.IsTriggerCharacter(character);
    }
  }
}
#endif
