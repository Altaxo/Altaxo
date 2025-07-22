// Copyright Eli Arbel (no explicit copyright notice in original file)

// Originated from: RoslynPad, RoslynPad.Roslyn, SignatureHelp/SignatureHelpTriggerReason.cs
#if !NoCompletion && !NoSignatureHelp

namespace Altaxo.CodeEditing.SignatureHelp;

public enum SignatureHelpTriggerReason
{
  InvokeSignatureHelpCommand,
  TypeCharCommand,
  RetriggerCommand
}

#endif
