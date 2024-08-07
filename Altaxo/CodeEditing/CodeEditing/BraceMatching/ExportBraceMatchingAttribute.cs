﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Originated from: Roslyn, EditorFeatures, Core/Extensibility/BraceMatching/ExportBraceMatcherAttribute.cs

#if !NoBraceMatching
using System;
using System.Composition;

namespace Altaxo.CodeEditing.BraceMatching
{
  [MetadataAttribute]
  [AttributeUsage(AttributeTargets.Class)]
  internal class ExportBraceMatcherAttribute : ExportAttribute
  {
    public string Language { get; }

    public ExportBraceMatcherAttribute(string language)
        : base(typeof(IBraceMatcher))
    {
      this.Language = language ?? throw new ArgumentNullException(nameof(language));
    }
  }
}
#endif
