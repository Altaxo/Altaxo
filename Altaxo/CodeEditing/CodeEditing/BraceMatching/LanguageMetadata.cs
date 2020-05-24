// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

// Originated from: Roslyn, Workspaces, Core/Portable/Workspace/Host/Mef/LanguageMetadata.cs

#if !NoBraceMatching
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.CodeEditing.BraceMatching
{
  /// <summary>
  /// This interface is provided purely to enable some shared logic that handles multiple kinds of
  /// metadata that share the Language property. It should not be used to find exports via MEF,
  /// use LanguageMetadata instead.
  /// </summary>
  public interface ILanguageMetadata
  {
    string Language { get; }
  }

  /// <summary>
  /// MEF metadata class used to find exports declared for a specific language.
  /// </summary>
  public class LanguageMetadata : ILanguageMetadata
  {
    public string Language { get; }

    public LanguageMetadata(IDictionary<string, object> data)
    {
      Language = (string)data["Language"];
    }

    public LanguageMetadata(string language)
    {
      Language = language;
    }
  }
}
#endif
