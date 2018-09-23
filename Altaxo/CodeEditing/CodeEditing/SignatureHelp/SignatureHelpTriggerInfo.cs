// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

// Originated from: Roslyn, Features, Core/Portable/SignatureHelp/SignatureHelpTriggerInfo.cs

namespace Altaxo.CodeEditing.SignatureHelp
{
  public struct SignatureHelpTriggerInfo
  {
    internal Microsoft.CodeAnalysis.SignatureHelp.SignatureHelpTriggerInfo Inner { get; }

    public SignatureHelpTriggerReason TriggerReason => (SignatureHelpTriggerReason)Inner.TriggerReason;

    public char? TriggerCharacter => Inner.TriggerCharacter;

    public SignatureHelpTriggerInfo(SignatureHelpTriggerReason triggerReason, char? triggerCharacter = null)
    {
      Inner = new Microsoft.CodeAnalysis.SignatureHelp.SignatureHelpTriggerInfo(
          (Microsoft.CodeAnalysis.SignatureHelp.SignatureHelpTriggerReason)triggerReason, triggerCharacter);
    }
  }
}
