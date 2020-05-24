// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

// Originated from: Roslyn, Features, Core/Portable/SignatureHelp/SignatureHelpTriggerReason.cs

#if !NoCompletion
namespace Altaxo.CodeEditing.SignatureHelp
{
  public enum SignatureHelpTriggerReason
  {
    InvokeSignatureHelpCommand,
    TypeCharCommand,
    RetriggerCommand
  }
}
#endif
