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
      // Anonymous objects are used on purpose here: System.Text.Json serializes the fields of a
      // ValueTuple without their names, which would result in empty json objects.
      var features = new[]
      {
        new
        {
          Feature = "Code blocks and inline code",
          Example = "`inline code`\n\n```\ncode block\n```",
          Remarks = "The opening and the closing fence of a code block must contain the same number of backticks (or tildes).",
        },
        new
        {
          Feature = "Bold, italic and additional emphasis",
          Example = "**bold**, *italic*, x^superscript^, H~subscript~, ~~strikethrough~~, ++inserted++, ==highlighted==",
          Remarks = "Superscript, subscript, strikethrough, inserted and highlighted text are enabled in addition to standard markdown emphasis.",
        },
        new
        {
          Feature = "Piped tables",
          Example = "| Tables        | Are           | Cool  |\n| ------------- |:-------------:| -----:|\n| col 3 is      | right-aligned |  1600 |\n| col 2 is      | centered      |    12 |",
          Remarks = "The alignment of a column is chosen in the separator row: - aligns left, -: aligns right and :-: centers the column.",
        },
        new
        {
          Feature = "Grid tables",
          Example = "+-----------------+------------+\n| Tables          | Are        |\n+===============+============+\n| Cell 1          | Cell 2     |\n+-----------------+------------+",
          Remarks = "Grid tables support multi-line cells and cells spanning several columns or rows.",
        },
        new
        {
          Feature = "Task lists",
          Example = "- [ ] Task 1\n- [x] Task 2",
          Remarks = string.Empty,
        },
        new
        {
          Feature = "Horizontal rules",
          Example = "---",
          Remarks = string.Empty,
        },
        new
        {
          Feature = "Inline formulas (LaTeX support)",
          Example = "$E=mc^2$",
          Remarks = string.Empty,
        },
        new
        {
          Feature = "Paragraph formulas (LaTeX support)",
          Example = "$$\nE=mc^2\n$$",
          Remarks = string.Empty,
        },
        new
        {
          Feature = "Embedded Altaxo graphs (path relative to this text document)",
          Example = "![](graph:MyGraphName)",
          Remarks = string.Empty,
        },
        new
        {
          Feature = "Embedded Altaxo graphs (absolute path within the project)",
          Example = "![](graph://Foo\\MyGraphName)",
          Remarks = "The folder parts of a graph name are separated by backslashes. Forward slashes are accepted as well.",
        },
        new
        {
          Feature = "Local images stored inside the text document",
          Example = "![](local:0CD2A31381F6A7E74CCF217F84ABAF25)",
          Remarks = "Images inserted from the file system or from the clipboard are stored within the document and are referenced by their MD5 hash sum.",
        },
        new
        {
          Feature = "Images from the Altaxo resources (e.g. icons)",
          Example = "![](res:Icons.16x16.ArrangeBottom)",
          Remarks = string.Empty,
        },
        new
        {
          Feature = "Escaping of image and graph names",
          Example = "![](graph://Weather%20Reports%2F2024%2FMyGraphName)",
          Remarks = "Spaces must be written as %20 because a markdown link target may not contain spaces. Folder separators may be written as %2F.",
        },
        new
        {
          Feature = "Size of images and graphs",
          Example = "![](graph://Foo\\MyGraphName){width=5cm}\n![](graph://Foo\\MyGraphName){width=5cm height=4cm}",
          Remarks = "The supported units are mm, cm, in, pt and px. There must be no spaces around the '='. If width and height are given together, the aspect ratio is not preserved.",
        },
        new
        {
          Feature = "Anchors and links within the document",
          Example = "### A heading {#MyAnchor}\n\nAs described in [my anchor](#MyAnchor).",
          Remarks = "Anchors are attached to most block and inline elements by writing {#AnchorName} behind the element.",
        },
        new
        {
          Feature = "Figure and table blocks with caption",
          Example = "^^^\n![](graph://Foo\\MyGraphName){width=5cm} {#An_important_graph}\n^^^ Figure 1: This is a graph",
          Remarks = "The caption may be written in the first ^^^ line (which is common for tables) or in the line below the block. Only one of these two lines may contain caption text.",
        },
        new
        {
          Feature = "Figure renumeration and references to figures",
          Example = "As can be seen in [Fig. 3](#Another_graph) ...",
          Remarks = "The command \"Text → Renumerate figures\" renumbers all figure captions of a category and updates the numbers in the link texts that point into figure blocks. It is also available as a preprocessor option when exporting a document.",
        },
        new
        {
          Feature = "Inclusion of child markdown documents",
          Example = "```{Altaxo child=\"Chapter1\\Text\"}\nChapter1 - Title\n```",
          Remarks = "The text inside the block is optional and only shows the user what is included. Child documents are expanded e.g. when exporting to Maml, to HTML or to an external markdown document.",
        },
        new
        {
          Feature = "Auto links",
          Example = "<https://www.altaxo.com>",
          Remarks = "Web addresses and e-mail addresses enclosed in angle brackets are turned into links.",
        },
        new
        {
          Feature = "Special characters",
          Example = "10 &pound;, 20&nbsp;&deg;C, or directly typed characters like Ω and °",
          Remarks = "HTML entities and literal unicode characters can be used.",
        },
      };

      return JsonSerializer.Serialize(new
      {
        SupportedMarkdownFeatures = features
      });
    }
  }
}
