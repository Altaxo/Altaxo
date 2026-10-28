#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2026 Dr. Dirk Lellinger
//
//    This program is free software; you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation; either version 2 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with this program; if not, write to the Free Software
//    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
//
/////////////////////////////////////////////////////////////////////////////

#endregion Copyright

using System.Collections.Generic;
using System.ComponentModel;
using System.Text.Json;

namespace Altaxo.Chat
{
  /// <summary>
  /// Provides the basic tools from the Altaxo data namespace, like creating tables, creating data columns and reading and writing data.
  /// </summary>
  /// <remarks>Each tool provider must be loaded via the .addin file of Altaxo.</remarks>
  public sealed class AltaxoTextCoreToolProvider : AltaxoToolProviderBase
  {
    /// <inheritdoc/>
    public override string Id => "altaxo.text.core";

    [Description("Creates a new empty text document (markdown syntax) in the current project and opens it."), ReadOnly(false)]
    private string CreateTextDocument(
      [Description("Optional: name of the text document to create. Folder names are separated by backslashes, not by forward slashes.")] string? textdocumentName = null
      )
    {
      var name = textdocumentName?.Trim();
      bool nameAlreadyExisted = false;

      if (!string.IsNullOrWhiteSpace(name))
      {
        if (Current.Project.TextDocumentCollection.Contains(name))
        {
          name = Current.Project.TextDocumentCollection.FindNewItemName(name);
          nameAlreadyExisted = true;
        }
      }
      else
      {
        name = Current.Project.TextDocumentCollection.FindNewItemName();
      }

      var document = new Altaxo.Text.TextDocument() { Name = name };
      Current.Project.TextDocumentCollection.Add(document);
      var worksheet = Altaxo.Current.ProjectService.OpenOrCreateViewContentForDocument(document);
      return JsonSerializer.Serialize(new
      {
        Created = true,
        DocumentName = document.Name,
        Message = $"Created a new text document '{document.Name}' and opened it.{(nameAlreadyExisted ? " A text document with the specified name already existed, so a new name was generated." : "")}"
      });
    }

    [Description("Reads the content of a text document (markdown syntax)."), ReadOnly(true)]
    private string ReadMarkdownContentOfTextDocument(
      [Description("Name of the text document to read content from.")] string textdocumentName)
    {
      if (Current.Project.TextDocumentCollection.TryGetValue(textdocumentName, out var document))
      {
        return JsonSerializer.Serialize(new
        {
          MarkdownContent = document.SourceText,
          RevisionNumber = document.SourceTextRevision,
        });
      }
      else
      {
        return Error($"Text document '{textdocumentName}' not found in the current project.");
      }
    }

    [Description("Sets the content of a text document (markdown syntax)."), ReadOnly(false)]
    private string SetMarkdownContentOfTextDocument(
     [Description("Name of the text document to set markdown content for.")] string textdocumentName,
     [Description("The content of the text document to set (markdown syntax).")] string markdownContent)
    {
      if (Current.Project.TextDocumentCollection.TryGetValue(textdocumentName, out var document))
      {
        document.SourceText = markdownContent;
        return JsonSerializer.Serialize(new
        {
          Message = $"The markdown content of text document '{textdocumentName}' has been updated."
        });
      }
      else
      {
        return Error($"Text document '{textdocumentName}' not found in the current project.");
      }
    }

    [Description("Gets the supported markdown extensions and their examples for text documents."), ReadOnly(true)]
    private string GetSupportedMarkdownExtensions()
    {
      var features = new List<(string Feature, string Example)>
      {
        ("Code blocks and inline code", "`inline code` and ```\ncode block\n```"),
        ("Tables", "| Header 1 | Header 2 |\n|----------|----------|\n| Cell 1   | Cell 2   |"),
        ("Inline formulas (LaTeX support)", "$E=mc^2$"),
        ("Paragraph formulas (LaTeX support)", "$$\nE=mc^2\n$$"),
        ("Embedded graphs (relative to this text document's folder)", "![](graph:MyGraphName)"),
        ("Embedded graphs (absolute path)", "![](graph//:/path/to/MyGraphName)"),
        ("Horizontal rules", "---"),
        ("Task lists", "- [ ] Task 1\n- [x] Task 2"),
      };
      return JsonSerializer.Serialize(new
      {
        SupportedMarkdownFeatures = features
      });
    }
  }
}
