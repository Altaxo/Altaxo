// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
// Originated from: Roslyn, EditorFeatures, Core/IBraceMatchingService.cs

#if !NoBraceMatching
extern alias MCW;
using System.Threading;
using System.Threading.Tasks;
using MCW::Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Altaxo.CodeEditing.BraceMatching
{
  public interface IBraceMatchingService
  {
    Task<BraceMatchingResult?> GetMatchingBracesAsync(Document document, int position, CancellationToken cancellationToken = default);
  }

  public struct BraceMatchingResult
  {
    public TextSpan LeftSpan { get; }
    public TextSpan RightSpan { get; }

    public BraceMatchingResult(TextSpan leftSpan, TextSpan rightSpan)
        : this()
    {
      this.LeftSpan = leftSpan;
      this.RightSpan = rightSpan;
    }
  }
}
#endif
