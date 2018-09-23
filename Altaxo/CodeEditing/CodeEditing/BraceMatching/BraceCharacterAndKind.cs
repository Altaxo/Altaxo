// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

// Originated from: Roslyn, EditorFeatures, Core/Implementation/BraceMatching/BraceCharacterAndKind.cs

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.CodeEditing.BraceMatching
{
  public struct BraceCharacterAndKind
  {
    public char Character { get; }
    public int Kind { get; }

    public BraceCharacterAndKind(char character, int kind)
        : this()
    {
      Character = character;
      Kind = kind;
    }
  }
}
