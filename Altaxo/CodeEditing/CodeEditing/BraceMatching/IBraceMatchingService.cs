// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

// Originated from: Roslyn, EditorFeatures, IBraceMatchingService.cs

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Altaxo.CodeEditing.BraceMatching
{
  public interface IBraceMatchingService
  {
    Task<BraceMatchingResult?> GetMatchingBracesAsync(Document document, int position, CancellationToken cancellationToken = default(CancellationToken));
  }

  public struct BraceMatchingResult
  {
    public TextSpan LeftSpan { get; }
    public TextSpan RightSpan { get; }

    public BraceMatchingResult(TextSpan leftSpan, TextSpan rightSpan)
        : this()
    {
      LeftSpan = leftSpan;
      RightSpan = rightSpan;
    }
  }
}
