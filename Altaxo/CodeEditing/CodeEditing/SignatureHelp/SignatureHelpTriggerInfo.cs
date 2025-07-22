// Copyright Eli Arbel (no explicit copyright notice in original file)

// Originated from: RoslynPad, RoslynPad.Roslyn, SignatureHelp/SignatureHelpTriggerInfo.cs
#if !NoCompletion && !NoSignatureHelp

namespace Altaxo.CodeEditing.SignatureHelp;

public readonly struct SignatureHelpTriggerInfo(SignatureHelpTriggerReason triggerReason, char? triggerCharacter = null)
{
  internal Microsoft.CodeAnalysis.SignatureHelp.SignatureHelpTriggerInfo Inner { get; } = new Microsoft.CodeAnalysis.SignatureHelp.SignatureHelpTriggerInfo(
          (Microsoft.CodeAnalysis.SignatureHelp.SignatureHelpTriggerReason)triggerReason, triggerCharacter);

  public SignatureHelpTriggerReason TriggerReason => (SignatureHelpTriggerReason)Inner.TriggerReason;

  public char? TriggerCharacter => Inner.TriggerCharacter;
}

#endif
